### 2026-08-14 — 无属性 Buff 不应触发派生生命/魔法刷新

- Symptom: Hiding/MassHiding 的纯状态 Buff 没有任何 stat modifier，却产生了不相关的派生 HP/MP 刷新或额外状态包。
- Root cause: 通用 Buff 应用路径在每次增删 Buff 后都无条件重算并钳制派生生命/魔法，把“Buff 状态变化”和“属性变化”当成同一类副作用。
- Prevention: 只有实际改变会影响 MaxHP/MaxMP 或其他派生属性的 Buff 才执行对应刷新；无属性 Buff 只提交自身状态及其必要的可观察包。
- Verification: Hiding/MassHiding 定向测试和真实 net.Pipe transcript 现锁定单次施法的 `HealthChanged` 数量、`ObjectHidden`/`AddBuff` 顺序及过期包，均通过。

### 2026-08-13 — 缓存目标和延迟投射物必须在命中阶段重验攻城资格

- Symptom: 玩家远程/魔法在开战时成功排入队列后，即使命中前战争结束或攻击者公会成为新 owner，仍会扣除 Conquest 资产生命；Hero 与普通宠物也能保留失效目标继续攻击。
- Root cause: `playerCanAttackMonsterLocked` 只用于请求/选目标阶段，延迟 resolver 和伴侣缓存目标的真正命中阶段没有复用同一门禁。
- Prevention: 所有延迟攻击和跨 tick 目标缓存都执行两阶段校验：选择/排队时校验一次，真正命中前按当前战争、owner、地图和存活状态再次校验；状态变化后必须清空伴侣目标且不产生伤害通知。
- Verification: 回归测试覆盖 Hero/普通宠物选中后战争结束，以及玩家远程/魔法排队后战争结束或 owner 变化；目标 HP、通知和动作队列终态均已锁定，服务端整包测试通过。

### 2026-08-13 — Buff 生命与魔法钳制必须保留逐步包状态

- Symptom: 同一个 Buff 同时降低 MaxHP 和 MaxMP 时，若先算最终状态再发包，两条 `HealthChanged` 会携带相同终态，丢失 Legacy 的 HP 步骤中间状态；逐包持久化又会产生重复写入。
- Root cause: 把连续属性刷新副作用压成一个最终快照，没有分离 wire 中间状态与最终权威持久状态。
- Prevention: 同时变化 HP/MP 时按原调用顺序捕获每一步 payload，先生成 HP 钳制包再生成 MP 钳制包；全部状态完成后只持久化最终快照一次。
- Verification: 回归测试锁定 `HealthChanged(18,20)` 后接 `HealthChanged(18,14)`，并断言只执行一次持久化。

### 2026-08-13 — Hero 解封业务类型必须依据变更前绑定状态

- Symptom: Type 42 封印物品解封后，用“本次是否成功召唤”区分首次 Hero 与仓库 Hero，会把 `NoHero` 地图上的首次绑定误报成“已加入仓库”，并遗漏首次 Hero 的未召唤持久状态。
- Root cause: 业务分支读取了地图门禁后的运行时结果，而 Legacy 的分支条件是解封前是否已有当前 Hero；绑定关系与召唤结果是两个独立状态。
- Prevention: 解封事务前固定捕获 `hadCurrentHero`；提交 auth/world/JSON 后，以它决定首次 Hero 或仓库 Hero 的包形状，再独立依据地图门禁决定是否生成 runtime。首次 Hero 在 `NoHero` 地图必须持久化为已绑定但未召唤。
- Verification: 首次解封、已有 Hero、`NoHero` 和槽位满四条真实会话均断言包序、auth/world/JSON 终态；跨 `NoHero` 地图及断线重登测试锁定未召唤状态不会复活。

### 2026-08-11 — 传送包必须按目标类逐字段对照而非复用相似包

- Symptom: 编写 NPC 传送的 MapChanged payload 初稿时误用了 MapInformation 的天气标志布局，并遗漏了目标类末尾字段；差异审查在测试前发现。
- Root cause: 两个包都包含地图元数据，但 .NET MapChanged 的字段顺序是 Lights + Location + Direction + MapDarkLight + Music + Weather，不能直接套用 MapInformation 的 flags。
- Prevention: 为每个新包从原版 packet 类的 WritePacket 逐字段列出 payload 表，再实现独立 serializer；不要以名称或相邻包推断布局，并为长度、字符串、尾字段和端到端包序列各加断言。
- Verification: 修正后 TestNPCTransportAndTeleportPayloadsMatchLegacyLayout 与 NPC 传送 net.Pipe 测试通过，随后 go test -race ./... 通过。

### 2026-08-11 — MagicInfo 伤害期望必须逐项代入公式

- Symptom: IceStorm 测试把 `MC=4`、`MPowerBase=12`、`PowerBase=14` 的 level-0 伤害写成 19，实际结果为 21。
- Root cause: 手算时漏加了 `round(MPowerBase/4)=3` 的完整项，沿用了另一条魔法的旧期望。
- Prevention: 每个魔法 fixture 先从当前 `MagicInfo` 记录列出基础 MC、MPower、Power、倍率、护甲和 MP cost，再逐项计算伤害与最终 HP；不同 spell 不复用相似数字。
- Verification: IceStorm 期望修正为 21、目标有效伤害 20 后，定向测试通过，随后继续执行全量验证。

### 2026-08-11 — NPC 技能重复判断必须以角色技能列表为准

- Symptom: Go 运行时为了兼容无持久化技能的测试玩家可能注入默认 FireBall；如果 GIVESKILL 只检查运行时 `Magics` map，会把默认技能误判为角色已经学习，或在移除时无法与持久化列表对齐。
- Root cause: 运行时施法表和角色持久化 `Info.Magics` 的职责不同；前者可能包含迁移阶段的 fallback，后者才对应原版 NPC `player.Info.Magics.Any(...)` 判断。
- Prevention: GIVESKILL/REMOVESKILL 的存在性、索引和持久化更新统一以 `SelectInfo.Magics`/`worldPlayer.Character.Magics` 为源；运行时 map 只负责施法门禁和数值。
- Verification: NPC 技能 net.Pipe 测试从持久化 FireBall 学习 ThunderBolt、按索引移除 FireBall，并同时断言 auth 与 world 快照一致。

### 2026-08-15 — Monster AI 新增攻击属性必须先核对实体字段

- Symptom: AI=66 CrazyManworm 分支初版直接访问 `worldMonster.MinMC/MaxMC`，包级编译报字段不存在，行为测试尚未运行。
- Root cause: 依据 Legacy 怪物统计概念推断 Go 运行实体必然保存 MC 字段，没有先读取当前 `worldMonster` 的完整字段和定义统计读取 helper。
- Prevention: 新增 AI 分支引用攻击属性前先核对当前实体结构及 materialize/统计访问路径；缺少字段时复用定义值 helper 或在同一批次完整贯通字段，随后立即运行受影响包仅编译门禁。
- Verification: 修正 MC 读取边界后，将以 `go test ./cmd/crystal-server -run '^$' -count=1` 和 AI=66 双分支定向测试确认编译及行为均恢复绿色。
- Strengthening after recurrence: AI=92 FlameSpear 再次把 MC 当成 `worldMonster` 运行字段，说明新增分支的所有攻击属性（包括 MC/SC）都必须同时核对已有生产 helper 的定义统计读取；包级编译失败时不得继续写测试或文档。
- Verification after recurrence: 本次错误在仅编译门禁捕获且未运行行为测试；改用 `monsterStatValue(info, statMinMC/statMaxMC)` 后，受影响包仅编译恢复绿色。

### 2026-08-15 — HellBomb poison 必须覆盖所有可攻击目标种类

- Symptom: HellBomb 宠物目标已按 AC 扣血，但没有收到 Legacy `PoisonTarget` 对应的 Frozen 状态；玩家路径通过、宠物路径缺失。
- Root cause: 爆炸实现只在玩家循环中添加 poison，未把 `CompleteDeath` 对每个成功 `Attacked` 的 Monster/宠物目标的 poison 分支迁移过来。
- Prevention: 迁移多态范围攻击时先按 `FindAllTargets` 的目标种类展开伤害、毒和包副作用矩阵；玩家与宠物怪物必须分别断言 HP、poison 类型和后续状态包。
- Verification: 将在 HellBomb 回归中锁定玩家与宠物均受 AC 伤害并获得对应 poison，野生怪物仍不受击，随后运行定向与全量门禁。

### 2026-08-15 — 相邻 Monster AI 不能按名称复用状态分支

- Symptom: AI=103 ElementGuard 与 AI=102 IceGuard 都是八格近远混合攻击，若直接复制 IceGuard，会错误发送 Type=1 火击或 Slow/Frozen，并遗漏 ElementGuard 近战独有的 Red poison。
- Root cause: 依据相邻 AI 的攻击形状推断完整效果，没有逐个读取 Legacy 的 `Attack`、`CompleteAttack` 和 `CompleteRangeAttack`；同样的 MAC 防御和延迟不代表状态、payload Type 或随机门相同。
- Prevention: 每个新 AI 建立近战/远程的 payload、伤害统计、防御类型、延迟、毒状态和命中重验表；只有 C# 明确存在的分支才迁移，未设置的 wire 字段保留零值，并用独立 fixture 锁定每条路径。
- Verification: AI=103 定向测试覆盖近战 MAC/Red poison、远程 500ms MC/MAC 无毒、零值 range Type、延迟安全区重验和攻击冷却期间移动；包级编译与服务端回归通过。

### 2026-08-15 — 混合怪物 AI 必须拆分攻击冷却与移动准入

- Symptom: AI=102 IceGuard 在攻击冷却期间被通用 `monsterCanAttack` 门禁提前返回，无法复现 Legacy 仍可追击移动的行为；延迟冰击若只在排队时校验目标，还会在目标进入安全区后错误造成伤害。
- Root cause: 把“当前不能发起攻击”误当成“当前不能处理目标移动”，并把排队时的目标资格当成延迟命中时的最终资格；Slow/Frozen 又需要两个独立概率门和各自的可观察状态包。
- Prevention: 对混合近战/远程 AI 先执行目标有效性和移动分支，再在真正攻击提交点检查攻击冷却；所有延迟动作在 impact tick 重验攻击者、目标、地图、安全区和存活状态；复合状态按 Legacy 顺序分别建模和投递。
- Verification: AI=102 定向测试锁定相邻 `ObjectAttack`/MAC 防御、远程 500ms `ObjectRangeAttack`、Type=0/1 分支、冷却期间 `ObjectWalk`、安全区重验及 Slow/Frozen 观察者 transcript；服务端整包测试通过。

### 2026-08-15 — AI 攻击属性必须复用 worldMonster 的定义统计边界

- Symptom: AI=111 WhiteMammoth 初版包级编译失败，代码直接访问不存在的 `worldMonster.MinMC/MaxMC` 字段，行为测试尚未运行。
- Root cause: 按 Legacy 统计概念假设运行实体保存 MC 标量，未先核对 Go `worldMonster` 的实际字段和现有 `monsterStatValue` 读取路径。
- Prevention: 新增 AI 分支引用 MC/SC 等属性前先读取实体结构及 materialize/统计 helper；缺少运行字段时统一从 `monster.Info` 用真实 stat ID 读取，并在首次 patch 后立即运行受影响包的仅编译门禁。
- Verification: 编译错误在行为测试前被捕获且未写入其他文件；修复后将用包级编译与 WhiteMammoth 定向测试确认统计读取和三条攻击分支。

### 2026-08-15 — BlackHammerCat 线攻击必须保留原目标和 Struck 冷却

- Symptom: AI=116 Type=1 线攻击测试初版只建了一个邻近线目标，实际 `LineAttack` 又命中了两格处的原目标；同一 tick 的第二次命中还没有新的 `ObjectStruck`。
- Root cause: 把 `LineAttack(damage, 2, 300)` 当成只攻击中间目标，且按每次命中都发完整 Struck 包，遗漏了 Legacy 的距离 1/2 全线扫描与 `MonsterStruckReadyAt` 500ms 门禁。
- Prevention: 迁移线/扇形动作时逐距离展开所有有效格（包括原锁定目标），再按接收者和同 tick 的 struck 冷却状态生成公开/私有包；不能按“每个目标四包”简化多次命中。
- Verification: BlackHammerCat 测试现锁定 MC 原目标、距离 1 与距离 2 DC actions，以及两个接收者的广播/冷却包序，定向测试通过。

### 2026-08-15 — RestlessJar Stomp 推退方向必须按攻击者到目标逐点计算

- Symptom: AI=122 Stomp 测试把位于攻击者正北的邻居期望为对角线 `(2,1)`，实际 Legacy-compatible push 到 `(1,1)`，定向测试失败。
- Root cause: 只按“远离攻击者”的直觉估算坐标，未使用 `DirectionFromPoint(CurrentLocation, target.CurrentLocation)` 的正交方向。
- Prevention: 推退测试先固定攻击者/目标坐标和方向哨兵，再用一步 `movePoint` 推导目标位置与反向朝向；正交、对角和同格分别覆盖。
- Verification: 将正北邻居期望修正为 `(1,1)`/Direction=4 后，RestlessJar 定向测试继续验证。

### 2026-08-17 — StoneGolem 的三格中心、值 map 和目标投影必须分别锁定

- Symptom: AI=139 若只按攻击距离推导 Quake 中心、复用 `world.monsters` 的旧副本，或用 nil Hero 夹具，可能出现中心错位、Monster HP 未持久化或测试未真正覆盖 Hero 投影；攻击冷却期还可能错误追加移动包。
- Root cause: Legacy 使用 `PointMove(CurrentLocation, Direction, 3)` 生成中心；Go Monster 表是 value map；Hero 运行实体与 owner-keyed 表、非空 `StoredHero` 是分开的契约；`ProcessTarget` 在 `!CanAttack` 时提前返回，不能把冷却期当作移动回退。
- Prevention: 逐步执行三次 `PointMove`；每次修改 Monster 后写回并从权威 map 回读；Hero fixture 同时设置 owner map key、runtime ObjectID 和非 nil Hero；测试把冷却期“无移动”作为独立可观察边界，并按 Player/Monster/Hero 分开生成命中矩阵。
- Verification: AI=139 世界测试覆盖 25 个有效 Quake、value-map HP 回读、Hero 非命中/单目标攻击、延迟重验和冷却期；真实 `net.Pipe` transcript 锁定中心 ObjectSpell、HP 与 25 个有序移除包。

### 2026-08-17 — OmaCannibal 近战与远程毒物分支必须按 DelayedAction 形状区分

- Symptom: AI=144 初版 resolver 在近战 DC 命中后也加入 Green poison；Legacy 只有带 `poison=true` 的 `CompleteRangeAttack` 远程路径施毒。新增死亡重验夹具还把预先设为 0 的 HP 误断言为初始 100，造成一次定向测试失败。
- Root cause: 只按“有效命中后施毒”的概括迁移，没有沿两个不同 `DelayedAction`/完成函数核对 poison 标志；测试断言把“目标已死亡”与“命中后死亡”混为一类。
- Prevention: 每个 AI 先建立动作构造参数到完成 resolver 的逐分支表，只有 Legacy 明确传入 poison 标志的路径才添加状态；重验 fixture 同时记录 mutation 后的预期 HP 与是否发生伤害，不能固定复用初始生命值断言。
- Verification: AI=144 近战世界测试锁定 AC/Agility 伤害且无 Green poison，远程世界与 `net.Pipe` transcript 锁定 Green poison 首跳和 `Elapsed=1`；地图/安全区/死亡重验测试修正后全部通过。

### 2026-08-17 — 新增 Monster AI 必须同时接入 population 白名单

- Symptom: AI=163 HornedMage 已有常量、dispatch 和处理函数，但世界 tick 没有产生任何攻击包；包级编译无法发现该运行时遗漏。
- Root cause: 只把 AI 接入处理 dispatch，遗漏了 `monsterAICommonPopulation` 的调度白名单，实体因此在进入处理函数前被过滤。
- Prevention: 新增 AI 时建立“常量 → common population → process dispatch → delayed-action resolver”四点接线清单，并在行为测试前至少验证一次真实 `world.tick` 能进入分支。
- Verification: 将 AI=163 加入 population 白名单后，HornedMage 世界近战/远程/传送、Player/宠物/Hero 投影和认证会话测试均产生预期动作；普通重复测试与 race 定向测试通过。

### 2026-08-18 — DragonWarrior 延迟 Player 动作必须归一化 TargetKind

- Symptom: DragonWarrior 首次行为测试中 Player 的普通/半月/Shield Bash 延迟动作均没有伤害，Hero 投影还因实际防御抽样出现 `bound=16` 失败。
- Root cause: 延迟动作沿用 Legacy/Go 的 Player `TargetKind=0` 编码，但新 resolver 直接将 0 传给多态目标查找；Hero fixture 未计入物化装备敏捷。
- Prevention: 所有延迟 resolver 进入多态查找前将 `TargetKind=0` 显式映射为 Player；Player/owned-Monster/Hero fixture 分别记录真实防御随机上界，包括 Hero materialized agility。
- Verification: resolver 已完成 Player kind 归一化，Hero callback 纳入 `bound=16`；DragonWarrior 普通、Halfmoon、Shield Bash、等级门禁和三类投影定向测试通过。

### 2026-08-18 — Kirin Player 中毒模型必须包含 monster-caster 二次抗性检查

- Symptom: Kirin Player IceThrust 的初始模型只安排了一次 poison resistance 抽样，无法解释 `ApplyPoison` 路径中的完整随机流。
- Root cause: 对照 C# 时只看到了 Kirin 的显式抗性/几率门，没有继续核对 `HumanObject.ApplyPoison` 对 Monster caster 的第二次抗性检查。
- Prevention: 迁移每个 poison effect 时同时读取施法者调用点与目标 `ApplyPoison` 实现，按目标 Race 明确列出所有 resistance/chance gate。
- Verification: Go Player fixture 固定并验证 `[10,5,10]`，owned-Monster fixture 验证 `[10,5,5]`；两类测试及 race 定向回归通过。

### 2026-08-18 — FrozenMiner 范围攻击必须在发起时固定目标动作

- Symptom: 若把 FrozenMiner 的范围分支实现为命中时重新扫描一格范围，目标在 1000ms 内移动或新进入范围会改变 Legacy 可观察的受击集合与伤害顺序。
- Root cause: Legacy `Attack()` 先执行一次 `FindAllTargets(1, CurrentLocation)`，再为每个目标分别加入 1000ms 的 `DelayedAction`；延迟完成只重验已保存目标的可攻击性、地图和节点，不重新生成目标列表。多目标分支还以短路表达式决定是否消费 `Next(8)`。
- Prevention: Go 在发起时按 Cell 顺序逐目标保存 `TargetKind/TargetID` 和 80% DC 动作，命中时只做目标重验；随机实现保留 `(Count > 1 && Next(2) == 0) || Next(8) == 0` 的短路消费顺序。
- Verification: FrozenMiner 世界测试在发起后移动第二个 Player 仍得到两个 80% 命中，单目标与多目标 `ObjectAttack` payload、600/1000ms 延迟、owned-Monster/Hero 投影及认证 `net.Pipe` transcript 均通过。

### 2026-08-18 — SnowYeti 固定 DC 伤害必须保留 Random.Next(1)

- Symptom: SnowYeti 近战/远程随机 transcript 初稿只记录了分支 `Next(5)`，没有出现 Legacy 固定 DC 的 `Next(1)`。
- Root cause: 共享 Go `monsterAIPowerLocked` 为 `Min==Max` 直接返回，省略了 Legacy `GetAttackPower` 的 unit-bound 消费。
- Prevention: 迁移固定范围攻击力时使用保留 unit-bound 的 helper，并在 callback 中验证 `Next(1)` 位于分支选择之后。
- Verification: SnowYeti 近战序列固定为 `[5,1]`，远程发起序列固定为 `[5,1,2]`，定向普通测试通过。

### 2026-08-18 — SoulFireBall 延迟 resolver 必须在单一路径内完成伤害与练习

- Symptom: 玩家目标命中后经验仍为 0；接入玩家专用 MAC/抗性路径时又短暂出现玩家被重复扣血，抗性测试先掉血后才返回 miss；专用路径还曾因 `int` 与 `int32` 比较未显式转换而编译失败。
- Root cause: 玩家分支遗漏了 SoulFireBall 的 `levelMagicLocked`；修复时先调用通用伤害再覆盖结果，两个调用都修改了 HP；协议统计值为 `int32`，随机函数返回 `int`。
- Prevention: 延迟目标类型先选择唯一的伤害实现，再执行一次状态 mutation，并在成功命中后统一练习；跨类型统计比较显式转换，目标测试覆盖命中、抗性 miss、HP 和经验。
- Verification: 加入玩家/Hero MAC 区间与魔法抗性处理、单次玩家伤害分支及成功练习后，SoulFireBall 普通/race 专项与相邻魔法回归均通过。

### 2026-08-18 — Trap 延迟完成要保留原始目标引用语义并以 impact 时间计时

- Symptom: Trap 延迟测试首次在原始目标死亡后无法选择同格的第二个怪物；修正实现后，生命周期断言又把 60 秒 expiry 锚定在施法时而非延迟完成时。
- Root cause: Legacy `CompleteSpell(Trap)` 即使原始 `MapObject` 已死亡仍读取其当前位置，再按 Cell 顺序筛选其他 Monster；`ShockTime` 和 `SpellObject.ExpireTime` 都在 500ms action 完成时写入。
- Prevention: 延迟 action 保存并重新读取原始目标的位置，不把原始目标死亡误作整条 action 失败；生命周期测试分别覆盖原始目标死亡、同格 Cell 顺序、`now == ExpireTime` 保留和 `now > ExpireTime` 移除，并以 resolver 的当前时间计算 expiry。
- Verification: Trap resolver 改为完成时重新扫描并在成功后执行第二次 Legacy 练习；定向 Trap 测试及 race 版本覆盖 ObjectSpell、MagicLeveled、静态可见性恢复/移除和精确过期边界并通过。

### 2026-08-18 — 周期 SpellObject 的精确 expiry tick 仍会先执行伤害检查

- Symptom: FireWall 生命周期测试首次把 `now == ExpireTime` 断言成没有任何 tick 通知；实现实际在精确 expiry 保留对象并执行了当轮到期前的周期检查。
- Root cause: Legacy `SpellObject.Process` 先用严格 `Envir.Time > ExpireTime` 判断移除，再处理 `TickTime`，所以精确 expiry 不移除但可能命中；测试只关注保留/移除而忽略了同轮 tick。
- Prevention: 生命周期测试把“精确 expiry 保留”和“超出 expiry 移除”分开；若不想让边界 tick 影响包断言，先把目标移出对象格，避免把周期伤害误判为生命周期失败。
- Verification: FireWall exact-expiry 测试在边界前移走目标，只断言五个对象仍在；`+1ns` 再断言全部移除。

### 2026-08-18 — 跨地图 SpellObject 移除包只发给旧地图观察者

- Symptom: FireWall caster 改到新地图后，测试错误地要求 caster 收到旧地图五个 `ObjectRemove`；实际 cleanup 已完成但 caster 不在旧地图可见范围，收到包的是旧地图观察者。
- Root cause: Legacy `Despawn` 广播以 SpellObject 的 `CurrentMap`/位置筛选接收者，不会向已跨图的 caster 发送旧地图对象移除。
- Prevention: 跨地图生命周期测试同时保留旧地图 observer 与已迁移 caster，分别断言对象状态清空和旧地图 observer 的移除包，不把 caster 作为旧图广播接收者。
- Verification: 增加旧地图 observer 后，FireWall cross-map cleanup 清除五个对象并向 observer 发送五个 `ObjectRemove`。

### 2026-08-18 — AttackMode.All 下自有宠物仍属于可攻击目标

- Symptom: Lightning 线扫描测试预期同格自有宠物被跳过并命中后续怪物，实际先命中宠物。
- Root cause: Legacy `MonsterObject.IsAttackTarget(HumanObject)` 对 `Master == attacker` 返回 `attacker.AMode == All`；`AttackMode.All` 明确允许该自有宠物作为目标。
- Prevention: 迁移目标过滤前逐分支核对 Legacy `IsAttackTarget`，不能把“自有对象”默认推断为友方；同格测试同时验证插入序和只命中首个合法对象。
- Verification: Go 线技能测试按 Legacy 插入序断言宠物受击、后续同格怪物不受击；其余每格一个目标、MAC、范围和延迟断言保持通过。

### 2026-08-18 — Plague 毒伤公式的等级项必须按实际 Level 验算

- Symptom: Go Plague 红毒定向测试把 Level 0 的持续时间/数值写成 9/5，测试失败；实际结果为 5/3。
- Root cause: 将公式 `2*(magic.Level+1)+value/10` 与 `value/15+magic.Level+1` 按 Level 2 心算，忽略了夹具的 Level 0。
- Prevention: 测试期望先从 action 的 `MagicLevel` 和冻结的 `PlagueValue` 逐项代入 Legacy 公式，再断言持续时间、Value、扣蓝等派生结果。
- Verification: 修正为 Level 0 的 5/3 后，Plague 定向世界测试通过。

### 2026-08-19 — FlamingSword 激活扣费与攻击扣费必须分开

- Symptom: FlamingSword 世界测试首次发现激活后攻击又发送一次 `HealthChanged`，MP 从 93 降到 86；Legacy 的 FlamingSword 攻击分支实际只检查运行时标记，不再次扣 MP。
- Root cause: 将 TwinDrake/普通战士技能的“攻击时按目录成本扣 MP”通用逻辑套到了 `HumanObject.Attack` 中与 Thrusting 共用的 FlamingSword 分支；FlamingSword 的成本只在 `SpellToggle` 激活时消费。
- Prevention: 迁移同一职业的技能时，先按 Legacy `Attack` 的 switch 分支逐项标注“激活扣费、攻击扣费、无扣费”三种边界；测试必须同时断言激活后的 MP、攻击 self packet 是否存在和最终 MP。
- Verification: Go admission 对 FlamingSword 返回零攻击成本；世界与 authenticated `net.Pipe` 测试验证 `SpellToggle -> HealthChanged` 激活顺序、+300ms AC 命中和 MP 保持 93，定向回归通过。

### 2026-08-19 — 复用公共伤害函数时必须使用其实际攻击者参数

- Symptom: CounterAttack 定向编译失败，`world.go` 报 `undefined: attacker`。
- Root cause: 在接入公共 `damagePlayerAsLocked` 时按调用方习惯引用了 `attacker`，但该函数用 `killer` 表示实际伤害归属者，函数体不存在 `attacker` 变量。
- Prevention: 修改共享伤害路径前先核对函数签名和 wire attacker/kill owner 的职责；新增逻辑只使用当前作用域中的参数，并为普通玩家与宠物攻击分别保留语义。
- Verification: 将反击触发参数改为 `killer` 后重新运行 CounterAttack 定向编译测试，确认该未定义标识符错误消失。
### 2026-08-19 — Hemorrhage 值映射目标的毒状态必须在副作用后重新加载

- Symptom: Hemorrhage 世界测试中主动被动伤害已正确变为 5、Monster HP 已扣除，但 `Bleeding` poison 列表为空。
- Root cause: `world.monsters` 是值映射；普通攻击路径在 Hemorrhage 写入 map 后仍持有旧 target 快照，后续普通命中收尾再次写回旧值，覆盖了刚加入的毒和 OperateTime 清零。
- Prevention: 任何会修改 Monster/Hero 值对象的攻击前置副作用后，必须在后续伤害/收尾写回前重新加载最新对象；测试同时断言即时伤害、毒列表、OperateTime 和持续 tick，不能只看 HP。
- Verification: 普通玩家攻击在被动副作用后 reload `w.monsters[target.ObjectID]`；Hemorrhage Monster、Player、Hero 世界测试与认证 `net.Pipe` transcript 均通过，并覆盖 Effect 17/18、duration/value、Player/Hero final tick 及 Monster expiry-before-damage。
- Strengthening after recurrence: 使用 `world.tick` 验证 Hero 毒伤时，Hero 的默认零 `ActionReadyAt` 会在同一 tick 自动攻击 Player，污染持续伤害期望；持续效果测试必须冻结非目标 AI 的 action timer，或直接调用对应 poison process。
- Verification after strengthening: Hero fixture 将 `ActionReadyAt`/`OperateReadyAt` 固定到未来，重新运行 Hemorrhage 定向测试，Player/Hero 每 tick 均只扣除 `MaxDC+1`。

### 2026-08-19 — 被动技能迁移必须同时核对属性刷新与成功命中练习

- Symptom: Fencing/SpiritSword 首批迁移已覆盖 `RefreshSkills` 的 Accuracy 加成，但遗漏了 `CompleteAttack` 成功命中后按 `Info.Magics` 顺序练习两项技能。
- Root cause: 差异盘点只搜索了被动技能的 stat-refresh 分支，没有继续追踪 `CompleteAttack` 的 `LevelMagic` 循环及普通/延迟攻击的实际命中边界。
- Prevention: 每个被动技能都必须同时搜索 stat refresh、攻击完成/命中练习和所有延迟 impact 入口；测试至少覆盖属性向量、成功命中经验、未命中不练习、重复 impact 次数及通知顺序。
- Verification: Go 已在普通近战、DoubleSlash 两段、TwinDrake 两段、FlamingSword、Slaying 的成功 impact 后复现该循环，并以世界测试验证普通命中与两段延迟命中各自增加 Fencing/SpiritSword 经验及保持角色技能顺序；HalfMoon/CrossHalfMoon/Thrusting 的直接侧向 `Attacked` 路径保持不练习。

### 2026-08-19 — 特殊法术完成后必须跳过普通目标清理分支

- Symptom: Reincarnation 会话的 `ServerMagic` 把请求目标 ID 发送成 0，尽管 Legacy 的 `S.Magic` 保留死者目标 ID。
- Root cause: Go 在 Reincarnation admission 后继续进入通用攻击目标校验分支，因死者不是普通攻击目标而清空了 `Magic.TargetID`；Legacy 的 Reincarnation 分支不会执行该清理。
- Prevention: 新增不属于普通攻击目标解析的法术时，逐项检查最终 `S.Magic`/`ObjectMagic` 字段是否被通用 fallback 覆盖，并用真实 session transcript 验证目标 ID。
- Verification: 对照 `HumanObject.Magic` 的 Reincarnation 分支，将 Go 通用分支改为排除该法术；双会话回归随后通过，目标 ID、请求/接受包和复活顺序均符合预期。

### 2026-08-19 — @MOVE 的 GM 权限、冷却和传送门槛必须分别迁移

- Symptom: 提交前静态复核发现 Go 初版让 GM 绕过已有 `LastTeleportTime`，并复用了带 `RequiredGroup` 检查的传送原语；同时遗漏了 `TestServer` 对无 Teleport 物品玩家的放行。
- Root cause: 把“权限绕过”误合并为所有后续检查绕过，并未按 Legacy 的命令门槛、无条件冷却、无组门传送顺序建模。
- Prevention: 对命令逐项保留权限、地图限制、冷却、冷却写入和底层移动原语的独立语义；GM 只绕过 Legacy 明确标注的分支，测试服开关单独进入权限条件。
- Verification: Go 实现改为无条件检查冷却、使用无组门传送并支持 `TestServer`；定向 GM 冷却/NoPosition/测试服测试、会话测试和全仓 `go test ./...` 均通过。

### 2026-08-19 — NoDuraLoss 必须分离攻击者武器与受击者装备耐久

- Symptom: 初版共享命中函数无法同时表达“魔法不扣攻击者武器”和“物理攻击即使被护甲完全吸收仍扣武器”，并可能把受击者装备耐久放在错误的命中阶段。
- Root cause: Legacy `HumanObject.Attacked` 在防御命中门通过后、护甲吸收判断前调用 `DamageWeapon`，只有实际掉血后才调用 `DamageDura`；各技能还通过独立 `damageWeapon` 布尔值覆盖默认行为。
- Prevention: 所有共享伤害入口显式传递 `damageWeapon` 与 `magicDefence`；武器扣损放在护甲门前，受击装备扣损放在实际掉血后，并把 Strong、Amulet、婚戒、零耐久即时包和十秒延迟 `DuraChanged` 放在同一个 `DamageItem` 等价边界。
- Verification: 物理/魔法/怪物命中、NoDuraLoss、强韧、护婚戒、十秒刷新和 14 格装备随机消耗测试通过；Go 全量测试、race、vet、build 后续均作为批次验收。

### 2026-08-19 — 双段与特殊战士技能必须显式携带 weapon-durability 上下文

- Symptom: 仅用 `magicDefence` 或 `agilityOnly` 推断 weapon durability 时，DoubleSlash/TwinDrake 的公共第二段会漏扣武器；进一步复核发现 FatalSword 会改变防御类型，但不能改变 HalfMoon/CrossHalfMoon/Thrusting 侧击显式 `false` 或 Slaying/FlamingSword 公共命中的显式 `true`。
- Root cause: Legacy 的 `DelayedAction` 单独保存 `damageWeapon`，它与 `DefenceType` 是两个独立维度；同一技能的两段甚至可以分别为 false/true。
- Prevention: 将 `damageWeapon` 从 action resolver 传到 Player/Monster/Hero 三类目标的最终伤害函数；普通前方命中、Slaying、FlamingSword、DoubleSlash/TwinDrake 公共段传 true，侧击和技能特殊段按 Legacy 显式 false，不从防御类型反推。
- Verification: DoubleSlash/TwinDrake/FlamingSword/CrescentSlash/CounterAttack/ShoulderDash 定向测试及全量 Go 回归通过，且 C# 基线文件未被修改。

### 2026-08-19 — 继承 MonsterObject 行为不能误用同名专用 AI 搜索器

- Symptom: AI=86 初版为了复用相邻实现，把目标搜索接到了 AI=83 Tornado 的 `tornadoFindTargetLocked`；Legacy `ManectricClaw` 实际没有覆盖 `FindTarget`，应沿用 `MonsterObject.FindTarget`。
- Root cause: 按攻击几何相似性选择了专用 helper，没有先核对 C# 子类是否真的覆盖目标搜索生命周期。
- Prevention: 每个 AI 批次先检查子类覆盖的方法列表，再复用“继承行为” helper；只有源类确实覆盖同一虚方法时才共享专用搜索器。
- Verification: 对照 `Server/MirObjects/Monsters/ManectricClaw.cs` 与 `MonsterObject.FindTarget` 后改为 `ancientBringerFindTargetLocked`，Go 编译门禁与 AI=86 定向测试通过。

### 2026-08-20 — Monster 子类构造器计时必须核对 Spawned 覆盖

- Symptom: RedMoonEvil 初版 Go 初始化把首次 `ActionTime` 写成构造器的 300ms，虽然攻击测试通过，却没有覆盖真实出生时序。
- Root cause: Legacy `RedMoonEvil` 构造器先写入 `ActionTime + 300`，随后通用 `MonsterObject.Spawned` 无条件覆盖为 `Envir.Time + 2000`；只有 `AttackTime + AttackSpeed` 保留构造器值。
- Prevention: 迁移带自定义构造器计时的 Monster AI 时，必须同时读取构造器和 `Spawned`/`Respawn` 生命周期，分别验证首次动作与攻击冷却，不把构造器赋值直接当作出生后的最终状态。
- Verification: Go AI=13 初始化改为通用 2s 出生动作门禁、300ms 攻击后的动作冷却；定向世界/会话测试通过。

### 2026-08-20 — BoneSpearman 行攻击不得复用搜索期 Hallucination 过滤

- Symptom: 对照 `MonsterObject.LineAttack` 时发现初版 Go 目标投影 helper 会在攻击行扫描阶段仍排除 Hallucination 期间的 Player；Legacy 只在 `FindTarget` 搜索阶段跳过这类 Player，已锁定目标的 `IsAttackTarget`/`LineAttack` 不重复检查该状态。
- Root cause: 搜索期可见性筛选与攻击期 `IsAttackTarget` 重验证被放进同一个无条件条件式，误把 AI 搜索规则延伸到了已开始的行攻击。
- Prevention: 目标投影 helper 明确区分 `needSight=true` 的搜索和 `needSight=false` 的行攻击；Hallucination/Hidden 只在搜索期应用，攻击期只保留目标类型、地图、生命和安全区等 `IsAttackTarget` 规则。
- Verification: 修正条件后重新运行 AI=29 定向世界测试及既有 BoneSpearman 玩家行攻击测试，均通过。

### 2026-08-20 — BoneLord 会在攻击冷却期继续尝试移动

- Symptom: net.Pipe transcript 在投射命中前收到 `ServerObjectWalk`，初始测试错误地把该时间点断言为空通知。
- Root cause: Legacy `BoneLord.ProcessTarget` 只有成功攻击后立即返回；下一 tick 若 `CanAttack` 仍为 false，会继续执行 `MoveTo(Target.CurrentLocation)`，不同于 BoneSpearman 的提前返回路径。
- Prevention: 为每个 AI 单独核对攻击后下一 tick 的 `ProcessTarget` 分支，不要把相邻 AI 的冷却期移动门禁套用过来；session transcript 要覆盖攻击、冷却移动和延迟命中顺序。
- Verification: transcript 加入预期的 `ObjectWalk`（位置 `(1,0)`、方向 6）后，`go test ./cmd/crystal-server -run 'BoneLord' -count=1 -timeout=60s` 通过。

### 2026-08-20 — MonsterObject 子类必须单独审计构造器可观察状态

- Symptom: VenomSpider 首轮完整目标迁移已覆盖攻击/目标/poison，但复核 Legacy 子类发现其构造器调用 `MonsterObject`，Go 物化路径尚未像 SandWorm 一样随机化出生朝向。
- Root cause: 先关注被攻击方向会在 `Attack` 中被重算，误把构造时朝向视为不可观察；忽略了 bootstrap/出生包在攻击前会暴露该状态。
- Prevention: 每个 Monster AI 批次都要分别核对子类构造器、`Spawn`/bootstrap、respawn 与第一次攻击前状态；凡继承基类随机/固定字段，都添加独立物化断言，不因运行时后续覆盖而跳过。
- Verification: AI=100 物化测试断言朝向始终为 0–7；VenomSpider 定向、race、全量测试、vet 和 build 均通过。

### 2026-08-20 — MinotaurKing 延迟范围攻击只在命中时重验目标有效性

- Symptom: 新增 AI=33 测试把远程攻击的锚点目标移出初始 6 格范围后，仍收到范围命中通知，测试错误地判定该攻击应失效。
- Root cause: Legacy `MinotaurKing.CompleteRangeAttack` 在延迟结算时只检查锚点仍是可攻击目标、仍在当前地图且仍有节点，然后以锚点当前位置执行 `FindAllTargets(3, ...)`；它不会再次检查初始攻击距离。
- Prevention: 移植延迟攻击时分离“攻击时的距离门禁”和“命中时的目标有效性门禁”；目标移远仍应按当前锚点结算，死亡、跨地图或不可攻击才应使整次范围攻击失效。
- Verification: 将测试改为在命中前杀死锚点后，AI=33 定向世界/会话测试验证无命中；代码仍保持攻击创建时 6 格限制与命中时 3 格扫描。
- Strengthening after recurrence: 修正失效夹具后首次重跑仍保留旧的 HP=100 终态断言，导致测试以预期错误失败；改动状态的测试必须同步检查新的终态语义。
- Verification after recurrence: 断言改为 HP=0 后，AI=33 定向世界/会话测试通过。

### 2026-08-20 — TrapRock 受击入口必须覆盖普通、技能和 Monster damage resolver

- Symptom: 仅在普通 Player attack 入口处理 TrapRock 首次受击会遗漏 Warrior/Magic/Monster 攻击路径，无法保持 parent first-hit death 与 child first-hit arming。
- Root cause: Legacy 的 `Attacked`/`Struck` 是多态对象入口，Go 将伤害拆成多个 resolver；没有在共享 resolver 边界统一调用 TrapRock override。
- Prevention: 先定义 `trapRockAttackedLocked` 状态转换，再在 Player normal/Warrior/Magic 与通用 Monster damage helpers 前置调用；父/子状态变化必须写回 value map。
- Verification: TrapRock 受击世界测试覆盖 parent immediate death、child arms parent、Magic/Monster 编译接线，定向包测试通过。


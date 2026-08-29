### 2026-08-20 — Dragon Body Hero 测试夹具必须先建立运行时 Hero

- Symptom: 新增 EvilMirBody Hero 定向测试第一次读取 `world.heroes[ownerID]` 得到 nil，测试失败；生产实现未触发。
- Root cause: `heroTestWorld` 只建立持久化 Hero 数据，未自动召唤 runtime Hero，测试遗漏了 `heroTestSummon` 前置步骤。
- Prevention: Hero 攻击测试先通过现有 summon fixture 建立运行时对象，再设置 TargetID 并驱动攻击；不要把持久化列表当作已召唤 runtime map。
- Verification: 补上 `heroTestSummon` 后 Dragon/EvilMir 定向测试通过。

### 2026-08-18 — AI=181 距离延迟测试必须从夹具坐标推导

- Symptom: WaterDragon 远程动作夹具把 `(2,2)` 到 `(6,2)` 当成 3 格，先写成 650ms 期望，实际 Legacy 距离延迟为 700ms。
- Root cause: 测试期望手工填写了距离常量，没有从攻击者与目标的 Chebyshev 坐标计算出 `max(|dx|,|dy|)`。
- Prevention: 延迟动作测试先由夹具坐标计算距离，再用 `distance*50ms + projectileDelay` 生成期望；攻击 payload 与动作 Due 必须共享同一距离变量。
- Verification: 修正为 4 格后，WaterDragon 近战/远程定向测试通过并同时确认 payload、冷却和延迟命中。

### 2026-08-18 — AI=180 测试夹具必须复现 MaxHP、配置名和对象 ID 状态

- Symptom: SnowWolfKing 定向测试把 HP=65/45/20 判成错误攻击阶段，低血量召唤断言看到了父对象自身，死亡测试还因手工子对象触发普通 AI 初始化而消费了 `Next(3000)`，最后把空动作 slice 错判为非空 pending 状态。
- Root cause: 夹具沿用了 SnowWolf 的 `MaxHP=200`，没有设置 `SnowWolfKingMob` 的精确名称，直接写入对象 map 却未推进 `nextObjectID`，也没有冻结手工奴仆的 AI 门禁；Go tick 保留了空的非 nil slice。
- Prevention: 阶段测试显式设置与断言对应的 `MaxHP`，配置驱动的召唤先注册精确名称，手工插入对象后推进 `nextObjectID` 并设置维护时间，队列断言使用 `len` 而不是 nil slice 身份。
- Verification: 修正夹具后 AI=180 world/session 定向测试通过，攻击阶段、三只召唤、500ms 死亡爆炸和奴仆转宠均稳定复现。

### 2026-08-17 — BoulderSpirit 会话必须冻结无时间门禁 AI 的维护 tick

- Symptom: BoulderSpirit 认证 `net.Pipe` 转录普通运行通过，但 `-race -count=10` 偶发在手工 admission 前已经收到延迟 AC 命中包；连接维护循环抢先完成了 300ms action。
- Root cause: 停止共享 world ticker 不会停止 `serveWithConfig` 每次等待客户端帧前执行的 `world.tick(time.Now)`；BoulderSpirit 覆盖了 Legacy `ProcessAI`，没有可把实体置于未来的普通 Action/Search 时间门禁。
- Prevention: 真实会话中先让维护 AI 关闭，再在持有 world 锁时直接驱动已核对的 Boulder admission 和 `tickMonsterAttackActionsLocked` resolver；通知仍通过认证连接投递，禁止依赖 KeepAlive 响应推断下一次维护 tick 已阻塞。
- Verification: BoulderSpirit world/session 定向测试与 `go test -race ./cmd/crystal-server -run 'BoulderSpirit' -count=10` 均通过，包序稳定为即时 `ObjectDied` 后 300ms 的 `Struck/ObjectStruck/DamageIndicator/HealthChanged`。

### 2026-08-17 — AI=169 生命周期夹具必须隔离维护移动与第二次冷却动作

- Symptom: HornedSorceror 定向 transcript 在预期攻击之外消费了 `Random.Next(2)`，龙卷风过期 tick 还会合法生成第二批 25 个单元。
- Root cause: world tick 在冷却/动作门禁期间仍尝试移动方向抽样；15 秒龙卷风冷却在 17 秒生命周期断言前已到期。
- Prevention: 确定性 callback 允许已解释的维护 `bound=2`；生命周期测试在初始施法后冻结 Action/Attack 时间，避免第二次 AI 动作污染目标生命周期。
- Verification: AI=169 world transcript 与 authenticated `net.Pipe` transcript 普通测试通过，`-race -count=10` 作为批次门禁复核。

### 2026-08-17 — 新增 MonsterSettings 字段必须同步默认值断言（AI=171）

- Symptom: 全仓 Go 测试在服务端回归通过后，`internal/worlddata` 的默认设置测试因新增 Commander 两个名称字段而失败。
- Root cause: 生产默认结构已扩展，但旧的完整结构体字面量断言没有同步新增字段；包级定向测试未覆盖该独立内部包。
- Prevention: 新增持久化/配置结构字段时，检索所有默认值、JSON 兼容和导出断言，并在批次门禁运行 `go test ./...`，不能只运行受影响服务端包。
- Verification: 默认断言补齐 `HornedCommanderMob/BombMob` 后，`go test ./...`、`go vet ./...` 和 `go build ./...` 全部通过。

### 2026-08-17 — AI=165 transcript 随机 callback 必须覆盖延迟伤害阶段

- Symptom: HornedWarrior WideLine admission 已正确消费 DC 的 `bound=11`，但测试把后续 Monster/Hero 延迟 AC+Agility 防御的 `bound=1/16` 误报为 admission 失败。
- Root cause: 同一 world tick 夹具复用了一个只允许攻击阶段上界的随机 callback；`world.tick` 在后续手工 impact tick 仍会调用同一 callback。
- Prevention: 先断言 admission tick 结束时的精确随机序列，再让 callback 显式允许该 transcript 后续可达的防御上界，并分别断言每个阶段的行为结果。
- Verification: AI=165 WideLine Player/owned-Monster/Hero 延迟命中测试与 authenticated `net.Pipe` transcript 在普通和 `-race -count=10` 门禁中通过。

### 2026-08-17 — AI=164 行为夹具必须隔离普通 AI 初始化与实际 Hero 防御抽样

- Symptom: HornedArcher 定向测试第一次分别出现未预期的 `Random.Next(3000)` 和 Hero 防御路径的 `bound=16`，导致确定性上界断言失败。
- Root cause: world tick 会初始化夹具中的普通 AI=0 友军并消费其搜索随机数；Hero 的 `heroEquipmentStats` 还会带入等级/职业基础敏捷，不能按手写零值假设断言。
- Prevention: transcript 中将非目标普通怪物预先标记为 initialized 并把 AI 时间置于未来；Player/Monster/Hero projection 按实际 materialized/equipment stats 记录每个随机上界。
- Verification: 友军初始化状态修正且 Hero `bound=16` 纳入排他随机流后，AI=164 普通测试、`-race -count=10` 和 authenticated `net.Pipe` transcript 全部通过。

### 2026-08-17 — 真实会话维护 tick 可能消费冷却期移动随机数

- Symptom: 全量 race 暴露 `TestSessionOmaWitchDoctorRangeTranscript` 偶发把攻击阶段的随机序列记录为 `[2, 11]` 而不是 `[11]`；堆栈显示连接读循环的实时 `world.tick` 在手工未来时钟之前执行了 OmaWitchDoctor 的 `MoveTo`，即使 `CanMove` 尚未到期仍先消费 `Random.Next(2)`。
- Root cause: 停止 world ticker 不会停止连接级维护 tick；仅把 AI 时间字段设到未来不能阻止 Legacy `MoveTo` 在不可移动时尝试随机方向。
- Prevention: net.Pipe transcript 把维护 tick 与手工 tick 的随机流分开统计，只对手工动作的排他上界断言，并允许已核实的维护 `bound=2`；需要完全隔离时在设置目标前保持目标为空，不能把停止后台 ticker 当成停止所有 runtime tick。
- Verification: 夹具改为校验唯一的手工攻击 `bound=11`，同时只允许维护 `bound=2`；OmaWitchDoctor 定向 race 重复测试和后续全量 race 门禁通过。

### 2026-08-17 — AI=152 读取证据与测试夹具必须隔离

- Symptom: 本轮一次跨仓库读取命令把 Legacy `Shared/Enums.cs` 与 Go glob 放在同一调用，读取失败；PlagueCrab 冷却期测试还发现攻击范围内不可攻击时错误产生 `ObjectWalk`，投影夹具则把宠物/英雄的 MAC+Agility 随机上界误假设为攻击阶段的 `bound=1`。
- Root cause: 对照命令没有保持单仓库参数边界；AI 测试只覆盖了攻击 admission，没有覆盖攻击后继续运行的冷却分支；投影目标的真实防御属性没有从 fixture 的 materialized stats 复核。
- Prevention: 失败的跨仓库读取输出整体作废；每个 AI transcript 都要覆盖攻击后冷却 tick，并在攻击范围内不可攻击时断言无移动；Player/Monster/Hero 投影测试按真实 MAC、Agility、MagicResist 随机调用顺序配置 callback，不把不同阶段的上界混用。
- Verification: 后续 PlagueCrab processor 在冷却期直接返回，session transcript 只消费攻击阶段 `bound=1`；投影测试允许并验证实际 `bound=16` 防御抽样，普通/race 定向测试均通过，Legacy 与 Go 命令已拆分为独立调用。

### 2026-08-15 — net.Pipe 手工 tick 前必须冻结 session loop 的 AI 时间线

- Symptom: 服务端整包 race 门禁再次出现 Armadillo session transcript 空 reveal（期望 `ObjectMonster -> ObjectShow`，实际为空）；普通单测仍可能通过。
- Root cause: `stopPoisonSessionTicker` 只停止后台 ticker，连接 session 的读循环仍会独立调用 `world.tick(time.Now)`；夹具仍以实时 `base` 驱动，race 调度下它先消费一次性的 DigOut reveal。
- Prevention: 真实会话夹具在启动手工时钟前把 AI/search/action 时间置于未来；对一次性 discovery gate 还要把 `DigOutCheckAt` 设为“人工首 tick 前、实时维护 tick 后”，并固定 `setLightClock`，或显式暂停 AI。停止 ticker 不等于停止连接级 runtime tick，不能把两者当作同一时钟。
- Verification: Armadillo 定向 transcript 普通与 `-count=10`、race `-count=10` 均通过；随后将重跑完整普通/race 门禁。

### 2026-08-15 — FlyingStatue 生命周期 transcript 要隔离下一次 AI 与 owner 清理

- Symptom: AI=136 定向测试第一次在 1100ms 命中同时收到第二个 `ObjectAttack`；龙卷风过期断言漏掉了 Slow 清除的 `ObjectPoisoned`；删除默认玩家后宠物用例没有产生龙卷风。
- Root cause: fixture 的 `AttackSpeed=1000` 早于远程命中，world tick 会继续运行 AI；移除 tornado owner 后同一 tick 的 poison processor 会广播状态清除；删除预置玩家后没有把攻击者的缓存目标改为宠物。
- Prevention: 生命周期测试使用足够大的攻击间隔或暂停 AI loop，断言包括 owner 消失引起的 poison 状态包；修改目标 population 后同步更新 `MonsterAITargetID/Kind`，不能保留已删除实体的缓存目标。
- Verification: AI=136 world transcript 现稳定覆盖近战、9 个 tornado 的 spawn/impact/9 个 remove、Slow 清除及宠物命中，定向测试通过。

### 2026-08-15 — StoningStatue 的 Random.Next(1) 也必须保留在后续 Dazed 随机流中

- Symptom: AI=135 等界防御回归实际只记录了魔抗与 Dazed 抽样，缺少敏捷/防御的 bound=1；初始期望序列失败。
- Root cause: Go 通用 `monsterAIRollLocked(1)` 为不可变结果直接返回，StoningStatue 的 MACAgility 敏捷和 `GetDefencePower(min,max)` 两次 Legacy `Random.Next(1)` 因此没有调用注入随机源。
- Prevention: 对 StoningStatue 的敏捷与防御路径使用保留 unit-bound 调用的专用 helper；通用 AI helper 仍维持既有无效分支语义。
- Verification: 等界 pet AOE 回归现在稳定记录 `[10, 1, 1, 5, 3, 10, 2]`，并确认 HP、Dazed duration/value 与毒物写回正确。

### 2026-08-15 — Monster AI 毒伤测试要分离即时绿毒 tick 与派生防御字段

- Symptom: GasToad 定向测试把 Type 2 玩家伤害期望为 90，实际同一 world tick 已为 83；Type 1 高 AC 用例仍掉血，重验用例还遗漏了内层 `Random.Next(2)`。
- Root cause: Go tick 在延迟命中后紧接着处理零 `TickAt` 的 Green poison；玩家伤害使用 `worldPlayer.MinAC/MaxAC` 派生字段而不是仅使用 `Stats` map；Type 0 分支仍按 Legacy 顺序消费 `Next(7)`、`Next(2)`。
- Prevention: 测试分别计算动作伤害与同 tick 毒伤，fixture 同时初始化权威派生 AC 字段和 Stats，确定性 roll 表覆盖每个可达分支的排他上界。
- Verification: GasToad ordinary/type1/type2、吸收伤害仍施 Paralysis、延迟目标重验及 net.Pipe transcript 均通过。

### 2026-08-14 — net.Pipe fixture assignments must pass vet

- Symptom: `go vet ./...` rejected the Poisoning/Purification session fixture's `caster.MP, caster.MaxMP = caster.MaxMP, caster.MaxMP` as a self-assignment.
- Root cause: the fixture used a two-field tuple assignment even though only the runtime MP needed to be restored to the already-existing MaxMP value.
- Prevention: use the narrowest single-field assignment in test fixtures and run `go vet ./...` after adding or changing session setup code.
- Verification: changing it to `caster.MP = caster.MaxMP` made `go vet ./...` pass before the batch gates continued.

### 2026-08-14 — 技能经验测试必须注入确定性随机源

- Symptom: 技能效果本身正确时，经验或概率门禁测试仍可能因生产随机数落点不同而偶发失败。
- Root cause: 测试直接使用运行时随机源，未把 Legacy 需要验证的命中/训练分支固定下来。
- Prevention: 领域测试在构造 World 后显式注入 `combatRoll`，按用例选择稳定的边界值，再单独覆盖概率负例；不要用重复运行碰运气验证技能经验。
- Verification: Hiding/MassHiding、现有战斗技能和对应真实会话测试在固定 roll 下重复运行，经验计数和包序稳定通过。
- Strengthening after recurrence: 新增真实 `net.Pipe` 会话时也必须在启动服务前注入同一确定性随机源；只固定 world 单元 fixture、遗漏 session fixture，仍会让 `MagicLeveled` 经验在完整定向回归中从 1 漂到 3。
- Verification after strengthening: 为 Hiding session fixture 设置 `combatRoll = 0` 后，Hiding/MassHealing 定向会话连续通过，经验 payload 恢复为稳定的 1；随后继续执行全量普通/race 门禁。

### 2026-08-14 — LightSetting 测试必须按旧版的 hour*2 区间逐值核对

- Symptom: 动态 `TimeOfDay` 定向测试把本地小时 9 期望为 Evening，Go 测试失败；旧版实际返回 Night。
- Root cause: 看到 Evening 的 `16/17` 区间后按直觉把连续本地小时映射成 8/9，遗漏了 Legacy 先执行 `Now.Hour * 2 % 24`。
- Prevention: 时间迁移先固定中间量 `hours = hour*2%24`，再列出每个边界小时的枚举和 wire 值；定义类型负例也显式转换为协议底层 `byte`，避免测试类型与业务枚举混淆。
- Verification: 修正 hour 9 为 Night、负例显式 `byte(LightNight)` 后，协议、探针和服务端 TimeOfDay 定向测试通过。

### 2026-08-25 — LightSetting 必须枚举 modulo 后的第二轮边界

- Symptom: MAP-light 初稿只覆盖 UTC 3/4/8/9，测试 oracle 错把 15–20 点都归为 Night。
- Root cause: 虽保留 `hour*2%24` 公式，却按前半天直觉编写期望，遗漏 12 小时后的 modulo 回绕。
- Prevention: 对全部 24 个 UTC hour 逐值计算，并单独锁定 3/4/8/9/15/16/20/21 八个转换边界；非 UTC 表示也必须映射同一 instant。
- Verification: 24-hour table、两轮 change-only 全局广播、non-UTC production clock、count-20 与 focused race 均覆盖修正后的 Dawn/Day/Evening/Night 序列。
- Review finding: 只读 Go review 因两个 NPC context 传入 host-local `time.Now()`，把 `legacyNPCMapLight` 内的 UTC 转换判为高风险。
- Ruling/root cause: 该判断只沿 Go 参数位置推断时间 authority；Legacy `MAPLIGHT` 实际比较同一个 UTC-anchored `Envir.Lights` 名称，而不是本地墙钟小时，因此 UTC 转换是必需等价行为。
- Prevention/verification: 时间消费者必须追到 Legacy authority writer，不能由调用参数 location 推断；主 Agent 以 `Envir.Now`/`AdjustLights`/MAPLIGHT 调用链裁决后保留实现，并把该证据交回 reviewer 做 focused re-review。

### 2026-08-14 — 全局时钟副作用必须区分在线 runtime 与纯 world fixture

- Symptom: 全仓测试中大量使用合成 epoch 时间调用 `world.tick` 的战斗、掉落和治疗 transcript 收到意外 `TimeOfDay` 通知并失败。
- Root cause: 新增的全局时间更新在所有 world fixture 上无条件执行；旧测试的 `tick` 不是在线服务 runtime，却被当成真实服务器时钟循环。
- Prevention: 需要连接级后台 ticker 才启用全局时钟副作用；`startTicker` 负责启用并用当前/注入时钟刷新状态，纯 world fixture 默认保持关闭，定向时间测试显式开启。
- Verification: 已把更新门禁移到 `lightsEnabled`，在线 session 通过 `setLightClock` 启用；先前受污染的现有测试及动态 TimeOfDay 定向测试随后重新运行验证。

### 2026-08-15 — Tucson map value fixture 必须回读权威实体

- Symptom: Tucson Mage WideLine 已排入 Monster 目标并实际产生伤害，但测试检查插入 map 前保留的 `worldMonster` 副本，误报 HP 仍为 100。
- Root cause: Go 的 `world.monsters` 是 value map；延迟命中 resolver 修改并回写 map 中的副本，不会更新测试中之前保存的局部值。
- Prevention: 对 value map 中的延迟实体，命中后必须从 `world.monsters[id]` 回读再断言；指针 map（如 players）和 value map 不得共用断言方式。
- Verification: 断言改为读取 `world.monsters[monsterID].HP`，Tucson world 与真实 net.Pipe transcript 均稳定确认 MC=20、AC=0 的命中后 HP=80。

### 2026-08-14 — Inspect 测试必须区分 Hero 运行表的 owner key 与 object ID

- Symptom: Hero 检查快照测试把 `world.heroes` 按 Hero `ObjectID` 建表，真实解析却按 owner `ObjectID` 查表，导致合法 Hero 请求返回空快照。
- Root cause: 只看到 map value 中的 Hero object ID，没有读取 Go 运行表所有读写路径确认 map key 语义。
- Prevention: 构造运行态 fixture 前先核对该表的 key contract；同时设置 map key、value 的 `ObjectID` 和 `OwnerID`，并用成功解析及深拷贝断言验证。
- Verification: 修正 fixture 为 `world.heroes[ownerObjectID]` 后，Hero detached-snapshot 与真实 Inspect session 定向测试通过。

### 2026-08-14 — 协议尾字段测试必须显式初始化非零语义哨兵

- Symptom: `UserInformation` observe-flags 尾部测试用 `UserInfo{}` 的零值推断 `SummonedCreatureType=None`，实际 Legacy-compatible `None` 哨兵为 99，断言失败。
- Root cause: 把 Go zero value 当成业务枚举的 None 值，未按生产角色创建路径显式设置协议哨兵。
- Prevention: 固定 wire vector 时逐项初始化业务枚举和 sentinel，禁止用结构体零值代表非零 Legacy 常量；同时断言完整尾部字节序列。
- Verification: 测试显式设置 `IntelligentCreatureNone` 后，protocol 定向、全仓普通和 race 测试均通过。

### 2026-08-13 — 在线组 fixture 必须在 world enter 后建立

- Symptom: 区域魔法的 Group 模式测试已在手工 `SelectInfo` 中设置相同 `GroupID`，命中阶段仍伤害了预期友方玩家。
- Root cause: `world.enter` 会主动清除持久化或手工携带的 stale `GroupID`；测试把进入前字段当成在线组权威投影，实际两名玩家入场后都已变为无组。
- Prevention: 所有依赖在线组关系的 world 测试先让全部成员完成 `world.enter`，再调用 `establishRuntimeGroupForTest` 建立 `world.groups` 与成员 `GroupID`；目标行为前断言非零且相等，禁止只在构造体中预填组号。
- Verification: 区域魔法 fixture 改为 enter 后建立组，Group 模式只命中后来进入区域的非组员，友方、离开者和安全区玩家均保持满血；定向测试通过。

### 2026-08-13 — 新增静态对象类型必须同步完整可见性 transcript

- Symptom: KingOfHill `ObjectSpell` 已正确进入会话可见性后，服务端整包测试仍在第二次 NPC 请求前读到遗留的 packet 150，`TestSessionConquestStartStopRefreshesWarZoneAndNPCVisibility` 失败。
- Root cause: 生产层新增了第五类静态可见对象，但首次修改只扩展了会话状态和刷新函数，没有在同一批次更新已有开战/停战真实会话的新增与移除包期望。
- Prevention: 每新增静态对象类型，都同时列出登录、原地重复刷新、进入/离开范围、领域创建/删除四条路径；明确旧对象移除、新普通对象加入、旧新类型移除、新类型加入的顺序，并一次性更新所有 helper 调用和精确 transcript。
- Verification: 边界测试现锁定几何、无效格、稳定 ObjectID、登录/移动恢复和删除；真实会话锁定开战 `NPC ObjectRemove -> ObjectSpell`、停战 `NewNPCInfo -> ObjectNPC -> effect ObjectRemove`，服务端整包测试通过。

### 2026-08-13 — 长测试封装必须保留并轮询 exec 会话

- Symptom: `go test ./cmd/crystal-server` 超过首次 30 秒 yield 后，JavaScript 包装只输出 `exit_code/output/wall`，遗漏返回的 `session_id`，外层脚本结束时无法确认测试最终状态，只能重新执行。
- Root cause: 把首次 `exec_command` 返回当成终态，并在序列化时丢弃了继续轮询所需字段；“脚本已完成”不等于其启动的长命令已完成。
- Prevention: 可能超过 yield 的门禁统一保留完整返回值，并在 `session_id` 存在时循环调用 `write_stdin`，直到取得明确 `exit_code`；不得以空输出或外层 cell 完成代替测试成功。
- Verification: 服务端整包、全仓普通测试和全仓 race 均改用会话轮询取得 `exit_code=0`，随后 `go vet ./...` 与 `go build ./...` 也明确返回 0。

### 2026-08-18 — Blizzard/Meteor 会话断言必须按接收者和技能分开

- Symptom: Blizzard/Meteor 首轮定向测试把场地 `ObjectSpell` 误算为一条，忽略同一可见格会分别广播给附近两个玩家；Meteor 会话还沿用了 Blizzard 的第三条 `ObjectPoisoned` 读取，最终等待 30 秒超时。
- Root cause: 将“事件产生次数”和“单个接收者收到的包数”混为一谈，并把 Blizzard 特有的 Slow 状态包矩阵复用于不施毒的 MeteorStrike。
- Prevention: 区域测试按接收者建立读取计数；世界测试只断言目标自身包序前缀，再单独断言广播/状态包；参数化技能会话的读取数量和期望包序必须由技能分支显式决定。
- Verification: 修正后 Blizzard/Meteor 世界与真实 `net.Pipe` transcript 连续 3 次通过，且未再发生超时。

### 2026-08-18 — 新增 Go 测试夹具必须先复用现有 helper 和真实类型

- Symptom: Blizzard 首次包级测试编译因重复定义已有 `mustWorldSpellInfo` helper，以及把不存在的 `HP/MaxHP` 字段写进 `worlddata.MonsterInfo` 而失败。
- Root cause: 新测试按概念命名/构造，没有先检索当前包的同名 helper 和配置结构定义。
- Prevention: 新增测试前用 `rg` 核对 helper 名称与结构字段，先跑 `go test <package> -run '^$'` 再执行行为测试。
- Verification: helper 改为批次专名、怪物 HP 留在运行时实体后，包级编译及 Blizzard/Meteor 定向测试通过。

### 2026-08-18 — Go 会话夹具赋值必须通过 vet 的自赋值门禁

- Symptom: Blizzard/Meteor 会话测试行为通过后，`go vet ./...` 仍因 `caster.MP, caster.MaxMP = caster.MaxMP, caster.MaxMP` 报 `self-assignment of caster.MaxMP`。
- Root cause: 为恢复运行时 MP 使用了包含不变字段的元组赋值。
- Prevention: 会话夹具只写入实际需要改变的字段，批次门禁必须包含 `go vet ./...`，不能以定向行为测试代替。
- Verification: 改为 `caster.MP = caster.MaxMP` 后，vet 重新通过，后续定向测试保持通过。

### 2026-08-15 — AI=135 定向夹具必须覆盖冷却期移动与真实方向

- Symptom: StoningStatue 线攻击在首个 550ms 命中后，测试因 AI 冷却期进入移动回退而遗漏 `Random.Next(2)`；type=1 会话夹具还把相邻目标的 `Direction` 期望成了 0。
- Root cause: 只按攻击 admission/impact 设计时序，没有把每次 world tick 继续执行的 `ProcessAI` 冷却期移动路径纳入确定性 roll 表；协议期望按手写直觉填写，未从攻击者/目标坐标计算方向。
- Prevention: AI transcript 的固定随机源覆盖攻击后可达的冷却/移动分支；所有 `ObjectAttack` 方向期望由 `DirectionFromPoint` 的坐标输入生成，并同时校验实际 payload。
- Verification: AI=135 world line/area tests 与真实 `net.Pipe` type=1 transcript 连续通过，包含首格/次格延迟、冷却期移动 roll 和相邻方向 payload。
- Verification: Siege 加载、受击方向刷新现使用 Gate 的 midpoint-to-even 公式，并由独立方向与 `ObjectTurn` 回归测试覆盖。

### 2026-08-13 — 权威状态 fixture 必须先完成领域种子再做 JSON 重载

- Symptom: Conquest 全量修复的 Admin 测试预期复活 1 个 Archer，实际得到 0/1；Gate、Wall、Siege 计数仍看似正常。
- Root cause: fixture 为设置 Admin 先保存并重载 auth，此时 JSON 已显式写入空的 `conquests` 数组；后续 Legacy seed 被现代空状态正确拒绝，Bind 只能按定义创建默认存活 Archer 和满血结构。
- Prevention: 通过 JSON 重载改变账户元数据时，必须先导入并绑定本用例依赖的所有权威领域状态，再保存、修改和重载；重载后在执行目标动作前断言关键非默认种子仍存在。
- Verification: fixture 已调整为 Conquest seed/Bind → Admin JSON 重载 → world load；Admin RepairAll 现稳定得到 Archer 1/1，并验证四类终态和公会金币不变。

### 2026-08-13 — 新测试引入依赖后立即执行最小编译

- Symptom: Buff 测试新增 `time` 标准库调用后遗漏 import，直到后续包测试才暴露编译失败。
- Root cause: 连续扩展测试场景时没有在首次使用新标识符后立即运行受影响包的编译门禁。
- Prevention: 新增标准库或 package 标识符后立刻核对 import，并执行 `go test <package> -run '^$' -count=1`；恢复编译绿色后再继续增加 transcript 或跨包测试。
- Verification: 已补齐 `time` import；Buff 定向测试和 `cmd/crystal-server` 整包测试通过，提交前继续执行全量门禁。

### 2026-08-13 — 公会进度 fixture 必须显式固定等级阈值

- Symptom: 公会经验测试最初没有观察到经验变化，因为共享 fixture 默认创建等级 5 公会，而测试配置的经验表已没有可升级阈值。
- Root cause: 测试依赖公会默认等级，没有把待验证的当前等级、当前经验和下一等级阈值组成明确前置状态。
- Prevention: 所有等级/经验测试显式设置实体等级、经验、完整阈值表和预期剩余经验；测试开始先断言该 fixture 确实存在目标升级边界。
- Verification: fixture 现固定公会等级与阈值，并覆盖最终经验同时进入玩家、普通宠物、mentee bank 和公会贡献的路径。

### 2026-08-14 — net.Pipe 包数量必须先由接收者矩阵推导

- Symptom: 两人组队接受测试先后把 leader/member 读取数量写成 8/9，实际生产序列是 7/11，两个 reader 因为一方少消费、另一方等待不存在的包而互相反压并最终挂起。
- Root cause: 根据旧测试和心算反复猜总包数，没有先把每条通知按接收者、包 ID 和顺序完整投影；`net.Pipe` 无缓冲，因此错误数量不仅造成断言失败，还会阻塞服务端向另一连接继续写入。
- Prevention: 多连接 transcript 在编码 count 前先列出领域通知的完整接收者矩阵，再机械统计每条连接；所有连接的 reader 必须在触发操作前并发启动，读取完成后再检查共享状态。功能新增包时先更新矩阵，再更新 count 和期望 ID，禁止靠超时或试数修正。
- Verification: 两人入组矩阵锁定 leader 7 包、member 11 包，三人入组与跨地图聊天另有精确 domain/session transcript；`TestSessionTwoPlayerGroupInviteAcceptAndLeave` 不再挂起，受影响包全量测试通过。

### 2026-08-13 — 原子回滚测试的期望快照不能与请求共享引用

- Symptom: `AttachSealedHero` 的伪造 stats 负例报告角色权威状态被修改，实际变化来自测试直接改写了与调用前 `before` 快照共享的 `StoredItem.AddedStats` map。
- Root cause: 构造事务请求时浅拷贝了物品切片/指针，把“待篡改输入”和“零变更基线”当成两份数据，导致测试自身污染期望值。
- Prevention: 所有原子失败测试在修改 request 前，递归深拷贝 item grids、Hero snapshots、stats maps 和嵌套 slots；失败后分别从服务重新读取角色与 registry，并与独立基线比较。
- Verification: attach fixture 改用 `cloneItemInfos`、`cloneStoredItems`、`cloneStoredHeroes` 构造请求；伪造 stats、stale stack、stale Hero 槽、满槽、已绑定及双侧物品缺失用例均通过。

### 2026-08-13 — 会话局部物品变更必须先同步 world 再读取整角色快照

- Symptom: P8 整包回归中，普通/太阳药水已经正确消费、删除响应也成功，但登出持久化又出现两个旧物品；`TestSessionUseAndDeleteItemTranscriptAndPersistence` 稳定失败。
- Root cause: 本批为了宠物登出读取 world 快照，使既有 `DeleteItem` 未同步 world 的缺口变成可见；退出阶段的整角色快照覆盖了 session 中更新后的背包。最初尝试只合并 `Pets` 仍会被后续整快照覆盖，未触及真正的状态所有权问题。
- Prevention: 任何 session 局部物品消费、删除、移动或创建一旦成功，必须在可能读取 `playerCharacterSnapshot` 前同步 `world.updatePlayerItems`；伴侣持久化只负责其领域字段，不能用条件式旧快照合并掩盖其他领域未同步。新增登出读取路径后，必须重跑所有会话物品持久化测试。
- Verification: `ClientDeleteItem` 成功后现同步 world，删除了无效的宠物条件合并；目标用例连续运行 20 次及 `cmd/crystal-server` 整包测试均通过。

### 2026-08-13 — 测试 helper 与生产标识符必须先做包级冲突检查

- Symptom: 新增生产 helper 时使用了测试文件已有的 `intelligentCreatureRewardRoll` 名称，导致同包编译冲突；新增磁盘会话测试又在使用 `filepath` 后遗漏 import。
- Root cause: patch 前只检索生产文件，没有检索整个 Go package 的标识符和 import 需求，也没有紧跟最小编译检查。
- Prevention: 新增 package 级标识符或标准库依赖前，用 `rg` 搜索整个目录（包含 `*_test.go`）；每个语义 patch 后立即 `gofmt` 并运行受影响包的定向编译/测试，再继续扩大改动。
- Verification: 生产 helper 改为 `rollIntelligentCreatureReward`，补齐 `path/filepath` import；智能生物定向测试和服务端整包测试均通过。

### 2026-08-13 — 测试调用 helper 前必须读取完整返回签名

- Symptom: Buff 会话 fixture 把无返回值的 `UpdateCharacterHealth` 当成可用于条件判断的成功布尔值，造成包级编译失败。
- Root cause: 只按同类 `Update...` 命名推断返回值，没有读取当前方法的真实声明。
- Prevention: 每次新增测试 helper 调用先用 `rg` 定位声明并读取完整参数/返回签名；首次 patch 后先跑受影响包的最小编译，再扩展 transcript。
- Verification: fixture 改为按无返回值签名调用，Buff 定向测试与 `cmd/crystal-server` 整包测试通过。

### 2026-08-13 — 测试参考模型不能替代生产入口覆盖

- Symptom: 奖励 shape 的参考调度测试即使全部通过，也不能证明 `main.go` 的真实 `ClientUseItem` 分支已接线或按相同顺序持久化和发包。
- Root cause: 参考模型与生产实现可能共享错误假设，且绕过连接状态、通用尾部和 auth/world 桥接。
- Prevention: 参考模型只用于穷举规则；每个功能簇至少增加一个真实 dispatch/net.Pipe 会话，覆盖生产 packet 入口、状态桥、网络 transcript、登出和 JSON 重载。
- Verification: `TestCurrentGoDispatchHandlesEveryRewardShapeInProduction` 之外，新增奖励生产会话逐 shape 经过真实 ClientUseItem，并从账户 JSON 重载验证终态。

### 2026-08-13 — 会话 transcript 必须包含登录后的延迟 Buff 包

- Symptom: 带持久 Buff 的角色登录后，延迟发送的 `AddBuff` 可能落在第一条业务请求响应之前，若测试只按静态 bootstrap 列包会把它误判成目标动作响应。
- Root cause: 登录状态完成与定时/延迟通知并非同一同步边界，fixture 没有把已持久 Buff 的后续投递列入 transcript。
- Prevention: 带 Buff 的会话测试在首个业务 marker 前显式消费并断言登录后 `AddBuff`，或把它作为有序 transcript 的第一项；payload 同时断言运行时 ObjectID。
- Verification: 安全区移动会话锁定 `AddBuff -> PauseBuff -> HealthChanged -> UserLocation`，并通过登出及磁盘重载验证。

### 2026-08-13 — Buff 属性 fixture 必须从真实基础属性逐项计算

- Symptom: WonderDrug/Knapsack 与安全区暂停测试若直接猜最终 HP、背包负重或 buff stats，容易遗漏 class/level 基础公式、现有附加属性和叠加规则。
- Root cause: 把预期终值按直觉填写，没有从角色基础属性、物品 AddedStats、已有 Buff 和暂停状态逐项代入生产计算。
- Prevention: 属性测试固定角色 class/level/HP/MP 和源物品 Stats/AddedStats，先列出基础值与每层增量，再断言最终 stats、生命钳制和“延长时长但不替换原 stats”等怪癖。
- Verification: 当前测试锁定 WonderDrug HP、Knapsack Luck→BagWeight 及二次使用仅延长时长，安全区暂停后 MaxHP/HP 钳制也由生产会话验证。

### 2026-08-13 — 双向协议 transcript 不得按 packet ID 反推方向

- Symptom: 智能生物 transcript 的第 21 项把客户端 `UpdateIntelligentCreature=125` 误判成服务端方向，因为服务端 `ObjectEffect` 也合法使用 ordinal 125。
- Root cause: 测试只保存 ID，并通过一组服务端 ID 推导方向；Crystal 的客户端与服务端枚举是独立命名空间，允许跨方向重号。
- Prevention: 所有双向 transcript 期望使用显式有序 `(ID, direction)` slice，每一帧同时声明 ordinal 和方向；禁止用 packet ID、数值范围或名称集合反推方向。
- Verification: P11 transcript 已改成逐项显式 ID/方向期望，合法的 125 跨方向重号分别按真实发送方向验收。

### 2026-08-13 — 有序协议 transcript 禁止使用 map 驱动

- Symptom: P11 Go 探针定向测试在发送觉醒重置请求时出现 `io: read/write on closed pipe`，对端已因收到顺序漂移的前一请求而退出。
- Root cause: 测试用 Go map 保存 NPC 面板读取器和客户端请求函数，却让 `net.Pipe` 服务端按固定 Legacy 顺序读取；map 迭代顺序不稳定，不能表达 wire transcript。
- Prevention: 所有有序协议、数据库迁移步骤和副作用序列统一用显式 slice/数组声明顺序；map 仅用于不关心顺序的集合断言。对端报 EOF/closed pipe 时先核对双方完整 packet 序列。
- Verification: 两处 map 已改成具名函数的有序 slice；P11 定向测试连续运行 10 次、`internal/probe` 整包测试、P11 race 测试和 `go vet` 均通过。

### 2026-08-13 — 领域层状态成功不等于完整网络 transcript

- Symptom: 战争和领地领域测试已覆盖扣款、敌对和所有权，但首次双会话测试仍暴露 `ColourChanged`/`ObjectColourChanged`/`ObjectPlayer` 的接收者顺序，以及离线时实际只有 `ObjectRemove` 而没有跨公会成员包。
- Root cause: 只按业务提交结果推断网络行为，没有按每个在线观察者展开 Legacy 的广播循环和会话注销路径。
- Prevention: 跨玩家功能必须同时建立领域终态测试与 net.Pipe 接收者矩阵；逐个列出发起者、同会成员、敌会成员和普通观察者的包序列，并以实际路径为准修正 fixture，不能凭相似公会通知推断注销包。
- Verification: 新增战争双会话和领地分页/购买 transcript，锁定双方聊天、金库、颜色/对象刷新与持久终态；定向服务端测试通过。

### 2026-08-12 — 跨会话状态变更必须主动唤醒目标会话

- Symptom: 爱人跨地图召回已立即发送 MapChanged/ObjectTeleportIn，但目标会话的本地地图快照、位置持久化以及 NPC/怪物/地面物品刷新，要等目标客户端再发一个包才执行；全量测试还暴露了额外传送包与 barrier 的时序竞争。
- Root cause: 共享 world 状态由发起者会话修改，目标会话却阻塞在 ReadFrame；初版 pending 队列只有轮询消费，没有事件唤醒，且测试把固定包数量误当成了跨 goroutine 的完成屏障。随后直接反复改 socket read deadline 虽能唤醒，却会与下一次 ReadFrame 竞争，造成已入队 KeepAlive 被旧 deadline 立即超时并丢失；临时 wake channel 又因重建窗口丢通知。
- Prevention: 外部会话入队 transition 后通过会话生命周期内固定的 buffered wake channel 唤醒其所有者；会话用单个受控读 goroutine 等待客户端帧，并在等待期间可多次应用本地状态、持久化和可见对象刷新，随后继续消费同一个客户端帧。只有真正的会话/任务超时才短暂设置 read deadline，业务唤醒禁止借用 socket deadline；队列不能用单槽覆盖，wake channel 不能在读循环中重建。会话测试使用 KeepAlive 完成屏障，并允许已经验证过但可能稍后到达的传送尾包，不能靠客户端主动包来触发业务状态。
- Verification: 同地图、跨地图静态对象刷新、无需目标主动发包的位置持久化和连续两个 pending transition 均有 net.Pipe 覆盖；Go 全量测试通过。

### 2026-08-12 — 会话 fixture 必须显式设置方向并穷举装备门控

- Symptom: 跨地图召回测试期望落在 (1,0)，但角色默认方向为 0，实际合法目标是原地 (0,0)；婚戒替换测试最初又因角色等级 1 与默认 RequiredType=Level/RequiredAmount=2 不符而提前走装备失败分支。
- Root cause: fixture 依赖了隐式默认方向和物品需求字段，没有把目标坐标公式及 CanEquipItem 的职业、性别、等级/属性门控全部固定下来。
- Prevention: 传送 fixture 在登录前显式持久化方向并按 Front 计算目标；装备事务 fixture 显式设置合法 RequiredType、RequiredAmount、RequiredClass、RequiredGender，同时为已绑定婚戒等隐藏状态补负例。
- Verification: 跨地图召回稳定落在预期前方坐标，婚戒替换覆盖余额不足、非法物品、已绑定婚戒及成功原子交换。
- Strengthening after recurrence: 该规则同样适用于钓鱼附件、Mount/Socket 与 Storage 来源的 `EquipSlotItem`；`RequiredClass=0` 或 `RequiredGender=0` 不是“无限制”，而是位掩码不包含任何角色。所有可使用附件 fixture 必须显式给当前职业/性别位，边界负例则单独改变目标字段。

### 2026-08-12 — 跨会话测试数据必须避开 bootstrap 提前消费

- Symptom: 为验证召回后的地面金钱刷新而预置目标地图对象时，对象在调用者 bootstrap 阶段就被发送，导致召回 transcript 缺少预期 ObjectGold。
- Root cause: 测试在会话建立前就把共享对象放在当前可见范围，没有区分 bootstrap 可见性与目标动作后的新增可见性。
- Prevention: 需要验证动作触发刷新时，先把对象放在不可见地图/坐标，相关客户端 bootstrap 完成后再移动到目标落点；NPC、怪物和地面对象分别断言 fixture 数量与动作后包序列。
- Verification: 跨地图召回测试在无需 KeepAlive 触发业务的情况下收到 NPC 定义/对象、怪物和 ObjectGold。

### 2026-08-12 — 新增 bootstrap 包必须同步所有 transcript fixture

- Symptom: 加入登录后的 `FriendUpdate` 后，功能测试正确，但大量旧会话测试仍把 `ReceiveMail` 后的下一包写死为物体或 `TimeOfDay`，整包测试集中失败。
- Root cause: bootstrap 是共享、严格有序的协议面；只更新通用 helper 和新功能测试，没有先枚举所有手写启动序列。
- Prevention: 每新增、删除或重排 bootstrap 包，先用 `rg` 搜索前后相邻包 ID，并一次性更新通用状态机与所有显式 expected 序列；随后先跑整个会话 package，再跑全量门禁。
- Verification: 所有 `ReceiveMail` 后的启动序列已加入 `FriendUpdate`，`go test ./cmd/crystal-server -count=1` 通过。

### 2026-08-12 — 邮件 transcript 必须消费发送方删除包与接收方定义包

- Symptom: 带附件的黑名单邮件兼容测试先期望 `MailSent`，实际先收到 `DeleteItem`；接收方先期望 `ReceiveMail`，实际先收到附件的 `NewItemInfo`。
- Root cause: 测试只关注邮件结果，遗漏附件发送的完整可观察包序列。
- Prevention: 邮件 fixture 按是否有附件分别列出发送方 `DeleteItem`/扣费/`MailSent` 与接收方新邮件提示/未见物品定义/`ReceiveMail`；共享世界重登还要先用 barrier 消费加入/离开通知。
- Verification: 黑名单下纯文本与附件两条真实网络路径均通过定向社交测试，附件删除、定义、邮件顺序和持久状态都有断言。

### 2026-08-12 — 会话 fixture 的角色名也必须满足 3–15 字符约束

- Symptom: 新增拍卖定义测试使用 `DefinitionViewer`，角色创建返回 1，定向测试在业务逻辑前失败。
- Root cause: 只检查了账号 ID，没有复用角色名同样存在的长度与字符集约束。
- Prevention: 测试账号和角色名统一使用 3–15 个 ASCII 字母数字，并在 fixture 创建失败时先核对认证枚举与输入长度，再排查目标功能。
- Verification: 改为 `DefViewer` 后嵌套 ItemInfo、市场会话及全量测试通过。

### 2026-08-12 — 买回数量测试必须区分堆叠上限与当前存量

- Symptom: 买回用例用 `Count=9` 配合 `StackSize=1/5`，加入原版堆叠上限门控后 transcript 等待超时；旧实现因缺少门控反而掩盖了 fixture 错误。
- Root cause: 把“超过当前存量时截断”误写成“任意超大数量都截断”；原版先拒绝超过 `ItemInfo.StackSize` 的请求，再把合法范围内超过存量的请求截到存量。
- Prevention: 数量测试同时列出 `request.Count`、`ItemInfo.StackSize`、`stored.Count`；截断场景必须满足 `stored.Count < request.Count <= StackSize`。
- Verification: 买回用例改为 `9 <= StackSize=10`，UsedGoods 用例改为 `3 < Count=4 <= StackSize=5`，并通过完整 net.Pipe transcript。

### 2026-08-12 — 背包持久化断言必须按物品身份而非槽位

- Symptom: `[BUYUSED]` net.Pipe transcript 的购买和登出数据都正确，但测试按 `Inventory[0]` 断言，实际物品被放入原版可用背包区的第 7 格而失败。
- Root cause: 把背包内部的自动落位策略误当成了对外行为契约；物品加入逻辑会按物品类别选择合法起始槽位。
- Prevention: 验证跨层物品迁移时按 `UniqueID`、数量、状态和持久化结果断言；只有原版明确固定槽位的 Move/Storage 测试才断言数组索引。
- Verification: 改为扫描持久化背包查找 `UniqueID` 后，`[BUYUSED]`、UsedGoods 合并/刷新及登出测试通过。

### 2026-08-11 — 会话测试 fixture 必须复用真实桥接签名

- Symptom: Craft net.Pipe 测试初版调用 UpdateCharacterItems 时少传了一个物品网格参数，并把 mapdata.NewOpen 的宽度变量以 int 传入，导致测试包无法编译。
- Root cause: 新测试 fixture 是按记忆拼接 helper 调用，没有先读取现有持久化 API 和地图构造函数的完整签名。
- Prevention: 新增跨层测试 fixture 前先用 rg/源码确认 helper 签名；对 UpdateCharacterItems 明确按 ItemInfos、Inventory、Equipment、QuestInventory 四段传参，地图尺寸在调用边界显式转换为 int32。
- Verification: 修正后 go test ./cmd/crystal-server -run 'TestCraftSession' -count=1 通过。

### 2026-08-11 — Refine 概率断言必须逐项代入公式

- Symptom: Refine 材料测试把成功率期望写成 79，实际实现按原版公式得到 94。
- Root cause: 手算时漏加了幸运项的 5% 和基础成功率的 20%，测试断言没有逐项列出中间结果。
- Prevention: 写强化概率断言前固定列出材料、矿石、幸运、基础四项及最终减项；涉及整数除法和边界材料时逐项核算后再运行测试。
- Verification: 修正期望为 94 后，Refine 定向测试通过；后续继续执行全量 Go 测试与竞态验证。
- Strengthening during `REFINE-P6-WORKBENCH-001`: legacy-width reduction 用例又把 RequiredAmount=2 的 item 项误算高 1，先断言 44 而实现正确得到 43；同时首个窄类型补丁直接比较 `int16` 与 `byte`，被 Go 编译门禁拒绝。现固定把 `refineStat*5 - required + 5`、材料三项、矿石三项、幸运、基础和 50 点减项逐行列出，并在比较 Legacy `short`/`byte` 前显式提升到 `int`；修正后精确 Refine 门禁通过。
- Strengthening during the restart transcript: 首次把收取成功后的下一包直接当作 `GainedItem`，实际生产先发 `NewItemInfo`；`CurrentRefine` 不属于 Legacy StartGame 的 Inventory/Equipment/QuestInventory definition bootstrap，重启客户端尚未见过该定义，`GainItem` 会先 `CheckItem`。现 transcript 明确锁定 `NPCResponse -> ItemReturned Chat -> NewItemInfo -> GainedItem -> NPCCollectRefine(true)`，精确重启门禁通过。
- Further strengthening after complete Legacy persistence/clone tracing: 初稿错误地让 wall-clock downtime 消耗绝对 deadline、让 `Refine[]` 跨重启，并把完整服务端 item clone 发给客户端。精确 `CharacterInfo.Save/Load` 与 `UserItem.Clone` 证明 checkpoint 保存剩余毫秒且不保存工作台，`GainedItem` clone 保留 `RefineAdded` 却重置 `RefinedValue`、`RefineSuccessChance` 和 `WeddingRing=-1`；服务端背包原物品仍保留这些字段。新的 live-checkpoint/restart/wire transcript 已分别锁定三层状态。
- Workflow failure evidence: 一个复合补丁的 protocol hunk 因字段对齐正文不匹配而失败，但 auth hunk 已落地，随后编译因缺少 `RefineTimeRemaining` 被拒；已按失败事务规则回读两文件并独立补 protocol。后续把“仅 check 成功时同步”补丁误落到相邻 start outcome，又因不存在 `Destroyed` 字段被编译门禁拒绝；复读两个 case 后分别恢复 start 无条件同步、check 按 mutation 同步。格式化改造还把 `int64` minutes 交给只支持 Legacy 数字类型的 formatter，返回错误后 fallback 暴露未展开模板；改为范围安全的 `int` 后 production restart transcript 恢复。跨相邻 case 和 formatter 参数都必须以精确 outcome 类型/完整签名核对。
- Review-recovery evidence: observer/refine 文件枚举误把通用 `session` 作为 OR 条件并输出近乎全部 session tests；该宽泛输出作废后改读精确文件。新增 observer transcript 又把无返回值的 `UpdateCharacterAllowObserve` 当作 `bool`，由最小编译拦截。随后仅按重复赋值行修改 session merge，补丁误落 generic item helper；物理回读发现后立即恢复，并以两个完整 closure 锚点重写。最终 touched compile、Refine/auth count-20、race count-3 及 observer/item-authority 回归均通过。
- Integration finding: 首次 fresh full test 只失败 `TestRentalTransactionJSONRoundTrip`；新 Load 正确重建 16 个空 Refine 槽，但 `CreateCharacter` 仍生成 nil 工作台，导致无关 Rental deep-equal 发现构造/恢复规范不一致。根因是只改 restart normalizer，没有同时核对 fresh CharacterInfo constructor。创建入口现也初始化固定 16 槽；精确 Rental 回归与后续 fresh full test 均通过。任何固定数组的 restart 规范化都必须同时核对 fresh create、Legacy import 和 JSON load 三个入口。
- Integration finding after revision-aware Refine commits: fresh server package initially failed only `TestAdminLogoutPreservesDirtyWorldItemsAcrossNonClearAuthRevision`. A rejected/no-op Refine callback returned the latest auth revision and the session helper applied that snapshot to world, overwriting an intentionally dirty unversioned world inventory even though Refine changed nothing. The helper now publishes auth/world grids only when `Committed`; ordinary CLEARBAG reconciliation remains with the established item-authority merge. The exact regression, Refine count-20 and focused race count-3 pass; the fresh package gate must be rerun.
- Repeated-gate finding: 非零 Refine restart transcript 曾硬断言重启后立即显示 19 分钟；极快重复中 Load、bootstrap、panel 与 collect 落在同一毫秒，Legacy 整数毫秒时钟合法显示 20，断言失败后未关闭 pipe 又占住全局 NPC flow mutex，使后续用例超时。夹具现显式跨过至少一个毫秒 tick 再锁定 19，并在 open 时注册 connection cleanup；精确 count-20 与 race count-3 通过。时间边界测试必须控制 Legacy 时钟桶，且 fatal path 也必须释放共享序列化资源。
- Terminal-review workflow finding: reviewer `01a04b3b-ef31-7332-a0bc-63f62d5c6772` was asked for read-only review but not explicitly forbidden from rerunning gates; it launched duplicate full test/full race after main had already passed them, remained running after two stop/report messages, and was closed without an acceptance report. Replacement review prompts now explicitly prohibit every test/build/vet/formatter/probe command and request code findings only; the abandoned agent contributes no acceptance evidence.
- Terminal review `01a04b60-f42d-7ae3-a8aa-839afc86ac69` raised two P1 findings that required Legacy rulings. Immediate per-request `SaveJSON` was rejected: exact `Deposit/Retrieve/Cancel/RefineItem/Check/Collect` contain no save, while `Envir` checkpoints all accounts only at `SaveDelay`; the production restart transcript explicitly samples the live periodic checkpoint, and adding a synchronous save-failure packet gate would diverge. Rental merge was also rejected: Legacy permits a rental item unless `BindingFlags.DontUpgrade`, moves it out of Inventory into `CurrentRefine`, and `ProcessRentedItems` scans only Inventory/Equipment, so temporary expiry invisibility is observable Legacy behavior. The auth callback already runs against the latest locked authority rather than a stale replacement; dedicated admission and authoritative-rental start/collect regressions now lock this quirk, with count-20 and race-count-5 passing.

### 2026-08-15 — AI 测试夹具必须复用精确 stat 名称、锁语义和随机公式

- Symptom: AI=98 定向测试先因把玩家 stat 的 `statMinMAC`/`statMinMC` 写成不存在的 `monsterStat...` 常量而无法编译；修正后又在持有 `world.mu` 时调用会再次加锁的 `enableMonsterAI`，测试超时；最后按单次 DC 随机直觉断言炸弹/地震伤害，未覆盖 Legacy HellLord 地震的嵌套 `Random.Next(Random.Next(min,max))`。
- Root cause: 新夹具没有先读取当前包的精确常量和锁边界，并把 Legacy 的随机调用序列/嵌套范围压缩成了看似等价的单次 roll。
- Prevention: 新测试先检索声明和完整 helper 签名；持锁区只直接写字段，锁外调用加锁 helper；迁移随机 AI 时逐次列出调用顺序、闭区间/开区间和嵌套 roll，并用计数确定性源验证最终 wire/HP。
- Verification: HellLord/HellKnight 定向测试现可在 30 秒门禁内完成，覆盖 600ms 动作、延迟召唤、地震、阶段免伤和炸弹 10 秒/500ms 生命周期；包级编译与定向回归通过。

## Entry format

```markdown
### 2026-08-11 — 地图 fixture 必须与 x-major 索引和函数返回值一致

- Symptom: 地图测试首次失败于门索引断言，服务端测试同时出现 `NewOpen` 返回值数量不匹配。
- Root cause: fixture 按错误的 cell 坐标写入门数据，调用点也把返回 `(map, error)` 当成了三值结果。
- Prevention: 先固定并复用 `x*Height+y` 的索引规则；修改构造函数后立即检查所有调用点的返回签名。
- Verification: 修正 fixture 与调用点后重新运行完整 Go 测试、`go vet` 和 `git diff --check`。

### 2026-08-11 — 协议测试必须遵守 Login/Select 状态边界

- Symptom: 新增账户生命周期的 net.Pipe 测试在改密请求处收到 EOF。
- Root cause: 测试在登录成功进入 Select 阶段后发送了只允许 Login 阶段处理的 `ChangePassword` 请求。
- Prevention: 为每个客户端包记录原服务端允许的 `GameStage`，状态机测试按 Version → Login-stage operation → Login → Select 的顺序发送。
- Verification: 将改密请求移到登录前后，重新运行完整 Go 测试、`go vet` 与 `go test -race`。

### 2026-08-11 — 地图格式修正必须同步 fixture 的真实记录步长

- Symptom: 按原版 `LoadMapCellsV100` 修正 v100 为 27 字节单元后，旧测试仍把 fishing 字段写在 30 字节偏移，地图测试失败。
- Root cause: fixture 是按迁移代码的旧猜测构造的，没有与 .NET 字段偏移和记录步长绑定。
- Prevention: 每种地图格式先从原版 loader 固定 header、字段偏移、记录步长，再生成最小 fixture；修正偏移时同时更新正向和截断数据测试。
- Verification: fixture 修正后重新运行 mapdata 全量测试，并继续执行 Go 全量测试、race、vet、build 和 diff 检查。

### 2026-08-11 — NPC 脚本 fixture 必须保留真实换行边界

- Symptom: 构造脚本 JSON/net.Pipe fixture 时曾把换行转义成字面量反斜杠，解析器会把多个 page 当成一行。
- Root cause: 测试字符串经过工具封装层二次转义，未在写入后检查 Go 源码中的实际转义层级。
- Prevention: 脚本 fixture 统一使用 Go 字符串的单层反斜杠 n，并在测试中断言 page 数量、文本顺序和按钮目标，不只断言请求成功。
- Verification: 增加 TestParseNPCScriptTextAndButtons 和脚本端到端 page 测试；普通/race 全量 Go 测试通过。

### 2026-08-11 — 新增 packet 布局测试必须先按字段宽度核算长度

- Symptom: 新增 `ObjectAttack` payload 测试把长度写成 20，测试实际序列化出 16 字节而失败。
- Root cause: 把字段数量误当成了额外的长度，未按 `uint32 + int32 + int32 + 4*byte` 逐字段求和。
- Prevention: 每个 packet 先从 legacy `WritePacket` 列出字段类型和顺序，手算并在测试中同时断言长度与关键偏移；不要凭相邻 payload 估算长度。
- Verification: 修正为 16 字节后，协议测试通过；提交前继续运行 `go test ./...`、`go test -race ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 新增 packet ordinal 测试必须同步 wants 与 got（强化）

- Symptom: 门 packet ordinal 测试加入 legacy 期望后，测试先报告 `ServerOpendoor = 0`；同一阶段新增 `ServerWorldMapSetup` 时又复现了 `= 0`。
- Root cause: 新 ID 被分两次 patch 追加，第二次只更新了期望表，遗漏实际值表，map 缺失项被 Go 的零值掩盖。
- Prevention: 每新增一个 ordinal，必须在同一个 patch 中成对更新 legacy `wants` 与 Go `got`，两侧都保留显式 legacy 数值；patch 后立即运行 ordinal 测试，再继续写功能代码。
- Verification: 补齐 `got["ServerOpendoor"]` 和 `got["ServerWorldMapSetup"]` 后，协议、地图和服务端测试重新通过。

### 2026-08-11 — 移动阻挡测试必须验证实际目标格

- Symptom: 门自动关闭测试把玩家放在门内，随后向远离门的方向移动，却期望移动被门阻挡。
- Root cause: 测试场景没有先核对方向对应的目标坐标；关闭的门只阻挡进入带门的目标格，不阻挡离开该格。
- Prevention: 编写移动/碰撞断言时先画出起点、方向、步数和每个中间目标格；门阻挡用“门外进入门格”场景验证，离开门格单独验证可通行。
- Verification: 将玩家恢复到门外并尝试进入关闭门格后，门状态单测通过。

### 2026-08-11 — Go 测试 helper 作用域必须按 package 检查

- Symptom: 地图查询测试在 `cmd/crystal-server` 编译时找不到 `readTestDotNetString`，同时缺少该测试自己的二进制解析 import。
- Root cause: 把 `internal/protocol` package 的测试私有 helper 当成了 `main` package 可复用 API；Go 测试文件之间只有同 package 标识符可见，`*_test.go` helper 不会跨 package 导出。
- Prevention: 新增测试先确认 package 声明和 helper 所在目录；跨 package 只调用公开函数，必要时在当前 package 定义带 `t.Helper()` 的本地适配器。
- Verification: 改为调用公开 `protocol.ReadDotNetString` 并补齐 `encoding/binary` 后，地图协议测试编译通过。

### 2026-08-11 — 战斗 transcript 期望必须先算伤害终态

- Symptom: 远程攻击单测把 3 点生命、4 点攻击、1 点护甲的场景期望为未击杀，实际 `damage - armour == HP` 触发了 `ObjectDied`，测试失败。
- Root cause: 测试只按“远程命中”直觉填写 `Killed` 和通知数量，没有先按当前确定性伤害公式核算 HP 终态。
- Prevention: 战斗测试先列出攻击力、护甲、有效伤害、初始/最终 HP，再决定 struck、damage、death packet 序列；通知数量必须与该终态一致。
- Verification: 修正远程 transcript 断言为四包并重新运行 Go 全量测试通过。

### 2026-08-11 — net.Pipe 测试必须先消费副作用包

- Symptom: NPC `ENTERMAP` 端到端测试在传送后直接发送 keep-alive，测试挂起；第一次断言还在服务端完成持久化前读取了角色状态。
- Root cause: `net.Pipe` 没有缓冲，地图切换后的静态对象移除包尚未被客户端消费时，服务端会阻塞写包；读取响应包也不等于 handler 已完成后续副作用。
- Prevention: 为每条 net.Pipe transcript 列出完整发送顺序，先消费 `ObjectRemove` 等副作用包，再用后续 keep-alive/响应建立处理完成屏障后检查持久化状态。
- Verification: `TestSessionNPCEnterMapConsumesNeedMoveDestination` 补齐旧地图 NPC 移除断言和 keep-alive 屏障后，定向 Go 测试通过。

### 2026-08-11 — 饱和余额测试必须先算每一步终态

- Symptom: NPC 货币单测在余额已经饱和后仍用 `TakeGold(10)` 期望扣除整笔余额，断言得到 10 而不是预期的最大值。
- Root cause: 测试把“请求超过余额时钳制”与“请求值为 10”混为一谈，没有按初始余额、饱和增益和实际扣款请求逐步计算终态。
- Prevention: 对带上限/下限的经济操作先列出每个操作后的余额，再选择确实超过余额的请求验证钳制；packet amount 也必须断言实际变化量。
- Verification: 将扣款请求改为 `uint32` 最大值后，NPC 货币 transcript 和 auth 余额单测通过。

### 2026-08-11 — LevelUp transcript 必须保留 HP/MP 中间状态

- Symptom: `CHANGELEVEL` 端到端测试收到第一条 `HealthChanged(100, 100)`，原版顺序应为先 `SetHP` 的 `(100, 20)`，再 `SetMP` 的 `(100, 100)`。
- Root cause: 实现先把 HP/MP 都更新为最终值，再序列化第一条副作用包，丢失了 `SetHP` 与 `SetMP` 之间的可观察状态。
- Prevention: 对连续副作用逐步记录旧状态、每一步新状态和 packet 顺序；只有完成 transcript 写出后才用最终状态作为后续动作基线。
- Verification: 修正为使用旧 MP 构造第一条包后，`TestSessionNPCScriptChangeLevelAction`、健康动作定向测试、观察者通知测试和协议布局测试通过；随后 `go test ./...`、`go test -race ./...`、`go vet ./...`、`go build ./...` 与 `git diff --check` 全部通过。

### 2026-08-11 — 条件 NPC transcript 必须显式设置角色前置状态

- Symptom: 条件 NPC 端到端测试在第二次调用时仍收到 ELSE 文本，成功分支没有被选中。
- Root cause: fixture 只设置了金币，却忘记 CreateCharacter 的默认等级是 1；测试场景期望等级条件 LEVEL >= 2 成功。
- Prevention: 每个条件 transcript 在启动会话前显式写入并断言等级、金币、性别、职业、地图和坐标等前置状态，不依赖创建默认值。
- Verification: 显式写入等级 2 后，低金币 ELSE 和满足等级/金币 SUCCESS 两条 net.Pipe 路径均通过，且 race 全量测试通过。

### 2026-08-11 — 多观察者战斗 transcript 必须展开接收者数量

- Symptom: MeteorShower 测试第一次把三个目标的命中包期望为 6 条，实际收到 12 条。
- Root cause: 每个目标的 `ObjectStruck`/`DamageIndicator` 会分别发送给范围内的施法者和观察者，测试只按目标数计算，没有乘以接收者数。
- Prevention: 编写广播型战斗 transcript 时先列出目标数、每目标包数和每个范围内接收者，再按发送顺序断言总数与关键 payload；私有包和广播包分开计算。
- Verification: 修正 MeteorShower 期望为 12 条后，定向测试通过，并将继续运行 race/vet/build 全量校验。

### 2026-08-11 — 怪物 fixture 不应假设 spawn 输入顺序

- Symptom: ThunderStorm/FlameField 测试把 object ID 2 当作亡灵，实际读取到的是位于 `(3,0)` 的普通 Goblin。
- Root cause: `loadMonsters` 会按 `MapIndex`、`RespawnIndex`、`MonsterIndex` 和坐标排序后再分配 object ID，输入 slice 顺序不是稳定的对象身份契约。
- Prevention: 生成怪物 fixture 后按坐标、`Info.Undead` 或其他领域属性定位目标；只有在 loader 明确保证顺序时才断言 object ID。
- Verification: 测试改为按坐标和 `Undead` 属性查找目标，`TestGameWorldThunderStormAndFlameFieldAreaRules` 通过。

### 2026-08-11 — 测试 helper 调用必须核对完整函数签名

- Symptom: 修正怪物 fixture 后，测试编译失败，`monstersNear` 实际需要四个参数但调用只传了三个。
- Root cause: 修改测试时只记住了查询坐标和距离，遗漏了函数签名中的 `mapIndex` 参数。
- Prevention: 复用测试 helper 前先检索函数声明和现有调用，按参数名称逐项核对地图、坐标和范围；修改后立即运行定向编译测试。
- Verification: 补齐 `mapIndex` 参数并执行 `gofmt` 后，ThunderStorm/FlameField 定向测试通过。

### 2026-08-11 — world 测试必须显式投递返回的通知

- Symptom: Healing 的 world tick 已返回健康通知，但测试中的 packet capture 为空。
- Root cause: `world.magicAttack`/`world.tick` 只构造并返回 `worldNotification`，不会自动调用 `deliverWorldNotifications`；测试只检查返回 slice，没有执行投递层。
- Prevention: 需要验证实际客户端包时，先断言 world 返回的通知 transcript，再显式调用 `deliverWorldNotifications`，最后检查 Send capture；不要把返回通知误认为已经写入连接。
- Verification: 补齐 cast/impact 两段通知投递后，目标/施法者 packet 顺序断言通过，且全量 race 测试通过。

### 2026-08-11 — 物品槽位 transcript 必须逐步计算交换与空位

- Symptom: `MoveItem(0→2)` 后的 P6 端到端测试把原槽 1 物品期望在槽 0，导致 logout 持久化断言失败。
- Root cause: 只看“移动到目标槽”的直觉，没有按 legacy `array[to] = array[from]`、`array[from] = oldTarget` 逐步推导后续 split 空位和 merge 槽位。
- Prevention: 物品事务测试先画出每一步的槽位、UniqueID、Count，再断言 split 产生的新槽和 merge 后的空槽；不要只按请求文字推断终态。
- Verification: 修正 transcript 后，Move/Split/Merge 的 net.Pipe 响应顺序和 JSON logout 持久化状态通过。

### 2026-08-11 — 装备 fixture 必须填充 legacy 默认关联字段

- Symptom: 装备单测中的物品明明类型、职业和性别都匹配，却被 `WeddingRing != -1` 规则拒绝。
- Root cause: Go 零值 `WeddingRing=0` 不等于 legacy `UserItem` 构造器的默认 `-1`，测试 fixture 没有显式还原该默认值。
- Prevention: 构造现代 `StoredItem` fixture 时显式设置所有非零语义默认值，尤其是 `SoulBoundID=-1`、`WeddingRing=-1`、空 sockets/awakening 与耐久度。
- Verification: 补齐默认字段后，装备/卸下限制、属性刷新和会话 transcript 均通过。

### 2026-08-11 — UseItem/DeleteItem transcript 必须区分副作用与回显字段

- Symptom: 迁移物品使用时，若把普通药水当作即时治疗，或把 DeleteItem 的实际裁剪数量写回响应，会与原版客户端可观察行为不一致。
- Root cause: legacy `UseItem` 对普通药水只累加恢复池、太阳水按 `ChangeHP` → `ChangeMP` 顺序产生健康包；`PlayerObject.DeleteItem` 在构造响应后才裁剪 `Count`，因此响应仍回显请求值。
- Prevention: 新增物品动作先从原版 handler、领域副作用方法和客户端响应处理共同列出完整 transcript；分别保存“实际状态变化”和“wire 回显字段”，并为每个分支增加 net.Pipe 顺序断言。
- Verification: 协议布局、普通/太阳水单测、太阳水健康包顺序、删除超量回显和 logout JSON 持久化测试通过。

### 2026-08-11 — 地面物品测试必须移动到真实拾取格

- Symptom: 地面物品/金币已经掉落到 `(1,0)`，测试却让玩家停在 `(0,0)` 直接拾取，结果没有收到任何拾取结果。
- Root cause: legacy `PickUp` 只遍历玩家当前 cell，不会在可见范围内自动寻找物品；测试只验证了可见性，没有验证当前格语义。
- Prevention: 每个掉落 transcript 先记录 origin、搜索到的 drop location、移动步骤和 pickup cell；可见不等于可拾取，拾取前必须显式移动到对象坐标。
- Verification: 修正 world fixture，并在双会话 transcript 中通过走到 `(1,0)`/`(2,0)` 后再拾取。

### 2026-08-11 — net.Pipe 多会话启动也必须消费 bootstrap 广播

- Symptom: 第二个会话启动后等待第一个会话的 `ObjectPlayer` 超时。
- Root cause: `world.enter` 之后服务端先向第二个连接写已有玩家对象，再向第一个连接写新玩家对象；`net.Pipe` 无缓冲，第二个连接未消费前服务端无法继续到第一个连接。
- Prevention: 多会话 transcript 先列出 bootstrap 广播顺序；启动第二会话后先消费其 `ObjectPlayer`，再等待第一会话的对等包；后续每次广播为所有接收者建立并发 reader。
- Verification: 双会话掉落/拾取/金币 transcript 通过普通测试和 `go test -race ./...`。

### 2026-08-11 — Storage bootstrap transcript 必须消费物品定义

- Symptom: 新增 Storage net.Pipe 测试时，启动 helper 期望先收到 `MapInformation`，实际先收到 `NewItemInfo`，导致 bootstrap 断言失败。
- Root cause: 角色包含 `ItemInfos` 时，Go Game bootstrap 与 legacy 一样会在地图和用户信息前发送每个物品定义；测试直接复用了只适用于空物品目录的 helper。
- Prevention: 每个 session fixture 先列出 `ItemInfos` 数量和完整 bootstrap 顺序；含物品定义时逐个消费 `NewItemInfo`，或使用显式支持物品目录的 helper，不能按测试名称猜测首包。
- Verification: Storage store/take-back JSON transcript 改为消费物品定义后，Storage 定向测试和 Go 全量测试通过。

### 2026-08-11 — 商店会话 fixture 必须恢复 RentalInformation 元数据

- Symptom: NPC 商店 Sell 失败门禁测试预期 Rental `DontSell` 返回普通失败包，但测试先收到“此处不能出售该物品”的类型限制聊天包。
- Root cause: fixture 只创建了 Rental 对应的 `ItemInfo`，没有把 `RentalInformation.BindingFlags=DontSell` 挂到实际背包物品；服务端因此按 `[TYPES]` 分支处理了它。
- Prevention: 测试 legacy 物品绑定规则时分别核对 definition 级 `ItemInfo.Bind`、instance 级 `StoredItem.RentalInformation.BindingFlags` 和 NPC 的 `[TYPES]`，不要只按索引命名推断 fixture 已具备语义元数据。
- Verification: 为实际 Rental item 补齐 binding flags 后，DontSell、Rental DontSell、类型限制的 net.Pipe transcript 与全量 Go 测试通过。

### 2026-08-11 — 无响应门禁测试不能调用 writeAndRead

- Symptom: 为验证 CALL 外部脚本的 active-script 页面门禁时，测试使用 writeAndRead 发送应被忽略的请求，net.Pipe 一直等待响应并触发超时。
- Root cause: 被拒绝的 NPC 请求按协议不会产生响应，writeAndRead 的读取步骤只能用于必有响应的请求。
- Prevention: 无响应断言只编码并写入请求，再发送一个确定有响应的 KeepAlive 作为处理完成屏障；需要包序列时显式区分忽略请求和响应请求。
- Verification: active-script 越权页测试改为 raw write + KeepAlive，定向控制流测试在 15 秒门禁内稳定通过。

### 2026-08-11 — NPC 脚本 fixture 必须检查源码实际反斜杠层级

- Symptom: 新增链式 refine fixture 时把双反斜杠 n 写成了 Go 源码中的双反斜杠，解析器看见字面量换行标记，测试在读取链式响应时阻塞。
- Root cause: JavaScript patch 字符串和 Go 字符串各有一层转义，写入后没有立即用十六进制/源码读取确认实际字节。
- Prevention: 生成脚本 fixture 后立刻检查目标行；Go 双引号字符串只保留单层反斜杠 n，跨工具层需要用普通字符串拼接，禁止凭视觉判断转义层级。
- Verification: 修正为真实换行转义后，GOTO/CALL 与 GOTO 特殊面板测试通过；此前同类换行边界规则继续适用。

### 2026-08-11 — 测试断言要区分 normalize 的状态修复与业务进度变化

- Symptom: 对旧版缺少任务明细的进度调用 normalize 后，测试把结构补全误判为“没有变化”或反过来误报业务进度变化。
- Root cause: 断言只看 `changed` 数量，没有先区分 schema normalization、任务状态变化和实际计数变化。
- Prevention: 测试同时断言规范化后的 task 数组、计数、完成状态和 packet 语义；不要用一个布尔值代表所有变化。
- Verification: legacy progress normalization、任务列表和 JSON round-trip 测试覆盖上述边界。

### 2026-08-11 — net.Pipe 多会话测试要先建立 reader 并消费完整 transcript

- Symptom: 组队邀请/离开测试曾因服务端写包等待客户端读取，或在第一包到达时过早检查共享状态而失败。
- Root cause: net.Pipe 没有缓冲；组队操作会向双方发送多包，最后一个响应不等于另一会话的清理已经完成。
- Prevention: 在触发跨会话操作前为每条连接建立异步 reader，按 legacy 顺序消费全部副作用包；检查 world 状态前使用会话完成 channel 作为屏障，并在读取共享 map 时持锁。
- Verification: TestSessionTwoPlayerGroupInviteAcceptAndLeave 覆盖邀请、接受、DeleteGroup、ObjectRemove 和断线清理，且 race 测试通过。

### 2026-08-12 — 会话测试账号必须符合认证层字符约束

- Symptom: RequiredGroup net.Pipe 测试使用带连字符的账号 ID，登录在测试 fixture 阶段被认证层拒绝，无法到达地图门禁断言。
- Root cause: 测试 fixture 只关注业务语义，没有先复用认证 parser 对账号 ID 的字符集约束。
- Prevention: 新增会话测试账号先从认证校验和现有 fixture 归纳合法字符集；优先使用仅含小写字母和数字的稳定 ID，不把非法账号错误归因于业务功能。
- Verification: 改用 `requiredgroup`/`rgleader` 等合法 ID 后，RequiredGroup 普通、登录恢复、多会话测试通过。

### 2026-08-12 — 跨会话地图变更必须延迟同步 session 局部状态

- Symptom: 组员被动离组时，world 已经把玩家移回安全地图，但直接在其他 session 回调里修改 `activeMap`/坐标会与主循环并发访问，且可能遗漏静态可见对象刷新。
- Root cause: world 通知在调用方或 ticker goroutine 投递，session 局部状态只应由所属读循环拥有；地图切换的 wire transcript 与本地状态提交被混在一个回调里。
- Prevention: 回调只发送换图包并把 transition 放入 mutex 保护的 pending 状态；所属 session 在下一次读包前统一更新地图、坐标、角色快照、持久化和静态可见对象。
- Verification: 双 net.Pipe 会话覆盖 DeleteGroup、ObjectRemove、MapChanged、UserLocation、ObjectPlayer、Chat 顺序；普通/race/vet/build 全部通过。

### 2026-08-12 — 邮件落库与在线到达 transcript 必须分别验收

- Symptom: 玩家邮件和交易溢出邮件已经写入收件箱，但在线客户端只收到 `ReceiveMail` 或完全没有即时刷新，缺少原版 `NewMail` 系统聊天及完整附件定义顺序。
- Root cause: 只沿持久化状态验证了 `MailInfo.Send` 的结果，没有迁移 `NewMail -> Process -> ReceiveChat -> CheckItem -> GetMail` 这条在线可观察链路。
- Prevention: 每个创建邮件的入口同时验收离线存储和在线 `Chat -> NewItemInfo* -> ReceiveMail` transcript；跨会话只生成通知快照，释放 world/mail 锁后再写连接。
- Verification: 普通邮件、带邮票附件邮件、交易取消溢出邮件测试均断言到达顺序，Mail 定向普通与 race 测试通过。

### 2026-08-12 — 跨会话同步必须按领域 revision 定向合并

- Symptom: Rental 为接收后台到期/死亡更新，把每次读包前的 world 同步扩大为整角色物品格和邮件覆盖，导致同一 session 刚完成的合成、装备、背包移动、精炼、修理、仓库和使用物品状态被 stale world 快照回滚；全量测试同时出现多类持久化和 transcript 失败。
- Root cause: world 快照既包含跨会话共享状态，也包含当前 session 自己拥有、但并非每条业务路径都立即回写 world 的局部状态；无版本判断的全量赋值把两种所有权混在一起。
- Prevention: 保留 Quest/Group 的既有定向同步；为 Rental 增加独立 revision，只在 Rental 生产路径实际修改网格/邮件时递增，并在 revision 变化时同步 Rental 所需字段。新增共享领域必须先明确所有权和变更版本，禁止为了接收一个后台字段而整角色覆盖。
- Verification: 修复后先前失败的 Craft、Equipment、ItemMove、Refine、Repair、Storage、Use/Delete 会话测试与 Rental 定向测试全部恢复通过。

### 2026-08-12 — ActionTime 迁移必须兼容连接队列与直接世界测试

- Symptom: 给 Turn/Walk/Run 写入 350/600ms `ActionReadyAt` 后，既有 net.Pipe 会话连续移动被同步读循环立即拒绝，大量距离门禁和可见性测试停在原地。
- Root cause: legacy 连接会把请求留在队列中等 `ActionTime` 到期再处理，而当前 Go 会话同步读取后立即执行；只迁移时间字段却没有迁移队列调度会改变外部行为。
- Prevention: 世界 helper 保留精确 capability/ActionTime 边界；会话入口保留单条 retry movement，在 `ActionReadyAt` 到期后重新派发，匹配 legacy `_retryList`，不能直接清零时间门禁。
- Verification: 直接 helper 的 capability 测试与连续 session movement、Craft/Shop 距离、地面拾取和静态可见性测试同时通过全量普通/race 门禁。
- Strengthening after P5 combat recurrence: melee、range、magic 也必须复用同一条会话 retry 边界；为其补门禁后，旧 `TestSessionFireBallTranscript` 仍把“立即收到冷却拒绝”当成正确结果，首次全包测试因此在 1800ms 后读到了重派成功的 `HealthChanged`。迁移 ActionTime/AttackTime/SpellTime 时必须同步审计既有 transcript 断言，区分世界 helper 的即时 capability 拒绝与连接层 `_retryList` 的延迟重派，不能为了保留旧 Go 测试而破坏 Legacy 会话行为。
- Verification after strengthening: FireBall 会话测试现锁定等待全局 SpellTime 后重派、再次扣 MP、更新方向并发送完整 Magic transcript；Haste/Fury 测试锁定 AttackSpeed 冷却，六技能测试锁定全局与单技能 CastTime 分离，`go test ./...` 与 `go test -race ./...` 全量通过。

### 2026-08-12 — 跨语言 round-trip fixture 不得直接比较含 map 的结构体

- Symptom: 公会导出 schema 测试首次编译失败，测试用 `!=` 比较包含 `ItemStats` map 的 `protocol.ItemInfo`。
- Root cause: 把结构体整体比较误当成通用零值检查，未先确认其字段是否全部可比较。
- Prevention: 为跨语言 JSON fixture 做零值或完整对象断言前，先检查 slice、map、pointer 字段；包含不可比较字段时统一使用 `reflect.DeepEqual` 或逐字段断言。
- Verification: 零值 creation-cost Item 改用 `reflect.DeepEqual` 后，`go test ./internal/worlddata -count=1` 通过。
- Strengthening after recurrence: `reflect.DeepEqual` 仍会区分 nil 与空 slice/map；wire parser 会把零长度 Slots、AddedStats、Awake.Values 规范化为空非 nil 容器。构造协议 round-trip fixture 时必须按 parser 的规范形状显式初始化这些字段，或逐字段比较语义值，不能把 nil/空容器差异误判成序列化错误。
- Second strengthening after recurrence: 即时 Buff 双连接测试又准备使用 `observerBuff != buff`，而 `ClientBuffInfo` 内含 `ItemStats` map 与 `Values` slice；虽在编译前复读时发现，仍说明新增断言没有先执行可比较性检查。今后写 `==`/`!=` 前先展开目标类型字段；协议对象默认逐字段断言，确需整体比较时才使用 `reflect.DeepEqual`，并同时明确 nil/空容器规范。
- Verification after second recurrence: 观察者 AddBuff 改为 `reflect.DeepEqual`，随后 `go test ./cmd/crystal-server -run '^$' -count=1` 和完整即时 Buff net.Pipe 测试均通过。

### 2026-08-14 — Observer transcript 新增 helper 后必须立即做包级编译

- Symptom: Observer 会话测试初稿引用了不存在的 UserInformation parser、错误哨兵和未解析的 Name 字段，随后真实 transcript 又暴露了目标连接自身的攻击/远程/施法包顺序与战斗退出锁。
- Root cause: 连续扩展测试时按意图猜测同包 helper 签名，并把观察者视角的转发包误当成目标连接唯一可见包，没有先复用现有 parser、核对 handler 写入顺序和 logout gate。
- Prevention: 新增 Observer 测试先检索整个 package 的 parser/错误值/返回签名，立即运行 `go test ./cmd/crystal-server -run '^$'`；每条多连接 transcript 分别列出目标、自身、观察者和退出 gate 的包矩阵，再写断言。
- Verification: 复用 `parseIntelligentCreatureSessionUserInfo` 并补齐 Name 后，Observer 拒绝、Admin 绕过、切换/generation、退出、持久 toggle、静态/地面对象顺序及服务端整包测试均通过。

### 2026-08-15 — Observer 接管新增可见物品时必须更新完整 transcript

- Symptom: RangeAttack 装备准入 fixture 为观察目标增加装备后，Observer 会话在 `ClientObserve` 后读到 `NewItemInfo`，旧测试却直接期待 `MapInformation`。
- Root cause: 目标的物品定义按连接可见性规则在观察接管阶段先发送；测试只更新了目标本身的装备状态，没有重新展开观察者接管的定义包顺序。
- Prevention: 任何会改变目标可见物品集合的 Observer/Inspect fixture，都要分别列出目标连接与观察者连接在请求后的完整 packet matrix，并在 `MapInformation` 前消费和解析新增 `NewItemInfo`。
- Verification: Observer transcript 已锁定目标装备定义 → `MapInformation` → `UserInformation` 顺序；RangeAttack 装备门禁相关定向测试和 `go test ./cmd/crystal-server -count=1` 均通过。

### 2026-08-15 — 支援魔法会话 fixture 必须保持在线状态与持久状态一致

- Symptom: SoulShield 会话 transcript 在预期 `AddBuff` 后读到额外 `HealthChanged`，失败后后台 session 仍尝试保存已被测试清理的临时目录。
- Root cause: fixture 在登录后把运行时 MP 改成远高于角色 MaxMP 的合成值，Buff 刷新按真实装备/等级重算并钳制生命资源；失败路径又先结束测试，掩盖了后续异步保存错误。
- Prevention: 网络 transcript 只在与登录 bootstrap 一致的 MaxHP/MaxMP 范围内修改运行时字段；若必须手工推进 ticker，先设置读取屏障并在断言失败前关闭会话，避免把 cleanup 后的后台日志当作首要根因。
- Verification: MP 改为当前 MaxMP 后，`HealthChanged → DeleteItem → UserLocation → Magic → AddBuff` 顺序和 JSON 物品/Buff 状态均稳定通过。

### 2026-08-14 — 新增会话测试变量必须避免跨阶段类型复用

- Symptom: Poisoning/Purification 会话测试包级编译失败，把 StoredMagic 结果赋给了前面用于协议 MagicResult 的同名局部变量，随后访问不存在的 Experience 字段。
- Root cause: 在一个长 transcript 中复用了语义相近但类型不同的变量名，没有在新增阶段前核对当前作用域已有声明。
- Prevention: 每个协议阶段使用带领域后缀的唯一变量名（例如 purifyPacket、storedPurify），新增测试后先执行受影响包的仅编译门禁，再运行行为测试。
- Verification: 本次失败发生在测试编译阶段且未运行服务端；修复后将用包级编译和 Poisoning/Purification 定向测试分别验证。

### 2026-08-14 — 扩展二进制 fixture 后截断测试要接受结构化 EOF

- Symptom: 为 RespawnSave 增加多条定宽记录后，截断数据库回归先得到 `count ... cannot fit`，旧断言只接受 `unexpected end of file`。
- Root cause: binary reader 会在计数声明与剩余字节明显不匹配时提前返回结构化容量错误，而不是继续读到 EOF；fixture 形状变化使该合法错误路径变得可达。
- Prevention: 截断数据库测试断言错误类别（EOF 或计数无法容纳），不要绑定单一读取深度的错误文本；每次扩展 fixture 后运行完整 exporter 包测试。
- Verification: 断言同时覆盖两种截断错误后，`go test ./internal/legacyworld -count=1` 通过。

### 2026-08-14 — 多连接 net.Pipe 世界通知前要建立会话就绪屏障

- Symptom: 全仓 race 门禁中 `TestSessionRequiredGroupEnforcementOnMemberLeave` 偶发在固定包数读取后因 `io: read/write on closed pipe` 超时；增加日志延迟后可通过，说明通知写入早于会话转换回调就绪。
- Root cause: 直接调用 world 的多连接强制迁移后立即向两个无缓冲 pipe 投递，测试只启动了读 goroutine，没有确认两条 session loop 已完成上一阶段处理。
- Prevention: 多连接 transcript 在直接触发世界通知前，为每个连接发送并读取 KeepAlive barrier；net.Pipe 的固定包数读取必须同时具备读 goroutine 和 handler-ready barrier，不能依靠调度偶然性。
- Verification: 加入双方 barrier 后该测试普通运行 5 次、race 运行 3 次及全仓 `go test -race ./...` 均通过。

### 2026-08-14 — Route 初始动作测试必须覆盖 Legacy 的绕行随机分支

- Symptom: route 测试第一次 tick 仍处于 Legacy 初始 `ActionTime` 时，注入源只接受 `Next(1000)`，却收到 `Next(2)` 调用而失败。
- Root cause: `MoveTo` 在直接 `Walk` 因动作冷却失败后，Legacy 仍会选择顺/逆时针绕行方向；测试只建模成功移动路径，遗漏了失败路径的随机调用。
- Prevention: 新增移动随机行为时先按 `MoveTo` 的“直行失败 → 随机侧向尝试”完整列出所有 roll bound，并让确定性测试源对每个边界显式返回值；不要让冷却门禁短路随机分支的验证。
- Verification: 测试注入源覆盖 `Next(1000)` 与 `Next(2)` 后，再验证初始冷却、route 移动、waypoint 等待和无效点重试。

### 2026-08-14 — 移动可见性矩阵 fixture 必须同时保存旧坐标与新坐标

- Symptom: route 可见性测试预期离开范围/进入范围的 `ObjectRemove` 与 `ObjectMonster`，实际所有接收者只收到 `ObjectWalk`。
- Root cause: 测试调用通知投影时仍把 monster 保留在旧坐标，距离差没有跨越 16 格边界；断言场景没有真正表达移动事件。
- Prevention: 构造移动通知 fixture 时显式传入 `oldX/oldY`，并保证待投影对象的坐标已经是 `newX/newY`；先逐接收者计算 old/new 可见性，再断言包序。
- Verification: 将 monster 新坐标设为 17、旧坐标设为 16 后，矩阵得到 leaving=`ObjectRemove`、staying=`ObjectWalk`、entering=`ObjectMonster`→`ObjectWalk`。

### 2026-08-15 — Monster AI 距离环 fixture 不得让观察者抢占目标

- Symptom: AI=4 SpittingSpider 定向测试预期攻击距离 2 的目标，却排入了距离 1 的观察者，导致延迟动作目标和预期时间均不符。
- Root cause: Legacy `FindTarget` 按距离环扫描，测试把可观察连接放在更近格子，观察者本身也满足普通玩家目标门禁。
- Prevention: 构造搜索/目标选择 fixture 时先列出所有在线玩家的距离环顺序；观察者应放在目标之后的环、设为安全区，或显式验证它不会成为合法目标，同时保持仍在数据可见范围内以覆盖通知矩阵。
- Verification: 将观察者移到目标之后的远环后，AI=4 的 `ObjectAttack`、400ms 直线命中、伤害/Green poison 状态和 observer 包序定向测试通过。

### 2026-08-15 — SandWorm 测试新增变量必须实际参与断言

- Symptom: SandWorm 定向测试新增 `impact` 变量后，Go 包级编译因 declared and not used 失败，行为测试尚未运行。
- Root cause: 为记录延迟命中结果引入了局部变量，但后续断言仍直接读取通知切片，没有把变量接入验证路径。
- Prevention: 新增测试局部变量后立即确认其用于断言、返回值或日志；先运行 `go test ./cmd/crystal-server -run '^$' -count=1`，编译绿色后再运行定向行为测试。
- Verification: 删除未使用变量后，后续将以包级编译、SandWorm 定向测试和完整 Go 门禁确认该测试实际执行并保持绿色。
- Strengthening after recurrence: AI=77 HellPirate 测试再次声明未使用的 `impact`，说明仅在测试结束前检查不足；新增每个分支的结果变量后，必须在同一 patch 中写入 HP、packet ID 或 payload 断言，并在继续扩展测试前运行仅编译门禁。
- Verification after strengthening: 本次复发在 `go test ./cmd/crystal-server -run '^$' -count=1` 阶段被捕获，修正前未执行行为测试；后续修复后将再次运行包级编译和 AI=77 定向测试。
- Symptom: AI=77 HellPirate 的 Fullmoon 目标/会话测试只允许注入 `Next(3)`，命中阶段实际收到 `Next(1)` 后失败。
- Root cause: Legacy `GetAttackPower` 及固定范围防御路径仍会消费 `Random.Next(1)`；测试 hook 只建模了分支选择，漏掉了延迟 impact 的固定范围抽样。
- Prevention: 确定性随机 hook 必须按 admission、延迟 impact 和每个防御/毒物 helper 的完整调用流接受所有实际 bound；`Next(1)` 不能被测试断言误判为非法调用。
- Verification after strengthening: 修复 hook 后 `go test ./cmd/crystal-server -run '^$' -count=1`、HellPirate 普通 `-count=10`、race `-count=3` 及认证 Fullmoon transcript 普通 `-count=5`/race `-count=3` 均通过。
- Symptom: 为确认 AI=77 target-kind 生产入口和 Cell insertion order，把直接 attack-helper 测试改为 `world.tick` 后，Fullmoon 的同格目标从低 ObjectID 玩家变为先插入的 owned Monster，旧断言失败。
- Root cause: 测试仍按 detached map 的 ObjectID 顺序理解 `Cell.Objects`，没有把生产入口、真实插入顺序和每格首目标作为同一判据。
- Prevention: target-kind/延迟动作测试必须驱动生产 `world.tick`；混合目标夹具显式调用 `recordObjectCellInsertionLocked`，并断言先插入对象的 `TargetKind`、HP 和 observer transcript，而不是依赖 map/ObjectID 顺序。
- Verification after strengthening: AI=77 定向测试在转换为 production-entry 后通过，混合同格目标确认先插入 owned Monster 被 Fullmoon 选中；随后重新运行包级编译、普通 HellPirate 测试和 race HellPirate 测试。

### 2026-08-15 — Monster AI opt-in 排除 fixture 必须避开已支持 AI 值

- Symptom: 新增 AI=8 后，既有“opt-in 后特殊 AI 不运行”测试把样本仍设为 AI=8，收到 `ObjectWalk` 而失败。
- Root cause: 扩展支持集合时没有同步检查负向 fixture，测试使用了刚刚变成生产路径的 AI 值。
- Prevention: 每次新增 AI/协议分支后，检索所有 `excludes`/opt-in 负向测试并将样本改为明确仍未迁移的值；同时保留一条正向测试锁定新值已启用，避免负向断言掩盖实际行为。
- Verification: fixture 改为 AI=11 后，AI=0/4/8/10 正向定向测试和特殊/宠物排除测试均通过。

### 2026-08-15 — 半月多目标测试必须展开范围广播接收者矩阵

- Symptom: AI=76 HellSlasher 半月测试按每个目标固定四包断言，目标 2 实际收到其他三个受击者的范围 `ObjectStruck`/`DamageIndicator` 广播后失败。
- Root cause: 把每个 action 的私有 `Struck`/`HealthChanged` 与对所有附近玩家重复投递的公共包合并成了单目标 transcript，未先按受击者和观察者展开顺序。
- Prevention: 多目标范围攻击先列出 action 顺序、每个受击者的私有包和每个观察者的公共包，再按接收者矩阵生成期望；不能对观察者复用目标自身的四包序列。
- Verification: 修正后将分别断言四个受击目标的 HP/私有包，以及目标 2/未命中观察者收到的完整公共广播序列，并重跑 AI=76 定向测试。
- Strengthening after recurrence: AI=94 FlameScythe 再次证明，多目标 action 按顺序投递时，目标自身的私有包在接收者 transcript 中可能处于第一项、中间或末尾；测试必须按每个 action 顺序分别生成目标/观察者期望，不能把同一“私有四包 + 公共包”模板复用给所有受击者。
- Verification after recurrence: AI=94 首次定向测试因把目标 3 的末尾私有包误放在开头而失败，按 target/hidden/ally 三种接收位置拆分期望后通过；MagicResist 排除者的三组公共包也保持独立断言。

### 2026-08-15 — 多次命中 transcript 必须应用 Struck 冷却

- Symptom: AI=96 FlameQueen 的第二个 50ms 间隔范围命中已正确扣血，但测试错误期望再次收到 `ObjectStruck`；实际目标和观察者只收到 `DamageIndicator`，目标另收到 `HealthChanged`。
- Root cause: 多段延迟动作测试按每次命中复用首次命中的包模板，遗漏了 Legacy `MonsterStruckReadyAt` 的 500ms 节流边界。
- Prevention: 多命中 transcript 先按每个命中时刻与目标的 `MonsterStruckReadyAt` 计算私有/广播包，再分别生成目标和观察者矩阵；后续命中不能默认追加 `Struck`/`ObjectStruck`。
- Verification: 修正 AI=96 第二次命中期望为 `DamageIndicator -> HealthChanged`（观察者仅 `DamageIndicator`）后，FlameQueen 定向测试通过。

### 2026-08-15 — 新增战斗 fixture 写入 ItemStats 前必须初始化 map

- Symptom: HellBomb AC/敏捷回归在行为阶段 panic，原因是向 `player.Stats` 写入高敏捷值时 map 为 nil。
- Root cause: fixture 只填了结构体标量字段，未沿生产角色初始化路径创建可写的 `protocol.ItemStats` map。
- Prevention: 新增战斗属性边界前显式用 map literal 初始化 `ItemStats`，并先运行定向测试而不是只依赖包级编译。
- Verification: 修正后将复跑 HellLord/HellBomb 定向测试，确认无 panic、AC 命中和敏捷负例均由实际行为断言覆盖。

### 2026-08-15 — AI=101 测试必须复用当前 Monster stat 标识符

- Symptom: AncientBringer 初版 Go 测试使用不存在的 `monsterStatMinMC`、`monsterStatMaxMC` 等名称，包级编译在行为测试前失败。
- Root cause: 测试夹具按 Legacy 统计概念猜测 Go 常量名，没有先读取当前 `worlddata`/monster AI 使用的完整 stat 标识符。
- Prevention: 新增怪物夹具前先在整个 Go package 检索生产声明和现有用法，逐项复用真实 `statMinMC`/`statMaxMC` 等标识符；首次 patch 后立即运行 `go test ./cmd/crystal-server -run '^$' -count=1`。
- Verification: AncientBringer 夹具改用当前 stat 常量后，包级编译、AI=101 定向测试和服务端整包测试均通过。

### 2026-08-15 — 重构后的测试 transcript 必须清理残留 objectID 引用

- Symptom: DemonGuard 经验测试从直接实体调用改为通知 transcript 后，包级编译发现旧的局部 `objectID` 引用仍残留在断言中。
- Root cause: 重构调用返回值后只更新了主要行为路径，没有按新的 `notification.Recipient.ObjectID` 语义逐处复核旧变量。
- Prevention: 重构测试的返回结构或身份来源时，立即搜索旧标识符并逐段复读所有断言；随后先运行受影响包的 `go test ... -run '^$'`，再运行行为测试。
- Verification: 残留引用在包级编译阶段被捕获并清理；DemonGuard/经验定向测试及服务端整包编译随后通过。

### 2026-08-15 — AI=105 测试字面量必须先过包级编译

- Symptom: KingGuard 测试新增 `Stats` 复合字面量时遗漏尾逗号，包级测试在执行行为前直接语法失败。
- Root cause: 连续扩展 fixture 与 transcript 时没有在首次完成复合字面量后立即运行编译门禁。
- Prevention: 每次新增或改写多行 Go 复合字面量后先执行 `gofmt` 与 `go test ./cmd/crystal-server -run '^$' -count=1`，再继续增加行为断言。
- Verification: 补齐逗号后包级编译恢复通过，KingGuard 定向测试及服务端整包普通测试均通过。

### 2026-08-15 — DeathCrawler 禁用 AI fixture 不应期待 ObjectWalk

- Symptom: AI=106 命中特效测试的毒伤 tick transcript 实际为 `[HealthChanged, DamageIndicator, Poisoned]`，测试却期待了前置 `ObjectWalk`。
- Root cause: fixture 明确设置 `monsterAIEnabled=false`，但断言沿用了启用普通怪物 AI 时的移动包；毒伤 tick 本身不会移动怪物。
- Prevention: 编写定向 transcript 时先列出 fixture 的开关状态及本次 tick 启用的处理器，再只断言可达包；普通 AI 产生的移动必须由独立启用 AI 用例覆盖。
- Verification: 删除 `ObjectWalk` 后 AI=106 四个定向测试通过，且命中测试仍锁定 HealthChanged、DamageIndicator、Poisoned 的顺序。

### 2026-08-15 — DeathCrawler 测试必须读取 world map 的权威怪物副本

- Symptom: DeathCrawler 致死测试报告局部 `monster.DeathCrawlerDeathAt` 为空，实际 `world.monsters` 已安排 500ms 后的死亡动作。
- Root cause: fixture 返回的是写入 map 前的局部结构体地址；`killMonsterLocked` 修改并保存的是 map value 副本，局部指针不会自动反映该状态。
- Prevention: 对 map-backed world entity 的变更断言先按 ObjectID 重新读取权威 map，再检查定时器、Dead 和后续动作；fixture 返回指针只用于初始身份/坐标。
- Verification: 测试改为读取 `world.monsters[monster.ObjectID]`，死亡延迟、隐藏相邻目标和范围外目标断言稳定通过。

### 2026-08-15 — DeathCrawler 多目标毒伤 transcript 必须保留逐目标通知顺序

- Symptom: 死亡毒雾测试观察者实际收到 `[Chat, DamageIndicator, ObjectPoisoned, HealthChanged, DamageIndicator, Poisoned, DamageIndicator, ObjectPoisoned]`，原断言把对象中毒包压缩到错误位置而失败。
- Root cause: `world.tick` 按玩家 ObjectID 逐个处理 Green poison；每个目标的 HealthChanged/Poisoned 与附近观察者的 DamageIndicator/ObjectPoisoned 会交错产生，不能只按“所有状态包”分组推断。
- Prevention: 多目标毒伤迁移先按处理顺序展开接收者矩阵，再逐接收者记录每次伤害和状态变化的包；使用协议常量断言完整序列，不用手工合并同类包。
- Verification: 修正 observer transcript 后 AI=106 定向测试通过，序列与现有 poison 处理路径的单目标/观察者测试保持一致。

### 2026-08-15 — 新增定向测试必须清理未使用的 fixture 返回值

- Symptom: AI=106 新增致死毒雾测试在包级编译阶段因 `crawler` 局部变量未使用而失败。
- Root cause: 测试 fixture 返回的怪物值在该用例只需通过 `world.monsters` 的权威副本验证，却仍绑定了局部变量。
- Prevention: 新测试完成后立即执行包级编译；不需要的多返回值显式使用 `_`，避免保留误导性局部状态。
- Verification: 将该返回值改为 `_` 后，`go test ./cmd/crystal-server -run '^$'`、DeathCrawler 定向测试、全仓普通/race、vet 和 build 均通过。
- Strengthening after recurrence: AI=107 远程测试又在首次包级编译中绑定了未使用的 `impact`；新增测试的编译检查必须覆盖每个新建局部变量，未用于断言的变量应立即删除或纳入可观察结果断言。
- Verification after strengthening: 为 `impact` 增加远程命中包序断言后，包级编译和四个 BurningZombie 定向测试均通过。

### 2026-08-15 — BurningZombie 延迟命中 transcript 必须包含攻击后的移动

- Symptom: AI=107 远程命中健康状态正确，但接收者实际收到 `[Struck, ObjectStruck, DamageIndicator, HealthChanged, ObjectWalk]`，测试只期待前四个包而失败。
- Root cause: Legacy `ProcessTarget` 在投射物命中后仍会继续执行；攻击冷却尚未结束时，目标仍在 `ViewRange` 内但 `CanAttack` 为假，于是怪物按正常路径移动。
- Prevention: 延迟攻击测试必须分别投影“命中处理”和同一 tick 后续 AI 处理；不要因为命中动作刚完成就假设该 tick 不再产生移动、转向或其他 AI 副作用。
- Verification: 远程 transcript 补上末尾 `ObjectWalk`，近战、远程、同格远程和离开视野移动用例均通过。

### 2026-08-15 — 同一 tick 的 AI 移动先于 poison tick transcript

- Symptom: AI=108 命中测试把 poison 的 `HealthChanged/DamageIndicator/Poisoned` 放在 `ObjectWalk` 前，实际接收顺序为 `Chat → ObjectWalk → HealthChanged → DamageIndicator → Poisoned`。
- Root cause: `world.tick` 先解析怪物延迟动作并运行怪物 AI，再在后续阶段处理刚加入的 poison；按领域语义分组而非按调度顺序断言，遗漏了跨阶段交错。
- Prevention: 延迟命中 transcript 必须按 `tick` 的实际阶段展开：动作 → AI → poison；新增状态效果后先记录同一 tick 内所有接收者包，再写期望序列。
- Verification: 修正 MudZombie 近战/远程 transcript 后，定向测试和包级编译均通过。

### 2026-08-15 — net.Pipe bootstrap helper 返回前必须同步 post-bootstrap 副作用

- Symptom: AI=108 全仓门禁偶发/复现失败 `TestSessionRequiredGroupEnforcementOnMemberLeave`，预 enforcement barrier 读到 `ServerObjectRemove` 而不是 `ServerKeepAlive`。
- Root cause: `startGameBootstrapForTest` 在 `GuildBuffList` 后返回，但服务端仍可能执行一次性的 required-group enforcement；测试此时手工把玩家移入受限地图，未完成的启动检查便把玩家异步移出并抢先写包。
- Prevention: bootstrap transcript 的最后一个业务包不等于 session 已进入稳定 game loop；凡是随后手工修改 world 在线状态的测试，先对每条 net.Pipe 连接发送 KeepAlive 并消费响应，作为 post-bootstrap barrier。
- Verification: 在 required-group fixture 的 world 投影前增加 leader/member 两个 KeepAlive barrier 后，定向测试、服务端整包、全仓普通/race、vet 和 build 均通过。

### 2026-08-15 — Go AI 定向测试的随机上界重叠必须按调用阶段断言

- Symptom: WhiteMammoth 抗性回归测试的 switch 同时声明了 `case 10` 和 `case poisonResistWeight`，包级编译在行为测试前失败。
- Root cause: 测试把同一个随机上界用于分支和抗性阶段，却试图用常量 case 区分调用语义。
- Prevention: 随机上界相同时按已知调用顺序或阶段计数记录消费，不要在 switch 中重复声明等值 case；新增测试后立即运行包级仅编译门禁。
- Verification: 测试改为统计 `poisonResistWeight` 的实际调用次数，包级编译和 WhiteMammoth 四条定向测试均通过。

### 2026-08-15 — ArcherGuard 定向 fixture 必须初始化运行态防御与 HP 投影

- Symptom: AI=113 首次定向测试命中后得到 20 点伤害而不是扣除 2 点 AC，且无效投射物用例的 `Character.HP` 仍为零。
- Root cause: fixture 只填了协议 `Stats`，没有填伤害路径实际读取的 `worldPlayer.MinAC/MaxAC`，也没有同步初始化 `SelectInfo.HP`。
- Prevention: 构造战斗实体时同时核对生产读取字段和协议持久投影；命中、未命中和失效动作都分别断言运行 HP 与 `Character.HP`。
- Verification: 补齐 `MinAC/MaxAC` 和初始 HP 后，ArcherGuard 的命中、PK 门禁、越界静止、失效重验与 NoFight 红名路径定向测试通过。

### 2026-08-15 — Mandrill 反应测试必须断言受击后的权威 HP

- Symptom: AI=114 heavy-hit teleport 测试已正确验证 effect-7 可见性和目标切换，但初版把 Mandrill 的 HP 期望写成受击前的 100，定向测试实际得到 90 后失败。
- Root cause: 测试只关注 teleport 副作用，未把同一 `attackMonster` 调用的伤害写入与 teleport 一起投影到权威怪物快照。
- Prevention: 多副作用战斗测试先列出调用顺序和每个权威实体的最终状态，再分别断言伤害、目标、位置和 packet transcript；不能用 teleport 成功推断 HP 未变化。
- Verification: 将断言修正为受击后的 HP=90 后，Mandrill DC close attack 与 effect-7 teleport 两个定向用例均通过。

### 2026-08-15 — SandSnail 区域毒伤测试必须包含同 tick 的首跳

- Symptom: SandSnail MAC 区域命中测试把邻近玩家的最终 HP 期望为 78，实际为 75。
- Root cause: 区域 MAC 命中先造成 20 点伤害，随后同一 `tick` 的 poison 阶段立即按当前 Go poison 调度再造成 5 点首跳，测试只计算了第一段。
- Prevention: 带毒的延迟区域动作必须把“命中伤害 → AddPoison/Chat → 同 tick poison 首跳”全部纳入最终 HP 和包序推导，不能只按 action.Damage 计算。
- Verification: 断言修正为 HP=75，并锁定 Chat、HealthChanged、DamageIndicator、Poisoned 顺序；SandSnail 定向测试通过。

### 2026-08-15 — Halfmoon 多目标 transcript 必须计入跨目标广播

- Symptom: SandSnail Halfmoon 两个相邻目标都受伤，但第一个目标的包序出现额外 `ObjectStruck`/`DamageIndicator`，初版只按自身命中四包断言失败。
- Root cause: 每个目标的 `ObjectStruck` 和 `DamageIndicator` 都广播给范围内其他目标；按目标分别命中而不是按接收者矩阵推导，遗漏了另一目标的公开包。
- Prevention: 扇形/区域多目标动作先按“动作顺序 × 接收者”展开 transcript，再断言每条连接的私有 `Struck/HealthChanged` 与公开广播包；不要复用单目标包序。
- Verification: Halfmoon 测试现分别锁定先命中邻居再命中主目标时两个接收者的重复公开包，HP 和 packet transcript 均通过。

### 2026-08-15 — Jar2 MC 测试必须沿用 Info 统计而非假设运行字段

- Symptom: AI=120 Jar2 测试夹具首次包级编译访问不存在的 `worldMonster.MinMC/MaxMC` 字段，行为测试尚未执行。
- Root cause: 只核对了 Legacy 的 MC 概念，没有先复用 Go 运行时实际的 `monster.Info` 统计读取边界。
- Prevention: 新 AI 引用 MC/SC/DC 前先读取 `worldMonster` 结构与 `monsterStatValue` helper；测试夹具只初始化生产路径真实读取的字段，并在新增复合夹具后立即运行包级仅编译门禁。
- Verification: 删除不存在的 MC 运行字段、保留 Info 的 MinMC/MaxMC 后，包级编译和全部 Jar2 定向测试通过。

### 2026-08-15 — CatShaman 测试随机夹具必须覆盖伤害与状态门禁的全部上界

- Symptom: AI=118 CatShaman 定向测试在红毒分支执行到 MC 伤害取值的 `Next(10)` 时失败；修复后又把命中同 tick 已执行的 Red poison 首跳误期望为 `Elapsed=0`。
- Root cause: 确定性 `monsterAIRoll` 夹具只列出了分支和部分旧调用的上界，且测试把延迟动作完成时刻与该 tick 后续 poison 调度边界混为一谈。
- Prevention: 新增 AI 测试先记录完整随机调用序列（分支、伤害、抗性、chance、毒值），夹具必须为每个实际上界提供确定值；延迟命中 transcript 同时投影命中、同 tick 移动和状态首跳，再断言 `Elapsed` 与包序。
- Verification: 加入 `Next(10)` 的固定返回并将 Red poison 的首跳期望改为 `Elapsed=1` 后，包级编译和全部 CatShaman 定向测试通过。

### 2026-08-25 — MAP-detail probe 必须拒绝重复地图详情

- Symptom: 独立 review 发现 production probe 在已请求并消费 `NewMapInfo` 后，仍把搜索阶段再次收到 `NewMapInfo` 当作可选兼容路径。
- Root cause: probe 只验证能解析两种 packet 顺序，没有把 Legacy `SentMapInfo.Contains` 的 repeat suppression 当成必须失败的协议判据。
- Prevention: probe 先消费一次 `WorldMapSetup -> NewMapInfo`，随后对同图搜索必须直接期望 `SearchMapResult`；任何重复 setup/map-info 都立即报错。
- Verification: probe 改为严格 `expectID(ServerSearchMapResult)`，新增 suppression 正例与重复 `NewMapInfo` 负例，定向 count-20 通过。

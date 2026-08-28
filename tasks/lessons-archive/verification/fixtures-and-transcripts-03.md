### 2026-08-18 — HealingCircle 测试必须读取权威值对象并覆盖完整广播

- Symptom: HealingCircle 世界测试第一次继续读取写入前的本地 `worldMonster` 副本，且只预期被命中目标的 `ObjectStruck`；认证会话还因未消费完整的慢速状态包而在 `net.Pipe` 写入处阻塞。
- Root cause: `world.monsters` 保存值类型，命中通知按范围广播给所有附近客户端；会话主循环也会在处理施法后继续推进真实时间的世界 tick。
- Prevention: 修改值类型对象后始终从权威 map 重新读取；按每个接收者的完整包矩阵断言广播；需要手动推进延迟效果的会话测试，将首个效果 tick 和过期时间移到真实时钟之后的合成时间点，避免后台主循环抢先消费。
- Verification: HealingCircle 世界测试已改为 map 重读并断言额外广播；认证会话连续三次通过，包含目标锁定、首个 `ObjectSpell`、命中和持久化。

### 2026-08-19 — Mirroring 测试夹具必须满足技能等级门槛

- Symptom: Mirroring 新 Clone 的世界测试首次失败，预期 admission `MagicLeveled` 为空。
- Root cause: 测试角色等级设为 30，而 Mirroring 的目录 `Level1` 门槛为 41；`levelMagicLocked` 正确拒绝了练习，生产实现并未丢失通知。
- Prevention: 新增法术测试夹具先读取 Go 对应的 Legacy 目录等级门槛，并把角色等级设到能练习的基线；同时保留低等级/拒绝路径的独立断言。
- Verification: 将夹具等级调整到 50 后重跑 Mirroring 定向测试，确认新 Clone admission 发出 `MagicLeveled`，而 NoPets/缺少定义路径仍不练习。

### 2026-08-19 — IceThrust 会话转录必须包含延迟命中练习包

- Symptom: IceThrust 会话世界状态和目标端包序正确，但施法者端实际收到 `ObjectStruck -> DamageIndicator -> MagicLeveled -> ObjectPoisoned`，测试期望漏掉了中间的 `MagicLeveled`。
- Root cause: 只按 FrostCrunch 的广播包矩阵复用施法者期望，忽略 IceThrust 的 `CompleteMagic` 命中后 `train=true` 与统一 `LevelMagic`。
- Prevention: 新法术会话测试先从 delayed resolver 的每个 `LevelMagic` 路径推导各接收者包矩阵，再分别断言世界通知和网络读取结果。
- Verification: 补入 `ServerMagicLeveled` 后由实际通知序列确认它先于主循环追加的 `ServerObjectPoisoned`；施法者期望已固定为 `ObjectStruck -> DamageIndicator -> MagicLeveled -> ObjectPoisoned`，会话定向测试、五次 race 测试及全仓库测试均通过。
  Recurrence evidence: 第二次会话失败不是功能差异，而是把延迟 poison 阶段的包放在 LevelMagic 之前；该顺序误判已纳入预防检查。

### 2026-08-19 — SpecialArrow 测试必须计入技能练习随机抽样

- Symptom: SpecialArrow 定向测试在命中和状态断言前失败，测试夹具把 bound=3 的随机调用报告为意外输入。
- Root cause: 夹具只建模了伤害和 Vampire/Poison 状态判定，却遗漏了命中完成后统一 levelMagicLocked 为技能练习抽取 combatRollLocked(3)。
- Prevention: 为带 LevelMagic 的延迟法术建立随机调用清单时，同时检查命中完成、状态结算和技能练习路径；严格 RNG 夹具必须显式允许并记录练习抽样，不得只覆盖法术专属随机数。
- Verification: 已从 Go levelMagicLocked 实现确认 bound=3 的调用点；修复夹具后 SpecialArrow 世界/会话定向测试、五次 race、go vet、全仓库测试/race 和 go build 均通过。

### 2026-08-19 — SpecialArrow 会话动作必须按目标类型断言

- Symptom: SpecialArrow 会话测试首次失败，断言玩家目标动作同时把 TargetID 和 PlayerTargetID 都当作目标 ID。
- Root cause: 测试忽略了 worldMagicAction 对目标类型的互斥表示；玩家目标只写 PlayerTargetID，普通 TargetID 保持 0。
- Prevention: 新增延迟动作会话断言前先按玩家、怪物、英雄三条 resolver 分支核对字段映射；不要从网络 MagicRequest 的 TargetID 直接推断内部动作字段。
- Verification: 失败转录显示 PlayerTargetID=目标且 TargetID=0；已将断言改为互斥字段并继续运行 SpecialArrow 会话测试。

### 2026-08-19 — SpecialArrow 会话夹具的法术等级必须与持久化等级一致

- Symptom: SpecialArrow 会话转录预期 PoisonShot 持续 10 秒，但实际 AddBuff 为 5 秒。
- Root cause: 会话账户保存的是 Level 0，而测试按世界夹具的 Level 1 计算了 5+5×level 的持续时间。
- Prevention: 会话测试的持续时间、伤害和练习断言必须从账户实际 StoredMagic 等级推导；世界测试等级与会话等级不同步时要显式标注。
- Verification: 按会话 Level 0 将 AddBuff、内存状态和持久化状态断言统一改为 5 秒，SpecialArrow 定向世界/会话测试通过。

### 2026-08-19 — SpecialArrow 会话毒状态公式必须使用实际法术等级

- Symptom: 修正会话 buff 持续时间后，目标毒状态断言仍失败；Level 0 的实际 duration 为 damage×2+7，而测试预期了 Level 1 的 damage×2+14。
- Root cause: 只同步了 buff 持续时间的等级修正，没有同步 SpecialArrow poison 的 magicLevel+1 公式。
- Prevention: 会话夹具统一从 action.MagicLevel 推导 buff duration、poison duration 和 poison value，禁止复用不同等级世界测试的常量。
- Verification: 失败状态显示 Level 0 的 duration/value，已将会话 poison duration 改为实际等级公式，SpecialArrow 定向世界/会话测试通过。

### 2026-08-19 — 新测试必须先检查包内辅助函数命名

- Symptom: SpecialArrow 定向测试首次编译失败，新增的 `mustWorldSpellInfo(*testing.T, byte)` 与包内已有的 `mustWorldSpellInfo(byte)` 重名，且调用参数数量不匹配。
- Root cause: 添加测试辅助函数前只检查了相邻测试文件，没有检索整个 Go 包的同名声明。
- Prevention: 新增包级测试 helper 前先用 `rg -n '^func <name>\(' cmd/...` 检查全包；已有 helper 直接复用，确需变体时使用带功能前缀的唯一名称。
- Verification: 失败输出已用于定位；删除重复声明并改用既有 `mustWorldSpellInfo(byte)` 后重新运行 SpecialArrow 定向测试验证。

### 2026-08-19 — PoisonSword 通知断言必须按接收者定位

- Symptom: PoisonSword 定向测试把整个 `BeforeMagicNotifications` 的固定下标当作删除包，玩家目标的中毒聊天通知插入前方后，删除包解析收到错误长度。
- Root cause: 即时扇形技能会在扫描过程中先为玩家目标追加目标侧 Chat，通知列表是全接收者混合序列，不能用全局下标代替接收者过滤。
- Prevention: 断言即时技能包序时先按接收者筛选，再按包类型/相对顺序定位；只有证明所有接收者通知都不会交错时才使用全局下标。
- Verification: 改为按施法者 ObjectID 查找 `ServerDeleteItem` 后，PoisonSword 世界层定向测试连续三次通过，并同时保留施法者 `MagicLeveled -> DeleteItem` 与玩家目标 `Chat` 顺序断言。

### 2026-08-19 — 全包验证需隔离既有 net.Pipe 会话阻塞

- Symptom: 单独运行现有 `TestSessionBlizzardAndMeteorStrikeTranscriptAndPersistence` 时，Blizzard/MeteorStrike 子测试在 30 秒后因 `net.Pipe` 读超时失败；堆栈显示服务端通知写入与测试端未消费读取互相等待。
- Root cause: 这是当前基线会话测试的 pipe 消费/通知突发阻塞，不涉及本批 PoisonSword 文件；重复并发启动全包测试还会把相同阻塞叠加，不能据此判断新法术失败。
- Prevention: 提交前先单进程运行新增功能的普通、race、会话测试，再运行全包；全包异常时用 `-json`/独立测试复现并检查堆栈，标明无关基线失败，不把它混入新功能证据。
- Verification: PoisonSword 世界/会话定向测试和五次 race 均通过；Blizzard 基线测试在不含 PoisonSword 的独立运行中仍复现同一 pipe 超时，且 `git diff` 未包含 Blizzard 源文件。

### 2026-08-18 — 测试夹具必须使用实际存在的统计字段

- Symptom: CatTongue 会话测试首次编译失败，引用了不存在的 `monsterStatMinMAC`/`monsterStatMaxMAC` 常量。
- Root cause: 将玩家/运行时对象的 MAC 字段命名习惯错误套用到怪物导入统计枚举；Go 怪物统计只提供 AC，MAC 夹具应在加载后直接设置对象字段。
- Prevention: 为新会话夹具添加怪物统计前先检索对应 `monsterStat*` 声明；若导入模型不支持某字段，使用运行时对象字段并在测试中显式覆盖。
- Verification: 移除不存在的导入统计后直接设置怪物 MAC，CatTongue 会话测试、全部 CatTongue 定向测试和 race 测试通过。
- Strengthening after recurrence: ArcherSummon 会话夹具再次错误使用不存在的 `monsterStatMinMAC`，首轮包编译立即失败；后续改为已声明的 `statMinMAC`/`statMaxMAC` 后通过目标包定向测试。
- Verification after recurrence: ArcherSummon 普通/race 定向测试、`go test ./...`、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-18 — 会话账号夹具必须遵守共同的 15 字符上限

- Symptom: CrescentSlash 会话测试在法术请求前收到 `ServerLogin` 的错误结果，未进入游戏阶段。
- Root cause: 测试账号 `crescentslashnet` 超过 Go/Legacy 账号 ID 的 3–15 字符校验范围。
- Prevention: 新增 net.Pipe 会话账号时先按认证层正则校验 ID/密码长度，并复用短、唯一、只含字母数字的账号名。
- Verification: 改为合法的 `cresnet` 后 CrescentSlash 会话完整通过，未再出现登录阶段错误包。

### 2026-08-18 — Warrior 扇形技能测试必须从旧版倍率和通知位置推导期望

- Symptom: HalfMoon/CrossHalfMoon 初始定向测试失败；测试把 CrossHalfMoon 侧向伤害按 HalfMoon 的 0.3 倍计算，并把 `MagicLeveled` 放在所有侧向命中之后。
- Root cause: 测试复用了 HalfMoon 的数值/顺序假设，没有逐项核对 Legacy `MagicInfo.GetDamage` 的 0.3/0.4 倍率以及 `HumanObject.Attack` 前方命中分支中先调用 `LevelMagic` 的位置。
- Prevention: 新增技能的伤害和包序断言必须直接由旧版公式、分支顺序和现有 Go packet helper 推导；不要从相邻技能复制常量或通知尾部位置。
- Verification: 修正 CrossHalfMoon level-0 侧伤害为 8、将 `MagicLeveled` 放到前方命中通知后且侧向通知前；世界、net.Pipe 会话和 race 定向测试通过。

### 2026-08-19 — 范围技能测试夹具必须设置实际读取的防御类型

- Symptom: BladeAvalanche 近格目标期望扣除 3 点 MAC，首次测试实际按 0 MAC 结算，HP 比预期少 3 点。
- Root cause: 复用了只初始化 `MinAC/MaxAC` 的普通战士怪物 helper，而 BladeAvalanche Legacy 分支使用 `DefenceType.MAC`。
- Prevention: 新技能测试先从 Legacy `DefenceType` 推导夹具字段；AC、MAC、ACAgility 和 Repulsion 分别显式设置，不能仅依赖通用 helper 的参数名。
- Verification: 为 BladeAvalanche 目标补充 `MinMAC/MaxMAC=3`，保留 AI=49 的 Repulsion 目标为零护甲，重新运行定向测试确认近格/外格/特殊分支伤害。
- Strengthening after recurrence: 特殊防御只改变护甲结算，不改变三格射线的 60% 外格倍率；AI=49 的第三格应按 `20*0.6-0=12` 伤害计算。
- Verification after strengthening: 分别断言 AI=49 近格 HP=60、第二格 HP=60、第三格 HP=68，覆盖防御和范围倍率的组合。

### 2026-08-19 — CounterAttack 测试必须分别固定 AC/MAC 与暴击边界

- Symptom: CounterAttack 定向测试首次失败：激活测试把不同基础 AC/MAC 当成相同值；玩家/怪物反击伤害分别得到 56/40 而不是未暴击值。
- Root cause: 角色基础 AC 与 MAC 是独立派生值，Buff 只对各自基础值加成；同时 Legacy 暴击条件是 `Random.Next(0, 100) <= Accuracy`，固定随机值 0 配合 `Accuracy=0` 会命中暴击边界。
- Prevention: 验证防御 Buff 时分别保存并比较 AC、MAC 四个基线；固定随机测试若要排除暴击，使用 `Accuracy=-1` 或让 bound=100 返回大于 Accuracy 的值，不能把 0 视为“不会暴击”。
- Verification: 修正测试基线与 Accuracy 后，重新运行 CounterAttack、FatalSword、TwinDrake、FlamingSword、Slaying 定向测试，确认伤害和防御断言按 Legacy 边界计算。
### 2026-08-19 — 会话技能经验持久化断言必须等待退出清理

- Symptom: CounterAttack `net.Pipe` 会话包序列和 world 运行时经验均正确，但断开前读取 auth 快照仍显示经验为 0。
- Root cause: Go 会话沿用退出清理边界，在 `serveWithConfig` 收到 EOF/Logout 后才从 world snapshot 调用 `UpdateCharacterMagics`；测试过早读取了持久化层。
- Prevention: 区分运行时状态与 auth 持久化状态；验证实时包/运行时后先关闭客户端并等待服务 goroutine 完成，再读取 CharacterByIndex 等持久化快照。
- Verification: 将 CounterAttack 会话的 HP/MP/技能经验 auth 断言移到 `done` 完成之后，确认退出清理后经验同步为 1。

### 2026-08-19 — MPEater 测试必须区分夹具约束、MP 变化和阈值时点

- Symptom: MPEater 初版测试先因公共战士夹具不接受 `SpellNone` 失败，随后把 Level 1/Accuracy 0 的目标 MP 期望写成 5，并把计数从 99 增至 100 后误判为当次已消费。
- Root cause: 测试没有先核对夹具允许的 spell 集合和 Legacy `ChangeMP`/“先累加、后检查 armed”语义；该配置每次只扣 5，20 应变为 15，达到 100 只武装下一次攻击。
- Prevention: 复用测试夹具前先检查其输入域；被动技能测试分开断言“本次累加后武装”和“下一次攻击消费”，并从 Legacy 公式推导 MP 前后值。
- Verification: 改用已有被动夹具后覆盖魔法表，修正 15/25 MP 与两次攻击阈值断言；MPEater 世界测试、认证 `net.Pipe` 测试和 Go 全量测试通过。
- Strengthening after recurrence: MPEater 首次会话断开后运行时 MP 已是 25，但 auth 快照仍回到初始 221；原因是被动 MP 变更没有调用玩家的 `PersistHealth` 回调，退出清理的 session cache 覆盖了实时值。
- Verification after strengthening: `changePlayerManaLocked` 在同步 `Character.MP` 后调用 `PersistHealth`；会话测试现在在服务 goroutine 结束后确认 auth MP=25、MPEater 经验=1。

### 2026-08-19 — 补齐持续效果时要更新既有测试的旧行为期望

- Symptom: 全量 Go 回归中 DemonWolf、DarkBeast、SackWarrior 的既有 Bleeding 测试仍期望延迟命中后只扣直接伤害；接入毒 tick 后实际同一 world tick 还按 Legacy 扣除了 Bleeding Value，并多发 Effect 18/持续伤害包。
- Root cause: 旧 Go 测试覆盖了毒 admission/state，却把尚未迁移的 `ProcessPoison` 缺口固化成了期望，没有对照 Legacy Player/HumanObject 的 Bleeding tick 顺序。
- Prevention: 新增持续效果前必须全量搜索该 poison type 的现有测试；按目标类型分别重算直接伤害、同 tick poison damage、final-tick/expiry 边界和 packet order，不能只让新增测试通过。
- Verification: 更新 4 组既有 Monster-AI/warrior Bleeding 断言为直接伤害加 Value，并加入 Effect 18、HealthChanged、DamageIndicator 的包序；定向 DemonWolf/DarkBeast/SackWarrior 回归通过。

### 2026-08-19 — 会话 transcript 手动 tick 必须避开实时维护循环

- Symptom: Go 全量回归中 `TestSessionTucsonMageNormalAttackTranscript` 偶发收到空攻击通知；单独运行同一测试可以通过。
- Root cause: 活跃会话读循环会持续执行 `world.tick(time.Now())`，测试同时用当前时间手动 tick，后台维护循环可能先消费一次性攻击。
- Prevention: 会话 transcript 若需要手动推进 world，应先停止可停的后台 ticker，并把测试时钟设为明显晚于实时 `time.Now()` 的未来时间，避免实时维护循环跨过测试事件。
- Verification: 将 Tucson transcript 基准时间改为 `time.Now().Add(time.Hour)` 后，定向测试连续 10 次通过，随后 `go test ./...` 全量通过。

### 2026-08-19 — Go map value 测试夹具必须整体写回嵌套结构

- Symptom: LionRoar 安全区测试在编译阶段报 `cannot assign to struct field ... in map`。
- Root cause: `world.mapRules` 是 `map[int32]worldMapRules`，直接修改 `world.mapRules[0].SafeZones` 试图写入 map 返回的结构体副本。
- Prevention: 修改 map value 的嵌套字段时先复制到局部变量，修改后用完整 value 写回 map；新增夹具先运行目标包编译测试再扩展行为断言。
- Verification: 改为 `rules := world.mapRules[0]`、修改 `rules.SafeZones`、`world.mapRules[0] = rules` 后，LionRoar world tests 通过。

### 2026-08-19 — Entrapment 推怪物的 transcript 必须区分观察者包与玩家私有包

- Symptom: Entrapment 首轮定向测试把怪物被推通知断言为 `Pushed`/九字节 payload，实际收到的是带 ObjectID 的 `ObjectPushed`；同批夹具还引用了项目不存在的 `minInt32` 辅助函数，先在编译阶段失败。
- Root cause: `pushMonsterLocked` 的 Legacy 对应路径只向附近客户端广播 `ObjectPushed`，玩家私有 `Pushed` 仅适用于被推动的 HumanObject；新增几何夹具没有先核对当前 Go 辅助函数清单。
- Prevention: 新增移动效果时按目标对象类型和接收者分别核对 packet ID/payload，怪物通知使用 `ObjectPushedPayload(ObjectID, x, y, direction)`；新增算术夹具先搜索现有 helper，再运行最小目标测试编译。
- Verification: Entrapment 世界测试按三次 `ObjectPushed` 的最终位置和认证会话按 `ObjectEffect -> ObjectPushed -> MagicLeveled -> ObjectPoisoned` 重新断言，世界/会话定向测试通过。

### 2026-08-19 — 延迟移动会话必须验证断开时的持久化坐标

- Symptom: SlashingBurst 世界状态与延迟 `UserLocation` 已移动到新坐标，但认证 `net.Pipe` transcript 关闭会话后，账号仍保存施法前坐标。
- Root cause: 世界 ticker/延迟动作可以在会话阻塞读取期间更新玩家；仅验证 world snapshot 和即时包不足以证明会话局部坐标及 logout map-runtime 写入已同步。
- Prevention: 每个会改变玩家坐标的延迟技能都要在会话测试中手动推进 action、读取私有位置包、触发一次会话边界刷新，再断开并断言 `Character.LocationX/Y/Direction` 与持久化 map runtime；修复必须同时覆盖 world snapshot 和 session-local projection。
- Verification: 本轮失败测试已复现“world=新坐标、stored=旧坐标”；修复后将重新运行 SlashingBurst world/session、race 与全量回归。

补充证据：首轮修复时曾把 runtime 局部变量放进 `leaveGameForObserver` 而不是持久化 defer，最小 Go 编译立即报未定义/未使用。预防为修改 defer/闭包时同时核对变量声明作用域、执行顺序和所有引用；随后 gofmt、服务端定向测试及全量 build 均通过。

补充证据：本轮 ShoulderDash 协议勘察中，一次 Go workdir 查询仍夹入了 Legacy 相对路径，导致 Legacy 文件找不到；命令保持只读且没有留下变更。预防再强化为：查询命令字符串只能引用当前 workdir 仓库，Legacy/Go 的相对路径不得跨命令复用；若需比对，必须拆成两个独立工具调用。

补充证据：同一勘察轮次的另一条 Go 查询再次在单一命令尾部加入 Legacy 相对路径，前半段 Go 查询成功、后半段 Legacy 路径失败。预防升级为：单仓库命令除了 workdir 外不得出现另一仓库目录名或相对路径；跨仓库比对必须在生成命令前拆分并逐条人工检查。

### 2026-08-19 — ShoulderDash transcript 断言必须保留推撞与移动的交错顺序

- Symptom: 世界测试把观察者包压缩成连续两个 `ObjectPushed`，实际 Legacy 顺序是每次 `ObjectPushed` 后紧跟该步 `ObjectDash`；零移动失败路径也会先广播 `ObjectDashFail`，再发送 Chat/MagicCast。
- Root cause: 测试只按效果类别分组推断顺序，没有从 Legacy 方法中的逐格循环和失败广播位置建立 transcript 序列。
- Prevention: 对逐步移动技能先收集完整 recipient packet IDs，再按 Legacy 循环顺序断言交错序列；失败路径必须把 self/observer fail packet 作为 transcript 的第一项纳入断言。
- Verification: 首次 ShoulderDash 定向测试输出已复现完整序列 `[ObjectPushed,ObjectDash,ObjectPushed,ObjectDash,ObjectStruck,DamageIndicator]` 及失败序列 `[ObjectDashFail,Chat,MagicCast]`，断言将按此修正。

补充证据：ShoulderDash 认证会话首次按类别修正顺序后仍把 `ObjectPushed` 的方向写成施法方向 2，随后又把 `ObjectStruck` 的目标方向写成 0；实际 Legacy `Pushed` 先将目标方向反转为 6，两处 payload 断言因此连续失败。
预防再强化为：凡是推撞技能，必须同时从 `Pushed` 的状态更新核对推撞包方向、后续受击包中的目标方向及每个接收者的 payload，不得只核对坐标和 packet ID。
Verification: 将两次 `ObjectPushed` 与最终 `ObjectStruck` 断言统一为反向 6 后，ShoulderDash 认证会话定向测试通过。

### 2026-08-19 — 会话延迟 action 测试必须隔离全局昼夜状态

- Symptom: Entrapment 会话回归把延迟 action 的期望写成 `[ObjectEffect,ObjectPushed,MagicLeveled,ObjectPoisoned]`，实际稳定多出一个 `ServerTimeOfDay`；race 与普通 `-count=10` 都可复现。
- Root cause: 测试把 `action.Due` 推到当前时间后一小时，`world.tick` 在处理 delayed magic 前先执行 `updateLightLocked`，跨过光照边界后向在线玩家广播昼夜包。
- Prevention: 会话 transcript 只验证目标行为时，必须关闭无关的全局 ticker/昼夜状态，或注入固定 light clock；不能用跨越环境边界的未来时间作为唯一隔离手段。
- Verification: 在 Entrapment 会话夹具停止 ticker 后显式关闭 `lightsEnabled`，重新运行定向测试与 race 定向回归，确认 transcript 不再含 `ServerTimeOfDay`。

### 2026-08-19 — 全量服务端回归失败必须区分既有 net.Pipe 基线

- Symptom: LevelEffects/Hair 批次的 `go test ./... -count=1` 中，服务端包仍复现 Blizzard/MeteorStrike 与 HealingCircle 的 net.Pipe 读写超时，并出现 TucsonMage 后台 tick 额外攻击包；本批两个新测试和所有内部包通过。
- Root cause: 这些会话测试的客户端消费边界与实时 world ticker 在既有基线中存在阻塞/竞态，失败堆栈不涉及本批 UserInformation/ObjectPlayer 投影代码。
- Prevention: 新批次先运行新增测试和相关包，再运行全量；全量失败时记录具体测试、超时和独立包结果，并用定向测试、race、vet、build 验证新改动，不能把既有 net.Pipe 基线失败标为本批回归。
- Verification: LevelEffects/Hair 定向测试通过；内部包全量通过；后续将单独复核上述既有会话测试，不改变本批实现以掩盖无关失败。

### 2026-08-19 — Reincarnation 会话夹具必须遵守 Legacy 名称长度

- Symptom: 新增 Reincarnation 双会话测试时，`CreateCharacter` 返回结果 1，测试尚未进入网络协议阶段。
- Root cause: 夹具角色名超过 Go/Legacy 兼容的 15 个字符上限，失败被误判为会话初始化问题。
- Prevention: 创建会话夹具角色时使用 3–15 字符名称，并在启动网络会话前断言创建结果为 10；不要用业务完整名称替代协议夹具标识。
- Verification: 改用 `ReincCaster`/`ReincTarget` 后，双会话测试成功完成登录、施法、延迟完成和接受复活流程。

### 2026-08-19 — DarkBody 测试必须包含通用隐身清理与观察者广播

- Symptom: 初版世界测试错误地期待目标只收到 `ObjectMonster`，并把第二次无效目标施法断言为没有任何 pre-Magic 通知。
- Root cause: 新 clone 的可见 AddBuff/Hidden 会通过 `notifyPlayersExcept` 广播给附近观察者；而施法入口在玩家已 Hidden 时会先按 Legacy 通用路径移除 DarkBody，产生 `RemoveBuff`/`ObjectHidden(false)`，即使本次目标无效也如此。
- Prevention: 为每个施法测试分别收集 caster/observer 的包 ID，并在断言前先追踪通用 Magic 前置阶段（隐藏清理、方向、MP）；不要只根据专用 spell helper 的返回值推断完整 transcript。
- Verification: 更新目标观察者包矩阵并保留无效目标后 clone 存活/不重复练习断言，DarkBody 定向测试与全量服务端测试通过。

### 2026-08-19 — 手工世界 fixture 的属性公式必须明确 StatsInitialized 语义

- Symptom: DarkBody AC 时长测试预期 6000ms，却得到 2500ms。
- Root cause: fixture 设置 `StatsInitialized=true` 后，`enter` 按空装备重新计算 AC，把手工 `MinAC=7/MaxAC=11` 替换成运行时基础值，公式本身没有错误。
- Prevention: 测试要验证手工战斗属性时保持 `StatsInitialized=false`；只有要覆盖装备/登录重算时才显式启用该标志，并在施法前断言参与公式的 AC/MC/DC 值。
- Verification: 移除 fixture 的初始化标志后时长恢复为 6000ms，公式、buff payload 和 DarkBody 全量测试均通过。

### 2026-08-19 — Hero/Monster 测试对象 ID 必须遵守全局 nextID 分配

- Symptom: Hero-target DarkBody 测试首次没有伤害，clone 移动后目标 ID 变成了另一个对象。
- Root cause: 手工 Hero 使用了即将由 world `nextIDLocked` 分配给新 clone 的 ID 3，导致普通宠物 target lookup 优先命中自身 Monster 并清除 Hero 目标。
- Prevention: 测试中新增运行时对象后，先读取 world 已分配的 ID；手工对象使用明确不与 `nextIDLocked` 冲突的 ID，并在断言中验证 target ID 全局唯一。
- Verification: 将 Hero fixture 改用 ID 99 后，clone 保留 Hero target 并在 action boundary 后造成预期伤害，相关普通宠物回归通过。

### 2026-08-19 — DarkBody 召回 transcript 必须按 pre-Magic 入队点断言

- Symptom: 首版 net.Pipe 召回测试期待 `ServerMagic` 先于 `ServerObjectDied`，实际读到 death packet 后测试失败并关闭 pipe。
- Root cause: DarkBody 的 `Die()` 等效通知被放入 `BeforeMagicNotifications`，服务端按 Legacy 入队点在最终 `ServerMagic` 前发送；只有 delayed Mirroring 的 death 才位于最终 Magic 之后。
- Prevention: 迁移立即效果或召回效果前，先按源码调用点区分 pre-Magic 与 post-Magic 队列，session transcript 同时覆盖成功 spawn 和 existing-object cleanup。
- Verification: 调整为 `HealthChanged -> UserLocation -> RemoveBuff -> ObjectHidden -> ObjectDied -> Magic` 后，DarkBody session、race、全量 Go 测试通过。

### 2026-08-19 — Portal 容量测试必须先满足通用施法距离门禁

- Symptom: Portal 双门户容量测试预期已经扣除法力，但实际返回未施法且 MP 未变化。
- Root cause: 测试目标点距施法者超过 Portal 的目录 Range=9，Magic 入口在专用容量判断前按 Legacy 通用范围门禁直接返回。
- Prevention: 为法术专用失败分支写测试时，先确认请求通过通用 CanCast、Range、CastReady 和 MP 门禁，再单独构造专用失败条件；范围外行为另设测试。
- Verification: 将容量测试目标移入 9 格范围后，确认 MP 已扣除、`Cast=false` 且没有延迟动作，Portal 定向测试全通过。

### 2026-08-19 — 装备属性测试必须断言 RefreshStatCaps 后的最终值

- Symptom: 接入 Legacy 末端属性上限后，RedFlower 套装与 Mir 套装定向测试仍期待未归一化的 `MP -50` 和 `MagicResist +6`，导致回归失败。
- Root cause: 测试把 `RefreshItemSetStats` 的中间结果当成了 `RefreshStats` 对客户端可见的最终结果，漏掉了随后执行的职业上限与非负/最小最大值归一化。
- Prevention: 对照 `RefreshStats` 时按完整调用顺序建立最终值断言；凡是新增末端 cap/normalize，都要复查已有套装、负值和抗性测试的期望，不只新增正向夹具。
- Verification: RedFlower 改为按基础 MP 归零、Mir MagicResist 改为职业上限 2 后，装备属性定向测试通过；新增异常属性向量覆盖全部 cap 与战斗范围归一化。

### 2026-08-19 — 生产代码不能复用仅在测试文件定义的通用 helper

- Symptom: 装备特殊模式批次首次定向测试在编译阶段因 `containsInt16` 与 `support_buffs_test.go` 的测试 helper 重名而失败。
- Root cause: 新增生产代码时只检查了测试侧已有 helper 名称，没有考虑 `_test.go` 符号与正式构建的可见范围，导致测试构建出现重复定义。
- Prevention: 新增生产 helper 使用领域前缀命名，并在测试前同时检查生产包和 `_test.go` 的符号；不要依赖只存在于测试文件中的辅助函数。
- Verification: 将 helper 改为 `containsEquipmentMode` 后，配置、协议、装备定向测试及 `cmd/crystal-server` 全包测试均通过。

### 2026-08-19 — 新增协议常量必须同步完整 ID 映射测试

- Symptom: 全仓测试在新增 `ServerPlayerUpdate=56` 后失败，完整协议 ID 测试把该名称的 `got` 值读成 0。
- Root cause: 只新增了常量和局部 payload 测试，没有同时把常量加入既有的完整 `wants`/`got` 映射表。
- Prevention: 每新增一个协议 ID，必须同时更新常量、payload/parser 测试、完整 ordinal 映射和唯一性表，并运行 `go test ./...`。
- Verification: 补齐 `ServerPlayerUpdate` 的 `got` 映射后，协议定向测试通过，随后重新执行全仓测试。

### 2026-08-19 — 特殊 Buff 测试必须区分相同包 ID 的 payload 顺序

- Symptom: 特殊装备 Buff 移除定向测试把两个 `RemoveBuff` 包误期望成一个；修正实现后又发现仅比较包 ID 无法验证 `Skill` 与 `ClearRing` 的反向遍历顺序。
- Root cause: `Skill` 和 `ClearRing` 都使用同一个 `ServerRemoveBuff` ordinal，测试只断言 ID 数量，没有解析 payload 的 Buff 类型；同时实现最初按正序扫描，偏离 Legacy `ProcessBuffs` 的倒序扫描。
- Prevention: 对同一 ordinal 的连续事件必须同时断言 payload discriminator 和顺序；迁移 Legacy 倒序列表逻辑时，先固定源端遍历方向，再写包序测试。
- Verification: Go 测试现在断言 `Skill=113`、`ClearRing=114` 的倒序移除及随后 `ObjectHidden`，定向测试与整个 `cmd/crystal-server` 包均通过。

### 2026-08-19 — GM 权限测试不能假设冷却检查也被绕过

- Symptom: `@FIND` 定向测试先成功查询后立即用 GM 身份重复查询，错误地期望直接得到“目标不在线”；Go 实现按 Legacy 顺序返回了剩余 180 秒冷却提示，测试失败。
- Root cause: Legacy `FIND` 先无条件检查 `LastProbeTime`，GM 只绕过权限判断和冷却写入，并不绕过已有冷却；测试把权限 bypass 误当成 cooldown bypass。
- Prevention: 迁移命令时把“权限门、冷却检查、冷却写入、目标查找”拆成有序断言；GM 测试必须分别覆盖已有冷却和冷却已过期两种状态。
- Verification: 测试改为在三分钟冷却结束后验证 GM 的缺失目标响应；Probe 权限、180 秒冷却、目标位置和会话包序测试均通过。

### 2026-08-19 — 保留已读取的地图规则字段时必须同步所有版本 fixture 期望

- Symptom: 读取 `NoPosition` 后，版本化地图 schema 定向测试只有 version 114 因期望仍为 false 失败。
- Root cause: parser 变更保留了此前丢弃的字段，但只更新了部分版本期望/fixture，未将同一二进制位置的字段贯穿所有版本用例。
- Prevention: 恢复任何旧字段时，逐个检查所有版本 fixture 的读取顺序、期望结构和导出 round-trip，并先运行完整 schema 测试。
- Verification: 所有版本 `TestReadMapInfoRuleFlagsPreserveLegacyVersionOrder` 均通过，`NoPosition` 从 legacy binary 到 world rules 的导入链已覆盖。

### 2026-08-19 — 装备耐久迁移必须更新严格随机序列与合成时钟

- Symptom: 耐久接入后多个 session 探针拒绝新增的 `Next(4)` 或十三次 `Next(1)`；Tucson、Blizzard、HealingCircle 在整包运行时还因合成时间跨昼夜收到未预期的 `TimeOfDay` 包而阻塞。
- Root cause: Legacy `DamageWeapon` 即使武器槽为空也消耗一次 `Next(4)`，`DamageDura` 对每个非武器槽（包括空槽）消耗一次 `Next(1)`；部分 session 测试把真实光照时钟与一小时后的合成 tick 混用。
- Prevention: 为严格随机回调使用按命中次数生成的显式 bound 序列；会话测试凡是推进合成时间，都在启动 ticker 前注入固定光照时钟，避免把全局维护包混入战斗 transcript。
- Verification: 修正重复测试 helper、零值 bound 期望、所有受影响 session 回调和三个合成时钟测试后，定向耐久/技能/session 回归与 `go test ./... -count=1 -timeout=5m` 通过。

### 2026-08-19 — Tornado 推送测试必须区分私有 Pushed 与广播 ObjectPushed

- Symptom: AI=83 Tornado 范围测试初版把原始玩家应收到的包固定为 `ObjectRangeAttack` 加两次 `Pushed`，实际还收到其他被推动对象的 `ObjectPushed` 广播，定向测试失败。
- Root cause: Legacy `FindAllTargets(..., false)` 会逐对象推动，Human/Monster/Hero 的推送广播会按观察范围送给原始玩家；测试只建模了被测玩家自己的私有包，遗漏了跨目标广播。
- Prevention: 推送类 AOE 测试分别断言攻击包首包、私有 `Pushed` 次数、最终坐标和伤害队列；只有在对象集合与广播接收矩阵均固定时才断言完整 ordinal 序列。
- Verification: 调整断言后，AI=83 Tornado adjacent/range/search 定向测试通过；范围测试同时覆盖隐藏玩家、owned Monster、Hero、方向与 `dist-1` 推送距离。

### 2026-08-19 — FlamingMutant AOE 测试必须按推送阻挡与效果 payload 建模

- Symptom: AI=85 FlamingMutant 定向测试首轮把每个目标都预期为完整 `dist-1` 推送，并按通知接收者查找 `FlamingMutantWeb`；实际坐标受先前目标占位阻挡，Monster/Hero 的效果通知接收者也不是被施加对象。
- Root cause: `Pushed` 是逐格尝试并遵守占位阻挡；`MonsterObject.Broadcast` 按观察范围发送，目标身份只存在于 `ObjectEffect` payload 的 `ObjectID`，不能用 recipient 推导。
- Prevention: 推送 AOE fixture 先固定对象处理顺序和每一步占位，再断言最终坐标；效果/状态广播测试按 payload discriminator 与 object ID 检索，并分别覆盖 Player/Monster/Hero。
- Verification: 修正 FlamingMutant fixture 后，AI=85 近战延迟伤害/三类目标推送、远程 MC 区域伤害/麻痹/Web 效果及人类二次抗性拒绝测试全部通过。

### 2026-08-19 — Go 延迟攻击测试必须先解析既有队列再断言后续伤害

- Symptom: ManectricClaw 相邻目标的冷却回退测试在第二次近战落点一次解析了首次 500ms thrust 和后续 300ms melee，HP 实际下降两次；随后又错误地把已解析的首个 action 计入队列长度。
- Root cause: 延迟 action 是按 `Due` 独立保留并在同一 tick 累积解析的，测试把“动作已发出”误当成“动作仍在队列中”，没有建立每个 impact 的前置状态。
- Prevention: 测试多段延迟攻击时，先推进并断言前一 action 的落点，再创建/检查下一 action；最终 HP 断言必须明确包含已完成的历史伤害。
- Verification: ManectricClaw 三列 thrust、当前坐标/朝向、双重抗毒和冷却回退测试在分段解析后通过。

### 2026-08-19 — AI=87 扇形测试必须按排除的 Front 方向遍历对象

- Symptom: ManectricBlest Type 1 定向测试在遍历期望对象 ID 时因 Front 方向没有创建对象而解引用 nil，测试 panic。
- Root cause: 夹具创建对象时跳过了 Legacy 明确排除的 Front 单元格，但断言仍把对象 ID 当作连续区间处理。
- Prevention: 几何攻击测试的断言必须复用与实现相同的方向域和排除条件，通过方向计算对象 ID；不能用带空洞的连续 ID 区间代替单元格集合。
- Verification: 断言改为遍历八个方向并跳过 Front 后，AI=87 四个行为测试、全 Go 测试包和定向 race 均通过。

### 2026-08-19 — WingedTigerLord AOE 测试必须区分目标私有包与邻格广播

- Symptom: Tornado 新增测试把目标收到的完整 packet ID 列表固定为单个命中的 Struck/Health/Effect/Chat 序列，实际列表还包含邻格目标的伤害和状态广播，定向测试失败。
- Root cause: Legacy AOE 对每个捕获目标分别命中，`ObjectStruck`、伤害和状态包会按观察范围广播给同一观察者；目标自身的私有包只构成序列前缀，不是整个 recipient transcript。
- Prevention: AOE 测试先按 payload/object ID 区分私有包、广播包和每个目标的状态，再对单目标前缀或通知集合断言；只有夹具只包含一个可见目标时才固定完整 recipient ordinal 序列。
- Verification: 失败栈定位到 `winged_tiger_lord_test.go` 的完整序列断言，未进入 Winged resolver 错误；将测试改为按广播矩阵验证后重新运行 AI=84 定向测试。

### 2026-08-19 — ChatItem 嵌套 round-trip 夹具必须使用解析器的 canonical 空集合

- Symptom: 新增 `NewChatItem`/`ChatItemStats` 嵌套物品 round-trip 测试首次用 `reflect.DeepEqual` 失败，线材字段相同但解析出的空 `Slots`/`Awake.Values` 与夹具的 nil 表示不同。
- Root cause: `ParseStoredItemPayload` 会把零长度数组规范化为非 nil 空 slice，而测试夹具只初始化了部分嵌套节点的空集合。
- Prevention: 新协议嵌套记录的 fixture 必须显式构造解析器会产生的 canonical 空 slice/map，或只比较语义字段；不能把 Go nil/empty 表示差异误判为 wire 不等价。
- Verification: 为顶层和嵌套 `StoredItem` 补齐空 `Slots`、`AddedStats`、`Awake.Values` 后，`go test ./internal/protocol -count=1` 通过。

### 2026-08-19 — TrapHexagon 测试必须直接使用 Legacy 0 级持续时间并隔离可见对象移除

- Symptom: TrapHexagon 首轮测试把 0 级持续时间断言为 15 秒；静态可见性移到远处时又把离开视野的怪物移除包误计入八个字段的移除数量。
- Root cause: Legacy 公式是 `(Level * 5 + 10) * 1000`，0 级为 10 秒；`refreshStaticVisibility` 会同时维护怪物和法术效果两类可见对象。
- Prevention: 行为测试的持续时间从基线公式逐级代入计算；可见性断言按 ObjectID/载荷区分对象族，不用所有 `ObjectRemove` 的总数代表单一效果。
- Verification: 修正测试期望后，TrapHexagon 定向测试重新覆盖 admission、500ms completion、八点坐标、精确到期和静态可见性移除。

### 2026-08-19 — 两段延迟技能测试必须推进实际队列时间点

- Symptom: ArcherSummon 测试首次把 admission 返回的最终 `ProjectileDelay` 当作第一段 action 的 `Due`，因此只推进到练习阶段，随后错误断言宠物已经生成。
- Root cause: Legacy/Go 把距离延迟、练习阶段和第二个固定 500ms map action 分别保存在队列；admission 的总延迟不是首个队列动作的到期时间。
- Prevention: 延迟技能测试先读取队列中的实际 `Due`，推进首个 action 并断言它产生的后续 action，再读取新的 `Due` 推进最终 impact；不要从 admission 总延迟倒推中间阶段。
- Verification: ArcherSummon 三个法术的两段生成、召回二段延迟和 session `MagicLeveled -> ObjectMonster/ObjectHealth` 测试通过。

### 2026-08-19 — net.Pipe 会话写入必须允许服务端先发送待处理通知

- Symptom: ArcherSummon 会话之后运行 TrapHexagon 会话时，Trap 测试客户端在同步写 keep-alive 处挂起；单独运行 Trap 测试可通过，服务端栈显示在读取客户端帧前阻塞写一帧 live-tick 通知。
- Root cause: `net.Pipe` 没有缓冲；客户端同步 `Write` 等服务端读取，而服务端主循环会先 tick 世界并写出待处理通知，双方在没有读端的情况下互等。前序会话改变时序后稳定暴露该测试竞态。
- Prevention: 会话测试发送下一帧时，用独立 goroutine 写入，同时主测试 goroutine 持续读取并消费允许出现的中间通知，最后再等待写入完成；不要假设服务端在每个客户端写入前都没有 tick 输出。
- Verification: ArcherSummon+TrapHexagon 组合会话重复 3 次通过；随后 `go test ./... -count=1 -timeout=600s`、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-19 — AI=5 测试状态变量必须保持领域类型独立

- Symptom: CannibalPlant 定向测试首次编译失败，生命周期测试中的 `state` 是 `worldMonster`，攻击断言又把 `world.players[...]`（`*worldPlayer`）赋回该变量并访问 `Character`。
- Root cause: 在同一测试函数中复用了语义相近但领域类型不同的局部变量名，Go 编译器在首次行为测试前才暴露了错误。
- Prevention: 测试跨 Player/Monster/Hero 读取时使用带领域含义的独立变量名（如 `monsterState`、`targetState`），提交前先运行目标测试包的空编译和定向测试。
- Verification: 将玩家断言改为独立的 `targetState`；随后重跑 AI=5 定向测试并确认编译与行为断言通过。

### 2026-08-19 — net.Pipe AI transcript 必须隔离 live tick 与手动时钟

- Symptom: Guard session 首轮在服务线程中触发了手动 `monsterAIRoll` 对 `3000` 的断言并使测试后续访问空玩家；此前同类 FurbolgGuard session 则曾先消费一个预期外的 `Random.Next(2)`。
- Root cause: session 测试安装了只接受 transcript 随机边界的 roll 后仍让 live AI 运行；服务循环在 bootstrap 或手动未来时钟之间初始化/移动怪物，既会消费无关随机数，也可能改变测试状态。
- Prevention: net.Pipe transcript 从启动服务前保持 `monsterAIEnabled=false`；停止 session ticker 后，在锁内直接调用目标 AI 处理并把结果交给后续 delayed-action `tick`，用 keep-alive barrier（需要时）确认服务循环回到读帧状态，避免 live tick 与 transcript 共用随机流。
- Verification: Guard session 定向测试与 `-race` 通过；FurbolgGuard session `-race -count=3` 及随后 `go test ./... -count=1 -timeout=600s` 通过。
- Strengthening after Deer AI=2 recurrence: Deer session 初版在停止 ticker 后重新启用 `monsterAIEnabled`，连接读循环仍以真实时钟执行了一次失败的逃跑 Walk，并额外消费 `Random.Next(2)`，导致只接受 transcript 随机边界的回调失败；问题不是 Deer 的手工未来时钟路径。
- Strengthened prevention: 认证 AI transcript 保持 live AI 关闭到手工状态写入完成；在锁内直接调用 `processMonsterAILocked`（而不是重新启用后调用 `world.tick`），再交付返回通知。只有明确要覆盖实时维护时，才用互斥记录并过滤允许的维护随机边界。
- Verification after Deer AI=2 recurrence: Deer 世界目标发现/直线逃跑/fallback、认证 `ObjectWalk` transcript、空编译及定向测试均通过；认证测试不再消费任何非 transcript 随机数。

### 2026-08-20 — AI=3 Tree 测试必须区分入口前置随机与 Tree 命中随机

- Symptom: Tree 玩家命中测试首次只期望 `[1,1]`，实际普通攻击入口还消费了一个 `bound=4` 的 FatalSword admission 抽样；敏捷 miss 夹具把 Tree 敏捷设为 0，`Random.Next(1)` 因而只能命中。
- Root cause: 将完整 `attackMonster` 的前置随机流误当成 Tree override 的局部随机流，并忽略了 Legacy 单位上界随机在 `Next(1)` 时仍消耗调用但返回 0。
- Prevention: 测试先分别记录入口前置与目标 override 的随机边界；需要构造 miss 时使用至少 1 的敏捷和越过准确率的返回值，不能用 `Next(1)` 作为 miss 场景。
- Verification: Tree 定向测试现锁定 `[1,1,4]` 的完整玩家入口序列，并用敏捷 1/返回 1 验证静默 miss；`go test ./cmd/crystal-server -run 'Tree' -count=1` 通过。

### 2026-08-20 — 引入 AI=3 Tree 后旧通用怪物夹具必须改用明确的普通 AI

- Symptom: 全包测试在 `TestSessionMeleeAttackEmitsLegacyCombatTranscript` 和 `TestGameWorldStaticMonsterVisibilityAndObjectIDs` 失败；旧 Goblin 夹具的 `AI=3` 被新 Tree 分支按固定 1 HP/无 DamageIndicator 处理，且生成朝向被固定为 Up。
- Root cause: 这些历史 Go 测试把 `AI=3` 当作未定义的普通怪物样例，而 Legacy `AI=3` 实际是 Tree；生产代码新增正确映射后，夹具的隐含语义暴露为冲突。
- Prevention: 新增 AI 常量或专用行为前，搜索所有测试和协议样例中的数值 AI；需要表达普通怪物时显式使用 `AI=0` 或对应已验证的普通 AI，Tree 行为测试只使用 Tree fixture。
- Verification: 将三个通用 Goblin 会话夹具和一个静态可见性夹具改为 `AI=0`；Tree 定向测试、旧失败测试及随后全包回归通过。

### 2026-08-20 — EvilCentipede map value 回写后的测试断言必须读取 world 状态

- Symptom: EvilCentipede AOE 测试中玩家和英雄已经受伤，但局部 `pet` 变量仍显示满血，误报宠物目标未被攻击。
- Root cause: 解析器从 `w.monsters` 取出 value copy，伤害与毒状态通过 map key 回写；测试在调用后继续读取创建时的局部副本。
- Prevention: 任何会修改 `worldMonster` 的解析或 poison 路径后，断言统一读取 `world.monsters[objectID]`；只有指针型 player/hero 才读取原变量。
- Verification: 修正夹具前先保留失败证据；修正后需重新运行 EvilCentipede 定向测试，并分别检查 player/pet/hero 最终血量与毒状态。

### 2026-08-20 — 认证会话夹具的账号名必须符合 15 字符协议边界

- Symptom: EvilCentipede 会话 bootstrap 在登录阶段收到 `ServerLogin` 结果码 1，而不是 `ServerLoginSuccess`。
- Root cause: 新测试账号 `evilcentipedenet` 超过 Go 认证层 `^[A-Za-z0-9]{3,15}$` 的最大长度，账号虽已注册仍被格式门禁拒绝。
- Prevention: 新增会话测试时先按认证正则核对账号、密码和角色名长度；失败时优先解析登录结果码，不把它归因到会话协议或 AI。
- Verification: 认证源码中的正则与返回码已核对；改用合法账号名后重新运行 bootstrap 与 EvilCentipede 会话定向测试。

### 2026-08-20 — 新增 Monster AI 后必须修正旧测试中的数值占位

- Symptom: AI=11 WoomaTaurus 接入后，完整 Go 测试中的 `TestGameWorldBasicMonsterAIIsOptInAndExcludesSpecialPopulations` 多出 `ObjectAttack`，但新增 AI 世界/会话测试本身均通过。
- Root cause: 旧测试把真实 Legacy AI=11 当作“尚未支持的特殊 AI”占位；接入该 AI 后，夹具开始正确进入 common population 并执行攻击。
- Prevention: 每次新增 Monster AI 常量后搜索 Go 测试中的同值 `AI` 字面量；普通样例显式用 AI=0，特殊样例改用已确认的专用常量/行为，不复用即将迁移的数值作为无语义占位。
- Verification: 将旧占位改为已迁移的 `treeMonsterAI` 静态特殊样例；WoomaTaurus 定向测试仍通过，随后完整 Go 测试需重新取得成功结果。

### 2026-08-20 — RedMoonEvil 多目标测试必须保留 Legacy 的同格与观察者通知语义

- Symptom: AI=13 定向测试初版错误地排除了与 RedMoonEvil 同格的玩家；多目标延迟伤害断言也只期待第一个目标的四个命中包，实际同一观察者还收到第二目标的 `ObjectStruck`/`DamageIndicator`。
- Root cause: `RedMoonEvil.ProcessTarget` 直接调用 `FindAllTargets(ViewRange, CurrentLocation)`，没有调用其 `InAttackRange` 覆盖，因此 `FindAllTargets` 的距离零格仍有效；每个延迟目标命中后又按 Legacy 广播范围向附近玩家发送可观察的伤害包。
- Prevention: 对群体 AI 分别验证“目标扫描入口”和“延迟结算的广播接收者”；不要从未调用的 `InAttackRange` 覆盖推导扫描排除规则，并在测试中区分目标自身包与附近观察者包。
- Verification: Go AI=13 世界测试改为包含同格目标并锁定目标顺序；延迟冲击断言补齐观察者收到的两个额外包，RedMoonEvil 世界、会话定向测试通过。

### 2026-08-20 — Monster AI 测试随机边界与 ObjectStruck payload 必须以真实调用路径为准

- Symptom: ZumaMonster 夹具把继承搜索/堆叠流程实际使用的 `Next(3)` 当成意外调用而失败；认证 transcript 又把 `ObjectStruck` 的方向误期望为攻击者方向，实际包使用目标当前方向。
- Root cause: 测试只按新增 AI 的局部攻击随机流配置回调，没有包含继承 `ProcessAI` 的搜索/移动分支；协议断言从 `ObjectAttack` 语义推测 `ObjectStruck` 字段，没有先读取真实 resolver 的 payload。
- Prevention: 新 AI 测试先列出完整 inherited/custom 调用序列和每个 bound，再配置固定随机回调；每个延迟伤害 transcript 分别核对 `ObjectAttack` 与 `ObjectStruck` 的实际编码来源，不复用攻击方向假设。
- Verification: AI=15 targeted world/session tests 在普通模式和上述 race 重复门禁中通过。

### 2026-08-20 — 修正 Monster 防御类型时必须同步 net.Pipe 随机边界与失败清理

- Symptom: AI=16 接入时将共享 Zuma 延迟伤害改为 Legacy 的 `MACAgility` 后，既有 AI=15 session fixture 仍只接受 bound=1；回调中的 `t.Fatalf` 使测试失败后清理阶段等待服务 goroutine，定向命令长时间无输出并最终被诊断终止。
- Root cause: 测试夹具沿用了旧 AC 路径的随机边界假设，没有随防御类型变更加入 `MagicResistWeight` bound=10；`net.Pipe` 测试在服务 goroutine 与 cleanup 之间使用 `FailNow`，放大了断言失败造成的阻塞。
- Prevention: 每次切换延迟攻击的防御类型，都重新列出 MAC/AC、敏捷和护甲的完整随机调用序列，先更新 fixture 的允许边界再跑认证 transcript；不要在仍可能阻塞的连接生命周期中触发未验证的 fatal callback。
- Verification: 已将 Zuma session callback 改为接受精确的 bound=10 与 bound=1；AI=15/16 世界和 session 定向测试均通过，新的 MAC 调用序列与 cleanup 路径已收口。
- Strengthening after world-fixture recurrence: owned Monster/Hero 回归随后仍因 `damageOmaWitchDoctor...` 的 MagicResist 使用 `monsterAIRollLocked(10)` 而失败；世界 fixture 只覆盖了初始化/搜索的 bounds，未覆盖新 MAC 解析的 AI 随机源。
- Verification after strengthening: 世界 Zuma fixture 已加入 bound=10；AI=15/16 定向测试已同时通过 Player 的 combatRoll 与 owned target 的 monsterAIRoll 两条随机源。
- Strengthening after hero-fixture recurrence: Hero 分支随后暴露 bound=16；这是 `heroEquipmentStats` 计算出的默认敏捷 `Next(Agility+1)`，并非新的 AI 分支随机。
- Verification after strengthening: 世界 fixture 的完整允许序列现包含初始化/搜索、MAC 抗性、Hero 敏捷和伤害范围边界；AI=15/16 世界定向测试已通过并保持这些来源分开断言。
- Symptom: AI=16 新测试首次编译失败，直接给 map 中 struct 字段 `boundaryWorld.monsters[id].MonsterAIAttackAt` 赋值。
- Root cause: Go map 元素不可寻址，测试为了模拟攻击冷却没有先复制、修改并写回 `worldMonster`。
- Prevention: 修改 map 中的值类型 struct 时始终使用局部副本并显式重新赋回；新增测试先跑编译门禁再看行为断言。
- Verification: 已将边界测试改为 copy-modify-store；AI=16 世界定向测试已编译并运行通过。
- Symptom: AI=16 行为测试首次运行时，远程攻击的“提前冲击”检查把所有通知误当作伤害包；同一组范围边界还触发了 MoveTo 备用方向选择的 bound=2。
- Root cause: Legacy RedThunderZuma 在攻击动作的 +300ms 后、投射命中前仍可进入移动分支，提前 tick 可能合法地产生 ObjectWalk；测试把通知为空错误地当成伤害未发生的必要条件，且 fixture 未列出继承移动 fallback 的随机边界。
- Prevention: 延迟攻击测试只用目标 HP/伤害包判断冲击是否提前，不排除合法的移动通知；范围/冷却测试必须把 `MoveTo` 的 direct-walk 失败 fallback bounds 一并列入随机序列。
- Verification: 已将提前检查改为只断言 HP=100，并允许 bound=2；AI=16 世界定向测试已确认攻击包、延迟伤害和移动时序符合 Legacy。
- Symptom: AI=16 范围边界测试初版把 9 格目标在 `AttackTime` 尚未到期时也期望为 `ObjectRangeAttack`，实际收到 `ObjectWalk`。
- Root cause: Legacy `RedThunderZuma.ProcessTarget` 只在 `InAttackRange() && CanAttack` 同时成立时攻击；“冷却中仍移动”的特殊性仅适用于超出 9 格的目标，不能反推为范围内忽略冷却。
- Prevention: 将攻击距离边界与攻击冷却边界拆成正交用例：9 格使用可攻击状态断言投射，10 格使用冷却状态断言移动。
- Verification: 已按两种状态改写测试；AI=16 世界定向测试已确认 9 格发 RangeAttack、10 格只移动且不排队伤害。
- Symptom: AI=16 认证 transcript 初版在启动后长时间无输出；诊断栈显示测试 goroutine 在持有 `world.mu` 时调用 `setLightClock`，服务 goroutine 也无法继续。
- Root cause: `setLightClock` 自身会获取 `world.mu`，而 session fixture 把它放进了已加锁的玩家/Monster 初始化区，形成同 goroutine 的非可重入锁死锁。
- Prevention: 认证夹具中凡是会获取 world mutex 的 helper（时钟、配置、连接状态）都必须放在显式 `world.mu.Lock` 外；锁内只做直接字段读写，完成后立即解锁再驱动 transcript。
- Verification: 已将 `setLightClock` 移到锁前；AI=16 session 定向测试已通过并正常执行 cleanup。

### 2026-08-20 — AI=17 认证 transcript 新增断言必须先通过未使用变量编译门禁

- Symptom: AI=17 session 测试首次编译因只用于进入一次循环、却没有读取的 `childID` 局部变量而失败。
- Root cause: 为检查野生子怪不发送 `ObjectHealth` 临时写了按子 ID 遍历的循环，但实际断言是对整批通知的聚合查询，留下了无意义的变量。
- Prevention: 新增 transcript 断言先确认是否需要逐对象上下文；批量通知断言直接在通知集合上完成，避免为了“覆盖”而引入未使用循环变量；补丁后立即运行定向编译/测试。
- Verification: 修正后 AI=17 世界与认证 transcript 定向测试通过，聚合断言仍确认整批通知没有 `ObjectHealth`。

### 2026-08-20 — AI=17 session 夹具角色名必须遵守认证服务长度约束

- Symptom: AI=17 认证测试在启动服务前因 `CreateCharacter` 返回结果 1 失败，没有进入协议 transcript。
- Root cause: 测试角色名 `ZumaTaurusTarget` 长度超过了当前认证服务允许的角色名上限。
- Prevention: 新增认证夹具沿用已通过的短角色名，并在服务启动前断言创建结果；不要把 Monster/AI 描述性长名直接复用为角色名。
- Verification: 将角色名缩短后 AI=17 session 定向测试通过，先完成认证 bootstrap，再按召唤/唤醒/攻击序列读取客户端包。

### 2026-08-20 — 全量门禁失败必须隔离既有 session 的随机流断言

- Symptom: AI=17 批次的首次全量 `go test ./... -count=1` 在既有 `TestSessionFurbolgCommanderRangedTranscript` 失败：launch roll bounds 收到 `[2 1]`，测试期待 `[1]`；AI=17 定向测试与相关 race 门禁均未失败。
- Root cause: 当前证据只表明 FurbolgCommander 认证夹具对 live session 期间的随机调用序列作了过窄的精确断言；该失败尚未证明由 AI=17 代码或子怪方向改动触发。
- Prevention: 全量失败时先按失败测试名隔离重跑并检查其 callback/ticker 生命周期，再决定是否属于当前批次；不要把与改动无关的随机 bound 失败直接记录为本批回归或扩大生产改动。
- Verification: 独立重跑 FurbolgCommander session 与 AI=17/相关组合门禁；只有在失败可稳定复现且调用栈指向本批改动时才修复生产代码，否则保留为既有门禁问题并在 handoff 明确记录。
- Strengthening after isolated recurrence: `stopPoisonSessionTicker` 在认证 bootstrap 后可能与服务启动 ticker 竞争；live tick 会在手工基准时刻前看到未来的 `MoveAt`，进入 `MoveTo` fallback 并消费 `bound=2`，再由手工攻击消费 `bound=1`。
- Prevention strengthening: 认证 transcript 若要断言精确 AI 随机序列，keep-alive barrier 后必须停止 ticker，并在锁内直接调用 `processMonsterAILocked`；用 `world.tick` 会重新混入实时 ticker 的非目标动作。
- Verification strengthening: FurbolgCommander session 改为直接 process 后 `-count=5` 通过；随后 `go test ./... -count=1 -timeout=600s`、`go vet ./...`、`go build ./...` 全部通过；该改动只收紧 Go 测试夹具，不改变生产 AI 行为。

### 2026-08-20 — AI=18 图像夹具必须先区分隐藏态与显形态

- Symptom: Shinsu 普通宠物图像测试期待初始 image 79，却因夹具已提前设置 `ShinsuMode=true` 收到 image 80。
- Root cause: 测试在验证基态对象包前复用了显形态状态，混淆了 `GetInfo` 的默认图像与 `Mode` 图像切换。
- Prevention: 状态投影测试按“默认 79 → Show 后 80 → Hide 后 79”顺序独立设置状态；创建夹具后先断言默认态，再进入显形态断言。
- Verification: 修正夹具后重跑 Shinsu 定向测试，默认宠物包与 Show/Hide 转录分别验证 79/80/79。

### 2026-08-20 — AI=19 随机攻击分支测试必须显式控制 1/5 判定

- Symptom: KingScorpion 普通攻击用例收到 `ObjectRangeAttack`，而测试期待 `ObjectAttack`。
- Root cause: 共享夹具把 `monsterAIRoll` 固定为 0；这正好命中 KingScorpion 的 `Random.Next(5) == 0` 范围攻击分支。
- Prevention: 对包含随机分支的 AI 测试按分支显式设置 roll callback；范围分支返回 0，普通分支对 bound=5 返回非零，其余伤害/命中 roll 保持确定值。
- Verification: 修正普通攻击夹具后重跑 KingScorpion 世界与 session 定向测试，确认两种攻击包、伤害类型和延迟均符合预期。

### 2026-08-20 — AI=20 落点测试必须分离发射与命中随机调用

- Symptom: DarkDevil 范围攻击测试在命中后看到随机 bound `[3 1]`，却只期待发射阶段的 `[3]`。
- Root cause: Legacy 的范围伤害在 500ms 延迟执行时重新调用 `GetAttackPower`；测试 callback 同时记录了发射时的冷却随机值和落点时的单位区间伤害随机值。
- Prevention: 对延迟攻击断言按阶段记录随机调用；发射断言 `[3]`，落点断言追加对应的伤害 bound，不要把完整调用序列误当成单阶段行为。
- Verification: 修正断言后 AI=20 DarkDevil 世界定向测试全部通过，确认范围包、延迟伤害和 MACAgility 落点行为。

- Strengthening after same-session recurrence: 本轮一次只读 `sed` 调用把已确认的 Go 根目录重复拼成 `Crystal.GoServer.GoServer`，命令未启动，输出不能作为证据。
- Strengthened prevention: 已确认绝对根目录后直接使用该路径作为 `workdir`；不要在后续 Go 文件读取中再次手工拼接仓库名，且失败路径调用必须立即作废。
- Verification after strengthening: 失败调用没有文件变化；随后恢复使用精确的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 读取 DarkDevil 源码。

### 2026-08-20 — AI=21 测试夹具必须先通过编译再判断生产语义

- Symptom: IncarnatedGhoul 首轮定向测试未进入运行阶段，Go 编译器报告测试夹具中的 `pet` 与 `attack` 局部变量未使用。
- Root cause: 新增多目标类型测试时先搭了分支变量与延迟攻击调用，却没有在首次执行前清理未使用变量或断言返回值。
- Prevention: 新增测试文件后先运行目标包编译/定向测试；多分支夹具只保留实际用于断言的变量，调用返回通知必须检查长度或内容，避免把编译失败误判为生产实现失败。
- Verification: 修正夹具变量后将重跑 AI=21 定向测试；本次编译失败没有产生 Go 源码或测试运行结果，后续只采用修正后的测试输出。

### 2026-08-20 — AI=21 多目标测试必须区分内部目标编码与广播通知范围

- Symptom: 首次运行 IncarnatedGhoul 世界测试时，基础夹具没有设置 `MonsterAITargetID`；修正后又把广播通知总数当成单个玩家应收到的包数，并把 Player 的 action `TargetKind=0` 误判为错误。
- Root cause: 测试同时覆盖了内部延迟动作投影和 `Broadcast` 可见范围，却沿用了单目标/非零目标 kind 的断言假设。
- Prevention: 夹具初始化必须显式设置目标 ID/kind；协议断言按具体 recipient 过滤，内部 action 对 Player 使用 legacy 的 `TargetKind=0` 映射，Monster/Hero 才保留专用 kind。
- Verification: 修正后 IncarnatedGhoul Player、Pet、Hero、延迟重验证和 Cell 顺序定向测试全部通过；失败输出未被当作生产语义证据。

### 2026-08-20 — AI=22 MACAgility 测试必须隔离防御抽样与受击副作用

- Symptom: IncarnatedZT 命中测试实际记录的 `combatRoll` 为 `[10 1 1 1 ...]`，而测试只期待 MACAgility 的 `[10 1 1]`，导致在生产逻辑已完成伤害后失败。
- Root cause: `Attacked` 后续的装备耐久/反击等通用受击副作用也会消费单位边界随机数；测试把完整 combat callback 序列误当成防御三步序列。
- Prevention: 断言 MACAgility 时只验证序列前缀（魔抗、Agility、MAC armour），其余副作用随机调用单独验证或不纳入该投影断言；AI/毒 callback 仍按阶段独立记录。
- Verification: 修正为核对 `[10,1,1]` 前缀；重新运行 AI=22 定向测试将确认伤害、12 分之一麻痹和延迟重验证。

### 2026-08-20 — 会话测试账号必须遵守登录协议长度边界

- Symptom: AI=25 会话测试首次使用较长账号名时，登录返回 `ServerLogin`（ID 7）而非 `ServerLoginSuccess`，测试在 bootstrap 前失败。
- Root cause: 账号名超过了当前测试登录协议/服务校验允许的长度；字符创建成功并不代表后续登录凭据会被接受。
- Prevention: 新增会话测试沿用已通过的短账号名模式，并在 `startGameBootstrapForTest` 前确认账号长度处于现有成功样例范围。
- Verification: 将账号改为 `reviveznet` 后重新运行 AI=25 定向测试，单元、会话和继承基础 AI 测试均通过。

### 2026-08-20 — AI=26 定向测试必须区分搜索范围与攻击几何

- Symptom: ShamanZombie 定向测试首次把距离七格的搜索结果断言为无通知，实际 AI 已正确锁定目标并发出移动包，测试失败。
- Root cause: Legacy 的目标搜索范围大于 ShamanZombie 的轴线/对角线六格攻击范围；测试把“找到目标并移动”与“进入攻击范围”混成了一个状态。
- Prevention: AI 测试分别断言搜索锁定、移动通知、六格攻击排队和延迟命中重验证；不要用攻击几何推断搜索通知为空。
- Verification: 修正测试后，ShamanZombie 定向测试通过，并验证目标进入安全区后六百毫秒延迟伤害被取消。

### 2026-08-20 — AI=27 夹具必须以协议方向和实体初始状态为准

- Symptom: Khazard 首轮定向测试把 `Direction` 断言为 0，且复用了满血 500 的宠物夹具，分别导致攻击 payload 不匹配和宠物 HP 变为 490 而非 90。
- Root cause: 测试夹具凭直觉假定方向编号，并未把被投影目标的独立初始 HP 归一化到测试期望。
- Prevention: 新增 AI 测试先从现有协议构造函数和目标类型夹具确认方向/字段编码；Player、owned Monster、Hero 的初始 HP 与 `Character`/`Hero` 镜像字段显式设置后再断言伤害。
- Verification: 修正为协议实际方向值并将宠物 HP/MaxHP 设为 100 后，Khazard 定向单元、投影和延迟伤害测试全部通过。

### 2026-08-20 — 会话 transcript 的 AI 随机边界必须覆盖实时维护 tick

- Symptom: 全量 Go 测试中 Khazard 会话偶发收到 AI roll bound `2`，严格只允许 PullAttack 的 `10` 使测试失败；单独运行时未必复现。
- Root cause: 认证会话仍有实时维护 tick；合成时钟把 Action/Move 推到未来时，Khazard 的 fallback MoveTo 仍会执行 `Next(2)` 分支选择，和手动合成攻击 tick 交错。
- Prevention: 会话测试的随机回调同时允许被实时维护路径实际触达的 `2` 与攻击路径的 `MagicResistWeight`，不要把“手动 transcript 期望”误当成整个连接生命周期的唯一随机调用集合；共享回调不追加无锁状态。
- Verification: 放宽为仅接受 `2`/`MagicResistWeight` 后，Khazard 会话测试重复 20 次通过，且不再依赖未同步的 roll 记录。

### 2026-08-20 — 延迟 AI 测试必须冻结夹具实体的独立行为

- Symptom: ToxicGhoul 死亡爆发的提前时间断言收到 `ObjectAttack`、`ObjectStruck` 和 `DamageIndicator`，看起来像延迟动作提前执行。
- Root cause: 死亡爆发夹具中的 Hero 没有把 `ActionReadyAt` 推到合成时钟之后；`world.tick` 的 Hero 维护路径在检查 ToxicGhoul action 前独立攻击了中心玩家。
- Prevention: 任何包含 Player、owned Monster、Hero 的合成 AI 时间线，都要把非被测实体的 AI/Action/Move/Attack 时钟显式冻结到未来，并用包 ID/实体 ID 区分“被测动作”和背景维护行为。
- Verification: 将 Hero `ActionReadyAt` 设为 `base + 1h` 后，AI=28 纯世界测试（攻击、投射、魔抗、延迟重验证、Effect=0、死亡爆发）通过。

### 2026-08-20 — BoneSpearman 新增测试必须显式归一化目标 HP 与几何

- Symptom: AI=29 首轮目标投影测试把 materialize 后 owned Monster 的 30 HP 当作 100 HP 断言；搜索测试还把有意冻结攻击/移动时钟的 tick 直接断言为零通知；另有一条 parity 几何期望把相对位移 `(2,1)` 误判为可攻击。
- Root cause: 测试夹具没有像目标投影实际状态一样显式设置 Monster 的 HP/MaxHP，且没有先按通知 ID 区分后台维护包与被测 AI；同格 Hero 的 `tickHeroesLocked` 还会在 `world.tick` 中独立产生攻击包；几何断言按直觉而不是 Legacy `x == y || x % 2 == y % 2` 计算。
- Prevention: Player、owned Monster、Hero 夹具在断言伤害前分别显式设置自身 HP/MaxHP 及镜像字段；包含 Hero 的合成时钟测试把 `ActionReadyAt`/`OperateReadyAt` 推到未来，并先用 `packetIDsForRecipient`/目标 ID 过滤通知，再判断是否应为空；逐条把相对坐标代入 Legacy parity 条件并覆盖奇偶边界。
- Verification: 修正夹具、通知断言和 parity 用例后，AI=29 定向世界测试重新运行通过，且已有 BoneSpearman 玩家目标测试保持通过。

### 2026-08-20 — 新增 MonsterSettings 字段必须同步更新结构体默认值断言

- Symptom: Go `internal/worlddata` 的 `TestMonsterSettingsDefaultsAndJSONCompatibility` 在新增 BoneMonster1–4 后失败，旧的显式结构体期望缺少四个非零默认字段。
- Root cause: 配置投影扩展时只更新了生产结构体和默认值，没有同步检查按完整结构体相等比较的兼容性测试。
- Prevention: 每次扩展持久化 settings 结构体后，搜索所有显式 `Default...Settings()` 结构体字面量和 JSON 兼容测试，补默认值与至少一个自定义字段的加载断言。
- Verification: 更新测试后重新运行 `go test ./internal/worlddata ./internal/legacyworld`，并通过全量 Go 测试中的 settings 断言。

### 2026-08-20 — AI=100 VenomSpider 测试必须区分同数值随机边界

- Symptom: 新增 VenomSpider 测试首次编译失败：SC 统计常量误用了不存在的 `monsterStatMinSC`/`monsterStatMaxSC`，且 `PoisonResistWeight` 与 `MagicResistWeight` 都是 10，不能在同一个 `switch` 中写重复 case。
- Root cause: 沿用了 Monster DC/HP 常量命名而未核对 SC 的协议统计常量；测试又把“随机边界数值”误当作“随机语义标识”，忽略了 VenomSpider Monster 目标的魔抗检查与一次 poison resistance 检查共享数值边界。
- Prevention: 新增夹具先用 `rg` 核对统计常量；随机回调测试按调用上下文/总次数断言，允许同值边界被多个 Legacy 阶段消费，不用重复数值 case 区分语义。
- Verification: 修正后 `go test ./cmd/crystal-server -run 'VenomSpider' -count=1 -timeout=180s`、`-race` 定向测试及完整 Go 测试均通过。

### 2026-08-20 — HedgeKekTal session 夹具必须区分 value map 与冷却期移动

- Symptom: 新增 HedgeKekTal transcript 首次把 `world.monsters` 的 value map 元素按指针解引用而编译失败；修正后又把延迟命中前的 `ObjectWalk` 误断言为空，定向测试失败。
- Root cause: Go 世界的 Monster map 存储 `worldMonster` 值而不是指针；Legacy `RightGuard.ProcessTarget` 在攻击冷却期仍会执行 `MoveTo`，因此投射发出后、伤害到达前可能先产生移动包。
- Prevention: 读取/写回 Monster map 时使用值副本；为带延迟动作的 RightGuard transcript 将“投射 tick、冷却移动 tick、命中 tick”分开断言，并按通知顺序核对移动包的位置和方向。
- Verification: 修正后 `go test ./cmd/crystal-server -run 'TestGameWorldHedgeKekTal|TestSessionHedgeKekTalRangeAttackTranscript' -count=1` 通过。

### 2026-08-20 — CrazyManworm 三类目标夹具必须隔离 Hero 维护与随机流

- Symptom: AI=66 三类目标测试首次在 Hero case 收到额外 `ObjectAttack/ObjectStruck/DamageIndicator`，随后随机回调又因 Hero 维护路径消费了 bound=16 而失败。
- Root cause: `world.tick` 会独立运行 Hero 的搜索/攻击维护；夹具只冻结了 Monster 时钟，没有冻结 Hero 的 `ActionReadyAt`/`OperateReadyAt`，并把共享 AI 随机回调误当成只服务 CrazyManworm 的 bound=3。
- Prevention: 合成 Player/owned-Monster/Hero 目标测试先把非被测 Hero 的 readiness 推到合成时钟之后；随机回调只对被测分支的 bound 做严格断言，允许并记录独立维护路径的合法边界。
- Verification: 修正夹具后 `go test ./cmd/crystal-server -run 'CrazyManworm' -count=1` 通过。

### 2026-08-20 — MutatedManworm inclusive damage draw 需要同步继承 AI 夹具

- Symptom: AI=65/66 组合回归首次在既有 `TestGameWorldMandrillTeleportsWithEffectSevenAfterHeavyHit` 失败，Mandrill 的随机回调收到合法 `Next(1)`，但夹具只接受 `Next(2)`。
- Root cause: MutatedManworm 受击反应改为 Legacy `GetAttackPower` 的 inclusive/fixed-range 语义后，Mandrill 继承同一反应路径并额外消费固定 DC 的 `Next(1)`；旧测试只覆盖了 `Next(2)` 的 50% gate。
- Prevention: 修改共享 Monster AI 的随机消费时，检索所有继承/复用该 helper 的 AI 测试，按调用语义允许固定范围 `Next(1)` 与分支 gate 的合法边界，不用单一测试名称推断完整随机流。
- Verification: Mandrill 夹具改为接受 `1`/`2` 后，AI=65/66/Mandrill 定向回归通过。

### 2026-08-20 — AI=34 会话夹具必须冻结 FrostTiger 的专有计时器

- Symptom: FrostTiger 延迟命中测试首次在命中时收到合法的 `ObjectSitDown` 广播，而不是只验证延迟命中结果。
- Root cause: 测试把 AI 随机值固定为 0，却没有把 Legacy `Random.Next(120000)` 生成的坐下时间推到未来；两分钟边界在后续 tick 到期，坐下行为先于延迟命中执行。
- Prevention: 攻击、延迟命中和中毒 admission 测试显式把 FrostTiger 的坐下/漫游计时器设到测试时钟之后；保留单独的坐下/唤醒测试覆盖到期语义。
- Verification: `go test ./cmd/crystal-server -run 'FrostTiger' -count=1 -timeout=180s`、全量普通测试和 FrostTiger 定向 race 均通过。

### 2026-08-20 — Yimoogi 随机夹具必须覆盖固定范围的单位边界与金额语义

- Symptom: Yimoogi 定向测试首次漏报毒效随机顺序，因为断言没有计入固定 DC/SC 范围各自的 `Next(1)`；成对掉落断言使用 `Gold=1` 时，现有随机金额规则合法地产生 0，导致“最终姐妹死亡应有掉落”的断言失败。
- Root cause: 测试夹具只按业务分支枚举随机调用，未按 Legacy `GetAttackPower` 的 inclusive range 和 Go 当前 `dropMonsterGoldLocked` 的 `[Gold/2, Gold+Gold/2)` 语义核对固定边界。
- Prevention: 为每个新增 AI 先记录完整随机调用序列（包括 `Next(1)`），并使用能产生正向观测结果的最小掉落金额；不要把“固定范围不消耗随机”或“Gold=1 必为 1”当作默认假设。
- Verification: 修正为断言 `[1,6,1,10,1,10]` 的毒效顺序并将掉落夹具改为 `Gold=2`；`go test ./cmd/crystal-server -run 'Yimoogi' -count=1`、`go test ./...` 均通过。

### 2026-08-20 — 构造状态与攻击后状态必须分开断言

- Symptom: HolyDeva 回归测试把构造器固定的 DownLeft 方向 5 断言到了攻击后的对象包，首次定向测试失败；攻击实现按 Legacy 先将方向转向当前目标。
- Root cause: 测试只关注了对象投影字段，没有区分“Spawned/首次 GetInfo”与“Attack 后 GetInfo”两个可观察时点。
- Prevention: 对有构造器状态和攻击时变状态的 AI，分别在首次投影与动作结算后采样并断言；不要把初始方向、计时器或 Summoned 标记的断言复用于攻击后的快照。
- Verification: 测试改为初始包断言图片 117/方向 5/Extra，攻击后断言方向为 `DirectionFromPoint`；`go test ./cmd/crystal-server -run 'CrystalSpider|HolyDeva' -count=1` 通过。

### 2026-08-20 — 会话测试的人工时钟必须隔离服务循环 tick

- Symptom: `TestSessionOmaMageRangeSlowFrozenTranscript` 在本批整套测试及单独运行时稳定得到攻击随机边界 `[2 1]`，而不是只得到 `[1]`。
- Root cause: 会话服务循环在真实 `time.Now()` 下先调用 `world.tick`；测试把 OmaMage 的动作时间设在未来人工时钟附近，OmaMage 即使暂时不能攻击仍会按 Legacy 覆盖逻辑进入 `MoveTo`，消费了 `monsterAIRollLocked(2)`；随后人工 `world.tick(base)` 才消费固定 MC 的 `Next(1)`。调用栈落在 `serveWithConfig -> processOmaMageLocked -> monsterAIMoveToLocked`，与 AI37/38 生产路径无关。
- Prevention: 会话 transcript 在安装随机回调后，必须阻止实时服务循环推进 AI，或只断言由人工时钟 tick 产生的调用；设置未来人工时钟不能代替 ticker 隔离。出现额外随机边界时先用调用栈定位，禁止为无关 AI 修改生产实现。
- Verification: 通过 `go test ./cmd/crystal-server -run '^TestSessionOmaMageRangeSlowFrozenTranscript$' -count=3` 稳定复现并定位；本批未修改 OmaMage 代码，AI37/38 定向测试仍通过。
- Further evidence: 2026-08-20 的全仓 `go test ./... -count=1 -timeout=900s` 再次只命中该用例，仍为 `[2 1]` 对 `[1]`；AI37/38 定向普通/race、vet 和 build 继续通过。
- Prevention strengthening: 批次收尾必须保留一次全仓普通结果，并将同一随机边界的既有会话维护前导与新 AI 的失败分开记录；不能因为全仓门禁非绿就扩大本批生产代码范围。
- Verification after strengthening: 本批全仓普通失败与先前调用栈完全一致，未进入 CrystalSpider/HolyDeva 文件；目标定向门禁仍为绿色。

### 2026-08-20 — AI=41/42 YinDevilNode 测试必须排除自身并保留名称颜色副作用

- Symptom: YinDevilNode 初版友方扫描把节点自身当成“不可攻击友方”，无其他对象时也排入 `ObjectAttack`；认证测试为模拟延迟命中设置 `RageTime` 后，又漏读了同一 tick 的 `ObjectColourChanged`。
- Root cause: Legacy `FindFriendsNearby` 明确跳过 `ob == this`，而 Rage/Hallucination 计时变化会先由现有视觉刷新路径广播红色名称颜色包，再进入延迟 Buff 结算；测试只按 AI action 预期设计，没有包含可观察视觉副作用。
- Prevention: 迁移环形对象扫描时显式排除 attacker ObjectID；会话夹具改变 Shock/Rage/Hallucination 等名称颜色来源时，先列出视觉包顺序，再断言延迟动作/状态包，不把“不可见 Buff”误当成“无通知”。
- Verification: 新增 AI=41/42 自身排除世界测试，session transcript 锁定 `ObjectAttack -> ObjectColourChanged` 与五秒目标 Buff；定向普通/race、全仓普通、vet/build 通过。

### 2026-08-20 — AI=43 OmaKing 随机顺序与延迟 AOE 必须按 Legacy override 重建

- Symptom: OmaKing 初版实现遗漏 `PoisonTarget` 内层 `SC -> PoisonResist(10) -> Next(1)`；近身测试还先把地图边界目标放在可继续 push 的位置，导致 line action 缺失；认证 transcript 预期为空，却收到 cooldown 期间的 `ObjectWalk`。
- Root cause: 只迁移了外层 `Next(8)` 分支，没有展开被调用 helper 的随机流；测试几何没有同时约束 push 终点与 LineAttack 首格；Legacy `ProcessTarget` 在 300ms action 解锁而 Attack cooldown 未到时仍会 `MoveTo`。
- Prevention: 新 AI 把嵌套 helper 的随机调用按调用栈完整列出；push/line fixture 使用实际地图边界；每个延迟 transcript 单独推进 impact 后的下一次 AI tick，并断言合法移动包。OmaKing `CompleteAttack` 必须按当前节点位置重新扫描七格 AOE，不能只命中 admission 目标。
- Verification: 修正后 OmaKing close/push/paralysis、ranged AOE rescan 和 authenticated transcript 连续通过，包级编译与全仓普通测试保持通过。

### 2026-08-20 — AI=43 初次编译必须先统一随机返回类型

- Symptom: OmaKing level-gap admission 首次包级编译把 `int` 的随机返回值直接与 `int32` level gap 比较，Go 编译在行为测试前失败。
- Root cause: 新增 helper 时没有沿现有 `monsterAIRollLocked` 签名核对返回类型。
- Prevention: 新 AI patch 后立即运行 `gofmt` 和 `go test ./cmd/crystal-server -run '^$' -count=1`；跨统计类型比较显式转换，不把编译失败当作行为证据。
- Verification: 显式转换后最小编译、AI=43 世界/会话测试通过。

### 2026-08-20 — AI=48 GuardianRock 测试必须读取 value-map 权威状态与 push 反向方向

- Symptom: GuardianRock 世界测试首次从旧 attacker 副本读取攻击计时，且会话 push 方向把移动方向 6 当成目标最终 `Pushed` 方向；实际 Go value-map 已写回新计时，`Pushed` payload 使用反向方向 2。
- Root cause: resolver 按值接收并写回 Monster，测试没有回读 `world.monsters[id]`；`pushPlayerLocked` 沿移动方向推进后把对象方向设为 reverse，wire 方向不是 push 向量本身。
- Prevention: 所有延迟 resolver 后的 Monster 状态从权威 map 回读；push transcript 分别计算移动向量和对象最终反向方向，不手写 compass ordinal。
- Verification: GuardianRock 世界/认证测试连续 3 次通过，覆盖成功 push、MagicResist 阻断、计时器和完整包序。

### 2026-08-20 — AI=47 TrapRock 子岩石测试必须推进 nextObjectID 并防御 parent 自引用

- Symptom: TrapRock 显形测试手工插入 parent 后没有推进 `nextObjectID`，第一个 child 复用 parent ObjectID；父死亡清理递归进入自身，触发 stack overflow。
- Root cause: Go 的 value-map fixture 不会自动更新全局 ID allocator；生产清理又按 child ID 递归，缺少坏数据下的 parent-ID 防护。
- Prevention: 手工插入实体后显式推进 `nextObjectID`；父/子清理遍历始终跳过自身 ID，并从 authoritative map 回读 child 状态。
- Verification: 修正 fixture 与生产防护后 TrapRock 隐藏/显形/三子岩石/移动死亡测试通过，未再出现递归栈增长。


### 2026-08-22 — AI=59 HumanAssassin 夹具修改必须先写回 map value

- Symptom: HumanAssassin movement/attack fixture 已把 clone 移到目标邻格，但 production-entry tick 仍按旧坐标执行，导致预期 `ObjectAttack` 变成 `ObjectWalk`，延迟 action 未入队。
- Root cause: 测试从 `map[uint32]worldMonster` 复制 value 后修改局部变量，随后 helper 又从 map 重新读取并覆盖了坐标；不是 AI 的距离或攻击门禁错误。
- Prevention: 任何 value-map 实体修改后，在调用会重新读取 map 的 helper 前立即写回；测试 failure 输出同时保留权威 map 坐标、目标 ID、动作队列和 transcript。
- Verification: AI=59 movement、cumulative threshold、delayed impact/invalidation、explosion insertion/plain-AC、logout tests 通过；定向普通测试 `-count=10` 与 race `-count=3` 通过。

### 2026-08-22 — ArcherSummon 子蛇必须独立检查 AliveTime 并保留死亡爆炸的 Legacy race 边界

- Symptom: ArcherSummon 初版 Go 只在父 SnakeTotem 失效时清理 CharmedSnake，子蛇自己的 `AliveTime` 到期不会死亡；同时 `CharmedSnake.Die()` 的 1-cell `10 * PetLevel` `MACAgility` 爆炸未接入，且误把 Hero 纳入了 Vampire/子蛇死亡爆炸。
- Root cause: 将 totem/child 生命周期合并为一个 owner/parent gate，没有沿 `CharmedSnake.Process()` 的独立 `AliveTime` 分支和 `Die()` 的 `ObjectType.Monster/Player` switch 逐项迁移。
- Prevention: 每个召唤层级分别核对 owner/parent 距离、独立存活截止、召回和死亡副作用；死亡爆炸按 Legacy race 白名单建模，不从通用 Player/Hero target projection 推断范围。
- Verification: Go `TestCharmedSnakeExpiresIndependentlyAndExplodesOnlyAtLegacyTargetRaces` 锁定子蛇严格 `now > AliveTime` 清理、父 child-list 回写和 Player 命中；ArcherSummon 普通 `-count=1` 与 race `-count=3` 定向测试通过。

### 2026-08-24 — NET-P1-HTTP shutdown tests must separate callback completion from socket closure

- Symptom: the first blocked-GET shutdown test required the response body even though transport shutdown legitimately returned EOF; the POST test accepted any read error, so a still-open socket timing out could pass. A later non-GET fixture hand-built a wildcard prefix without its port and became a false 404 after port authority was enforced.
- Root cause: handler-lifecycle completion and client wire completion were treated as one signal, deadline errors were not distinguished from close/EOF errors, and the fixture bypassed the production prefix parser.
- Prevention: use an explicit callback-finished barrier for handler ownership; test the connection separately, accepting close/EOF/reset but rejecting deadline timeout after service Close; construct URI-authority fixtures through `parseLegacyHTTPPrefix` rather than partial structs.
- Verification: the active-handler, repeated POST shutdown, and all-non-GET tests pass after the assertions were split and the wildcard fixture used the real parser.

### 2026-08-24 — OPS-P1-LIFECYCLE shutdown fixtures must consume service preambles before EOF

- Symptom: the first integrated Stop test reported that the status connection remained open because its post-Stop read returned the unread initial status payload; a package-wide run separately hit the established class of asynchronous transcript noise when `TestSessionHallucinationTranscript` timed out after a closed pipe.
- Root cause: the lifecycle fixture treated the first successful read as proof of a live post-shutdown transport instead of consuming the service preamble before the shutdown boundary; the broad-run failure stack did not enter lifecycle production or test code.
- Prevention: establish a per-service readiness/preamble barrier before Stop, then require EOF/reset after Stop; attribute broad failures by exact test and stack, and isolate them before changing the active leaf.
- Verification: the corrected integrated HTTP→game→status closure test passes repeatedly and under race; the exact Hallucination test passed independently at `-count=5`.
- Strengthening during `RANK-P3-CHAR-LIFECYCLE-001`: fresh unexcluded `go test ./... -count=1 -timeout=20m` again had exactly `TestSessionHallucinationTranscript` fail after 30 seconds with a closed-pipe stack outside every owned rank file; the failed full run is not a pass. Exact isolation `go test ./cmd/crystal-server -run '^TestSessionHallucinationTranscript$' -count=10 -timeout=10m` exited 0, and the next fresh unexcluded full rerun exited 0, so no Hallucination-scope change was made.
- Strengthening during `BOOT-P4-STARTPOINT-001`: fresh unexcluded `go test ./cmd/crystal-server -count=1` again had only the same 30-second closed-pipe Hallucination failure; its stack did not enter `main.go` or either owned startup test. Exact isolation at `-count=10 -timeout=10m` exited 0, then the fresh unexcluded package rerun exited 0 in 75.352s. The first run remains recorded as exit 1 and no Hallucination-scope change was made.
- Strengthening during `MONSTER-P5-SPECIALIZED-CONSTRUCTOR-002`: the first fresh full test after terminal constructor corrections again had only this 30-second closed-pipe failure at `hallucination_session_test.go:123`, outside every owned constructor path. Exact isolation at `-count=10` exited 0 and the next fresh unexcluded `go test -count=1 ./...` exited 0, so the failed run remains exit 1 and Hallucination scope stayed unchanged.

### 2026-08-24 — Production source quirks and injectable seam cleanup must be tested separately

- Symptom: after production status-bind behavior was corrected to retain Legacy's already-bound game listener, the older injected-listener test failed because it intentionally expected its caller-owned fixture to close.
- Root cause: one helper had been used both as a production-fidelity authority and as a leak-free unit-test seam, although those ownership contracts differ.
- Prevention: keep production factory tests authoritative for occupied-port/leaked-state behavior; preserve explicit cleanup only for the pre-opened test seam and label that extension in production code.
- Verification: the legacy helper test passes again, while new production-entry and lifecycle tests independently lock game-first binding, status failure, stale Running, duplicate-Start no-op, and the leaked game listener.

### 2026-08-24 — Fatal reason 3/0 delivery needs separate callback and wire barriers

- Symptom: synchronously writing reason 3 before closing the listener blocked the fatal callback and made the max-user wake test receive a nil terminal error; releasing the callback before reason 0 completed then intermittently let session cleanup close the sender first.
- Root cause: Legacy enqueue order was projected onto direct Go socket writes without separating “reason 3 was scheduled before listener close”, “the fatal producer may return”, and “both frames reached each accepted client”.
- Prevention: snapshot accepted clients in stable order, start the reason-3 writer before listener close, signal the accept owner to request reason 0, release the producer only after that handoff, and keep the session callback alive until the ordered wire writer completes.
- Verification: max-user fatal wake and utility-format fatal transcripts both pass at `-count=20`; focused lifecycle/gate tests pass under race with reason 3 then 0.

### 2026-08-24 — Cross-platform process signals must use the platform's os.Signal value

- Symptom: a raw `syscall.Signal(1)` SIGHUP representation compiled on Unix/Windows but failed on Plan 9 because that target exposes `syscall.Note` instead of `syscall.Signal`.
- Root cause: numeric signal identity was coupled to one syscall concrete type even though the standard package already exposes target-specific `SIGHUP` values implementing `os.Signal`.
- Prevention: store `syscall.SIGHUP` behind the `os.Signal` interface and compare interface values; cross-build the entire module for every claimed target without creating binaries in the worktree.
- Verification: full-module builds pass for Plan 9, Windows, and Linux after the type change.

### 2026-08-24 — Fatal wake and writer startup must follow the accept-owner handoff

- Symptom: the first normal `-count=20` run and a later full-race run each returned a nil listener error once from the new reason-3/listener ordering regression, although the wire-order events completed.
- Root cause: `gates.stopAccepting()` could wake the MaxUser accept owner before the fatal descriptor was published; its nonblocking capture then saw an empty channel and attributed the shutdown as ordinary. Merely publishing before writer startup did not close this earlier wake race.
- Prevention: publish the fatal descriptor first, then wake the accept owner, then start any writer that can close the listener. Both wake and close must follow the attribution handoff.
- Verification: after enforcing publish→wake→writer order, the ordering regression passes under race at `-count=50`, and the combined ordering/max-user/utility-format fatal set passes under race at `-count=10`.

### 2026-08-24 — Concurrent Stop must not overtake fatal wire completion

- Symptom: final review found that context cancellation could close the listener before reason 3, a two-second callback timeout could release the reporting session before the two five-second wire phases, late accepted clients could miss normal reason 0, and HTTP/session joins exceeded Legacy's bounded service windows.
- Root cause: fatal attribution, listener closure, producer lifetime, accepted-client snapshots, and service joins were bounded independently rather than as one ordered shutdown transaction.
- Prevention: register and unconditionally publish the once-only fatal descriptor before wake; coordinate context/listener close through a reason-3 completion channel; keep the producer alive for the full bounded reason-3/0 window; take a fresh accepted-client snapshot for reason 0; cap HTTP waiting once at one second and session joining at five seconds. Preserve the separate unbounded top-level Stop join because Legacy explicitly spins until `_thread` clears.
- Verification: the strengthened regression cancels context while reason 3 is blocked beyond the former two-second timeout and locks reason3-done→listener-close→reason0; it passes under race at `-count=5`. Final independent read-only review found no remaining P1/P2 issue, and fresh focused, full unexcluded normal/race, vet/build, and Plan 9/Windows/Linux builds pass.

### 2026-08-25 — Reincarnation denial must separate runtime, immediate persistence, and logout normalization

- Symptom: the strengthened authenticated `NoReincarnation` test first expected logout to restore the pre-cast direction, then compared a target's nil equipment slice with the logout-normalized all-nil grid; the focused test failed both assumptions in sequence.
- Root cause: the fixture conflated runtime direction, immediate auth projection and final durable state, and treated nil/allocated empty-grid representation as item semantics. Exact Legacy tracing then showed `HumanObject.Direction` directly proxies persisted `CharacterInfo.Direction`, exposing a real Go snapshot defect rather than validating the stale overwrite.
- Prevention: derive durable fields from the Legacy authority separately from intermediate Go projection; synchronize runtime direction into the authoritative character snapshot; compare inventory/equipment by deep identity when populated and by semantic nonnil-item count when nil/empty normalization is permitted.
- Verification: `world.go` now preserves the requested spell direction through logout, the denial test locks immediate and final state separately, focused count-20, focused race count-5, fresh full tests, vet/build, and final independent re-review all pass.

### 2026-08-25 — VIS 生命周期必须同步 runtime visibility、连接 cache 与旧夹具默认值

- Symptom: 首轮 NPC day/time 实现让直接构造的 `worldNPC` 因新增 `Visible=false` 零值丢失 Observer bootstrap；全包又暴露原有远距 join fixture 仍期待整图 `ObjectPlayer`。独立 review 进一步发现 ticker 发包后异步更新 session-local NPC cache 会留下重复 remove/add/缺失定义窗口，transition synthetic player 丢失装备/光照/毒/公会运行时投影，旧 JSON 还漏掉 Legacy `Rate=100` 默认值。
- Root cause: 把 Legacy 对象状态、Go 测试零值兼容和连接可见 cache 当成三个独立问题；只修 packet viewer 参数，没有携带完整 live player snapshot；schema 默认清单也未机械逐字段对照声明初始化器。
- Prevention: 新增 runtime 可见位时显式区分“未初始化夹具”与“已隐藏”；动态 hide/show 在发送前与所有 refresh 共用同一 cache mutex；transition 携带完整 live `worldPlayer` snapshot，再用最新 Character/坐标覆盖；JSON defaults 逐项从 Legacy 字段声明核对。join/leave 测试必须把 16 与 17 格边界分开，不能保留整图广播假设。
- Verification: binary v63-v117/JSON defaults、minute strict-boundary/day-only quirk、filter/blocking/Observer/cache、range-16 join/leave、full appearance transition/revive tests 通过 count-20 与 focused race count-5；fresh full tests/vet/build 退出 0。首个 full run 的 Mirroring 随机 transcript 失败隔离 count-20 通过，fresh full rerun通过。
- Review recurrence: `codex-auto-review` 终审发现 schedule 仍吃 host location、CallNPC 未执行 `CheckVisible`、logout 清 cache 无锁、queued tick 与 refresh 可重复发包、map iteration 顺序不稳、`uint16` Level 被截断。修复采用共享 UTC monotonic clock、普通 NPC call 的同一 runtime/player predicate、generation+mutex teardown、cache-aware send suppression、Legacy load/insertion 顺序和 int32 比较。
- Failed-gate ruling: 首次补测猜测不存在的 `ParseObjectRemovePayload` 且把 bootstrap struct 当 slice，compile exit 1；首次 fresh full 又暴露旧 world fixture 依赖已接受的 NPC load sort，以及 conquest test 让战争中已隐藏 NPC 自己停止战争、违反 Legacy `CallNPC -> CheckVisible -> VisibleLog/Visible`。恢复 load sort，并把征服测试拆为可见 controller 与被隐藏 target，未削弱生产 gate。
- Final verification: 后续复审继续修正 bootstrap callback 安装窗口、Observer/target-loss generation 失效、持锁网络写和逐包 reservation 交错；最终以同锁 cache claim + 单一 bounded FIFO NPC batch 落盘。parser/world/session focused `-count=20`、focused race `-count=5`、征服与旧 world 回归 race、fresh unexcluded `go test ./... -count=1`（server 79.853s）、`go vet ./...`、`go build ./...` 均 exit 0，终审 `resolved, no findings`。

### 2026-08-26 — Legacy formatter arguments must cross a supported Go type boundary

- Symptom: the first authenticated `@MAKE ... 0` transcript closed the server pipe before its KeepAlive barrier with `legacy composite format error: unsupported argument type`.
- Root cause: the command passed a `uint16` count directly to `legacyfmt.Format`; compilation succeeded, but the formatter's supported runtime argument set does not include that width even though .NET formats `UInt16` normally.
- Prevention: at each localized composite-format callsite, convert protocol-width numerics to the formatter's supported semantic integer type and drive the path through a production-entry transcript; compile-only gates cannot validate dynamic argument dispatch.
- Verification: both `ItemHasBeenCreated` and `PlayerAttemptCreateItem` now receive `int(remaining)`; the exact authenticated session test passes and retains the zero-count System chat before its barrier.

### 2026-08-26 — Administrator bootstrap has a post-ledger buff packet

- Symptom: a raw net.Pipe `@MAKE` logging transcript timed out writing its first KeepAlive after `startGameBootstrapForTest` returned; server cleanup then reported a blocked final GameMaster buff.
- Root cause: the generic bootstrap helper returns at `ServerGuildBuffList`, while an AdminAccount session still emits its private GameMaster `ServerAddBuff` afterward; the raw client had not consumed it, so the server could not resume reads.
- Prevention: after generic bootstrap of an administrator, derive the object ID from `UserInformation` and consume/assert the final private GameMaster buff before sending a barrier or command. Buffered mail-session helpers may hide this direct-pipe ordering requirement.
- Verification: the logging transcript now asserts that exact buff first, then passes its pre-command KeepAlive, zero-count chat, Server queue and post-command KeepAlive checks.

### 2026-08-26 — Revision assertions must baseline after session bootstrap

- Symptom: the new full-bag `@MAKE` test reported revision 2 instead of 1 even though the rejected command no longer committed an item mutation.
- Root cause: the test captured its baseline before authenticated bootstrap, whose existing item-state preparation legitimately advanced the revision.
- Prevention: when proving a command does not advance authority, capture the revision after login/bootstrap and immediately before the production command; do not compare against a pre-session fixture revision.
- Verification: moving the no-op revision snapshot behind the bootstrap KeepAlive made the exact full-bag production-entry test pass while preserving the rejected ID/RNG assertions.

### 2026-08-26 — Logout must distinguish administrator clears from ordinary item revisions

- Symptom: the first latest-auth logout correction made fresh integration fail Craft success/failure, equipment remove, NPC give/take and other persistence transcripts because their legitimate session/world item changes were replaced by an older auth snapshot before write-back.
- Root cause: a no-op auth transaction was treated as unconditional authority refresh; the session had no recorded login revision with which to distinguish a remote `CLEARBAG` advance from ordinary local dirty state.
- Prevention: a broad item revision cannot identify why auth advanced. Keep a separate process-local `CLEARBAG` watermark, baseline it when the session enters, and merge latest auth grids during graceful logout, abrupt cleanup or observer transition only when that clear watermark advances. Ordinary craft/equip/NPC/hero revisions must not trigger rebase; the world still receives the ordinary item revision baseline for stale-result rejection.
- Verification: the known Craft, equipment and NPC persistence regressions pass after the clear-specific watermark; stale-auth-clear tests pass through both `ClientLogout` and abrupt cleanup, while a separate production-session regression advances an ordinary auth revision, dirties the world inventory, logs out and proves that local item survives. Focused count 20, focused race count 5, fresh full normal/race, vet and build all pass.

### 2026-08-26 — Shared result types must be copied from the declaration before wiring fallback paths

- Symptom: the first detached `CLEARBAG` fallback used a guessed `playerEquipmentRefreshResult` and failed the touched-package compile; the real return type is `worldStatsRefreshResult`.
- Root cause: the new fallback was patterned from an existing callsite without first reading the helper's complete declaration and named result type.
- Prevention: locate and copy the exact helper signature before declaring an intermediate value, even when the fields used at the callsite look obvious.
- Verification: the declaration at `world.go` was reread, the exact type substituted, and touched-package compile plus focused administrator tests passed.

### 2026-08-26 — Authenticated fixture identifiers must satisfy the real account regex

- Symptom: the first ordinary-revision/logout regression failed all twenty repetitions at login with result 1 before reaching its revision assertion.
- Root cause: the synthetic account ID `adminrevisionguard` exceeded the production account-name constraint; the fixture built storage state successfully but did not satisfy authenticated entry.
- Prevention: derive account IDs from existing authenticated fixtures or validate their length/syntax before using a low-level seed helper; a successful `AddPlaintextAccount` setup is not proof that the wire login gate will accept the identifier.
- Verification: shortening the ID to `revisionguard` made the exact production-session test pass at count 20 without changing the behavior under test.

### 2026-08-26 — Mine terminal review must retain prior quest fanout and inherited attack quirks

- Symptom: terminal review found mounted requests were not coerced to `Spell.None`, and a later overlapping ordinary-payout failure discarded already committed quest notifications. Main review also had to distinguish `.NET Random.Next(0)` from negative bounds and preserve its source advancement.
- Root cause: the implementation followed `HumanObject.Attack` without the reachable `PlayerObject`/mount override, and failure returns rebuilt an empty result instead of returning accumulated side effects; degenerate random bounds were initially treated as “no draw.”
- Prevention: trace inherited overrides before the shared body; return the accumulated transaction result after any later failure; explicitly model zero/unit random calls and clamp only the source-clamped negative ore range.
- Verification: mounted mining now broadcasts `Spell.None`; overlapping quest diversion retains `GainedQuestItem`/message/quest update after the next allocation fails; zero/unit mine draws retain source order. Focused count-20, race count-5, fresh full normal/race, vet/build passed, and Luna follow-up `01a03ab3-90e8-7de3-8f63-135618c86d34` returned `No findings`.

### 2026-08-26 — Hazard restart fixtures must persist values within real derived caps

- Symptom: the first authenticated map-hazard restart test persisted HP 90 after manually raising only the runtime maximum to 100, then expected 90 after relogin; production bootstrap correctly rebuilt the level-one maximum as 18 and clamped the character.
- Root cause: the fixture replaced runtime vitals without installing a matching durable base-stat profile, so its restart expectation described an impossible persisted character rather than Legacy-derived state.
- Prevention: restart tests for HP/MP damage should either seed a real durable stats profile or use the authenticated character's production-derived caps; never treat a session-local maximum override as restart authority.
- Verification: the test now damages the real level-one 18 HP to 8, persists 8, relogs through production bootstrap and observes 8 with no runtime hazard objects; the focused authenticated test passes.

### 2026-08-26 — Map-hazard Observer exemption needs live account authority, not `IsGM`

- Symptom: terminal main review found the hazard tick checked `IsGM && ObserverMode`, so transient `@LOGIN` promotion would incorrectly gain the exemption; the first fix then froze account-admin state at login, and its first authenticated test used `ReplaceAccounts`, replacing the captured identity so a supposed online grant was invisible.
- Root cause: command permission (`PlayerObject.IsGM`) and hazard admission (`player.Account.AdminAccount`) were collapsed even though Legacy snapshots the former in the constructor but retains a live shared AccountInfo reference for the latter; replacing an entire fixture store is not equivalent to mutating that object.
- Prevention: capture a race-safe authority handle to the exact account identity and mutate fixtures through the real operator patch path; transient GM changes only `IsGM`, operator grant/revoke updates the captured object, removal retains it, and reused IDs create a distinct object.
- Verification: focused auth/unit/authenticated-session tests now drive real `@LOGIN` promotion and cover ordinary account admin, live grant/revoke, removal and reused-ID identity independently; corrected count-20 and race-count-5 commands exit zero.

### 2026-08-26 — Hazard death timestamps must distinguish completed death from revival

- Symptom: terminal Luna review found lethal map-hazard damage called the shared death helper without assigning `BrownUntil`, while the first proposed fix assigned it before the helper and would also mark a successful revival-ring path.
- Root cause: the Go death helper returns the revival transcript without exposing a separate outcome flag, and the existing combat caller's pre-helper assignment was treated as authority instead of tracing the early return in Legacy `PlayerObject.Die`.
- Prevention: after the death helper, assign the death timestamp only when HP remains zero; do not infer pre/post-helper placement from another Go caller when Legacy has an early revival return.
- Verification: the focused hazard test now asserts a completed death receives the exact sampled time and a successful revival retains its prior timestamp; focused count-20, race count-5, and fresh full normal/race, vet and build gates exit zero.

### 2026-08-26 — Hazard session RNG must isolate the connection read loop too

- Symptom: a fresh full test intermittently expected the impact-time MAC unit draw but observed a second hazard-schedule `Next(12000)`; isolated count-100 reproduced the failure.
- Root cause: stopping the world ticker did not stop the authenticated connection loop's own pre-read `world.tick(time.Now())`; exposing a zero hazard schedule after releasing the world lock allowed that loop to consume deterministic RNG before the manual impact.
- Prevention: session transcript tests may use focused create/process helpers when a separate unit test owns world-tick ordering; invoke them under the world lock and restore a far-future schedule before unlocking so the connection loop cannot enter the deterministic RNG window.
- Verification: the spawn/damage/removal session test now keeps schedule mutation and focused helper calls in one lock domain, retains production packet delivery/restart evidence, and passes isolated count-100; focused count-20/race-count-5 and fresh full normal/race, vet and build gates all exit zero.

### 2026-08-26 — Human regen makes long ticks and partial bootstrap vitals observable

- Symptom: the first unexcluded `go test ./cmd/crystal-server ./internal/config -count=1` after wiring `ProcessRegen` failed `TestElectricShockSelfPetAndOtherTargetShockRefreshVisuals` with an extra `ServerHealthChanged`, then timed out at ten minutes in `TestSessionElementalShotObtainTranscript`; a later broad focused run also timed out in `TestSessionMassHealingTranscriptAndJSONHealthPersistence` with client and server blocked on opposite `net.Pipe` writes.
- Root cause: the Shock fixture advanced 10.5 seconds past an accepted magic cast whose MP was below maximum, the two-session Elemental fixture let a partially initialized online player retain the zero natural-regen timer, and MassHealing set `RegenReadyAt` only after bootstrap even though the connection loop had already entered its next pre-read tick. The new production tick correctly made all states observable, but those tests claimed unrelated transcripts and had no reader for the extra packet.
- Prevention: whenever a new periodic producer is migrated, audit every existing synthetic clock that crosses its boundary and every shared-world bootstrap with non-full vitals. For tests that do not own the new behavior, set only the affected runtime timer to a far-future value before the relevant tick or via a pre-ticker player-enter hook; never weaken the production timer or silently discard arbitrary packets.
- Verification: both ten-minute failures remain exit 1 with exact test/stack attribution. After freezing only `RegenReadyAt` in the first two fixtures and moving the MassHealing freeze to a pre-server `setPlayerEnteredHook`, the two-test rerun and isolated `go test ./cmd/crystal-server -run '^TestSessionMassHealingTranscriptAndJSONHealthPersistence$' -count=1 -timeout=2m` exited 0; a fresh unexcluded run is still required.
- Recurrence: the next fresh unexcluded package run timed out after twenty minutes in `TestSessionImmediateSelfBuffBatchOrderMovementPersistenceAndRestore`; the server was blocked writing an unsolicited regen packet while bootstrap/expiry readers expected only the buff transcript. The first isolation patch fixed bootstrap but the focused rerun still exited 1 after thirty seconds because accepted SwiftFeet reset the timer and the synthetic thirty-five-second expiry crossed it. The fixture now installs the one-shot pre-entry freeze for both initial sessions/relogin and re-freezes after the accepted cast through the synthetic expiry. The exact focused rerun exits 0 in under one second; another fresh unexcluded package run remains required.
- Closure recurrence: the first post-compaction unexcluded package run exited 1 after 77 seconds with eighteen parent/subtest failures. Frozen ranged admission exposed an obsolete test expectation, Hero `SendHealthChanged` exposed the missing `ObjectMana` expectation, strict Vamp equality exposed an old inclusive-timer test, and the remaining failures were unrelated synthetic portal/spell/trap/utility ticks crossing natural regen after admission had reset the timer. The finite affected test authority was registered, only those fixtures were re-frozen after their accepted action or pre-entry hook, and the exact failing set then exited 0. A fresh unexcluded `go test ./cmd/crystal-server ./internal/config -count=1 -timeout=30m` exited 0 in 76.904s/1.046s without weakening production regen.
- Terminal review: Luna `01a03d12-8a8f-7390-aaec-e95710f71379` found that Hero auto-pot still ran before the later centralized poison pass and that the first relationship snapshot fix shallow-aliased mutable visibility collections. Hero poison now runs exactly once between natural regen and auto-pot, lethal poison suppresses auto-pot, and runtime/map-transition visibility snapshots deep-copy Buffs, Flags and enemy-guild state. The first detachment fixture incorrectly installed `EnemyGuildIDs` before enter and panicked after bootstrap rebuilt it to nil; moving that runtime projection under the post-enter world lock made the exact five-test rerun pass.
- Follow-up review: Luna `01a03d3b-74be-75c3-af27-ec151385358f` then found TownRevive overwrote two detached projections with shallow copies. Both replacements now use the same deep snapshot, the existing TownRevive test mutates live Buff/enemy-guild state after return to prove detachment, focused count-20/race-count-5 passes, and the final follow-up returned `No findings`. Current-fingerprint fresh `go test ./... -count=1`, `go vet ./...`, `go build ./...`, and fresh unexcluded `go test -race ./... -count=1 -timeout=60m` all exit 0.

### 2026-08-26 — HP-drain tests must distinguish recipient fanout and capability context

- Symptom: the first focused tests expected one observer copy per packet, miscomputed a strict-threshold Hero payout, and installed a stat into a nil fixture map; after those fixture corrections, the first fresh full run failed `TestCombatDurabilityOrdinaryPlayerMonsterContextControlsWeaponRoll` because weapon durability fell from 10 to 8 instead of 9.
- Root cause: unit expectations flattened multi-recipient notifications without retaining duplicate broadcast IDs, and production reused a new `DamageWeapon` action field both as HP-drain admission evidence and as an instruction to execute durability after the wrapper had already damaged the weapon.
- Prevention: derive transcript expectations per recipient, calculate payout from the preexisting accumulator plus the new float32 increment, initialize post-bootstrap maps explicitly, and keep capability/admission metadata separate from commands that perform a side effect.
- Verification: corrected focused tests pass; the exact durability regression plus HP-drain tests pass together, current HP-drain count-20 and race-count-5 exit 0, and the post-fix fresh unexcluded full test exits 0.

### 2026-08-26 — Revelation session fixtures must preserve CreateCharacter argument order

- Symptom: the first authenticated high-SC Revelation test failed immediately with character result 2 instead of reaching bootstrap.
- Root cause: the fixture passed class 2 in `CreateCharacter`'s gender position, relying on remembered semantic order instead of the actual `(id, name, gender, class)` declaration.
- Prevention: reread mutation-helper declarations before constructing a production-entry fixture, especially adjacent same-type enum parameters; label or review each positional argument against the declaration.
- Verification: changing the call from `(2, 0)` to `(0, 2)` made the exact authenticated high-SC Revelation test exit 0 and reach its wrapped-expiration transcript.
- Recurrence: the first active-Revelation bootstrap extension then timed out waiting for the viewer session to stop because viewer logout synchronously broadcast `ObjectRemove` into two unread `net.Pipe` peers. Install both peer removal readers before closing the viewer; the corrected authenticated test exits 0 and verifies the full bootstrap/removal lifecycle.

### 2026-08-27 — Dynamic-constructor convergence requires fixture-wide RNG prefixes

- Symptom: the first server-package run failed creator fixtures at unexpected bounds `100/8/10000/3000/1000`, shifted HellLord quake values, added a valid later Regen indicator, and a ZumaTaurus session fataled while holding the world lock, leaving cleanup deadlocked until the ten-minute package timeout.
- Root cause: old fixtures described creator-local choices only because `materializeMonster` consumed constructor draws on package-global randomness; once production correctly injected one stream, every deterministic callback and transcript had to include the full constructor prefix/suffix and isolate non-target Regen.
- Prevention: constructor-bearing creator fixtures must record the complete ordered stream, return a future first-Regen value when Regen is not under test, and update downstream semantic rolls by their exact new positions rather than guessing a constant offset. Fatal callbacks used under a held world lock require focused execution before package gates.
- Verification: all failure-proved creator fixtures were corrected from exact bounds; the combined dynamic creator focused set and protected constructor/natural-Regeneration set exit 0 without timeout.

### 2026-08-27 — Cross-domain fixture IDs must retain their declared widths

- Symptom: the new delayed-HellKnight production-entry test failed touched compile because a `uint32` object ID was assigned directly to `protocol.SelectInfo.Index`, which is `int32`.
- Root cause: the fixture reused one numeric identity across world-object and character-protocol domains without checking the target field type.
- Prevention: preserve typed IDs at each boundary and use an explicit checked conversion when a test intentionally projects one value into both domains; run touched compile before semantic test execution.
- Verification: the exact `int32(observerID)` projection compiles, and the RootSpider/HellKnight delayed-spawn tests pass at count 10 and under race at count 3.
- Transcript recurrence: the wider RootSpider group then failed five exact fixtures because they still expected only `ObjectMonster`; Legacy `MonsterObject.Spawned` broadcasts object then health. Updating the unit and authenticated-session transcripts to require `ObjectMonster,ObjectHealth` made the full dynamic/RootSpider/HellLord group pass at count 10 and under race at count 3.

### 2026-08-28 — Ordinary SnakeTotem target fixtures must not create accidental stacking

- Symptom: the first sibling-only target rejection extension made the later
  parent-linked CharmedSnake impact remain at HP 100 in all ten focused runs.
- Root cause: the copied sibling retained the tested child's exact cell, so the
  real CharmedSnake `ProcessSearch` stacking branch moved the attacker before
  the later adjacent-impact scenario; the target-link correction itself was
  not the failure.
- Prevention: when a fixture adds a relationship-only sibling, place it on a
  distinct valid cell unless stacking is the behavior under test, and preserve
  movement state separately from target-link state.
- Verification: moving only the sibling to `(7,7)` made the exact target-link
  test pass ten times while retaining the negative sibling-only assertion and
  the positive parent-targeted Monster impact.
- Death-order recurrence: the recovered SnakeTotem cleanup fixture expected
  `killMonsterLocked` to return no packets and deferred both deaths to the next
  tick. Legacy `SnakeTotem.Die` calls `base.Die()` first, which immediately
  broadcasts the parent `ObjectDied`, then walks `SlaveList` in reverse and
  calls each child `Die`, before zero-dead-time processing removes either
  object. The fixture now asserts parent/child `ObjectDied` at the death call
  and parent/child `ObjectRemove` at the cleanup tick; its exact count-10 and
  the complete `TestOrdinarySpawn` focused run exit zero.
- Protected-fixture recurrence: the existing summon-created expiry test still
  indexed the child map entry after the parent-first tick. The parent has the
  lower ObjectID, so its `SnakeTotem.Die` kills and zeroes the child's dead
  timer before that child reaches its own process/removal position in the same
  sweep. The test now requires the parent corpse and an already-removed child;
  its exact count-10 exits zero. When a lifecycle correction changes packet or
  removal order shared by protected siblings, update only the stale assertion
  from the traced Legacy object order rather than reverting production order.
- Callsite-ledger recurrence: the first ordinary CharmedSnake wrapper entry
  guessed two calls, but the AST correctly found three syntactic paths: the
  initial forward walk plus the clockwise and counter-clockwise fallback loops.
  Static callsite counts must enumerate physical branches from the owned
  function rather than infer them from semantic outcomes; the corrected ledger
  count of three passes its exact count-10 gate.
- Compile recurrence: the new table subtest used `fmt.Sprintf` without adding
  `fmt` to the existing import block. The immediate focused compile caught the
  undefined symbol before behavior analysis; after reconciling and formatting
  imports, the exact single-death/suppression test passes at count ten.
- Corpse-projection review finding: Go retained an ordinary CharmedSnake
  child's `OwnerObjectID` for cleanup and projected it as `MasterObjectID` even
  after death. Legacy `MonsterObject.Die` clears `Master` before a corpse can
  be serialized, so `GetInfo` emits master zero while the CharmedSnake name
  also loses its parent suffix. The serializer now exposes the retained owner
  only while live, and the exact lifetime/death test locks corpse name/master
  projection at count ten.
- Shared-GetInfo review finding: the first ordinary-map fix forced subclass
  images only in the ownerless serializer, leaving summoned AI=60-63 objects
  on configured `MonsterInfo.Image` values even though all four Legacy
  overrides always emit enum images 359-362. The bounded ordinary-pet
  serializer now applies the same class image rule and clears the archer
  owner/name projection after `Die`; a four-class live/corpse table and the
  authenticated Vampire spawn transcript pass at count ten. When a concrete
  virtual `GetInfo` override is independent of creator provenance, audit every
  serializer dispatch branch rather than scoping the field fix by ownership.
- Nested-master recurrence: a summon-created CharmedSnake used the player's
  name directly even though its Legacy `Master` is the SnakeTotem, whose own
  live name already includes the player. Child creation now snapshots the
  parent's exact live display name, producing
  `Child(Parent(Player))`, while `MasterObjectID` remains the parent; the
  summon/expiry production test locks name, image, master and Extra at count
  ten.
- Runtime-admission fixture failure: after ownerless AI=60-63 was correctly
  placed behind the production `monsterAIEnabled` seam, four hand-built
  SnakeTotem/CharmedSnake tests still left that seam disabled and therefore
  never reached movement, aggro, targeting or minion creation. The fixtures,
  not production behavior, were stale. Every fixture that expects ordinary
  ownerless AI must enable the runtime explicitly; a separate negative test
  keeps the disabled state quiet. The corrected focused count-10 and race
  count-3 gates exit zero.

### 2026-08-28 — One-shot player-enter hooks must be reinstalled per session

- Symptom: the first authenticated CHAT map-context transcript froze natural
  regeneration for only the first of three players; the next setup barrier saw
  an unexpected `DamageIndicator` from a later player's live regen.
- Root cause: `setPlayerEnteredHook` is intentionally consumed and cleared by
  each successful `world.enter`, but the multi-session fixture installed it
  only once before the loop.
- Prevention: reinstall every one-shot enter hook immediately before each
  session bootstrap, and update all already-entered runtime players inside that
  hook when a shared-world timer must remain frozen.
- Verification: the hook now runs once per client open; the exact authenticated
  transcript, count-10 repetition and focused race count 5 all exit 0 with no
  stray regen packet.

### 2026-08-28 — Access tightening requires explicit default activation and dead-call silence

- Symptom: the first full server-package run after the P7 access candidate
  produced five failures: two guild-buff, one mail and one wedding fixture timed
  out after calling a default NPC object directly, while the dead-shop fixture
  waited for panels after an ordinary NPC call that Legacy rejects. The first
  correction then failed compile by guessing a nonexistent
  `shopSession.npcObjectID` field.
- Root cause: old tests encoded Go's permissive direct-default fallback and
  dead ordinary navigation instead of Legacy `CallDefaultNPC` identity and
  `PlayerObject.CallNPC` dead gates; the correction reused a presumed symmetric
  fixture field without reading the actual helper, which fixes the runtime NPC
  ID at 1.
- Prevention: when tightening a production admission gate, enumerate every
  fixture that configures its producer. Default-NPC transcripts must enter via
  the max-uint Client sentinel, consume `NPCUpdate`, select a real `[@_CLIENT]`
  response link, and only then call the default object. Dead ordinary calls are
  silent and require a KeepAlive barrier, not expected panels. Read the concrete
  fixture struct/helper before naming an ID field, and register expanded test
  authority before editing protected files.
- Follow-up finding: the candidate still assigned the default object before
  looking up `[@_CLIENT]`, so a sentinel with a missing page wrongly authorized
  later direct calls. Legacy changes `NPCObjectID` only in
  `NPCScript.ProcessSegment`; Go now changes active default identity only from
  `setActivePage`. The regression first proves missing Client page + sentinel is
  still silent, then reloads a real Client page for the positive/input path.
- Compile recurrence: removing the eager assignment left the destructured
  `defaultScript` unused. The first minimum compile failed and was discarded;
  the snapshot call now binds that return to `_`, and the exact compile rerun
  passes. Gate-owner refactors must update producer outputs and consumers in the
  same compile transaction.
- Visibility-log failure: the first separate Legacy `VisibleLog` map was only
  populated by `NPCVisibilitySend`, but bootstrap/static refresh mutates the
  wire cache in a reserved batch and bypasses that callback. Three quest tests
  timed out because their initially visible NPCs had no access-log entry. The
  access lookup now lazily promotes an already-sent wire-cache entry, while
  later global removals do not clear the Legacy log and player-gate failures do.
  Exact failed tests were rerun together at exit 0.
- Fixture recurrence: the new persistent-VisibleLog quest case then waited for
  `NPCUpdate` even though that isolated world never configured a default NPC.
  Ten repetitions each timed out at the same read; the invented packet was
  removed and the exact test now passes ten times. Callback packets may only be
  expected after the fixture proves a configured default producer.
- Integration finding: the first fresh `go test -count=1 ./...` stopped before
  vet/build because five storage tests still called `[@STORAGE]` on an NPC with
  no script or selected link. That was the removed direct-storage access
  fallback, not a storage response defect. After expanding access-only test
  authority, each fixture now loads a real MAIN link plus existing STORAGE page,
  consumes MAIN, then enters STORAGE. The exact main/operator/P11/two storage
  tests pass together; the full integration command must be rerun from test.
- Second integration finding: the next server-package run exposed market,
  refine, repair, P11 awakening and four shop fixtures whose special pages had
  no segment. Legacy only changes `NPCPage` from `NPCScript.ProcessSegment`, so
  an empty page may display its special panel but cannot authorize the later
  transaction. The fixtures now retain empty response payloads with an explicit
  `#SAY` segment; exact reruns pass without weakening any business assertion.
- Unrelated-gate attribution: one subsequent server-package run failed only
  `TestSessionHallucinationTranscript` after its 30-second pipe timeout. A clean
  `git archive` of the pre-leaf Go HEAD passed once but reproduced the same
  failure under `-count=20`; current exact rerun then passed. The failure is a
  pre-existing time-sensitive fixture, not an access-gate stack. Script Load's
  first integration gate reproduced the same 30-second closed-pipe failure with
  no loader stack; its exact rerun and the next fresh unexcluded full test both
  passed. The prior Access fresh full race remains the current full-race gate.
- Verification: owned minimum compile, focused production sessions, count-10
  repetitions and focused race count-3 pass. Fresh `go test -count=1 ./...`,
  `go vet ./...`, `go build ./...`, and fresh `go test -race -count=1 ./...`
  all exit 0 after the access-only fixture corrections.

### 2026-08-28 — Panel-routing negatives must reach the intended boundary and authoritative NPC fixtures need maps

- Symptom: the first direct-storage negative was described as a missing-page
  test even though the selected-button gate rejected it before lookup; a later
  exact-active-NPC unit test rejected both candidates instead of only the
  out-of-range one.
- Root cause: the test name inferred the production branch from the requested
  key, and the unit fixture supplied authoritative NPC ObjectIDs without first
  registering their map, so `loadNPCs` correctly skipped both objects.
- Prevention: prove lookup absence independently from client button admission,
  name each negative for the gate it actually reaches, and register a real map
  with valid coordinates before loading authoritative runtime ObjectIDs.
- Verification: the routing helper now has an explicit matched/unmatched
  dispatch test; the session test is named for direct-storage admission; the
  exact-active-NPC fixture registers a 20x4 map and distinguishes an out-of-range
  active NPC from an in-range decoy. Focused count-10 and race count-3 pass.

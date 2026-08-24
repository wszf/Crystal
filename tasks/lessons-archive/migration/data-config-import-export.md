### 2026-08-18 — AI=181 Player 目标持久种类与动作编码不同

- Symptom: WaterDragon session reveal 断言把 Player 的 `MonsterAITargetKind` 写成 0，实际搜索后持久状态为 `ancientBringerPlayerTarget (1)`。
- Root cause: Player 只有写入延迟攻击动作时才映射为 Legacy 的默认 kind 0；世界目标状态仍保存统一目标枚举 1。
- Prevention: 分别断言持久目标字段与 `worldMonsterAttackAction.TargetKind`；不要用协议/动作的默认 Player 编码推断 AI 状态枚举。
- Verification: 修正 transcript 后，reveal 状态断言继续确认 kind=1，后续近战 action 使用 kind=0 并按预期命中。

### 2026-08-14 — UserMagic 冷却必须在世界快照中转换为剩余时间

- Symptom: Go 运行时已经设置了每个技能的 `CastReadyAt`，但注销快照仍复制旧的 `StoredMagic.CastTime`；跨注销重登会丢失活动冷却，技能升级经验也没有统一的完整魔法 slice 提交入口。
- Root cause: 只迁移了客户端 `ClientMagic` 的字段和在线施法门禁，没有把 Legacy 注销时“绝对 CastTime 减当前时间、就绪写入 int.MinValue”这条持久化边界接到 world/auth 提交路径。
- Prevention: 运行时冷却使用确定的 `now` 转换为正的剩余毫秒，已就绪统一使用 Legacy 哨兵；`playerCharacterSnapshot` 与显式/异常注销、Observer 接管共同调用完整 `UpdateCharacterMagics`，并用恢复后的客户端 payload 验证边界。
- Verification: 新增 world CastTime 活跃/到期快照测试、auth 魔法 slice 深拷贝测试；P5 ElectricShock 定向、协议 packet、服务端包级编译均通过，后续继续执行全量普通/race 门禁。

### 2026-08-13 — 等级化实体恢复必须在刷新属性后无条件应用保存生命值

- Symptom: 等级 2 宠物基础 HP 20、刷新后 MaxHP 60、保存 HP 50 时，旧条件 `info.HP < pet.HP` 会错误保留初始化 HP 20；Wizard 非 Clone 的零 `TameTime` 也被误当成永久宠物，且到期没有名称广播。
- Root cause: 用刷新前/初始化生命值作为恢复条件，并把 `TameTime > 0` 错当成“字段是否存在”；Legacy 实际对 Wizard 非 Clone 总是执行 `now + TameTime`，到期解除主人后发送 `ObjectName`。
- Prevention: 恢复等级化实体时先计算 MaxHP/属性，再无条件赋保存 HP 并钳制到 `[0, MaxHP]`；时间字段的零值语义必须沿真实加载调用核对，不能用非零判断代替存在性；所有所有权到期路径同时核对可见名称/颜色等广播。
- Verification: 新增等级宠物 HP/属性/速度、特殊三倍经验/等级上限、PetSave 筛选、正负剩余时间，以及零 TameTime 到期 `ObjectName` 的回归测试；协议 serializer 与 malformed-data 测试通过。

### 2026-08-13 — 声明的持久化字段不代表 Legacy 实际写入记录

- Symptom: `PetInfo` 声明了 `TameTime`，但 117/0 `Server.MirADB` 的构造、保存和读取路径均未处理它，每只普通宠物实际仍只有 14 字节；若按字段声明追加读取会错位后续角色数据。
- Root cause: 把模型成员列表误当成二进制格式契约，没有以真实 reader/writer 调用序列核定记录宽度。
- Prevention: 迁移 Legacy 二进制记录时逐字段对照构造、Save 和 reader，并用后续哨兵字段锁定偏移；未写入旧格式但 Go 运行时需要的状态通过 Go JSON 模型单独贯通，不能改变旧记录宽度。
- Verification: Go 导入器继续严格消费 14 字节并令导入 `TameTime=0`，偏移回归测试保留后续字段；非零/负 `TameTime` 已通过 protocol/auth JSON 往返测试。

### 2026-08-13 — 延迟物品生产必须同时固定锁序和最终快照顺序

- Symptom: 智能生物自动生产黑石最初在 `world.mu` 内调用 auth 全局 ID 分配器；移出锁后，生产通知又排在通用持久化通知之前，后续旧角色快照可能覆盖刚生成的物品。
- Root cause: 只把跨域调用改成延迟执行，没有把同一 tick 的所有快照按实际提交时刻排序；“锁外执行”与“最终权威状态”被当成两个无关问题。
- Prevention: world 锁内只准备不可变创建上下文，所有 auth/全局 ID/随机物品创建在锁外执行；同一 tick 先提交锁内捕获的普通状态和拾取金币，再执行物品创建、重入 world 合并并提交包含新物品的终态。测试必须让创建回调主动获取 `world.mu` 验证无反向锁序，并逐个执行通知后检查 auth 最终物品。
- Verification: 黑石创建回调已整体移到 world 锁外，生产通知排在通用 persist 之后；锁序超时测试和 auth 终态测试通过，`cmd/crystal-server` 整包测试通过。

### 2026-08-13 — 独立 Go 导出器必须复现 Legacy 加载语义而非只解码字段

- Symptom: 世界导出器能完整解码 117/0 文件，但账户归档角色仍会绑定拍卖，Windows 反斜杠/大小写路径在 Unix 主机失效，嵌套 `#INSERT` 没有继续展开，缺失物品定义会中止整个拍卖或公会文件，Quest NPC ID 也可能与运行时实例不一致。
- Root cause: 把二进制字段布局正确等同于加载结果等价，遗漏了原服务端在解码后的归档过滤、文件系统语义、增长列表展开、`BindItem` 容错及地图/NPC 实例化顺序。
- Prevention: 所有独立 Go 迁移工具沿“读取 → Legacy 过滤/绑定 → 可观察投影”逐层验收；路径同时兼容 `\\`/`/` 和 Windows 大小写，动态 include 保留原增长列表顺序并加循环/规模保护，记录必须先完整消费再按绑定结果丢弃，运行时身份由导出器显式携带并保留旧 JSON fallback。
- Verification: 新增归档月份边界与拍卖绑定、跨平台路径、两层/循环 Drop include、拍卖/公会缺失定义、NPC 数据库/地图顺序及无效坐标、Monster 客户端投影的 Go 回归测试；定向测试通过，提交前继续执行全量 test/race/vet/build 门禁。

### 2026-08-12 — 市场文本必须区分网络快照与 AddItem 后的 UserItem

- Symptom: Market 系统邮件曾输出未格式化的 `7000`，市场 Success/Hint 只清理 ASCII 数字；固定价物品部分合堆时，Go 文本显示原始 `(5)`，原版因 `AddItem` 修改同一对象而显示剩余 `(3)`。
- Root cause: 邮件与会话层各自实现 FriendlyName，且把发送前必须保留的 `GainedItem` 快照错误地同时用于操作后的文本。
- Prevention: FriendlyName 一律先按 Unicode 数字清理尾缀、再移除方括号，金额使用千分位；物品入包前 clone 原始数量，Success/Hint 则使用 AddItem 后的剩余数量。
- Verification: auth 与 net.Pipe 测试分别锁定 `7,000`、全角数字清理、原始 `GainedItem.Count=5` 和成功文本 `(3)`。

### 2026-08-12 — 经济事务必须先落盘再做可失败的网络投递

- Symptom: SellNow、到期和 GameShop 成功后若先写在线连接，断线错误可能让已提交经济状态来不及保存；批量邮件遇到一个失效连接也可能短路其他玩家。
- Root cause: 把持久化提交和在线通知当成一个顺序循环，没有区分权威状态与尽力投递的副作用。
- Prevention: 在持锁事务内原子准备/提交货币、物品、拍卖、邮件和 ID，释放锁后先保存 JSON，再按 legacy 顺序投递；批量通知继续遍历，只保留第一个错误。
- Verification: 失败不扣款/不改拍卖、断线后状态保存、多个收件人继续投递以及普通/race 测试通过。

### 2026-08-11 — 迁移状态必须贯通原版导出器与 Go 持久化桥

- Symptom: Refine 的 Go JSON/运行时状态已经支持 `CurrentRefine`，但原版账户导出器没有输出当前强化物品和收取截止时间，跨数据库迁移会丢失进行中的强化任务。
- Root cause: 只检查了 Go 侧的 JSON bridge 和 logout persistence，没有沿着原版数据库 → .NET exporter → Go JSON → Game stage 的完整链路核对字段与时间单位。
- Prevention: 每个迁移功能都要同时检查原版持久化字段、导出器字段、Go bridge 字段和运行时恢复逻辑；单调计时器必须在导出时转换成跨进程的绝对时间，并验证零值语义。
- Verification: `Crystal.LegacyAccountExport` 已补充 Refine workbench/current item/deadline 导出；当前环境没有 .NET SDK，需在具备 SDK 的环境补跑 exporter 编译，Go 验证继续执行全量 test/race/vet/build。

### 2026-08-11 — Go 配置解析不要在同一作用域重复声明 err

- Symptom: 新增配置加载器后，`go test ./...` 和 `go vet ./...` 在 `var err error` 处报告 `err redeclared in this block`。
- Root cause: 读取文件的短变量声明已经在当前作用域创建了 `err`，解析 INI 时又用 `var err error` 重复声明。
- Prevention: 在同一作用域复用已有错误变量；只有需要缩小作用域时才使用带初始化的局部声明。
- Verification: 修正后重新运行 `gofmt`、`go test ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 地图切换必须持久化 CurrentMapIndex

- Symptom: 跨地图后只更新角色坐标，logout 再登录时坐标会被解释为旧地图坐标，角色回到错误地图。
- Root cause: 原有 `UpdateCharacterRuntime` API 只接受 x/y/direction，没有把地图索引作为同一份运行时位置状态保存。
- Prevention: 所有会改变地图的路径使用 map-aware runtime 更新接口；普通坐标更新接口保留给不改变地图的旧调用，并为 map index 单独断言。
- Verification: 增加 `TestUpdateCharacterMapRuntimePersistsMapIndex`，地图 movement net.Pipe 测试使用新接口更新运行时状态，Go 全量测试通过。

### 2026-08-11 — .NET exporter 与 Go JSON bridge 必须逐字段核对语义

- Symptom: 导出器把 `CurrentQuests` 写成任务对象列表，而 Go bridge 的同名字段是 `[]int32`，导致真实账户 JSON 无法按预期恢复。
- Root cause: 只对照了字段名，没有对照字段的元素类型和“ID 列表/详情列表”职责。
- Prevention: 每增加跨语言字段，必须同时核对 C# 类型、导出 JSON 形状、Go 结构体类型和 round-trip fixture；`currentQuests` 只保存 ID，`questProgress` 保存详情。
- Verification: 增加 JSON 原始形状断言和加载后进度 round-trip 测试。

### 2026-08-12 — 物品加入背包前必须复制网络快照

- Symptom: NPC 商店批量购买改为先更新角色背包后再编码 `GainedItem` 时，`addCharacterItem` 合并/消耗了传入对象，客户端收到的购买数量变成 0。
- Root cause: 领域层的背包操作会原地修改传入 `StoredItem`；网络 payload 不能直接引用会被后续事务改变的对象。
- Prevention: 所有会调用 `addCharacterItem`、堆叠合并或删除的路径，在第一次状态变更前复制独立的 packet snapshot；测试同时断言内部持久化数量和线上的 `GainedItem.Count`。
- Verification: NPC 商店普通购买和 `[BUYBACK]` 会话 transcript 均断言数量/UniqueID，Go 全量 test、race、vet、build 和 `git diff --check` 通过。

### 2026-08-12 — 持久化物品目录不能代替连接级已发送状态

- Symptom: Quest 定义在第一次登录顺序正确，但定义被写入 `SelectInfo.ItemInfos` 后，重登会把全部定义错误提前到 `MapInformation` 前；Mail 附件也可能因目录中已有定义而跳过其应位于 `ReceiveMail` 前的 `NewItemInfo`。
- Root cause: `gameItemCatalog` 同时承担服务端持久化定义目录和客户端当前连接已知集合；前者跨登录保留，后者必须随每次 StartGame 清空，两者语义不能合并。
- Prevention: 每个连接维护独立、并发安全的 sent-item-info 集合；初始角色格、Quest、Recipe、Mail、NPC 商品和显式 RequestItemInfo 各按原版阶段检查并标记，目录是否已有定义只决定服务端是否追加，不能决定是否发包。
- Verification: Quest 重登锁定“当前携带定义 → Map/User → 其余 Quest 定义”，Mail 嵌套附件锁定“CompleteQuest → NewItemInfo* → ReceiveMail”，UsedGoods 与重复 RequestItemInfo transcript 通过；Go 全量 test、race、vet、build 和 `git diff --check` 全部通过。

### 2026-08-14 — 导入全局计时器后必须重算未保存重生组的下一 tick

- Symptom: 世界先按默认计时器创建重生组，再导入 Legacy `CurrentTickcounter` 时，未匹配 `RespawnSave` 的组仍保留默认基准的 `NextSpawnTick`，可能在启动后立即重生。
- Root cause: 加载顺序把静态组构造与计时器覆盖分开，却只对有保存记录的组应用了新计时器。
- Prevention: 配置计时器后，保存记录匹配组使用持久化 `NextSpawnTick`，其余 tick 组统一以导入的当前计数器加 `RespawnTicks` 重算；用非零导入计数器测试无保存记录路径。
- Verification: Go 重启/持久化测试锁定初始 `CurrentTickcounter=7` 时未保存组的下一 tick 为 9，随后全仓普通/race 门禁通过。

### 2026-08-18 — 即时法术练习不能被 admission 快照覆盖

- Symptom: EnergyRepulsor 的 resolver 已发出 `MagicLeveled`，但 caster 的 `Magics` 中经验仍为 0。
- Root cause: `magicAttack` 在 admission 阶段复制了 `worldMagic` 值；即时 resolver 通过 map 更新经验后，函数尾部仍把旧快照写回，覆盖了练习结果。
- Prevention: 任何在 `magicAttack` 内即时执行并可能调用 `levelMagicLocked` 的分支完成后，重新读取 `player.Magics[spell]`，再写入本次 cast timestamp；测试同时检查状态 map 和升级通知。
- Verification: 加入即时效果后的 map-value 刷新，EnergyRepulsor 经验/通知定向测试、普通/race 全量门禁和构建检查全部通过。

### 2026-08-18 — 物品消耗后的 Buff 持久化必须使用最终快照

- Symptom: UltimateEnhancer 认证转录的 DeleteItem 和运行时护符数量正确，但 auth JSON 仍保存 2 个护符。
- Root cause: 私有 AddBuff 持久化通知在 `ConsumeItem` 前捕获了角色快照，随后覆盖了主处理器已经写入的 1 个护符状态。
- Prevention: 任何“先 AddBuff/技能升级、后 ConsumeItem”的路径都要在物品变更后追加最终持久化快照，并在 net.Pipe 测试同时断言包序和 auth 存档数量。
- Verification: UltimateEnhancer 在消耗后追加最终快照；认证测试确认 `AddBuff -> MagicLeveled -> DeleteItem -> Magic` 包序、运行时数量和持久化数量均正确。

### 2026-08-19 — 魔法扣费必须同步运行时与持久化角色 MP

- Symptom: BladeAvalanche authenticated session 的健康包和运行时 `player.MP` 都已变为 86，但最终 `player.Character.MP` 仍为 100，导致会话状态断言失败。
- Root cause: Go 通用 `magicAttack` admission 只扣减了运行时 MP；与普通攻击和 SpellToggle 路径不同，它没有同步角色持久化结构中的 MP 字段。
- Prevention: 每个会改变 MP 的世界路径都必须同时更新运行时对象和 `Character` 镜像；会话测试除了检查包和运行时值，还要检查最终角色状态。
- Verification: 在通用魔法扣费点同步 `player.Character.MP` 后，BladeAvalanche 世界/会话定向测试通过，并确认最终两个 MP 字段均为 86。
- Strengthening after recurrence: 仅同步运行时与 `Character` 仍不足以覆盖攻击路径；若会话持有独立的 `gameMP`，世界侧的即时 MP 变化还必须调用 `PersistHealth`，否则退出清理可能用旧缓存覆盖 auth。
- Verification after strengthening: MPEater 会话曾复现 auth MP 从运行时 25 回到初始 221；补上 MP 变更点的 `PersistHealth` 后，断开并等待服务结束仍持久化为 25。

### 2026-08-19 — 新账户导入初始化必须核对嵌套状态类型

- Symptom: 接入 Legacy 二进制账户导入后，Go 目标包最小编译失败，提示租赁迁移暂存表赋值类型不匹配。
- Root cause: 导入替换逻辑按角色索引清理暂存状态，但根据字段名误写成单层 `map[int32]...`，实际服务字段还按物品 ID 嵌套了一层 map。
- Prevention: 新增持久化导入/替换路径时，先读取目标字段的完整声明并核对所有嵌套层级；新增状态初始化后立即运行受影响包的 `gofmt` 与 `go test -run '^$'` 编译门槛。
- Verification: 改为 `map[int32]map[uint64]rentalMailTransferProposal` 后，auth、legacyaccount、legacyaccountbridge 与 crystal-server 目标包最小编译通过；后续导入往返、密码登录测试也通过。

补充证据：本批把普通测试、服务端 race 和全仓编译串成一条命令时，首个 `cmd/crystal-server` 普通测试在约 3 分 40 秒内无输出，仍处于默认 10 分钟测试超时窗口，手动中止后后续门禁未执行。预防再强化为：实时服务端测试必须独立运行并显式设置 `-timeout`；内部包测试、全仓编译和 vet/build 必须拆成可单独判定的命令，不能用一条串行命令掩盖后续门禁是否实际执行。

补充验证：随后独立执行 `go test ./cmd/crystal-server -count=1 -timeout=180s` 于约 39.7 秒通过，`go test ./... -count=1 -timeout=180s` 也全部通过；因此本次组合命令的中止只能作为无界等待证据，不能归类为当前服务端包的确定性失败。

### 2026-08-19 — Go 配置加载的多返回值先拆开再组装

- Symptom: 元素配置首轮定向测试未能编译，提示 `loadElementalSettings` 的多返回值被用于结构体字面量的单值位置。
- Root cause: 为保持初始化紧凑，把 `(value, error)` 函数调用直接嵌入 `legacySettings` 字面量；Go 不允许该多返回值上下文。
- Prevention: 配置加载函数返回值包含错误时，先显式接收并检查 error，再赋入结构体字段；完成新 loader 后先跑目标包的最小编译/定向测试。
- Verification: 改为显式赋值并传播错误后，`go test ./internal/worlddata ./internal/legacyworld ./cmd/crystal-server`、`go test ./...`、`go vet ./...` 与 `go build ./...` 均通过。

### 2026-08-19 — AI=89 HumanObject 重载不能从共享伤害路径补加 AttackBonus

- Symptom: 初版 Go IcePillar 玩家命中 helper 为物理调用点补加 `AttackBonus`；逐行对照 Legacy 后确认 `IcePillar.Attacked(HumanObject)` 自身不执行基类的 AttackBonus 加成。
- Root cause: 把普通 MonsterObject 命中路径的“基类在目标端加成”投影到一个覆盖重载，忽略了 IcePillar override 直接使用传入 damage。
- Prevention: 对覆盖 `Attacked` 的怪物先标注它是否调用基类/是否显式加 AttackBonus，再决定调用方是否传递或补算；测试 fixture 必须设置非零 AttackBonus。
- Verification: helper 已移除额外加成，测试以 AttackBonus=2 仍断言传入 damage=10；IcePillar 定向测试通过。

### 2026-08-24 — Source-precedence transcript 必须隔离关服 checkpoint 与 Legacy 留存门禁

- Symptom: 主审将候选从 HTTP wrapper 收紧到 `runServerWithContext` 后，首次编译残留未使用 `fmt`；首次 direct-binary reload 又只返回 2 个而非 3 个账户；第二会话随后被首连接安装的短期 IP block 静默拒绝并在版本握手处 EOF。
- Root cause: 删除 HTTP fixture 时未同步 import；新建账户没有角色，命中了 Legacy 对首账户之后无留存角色账户的加载过滤；多连接生产 transcript 没有隔离进程级 IP block。
- Prevention: wrapper 收紧后立即跑 touched compile；需要通过 117 loader 验收的新账户必须创建一个可留存角色或避免按原始写入数断言；非 gate 测试显式将 `IPBlockDuration` 设为零并保留真实 `MaxIP` 语义。为单独证明 graceful checkpoint，先等待 SaveJSON hook 完成，再把冲突 binary 写回，最后取消生产 context。
- Verification: 移除残留 import、给 TCP 新账户创建 `TCPHero`、将测试 IP block 设为零后，精确定向测试曾通过；独立 review 随后指出该夹具会不必要地触及 P3 character-metadata 边界，最终改为在同一 login-stage 连接用已完成的 ChangePassword 路径触发 SaveJSON，并直接重载两个原有 JSON 账户。关服前恢复的冲突 binary 仍只能由最终 checkpoint 替换，direct 117 reload 保留 JSON-backed 账户、密码变化与登录 metadata，并清除冲突 binary global sentinel。

### 2026-08-24 — 117 角色夹具头部必须从真实 writer 字段序列推导

- Symptom: `CHAR-P3-BAN-DELETE-001` 首个 retained-tombstone 定向测试因手写 version-117 头部偏移错误而在目标角色字段前解析失败。
- Root cause: 夹具按注释猜测相邻 header 字段，没有逐项对照现有 Go writer/parser 的实际宽度与顺序。
- Prevention: 二进制夹具先复用现有 fixture writer，并从生产 parser/writer 逐字段列出 header、count 和 record 边界；新增语义断言前先跑单个 parse smoke。
- Verification: 修正头部字段序列后，recent tombstone、严格归档边界和第五 retained record 测试的普通、`-count=10`、race `-count=3`、完整 package 与 vet 均退出 0。

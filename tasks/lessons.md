# Lessons learned

Record project-specific corrections and failure-prevention patterns here.

## Entry format

```markdown
### YYYY-MM-DD — Short title

- Symptom:
- Root cause:
- Prevention:
- Verification:
```

### 2026-08-11 — Go 多变量声明必须显式使用 var

- Symptom: 新增 Game 阶段位置状态后，`go test ./...` 在 `gameX, gameY int32` 处编译失败。
- Root cause: Go 的局部变量声明不能使用没有 `var` 或初始化表达式的裸类型声明。
- Prevention: 新增状态变量时统一使用 `var x, y int32`，并在提交前运行 `gofmt` 与 `go test ./...`。
- Verification: 修正后 `go test ./...` 和 `go vet ./...` 通过。

### 2026-08-11 — Go 类型化常量不能直接混用底层 byte 类型

- Symptom: 新增 `mapdata.CellAttribute` 常量时，`go test ./...` 报告 `byte` 常量不能用于 `CellAttribute` 常量声明。
- Root cause: 常量组中的底层类型被显式固定为 `byte`，随后又把它们直接赋给另一种命名类型。
- Prevention: 对领域类型常量直接使用 `CellAttribute = iota` 或显式类型转换，不保留无必要的中间类型常量。
- Verification: 修正后重新运行 `gofmt`、`go test ./...` 和 `go vet ./...`。

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

### 2026-08-11 — Go 配置解析不要在同一作用域重复声明 err

- Symptom: 新增配置加载器后，`go test ./...` 和 `go vet ./...` 在 `var err error` 处报告 `err redeclared in this block`。
- Root cause: 读取文件的短变量声明已经在当前作用域创建了 `err`，解析 INI 时又用 `var err error` 重复声明。
- Prevention: 在同一作用域复用已有错误变量；只有需要缩小作用域时才使用带初始化的局部声明。
- Verification: 修正后重新运行 `gofmt`、`go test ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 时间值进入协议结构前必须显式转换为 .NET binary

- Symptom: 存储密码接线后编译失败，`time.Time` 被赋给协议结构的 `int64 StoragePasswordLastSet`。
- Root cause: auth 层使用语义化的 `time.Time`，而 Crystal wire payload 使用 `DateTime.ToBinary()` 的 64 位值，边界转换遗漏。
- Prevention: 领域层保留 `time.Time`，进入协议结构或 payload 时统一调用 `protocol.DotNetDateTimeBinary`，并为零时间保留 .NET `DateTime.MinValue` 的编码。
- Verification: 修正后重新运行完整 Go 测试、`go vet`、race 测试和构建。

### 2026-08-11 — Go 复合字面量中不能插入局部语句

- Symptom: 角色运行时字段接入后，`gofmt`/编译在 `UserInfo{}` 内的 `hp, mp :=` 处报告缺少逗号和非法语法。
- Root cause: 局部变量声明和条件语句被放进了结构体复合字面量；复合字面量只能包含字段表达式。
- Prevention: 先在字面量外完成默认值和派生值计算，再把最终变量作为字段值传入。
- Verification: 修正后重新运行 `gofmt`、`go test ./...`、`go vet ./...` 和 `git diff --check`。

### 2026-08-11 — 通过脚本封装 apply_patch 时必须处理 Markdown 反引号

- Symptom: 本轮更新 NPCAction 模型时，包含 JSON struct tag 的 patch 又因使用 JavaScript 模板字符串承载反引号而未执行。
- Root cause: 预防规则只在新增 lessons 时落实，代码 patch 仍沿用了模板字符串；工具封装层和源码问题没有区分处理。
- Prevention: 所有包含 struct tag、Markdown 或 raw-string 内容的 patch 都禁止模板字符串；统一用普通字符串数组拼接，或拆成不含反引号的稳定锚点 patch，并在工具返回后检查目标文件。
- Verification: 改用稳定锚点 patch 后 NPC 消息动作实现成功，parser/端到端定向测试、Go 全量 race/vet/build、文档差异检查均通过。

### 2026-08-11 — .NET 工具必须单独标注未编译验证

- Symptom: 新增 `Crystal.LegacyWorldExport` 或修改 `Crystal.ProtocolProbe` 后尝试执行 .NET 构建，当前环境没有 `dotnet`，命令输出 `dotnet unavailable`。
- Root cause: 运行环境只具备 Go 工具链，不能把 .NET 项目静态检查当成真实编译验证。
- Prevention: 提交前先探测 `dotnet`/`csc`/`mcs`；若均不可用，记录 exporter 与 probe 的未验证边界，并在有 .NET 8 SDK 的环境补跑两者及现有客户端探针。
- Verification: Go 的 `test`、`race`、`vet`、`build` 与差异检查通过；.NET exporter 和 ProtocolProbe 保留为待 SDK 环境验证项。

### 2026-08-11 — 地图格式修正必须同步 fixture 的真实记录步长

- Symptom: 按原版 `LoadMapCellsV100` 修正 v100 为 27 字节单元后，旧测试仍把 fishing 字段写在 30 字节偏移，地图测试失败。
- Root cause: fixture 是按迁移代码的旧猜测构造的，没有与 .NET 字段偏移和记录步长绑定。
- Prevention: 每种地图格式先从原版 loader 固定 header、字段偏移、记录步长，再生成最小 fixture；修正偏移时同时更新正向和截断数据测试。
- Verification: fixture 修正后重新运行 mapdata 全量测试，并继续执行 Go 全量测试、race、vet、build 和 diff 检查。

### 2026-08-11 — 传送包必须按目标类逐字段对照而非复用相似包

- Symptom: 编写 NPC 传送的 MapChanged payload 初稿时误用了 MapInformation 的天气标志布局，并遗漏了目标类末尾字段；差异审查在测试前发现。
- Root cause: 两个包都包含地图元数据，但 .NET MapChanged 的字段顺序是 Lights + Location + Direction + MapDarkLight + Music + Weather，不能直接套用 MapInformation 的 flags。
- Prevention: 为每个新包从原版 packet 类的 WritePacket 逐字段列出 payload 表，再实现独立 serializer；不要以名称或相邻包推断布局，并为长度、字符串、尾字段和端到端包序列各加断言。
- Verification: 修正后 TestNPCTransportAndTeleportPayloadsMatchLegacyLayout 与 NPC 传送 net.Pipe 测试通过，随后 go test -race ./... 通过。

### 2026-08-11 — NPC 脚本 fixture 必须保留真实换行边界

- Symptom: 构造脚本 JSON/net.Pipe fixture 时曾把换行转义成字面量反斜杠，解析器会把多个 page 当成一行。
- Root cause: 测试字符串经过工具封装层二次转义，未在写入后检查 Go 源码中的实际转义层级。
- Prevention: 脚本 fixture 统一使用 Go 字符串的单层反斜杠 n，并在测试中断言 page 数量、文本顺序和按钮目标，不只断言请求成功。
- Verification: 增加 TestParseNPCScriptTextAndButtons 和脚本端到端 page 测试；普通/race 全量 Go 测试通过。

### 2026-08-11 — Packet ordinal 必须从完整 enum 锁定

- Symptom: 对照怪物 packet 时发现已有 Go 的 NPC、Player、Storage 等常量因遗漏原版 enum 成员而整体偏移，测试虽然通过但真实客户端会把 packet 解释成别的类型。
- Root cause: 只按相邻名称或记忆手写 ordinal，没有从 `Shared/Enums.cs` 的完整 `ServerPacketIds`/`ClientPacketIds` 顺序核对。
- Prevention: 每条新增 packet 先从完整 enum 计算 ordinal；Go 协议包测试必须包含关键 ID 的显式 legacy ordinal 断言，禁止只使用 Go 常量自洽测试。
- Verification: 修正 `ObjectPlayer`、`NewMonsterInfo`、`NewNPCInfo`、`ObjectMonster`、`ObjectNPC`、NPC/Storage 以及 Monster/NPC 请求 IDs，并通过 `TestPacketIDsMatchLegacyEnums` 与全量 Go 测试。

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

### 2026-08-11 — 复合协议包必须覆盖外层字段

- Symptom: 地图信息测试能正确解析标题和地图内容，但对照 `NewMapInfo.WritePacket` 时发现 Go payload 遗漏了开头的 `MapIndex`；真实客户端会从错误的偏移读取标题。
- Root cause: 只验证了复用的 `ClientMapInfo.Save` 内容，没有把外层 packet 类自己的字段纳入 payload 对照。
- Prevention: 每个复合 packet 先分别列出 packet 外层字段和嵌套对象字段；测试从第一个字节开始解析完整 payload，并断言外层索引、长度和嵌套字段。
- Verification: `NewMapInfoPayload` 现在先写入 `MapIndex`，协议、world 和 net.Pipe 测试均从完整 payload 解析并通过。

### 2026-08-11 — 地图切换必须持久化 CurrentMapIndex

- Symptom: 跨地图后只更新角色坐标，logout 再登录时坐标会被解释为旧地图坐标，角色回到错误地图。
- Root cause: 原有 `UpdateCharacterRuntime` API 只接受 x/y/direction，没有把地图索引作为同一份运行时位置状态保存。
- Prevention: 所有会改变地图的路径使用 map-aware runtime 更新接口；普通坐标更新接口保留给不改变地图的旧调用，并为 map index 单独断言。
- Verification: 增加 `TestUpdateCharacterMapRuntimePersistsMapIndex`，地图 movement net.Pipe 测试使用新接口更新运行时状态，Go 全量测试通过。

### 2026-08-11 — 地图切换可见性必须双向刷新

- Symptom: 传送后的玩家会被发送给目标地图观察者，但当前玩家没有收到目标地图已有玩家对象。
- Root cause: 只实现了旧地图 `ObjectRemove` 和当前玩家向新地图广播，遗漏了目标地图已有对象的 `GetObjects` 对等行为。
- Prevention: 地图切换验收同时断言旧观察者收到移除、新观察者收到当前玩家对象、当前玩家收到新地图已有玩家对象，并在静态对象刷新前完成玩家对象同步。
- Verification: transition world 状态测试锁定 old/new map 集合，net.Pipe 路径发送双向 player packet；全量 Go 测试通过。

### 2026-08-11 — 战斗 transcript 期望必须先算伤害终态

- Symptom: 远程攻击单测把 3 点生命、4 点攻击、1 点护甲的场景期望为未击杀，实际 `damage - armour == HP` 触发了 `ObjectDied`，测试失败。
- Root cause: 测试只按“远程命中”直觉填写 `Killed` 和通知数量，没有先按当前确定性伤害公式核算 HP 终态。
- Prevention: 战斗测试先列出攻击力、护甲、有效伤害、初始/最终 HP，再决定 struck、damage、death packet 序列；通知数量必须与该终态一致。
- Verification: 修正远程 transcript 断言为四包并重新运行 Go 全量测试通过。

### 2026-08-11 — awk 枚举核对变量不要使用保留字

- Symptom: 用 awk 计算原版 packet enum ordinal 时，命令因 `in` 变量名触发语法错误而未执行。
- Root cause: `in` 是 awk 的语法关键字，不能作为普通变量名；这是核对脚本错误，不是项目源码错误。
- Prevention: 编写 awk 枚举脚本时使用 `inside` 等非保留变量名，并先用最小命令验证脚本能运行，再依赖其输出修改协议常量。
- Verification: 改用 `inside` 后成功核对 `MagicKey=57`、`Magic=58`、`NewMagic=117`、`Magic=120`、`MagicDelay=121`、`MagicCast=122`、`ObjectMagic=123`，随后以原版 enum 数值为依据实现测试。

### 2026-08-11 — apply_patch 上下文必须重新读取精确空格

- Symptom: 批量加入魔法 packet ordinal 的 patch、随后给 `world.go` 增加字段的 patch、更新迁移矩阵的 patch、本轮加入 Chat ordinal 的 patch、本轮更新地图 gate 文案的 patch，以及本轮首次加入 GainExperience ordinal 的 patch，都因实际对齐空格或换行与手写上下文不一致而未应用。
- Root cause: patch 上下文包含了脆弱的列对齐空格，未先读取目标文件的精确文本；本轮 `HealthChanged` 表格与迁移矩阵文案再次触发同一问题。
- Prevention: 修改已有表格、结构体或文档段落时先用 `rg`/`sed -n l` 核对精确上下文，再拆成以稳定字段名或单行句子为锚点的小 patch；patch 失败后不继续假设文件已变更，并在同一轮重复失败时改用更小的锚点。对代码和文档分别应用、分别检查 `git diff`。
- Verification: 本轮重新读取 `packet_test.go`、迁移矩阵和 README 后按稳定行锚点分块应用，GainExperience 协议、NPC parser、经验表和定向端到端测试通过；随后继续执行全量校验。

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

### 2026-08-11 — 经验状态抽取后必须检查残留局部变量

- Symptom: 将经验升级逻辑抽取到共享 helper 时，初版 patch 在 world 状态写回处残留了已不存在的局部变量引用，静态复查在测试前发现。
- Root cause: 用宽泛上下文替换相似代码时命中了相邻的 CHANGELEVEL 状态写回，未立即检查 helper 调用后的变量来源。
- Prevention: 抽取状态计算函数后，立即逐行核对 result 的 Level、Experience、HP、MP 写回点，并在继续扩展功能前运行 gofmt、定向编译和单元测试。
- Verification: 修正为使用计算结果的 Level 后，经验表、世界状态、NPC parser、net.Pipe transcript 和 go test ./... 全部通过。

### 2026-08-11 — 条件 NPC transcript 必须显式设置角色前置状态

- Symptom: 条件 NPC 端到端测试在第二次调用时仍收到 ELSE 文本，成功分支没有被选中。
- Root cause: fixture 只设置了金币，却忘记 CreateCharacter 的默认等级是 1；测试场景期望等级条件 LEVEL >= 2 成功。
- Prevention: 每个条件 transcript 在启动会话前显式写入并断言等级、金币、性别、职业、地图和坐标等前置状态，不依赖创建默认值。
- Verification: 显式写入等级 2 后，低金币 ELSE 和满足等级/金币 SUCCESS 两条 net.Pipe 路径均通过，且 race 全量测试通过。

### 2026-08-11 — 跨功能测试 patch 必须使用唯一行为锚点

- Symptom: 更新 FireBall 延迟/MP transcript 时，通用的 `Damage != 4` 上下文先误改了相邻的近战测试；本轮修改远程 fixture 的同名 HP 行又误改了 melee fixture。
- Root cause: 相似断言或 fixture 行跨多个测试重复出现，patch 没有把 `world.magicAttack`/`world.rangeAttack` 或测试函数名作为唯一锚点。
- Prevention: 修改相似测试时先用测试函数名和调用函数组成双重上下文；同一常量在多个 fixture 出现时必须把函数头、地图尺寸和调用一起纳入 patch；patch 后立即用 `git diff --` 检查命中位置，再运行最小定向测试。
- Verification: 恢复近战 fixture、按 `TestGameWorldRangedAttackUsesTargetPacketAndDamageTranscript` 唯一上下文重做后，远程命中/Miss 定向测试、`go test ./...`、race、vet 和 build 均通过。

### 2026-08-11 — 协议类型命名必须先避开已有 packet 常量

- Symptom: 新增客户端魔法资料结构时，Go 编译器报告 `ClientMagic (constant) is not a type` 和同名类型重复声明。
- Root cause: `internal/protocol` 已经用 `ClientMagic` 表示客户端 packet ordinal，却在同一包中再次使用该标识符声明 wire 数据类型。
- Prevention: 新增协议领域类型前先检索同包常量、函数和类型名称；packet ID 保留 legacy 名称，数据结构使用 `ClientMagicInfo` 等不冲突的明确后缀。
- Verification: 重命名后定向协议/auth/world 测试、Go 全量测试、race、vet 和 build 全部通过。

### 2026-08-11 — apply_patch 目标路径必须先验证仓库根目录

- Symptom: 新增协议测试时误把目标路径写成仓库外的相似目录，工具实际创建了一个临时文件，正确仓库没有变化。
- Root cause: 手写绝对路径时重复了目录片段，调用前没有用当前仓库根目录和目标文件存在性做交叉确认。
- Prevention: 生成 patch 前先执行 `pwd`/`git rev-parse --show-toplevel`，对新增文件逐项检查目标路径；patch 后立即检查 `git status --short` 和目标文件绝对路径，禁止凭工具成功返回推断命中正确仓库。
- Verification: 删除误创建的仓库外文件，按正确路径重新创建测试，并通过定向协议/world 测试和 diff 检查。

### 2026-08-11 — 同级迁移仓库路径必须先从实际目录发现

- Symptom: 继续 Go 迁移时先假定 Go 项目位于原仓库内的 `Crystal-Go`，本轮竞态命令又因手写成重复的 `Dropbox/Dropbox` 而未启动。
- Root cause: 跨仓库命令没有在执行前复用已核对的绝对根目录，路径依赖手写和历史命名假设。
- Prevention: 涉及多个仓库时，先发现 `go.mod` 并执行 `git rev-parse --show-toplevel`；将输出的绝对根目录作为后续所有命令的唯一 `workdir`，不手写拼接路径。
- Verification: 使用已确认的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 重跑后，格式化、Go 全量测试、竞态测试、`vet`、构建和 `git diff --check` 均通过。

### 2026-08-11 — apply_patch JavaScript 封装必须逐行校验 patch 标记

- Symptom: 通过 `functions.exec` 组装 patch 时，多次因未引用 `@@`/`*** End Patch` 或错误的数组行出现 JavaScript 语法错误；本轮 GreatFireBall/ThunderBolt 文档和测试 patch 又重复了同类失败。
- Root cause: patch 文本和 JavaScript 源码共用一层封装，工具标记没有作为字符串逐行传入；修正后仍有调用使用了未闭合或未引用的 marker。
- Prevention: 使用数组拼接 patch 时，`*** Begin/End Patch`、每个 `@@`、每个上下文行都必须是独立字符串；先完成 `const patch = [...].join("\\n")` 的语法检查，再调用工具，失败后改用更小 patch，不把工具返回当成源码状态。
- Verification: 将配置、world、远程/魔法测试和文档拆成小 patch 后，目标文件 diff 与 `git diff --check` 均正确；`go test ./...`、race、vet、build 全部通过，GreatFireBall/ThunderBolt 提交为 `46732e4`。

### 2026-08-11 — Markdown patch 必须核对目标文件和字符串行

- Symptom: 本轮文档 patch 曾因 JavaScript 数组行误写成一元 `+` 表达式而报 `NaN`/语法错误，也曾把迁移矩阵段落误作为 README 上下文而无法应用。
- Root cause: patch 封装层的字符串标记没有逐行复核，且相似文案没有在修改前用文件名和行号确认唯一目标。
- Prevention: Markdown patch 使用双引号数组逐行拼接，避免把 `+` 放在字符串外；调用前逐项检查每个 hunk 行都以空格、`+` 或 `-` 开始，新增行不得遗漏 `+`；每次只修改一个文件，先用 `nl -ba`/`sed -n l` 核对完整行，再按文件分别检查 `git diff`。
- Verification: README 与 `docs/migration-matrix.md` 已分别按精确上下文更新，`git diff --check` 和 Go 全量测试通过；本轮复发后已强化 hunk 首字符检查规则。

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

### 2026-08-11 — MagicInfo 伤害期望必须逐项代入公式

- Symptom: IceStorm 测试把 `MC=4`、`MPowerBase=12`、`PowerBase=14` 的 level-0 伤害写成 19，实际结果为 21。
- Root cause: 手算时漏加了 `round(MPowerBase/4)=3` 的完整项，沿用了另一条魔法的旧期望。
- Prevention: 每个魔法 fixture 先从当前 `MagicInfo` 记录列出基础 MC、MPower、Power、倍率、护甲和 MP cost，再逐项计算伤害与最终 HP；不同 spell 不复用相似数字。
- Verification: IceStorm 期望修正为 21、目标有效伤害 20 后，定向测试通过，随后继续执行全量验证。

### 2026-08-11 — Markdown hunk 标记复发后的机械检查

- Symptom: FireBang/IceStorm 文档 patch 再次出现新增行遗漏 `+`，并因相似段落上下文造成重复文案。
- Root cause: 仅靠人工阅读数组内容，没有在调用前校验每个 hunk 行的首字符和目标文件中的唯一锚点。
- Prevention: apply_patch 前逐行断言 hunk 行首字符属于空格、`+`、`-`，并对同一段文案执行 `rg` 计数，确认目标文件只命中一次。
- Verification: 清理迁移矩阵重复段落后，README/矩阵差异检查和 Go 全量测试通过。

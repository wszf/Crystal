# Lessons learned

Record project-specific corrections and failure-prevention patterns here.

### 2026-08-11 — NPC 条件接线必须复用现有依赖并先核对语义字段

- Symptom: 新增 NPC 地图光照条件的服务端定向测试先因使用未导入的 `fmt` 编译失败；随后按相似字段接线时发现 `MAPLIGHT` 被错误映射到地图环境光字节。
- Root cause: 接线前没有先检查主文件已有的字符串转换依赖，也没有从原版 `Envir.AdjustLights`/`MAPLIGHT` 实现确认它比较的是全局 `LightSetting` 名称。
- Prevention: 新增运行时字段前先复用当前文件已有标准库依赖；每个条件从原版 parser、context 数据源和 evaluator 三处逐字段核对，禁止按同名/相似字段推断语义。
- Verification: 改用已有 `strconv`，并按 `Dawn/Day/Evening/Night` 的原版时间规则实现；纯条件、光照规则和 net.Pipe NPC 会话定向测试均通过。

### 2026-08-11 — apply_patch hunk 的上下文行必须显式标记

- Symptom: 修改 NPC 条件测试时，第一次 patch 因未给上下文行添加空格/加号标记而被工具拒绝。
- Root cause: 手写 patch 时把普通源码行当成了未标记的 hunk 内容，没有遵守 unified diff 的上下文格式。
- Prevention: 通过工具封装 patch 前，所有保留行都以空格开头，删除行以 `-` 开头，新增行以 `+` 开头；patch 失败后先重新读取目标文件，不假设变更已经落盘。
- Verification: 按稳定锚点拆分 patch 后，四个 Go 文件的 diff、gofmt、定向测试和 `git diff --check` 均通过。

### 2026-08-11 — 会话测试 fixture 必须复用真实桥接签名

- Symptom: Craft net.Pipe 测试初版调用 UpdateCharacterItems 时少传了一个物品网格参数，并把 mapdata.NewOpen 的宽度变量以 int 传入，导致测试包无法编译。
- Root cause: 新测试 fixture 是按记忆拼接 helper 调用，没有先读取现有持久化 API 和地图构造函数的完整签名。
- Prevention: 新增跨层测试 fixture 前先用 rg/源码确认 helper 签名；对 UpdateCharacterItems 明确按 ItemInfos、Inventory、Equipment、QuestInventory 四段传参，地图尺寸在调用边界显式转换为 int32。
- Verification: 修正后 go test ./cmd/crystal-server -run 'TestCraftSession' -count=1 通过。

### 2026-08-11 — 迁移状态必须贯通原版导出器与 Go 持久化桥

- Symptom: Refine 的 Go JSON/运行时状态已经支持 `CurrentRefine`，但原版账户导出器没有输出当前强化物品和收取截止时间，跨数据库迁移会丢失进行中的强化任务。
- Root cause: 只检查了 Go 侧的 JSON bridge 和 logout persistence，没有沿着原版数据库 → .NET exporter → Go JSON → Game stage 的完整链路核对字段与时间单位。
- Prevention: 每个迁移功能都要同时检查原版持久化字段、导出器字段、Go bridge 字段和运行时恢复逻辑；单调计时器必须在导出时转换成跨进程的绝对时间，并验证零值语义。
- Verification: `Crystal.LegacyAccountExport` 已补充 Refine workbench/current item/deadline 导出；当前环境没有 .NET SDK，需在具备 SDK 的环境补跑 exporter 编译，Go 验证继续执行全量 test/race/vet/build。

### 2026-08-11 — Refine 概率断言必须逐项代入公式

- Symptom: Refine 材料测试把成功率期望写成 79，实际实现按原版公式得到 94。
- Root cause: 手算时漏加了幸运项的 5% 和基础成功率的 20%，测试断言没有逐项列出中间结果。
- Prevention: 写强化概率断言前固定列出材料、矿石、幸运、基础四项及最终减项；涉及整数除法和边界材料时逐项核算后再运行测试。
- Verification: 修正期望为 94 后，Refine 定向测试通过；后续继续执行全量 Go 测试与竞态验证。

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

### 2026-08-11 — world 通知的指针和值边界必须显式处理

- Symptom: Healing action 编译失败，`worldPlayer` 指针不能直接赋给 `worldNotification.Recipient` 的值类型字段。
- Root cause: 世界对象 map 保存 `*worldPlayer`，而通知快照刻意保存值副本；新增通知路径遗漏了显式解引用。
- Prevention: 读取对象 map 后先确认 API 需要指针还是快照值；进入通知、排序或跨锁返回值时统一使用显式 `*player` 副本，并在编译后检查接收者身份。
- Verification: 改为 `Recipient: *target` 后，Healing 定向测试、Go 全量测试、race、vet 和 build 均通过。

### 2026-08-11 — world 测试必须显式投递返回的通知

- Symptom: Healing 的 world tick 已返回健康通知，但测试中的 packet capture 为空。
- Root cause: `world.magicAttack`/`world.tick` 只构造并返回 `worldNotification`，不会自动调用 `deliverWorldNotifications`；测试只检查返回 slice，没有执行投递层。
- Prevention: 需要验证实际客户端包时，先断言 world 返回的通知 transcript，再显式调用 `deliverWorldNotifications`，最后检查 Send capture；不要把返回通知误认为已经写入连接。
- Verification: 补齐 cast/impact 两段通知投递后，目标/施法者 packet 顺序断言通过，且全量 race 测试通过。

### 2026-08-11 — functions.exec 封装 patch 前必须先校验字符串和工作目录

- Symptom: 本轮协议、auth、world 和 main 的多个 patch 曾因 JavaScript 数组中遗漏字符串引号、未引用 patch 结束标记或把加号写成数组外表达式而未执行；一次检索还因 workdir 重复目录导致进程无法创建。
- Root cause: patch 文本、JavaScript 语法和跨仓库绝对路径同时手写，没有在调用前做逐行 hunk 标记检查、JavaScript 语法检查和仓库根目录复核。
- Prevention: 组装 patch 时所有 Begin/End、@@、上下文和新增行都必须是独立字符串；调用前检查每个 hunk 行首字符属于空格、加号或减号，并复用已确认的绝对仓库根目录；工具失败后立即检查目标文件，不把失败返回当成部分成功。
- Verification: 重新按数组字符串逐行修正后，protocol/auth/world/main 修改均落在 Go 仓库，gofmt 与 protocol/auth/world 定向测试通过；后续继续执行全量检查。

### 2026-08-11 — PvP 友方规则必须保留 legacy 的严格时间和空公会语义

- Symptom: 复核攻击模式迁移时发现 Red/Brown 在 `BrownTime` 恰好到期时被 Go 判为友方；EnemyGuild 对无公会施法者也被错误判为非友方。
- Root cause: 用 `!now.Before(...)` 近似了 legacy 的严格 `Envir.Time > BrownTime`，并把 `Guild.IsEnemy(null)` 的 false 语义简化成了双方必须有公会。
- Prevention: 对照原版条件逐运算符迁移（`Before`/`After` 保持严格边界），并为 null guild、同 guild、敌对 guild、非敌对 guild 分别建立 truth table 和测试。
- Verification: 增加 BrownTime 相等时刻、敌对公会、非敌对公会和无公会 ally 测试；Go 定向测试通过，提交前继续运行 race、vet、build 和 diff 检查。

### 2026-08-11 — net.Pipe 广播必须并发消费所有接收者

- Symptom: 玩家 PvP 端到端 transcript 在服务端广播攻击和死亡包时挂起。
- Root cause: `net.Pipe` 没有缓冲；服务端向多个连接顺序写广播包时，尚未被读取的接收者会阻塞后续写入，单独消费一个连接无法推进整个广播。
- Prevention: 为每个 net.Pipe transcript 列出所有接收者和完整包序列；涉及广播时为每个连接启动并发 reader，或先建立等价的消费屏障，再等待 handler 完成。
- Verification: `TestSessionPlayerMeleePvPTranscript` 并发消费目标连接的 7 个广播包，PvP 定向测试通过；提交前继续执行 race 测试。

### 2026-08-11 — 重复测试配置必须用函数名锚定 patch

- Symptom: 修复 PvP transcript 的 timeout 时，非唯一上下文曾误改已有可见性测试的 `cfg.TimeOut`。
- Root cause: 多个测试包含相同的 `cfg.AllowStartGame`/`cfg.TimeOut` 行，patch 没有把测试函数名和 fixture 一起作为定位条件。
- Prevention: 修改重复配置时必须把完整测试函数头、关键 fixture 或调用 API 放入 patch 上下文；patch 后先看 `git diff --unified=0` 和所有同名配置行，再运行定向测试。
- Verification: 已恢复可见性测试原有 2 秒 timeout，仅保留 PvP transcript 的 30 秒 timeout，并重新通过定向测试。

### 2026-08-11 — apply_patch 上下文行首字符必须保留（强化）

- Symptom: 本轮同步 NPC 升级上限时，第一次 patch 把代码上下文行写成了没有前导空格的字符串，工具拒绝整个 hunk；第二次又因文档行未按实际完整句子定位而失败。
- Root cause: 组装 patch 数组时把 hunk 的上下文语义误当成普通源码文本，且没有先读取目标行的完整前后文；失败后还同时修改多个文件，扩大了定位范围。
- Prevention: patch 数组中每个上下文行必须显式以一个空格开头，新增/删除行分别以 `+`/`-` 开头；先用 `nl -ba` 或 `sed -n l` 读取精确行，再按单文件、单行为锚点拆 patch，失败后检查 diff 再重试。
- Verification: 拆分为代码 patch 和文档 patch 后，NPC 会话上限同步及 README/迁移矩阵措辞均正确落盘；后续用 `gofmt`、定向测试和 `git diff --check` 验证。

### 2026-08-11 — P6 跨仓库目标和 patch 上下文必须双重确认

- Symptom: 继续 P6 物品迁移时，曾因手写相似仓库路径把 patch 目标指向错误位置；本轮物品 ordinal patch 又因上下文行缺少 patch 必需的前导空格而被拒绝。
- Root cause: 跨仓库编辑没有把已确认的绝对仓库根目录和 apply_patch hunk 语法作为调用前的机械检查项。
- Prevention: 修改前先用 `git rev-parse --show-toplevel`/目标文件存在性确认仓库；patch 数组逐行检查 Begin/End、`@@` 和上下文首字符，失败后立即查看目标仓库 `git status` 与 diff。
- Verification: 物品协议测试和文档最终都落在 `Crystal.GoServer`，原仓库无误写；ordinal 定向测试、Go 全量校验和 `git diff --check` 作为提交前门禁。

### 2026-08-11 — 物品槽位 transcript 必须逐步计算交换与空位

- Symptom: `MoveItem(0→2)` 后的 P6 端到端测试把原槽 1 物品期望在槽 0，导致 logout 持久化断言失败。
- Root cause: 只看“移动到目标槽”的直觉，没有按 legacy `array[to] = array[from]`、`array[from] = oldTarget` 逐步推导后续 split 空位和 merge 槽位。
- Prevention: 物品事务测试先画出每一步的槽位、UniqueID、Count，再断言 split 产生的新槽和 merge 后的空槽；不要只按请求文字推断终态。
- Verification: 修正 transcript 后，Move/Split/Merge 的 net.Pipe 响应顺序和 JSON logout 持久化状态通过。

### 2026-08-11 — 装备条件比较必须显式统一整数类型

- Symptom: 新增装备解析时，Go 编译器拒绝 `byte` 的 `RequiredAmount` 与 `uint16` 的角色等级直接比较。
- Root cause: 协议字段保留 legacy 的 `byte` 宽度，领域角色等级使用 `uint16`，边界比较遗漏了显式转换。
- Prevention: 从 wire 读取的窄整数进入等级/计数比较前统一转换到领域类型，并在新增条件分支后立即运行定向 `go test`。
- Verification: 将候选装备等级转换为 `uint16` 后，协议与服务端装备测试通过。

### 2026-08-11 — 装备 fixture 必须填充 legacy 默认关联字段

- Symptom: 装备单测中的物品明明类型、职业和性别都匹配，却被 `WeddingRing != -1` 规则拒绝。
- Root cause: Go 零值 `WeddingRing=0` 不等于 legacy `UserItem` 构造器的默认 `-1`，测试 fixture 没有显式还原该默认值。
- Prevention: 构造现代 `StoredItem` fixture 时显式设置所有非零语义默认值，尤其是 `SoulBoundID=-1`、`WeddingRing=-1`、空 sockets/awakening 与耐久度。
- Verification: 补齐默认字段后，装备/卸下限制、属性刷新和会话 transcript 均通过。

### 2026-08-11 — nil 物品格与空物品格必须区分

- Symptom: 会话装备请求返回失败；原因是持久化的 equipment slice 是非 nil 空数组，默认容量补齐函数只对 nil 生效。
- Root cause: legacy 14 格默认语义在 Go 中由 `nil` 触发，空 slice 表示一个真实的零长度格子，不能混同为缺省值。
- Prevention: 对需要默认容量的导入数据传递 nil 或显式补齐 14/46/40 格；测试同时覆盖 nil、空 slice 和正确容量三种输入。
- Verification: 会话 fixture 改为 nil equipment/quest grids 后，Equip/Remove/Logout transcript 与持久化断言通过。

### 2026-08-11 — 装备阶段跨仓库 patch 目标复发后必须先锁定根目录

- Symptom: 在原 Crystal 仓库工作目录下首次补协议时，patch 错把目标写成 `internal/protocol/item_actions.go`，未命中同级 Go 仓库。
- Root cause: 已知 Go 仓库是同级目录，却没有在 apply_patch 调用中使用已验证的 `../Crystal.GoServer/...` 目标。
- Prevention: 每次跨仓库编辑前执行 `git rev-parse --show-toplevel`，再把同一绝对根目录转换为 apply_patch 的相对目标；工具失败后立即检查两仓库 status。
- Verification: 改用 `../Crystal.GoServer/...` 后所有装备源码、测试和文档均落在 Go 仓库，原仓库没有误写代码。

### 2026-08-11 — 装备 patch 仍需机械校验 hunk 标记

- Symptom: 插入装备会话 case 和测试循环时，JavaScript patch 封装曾因未引用 `*** End Patch`，以及上下文行缺少前导空格而整块拒绝。
- Root cause: patch 标记、上下文语义和 JavaScript 字符串层级同时手写，调用前没有逐行校验。
- Prevention: 所有 patch 行先放入已引用的字符串数组；逐行确认 Begin/End、`@@`、上下文空格和新增/删除标记，再调用工具并查看 diff。
- Verification: 重做小块 patch 后 main handler、会话测试和 `git diff --check` 均通过。

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

### 2026-08-11 — Go 与 C# 工具链必须按文件类型隔离

- Symptom: 迁移过程中曾把 C# 导出器文件交给 `gofmt`，命令报语法错误；Go 源码本身没有问题。
- Root cause: 批量格式化命令使用了过宽的路径集合，没有先按扩展名和语言工具链划分目标。
- Prevention: `gofmt` 只接收 Go 文件/Go 目录；C# 只在检测到 `dotnet`/C# 工具链后使用对应格式化或构建命令，不能用 Go 工具替代静态验证。
- Verification: 本轮只对 Go 目录执行 `gofmt`；C# exporter 仅做最小字段 diff 检查，并明确保留 .NET SDK 环境下的编译验证边界。

### 2026-08-11 — FriendlyName 清洗顺序必须保持 legacy 顺序

- Symptom: 地面物品名称清洗初稿先移除方括号再移除尾部数字，`Drop Blade7[rare]` 会错误变成 `Drop Blade`。
- Root cause: 原版 `ItemInfo.FriendlyName` 明确先执行尾部数字正则，再移除方括号；相似的两个清洗步骤不可交换。
- Prevention: 迁移字符串派生字段时保留原方法的操作顺序，并用“数字在方括号前/后”两种 fixture 做差异断言。
- Verification: Go 实现改为 trailing-digits → bracketed-text，双会话 ObjectItem transcript 与全量测试通过。

### 2026-08-11 — 系统提示文本也属于可观察协议行为

- Symptom: 地图禁止丢弃和 Owner 拒绝拾取的 Go 提示初稿语义相近但不等于原版默认本地化文本。
- Root cause: 只迁移了 Chat packet 类型，没有从 `Shared/Language.cs` 核对 `ServerTextKeys` 的默认字符串。
- Prevention: 对每个失败/提示分支同时对照 packet 类型、默认文本和参数；未迁移本地化表时先使用原版英文键值，不自行改写措辞。
- Verification: Go handler 已使用 `CanNotDrop` 和 `CannotPickupNotOwner` 的默认文本，并通过全量 Go 测试与差异检查。

### 2026-08-11 — Storage bootstrap transcript 必须消费物品定义

- Symptom: 新增 Storage net.Pipe 测试时，启动 helper 期望先收到 `MapInformation`，实际先收到 `NewItemInfo`，导致 bootstrap 断言失败。
- Root cause: 角色包含 `ItemInfos` 时，Go Game bootstrap 与 legacy 一样会在地图和用户信息前发送每个物品定义；测试直接复用了只适用于空物品目录的 helper。
- Prevention: 每个 session fixture 先列出 `ItemInfos` 数量和完整 bootstrap 顺序；含物品定义时逐个消费 `NewItemInfo`，或使用显式支持物品目录的 helper，不能按测试名称猜测首包。
- Verification: Storage store/take-back JSON transcript 改为消费物品定义后，Storage 定向测试和 Go 全量测试通过。

### 2026-08-11 — apply_patch 新增函数必须立即检查闭合与上下文空行

- Symptom: 通过 patch 插入 Storage 协议测试时，函数末尾闭合大括号遗漏，`gofmt` 在后续函数声明处报告 `expected '('`；多个小 patch 还因空行上下文未标记而被拒绝。
- Root cause: patch 数组同时承载 JavaScript 字符串、apply_patch 标记和 Go 语法，插入函数前后只核对了行为行，没有检查新增函数的完整括号和空行上下文。
- Prevention: 新增函数 patch 必须包含并核对完整 `func`/`}`；空行在 hunk 中用带上下文或新增标记显式表示；patch 返回后立即查看目标文件、运行 `gofmt`，再继续扩展功能。
- Verification: 补齐协议测试闭合后，`gofmt`、协议/auth/server 定向测试通过；继续执行完整 race/vet/build 门禁。

### 2026-08-11 — 跨仓库绝对路径必须复用已验证根目录

- Symptom: 本轮补 Trade 溢出邮件时，apply_patch 首次把 Go 仓库路径写成了重复目录，工具找不到目标文件，补丁未落地。
- Root cause: 已知 Go 仓库真实根目录为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，但调用时凭记忆手写路径，未在 patch 前交叉验证目标文件。
- Prevention: 跨仓库修改前先执行 `git rev-parse --show-toplevel` 和目标文件存在性检查；apply_patch 只使用该输出生成的绝对路径，失败后立即检查两仓库 status，禁止凭工具错误/成功推断源码状态。
- Verification: 改用已验证路径后 StoredMail、auth 持久化、Trade fallback 和测试均正确落在 Go 仓库；定向 Go 测试、竞态测试和 `git diff --check` 通过。

### 2026-08-11 — Repair ordinal 与浮点费用必须从原版逐项核算

- Symptom: Repair 初稿沿用了摘要中的客户端 ordinal `55/57`，与原版 `MagicKey=57` 冲突；会话测试的 125% 费用也曾把 `75*1.25` 写成 94。
- Root cause: 依赖二手摘要和心算，没有从完整 `ClientPacketIds` 枚举以及 C# `float` 截断规则分别建立证据和计算表。
- Prevention: 新增 packet 先从完整 `Shared/Enums.cs` 计算 ordinal 并加入显式 legacy wants；涉及金额时按原版字段类型、运算顺序、截断点和每一步余额写出 transcript，再填写断言。
- Verification: 原版核对确认 `RepairItem=54`、`SRepairItem=56`；协议 ordinal/payload 测试与普通/特殊 Repair net.Pipe transcript 通过，费用断言分别为 93 和 375。

### 2026-08-11 — 跨仓库只读检索也必须使用对应根目录

- Symptom: 本轮把原版 `Shared`/`Server` 检索路径放在 Go 仓库工作目录下，命令报路径不存在；源码没有被修改，但检查结果不完整。
- Root cause: 只确认了 Go 仓库的 workdir，没有为同一批跨仓库命令分别绑定原版 Crystal 根目录和 Go 根目录。
- Prevention: 跨仓库读写前分别执行 `git rev-parse --show-toplevel`，并让每条命令的 workdir 与目标文件所属仓库一致；不要在一个仓库 workdir 中访问另一个仓库的相对路径。
- Verification: 后续原版查询统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，Go 查询统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，并在修改前检查两个仓库的 status。
- Strengthening after recurrence: 禁止在同一条 `exec_command` 中混合两个仓库的相对路径；需要跨仓库时拆成独立调用，并在输出中保留仓库标识。

### 2026-08-11 — 商店会话 fixture 必须恢复 RentalInformation 元数据

- Symptom: NPC 商店 Sell 失败门禁测试预期 Rental `DontSell` 返回普通失败包，但测试先收到“此处不能出售该物品”的类型限制聊天包。
- Root cause: fixture 只创建了 Rental 对应的 `ItemInfo`，没有把 `RentalInformation.BindingFlags=DontSell` 挂到实际背包物品；服务端因此按 `[TYPES]` 分支处理了它。
- Prevention: 测试 legacy 物品绑定规则时分别核对 definition 级 `ItemInfo.Bind`、instance 级 `StoredItem.RentalInformation.BindingFlags` 和 NPC 的 `[TYPES]`，不要只按索引命名推断 fixture 已具备语义元数据。
- Verification: 为实际 Rental item 补齐 binding flags 后，DontSell、Rental DontSell、类型限制的 net.Pipe transcript 与全量 Go 测试通过。

### 2026-08-11 — 新协议 patch 必须以当前常量表为唯一上下文

- Symptom: TownRevive 协议 patch 把尚未存在的 `ClientChangeHero` 当作 Go 常量表上下文，apply_patch 被拒绝；目标文件没有部分修改。
- Root cause: 直接套用了原版 enum 的相邻名称，没有先读取 Go 当前已迁移常量的精确范围。
- Prevention: 新增 packet ordinal 时先从完整原版 enum 锁定数值，再从目标 Go 文件读取真实相邻标识；只使用当前存在的单行稳定锚点，patch 失败后立即检查 diff/status。
- Verification: 改用 `ServerUserStorage`、`ServerObjectRevived` 和当前存在的客户端常量作为锚点后继续实现，并在定向协议测试中同时断言 legacy ordinal。

### 2026-08-11 — TownRevive 回归验证必须检查 fixture 与仓库边界

- Symptom: PK Town 配置测试初版把换行写成字面量标记；本轮多个 `functions.exec` patch wrapper 又出现漏引号/漏 hunk 标记；一次只读检索还使用了不完整的 Go 仓库路径。
- Root cause: 测试 fixture、JavaScript patch 字符串和跨仓库 workdir 都依赖手写文本，调用前没有逐层检查实际文件内容、hunk 首字符和仓库根目录。
- Prevention: 写入后用源码读取确认 fixture 的真实转义层级；patch 先逐行检查字符串引号、`*** Begin/End Patch`、`@@` 及上下文前导空格；跨仓库命令分别使用已验证的绝对根目录，禁止混用相对路径。
- Verification: 修正 fixture 后 config/TownRevive 定向测试和 Go 全量 test/race/vet/build 通过；每次工具调用后均检查对应仓库 status/diff，未发生跨仓库误写。

### 2026-08-11 — NPC 物品堆叠终态必须逐步推导

- Symptom: 本轮 `GIVEITEM` 测试的背包终态曾两次误算，先后把 fresh stack 的协议数量、已有堆叠和空槽放置顺序混在一起。
- Root cause: 没有按每次 `AddItem` 的“先合并已有堆、再按物品类别找槽位”逐步记录操作前后状态，也没有把发送前 clone 与背包中的可变对象分开核算。
- Prevention: 物品 transcript 先列出每个 fresh item 的 UniqueID/Count、合并目标、剩余数量和最终槽位；wire clone 与库存终态分别断言，不能凭总数量推断槽位。
- Verification: 修正 `GIVEITEM/TAKEITEM` 单测和 net.Pipe 持久化断言后，定向测试与 Go 全量 test/race/vet/build 通过。

### 2026-08-11 — 跨功能 patch 必须使用唯一函数锚点

- Symptom: NPC 物品动作 patch 曾因使用相似的 `return`/`addCharacterItem` 上下文误命中 `addCharacterItem`，目标函数没有得到预期修改。
- Root cause: 相邻功能复用了相同的局部代码形状，patch 没有同时带上完整函数签名和行为调用作为唯一定位条件。
- Prevention: 修改已有 helper 时以完整函数声明、调用方和唯一行为行组成双重锚点；每次 patch 后立即查看目标文件的 `git diff`，确认没有跨函数命中。
- Verification: 误命中在测试前被发现并修正；当前 `item_transactions.go` diff 只包含 NPC 物品 helper，定向测试、全量门禁和 `git diff --check` 通过。

### 2026-08-11 — apply_patch 上下文行必须显式保留前导空格

- Symptom: 本轮修改 NPC 物品 handler 时，两次 patch 因 `log.Printf` 和函数末尾 `}` 作为上下文行却缺少 hunk 要求的前导空格而被拒绝；源码没有部分落盘。
- Root cause: JavaScript 字符串数组把源码行和 patch 语义混在一起，人工检查只看了内容，没有检查每行首字符。
- Prevention: 调用 `apply_patch` 前逐行检查上下文以一个空格开头、删除以 `-` 开头、新增以 `+` 开头；失败后先查 status/diff，再重试更小 hunk。
- Verification: 按单函数小 hunk 重做，检查到 `executeNPCItemAction` 的完整返回状态与日志均落盘；随后用 gofmt、定向测试和完整 Go 门禁验证。

### 2026-08-11 — 文档检索也必须复用已确认的仓库根目录

- Symptom: 本轮 Go 文档检索命令把工作目录手写成重复的 `me_work` 路径，进程未能启动，文档检查被延迟。
- Root cause: 只记住了仓库名称，没有复用前面 `git rev-parse --show-toplevel` 得到的绝对路径。
- Prevention: 所有跨仓库命令（包括只读的 README/文档检索）统一使用已验证的绝对根目录；命令失败后先检查路径和两仓库 status，再继续。
- Verification: 改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 后 README/迁移矩阵检查完成，未产生文件修改。

### 2026-08-11 — NPC 响应包不能作为动作执行完成屏障

- Symptom: `SET` 会话测试读到 `ServerNPCResponse` 后立即检查角色旗标，偶发看到旧值并误报迁移失败。
- Root cause: Go NPC handler 按 legacy transcript 先写响应包，再执行页面 Actions；`net.Pipe` 客户端读到响应时，服务端仍可能在执行 `SET`。
- Prevention: 会话测试在断言 NPC 动作副作用前发送并消费 `ClientKeepAlive`/`ServerKeepAlive`，把 keep-alive 返回作为当前请求处理完成的屏障。
- Verification: `SET` + `BREAK` + 下一页面 `CHECK` 会话测试改用 keep-alive 屏障后稳定通过。

### 2026-08-11 — 跨仓库并行查询必须按调用拆分

- Symptom: 本轮并行核对 NPC 技能时，其中一条 Go workdir 命令混入了原版 `Shared/Enums.cs` 路径，查询报文件不存在；源码没有被修改，但原版证据检查被打断。
- Root cause: 并行命令数组只统一设置了一个 workdir，却把两个仓库的相对路径放进同一批查询中。
- Prevention: 同一批跨仓库查询也必须按仓库拆成独立调用；Go 调用只使用 Go 相对路径，原版调用只使用 Crystal 相对路径，并在输出中保留仓库标识。
- Verification: 后续原版 enum/NPC 动作查询改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，Go 协议/运行时查询改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`。

### 2026-08-11 — NPC 技能重复判断必须以角色技能列表为准

- Symptom: Go 运行时为了兼容无持久化技能的测试玩家可能注入默认 FireBall；如果 GIVESKILL 只检查运行时 `Magics` map，会把默认技能误判为角色已经学习，或在移除时无法与持久化列表对齐。
- Root cause: 运行时施法表和角色持久化 `Info.Magics` 的职责不同；前者可能包含迁移阶段的 fallback，后者才对应原版 NPC `player.Info.Magics.Any(...)` 判断。
- Prevention: GIVESKILL/REMOVESKILL 的存在性、索引和持久化更新统一以 `SelectInfo.Magics`/`worldPlayer.Character.Magics` 为源；运行时 map 只负责施法门禁和数值。
- Verification: NPC 技能 net.Pipe 测试从持久化 FireBall 学习 ThunderBolt、按索引移除 FireBall，并同时断言 auth 与 world 快照一致。

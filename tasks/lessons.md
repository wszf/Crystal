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

- Symptom: 更新包含 Go/Markdown 代码反引号的文档时，工具调用脚本先因 JavaScript 模板字符串语法错误而未执行。
- Root cause: patch 文本被放进 JavaScript 模板字符串，未转义其中的反引号；这是工具封装层错误，不是项目源码错误。
- Prevention: 通过脚本传递 patch 前先检查文本中的反引号；优先使用不含模板语法冲突的字符串形式，或逐一转义后再提交 patch。
- Verification: 重新提交同一文档 patch 后成功应用，再运行文档差异检查和 Go 全量校验。

### 2026-08-11 — .NET 导出器必须单独标注未编译验证

- Symptom: 新增 `Crystal.LegacyWorldExport` 后尝试执行 .NET 构建，当前环境没有 `dotnet`，命令以 `dotnet unavailable` 退出。
- Root cause: 运行环境只具备 Go 工具链，不能把 .NET 项目静态检查当成真实编译验证。
- Prevention: 提交前先探测 `dotnet`/`csc`/`mcs`；若均不可用，记录未验证边界，并在有 .NET 8 SDK 的环境补跑 exporter 和现有 Probe。
- Verification: Go 的 `test`、`race`、`vet`、`build` 与差异检查通过；.NET exporter 保留为待 SDK 环境验证项。

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

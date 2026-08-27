### 2026-08-17 — DarkCaptain 检索模式不得交给 zsh 裸展开

- Symptom: 一次 Go 只读命令把未核验的 `cmd/crystal-server/*teleport*` 作为裸 shell 参数，zsh 在无匹配时退出；该调用无写入，所有输出作废。
- Root cause: 习惯性使用文件名 glob，未先列出精确文件，也未使用 `rg --glob` 让检索器处理模式。
- Prevention: 先用 `rg --files` 核对候选，或把模式作为 `rg --glob` 参数；禁止未核验 glob 交给 zsh，任何非零读取调用整体作废。
- Verification: 本次 shell 展开失败未改变文件；后续 DarkCaptain 读取只使用已存在的 Go 文件路径和 `rg --glob`。

### 2026-08-17 — Go 读取前必须核对精确文件清单

- Symptom: 一次 Go 只读检索凭记忆加入不存在的 `monsters.go`、`visibility.go`，`rg` 返回路径错误；该调用的其他输出不能作为源码证据。
- Root cause: 先按概念猜文件名再检索，没有用当前仓库的 `rg --files` 确认目标文件。
- Prevention: 读取前先在同一 Go 根目录用 `rg --files` 列出精确候选；后续命令只使用已存在的路径或让 `rg` 在已确认目录内处理 glob。任一非零读取命令整体作废。
- Verification: 本次调用未写入；后续将改用已确认的 `main.go`、`monster_ai.go` 及 `rg --files` 返回的实际文件。

### 2026-08-17 — Go 只读检索不得使用未核验 shell glob

- Symptom: 本轮 Go 读取命令把不存在的 `cmd/crystal-server/session*.go` 交给 zsh，命令在展开阶段失败；该调用的其他输出未作为证据。
- Root cause: 依赖文件名模式而没有先确认目录内容，违反了 shell 参数和当前仓库边界的逐项核验。
- Prevention: Go 检索使用 `rg --files`/`rg --glob` 让检索器处理模式，禁止让 zsh 展开未核验 glob；任一非零读取命令整体作废并重跑。
- Verification: 失败调用未写入文件；后续仅使用独立根目录核验后的 `rg --glob '*.go'` 输出。

### 2026-08-14 — 工具调用前必须复核完整绝对工作目录

- Symptom: 一次 Go 只读命令把已知根目录手工重复拼接，工具在进程启动前因目录不存在而拒绝执行，没有产生文件变化。
- Root cause: 复制路径时未重新核对完整绝对路径，依赖记忆而不是当前仓库的根目录检查。
- Prevention: 每次工具调用都使用已验证的完整 `/Users/.../Crystal.GoServer` 工作目录；切换前先单独执行 `git rev-parse --show-toplevel`，失败时不继续读取或补丁。
- Verification: 错误调用在启动阶段失败且 Go/Legacy 工作树状态未变；随后恢复正确根目录并仅采用成功调用的源码输出。

### 2026-08-14 — Shell 搜索模式中的反引号不能裸露

- Symptom: 用 `rg` 搜索 Markdown 文本时，把包含反引号的模式放进双引号，shell 尝试执行其中的 `GroupID` 并输出 `command not found`。
- Root cause: 忽略了双引号内反引号仍会触发命令替换，把文档字面量直接拼进了 shell 命令。
- Prevention: 搜索 Markdown、Go struct tag 或其他可能含反引号的文本时，整段 pattern 使用单引号；需要同时包含单引号时改用参数化调用或拆分模式，禁止把未知文本插入双引号命令。
- Verification: 后续同类 `rg` 查询使用单引号模式完成，源码和文档未被命令替换影响；提交前 `git diff --check` 与敏感文件检查继续通过。

### 2026-08-13 — `go test -run` 正则必须作为单个 shell 参数引用

- Symptom: 定向测试命令中的 `TestA|TestB` 未加引号，zsh 把竖线解析成管道，首段测试输出被后续的 `command not found` 覆盖。
- Root cause: 在构造 shell 命令时只关注 Go 的正则语义，没有同时保护 shell 元字符。
- Prevention: 所有含 `|`、`(`、`)` 或通配符的 `go test -run` 表达式统一用单引号包成一个参数；若通过参数数组组装命令，不能用无转义的空格拼接后直接交给 shell。
- Verification: 改为 `-run 'TestFishing|TestAwakening|TestRanking|TestEquipCharacterSlotItem'` 后，服务端定向测试通过。

### 2026-08-13 — 新测试 fixture 必须先检索真实 bootstrap API

- Symptom: 排行榜领域测试按记忆调用不存在的 `AddTestAccount`，导致包级编译失败。
- Root cause: 把其他项目常见的测试 helper 名称带入当前 auth 包，没有先检索现有账户 fixture；本项目公开 helper 实际是 `AddPlaintextAccount`。
- Prevention: 新测试调用服务 bootstrap/helper 前先用 `rg` 查声明和同包现有用法，复制真实签名后再写测试；包级仅编译门禁应紧跟新增 fixture。
- Verification: 排行榜 fixture 改用 `AddPlaintextAccount(id, password, nil)`，auth 定向测试恢复通过。
- Strengthening after recurrence: 即使 helper 名称已通过检索确认，也必须读取其完整函数签名，不能凭同类 `Update...` 方法习惯推断有 `bool` 返回值；本次 `UpdateCharacterMapRuntime` 是无返回值写入，测试误将其用在条件表达式导致包级编译失败。新增 fixture 后先跑最小包级编译，再扩展网络 transcript。

### 2026-08-13 — 清理 helper 的多 hunk patch 也必须逐段复读

- Symptom: 清理两个未使用 helper 时，一次多 hunk patch 因第二个函数正文与记忆中的调用顺序不一致而整体拒绝；随后把 `rg`、格式化和测试串在同一命令中，又因预期的零匹配让整条门禁提前退出。
- Root cause: 依赖先前摘要而没有在 patch 前复读每个目标函数的精确正文，并把“零匹配即成功”的检索当作普通零退出命令。
- Prevention: 删除多个独立 helper 时逐个读取、逐个 patch；需要断言无匹配的 `rg` 单独执行并按空输出验收，不能让其退出码短路后续格式化或测试。
- Verification: 两个 helper 已按实际正文分别删除，无用 `strings` import 单独清理；格式化、定向测试和全量门禁随后独立执行。
- Strengthening after recurrence: `git diff --no-index` 在文件确有差异且内容合法时也固定返回 1；不得把它与 `git diff --check` 串成一个总门禁并把预期的 1 误报为失败。新增文件检查要单独运行，并显式把“有差异”的退出码 1 转换为成功，同时保留真正的 whitespace 错误输出。

### 2026-08-13 — Schema tests must assert semantics, not gofmt alignment

- Symptom: Adding GuildSettings fields made existing expected structs stale, while a static schema guard failed only because `gofmt` changed column-alignment spaces.
- Root cause: Schema evolution was not accompanied by all aggregate fixture updates, and source-text assertions encoded incidental whitespace instead of declarations.
- Prevention: When extending persisted structs, search every composite literal and legacy-JSON fixture for expected values; normalize source whitespace (or inspect types structurally) before static declaration checks, and explicitly test omitted JSON fields retain zero-value runtime fallbacks.
- Verification: Guild settings fixtures now cover defaults, Setup.ini overrides, invalid-value fallback, round-trip values, and old-JSON zero values; both `./internal/worlddata` and `./internal/legacyworld` pass.

### 2026-08-12 — 大型多文件 patch 必须按稳定 hunk 拆分并复读

- Symptom: 一次同时修改 service/economy 的 patch 因最后一个 `removeAuctionLocked` 上下文不匹配而整体拒绝；GameShop 测试 patch 也因手写失败文案与当前源码不同而未应用。
- Root cause: 多文件 patch 依赖较早读取的长上下文，任一末端 hunk 漂移都会让整批失败；工具成功返回也不能证明每个预期字段都已落盘。
- Prevention: 每个文件或语义 hunk 单独 patch，使用函数签名/字段名等短稳定锚点；失败后立即重新读取精确上下文，成功后用 `rg`/`sed` 和 diff 复核，JavaScript 包装只用普通字符串，禁止让未定义值变成 `NaN` 参与 patch 文本。
- Verification: 拆分后 retired 状态、双 MailID、会话测试和文档均正确落盘，`git diff --check` 与全量 Go 门禁通过。
- Strengthening after recurrence: P11 收尾曾把 fishing、EquipSlotItem 和 awakening 三个独立修正放入一个 patch，最后一个 main.go hunk 漂移导致整块拒绝。即使改动属于同一审查批次，也必须按单文件、单语义提交 patch；被拒绝后先确认前面 hunk 均未应用，再逐项重做并立即跑最小定向测试。
- Second strengthening after recurrence: 终端或普通 `sed` 输出的视觉换行不代表文件中的物理行边界；长段落 patch 前先用 `sed -n l` 核对行首和续行，再选择真实存在的短锚点。文档也必须按单文件 patch，避免一个错误行首让其他文件的正确 hunk 一并回滚。本次 Conquest 文档补丁确认无部分写入后按 README、迁移矩阵分别重做，并以 diff 复核落点。
- Strengthening after third recurrence: AI=93 代码插入第一次使用了错误的函数锚点，补丁在校验阶段拒绝；新增函数前必须从当前文件重新读取精确声明，并把同一语义插入拆成可验证的单 hunk。
- Verification after third recurrence: 失败补丁没有部分写入；改用实际 `monsterAIAxeSkeletonAttackLocked` 声明后成功插入，包级编译、FlameMage 定向及全量门禁均通过。

### 2026-08-11 — apply_patch hunk 的上下文行必须显式标记

- Symptom: 修改 NPC 条件测试时，第一次 patch 因未给上下文行添加空格/加号标记而被工具拒绝。
- Root cause: 手写 patch 时把普通源码行当成了未标记的 hunk 内容，没有遵守 unified diff 的上下文格式。
- Prevention: 通过工具封装 patch 前，所有保留行都以空格开头，删除行以 `-` 开头，新增行以 `+` 开头；patch 失败后先重新读取目标文件，不假设变更已经落盘。
- Verification: 按稳定锚点拆分 patch 后，四个 Go 文件的 diff、gofmt、定向测试和 `git diff --check` 均通过。
- Strengthening after recurrence: 本批首次给 `worldMagicAction` 增加区域动作字段时，patch 数组中的 Go 原文以制表符直接开头，遗漏 unified diff 要求的上下文空格，整个 hunk 被拒绝。今后数组中每条保留源码行必须先加一个字面量空格，再接源码原有缩进；构造后逐行检查除 marker 外的首字符只能是空格、`+` 或 `-`。
- Verification after recurrence: 失败补丁未产生部分修改；区域动作结构改用带显式上下文空格的小 hunk 重做，并在后续读取和最小编译中确认字段只出现一次。

### 2026-08-11 — 通过脚本封装 apply_patch 时必须处理 Markdown 反引号

- Symptom: 本轮更新 NPCAction 模型时，包含 JSON struct tag 的 patch 又因使用 JavaScript 模板字符串承载反引号而未执行。
- Root cause: 预防规则只在新增 lessons 时落实，代码 patch 仍沿用了模板字符串；工具封装层和源码问题没有区分处理。
- Prevention: 所有包含 struct tag、Markdown 或 raw-string 内容的 patch 都禁止模板字符串；统一用普通字符串数组拼接，或拆成不含反引号的稳定锚点 patch，并在工具返回后检查目标文件。
- Verification: 改用稳定锚点 patch 后 NPC 消息动作实现成功，parser/端到端定向测试、Go 全量 race/vet/build、文档差异检查均通过。

### 2026-08-11 — apply_patch 上下文必须重新读取精确空格

- Symptom: 批量加入魔法 packet ordinal 的 patch、随后给 `world.go` 增加字段的 patch、更新迁移矩阵的 patch、本轮加入 Chat ordinal 的 patch、本轮更新地图 gate 文案的 patch，以及本轮首次加入 GainExperience ordinal 的 patch，都因实际对齐空格或换行与手写上下文不一致而未应用。
- Root cause: patch 上下文包含了脆弱的列对齐空格，未先读取目标文件的精确文本；本轮 `HealthChanged` 表格与迁移矩阵文案再次触发同一问题。
- Prevention: 修改已有表格、结构体或文档段落时先用 `rg`/`sed -n l` 核对精确上下文，再拆成以稳定字段名或单行句子为锚点的小 patch；patch 失败后不继续假设文件已变更，并在同一轮重复失败时改用更小的锚点。对代码和文档分别应用、分别检查 `git diff`。
- Verification: 本轮重新读取 `packet_test.go`、迁移矩阵和 README 后按稳定行锚点分块应用，GainExperience 协议、NPC parser、经验表和定向端到端测试通过；随后继续执行全量校验。

### 2026-08-11 — 跨功能测试 patch 必须使用唯一行为锚点

- Symptom: 更新 FireBall 延迟/MP transcript 时，通用的 `Damage != 4` 上下文先误改了相邻的近战测试；本轮修改远程 fixture 的同名 HP 行又误改了 melee fixture。
- Root cause: 相似断言或 fixture 行跨多个测试重复出现，patch 没有把 `world.magicAttack`/`world.rangeAttack` 或测试函数名作为唯一锚点。
- Prevention: 修改相似测试时先用测试函数名和调用函数组成双重上下文；同一常量在多个 fixture 出现时必须把函数头、地图尺寸和调用一起纳入 patch；patch 后立即用 `git diff --` 检查命中位置，再运行最小定向测试。
- Verification: 恢复近战 fixture、按 `TestGameWorldRangedAttackUsesTargetPacketAndDamageTranscript` 唯一上下文重做后，远程命中/Miss 定向测试、`go test ./...`、race、vet 和 build 均通过。

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
- Strengthening after recurrence: 删除 Markdown 列表项时，patch 行必须以两个连字符开头（第一个是 diff 删除标记，第二个是文档列表标记）；上下文列表项必须以空格再接连字符开头。应用前按“patch 标记 + 文档内容”两层机械检查。

### 2026-08-11 — Markdown hunk 标记复发后的机械检查

- Symptom: FireBang/IceStorm 文档 patch 再次出现新增行遗漏 `+`，并因相似段落上下文造成重复文案。
- Root cause: 仅靠人工阅读数组内容，没有在调用前校验每个 hunk 行的首字符和目标文件中的唯一锚点。
- Prevention: apply_patch 前逐行断言 hunk 行首字符属于空格、`+`、`-`，并对同一段文案执行 `rg` 计数，确认目标文件只命中一次。
- Verification: 清理迁移矩阵重复段落后，README/矩阵差异检查和 Go 全量测试通过。

### 2026-08-11 — functions.exec 封装 patch 前必须先校验字符串和工作目录

- Symptom: 本轮协议、auth、world 和 main 的多个 patch 曾因 JavaScript 数组中遗漏字符串引号、未引用 patch 结束标记或把加号写成数组外表达式而未执行；一次检索还因 workdir 重复目录导致进程无法创建。
- Root cause: patch 文本、JavaScript 语法和跨仓库绝对路径同时手写，没有在调用前做逐行 hunk 标记检查、JavaScript 语法检查和仓库根目录复核。
- Prevention: 组装 patch 时所有 Begin/End、@@、上下文和新增行都必须是独立字符串；调用前检查每个 hunk 行首字符属于空格、加号或减号，并复用已确认的绝对仓库根目录；工具失败后立即检查目标文件，不把失败返回当成部分成功。
- Verification: 重新按数组字符串逐行修正后，protocol/auth/world/main 修改均落在 Go 仓库，gofmt 与 protocol/auth/world 定向测试通过；后续继续执行全量检查。
- Strengthening after recurrence: `*** End Patch` 也必须是数组中的已引用字符串，不能落在 `.join()` 调用之后成为 JavaScript token；构造后先机械确认首项为 Begin、末项为 End，再调用工具。本次 Conquest auth 补丁在执行前被 JavaScript 解析拒绝，源码无部分修改，已改为单文件小补丁重做。

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

### 2026-08-11 — 装备 patch 仍需机械校验 hunk 标记

- Symptom: 插入装备会话 case 和测试循环时，JavaScript patch 封装曾因未引用 `*** End Patch`，以及上下文行缺少前导空格而整块拒绝。
- Root cause: patch 标记、上下文语义和 JavaScript 字符串层级同时手写，调用前没有逐行校验。
- Prevention: 所有 patch 行先放入已引用的字符串数组；逐行确认 Begin/End、`@@`、上下文空格和新增/删除标记，再调用工具并查看 diff。
- Verification: 重做小块 patch 后 main handler、会话测试和 `git diff --check` 均通过。

### 2026-08-11 — apply_patch 新增函数必须立即检查闭合与上下文空行

- Symptom: 通过 patch 插入 Storage 协议测试时，函数末尾闭合大括号遗漏，`gofmt` 在后续函数声明处报告 `expected '('`；多个小 patch 还因空行上下文未标记而被拒绝。
- Root cause: patch 数组同时承载 JavaScript 字符串、apply_patch 标记和 Go 语法，插入函数前后只核对了行为行，没有检查新增函数的完整括号和空行上下文。
- Prevention: 新增函数 patch 必须包含并核对完整 `func`/`}`；空行在 hunk 中用带上下文或新增标记显式表示；patch 返回后立即查看目标文件、运行 `gofmt`，再继续扩展功能。
- Verification: 补齐协议测试闭合后，`gofmt`、协议/auth/server 定向测试通过；继续执行完整 race/vet/build 门禁。

### 2026-08-11 — 新协议 patch 必须以当前常量表为唯一上下文

- Symptom: TownRevive 协议 patch 把尚未存在的 `ClientChangeHero` 当作 Go 常量表上下文，apply_patch 被拒绝；目标文件没有部分修改。
- Root cause: 直接套用了原版 enum 的相邻名称，没有先读取 Go 当前已迁移常量的精确范围。
- Prevention: 新增 packet ordinal 时先从完整原版 enum 锁定数值，再从目标 Go 文件读取真实相邻标识；只使用当前存在的单行稳定锚点，patch 失败后立即检查 diff/status。
- Verification: 改用 `ServerUserStorage`、`ServerObjectRevived` 和当前存在的客户端常量作为锚点后继续实现，并在定向协议测试中同时断言 legacy ordinal。

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

### 2026-08-11 — apply_patch 封装必须验证路径和闭合标记

- Symptom: 更新 lessons 时一次 patch 因目标路径重复了仓库目录而未应用。
- Root cause: JavaScript 工具封装中的绝对路径是手写的，提交前没有检查目标文件是否存在；复杂 Markdown/raw 内容也容易遗漏 End Patch 闭合标记。
- Prevention: 调用 apply_patch 前先确认绝对路径和目标文件；多行 Markdown/raw 内容使用普通字符串数组拼接，明确包含 Begin Patch/End Patch，并在返回后读取文件确认落盘。
- Verification: 修正路径后 lessons 成功追加；随后 git diff --check 通过。

### 2026-08-17 — Go 检索不得引用未存在的概念文件名

- Symptom: AI=153 对 Go 运行时辅助函数的检索误带不存在的 `cmd/crystal-server/session.go`，命令返回路径错误；没有写入，整条检索输出不能作为源码证据。
- Root cause: 按概念猜测文件名并追加到 shell 参数，没有先用 `rg --files` 确认实际文件清单。
- Prevention: 读取前先列出当前 Go 目录的精确文件；后续只使用已存在的路径或让 `rg --glob` 处理模式，任一非零读取命令整体作废并重跑。
- Verification: 重跑时只读取已核验的 `world.go`、`main.go`、`armadillo.go` 与测试文件，CreeperPlant 行为判断只采用成功调用输出。

### 2026-08-17 — Go patch 锚点必须先由当前源码核验

- Symptom: AI=155 初次 Go patch 使用了不存在的 `monsterAIAxeSkeletonProcessTargetLocked` 函数锚点，patch 失败且没有写入源码。
- Root cause: 按 Legacy 类型名推测 Go 的 dispatch 函数名，没有先读取当前 Go 文件确认精确锚点。
- Prevention: 修改前先在目标仓库独立核对 `git rev-parse --show-toplevel`，再用 `rg -n`/精确源码读取确认函数签名；每个 patch 只使用已核验的上下文，失败后先重新读取再重试。
- Verification: 失败 patch 未改变 Go 工作树；随后使用实际的 `processMonsterAICoreLocked`/`monsterAIAvengingSpiritAttackLocked` 锚点完成接入，包级编译将在行为测试前验证。

### 2026-08-17 — apply_patch 编排的结束标记必须保持 JavaScript 字符串完整

- Symptom: WoodBox 补丁的一次 `functions.exec` 编排因 JavaScript 结束标记引号不匹配而在执行前失败；没有产生文件写入。
- Root cause: 手写多行 patch 字符串时把结束标记放进了未闭合的字符串，未先检查调用脚本的语法边界。
- Prevention: 使用 `const patch = "..."` 时逐项核对开头/结尾引号，结束标记必须位于字符串外；脚本失败时整条调用作废，不把错误文本当作源码证据。
- Verification: 失败调用未启动 patch 且工作树无变化；随后使用语法完整的独立 `apply_patch` 调用完成 WoodBox 测试修改。

### 2026-08-17 — AI=167 对照读取失败时立即丢弃混合调用证据

- Symptom: AI=167 研究期间一次 Go 根目录读取命令误带 Legacy `Server/MirObjects/MonsterObject.cs` 路径，命令返回路径不存在；没有文件写入。
- Root cause: 为核对 `GetAttackPower` 时复用了上一条 Legacy 相对路径，没有把 workdir 与参数集合绑定到同一仓库。
- Prevention: 跨仓库读取先结束当前调用；每个新调用先独立执行 `git rev-parse --show-toplevel`，参数只允许当前仓库已核验路径，失败调用的其他输出一律不作为源码证据。
- Verification: 本次混合调用在读取阶段失败且两个工作树无变更；后续将分别在 Legacy 与 Go 根目录重读 AI=167 所需片段。

### 2026-08-18 — 复杂 patch 编排必须先做字符串与闭合标记检查

- Symptom: AI=175 会话测试的一次复杂 patch 因 JavaScript 字符串语法错误未执行，目标文件没有变化。
- Root cause: 多行 patch 内容与编排脚本共用一层引号/转义，未在调用前确认每个字符串和 `*** End Patch` 标记已闭合。
- Prevention: 复杂 patch 优先拆成单文件小 hunk，使用普通字符串数组或直接 patch；调用前检查字符串闭合、hunk 首字符和 Begin/End 标记，失败后先复读目标文件与状态。
- Verification: 失败调用无文件副作用；改用闭合的 patch 字符串重做后，AI=175 定向普通/race transcript 与 Go 包级回归通过。

### 2026-08-18 — AI=177 JavaScript patch 编排失败不得作为源码状态

- Symptom: 一次 FrozenKnight patch 编排因 JavaScript 字符串语法错误在执行前失败，未调用写入工具。
- Root cause: 多行 patch 与编排脚本共用引号层，未先验证字符串闭合和 Begin/End 标记。
- Prevention: 复杂 patch 优先拆为单文件小 hunk；执行前逐项检查字符串闭合、每行 hunk 标记和 `*** End Patch`，脚本失败后不据错误文本推断源码状态。
- Verification: 改用语法完整的独立 patch 后，FrozenKnight 源码与测试成功落盘，随后包级编译和定向普通/race 测试通过。

### 2026-08-18 — 矩阵 patch 必须锚定当前文件的完整段落并优先最小插入

- Symptom: FrozenMagician 矩阵首次 patch 因上下文把同一行错误拼接而拒绝；SnowYeti 矩阵 patch 又因把较长的 AI=189 段落重新拼入 hunk 而拒绝，均未产生部分写入。
- Root cause: 没有把已读取的连续原文压缩为稳定的最小插入锚点，长上下文中混入了新旧段落边界。
- Prevention: patch 前先读取目标段落的连续行；矩阵追加优先锚定下一个稳定标题（如 `## P8`）做最小插入，保持原有换行和标点；失败 patch 后重新读取确认工作树未改变。
- Verification: FrozenMagician 以连续段落重放成功，SnowYeti 以 `## P8` 最小锚点重放成功，矩阵最终只追加对应 AI 条目。

### 2026-08-18 — SnowYeti 复杂 patch 必须把动作分发归属核对到实际文件

- Symptom: SnowYeti 首次生产 patch 把 `worldMonsterAttackAction` 的动作分发 hunk 放进了 `world.go`，apply_patch 拒绝，未产生部分写入。
- Root cause: 只按数据结构定义文件推断分发位置，没有用连续上下文确认 resolver dispatch 实际位于 `monster_ai.go`。
- Prevention: 复杂 patch 前逐 hunk 用 `rg` 核对符号实际所在文件和连续上下文；路径不匹配时整条 patch 作废，不假设会部分应用。
- Verification: Go 工作树在失败后无变化；改用 `monster_ai.go` 的实际分发段重放成功，随后 SnowYeti 包级编译通过。

### 2026-08-18 — Go 复杂 patch 必须先复读精确字段空格锚点

- Symptom: AI=192 首次生产代码 patch 因 `DarkCaptainOrbAt` 附近的实际空格与手写上下文不一致而被拒绝；未产生部分写入。
- Root cause: 对结构体字段的物理格式作了近似假设，没有先读取精确锚点。
- Prevention: patch 被拒绝后立即复读目标文件的精确行，只用最小字段/分发 hunk 重试，并先确认没有部分写入再继续。
- Verification: 精确最小 hunk 成功加入 DarkWraith 字段与分发；`gofmt` 后 `go test ./cmd/crystal-server -run '^$' -count=1` 通过。

### 2026-08-18 — AI=197 patch 必须复用 gofmt 后的精确锚点

- Symptom: GlacierSnail 常量重命名和迁移矩阵插入的首次 patch 都因手写空格/上下文与当前文件不一致而被拒绝，未产生部分写入。
- Root cause: 没有在 `gofmt` 后重新读取物理行，且矩阵 hunk 使用了概括文本而非当前稳定标题。
- Prevention: patch 前复读目标文件的连续精确行；格式化后的声明按实际空格取锚点，矩阵追加优先使用稳定标题的最小 hunk；拒绝后先确认工作树未变再重试。
- Verification: 重读后两个最小 patch 均成功；包级编译、GlacierSnail 世界/认证测试及普通/race 重复回归通过。

### 2026-08-18 — AI=198 patch 必须保留 apply_patch 的精确结束标记

- Symptom: FurbolgWarrior 初稿的两次 `apply_patch` 因 `*** End Patch` 被误写进增量行而拒绝，未产生写入。
- Root cause: 组合较长 patch 时没有把 unified patch 的结束标记作为独立原文校验。
- Prevention: 失败后先确认工作树无部分写入；重试时让 `*** End Patch` 独占最后一行，再执行 `git diff --check`。
- Verification: 后续最小 patch 成功，生产文件经 `gofmt`，Go 包编译和 AI=198 定向测试通过。

### 2026-08-18 — Legacy 源码定位必须先由文件清单确认

- Symptom: 本批次一次旧版行为复核把不存在的 `Server/Objects/...` 路径传给 `rg`，命令失败，输出不能作为对照证据。
- Root cause: 根据记忆猜测旧版目录层级，没有先从当前 Legacy 根目录枚举目标文件。
- Prevention: 读取旧版 C# 前先在 Legacy 仓库独立执行 `rg --files` 并筛选实际文件名；后续只引用已返回的相对路径。路径失败时立即丢弃输出并重新定位，不把失败命令当作行为结论。
- Verification: 改用 `Server/MirObjects/{HumanObject,MonsterObject,HeroObject,MapObject}.cs` 后完成 DelayedExplosion admission、CompleteMagic、ProcessPoison、ApplyPoison 与 Attacked 的只读核对。

### 2026-08-18 — shell 查询中的反引号要避免命令替换

- Symptom: 用反引号包住循环变量给 `rg` 时，zsh 将 spell 名称当作命令执行，查询输出无效。
- Root cause: 把 shell 命令替换语法误用在变量匹配场景中。
- Prevention: 使用 `rg -q "$spell"` 或单引号固定模式；执行前检查命令正文中的反引号，避免无意触发命令替换。
- Verification: 该失败调用未修改源码；后续扫描改用固定模式和双引号变量。

### 2026-08-18 — shell 查询不要依赖可能为空的通配符

- Symptom: Legacy 查询使用未匹配的 `Shared/Packets/*.cs` 通配符时，zsh 直接报 `no matches found`，没有产生可用证据。
- Root cause: zsh 的 `nomatch` 行为会在 `rg` 启动前拒绝空 glob。
- Prevention: 查询文件集合时使用 `rg --glob '*.cs'`，或先用 `rg --files` 确认路径；不要在命令正文中放未经验证的裸通配符。
- Verification: 失败调用未修改源码；重跑查询已改用 `rg --glob '*.cs'` 并成功返回旧版定义。

### 2026-08-18 — 复杂 rg 命令必须先做最小语法校验

- Symptom: 筛选下一迁移批次时，包含嵌套引号和反引号的 `rg` 命令以 `zsh: unmatched '"'` 失败，没有产生可用的源码证据。
- Root cause: 一次性拼接了多层 shell 引号，没有先把检索拆成简单的单引号模式。
- Prevention: 先用 `rg --files` 确认文件，再用不含嵌套引号/反引号的单引号正则分步读取；命令非零退出时丢弃整个输出。
- Verification: 后续分步检索成功读取 Plague 的 Legacy 实现；本批实现没有使用失败调用的输出。

- Strengthening after recurrence: 2026-08-19 再次因矩阵检索 pattern 含反引号导致 `zsh: unmatched "`；执行前先检查命令字符串，优先使用单引号固定模式或拆成多条简单 `rg`，失败整条输出继续作废。
- Verification after strengthening: 后续矩阵、Legacy 对照和 Go 源码读取均改用单仓库、无反引号的分步命令，成功取得可用证据且没有文件变化。

### 2026-08-19 — tail 读取 lessons 必须使用当前 shell 的合法参数

- Symptom: 读取 lessons 尾部时误用了 `tail -320p`，BSD/macOS `tail` 报 `illegal option -- -320p`。
- Root cause: 把另一种 `tail` 实现的参数记法直接带入当前 zsh 环境，没有先使用 `tail -n 320` 的可移植形式。
- Prevention: 读取固定行数统一使用 `tail -n <count>`；命令失败后先修正参数再继续读取，不把失败输出当作上下文。
- Verification: 改用 `tail -n 320 tasks/lessons.md` 后成功读取文件尾部，未产生文件修改。

### 2026-08-19 — 长行文档与功能补丁必须使用小范围 apply_patch 上下文

- Symptom: DarkBody 初版代码补丁和迁移矩阵整行替换都因 `apply_patch verification failed` 未应用。
- Root cause: 补丁把包含超长 P5 行或已变化上下文的整段文本作为精确匹配，任何微小换行/字段差异都会使整个 hunk 无法定位。
- Prevention: 代码按文件/函数使用短上下文分块；迁移矩阵只在稳定的段落锚点附近插入新条目，避免替换整行长表格；每次失败后先用只读 `sed`/`rg` 重新确认上下文再重试。
- Verification: 将 DarkBody 实现拆成支持列表、magic 分支、ObjectMagic 快照和新文件四个小 hunk 后成功；矩阵改为在 Mirroring 段落后插入独立 DarkBody 条目并成功应用。

### 2026-08-20 — 路径核对不能把已知存在目录与猜测目录混在同一条读取命令

- Symptom: 为寻找迁移交接文件执行 `rg --files tasks docs`，Legacy 仓库没有 `docs` 目录，zsh/rg 返回非零。
- Root cause: 没有先确认目录存在就把猜测路径并入检索；即使命令同时打印了 `tasks` 文件，也不能把非零调用的部分输出当作证据。
- Prevention: 每次检索先对已知目录单独执行 `rg --files tasks`；跨目录查找只使用该仓库此前核实存在的目录，禁止在同一命令混入猜测路径。
- Verification: 本次失败无写入，输出已作废；随后应在 `tasks` 单目录中读取 `migration-handoff.md`，并重新检查状态。
- Strengthening after same-session recurrence: 本轮在 Go workdir 再次执行了包含不存在 `tasks` 目录的 `rg --files tasks docs`；该非零调用不能证明 `docs` 之外的任何路径。目录查询必须按仓库、按已确认目录拆开。
- Verification after strengthening: 随后 Legacy 只读取 `tasks`，Go 只读取 `docs`，两侧分别返回成功；后续文档更新与状态核对不再复用混合目录命令。

### 2026-08-20 — 只读 `rg` 组合模式失败后必须立即作废输出

- Symptom: 查询 `AttackPowerLocked` 等符号的组合 `rg` 模式括号未闭合，命令直接报正则错误。
- Root cause: 临时增加分组时没有保留已验证的最小模式，导致模式本身而非源码匹配失败。
- Prevention: 优先使用多个简单字面模式；需要组合时先单独运行最小正则校验，并将失败调用的空输出标记为无证据。
- Verification: 随后改用简单字面 `rg`/分段 `sed` 读取 Go 实现，AI=21 的生产判断与测试均基于成功读取和通过的测试结果。

### 2026-08-20 — 下一项 AI 勘察不能假设不存在的 Legacy glob

- Symptom: 定位 AI=23 工厂入口时在 Legacy workdir 使用不存在的 `Server/Objects/Monster/*.cs` glob，zsh 报 `no matches found`，没有取得源码证据。
- Root cause: 目录结构尚未由 `rg --files` 确认，直接套用了过期的路径假设。
- Prevention: Legacy 只读定位先用 `rg --files -g '*.cs'` 确认实际目录，再对已确认文件执行 `rg`/ `sed`；glob 失败输出一律作废。
- Verification: 失败命令没有启动修改或读取目标文件；随后通过 `rg --files` 确认 `Server/MirObjects/Monsters` 并成功读取 BoneFamiliar 基线。

### 2026-08-20 — 多模式 rg 必须使用显式 -e

- Symptom: AI=23 入口查询重复使用多个 `-F` 选项时，rg 将后续模式解释为路径并报文件不存在；输出仅部分有效。
- Root cause: 组合固定字符串没有采用 rg 的显式 `-e` 语法，导致查询参数边界错误。
- Prevention: 多模式固定字符串统一写成 `rg -n -e 'pattern1' -e 'pattern2' files`，失败调用的混合输出不作为完整证据。
- Verification: 随后用显式 `-e` 重跑，确认工厂 `case 23` 与 `BoneFamiliar.cs`；未把前一次错误输出用于实现判断。

### 2026-08-20 — Handoff patch 内容中的反引号也必须转义

- Symptom: AI=24 handoff 补丁的 `functions.exec` 编排因 patch 正文中的 `Value=1` 反引号未转义，执行前报 `SyntaxError: Unexpected identifier 'Value'`，handoff 未变化。
- Root cause: 只校验了外层多行模板字面量，遗漏了 patch 内容本身的模板定界符。
- Prevention: 调用 `apply_patch` 前扫描整个 JavaScript 模板字面量，正文中的每个反引号都要转义；解析失败时先核对工作树，再重试完整补丁。
- Verification: 首次调用没有进入 apply_patch 且 handoff diff 未变化；随后将用反引号全部转义的 patch 重新应用。

### 2026-08-20 — shell 查询中的 Markdown 反引号必须避免进入双引号

- Symptom: AI=26 handoff 核查把含 Markdown 反引号的 rg 模式放进双引号，zsh 报 unmatched quote，没有取得核查输出。
- Root cause: shell 会在双引号命令参数中处理反引号，查询字符串没有使用安全的单引号边界。
- Prevention: 含反引号的 rg/sed 模式统一使用单引号或拆成不含反引号的多个查询；失败命令输出不作为状态证据。
- Verification: 失败命令没有文件变化；随后用单引号模式重新核对 handoff、diff 和状态。

### 2026-08-20 — zsh 诊断变量不得使用只读系统名称

- Symptom: 一次全仓测试摘要脚本把 Go 退出码保存到 zsh 只读变量 `status`，在测试结束后赋值时报 `read-only variable`，该调用结果作废。
- Root cause: shell 变量名与 zsh 内置只读状态名冲突。
- Prevention: 退出码变量使用任务专用名称（如 `go_rc`），避免 `status`、`PWD`、`HOME` 等系统保留变量；脚本失败时不采用其前置命令输出作为证据。
- Verification: 改用 `go_rc` 后完整普通门禁取得明确 `go_rc=0`。

### 2026-08-20 — 只读/patch 调用失败的部分输出必须整体作废（TrapRock 复发）

- Symptom: TrapRock 研究期间一次 Go workdir 命令混入 Legacy `Shared/ServerPackets.cs`，另一次多文件 patch 使用仓库根下不存在的 `warrior_attack.go`；前者读取失败，后者部分 hunk 被拒绝。
- Root cause: 没有在命令生成前检查每个路径所属仓库和 `cmd/crystal-server` 包前缀，跨仓库/根路径假设污染了工具调用。
- Prevention: 每条调用只使用当前仓库路径；Go 源文件 patch 目标必须带完整 `cmd/crystal-server/...` 绝对路径；任何非零读取或 patch 校验失败都不采用同调用其他输出。
- Verification: 失败调用均未产生未预期文件；随后拆分为纯 Legacy/Go 调用并以精确路径完成 TrapRock 协议、受击和测试接线。


### 2026-08-24 — Replace-in-place heredocs must not contain patch diff markers

- Symptom: the reconstructed migration handoff heredoc accidentally prefixed new section lines with `+` copied from patch notation.
- Root cause: patch-formatted draft text was pasted into a raw file replacement without a literal-content review.
- Prevention: before sending a heredoc replacement, mechanically reject leading patch markers and prefer one `apply_patch` update when practical; always read the complete replaced file back.
- Verification: the write succeeded, the exact accidental prefixes were removed immediately, full handoff readback contained no markers, and the migration control checker plus `git diff --check` passed.

### 2026-08-24 — Source-precedence archive 零匹配必须显式接收

- Symptom: 首次 archive 精确检索在合法零匹配时仍由 `set -e` 退出 1。
- Root cause: 未在发送前声明 `rg` 退出 1 是有效答案。
- Prevention: 可能合法零匹配的检索必须显式接收退出 0/1，只把大于 1 视为错误。
- Verification: 失败调用全部作废；随后以显式 0/1 分支确认无精确匹配，并以 version-117/checkpoint 关键词取得有限命中。

### 2026-08-24 — P3 operator UI 路径必须先枚举项目根

- Symptom: P3 closure 勘察把不存在的 `Server/MirForms` 追加到已存在的 `CharacterInfo.cs` 读取命令，整次调用退出 2。
- Root cause: 依据 namespace/项目名猜了子目录，而实际独立项目根是 `Server.MirForms/`。
- Prevention: 跨项目目录定位先用仓库根 `rg --files`，下一调用的每个路径只能来自该零退出清单；任一尾部路径失败使前段输出同时作废。
- Verification: 错误调用全部丢弃；随后先枚举并确认 `Server.MirForms/Account`，再分别零退出重读 `CharacterInfo.cs` 和 operator account handlers。

### 2026-08-24 — 一次性 Go 编辑器不能使用点号前缀并掩盖中间失败

- Symptom: matrix 单行替换 helper 命名为 `.tmp_p3_matrix_edit.go`，`go run` 即使显式传入仍报告根目录无 Go 文件；后续删除命令成功又把复合 shell 的最终退出码变成零。
- Root cause: 猜测 Go tool 会接受隐藏 `.go` 文件，并且复合命令没有先启用 `set -e`，只看工具调用最终退出码。
- Prevention: 一次性 Go helper 使用非隐藏、精确自有的 `.go` 文件；复合命令第一行固定 `set -e`，程序内断言替换计数恰为一；只用 `apply_patch` 新增/删除并在新调用回读目标与 status。
- Verification: 失败调用不用于状态裁决；随后以 `tmp_p3_matrix_edit.go`、`set -e` 和两个单匹配断言重跑，目标 P3 行精确更新，临时文件已由 patch 删除且 Go status 只剩 matrix。

### 2026-08-24 — CHAR-P3-CREATE tracing 仍须逐步构造检索 argv

- Symptom: 主线程先把猜测的 `Shared/Packets` 加到已验证 Client 搜索，随后又把 `--glob` 放到 `rg --` 之后，并让预期可为空的精确测试名搜索裸退出 1；三次调用都不能作为证据。
- Root cause: 在一个命令里同时做定位、过滤和读取，发送前没有执行已存在的路径、`rg` 四段 argv、零匹配三项机械检查。
- Prevention: 新 leaf tracing 固定拆成三次：`rg --files` 定位；`rg [options] -e pattern -- verified-paths` 取行号；下一调用才读精确范围。任何“可能没有直接调用者”的检索在成形时就显式接收 0/1。
- Verification: 三次失败输出全部丢弃；随后分别从 `Shared/{ClientPackets,ServerPackets}.cs`、选项前置的测试 glob、显式 0/1 分支零退出重跑，并只采用重跑结果。

### 2026-08-24 — replace-in-place handoff 不可在一个 patch 中 delete-plus-add

- Symptom: 重建 `tasks/migration-handoff.md` 时，同一 `apply_patch` 同时 Delete/Add 同一路径，被工具拒绝。
- Root cause: 把文件整体替换错误表达为两个互斥 path operation，而不是一个 update/replace transaction。
- Prevention: 同一路径在一次 patch 中只允许一种操作；重写快照用单次 replace-in-place，失败后先独立确认无写入。
- Verification: 被拒调用未修改文件；随后单次 here-doc replace 写入、控制检查、`git diff --check` 与完整回读均退出 0。
- Strengthening during candidate repair: 一个复合调用的首个 `service_test.go` patch 因格式化上下文不匹配失败，后续 `hero_test.go` patch 仍成功；主线程没有采用混合结果，先独立回读确认 Hero 两处实际写入，再按精确物理行单独修改 service 测试。补丁事务前必须启用 `set -e` 或拆调用，不能让失败后继续执行形成隐式部分写入。
- Strengthening during 117 tracing: 读取 bridge 后追加猜测 `ReplaceAccounts` 位于 `service.go`，尾部 `rg` exit 1，整次输出作废；随后先在 `internal/auth` 定位真实 `accounts_import.go`，再以两个单独零退出调用重读 bridge 与 counter authority。已知符号也必须先定位，不能把猜测声明路径附在读取末尾。
- Strengthening after IP-order test edit: 仅凭重复正文替换 `response = p2WriteAndRead(... invalid)` 命中了第一个 validation request，而不是最后一个 blocked request，focused test 因 malformed payload 提前断线/EOF。修复时以相邻 `service.AllowNewCharacter` 和 `blockedUntil` 两个唯一语义锚点分别恢复首包、修改末包；复跑 P3 focused 退出 0。重复测试语句必须带阶段语义上下文，不能只按首个文本匹配。
- Strengthening at account-ops startup: 搜索 operator authority 时用 `rg ... | head`，零匹配的 `rg` exit 1 被尾端 `head` 掩成复合命令 exit 0。该空结果未用于裁决；随后以显式捕获 `rg` 返回码并仅接受 0/1 的单仓调用重跑。只要零匹配可能有效，禁止用下游 pipe 代替退出码分支。
- Strengthening during account-ops recovery/tests: 首次恢复再次在 Legacy 根用 `git -C` 混读 Go，主审随后又在 Legacy 读取尾部追加 Go 测试路径；两次整调用均作废并分仓重跑。Go 搜索附带未枚举的 `internal/auth/crypto.go` 也 exit 2 后整体作废。新测试猜错 `MarketStatusSold`、`Guild.Index` 与 `CreateCharacter` 的 bool 返回，均由编译门禁捕获并在读取真实声明后修正。恢复模板、读取参数和 fixture 必须由本轮枚举/声明生成，不能因语义熟悉而猜路径、字段或返回类型。
- Strengthening at admin-authority startup: 首个状态调用在 Legacy 根再次用 `git -C ../Crystal.GoServer` 混仓；该调用全部输出已作废。随后 Legacy 与 Go 的 HEAD/status 分别在各自根零退出重跑，启动文档也按完整可见的小段读取；任何恢复复合命令在发送前都必须字面拒绝 `git -C` 和对侧根。
- Strengthening at admin-authority matrix read: 为读取单个 P3 stage row 使用 `sed -n '780,790p'`，意外带入相邻 P4/P5 超长阶段行；超出授权的输出已丢弃并改用精确单行重读。矩阵长表不得用“多给几行上下文”的常规源码习惯，先由 `rg -n -F` 定位，再只读目标物理行。
- Strengthening during admin-authority implementation: 搜索可能不存在的 ChatBanned authority 时再次让裸 `rg` exit 1，整调用作废并按显式接收 0/1 重跑；随后一个只含原样 `func remoteIP` 的占位 patch 返回 Success 却没有写入 helper，立即回读定位后才用真实插入 hunk 修正并通过 focused tests。搜索和 patch 成功都必须由退出码加目标 diff/符号回读共同证明，不能把工具文案当作变更证据。
- Strengthening during admin-authority review fixes: 一个多文件 patch 在首个 file hunk 尚未闭合时直接出现下一条 `*** Update File`，解析器以 `Invalid patch hunk` 拒绝且零写入；确认 status/diff 后按文件拆成独立 patch 成功。多文件事务不仅要检查锚点，还要机械检查每个 hunk 的正文前缀与文件边界，复杂修复默认拆文件发送。
- Strengthening during final admin-authority review: 主线程在 Go 根读取 `main.go` 时又附加 Legacy `PlayerObject.cs`，整调用输出作废并按仓重跑；随后又裸跑预期可零匹配的 `cloneSelectInfo` 搜索，exit 1 后按显式 0/1 分支重跑。为 deep-snapshot 新增 `checkpoint.go` 时还先写后补 Active ownership，虽立即登记且无越界并发写，仍违反“写前冻结精确文件”。最终候选的命令文本、零匹配分支与 Active write set 均重新逐项核验；以后 reviewer 引出的 scope expansion 也必须先改 active index，再发送任何代码 patch。
- Strengthening during admin-authority handoff reconstruction: 新快照把检查器要求的 `## Verification ledger` 自拟为 `## Recovered candidate evidence`，`tasks/check-migration-control.sh` exit 1；按脚本第 97-108 行完整 heading 清单精确改名后零退出。该事件与此前 `Candidate behavior and open review` 失败共同证明语义等价标题不属于控制 schema，重写前必须从检查器逐字复制全部 heading/field。
- Strengthening during final admin-authority fixes: 一个跨 auth/main/ranking 的多文件 `apply_patch` 在 `main.go` 陈旧上下文失败，但前面的 service/import/ranking hunks 已实际落地；同一 shell 又因缺少 `set -e` 继续 gofmt/compile。主线程立即回读新增符号和 status，确认 partial write 后按文件拆 patch。随后新 observer transcript 先后因角色名、AccountID 超过 Legacy 长度门禁失败，改用真实约束内夹具后通过。多文件 patch 失败绝不代表原子零写入，且测试数据必须先过真实账户/角色约束。
- Strengthening during P3 stage-row closure: 为避免手贴超长 matrix 行使用 Perl 精确替换，但 replacement 中未转义 `@LOGIN`，Perl 将其解释为空数组并写成双空格；后置计数门禁使命令 exit 1，却不能撤回已发生的写入。随后精确回读目标行，以 `\@LOGIN` 恢复并加回 Markdown backticks。即使 shell 最终非零，原位编辑也可能已落地；Perl replacement 的 `@/$/\\` 必须转义并在写后回读。

### 2026-08-24 — Rank lifecycle tracing 必须逐字复制 rg argv 模板

- Symptom: archive 检索先把 command substitution、`||`、赋值和分号塞进同一 zsh 子 shell而 parse error；随后三次把 `--glob` 放到 `rg --` 后。将事故细节追加 active lessons 又使控制检查以 52323 > 51200 bytes 失败。
- Root cause: 仍从自然语言或旧失败命令重拼 argv，并把 leaf-specific 事故证据放入已经接近上限的 active canonical。
- Prevention: 含 glob 的检索逐字复制 `rg -n --glob '*.go' -e 'pattern' -- path`，先完成选项段再输入 `--`；复杂退出码用顶层多行分支。事故证据归档，active 只保留既有 C02/C28 canonical。
- Verification: 四次失败调用全部作废；按固定模板或顶层 `rc` 分支重跑均 exit 0。新增细节移入本 archive 后，active 恢复限额内并由控制检查复验。

### 2026-08-24 — Rank lifecycle recovery 必须先清点延迟 subagent

- Symptom: 环境列出 `01a0345f-b16b-7580-a80d-b3892c329aee`，handoff 却称无 active worker；主线程重复做 Legacy tracing并另开 reviewer。随后未列在环境中的旧 Go auditor `01a0345f-eb1b-7901-a913-a21abb43179f` 延迟返回，再暴露一项重复委派。
- Root cause: 采用陈旧 handoff 的 quiescence 声明，没有先查询环境 ID，也没有给已完成但延迟投递的通知一个去重边界。
- Prevention: 恢复先 `wait`/非中断询问环境列出的每个 ID，第一轮 wave 前短暂接收延迟通知，再按仓库/Leaf/范围去重；不得让 handoff 覆盖当前工具事实。
- Verification: 所有相关线程均确认只读、零写入、无进程并已关闭；后续精确 ownership 由主线程统一冻结。

### 2026-08-24 — Handoff 文件清单必须取最终 status 而非补丁记忆

- Symptom: rank-lifecycle handoff 初稿把曾编辑但最终净零差异的 `tasks/lessons.md` 列为 tracked unstaged，实际 `git status --short` 只有 archive、active、handoff 三文件。
- Root cause: 依据本轮操作记忆编写状态，没有用写入后最终 Git 输出逐项覆盖草稿。
- Prevention: handoff 写后必须以最终 `git status --short` 为唯一文件清单，并立即修正任何净零或新增差异。
- Verification: 已删除虚假条目；控制检查、`git diff --check` 和最终 status 再次零错误核对。

### 2026-08-24 — Rank session helper 定位不得夹带已碰巧命中的 shell glob

- Symptom: 定位 `mailSessionTestClient` 时把精确文件与未预先枚举的 `cmd/crystal-server/*mail*test.go` 放进同一命令；稍后搜索 Disconnect 用例又追加 `cmd/crystal-server/*_test.go`。glob 两次都碰巧命中且命令 exit 0，但仍不满足路径最小验证。
- Root cause: 把 shell 成功展开误当成路径已由本轮 authority 枚举。
- Prevention: 已知声明文件只传精确路径；文件族必须先独立 `rg --files`，不得为“保险”追加 shell glob。
- Verification: 两次输出均作废；随后分别只用 `mail_session_test.go` 与已验证的 `process_lifecycle_test.go` 零退出重跑并读取精确范围。

### 2026-08-25 — Rank lifecycle 测试必须先核对 typed outcome 字段

- Symptom: 新删除后重入 transcript 把 `DeleteCharacterOutcome` 猜成通用 `Result == 10`，真实 API 使用 `Status == auth.DeleteCharacterSuccess`。
- Root cause: 沿创建结果习惯推断另一个领域返回类型，没有先读取现有 ban/delete 测试。
- Prevention: 新测试写断言前先从同包既有 production test 复制 typed outcome 的字段和常量；不同 mutation 不推断对称返回面。
- Verification: 已回读 `internal/auth/p3_ban_delete_test.go`，改用真实 `Status`/typed constant，并重新 gofmt/diff check；行为测试仍待候选编译后执行。

### 2026-08-25 — Rank seed 改变既有 fixture 前必须先扩展 test ownership

- Symptom: 13-day seed使 ban/delete 的 imported-live fixture 因 `LastAccess=0` 正确不入榜；主线程先补 recent timestamp，随后才发现 `internal/auth/p3_ban_delete_test.go` 未列入精确 write set。
- Root cause: 把“只修既有测试夹具”误当成不需要 ownership 的例外。
- Prevention: production 语义引起的任何旧测试调整也属于写权限；失败归因后先更新 active exact path，再修改夹具。
- Verification: active 已补登记该 bounded regression test；改动仅增加确定性 recent `LastAccess`/`ReplaceAccountsAt`，不改变已完成 ban/delete 生产语义，并将重跑其 focused gate。

### 2026-08-25 — Rank review 仍不得在已定位声明后追加猜测路径

- Symptom: 已由 `rg` 定位 `orderedAccountKeysLocked` 在 `operator_accounts.go` 后，读取调用仍追加不存在的 `account_order.go`；随后 IsGM 搜索又附带不存在的 `admin_authority.go`。两次调用均含有效前段与路径错误尾段。
- Root cause: 把概念文件名当成“可能的补充上下文”，违反定位结果只能生成下一调用精确参数的规则。
- Prevention: 定位调用结束后，下一次读取参数逐项复制其真实输出；不得追加语义上可能存在的候选，即使以 `2>/dev/null || true` 隐藏错误。
- Verification: 两次混合输出全部作废；分别只读 `operator_accounts.go` 与 `game_master.go` 的精确范围后零退出取得证据。

### 2026-08-25 — Materialized rank 测试夹具必须显式经过 load 或 StartGame

- Symptom: 首轮 cmd focused 中四个 ranking tests 只见已登录角色或直接静默；既有 admin transcript 在 GM removal 后 overall 为空而读包超时，ban/import 与 account-operator loaded-admin fixtures 又因 live record `LastAccess=0` 得到空榜，live-grant operator test 的离线 control 也从未物化。
- Root cause: 旧测试依赖动态扫描全部 auth characters；新权威状态按 Legacy 只在 recent load、non-GM StartGame 或 level-change-on-existing-row 时物化。直接 Create+Update 不能替代生命周期。
- Prevention: ranking fixture 必须明确选择 recent `ReplaceAccountsAt` seed 或 `MaterializeCharacterRankingAt`；需要测试离线竞争者时也必须先物化。零 LastAccess 表示 DateTime.Min，不得当成 recent 默认。
- Verification: 相关 fixtures 已补物化/recent timestamp；auth/cmd/admin/ban focused、两个 account-operator regressions 与最终 cmd 整包均 exit 0。

### 2026-08-25 — Archive 新 section 必须锚定完整前一 lesson

- Symptom: 向 protocol archive 追加 rank section 时只锚定 Character-delete 的 Symptom，导致其 Root cause/Prevention/Verification 落到新 rank heading 下。
- Root cause: 在未读取完整前一 section 边界时按首个匹配行插入。
- Prevention: archive 插入前读取目标 heading 到下一 heading；新 section 只能追加在完整四字段 block 后。
- Verification: 已把 Character-delete 三字段移回原 heading，并回读确认两个 lesson 各自结构完整。

### 2026-08-25 — Matrix 多行 prose 替换必须回读完整语法句

- Symptom: rank closure 的 inventory patch exit 0，却只替换旧 continuation 的末段并额外写入第二个 `progress**`，留下“eight Complete”旧行和重复新行。
- Root cause: hunk 草稿把 Markdown 跨行强调词误当成两个独立句首，没有先把 `**scope-frozen ... progress**` 整个物理范围作为一个 replacement。
- Prevention: 修改跨行强调/表前 prose 时，先读取 heading 到下一空行；replacement 覆盖完整句子，成功后立即回读同一范围并检查旧计数、新计数和 Markdown delimiter 各唯一一次。
- Verification: 已以完整三行 hunk 删除旧计数和重复词；目标段现唯一写成 eleven children、nine Complete、two Ready，`git diff --check` exit 0。
- Strengthening during the same closure: stage-row Perl replacement succeeded, but the post-write assertion put an expected zero-match `rg` inside `set -e` command substitution, so the shell exited 1 before printing counts. The call was discarded; an independent explicit 0/1 audit proved old count 0, new count 1 and `git diff --check` 0. Post-write negative assertions must use the same explicit rc branch as discovery searches.

### 2026-08-25 — Metadata tracing 必须预声明零匹配并预算控制文档净行数

- Symptom: metadata tracing 多次把预期可零匹配的 `rg` 裸放进 `set -e`，并把未由本轮枚举验证的测试文件 shell glob 交给读取命令；相关调用即使前段有效或 glob 碰巧命中也全部作废。首次 ownership 补丁又把 active index 从 297 行增到 301 行，控制检查 exit 1。
- Root cause: 临时按自然语言拼搜索 argv，并把“shell 成功展开”误当成路径验证；控制补丁只检查语义完整，没有在写前计算新增减行净额。
- Prevention: 每次搜索在发送前声明零匹配是否有效，有效时固定使用显式 0/1 分支；文件族只用 `rg --glob`，已知文件只传精确路径。接近上限的控制文件先计算 replacement 净行数并为后续路由保留余量。
- Verification: 所有失败/未验证调用均已按显式 rc、`rg --glob` 或精确路径零退出重跑；ownership prose 压缩到 299 行，`tasks/check-migration-control.sh` 与 `git diff --check` 均恢复 exit 0 后才开放 Go 写入。
- Strengthening during `MINE-P6-RUBBLE-001` freeze: active index 原有 272 行，首次冻结补丁未先计算净增量而扩到 307 行，首轮压缩仍有 303 行，两次控制检查均如实 exit 1。Go 写权限继续关闭；必须在每次压缩前用实际 `wc -l` 计算至少留一行余量，并只在 checker/diff 零退出后允许实现。
- Strengthening during review fixes: 将 account-session disconnect callback 改成返回完成状态时，首次补丁漏写 closure 的 `bool` 返回类型，并遗漏两个既有 operator-test closure，只编译门禁如实 exit 1。该结果仅作 finding，不作行为证据；主线程先把真实调用点 `operator_accounts_test.go` 扩入 exact ownership，再修正三个 closure，随后 touched-package compile、focused repeated 与 race 全部 exit 0。
- Strengthening during metadata closure: callback API 再增加 rollback 返回值时又遗漏两个 operator-test assignment，compile 再次如实 exit 1；P2 active prose 的局部 replacement 还留下相邻 “Six are Complete / seven are Complete”，控制脚本因结构合法未捕获，最终逐段回读才发现。Matrix 三项 `perl -e` 首次又因脚本间缺分号在编译期 exit 255、确认零写入后才重跑。API arity 变化必须先列全部真实调用点；计数 prose 修改后既跑 checker 又回读完整句；多段 Perl 每段显式以分号闭合并做唯一计数。

### 2026-08-25 — Metadata compact recovery 必须拒绝混仓状态模板

- Symptom: compact 后首个恢复调用在 Legacy `workdir` 使用 `git -C` 核验 Go，且输出被截断；新增事故行又使 active lessons 达到 51421 bytes，控制检查 exit 1。
- Root cause: 复用了混仓状态模板，并把 leaf-specific 恢复事故继续追加到已接近 50 KiB 上限的 active canonical。
- Prevention: 恢复命令发送前机械拒绝 `git -C`/对侧根，双仓状态物理拆调用；事故证据进入 workflow archive，active 只保留既有 C01/C28 规则。
- Verification: 混仓与非零控制调用全部作废；两仓状态/C# 门禁已分别零退出重跑，本节归档后 active 恢复限额内并重新执行控制检查。
- Strengthening during Start/Logout archive search: 首次 `rg -l ... | sort` 在零匹配时被下游 `sort` 掩成 exit 0；该空输出未用于裁决，随后显式捕获 `rg` 返回码并仅接受 0/1 重跑，再用较宽但仍 leaf-specific 的 StartGame/LogOut 关键词定位六个文件。可为空检索禁止用排序/截断管道代替退出码分支。
- Strengthening during Start/Logout compaction handoff: durable handoff 初稿把控制脚本要求的固定标题 `## Active leaf and protected work` 改写成同义标题，导致 `tasks/check-migration-control.sh` exit 1；该复合调用全部输出作废。以后 replace-in-place handoff 必须从现有结构复制固定标题，只替换正文；回读后以控制脚本零退出重跑验证。本次已恢复精确标题并重新执行完整单仓门禁。
- Strengthening during Start/Logout runtime-persistence coverage: 在既有 re-entry transcript 中直接调用 `world.update(objectID, ...)`，却没有先从该测试自己的 StartGame `UserInfo` 提取 `objectID`，只编译门禁报 undefined。新增任何生产 seam 前必须在同一测试作用域列出 ID authority，不能借用相邻测试的同名局部变量；本次从首轮 bootstrap 回包解析 object ID 后，定向测试 `-count=10` exit 0，并锁定显式 logout 从 world snapshot 持久化最终位置。

### 2026-08-25 — P5 closure/compact recovery must reuse single-repository and exact-row gates

- Symptom: P5 closure first tried to patch only the visible leading cells of a physically long matrix row, then compact recovery again used `git -C` to mix both repositories and bundled oversized startup documents; a later matrix Active audit also put an expected zero-match `rg` under `set -e`.
- Root cause: visual Markdown cells were mistaken for physical patch lines, and the recovery/search commands were assembled as narrative convenience rather than checked against the existing C01/C02 mechanical gates.
- Prevention: replace long table rows only with full exact-line context or a unique-count Go replacer; recovery calls must be physically single-repository and size-bounded; every potentially empty search must declare and handle rc 0/1 before execution.
- Verification: failed/nonzero/mixed calls and all preceding output were discarded; both repository states and C# gates were rerun separately, the matrix audit reran with explicit 0/1 handling, and the exact P4 leaf row was synchronized independently.

### 2026-08-26 — Revelation Go trace reused unverified shell globs

- Symptom: a bounded health-producer query appended unquoted `world*.go` and `*visibility*.go` arguments; zsh happened to expand them and the compound call exited 0, but the paths were not from the current enumeration.
- Root cause: exact candidate files and a convenient file family were combined in one argv instead of using ripgrep's own `--glob` over the verified directory.
- Prevention: use only exact paths from the immediately preceding file enumeration, or use `rg --glob '*.go' -- cmd/crystal-server`; never let zsh select evidence paths.
- Verification: the complete compound result was discarded and both Revelation-runtime and bounded-health searches were rerun against the verified directory with `rg --glob '*.go'`, exit 0.

### 2026-08-26 — Revelation authority freeze used a phantom trailing hunk

- Symptom: the first Active Index patch bundled valid timestamp/authority/checklist replacements with a second deletion context that did not exist, so the whole patch was rejected.
- Root cause: the draft repeated remembered checklist text instead of ending after the exact range just reread.
- Prevention: freeze routing changes as separate physical transactions: counters, authority, ruling, then registry; each patch may use only the immediately reread range.
- Verification: the failed patch made no change; four exact-range replacements then succeeded, the Active Index is 280 lines, control check and `git diff --check` exit 0.

### 2026-08-26 — Revelation post-patch negative search omitted rc handling

- Symptom: the expected-zero audit for remaining ad-hoc `RevelationUntil.After` checks used bare `rg` and exited 1.
- Root cause: the query was drafted as a positive locator even though zero matches was the acceptance result.
- Prevention: post-patch absence checks must use the explicit rc-0/1 branch before execution, not be repaired only after a bare search fails.
- Verification: the failed call was discarded; the same exact query reran with explicit rc handling, exited 0, and reported no remaining ad-hoc player Revelation checks.

### 2026-08-26 — Revelation matrix insertion guessed table-to-evidence adjacency

- Symptom: the first discovery-evidence insertion expected the HP-drain section immediately after the P5 table, but a frozen-count and Regen-evidence block intervened, so the patch was rejected.
- Root cause: a verified table row and a separately located evidence heading were incorrectly treated as adjacent patch context.
- Prevention: locate the exact insertion boundary, read that contiguous range, then patch only that boundary; independently located anchors must never be combined into one hunk.
- Verification: the rejected patch made no change; the frozen count was updated separately, the discovery prose was inserted at the exact reread boundary, and matrix `git diff --check` exited 0.

### 2026-08-26 — Revelation refresh searches again bypassed the glob template

- Symptom: one bounded helper search placed `--glob '*.go'` after ripgrep's `--`, emitted partial matches and exited 2; the next authority search passed an unquoted `cmd/crystal-server/*.go` to zsh and produced a broad truncated dump; a later expected-zero diff query omitted explicit rc handling and exited 1.
- Root cause: the commands were assembled from natural-language order instead of the canonical ripgrep/absence templates despite repeated C02 findings.
- Prevention: copy `rg -n --glob '*.go' -e 'pattern' -- path` literally; reject `-- --glob` and every shell-expanded source glob before sending.
- Verification: all invalid outputs were discarded. The helper query reran with options before `--`; the struct was read from exact `world.go`; `ExperienceOwnerID` usages reran with ripgrep-owned globbing; and the bounded baseline diff reran with explicit rc 0/1 handling and reported `NO_MATCH`. All accepted calls exited 0.

### 2026-08-26 — Revelation integration review repeated the option-boundary failure

- Symptom: a review query again put `--glob '*.go'` after ripgrep's `--`; it printed partial matches but exited 2, so none of that output was admissible.
- Root cause: the rendered argv was not reread left-to-right against the canonical four-part template before execution.
- Prevention: after rendering every ripgrep command, mechanically verify `rg [options including --glob] -e pattern -- verified-paths`; reject any option after the delimiter.
- Verification: the entire exit-2 result was discarded and the exact query reran as `rg -n --glob '*.go' -e ... -- cmd/crystal-server`, exit 0.

### 2026-08-26 — Revelation review recovery copied the failed argv and split a coupled signature

- Symptom: the very next symbol query copied the same post-delimiter `--glob` shape and exited 2; the EnergyShield timestamp correction then changed the callee signature and ran compile before changing its sole consumer, producing an avoidable compile failure.
- Root cause: the failed search was edited in place rather than rebuilt from the canonical template, and a coupled API/consumer edit was mistaken for two independently compilable transactions.
- Prevention: rebuild failed ripgrep argv from the literal template; change a private signature and every verified consumer in one compile transaction, while still keeping unrelated behavior patches separate.
- Verification: the search reran with all options before `--`, exit 0; `world.go` now passes the operation timestamp to EnergyShield, the deterministic 1970-clock regression and existing EnergyShield test pass count-20, and touched compile exits 0.

### 2026-08-26 — Timestamp correction matched the wrong repeated helper call

- Symptom: a two-occurrence replacement intended for FlamingSword and `magicAttack` matched TwinDrake and FlamingSword instead, introducing an undefined `now` and leaving the intended magic call unchanged.
- Root cause: the patch matched repeated helper-call text without unique function context.
- Prevention: every replacement among repeated calls must include the enclosing function's unique declaration or adjacent semantic fields.
- Verification: the compile failure was discarded as behavior evidence; exact TwinDrake and `result.SelfPackets` anchors restored the current-time call only where no operation clock exists and installed explicit `now` in FlamingSword/magic, after which focused count-20 exits 0.

### 2026-08-26 — Revelation authority correction exceeded the Active Index line gate

- Symptom: adding five review-proven mixed producer files expanded `migration-active.md` to 301 lines, so the control checker failed after the patch had landed.
- Root cause: authority prose was drafted semantically without first budgeting replacement physical lines against the existing 299-line file.
- Prevention: calculate old-versus-new physical lines before every near-limit control patch and pre-wrap the replacement within the enforced limit.
- Verification: the producer list was reflowed to 300 lines without removing evidence; the control checker then exited 0.

### 2026-08-26 — Mixed-producer classification patch used stale map alignment

- Symptom: a four-hunk ledger patch failed on the final stale line, but the compound command lacked `set -e`, so a later unchanged static test passed and made the overall shell exit 0.
- Root cause: separately remembered map rows were bundled without a fresh contiguous reread, and the patch/test transaction did not fail fast.
- Prevention: reread the exact map range immediately before patching and begin every patch-plus-format/test command with `set -e`.
- Verification: diff review confirmed zero partial classification write; four exact current rows were then patched, formatted, and the static ledger exited 0.

### 2026-08-26 — Mixed-producer regression guessed the MonsterInfo field type and path

- Symptom: the new test first guessed `protocol.MonsterInfo`, then guessed `worlddata.MonsterInfo.Stats` was `protocol.ItemStats`; both compile gates failed. The declaration locator then appended a guessed `monsters.go` path after finding the real file and made that compound read exit 1.
- Root cause: the fixture literal and read path were written from analogous APIs rather than the exact declaration and locator output.
- Prevention: finish and read the symbol locator call, then use only its exact path and copy every composite field's declared type before constructing the fixture.
- Verification: the exact `internal/worlddata/world.go` declaration showed `Stats []MonsterStat`; the corrected fixture passes the mixed-producer death/order test count-20.

### 2026-08-26 — SafeZoneHealing seam search expanded every test file in zsh

- Symptom: a bounded seam query passed unquoted `cmd/crystal-server/*test.go`, so zsh expanded the package's entire test set and produced a broad result that violated the targeted-context gate despite exiting zero.
- Root cause: exact known files and a shell wildcard were mixed in one argv instead of applying ripgrep-owned filtering to the verified directory.
- Prevention: source-file families must always use `rg --glob '*test.go' ... -- cmd/crystal-server`; never append an unquoted shell glob to an otherwise exact file list.
- Verification: the broad output was discarded. The query reran against four exact verified files with ripgrep-owned `--glob '*.go'`, exited zero, and only that bounded result informed the startup-seam ruling.
- Recurrence: recovery then combined broad `SafeZone|Healing|mapInfo` patterns over three Go directories, producing 1,206 truncated lines; later reads guessed nonexistent `regen.go` and `hero_runtime.go`, and two more commands appended shell-expanded `hero*.go`/`*visibility*.go` families. Every affected call was discarded even when an earlier segment or the shell expansion exited zero.
- Strengthened prevention: split semantic locators into one exact concept at a time and use `rg -l` before line output; finish `rg --files | rg` enumeration before constructing a read argv, and reject every explicit path not copied from that result. A known directory plus an ripgrep-owned `--glob '*.go'` is the only allowed file-family form.
- Verification: exact locators established `natural_regeneration.go`, `healing_circle.go`, `conquest_effects.go`, `heroes.go`, `maps.go`, `world.go`, `main.go`, and config files before bounded range reads; zero-match `SafeZoneHealing` was rerun with an explicit accepted rc=1 branch.
- Further recurrence: a compound hero read ended with an expected-absence `rg` but no 0/1 branch, so exit 1 invalidated its earlier `sed` output; a later movement locator used many broad alternatives and emitted thousands of lines before `head`. Both calls were discarded and the hero ranges were rerun alone. Negative checks must be physically separate from positive reads, and a source-wide movement inventory belongs in a bounded auditor report rather than the main thread.
- Direct-leaf recurrence: a Go call again appended unquoted `cmd/crystal-server/*session_test.go`, so zsh expanded the broad session suite even though the visible result was short. The entire call was discarded and the production locator reran against exact `main.go` only; test-family discovery must use ripgrep-owned `--glob` in a separate call. Integration later put `--glob '*.go'` after the path operand, so ripgrep treated both tokens as missing paths and exited 2; that output was also discarded and rerun with every option before `-e` and `--`.

### 2026-08-27 — Reconstructed handoff used a near-match heading

- Symptom: `tasks/check-migration-control.sh` exited 1 after compaction recovery because the reconstructed handoff used `## Goal and control plane` instead of the required literal `## Goal and control-plane state`.
- Root cause: the handoff body was reconstructed from semantics without first copying the checker's exact heading contract.
- Prevention: when replacing a control document, preserve checker-recognized headings byte-for-byte or inspect the checker before drafting; semantic near-matches are invalid.
- Recurrence: the first rerun then stopped at the next near-match, `Legacy repository snapshot`; checking only the first reported literal was insufficient because the checker is fail-fast.
- Strengthened prevention: inspect and copy the checker's complete required-heading list before the next rerun, not just the first reported missing heading.
- Verification: all required headings and the unique Active-leaf field were aligned with the checker, then the full control check was rerun.

### 2026-08-27 — Verify ripgrep availability before archive searches

- Symptom: the required bounded lesson search failed immediately with `zsh: command not found: rg` and produced no admissible search evidence.
- Root cause: the active execution image includes ripgrep-oriented workflow rules but no `rg` binary anywhere on its configured `PATH`.
- Prevention: run `command -v rg` before the first search; when it is absent, preserve exact tracked path/glob boundaries with `git grep` rather than installing tools or treating the failed output as evidence.
- Verification: the failed output was discarded; exact `git grep` queries over `tasks/lessons.md` and `tasks/lessons-archive/**/*.md` exited 0, and only their matching sections were read.
- Recurrence: three later queries respectively combined a broad symbol list with long file reads, used a function regex broad enough to match every call, and left an expected-zero match at rc=1. All three outputs were discarded; the accepted reruns declared zero-match semantics first, used fixed full declarations, and read exact ranges separately.
- Control finding: adding that historical evidence to the already-near-limit active lesson made `check-migration-control.sh` fail at 51,473 bytes versus 51,200. The evidence was moved here, the canonical C02 rule was left concise, and the full control gate was rerun.
- Patch finding: a line-oriented Perl rewrite treated the nested `nextIDLocked()` close parenthesis as the end of two multiline constructor calls, yielding `nextIDLocked(, now)`. Automated call rewrites must not parse nested Go syntax; multiline calls now use exact `apply_patch` hunks, and `gofmt`, `git diff --check`, and touched compile verified the correction.
- Compile finding: removing BoneLord's last package-global direction draw left `math/rand` imported, so the first touched compile failed only on the unused import. Symbol-removal patches must include their import cleanup in the same transaction; after deleting the import, `gofmt`, diff check, and touched compile exited 0.
- Coupled-patch finding: Mirroring field removal was applied before its consumer because the consumer hunk guessed both a field layout and `action.MirroringMapIndex`; even the first reread was not copied literally and the second hunk repeated the wrong map expression. Both compiles failed only on the dangling field use. Coupled producer/consumer edits now copy the exact contiguous reread into one hunk; the literal `mapIndex` correction then passed gofmt, diff check, and compile.
- Test-log finding: the first package gate used `tee`, so expected server logs and a fatal-under-lock timeout were streamed back as a broad truncated tool result despite also being saved under `/tmp`. Long package gates must redirect to a bounded log, print only rc and failure headings, and read exact failure ranges separately; all accepted follow-up gates use that shape.
- Fail-fast recurrence: after `gofmt` changed test-field alignment, a stale patch hunk failed; the compound remediation command omitted `set -e`, so compile and focused tests still ran and repeated the known type error. Every patch/format/test compound call must begin with `set -e`; after any failed hunk, reread the exact physical lines before sending a new standalone patch. The corrected fail-fast call applied the explicit ID conversion, then diff check, touched compile, count-10 and race-count-3 exited 0.

### 2026-08-27 — Silent read-only reviewers provide no acceptance verdict

- Symptom: two serial bounded `luna_worker` final-diff reviewers remained
  running for extended waits, returned no partial or final report after normal
  and interrupting stop requests, and exposed no active Go process.
- Root cause: the worker execution/report channel did not reach a terminal
  response; absence of a process or workspace write cannot be promoted to a
  no-finding review result.
- Prevention: keep final review workers read-only and off the critical path;
  never interrupt a writer under this rule. After bounded long waits and an
  explicit stop/report request, close an unresponsive read-only thread, record
  that it supplied no verdict, and have main reproduce every provisional
  finding from authoritative Legacy source before acceptance.
- Verification: workers `01a0431c-5f2f-7772-a941-dcac6ef04386` and
  `01a04349-736b-72b0-ae15-892caedb6227` were closed with zero writes; main's
  terminal review found and corrected invalid-Spawn constructor order and
  restored-pet first packets, then focused/full/race/vet/build gates passed.

### 2026-08-28 — Archive search must verify ripgrep availability first

- Symptom: the ordinary-spawn startup archive search invoked `rg` twice and
  returned only `command not found`, so no keyword result from that call was
  usable.
- Root cause: the workflow assumed the prior session's ripgrep availability
  without checking the current login-shell toolchain.
- Prevention: run `command -v rg` before the first archive query in a recovered
  environment; when absent, use repository-local `find` plus quoted `grep`
  patterns rather than retrying the unavailable binary.
- Verification: `command -v rg` confirmed absence, and the bounded candidate
  file and ordinary-spawn/AI/ownerless/respawn keyword searches were rerun with
  `find`/`grep` at exit 0 before exact matching sections were read.
- Strengthening during `CHAT-P4-MAP-CONTEXT-001`: the next recovered session
  repeated the same unverified `rg` call even though the active C02 rule already
  required `command -v`; that output was discarded and the bounded archive
  search was rerun with tracked `git grep` at exit 0. Tool availability checks
  must be the first command of the workflow, not a diagnosis after failure.

### 2026-08-28 — Compile-coupled imports and consumers belong in one patch transaction

- Symptom: the CHAT map-context runtime-rule patch added the `config` import,
  then ran the compile gate before adding the chat consumer, yielding an
  expected-but-invalid unused-import failure.
- Root cause: a compile-coupled import and its only consumer were split across
  transactions despite C06 requiring signatures and consumers to compile
  together.
- Prevention: when a small patch introduces a package solely for the next
  bounded function, add the import and first consumer in one patch/gofmt/compile
  transaction; otherwise defer the import until the consumer exists.
- Verification: `chatWithContext` and its config types were added, gofmt ran,
  and `go test ./cmd/crystal-server -run '^$' -count=1` then exited 0.

### 2026-08-28 — A zero-exit focused filter must prove it selected the protected tests

- Symptom: the first current-candidate CHAT count-10/race commands exited zero
  but the anchored `TestItemShout.*` alternative matched no protected P6 tests;
  their real names begin with `TestSessionItemShout`, `TestUseItemShout`, and
  `TestWorldItemShout`.
- Root cause: the filter was composed from a feature noun instead of enumerated
  physical test names, and exit zero was mistaken for selection evidence.
- Prevention: before an acceptance gate, list the exact `func Test...` names
  and verify every intended family can match the final anchored expression;
  a passing command cannot prove an unmatched family ran.
- Verification: tracked test declarations were enumerated with `grep`; the
  corrected `Test.*ItemShout.*` command reran the full current CHAT family at
  count 10 and focused race count 5, both exit 0.

### 2026-08-28 — Reviewer model effort must follow the runtime's accepted surface

- Symptom: after two bounded `luna_worker` reviews produced no report and were
  closed, the explicit `codex-auto-review` fallback was first spawned with
  `reasoning_effort=max`; the API rejected it because that model accepts only
  through `xhigh`, despite deferred tool metadata advertising `max`.
- Root cause: the generic spawn metadata was treated as stronger than the
  selected review model's runtime validation.
- Prevention: preserve the required Luna/max split for Luna workers, but when a
  specialized fallback is explicitly justified, use that model's last confirmed
  accepted effort and preserve any rejection as non-review evidence.
- Verification: the rejected agent was closed, `codex-auto-review/xhigh` ran the
  same read-only scope, returned four findings, and its post-ruling follow-up
  returned `No findings`; all reviewer threads are closed.

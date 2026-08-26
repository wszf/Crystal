### 2026-08-17 — 跨仓库失败读取的全部输出必须作废

- Symptom: AI=155 随机边界核对的一条 Legacy 命令误带了 Go 的 `cmd/crystal-server` 路径并以路径不存在失败；没有源码写入，因此该调用中随后成功打印的 Legacy 片段也不能作证。
- Root cause: 为连续查找 Legacy `MaxLuck` 与 Go AI 常量而在同一调用复用了另一仓库的路径，违反单仓库命令边界。
- Prevention: 每个读取调用只绑定一个已核验仓库根目录和路径 allowlist；需要对照时结束当前调用，再新建调用核验另一仓库，任一混合路径或非零状态立即作废整条输出。
- Verification: 失败调用未改变两个工作树；随后已在独立 Legacy 调用确认 `MaxLuck=10`，后续 Go 读取将另行执行。

- Strengthening after recurrence: 后续 Legacy 对照命令又在尾部混入 Go 的 `cmd/crystal-server/poison.go`，即使前面的 C# 输出成功，该混合调用也必须全部作废。
- Prevention: 执行前逐项扫描命令字符串：Legacy cell 只允许 `Server/`、`Shared/`、`Client/`、`tasks/` 路径，Go cell 只允许 `cmd/`、`internal/`、`docs/`；跨仓库读取必须由两个独立 cell 串行完成。
- Verification after recurrence: 本次调用无文件写入；之后将分别重新核对 Legacy 的 C# poison 实现和 Go 的 poison helper，后续判断只采用各自成功调用的输出。

- Strengthening after recurrence: 随后一次 Go 工作目录中的读取又误带 Legacy `Server/MirObjects/HeroObject.cs`；该调用也必须整体作废，不能采用同一调用中成功输出的 Go 片段。
- Prevention: 工具调用前先把 `workdir` 与每个路径逐项标记为同一仓库；Go cell 禁止出现 `Server/`、`Shared/`、`Client/`，Legacy cell 禁止出现 `cmd/`、`internal/`、`docs/`，并在执行后检查退出码。
- Verification after recurrence: 该调用只读且未改变工作树；AI=155 当前实现与测试不依赖该失败输出，后续只使用已完成的独立 Go 测试结果。

### 2026-08-17 — Legacy 对照命令不得夹带 Go 路径（AI=157）

- Symptom: 对照 AxePlant 时在 Legacy 根目录的 shell 命令误带 Go 的 `cmd/crystal-server/*.go` 路径，zsh 因未匹配 glob 失败；该调用没有写入，整条输出不能作为源码证据。
- Root cause: 为连续读取两侧实现，复用了另一仓库的相对路径，没有把每次调用的工作目录和参数限制为同一仓库。
- Prevention: 每次 Legacy/Go 对照拆成独立调用；Legacy 命令只允许 `Server/`、`Shared/`、`Client/`、`tasks/` 等已核验路径，Go 命令只允许 Go 仓库路径。失败的读取调用整体作废，不采用其余输出。
- Verification: 失败调用发生在 zsh glob 展开阶段且未产生文件变化；之后在独立核验的两个仓库中分别重新读取 AxePlant 基线和 Go 实现，AI=157 定向测试通过。

### 2026-08-17 — Go 命令的仓库根目录必须直接复用核验结果

- Symptom: 一次 BlueSoul Go 命令误用了重复目录 `Crystal.GoServer.GoServer`，进程未启动；该调用没有产生源码证据。
- Root cause: 复制绝对工作目录时凭记忆再次拼接 Go 仓库名，没有直接使用最近一次 `git rev-parse --show-toplevel` 返回值。
- Prevention: 每次 Go 工具调用先在独立调用中核验 `git rev-parse --show-toplevel`，随后 `workdir` 只使用该返回值，命令参数只允许当前 Go 仓库路径；进程启动失败时整条调用作废。
- Verification: 失败调用未写入文件；BlueSoul 的矩阵、格式化和测试均在重新核验的 `Crystal.GoServer` 根目录完成。

### 2026-08-17 — SackWarrior 对照调用必须隔离仓库并核验文件

- Symptom: 定位 SackWarrior 时，一条 Go 命令带入不存在的 `cmd/crystal-server/black_foxman.go`，另一条 Go 命令带入 Legacy 的 `Server/MirObjects/MapObject.cs`；两条调用均失败，输出不能作为源码证据。
- Root cause: 凭概念猜 Go 文件名，并在 Go 对照中复用 Legacy 路径，没有先按当前仓库文件清单和根目录建立参数边界。
- Prevention: 每次调用先独立核验 `git rev-parse --show-toplevel`；当前仓库先用 `rg --files` 取得精确路径，命令中禁止出现另一仓库的 `Server/` 或 `cmd/` 路径；任一非零读取调用整体作废。
- Verification: 之后分别在核验后的 Legacy 与 Go 根目录读取成功，SackWarrior 实现只采用成功调用证据；失败调用未产生写入。

### 2026-08-17 — KingHydrax 对照读取不得混入 Go 路径

- Symptom: KingHydrax Legacy 对照命令末尾误带 Go 的 `internal/legacyworld/settings.go`，因当前仓库不存在该路径退出；命令没有写入，整条输出不能作为源码证据。
- Root cause: 为连续读取 C# 设置和 Go 导入器，复用了另一仓库的相对路径，没有把工作目录与全部参数作为单仓库调用边界。
- Prevention: 跨仓库对照拆成独立调用；每次切换先核对 `git rev-parse --show-toplevel`，Legacy 命令只使用已核验的 `Server/`、`Shared/`、`tasks/` 路径，Go 路径必须在新的 Go 调用读取；任一非零读取调用整体作废。
- Verification: 该调用只读、未产生 C# 或 Go 变化；后续 KingHydrax 设置与实现证据将分别在两个已核验根目录重读。

### 2026-08-17 — 跨仓库同一工具调用的隔离规则复发后必须强化

- Symptom: 本批恢复前的状态/对照流程再次出现把 Legacy 与 Go 查询放进同一工具编排的情况；虽然只读，但不能由单次结果独立证明仓库边界。
- Root cause: 为降低往返延迟，把两个仓库的状态、源码和矩阵查询当作可并行的独立任务，忽略了每个工具调用必须绑定唯一根目录的项目约束。
- Prevention: Legacy lessons/C# 状态、Go 源码/测试/矩阵及各自 Git 操作全部拆成独立调用；每次先核对当前仓库 `git rev-parse --show-toplevel`，参数只允许当前仓库路径，禁止在 shell、Promise 或路径变量中混放两侧。
- Verification: 混合调用的输出不再作为证据；本批后续 Legacy 与 Go 的读取、补丁、测试和 C# 零差异检查均按独立根目录串行完成。

### 2026-08-17 — 跨仓库状态读取再次不得放入同一编排

- Symptom: 本轮恢复时把 Legacy lessons/status 与 Go lessons/status 查询放入同一个 `Promise.all`；Go 的不存在文件错误使该读取调用的全部输出不能作为证据，且违反了仓库隔离边界。
- Root cause: 为了同时恢复上下文，把两个仓库的只读调用当成互不相关的任务；没有让一次工具编排只绑定一个已核验仓库根目录。
- Prevention: 跨仓库的 lessons、根目录、状态、源码、测试和写入都必须拆成独立调用；先在当前仓库核对 `git rev-parse --show-toplevel` 与文件清单，再使用该仓库路径，失败调用整体作废。
- Verification: 本次调用没有写入源码；随后已在独立 Go 调用中核对 `Crystal.GoServer` 根目录及 lessons 文件清单，后续 KingHydrax 操作按仓库逐次执行。

### 2026-08-17 — Legacy 读取命令不得追加 Go 检索路径（再次强化）

- Symptom: 本轮重读 Legacy `CanFly` 时在命令末尾追加了 Go 的 `cmd/crystal-server` 检索路径；当前仓库不存在该路径，读取调用失败，因此整条输出作废。
- Root cause: 读取 C# 基线后试图在同一命令继续确认 Go helper，未把跨仓库切换当作新的调用边界。
- Prevention: Legacy 调用的参数只允许当前仓库已核验的 `Server/`、`Shared/`、`Client/`、`tasks/` 路径；Go 侧必须另起调用并先核对根目录。任一非零读取调用整体不得作为证据。
- Verification: 该调用只读且未改变文件；后续已拆分为纯 Legacy 重读，再切换到独立 Go 调用。

### 2026-08-17 — Go patch 锚点与仓库路径必须在调用前核验

- Symptom: KingHydrax 会话与迁移矩阵补丁曾使用不存在的上下文锚点而被 `apply_patch` 拒绝；此前同批还出现过把 Go 目标路径写错到仓库根目录的失败尝试，均未写入。
- Root cause: 依据记忆拼接文件/函数位置，没有在补丁前重新读取当前文件的精确上下文，并把跨仓库路径边界当成可复用字符串。
- Prevention: 每次补丁先独立核对当前仓库的 `git rev-parse --show-toplevel`，再用 `rg -n`/`sed` 取得实际锚点；绝对目标路径逐项确认存在，失败补丁的其他输出不得作为证据。
- Verification: 失败补丁均无文件变化；随后使用当前 `king_hydrax_session_test.go` 与 `docs/migration-matrix.md` 的精确锚点完成修改，格式化及 KingHydrax 普通/race 定向测试通过。

### 2026-08-17 — AI=163 Go 读取命令不得追加 Legacy 路径

- Symptom: HornedMage 对照期间一次 Go 读取命令末尾误带 Legacy `Server/...` 路径，读取返回非零；该调用的 Go 输出也不能作为源码证据。
- Root cause: 为连续比较两侧实现而复用了上一仓库的相对路径，没有把 `workdir` 与全部路径参数作为单仓库契约检查。
- Prevention: 每个读取调用先独立核对 `git rev-parse --show-toplevel`，命令参数只允许当前仓库已确认存在的路径；任一混合路径或非零读取结果整体作废并在新调用重跑。
- Verification: 失败调用未产生写入；HornedMage 的 Legacy/Go 证据随后分别在核验后的根目录重读，后续实现未使用混合输出。

### 2026-08-17 — AI=164 对照读取继续保持两个仓库完全隔离

- Symptom: HornedArcher 对照期间曾把 Go 路径与 Legacy 路径放进同一读取编排；调用未产生写入，但混合调用的输出不能作为源码证据。
- Root cause: 为连续查看继承 AI、Buff 和 Go 运行时 helper，复用了另一仓库的路径参数，没有把一次工具调用绑定到单一已核验根目录。
- Prevention: 先在当前仓库独立核对 `git rev-parse --show-toplevel` 和文件清单；该调用只读取当前仓库。切换另一仓库必须新建调用并重新核对，失败调用整体作废。
- Verification: 本次混合读取没有写入 C# 或 Go 文件；后续 HornedArcher Legacy 基线与 Go 实现分别在独立根目录调用中重读。

### 2026-08-17 — AI=166 跨仓库读取与提交必须继续完全隔离

- Symptom: 本批次需要同时更新 Go 矩阵和 Legacy lessons；若在同一 shell/编排中混入两侧路径，任一失败会使另一侧的只读输出失去证据边界。
- Root cause: 将“两个仓库都只是迁移记录”误认为可以共享一次工具调用，忽略了本项目将 C# 基线和 Go 实现定义为独立仓库。
- Prevention: Legacy lessons、C# 状态检查、Go 源码/测试/矩阵和 Git 提交全部使用独立调用；每次先核对本次 workdir 的 `git rev-parse --show-toplevel`，参数只包含当前仓库路径。
- Verification: 本批次 Legacy 仅新增 lessons，Go 仅修改 AI=166 实现/测试/矩阵；后续分别执行 C# 差异与未跟踪检查确认无 C# 变化。

### 2026-08-17 — 跨仓库状态检查也必须按调用隔离

- Symptom: 本轮恢复时曾把 Legacy 与 Go 的 status/diff 查询放进同一个 `Promise.all`；命令只读且没有文件变化，但违反了迁移仓库的证据边界。
- Root cause: 为减少工具往返，把“查询互不写入”错误等同于“可以共享一次编排”，没有让每个工具调用绑定单一仓库根目录。
- Prevention: Legacy 与 Go 的根目录核验、状态检查、源码读取、测试、格式化和提交都分别启动调用；一次调用的 `workdir` 与路径参数只允许来自同一仓库，任何混合输出不作为实现证据。
- Verification: 本次并行查询未产生文件变化；之后的 HEAD 基线复现和 TurtleGrass源码检查均使用纯 Go 调用，Legacy lessons 仅在本仓库独立补丁中更新。

### 2026-08-17 — ManTree 对照读取不得在 Go workdir 混入 Legacy 路径

- Symptom: AI=174 研究时一条 Go workdir 只读命令同时带入 `Server/MirObjects/...` 与 `cmd/crystal-server/...`，Legacy 路径解析失败；命令未写入，整条输出作废。
- Root cause: 连续读取 C# `IsAttackTarget` 与 Go helper 时复用了两侧相对路径，没有把工具调用与单一仓库根目录绑定。
- Prevention: 跨仓库对照必须先结束当前调用；每次新调用先核对 `git rev-parse --show-toplevel` 和当前仓库文件清单，参数只允许该仓库路径，失败调用的其余输出不得作为证据。
- Verification: 失败调用未改变任何文件；随后 Legacy 与 Go 目标读取已拆成两个纯仓库调用，后续 AI=174 补丁和测试继续按同一边界执行。

### 2026-08-18 — AI=175 对照读取继续保持两个仓库隔离

- Symptom: AI=175 对照阶段再次把 Legacy 与 Go 路径放入同一只读调用；读取阶段失败，不能使用该调用的任何部分输出作为源码证据。
- Root cause: 为连续比较基线、Go 实现和迁移矩阵而复用了上一侧的路径参数，没有把每次工具调用绑定到唯一仓库根目录。
- Prevention: 跨仓库对照先结束当前调用；每个新调用先核对 `git rev-parse --show-toplevel` 和当前仓库文件清单，参数只允许该仓库路径，混合路径或非零读取结果整体作废并重跑。
- Verification: 失败调用没有写入文件；Legacy/Go 证据随后在独立、已核验的根目录调用中重读，AI=175 实现和测试只采用成功调用的输出。

### 2026-08-18 — Go 读取调用混入 Legacy 路径时必须立即作废

- Symptom: DragonWarrior 对照期间一次 Go workdir 读取命令误带 Legacy `Server/...` 路径，命令因路径不存在失败；没有产生写入。
- Root cause: 连续读取 C# 基线和 Go poison helper 时复用了另一仓库的相对路径，没有在调用参数层执行单仓库 allowlist。
- Prevention: Go 调用只允许已核验的 Go 仓库路径；Legacy 对照必须另起独立调用，任一混合路径或非零读取调用的全部输出都不得作为证据。
- Verification: 失败调用未改变工作树；后续 Go 读取将重新核对 `git rev-parse --show-toplevel` 后只读取 `cmd/`、`internal/`、`docs/` 路径。

- Strengthening after recurrence: DragonWarrior 防御 helper 核对时又在 Go workdir 混入 Legacy `Server/...` glob，zsh 在读取前失败；该调用的全部输出继续作废。
- Prevention strengthening: 执行 Go 命令前逐项扫描命令字符串，禁止任何 `Server/`、`Shared/`、`Client/` 路径或未核验 glob；跨仓库基线必须改用独立 Legacy 调用。
- Verification after recurrence: 失败命令未产生文件变化；本批后续判断不采用该调用输出。

### 2026-08-18 — Go 复杂 patch 的每个绝对路径都必须重新核对

- Symptom: FrozenAxeman 首次实现 patch 在执行前因 `world.go`/`monster_ai.go` 目标路径漏写 `me_work` 被拒绝；新增文件和源文件更新没有部分写入。
- Root cause: 只核对了新增文件路径，没有逐 hunk 复核所有绝对目标路径，重复了既有 Go patch 路径边界问题。
- Prevention: 复杂 patch 提交前逐项检查每个 `Add/Update File` 的完整路径，并在调用前用当前 Go workdir 的 `pwd`/`test -e` 验证；任一 hunk 路径异常时整条 patch 作废并重放。
- Verification: 失败 patch 后 Go 工作树无变化；使用完整 `.../git_work/me_work/Crystal.GoServer/...` 路径重放成功，随后 gofmt、包级编译和 FrozenAxeman 普通/race 测试通过。

### 2026-08-18 — Legacy 对照函数路径必须先由文件清单确认

- Symptom: FrozenMagician 对照时一次读取了不存在的 `Server/MirEnvir/Functions.cs`，命令非零退出；该输出不能作为源码证据。
- Root cause: 按 namespace/目录名称猜测文件位置，实际实现位于 `Shared/Functions/Functions.cs`。
- Prevention: 读取具体源码前先在目标仓库执行 `rg --files` 定位，再只读取清单返回的精确路径；任何非零读取结果全部作废。
- Verification: 重新定位并读取 `Shared/Functions/Functions.cs`，确认 `InRange` 是包含同格的 Chebyshev 距离，并据此通过 FrozenMagician 几何测试。

### 2026-08-18 — 双仓最终门禁必须复核真实仓库根路径

- Symptom: SnowYeti 最终 Legacy 状态检查误用了重复的 `Dropbox` 路径，进程未启动；该命令输出不能作为状态证据。
- Root cause: 手工重输绝对 workdir，没有复用本轮已审计的仓库根路径。
- Prevention: 双仓最终检查先在各自已知 workdir 执行 `git rev-parse --show-toplevel`，确认根路径后再读取状态和 C# 变更；任何非零命令输出全部作废。
- Verification: 从 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` 重跑，确认仅 `tasks/lessons.md` 修改、diff check 通过且三项 C# 检查均为空。

### 2026-08-18 — AI=201 多仓 patch 必须先固定目标仓库

- Symptom: AI=201 共享接线的首次多文件 patch 在 Legacy 工作目录尝试读取 Go `world.go`，整个 patch 被拒绝且未写入。
- Root cause: 跨仓文件路径和当前 workdir 混用，违反了本项目的单仓操作边界。
- Prevention: 每个 patch/命令只服务一个仓库；执行前确认 workdir 与所有目标文件属于同一根目录，失败后先检查无部分写入再重试。
- Verification: 改用 Go 仓库单独 patch 后，`FurbolgGuard` action 字段、AI 分发与解析均接入，`go test ./cmd/crystal-server` 编译通过。

### 2026-08-18 — 跨仓库工具调用必须复用已验证的仓库根路径

- Symptom: 本批次早先两次只读检查因手写 `workdir` 漏掉 `me_work` 或重复目录而失败；随后多次又把旧版 `Server/...` 路径带进 Go 仓库命令，相关输出都不能作为证据；本次 ExplosiveTrap 复核再次在 Go `workdir` 中执行了含 Legacy `Server/...` 前缀的只读定位命令。
- Root cause: 在已知两个相邻仓库的情况下重新猜测绝对路径，并在同一个命令中混用了两个仓库的路径；没有把仓库切换当成独立边界。
- Prevention: 每次切换仓库先在独立工具调用中执行并记录 `git rev-parse --show-toplevel`；后续同一批次的每个工具调用只使用该精确 `workdir`，命令正文也不得引用另一个仓库的路径。任何跨仓库比较必须拆成两个独立工具调用；一旦路径混用失败，立即丢弃全部输出并重跑，下一次调用前重新检查命令中每个路径前缀；若命令包含 `Server/`、`Shared/` 等旧版前缀，必须先确认当前 `workdir` 是 Legacy 根；Go-only 命令先用 `rg --files`/Go 相对路径确认目标，禁止把 Legacy 路径作为“顺手的对照查询”混入。
- Verification: 重新按两个独立仓库根目录完成 Go Trap 编译、普通/race 定向测试、后续全量检查及 C# 只读核验；本次复发的失败输出已丢弃，后续 ExplosiveTrap 检查改为不含旧版前缀的 Go-only 命令，并在提交前重复三项 C# 只读核验。

### 2026-08-18 — FireWall 复核仍要保持仓库路径边界

- Symptom: FireWall 行为复核中两次在 Go 仓库命令里误带旧版 `Server/...` 路径，命令只返回 `not found`，输出不能作为证据。
- Root cause: 为了在同一轮查询 Go 实现和 C# 基线，把旧版相对路径混进了 Go-only 命令；没有在切换仓库后把对照读取拆成独立工具调用。
- Prevention: Go 调用正文只允许 `cmd/`、`internal/`、`docs/` 等 Go 仓库路径；C# 对照必须另起 Legacy workdir 调用，且失败输出立即丢弃，不以失败定位结果推断行为。
- Verification: 后续 FireWall C# 读取改为 Legacy-only 调用，Go 读取改为 Go-only 调用；两边路径均由各自仓库根目录确认后继续。

### 2026-08-18 — Go-only 命令不得夹带 Legacy 文件路径

- Symptom: Concentration 行为定位的一次 Go 仓库查询误带 `Shared/Enums.cs`，命令失败；该输出不能作为协议或枚举证据。
- Root cause: 想在同一个查询里同时确认旧版 BuffType 与 Go 实现，违反了两个仓库的路径边界。
- Prevention: 旧版枚举、BuffInfo 和协议基线一律在 Legacy 独立调用读取；Go 调用只引用 `cmd/`、`internal/`、`docs/` 等 Go 相对路径，失败输出立即丢弃。
- Verification: 重新在 Legacy 确认 Concentration=15、Visible=false，在 Go 独立实现并通过协议、world、race 和 session 测试。

### 2026-08-18 — 跨仓库读取不得放入同一并行编排

- Symptom: 本批次恢复元素系统时，为降低往返延迟把 Legacy 与 Go 的只读定位命令放进同一个 `Promise.all`；虽然没有写入，但违反了项目规定的单仓库工具调用边界。
- Root cause: 把“命令均为只读”误当成“可以共享一次工具编排”，没有把每个工具调用绑定到唯一仓库根目录。
- Prevention: Legacy 与 Go 的状态、源码、测试、格式化和提交调用都必须拆成独立工具调用；禁止在同一个 `Promise.all`、shell 或路径变量中混放两侧，跨仓库对照只允许在两次独立成功调用后组合结论。
- Verification: 本次并行读取输出全部作废且未产生文件变化；后续元素系统复核已改为 Legacy-only 与 Go-only 的顺序调用。

### 2026-08-18 — 工具调用必须复用已验证的 Legacy workdir

- Symptom: 元素系统复核的一次 Legacy-only 读取把项目根目录手写成重复的 `Dropbox` 路径，进程创建失败，输出不能作为源码证据。
- Root cause: 没有复用当前会话已确认的绝对根路径，而是在工具调用中重新拼接路径。
- Prevention: 每次调用固定使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`；切换仓库前先独立核对 `git rev-parse --show-toplevel`，禁止手写或拼接已验证根路径。
- Verification: 失败调用未产生文件变化；下一次 Legacy 读取将先核对根目录，再使用纯仓库相对路径。

### 2026-08-18 — Go-only 查询再次禁止旧版路径

- Symptom: 元素球英雄目标复核的一次 Go 仓库查询仍夹带 `Server/MirObjects/HeroObject.cs`，旧版路径定位失败；该调用的全部输出不能作为实现证据。
- Root cause: 为同时查找旧版 HeroObject 和 Go 英雄攻击 helper，复用了跨仓库命令正文，没有在仓库切换时清空旧版参数。
- Prevention: Go workdir 的命令正文只允许 Go 仓库实际目录；旧版 C# 对照必须另起 Legacy-only 调用。即使同一 shell 后续还有合法 Go 子命令，只要出现旧版路径，整条输出作废并重跑。
- Verification: 失败查询未写入源码；后续英雄攻击复核改为纯 Go 调用，旧版 HeroObject 另行读取。

### 2026-08-18 — 仓库切换后命令正文必须清空另一侧路径

- Symptom: BackStep 复核期间多次把 Go 路径带入 Legacy 命令，或把 Legacy 路径带入 Go 命令；本次 FlashDash 评估又在 Go workdir 中夹带旧版路径，导致输出不能作为证据。
- Root cause: 切换 `workdir` 时沿用了上一仓库的命令片段，没有把命令正文限制为当前仓库的相对路径。
- Prevention: 工具调用先固定仓库根目录，再从空白正文重建命令；Go 调用只允许 Go 相对路径，Legacy 调用只允许 Legacy 相对路径。任何混入另一侧路径的调用整条作废并重跑，不能复用其中看似有效的子输出。
- Verification: 失败调用未写入源码；FlashDash 的 Go/Legacy 证据改为两个独立调用，后续 Go 调用不再引用旧版路径。

### 2026-08-18 — 路径隔离复发时整条跨仓库命令必须作废

- Symptom: BindingShot 协议序号复核时，把 Legacy `Shared/Enums.cs` 路径带入 Go 工作目录，导致该次调用的 Legacy 与 Go 查询同时报路径不存在，不能作为证据。
- Root cause: 切换工作目录后继续复用上一条命令正文，未先删除另一仓库的相对路径。
- Prevention: 每次仓库切换都从空白命令开始；Go 调用只能引用 Go 路径，Legacy 调用只能引用 Legacy 路径。发现混入后立即停止使用整条输出，记录并分仓库重跑。
- Verification: 本条记录后，协议序号与载荷核对拆成独立的 Legacy-only、Go-only 调用，并仅采用重跑结果。

### 2026-08-18 — Legacy 对照读取前要核对实际文件路径

- Symptom: HellFire 对照查询首次使用了不存在的 `Server/MirObjects/Map.cs` 和 `Server/Shared` 路径；随后又在 Legacy 仓库查询不存在的 `docs` 目录，命令输出包含错误信息，不能作为行为证据。
- Root cause: 未先从 `rg --files` 确认目标文件/目录归属，就凭目录印象拼接路径；迁移矩阵实际位于 Go 仓库。
- Prevention: 读取前先列出并核对实际路径与仓库归属；本项目中地图实现位于 `Server/MirEnvir/Map.cs`，矩阵位于 Go 仓库的 `docs/migration-matrix.md`，不得凭空假设协议或文档目录。任何混入错误的整条输出都丢弃后重跑。
- Verification: 用 Legacy-only `rg --files Server | rg '/(HumanObject|Map|MapObject)\\.cs$'` 找到源文件，并用 Go-only 读取矩阵；重新确认 HellFire 与 `LevelMagic` 行为，没有修改 C# 文件。

### 2026-08-18 — Teleport 对照查询混入 Go 路径时整条输出作废

- Symptom: Teleport/Blink/StormEscape 复核时，Legacy workdir 的查询正文混入了 Go 的 `cmd/crystal-server` 与 `internal/protocol` 路径；第一段报路径不存在，不能把同一调用后续看似有效的输出当作证据。
- Root cause: 从上一仓库复用查询正文，只切换了 `workdir`，没有重新按当前仓库的实际目录构造命令。
- Prevention: 仓库切换后从空白命令开始；Legacy 调用只允许 `Server/Shared/Client/tasks` 相对路径，Go 调用只允许 `cmd/internal/docs` 相对路径。发现跨仓库路径后立即丢弃整条输出并分仓库重跑。
- Verification: 本次没有修改 C# 或 Go 源文件；已记录该失败调用，后续 Teleport 行为证据改为独立的 Legacy-only 与 Go-only 查询。本轮复核时一次 Legacy-only 查询又误带 Go 相对路径，整条输出立即作废并用独立仓库命令重跑；后续工具调用继续按仓库单元审计。

### 2026-08-18 — EnergyShield 迁移继续隔离仓库调用并先核对值/指针形态
- Symptom: 本批次两次只读调用把 Legacy 路径混进 Go workdir，另一次 Legacy 查询使用了未匹配的裸 glob；一次 EnergyShield 编译还报 `cannot indirect target`；提交后的复核又假定了不存在的 `cmd/crystal-server/group.go`。
- Root cause: 仓库切换后复用了上一侧命令正文，未先确认 zsh 的 glob 行为、Go map 值类型或目标文件清单。
- Prevention: 每个工具调用只允许一个已核验仓库及其相对路径；查询 glob 使用 `rg --glob` 或先列文件；读取前用 `rg --files` 确认目标；接入 map 元素前先检查声明，值类型不做指针解引用，失败调用的全部输出作废。
- Verification: 后续 Legacy/Go 读取、补丁、测试和提交均按仓库拆分；错误路径复核输出被丢弃并用文件清单重跑；修正 `worldMonster` 值拷贝后，EnergyShield 定向测试、`go test ./...`、race、vet 与 build 全部通过。

### 2026-08-18 — 单仓只读命令不得携带另一仓库路径

- Symptom: MoonLight 研究期间一次 Go-only 命令误带了 Legacy 的 `Server/MirObjects/MapObject.cs`；随后核对 DarkBody/MoonLight 增伤公式时同类错误又发生两次，导致命令部分成功、部分失败。
- Root cause: 在同一 shell 命令中混用了两个仓库的相对路径，没有把跨仓库核对拆成独立工具调用；连续研究时也没有在执行前检查命令正文是否仍含对侧路径。
- Prevention: 每次工具调用先固定唯一且存在的 `workdir`，命令正文只使用该仓库的相对路径；执行前用 `pwd`/文件清单验证目录，并逐字检查命令中不得出现另一仓库根路径或对侧目录；出现路径错误或部分失败时丢弃整次输出，重新单仓读取。
- Verification: 三次混仓错误输出和一次不存在目录的失败调用均已作废；后续 Legacy 与 Go 查询分别在已验证的 workdir 重跑，并在执行前确认命令正文没有对侧仓库路径。

### 2026-08-18 — Legacy 只读检索必须先确认每个精确文件

- Symptom: UltimateEnhancer 包序核对时，Legacy `rg` 参数混入了尚未确认存在的 `Server/MirObjects/Buff.cs` 和其他路径，命令以路径错误结束；该调用的其余输出不能作为源码证据。
- Root cause: 连续查看实现和枚举时凭记忆追加了概念文件名，没有先用 `rg --files` 建立精确清单。
- Prevention: 只读调用先在当前仓库独立确认根目录和目标文件存在性，再让 `rg` 只读取已确认路径；任一非零读取调用的全部输出作废，禁止从部分成功输出继续推导行为。
- Verification: 失败调用未写入文件；随后 UltimateEnhancer 的实现判断只保留此前成功的 HumanObject/MapObject 读取和 Go 侧已核对的源码，未修改任何 C# 文件。

### 2026-08-18 — 已核对的仓库根目录必须直接复用于 Go 补丁

- Symptom: UltimateEnhancer 实现期间两次 `apply_patch` 把 Go 仓库绝对路径写错：一次重复了 `Dropbox`，一次漏掉了 `me_work`；补丁均在写入前失败。
- Root cause: 手工复制绝对路径时没有直接复用最近核对的 `git rev-parse --show-toplevel` 结果，也没有在补丁前用 `test -f` 验证每个目标。
- Prevention: Go 补丁先独立核对根目录和目标文件，再逐字复制该根目录；每次补丁只包含当前仓库路径，目标不存在时停止并重建补丁，不从失败文本推断状态。
- Verification: 两次失败补丁均未产生文件变化；使用精确的 `Crystal.GoServer` 根目录重试后，UltimateEnhancer 实现、世界测试和认证转录通过。

### 2026-08-18 — Go 补丁必须复用已核对的完整仓库根目录

- Symptom: PoisonCloud 审计期间一次 Go 只读命令和一次补丁因绝对路径漏写 `me_work` 而失败；失败调用没有产生可用源码证据或文件变化。
- Root cause: 手工重建 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 时遗漏了中间目录，没有直接复制最近确认的根目录。
- Prevention: 任何 Go 读写前先固定并复用完整根目录；`apply_patch` 前逐个用 `test -e`/`rg --files` 核对包含子目录的目标路径，路径错误时丢弃整次调用输出并重试，不从失败调用推断状态。
- Verification: PoisonCloud 补丁改用完整根目录后成功落盘并通过定向编译检查；IceThrust 首次补丁因漏写 `cmd/crystal-server/` 被拒绝且未改动文件，随后重新核对目标路径后再继续；失败补丁未修改文件。

### 2026-08-18 — Legacy 检索参数也必须逐项验证

- Symptom: MoonMist 入口核对的一次 Legacy `rg` 调用额外携带不存在的 `Crystal` 路径，虽读到了 `Server` 输出但命令返回路径错误。
- Root cause: 复制检索命令时没有先确认参数列表，每个路径是否属于当前 Legacy 根目录。
- Prevention: Legacy 只读查询只传当前仓库已由 `rg --files` 确认的路径；出现任一非零退出或路径错误时整次输出作废，不从部分成功结果继续推导。
- Verification: 错误调用输出未用于 MoonMist 判断；随后入口、法术和 `Map.CompleteMagic` 均在单独的合法 Legacy 调用中重读确认。

### 2026-08-19 — Legacy 对照命令必须先核对实际文件清单

- Symptom: 评估 Mirroring 时把不存在的 `Server/MirEnvir/Settings.cs` 放入 Legacy 只读命令；命令末尾失败，整条输出作废。
- Root cause: 凭记忆构造文件路径，未先用 `rg --files` 核对 Settings 实际位于 `Server/Settings.cs`。
- Prevention: Legacy 读取前先独立列出精确文件清单；任何非零读取调用的全部输出丢弃，不能使用前面的成功片段。
- Verification: 随后 `rg --files Server | rg '(^|/)(Settings|Config|Envir)\\.cs$'` 返回 `Server/Settings.cs`/`Server/MirEnvir/Envir.cs`，重读只使用成功纯 Legacy 调用；无文件写入。

### 2026-08-19 — 跨仓库对照命令必须保持单仓库边界

- Symptom: 复核 SpecialArrow 伤害时把 Legacy 和 Go 路径放进同一条 shell 命令，且 Legacy glob 在当前 shell 下失败；该混合调用的全部输出不能作为证据。
- Root cause: 忘记项目级约束要求每个工具调用只访问一个仓库，并在使用 glob 前没有先核对可用路径。
- Prevention: Legacy 对照、Go 实现检查和测试始终拆成独立工具调用；每次调用固定 workdir，跨仓库比较只使用两次成功的纯单仓库读取结果。
- Verification: 已丢弃混合命令输出，分别重读 Go 伤害函数和 Legacy HumanObject/MapObject 片段；后续检查继续使用单仓库调用。

### 2026-08-19 — 修改 Go 魔法目录前必须先查完整现有键

- Symptom: FatalSword 定向测试在编译阶段失败，`magic_catalog.go` 的 map literal 报 `duplicate key 91`。
- Root cause: 只查看了目录前段就追加 FatalSword，没有先用全文件搜索确认该键已存在于目录后段。
- Prevention: 修改静态 map 前先用 `rg -n` 搜索完整键名并检查所有命中；新增条目必须同时更新唯一性/数量测试，不能凭目录片段推断缺失。
- Verification: 删除重复条目、恢复 109 条目录数量断言，并重新运行目录编译测试后确认不再出现重复键。

### 2026-08-19 — 跨仓库路径错误复发后必须把每个 read 命令绑定到绝对根

- Symptom: 已有“跨仓库读取不得放入同一并行编排”的约束后，勘察下一技能时仍有只读命令在 Legacy workdir 下引用 Go 相对路径/通配符，产生路径不存在或 zsh glob 错误；没有修改文件，但浪费了勘察轮次。
- Root cause: 依赖当前 workdir 和相对路径来回切换仓库，并在同一命令字符串中混用第二个仓库的路径。
- Prevention: 每个 `exec_command`、补丁、格式化、测试和提交调用都显式绑定单一仓库的绝对 workdir，命令内只允许出现该仓库下的路径；派发前检查命令字符串，不在分隔符后追加另一仓库路径或 glob。
- Verification: 后续 Entrapment 的格式化、测试、构建、提交和三项 C# 审计均按仓库隔离执行并通过；误用命令未留下文件变更。

补充证据：本轮 SlashingBurst 勘察仍在 Go 查询编排中夹入了 Legacy 绝对路径；命令保持只读且失败在路径解析阶段。预防再加强为“一次 `functions.exec` 编排只允许一个仓库根，跨仓库查询必须拆成独立工具调用”，并在派发前逐个检查所有嵌套命令的 `workdir` 与路径。

### 2026-08-19 — 跨仓库核对不得使用 Promise.all 或混合命令编排

- Symptom: 本轮一次 `functions.exec` 用 `Promise.all` 同时发起 Legacy 与 Go 查询，违反单仓库调用边界；后续还出现了在 Go workdir 中引用 Legacy `Shared/Enums.cs`、以及缺少 `me_work` 的错误 Go 路径。
- Root cause: 为减少往返而把两个仓库的 workdir/相对路径放进同一编排，调用前没有逐条检查仓库根与命令字符串。
- Prevention: 每个工具调用/嵌套命令只允许一个明确仓库根；跨仓库比较必须拆成串行、独立调用，命令内不得出现另一仓库路径或相对路径；运行门禁前先用 `git rev-parse --show-toplevel` 核对 workdir，避免重复目录段导致命令根本未启动。
- Verification: 本批后续 Go 格式化、测试、矩阵编辑均只触及 Go 仓库，Legacy 仅通过独立调用读取 lessons；最后一次全仓测试前的错误 `Dropbox/Dropbox/...` 路径被及时识别，改用已核对的 Go 根目录后全仓测试通过，错误输出未用于实现判断。

- Strengthened evidence: 本批提交前的 Legacy `.cs` 审计再次因手写成 `Dropbox/Dropbox/...` 被拒绝，命令未启动；这证明仅凭记忆填写绝对路径仍会复发。
- Strengthened prevention: 所有跨仓库门禁调用前先运行独立的 `git rev-parse --show-toplevel`，只把其输出复制为后续 workdir；若路径校验失败，先修正并记录，不继续执行审计或提交。
- Strengthening after recurrence: 本轮 Deer 基线核对时又在 Legacy workdir 的单条只读命令中混入 Go 相对路径，命令在首个不存在目录处失败，输出完全作废。
- Strengthened prevention: 读取/测试/审计调用在派发前逐字检查“workdir + 所有相对路径”是否属于同一仓库；需要比较两侧时先完成两个纯仓库调用，再在模型侧比较结果，不在一个 shell 命令中拼接两侧路径。
- Verification after recurrence: 误用命令没有写入文件；随后 Go 漫游代码和 Legacy `MonsterObject`/`Deer` 基线分别在各自根目录成功读取，后续 Deer 测试与文档补丁未使用失败输出。

### 2026-08-19 — Go 检索编排必须先验证仓库路径与 JavaScript 参数

- Symptom: 本轮多次检索命令在执行前失败：两次 `functions.exec` 编排脚本报 JavaScript `Unexpected token ')'`，一次 Go workdir 拼成了不存在的 `Crystal.GoServer.GoServer`，另一次旧目录检索先报 `cmd/crystal-server` 不存在。
- Root cause: 手写嵌套工具参数时混用了转义引号，并沿用了未先核对的目录/相对路径假设；失败发生在进程启动前，不能提供代码证据。
- Prevention: 工具脚本统一使用最小、单引号参数对象；每次跨仓库/长任务前先独立运行 `pwd` 与 `git rev-parse --show-toplevel`，再使用已验证根目录和 `rg --files` 得到的实际相对路径。
- Verification: 改用保守参数格式和已核对的 Go/Legacy 根目录后，Legacy 源码、Go 实现读取及后续测试均成功，失败命令没有产生文件修改。

### 2026-08-19 — 对照命令不得把 Go 路径带入 Legacy 工作目录

- Symptom: 读取 PetSettings 对照时在 Legacy 根目录追加了 Go 的 `internal/worlddata/world.go`，命令在首个不存在路径处失败；没有写入，整条输出不能作为源码证据。
- Root cause: 连续查看两侧配置时复用了另一仓库的相对路径，没有把 `workdir` 与参数集合绑定为单一仓库。
- Prevention: 跨仓库对照必须拆成独立调用；每次切换前重新执行当前仓库的 `git rev-parse --show-toplevel`，调用中只允许出现该仓库已核验的路径，失败输出整体作废。
- Verification: 本次失败命令未改变工作树；随后将 Legacy `Server/...` 与 Go `internal/...` 查询拆开，并只采用两条成功调用的输出。

- Strengthening after recurrence: 本批一次 Go 只读编排又因 `functions.exec` JavaScript 参数对象漏写闭合括号而在执行前失败；即使命令本意只读，也不能把未执行当作源码证据。
- Strengthened prevention: 复杂读取拆为最小单仓库调用，先用单个 `exec_command` 验证参数闭合和文件存在，再扩展查询；编排脚本必须在提交前逐字检查 `await tools.exec_command({...}); text(r.output);` 结构。
- Verification after recurrence: 失败发生在工具脚本解析阶段、两个仓库均无文件变化；后续 Go 读取改为单个合法调用后再继续分析。

- Strengthening after second recurrence: 本轮再次把 Go 的 `cmd/crystal-server/*.go` 路径带入 Legacy 命令，shell 在执行前因路径不存在而失败；以后每条只读命令也必须做仓库路径审计，不能仅依赖 workdir。
- Verification after second recurrence: 失败发生在 shell 展开阶段且无文件写入；拆分为 Legacy-only 与 Go-only 调用后，元素/觉醒源码证据均来自成功命令。

- Strengthening after third recurrence: 本轮读取 Go monster AI 时又在 Go workdir 下引用了错误的 `internal/world/monster_ai.go` 路径；命令只读但没有产生源码证据。
- Strengthened prevention: 任何目标文件读取前先用当前仓库的 `rg --files -g '<name>'` 解析实际相对路径，再把该路径用于后续单仓库命令；不得凭记忆补目录名。
- Verification after third recurrence: 通过 `rg --files` 找到 `cmd/crystal-server/monster_ai.go` 后重新读取成功；Go logging/config/服务端全量测试、race、vet 和 build 均使用已核验的单一 Go 根目录，错误读取没有文件变化。

- Strengthening after fourth recurrence: 本轮 Legacy 读取命令再次误拼成 `Dropbox/Dropbox/...`，进程未启动且输出不可作证据。
- Strengthened prevention: 每次切换仓库先独立执行 `git rev-parse --show-toplevel`，再从当前仓库用 `rg --files` 解析目标路径；命令中不得出现另一仓库路径，路径校验失败后不得继续使用输出。
- Verification after fourth recurrence: 纠正为已核验的 Legacy 根目录后，`HumanObject.cs`/共享数据读取成功；错误命令没有文件修改或源码证据。

### 2026-08-19 — AI=89 特效入口必须按 C# 的 Struck/Attacked 重载逐条路由

- Symptom: 直接特效路径初版把 TreeQueen 根系等 `Struck` 调用误接到 IcePillar 的 `Attacked` 逻辑，可能错误扣 HP/施加后续毒。
- Root cause: 只按防御类型名称推断目标入口，未同时核对 C# `SpellObject.ProcessSpell` 的实际重载与后续 `ApplyPoison`；IcePillar 的 `Struck` 和 `ApplyPoison` 都是 no-op。
- Prevention: 为静态/特效目标逐路径标注 `Attacked` 或 `Struck`；目标 no-op override 必须在 Go 入口和后续状态效果两处短路，不能只在公共伤害函数里兜底。
- Verification: 已按 C# 修正 TreeQueen/Horned/Flying/Stone/Tucson/DarkOma/General/Healing/MapQuake 等 Struck 路径，并通过 IcePillar/相邻 AI 定向、全量 Go 测试、vet 和 build。

### 2026-08-20 — Legacy/Go 只读核对命令也必须保持仓库边界

- Symptom: 本轮核对 AI=3 Tree 攻击入口时，在 Legacy workdir 的单条只读命令中混入 `cmd/crystal-server` Go 路径；Go glob 未命中，后续输出无效。
- Root cause: 将跨仓库对照误写成一个 shell 调用，违反了每个命令只使用当前 `workdir` 同仓库相对路径的边界约定。
- Prevention: Legacy 与 Go 的读取、`rg`、测试和审计全部拆成独立调用；需要比较时先分别取得两份证据，再在模型侧对照，禁止一个命令引用两个仓库路径。
- Verification: 该失败调用未产生写入；随后 Legacy 读取保持仅含 `Server/...` 路径，Go 读取单独在 Go workdir 执行，继续核对前先废弃失败输出。
- Strengthening after same-session recurrence: 后续一次 Legacy workdir 命令又误带了 Go 绝对路径；虽然仍未写文件，但该段输出同样被废弃。
- Strengthened prevention: 每次调用前逐项检查 `workdir`、命令内相对路径和绝对路径，禁止在 Legacy workdir 出现任何 Go 路径，反之亦然；比较动作只在模型侧合并两次独立读取。
- Verification after recurrence: 已在第二次错误命令后立即停止使用其输出并更新本 lesson；后续实现调用将按单仓库白名单执行。

### 2026-08-20 — Legacy 缺口检索不得使用未核对目录 glob

- Symptom: 对照 Monster AI 缺口时把不存在的 `Server/MirDB/*.cs` 放入 Legacy 命令，zsh 因 glob 无匹配而失败。
- Root cause: 未先用 `rg --files`/目录检查确认路径，且把多个猜测 glob 放进同一读取调用。
- Prevention: 先独立核对目录和精确文件，再使用已确认路径或 `rg --glob`；任何非零读取调用的全部输出都作废。
- Verification: 失败命令未写入项目；后续只使用已确认的 `Server/MirObjects/MonsterObject.cs`、`Server/MirEnvir/Envir.cs` 等路径。
- Strengthening after same-session recurrence: Go 仓库根目录没有匹配 `*.go` 的文件，未核对布局的根目录 glob 再次使只读检索失败。
- Strengthened prevention: 先用 `rg --files` 确认文件所在目录，再把精确相对路径或已确认目录传给 `rg`；禁止把猜测 glob 放进后续读取命令。
- Verification: 该 Go 命令没有被用于推理；后续读取改在已确认的 `cmd/crystal-server` 路径中执行。
- Strengthening after second same-session recurrence: 本轮又误读了不存在的 `Server/MirObjects/MonsterInfo.cs`、`Server/MirEnvir/Settings.cs`、`Server/MirMap.cs`，并在 Legacy workdir 中带入 `cmd/crystal-server/*.go`；另一条 Go 读取还猜测了 `cmd/crystal-server/worlddata.go`。这些调用的输出全部作废。
- Verification after strengthening: 后续先以 `rg --files` 核对实际的 `Server/MirDatabase/MonsterInfo.cs`、`Server/Settings.cs`、`Server/MirEnvir/Map.cs` 与 `internal/worlddata/world.go`，再分别在单仓库 workdir 重跑读取；实现和测试只采用成功调用的证据。
- Strengthening after third same-session recurrence: 本轮 Legacy 读取 `MonsterObject.cs` 时又在同一命令尾部加入 Go 的 `cmd/crystal-server` 路径，导致该只读调用非零；即使前段 Legacy 输出完整，也全部作废，未用于后续判断。
- Verification after third strengthening: C# 构造器证据随后在纯 Legacy 调用重新读取，Go 字段/实现证据在独立 Go 调用读取；之后的测试、补丁和审计均按仓库边界执行。

### 2026-08-20 — RootSpider/BombSpider 配置与延迟毒伤必须覆盖完整时序

- Symptom: 新增 `BugBatName`/`BombSpiderName` 后，`TestMonsterSettingsDefaultsAndJSONCompatibility` 首次失败；BombSpider 爆炸测试在首次实现中把玩家 HP 从 100 算到 83，而 Legacy 期望主伤害后仍为 90。
- Root cause: 配置字段只同步了生产结构和 loader，遗漏了完整默认值/JSON 夹具；BombSpider Green poison 未设置首次 `TickAt`，统一 poison processor 在爆炸同一 tick 立即追加了首跳毒伤，违背 Legacy 的两秒 TickSpeed。
- Prevention: 每个新增 Setup 字段同时更新默认值、loader、运行时 fallback、JSON 完整结构断言和自定义 Setup 读取测试；所有延迟施加的毒物显式设置 `TickAt = impact + Tick`，并分别断言命中 HP、毒物列表和首次到期 HP。
- Verification: `go test ./internal/worlddata ./internal/legacyworld -count=1`、`go test ./internal/legacyworld -count=1` 与 RootSpider/BombSpider 定向测试通过；随后 `go test ./... -count=1 -timeout=600s` 全部通过。
- Strengthening after session recurrence: RootSpider/BombSpider 认证夹具首次把 Player 放在 child 同一格，误以为会立即触发接触死亡；Legacy 的 `InAttackRange` 对同格返回 false，只有相邻不同格才进入 `Die`。随后又把 `ServerPoisoned` 错放在爆炸通知中，忽略了 Green 的首个 2000ms 处理边界。
- Verification after strengthening: 认证转录改为 child 与 Player 相邻，锁定 `ObjectDied`；爆炸时只断言伤害/Chat，`impact+2s+1ms` 单独断言首跳伤害和 `ServerPoisoned`，定向测试通过。
- Strengthening after race recurrence: `net.Pipe` 转录首次在 `-race -count=5` 中偶发/重复看到手工接触阶段没有 `ObjectDied`；停止 poison ticker 不会停止连接读循环在每次客户端读取间执行的实时 `world.tick`。
- Verification after strengthening: 认证夹具在手工驱动前显式将 `monsterAIEnabled=false`，保留直接 resolver 的通知投递；普通定向测试与 `go test -race ./cmd/crystal-server -run 'RootSpider|BombSpider|BugBag' -count=5` 通过。

### 2026-08-20 — 迁移矩阵读取必须按仓库分别验证目录

- Symptom: 本轮在 Legacy workdir 同一条 `rg` 命令中同时读取存在的 `tasks/migration-handoff.md` 和未核实的 `docs/migration-matrix.md`，后者不存在使整条读取调用非零。
- Root cause: 继续把 Legacy handoff 与 Go migration matrix 当成一个跨仓库读取动作，未先核对目录归属。
- Prevention: 读取或更新 handoff 只在 Legacy workdir 使用 `tasks/...`；读取或更新 matrix 只在 Go workdir 使用已核实的 `docs/...`，比较时在模型侧合并两次成功输出；任何非零读取调用的全部输出作废。
- Verification: 本次调用没有写入；后续将分别在两个仓库重读目标文件，再进行文档更新与审计。

### 2026-08-20 — 跨仓库工具调用的 workdir、路径和 patch 锚点必须三重核验

- Symptom: 本轮一次 Go 读取使用了重复的 `Crystal.GoServer.GoServer` 根路径，一次 Legacy 读取带入未存在的 `docs/*.md` glob，另一次对 Go 文件的绝对路径 `apply_patch` 在 Legacy 工具上下文中找不到文件；这些调用均失败且没有写入。
- Root cause: 继续执行时复用了摘要中的路径/目录假设，没有在每个工具调用中把唯一仓库根、已确认目录和 patch 相对锚点绑定起来。
- Prevention: 每个 shell 调用只使用一个仓库根和绝对 `workdir`；zsh 查询先用 `rg --files` 核对目录，禁止未确认 glob；从 Legacy 上下文修改 Go 时先核对 `../Crystal.GoServer/...` 相对锚点，失败调用整体作废。
- Verification: 三次失败调用均未改变工作树；随后在独立 Go/Legacy 调用中完成 AI=15 源码、测试和文档操作，并通过定向测试。

### 2026-08-20 — AI=17 研究读取必须继续保持 Go/Legacy 目录隔离

- Symptom: 在 Go workdir 查询 AI=17 矩阵后，把 Legacy 的 `tasks/lessons.md` 追加到同一条命令；Go 仓库没有该目录，调用返回非零。
- Root cause: 继续执行时把“当前 Go 的 matrix”和“Legacy 的 lessons”当成同一读取步骤，未按仓库边界拆分命令。
- Prevention: Go 只在自身已确认存在的 `docs/...` 目录读取矩阵；Legacy 只在自身 `tasks/...` 目录读取 lessons，任何跨仓库证据都用独立调用取得。
- Verification: 本次失败调用无写入且输出全部作废；随后将分别用 Go/Legacy workdir 重读目标文件，再继续 AI=17 对照。
- Strengthening after next-session recurrence: 本轮在 Go workdir 查询 Shinsu 时又把 Legacy 的 `Shared/Client/Server` 相对路径带入同一条命令；该读取返回非零，全部输出作废。
- Strengthened prevention: 先在当前仓库用 `rg --files` 解析实际路径；跨仓库查询必须拆成独立工具调用，且每条命令的相对路径只能来自当前 `workdir`，不能为了并行对照拼接另一仓库目录。
- Verification after strengthening: 失败调用未写入；随后 Legacy 的 `Server/...` 与 Go 的 `cmd/...`、`internal/...` 查询分别成功，后续判断只采用这些独立结果。
- Strengthening after same-session path recurrence: 本轮一次 Go 只读调用把已知根目录误写成 `Crystal.GoServer.GoServer`，进程未启动，命令输出不能作为证据。
- Verification after this recurrence: 该调用没有文件变化；之后恢复使用精确的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` workdir，并先用 `rg --files`/`git rev-parse` 校验路径后再读取。
- Strengthening after same-session recurrence: 本轮另一条 Go workdir 查询又把 Legacy `Server/MirObjects/...` 路径接在 Go 查询后，导致命令非零；跨仓库的失败输出继续不能作为证据。
- Verification after this recurrence: 无文件变化；后续只在 Go 根读取 `cmd/...`，需要 C# `GetArmour` 证据时另开 Legacy 调用，并在模型侧合并结果。

### 2026-08-20 — AI=17 Legacy 对照文件必须先用仓库索引确认路径

- Symptom: 研究子怪构造方向时先读取了不存在的 `Server/MirObjects/Monsters/MonsterObject.cs`，导致对照命令失败。
- Root cause: `MonsterObject.cs` 位于 `Server/MirObjects/MonsterObject.cs`，未先通过仓库文件索引确认公共基类路径。
- Prevention: Legacy 对照从 `rg --files` 定位基类和派生类后再读取；不要根据目录层级猜测 C# 文件位置。
- Verification: 重新索引确认 `Server/MirObjects/MonsterObject.cs`、`ZumaMonster.cs`、`ZumaTaurus.cs` 均存在；未发生任何 C# 写入。

### 2026-08-20 — Goal continuation 的初始审计也必须隔离 Legacy 与 Go 调用

- Symptom: 本轮继续 active Goal 的初始只读审计把 Legacy lessons/status 与 Go status/migration matrix 放进同一个 `Promise.all`；虽然没有写入，但违反了项目的单仓库工具调用边界。
- Root cause: 为减少状态核对往返，把“只读”误当成可以共享编排；没有把每个工具调用的 workdir、路径参数和证据来源绑定到唯一仓库。
- Prevention: Goal 续跑的 lessons、status、源码、矩阵、测试和最终审计都按仓库拆成独立 `functions.exec` cell/call；禁止在同一个 `Promise.all`、shell 或 JavaScript 编排中混放 Legacy 与 Go，比较只在两次成功的纯单仓库调用返回后进行。
- Verification: 本轮初始混合调用输出未用于源码判断；随后完整回读 Legacy lessons 使用独立调用，后续批次勘察与实现将先分别核对根目录和目标路径，再执行另一仓库调用，且本 lesson 在任何源码修改前追加完成。

### 2026-08-20 — Legacy 只读命令不能携带 Go 相对路径

- Symptom: 在 Legacy workdir 中查询迁移矩阵时把 Go 的 `docs/migration-matrix.md` 相对路径带入同一条命令，得到文件不存在；该调用没有取得有效矩阵证据。
- Root cause: 只读审计的 workdir 与相对路径没有绑定到同一个仓库，复用了另一仓库的路径习惯。
- Prevention: 每条 `functions.exec` 命令只使用当前 workdir 仓库内的相对路径；需要比较时先分别完成两次单仓库调用，再在模型侧比较输出，禁止跨仓库路径混入一条命令。
- Verification: 失败调用输出未用于判断；随后在精确 Go 根目录独立读取 `docs/migration-matrix.md`，并继续基于成功的 Go/Legacy 单仓库证据推进。

- Strengthening after same-session recurrence: 本轮 Legacy 最终审计的 `workdir` 手工拼成了不存在的路径，进程未创建。
- Strengthened prevention: 最终门禁命令必须直接复制已确认的绝对仓库根目录；若 CreateProcess 报路径不存在，立即作废调用并在任何状态判断前重跑精确根目录。
- Verification after strengthening: 该失败调用没有文件变化；后续将使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` 重新执行全部 Legacy/C# 审计。

### 2026-08-20 — AI=22 勘察必须保持仓库路径隔离

- Symptom: 定位 Legacy AI=22 工厂入口时，把 Go 的 `docs/migration-matrix.md` 相对路径混入 Legacy workdir；该部分 `rg` 报文件不存在，只有前面的 Legacy 输出有效。
- Root cause: 同一条只读 shell 命令同时依赖两个仓库的相对路径，违反了本项目的单仓库调用边界。
- Prevention: Legacy 勘察命令只查询 Legacy 路径；Go 矩阵或实现查询必须在独立的精确 Go workdir 调用中执行，不能把跨仓库相对路径放入同一命令。
- Verification: 失败的跨路径部分未用于判断；随后只依据 Legacy `MonsterObject`/`IncarnatedZT.cs` 成功输出确认 AI=22 入口。

### 2026-08-20 — Go 仓库查询不能把已知文件名降级为错误相对路径

- Symptom: AI=23 补丁失败后的 Go 状态核查在 Go 根目录执行了 `rg world.go`，因文件实际位于 `cmd/crystal-server/world.go` 报不存在；该部分没有源码证据。
- Root cause: 已确认的仓库层级没有与命令中的相对路径同步，误把包内文件当成根目录文件。
- Prevention: Go 查询统一使用从仓库根展开的精确相对路径（如 `cmd/crystal-server/world.go`），路径错误输出立即作废，不与同条命令的成功输出混用。
- Verification: 失败命令没有文件变化；随后用精确包路径读取 world.go 与 summon_skeleton_test.go，确认实际上下文。

### 2026-08-20 — AI=25 勘察命令不能把 Legacy 相对路径带入 Go workdir

- Symptom: 在 Go workdir 查询 RevivingZombie 时混入 `Server/MirObjects/MonsterObject.cs`，`sed` 报文件不存在；该部分没有读取到 Legacy 基线。
- Root cause: 同一编排中复用了 Legacy 相对路径，没有保持仓库与相对路径的一一绑定。
- Prevention: Legacy 源码只在 Legacy 精确 workdir 查询，Go 源码只在 Go 精确 workdir 查询；跨仓库比较拆成独立调用，失败部分输出立即作废。
- Verification: 失败的 `sed` 没有修改文件；随后将分别在正确的 Legacy/Go workdir 重读 RevivingZombie 基线与 revive runtime。

### 2026-08-20 — Go 查询 workdir 必须直接使用已确认根目录

- Symptom: AI=26 只读查询把 Go workdir 写成 `Crystal.GoServer.GoServer`，CreateProcess 报路径不存在；该调用没有取得源码证据。
- Root cause: 手工拼接已确认的绝对仓库根目录时重复追加了仓库名。
- Prevention: Go 命令统一复制精确根目录 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，不在其后追加目录；路径失败输出立即作废。
- Verification: 失败调用没有文件变化；随后在精确 Go 根目录重读 `monster_ai.go`、`stoning_statue.go` 和攻击 resolver。

### 2026-08-20 — AI=27 双仓库核查不得混用相对路径

- Symptom: Khazard 勘察命令在 Legacy workdir 中带入不存在的 `cmd`/`docs` 路径，随后另一条 Legacy 命令又带入 `cmd/crystal-server/*.go`，分别产生 `No such file or directory` 和 zsh `no matches found`；这些片段没有取得有效证据。
- Root cause: 跨仓库读取时没有让 workdir 与相对路径保持一一绑定，把 Go 包路径误用于 Legacy 根目录。
- Prevention: 每条核查命令只服务一个仓库；Legacy 仅使用 `Server/...`、`tasks/...`，Go 仅使用 `cmd/...`、`docs/...`，跨仓库比较拆成独立调用，路径错误输出全部作废。
- Verification: 失败命令没有修改文件；随后在各自精确根目录重新读取 Khazard C# 基线与 Go 运行时代码，且后续状态核查分别通过。

### 2026-08-20 — 跨仓库初始审计复发后必须丢弃混合调用证据

- Symptom: 本轮继续 active Goal 时，初始 Legacy lessons/status 与 Go status 查询再次放入同一个 `Promise.all`；调用只读且没有写入，但违反了单仓库证据边界。
- Root cause: 把降低往返延迟置于已建立的仓库隔离约束之上，未在派发前检查每个嵌套调用的 workdir 与路径集合。
- Prevention: 每个 `functions.exec` 编排只绑定一个绝对仓库根目录；跨仓库状态、源码、矩阵、测试、审计和 Git 操作必须拆成串行独立调用，混合调用的全部输出立即作废。
- Verification: 该混合调用未产生文件变化；随后 lessons 以 Legacy-only 分块完整读取，后续 AI=28 勘察将使用独立 Legacy/Go 调用，并在最终门禁重复验证 C# 零变化。

### 2026-08-20 — Go-only 矩阵查询不得携带 Legacy `tasks` 路径

- Symptom: AI=28 矩阵核对的 Go workdir 命令同时引用了 Go `docs/migration-matrix.md` 与不存在的 Legacy `tasks/migration-handoff.md`；即使 `|| true` 隐藏退出码，整条调用仍不具备可用证据。
- Root cause: 为同时读取矩阵和交接文档复用了跨仓库参数，且用 shell 容错掩盖了路径错误，没有执行单仓库路径 allowlist。
- Prevention: Go-only 调用只允许已核验的 `cmd/`、`internal/`、`docs/` 路径；Legacy `tasks/` 文档另起 Legacy-only 调用，任何错误路径、`|| true` 或混合参数都使整条输出作废。
- Verification: 该查询只读且没有文件变化；矩阵将由纯 Go `docs/migration-matrix.md` 调用重读，后续审计不再用容错掩盖路径错误。

### 2026-08-20 — Legacy lesson 写入必须复用完整仓库根路径

- Symptom: 追加 BoneSpearman lesson 的一次 `apply_patch` 使用了缺少 `me_work` 的 `/Users/wszf/Dropbox/source_code/git_work/Crystal/...`，工具报文件不存在，未取得写入结果。
- Root cause: 手工重敲绝对路径时遗漏已确认的中间目录，违反了 lessons 中“复制精确 workdir、禁止猜路径”的约束。
- Prevention: Legacy 文档和源码写入统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` 根目录；执行前逐段核对 `Dropbox/source_code/git_work/me_work/Crystal`，失败调用不作为状态证据。
- Verification: 随后使用完整路径成功追加本次 AI=29 两条 lesson，并重新读取其尾部确认内容存在。

### 2026-08-20 — Goal continuation 的勘察调用必须保持单仓库路径闭包

- Symptom: BoneLord 勘察先后出现两次无效调用：Legacy workdir 中追加了 Go 的 `cmd/...`/`docs/...`，Go workdir 中又追加了 Legacy 的 `Server/...`；均因路径不存在非零退出，同一调用的有效输出也不能作为证据。
- Root cause: 选择下一批 AI 时复用了跨仓库查询正文，派发前没有按 workdir 检查每个相对路径；且未把“单仓库路径闭包”当作调用级约束。
- Prevention: 每条调用只能使用当前 workdir 所属仓库的相对路径：Legacy 仅 `Server/...`、`tasks/...`，Go 仅 `cmd/...`、`internal/...`、`docs/...`；混合路径或非零退出的只读调用整体作废并重跑，禁止引用其任何输出。
- Verification: 两次失败调用均未修改源码；随后只用精确 Legacy/Go 根目录的独立成功调用重新读取 BoneLord 基线与 Go 缺口。

### 2026-08-20 — Legacy 基线读取必须先核验实际目录

- Symptom: SandWorm 对照读取把存在于 `Server/MirObjects/MonsterObject.cs` 的基类路径误写成 `Server/MirObjects/Monsters/MonsterObject.cs`，命令非零；该调用包含的其他输出按规则全部作废。
- Root cause: 根据子类目录手工推断基类位置，没有先用 `rg --files Server` 核验实际文件路径。
- Prevention: Legacy 对照前先在同一仓库根目录用 `rg --files` 确认每个目标文件，再执行 `sed`；任一目标不存在或命令非零时丢弃整条只读调用，不引用其部分输出。
- Verification: 随后用精确的 `Server/MirObjects/MonsterObject.cs` 路径独立重读 ProcessTarget、LineAttack、目标校验和 GetAttackPower，SandWorm 实现与测试依据来自成功调用。

### 2026-08-20 — Goal continuation 的 Go 路径必须先验证文件存在

- Symptom: VenomSpider 勘察期间一次 Go workdir 少写了 `me_work`，另一次把不存在的 `player_combat.go` 当作实现文件读取；另有一次 poison 对照命令把 Go 路径混入 Legacy workdir，均产生非零调用。
- Root cause: 手工重敲绝对 workdir/文件名，且在跨仓库对照时没有先按当前仓库建立路径闭包；错误调用中的部分输出不能作为证据。
- Prevention: 继续 Goal 时先用当前仓库根目录的 `rg --files` 核验文件，再执行 `sed`；每个调用只允许当前仓库路径，Legacy/Go 对照必须拆成独立成功调用，任何非零或混合路径输出整体作废。
- Verification: 后续 Legacy 与 Go 基线均由独立成功调用重读；VenomSpider 实现经定向、race、全量测试、vet 和 build 门禁通过。

### 2026-08-20 — 跨仓库只读调用不可混入对照路径

- Symptom: BlackFoxman 勘察期间两次 Go 调用误带入 Legacy 的 `Server/MirObjects/MonsterObject.cs` 路径并产生非零输出；两条调用的全部输出均被作废，未用于实现判断。
- Root cause: 在同一 shell 命令中拼接了两个仓库的读取路径，没有先按仓库拆分调用，也没有在执行前扫描命令中的路径前缀。
- Prevention: 每个工具调用只使用一个仓库的 workdir 和已核验路径；生成命令后先确认所有路径都属于当前仓库，Legacy/Go 对照必须拆成独立成功调用，任一混入路径或非零结果都丢弃整条调用。
- Verification: 记录后先用 Legacy-only 调用更新本 lesson，再重新发起不含 Legacy 路径的 Go-only 读取；后续只引用成功且单仓库的输出，未产生源代码变更。

- Strengthening after AI=66 selection: 本次勘察的一条调用在 Go workdir 中同时带入 Legacy `Server/MirObjects/...` 与 Go `cmd/crystal-server/...` 路径并非零；整条输出已作废，未用于 AI=66 判断。
- Verification after recurrence: 已停用该调用结果，后续 AI=66 对照将严格拆成 Legacy-only 与 Go-only 两条成功调用；本 lesson 写入本身使用完整 Legacy 根路径。
- Strengthening after immediate recurrence: 切换回 Legacy workdir 后又复用了 Go `cmd/crystal-server/...` 查询正文并产生非零；这次输出同样全部作废，说明仓库切换时不能复用上一条命令字符串。
- Prevention after recurrence: 每次切换仓库先重建命令 allowlist，再执行；Legacy 调用只出现 `Server/...`/`tasks/...`，Go 调用只出现 `cmd/...`/`internal/...`/`docs/...`，并逐条检查 workdir 与相对路径前缀。

### 2026-08-20 — Goal continuation 的 patch 路径必须绑定当前仓库绝对根

- Symptom: AI=31/32 实现期间一次相对路径 `apply_patch` 在 Legacy 根目录解析 Go 文件而被拒绝，另一次手写绝对路径重复了 `Dropbox` 目录；两次均未改文件。
- Root cause: `apply_patch` 不继承 `exec_command` 的 `workdir`，且手工复制绝对路径时没有复核 `.../git_work/me_work/Crystal[.GoServer]` 的根段。
- Prevention: patch 一律使用当前仓库已核验的绝对路径；执行前检查路径只包含一个仓库根，失败调用输出作废，不把相对路径假定为当前 shell workdir。
- Verification: 两次失败调用均无文件变化；随后 Go patch 使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer/...` 成功，Legacy lesson 使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal/...` 成功，状态与 diff 检查通过。
- Strengthening after recurrence: 提交后 Legacy 审计又把根路径写成重复的 `Dropbox` 目录，进程在创建阶段失败且没有取得状态证据。
- Prevention after recurrence: 仓库切换后的每条命令直接复制已确认的 `workdir`，不手写或拼接绝对路径；审计命令必须在成功返回后才可作为提交证据。
- Verification after recurrence: 丢弃错误调用，使用精确 Legacy 根路径重新完成 diff/status/log 与 `.cs` 三项审计，结果通过且无文件变化。

### 2026-08-20 — Legacy 协议文件路径必须先用精确文件清单确认

- Symptom: 读取 `Server/ServerPackets.cs` 失败，因为当前仓库的协议定义实际位于 `Shared/ServerPackets.cs`。
- Root cause: 依据文件名推测目录，未先用 `rg --files` 验证精确路径。
- Prevention: 访问 Legacy 文件前先用 `rg --files | rg '(^|/)文件名$'` 定位，再执行逐行读取；迁移代码仍只在 Go 仓库编辑。
- Verification: 重新定位后已逐行核对 `Shared/ServerPackets.cs` 的 `ObjectSitDown` 字段、读写顺序和枚举位置，未修改任何 C# 文件。

### 2026-08-20 — 仓库切换后的命令不得混入另一仓库路径

- Symptom: 一次 Go 仓库只读核对命令同时引用了 Legacy 绝对路径，失去“单工具调用只服务一个仓库”的审计边界。
- Root cause: 为补充 Legacy 对照时把跨仓库检索路径拼进了 Go workdir 命令，没有拆成独立的 Legacy 调用。
- Prevention: 每条 `exec_command` 只使用一个仓库的 `workdir` 和相对路径；需要跨仓库时拆成两个独立调用，禁止在同一命令中出现另一仓库根。
- Verification: 本次后续 Go 与 Legacy 命令均按仓库拆分执行；最终两仓库 `.cs` diff/staged/untracked 审计独立通过。

### 2026-08-20 — Legacy 对照与 Go 检索不得共用一次命令

- Symptom: Yimoogi 对照检索时把 Legacy 工作目录与 Go 相对路径放进同一条命令；命令因在 Legacy 根解析 Go 路径而失败，未产生文件改动。
- Root cause: 读取 Legacy 基线后切换仓库时，仍沿用同一调用构造跨仓库路径，违反单调用单仓库边界。
- Prevention: 先在当前仓库完成一组只读证据，再结束调用；需要另一仓库时新开独立调用，并让 `workdir` 与所有相对路径属于同一仓库，禁止混入另一仓库根或路径片段。
- Verification: 该失败调用输出作废；后续 Yimoogi Legacy 与 Go 检索分别在各自精确根目录成功执行，且 `git diff` 未出现 C# 文件变化。
- Strengthening after recurrence: 后续一次调用仍沿用 Legacy `workdir` 却引用 Go 相对路径，命令再次失败且未改文件。
- Prevention after recurrence: 每次工具调用前显式复核 `workdir` 字符串是 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` 或 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 之一，并只允许对应仓库的相对路径；不要依据上一条调用的仓库猜测当前目录。
- Verification after recurrence: 丢弃该失败输出；在继续执行前重新核验每条命令的根路径，Legacy/Go 证据分别成功取得，C# 文件仍无变化。

### 2026-08-20 — 交接与 lessons 文件必须按仓库归属读取

- Symptom: 在 Go 仓库检索 Legacy 专属的 `tasks/migration-handoff.md` 与 `tasks/lessons.md`，命令因相对路径不存在而报错。
- Root cause: 已知两份项目文档位于 Legacy 仓库，却在切换 Go workdir 后仍按文件名直接读取，未先确认文档归属。
- Prevention: 每条命令先确认目标文件属于当前仓库；Legacy 交接/lessons 只在 Legacy 根目录读取，Go 矩阵/源码只在 Go 根目录读取；跨仓库证据拆成独立调用。
- Verification: 该命令未修改文件；随后在 Legacy 根目录成功读取 lessons 尾部，并继续将 Go 与 Legacy 检索拆分，C# 审计范围保持不变。

### 2026-08-20 — AI=49/50 对照读取仍须保持单仓库调用边界

- Symptom: ThunderElement/GreatFoxSpirit 收尾期间，一次 Legacy 只读命令混入了 Go 相对路径；命令在读取阶段失败，同一调用前面的读取输出也不能继续作为源码证据。
- Root cause: 为连续查看两侧实现复用了另一仓库的路径，没有把 `workdir` 与命令参数绑定为单一仓库集合。
- Prevention: 跨仓库对照必须拆成独立调用；每次先核对当前 `git rev-parse --show-toplevel`，调用内只使用该仓库路径，失败调用的全部输出作废。
- Verification: 失败调用未产生写入；后续 Legacy 与 Go 查询分别在各自核验过的根目录完成，AI=49/50 判断只采用成功调用的输出。

### 2026-08-20 — AI=52/53 对照读取再次不得混入 Legacy 路径

- Symptom: EvilMir/EvilMirBody 研究期间一次 Go 只读命令夹带 Legacy `tasks` 路径并失败；该调用没有写入，整条输出不能作为源码证据。
- Root cause: 在恢复会话时复用了另一仓库的相对路径，没有把工作目录与参数集合绑定为单一仓库。
- Prevention: 每次调用只使用当前已核验仓库的路径；切换仓库必须结束调用、重新核对 `git rev-parse --show-toplevel`，任一非零读取输出整体作废。
- Verification: 本次仅写入 lessons；后续 EvilMir/EvilMirBody 对照将拆成独立 Legacy 与 Go 调用，并只采用成功调用的结果。

### 2026-08-20 — AI=52/53 Go 检索必须先核对目录清单

- Symptom: 恢复检索把不存在的 Go `worlddata` 目录作为 `rg` 参数，命令返回非零；同一调用的其他输出不能作为源码证据。
- Root cause: 依据概念目录名构造参数，没有先用当前 Go 根目录的 `rg --files`/目录清单确认实际路径。
- Prevention: 每次 Go 读取先核验根目录和精确候选路径；只传已存在的路径或让 `rg` 在已存在目录内处理模式，失败调用整体作废。
- Verification: 本次没有 Go 文件写入；后续重跑只使用已确认的 `cmd`、`internal`、`docs` 路径。

### 2026-08-24 — Source-precedence 恢复状态审计再次混仓

- Symptom: 首个 Legacy 状态调用使用 `git -C` 混读 Go 仓库。
- Root cause: 恢复模板没有在发送前机械拒绝跨仓参数。
- Prevention: 每次调用只允许当前 `workdir` 所属仓库；命令文本出现 `git -C` 或对侧根时拒绝发送。
- Verification: 整次输出已作废；双仓 HEAD/status/三类 C# 门禁随后以独立 `workdir` 零退出重跑。

### 2026-08-24 — CHAR-P3-CREATE writer 候选必须按 forbidden behavior 复审

- Symptom: bounded writer 虽只修改获授权文件，却把明确禁止的 DeleteCharacter 物理删除改成 tombstone，并让旧 CreateCharacter wrapper 保留 Go 自创的 Hero-name 冲突；missing DisabledChars loader 也没有按 Legacy 创建空文件，生产时间断言仍非确定性。
- Root cause: 只把文件集合当成 ownership 边界，没有在 worker 自报通过后立刻按 forbidden behavior、Legacy authority 和测试判据逐 hunk 复审。
- Prevention: writer 返回后、运行主线程行为门禁前，固定审查三张清单：允许文件、允许行为、显式 forbidden；wrapper/helper 也必须服从 Legacy 生产语义，不能以“历史 Go 兼容”为由保留错误；fixture 的时间/文件副作用必须确定性验证。
- Verification: 主线程在首次 diff review 即恢复 DeleteCharacter 原实现，统一两个 CreateCharacter 入口为仅检查角色名，补 missing-file 创建与 gate 时钟注入；随后 touched compile 和 focused 首跑退出 0，删除/tombstone leaf 未被实现。

### 2026-08-24 — CHAR-P3-CREATE recovery 首调用仍不得跨仓

- Symptom: 新 Session 首个诊断在 Legacy `workdir` 使用 `git -C` 核验 Go 根，违反单仓路径闭包。
- Root cause: 把“两仓启动核验”误写成一个复合命令，没有在发送前机械拒绝 `git -C`。
- Prevention: 启动恢复命令先只写当前仓清单并结束调用；收到零退出结果后才能构造下一仓命令，命令文本内禁止出现对侧根或 `git -C`。
- Verification: 整次混仓输出已作废；Legacy 与 Go 的 HEAD/status/三类 C# 门禁随后分别在各自 `workdir` 零退出重跑，准确 handoff 写入并通过控制检查。

### 2026-08-26 — Canonical C01 historical verification compacted before Mine closure

- The following pre-compaction active verification line is preserved verbatim:
- Verification: 命令零退出且所有路径属于同一根；任一读取失败、非零退出或混合根调用时，丢弃该调用的全部输出（包括前面成功的片段），不得用于实现、测试归因或文档。 本次 BaseStats 审查又在 Go workdir 的只读调用中误带 Legacy `Shared/Data/Stat.cs`，整次输出已丢弃并按两仓分别重跑。本轮跨仓库 status 审计因混入另一根路径作废，随后已拆成两次单仓调用重跑；本批一次 Legacy workdir 混入 Go 文件路径的只读调用同样整体作废，随后按仓库拆开重跑；本轮 Legacy 方向核对命令再次混入 Go 相对路径，整次输出作废，随后按仓库边界重跑。 本轮一次 Legacy 读取调用误附 Go 相对路径，整次输出再次作废并已拆分重跑；一次委派消息误将已选 AI=8 写成 AI=80，相关 AI=80 tracing 已明确丢弃并按 AI=8 重做；本轮两次继续勘察时又把 Legacy lesson/archive 路径或 Go 源码路径混入对侧 workdir，相关调用输出均作废，随后已按仓库分别重跑。 本 Session 首次恢复读取又在 Legacy workdir 的同一命令中加入 Go migration-matrix 绝对路径；整次约 8 万 token 输出已作废，并按两仓独立调用重新读取。 本批 Notice 勘察又在 Go workdir 的同一读取中附带 Legacy `Server/Settings.cs`，整次输出立即作废，随后分别在 Go 与 Legacy 根重跑并只采用独立结果。
- Mine-session recurrence: the first recovery status command again used `git -C` to mix both repositories; the entire output was discarded, and branch/HEAD/status plus all C# gates were rerun in independent repository-local calls.
- Verification: subsequent startup, matrix, source, status and C# calls used one verified root each and exited zero.

### 2026-08-26 — Hazard world patches must separate declaration and constructor anchors

- Symptom: one four-hunk `world.go` patch failed because the remembered constructor alignment did not match the formatted source, even though the three declaration anchors were otherwise current.
- Root cause: independent struct/type/constructor edits were bundled behind one stale whitespace-sensitive hunk instead of being applied from separately reread physical ranges.
- Prevention: split declarations, runtime structs and constructor literals into independent minimal patch transactions after reading each exact range; a semantic match is not a physical patch anchor.
- Verification: the failed transaction left no partial hazard fields; four independently anchored patches then applied, `gofmt` and `git diff --check` exited zero, and touched-package compile passed.

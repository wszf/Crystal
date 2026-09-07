### 2026-08-20 — Legacy 对照命令末尾混入 Go 路径时整条证据作废（再次强化）

- Symptom: EvilMirBody 对照命令在 Legacy 根目录读取成功片段后追加了 Go 的 `cmd/crystal-server/...` 路径，命令以路径错误结束；该调用没有写入，前面的 Legacy 输出也不能继续使用。
- Root cause: 为连续查看 `SpellObject` 与 Go helper 复用了跨仓库参数，未把一次工具调用绑定到唯一仓库。
- Prevention: 对照读取必须拆成独立调用；每次调用先核验 `git rev-parse --show-toplevel`，参数只允许当前仓库的已存在路径；任一非零读取的全部输出丢弃。
- Verification: 本次失败调用未改变源码；后续将分别重跑纯 Legacy 与纯 Go 读取，并只使用成功调用的结果。

- Strengthening after recurrence: 本轮读取 `ItemObject.DragonDrop` 后又在同一 Legacy 调用追加了 Go 路径，命令再次失败；即使 Legacy 片段已打印，也必须全部丢弃并重读。
- Verification after recurrence: 失败调用未写入；后续每个调用的工作目录、根目录核验和路径参数将保持单一仓库。

### 2026-08-20 — EvilMir 对照读取仍须保持单仓库 shell 边界

- Symptom: 读取 Legacy `MonsterObject.cs` 后在同一命令追加 Go `cmd/crystal-server/*.go`，zsh 因未匹配 glob 退出；该调用没有写入，整条输出不能作为源码证据。
- Root cause: 为连续查看 EvilMir 的 Go 可见性 helper 复用了当前 Legacy shell，违反了每次调用只属于一个仓库的边界。
- Prevention: 当前仓库的对照读取结束后立即结束工具调用；切换 Go 前单独核验其 `git rev-parse --show-toplevel`，Go 检索只使用已存在的 Go 路径或 `rg --glob`。
- Verification: 本次失败发生在 shell 展开/读取阶段，Legacy 与 Go 均无源码变化；后续 EvilMir 读取拆成纯 Legacy 和纯 Go 调用。

### 2026-08-17 — Go 只读调用不得混入 Legacy 相对路径（AI=171）

- Symptom: AI=171 对照读取时在 Go 根目录追加了 Legacy 的 `Shared/Enums.cs` 路径；命令在读取阶段失败，不能把同一调用的其他输出当作源码证据。
- Root cause: 为连续查看 Buff 枚举而复用了另一仓库的相对路径，没有把工作目录与参数集合绑定为单一仓库。
- Prevention: 每次工具调用只使用当前已核验根目录下的路径；切换仓库必须结束调用、重新执行 `git rev-parse --show-toplevel`，失败调用的全部读取输出作废。
- Verification: 本次命令未写入文件；后续 Legacy 的 Buff 枚举与 Go 的实现读取将拆成两个独立、纯仓库调用。
- Strengthening after recurrence: Repulsion/FireBurst 差集查询再次在 Go 根目录命令末尾混入 Legacy 相对路径，导致整条读取作废；即使前面的 Go 输出看似成功，也不得继续使用。
- Verification after recurrence: 将 Go 与 Legacy 查询拆为两个纯仓库调用并重新核对根目录后，后续实现判断只采用成功调用的输出。
- Strengthening after recurrence: CounterAttack 对照时又在 Go 命令中追加 `../Crystal/Server/...` Legacy 路径并触发读取失败；任何跨仓库参数再次出现时，整条命令输出必须丢弃，不能只忽略失败的末尾路径。
- Verification after strengthening: 随后用独立 Legacy 调用重新读取 `HumanObject.cs`，Go 调用只读取 `Crystal.GoServer` 文件；CounterAttack 顺序判断仅采用这两次成功调用的结果。

### 2026-08-17 — Go 检索先核对精确文件和根目录再使用 shell 参数

- Symptom: BoulderSpirit 调试时一次命令把未核验的 `cmd/crystal-server/session*.go` 交给 zsh，另一次把 Go 根目录重复成 `Crystal.GoServer.GoServer`；两次读取均在启动/展开阶段失败，不能提供源码证据。
- Root cause: 凭概念文件名和记忆中的绝对路径构造查询，没有先用当前 Go 仓库的 `rg --files` 与独立 `git rev-parse --show-toplevel` 建立参数边界。
- Prevention: 每次 Go 工具调用先独立核对精确根目录；目标文件先由 `rg --files` 列出，模式交给 `rg --glob` 而不是 zsh 裸展开，失败调用的其他输出整体作废。
- Verification: 后续只在核验后的 `Crystal.GoServer` 根目录读取 `main.go`、`world.go` 和已列出的 Go 文件，BoulderSpirit 运行时接线、测试和 race 门禁均来自成功调用。

### 2026-08-17 — 对照读取不得在 Legacy 命令中混入 Go 路径（再次强化）

- Symptom: HornedSorceror 对照命令在 Legacy 根目录追加了 Go `cmd/crystal-server/...` 路径，读取阶段失败；该调用没有写入，整条输出作废。
- Root cause: 连续读取两侧实现时复用了另一仓库的相对路径，没有把工作目录与参数集合绑定到同一仓库。
- Prevention: 每次工具调用只使用当前已核验根目录下的路径；切换仓库必须结束调用、重新核验根目录，再执行另一侧读取。
- Verification: 失败调用未改变工作树；后续 HornedSorceror 对照将拆成 Legacy 与 Go 两个纯仓库调用。

### 2026-08-17 — Go 根目录复用不得手工重复拼接

- Symptom: HornedWarrior 对照期间两次 Go 只读命令把已核验根目录重复成不存在的路径，进程未启动，输出不能作为源码证据。
- Root cause: 复制绝对工作目录时手工拼接了仓库路径，没有直接使用最近一次 `git rev-parse --show-toplevel` 结果。
- Prevention: 每次 Go 工具调用先独立核验根目录和目标文件；后续 `workdir` 只使用该返回根目录，禁止手工追加目录，失败读取整体作废。
- Verification: 两次失败调用均未写入；随后在独立核验的 `Crystal.GoServer` 根目录重新读取 ElephantMan/目标辅助代码。
- Strengthening after recurrence: 本批一次 `apply_patch` 仍因手工构造的绝对目标未与当前 `git rev-parse --show-toplevel` 逐字一致而在写入前失败；补丁目标必须先用当前根目录和 `test -f` 核对，再执行写入。
- Verification after recurrence: 失败补丁未产生文件变化；重新核对 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 后只修改已存在的会话测试文件，定向普通/race 回归通过。

### 2026-08-17 — Legacy 对照读取失败时整条命令证据作废

- Symptom: 一次 Legacy 对照命令末尾夹带了不存在的 Go 路径，读取阶段失败；该调用没有写入，前面的 Legacy 输出也不能继续作为实现证据。
- Root cause: 为连续查看两侧代码复用了另一仓库的相对路径，没有把工作目录与参数集合绑定到单一仓库。
- Prevention: 跨仓库对照拆成独立调用；每次先核对 `git rev-parse --show-toplevel`，命令参数只出现当前仓库路径，任一非零读取整体作废。
- Verification: 失败调用未改变工作树；随后在单独核验的 Legacy/Go 根目录分别重读，AI=164 判断只采用成功调用输出。

### 2026-08-17 — DarkCaptain 对照读取仍须保持单仓库参数

- Symptom: 一次 Legacy 只读命令在 `Crystal` 根目录夹带了 Go 的 `cmd/crystal-server/world.go`，读取在路径解析阶段失败；该调用没有写入，输出不能作为源码证据。
- Root cause: 为了连续查看 Go 的 `killMonsterLocked`，复用了另一仓库的相对路径，没有把工作目录与命令参数作为不可拆分的单仓库集合。
- Prevention: 当前仓库读取完成后必须结束调用；切换仓库先独立执行并核对 `git rev-parse --show-toplevel`，新调用的路径参数只允许当前仓库文件。失败调用整体作废。
- Verification: 本次失败调用未产生源码变化；后续 DarkCaptain Legacy 与 Go 片段将分成两次已核验根目录的调用。

- Strengthening after recurrence: 即使 Legacy 片段已经成功读出，命令末尾也不得追加 Go 文件；必须在切换根目录后重新核验，并让整条命令只包含当前仓库路径。
- Verification after recurrence: 本次混合调用在 Go 路径解析阶段失败且未写入；其 Legacy 输出也不再作为证据，后续将分别重读两侧。

### 2026-08-17 — Go 工作目录绝对路径不得重复拼接

- Symptom: 一次 Go 只读检索把已核验根目录重复成 `.../me_work/me_work/Crystal.GoServer`，进程未启动，输出不能作为源码证据。
- Root cause: 复制绝对工作目录时凭记忆再次拼接父目录，没有直接复用最近一次 `git rev-parse --show-toplevel` 结果。
- Prevention: 每次 Go 工具调用前先独立核对 `git rev-parse --show-toplevel`；后续 `workdir` 只使用该返回值，禁止手工追加仓库路径。启动失败的读取调用整体作废。
- Verification: 本次调用在进程创建阶段失败且未写入；后续将先在单独调用中核对 Go 根目录，再执行纯 Go 路径检索。

### 2026-08-17 — 跨仓库 Legacy 对照调用必须完全隔离

- Symptom: DarkCaptain 对照命令在 Legacy 根目录中夹带 Go 路径和未核验 Go glob，shell 展开失败；该调用的 Legacy 输出不能作为源码证据。
- Root cause: 为连续读取两侧实现，把另一个仓库的参数复用到当前 shell，没有把命令失败视为整条证据失效。
- Prevention: 每次工具调用只使用当前 `git rev-parse --show-toplevel` 对应仓库的路径；切换仓库必须结束调用并重新核验根目录，禁止跨仓库路径或 shell glob 混入。
- Verification: 失败调用未产生写入；随后以纯 Legacy 调用重新读取 `DarkCaptain.cs`、`MonsterObject.cs`，迁移判断只采用成功调用的输出。

### 2026-08-17 — Legacy 只读命令也不得夹带 Go 路径

- Symptom: DarkOmaKing 对照命令在 Legacy 根目录中追加了 `cmd/crystal-server/...` 路径；该部分读取失败，整条命令的输出不能作为证据。
- Root cause: 为了连续读取对照实现，复用了另一仓库的相对路径，没有把命令参数和工作目录作为同一仓库边界检查。
- Prevention: 每条跨仓库命令只允许当前根目录的路径字面量；切换仓库必须结束当前调用，先独立核对 `git rev-parse --show-toplevel`，再读取另一侧。
- Verification: 失败命令未产生文件变化；后续 DarkOmaKing 对照将拆为 Legacy 与 Go 两个独立调用，且只使用成功核对后的输出。
- Strengthening after recurrence: 即使 Go 文件只是作为后续实现参考，也不能出现在 Legacy shell 的参数中；当前仓库源码读取完成后必须结束调用，再新建已核验 Go 根目录的读取调用。
- Verification after recurrence: 本次混合命令未产生写入；其输出未用于实现判断，后续仅采用前一条成功的 Legacy DarkOmaKing 片段和独立 Go 源码读取结果。
- Strengthening after second recurrence: Go 根目录的命令也不得包含 `Server/...` 等 Legacy 路径，即使命令主体还包含合法的 Go 检索；跨仓库对照必须拆成两次调用，失败的一侧不污染另一侧证据。
- Verification after second recurrence: 本次 Go 调用中的 Legacy 路径只在读取阶段失败且没有写入；后续 DarkOmaKing 实现只使用此前成功的 Legacy片段和新的纯 Go 调用。

### 2026-08-15 — 跨仓库状态核对不得放入同一并行编排（再次强化）

- Symptom: 本轮恢复时再次把 Legacy 与 Go 的 status/diff 查询放进同一个 `Promise.all`；查询只读且没有源码写入，但违反了单仓库调用边界，不能保证后续不会把两侧结果混作证据。
- Root cause: 为降低往返延迟，把“两个命令互不写入”误当成“可以共享一次工具编排”；没有把每个 `functions.exec` cell 绑定到唯一仓库根目录。
- Prevention: 跨仓库的根目录核验、状态检查、源码读取、格式化、测试和写入都按仓库使用独立工具调用；每次调用只允许当前仓库的路径参数，并在返回后核对 `git rev-parse --show-toplevel`。禁止在同一个 `Promise.all`、shell 或路径变量中混放两侧。
- Verification: 本次并行查询没有产生文件变化；后续 Legacy lessons 完整读取已在单独调用完成，后续 Go 测试与文档操作将只在独立核验的 Go 根目录调用中执行。

### 2026-08-15 — 已核验 Go 根目录不得手工重复拼接

- Symptom: ElephantMan session 对照期间一次只读命令把已核验的 Go 根目录手工重复成不存在的路径，进程未启动，不能使用其输出作为源码证据。
- Root cause: 复制完整绝对工作目录时凭记忆重复了仓库名，没有在新调用中直接复用最近一次 `git rev-parse --show-toplevel` 的结果。
- Prevention: 每次 Go 工具调用先在独立调用中核对完整根目录和目标文件存在性；后续 `workdir` 只使用该返回值，禁止手写拼接。启动失败的调用整体作废，不据其错误文本推断源码状态。
- Verification: 本次命令在进程创建前失败且没有文件变化；随后在正确的 `Crystal.GoServer` 根目录重新读取并完成 ElephantMan session 测试。

### 2026-08-15 — Legacy 检索不得使用未核验的 shell glob

- Symptom: 查找 AI=139 时把不存在的 `Server/MirObjects/Mob*.cs` glob 交给 zsh，命令在展开阶段失败；没有源码写入，失败调用的部分输出无效。
- Root cause: 依赖文件名习惯追加 glob，没有先用当前仓库的 `rg --files` 确认目录和匹配文件。
- Prevention: Legacy 检索只使用已验证的相对路径；需要模式匹配时使用 `rg --files -g` 先取得精确清单，或让 `rg` 自己处理 pattern，禁止让 shell 展开未核验的 glob。出现 shell 非零读取错误时整体作废输出。
- Verification: 失败命令未启动源码读取且工作树无变化；随后只在已核验的 `Server/MirObjects` 路径中重跑 AI=139 检索并取得 StoneGolem 基线。

### 2026-08-15 — Legacy 对照路径不得带入 Go 工作目录（AI=139 复发）

- Symptom: 读取 StoneGolem 继承的 `FindTarget` 时把 Legacy `Server/MirObjects/MonsterObject.cs` 路径放进 Go 根目录，命令返回路径不存在；没有写入，混合调用输出全部无效。
- Root cause: 在同一迁移分析中切换对照侧时复用了上一仓库的相对路径，没有把工作目录与路径参数作为一个不可拆分的单仓库调用。
- Prevention: Legacy 源码读取必须在独立、已核验的 Legacy 根目录调用中完成；Go 调用参数只允许 Go 路径。任一混合或非零读取调用整体作废，禁止采用其余部分输出。
- Verification: 失败命令未启动源码读取且两个工作树无源码变化；后续将在 Legacy 独立调用重新获取 `FindTarget`，再以独立 Go 调用读取对应运行时 helper。

### 2026-08-15 — 跨仓库只读调用的路径边界仍需逐调用核验

- Symptom: ElephantMan 对照期间两次只读检索把错误仓库路径带入命令：一次工作目录拼错，另一次在 Go 根目录检索了 Legacy `Server/...` 路径；命令均在读取阶段失败，没有源码写入。
- Root cause: 复用上一条调用的路径片段时，没有把当前仓库根目录和相对路径作为同一组重新核对。
- Prevention: 每次跨仓库调用先独立执行 `git rev-parse --show-toplevel`，成功后命令参数只允许出现当前仓库相对路径；切换仓库必须结束当前调用，不能把另一仓库路径放入同一 shell 或编排。
- Verification: 两次失败命令均未启动/未写入；后续 Legacy 与 Go 查询拆分并分别核对根目录，迁移判断只使用成功调用的输出。

### 2026-08-15 — Legacy 对照路径不得放入 Go 工作目录

- Symptom: 一次只读命令在 Go 根目录检索 `Server/MirObjects/HumanObject.cs`，因路径不存在失败；没有产生写入，输出不能用于判断。
- Root cause: 对照 Legacy 与 Go 的命令参数复用了另一仓库的相对路径，未让工作目录和路径集合保持同仓库。
- Prevention: 每个仓库读取先在独立调用核对 `git rev-parse --show-toplevel`，随后只使用该仓库相对路径；跨仓库对照必须拆成新的调用。
- Verification: 失败命令只发生在读取阶段，Go 与 Legacy 工作树无源码变化；后续 AI=137 判断仅使用已核对仓库的输出。
- Strengthening after recurrence: 即使只是补充对照，Go 调用中也不得出现 `Server/...` 路径；需要读取 Legacy 文件时必须先结束 Go 调用，再独立核对 Legacy 根目录后执行。
- Verification after recurrence: 本次命令在 Go 根目录只返回 Legacy 路径不存在，未产生写入；后续不使用该输出，并将 Legacy 对照拆为独立调用。

### 2026-08-15 — 跨仓库补丁目标与检索参数必须再次核验

- Symptom: AI=136 工作中一次 apply patch 手工重复了 Go 仓库目录，另一次 Go 只读命令夹带了 Legacy `Server/...` 路径；命令未产生源码写入，但其失败输出不可用于判断。
- Root cause: 复用上一调用的绝对路径/对照路径，没有在新调用前把 `git rev-parse --show-toplevel`、工作目录和参数重新作为单仓库集合核对。
- Prevention: 每次切换仓库先独立打印根目录；随后源码检索和补丁参数只出现当前仓库路径，补丁前对每个绝对目标执行存在性核验，禁止凭记忆拼接目录。
- Verification: 错误调用均在读取/补丁验证阶段停止且工作树无新增目标文件；之后仅在核验后的 Go 根目录完成 AI=136 修改。

### 2026-08-15 — Go 仓库命令的绝对路径需避免手工重复目录

- Symptom: 一次只读 `sed`/`rg` 调用把 Go 根目录重复写成不存在的路径，命令未启动，不能使用其输出判断 stat 常量。
- Root cause: 切换到已核验 Go 根目录后复制路径时又手工拼接了 `me_work` 目录。
- Prevention: 每次 Go 工具调用先在独立调用中执行 `git rev-parse --show-toplevel`；随后只使用该调用返回根目录下的相对路径，禁止凭记忆拼接绝对路径。
- Verification: 错误调用在进程创建前被拒绝且无文件变化；随后在正确 Go 根目录读取常量并完成回归修复。

### 2026-08-15 — 跨仓库命令中的工作目录必须逐字核验

- Symptom: 一次 Go 只读核对把已知根目录误拼成不存在的重复路径，命令未启动，不能使用其输出判断代码状态。
- Root cause: 复制完整绝对路径时手工重复了仓库目录，没有在新调用中重新核对工作目录。
- Prevention: 每次切换仓库都先单独运行 `git rev-parse --show-toplevel`；随后调用只使用该次返回的根目录，禁止凭记忆或拼接路径继续执行。
- Verification: 本次错误命令在进程创建前被拒绝且没有文件变化；随后重新核对 Legacy 根目录并只在正确仓库记录本 lesson。
- Strengthening after recurrence: 本轮在 Legacy 根目录读取状态后又把 Go 专属的 `docs/migration-matrix.md` 作为相对路径检索，命令以路径不存在退出；即使文档路径名称看似通用，也必须先按仓库实际文件清单确认归属，不能把失败命令的其他输出当作证据。
- Verification after recurrence: 该调用只读且没有文件变化；随后先独立核验 Go 根目录，再从 Go 仓库读取迁移矩阵，错误输出未用于 AI=130 判断。

### 2026-08-15 — 只读对照命令的工作目录与路径必须同仓库

- Symptom: 一次 Legacy 读取命令使用了 Go 的 `cmd/crystal-server/monster_ai.go` 路径，只返回路径不存在；没有写入，但不能把该命令的输出用于行为判断。
- Root cause: 在准备 Legacy/Go 对照时复用了上一条命令的路径片段，没有把 `workdir` 与相对路径作为不可分割的一组重新核对。
- Prevention: 每次跨仓库读取先单独执行并核对 `git rev-parse --show-toplevel`，随后命令只出现当前仓库的相对路径；另一仓库必须在新的调用中读取。
- Verification: 本次错误命令在读取阶段失败且工作树无变化；后续先在 Go 根目录独立读取 AI 代码，Legacy 对照命令仅使用 `Server/...` 路径。
- Strengthening after recurrence: 本轮在 Legacy 根目录核对 Harvest 基线时又追加了 Go 的 `cmd/crystal-server` 路径，导致整条读取命令失败；即使 Legacy 前半段输出看似完整，也不得继续采用。
- Verification after recurrence: 本次调用只读且没有文件变化；后续 Harvest 对照将拆成纯 Legacy 与纯 Go 两次调用，并在每次调用前重新核对根目录。
- Strengthening after second recurrence: 同一 Harvest 对照中又把 Go 的 `internal/protocol/packet_test.go` 路径追加到 Legacy 命令；任何跨仓库路径一旦出现，整条命令仍必须丢弃，不能只保留前面的 Legacy 输出。
- Verification after second recurrence: 调用只读且未改动文件；后续先完成纯 Legacy 读取，再结束调用并独立核对 Go 根目录后读取协议实现。

- Strengthening after recurrence: 跨仓库补丁目标也必须在调用前用完整绝对路径核验；少拼一段目录的目标会在 apply 阶段失败，不能依赖工具错误文本代替路径检查。
- Verification after recurrence: 本次 Go 测试期望补丁因缺少 `me_work` 目录在写入前被拒绝；随后将所有目标固定为已核验的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer/...` 路径。

- Strengthening after second recurrence: 已核验的仓库根目录不能手工重复或变形；每次新工具调用仍先使用不带源码路径的 `git rev-parse --show-toplevel`，成功后才读取文件。
- Verification after second recurrence: 本次错误的 `Crystal.GoServer.GoServer` 工作目录在进程启动前失败且无文件变化；后续调用恢复为完整单一 Go 根目录。

### 2026-08-14 — Go 工作目录中不得携带 Legacy 相对路径

- Symptom: 在 Go 仓库只读核对时把 `Server/...` 的 Legacy 检索路径放进同一命令，命令只返回路径不存在，未产生写入，但输出不能作为语义判断依据。
- Root cause: 切换仓库后仍复用了上一阶段的相对路径模式，没有让每个 shell 调用的参数只属于当前 `workdir`。
- Prevention: 跨仓库检索必须拆成独立调用；每次调用先核对 `git rev-parse --show-toplevel`，随后只使用该仓库的相对路径，禁止在 Go 命令中出现 Legacy 路径字面量。
- Verification: 本次错误调用在读取阶段失败且两个工作树无新增改动；后续 Legacy 与 Go 查询分开执行，判断只采用各自仓库的输出。

### 2026-08-14 — Legacy shell 也不得夹带 Go 路径

- Symptom: Legacy 只读命令末尾继续检索 `cmd/crystal-server/...`，该部分只返回路径不存在，没有写入，但违反了单仓库调用边界。
- Root cause: 在同一 shell 中完成源码读取和另一仓库对照，忽略了“每次命令参数只属于当前根目录”的约束。
- Prevention: Legacy 命令结束后必须终止调用；任何 Go 对照都在新的调用中先核对 Go 根目录，再使用 Go 相对路径，禁止用分号或同一编排串接两仓库路径。
- Verification: 本次 Go 路径错误发生在读取阶段且工作树未变化；随后将 Legacy/Go 查询拆为两个独立调用，并分别核对根目录。

### 2026-08-14 — 跨仓库只读检索必须显式固定工作目录

- Symptom: 两次只读 `rg` 检索及一次补丁命令把原 Crystal 路径和 Crystal.GoServer 路径混用/重复拼接，输出或命令来自错误路径，增加了判断迁移状态的风险。
- Root cause: 并行检索与补丁调用中没有复核绝对工作目录，且把仓库路径再次拼进了已绝对化的文件路径。
- Prevention: 跨仓库查询一律使用绝对 `workdir`，同一批次先分别打印目标仓库的 `git status`，补丁目标只使用已核验的绝对路径；禁止凭相对路径或重复拼接推断结果属于哪个仓库。
- Verification: 本批次已分别在两个绝对工作目录执行状态检查，确认原仓库仅追加本 lessons 记录，Go 仓库仅包含 Observer 未提交改动；错误路径命令均在创建/修改前失败且未改动文件。
- Strengthening after recurrence: 即使同一批次已经固定了两个仓库的 `workdir`，补丁仍可能因手工复制绝对路径时漏掉中间目录而指向不存在的仓库；跨仓库修改前必须对每个补丁目标执行 `test -f`/`git rev-parse --show-toplevel`，失败时先停止补丁，不得依赖 apply 工具的部分输出判断是否已修改。
- Verification after strengthening: 本批次先分别读取 Legacy lessons 与 Go 状态，再用完整 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 根路径核验；一次错误的缺少 `me_work` 目标在写入前被拒绝，随后所有 Go 补丁均在核验后的绝对路径成功应用，两个仓库的 C# 差异/未跟踪检查保持为空。
- Strengthening after second recurrence: 并行只读查询仍可能把已核验的 Legacy `workdir` 误配给 Go 命令；查询也必须按仓库分组逐项执行，或在每个结果中同时核对 `git rev-parse --show-toplevel`，禁止仅依赖命令返回的错误文本判断目标仓库。
- Verification after second strengthening: 本次错误查询只在读取阶段返回 `cmd/crystal-server: No such file or directory`，没有创建或修改文件；随后改用完整 Go 根路径查询 Legacy/Go 代码，目标和工作树均已复核。
- Strengthening after third recurrence: 禁止在同一个 `Promise.all`/并行工具调用中混放两个仓库的命令；跨仓库检索必须拆成独立调用，先执行 `git rev-parse --show-toplevel` 并核对期望根目录，再读取源码。
- Verification after third strengthening: 本次 Legacy 查询在 Go 根目录仅返回路径错误且无写入；后续改为单仓库调用前先核对根目录，未再使用错误路径结果作为实现依据。
- Strengthening after fourth recurrence: 单次 shell command 也不得混合 Legacy 源码路径和 Go 源码路径；每次调用只允许使用一个仓库的路径字面量，另一仓库必须在新的、独立且已核验根目录的调用中检索。
- Verification after fourth strengthening: 本次错误的 Go 路径只在 Legacy 根目录返回不存在，未产生写入；后续仓库查询拆为独立调用并先验证根目录。
- Strengthening after fifth recurrence: 命令参数本身也必须单仓库化；即使 `workdir` 正确，`rg`/`sed` 的路径模式不得包含另一仓库的目录。先完成当前仓库检索，再在新调用切换根目录。
- Verification after fifth strengthening: 本次 Go 调用中的 Legacy 路径模式只返回不存在且无写入；后续不再把跨仓库路径放入同一命令，源码判断仅使用当前仓库结果。
- Strengthening after sixth recurrence: 即使命令主体只读，单仓库调用中也不得在 Legacy 根目录拼接或检索 Go 路径；每次切换仓库前重新执行 `git rev-parse --show-toplevel`，并让命令参数只包含当前根目录下的相对路径。

### 2026-08-17 — Legacy 对照命令不得夹带 Go shell 模式

- Symptom: 读取 Legacy `MonsterObject.cs` 时把 Go 的 `cmd/crystal-server/*.go` 模式放进同一 shell；zsh 在展开阶段失败，整条命令输出不能作为证据。
- Root cause: 为了同时查看两个实现，混用了当前仓库的工作目录和另一仓库的相对模式，没有把 shell 参数作为单仓库边界检查。
- Prevention: Legacy 调用只允许 `Server/...` 路径和已存在的 Legacy 文件；Go 源码必须在结束该调用后，单独核验 Go 根目录再读取。禁止在任一 shell、并行编排或路径变量中混放两侧模式。
- Verification: 失败调用未产生文件变化，输出未用于实现判断；后续将分别在已核验的 Legacy 与 Go 根目录重跑所需片段。
- Verification after sixth strengthening: 本次错误的 Legacy 调用只在读取阶段返回 Go 路径不存在，没有写入；随后改为先独立核验 Go 根目录再查询，迁移判断未使用错误输出。
- Strengthening after seventh recurrence: Legacy 根目录下的检索命令即使只是比较 Go 侧已有符号，也不得包含任何 Go 路径或 glob；先结束 Legacy 只读核对，再以新的、单独核验根目录的 Go 调用继续，禁止在同一 shell 中跨边界。
- Verification after seventh strengthening: 本次 Legacy shell 中误带的 Go glob 只返回 shell 的未匹配错误，没有写入或 C# 变化；后续实现判断不使用该输出，并恢复为单仓库调用。
- Strengthening after eighth recurrence: 即使前一条规则已要求单仓库命令，工具编排仍可能在参数层把另一仓库的绝对路径带入当前调用；跨仓库任务必须把“根目录核验、源码读取、写入”拆成独立调用，并在每次返回后核对根目录，禁止复用上一调用的路径变量。
- Verification after eighth strengthening: 本轮错误的跨仓库查询只在读取阶段返回路径不存在且没有文件写入；随后用两个独立的绝对根目录调用完成判断，Go 改动与 Legacy lessons 均落在预期仓库，C# 状态未变。
- Strengthening after ninth recurrence: 用户指定的当前目录是 Legacy 根目录时，Go 测试/检索调用仍可能沿用该目录；执行任何 Go 命令前必须先在独立调用中打印并核对 Go 仓库的 `git rev-parse --show-toplevel`，随后才使用 Go 相对路径，不能仅凭“上一批次已知根目录”继续。
- Verification after ninth strengthening: 本轮在 Legacy 根目录运行 Go 命令只返回无 module/路径不存在且无写入；随后切换到已核对的 Go 根目录，Tucson 定向测试、net.Pipe transcript 和 Go 代码读取均来自正确仓库。

### 2026-08-18 — 跨仓库检索命令必须按仓库拆分

- Symptom: 继续筛选下一法术时，把 Legacy `Server/...` 路径追加到 Go 根目录的只读命令，末尾 `sed` 报路径不存在；该调用的全部混合输出作废。
- Root cause: 为连续查看两侧代码，把相对路径和 `workdir` 绑定错误，破坏了单仓库调用边界。
- Prevention: 一次工具调用只允许一个仓库；调用前先验证并复用精确的绝对 `workdir`；命令正文只出现该仓库已确认存在的相对路径；对侧研究另起独立调用，失败调用的输出不作为源码证据。
- Verification: 随后 Legacy 只读调用单独读取 `HumanObject`/`Map`/`SpellObject`，Go 只读调用单独读取 `world.go`；Mirroring 研究中一次重复 `Dropbox` 的错误 `workdir` 被识别为无效并丢弃，重试使用精确根目录成功；另一次把 Go 路径混入 Legacy 调用时，首个 `sed` 失败，后续看似成功的 Legacy 片段也按规则全部丢弃；未发生文件变化。

### 2026-08-15 — 跨仓库检索参数不得携带另一仓库路径（AI=135 复发）

- Symptom: 一次 Go 仓库只读检索命令仍带有 Legacy `Server/...` 路径；命令只返回路径不存在，没有写入，但其输出不能作为语义判断依据。
- Root cause: 查询多个对照点时复用了上一条 Legacy 命令的路径字面量，没有把当前 `workdir` 与命令参数作为单仓库边界一起校验。
- Prevention: 每次跨仓库读取必须拆成独立调用，先执行并核对当前仓库的 `git rev-parse --show-toplevel`，随后命令参数只使用当前仓库相对路径；另一仓库必须在新的调用中读取。

### 2026-08-15 — AI=136 研究期间 Go 命令不得混入 Legacy 路径

- Symptom: AI=136 研究期间两次 Go 仓库只读命令混入了 Legacy `Server/...` 路径；命令返回路径不存在，未产生写入，但输出不能用于行为判断。
- Root cause: 在跨仓库对照时复用了上一条 Legacy 命令的相对路径，没有把当前 `workdir` 与命令参数作为同一仓库边界重新核验。
- Prevention: 每次切换仓库先独立执行并核对 `git rev-parse --show-toplevel`，随后命令参数只使用该仓库的相对路径；Legacy 与 Go 读取必须拆成独立调用。
- Verification: 错误命令均在只读阶段失败且两个工作树无文件变化；后续先核验 Go 根目录，再只使用 Go 路径读取 AI=136 相关代码。
- Verification: 本次错误命令在读取阶段失败且两个工作树无源码变化；记录后继续实现前已恢复为单仓库调用，并将错误输出排除在判断之外。

- Strengthening after recurrence: 工具调用的绝对 `workdir` 也必须逐字使用最近一次 `git rev-parse` 的结果；少一段目录会在进程启动前失败，不能用错误文本替代根目录核验。
- Verification after recurrence: 本次拼写错误的工作目录在进程创建前被拒绝且无文件变化；随后先独立核对 Legacy 根目录，再继续只读对照。
- Strengthening after second recurrence: 即使当前 `workdir` 已是 Go 根目录，命令参数也不得包含 Legacy `Server/...` 路径；跨仓库对照必须拆为独立调用，并在命令返回前不复用另一仓库的路径变量。
- Verification after second recurrence: 本次 Go 只读检索仅因混入 Legacy 路径返回不存在，未产生文件变化；随后切回 Legacy 追加本 lesson，后续实现调用将只使用 Go 相对路径。

### 2026-08-13 — C# 源码与既有工具必须保持只读基线

- Symptom: 为公会仓库补协议向量和导出字段时，直接修改了迁移仓库中的两个 `.cs` 工具文件，破坏了用户用于逐项对照的 C# 基线。
- Root cause: 把“C# 可作为迁移辅助工具”误当成了允许继续演进 C#；实际上服务端、测试客户端、协议探针和数据转换工具都属于待迁移范围。
- Prevention: 两个仓库中的所有 `.cs` 一律只读，不新增、不修改、不删除、不重命名；只可读取作为行为证据。运行时、测试客户端、协议探针、导入/导出及其他迁移工具全部用 Go 实现。每次提交前分别执行工作区 diff、暂存区 diff 和未跟踪文件三项 `.cs` 检查，结果必须全部为空。
- Verification: 已用反向补丁精确撤销本批两个未提交 C# 差异，并把语言边界固化到 `AGENTS.md`；用户再次明确工具也必须用 Go 后，本批只修改 Go、Markdown 和 Git 元数据，提交前继续以两仓库六项 `.cs` 零输出门禁验收。

### 2026-08-14 — 跨仓库证据读取必须禁止参数层混入另一侧路径

- Symptom: 一次 Go 仓库只读检索在命令尾部混入 Legacy `Server/Shared` 路径，因目标不存在退出；该输出不能作为证据。
- Root cause: 命令编排时只复核了 workdir，没有在执行前逐项检查所有路径参数属于同一仓库。
- Prevention: 每个源码证据调用固定一个仓库根目录和该仓库已由 `rg --files` 确认的相对路径 allowlist；跨仓库读取必须使用新的独立工具调用，任何退出码 2 的混合命令结果立即作废。
- Verification: 本次错误命令未写入文件；后续 Legacy 与 Go 查询分别在各自绝对根目录重跑，修改前继续执行双仓库 status 与 C# 零差异检查。
- Strengthening after immediate recurrence: 即使 workdir 已正确固定，命令参数仍可能把另一仓库的路径追加到同一调用；参数 allowlist 必须在执行前逐项检查，Go 调用禁止出现 `Server/`、`Shared/`、`Client/`，Legacy 调用禁止出现 `cmd/`、`internal/`、Go 文档路径。
- Verification after strengthening: 本次混合命令只在读取阶段失败，没有写入；后续恢复时将 Legacy 与 Go 查询放入不同的独立工具调用，任何退出码 2 的结果不参与实现判断。

### 2026-08-15 — 跨仓库状态查询不得放入同一并行编排

- Symptom: 本批恢复时把 Legacy 与 Go 的 status/diff 查询放入同一个 `Promise.all`；查询本身只读且没有写入，但违反了已建立的仓库边界规则，增加了把错误工作目录结果混作证据的风险。
- Root cause: 只按工具调用延迟做并行化，没有把跨仓库隔离视为每个调用都必须独立核验的约束。
- Prevention: Legacy 与 Go 的根目录核验、源码读取、状态检查和写入全部使用独立工具调用；每次调用只允许当前仓库的路径参数，并在结果中核对 `git rev-parse --show-toplevel`，禁止把两侧命令放进同一 `Promise.all`。
- Verification: 本次混合查询未产生文件变化；改进后的后续读取按仓库串行执行，并分别核对根目录与工作树状态。

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

### 2026-08-11 — P6 跨仓库目标和 patch 上下文必须双重确认

- Symptom: 继续 P6 物品迁移时，曾因手写相似仓库路径把 patch 目标指向错误位置；本轮物品 ordinal patch 又因上下文行缺少 patch 必需的前导空格而被拒绝。
- Root cause: 跨仓库编辑没有把已确认的绝对仓库根目录和 apply_patch hunk 语法作为调用前的机械检查项。
- Prevention: 修改前先用 `git rev-parse --show-toplevel`/目标文件存在性确认仓库；patch 数组逐行检查 Begin/End、`@@` 和上下文首字符，失败后立即查看目标仓库 `git status` 与 diff。
- Verification: 物品协议测试和文档最终都落在 `Crystal.GoServer`，原仓库无误写；ordinal 定向测试、Go 全量校验和 `git diff --check` 作为提交前门禁。

### 2026-08-11 — 装备阶段跨仓库 patch 目标复发后必须先锁定根目录

- Symptom: 在原 Crystal 仓库工作目录下首次补协议时，patch 错把目标写成 `internal/protocol/item_actions.go`，未命中同级 Go 仓库。
- Root cause: 已知 Go 仓库是同级目录，却没有在 apply_patch 调用中使用已验证的 `../Crystal.GoServer/...` 目标。
- Prevention: 每次跨仓库编辑前执行 `git rev-parse --show-toplevel`，再把同一绝对根目录转换为 apply_patch 的相对目标；工具失败后立即检查两仓库 status。
- Verification: 改用 `../Crystal.GoServer/...` 后所有装备源码、测试和文档均落在 Go 仓库，原仓库没有误写代码。

### 2026-08-11 — Go 与 C# 工具链必须按文件类型隔离

- Symptom: 迁移过程中曾把 C# 导出器文件交给 `gofmt`，命令报语法错误；Go 源码本身没有问题。
- Root cause: 批量格式化命令使用了过宽的路径集合，没有先按扩展名和语言工具链划分目标。
- Prevention: `gofmt` 只接收 Go 文件/Go 目录；C# 只在检测到 `dotnet`/C# 工具链后使用对应格式化或构建命令，不能用 Go 工具替代静态验证。
- Verification: 本轮只对 Go 目录执行 `gofmt`；C# exporter 仅做最小字段 diff 检查，并明确保留 .NET SDK 环境下的编译验证边界。

### 2026-08-11 — 跨仓库绝对路径必须复用已验证根目录

- Symptom: 本轮补 Trade 溢出邮件时，apply_patch 首次把 Go 仓库路径写成了重复目录，工具找不到目标文件，补丁未落地。
- Root cause: 已知 Go 仓库真实根目录为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，但调用时凭记忆手写路径，未在 patch 前交叉验证目标文件。
- Prevention: 跨仓库修改前先执行 `git rev-parse --show-toplevel` 和目标文件存在性检查；apply_patch 只使用该输出生成的绝对路径，失败后立即检查两仓库 status，禁止凭工具错误/成功推断源码状态。
- Verification: 改用已验证路径后 StoredMail、auth 持久化、Trade fallback 和测试均正确落在 Go 仓库；定向 Go 测试、竞态测试和 `git diff --check` 通过。

### 2026-08-11 — 跨仓库只读检索也必须使用对应根目录

- Symptom: 本轮把原版 `Shared`/`Server` 检索路径放在 Go 仓库工作目录下，命令报路径不存在；源码没有被修改，但检查结果不完整。
- Root cause: 只确认了 Go 仓库的 workdir，没有为同一批跨仓库命令分别绑定原版 Crystal 根目录和 Go 根目录。
- Prevention: 跨仓库读写前分别执行 `git rev-parse --show-toplevel`，并让每条命令的 workdir 与目标文件所属仓库一致；不要在一个仓库 workdir 中访问另一个仓库的相对路径。
- Verification: 后续原版查询统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，Go 查询统一使用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，并在修改前检查两个仓库的 status。
- Strengthening after recurrence: 禁止在同一条 `exec_command` 中混合两个仓库的相对路径；需要跨仓库时拆成独立调用，并在输出中保留仓库标识。
- Second strengthening after recurrence: 并行调用也不能靠“同一批任务”推断 workdir；构造每个调用后先机械核对其路径只属于该调用的仓库。本次 P9 查询把 Go 测试路径和原版 `Libraries` 路径放进 Crystal workdir，命令整体无结果；拆分为两个绝对 workdir 后再继续，防止把空检索误当成不存在。
- Third strengthening after recurrence: 即便查询结果逻辑相关，也不能把原版源码读取与 Go 标识符检索合并成一条命令；每次调用在执行前按“命令中的每个相对路径都属于 workdir”逐项验收。本次 Conquest 核对把 `Server/MirObjects/GuildObject.cs` 放进 Go workdir，命令只读失败且无文件改动；已拆成两个仓库各自独立重跑。
- Fourth strengthening after recurrence: 同一仓库的搜索目录也必须先由 `rg --files` 或已验证路径清单确认；任一不存在的目录都会让 `rg` 以 2 退出，即使同时打印了其他目录的部分结果，也不能当作完整证据。本次误带不存在的 `Libraries` 后，已仅用确认存在的 `Server`/`Shared` 重跑 Conquest 查询。
- Fifth strengthening after recurrence: 即使同一查询只想并列核对 Go 与 Legacy 常量，也不能在 Go workdir 的 `rg` 参数中附带 `Server/...cs`；本批核对 ControlPoints 上限时该混用再次以 2 退出。以后先按仓库构造两份路径清单并分别调用，当前已在 Crystal 根目录单独重跑 `MAX_KING_POINTS/MAX_CONTROL_POINTS` 查询并取得完整结果。
- Sixth strengthening after recurrence: 已确认仓库根目录仍不代表任意常见子目录存在；本批在原版 Crystal workdir 的检索参数中误带 Go 专属 `cmd`，使 `rg` 以 2 退出。每次多目录搜索前必须先用该仓库的 `rg --files` 生成实际顶层路径集合，命令参数只能从集合中选择；当前已移除 `cmd` 并在原版 `Server/MirObjects`、`Shared` 与 Go 仓库各自独立重跑相关查询。
- Seventh strengthening after recurrence: 本批只读分析又在 Go 仓库搜索不存在的 `tasks`，并在 Go workdir 的同一命令中混入原版 `Server/MirObjects/PlayerObject.cs`。今后每条检索命令先在该命令自己的 workdir 用 `rg --files` 确认全部目标路径；命令参数只能来自这份清单，Go 与 Legacy 即使逻辑相关也必须拆成两个独立调用，任何 `rg` 退出码 2 都视为证据无效并在正确根目录完整重跑。
- Verification after seventh recurrence: 已在 Crystal 根目录确认 `tasks/lessons.md` 与原版 `Server` 路径，在 Crystal.GoServer 根目录单独确认 Go 源码路径；后续区域魔法核对将按仓库独立执行，不再混合相对路径。
- Eighth strengthening after immediate recurrence: 追加第七次规则后的下一条调用仍把 Legacy `Server` 检索附在 Go 协议读取命令后，说明仅靠执行前自然语言提醒无效。此后在提交每个 `exec_command` 前执行前缀 allowlist 检查：Crystal.GoServer 命令出现 `Server/`、`Shared/`、`Client/` 或 `tasks/` 时禁止执行；Crystal 命令出现 `cmd/`、`internal/` 或 Go 文档路径时同样禁止执行。逻辑相关的读取也必须由两个独立 tool call 承载。
- Verification after eighth recurrence: 失败调用只完成了 Go 文件读取，Legacy 查询以退出码 2 明确作废；`BeginMagic` 证据已改在 Crystal 根目录的独立调用中重跑，后续命令逐条应用仓库前缀 allowlist。
- Ninth strengthening after recurrence: FrostCrunch 复核时仍在 Crystal workdir 的多段 `sed` 末尾误附 Go 路径 `cmd/crystal-server/world.go`，最终退出码 2。以后一个 `exec_command` 只允许一个仓库的一组已存在前缀；准备调用时先按 workdir 写出允许前缀（Crystal: `Server/Shared/Client/tasks`，Go: `cmd/internal/docs/README.md`），参数中出现另一组即拆成下一次调用，禁止在已写好的命令尾部追加跨仓库路径。
- Verification after ninth recurrence: 混合调用结果已作废；Legacy `MonsterObject.ProcessPoison`/`MapObject.ApplyPoison` 与 Go `worldMonster`/AI 字段读取分别在各自根目录重跑并取得 `exit_code=0`，后续修正也仅写入所属仓库。
- Tenth strengthening after recurrence: 自增益技能下一批分析时又在 Crystal.GoServer workdir 查询 Legacy `Server/...`，说明仅靠人工前缀复核仍会漏过临时追加的参数。此后 Legacy 与 Go 证据读取不仅分调用，还分不同的 `functions.exec` cell；每个 cell 只允许固定仓库根目录，调用前以该根目录的 `rg --files` 结果确认目标存在，禁止在同一 JavaScript 数组中编排两个仓库的源码查询。
- Verification after tenth recurrence: 该退出码 2 的结果已作废；本轮恢复迁移前分别确认 Crystal 仅使用 `Server/Shared/Client/tasks`，Crystal.GoServer 仅使用 `cmd/internal/docs/README.md`，后续 ProtectionField/Rage/SwiftFeet 证据将按独立 cell 读取，并在两个仓库分别检查 `.cs` 零变化。
- Eleventh strengthening after recurrence: 本批虽先由 `rg` 找到 Buff 定义位于 `Server/MirDatabase/BuffInfo.cs`，随后仍凭命名习惯追加了不存在的 `Buff.cs`；Go 查询又把未匹配的 shell `*catalog*_test.go` 直接作为参数，分别造成退出码 1/2。以后读取文件前只使用同一仓库 `rg --files -g '<pattern>'` 实际返回的精确路径；文件筛选使用 `rg -g`，禁止 shell 展开未验证的 glob，也禁止把已成功的 `sed` 与猜测性检索串成一条命令。
- Verification after eleventh recurrence: Buff 模型已改用确认存在的 `Server/MirDatabase/BuffInfo.cs` 独立重读；Go 测试文件清单已用 `rg --files cmd/crystal-server -g '*magic*test.go'` 验证，后续实现只修改清单中存在的文件或通过 `apply_patch` 明确新建文件。

### 2026-08-11 — TownRevive 回归验证必须检查 fixture 与仓库边界

- Symptom: PK Town 配置测试初版把换行写成字面量标记；本轮多个 `functions.exec` patch wrapper 又出现漏引号/漏 hunk 标记；一次只读检索还使用了不完整的 Go 仓库路径。
- Root cause: 测试 fixture、JavaScript patch 字符串和跨仓库 workdir 都依赖手写文本，调用前没有逐层检查实际文件内容、hunk 首字符和仓库根目录。
- Prevention: 写入后用源码读取确认 fixture 的真实转义层级；patch 先逐行检查字符串引号、`*** Begin/End Patch`、`@@` 及上下文前导空格；跨仓库命令分别使用已验证的绝对根目录，禁止混用相对路径。
- Verification: 修正 fixture 后 config/TownRevive 定向测试和 Go 全量 test/race/vet/build 通过；每次工具调用后均检查对应仓库 status/diff，未发生跨仓库误写。

### 2026-08-11 — 文档检索也必须复用已确认的仓库根目录

- Symptom: 本轮 Go 文档检索命令把工作目录手写成重复的 `me_work` 路径，进程未能启动，文档检查被延迟。
- Root cause: 只记住了仓库名称，没有复用前面 `git rev-parse --show-toplevel` 得到的绝对路径。
- Prevention: 所有跨仓库命令（包括只读的 README/文档检索）统一使用已验证的绝对根目录；命令失败后先检查路径和两仓库 status，再继续。
- Verification: 改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 后 README/迁移矩阵检查完成，未产生文件修改。

### 2026-08-11 — 跨仓库并行查询必须按调用拆分

- Symptom: 本轮并行核对 NPC 技能时，其中一条 Go workdir 命令混入了原版 `Shared/Enums.cs` 路径，查询报文件不存在；源码没有被修改，但原版证据检查被打断。
- Root cause: 并行命令数组只统一设置了一个 workdir，却把两个仓库的相对路径放进同一批查询中。
- Prevention: 同一批跨仓库查询也必须按仓库拆成独立调用；Go 调用只使用 Go 相对路径，原版调用只使用 Crystal 相对路径，并在输出中保留仓库标识。
- Verification: 后续原版 enum/NPC 动作查询改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，Go 协议/运行时查询改用 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`。

### 2026-08-12 — C# 顶层导出器新增显式参数类型时必须核对命名空间

- Symptom: 公会 creation-cost helper 使用了 `GuildItemVolume` 作为参数类型，而该类型声明在 `Server.MirObjects`，原有 exporter imports 不包含该命名空间。
- Root cause: 调用点可通过类型推断访问成员，但抽成具名 helper 后需要编译器直接解析参数类型；静态审查最初只核对了字段，没有核对类型所属 namespace。
- Prevention: C# exporter 新增 helper 签名时，用原版类型声明反查 namespace，并显式加入对应 `using`；无 SDK 环境的静态 guard 同时锁定该 import 和签名。
- Verification: world exporter 已加入 `using Server.MirObjects;`，静态 schema guard 覆盖 import；`go test ./internal/worlddata -count=1` 通过，真实 C# 编译仍留待具备 .NET 8 SDK 的环境验证。

### 2026-08-14 — CanFly 必须沿 Legacy 的逐格路径并区分技能例外

- Symptom: Go 的远程与单目标魔法已有目标距离和延迟命中，却没有按地图墙体检查投射路径；直接把所有远程魔法统一加墙门禁还会错误阻断 ThunderBolt/FlameDisruptor 及 MentalState TrickShot。
- Root cause: 只迁移了目标选择/命中距离，没有把 `MapObject.CanFly` 的八方向逐格推进、HighWall/LowWall 的共同 `Valid` 语义和各 spell 入口的例外条件连在一起；Go 地图属性常量也曾与 Legacy ordinal 反置。
- Prevention: 先固定 `CellAttribute` 为 Walk=0、HighWall=1、LowWall=2，再复刻 `DirectionFromPoint` + `PointMove` 的每格校验；按 Legacy switch 建立需要 CanFly 的技能表，普通 RangeAttack 与 Straight/DoubleShot 单独保留 MentalState=1 绕过，FireBounce 每一跳用来源对象当前位置重验路径，延迟命中仍使用 Legacy 的目标位置窗口。
- Verification: Go 新增地图 ordinal/解析、两种墙与边界、单目标魔法、ThunderBolt/FlameDisruptor 例外、普通远程/TrickShot、FireBounce 墙前跳转和延迟目标移动测试；受影响包及 `go test ./...` 均通过。

### 2026-08-15 — 已核验的 Go 根目录不得再次手工拼接

- Symptom: MentalState 只读检索曾把已确认的 Go 根目录重复拼成 `Crystal.GoServer.GoServer`，命令在启动前失败。
- Root cause: 已有绝对根目录没有直接复用，临时手写路径引入重复目录片段；源码未被读取或修改。
- Prevention: 每次跨仓库调用只使用先前 `git rev-parse --show-toplevel` 的完整根目录，并在命令前检查 `test -d`/`test -f`；失败命令不得作为证据，立即切换到新调用重跑。
- Verification: 纠正后 Go 查询完整返回，Legacy/Go 工作树和 C# 差异均未产生额外变化。

### 2026-08-14 — 跨仓库只读命令的工作目录必须与路径参数同仓库

- Symptom: 继续 P5 前的只读探查在 Go 工作目录读取 Legacy 的 `tasks/lessons.md`，命令仅返回文件不存在，但没有得到预期源码证据。
- Root cause: 只核对了调用目标仓库，没有同时检查命令参数中的相对路径属于哪个仓库，导致 Legacy 文件路径被错误地带入 Go 调用。
- Prevention: 每条跨仓库命令只使用当前仓库的相对路径；读取 Legacy lessons 与 Go 源码必须拆成两个独立调用，并在调用前后分别核对 `git rev-parse --show-toplevel`。失败的只读输出不得作为实现依据。
- Verification: 该命令在写入前失败且两个工作树均无源码变化；后续将 Legacy lessons 与 Go 源码查询拆开，并重新核对两仓库状态。

### 2026-08-14 — 跨仓库初始核对不得放入同一并行编排

- Symptom: 本轮开始时把 Legacy lessons、Legacy 状态和 Go 状态/文件清单放进同一个 Promise.all，虽然只是只读且未产生代码写入，但违反了项目要求的跨仓库调用边界。
- Root cause: 为减少往返而把不同仓库的独立证据查询视为可安全并行的任务，没有执行“每个 functions.exec cell 固定一个仓库”的机械检查。
- Prevention: 跨仓库任务的每个工具编排 cell 只允许一个已核验的绝对仓库根目录；需要比较时先完成一个仓库的根目录/状态/证据读取，再在新的独立 cell 切换另一仓库。禁止在同一 Promise.all、命令或路径变量中混放两侧。
- Verification: 当前并行调用只做了读取，没有 .cs 或源码写入；后续已先完整读取 Legacy lessons，并将 Legacy/Go 核对拆为独立调用，继续迁移前再执行 C# 零差异门禁。
- Strengthening after recurrence: 本轮虽已知该规则，仍把 Legacy 与 Go 的状态查询放入同一个 `Promise.all`；以后任何跨仓库任务的首个 cell 也必须只绑定一个绝对根目录，不能以“全部只读”或降低延迟为例外。执行前逐项检查编排中的每个调用，发现第二个仓库立即拆到下一 cell。
- Verification after strengthening: 本次混合查询只产生读取输出，未写入 C# 或源码；已重新独立读取 Legacy lessons，并在后续调用中只使用 Go 根目录，未采用混合调用的路径证据。

### 2026-08-14 — Go 工作目录中不得使用 Legacy 相对源码路径（再次强化）

- Symptom: 在 Go 根目录执行 Legacy `Server/...` 只读检索时，命令因路径不存在失败；没有写入，但输出不能用于语义判断。
- Root cause: route 可观察性核对切换仓库后复用了上一调用的相对路径，违反了命令参数单仓库边界。
- Prevention: 每次跨仓库查询都拆成独立工具调用，先核对 `git rev-parse --show-toplevel`，再只使用当前根目录下的相对路径；错误输出不得作为实现依据。
- Verification: 本次失败发生在读取阶段且工作树无变化；后续 Legacy 与 Go 查询分别核对根目录后再继续。

### 2026-08-14 — 跨仓库检索参数必须保持单仓库（再次强化）

- Symptom: 本批次开始核对状态时把 Legacy 与 Go 状态放进同一工具编排，后续 Legacy 只读检索又夹带了 Go 相对路径；命令只在读取阶段返回路径不存在，没有产生写入，但错误输出不能作为迁移证据。
- Root cause: 为减少往返而复用上一调用的仓库路径，未把“每个编排 cell 只绑定一个已核验根目录”落实为机械约束。
- Prevention: 每个工具编排只允许一个仓库根目录；切换前新建独立调用，先执行 `git rev-parse --show-toplevel`，再让命令参数只包含当前仓库的相对路径。跨仓库比较只使用两次独立调用的成功输出。
- Verification: 本轮错误查询均发生在只读阶段且两仓库源码/C# 状态未变化；纠正后 Legacy 与 Go 分别核验根目录，迁移实现和门禁只采用单仓库输出。

### 2026-08-15 — 跨仓库初始状态读取仍不得并行混编

- Symptom: 本轮开始时再次把 Legacy lessons/状态和 Go 状态放进同一个 `Promise.all`，虽然命令均为只读且没有文件变化，但违反了已建立的单仓库编排边界。
- Root cause: 把“状态检查彼此独立”误当成“可以跨仓库并行”，没有在工具编排提交前逐项核对每个调用的绝对 `workdir`。
- Prevention: 跨仓库任务的第一轮也必须按仓库拆成独立工具调用；一个编排 cell 中所有 nested command 只能使用同一个、先由 `git rev-parse --show-toplevel` 核验的根目录。需要并行时只并行同一仓库的查询，切换仓库必须开始新的 cell。
- Verification: 本次混合编排只执行了读取，Legacy/Go 源码与 C# 均无新增变化；后续矩阵和 Go 源码查询已在独立的 Go 调用完成，收口门禁将继续按仓库分别执行。

### 2026-08-15 — 跨仓库源码检索参数复发时必须作废混合输出

- Symptom: 核对 AI=112/76 时在 Legacy workdir 的 `rg` 命令中附带了 Go 的 `cmd/crystal-server` 路径；Legacy 查询有结果，但 Go 部分以路径不存在退出，不能作为证据。
- Root cause: 为并列比较而在同一 shell 中复用两个仓库的相对路径，未执行命令参数的单仓库 allowlist 检查。
- Prevention: 每个源码证据调用只能包含当前 workdir 的路径；Legacy 与 Go 必须使用不同的独立工具调用，任一退出码 2 的混合输出全部作废，不得据此决定实现。
- Verification: 本次命令只读且无文件变化；后续将分别在 Legacy 根目录读取 C#，再在 Go 根目录读取 Go 协议/实体，提交前继续做两仓库 C# 零变化检查。
- Strengthening after recurrence: 即使当前调用的工作目录是 Legacy，命令末尾追加 Go 的相对路径仍会制造混合输出；读取命令的路径参数必须在提交前逐项通过当前仓库 allowlist，发现另一侧路径立即拆到新的、已核验根目录调用。
- Verification after recurrence: 本次误带 `cmd/crystal-server` 的查询只在读取阶段返回路径不存在且没有写入；后续实现判断将作废该输出，并只采用单仓库调用的成功结果。
- Strengthening after second recurrence: 即使 Go 命令主体只读取 Go 源码，末尾追加 Legacy 统计或 C# 路径仍会让整条证据调用混合并失效；执行前必须逐项检查命令参数是否只包含当前仓库的已验证前缀，发现另一侧路径立即拆到新的 functions.exec cell。
- Verification after second recurrence: 本次 Go 只读查询因附带 Legacy 路径返回不存在，输出未用于实现判断且没有写入；后续已按单仓库调用重新核对统计字段，继续迁移前保持该边界。

### 2026-08-15 — 混合仓库只读命令的全部输出必须作废

- Symptom: AI=98 复核时在 Legacy 根目录的只读命令末尾误带 Go 路径，命令以路径不存在结束；虽然没有写入，但同一调用的输出不能作为证据。
- Root cause: 读取 Legacy 方向函数后复用了 Go 文件路径，没有在执行前逐项检查命令参数是否只属于当前仓库。
- Prevention: 每个源码证据调用只保留当前仓库的已验证相对路径；发现另一侧路径或退出码 2 时，整条输出作废，并在新的、单独核验根目录调用中重跑。
- Verification: 本次错误发生在读取阶段且两仓库工作树无源码变化；后续方向与 Go helper 核对将拆成独立调用，提交前继续执行 C# 零变化检查。

### 2026-08-15 — Go 查询不得携带 Legacy 路径参数

- Symptom: AI=98 复核时在 Go workdir 的只读命令中再次附带 `Server/MirObjects/...`，Legacy 路径不存在导致命令失败；同一调用的其他输出也不能作为证据。
- Root cause: 为并列读取 Legacy 实现而复用了跨仓库路径参数，没有在工具调用边界执行当前仓库路径 allowlist 检查。
- Prevention: 每个 Go 查询只使用 Go 仓库内的相对路径；需要 Legacy 对照时先结束 Go 调用，再以新的、已核验 Legacy 根目录调用读取，禁止在同一 shell、Promise 或参数列表混放路径。
- Verification: 本次错误只发生在读取阶段且没有写入；后续会分别核验两个仓库根目录并仅采用单仓库成功输出，批次结束继续执行双仓库 C# 差异/未跟踪检查。
- Strengthening after recurrence: 本批后续的 Go 命令再次附带 Legacy `Server/...` 路径，说明仅检查 workdir 不足；执行前必须逐项检查命令中的每个路径和 glob，只允许当前仓库前缀，混合调用的全部输出一律作废。
- Verification after recurrence: 第二次错误仍停留在读取阶段且无写入；后续改为先单独核验 Legacy 根目录读取 C#，再新建 Go 调用读取 Go 结构，继续以双仓库零 C# 差异作为批次门禁。
- Strengthening after second recurrence: 本轮为读取 Hero 对照时第三次把 Legacy `Server/...` 放进 Go 调用；跨仓库比较必须按“一个调用只读一个仓库”的顺序执行，不能在 Go 命令中预留任何 Legacy 路径，即使只是同一问题的对照查询。
- Verification after second recurrence: 本次调用仍在读取阶段失败且没有写入；后续先以独立 Legacy 调用读取 C#，结束后再以独立 Go 调用读取 Hero 结构，混合输出不再用于实现判断。

### 2026-08-15 — Go 源码查询不得夹带 Legacy 路径（再次强化）

- Symptom: AI=104 对照读取时，Go 工作目录的同一条只读命令误带 `Server/MirObjects/Monsters/ZumaMonster.cs`，命令在读取阶段报路径不存在；没有写入，但该调用的其他输出不能作为证据。
- Root cause: 为并列读取 Legacy 行为而复用了跨仓库相对路径，没有在 `functions.exec` cell 内执行当前仓库路径 allowlist 检查。
- Prevention: Go cell 只允许 `cmd/`、`internal/`、`docs/`、`README.md` 等 Go 路径；Legacy 对照必须在结束 Go 调用后以新的、已核验 Legacy 根目录调用读取，混合调用的全部输出一律作废。
- Verification: 错误调用未产生文件变化；随后分别在 Legacy 根目录读取 `ZumaMonster`/`DemonGuard`，再在 Go 根目录读取 `worldMonster`/复活路径，后续实现只采用独立调用的成功输出。

### 2026-08-15 — AI=101 对照检索的仓库路径必须分开

- Symptom: Go 仓库的 AncientBringer 只读查询命令曾混入 Legacy `Server/...` 路径；命令以路径不存在结束，同一调用的其他输出也不能作为语义证据。
- Root cause: 为并列比较而在 Go workdir 复用了 Legacy 相对路径，没有执行单仓库参数 allowlist 检查。
- Prevention: Legacy 与 Go 的根目录核验、源码读取和状态检查必须使用独立工具调用；Go 命令只允许 Go 相对路径，任何退出码 2 的混合输出全部作废并在新的 Legacy 调用重跑。
- Verification: 本次错误只发生在读取阶段且无文件写入；后续分别核验两个根目录，AI=101 实现与测试只采用单仓库成功输出，C# 零差异门禁保持为空。

### 2026-08-15 — 失败的跨仓库只读命令不得采用任何部分输出

- Symptom: Legacy 读取命令末尾误带 Go 的 `cmd/crystal-server/groups.go`，命令以路径不存在结束；此前成功打印的 Legacy 片段与后续输出不能组成完整证据。
- Root cause: 为并列读取两侧实现而在同一 shell 参数中混用了两个仓库的相对路径，未在执行前逐项检查当前 workdir 的路径 allowlist。
- Prevention: 每个源码证据调用只允许一个已核验仓库的路径；发现另一侧路径或退出码 2 时，整条调用输出全部作废，并在新的独立调用中重跑所需证据。
- Verification: 本次调用发生在只读阶段且没有文件写入；后续 Legacy 与 Go 查询已拆成独立调用，迁移实现不采用混合命令的任何输出。

### 2026-08-15 — AI=104 收尾 patch 必须先验证目标仓库路径

- Symptom: 收尾 DemonGuard 经验测试时，一次 patch 使用了错误的 Go 仓库路径，工具在校验阶段拒绝，目标源码没有变化。
- Root cause: 依赖记忆手写同级迁移仓库路径，没有在写入前复用已核验的 `git rev-parse --show-toplevel` 和目标文件存在性。
- Prevention: 每次跨仓库修改前先独立核对完整绝对根目录和目标文件；patch 失败后重新读取实际文件与状态，不把工具返回当成部分落盘。
- Verification: 错误 patch 未产生文件变化；改用已核验的 `Crystal.GoServer` 绝对路径后，main 测试修改、gofmt、定向测试和包级编译均通过。

### 2026-08-15 — 跨仓库对照命令必须拒绝另一仓库路径

- Symptom: Go 根目录的一次只读命令误带 Legacy `Server/...` 路径；该路径错误使整条输出不能作为迁移语义证据。
- Root cause: 对照 Legacy 的源码检索时没有把命令参数限制为当前 Go 仓库，复用了上一侧的相对路径。
- Prevention: 每次切换仓库都用独立调用核验 `git rev-parse --show-toplevel`，命令只允许当前根目录下的路径；混合调用的所有输出全部作废并重跑。
- Verification: 错误调用只发生在读取阶段且无写入；随后以独立 Legacy/Go 调用完成 AI=105 对照，C# 工作区无变化。

### 2026-08-15 — 每次 Go 工具调用必须复核完整仓库根目录

- Symptom: 本轮恢复 AI=109 时，一次 Go 命令误用了 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer.GoServer`，进程在启动前因目录不存在而失败。
- Root cause: 手工复制已知 Go 根目录时重复了目录片段，没有在工具调用前重新核对完整绝对路径。
- Prevention: 每次 Go 工具调用都直接使用已验证的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，并在切换或写入前单独执行 `git rev-parse --show-toplevel`/目标存在性检查；失败调用的输出不作为证据。
- Verification: 错误命令未启动且没有文件变化；随后恢复到正确根目录，继续使用单仓库调用完成 AI=109 门禁与提交前核对。

### 2026-08-15 — AI=113 对照必须在根目录核验后拒绝跨仓库路径

- Symptom: 读取 ArcherGuard 的 Go 侧字段时，工具输出的仓库根目录与预期不符，命令还夹带了 Legacy `Server/...` 路径；该输出不能作为语义证据。
- Root cause: 跨仓库只读调用没有把 `git rev-parse --show-toplevel` 的结果和本次 `workdir` 绑定校验，且复用了另一侧的相对路径模式。
- Prevention: 每次对照先单独调用并核对期望根目录；随后每条命令只使用当前仓库的相对路径，Legacy/Go 读取必须是两个独立调用，根目录异常时丢弃全部输出。
- Verification: 错误调用只发生在读取阶段且未产生文件变化；后续 ArcherGuard 对照改用独立、已核验的 Legacy 与 Go 调用。
- Strengthening after recurrence: 本轮读取 Go 的 monster visibility helper 时仍把 Legacy `Server/...` 路径放进 Go 命令，虽只返回路径不存在且未写入，但再次证明源码读取也必须执行单仓库参数 allowlist；不要在同一调用中附带另一侧的对照路径。
- Verification after strengthening: 丢弃混合命令的全部输出，随后用独立 Legacy/Go 调用重新核对 Mandrill 基类与 Go damage/visibility 路径；两仓库状态均保持预期。
- Strengthening after second recurrence: 本轮 SandSnail 对照时再次在 Go 根目录执行了 Legacy `Server/...` 检索；即使命令只读失败，也必须把“当前 workdir + 所有路径参数”作为同一 allowlist 审核，禁止为了连续比较而复用上一仓库的路径模式。
- Verification after second strengthening: 该调用无写入且输出已作废；随后先在 Legacy 根目录独立读取 `PoisonTarget`/`HalfmoonAttack`，再在 Go 根目录独立读取 poison/action resolver，未再使用混合输出。

### 2026-08-15 — 跨仓库只读命令的输出必须整体作废

- Symptom: Legacy 只读命令末尾误带 Go 仓库路径，shell 返回路径不存在；没有写入源码，但该次混合输出不能作为对照证据。
- Root cause: 读取 Legacy 源码后在同一调用中继续拼接 Go 相对路径，违反了单次命令只服务一个仓库的边界。
- Prevention: 每条命令只使用当前已核验仓库的路径；跨仓库对照必须在新的独立调用中先核对 `git rev-parse --show-toplevel`，任一混合路径或非零读取结果出现时，丢弃整条输出并重跑。
- Verification: 本次错误调用仅在读取阶段失败且两个工作树无源码写入；后续 Legacy/Go 查询拆分执行，迁移实现只采用重新取得的独立输出。
- Strengthening after recurrence: 本轮一次 Go 只读命令又手写成重复的仓库根目录，进程启动前即被拒绝；即使只是读取，也不能凭记忆拼接跨仓库绝对路径。
- Verification after strengthening: 错误命令没有启动、没有输出可用证据或文件变化；随后重新核对 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 并只采用正确根目录的查询结果。

### 2026-08-15 — 跨仓库 shell 调用中的尾部路径错误会使整段证据失效

- Symptom: Legacy 根目录的一次只读命令在完成 C# 源码读取后又追加了 Go `cmd/crystal-server` glob，shell 因路径不存在退出；前面的 Legacy 输出也不能继续作为该次调用的证据。
- Root cause: 把两个仓库的连续对照塞进同一个 shell 调用，未在参数层执行单仓库 allowlist。
- Prevention: 一个工具调用只允许一个已核验仓库的工作目录和路径字面量；跨仓库读取必须结束当前调用后再切换根目录，任何尾部非零错误都整体丢弃并重跑。
- Verification: 该调用只读失败且无文件写入；随后以独立 Legacy 与 Go 调用重新读取 Jar1/运行时路径，AI=119 判断只采用重跑结果，C# 差异保持为空。

### 2026-08-15 — 跨仓库证据读取必须使用独立根目录与路径 allowlist

- Symptom: 一次 Go 工作目录的只读命令误带 Legacy `Server/...` glob，shell 仅返回路径不存在；没有写入，但该次混合输出不能作为迁移语义证据。
- Root cause: 跨仓库对照时复用了上一仓库的相对路径参数，没有把工作目录、根目录校验和源码路径作为同一调用的单仓库边界。
- Prevention: Legacy 与 Go 的根目录核验、源码读取和写入必须拆成独立工具调用；每次先执行 `git rev-parse --show-toplevel`，命令参数只允许当前仓库路径，出现混合路径或非零读取结果时整体丢弃输出并重跑。
- Verification: 错误调用只发生在读取阶段且两个工作树无 C# 变化；随后用独立根目录调用重新获取对照证据，AI=118 实现与矩阵判断未使用错误输出。

### 2026-08-16 — 跨仓库工具编排也必须保持调用隔离

- Symptom: 本轮只读状态核对把 Legacy 与 Go 两个仓库的命令放进同一个并行工具编排；没有写入，但调用边界不再能由每个结果单独证明。
- Root cause: 只把“命令参数不混用”理解为 shell 层约束，遗漏了工具编排层的并行调用也可能让 workdir、结果和后续判断发生错配。
- Prevention: 跨仓库核验、读取和写入都按仓库分成独立工具调用；每次调用只核对一个根目录，返回后再开始另一个仓库，禁止用 `Promise.all`/并行编排混放两侧命令。
- Verification: 本次调用无文件变化；后续 AI=128/129 的 Legacy 对照、Go 实现、测试和 C# 检查均按独立调用执行，判断只采用根目录与命令参数一致的结果。

### 2026-08-16 — Legacy 检索命令中的不存在路径会使整条输出失效

- Symptom: 本轮 Legacy 对照命令尾部误带不存在的 `Shared/Packets.cs` 和 `Server/Packets` 路径；该调用的检索结果不能作为迁移判断依据。
- Root cause: 复制检索参数时没有先确认当前仓库的文件清单，把另一版本/旧目录结构当成 Legacy 路径继续使用。
- Prevention: 检索前先用当前仓库的 `rg --files`/已核对路径确认目标存在；命令出现不存在路径或非零退出时整体作废并在新调用中重跑，不混用其余输出。
- Verification: 该命令只读且未改变文件；后续 AI=131 对照仅采用当前 Legacy 根目录中实际存在的源文件结果。

### 2026-08-15 — Go 门禁命令不得携带 Legacy 相对路径

- Symptom: 本批次在 Go 根目录读取门禁前误用了 Legacy `tasks/lessons.md`，命令在读取阶段因路径不存在失败；没有写入，但该调用的任何其他输出都不能作为证据。
- Root cause: 跨仓库切换后沿用了上一调用的相对路径，没有让工作目录、根目录校验和命令参数保持单仓库一致。
- Prevention: 执行 Go 命令前先在独立调用中核对 Go `git rev-parse --show-toplevel`；Go 调用只允许 Go 仓库路径，Legacy 经验或源码必须在新的 Legacy 调用中读取，失败调用整体作废。
- Verification: 错误命令未启动写入且工作树无变化；随后在核对后的 Legacy 根目录补记本课经验，并在核对后的 Go 根目录完成 diff、测试和构建门禁。

### 2026-08-16 — Legacy 对照检索不得夹带 Go 路径

- Symptom: 读取 Legacy RhinoPriest 基线时，同一条命令末尾误带了 Go 的 `docs/migration-matrix.md` 和 `cmd/crystal-server` 路径；Legacy 源码读取成功，但 Go 路径部分只返回不存在，不能作为判断依据。
- Root cause: 在完成 Legacy 检索后复用了下一仓库的相对路径，未把每个命令限制为单一仓库参数集合。
- Prevention: 跨仓库检索拆成独立调用；每次切换前先执行并核对 `git rev-parse --show-toplevel`，当前调用只使用该仓库路径，另一仓库必须在新的调用中读取。
- Verification: 错误调用只读且没有工作树变化；后续 RhinoPriest 与 Go 矩阵读取将分别在核验后的根目录执行。

### 2026-08-16 — Go 读取命令必须重新核验 workdir

- Symptom: 读取 RhinoPriest 对应 Go damage helper 时把 Go 相对路径放在 Legacy 根目录执行，命令返回路径不存在；该输出不能用于实现判断。
- Root cause: 切换回 Go 仓库时复用了 Legacy 的绝对工作目录，没有在新调用中先打印并核对 Go 的 `git rev-parse --show-toplevel`。
- Prevention: 每次仓库切换都拆成独立调用：先用目标绝对根目录执行 `git rev-parse --show-toplevel`，成功后再只用该仓库的相对路径读取或编辑。
- Verification: 错误调用只读、无文件变化；后续 Go 代码查询将在核验后的 `Crystal.GoServer` 根目录独立执行。

### 2026-08-16 — Legacy 对照路径必须先确认存在

- Symptom: RhinoPriest 对照检索附带了不存在的 Legacy `Envir` 路径，shell 返回路径错误；同命令中已成功读取的 `Server/MirDatabase/BuffInfo.cs` 仍有效，但错误段不能作为证据。
- Root cause: 按仓库习惯猜测了目录名，没有先用当前仓库文件清单核对真实基线路径。
- Prevention: Legacy 只读命令只使用已由 `rg --files`/`test -e` 确认存在的路径；不存在的对照目录先停止检索，不把同一命令的其他失败输出混入判断。
- Verification: 该调用只读且工作树无变化；后续只引用已核验的 `Server/...` 文件。

### 2026-08-16 — Go shell 参数不得混入 Legacy 路径

- Symptom: 在 Go 根目录读取 buff helper 时，命令末尾误带了 Legacy `Server/MirObjects/...` 路径；Go 文件输出有效，Legacy 路径部分仅返回不存在。
- Root cause: 试图在一条 shell 命令中同时做 Go 实现读取和 C# 对照，违反了单仓库调用边界。
- Prevention: 一条工具调用的 `workdir`、检索模式和文件路径必须属于同一仓库；跨仓库对照拆为两次独立调用，并分别核对根目录。
- Verification: 错误命令只读、没有文件变化；后续只采用 Go 当前根目录中的输出，Legacy 对照另行执行。

### 2026-08-17 — 跨仓库初始查询仍必须按工具调用隔离

- Symptom: 本批恢复时又把 Legacy 与 Go 的初始状态查询放入同一个 `Promise.all`；查询没有写入文件，但不能由每个结果独立证明其仓库边界。
- Root cause: 将“都是只读查询”误当成可以跨仓库并行，忽略了工具编排层同样可能混淆 workdir、输出和后续判断。
- Prevention: 每个 `functions.exec` cell 只绑定一个先经 `git rev-parse --show-toplevel` 核验的仓库根目录；Legacy 与 Go 的状态、源码、测试和写入按独立调用串行切换，禁止用 `Promise.all` 混放两侧。
- Verification: 混合查询未产生文件变化；后续 lessons、Go 状态、测试和提交前门禁均按独立仓库调用完成。

### 2026-08-17 — EarthGolem 对照命令的仓库路径必须机械隔离

- Symptom: AI=140 对照期间两次只读命令把另一仓库路径追加到当前命令（先在 Legacy 命令尾部带 Go 路径，后在 Go 命令中带 Legacy `Server/...` 路径）；读取阶段失败，不能使用同一调用的任何输出作为语义证据。
- Root cause: 为并列读取 C# 基线、Go 实现和迁移矩阵而复用了上一侧的相对路径，没有把 `workdir` 与全部路径参数作为一个不可拆分的单仓库调用契约。
- Prevention: 每次读取或写入前先在独立调用中核对 `git rev-parse --show-toplevel`，再逐项检查命令参数只属于当前仓库；Legacy 与 Go 必须使用不同的 `functions.exec` cell，任一混合路径或非零读取结果都整体作废并重跑。
- Verification: 两次错误调用均只读、没有 C# 或 Go 源码写入；EarthGolem 的 C#、Go 和矩阵证据随后分别在已核验根目录独立读取，后续测试与补丁只作用于 Go 仓库。

### 2026-08-17 — Go 仓库绝对路径重复时必须立即作废调用

- Symptom: AI=141 开始前一次只读命令把 Go 根目录手写成重复的 `.../me_work/me_work/Crystal.GoServer`，进程未启动，不能使用其输出作为源码或状态证据。
- Root cause: 从 Legacy 切换到同级 Go 仓库时凭记忆拼接绝对路径，没有先复用独立 `git rev-parse --show-toplevel` 返回的完整根目录。
- Prevention: 每次 Go 工具调用先在独立调用中核对 `git rev-parse --show-toplevel` 和目标存在性，后续只使用该调用返回的根目录；启动失败的调用整体作废，不据错误文本推断文件状态。
- Verification: 本次命令在进程创建前失败且没有文件变化；后续 AI=141 的 Go 读取、补丁、测试和提交将只使用核验后的 `Crystal.GoServer` 根目录。

### 2026-08-17 — Go 仓库根目录重复时必须作废启动失败调用

- Symptom: AI=141 开始前一次命令把 Go 根目录写成 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer.GoServer`，进程在启动前失败，不能提供源码或状态证据。
- Root cause: 从 Legacy 切换到同级 Go 仓库时手工重复拼接了仓库名，没有直接复用独立 `git rev-parse --show-toplevel` 的结果。
- Prevention: 每次 Go 工具调用先独立核验根目录和目标存在性，后续只使用该调用返回的完整路径；启动失败的调用整体作废，不能根据错误文本推断状态。
- Verification: 错误调用未启动且没有文件变化；随后以核验后的 `Crystal.GoServer` 根目录完成 AI=141 的读取、测试和提交准备。

### 2026-08-17 — Legacy 只读命令末尾不得夹带 Go 路径

- Symptom: PeacockSpider 对照读取时，Legacy 命令末尾误带了 Go 的 `cmd/crystal-server/monster_ai.go` 路径；命令以路径不存在结束，不能使用同一调用的任何输出作为证据。
- Root cause: 切换仓库后复用了上一侧的相对路径，未在命令参数层执行单仓库 allowlist 检查。
- Prevention: 每个源码证据调用只使用当前已核验根目录下的路径；跨仓库对照必须结束当前调用，再在新的、独立核验的 Go/Legacy 工具调用中读取，任一非零读取结果整体作废。
- Verification: 该调用只读且没有文件写入；后续将分别在 Legacy 与 Go 根目录重跑 PeacockSpider 对照，提交前继续执行 C# 零变化门禁。

### 2026-08-17 — 跨仓库只读命令必须先核验每个路径

- Symptom: AI=144 研究期间一次 Legacy 命令末尾误带 Go 源码路径，另一次 Go 命令引用了不存在的测试文件；两条命令均在读取阶段失败，不能使用同一调用的任何部分输出作为证据。
- Root cause: 为连续读取对照代码而复用了上一仓库的路径参数，并凭文件名猜测 Go 测试文件存在，没有在执行前按当前仓库的 `rg --files` 清单核验全部路径。
- Prevention: 每个 `functions.exec` cell 只绑定一个已核验仓库根目录；命令中的每个文件、目录和 glob 都必须来自该仓库的实际清单，出现混合路径、猜测路径或非零读取结果时整条输出作废并在新的独立调用重跑。
- Verification: 两次失败调用均无文件写入；后续 AI=144 对照与实现读取将按 Legacy/Go 独立 cell 和已存在路径执行。

### 2026-08-17 — AI=145 分析时 Go 工作目录不得携带 Legacy 路径

- Symptom: AI=145 对照期间一次只读命令在 Go 工作目录中附带了 Legacy `Server/...` 路径；命令没有写入文件，但读取失败，整条输出不能作为证据。
- Root cause: 切换仓库后复用了上一侧的相对路径，没有把工作目录和所有路径参数作为同一个单仓库调用契约核验。
- Prevention: 每次切换仓库先独立核对 `git rev-parse --show-toplevel`，随后命令参数只使用当前仓库已确认存在的路径；出现混合路径或非零读取结果时整体作废并在新的独立调用重跑。
- Verification: 该调用只读且两仓库均无文件变化；后续 OmaBlest 的 Legacy/Go 源码读取将按独立工具调用和路径清单执行。

### 2026-08-17 — 跨仓库状态与源码核对不得放入同一个并行工具编排

- Symptom: 本轮恢复时把 Legacy lessons/status 与 Go status 放入同一个 `Promise.all`；随后 AI=148 对照又把 Legacy C#、Go 实现和迁移矩阵读取放入同一个并行编排。查询没有写入，但两次都违反了单仓库调用边界，混合调用的输出不能作为后续证据。
- Root cause: 为降低往返延迟，把不同仓库的只读查询误认为可以共享编排，未把每个工具调用绑定到唯一仓库根目录。
- Prevention: 跨仓库状态、源码、矩阵、测试、格式化和写入都使用独立工具调用；每个 cell 只允许一个仓库路径，先核对 `git rev-parse --show-toplevel`，返回后再串行切换另一仓库。
- Verification: 两次并行编排均未产生文件变化；AI=148 的 Legacy/Go 证据随后按独立调用重读，后续测试、矩阵更新和提交继续按仓库串行执行。

### 2026-08-17 — Legacy 对照检索中的每个路径都必须先由文件清单核验

- Symptom: AI=146 基线读取命令附带了不存在的 `Server/MirEnvir/Settings.cs`，命令以非零状态结束，整条输出不能作为源码证据。
- Root cause: 依据目录习惯猜测 Settings 文件位置，没有先用当前仓库的 `rg --files` 清单确认目标。
- Prevention: Legacy 对照只使用已存在的精确路径；新增路径先在独立调用执行 `rg --files`/`test -f`，任一读取非零时作废全部输出并重新运行。
- Verification: 该命令只读且没有文件变化；OmaSlasher 的有效基线仅采用此前成功读取的 `MapObject.GetAttackPower` 与 `MonsterObject.HalfmoonAttack` 内容。

### 2026-08-17 — AI=147 开始前的 Legacy 命令不得携带 Go 文档路径

- Symptom: AI=147 开始前的一次 Legacy 只读命令末尾误带了 Go 专属 `docs/migration-matrix.md` 路径；命令以路径不存在结束，不能把同一调用的其他输出当作完整证据。
- Root cause: 切换仓库时复用了 Go 侧文档路径，没有按当前 Legacy 根目录的文件清单逐项核对命令参数。
- Prevention: Legacy 与 Go 的源码、文档和状态读取必须拆成独立工具调用；每次调用先核对 `git rev-parse --show-toplevel`，参数只允许当前仓库已确认存在的路径，任一非零读取结果整体作废并重跑。
- Verification: 该调用只读且没有文件写入；后续 AI=147 将在独立核验的 Go 根目录读取矩阵和实现，提交前继续分别执行两仓库 C# 零变化检查。

### 2026-08-17 — 跨仓库只读核对也必须保持单仓库调用边界

- Symptom: AI=149 开始前的一次只读状态/lessons/源码核对把 Legacy 与 Go 命令放进同一个并行调用；虽然没有写入，但该调用违反了已建立的单仓库证据边界。
- Root cause: 为减少往返，把两个仓库的状态、C# 基线和 Go 实现查询合并编排，忽略了 lessons 对跨仓库输出整体作废的约束。
- Prevention: 每次跨仓库工作先独立读取 Legacy lessons 和 Legacy 状态，再用另一个独立调用读取 Go 状态/源码；混合调用的输出只作诊断，不作为实现依据，继续工作前重新运行对应仓库的独立核验。
- Verification: 混合调用只产生读取输出且两个工作树没有源码变化；随后已独立读取 Legacy lessons、Legacy 对照和 Go 相关实现，后续补丁与测试仅以 Go 根目录调用的证据为依据。

### 2026-08-17 — Go 读取命令末尾不得追加 Legacy 路径

- Symptom: PowerBead 只读核对在 Go 根目录的命令末尾误带了 Legacy `Server/MirDatabase/BuffInfo.cs`，调用以路径不存在退出；没有写入，但同一调用的全部输出不能作为证据。
- Root cause: 为连续读取 Go 支持 Buff 与 Legacy 定义而复用了另一仓库的路径参数，未在执行前检查当前调用的单仓库路径 allowlist。
- Prevention: 每个 `functions.exec` cell 只使用当前已核验仓库的路径；跨仓库对照必须结束当前调用后新建独立调用并重新核对根目录，出现混合路径或非零读取结果时整体作废。
- Verification: 该调用只读且两仓库均无源码写入；后续 Buff 读取将只在 Go 根目录重跑，Legacy 对照另行执行。

### 2026-08-17 — Go 文档读取必须使用核验后的 Go 工作目录

- Symptom: 读取迁移矩阵时把 Go 的 `docs/migration-matrix.md` 路径放在 Legacy 工作目录，命令只读失败，不能把该调用输出作为证据。
- Root cause: 切换仓库后手工复用了上一侧的工作目录，没有把目标绝对根目录与路径参数作为不可拆分的单仓库调用。
- Prevention: 读取 Go 文档或源码前先在独立调用核对 Go 的 `git rev-parse --show-toplevel`；随后只使用 Go 仓库已确认存在的路径，失败调用整体作废。
- Verification: 错误调用未产生文件变化；随后在核验后的 Go 根目录成功读取迁移矩阵。

### 2026-08-17 — AI=150 恢复时的跨仓库路径错误必须整体作废

- Symptom: 恢复 DarkOmaKing 前的三次只读命令分别使用了未核验的 Legacy shell glob、在 Go 工作目录带入了 Legacy `Server/...` 路径、以及再次带入了 Legacy Fullmoon 对照路径；命令没有写入源码，但这些调用的输出不能作为证据。
- Root cause: 迁移分析切换仓库时沿用了上一调用的 glob/相对路径，没有把 `workdir`、根目录核验和参数 allowlist 作为不可拆分的单仓库边界。
- Prevention: 每个调用先独立执行并核对 `git rev-parse --show-toplevel`；Legacy 先用 `rg --files` 得到精确文件清单，Go 调用参数禁止出现 `Server/`、`Shared/`、`Client/`，Legacy 调用参数禁止出现 `cmd/`、`internal/`、Go 文档或 Fullmoon 路径。任何 shell 展开失败、路径不存在或退出码非零的只读调用整体作废，不能采用其中另一部分输出。
- Verification: 三次失败调用均发生在读取阶段且两个工作树无源码变化；随后分别在核验后的 Legacy 与 Go 根目录读取 DarkOmaKing 基线、Go AI 调度和 Fullmoon 参考实现，后续实现只采用成功调用的输出。

### 2026-08-17 — AI=150 Legacy 对照调用混入 Go 根目录必须整体作废

- Symptom: 继续读取 DarkOmaKing 的 Go 辅助代码时，把 Legacy `Server/MirObjects/...` 路径放进了 Go 工作目录；命令只读失败，没有源码写入，因此该调用的所有输出均不能作为证据。
- Root cause: 连续读取 Go/Legacy 对照时复用了上一侧的相对路径，没有把工作目录、根目录核验和参数 allowlist 作为不可拆分的单仓库边界。
- Prevention: 每个 `functions.exec` cell 只绑定一个已核验仓库；Go 调用参数不得出现 `Server/`、`Shared/`，Legacy 调用参数不得出现 `cmd/`、`internal/`。切换仓库必须结束当前调用、重新核对 `git rev-parse --show-toplevel`，任一混合路径或非零读取调用整体作废。
- Verification: 本次调用在 Go 根目录读取阶段以 Legacy 路径不存在结束且无文件变化；后续 DarkOmaKing 对照将拆为 Legacy 独立调用与 Go 独立调用，并只采用各自成功输出。

### 2026-08-17 — 跨仓库读取失败的输出不得继续用于 AI=151 判断

- Symptom: AI=151 方向核对的一条 Go 命令混入了 Legacy `Shared/Enums.cs` 路径并以路径不存在失败；同一调用的其他输出也不能作为证据。
- Root cause: 为确认旧版方向枚举，把 Legacy 相对路径追加到 Go 工作目录，违反了单仓库命令边界。
- Prevention: Go 调用参数只允许 Go 仓库路径；Legacy 对照必须结束当前调用后重新核对 Legacy 根目录，任何混合路径或非零只读调用整体作废。
- Verification: 失败命令只读且没有文件变化；随后仅使用独立 Go 的 `directionFromPoints` 输出和独立 Legacy `CaveStatue.cs` 基线完成 AI=151 实现。

### 2026-08-17 — 跨仓库并行编排仍不得混合证据

- Symptom: AI=153 恢复时再次把 Legacy 与 Go 的状态/文档检索放进同一个并行编排；调用只读且没有源码变化，但违反了项目规定的单仓库证据边界。
- Root cause: 把“命令互不写入”误当成“可以共享一次工具调用”，没有让每个编排 cell 绑定唯一仓库根目录。
- Prevention: Legacy lessons、Legacy 状态/源码与 Go 状态/源码必须分别调用；每次切换前独立核对 `git rev-parse --show-toplevel`，混合调用输出整体作废。
- Verification: 本次混合调用未产生文件变化；之后 CreeperPlant 对照、Go 实现、测试和矩阵更新均在独立仓库调用中完成。

### 2026-08-17 — Go 只读调用误带 Legacy 路径时必须整体作废

- Symptom: Go 根目录的一次只读命令误带了 Legacy `Server/MirObjects/...` 路径，命令因路径不存在失败；没有写入，但该调用不能提供可用源码证据。
- Root cause: 对照 Legacy 基线时复用了上一侧的相对路径，没有在执行前检查当前 Go 调用的路径 allowlist。
- Prevention: 每个 `functions.exec` cell 只允许当前已核验仓库的路径；Go 调用禁止出现 `Server/`、`Shared/`、`Client/`，跨仓库对照必须结束当前调用后新建独立调用，任一非零读取结果整体作废。
- Verification: 本次命令在读取阶段失败且两个工作树没有源码写入；后续 Nadz 对照将先独立核验 Legacy 根目录，再只在 Go 根目录读取 Go 文件。

- Strengthening after recurrence: 后续 Go 只读命令再次在参数末尾带入 Legacy `Server/...` 路径，导致命令以路径不存在结束；即使 Go 片段成功打印，整条混合调用的输出也必须全部作废。
- Verification after recurrence: 本次调用只读、未产生任何文件变化；之后将先结束 Go 调用，再以独立 Legacy cell 读取 C#，并重新核验 Go 根目录后只读取 Go 路径。

- Strengthening after recurrence: Nadz 对照读取再次在 Go 工作目录混入 Legacy `Server/...` 参数，命令在读取阶段失败；失败调用的其他输出同样不得继续作为证据。
- Prevention: 在每次 Go 读取前先检查命令参数只包含 Go 仓库已列出的路径；Legacy 对照必须另起独立调用，且跨仓库调用出现非零状态时立即作废整条输出。
- Verification: 本次调用没有写入任何文件；后续 Nadz 基线与 Go 源码将分别在各自已核验根目录重读。

- Strengthening after recurrence: 同一 Nadz 源码核对流程再次在 Go 调用中携带 `Server/...` 参数并失败；即使前置 Go 片段成功，也不得采用该混合调用的任何输出。
- Prevention: 执行前逐项检查 `cmd` 字符串和 `workdir`：Go 调用只准出现 `cmd/`、`internal/`、`docs/` 等已核验 Go 路径；Legacy 文件必须在后续独立 Legacy 调用读取。
- Verification: 该调用未产生写入；后续已将读取动作拆分，并以非零退出作为整条调用作废条件。

### 2026-08-17 — Legacy 只读命令不得追加 Go 路径

- Symptom: 上一轮一次 Legacy 只读命令在完成源码读取后误追加了 Go 的 `cmd/crystal-server/ancient_bringer.go`，命令在读取阶段失败；没有文件写入，整条输出不能作为证据。
- Root cause: 为连续对照而复用了另一仓库的相对路径，没有在调用参数层执行单仓库路径 allowlist。
- Prevention: Legacy 调用只使用已核验的 `Server/`、`Shared/`、`Client/`、`tasks/` 路径；Go 对照必须在新的、先核验根目录的独立调用中读取，任一非零混合调用整体作废。
- Verification: 该调用未产生文件变化；本批已先完整读取 lessons，后续 Go/Legacy 读取、测试和写入将按独立仓库调用执行。


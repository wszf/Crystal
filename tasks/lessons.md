# Lessons learned

Record project-specific corrections and failure-prevention patterns here.

### 2026-08-17 — DarkCaptain 对照读取仍须保持单仓库参数

- Symptom: 一次 Legacy 只读命令在 `Crystal` 根目录夹带了 Go 的 `cmd/crystal-server/world.go`，读取在路径解析阶段失败；该调用没有写入，输出不能作为源码证据。
- Root cause: 为了连续查看 Go 的 `killMonsterLocked`，复用了另一仓库的相对路径，没有把工作目录与命令参数作为不可拆分的单仓库集合。
- Prevention: 当前仓库读取完成后必须结束调用；切换仓库先独立执行并核对 `git rev-parse --show-toplevel`，新调用的路径参数只允许当前仓库文件。失败调用整体作废。
- Verification: 本次失败调用未产生源码变化；后续 DarkCaptain Legacy 与 Go 片段将分成两次已核验根目录的调用。

- Strengthening after recurrence: 即使 Legacy 片段已经成功读出，命令末尾也不得追加 Go 文件；必须在切换根目录后重新核验，并让整条命令只包含当前仓库路径。
- Verification after recurrence: 本次混合调用在 Go 路径解析阶段失败且未写入；其 Legacy 输出也不再作为证据，后续将分别重读两侧。

### 2026-08-17 — DarkCaptain 检索模式不得交给 zsh 裸展开

- Symptom: 一次 Go 只读命令把未核验的 `cmd/crystal-server/*teleport*` 作为裸 shell 参数，zsh 在无匹配时退出；该调用无写入，所有输出作废。
- Root cause: 习惯性使用文件名 glob，未先列出精确文件，也未使用 `rg --glob` 让检索器处理模式。
- Prevention: 先用 `rg --files` 核对候选，或把模式作为 `rg --glob` 参数；禁止未核验 glob 交给 zsh，任何非零读取调用整体作废。
- Verification: 本次 shell 展开失败未改变文件；后续 DarkCaptain 读取只使用已存在的 Go 文件路径和 `rg --glob`。

### 2026-08-17 — Go 工作目录绝对路径不得重复拼接

- Symptom: 一次 Go 只读检索把已核验根目录重复成 `.../me_work/me_work/Crystal.GoServer`，进程未启动，输出不能作为源码证据。
- Root cause: 复制绝对工作目录时凭记忆再次拼接父目录，没有直接复用最近一次 `git rev-parse --show-toplevel` 结果。
- Prevention: 每次 Go 工具调用前先独立核对 `git rev-parse --show-toplevel`；后续 `workdir` 只使用该返回值，禁止手工追加仓库路径。启动失败的读取调用整体作废。
- Verification: 本次调用在进程创建阶段失败且未写入；后续将先在单独调用中核对 Go 根目录，再执行纯 Go 路径检索。

### 2026-08-17 — Go 读取前必须核对精确文件清单

- Symptom: 一次 Go 只读检索凭记忆加入不存在的 `monsters.go`、`visibility.go`，`rg` 返回路径错误；该调用的其他输出不能作为源码证据。
- Root cause: 先按概念猜文件名再检索，没有用当前仓库的 `rg --files` 确认目标文件。
- Prevention: 读取前先在同一 Go 根目录用 `rg --files` 列出精确候选；后续命令只使用已存在的路径或让 `rg` 在已确认目录内处理 glob。任一非零读取命令整体作废。
- Verification: 本次调用未写入；后续将改用已确认的 `main.go`、`monster_ai.go` 及 `rg --files` 返回的实际文件。

### 2026-08-17 — 跨仓库 Legacy 对照调用必须完全隔离

- Symptom: DarkCaptain 对照命令在 Legacy 根目录中夹带 Go 路径和未核验 Go glob，shell 展开失败；该调用的 Legacy 输出不能作为源码证据。
- Root cause: 为连续读取两侧实现，把另一个仓库的参数复用到当前 shell，没有把命令失败视为整条证据失效。
- Prevention: 每次工具调用只使用当前 `git rev-parse --show-toplevel` 对应仓库的路径；切换仓库必须结束调用并重新核验根目录，禁止跨仓库路径或 shell glob 混入。
- Verification: 失败调用未产生写入；随后以纯 Legacy 调用重新读取 `DarkCaptain.cs`、`MonsterObject.cs`，迁移判断只采用成功调用的输出。

### 2026-08-17 — 工具编排失败不得作为源码证据

- Symptom: 一次只读对照编排把 `exec_command` 拼写成不存在的工具函数，调用在执行前失败且没有任何源码输出。
- Root cause: 手写工具名时没有沿用已确认的工具接口，也没有先做最小调用验证。
- Prevention: 工具编排只使用已声明的 `tools.exec_command`/`tools.apply_patch` 等名称；脚本失败时整条调用作废，不从错误文本推断代码内容。
- Verification: 本次失败发生在脚本执行阶段、无文件变化；后续继续使用已核验工具接口和独立仓库调用。

### 2026-08-17 — 真实会话维护 tick 可能消费冷却期移动随机数

- Symptom: 全量 race 暴露 `TestSessionOmaWitchDoctorRangeTranscript` 偶发把攻击阶段的随机序列记录为 `[2, 11]` 而不是 `[11]`；堆栈显示连接读循环的实时 `world.tick` 在手工未来时钟之前执行了 OmaWitchDoctor 的 `MoveTo`，即使 `CanMove` 尚未到期仍先消费 `Random.Next(2)`。
- Root cause: 停止 world ticker 不会停止连接级维护 tick；仅把 AI 时间字段设到未来不能阻止 Legacy `MoveTo` 在不可移动时尝试随机方向。
- Prevention: net.Pipe transcript 把维护 tick 与手工 tick 的随机流分开统计，只对手工动作的排他上界断言，并允许已核实的维护 `bound=2`；需要完全隔离时在设置目标前保持目标为空，不能把停止后台 ticker 当成停止所有 runtime tick。
- Verification: 夹具改为校验唯一的手工攻击 `bound=11`，同时只允许维护 `bound=2`；OmaWitchDoctor 定向 race 重复测试和后续全量 race 门禁通过。

### 2026-08-17 — Go 只读检索不得使用未核验 shell glob

- Symptom: 本轮 Go 读取命令把不存在的 `cmd/crystal-server/session*.go` 交给 zsh，命令在展开阶段失败；该调用的其他输出未作为证据。
- Root cause: 依赖文件名模式而没有先确认目录内容，违反了 shell 参数和当前仓库边界的逐项核验。
- Prevention: Go 检索使用 `rg --files`/`rg --glob` 让检索器处理模式，禁止让 zsh 展开未核验 glob；任一非零读取命令整体作废并重跑。
- Verification: 失败调用未写入文件；后续仅使用独立根目录核验后的 `rg --glob '*.go'` 输出。

### 2026-08-17 — AI=152 读取证据与测试夹具必须隔离

- Symptom: 本轮一次跨仓库读取命令把 Legacy `Shared/Enums.cs` 与 Go glob 放在同一调用，读取失败；PlagueCrab 冷却期测试还发现攻击范围内不可攻击时错误产生 `ObjectWalk`，投影夹具则把宠物/英雄的 MAC+Agility 随机上界误假设为攻击阶段的 `bound=1`。
- Root cause: 对照命令没有保持单仓库参数边界；AI 测试只覆盖了攻击 admission，没有覆盖攻击后继续运行的冷却分支；投影目标的真实防御属性没有从 fixture 的 materialized stats 复核。
- Prevention: 失败的跨仓库读取输出整体作废；每个 AI transcript 都要覆盖攻击后冷却 tick，并在攻击范围内不可攻击时断言无移动；Player/Monster/Hero 投影测试按真实 MAC、Agility、MagicResist 随机调用顺序配置 callback，不把不同阶段的上界混用。
- Verification: 后续 PlagueCrab processor 在冷却期直接返回，session transcript 只消费攻击阶段 `bound=1`；投影测试允许并验证实际 `bound=16` 防御抽样，普通/race 定向测试均通过，Legacy 与 Go 命令已拆分为独立调用。

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

### 2026-08-15 — net.Pipe 手工 tick 前必须冻结 session loop 的 AI 时间线

- Symptom: 服务端整包 race 门禁再次出现 Armadillo session transcript 空 reveal（期望 `ObjectMonster -> ObjectShow`，实际为空）；普通单测仍可能通过。
- Root cause: `stopPoisonSessionTicker` 只停止后台 ticker，连接 session 的读循环仍会独立调用 `world.tick(time.Now)`；夹具仍以实时 `base` 驱动，race 调度下它先消费一次性的 DigOut reveal。
- Prevention: 真实会话夹具在启动手工时钟前把 AI/search/action 时间置于未来；对一次性 discovery gate 还要把 `DigOutCheckAt` 设为“人工首 tick 前、实时维护 tick 后”，并固定 `setLightClock`，或显式暂停 AI。停止 ticker 不等于停止连接级 runtime tick，不能把两者当作同一时钟。
- Verification: Armadillo 定向 transcript 普通与 `-count=10`、race `-count=10` 均通过；随后将重跑完整普通/race 门禁。

### 2026-08-15 — 跨仓库补丁目标与检索参数必须再次核验

- Symptom: AI=136 工作中一次 apply patch 手工重复了 Go 仓库目录，另一次 Go 只读命令夹带了 Legacy `Server/...` 路径；命令未产生源码写入，但其失败输出不可用于判断。
- Root cause: 复用上一调用的绝对路径/对照路径，没有在新调用前把 `git rev-parse --show-toplevel`、工作目录和参数重新作为单仓库集合核对。
- Prevention: 每次切换仓库先独立打印根目录；随后源码检索和补丁参数只出现当前仓库路径，补丁前对每个绝对目标执行存在性核验，禁止凭记忆拼接目录。
- Verification: 错误调用均在读取/补丁验证阶段停止且工作树无新增目标文件；之后仅在核验后的 Go 根目录完成 AI=136 修改。

### 2026-08-15 — FlyingStatue 生命周期 transcript 要隔离下一次 AI 与 owner 清理

- Symptom: AI=136 定向测试第一次在 1100ms 命中同时收到第二个 `ObjectAttack`；龙卷风过期断言漏掉了 Slow 清除的 `ObjectPoisoned`；删除默认玩家后宠物用例没有产生龙卷风。
- Root cause: fixture 的 `AttackSpeed=1000` 早于远程命中，world tick 会继续运行 AI；移除 tornado owner 后同一 tick 的 poison processor 会广播状态清除；删除预置玩家后没有把攻击者的缓存目标改为宠物。
- Prevention: 生命周期测试使用足够大的攻击间隔或暂停 AI loop，断言包括 owner 消失引起的 poison 状态包；修改目标 population 后同步更新 `MonsterAITargetID/Kind`，不能保留已删除实体的缓存目标。
- Verification: AI=136 world transcript 现稳定覆盖近战、9 个 tornado 的 spawn/impact/9 个 remove、Slow 清除及宠物命中，定向测试通过。

### 2026-08-15 — FlyingStatue 目标扫描的嵌套循环必须先通过包级编译

- Symptom: AI=136 首次 `gofmt`/包级编译在 `flying_statue.go` 约第 163 行报告缺少逗号和操作数，功能代码尚未进入行为测试。
- Root cause: 目标扫描补丁调整缩进时遗漏了 `for x`/`for y`/`for distance` 的闭合层级，且候选目标语句脱离了 `for x` 体。
- Prevention: 新增多层坐标扫描后立即用 `gofmt` 和 `go test <package> -run '^$'` 做语法/包级编译门禁，再开始编写行为断言；检查每层循环的缩进和闭合数。
- Verification: 补回循环闭合、格式化后 `go test ./cmd/crystal-server -run '^$'` 通过。

### 2026-08-15 — StoningStatue 的 Random.Next(1) 也必须保留在后续 Dazed 随机流中

- Symptom: AI=135 等界防御回归实际只记录了魔抗与 Dazed 抽样，缺少敏捷/防御的 bound=1；初始期望序列失败。
- Root cause: Go 通用 `monsterAIRollLocked(1)` 为不可变结果直接返回，StoningStatue 的 MACAgility 敏捷和 `GetDefencePower(min,max)` 两次 Legacy `Random.Next(1)` 因此没有调用注入随机源。
- Prevention: 对 StoningStatue 的敏捷与防御路径使用保留 unit-bound 调用的专用 helper；通用 AI helper 仍维持既有无效分支语义。
- Verification: 等界 pet AOE 回归现在稳定记录 `[10, 1, 1, 5, 3, 10, 2]`，并确认 HP、Dazed duration/value 与毒物写回正确。

### 2026-08-15 — Go 仓库命令的绝对路径需避免手工重复目录

- Symptom: 一次只读 `sed`/`rg` 调用把 Go 根目录重复写成不存在的路径，命令未启动，不能使用其输出判断 stat 常量。
- Root cause: 切换到已核验 Go 根目录后复制路径时又手工拼接了 `me_work` 目录。
- Prevention: 每次 Go 工具调用先在独立调用中执行 `git rev-parse --show-toplevel`；随后只使用该调用返回根目录下的相对路径，禁止凭记忆拼接绝对路径。
- Verification: 错误调用在进程创建前被拒绝且无文件变化；随后在正确 Go 根目录读取常量并完成回归修复。

### 2026-08-15 — GasToad 多目标毒物写回必须先提交 value-map 伤害副本

- Symptom: GasToad Type 2 对宠物的直接伤害已生效，但同一延迟结算后的 Green poison 列表为空、宠物只少了 10 点 HP。
- Root cause: Go `world.monsters` 是 value map；伤害函数修改局部副本后，毒物 helper 从 map 取出并写回了带毒副本，调用方随后又用未带毒的旧伤害副本覆盖了 map。
- Prevention: 延迟多目标处理每次修改 value-map 实体后，先写回权威 map，再调用会再次读取/写回的状态 helper；若还需保存，必须从 map 回读而不是提交旧副本。
- Verification: GasToad Type 2 world transcript 对 Player、Monster 宠物和 Hero 的 HP/Green poison/Elapsed 均稳定通过；全量测试前的定向回归已确认宠物从 100→83 且保留毒物。

### 2026-08-15 — Monster AI 毒伤测试要分离即时绿毒 tick 与派生防御字段

- Symptom: GasToad 定向测试把 Type 2 玩家伤害期望为 90，实际同一 world tick 已为 83；Type 1 高 AC 用例仍掉血，重验用例还遗漏了内层 `Random.Next(2)`。
- Root cause: Go tick 在延迟命中后紧接着处理零 `TickAt` 的 Green poison；玩家伤害使用 `worldPlayer.MinAC/MaxAC` 派生字段而不是仅使用 `Stats` map；Type 0 分支仍按 Legacy 顺序消费 `Next(7)`、`Next(2)`。
- Prevention: 测试分别计算动作伤害与同 tick 毒伤，fixture 同时初始化权威派生 AC 字段和 Stats，确定性 roll 表覆盖每个可达分支的排他上界。
- Verification: GasToad ordinary/type1/type2、吸收伤害仍施 Paralysis、延迟目标重验及 net.Pipe transcript 均通过。

### 2026-08-15 — net.Pipe AI transcript 必须显式隔离认证与后台 ticker 状态

- Symptom: AI=131 会话夹具先因账号/角色名超出 15 字符限制登录失败，随后默认登录 HP 只有 18，后台 ticker 还抢先消费了 AI 的 3000ms 搜索随机数。
- Root cause: 测试把领域名称和登录体力当成无约束值，并在服务启动前没有冻结已加载怪物的 AI 初始化/时间状态。
- Prevention: 真实会话 fixture 使用认证正则允许的短标识；bootstrap 后读取实际协议 MP，再显式设置 world 权威 HP；启动服务前将怪物初始化并把搜索/动作时间置于未来，停止 ticker 后才注入确定性目标和时钟。
- Verification: TucsonGeneral Rage transcript 现稳定锁定登录后的 Rage、15 个岩石、两次命中和移除包序，连续定向运行通过。

### 2026-08-15 — TucsonGeneral 岩石值必须保留 Random.Next 的排他上界

- Symptom: AI=131 岩石生命周期夹具把 15 个岩石的首跳/次跳生命分别期望为 850/700，定向测试实际得到 835。
- Root cause: Legacy 使用 `Random.Next(minDC, maxDC)`，上界排他；固定值 11 时每个岩石造成 11 点伤害，不能按包含 12 的区间计算。
- Prevention: 迁移随机区间时先核对调用 API 的上下界语义；岩石测试按 `15*11` 明确计算累计伤害，并单独覆盖边界值。
- Verification: 修正期望为 835/670 后重新运行 AI=131 定向测试，并在完整普通/race、vet、build 门禁中确认。

### 2026-08-15 — 延迟毒伤必须区分挂入列表与当前状态广播

- Symptom: TucsonEgg 爆炸定向测试第一次得到 1 点额外 HP 损失；设置毒伤时间后，测试又把尚未到处理时刻的 `CurrentPoison` 当成未施毒。
- Root cause: Go 的零值 `TickAt` 会在同一世界 tick 立即处理 Green poison；修复后 Legacy-compatible poison 先进入列表，`CurrentPoison`/状态包要等下一次到期处理。测试混用了两个时序边界。
- Prevention: 所有延迟毒伤构造时显式设置 `TickAt = now + Tick`；命中时断言毒列表、到期时再断言伤害与状态包，不能用 `CurrentPoison` 替代挂入列表。
- Verification: TucsonEgg 爆炸现保持初始固定伤害、只新增 Green poison 列表；AI=128/129 定向测试和新增真实 SwampWarrior `net.Pipe` transcript 均通过。

### 2026-08-15 — 跨仓库命令中的工作目录必须逐字核验

- Symptom: 一次 Go 只读核对把已知根目录误拼成不存在的重复路径，命令未启动，不能使用其输出判断代码状态。
- Root cause: 复制完整绝对路径时手工重复了仓库目录，没有在新调用中重新核对工作目录。
- Prevention: 每次切换仓库都先单独运行 `git rev-parse --show-toplevel`；随后调用只使用该次返回的根目录，禁止凭记忆或拼接路径继续执行。
- Verification: 本次错误命令在进程创建前被拒绝且没有文件变化；随后重新核对 Legacy 根目录并只在正确仓库记录本 lesson。
- Strengthening after recurrence: 本轮在 Legacy 根目录读取状态后又把 Go 专属的 `docs/migration-matrix.md` 作为相对路径检索，命令以路径不存在退出；即使文档路径名称看似通用，也必须先按仓库实际文件清单确认归属，不能把失败命令的其他输出当作证据。
- Verification after recurrence: 该调用只读且没有文件变化；随后先独立核验 Go 根目录，再从 Go 仓库读取迁移矩阵，错误输出未用于 AI=130 判断。

### 2026-08-15 — DigOut 发现探测不能复用隐藏状态攻击门禁

- Symptom: Armadillo/ArmadilloElder 的隐藏状态如果直接调用普通怪物目标门禁，`FindNearby(3)` 会把攻击者自身的 `Visible=false` 当成拒绝条件，导致附近玩家永远不能触发 `ObjectMonster`/`ObjectShow` reveal。
- Root cause: Legacy 的“发现附近目标”和“已显示后允许攻击/移动”是两个阶段；迁移时只复用了后者的 `IsAttackTarget` 投影，遗漏了发现阶段应忽略攻击者自身隐藏标志。
- Prevention: 对 DigOut 的 discovery probe 使用仅在探测期间将 `DigOutVisible` 投影为 true 的副本，保留玩家安全区、NoFight、死亡和目标隐藏/CoolEye/等级门禁；普通 AI 和真正攻击路径继续使用原始隐藏门禁。
- Verification: Armadillo world transcript 通过了初始隐藏、3 格发现和 `ObjectMonster -> ObjectShow` 顺序；真实 `net.Pipe` 登录 transcript 确认隐藏对象不在 bootstrap 中，手动 tick 后只收到 reveal 两包。

### 2026-08-15 — 只读对照命令的工作目录与路径必须同仓库

- Symptom: 一次 Legacy 读取命令使用了 Go 的 `cmd/crystal-server/monster_ai.go` 路径，只返回路径不存在；没有写入，但不能把该命令的输出用于行为判断。
- Root cause: 在准备 Legacy/Go 对照时复用了上一条命令的路径片段，没有把 `workdir` 与相对路径作为不可分割的一组重新核对。
- Prevention: 每次跨仓库读取先单独执行并核对 `git rev-parse --show-toplevel`，随后命令只出现当前仓库的相对路径；另一仓库必须在新的调用中读取。
- Verification: 本次错误命令在读取阶段失败且工作树无变化；后续先在 Go 根目录独立读取 AI 代码，Legacy 对照命令仅使用 `Server/...` 路径。

- Strengthening after recurrence: 跨仓库补丁目标也必须在调用前用完整绝对路径核验；少拼一段目录的目标会在 apply 阶段失败，不能依赖工具错误文本代替路径检查。
- Verification after recurrence: 本次 Go 测试期望补丁因缺少 `me_work` 目录在写入前被拒绝；随后将所有目标固定为已核验的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer/...` 路径。

- Strengthening after second recurrence: 已核验的仓库根目录不能手工重复或变形；每次新工具调用仍先使用不带源码路径的 `git rev-parse --show-toplevel`，成功后才读取文件。
- Verification after second recurrence: 本次错误的 `Crystal.GoServer.GoServer` 工作目录在进程启动前失败且无文件变化；后续调用恢复为完整单一 Go 根目录。

### 2026-08-14 — net.Pipe fixture assignments must pass vet

- Symptom: `go vet ./...` rejected the Poisoning/Purification session fixture's `caster.MP, caster.MaxMP = caster.MaxMP, caster.MaxMP` as a self-assignment.
- Root cause: the fixture used a two-field tuple assignment even though only the runtime MP needed to be restored to the already-existing MaxMP value.
- Prevention: use the narrowest single-field assignment in test fixtures and run `go vet ./...` after adding or changing session setup code.
- Verification: changing it to `caster.MP = caster.MaxMP` made `go vet ./...` pass before the batch gates continued.

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

### 2026-08-14 — 工具调用前必须复核完整绝对工作目录

- Symptom: 一次 Go 只读命令把已知根目录手工重复拼接，工具在进程启动前因目录不存在而拒绝执行，没有产生文件变化。
- Root cause: 复制路径时未重新核对完整绝对路径，依赖记忆而不是当前仓库的根目录检查。
- Prevention: 每次工具调用都使用已验证的完整 `/Users/.../Crystal.GoServer` 工作目录；切换前先单独执行 `git rev-parse --show-toplevel`，失败时不继续读取或补丁。
- Verification: 错误调用在启动阶段失败且 Go/Legacy 工作树状态未变；随后恢复正确根目录并仅采用成功调用的源码输出。

### 2026-08-14 — 无属性 Buff 不应触发派生生命/魔法刷新

- Symptom: Hiding/MassHiding 的纯状态 Buff 没有任何 stat modifier，却产生了不相关的派生 HP/MP 刷新或额外状态包。
- Root cause: 通用 Buff 应用路径在每次增删 Buff 后都无条件重算并钳制派生生命/魔法，把“Buff 状态变化”和“属性变化”当成同一类副作用。
- Prevention: 只有实际改变会影响 MaxHP/MaxMP 或其他派生属性的 Buff 才执行对应刷新；无属性 Buff 只提交自身状态及其必要的可观察包。
- Verification: Hiding/MassHiding 定向测试和真实 net.Pipe transcript 现锁定单次施法的 `HealthChanged` 数量、`ObjectHidden`/`AddBuff` 顺序及过期包，均通过。

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

### 2026-08-14 — 全局时钟副作用必须区分在线 runtime 与纯 world fixture

- Symptom: 全仓测试中大量使用合成 epoch 时间调用 `world.tick` 的战斗、掉落和治疗 transcript 收到意外 `TimeOfDay` 通知并失败。
- Root cause: 新增的全局时间更新在所有 world fixture 上无条件执行；旧测试的 `tick` 不是在线服务 runtime，却被当成真实服务器时钟循环。
- Prevention: 需要连接级后台 ticker 才启用全局时钟副作用；`startTicker` 负责启用并用当前/注入时钟刷新状态，纯 world fixture 默认保持关闭，定向时间测试显式开启。
- Verification: 已把更新门禁移到 `lightsEnabled`，在线 session 通过 `setLightClock` 启用；先前受污染的现有测试及动态 TimeOfDay 定向测试随后重新运行验证。

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

### 2026-08-15 — Tucson map value fixture 必须回读权威实体

- Symptom: Tucson Mage WideLine 已排入 Monster 目标并实际产生伤害，但测试检查插入 map 前保留的 `worldMonster` 副本，误报 HP 仍为 100。
- Root cause: Go 的 `world.monsters` 是 value map；延迟命中 resolver 修改并回写 map 中的副本，不会更新测试中之前保存的局部值。
- Prevention: 对 value map 中的延迟实体，命中后必须从 `world.monsters[id]` 回读再断言；指针 map（如 players）和 value map 不得共用断言方式。
- Verification: 断言改为读取 `world.monsters[monsterID].HP`，Tucson world 与真实 net.Pipe transcript 均稳定确认 MC=20、AC=0 的命中后 HP=80。

### 2026-08-15 — 协议 helper 名称必须先从 Go 定义核对

- Symptom: Tucson net.Pipe transcript 首次编译使用不存在的 `protocol.ParseObjectAttackPayload`，包测试在实现行为验证前失败。
- Root cause: 看到请求侧 `ParseAttackPayload` 后按对称命名猜测服务端对象包 parser，未先检索协议包实际导出 API。
- Prevention: 新增 transcript 的每个 parser/helper 先用 `rg` 在当前 Go `internal/protocol` 定义中核对；若无 parser，直接与已核对的 wire builder 比较完整 payload，并立即运行包级最小编译。
- Verification: 改为比较 `ObjectAttackPayload(ObjectAttackInfo{...})` 的完整字节序列，`go test ./cmd/crystal-server -run TestSessionTucsonMageNormalAttackTranscript` 通过。

### 2026-08-14 — UserMagic 冷却必须在世界快照中转换为剩余时间

- Symptom: Go 运行时已经设置了每个技能的 `CastReadyAt`，但注销快照仍复制旧的 `StoredMagic.CastTime`；跨注销重登会丢失活动冷却，技能升级经验也没有统一的完整魔法 slice 提交入口。
- Root cause: 只迁移了客户端 `ClientMagic` 的字段和在线施法门禁，没有把 Legacy 注销时“绝对 CastTime 减当前时间、就绪写入 int.MinValue”这条持久化边界接到 world/auth 提交路径。
- Prevention: 运行时冷却使用确定的 `now` 转换为正的剩余毫秒，已就绪统一使用 Legacy 哨兵；`playerCharacterSnapshot` 与显式/异常注销、Observer 接管共同调用完整 `UpdateCharacterMagics`，并用恢复后的客户端 payload 验证边界。
- Verification: 新增 world CastTime 活跃/到期快照测试、auth 魔法 slice 深拷贝测试；P5 ElectricShock 定向、协议 packet、服务端包级编译均通过，后续继续执行全量普通/race 门禁。

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

### 2026-08-13 — ControlPoints 结算必须组合 EndWar 与 TakeConquest 的提前返回

- Symptom: Go 的 ControlPoints `End` 无条件清空积分并把每个控制点归一到最终 owner；静态复核发现，Legacy 平局保留当前 owner 时会在 `TakeConquest` 的“winning guild 已拥有 Conquest”门禁提前返回，控制点积分和各点 owner 实际都保持不变。
- Root cause: 只按 `EndWar` 的“计算最终赢家后调用 TakeConquest”概括结算意图，没有把被调函数入口门禁、是否真正换 owner、积分重置和旗帜刷新组成完整可达路径。
- Prevention: 迁移跨函数结算时分别建立“owner 真正变化”和“owner 保持”状态表；只有实际通过 `TakeConquest` 并换 owner 才重置积分、重投全部控制点（包括 owner 已相同的点）和主旗帜，平局/不可接管路径保留运行状态。
- Verification: 领域测试现同时锁定平局保留积分/分点 owner，以及新赢家清空积分并为全部控制点生成刷新事件；控制点结束包序测试覆盖颜色 → 全部控制点 → 主 owner → `BroadcastInfo`，全量普通/race 测试通过。

### 2026-08-13 — 长测试封装必须保留并轮询 exec 会话

- Symptom: `go test ./cmd/crystal-server` 超过首次 30 秒 yield 后，JavaScript 包装只输出 `exit_code/output/wall`，遗漏返回的 `session_id`，外层脚本结束时无法确认测试最终状态，只能重新执行。
- Root cause: 把首次 `exec_command` 返回当成终态，并在序列化时丢弃了继续轮询所需字段；“脚本已完成”不等于其启动的长命令已完成。
- Prevention: 可能超过 yield 的门禁统一保留完整返回值，并在 `session_id` 存在时循环调用 `write_stdin`，直到取得明确 `exit_code`；不得以空输出或外层 cell 完成代替测试成功。
- Verification: 服务端整包、全仓普通测试和全仓 race 均改用会话轮询取得 `exit_code=0`，随后 `go vet ./...` 与 `go build ./...` 也明确返回 0。

### 2026-08-13 — 持久化重试必须分离领域提交与可重复落盘

- Symptom: 未配置账户 JSON 路径时，Conquest 生命周期通知会永久停在 `pendingSave`；连续两次资产保存失败后，重放第一条旧通知还会把 authority HP 从较新的 80 暂时写回旧值 90 再保存。
- Root cause: 把“没有持久化回调”误当成“保存仍未成功”，并把只应执行一次的 authority 状态提交与可重复执行的落盘操作放进同一个 `BeforeSend` 重试闭包。
- Prevention: 可选持久化回调为空时按成功的 no-op 处理；提交后通知拆成一次性领域写入和可重复保存两个阶段，队列重试只能保存当前最新 authority，不能重新应用旧快照。
- Verification: 新增无持久化即时投递测试，以及 90→80 两次失败后重试仍只保存最新 80 的回归测试；Conquest 定向、服务端整包、全量普通/race、vet、build 与差异门禁均通过。

### 2026-08-13 — 缓存目标和延迟投射物必须在命中阶段重验攻城资格

- Symptom: 玩家远程/魔法在开战时成功排入队列后，即使命中前战争结束或攻击者公会成为新 owner，仍会扣除 Conquest 资产生命；Hero 与普通宠物也能保留失效目标继续攻击。
- Root cause: `playerCanAttackMonsterLocked` 只用于请求/选目标阶段，延迟 resolver 和伴侣缓存目标的真正命中阶段没有复用同一门禁。
- Prevention: 所有延迟攻击和跨 tick 目标缓存都执行两阶段校验：选择/排队时校验一次，真正命中前按当前战争、owner、地图和存活状态再次校验；状态变化后必须清空伴侣目标且不产生伤害通知。
- Verification: 回归测试覆盖 Hero/普通宠物选中后战争结束，以及玩家远程/魔法排队后战争结束或 owner 变化；目标 HP、通知和动作队列终态均已锁定，服务端整包测试通过。

### 2026-08-13 — 战争颜色刷新与 BroadcastInfo 是两条独立协议副作用

- Symptom: Conquest 开始/结束只为 WarZone 变化的玩家发送对象刷新，遗漏其他地图的 Legacy `BroadcastInfo`；三人测试中的颜色包顺序还随 Go map 迭代随机变化。
- Root cause: 把 `RefreshNameColour` 的 self/object colour 包和 `StartWar`/`EndWar` 的全服逐玩家 `BroadcastInfo` 合并成一条局部通知路径，同时直接遍历无序玩家 map。
- Prevention: 分别建模颜色变化、全局逐 subject 的附近 `ObjectPlayer` 广播和 NPC 强制开战的额外 announcement 循环；所有可观察玩家遍历先按 ObjectID 排序，再用接收者矩阵锁定每条连接的顺序与重复次数。
- Verification: 参与地图与无关地图的双玩家矩阵均收到正确 `ObjectPlayer`，三玩家 START/STOP 精确包序连续运行 10 次稳定通过，真实会话仍保持颜色/聊天/响应/NPC 可见性顺序。

### 2026-08-13 — Conquest Siege 的运行实体语义必须按实际 Gate 类型迁移

- Symptom: Siege 在修复时使用 Gate 方向公式，但初始加载和普通受击只按 Wall/默认方向处理，导致同一资产在修复前后显示不一致。
- Root cause: 依据数据库列表名 `Siege` 推断运行类型，忽略 Legacy `ConquestGuildSiegeInfo.Spawn` 实际严格创建 AI 72 `Gate` 并调用 `CheckDirection`。
- Prevention: 迁移多态资产时沿 Spawn 的实际构造类型决定运行行为；列表分类只决定持久字段和 NPC 动作，方向、受击和对象投影应复用真实实例类型的公式。

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

### 2026-08-13 — Go 条件中的短声明只能位于 if 初始化子句

- Symptom: 新增 Conquest 测试把 `got := ...` 写在 `if existingCondition || got := ...` 中，包级编译报 non-name on left side of `:=`。
- Root cause: 把 Go 的 `if init; condition` 语法误写成了布尔表达式内部声明。
- Prevention: 条件需要复用计算结果时，在 `if` 前单独声明，或严格写成 `if got := expression; conditionUsingGot { ... }`；短声明不得出现在 `&&`/`||` 表达式中。
- Verification: 结果变量改为条件前声明后，Conquest NPC 定向包测试编译并全部通过。

### 2026-08-13 — 特殊物品分支不能绕过通用使用门禁

- Symptom: Guild Skill Scroll 的 shape 10 分支最初直接进入公会逻辑，遗漏通用 `CanUseItem` 校验，也允许死亡角色使用。
- Root cause: 把特殊 shape 当成完整入口实现，只迁移了分支领域效果，没有先执行所有物品共享的角色状态和物品可用性门禁。
- Prevention: 每个特殊物品 dispatch 先列出并复用通用入口前置条件，再进入 shape 专属逻辑；至少覆盖死亡、无权限/不满足条件、定义缺失、成功消费和失败不消费。
- Verification: shape 10 现先调用 `canUseCharacterItem` 并拒绝死亡角色；无公会、非 leader、定义缺失、重复、成功消费等真实入口测试均通过。

### 2026-08-13 — 登录规范化瞬态字段必须回写权威持久层

- Symptom: 登录时从 session/world 投影移除了旧 JSON 错误保存的 newbie Buff type 115，但 auth 内存仍保留旧值，正常登出或重载可能再次恢复该瞬态 Buff。
- Root cause: 把登录规范化当成连接局部清理，没有同步拥有角色持久状态的 auth authority。
- Prevention: 登录期间清理或修复任何持久字段时，必须同时更新 session、world 与 auth 权威记录，并通过正常登出和 JSON 重载验证旧值不会复活；瞬态运行时状态不得写回持久模型。
- Verification: type 115 登录过滤现立即同步 auth；测试覆盖登录运行时属性生效、正常登出和磁盘重载后 type 115 不存在。

### 2026-08-13 — Buff 生命与魔法钳制必须保留逐步包状态

- Symptom: 同一个 Buff 同时降低 MaxHP 和 MaxMP 时，若先算最终状态再发包，两条 `HealthChanged` 会携带相同终态，丢失 Legacy 的 HP 步骤中间状态；逐包持久化又会产生重复写入。
- Root cause: 把连续属性刷新副作用压成一个最终快照，没有分离 wire 中间状态与最终权威持久状态。
- Prevention: 同时变化 HP/MP 时按原调用顺序捕获每一步 payload，先生成 HP 钳制包再生成 MP 钳制包；全部状态完成后只持久化最终快照一次。
- Verification: 回归测试锁定 `HealthChanged(18,20)` 后接 `HealthChanged(18,14)`，并断言只执行一次持久化。

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

### 2026-08-14 — Shell 搜索模式中的反引号不能裸露

- Symptom: 用 `rg` 搜索 Markdown 文本时，把包含反引号的模式放进双引号，shell 尝试执行其中的 `GroupID` 并输出 `command not found`。
- Root cause: 忽略了双引号内反引号仍会触发命令替换，把文档字面量直接拼进了 shell 命令。
- Prevention: 搜索 Markdown、Go struct tag 或其他可能含反引号的文本时，整段 pattern 使用单引号；需要同时包含单引号时改用参数化调用或拆分模式，禁止把未知文本插入双引号命令。
- Verification: 后续同类 `rg` 查询使用单引号模式完成，源码和文档未被命令替换影响；提交前 `git diff --check` 与敏感文件检查继续通过。

### 2026-08-13 — 原子回滚测试的期望快照不能与请求共享引用

- Symptom: `AttachSealedHero` 的伪造 stats 负例报告角色权威状态被修改，实际变化来自测试直接改写了与调用前 `before` 快照共享的 `StoredItem.AddedStats` map。
- Root cause: 构造事务请求时浅拷贝了物品切片/指针，把“待篡改输入”和“零变更基线”当成两份数据，导致测试自身污染期望值。
- Prevention: 所有原子失败测试在修改 request 前，递归深拷贝 item grids、Hero snapshots、stats maps 和嵌套 slots；失败后分别从服务重新读取角色与 registry，并与独立基线比较。
- Verification: attach fixture 改用 `cloneItemInfos`、`cloneStoredItems`、`cloneStoredHeroes` 构造请求；伪造 stats、stale stack、stale Hero 槽、满槽、已绑定及双侧物品缺失用例均通过。

### 2026-08-13 — 全局实体表不能简化成角色槽内嵌生命周期

- Symptom: P8 Hero 草稿把完整 Hero 只存于角色槽；按原版执行封印时一旦清空槽位，封印物品虽然保留 Hero ID，Hero 本体却会从 Go 持久化状态消失，无法再次解封。
- Root cause: 只迁移了 Character 保存的 Hero 槽投影，没有同时保留 Legacy 独立 Hero 全局表；把“当前绑定关系”和“实体生命周期”合并成了同一份内嵌数据。
- Prevention: 迁移由全局表实体加外键槽位组成的数据模型时，权威存储必须分别表达实体 registry 与绑定投影；解绑、封印、删除只改变绑定/删除状态，不能隐式释放全局身份、名称或 ID。旧 JSON 可从槽位重建 registry，新格式必须覆盖游离实体保存重载和按 ID 恢复。
- Verification: `TestUnboundHeroRegistryLifecycleSurvivesJSONAndRetainsName`、`TestHeroUnbindRetainsRegistryAndRequiresExplicitRebind`、`TestHeroRegistryIsAuthoritativeAcrossCommitAndMaximumItemIDScan`、封印会话重载及旧 JSON fallback 测试已覆盖游离实体、名字占用、`AddedStats[129]` 按 ID 恢复和 ID 连续性。

### 2026-08-13 — Hero 解封业务类型必须依据变更前绑定状态

- Symptom: Type 42 封印物品解封后，用“本次是否成功召唤”区分首次 Hero 与仓库 Hero，会把 `NoHero` 地图上的首次绑定误报成“已加入仓库”，并遗漏首次 Hero 的未召唤持久状态。
- Root cause: 业务分支读取了地图门禁后的运行时结果，而 Legacy 的分支条件是解封前是否已有当前 Hero；绑定关系与召唤结果是两个独立状态。
- Prevention: 解封事务前固定捕获 `hadCurrentHero`；提交 auth/world/JSON 后，以它决定首次 Hero 或仓库 Hero 的包形状，再独立依据地图门禁决定是否生成 runtime。首次 Hero 在 `NoHero` 地图必须持久化为已绑定但未召唤。
- Verification: 首次解封、已有 Hero、`NoHero` 和槽位满四条真实会话均断言包序、auth/world/JSON 终态；跨 `NoHero` 地图及断线重登测试锁定未召唤状态不会复活。

### 2026-08-13 — Hero 可见性必须按接收者矩阵拆分属性包

- Symptom: 初版 Hero 广播给所有观察者相同的 `ObjectHealth`/`ObjectMana`，并重复发送颜色包；Hero 名称颜色还直接沿用了内部 MediumOrchid，而 Legacy 的 viewer-relative 投影为 White。
- Root cause: 把 `Broadcast` 对象包、owner/group 属性可见性和 owner-only Mana 合并成一条通知路径，没有展开 `HumanObject.GetInfoEx(viewer)` 与召唤后的额外 owner 更新。
- Prevention: 明确矩阵：所有可见者收到 `ObjectHero` 和一次颜色；owner 与非零同组收到可见生命，owner 额外收到 Mana 及召唤后的第二次 Health；后续进入视野只发其当时有权看到的 Health，不补 Mana。对象名称颜色从 viewer-relative 投影生成。
- Verification: `TestGameWorldHeroSummonObserverPacketMatrix` 和 `TestRefreshStaticHeroVisibilityObserverMatrix` 分别锁定召唤时与后续入视野的 owner/group/普通观察者包序、数量、颜色和 Mana 边界。

### 2026-08-13 — 会话局部物品变更必须先同步 world 再读取整角色快照

- Symptom: P8 整包回归中，普通/太阳药水已经正确消费、删除响应也成功，但登出持久化又出现两个旧物品；`TestSessionUseAndDeleteItemTranscriptAndPersistence` 稳定失败。
- Root cause: 本批为了宠物登出读取 world 快照，使既有 `DeleteItem` 未同步 world 的缺口变成可见；退出阶段的整角色快照覆盖了 session 中更新后的背包。最初尝试只合并 `Pets` 仍会被后续整快照覆盖，未触及真正的状态所有权问题。
- Prevention: 任何 session 局部物品消费、删除、移动或创建一旦成功，必须在可能读取 `playerCharacterSnapshot` 前同步 `world.updatePlayerItems`；伴侣持久化只负责其领域字段，不能用条件式旧快照合并掩盖其他领域未同步。新增登出读取路径后，必须重跑所有会话物品持久化测试。
- Verification: `ClientDeleteItem` 成功后现同步 world，删除了无效的宠物条件合并；目标用例连续运行 20 次及 `cmd/crystal-server` 整包测试均通过。

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

### 2026-08-13 — 提交 tracked diff 不会自动包含未跟踪源码

- Symptom: P11 首次用 `git diff --name-only -z | xargs git add` 暂存时，只提交了已跟踪修改，十个新 Go 源码/测试文件仍留在工作区，提交统计与已通过测试的源码集合不一致。
- Root cause: `git diff --name-only` 默认不列出 untracked 文件，把“当前差异清单”误当成了完整工作区清单。
- Prevention: 提交前以 `git status --short` 为权威清单；明确暂存目标范围时同时处理 `??` 文件，提交后立即再次检查 status 与 `git show --stat`。禁止仅依赖 `git diff --name-only` 构造完整暂存集合。
- Verification: 十个未跟踪 Go 文件已显式暂存并 amend 到同一个 P11 提交，Go 仓库提交后工作区为空，提交包含 43 个文件。

### 2026-08-13 — 延迟物品生产必须同时固定锁序和最终快照顺序

- Symptom: 智能生物自动生产黑石最初在 `world.mu` 内调用 auth 全局 ID 分配器；移出锁后，生产通知又排在通用持久化通知之前，后续旧角色快照可能覆盖刚生成的物品。
- Root cause: 只把跨域调用改成延迟执行，没有把同一 tick 的所有快照按实际提交时刻排序；“锁外执行”与“最终权威状态”被当成两个无关问题。
- Prevention: world 锁内只准备不可变创建上下文，所有 auth/全局 ID/随机物品创建在锁外执行；同一 tick 先提交锁内捕获的普通状态和拾取金币，再执行物品创建、重入 world 合并并提交包含新物品的终态。测试必须让创建回调主动获取 `world.mu` 验证无反向锁序，并逐个执行通知后检查 auth 最终物品。
- Verification: 黑石创建回调已整体移到 world 锁外，生产通知排在通用 persist 之后；锁序超时测试和 auth 终态测试通过，`cmd/crystal-server` 整包测试通过。

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

### 2026-08-13 — 定时业务必须沿调用链传递同一个确定时间

- Symptom: 自动黑石的名称时效最初使用 `time.Now()`，与 tick 测试传入的 `now` 不一致；给 fixture 名称加入 `[2h]` 后又不再匹配默认黑石配置名。
- Root cause: 创建 helper 在调用链中重新读取墙上时间，测试 fixture 也隐式依赖默认名称匹配，随机/时间/配置三类输入没有全部显式化。
- Prevention: 所有 tick 驱动的过期、随机和生产逻辑把同一个 `now`、roll 与配置名称封装进不可变上下文向下传递；带时效标记的物品 fixture 显式设置对应配置名，不依赖默认模糊匹配。
- Verification: 自动黑石使用 tick 的 `now` 计算 `[2h]` 到期，测试显式调用 `setIntelligentCreatureSettings`，确定性 `CreateDropItem` 与时效测试通过。
- Strengthening after recurrence: FlameSpear FearTime 定向 fixture 把外层断言时间与 helper 内部初始化时间错开，导致预期的 fear-move 分支实际进入远程攻击；同一测试的状态时间、触发时间和断言时间必须由一个明确的 `base` 派生，不能靠相近的 Unix 秒值。
- Verification after recurrence: 失败 transcript 只产生错误断言、没有源码写入；将 fear-move 使用 helper 同一 `base` 后，包级编译和 FlameSpear 定向测试通过。

### 2026-08-13 — 通知测试 helper 不得隐式提交所有副作用

- Symptom: 为方便测试而让 tick helper 自动执行所有 `BeforeSend` 后，既有用例无法再断言延迟持久化/创建的通知数量和时序。
- Root cause: 读取状态的 helper 混入了投递副作用，调用者无法选择观察“生成通知”还是“完成投递”两个阶段。
- Prevention: tick helper 只返回通知；另设显式 deliver helper，并在需要时逐项执行 `BeforeSend`。涉及多阶段事务的测试分别断言通知顺序、阶段中间态和最终权威状态。
- Verification: `intelligentCreatureTestTick` 与 `intelligentCreatureTestDeliver` 已拆分，黑石锁序和 stale 快照回归测试可分别验收投递前后状态。

### 2026-08-13 — Buff 协议对象必须使用运行时 ObjectID

- Symptom: 智能生物奖励和安全区 Buff 测试若使用持久化角色 Index 编码 `AddBuff`/`PauseBuff`，单角色 fixture 可能碰巧通过，但观察者会定位到错误世界对象。
- Root cause: 把数据库角色身份和当前 world 会话身份混用，没有在 wire transcript 中用不同数值锁定边界。
- Prevention: 所有 Object*、Buff 增删/暂停协议字段使用 `worldPlayer.ObjectID`；持久化 API 才使用 `SelectInfo.Index`。会话 fixture 必须断言 bootstrap 分配的 ObjectID 出现在 payload 中。
- Verification: WonderDrug `AddBuff` 与安全区 `PauseBuff` 会话测试都解析并断言真实运行时 ObjectID，同时验证 auth 与 JSON 持久化。

### 2026-08-13 — 特殊物品迁移必须审计通用入口尾部副作用

- Symptom: 只实现 Strongbox/BlackStone/WonderDrug/Knapsack 等特殊分支时，容易遗漏通用 UseItem 尾部的二次响应、源物品消耗或无效果也消费等 Legacy 可观察行为。
- Root cause: 测试只调用领域 helper 或只关注奖励结果，没有覆盖生产 dispatch 返回后的通用处理链。
- Prevention: 每种特殊物品同时测试领域结果和真实生产入口；逐项列出分支内部与通用尾部各自的消费、持久化和响应包，尤其锁定 Strongbox 双 `UseItem`、FortuneCookie 无效果仍消费以及材料不足/不适用路径。
- Verification: 当前生产会话覆盖 BlackStone、Strongbox、WonderDrug、FortuneCookie、Knapsack 全部 shape，并断言完整包序列、源物品终态、auth 状态与磁盘重载。

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

### 2026-08-13 — 地图领域通知与真实接收者矩阵必须同时验收

- Symptom: 智能生物召回/跨图/禁宠地图的领域状态正确，不代表 owner、旧地图观察者和目标地图观察者都收到正确的 Teleport/Remove/ObjectMonster/Health 顺序。
- Root cause: 只断言 world 对象坐标或通知总数，没有按每个接收者过滤领域通知并走真实会话投递。
- Prevention: 地图对象功能同时建立领域接收者矩阵和 net.Pipe 会话：分别断言 owner、旧观察者、新观察者的 packet ID、payload ObjectID 与顺序；禁用地图还要验证持久化解除召唤。
- Verification: 智能生物 visibility 测试覆盖第二观察者登录、移动、召回、跨图和禁宠地图，逐接收者断言通知并通过服务端整包测试。

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

### 2026-08-13 — `go test -run` 正则必须作为单个 shell 参数引用

- Symptom: 定向测试命令中的 `TestA|TestB` 未加引号，zsh 把竖线解析成管道，首段测试输出被后续的 `command not found` 覆盖。
- Root cause: 在构造 shell 命令时只关注 Go 的正则语义，没有同时保护 shell 元字符。
- Prevention: 所有含 `|`、`(`、`)` 或通配符的 `go test -run` 表达式统一用单引号包成一个参数；若通过参数数组组装命令，不能用无转义的空格拼接后直接交给 shell。
- Verification: 改为 `-run 'TestFishing|TestAwakening|TestRanking|TestEquipCharacterSlotItem'` 后，服务端定向测试通过。

### 2026-08-13 — 有序协议 transcript 禁止使用 map 驱动

- Symptom: P11 Go 探针定向测试在发送觉醒重置请求时出现 `io: read/write on closed pipe`，对端已因收到顺序漂移的前一请求而退出。
- Root cause: 测试用 Go map 保存 NPC 面板读取器和客户端请求函数，却让 `net.Pipe` 服务端按固定 Legacy 顺序读取；map 迭代顺序不稳定，不能表达 wire transcript。
- Prevention: 所有有序协议、数据库迁移步骤和副作用序列统一用显式 slice/数组声明顺序；map 仅用于不关心顺序的集合断言。对端报 EOF/closed pipe 时先核对双方完整 packet 序列。
- Verification: 两处 map 已改成具名函数的有序 slice；P11 定向测试连续运行 10 次、`internal/probe` 整包测试、P11 race 测试和 `go vet` 均通过。

### 2026-08-13 — 新测试 fixture 必须先检索真实 bootstrap API

- Symptom: 排行榜领域测试按记忆调用不存在的 `AddTestAccount`，导致包级编译失败。
- Root cause: 把其他项目常见的测试 helper 名称带入当前 auth 包，没有先检索现有账户 fixture；本项目公开 helper 实际是 `AddPlaintextAccount`。
- Prevention: 新测试调用服务 bootstrap/helper 前先用 `rg` 查声明和同包现有用法，复制真实签名后再写测试；包级仅编译门禁应紧跟新增 fixture。
- Verification: 排行榜 fixture 改用 `AddPlaintextAccount(id, password, nil)`，auth 定向测试恢复通过。
- Strengthening after recurrence: 即使 helper 名称已通过检索确认，也必须读取其完整函数签名，不能凭同类 `Update...` 方法习惯推断有 `bool` 返回值；本次 `UpdateCharacterMapRuntime` 是无返回值写入，测试误将其用在条件表达式导致包级编译失败。新增 fixture 后先跑最小包级编译，再扩展网络 transcript。

### 2026-08-13 — 新功能 ordinal 必须套用当前基线的历史插入偏移

- Symptom: P11 首次把本地 Legacy enum 直接计数得到的钓鱼、觉醒和排行榜 ordinal 写入 Go，立即与当前 Go 的 `SendOutputMessage` 等常量冲突。
- Root cause: 忘记当前迁移 wire 基线在 `DeleteItem` 和 `TownRevive` 处各有一个历史插入；本地只读 C# 快照的后续枚举值必须整体加一，不能把原始计数直接用于当前基线。
- Prevention: 每批新增协议先定位目标包前后两个已迁移常量，机械应用已记录的方向插入偏移，再在同一 patch 中加入显式 ordinal 和方向唯一性测试；任何冲突都先修正基线计算，不能挪用空闲 ID。
- Verification: P11 生产常量改为 Server `ObjectEffect=125`、`FishingUpdate=201`、`NPCAwakening=225`、`Rankings=253`，Client `FishingCast=103`、`AwakeningNeedMaterials=112`、`GetRanking=136`；现有 ordinal 测试表在统一偏移循环前故意填写 C# 快照原值，`got` 才填写当前生产值。首次把当前值直接写进 `wants` 导致测试再次加一，现已修正并由完整唯一性测试锁定。

### 2026-08-13 — 清理 helper 的多 hunk patch 也必须逐段复读

- Symptom: 清理两个未使用 helper 时，一次多 hunk patch 因第二个函数正文与记忆中的调用顺序不一致而整体拒绝；随后把 `rg`、格式化和测试串在同一命令中，又因预期的零匹配让整条门禁提前退出。
- Root cause: 依赖先前摘要而没有在 patch 前复读每个目标函数的精确正文，并把“零匹配即成功”的检索当作普通零退出命令。
- Prevention: 删除多个独立 helper 时逐个读取、逐个 patch；需要断言无匹配的 `rg` 单独执行并按空输出验收，不能让其退出码短路后续格式化或测试。
- Verification: 两个 helper 已按实际正文分别删除，无用 `strings` import 单独清理；格式化、定向测试和全量门禁随后独立执行。
- Strengthening after recurrence: `git diff --no-index` 在文件确有差异且内容合法时也固定返回 1；不得把它与 `git diff --check` 串成一个总门禁并把预期的 1 误报为失败。新增文件检查要单独运行，并显式把“有差异”的退出码 1 转换为成功，同时保留真正的 whitespace 错误输出。

### 2026-08-13 — 跨领域锁内禁止调用公会 authority

- Symptom: 公会战争/领地骨架最初在持有 `world.mu` 时调用 `GuildAuthority`，而 authority 的事务顺序是 authority → auth；这会与公会投影、会话初始化或生命周期线程形成反向锁序风险。
- Root cause: 把“刷新在线投影”写成了锁内查询权威状态，没有先切分 detached authority 快照与 world 写入阶段。
- Prevention: 公会 authority 调用必须在 `world.mu` 外完成；先取得敌对、战争或领地结果并释放 authority/auth 锁，再进入 world 锁合并投影和构造通知，网络投递与 SaveJSON 都放在所有锁之外。懒初始化同样先复制配置/定义，再在 world 锁外构造 authority。
- Verification: P9 接线改为 authority 查询/事务 → world 投影 → SaveJSON → 通知四阶段，服务端整包测试通过；race 门禁在提交前继续验证。

### 2026-08-13 — 领域层状态成功不等于完整网络 transcript

- Symptom: 战争和领地领域测试已覆盖扣款、敌对和所有权，但首次双会话测试仍暴露 `ColourChanged`/`ObjectColourChanged`/`ObjectPlayer` 的接收者顺序，以及离线时实际只有 `ObjectRemove` 而没有跨公会成员包。
- Root cause: 只按业务提交结果推断网络行为，没有按每个在线观察者展开 Legacy 的广播循环和会话注销路径。
- Prevention: 跨玩家功能必须同时建立领域终态测试与 net.Pipe 接收者矩阵；逐个列出发起者、同会成员、敌会成员和普通观察者的包序列，并以实际路径为准修正 fixture，不能凭相似公会通知推断注销包。
- Verification: 新增战争双会话和领地分页/购买 transcript，锁定双方聊天、金库、颜色/对象刷新与持久终态；定向服务端测试通过。

### 2026-08-13 — Schema tests must assert semantics, not gofmt alignment

- Symptom: Adding GuildSettings fields made existing expected structs stale, while a static schema guard failed only because `gofmt` changed column-alignment spaces.
- Root cause: Schema evolution was not accompanied by all aggregate fixture updates, and source-text assertions encoded incidental whitespace instead of declarations.
- Prevention: When extending persisted structs, search every composite literal and legacy-JSON fixture for expected values; normalize source whitespace (or inspect types structurally) before static declaration checks, and explicitly test omitted JSON fields retain zero-value runtime fallbacks.
- Verification: Guild settings fixtures now cover defaults, Setup.ini overrides, invalid-value fallback, round-trip values, and old-JSON zero values; both `./internal/worlddata` and `./internal/legacyworld` pass.

### 2026-08-13 — 独立 Go 导出器必须复现 Legacy 加载语义而非只解码字段

- Symptom: 世界导出器能完整解码 117/0 文件，但账户归档角色仍会绑定拍卖，Windows 反斜杠/大小写路径在 Unix 主机失效，嵌套 `#INSERT` 没有继续展开，缺失物品定义会中止整个拍卖或公会文件，Quest NPC ID 也可能与运行时实例不一致。
- Root cause: 把二进制字段布局正确等同于加载结果等价，遗漏了原服务端在解码后的归档过滤、文件系统语义、增长列表展开、`BindItem` 容错及地图/NPC 实例化顺序。
- Prevention: 所有独立 Go 迁移工具沿“读取 → Legacy 过滤/绑定 → 可观察投影”逐层验收；路径同时兼容 `\\`/`/` 和 Windows 大小写，动态 include 保留原增长列表顺序并加循环/规模保护，记录必须先完整消费再按绑定结果丢弃，运行时身份由导出器显式携带并保留旧 JSON fallback。
- Verification: 新增归档月份边界与拍卖绑定、跨平台路径、两层/循环 Drop include、拍卖/公会缺失定义、NPC 数据库/地图顺序及无效坐标、Monster 客户端投影的 Go 回归测试；定向测试通过，提交前继续执行全量 test/race/vet/build 门禁。

### 2026-08-13 — C# 源码与既有工具必须保持只读基线

- Symptom: 为公会仓库补协议向量和导出字段时，直接修改了迁移仓库中的两个 `.cs` 工具文件，破坏了用户用于逐项对照的 C# 基线。
- Root cause: 把“C# 可作为迁移辅助工具”误当成了允许继续演进 C#；实际上服务端、测试客户端、协议探针和数据转换工具都属于待迁移范围。
- Prevention: 两个仓库中的所有 `.cs` 一律只读，不新增、不修改、不删除、不重命名；只可读取作为行为证据。运行时、测试客户端、协议探针、导入/导出及其他迁移工具全部用 Go 实现。每次提交前分别执行工作区 diff、暂存区 diff 和未跟踪文件三项 `.cs` 检查，结果必须全部为空。
- Verification: 已用反向补丁精确撤销本批两个未提交 C# 差异，并把语言边界固化到 `AGENTS.md`；用户再次明确工具也必须用 Go 后，本批只修改 Go、Markdown 和 Git 元数据，提交前继续以两仓库六项 `.cs` 零输出门禁验收。

### 2026-08-13 — 跨 auth/world 物品事务必须先同步权威状态再投递网络

- Symptom: P11 审查发现 Storage 附件已经在 auth 原子提交，但 world 物品快照直到 `RefreshItem`/结果包写出后才更新；钓鱼 Tick 也先投递通知再持久化，觉醒拆解新增的 `ItemInfos` 没有同步到 world。连接写失败后的 cleanup 可能用旧 world 快照覆盖已提交状态。
- Root cause: 把成功响应顺序当成了整个事务顺序，没有区分“auth/world 双权威状态提交”和“可能失败的网络通知”；同时只同步物品格，遗漏了新物品定义目录。
- Prevention: 所有跨 auth/world 的物品事务先完成 auth 提交、world `ItemInfos`/三类物品格同步及必要落盘，再按 Legacy 顺序投递网络包；网络失败只能影响通知，不能让 cleanup 从旧快照回滚事务。多个定时结果先选取最后一个 changed 快照持久化，再保持原结果顺序投递全部通知。
- Verification: `advanceFishing` 已改为先持久化最后一个变化快照再投递 Tick 通知；`EquipSlotItem` 在任何响应写入前同步 world；觉醒持久化同时同步 `ItemInfos` 与物品格，并新增 world/auth 定义一致性测试和网络材料不足事务测试。提交前继续运行普通/race 全量门禁。

### 2026-08-13 — 地图未标记格与拆解满包必须保留 Legacy 怪癖

- Symptom: P11 兼容审查发现地图格 Go 零值会把未标记水域误当成 `FishingAttribute=0`；拆解奖励在满包时若按普通事务回滚，又会改变原版仍发送 `GainedItem`、扣金并删除原物品的行为。
- Root cause: Go 零值与 Legacy 构造默认值不同，并把异常但可观察的原版副作用误当成应修复的失败事务。
- Prevention: 所有未显式标记的地图格初始化 `FishingAttribute=-1`；拆解先保留发送奖励包的快照，若 `AddItem` 无法落位仍继续原版扣金/删除流程。迁移目标是有效路径和可观察怪癖等价，不自行修正 Legacy 行为。
- Verification: map fixture 覆盖默认 `-1` 和显式水域值；觉醒测试锁定满包时 `GainedItem -> LoseGold -> DeleteItem`、奖励不入包且原物品消失。

### 2026-08-13 — XOR 权限接口不能用 false 清除未设置位

- Symptom: 公会仓库无取回权限测试调用 `ChangeGuildRankOption(..., "false")` 后，Members rank 反而获得了 `CanRetrieveItem`。
- Root cause: Legacy 的 `"false"` 分支使用 XOR 切换权限位，而 Members rank 初始并没有该位；测试把切换操作误当成了幂等清除。
- Prevention: 测试权限关闭状态时优先直接断言默认 rank；若必须走变更接口，则先设为 true 再设为 false，并断言每一步位掩码。不能用 XOR 风格接口清除一个未经确认已设置的位。
- Verification: fixture 已改为直接验证 Members rank 初始不含 Retrieve 位，避免在目标行为前改变权限。

### 2026-08-13 — 公会门禁必须以 auth 权威成员关系为准

- Symptom: 公会仓库 world 入口先看 session 中缓存的 `Character.GuildIndex`，会让已失效但尚未同步的公会投影走到安全区提示；Type 3 请求还可能在确认权威成员关系前消耗一次性列表状态。
- Root cause: 把连接局部投影当成了成员资格的授权源，并过早提交了 `GuildCanRequestItems=false` 副作用。
- Prevention: 公会授权先在 auth 的同一锁域内校验 guild 和 member，再检查安全区/权限；一次性状态只有在所有前置校验成功后才能消耗。world 快照只用于定位连接和投递包，不决定权威成员资格。
- Verification: 新增 stale world GuildIndex 回归测试，锁定金钱请求优先返回 NotPartOfGuild，失败的列表请求保留 `GuildCanRequestItems`；公会仓库定向测试通过。

### 2026-08-12 — 公会 ordinal 必须锁定目标 wire 基线而非相似源码快照

- Symptom: 初次从本地 `Shared/Enums.cs` 计数得到公会 Client `79..83`、Server `84/166..171`，但当前迁移目标的精确 wire 基线实际整体高一位，并且需要包含 `GuildExpGain=171`。
- Root cause: 把工作区中的一个 enum 快照直接当成最终目标版本，未先与主线正在使用的当前协议基线交叉确认；同时只按用户最初列举包名收口，漏掉了夹在 `GuildInvite` 与 `GuildNameRequest` 之间、决定后者 ordinal 的 `GuildExpGain`。
- Prevention: 对版本可能漂移的协议枚举，同时核对完整成员序列、主线当前 wire 基线和相邻占位包；显式测试必须包含整个连续区间，不能只断言功能入口包。收到权威 ordinal 修正后，以其为当前迁移基线并检查相邻常量是否遗漏。
- Verification: 公会测试现显式断言 Client `80..84`、Server `ObjectGuildNameChanged=85` 与 `GuildNoticeChange..GuildNameRequest=167..172`，并覆盖 `GuildExpGain` UInt32 小端 payload；`go test ./internal/protocol -count=1` 通过。
- Strengthening after recurrence: 插入枚举成员会让插入点后的所有现有常量漂移，局部修目标包会制造跨功能 ID 冲突（本次为 `ObjectGuildNameChanged=85` 与旧 `GainExperience=85`）。今后发现插入点后必须机械审计该方向所有已定义常量，更新所有领域 ordinal 测试，并让唯一性测试枚举生产常量表中的每个已定义 ID；当前验证覆盖 175 个 Server 与 116 个 Client 常量，且锁定 Server `HealthChanged=77 → DeleteItem=80 → ObjectGuildNameChanged=85 → GainExperience=86`、Client `GroupInvite=62 → TownRevive=69 → EditGuildMember=80` 边界。

### 2026-08-12 — 单次异常排查不得变成固定汇报项

- Symptom: 用户只为确认一次异常而询问模型、Goal 或代码比对状态，后续迁移仍反复展示同类截图分析和内部状态。
- Root cause: 把一次性诊断误当成长期进度模板，没有在问题解释清楚后恢复到只汇报交付结果的沟通边界。
- Prevention: 一次性异常只回答当次；除非用户再次询问或出现新的真实异常，后续不主动汇报代码比对、Goal 数量、模型名称、内部调度或已解释过的原因。正常迁移仅在整批完成、真实阻塞或必须由用户决策时汇报。
- Verification: 本轮已停止重复发送该类状态，并继续直接完成关系功能批次。

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

### 2026-08-12 — 关系提交必须分离 world/auth 锁并同步会话快照

- Symptom: 婚姻和导师初版在持有 `world.mu` 时调用 auth 原子事务与离线角色查询，且 session 的 `gameCharacter` 可能在另一会话提交后用旧整角色状态覆盖最新关系字段；导师经验只改了在线 world，登出可能丢失。
- Root cause: 把在线投影、权威持久状态和连接局部快照当作同一份对象，未定义跨层锁顺序和字段级同步边界。
- Prevention: 关系事务统一由独立 `relationshipMu` 串行化，按 world 快照 → 释放 world 锁 → auth 原子提交/查询 → 重取 world 玩家并字段合并执行；禁止同时持有 world/auth 锁。session 层从 world 同步关系/进度字段，导师临时经验在登出前原子转入 auth，关系功能只合并其拥有的字段。
- Verification: 关系 world/auth 定向测试、双会话 net.Pipe 婚姻/导师完整 transcript、导师登出/到期经验结算和 Go 全量 race 门禁用于验证。
- Strengthening after review: 不得在持有 `world.mu` 时读取 auth 的离线关系记录；先复制角色索引/等级并释放 world 锁，再查 auth，重取 world 玩家并校验快照未漂移后提交。导师奖励必须在 `SaveJSON` 前完成 world/auth 经验变更，登录 bootstrap 严格拆成 Lover → 到期 Chat/MentorUpdate/奖励（或普通 MentorUpdate），到期只在登录检查；学生升级后再执行等级差自动解除。C# 默认 unchecked 的 `uint` 乘法、`long` 加法和 `long → uint` 转换也必须按位宽回绕，不能自行改成饱和运算。

### 2026-08-12 — Go 门禁前检查并清理可重建构建缓存

- Symptom: 新关系网络测试编译时出现 `no space left on device`，系统数据卷仅剩约 204 MiB，而 Go build cache 占用约 5.4 GiB。
- Root cause: 长期多批次 `go test`/race 构建缓存累积，门禁前未检查临时卷余量。
- Prevention: 大批次全量/race 门禁前用 `df -h` 和 `du -sh $(go env GOCACHE)` 检查空间；不足时仅执行 `go clean -cache -testcache` 清理可重建缓存，不删除项目或用户数据。
- Verification: 清理后数据卷恢复约 25 GiB，关系双会话测试重新编译并通过。
- Strengthening after recurrence: 本批普通测试先通过，但随后 `go test -race ./...` 因 Go build cache 达到约 12 GiB、系统临时卷仅剩约 303 MiB 而在构建阶段失败；门禁前必须同时检查可用空间、Go cache 和临时构建目录，不能只在历史失败后清理一次。
- Verification after recurrence: 仅执行 `go clean -cache -testcache` 后可用空间恢复约 12 GiB、Go cache 降至 8 KiB；重新运行 `go test -race ./...`、`go vet ./...` 和 `go build ./...` 均以退出码 0 完成。

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

### 2026-08-12 — 源码比对与子任务细节只作为内部过程

- Symptom: 源码逐段比对、子任务完成报告被频繁发送给用户，界面还因已完成代理未及时关闭而呈现持续活动，造成迁移反复汇报、模型来回切换的观感。
- Root cause: 把内部审计证据和每条并行线的原始输出当成了用户必须逐项接收的进度；同时没有在取得结果后立即关闭 completed 子代理，也没有区分线程跨轮次换模与同一时刻的模型路由。
- Prevention: 源码比对结果默认仅用于内部实现和验收；只在整批功能完成、需要用户决策、出现真实阻塞或长时间工作需简短报平安时汇报。取得子代理结果后立即关闭，并在解释模型活动前同时核对主线程当前模型和子线程生命周期。
- Verification: 本次审计确认主线程当前固定为 `gpt-5.6-sol`，Luna 仅存在于历史轮次/已关闭子线程；遗留的 Maxwell、Gibbs 已关闭，当前所有子线程均为 closed，后续不再逐项转发源码比对结果。
- Strengthening after correction: 用户对 Goal/模型状态的询问属于一次性故障排查，不得沉淀为每轮固定报告；除非用户再次询问或发现新的实际异常，后续不主动复述 Goal 数量、模型名称或子线程状态。
- Second strengthening after correction: 用户针对异常截图或某项内部状态的单次询问，只回答当次问题；不得把该分析扩展成持续汇报模板，也不得在后续迁移批次中重复发送相同的代码比对、调度状态或原因分析。验证方式是后续仅汇报整批迁移结果、真实阻塞和必须由用户决定的事项。
- Third strengthening after correction: 用户指出某类内部分析无需每次展示后，立即从所有后续进度模板中移除该项；即使内容更新或截图形式变化，也不能以“新结果”为由再次主动发送。除非用户明确重新询问，否则只在内部用于实现和验收。

### 2026-08-12 — 市场文本必须区分网络快照与 AddItem 后的 UserItem

- Symptom: Market 系统邮件曾输出未格式化的 `7000`，市场 Success/Hint 只清理 ASCII 数字；固定价物品部分合堆时，Go 文本显示原始 `(5)`，原版因 `AddItem` 修改同一对象而显示剩余 `(3)`。
- Root cause: 邮件与会话层各自实现 FriendlyName，且把发送前必须保留的 `GainedItem` 快照错误地同时用于操作后的文本。
- Prevention: FriendlyName 一律先按 Unicode 数字清理尾缀、再移除方括号，金额使用千分位；物品入包前 clone 原始数量，Success/Hint 则使用 AddItem 后的剩余数量。
- Verification: auth 与 net.Pipe 测试分别锁定 `7,000`、全角数字清理、原始 `GainedItem.Count=5` 和成功文本 `(3)`。

### 2026-08-12 — 拍卖到期与 stale Search 必须保留 legacy 生命周期

- Symptom: Go 初版每 500ms 并在每个 Game 请求前处理到期，消除了原版十分钟扫描窗口；已撤回拍品的旧搜索请求返回 reason 7，而原版因仍持有 AuctionInfo 引用返回 reason 3。
- Root cause: 把按请求查询当前 map 的 Go 模型当成了原版全局定时器和连接级对象引用模型，没有验收到期前后及移除后的旧引用。
- Prevention: 服务启动立即扫描一次、随后严格每十分钟扫描，禁止请求前隐式扫描；移除拍品时保留运行期终态 tombstone，使 stale buy 按 Sold→2、其他已移除→3 返回。
- Verification: 定时常量、显式到期处理、stale 撤回会话和 sold/withdrawn auth 测试通过。

### 2026-08-12 — GameShop 必须用不同的 GIndex 与 ItemInfo.Index 锁定怪癖

- Symptom: 既有测试一直令 `GIndex == ItemInfo.Index`，掩盖了原版库存读取用 Info.Index、写入用 GIndex、Stock 包再发 Info.Index，以及每封购买邮件消费两个 MailID 的行为。
- Root cause: fixture 使用相同索引让错误键路径退化成正常字典操作，同时只断言邮件存在，没有断言全局 ID 步进。
- Prevention: GameShop 回归 fixture 固定使用不同索引，分别断言查找键、持久化键、Stock 包键、重复购买覆盖语义和 MailID 2/4 步进；Quantity 1..99 必须先于商品查找校验。
- Verification: auth 与完整会话测试覆盖错位库存、非法数量静默、双 MailID、信用扣款和邮件 transcript。

### 2026-08-12 — 经济事务必须先落盘再做可失败的网络投递

- Symptom: SellNow、到期和 GameShop 成功后若先写在线连接，断线错误可能让已提交经济状态来不及保存；批量邮件遇到一个失效连接也可能短路其他玩家。
- Root cause: 把持久化提交和在线通知当成一个顺序循环，没有区分权威状态与尽力投递的副作用。
- Prevention: 在持锁事务内原子准备/提交货币、物品、拍卖、邮件和 ID，释放锁后先保存 JSON，再按 legacy 顺序投递；批量通知继续遍历，只保留第一个错误。
- Verification: 失败不扣款/不改拍卖、断线后状态保存、多个收件人继续投递以及普通/race 测试通过。

### 2026-08-12 — 会话 fixture 的角色名也必须满足 3–15 字符约束

- Symptom: 新增拍卖定义测试使用 `DefinitionViewer`，角色创建返回 1，定向测试在业务逻辑前失败。
- Root cause: 只检查了账号 ID，没有复用角色名同样存在的长度与字符集约束。
- Prevention: 测试账号和角色名统一使用 3–15 个 ASCII 字母数字，并在 fixture 创建失败时先核对认证枚举与输入长度，再排查目标功能。
- Verification: 改为 `DefViewer` 后嵌套 ItemInfo、市场会话及全量测试通过。

### 2026-08-12 — 同一文件的格式化写入与读取验证必须串行

- Symptom: 本轮曾把 `gofmt -w` 与对相同文件的 `rg` 放进并行调用；另有跨仓库读取曾通过命令链组合，存在读取中间态和混淆失败来源的风险。
- Root cause: 只按降低延迟判断可并行性，没有把格式化视为写操作，也没有保持每个仓库、每个验证步骤的独立 workdir/退出状态。
- Prevention: apply_patch、gofmt 和任何生成操作均串行完成，随后再并行执行互不写入的读取；跨仓库命令各用独立绝对 workdir，禁止用 `&&`、`;` 或管道拼接多个验证步骤。
- Verification: 后续格式化、定向测试、全量 test/race/vet/build 和两个仓库的 diff/status 均用独立调用完成。

### 2026-08-12 — 大型多文件 patch 必须按稳定 hunk 拆分并复读

- Symptom: 一次同时修改 service/economy 的 patch 因最后一个 `removeAuctionLocked` 上下文不匹配而整体拒绝；GameShop 测试 patch 也因手写失败文案与当前源码不同而未应用。
- Root cause: 多文件 patch 依赖较早读取的长上下文，任一末端 hunk 漂移都会让整批失败；工具成功返回也不能证明每个预期字段都已落盘。
- Prevention: 每个文件或语义 hunk 单独 patch，使用函数签名/字段名等短稳定锚点；失败后立即重新读取精确上下文，成功后用 `rg`/`sed` 和 diff 复核，JavaScript 包装只用普通字符串，禁止让未定义值变成 `NaN` 参与 patch 文本。
- Verification: 拆分后 retired 状态、双 MailID、会话测试和文档均正确落盘，`git diff --check` 与全量 Go 门禁通过。
- Strengthening after recurrence: P11 收尾曾把 fishing、EquipSlotItem 和 awakening 三个独立修正放入一个 patch，最后一个 main.go hunk 漂移导致整块拒绝。即使改动属于同一审查批次，也必须按单文件、单语义提交 patch；被拒绝后先确认前面 hunk 均未应用，再逐项重做并立即跑最小定向测试。
- Second strengthening after recurrence: 终端或普通 `sed` 输出的视觉换行不代表文件中的物理行边界；长段落 patch 前先用 `sed -n l` 核对行首和续行，再选择真实存在的短锚点。文档也必须按单文件 patch，避免一个错误行首让其他文件的正确 hunk 一并回滚。本次 Conquest 文档补丁确认无部分写入后按 README、迁移矩阵分别重做，并以 diff 复核落点。
- Strengthening after third recurrence: AI=93 代码插入第一次使用了错误的函数锚点，补丁在校验阶段拒绝；新增函数前必须从当前文件重新读取精确声明，并把同一语义插入拆成可验证的单 hunk。
- Verification after third recurrence: 失败补丁没有部分写入；改用实际 `monsterAIAxeSkeletonAttackLocked` 声明后成功插入，包级编译、FlameMage 定向及全量门禁均通过。

### 2026-08-12 — 买回数量测试必须区分堆叠上限与当前存量

- Symptom: 买回用例用 `Count=9` 配合 `StackSize=1/5`，加入原版堆叠上限门控后 transcript 等待超时；旧实现因缺少门控反而掩盖了 fixture 错误。
- Root cause: 把“超过当前存量时截断”误写成“任意超大数量都截断”；原版先拒绝超过 `ItemInfo.StackSize` 的请求，再把合法范围内超过存量的请求截到存量。
- Prevention: 数量测试同时列出 `request.Count`、`ItemInfo.StackSize`、`stored.Count`；截断场景必须满足 `stored.Count < request.Count <= StackSize`。
- Verification: 买回用例改为 `9 <= StackSize=10`，UsedGoods 用例改为 `3 < Count=4 <= StackSize=5`，并通过完整 net.Pipe transcript。

### 2026-08-12 — 原版物品价格必须按附加属性数值总量计算

- Symptom: 带多个或负值附加属性的 UsedGoods 买回价格可能与原版不同。
- Root cause: Go 价格辅助函数曾按附加属性键数量计算；原版 `Stats.Count` 是每个属性值绝对值之和。
- Prevention: 迁移包含 `Stats` 的物品公式时，先对照 `Stats.Count` 的实现及符号规则，再复用统一的数值总量辅助函数；不要把 map 长度当成属性强度。
- Verification: 增加附加属性买回价格回归测试，`{2, -3}`、数量 2、单价 1000 得到原版价格 3000。

### 2026-08-12 — 背包持久化断言必须按物品身份而非槽位

- Symptom: `[BUYUSED]` net.Pipe transcript 的购买和登出数据都正确，但测试按 `Inventory[0]` 断言，实际物品被放入原版可用背包区的第 7 格而失败。
- Root cause: 把背包内部的自动落位策略误当成了对外行为契约；物品加入逻辑会按物品类别选择合法起始槽位。
- Prevention: 验证跨层物品迁移时按 `UniqueID`、数量、状态和持久化结果断言；只有原版明确固定槽位的 Move/Storage 测试才断言数组索引。
- Verification: 改为扫描持久化背包查找 `UniqueID` 后，`[BUYUSED]`、UsedGoods 合并/刷新及登出测试通过。

### 2026-08-12 — NPC 商品响应面板与购买请求类型不是同一语义

- Symptom: 初版 `[BUYUSED]` transcript 使用 `PanelType.BuySub` 发送购买请求；静态对照客户端后确认真实客户端会一直发送 `PanelType.Buy`，原版服务端因此会拒绝该初版请求。
- Root cause: 把 `NPCGoods.Type`（控制客户端显示 BuySub 子面板）误当成 `ClientPackets.BuyItem.Type`（原版购买入口只接受 Buy）。
- Prevention: 每个双向协议功能分别核对服务端响应字段、客户端发送字段和服务端门控；不能从同名/相近 enum 值推断请求值。
- Verification: Go 服务端 `[BUYUSED]` 请求门控改为接受 `Buy`，返回仍保持 `BuySub`；net.Pipe transcript 和客户端源码对照均通过。

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
- Strengthening after recurrence: 本批首次给 `worldMagicAction` 增加区域动作字段时，patch 数组中的 Go 原文以制表符直接开头，遗漏 unified diff 要求的上下文空格，整个 hunk 被拒绝。今后数组中每条保留源码行必须先加一个字面量空格，再接源码原有缩进；构造后逐行检查除 marker 外的首字符只能是空格、`+` 或 `-`。
- Verification after recurrence: 失败补丁未产生部分修改；区域动作结构改用带显式上下文空格的小 hunk 重做，并在后续读取和最小编译中确认字段只出现一次。

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

### 2026-08-15 — 召唤 helper 返回值必须明确绑定语义

- Symptom: AI=97 定向测试首次通过召唤 helper 返回的 `worldMonster` 副本调用死亡逻辑，但副本的 `Dead`/`DespawnAt` 更新没有自动写回 `world.monsters`，后续 tick 仍把世界中的实体当作存活对象。
- Root cause: helper 同时返回“刚创建的快照”和持锁 world map 中的实体，调用者没有意识到返回值是 detached value，而 Go struct 不提供引用式回写。
- Prevention: 领域 helper 要么只返回不可变结果并在内部完成 map 提交，要么明确命名/文档化 detached contract；任何修改返回实体后都必须在同一锁边界显式写回权威 map，并增加后续 tick/重载断言。
- Verification: AI=97 测试已在死亡回调后显式写回 knight，再验证 despawn 不进入普通 respawn；定向、全包和 race 门禁通过。

### 2026-08-15 — AI 测试夹具必须复用精确 stat 名称、锁语义和随机公式

- Symptom: AI=98 定向测试先因把玩家 stat 的 `statMinMAC`/`statMinMC` 写成不存在的 `monsterStat...` 常量而无法编译；修正后又在持有 `world.mu` 时调用会再次加锁的 `enableMonsterAI`，测试超时；最后按单次 DC 随机直觉断言炸弹/地震伤害，未覆盖 Legacy HellLord 地震的嵌套 `Random.Next(Random.Next(min,max))`。
- Root cause: 新夹具没有先读取当前包的精确常量和锁边界，并把 Legacy 的随机调用序列/嵌套范围压缩成了看似等价的单次 roll。
- Prevention: 新测试先检索声明和完整 helper 签名；持锁区只直接写字段，锁外调用加锁 helper；迁移随机 AI 时逐次列出调用顺序、闭区间/开区间和嵌套 roll，并用计数确定性源验证最终 wire/HP。
- Verification: HellLord/HellKnight 定向测试现可在 30 秒门禁内完成，覆盖 600ms 动作、延迟召唤、地震、阶段免伤和炸弹 10 秒/500ms 生命周期；包级编译与定向回归通过。

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
- Verification: Go 的 `test`、`race`、`vet`、`build` 与差异检查通过；本轮怪物掉落/default NPC exporter 仍因同一环境限制未编译，.NET exporter 和 ProtocolProbe 保留为待 SDK 环境验证项。

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
- Strengthening after recurrence: 删除 Markdown 列表项时，patch 行必须以两个连字符开头（第一个是 diff 删除标记，第二个是文档列表标记）；上下文列表项必须以空格再接连字符开头。应用前按“patch 标记 + 文档内容”两层机械检查。

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
- Strengthening after recurrence: `*** End Patch` 也必须是数组中的已引用字符串，不能落在 `.join()` 调用之后成为 JavaScript token；构造后先机械确认首项为 Begin、末项为 End，再调用工具。本次 Conquest auth 补丁在执行前被 JavaScript 解析拒绝，源码无部分修改，已改为单文件小补丁重做。

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
- Strengthening after recurrence: auth 领域事务测试若要验证合法背包槽，也必须显式创建 Legacy 的 46 格 Inventory；新建角色的 nil/零长度 Inventory 不代表可用空背包。否则合法取出会被正确判成目标越界，容易被误判为事务实现问题。

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

### 2026-08-11 — NPC 链式页必须逐页执行特殊面板

- Symptom: GOTO/CALL 初版只在整条链最后按 active page 发送 shop/repair/refine 面板，链中间进入功能页时客户端只能收到文本，缺少对应功能包。
- Root cause: 控制流 job 队列只复用了页面文本和 actions，特殊页处理仍留在原请求末尾，没有随每个 job 的生命周期执行。
- Prevention: 把 NPC 页处理拆成“选择响应 → 执行动作/追加链 → 当前页特殊处理”，每个链式 job 都按自身 page key 跑 shop、repair、refine、storage 和 collect 分支，并分别锁定包序列。
- Verification: 新增 GOTO 到 refine 页和自循环上限的 net.Pipe 测试；现有 shop、repair、refine、storage 测试和控制流测试均通过。

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

### 2026-08-11 — 同一连接重新登录必须清空可见对象缓存

- Symptom: 任务登出后在同一连接重新 StartGame，服务器先发送任务状态，但没有重新发送 NPC 对象；会话验收按启动包序列阻塞。
- Root cause: logout 已移除 world player，却保留了 session 层 `visibleNPCs`/`visibleMonsters` 集合，下一次刷新把仍存在的静态对象误判为客户端已拥有。
- Prevention: 任何离开 Game stage 的路径都同时清理 world player、活动 NPC 状态和可见对象缓存；重进验收必须断言静态对象与任务状态都重新 bootstrap。
- Verification: `TestQuestSessionAcceptFinishAndPersist` 覆盖接取→登出保存→同连接重登恢复→完成→再次登出；Go 全量测试通过。

### 2026-08-11 — .NET exporter 与 Go JSON bridge 必须逐字段核对语义

- Symptom: 导出器把 `CurrentQuests` 写成任务对象列表，而 Go bridge 的同名字段是 `[]int32`，导致真实账户 JSON 无法按预期恢复。
- Root cause: 只对照了字段名，没有对照字段的元素类型和“ID 列表/详情列表”职责。
- Prevention: 每增加跨语言字段，必须同时核对 C# 类型、导出 JSON 形状、Go 结构体类型和 round-trip fixture；`currentQuests` 只保存 ID，`questProgress` 保存详情。
- Verification: 增加 JSON 原始形状断言和加载后进度 round-trip 测试。

### 2026-08-11 — Quest NPC 校验不能信任客户端上下文

- Symptom: Go 实现曾把 `AcceptQuest.NPCIndex` 和当前活动 NPC 作为完成任务的额外条件，导致合法请求可能被拒绝。
- Root cause: 将“客户端打开了哪个 NPC”误当成了原版 `PlayerObject` 的授权条件；原版按任务定义查找 NPC，只检查当前地图和 DataRange。
- Prevention: request 中可由客户端伪造的 NPC ID 和 session 中缓存的 active NPC 只用于 UI/script 状态，不参与 Quest accept/finish 授权；回归测试必须覆盖错误 ID 仍能通过。
- Verification: Quest NPC range/request-ID 测试和同连接重登后的接取/完成会话测试通过。

### 2026-08-11 — 测试断言要区分 normalize 的状态修复与业务进度变化

- Symptom: 对旧版缺少任务明细的进度调用 normalize 后，测试把结构补全误判为“没有变化”或反过来误报业务进度变化。
- Root cause: 断言只看 `changed` 数量，没有先区分 schema normalization、任务状态变化和实际计数变化。
- Prevention: 测试同时断言规范化后的 task 数组、计数、完成状态和 packet 语义；不要用一个布尔值代表所有变化。
- Verification: legacy progress normalization、任务列表和 JSON round-trip 测试覆盖上述边界。

### 2026-08-11 — 工具链格式化必须按语言分组

- Symptom: Quest 收尾时把 C# exporter 文件和 Go 文件一起传给 `gofmt`，命令在 C# 文件处报 `expected 'package'`，导致格式化门禁中断。
- Root cause: 只按本批改动文件列表执行格式化，没有按扩展名和项目工具链拆分命令。
- Prevention: 提交前按语言分别运行格式化和编译检查；Go 只交给 `gofmt`，C# 只交给 .NET SDK/对应格式化工具，并在命令失败后确认源码没有被部分修改。
- Verification: 重跑仅包含 Go 文件的 `gofmt`、`git diff --check`，随后单独检查 .NET SDK 可用性并记录未验证边界。
- Strengthening after recurrence: Mail 收尾再次把 `Program.cs` 放入 `gofmt` 参数后，禁止直接复用“全部改动文件”作为格式化输入；必须先按 `.go` 扩展名生成或人工核对参数清单，C# exporter 另行探测 `dotnet`/`csc`/`mcs`，本轮已用显式 Go 文件清单完成格式化。

### 2026-08-11 — 断线广播不能被失效连接短路

- Symptom: 组队成员通过 net.Pipe 断开后，在线 leader 先收到 ObjectRemove，收不到应有的 DeleteGroup。
- Root cause: 广播按通知顺序写入时，第一个已断开成员的写操作报错，deliverWorldNotifications 立即返回，后续在线成员没有收到通知。
- Prevention: 广播遍历必须继续投递所有 recipient，只保存并返回第一个错误；新增断线场景要同时断言在线成员的协议包顺序和 world 清理结果。
- Verification: 修正广播后双会话测试稳定收到 DeleteGroup → ObjectRemove；普通测试、race、vet 和 build 全部通过。

### 2026-08-11 — net.Pipe 多会话测试要先建立 reader 并消费完整 transcript

- Symptom: 组队邀请/离开测试曾因服务端写包等待客户端读取，或在第一包到达时过早检查共享状态而失败。
- Root cause: net.Pipe 没有缓冲；组队操作会向双方发送多包，最后一个响应不等于另一会话的清理已经完成。
- Prevention: 在触发跨会话操作前为每条连接建立异步 reader，按 legacy 顺序消费全部副作用包；检查 world 状态前使用会话完成 channel 作为屏障，并在读取共享 map 时持锁。
- Verification: TestSessionTwoPlayerGroupInviteAcceptAndLeave 覆盖邀请、接受、DeleteGroup、ObjectRemove 和断线清理，且 race 测试通过。

### 2026-08-11 — apply_patch 封装必须验证路径和闭合标记

- Symptom: 更新 lessons 时一次 patch 因目标路径重复了仓库目录而未应用。
- Root cause: JavaScript 工具封装中的绝对路径是手写的，提交前没有检查目标文件是否存在；复杂 Markdown/raw 内容也容易遗漏 End Patch 闭合标记。
- Prevention: 调用 apply_patch 前先确认绝对路径和目标文件；多行 Markdown/raw 内容使用普通字符串数组拼接，明确包含 Begin Patch/End Patch，并在返回后读取文件确认落盘。
- Verification: 修正路径后 lessons 成功追加；随后 git diff --check 通过。

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

### 2026-08-12 — 地图门禁失败路径必须保持事务性

- Symptom: TownRevive 先恢复 HP、ENTERMAP 先清空 pending 目的地，再检查 RequiredGroup 时，失败请求会留下不可见的生命值或入口状态变化。
- Root cause: 业务动作的副作用早于目标地图资格校验提交，失败结果没有回滚。
- Prevention: 所有 RequiredGroup 入口先完成地图存在、坐标和组人数校验；成功后才写回 HP/坐标，ENTERMAP 只有非门禁失败时才消费 pending 目的地。
- Verification: RequiredGroup world 测试断言 TownRevive 不恢复 HP、ENTERMAP 保留 pending；全量 Go 门禁通过。

### 2026-08-12 — 物品加入背包前必须复制网络快照

- Symptom: NPC 商店批量购买改为先更新角色背包后再编码 `GainedItem` 时，`addCharacterItem` 合并/消耗了传入对象，客户端收到的购买数量变成 0。
- Root cause: 领域层的背包操作会原地修改传入 `StoredItem`；网络 payload 不能直接引用会被后续事务改变的对象。
- Prevention: 所有会调用 `addCharacterItem`、堆叠合并或删除的路径，在第一次状态变更前复制独立的 packet snapshot；测试同时断言内部持久化数量和线上的 `GainedItem.Count`。
- Verification: NPC 商店普通购买和 `[BUYBACK]` 会话 transcript 均断言数量/UniqueID，Go 全量 test、race、vet、build 和 `git diff --check` 通过。

### 2026-08-12 — 邮件落库与在线到达 transcript 必须分别验收

- Symptom: 玩家邮件和交易溢出邮件已经写入收件箱，但在线客户端只收到 `ReceiveMail` 或完全没有即时刷新，缺少原版 `NewMail` 系统聊天及完整附件定义顺序。
- Root cause: 只沿持久化状态验证了 `MailInfo.Send` 的结果，没有迁移 `NewMail -> Process -> ReceiveChat -> CheckItem -> GetMail` 这条在线可观察链路。
- Prevention: 每个创建邮件的入口同时验收离线存储和在线 `Chat -> NewItemInfo* -> ReceiveMail` transcript；跨会话只生成通知快照，释放 world/mail 锁后再写连接。
- Verification: 普通邮件、带邮票附件邮件、交易取消溢出邮件测试均断言到达顺序，Mail 定向普通与 race 测试通过。

### 2026-08-12 — 禁邮标志不能合并成一个通用失败分支

- Symptom: Go 初版把 `DontTrade`、`NoMail` 和 Rental `DontTrade` 都返回通用聊天加 `MailSent(-1)`，且聊天文本没有附带 legacy FriendlyName。
- Root cause: 只迁移了“物品不可邮寄”的领域结论，没有逐分支对照原版的聊天文本、FriendlyName 清洗和是否发送失败结果包。
- Prevention: 失败分类必须保留 definition/rental 标志来源；按原版先 trailing-digits、后方括号清洗名称，并分别锁定 `DontTrade`/Rental 仅 Chat、`NoMail` 为 Chat 加 `MailSent(-1)`。
- Verification: net.Pipe 会话分别覆盖 definition `DontTrade`、`NoMail` 和 Rental `DontTrade`，断言精确默认英文、差异化包序列和失败后背包状态；全量普通/race 测试通过。

### 2026-08-12 — 全局邮件锁不得跨网络写包

- Symptom: Refine 取消的误迁移邮件分支曾用 `defer` 释放 `mailMu`，导致后续取消结果包也可能在全局邮件锁内写入；慢客户端会阻塞其他邮件、交易和登出路径。
- Root cause: 锁的作用域按整个函数设置，而真正需要原子保护的只有邮件持久化及 world 快照同步。
- Prevention: `mailMu` 只包围邮件状态读取/提交，并保持 `mailMu -> world.mu` 顺序；在任何 `writePacket`、跨会话 Send 或日志保存前显式释放。
- Verification: 删除不符合原版可达语义的 Refine 邮件分支；其余 Mail/Trade 路径均先生成快照或通知、释放 `mailMu`，再执行网络投递。

### 2026-08-12 — 迁移分支必须证明可达性而非补全注释意图

- Symptom: Go 把 Refine 取消时“最后一个空槽被前一物品占用”的后续物品自动邮寄，并准备补发新邮件通知；原版此时不会进入邮寄分支，而会把物品保留在 Refine 工作台。
- Root cause: 只根据 `RefineCancel` 中的邮寄注释和 `MailInfo.Send` 推断意图，没有把外层空槽循环与 `CanGainItem` 的实现合并做可达性分析；原版只在先找到空槽后调用 `CanGainItem`，而后者发现任意空槽就直接返回 true。
- Prevention: 对看似存在的 legacy 分支同时核对调用条件、循环边界和被调函数，并构造状态表证明分支可达；不可达意图不能替代客户端实际可观察行为。
- Verification: Go 现在只为找到空槽的 Refine 物品发送 `RetrieveRefineItem`，背包填满后的后续物品继续留在工作台；单元测试和交互取消→KeepAlive→登出持久化 transcript 均通过。

### 2026-08-12 — 启动包排序必须区分角色物品与功能定义阶段

- Symptom: Quest 会话测试仍要求三个任务物品定义出现在 `MapInformation` 前，并一度准备把 Quest/Recipe 所需定义全部提前到地图包之前。
- Root cause: 把 Go bridge 的 `SelectInfo.ItemInfos` 目录与世界级 Quest/Recipe 定义混为一谈，没有沿 legacy `StartGameSuccess` 的实际调用顺序核对：`GetItemInfo` 只遍历角色三类物品格，随后才是 `GetMapInfo -> GetUserInfo -> GetQuestInfo -> GetRecipeInfo`；Recipe 的关联物品定义也由后置的 `CheckRecipeInfo` 发送。
- Prevention: 修改 bootstrap 顺序前同时列出 `StartGameSuccess` 调用链、每个 helper 的真实遍历范围和 FIFO `Enqueue` 行为；不能因为包都叫 `NewItemInfo` 就把不同来源统一提前。
- Verification: Quest 首次登录 transcript 改为 `MapInformation -> UserInformation -> quest NewItemInfo* -> NewQuestInfo`，通用 bootstrap 状态机继续区分角色物品和功能定义阶段；Go 全量普通/race 测试、vet、build 与 diff 检查通过。

### 2026-08-12 — 定时协议常量必须先查完整枚举

- Symptom: 启动状态机补任务定时包时曾使用不存在的 `protocol.ServerQuestExpired`，导致新增测试无法编译。
- Root cause: 按业务语义猜测了包名，没有先检索 legacy `ServerPacketIds` 和 Go 常量表；实际任务计时使用通用 `ServerSetTimer`。
- Prevention: 新增 transcript 常量前先用 `rg` 同时核对 legacy enum、packet class 和 Go 常量，并在同一修改中补显式 ordinal 断言；禁止从功能名称推导常量名。
- Verification: bootstrap helper 现在只接受已核实的 `ServerSetTimer`/`ServerChangeQuest`，协议和服务端全量门禁通过。

### 2026-08-12 — 持久化物品目录不能代替连接级已发送状态

- Symptom: Quest 定义在第一次登录顺序正确，但定义被写入 `SelectInfo.ItemInfos` 后，重登会把全部定义错误提前到 `MapInformation` 前；Mail 附件也可能因目录中已有定义而跳过其应位于 `ReceiveMail` 前的 `NewItemInfo`。
- Root cause: `gameItemCatalog` 同时承担服务端持久化定义目录和客户端当前连接已知集合；前者跨登录保留，后者必须随每次 StartGame 清空，两者语义不能合并。
- Prevention: 每个连接维护独立、并发安全的 sent-item-info 集合；初始角色格、Quest、Recipe、Mail、NPC 商品和显式 RequestItemInfo 各按原版阶段检查并标记，目录是否已有定义只决定服务端是否追加，不能决定是否发包。
- Verification: Quest 重登锁定“当前携带定义 → Map/User → 其余 Quest 定义”，Mail 嵌套附件锁定“CompleteQuest → NewItemInfo* → ReceiveMail”，UsedGoods 与重复 RequestItemInfo transcript 通过；Go 全量 test、race、vet、build 和 `git diff --check` 全部通过。

### 2026-08-12 — Goal 状态审计必须使用当前 CODEX_HOME 并关联线程生命周期

- Symptom: 只调用当前线程的 Goal 查询时看起来只有 1 个 Goal，但当前 Codex 实例的数据库实际有 4 条 `active` 记录，容易把“当前线程 Goal”误报成“整个实例正在执行的 Goal”。
- Root cause: Goal 查询接口按当前线程返回；同时若未先解析 `CODEX_HOME`，可能误查默认 `~/.codex`。子代理线程已经关闭后，其 Goal 行还可能残留为 `active`，单看 Goal 表不能代表仍在运行。
- Prevention: 回答 Goal 总数前先确认当前 `CODEX_HOME`，查询其 `goals_1.sqlite`，再与同目录 `state_5.sqlite` 的线程和 `thread_spawn_edges` 生命周期关联；分别报告“数据库标记 active”和“实际仍运行”，且不得手工改内部数据库清理残留。
- Verification: 本次关联审计确认 4 条 `active` 中仅主线程 1 条仍执行，另外 3 条都属于 `closed` 子代理；该主线程下 23 条子代理边全部为 `closed`，当前没有活跃子代理。

### 2026-08-12 — 跨会话同步必须按领域 revision 定向合并

- Symptom: Rental 为接收后台到期/死亡更新，把每次读包前的 world 同步扩大为整角色物品格和邮件覆盖，导致同一 session 刚完成的合成、装备、背包移动、精炼、修理、仓库和使用物品状态被 stale world 快照回滚；全量测试同时出现多类持久化和 transcript 失败。
- Root cause: world 快照既包含跨会话共享状态，也包含当前 session 自己拥有、但并非每条业务路径都立即回写 world 的局部状态；无版本判断的全量赋值把两种所有权混在一起。
- Prevention: 保留 Quest/Group 的既有定向同步；为 Rental 增加独立 revision，只在 Rental 生产路径实际修改网格/邮件时递增，并在 revision 变化时同步 Rental 所需字段。新增共享领域必须先明确所有权和变更版本，禁止为了接收一个后台字段而整角色覆盖。
- Verification: 修复后先前失败的 Craft、Equipment、ItemMove、Refine、Repair、Storage、Use/Delete 会话测试与 Rental 定向测试全部恢复通过。

### 2026-08-12 — 并行功能线交接后必须先恢复包级编译绿色

- Symptom: Rental 与移动兼容线合入共享工作树后，`cmd/crystal-server` 留下未使用的 `oldDirection` 和尚未实现的 `deliverMovementMapTransition`，后续工作处在不可编译中间态，放大了“迁移没有进展”的感受。
- Root cause: 多条功能线同时触碰 `main.go`/`world.go`，交接时只记录了剩余事项，没有在每个写入边界立即执行最小包级编译；新的兼容分支继续叠加在未完成的整合点上。
- Prevention: 并行线优先按互斥文件或稳定接口拆分；任何功能线完成写入后，整合者先运行受影响包的仅编译门禁，编译未恢复绿色前不再叠加另一条共享文件修改；未完成 helper 必须与调用点在同一整合步落盘。
- Verification: 本次 `go test ./cmd/crystal-server -run '^$' -count=1` 在 0.1 秒内准确定位两个整合缺口；修复后必须以同一命令恢复绿色，再进入 Rental 定向及全量门禁。

### 2026-08-12 — 地图切换包序列必须按触发路径拆分

- Symptom: Go 曾把地图坐标移动、NPC `ENTERMAP`、NPC `MOVE` 和 RequiredGroup 强制离开统一投递为 `MapChanged + UserLocation`，导致移动测试读取到后续 NPC 包，并掩盖 `ObjectTeleportIn` 的差异。
- Root cause: 复用了一个通用 transition helper，没有保留 legacy `CompleteMapMovement`、`Teleport(..., false)` 与默认 effectful `Teleport` 三条独立调用链。
- Prevention: transition 先标注触发类型，再分别锁定坐标移动/ENTERMAP 仅 `MapChanged`，MOVE/强制传送为 `MapChanged + ObjectTeleportIn`，TownRevive 使用自己的 `MapChanged` 路径；禁止用统一的 `UserLocation` 补包。
- Verification: 更新 movement、ENTERMAP、MOVE、RequiredGroup 和 Rental 顺序测试后，Go 全量普通/race 测试、vet、build 与差异检查通过。

### 2026-08-12 — ActionTime 迁移必须兼容连接队列与直接世界测试

- Symptom: 给 Turn/Walk/Run 写入 350/600ms `ActionReadyAt` 后，既有 net.Pipe 会话连续移动被同步读循环立即拒绝，大量距离门禁和可见性测试停在原地。
- Root cause: legacy 连接会把请求留在队列中等 `ActionTime` 到期再处理，而当前 Go 会话同步读取后立即执行；只迁移时间字段却没有迁移队列调度会改变外部行为。
- Prevention: 世界 helper 保留精确 capability/ActionTime 边界；会话入口保留单条 retry movement，在 `ActionReadyAt` 到期后重新派发，匹配 legacy `_retryList`，不能直接清零时间门禁。
- Verification: 直接 helper 的 capability 测试与连续 session movement、Craft/Shop 距离、地面拾取和静态可见性测试同时通过全量普通/race 门禁。
- Strengthening after P5 combat recurrence: melee、range、magic 也必须复用同一条会话 retry 边界；为其补门禁后，旧 `TestSessionFireBallTranscript` 仍把“立即收到冷却拒绝”当成正确结果，首次全包测试因此在 1800ms 后读到了重派成功的 `HealthChanged`。迁移 ActionTime/AttackTime/SpellTime 时必须同步审计既有 transcript 断言，区分世界 helper 的即时 capability 拒绝与连接层 `_retryList` 的延迟重派，不能为了保留旧 Go 测试而破坏 Legacy 会话行为。
- Verification after strengthening: FireBall 会话测试现锁定等待全局 SpellTime 后重派、再次扣 MP、更新方向并发送完整 Magic transcript；Haste/Fury 测试锁定 AttackSpeed 冷却，六技能测试锁定全局与单技能 CastTime 分离，`go test ./...` 与 `go test -race ./...` 全量通过。

### 2026-08-12 — 一次性异常排查不能变成固定汇报项

- Symptom: 用户只是临时询问一次异常现象，后续进度消息却持续重复相关分析，形成无关噪声。
- Root cause: 把一次诊断请求误解成了长期监控和每轮汇报要求。
- Prevention: 临时排查默认只回答当次；除非同类异常再次影响迁移、需要用户决策或用户明确追问，否则不主动重复诊断结论，也不把它加入固定进度模板。
- Verification: 后续迁移仅在整批完成、真实阻塞或需要用户决策时汇报，不再附带该一次性排查内容。

### 2026-08-12 — 跨语言 round-trip fixture 不得直接比较含 map 的结构体

- Symptom: 公会导出 schema 测试首次编译失败，测试用 `!=` 比较包含 `ItemStats` map 的 `protocol.ItemInfo`。
- Root cause: 把结构体整体比较误当成通用零值检查，未先确认其字段是否全部可比较。
- Prevention: 为跨语言 JSON fixture 做零值或完整对象断言前，先检查 slice、map、pointer 字段；包含不可比较字段时统一使用 `reflect.DeepEqual` 或逐字段断言。
- Verification: 零值 creation-cost Item 改用 `reflect.DeepEqual` 后，`go test ./internal/worlddata -count=1` 通过。
- Strengthening after recurrence: `reflect.DeepEqual` 仍会区分 nil 与空 slice/map；wire parser 会把零长度 Slots、AddedStats、Awake.Values 规范化为空非 nil 容器。构造协议 round-trip fixture 时必须按 parser 的规范形状显式初始化这些字段，或逐字段比较语义值，不能把 nil/空容器差异误判成序列化错误。
- Second strengthening after recurrence: 即时 Buff 双连接测试又准备使用 `observerBuff != buff`，而 `ClientBuffInfo` 内含 `ItemStats` map 与 `Values` slice；虽在编译前复读时发现，仍说明新增断言没有先执行可比较性检查。今后写 `==`/`!=` 前先展开目标类型字段；协议对象默认逐字段断言，确需整体比较时才使用 `reflect.DeepEqual`，并同时明确 nil/空容器规范。
- Verification after second recurrence: 观察者 AddBuff 改为 `reflect.DeepEqual`，随后 `go test ./cmd/crystal-server -run '^$' -count=1` 和完整即时 Buff net.Pipe 测试均通过。

### 2026-08-12 — C# 顶层导出器新增显式参数类型时必须核对命名空间

- Symptom: 公会 creation-cost helper 使用了 `GuildItemVolume` 作为参数类型，而该类型声明在 `Server.MirObjects`，原有 exporter imports 不包含该命名空间。
- Root cause: 调用点可通过类型推断访问成员，但抽成具名 helper 后需要编译器直接解析参数类型；静态审查最初只核对了字段，没有核对类型所属 namespace。
- Prevention: C# exporter 新增 helper 签名时，用原版类型声明反查 namespace，并显式加入对应 `using`；无 SDK 环境的静态 guard 同时锁定该 import 和签名。
- Verification: world exporter 已加入 `using Server.MirObjects;`，静态 schema guard 覆盖 import；`go test ./internal/worlddata -count=1` 通过，真实 C# 编译仍留待具备 .NET 8 SDK 的环境验证。

### 2026-08-12 — 公会默认创建费用必须解析 Legacy 物品名

- Symptom: Go 无 world export 时最初只要求 1,000,000 金币；补上默认 `WoomaHorn` 后，费用记录只有名称、索引为零，导致真实背包中的 Wooma Horn 无法匹配。
- Root cause: Legacy 默认配置先保存去空格物品名，再由 `LinkGuildCreationItems` 对完整物品目录解析；Go 把已解析的 `ItemInfo` 与未解析的默认名称当成了同一种输入。
- Prevention: 默认费用保留 Legacy 的金币加 WoomaHorn 两项；事务开始前按去空格、忽略大小写规则从角色物品定义解析名称，随后才执行原子校验和扣除。默认配置和 exporter 配置必须共用同一事务路径。
- Verification: auth 测试锁定 `WoomaHorn` → `Wooma Horn` 的解析与扣除，完整 NPC 会话锁定 `DeleteItem → LoseGold → GuildStatus`，Go 全量普通/race 门禁通过。

### 2026-08-12 — 注销通知快照与投递时机必须分离

- Symptom: 公会注销状态包若在 `world.leave` 前立即投递，会先写向正在关闭且无人读取的 net.Pipe，阻塞后续在线成员；若在 leave 后才构造，又丢失注销者姓名并退化为角色数字 ID。
- Root cause: 把“从在线 world 读取姓名并生成通知”和“向剩余会话实际写包”绑定在同一步，没有保留快照边界。
- Prevention: 离线流程先在玩家仍在线时提交 auth 状态并构造包含姓名的通知快照，完成 world 移除后再投递；通知接收者列表排除注销者，持久化在状态提交后执行。
- Verification: 双成员 world 测试锁定仅另一成员收到 `GuildMemberChange(Status=0, Name=leader)`；公会会话测试、Go 全量普通/race 门禁通过。

### 2026-08-14 — 状态魔法归属必须沿 Legacy 命中分支确认

- Symptom: 迁移矩阵和实现分析一度把 Slow/Frozen 当成 IceStorm 的待迁移状态，而 Legacy 的 IceStorm 实际只执行 3×3 MAC 区域伤害。
- Root cause: 根据技能名称和常见游戏直觉推断状态归属，没有先把施法入口、延迟命中 switch 和 `AddPoison` 调用点连成完整可达路径；真正施加 Slow/Frozen 的技能是 FrostCrunch。
- Prevention: 每批状态型技能先从 Legacy 命中 resolver 建立“spell → damage → secondary effect”表，再检索所有状态创建调用点交叉确认；文档、实现和测试必须共用该表，禁止从技能名称推断效果。
- Verification: Legacy spell switch 确认 FireBang/IceStorm 只有区域 MAC 伤害、FrostCrunch 命中后才执行 Slow/Frozen 的独立门槛与概率；Go 区域魔法测试保持 IceStorm 纯伤害 transcript，FrostCrunch 领域和三会话测试覆盖完整状态生命周期。

### 2026-08-14 — 手工 world tick 测试必须隔离后台 ticker 并使用会话屏障

- Symptom: FrostCrunch 三会话测试手工推进命中/到期 tick 时，100ms 后台 world ticker 可能抢先发布状态；Frozen 攻击测试读到入口先发送的 `UserLocation` 后立即检查 world，又可能早于真正的 admission 处理。
- Root cause: 确定性 synthetic tick 与生产后台 ticker 同时驱动同一世界，且把 handler 中间包误当成请求完成信号；`net.Pipe` 的包到达只证明该次写入完成，不证明后续领域逻辑已结束。
- Prevention: 需要手工时间轴的会话测试在状态创建前停止该 world 的测试 ticker，并确认可能已选中的空 tick 返回；读取入口中间包后再发送并消费 `KeepAlive`，用后续请求作为所属会话完成屏障，再检查共享状态或断言没有广播。
- Verification: 后台 ticker 隔离后，三会话 FrostCrunch transcript 稳定锁定伤害、两条 Chat、Slow/Frozen 广播、Frozen 拒绝以及逐步清除；Frozen 攻击后的 KeepAlive 屏障确认方向和动作队列均未变化。
- Strengthening after recurrence: 毒状态的 `Elapsed` 按实际处理事件次数递增，不会因一次 synthetic tick 的墙上时间跨越而补齐多个周期；到期测试必须按递增且严格晚于 `TickAt` 的每个 tick 逐步驱动，再单独执行清除广播阶段。
- Verification after recurrence: AI=95 FlameAssassin 首次到期测试把 8 秒跳跃误当成 8 次处理，仅得到 `Elapsed=2`；改为逐秒、递增纳秒边界的 8 次 tick 后，Slow 到期与 `Poisoned/ObjectPoisoned(None)` 顺序通过。

### 2026-08-14 — 状态门禁必须收窄到原版 capability 边界

- Symptom: FrostCrunch 初版在普通宠物和 Conquest archer 的外层 AI tick 遇到 Frozen 就直接 `continue`，同时跳过了普通宠物驯服到期/跨图召回和 archer 搜索目标等不属于移动或攻击的生命周期工作。
- Root cause: 把“Frozen 时不能 Move/Attack”简化成“整个对象不处理”，没有沿 Legacy `MonsterObject.Process` 的顺序区分 tame lifecycle、`ProcessSearch`、recall 与最终 `CanMove`/`CanAttack` 门禁。
- Prevention: 每个控制状态先标注它约束的原版 capability；门禁只能放在对应动作提交点，外层对象 tick、到期清理、持久化、召回和搜索继续运行。新增状态测试必须同时覆盖“受禁动作不发生”和“非受禁生命周期仍推进”。
- Verification: Go 已把 Frozen 从普通宠物/archer 外层循环移到 follow/target/attack 动作点；回归测试确认宠物保留目标且不移动/攻击、Frozen 期间仍执行 tame 到期广播，archer 仍更新搜索/缓存目标但不创建投射物，相关聚焦测试通过。

### 2026-08-14 — 当前批次收尾后停止的指令优先于长期迁移计划

- Symptom: 用户明确要求“干完这个就不要再做”时，既有计划仍包含分析和实现下一批功能，存在当前提交完成后继续扩展工作范围的风险。
- Root cause: 把长期迁移 Goal 的持续执行惯性当成当前轮默认，没有立即按最新的停止边界收窄计划。
- Prevention: 收到“完成当前批次后停止”指令后，只执行在途批次必需的测试、提交、C# 零变化检查和工作树核验；不得分析或实现下一批，也不得把尚未 100% 完成的整体迁移 Goal 标记完成。
- Verification: 本轮仅收口并提交 ProtectionField、Rage、SwiftFeet 及经验记录，核验两个仓库后停止，没有开启下一批源码修改。

### 2026-08-14 — Observer transcript 新增 helper 后必须立即做包级编译

- Symptom: Observer 会话测试初稿引用了不存在的 UserInformation parser、错误哨兵和未解析的 Name 字段，随后真实 transcript 又暴露了目标连接自身的攻击/远程/施法包顺序与战斗退出锁。
- Root cause: 连续扩展测试时按意图猜测同包 helper 签名，并把观察者视角的转发包误当成目标连接唯一可见包，没有先复用现有 parser、核对 handler 写入顺序和 logout gate。
- Prevention: 新增 Observer 测试先检索整个 package 的 parser/错误值/返回签名，立即运行 `go test ./cmd/crystal-server -run '^$'`；每条多连接 transcript 分别列出目标、自身、观察者和退出 gate 的包矩阵，再写断言。
- Verification: 复用 `parseIntelligentCreatureSessionUserInfo` 并补齐 Name 后，Observer 拒绝、Admin 绕过、切换/generation、退出、持久 toggle、静态/地面对象顺序及服务端整包测试均通过。

### 2026-08-14 — 广播包必须按源 ObjectID 转发给 Observer

- Symptom: 复核 Observer forwarding 时发现把每个 `worldPlayer.Send` 回调都转发给该回调所属玩家的观察者，会把发送给附近接收者的源玩家动作包误投给错误观察者并可能重复。
- Root cause: 把“通知接收者的 Send”误当成“动作源的 Send”，没有区分 `notifyPlayers` 的 recipient 与产生 `ObjectTurn/Attack/RangeAttack/Magic` 的 source。
- Prevention: Observer forwarding 只由动作源路径显式提交带源 ObjectID 的 packet；需要复用 world notification 时先确认源包是否也会发给源连接，禁止在通用 recipient callback 中推断来源。
- Verification: 攻击、远程攻击改为 source `ObserverPacket`，移动和魔法保留 source path 转发；全仓普通/race 测试、vet/build 与 Observer transcript 均通过。

### 2026-08-14 — CanFly 必须沿 Legacy 的逐格路径并区分技能例外

- Symptom: Go 的远程与单目标魔法已有目标距离和延迟命中，却没有按地图墙体检查投射路径；直接把所有远程魔法统一加墙门禁还会错误阻断 ThunderBolt/FlameDisruptor 及 MentalState TrickShot。
- Root cause: 只迁移了目标选择/命中距离，没有把 `MapObject.CanFly` 的八方向逐格推进、HighWall/LowWall 的共同 `Valid` 语义和各 spell 入口的例外条件连在一起；Go 地图属性常量也曾与 Legacy ordinal 反置。
- Prevention: 先固定 `CellAttribute` 为 Walk=0、HighWall=1、LowWall=2，再复刻 `DirectionFromPoint` + `PointMove` 的每格校验；按 Legacy switch 建立需要 CanFly 的技能表，普通 RangeAttack 与 Straight/DoubleShot 单独保留 MentalState=1 绕过，FireBounce 每一跳用来源对象当前位置重验路径，延迟命中仍使用 Legacy 的目标位置窗口。
- Verification: Go 新增地图 ordinal/解析、两种墙与边界、单目标魔法、ThunderBolt/FlameDisruptor 例外、普通远程/TrickShot、FireBounce 墙前跳转和延迟目标移动测试；受影响包及 `go test ./...` 均通过。

### 2026-08-15 — Observer 接管新增可见物品时必须更新完整 transcript

- Symptom: RangeAttack 装备准入 fixture 为观察目标增加装备后，Observer 会话在 `ClientObserve` 后读到 `NewItemInfo`，旧测试却直接期待 `MapInformation`。
- Root cause: 目标的物品定义按连接可见性规则在观察接管阶段先发送；测试只更新了目标本身的装备状态，没有重新展开观察者接管的定义包顺序。
- Prevention: 任何会改变目标可见物品集合的 Observer/Inspect fixture，都要分别列出目标连接与观察者连接在请求后的完整 packet matrix，并在 `MapInformation` 前消费和解析新增 `NewItemInfo`。
- Verification: Observer transcript 已锁定目标装备定义 → `MapInformation` → `UserInformation` 顺序；RangeAttack 装备门禁相关定向测试和 `go test ./cmd/crystal-server -count=1` 均通过。

### 2026-08-15 — 手工 world tick 停止 ticker 必须等待 goroutine 退出

- Symptom: 全仓 race 门禁曾在 FrostCrunch 会话的合成时间轴中看到 Frozen 清除通知为空；单测偶发通过。
- Root cause: 测试只关闭后台 ticker 的 stop channel 并固定 sleep，没有确认 ticker goroutine 已退出；真实时间 tick 可能在合成时间轴 tick 前处理 1970 年的毒状态。
- Prevention: world ticker 暴露仅供内部同步的完成 channel；手工时间轴 fixture 关闭 ticker 后等待该 channel，再调用 synthetic `world.tick`，禁止用固定 sleep 充当 goroutine 完成屏障。
- Verification: FrostCrunch 普通与 race 定向测试各连续 10 次通过；提交前重新执行全仓 race 门禁。

### 2026-08-15 — 已核验的 Go 根目录不得再次手工拼接

- Symptom: MentalState 只读检索曾把已确认的 Go 根目录重复拼成 `Crystal.GoServer.GoServer`，命令在启动前失败。
- Root cause: 已有绝对根目录没有直接复用，临时手写路径引入重复目录片段；源码未被读取或修改。
- Prevention: 每次跨仓库调用只使用先前 `git rev-parse --show-toplevel` 的完整根目录，并在命令前检查 `test -d`/`test -f`；失败命令不得作为证据，立即切换到新调用重跑。
- Verification: 纠正后 Go 查询完整返回，Legacy/Go 工作树和 C# 差异均未产生额外变化。

### 2026-08-15 — 支援魔法会话 fixture 必须保持在线状态与持久状态一致

- Symptom: SoulShield 会话 transcript 在预期 `AddBuff` 后读到额外 `HealthChanged`，失败后后台 session 仍尝试保存已被测试清理的临时目录。
- Root cause: fixture 在登录后把运行时 MP 改成远高于角色 MaxMP 的合成值，Buff 刷新按真实装备/等级重算并钳制生命资源；失败路径又先结束测试，掩盖了后续异步保存错误。
- Prevention: 网络 transcript 只在与登录 bootstrap 一致的 MaxHP/MaxMP 范围内修改运行时字段；若必须手工推进 ticker，先设置读取屏障并在断言失败前关闭会话，避免把 cleanup 后的后台日志当作首要根因。
- Verification: MP 改为当前 MaxMP 后，`HealthChanged → DeleteItem → UserLocation → Magic → AddBuff` 顺序和 JSON 物品/Buff 状态均稳定通过。

### 2026-08-14 — 跨仓库只读命令的工作目录必须与路径参数同仓库

- Symptom: 继续 P5 前的只读探查在 Go 工作目录读取 Legacy 的 `tasks/lessons.md`，命令仅返回文件不存在，但没有得到预期源码证据。
- Root cause: 只核对了调用目标仓库，没有同时检查命令参数中的相对路径属于哪个仓库，导致 Legacy 文件路径被错误地带入 Go 调用。
- Prevention: 每条跨仓库命令只使用当前仓库的相对路径；读取 Legacy lessons 与 Go 源码必须拆成两个独立调用，并在调用前后分别核对 `git rev-parse --show-toplevel`。失败的只读输出不得作为实现依据。
- Verification: 该命令在写入前失败且两个工作树均无源码变化；后续将 Legacy lessons 与 Go 源码查询拆开，并重新核对两仓库状态。

### 2026-08-15 — 嵌套魔法分支修改后必须立即做语法门禁

- Symptom: 支援魔法分支初版漏掉嵌套 `if` 的闭合括号，包级编译在后续函数定义处才报告 `expected '('`。
- Root cause: 在已有长 `if/else if` 链中一次性插入两层局部逻辑，没有先验证嵌套边界。
- Prevention: 长分支新增后立即用 `gofmt` 和 `go test <affected-package> -run '^$' -count=1`；对每个局部 `if` 先明确闭合范围，再加入行为断言。
- Verification: 修正闭合边界后包级编译、支援魔法定向世界测试和真实 net.Pipe transcript 均通过。

### 2026-08-14 — 跨仓库初始核对不得放入同一并行编排

- Symptom: 本轮开始时把 Legacy lessons、Legacy 状态和 Go 状态/文件清单放进同一个 Promise.all，虽然只是只读且未产生代码写入，但违反了项目要求的跨仓库调用边界。
- Root cause: 为减少往返而把不同仓库的独立证据查询视为可安全并行的任务，没有执行“每个 functions.exec cell 固定一个仓库”的机械检查。
- Prevention: 跨仓库任务的每个工具编排 cell 只允许一个已核验的绝对仓库根目录；需要比较时先完成一个仓库的根目录/状态/证据读取，再在新的独立 cell 切换另一仓库。禁止在同一 Promise.all、命令或路径变量中混放两侧。
- Verification: 当前并行调用只做了读取，没有 .cs 或源码写入；后续已先完整读取 Legacy lessons，并将 Legacy/Go 核对拆为独立调用，继续迁移前再执行 C# 零差异门禁。
- Strengthening after recurrence: 本轮虽已知该规则，仍把 Legacy 与 Go 的状态查询放入同一个 `Promise.all`；以后任何跨仓库任务的首个 cell 也必须只绑定一个绝对根目录，不能以“全部只读”或降低延迟为例外。执行前逐项检查编排中的每个调用，发现第二个仓库立即拆到下一 cell。
- Verification after strengthening: 本次混合查询只产生读取输出，未写入 C# 或源码；已重新独立读取 Legacy lessons，并在后续调用中只使用 Go 根目录，未采用混合调用的路径证据。

### 2026-08-14 — 新增会话测试变量必须避免跨阶段类型复用

- Symptom: Poisoning/Purification 会话测试包级编译失败，把 StoredMagic 结果赋给了前面用于协议 MagicResult 的同名局部变量，随后访问不存在的 Experience 字段。
- Root cause: 在一个长 transcript 中复用了语义相近但类型不同的变量名，没有在新增阶段前核对当前作用域已有声明。
- Prevention: 每个协议阶段使用带领域后缀的唯一变量名（例如 purifyPacket、storedPurify），新增测试后先执行受影响包的仅编译门禁，再运行行为测试。
- Verification: 本次失败发生在测试编译阶段且未运行服务端；修复后将用包级编译和 Poisoning/Purification 定向测试分别验证。

### 2026-08-14 — Legacy 定宽整数进入领域类型必须显式转换

- Symptom: `RespawnTickOption.UserCount` 为 `int`，读取 Legacy `int32` 后直接赋值导致受影响包编译失败。
- Root cause: 二进制定宽字段进入 Go 领域模型时遗漏了显式窄/宽类型边界。
- Prevention: 读取 Legacy 整数先落到同宽临时变量，再显式转换到领域类型；新增解析字段后立即执行受影响包的编译门禁。
- Verification: 改为 `value.Options[index].UserCount = int(userCount)` 后，`internal/worlddata`、`internal/legacyworld`、`internal/config` 和 `cmd/crystal-server` 包级测试通过。

### 2026-08-14 — 扩展二进制 fixture 后截断测试要接受结构化 EOF

- Symptom: 为 RespawnSave 增加多条定宽记录后，截断数据库回归先得到 `count ... cannot fit`，旧断言只接受 `unexpected end of file`。
- Root cause: binary reader 会在计数声明与剩余字节明显不匹配时提前返回结构化容量错误，而不是继续读到 EOF；fixture 形状变化使该合法错误路径变得可达。
- Prevention: 截断数据库测试断言错误类别（EOF 或计数无法容纳），不要绑定单一读取深度的错误文本；每次扩展 fixture 后运行完整 exporter 包测试。
- Verification: 断言同时覆盖两种截断错误后，`go test ./internal/legacyworld -count=1` 通过。

### 2026-08-14 — 导入全局计时器后必须重算未保存重生组的下一 tick

- Symptom: 世界先按默认计时器创建重生组，再导入 Legacy `CurrentTickcounter` 时，未匹配 `RespawnSave` 的组仍保留默认基准的 `NextSpawnTick`，可能在启动后立即重生。
- Root cause: 加载顺序把静态组构造与计时器覆盖分开，却只对有保存记录的组应用了新计时器。
- Prevention: 配置计时器后，保存记录匹配组使用持久化 `NextSpawnTick`，其余 tick 组统一以导入的当前计数器加 `RespawnTicks` 重算；用非零导入计数器测试无保存记录路径。
- Verification: Go 重启/持久化测试锁定初始 `CurrentTickcounter=7` 时未保存组的下一 tick 为 9，随后全仓普通/race 门禁通过。

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

### 2026-08-14 — Go 工作目录中不得使用 Legacy 相对源码路径（再次强化）

- Symptom: 在 Go 根目录执行 Legacy `Server/...` 只读检索时，命令因路径不存在失败；没有写入，但输出不能用于语义判断。
- Root cause: route 可观察性核对切换仓库后复用了上一调用的相对路径，违反了命令参数单仓库边界。
- Prevention: 每次跨仓库查询都拆成独立工具调用，先核对 `git rev-parse --show-toplevel`，再只使用当前根目录下的相对路径；错误输出不得作为实现依据。
- Verification: 本次失败发生在读取阶段且工作树无变化；后续 Legacy 与 Go 查询分别核对根目录后再继续。

### 2026-08-14 — 移动可见性矩阵 fixture 必须同时保存旧坐标与新坐标

- Symptom: route 可见性测试预期离开范围/进入范围的 `ObjectRemove` 与 `ObjectMonster`，实际所有接收者只收到 `ObjectWalk`。
- Root cause: 测试调用通知投影时仍把 monster 保留在旧坐标，距离差没有跨越 16 格边界；断言场景没有真正表达移动事件。
- Prevention: 构造移动通知 fixture 时显式传入 `oldX/oldY`，并保证待投影对象的坐标已经是 `newX/newY`；先逐接收者计算 old/new 可见性，再断言包序。
- Verification: 将 monster 新坐标设为 17、旧坐标设为 16 后，矩阵得到 leaving=`ObjectRemove`、staying=`ObjectWalk`、entering=`ObjectMonster`→`ObjectWalk`。

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

### 2026-08-15 — Monster AI 距离环 fixture 不得让观察者抢占目标

- Symptom: AI=4 SpittingSpider 定向测试预期攻击距离 2 的目标，却排入了距离 1 的观察者，导致延迟动作目标和预期时间均不符。
- Root cause: Legacy `FindTarget` 按距离环扫描，测试把可观察连接放在更近格子，观察者本身也满足普通玩家目标门禁。
- Prevention: 构造搜索/目标选择 fixture 时先列出所有在线玩家的距离环顺序；观察者应放在目标之后的环、设为安全区，或显式验证它不会成为合法目标，同时保持仍在数据可见范围内以覆盖通知矩阵。
- Verification: 将观察者移到目标之后的远环后，AI=4 的 `ObjectAttack`、400ms 直线命中、伤害/Green poison 状态和 observer 包序定向测试通过。

### 2026-08-15 — 长 Markdown 行补丁必须使用已读取的完整上下文

- Symptom: 更新 P5 矩阵时使用了带省略号的占位行作为 `apply_patch` 上下文，补丁校验失败，没有修改文件。
- Root cause: 没有先读取实际长行就假定其内容可用缩写匹配；Markdown 矩阵的整行字段不允许把省略号当作通配符。
- Prevention: 长行文档修改先用当前仓库的 `sed`/精确检索读取真实上下文，再做最小完整行替换；补丁失败后重新读取并确认文件未变，禁止凭失败输出继续判断。
- Verification: 失败补丁未产生写入；随后读取实际 P5 行并用完整上下文更新 AI=4 说明，`git diff --check` 将在提交前验证格式。
- Strengthening after recurrence: 本批再次把不完整上下文用于 AI=29 矩阵更新，补丁仍在校验阶段失败；以后每次长行更新必须先读取包含前后完整句子的固定行块，并只替换该实际块，禁止手写任何省略号或猜测尾句。
- Verification after strengthening: 第二次失败补丁没有写入；随后读取 `sed -n '108,132p'` 的真实 note，按完整上下文修正重复句并加入 AI=29 说明，Go 文档 diff 检查保持通过。
- Strengthening after second recurrence: AI=100 更新虽使用了真实上下文，仍把被匹配的保留句再次复制进 replacement，产生重复文本；以后采用“删除旧句再插入新句”的最小补丁，应用后必须立即 `sed` 复读目标块并检查相邻句不重复。
- Verification after second strengthening: 重复句在提交前已被移除，AI=100 note 的实际块复读正确；Go 定向测试继续通过，文档 diff 检查将在门禁中复核。
- Strengthening after third recurrence: AI=65 更新第一次仍按视觉换行猜测物理行边界，导致完整上下文匹配失败；长 Markdown 补丁前必须用 `sed -n l` 确认实际行边界，并优先锚定已读取的短、唯一连续句。
- Verification after third strengthening: 失败补丁未产生写入；随后按 `sed`/`sed -n l` 的真实行块成功加入 AI=65 说明，Go 文档 `git diff --check` 与全量门禁通过。
- Strengthening after fourth recurrence: 即使目标段落已复读，替换上下文中的标点或斜杠差异也会使 Markdown 补丁整块拒绝；应用前必须逐字复制实际行，补丁失败后重新读取目标段，禁止凭近似文案重试。
- Verification after fourth strengthening: 本批 AI=94 矩阵首次补丁因把实际 `HP/私有包` 写成近似标点而拒绝，未产生写入；重新检索真实行后按精确上下文追加 lessons，Go 文档随后通过差异检查。
- Strengthening after fifth recurrence: AI=97 矩阵更新再次因把视觉折行和近似句子当作完整上下文而拒绝；以后长段落只在 `sed -n l` 确认物理行后，以已读取的短唯一句插入，禁止把相邻保留句复制进 replacement。
- Verification after fifth recurrence: 失败补丁未写入；随后按真实 `packet matrices are covered.` 行重新插入 AI=97 说明，Go 文档 `git diff --check` 和全量门禁通过。

### 2026-08-15 — SandWorm 测试新增变量必须实际参与断言

- Symptom: SandWorm 定向测试新增 `impact` 变量后，Go 包级编译因 declared and not used 失败，行为测试尚未运行。
- Root cause: 为记录延迟命中结果引入了局部变量，但后续断言仍直接读取通知切片，没有把变量接入验证路径。
- Prevention: 新增测试局部变量后立即确认其用于断言、返回值或日志；先运行 `go test ./cmd/crystal-server -run '^$' -count=1`，编译绿色后再运行定向行为测试。
- Verification: 删除未使用变量后，后续将以包级编译、SandWorm 定向测试和完整 Go 门禁确认该测试实际执行并保持绿色。
- Strengthening after recurrence: AI=77 HellPirate 测试再次声明未使用的 `impact`，说明仅在测试结束前检查不足；新增每个分支的结果变量后，必须在同一 patch 中写入 HP、packet ID 或 payload 断言，并在继续扩展测试前运行仅编译门禁。
- Verification after strengthening: 本次复发在 `go test ./cmd/crystal-server -run '^$' -count=1` 阶段被捕获，修正前未执行行为测试；后续修复后将再次运行包级编译和 AI=77 定向测试。

### 2026-08-15 — Monster AI 新增攻击属性必须先核对实体字段

- Symptom: AI=66 CrazyManworm 分支初版直接访问 `worldMonster.MinMC/MaxMC`，包级编译报字段不存在，行为测试尚未运行。
- Root cause: 依据 Legacy 怪物统计概念推断 Go 运行实体必然保存 MC 字段，没有先读取当前 `worldMonster` 的完整字段和定义统计读取 helper。
- Prevention: 新增 AI 分支引用攻击属性前先核对当前实体结构及 materialize/统计访问路径；缺少字段时复用定义值 helper 或在同一批次完整贯通字段，随后立即运行受影响包仅编译门禁。
- Verification: 修正 MC 读取边界后，将以 `go test ./cmd/crystal-server -run '^$' -count=1` 和 AI=66 双分支定向测试确认编译及行为均恢复绿色。
- Strengthening after recurrence: AI=92 FlameSpear 再次把 MC 当成 `worldMonster` 运行字段，说明新增分支的所有攻击属性（包括 MC/SC）都必须同时核对已有生产 helper 的定义统计读取；包级编译失败时不得继续写测试或文档。
- Verification after recurrence: 本次错误在仅编译门禁捕获且未运行行为测试；改用 `monsterStatValue(info, statMinMC/statMaxMC)` 后，受影响包仅编译恢复绿色。

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

### 2026-08-15 — 跨仓库源码检索参数复发时必须作废混合输出

- Symptom: 核对 AI=112/76 时在 Legacy workdir 的 `rg` 命令中附带了 Go 的 `cmd/crystal-server` 路径；Legacy 查询有结果，但 Go 部分以路径不存在退出，不能作为证据。
- Root cause: 为并列比较而在同一 shell 中复用两个仓库的相对路径，未执行命令参数的单仓库 allowlist 检查。
- Prevention: 每个源码证据调用只能包含当前 workdir 的路径；Legacy 与 Go 必须使用不同的独立工具调用，任一退出码 2 的混合输出全部作废，不得据此决定实现。
- Verification: 本次命令只读且无文件变化；后续将分别在 Legacy 根目录读取 C#，再在 Go 根目录读取 Go 协议/实体，提交前继续做两仓库 C# 零变化检查。
- Strengthening after recurrence: 即使当前调用的工作目录是 Legacy，命令末尾追加 Go 的相对路径仍会制造混合输出；读取命令的路径参数必须在提交前逐项通过当前仓库 allowlist，发现另一侧路径立即拆到新的、已核验根目录调用。
- Verification after recurrence: 本次误带 `cmd/crystal-server` 的查询只在读取阶段返回路径不存在且没有写入；后续实现判断将作废该输出，并只采用单仓库调用的成功结果。
- Strengthening after second recurrence: 即使 Go 命令主体只读取 Go 源码，末尾追加 Legacy 统计或 C# 路径仍会让整条证据调用混合并失效；执行前必须逐项检查命令参数是否只包含当前仓库的已验证前缀，发现另一侧路径立即拆到新的 functions.exec cell。
- Verification after second recurrence: 本次 Go 只读查询因附带 Legacy 路径返回不存在，输出未用于实现判断且没有写入；后续已按单仓库调用重新核对统计字段，继续迁移前保持该边界。

### 2026-08-15 — 多次命中 transcript 必须应用 Struck 冷却

- Symptom: AI=96 FlameQueen 的第二个 50ms 间隔范围命中已正确扣血，但测试错误期望再次收到 `ObjectStruck`；实际目标和观察者只收到 `DamageIndicator`，目标另收到 `HealthChanged`。
- Root cause: 多段延迟动作测试按每次命中复用首次命中的包模板，遗漏了 Legacy `MonsterStruckReadyAt` 的 500ms 节流边界。
- Prevention: 多命中 transcript 先按每个命中时刻与目标的 `MonsterStruckReadyAt` 计算私有/广播包，再分别生成目标和观察者矩阵；后续命中不能默认追加 `Struck`/`ObjectStruck`。
- Verification: 修正 AI=96 第二次命中期望为 `DamageIndicator -> HealthChanged`（观察者仅 `DamageIndicator`）后，FlameQueen 定向测试通过。

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

### 2026-08-15 — 新增战斗 fixture 写入 ItemStats 前必须初始化 map

- Symptom: HellBomb AC/敏捷回归在行为阶段 panic，原因是向 `player.Stats` 写入高敏捷值时 map 为 nil。
- Root cause: fixture 只填了结构体标量字段，未沿生产角色初始化路径创建可写的 `protocol.ItemStats` map。
- Prevention: 新增战斗属性边界前显式用 map literal 初始化 `ItemStats`，并先运行定向测试而不是只依赖包级编译。
- Verification: 修正后将复跑 HellLord/HellBomb 定向测试，确认无 panic、AC 命中和敏捷负例均由实际行为断言覆盖。

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

### 2026-08-15 — Go 源码查询不得夹带 Legacy 路径（再次强化）

- Symptom: AI=104 对照读取时，Go 工作目录的同一条只读命令误带 `Server/MirObjects/Monsters/ZumaMonster.cs`，命令在读取阶段报路径不存在；没有写入，但该调用的其他输出不能作为证据。
- Root cause: 为并列读取 Legacy 行为而复用了跨仓库相对路径，没有在 `functions.exec` cell 内执行当前仓库路径 allowlist 检查。
- Prevention: Go cell 只允许 `cmd/`、`internal/`、`docs/`、`README.md` 等 Go 路径；Legacy 对照必须在结束 Go 调用后以新的、已核验 Legacy 根目录调用读取，混合调用的全部输出一律作废。
- Verification: 错误调用未产生文件变化；随后分别在 Legacy 根目录读取 `ZumaMonster`/`DemonGuard`，再在 Go 根目录读取 `worldMonster`/复活路径，后续实现只采用独立调用的成功输出。

### 2026-08-15 — AI=101 测试必须复用当前 Monster stat 标识符

- Symptom: AncientBringer 初版 Go 测试使用不存在的 `monsterStatMinMC`、`monsterStatMaxMC` 等名称，包级编译在行为测试前失败。
- Root cause: 测试夹具按 Legacy 统计概念猜测 Go 常量名，没有先读取当前 `worlddata`/monster AI 使用的完整 stat 标识符。
- Prevention: 新增怪物夹具前先在整个 Go package 检索生产声明和现有用法，逐项复用真实 `statMinMC`/`statMaxMC` 等标识符；首次 patch 后立即运行 `go test ./cmd/crystal-server -run '^$' -count=1`。
- Verification: AncientBringer 夹具改用当前 stat 常量后，包级编译、AI=101 定向测试和服务端整包测试均通过。

### 2026-08-15 — 混合怪物 AI 必须拆分攻击冷却与移动准入

- Symptom: AI=102 IceGuard 在攻击冷却期间被通用 `monsterCanAttack` 门禁提前返回，无法复现 Legacy 仍可追击移动的行为；延迟冰击若只在排队时校验目标，还会在目标进入安全区后错误造成伤害。
- Root cause: 把“当前不能发起攻击”误当成“当前不能处理目标移动”，并把排队时的目标资格当成延迟命中时的最终资格；Slow/Frozen 又需要两个独立概率门和各自的可观察状态包。
- Prevention: 对混合近战/远程 AI 先执行目标有效性和移动分支，再在真正攻击提交点检查攻击冷却；所有延迟动作在 impact tick 重验攻击者、目标、地图、安全区和存活状态；复合状态按 Legacy 顺序分别建模和投递。
- Verification: AI=102 定向测试锁定相邻 `ObjectAttack`/MAC 防御、远程 500ms `ObjectRangeAttack`、Type=0/1 分支、冷却期间 `ObjectWalk`、安全区重验及 Slow/Frozen 观察者 transcript；服务端整包测试通过。

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

### 2026-08-15 — 跨仓库对照命令必须拒绝另一仓库路径

- Symptom: Go 根目录的一次只读命令误带 Legacy `Server/...` 路径；该路径错误使整条输出不能作为迁移语义证据。
- Root cause: 对照 Legacy 的源码检索时没有把命令参数限制为当前 Go 仓库，复用了上一侧的相对路径。
- Prevention: 每次切换仓库都用独立调用核验 `git rev-parse --show-toplevel`，命令只允许当前根目录下的路径；混合调用的所有输出全部作废并重跑。
- Verification: 错误调用只发生在读取阶段且无写入；随后以独立 Legacy/Go 调用完成 AI=105 对照，C# 工作区无变化。

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

### 2026-08-15 — Go AI 距离常量必须匹配 int32 几何运算

- Symptom: AI=108 MudZombie 首次包级编译因 `distance > legacyMudZombieLineDistance` 的 `int32`/`int` 比较失败。
- Root cause: 新增距离常量显式声明为 `int`，而世界坐标和 `maxInt32` 结果使用 `int32`；Go 不允许这两个不同的具体类型直接比较。
- Prevention: 几何阈值优先使用无类型常量，或在声明处与坐标运算统一为 `int32`；新增 AI 后立即执行包级编译，不把类型错误留到全量门禁。
- Verification: 将 MudZombie 线距离改为无类型常量后，包级编译和四个 MudZombie 定向测试通过。

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

### 2026-08-15 — 每次 Go 工具调用必须复核完整仓库根目录

- Symptom: 本轮恢复 AI=109 时，一次 Go 命令误用了 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer.GoServer`，进程在启动前因目录不存在而失败。
- Root cause: 手工复制已知 Go 根目录时重复了目录片段，没有在工具调用前重新核对完整绝对路径。
- Prevention: 每次 Go 工具调用都直接使用已验证的 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，并在切换或写入前单独执行 `git rev-parse --show-toplevel`/目标存在性检查；失败调用的输出不作为证据。
- Verification: 错误命令未启动且没有文件变化；随后恢复到正确根目录，继续使用单仓库调用完成 AI=109 门禁与提交前核对。

### 2026-08-15 — AI 攻击属性必须复用 worldMonster 的定义统计边界

- Symptom: AI=111 WhiteMammoth 初版包级编译失败，代码直接访问不存在的 `worldMonster.MinMC/MaxMC` 字段，行为测试尚未运行。
- Root cause: 按 Legacy 统计概念假设运行实体保存 MC 标量，未先核对 Go `worldMonster` 的实际字段和现有 `monsterStatValue` 读取路径。
- Prevention: 新增 AI 分支引用 MC/SC 等属性前先读取实体结构及 materialize/统计 helper；缺少运行字段时统一从 `monster.Info` 用真实 stat ID 读取，并在首次 patch 后立即运行受影响包的仅编译门禁。
- Verification: 编译错误在行为测试前被捕获且未写入其他文件；修复后将用包级编译与 WhiteMammoth 定向测试确认统计读取和三条攻击分支。

### 2026-08-15 — WhiteMammoth PoisonTarget 的 chance=0 只做一次抗性检定

- Symptom: WhiteMammoth stomp 初版动作把 `PoisonTarget(..., 0, ...)` 迁移为两次抗性检定，和 Legacy 的可观察抵抗概率不一致。
- Root cause: 沿用了其他已迁移怪物动作的双检定字段，没有逐行读取 WhiteMammoth 的 `PoisonTarget` 调用及其单次 `PoisonResistWeight` 门禁。
- Prevention: 每个新 AI 的毒效果都要独立展开 chance、抗性检定次数、值计算顺序和状态包；不能从相邻 AI 复用默认次数。
- Verification: WhiteMammoth 动作断言固定 `PoisonResistChecks=1`，并用抗性 roll 计数测试确认 stomp 只消费一次抵抗检定。

### 2026-08-15 — Go AI 定向测试的随机上界重叠必须按调用阶段断言

- Symptom: WhiteMammoth 抗性回归测试的 switch 同时声明了 `case 10` 和 `case poisonResistWeight`，包级编译在行为测试前失败。
- Root cause: 测试把同一个随机上界用于分支和抗性阶段，却试图用常量 case 区分调用语义。
- Prevention: 随机上界相同时按已知调用顺序或阶段计数记录消费，不要在 switch 中重复声明等值 case；新增测试后立即运行包级仅编译门禁。
- Verification: 测试改为统计 `poisonResistWeight` 的实际调用次数，包级编译和 WhiteMammoth 四条定向测试均通过。

### 2026-08-15 — AI=113 对照必须在根目录核验后拒绝跨仓库路径

- Symptom: 读取 ArcherGuard 的 Go 侧字段时，工具输出的仓库根目录与预期不符，命令还夹带了 Legacy `Server/...` 路径；该输出不能作为语义证据。
- Root cause: 跨仓库只读调用没有把 `git rev-parse --show-toplevel` 的结果和本次 `workdir` 绑定校验，且复用了另一侧的相对路径模式。
- Prevention: 每次对照先单独调用并核对期望根目录；随后每条命令只使用当前仓库的相对路径，Legacy/Go 读取必须是两个独立调用，根目录异常时丢弃全部输出。
- Verification: 错误调用只发生在读取阶段且未产生文件变化；后续 ArcherGuard 对照改用独立、已核验的 Legacy 与 Go 调用。
- Strengthening after recurrence: 本轮读取 Go 的 monster visibility helper 时仍把 Legacy `Server/...` 路径放进 Go 命令，虽只返回路径不存在且未写入，但再次证明源码读取也必须执行单仓库参数 allowlist；不要在同一调用中附带另一侧的对照路径。
- Verification after strengthening: 丢弃混合命令的全部输出，随后用独立 Legacy/Go 调用重新核对 Mandrill 基类与 Go damage/visibility 路径；两仓库状态均保持预期。
- Strengthening after second recurrence: 本轮 SandSnail 对照时再次在 Go 根目录执行了 Legacy `Server/...` 检索；即使命令只读失败，也必须把“当前 workdir + 所有路径参数”作为同一 allowlist 审核，禁止为了连续比较而复用上一仓库的路径模式。
- Verification after second strengthening: 该调用无写入且输出已作废；随后先在 Legacy 根目录独立读取 `PoisonTarget`/`HalfmoonAttack`，再在 Go 根目录独立读取 poison/action resolver，未再使用混合输出。

### 2026-08-15 — ArcherGuard 定向 fixture 必须初始化运行态防御与 HP 投影

- Symptom: AI=113 首次定向测试命中后得到 20 点伤害而不是扣除 2 点 AC，且无效投射物用例的 `Character.HP` 仍为零。
- Root cause: fixture 只填了协议 `Stats`，没有填伤害路径实际读取的 `worldPlayer.MinAC/MaxAC`，也没有同步初始化 `SelectInfo.HP`。
- Prevention: 构造战斗实体时同时核对生产读取字段和协议持久投影；命中、未命中和失效动作都分别断言运行 HP 与 `Character.HP`。
- Verification: 补齐 `MinAC/MaxAC` 和初始 HP 后，ArcherGuard 的命中、PK 门禁、越界静止、失效重验与 NoFight 红名路径定向测试通过。

### 2026-08-15 — Guard 的攻击方向门禁必须与 AI 攻击门禁分开迁移

- Symptom: ArcherGuard 已能按 Legacy 规则攻击红名玩家，但 Go 玩家攻击公共入口仍可能把 AI=113 当作普通可攻击怪物。
- Root cause: 只迁移了 `PlayerObject.IsAttackTarget(MonsterObject)` 的 PK 门禁，遗漏了 `Guard.IsAttackTarget` 对玩家和怪物攻击者恒为 `false` 的独立覆盖。
- Prevention: 迁移同一对象的双向战斗语义时，分别检查“对象主动攻击目标”和“玩家/宠物主动攻击对象”两套入口；特殊 AI 的攻击资格不能推导出其可被攻击资格。
- Verification: `playerCanAttackMonsterLocked` 现明确拒绝 AI=113，ArcherGuard 定向测试同时覆盖主动攻击和玩家近战入口，均确认玩家攻击不造成伤害。

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

### 2026-08-15 — BlackHammerCat 线攻击必须保留原目标和 Struck 冷却

- Symptom: AI=116 Type=1 线攻击测试初版只建了一个邻近线目标，实际 `LineAttack` 又命中了两格处的原目标；同一 tick 的第二次命中还没有新的 `ObjectStruck`。
- Root cause: 把 `LineAttack(damage, 2, 300)` 当成只攻击中间目标，且按每次命中都发完整 Struck 包，遗漏了 Legacy 的距离 1/2 全线扫描与 `MonsterStruckReadyAt` 500ms 门禁。
- Prevention: 迁移线/扇形动作时逐距离展开所有有效格（包括原锁定目标），再按接收者和同 tick 的 struck 冷却状态生成公开/私有包；不能按“每个目标四包”简化多次命中。
- Verification: BlackHammerCat 测试现锁定 MC 原目标、距离 1 与距离 2 DC actions，以及两个接收者的广播/冷却包序，定向测试通过。

### 2026-08-15 — 跨仓库只读命令的输出必须整体作废

- Symptom: Legacy 只读命令末尾误带 Go 仓库路径，shell 返回路径不存在；没有写入源码，但该次混合输出不能作为对照证据。
- Root cause: 读取 Legacy 源码后在同一调用中继续拼接 Go 相对路径，违反了单次命令只服务一个仓库的边界。
- Prevention: 每条命令只使用当前已核验仓库的路径；跨仓库对照必须在新的独立调用中先核对 `git rev-parse --show-toplevel`，任一混合路径或非零读取结果出现时，丢弃整条输出并重跑。
- Verification: 本次错误调用仅在读取阶段失败且两个工作树无源码写入；后续 Legacy/Go 查询拆分执行，迁移实现只采用重新取得的独立输出。
- Strengthening after recurrence: 本轮一次 Go 只读命令又手写成重复的仓库根目录，进程启动前即被拒绝；即使只是读取，也不能凭记忆拼接跨仓库绝对路径。
- Verification after strengthening: 错误命令没有启动、没有输出可用证据或文件变化；随后重新核对 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` 并只采用正确根目录的查询结果。

### 2026-08-15 — Jar2 MC 测试必须沿用 Info 统计而非假设运行字段

- Symptom: AI=120 Jar2 测试夹具首次包级编译访问不存在的 `worldMonster.MinMC/MaxMC` 字段，行为测试尚未执行。
- Root cause: 只核对了 Legacy 的 MC 概念，没有先复用 Go 运行时实际的 `monster.Info` 统计读取边界。
- Prevention: 新 AI 引用 MC/SC/DC 前先读取 `worldMonster` 结构与 `monsterStatValue` helper；测试夹具只初始化生产路径真实读取的字段，并在新增复合夹具后立即运行包级仅编译门禁。
- Verification: 删除不存在的 MC 运行字段、保留 Info 的 MinMC/MaxMC 后，包级编译和全部 Jar2 定向测试通过。

### 2026-08-15 — 跨仓库 shell 调用中的尾部路径错误会使整段证据失效

- Symptom: Legacy 根目录的一次只读命令在完成 C# 源码读取后又追加了 Go `cmd/crystal-server` glob，shell 因路径不存在退出；前面的 Legacy 输出也不能继续作为该次调用的证据。
- Root cause: 把两个仓库的连续对照塞进同一个 shell 调用，未在参数层执行单仓库 allowlist。
- Prevention: 一个工具调用只允许一个已核验仓库的工作目录和路径字面量；跨仓库读取必须结束当前调用后再切换根目录，任何尾部非零错误都整体丢弃并重跑。
- Verification: 该调用只读失败且无文件写入；随后以独立 Legacy 与 Go 调用重新读取 Jar1/运行时路径，AI=119 判断只采用重跑结果，C# 差异保持为空。

### 2026-08-15 — CatShaman 测试随机夹具必须覆盖伤害与状态门禁的全部上界

- Symptom: AI=118 CatShaman 定向测试在红毒分支执行到 MC 伤害取值的 `Next(10)` 时失败；修复后又把命中同 tick 已执行的 Red poison 首跳误期望为 `Elapsed=0`。
- Root cause: 确定性 `monsterAIRoll` 夹具只列出了分支和部分旧调用的上界，且测试把延迟动作完成时刻与该 tick 后续 poison 调度边界混为一谈。
- Prevention: 新增 AI 测试先记录完整随机调用序列（分支、伤害、抗性、chance、毒值），夹具必须为每个实际上界提供确定值；延迟命中 transcript 同时投影命中、同 tick 移动和状态首跳，再断言 `Elapsed` 与包序。
- Verification: 加入 `Next(10)` 的固定返回并将 Red poison 的首跳期望改为 `Elapsed=1` 后，包级编译和全部 CatShaman 定向测试通过。

### 2026-08-15 — 跨仓库证据读取必须使用独立根目录与路径 allowlist

- Symptom: 一次 Go 工作目录的只读命令误带 Legacy `Server/...` glob，shell 仅返回路径不存在；没有写入，但该次混合输出不能作为迁移语义证据。
- Root cause: 跨仓库对照时复用了上一仓库的相对路径参数，没有把工作目录、根目录校验和源码路径作为同一调用的单仓库边界。
- Prevention: Legacy 与 Go 的根目录核验、源码读取和写入必须拆成独立工具调用；每次先执行 `git rev-parse --show-toplevel`，命令参数只允许当前仓库路径，出现混合路径或非零读取结果时整体丢弃输出并重跑。
- Verification: 错误调用只发生在读取阶段且两个工作树无 C# 变化；随后用独立根目录调用重新获取对照证据，AI=118 实现与矩阵判断未使用错误输出。

### 2026-08-15 — 新 AI 测试必须在仅编译门禁下清理临时变量

- Symptom: AI=121 SeedingsGeneral 定向测试首次包级仅编译因声明但未使用的 `impact` 变量失败，行为测试尚未执行。
- Root cause: 从多目标 transcript 测试复制了结果变量，但该断言只需要检查实体状态，没有同步删掉变量。
- Prevention: 新增测试后立即运行 `go test ./cmd/crystal-server -run '^$' -count=1`；若只读最终状态，直接调用 tick 而不绑定未使用结果，或对结果做明确包序断言。
- Verification: 删除未使用绑定后重新执行包级仅编译，随后运行 SeedingsGeneral 定向测试确认修复覆盖实际行为。

### 2026-08-15 — AOE 测试必须区分受伤状态与旁观者公开包

- Symptom: AI=121 SeedingsGeneral 定向测试把 Echo poison 误期望为额外移动/生命包，并把 Stomp 区域外玩家误期望为零包；实际 transcript 分别只含当前 Slow poison 包，以及区域内动作对旁观者广播的公开受击/中毒包。
- Root cause: 沿用了另一个 AI 的 poison 副作用包序，且把“区域外不扣 HP”错误简化成“区域外不接收广播”。
- Prevention: 新增多目标动作时先以实际 Go notify 路径取得完整接收者矩阵，分别断言每个实体的 HP/毒状态和每条连接的公开包；不同毒类型不能直接复用其他 AI 的包序期望。
- Verification: 将断言改为当前 SeedingsGeneral transcript，并保留区域外 HP/毒状态不变的语义断言后，重新运行定向测试验证。

### 2026-08-15 — 同格攻击 transcript 必须使用 Direction=0 哨兵

- Symptom: AI=121 同格远程边界测试把方向期望写成相邻目标的 2，实际 `DirectionFromPoint` wire 值为 0。
- Root cause: 测试只按攻击者面向目标的普通方向推导 payload，没有单独核对同格输入的旧版方向函数哨兵。
- Prevention: 协议 payload 测试覆盖同格、相邻和远距时逐 case 计算方向；同格不可从相邻方向复用期望值。
- Verification: 同格 case 改为 Direction=0 后，SeedingsGeneral 边界定向测试验证 same-cell 与 adjacent-fallback 均通过。

### 2026-08-15 — RestlessJar 测试夹具需匹配协议索引类型并清理未用 transcript

- Symptom: AI=122 RestlessJar 首次包级仅编译同时报告 `uint32` 到 `SelectInfo.Index` 的类型不匹配，以及 Spin/Tornado 未使用的 `impact` 变量。
- Root cause: 通用玩家 helper 复用了运行时 ObjectID 类型，没有按协议结构的 `int32` 字段转换；多余的 tick 结果也未在复制测试后清理。
- Prevention: 构造协议夹具时逐字段核对底层类型，新增测试立即运行包级仅编译；只验证实体状态时直接调用 tick，不保留未使用返回值。
- Verification: 修正索引为 `int32(id)` 并删除未使用绑定后，重新执行仅编译门禁确认 RestlessJar 行为测试可构建。

### 2026-08-15 — RestlessJar Stomp 推退方向必须按攻击者到目标逐点计算

- Symptom: AI=122 Stomp 测试把位于攻击者正北的邻居期望为对角线 `(2,1)`，实际 Legacy-compatible push 到 `(1,1)`，定向测试失败。
- Root cause: 只按“远离攻击者”的直觉估算坐标，未使用 `DirectionFromPoint(CurrentLocation, target.CurrentLocation)` 的正交方向。
- Prevention: 推退测试先固定攻击者/目标坐标和方向哨兵，再用一步 `movePoint` 推导目标位置与反向朝向；正交、对角和同格分别覆盖。
- Verification: 将正北邻居期望修正为 `(1,1)`/Direction=4 后，RestlessJar 定向测试继续验证。

### 2026-08-15 — RestlessJar 多目标 Stomp transcript 必须包含先前目标的公开推退包

- Symptom: Stomp 状态已正确，但主目标 packet 断言只列自身命中的五包；实际先命中的邻居向主目标广播了 `ObjectStruck`、`DamageIndicator`、`ObjectPushed`，随后才是主目标自己的命中和 `Pushed`。
- Root cause: 按目标拆分了私有包，遗漏了每个区域目标动作对同一范围内接收者的公开广播和顺序。
- Prevention: 多目标伤害/推退先按目标处理顺序建立接收者矩阵，再分别断言主目标、邻居和旁观者的公开/私有包序。
- Verification: transcript 增加邻居的三条公开包后，RestlessJar Stomp 定向测试继续验证完整顺序。

### 2026-08-16 — 在线 session transcript 必须过滤允许插入的 TimeOfDay 包

- Symptom: 全仓普通测试中 `TestCraftSessionRejectsDeadFarWrongPageAndUnknownRecipe/out_of_range` 在第 3 次走路读取到 `ServerTimeOfDay`（ID 61）而非 `ServerUserLocation`；单独运行因时钟落点不同未复现。
- Root cause: 在线 session 的全局时钟 ticker 可在任意请求响应之间插入时间通知，测试 reader 把异步允许包当成了本次 walk 响应。
- Prevention: 有在线 ticker 的 transcript reader 对明确允许的 `ServerTimeOfDay` 循环过滤，再继续等待目标响应；其他包仍立即失败，禁止用固定 sleep 或忽略所有异常包掩盖协议错误。
- Verification: 将 Craft 距离移动、NPC 移除和 ignored-request reader 改为只过滤 TimeOfDay 后，单测重复与全仓普通/race 门禁验证插入通知不再造成假失败。

### 2026-08-15 — 本批次两次路径边界复发仍须整体作废错误证据

- Symptom: 本批次一次 Go 命令误带 Legacy 路径，另一次 Legacy 命令误带 Go 路径；两次均只读失败、未产生写入，但输出不能用于迁移判断。
- Root cause: 切换仓库后复用了上一调用的路径参数，没有把绝对 `workdir`、根目录校验和相对路径 allowlist 绑定为一个不可拆分的调用契约。
- Prevention: 跨仓库读取/写入拆成独立工具调用；每次先核对 `git rev-parse --show-toplevel`，命令参数只允许当前仓库路径，出现混合路径或非零结果时整体丢弃并重跑。
- Verification: 两次错误调用均在读取/启动阶段失败且两个工作树无源码变化；随后按 Legacy、Go 分开的已核验根目录重新读取，后续实现和测试未使用错误输出。

### 2026-08-16 — 跨仓库工具编排也必须保持调用隔离

- Symptom: 本轮只读状态核对把 Legacy 与 Go 两个仓库的命令放进同一个并行工具编排；没有写入，但调用边界不再能由每个结果单独证明。
- Root cause: 只把“命令参数不混用”理解为 shell 层约束，遗漏了工具编排层的并行调用也可能让 workdir、结果和后续判断发生错配。
- Prevention: 跨仓库核验、读取和写入都按仓库分成独立工具调用；每次调用只核对一个根目录，返回后再开始另一个仓库，禁止用 `Promise.all`/并行编排混放两侧命令。
- Verification: 本次调用无文件变化；后续 AI=128/129 的 Legacy 对照、Go 实现、测试和 C# 检查均按独立调用执行，判断只采用根目录与命令参数一致的结果。

### 2026-08-15 — AI=130 CannibalTentacles 测试必须区分完整 tick 的后续 AI 副作用

- Symptom: CannibalTentacles 攻击命中测试只断言命中伤害包，但完整 `world.tick` 在 resolver 后继续执行怪物 AI，额外产生 `ObjectWalk`，导致 transcript 断言失败。
- Root cause: 混淆了隔离攻击动作解析器与完整世界 tick 的可观察阶段；命中处理完成不代表同一 tick 已结束。
- Prevention: 测试完整 tick 时按实际调度顺序把命中后的移动、毒伤和其他 AI 通知纳入接收者 transcript；若只验证 resolver，则直接调用隔离入口并明确不覆盖后续 tick 副作用。
- Verification: 补入命中后的移动包并按 map 中的权威实体断言后，AI=130 CannibalTentacles 定向测试通过。

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

### 2026-08-15 — 在线 AI transcript 的人工时钟必须领先会话维护 tick

- Symptom: TucsonGeneral 真实 `net.Pipe` transcript 在 race 下偶发没有 15 个岩石 spawn 通知，手工 `world.tick` 返回空结果；普通模式也可重复复现。
- Root cause: 停止 world ticker 不会停止 session 主循环在每轮读包前执行的 `world.tick(time.Now())`；注入即时到期的 AI 状态后，该维护 tick 可能先消费 Rage 并生成/发送岩石。
- Prevention: bootstrap 后停止后台 ticker，并发送 KeepAlive 读完响应确认会话已完成同步；注入状态时把 transcript 的 `base` 放到实时会话时钟之后，保证会话维护 tick 只能看到尚未到期的动作，再用人工时间推进完整生命周期。
- Verification: TucsonGeneral rage transcript 普通定向通过，race 连续 10 次通过；随后 GasToad/TucsonGeneral 定向普通/race、服务端整包普通/race 均通过。

### 2026-08-15 — Mantis 定向 transcript 必须包含同 tick 的旁观者状态包和目标 AC 随机值

- Symptom: AI=133 Mantis 定向测试先遗漏了 Dazed 首跳后旁观者收到的 `ServerObjectPoisoned`，并把 Monster 目标用 `Next(3)=0` 的 AC/伤害场景期望为 89 HP，实际为 90。
- Root cause: 完整 `world.tick` 在 AI 施毒后同一 tick 继续处理零时刻毒物，并按接收者广播毒包；测试又把 `monsterAIPowerLocked(10,12)` 的确定性 roll=0 误按包含上界 11 计算，而该调用得到 10。
- Prevention: AI transcript 按 tick 阶段和接收者矩阵生成包序；每个确定性随机源先列出调用上界及返回值，再逐项计算伤害、AC 和最终 HP，不能复用相邻用例的期望。
- Verification: 修正观察者包序为 `ObjectAttack -> ObjectEffect -> ServerObjectPoisoned`，Monster 目标期望为 90，并移除 300ms 命中阶段重复的 `ServerPoisoned` 期望；随后重新运行 Mantis 定向测试确认。

### 2026-08-15 — 新增 session transcript helper 必须先检查整个 Go package

- Symptom: AI=133 Mantis 真实会话测试首次包级编译因新增的 `packetIDs` helper 与现有 `awakening_test.go` 同包 helper 重名而失败。
- Root cause: 新测试只按当前文件判断辅助函数是否可用，没有检索 `cmd/crystal-server` 的整个 `main` test package；Go 的 `*_test.go` 文件共享同一 package 命名空间。
- Prevention: 新增测试 helper 前先用 `rg` 搜索整个 package 的声明与调用，优先复用现有 helper；新增后先运行 `go test ./cmd/crystal-server -run '^$' -count=1`，再运行行为 transcript。
- Verification: 删除重复 helper、复用现有 `packetIDs` 后，Mantis 会话测试进入运行阶段；随后用包级编译和定向 transcript 验证。

### 2026-08-15 — AssassinBird 测试复合字面量必须先通过语法门禁

- Symptom: 新增 AI=134 测试夹具执行 `gofmt` 时在 `Stats` 切片闭合处报告 `missing ',' before newline in composite literal`，行为测试尚未运行。
- Root cause: 多行 `worlddata.MonsterInfo` 复合字面量的最后一个切片元素后遗漏了 trailing comma，长测试 patch 没有在首次落盘后立即做语法门禁。
- Prevention: 新增或改写多行 Go 复合字面量后立即运行 `gofmt` 和受影响包的 `go test ... -run '^$'`；切片/映射闭合行逐项核对逗号与层级，再继续增加行为断言。
- Verification: 该错误在 `gofmt` 阶段被捕获且未运行服务端测试；补齐逗号后将重新执行包级编译和 AssassinBird 定向测试。

### 2026-08-15 — AssassinBird session 随机源必须避开 AI 初始化

- Symptom: AI=134 真实 `net.Pipe` transcript 在 bootstrap 阶段先消费 `Random.Next(3000)`，确定性夹具把它当作攻击分支而失败，随后 pipe 被关闭。
- Root cause: 测试在服务启动前安装了只允许攻击阶段上界的 `monsterAIRoll`；后台维护 tick/AI 初始化仍会先读取初始化搜索延迟和 CoolEye 随机值。
- Prevention: session fixture 启动前不注入攻击专用随机源；完成 bootstrap、停止 ticker 并用 KeepAlive barrier 同步后，再初始化确定性目标/时间并安装只覆盖实际攻击调用序列的随机源。初始化阶段若必须运行，使用非断言默认源。
- Verification: 失败发生在 bootstrap fixture、业务 transcript 尚未执行；调整随机源安装边界后将先运行包级编译，再重复 AssassinBird session transcript。

### 2026-08-16 — 人工 session 时钟必须固定全局光照状态

- Symptom: AssassinBird 真实 `net.Pipe` transcript 在攻击 tick 多收到一个 ID 61 (`ServerTimeOfDay`)，精确包序断言失败。
- Root cause: 测试把人工 `base` 设置为实时后一小时，跨过了 `lightSettingAt` 的小时边界；在线 world 的全局光照更新因此在战斗通知前广播了状态包。
- Prevention: 只要 session transcript 使用与实时维护 tick 不同的人工时钟，就在注入 `base` 后调用 `setLightClock` 固定同一光照值，再驱动 world tick；不能假设停止后台 ticker 会关闭在线光照副作用。
- Verification: 固定光照时钟后，AssassinBird session transcript 的通知 ID 和完整 payload 连续通过，包级编译保持通过。

### 2026-08-16 — 全量会话回归必须覆盖旧 transcript 的光照时钟稳定性

- Symptom: 服务端整包普通测试在 Mantis 和 Tucson transcript 中分别收到额外的 `ServerTimeOfDay`，虽然领域行为未变，精确包序仍失败。
- Root cause: 新增光照副作用后，旧会话夹具仍用 `time.Now().Add(time.Hour)` 作为人工推进时钟，当前小时变化可能跨越 Legacy 光照区间。
- Prevention: 每个使用人工 `base` 的在线会话夹具都必须在停止维护 ticker 后注入固定 `setLightClock`；整包回归不能只验证新功能的 session 测试。
- Verification: 将 Mantis/Tucson 受影响夹具同步固定光照后，先重跑各自 transcript，再重跑服务端整包普通、race、vet 和 build 门禁。

### 2026-08-16 — race 会话夹具的共享 AI 配置必须在 world.mu 下更新

- Symptom: `go test -race ./...` 检测到 FlyingStatue/GasToad session 测试直接改写 `monsterAIEnabled`/`monsterAIRoll`，后台连接循环同时在 `world.tick` 中读取；GasToad 在 race 变慢时还会让后台 tick 先消费一次性攻击。
- Root cause: 停止维护 ticker 不会停止每个连接的请求维护循环；测试把共享 AI 配置当成本地字段，并把人工基准时间设在墙钟当前时刻。
- Prevention: 所有在线 session 的共享 AI 注入通过持有 `world.mu` 的 helper 完成，AI 开关写入同样加锁；人工 transcript 时钟至少领先墙钟一小时，避免连接循环抢先处理未来动作。
- Verification: FlyingStatue/GasToad session 在 `go test -race` 下连续 10 次通过；随后将重跑全量 race 门禁。

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

### 2026-08-17 — RhinoPriest session 与多目标 transcript 夹具必须隔离派生状态和独立 AI

- Symptom: AI=137 RhinoPriest session transcript 在减益命中后多收到一个 `ServerHealthChanged`；新增夹具还曾引用不存在的 `monsterStatMinMAC/MaxMAC`、遗漏 `worlddata` import，并把相邻 `(1,0)` 目标误判为远程；宠物/Hero 回归初版又发生 nil 宠物解引用，Hero 额外产生自身攻击包。
- Root cause: `StatsInitialized=true` 的真实会话在 DC/MC/SC 减益后会按角色等级刷新派生 MaxHP，人工 100 HP 超过新手上限；Monster stat 常量和 import 未先从当前 Go package 核对；RhinoPriest 的同格/相邻分支边界与 Legacy 显式距离判断未逐项列出；Hero runtime 与 Monster AI 会在同一 `world.tick` 独立推进，而双目标表中另一分支的指针为 nil。
- Prevention: session fixture 保留真实 stat refresh，但把合成角色等级提高到足以容纳人工 HP，并固定光照/时间；新增测试先用当前 package 的符号检索和包级编译门禁；按 Chebyshev 距离逐项标注近战、同格和远程；多目标测试先按 kind 选择非 nil ID，并把 Hero `ActionReadyAt` 置于人工时钟之后，避免无关 AI transcript 污染。
- Verification: RhinoPriest 世界攻击、毒物、减益、宠物 Monster、Hero 和真实 `net.Pipe` transcript 均通过；失败夹具修正后重新运行定向测试，完整门禁将在本批次末执行。

### 2026-08-17 — 跨仓库初始查询仍必须按工具调用隔离

- Symptom: 本批恢复时又把 Legacy 与 Go 的初始状态查询放入同一个 `Promise.all`；查询没有写入文件，但不能由每个结果独立证明其仓库边界。
- Root cause: 将“都是只读查询”误当成可以跨仓库并行，忽略了工具编排层同样可能混淆 workdir、输出和后续判断。
- Prevention: 每个 `functions.exec` cell 只绑定一个先经 `git rev-parse --show-toplevel` 核验的仓库根目录；Legacy 与 Go 的状态、源码、测试和写入按独立调用串行切换，禁止用 `Promise.all` 混放两侧。
- Verification: 混合查询未产生文件变化；后续 lessons、Go 状态、测试和提交前门禁均按独立仓库调用完成。

### 2026-08-17 — StoneGolem 的三格中心、值 map 和目标投影必须分别锁定

- Symptom: AI=139 若只按攻击距离推导 Quake 中心、复用 `world.monsters` 的旧副本，或用 nil Hero 夹具，可能出现中心错位、Monster HP 未持久化或测试未真正覆盖 Hero 投影；攻击冷却期还可能错误追加移动包。
- Root cause: Legacy 使用 `PointMove(CurrentLocation, Direction, 3)` 生成中心；Go Monster 表是 value map；Hero 运行实体与 owner-keyed 表、非空 `StoredHero` 是分开的契约；`ProcessTarget` 在 `!CanAttack` 时提前返回，不能把冷却期当作移动回退。
- Prevention: 逐步执行三次 `PointMove`；每次修改 Monster 后写回并从权威 map 回读；Hero fixture 同时设置 owner map key、runtime ObjectID 和非 nil Hero；测试把冷却期“无移动”作为独立可观察边界，并按 Player/Monster/Hero 分开生成命中矩阵。
- Verification: AI=139 世界测试覆盖 25 个有效 Quake、value-map HP 回读、Hero 非命中/单目标攻击、延迟重验和冷却期；真实 `net.Pipe` transcript 锁定中心 ObjectSpell、HP 与 25 个有序移除包。

### 2026-08-17 — race 下人工 session 时间轴必须冻结光照时钟

- Symptom: SwiftFeet 到期的合成 `world.tick` 在 race 下收到 `ServerTimeOfDay`（ID 61）而不是预期的 `ServerRemoveBuff`，单个领域状态本身没有错误。
- Root cause: 停止后台 ticker 只等待 ticker goroutine 退出，仍保留 `lightsEnabled`；人工到期时间与墙钟光照区间不同，在线世界会把全局光照变化插入精确 transcript。
- Prevention: 停止维护 ticker 后，在每个使用人工时间轴的在线 fixture 中用 `setLightClock` 固定一个稳定时刻，再驱动 synthetic tick；不能把 ticker 停止当成光照副作用关闭。
- Verification: SwiftFeet 定向普通/race、Go 全量 `go test -race ./...`、`go vet ./...` 与 `go build ./...` 均通过，且没有额外 TimeOfDay 包。

### 2026-08-17 — EarthGolem 对照命令的仓库路径必须机械隔离

- Symptom: AI=140 对照期间两次只读命令把另一仓库路径追加到当前命令（先在 Legacy 命令尾部带 Go 路径，后在 Go 命令中带 Legacy `Server/...` 路径）；读取阶段失败，不能使用同一调用的任何输出作为语义证据。
- Root cause: 为并列读取 C# 基线、Go 实现和迁移矩阵而复用了上一侧的相对路径，没有把 `workdir` 与全部路径参数作为一个不可拆分的单仓库调用契约。
- Prevention: 每次读取或写入前先在独立调用中核对 `git rev-parse --show-toplevel`，再逐项检查命令参数只属于当前仓库；Legacy 与 Go 必须使用不同的 `functions.exec` cell，任一混合路径或非零读取结果都整体作废并重跑。
- Verification: 两次错误调用均只读、没有 C# 或 Go 源码写入；EarthGolem 的 C#、Go 和矩阵证据随后分别在已核验根目录独立读取，后续测试与补丁只作用于 Go 仓库。

### 2026-08-17 — EarthGolem fixture 必须先核对运行时字段并隔离 Hero tick

- Symptom: AI=140 首次包级编译使用了不存在的 Monster MAC stat 名称和 `worldMonster/worldHero` 未定义的冷却字段；修正后 Hero 失效目标测试又收到运行时自动传回 owner 位置的对象包。
- Root cause: 测试夹具按 Legacy/其他实体的字段直觉拼接，没有先读取当前 Go struct 与 stat 常量；把 Hero 改图作为唯一失效条件，却遗漏 `tickHeroesLocked` 会在 owner 与 Hero 不同地图时自动 teleport。
- Prevention: 新增夹具先检索当前 package 的精确常量、struct 字段和 map key 语义并执行包级编译；失效目标 transcript 要隔离无关实体 tick（同步 owner 状态或冻结/移除独立运行实体），再断言目标失效无攻击包。
- Verification: EarthGolem Player/Monster/Hero 世界测试及真实 session transcript 均通过，包级 `go test -run '^$'` 先于行为测试通过。

### 2026-08-17 — 特殊石化门禁必须覆盖每个玩家攻击入口

- Symptom: EarthGolem 石化后普通玩家攻击已被拒绝，但区域攻击入口仍返回可攻击，能力门禁回归失败。
- Root cause: 只在单目标 `playerCanAttackMonsterLocked` 迁移了石化条件，遗漏独立的 `playerCanAreaAttackMonsterLocked` 分支。
- Prevention: 迁移不可攻击状态时建立入口矩阵，至少覆盖单体、区域、魔法、宠物/Hero、毒/Buff、推退和移动/怪物 AI；每个入口都用同一状态 fixture 断言拒绝且不产生副作用。
- Verification: EarthGolem 石化世界测试现覆盖单体/区域/移动/攻击/毒/Buff/推退门禁，并在服务端整包测试中通过。

### 2026-08-17 — 生命周期代码修改前必须复读精确物理行

- Symptom: 修改 EarthGolem Pile 生命周期时曾凭摘要怀疑生成分支缺少 `continue`，准备的补丁与当前源码不符；精确复读后确认分支已有 `continue`，补丁未应用。
- Root cause: 依赖前一轮分析记忆，没有在生命周期分支修改前用带行号/不可歧义的读取重新核对实际控制流。
- Prevention: 修改 spawn/impact/expiry 状态机前先复读完整分支的精确物理行（包括条件、状态写入和 `continue`/`return`），再用最小 hunk 修改；补丁失败或上下文不符时不据工具输出推断源码状态。
- Verification: EarthGolem Pile 的 spawn、首次/重复命中、过期移除和 ordered transcript 均通过，生产状态机未引入重复处理。

### 2026-08-17 — Go 仓库绝对路径重复时必须立即作废调用

- Symptom: AI=141 开始前一次只读命令把 Go 根目录手写成重复的 `.../me_work/me_work/Crystal.GoServer`，进程未启动，不能使用其输出作为源码或状态证据。
- Root cause: 从 Legacy 切换到同级 Go 仓库时凭记忆拼接绝对路径，没有先复用独立 `git rev-parse --show-toplevel` 返回的完整根目录。
- Prevention: 每次 Go 工具调用先在独立调用中核对 `git rev-parse --show-toplevel` 和目标存在性，后续只使用该调用返回的根目录；启动失败的调用整体作废，不据错误文本推断文件状态。
- Verification: 本次命令在进程创建前失败且没有文件变化；后续 AI=141 的 Go 读取、补丁、测试和提交将只使用核验后的 `Crystal.GoServer` 根目录。

### 2026-08-17 — TreeGuardian 测试必须锁定防御公式、Hero 基础敏捷和 value-map 回写

- Symptom: AI=141 首次定向断言把 AC/MAC 伤害按直觉算错；Hero 夹具遗漏等级基础敏捷的 `Random.Next(16)`；真实 session 第二次攻击因未回写 `world.monsters` 的 value 副本而没有产生攻击包。
- Root cause: 测试只看攻击力和手工 AC，没有沿实际 Player/Monster/Hero 命中路径计算防御；没有从 Hero class/level 的基础属性推导随机边界；修改 detached `worldMonster` 后把 map 当成引用容器使用。
- Prevention: 每个 AI 分支先分别固定攻击随机源、战斗随机源和目标层防御/敏捷公式；Hero transcript 覆盖其实际基础 stat roll；修改 `world.monsters` 中的实体后立即显式写回，并在下一 tick 从权威 map 回读状态。
- Verification: TreeGuardian 四分支/Fullmoon 世界测试、真实 `net.Pipe` 攻击与失效目标 transcript 均通过；普通/race 全仓测试、vet、build 和 diff 检查均通过。

### 2026-08-17 — Go 仓库根目录重复时必须作废启动失败调用

- Symptom: AI=141 开始前一次命令把 Go 根目录写成 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer.GoServer`，进程在启动前失败，不能提供源码或状态证据。
- Root cause: 从 Legacy 切换到同级 Go 仓库时手工重复拼接了仓库名，没有直接复用独立 `git rev-parse --show-toplevel` 的结果。
- Prevention: 每次 Go 工具调用先独立核验根目录和目标存在性，后续只使用该调用返回的完整路径；启动失败的调用整体作废，不能根据错误文本推断状态。
- Verification: 错误调用未启动且没有文件变化；随后以核验后的 `Crystal.GoServer` 根目录完成 AI=141 的读取、测试和提交准备。

### 2026-08-17 — TreeQueen transcript 必须按真实 recipient 与对象生命周期验收

- Symptom: AI=142 初始定向测试使用了错误的 recipient ID，并假定对象在 spawn tick 就完成后续命中/移除，导致 transcript 与实际通知和生命周期不符。
- Root cause: 测试按 fixture 中的猜测身份和单次 tick 推导网络行为，没有先确认真实接收者，也没有把召唤、延迟命中和过期移除拆成实际世界 tick。
- Prevention: 多对象 transcript 先从在线连接和通知接收者矩阵确定 recipient，再按每个对象的 spawn/impact/expiry 时刻逐 tick 断言；value-map 实体状态从权威 world map 回读。
- Verification: TreeQueen 世界测试和真实 `net.Pipe` transcript 已按实际 recipient、目标投影和有序生命周期修正并通过，普通/race/vet/build 门禁保持绿色。

### 2026-08-17 — TucsonMage bootstrap 完成后再注入攻击随机源

- Symptom: TucsonMage session fixture 的攻击随机源可能在 bootstrap 阶段先被 `Next(3000)` 消费，导致后续攻击 transcript 的随机序列漂移。
- Root cause: bootstrap、session 维护 tick 和攻击动作共享同一个确定性随机源，测试在连接尚未稳定前就安装了只覆盖攻击阶段的 roll 表。
- Prevention: bootstrap 阶段保留宽松随机源；完成 bootstrap 并用 KeepAlive barrier 同步后，停止维护 ticker，再安装攻击专用随机源和人工时钟，避免初始化消费混入攻击序列。
- Verification: TucsonMage 定向测试连续 5 次通过，随后服务端整包、全仓普通/race、vet 和 build 门禁均通过。

### 2026-08-17 — Legacy 只读命令末尾不得夹带 Go 路径

- Symptom: PeacockSpider 对照读取时，Legacy 命令末尾误带了 Go 的 `cmd/crystal-server/monster_ai.go` 路径；命令以路径不存在结束，不能使用同一调用的任何输出作为证据。
- Root cause: 切换仓库后复用了上一侧的相对路径，未在命令参数层执行单仓库 allowlist 检查。
- Prevention: 每个源码证据调用只使用当前已核验根目录下的路径；跨仓库对照必须结束当前调用，再在新的、独立核验的 Go/Legacy 工具调用中读取，任一非零读取结果整体作废。
- Verification: 该调用只读且没有文件写入；后续将分别在 Legacy 与 Go 根目录重跑 PeacockSpider 对照，提交前继续执行 C# 零变化门禁。

### 2026-08-17 — PeacockSpider cooldown must stop target movement

- Symptom: The first PeacockSpider ranged `net.Pipe` transcript received an extra `ObjectWalk` during the delayed projectile impact, and the deterministic roll fixture unexpectedly reached the movement fallback bound.
- Root cause: The migrated specialized `ProcessTarget` path treated `!CanAttack` as permission to chase; Legacy returns immediately during ActionTime/AttackTime recovery before evaluating attack range or movement.
- Prevention: For every delayed monster AI, model the `Target == null || !CanAttack` early return before the attack-range/movement branch; add a transcript assertion that the cooldown tick emits no movement packet and does not consume movement rolls.
- Verification: PeacockSpider's cooldown/session test now passes with only the ranged packet at generation and the ordered damage/paralysis packets at impact; the full `cmd/crystal-server` suite passes.

### 2026-08-17 — PeacockSpider fixtures must use the package's stat identifiers

- Symptom: The first PeacockSpider test fixture failed to compile because it used nonexistent `monsterStatMinMAC`/`monsterStatMinMC`/`monsterStatMinSC` identifiers.
- Root cause: The fixture copied the DC-specific `monsterStat*` naming pattern instead of checking the current Go package's shared `statMinMAC`/`statMinMC`/`statMinSC` constants.
- Prevention: Before adding a migrated monster fixture, search the current Go package for each stat constant and run `gofmt` plus `go test ./cmd/crystal-server -run '^$' -count=1` immediately after the first patch.
- Verification: The fixture now uses the authoritative shared stat constants; package compilation, PeacockSpider tests, and the full server test suite pass.

### 2026-08-17 — 跨仓库只读命令必须先核验每个路径

- Symptom: AI=144 研究期间一次 Legacy 命令末尾误带 Go 源码路径，另一次 Go 命令引用了不存在的测试文件；两条命令均在读取阶段失败，不能使用同一调用的任何部分输出作为证据。
- Root cause: 为连续读取对照代码而复用了上一仓库的路径参数，并凭文件名猜测 Go 测试文件存在，没有在执行前按当前仓库的 `rg --files` 清单核验全部路径。
- Prevention: 每个 `functions.exec` cell 只绑定一个已核验仓库根目录；命令中的每个文件、目录和 glob 都必须来自该仓库的实际清单，出现混合路径、猜测路径或非零读取结果时整条输出作废并在新的独立调用重跑。
- Verification: 两次失败调用均无文件写入；后续 AI=144 对照与实现读取将按 Legacy/Go 独立 cell 和已存在路径执行。

### 2026-08-17 — AI session transcript 的共享 world 读取必须持锁

- Symptom: AI=144 真实 `net.Pipe` transcript 在 `go test -race` 下读取 `world.monsterAttackActions[0]` 时报告与连接维护 tick 的数据竞态；普通测试未暴露问题。
- Root cause: 停止后台 ticker 不会停止连接 session loop 的维护 tick；夹具只在写入共享 AI 状态时持锁，却无锁读取仍由 session loop 访问的 world slice。
- Prevention: 在线 transcript 对共享 world 的每次读取都经过 `world.mu` 快照，写入也保持同一锁；KeepAlive 只提供会话阶段屏障，不等价于停止连接级 tick。
- Verification: action due/count 断言改为锁内快照后，AI=144 session race 定向测试连续 5 次通过，普通 transcript 仍锁定完整包序。

### 2026-08-17 — OmaCannibal 近战与远程毒物分支必须按 DelayedAction 形状区分

- Symptom: AI=144 初版 resolver 在近战 DC 命中后也加入 Green poison；Legacy 只有带 `poison=true` 的 `CompleteRangeAttack` 远程路径施毒。新增死亡重验夹具还把预先设为 0 的 HP 误断言为初始 100，造成一次定向测试失败。
- Root cause: 只按“有效命中后施毒”的概括迁移，没有沿两个不同 `DelayedAction`/完成函数核对 poison 标志；测试断言把“目标已死亡”与“命中后死亡”混为一类。
- Prevention: 每个 AI 先建立动作构造参数到完成 resolver 的逐分支表，只有 Legacy 明确传入 poison 标志的路径才添加状态；重验 fixture 同时记录 mutation 后的预期 HP 与是否发生伤害，不能固定复用初始生命值断言。
- Verification: AI=144 近战世界测试锁定 AC/Agility 伤害且无 Green poison，远程世界与 `net.Pipe` transcript 锁定 Green poison 首跳和 `Elapsed=1`；地图/安全区/死亡重验测试修正后全部通过。

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

### 2026-08-17 — Halfmoon 多目标 transcript 必须区分自身包与范围广播

- Symptom: AI=146 Halfmoon 定向测试把某个玩家 recipient 收到的完整 impact 包序固定为自身四包，但四个相邻目标都在 16 格通知范围内，实际还包含其他目标的 `ObjectStruck`/伤害/生命广播。
- Root cause: 把单目标会话 transcript 的通知假设复用到多目标世界夹具，没有先按通知范围和每个命中目标拆分观察者可见包。
- Prevention: 多目标攻击断言先锁定动作目标顺序和各目标权威 HP，再只检查 recipient 必须存在的自身包，或为隔离观察者/按 object ID 过滤范围广播；不要把完整 recipient 列表当成单目标序列。
- Verification: 修正后 AI=146 四格 Halfmoon 世界测试验证了 PreviousDir 顺序、四个目标 HP 和隐藏目标命中；单目标 `net.Pipe` transcript 继续验证完整四包顺序。

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

### 2026-08-17 — AI=147 固定随机边界、目标投影和 session 时钟必须分别锁定

- Symptom: OmaMage 首次定向测试把固定 DC/MC 区间期望成大于 1 的随机上界；Player 目标的延迟动作使用协议兼容的 `TargetKind=0` 后未还原，导致命中静默丢弃；真实 session 重复运行还可能在手工 tick 外消费一次 AI fallback 抽样。
- Root cause: Legacy `GetAttackPower(min,max)` 对相等区间仍调用 `Random.Next(1)`；内部目标 kind 与线上的 Player 默认值没有在 resolver 边界归一化；停止 world ticker 不代表测试期间所有 session 调度都已冻结，且回调中直接 `Fatal` 会把后台抽样误报为业务失败。
- Prevention: 迁移固定区间时保留 unit-bound 随机调用；所有延迟动作 resolver 先把 `TargetKind=0` 还原为 Player，并用 delayed-hit 测试覆盖；真实 session 将 AI/search/action 时间置于手工时钟之后，随机回调只做同步记录并断言业务阶段的稳定序列，不在异步回调中终止测试。
- Verification: OmaMage 世界测试覆盖固定攻击边界、CanFly/冷却移动、延迟重验、Player/Monster/Hero 与两次独立毒物抽样；`net.Pipe` transcript 普通和重复 10 次、全量 `go test ./...`、`go test -race ./...`、`go vet ./...`、`go build ./...` 均通过。

### 2026-08-17 — AI=148 夹具必须遵循 Go 坐标类型与真实距离分支

- Symptom: OmaWitchDoctor 初次定向测试在 `gofmt` 前由 `int` 坐标传入 `int32` 字段而编译失败；修正后又把距离 1 的对角目标误标成远程，并在非直线延迟 tick 误断言“无通知”，遗漏了专用 `ProcessTarget` 的冷却追击 `ObjectWalk`。
- Root cause: 测试夹具按字面量直觉推导坐标和“远程=非相邻方向”，没有先核对 Go struct 的精确类型，也没有按 Legacy `CurrentLocation == Target` 或 `!InRange(...,1)` 的 Chebyshev 距离条件建分支表。
- Prevention: 新增坐标夹具先显式声明 `int32`；攻击矩阵用距离 0、1、>1 分别覆盖同格、近战和远程，再单独标注直线/非直线；延迟 tick 必须继续执行一次完整 AI 流程，把冷却期间的移动/无移动作为包级断言。
- Verification: AI=148 测试改用 `int32` 坐标、距离 2/3 的直线远程和距离 2/1 的非直线场景；世界测试、`net.Pipe` transcript 及普通/race 定向门禁均通过。

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

### 2026-08-17 — PowerBead transcript 的随机消费发生在延迟命中阶段

- Symptom: AI=149 `net.Pipe` transcript 在发出 `ObjectRangeAttack` 后立即断言固定 DC 的 `Random.Next(1)`，观察到没有 AI 随机调用。
- Root cause: Legacy `GetAttackPower` 位于 `CompleteRangeAttack`，而不是发射范围攻击的 `ProcessTarget`；Go action 的攻击力同样只在 300ms 到期 resolver 中计算。
- Prevention: 将随机边界断言按发射阶段和 impact 阶段拆开；发射只验证范围攻击包和 action due，命中后再验证固定区间的 `[1]` 消费。
- Verification: PowerBead world test 与真实 `net.Pipe` transcript 均验证发射阶段无该消费、300ms impact 阶段恰有一次 `bound=1`。

### 2026-08-17 — Monster/Hero 的 PowerBead 包必须按客户端观察者投影断言

- Symptom: AI=149 初始测试把 Monster/Hero 的 `ObjectRangeAttack` recipient 写成目标 ObjectID，导致包序为空或混入 Hero 自身 AI 包。
- Root cause: 服务端只向附近在线 Player 写广播；Monster 不接收 socket，Hero 的 owner Player 还可能在同一 tick 自己运行 Hero AI。
- Prevention: Monster 目标使用独立附近 Player observer，Hero 目标使用 owner/隔离 observer，并先冻结 Hero `ActionReadyAt`；所有断言按 recipient 的可观察包序过滤。
- Verification: Effect 0 Player/owned Monster/Hero 世界测试与 PowerBead session transcript 均通过，且只保留目标类型应有的广播。

### 2026-08-17 — PowerBead 的 PetOwner 夹具不能走普通 Monster AI population

- Symptom: AI=149 Effect 1 与 PetMode 测试通过 `world.tick` 驱动带 `PetOwnerID` 的 bead，却没有产生 action。
- Root cause: Go 的普通 `tickMonsterAILocked` 明确把 `PetOwnerID` monster 交给 ordinary-pet pipeline；Legacy AI=149 的常见 SpawnRandom bead 是无 Master 的 summoned monster。直接测试 master 分支时必须调用 PowerBead processor，或使用真实无 owner bead。
- Prevention: session/生产路径使用无 `PetOwnerID` 的 AI=149 bead；仅验证 owner/PetMode 边界时在持有 world 状态的夹具中直接调用 `processPowerBeadLocked` 并写回 authoritative map。
- Verification: Effect 1 清理、PetMode matrix 和真实无 owner `net.Pipe` transcript 均按对应调度路径通过。

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

### 2026-08-17 — AI=150 测试夹具必须使用 worldHero 的实际冷却字段

- Symptom: DarkOmaKing MassThunder 定向测试首次编译失败，夹具给 `worldHero` 设置了不存在的 `AttackReadyAt` 字段。
- Root cause: 参照玩家/其他实体的攻击冷却命名构造 Hero fixture，没有先核对 `worldHero` 的实际结构；该类型只暴露 `ActionReadyAt`。
- Prevention: 新增实体夹具赋值前先用当前 Go 类型定义和现有测试检索确认字段名；先执行 `gofmt` 与包级编译，再进入行为断言，避免把编译错误混入功能失败。
- Verification: 删除无效字段并保留 `ActionReadyAt` 冻结 Hero AI 后，`go test ./cmd/crystal-server -run 'DarkOmaKing' -count=1` 通过，随后真实 `net.Pipe` transcript 也通过。

### 2026-08-17 — 跨仓库读取失败的输出不得继续用于 AI=151 判断

- Symptom: AI=151 方向核对的一条 Go 命令混入了 Legacy `Shared/Enums.cs` 路径并以路径不存在失败；同一调用的其他输出也不能作为证据。
- Root cause: 为确认旧版方向枚举，把 Legacy 相对路径追加到 Go 工作目录，违反了单仓库命令边界。
- Prevention: Go 调用参数只允许 Go 仓库路径；Legacy 对照必须结束当前调用后重新核对 Legacy 根目录，任何混合路径或非零只读调用整体作废。
- Verification: 失败命令只读且没有文件变化；随后仅使用独立 Go 的 `directionFromPoints` 输出和独立 Legacy `CaveStatue.cs` 基线完成 AI=151 实现。

### 2026-08-17 — AI=151 value-map 实体必须回读后写回

- Symptom: CaveStatue session 夹具首次包级编译失败，直接给 `world.monsters[1].Route` 和 `RouteMoveReadyAt` 赋值；Go map value 不能对索引表达式的字段赋值。
- Root cause: 把 `world.monsters` 当成指针 map 使用，未先复制实体到局部变量。
- Prevention: 修改 Monster value-map 实体时先取局部副本，完成所有字段变更后通过同一 ObjectID 写回；延迟状态断言也从权威 map 回读。
- Verification: 改为回读/修改/写回后，AI=151 CaveStatue world 与真实 `net.Pipe` 定向测试通过。

### 2026-08-17 — 跨仓库并行编排仍不得混合证据

- Symptom: AI=153 恢复时再次把 Legacy 与 Go 的状态/文档检索放进同一个并行编排；调用只读且没有源码变化，但违反了项目规定的单仓库证据边界。
- Root cause: 把“命令互不写入”误当成“可以共享一次工具调用”，没有让每个编排 cell 绑定唯一仓库根目录。
- Prevention: Legacy lessons、Legacy 状态/源码与 Go 状态/源码必须分别调用；每次切换前独立核对 `git rev-parse --show-toplevel`，混合调用输出整体作废。
- Verification: 本次混合调用未产生文件变化；之后 CreeperPlant 对照、Go 实现、测试和矩阵更新均在独立仓库调用中完成。

### 2026-08-17 — Go 检索不得引用未存在的概念文件名

- Symptom: AI=153 对 Go 运行时辅助函数的检索误带不存在的 `cmd/crystal-server/session.go`，命令返回路径错误；没有写入，整条检索输出不能作为源码证据。
- Root cause: 按概念猜测文件名并追加到 shell 参数，没有先用 `rg --files` 确认实际文件清单。
- Prevention: 读取前先列出当前 Go 目录的精确文件；后续只使用已存在的路径或让 `rg --glob` 处理模式，任一非零读取命令整体作废并重跑。
- Verification: 重跑时只读取已核验的 `world.go`、`main.go`、`armadillo.go` 与测试文件，CreeperPlant 行为判断只采用成功调用输出。

### 2026-08-17 — AssassinBird session 必须在服务启动前冻结 AI 初始化

- Symptom: Go 包级全量回归中 `TestSessionAssassinBirdPushTranscript` 偶发收到注入回调不允许的 `Random.Next(3000)`，随后 transcript 以 EOF 失败；单独运行可通过。
- Root cause: 服务启动前未把 AssassinBird 的 `MonsterAIInitialized` 和 search/action/move/attack 时间置于未来，连接维护 tick 可能在测试安装随机回调后执行首次 AI 初始化。
- Prevention: 真实 `net.Pipe` 夹具在启动服务前先设置 `MonsterAIInitialized=true` 并把所有 AI 时间置于未来；手工时钟和业务状态只在 bootstrap 后注入，停止 ticker 不视为停止连接维护 tick。
- Verification: 修复后 AssassinBird transcript 连续 10 次通过，CreeperPlant transcript 普通/race 回归通过，随后 `go test ./cmd/crystal-server -count=1` 全包通过。

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

### 2026-08-17 — SwampWarrior session 人工时钟必须隔离连接维护 tick

- Symptom: 全仓 `go test -race ./...` 偶发在 `TestSessionSwampWarriorRangeAttackTranscript` 看到空的手工攻击通知；普通运行或单独测试可能通过。
- Root cause: 夹具在停止 world ticker 后仍使用墙钟当前时刻，且没有 post-bootstrap KeepAlive 屏障；连接 session loop 的维护 `world.tick(time.Now())` 可以先消费已到期的 SwampWarrior 动作，测试随后再手工 tick 时动作已被取走。
- Prevention: 真实 AI transcript 在服务启动前将实体初始化和 search/action/move/attack 时间置于未来；bootstrap 后先停止 ticker 并消费 KeepAlive，再把人工 `base` 放到墙钟之后、固定 `setLightClock`，最后注入手工动作状态。
- Verification: 修正夹具后 `TestSessionSwampWarriorRangeAttackTranscript` 的 race 定向测试连续 10 次通过；随后全仓普通测试、全仓 race、`go vet ./...` 和 `go build ./...` 均通过。

### 2026-08-17 — Legacy 只读命令不得追加 Go 路径

- Symptom: 上一轮一次 Legacy 只读命令在完成源码读取后误追加了 Go 的 `cmd/crystal-server/ancient_bringer.go`，命令在读取阶段失败；没有文件写入，整条输出不能作为证据。
- Root cause: 为连续对照而复用了另一仓库的相对路径，没有在调用参数层执行单仓库路径 allowlist。
- Prevention: Legacy 调用只使用已核验的 `Server/`、`Shared/`、`Client/`、`tasks/` 路径；Go 对照必须在新的、先核验根目录的独立调用中读取，任一非零混合调用整体作废。
- Verification: 该调用未产生文件变化；本批已先完整读取 lessons，后续 Go/Legacy 读取、测试和写入将按独立仓库调用执行。

### 2026-08-17 — Go patch 锚点必须先由当前源码核验

- Symptom: AI=155 初次 Go patch 使用了不存在的 `monsterAIAxeSkeletonProcessTargetLocked` 函数锚点，patch 失败且没有写入源码。
- Root cause: 按 Legacy 类型名推测 Go 的 dispatch 函数名，没有先读取当前 Go 文件确认精确锚点。
- Prevention: 修改前先在目标仓库独立核对 `git rev-parse --show-toplevel`，再用 `rg -n`/精确源码读取确认函数签名；每个 patch 只使用已核验的上下文，失败后先重新读取再重试。
- Verification: 失败 patch 未改变 Go 工作树；随后使用实际的 `processMonsterAICoreLocked`/`monsterAIAvengingSpiritAttackLocked` 锚点完成接入，包级编译将在行为测试前验证。

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

### 2026-08-17 — AI=155 transcript 必须初始化继承的 FearTime，并计入同 tick 绿毒首跳

- Symptom: AvengingSpirit 夹具未设置未来的 FearTime，首个手工 tick 正确执行了继承的 fear/walk 分支而没有攻击；远程攻击期望只计算直接伤害，却漏掉同一 tick 立即处理的 Green poison 首跳。
- Root cause: 测试 fixture 只配置了 AvengingSpirit 自身的 action 状态，没有冻结 inherited AxeSkeleton fear 状态；断言把“命中时挂入毒物”和“零 TickAt 毒物在当前 tick 结算”混成了单一伤害。
- Prevention: 所有特殊 AI transcript 在启动手工时钟前显式把继承 AI 的 FearTime/搜索/动作时间置于未来；命中断言分别核对直接伤害、毒物列表和到期结算，零 TickAt Green poison 必须纳入同 tick HP/状态包期望。
- Verification: 修正 fixture 和伤害期望后，AvengingSpirit world/session 定向测试通过，包级空测试编译也通过。

### 2026-08-17 — race 门禁前检查可重建缓存空间

- Symptom: AI=155 首次全仓 `go test -race ./...` 在链接主服务测试二进制前因 `no space left on device` 失败，未执行到测试断言。
- Root cause: 主机数据卷仅剩约 186 MiB，累积的 Go build/test cache 占用了可回收空间。
- Prevention: 将编译/测试链接失败先分类为环境资源错误；在代码诊断前读取 `df -h`，仅清理可由 Go 重新生成的 build/test cache，再重跑同一 race 命令。
- Verification: 清理后可用空间恢复到约 6.1 GiB，`go test -race ./...` 完整通过；普通测试、vet 和 build 也均通过。

### 2026-08-17 — AI=156 fixture 必须保留排他随机上界并避免状态重声明

- Symptom: AvengingWarrior 远程 fixture 将 MC `Next(2)=0` 的结果误期望为最大值 21（实际为 20）；补充移动断言时又在同一作用域用 `:=` 重声明 `state`，包测试编译失败。
- Root cause: 没有按 Legacy `Random.Next(min,max)` 的排他上界配置固定回调，且没有区分首次声明与后续赋值。
- Prevention: 每个固定随机回调先列出 `bound → returned value → expected stat`，覆盖最小/最大边界；Go fixture 中已存在的实体副本一律使用 `=` 回写，修改后立即跑包级编译。
- Verification: 修正 MC 回调为 `Next(2)=1`、改用 `state = ...` 后，AI=156 定向 world/session 测试和包级编译均通过。

### 2026-08-17 — AI=156 Red poison session 断言以真实包序为准

- Symptom: AvengingWarrior 真实 `net.Pipe` transcript 首次把 Red poison 命中期望为包含 `ObjectWalk`，实际通知序列为 `Struck/ObjectStruck/DamageIndicator/HealthChanged/Chat/Poisoned`，断言失败并关闭连接。
- Root cause: 复用了 Green poison/其他 AI 的状态包假设，没有先按该 Red poison 处理路径核对客户端可观察包序。
- Prevention: 新增 AI 的 session 夹具先记录完整 recipient-specific packet IDs，再只断言 Legacy 对应路径实际产生的包；不同 poison 类型不得复用额外状态包期望。
- Verification: 移除错误的 `ObjectWalk` 期望后，AvengingWarrior session transcript 通过，race 重复 10 次通过。

### 2026-08-17 — Legacy 对照命令不得夹带 Go 路径（AI=157）

- Symptom: 对照 AxePlant 时在 Legacy 根目录的 shell 命令误带 Go 的 `cmd/crystal-server/*.go` 路径，zsh 因未匹配 glob 失败；该调用没有写入，整条输出不能作为源码证据。
- Root cause: 为连续读取两侧实现，复用了另一仓库的相对路径，没有把每次调用的工作目录和参数限制为同一仓库。
- Prevention: 每次 Legacy/Go 对照拆成独立调用；Legacy 命令只允许 `Server/`、`Shared/`、`Client/`、`tasks/` 等已核验路径，Go 命令只允许 Go 仓库路径。失败的读取调用整体作废，不采用其余输出。
- Verification: 失败调用发生在 zsh glob 展开阶段且未产生文件变化；之后在独立核验的两个仓库中分别重新读取 AxePlant 基线和 Go 实现，AI=157 定向测试通过。

### 2026-08-17 — apply_patch 编排的结束标记必须保持 JavaScript 字符串完整

- Symptom: WoodBox 补丁的一次 `functions.exec` 编排因 JavaScript 结束标记引号不匹配而在执行前失败；没有产生文件写入。
- Root cause: 手写多行 patch 字符串时把结束标记放进了未闭合的字符串，未先检查调用脚本的语法边界。
- Prevention: 使用 `const patch = "..."` 时逐项核对开头/结尾引号，结束标记必须位于字符串外；脚本失败时整条调用作废，不把错误文本当作源码证据。
- Verification: 失败调用未启动 patch 且工作树无变化；随后使用语法完整的独立 `apply_patch` 调用完成 WoodBox 测试修改。

### 2026-08-17 — 新增 AI 常量前必须搜索全包声明

- Symptom: WoodBox 初次接入时 `woodBoxMonsterAI` 重复声明，Go 包编译失败；修复后 WoodBox 定向测试通过。
- Root cause: 新 AI 常量补丁没有先搜索现有常量声明，导致把已存在的标识符再次加入常量区。
- Prevention: 新增 AI/协议常量前先用 `rg -n` 在 Go 包内搜索完整标识符，确认唯一声明位置；随后立即运行 `gofmt` 和 `go test ./cmd/crystal-server -run '^$'`，包级编译通过后再写行为测试。
- Verification: 删除重复声明并格式化后，包级空测试和 `TestWoodBox` 定向测试均通过。

### 2026-08-17 — DarkCaptain 范围、推退与传送必须按客户端副作用验收

- Symptom: DarkCaptain 初版范围测试把半径边界、Fullmoon 推退和传送后的目标切换当成单一伤害行为，遗漏了边界之外目标、`ObjectPushed` 广播及传送后下一 tick 重新选目标的可观察结果。
- Root cause: 只按技能名称概括了 AI 效果，没有分别展开 Thunder/MassThunder 的范围比较、Fullmoon 的每目标推退通知和 `Teleport` 对缓存目标字段的同步；真实会话还可能被后台维护 tick 抢先消费。
- Prevention: 新 AI 测试先建立坐标半径/目标种类矩阵，再按每个目标的私有与观察者包序断言；推退必须验证坐标、方向和 `ObjectPushed`；传送改变实体人口后同步 `MonsterAITargetID/Kind`，真实 `net.Pipe` transcript 在手工 tick 前冻结初始化、搜索和动作时间并用 KeepAlive 屏障。
- Verification: DarkCaptain world 测试覆盖 Player、owned Monster/Hero、半径边界、Thunder/MassThunder、Orb、Line/Fullmoon、传送、延迟伤害和 Slave 清理；authenticated `net.Pipe` transcript 与包级定向回归通过，修复后的传送后弱目标选择测试确认缓存目标已同步。

### 2026-08-17 — Go 命令的仓库根目录必须直接复用核验结果

- Symptom: 一次 BlueSoul Go 命令误用了重复目录 `Crystal.GoServer.GoServer`，进程未启动；该调用没有产生源码证据。
- Root cause: 复制绝对工作目录时凭记忆再次拼接 Go 仓库名，没有直接使用最近一次 `git rev-parse --show-toplevel` 返回值。
- Prevention: 每次 Go 工具调用先在独立调用中核验 `git rev-parse --show-toplevel`，随后 `workdir` 只使用该返回值，命令参数只允许当前 Go 仓库路径；进程启动失败时整条调用作废。
- Verification: 失败调用未写入文件；BlueSoul 的矩阵、格式化和测试均在重新核验的 `Crystal.GoServer` 根目录完成。

### 2026-08-17 — BlueSoul Hero 夹具必须物化 Legacy 等级敏捷

- Symptom: BlueSoul Hero 防御测试最初遗漏了 Hero level-20 materialized agility，实际防御随机上界为 `16`，导致按 `1` 配置的确定性测试夹具与 Legacy 行为不一致。
- Root cause: 只初始化了 Hero 的基础坐标/HP 与零值防御字段，没有按客户端可观察的等级物化属性复核 MAC+Agility 防御计算。
- Prevention: 为 Player、owned Monster、Hero 分别从 materialized stats 推导防御随机上界；新增/修改 Hero 夹具后先记录每次 `Random.Next(bound)` 的真实 bound，再断言 MAC、Agility 和伤害路径。
- Verification: 修正 Hero level-20 materialized agility 夹具后，BlueSoul 定向 world/session 测试通过，并锁定实际 `bound=16`。

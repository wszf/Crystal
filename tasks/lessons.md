# Active lessons

Read this file at the start of every session. It contains only canonical,
cross-cutting prevention rules. Historical feature-specific evidence is stored
under [`tasks/lessons-archive/`](lessons-archive/README.md); do not read that
directory wholesale.

Before repeating a workflow or starting a migration batch, search the archive
with the AI ID, entity/module name, and failure/workflow keyword, then read only
matching sections. Keep active lessons within the recommended limit of 50 KB
or 500 lines. Strengthen an existing canonical lesson instead of adding a
duplicate.

### 2026-08-21 C01 — 每次工具调用必须保持单仓库路径闭包

- Symptom: Go/Legacy 路径混入同一调用，出现部分成功输出、尾部失败或错误目标。
- Root cause: 把工作目录、仓库根和参数列表分开考虑，并复用了另一仓库的路径。
- Prevention: 一次调用只属于一个仓库；先核验 `git rev-parse --show-toplevel`，参数只允许该根下已存在的路径；切换仓库必须新开调用；不得在当前 workdir 中用 `git -C`、绝对路径或脚本参数指向另一仓库；不得以只读、并行或减少往返为例外。
- Verification: 命令零退出且所有路径属于同一根；任一读取失败、非零退出或混合根调用时，丢弃该调用的全部输出（包括前面成功的片段），不得用于实现、测试归因或文档。 本次 BaseStats 审查又在 Go workdir 的只读调用中误带 Legacy `Shared/Data/Stat.cs`，整次输出已丢弃并按两仓分别重跑。本轮跨仓库 status 审计因混入另一根路径作废，随后已拆成两次单仓调用重跑；本批一次 Legacy workdir 混入 Go 文件路径的只读调用同样整体作废，随后按仓库拆开重跑；本轮 Legacy 方向核对命令再次混入 Go 相对路径，整次输出作废，随后按仓库边界重跑。 本轮一次 Legacy 读取调用误附 Go 相对路径，整次输出再次作废并已拆分重跑；一次委派消息误将已选 AI=8 写成 AI=80，相关 AI=80 tracing 已明确丢弃并按 AI=8 重做；本轮两次继续勘察时又把 Legacy lesson/archive 路径或 Go 源码路径混入对侧 workdir，相关调用输出均作废，随后已按仓库分别重跑。 本 Session 首次恢复读取又在 Legacy workdir 的同一命令中加入 Go migration-matrix 绝对路径；整次约 8 万 token 输出已作废，并按两仓独立调用重新读取。 本批 Notice 勘察又在 Go workdir 的同一读取中附带 Legacy `Server/Settings.cs`，整次输出立即作废，随后分别在 Go 与 Legacy 根重跑并只采用独立结果。
- Strengthening after localized-welcome recovery: compact 后首个启动读取再次把 Go matrix 绝对路径放入 Legacy 调用；该调用全部输出已作废，随后两仓 status、文档和 matrix 均以独立 workdir 重跑，并以重建 handoff 为恢复 authority。
- Strengthening after TestServer/GameMaster selection: compact 硬门恢复时又在单次启动审计中通过 `git -C` 混读两仓；该调用全部输出已作废。后续只在对应 `workdir` 内使用相对路径，并分别重跑 Legacy 文档/status/C# 门禁与 Go matrix/status/C# 门禁后才刷新 handoff。
- Strengthening after Superman recovery: 连续两次在 Go `workdir` 的上下文读取中追加 Legacy 相对路径，导致前段成功输出与尾部路径失败混杂；两次调用全部作废。此后构造命令前先把每个路径按仓库分类，命令文本只允许出现当前 `git rev-parse --show-toplevel` 下的相对路径；跨仓证据必须拆成相邻但独立的工具调用，并分别要求零退出后才可采用。
- Strengthening after Observer recovery: 新 Session 首次门禁读取又在 Legacy `workdir` 中追加 Go matrix 路径，整次 9 万余 token 输出已作废；随后 `agents.md`、三份 Legacy 文档、双仓 status/`.cs` 门禁与 Go matrix 全部按单仓调用重跑。恢复时还发现 handoff 声称 Go clean、实际已有 5 tracked + 2 untracked Observer 文件，因此在重建 handoff 前停止实现和测试。
- Strengthening after ALLOWTRADE recovery: compact 后首个恢复读取再次在 Legacy `workdir` 中拼入 Go matrix 绝对路径，约 9.7 万 token 的整次输出已作废；随后按仓库分别完整重读启动文档与 matrix。恢复审计又发现 handoff 声称 Go clean、实际已有 3 tracked + 3 untracked ALLOWTRADE 文件，因此在补写并回读准确 handoff 前继续停止实现和测试。以后恢复命令必须先按仓库写成两个独立命令块，再逐块调用，禁止在一个 shell 字符串中“顺便”读取对侧文件。
- Strengthening after `@TIME/@ROLL/@MAP` recovery: compact 后首个启动读取再次在 Legacy `workdir` 中同时 `cat` Go matrix，整次约 9.8 万 token 输出已立即作废；随后 Legacy 的 `agents.md`/三份任务文档与 Go matrix 均按独立 `workdir` 分块完整重读。恢复审计还发现旧 handoff 声称 Go clean、实际已有 4 tracked + 2 untracked utility-command 文件；实现和测试继续停止到准确 handoff 写入、回读并与双仓状态核对完成。本次接续又在 Legacy 启动命令末尾附加绝对 Go `find`，因此整次输出再次作废；恢复模板今后不得包含任何“定位对侧文件”的尾命令，第二仓的定位、读取和审计必须从新的工具调用开始。
- Strengthening after utility-command compact continuation: 恢复命令再次把绝对 Go matrix 搜索拼进 Legacy 文档读取，导致整次 10 万余 token 输出作废。以后启动阶段先只执行 Legacy 固定清单并结束调用；收到其零退出结果后，才在新的 Go `workdir` 调用中读取 matrix，禁止用一条复合 shell 命令跨越仓库边界。
- Strengthening after 18:24 utility-command rollover: 本次接续仍在 Legacy 启动读取中直接 `cat` 绝对 Go matrix，整次 10 万余 token 输出再次作废；随后不仅按仓拆开，还把超过输出上限的 matrix 区段进一步缩小重读到无截断。恢复模板必须物理拆成两个独立工具调用，且大文件必须按可完整返回的小段读取；“命令零退出”不能替代“输出完整可见”。
- Strengthening after fourth utility-command compact: 本次恢复首调用又在 Legacy 文档读取末尾追加了对侧 Go 根的绝对 `find`，整次输出按 C01 作废；随后先以纯 Legacy 调用完整重读启动文档，再以纯 Go 调用分块重读 3223 行 matrix。恢复时禁止在第一仓命令中承担任何对侧“定位”工作；若 matrix 区段发生输出截断，该区段也必须整体作废并缩小范围重读。
- Strengthening after bounded-control recovery: 控制面已经明确禁止跨仓后，首次恢复调用仍把绝对 Go matrix 路径附在 Legacy 文档读取后，约 8 万 token 输出再次全部作废。随后已物理拆成 Legacy 启动/状态与 Go 状态/matrix 两组调用，并以三项指纹核对十二文件未变。以后发送恢复命令前必须先做“命令文本内是否出现另一仓根”的机械检查，不能只检查 `workdir`。
- Strengthening after `DISC-P1-CLOSURE`: P1 版本勘察又在 Legacy `workdir` 的同一命令中加入 Go 的 `internal/config`/`cmd/crystal-server` 相对路径；compact 恢复首调用随后再次把绝对 Go matrix 附在 Legacy 启动读取末尾。两次整调用均已作废并按两仓独立零退出重跑。即使核对同一 setting 或启动 authority，也必须在发送前机械检查“命令文本只含当前根”，且 Legacy 启动调用必须物理结束后才能构造 Go 调用。
- Strengthening after Goal restart for `CFG-P1-CONTRACT-001`: 新 Goal 的首个启动调用仍用 `git -C` 在 Legacy `workdir` 中核验 Go 仓库，整次输出已作废；随后每份启动文档、双仓 status/`.cs` 门禁和指定 matrix anchors 均拆为单仓零退出调用重读。今后启动模板的第一步必须先做字面预检：命令中出现 `git -C` 或对侧根即拒绝发送，而不是依靠执行后的人工发现。
- Strengthening after `LOG-P1-CATEGORY-001` recovery: 本轮首个状态调用再次在 Legacy `workdir` 中用 `git -C` 混入 Go 仓库；整次输出立即作废，随后两仓 HEAD/status/三类 C# 门禁分别以独立零退出调用重跑。即使 handoff 已给出两仓命令，发送前仍必须逐字拒绝任何含 `git -C` 或对侧根的启动命令。
- Strengthening after `NET-P1-GATES-001` recovery: 本轮首个恢复调用又在 Legacy 根用 `git -C` 读取 Go 状态，整次输出已作废并按仓重跑。恢复模板今后必须把“两仓核验”落实为两个物理工具调用，而不是同一 shell 中的两个标题段；发送前机械拒绝 `git -C`。

### 2026-08-21 C02 — 路径、glob、正则和 shell 字符串必须先做最小验证

- Symptom: 猜测目录、空 glob、裸反引号、错误正则或未闭合字符串导致勘察失败。
- Root cause: 依赖 shell 隐式展开和记忆中的文件布局，没有先验证最小查询。
- Prevention: 先用 `rg --files` 列精确文件；优先 fixed pattern 或显式 `-e`；引用正则并检查字符串、反引号和参数边界；所有 `rg` 选项必须放在 `--` 前，`--` 后只能放 pattern/路径；禁止未引用 glob，也禁止把换行文件列表放进 zsh 标量命令替换后期待自动分词；shell 变量不得使用 `PATH`/`path` 等环境保留名或 zsh 只读特殊参数（如 `status`）；多文件列表直接用 `rg --glob`，或用 NUL 分隔加 `xargs -0`；调用 CLI 子命令前按对应 `--help` 核对全局与子命令选项位置；数据库对象名必须从实际 schema 复制，禁止查看 schema 后仍使用惯例名称猜测；精确 commit/thread/agent ID 必须从权威命令输出复制，禁止根据短 ID 猜测补全；调用语言模块前先核对运行时版本/可用性，并优先让目标程序自身解析配置；修改含非 ASCII 的文档时优先使用 `apply_patch`，若必须用脚本 here-doc，先最小验证解释器与源码编码。
- Verification: `rg` 选项、`path` 覆盖 `PATH`、未命中 glob 和 zsh 标量命令替换均已最小重跑；本批 archive 检索从失败的裸 `*.json`/换行标量改为直接 `rg --glob` 后零错误完成；Python 3.9 缺少 `tomllib` 时改由实际 Codex CLI 解析配置；Codex 验证脚本将只读 `status` 改为 `rc`，并把全局 approval 选项移到 `exec` 前后成功执行；Codex 0.148.0 不支持对 `features`/`debug` 使用全局 `--strict-config`，且 `| ... || true` 曾掩盖该错误后误打印 PASS；该证据已作废，后续先查目标子命令 `--help`、保留上游退出码，并只在命令真实零退出后报告通过，本次改由 `app-server --strict-config` 零退出与 `doctor` 的 `config loaded` 交叉验证；Goal 数据库查询从猜测的 `goals` 改为 schema 中实际的 `thread_goals` 后成功核对字段约束；本批从 `cmd/crystal-server` 子目录误用根级 `./cmd/crystal-server` 失败后，改用 `go test .`，并将仓库根/命令包路径作为同一最小验证；本 Session Legacy `/usr/bin/python3` 拒绝含中文的 here-doc 后，确认 Python 3.9.6 并改用 `apply_patch` 零错误写入文档；本批 handoff 初稿根据 `3e85ec4` 猜测完整哈希，随即以 Go 仓库 `git rev-parse HEAD` 的 `3e85ec4c4268bc4a24e5ec8cc0ff7a96ef58775c` 替换并回读核对。
- Strengthening after localized-welcome review: 再次猜测仓库根存在 `Localization/` 导致 `rg` 读取报错；整次调用证据已丢弃，随后先用根级 `rg --files` 定位实际 tracked localization fixtures，确认 server 根目录文件由运行时生成而非仓库资产。
- Strengthening after TestServer bootstrap: Go 勘察再次把不存在的 `world_player*.go` 作为未引用 glob 交给 zsh，并有数次让预期“零匹配”的 `rg` 在 `set -e` 下终止调用；相关调用输出均作废。后续先用 `rg --files` 定位文件，并仅在“零匹配本身是有效答案”时显式使用 `rg ... || true`，实现、测试和文档只采用零退出的重跑结果。
- Strengthening after Superman recovery: 勘察 session-local MP 扣减入口时又让预期可为零匹配的 `rg` 在 `set -e` 下退出 1；该调用证据已作废并以显式 `|| true` 零退出重跑。后续每个搜索在执行前先声明“零匹配是答案还是错误”，前者禁止与裸 `set -e` 组合。
- Strengthening after Superman review: 主审又把已知不存在的 Legacy 根级 `Localization/` 和猜测的 `Monsters/Plague.cs` 传给读取命令；两次调用全部作废，随后分别用现有 `Server`/`Shared` 路径和 `rg --files | rg -i plague` 定位 `PlagueCrab.cs` 后重跑。曾经验证过路径不存在也不能成为下一次跳过最小文件枚举的理由。
- Strengthening after `luna_worker` config validation: 首次把 `app-server --listen off` 猜成只解析配置模式，实际因无 transport 退出 1；随后又假定 malformed custom agent 会令进程非零，但 Codex 0.148.0 只打印 `Ignoring malformed agent role definition` 并退出 0。根因是把进程退出码误当成 custom-agent loader 的完整裁决。以后使用 `app-server --strict-config --listen stdio:// < /dev/null`，同时检查退出码和 stderr 中的 malformed/ignored-agent 诊断；`doctor config.load=ok` 只能作为主配置交叉证据。目标 `luna-worker.toml` 已以退出 0、无 malformed-agent 诊断和 required-field 回读验证；故意缺少 `developer_instructions` 的隔离负控确认了 stderr 门禁不可省略。
- Strengthening after Observer archive search: 首次检索猜测 `tasks/lessons-archive/manifest.json` 存在并退出 2，整次结果作废；随后先用 `rg --files tasks/lessons-archive` 定位实际 `manifest/2026.jsonl`，再对两个已存在文件零退出重跑。archive manifest 也必须先枚举，不能依据常见命名猜路径。
- Strengthening after utility-command recovery: 勘察 Go 时钟 authority 时又把不存在的未引用 glob `cmd/crystal-server/*clock*.go` 交给 zsh，命令在搜索前退出 1；整次输出已作废，随后改用已存在目录上的 `rg --glob '*.go'` 重跑。接续审查又猜测 Legacy `Server/Program.cs` 与 Go `go.sum` 存在，两个调用输出均作废并分别从已枚举的 `Envir.cs` 与 module-file 清单重跑。即使只是追加一个“可能存在”的文件或文件族，也必须先枚举或使用 rg 自身的 glob 过滤，禁止把惯例路径直接加入读取参数。
- Strengthening after utility-command hard-gate process audit: 首个进程检查因使用临时文件后 `rm -f` 被执行策略在启动前拒绝；第二个正则又匹配了自身命令行中的诊断文本，两个结果均作废。随后改用 `ps -Ao pid=,comm=,args=` 并只按 `comm` 精确匹配 `go`/`crystal-server`，零退出确认无残留进程。进程审计不得依赖临时文件清理，也不得用会命中自身正则或输出文案的全文匹配。
- Strengthening after Goal model audit: 已有上述禁令后，Codex 配置验证仍再次使用 `mktemp` 后 `rm -f`，在启动前被策略拒绝；随后又凭记忆在错误数据库查询 `thread_goals`、并猜错时间戳列名，导致多次非零和部分输出作废。根因是没有在执行前把旧 lesson 转换成当前命令的机械检查。以后只读诊断默认用 shell command substitution 捕获 stdout/stderr，不创建需清理的临时文件；数据库先 `find` 定位当前 `$CODEX_HOME`，再用 `sqlite_master`/`pragma_table_info` 独立零退出确认库、表和列，每个尚未确认的查询必须单独调用。此次已用无临时文件的 `app-server --strict-config ... </dev/null` 验证 `gpt-5.6-sol/ultra` 退出 0，并从 Codex Box 的实际 `goals_1.sqlite`/`state_5.sqlite` 零退出回读 Goal 与线程模型状态。
- Strengthening after Goal pause audit: 已有上述 schema 防猜规则后，首次 `pragma_table_info` 投影仍直接使用未引用的 SQLite 关键字列 `notnull`，两个查询退出 1，整次输出作废。以后 schema 勘察第一步固定为独立的 `SELECT * FROM pragma_table_info(...)`；完整回读真实列名后才允许自定义投影，关键字列必须引用。随后已分别零退出回读 `thread_goals` 与 continuation-deferral schema，再查询指定 Goal。
- Strengthening after utility-command main review: 同一轮连续把 `--glob` 放到 `rg --` 之后、使用未验证 shell glob，并把不存在的目录加入搜索；相关调用均作废后按现有路径重跑。每次 `rg` 必须按固定 argv 模板构造：所有选项与 `-e` 在前，单个 `--` 居中，已验证路径在后；不得用尾部 `|| true` 掩盖语法/路径错误，只有事先声明“零匹配即有效答案”时才允许它。
- Strengthening after P1 matrix reconciliation: 用记忆中的整句搜索实际跨行的 matrix prose 得到空行号，随后把负数范围交给 BSD `sed` 并 exit 1；恢复审计又把预期可为零的 archive `rg` 裸放在 `set -e` 下，并在 Go 定位中使用未引用 shell glob。相关调用均已作废并以稳定单行片段、显式接收 `rg` 退出 0/1、已确认精确路径重跑。不得对空搜索结果做算术，不得把“零匹配”或“glob 恰好命中”当成 argv 已验证。
- Strengthening after `CFG-P1-CONTRACT-001` tracing: 查找 Legacy startup consumers 时再次把可能零匹配的 `rg` 裸放在 `set -e` 调用中，整次零输出结果已作废；随后先声明零匹配有效、显式接收退出码 0/1，并扩大到已验证的 `.cs` 路径后定位真实 `Server.MirForms/Program.cs` 入口。凡搜索“可能不存在的限定写法”必须在命令成形时就使用 0/1 分支，不能等退出 1 后补救。
- Strengthening after `LOC-P1-CATALOG-001` discovery: Active Index 将 `server_text_catalog*.go` 列为允许新建文件，主线程却在枚举只返回现有 localization 两文件后仍把这两个未来路径交给 `wc`，调用退出 1 且整次输出作废。清单中的 write authority 不等于文件已存在；每个候选必须以本次 `rg --files` 结果分类为 existing/new，只有 existing 才能进入读取参数。
- Strengthening after `LOC-P1-CATALOG-001` integration: 主审已有固定 argv 规则后仍把 `--glob` 放在 `rg --` 之后，调用 exit 2 且全部输出作废；随后清理本批测试生成文件时又把 `rm -f` 拼进补丁/测试复合命令，整次在执行前被策略拒绝。已分别以选项前置的 `rg` 和只删除精确自有文件的 `apply_patch` 零退出重跑；今后发送前同时机械检查 `rg` 的 `--` 边界和复合命令是否含被禁止的清理动作。
- Strengthening after `LOG-P1-CATEGORY-001` tracing: 同一批三次把 `--glob` 放到 `rg --` 之后，导致有效前段与 exit 2 混杂；每次整调用均作废并用 `rg [options] -e pattern -- paths` 重跑。发送任何 `rg` 前必须先按固定四段模板目检，禁止从自然语言顺序临时拼 argv。
- Strengthening after `NET-P1-GATES-001` recovery: 已用 `rg` 定位真实声明在 `internal/protocol/packet.go` 后，读取命令仍追加猜测的 `codec.go` 并 exit 1；整次输出作废并只用已枚举路径重跑。定位结果必须先结束并回读，下一调用的每个读取参数只能来自该结果，禁止再补惯例文件名。
- Strengthening after NET closure: 读取又猜了 `Shared/Packets`，且 `go build ./cmd/crystal-server` 在根生成未跟踪二进制。读取参数只取本轮枚举结果；证据构建用 `go build ./...`，status 必须识别并精确移除本批自产物。
- Strengthening after HTTP authority expansion: handoff 再次从短哈希 `88b0e15` 猜出错误完整值；立即以 Go 根 `git rev-parse HEAD` 的 `88b0e15771a909c18c64dd4040c12264251f5349` 修正。任何新 commit 的完整 ID 必须先在所属仓库独立回读，再允许写入对侧控制文档。

### 2026-08-21 C03 — 补丁必须使用精确、唯一、小范围上下文

- Symptom: patch 被拒绝、落到相似函数、部分 hunk 成功或格式化后锚点失效。
- Root cause: 使用陈旧正文、模糊锚点或人工拼装错误 hunk 标记。
- Prevention: patch 前复读精确物理行；按唯一函数锚点拆小 hunk；检查完整路径、上下文行和闭合标记。
- Verification: 逐段复读 diff，并运行格式化、最小编译和定向测试；任一 patch 失败时不采用同调用的其他结果。本批矩阵第二个 hunk 因正文换行与预期不符而失败；先复读并独立核验同调用已落地的第一个 hunk，再以精确物理行单独重跑第二个 hunk，最终 `git diff --check` 通过。
- Strengthening after Goal continuity patch: 一个补丁同时修改互为 hard link 的 `AGENTS.md` 与 `agents.md`；首个 hunk 已经同步改变两个路径，第二个 hunk 因旧正文消失而失败，形成“调用失败但修改已落地”。以后 patch 前先用 `ls -li`/`git ls-files` 核对别名与 inode；hard link 只修改一个 tracked canonical 路径，并在失败后立即独立回读全部别名和 Git 状态。本次已确认两路径仍共享 inode、内容均为 Sol Ultra/checkpoint-not-blocker，Git 只跟踪 canonical `agents.md` 变化。
- Strengthening after P1 inventory insertion: 一次四-hunk matrix 补丁因旧段落物理换行与草稿不一致而整体失败；随后已独立确认没有部分写入，再把新段插入、P1 单行精确替换和两个小 prose hunk 分开发送并逐项 `git diff --check`。长表插入与陈旧 prose 修订不得共用一个补丁事务；先复读每个唯一锚点，失败后先查 status/目标标记再重试。
- Strengthening after `LOG-P1-CATEGORY-001` wiring: 一个三-hunk 主文件补丁因其中 `stage = stageGame` 上下文未匹配而整体失败；随后 handoff 刷新又把只存在于 Active Index 的 ownership 正文当成 handoff 锚点。两次均确认零部分写入后按目标文件实际物理行拆成独立 hunk。跨越数千行或相邻控制文档的接线都必须逐文件复读并拆事务，不能因相似语义而复用锚点。
- Strengthening after NET closure: 单行日志补丁夹带不连续的陈旧上下文而失败；确认零写入后复读并用最小 hunk 成功。不得把记忆中的远端行拼进单行补丁。

### 2026-08-21 C04 — C# 基线只读，语言工具链严格隔离

- Symptom: 迁移或格式化流程误触 `.cs`，或用错误工具链验证不同语言。
- Root cause: 把 Legacy 对照和 Go 迁移实现当成同一可编辑工作区。
- Prevention: 所有迁移实现和工具使用 Go；`.cs` 只读；格式化、编译、patch 和提交按语言及仓库分组。
- Verification: 两仓分别检查 tracked、staged、untracked `.cs`，结果必须为空。
- Strengthening after LOC asset audit: 只读 `luna_worker` 未遵守“迁移工具只能使用 Go”，用 Python 解析两份 Legacy JSON；该报告的机械计数/placeholder 结论已撤销为验收证据，并改由 Go-only 生成器与自包含 Go fixture/tests 重做。委派 prompt 今后必须把“read-only audit scripts 也只能 Go”写成显式 acceptance gate，返回报告必须列出实际语言。

### 2026-08-21 C05 — 复用 API 前核对完整声明而非猜测对称名称

- Symptom: helper 不存在、receiver 遗漏、返回值数量错误、字段或常量名称猜错。
- Root cause: 依据相似模块、Legacy 名称或“应该对称”推断 Go API。
- Prevention: 先读取声明、receiver、参数顺序、返回值、领域类型和包级符号，再接线。
- Verification: 新调用接入后立即运行包级只编译门禁；本批测试夹具误写不存在的 `worldMagic.Spell` 字段后由编译器立即拦截，复读声明并改用实际 `Level` 字段后定向测试通过。
- Strengthening after `LOG-P1-CATEGORY-001` compile: 主线程在 `runServerWithContext` 中取得 `runtimeLogs` 后，误以为数千行后的独立 `serveWithConfig` 仍共享该局部变量，首次只编译门禁报三处 undefined。复读函数边界后通过保留旧签名的显式 wrapper/context 传递修复；任何跨入口接线都要先列完整调用链与变量所有权，再落调用点。

### 2026-08-21 C06 — 行为判断前先通过 Go 语法、类型和 vet 门禁

- Symptom: 未使用变量、自赋值、类型宽度、多返回值或复合字面量错误阻止行为测试。
- Root cause: 一次写入过多逻辑，在编译失败时仍试图分析生产语义。
- Prevention: 小步运行 `gofmt` 和 `go test ... -run '^$'`；显式转换不同领域类型，再进入行为测试。
- Verification: 最小编译、定向测试和 `go vet` 分层通过。
- Strengthening after NET frame-reader integration: 替换返回类型时漏改三个消费者，测试重构又留了未使用 import，均由只编译门禁捕获。接口变更先列声明与全部调用点，同一补丁改完后立即 gofmt/compile。

### 2026-08-21 C07 — 全量、race 和环境失败必须按实际栈归因

- Symptom: 新批次被既有 session/race 问题、缓存、磁盘或超时噪声误判为回归。
- Root cause: 只看最终 `FAIL`，没有保留退出码、测试名、栈和定向复跑结果。
- Prevention: 分开运行定向、普通全量和 race；保存退出码和失败摘要；环境错误先检查空间与可重建缓存。
- Verification: 只以实际失败栈是否进入本批代码或测试为归因依据，已知排除项写入交接而不修改无关模块。
- Strengthening after `CFG-P1-CONTRACT-001`: 必跑的 unexcluded 全仓测试复现既有 OmaMage `[2 1]`/`[1]` 后，首次只排除该项的重跑又偶发 YinDevilNode/42 空通知；后者独立 `-count=10` 通过，排除两项的全仓重跑、vet/build 和本 Leaf 定向/重复/race 全通过。全仓命令不得与 vet/build 串在同一 `set -e` 证据中后仍声称后两项执行；新失败必须逐个隔离，排除集合和“非 full pass”标签必须写入 matrix/handoff。

### 2026-08-21 C08 — 测试夹具必须复用真实 bootstrap、helper 和认证约束

- Symptom: 生产分支未执行，测试先因缺少 runtime 实体、派生属性、元数据或名称约束失败。
- Root cause: 人工夹具只建立持久数据，未复现登录、world enter、summon 或 bootstrap 生命周期。
- Prevention: 优先复用现有 helper；按 seed→load→enter→summon 顺序建立状态；满足真实账号、角色和配置约束。
- Verification: 先确认夹具到达目标生产入口，再断言行为和 transcript。

### 2026-08-21 C09 — Go value-map 实体修改后必须显式写回并回读

- Symptom: 局部副本中的 HP、毒物、计时器已改变，但 world 状态未变或被旧副本覆盖。
- Root cause: 把 `map[ID]Struct` 当作引用容器，多个 helper 交错读写旧副本。
- Prevention: 每阶段修改后立即写回；调用会重读 map 的 helper 前先提交；后续保存从权威 map 回读。
- Verification: 同时断言权威 map、下一 tick 行为和会话可观察结果。

### 2026-08-21 C10 — 持久层、runtime、动作队列和 wire 编码必须分层建模

- Symptom: owner key 与 ObjectID、持久目标类型与动作编码、绝对时间与剩余时间被混用。
- Root cause: 同一概念在不同层具有不同 ID、枚举、默认值和派生规则。
- Prevention: 明确 storage→runtime→queued action→wire 的转换边界，不跨层推断字段。
- Verification: 分别断言持久快照、运行实体、队列内容和 payload。

### 2026-08-21 C11 — 随机测试必须复现完整调用流和精确区间

- Symptom: 遗漏 `Next(1)`、混淆排他/包含上界，或初始化、后台和延迟阶段额外消费随机数。
- Root cause: 只按业务分支记录随机，没有展开嵌套 helper、构造、命中和防御抽样。
- Prevention: 按调用栈列出每次随机调用、顺序、阶段和 bound，并使用记录型确定性源。
- Verification: 分阶段断言随机序列，并重复普通/race 测试确认稳定；AI=77 HellPirate 的延迟命中还核对了固定范围 `Random.Next(1)`，确定性 hook 不再把合法的 unit-bound 消费误报为失败。

### 2026-08-21 C12 — 人工时钟必须隔离所有后台 tick 和全局时间源

- Symptom: 手工 tick 前 session loop 已移动、攻击、刷新状态或消费随机数。
- Root cause: 停止 world ticker 被误认为同时停止连接维护循环和其他实体 AI。
- Prevention: 冻结 ticker、session loop、独立实体计时和全局时间源；必要时等待 goroutine 完全退出。
- Verification: 重复/race 运行只出现人工推进产生的事件和随机调用。

### 2026-08-21 C13 — 延迟动作必须区分 admission、snapshot、impact 和后续 tick

- Symptom: 发起时状态被错误用于命中，或命中时新增 Legacy 不存在的距离、资格或状态门禁。
- Root cause: 没有明确哪些值在 admission 捕获、哪些在 impact 重验、哪些在后续 tick 生效。
- Prevention: 为每个延迟动作列出快照字段、命中重验项、Due 边界和动作后状态。
- Verification: 测试覆盖入队后状态变化、精确 Due、命中结果、后续 tick 和动作清理。

### 2026-08-21 C14 — 构造器、对象身份、population 和生命周期必须单独审计

- Symptom: ObjectID 冲突、自引用、错误方向/计时器、未召唤 runtime 对象或旧夹具被新 AI 污染。
- Root cause: 手工插入对象绕过构造器、`nextObjectID`、Spawn 初始化和 population 注册。
- Prevention: 复用真实构造/Spawn；手工插入时推进 ID 并初始化完整可观察状态；新增 AI 同步 population。
- Verification: 分别断言构造后、首次 tick、攻击后及死亡/清理状态。

### 2026-08-21 C15 — 坐标、方向、距离和地图布局必须从夹具计算

- Symptom: 手写距离、方向 ordinal、x/y 顺序或推退方向与实际算法不一致。
- Root cause: 依据视觉直觉，没有使用项目的距离、方向表、地图布局和边界规则。
- Prevention: 从攻击者、目标和地图尺寸计算几何；区分移动向量、最终朝向和 payload 方向。
- Verification: 同一计算变量驱动动作时间、位置终态和 wire 断言。

### 2026-08-21 C16 — 数值期望必须代入真实公式、派生属性和类型宽度

- Symptom: HP、MP、伤害、费用、持续时间或随机 bound 按直觉计算错误。
- Root cause: 忽略等级、派生属性、钳制、中间状态、协议宽度或当前库存。
- Prevention: 逐项代入生产公式，明确单位和整数类型，并从夹具真实字段推导。
- Verification: 同时断言中间值、最终值和对应通知，而不是只检查单个结果；本批 BaseStats 测试首次把 Wizard Mana 特例误算并把 `Gain == 0` 的计算短路误当成字段清零，修正后按原始 profile 与逐分支计算分别验收。

### 2026-08-21 C17 — 协议变更必须同步 ordinal、布局、方向和完整 payload

- Symptom: ordinal 基线、字段长度、包方向或尾字段更新不完整。
- Root cause: 从 packet ID 或相似包推断 wire，没有读取完整 enum 和 serializer。
- Prevention: 以目标 wire 基线和当前常量表为准；核算字段宽度和完整帧；使用非零语义哨兵。
- Verification: ordinal 向量、编码/解析 round-trip、固定 payload 和双向 transcript 全部通过。

### 2026-08-21 C18 — Transcript 必须验证完整事件顺序和所有副作用

- Symptom: 主行为正确但漏掉 bootstrap、练习、状态、聊天、移动或延迟包。
- Root cause: 把领域成功或一个响应包当作整个动作的完成屏障。
- Prevention: 沿实际入队和广播点列出完整有序事件；禁止 map 驱动有序期望；保留中间状态。
- Verification: 逐包核对数量、顺序、payload 和最终领域状态。

### 2026-08-21 C19 — 通知必须按接收者和可见性矩阵验收

- Symptom: 私有包、目标包、观察者广播或跨地图移除发送给错误对象。
- Root cause: 只验证事件存在，没有展开 fan-out、源坐标和接收者边界。
- Prevention: 建立 actor/target/owner/observer/其他地图矩阵，并区分私有与公开 payload。
- Verification: 每个接收者独立消费 transcript，同时确认不应接收者没有包。

### 2026-08-21 C20 — net.Pipe 测试必须建立 reader、就绪屏障并消费完整前导

- Symptom: 服务端先写导致阻塞，bootstrap 残留包抢占预期响应，或 cleanup 死锁。
- Root cause: 假设请求后才有响应，或把最后一个 bootstrap 包当作稳定 game loop。
- Prevention: 写入前启动所有 reader；并发消费多接收者；使用 post-bootstrap 屏障；异步 callback 不直接调用 `Fatal`。
- Verification: 普通、重复和 race 模式均无阻塞，包数与接收者矩阵一致。Superman 认证
  transcript 首版在测试 goroutine 中同步调用 `deliverWorldNotifications`，而客户端 reader
  尚未启动，两个测试均等到 30 秒 pipe timeout 后失败；改为先启动 delivery goroutine、
  主测试同步读取并在独立 channel 回收发送结果后，两个定向 session tests 退出 0。

### 2026-08-21 C21 — 迁移必须沿真实 Legacy 调用链、动态类型和 override

- Symptom: 按名称、注释、陈旧矩阵或相似实现迁移，遗漏重载、尾部副作用或 Legacy 怪癖。
- Root cause: 读取声明但没有追踪 Spawn、调用者、helper 和消费者。
- Prevention: 从真实入口追到构造类型、override、共享 helper 和所有消费者；当前源码与测试优先于文档。Review finding 也必须回到 Legacy 实现裁决，不能把“通常不应发包”“应显示实际变化量”或“应避免整数回绕”等常规工程直觉覆盖原版怪癖。
- Verification: 用生产入口测试覆盖可达路径、历史怪癖和关键失败分支。本批只读 review 把 Plague 在零 MP 时仍发 `HealthChanged`、治疗接近满血时显示请求恢复量、`int32` unchecked 运算列为风险；主审回读 `Map.CompleteMagic`、`HumanObject.ProcessRegen/ChangeMP` 和项目 overflow 配置后确认三者均为 Legacy 行为，未按直觉“修正”，并保留对应边界测试。
- Strengthening after P1 config compatibility: 既有 `TestVersionCheckingRequiresAFile` 首次失败，因为测试把 Go 的 fail-fast 规则当成权威；Legacy `LoadVersion` 实际跳过缺失路径并保留空 hash 列表，启用版本检查时由客户端 gate 全部拒绝。测试已改为锁定空列表/全拒绝，缺失、空白、多文件和 partial MD5 定向/重复/race 全通过。任何“更严格更安全”的配置错误都必须先由 Legacy loader 和消费者共同裁决。
- Strengthening after `NET-P1-GATES-001` tracing: Active acceptance 草稿把 MaxPacket reset 凭直觉写成一秒，Legacy `MirConnection.ReceiveData` 实际在严格 `< Now` 时重置并设为 `Now.AddSeconds(5)`。时间窗口、比较边界和计数单位必须从真实入口逐项抄录后再写验收清单；本次在任何 NET 代码写入前改回五秒并保留 equality 边界待测。
- Strengthening after NET review: 测试用 Go ordinal 猜 Legacy enum 长度，并凭印象写 IPv6 首段；真实值是 153 项和 `[2001`。期望必须从目标表达式与 enum 逐项编号，不得跨基线借用相邻协议表。

### 2026-08-21 C22 — 不同 capability、门禁和业务阶段不得过度复用

- Symptom: 搜索 gate 被用于攻击、权限被误认为绕过冷却，或特殊分支绕过通用门禁。
- Root cause: 把相似布尔条件合并为一个过宽 helper。
- Prevention: 分别建模搜索、移动、攻击、可见性、权限、冷却和状态门禁；复用前证明语义集合完全相同。
- Verification: 为每个 gate 提供独立成功/失败组合测试。

### 2026-08-21 C23 — 权威事务、锁、落盘和网络投递必须分层排序

- Symptom: 网络失败阻断持久化，跨领域锁死锁，或重试重复领域副作用。
- Root cause: 把状态提交、序列化和尽力通知放在同一锁内或重试循环中。
- Prevention: 锁内原子更新领域状态；释放锁后落盘；最后投递网络；重试只重复可幂等步骤。
- Verification: 覆盖失败回滚、断线保存、批量通知继续和 race 门禁。

### 2026-08-21 C24 — 跨 world/auth/session 状态必须以 authority 和 revision 同步

- Symptom: 权威状态已变，但在线 session 使用陈旧成员关系、物品或地图快照。
- Root cause: 直接覆盖整快照，或在错误 authority/锁域内判定。
- Prevention: 明确字段 authority；分离锁；按 revision 定向合并并主动唤醒目标会话。
- Verification: 跨会话测试确认权威存储、在线快照和双方通知一致。

### 2026-08-21 C25 — 导入导出和 round-trip 必须验证语义规范化

- Symptom: 字段解码成功但加载语义错误，或 nil/empty、map、时间表示造成伪差异。
- Root cause: 把 schema 对称等同于领域加载等价。
- Prevention: 逐字段核对 Legacy 加载顺序、默认值和 canonical 空集合；不可比较结构逐字段或 DeepEqual。
- Verification: Legacy→Go→持久化 round-trip 与截断、默认和非零哨兵测试通过。

### 2026-08-21 C26 — Schema、配置或常量扩展必须更新完整契约面

- Symptom: 生产字段已增加，但 defaults、JSON、Setup.ini、导出器或完整结构断言失败。
- Root cause: 只修改结构体或 loader，没有检索所有消费者和夹具。
- Prevention: 同步 schema、默认值、loader、runtime fallback、导出器和兼容性测试。
- Verification: 相关内部包定向测试及全仓测试共同通过。

### 2026-08-21 C27 — 测试判据必须来自生产入口和当前权威状态

- Symptom: 参考模型通过但生产入口未覆盖，或测试读取旧副本、陈旧矩阵和手写常量。
- Root cause: 测试复制实现，没有观察真实入口、最终 map 状态和可观察协议。
- Prevention: 通过生产入口驱动，并从当前源码、夹具坐标、公式和权威实体推导期望。
- Verification: 同时验证领域终态、持久状态和网络 transcript。
- Strengthening after `NET-P1-STATUS-001`: 独立 service 与注入 listener 的测试虽覆盖 payload/计时/关闭，仍未证明真实 runtime 入口按 `cfg.IPAddress:3000` 打开第二 listener。生产接线必须保留可注入的最小系统调用 seam，并通过同一 runtime 函数断言 game→status 的实际调用顺序、精确地址和关闭；helper-only 测试不能冒充 production-entry evidence。

### 2026-08-21 C28 — Active lessons 只保留可执行、跨批次规则

- Symptom: 单次异常、某个 AI 数值或已知失败栈不断成为固定汇报项，且复合 AI 标题只索引首个 ID，导致上下文膨胀或检索漏项。
- Root cause: lessons 同时承担事故日志、交接记录和长期规则；归档职责不清，索引器把 `AI=52/53` 当成单 ID。
- Prevention: 复发项强化同一 canonical；功能特定或历史事实进入对应 archive 文件；已归档证据只追加不删除，冻结的 legacy 分片不得改写；manifest 的 `ai_ids` 必须收录复合标题中的每个 ID；active 达到 50 KB 或 500 行前归档。
- Verification: active 每条规则跨批次可用且通过大小门禁；manifest 可按任一复合 AI ID 定位冻结原始块，831 个 legacy 历史块仍可无损重建。

### 2026-08-22 C29 — compact 前必须完成并校验 durable handoff

- Symptom: 自动 compact 摘要遗漏当前未提交批次、仓库状态或失败归因，恢复后的信息与预期不一致。
- Root cause: 把 compact 摘要当成迁移记录，或只在“快要 compact”之后才补 handoff，导致 compact 前没有可核验的完整状态边界。
- Prevention: 每次 compact 前，或收到 compaction/上下文上限/rollover 信号时，立即停止实现和测试，先写/刷新 `tasks/migration-handoff.md`；即使当前批次只有 Markdown/文档变更也必须执行。记录两仓路径、分支/HEAD、完整 tracked/staged/untracked 状态、所属文件、测试退出码与失败归因、矩阵行、未提交工作和恢复命令；回读并对照两仓校验后再 compact。compact 后沿用同一 active Goal，不因 compact 单独重开 Goal；自动摘要仅作不可信上下文。
- Verification: `agents.md`、`tasks/goal-task.md` 和 `tasks/migration-handoff.md` 均将其定义为 hard gate，并要求无 handoff 时先从两仓重建记录再继续；本次 compact 摘要声称已刷新，但 handoff 仍写 Go clean、实际却有 8 tracked + 2 untracked，因此已停止实现/测试并从两仓重建后才恢复。
- Strengthening after third utility-command rollover: compact 恢复环境只列出一名 direct subagent，但首版 handoff 回读期间 Go 状态从九文件继续增长为十二文件，证明另一个未暴露 worker 尚在写入；仅看环境 `<subagents>` 和 `go`/server 进程不足以证明 quiescence。Hard gate 今后先关闭所有已知 agent，再核对当前 thread-writer locks；对未知 lock ID 用 agent tool 停止并收取 memory-only 报告，直到只剩主线程 lock。随后至少两次对照工作树，第二次必须发生在 handoff 写入后；任一文件列表、mtime 或 status 变化都使草稿失效，须重新冻结并回读。本次据 lock 精确关闭 `01a02e52-2e36-7951-a009-65595fdd6d7e`，收取其三文件/测试记账，确认只剩主线程 lock、无 Go/server 进程且 Go 状态稳定为 6 tracked + 6 untracked 后重写 handoff。

### 2026-08-23 C30 — Subagent 模型不可用时必须在执行前显式说明

- Symptom: 整个批次未使用 subagent，用户只能在事后从结果推断原因，容易误认为主 Agent 忽略了既定的 `luna_worker`/`gpt-5.6-luna` 分工。
- Root cause: 工具发现确认当前 spawn override 列表不提供要求的 worker/model；为遵守“不得静默替换”而改由主 Agent 本地执行，但只把原因写进 plan/handoff，没有在开始 tracing 前用清晰的用户可见说明建立预期。
- Prevention: 每批开始先发现并核对 subagent 工具、角色、model 和 reasoning effort；可用时把非关键路径的 bounded 独立工作委派并记录 agent ID/范围，不可用时在本地执行前立即明确说明缺失项、禁止替代规则和 fallback。不得既不委派也不解释，也不得用其他模型静默冒充指定 worker。
- Verification: 本批实际 spawn model 列表只有 `codex-auto-review`、`deepseek-v4-flash`、`deepseek-v4-pro`、`gpt-5.3-codex-spark`、`gpt-5.4`，不含 `luna_worker` 或 `gpt-5.6-luna`；因此未生成 agent ID。后续 handoff 必须记录实际 agent ID/模型/范围，或记录执行前已公开的不可用证据。
- Strengthening after Luna availability recovery: 当前工具已提供固定 `luna_worker` 角色，但首次同时传 `agent_type=luna_worker` 与 `fork_context=true` 被 API 拒绝且未生成 agent。固定角色今后使用独立完整 prompt 且省略 full-history fork；成功返回精确 agent ID 后才算委派成立。本轮已按该形式成功启动 formatter writer 与 Legacy read-only auditor。
- Strengthening after formatter review interruption: 主 Agent 看到 worker 最后一次写入后迟迟未返回且无 `go` 进程，误用 `interrupt=true` 催收报告，恰好打断两个 helper 签名的同步修改，留下可复现编译错误。以后不得仅因报告延迟中断仍为 active 的 writer；先继续非重叠工作并正常等待，确需停止时先发非中断的收尾请求。任何中断/停止后的工作树一律视为未验证，立即运行最小编译并由主 Agent 修复、复审和重测。本轮两个签名错误已由 `go test ... -run '^$'` 捕获，formatter 全量定向测试随后恢复为 exit 0。

### 2026-08-23 C31 — Goal rollover 是 durable checkpoint，不是 blocker

- Symptom: Goal 线程 `01a02d0d-6a74-75f2-a72a-a2f2736980a2` 在第四次 compact 后连续三个自动续跑 turn 只重复“必须打开新 Session”，第三个 turn 由主 Agent 调用 `update_goal({"status":"blocked"})`，使仍可继续且无预算限制的长期 Goal 停止自动推进。
- Root cause: 把仓库自定义的 token/rollout/compact 会话卫生阈值误判为外部 impasse；安全 handoff 已完成、工具仍可用，却没有从恢复点做有意义工作，因而机械满足了三轮 blocked audit。
- Prevention: token 数、rollout 大小、compact 次数、性能下降和“偏好新 Session”只作观测，不得独立触发 checkpoint 循环、停止或状态变化；只有真实 compaction/context-limit/new-session 信号才触发冻结、handoff 与关闭 subagents。handoff 核验后同一 Goal 的下一自动续跑必须恢复工作。不得仅因会话卫生阈值 pause Goal 或调用 `update_goal(status="blocked")`；只有平台或外部状态实际阻止任何有意义进展并连续满足 blocked audit 时才可标 blocked。
- Verification: 指定 rollout 无 `turn_aborted`，末尾事件明确显示两次自动 continuation 后的第三轮 `update_goal({"status":"blocked"})`；Goal 无 token budget、无 continuation deferral，停止前工具与 handoff 均可用。`agents.md`、`tasks/goal-task.md` 与当前 handoff 已同步为 checkpoint-not-blocker 规则。
- Strengthening after the utility-command compaction: 收到真实 compact 信号时虽然停止了实现和测试，但 compact 在 durable handoff 写入前发生，自动摘要只留下“现在准备刷新”的意图；恢复后旧 handoff 仍声称十二文件未变，而实际已有十五文件。真实信号到达后的第一项工具动作必须是写入并回读当前 handoff，不得先输出叙述性进度；若平台仍先行 compact，下一 turn 必须把摘要视为不可信并从双仓重建，完成核验前不得实现或测试。本次已按当前 status、指纹、进程和 `.cs` 门禁重建。

### 2026-08-23 C32 — 迁移控制面不得反向吞噬实现上下文

- Symptom: 线程 `01a02d0d-6a74-75f2-a72a-a2f2736980a2` 在约 6 小时 40 分内记录 11 个 compact window、295 次 matrix 相关调用、131 次 handoff 相关调用和约 870 万字符工具输出；当前 handoff 增长到 1421 行/128 KB，compact 恢复主要消耗在全量重读、历史追加和重复门禁，而不是继续实现。
- Root cause: 把详细 matrix、append-only handoff、事故历史和每叶全仓门禁同时当成启动上下文；每次 compact 都完整重读 3223 行 matrix 与全部旧 checkpoint，形成“文档越大→更快 compact→再次全读”的自放大循环。十三个宽阶段又不是有限分母，无法给出可信百分比或 ETA。
- Prevention: `migration-handoff.md` 只保留当前快照并限制为 250 行/24 KiB；独有未提交历史一次性归档，启动永不读 archive；使用 `migration-active.md` 注册唯一 active leaf、精确 matrix anchors 和 scope-freeze discovery leaves；compact 后只读 active+handoff 并核验状态；每叶运行 focused leaf gate，全仓普通/race 按有上限的 integration cadence 运行，阶段/Goal 收口仍要求新鲜无排除全量门禁；主线程禁止 broad dump。
- Verification: 旧 1421 行 handoff 已逐字复制到 `tasks/migration-handoff-archive/2026-08-23-2055-pre-control-plane-optimization.md` 后改为短快照；`tasks/check-migration-control.sh` 对 handoff、active index、goal contract、agents 和 active lessons 执行行数/字节/必需标题门禁；Go 十二个未提交文件在优化中保持原样，双仓 `.cs` 三类审计为空。
- Strengthening after utility/P1 closure: 将 active index 固定标题改成自然语言曾使控制脚本 exit 1；本次重建 handoff 又把必需的唯一 `- Active leaf: \`` 字段改写成普通叙述，再次被脚本拦截。控制标题和 section field 都是可执行 schema，补丁前必须先从 `tasks/check-migration-control.sh` 复制精确字符串；修复并让检查器 exit 0 后，才可采用回读、scope-freeze 或提交结论。
- Strengthening after `LOC-P1-CATALOG-001` closure: 主线程将 Active Index 从 LOC 路由到 LOG 并提交控制面，却遗漏把 Go matrix 的 LOG 行从 `Ready` 同步为 `Active`；下一循环按锚点回读时才发现 index/matrix 不一致。Leaf 状态转换必须作为跨仓事务检查：完成行、下一 Active 行、残余计数、Active Index 和 handoff 五项逐一回读后再开放实现；本次已先提交 matrix 状态修复，再刷新并提交 handoff，未在不一致期间写 LOG 代码。
- Strengthening after `LOG-P1-CATEGORY-001` closure: 按旧 prose 机械把“九未完成”减为八后，逐行重数十二条 P1 child 才发现 LOC 完成时残余数从未同步；LOG 完成后的真实状态是五 Complete、七 unfinished。状态转换的残余数必须由当前 registry 行重新计数，禁止只对旧叙述做加减；本次已在提交前同步修正 matrix、Active Index 和 handoff。
- Strengthening after NET route reread: LOG 与 Legacy 控制提交后首次读取命名 NET anchor，仍发现 Active Index=`Active`、matrix=`Ready`；说明“提交前同步”不能只靠叙述核对。以后路由事务在开放写权限前必须分别以 `rg` 回读旧 Leaf Complete、下一 Leaf Active、唯一 Active Index、registry 计数和 handoff Active 五个机器可见值；任一不一致先单独修复并提交。本次 NET 尚无代码写入，先补 matrix `Active`、刷新 handoff 后再勘察。
- Strengthening after NET handoff reconstruction: 快照虽含 Active leaf 语义，却把必需 schema 标题改成 `Candidate behavior and open review`，控制检查器 exit 1。重写 handoff 前必须先从检查脚本复制必需标题和字段，并在同一最小补丁中保留 `## Active leaf and protected work` 与唯一 `- Active leaf: \``。

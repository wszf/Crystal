# Crystal Go 迁移 Session 交接

最后更新：2026-08-23 20:55（Asia/Singapore；Goal continuity 与 Sol Ultra 规则修正，未改变 Go 功能边界）

## 迁移目标与硬边界

目标是把当前 Crystal 项目的服务端功能与客户端可观察行为 100% 迁移到独立、跨平台的 Go 项目。服务端、测试客户端、协议探针、Legacy 数据导入/导出器和其他迁移工具全部使用 Go。

- 原 Crystal 仓库和 Go 迁移仓库中的所有 `.cs` 文件都是只读对照基线，禁止新增、修改、重命名或删除。
- 每个功能批次必须同时覆盖领域状态、协议序列、持久化/重载和真实会话路径；仅存在 handler 不算迁移完成。
- 可以共享运行时和测试矩阵的功能应成组迁移，但不能牺牲行为等价、竞态安全和回归覆盖。
- 每批完成后更新迁移矩阵、运行质量门禁并 Git 提交；整体迁移未达到 100% 前不得标记最终 Goal 完成。

## Compact 硬门禁

- 每次 context compact 前，立即停止实现和测试，先写入或刷新本文件；即使
  当前批次只有 Markdown/文档变更也必须执行。不得把自动生成的 compact 摘要当作
  迁移记录。收到 compaction、上下文上限或
  rollover 信号本身就触发该门禁，不得等到压缩完成后再补 handoff。
- Handoff 必须记录两仓路径、分支/HEAD、完整 tracked/staged/untracked 状态、
  本批所属文件、测试退出码及失败归因、矩阵行、未提交工作和恢复命令；写完
  后回读并与两仓实际状态核对。
- Compact 后沿用同一个 active Goal，从已核对的 handoff 恢复；不得仅因
  compact 重开或重建 Goal。若没有已核对的 handoff，先从两仓重建，再继续实现。
- Compact 次数、累计 token、rollout 大小、性能下降或“偏好新 Session”只作观测，
  不得独立触发 checkpoint 循环、停止或 Goal 状态变化；只有真实
  compaction/context-limit/new-session 信号才触发 durable handoff。handoff 核验后，
  下一自动 continuation 必须在同一 Goal 恢复有意义工作。不得仅因会话卫生阈值
  pause Goal 或调用 `update_goal(status="blocked")`。只有真实外部/平台 impasse
  连续满足 blocked audit 才能标 blocked。
- 2026-08-23 20:55 对线程 `01a02d0d-6a74-75f2-a72a-a2f2736980a2` 的审计确认：
  它没有 `turn_aborted`、token budget 或 continuation deferral；在两次自动续跑仍只重复
  “打开新 Session”后，第三轮由主 Agent 显式调用 `update_goal({"status":"blocked"})`。
  因此下文 19:54 边界中“第四次 compact 后必须由新 Session 继续”的旧裁决已废止，
  不得再作为停止 Goal 的依据。主 Agent 持久配置与执行规则现为
  `gpt-5.6-sol/ultra`；`luna_worker` 保持 `gpt-5.6-luna/max`。Codex Box 配置已由
  `CODEX_HOME=~/.codex-box/Crystal/codex codex app-server --strict-config --listen stdio://`
  零退出验证。Goal objective 中历史性的 `gpt-5.6-sol/high` 文本无法通过当前 Goal API
  原地编辑，已被本规则和用户最新明确指令取代；不得仅为改这段文本重建 Goal。

## 2026-08-23 19:54 第四次 compact durable boundary（未提交 `@TIME/@ROLL/@MAP`）

- 自动 compact 摘要只触发 hard gate，不是状态 authority。本次恢复立即 interrupt 并关闭
  Mencius (`01a02e4d-df92-7be2-900a-0a233a0438ec`)、Bohr
  (`01a02e52-2e36-7951-a009-65595fdd6d7e`) 与 Ptolemy
  (`01a02e60-dd50-7082-9077-c2b893a2ba11`)；三者均已返回 completed bookkeeping 后由
  `close_agent` 关闭。Codex `thread-writer-locks` 审计只剩当前主线程
  `01a02d0d-6a74-75f2-a72a-a2f2736980a2` 与空的 coordination lock；精确 `comm` 进程审计
  退出 0 且无输出，确认没有 `go` 或 `crystal-server` 进程。恢复后未继续实现、格式化或测试。
- Mencius 与 Ptolemy 均报告未创建、修改或删除文件，只运行退出 0 的只读 Git/`rg`/`sed`/
  `cat`/`wc` 勘察；两者均未运行 gofmt、测试或构建。Mencius 的未裁决 finding 是当前 culture
  数据令 en-US 与 tr-TR 的 PercentPositivePattern 都为 0，但所需 oracle 分别为 `500%` 与
  `%500`；Ptolemy 的未裁决 finding 是根级 formatter 原型不能被命令包安全 import，且
  production `@TIME` 尚未接入 `CurrentCulture`。这些只是 review 输入，不是已验收结论。
- Bohr 的独占范围仍为 `internal/config/config.go`、`internal/config/culture.go` 与
  `internal/config/culture_test.go`。它在先前 gofmt、focused config test 和
  `git diff --check` 均退出 0 后，又应用了 en-SG preset、percent pattern、Turkish casing 与
  time-format refinement；最新两个 patch 均退出 0，但 hard gate 前来不及重新 gofmt 或测试。
  因此当前三文件版本没有可信 formatter/compile/focused 退出码，不得沿用较早通过结果。
- 本次首个恢复读取在 Legacy `workdir` 末尾追加绝对 Go `find`，整次输出已按 C01 作废；随后
  按单仓边界完整重读 Legacy `agents.md`、`tasks/lessons.md`、`tasks/goal-task.md` 和本 handoff，
  并在独立 Go 调用中完整分块重读 3223 行 `docs/migration-matrix.md`。Matrix 601–900 与
  601–700 的首次读取均因输出截断而整体作废，随后缩小到完整可见区段重读；没有采用混仓或
  截断输出。相关 archive 仅以 `utility command`、`culture`、`composite format`、
  `string.Format`、`process audit`、`compact`、`handoff` 检索并只读三个匹配 patch-workflow
  小节，没有 wholesale 读取 archive。
- 原 Goal 保持 active；恢复时 Goal 工具报告累计 `6,682,086` tokens。本线程已发生第四次
  compact，因此本节完成了 durable checkpoint。按 20:55 continuity 修正，这些数值不构成
  blocker；handoff 核验后应由同一 Goal 的下一自动 continuation 恢复实现与测试，不得重建
  Goal，也不得因 rollover 阈值调用 `update_goal(status="blocked")`。
- Legacy 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)。本节写入后完整状态仍仅为
  `M tasks/lessons.md` 与 `M tasks/migration-handoff.md`；无 staged/untracked。`git diff --check`
  退出 0；tracked、staged、untracked 三类 `.cs` 审计均为空。Legacy owned boundary 仅为这
  两个文档。
- Go 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，HEAD
  `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。完整状态为六个 tracked：
  `M cmd/crystal-server/main.go`、`M cmd/crystal-server/observer.go`、
  `M cmd/crystal-server/world.go`、`M internal/config/config.go`、
  `M internal/config/localization.go`、`M internal/config/localization_test.go`；六个 untracked：
  `?? cmd/crystal-server/utility_command.go`、
  `?? cmd/crystal-server/utility_command_session_test.go`、
  `?? cmd/crystal-server/utility_command_test.go`、`?? internal/config/culture.go`、
  `?? internal/config/culture_test.go`、`?? legacy_composite_format.go`。无 staged；
  `git diff --check` 退出 0；tracked、staged、untracked 三类 `.cs` 均为空；matrix 未修改。
  上述十二文件全部属于当前未完成切片的保护边界，禁止 reset、stash、checkout、clean、删除、
  移动或覆盖，尤其不得未经主审处理根级 formatter 原型。
- 本 hard gate 内主 Agent没有运行 gofmt、compile、focused、repeated、race、full、vet、build
  或 protocol probe；Mencius/Ptolemy 也没有。Bohr 的较早 focused 结果被后续未验证 patch
  supersede，故当前版本没有功能测试退出码。当前唯一可采用的本地质量证据是两仓
  `git diff --check` 退出 0、完整 status 稳定、三类 `.cs` 为空，以及无 writer/process 残留；
  不得把本节当成功能验收。
- Matrix 仍为 P0/P8 两个 Complete、其余 11 个 In progress。本切片暂属 P1 localization、
  P3 Game-stage interactive commands、P4 map projection 与 P8 group fan-out；matrix 尚未更新，
  P8 状态不得改变。current-culture casing/format、FormatException、formatter 边界、observer、
  transcript 和全部主审门禁完成前，不得提交或宣告切片、阶段或整体 Goal 完成。
- 下一恢复点：同一 Goal 的下一自动 continuation 先在 Legacy 根完整回读启动四文件与本节，并核对 HEAD/status/三类
  `.cs`；收到零退出结果后，再以独立 Go 调用完整读取 matrix 并核对上述十二文件、HEAD、
  status/diff/三类 `.cs`。随后只以 `utility command`、`TIME`、`culture`、
  `composite format`、`string.Format`、`localization`、`process audit` 检索相关 archive；主
  Agent review 原八文件、三个 culture sidecar 与根级原型，先裁决 Legacy current-culture
  casing/format/FormatException 的最小等价边界，再重新划分 disjoint `luna_worker` 范围。
  完成裁决后才允许运行
  `go test ./cmd/crystal-server ./internal/config -run '^$' -count=1`；不要重复已收口的
  `@ALLOWTRADE`。

## 2026-08-23 19:22 第三次 compact durable boundary（未提交 `@TIME/@ROLL/@MAP`）

- 自动 compact 摘要只用于触发 hard gate，不是状态 authority；它声称要停止两名 worker，
  但恢复后的环境最初只暴露 Mencius
  (`01a02e4d-df92-7be2-900a-0a233a0438ec`) 一名 direct subagent。主 Agent 强制关闭该线程，
  仅恢复它收取禁止工具/写入的 memory-only 报告后再次关闭。首版 handoff 回读期间，Go
  status 从九文件继续增长为十二文件，故该草稿立即作废；对 Codex thread-writer locks 的
  独立审计定位了仍为 running 的第二名 culture worker
  (`01a02e52-2e36-7951-a009-65595fdd6d7e`)。主 Agent 同样强制关闭、仅收取 memory-only
  报告并再次关闭。最终 lock 审计只剩当前主线程
  `01a02d0d-6a74-75f2-a72a-a2f2736980a2`，精确 `comm` 进程审计退出 0，确认没有 `go` 或
  `crystal-server` 进程；两名 worker 均报告没有 descendants。
- Mencius 原获分配：删除错误的根级 `legacy_composite_format.go`，并只创建
  `cmd/crystal-server/legacy_composite_format.go` 与
  `cmd/crystal-server/legacy_composite_format_test.go`。它没有创建、修改或删除任何文件，
  没有运行 gofmt、测试或构建；只读 `cat`/`sed`/`rg`/`wc`/`ls` 与 Git 审计均报告退出 0。
  一次混仓读取和两次截断搜索已由该 worker 明确作废；其 matrix 只读到第 1800 行，不能
  作为完整 matrix 证据。根级未跟踪原型仍原样保留；不得把未执行的删除/新文件建议当成
  已有补丁，也不得在下一主审前删除、移动或接线原型。
- 第二名 culture worker 的独占范围是三个 config sidecar 文件；它修改
  `internal/config/config.go`，创建 `internal/config/culture.go` 与
  `internal/config/culture_test.go`，未修改 localization/command 文件、未删除文件或提交。
  它报告三次 `apply_patch`、三文件 `gofmt`、`git diff --check` 以及
  `go test ./internal/config -run 'CurrentCulture' -count=1` 均退出 0；一个可选 Java locale
  probe 在执行前被拒绝，无退出码和文件影响。补丁包含 en-US 默认、en/invariant、zh、
  Turkish preset、环境覆盖与 pattern validation，但尚未经主 Agent review、formatter 接线、
  broader test 或 Legacy runtime oracle 验收，不得因 focused test 退出 0 视为已集成等价。
- 本 Session 按单仓边界完整回读 Legacy `agents.md`、`tasks/lessons.md`、
  `tasks/goal-task.md`、本 handoff，以及 Go 的完整 3223 行 `docs/migration-matrix.md`。
  Matrix 501–750 行的首次读取因工具输出截断而整体作废，随后分别以 501–625 和
  626–750 完整重读；没有采用截断输出。原 Goal 保持 active；恢复时 Goal 工具报告累计
  `5,842,527` tokens，且本线程已经发生第三次 compact，超过
  `tasks/goal-task.md` 的 preferred rollover 条件，因此本边界核对后必须由新 Session
  继续，不在当前 Session 恢复实现或测试。
- Legacy 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)。本节写入后完整状态仍仅为
  `M tasks/lessons.md` 与 `M tasks/migration-handoff.md`；无 staged/untracked。tracked、
  staged、untracked 三类 `.cs` 审计均为空。Legacy owned boundary 仅为这两个文档。
- Go 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，
  HEAD `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。完整状态为六个 tracked：
  `M cmd/crystal-server/main.go`、`M cmd/crystal-server/observer.go`、
  `M cmd/crystal-server/world.go`、`M internal/config/config.go`、
  `M internal/config/localization.go`、`M internal/config/localization_test.go`；六个 untracked：
  `?? cmd/crystal-server/utility_command.go`、
  `?? cmd/crystal-server/utility_command_session_test.go`、
  `?? cmd/crystal-server/utility_command_test.go`、`?? internal/config/culture.go`、
  `?? internal/config/culture_test.go`、`?? legacy_composite_format.go`。
  无 staged；tracked、staged、untracked 三类 `.cs` 均为空；`git diff --check` 退出 0；
  `docs/migration-matrix.md` 未修改。十二个 Go 文件均属于本未完成切片的保护边界。
- 本 hard gate 内主 Agent 没有运行 gofmt、compile、focused、repeated、race、full、vet、build
  或 probe。唯一新增功能测试证据是第二名 worker 报告的 config `CurrentCulture` focused test
  退出 0；这不是主 Agent 或 broader 门禁。18:48 节记录的根级 formatter 命令仍是
  “no tests to run”的无效验收证据，Mencius 未测试；不得提升任何报告为本切片验收。
- 当前矩阵仍为 P0/P8 两个 Complete、其余 11 个 In progress。本切片暂属 P1 localization、
  P3 Game-stage interactive commands、P4 map projection 与 P8 group fan-out；matrix 尚未更新，
  P8 状态不得改变。formatter/culture、current-culture command casing、异常边界、observer、
  transcript 和全部主审门禁完成前，不得提交或宣告本切片、阶段或整体 Goal 完成。
- 下一恢复点：新 Session 先在 Legacy 根完整回读启动文档与本节，并核对 HEAD/status/三类
  `.cs`；再以独立 Go 调用完整读取 matrix，核对上述十二文件、HEAD/status/diff/三类 `.cs`。
  随后只以 `utility command`、`TIME`、`culture`、`composite format`、`string.Format`、
  `localization`、`process audit` 为关键词检索相关 lessons archive；主 Agent review 原八文件、
  三个 culture sidecar 与根级原型，先裁决 Legacy current-culture
  casing/format/FormatException 的最小等价边界，再重新划分 disjoint `luna_worker` 范围。
  完成裁决后才允许运行
  `go test ./cmd/crystal-server ./internal/config -run '^$' -count=1`。禁止 reset、stash、
  checkout、clean、删除或覆盖十二文件，也不要重复已收口的 `@ALLOWTRADE`。

## 2026-08-23 18:48 第二次 compact durable boundary（未提交 `@TIME/@ROLL/@MAP`）

- 自动 compact 摘要只触发 hard gate，不是状态 authority。本次接续立即停止实现与测试，
  将仍在运行的 Ramanujan (`01a02e29-adca-7c82-a331-94f957f2e08d`) 和 Franklin
  (`01a02e29-ebb7-78b3-ae3e-8f45e9391306`) 强制停止；为准确重建状态，仅恢复线程收取
  禁止工具/写入的 bookkeeping 报告后再次关闭。最终精确 `comm` 进程审计退出 0，确认没有
  `go` 或 `crystal-server` 进程。两个线程均已关闭，后续不得把其未完成建议当成已验收实现。
- 本 Session 已按单仓边界完整回读 `agents.md`、`tasks/lessons.md`、`tasks/goal-task.md`、本
  handoff 与 3223 行 Go matrix。首次合并读取因输出截断而整体不用；matrix 的 501–750 行
  首次也被工具截断，随后拆成 501–625 与 626–750 两段完整重读。没有采用任何截断输出。
  Goal 仍是原 active Goal；恢复时工具报告累计 `5,450,983` tokens，本线程又已发生第二次
  compact，均达到 `tasks/goal-task.md` 的 preferred rollover 条件，因此本边界核对后应由
  新 Session 继续，不在当前 Session 恢复实现。
- Legacy 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)。本节写入后完整状态仍仅为
  `M tasks/lessons.md` 与 `M tasks/migration-handoff.md`；无 staged/untracked。tracked、
  staged、untracked 三类 `.cs` 审计均为空。Legacy owned boundary 仅为这两个文档。
- Go 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，
  HEAD `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。完整状态为五个 tracked：
  `M cmd/crystal-server/main.go`、`M cmd/crystal-server/observer.go`、
  `M cmd/crystal-server/world.go`、`M internal/config/localization.go`、
  `M internal/config/localization_test.go`；四个 untracked：
  `?? cmd/crystal-server/utility_command.go`、
  `?? cmd/crystal-server/utility_command_session_test.go`、
  `?? cmd/crystal-server/utility_command_test.go`、`?? legacy_composite_format.go`。无 staged；
  tracked、staged、untracked 三类 `.cs` 均为空。`docs/migration-matrix.md` 未修改；恢复时
  `git diff --check` 对 tracked diff 退出 0。九个 Go 文件全部属于本未完成切片的保护边界，
  但根级 formatter 原型处于隔离状态，不得直接集成。
- 原八文件草稿仍是 18:24 节记录的 Game-stage `@TIME/@ROLL/@MAP` route、UTC utility clock、
  localization、Group/System Chat、observer forwarding、领域测试和 authenticated 四接收端
  transcript。Ramanujan 原获分配 `cmd/crystal-server/legacy_composite_format.go` 与对应 test
  的独占范围，却误在仓库根创建了 892 行 `legacy_composite_format.go`，没有创建测试、没有
  修改其他文件。该文件是过大的未审原型，culture/custom numeric 行为不完整，未与 .NET
  runtime oracle 对照，并存在巨大 alignment 分配风险；不得按现状移动或接线，也不得因
  路径错误而 reset/clean/delete，须由下一主 Agent review 后裁决。
- Ramanujan 报告：创建补丁退出 0；组合命令
  `gofmt -w legacy_composite_format.go; go test ./cmd/crystal-server -run '^LegacyComposite$' -count=1`
  最终退出 0，但测试明确显示 `[no tests to run]`，且命令包测试没有编译根级原型，所以不是
  formatter 验收证据；`command -v dotnet` 包装调用退出 0 但无输出，未运行 .NET oracle。
  Franklin 只读 Legacy、未改文件、未格式化、未测试或构建；其一次被 `|| true` 掩盖的空
  glob 输出已丢弃并零错误重跑。主 Agent 在本 hard gate 内没有运行 gofmt、compile、focused、
  repeated、race、full、vet、build 或 probe；因此除上述无测试用例的 worker 命令外没有新的
  功能测试退出码。
- Franklin 的未集成 ruling 证据：Game-stage 命令先去掉一个 `@`，只按 U+0020 分隔并
  `RemoveEmptyEntries`，随后使用 current-culture `ToUpper()`；因此 Turkish culture 下小写
  `@time` 可变成 `TİME` 而不匹配，`ROLL/MAP` 不受该 I 规则影响。`@TIME` 先把 UTC-anchored
  `Envir.Now` 格式化为 current-culture `hh:mm tt` string；`@ROLL` formatter 参数是 Name
  string 与 boxed int；`@MAP` 是 Title/FileName strings。localization 使用无 provider、无
  catch 的 `string.Format`；有效 JSON 中的 malformed composite format 会在命令时抛出并沿
  WorkLoop catch 关闭连接/环境。Legacy 没有显式设置 CurrentCulture，Settings.Language 只选
  JSON；实际 AM/PM designator 仍由宿主 current culture 决定。当前 English/Chinese 三个模板
  均合法，未组队 `@ROLL` 仍在 group null/empty 检查前消费随机数。
- 当前矩阵仍是 P0/P8 两个 Complete、其余 11 个 In progress。本切片暂属 P1 localization、
  P3 Game-stage interactive commands、P4 map projection 与 P8 group fan-out；matrix 尚未更新，
  P8 状态不得改变。formatter/culture、current-culture command casing、异常边界、observer、
  transcript 和全部主审门禁完成前，不得提交或宣告本切片/阶段/整体 Goal 完成。
- 下一恢复点：新 Session 先在 Legacy 根回读本节并核对 HEAD/status/三类 `.cs`；再以独立
  Go 调用核对上述九文件、HEAD/status/三类 `.cs` 与完整 matrix。随后用 `utility command`、
  `TIME`、`culture`、`composite format`、`string.Format`、`localization`、`process audit`
  关键词只检索相关 lessons archive。主 Agent先 review 八文件与根级原型，保留原补丁但不要
  接线原型；从 Legacy 当前 culture/casing/FormatException 可观察边界裁决最小实现与测试范围，
  再重新划分 disjoint `luna_worker` 写入范围。完成裁决后才允许运行
  `go test ./cmd/crystal-server ./internal/config -run '^$' -count=1`。禁止 reset、stash、
  checkout、clean、删除或覆盖九文件，也不要重复已收口的 `@ALLOWTRADE`。

## 2026-08-23 18:24 compact 后 durable boundary（未提交 `@TIME/@ROLL/@MAP`）

- 自动 compact 摘要只说明应停止两个 worker 并刷新 handoff，不是状态 authority。本次
  接续沿用原 active Goal；主 Agent 尚未继续实现或运行测试。Pascal
  (`01a02e19-e92e-72f2-be20-4dd433c20be2`) 与 Hilbert
  (`01a02e1c-13b5-71e2-a9cd-dd2af4f6f47b`) 已收到 interrupt、停止并关闭；两者均报告
  没有创建或修改任何文件、没有运行 gofmt/测试/构建。Pascal 的未完成建议是新增
  `legacy_composite_format.go` 及表驱动测试；Hilbert 的未完成建议是 culture/formatter/
  parser/test sidecar。恢复时必须重新划分互不重叠的具体写入范围，不能把建议当成已有补丁。
- 本次首次恢复调用又在 Legacy `workdir` 中直接读取绝对 Go matrix；该调用虽退出 0，仍因
  混仓按 C01 整体作废。随后 `agents.md`、`tasks/lessons.md`、`tasks/goal-task.md`、本 handoff
  与完整 3223 行 Go matrix 均按两个仓库独立调用分块重读；matrix 中一段首次因输出截断，
  又缩小区间重读到完整可见。C01 已再次强化；作废调用的任何内容未用于本边界或实现裁决。
- Legacy 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)。本节写入后完整状态仍仅为
  `M tasks/lessons.md` 与 `M tasks/migration-handoff.md`；无 staged/untracked。tracked、
  staged、untracked 三类 `.cs` 审计均为空。Legacy owned boundary 仅为这两个文档。
- Go 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，
  HEAD `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。完整状态为五个 tracked：
  `M cmd/crystal-server/main.go`、`M cmd/crystal-server/observer.go`、
  `M cmd/crystal-server/world.go`、`M internal/config/localization.go`、
  `M internal/config/localization_test.go`；三个 untracked：
  `?? cmd/crystal-server/utility_command.go`、
  `?? cmd/crystal-server/utility_command_session_test.go`、
  `?? cmd/crystal-server/utility_command_test.go`。无 staged；tracked、staged、untracked
  三类 `.cs` 均为空。Go owned boundary 就是上述八文件，`docs/migration-matrix.md` 未修改。
- 未提交补丁现状：Game-stage chat route 用仅按 U+0020 分隔的命令 tokenizer 接入大小写
  不敏感的 `@TIME/@ROLL/@MAP`；world 增加启动时钟和可注入 utility clock；localization
  增加 `TheTimeIs`、`HasRolledNumber`、`YouAreInMapId` defaults/overlay；`@ROLL` 在 group
  判定前消费 `combatRollLocked(5)` 并按当前成员顺序发送跨地图 Group Chat；`@MAP` 使用当前
  `MapInfo.Title/FileName`；System/Group Chat 现可转发给目标 observer；领域与 authenticated
  四接收端 transcript 草稿已存在。当前 formatter 仍只支持简单 `{n}`/brace escape，
  `@TIME` 仍固定 UTC English `03:04 PM`；此前只读 review 指出的 .NET current-culture
  `hh:mm tt` 与完整 `string.Format` 等价风险尚未裁决或修复。
- 当前矩阵仍为 P0/P8 两个 Complete、其余 11 个 In progress。本切片暂属 P1 localization、
  P3 Game-stage interactive commands、P4 map projection 与 P8 group fan-out；在 formatter/
  culture、observer、stage/tokenization 和 transcript 全部主审及验收前，不更新 matrix、
  不改变 P8 状态，也不宣告本切片或整体 Goal 完成。
- 本次 hard gate 没有新的测试退出码。Pascal/Hilbert 均未测试；主 Agent 自 compact 后也未
  运行 gofmt、compile、focused、race、full、vet、build 或 probe。较早 Arendt 报告的
  focused 普通/race 各 `-count=1` 退出 0 仍只是 subagent 历史报告，不是当前主审门禁。
- 下一恢复命令：先在 Legacy 根回读本节并核对 status/HEAD/三类 `.cs`；再用独立 Go 调用
  核对八文件 status/diff/HEAD/三类 `.cs`。确认一致后，以 `utility command`、`TIME`、
  `culture`、`composite format`、`string.Format`、`localization` 为关键词只检索相关 lessons
  archive，再分别回读 Legacy culture/format 调用链和 Go 现有声明；裁决并实现 formatter/
  culture 修正后，才运行 `go test ./cmd/crystal-server ./internal/config -run '^$' -count=1`。
  不得 reset、stash、checkout、clean、覆盖八文件或重复已收口的 `@ALLOWTRADE` 批次。

## 2026-08-23 17:30 新 Session durable boundary 复核（未提交 `@TIME/@ROLL/@MAP`）

- 新 Session 沿用原 active Goal。首次启动调用在 Legacy `workdir` 的文档读取末尾附加了
  绝对 Go 仓 `find`，整次输出已按 C01 作废；随后以独立调用完整回读 Legacy
  `agents.md`、`tasks/lessons.md`、`tasks/goal-task.md` 和本 handoff，并开始在 Go 根分块
  回读完整 migration matrix。完成该读取和本边界回读前不进行本地实现或测试。
- Legacy 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)。完整状态仅为
  `M tasks/lessons.md` 与 `M tasks/migration-handoff.md`；无 staged/untracked，三类 `.cs`
  均为空。
- Go 根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，
  HEAD `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。完整状态为五个 tracked：
  `M cmd/crystal-server/main.go`、`M cmd/crystal-server/observer.go`、
  `M cmd/crystal-server/world.go`、`M internal/config/localization.go`、
  `M internal/config/localization_test.go`；三个 untracked：
  `?? cmd/crystal-server/utility_command.go`、
  `?? cmd/crystal-server/utility_command_session_test.go`、
  `?? cmd/crystal-server/utility_command_test.go`。无 staged，三类 `.cs` 均为空。
- 当前 Go owned boundary 已从旧 handoff 的六文件准确扩展为上述八文件；matrix 尚未修改。
  `observer.go` 是 production observer-forwarding 接线，session test 是 Arendt
  (`01a02dd9-4463-7b61-9b9c-daacdfde0ace`，`luna_worker`）独占写入。该 agent 报告
  authenticated multi-session transcript 已完成，focused 普通与 race 各 `-count=1`
  退出 0；这是 subagent 报告，主 Agent 尚未复跑。其先前 timeout 根因为测试在目标进入后
  才设置 `AllowObserve`，现已在开 session 前设置。线程已停止并关闭。
- Erdos (`01a02dd9-a4ab-7311-a62e-fb2d66dc76f2`，`luna_worker`) 完成八文件只读 review，
  未改文件、未格式化、未测试并已关闭。其两项 medium finding 是 `@TIME` 当前硬编码
  English AM/PM，未证明等价于 Legacy current-culture `hh:mm tt`；localized formatter
  仅替换简单 `{n}`，不等价于完整 `.NET string.Format`。routing/stage、权限、ROLL RNG/
  group 顺序、observer forwarding、locking 与 persistence 未发现其他问题。
- 当前可信测试证据仅限上述 Arendt 报告的 focused 普通/race 退出 0；主 Agent 在本恢复
  hard gate 后尚未运行 `gofmt`、compile、unit、race、full、vet、build 或 probe。继续前须
  完整读完 matrix 与相关 archive，回读本节并重新核对双仓 status/HEAD/三类 `.cs`；然后
  才可主审 `Envir.Now`/culture、formatter、observer 和 transcript，运行最小编译门禁。
- 下一恢复命令：在 Legacy 根回读本节并核对 status/HEAD/三类 `.cs`；在独立 Go 调用中
  完整分块读完 `docs/migration-matrix.md`，再核对上述八文件 status/diff 与三类 `.cs`。
  不得 reset、stash、checkout、clean、覆盖八文件或重复已收口的 `@ALLOWTRADE` 批次。

## 2026-08-23 compact 后恢复检查点（未提交 `@TIME/@ROLL/@MAP`）

- 收到 compaction/rollover 信号后已停止实现和测试；两个遗留 subagent 均被要求立即
  停止并已关闭，当前没有运行中的 `go test`、`go vet`、`go build` 或 server 进程。
  compact 摘要只用于触发 hard gate，不作为状态 authority。旧 handoff 仍写 Go clean，
  但恢复审计实际发现 4 个 tracked 修改和 2 个 untracked Go 文件，因此在本节写入、
  回读并与两仓状态核对前没有继续实现或测试。
- Legacy 仓库根为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)。恢复审计时工作树 clean；
  本检查点写入后仅 `tasks/lessons.md` 与本 `tasks/migration-handoff.md` 为 tracked 修改，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Go 仓库根为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。完整未提交状态为：
  `M cmd/crystal-server/main.go`、`M cmd/crystal-server/world.go`、
  `M internal/config/localization.go`、`M internal/config/localization_test.go`、
  `?? cmd/crystal-server/utility_command.go`、
  `?? cmd/crystal-server/utility_command_test.go`；无 staged 文件，三类 `.cs` 均为空。
- 当前 Go owned boundary 就是上述六文件；Legacy owned boundary 仅为本 handoff 与强化
  C01 的 active lesson。`docs/migration-matrix.md` 当前未修改，也没有 session test 文件。
  暂定矩阵范围为 P1 localization、P3 Game-stage interactive commands、P4 map projection
  与 P8 group fan-out；P1/P3/P4 仍 `In progress`，P8 当前矩阵仍为 `Complete`，在本切片
  完整 review/验收前不得更新任何阶段状态或宣告整体 Goal 完成。
- 未提交补丁当前内容：Game-stage chat route 对大小写不敏感的 `TIME/ROLL/MAP` 调用
  `world.utilityCommand`；world 增加可注入 utility clock；localization 增加
  `TheTimeIs`、`HasRolledNumber`、`YouAreInMapId` English defaults/overlay；新 helper
  生成 System/Group `Server.Chat`，`ROLL` 先消费 `combatRollLocked(5)` 再检查 group，
  `MAP` 使用当前 `MapInfo.Title/FileName`；两个仅领域测试覆盖 fixed clock、map、三成员
  roll 顺序和未组队静默随机消费。该补丁仍是未验证草稿，不得视为已迁移。
- Legacy 只读 tracing 由 James
  (`01a02dbe-719f-74f0-9b80-7956e754edf1`) 完成：只有 Game stage 调用
  `Player.Chat`，命令大小写不敏感、额外参数忽略且没有 GM/TestServer 权限；`@TIME`
  使用 `Envir.Now.ToString("hh:mm tt")` 并向本人发一条 System Chat；`@ROLL` 总是先
  `Random.Next(5)+1`，未组队静默，组队时按当前 `GroupMembers` 顺序向包括本人和跨地图
  成员各发一条 Group Chat；`@MAP` 用 `CurrentMap.Info.Title` 与字符串 `FileName` 向本人
  发 System Chat。三种 Chat 均属于 Legacy observable packet，目标玩家的 observer 也应
  收到；命令本身不持久化业务状态。该 agent 未读 Go、未改文件、未运行测试。
- Newton (`01a02dc3-869e-7503-9d9f-33537abafc7f`) 只读勘察 Go helper，确认现有
  `world.combatRoll`、mail/bootstrap/peer/barrier 与 `establishRuntimeGroupForTest` 可复用；
  未编辑、未格式化、未测试。其一次错误路径 `rg` 退出 2，整次结果已作废。恢复后的
  主线程还发现 `observerPacketObservable` 当前未包含 `ServerChat`，以及草稿使用
  `time.Now().Format("03:04 PM")`；这些只是待裁决 review 风险，不是已确认修复。
- 本未提交批次没有可信测试退出码：两个 subagent 均未运行测试；hard gate 后主线程也
  未运行 `gofmt`/compile/unit/race/full/vet/build/probe。恢复时唯一非零命令是上述已作废
  的 subagent `rg`=2；主线程首次启动读取又在 Legacy 调用中混入 Go matrix，整次约
  9.8 万 token 输出按 C01 作废并已按两个 workdir 分块完整重读，命令本身未写文件。
- 继续前的 review 清单：裁决 `Envir.Now` 的 UTC-start/monotonic 语义与 culture-sensitive
  `hh:mm tt`，不能直接假定本地 wall clock/固定 English；验证 localized `string.Format`
  的 braces/format 语义；补齐 System/Group Chat 的 observer forwarding；确认 stale/missing
  group/map state 的可达行为；新增真实 authenticated multi-session `net.Pipe` transcript，
  先列出每连接接收者矩阵、并发启动全部 reader、停止 ticker、设置 KeepAlive barriers，
  并覆盖未组队 RNG、三成员跨地图 roll、普通玩家权限、mixed case/额外参数和 observer。
- 下一恢复命令：先在 Legacy 根回读本节并核对 status/HEAD/三类 `.cs`，再在 Go 根核对
  上述六文件的 status/diff、HEAD 和三类 `.cs`；确认一致后从
  `cmd/crystal-server/utility_command.go` 的时间/observer/formatter review 开始，随后才运行
  `go test ./cmd/crystal-server ./internal/config -run '^$' -count=1`。不得 reset、stash、
  checkout、clean、覆盖六文件或重跑已收口的 `@ALLOWTRADE` 批次。

## 最近完成批次（交互式 `@ALLOWTRADE`，已收口）

- compact 摘要漏掉实际 Go 补丁且旧 handoff 错写 Go clean；恢复审计发现 3 tracked +
  3 untracked ALLOWTRADE 文件后立即停止实现/测试，按两仓分别重读启动材料、完整 matrix、
  status/HEAD 和三类 `.cs`，先重建并回读 durable boundary 后才继续。本次启动时又误把 Go
  matrix 拼入 Legacy 调用，整次约 9.7 万 token 输出已作废并强化 C01。
- Legacy ruling：Game-stage `PlayerObject.Chat` 对所有 live players 开放大小写不敏感的
  `@ALLOWTRADE`，额外参数忽略；命令直接翻转持久 `CharacterInfo.AllowTrade`，只向本人发送
  一条 localized `Chat(System)`（`AllowTradeNow`/`NoLongerAllowTrade`）。该字段同时是后续
  `TradeRequest` target gate，并经现有 character save/load 保留；主审已核对 Chat parser、
  `Info.AllowTrade` property、TradeRequest gate、Language defaults 与 DB read/write 链。
- Go 实现：world lock 内原子 toggle 并生成 private System notification；Game-stage route
  同步 session identity、auth authority 与 JSON checkpoint；localization 增加两个精确 English
  fallback/overlay key。domain test 锁定 recipient/type/text、enabled/disabled gate；双玩家
  authenticated `net.Pipe` 锁定 mixed-case/额外参数、邀请/拒绝、disabled rejection、
  live/auth/JSON、logout/reload 与最终非默认 true relogin。
- `luna_worker` Lorentz (`01a02d7f-3e4c-7042-aedb-0e28c25fa211`) 只读审查 authority/
  concurrency/session fixture，未写文件或运行测试；无生产 correctness finding。其 medium
  裸 socket/done 等待与 low ticker 污染 finding 已修正为双 reader、5s 有界 shutdown 和
  bootstrap 后同步 stop ticker，线程已关闭。主审另将最终 relogin 从默认 false 改为明确
  re-enable 后的 true，避免伪通过。
- 首次 focused 普通退出 1：新增第二次 TradeRequest 在 test-only 1ms `TradeDelay` 内被合法
  静默丢弃，`trade_command_session_test.go:179` 等待 System chat 超时；失败后日志中的临时
  JSON 路径错误来自 teardown，不是首因。该非冷却测试已显式使用零延迟，修正和 prevention
  写入 `tasks/lessons-archive/migration/protocol-session-wire.md`。
- 已通过（退出码 0）：两包最小编译；focused
  `TradePermission|AllowTradeCommand|ServerLanguage` 普通首次修正、`-count=10` 与 race
  `-count=3`；全部 Trade 普通 `-count=5` 与 race `-count=3`；服务端整包；
  `go test ./... -count=1 -timeout=900s`；`go test -race ./... -count=1 -timeout=900s`；
  `go vet ./...`；`go build ./...`；`go run ./cmd/crystal-protocol-probe -mode vectors`；
  owned gofmt、`git diff --check` 与双仓三类 `.cs` 门禁。本批最终全量无排除、无失败。
- Go 仓根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，
  提交 `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)。owned 七文件为
  `cmd/crystal-server/main.go`、`trade_command.go`、`trade_command_test.go`、
  `trade_command_session_test.go`、`internal/config/localization.go`、
  `localization_test.go` 与 `docs/migration-matrix.md`；提交统计 434 insertions、19 deletions，
  提交后工作树 clean，无 staged/untracked，三类 `.cs` 为空。
- Legacy 仓根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 415]`，HEAD 为本 post-commit handoff refresh（parent
  `84668b508fc6c2f5b19894b56e8aaa23391f572d`，
  `84668b50 docs(migration): record interactive ALLOWTRADE command`）。功能文档提交只包含
  `tasks/lessons.md`、`tasks/lessons-archive/migration/protocol-session-wire.md` 与本 handoff；
  提交后工作树 clean。本 refresh 仅更新本 handoff；提交后无 staged/untracked，三类 `.cs`
  为空。
- matrix 已更新 P1 localization、P3 character preference/interactive command 与 P6 Trade
  admission；三个阶段继续 `In progress`，整体 Goal 继续 active。下一恢复命令：新 Session
  分别核对双仓 status/HEAD/三类 `.cs` 并完整读取 matrix，从明确 pending、dependency-ready
  子切片重新选择；不要重复 `@ALLOWTRADE`，也不得把 P1/P3/P6 或整体 Goal 标为完成。

## 最近完成批次（交互式 `@HAIR`，已收口）

- 本批只迁移 P3 交互命令 `@HAIR`。Legacy ruling：权限为
  `IsGM || Settings.TestServer`，只改本人；无参数使用 `Random.Next(0, 9)`；有参数仅按
  `byte.TryParse(parts[1])` 解析首项，接受前导 `+`，invalid/negative/>255 失败时 out 值为
  0，额外参数忽略；命令完全静默，不广播 appearance，Hair 随 `CharacterInfo` 持久化。
- Go 实现已接入 Game-stage case-insensitive chat route、world-lock 内 live Hair 更新、auth
  identity authority 与 JSON save。domain tests 覆盖管理员/TestServer/未授权、`+8`、255、
  负数、overflow、非法值、额外参数和 `[0,9)` 随机 bound；authenticated `net.Pipe`
  覆盖管理员、TestServer、未授权、静默 KeepAlive barrier、stored restore/final GameMaster
  AddBuff tail、logout、真实 JSON reload 与 relogin Hair 投影。
- 主审在 focused test 中发现初稿用 `strconv.ParseUint` 会把 Legacy 接受的 `+8` 错置为
  0，已改为 `ParseInt(..., 16)` 后显式约束 `0..255`；该失败、此前管理员 final AddBuff
  未消费的 fixture 失败及 prevention 已写入
  `tasks/lessons-archive/migration/protocol-session-wire.md`。
- `luna_worker` Rawls (`01a02d4f-f2d3-7493-9c50-d21640b84a48`) 只读审查四个 Go 文件，
  无生产 correctness finding；其两个低严重度测试 finding（TestServer 初始/期望均为零不能
  证明权限分支、relogin 未 drain 第二个 AddBuff/final barrier）均已修正并复跑，线程已关闭。
- 已通过（退出码 0）：`go test ./cmd/crystal-server -run '^$' -count=1`；focused 普通
  `-run 'HairCommand|InteractiveHair' -count=10` 与 race `-count=3`；排除下述已知 OmaMage
  单测后的服务端整包及 `go test ./... -count=1`；无排除 `go test -race ./... -count=1`；
  `go vet ./...`；`go build ./...`；`go run ./cmd/crystal-protocol-probe -mode vectors`；
  owned gofmt、`git diff --check` 与双仓三类 `.cs` 门禁。
- 无排除服务端整包与 `go test ./... -count=1` 均退出 1，唯一失败为 handoff 已记录的既有
  `TestSessionOmaMageRangeSlowFrozenTranscript` 随机 bound `[2 1]` vs `[1]`；隔离
  `-count=10` 同样退出 1，栈未进入本批文件；未修改无关 OmaMage 代码掩盖失败。排除该
  单测的服务端整包和全仓普通均退出 0，完整 race 无排除退出 0。
- Go 仓根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，
  提交 `cdd497656925f1b524841a8aab1acdda5362c14b`
  (`cdd4976 feat(p3): restore interactive HAIR command`)；提交包含五个本批文件、205
  insertions、1 deletion，提交后工作树 clean，无 staged/untracked，三类 `.cs` 为空。
  矩阵仅更新 P3，阶段仍 `In progress`。
- Legacy 仓根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 413]`，文档提交
  `c481fd9dcfae778fef0141296d27f7b1a4ad47ed`
  (`c481fd9d docs(migration): record interactive HAIR command`) 只包含本 handoff 与
  `tasks/lessons-archive/migration/protocol-session-wire.md`。提交后工作树 clean，无
  staged/untracked，三类 `.cs` 为空；本段 post-commit 刷新后仅本 handoff 为 tracked 修改。
- 下一恢复命令：新 Session 分别核对双仓 status/HEAD/三类 `.cs` 并读取 Go matrix，再从
  明确 pending、dependency-ready 子切片选择。不要重复 `@HAIR`，也不要把 P3 或整体
  Goal 标为完成。

## 最近完成批次（交互式 `@OBSERVER`，已收口）

本批从 compact 后发现的 5 tracked + 2 untracked 未验证补丁恢复；主 Agent 重新沿
Legacy `PlayerObject.Chat -> MapObject.Observer setter -> HumanObject.UpdateGMBuff`
调用链裁决，修正行为后补齐领域、协议、可见性、持久化与 authenticated session 证据。
ranking closure 和其他管理员命令未混入本原子切片。

- Legacy 仓库根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master...origin/master [ahead 412]`，HEAD
  `49e8504f9002c53e567869f156668c421e6c9ad5`
  (`49e8504f docs(migration): record interactive Observer mode`)；功能文档提交包含
  active lessons、protocol-session-wire archive 与本 handoff。提交后工作树 clean；本段
  post-commit 刷新后只有本 handoff 为 tracked 修改，无 staged/untracked，三类 `.cs` 为空。
- Go 仓库根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，提交 `a79e25c` (`feat(p3): restore interactive Observer mode`)；提交包含
  17 个文件、720 insertions、90 deletions，提交后工作树 clean，无 staged/untracked，
  tracked/staged/untracked `.cs` 均为空。
- Legacy ruling：命令只允许 `IsGM`，TestServer 普通玩家仍被拒绝；命令大小写不敏感。
  `Observer` 属性 setter 在 Hint 之前同步执行：启用先向附近广播 `ObjectRemove`，禁用先按
  viewer 生成 `ObjectPlayer`；随后 owner 收 localized `ObserverMode`/`NormalMode` Hint，
  `UpdateGMBuff` 再从 live `GMGameMaster`/`GMNeverDie`/`Observer` 三项重建 options，按
  owner AddBuff -> 可见 nearby AddBuff -> persistence 更新。Observer 令对象不可见且
  `Blocking=false`，但不会带来通用 player/monster attack-target immunity。
- Go 实现：新增 localized ObserverMode、Game-stage admin chat route 与 transient
  `worldPlayer.ObserverMode`；collision 排除 Observer，并在登录 bootstrap、observer passive、
  map transition、NPC/relationship/forced teleport、revive、SlashingBurst、Guild/Conquest
  appearance refresh 中抑制 Observer 的后续 `ObjectPlayer` materialization。GameMaster、
  Superman、Observer 三个命令统一从 live runtime flags 重建 option byte，登录显式重置
  Observer/Superman runtime；durable Observer option 仍先 restore，再由 final projection 清零。
- 主审纠正了未验证补丁的两项错误：原补丁把 Hint 放在 setter visibility 之前，并错误把
  Observer 加入通用玩家/怪物 target gate；两项均按当前 Legacy 源码修正。runtime options
  统一后，既有 GameMaster/Superman fixture 因只 seed durable bits 出现期望 7、实际 1/5；
  fixture 改为同时 seed live flags 后通过，教训已写入 protocol-session-wire archive。
- 测试覆盖：world/config tests 锁定 permission、localized fallback/overlay、
  `ObjectRemove/ObjectPlayer -> Hint -> AddBuff -> persist`、non-blocking 但仍 targetable、
  runtime option composition 与后续投影抑制；authenticated `net.Pipe` 锁定管理员
  enable/disable、TestServer-only 拒绝、join-time invisibility、logout persistence 与 relogin
  restore/final reset。
- 已通过（退出码 0）：两包最小编译；config Observer 普通 `-count=10`；world Observer
  普通 `-count=10`；interactive Observer session 普通 `-count=10` 与 race `-count=3`；
  GameMaster/Superman/Observer 组合普通 `-count=5`；相关 Guild/Conquest/HornedMage/
  Reincarnation/SlashingBurst/TurtleKing 定向集合；服务端整包；`go test ./... -count=1
  -timeout=900s`；`go test -race ./... -count=1 -timeout=900s`；`go vet ./...`；
  `go build ./...`；`go run ./cmd/crystal-protocol-probe -mode vectors`；owned gofmt、
  `git diff --check` 与两仓三类 `.cs` 门禁。本批没有排除项或未解决失败。
- Subagent：Tesla (`01a02d25-25ce-7232-a31f-72b3c8be6260`) 在 compact gate 后停止，未改
  文件；Hubble (`01a02d2c-495a-73d1-b8bf-fba64a1e6c4d`) 独占新增 authenticated session
  test 文件并通过 focused 普通/race；Singer 只读 review 线程长时间无结果后关闭，未写文件。
- 矩阵范围：P1/P3/P4/P5 已记录本切片，四阶段继续 `In progress`；整体 Goal 继续 active。
- 当前没有未提交的 Go 实现、测试或 matrix 工作；Legacy 仅本 post-commit handoff 刷新
  未提交。下一恢复命令：分别核对双仓 status/HEAD/三类 `.cs` 门禁并读取 Go matrix，
  再从 ranking closure 或其他明确 pending、dependency-ready 子切片选择；不得重复
  `@OBSERVER`，也不得把本批或整体 Goal 误标为全部完成。

## 2026-08-23 compact 后恢复检查点

- 本线程 Goal 累计用量在重新读取两仓启动材料并核验外部状态后达到
  `8,014,337` tokens，已越过 `tasks/goal-task.md` 规定的约 800 万 hard ceiling；因此
  立即停止选批、Legacy tracing、实现和测试，并触发 new-session rollover。本轮没有
  选择 `@OBSERVER`、ranking closure 或其他新切片，也没有产生 Go 实现改动。
- 当前会话的 `spawn_agent` schema 仍未暴露 `luna_worker` 或 `gpt-5.6-luna`，故未用
  其他模型静默替代；新 Session 必须重新发现工具能力，可用时优先按
  `gpt-5.6-luna/max` 委派边界明确的工作。

- 收到 context compaction 信号后已停止选批、委派、实现与测试；本检查点没有选择或
  启动新的迁移矩阵行，也没有恢复此前已收口的 `@SUPERMAN` 批次。
- Legacy 仓库根为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，HEAD `68f5e05585d841ab69b1d194bfb26e0b42fd4e41`
  (`68f5e055 docs(migration): refresh Superman handoff`)。刷新前工作树 clean；刷新后
  tracked 修改为本 `tasks/migration-handoff.md`，以及记录 Codex custom-agent 验证误区的
  `tasks/lessons.md`；无 staged/untracked 文件，三类 `.cs` 门禁均为空。
- Go 仓库根为 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `901e6213df8240ba498eb1e00529cf72857a9de2`
  (`901e621 feat(p3): restore interactive Superman mode`)；工作树 clean，无
  staged/untracked 文件；三类 `.cs` 门禁均为空。
- 当前 owned migration docs 为本 handoff 与上述 active lesson；没有 Go/C# 实现文件、
  矩阵文件或测试文件处于本批所有权范围。当前没有未提交的迁移实现；两份文档刷新
  尚未提交。
- 本恢复检查点未运行迁移测试，故没有新的测试退出码或失败归因；最近完成批次的测试
  证据仍以 `@SUPERMAN` 收口段为准。
- 本轮先完成用户明确要求的 `$CODEX_HOME/agents/luna-worker.toml` 本地 Codex 配置复制
  与验证；该路径位于两仓之外，不属于迁移批次，也不改变整体迁移目标或矩阵状态。
- 下一恢复命令：分别重新核对两仓 `git status --short --branch`、HEAD 与三类 `.cs`
  门禁，读取 Go `docs/migration-matrix.md` 后再选择一个 dependency-ready 的 pending
  子切片；不得在未选定矩阵行前开始实现或重复已收口的 `@SUPERMAN`。

## 最近完成批次（交互式 `@SUPERMAN`，已收口）

本批从 compact 后重建的 34 文件未验证补丁恢复，先由两个边界明确的
`luna_worker` 完成只读 Legacy tracing 与 Go review，再由主 Agent 按 Legacy 调用链裁决、
修正共享 vital 边界并完成全部门禁。`@OBSERVER`、ranking closure 和其他管理员命令未
混入本原子切片。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，功能文档提交 `cc848b9824020b1dec9206f3aeda632c09509bc0`
  (`cc848b98 docs(migration): record interactive Superman mode`)；该提交包含
  `tasks/lessons.md`、`tasks/lessons-archive/migration/protocol-session-wire.md` 与本 handoff。
  提交后工作树 clean，无 staged/untracked；tracked/staged/untracked `.cs` 均为空，
  `git diff --check` 退出 0。本段 post-commit 刷新将作为独立 handoff 提交。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `901e6213df8240ba498eb1e00529cf72857a9de2`
  (`901e621 feat(p3): restore interactive Superman mode`)；提交包含 35 个文件：
  `cmd/crystal-server/conquest_archers.go`、`counter_attack.go`、
  `counter_attack_test.go`、`curse_test.go`、`dark_oma_king.go`、`earth_golem.go`、
  `energy_shield.go`、`energy_shield_test.go`、`flying_statue.go`、`game_master.go`、
  `game_master_session_test.go`、`game_master_test.go`、`general_meow_meow.go`、
  `healing_circle.go`、`hell_lord.go`、`hiding_buffs_test.go`、`horned_commander.go`、
  `main.go`、`manectric_blest.go`、`mass_healing_test.go`、`plague.go`、`poison.go`、
  `reincarnation.go`、`scaly_beast.go`、`soul_fireball_test.go`、
  `special_item_modes_test.go`、`stone_golem.go`、`support_buffs_test.go`、
  `tucson_general.go`、`ultimate_enhancer_test.go`、`warrior_attack.go`、`world.go`、
  `internal/config/localization.go`、`localization_test.go` 与
  `docs/migration-matrix.md`。提交后工作树 clean，无 staged/untracked；
  tracked/staged/untracked `.cs` 均为空。提交统计为 749 insertions、179 deletions。
- Legacy ruling：`@SUPERMAN` 只在 Game stage 可达，命令大小写不敏感且忽略额外参数；
  permission 为 `IsGM || Settings.TestServer`。合法调用严格执行
  `GMNeverDie toggle -> localized InvincibleMode/NormalMode Hint -> UpdateGMBuff`；普通
  TestServer 玩家只有瞬态 flag 与 Hint，管理员才把 GameMaster/Observer/Superman 的
  1/2/4 bits 合并为 Buff type 100，按 owner→可见 nearby→persistence 更新。Buff 可持久，
  runtime `GMNeverDie` 不持久；重登先 restore stored Buff，再按 false runtime final reset。
  `HumanObject.ChangeHP` 先执行 Protection→`ChangeMP`，随后两种 vital 的非零请求均在
  上/下限处理后由 GMNeverDie 强制回满；零 MP 负扣仍发 HealthChanged，regen indicator
  仍显示请求恢复量，整数运算保持 .NET 默认 unchecked 怪癖。
- Go 实现：新增 `worldPlayer.SupermanMode`、localized `InvincibleMode`、生产聊天路由、
  option-preserving administrator Buff projection 与 TestServer transient-only 分支；共享
  `ChangeHP`/`ChangeMP`/`SetHP` helper 覆盖 Protection、over-cap/zero notification、
  Superman refill，并接入 NPC vital、item recovery、mana cost、普通/特殊玩家伤害、poison、
  healing、Vampirism、EnergyShield 与 Reincarnation。主审发现 helper 初稿未自行执行
  Protection redirect，已在 `world.go` 修正并把 world test 改为直接验证共享入口。
- 测试覆盖：world tests 锁定未授权、TestServer-only、管理员其他 option 保留、
  Hint/owner/nearby/persist 顺序、Protection、同终值通知、lethal monster/poison 与共享
  clamp 怪癖；authenticated `net.Pipe` 锁定 localized enable/disable、AddBuff tail、HP/MP
  包、logout durable option、relogin restore/final reset 和 TestServer 无持久状态。
- 已通过（退出码 0）：两包最小只编译；config `ServerLanguage` 普通 `-count=10`；server
  `Superman|LegacyVitalChange` 普通 `-count=10` 与 race `-count=3`；服务端整包；最终
  `go test ./... -count=1 -timeout=900s`；最终 `go test -race ./... -count=1
  -timeout=900s`；`go vet ./...`；`go build ./...`；
  `go run ./cmd/crystal-protocol-probe -mode vectors`；owned-file gofmt、
  `git diff --check` 与两仓三类 `.cs` 门禁。
- 失败归因：首次全仓普通退出 1，唯一失败为既有
  `TestSessionYinDevilNodeTranscript/42` 的通知 `[]` vs `[84]`；隔离 `-count=20` 与随后
  服务端整包、全仓普通均退出 0。首次完整 race 退出 1，唯一失败为 handoff 已记录的
  `TestSessionOmaMageRangeSlowFrozenTranscript` 随机 bound `[2 1]` vs `[1]`；隔离普通可过、
  race 可复现，随后未改无关文件的完整 race 重跑退出 0。两者栈均未进入本批文件。
- Review 裁决：Go 只读 review 报告的 `int32` overflow、near-full healing indicator 与
  Plague zero-MP notification 均经主 Agent 回读 Legacy 后确认为原版行为，未按常规直觉
  “修正”；唯一采纳并修复的是共享 ChangeHP 缺失 Protection redirect。
- Subagent：`01a02be7-cab3-7f00-916f-706bbc3de6a7`（Faraday，`luna_worker`）只读追踪
  Legacy `@SUPERMAN`/GMNeverDie/UpdateGMBuff/ChangeHP/ChangeMP；
  `01a02be8-fe07-70c0-9fdf-8617a6c21e30`（Noether，`luna_worker`）只读审查 Go 34 文件
  vital 写入口与 persistence/notification 风险。两个线程均未写文件、未测试、未提交，
  结果已由主 Agent review 后关闭。
- 矩阵范围：P1/P3/P5 已记录本切片，三阶段继续 `In progress`；整体 Goal 继续 active。
- 下一恢复命令：新 Session 分别核对两仓 clean status/HEAD 与三类 `.cs` 门禁，完整读取
  Go 矩阵后从 `@OBSERVER`、ranking closure 或其他 dependency-ready 子切片选择一个，
  不得重复 `@SUPERMAN`。

### 本批提交边界

- Go 原子提交 `901e621` 只包含上述 35 个文件；Legacy 文档提交 `cc848b98` 只包含
  active lessons、protocol-session-wire archive 与本 handoff。
- 禁止 reset、stash、checkout、clean、覆盖其他工作或修改任何 `.cs`。整体 Goal 未完成。

## 最近完成批次（交互式 `@GAMEMASTER`，已收口）

本批从已提交的管理员 startup 边界继续，只迁移 Legacy `Chat("@GAMEMASTER")`
交互切换；`@SUPERMAN`、`@OBSERVER`、ranking closure 和其他管理员命令未混入本原子
提交。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，HEAD `03274059faea246c6c5adcd2a186455bd30104ce`
  (`03274059 docs(migration): refresh interactive GameMaster handoff`)；compact 后恢复时
  tracked 修改为 `tasks/lessons.md` 中本地 Codex strict-config 验证教训，以及本 handoff
  的恢复刷新，无 staged/untracked；tracked/staged/untracked `.cs` 均为空。前一份
  post-commit handoff 已由 `03274059` 提交，不再待提交。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `3e85ec4c4268bc4a24e5ec8cc0ff7a96ef58775c`
  (`3e85ec4 feat(p3): restore interactive GameMaster mode`)；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Legacy ruling：命令名大小写不敏感；runtime `IsGM` 或 `Settings.TestServer` 才能执行。
  每次先翻转瞬态 `GMGameMaster`，向本人发送本地化 `GameMasterMode` 或 `NormalMode`
  `Chat(Hint)`，再调用 `UpdateGMBuff`。普通 TestServer 玩家因 `IsGM=false` 不生成 Buff；
  管理员将 GameMaster bit 与 Observer/Superman bits 合并为一个 value，owner AddBuff 后按
  `GameMasterEffect` 可见性广播。该 mode 控制玩家与怪物 target gate，但不是管理员 authority。
- Go 实现：localization 增加 `NormalMode` English fallback/精确 overlay；生产聊天路由接入
  `@GAMEMASTER`；world lock 内完成权限、切换与 Hint，管理员保留其他 option bits，按
  `Hint -> owner AddBuff -> nearby AddBuff -> persistence` 排序；TestServer-only 分支只改变
  runtime `GameMasterMode`，不创建 Buff、不推进 durable revision。
- 测试覆盖：world tests 锁定未授权拒绝、TestServer transient-only、管理员 option 保留、
  recipient/可见性/persist 顺序；authenticated `net.Pipe` 锁定大小写命令、localized
  enable/disable Hint、精确 AddBuff tail、KeepAlive 屏障、logout persistence，以及 relogin
  时 stored GameMaster bit 先 restore、随后按新 runtime 默认值 final reset；TestServer 普通
  账户两次切换均证明没有 AddBuff 或持久化管理员状态。
- 已通过（退出码 0）：两包最小只编译；config/server focused 普通 `-count=10`；focused
  race `-count=3`；`go test ./cmd/crystal-server -count=1 -timeout=900s`；最终
  `go test ./... -count=1 -timeout=900s`；`go test -race ./... -count=1 -timeout=900s`；
  `go vet ./...`；`go build ./...`；`go run ./cmd/crystal-protocol-probe -mode vectors`；
  owned-file `gofmt -d`、`git diff --check` 与两仓三类 `.cs` 门禁。无排除项。
- 失败归因与修正：首次 focused 调用退出 1，唯一失败为新 TestServer session fixture 沿用
  `AllowStartGame=false` 默认值，普通非管理员正确收到 StartGame result 0；显式设置
  `AllowStartGame=true` 后 focused、重复、race 与全部门禁均退出 0，未修改生产 gate。
- 矩阵：P1/P3/P5 已记录本切片，三阶段继续 In progress；整体 Goal 不得标为完成。
- Subagent：`luna_worker` 已复制到当前 `$CODEX_HOME/agents`；本恢复会话将在选批前
  重新发现 `spawn_agent` schema，并只在工具实际提供 custom agent 或
  `gpt-5.6-luna/max` 时委派，不会用其他模型冒充。
- 恢复核对未运行 Go 测试；最近已完成批次的退出码 0 证据仍见下文。当前 owned 文件仅
  Legacy `tasks/lessons.md` 与本 handoff；尚未选择或开始新的 Go 实现批次。
- 下一恢复命令：继续完整读取 Go 矩阵并核对 `luna_worker` 可用性，然后只选择
  `@SUPERMAN`、`@OBSERVER` 或 ranking closure 中一个 dependency-ready 原子切片；不得
  重复 startup 或本 `@GAMEMASTER` 切片。

### 本批提交边界

- Go 原子提交 `3e85ec4` 只包含 7 个 owned 文件：
  `cmd/crystal-server/game_master.go`、`game_master_test.go`、
  `game_master_session_test.go`、`main.go`、`internal/config/localization.go`、
  `localization_test.go` 与 `docs/migration-matrix.md`。
- Legacy 文档提交 `0d0d2555` 只包含 `tasks/lessons.md`、本 handoff 与
  protocol-session-wire archive；后续 post-commit 状态刷新已由 `03274059` 单独提交。
  当前仅本地 Codex 验证 lesson 与 compact 后恢复 handoff 未提交；整体 Goal 继续 active，
  P1/P3/P5 均保持 In progress。

## 历史批次快照（管理员 GameMaster startup，已收口）

本节从 compact 后 handoff 与两仓实际状态恢复，并以当前源码、Legacy 调用链和重新运行的
完整门禁为 authority。恢复时 handoff 只记录 11 个 Go 文件，但实际 worktree 已包含
required-group 与认证 session 测试；现已逐文件 review、补齐旧 consumer 和 probe 状态机，
没有 reset、stash、checkout、clean 或覆盖任何既有工作。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，HEAD `8fd79aef3d5868109743bdd8ba065d13309064bb`，分支
  `master...origin/master [ahead 406]`；当前 tracked 修改仅
  `tasks/migration-handoff.md` 与
  `tasks/lessons-archive/migration/protocol-session-wire.md`，无 staged/untracked。
  tracked/staged/untracked `.cs` 均为空，`git diff --check` 退出 0。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `ed29535c60d8bd96663035bfb584bd8195235634`
  (`ed29535 feat(p3): restore administrator GameMaster bootstrap`)；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空，`git diff --check` 退出 0。
- Legacy ruling：`Account.AdminAccount` 在 `PlayerObject.Load` 中建立 runtime `IsGM`；管理员
  绕过 `AllowStartGame=false` 及 RequiredGroup 登录/移动/传送/持续 enforcement。
  `UpdateGMBuff` 将 `GMGameMaster`/`Observer`/`GMNeverDie` 编为 1/2/4 bits，并通过
  `AddBuff(GameMaster, duration=0, values: byte)` 发送 infinite Buff type 100；可见性来自
  `[Optional] GameMasterEffect`。fresh admin 在 StartGame 尾部发送一次，stored admin 先
  restore 再 final update；TestServer admin 在两条 Hint 后 early update，再 restore 和
  final update；revoked non-admin 先 restore stale Buff，再由 `ProcessBuffs` RemoveBuff。
- Go 实现：管理员 authority 与 TestServer `GameMasterMode` 分离；支持 start/required-group
  bypass、GameMasterEffect INI/env、type/options wire 常量、pre-entry ObjectID reservation、
  owner/nearby AddBuff 和 visible ObjectPlayer snapshot、durable upsert/relogin、revocation
  removal，以及生产 probe 的 early packet和严格 zero/one/two tail KeepAlive 状态机。
  交互式 GM toggles、ranking suppression 和其他管理员命令/capability 明确仍 pending。
- 测试覆盖：固定 AddBuff payload；world upsert/字段保留/recipient/persist/order/revocation/
  ObjectID/snapshot；管理员 start gate 与 required-group bypass；认证 fresh、TestServer
  owner+nearby 三次包序、stored relogin、revoked authority；受影响 admin observer consumer；
  config 默认/INI/env/error；probe early、zero/one/two、restore+remove、malformed/type/options/
  invalid sequence 及完整 authenticated network transcript。
- 已通过（退出码 0）：四包最小只编译；server GameMaster/required-group/observer focused
  普通 `-count=10`；config/protocol/probe focused 普通 `-count=10`；四包 focused race
  `-count=3`；`go test ./cmd/crystal-server -count=1 -timeout=900s`；最终
  `go test ./... -count=1 -timeout=900s`；`go test -race ./... -count=1 -timeout=900s`；
  `go vet ./...`；`go build ./...`；`go run ./cmd/crystal-protocol-probe -mode vectors`；
  owned-file `gofmt -d`、`git diff --check` 与两仓三类 `.cs` 门禁。
- 失败归因与修正：首次服务端整包退出 1，唯一失败为 admin observer 测试未消费合法 final
  GameMaster AddBuff，已补显式消费/断言，随后 focused 与两次服务端整包退出 0。首次全仓
  普通重跑退出 1，唯一失败为既有 `TestSessionHallucinationTranscript` 30 秒 pipe timeout；
  隔离 `-count=10`、后续服务端整包和全仓普通均退出 0，栈未进入本批文件，未修改无关模块。
  Review 同时将 probe 从无限宽松 tail 收紧为 Legacy 正常路径的最多两个包与 remove 顺序。
- 矩阵：P1/P3/P4/P5 已记录本切片，四阶段继续 In progress；整体 Goal 不得标为完成。
- Subagent：恢复时关闭遗留 Hooke 线程；当前会话的 spawn schema 未热加载 `luna_worker` 且
  不提供 `gpt-5.6-luna`，因此按规则未用其他模型静默替代。主 Agent完成恢复、Legacy ruling、
  review、集成修正、门禁、matrix 与 handoff。
- 下一恢复命令：提交 Legacy 两个 Markdown 后，重新核对两仓 clean status/HEAD 与三类
  `.cs` 门禁。新 Session 从矩阵选择下一项，优先继续交互式 GM toggles、ranking
  suppression 或其他明确 dependency-ready 的管理员差集，不得重复本 startup 切片。

### 本批提交边界

- Go 原子提交 `ed29535` 只包含上述 16 个 owned 文件；提交后工作树 clean。
- Legacy 文档提交只允许本 handoff 与 protocol-session-wire archive；整体 Goal 继续 active，
  P1/P3/P4/P5 均保持 In progress。

## 历史批次快照（TestServer StartGame 已收口）

本批从 compact 硬门的“仅选定”边界恢复，完成 Legacy 可达性追踪后把两种 GameMaster
authority 拆开：本原子切片只迁移 TestServer 普通账户的两条 Hint 与瞬态
`GMGameMaster` 攻击免疫；管理员账户 `IsGM`、GameMaster Buff/options 与交互式命令留待
独立批次。Go 功能提交 `375fa60 feat(p3): restore test server bootstrap` 已收口。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，本批文档提交 parent `d471d1b3dd8648ef343442764d215037595f95eb`
  (`d471d1b docs(migration): record localized welcome bootstrap`)；owned 文档为
  `tasks/lessons.md`、`tasks/lessons-archive/migration/protocol-session-wire.md` 与本 handoff。
  提交前 tracked/staged/untracked `.cs` 均为空。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `375fa60 feat(p3): restore test server bootstrap`；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Legacy 权威链：`StartGameSuccess` 在 `StartGame(Result=4) -> localized Welcome` 后，若
  `Settings.TestServer` 为 true，依次调用 `ReceiveChat(GameIsTestMode, Hint)` 与
  `Chat("@GAMEMASTER")`。后者从默认 false 切换 `GMGameMaster=true`、发送
  `GameMasterMode` Hint；该 flag 只在两种 `PlayerObject.IsAttackTarget` 重载中拒绝
  Human/Monster 攻击。非管理员 TestServer 玩家不会获得 `IsGM`、GM Buff 或管理权限，
  flag 也不写入 CharacterInfo。
- Go 实现：扩展 localization 的 `GameIsTestMode`/`GameMasterMode` English fallback 与
  case-sensitive overlay；成功 StartGame 在 welcome 后按固定顺序发送两条条件 Hint；
  `worldPlayer.GameMasterMode` 仅由本次 runtime entry seed，player-vs-player 与通用 monster
  target gate 均拒绝该目标。生产 probe 允许零条或完整两条 TestServer Hint，拒绝单条，
  且仍要求它们位于 Notice/item/map 之前。
- Go owned 文件共 11 个：`cmd/crystal-server/main.go`、`main_test.go`、
  `monster_ai.go`、`welcome_bootstrap.go`、`world.go`、
  `test_server_bootstrap_test.go`；`internal/config/localization.go`、
  `localization_test.go`；`internal/probe/network.go`、`network_test.go`；
  `docs/migration-matrix.md`。
- 已通过（退出码 0）：三包最小只编译；config/server/probe focused 普通
  `-count=10` 与 race `-count=3`；`go test ./cmd/crystal-server -count=1
  -timeout=900s`；`go test ./... -count=1 -timeout=900s`；`go test -race ./...
  -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；
  `go run ./cmd/crystal-protocol-probe -mode vectors`；owned-file `gofmt -d` 与
  `git diff --check`。本批没有失败测试或排除项。
- 测试证据：helper 锁定 localized payload；authenticated `net.Pipe` 锁定
  `StartGame -> Welcome -> TestMode -> GameMasterMode -> map/bootstrap`，KeepAlive barrier
  后回读 runtime flag；world tests 同时覆盖玩家与怪物 target gate 的 on/off；probe tests
  覆盖 0/2 接受与 1 拒绝。
- 矩阵：P1/P3/P5 已记录配置、FIFO、runtime target gate 与 probe 证据，三个阶段仍
  In progress。管理员 `IsGM` startup、GM Buff/options、`@GAMEMASTER` 交互切换和完整
  startup closure 明确 pending。
- Subagent：暴露的 spawn model 列表仍不提供要求的 `luna_worker`/
  `gpt-5.6-luna`，因此未静默替换；主 Agent 完成 tracing、架构拆分、实现、验证、文档和提交。
- 工作流修正：compact 恢复首调用混仓已按 C01 作废；Go 勘察的未引用 glob 与预期零匹配
  `rg` 在 `set -e` 下失败，相关调用同样全部作废并按 `rg --files`/明确 `|| true` 重跑；
  C01/C02 已强化。
- 下一恢复：分别核对两仓 clean status/HEAD 和三类 `.cs` 审计，再从 matrix 选择下一个
  dependency-ready 子切片。若继续 StartGame，优先只读追踪管理员 `IsGM` 构造、
  `UpdateGMBuff` 的精确 AddBuff 次数/顺序、GameMasterEffect 可见性、ranking/required-group
  bypass 与交互式命令，不得把本批普通 TestServer 模式误当成 admin authority。

### 当前提交边界

- Go 原子提交 `375fa60` 仅包含上述 11 个文件；提交前后均完成完整 status、diff check
  与 `.cs` 审计。
- Legacy 文档提交只包含上述三个 Markdown。整体 Goal 继续 active，P1/P3/P5 均未完成。

## 历史批次快照（localized welcome bootstrap 已收口）

本节是在 context compact 后按两仓实际 worktree 重建的 durable boundary；compact 摘要
仅提示“welcome-bootstrap 未提交批次正在写 handoff”，没有保留可信的完整 status 或测试
退出码，因此以下内容以恢复后的 `git status`、diff 和只读 Legacy 调用链复核为准。在本节
写入并回读前已停止所有实现与测试。恢复后已完成 review、修正与全部门禁；Go 原子提交
`7e10f9f feat(p3): restore localized welcome bootstrap` 与本 Legacy 文档提交均已收口。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，本批文档提交 parent 为
  `f00b27db docs(migration): record server notice bootstrap`（提交后 HEAD 以
  `git log -1 --oneline` 为准）；四个 owned 文档为 `tasks/lessons.md`、
  `tasks/lessons-archive/migration/protocol-session-wire.md`、
  `tasks/lessons-archive/verification/race-and-flake-attribution.md` 与本 handoff。提交后工作树
  clean，无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `7e10f9f feat(p3): restore localized welcome bootstrap`；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- 当前切片：P1/P3 `StartGameSuccess` localized welcome bootstrap。Legacy 在成功
  `S.StartGame(Result=4)` 后无条件调用 `ReceiveChat(Welcome(GameName), ChatType.Hint)`，该包
  位于 TestServer-only greeting/GameMaster side effect、invalid-guild repair、Notice 与
  item/map/user bootstrap 之前。`Settings` 从 `[General] Language` 选择
  `Localization/<Language>.json`；`GameLanguage.LoadServerLanguage` 以 built-in English map
  为底，只覆盖已知 key，missing file 会写默认文件，malformed JSON 被吞掉。当前 Go 切片
  只迁移客户端可见的 `GameName`/`Welcome` 子集；TestServer greeting/GameMaster 行为与其余
  localization keys 明确留待后续，不得把 P1/P3 标为 Complete。
- Go production/config/matrix owned 文件（4 个 tracked + 2 个 untracked）：
  `cmd/crystal-server/main.go`、`cmd/crystal-server/welcome_bootstrap.go`、
  `internal/config/config.go`、`internal/config/localization.go`、
  `internal/probe/network.go`、`docs/migration-matrix.md`。实现新增 `Config.Language`、显式
  localization path override、built-in `ServerText`、BOM-aware/case-sensitive JSON overlay、
  `playerWelcomeBootstrapPacket` 和 probe 的 mandatory Hint 消费，并在成功
  `ServerStartGame` 后立即发送现有 `ServerChat`/`ChatHint` payload。
- Go test owned 文件（2 个 untracked + 21 个 tracked）：
  `cmd/crystal-server/welcome_bootstrap_test.go`、`internal/config/localization_test.go`、
  `internal/probe/network_test.go`、
  `cmd/crystal-server/main_test.go`、`conquest_npc_actions_test.go`、`conquest_test.go`、
  `default_npc_session_test.go`、`equipment_session_test.go`、`guilds_session_test.go`、
  `hero_seal_session_test.go`、`intelligent_creature_visibility_test.go`、
  `mail_session_test.go`、`mounts_session_test.go`、`npc_item_session_test.go`、
  `quests_session_test.go`、`refine_logout_test.go`、`refine_session_test.go`、
  `relationships_session_test.go`、`repair_session_test.go`、`shop_session_test.go`、
  `storage_session_test.go`、`trade_session_test.go`、`use_item_session_test.go`。这些修改为
  localized helper/session 测试及既有手写 StartGame consumer 的新 server-first 包消费。
- Review 修正：初稿 Go JSON decoder 会拒绝 .NET 已去除的 UTF-8 BOM、错误接受小写根键
  `text`，且生产 network probe 没有消费新 server-first Chat；现已按 Legacy
  `File.ReadAllText`/`System.Text.Json` 语义与完整探针 state machine 修正并补测试。
- 已通过（退出码 0）：owned Go 文件 `gofmt -d`；server/config/probe 最小只编译；
  config/server/probe focused 普通 `-count=10` 与 race `-count=3`；probe 新非 Hint 拒绝测试；
  `go test ./cmd/crystal-server -skip '^TestSessionOmaMageRangeSlowFrozenTranscript$' -count=1
  -timeout=900s`；对应 `go test ./... -skip ...`；`go test -race ./... -skip ...`；
  `go vet ./...`；`go build ./...`；`go run ./cmd/crystal-protocol-probe -mode vectors`；
  `git diff --check`。22 个直接发送 `ClientStartGame` 的文件已机械枚举，所有 consumer 均由
  服务端整包与完整 race（排除下述唯一 flake）执行。
- 已知排除项：无排除 `go test ./cmd/crystal-server -count=1` 与两次无排除
  `go test ./... -count=1` 均退出 1，唯一失败为既有
  `TestSessionOmaMageRangeSlowFrozenTranscript` 的随机边界 `[2 1]`/期望 `[1]`；普通隔离
  `-count=10` 退出 1 并两次复现。较早一次无排除完整 race 退出 0，但最终
  `go test -race ./... -count=1 -timeout=900s` 退出 1，同样只命中 OmaMage assertion、无 race
  detector 报告；隔离 race `-count=3` 恰好退出 0，排除该用例的最终完整 race 退出 0。
  失败栈未进入本批文件，未修改 OmaMage 掩盖。
- Subagent：可用 spawn model 列表不提供要求的 `luna_worker`/`gpt-5.6-luna`，故未静默
  替换；主 Agent 完成恢复、Legacy tracing、review、修正、验证、文档与提交。
- 下一步：新 Session 分别核对两仓 clean status/HEAD 与三类 `.cs` 审计，再从 matrix 选择
  下一个 dependency-ready StartGame 差集。不要重复 BaseStats、SpellToggle、Notice 或
  localized welcome bootstrap。

### 本批提交边界

- Go 提交 `7e10f9f` 仅包含上述 29 个 owned 文件（25 tracked 修改 + 4 个原 untracked）；
  提交后工作树 clean。
- Legacy 文档提交仅包含上述四个 owned Markdown；提交后工作树 clean，所有 `.cs` 继续只读。
- 整体 Goal 仍 active；当前切片已验证并提交，P1/P3 均保持 In progress。

## 历史批次快照（server Notice bootstrap 已收口）

恢复时间：2026-08-23；沿用同一个 active Goal。本批继续机械核对 Legacy
`PlayerObject.StartGameSuccess` 与 Go `serveWithConfig`，选出 P1/P2/P3 的
dependency-ready 条件 bootstrap 差集：Legacy 在 Notice 文件更新时间严格晚于角色
`LastLogoutDate` 时，于角色物品定义前发送 `UpdateNotice`；Go 同时缺少该包与真实会话
日期写入。该切片已由 Go 提交 `be5c355 feat(p3): restore server notice bootstrap`
原子收口；P1/P2/P3 与整体 Goal 仍为 In progress。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，本批文档提交 parent `28c87d2a docs(migration): record persisted combat
  toggles`（提交后以 `git log -1 --oneline` 为准）；本批 owned 文档为
  `tasks/lessons.md`、`tasks/lessons-archive/migration/protocol-session-wire.md` 与本
  handoff，无其他文件；提交后工作树 clean，tracked/staged/untracked `.cs` 均为空。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `be5c355 feat(p3): restore server notice bootstrap`；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Legacy 权威行为：`Settings.LoadNotice` 从 `./Envir/Notice.txt` 读取 UTF-8 行，非空文件
  记录 `GetLastWriteTime`，保留当前 `string.Compare(line, "TITLE", false) > 0`、
  `Contains("=")`、`Split('=')[1]` 与逐正文行追加 `\r\n` 的怪癖；
  `StartGameSuccess` 仅在 Message 非 whitespace 且 `LastUpdate > Info.LastLogoutDate` 时，
  于 StartGame/欢迎与 guild 校验之后、`GetItemInfo` 之前 enqueue
  `S.UpdateNotice`。wire 为 ordinal 272 与连续两个 .NET string（Title、Message）。
  Player 构造写 `LastLoginDate=Envir.Now`，`StopGame` 写 `LastLogoutDate=Envir.Now`，
  `ToSelectInfo` 将后者投影为 `LastAccess`。
- Go 实现：`internal/config` 新增 Legacy-compatible Notice loader 与
  `CRYSTAL_NOTICE_PATH`；`internal/protocol` 新增 ordinal 272、payload/parser；probe 固定
  向量与网络 bootstrap 接受该包；auth 新增 login/logout date authority；StartGame 在
  pre-item boundary 按严格时间门禁发包，并在成功进入、显式登出、observer takeover、
  断线 cleanup 的 JSON/checkpoint 边界写日期。
- Go 本批 16 个 owned 文件：`cmd/crystal-server/main.go`、`main_test.go`、
  `notice_bootstrap.go`、`notice_bootstrap_test.go`；`internal/auth/service.go`、
  `service_test.go`；`internal/config/config.go`、`notice.go`、`notice_test.go`；
  `internal/probe/network.go`、`vectors.go`；`internal/protocol/packet.go`、
  `packet_test.go`、`notice.go`、`notice_test.go`；`docs/migration-matrix.md`。
- 测试覆盖：loader BOM/CRLF/missing/empty/title 怪癖与 timestamp reset；strict equality
  admission；固定 wire/malformed payload；auth JSON 与 SelectInfo LastAccess；真实认证
  `net.Pipe` 首登公告→显式登出→JSON restart→不重复公告→断线 checkpoint。
- 已通过（退出码 0）：owned Go 文件 `gofmt -d`；五包最小只编译；focused 普通
  `-count=10`；focused race `-count=3`；`go test ./cmd/crystal-server -count=1
  -timeout=900s`；`go test ./... -count=1 -timeout=900s`；`go test -race ./...
  -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；
  `go run ./cmd/crystal-protocol-probe -mode vectors`；`git diff --check`。
- 失败/修正证据：一次 Go workdir 读取误带 Legacy Settings 路径，整次输出按 C01 作废
  并拆仓重跑；Notice loader 初稿错误地用 `UTC()` 代替 timestamp 清零，主 Agent review
  后改为显式 zero 并由 missing/empty 测试锁定；矩阵复合 patch 第二 hunk 因陈旧正文
  失败，已复读物理行、独立核验第一 hunk、单独重跑第二 hunk并通过 diff check。
- Subagent 状态：spawn tool 的模型列表不提供要求的 `luna_worker`/
  `gpt-5.6-luna`，因此未静默替换；主 Agent 完成 tracing、实现、审查和验证。
- 矩阵行：P1/P2/P3 已增加 Notice file/config、UpdateNotice wire/session、
  login/logout timestamp authority 与 restart/relogin evidence；三阶段仍 In progress。
- 下一恢复命令：新 Session 回读本文件、`tasks/goal-task.md`、`tasks/lessons.md` 与 Go
  `docs/migration-matrix.md`，分别核对两仓 clean status/HEAD；从矩阵选择下一个
  dependency-ready pending 子切片，优先继续机械核对 Legacy/Go StartGame packet 差集，
  但不得重复 BaseStats、persisted SpellToggle 或 server Notice bootstrap。

### 本批提交边界

- Go 原子提交 `be5c355` 仅包含上述 16 个 production/test/probe/matrix 文件；Legacy
  文档提交仅包含上述三个 Markdown。两仓提交前后均按完整 status、diff check 和
  tracked/staged/untracked `.cs` 审计。
- 当前批次已完成且可安全切换，但整体 Goal 未完成；下一 Session 必须重新选择 pending
  子切片，不得重复 BaseStats、persisted SpellToggle 或 server Notice bootstrap。

## 历史批次快照（persisted combat SpellToggle bootstrap 已收口）

恢复时间：2026-08-23；沿用同一个 active Goal。本批从 Legacy
`PlayerObject.StartGameSuccess` 与 Go `serveWithConfig` 的真实登录调用链选出一个明确、
dependency-ready 的 P3/P5 bootstrap 差集：Go 已持久化四个战斗模式，但登录时未像
Legacy 一样恢复对应 `SpellToggle` 包。该切片已由 Go 提交
`c066802 feat(p3): restore persisted combat toggles` 原子收口；P3、P5 与整体 Goal 仍为
In progress，不因本批完成而重开、reset 或标记 Complete。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，本批文档提交 parent 为
  `8d69694a docs(migration): record configured base stats`（提交后以
  `git log -1 --oneline` 为准）；本批 owned 文档为 `tasks/lessons.md`、
  `tasks/lessons-archive/migration/protocol-session-wire.md` 与本 handoff。提交前无任何
  其他 tracked/staged/untracked 文件，tracked/staged/untracked `.cs` 均为空。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支
  `main`，HEAD `c066802 feat(p3): restore persisted combat toggles`；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Legacy 权威行为：`StartGameSuccess` 在 `DefaultNPC -> GuildBuffList` 后按固定顺序检查
  `Info.Thrusting`、`Info.HalfMoon`、`Info.CrossHalfMoon`、`Info.DoubleSlash`；只为 true
  项发送 `S.SpellToggle(ObjectID, Spell, CanUse=true)`，随后才恢复普通宠物。Server
  serializer 为 `UInt32 ObjectID + Byte Spell + Boolean CanUse`。
- Go 实现：新增 `playerSpellToggleBootstrapPackets`，按相同固定顺序过滤 false 状态并
  使用当前 world ObjectID；`serveWithConfig` 在 guild-buff definition bootstrap 后、
  mailbox/pet/buff/Hero 恢复前逐包写出。既有 ClientSpellToggle handler、auth JSON/Legacy
  bridge 字段继续作为持久化 authority，没有新增 schema 或 C#。
- Go 本批六个提交文件：`cmd/crystal-server/main.go`、
  `spell_toggle_bootstrap.go`、`spell_toggle_bootstrap_test.go`、
  `warrior_attack_session_test.go`、`observer_session_test.go` 与
  `docs/migration-matrix.md`。
- 新测试覆盖：helper 的 disabled omission 与 partial fixed order；真实认证 `net.Pipe`
  StartGame 在 `GuildBuffList` 后依次恢复四个精确 payload，并通过 KeepAlive 确认无额外
  toggle；既有 HalfMoon/Thrusting/DoubleSlash/FatalSword 和 observer transcript 明确
  消费新增条件式 server-first 包。
- 已通过（退出码 0）：owned Go 文件 `gofmt -d`；最小/首次 focused 测试；上述完整
  toggle/warrior/observer 集合普通 `-count=10`；对应 race `-count=3`；
  `go test ./cmd/crystal-server -count=1 -timeout=900s`；
  `go test ./... -count=1 -timeout=900s`；
  `go test -race ./... -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；
  `git diff --check` 与两仓 `.cs` 审计。
- 失败与修复证据：首轮组合定向命令因旧测试未消费新 toggle 而无输出挂起，手工中断
  退出 1；隔离 HalfMoon 后 30 秒超时退出 1，栈显示 client 在写 Attack、server 在
  `main.go` toggle write，确认 `net.Pipe` write/write deadlock。根因是只按 packet ID
  检索消费者，没有同时检索使条件包出现的 `UpdateCharacterSpellToggle` seed。修复四个
  warrior 与 observer consumer 后，隔离、重复、race、服务端整包和全仓门禁全部通过；
  symptom/root/prevention/verification 已强化到 protocol-session-wire archive。
- 工作流证据：本 Session 首次恢复读取误把 Go matrix 绝对路径放入 Legacy 命令，整次
  输出按 C01 作废并按两仓重跑；一次含中文 Python here-doc 在执行前被源码编码拒绝，
  确认解释器后改用 `apply_patch`，并强化 C02。两个失败均未产生 C# 或未归属代码变更。
- Subagent 状态：可用 spawn tool 未提供要求的 `luna_worker`/`gpt-5.6-luna`，因此没有
  静默替换模型；主 Agent 完成 batch selection、tracing、实现、审查、验证、文档和提交。
- 矩阵行：P3/P5 已增加 persisted four-mode `SpellToggle` StartGame restoration 与精确
  GuildBuff boundary 证据；两个阶段仍保持 In progress。
- 下一恢复命令：新 Session 回读本文件、`tasks/goal-task.md`、`tasks/lessons.md` 与 Go
  `docs/migration-matrix.md`，分别核对两仓 clean status/HEAD；从矩阵选择下一个
  dependency-ready pending 子切片，优先继续机械核对 Legacy/Go StartGame packet 差集，
  但不得再次选择已完成的 BaseStats 或 persisted combat-toggle bootstrap。

### 本批提交边界

- Go 原子提交 `c066802` 仅包含上述六个 production/test/matrix 文件；未混入无关修复或
  任何 `.cs`。
- Legacy 文档提交仅包含上述三个 Markdown 文件；提交前后均按完整 status、
  `git diff --check` 与 tracked/staged/untracked `.cs` 审计。
- 当前批次已完成且可安全切换；下一批必须重新选择并建立新的 owned-file boundary。

## 历史批次快照（custom BaseStats 已收口）

恢复时间：2026-08-23；当前继续沿用同一个 active Goal。compact 后发现上一版
handoff 仍声称 Go clean，但实际有 8 个 tracked 修改和 2 个 untracked 文件，因此先
停止实现/测试并从两仓重建 durable boundary，随后完成上一 `BaseStatsInfo` 批次明确
pending 的配置/runtime 子切片。该子切片已由 Go 提交
`ce0283c feat(p3): load configured base stats` 原子收口；整体 Goal、P1、P3、P5 仍未
完成，不因本批提交而重开、reset 或标记 Complete。

- Legacy 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支
  `master`，本 handoff 文档提交的 parent 为
  `67139fbb docs(migration): record base stats bootstrap`（提交后以
  `git log -1 --oneline` 为准）；本批四个 owned 文档为 `tasks/lessons.md`、
  `tasks/lessons-archive/migration/protocol-session-wire.md`、
  `tasks/lessons-archive/verification/race-and-flake-attribution.md` 与本 handoff；提交后
  工作树 clean，无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Go 仓库：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，
  分支 `main`，HEAD `ce0283c feat(p3): load configured base stats`；提交后工作树 clean，
  无 staged/untracked，tracked/staged/untracked `.cs` 均为空。
- Legacy 权威链：`Settings.LoadBaseStats/LoadHeroBaseStats` 从 `Configs` 中按五职业
  enum 名加载 profile，缺失文件保留/生成默认值；numeric `TryParse` 失败回落零，
  非空未知 formula 由 `Enum.Parse` 拒绝；`BaseStat.Calculate` 保留 `Gain == 0` 直接返回
  Base、Warrior Health、Wizard/Taoist Mana、Weight/Stat 与 Max；
  `HumanObject/HeroObject.RefreshLevelStats`、`RefreshStatCaps` 和 `SendBaseStats` 使用同一
  profile authority。
- Go 生产实现：`internal/config` 加载同目录五组 `BaseStats<Class>.ini` 与
  `HeroBaseStats<Class>.ini`；`internal/progression` 统一五职业默认 profile、全部 Legacy
  Stat ordinal、四类公式、class 特例、Max 与深复制；world/session 在 player enter 和
  Hero summon 挂接 runtime-only profile，equipment/runtime/cap、`BaseStatsInfo`、
  `HeroBaseStatsInfo`/`HeroInformation` 使用同一配置，并通过 `json:"-"` 禁止配置泄入
  account JSON。自定义 profile 可省略 stat/cap；零值不再被旧登录默认 fallback 覆盖。
- Go 本批 17 个提交文件：`cmd/crystal-server/base_stats.go`、
  `base_stats_config_test.go`、`equipment_transactions.go`、`hero_session_test.go`、
  `heroes.go`、`main.go`、`world.go`；`internal/config/config.go`、`base_stats.go`、
  `base_stats_test.go`；`internal/progression/player_stats.go`、`base_stats.go`、
  `base_stats_test.go`；`internal/protocol/hero.go`、`packet.go`、
  `base_stats_transient_test.go`；`docs/migration-matrix.md`。
- 已通过（退出码 0）：owned Go 文件 `gofmt -d` 零输出；最小三包只编译；
  config/progression/server BaseStats 定向普通 `-count=10`；对应 race `-count=3`；
  protocol BaseStats/JSON `-count=10`；服务端整包重跑；全仓普通重跑；
  `go test -race ./... -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；
  `git diff --check` 与两仓 `.cs` 审计。
- 失败与修复证据：首次定向普通退出 1，因为测试误把 `Gain == 0` 的计算短路当成
  profile 字段清零，并把 Wizard Mana level-10 结果误算为 112 而非 129；按 Legacy
  原始字段与公式修正期望后重复/race 通过。首次 `go test ./...` 退出 1，仅命中既有
  `TestSessionOmaMageRangeSlowFrozenTranscript` 随机边界 `[2 1]`/`[1]`；隔离
  `-count=10` 随后退出 0，服务端整包及全仓普通重跑退出 0，最终完整 race 也退出 0。
  两次失败均已按 symptom/root/prevention/verification 写入/强化对应 archive/active
  lesson，不修改 OmaMage 或无关模块掩盖。
- Subagent 状态：本 Session 的 spawn 工具不提供要求的 `luna_worker`/
  `gpt-5.6-luna`，因此没有静默替换模型或委派；主 Agent 本地完成 tracing、审查、
  实现、测试、文档和提交。
- 矩阵行：P1/P3/P5 证据已更新为 customized player/Hero BaseStats INI、runtime
  formulas/caps、authenticated player bootstrap、authenticated Hero summon 和 transient
  JSON；三个阶段仍保持 In progress。
- 下一恢复命令：新 Session 回读本文件、`tasks/goal-task.md`、`tasks/lessons.md` 与
  Go `docs/migration-matrix.md`，分别核对两仓 clean status/HEAD；从矩阵选择下一个
  dependency-ready pending 子切片（不得再次选择已完成的 custom BaseStats），继续同一
  active Goal。

### 本批提交边界

- Go 原子提交 `ce0283c` 仅包含上述 17 个 production/test/matrix 文件；未混入
  OmaMage/DarkBody 等无关修复或任何 `.cs`。
- Legacy 文档提交仅包含上述四个 Markdown 文件；提交前后均按完整 status、
  `git diff --check` 与 tracked/staged/untracked `.cs` 审计。
- 当前批次已完成且可安全切换；下一批必须重新按矩阵选择并建立新的 owned-file
  boundary，不能继续向本提交追加功能。

## 历史批次快照（2026-08-23 AI=49 ThunderElement）

恢复时间：2026-08-23；当前仍沿用同一个 active Goal。矩阵中 P5 AI=49
`ThunderElement` 的领域行为已有 deterministic 覆盖，本批补齐真实认证
`net.Pipe` 攻击 transcript 与矩阵证据；整体 Goal 仍未完成，不因本批完成宣告
P5 或整体迁移完成。

- Legacy 仓库实际状态：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支 `master`，HEAD 为本批 handoff 提交（以 `git log -1 --oneline` 为准）；本批两个文档已提交，工作树 clean，无 staged/untracked；tracked/staged/untracked `.cs` 均为空。
- Go 仓库实际状态：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，HEAD `ab4e382 test(p5): add ThunderElement session transcript`；本批两个文件已原子提交，工作树 clean，无 staged/untracked；tracked/staged/untracked `.cs` 均为空。
- 本批 Go 所属文件仅为 `cmd/crystal-server/thunder_element_session_test.go` 与 `docs/migration-matrix.md`；Legacy 所属文件仅为 `tasks/lessons-archive/verification/fixtures-and-transcripts-02.md` 与本 handoff。未修改任何 C# 或 Go 生产实现。
- 本批 Legacy 对照范围为 `Server/MirObjects/Monsters/ThunderElement.cs`、`Server/MirObjects/MonsterObject.cs` 的 `Process`/`CompleteAttack`/`Attacked`/`Pushed` 调用链，以及共享 Player MAC damage/notification serializer；保留 `CompleteAttack` 先完成全部目标 `Attacked`、再广播 `ObjectAttack` 的顺序。
- 本批 Go transcript 覆盖真实 StartGame bootstrap 的 `ObjectMonster(AI=49)`、AI admission 的 300ms 延迟动作、impact-time radius-two rescan、Player `Struck -> ObjectStruck -> DamageIndicator -> HealthChanged` 及最后的 `ObjectAttack`，并断言 HP 100→80、动作队列清理和精确 payload。
- 已通过（退出码 0）：`gofmt`；`go test ./cmd/crystal-server -run '^$' -count=1 -timeout=900s`；`go test ./cmd/crystal-server -run '^TestSessionThunderElementAttackTranscript$' -count=10 -timeout=600s`；对应 `-race -count=3`；`go test ./cmd/crystal-server -run 'ThunderElement|GreatFoxSpirit' -count=10 -timeout=600s`；对应 `-race -count=3`；`go vet ./...`；`go build ./...`；`git diff --check`。
- 全量普通门禁 `go test ./... -count=1 -timeout=900s` 退出码 0，所有包通过。
- 无排除完整 race 门禁 `go test -race ./... -count=1 -timeout=900s` 退出码 1；实际唯一失败为既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`（`player_spell_buffs.go:742,824` 与 `intelligent_creature_items.go:549` 的共享 session/equipment-stat race），未进入 AI=49 文件或测试，未修改无关模块掩盖。
- 本批曾因 transcript 直接读取未加锁的 `monsterAttackActions`/HP 触发一次定向 race；已改为锁内复制快照，并将 symptom/root cause/prevention/verification 追加至 `tasks/lessons-archive/verification/fixtures-and-transcripts-02.md`；最终 AI=49/50 定向 race 未复现本批 race。
- 恢复命令：新 Session 先回读本文件、`tasks/goal-task.md`、`tasks/lessons.md` 和 Go `docs/migration-matrix.md`，分别核对两仓 status/HEAD；本批 Go `ab4e382` 与 Legacy handoff 提交均已提交，随后从矩阵中选择下一个仍未完成的 P5 明确子切片或其他 dependency-ready 项；不得因 compact、文档批次或单个 AI 完成重开/重建/重置 Goal。

### 本批质量门禁与提交边界

- Go 功能提交 `ab4e382` 只包含 `cmd/crystal-server/thunder_element_session_test.go` 与 `docs/migration-matrix.md`；不得把 Legacy 文档混入 Go 提交。
- Legacy 文档提交只包含 `tasks/lessons-archive/verification/fixtures-and-transcripts-02.md` 与 `tasks/migration-handoff.md`；不得暂存或提交任何 `.cs` 文件。
- 提交前后均须分别执行两仓的 `git diff --check`、tracked/staged/untracked `.cs` 审计和完整 `git status --short --branch`；全量普通/race 的既有失败必须保留在 handoff，不得为通过全量门禁修改无关模块。

## 历史批次快照（2026-08-22 Conquest Gate/Wall 阻挡回归）

恢复时间：2026-08-22；当前仍沿用同一个 active Goal。上一批 P5 AI=60/61/62/63 ArcherSummon（`VampireSpider`/`SpittingToad`/`SnakeTotem`/`CharmedSnake`）已由 Go 提交 `f195d89 feat(p5): complete archer summon target projections` 收口；本批 P9 Conquest Gate/Wall 阻挡几何回归测试已由 Go 提交 `d09cbe9 test(p9): cover conquest gate blocking geometry` 收口。整体 Goal 仍未完成，不因 compact 或单批完成宣告整体 Goal 完成。

- Legacy 仓库实际状态：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`，分支 `master`；本 handoff 提交的 parent 为 `4a9242a0 docs(migration): record conquest structure handoff`，提交后工作树 clean，无 staged/untracked；本批仅修改文档，未修改 `.cs`。
- Go 仓库实际状态：根 `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`，分支 `main`，HEAD `d09cbe9 test(p9): cover conquest gate blocking geometry`；本批两个文件已原子提交，工作树 clean，无 staged/untracked。
- 本批 Go 所属文件为 `cmd/crystal-server/conquest_structures_test.go`、`docs/migration-matrix.md`；Legacy tracing 范围为 `Server/MirObjects/ConquestObject.cs`、`Server/MirObjects/Monsters/CastleGate.cs`、`Gate.cs`、`Wall.cs`、`Siege.cs` 及地图阻挡/进入检查调用链。Legacy C# 仍为只读基线。
- 本批覆盖四种 Legacy Gate `BlockArray` 的逐项偏移顺序、关闭 Gate 的扩展阻挡格与父 Gate 反查、Wall 仅阻挡自身格、非自身格可进入，以及 Gate 打开后扩展阻挡清理；现有 Conquest 生产逻辑未新增 C# 或 Go 生产文件。
- 本批已通过：`go test ./cmd/crystal-server -run '^$' -count=1 -timeout=900s`；`go test ./cmd/crystal-server -run 'TestConquestGate|TestConquestStructure' -count=10 -timeout=600s`；对应 `-race -count=3`；`go test ./cmd/crystal-server -run 'Conquest|conquest' -count=1 -timeout=900s`；对应 `-race -count=3`；`go vet ./...`；`go build ./...`；`git diff --check`。
- `go test ./cmd/crystal-server -count=1 -timeout=900s` 与 `go test ./... -count=1 -timeout=900s` 均未通过，唯一领域失败为既有 `TestSessionOmaMageRangeSlowFrozenTranscript`（`oma_mage_session_test.go:145`，实际 attack-roll bounds `[2 1]`，期望 `[1]`），栈未进入本批文件或测试；未修改无关模块掩盖。
- `go test -race ./... -count=1 -timeout=900s` 未通过，实际失败为既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`；race 栈位于 `player_spell_buffs.go:742` 与 `intelligent_creature_items.go:549` 的共享 session/equipment-stat 读写，未进入本批文件或测试；未修改无关模块掩盖。
- 两仓当前 tracked/staged/untracked `.cs` 审计均为空。
- 恢复命令：回读本文件、`tasks/goal-task.md`、`tasks/lessons.md` 和 Go `docs/migration-matrix.md`，分别核对两仓实际状态；沿用同一个 active Goal，从矩阵核对后的下一个仍 pending 的 P5 AI/target 或其他明确子切片继续，不得因 compact 或新 Session 重开、重建或 reset Goal。

### 本批质量门禁与提交边界

- Go 功能提交只包含 `cmd/crystal-server/conquest_structures_test.go` 和 `docs/migration-matrix.md`；不得把 Legacy 文档混入 Go 提交。
- Legacy 本批只提交 `tasks/migration-handoff.md`；不得暂存或提交任何 `.cs` 文件。提交前后均须按下方固定命令审计 tracked/staged/untracked `.cs`。
- 完整普通测试的既有 OmaMage 随机边界失败和完整 race 的 GuildBuff/session 共享状态失败保留在 handoff 中，不得为通过全量门禁修改无关模块。

## 仓库快照

| 仓库 | 路径 | 分支 | 当前基线 | 交接前状态 |
|---|---|---|---|---|
| Legacy Crystal | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal` | `master` | `HEAD（本 handoff 文档提交；parent 4a9242a0）` | 本批文档提交后工作树 clean；无 staged/untracked、无 `.cs` 变化 |
| Go migration | `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer` | `main` | `d09cbe9 test(p9): cover conquest gate blocking geometry` | Conquest Gate/Wall 回归批次已原子提交；工作树 clean，无 staged/untracked、无 `.cs` 变化 |

新 Session 应以实际 `git status --short --branch` 和 `git log -1 --oneline` 为准；不要把矩阵 Complete 或历史测试记录误判为当前工作树和当前门禁已验证。

迁移状态的权威明细位于 Go 仓库的 `docs/migration-matrix.md`；`README.md` 是当前实现能力的长说明。

## 最近完成批次：AI=75 WitchDoctor

- Go 已由提交 `7801232 feat(p5): complete WitchDoctor AI` 收口，涉及
  `witch_doctor.go`、`witch_doctor_test.go`、`witch_doctor_session_test.go`、
  `monster_ai.go`、`world.go` 和 `docs/migration-matrix.md`；Go 工作树 clean。
- 已通过：`gofmt`；`go test ./cmd/crystal-server -run '^$' -count=1 -timeout=600s`；
  `go test ./cmd/crystal-server -run 'WitchDoctor' -count=10 -timeout=600s`；
  `go test -race ./cmd/crystal-server -run 'WitchDoctor' -count=3 -timeout=600s`；
  `go test ./cmd/crystal-server -count=1 -timeout=900s`；
  `go test ./... -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；
  `git diff --check`。
- `go test -race ./... -count=1 -timeout=900s` 未通过，实际失败仅为既有
  `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`（
  `player_spell_buffs.go`/`intelligent_creature_items.go` 与共享 session fixture
  的 equipment-stat race）以及 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`
  （`player_spell_buffs.go` 与 `teleport_magic_session_test.go` 的 buff/packet race）；
  两个栈均未进入 AI=75 文件或测试，不修改无关模块掩盖。
- 本批覆盖 common population/dispatch、Legacy 基类随机方向、cell-order 搜索、
  六格含同格 FearTime/撤退与 Shock 门禁、Player/owned-Monster/Hero 投影、
  self-heal、1/5 teleport 及 effect-5 visibility、MACAgility 延迟命中、
  safe-zone 目标重验和认证 `net.Pipe` ranged transcript。
- Legacy 本批文档范围：`tasks/lessons-archive/migration/combat-general.md` 与
  本 handoff；均为文档，未触碰 `.cs`。Legacy 当前 tracked/staged/untracked `.cs`
  均为空。
- 下一恢复命令：新 Session 重新读取本文件、`tasks/goal-task.md`、
  `tasks/lessons.md` 和 Go `docs/migration-matrix.md`，分别核对两仓实际状态，
  再从矩阵中选择下一个仍 pending 的 P5 子切片；不要把 P5 或整体 Goal 标为完成。

## 最近完成批次：AI=74 LightTurtle

- Go 已由提交 `4239ef3 feat(p5): complete LightTurtle AI` 收口，涉及
  `light_turtle.go`、`light_turtle_test.go`、`light_turtle_session_test.go`、
  `monster_ai.go`、`world.go` 和 `docs/migration-matrix.md`；Go 工作树 clean。
- 已通过：`gofmt`；`go test ./cmd/crystal-server -run '^$' -count=1 -timeout=600s`；
  `go test ./cmd/crystal-server -run 'LightTurtle' -count=10 -timeout=600s`；
  `go test -race ./cmd/crystal-server -run 'LightTurtle' -count=3 -timeout=600s`；
  `go vet ./...`；`go build ./...`；`git diff --check`。
- `go test ./cmd/crystal-server -count=1 -timeout=900s` 与
  `go test ./... -count=1 -timeout=900s` 均未通过，唯一领域失败为既有
  `TestSessionOmaMageRangeSlowFrozenTranscript`（实际随机边界 `[2 1]`，期望
  `[1]`）；单独 `-run '^TestSessionOmaMageRangeSlowFrozenTranscript$' -count=10`
  亦复现，栈未进入 AI=74 文件或测试；其余 Go 包通过。
- `go test -race ./... -count=1 -timeout=900s` 未通过，实际失败为既有
  `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`（
  `player_spell_buffs.go`/`intelligent_creature_items.go` 与共享 session fixture
  race）以及 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff`
  （`player_spell_buffs.go` 与 `teleport_magic_session_test.go` 的 buff/packet
  并发 race）；两个栈均未进入 AI=74 文件或测试，不修改无关模块掩盖。
- 本批范围是 common population、AI dispatch/action 接线、随机构造方向、两种
  `ObjectAttack` 分支、延迟目标/map 重验、Player/owned-Monster/Hero 投影、
  Green poison 和认证 `net.Pipe` transcript；Legacy C# 仍为只读基线。
- Legacy 本次文档工作树仍为 `agents.md`、`tasks/goal-task.md`、
  `tasks/lessons-archive/migration/combat-general.md`、`tasks/lessons.md` 和
  本 handoff；tracked/staged/untracked `.cs` 均为空。

## 最近完成批次：AI=73 TurtleKing

- Legacy 当前未提交文件：`agents.md`、`tasks/goal-task.md`、`tasks/lessons.md`、`tasks/lessons-archive/migration/combat-general.md`、`tasks/migration-handoff.md`；均为文档，未触碰 `.cs`。
- Go 本批提交：`e2033e82b2fd2528aa4879a58489d3fc286dce31 feat(p5): complete TurtleKing AI`，涉及 TurtleKing 实现/测试、monster population/dispatch、settings/world schema、matrix 和 FinialTurtle poison helper；Go 工作树 clean。
- 已通过：`gofmt`；`go test ./cmd/crystal-server -run '^$' -count=1 -timeout=600s`；`go test ./cmd/crystal-server -run 'TurtleKing' -count=10 -timeout=600s`；`go test -race ./cmd/crystal-server -run 'TurtleKing' -count=3 -timeout=600s`；`go test ./cmd/crystal-server -count=1 -timeout=900s`；`go test ./... -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；`git diff --check`。
- 最新 `go test -race ./... -count=1 -timeout=900s` 仅失败于既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`；race 栈在 `player_spell_buffs.go` 的 equipment-stat 与共享 session fixture 读写，未进入 AI=73 文件或测试；其余包通过。该失败不是本批回归，不修改无关模块掩盖。
- AI=73 production-entry tests 覆盖五阶段召唤、配置名/front fallback、30-child cap、spawn gate/cooldown movement、Player/owned-Monster/Hero 搜索与伤害、close DC line、close/long-range Type=1 `CompleteRangeAttack` area、target/attacker teleport、effect/poison、延迟 target/map revalidation、attacker-dead queued hit、observer fan-out 和 authenticated `net.Pipe` ranged transcript。
- 下一恢复命令：新 Session 重新读取本文件、`tasks/goal-task.md`、`tasks/lessons.md` 和 Go `docs/migration-matrix.md`，确认两仓实际状态后，从矩阵中下一个仍 pending 的 P5 AI/target sub-slice 继续；不要把 P5 或整体 Goal 标为完成。

## AI=72 FinialTurtle 上一批恢复点

- Go AI=72 已由提交 `12ac311 feat(p5): complete FinialTurtle AI` 收口；Legacy AI=72/archive/handoff 文档仍与当前 compact-safety 文档一起处于本仓库未提交文档变更中，未触碰 `.cs`。
- Legacy 本批文档范围：`tasks/migration-handoff.md`、`tasks/lessons-archive/migration/combat-general.md`；均为文档，未触碰 `.cs`。
- 已通过：`go test ./cmd/crystal-server -run 'FinialTurtle' -count=10 -timeout=600s`；`go test -race ./cmd/crystal-server -run 'FinialTurtle' -count=3 -timeout=600s`；`go test ./cmd/crystal-server -count=1 -timeout=600s`；`go test ./... -count=1 -timeout=900s`；`go vet ./...`；`go build ./...`；`git diff --check`。
- 最新 `go test -race ./... -count=1 -timeout=900s` 未通过，唯一失败为既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff`，栈在 `player_spell_buffs.go` 装备统计与共享 session fixture 的并发读写，未进入 AI=72 文件或测试；其余包通过。
- AI=72 测试覆盖 common population、两秒 Spawned gate、随机构造方向、六格 admission、相邻 DC/远程 MC、Player/owned-Monster/Hero、延迟 target/map 重验、攻击者死亡后仍结算、Slow/Frozen 双 poison、冷却期 `ObjectWalk` 与认证 `net.Pipe` transcript。
- 下一恢复命令：新 Session 重新读取本文件和 Go 矩阵，从下一个仍未完成的 P5 AI/target sub-slice 继续；不要把 AI=72 批次或本 Goal 标为整体完成。

## 当前 Goal 状态

本线程当前已恢复以下完整 Goal，状态为 active；不要因为某个批次完成而把
整体 100% 迁移误标为完成。后续 Session 继续沿用该 Goal：

> 将当前 Crystal 项目按现有功能与客户端可观察行为 100% 迁移到独立 Git 管理的跨平台 Go 项目：服务端、测试客户端、协议探针、导入导出及迁移工具全部使用 Go；两个仓库中的所有 C# 文件仅作只读基线；按可安全合并的功能批次实现、测试、更新迁移矩阵并 Git 提交，持续推进直至功能矩阵全部完成。

## 阶段进度

迁移矩阵共有 P0–P12 十三个阶段。目前是 2 个 Complete、11 个 In progress、0 个 Pending。该计数只表示阶段状态，不等于准确的代码或功能完成百分比。

| 阶段 | 状态 | 当前摘要 |
|---|---|---|
| P0 协议帧/连接基础 | Complete | 帧编码、版本、断开、KeepAlive 和 Go 固定向量已完成。 |
| P1 配置与生命周期 | In progress | INI/环境配置、版本哈希、连接限制、超时、包速率和优雅关闭已有覆盖；完整重启/日志生命周期待闭环。 |
| P2 账户与密码 | In progress | 登录、账户创建、改密、StoragePassword、导入账户和 SelectInfo 已迁移；直接二进制写回及完整 NPC 访问仍待完成。 |
| P3 角色与 StartGame | In progress | 角色列表/创建/删除、运行时字段、有效/无效出生点、基础属性与登出持久化已有覆盖，尚未按完整客户端启动流程宣告完成。 |
| P4 地图/移动/可见性 | In progress | 多版本地图、碰撞/门、玩家/NPC/怪物可见性、地图切换、普通/私聊及聊天物品链接授权展开和多项地图门禁已迁移；完整 bootstrap 仍待完成。 |
| P5 战斗/技能/怪物/掉落 | In progress | 已完成核心近战、远程、PvP 基础、多个单体/区域魔法、九个自增益 Buff、FrostCrunch 状态、基础属性、掉落树和持久化；最近批次新增 AI=75 WitchDoctor、AI=70 Hugger 的邻接攻击/死亡爆炸与 poison 生命周期，并新增玩家 TrapHexagon、SummonVampire/SummonToad/SummonSnakes、CannibalPlant、Guard、Tao Guard、Deer AI=1/2、Tree AI=3、EvilCentipede AI=14、WoomaTaurus AI=11、RedMoonEvil AI=13、Shinsu AI=18、BugBagMaggot AI=12/RootSpider AI=39/BombSpider AI=40、RightGuard/LeftGuard AI=31/32、MinotaurKing AI=33、FrostTiger AI=34、ThunderElement AI=49 GreatFoxSpirit AI=50、Dragon/EvilMir AI=52/53、DragonStatue AI=54、HumanWizard AI=55 以及 Trainer AI=56 的 Legacy admission、目标捕获、延迟/生命周期、AI/伤害/逃跑、静态/Observer 可见性、HumanWizard owner-MP/owner-appearance/inspection/persistence 和 net.Pipe transcript；Trainer 以确定性 world production-entry tests 覆盖玩家/Hero/owned-pet、AC/MAC、AttackBonus、owner-root、miss、poison/DPS、切换/超时、Healing ChangeHP 和 value-map writeback；TownArcher AI=57 已补 common population、route-only movement、Player-only red-PK/GM/Hidden/CoolEye/level admission、strict FearTime、inclusive ten-cell range、ObjectRangeAttack、Luck/DC、500ms+distance×50ms 延迟 ACAgility、respawn direction reset 和 impact revalidation；HumanAssassin AI=59 已补 owner-derived stats/appearance、two-cell movement、delayed ACAgility、cumulative threshold、strict lifetime explosion 和 recall/logout lifecycle；剩余技能/Buff、飞行/墙体规则、高级 PvP/组队战斗、其他通用/特殊怪物 AI、持久重生状态及完整包序仍待迁移。 |
| P6 物品/装备/维修/强化/制作 | In progress | 背包、装备、Storage、Trade、Repair、Refine、Craft 和基础 Use/Delete/Drop/Pickup 已有完整功能簇；尚未对整个 P6 做 100% 等价收口。 |
| P7 NPC/商店/任务/脚本 | In progress | NPC 可见性、传送、核心脚本动作/控制流、商店/BuyBack、Quest 生命周期及回调已迁移；剩余脚本/商店动作和完整包序待完成。 |
| P8 Group/Hero/Pet/Mount/Social | Complete | Group、Hero、普通战斗宠物、Mount、好友/黑名单、婚姻和导师体系已完成并有领域、协议、会话及持久化证据。 |
| P9 Guild/War/Territory/Conquest | In progress | 公会核心、仓库、进度/Buff、战争、领地和完整 Conquest runtime/NPC/assets 子簇已迁移；P9 其余范围尚未统一收口。 |
| P10 Mail/Market/Auction/Rental/GameShop | In progress | 五个主要经济功能簇均已有协议、事务、在线通知、持久化和竞态测试，但阶段仍未宣告完整。 |
| P11 Fishing/Awakening/Ranking/Intelligent Creature/Misc | In progress | Fishing、Awakening、Ranking、Intelligent Creature 功能簇已迁移；聊天物品链接已按库存/解锁 Storage/已召唤 HeroInventory 授权并展开定义；`RequestUserName`/`UserName` 已补齐全局角色名查询、缺失静默和协议探针；共享 HarvestMonster carcass 生命周期与 Deer AI=1/2 逃跑移动已迁移；其他 miscellaneous 系统待补。 |
| P12 恢复/备份/部署 | In progress | 已有生产化 TCP startup/shutdown/restart smoke 与 Legacy checkpoint 恢复；完整 restart equivalence、备份与部署仍待完成。 |

## 已交付的重要能力

- 纯 Go 迁移工具：`crystal-legacy-account-export`、`crystal-legacy-world-export`、`crystal-protocol-probe`。保留的 C# 工具仅作历史基线。
- Go 服务端已贯通账户/角色、地图与可见性、NPC/Quest、物品与经济、Group/Hero/Pet/Mount/Social、Guild/Conquest、Mail/Market/Auction/Rental/GameShop、Fishing/Awakening/Ranking/Intelligent Creature 等大量功能簇。
- 协议测试使用 Legacy ordinal、精确 payload 和固定向量；关键跨玩家功能使用真实 `net.Pipe` 多会话接收者矩阵。
- `RequestUserName`/`UserName` 已按 Legacy 的 UInt32 角色索引、全局离线角色查询、缺失无响应语义和 .NET 字符串布局接入服务端，并由协议向量、认证探针和 Game 阶段会话测试锁定。
- Legacy 数据可通过纯 Go 账户/世界导出器进入 Go JSON 权威状态，并覆盖重登与 restart 恢复路径。

关键里程碑提交：

- `9084e6c feat(p5): migrate immediate self-buff runtime`
- `f0c768c feat(p5): migrate self-buff combat runtime`
- `c9f8264 feat(p5): migrate FrostCrunch poison states`
- `4e4d811 feat(p5): migrate delayed player area magic`
- `0758293 feat(p9): migrate conquest boundary effects and archer AI`
- `1b055eb feat(p9): migrate conquest runtime`
- `16c8a8a feat(p8): complete group compatibility`
- `9b5bbed feat(p8): migrate heroes`
- `0cf4a44 feat(p8): migrate mounts and ordinary pets`
- `03f9b2b feat(p11): migrate intelligent creatures`
- `0e17233 feat(p11): migrate fishing awakening and rankings`
- `225e855 feat(p11): migrate user name lookup`
- `d7bc163 feat(p5): migrate TrapHexagon spell`
- `1126c84 feat(p5): migrate archer summon spells`
- `213b92e feat(p5): migrate CaveMaggot AI`
- `b747d76 feat(p5): migrate CannibalPlant AI`
- `ff39426 feat(migration): add Go legacy account exporter`
- `cb4b899 feat(migration): add Go legacy world exporter`
- `296f1c1 feat(migration): add Go protocol probe`
- `03dad46 feat(p11): migrate harvest lifecycle`
- `93b1d77 feat(p5): migrate Deer AI`
- `a25713a feat(p5): migrate EvilCentipede AI`
- `ff1cbd3 feat(p5): migrate RootSpider and BombSpider AI`
- `789022b feat(p5): migrate WoomaTaurus AI`
- `2215a6f feat(p5): migrate RedMoonEvil AI`
- `aa61479 feat(p5): migrate Shinsu AI`
- `fe56473 feat(p5): migrate KingScorpion AI`
- `9d15529 feat(p5): complete RightGuard and LeftGuard AI`
- `f04430f feat(p5): complete MinotaurKing AI`
- `7df022c feat(p5): complete Yimoogi AI`
- `149c60d feat(p5): complete TrapRock AI`
- `8eefcf2 feat(p5): complete ThunderElement and GreatFoxSpirit AI`
- `e29e1f9 Migrate Dragon EvilMir and EvilMirBody`
- `71323ce test(p5): verify Dragon session compatibility`
- `f53167c feat(p5): complete DragonStatue AI`
- `b374fd5 feat(p5): complete HumanWizard AI`
- `60a97c9 feat(p5): complete Trainer AI`
- `7e5d6f3 feat(p5): complete Football AI`
- `a76c2de feat(p5): complete PoisonHugger AI`
- `ab72623 feat(p5): complete Hugger AI`

- AI=57 TownArcher 批次已由 Go 提交 `2c5171a` 收口；`town_archer.go` 与 `town_archer_test.go` 已覆盖 Legacy admission/search/route/attack/impact 调用链和 production-entry world tests。
- AI=59 HumanAssassin 批次已由 Go 提交 `a9488a3` 收口：`human_assassin.go` 及 DarkBody/ordinary-pet/world/hero 接线与测试；Legacy `HumanAssassin`/`DarkBody` Spawn、ProcessAI、Walk、Attack、Delayed ACAgility、ExplosionDie、GetInfo、logout capture 调用链已核对，Go 通过确定性 world tests 与认证 DarkBody `net.Pipe` transcript。
- AI=67 DarkDevourer 批次已由 Go 提交 `3649bc7` 收口：`dark_devourer.go` 与 common-population/action 接线；Legacy Player/owned-Monster/Hero projection、Cell insertion-order search、GM/Hidden/CoolEye/Hallucination gates、inclusive `ViewRange`、plain-AC melee、same-cell/remote `ObjectRangeAttack`、Luck-aware DC/SC、Effect=1 Green poison、delayed impact 与 map/target revalidation 均由确定性 production-entry world tests 覆盖。
- AI=68 Football 批次已由 Go 提交 `7e5d6f3` 收口：`football.go`/`football_test.go` 与 common-population、player/Hero、warrior、delayed range/magic、ordinary-pet 和 death 接线；Legacy 四次推球、无效地形反向、阻挡消耗尝试、边界停止、计时器重置、命中时移动、miss 不移动、无伤害和 no-op Die 均由 deterministic production-entry world tests 覆盖。
- AI=69 PoisonHugger 批次已由 Go 提交 `a76c2de` 收口，后由 `0e558d0` 修正搜索门禁：`poison_hugger.go`/`poison_hugger_test.go` 与 common-population、继承搜索/生命周期、Player/owned-Monster/Hero Cell-order 目标投影、严格五分钟 expiry、同格死亡、五格范围、1/5 ranged admission、失败分支双重移动、ObjectRangeAttack、距离×50ms+500ms ACAgility 延迟、半秒死亡爆炸逐目标 DC capture、当前目标重验和 Green poison TickAt 接线；production-entry tests 覆盖 expiry equality、玩家/宠物/Hero 死亡爆炸、随机/移动/毒物/value-map writeback，并通过普通 `-count=10` 与 race `-count=3` 定向门禁。
- AI=70 Hugger 批次已由 Go 提交 `ab72623` 收口：`hugger.go`/`hugger_test.go` 与 common population、继承搜索/生命周期、邻接 `ObjectAttack`、300ms delayed ACAgility、严格五分钟 expiry、单个 +500ms impact-time radius-one death action、hidden target、Player/owned-Monster/Hero projection、逐目标 DC/SC poison、早停和 value-map writeback；production-entry tests 通过普通 `-count=10` 与 race `-count=3`。

## 最近完成的 P5 批次

Go 提交 `e29e1f9`/`71323ce` 完成 Dragon、EvilMir 与 EvilMirBody 的生产行为和真实会话验证；`f53167c` 完成 AI=54 `DragonStatue`，`b374fd5` 完成 AI=55 `HumanWizard`，`60a97c9` 完成 AI=56 `Trainer`：

- DragonStatue 保留 common population、出生/respawn 方向钳制、永久静止但继承 push/blocking，以及 Player/owned-Monster/Hero/Hallucination 目标搜索；
- DragonStatue 锁定 300ms 动作、500ms live-target 半径二 MAC 延迟攻击、逐目标 DC/Luck、Shock-after-queue、睡眠后已排队命中、Struck 无伤害与 SpellObject poison follow-ups；
- DragonStatue 的睡眠目标仍可见且可被选中，但致死只进入严格 15 分钟睡眠，不发送 `ObjectDied`，不掉落、不清理、不减少 respawn population，restart 后不持久化睡眠；
- HumanWizard 注册 common/ordinary-pet population，保留固定 Down 方向、六格 ThunderBolt、Fear/撤退、主人跟随、Player/Monster/Hero 投影与 300ms/500ms 延迟边界；
- HumanWizard 的伤害、毒伤和治疗通过 `ChangeHP` 转移给主人 MP；主人每秒扣 10 MP，耗尽后宠物以 `ObjectDied(Type=1)` 清理；owner class/gender/hair/light/weapon/armour/wing 外观通过 `ObjectPlayer` 投影，首次 Spawned 的 `Extra=false`；
- HumanWizard 的 Mirroring、inspection owner resolution、logout/relogin persistence 及真实认证 `net.Pipe` spawn/logout/relogin transcript 已覆盖。
- Trainer 保留 common population 但静态禁止移动、攻击和 roam；玩家、Hero、owned pet、延迟 ranged/magic 及 queued owned-monster 命中均走独立 AC/MAC 边界，命中不改变 HP/death/loot/普通 struck-health 副作用；Trainer chat、AttackBonus、miss、毒伤/DPS、owner-root、攻击者切换和严格 `>5s` 平均输出均由确定性 world tests 锁定。Hero 使用自身 ObjectID/AttackBonus 且不把 Trainer chat 泄露给 owner；链式 owned monster 可解析到根玩家；Healing 通过真实 `resolveMassHealingActionLocked` 的 `ChangeHP` 统计路径验证，未生成满血 Trainer 的伪 regen。
- TownArcher 保留 Legacy 的 Player-only 红名搜索、GM/Hidden/CoolEye/等级门禁、十格含边界/同格范围、FearTime 与 CanAttack 严格时序、无路不移动/越界恢复出生方向、ObjectRangeAttack，以及 500ms+距离×50ms 的延迟 ACAgility 命中；`town_archer_test.go` 覆盖 production-entry 门禁、payload、impact 重验和 route movement。
- HumanAssassin AI=59 保留 common/ordinary-pet population、主人派生战斗属性/外观、严格两秒 Spawn boundary、两格优先/一格 fallback movement、stacking search、近战 ACAgility delayed impact、累计 500 DC death(Type=2)、严格 `now > 10s` 的 16-cell owner explosion、召回爆炸及 logout non-persistence；world tests 覆盖 movement packet type、impact invalidation、插入顺序/plain AC geometry、阈值死亡和 persistence gate。

AI=57 批次已由 Go 提交 `2c5171a` 收口，AI=59 已由 Go 提交 `a9488a3` 收口，AI=68 已由 Go 提交 `7e5d6f3` 收口，AI=70 已由 Go 提交 `ab72623` 收口，AI=74 已由 Go 提交 `4239ef3` 收口；本批未修改任何 `.cs` 文件。Go 功能提交与 Legacy 文档提交分开进行。主 Agent 使用 `gpt-5.6-sol/high`，只读 review subagents 使用 `gpt-5.6-luna/max`；该模型拆分已写入 `agents.md`。

## 历史质量门禁（AI=74 及更早批次）

AI=55 HumanWizard/Mirroring、AI=56 Trainer、AI=57 TownArcher、AI=59 HumanAssassin、AI=67 DarkDevourer、AI=68 Football、AI=70 Hugger 与 AI=74 LightTurtle 定向门禁通过：

- `go test ./cmd/crystal-server -run 'Trainer|HumanWizard' -count=5 -timeout=300s`（AI=56 基线）
- `go test -race ./cmd/crystal-server -run 'Trainer|HumanWizard' -count=5 -timeout=300s`（AI=56 基线）
- `go test ./cmd/crystal-server -run 'TownArcher' -count=5 -timeout=300s`
- `go test -race ./cmd/crystal-server -run 'TownArcher' -count=5 -timeout=300s`
- `go test ./cmd/crystal-server -run 'DarkBody|HumanAssassin' -count=10 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'DarkBody|HumanAssassin' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'DarkDevourer' -count=10 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'DarkDevourer' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'Football' -count=10 -timeout=300s`
- `go test -race ./cmd/crystal-server -run 'Football' -count=3 -timeout=300s`
- `go test ./cmd/crystal-server -run 'PoisonHugger|Hugger' -count=10 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'PoisonHugger|Hugger' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -run 'LightTurtle' -count=10 -timeout=600s`
- `go test -race ./cmd/crystal-server -run 'LightTurtle' -count=3 -timeout=600s`
- `go test ./cmd/crystal-server -count=1 -timeout=900s`（失败于既有 `TestSessionOmaMageRangeSlowFrozenTranscript` 的随机边界 `[2 1]` vs `[1]`；单独 `-count=10` 亦复现，未进入 AI=74）
- `go test ./... -count=1 -timeout=900s`（同一既有 OmaMage 随机边界失败；其余包通过）
- `go test -race ./... -count=1 -timeout=900s`（失败于既有 `TestGuildBuffSessionNewbieLoginReplacesStalePersistedBuff` 的 `player_spell_buffs.go`/`intelligent_creature_items.go` 共享 session race，以及 `TestSessionBlinkTranscriptIncludesDelayedMapChangeEffectAndBuff` 的 buff/packet race；均未进入 AI=74）
- `go vet ./...`
- `go build ./...`
- `gofmt`、`git diff --check` 与两仓 tracked/staged/untracked `.cs` 零变化门禁

无排除项的普通全仓测试本次未通过，失败归因已记录为既有 OmaMage 随机边界；完整 race 的当前失败归因已记录为既有 GuildBuff 与 Blink buff/packet 并发问题，均未进入 AI=74 代码或测试；未修改无关模块掩盖失败。

## 历史建议的迁移线（已由当前 ArcherSummon 批次覆盖）

AI=56/57/59/68/69/70/71/72/73/74 已完成（AI=74 已提交）；下一条仍优先继续 P5，因为最近批次已经建立了稳定的魔法/Buff/延迟动作/状态生命周期基础设施：

1. 继续 P5 通用 monster AI，按 `docs/migration-matrix.md` 中下一个仍 pending
   的 AI/target sub-slice 排序；AI=57 TownArcher 与 AI=59 HumanAssassin 已完成，下一批逐项确认可生成入口、隐藏/移动/目标
   门禁和攻击 resolver；AI=68 Football、AI=71 Behemoth、AI=72 FinialTurtle、AI=73 TurtleKing、AI=74 LightTurtle 已完成；随后继续按矩阵选择仍 pending 的 AI/target sub-slice；RootSpider/BugBag 已在此前批次完成。
2. 每批从一个可独立验证的 AI 行为簇开始，同时覆盖公式、冷却、延迟动作/目标重验、玩家/宠物/Hero/Monster 投影、包序和 net.Pipe transcript；不要仅凭怪物名称推断行为。
3. 对已完成的玩家法术差集继续保持机械校验；若发现真实主动入口缺口，再从 Legacy 施法入口、命中 resolver、Buff/Poison 创建点建立准确的 `spell -> effect -> side effects` 表后成批迁移。
4. 之后再处理 P5 的 fly/wall validation、高级 PvP/Group combat、persistent respawn state 和完整 packet-order closure。

如果下一 Session 决定切换阶段，应以 `docs/migration-matrix.md` 中明确写出的 pending 项为准，不要从 README 的概述自行推导缺口。

## 新 Session 启动清单

```sh
cd /Users/wszf/Dropbox/source_code/git_work/me_work/Crystal
cat agents.md
cat tasks/lessons.md
cat tasks/migration-handoff.md
git status --short --branch
git log -1 --oneline

cd /Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer
git status --short --branch
git log -8 --oneline
sed -n '1,180p' docs/migration-matrix.md
go test ./cmd/crystal-server -run '^$' -count=1
```

如果 compact 后 Goal 仍为 active，则沿用同一个 Goal，从本 handoff 的恢复点开始；仅当系统没有可恢复的 Goal 状态时才按上文目标重建。不要仅因新 Session 或 compact 创建新 Goal，也不要修改任何 C# 文件。

## 每批提交前固定检查

Go 仓库：

先根据 `git status --short` 的完整清单，只对本批实际修改的 `.go` 文件运行 `gofmt -w`；不要把 C# 文件或猜测的 glob 传给 Go 工具。随后运行：

```sh
go test ./...
go test -race ./...
go vet ./...
go build ./...
git diff --check
git diff --name-only -- '*.cs'
git diff --cached --name-only -- '*.cs'
git ls-files --others --exclude-standard '*.cs'
git status --short
```

Legacy 仓库：

```sh
git diff --check
git diff --name-only -- '*.cs'
git diff --cached --name-only -- '*.cs'
git ls-files --others --exclude-standard '*.cs'
git status --short
```

暂存时必须以 `git status --short` 为完整清单，不能只依赖 `git diff --name-only`，因为后者不包含未跟踪的新 Go 文件。功能和文档提交后再次检查两个工作树，并记录提交哈希。

# Crystal Go 迁移 Session 交接

最后更新：2026-08-23（Asia/Singapore；管理员 GameMaster startup 切片已提交收口）

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

## 最近完成批次（管理员 GameMaster startup，已收口）

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

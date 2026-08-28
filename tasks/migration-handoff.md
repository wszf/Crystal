# Crystal Go migration current handoff

Last updated: 2026-08-29 04:30 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-TELEPORT-ACTIONS-001` is accepted Complete in Go commit
  `2983b87ec6b4cd8aa6a8ef2352bea51de2cc0f74`. The unique next Active leaf is dependency-ready
  `QUEST-P7-REWARD-RATES-001` at matrix row 195, ledger Q row 217 and P7 summary
  row 966. P7 is frozen at 24 children: 14 Complete, 1 Active and 9 Ready.
- Reward Rates read-only Go auditor
  `01a049f4-744f-78a1-8171-f84e5689f0ac` returned a bounded report and is
  closed. Legacy tracer `01a049f4-5508-7141-b89c-ce5ab66ee2f3` and replacement
  arithmetic auditor `01a04a0a-6617-7052-b789-ec175be7f363` were unresponsive
  after interrupt and are closed without evidence; main independently reread
  the exact source/JIT ranges. No agent or test process is active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `76a25154b110d8313c77ca964fdc7fc8e193f406`;
  upstream `origin/master`, ahead 509 and behind 0.
- Unstaged owned paths are `tasks/migration-active.md` and this handoff. There
  are no staged or untracked paths and no Legacy implementation changes.
- `tasks/check-migration-control.sh`, `git diff --check`, and tracked/staged/
  untracked Legacy C# gates exit 0 before this refresh.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `2983b87ec6b4cd8aa6a8ef2352bea51de2cc0f74`;
  no upstream; index and worktree are clean.
- Commit `2983b87` contains exactly the twelve owned Teleport/parser/Flow/world/
  session/matrix paths. `git diff --check` and tracked/staged/untracked
  Go-repository C# gates exit 0.

## Active leaf and protected work

- Active leaf: `QUEST-P7-REWARD-RATES-001`.
- Teleport's committed seven-key parser/runtime/session implementation includes
  exact instance/random/time/group behavior,
  transition/rental/persistence ordering, current-session location projection,
  remote mount authority separation and immutable recipient-ID capture.
- Reward Rates owns new bounded `quest_rewards.go` and tests,
  `internal/config/{config.go,config_test.go}`, minimum `main.go` finish-quest
  wiring and `quests_session_test.go`. It must preserve Single-precision
  Gold/Experience scaling, ExpRate default/INI/env, Credit/item non-scaling and
  packet/persistence order. Complete Quest Core/level-up/item/protocol and the
  committed Teleport paths are protected.
- Frozen arithmetic ruling: both operands and multiplication use IEEE Single.
  The unchecked `conv.u4` result is source-language unspecified out of range;
  the Legacy net8 x64 JIT widens uint conversion to signed 64-bit
  `cvttss2si`, then observes the low 32 bits. Go will emulate that x64 baseline
  explicitly on every host, including modulo-2^32 finite overflow and zero for
  NaN/infinity/values outside signed-64 conversion range.

## Verification ledger

- `gofmt -d` on all owned Go files and touched-package compile exit 0.
- Focused Teleport/parser/rental production tests count-50 and focused race
  count-3 exit 0; Control Flow/rental regressions count-10/race count-3 exit 0.
- Mounted remote-group and authenticated instance/group-recall tests ordinary
  and race count-20 exit 0. Exact `INSTANCEMOVE` and `GROUPRECALL` production
  entries, persistence and packet order are covered.
- Final fresh `go test -count=1 ./...`, `go vet ./...`, `go build ./...`, and
  `go test -race -count=1 ./...` exit 0. An earlier pre-review full-race run hit
  only the established Kirin fixture race; exact isolated race count-3 and two
  later fresh unexcluded full-race runs passed. The failed run remains recorded.
- Review findings were adjudicated against exact Legacy source: active
  Revelation correctly includes self via `Map.Broadcast`; valid session-location,
  remote-local authority and teardown-ID findings were fixed. Revision 3 is
  `No findings`.

## Exact recovery sequence

1. Commit only the active-index/handoff authority expansion after Legacy
   control/diff/status/C# gates; verify both worktrees.
2. Implement only the registered config/helper/main/session scope: ExpRate
   default/INI/env, explicit Single/x64 reward conversion, unscaled Credit/items,
   unique multi-stack item IDs and exact finish-session order/persistence.
3. Run the full leaf gate and terminal review before matrix closure.

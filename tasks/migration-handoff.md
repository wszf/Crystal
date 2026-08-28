# Crystal Go migration current handoff

Last updated: 2026-08-28 16:29 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-CONTROL-FLOW-001` is Complete in Go commit
  `8b39791ae9e0a660c3cb77661df86a2e09c71a91`. The unique next Active leaf is
  dependency-ready `NPC-P7-ACTION-STATE-001`.
- Exact next matrix scope is row 184, routing ledger F row 207 and P7 summary
  row 966 only. P7 remains frozen at 24 children: 9 Complete, 1 Active and 14
  Ready.
- No subagent remains open. Terminal reviewer
  `01a04744-35d7-7350-89b0-7d371aa18764` withdrew its only provisional BREAK
  concurrency finding after tracing every production entry under the global
  flow mutex; no unwrapped production runner exists.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-documentation HEAD
  `c8209d99109404aab2c7fa246d13ec16524fd3c9`; upstream `origin/master`, ahead
  500 and behind 0.
- Current owned unstaged documents are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths.
- The active index routes Control Flow Complete to Action State Active. Lessons
  preserve the flow/input/BREAK/roll/full-gate evidence and the repeated missing
  `rg` first-probe workflow failure.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `8b39791ae9e0a660c3cb77661df86a2e09c71a91`;
  no upstream. The index and worktree are clean.
- Commit `8b39791 Complete P7 NPC control flow` changes bounded `main.go`,
  `default_npc.go`, parser/protocol files, required existing session fixtures
  and `docs/migration-matrix.md`; it adds
  `npc_script_flow.go`, `npc_script_flow_test.go`,
  `npc_control_flow_session_test.go`, and
  `internal/worlddata/npcscript_controlflow_test.go` (24 paths, 3,463 insertions,
  878 deletions).
- Matrix row 183 is Complete, row 184 is the sole Active row, ledger E contains
  accepted evidence, and P7 summary is exactly 9+1+14.

## Completed Control Flow behavior

- Production ordinary/default runners share one delayed/immediate queue model,
  route group calls by stored group order, preserve the Legacy forward-removal
  scan quirks and have no artificial 128-page/job cap.
- GOTO/CALL/BREAK, DELAYGOTO/GROUPGOTO and ROLLDIE/ROLLYUT preserve raw parameter
  resolution, malformed-tail abort, RNG/state/wire order, fake-clock delay,
  action-before-SAY/response/special ordering and cross-script/page state.
- Every detached page call consumes `%INPUTSTR`; queued input pages request input
  before page lookup. Parsed page copies share race-safe BREAK carry, while the
  global flow mutex preserves Legacy's single work-loop interval across the
  originating call and immediate scan.
- `ServerRoll=273` and its .NET-shaped payload/parser are covered by fixed wire
  vectors. Existing action-bearing NPC session fixtures now assert action
  packets before the terminal `NPCResponse` instead of their old approximation.

## Verification ledger

- Owned gofmt and `git diff --check` passed.
- Minimum compile:
  `go test ./internal/worlddata ./internal/protocol ./cmd/crystal-server -run '^$' -count=1`
  exit 0.
- Focused final repeated gate over parser/protocol/unit/production Control Flow
  tests with `-count=10` exited 0 for all three packages.
- Focused race gate over the same corpus with `-race -count=3` exited 0 for all
  three packages.
- `go test ./cmd/crystal-server -count=1` exit 0; server 86.007s.
- Fresh `go test -count=1 ./...` exit 0; server 85.013s.
- Fresh `go vet ./...` and `go build ./...` both exit 0.
- Fresh unexcluded `go test -race -count=1 ./...` exit 0; server 97.385s.
- Earlier broad attempts failed only at archived
  `TestSessionHallucinationTranscript`; isolated count-1/count-10 and both final
  fresh unexcluded full runs passed. No unrelated production change was made.
- Both repository tracked/staged/untracked `.cs` gates were empty before this
  handoff refresh; rerun them immediately before each commit.

## Active leaf and protected work

- Active leaf: `NPC-P7-ACTION-STATE-001`.
- Next Go authority is bounded parser fields in
  `internal/worlddata/npcscript.go`, bounded runtime adapters in
  `cmd/crystal-server/{main.go,default_npc.go}`, new
  `npc_script_actions_state.go` and tests, narrowly required session fixtures,
  matrix/index/handoff.
- Complete Load, Grammar, Speech/Input and Control Flow are protected. Social,
  world and teleport actions, conditions, callbacks, unrelated protocol layouts
  and every `.cs` are forbidden.

## Exact recovery sequence

1. Rerun the Legacy control checker/status/`.cs` gates, then commit exactly the
   five owned Legacy documentation paths. This handoff observes Legacy HEAD
   `c8209d99109404aab2c7fa246d13ec16524fd3c9` immediately before that expected
   documentation-only commit delta.
2. Verify both worktrees clean. Read only matrix row 184, ledger F row 207 and
   summary row 966; search the archive only for Action State and the 14 named
   keywords, then freeze the exact Legacy checklist before implementation.

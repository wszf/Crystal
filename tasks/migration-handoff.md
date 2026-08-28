# Crystal Go migration current handoff

Last updated: 2026-08-28 18:32 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-ACTION-STATE-001` is Complete in Go commit
  `83a37867942edc42d780edacb1083db4bfebd13c`. The unique Active leaf is
  dependency-ready `NPC-P7-COND-LOCAL-001`.
- Exact matrix scope is row 187, routing ledger I/J row 210 and P7 summary row
  966 only. P7 remains frozen at 24 children: 10 Complete, 1 Active and 13
  Ready.
- The context-handoff gate stopped and closed read-only workers
  `01a047e9-e803-7cc1-992d-1fab127e0fab` and
  `01a047ea-0b05-7822-bf54-a4562f713aa6`; neither changed files.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `25d94781093dc6caacac4d2a12b46eab4cf70f06`;
  upstream `origin/master`, ahead 502 and behind 0.
- HEAD is documentation-only commit `Route P7 NPC local conditions migration`,
  covering `tasks/lessons-archive/misc.md`, `tasks/migration-active.md` and the
  preceding handoff.
- Immediately before this refresh the index and worktree were clean. Current
  owned unstaged paths are `tasks/lessons.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`, and this handoff;
  there are no staged or untracked paths and no implementation work here.
- Post-refresh control checks caught an oversized active lesson and two invented
  required-heading variants. Historical detail was archived, the active rule
  condensed, and the exact checker headings restored. The final control check
  must exit 0 before recovery. All three C# gates were empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `83a37867942edc42d780edacb1083db4bfebd13c`;
  no upstream. The index and worktree are clean.
- Commit `83a3786 Complete P7 NPC action state` changes bounded
  parser/protocol/world/flow/default/main wiring, adds Action State runtime and
  parser/protocol/session tests, and updates the matrix.
- Matrix row 184 is Complete, row 187 is the sole Active row, ledger F contains
  accepted evidence, and P7 summary is exactly 10+1+13.
- No local-condition implementation or tests have started. There is no
  uncommitted Go work and no owned Go path currently modified.

## Verification ledger

- Owned gofmt and `git diff --check` passed.
- Final focused parser/protocol/runtime/ordinary/default/Control Flow corpus with
  `-count=10` exited 0 for all three packages; focused `-race -count=3` also
  exited 0.
- Fresh final `go test -count=1 ./...`, `go vet ./...`, `go build ./...`, and
  fresh unexcluded `go test -race -count=1 ./...` each exited 0.
- The first full attempt failed at over-broad unexported segment-state equality
  plus the archived OmaMage `[2 1]`/`[1]` fixture. Segment ownership was fixed;
  OmaMage isolated `-count=10` passed, and every final unexcluded gate passed.

## Active Local Conditions evidence

- Legacy read-only trace found parser minima/defaults and runtime branches in
  `Server/MirObjects/NPC/NPCSegment.cs`; call paths are in `NPCScript.cs`.
- The seven keys are `CHECKNAMELIST`, `CHECKGUILDNAMELIST`, `ISADMIN`,
  `CHECKCALC`, `ISNEWHUMAN`, `CHECKTIMER`, and `HASGT`.
- Name-list paths resolve below `Settings.NameListPath`; parsing creates parent
  directories and retains only existing files. Player list membership is exact
  and case-sensitive. `ISADMIN` uses `IsGM`; `ISNEWHUMAN` fails above one
  account character.
- Legacy has no `HasGT` switch case, so parsed `HASGT` succeeds. Timer lookup is
  global-first then local; a present timer exits its branch early, while the
  absent-timer comparison consumes `param[0]` and leaves `param[2]` unused.
  Territory ownership authority is `GTRent > DateTime.Now`.
- Go read-only trace found generic condition parsing/evaluation in
  `internal/worlddata/npcscript.go`; none of the seven is implemented and the
  default evaluator currently fail-closes. Ordinary/default context wiring in
  `cmd/crystal-server/main.go` and `default_npc.go` lacks the seven authorities.
  Existing Action State timer/CALC state is reusable authority, not yet exposed
  through condition context.

## Active leaf and protected work

- Active leaf: `NPC-P7-COND-LOCAL-001`.
- Write authority is bounded seven-key parsing/evaluation in
  `internal/worlddata/npcscript.go`, bounded local context adapters in
  `cmd/crystal-server/{main.go,default_npc.go}` and a new
  `npc_script_context.go`, focused tests, matrix/index/handoff.
- Complete loading, page grammar, Speech/Input, Control Flow and Action State
  are protected. The 24 world conditions, actions, callbacks, scheduler,
  unrelated context/protocol surfaces and every `.cs` are forbidden.

## Exact recovery sequence

1. Read this handoff and `tasks/migration-active.md`, then separately verify the
   Legacy HEAD/status/control/C# gates and Go HEAD/status/C# gates. Do not trust
   the compact summary over these checks.
2. Read only matrix row 187, ledger I/J row 210 and P7 summary row 966. Search
   only matching Local Conditions archive sections; then finish the bounded
   Legacy seven-key pass/fail/exception checklist before implementing.
3. Resume `NPC-P7-COND-LOCAL-001`; first recovery command in the Go root is
   `git status --short`, followed by exact-range reads of the already identified
   parser/context/runtime symbols. Do not open a new discovery leaf.

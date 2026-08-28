# Crystal Go migration current handoff

Last updated: 2026-08-28 18:26 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-ACTION-STATE-001` is Complete in Go commit
  `83a37867942edc42d780edacb1083db4bfebd13c`. The unique next Active leaf is
  dependency-ready `NPC-P7-COND-LOCAL-001`.
- Exact next matrix scope is row 187, routing ledger I/J row 210 and P7 summary
  row 966 only. P7 remains frozen at 24 children: 10 Complete, 1 Active and 13
  Ready.
- No subagent remains open. Terminal reviewer
  `01a047ca-c519-77f2-a76a-6e537e9c7a87` found four integration/message issues
  over two revisions; all were repaired or source-adjudicated, and final
  revision returned `No findings`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-documentation HEAD
  `8dba3bd82b7f413712d74ef35bd05374ef457fe0`; upstream `origin/master`, ahead
  501 and behind 0 at startup.
- Current owned unstaged documents are `tasks/lessons-archive/misc.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths.
- The active index routes Action State Complete to Local Conditions Active.
  Historical evidence records parser/runtime return separation, source-proven
  unsafe I/O/INI/arithmetic quirks, item authority, clock/message repairs and
  final gates.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `83a37867942edc42d780edacb1083db4bfebd13c`;
  no upstream. The index and worktree are clean.
- Commit `83a3786 Complete P7 NPC action state` changes bounded
  parser/protocol/world/flow/default/main wiring, adds Action State runtime and
  parser/protocol/session tests, and updates the matrix (13 paths, 1,717
  insertions, 41 deletions).
- Matrix row 184 is Complete, row 187 is the sole Active row, ledger F contains
  accepted evidence, and P7 summary is exactly 10+1+13.

## Completed Action State behavior

- All fourteen PARAM/MOV/CALC/value-file/random-text/timer/UI/experience/unequip
  branches preserve exact parser minima, per-line skips, runtime tail abort,
  one resolved snapshot and action-before-response order.
- PARAM state is shared only with its direct future MONGEN consumer. Value and
  random files preserve BOM/newlines, parser-time creation, restart and exact
  replacement/INI/path/failure quirks; random draw/order and localized server
  messages carry the current queued page.
- Local/global timers preserve key, type, Int32 time arithmetic, packet
  asymmetry, equality expiry, relogin lifetime and restart loss. CanGainExp is
  runtime-only and resets on entry.
- UNEQUIPITEM rebases latest auth item authority, applies the revision to world
  and session, preserves slot/cursed/wedding/riding/full-bag restrictions,
  reports accepted moves, sends full slot arrays before stat notifications and
  is covered by a stale-session production transcript.

## Verification ledger

- Owned gofmt and `git diff --check` passed.
- Minimum touched-package compile passed; final focused parser/protocol/runtime/
  ordinary/default/Control Flow corpus with `-count=10` exited 0 for all three
  packages.
- Final focused race over the same corpus with `-race -count=3` exited 0 for all
  three packages.
- Fresh final `go test -count=1 ./...` exit 0; server 81.796s.
- Fresh final `go vet ./...` and `go build ./...` both exit 0.
- Fresh final unexcluded `go test -race -count=1 ./...` exit 0; server 100.225s.
- The first full attempt failed at over-broad unexported segment-state equality
  plus the archived OmaMage `[2 1]`/`[1]` fixture. Segment ownership was fixed;
  OmaMage isolated `-count=10` passed, and every final unexcluded gate passed.
- Both repository tracked/staged/untracked `.cs` gates were empty before the Go
  commit and this handoff refresh; rerun them immediately before the Legacy
  documentation commit.

## Active leaf and protected work

- Active leaf: `NPC-P7-COND-LOCAL-001`.
- Next authority is bounded seven-key parsing/evaluation in
  `internal/worlddata/npcscript.go`, bounded local context adapters in
  `cmd/crystal-server/{main.go,default_npc.go}` and a new
  `npc_script_context.go`, focused tests, matrix/index/handoff.
- Complete loading, page grammar, Speech/Input, Control Flow and Action State
  are protected. The 24 world conditions, actions, callbacks, scheduler,
  unrelated context/protocol surfaces and every `.cs` are forbidden.

## Exact recovery sequence

1. Rerun the Legacy control checker/status/`.cs` gates, then commit exactly the
   three owned Legacy documentation paths. This handoff observes Legacy HEAD
   `8dba3bd82b7f413712d74ef35bd05374ef457fe0` immediately before that expected
   documentation-only commit delta.
2. Verify both worktrees clean. Read only matrix row 187, ledger I/J row 210 and
   summary row 966; search the archive only for Local Conditions and the seven
   named keywords, then freeze the exact Legacy checklist before implementation.

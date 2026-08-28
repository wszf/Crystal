# Crystal Go migration current handoff

Last updated: 2026-08-29 07:56 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `QUEST-P7-ACCEPT-CARRY-QUIRK-001` is accepted Complete in Go commit
  `2a3ccc677aebef3f54e5df209a17b0ff857621ba`. P7 is frozen at 24 children:
  18 Complete and 6 dependency-blocked Ready.
- The unique next Active leaf is dependency-ready `STORAGE-P2-NPC-GATE-001` at
  matrix row 139 and P2 summary row 912. P2 is frozen at eight children: seven
  Complete and one Active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-control-commit HEAD
  `08f34e58934e062492734206853e8ef912771cfc`; upstream `origin/master`, ahead
  513 and behind 0.
- Owned unstaged paths are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/migration-active.md` and this handoff. There are no staged/untracked
  paths or Legacy implementation changes.
- The active lesson now makes the authenticated account-ID grammar explicit;
  archived evidence records the rejected hyphenated fixture and the corrected
  production-entry rerun. The workflow archive records the corrected control
  timestamp assumption.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `2a3ccc677aebef3f54e5df209a17b0ff857621ba`;
  no upstream; index and worktree are clean.
- Commit `2a3ccc6` contains exactly five owned carry runtime, unit/session-test
  and matrix paths. It removes detached preflight, preserves sequential gains,
  capacity Chat, descending recalculation/delete, retained/deleted authority,
  no Add/callback, and logout/JSON/relogin continuity.
- Go tracked/staged/untracked C# queries and `git diff --check` were empty before
  commit.

## Active leaf and protected work

- Active leaf: `STORAGE-P2-NPC-GATE-001`.
- The leaf owns only final ordinary-NPC authorization at all three storage-
  password handlers: wrong-stage keepalive, current authorized `[@STORAGE]`
  transition, ordinary visible runtime object, exact key/map/range state and
  response flags.
- Completed P7 NPC access/page grammar, P2 storage-password service/protocol,
  P6 account storage, carry/quest behavior, item capacity/economy, protocol and
  persistence authorities are protected inputs.
- Allowed Go writes are bounded storage branches in `cmd/crystal-server/main.go`,
  existing storage-only session tests, one new
  `cmd/crystal-server/p2_storage_npc_gate_test.go`, and control/matrix evidence.

## Verification ledger

- Carry owned gofmt, touched-package compile, `git diff --check`, exact focused
  count 1 and count 20, protected quest regression count 3, and focused race
  count 3 all exited 0.
- Authenticated retained/deleted transcripts prove `GainedQuestItem` -> capacity
  Chat -> optional descending `DeleteQuestItem`, no ChangeQuest/default callback,
  exact world/auth state, logout JSON reload and relogin projection. Unit tests
  also prove one-definition later-stack failure, multiple-definition first-
  failure stop and reverse slot deletion.
- The initial session run failed before AcceptQuest because hyphenated account
  IDs violated `[A-Za-z0-9]{3,15}`; after fixture correction the exact suite
  passed. This is fixture attribution, not a production regression.
- Fresh `go test -count=1 ./cmd/crystal-server` passed in 82.597s. Fresh
  unexcluded `go test -count=1 ./...` passed with server 85.342s; `go vet ./...`
  and `go build ./...` passed. Full race is not cadence-due; no excluded run is
  represented as full race evidence.
- Legacy tracer `01a04aab-b52e-7b91-94bd-a340e87375eb` supplied bounded source
  leads that main reread at the exact C# ranges. Terminal reviewer
  `01a04abf-067f-7f50-a3dd-28238c292ce3` returned `No findings`; both are closed.
  No migration subagent or owned `go`/`crystal-server` process remains active.

## Exact recovery sequence

1. Run `tasks/check-migration-control.sh`, Legacy diff/status and all three C#
   gates; commit only the five owned lesson/index/handoff paths, then verify both
   repositories independently.
2. Resume only `STORAGE-P2-NPC-GATE-001`: read matrix row 139 and P2 summary row
   912, then search targeted archived lessons for storage/NPC gate/session
   failures.
3. Trace the exact Legacy wrong-stage and ordinary-NPC authorization call chain
   for all three storage-password handlers and compare the bounded Go branches
   before any implementation write; freeze the finite 16/17 and stale-state
   response matrix first.

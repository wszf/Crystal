# Crystal Go migration current handoff

Last updated: 2026-08-29 07:06 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `QUEST-P7-PROGRESS-QUIRKS-001` is accepted Complete in Go commit
  `a55295174fb08859ef0ae83db67f6f42a5a6faa1`. The unique next Active leaf is
  dependency-ready `QUEST-P7-ACCEPT-CARRY-QUIRK-001` at matrix row 198, ledger T
  row 220 and P7 summary row 966. P7 is frozen at 24 children: 17 Complete,
  1 Active and 6 Ready.
- Read-only Luna auditor `01a04a94-2245-7ff3-8252-302fe41b5fa1` remained
  unresponsive after interrupt and was closed without evidence. This was stated
  before local execution; no substitute model was used. Main terminal source
  review found and repaired the registered quirks and found no remaining defect.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-control-commit HEAD
  `2a10818a34cdefd3ebe6e901d27eb029037a7d08`; upstream `origin/master`, ahead
  512 and behind 0.
- Owned unstaged paths are `tasks/lessons-archive/misc.md`,
  `tasks/migration-active.md` and this handoff. There are no staged/untracked
  paths or Legacy implementation changes.
- Legacy tracked/staged/untracked C# queries were empty before routing; control
  and all three C# gates remain to rerun after this handoff replacement.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `a55295174fb08859ef0ae83db67f6f42a5a6faa1`;
  no upstream; index and worktree are clean.
- Commit `a552951` contains exactly five owned progress runtime, default-NPC,
  unit/session-test and matrix paths. Go tracked/staged/untracked C# queries and
  `git diff --check` were empty before commit.

## Active leaf and protected work

- Active leaf: `QUEST-P7-ACCEPT-CARRY-QUIRK-001`.
- Progress Quirks now preserves zero-mask rejection for valid classes, mask-31
  acceptance, imported unknown-class switch fallthrough, identity admission but
  raw case-sensitive name counting across different item indexes, and first-
  quest monotonic flag processing with the strict terminal 999/adjacent 998
  boundary and no-op Update persistence across relogin.
- Accept Carry owns only sequential carry-stack admission when a later stack
  fails capacity: prior gains/reports, recalculate/delete order, no Quest Add or
  default callback, and retained/deleted logout persistence. Quest Core,
  Lifecycle Timer, Progress Quirks, item-grid, rewards, protocol and persistence
  authorities are protected.

## Verification ledger

- Owned gofmt, `git diff --check`, touched-package compile, focused progress plus
  lifecycle/default-NPC count-10 and focused race count-3 exit 0.
- The authenticated production transcript proves zero-class Chat rejection,
  valid-mask acceptance, same-name different-index carry counting after target
  pickup, terminal/adjacent default-NPC SET Updates, raw authority, logout JSON
  reload and two relogin projections.
- The first name-count count-20 run exposed a nil sparse-grid slot dereference;
  the lookup now follows the nil guard and the exact suite passes count-20.
- Fresh server package tests pass in 84.676s. Fresh unexcluded
  `go test -count=1 ./...` passes, latest server 84.383s; `go vet ./...` and
  `go build ./...` pass. Full race remains fresh and is not cadence-due.
- No migration subagent remains active. No owned Go/crystal-server test process
  remained when the batch was committed.

## Exact recovery sequence

1. Run `tasks/check-migration-control.sh`, Legacy diff/status and all three C#
   gates; commit only the three owned lesson/index/handoff paths, then verify
   both repositories independently.
2. Resume only `QUEST-P7-ACCEPT-CARRY-QUIRK-001`: read matrix row 198, ledger T
   row 220 and P7 summary row 966, then search targeted archived lessons.
3. Trace Legacy AcceptQuest's sequential carry loop, capacity/report/recalculate
   call chain and exact packet/persistence outcomes before any Go write; freeze
   the two retained-versus-deleted failure transcripts.

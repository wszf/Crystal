# Crystal Go migration current handoff

Last updated: 2026-08-29 06:47 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `QUEST-P7-LIFECYCLE-TIMER-001` is accepted Complete in Go commit
  `39e82d282763f7825b3020aebc78cdd889d1f682`. The unique next Active leaf is
  dependency-ready `QUEST-P7-PROGRESS-QUIRKS-001` at matrix row 197, ledger S
  row 219 and P7 summary row 966. P7 is frozen at 24 children: 16 Complete,
  1 Active and 7 Ready.
- Read-only Luna reviewers `01a04a68-4d43-7c61-a2ad-1306ad286af8` and
  `01a04a80-5330-7ef3-8429-ad6ad55f31fc` remained unresponsive after interrupt
  and were closed without evidence. Main terminal review found and repaired the
  one-shot relogin marker defect and found no remaining lifecycle defect.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-control-commit HEAD
  `0586e422f19162a5f3bc9fa8d4b7e4a566e525dd`; upstream `origin/master`, ahead
  511 and behind 0.
- Owned unstaged paths are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`, `tasks/migration-active.md` and this handoff.
  There are no staged/untracked paths or Legacy implementation changes.
- Control and Legacy tracked/staged/untracked C# gates are pending final rerun
  after this handoff replacement; the immediately preceding three C# queries
  were empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `39e82d282763f7825b3020aebc78cdd889d1f682`;
  no upstream; index and worktree are clean.
- Commit `39e82d2` contains exactly fourteen owned lifecycle runtime, producer,
  test and matrix paths. Go tracked/staged/untracked C# queries were empty before
  the commit and `git diff --check` passed.

## Active leaf and protected work

- Active leaf: `QUEST-P7-PROGRESS-QUIRKS-001`.
- Lifecycle Timer now preserves dynamic strict-24-hour `New`; DateTime binary
  continuity; completion ExpireTimer before Add SetTimer/Update Change;
  RemoveTimer before Remove Change; zero-task/due double transitions; exact
  equality expiry; all kill/item/flag completion producers; logout/JSON reload;
  and same-connection relogin without replaying a preparation completion.
- Progress Quirks owns only zero `RequiredClass` admission, item-task matching by
  name versus definition identity, the terminal/adjacent flag-loop boundary and
  their direct relogin projection in bounded quest runtime/main/default-NPC
  surfaces. Quest Core, Lifecycle Timer, item-grid, protocol, rewards and
  persistence authorities are protected; partial carry admission remains a
  separate Ready leaf.

## Verification ledger

- Owned gofmt, `git diff --check`, touched-package compile and the complete
  focused lifecycle/producer count-10 suite exit 0. The same focused suite under
  race count-3 exits 0; the authenticated preparation-completion relogin test
  count-20 exits 0.
- Main review caught stale `questBootstrapCompletions` state: SelectCharacter
  reset it, but first StartGame did not consume it. The one-shot helper plus a
  real logout/StartGame transcript now prove first Expire/Set/Change and later
  Set/Change only.
- Final fresh unexcluded `go test -count=1 ./...` exits 0, latest server 82.937s.
  Two earlier final-state attempts hit only archived Hallucination closed-pipe
  timing and map-hazard HP 9/8 regen timing; exact Hallucination count-1 and
  map-hazard count-20 passed before the successful fresh rerun.
- Final `go vet ./...` and `go build ./...` exit 0. Full race remains fresh from
  Teleport and is within cadence; no broad concurrency change made it due.
- No owned Go/crystal-server process remains. PID 43959 was an unrelated Go test
  under `/Users/wszf/.codex-box/Crystal/claude/jobs/...`, outside both migration
  repositories, and was not touched.

## Exact recovery sequence

1. Run `tasks/check-migration-control.sh`, Legacy diff/status and all three C#
   gates; commit only the four owned control/lesson paths, then independently
   verify both repository HEADs and clean status.
2. Resume only `QUEST-P7-PROGRESS-QUIRKS-001`: read matrix row 197, ledger S row
   219 and P7 summary row 966, then search targeted archived lessons.
3. Trace the exact Legacy class-mask, item-name and flag-loop boundaries before
   any Go write; freeze the finite checklist and delegate one bounded Luna wave.

# Crystal Go migration current handoff

Last updated: 2026-08-29 03:55 (Asia/Singapore)

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
- Terminal reviewer `01a049d2-c2ce-7472-aaeb-8e356fd41be6`
  (`luna_worker`, `gpt-5.6-luna/max`) accepted revision 3 with `No findings` and
  is closed. The earlier unresponsive reviewer is also closed. No agent or test
  process remains active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `5250f1068d15da6a84b49deaa6c9dcdcd2d36f78`;
  upstream `origin/master`, ahead 508 and behind 0.
- Unstaged owned paths are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`,
  `tasks/lessons-archive/verification/race-and-flake-attribution.md`,
  `tasks/lessons-archive/workflow/repository-boundaries-02.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths and no Legacy implementation changes.
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
- Reward Rates owns only Gold/Experience rate arithmetic and Credit/item
  non-scaling at existing quest completion call sites plus bounded tests and
  minimum session wiring. Complete Quest Core/config/level-up/item/protocol and
  the committed Teleport paths are protected.

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

1. Rerun Legacy control/diff/status/C# gates and commit only the six owned
   control/lesson paths. Verify both worktrees clean and read back both HEADs.
2. Resume only `QUEST-P7-REWARD-RATES-001`: search targeted archived lessons,
   read matrix row 195/ledger Q/summary only, freeze the exact Legacy reward
   checklist, then delegate bounded tracing/tests before any Go write.

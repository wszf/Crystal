# Crystal Go migration current handoff

Last updated: 2026-08-26 04:07 (Asia/Singapore)

This replace-in-place snapshot reconstructs the durable state after recovery
found an uncommitted mining implementation that was absent from the previous
handoff. It preserves that work, resumes the same Goal and claims no leaf,
phase or project closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `gpt-5.6-luna/max` through `luna_worker` without silent substitution.
- P6 is scope-frozen In progress: eleven of nineteen children are Complete,
  `MINE-P6-RUBBLE-001` is the unique Active leaf and seven are Ready.
- Go matrix anchors are P6 summary row 901, Active registry row 3206 and
  completed administrator-item evidence 3263-3282. Independent recovery reads
  found matrix/index parity.
- The Active Index freezes the exact fifteen-file Go write set and finite
  parser/map/RNG/timer/wire/payout/persistence checklist. Do not widen it.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed pre-handoff-refresh HEAD:
  `5642e91d26567f575cc75ee2a5961223dad01bde`
  (`Freeze P6 mining implementation scope`).
- The worktree and index were clean before this handoff reconstruction. The
  expected only Legacy change is this handoff; recovery must compare the
  recorded pre-documentation HEAD with actual HEAD/status.
- Tracked, staged and untracked Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `9e7edac7aaf22f35677f90427ca79da4e998b096`
  (`Complete P6 administrator item commands`).
- The index is unstaged. Exact tracked modifications are:
  - `cmd/crystal-server/main.go`
  - `cmd/crystal-server/world.go`
  - `internal/legacyworld/database.go`
  - `internal/legacyworld/export.go`
  - `internal/legacyworld/export_test.go`
  - `internal/legacyworld/map_schema_test.go`
  - `internal/legacyworld/reader_settings_test.go`
  - `internal/legacyworld/settings.go`
  - `internal/protocol/packet.go`
  - `internal/protocol/packet_test.go`
  - `internal/worlddata/world.go`
  - `internal/worlddata/world_test.go`
- Exact untracked files are:
  - `cmd/crystal-server/mine.go`
  - `cmd/crystal-server/mine_session_test.go`
  - `cmd/crystal-server/mine_test.go`
- These fifteen files exactly match the frozen ownership set. Preserve every
  byte until the main agent reviews and verifies it; do not reset, stash,
  checkout, clean, delete, move or overwrite the batch.
- Tracked, staged and untracked Go `.cs` gates are empty.

## Uncommitted mining implementation

- Recovery observed 495 tracked-file insertions/deletions plus 739 lines in the
  three new files. The batch appears to implement Mines settings/export, map
  mine state, the Mine `MapEffect` adapter, Rubble/runtime actions and focused
  parser/world/session tests.
- This description is status-level reconstruction only. The main agent has not
  yet accepted the diff, re-read every Legacy ruling needed for review, or
  established that all frozen checklist items are correct.
- A previously returned read-only Luna tracing report was used to freeze the
  checklist, but its ID was not preserved in the durable control documents; no
  active writer or reviewer is claimed. A process audit found no `go` or
  `crystal-server` process, and two status snapshots must remain stable before
  verification resumes.

## Verification ledger

- No compile, focused, repeated, race, integration or full-race result for the
  current uncommitted mining implementation is accepted yet.
- Recovery-only `git diff --check` in the Go repository exited 0.
- Go status and mtimes were captured once at 04:06; no Go/server process was
  present. Recheck stability after this handoff write before running tests.
- The prior committed administrator-item leaf had fresh full gates, but those
  results predate this shared schema/protocol/world batch and cannot satisfy its
  mandatory immediate integration/full-race gate.

## Active leaf and protected work

- Active leaf: `MINE-P6-RUBBLE-001`.
- Owned files are exactly the fifteen paths listed above and in the Active
  Index. All other Go files, every earlier migration commit and all C# files in
  both repositories remain protected.
- Unresolved decisions are limited to findings from main-agent diff/Legacy
  review. Any required file expansion must first revise and validate the Active
  Index; no implicit expansion is allowed.

## Exact recovery sequence

1. Verify both repositories separately, including exact status and all six C#
   gates; read this handoff and the Active Index, not historical handoffs.
2. In Go, re-read only matrix rows 901, 3206 and 3263-3282. Confirm the fifteen
   file status/mtime set is stable and no writer/process remains.
3. Main-review the full owned diff against the frozen checklist and bounded
   Legacy entry points; correct only proven findings within frozen ownership.
4. Run gofmt, touched-package compile, focused production-entry tests at
   `-count=20`, focused race at `-count=5`, then the immediately due fresh
   unexcluded full test/vet/build/full-race gates.
5. Obtain one bounded read-only Luna terminal review, resolve findings, update
   matrix/index/handoff, commit only owned files and route the next ready P6
   child.

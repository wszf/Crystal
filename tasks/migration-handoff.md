# Crystal Go migration current handoff

Last updated: 2026-09-03 08:42 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB export composition is Complete in Go
  `4656357d49f9dffbf1eec85729b6fa45fd29411b`. SaveDelay MirDB wiring remains
  unselected because JSON exports do not carry magics.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD
  `5a63a1cc7c0c4f3373f122d2b4e4194d946a6e90`.
- Active index records the completed MirDB export-composer batch and is pushed
  to origin. No Legacy worktree changes are pending.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `4656357d49f9dffbf1eec85729b6fa45fd29411b`, pushed to origin.
- Only `docs/migration-matrix.md` has the pending evidence edit for this batch.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldExport now maps a loaded world JSON export into Server.MirDB,
  including Dragon and respawn snapshots.
- Do not wire SaveDelay to this composer while magics would be written empty.
  All `.cs` remains read-only.

## Verification ledger

- Export-composer plus prior catalog writer tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat export composition as a completed input, not SaveDelay MirDB wiring.
3. Continue owner tracing for remaining restart-equivalence. Do not write C#.

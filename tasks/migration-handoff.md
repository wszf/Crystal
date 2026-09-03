# Crystal Go migration current handoff

Last updated: 2026-09-03 08:45 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. WriteWorldExport now merges existing MirDB magics,
  counters and quest FileNames in Go `2334662bc9b6f9883137a87104b05d09e9c8d965`.
  SaveDelay MirDB path selection remains unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `5a63a1cc7c0c4f3373f122d2b4e4194d946a6e90`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `2334662bc9b6f9883137a87104b05d09e9c8d965`.
- Unrelated unstaged file (not this leaf): `docs/migration-matrix.md`. Preserve it.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldExport parses an existing Server.MirDB first so magics, editor
  counters and quest FileNames survive a JSON-export rewrite.
- Do not wire SaveDelay without an explicit MirDB path. All `.cs` remains
  read-only.

## Verification ledger

- Preserve-magics plus prior catalog writer tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Preserve unstaged matrix edits.
   Resume only `DISC-P12-CLOSURE`.
2. Treat export merge as a completed input, not SaveDelay MirDB wiring.
3. Continue owner tracing for remaining restart-equivalence. Do not write C#.

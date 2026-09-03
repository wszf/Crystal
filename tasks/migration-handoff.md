# Crystal Go migration current handoff

Last updated: 2026-09-03 09:20 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  complete restart-equivalence. `WriteWorldExport` merges existing MirDB
  magics, counters and quest FileNames; SaveDelay now has a bounded MirDB
  production caller in Go `9904aef`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD is the control-doc update pending
  below this handoff. No `.cs` file is modified, added, deleted or renamed.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD `7781073`, with production SaveDelay
  wiring in `9904aef`; both commits are pushed to origin.
- The SaveDelay database step is guarded by a configured world-export path,
  backs up and rewrites that JSON catalog, then writes Legacy's fixed CWD-relative
  `./Server.MirDB` through `WriteWorldExport`. An empty world path does not emit
  a CWD MirDB file.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- `WS-PERSIST-P12-SAVEDELAY-MIRDB-001` is Complete only for this bounded write
  caller and its SaveDelay store order; it does not close restart-equivalence.
- Existing-target MagicInfo/editor counters/quest filenames are preserved by the
  export merge. New-target authority for those sections, backup/restore,
  manifest, rollback, crash recovery, runtime ObjectID and sidecar ownership
  remain open/shared-owner questions.
- All Legacy `.cs` files remain permanently read-only.

## Verification ledger

- SaveDelay order/path tests pass at focused `-count=20` and focused race
  `-count=5`, including the no-world-path no-MirDB guard.
- `gofmt`, `git diff --check`, `go vet ./...`, and `go build ./...` pass on the
  Go tree. The matrix records the bounded production caller and keeps
  `PERSIST-P12-RESTART-EQUIV-001` Open/shared-owner.

## Exact recovery sequence

1. Push Go commits `9904aef` and `7781073`, then push the Legacy control-doc
   commit after updating this handoff and the active index.
2. Re-run both repository status/C# gates and `tasks/check-migration-control.sh`;
   preserve any unrelated `tasks/lessons.md` modification.
3. Treat the SaveDelay MirDB caller as a completed bounded input, not complete
   MirDB persistence/recovery. Resume only `DISC-P12-CLOSURE` and select the next
   dependency-ready owner; do not write C# or reopen completed leaves.

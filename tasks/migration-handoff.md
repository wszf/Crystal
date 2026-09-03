# Crystal Go migration current handoff

Last updated: 2026-09-03 10:04 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  complete restart-equivalence. `WriteWorldExport` merges existing MirDB
  magics, counters and quest FileNames; SaveDelay now has a bounded MirDB
  production caller in Go `9904aef`; graceful shutdown store persistence is in
  Go `55cf0b5`, with its error boundary in `e63a540`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD `fbc3fd9d`, with the current
  active/handoff update pending this batch's control-plane commit. No `.cs` file
  is modified, added, deleted or renamed.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD `163514c`, with production error
  boundary in `e63a540`, shutdown writers in `55cf0b5`, SaveDelay wiring in
  `9904aef`, and matrix evidence in `163514c`; these commits are ready to push.
- The SaveDelay database step is guarded by a configured world-export path,
  backs up and rewrites that JSON catalog, then writes Legacy's fixed CWD-relative
  `./Server.MirDB` through `WriteWorldExport`. An empty world path does not emit
  a CWD MirDB file.
- The final production shutdown now force-writes UsedGoods before the existing
  account checkpoint boundary, then force-writes Guilds and Conquests; the
  `serveListener` return boundary now surfaces UsedGoods/respawn persistence
  failures. This remains bounded store-order coverage, not complete recovery.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- `WS-PERSIST-P12-SHUTDOWN-ERROR-BOUNDARY-001` is Complete only for surfacing
  final world-runtime persistence failures; `WS-PERSIST-P12-SHUTDOWN-WORKLOOP-001`
  and `WS-PERSIST-P12-SAVEDELAY-MIRDB-001` remain completed inputs.
- Existing-target MagicInfo/editor counters/quest filenames are preserved by the
  export merge. New-target authority for those sections, backup/restore,
  manifest, rollback, crash recovery, runtime ObjectID, retry-after-exit and
  complete multi-store recovery remain open/shared-owner questions.
- All Legacy `.cs` files remain permanently read-only.

## Verification ledger

- SaveDelay order/path tests pass at focused `-count=20` and focused race
  `-count=5`, including the no-world-path no-MirDB guard; shutdown persistence
  and production error-boundary tests pass at focused `-count=20`/race `-count=5`.
- `gofmt`, `git diff --check`, `go vet ./...`, and `go build ./...` pass on the
  Go tree. The matrix records both bounded callers and keeps
  `PERSIST-P12-RESTART-EQUIV-001` Open/shared-owner.

## Exact recovery sequence

1. Re-run both repository status/C# gates and `tasks/check-migration-control.sh`;
   push Go commits `e63a540`/`163514c` and the next Legacy control commit after
   this handoff commit.
2. Keep the active index and handoff synchronized with the next bounded workstream;
   preserve any unrelated `tasks/lessons.md` modification.
3. Treat the SaveDelay and shutdown callers as completed bounded inputs, not complete
   MirDB persistence/recovery. Resume only `DISC-P12-CLOSURE` and select the next
   dependency-ready owner; do not write C# or reopen completed leaves.

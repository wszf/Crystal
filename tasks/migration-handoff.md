# Crystal Go migration current handoff

Last updated: 2026-09-03 10:23 (Asia/Singapore)

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
  Go `55cf0b5`, with world/account/periodic error boundaries in
  `e63a540`/`dea8d24`/`0acab15`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD `fbc3fd9d`, with the current
  active/handoff update pending this batch's control-plane commit. No `.cs` file
  is modified, added, deleted or renamed.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD `efba6ff`, with periodic error context
  in `0acab15`, account boundary in `dea8d24`, world boundary in `e63a540`,
  shutdown writers in `55cf0b5`, SaveDelay wiring in `9904aef`, and matrix
  evidence in `efba6ff`; the current production/doc commits remain to be pushed.
- The SaveDelay database step is guarded by a configured world-export path,
  backs up and rewrites that JSON catalog, then writes Legacy's fixed CWD-relative
  `./Server.MirDB` through `WriteWorldExport`. An empty world path does not emit
  a CWD MirDB file.
- The final production shutdown now force-writes UsedGoods before the existing
  account checkpoint boundary, then force-writes Guilds and Conquests; the
  production result surfaces UsedGoods/respawn failures and JSON-only account
  export failures while preserving the configured 117 retry. This remains
  bounded store-order coverage, not complete recovery.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- `WS-PERSIST-P12-PERIODIC-ERROR-CONTEXT-001` is Complete only for naming the
  first failing SaveDelay store while retaining all ordered attempts;
  account/world/shutdown/SaveDelay callers remain completed inputs.
- Existing-target MagicInfo/editor counters/quest filenames are preserved by the
  export merge. New-target authority for those sections, backup/restore,
  manifest, rollback, crash recovery, runtime ObjectID, retry-after-exit and
  complete multi-store recovery remain open/shared-owner questions.
- All Legacy `.cs` files remain permanently read-only.

## Verification ledger

- SaveDelay order/path tests pass at focused `-count=20` and focused race
  `-count=5`, including the no-world-path no-MirDB guard; stage-context,
  shutdown persistence, world-error and JSON-only account-error tests pass at
  focused `-count=20`/race `-count=5`.
- `gofmt`, `git diff --check`, `go vet ./...`, and `go build ./...` pass on the
  Go tree. The matrix records the bounded callers and keeps
  `PERSIST-P12-RESTART-EQUIV-001` Open/shared-owner.

## Exact recovery sequence

1. Re-run both repository status/C# gates and `tasks/check-migration-control.sh`;
   push Go commits `0acab15`/`efba6ff` and the next Legacy control commit after
   this handoff commit.
2. Keep the active index and handoff synchronized with the next bounded workstream;
   preserve any unrelated `tasks/lessons.md` modification.
3. Treat the SaveDelay and shutdown callers as completed bounded inputs, not complete
   MirDB persistence/recovery. Resume only `DISC-P12-CLOSURE` and select the next
   dependency-ready owner; do not write C# or reopen completed leaves.

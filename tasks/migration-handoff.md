# Crystal Go migration current handoff

Last updated: 2026-09-03 (after path-alias closure; Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  complete restart-equivalence. `WS-PERSIST-P12-PATH-ALIAS-001` is complete in Go
  `fce9835` with matrix evidence in `e3561df`; fixed-store alias validation is
  complete in Go `e0507a0`, and the next active bounded workstream is
  `WS-PERSIST-P12-DERIVED-RUNTIME-ALIAS-001`. Existing SaveDelay, shutdown, world,
  account, periodic and sidecar boundaries remain bounded inputs, not complete recovery.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD
  `227a87239bab06b92bb8386c3da94d43dc9b9d47`. Only the active/handoff control
  documents are modified for this batch; no `.cs` file is modified, added,
  deleted or renamed.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `e3561df36e66d6dd3ea4d974e3955db40d243369`, pushed to
  `origin/migrate/drop-owner-p12`. Path-alias production code is in
  `fce983582619f70fdacbbffef7a9a32235f55448`; matrix evidence is in `e3561df`.
- `Config.Validate` and the production startup seam now reject whitespace-only
  optional paths, pairwise clean-absolute aliases, and cross-authority aliases to
  fixed CWD `Server.MirDB`/`Server.MirADB`; intentional Legacy-account → MirADB
  mapping remains allowed. The next slice guards the derived runtime-sidecar path.
- SaveDelay/shutdown account, world, Guild, Goods, Conquest and runtime-sidecar
  callers remain bounded store-order coverage, not complete recovery.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- `WS-PERSIST-P12-SIDECAR-ERROR-CONTEXT-001` is Complete only for logging
  runtime-sidecar write failures while retaining dirty/retry semantics;
  periodic/account/world/shutdown callers remain completed inputs.
- Existing-target MagicInfo/editor counters/quest filenames are preserved by the
  export merge. New-target authority for those sections, backup/restore,
  manifest, rollback, crash recovery, runtime ObjectID, retry-after-exit and
  complete multi-store recovery remain open/shared-owner questions.
- All Legacy `.cs` files remain permanently read-only.

## Verification ledger

- Optional and fixed-store path validation/config/startup tests pass at focused
  `-count=20` and focused race `-count=5`; rejected aliases make zero listener
  calls and leave fixed targets absent, while the intentional Legacy-account →
  MirADB mapping remains accepted. Full Go tests and full race pass when excluding
  the four registered baselines: three world-bootstrap error expectation tests and
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` (`mail packet id = 26, want 206`).
- Unskipped full test and race runs reproduce those same four baseline failures;
  no path-alias failure or race is present. `gofmt`, `git diff --check`,
  `go vet ./...`, and `go build ./...` pass. `PERSIST-P12-RESTART-EQUIV-001`
  remains Open/shared-owner.

## Exact recovery sequence

1. Re-run both repository status/C# gates and `tasks/check-migration-control.sh`;
   commit and push this active/handoff control snapshot.
2. Resume `DISC-P12-CLOSURE` at `WS-PERSIST-P12-DERIVED-RUNTIME-ALIAS-001`:
   guard account/Legacy-account paths against the effective
   `WorldExportPath + ".runtime.json"` fallback only when RespawnStatePath is empty,
   then prove production startup rejection before load/bind/write at count 20/race 5.
3. Treat SaveDelay, shutdown and prior path-alias callers as completed bounded
   inputs, not complete MirDB persistence/recovery. Do not write C# or reopen
   completed leaves; after the derived-sidecar slice, update matrix/index/handoff
   and select the next dependency-ready workstream.

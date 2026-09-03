# Crystal Go migration current handoff

Last updated: 2026-09-03 (derived-sidecar closure; blocked at shared-owner boundary; Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  complete restart-equivalence. `WS-PERSIST-P12-PATH-ALIAS-001` is complete in Go
  `fce9835` with matrix evidence in `e3561df`; fixed-store alias validation is
  complete in Go `e0507a`, and derived-sidecar alias validation is complete in Go
  `84e3301`. No further dependency-ready production child is selected until a
  Legacy-backed shared recovery owner exists.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD
  `227a87239bab06b92bb8386c3da94d43dc9b9d47`. Only the active/handoff control
  documents are modified for this batch; no `.cs` file is modified, added,
  deleted or renamed.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `5035b5a4f123d389ea103550372170f8abb403fc`, pushed to
  `origin/migrate/drop-owner-p12`. Path-alias code is in `fce9835`, fixed-store
  guard in `e0507a`, derived-sidecar guard in `84e3301`; matrix evidence is in
  `5035b5a`.
- `Config.Validate` and the production startup seam now reject whitespace-only
  optional paths, pairwise clean-absolute aliases, cross-authority fixed-store
  aliases, and derived runtime-sidecar aliases; the intentional Legacy-account →
  MirADB mapping remains allowed.
- SaveDelay/shutdown account, world, Guild, Goods, Conquest and runtime-sidecar
  callers remain bounded store-order coverage, not complete recovery.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- The three path-alias guards are Complete only for startup ownership checks:
  optional pairwise paths, fixed CWD MirDB/MirADB cross-authority paths, and the
  derived runtime-sidecar fallback. Periodic/account/world/shutdown callers remain
  bounded completed inputs.
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
2. No dependency-ready P12 production child is currently available: the remaining
   `PERSIST-P12-RESTART-EQUIV-001` work spans auth/world/economy/sidecar owners and
   lacks a Legacy-backed shared recovery owner, source precedence and failure contract.
3. Do not synthesize manifest/generation/restore/rollback/crash or cross-store atomicity
   semantics. Resume only when a finite Legacy-backed owner is registered; then update
   matrix/index/handoff and continue, without writing C# or reopening completed leaves.

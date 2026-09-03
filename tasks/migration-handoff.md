# Crystal Go migration current handoff

Last updated: 2026-09-03 (dependency-ready leaf audit; P12 shared-owner blocker preserved; UTC)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. This continuation uses the user-requested
  `gpt-5.6-luna/max` for main and bounded workers; `gpt-5.6-sol` is not used.
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  complete restart-equivalence. `WS-PERSIST-P12-PATH-ALIAS-001` is complete in Go
  `fce9835` with matrix evidence in `e3561df`; fixed-store alias validation is
  complete in Go `e0507a`, and derived-sidecar alias validation is complete in Go
  `84e3301`. No further dependency-ready production child is selected until a
  Legacy-backed shared recovery owner exists.
- Read-only `WS-P12-OWNER-CANDIDATE-AUDIT-001` (`gpt-5.6-luna/max`, completed and
  closed) found no independently registerable child. Legacy `Envir.Main` owns the
  combined WorkLoop/shutdown/restart boundary (`Server/MirEnvir/Envir.cs:46-63,
  1996-2004,2147-2155,2199-2204,3294-3301`); MirDB respawn/spawn state crosses
  account loading, and GTMap/guild/conquest startup joins cross stores. Legacy
  loaders read primary files only and expose no `.bak`/`.o`/`.n` restore path.
- Non-P12 selection audit found no dependency-ready implementation leaf: P9/P10
  child registries remain discovery-only/no-child-authorized, which blocks the
  remaining P1/P4/P6/P7 candidates. No leaf was activated and no code scope opened.

## Legacy repository state

- Root: `/workspace/Crystal`.
- Branch `migration/goal-orchestration`; HEAD
  `db71a24768df0fc79a435f96350f0d38da591b2d` before this documentation-only
  refresh. The expected post-refresh state is clean: no tracked, staged or
  untracked changes, and all three `.cs` gates are empty.

## Go repository state

- Root: `/workspace/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `5035b5a4f123d389ea103550372170f8abb403fc`, pushed to
  `origin/migrate/drop-owner-p12`. The worktree is clean with no tracked,
  staged or untracked changes and all three `.cs` gates are empty. Path-alias code
  is in `fce9835`, fixed-store
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
- Agents/processes are quiescent: the bounded auditor above is closed, no writer
  agent remains, and no `go` or `crystal-server` process was observed.

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
- The owner audit's Legacy evidence confirms that a MirDB-only recovery child is
  not source-independent: account-held `SavedSpawns`, startup GTMap/guild joins,
  and primary-only loaders cross the proposed boundary. No finite Legacy-backed
  child was registered and no implementation scope was opened.

## Exact recovery sequence

1. Re-run both repository status/C# gates and `tasks/check-migration-control.sh`;
   commit and push this active/handoff control snapshot.
2. No dependency-ready P12 production child is currently available: the remaining
  `PERSIST-P12-RESTART-EQUIV-001` work spans auth/world/economy/sidecar owners and
  lacks a Legacy-backed shared recovery owner, source precedence and failure contract;
  the completed Legacy audit found no independently bounded save/load/recovery child.
3. Do not synthesize manifest/generation/restore/rollback/crash or cross-store atomicity
   semantics. Resume only when a finite Legacy-backed owner is registered; then update
   matrix/index/handoff and continue, without writing C# or reopening completed leaves.

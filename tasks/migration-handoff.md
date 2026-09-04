# Crystal Go migration current handoff

Last updated: 2026-09-04 (P9/P10 finite closure registered; P7 social leaf Active; UTC)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. This continuation uses the user-requested
  `gpt-5.6-luna/max` for main and bounded workers; `gpt-5.6-sol` is not used.
- Unique Active leaf is `NPC-P7-ACTION-SOCIAL-001`. The finite P9/P10 closure
  registries and the Ready cross-phase inputs `GUILD-P9-NPC-SCRIPT-001`,
  `CONQUEST-P9-NPC-ECONOMY-001` and `MAIL-P10-NPC-SCRIPT-001` are registered in
  Go matrix commit `5c685ac92425f5adb63d7c2b91ee8b24a99fa8e6`; P7 social implementation
  is now the routed leaf.
  P12 remains blocked/shared-owner for complete restart-equivalence.
- Read-only `WS-P12-OWNER-CANDIDATE-AUDIT-001` (`gpt-5.6-luna/max`, completed and
  closed) found no independently registerable child. Legacy `Envir.Main` owns the
  combined WorkLoop/shutdown/restart boundary (`Server/MirEnvir/Envir.cs:46-63,
  1996-2004,2147-2155,2199-2204,3294-3301`); MirDB respawn/spawn state crosses
  account loading, and GTMap/guild/conquest startup joins cross stores. Legacy
  loaders read primary files only and expose no `.bak`/`.o`/`.n` restore path.
- The bounded P9/P10 discovery aliases were reconciled to existing authoritative
  rows without duplicates: siege-objects→`CONQUEST-P9-SIEGE-OBJECT-001`,
  mail-authority→`MAIL-P10-CORE-001`, gameshop-purchase→`GAMESHOP-P10-CORE-001`,
  and account-checkpoint→`ECONOMY-P10-CHECKPOINT-001`. No P9/P10 production child
  is implementation-ready; their explicit Ready cross-phase inputs unlock P7.

## Legacy repository state

- Root: `/workspace/Crystal`.
- Branch `migration/goal-orchestration`; HEAD
  `cdcbd6c99b08f2cfb058785bf3334b3218c975a2`. Expected tracked changes for this
  routing snapshot are `tasks/migration-active.md` and
  `tasks/migration-handoff.md`; nothing staged or untracked. All three `.cs`
  gates are empty.

## Go repository state

- Root: `/workspace/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `5c685ac92425f5adb63d7c2b91ee8b24a99fa8e6` (`migration: register finite P9 and P10 closure`).
  The registration commit is local pending the implementation batch; the worktree is clean with no tracked,
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

- Active leaf: `NPC-P7-ACTION-SOCIAL-001`.
- Owned Go scope: `internal/worlddata/npcscript.go` and focused parser tests;
  bounded `cmd/crystal-server/default_npc.go`, new social action helper/tests,
  and production-session tests. The 12 actions are the two guild-gold, six
  name-list, and four scripted-mail operations; all `.cs` files remain read-only.
- P9/P10 closure registration is finite and scope-frozen, but their broad child
  implementations remain non-ready or blocked by their recorded evidence gates.
- Existing-target MagicInfo/editor counters/quest filenames are preserved by the
  export merge. New-target authority for those sections, backup/restore,
  manifest, rollback, crash recovery, runtime ObjectID, retry-after-exit and
  complete multi-store recovery remain open/shared-owner questions.
- All Legacy `.cs` files remain permanently read-only.
- Active process: read-only `WS-P7-SOCIAL-LEGACY-TRACE-001` (`gpt-5.6-luna/max`)
  is running; it has no write authority. The prior P12 auditor is closed. No
  writer agent or `go`/`crystal-server` process is expected.

## Verification ledger

- Previous optional and fixed-store path validation/config/startup tests pass at focused
  `-count=20` and focused race `-count=5`; rejected aliases make zero listener
  calls and leave fixed targets absent, while the intentional Legacy-account →
  MirADB mapping remains accepted. Full Go tests and full race pass when excluding
  the four registered baselines: three world-bootstrap error expectation tests and
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` (`mail packet id = 26, want 206`).
- Unskipped full test and race runs reproduce those same four baseline failures;
  no path-alias failure or race is present. `gofmt`, `git diff --check`,
  `go vet ./...`, and `go build ./...` pass. `PERSIST-P12-RESTART-EQUIV-001`
  remains Open/shared-owner.
- The owner audit's Legacy evidence confirms that an independently bounded P12 MirDB-only recovery child is
  not source-independent: account-held `SavedSpawns`, startup GTMap/guild joins,
  and primary-only loaders cross the proposed boundary. No finite Legacy-backed
  child was registered and no P12 implementation scope was opened. The current P7 social
  leaf has not yet run its focused gate; registration commit
  `5c685ac92425f5adb63d7c2b91ee8b24a99fa8e6` is the only new Go commit here.

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

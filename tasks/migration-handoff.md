# Crystal Go migration current handoff

Last updated: 2026-09-02 (P12 CanStart DB-check projection complete)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is complete for its registered scope with no dependency-ready
  successor; P9 closure is finite and reviewed with no production-ready child.
  P10 closure is also finite and reviewed with no production-ready child. P11 closure is
  now finite and reviewed: all residual dispatch families are routed to accepted P6-P10
  owners or explicitly excluded, with no unassigned P11-owned behavior. P12 now has a finite
  five-ID candidate registry; Account-ID, Character-ID, bounded corrupt-index continuity,
  dirty-shutdown checkpoint correction and CanStart DB-check projection are complete for their
  bounded contracts. Restart-equivalence retains its shared-owner and recovery evidence gaps.
- `WS-ITEM-P6-USE-HERO-BUFF-SEAL-LIFECYCLE-001` is Complete in Go `732f8fe`;
  this closes one bounded correction only, not the active leaf, P6 or the Goal.
- `WS-ITEM-P6-USE-BOOK-001` is Complete in Go `5779c89`; Player Inventory Book
  learning, atomic item/Magics commit and authenticated persistence/projection evidence
  are closed without reopening the catalog leaf.
- Verified correction `WS-ITEM-P6-USE-BOOK-AUTHORITY-001` is Complete in Go `349d5a0`;
  Book admission ignores only temporary Magic records, world runtime Magic authority is
  rebased before commit, stale temporary records are not persisted and active temporary
  equipment Magics still reject duplicate learning.
- `WS-ITEM-P6-USE-SCROLL-001` is Complete in Go `59b2914`; Player Inventory Scroll
  shapes 0-7 and 11-12 now have production-entry, persistence, observer and failure
  evidence without reopening completed item-use leaves.
- P9 and P10 closures are finite and reviewed without a production-ready child; both phases
  remain In progress/Frozen. P11 closure is finite and reviewed as scope-frozen Complete:
  its residual dispatch families route to existing phase owners or explicit exclusions.
  Current routing leaf is `DISC-P12-CLOSURE`; the finite P12 candidate ledger is now recorded
  in the Go matrix. Account-ID, Character-ID, bounded corrupt-index continuity and CanStart
  DB checks are complete; remaining discovery is limited to restart-equivalence.
- `WS-ITEM-P6-USE-FOOD-NONZERO-001` is Complete in Go `c620075`; Player Inventory
  Food Shape != 0 repair/report semantics, including unknown nonzero shapes, are closed.
- `DISC-P6-USE-CATALOG-CLOSURE-001` is complete: the finite Script/Transform/Deco/
  MonsterSpawn family/shape ledger, Legacy contracts, Go owners and blockers are recorded;
  no dependency-ready functional successor remains inside this leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- Observed HEAD: `e2c0f6c4` (`docs: close P12 CanStart DB-check workstream`). This handoff update is
  another Legacy control-document-only change; every Legacy `.cs` file remains untouched.
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `fb60853` (`feat: migrate P12 startup database checks`), not pushed. This commit adds
  the P12 `LegacyStartupChecks` export/projection, production startup validator, source-specific
  settings and real startup-entry tests; the matrix evidence is included in the same commit.
  Earlier shutdown, Account-ID, Character-ID and corrupt-index commits remain in history.
- The Go worktree is clean and no generated binary remains in the repo. The Legacy
  `tasks/lessons.md` change remains unrelated and uncommitted.

## Completed Hero seal Buff lifecycle evidence

- Legacy `SealHero` removes the Hero from character slots but leaves the same global
  live HeroInfo, whose Buff list is reused by `AddHero`/`SummonHero` in the same process.
  HeroInfo Save/Load still omits Buffs, so logout/relogin and restart remain loss edges.
- Go now clones a summoned Hero's Buffs and per-type clocks before successful seal
  removes its runtime. An already-unsummoned Hero keeps its existing dormant sidecar.
- DeleteHero removes only the released Hero's sidecar. Other sealed Hero sidecars owned
  by the same live session are no longer erased by broad slot-array cleanup.
- Same-owner SealedHero attach reuses the dormant state. If a sealed item is transferred
  between two online sessions, world attach atomically moves that Hero's sidecar from
  the source player to the authenticated target before summon.
- Failed/stale attach does not move the sidecar because ownership transfer runs only
  after the runtime identity guard accepts the authoritative attach result.
- Summon clones the transferred state, deletes its dormant entry, resets LastTime,
  retains NextTime, restores hidden/paused state and replays AddBuff through the existing
  ObjectHero/health/colour/AddBuff/spawn-state/UseItem ordering.
- No Buff enters Character, auth or JSON persistence; no timer, queue, global durable
  Hero store, item authority or SealedHero business branch was added.

## Verification ledger

Passed for Go `c1e73b5` (Book production correction `349d5a0`) after the P9-P11 closure evidence updates, including prior Scroll/Food and Hero seal evidence:

- summoned and unsummoned SealHero plus DeleteHero transcript/sidecar matrix at count 20;
- authenticated NPC seal→ClientUseItem→same-process reattach→AddBuff and relogin-loss
  transcript, valid cross-owner handoff, full-slot and missing-Hero failures at count 20;
- focused race for the same production paths at count 5;
- Player Book known/unknown/duplicate/count/reload authenticated session, atomic concurrent
  auth mutation, current-world Magic progress rebase, stale/active temporary Magic gates,
  Player stat-refresh and existing cooldown-preservation tests at count 20; the Book
  authority correction runs through the same production session/auth/world path.
- Player Scroll shapes 0-7 and 11-12 authenticated production-entry success/failure,
  transition, revival, balance saturation, observer and JSON persistence transcripts;
- focused Scroll tests pass at count 10 and focused race passes;
- Player Food Shape 1 and unknown positive/negative nonzero shapes through the authenticated
  mount-feed production path, including stack consumption, cap/no-loss durability semantics,
  localized `MountFed`, Item Lost reporting, JSON reload and failure gates;
- focused Food-adjacent tests pass at count 20 and focused race passes;
- P12 Account-ID focused tests pass at count 10 and focused race: production TCP raw-header
  100 → account indexes 101/102 across graceful checkpoint/restart, retained-gap re-export,
  failed/duplicate no-consume behavior, and deterministic JSON/117 checkpoint interleave;
- P12 corrupt-character-index focused tests pass at count 10 and focused race: real TCP load/login
  preserves duplicate, zero and negative indexes in physical order; first-match delete tombstones
  only the first duplicate; graceful 117 checkpoint/restart preserves the remaining records; the
  next character allocates index 100 and re-export retains the raw order and values;
- complete `cmd/crystal-server` package with the registered Quest fixture skipped;
- full `go test ./... -skip '^TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin$' -count=1`;
- final full `go test -race ./... -skip '^TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin$' -count=1`;
- `go vet ./...`, `go build ./...`, formatting, `git diff --check`, staged diff check,
  C# audits and an independent read-only correction review with no finding.
- The Book correction was triggered by a reproduced `ServerUseItem(false)` common-gate
  regression: stale `IsTempSpell` records in the session/auth snapshot no longer block
  admission, while the world runtime snapshot remains the duplicate-learning authority.

The unskipped full `go test ./... -count=1` currently retains only the registered Quest
fixture baseline: `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` fails
with `mail packet id = 26, want 206`; no Scroll test fails.

The first full-race invocation returned exit 1 in `cmd/crystal-server`, but its displayed
output was truncated before the failing test name. An immediate isolated server-race
rerun passed, followed by a complete full-race rerun that also passed. The Quest P7
fixture remains the unchanged registered baseline blocker (`mail packet id = 26,
want 206`).

## P12 Account-ID bounded closure evidence

- `PERSIST-P12-ACCOUNT-ID-001` is Complete for the ordinary positive-int32 contract. Legacy
  117 offset 8 is the used account high-watermark; Go account creation allocates the next
  positive index only after validation and duplicate checks, and successful metadata creation
  is fenced by `saveMu → s.mu`.
- Production TCP `TestProductionStartupLegacyAccountCounterCheckpointRestart` starts from raw
  header 100, creates `FirstAccount` at index 101 and `SecondAccount` at index 102, checkpoints
  both generations on graceful shutdown, preserves creation IP/date metadata, and verifies the
  retained account after restart. `TestP12AccountIDReexportRestartRetainsGap` removes index
  100, proves the checkpoint header remains monotonic, re-exports indexes 101/102, restarts,
  and proves the removed record is not reused.
- `TestP12AccountIDSerializesCreateWithJSONCheckpoint` blocks account creation during the
  JSON snapshot → 117 checkpoint hook window and verifies JSON/117 contain only the old
  generation before release. Focused repeated and focused race tests pass. JSON-authoritative
  source precedence remains unchanged; account wrap/zero/negative, corrupt duplicate-index
  normalization, backup/restore, manifest and broad restart recovery remain excluded.

## P12 Character-ID bounded closure evidence

- `PERSIST-P12-CHARACTER-ID-001` is Complete for its bounded contract. Legacy 117 offset 12
  is the used-character high-watermark; Go auth and JSON store the next-available counter.
  Import/export, bridge checkpoint and restart preserve ordinary retained gaps plus int32
  negative/zero wrap IDs. `TestP12CharacterIDSerializesCreateWithJSONCheckpoint` also holds
  `CreateCharacter`/`CreateCharacterWithMetadata` behind the SaveJSON-to-117 checkpoint window,
  so the role counter cannot be captured in different generations by that production path.
- Production TCP `TestProductionStartupLegacyCharacterCounterCheckpointRestart` authenticates
  through `ServerConnected`, `ClientVersion`, `ClientLogin` and `ClientNewCharacter`, creates
  indexes 101/102 after raw header 100, checkpoints on graceful shutdown, restarts, and verifies
  the next allocation. Bridge tests cover JSON/117 conversion, concurrent checkpoint/create,
  negative wrap record retention and zero-wrap restart continuity.
- Focused repeated and focused race tests pass. Full `go test ./...` and full `go test -race ./...`
  pass with the registered Quest baseline skipped; the unskipped suite retains only
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` (`mail packet id = 26, want
  206`). `go vet ./...`, `go build ./...`, formatting, `git diff --check` and C# audits pass.
  Production changes are committed in Go `ed71f29` (with `0156b3d` and `1c45002` as earlier
  counter/wrap commits); matrix documentation is committed in Go `dde96a3`.

## P12 corrupt-character-index bounded closure evidence

- `PERSIST-P12-CORRUPT-CHAR-INDEX-001` is Complete for the source-equivalent bounded contract.
  The Go counter-aware 117 writer preserves duplicate, zero and negative indexes in Legacy order;
  compatibility marshal zero-normalization remains outside this bridge scope.
- `TestP12CorruptCharacterIndexReexportsInPhysicalOrder` proves indexes `7, 7, 0, -1` round-trip;
  real TCP `TestProductionStartupLegacyCorruptCharacterIndexCheckpointRestart` proves selection,
  first-match delete, graceful checkpoint/restart, next-create 100 and physical-order re-export.
- `WS-PERSIST-P12-DIRTY-CHECKPOINT-001` is complete in `26a037c`: real HTTP dirty shutdown
  hook count is 1 after successful SaveJSON, and 2 when the committed JSON/hook failure requires
  explicit retry; both JSON and 117 retain the created account metadata. This correction remains
  outside broad corrupt-data recovery, backup/restore and manifest work.
- Legacy's duplicate/nonpositive index behavior is a post-migration review item, not a blocker;
  new-character allocation remains unchecked.

## P12 CanStart DB-check bounded closure

- `PERSIST-P12-CANSTART-DBCHECKS-001` is Complete for the bounded projection in Go `fb60853`: Legacy Setup/FishingSystem/RefineSystem values, exact 50-name order, strict monster lookup, normalized item lookup, first-missing/item suffixes, disabled bypass, malformed projection rejection and WorldMap-after-DB ordering are implemented.
- `TestP12CanStart*` exercises the real startup entry with listener-not-called failures and successful game/status listener opening; export/reload, source fallback and full 50-position first-missing tests pass at repeated count 20 and focused race count 5. `go test ./...` and `go test -race ./...` pass with the registered Quest fixture skipped; vet/build pass.
- Legacy default `WhiteSerpent` is retained from `Settings.cs`; old world JSON without `legacyStartupChecks` leaves the P12 gate disabled because Enforce/Fishing/Refine values are unavailable, so re-export is required for an equivalent gate.

## P12 finite candidate review

- Existing IDs in scope: `PERSIST-P12-ACCOUNT-ID-001`, `PERSIST-P12-CHARACTER-ID-001`,
  `PERSIST-P12-CORRUPT-CHAR-INDEX-001`, `PERSIST-P12-CANSTART-DBCHECKS-001` and
  `PERSIST-P12-RESTART-EQUIV-001`. No new child ID was created.
- `PERSIST-P12-ACCOUNT-ID-001` is complete for the ordinary positive-int32 contract:
  production high-header TCP, re-export/restart, retained-gap and create/checkpoint race
  evidence are closed. `PERSIST-P12-CHARACTER-ID-001` is complete for the bounded implementation
  above; `PERSIST-P12-CORRUPT-CHAR-INDEX-001` is also complete for its source-equivalent physical
  order, first-match, checkpoint/restart and next-create contract. Neither workstream enlarges P3
  ownership or authorizes broad P12 recovery.
- `DISC-P12-CLOSURE-REVIEW-002` traced Legacy `Envir.CanStartEnvir` at
  `Server/MirEnvir/Envir.cs:1925-1994`: StartPoints, 50 configured monster names,
  RefineOreName and WorldMap are checked in that order; `Settings.cs` supplies the defaults and
  auxiliary INI values. The raw P5/P6 catalogs were already complete, so P12 implemented the
  finite `LegacyStartupChecks` projection/validator without reopening P1/P5/P6.
- Legacy periodic save spans DB/accounts/guilds/goods/conquests, with account/DB backups and
  account `.n/.o` rotation (`Server/MirEnvir/Envir.cs:2147-2155,2439-2507,2550-2819`). Go
  `SaveJSON`, 117 bridge, world export and respawn sidecar are separate stores without a
  generation/manifest, backup selector/fallback or crash contract. Thus
  `PERSIST-P12-RESTART-EQUIV-001` remains Open/shared-owner, dependent on P10 economy and P1
  lifecycle/deployment; no dependency-ready successor is authorized.
- The finite persistence sub-slices are evidence partitions under the existing IDs, not new
  children. The audit found no unique closed production owner for either residual; keep production
  implementation closed during discovery.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE` (bounded discovery; P12 remains Open).
- Previous `DISC-P9-CLOSURE` and `DISC-P10-CLOSURE` are finite and reviewed; P9/P10 are
  Frozen but remain In progress because no child met the production evidence gate. `DISC-P11-CLOSURE`
  is also complete: P11 is scope-frozen Complete after every residual dispatch family was routed
  to an accepted phase owner or explicit no-op exclusion.
- Completed workstreams include `WS-ITEM-P6-USE-BOOK-AUTHORITY-001` at Go `349d5a0`,
  `WS-ITEM-P6-USE-SCROLL-001` at Go `59b2914` and `WS-ITEM-P6-USE-FOOD-NONZERO-001`
  at Go `c620075`, plus the earlier P6/P8 evidence listed in the matrix.
- The finite P12 candidate registry is recorded below the P12 summary in the Go matrix.
  `PERSIST-P12-CHARACTER-ID-001`, bounded `PERSIST-P12-ACCOUNT-ID-001`,
  `PERSIST-P12-CORRUPT-CHAR-INDEX-001` and `WS-PERSIST-P12-DIRTY-CHECKPOINT-001` are complete
  for their bounded production evidence. The next recovery point is dependency discovery for
  CanStart and restart-equivalence; no broad persistence, backup or deployment implementation
  is authorized.
- Preserve completed P2-P11 authorities, latest-auth revision/CAS and persistence-before-visible
  projection; do not infer an owner for an unassessed P12 boundary.
- Do not implement P12 broad scope, reopen completed leaves, assign unverified owners, change
  protocols or modify any C#.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only the P12 summary and finite ledger in the Go matrix, plus the active index and handoff.
3. Trace read-only Legacy checkpoint, backup, restart and deployment call chains for each
   P12 residual; do not write C#.
4. Reconcile each finite child’s wire/store format, unique Go owner, dependencies and
   focused/repeated/race/restart/backup evidence; keep unresolved gaps explicit.
5. Keep P2 account bridge, P10 economy and P1 deployment inputs separate from P12 ownership;
   no residual is ready until its own owner is verified.
6. Keep production implementation closed during discovery. Only after the registry and
   dependencies are reviewed may a dependency-ready child be frozen before Go changes.

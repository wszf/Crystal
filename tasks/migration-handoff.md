# Crystal Go migration current handoff

Last updated: 2026-09-01 (P10 closure routing)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is complete for its registered scope with no dependency-ready
  successor; P9 closure is now finite and reviewed with no production-ready child.
  The current routing batch is P10 closure discovery.
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
- P9 closure is finite and reviewed: no P9 production child met the evidence gate; P9 is
  Frozen but remains In progress. Current routing leaf is `DISC-P10-CLOSURE`, with the P10
  summary and finite ledger in the Go matrix.
- `WS-ITEM-P6-USE-FOOD-NONZERO-001` is Complete in Go `c620075`; Player Inventory
  Food Shape != 0 repair/report semantics, including unknown nonzero shapes, are closed.
- `DISC-P6-USE-CATALOG-CLOSURE-001` is complete: the finite Script/Transform/Deco/
  MonsterSpawn family/shape ledger, Legacy contracts, Go owners and blockers are recorded;
  no dependency-ready functional successor remains inside this leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD: `20222440` (`docs: route P9 closure discovery`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `7a0fb1b` (`docs: record P10 closure ledger`), not pushed; Book production code is
  in `349d5a0`.
- The committed Book/Scroll/Food migration tree is clean and no generated binary remains in the repo.

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

Passed for Go `f7c95dd` (Book production correction `349d5a0`) after the authority evidence update, including prior Scroll/Food and Hero seal evidence:

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

## Active leaf and protected work

- Active leaf: `DISC-P10-CLOSURE` (bounded discovery; P10 remains Open).
- Previous `DISC-P9-CLOSURE` is finite and reviewed; P9 is Frozen but remains In progress
  because no child met the production evidence gate.
- Completed workstreams include `WS-ITEM-P6-USE-BOOK-AUTHORITY-001` at Go `349d5a0`,
  `WS-ITEM-P6-USE-SCROLL-001` at Go `59b2914` and `WS-ITEM-P6-USE-FOOD-NONZERO-001`
  at Go `c620075`, plus the earlier P6/P8 evidence listed in the matrix.
- Active discovery will record finite P10 mail, market/auction, rental, GameShop and
  economy children in the Go matrix below P10 row 1004.
- Preserve existing P10 authorities, completed P6-P9 workstreams, latest-auth revision/CAS
  and persistence-before-visible projection.
- Do not implement P10/P11/P12 broad scope, reopen completed leaves, assign unverified owners,
  change protocols or modify any C#.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix row 1004 and the finite P10 ledger below it, plus the active index and handoff.
3. Trace read-only Legacy mail/market/auction/rental/GameShop owners and their MirConnection/
   PlayerObject/Envir dispatch; do not write C#.
4. For each finite child, record the Legacy call chain, wire/consume/persistence contract,
   unique Go owner, dependencies and focused/repeated/race/restart evidence.
5. Keep P7 script/economy inputs and P12 restart consumers separate from P10 implementation
   leaves; retain P6 item/grid boundaries and all cross-phase blockers.
6. Keep production implementation closed during discovery. Only after the registry and
   dependencies are reviewed may a dependency-ready child be frozen before Go changes.

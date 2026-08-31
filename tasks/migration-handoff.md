# Crystal Go migration current handoff

Last updated: 2026-09-01 05:50 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is the sole Active unfinished child.
- `WS-ITEM-P6-USE-HERO-BUFF-SEAL-LIFECYCLE-001` is Complete in Go `732f8fe`;
  this closes one bounded correction only, not the active leaf, P6 or the Goal.
- `WS-ITEM-P6-USE-BOOK-001` is Complete in Go `5779c89`; Player Inventory Book
  learning, atomic item/Magics commit and authenticated persistence/projection evidence
  are closed without reopening the catalog leaf.
- `WS-ITEM-P6-USE-SCROLL-001` is Complete in Go `59b2914`; Player Inventory Scroll
  shapes 0-7 and 11-12 now have production-entry, persistence, observer and failure
  evidence without reopening completed item-use leaves.
- Primary dependency-ready leaf remains `ITEM-P6-USE-CATALOG-001`, matrix row 3546
  and P6 summary row 965.
- `WS-ITEM-P6-USE-FOOD-NONZERO-001` is Complete in Go `c620075`; Player Inventory
  Food Shape != 0 repair/report semantics, including unknown nonzero shapes, are closed.
- `DISC-P6-USE-CATALOG-CLOSURE-001` is complete: the finite Script/Transform/Deco/
  MonsterSpawn family/shape ledger, Legacy contracts, Go owners and blockers are recorded;
  no dependency-ready functional successor remains inside this leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `b66d32c2`
  (`docs: advance migration handoff to scroll`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `c620075` (`Close nonzero Food item use`), not pushed.
- The committed Scroll/Food migration tree is clean and no generated binary remains in the repo.

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

Passed for Go `c620075` after the Scroll/Food commits (including prior Book and Hero seal evidence):

- summoned and unsummoned SealHero plus DeleteHero transcript/sidecar matrix at count 20;
- authenticated NPC seal→ClientUseItem→same-process reattach→AddBuff and relogin-loss
  transcript, valid cross-owner handoff, full-slot and missing-Hero failures at count 20;
- focused race for the same production paths at count 5;
- Player Book known/unknown/duplicate/count/reload authenticated session, atomic concurrent
  auth mutation, Player stat-refresh and existing cooldown-preservation tests at count 20;
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

The unskipped full `go test ./... -count=1` currently retains only the registered Quest
fixture baseline: `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` fails
with `mail packet id = 26, want 206`; no Scroll test fails.

The first full-race invocation returned exit 1 in `cmd/crystal-server`, but its displayed
output was truncated before the failing test name. An immediate isolated server-race
rerun passed, followed by a complete full-race rerun that also passed. The Quest P7
fixture remains the unchanged registered baseline blocker (`mail packet id = 26,
want 206`).

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-CATALOG-001`.
- Completed workstreams: `WS-ITEM-P6-USE-NOOP-CONSUME-001` at Go `84adba5`,
  `WS-ITEM-P6-USE-POTION-BUFF-003-001` at Go `c4f1e38`,
  `WS-ITEM-P6-USE-POTION-RATE-004-005-001` at Go `e7c5a10`,
  `WS-ITEM-P6-USE-HERO-POTION-BUFF-003-005-001` at Go `8596f22`,
  `WS-ITEM-P6-USE-HERO-BUFF-SEAL-LIFECYCLE-001` at Go `732f8fe`,
  `WS-ITEM-P6-USE-BOOK-001` at Go `5779c89`, `WS-ITEM-P6-USE-SCROLL-001` at Go
  `59b2914`, and `WS-ITEM-P6-USE-FOOD-NONZERO-001` at Go `c620075`.
- Closure workstream `DISC-P6-USE-CATALOG-CLOSURE-001` is complete; no dependency-ready
  functional successor remains inside the active leaf. Its finite ledger is in matrix row 3546.
- Preserve common item-use admission/death/riding gates, completed Book/Potion/
  SealedHero/Scroll/Food behavior, latest-auth item revision/CAS and persistence-before-visible
  projection.
- Do not implement Scroll 8-10/13-15, Food shape 0, Pets, Script, Transform, Deco,
  MonsterSpawn, spell casting/combat, protocol changes or another item authority.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3546 plus the active index and this handoff.
3. Trace read-only Legacy Script at `PlayerObject.cs:6090-6092` and
   Transform/Deco/MonsterSpawn at `PlayerObject.cs:6250-6314`, plus common tail
   `PlayerObject.cs:6330-6337` and entry/death gates `:5799-5824`; do not write C#.
4. For each remaining family/shape, record the Legacy call chain, packet/consume and
   persistence contract, current Go owner, and the blocking phase/leaf; do not infer
   dependency readiness from tests alone.
5. Confirm finite closure: Script requires P7 default callback/action-world/condition
   authorities; Transform requires P5 transform effect authority; Deco/MonsterSpawn require
   world object/spawn ownership, with conquest MonsterSpawn additionally requiring P9 state.
6. Keep production implementation closed during discovery. If a dependency-ready bounded
   workstream is proven, freeze its owner/files/gates in active index before Go changes;
   otherwise update the finite ledger and continue the persistent Goal without push or merge.

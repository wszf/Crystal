# Crystal Go migration current handoff

Last updated: 2026-08-31 21:11 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is the sole Active unfinished child.
- `WS-ITEM-P6-USE-POTION-RATE-004-005-001` is Complete in Go `e7c5a10`; this
  closes one bounded workstream only, not the active leaf, P6 or the Goal.
- Primary dependency-ready leaf remains `ITEM-P6-USE-CATALOG-001`, matrix row 3546
  and P6 summary row 965.
- Active workstream is `WS-ITEM-P6-USE-HERO-POTION-BUFF-003-005-001`: only
  HeroInventory Potion Shape 3 item-stat Buff and Shape 4/5 Exp/Drop Buff branches
  plus the existing Hero successful consume tail.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `6f649f7c` (`Record Potion Shape 3 migration`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `e7c5a10` (`Migrate Potion Shape 4 and 5 rate Buffs`), not pushed.
- The feature and matrix are committed; no generated binary remains in the repo.

## Completed Player Potion Shape 4/5 evidence

- Authenticated Player Inventory Potion Shape 4 always creates private invisible
  Exp Buff 102 with `ExpRatePercent` Stat 100; Shape 5 always creates private
  invisible Drop Buff 103 with `ItemDropRatePercent` Stat 101.
- Both project base plus added Luck without a positivity gate. Zero Luck creates an
  empty sparse Stats map and still creates/consumes the Buff; negative Luck is
  retained. Duration preserves the C# Int32 `60000 * durability` wrap.
- Shape 4/5 reuse Shape 3's latest-auth item/Buff transaction, explicit Rental
  consumption authorization, per-Buff clocks and persistence-before-visible
  projection barrier. No second timer, queue, player state or persistence owner was
  introduced.
- StackDuration retains the first Stats and existing LastTime/NextTime phase while
  adding the wrapped duration and refreshing Values. New positive Buffs keep the
  accepted one-second phase; finite nonpositive Buffs remain immediately eligible.
- `PauseInSafeZone` is applied before the initial AddBuff, so use while already safe
  projects `Paused=true` and excludes the rate Stat from derived player state.
  Existing safe-zone transition authority continues to emit PauseBuff, refresh Stats
  and persist the change.
- A shared verified correction now removes paused finite Buffs whose duration is
  already nonpositive; Paused stops duration subtraction but does not make an expired
  Buff immortal. Infinite Buffs and accepted positive-duration phase behavior remain
  unchanged.
- The production experience consumer leaves paused/negative/zero Exp unchanged and
  applies positive `ExpRatePercent`; the actual monster-drop path receives positive
  `ItemDropRatePercent` and preserves the existing Legacy divisor/overflow formula.
- Successful use persists JSON before visible success, reports one Item Lost,
  consumes once, sends AddBuff before UseItem=true, and converges auth/world/JSON and
  relogin state.

## Verification ledger

Passed for Go `e7c5a10` before commit:

- focused Shape 3-5 domain/world/authenticated session, safe-zone pause, independent
  Buff clock, deferred projection, experience consumer and monster-drop consumer at
  count 20;
- focused race for the same production paths at count 5;
- authenticated Shape 4/5 session additionally repeated three times after final
  integration;
- full `go test ./... -count=1` with the date-sensitive Quest fixture and the observed
  full-suite-only Hallucination fixture skipped; Hallucination passed in isolation at
  count 3 after its one closed-pipe full-suite failure;
- full race with that Quest fixture plus the two registered isolated-green fixtures
  `TestProductionTickerInitializesLightFromUTCOnNonUTCHost` and
  `TestSessionHallucinationTranscript` skipped;
- `go vet ./...`, `go build ./...`, formatting, `git diff --check`, staged diff check,
  control-plane checker and a final independent read-only review with no finding;
- tracked/staged/untracked `.cs` audits in both repositories returned no paths.

The Quest P7 fixture remains date-sensitive because its fixed 2026-08-29 clock is
older than the real 2026-08-31 process clock. Hallucination's full-suite closed-pipe
failure was not hidden: it was isolated green before the final full-suite skip.

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-CATALOG-001`.
- Completed workstreams: `WS-ITEM-P6-USE-NOOP-CONSUME-001` at Go `84adba5`,
  `WS-ITEM-P6-USE-POTION-BUFF-003-001` at Go `c4f1e38`, and
  `WS-ITEM-P6-USE-POTION-RATE-004-005-001` at Go `e7c5a10`.
- Active workstream: `WS-ITEM-P6-USE-HERO-POTION-BUFF-003-005-001`.
- It owns only HeroInventory Potion Shape 3-5 and the existing Hero consume tail.
  Freeze Legacy Hero Shape 3's max-only branches separately from Player pair Stats;
  Shape 4/5 may reuse the frozen Luck/rate metadata where evidence matches.
- Do not intercept Player Potion behavior, Hero Shape 0-2, Scroll, Food, Pets, Book,
  Script, Transform, Deco, MonsterSpawn, SealedHero or completed feature lifecycles.
- Preserve NoDrug and dead-Hero admission, latest-auth Hero item fencing, live Hero
  HP/MP/equipment/runtime state, and persistence-before-visible ordering.
- The independently verified nested Rental-metadata merge residual remains a separate
  shared-authority correction candidate; it does not block top-level Potion consume
  and must not be mixed into the Hero Potion workstream.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3546 plus the active index and this handoff.
3. Freeze read-only Legacy `HeroObject.UseItem` Potion Shape 3-5 at
   `Server/MirObjects/HeroObject.cs:322-550`, including max-only Shape 3 gates,
   AddBuff metadata/stacking, consume order and Hero consumer semantics.
4. Trace the current Go Hero Shape 0/1 transaction and Hero Buff authority; reuse
   shared Player Buff metadata/formulas only where contracts match.
5. Implement through authenticated `ClientUseItem` without adding another timer,
   projection queue, Hero state store or item authority.
6. Verify focused/repeated/race and integration gates, update evidence, create Go and
   Legacy commits, then continue the persistent Goal without push or merge.

# Crystal Go migration current handoff

Last updated: 2026-08-31 20:02 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is the sole Active unfinished child.
- `WS-ITEM-P6-USE-POTION-BUFF-003-001` is Complete in Go `c4f1e38`; this closes
  one bounded workstream only, not the active leaf, P6 or the persistent Goal.
- Primary dependency-ready leaf remains `ITEM-P6-USE-CATALOG-001`, matrix row 3546
  and P6 summary row 965.
- Active workstream is `WS-ITEM-P6-USE-POTION-RATE-004-005-001`: only Player
  Inventory Potion Shape 4 Exp and Shape 5 Drop Buff branches plus the shared
  successful consume tail.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `ad21f9e0` (`Record no-effect item use migration`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `c4f1e38` (`Migrate Potion Shape 3 stat Buffs`), not pushed.
- Worktree and index are clean after the feature/matrix commit.

## Completed Potion Shape 3 evidence

- Authenticated Player Inventory Potion Shape 3 now maps base plus added item Stats
  into Impact, Magic, Taoist, Storm, HealthAid, ManaAid, Defence, MagicDefence and
  BagWeight in the exact Legacy order. Pair branches retain every nonzero member,
  sparse zero keys stay absent, and duration preserves C# Int32 minute overflow.
- One latest-auth transaction consumes the item and publishes the final durable Buff
  set. Explicitly consumed Rental IDs may disappear without restoring the item while
  unrelated Rental authority remains protected; the same correction covers adjacent
  successful consume paths that shared the defect.
- Every Buff type has its own runtime LastTime/NextTime phase. StackDuration retains
  the first Stats and existing phase; a new positive Buff starts the next one-second
  boundary from use time, while wrapped nonpositive finite durations remove on the
  next eligible process.
- Runtime Buff expiry and combat effects continue during slow JSON persistence. Only
  Buff notifications are deferred, then drained behind the initial AddBuff barrier;
  failed sessions execute persistence-only notifications and never expose RemoveBuff
  before an AddBuff that was not sent.
- Each private invisible AddBuff is followed by that step's RefreshStats effects;
  nearby observers receive neither AddBuff nor RemoveBuff. Zero-stat Shape 3 still
  consumes, reports one Item Lost, refreshes bag weight and returns UseItem=true.
- Imported `MapInfo.NoDrug` now reaches world rules and rejects Player and Hero Potion
  use with localized System Chat before UseItem=false, without item, Buff or recovery
  mutation. Final independent review found and verified the HeroInventory correction.
- JSON rename completion precedes visible success. A later checkpoint-hook failure is
  logged but remains committed, Item Lost reporting occurs before socket projection,
  and auth/world/JSON/relogin converge without duplicate consumption.

## Verification ledger

Passed for Go `c4f1e38` before commit:

- focused Shape 3 domain/world/authenticated session, per-Buff clock, deferred
  projection, allowed Rental consumption, NoDrug import and Player/Hero admission,
  plus adjacent item-use/Buff paths at count 20;
- focused race for the same production paths at count 5;
- full `go test ./... -count=1` with only the known date-sensitive
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` fixture skipped;
- full race with that Quest fixture plus the two unrelated isolated-green fixtures
  `TestProductionTickerInitializesLightFromUTCOnNonUTCHost` and
  `TestSessionHallucinationTranscript` skipped;
- `go vet ./...`, `go build ./...`, formatting, `git diff --check`, staged diff check
  and terminal bounded review with its sole Hero NoDrug finding fixed and closed;
- tracked/staged/untracked `.cs` audits in both repositories returned no paths.

The known Quest P7 fixture remains date-sensitive because its fixed 2026-08-29
clock is older than the real 2026-08-31 process clock. It remains the only non-race
full-suite skip and was not hidden.

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-CATALOG-001`.
- Completed workstreams: `WS-ITEM-P6-USE-NOOP-CONSUME-001` at Go `84adba5` and
  `WS-ITEM-P6-USE-POTION-BUFF-003-001` at Go `c4f1e38`.
- Active workstream: `WS-ITEM-P6-USE-POTION-RATE-004-005-001`.
- It owns only Player Inventory Potion Shape 4 Exp and Shape 5 Drop Buff effects and
  the common consume tail. Reuse Shape 3's item/Buff transaction, per-Buff clock and
  persistence-before-visible projection barrier.
- Freeze exact Luck-to-ExpRatePercent/ItemDropRatePercent projection, Int32-wrapped
  duration, StackDuration phase, AddBuff/RefreshStats order, private expiry and the
  actual experience/drop consumers before implementation.
- Do not intercept Shape 2/3, other Potion shapes, Scroll, Food, Pets, Book, Script,
  Transform, Deco, MonsterSpawn, SealedHero or any completed feature lifecycle.
- Legacy Hero Potion Shape 3-5 remains a separately bounded catalog follow-up: current
  Go Hero potion production handles only Shape 0/1. Do not silently claim it in the
  Player workstream or broaden it into a P8 lifecycle rewrite.
- The independently verified nested Rental-metadata merge residual remains a separate
  shared-authority correction candidate; it does not block top-level Potion consume
  and must not be hidden or opportunistically mixed into the rate-Buff workstream.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3546 plus the active index and this handoff.
3. Freeze PlayerObject Potion Shape 4/5, AddBuff metadata/stacking, Exp/drop consumer
   evidence and the common tail from read-only Legacy C#.
4. Reuse current Shape 3 world/auth/session authority; do not add another timer,
   projection queue, player-state or persistence path.
5. Implement through authenticated `ClientUseItem`; verify NoDrug/riding/death gates,
   effect/consume order, private expiry, consumer behavior, JSON/relogin and race.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

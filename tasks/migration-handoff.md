# Crystal Go migration current handoff

Last updated: 2026-08-31 14:11 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is the sole Active unfinished child.
- `ITEM-P6-EXPIRY-001` is Complete in Go `e7e3b4c`; this closes one P6 leaf only,
  not P6 or the persistent Goal.
- Primary dependency-ready leaf is `ITEM-P6-USE-CATALOG-001`, matrix row 3546 and
  P6 summary row 965.
- Active workstream is `WS-ITEM-P6-USE-NOOP-CONSUME-001`: Legacy successful
  no-effect consumption for otherwise-unmatched Potion/Scroll/Pets shapes and
  every SiegeAmmo shape.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `d597ed36` (`Record storage capacity migration`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `e7e3b4c` (`Migrate item expiry lifecycle`), not pushed.
- Worktree and index are clean after the feature/matrix commit.

## Completed Item Expiry evidence

- A strict per-player one-minute schedule runs only when world time is later than
  the previous schedule point; item and rental-lock deadlines themselves are
  inclusive.
- Auth applies one atomic Inventory→Equipment→Storage scan. Inventory and
  Equipment remove expired items and release expired rental locks; Storage only
  removes expired items. `ExpireInfo` wins over rental unlock on the same item.
- Localized Hint and `DeleteItem` packets preserve Legacy order and Observer
  parity. Login enables the ticker only after the complete bootstrap transcript.
- Expired top-level and nested UniqueIDs retain process-local deletion tombstones.
  Unlock acknowledgement releases temporary protection only after an explicit
  unlocked player-grid snapshot and cannot erase a later deletion tombstone.
- Market/GameShop stale Inventory projections preserve the current protected
  container location and reject an impossible full-grid rebase rather than
  overwrite an ordinary item. Storage Store/TakeBack now share CrossGrid CAS.
- Item changes advance the shared revision and converge auth, world and session
  projections without immediately refreshing Equipment-derived live stats.
- JSON mode and checkpoint-only version-117 mode persist before the first visible
  packet, retry once, log and close the affected session on final failure.
- Target/Observer transcript, JSON reload, version-117 checkpoint/reload and
  relogin prove committed removal and rental-lock release survive restart.

## Verification ledger

Passed for Go `e7e3b4c` before commit:

- focused Item Expiry, protected Inventory, persistence helper, Rental authority
  and Storage CrossGrid tests at count 20;
- focused Item Expiry/Market/GameShop/Rental/Storage race tests at count 5;
- adjacent economy/session tests at count 20 with only the exact known Quest P7
  fixture skipped;
- full `go test ./... -count=1` with only
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` skipped;
- full race with that Quest fixture plus the two unrelated isolated-green fixtures
  `TestProductionTickerInitializesLightFromUTCOnNonUTCHost` and
  `TestSessionHallucinationTranscript` skipped;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- tracked/staged/untracked `.cs` audits in both repositories returned no paths;
- final independent diff/contract reviews found no remaining high-confidence Item
  Expiry, nested tombstone or full-grid stale-writeback defect.

The first adjacent count-20 command included the known Quest P7 fixture and failed
at `quest_progress_session_test.go:80`: its fixed 2026-08-29 ground-item clock is
now older than the real 2026-08-31 process clock, so `ObjectRemove` precedes the
expected quest-item packet. The exact same adjacent gate passes when only that
pre-existing date-sensitive fixture is skipped; the failure was not hidden.

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-CATALOG-001`.
- Active workstream: `WS-ITEM-P6-USE-NOOP-CONSUME-001`.
- Legacy authority is read-only `PlayerObject.UseItem` dispatch/tail at
  `Server/MirObjects/PlayerObject.cs:5826-6337`:
  - Potion shapes outside 0-5 reach the common successful tail with no effect;
  - Scroll shapes outside 0-15 do the same;
  - Pets shapes 29+ do the same, while every shape below 29 remains feature-owned;
  - SiegeAmmo every shape reaches the TODO branch and then the successful tail.
- The common tail decrements `Count` or clears the Inventory slot, refreshes bag
  weight, records `ItemChanged`, sets `UseItem.Success=true` and sends the response.
- Reuse common item-use admission, `commitPlayerItemMutation`, shared item revision,
  world projection and persistence. Do not create a parallel item authority.
- Do not implement Potion 0-5, Scroll 0-15, Pets below 29, Food, Book, Script,
  Transform, Deco, MonsterSpawn or SealedHero effects in this workstream.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3546 plus the active index and this handoff.
3. Reconfirm the four Legacy no-op partitions and common tail; every `.cs` remains
   read-only.
4. Add the smallest Go classifier/consume helper and invoke it in the authenticated
   `ClientUseItem` route after shared admission and before generic unhandled failure.
5. Verify negative/boundary/unknown shapes, stack decrement versus slot removal,
   successful response with zero effect packets, persistence/relogin, repeated and
   race behavior; non-owned shapes must remain unchanged.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

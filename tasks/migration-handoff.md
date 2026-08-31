# Crystal Go migration current handoff

Last updated: 2026-08-31 10:25 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- `CAPACITY-P6-GRIDS-001` is Complete in Go `7794f32`; this closes one P6 leaf only,
  not P6 or the persistent Goal.
- P6 remains scope-frozen at nineteen children: seventeen Complete,
  `ITEM-P6-EXPIRY-001` Active and `ITEM-P6-USE-CATALOG-001` Ready.
- Primary dependency-ready leaf is `ITEM-P6-EXPIRY-001`, matrix row 3549 and P6
  summary row 965.
- Active workstream is `WS-ITEM-P6-EXPIRY-CORE-001`: strict periodic item and
  rental-lock expiry through the existing item authority and production ticker.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `b82cefeb` (`Record inventory capacity migration`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `7794f32` (`Migrate storage capacity command`), not pushed.
- Worktree and index are clean after the feature/matrix commit.

## Completed Storage-capacity workstream evidence

- The ordinary game Chat route accepts case-insensitive `@ADDSTORAGE`, ignores extra
  arguments and has no GM, map, safe-zone or death gate.
- One auth-locked transaction checks and deducts the fixed 1,000,000 Gold cost,
  performs the one-time physical Storage resize from 80 to 160 and preserves items.
  A malformed Legacy-compatible expanded account at physical size 80 is not repaired
  by another purchase; every accepted repeat still charges the fixed cost.
- A strictly future deadline extends from its previous value by ten days. A deadline
  equal to or earlier than current world time resets to current world time plus ten
  days.
- Periodic authority intentionally expires only when `now > expiry`, so expanded slots
  remain server-accessible at equality. Login and Observer `UserInformation`
  intentionally project active only when `expiry > now`, preserving the Legacy equal-
  deadline UI/access mismatch.
- `ResizeStorage=239` carries `int32 Size`, `bool HasExpandedStorage` and the Legacy
  `DateTime.ToBinary` expiry. Successful purchase emits `System Chat -> LoseGold ->
  ResizeStorage`; LowGold emits only System Chat and changes no authority.
- The production world ticker persists strict expiry before sending
  `ExpandedStorageExpired` Chat and `ResizeStorage(false)`. Source-success packets are
  forwarded to active observers in the same order. Save failure prevents visible
  output and closes the affected session while retaining the in-memory commit.
- Authenticated evidence uses a real Storage NPC to move an Inventory item into slot 80,
  proves access at equality, lock after expiry, identity preservation and access after
  renewal. JSON and version-117 restart/relogin preserve size, item, Gold, flag and
  deadline. Concurrent purchase tests prove one shared balance cannot double-spend.

## Verification ledger

Passed for Go `7794f32` before commit:

- Storage auth/protocol/authenticated-session tests at count 20;
- focused Storage auth/session race tests at count 5;
- all first/repeated/expired/LowGold, dead-player, case, extra-argument, observer,
  persistence-failure, slot-80, JSON and version-117 restart paths;
- full `go test ./... -count=1` with only the exact known Quest P7 fixture skipped;
- full race with the Quest fixture plus two unrelated isolated-green flaky fixtures
  skipped: `TestProductionTickerInitializesLightFromUTCOnNonUTCHost` and
  `TestSessionHallucinationTranscript`;
- the two additional race fixtures pass independently at count 10 and count 5;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- final parity and lock-order review found no remaining high-confidence Storage defect.

The first fresh full-race attempt exposed the existing global `time.Local` test race;
a second attempt with it skipped hit the unrelated Hallucination transcript closed-pipe
flake. Both isolate green, and the final exact-three-fixture-skipped full race passes.
These failures did not touch the Storage-focused race gate and were not hidden.

## Active leaf and protected work

- Active leaf: `ITEM-P6-EXPIRY-001`.
- Active workstream: `WS-ITEM-P6-EXPIRY-CORE-001`.
- Trace only Legacy item process expiry loops, rental-lock deadline handling, localized
  messages, DeleteItem output and directly reached persistence helpers; C# stays
  read-only.
- Reuse existing item revision/CAS, Storage access, rental metadata, world timing,
  localization and checkpoint authorities. Do not create a parallel item collection,
  timer or persistence model.
- Freeze strict before/equal/after rules, scan/order semantics, three item-expiry grids,
  two rental-lock grids and simultaneous expiry ordering.
- Preserve Legacy's no-immediate-stat-refresh quirk for expired Equipment items.
- Do not reopen Capacity, Drop, Rental business flows, broad item-use catalogue,
  unrelated P10/P12 lifecycle behavior or protocol layouts.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3549 plus the active index and this handoff.
3. Trace Legacy item `ExpireInfo` and rental-lock expiry loops, exact strict deadlines,
   grid order, localized text and DeleteItem outputs; every `.cs` remains read-only.
4. Freeze the smallest auth/world/session adapter around existing item revision, rental
   metadata, Storage and ticker authorities; do not reopen completed leaves.
5. Verify three item-expiry grids, two lock-expiry grids, before/equal/after boundaries,
   simultaneous order, unchanged live Equipment stats, persistence/restart, repeated
   and race behavior through authenticated production entry points.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

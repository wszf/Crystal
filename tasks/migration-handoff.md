# Crystal Go migration current handoff

Last updated: 2026-08-31 06:47 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- `DROP-P6-GROUND-LIFECYCLE-001` is Complete in Go `4265f77`; this closes one P6
  child only, not P6 or the persistent Goal.
- Primary dependency-ready leaf is now `CAPACITY-P6-GRIDS-001`, matrix row 3541
  and P6 summary row 965. P6 remains frozen at nineteen children: sixteen Complete,
  one Active and two Ready.
- Active workstream is `WS-CAPACITY-P6-INVENTORY-001`: migrate ordinary
  `@ADDINVENTORY` capacity growth, costs and capped repeated-charge behavior.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `7661ec23` (`Close Equip migration leaf`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `4265f77` (`Materialize player death ground drops`), not pushed.
- Worktree and index are clean after the feature/matrix commit.

## Completed Drop closure evidence

- `playerDeathLocked` now executes Legacy normal/red Equipment and Inventory
  selection once and stages every ordinary result through the accepted ground
  authority before Death/ObjectDied.
- Normal and red RNG, PK `> 200`, player/non-player killer branches, safe-zone and
  `NoDropPlayer` boundaries, bind/wedding/sealed gates, BreakOnDeath and incomplete
  Spirit no-continue quirks are preserved.
- Whole drops preserve item identity. Partial stacks use the global item allocator
  and fresh item defaults; ordinary Equipment also preserves the zero-count fresh
  ground-object quirk. Meat durability is reduced only on the staged ground copy.
- Death ground objects ignore `NoThrowItem`, have no owner/group pickup window and
  expire after 120 minutes. Another authenticated live player can pick them up
  immediately.
- Ground objects are staged without visibility, the target auth item snapshot is
  committed first, and rejected commits remove staged objects without leaking
  `VisibleGround`, DeleteItem, chat or Player report output.
- Rental returns remain one persisted batch. Renter DeleteItem/Hint packets are
  split at their original slot positions relative to ordinary ground/delete events;
  owner mail remains after nearby ObjectDied delivery.
- Existing manual item/gold drop, monster/quest loot and P11 fishing/intelligent-
  creature producer behavior remain unchanged.
- Real authenticated `ClientMagic` FireBang death proves ground ObjectItem and
  DeleteItem before Death/ObjectDied, target auth deletion, ownerless 120-minute
  authority, walk/pickup removal, and final picker persistence after session close.
- The feature also updates dead Craft/Shop transcript tests to admit exact ordered
  ground/delete pairs and protects the dead Guild-scroll gate from random item loss.

## Verification ledger

Passed for Go `4265f77` before commit:

- focused player-death, authenticated death, Rental and NPC-Rental tests at count 20;
- all server tests matching Death or Drop at count 20;
- focused death/Rental race tests at count 5;
- mixed earlier-Rental/later-ground slot-order regression at count 20;
- full `go test ./... -count=1` with only the exact known Quest P7 fixture skipped;
- full `go test -race ./... -count=1` with the same exact fixture skipped;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- tracked/staged/untracked `.cs` audits in both repositories.

Integration attribution:

- An earlier unfiltered full run exposed the standing Quest P7 packet-26 versus
  packet-206 fixture and stale Craft dead-transcript expectations now legitimately
  reached by death ground drops. A full-race run similarly exposed the Shop version.
- Craft/Shop assertions now accept only zero or more ordered ObjectItem/DeleteItem
  pairs followed by Death/Health; the dead Guild-scroll fixture is non-deathdrop so
  it still tests the intended dead-use gate rather than random item loss.
- Final exact-known-fixture-skipped full and full-race suites pass.
- Independent review found one real mixed Rental/ordinary slot-order defect. The
  implementation was corrected with ordered Rental markers and per-item delivery;
  final review returned no correctness finding. A provisional delivery-tail concern
  was withdrawn after tracing that ordinary send errors do not stop AfterSend.

## Active leaf and protected work

- Active leaf: `CAPACITY-P6-GRIDS-001`.
- Active workstream: `WS-CAPACITY-P6-INVENTORY-001`.
- Trace only Legacy `@ADDINVENTORY` parsing, size/cost transitions and directly
  reached persistence/projection helpers; C# stays read-only.
- Reuse existing gold mutation, Inventory grid, auth item authority, bootstrap and
  checkpoint persistence. Do not create parallel balance or capacity state.
- Freeze 46-to-54 then four-slot growth through 86, every cost tier, low-gold paths,
  packet/text order and the capped repeated-charge quirk.
- Keep `@ADDSTORAGE` as the next bounded workstream; do not enter item-use, item-
  expiry, unrelated P10 economy, protocol layout or other phase behavior.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3541 plus the active index and this handoff.
3. Trace Legacy `@ADDINVENTORY` command admission, cost table, size mutation and
   response/persistence behavior; every `.cs` remains permanently read-only.
4. Freeze the smallest command/auth/session adapter around the existing gold and
   Inventory authorities; do not implement `@ADDSTORAGE` in the same workstream.
5. Verify every size/cost boundary, capped repeated charge, low-gold atomicity,
   authenticated packet order, relogin/restart, repeated and race behavior.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

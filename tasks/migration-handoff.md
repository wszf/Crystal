# Crystal Go migration current handoff

Last updated: 2026-08-31 05:13 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- `EQUIP-P6-CORE-001` is Complete in Go `519f9f6`; this closes one P6 child only,
  not P6 or the persistent Goal.
- Primary dependency-ready leaf is now `DROP-P6-GROUND-LIFECYCLE-001`, matrix row
  3548 and P6 summary row 965. P6 remains frozen at nineteen children: fifteen
  Complete, one Active and three Ready.
- Active workstream is `WS-DROP-P6-DEATH-MATERIALIZE-001`: materialize existing
  ordinary/red player death item candidates through the committed ground authority.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update:
  `f0f5daa1` (`Record equipment report migration evidence`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `519f9f6` (`Close equipment transaction edge cases`), not pushed.
- Worktree and index are clean after the Equip closure commit.

## Active leaf and protected work

- Active leaf: `DROP-P6-GROUND-LIFECYCLE-001`.
- Active workstream: `WS-DROP-P6-DEATH-MATERIALIZE-001`.
- Trace only ordinary/red player Inventory/Equipment death selection and directly
  reached ground materialization helpers in Legacy; C# stays read-only.
- Connect existing Go `playerDeathLocked` eligible candidates to the committed
  ground/drop authority. Reuse ground object IDs, owner/group window, visibility,
  expiry, stacking, item-definition projection and pickup behavior.
- Preserve rental death return and existing manual item/gold drop behavior.
- Do not reopen P5 monster/quest loot, P11 fishing/intelligent-creature producers,
  capacity/use/expiry leaves, protocol layouts or unrelated death/combat behavior.

Completed Equip closure evidence:

- Report/broadcast commit `8bc1974` migrated all four successful equipment Player
  `Item Moved` records, Hero owner logging and ordinary RemoveItem observer parity.
- Independent read-only review returned `无 correctness finding` for item/grid/index/
  info identity, ordering, failure silence, Hero logging, observer recipients and
  RemoveSlot occupied-cursed commit gating.
- Final edge commit `519f9f6` preserves Legacy silent negative RemoveItem and
  RemoveSlotItem Inventory/Storage targets.
- Storage `CanRemoveItem` now scans the complete physical backing array for
  `FreeSpace`, while the selected target still enforces the unlocked entitlement.
  This preserves the expired-expansion/hidden-empty occupied-target quirk.
- Socket owner resolution now scans Equipment then Inventory and lets a duplicate
  Inventory ID override the Equipment result, matching both Legacy loops.
- Authenticated production entry proves negative requests are silent, physical
  hidden space enables the occupied failure disposition, the detached attachment
  persists removed, and the duplicate Equipment owner remains unchanged.
- Independent final-edge review returned `无 correctness finding` and confirmed no
  out-of-bounds or ordinary RemoveItem/RemoveSlotItem disposition regression.

## Verification ledger

Passed for Go commits `8bc1974` and `519f9f6`:

- report authenticated sessions up to count 50 and focused race count 5;
- final negative/physical-storage/duplicate-owner matrix at count 20;
- the same final-edge matrix under `-race` at count 5;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- full `go test ./... -count=1` with only the known Quest P7 fixture skipped;
- tracked/staged/untracked `.cs` audits in both repositories.

Integration attribution:

- Unfiltered full tests continue to fail only the known
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` packet-26 versus
  packet-206 fixture.
- The full suite passes when only that exact fixture is skipped. No equipment,
  report, observer, storage, persistence, vet, build or race failure remains.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3548 plus the active index and this handoff.
3. Trace Legacy normal/red death selection and ground creation directly; C# remains
   permanently read-only.
4. Freeze the exact selected-item/result structure and the smallest adapter into the
   existing Go ground authority; do not create a parallel item/drop model.
5. Verify real authenticated death, ground visibility/pickup, exclusions, recipient
   order, repeated/race and applicable persistence/relogin behavior.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

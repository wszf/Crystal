# Crystal Go migration current handoff

Last updated: 2026-08-31 03:33 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- Primary dependency-ready leaf remains `EQUIP-P6-CORE-001`, matrix row 3543 and
  P6 summary row 965. P6 remains frozen at nineteen children: fourteen Complete,
  one Active and four Ready.
- `WS-EQUIP-HERO-SNAPSHOT-HARDEN-001` is complete in Go commit
  `7657f4c0555c7e146b3e9b0ee43ddf9a548600a6`.
- `WS-EQUIP-HERO-EQUIP-REMOVE-001` is complete in Go commit
  `f13cfcdffa7434ef78c8168561d16ba948f98c40`; this does not complete the parent.
- Read-only closure discovery confirmed active Fishing EquipItem admission as the
  highest-priority remaining correctness edge. Reporting/broadcast parity remains
  a later finite residual.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- Base HEAD before this evidence update:
  `ab7bd47994fa66d08a06b80494e1d9f056c39843`.
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `f13cfcdffa7434ef78c8168561d16ba948f98c40`, ahead 8 of the configured
  upstream; it has not been pushed.
- Worktree and index are clean after `Migrate Hero equipment transactions`.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Active workstream: `WS-EQUIP-P6-FISHING-EQUIP-GATE-001`.
- Legacy `PlayerObject.EquipItem` rejects Inventory/Storage EquipItem immediately
  while Fishing is active, sends `EquipItem{Success:false}`, and does not mutate
  items or stats. The gate applies to every destination and to Torch.
- The Go ordinary EquipItem production entry currently gates RidingMount but has
  no Fishing check, so a valid Inventory/Storage equip can proceed while Fishing.
- Write authority is the existing ordinary EquipItem admission plus authenticated
  session regression and evidence. Reuse current world Fishing state and item
  transaction; do not create a second authority.
- Frozen behavior matrix: non-Fishing success baseline; Fishing Inventory weapon
  failure; Fishing Inventory Torch failure; Fishing Storage failure; no item,
  RefreshItem, health or appearance side effects on rejection.
- Preserve RemoveItem behavior and the existing mounted non-Torch/Torch matrix.
- Do not modify Fishing runtime/persistence, nested Mount/Fishing/Socket moves,
  Hero, MysteryWater, latest authority, protocols or any `.cs` file.
- After this correctness edge, successful ItemMoved reports and ordinary
  RemoveItem forced observer update remain a separate bounded workstream.

Completed Hero equipment evidence:

- Hero EquipItem/RemoveItem executes against latest detached auth item authority
  behind a stable Hero-slot identity fence while preserving later world runtime.
- Admission freezes live Hero class, gender, level, base profile, Magics, Buffs
  and riding state. Passive Magic/Buff/rate/cap order matches Legacy RefreshStats.
- Equip requires a summoned living Hero; Remove accepts a summoned dead Hero.
  Riding rejects non-Torch Hero equip and accepts Torch.
- BindOnEquip uses the player index. Old Hero-ID bindings are accepted only on the
  Hero path and normalized on successful re-equip; ordinary foreign bindings fail.
- NeedIdentify/binding RefreshItem order, Hero NewMagic with `hero=true`, silent
  temporary RemoveMagic, appearance, HP/MP clamps and remove-only owner update are
  covered through authenticated production sessions.
- Disjoint latest player Inventory/Equipment writes, CurrentBagWeight, rejected
  authority convergence, logout/relogin and JSON persistence are covered.

Completed curse and nested attachment evidence:

- Potion Shape 2 arms runtime-only `UnlockCurse`; first/repeated use, hint/order,
  consumption and logout reset match Legacy.
- Ordinary/Hero cursed equip/remove and Mount/Fishing/Socket RemoveSlotItem share
  the flag with Legacy gate and consumption ordering.
- Occupied nested removal may remove the attachment, consume the flag and reply
  failure; locked, wedding, riding and full gates preserve their reply/silence and
  consumption disposition.

## Verification ledger

Passed on Go commit `f13cfcd`:

- focused authenticated equipment/Hero/MysteryWater/RemoveSlot and passive-skill
  suites at count 10, plus the same focused set under `-race` at count 3;
- `internal/auth` at count 10 and race count 3;
- fresh `go test ./... -count=1` across every package;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- read-only adversarial Hero admission review with no correctness finding.

Full race attribution:

- `go test -race ./... -count=1` ran all packages. Its sole failure was the known
  Quest P7 fixture `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`:
  packet 26 while expecting 206.
- No race detector report was emitted. The fresh non-race full suite passed.
- Final tracked/staged/untracked `.cs` audits in both repositories returned empty.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3543 plus the current active index and handoff.
3. Freeze the Legacy Fishing EquipItem pre-gate as read-only evidence, then add the
   minimal gate to the existing Go authenticated ordinary EquipItem entry.
4. Verify Inventory weapon, Inventory Torch and Storage rejection with unchanged
   authority and no refresh/health/appearance packets; retain non-Fishing and
   mounted baselines, and prove RemoveItem is unchanged.
5. Run focused, repeated, focused-race and applicable integration gates; update
   matrix, active index and this handoff, then create the Go feature commit.
6. Create the matching Legacy evidence commit without `tasks/lessons.md`, then
   continue the next bounded workstream without stopping the Goal.

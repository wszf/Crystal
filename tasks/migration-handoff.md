# Crystal Go migration current handoff

Last updated: 2026-08-31 04:44 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- Primary dependency-ready leaf remains `EQUIP-P6-CORE-001`, matrix row 3543 and
  P6 summary row 965. P6 remains frozen at nineteen children: fourteen Complete,
  one Active and four Ready.
- Completed Go workstreams include Hero snapshot hardening `7657f4c`, Hero
  Equip/Remove plus curse/attachment closure `f13cfcd`, active-Fishing ordinary
  EquipItem admission `d5be202`, and equipment reports/broadcast `8bc1974`.
- None of those commits alone completes the parent leaf. Only the bounded final
  current-HEAD edge review remains before deciding Equip Core closure.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update:
  `9d116ee95fa3c39831b020e7a8c9873ec6f544fd`.
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `8bc1974d2bdb9963b942dffd6e58da909f2c6509`, ahead of the configured
  upstream and not pushed.
- Worktree and index are clean after `Migrate equipment move reports`.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Active workstream: `WS-EQUIP-FINAL-EDGE-001`.
- Revalidate only reachable RemoveSlotItem Storage destination/free-space behavior,
  expanded physical-slot boundaries and real-item/slot-owner resolution on current
  HEAD. Do not inherit unverified findings from an old intermediate worktree.
- If no residual survives, close the parent leaf with evidence only. If a residual
  survives, freeze the smallest production workstream and verify through the real
  authenticated equipment entry.
- Preserve accepted item transaction/latest-authority, cursed, report, observer,
  Hero, Fishing, riding, Storage/NPC and persistence behavior.
- Do not reopen P8 Hero/Mount or P11 Fishing/Awakening/IntelligentCreature owner
  lifecycle behavior, unrelated item operations, protocol layouts or any `.cs`.

Completed report/broadcast evidence:

- `EquipItem`, `RemoveItem`, `EquipSlotItem` and `RemoveSlotItem` now route every
  successful Legacy `Item Moved` record through the existing Player runtime logger.
- EquipItem preserves the Legacy two-record quirk: both records identify the newly
  equipped item, and the first uses `RemoveItem` info before the success response.
- EquipSlotItem reports the moved attachment after its response/stat notifications;
  RemoveItem reports the removed equipment after response/refresh/broadcast.
- RemoveSlotItem preserves the Legacy host-owner identity quirk rather than reporting
  the detached attachment; Mount and Socket grid names are now exact.
- Hero EquipItem/RemoveItem reports use the owner Player logger with Hero grid names.
- All four failure paths remain report-silent and only committed successful authority
  mutations may emit reports.
- Ordinary RemoveItem now forces the nearby same-map non-owner `PlayerUpdate` even
  when the removed slot does not change appearance. Existing RemoveSlotItem forced
  broadcast is retained; the actor remains excluded from ordinary Broadcast.

Previously completed Fishing/Hero/curse evidence remains authoritative:

- Active Fishing rejects ordinary Inventory/Storage EquipItem before authority/NPC/
  riding admission, including Torch; RemoveItem and HeroInventory stay unchanged.
- Hero EquipItem/RemoveItem uses latest detached auth item authority behind a stable
  Hero-slot fence while preserving live actor/runtime fields and old Hero-ID binding.
- MysteryWater and ordinary/Hero/nested cursed moves preserve Legacy one-shot,
  occupied-target, reply/silence and consumption disposition.

## Verification ledger

Passed for Go commit `8bc1974`:

- authenticated Player logger and two-player observer report sessions;
- ordinary and Hero reports plus failure suppression at count 10;
- report, equipment transcript, latest Hero authority and Storage gate matrix under
  focused `-race` at count 3;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- full `go test ./... -count=1` with only the known Quest P7 fixture skipped;
- tracked/staged/untracked `.cs` audits in both repositories.

Integration attribution:

- Unfiltered `go test ./... -count=1` failed only
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`: packet 26 while
  expecting 206.
- The full suite passed when only that exact known Quest fixture was skipped. No
  equipment report, observer, persistence, vet, build or race failure was observed.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3543 plus the active index and this handoff.
3. Complete the current read-only final edge review against current Legacy/Go code;
   C# remains permanently read-only.
4. If a real residual survives, freeze and implement only that bounded Go production
   path with authenticated focused/repeated/race evidence.
5. Otherwise close `EQUIP-P6-CORE-001`, update matrix/index/handoff and select the
   next dependency-ready frozen child.
6. Commit only owned Go/evidence files, exclude `tasks/lessons.md`, then continue the
   persistent migration Goal without push or merge.

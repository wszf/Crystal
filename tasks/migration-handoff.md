# Crystal Go migration current handoff

Last updated: 2026-08-31 04:00 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- Primary dependency-ready leaf remains `EQUIP-P6-CORE-001`, matrix row 3543 and
  P6 summary row 965. P6 remains frozen at nineteen children: fourteen Complete,
  one Active and four Ready.
- Completed Go workstreams include Hero snapshot hardening `7657f4c`, Hero
  Equip/Remove plus curse/attachment closure `f13cfcd`, and active-Fishing
  ordinary EquipItem admission `d5be202`.
- None of those commits completes the parent leaf. Successful equipment reporting,
  ordinary RemoveItem observer parity and final finite edge review remain open.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update:
  `8fc43da2e3949c861ed71768e8a9c44b5a3253b2`.
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `d5be202790019dcc1a46d7ab7e97d0fbf698b361`, ahead 9 of the configured
  upstream; it has not been pushed.
- Worktree and index are clean after `Migrate fishing equipment admission gate`.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Active workstream: `WS-EQUIP-REPORT-BROADCAST-001`.
- Freeze the four Legacy successful `Report.ItemMoved` tails for EquipItem,
  RemoveItem, EquipSlotItem and RemoveSlotItem, including exact source/destination
  grid, item identity/name and index/to arguments.
- Close ordinary RemoveItem's unconditional `Broadcast(GetUpdateInfo())` observer
  behavior. RemoveSlotItem already uses a forced equipment broadcast; verify rather
  than reimplement that completed path.
- Reuse the existing Go report adapter, report sink and world observer projection.
  Do not create a parallel transaction/report authority.
- Preserve all accepted success/failure, latest-authority, cursed, storage/NPC,
  Hero, Fishing, riding, refresh, persistence and response-order semantics.
- Do not reopen P8 Hero/Mount or P11 Fishing/Awakening/IntelligentCreature owner
  lifecycle behavior, unrelated reporting, protocol layouts or any `.cs` file.

Completed active-Fishing evidence:

- Legacy ordinary EquipItem rejects Inventory/Storage before actor, destination,
  riding and Storage-NPC admission whenever Fishing is active. The failed response
  echoes the request and the gate includes Torch.
- Go now freezes RidingMount and Fishing in one world-lock snapshot, rejects before
  cross-grid authority, and leaves HeroInventory on its separate existing path.
- An authenticated session opens valid Storage access, proves a non-Fishing weapon
  equip baseline, proves RemoveItem still succeeds while Fishing, then rejects
  Inventory weapon/Torch and Storage weapon/Torch requests.
- KeepAlive barriers prove no RefreshItem, health or appearance side packet; auth
  Inventory, Equipment and Storage remain unchanged after every rejected request.

Previously completed Hero/curse evidence remains authoritative:

- Hero EquipItem/RemoveItem uses latest detached auth item authority behind a stable
  Hero-slot fence while freezing live actor requirements and preserving later world
  runtime, dead-Hero asymmetry, riding and old Hero-ID binding compatibility.
- NeedIdentify/binding refresh order, NewMagic `hero=true`, silent temporary
  RemoveMagic, appearance, HP/MP clamps, rejected convergence, bag weight,
  logout/relogin and JSON state are covered through authenticated sessions.
- MysteryWater and ordinary/Hero/nested cursed moves preserve Legacy one-shot,
  occupied-target, reply/silence and consumption disposition.

## Verification ledger

Passed on Go commit `d5be202`:

- focused active-Fishing authenticated session at count 20;
- Fishing/non-Fishing, Storage and mounted admission matrix at count 10;
- the same matrix under `-race` at count 3;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- full `go test ./... -count=1` with the known Quest P7 fixture explicitly skipped;
- tracked/staged/untracked `.cs` audits in both repositories.

Integration attribution:

- Unfiltered `go test ./... -count=1` failed only
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`: packet 26 while
  expecting 206. The exact Quest test reproduced the same result at count 3.
- This is the already attributed Quest P7 baseline; the full suite passed when only
  that exact fixture was skipped. No equipment admission failure was observed.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3543 plus the active index and this handoff.
3. Read the four Legacy successful equipment tails and directly reached
   `Report.ItemMoved`/broadcast helpers only; C# remains read-only.
4. Implement ItemMoved through the existing Go report adapter and ordinary
   RemoveItem observer parity through the existing world update path. Keep item
   commit authority and response/refresh ordering unchanged.
5. Verify real authenticated report-sink records, observer recipients, failure
   suppression, persistence, repeated and race gates.
6. Update matrix, active index and handoff, create the Go feature commit and a
   Legacy evidence commit excluding `tasks/lessons.md`, then continue the Goal.

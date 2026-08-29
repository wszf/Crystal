# Crystal Go migration current handoff

Last updated: 2026-08-29 13:59 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. The platform reported it paused at this real compact
  boundary; resume this same Goal after this gate. Main is `gpt-5.6-sol/ultra`;
  bounded workers default to `luna_worker` (`gpt-5.6-luna/max`).
- `ITEM-P6-GRID-CROSS-001` is accepted Complete in Go commit
  `3d124e0a1029a08223238e81e02e20303e287fea`. P6 is scope-frozen at nineteen
  children: 14 Complete, one Active and four Ready.
- The unique next Active leaf is dependency-ready `EQUIP-P6-CORE-001`, matrix
  row 3543 and P6 summary row 965. Matrix/index routing is synchronized.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `master`; HEAD `5915d3f5aa922f24b1f94da8d3ab920df107bcb0`;
  upstream `origin/master`, ahead 517 and behind 0.
- Before this compact refresh the index and worktree were clean. The only
  current unstaged tracked paths are `tasks/lessons.md` and this handoff; there
  are no staged or untracked paths and no Legacy implementation changes. These
  two paths are the main agent's compact-gate control edits.
- Post-edit `git diff --check`, `git diff --cached --check`, control checker and
  all three Legacy C# queries exit 0/empty. This snapshot records the expected
  one compact-gate control commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `main`; HEAD `3d124e0a1029a08223238e81e02e20303e287fea`;
  no upstream. Index and worktree are clean.
- Commit `3d124e0` contains exactly the accepted Grid Cross production, tests,
  matrix evidence and next-Leaf routing.
- `git diff --check`, `git diff --cached --check`, and all three Go C# queries
  exit 0/empty. No owned Go/server process is active. PID 57214 is an unrelated
  Claude `go ... droprate-replay-v5 list ./...` job; do not stop it.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Accepted Grid Cross production now provides latest-auth atomic Storage/player/
  refine/Hero mutations, normalized Rental removal, shared item revision/world
  CAS, owned-domain session projection, exact Move/Split/Merge matrix and order,
  Split fresh slot shape, Trade sealed-Hero metadata, persistence/relogin and
  unsupported-grid failure. It must remain a protected input.
- Equip owns player EquipItem/RemoveItem transactions over Inventory/Storage/
  Socket/Mount/Fishing, cursed/bind/wedding and slot/type/class/gender/
  requirement/weight gates, stat/HP/appearance/set/special/temporary-skill
  refresh order. P8 mount/Hero and P11 fishing/awakening business lifecycles,
  Grid Cross, item-use admission/catalog and protocol layouts are protected.
- Before any Equip write, trace the exact Legacy handlers and directly reached
  CanEquip/CanRemove/RefreshItem/RefreshStats helpers into a finite pair/gate/
  side-effect matrix. All `.cs` remains read-only.
- Read-only worker `01a04c15-b967-7502-8493-e85443b71750` traced Equip dispatch
  and `PlayerObject.EquipItem` lines 5527-5795: Game-stage forwarding; Inventory,
  Storage and HeroInventory sources; Storage NPC/range/access and live-Hero
  gates; cursed/soul-bind/wedding/DontStore checks; identify/BindOnEquip
  `RefreshItem`; two ItemMoved reports; success packet then actor RefreshStats.
  Treat this as a lead pending main-agent source reread.
- `CanEquipItem`, RemoveItem, exact slot/type ordering, RefreshStats, sockets,
  mount/fishing and temporary-skill side effects remain untraced or incomplete.

## Verification ledger

- Accepted Grid Cross evidence remains: compile exit 0; focused count 20 and
  focused race count 5 pass; touched-package gate exits 0; final fresh full
  test/vet/build exits 0; final fresh full race exits 0. Terminal reviewer
  `01a04bfb-9715-7ee3-8ee5-f1905cc5397a` returned `No findings`.
- No Equip code was written and no tests were started in this compact window.
  Read-only workers `01a04c15-b967-7502-8493-e85443b71750`,
  `01a04c15-e1b3-7391-80b3-98d4544d5944`, and
  `01a04c16-0636-7622-b3bf-7722e915c4c3` were interrupted, collected and
  closed; all declared no writes. The first combined two-repository status call
  violated C01 and was discarded; independent zero-exit checks replaced it.

## Exact recovery sequence

1. Reread this handoff, rerun Legacy control/diff/C# gates, and commit only
   `tasks/lessons.md` plus this compact-gate snapshot; then independently verify
   both repositories and the unchanged Go HEAD.
2. Resume only `EQUIP-P6-CORE-001`; rows 965 and 3543 have been reread. Search
   archived lessons for EquipItem, RemoveItem, RefreshItem, sockets, storage,
   mount, fishing, stats, appearance, temporary skills, packet order, revision
   and persistence.
3. Main-reread the cited Equip source, then trace RemoveItem, CanEquipItem,
   CanRemoveItem and directly reached refresh helpers. Freeze the finite matrix
   before any Go write; do not reopen accepted Grid Cross behavior.

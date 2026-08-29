# Crystal Go migration current handoff

Last updated: 2026-08-29 13:50 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `ITEM-P6-GRID-CROSS-001` is accepted Complete in Go commit
  `3d124e0a1029a08223238e81e02e20303e287fea`. P6 is scope-frozen at nineteen
  children: 14 Complete, one Active and four Ready.
- The unique next Active leaf is dependency-ready `EQUIP-P6-CORE-001`, matrix
  row 3543 and P6 summary row 965. Matrix/index routing is synchronized.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `master`; pre-control-commit HEAD
  `986f08bfa91972359e5c9c92a79e29644e790aa7`; upstream `origin/master`, ahead
  516 and behind 0.
- Unstaged tracked paths are exactly:
  `tasks/lessons.md`,
  `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/lessons-archive/migration/world-state-lifecycle.md`,
  `tasks/lessons-archive/verification/fixtures-and-transcripts-01.md`,
  `tasks/lessons-archive/verification/race-and-flake-attribution.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths and no Legacy implementation changes.
- `git diff --check`, `git diff --cached --check`, control checker and all three
  Legacy C# queries exit 0/empty. This handoff records the expected one Go leaf
  commit followed by one Legacy control/lesson commit.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `main`; HEAD `3d124e0a1029a08223238e81e02e20303e287fea`;
  no upstream. Index and worktree are clean.
- Commit `3d124e0` contains exactly the accepted Grid Cross production, tests,
  matrix evidence and next-Leaf routing.
- `git diff --check`, `git diff --cached --check`, and all three Go C# queries
  exit 0/empty. No owned Go/server process is active. PID 44901 is an unrelated
  Claude job under `.codex-box/.../droprate-candidate-v4`; do not stop it.

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

## Verification ledger

- Three terminal findings were corrected: Rental source resurrection during
  Storage/Hero merges, Split loss of definition-specific nested slots, and
  missing cached sealed-Hero Trade metadata. Integration also prevented rejected
  callback publication and deep-copied only Cross-Grid-owned domains.
- One new authority-merge alias regression initially failed because the helper
  reused direct grid assignment. It now clones grids; exact count-20 passes.
  The earlier Rental regression first failed after only passing the allowed map,
  proving the shared source-preservation loop also needed the fix; it passes.
- Final compile `go test ./internal/itemstate ./internal/auth
  ./cmd/crystal-server -run '^$'` exits 0. Focused itemstate/auth/server tests
  pass count 20; focused race passes itemstate/auth/server count 5, including
  sessions, admin dirty-world, Trade cache and all terminal corrections.
- Final touched packages `go test ./internal/itemstate ./internal/auth
  ./cmd/crystal-server -count=1` exit 0; server 83.579s.
- Final fresh `go test -count=1 ./... && go vet ./... && go build ./...` exits 0;
  server 88.086s. Final fresh `go test -race -count=1 ./...` exits 0; server
  101.322s.
- An earlier full race exited 1 only at the archived timed-recall due-scan queue
  flake and OmaMage `[2 1]`/`[1]` random-boundary fixture, with no detector or
  Cross-Grid stack. Their combined isolated race count 20 exited 0; two later
  fresh unexcluded full-race runs exited 0 (server 100.188s and final 101.322s).
- Terminal reviewer `01a04bfb-9715-7ee3-8ee5-f1905cc5397a` reviewed the latest
  candidate and returned `No findings`. All migration subagents are closed.

## Exact recovery sequence

1. Rerun the Legacy control/C# gates, stage only the seven listed Legacy paths
   and commit the accepted Grid Cross lessons/control routing.
2. Verify both repositories independently. Resume only `EQUIP-P6-CORE-001`:
   read matrix rows 965 and 3543, then search archived lessons for EquipItem,
   RemoveItem, RefreshItem, sockets, storage, mount, fishing, stats, appearance,
   temporary skills, packet order, revision and persistence.
3. First Equip source command after recovery: locate exact Legacy EquipItem and
   RemoveItem handlers plus direct helpers; freeze the finite matrix before Go
   writes. Do not read the full matrix or reopen accepted Grid Cross behavior.

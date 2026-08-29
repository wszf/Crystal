# Crystal Go migration current handoff

Last updated: 2026-08-30 03:13 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Resume the same Goal after this control-plane gate.
- `ITEM-P6-GRID-CROSS-001` is accepted Complete in Go commit `3d124e0`; P6 is
  scope-frozen at nineteen children: fourteen Complete, one Active and four
  Ready.
- The Primary Active leaf is dependency-ready `EQUIP-P6-CORE-001`, matrix row
  3543 and P6 summary row 965. Its ordinary Inventory/Storage EquipItem and
  RemoveItem slice is Complete in Go commit `3c6d3c3`; nested slot attachments
  remain open.
- The new policy permits bounded workstreams inside this Primary leaf. Agent
  identity and a fixed count are not Goal fields; each workstream must carry a
  scope, authority lock, dependency and evidence record.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD observed before the control-plane
  commit: `4297dda58561c06456ed4b744c719c6e07927926`; upstream `origin/master`,
  ahead 518 and behind 0.
- Existing user/lesson work is the only pre-existing tracked modification:
  `tasks/lessons.md`. Do not reset, overwrite or include it accidentally.
- The expected control-plane delta for this cycle is limited to the Goal/Agent
  rules, active/handoff routing, control checker and `.claude/commands/goal.md`.
  Verify actual status before and after that documentation commit.
- No Legacy implementation change is authorized; every `.cs` file remains
  read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD `3c6d3c3824d6ad40ae89a5964bd81f2082cb4937`;
  upstream `origin/migrate/drop-owner-p12`, ahead 1 and behind 0.
- Index and worktree are clean at this snapshot. The latest commit contains the
  ordinary player Inventory/Storage equipment authority, tests and matrix
  evidence; it has not been pushed.
- Before any new Go writer, independently verify this HEAD/status and the
  active matrix anchors; do not trust a stale handoff hash.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Protected inputs: accepted Grid Cross, Storage, Refine, P8 Hero/Mount
  lifecycle and P11 Fishing/Awakening lifecycle authorities. Do not reopen or
  rewrite them from this leaf.
- Next bounded workstream: `WS-EQUIP-SLOT-AUTH-001`, nested
  `EquipSlotItem/RemoveSlotItem` authority migration over Inventory/Storage and
  Mount, Fishing-rod and Socket nested Slots. Its writer scope is the existing
  equipment transaction/session routing and focused tests; protocol layouts,
  owner lifecycles and generic Move/Merge endpoints are forbidden.
- Required source rulings before writing include `CanUseItem`, `CanRemoveItem`,
  UnlockCurse, NeedIdentify-before-Shape ordering, BindOnEquip, ItemMoved
  reporting and the Legacy occupied-target quirk. The Go authority must keep
  Character and Storage/nested Slots atomic and must not lose state on failure.
- At this handoff boundary no implementation writer is active. Read-only
  discovery evidence is collected; the next recovery step is to freeze the
  finite attachment gate/order matrix, then implement only the bounded slice.

## Verification ledger

- Go commit `3c6d3c3` was previously verified with the ordinary equipment
  focused, repeated and focused-race tests plus fresh package/full test, vet and
  build gates; the nested attachment slice has not been implemented.
- The last accepted Grid Cross evidence remains separate and must not be reused
  as proof for the nested SlotItem slice.
- Control-plane checks for this cycle must include the new `/goal` command,
  staged and unstaged diff checks, hard-link parity and both-repository C# gates.

## Exact recovery sequence

1. From the Legacy root, run `tasks/check-migration-control.sh`, inspect the
   control diff and preserve `tasks/lessons.md`; commit only the expected
   control-plane files on `migration/goal-orchestration`.
2. Independently verify Legacy and Go branches, full status, HEADs and three
   `.cs` queries. Re-read this handoff and `tasks/migration-active.md`; read only
   matrix rows 965 and 3543.
3. Search the lessons archive for `EQUIP-P6-CORE-001`, SlotItem, sockets,
   storage, mount, fishing, stats, magic, packet order, revision and persistence.
4. Main-reread the cited Legacy handlers and directly reached helpers. Freeze
   `WS-EQUIP-SLOT-AUTH-001` as a finite gate/order/source-target matrix before
   any Go write; keep read-only tracing, test preparation and review parallel.
5. Implement through latest-auth CrossGrid authority, world CAS/projection and
   production session handlers. Verify unit, authority, session transcript,
   failure atomicity, persistence/relogin and focused race evidence.
6. Before the feature commit, update the Go matrix, active index and this
   handoff, run all applicable leaf/integration gates, audit both repositories
   for C# changes, and commit only owned Go and Go-document files. Do not push.

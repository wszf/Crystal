# Crystal Go migration current handoff

Last updated: 2026-08-30 04:33 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Resume the same Goal after this control-plane gate.
- `ITEM-P6-GRID-CROSS-001` is accepted Complete in Go commit `3d124e0`; P6 is
  scope-frozen at nineteen children: fourteen Complete, one Active and four
  Ready.
- The Primary Active leaf is dependency-ready `EQUIP-P6-CORE-001`, matrix row
  3543 and P6 summary row 965. Ordinary Inventory/Storage equipment is Complete
  in `3c6d3c3`; nested SlotItem authority is Complete in `d85b430`.
- The new policy permits bounded workstreams inside this Primary leaf. Agent
  identity and a fixed count are not Goal fields; each workstream must carry a
  scope, authority lock, dependency and evidence record.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; control-plane HEAD is
  `888bfa0e91e8cccb130f84237f94e9614c4a6616`; upstream `origin/master`, ahead
  518 and behind 0.
- Existing user/lesson work is the only pre-existing tracked modification:
  `tasks/lessons.md`. Do not reset, overwrite or include it accidentally.
- Current active-index/handoff routing is bounded documentation only; no Legacy
  implementation change is authorized. Every `.cs` file remains read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; latest HEAD is
  `c89111f7f0c1b09f5b2df0e56ee72aa58a785ce1`; feature commit
  `d85b4305cee938b99a70c6fd8d716cc586d31bd2` plus the WeddingRing correction
  and evidence commits follow it; upstream `origin/migrate/drop-owner-p12`,
  ahead 6 and behind 0.
- Index and worktree are clean at this snapshot. The latest commits contain the
  nested SlotItem authority/session implementation, WeddingRing regression fix,
  matrix evidence and comment cleanup; they have not been pushed.
- Before any new Go writer, independently verify this HEAD/status and the
  active matrix anchors; do not trust a stale handoff hash.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Protected inputs: accepted Grid Cross, Storage, Refine, P8 Hero/Mount
  lifecycle and P11 Fishing/Awakening lifecycle authorities. Do not reopen or
  rewrite them from this leaf.
- Completed workstream: `WS-EQUIP-SLOT-AUTH-001` is accepted in Go commit
  `d85b4305cee938b99a70c6fd8d716cc586d31bd2`; it migrated atomic nested
  attachment moves without changing protocol layouts or owner lifecycles.
- Active workstream: `WS-EQUIP-HERO-SNAPSHOT-HARDEN-001`. Role: production
  authority prerequisite. Go read/write scope is bounded to Hero auth/runtime
  persistence, CrossGrid Hero-item projection, terminal persistence fences,
  detached profile clones and focused production-entry tests. Authority lock:
  Hero runtime persistence plus CrossGrid Hero item publication. Dependencies:
  accepted P6 CrossGrid and P8 Hero lifecycle. Forbidden scope: Hero Equip/Remove
  business gates, protocol layouts, P8 lifecycle semantics, P11 owners and C#.
  Acceptance requires latest-auth runtime-only commits, item-only projection,
  no stale terminal overwrite, deep clone isolation, relogin/restart and race.
- Source rulings recorded for the completed slice include strict production
  Fishing-rod validation, NeedIdentify-before-Shape RefreshItem ordering,
  commented BindOnEquip behavior, no UnlockCurse consumption in EquipSlotItem,
  safe failure atomicity, and the Legacy occupied-target loss quirk intentionally
  not reproduced by the Go authority.
- At this handoff boundary no implementation writer is active; resume with a
  finite next workstream only after refreshing the matrix anchors and both
  repository status/audits.

## Verification ledger

- Go feature commit `d85b4305cee938b99a70c6fd8d716cc586d31bd2`, WeddingRing
  correction `95b8963`, and evidence commits passed focused, repeated,
  focused-race, package/full test, full race, vet and build gates after the
  correction.
- The nested slice evidence is separate from the accepted Grid Cross evidence;
  the leaf remains Active because Hero equipment and remaining Legacy edge rows
  are not closed.
- Control-plane checks passed for `/goal`, staged/unstaged diff checks, hard-link
  parity and both-repository C# gates; Legacy `tasks/lessons.md` remains the only
  pre-existing tracked modification.

## Exact recovery sequence

1. From the Legacy root, run `tasks/check-migration-control.sh` and preserve
   `tasks/lessons.md`; commit only intentional control-plane updates.
2. Independently verify Legacy and Go branches, full status, HEADs and three
   `.cs` queries. Re-read this handoff and `tasks/migration-active.md`; read only
   matrix rows 965 and 3543.
3. Search the lessons archive and cited Legacy handlers for the next finite
   `EQUIP-P6-CORE-001` residual; do not reopen completed SlotItem or Grid Cross
   evidence.
4. Register the next bounded workstream with disjoint file/authority scope and
   explicit forbidden scope before any Go write; keep read-only tracing and review
   parallel where safe.
5. Implement through the latest-auth authority, world CAS/projection and real
   production session handlers. Verify unit, authority, transcript,
   failure-atomicity, persistence/relogin and focused race evidence.
6. Before the next feature commit, update the Go matrix, active index and this
   handoff, run applicable leaf/integration gates, audit both repositories for
   C# changes, and commit only owned Go and Go-document files. Do not push.

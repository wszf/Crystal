# Crystal migration active index

Last verified: 2026-08-24 00:20 (Asia/Singapore)

This is the concise execution router for the persistent migration Goal. The Go
`docs/migration-matrix.md` remains the detailed status/evidence authority. Do not
copy its narratives here and do not read the full matrix during normal recovery.
Keep this file at or below 300 lines and 32 KiB.

## Progress semantics

- One main implementation leaf may be `Active`.
- A phase is scope-frozen only after its closure audit converts every vague
  residual into finite child leaves.
- Scope is frozen phase by phase; the whole project's remaining inventory does
  not need to be calculated in one up-front pass.
- `Complete` phase labels are not a project percentage. Leaf burn-down and ETA
  are publishable only for a scope-frozen phase.
- Current project-wide ETA and percentage remain `Unavailable` while eleven
  phases still have open closure inventories.

## Phase routing summary

| Phase | Matrix status | Scope state | Closure leaf |
|---|---|---|---|
| P0 | Complete | Frozen | — |
| P1 | In progress | Open | `DISC-P1-CLOSURE` |
| P2 | In progress | Open | `DISC-P2-CLOSURE` |
| P3 | In progress | Open | `DISC-P3-CLOSURE` |
| P4 | In progress | Open | `DISC-P4-CLOSURE` |
| P5 | In progress | Open | `DISC-P5-CLOSURE` |
| P6 | In progress | Open | `DISC-P6-CLOSURE` |
| P7 | In progress | Open | `DISC-P7-CLOSURE` |
| P8 | Complete | Frozen | — |
| P9 | In progress | Open | `DISC-P9-CLOSURE` |
| P10 | In progress | Open | `DISC-P10-CLOSURE` |
| P11 | In progress | Open | `DISC-P11-CLOSURE` |
| P12 | In progress | Open | `DISC-P12-CLOSURE` |

## Active batch

- Leaf ID: `DISC-P1-CLOSURE`
- Status: `Active` (selected for recovery routing; no discovery command has run
  since the actual compaction signal)
- Outcome: enumerate the finite residual P1 configuration, localization,
  logging/lifecycle, and deployment behaviors into stable child leaf IDs.
- Go matrix anchors to read: row beginning `| P1 |`; its exact residual phrases
  `remaining language keys`, `category-specific business call-site coverage`,
  and `deployment validation`; only directly linked P1 headings found from those
  anchors.
- Authority boundary: Legacy and Go sources needed to prove each candidate P1
  behavior are read-only during inventory. Discovery may write only
  `tasks/migration-active.md`, `tasks/migration-handoff.md`, and the bounded P1
  inventory/evidence portion of Go `docs/migration-matrix.md`.
- Forbidden during this discovery leaf: functional implementation, broad matrix
  or source-tree dumps, other-phase expansion, percentage/ETA publication, and
  declaring P1 scope-frozen before every residual has a finite child leaf.

The immediately prior leaf `OPS-P1-P3-P4-UTILITY-001` is Complete at Go commit
`5bce7c28c162296d27b61db60897d1b23a80c91e` (`feat(p3): restore public utility
commands`). It updated the 2026-08-24 utility paragraph and P1/P3/P4/P8 rows;
P1/P3/P4 remain In progress and P8 remains Complete.

### Protected Go ownership

- The Go worktree is clean; there is no uncommitted functional file to protect.
- During this discovery leaf the only permitted Go write is the bounded P1
  inventory/evidence portion of `docs/migration-matrix.md`.
- Functional Go ownership must be assigned to finite child leaves before any
  implementation begins.

### Remaining acceptance work

- [ ] Enumerate each P1 residual as an observable behavior, not a vague module.
- [ ] Give every child a stable ID, matrix anchor, Legacy entry point, dependency
      boundary, owned files, acceptance evidence, and required test tier.
- [ ] Separate independent children from shared lifecycle/startup architecture.
- [ ] Reconcile duplicate or already-complete evidence instead of creating work
      from stale prose.
- [ ] Mark P1 scope-frozen only when a bounded search proves no unnamed residual
      remains; otherwise retain a finite follow-up discovery child.
- [ ] Select exactly one dependency-ready child as the next `Active` batch and
      refresh the handoff before any production write.

## Scope-freeze discovery queue

These are registered phase-local inventory audits, not permission to implement
broad unnamed scope.

| Leaf ID | Phase | Status | Required output |
|---|---|---|---|
| `DISC-P1-CLOSURE` | P1 | Active | finite config/localization/log/lifecycle/deployment children |
| `DISC-P2-CLOSURE` | P2 | Discovery | finite account/NPC-access/restart children |
| `DISC-P3-CLOSURE` | P3 | Discovery | finite startup/admin/ranking children |
| `DISC-P4-CLOSURE` | P4 | Discovery | finite map/bootstrap/visibility children |
| `DISC-P5-CLOSURE` | P5 | Discovery | finite spell/combat/AI/respawn/packet children |
| `DISC-P6-CLOSURE` | P6 | Discovery | finite item/equipment/craft children |
| `DISC-P7-CLOSURE` | P7 | Discovery | finite NPC/shop/quest/script children |
| `DISC-P9-CLOSURE` | P9 | Discovery | finite guild/war/territory children |
| `DISC-P10-CLOSURE` | P10 | Discovery | finite economy-system children |
| `DISC-P11-CLOSURE` | P11 | Discovery | finite miscellaneous-system children |
| `DISC-P12-CLOSURE` | P12 | Discovery | finite persistence/backup/deployment children |

## Selection protocol

1. Verify this index and the current handoff against each repository separately.
2. Resume only the one `Active` phase-local closure leaf; do not inventory all
   remaining phases up front.
3. Close discovery by registering finite children, then activate one
   dependency-ready child before production changes begin.
4. Keep only current routing state here. Completed details belong in the matrix
   and Git history, not as appended narratives.

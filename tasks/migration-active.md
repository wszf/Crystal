# Crystal migration active index

Last verified: 2026-08-23 21:40 (Asia/Singapore)

This is the concise execution router for the persistent migration Goal. The Go
`docs/migration-matrix.md` remains the detailed status/evidence authority. Do not
copy its narratives here and do not read the full matrix during normal recovery.
Keep this file at or below 300 lines and 32 KiB.

## Progress semantics

- One main leaf may be `Active`.
- A phase is scope-frozen only after its closure audit has converted every vague
  residual into finite child leaves.
- `Complete` phase labels are not a project percentage; leaf counts become a
  useful burn-down only after scope freeze.
- Current project ETA and percentage are intentionally `Unavailable`: eleven
  phases still require finite closure inventories.

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

- Leaf ID: `OPS-P1-P3-P4-UTILITY-001`
- Status: `Active` (implementation paused only while the control plane is being
  optimized; this is not an external blocker)
- Outcome: finish Legacy-equivalent Game-stage `@TIME`, `@ROLL`, and `@MAP`
  command handling, current-culture formatting/casing, recipient projection,
  protocol ordering, and authenticated transcript coverage.
- Go matrix anchors to read: stage table rows beginning `| P1 |`, `| P3 |`,
  `| P4 |`, and `| P8 |`; search those fixed row prefixes instead of reading
  the complete 3223-line matrix.
- Legacy evidence anchors: command dispatch and `@TIME`/`@ROLL`/`@MAP` handling,
  current-culture `ToUpper`/composite formatting, Group/System Chat recipients,
  observer forwarding, and FormatException behavior.

### Protected Go ownership

Do not reset, stash, checkout, clean, delete, move, or overwrite these files:

- tracked: `cmd/crystal-server/main.go`
- tracked: `cmd/crystal-server/observer.go`
- tracked: `cmd/crystal-server/world.go`
- tracked: `internal/config/config.go`
- tracked: `internal/config/localization.go`
- tracked: `internal/config/localization_test.go`
- untracked: `cmd/crystal-server/utility_command.go`
- untracked: `cmd/crystal-server/utility_command_session_test.go`
- untracked: `cmd/crystal-server/utility_command_test.go`
- untracked: `internal/config/culture.go`
- untracked: `internal/config/culture_test.go`
- untracked: `legacy_composite_format.go`

### Remaining acceptance work

- [ ] Main-agent review of all twelve protected files.
- [ ] Decide the minimal package boundary for the root-level 892-line formatter
      prototype; do not wire, move, or delete it before review.
- [ ] Lock current-culture command casing, including Turkish `i` behavior.
- [ ] Lock time, integer, percent, alignment, escaped-brace, custom numeric, and
      FormatException behavior actually reachable by the three commands.
- [ ] Verify `@TIME`, `@ROLL`, and `@MAP` production routing, private/group/system
      recipients, observer forwarding, and packet FIFO.
- [ ] Run the leaf compile/focused/repeated/relevant-race gates.
- [ ] Run an integration gate because this leaf crosses config, session routing,
      observer projection, and shared command infrastructure.
- [ ] Update the authoritative Go matrix and this index, refresh the concise
      handoff, and make atomic Go/Legacy commits.

## Scope-freeze discovery queue

These rows are inventory audits, not permission to implement broad unnamed
scope. Each audit must produce finite child leaf IDs and matrix anchors.

| Leaf ID | Phase | Status | Required output |
|---|---|---|---|
| `DISC-P1-CLOSURE` | P1 | Discovery | finite config/log/lifecycle/deployment residuals |
| `DISC-P2-CLOSURE` | P2 | Discovery | finite account/NPC-access/restart residuals |
| `DISC-P3-CLOSURE` | P3 | Discovery | finite startup/admin/ranking residuals |
| `DISC-P4-CLOSURE` | P4 | Discovery | finite map/bootstrap/visibility residuals |
| `DISC-P5-CLOSURE` | P5 | Discovery | finite spell/combat/AI/respawn/packet residuals |
| `DISC-P6-CLOSURE` | P6 | Discovery | finite item/equipment/craft residuals |
| `DISC-P7-CLOSURE` | P7 | Discovery | finite NPC/shop/quest/script residuals |
| `DISC-P9-CLOSURE` | P9 | Discovery | finite guild/war/territory residuals |
| `DISC-P10-CLOSURE` | P10 | Discovery | finite economy-system residuals |
| `DISC-P11-CLOSURE` | P11 | Discovery | finite miscellaneous-system residuals |
| `DISC-P12-CLOSURE` | P12 | Discovery | finite persistence/backup/deployment residuals |

## Selection protocol

1. Finish or safely close the active leaf before activating another writing
   batch.
2. When no leaf is active, choose one dependency-ready finite leaf from the
   already indexed anchors. Before closing the current leaf, register that next
   leaf and its anchors. If no finite item is known, activate one
   `DISC-Px-CLOSURE` row and search only that phase's row/headings to materialize
   child leaves; do not use an empty queue as permission to read the full matrix.
3. Record its ID, outcome, matrix anchors, owned files, acceptance evidence, and
   gate tier here before implementation.
4. Keep only current routing state here. Completed details belong in the matrix
   and Git history, not appended below.

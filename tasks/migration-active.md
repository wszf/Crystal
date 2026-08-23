# Crystal migration active index

Last verified: 2026-08-24 02:56 (Asia/Singapore)

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
- Current project-wide ETA and percentage remain `Unavailable`: P1 now has a
  finite denominator, but ten other phases still have open closure inventories.

## Phase routing summary

| Phase | Matrix status | Scope state | Closure leaf |
|---|---|---|---|
| P0 | Complete | Frozen | — |
| P1 | In progress | Frozen | `DISC-P1-CLOSURE` (Complete) |
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

- Leaf ID: `LOC-P1-CATALOG-001`
- Status: `Active` for routing; CFG is committed at Go
  `fbc69d25f3577ac7d2f16be17bbdbeba10722285`, and LOC writes remain gated only
  until this control transition is committed and both repositories are clean.
- Outcome: provide the exact Legacy 768-key server default catalog and English/
  Chinese overlay behavior without importing client-only UI localization.
- Go matrix anchors to read: `### 2026-08-24 P1 finite closure inventory`, the
  `LOC-P1-CATALOG-001` row, and the row beginning `| P1 |` only.
- Legacy read authority: `Shared/Language.cs`,
  `Server.MirForms/Localization/English.json`,
  `Server.MirForms/Localization/Chinese.json`, and bounded startup/lookup
  consumers required to rule unknown/malformed/fallback behavior.
- Go write authority: `internal/config/localization.go`,
  `internal/config/localization_test.go`,
  `internal/config/server_text_catalog.go`, and
  `internal/config/server_text_catalog_test.go`, plus only the bounded
  localization-load invocation in `internal/config/config.go` needed to
  distinguish Legacy Setup startup from the documented empty-path Go extension.
- Forbidden scope: feature call-site migration, client UI catalogs, network,
  logging, lifecycle, unrelated config parsing, broad matrix edits, and C#
  changes.

`CFG-P1-CONTRACT-001` is Complete in the verified Go candidate. Exact-case and
first-match INI behavior, UInt16 fallback/write-back, default production path,
CRYSTAL extension precedence, service mappings, version MD5 edge behavior, and
startup-visible failures are locked by focused/repeated/race tests. Its required
unexcluded integration run reproduced the established OmaMage flake; the
precisely excluded rerun and vet/build passed as recorded in the handoff.

`DISC-P1-CLOSURE` is Complete in committed control. A bounded
main-agent audit and independent read-only `luna_worker` review both found no
missing, duplicate, cross-phase, or non-observable P1 child. Static evidence
locks 768 default server keys, 981 direct lookups/765 used keys/three unused
aliases, 142 MessageQueue business invocations, and four direct Player/Spawn/
Server logger sites. P1 has ten unfinished leaves and two completed audit items;
this is a P1 denominator only, not a project percentage.

### Protected Go ownership

- CFG is committed at Go `fbc69d25f3577ac7d2f16be17bbdbeba10722285`;
  the Go worktree is clean and no CFG path remains protected.
- Current protected Legacy control paths are recorded in the handoff; the LOC
  write set remains gated until this routing commit is clean.
- Later P1 leaves may share files only serially. The one active leaf owns the
  exact paths above; no subagent may expand them.

### Remaining acceptance work

- [ ] Generate the exact 768-key server default catalog in Go-native form and
      prove its key count against Legacy declarations.
- [ ] Lock the English/Chinese server asset key sets, exact values, and composite
      format placeholder signatures without modifying either Legacy JSON file.
- [ ] Match unknown key, missing file, malformed JSON/value, empty catalog, and
      unwritable generated-file behavior at the production localization entry.
- [ ] Preserve the active missing-Chinese-key English fallback and distinguish it
      from unknown server keys and client-only UI Text/Enum assets.
- [ ] Prove startup loading and environment path overrides with focused/repeated
      catalog tests; add focused race only if shared mutable state is introduced.

### P1 frozen child registry

Every functional leaf also runs the standard leaf gate. `Ready` means its finite
contract is registered; the dependency column determines whether it may be
selected now.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `CFG-P1-CONTRACT-001` | Complete | — | `internal/config/{config.go,config_test.go,p1_contract_test.go}`; bounded `cmd/crystal-server/main.go`; new `cmd/crystal-server/p1_config_startup_test.go` | focused/repeated config + startup |
| `LOC-P1-CATALOG-001` | Active | CFG (Complete) | `internal/config/{localization.go,localization_test.go,server_text_catalog.go,server_text_catalog_test.go}` plus bounded `config.go` loader invocation | catalog/static + startup |
| `LOC-P1-CALLSITE-CLOSURE-001` | Ready | LOC catalog + P2-P11 feature closure | new `docs/p1-localization-callsite-ledger.md`; new `internal/config/localization_coverage_test.go`; matrix evidence only | static ledger + owning transcripts + phase integration |
| `LOG-P1-CATEGORY-001` | Ready | CFG | `internal/logging/{logging.go,logging_test.go}`; `cmd/crystal-server/main.go`; new `runtime_logging.go`/`runtime_logging_test.go` | focused/repeated/race |
| `LOG-P1-CALLSITE-CLOSURE-001` | Ready | LOG category + P2-P11 feature closure | new `docs/p1-logging-callsite-ledger.md`; new `cmd/crystal-server/logging_coverage_test.go`; matrix evidence only | static ledger + representative integration |
| `NET-P1-GATES-001` | Ready | CFG | `cmd/crystal-server/main.go`, `main_test.go`; new `connection_gates.go`/`connection_gates_test.go` | deterministic TCP/repeated/race |
| `NET-P1-STATUS-001` | Ready | CFG | `cmd/crystal-server/main.go`; new `status_service.go`/`status_service_test.go` | TCP cadence/shutdown/race |
| `NET-P1-HTTP-001` | Ready | CFG + P2/P3/P4 authorities | `cmd/crystal-server/main.go`; new `http_service.go`/`http_service_test.go` | HTTP integration/shutdown/race |
| `OPS-P1-LIFECYCLE-001` | Ready | NET status + HTTP | `cmd/crystal-server/main.go`, `main_test.go`; new `process_lifecycle_test.go` | failure injection + integration/full race |
| `OPS-P1-DEPLOY-001` | Ready | all other P1 leaves | `README.md`; new `cmd/crystal-server/deployment_test.go`; matrix evidence | fresh package + full unexcluded gates |
| `NOTICE-P1-EDGE-001` | Complete | — | none | existing notice/session evidence |
| `DOC-P1-EVIDENCE-001` | Complete | — | matrix P1/P12 prose | bounded documentation review |

## Scope-freeze discovery queue

These are registered phase-local inventory audits, not permission to implement
broad unnamed scope.

| Leaf ID | Phase | Status | Required output |
|---|---|---|---|
| `DISC-P1-CLOSURE` | P1 | Complete | 10 unfinished finite children + 2 completed audit items |
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
2. Resume only the one `Active` leaf; do not inventory all remaining phases up
   front.
3. When an active leaf completes, select one dependency-ready child from the
   current frozen phase before opening another phase discovery audit.
4. Keep only current routing state here. Completed details belong in the matrix
   and Git history, not as appended narratives.

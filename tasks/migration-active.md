# Crystal migration active index

Last verified: 2026-08-24 10:31 (Asia/Singapore)

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

- Leaf ID: `DISC-P2-CLOSURE`
- Status: `Active`; lifecycle is Complete and committed at Go
  `cb595b4e40aeffa9f94e22d2deddaeb9abc7b75f`.
- Outcome: convert every vague P2 account/login/password/storage-password,
  static NPC-access, and restart residual into a finite child registry with
  explicit completed evidence, dependencies, ownership, and gates.
- Go matrix anchors to read: the row beginning `| P2 |` only; discovery may
  locate P2-specific headings/rows by exact account/login/password/NPC/restart
  terms, but must not read the full matrix.
- Legacy read authority: bounded real account/login/password/storage-password,
  account/character metadata, static NPC-access, and restart entry points found
  from exact P2 terms; source tracing remains read-only and phase-local.
- Go write authority: `docs/migration-matrix.md` P2 finite inventory/status only;
  Legacy `tasks/migration-active.md` and current handoff are main-agent control.
- Forbidden scope: implementation code, P3-P12 inventory, broad matrix/source
  dumps, deployment packaging, P1 call-site closure, and every C# file.

`CFG-P1-CONTRACT-001` is Complete in the verified Go candidate. Exact-case and
first-match INI behavior, UInt16 fallback/write-back, default production path,
CRYSTAL extension precedence, service mappings, version MD5 edge behavior, and
startup-visible failures are locked by focused/repeated/race tests. Its required
unexcluded integration run reproduced the established OmaMage flake; the
precisely excluded rerun and vet/build passed as recorded in the handoff.

`LOC-P1-CATALOG-001` is Complete at Go `d21681845090f0030e8f214628fd9aa3d60172b7`.
The complete 768-key defaults, exact 766-key English/Chinese assets, two active
English fallbacks, placeholders, unknown/malformed/rewrite/generation behavior,
startup loading, repeated tests, focused race, and integration attribution are
locked by Go-only production/test evidence.

`LOG-P1-CATEGORY-001` is Complete at Go
`0b7a68078e5bc4b182534ca5a5db7260b45267c7`. Production
startup/session wiring now covers all five Legacy categories, the exact rolling
layouts and three 100-entry queues, post-saturation file writes, Player/Spawn
file-only behavior, isolated sink failure/recovery, repeated tests, focused
race, fresh unexcluded integration, and full race.

`DISC-P1-CLOSURE` is Complete in committed control. A bounded
main-agent audit and independent read-only `luna_worker` review both found no
missing, duplicate, cross-phase, or non-observable P1 child. Static evidence
locks 768 default server keys, 981 direct lookups/765 used keys/three unused
aliases, 142 MessageQueue business invocations, and four direct Player/Spawn/
Server logger sites. P1 has three unfinished leaves and nine completed findings;
this is a P1 denominator only, not a project percentage.

`NET-P1-HTTP-001` is Complete at Go `dbbe12e8f53f944250a1bef52ba4a1446e491622`. Full configured
URI authority, trusted IP, source-order query merging/current-culture routing,
exact routes/headers/error prefix/non-GET behavior, Windows/BOM name-list
semantics, POST lifetime, account metadata/counter, JSON and version-117
shutdown persistence, all-player Shout2 broadcast, active-handler shutdown,
focused/repeated/race, and fresh unexcluded full tests/full race/vet/build are
locked. A read-only `luna_worker` re-review found six issues; five are resolved,
and the imported higher-than-retained NextAccountID header is registered below
as finite P12 authority rather than hidden HTTP work.

`OPS-P1-LIFECYCLE-001` is Complete at Go
`cb595b4e40aeffa9f94e22d2deddaeb9abc7b75f`. Operator state/control,
bootstrap/bind quirks, exact lifecycle messages, HTTP→game→status→persistence
shutdown, reason-0 normal fan-out, bounded reason-3→listener-close→reason-0
fatal ordering under concurrent Stop, focused/repeated/race, fresh unexcluded
full tests/full race, vet/build, and Plan 9/Windows/Linux builds are locked.

### Protected Go ownership

- LOG code and matrix evidence are committed at Go `0b7a680`; no Go LOG path
  remains protected.
- NET gate code and evidence are committed at Go `46c1b81`; no gate file remains
  protected.
- NET status code and evidence are committed at Go `269590b`; no status path
  remains protected.
- NET HTTP code/evidence is committed at Go `dbbe12e`; no HTTP path remains
  protected.
- OPS lifecycle code/evidence is committed at Go `cb595b4`; no lifecycle path
  remains protected.
- The active discovery leaf owns only bounded P2 matrix inventory prose; no
  implementation path is writable until a finite child is selected.

### Remaining acceptance work

- [ ] Reconcile every clause of the P2 stage row to current Legacy entry points,
      Go production evidence, or one finite missing child.
- [ ] Separate account service/protocol, metadata timestamps, storage password,
      static NPC access, and restart/import dependencies without duplication.
- [ ] Name exact child ownership/dependencies/gates and obtain independent
      read-only review before marking P2 scope frozen.
- [ ] Select one dependency-ready P2 child; do not implement during discovery.

### P1 frozen child registry

Every functional leaf also runs the standard leaf gate. `Ready` means its finite
contract is registered; the dependency column determines whether it may be
selected now.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `CFG-P1-CONTRACT-001` | Complete | — | `internal/config/{config.go,config_test.go,p1_contract_test.go}`; bounded `cmd/crystal-server/main.go`; new `cmd/crystal-server/p1_config_startup_test.go` | focused/repeated config + startup |
| `LOC-P1-CATALOG-001` | Complete | CFG (Complete) | `internal/config/{localization.go,localization_test.go,server_text_catalog.go,server_text_catalog_test.go}` plus bounded `config.go` loader invocation | catalog/static + startup |
| `LOC-P1-CALLSITE-CLOSURE-001` | Ready | LOC catalog + P2-P11 feature closure | new `docs/p1-localization-callsite-ledger.md`; new `internal/config/localization_coverage_test.go`; matrix evidence only | static ledger + owning transcripts + phase integration |
| `LOG-P1-CATEGORY-001` | Complete | CFG | `internal/logging/{logging.go,logging_test.go}`; `cmd/crystal-server/main.go`; new `runtime_logging.go`/`runtime_logging_test.go` | focused/repeated/race |
| `LOG-P1-CALLSITE-CLOSURE-001` | Ready | LOG category + P2-P11 feature closure | new `docs/p1-logging-callsite-ledger.md`; new `cmd/crystal-server/logging_coverage_test.go`; matrix evidence only | static ledger + representative integration |
| `NET-P1-GATES-001` | Complete | CFG | `cmd/crystal-server/main.go`, `main_test.go`; new `connection_gates.go`/`connection_gates_test.go` | deterministic TCP/repeated/race |
| `NET-P1-STATUS-001` | Complete | CFG | `cmd/crystal-server/main.go`; new `status_service.go`/`status_service_test.go` | TCP cadence/shutdown/race |
| `NET-P1-HTTP-001` | Complete | CFG + P2/P3/P4 authorities | `cmd/crystal-server/main.go`; new `http_service.go`/`http_service_test.go`; bounded `internal/auth/service.go`/`service_test.go` adapter | HTTP integration/shutdown/persistence/race |
| `OPS-P1-LIFECYCLE-001` | Complete | NET status + HTTP | `cmd/crystal-server/main.go`, `main_test.go`; new `process_lifecycle_test.go` | failure injection + integration/full race |
| `OPS-P1-DEPLOY-001` | Ready | all other P1 leaves | `README.md`; new `cmd/crystal-server/deployment_test.go`; matrix evidence | fresh package + full unexcluded gates |
| `NOTICE-P1-EDGE-001` | Complete | — | none | existing notice/session evidence |
| `DOC-P1-EVIDENCE-001` | Complete | — | matrix P1/P12 prose | bounded documentation review |

## Scope-freeze discovery queue

These are registered phase-local inventory audits, not permission to implement
broad unnamed scope.

| Leaf ID | Phase | Status | Required output |
|---|---|---|---|
| `DISC-P1-CLOSURE` | P1 | Complete | 10 unfinished finite children + 2 completed audit items |
| `DISC-P2-CLOSURE` | P2 | Active | finite account/NPC-access/restart children |
| `DISC-P3-CLOSURE` | P3 | Discovery | finite startup/admin/ranking children |
| `DISC-P4-CLOSURE` | P4 | Discovery | finite map/bootstrap/visibility children |
| `DISC-P5-CLOSURE` | P5 | Discovery | finite spell/combat/AI/respawn/packet children |
| `DISC-P6-CLOSURE` | P6 | Discovery | finite item/equipment/craft children |
| `DISC-P7-CLOSURE` | P7 | Discovery | finite NPC/shop/quest/script children |
| `DISC-P9-CLOSURE` | P9 | Discovery | finite guild/war/territory children |
| `DISC-P10-CLOSURE` | P10 | Discovery | finite economy-system children |
| `DISC-P11-CLOSURE` | P11 | Discovery | finite miscellaneous-system children |
| `DISC-P12-CLOSURE` | P12 | Discovery | finite persistence/backup/deployment children |

### Registered cross-phase finding

- `PERSIST-P12-ACCOUNT-ID-001` (`Ready` input to `DISC-P12-CLOSURE`): preserve
  the version-117 `Server.MirADB` header `NextAccountID` through Go import,
  retained-record gaps, checkpoint write, restart, and the next account create.
  Live/JSON HTTP counters are complete; this finite importer/checkpoint gap is
  P12-owned and does not reopen P1 HTTP transport behavior.
- `BOOT-P4-STARTPOINT-001` (`Ready` input to `DISC-P4-CLOSURE`): preserve the
  mandatory nonempty Legacy `StartPoints` startup gate from authoritative
  `SafeZoneInfo.StartPoint` map/bootstrap data and its localized failure.
- `PERSIST-P12-CANSTART-DBCHECKS-001` (`Ready` input to
  `DISC-P12-CLOSURE`, dependent on P5/P6 catalogs): preserve
  `EnforceDBChecks`, first-missing monster/item source order, configured-name
  suffixes, and the disabled-check bypass after imported catalogs are complete.

## Selection protocol

1. Verify this index and the current handoff against each repository separately.
2. Resume only the one `Active` leaf; do not inventory all remaining phases up
   front.
3. When an active leaf completes, select one dependency-ready child from the
   current frozen phase before opening another phase discovery audit.
4. Keep only current routing state here. Completed details belong in the matrix
   and Git history, not as appended narratives.

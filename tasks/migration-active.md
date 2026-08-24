# Crystal migration active index

Last verified: 2026-08-24 22:15 (Asia/Singapore)

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
- Current project-wide ETA and percentage remain `Unavailable`: P1, P2 and P3
  now have finite denominators, but eight other phases still have open
  inventories.

## Phase routing summary

| Phase | Matrix status | Scope state | Closure leaf |
|---|---|---|---|
| P0 | Complete | Frozen | — |
| P1 | In progress | Frozen | `DISC-P1-CLOSURE` (Complete) |
| P2 | In progress | Frozen | `DISC-P2-CLOSURE` (Complete) |
| P3 | In progress | Frozen | `DISC-P3-CLOSURE` (Complete) |
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

- Leaf ID: `RANK-P3-CHAR-LIFECYCLE-001`
- Status: `Active`; `CHAR-P3-BAN-DELETE-001` is Complete at Go
  `c0f70e833b15599d8acb9ad3ebf423cb731fc6a9`.
- Outcome: preserve ranking seed eligibility, non-admin/non-deleted/13-day
  filtering, StartGame and level-up updates, deletion and GM-login removal
  quirks, class-list stale-entry/zero-based re-rank defects, RankType 6 and
  offset no-response boundaries, and reachable old-timestamp detail resend.
- Go matrix anchors to read: only the P3 finite-inventory row
  `RANK-P3-CHAR-LIFECYCLE-001`, exact P3 stage row, and evidence headings/tests
  named by that row. Do not read the full matrix.
- Legacy read authority, closed until tracing: exact rank seed/load/update/
  remove/query consumers in `AccountInfo`, `Envir`, `PlayerObject`, and
  `MirConnection`, plus exact ranking packet serializers. Every C# file stays
  read-only and audit tooling stays Go-only.
- Tentative Go write authority, closed until tracing: bounded
  `internal/auth/{ranking.go,service.go}` and focused tests; bounded
  `cmd/crystal-server/{ranking.go,main.go}` plus one focused lifecycle session
  test; and `docs/migration-matrix.md`. Refine exact existing/new paths before
  any Go code write.
- Required gate: load/admin/deleted/age seed matrix; StartGame/level-up/delete/
  GM-login transitions; exact overall/class stale rows and `MyRank`; zero-based
  re-rank defects, RankType 6/offset silence, timestamp/detail resend, production
  transcripts, repeated/race, cadence-aware integration, diff/status/process,
  and all six C# gates.
- Forbidden scope: completed ban/delete/auth/admin mutation, StartGame map/
  bootstrap/logout lifecycle, P2 metadata, P11 ranking-core redesign, native
  client UI/no-producer navigation, broad matrix/source dumps, and every C#
  write.

`CHAR-P3-BAN-DELETE-001` is Complete at Go
`c0f70e833b15599d8acb9ad3ebf423cb731fc6a9`. Exact wrong-stage lifetime,
delete result/source order, retained/repeated/duplicate/signed tombstones,
selection projection, strict persisted ban admission, ordinal-15 packet, JSON/
ordinary-117 restart, importer retention/archive behavior, current ranking
snapshot removal, focused/repeated/race, fresh full/full-race/vet/build, and
independent `no findings` review are locked. Materialized class lifecycle stays
with the active ranking child; corrupt-index 117 re-export stays P12.

`PERSIST-P2-SOURCE-PRECEDENCE-001` is Complete at Go
`4729fed32ded396d36b05c4bdafad6e17e4fc1dd`. Exact production startup with
conflicting JSON/117 account and global sentinels proves JSON authority,
non-merge, graceful final checkpoint replacement, and direct 117 reload under
repeated/race tests. Retained-gap counters and global re-export remain P12.

`CHAR-P3-CREATE-001` is Complete at Go
`a7f16f8bfb5a0ec4e885d5101dce6635bfafd6e5`. Exact Select-
stage dispatch, every result/source-order edge, first-request/one-stale-entry IP
quirks and future admission, strict name range/current-culture DisabledChars,
administrator/class/count/tombstone gates, unchecked IDs, constructor-zero
returned SelectInfo, creation metadata/insertion, JSON counter gaps, and
ordinary retained-record 117 checkpoint/reload are locked by focused,
repeated, race, and fresh unexcluded integration evidence. Level-1 StartGame
initialization remains with its named Start child; higher 117 header gaps remain
P12.

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
- Account-session code and matrix evidence are committed at Go `96ffa15`; no
  account-session path remains protected.
- Source-precedence test/evidence is committed at Go `4729fed`; no P2 source-
  precedence path remains protected.
- Character-creation code/tests/matrix are committed at Go `a7f16f8`; no
  creation path remains protected.
- Account-operator code/tests/matrix are committed at Go `2e6df8a`; no account-
  ops path remains protected.
- Admin-authority code/tests/matrix are committed at Go `02a63ce`; no admin-
  authority path remains protected.
- Ban/delete code/tests/matrix are committed at Go `c0f70e8`; no ban/delete
  path remains protected.
- `RANK-P3-CHAR-LIFECYCLE-001` has only tentative bounded ownership above;
  exact writes remain closed until its source trace ends.

### Remaining acceptance work

- [ ] Trace Legacy rank seed/update/remove/query consumers and refine exact Go
      files without reopening P11 core or completed ban/admin mutation.
- [ ] Implement the bounded lifecycle surface and run production-entry,
      repeated, race, and cadence-aware integration gates.

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

### P2 frozen child registry

Independent read-only reviewer `01a031c0-18ea-71e3-ba9f-b6cf96be57d4`
accepted this exact eight-child denominator after two rounds. Six are Complete
and two are dependency-blocked Ready. This is a P2 denominator
only.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `AUTH-P2-CRYPTO-WIRE-001` | Complete | — | none | existing auth/protocol vectors |
| `AUTH-P2-ACCOUNT-SESSION-001` | Complete | crypto/wire | `internal/auth/service.go` + tests; bounded `cmd/crystal-server/main.go`; new `p2_account_session_test.go`; optional one account-session helper | authenticated repeated/race + JSON/117 |
| `AUTH-P2-CHAR-METADATA-001` | Ready | account session + P3 mutation authority | bounded auth/main + new metadata lifecycle tests | login/logout projections + persistence/race |
| `STORAGE-P2-PASSWORD-001` | Complete | — | none | existing service/protocol/valid-page sessions |
| `STORAGE-P2-NPC-GATE-001` | Ready | `NPC-P7-ACCESS-GATE-001` | bounded storage handlers + new `p2_storage_npc_gate_test.go` | all-handler wrong-stage + NPC boundary/race |
| `PERSIST-P2-ACCOUNT-BRIDGE-001` | Complete | — | none | existing JSON/117/global merge evidence |
| `PERSIST-P2-CHECKPOINT-RESTART-001` | Complete | bridge | none | existing production checkpoint/restart smoke |
| `PERSIST-P2-SOURCE-PRECEDENCE-001` | Complete | bridge | new `p2_account_precedence_test.go`; bounded startup if needed | conflicting-source startup/checkpoint/reload |

### P3 frozen child registry

Independent reviewer `01a0327e-55a1-7f63-9fb7-0b8bdcc061af` accepted this
exact eleven-child denominator after one revision. Eight are Complete,
`RANK-P3-CHAR-LIFECYCLE-001` is Active, and two are dependency-blocked Ready.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `CHAR-P3-WIRE-BOUNDARY-001` | Complete | — | none | existing production packet transcript/vectors |
| `CHAR-P3-START-BOOTSTRAP-001` | Complete | — | none | existing post-admission bootstrap transcripts |
| `ADMIN-P3-RUNTIME-MODES-001` | Complete | seeded/imported authority | none | existing authenticated mode/relogin transcripts |
| `CMD-P3-PUBLIC-UTILITY-001` | Complete | P4/P6/P8 business owners | none | existing command repeated/race transcripts |
| `CHAR-P3-CREATE-001` | Complete | P2 account session (Complete) | bounded `internal/auth/service.go`, `cmd/crystal-server/main.go`, existing tests, one new focused creation session test | results/order + IP + JSON counter + ordinary 117 + repeated/race |
| `CHAR-P3-BAN-DELETE-001` | Complete | P2 projection/import + P11 ranking core | committed auth/main/import/protocol + focused mutation/restart tests | tombstone/ban/index + repeated/race |
| `CHAR-P3-START-LOGOUT-001` | Ready | ban/delete + P2 character metadata | bounded auth/main lifecycle tests | transitions/persistence/race |
| `ADMIN-P3-AUTHORITY-001` | Complete | P1 config/localization/logging + P11 ranking | committed config/auth/logging/game-session | grant/revoke/@LOGIN/relogin/race |
| `ADMIN-P3-ACCOUNT-OPS-001` | Complete | P2 account/storage (Complete) | committed Go operator control + auth/main tests | live/offline JSON/117/race |
| `RANK-P3-CHAR-LIFECYCLE-001` | Active | P11 core + ban/delete + admin authority | bounded auth/ranking/main | deleted/age/admin/lifecycle transcripts |
| `CLIENT-P3-SELECT-PROBE-001` | Ready | all P3 character leaves + P2 metadata | bounded `internal/probe` + session tests | full transition transcript |

## Scope-freeze discovery queue

These are registered phase-local inventory audits, not permission to implement
broad unnamed scope.

| Leaf ID | Phase | Status | Required output |
|---|---|---|---|
| `DISC-P1-CLOSURE` | P1 | Complete | 10 unfinished finite children + 2 completed audit items |
| `DISC-P2-CLOSURE` | P2 | Complete | 8 finite children: 5 Complete + 3 unfinished |
| `DISC-P3-CLOSURE` | P3 | Complete | 11 finite children: 4 Complete + 7 unfinished |
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
- `PERSIST-P12-CHARACTER-ID-001` (`Ready` input to `DISC-P12-CLOSURE`):
  preserve a version-117 `NextCharacterID` header above every retained
  character through import, checkpoint/re-export, restart, and the next create.
  P3 owns allocation after an authoritative counter is installed; retained-gap
  importer/writer authority remains P12.
- `PERSIST-P12-CORRUPT-CHAR-INDEX-001` (`Ready` input to
  `DISC-P12-CLOSURE`): preserve duplicate and nonpositive retained
  `CharacterInfo.Index` values through version-117 checkpoint/re-export and
  restart. P3 owns current-process first-match client lookup/mutation outcomes;
  P12 owns writer normalization/rejection and recovery continuity.
- `NPC-P7-ACCESS-GATE-001` (`Ready` input to `DISC-P7-CLOSURE`): preserve
  normal NPC map/range/visibility and script-page/button authorization plus the
  distinct default-NPC object gate before special pages become active.
- `NPC-P7-SCRIPT-CLOSURE-001` (`Ready` input to `DISC-P7-CLOSURE`): enumerate
  all remaining reachable normal/default-NPC action/condition/page/input
  families without hiding them in P2 storage behavior.
- `PERSIST-P12-RESTART-EQUIV-001` (`Ready` input to `DISC-P12-CLOSURE`,
  dependent on P3-P11 authorities): preserve periodic save, atomic replacement,
  backup, global re-export, and complete multi-store restart/recovery.

## Selection protocol

1. Verify this index and the current handoff against each repository separately.
2. Resume only the one `Active` leaf; do not inventory all remaining phases up
   front.
3. When an active leaf completes, select one dependency-ready child from the
   current frozen phase before opening another phase discovery audit.
4. Keep only current routing state here. Completed details belong in the matrix
   and Git history, not as appended narratives.

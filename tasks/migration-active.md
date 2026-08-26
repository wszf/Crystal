# Crystal migration active index

Last verified: 2026-08-26 17:08 (Asia/Singapore)

This is the concise execution router for the persistent migration Goal. The Go `docs/migration-matrix.md` remains the detailed status/evidence authority; do not copy its narratives here or read the full matrix during normal recovery.
Keep this file at or below 300 lines and 32 KiB.

## Progress semantics

- One main implementation leaf may be `Active`.
- A phase is scope-frozen only after its closure audit converts every vague
  residual into finite child leaves.
- Scope is frozen phase by phase; the whole project's remaining inventory does
  not need to be calculated in one up-front pass.
- `Complete` phase labels are not a project percentage. Leaf burn-down and ETA
  are publishable only for a scope-frozen phase.
- Current project-wide ETA/percentage remain `Unavailable`: P1-P6 are finite, but five other phases still have open inventories.

## Phase routing summary

| Phase | Matrix status | Scope state | Closure leaf |
|---|---|---|---|
| P0 | Complete | Frozen | — |
| P1 | In progress | Frozen | `DISC-P1-CLOSURE` (Complete) |
| P2 | In progress | Frozen | `DISC-P2-CLOSURE` (Complete) |
| P3 | Complete | Frozen | `DISC-P3-CLOSURE` (Complete) |
| P4 | In progress | Frozen | `DISC-P4-CLOSURE` (Complete) |
| P5 | In progress | Frozen | `DISC-P5-CLOSURE` (Complete) |
| P6 | In progress | Frozen | `DISC-P6-CLOSURE` (Complete) |
| P7 | In progress | Open | `DISC-P7-CLOSURE` |
| P8 | Complete | Frozen | — |
| P9 | In progress | Open | `DISC-P9-CLOSURE` |
| P10 | In progress | Open | `DISC-P10-CLOSURE` |
| P11 | In progress | Open | `DISC-P11-CLOSURE` |
| P12 | In progress | Open | `DISC-P12-CLOSURE` |

## Active batch

- Leaf ID: `COMBAT-P5-HP-DRAIN-001`
- Status: `Active`; P5 is scope-frozen with ten of fifteen children Complete,
  this one Active and four Ready. Combat core and Human/Hero regen are Complete
  dependencies.
- Outcome: migrate the reachable `HumanObject.HpDrain` float accumulator from
  successful Human/Monster target attacks through strict payout, HP cap loss,
  Player/Hero notifications and runtime reset semantics.
- Go matrix anchors to read: only P5 summary row 851, this registry row 3164 and
  completed Regen evidence 3184-3199; never the full matrix/another phase.
- Legacy read authority: bounded `HumanObject.HpDrain` declaration and every
  write/read/payout callsite, the invoked Human/Monster `Attacked`/attack
  families, HP change/notification helpers and runtime construction/reset paths;
  every C# file is read-only.
- Exact Go write authority: existing `cmd/crystal-server/{equipment_transactions.go,
  world.go,warrior_attack.go,heroes.go}`; new `hp_drain.go`,
  `hp_drain_test.go` and `hp_drain_session_test.go`. No other file may change.
- Required gate: touched compile; exact float32 threshold/remainder/cap tests;
  Player/Hero versus Human/ordinary-Monster production paths; damageWeapon and
  miss gates; packet/persistence order; logout/relogin/restart reset;
  count-20, race-count-5 and authenticated attack transcript.
- Forbidden scope: natural regen redesign, SafeZoneHealing, Revelation expiry,
  unrelated attack refactors, another P5 child and every C# write.

### Protected Go ownership

- Completed Regen commit
  `194c209b46f84876c577085c94b5b3983178691a` remains read-only until the
  bounded trace explicitly opens exact HP-drain files; earlier P1-P6 evidence
  may be consumed, not redesigned.

### Remaining acceptance work

- [ ] Trace every reachable Legacy/Go HP-drain callsite and freeze exact Go files.
- [ ] Implement only the finite HP-drain behavior and production-entry tests.
- [ ] Run the leaf gate, obtain bounded review, commit and route the next
  dependency-ready child.

### Frozen behavior checklist

- Only `Attacked(HumanObject, ..., damageWeapon=true)` after hit and strict
  `armour < damage` accumulates. Player/Hero targets use the Human base path;
  ordinary Monster subclasses that reach `MonsterObject.Attacked` use the
  Monster path. Monster attackers and overrides returning before base do not.
- A Hero hitting a Human target is normalized to its Player owner before armour,
  stats, durability and drain; a Hero hitting an ordinary Monster keeps its own
  stats, accumulator and HP.
- Accumulate float32 `max(0,(float32(damage-armour)/100)*rate)`. Payout only at
  strict `accumulator > 2`; floor to int, call capped HP change, then subtract
  the full floored amount even when full HP loses the heal. Exact 2 is retained.
- Player payout emits ordinary ChangeHP private/group/Revelation health packets
  and persistence, but no regen indicator. Hero payout emits HeroHealthChanged,
  owner/group ObjectHealth and owner ObjectMana, then durable Hero HP.
- Player-on-Human payout precedes target Struck/ObjectStruck/indicator/damage;
  Player/Hero-on-Monster payout follows ObjectStruck and precedes gather,
  indicator and target HP. Accumulators are runtime-only zero defaults and are
  discarded by logout/relogin/restart.

### P6 frozen child registry

Independent Legacy/Go auditors produced the finite denominator. Reviewer
`01a037ed-f35d-7d23-a532-803fdce5a5ff` required two correction rounds and then
accepted all nineteen children with no finding. Twelve are Complete and seven are Ready.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `ITEM-P6-WIRE-CATALOG-001` | Complete | — | existing committed evidence | layouts/nesting/grids/definitions |
| `ITEM-P6-GRID-MUTATION-001` | Complete | logging/localization | committed/accepted grid mutation evidence | error/report/response order + normalized revision/CAS |
| `ITEM-P6-GRID-CROSS-001` | Ready | P8/P11 feature owners | item/storage/trade/Hero/equipment | exact positive/negative grid matrix |
| `CAPACITY-P6-GRIDS-001` | Ready | P10 gold | command/protocol/auth storage | 46-86, 80-160, expiry/restart |
| `ADMIN-P6-ITEM-COMMAND-001` | Complete | P3 authority + P5 creation | committed Go `9e7edac7aaf22f35677f90427ca79da4e998b096` | MAKE/CLEARBAG quirks |
| `EQUIP-P6-CORE-001` | Ready | P8/P11 feature owners | equipment/session | slots/sockets/stats/order/race |
| `ITEM-P6-USE-ADMISSION-001` | Complete | P1 LOC + P5 stats | committed Go `f6e9f2ba69d3a1cb0e7d37536e726c3e2a666cd2` | all gates and localized order |
| `ITEM-P6-USE-BASIC-001` | Complete | — | existing committed evidence | potion/delete success paths |
| `ITEM-P6-USE-CATALOG-001` | Ready | P4/P5/P7/P9/P10 | item use/session | exact known/unknown shape partition |
| `ITEM-P6-SHOUT-ARMING-001` | Complete | — | committed Go `1d399992a690614a122cc46b3e64b4cda8272c2f` | Hint/UseItem/state/repeated/race |
| `DROP-P6-GROUND-LIFECYCLE-001` | Ready | P5/P11 producers | ground/drop/death/session | actual death ground drops |
| `ITEM-P6-EXPIRY-001` | Ready | P10/P12 consumers | item lifecycle/ticker/session | strict times/no-refresh/restart |
| `STORAGE-P6-ACCOUNT-001` | Complete | P2/P7 final access | existing committed evidence | default 80-slot boundary |
| `TRADE-P6-PLAYER-001` | Complete | P10 mail/economy | existing committed evidence | two-peer lifecycle/race |
| `RENTAL-P6-LIFECYCLE-001` | Complete | P10 owner + P12 restart | existing committed evidence | expiry/death/return/idempotency |
| `REPAIR-P6-NPC-001` | Complete | P7 page authority | existing committed evidence | formulas/order/persistence |
| `REFINE-P6-WORKBENCH-001` | Ready | P7 page authority | refine/session | delayed production/restart/race |
| `CRAFT-P6-NPC-001` | Complete | P7/P10 | existing committed evidence | ingredients/RNG/order/persistence |
| `MINE-P6-RUBBLE-001` | Complete | P4 map + P5 adapters | config/schema/world/session | RNG/timers/payout/restart/race |

### P5 frozen child registry

Independent Legacy auditor `01a036ed-ee8f-7a50-a042-f1d665f83627` confirmed
211 mapped/45 default AI ordinals and both map-hazard producers. Independent
reviewer `01a036ff-6ca2-7f50-ada6-1c68c2ded15d` accepted the original eleven
children. Hazard Struck tracing proved one finite omitted regen child; its bounded attack
trace then proved HP-drain combat and optional SafeZoneHealing children. Regen review
proved one reachable long-Revelation expiry-width correction. Ten are Complete,
one is Active and four are Ready.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `SPELL-P5-PLAYER-CATALOG-001` | Complete | — | existing committed evidence | 130-ID partition / 109 user spells |
| `SPELL-P5-INTERNAL-EFFECT-001` | Complete | mapped monster owners | existing committed evidence | 16 internal effects |
| `SPELL-P5-MAP-HAZARD-001` | Complete | map load | accepted hazard schema/runtime/session evidence | parser/export + timers/RNG/session/race |
| `COMBAT-P5-HUMAN-HERO-001` | Complete | spell/state consumers | existing committed evidence | target/defence/death/relogin |
| `COMBAT-P5-HP-DRAIN-001` | Active | combat core | bounded human/monster attacked paths + focused tests | float accumulation/strict payout/remainder/packets/race |
| `REGEN-P5-HUMAN-001` | Complete | combat + P1 weights + P6 potion pools | committed Go `194c209b46f84876c577085c94b5b3983178691a` | natural/Pot/Heal/Vamp timers, resets, packets, persistence/race |
| `SPELL-P5-REVELATION-EXPIRE-001` | Ready | Human regen review | revelation helper/tests + owning transcripts | 255/256/260 byte wrap, completion/passive/bootstrap/race |
| `REGEN-P5-SAFEZONE-001` | Ready | Human regen + map safe zones | config/map healing objects + production tests | enabled/disabled, cells/ticks/recipients/restart/race |
| `STATE-P5-EFFECT-LIFECYCLE-001` | Complete | — | existing committed evidence | recipients/expiry/persistence/race |
| `MONSTER-P5-MAPPED-AI-001` | Complete | — | existing committed evidence | 201 mapped ordinals |
| `MONSTER-P5-BASE-FAMILY-001` | Ready | mapped core | monster_ai/world + focused tests | 46 ordinals + target-kind/race |
| `MONSTER-P5-ORDINARY-SPAWN-001` | Ready | base family | archer_summons/hell_lord/world + focused tests | AI=60-63/99 + restart/race |
| `DROP-P5-LOOT-HARVEST-001` | Complete | item schema consumer | existing committed evidence | randomness/owner/inventory/ground |
| `RESPAWN-P5-POPULATION-001` | Complete | — | existing committed evidence | strict timing/order/restart/race |
| `WIRE-P5-CONTRACT-001` | Complete | functional owners | existing committed evidence | fixed vectors/transcripts |

### P4 frozen child registry

Independent reviewer `01a03617-76c2-7652-ab64-7e27143ce72b` rejected two
candidate revisions, then accepted this ten-child denominator with no remaining
finding. Eight children are Complete and two dependency-blocked children are unfinished.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `MAP-P4-LOAD-001` | Complete | — | committed Go `7cceb308cef7232a1b02c64c301adc1f89275e9a` | failure continuation/order + repeated/race |
| `BOOT-P4-STARTPOINT-001` | Complete | map load + LOC | committed Go `6db6bbb002fe7e74d8b51473409252f5c407455e` | localized pre-bind rejection |
| `ENTRY-P4-LOCATION-001` | Complete | P3 start/logout | none | existing transition transcripts |
| `MOVE-P4-ACTION-001` | Complete | — | none | existing movement/repeated/race |
| `MOVE-P4-COLLISION-RULES-001` | Ready | P9 conquest state | bounded world/main + RequiredGroup test | authenticated gate/order matrix |
| `VIS-P4-OBJECT-001` | Complete | P5 hide/show producers (Complete) | committed/accepted visibility evidence | recipient/filter/re-entry matrix |
| `MAP-P4-DETAIL-001` | Complete | map load + StartPoint | verified Go protocol/probe/map-info candidate | payload/order/suppression/search |
| `MAP-P4-LIGHT-001` | Complete | — | committed Go `fc6b98b6b312357d850f6710fbf697b8678fd4c2` | UTC production clock/non-UTC host |
| `MAP-P4-NOREINCARNATION-AUTH-001` | Complete | P5 spell (Complete) | committed Go `1aef1eac1953ca184d7e96c5394ae3c2cc5a3cc3` | imported denial + unchanged amulet/no successful cast |
| `CHAT-P4-MAP-CONTEXT-001` | Ready | P6 shout arming | map-rule import + bounded chat/session | NoNames/shout recipients/order |

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
accepted this exact eight-child denominator after two rounds. Seven are Complete
and one remains dependency-blocked Ready. This is a P2
denominator only.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `AUTH-P2-CRYPTO-WIRE-001` | Complete | — | none | existing auth/protocol vectors |
| `AUTH-P2-ACCOUNT-SESSION-001` | Complete | crypto/wire | `internal/auth/service.go` + tests; bounded `cmd/crystal-server/main.go`; new `p2_account_session_test.go`; optional one account-session helper | authenticated repeated/race + JSON/117 |
| `AUTH-P2-CHAR-METADATA-001` | Complete | account session + P3 mutation authority (Complete) | verified auth/main metadata lifecycle candidate | login/logout projections + persistence/race |
| `STORAGE-P2-PASSWORD-001` | Complete | — | none | existing service/protocol/valid-page sessions |
| `STORAGE-P2-NPC-GATE-001` | Ready | `NPC-P7-ACCESS-GATE-001` | bounded storage handlers + new `p2_storage_npc_gate_test.go` | all-handler wrong-stage + NPC boundary/race |
| `PERSIST-P2-ACCOUNT-BRIDGE-001` | Complete | — | none | existing JSON/117/global merge evidence |
| `PERSIST-P2-CHECKPOINT-RESTART-001` | Complete | bridge | none | existing production checkpoint/restart smoke |
| `PERSIST-P2-SOURCE-PRECEDENCE-001` | Complete | bridge | new `p2_account_precedence_test.go`; bounded startup if needed | conflicting-source startup/checkpoint/reload |

### P3 frozen child registry

Independent reviewer `01a0327e-55a1-7f63-9fb7-0b8bdcc061af` accepted this
exact eleven-child denominator after one revision. All eleven children are
Complete; P3 is scope-frozen and Complete.

| Leaf ID | Status | Dependency | Go write authority | Additional gate |
|---|---|---|---|---|
| `CHAR-P3-WIRE-BOUNDARY-001` | Complete | — | none | existing production packet transcript/vectors |
| `CHAR-P3-START-BOOTSTRAP-001` | Complete | — | none | existing post-admission bootstrap transcripts |
| `ADMIN-P3-RUNTIME-MODES-001` | Complete | seeded/imported authority | none | existing authenticated mode/relogin transcripts |
| `CMD-P3-PUBLIC-UTILITY-001` | Complete | P4/P6/P8 business owners | none | existing command repeated/race transcripts |
| `CHAR-P3-CREATE-001` | Complete | P2 account session (Complete) | bounded `internal/auth/service.go`, `cmd/crystal-server/main.go`, existing tests, one new focused creation session test | results/order + IP + JSON counter + ordinary 117 + repeated/race |
| `CHAR-P3-BAN-DELETE-001` | Complete | P2 projection/import + P11 ranking core | committed auth/main/import/protocol + focused mutation/restart tests | tombstone/ban/index + repeated/race |
| `CHAR-P3-START-LOGOUT-001` | Complete | ban/delete + P2 character metadata (Complete) | committed auth/main/world/schema lifecycle tests | transitions/persistence/race |
| `ADMIN-P3-AUTHORITY-001` | Complete | P1 config/localization/logging + P11 ranking | committed config/auth/logging/game-session | grant/revoke/@LOGIN/relogin/race |
| `ADMIN-P3-ACCOUNT-OPS-001` | Complete | P2 account/storage (Complete) | committed Go operator control + auth/main tests | live/offline JSON/117/race |
| `RANK-P3-CHAR-LIFECYCLE-001` | Complete | P11 core + ban/delete + admin authority | committed auth/ranking/main | deleted/age/admin/lifecycle transcripts |
| `CLIENT-P3-SELECT-PROBE-001` | Complete | all P3 character leaves + P2 metadata (Complete) | committed/accepted bounded `internal/probe` + session tests | full transition transcript + phase closure gates |

## Scope-freeze discovery queue

These are registered phase-local inventory audits, not permission to implement
broad unnamed scope.

| Leaf ID | Phase | Status | Required output |
|---|---|---|---|
| `DISC-P1-CLOSURE` | P1 | Complete | 10 unfinished finite children + 2 completed audit items |
| `DISC-P2-CLOSURE` | P2 | Complete | 8 finite children: 5 Complete + 3 unfinished |
| `DISC-P3-CLOSURE` | P3 | Complete | 11 finite children: all Complete |
| `DISC-P4-CLOSURE` | P4 | Complete | 10 finite children: 2 Complete + 8 unfinished |
| `DISC-P5-CLOSURE` | P5 | Complete | 15 finite children after regen/HP-drain/safe-zone/Revelation findings: 8 Complete + 7 unfinished |
| `DISC-P6-CLOSURE` | P6 | Complete | 19 finite children: 7 Complete + 12 unfinished at freeze |
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

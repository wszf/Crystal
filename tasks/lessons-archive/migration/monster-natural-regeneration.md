### 2026-08-27 — Empty HealAmount passes still advance Monster HealTime

- Symptom: the first implementation advanced `HealingReadyAt` only while the
  pool was positive, so healing added after an empty process could drain
  immediately.
- Root cause: the old Go helper short-circuited on `HealingAmount <= 0`, while
  Legacy `MonsterObject.ProcessRegen` updates `HealTime` whenever strict
  `Envir.Time > HealTime`, independently of the pool value.
- Prevention: model the strict timer transition before testing the pool; test
  empty pass, equality, one-nanosecond-after, and later pool admission.
- Verification: terminal Legacy reviewer found the gap; the corrected focused
  count-10 and race count-3, fresh full tests/full race, vet and build exit 0.

### 2026-08-27 — First Regen due requires the complete constructor RNG stream

- Symptom: non-base constructors initially consumed only `Next(10000)` before
  respawn-cell selection, producing the wrong Regen deadline and subsequent RNG.
- Root cause: BaseFamily had migrated the inherited `100/8/10000/3000/1000`
  prefix, while specialized constructors retained partial compensating draws
  and package-global randomness.
- Prevention: route the full inherited prefix, every concrete constructor draw
  and the cell draw through one injected stream. Keep non-Regen field projection
  in its registered specialized-constructor leaf instead of silently reopening
  completed processors.
- Verification: AI=4 locks the prefix and unit-bound cell draw; Deer locks
  `100,8,10000,3000,1000,7,8,1`; two independent reviewers accepted the
  current natural-Regen boundary with no remaining finding.

### 2026-08-27 — Periodic Monster behavior must distinguish impossible zero fixtures

- Symptom: focused/full gates gained unrelated DamageIndicator packets and HP
  changes in HealingCircle, poison, revival, combat and session transcripts.
- Root cause: hand-built `worldMonster` values have a zero constructor deadline,
  while every production-materialized Monster has a nonzero deadline; loaded
  session fixtures also crossed real deadlines while testing another behavior.
- Prevention: treat zero `MonsterAIRegenAt` as an unmaterialized test seam;
  freeze non-target regen timers in loaded transcript fixtures, but preserve
  same-process regen where Legacy `ProcessAI` revives before `ProcessRegen`.
- Verification: exact affected tests pass repeatedly; the final fresh full and
  full-race gates pass after the fixture corrections. The specialized-
  constructor package gate later exposed the same omission in the Slaying
  session: a constructor-timed +5 Monster heal inserted `DamageIndicator=75`
  into its attack transcript. Freezing the non-target deadline across the eight
  loaded warrior transcript fixtures made all eight pass ten times before the
  package gate was rerun successfully.

### 2026-08-27 — Constructor suffix audits must include timer helper calls

- Symptom: specialized-constructor review found that factory FrostTiger cells
  were selected before its constructor-time `Random.Next(120000)` draw; Go
  deferred the draw until the first AI tick.
- Root cause: the frozen suffix ledger searched direct random assignments but
  missed the constructor call to `NewSitDownTime()`, whose helper owns the draw.
- Prevention: trace every concrete constructor into timer/initialization helpers
  before freezing the suffix ledger, and assert each suffix before the cell draw.
- Verification: factory AI 34 now records
  `100,8,10000,3000,1000,120000,cell`, projects the exact SitDown deadline, and
  the specialized/FrostTiger focused tests pass ten times.

### 2026-08-27 — Constructor projection must include fixed deadlines and flags

- Symptom: terminal review found factory HumanAssassin, BombSpider, Hugger,
  PoisonHugger, HellBomb and TrapRock first ticks starting fixed constructor
  fields late or not at all, despite correct inherited RNG projection.
- Root cause: the ledger focused on random draws and direction overrides, while
  fixed `Envir.Time + duration` assignments and `Summoned`/visibility flags were
  left in dynamic creators or lazy processors.
- Prevention: freeze every concrete constructor assignment, including fixed
  deadlines and booleans, then distinguish constructor state from Spawned and
  dynamic-creator overrides before accepting the field matrix.
- Verification: the 210-AI factory test now locks all six deadline families and
  HumanAssassin `Summoned`; focused normal count-10 and race count-3 pass after
  the terminal review corrections.

### 2026-08-27 — Delayed dynamic children are constructed before they are registered

- Symptom: RootSpider, HellKnight, SepHighTaoist, StoneTrap, SummonSkeleton and Mirroring consumed only partial constructor draws at admission, then allocated/materialized the child when the delayed spawn fired; immediate creators split the inherited prefix between package-global and world RNG streams.
- Root cause: the Go delayed-action records captured definitions and compensating fields rather than the constructor-complete object that Legacy passes to `DelayedType.Spawn`/`CompleteMagic`.
- Prevention: materialize every dynamic child with the creator's locked world stream at the Legacy `GetMonster` point, retain the unregistered value by reserved ObjectID, and only place/register/broadcast it at the delayed spawn boundary. Spawn-time overrides must not consume the constructor twice.
- Verification: the finite 29-call/26-file ledger routes through the dynamic wrapper or pending queue; focused creator and protected constructor/natural-regeneration suites pass after failure-proved fixture corrections.

### 2026-08-27 — Same-tick dynamic children are outside the existing-object process set

- Symptom: a zero first-Regen draw let a FloatingRock child emit `DamageIndicator` later in the same world tick that created it, although Legacy map-object enumeration precedes the creator and delayed-action registration boundary.
- Root cause: Go AI inserted the child before the later aggregate natural-Regen pass, which rebuilt its ID list from the mutated world map.
- Prevention: stamp the actual dynamic registration time, skip both common AI and natural Regen when it equals the current tick, and retain the stamp rather than clearing it before the later Regen phase; delayed children replace construction time with registration time when they enter the map.
- Verification: the exact zero-boundary failure no longer emits a same-tick packet; protected natural-Regen/BaseFamily/specialized-constructor tests and the dynamic creator focused set exit 0.

### 2026-08-27 — Spawned packet projection must follow concrete GetInfo, not an internal summoned marker

- Symptom: a review inferred that HumanAssassin's constructor-time `Summoned=true`
  made its first `ObjectPlayer.Extra` true, while generic dynamically created
  monsters exposed Go's internal `Summoned` marker through `ObjectMonster.Extra`.
- Root cause: constructor state, the virtual `GetInfo` implementation, base
  `Spawned` broadcast, and derived post-broadcast mutation were collapsed into
  one steady-state packet helper. HumanAssassin actually hard-codes
  `Extra=false`; HumanWizard exposes the constructor value on the first packet;
  base `MonsterObject.GetInfo` leaves Extra false.
- Prevention: project the packet from the concrete Legacy `GetInfo` at the base
  broadcast boundary, then apply the derived `Spawned` mutation to runtime
  state. Never expose a Go-only lifecycle marker through a base packet field.
- Verification: a GeneralMeowMeow production-entry fixture first failed on the
  generic `Extra=true` leak; HumanAssassin/HumanWizard production-entry cases,
  generic ordinary-pet and session transcripts, the exact focused count-10
  reruns, and the full dynamic creator focused group now pass.
- Review ruling: a proposed repeated SummonSkeleton/Mirroring pending-child test
  failed because the second production request is rejected by the Legacy
  1800ms global `CanCast`/SpellTime gate before the 500ms action boundary. The
  unreachable repeat-cancel finding was rejected rather than encoded in helper-
  only behavior.

### 2026-08-27 — Failed Spawn attempts still retain constructor side effects

- Symptom: HornedCommander edge boulders and TrapRock edge children were
  rejected before Go materialization, so invalid cells consumed neither their
  reserved ObjectID nor the inherited constructor RNG prefix.
- Root cause: Go used map validity as a creator admission gate, while Legacy
  calls `GetMonster` first and lets `MonsterObject.Spawn` reject the cell.
- Prevention: place construction and ObjectID allocation at the exact Legacy
  `GetMonster` point; only the Spawned suffix, registration and packets remain
  conditional on successful `Spawn`.
- Verification: production-entry edge fixtures lock all eight rejected
  HornedCommander constructions and TrapRock invalid/valid/invalid ID gaps;
  the dynamic focused count-10 and race count-3 gates pass.

### 2026-08-27 — Restore and item summon use the first Spawned projection

- Symptom: restored or item-summoned derived pets emitted their steady
  post-`Spawned` `Extra` value as the first object packet; the full server gate
  exposed the stale HumanWizard session expectation after correction.
- Root cause: the shared constructor helper returned post-derived runtime state
  and two production callers serialized it directly instead of replaying the
  base `Spawned` broadcast boundary.
- Prevention: every production registration path must serialize the concrete
  pre-derived `GetInfo` projection, then retain the derived runtime mutation;
  login restoration is a real Spawn, not an already-spawned snapshot.
- Verification: Shinsu restore/item production entries assert first
  `Extra=false` and steady `true`; HumanWizard authenticated restore/relogin
  asserts first `true` and steady `false` at count 10 and race count 3.

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
  full-race gates pass after the fixture corrections.

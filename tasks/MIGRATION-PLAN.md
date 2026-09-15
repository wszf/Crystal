# Remaining Crystal → Go migration

Replanned 2026-09-07 from current C# and Go source; archived orchestration is
historical only. C# remains read-only. Implementation belongs in Crystal.GoServer.

## Current evidence

- Go has a working session dispatcher, account/character lifecycle, map loading,
  movement, inventory transactions, combat/spells, monster AI, pets/heroes,
  quests, shops/trade/mail/market, guilds/conquest, and periodic persistence.
  Evidence: `cmd/crystal-server/main.go`, the corresponding runtime files and
  session tests; `internal/auth`, `internal/protocol`, `internal/mapdata`,
  `internal/legacyworld`, and their tests. Presence is not proof of full parity.
- Import/export supports explicit legacy layouts (world/account 117/0 in
  legacyworld; legacyaccount also has a separate 116/0 catalog reader).
  All detected map formats 0–7 and 100 have implementations.
- WORLD actions, CombineItem and corrected monster shield wire IDs are committed
  with focused/protocol/session/race coverage (packages 1–3 below).
- NPC player actions are substantially implemented, but C# NPCSegment also has
  actorless and monster action overloads. These require a separate execution-
  context audit; player-action coverage cannot establish their parity.
- A packet-name comparison is only a discovery aid: ReportIssue is an explicit
  Legacy no-op, and ClientVersion/LogOut/DellMember have differently named Go
  equivalents. Do not count these as missing gameplay from spelling alone.

## Next packages, in order

1. **Land WORLD NPC actions.** Review against NPCSegment, MapObject/HumanObject
   and DropInfo; retain useful pending files, exclude generated Envir/Goods
   data. Acceptance: parser/protocol tests, production NPC session tests,
   pet spawn/death timing, guild reload and DROP reward/capacity checks pass;
   run `go test ./...` before committing the implementation.
2. **Correct monster buff identity across player/monster projections.** Fix
   HornedWarriorShield's wire value and distinguish Commander. Check NPC enum
   lookup, AddBuff/removal and restored visibility against BuffInfo.Load.
   Acceptance: explicit numeric wire assertions and actor/observer lifecycle
   tests, affected monster tests and full Go suite pass.
3. **Implement CombineItem end to end.** Port packet layouts and Legacy
   admission checks, then item-type branches in focused commits using existing
   inventory authority. Acceptance: inventory and spawned-hero success/failure,
   dead/invalid/stale requests, RNG outcomes, packets, save/reload, and no item
   duplication. Read the whole C# method before selecting branch boundaries.
4. **Close NPC execution-context and data-path gaps.** Trace actorless/map and
   monster events from their C# callers into Go; implement missing reachable
   paths. Verify configured NPC/drop/insert roots through production startup
   (npcDropPath currently exists only in test/session construction). Acceptance:
   event-driven tests, parameter isolation, absent actor behavior and scripts
   loaded from a non-default data root; list any unsupported reachable actions.
5. **Rehearse migration and close discovered integration gaps.** With packages
   above in place, use a representative exported world/account dataset to
   exercise login → combat/quest → economy/guild → logout/restart. Compare packet
   traces and persisted balances/items/progress to Legacy. Audit remaining
   packet handlers, spell/AI dispatch and Settings consumers by behavior, then
   fix discrepancies in small commits. Acceptance: full tests, focused race
   checks, clean restart, and a recorded client smoke test with dataset/version.

## Remaining migration beyond individual features

Data/session authority and shared buff/spawn behavior precede reward mutations.
After functional parity, validate timers, concurrent saves, crash/restart recovery,
operator configuration/logging and realistic population/load before cutover.

## Non-goals and blockers

No C# implementation edits, global git configuration changes, new orchestration
framework, speculative server rewrite, or production cutover in these packages.
Retain existing tests even when their historical names mention old milestones.
The isolated 117/0 snapshot validates client/gameplay/restart traces; production
parity and load/cutover remain unproven. Unsupported database versions must error.

## Execution and current acceptance — Packages 1–5 closed

Authoritative current status: Packages 1–5 are accepted and complete. The
detailed execution history below is retained as historical evidence only; it
is not an unfinished work queue. Post-audit work evaluates true completion
against the real C# source and current Go evidence without reopening the
historical Package 5 acceptance decision.

1. WORLD actions landed as Go `3591b2b`; focused parser/runtime/session tests pass.
2. Shield IDs landed as `712134e`; numeric wire and lifecycle/race tests pass.
3. CombineItem landed as `5c87a52` / `a5526c8`: all four branches, player/Hero
   inventory authority, packet order, rejected/replayed requests, RNG, persistence
   and concurrent consumption are covered. Package complete.
4. NPC roots and automatic contexts landed as `8bf588b` / `89daa2d`; event-driven
   context/parameter isolation, configured roots and race checks pass. Package
   complete. Go `docs/NPC-EXECUTION-CONTEXTS.md` records the source audit.
5. Rehearsal and integration acceptance is closed by decision. The post-audit
   true-completion evidence and remaining cutover limits live in Go
   `docs/CSHARP-GO-COMPLETION-AUDIT.md`, `docs/CSHARP-GO-REPLAY.md` and
   `docs/MIGRATION-STATUS.md`.

## Active true-completion replan — 2026-09-12

The current Go completion audit is
`Crystal.GoServer/docs/CSHARP-GO-COMPLETION-AUDIT.md`; its verdict is
functional migration **Partial** and production replacement **No**. The
following is the active, short ranked plan. Dated execution text below remains
historical evidence and must not be read as a revived leaf/matrix queue.

1. **Default-timeout capacity.** Keep the production/default `TimeOut=10000`
   unchanged. The source-confirmed AI cache (`08815a5`), scheduler service
   window (`44e6b1b`), and spatial lookup (`8782ee1`) established bounded
   8/8, 10/10 and 16/16 waves. Follow-up commit `e474a70` reuses sorted monster
   IDs, indexes sparse AI/route populations, avoids unchanged writebacks, and
   initializes playerless base-family monsters at the Legacy inherited
   `CheckAlone` boundary. Three fresh 32-client existing-character waves over
   about 75,743 materialized monsters pass 32/32 in 28.606s, 30.733s and
   33.526s under unchanged `TimeOut=10000`; the earlier 31/32 waves are
   pre-change comparison evidence. `aede46f` indexes due regeneration
   candidates with focused normal and targeted-race coverage. A subsequent
   disposable 50-client diagnostic accepted all connections but reached 0/50
   login-success responses before the ten-second client deadline; its fixture
   used cloned disposable records and is not promoted as a clean acceptance or
   regression result. `4a748f1` adds owner-scoped cleanup indexing, and two
   protocol-created 50-client existing-character waves then pass 50/50 with
   slowest totals of 13.256s and 16.235s under the unchanged timeout. The
   bounded MaxUser lifecycle is now positive. A subsequent four-wave
   200/200 existing-character soak also passes under the unchanged timeout,
   with slowest totals of 15.331s, 17.674s, 16.564s and 17.349s. Extended
   production-duration soak, rollback and production operations evidence
   remain open. A current post-`b08f73a` disposable full-data recheck also
   passed 50/50 bootstrap → turn → logout with a 2.957s slowest client from
   50 unique existing-character sessions, while retaining `MaxUser=50`,
   `TimeOut=10000`, no `CRYSTAL_TIMEOUT_MS` override, and an unchanged
   world-export hash. The post-commit spatial/route/Elephant/cell focused
   tests pass normally and under targeted `-race`; this closes the bounded
   movement/relogin target for the current source, not extended production
   duration or cutover acceptance. The cooldown-aware follow-up then ran
   twelve consecutive 50-client waves (600/600) from a fresh disposable
   runtime, with per-wave slowest clients from 3.638s to 4.279s, while
   honoring the five-second Legacy IP cooldown. Its server log is
   `/tmp/pkg5-rank1-current-soak.u0Q58D/Logs/rank1-12wave/Server/Server (13-09-2026).log`
   (SHA-256
   `ff1fa91425f19d0951566841b3bf485d9856b86537de80d933d3c9f358fb5106`),
   and the world export stayed at SHA-256
   `d45f8ad0fddccb121d5befaea120d94524948576fc74b39313a1d90ab0d2f09e`.
   This strengthens bounded movement/relogin evidence under the unchanged
   default timeout; extended production-duration soak, rollback and cutover
   acceptance remain open.
2. **Behavioral parity evidence.** Same-account PBKDF2 auth (`b58663e`), strict
   4L lifecycle and clean disposable restart are proven. A 2026-09-13
   controlled driver sent 20 attacks to each live target and held the Go socket
   open for delayed-response drain; both combat paths were reached, but Go and
   4L selected different target types/IDs and produced different impact
   traffic. One bounded current credentialed recheck then returned EOF during
   the initial 4L handshake before lifecycle/combat capture. Same-state combat
   outcomes, full payload equality and reward/death equivalence remain open. A
   fresh post-`078533f` source-isolated replay passed the identical eight-request
   lifecycle on Go and 4L; both traces contain 9 `ObjectNPC` and 0
   `NewNPCInfo` frames after the passive-visibility correction. Ambient volume
   still differs (Go 303 versus 4L 145 `NewItemInfo`, and Go 17 versus 4L 11
   `ObjectMonster`), so this narrows but does not close payload parity. An
   opt-in direct credentialed relogin probe also returned EOF before
   `LoginSuccess`; no verdict is softened from the completion audit.
3. **Regression gate.** The current required unfiltered command is
   non-repeatable/open. At Go commit `9a311f7`, the first unchanged run failed
   after 109.116s at `cmd/crystal-server/inspection_session_test.go:120`
   (`ServerKeepAlive` packet 3 expected, mail packet 26 observed) while 21 of
   22 packages passed; an unchanged rerun passed all 22 packages in 108.921s.
   The full-log SHA-256 values are
   `3050369c78b1c0265fe32a9d324b4b5d37aa161248b5e0bca8eecb5a4e4ef179` and
   `6e15320a7dbbe4d79d5eabdfa4a602766add8c67a10c7d9ab5e0590272206ea8`.
   A targeted five-repeat passed. Filtered runs are not a substitute and must
   not hide failures by weakening assertions; the exact package manifest and
   historical disposition remain in the latest Step 0 Rank 3 checkpoint.
4. **Protocol compatibility.** `Crystal.GoServer/docs/CSHARP-GO-COMPATIBILITY-POLICY.md`
   now records the explicit accept/reject decisions: inactive rows are accepted
   only as inactive, wire-only rows have no invented triggers, C/148 remains
   partial, and GameMaster 100 is distinct from Rested 112. Focused ordinal,
   GameMaster and Rested tests pass; strict future-client behavior remains a
   cutover caveat rather than an unclassified inventory gap.
5. **Operations.** Current-source disposable two-generation restart/recovery
   passes in `/tmp/pkg5-ops-current.A9i0pm`; one abrupt disposable kill during
   relogin followed by restart also recovers the persisted character. The
   fresh selected operations batch covers 42 lifecycle/startup/shutdown cases
   and 45 auth/bridge/staged-file/world persistence cases, passing normally
   and under targeted `-race`; the procedure and limits are recorded in
   `Crystal.GoServer/docs/CSHARP-GO-OPERATIONS.md`. Transaction rollback,
   corruption drills, monitoring/alerting and production runbook readiness
   remain unproven. No cutover claim is allowed while the behavioral proof and
   operations boundaries remain.

## Historical true-completion checkpoint — 2026-09-14 (superseded for Rank 3)

This is a historical snapshot before the later `9a311f7` repeatability runs;
its green gate result remains evidence at that revision, not the current Rank 3
verdict. The Step 0 audit recheck remains A=`Partial`, B=`No`; Packages 1–5
remain historically closed and archived orchestration is not revived. The
required post-`0f1324f` unfiltered gate
`go test ./... -count=1 -timeout 5m` passed all 21 listed command/internal
packages, with `cmd/crystal-server` completing in 108.924s. Its captured
transcript is
`/tmp/pkg5-post-0f1324f-unfiltered-20260914.log` (SHA-256
`799f7d82e13ba57a3a22e17675d37a2b0601dcbbf4217dfcd4fe5348bca7de7`). The
QuestP7 failures exposed by `eec03d2` were fixed by `0f1324f`; they are not
baseline skips. Rank 3 is green for this batch.

Rank 1 now has positive bounded evidence: the current full-population
`TimeOut=10000`/`MaxUser=50` run completed 30 consecutive 50-client waves,
1,500/1,500 bootstrap → turn → logout sessions, with a 5.075s maximum client
time and no timeout override. Rank 2's fresh same-account lifecycle passes on
both Go and 4L, but the corrected five-attack Scarecrow captures differ (Go
4 `ObjectStruck`/5 health updates versus 4L 0/0), with unsynchronized target
IDs/coordinates and EOF/read-error cleanup. Therefore lifecycle is Pass while
same-state combat and full-payload parity remain Fail/open. Details and trace
hashes are in Go `docs/CSHARP-GO-COMPLETION-AUDIT.md` Section 4.20 and
`docs/CSHARP-GO-REPLAY.md`.

Continue the active ranked work in order: (1) retain the measured default
`TimeOut=10000` performance boundary and document or improve any remaining
production-scale scheduler/long-duration evidence without increasing the
timeout; (2) continue paired same-account Go/4L payload and controlled combat
work, synchronizing target state where the unmodified 4L runtime permits and
recording honest pass/fail results; (3) keep the unfiltered regression gate
and known baseline policy explicit; (4) retain the accept/reject policy for
dead or protocol-only rows and `GameMaster=100` versus `Rested=112`; and (5)
document operations/recovery evidence only after the earlier items move. The
live 4L listener remains at `127.0.0.1:7000` and must not be stopped or
reconfigured.

## Historical true-completion checkpoint — 2026-09-14 (continued)

This continuation is retained as historical evidence; the later Step 0
Rank 3 checkpoint records the authoritative repeatability disposition.
Rank 1 received Go commit `2b2f013`, which maintains and reuses the sorted
monster ObjectID order during production world ticks. Focused spatial tests
pass normally and under targeted `-race`. The isolated benchmark mean fell
from 140.432 ms to 98.769 ms (29.7%) across three samples. A fresh
full-population runtime loaded 463 maps, 6,341 spawns and approximately 75,743
monsters with unchanged `TimeOut=10000`/`MaxUser=50`; one 50/50 wave passed in
3.506 s and twelve cooldown-aware waves passed 600/600 with a 4.707 s maximum
slowest client. This advances, but does not close, the extended-duration and
production-capacity Rank 1 boundary. Continue to Rank 2 same-account paired
payload/combat proof; preserve the honest A=`Partial`, B=`No` verdict and do
not stop or reconfigure live 4L.

## Current Rank 2 paired-evidence checkpoint — 2026-09-14

The focused `cmd/pkg5-combat-driver` and `internal/probe` tests pass normally
and under targeted `-race`. The latest redacted same-account replay uses
`/tmp/4l-planb-credentials.txt`, with disposable Go at `127.0.0.7:17397` and
live 4L still at `127.0.0.1:7000`. Both sides complete the identical
eight-request lifecycle (Go 560 frames, 4L 558), but packet volumes differ:
Go `ObjectMonster=18`, `ObjectNPC=1`, `NPCResponse=2`, `ObjectSpell=23`,
`NPCUpdate=1`; 4L `ObjectMonster=9`, `ObjectNPC=10`, no `NPCResponse`,
`ObjectSpell=22`, `NPCUpdate=2`.

The paired bounded combat driver sends five attacks on each `Scarecrow` path.
Go records 3 target strikes/3 damage indicators/3 health updates in 698
frames; 4L records 0/5/0 in 630 frames. This is lifecycle and combat-path
reach evidence, while synchronized combat and full-payload parity remain
**Fail/open** because target IDs, coordinates and runtime states are not
synchronized. Continue the ranked true-completion order with this honest
Rank 2 boundary; A=`Partial`, B=`No` remain unchanged. Keep the live 4L
listener untouched and exclude generated `Envir/`/`Goods/` data from commits.

## Package 5 persistence follow-up — 2026-09-13

The post-closure recovery audit has now covered JSON account decoding, 117
account/world/UsedGoods staging, JSON and export fsync, respawn runtime
promotion, periodic account backup, world backup replacement, invalid JSON
roots/store versions/runtime envelopes, and guild/conquest/UsedGoods retry
acknowledgement. Commits `1997753`, `f0df9e5`, `9f543c1`, `7712d3d`, `b7e8a8d`,
`b08e124`, `fb3fb3c`, `0500687`, `e5557a1`, `ffeaccf`, `54abc9b`, `0742cbd`
and `12dbdb2` are recorded in Go status. Focused tests and the latest
unfiltered gate pass; the known inspection packet-26 ordering baseline is
retained where reproduced. The next source-backed boundary is transactional
guild refresh deletion/write failure handling. Rollback beyond these file
paths, corruption recovery policy, monitoring/runbooks, same-state combat
parity, extended soak and production cutover remain open.

Package 5 has corrected map decoding and experience paths, full-population
runtime scans and bootstrap delivery, 279 packet ordinals, player/Hero starter
items, monster experience/rarity, globally allocated NPC purchases, atomic guild
costs, and current guild/economy/Hero checkpoint reload. The graphical client
enters both servers and completes the initial quest. Paired ordinary gameplay
matches level 22, XP 11, gold 930, two potions and completed[1]/active[2] quests.
A fresh Go guild retry survives clean restart/relogin with 50 guild gold; the
earlier failure and successful retry remain separately recorded.

Completed operator/NPC, archive, skill metadata and session-authority commit
history is recorded in Go `docs/MIGRATION-STATUS.md`. Use the current execution
items below; completed historical commits are evidence, not a new work queue.

Historical execution order (completed; retained as evidence):
1. KILL/DIE now use direct death lifecycle with revival/drop authority,
   GMNeverDie distinctions and Legacy broadcast counts. Go status records the
   tests and remaining shared death gaps (PK/luck penalties and default-NPC Die).
   CHANGEGENDER/CHANGECLASS now preserve targeting, identity persistence and
   normal logout admission; Go status records offline exception handling and tests.
   Continue tracing remaining reachable operator commands before declaring parity:
   Online archive session and guild authority are implemented across storage,
   currency/logout, rank definitions/readback, creation/invitation, leave/kick,
   promotion, buffs/XP/member views, war, territory/recall and conquest NPC
   transactions. Palace/member views project live membership; inspection uses
   Legacy registry resolution (removed target is silent, restore supplies row).
   Focused archive/checkpoint, race and filtered broad tests pass; detailed
   commits and validation remain in Go status.
   Wedding rings, marriage/divorce and mentorship acceptance now retain connected
   actor/registry partner/recipient authority with focused checkpoint tests.
   Logout now preserves live actor/recipient state and registry partner XP.
   Acceptance preserves existing XP; settlement auth arithmetic now matches Legacy.
   Settlement actor/recipient integration passes focused/race and filtered full tests;
   auth rental protection/acks/mail transfers now detach with the live character
   (auth, race and focused server tests pass). World rental returns now retain
   session recipients and per-player protection; rental/item-expiry race passes.
   Missing-owner-record returns now match Legacy with auth/race coverage.
   Periodic equipment scanning now matches Legacy with auth/race tests.
   Ordinary NPC ambient speech/reload now passes focused/race tests.
   CALL/automatic exclusions and shared filenames now pass focused/race tests,
   preserving normal/event pages through reload; filtered broad tests pass.
   Cell cleanup/re-entry tests pass; initial allocation already stamps order.
   Facing INFO now follows first-occupant ordering across live object families;
   delayed spell insertion and exact formatted readback have focused/race coverage.
   CLEARQUESTS/SETQUEST now preserve live owner/receiver identity with focused
   archive, timer and session checkpoint tests; race passes. Broad validation
   hits only the already-reproduced inspection packet-26 flake (no new skip).
   TOGGLETRANSFORM and player appearance/FastRun lifecycle now have focused
   runtime/session/wire/race tests; broad has only the known inspection flake.
   GATES, STARTCONQUEST, RESETCONQUEST and flag commands now have focused
   authority/transcript/session, race and filtered broad passes. Reset preserves
   archer Alive/flag quirks; flag RNG/parse order is covered. REVIVE now passes
   focused/session/race and filtered broad checks, including archive isolation
   and ordered object replay. Free STARTWAR now passes focused/session/race and
   filtered broad checks. FIND/MOVE text, missing-map and group-gate fixes pass
   focused/session/race and filtered broad tests. LEAVEGUILD deferred buffs and
   localized/ordered replies pass focused/session/race/broad checks. MAPMOVE/RECALL
   pre-gates, skill announcements and TRIGGER world order pass focused/race/broad.
   Shared global replies and SETLIGHT/STARTCONQUEST map order pass focused/race/broad.
   RELOADNPCS custom registration/dispatch passes focused/session/race/broad.
   Hero timing, same-cell melee and learned-ranged caster behaviour gates pass
   focused/race/broad checks. Archer swing range/front-cell targeting now passes
   focused/race. Player/Hero victims use owner combat authority; focused/race
   and filtered broad pass. MagicShield tick dispatch, delayed training/private buffs
   and cooldowns pass focused/race/broad checks. Summon/snapshot boundaries and
   archived-owner cast/training JSON checkpoints pass focused/race acceptance.
   MagicBooster priority/delayed visible buff now passes focused/race/broad.
   Wizard offensive selector passes focused/race tests; live dispatch remains open.
   MAC-only Human-victim impact authority passes focused/race tests.
   FlameDisruptor/ThunderBolt now dispatch delayed impacts through ticks;
   focused/refresh-audit/race/broad pass. Offensive archived-owner session/reload
   and snapshot acceptance pass focused/race. FireBall/GreatFireBall flight/distance
   delays pass focused/race/broad; projectile archived-owner checkpoints pass
   focused/race. FrostCrunch poison authority/order passes focused/race/broad;
   archived-owner and poison-caster lifecycle acceptance pass focused/race.
   Vampirism damage-return foundation passes focused tests for overkill,
   armour, Protection/revival and special monsters. Delayed Vampirism dispatch
   and deferred Hero healing pass focused/refresh-audit/race/filtered broad checks.
   Archive/reload and dismissal acceptance pass; pending healing is transient.
   TurnUndead chance/retarget and delayed kill authority pass focused/refresh
   checks, including Hero EXPOwner and special death overrides. Archive/reload,
   dismissal and statue-training acceptance pass focused/race/filtered broad.
   Repulsion shared live-cell traversal and Hero synchronous push/damage
   authority and immediate archive/checkpoint acceptance pass focused/refresh,
   race and filtered broad tests. Player Repulsion-family metadata damage
   correction passes focused/refresh/race/filtered broad. Next FlameField/
   ThunderStorm map-owned queue and eligible-attempt training, including
   archived-owner acceptance, pass focused/refresh/race/filtered broad. Next
   IceStorm/FireBang 3x3 actions, positive-return training and archive acceptance
   pass focused/refresh/race/filtered broad. Next shared player area dispatch
   and returned-damage training, session checkpoints and Attacked miss states pass
   focused/race/filtered broad checks (same six exclusions). FlameField rejection
   global-delay correction and randomized player area cast metadata pass focused/
   race/filtered broad checks. Taoist support and Purification operation wake
   passes focused/session/race. Taoist selector/dispatch/acceptance passes focused/refresh/race/
   filtered broad. Healing/MassHealing checkpoints pass focused/refresh/race/filtered broad.
   Player Healing metadata passes focused/refresh/race/filtered broad.
   Monster friendliness and delayed Healing pass focused/refresh/race; broad only known
   inspection flake. Session skill/logout checks pass with race. Player MassHealing
   metadata/live-cell completion pass focused/session/refresh/race/filtered broad.
   Armour/Hiding SC, support wake/live cells pass focused/session/race/broad; target lock focused/race.
   Hero armour/combined amulet authority passes focused/archive/race/broad; UltimateEnhancer passes focused/archive/race/broad; Taoist selector/Poisoning metadata pass focused/race/broad; Poisoning passes focused/archive/race/broad; Curse metadata/impact focused/session/race; Hero Curse focused/archive/race/broad; Revelation foundation focused/session/race; broad only known inspection flake; Hero Revelation focused/archive/race/broad; SoulFireBall metadata focused/race; Hero SoulFireBall focused/archive/race; broad known inspection flake; Archer selector focused/race; Concentration focused/archive/race/broad; SpecialArrow metadata focused/race/broad; live-level/cells and monster return/Human damage focused/session/race; cooldown/repeats focused/session/race; broad only known inspection flake; poison overrides/pet brown/target rules focused/session/race; broad known inspection flake; Hero PoisonShot admission focused/race/broad; Hero MentalState toggle focused/session/race/broad; Hero PoisonShot focused/archive/race/broad; ElementalShot metadata/live-level/cell/cooldown focused/session/race/broad; monster/Human MAC return and knockback revalidation focused/race/broad; Hero orb acquisition/broadcasts/gathering foundation focused/race; Hero ElementalShot admission/completion focused/race; monster/Human combat gathering focused/race/broad; Hero ElementalShot dispatch/archive focused/race/broad; player acquisition cast/cooldown and Hero wire level focused/race/broad; StraightShot/DoubleShot range metadata/cells focused/race/broad; MAC completion/cooldown and Hero recipient admission focused/race/broad; Hero StraightShot dispatch/archive/rejection/dismissal focused/race/broad; Assassin selector and Haste/LightBody delayed dispatch before CanAttack pass focused/archive/race/broad; HeavenlySword player DC/metadata passes focused; line completion MAC return/training/cooldown passes focused; Hero HeavenlySword dispatch/completion passes focused/archive/race/broad; DoubleSlash player metadata passes focused/session; Hero DoubleSlash admission/front-cell capture foundations pass focused (unwired); Hero monster defence modes pass focused/race/broad; Hero weapon wear helper passes focused/stale/archive/race/broad; impact wear/HP drain pass focused/race/broad; Human authority and wear/live bonus pass focused/race/broad (final clamp covered by race); DoubleSlash dispatch/post-hit training passes focused/archive/dismissal/fallback/race/broad; FatalSword single-capture randomized metadata and skill DC/arm order pass focused/race/broad; Hero FatalSword armed state/DoubleSlash live defence passes focused/Human/archive/race; broad only known inspection flake; ordinary Hero DC capture and AC/Agility foundation pass expanded Hero/race/broad; Human AC mode passes focused; +300ms melee FatalSword queue foundation passes focused; ordinary +300ms dispatch passes Hero tests; Hero MPEater admission passes focused; Hemorrhage admission passes focused; combined passive archive passes; Human AC/+300ms/FatalSword/MPEater/Hemorrhage pass expanded race/broad; Hero Holy-before-passives passes focused/archive/race/broad; Hero red-poison armour roll order passes focused/race/broad; received-rate numeric helper passes focused/60k .NET oracle; Hero Stun/red/normal received-rate integration passes focused/race/broad; Human rate conversion matches 10.1m .NET cases; Human impact rates pass focused; Human rates/physical-miss roll pass expanded/race/broad; critical Settings foundation passes config tests; critical runtime plumbing passes production-path test; critical arithmetic/roll foundation passes .NET 8 boundary tests; Hero monster critical passes expanded/race; broad only known inspection flake; Human critical passes expanded/race/broad; Hero EnergyShield impact passes focused; Hero shields pass expanded/race/broad; HeroInfo override confirms no saved shield buffs; Reflect gate foundation passes focused; Human Reflect integration passes expanded; Human/chained Reflect passes focused/race/broad; negative-effect Settings pass config tests; negative-effect runtime/CatTongue flag pass focused; shared debuff RNG/admission helper passes focused (unwired); runtime/CatTongue pass race/broad; caster/owner and zero tick corrected with focused race; Hero monster effects pass expanded; live-wear/initial-tick/race/broad pass; Human resist Settings pass config tests; runtime resistance gate passes focused; Human negative effects pass expanded; drain/notification-order acceptance passes; race passes; broad only known inspection flake; monster LRParalysis/wake/ShockTime passes focused; Human removal/wake passes focused; hit-state race passes; broad only known inspection flake; shared receiver eligibility/owner delegation passes focused; Hero-master Peace and Hero retarget stages pass expanded; retarget race/broad pass; Player LastHitter/+10s ordering passes focused; Player impact regen timestamp passes focused; hit-state race passes; broad only known inspection flake; Hero drop killer gates unconditional; Hero impact regen passes expanded; actual deadline recovery passes focused; Hero regen race/broad pass; Hero brown-name assignment passes focused; RedBrown/expanded acceptance passes; Hero brown race/broad pass; Player exact AtWar brown admission passes focused; RedBrown/computed-colour expanded acceptance passes; Player brown race/broad pass; viewer guild/conquest colour rules pass expanded; cached colour processing/explicit synchronization pass expanded; colour race passes; introduced transcript/clock failures repaired with 3 focused runs; remaining spell/PK-town/TownArcher colour expectations pass focused; final colour race/broad pass; two-session brown expiry/viewer/repeat acceptance passes three runs/race; Hero Human LastHitter admission/strict expiry passes focused/expanded race; dismissal acceptance passes three runs; broad only palace timing (c63e35f 1/100 premature colour, current 1/100 save-order; exact broad white-tail not isolated); Player-to-Hero MAC/shared hit state/live bonus/gathering passes focused/session/expanded race/filtered broad; Player-victim SoulFireBall/BladeAvalanche shared MAC passes focused/session/race; generic Player projectile shared MAC/FrostCrunch session acceptance passes focused/race; captured-map HellFire/IceThrust shared MAC passes focused/session/race; introduced colour expectations repaired; final filtered broad passes; Archer area Player shared MAC/effects/training passes focused/session/race; Archer captured-map completion passes focused/session/race; Archer monster returned-damage/Trainer admission passes focused/race; Archer area filtered broad passes; DelayedExplosion captured-map burst and all fuse producers pass focused/race (3x3/no rearm or training); world-tick movement passes focused/race; broad only known inspection flake; initial Player shared MAC passes focused/race/broad; initial Hero MAC passes focused/race; explosion operation wake passes focused; initial monster return and Hero/wake changes pass focused/race/broad; Player-owned Human fuse attribution passes focused/race; Hero processing lifetime and Human attribution pass focused/race/broad; monster-carrier fuse state passes focused/race/broad; monster-owned monster-carrier fuse MAC/attribution passes focused/race/broad; pet Trainer passes race; Player monster-owned fuse passes focused; Hero dispatch and Player changes pass focused/race/broad; raw Human monster attribution and replacement/DIE pass focused/race/broad; monster-to-Player regen/logout ordering passes focused; admitted Hero regen and Player ordering pass focused/race/broad; Player receiver master attribution/death-item credit pass focused/race/broad; Hero-master identity/lifetime/death-item routing pass focused; counterattack selection/Hero AC completion pass focused/race/broad; receiver-to-Hero counterattack acceptance passes focused; admitted Hero-victim master attribution/lifetime pass focused/expanded; stale fuse deadline assertions corrected; pet-owned Hero fuse acceptance passes focused/expanded race/final broad; monster-to-Human received rates/reduction pass focused; LionRoar removal/wake pass focused/expanded race/broad; monster-hit EnergyShield passes focused; monster-hit shield duration/phase/order pass focused/expanded race/broad; Player MoonLight/DarkBody deferred removal preserving Hiding passes focused; Hero deferred stealth and five visibility fixtures pass focused/race/final broad; reflection MAC-only reverse receiver foundation passes focused; Player monster-hit reflection passes focused/expanded race; RNG fixtures pass targeted race/final broad; Hero reflection/plain AC reverse defence pass focused; cross-map fuse reflection passes focused/race; broad only known inspection flake; Trainer Player reverse defence passes focused/race; StoneGolem Hero shared AC passes focused/race (regen fixture corrected), filtered broad passes; field Struck rates pass focused/race; field Struck stealth passes focused/race; field shields pass focused/race; field regen/logout/channel state passes focused/race/broad; field durability passes focused/race/broad; GeneralMeowMeow MAC Struck passes focused/race; DarkOmaKing Player AC Struck/zero-value Dazed passes focused/session/race/broad; StoneGolem/FlyingStatue monster Struck rates pass focused/race/broad; GeneralMeowMeow monster Struck rates/luck/Armadillo passes focused; DarkOmaKing monster Struck rates/indicators/zero Dazed pass focused; Trainer field Struck immunity passes focused; combined race passes, broad only known timed-recall queue flake; TrapRock/StoneTrap field immunity passes focused; Football/GuardianRock field immunity passes focused; HellBomb/Tree field immunity passes focused; combined race/broad pass; EvilMir/EvilMirBody field immunity passes focused; Guard/TownArcher monster-target admission passes focused; combined race/broad pass; tornado null poison-owner/lifetime parity passes focused/session; nuke null poison-owner parity passes focused/session; combined race/broad pass; tornado Slow first-tick parity passes focused/session; TreeQueen monster root rates/immunity pass focused; ground-root poison parity passes focused/session; zero-value Player GroundRoots passes focused/race; broad known inspection flake and introduced static ledger expectation corrected/passing (ordinary monster hits only assign EXPOwner), PoisonCloud first Green tick passes focused/session; PoisonCloud raw MAC poison defence passes focused; Player resistance passes focused; PoisonCloud monster ApplyPoison overrides pass focused; consolidated race/broad pass; PoisonCloud Player brown attribution passes focused; monster application-time targeting passes focused; remaining PoisonCloud no-op class overrides pass focused/race; broad only known timed-recall flake; PoisonCloud master brown attribution passes focused; FireWall/ExplosiveTrap specialized MAC dispatch passes focused; consolidated race/broad pass; ordinary monster field MAC/Luck/received rates/bonus pass focused; field target acquisition before MAC passes focused/race/broad; ordinary field critical processing passes focused; post-critical retargeting passes focused/race; broad only reproduced Guard packet flake; field BindingShot/ShockTime passes focused; LRParalysis hit state passes focused/race/broad; field master brown attribution passes focused; EXPOwner/MAC negative-effect acceptance passes focused/race/broad; mentor damage branch audited inactive (monster GroupMembers never assigned); wild-victim pet assistance passes focused/race/broad; pet-versus-pet assistance passes focused; specialized assistance/direct master acceptance pass focused/race; broad only known inspection flake; DelayedExplosion ordinary MAC/critical/hit-state/assistance passes focused; fuse/burst acceptance passes focused/race/broad; field resistance/plain-MAC corrected against C# and passes focused/race/broad; monster-owned fuse MAC roll/rates pass focused; fuse cleanup/live poison iteration pass focused/race/broad; reverse-target predicate/fuse dispatch and direct-master acceptance pass focused/race; broad only known inspection flake; GuardianRock/ThunderElement fuse immunity passes focused; specialized initial ApplyPoison gates pass focused; specialized gates/base target/brown attribution and duplicate ordering pass focused/race/broad; Human initial fuse poison resistance passes focused; Human brown attribution/caller wake/training pass focused/race (MAC-miss fixture corrected); broad only known Guard packet flake; Wizard area ordinary monster receiver parity passes focused; map-completion scaling/training/assistance acceptance passes focused/race; old caller-armour fixture corrected/passing with live MAC; final race/filtered broad pass; FireBall/GreatFireBall/ThunderBolt ordinary monster projectile receivers and specialized/revalidation acceptance pass focused/session/race; old AC-based HP fixtures corrected; final filtered broad passes. ThunderBolt fixed500ms Player/monster cast timing passes focused/race/filtered broad. Basic Wizard randomized cast metadata/live-impact acceptance passes focused/session/race (ThunderBolt RNG fixture pinned); custom catalog multiplier expectation corrected; final race/filtered broad pass. FlameDisruptor cast metadata/fixed500ms/monster MAC and deadline/Player boost acceptance pass focused/race (world RNG/timing/MAC fixture corrected); filtered broad passes. SoulFireBall ordinary monster MAC receiver and SC/amulet/specialized acceptance pass focused/Human/Hero/race/filtered broad. FrostCrunch monster MAC/skill metadata/positive-return poison ordering and poison overrides/attribution/wake pass focused/Human/Hero/race (MAC/RNG fixtures corrected); filtered broad passes. FrostCrunch randomized cast metadata/live skill level passes focused/race (fixed-MC RNG fixture corrected); filtered broad passes. FrostCrunch Player poison resistance/attribution/wake and impact/brown-exception acceptance pass focused/race/filtered broad. Hero FrostCrunch configured resistance/post-ApplyPoison wake passes focused/race/filtered broad. FireBounce randomized initial/per-bounce damage and training/deadline acceptance pass focused/race; broad only known inspection packet26 flake. FireBounce monster MAC/caller-owned continuation passes focused (MAC fixtures corrected); moved-source acceptance/race/filtered broad pass; Blizzard/MeteorStrike randomized cast capture passes focused/session/race/filtered broad; shared monster MAC passes focused/session/race/filtered broad; captured-map Human MAC passes focused/session/race (brown colour transcripts corrected); broad only known inspection flake; Player Slow resistance/brown attribution passes focused/session/race/filtered broad; monster Slow attribution/overrides and zero/unit-Freezing draws pass focused/session/race; broad only known inspection flake. MeteorShower randomized Player/monster MC and half-damage capture pass focused; shared monster MAC passes focused/race/filtered broad (AC fixtures corrected); secondary ring/cell order, Player/Hero inclusion and limit-before-filter/queue order pass focused/world-tick/race/filtered broad; initial Hero admission and mixed-Human completion/checkpoints pass focused/Archer/race/filtered broad; MeteorShower early-return CastTime/rejected-packet/captured-coordinate parity passes focused/race; broad only known Guard packet82 flake. FireBounce mixed Player/Hero selection/ring order and shared Human completion/checkpoints pass focused/race/filtered broad; terminal/unit selection RNG passes focused; initial Hero admission/Human bounce-count metadata and consolidated nearby/RNG/Archer race/filtered broad pass; Hero-victim FireBall/GreatFireBall/ThunderBolt admission/live-MAC/checkpoints pass focused/race/filtered broad; FrostCrunch Hero admission/post-MAC poisons/live-level/resistance/wake pass focused/race/filtered broad; FireWall monster MAC/state passes focused/race; FireWall Human MAC/state and explicit Hero exclusion pass focused/race; randomized FireWall cast metadata passes focused/consolidated race/filtered broad; PoisonCloud Player captured-map MAC and surviving-victim poison attempts after misses pass expanded race (broad only known inspection flake); monster shared admission/Green roll order passes focused/race/filtered broad; full catalog power/capture-before-reagents and zero/unit PoisonAttack RNG pass focused/consolidated race/filtered broad; ExplosiveTrap shared MAC passes focused/race; eligible-target detonation/blocked-hit/linked-despawn order passes focused; captured-map Player admission passes focused; randomized MC/catalog capture passes focused/consolidated race/filtered broad (all20 tested packages, no JSON failures); Lightning randomized MC/catalog capture and moved-caster Human impact pass focused/session; ordinary monster MAC rates/critical and one target/cell/positive-return training pass expanded race/filtered broad; HellFire single MC/catalog capture across rays/continuations passes focused/session; monster MAC/per-cell positive-return training and queue ordering pass expanded race/filtered broad; IceThrust catalog capture and near/far Human impact pass focused/expanded race/filtered broad (all20 tested packages, no JSON failures); monster MAC/positive-return training and post-return poison ordering pass focused/expanded race; live-level/resistance/attribution/wake passes focused; shared Frozen-duration zero/unit RNG passes focused; Hero caster/victim duration RNG passes consolidated race/filtered broad; DelayedExplosion randomized MC/catalog capture for Player/monster/Hero targets passes expanded race (round-even fixtures corrected); live fuse level after first training passes focused/expanded race/filtered broad (all20 tested packages, no JSON failures); OneWithNature randomized MC/catalog capture passes focused/session; ordinary monster MAC rates/critical passes focused/session; live effect level passes focused/session; poison RNG/resistance/attribution/overrides pass focused/session; initial Green tick timing passes focused/session; consolidated race passes; broad only known inspection packet26 flake; NapalmShot randomized ranged MC/catalog capture passes focused/session; monster MAC/training passes focused/session; expanded race/filtered broad pass (all20 tested packages, no JSON failures); mixed Human center admission/Hero impact exclusion passes race; StormEscape randomized MC/catalog capture and separate750ms teleport pass focused; shared map MAC/attempt-training passes focused; captured-map Human/blocked/removed/Hero-exclusion acceptance passes expanded race; broad known inspection packet26 plus new level61 fixture corrected/passing race; Player Vampirism randomized MC/catalog capture/fixed500ms for Player/monster/Hero passes focused; shared MAC/moved-target admission/post-training live-level healing passes focused (Trainer return1 and old MAC fixture corrected); expanded race/filtered broad pass (all20 tested packages, no JSON failures); CatTongue full DC/catalog capture passes focused/session; PoisonSword full catalog/reagent order passes focused/session; PoisonSword poison admission/first-tick/wake ordering passes focused/session; unused catalog helper removed; expanded race/filtered broad pass (all20 tested packages, no JSON failures); CatTongue shared Human AC passes focused/session; monster AC/positive-return effects pass focused/session; expanded race passes; old Hero regen assertions corrected and revival race passes; final filtered broad passes (all20 tested packages, no JSON failures); CatTongue live poison level/null owner passes focused/session; CatTongue resistance/overrides/lethal admission passes focused/session; CatTongue initial tick/caster removal passes focused/session (ObjectPoisoned transcript corrected); expanded race/filtered broad pass (all20 tested packages, no JSON failures); all60 proc branches pass focused/session; Node is runtime lifecycle (MapObject.cs344/375); lifecycle/map/moved-target acceptance passes race; BladeAvalanche full DC/catalog capture after skill critical passes focused; expanded race/filtered broad pass (all20 tested packages, no JSON failures); monster MAC/Repulsion rates/critical/hit-state and positive-return training pass focused/session/race; broad only known inspection packet26 flake; MoonMist monster AC/returned training and Player captured-map AC pass focused/session/race/filtered broad (colour transcript corrected; all20 packages, no JSON failures); MoonMist live-level/null-owner/initial-tick Stun, poison overrides and full randomized catalog damage pass focused/session/race/filtered broad (all20 packages, no JSON failures); Plague monster MAC/weapon wear/live bonus/attempt training passes focused/session/race/filtered broad (all20 packages, no JSON failures); Plague Player MAC/weapon wear/poison→MP→damage/captured map and live poison level/value/MP pass focused/session/race/filtered broad (colour transcript corrected; all20 packages, no JSON failures); Plague Player poison resistance/attribution and monster overrides/attribution pass focused/session/race/filtered broad (pet aggression fixture corrected; all20 packages, no JSON failures); Plague initial Player/monster poison tick passes focused/session/race/filtered broad (poison transcript corrected; all20 packages, no JSON failures); Plague pre-reagent randomized SC/catalog and deletion order pass focused/session/race/filtered broad (all20 packages, no JSON failures); HealingCircle randomized SC/catalog and ordinary monster Struck MAC/rates pass focused/session/race; broad only known Guard packet82 flake; HealingCircle specialized Struck return0 overrides and Trainer Slow continuation pass focused/session/race/filtered broad (all20 packages, no JSON failures); HealingCircle Player Struck MAC/luck/rates/shields/wear/regen/unthrottled packets pass focused/session; Player operation wake/action preservation passes focused/session; consolidated race/filtered broad pass (all20 packages, no JSON failures); Player Slow resistance/attribution passes focused/session (colour expectations corrected); monster Slow attribution/immune wake passes focused/session; initial Slow tick passes focused/session (poison transcript corrected); Slow consolidated race/filtered broad passes (all20 packages, no JSON failures); live-level expiry/pre-training capture passes focused/session; caster lifecycle boundaries pass focused/session; Reincarnation ready-timeout ordering passes focused/session; consolidated race/filtered broad passes (all20 packages, no JSON failures); Reincarnation separate action/object expiry and interruption lifecycle pass focused/session; separate deadline session acceptance passes; consolidated race/filtered broad passes (all20 packages, no JSON failures); Reincarnation already-alive acceptance host cleanup passes focused/session; stale-accept session clock/barrier regressions fixed; expanded focused/session/race count3 passes; final filtered broad passes (all20 packages, no JSON failures); Reincarnation fishing refresh passes focused/session; group-map/location session payloads pass focused; consolidated race count3/filtered broad passes (all20 packages, no JSON failures); Reincarnation base health/broadcast/re-entry ordering passes focused/session; full shared GetObjects/cache delivery and mixed/grouped acceptance pass focused/session; expanded race count3 passes; filtered broad only known inspection packet26 flake; Portal exhausted zero-expiry/same-timestamp paired removal passes focused; natural expiry/caster lifecycle boundaries pass focused; lifecycle race/filtered broad pass (all20 packages, no JSON failures); Portal live full-cell traversal passes focused; live-cell race count3 passes; same-cell re-entry passes focused; final re-entry race count3/filtered broad pass (all20 packages, no JSON failures); persistent-field placement/player-entry order passes focused unchanged (nextIDLocked records order); delayed spawn membership/order passes focused unchanged; consolidated field/DarkOmaKing/Portal race count3 passes (test-only slice); Blizzard/MeteorStrike Reflect channel interruption per receiver passes expanded focused/session and initial race count3; expanded race count3/filtered broad pass (all20 packages, no JSON failures); DarkOmaKing nuke dead-caster versus removal passes focused; pet/Hero acceptance passes focused; initial race count3 passes; expanded race count3/filtered broad pass (all20 packages, no JSON failures); nuke captured-map/direct receiver admission passes expanded focused (Player/pet/GM/Hero/wild/decoy); owned-caster admission passes focused; expanded race count3 passes; owner race passes; broad exposed football expectation corrected per C# and full field-focused checks pass; final race count3/filtered broad pass (all20 packages, no JSON failures); GeneralMeowMeow Thunder live mixed-cell order passes focused; re-entry acceptance passes focused; expanded race count3 passes; final re-entry race count3/filtered broad pass (all20 packages, no JSON failures); Thunder captured-map/direct Player and monster admission passes expanded focused; dead/removed acceptance passes focused; expanded race count3 passes; final lifecycle race count3/filtered broad pass (all20 packages, no JSON failures); FlyingStatue tornado captured-cell/direct admission passes expanded focused (orphan-owner fixture corrected); dead/removed lifecycle passes focused; final expanded race count3/filtered broad pass (all20 packages, no JSON failures); tornado Slow chance-before-override correction passes expanded focused; Player/pet lethal follow-up passes focused; expanded race count3 passes; final lethal race count3/filtered broad pass (all20 packages, no JSON failures); Dust Tornado captured-cell/direct admission and unowned AC Struck pass expanded focused (fixture IDs/transcript corrected); boundary acceptance/expanded race count3/filtered broad pass (all20 packages, no JSON failures); RockFall/RockSpike captured-cell/direct admission/shared AC Struck and inert overrides pass expanded focused/race count3; introduced static refresh ledger corrected; final field/ledger race count3/filtered broad pass (all20 packages, no JSON failures); StoneGolem Quake captured-cell/direct admission passes expanded focused/race count3; EarthGolem Pile/shared AC cell handler and mixed receiver armour/re-entry order pass expanded focused (RNG fixture corrected); final expanded race count3/filtered broad pass (all20 packages, no JSON failures); Tucson rock captured-cell/direct admission/shared AC Struck, mixed order, inert overrides and unthrottled packets pass expanded focused/session (fixture allocator/RNG/transcript corrected); Player rate/stealth/regen acceptance and expanded race count3 pass; broad only historical palace white ColourChanged before GuildStatus (untouched7487ad9 isolation100/100 pass); TreeQueen Root/MassRoots/GroundRoots captured-cell/direct MAC admission passes expanded focused/session; GroundRoots poison overrides/stoned boundary/zero and lethal continuation pass expanded focused/session; final expanded race count3/filtered broad pass (all20 packages, no JSON failures); map-hazard MAC rates/stealth/impact regen pass expanded focused (revival/group-health retained); MapQuake Player nonzero/alive/location admission/shared MAC Struck and preserved hit state pass expanded focused; final expanded race count3/filtered broad pass (all20 packages, no JSON failures); monster Quake shared MAC/resistance removal/rates/inert overrides pass expanded focused; mixed live-cell/re-entry/callback insertion and Armadillo acceptance pass expanded focused; final expanded race count3/filtered broad pass (all20 packages, no JSON failures); safe-zone Healing direct Player-master admission passes expanded focused/session (CharmedSnake parent cases reproduced); safe-zone Healing field-origin broadcasts pass expanded focused/session (prior expectation corrected); final expanded race count3/filtered broad pass (all20 packages, no JSON failures); HealingCircle direct-master grouping passes expanded focused/race count3/filtered broad (all20 packages, no JSON failures; live/removed CharmedSnake regressions); direct-entry audit confirms missing Player walk/run/push and monster callbacks; Player HealingCircle walk/run entry and traversed-field effect origin implemented, expanded focused movement/session ordering checks pass; Player final-cell push entry passes expanded focused (full/partial regressions reproduced); consolidated race count3/filtered broad pass (all20 packages, no JSON failures); monster receiver/ordered direct field dispatcher passes expanded focused (lethal overlap/authoritative position); monster turn/walk/push/routes/pet entry wiring passes expanded focused and production tick writeback; unused safe-only helper removed and direct tests exercise production dispatcher; expanded focused/race count3 pass; filtered broad hits documented YinDevilNode missing83 baseline (untouched498caac 5/5000); SepWarrior field callbacks pass expanded focused (final-cell effects and packet order); production tick lethal/nonlethal writeback and safe-zone entry pass expanded focused/race count3/filtered broad (all20 packages, no JSON failures); SepWarrior inherited one-cell chase/roam dispatch passes expanded focused (edge/rotated regressions reproduced; field geometry corrected); inherited/custom Hiding and deadlines pass expanded focused/race count3/filtered broad (all20 packages, no JSON failures); SepWizard dispatch already matches; shared base-Walk Hiding marking/reveal passes expanded focused (corrected immediate-deletion interpretation per MapObject.cs672); field-order/one-shot reveal/concealment pass expanded focused/race count3/filtered broad (all20 packages, no JSON failures); routed Hiding marking/reveal and field order pass expanded focused with blocked/waiting acceptance; route destination insertion passes expanded focused/targeted race count3/filtered broad (all20 packages); expanded race only known Guard early82 baseline; route shared movement admission passes expanded focused (stationary/deadline regressions; Guard override preserved); route deadline writeback/strict boundaries pass expanded focused; safe-zone pet empty transcript confirmed baseline c7170b0 3/1000; filtered broad all20 pass; targeted race count3 passes, expanded race only known TaoGuard missing82 baseline; WoodBox/FloatingRock/YinDevilNode movement overrides pass expanded focused; empty-roam exemption/preserved support AI pass expanded focused/race count3; filtered broad only known Guard missing82 baseline; IcePillar/GuardianRock overrides pass expanded focused; SnakeTotem distinct Walk/ProcessAI dispatch passes expanded focused; TrapRock CanMove/Walk/empty-roam overrides pass expanded focused/targeted race; StoneTrap Walk without CanMove and custom ProcessAI pass expanded focused/targeted race; TreeQueen/CaveStatue/HellLord CanMove/Walk overrides pass expanded focused/targeted race; HellBomb/CannibalPlant/CreeperPlant CanMove/Walk/empty-roam overrides pass expanded focused/targeted race; PowerBead CanMove/imported-route skip pass focused/targeted race; consolidated movement-override filtered broad all20 pass (no JSON failures); TucsonEgg/BoulderSpirit CanMove/Walk and imported-route skip pass focused/targeted race; PurpleFaeFlower/BugBagMaggot/RootSpider/GreatFoxSpirit imported-route skip pass focused/targeted race; Jar1/Jar2/RestlessJar imported-route skip pass focused/targeted race; Trap/TrapHexagon exact-deadline expiry pass focused/targeted race; HellKeeper/EvilCentipede/EvilMir imported-route skip pass focused/targeted race; FireWall live-cell receiver traversal pass focused; PoisonCloud live-cell receiver traversal pass focused; Blizzard/MeteorStrike live-cell receiver traversal pass focused; HealingCircle live-cell receiver traversal pass focused; ExplosiveTrap live-cell first-eligible receiver pass focused; persistent-field live-cell slice race count3/filtered broad pass (known inspection packet-26 flake only); CastleGate still absent from Go; DarkOmaKing lethal nuke unowned Dazed after Struck pass focused; DarkOmaKing Dazed class ApplyPoison no-ops pass focused; DarkOmaKing nuke live-cell receiver traversal pass focused; MapLava/MapLightning live-cell receiver traversal pass focused; safe-zone Healing live-cell receiver traversal pass focused; HealingCircle lethal monster Slow after Struck covered focused; SpellObject live-cell batch targeted race count3/filtered broad pass (known Hiding and LoverRecall flakes only); CastleGate BlockingObject bodies now spawn at Gate.BlockArray offsets with Image 901 / AI 0 and no DragonLink (focused spawn/hide/idle pass); CastleGate Hide/Show packets now follow ObjectAttack on open/close/die (focused door-packet pass); CastleGate BlockingObject batch (spawn Image 901 bodies, Hide/Show packets, parent Attacked redirect) targeted race count3/filtered broad pass (known Hiding and PoisonCloud flakes only; constructor ledger updated); Walk/run/push ProcessSpell now covers FireWall, PoisonCloud, Blizzard/MeteorStrike and ExplosiveTrap without advancing field ticks (focused walk/run/push pass); Portal walk ProcessSpell now teleports caster/group members through the paired exit and consumes one pass (focused walk pass); MapLava/MapLightning walk ProcessSpell now Strikes the walker without clearing Value or advancing the field tick (focused walk pass); Walk ProcessSpell batch (FireWall, PoisonCloud, Blizzard/MeteorStrike, ExplosiveTrap, Portal, MapLava/MapLightning) targeted race count3/filtered broad pass (known Hiding, PoisonCloud and LoverRecall flakes only); Walk ProcessSpell now covers DarkOmaKing nukes, TreeQueen roots and FlyingStatue tornados without advancing field ticks (focused walk/run/push pass); remaining AC/MAC monster-field walk ProcessSpell now covers StoneGolem/EarthGolem/Tucson/Horned/Meow/MapQuake without advancing field ticks (focused walk/run/push pass); monster-field walk ProcessSpell batch targeted race count3/filtered broad pass (known Hiding and PoisonCloud flakes only); player Turn ProcessSpell now covers the current cell after CanMove/CheckMovement without advancing field ticks (focused turn pass); CastleGate peace CheckDirection now ObjectTurns closed gates with 0 damage (focused peace/owner/open/war offset pass); remaining generic/AxeSkeleton idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused roam/AxeSkeleton/TucsonEgg pass); generic idle-roam fixtures now pin MonsterAIRoamAt (focused delayed-recheck/pack/teleport pass); HellSlasher/HellPirate/HellCannibal idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused hell-family pass); CrazyManworm/MutatedManworm idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused manworm pass); Turn-slice filtered broad all20 pass except known PoisonCloud and LoverRecall flakes; RightGuard/LeftGuard/MinotaurKing idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused guard/king pass); SandWorm/VenomSpider/BlackFoxman/HedgeKekTal idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused spearman-family pass); BoneSpearman/BoneLord idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused bone-family pass); specialized idle Turn batch (Right/LeftGuard/MinotaurKing, SandWorm/VenomSpider/BlackFoxman/HedgeKekTal, BoneSpearman/BoneLord) filtered broad all20 pass except known PoisonCloud and LoverRecall flakes; DarkDevil/IncarnatedGhoul/ShamanZombie/Khazard/ToxicGhoul idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused ghoul/devil pass); WingedTigerLord/TrollBomber/TrollKing idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused WTL/troll pass); ghoul/devil and WingedTigerLord/troll idle Turn batch filtered broad all20 pass except known Hiding and inspection packet-26 flakes; FlyingStatue/StoningStatue/KingScorpion/AssassinBird/Mantis/AxePlant idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused statue/plant pass); AvengingSpirit/AvengingWarrior/Nadz idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused avenging/Nadz pass); statue/plant/avenging/Nadz idle Turn batch filtered broad all20 start with crystal-server skip-PoisonCloud 132.718s except known Hiding and mount stale flakes (PoisonCloud hung in unskipped ./...); OmaBlest/OmaCannibal/OmaKing/OmaMage/OmaSlasher/OmaWitchDoctor idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused oma-family pass); FrozenAxeman/FrozenKnight/FrozenMagician/FrozenMiner idle Turn ProcessSpell now covers FireWall without advancing field ticks (focused frozen-family pass); next remaining specialized-AI idle Turn/ProcessRoam; retain explicit FlameDisruptor Hero exclusion. TRIGGER preserves callback/NPCUpdate order. RELOADNPCS drains goods/reloads scripts;
   ordinary quest roots/speech and CALL side effects are tested. CLEARIPBLOCKS
   preserves live admission counts and creation-abuse history. Archive registry,
   connected item/storage/reward and guild-storage authority work; retain global-index gaps.
   Idle Turn/ProcessRoam audit now covers Furbolg, Horned/Snow, BlackTortoise,
   DragonWarrior, Kirin, ScalyBeast, WereTiger, PlagueCrab and TreeGuardian with
   focused/session/race evidence; IcePhantom, RhinoPriest, HoodedSummoner,
   HoodedSummonerScrolls and KingHydrax now have the same coverage. The
   DarkOmaKing, GasToad, LightTurtle, ManectricClaw and PeacockSpider are now
   covered as well. The consolidated batch gates found only repaired Furbolg
   fixtures and documented Guard/LoverRecall/timed-recall baseline flakes; the
   latest filtered gate passed all packages with the six documented skips.
   EarthGolem, ManTree, StoneGolem, ThunderElement and TucsonGeneral now also
   execute inherited idle Turn/ProcessRoam, with a shared receiver publication
   fix for immune direct fields; their filtered gate passed all packages.
   The inherited idle Turn/ProcessRoam audit is complete. Explicit
   no-op/custom roam exclusions are recorded in Go status. Settings consumers,
   NPC conquest tax, LineMessageTimer and OnlinePlayers workloop broadcasts
   are landed; move next to remaining packet/spell-AI/item-ID behavior audit,
   then representative restart/economy integration.
   WarriorHero HalfMoon/CrossHalfMoon/TwinDrakeBlade leftovers and Hero
   high-bar MagicKey routing are now landed as Go commits `239a9ea`, `4327a29`
   and `597129c`, with focused/race evidence in Go status. The batch broad
   gate still has the documented Guard/TaoGuard ordering failures; the
   PlayerMeleePvP transcript also reproduces on untouched `424cda4`.
   Manual `ClientMagic` requests addressed to a summoned Hero now enter the
   existing Hero tick/cast authority as `cf85e04`; connected-client transcript
   acceptance is now covered by `ccc8515` with focused/race evidence in Go
   status.
   The shared PlayerObject.Die PK/luck penalty path is restored as `a14250c`;
   default-NPC Die callback parity is now page-gated and uses the Legacy
   `[@_Die]` key in `e143c1d`; absent pages remain silent without changing
   normal default-NPC activation behavior.
   Focused representative quest/shop/guild/economy/combat restart checks pass;
   the corrected filtered broad gate retains Guard/TaoGuard plus the already
   documented intermittent PlayerMeleePvP baseline.
2. Audit remaining packet, spell/AI and item-ID allocation paths.
   Fix confirmed gaps in focused commits with tests; update this plan with evidence.
3. Complete remaining integration acceptance and report its actual limits.
Regression policy: keep the six reproduced baseline timing failures skipped (PoisonCloud, map-hazard restart, mount stale
recovery, Hiding, NPC delayed GOTO, cross-map LoverRecall). Their unchanged-baseline
reproductions and earlier repaired fixtures are recorded in Go status. This is
not an unfiltered-green suite. OmaMage extra-roll transcript flakiness also
reproduces on untouched ccdb06b; it has not been added to the six exclusions.
Inspection packet-26 reproduces 21/100 on 9821e04; timed-recall queue flake
reproduces 10/5000 on 6a61c66; skill timing on 20325e4; YinDevilNode 5/5000 on 498caac; TaoGuard colour 5/50 on 3bc5121 (two missing, three early). Guard AttackMode packet order reproduces 8/100 on b75248a (six missing, two early). No new skips.
New regressions remain subject to the documented baseline policy. Packages 1–5
are accepted; overall production cutover remains unclaimed because the active
completion audit still has ranked proof gaps in same-state combat/payload
equivalence, MaxUser/long-duration load, and crash/rollback/operations
readiness. These are completion/cutover acceptance boundaries, not missing
historical Package 5 feature leaves.

The latest committed Settings/packet/workloop batch is `908a3ad`, `001cff4`,
`c70cf8d`, `7ce3fac`, `3714d8a`, `1f699df`, `bacd310`, `f58e8e8`, `6903286`,
`488f02d`, `245cad9`, `3f8eaa6`, and `42aeb51`; focused and targeted race
checks pass. Leftover recall/rested slices landed as `c18935e`, `9ed4553`,
`4f9928e`, and `164b112`: monster recall stays in the revelation refresh
ledger, AutoRev wild health notifies the master group then a distinct
experience-owner group (not nearby-all), login Rested AddBuff is drained
during StartGame bootstrap, and recall fixtures attribute AutoRev health to
the experience owner. Item-ID CreateFreshItem gaps landed as `06e1c6c`,
`386a1d6`, `57af2ff`, `5828edf`, `6ea93f5`, and `8993825` (partial stack
drop including DestroyOnDrop, NPC GiveItem, craft output including failed
rolls, quest carry including failed CanGainQuestItem, UniqueID-0 pickup,
and awakening disassemble). Leftover generic magic and remaining shop
identity landed as `e09364a`, `ecebe8c`, and `d54f91d`: leftover targeted
magic uses luck-aware MC range plus UserMagic.GetDamage, NPC partial-stack
sell CreateFreshItem uses Envir.NextUserItemID including gold-overflow
consumption, and PoisonShot expected damage uses production range MC.
Leftover ordinary DC damage landed as `d0081db`, `169de5e`, `c59bbe4`,
and `b87b32b`: Spell.None melee uses luck-aware GetAttackPower, FlashDash
uses UserMagic.GetDamage of that DC range, RangeAttack uses
GetRangeAttackPower instead of MinDC-only scaling, and MaxDC<=0 keeps the
level/min-1 melee bridge. Test-only CreateFreshItem fallbacks landed as
`c9aefec`, `aa1b856`, `ec0ec61`, and `a950352` (shop buy after gold,
inventory split, mine/drop, and nil-Create BlackStone). Leftover Hero
target gaps landed as `7984918`, `64112df`, and `a3c452c`: Spell.None
melee hits a front-cell Hero after Player/Monster, ordinary RangeAttack
queues live Hero projectiles, and player Purification admits friendly
Heroes through owner IsFriendlyTarget. C# FlashDash still excludes Hero
and was not extended. Focused tests pass. Targeted race on the hero-
target tests passed 2.016s. Filtered broad with the six documented
timing tests skipped completed crystal-server in 104.318s and failed
only the known Guard/TaoGuard attack packet-order baseline flakes
(`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`; this run was the
missing-73/74/75/77 variant). Shop quirks and inspection packet-26 did
not fail this run; treat suite flakes there as documented, not a new
skip.
Leftover ordinary melee/shield slices landed as `2342683`, `3a650bb`,
`8d3abc2`, and `a16d4ca`: undead ordinary and warrior front hits add
Stat.Holy, Spell.None selects frontTargets[0] in cell insertion order
(empty list still falls back to playerAtLocked/monsterAtLocked), and
MagicShield/ElementalBarrier durations use luck-aware MC. C# FlashDash
still excludes Hero. Ordinary +300ms delayed impact remains immediate.
Focused tests pass. Filtered broad with the six documented timing tests
skipped completed crystal-server in 104.964s and failed only the known
Guard/TaoGuard attack packet-order baseline flakes
(`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`) plus the known inspection
packet-26 flake (`TestInspectArchivedOnlineObjectUsesRegistry`). No new
skip. CastleGate `AutoOpen` is dead C# and is not implemented.
`PvpCanResistMagic` is editor-only. `CredxGold` is the editor GameShop
gold-price seed. Leftover FocusMasterTarget assignment landed as
`5979d6a`, `ce04c0e`, and `168460a`: player Attack/RangeAttack/Magic
copy a live hostile target onto non-creature pets and the spawned Hero
immediately, leftover FireBall Hero admission uses luck-aware MC, and
RangeAttack assigns pets before CanFly can cancel the shot. Focused
tests and targeted race pass. Filtered broad with the six documented
timing tests skipped completed crystal-server in 104.668s and failed
only the known Guard/TaoGuard attack packet-order baseline flakes
(`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`). Inspection/shop quirks did not
fail this run. No new skip. Leftover ordinary Spell.None CompleteAttack
landed as `0191044`, `7e17b73`, and `166df69`: player melee queues the
shared +300ms ACAgility impact used by Slaying. Holy, FatalSword arming,
MPEater, Hemorrhage, and FocusMasterTarget stay at admission. Focused
tests and targeted race pass. Filtered broad with the six documented
timing tests skipped completed crystal-server in 115.415s and failed
only the known Guard/TaoGuard attack packet-order baseline flakes
(`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`; this run was the missing-73/74/75/77
variant). Inspection/shop quirks did not fail this run. No new skip.
Leftover Hiding HideFromTargets and MoonLight/DarkBody CheckSneakRadius
landed as Go `7eaffd2` and `40b4c40`. Player Hiding now clears nearby
monster targets like MoonLight. SneakingActive uses Chebyshev radius 3
for other Players only; isolated sneak is non-blocking ObjectRemove,
nearby players restore BroadcastInfo, and Heroes/radius-4 do not count.
C# FlashDash still excludes Hero. CastleGate AutoOpen is dead C# and is
not implemented. PvpCanResistMagic is editor-only. CredxGold is the
editor GameShop gold-price seed. Focused tests and targeted race pass.
Filtered broad with the six documented timing tests skipped completed
crystal-server in 121.395s and failed only the known Guard/TaoGuard
attack packet-order baseline flakes (`TestSessionGuardAttackTranscript`
and `TestSessionTaoGuardAttackTranscript`; this run was the
missing-73/74/75/77 variant, packet 82 only). Inspection/shop quirks
did not fail this run. No new skip.
Leftover HumanObject.Process run fatigue, stacking unstack, and torch wear
landed as Go `26d721f`. Unmounted runs increment `_runCounter` and
ChangeHP(-1) after 10. Teleport/NPC Show arm 1s stacking pushes.
Equipped torches wear every 10s and delete at 0 dura. C# FlashDash still
excludes Hero. CastleGate AutoOpen is dead C# and is not implemented.
PvpCanResistMagic is editor-only. CredxGold is the editor GameShop
gold-price seed. Focused tests and targeted race pass. Filtered broad
with the six documented timing tests skipped completed crystal-server in
105.159s and failed only the known Guard/TaoGuard attack packet-order
baseline flakes (`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`; this run was the
missing-73/74/75/77 variant, packet 82 only). Inspection/shop quirks
did not fail this run. No new skip. Remaining work is leftover
packet/spell-AI behavior, then representative restart/economy
integration.

## Current Rank 1 timing diagnostic — 2026-09-14

A focused current-source full-world diagnostic materialized 75,719 monsters
under the unchanged production/default `TimeOut=10000` and measured three
world-tick passes at 1.141s, 1.080s and 1.146s. The CPU profile is
`/tmp/pkg5-fulltick-current-20260914.cpuprofile` (SHA-256
`a94a68f25e99caee05dd84a3174058e3d0f214781537e96578fc8da4e8cc93af`). The
5.74s profile includes startup/load work as well as ticks; therefore
`spawnPositions` (0.84s inclusive) is not recorded as a steady-state tick
cost. `gameWorld.tick` was 3.34s inclusive and `tickMonsterAILocked` 1.17s in
the capture. This strengthens the bounded Rank 1 evidence without closing
indefinite scheduler soak or production cutover acceptance. A=`Partial`,
B=`No` remain unchanged; Packages 1–5 remain historically closed.

## Rank 1 controlled steady-state scheduler benchmark — 2026-09-14

The test-only `cmd/crystal-server/rank1_steady_state_benchmark_test.go` now
provides a reproducible controlled scheduler measurement separate from
startup/materialization. It builds exactly 75,719 monsters on one open 512×512
map, uses unique cells and no players, pins due fields beyond the measured
window, and warms up for three ticks. This is not the canonical imported-world
fixture or a C#/4L comparison.

The `GOMAXPROCS=1`, `-benchtime=3x`, `-count=3` run from Go source base
`d901765` is recorded at
`/tmp/rank1-steady-state-controlled-d901765-20260914.log` (SHA-256
`2294e27e8a9de97c80eada32028bcc4188a1b4756bc06267f4a0d4994ef90a52`). The
three repeated per-tick means/ranges were 445.083 ms (441.789–447.232 ms) for
`MonsterProcessWhenAlone=false` and 5,136.142 ms (4,714.785–5,403.898 ms) for
`MonsterProcessWhenAlone=true`; allocation rates were 310,145,728 B/75,748
allocs and 386,618,944 B/151,467 allocs respectively. No approved threshold
exists, so this is evidence only and leaves A=`Partial`, B=`No` unchanged.

## True-completion Step 0 — full ranked inventory gate — 2026-09-14

Before any further C#→Go alignment, the five active blockers are explicitly
inventoried in order in `Crystal.GoServer/docs/CSHARP-GO-COMPLETION-AUDIT.md`.
The audit's detailed ten-item checklist is the authoritative proof/governance
register; this plan records the same boundaries:

1. default-timeout full-population performance/capacity: bounded 1,500/1,500
   evidence and three 75,719-monster tick samples are positive, but the mixed
   startup/load profile, absent duration/CPU/memory/GC/network/tick-latency/
   reconnect/error-rate thresholds, indefinite scheduler/capacity proof, and
   direct C#/4L steady-state comparison remain open;
2. same-state combat/payload versus live 4L: lifecycle and combat-path reach
   pass, but packet volumes, target state, impact output and cleanup are not
   synchronized. A Go-only fixed-state combat/payload fixture now exists, but
   no common Go/4L world-state fixture or portable snapshot exists, and
   persistence/restart equivalence across stores is unresolved, so parity is
   fail/open;
3. regression gate/baseline policy: the required unfiltered command is
   non-repeatable/open, not a stable green gate. From Go commit `9a311f7`, the
   first run failed after 109.116s at
   `cmd/crystal-server/inspection_session_test.go:120` (expected
   `ServerKeepAlive` packet 3, observed mail packet 26) while 21 other packages
   passed; an unchanged rerun passed all 22 packages in 108.921s. The full-log
   SHA-256 values are `3050369c78b1c0265fe32a9d324b4b5d37aa161248b5e0bca8eecb5a4e4ef179`
   and `6e15320a7dbbe4d79d5eabdfa4a602766add8c67a10c7d9ab5e0590272206ea8`.
   A targeted five-repeat passed (SHA-256
   `5385b242dd517b43b3bf2ab5a1cb31f4e3b022b61f5c205532355af9fcff4b8e`). The
   current package manifest has 22 entries (SHA-256
   `7af8d79b2272a43cee9f90fd11ee6409ae756a7a947b63652c5577f4392471c1`).
   The seven historical failures and six timing-policy exclusions are separate
   categories; only `TestMountSessionStaleRecoveryKeepsConnectionAndRetries`
   appears in both. The exact test-name manifest and disposition of formerly
   excluded tests, environment and thresholds remain open; repeatability and
   the no-new-skip policy remain explicit;
4. protocol caveats: dead/protocol-only rows, wire-only `SetCompass`, partial
   C/148, and `GameMaster=100` versus `Rested=112` are accepted policy rows,
   not invented active features. The 72/72 command-name result and ordinal
   counts still need consolidated behavioral dispositions for authorization,
   lookup, persistence, ordering, side effects, and errors;
5. operations/restart: disposable clean/abrupt recovery passes, while a
   unified generation/transaction manifest, cross-store commit identifier,
   rollback selector, newest-known-good backup selection, corruption/restore
   drills, monitoring/alerting, production runbook/readiness, explicit
   operator ownership/escalation, and 4L binary semantic comparison remain
   open.

The inventory gate is complete, not a completion claim. A=`Partial`, B=`No`,
Package 5 remains historically closed, the production `TimeOut=10000` and live
4L safety boundaries remain unchanged, and generated `Envir/`/`Goods/` data is
excluded. Further work may now be labeled alignment only when it cites one of
these five rows and one checklist item, then records an honest result.

## Rank 1 source scheduling alignment checkpoint — 2026-09-14

The first post-checklist Rank 1 alignment audit found a real architectural
candidate but no proven defect. Legacy processes due objects incrementally
through `OperateTime` and a work-budget loop (`Server/MirEnvir/Envir.cs:1996-2218`);
its monster turn orders AI, buffs, regeneration, and poison, with route handling
inside the AI turn (`MonsterObject.cs:1192-1310`, `1671-1840`). Go holds the
world lock for globally phased route, AI, regeneration, poison, expiration,
removal, and respawn work over indexed populations
(`cmd/crystal-server/world.go:11820-12106`).

The difference warrants measurement around route/target/alone gating and due-
time boundaries, but no reproduced movement, notification, combat, state, or
timing divergence justifies a Go patch. The controlled benchmark now separates
three warm-up ticks and repeated steady-state `world.tick` samples, but it is
one-map Go-only evidence: the canonical imported-world fixture,
CPU/memory/RSS, GC, network, lock duration, reconnects, timeouts, errors,
approved duration and thresholds, and a comparable C#/4L baseline remain open.
Checklist item 2 remains partial/open. No thresholds are invented and the
protected 4L listener remains untouched.

## Rank 2 combat source and replay alignment checkpoint — 2026-09-14

The post-inventory source/replay review found no concrete Go combat defect that
justifies a speculative patch. The ordinary attack paths cover direct
`MinDC`/`MaxDC`, Luck handling, front-cell target lookup, delayed impact,
durability, critical handling, poison/armour behavior, and target revalidation
across C# (`Server/MirObjects/HumanObject.cs:2848-3203, 6775-6825`,
`MonsterObject.cs:2569-2779`) and Go (`cmd/crystal-server/world.go:8555-8680`,
`warrior_attack.go:452-556, 630-817, 967-1030`).

The live captures remain diagnostic: the independently running servers selected
different object IDs/coordinates and have uncontrolled HP, AC, MAC, DC,
equipment, buffs, progression, AI state, and RNG. Canonical target hashing
normalizes object-ID bytes only. Equal or unequal impact counts therefore do
not isolate a formula or payload defect. A Go-only fixed-state ordinary-melee
fixture now exists, but checklist items 3 and 4 remain open: without a common
Go/4L fixture or equivalent allowed snapshot, no same-state combat, payload, or
persistence-equivalence claim is made and no Go patch is justified.

## Rank 2 deterministic Go combat/payload fixture — 2026-09-14

Go commit `8eb0283` adds the test-only fixture
`cmd/crystal-server/combat_durability_test.go:TestDeterministicAttackerTargetSnapshot`.
It fixes attacker object `1` at map `0`, `(10,10)`, direction `2`,
`MinDC=MaxDC=20`, accuracy `100`, and target object `2` at `(11,10)`, direction
`4`, `HP=MaxHP=100`, AC `3`. FatalSword is removed, monster AI is disabled,
the existing fixed test clock is used, and all combat rolls return zero.

The fixture asserts the `+300ms` ordinary-melee boundary, exact `ObjectAttack`,
`ObjectStruck`, and `DamageIndicator(-17, 0, 2)` payload bytes, final HP `83`,
no death or pending action, and roll bounds `[1 1 1 4 100]`. The focused normal
suite passed with SHA-256
`5aa30872c6b45891ce68fe45d6ae4272624fe6f78d39401727993e87759182ed`; the
focused `-race` suite passed with SHA-256
`366d10a6012b753cea14829a8d082d0bcfd71431a04b37fb338d4bfcd2d652ee`.

This is deterministic Go behavior/payload evidence only. It is not a common
Go/4L snapshot or live replay, does not inject state into `127.0.0.1:7000`, and
does not close same-state combat/full-payload parity or persistence/restart
equivalence. A=`Partial`, B=`No` remain unchanged.

## Rank 3 unfiltered gate recheck after Rank 2 fixture — 2026-09-14

At Go HEAD `ab6d12b`, the required unchanged command
`go test ./... -count=1 -timeout 5m` passed twice. All 22 listed packages reported
`ok`; `cmd/crystal-server` completed in 104.405s and 104.159s. The raw-log
SHA-256 values are
`7654086edaffa06d2ad829bd41c9f6350de1fee3e8fc18592a1d03558ea1ac92` and
`14b80b3e2a380b7ddfefacf9e4b6439c0f4322b86f3da03aca7199a5484953ac`.

The focused reproduction
`go test ./cmd/crystal-server -run '^TestInspectArchivedOnlineObjectUsesRegistry$' -count=5`
failed before completing five repetitions: `inspection_session_test.go:120`
received mail packet `26` where `ServerKeepAlive` packet `3` was expected. Its
raw log is `/tmp/rank3-post-ab6d12b-inspection-repeat-20260914.log` with
SHA-256 `5359e33b93c1a7cac75333170d6bfcbf5ebc2a69d36ec22362fc93dc2140c439`.

The two current full-suite passes are useful samples, but the focused
reproduction preserves Rank 3 as non-repeatable/open. Historical seven failures
and six timing-policy exclusions remain separate categories; no skip or
weakened assertion was added. A=`Partial`, B=`No` remain unchanged.

## Rank 4 bounded exceptional protocol disposition evidence — 2026-09-14

At Go commit `e78bdb0`, the test-only
`internal/protocol/legacy_packet_dispositions_test.go:TestExceptionalProtocolDispositions`
locks the bounded eight-row policy slice: S/16 `StartGameDelay`, S/213
`UserAttackMove`, and S/220 `ObjectSneaking` remain inactive/protocol-only; S/273
`SetCompass` remains wire-only; S/48 `RefineCancel`, S/101 `TeleportIn`, and
S/165 `ChatItemStats` remain ordinal-catalog/protocol-only; and C/148
`PurchaseGuildTerritory` remains partial. The test records source-declared wire
shapes and keeps `BuffGameMaster=100` distinct from `BuffRested=112`.

The focused test passed ten repetitions (SHA-256
`505b3496caaf1d4f484973f239b18497d73803c5e30440d61c71f3f1c87d1f45`). The
combined protocol batch passed three repetitions normally (SHA-256
`01d62bd7e9149940c1dd0645afab5b0994512facb81494d9b8606290971ecd81`) and once
under `-race` (SHA-256
`0602833095ff742341ddc80a946711962b635671cba3369dc86cefc3cc182970`). The
focused GameMaster/Rested probe batch passed three repetitions (SHA-256
`1927b61bf7d79c4eace864b0427e1d50a27d0b8c5d85490bb2bc8612ac365728`).

This is a Go policy/evidence lock, not a replacement for the C# source traces
for absent or commented sends, alternate live packet paths, or the C/148 decoder
omission. The all-ordinal behavioral ledger and 72-command side-effect/
authorization comparison remain open. No live 4L listener was contacted, no
inactive trigger was invented, and A=`Partial`, B=`No` remain unchanged.

## Rank 3 gate repeatability checkpoint — 2026-09-14

The required unfiltered gate ran twice unchanged at Go commit `9a311f7`. The
first run failed after 109.116s at
`cmd/crystal-server/inspection_session_test.go:120`: it expected
`ServerKeepAlive` packet 3 but observed mail packet 26; 21 of 22 packages
passed. Log SHA-256:
`3050369c78b1c0265fe32a9d324b4b5d37aa161248b5e0bca8eecb5a4e4ef179`.
The unchanged rerun passed all 22 packages in 108.921s; log SHA-256:
`6e15320a7dbbe4d79d5eabdfa4a602766add8c67a10c7d9ab5e0590272206ea8`.
A targeted five-repeat passed; SHA-256:
`5385b242dd517b43b3bf2ab5a1cb31f4e3b022b61f5c205532355af9fcff4b8e`.
The 22-package manifest is at
`/tmp/pkg5-go-package-manifest-9a311f7-20260914.txt` with SHA-256
`7af8d79b2272a43cee9f90fd11ee6409ae756a7a947b63652c5577f4392471c1`.
This demonstrates an intermittent packet-ordering symptom, not a stable
baseline pass; historical seven-test failures and six timing exclusions remain
unreconciled and no skip or weakened assertion was added.

## Rank 4 focused compatibility checkpoint — 2026-09-14

The focused ordinal/codec checks passed three repetitions normally and under
`-race`: `TestAllDeclaredPacketIDsMatchLegacySourceFixture`,
`TestPacketIDsMatchLegacyEnums`, `TestDefinedPacketIDsAreUniqueWithinDirection`,
and `TestSetCompassLegacyWire`. Normal and race log SHA-256 values are
`ef9b554fa3a2c2986e1738388ae9c284e745702ca30934cb80c7dd724b46d8a3` and
`dd935d7b3174f0d500d06475c4b8cf5d444ade3d8f0e2349fb925e4880552296`.
The GameMaster/Rested probe checks also passed normally and under race, with
SHA-256 values `1927b61bf7d79c4eace864b0427e1d50a27d0b8c5d85490bb2bc8612ac365728`
and `80dc154488eaa547176811a67797489fce30a4ddc0c0086384f5547c01f9ff36`.
This supports the `GameMaster=100` versus `Rested=112` policy and wire-only
`SetCompass`, but does not close per-ordinal behavioral classifications or the
72-command side-effect/authorization comparison; no inactive trigger is
invented.

## Rank 5 focused persistence/recovery checkpoint — 2026-09-14

The component persistence/recovery packages
`internal/auth`, `internal/legacyaccount`, `internal/legacyaccountbridge`,
`internal/legacyfile`, `internal/legacyworld`, and `internal/worlddata` passed
three normal repetitions and a focused `-race` run. Log SHA-256 values are
`a141d860c6a135256af02c9774e10fc007e84bcda723b8cf205ec4c6747539bc` and
`05605683e8cd61f11892d199a9c4611af21219aae6c0fd0d999dce522bce365f`.
These passes support component-level staged-file, backup, checkpoint, restart,
and persistence behavior only. Cross-store transaction/rollback manifests,
checksums, corruption/restore drills, readiness/metrics/alerting, supervision,
and semantic 4L `Server.MirADB` comparison remain open; no operational patch or
cutover claim follows.

## True-completion Step 0 and current ranked evidence — 2026-09-14

Rechecked the current Go completion audit against
`Crystal.GoServer/docs/CSHARP-GO-DIFF.md`,
`Crystal.GoServer/docs/CSHARP-GO-REPLAY.md`, and
`Crystal.GoServer/docs/MIGRATION-STATUS.md` through Go commit `3a4f882`.
The verdict remains functional migration A=`Partial` and production
replacement B=`No`; no No→Yes change is made. Packages 1–5 remain historically
closed and archived Goal/leaf/matrix orchestration is not revived.

Rank 1 retains positive bounded default-timeout evidence: the current
full-population run passed 1,500/1,500 bootstrap → turn → logout sessions over
30 cooldown-aware 50-client waves, with a 5.075 s maximum client time, about
75,743 materialized monsters, `TimeOut=10000`, `MaxUser=50`, and no timeout
override. The persistent monster-order optimization in `2b2f013` reduced the
isolated benchmark mean from 140.432 ms to 98.769 ms. Extended production-
duration scheduler stability and production-capacity acceptance remain open.

Rank 2's newest corrected five-attack traces supersede the older Go-hit count:
Go has 659 frames, no target strikes or health updates, and 4 miss indicators;
4L has 615 frames, no target strikes or health updates, and 5 miss indicators.
The raw trace hashes and the green fixture-focused unfiltered gate are recorded
in the Go replay/audit docs. The worlds and target state are not synchronized,
so same-state combat, full-payload and reward/death equivalence remain
Fail/open despite both lifecycle paths accepting five attacks.

That historical Rank 3 snapshot reported all 21 packages passed in
`/tmp/pkg5-post-0526aaf-fixture-unfiltered-20260914.log`; it is superseded for
current disposition by the later 22-package manifest and non-repeatable gate
recorded above. Known historical baseline failures remain documented without a
new skip. Rank 4 retains the
explicit accept/reject policy: dead/protocol-only rows are inactive or
wire-only, `SetCompass` has no invented trigger, C/148 remains partial, and
`GameMaster=100` is distinct from `Rested=112`. Rank 5 operations evidence
remains bounded and is not a cutover claim. Continue in this order, without
stopping/reconfiguring live 4L or committing generated `Envir/`/`Goods/` data.

## True-completion Step 0 recheck — 2026-09-14

The current Go audit was rechecked against `CSHARP-GO-DIFF.md`,
`CSHARP-GO-REPLAY.md`, and `MIGRATION-STATUS.md` through Go commit `6baf502`.
The verdict remains functional migration A=`Partial` and production
replacement B=`No`; no No→Yes change is made. Package 5 remains historically
closed and archived Goal/leaf/matrix orchestration is not revived.

The post-`6baf502` same-account probes again pass the identical eight-request
Go/4L lifecycle using `/tmp/4l-planb-credentials.txt`. Go recorded 560 frames
(8 client / 552 server), SHA-256
`a9915bedaa8f662a5956a05d801c70274f12a6b7e1f9059ca2990f0614bf5a56`; 4L
recorded 558 (8 / 550), SHA-256
`88925a0b2f54df6a76d0191721fe9b9697fe7dec96621485878abc029511afe`.
`6baf502` accepts the source-confirmed delayed default-NPC update with strict
four-byte validation and net.Pipe coverage. Payload volumes remain unequal
(Go `ObjectNPC=3`, `NPCResponse=2`, `ObjectSpell=30`; 4L `ObjectNPC=10`, no
`NPCResponse`, `ObjectSpell=22`), so same-state combat/full-payload parity
remains open.

Continue the ranked true-completion order: default-timeout full-population
performance/long-duration evidence without raising `TimeOut`; paired
same-account combat and payload comparison; regression baseline policy;
dead/protocol-only and `GameMaster=100` versus `Rested=112` compatibility
policy; then operations evidence. Generated `Envir/`/`Goods/` data remains
excluded from commits.

## True-completion follow-up — 2026-09-14

The active post-audit order remains Rank 1 performance, Rank 2 paired
behavioral proof, Rank 3 regression policy, Rank 4 protocol compatibility and
Rank 5 operations evidence. Package 5 remains historically closed; this is
new true-completion evidence, not a revived Goal/leaf/matrix queue.

- Rank 1 valid current-source full-population evidence now includes 30
  cooldown-aware waves of 50 existing-character clients: 1,500/1,500
  bootstrap → turn → logout sessions. The run loaded 463 maps and 6,341
  spawns (about 75,743 materialized monsters), kept `TimeOut=10000` and
  `MaxUser=50`, used no timeout override, and measured 3.313s–5.075s slowest
  clients. Runtime, per-wave outputs, log hash and unchanged world hash are
  recorded in the Go completion audit and replay. The earlier wrong-map launch
  in that disposable runtime is explicitly excluded. Bounded performance is
  positive; indefinite production-duration scheduler/load acceptance remains
  open.
- Go commit `eec03d2` closes the next local quest visibility correction with
  focused and targeted-race coverage: quest collection definitions stay in
  `NewQuestInfo`, explicit text links request only referenced definitions, and
  actual quest item transfers announce definitions immediately before their
  first transfer. This does not establish paired 4L quest payload equality.
- Step 0 A/B remains A=`Partial`, B=`No`. Continue Rank 2 paired same-account
  lifecycle, combat and full-payload comparison against live 4L using
  `/tmp/4l-planb-credentials.txt`; document honest pass/fail and do not stop or
  reconfigure 4L. Keep baseline failures visible and do not add artificial
  skips. Generated `Envir/`/`Goods/` data remains excluded.

## Current true-completion ranked recheck — 2026-09-13

This is active true-completion evidence, not the archived Goal/leaf/matrix
orchestration. The Go audit was rechecked against the current diff/replay/
status documents and commit `b8f31b5`: functional migration remains
**Partial**, production replacement remains **No**, and Package 5 remains
historically closed.

1. Rank 1 is now positively reproduced with explicit account/world/map/route/
   log exports: 50/50 distinct existing-character bootstrap → turn → logout,
   3.29s slowest client, 463 maps, 6,341 spawns, unchanged `TimeOut=10000`
   and `MaxUser=50`, and no timeout override. A cooldown-aware follow-up
   passed twelve consecutive 50-client waves (600/600) with slowest clients
   from 3.638s to 4.279s, using six-second inter-wave waits for the five-second
   Legacy IP admission cooldown. The world hash remains
   `d45f8ad0fddccb121d5befaea120d94524948576fc74b39313a1d90ab0d2f09e`;
   the detailed server log and per-wave outputs are recorded in the Go replay
   and completion audit.
   The prior 0/50 no-export runs are discarded diagnostics because their
   startup omitted account/world export loading. Extended production-duration
   scheduler stability remains open.
2. Rank 2 lifecycle is positive on Go and live 4L for the same account/
   character and identical eight-request client sequence. Redacted frame
   hashes are Go `46b292f42e782dec17ecbd756716c6ebc35cdb76982d68b80428fb4a467ed86c`
   and 4L `9ffae68912860f907095b9cbb4f29b5dacf613ea996e3fd769f9e2003a597b84`.
   Five attacks reached both paths, but the Go/4L combat traces use
   unsynchronized target classes/states and differ in impact output; parity
   remains an honest fail/open. Full payload and same-state economy/quest/
   guild equivalence remain unproven.
3. Rank 3 unfiltered `go test ./... -count=1 -timeout 5m` is green (20/20,
   113.276s; log SHA-256
   `a1073e838ef158b121b4f50f401725ba34cdd57137a9b20459b6ca351ab6dcd0`).
   Rank 4 focused policy tests pass normal and targeted race
   (`2366f2f7f2046d27698cdc5ba324a7f02984cd38611fff5dc82a7eb7f01f4397`).
4. Rank 5 focused lifecycle/checkpoint/recovery tests pass normal and
   targeted race (`0a9f7c151d8a7c87f266b4a61ef21da6c230ed0e76ff520bd593208472708263`).
   The existing operations runbook still explicitly lacks cross-store
   rollback, corruption drills, health/alerting, and production cutover
   controls.

The next actionable blocker remains same-state combat/full-payload evidence;
the current bounded capacity and regression gates are positive but do not
authorize a cutover claim. Protected `Envir/`/`Goods/` data and the live 4L
listener remain outside this work.

The supplemental Rank 2 Guard-target retry adds no parity claim: Go selected
Guard object `4075` at adjacent coordinates and completed five combat-only
attack writes with no target-impact packets, while the identical 4L attempt
selected Guard object `4090` but stopped before attack when the target was
outside the helper's bounded melee window. The redacted traces/diagnostic and
hashes are recorded in `Crystal.GoServer/docs/CSHARP-GO-REPLAY.md`; lifecycle
is positive, while same-state combat and full-payload parity remain fail/open.

## Rank 1 corrected 20-wave MaxUser soak — 2026-09-13

The valid current-source Rank 1 follow-up used disposable runtime
`/tmp/pkg5-rank1-extended-runtime.4fa00o` with explicit account/world exports,
463 maps and 6,341 spawns (about 75,743 materialized monsters). Production
defaults remained `TimeOut=10000` and `MaxUser=50`; no timeout override was
set. Twenty cooldown-aware waves of 50 distinct existing-character clients
all passed bootstrap → turn → logout, for 1,000/1,000 sessions. Per-wave
slowest clients ranged from 4.215s to 5.584s, with an overall maximum of
5.584s. The driver outputs, aggregate log/hash and clean shutdown boundary are
recorded in `Crystal.GoServer/docs/CSHARP-GO-REPLAY.md` and
`CSHARP-GO-COMPLETION-AUDIT.md`.

The aggregate log includes an earlier discarded status-port collision before
the valid interval; that interval alone is counted. This is stronger bounded
default-timeout MaxUser evidence, not indefinite production-duration scheduler
acceptance. Same-state combat/full-payload parity, rollback/corruption drills,
monitoring and cutover remain open. Protected `Envir/`/`Goods/` data and the
live 4L listener were untouched.

## Package 5 persistence/recovery follow-up — 2026-09-13

The cross-file checkpoint audit now covers interrupted `n/o` promotions for
the 117 account database, world database, conquest files, UsedGoods files and
respawn runtime state. Go loaders promote a complete `.n`, restore `.o` when
promotion stopped after the old file moved, and leave an existing final file
authoritative. `1fc3047` adds the shared recovery helper and focused loader
coverage; `147f1d0` wires the same recovery into respawn runtime loading.

`ec3ec4d` makes `WriteGuildFiles(refresh=true)` build a complete sibling
directory before swapping it into place. A readiness marker and `Guilds.n` /
`Guilds.o` recovery prevent process loss from exposing a partially refreshed
guild set; `d1f8450` updates the periodic/shutdown failure fixtures and
`4d1fd7a` removes the marker after successful activation. Focused affected
package tests and targeted race checks pass. The consolidated unfiltered
`go test ./... -count=1 -timeout 5m` gate after `d1f8450` is green, with
`cmd/crystal-server` completing in 108.843s and every package reporting `ok`.

`c19a115` wires account checkpoint recovery before production startup checks
whether `Server.MirADB` exists; a crash leaving only `.n` or `.o` is now
loaded rather than treated as a missing optional database. The production
restart regression and targeted race checks pass.

Generated `Configs/`, `Envir/`, `Localization/`, `Logs/` and
`cmd/crystal-server/Envir/` remain excluded from commits. Broader transaction
rollback, malformed-artifact policy, monitoring/runbooks, behavioral parity,
extended soak and production cutover remain open; no completion or cutover
claim follows from this persistence slice.

## Active Package 5 persistence execution — 2026-09-13

`1046074` closes the next loader boundary: modern JSON replacement resets
incoming-generation counters and non-persisted session projections, while
legacy array exports retain world-owned counter precedence. Focused auth tests
and targeted race checks pass. The follow-up account-bridge generation work
and cross-file checkpoint staging/recovery are recorded above; keep transaction
rollback beyond exercised paths, malformed-artifact policy, monitoring/runbooks,
behavioral parity, extended soak, and production cutover explicitly open.

## Active execution checkpoint — 2026-09-13

The historical Package 5 closure is retained, but the current true-completion
audit still leaves operations/data-integrity and external parity evidence open.
The first implementation batch from that audit is now landed in Go:

- `1997753` makes `auth.LoadJSON` validate every account encoding/timestamp
  before replacing live state, with malformed-later-record coverage.
- `f0df9e5` makes 117 account checkpoint promotion recover the original file
  after a failed staged rename and cleans owned staging artifacts.
- `9f543c1` applies the same rollback boundary to world/UsedGoods checkpoint
  staging; `7712d3d` syncs JSON/account-export staging files before replacement.
- `b7e8a8d` makes respawn runtime-state promotion restore the prior file on
  failure; `b08e124` copies the live 117 file before periodic save work; and
  `fb3fb3c` makes world backups atomic without disturbing the live world.
- `0500687` rejects scalar/null JSON roots without replacing live account or
  world state; `e5557a1` rejects unsupported account-store versions while
  retaining version-0 object compatibility; `ffeaccf` rejects empty or
  unknown respawn-runtime envelopes.
- Focused auth, legacy-account, legacy-world tests and targeted races pass.
  The initial consolidated gate reproduced only the known asynchronous
  `TestInspectArchivedOnlineObjectUsesRegistry` mail-before-keepalive baseline
  in `cmd/crystal-server`; the first follow-up unfiltered gate completed every
  package successfully (`cmd/crystal-server` 104.204s). The subsequent
  recovery-loader gate also completed every package successfully
  (`cmd/crystal-server` 115.431s), with raw output at
  `/tmp/pkg5-recovery-loader-unfiltered-20260913.log` (SHA-256
  `ff472a2c14744ed3c8fa6acc7f3bd8e0c1f914443d6ef42e2298d10a3abf98d9`).
  No new skip or regression was added.

Next, in dependency order: extend corruption/retry coverage across the JSON and
117 bridge without automatic speculative restore; continue only source-backed
packet/spell/AI differences; then repeat the consolidated gate and update the
replay/operations evidence. Same-state 4L combat/payload parity, extended soak,
monitoring/runbooks and production cutover remain explicit blockers. Generated
`Envir/` and `Goods/` data remains excluded.

## Legacy bridge generation follow-up — 2026-09-13

Go commit `f9c558d` closes the account-generation replacement boundary. A 117
reload now clears transient Daily callback markers and rebinds cached auction
seller/buyer account projections to the replacement character registry while
preserving world-owned economy, guild, and conquest authorities. Repeated
generation tests cover the resulting market view and guild/conquest state;
focused auth/bridge tests and targeted `-race` pass.

Next: audit the remaining cross-file checkpoint retry/rollback edges and
corruption handling. Same-state 4L combat/payload parity, extended soak,
monitoring/runbooks, and production cutover remain explicit blockers.

Go commit `51d6b11` adds bootstrap-only idle-timeout grace. The default-timeout
full-map replay now clears the GameMaster bootstrap but still resets before the
first movement response after full-population visibility; the same replay
passes with the disposable 60-second timeout override. Isolate that remaining
movement/visibility latency before changing production timeout policy; no
production configuration or cutover claim is made.

Go commit `fcdc892` reduces idle inherited-AI work on playerless maps while
preserving CheckAlone state and custom/route behavior. Focused/race tests pass;
the full-map default-timeout replay now reaches movement but still resets at
logout response, while the disposable 60-second run succeeds. Isolate the
remaining full-population world/write latency before changing production
timeout policy.

Timing isolates that boundary: about 75,743 monsters are loaded and
`tickMonsterAILocked` still spends roughly 10–12 seconds processing about
8,086 custom-AI monsters while no players are present. The inherited idle pass
skips roughly 66,550 objects; suppressing custom alone-processing classes
requires a Legacy audit and is not safe to invent.

The first source-confirmed custom-AI idle audit is now landed in Go commits
`34fd959`, `0a4300f` and `daf5513`: RevivingZombie uses the inherited idle gate
because its separate revival pass already runs before live AI; wild HumanWizard
uses it because the owner-MP branch is handled by ordinary-pet processing; and
idle Trainers are skipped only when no attacker report is pending. Focused and
targeted-race tests pass. The audit leaves Zuma/Wooma/DigOut/Cannibal and other
nearby-object visibility/wake overrides active because Legacy `FindNearby`
includes eligible monsters and Heroes, so no speculative custom suppression is
claimed. The post-batch filtered broad gate completed in 111.9s, with all
non-server packages passing; the server package reproduced current unrelated
awakening, loot/harvest/fishing, LoverRecall, reload-drop and mob-command
baseline failures plus the known Guard/TaoGuard packet-order failures. These
remain documented/non-blocking and no new skip was added. Continue the remaining
source-confirmed custom-AI audit and then rerun the consolidated acceptance gate.

Go commit `6a18b3e` extends the same source-confirmed inherited-tail boundary
through the Zuma family, WoomaTaurus, DigOutZombie, CannibalPlant,
EvilCentipede and WaterDragon. Custom wake/visibility/stage/hole hooks run
before the playerless gate; focused and targeted-race tests pass. The next
filtered broad gate completed in 105.4s with all non-server packages passing
and the same unrelated awakening/loot/harvest/fishing/LoverRecall/reload-drop/
mob-command plus Guard/TaoGuard baseline failures. No new skip or regression
was identified; continue the remaining custom-AI source audit.

Go commits `6821978`, `d6f4f10` and `11d7dcb` continue the source-confirmed
post-hook gate through TurtleGrass, ManTree, HoodedSummonerScrolls,
PurpleFaeFlower, TrapRock, GreatFoxSpirit, DragonStatue, TurtleKing,
FrostTiger and Yimoogi. Focused and targeted-race checks pass. The following
filtered broad run reproduced the same unrelated awakening/loot/harvest/fishing
and Guard/TaoGuard baselines, then reached the documented five-minute
`TestSessionPoisonCloudTranscriptAndPersistence` timeout; auxiliary packages
passed and no new skip was added. GeneralMeowMeow, DemonGuard and HellLord
remain separate state-machine audits.

Go commit `0eee3ee` closes the DemonGuard portion of that audit: the live Zuma
wake hook remains active, while the inherited playerless tail is gated after
it and dead-state revival remains separately processed. Focused and targeted-
race checks pass. No additional broad run was started after the documented
PoisonCloud timeout; GeneralMeowMeow and HellLord remain separate audits.

Go commit `d36b685` completes that small state-machine slice: GeneralMeowMeow
gates playerless search, while HellLord resets its stage and preserves the
inherited-alone state before returning. Active-target behavior is unchanged;
focused and targeted-race checks pass. The documented PoisonCloud broad
timeout remains the last batch-level baseline, so no broad rerun was repeated.

Go commit `87ea623` extends the same post-hook boundary through Shinsu,
EvilMir, EarthGolem, CreeperPlant and HornedCommander. Mode, sleep/wake, stone,
visibility and health-phase transitions remain active before idle search is
suppressed; focused and targeted-race checks pass. Owner/pet-specific custom
loops and non-base StoneTrap behavior remain intentionally outside this gate,
and no broad rerun was started after the known PoisonCloud timeout.

Go commit `d7f19e5` adds the source ledger test for the remaining non-inherited
exceptions: owner/pet-only HumanAssassin, VampireSpider, SpittingToad,
SnakeTotem, CharmedSnake and IntelligentCreature paths, plus CastleGate,
BoulderSpirit and StoneTrap. Focused and targeted-race checks pass; no broad
rerun was repeated for this test-only audit closure.

A fresh current-code full-map replay now passes under the disposable
`CRYSTAL_TIMEOUT_MS=60000` override: all 463 map metadata records loaded,
authenticated `Pkg5Current` create/gameplay/logout/relogin/turn completed, and
the server stopped cleanly. The account export changed only for the persisted
character (`c9387064...` to `e089ebf2...`); `world.json` remained byte-for-byte
unchanged. **KNOWN OPEN BOUNDARY:** the default 10-second
movement/visibility latency remains open, with no production timeout or
cutover change claimed.

The matching current-code diagnostic without the disposable timeout loaded all
463 map metadata records and reached the authenticated relogin turn on isolated
`127.0.0.2:17321`, then timed out waiting for the turn response after 15
seconds. This confirms the existing default 10-second full-population
movement/visibility boundary; no persisted comparison, production timeout
change, or cutover claim follows.

The focused representative restart/economy batch was rerun after the
custom-AI audit and passes normally and under targeted `-race`: quest reward
restart, NPC shop logout persistence, guild storage restart, economy checkpoint
counters/collection, and ordinary combat. Full representative load/cutover
acceptance was historically open at this checkpoint; the authoritative Package
5 closure below retains only the known default-timeout performance boundary,
with PoisonCloud and other baselines documented separately.

Temporary opt-in timing on the same disposable full-map runtime measured the
locked world tick at roughly 0.5s before monster AI, 1.3–1.7s in monster AI,
and 2.0–2.6s total; the relogin turn waits behind repeated ticks. The timing
instrumentation was removed. Remaining custom nearby-object probes are
source-sensitive, so no speculative suppression or production timeout change
was made.

## Package 5 closure — authoritative

Packages 1–5 are accepted and complete. Focused parity, packet/spell-AI,
restart/economy, authenticated replay, persisted restart and explicit-timeout
full-map rehearsal checks pass. The only known open boundary is the default
10-second full-map movement/visibility performance limit, measured at roughly
2–2.6 seconds per locked world tick; the disposable 60-second run passes. This
is a performance acceptance boundary, not an unfinished Package 5 feature; do
not change production timeout policy or claim cutover/migration completion from
this evidence. Dated open-status entries above are historical and superseded.

## Package-5 item-identity checkpoint — 2026-09-11

Go commits `912f7ce` and `f868ffe` remove the intelligent-creature and shared
`nextCharacterItemID` fallbacks. Persistent rewards, shop/split/NPC-give/craft/
quest-carry/mine/drop paths now require the configured world allocator,
matching production `authService.AllocateItemID`; absent or exhausted
allocators reject creation rather than fabricating character-local identities.
Focused and targeted-race tests pass. The item-ID fallback audit is complete;
the next slice is representative restart/economy acceptance. The known
Guard/TaoGuard packet-order baselines remain non-blocking, and
`cmd/crystal-server/Envir/` remains untracked.

The focused representative acceptance batch also passes after the identity
audit: quest reward restart, NPC shop logout persistence, guild storage
restart, economy checkpoint counters, and ordinary combat pass in focused and
targeted-race runs. Full representative load/cutover evidence remains open;
the known Guard/TaoGuard packet-order baselines remain non-blocking.

Go commit `734edc7` adds an existing-character restart probe. A fresh
post-session export restarted on an isolated listener, relogged `Pkg5Replay`
without creating/deleting it, completed gameplay/logout, and preserved the
account character/item counters while leaving `world.json` byte-for-byte
unchanged. Focused probe/race tests pass. This closes the focused persisted
restart boundary; full representative load/cutover acceptance remains open.

The complete-map follow-up is now isolated: Go commit `b0b897b` matches Legacy
case-insensitively on map filenames/extensions, and the changed rehearsal
loaded all 463 map metadata records from 497 available map files with zero map
warnings. Existing-character authenticated gameplay/logout passed with the
explicit disposable `CRYSTAL_TIMEOUT_MS=60000` override. The default 10-second
timeout still resets this large bootstrap, so production/default-timeout tuning
and full load/cutover acceptance remain open; no production configuration was
changed and no credential blocker remains.

## Current package-5 checkpoint — 2026-09-11

Daily parity is complete across online midnight reset, offline auth reset,
and one-shot authenticated `[@_Daily]` replay. The final filtered gate
retained only the documented Guard/TaoGuard packet-order baselines; continue
the remaining packet/spell-AI audit from the documented non-goals.

- `a9dd9c3` restores the source-confirmed Hero `SpellToggle(None)` route for
  `CounterAttack`: living summoned Heroes now receive the seven-second AC/MAC
  buff and authoritative mana deduction, with focused/race coverage. Hero
  CounterAttack retaliation in shared receivers remains the next focused leaf.
- `74703a8` completes that retaliation leaf for positive player/monster→Hero
  receiver paths, including Hero-owned delayed AC damage, skill training and
  owner-backed target authority. Focused/race coverage passes; the filtered
  broad gate retains only the documented Guard/TaoGuard packet-order baselines.
- The follow-up Legacy packet/spell-AI comparison found no additional
  reachable gap after the inherited Hero toggle audit. The focused
  representative integration batch (quest relogin/restart, NPC shop logout,
  guild storage restart, economy checkpoint and ordinary combat) passes.
  Continue toward the representative dataset rehearsal; retain the documented
  non-goals and baseline failures.
- `76dbe08` adds connected-session acceptance for the inherited Hero
  CounterAttack toggle route; its packet order and mana result pass focused
  and targeted-race validation.

- Real-data rehearsal startup/handshake now passes from a fresh 117/0 export
  in a disposable runtime. Authenticated replay remains open because the
  exported account credentials are unavailable; continue with the documented
  representative authenticated dataset evidence when credentials are supplied.
- A second clean startup/shutdown against the same exported files also reaches
  `Network Started`, so restart loading is evidenced; authenticated replay and
  persisted gameplay comparison remain credential-gated.
- `1e04104` wires Hero CounterAttack into five specialized monster→Hero
  receivers that bypass the shared body, with focused/race coverage and a
  filtered broad gate retaining only the documented Guard/TaoGuard baselines.

- `bc18674` wires Hero CounterAttack into the next five specialized
  monster→Hero receivers: DarkOmaKing, GeneralMeowMeow, TucsonGeneral,
  EarthGolem and AssassinBird. Focused direct-receiver coverage and the
  targeted race pass. The filtered broad gate completed
  `cmd/crystal-server` in 105.144s and retained only the documented
  Guard/TaoGuard packet-order baselines; all other packages passed. No new
  skip was added, and `cmd/crystal-server/Envir/Goods/700.msd` remains
  untracked/excluded.

- `c08bf03` completes the specialized monster→Hero CounterAttack receiver
  audit by wiring HumanAssassin's positive Hero-hit path into the shared
  retaliation queue. Focused direct-receiver coverage and the targeted race
  pass. The consolidated filtered broad gate completed
  `cmd/crystal-server` in 114.625s and retained only the documented
  Guard/TaoGuard packet-order baselines; all other packages passed. No new
  skip was added, and `cmd/crystal-server/Envir/Goods/700.msd` remains
  untracked/excluded.

- `7a93606` restores Legacy monster-master targeting for Hero CounterAttack:
  a pet hit now retaliates against its Player, Hero or monster master instead
  of the pet object, matching `LastHitter = attacker.Master ?? attacker`.
  Focused master-target coverage and the targeted race pass. The shared-helper
  filtered broad gate completed `cmd/crystal-server` in 127.777s and retained
  only the documented Guard/TaoGuard packet-order baselines; all other
  packages passed. No new skip was added, and `700.msd` remains
  untracked/excluded.

- `fe14312` records Hero monster hitter attribution in the first five direct
  specialized receivers (AncientBringer, ScalyBeast, RhinoPriest,
  StoningStatue and DarkOmaKing); `a58b7a2` completes the remaining five
  (GeneralMeowMeow, TucsonGeneral, EarthGolem, AssassinBird and HumanAssassin).
  Positive hits retain the source-confirmed ten-second monster/Player/Hero
  hitter IDs after the existing Hero damage reset. Focused attribution/race
  checks pass. The consolidated filtered broad gate completed
  `cmd/crystal-server` in 105.025s and reproduced only the known
  Guard/TaoGuard packet-order baselines plus the documented intermittent
  inspection packet-26 baseline; all other packages passed. No new skip was
  added.

- `c6ecac4` adds the source-confirmed 500ms Hero monster-hit
  `Struck`/`ObjectStruck` throttle to the shared receiver and first five
  specialized paths; `54113eb` completes the remaining five specialized
  paths. Focused repeated-hit coverage and targeted race checks pass. The
  consolidated filtered broad gate completed `cmd/crystal-server` in
  105.195s and retained only the documented Guard/TaoGuard packet-order
  baselines; all other packages passed. No new skip was added.

- `9af1b37` applies the shared Hero stealth-removal rule to the first five
  direct monster receivers; `063599a` completes the remaining five. Positive
  monster hits clear Hero MoonLight/DarkBody state and publish the existing
  hidden-state removal packet before retaliation/damage. Focused direct/race
  checks pass. The consolidated filtered broad gate completed
  `cmd/crystal-server` in 105.777s and retained only the documented
  Guard/TaoGuard packet-order baselines plus the intermittent inspection
  registry baseline; all other packages passed. No new skip was added.

- `b43597c` applies the source-confirmed LRParalysis removal and Hero operation
  reset to the first five direct monster receivers; `24354aa` completes the
  remaining five. Focused poison-state/race checks pass. The consolidated
  filtered broad gate completed `cmd/crystal-server` in 105.471s and retained
  only the documented Guard/TaoGuard packet-order baselines; all other
  packages passed. No new skip was added.

- Candidate immediate natural-regen reset commits `0e01ed5`/`b4efcb6` were
  rolled back by `4fe21ad`/`0e9e944`: the existing specialized Hero death/
  protection acceptance contract intentionally retains `RegenResetPending`
  for the tick-based reset on those receivers. Focused death/direct/race
  checks pass after rollback; the filtered broad gate retained only the
  documented Guard/TaoGuard packet-order baselines.

- `9f7b65f` applies Legacy shield-duration damage to the first five direct
  monster→Hero receivers; `a767018` completes the remaining five. Positive
  hits shorten MagicShield/ElementalBarrier while preserving buff stats and
  clocks. Focused duration/race checks pass. The consolidated filtered broad
  gate completed `cmd/crystal-server` in 106.409s and retained only the known
  Guard/TaoGuard packet-order baselines plus the documented intermittent
  `TestSessionPlayerMeleePvPTranscript` baseline; all other packages passed.
  No new skip was added.

- `ae79835` applies the existing Hero EnergyShield absorption authority to the
  first five direct monster→Hero receivers; `9456c57` completes the remaining
  five. Focused absorption-order/race checks pass. The consolidated filtered
  broad gate completed `cmd/crystal-server` in 107.888s and retained only the
  documented Guard/TaoGuard packet-order baselines; all other packages passed.
  No new skip was added.

- `3ed5420` adds pre-damage Hero reflection to the first five direct monster
  receivers; `dea04dd` completes the remaining five. `623ab3b` corrects the
  zero-rate path so Reflect does not consume an RNG draw when disabled. Focused
  reflection/race checks pass. The consolidated filtered broad gate completed
  `cmd/crystal-server` in 105.481s and retained only the documented
  Guard/TaoGuard packet-order baselines; all other packages passed. No new
  skip was added.

- Focused representative package-5 rehearsal batch passes in 0.323s:
  quest reward restart, NPC shop logout persistence, guild storage, economy
  checkpoint collection, and ordinary combat transcript. Real-data
  authenticated replay and paired persisted-state comparison remain gated on
  credentials unavailable in the exported dataset.

- `e4c5a62` restores non-weapon Hero `DamageDura` for the first five direct
  monster receivers; `56e87ff` completes the remaining five, and `1874f12`
  extends the same runtime-authority wear to shared monster/player→Hero
  receivers. Hero durability packets remain filtered, while broken equipment
  refreshes runtime stats. Focused durability/race checks pass. The
  consolidated filtered broad gate completed `cmd/crystal-server` in
  105.468s and retained only the documented Guard/TaoGuard packet-order
  baselines; all other packages passed. No new skip was added.

- `710ec23` applies Legacy `DamageReductionPercent` before armour admission to
  the three remaining direct Hero receivers: AncientBringer, AssassinBird and
  HumanAssassin. Focused HP-reduction/race checks pass. The consolidated
  filtered broad gate completed `cmd/crystal-server` in 104.824s and retained
  only the documented Guard/TaoGuard packet-order baselines; all other
  packages passed. No new skip was added.

- `e4c5a62` restores Legacy non-weapon Hero `DamageDura` for the first five
  direct monster receivers; `56e87ff` completes the remaining five. Runtime
  equipment authority wears one point per eligible slot, refreshes broken
  equipment stats, and suppresses Hero durability packets as Legacy does.
  Focused durability/race checks pass. The consolidated filtered broad gate
  completed `cmd/crystal-server` in 105.380s and retained only the documented
  Guard/TaoGuard packet-order baselines; all other packages passed. No new
  skip was added.

- `3ed5420` adds pre-damage Hero reflection to the first five direct monster
  receivers; `dea04dd` completes the remaining five. `623ab3b` corrects the
  zero-rate path so Reflect does not consume an RNG draw when disabled. Focused
  reflection/race checks pass. The consolidated filtered broad gate completed
  `cmd/crystal-server` in 105.481s and retained only the documented
  Guard/TaoGuard packet-order baselines; all other packages passed. No new
  skip was added.

## Current package-5 checkpoint — 2026-09-10

- `e143c1d` gates the player death default-NPC callback on a registered
  `[@_Die]` page and preserves the existing default activation path when that
  page is absent. `5e81c2c` adds the positive registered-page session case.
  Focused death/NPC-access/integration tests and the targeted race slice pass.
- `03e8e32` restores the ordinary PlayerObject.LevelUp default-NPC callback
  through the shared experience notification path; focused/race checks pass.
- `1d45f0d` restores default-NPC `[@_MapEnter(map)]` at the shared cross-map
  transition boundary with focused/race coverage.
- `5c734af` restores registered default-NPC `[@_Login]` during authenticated
  bootstrap with focused/race coverage; absent pages remain silent.
- `a7873f0` restores registered default-NPC `[@_UseItem(shape)]` for Script
  items with focused/session/race coverage and authority-backed consumption.
- `aa6bd72` restores `[@_MapCoord(map,x,y)]` registration and Turn/Walk/Run
  dispatch with focused/race coverage; Daily remains blocked on missing Go
  NewDay state.
- `f53ea02` / `382f5d1` add the production online midnight Daily reset and
  `[@_Daily]` callback with focused/race coverage; offline NewDay persistence
  remains open.
- `1409ec3` adds auth-wide offline daily-completion reset; focused auth/server
  tests and targeted race pass. Offline `[@_Daily]` execution on next login
  remains separate.
- `7386296` replays pending registered `[@_Daily]` once during authenticated
  bootstrap and consumes the marker for online callbacks, with focused/race
  coverage.
- Consolidated focused package-5 acceptance passes across NewDay replay,
  MapCoord, default-NPC callbacks, quest/shop/guild/economy restart and combat
  transcripts. No additional source-confirmed packet/spell-AI leaf is selected
  yet; retain the documented baselines and non-goals.
- The corrected filtered broad gate with the six documented timing exclusions
  completed the server package in 104.339s. It retained only the known
  Guard/TaoGuard packet-order failures; all other packages passed. No new skip
  was added.
- Continue the remaining package-5 packet/spell-AI behavior audit and record
  each confirmed gap with focused tests before the next consolidated gate.

Leftover Hero OwnerRecall Back and HumanObject.Teleport stacking landed
as Go `1e55c03` and `4a99f20`. OwnerRecall uses Owner.Back (owner cell
if Back is invalid); Summon still uses Front. OwnerRecall, TurtleKing,
and HornedMage arm the 1s stacking push after CheckStacked for players
and Heroes. C# FlashDash still excludes Hero. CastleGate AutoOpen is
dead C# and is not implemented. PvpCanResistMagic is editor-only.
CredxGold is the editor GameShop gold-price seed. Focused tests and
targeted race pass. Filtered broad with the six documented timing tests
skipped completed crystal-server in 105.903s and failed only the known
Guard/TaoGuard attack packet-order baseline flakes
(`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`; this run was the
missing-73/74/75/77 variant, packet 82 only). Inspection/shop quirks
did not fail this run. No new skip. Remaining work is leftover
packet/spell-AI behavior, then representative restart/economy
integration.

## Credentialed Package-5 replay update — 2026-09-11

The earlier startup-only rehearsal's credential blocker is lifted. A fresh
disposable Go run copied from `/tmp/pkg5-auth-ready/` authenticated as
`pkg5test`, created `Pkg5Replay`, completed gameplay/bootstrap traffic, cleanly
logged out, relogged the same character, performed one harmless turn, and
cleanly logged out again on isolated listener `127.0.0.2:17453`. The protected
4L `127.0.0.1:7000` server and its databases, `Envir/`, and `Goods/` were not
stopped or modified. Go commit `b2307b8` adds the opt-in relogin probe path and
real-data ambient bootstrap filtering.

Post-shutdown persistence evidence: `accounts.json` changed from zero
`pkg5test` characters to persisted character index 3 (`Pkg5Replay`), with
`nextCharacterId` 3→4 and `nextItemId` 8→12. `world.json` remained
byte-for-byte unchanged. Exact hashes, the 463-map metadata startup evidence,
and the clean-stop log markers are recorded in
`Crystal.GoServer/docs/MIGRATION-REHEARSAL.md` and
`Crystal.GoServer/docs/MIGRATION-STATUS.md`. Credentials and disposable JSON
files remain outside Git.

This closes the credential-only replay blocker, not Package 5 acceptance. The
remaining slice is the source-confirmed packet/spell-AI audit and consolidated
restart/economy acceptance. Existing Guard/TaoGuard packet-order baseline
failures remain documented and non-blocking; no broad suite was rerun for this
focused replay leaf.

Leftover Hero Process torch wear and WarriorHero.ProcessFriend Rage/
ProtectionField landed as Go `cfbf214` and `4deff57`. Heroes wear torches
every 10s and DeleteItem at 0 dura without DuraChanged. Warriors select
Rage then ProtectionField while a target exists and apply them immediately
during Magic. C# FlashDash still excludes Hero. CastleGate AutoOpen is
dead C# and is not implemented. PvpCanResistMagic is editor-only.
CredxGold is the editor GameShop gold-price seed. Focused tests and
targeted race pass. Filtered broad with the six documented timing tests
skipped completed crystal-server in 105.105s and failed only the known
Guard/TaoGuard attack packet-order baseline flakes
(`TestSessionGuardAttackTranscript` and
`TestSessionTaoGuardAttackTranscript`; this run was the
missing-73/74/75/77 variant, packet 82 only). Inspection/shop quirks
did not fail this run. No new skip. Remaining work is leftover
packet/spell-AI behavior, then representative restart/economy
integration.

## Rank 5 recovery transaction contract — 2026-09-14

The next true-completion checkpoint records checklist item 7 without reviving
the historical Package 5 queue or changing runtime behavior. The current Go
contract is local atomic/recoverable staging per artifact, not one transaction
across account JSON, 117 account state, world JSON/`Server.MirDB`, guilds,
conquests, UsedGoods, and respawn/runtime state.

Source-backed boundaries:

- `persistPeriodicWorkLoopSave` runs `accounts → database → guilds → goods →
  conquests`, keeps the first error, and continues later steps;
- `auth.Service.SaveJSON` renames JSON before its registered 117 checkpoint hook,
  and a hook failure reports `JSONCommitted=true`;
- account, world, conquest, UsedGoods, and respawn binary/sidecar writers use
  local temporary/`n/o` promotion, while guild refresh adds `.ready` and
  `Guilds.n`/`Guilds.o` directory recovery;
- existing final artifacts remain authoritative under local recovery rules;
  malformed primaries fail load, and dated backups are not automatically
  selected;
- goods acknowledgement is per file, guild/conquest acknowledgement is after
  its writer succeeds, and failed respawn persistence restores its dirty flag.

There is no shared generation/transaction manifest, checksum catalog,
cross-store commit identifier, durable recovery record, deterministic rollback
selector, or automated newest-known-good restore selector. A later periodic
failure can therefore leave earlier stores committed and later stores at their
prior generation; local `.n`/`.o`/`.ready` recovery cannot reconcile that
cross-store state. The full matrix and source references are recorded in
`Crystal.GoServer/docs/CSHARP-GO-OPERATIONS.md`; the audit, replay, and status
documents carry the same boundary.

This is documentation-only evidence for checklist item 7. Restore/corruption
drills, observability/readiness, semantic 4L persistence comparison, and the
final decision gate remain open. A=`Partial`, B=`No`, Package 5 remains
historically closed, C# remains read-only, the live 4L listener remains
untouched, and generated `Envir/`/`Goods/` data remains excluded.

## Rank 5 restore and corruption drill checkpoint — 2026-09-14

Checklist item 8 now has a bounded automated Go-only JSON/117 bridge drill in
`Crystal.GoServer/internal/legacyaccountbridge/bridge_test.go`, using a private
`t.TempDir()` and no live 4L or generated runtime data. The test proves only
local artifact behavior: failed JSON staging preserves the prior final; a
post-rename 117 hook failure returns `JSONCommitted=true` with newer JSON and
prior 117 state; `.n` and `.o` recover into fresh services; stale `.n`/`.o`
remain subordinate to an existing final; and malformed JSON/117 primaries fail
without speculative stale `.tmp`/`.o` fallback. Account, character, gold, and
counter sentinels are checked after fresh reload.

The repeated normal command passed three times (output SHA-256
`70f38337bee8580262c9da3f96a224b83569835bf0c6c3d0ca7db4f1325b03e8`):

```text
go test ./internal/legacyaccountbridge -run '^TestRecoveryDrillJSONAndLegacyBridge$' -count=3 -timeout 5m
```

The targeted race command passed once (output SHA-256
`c219ef815c40b15115523ce995d1f752298890ad64c3aefa0b8d405b3ced9bee`):

```text
go test -race ./internal/legacyaccountbridge -run '^TestRecoveryDrillJSONAndLegacyBridge$' -count=1 -timeout 5m
```

This advances item-8 evidence but does not close it. Dated-backup restore,
crash/power-loss points, complete cross-store restart equivalence,
world/economy/runtime corruption coverage, semantic 4L persistence comparison,
observability/readiness, and the final decision gate remain open. A=`Partial`,
B=`No`, Package 5 remains historically closed, C# remains read-only, the live
4L listener remains untouched, and generated `Envir/`/`Goods/` data remains
excluded.

## Rank 5 operational observability/readiness checkpoint — 2026-09-14

Checklist item 9 is a source-backed documentation-only checkpoint, not a new
implementation task. Current Go evidence covers lifecycle generation and
stop/join behavior in `cmd/crystal-server/main.go` and
`cmd/crystal-server/process_lifecycle_test.go`; startup/shutdown marker order
for `EnvirStarted`, `NetworkStarted`, `NetworkStopped`, and `EnvirStopped`;
persistence failure and shutdown error outcomes; game/Legacy status listener
reachability; and `SIGHUP`/reboot generation transitions. Rolling category logs
and the on-demand credentialed protocol probe provide local signals only.

The repository still lacks a readiness/health endpoint, metrics exporter,
alerting policy or destination, supervisor unit, automatic crash restart/backoff
policy, automated dated-backup restore command, durable cross-store transaction
log, and owned operator escalation record. The Legacy status listener is not a
readiness or dependency-health endpoint. An unexpected runner failure requires
an explicit operator stop before a new start is accepted; there is no external
monitor or owner for that transition. The item-8 JSON/117 drill does not
provide these controls, and no authorized operations owner has accepted their
absence.

Item 9 remains open and item 10 is therefore ineligible. This checkpoint adds
no runtime behavior or production control. Keep A=`Partial`, B=`No`, Package 5
historically closed, C# read-only, the live 4L listener untouched, and generated
`Envir/`/`Goods/` data excluded while the operations owner/acceptance,
observability, and readiness gaps remain unresolved.

## Rank 3 inspection/mail ordering convergence — 2026-09-14

The intermittent inspection/mail packet-order item now has a bounded
current-source convergence result. Before the test-only synchronization fix,
`go test ./cmd/crystal-server -run '^TestInspectArchivedOnlineObjectUsesRegistry$' -count=20 -timeout 5m`
failed 5/20 with `ServerObjectRemove` packet ID 26 where the strict barrier
expected `ServerKeepAlive` packet ID 3. The cause was a test boundary: the
bootstrap helper returned at `ServerGuildBuffList`, while production startup
continued to `MaterializeCharacterRankingAt` in `main.go:6508-6511`; archiving
the target before that step allowed its asynchronous removal notification to
contaminate the viewer stream.

`inspection_session_test.go` now waits on `targetClient.barrier(t, 7100)` before
`ARCHIVEPLAYER`. The strict packet assertions remain unchanged. The focused
normal 100-repeat run and focused `-race` 20-repeat run both passed, with
output SHA-256 values
`8b7a3f4dad6b750d849d0c8d7cf689dd08f495b9f3d6ee5cd6befd84f78e3026` and
`1bdcbb4d7060830bd01dfbaa368a98fe30b9f7eab61250696d70e4ecaae22d98`.
Three consecutive unfiltered gates passed all 22 listed packages; `cmd/crystal-server`
completed in 105.156s, 105.683s, and 105.272s. Their output SHA-256 values are
`b682023edb835756f36e8acce024e6ea0a967bbf25e18fdc05e444dd60691a89`,
`8182673ebc5b085787cdc9b6b1957ef0562753c8afd41393b4588ed5dd8bae61`, and
`5cb0390ba96cff2c1488f056602a68541f957aeaa32500bb991ecd293cb04d27`; the
third retained log is `/tmp/pkg5-rank3-post-8042a08-unfiltered-20260914.log`.

This converges the specific packet-order symptom without changing production
behavior or live 4L. The broader Rank 3 baseline policy remains open until
historical failures, exclusions, and the authoritative manifest/ownership are
reconciled. A=`Partial`, B=`No`, Package 5 remains historically closed, C#
remains read-only, the live 4L listener remains untouched, and generated
`Envir/`/`Goods/` data remains excluded.

## Rank 2 portable ordinary-melee snapshot fixture — 2026-09-14

Checklist item 3 now has a test-only portable semantic vector at
`Crystal.GoServer/cmd/crystal-server/testdata/rank2_ordinary_melee_snapshot.json`
(SHA-256 `ed7745d164a2004fe4551f99d9a40f0d9a6e7895e2172c497653cc7879606ccb`).
It fixes the attacker/target state, 300ms impact boundary, admission result,
load-bearing packet payload fields, final HP/death state, packet IDs, and
random-bound sequence. `rank2_snapshot_test.go` embeds and executes the vector;
it is a future offline comparator input, not a live 4L injection mechanism.

The focused normal run passed 100/100 (SHA-256
`bea1425e7819a8506e54f70f350e60bac10f52c378cd3ed6eccaf54920107297`), the
focused race run passed 20/20 (SHA-256
`ae877ffc0574a725c56c4187cac53c1de981dc33e9af598915756ce29ee5aa66`), and the
combined deterministic combat checks passed three repetitions. A subsequent
unfiltered gate passed all 22 listed packages; `cmd/crystal-server` completed
in 104.527s (SHA-256
`b2ef7791203c788d5c7e935ca82eb3a6a7af24c07e32a8629be47a05de2c72a9`).

This advances portable Go-only Rank 2 fixture evidence but does not establish
synchronized Go/4L state, full payload equality, or same-state combat/economy
outcomes. Keep Rank 2 fail/open, A=`Partial`, B=`No`, Package 5 historically
closed, C# read-only, the live 4L listener untouched, and generated
`Envir/`/`Goods/` data excluded.

## Rank 1 controlled steady-state benchmark recheck — 2026-09-14

At Go HEAD `cf528ac`, the controlled Rank 1 scheduler fixture was rerun with
`GOMAXPROCS=1`, `-benchtime=3x`, `-benchmem`, and `-count=3`. It contains
75,719 monsters on one open 512×512 map, unique cells, no players, three warm-up
ticks, and pinned future due fields.

| Mode | Mean | Range | Allocations |
| --- | ---: | ---: | ---: |
| `playerless-skip` | 445.602 ms | 435.385–459.422 ms | 310,145,728 B / 75,748 allocs |
| `process-when-alone` | 4,565.073 ms | 4,521.481–4,588.005 ms | 386,618,944 B / 151,467 allocs |

Raw log `/tmp/rank1-steady-state-controlled-cf528ac-20260914.log` has
SHA-256 `1f6806c691584d21ad0a8e29affd13e2645dffab3a311aa6ebe4e5a1760357ee`.
This adds controlled Go-only repeatability/allocation evidence, not a
canonical imported-world or C#/4L steady-state comparison and not an approved
capacity threshold. Keep Rank 1/item 2 open, A=`Partial`, B=`No`, Package 5
historically closed, C# read-only, the live 4L listener untouched, and
`Envir/`/`Goods/` data excluded.

## Rank 3 authoritative test-name manifest — 2026-09-14

Checklist item 5 now has the checked-in manifest
`Crystal.GoServer/docs/CSHARP-GO-TEST-MANIFEST-2026-09-14.txt`, generated at Go
HEAD `606280d` from `go list ./...` and `go test "$package" -list .` for every
package. It contains 22 package headings, 4,859 `Test*` names, one benchmark,
and 4,908 lines; SHA-256
`3ee1de7d181f5287c721f712ec3d65846cbde7710270adce17f47beb6cc4a979`.

This is an exact name inventory, not an execution result. The unfiltered gate,
historical failure set, timing exclusions, overlap, and owner-approved baseline
disposition remain separate. Keep checklist item 5 open pending that policy and
ownership reconciliation; A=`Partial`, B=`No`, Package 5 historically closed,
C# read-only, the live 4L listener untouched, and generated `Envir/`/`Goods/`
data excluded.

## Rank 3 historical-disposition filtered recheck — 2026-09-14

At Go HEAD `6f1d19e`, reran the exact anchored selection of the seven historical
failure records and six timing-policy records, with
`TestMountSessionStaleRecoveryKeepsConnectionAndRetries` intentionally retained
as the one overlap:

```text
go test ./cmd/crystal-server -run '^(TestSessionHallucinationTranscript|TestProductionStartupReturnsBootstrapError|TestProductionRuntimeBootstrapPrecedesGameBind|TestProcessLifecycleOuterBootstrapFailureKeepsStaleRunning|TestMountSessionStaleRecoveryKeepsConnectionAndRetries|TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin|TestDropIncludesExpandLikeGrowingLegacyList|TestSessionPoisonCloudTranscriptAndPersistence|TestSessionMapHazardSpawnDamageRemovalAndRestart|TestSessionHidingTranscriptPersistenceAndExpiry|TestNPCP7ControlFlowSessionDelayGotoFakeClockProductionEntry|TestLoverRecallNetworkSameMapCooldownAndCrossMapStaticRefresh)$' -count=1 -timeout 20m
```

The selected package passed in 0.129s. Log
`/tmp/pkg5-rank3-historical-dispositions-20260914.log` has SHA-256
`df8b3bff3a6cc3e22f6d4d2c894b57ae4fd16c506f5ea0c82332587ea5f984e6`.
All 12 names are current passes. The six non-overlap historical-failure records
are `TestSessionHallucinationTranscript`,
`TestProductionStartupReturnsBootstrapError`,
`TestProductionRuntimeBootstrapPrecedesGameBind`,
`TestProcessLifecycleOuterBootstrapFailureKeepsStaleRunning`,
`TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`, and
`TestDropIncludesExpandLikeGrowingLegacyList`; the overlap record is
`TestMountSessionStaleRecoveryKeepsConnectionAndRetries`; and the five
non-overlap timing-policy records are `TestSessionPoisonCloudTranscriptAndPersistence`,
`TestSessionMapHazardSpawnDamageRemovalAndRestart`,
`TestSessionHidingTranscriptPersistenceAndExpiry`,
`TestNPCP7ControlFlowSessionDelayGotoFakeClockProductionEntry`, and
`TestLoverRecallNetworkSameMapCooldownAndCrossMapStaticRefresh`.

This current pass result does not erase the historical labels and is not the
required unfiltered baseline. No skip, timeout, current failure, or weakened
assertion occurred. Checklist item 5 remains open pending reconciliation of the
six-exclusion policy and owner acceptance. Keep A=`Partial`, B=`No`, Package 5
historically closed, C# read-only, the live 4L listener untouched, and generated
`Envir/`/`Goods/` data excluded.

## Rank 1 longer controlled steady-state benchmark — 2026-09-14

At Go HEAD `29da9de`, reran the controlled Rank 1 scheduler fixture with
`GOMAXPROCS=1`, `-benchtime=10x`, `-benchmem`, and three repetitions. The
75,719-monster one-map fixture retains unique cells, no players, three warm-up
ticks, and future due fields pinned:

| Mode | Mean | Range | Allocations |
| --- | ---: | ---: | ---: |
| `playerless-skip` | 449.239 ms | 418.715–477.348 ms | 310,145,728 B / 75,748 allocs |
| `process-when-alone` | 4,474.710 ms | 4,457.564–4,495.933 ms | 386,618,944 B / 151,467 allocs |

Command:

```text
GOMAXPROCS=1 go test ./cmd/crystal-server -run '^$' \\
  -bench '^BenchmarkWorldTickSteadyStateControlledPopulation$' \\
  -benchtime=10x -benchmem -count=3 -timeout 15m
```

The package completed in 417.466s. Raw log
`/tmp/rank1-steady-state-controlled-10x-29da9de-20260914.log` has SHA-256
`a5f81570a901ac4f2c1a1a53d46456db9a8ce0edfd7605cf5614d3d7c8aa4e02`. This
is longer controlled Go-only evidence, not an imported-world/C#/4L comparison,
production threshold, or indefinite soak. Rank 1/item 2 remains open. Keep
A=`Partial`, B=`No`, Package 5 historically closed, C# read-only, the live 4L
listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 3 current unfiltered regression gate — 2026-09-14

At Go HEAD `8226b1d`, the unchanged required gate passed all 22 listed packages:

```text
go test ./... -count=1 -timeout 5m
```

`cmd/crystal-server` completed in 103.858s. Raw log
`/tmp/pkg5-rank3-post-8226b1d-unfiltered-20260914.log` has SHA-256
`0b54235b82662d9d8c02a17a40ebefea6a6aa33783bc9035c945f388db7ca5a8`. This is
the fourth current unfiltered green observation after the three prior
post-convergence gates. It used no filter, skip, weakened assertion, production
source change, live 4L operation, C# edit, or generated-data staging.

This strengthens current-source gate evidence but does not close item 5. Keep
the historical seven-failure records, six timing-policy records, overlap, and
owner-approved disposition as separate open policy categories. A=`Partial`,
B=`No`, Package 5 historically closed, C# read-only, the live 4L listener
untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 2 portable empty-DC ordinary-melee snapshot — 2026-09-14

At Go commit `a322f69`, added the second portable Rank 2 vector
`Crystal.GoServer/cmd/crystal-server/testdata/rank2_ordinary_melee_empty_dc_snapshot.json`
(SHA-256 `335d2d0cdf28416bf18e4cb43c2e71b2960c634925b92d9de39b9a5befcb4485`)
and `TestPortableRank2OrdinaryMeleeEmptyDCSnapshot`. It fixes ordinary
`Spell.None`, zero DC, zero target AC, adjacent cells, and 300ms delay; asserts
zero admission damage, exact attack payload, impact IDs `[75]`, exact zero-
damage miss indicator, no `ObjectStruck`/health traffic, unchanged HP 100, no
death, and random bounds `[1,1,1,4]`.

The first provisional random vector omitted the final bound; the deterministic
source result `[1,1,1,4]` was recorded in the fixture without a production-code
change. Normal 100/100 passed (SHA-256
`f3b1f20c4fca83a12ea004a76291aab5929bbc41ba1380c1df0cb3ae89d0c0f0`), race
20/20 passed (SHA-256
`63074470065e6d0048b19a2cd6a14d22aed822627767463599c556814bb4cb65`), and the
post-fixture unfiltered gate passed all 22 packages in 104.212s (SHA-256
`064bbbea18fe86afc72fba4ed007ea29b8a860ce0873055eaae31e39857ff937`). This
remains Go-only fixture evidence, not synchronized Go/4L parity. Keep Rank 2
item 3 open, A=`Partial`, B=`No`, Package 5 historically closed, C# read-only,
the live 4L listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 3 authoritative manifest regeneration — 2026-09-14

After Go commit `a322f69` added the empty-DC fixture, regenerated
`Crystal.GoServer/docs/CSHARP-GO-TEST-MANIFEST-2026-09-14.txt` using
`go list ./...` and `go test "$package" -list .` for every package. It contains
22 package headings, 4,860 test names, one benchmark, zero examples, and 4,909
lines; SHA-256 `45b216aa95450155667f04376b5377d23a2777e7d11e7b2b6843ac8b1ec3c1ee`.
The new empty-DC portable test name is included.

This remains an inventory update, not a green execution claim. Keep item 5 open
for reconciliation of the unfiltered gate, historical failure/timing-policy
records, overlap, and owner-approved baseline disposition. A=`Partial`, B=`No`,
Package 5 historically closed, C# read-only, the live 4L listener untouched,
and generated `Envir/`/`Goods/` data excluded.

## Rank 2 combined portable fixture determinism — 2026-09-14

The successful-hit and empty-DC vectors passed together under the anchored
selection with normal `-count=3` (SHA-256
`05157d4c6e617614fe26b87f67da710fdf61d90fef8e6dba5024ae716e455bd6`) and
`-race -count=20` (SHA-256
`2ad6e4f8cffe561c76857be2f498a19bb8ca76f75e550e078f641c3825eba190`). This
checks deterministic fixture coexistence, not synchronized Go/4L parity. Keep
Rank 2 item 3 open, A=`Partial`, B=`No`, Package 5 historically closed, C#
read-only, the live 4L listener untouched, and generated `Envir/`/`Goods/` data
excluded.

## Rank 1 one-minute controlled steady-state benchmark — 2026-09-15

At Go HEAD `e1b6de6`, ran the 75,719-monster controlled scheduler fixture with
one minute per benchmark sample, `GOMAXPROCS=1`, `-benchmem`, and `-count=3`:

| Mode | Mean | Range | Allocations |
| --- | ---: | ---: | ---: |
| `playerless-skip` | 432.964 ms | 423.484–440.142 ms | 310,145,728 B / 75,748 allocs |
| `process-when-alone` | 4,497.364 ms | 4,429.252–4,579.545 ms | 386,618,944 B / 151,467 allocs |

The package completed in 871.287s. Raw log
`/tmp/rank1-steady-state-controlled-1m-e1b6de6-20260915.log` has SHA-256
`da49cb6ca3858c303c1b6a5d8cee90ac2993738ca3b7083947ee581042c1bac5`. This is
longer controlled Go-only time-based evidence, not an imported-world/C#/4L
comparison, accepted capacity threshold, or indefinite soak. Keep Rank 1/item 2
open, A=`Partial`, B=`No`, Package 5 historically closed, C# read-only, the
live 4L listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 2 cross-map delayed-target revalidation fixture — 2026-09-15

Go commit `2db7546` added the portable fixture
`Crystal.GoServer/cmd/crystal-server/testdata/rank2_ordinary_melee_cross_map_snapshot.json`
(SHA-256 `d778411b38f097c3b680531805988f438a16e9554941fdd498db787c2e182e41`)
and `TestPortableRank2OrdinaryMeleeCrossMapSnapshot`. The vector admits an
ordinary adjacent melee attack for damage 20, mutates the target map index to 1
before the 300ms delayed impact, and verifies no impact packets, unchanged HP
100, no death, no pending action, and random bounds `[1]`.

Focused normal 100/100 passed (SHA-256
`a533f408de48784d73b72804f7fdaee09675f4279a8094f1b548e53c6f72e2da`); focused
race 20/20 passed (SHA-256
`b3efa249d02adb361e2157d47933fda2705abde1e6c2ebe356b8bbb0f7853fc5`). The
anchored three-fixture normal run passed with SHA-256
`e0fd51318171076e0be8daeb39f050567811b4ee1b265e5866018d35b1ec4178`. The
post-fixture `go test ./... -count=1 -timeout 5m` gate passed all 22 packages;
`cmd/crystal-server` completed in 105.579s. Raw log
`/tmp/pkg5-rank2-cross-map-unfiltered-20260915.log` has SHA-256
`8e765bfcf589f4e7b97eb43430fd607436440a323ec18996616fb994a8478f52`.

This remains portable Go-only evidence, not synchronized Go/4L parity or
same-state payload/outcome equivalence. Keep Rank 2 item 3 open, A=`Partial`,
B=`No`, Package 5 historically closed, C# read-only, the live 4L listener
untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 3 current unfiltered regression gate — 2026-09-15

At Go HEAD `a366797`, the unchanged required gate passed all 22 packages:

```text
go test ./... -count=1 -timeout 5m
```

`cmd/crystal-server` completed in 105.052s. Retained raw log
`/tmp/pkg5-rank3-post-a366797-unfiltered-20260915.log` has SHA-256
`5ffd1a77e4fcfe8e96231e33423a9d152b7e651f938f37de719e794b6d054e11`. This is
another current unfiltered green observation after the cross-map fixture, with
no filter, skip, weakened assertion, production source change, live 4L
operation, C# edit, or generated-data staging.

This strengthens current-source execution evidence but does not approve a
baseline policy or reconcile historical failure/timing-policy categories,
their overlap, or owner-approved disposition. Keep Rank 3 item 5 open;
A=`Partial`, B=`No`, Package 5 historically closed, C# read-only, the live 4L
listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 2 same-map delayed movement fixture — 2026-09-15

Go commit `9127ebc` added the portable fixture
`Crystal.GoServer/cmd/crystal-server/testdata/rank2_ordinary_melee_same_map_move_snapshot.json`
(SHA-256 `a5e82d4e53e51a70f4551f00a3e695d0c1c029193367e9e6589e2f4897bb8889`)
and `TestPortableRank2OrdinaryMeleeSameMapMoveSnapshot`. After accepted
admission, the target moves from `(11,10)` to `(12,10)` on the same map before
the 300ms impact. Delayed resolution preserves target identity, lands 17 damage,
emits `ObjectStruck` and `DamageIndicator` at current coordinates, leaves HP at
83, and records random bounds `[1,1,1,4,100]`.

Focused normal 100/100 passed (SHA-256
`e91a081d6b61aef9928438888701aa5876ae0af4957308d46899a87e0a2a1dd5`); focused
race 20/20 passed (SHA-256
`3fab4e5c5e6c83bc03d027028e4aaca692dc9e7c0327228b59a8cb37eff2ecc0`). The
anchored four-fixture normal run passed with SHA-256
`6d0f29a1984844568acdaef088f2356c73320c24cd521046521e070b68835bc7`. The
post-source unfiltered `go test ./... -count=1 -timeout 5m` gate passed all 22
packages; `cmd/crystal-server` completed in 105.667s. Raw log
`/tmp/pkg5-rank2-same-map-move-unfiltered-20260915.log` has SHA-256
`caed0842fd78a270161a5751b15309c85d31cf57f201b1336da4fce4a0a5a087`.

This remains portable Go-only evidence, not synchronized Go/4L parity or
same-state payload/outcome equivalence. Keep Rank 2 item 3 open, A=`Partial`,
B=`No`, Package 5 historically closed, C# read-only, the live 4L listener
untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 1 default-GOMAXPROCS timed controlled benchmark — 2026-09-15

At Go HEAD `3724fdc`, ran one 10-iteration sample per controlled 75,719-monster
scheduler mode without overriding `GOMAXPROCS`; benchmark suffix `-8` records
the host default `GOMAXPROCS=8`:

| Mode | Observed | Allocations |
| --- | ---: | ---: |
| `playerless-skip` | 339.000 ms/op | 310,145,764 B/op / 75,748 allocs/op |
| `process-when-alone` | 4,631.332742 ms/op | 386,618,977 B/op / 151,467 allocs/op |

Command used `time go test ./cmd/crystal-server -run '^$' -bench
'^BenchmarkWorldTickSteadyStateControlledPopulation$' -benchtime=10x -benchmem
-count=1 -timeout 15m`. The package reported 138.013s; shell timing was real
2m19.400s, user 2m22.161s, sys 0m2.202s. Raw log
`/tmp/rank1-steady-state-controlled-10x-timing-3724fdc-20260915.log` has
SHA-256 `5f4a7e7253a02941deb5bca5fe2311fd0f0102773bf61f6b3e95ca652bae1730`.
This is controlled Go-only timing/allocation evidence under default runtime
concurrency, not directly comparable to the prior `GOMAXPROCS=1` run, and not
production RSS/GC/network/reconnect, C#/4L, indefinite-soak, or accepted
capacity-threshold evidence. Keep Rank 1 item 2 open, A=`Partial`, B=`No`,
Package 5 historically closed, C# read-only, the live 4L listener untouched,
and generated `Envir/`/`Goods/` data excluded.

## Rank 3 shuffled historical-disposition recheck — 2026-09-15

At Go HEAD `4142261`, reran the same 12-test historical-failure, overlap, and
timing-policy disposition set with `-count=20 -shuffle=on`:

```text
go test ./cmd/crystal-server -run '^(TestSessionHallucinationTranscript|TestProductionStartupReturnsBootstrapError|TestProductionRuntimeBootstrapPrecedesGameBind|TestProcessLifecycleOuterBootstrapFailureKeepsStaleRunning|TestMountSessionStaleRecoveryKeepsConnectionAndRetries|TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin|TestDropIncludesExpandLikeGrowingLegacyList|TestSessionPoisonCloudTranscriptAndPersistence|TestSessionMapHazardSpawnDamageRemovalAndRestart|TestSessionHidingTranscriptPersistenceAndExpiry|TestNPCP7ControlFlowSessionDelayGotoFakeClockProductionEntry|TestLoverRecallNetworkSameMapCooldownAndCrossMapStaticRefresh)$' -count=20 -shuffle=on -timeout 20m
```

All 12 selected tests passed in 2.058s. Retained log
`/tmp/rank3-historical-dispositions-shuffle20-4142261-20260915.log` has
SHA-256 `19e08971a83fa0123817006e6b62253b11e70b036ba234530cc877d852d96a65`.
This is a current shuffled filtered recheck; historical labels remain in force,
not the required unfiltered baseline or owner-approved disposition. Keep Rank 3
item 5 open; A=`Partial`, B=`No`, Package 5 historically closed, C# read-only,
the live 4L listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 5 item 9 owner boundary — 2026-09-15

No authorized operations owner acceptance of the missing production controls is
recorded: readiness/health endpoint, metrics exporter, alert destination or
policy, supervisor and automatic restart/backoff, automated dated-backup
restore, durable cross-store transaction log, and owned escalation path. Item 9
remains open and item 10 remains ineligible. This is an explicit evidence
boundary, not a waiver or permission to invent those controls.

This checkpoint changes no runtime behavior, does not stop or reconfigure live
4L, and does not stage generated `Envir/`/`Goods/` data. Keep A=`Partial`,
B=`No`, Package 5 historically closed, and continue only with bounded Rank 1,
Rank 2, or Rank 3 evidence.

## Rank 2 delayed target-removal fixture — 2026-09-15

Go commit `79f9562` added the portable fixture
`Crystal.GoServer/cmd/crystal-server/testdata/rank2_ordinary_melee_target_removed_snapshot.json`
(SHA-256 `67f3e13df94705650c2ae89d13fdcfdfdba57d6a7ed9cdff377557c485cb85aa`)
and `TestPortableRank2OrdinaryMeleeTargetRemovedSnapshot`. The target is
removed after accepted admission and before the 300ms impact; delayed
resolution emits no impact packets, drains the queued action, leaves the target
absent, and records random bounds `[1]`.

Focused normal 100/100 passed (SHA-256
`a92b7320b3341a8186c6b3c20966da96a9a5c38d04c52415007510c4fa595630`); focused
race 20/20 passed (SHA-256
`a5050867430a1de44079735294f77801064c6c177cc962c2a57c248df86a5ad5`). The
anchored five-fixture normal run passed with SHA-256
`e0fd51318171076e0be8daeb39f050567811b4ee1b265e5866018d35b1ec4178`. The
post-source unfiltered `go test ./... -count=1 -timeout 5m` gate passed all 22
packages; `cmd/crystal-server` completed in 105.474s. Raw log
`/tmp/pkg5-rank2-target-removed-unfiltered-20260915.log` has SHA-256
`6d359c1593a54622699c9101ea3706ace8d544cffbba8fcc6f80fb3052b6916b`.

This remains portable Go-only delayed-action evidence, not synchronized Go/4L
parity or same-state payload/outcome equivalence. Keep Rank 2 item 3 open,
A=`Partial`, B=`No`, Package 5 historically closed, C# read-only, the live 4L
listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Authoritative Go test manifest refresh — 2026-09-15

Regenerated `Crystal.GoServer/docs/CSHARP-GO-TEST-MANIFEST-2026-09-14.txt`
from Go HEAD `325054b` on 2026-09-15 using the manifest's recorded command:
`for package in $(go list ./...); do go test "$package" -list .; done`.
The filename is retained for continuity; the header records the new source
commit and date. The inventory contains 22 package headings, 4,863 test names,
one benchmark, zero examples, and 4,912 lines. Its SHA-256 is
`ab53a22b1f7a0dbe28b28db7373307b3bfd5059ff03bee93f52c022d688c7ef7`.

The refreshed inventory includes `TestPortableRank2OrdinaryMeleeCrossMapSnapshot`,
`TestPortableRank2OrdinaryMeleeSameMapMoveSnapshot`, and
`TestPortableRank2OrdinaryMeleeTargetRemovedSnapshot`. It is a name inventory,
not a green execution result and not synchronized Go/4L behavioral parity.
A=`Partial`, B=`No`, Package 5 historically closed, C# read-only, the live 4L
listener untouched, and generated `Envir/`/`Goods/` data excluded.

## Rank 3 unfiltered shuffled regression recheck — 2026-09-15

At Go HEAD `3fbb33e`, ran three shuffled repetitions of the full package gate:

```text
go test ./... -count=3 -shuffle=on -timeout 20m
```

All 22 packages passed. `cmd/crystal-server` reported 312.567s for the
three-repetition package run. Retained log
`/tmp/rank3-full-unfiltered-shuffle3-20260915-retry.log` has SHA-256
`114fe5e7eb917d3ebffe2bbf3b725640dd4f1671f76d4c8ffcf7488c216aa968`.
This is current unfiltered shuffled stability evidence, but it is not an
owner-approved baseline or an approval of the historical disposition labels.
Rank 3/checklist item 5 remains open. A=`Partial`, B=`No`, Package 5 remains
historically closed, C# read-only, the live 4L listener remains untouched, and
generated `Envir/`/`Goods/` data remains excluded.

## Rank 1 repeated controlled-population benchmark — 2026-09-15

At Go HEAD `886bd0f`, reran the controlled 75,719-monster, one-open-map
fixture with `GOMAXPROCS=1`, `-benchtime=30s`, and `-count=3`:

```text
env GOMAXPROCS=1 go test ./cmd/crystal-server -run '^$' \
  -bench '^BenchmarkWorldTickSteadyStateControlledPopulation$' \
  -benchtime=30s -benchmem -count=3 -timeout 30m
```

| Mode | Sample 1 | Sample 2 | Sample 3 | Allocations |
| --- | ---: | ---: | ---: | ---: |
| `playerless-skip` | 418.362487 ms/op | 415.014491 ms/op | 422.210364 ms/op | 310,145,728 B/op / 75,748 allocs/op |
| `process-when-alone` | 4,421.392547 ms/op | 4,333.109218 ms/op | 4,409.258426 ms/op | 386,618,944 B/op / 151,467 allocs/op |

The package reported 560.099s; shell timing was real 9m21.812s, user
9m12.588s, and sys 0m10.016s. Retained log
`/tmp/rank1-steady-state-controlled-30s-gomaxprocs1-886bd0f-20260915.log` has
SHA-256 `90a6d9e8f2733e174237b1a12f8d76229c1387d7292489b335b01be51f626e3a`.
This is repeated controlled Go-only timing/allocation evidence. It is not a
production RSS/GC/network/reconnect acceptance record, C#/4L comparator,
indefinite soak, or approved capacity threshold. Rank 1/checklist item 2
remains open. A=`Partial`, B=`No`, Package 5 remains historically closed, C#
read-only, the live 4L listener remains untouched, and generated
`Envir/`/`Goods/` data remains excluded.

## Rank 2 delayed target-dead fixture and manifest refresh — 2026-09-15

Go commit `8c2de30` added the portable fixture
`Crystal.GoServer/cmd/crystal-server/testdata/rank2_ordinary_melee_target_dead_snapshot.json`
(SHA-256 `5833b9bedacf08f00e65f04e5c04a5d6d998b0d0cac70a0797ebd5cfa77d8a36`)
and `TestPortableRank2OrdinaryMeleeTargetDeadSnapshot`. After ordinary melee
admission, the fixture marks the target dead before the 300ms impact. Delayed
resolution emits no impact packets, retains the target with HP 100 and
`Dead=true`, drains the pending action, and records random bounds `[1]`.

The focused normal 100-repeat run passed with output SHA-256
`bd66982944e730a913ed7a72193e1dc0094ab4a08758d25d88bc495ca298600b`; the
focused race 20-repeat run passed with output SHA-256
`b3efa249d02adb361e2157d47933fda2705abde1e6c2ebe356b8bbb0f7853fc5`. The
six-fixture anchored normal run passed with output SHA-256
`e79926aece0de181837a316bc93de7104c2d883d58e387d7314c92770c73d0f0`.

The post-source unfiltered `go test ./... -count=1 -timeout 5m` gate passed all
22 packages; `cmd/crystal-server` completed in 108.729s. Retained raw log
`/tmp/pkg5-rank2-target-dead-unfiltered-20260915.log` has SHA-256
`90d241f381629c407348f6176b9b3bff0d1ddfbfcf6d5c5b0a82b95bf1d94c54`.
The refreshed authoritative manifest at Go HEAD `8c2de30` contains 22 package
headings, 4,864 test names, one benchmark, zero examples, and 4,913 lines; its
SHA-256 is `34e32f2f0091aba8ef775a53770c214240b50aba38d6b8bfb067d5dd5d43269d`.
It includes the target-dead test. This remains portable Go-only evidence, not
synchronized Go/4L state, payload, or outcome equivalence. Rank 2/checklist
item 3 remains open. A=`Partial`, B=`No`, Package 5 remains historically closed,
C# remains read-only, the live 4L listener remains untouched, and generated
`Envir/`/`Goods/` data remains excluded.

## Rank 3 full race-gate failure — 2026-09-15

At Go HEAD `e645905`, ran the unfiltered race gate:

```text
go test -race ./... -count=1 -timeout 30m
```

The gate failed in `cmd/crystal-server` after 142.756s; the other 21 package
results were green. `TestDeathCommandRemoteSessionBroadcastAndReload` failed
with `death_commands_session_test.go:38: mail packet id = 82, want 30 ({ID:82 Payload:[19 69 139 255]})`.
`TestSessionPoisonCloudTranscriptAndPersistence` failed under the race detector:
its write was at `poison_cloud_session_test.go:95`, the concurrent read was at
`main.go:4615`, and the serving goroutine originated at
`poison_cloud_session_test.go:78`. Retained raw log
`/tmp/rank3-full-race-gate-20260915.log` has SHA-256
`0ccbceada5d69310ad6536d31591fd75a94fa8614e42ca4db53ce9ce1a00ca51`.

This is failed race/concurrency evidence, not a green regression baseline or
owner-approved disposition. Rank 3/checklist item 5 remains open. A=`Partial`,
B=`No`, Package 5 remains historically closed, C# remains read-only, the live
4L listener remains untouched, and generated `Envir/`/`Goods/` data remains
excluded.

## Rank 3 focused race characterization — 2026-09-15

At Go HEAD `fd98c19`, reran the two failing `cmd/crystal-server` tests in
isolation under the race detector:

```text
go test -race ./cmd/crystal-server -run '^TestDeathCommandRemoteSessionBroadcastAndReload$' -count=20 -timeout 20m
go test -race ./cmd/crystal-server -run '^TestSessionPoisonCloudTranscriptAndPersistence$' -count=20 -timeout 20m
```

The death-command focused run passed with log SHA-256
`7edda4aefa8c62401d88419eba6f95c91187915689a21d3c6be8992e66728b1d`.
The poison-cloud focused command failed with the same race detector report;
its write was at `poison_cloud_session_test.go:95`, the concurrent read at
`main.go:4615`, and the serving goroutine originated at
`poison_cloud_session_test.go:78`. Its log SHA-256 is
`b460425a46f16e76d9b22136c7ec4429d40a0df6d8c98513f582812467b90c4c`.
The isolated death-command pass does not clear the full-suite packet-order
failure, and the poison-cloud race remains reproducible in focused execution.
Rank 3/checklist item 5 remains open; A=`Partial`, B=`No`, Package 5 remains
historically closed, C# remains read-only, the live 4L listener remains
untouched, and generated `Envir/`/`Goods/` data remains excluded.

## Rank 3 synchronized session-gate remediation — 2026-09-15

The reproducible PoisonCloud race was confined to the shared
`sessionWorldConfigured` flag: detached test sessions changed it after
bootstrap while `main.go:4615` read it from a live serving goroutine. The Go
change makes this flag an `atomic.Bool` and routes every read and write through
`isSessionWorldConfigured`/`setSessionWorldConfigured`; no combat, persistence,
or wire behavior was changed.

At the resulting Go working tree, the focused race gate passed:

```text
go test -race ./cmd/crystal-server -run '^TestSessionPoisonCloudTranscriptAndPersistence$' -count=20 -timeout 20m
```

The log SHA-256 is
`9d916482a4094ac75c482324875572d89f0b73e755a9b6c0081a7eea8d404d7e`.
The full `cmd/crystal-server` race package was then run:

```text
go test -race ./cmd/crystal-server -count=1 -timeout 30m
```

It still failed at `TestDeathCommandRemoteSessionBroadcastAndReload` with
`death_commands_session_test.go:38: mail packet id = 82, want 30`; its log SHA-256
is `a4f95b3d4e61374eab24b8ff884101cf5c9fb0c059f8a3d869dee0444b01afc9`.
That run contained no `WARNING: DATA RACE` and no PoisonCloud test failure.
This clears the previously characterized PoisonCloud data race for the tested
path, but the full package race gate remains failed because the packet-order
failure persists. Rank 3/checklist item 5 remains open; A=`Partial`, B=`No`,
Package 5 remains historically closed, C# remains read-only, the live 4L
listener remains untouched, and generated `Envir/`/`Goods/` data remains
excluded.

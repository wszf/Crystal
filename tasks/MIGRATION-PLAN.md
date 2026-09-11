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

## Execution and current acceptance

1. WORLD actions landed as Go `3591b2b`; focused parser/runtime/session tests pass.
2. Shield IDs landed as `712134e`; numeric wire and lifecycle/race tests pass.
3. CombineItem landed as `5c87a52` / `a5526c8`: all four branches, player/Hero
   inventory authority, packet order, rejected/replayed requests, RNG, persistence
   and concurrent consumption are covered. Package complete.
4. NPC roots and automatic contexts landed as `8bf588b` / `89daa2d`; event-driven
   context/parameter isolation, configured roots and race checks pass. Package
   complete. Go `docs/NPC-EXECUTION-CONTEXTS.md` records the source audit.
5. Rehearsal and integration audit remain open. Detailed commit and test evidence
   lives in Go `docs/MIGRATION-STATUS.md` and `docs/MIGRATION-REHEARSAL.md`.

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

Continue in this order:
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
New regressions must be fixed; pre-existing failures do not block package work. Long-running load/crash recovery and production
cutover acceptance remain unproven. No migration-complete claim yet.

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

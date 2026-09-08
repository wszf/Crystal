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
   Hero armour/combined amulet authority passes focused/archive/race/broad; UltimateEnhancer passes focused/archive/race/broad; Taoist selector/Poisoning metadata pass focused/race/broad; Poisoning passes focused/archive/race/broad; Curse metadata/impact focused/session/race; Hero Curse focused/archive/race/broad; Revelation foundation focused/session/race; broad only known inspection flake; Hero Revelation focused/archive/race/broad; SoulFireBall metadata focused/race; Hero SoulFireBall focused/archive/race; broad known inspection flake; Archer selector focused/race; Concentration focused/archive/race/broad; SpecialArrow metadata focused/race/broad; live-level/cells and monster return/Human damage focused/session/race; cooldown/repeats focused/session/race; broad only known inspection flake; poison overrides/pet brown/target rules focused/session/race; broad known inspection flake; Hero PoisonShot admission focused/race/broad; Hero MentalState toggle focused/session/race/broad; Hero PoisonShot focused/archive/race/broad; ElementalShot metadata/live-level/cell/cooldown focused/session/race/broad; monster/Human MAC return and knockback revalidation focused/race/broad; Hero orb acquisition/broadcasts/gathering foundation focused/race; Hero ElementalShot admission/completion focused/race; monster/Human combat gathering focused/race/broad; Hero ElementalShot dispatch/archive focused/race/broad; player acquisition cast/cooldown and Hero wire level focused/race/broad; StraightShot/DoubleShot range metadata/cells focused/race/broad; MAC completion/cooldown and Hero recipient admission focused/race/broad; Hero StraightShot dispatch/archive/rejection/dismissal focused/race/broad; Assassin selector and Haste/LightBody delayed dispatch before CanAttack pass focused/archive/race/broad; HeavenlySword player DC/metadata passes focused; line completion MAC return/training/cooldown passes focused; Hero HeavenlySword dispatch/completion passes focused/archive/race/broad; DoubleSlash player metadata passes focused/session; Hero DoubleSlash admission/front-cell capture foundations pass focused (unwired); Hero monster defence modes pass focused/race/broad; Hero weapon wear helper passes focused/stale/archive/race/broad; impact wear/HP drain pass focused/race/broad; Human authority and wear/live bonus pass focused/race/broad (final clamp covered by race); DoubleSlash dispatch/post-hit training passes focused/archive/dismissal/fallback/race/broad; FatalSword single-capture randomized metadata and skill DC/arm order pass focused/race/broad; Hero FatalSword armed state/DoubleSlash live defence passes focused/Human/archive/race; broad only known inspection flake; ordinary Hero DC capture and AC/Agility foundation pass expanded Hero/race/broad; Human AC mode passes focused; +300ms melee FatalSword queue foundation passes focused; ordinary +300ms dispatch passes Hero tests; Hero MPEater admission passes focused; Hemorrhage admission passes focused; combined passive archive passes; Human AC/+300ms/FatalSword/MPEater/Hemorrhage pass expanded race/broad; Hero Holy-before-passives passes focused/archive/race/broad; Hero red-poison armour roll order passes focused/race/broad; received-rate numeric helper passes focused/60k .NET oracle; Hero Stun/red/normal received-rate integration passes focused/race/broad; Human rate conversion matches 10.1m .NET cases; Human impact rates pass focused; Human rates/physical-miss roll pass expanded/race/broad; critical Settings foundation passes config tests; critical runtime plumbing passes production-path test; critical arithmetic/roll foundation passes .NET 8 boundary tests; Hero monster critical passes expanded/race; broad only known inspection flake; Human critical passes expanded/race/broad; Hero EnergyShield impact passes focused; Hero shields pass expanded/race/broad; HeroInfo override confirms no saved shield buffs; Reflect gate foundation passes focused; Human Reflect integration passes expanded; Human/chained Reflect passes focused/race/broad; negative-effect Settings pass config tests; negative-effect runtime/CatTongue flag pass focused; next shared debuff RNG/admission and validation. TRIGGER preserves callback/NPCUpdate order. RELOADNPCS drains goods/reloads scripts;
   ordinary quest roots/speech and CALL side effects are tested. CLEARIPBLOCKS
   preserves live admission counts and creation-abuse history. Archive registry,
   connected item/storage/reward and guild-storage authority work; retain global-index gaps.
2. Audit packet, Settings, spell/AI, item-ID allocation and NPC conquest/tax paths.
   Fix confirmed gaps in focused commits with tests; update this plan with evidence.
3. Complete remaining integration acceptance and report its actual limits.
Regression policy: keep the six reproduced baseline timing failures skipped (PoisonCloud, map-hazard restart, mount stale
recovery, Hiding, NPC delayed GOTO, cross-map LoverRecall). Their unchanged-baseline
reproductions and earlier repaired fixtures are recorded in Go status. This is
not an unfiltered-green suite. OmaMage extra-roll transcript flakiness also
reproduces on untouched ccdb06b; it has not been added to the six exclusions.
Inspection packet-26 reproduces 21/100 on 9821e04; timed-recall queue flake
reproduces 10/5000 on 6a61c66; skill timing on 20325e4; YinDevilNode 5/5000 on 498caac. No new skips.
New regressions must be fixed; pre-existing failures do not block package work. Long-running load/crash recovery and production
cutover acceptance remain unproven. No migration-complete claim yet.

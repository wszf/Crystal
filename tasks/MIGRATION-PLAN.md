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
- Useful uncommitted WORLD work adds parser/runtime support for DROP, pets,
  MONGEN/MONCLEAR, buffs, guild membership and REFRESHEFFECTS. It includes
  protocol, persistence, recipient-order and production-session tests. Keep it.
  Constructor/refresh test changes account for the new spawn callers.
- Concrete gaps: Legacy `PlayerObject.CombineItem` and its connection dispatch
  have no Go packet/handler; Go's HornedWarriorShield constant was 56 while
  `Shared/Enums.cs` assigns 55 (56 is HornedCommanderShield); fixed below.
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

Data compatibility and session/item authority precede new reward mutations;
shared buff/spawn behavior precedes NPC and monster consumers. After functional
parity, validate long-running timers, concurrent saves, crash/restart recovery,
operator configuration/logging, and realistic population/load before cutover.
Keep findings in this short plan; add the next concrete package as evidence
arrives, rather than declaring an exhaustive feature percentage.

## Non-goals and blockers

No C# implementation edits, global git configuration changes, new orchestration
framework, speculative server rewrite, or production cutover in these packages.
Retain existing tests even when their historical names mention old milestones.
The isolated 117/0 development snapshot has validated client/gameplay/restart
traces; production parity and load/cutover acceptance remain unproven. Unsupported legacy database versions must remain explicit errors.

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

Recent focused commits:
- `6e8c5d5` / `0fdb8a2`: ordinary NPC quest rebinding and TRIGGER target-session
  callbacks; focused/race and the same filtered all-package checks pass.
- `410703e` / `9c1c19d`: NPC source provenance and core RELOADNPCS goods/script
  lifecycle, open dialogue/queued-call refresh, robot clock and filtered tests.
- `762c36f`: CLEARIPBLOCKS with GM admission, retained connection/creation history,
  production-session/race checks and the same filtered full-suite pass.
- `3e0753d`: public CLEARBUFFS and reverse buff expiry/visibility packet order.
- `06ce788`: rarity colour restoration after tame expiry.
- `199a60d`: unbound Hero registry, inventory and reserved IDs across checkpoints.
- `c62275e`: ADJUSTPKPOINT assignment and target-session persistence.
- `76eac73`: group-recall consent, equipment gates, cooldown and teleport delivery.
- `f8e1acb`: current conquest-file reload with authoritative JSON precedence.
- `5664f4e`: MAPMOVE/GOTO/RECALL with session persistence and race checks.
- `1cf0452`: SETTIMER/SETLIGHT with focused session and race checks.
- `e34ef84`: CLEARMOB with map-cell ordering and shared death lifecycle.
- `24b05cc`: MOB/RECALLMOB, GM-made drops and pet level colours.
- `d122d10` / `d571f72`: MagicInfo export/JSON and Legacy database saves.
- `c80a2ab`: custom spell metadata across runtime, packets, books and account admission.
- `38323f0`: source enum parsing independent of editable spell display names.
- `fdb60cf`: all 109 current spell defaults match FillMagicInfoList.
- `85bd224`: GIVESKILL/DELETESKILL with session persistence and packet quirks.
- `a387a97`: NPC custom skill admission and canonical enum names.
- `0e8465c`: successful projectile-hit skill training, packet/XP persistence
  checks and custom cast thresholds; focused race and filtered full suite pass.
- `c7d1c80`: KILL/DIE direct death, revival/drop authority, GMNeverDie and
  broadcast quirks; session/reload/race and filtered full-suite checks pass.
- `f39e0a2`: CHANGEGENDER/CHANGECLASS identity authority, target-session logout,
  offline handling and parsing; session/reload/race and filtered full suite pass.
- `23e807a`: LEVELHERO spawned-Hero admission, XP preservation, vital/level
  packet order and persistence; focused/session/race and filtered full suite pass.
- `f8cdee6`: DECO runtime objects, actor duplicate spawn, later-entry/movement visibility;
  focused command/session/codec and static visibility race checks pass.
- `c70c481`: AWAKENING/REMOVEAWAKENING atomic equipment mutation without NPC payment,
  source failure/removal quirks, session authority/reload and focused race checks;
  filtered all-package pass after both commands (same six baseline skips).
- `1a2e50a`: version-117 character archives, timestamp overwrite/prefix rules
  and Hero registry references; independent codec/file tests pass.
- `4751b8a`: archive command registry/session/reload and race checks pass; filtered
  full suite passes with the same six skips. Online archived characters still
  need detached authority for later auth-backed mutations (see Go status).
- `98d4b56`: RELOADDROPS live monster/harvest and special tables, source file
  creation and partial-IO behavior; session/race and filtered full suite pass.
- Earlier operator command fixes include GIVEGOLD, LEVEL, CREATEGUILD,
  GIVECREDIT/GIVEPEARLS and SETFLAG/LISTFLAGS/CLEARFLAGS (see Go status).

Continue in this order:
1. KILL/DIE now use direct death lifecycle with revival/drop authority,
   GMNeverDie distinctions and Legacy broadcast counts. Go status records the
   tests and remaining shared death gaps (PK/luck penalties and default-NPC Die).
   CHANGEGENDER/CHANGECLASS now preserve targeting, identity persistence and
   normal logout admission; Go status records offline exception handling and tests.
   Continue tracing remaining reachable operator commands before declaring parity:
   Online archive session binding landed as Go `e0fcd7d`, guild storage as
   `6543b0e`, and NPC guild currency as `3005d96`. `7b0f92d` repairs the live
   GIVEGOLD fixture to preserve account identity. Guild logout is `2d36283`;
   rank definitions/notices and readback use scoped actors in `66e4cb8`.
   Creation/invitation authority landed as Go `6c3154f`.
   Leave/kick/NPC removal is `964b453`; member promotion in `8cd4c95` preserves
   Legacy registry-vs-live target distinctions. Guild buff admission/status delivery
   landed as `5969c5d`; guild XP/member-view continuation landed as `4c5193a`.
   War request/declaration authority landed as `e172742`; territory and recall
   scope landed as `903e36b`. Conquest NPC transactions now use connected actor handles, with focused
   archive/repair/checkpoint tests passing. Palace/member views now project live
   guild membership; inspection follows Legacy registry resolution even for
   online objects (removed target gives no response, restore supplies the row).
   Conquest/inspection race and filtered full suite pass (same six skips).
   Wedding-ring creation/replacement now use connected owner handles; focused
   archive/item/gold/checkpoint tests pass. Marriage/consensual divorce now
   scope both connected actors with pair/checkpoint tests. Forced divorce now
   preserves live owner, registry spouse and recipient authority; focused tests
   pass. Mentorship acceptance now scopes both actors with isolation tests.
   Logout now preserves live actor/recipient state and registry partner XP.
   Next: mentorship settlement/retained XP and rental (see Go status).
   TRIGGER now queues target-session
   default callbacks with NPCUpdate ordering. Core RELOADNPCS now saves/drains
   goods and reloads scripts without changing NPC identities; retain the shared
   ambient-speech and called-script context follow-ups documented in Go status
   (ordinary root quest endpoints are implemented). CLEARIPBLOCKS
   preserves live admission counts and creation-abuse history. Archive registry,
   connected item/storage/reward authority and guild storage
   are implemented; retain the remaining global-index continuation gaps in scope
   before claiming command parity. Audit later switch branches
   as well: STARTWAR, INFO, CLEARQUESTS/SETQUEST, TOGGLETRANSFORM, STARTCONQUEST/
   RESETCONQUEST/GATES, CHANGEFLAG/CHANGEFLAGCOLOUR, REVIVE and their remaining surrounding command branches. This discovery list is not an exhaustive absence claim.
   Revisit shared death gaps only if they block the next operator command.
2. Finish packet, Settings, spell and AI behavior audits, including remaining
   item-ID allocation and NPC conquest/tax price paths. Fix confirmed gaps in
   focused commits with tests and update this plan as evidence changes.
3. Complete remaining integration acceptance and report its actual limits.

Regression policy: the latest all-package checks pass with only six reproduced
baseline timing failures skipped (PoisonCloud, map-hazard restart, mount stale
recovery, Hiding, NPC delayed GOTO, cross-map LoverRecall). Their unchanged-baseline
reproductions and earlier repaired fixtures are recorded in Go status. This is
not an unfiltered-green suite. OmaMage extra-roll transcript flakiness also
reproduces on untouched ccdb06b; it has not been added to the six exclusions.
New regressions must be fixed; pre-existing failures do not block package work. Long-running load/crash recovery and production
cutover acceptance remain unproven. No migration-complete claim yet.

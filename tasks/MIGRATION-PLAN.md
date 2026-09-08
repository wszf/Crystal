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
   Hero rhino/scaly revival passes tests/race; next four AI paths and death drops.
   TRIGGER preserves callback/NPCUpdate order. RELOADNPCS drains goods/reloads scripts;
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
reproduces 10/5000 on 6a61c66. Skill-session timing also reproduces on 20325e4; no new skips.
New regressions must be fixed; pre-existing failures do not block package work. Long-running load/crash recovery and production
cutover acceptance remain unproven. No migration-complete claim yet.

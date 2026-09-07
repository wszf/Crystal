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
- `3e0753d`: public CLEARBUFFS and reverse buff expiry/visibility packet order.
- `06ce788`: rarity colour restoration after tame expiry.
- `199a60d`: unbound Hero registry, inventory and reserved IDs across checkpoints.
- `c62275e`: ADJUSTPKPOINT assignment and target-session persistence.
- Earlier operator command fixes include GIVEGOLD, LEVEL, CREATEGUILD,
  GIVECREDIT/GIVEPEARLS and SETFLAG/LISTFLAGS/CLEARFLAGS (see Go status).

Continue in this order:
1. Finish group-recall command validation (ENABLEGROUPRECALL, GROUPRECALL,
   RECALLMEMBER), including consent, Recall equipment set, shared cooldown,
   target teleport delivery and persisted location.
2. Close the confirmed conquest restart gap: startup imports stale exported
   conquest state while runtime writes current `Conquests/*.mcd` files. Retain
   authoritative JSON precedence and test ownership/balances/structures on reload.
3. Trace remaining reachable commands before declaring parity: KILL, DIE,
   CHANGEGENDER/CHANGECLASS, LEVELHERO, GIVESKILL, MAPMOVE/GOTO/RECALL,
   MOB/CLEARMOB/RECALLMOB, DECO, AWAKENING, archive/backup/load/restore,
   RELOADDROPS/RELOADNPCS, CLEARIPBLOCKS and TRIGGER. Audit later switch branches
   as well; this discovery list is not an exhaustive absence claim.
4. Finish packet, Settings, spell and AI behavior audits, including remaining
   item-ID allocation and NPC conquest/tax price paths. Fix confirmed gaps in
   focused commits with tests and update this plan as evidence changes.
5. Complete remaining integration acceptance and report its actual limits.

Regression policy: the latest all-package checks pass with only six reproduced
baseline timing failures skipped (PoisonCloud, map-hazard restart, mount stale
recovery, Hiding, NPC delayed GOTO, cross-map LoverRecall). Their unchanged-baseline
reproductions and earlier repaired fixtures are recorded in Go status. This is
not an unfiltered-green suite. New regressions must be fixed; those pre-existing
failures do not block package work. Long-running load/crash recovery and production
cutover acceptance remain unproven. No migration-complete claim yet.

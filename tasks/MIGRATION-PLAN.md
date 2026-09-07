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
No representative production dataset/client trace has been validated in this
review; production parity and load/cutover acceptance remain unproven until that
rehearsal. Unsupported legacy database versions must remain explicit errors.

## Execution

- Plan committed first as `1a21e4a4` in Crystal.
- Go `3591b2b` lands reviewed WORLD actions; focused parser, protocol, runtime,
  persistence and production-session tests pass.
- Go `712134e` corrects Warrior/Commander shield IDs to 55/56, separates Blindness
  (57), and tests literal wire IDs, restored visibility and observer lifecycle.
  Affected tests and focused WORLD/buff race checks pass.
- Full regression acceptance remains open: seven initial suite failures reproduce
  on untouched Go baseline `80a2324`, including an intermittent mount transcript.
  Details and reproducible commands: Go `docs/MIGRATION-STATUS.md`.
  Per the current execution instruction, these baseline failures do not block
  packages 3–5 unless new regressions appear. Full regression acceptance stays
  open; no owners invented.

- Package 3 protocol landed as Go `5c87a52`. All four CombineItem branches now
  landed as Go `a5526c8` through current cross-grid item authority; focused session tests cover
  player/Hero mutations and save/reload. Final CombineItem and cross-grid race
  checks pass. Full suite has six known baseline failures and no new failures.

- Package 4 data roots landed as Go `8bf588b`; Robot/monster lifecycle execution
  landed as Go `89daa2d` (focused and race checks pass). Full suite
  reports only six known baseline failures. No automatic-overload action is
  deliberately unsupported; recursion is bounded and logged at depth 32.
- Package 5 uses an isolated 117/0 development dataset snapshot. Go `d8ce4ce`
  fixes map decoding; `4f297dd` fixes relative player/Hero experience paths.
  Export and server binding succeed; the initial client handshake times out
  behind a world tick doing repeated full-population target scans. Fix and
  re-run the rehearsal. `04a7cc6` corrects five baseline test fixtures.

- Package 5 follow-up: `b8ad867` / `349778e` remove repeated target scans/sorts;
  `15e11da` / `8841c77` wire and apply inherited MonsterProcessWhenAlone;
  `86a58f0` indexes monster cells during ticks and centralizes index updates.
  Focused AI/query tests and race checks pass. `f5ddcb6` fixes Hallucination's
  test synchronization (100 repetitions pass). Two more intermittent tests
  (PoisonCloud transcript and map-hazard restart HP) reproduce on unchanged
  `f5ddcb6`; details remain in Go status. Full-data session setup exposed a nil
  synthetic-map assumption, now under regression testing. Package 5 remains open.

- Package 5: `794ce92` fixes exported-world nil-map login; `2c1bb54` configures
  shared world settings before startup; `78d94b9` indexes additional nearby
  consumers; `1358721` keeps production world advancement on its shared ticker.
  `b9e905c` gates asynchronous broadcasts through bootstrap, with mentorship
  ordering corrected in `a0a211b`. `2185ce4` adds explicit live-world probe mode.
  Focused race checks and the full package set with six reproduced baseline
  timing failures skipped pass. Graphical client reaches character selection;
  full-population game entry and live probe still stall. Profile and resolve
  the remaining runtime delay before accepting the client/Legacy rehearsal.

- Package 5 full-population protocol rehearsal now passes. `d319bad` / `1e3cf3c`
  remove profiled route/target scans; `9fcebdf` yields between overdue world
  passes. `f971b26` batches initial bootstrap; `9eae2a5` / `dcd3dda` make the live
  probe respect admission/movement rules. Filtered all-package and focused race
  checks pass. Graphical client still crashes during game entry; isolated Legacy
  comparison is running. Quest/economy/combat and client acceptance remain open.

- Package 5 Legacy comparison found incorrect packet ordinals across 279 Go
  constants. `e8ccef3` removes synthetic insertion shifts and adds a literal
  Shared/Enums.cs fixture. The corrected client now enters BichonProvince on
  both servers; recorded screenshots and dataset hashes are in Go
  `docs/MIGRATION-REHEARSAL.md`. `c250fa3` adds the missing first-login starter
  grants atomically. Shared HumanObject initialization also exposed the Hero
  level-zero/start-item path; `a545a67` fixes initialization and starter grants.
  Focused race and filtered all-package checks pass. Both graphical clients
  complete quest 1 for 10 experience and one potion and unlock quest 2.
  Combat/economy/guild and restart comparisons remain in progress.

- Package 5 command rehearsal: `5e05c7c` / `a7f8ae1` restore GIVEGOLD and LEVEL;
  `8eae9a6` protects guild creation's consumed items and packet order;
  `670f4bf` restores CREATEGUILD. Focused session/reload/race checks pass.
  Ordinary quest checkpoints match; Legacy's authenticated probe, prepared
  guild/economy sequence, clean restart and Hen combat succeed. Go replay and
  broad regression checks are underway. The reachable-command audit has also
  found candidates in operator progression/appearance, monster/group recall,
  and archive/reload commands; trace and close them within package 5 before
  declaring the overall remaining migration complete.

- Package 5: Go `efda36c` fixes fresh NPC shop identity and atomic gold purchases;
  `d3e1f0d` restores monster level-based experience reduction and global rate.
  Focused/race and filtered all-package checks pass. Paired replay remains open.
  The Settings/AI audit confirms missing map-respawn rarity profiles affecting
  stats, rewards and display; close this and the recorded reachable-command
  gaps before completing package 5.

- Package 5: Go `526b1bb` implements map-respawn rarity with focused/race checks.
  Fresh paired gameplay matches level 22, XP 11, gold 930, two potions and quest
  progress. The clean-restart comparison exposes lost Go guild membership:
  zero guild headers and the stale exported seed bypass saved guild files.
  Header/file reload repair and regression checks are in progress. Full suite
  also exposed an operator-ban test deadline race reproduced on `f5ddcb6`;
  its terminal-read assertion was corrected without adding a baseline skip.

- Package 5: Go `161518b` fixes guild checkpoint headers and current-file reload;
  focused/race and the filtered all-package run pass. Live retry is underway.
  The same bridge also drops auction/global GameShop state and unused item-ID
  reservations in legacy-only mode. Extend its runtime snapshot and prevent
  empty current auction lists from resurrecting the old exported seed.

- Package 5: Go `76a89d5`/`95ebe2c` preserve current auction/GameShop state and
  item reservations across Legacy checkpoints; focused/race and filtered
  all-package tests pass. The fresh live guild retry now survives clean
  shutdown/restart/relogin with the matching balances/items/progress and 50
  guild gold. GIVECREDIT/GIVEPEARLS and the remaining reachable-command/Settings
  audit are still in progress; package 5 remains open.

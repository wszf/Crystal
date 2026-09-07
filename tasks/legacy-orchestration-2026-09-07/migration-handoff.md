# Crystal Go migration current handoff

Last updated: 2026-09-05 22:29 UTC (P7 world-action leaf active)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. This continuation uses the user-requested
  `gpt-5.6-luna/max`; `gpt-5.6-sol` is not used.
- Unique Active leaf: `NPC-P7-ACTION-WORLD-001`; the finite P9/P10 closure
  registries and the Ready inputs `GUILD-P9-NPC-SCRIPT-001`,
  `CONQUEST-P9-NPC-ECONOMY-001`, and `MAIL-P10-NPC-SCRIPT-001` were registered
  by Go commit `5c685ac92425f5adb63d7c2b91ee8b24a99fa8e6`. The preceding
  `NPC-P7-ACTION-SOCIAL-001` implementation is Complete at Go commit
  `4d6995113a7f69d74fe591608cec909edb7b63c9`.
- P12 `PERSIST-P12-RESTART-EQUIV-001` remains blocked/shared-owner. No recovery
  owner is invented, no P12 scope is reopened, and no C# is written.
- Read-only `WS-P12-OWNER-CANDIDATE-AUDIT-001` (`gpt-5.6-luna/max`, completed and
  closed) found no independently registerable child. Legacy `Envir.Main` owns the
  combined WorkLoop/shutdown/restart boundary (`Server/MirEnvir/Envir.cs:46-63,
  1996-2004,2147-2155,2199-2204,3294-3301`); MirDB respawn/spawn state crosses
  account loading, and GTMap/guild/conquest startup joins cross stores. Legacy
  loaders read primary files only and expose no `.bak`/`.o`/`.n` restore path.
- The bounded P9/P10 discovery aliases were reconciled to existing authoritative
  rows; no duplicate or broad P9/P10 child was created.

## Legacy repository state

- Root: `/workspace/Crystal`.
- Branch `migration/goal-orchestration`; HEAD
  `de36f1627c1c4ed86027701197614f126b2a0672`.
- Tracked status: `M tasks/migration-active.md`, `M tasks/migration-handoff.md`
  (this snapshot); staged: none; untracked: none. All three tracked/staged/
  untracked `.cs` gates are empty.

## Go repository state

- Root: `/workspace/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `4d6995113a7f69d74fe591608cec909edb7b63c9`.
- Tracked status: `M docs/migration-matrix.md`, `M internal/protocol/packet.go`,
  `M internal/worlddata/npcscript.go`; staged: none; untracked:
  `cmd/crystal-server/Envir/Goods/700.msd`,
  `internal/protocol/object_level_effects.go`,
  `internal/protocol/object_level_effects_test.go`, and
  `internal/worlddata/npcscript_world_actions_test.go` (preserve all four
  artifacts).
- All three tracked/staged/untracked `.cs` gates are empty. No `go` or
  `crystal-server` process is active.
- `Config.Validate` and the production startup seam now reject whitespace-only
  optional paths, pairwise clean-absolute aliases, cross-authority fixed-store
  aliases, and derived runtime-sidecar aliases; the intentional Legacy-account →
  MirADB mapping remains allowed.
- SaveDelay/shutdown account, world, Guild, Goods, Conquest and runtime-sidecar
  callers remain bounded store-order coverage, not complete recovery.

## Active leaf and protected work

- Active leaf: `NPC-P7-ACTION-WORLD-001`.
- Workstream: `WS-P7-WORLD-ACTIONS-GO-IMPLEMENT-001`; Go-only parser/runtime,
  focused production-entry tests, and world-state/recipient/persistence/race
  evidence. Owned scope is `internal/worlddata/npcscript.go`, its focused
  parser test, bounded `cmd/crystal-server/default_npc.go`, a world-action
  helper and production-session tests, plus only directly required Go
  world/auth adapters. All `.cs` files remain read-only.
- The finite slice covers `GIVEPET`, `CLEARPETS`, `REMOVEPET`, `MONGEN`,
  `MONCLEAR`, `GIVEBUFF`, `REMOVEBUFF`, `ADDTOGUILD`, `REMOVEFROMGUILD`,
  `REFRESHEFFECTS`, and `DROP`.
- Legacy trace is captured in `Server/MirObjects/NPC/NPCSegment.cs`: pet
  count/level caps and exact spawn location, shared `PARAM1/2/3` map/monster
  generation, row-major monster clearing, seconds-based buff duration and
  visibility-only AddBuff, guild invite/removal gates, level-effect refresh,
  and configured drop-script/gold/item/capacity/quest gates. This is a
  read-only behavior baseline; no C# change is authorized.
- P9/P10 closure registration is finite and scope-frozen, but their broad child
  implementations remain non-ready or blocked by their recorded evidence gates.
- Existing-target MagicInfo/editor counters/quest filenames are preserved by the
  export merge. New-target authority for those sections, backup/restore,
  manifest, rollback, crash recovery, runtime ObjectID, retry-after-exit and
  complete multi-store recovery remain open/shared-owner questions.
- All Legacy `.cs` files remain permanently read-only.
- Active processes: none. Read-only `WS-P7-SOCIAL-LEGACY-TRACE-001`
  (`gpt-5.6-luna/max`) and the bounded P9/P10 discovery workers are complete
  and closed; the prior P12 owner audit is also closed. No writer agent or
  `go`/`crystal-server` process is active.

## Verification ledger

- The completed social leaf's focused ordinary/repeated/race, NPC/worlddata,
  auth guild/mail/item, parser, vet and build checks passed before this world
  batch. World-action parser changes and its focused test are present but the
  runtime implementation and leaf gate have not yet run in this cycle.
- Full ordinary tests exit 1 on established unrelated baselines: three
  startup/bootstrap expectation tests, `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`,
  and missing temporary `SHARED/FIRST.TXT` in
  `internal/legacyworld.TestDropIncludesExpandLikeGrowingLegacyList`.
- Full race exits 1 on those same baselines plus the existing unrelated shared
  `order` race in `TestSessionGroundItemAndGoldDropPickupBroadcastTranscript`
  (`ground_items_session_test.go:102-107`). Focused social race tests are clean.
- Known full-gate baselines remain the three startup/bootstrap expectation
  failures, `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`,
  missing temporary `SHARED/FIRST.TXT` in
  `internal/legacyworld.TestDropIncludesExpandLikeGrowingLegacyList`, and the
  existing shared `order` race in
  `TestSessionGroundItemAndGoldDropPickupBroadcastTranscript`; preserve their
  attribution if they recur and inspect any new failure.

## Exact recovery sequence

1. Inspect the exact Legacy/Go parser, runner, world, monster, buff, guild and
   drop authorities named by ledger H; preserve the untracked `700.msd` fixture.
2. Implement the finite 11-action world slice and focused production-entry
   regressions in `Crystal.GoServer` only; do not write C# or reopen P12.
3. Run the leaf gate: gofmt owned Go files, touched-package compile, focused
   ordinary/repeated/race tests, diff checks, and both-repository C# audits.
4. Update the P7 matrix/index/handoff only after evidence is green; run
   `tasks/check-migration-control.sh`, then commit owned Go/control files.
   Keep `PERSIST-P12-RESTART-EQUIV-001` blocked/shared-owner.

# Crystal Go migration current handoff

Last updated: 2026-08-29 01:39 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-SHOP-QUIRKS-001` is Complete in Go commit
  `e122f1208a18373fe29f52b66547ad032c1e18a2`. The unique next Active leaf is
  dependency-ready `NPC-P7-TELEPORT-ACTIONS-001`.
- Next matrix scope is row 194, routing ledger P row 216 and P7 summary row 966
  only. P7 remains frozen at 24 children: 13 Complete, 1 Active and 10 Ready.
- Teleport read-only tracers `01a04951-412a-7482-af13-a294f3da4f46` and
  `01a04951-73a3-7da1-9279-18d47f1c0350` are closed; no agent or test process
  remains active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `3625f02618fd4a7fe4168d143a3ec024b38ec0d1`;
  upstream `origin/master`, ahead 507 and behind 0.
- Unstaged owned paths are `tasks/lessons-archive/misc.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or untracked
  paths and no Legacy implementation changes.
- The active index routes only Teleport Actions and names its seven keys. Control
  check and `git diff --check` exit 0. Tracked, staged and untracked C# gates are
  empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `19a2762344475a1fce237e08e09a0e79775d7a7b`;
  no upstream; index and worktree are clean.
- Commit `19a2762 Correct P7 teleport RNG scope` corrects the source-proven RNG
  contract before implementation. No Teleport code has started.
- Matrix row 192/ledger N are Complete, row 194/ledger P are Active, and the P7
  summary is exactly 13+1+10. `git diff --check`, `go mod verify`, and all three
  Go C# gates exit 0.

## Active leaf and protected work

- Active leaf: `NPC-P7-TELEPORT-ACTIONS-001`.
- Outcome: exact seven-key MOVE/instance/time-recall/group teleport parsing and
  effects, including coordinate-less MOVE's one walkable-cell RNG draw despite
  the ignored `200` attempts argument.
- Write authority is new bounded `npc_teleport_actions.go` and tests, minimum
  shared Flow wiring, world/group/rental adapters, transition sessions and
  matrix/index/handoff. No Teleport implementation or tests have started.
- Complete Control Flow, movement, rental, panel routing and Shop Quirks are
  protected. Generic collision/movement, unrelated action/condition catalogs,
  conquest/callback/monster/Robot behavior, protocol layouts and all C# files
  are forbidden.

## Verification ledger

- Legacy parser/runtime minima, early-return behavior, instance `0`/`1` alias,
  inclusive delayed due, captured origin, all-NPC flag cancellation and ordered
  live group membership are frozen with exact source citations.
- Source correction: `TeleportRandom(200,0,map)` ignores both arguments and does
  exactly one `Random.Next(WalkableCells.Count)` selection; empty/cell-null maps
  fail before RNG and the action ignores the teleport result.
- Successful teleport sends source removal and target visibility/effect packets
  before trade/rental cancellation; location fields mutate immediately but save
  occurs through later persistence. Timed recall teleports before delayed page
  execution, and teleport failure does not suppress that page.
- Go has reusable locked transition projection and per-recipient callbacks but
  lacks generic instance/group/time-recall authorities. Existing deterministic
  first-walkable MOVE behavior is known non-equivalent and must be replaced.
- Matrix correction, Legacy `git diff --check`, control check, Go
  `git diff --check`, and both-repository C# gates exit 0. No behavior tests have
  run for this leaf yet.

## Exact recovery sequence

1. Rerun Legacy control/C#/diff gates, commit only the three owned correction
   documents, and verify clean status.
2. Resume only `NPC-P7-TELEPORT-ACTIONS-001` at Go
   `19a2762344475a1fce237e08e09a0e79775d7a7b`.
3. Freeze parser tests first, then implement the bounded action adapter and
   world delayed/group/instance transition authority before authenticated
   session, persistence, relogin and race gates.

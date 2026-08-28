# Crystal Go migration current handoff

Last updated: 2026-08-29 00:56 (Asia/Singapore)

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
- Test writer `01a048d8-317e-7630-abf4-fb8433599a05`, production reviewer
  `01a048e0-9906-71e3-b295-d61c5af13bee` and terminal reviewer
  `01a04904-4cc1-7632-9e35-221b6b62ee42` are closed. Terminal review revision 6
  returned `No findings`; no agent or test process remains active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `b8993facff66520d9394e919468328503e7165ef`;
  upstream `origin/master`, ahead 506 and behind 0.
- Unstaged owned paths are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`,
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`,
  `tasks/lessons-archive/workflow/repository-boundaries-02.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths and no Legacy implementation changes.
- The active index routes only Teleport Actions and names its seven keys. Control
  check and `git diff --check` exit 0. Tracked, staged and untracked C# gates are
  empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `e122f1208a18373fe29f52b66547ad032c1e18a2`;
  no upstream; index and worktree are clean.
- Commit `e122f12 Complete P7 NPC shop quirks` owns the seven bounded Go
  source/test files plus the matrix transition. No Teleport code has started.
- Matrix row 192/ledger N are Complete, row 194/ledger P are Active, and the P7
  summary is exactly 13+1+10. `git diff --check`, `go mod verify`, and all three
  Go C# gates exit 0.

## Active leaf and protected work

- Active leaf: `NPC-P7-TELEPORT-ACTIONS-001`.
- Outcome: exact seven-key MOVE/instance/time-recall/group teleport parsing and
  effects, including coordinate-less MOVE's bounded 200-draw RNG.
- Write authority is new bounded `npc_teleport_actions.go` and tests, minimum
  shared Flow wiring, world/group/rental adapters, transition sessions and
  matrix/index/handoff. No Teleport implementation or tests have started.
- Complete Control Flow, movement, rental, panel routing and Shop Quirks are
  protected. Generic collision/movement, unrelated action/condition catalogs,
  conquest/callback/monster/Robot behavior, protocol layouts and all C# files
  are forbidden.

## Verification ledger

- Stable local `G+U` BUY/BUYSELL and `G` BUYNEW snapshots do not mutate source
  membership across repeated/same/second-user authenticated opens.
- `GoodsOn=false` suppresses only BUYBACK/BUYUSED panels; enabled owner keys,
  strict scheduled expiry, deterministic owner drains and pre-due reads match
  Legacy. Repeated session setup cannot postpone the schedule.
- Atomic runtime state persists UsedGoods, including explicit empty overrides.
  Last-player stop flushes dirty state; graceful shutdown forces remaining
  BuyBack items into public UsedGoods while owner association stays transient.
- Final minimum compile exits 0. Focused/relevant count-10 and race count-3,
  scheduler count-20/race count-5, and runtime schema count-10/race count-3 exit
  0. Fresh `go test -count=1 ./...`, `go vet ./...`, `go build ./...`, and fresh
  `go test -race -count=1 ./...` exit 0 after the final candidate.
- An earlier fresh full test failed only `TestSessionHallucinationTranscript`
  after its known 30-second closed-pipe timeout. Exact isolation count-3 and the
  original fresh full rerun passed; the failed run remains exit 1 and no
  Hallucination file changed.
- Writer's first focused run exposed immediate deletion of the first empty owner.
  Reviewers then exposed missing durable UsedGoods, stop-time flush, scheduled
  cleanup/order, early read-time expiry, forced-save and fixture observability
  gaps. Each was source-corrected; terminal revision 6 reports `No findings`.

## Exact recovery sequence

1. Independently verify both repository HEAD/status/C# gates and Go commit
   `e122f1208a18373fe29f52b66547ad032c1e18a2`.
2. Rerun Legacy control/C#/diff gates, stage only the seven owned Legacy
   documents, commit, and verify clean status.
3. Resume only `NPC-P7-TELEPORT-ACTIONS-001`; read row 194, ledger P row 216 and
   P7 row 966, search matching archive sections,
   then freeze the seven-key Legacy parser/execution checklist before Go edits.

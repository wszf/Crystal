# Crystal Go migration current handoff

Last updated: 2026-08-29 05:06 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `QUEST-P7-REWARD-RATES-001` is accepted Complete in Go commit
  `3bee34a358a0bed47ab0ff659429520f6731803b`. The unique next Active leaf is
  dependency-ready `QUEST-P7-LIFECYCLE-TIMER-001` at matrix row 196, ledger R
  row 218 and P7 summary row 966. P7 is frozen at 24 children: 15 Complete,
  1 Active and 8 Ready.
- Config writer `01a04a12-4991-7880-97d9-958d1a6ce63c` completed and is closed.
  Three bounded terminal reviewers `01a04a20-a2e2-75a0-b8bf-c1747b2edbcd`,
  `01a04a25-f2f2-7790-b11e-258b7d13de6c` and
  `01a04a29-d8e2-7890-9435-39a5872ef8c8` remained unresponsive after interrupt
  and were closed without evidence; main terminal review repaired write-back
  and global-ID findings and found no remaining defect. No agent/test process is
  active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `e6657b8b1c1c40700d2b5020b5cc7ffd8a2667ef`;
  upstream `origin/master`, ahead 510 and behind 0.
- Unstaged owned paths are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`, `tasks/migration-active.md` and this handoff.
  There are no staged/untracked paths or Legacy implementation changes.
- Control/diff and tracked/staged/untracked Legacy C# gates exit 0 before this
  refresh.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `3bee34a358a0bed47ab0ff659429520f6731803b`;
  no upstream; index and worktree are clean.
- Commit `3bee34a` contains exactly the seven owned reward/config/session/matrix
  paths. Tracked/staged/untracked Go-repository C# gates and `git diff --check`
  exit 0.

## Active leaf and protected work

- Active leaf: `QUEST-P7-LIFECYCLE-TIMER-001`.
- Reward Rates preserves Single-aware Setup.ini fallback/write-back and env
  precedence; net8 x64 unchecked Single-to-uint low-word conversion; scaled
  Gold/XP; unscaled Credit/items; global pre-admission reward IDs; exact
  completion/item/currency/level packet order; saturation and JSON restart.
- Lifecycle Timer owns bounded `quests_runtime.go` state/timer transitions,
  minimum main ticker/checkpoint/session wiring and new focused lifecycle tests.
  Complete Quest Core, checkpoint/restart, Reward Rates, item/protocol and NPC
  authorities are protected. Progress quirks and partial carry admission remain
  separate Ready leaves.

## Verification ledger

- Owned gofmt and touched-package compile exit 0. Config and quest/reward
  production tests count-10 and focused reward race count-3 exit 0.
- Authenticated session evidence covers four fixed/selected item stacks, global
  IDs, Gold/Credit saturation, scaled XP level-up with two HealthChanged packets,
  Complete/Change/item/Gold/XP/Health/Level/Credit order and fresh JSON reload.
- One initial focused test exposed a too-short synthetic experience table; the
  corrected `{5,100,100}` fixture passes. One link failed with `no space left on
  device` at 250 MiB free; `go clean -cache -testcache` restored 15 GiB and the
  exact command passed.
- The first integration run failed only archived
  `TestSessionHallucinationTranscript` line 123 closed-pipe behavior. Its first
  isolated count-10 flaked, while exact count-1 and a second count-10 passed;
  subsequent fresh unexcluded full tests passed, latest server 83.629s.
- Latest fresh `go test -count=1 ./...`, `go vet ./...` and `go build ./...`
  exit 0. Focused race is fresh; full race remains fresh from the immediately
  preceding Teleport leaf and is not cadence-due.

## Exact recovery sequence

1. Run Legacy control/diff/status/C# gates and commit only the four owned
   lessons/index/handoff paths; independently verify both worktrees and HEADs.
2. Resume only `QUEST-P7-LIFECYCLE-TIMER-001`: search targeted archived lessons,
   then trace the exact Legacy quest New/timer/completion/expiry call chain and
   bounded Go ledger-R surfaces before any code write.
3. Freeze every completion producer and strict 24-hour/order/restart checklist,
   then delegate one bounded read-only/implementation wave and execute its leaf
   gate.

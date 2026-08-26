# Crystal Go migration current handoff

Last updated: 2026-08-26 17:18 (Asia/Singapore)

This replace-in-place snapshot closes the real compaction recovery gate received
immediately after the Regen leaf's terminal gates/review and route to HP drain.
The prior 15:50 snapshot is preserved once at
`tasks/migration-handoff-archive/2026-08-26-1704-pre-hp-drain-route.md`
(162 lines/9525 bytes, SHA-256
`0d959d540af4af640d47275f00567fa3b7393a68c6200a0cb4a9d16ce493ef95`).
No implementation or behavior test ran during this reconstruction. The first
status call mixed both roots and is discarded in full; every repository fact
below comes from later independent repository-local zero-exit calls.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active and unchanged;
  it is neither Complete nor Blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- `REGEN-P5-HUMAN-001` is committed in Go as
  `194c209b46f84876c577085c94b5b3983178691a`. Its matching Legacy
  control/evidence update is the only pending documentation commit.
- The unique Active Leaf is `COMBAT-P5-HP-DRAIN-001`. P5 is scope-frozen with
  fifteen children: ten Complete, this leaf Active and four Ready.
- Normal recovery may read only Go matrix P5 summary row 851, HP-drain registry
  row 3164 and completed Regen evidence 3184-3199.
- `tasks/migration-active.md` consistently routes HP drain in both the Active
  batch and P5 registry and records the Regen commit.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `139d5b4cf41aaa1b37f9e1f8bcc9b5acc993d452`;
  upstream is `origin/master`, local branch ahead by 476.
- Index/staged set is empty. The six unstaged tracked files are exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/verification/race-and-flake-attribution.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Excluding this self-changing handoff, the other five tracked diffs have
  SHA-256 `ef65613a4238d6284f75321f1a1e822dce854c2961e15cb6aaffbf9189a8242e`.
- The five untracked current-batch archive snapshots are exactly:
  - `tasks/migration-handoff-archive/2026-08-26-1032-pre-regen-review.md`
    (116 lines/6109 bytes; SHA-256
    `097776898fcad1d4648fc027cbc95eb7247195cfffe933d786230200a1b51c3c`)
  - `tasks/migration-handoff-archive/2026-08-26-1424-pre-review-fixes.md`
    (154 lines/9084 bytes; SHA-256
    `c49dcee5341aadd1e62d7db1b8bab7e4bafaabf767bb7537193c0e37d332e183`)
  - `tasks/migration-handoff-archive/2026-08-26-1519-pre-full-race-fix.md`
    (152 lines/8926 bytes; SHA-256
    `7b104d722571e96f2de032f40556c6dd4c6ac1929790b5646898f282db960321`)
  - `tasks/migration-handoff-archive/2026-08-26-1704-pre-hp-drain-route.md`
    (metadata and hash above)
  - `tasks/migration-handoff-archive/2026-08-26-regen-76-file-recovery.md`
    (156 lines/8970 bytes; SHA-256
    `6284b28ca3030d4093969f58fc1fc1a41cbfcb6c1d5034dff0b6e5601652d33f`)
- Repository-local lock search and exact `go`/`crystal-server` process audit
  were empty. All three Legacy `.cs` gates were empty and exited 0.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `194c209b46f84876c577085c94b5b3983178691a`.
- Worktree and index are clean: no tracked, staged or untracked files.
- Commit `194c209` contains 79 files, 2413 insertions/364 deletions and the
  matrix route/evidence. Its staged `git diff --check` and staged `.cs` gate
  exited 0 before commit.
- Post-commit repository-local status is clean. All three Go `.cs` gates were
  empty before commit; rerun them in the next verification step.

## Active leaf and protected work

- Active leaf: `COMBAT-P5-HP-DRAIN-001`.
- HP-drain Go write authority remains closed. Trace every Legacy declaration,
  add/read/payout/reset callsite first, then replace the closed-authority sentence
  in `tasks/migration-active.md` with an exact existing/new file set.
- Freeze attacker/target kinds, damageWeapon and defence admission, float32/int
  order, strict `>2`, floor/remainder/cap loss, packets/persistence and runtime
  reset before implementation.
- Completed Regen commit `194c209b46f84876c577085c94b5b3983178691a`
  is read-only while tracing HP drain.
- SafeZoneHealing, long-Revelation expiry, unrelated combat refactors, another
  P5 child and every `.cs` write remain forbidden.

## Verification ledger

- Regen terminal review `01a03d12-8a8f-7390-aaec-e95710f71379` found Hero
  poison/order and shallow relationship snapshots. Follow-up
  `01a03d3b-74be-75c3-af27-ec151385358f` found TownRevive's two remaining
  shallow overwrite sites, then returned `No findings` after all runtime and
  transition projections were deep-detached.
- Focused Regen/relationship/TownRevive tests passed at count 20 and focused race
  count 5 after the final fixes. Their exact regex argv is preserved only in the
  pre-compaction execution record, not reconstructed here; matrix/lesson evidence
  records the zero exits, and the exact fresh unexcluded gates below cover the
  current fingerprint.
- Two post-review `go test -race ./... -count=1 -timeout=60m` attempts exited 1
  only at pre-existing `TestSessionOmaMageRangeSlowFrozenTranscript` RNG
  `[2 1]` versus `[1]`, with no race-detector or Regen stack. Exact isolated race
  count 10 and the explicitly excluded remainder passed; neither is called a
  full-race pass.
- Final current-fingerprint `go test ./... -count=1` exited 0.
- Final current-fingerprint `go vet ./...` exited 0.
- Final current-fingerprint `go build ./...` exited 0.
- Final fresh unexcluded `go test -race ./... -count=1 -timeout=60m` exited 0;
  `cmd/crystal-server` completed in 86.418s.
- No test ran after the matrix/index route-only edits; those edits do not change
  Go behavior. No command above is being rerun during this compaction gate.

## Exact recovery sequence

1. In Legacy independently run `tasks/check-migration-control.sh`, `git
   diff --check`, status and all three `.cs` queries. In Go independently run
   `git diff --check`, status and all three `.cs` queries; read only matrix rows
   851, 3164 and 3184-3199. Verify the stated hashes before adopting this state.
2. Commit the matching Legacy lessons, active index, archives and current
   handoff as one documentation/control batch. This handoff intentionally
   records Legacy HEAD immediately before that one expected commit delta.
3. Resume `COMBAT-P5-HP-DRAIN-001` with read-only Legacy/Go tracing. Search the
   archive only for `COMBAT-P5-HP-DRAIN-001`, `HpDrain`, attacked, float,
   remainder and transcript/race keywords. Freeze exact Go write authority
   before any implementation, then delegate bounded independent tracing/review
   only to `luna_worker`.

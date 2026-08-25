# Crystal Go migration current handoff

Last updated: 2026-08-25 17:29 (Asia/Singapore)

This replace-in-place snapshot records the completed P6 shout-arming child and
routes the persistent Goal to the shared inventory-grid mutation child.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- P6 remains scope-frozen In progress: eight of nineteen children are Complete,
  `ITEM-P6-GRID-MUTATION-001` is Active and ten are Ready.
- `ITEM-P6-SHOUT-ARMING-001` is Complete in Go commit
  `1d399992a690614a122cc46b3e64b4cda8272c2f`; matrix-only commit
  `2d41a659ee3f3b1d6075c9f401f94db4e28a4dba` made the review-proven shared
  revision/CAS residual explicit in the next child.
- Reviewer `01a0382a-60e1-7b02-97a5-f1345ba94376` rejected the first shout
  candidate, rejected a nested world→auth callback correction, then accepted
  the final bounded candidate with no findings. It wrote nothing and ran no
  tests; the thread is closed.
- No `go` or `crystal-server` process remains.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD before the expected control commit:
  `7a38c3d43f2611bc8aa74436653c59b0967f4191`
  (`Refresh P6 shout recovery handoff`).
- Tracked unstaged files before this refresh are `tasks/lessons.md`,
  `tasks/lessons-archive/migration/world-state-lifecycle.md` and
  `tasks/migration-active.md`; this handoff becomes the fourth expected file.
  Staged and untracked sets are empty.
- Active lessons record the discarded mixed-repository/glob/path/zero-match
  calls. The targeted archive section records why a bool auth callback under
  `world.mu` is not an atomicity fix. Control checker, diff check and all three
  Legacy C# gates passed.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `2d41a659ee3f3b1d6075c9f401f94db4e28a4dba`
  (`Refine P6 grid mutation authority`).
- Tracked unstaged, staged and untracked sets are empty.
- Matrix P6 summary is 8/19 Complete; the exact shout row is Complete and the
  exact grid-mutation row is Active. All three Go C# gates are empty.

## Active leaf and protected work

- Active leaf: `ITEM-P6-GRID-MUTATION-001`.
- Outcome: preserve inventory-only Move/Split/Merge/Delete success and failure
  behavior, including null-source Move's localized System chat, report/log and
  failure response order. Add normalized auth/world revision/CAS convergence
  without a nested cross-domain lock.
- Matrix anchors are only the P6 summary, exact active row and completed shout
  evidence paragraph that registered the shared transaction residual.
- Legacy read authority is bounded to the four `PlayerObject` methods, matching
  `MirConnection` dispatch, `Player.Reporting` item methods and exact
  localization key. Every C# file is read-only.
- Exact Go write authority is the bounded files listed in
  `tasks/migration-active.md`: item transaction/session/world/runtime logging,
  auth item-commit code/tests and at most two new focused test files.
- Storage/Trade/Refine/Hero/equipment/fishing grid expansion, another P6 child,
  P4 shout routing and unrelated refactors remain forbidden.

## Verification ledger

- Final shout compile: `go test ./cmd/crystal-server -run '^$'` exit 0.
- Final focused repetition: four shout tests at `-count=20` exit 0.
- Final focused race: the same four tests under `go test -race ... -count=5`
  exit 0. Related existing Use/Delete tests plus the four shout tests at
  `-count=1` also exited 0.
- Final Go `gofmt`, `git diff --check`, exact status/process review and all three
  Go C# gates passed before commits; Go worktree is clean afterward.
- Integration/full-race was not due for the bounded shout leaf. It is explicitly
  due for the newly active shared locking/persistence leaf.
- Legacy control checker, `git diff --check`, status/process review and all
  three Legacy C# gates passed before this refresh.

## Exact recovery sequence

1. Run the Legacy control checker and separately verify both repository
   status/HEAD/process/C# gates; commit exactly the four owned Legacy control/
   lesson files and verify both repositories clean.
2. Read only the named active matrix anchors, the exact Legacy methods and the
   existing targeted auth/world item-transaction lesson section.
3. Derive the finite invalid/success ordering matrix and a no-nested-lock
   normalized revision/CAS design before code writes; then use one bounded Luna
   wave with disjoint read-only tracing/review or test-only authority.

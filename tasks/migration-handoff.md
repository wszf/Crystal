# Crystal Go migration current handoff

Last updated: 2026-08-25 16:27 (Asia/Singapore)

This replace-in-place snapshot records the accepted P6 scope freeze and the
route to the first dependency-ready P6 child.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- `DISC-P6-CLOSURE` is Complete. The matrix and Active Index contain one
  accepted nineteen-child P6 denominator: seven Complete and twelve Ready.
- Independent Legacy auditor `01a037e1-30fa-7840-860a-237521bf558e` and Go
  auditor `01a037e1-67d2-7013-bf45-313d11b10b4e` wrote no files. Reviewer
  `01a037ed-f35d-7d23-a532-803fdce5a5ff` required death-drop, expanded-storage,
  cross-grid, UseItem-shape and expiry corrections, then returned
  `accepted, no findings`; all three threads are closed.
- `ITEM-P6-SHOUT-ARMING-001` is the unique Active leaf because it can unblock
  the remaining P4 chat child without entering P4 routing authority.
- No `go` or `crystal-server` process remained at the documentation gate.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD before the expected control commit:
  `133f5164c142ac7e03f2018f85e687fc8c850f0a`.
- Tracked unstaged files are `tasks/lessons.md`,
  `tasks/migration-active.md`, and this handoff. The staged and untracked sets
  are empty.
- `tasks/lessons.md` records the discarded cross-repository P6 search and its
  successful separate reruns. All three Legacy C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `45e532f3703ece18f1f45392c0aa959c61b21b2e`
  (`Freeze P6 migration inventory`).
- Tracked unstaged, staged and untracked sets are empty.
- Matrix P6 is scope-frozen In progress at 7/19 children Complete, records
  `DISC-P6-CLOSURE` Complete and routes the twelve finite unfinished children.
- All three Go-repository C# gates are empty.

## Active leaf and protected work

- Active leaf: `ITEM-P6-SHOUT-ARMING-001`.
- Outcome: preserve Scroll shape 8/9 common admission, transient one-shot
  map/server-shout arming, localized Hint, item consumption and successful
  `UseItem` order. Re-arming consumes another scroll; logout drops both flags.
- Matrix anchors are only the P6 summary row, this exact P6 child row and the
  exact `CHAT-P4-MAP-CONTEXT-001` dependency row.
- Go write authority is bounded to `cmd/crystal-server/main.go`, `world.go`, new
  `item_shout.go`, `item_shout_test.go`, and `item_shout_session_test.go`.
- P4 owns shout consumption, cooldown, linked-item projection, routing and
  recipients. Every other P6 child, accepted registry and `.cs` file is read-only.

## Verification ledger

- Discovery performed no production/test-code change, so no Go compile/test,
  race, vet or build was run for this documentation-only leaf.
- Two independent inventories and the final two-round read-only denominator
  review completed; reviewers wrote no files and ran no tests/builds.
- Go `git diff --check`, exact status, process review and all three Go C# gates:
  exit 0/empty before matrix commit.
- Go matrix commit `45e532f` succeeded and the Go worktree is clean.
- Legacy `git diff --check`, `tasks/check-migration-control.sh`, exact
  three-file status, process review and all six cross-repository C# gates:
  exit 0/empty after this handoff refresh.
- Full tests/full race are not due for a documentation-only scope freeze; the
  selected functional child retains focused compile/repetition/race gates.

## Exact recovery sequence

1. Run the Legacy control checker, diff/status/process review and both
   repositories' six C# gates; commit exactly the three owned Legacy control
   files and verify both repositories clean.
2. Read only the named matrix/Legacy/Go anchors for
   `ITEM-P6-SHOUT-ARMING-001`, then implement its bounded item-side state and
   authenticated tests without entering P4 chat routing.

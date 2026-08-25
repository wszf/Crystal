# Crystal Go migration current handoff

Last updated: 2026-08-25 18:53 (Asia/Singapore)

This replace-in-place snapshot closes the reviewed P6 grid-mutation candidate
and routes the persistent Goal to shared item-use admission. The Goal remains
Active; this is not a phase closure or project completion boundary.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `gpt-5.6-luna/max` through `luna_worker`.
- P6 is scope-frozen In progress: nine of nineteen children are Complete,
  `ITEM-P6-USE-ADMISSION-001` is Active and nine are Ready.
- `ITEM-P6-GRID-MUTATION-001` is Complete in Go commit
  `fccbfcd91a36d867d48974c7d14b43e092c7a33d`.
- All completed reviewer/worker threads are closed. No `go` or
  `crystal-server` process remains.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD:
  `5abeef47bdedf8a3162d61fcb26bc02e33f49e17`
  (`Close P6 shout leaf and route grid mutations`).
- Tracked unstaged files are exactly `tasks/lessons.md`,
  `tasks/lessons-archive/migration/world-state-lifecycle.md`,
  `tasks/migration-active.md` and `tasks/migration-handoff.md`.
- Staged and untracked sets are empty. All three Legacy C# gates are empty.
- The active lesson records two corrected API-signature guesses; the targeted
  archive section records the Use -> Delete resurrection and latest-auth rule.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `fccbfcd91a36d867d48974c7d14b43e092c7a33d`
  (`Complete P6 inventory grid mutations`).
- Tracked unstaged, staged and untracked sets are empty.
- The matrix marks grid mutation Complete, use admission Active and P6 at 9/19
  Complete. All three Go C# gates and `git diff --check` are empty/passing.

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-ADMISSION-001`.
- Outcome: migrate shared `CanUseItem` gender/class, every RequiredType stat/
  level/rebirth gate and common item/grid/death admission, with exact localized
  System text before `UseItem=false` and no consumption/effect on denial.
- Read only the P6 summary, exact active row and completed grid evidence
  paragraph. Legacy read/write and exact Go write boundaries are registered in
  `tasks/migration-active.md`; every C# file remains read-only.
- The accepted grid commit authority, normalized revision/CAS world apply,
  runtime item reports and inventory-only Move/Split/Merge/Delete behavior are
  protected. Do not redesign them in admission work.
- Item-family effects, use catalog expansion, equipment movement/stat formulas,
  another P6 child, P4 chat routing and unrelated refactors remain forbidden.

## Completed grid-mutation evidence

- Auth mutators receive detached latest item state, commit only deep-cloned
  grids, capture rental transfer proposals, reserve globally unique split IDs
  including rental authority IDs and publish global process-monotonic revisions.
- World applies returned normalized snapshots only after auth unlock, clones all
  grids and rejects delayed revisions without a nested world-to-auth lock.
- Session auth/world/configured JSON commit precedes Legacy-ordered Player logs
  and packets. Null-source Move is report -> localized System chat -> failure;
  successful Move/Split/Merge/Delete preserve their exact report/wire order.
- A focused regression proved basic potion consumption now commits against the
  latest auth snapshot and cannot resurrect or erase a concurrent item before a
  later Delete.
- Reviewer `01a0385f-7607-7be3-9541-8759a864ab1c` rejected the initial auth
  authority. Reviewer `01a0387f-52d9-7d62-8803-340b6f3f53b8` found the stale
  potion write and accepted its correction with no remaining blocker.

## Verification ledger

- Touched-package compile for `./internal/auth` and `./cmd/crystal-server`:
  exit 0.
- Focused auth/world/helper/authenticated/failure tests at `-count=20`: exit 0.
- The same concurrency-relevant set under race at `-count=5`: exit 0.
- Fresh unexcluded `go test ./...`: exit 0.
- Fresh `go vet ./...` and `go build ./...`: exit 0.
- Fresh unexcluded `go test -race ./...`: exit 0.
- Earlier failures were implementation-attributed and resolved: wrong
  `RentalInformation` type, wrong `ParseChatPayload` arity and Use -> Delete
  stale-auth resurrection. Final focused/package/full gates all pass.

## Exact recovery sequence

1. Run the Legacy control checker, commit exactly the four owned Legacy files
   and verify both roots plus all six C# gates.
2. Read only the active admission matrix anchors and bounded Legacy `CanUseItem`/
   common UseItem admission code. Derive the finite precedence/text matrix before
   the next Go write, then use one bounded Luna wave with disjoint authority.

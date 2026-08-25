# Crystal Go migration current handoff

Last updated: 2026-08-26 00:04 (Asia/Singapore)

This replace-in-place snapshot closes the accepted item-use-admission leaf and
routes the same active Goal to the next dependency-ready P6 child. No phase or
project closure is claimed.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `gpt-5.6-luna/max` through `luna_worker` without silent substitution.
- P6 is scope-frozen In progress: ten of nineteen children are Complete,
  `ADMIN-P6-ITEM-COMMAND-001` is the unique Active leaf and eight are Ready.
- Go matrix anchors are the P6 summary row near 901, exact Active row near 3192
  and immediately preceding item-admission completion evidence near 3243-3261.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed pre-documentation-commit HEAD:
  `0e672087925fede6f756be14e81e45b3e56747ad`
  (`Close P6 grid mutations and route item admission`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/world-state-lifecycle.md`
  - `tasks/lessons-archive/verification/build-vet-and-gates.md`
  - `tasks/lessons-archive/verification/race-and-flake-attribution.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - this handoff
- Staged and untracked sets are empty. One expected owned Legacy documentation
  commit will archive the completed leaf evidence and route the next leaf.
- All three Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `f6e9f2ba69d3a1cb0e7d37536e726c3e2a666cd2`
  (`Complete P6 item use admission`).
- Tracked, staged and untracked sets are empty after the owned 24-file commit.
- All three Go `.cs` gates are empty.

## Active leaf and protected work

- Active leaf: `ADMIN-P6-ITEM-COMMAND-001`.
- Outcome: preserve administrator/TestServer `@MAKE` item index/name lookup,
  ushort count fallback, stack splitting, `GMMade`, early-return/full-bag quirks
  and observable text/report behavior; preserve self/online-target `@CLEARBAG`
  per-slot `DeleteItem` ordering and final stat refresh.
- Legacy authority is bounded to `PlayerObject` command dispatch and the exact
  `MAKE`/`CLEARBAG` cases near lines 2362-2434, `Envir.CreateDropItem` near
  4403-4427 and only directly invoked lookup/gain/refresh/packet helpers.
- Go write authority is bounded to `cmd/crystal-server/{utility_command.go,
  utility_command_test.go,utility_command_session_test.go,main.go,
  item_transactions.go,item_transactions_test.go}` plus one focused new test if
  needed, and `internal/auth/{service.go,service_test.go}` only if latest-state
  atomic create/clear persistence requires it.
- Completed item admission commit `f6e9f2ba69d3a1cb0e7d37536e726c3e2a666cd2`
  and all other files are protected from redesign or unrelated cleanup.

## Verification ledger

- Post-correction `gofmt` and
  `go test ./cmd/crystal-server ./internal/auth -run '^$'` exited 0.
- The combined admission/family/latest-auth/Hero/pet focused set passed at
  `-count=20`; the same set passed under race at `-count=5`.
- Fresh unexcluded `go test ./... -count=1`, `go vet ./...`, `go build ./...`
  and `go test -race ./... -count=1` all exited 0 after final corrections.
- Reviewer `01a0398c-2192-7710-baba-e3c5548d5d1a` rejected stale sealed-Hero
  full-snapshot commit and post-auth ordinary-pet rollback. Production now uses
  one revisioned `AttachSealedHeroLatest` transaction and retains a pet after
  authoritative item consumption; focused/full gates passed and re-review
  returned `no findings` / ACCEPT. Its allocator-gap observation was ruled
  intentional by the existing Legacy-compatible `AllocateItemID` contract.
- The earlier read-only reviewer `01a03979-3ff3-7980-967f-e51afec821b8` was
  shut down after failing to return; it changed no files. All reviewer threads
  are closed and no `go` or `crystal-server` process remains.

## Exact recovery sequence

1. Commit the six owned Legacy control/evidence files, then verify both
   repositories separately, all six `.cs` gates and active-index/matrix parity.
2. Search matching archive sections only for `ADMIN-P6-ITEM-COMMAND-001`,
   `MAKE`, `CLEARBAG`, `CreateDropItem`, command parser and item allocator.
3. Trace exact Legacy parser/permission/count/stack/full-bag/target order and
   inspect only current Go command/item authorities; register any bounded write
   expansion before implementation.
4. Implement the finite delta, run compile/focused `-count=20`/race `-count=5`
   and any due integration gate, obtain bounded Luna review, update matrix,
   index and this snapshot, then commit only accepted owned files.

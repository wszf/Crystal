# Crystal Go migration current handoff

Last updated: 2026-08-26 00:45 (Asia/Singapore)

This replace-in-place snapshot preserves the uncommitted implementation state of
the unique active P6 administrator-item-command leaf. No leaf, phase, or project
closure is claimed.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `gpt-5.6-luna/max` through `luna_worker` without silent substitution.
- P6 remains scope-frozen In progress: ten of nineteen children are Complete,
  `ADMIN-P6-ITEM-COMMAND-001` is the unique Active leaf and eight are Ready.
- Exact Go matrix anchors remain the P6 summary row at 901, Active registry row
  at 3192 and completed item-admission evidence at 3243-3261. They agree with
  the active index; the matrix itself is unchanged.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD:
  `18be855c3fe2f70caeb3672a9e2a99d93a3bea1d`
  (`Close P6 item admission and route admin item commands`).
- Tracked unstaged files are exactly `tasks/lessons.md`,
  `tasks/migration-active.md`, and this handoff. The lesson records the repeated
  mixed-repository status-audit hazard; the active-index delta registers the
  seven-file Go implementation authority below.
- Staged and untracked sets are empty.
- Tracked, staged and untracked Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `f6e9f2ba69d3a1cb0e7d37536e726c3e2a666cd2`
  (`Complete P6 item use admission`).
- Tracked unstaged files are exactly:
  - `cmd/crystal-server/drop_items.go`
  - `cmd/crystal-server/main.go`
  - `internal/auth/service.go`
  - `internal/auth/service_test.go`
- Untracked files are exactly:
  - `cmd/crystal-server/admin_item_commands.go`
  - `cmd/crystal-server/admin_item_commands_session_test.go`
  - `cmd/crystal-server/admin_item_commands_test.go`
- Staged set is empty. `git diff --check` exits 0. Tracked, staged and untracked
  Go `.cs` gates are empty.
- The current delta adds bounded `@MAKE`/`@CLEARBAG` dispatch, parser/stack
  helpers, a world-owned random-item template helper, an auth-owned atomic
  inventory clear and focused parser/authenticated-session/auth tests. Preserve
  all seven files exactly until they are reviewed and verified; do not reset,
  stash, clean, checkout, overwrite, or discard them.

## Active leaf and protected work

- Active leaf: `ADMIN-P6-ITEM-COMMAND-001`.
- Outcome remains Legacy-equivalent administrator/TestServer `@MAKE` numeric or
  case-insensitive name lookup, ushort fallback, CreateDropItem randomness,
  stack splitting, `GMMade`, item-ID/full-bag/early-return text and report order;
  and self/online-target `@CLEARBAG` slot-order deletions followed by the target
  stat refresh with persistence/relogin equivalence.
- Legacy read authority remains the exact `PlayerObject` command cases,
  `Envir.CreateDropItem`, direct definition/gain/lookup/refresh helpers and
  resulting packets/text/reports. Every C# file remains read-only.
- Go write authority is exactly the seven current changed files above, plus only
  the still-untouched registered files `drop_items_test.go`,
  `item_transactions.go`, `item_transactions_test.go`, and
  `utility_command_session_test.go` if the finite behavior checklist proves a
  need. No protocol/config schema or unrelated command expansion is authorized.
- Unresolved rulings before acceptance: review allocator consumption and random
  template timing on full-bag paths; verify latest-state mutation/writeback and
  target disconnect races; prove zero/overflow count text/log behavior,
  permission and missing-target silence; prove exact packet/stat-refresh order,
  persistence and relogin. The interrupted read-only audit produced no accepted
  report, so none of these may be assumed closed.
- Completed commit `f6e9f2ba69d3a1cb0e7d37536e726c3e2a666cd2`
  and all files outside registered authority remain protected.

## Verification ledger

- No compile, focused test, race, persistence/relogin transcript or integration
  result for the current seven-file uncommitted leaf is trusted or claimed by
  this snapshot. The recovered context did not contain a complete command/exit
  ledger, so verification must restart from the minimum touched-package compile.
- Current non-test audit: separate per-repository branch/HEAD/full status and all
  six `.cs` commands exited 0; Go `git diff --check` exited 0.
- No `go test`, `go build`, `go vet`, or repository `crystal-server` process was
  active at the snapshot. Read-only auditor
  `01a039b7-e455-7743-8628-fff48b70e252` was closed while running and returned no
  review result; it had no write authority. No subagent remains active.
- The fresh full test/vet/build/full-race passes recorded for the committed base
  belong to the preceding leaf only and do not validate this uncommitted delta.

## Exact recovery sequence

1. Read this snapshot and `tasks/migration-active.md`, then separately verify
   each repository's HEAD, exact status and three `.cs` gates without combining
   repositories in one shell command.
2. In the Go repository, inspect the seven-file delta and run
   `go test ./cmd/crystal-server ./internal/auth -run '^$'` first. Fix only
   current-leaf compile defects; preserve unrelated work.
3. Finish the bounded Legacy/Go behavior review for the unresolved rulings, then
   run exhaustive permission/parser/count/stack/full-bag/target tests at
   `-count=20`, authenticated persistence/relogin transcripts, and focused race
   at `-count=5`; run any integration gate due from shared persistence changes.
4. Obtain a fresh bounded Luna review, integrate accepted findings, update the
   matrix/index/handoff as one status transaction, run control and all six C#
   gates, then commit only the accepted owned files in their respective repos.

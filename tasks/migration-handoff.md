# Crystal Go migration current handoff

Last updated: 2026-08-26 02:57 (Asia/Singapore)

This replace-in-place snapshot is the durable compaction recovery point after
the accepted administrator-item-command commits. It routes the same active Goal
to the finite P6 mining leaf and claims no phase or project closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `gpt-5.6-luna/max` through `luna_worker` without silent substitution.
- P6 is scope-frozen In progress: eleven of nineteen children are Complete,
  `MINE-P6-RUBBLE-001` is the unique Active leaf and seven are Ready.
- Go matrix anchors are P6 summary row 901, Active registry row 3206 and
  completed administrator-item evidence 3263-3282. Matrix/index parity holds.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed pre-handoff-refresh HEAD:
  `bd0e0319e6907df90dbed565872a04bf375f6d53`
  (`Record P6 admin item closure and route mining`).
- The worktree and index were clean immediately before this handoff refresh.
  The expected next Legacy commit contains only this handoff; recovery must
  compare the recorded pre-commit HEAD with actual HEAD/status.
- Tracked, staged and untracked Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `9e7edac7aaf22f35677f90427ca79da4e998b096`
  (`Complete P6 administrator item commands`).
- Worktree and index are clean. Tracked, staged and untracked Go `.cs` gates are
  empty. Preserve this commit and every earlier completed migration commit.

## Completed administrator-item evidence

- `ADMIN-P6-ITEM-COMMAND-001` is Complete at Go commit `9e7edac`.
- Production preserves literal-space tokenization, raw numeric/name/count
  parsing, ordered lookup, stack/zero/full quirks, GMMade creation, ID/RNG
  consumption, rejected no-commit/no-save behavior, sibling definitions before
  primary/GainedItem and connection-local cache suppression.
- CLEARBAG resolves self by ObjectID, routes remote work through the target
  session, uses a clear-specific auth revision watermark, retains slot deletion
  and stat order, survives graceful/abrupt logout and relogin, and does not
  misclassify ordinary dirty craft/equip/NPC item revisions as a clear.
- Targeted read-only Luna audits
  `01a039fb-a032-7c60-98ed-fe9155cbd350` and
  `01a039fb-c22b-7ca0-a585-96de7cfdb055` supplied final identity/revision and
  raw-token/sibling rulings. Three later no-write terminal-review attempts
  (`01a03a1f-d36b-7df2-ab5b-8a57afecb4bc`,
  `01a03a31-3705-7341-9ac4-bfea0eca5f48`, and
  `01a03a3b-c7cf-7f11-8533-c4b6d5ffbd4c`) remained running without a report
  despite bounded conclude requests and were closed; no conclusion from them is
  claimed. No writer or reviewer remains active.

## Verification ledger

- Touched compile `go test ./cmd/crystal-server ./internal/auth -run '^$'`
  exited 0 after the final code.
- The thirteen administrator/parser/auth/session/revision tests passed with
  `go test ./cmd/crystal-server ./internal/auth -run '<focused-regex>'
  -count=20` and the same `go test -race ... -count=5`, both exit 0.
- Craft success/failure, equipment, NPC item persistence, clear logout and
  ordinary-revision dirty-world regressions passed together at `-count=20`.
- Final fresh unexcluded gates after the clear-watermark correction all exit 0:
  - `go test ./... -count=1` (`cmd/crystal-server` 78.880s)
  - `go vet ./...`
  - `go build ./...`
  - `go test -race ./... -count=1` (`cmd/crystal-server` 84.555s)
- Attributed corrections are archived: guessed refresh result type caused one
  compile failure; unconditional auth rebase caused Craft/equipment/NPC full-
  test failures; it was replaced by the clear-specific watermark. One later
  full package run hit a Hallucination closed-pipe failure whose isolated rerun
  passed; subsequent fresh full normal and race gates passed. An overlength
  synthetic account ID blocked a new fixture before admission and was corrected
  to a production-valid identifier.
- `git diff --check`, gofmt-diff, exact Go status/process review and all Go `.cs`
  gates were clean immediately before commit. No `go` or `crystal-server`
  process remains.

## Active leaf and protected work

- Active leaf: `MINE-P6-RUBBLE-001`.
- The interrupted tracing produced no repository changes, no accepted behavior
  ruling and no frozen write set. No Go implementation is uncommitted and no
  Mine/Rubble file is yet owned.

## Exact recovery sequence

- Resume by verifying both repositories separately and reading only matrix rows
  901, 3206 and 3263-3282.
- Search the lessons archive only for `MINE-P6-RUBBLE-001`, Mine, Rubble,
  mining RNG/timer and parser/export keywords. Trace bounded Legacy settings,
  map mine state, request/action, Rubble and direct helpers.
- Before any Go write, freeze exact existing/new filenames and the finite
  config/schema/RNG/400ms/five-minute/regen/payout/session/restart checklist in
  the active index. Then delegate at most one bounded wave with disjoint writes.

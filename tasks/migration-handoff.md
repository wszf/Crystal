# Crystal Go migration current handoff

Last updated: 2026-08-26 19:40 (Asia/Singapore)

This replace-in-place snapshot records the completed Revelation-expiry leaf and
routes the same active Goal to the bounded global Revelation health-refresh
ledger. It is the current recovery authority, not a historical journal.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; it is neither
  Complete nor Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers
  must use `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- Active leaf: `STATE-P5-REVELATION-REFRESH-002`.
- P5 is scope-frozen with twelve of sixteen children Complete, this leaf Active
  and three Ready. `SPELL-P5-REVELATION-EXPIRE-001` is Complete in Go commit
  `a2cd1cc3768eae92b69e839e14446f2debd9249f`.
- Normal recovery reads only Go matrix P5 row 851, registry rows 3166-3167,
  completed Revelation evidence 3198-3211 and refresh ruling 3212-3220.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `560f2f28442aa72a75d93d3101bfd8f8738a0217`;
  upstream `origin/master`, ahead 479 and behind 0.
- Complete current status is six unstaged tracked files:
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`,
  `tasks/lessons-archive/workflow/repository-boundaries-02.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/lessons.md`, `tasks/migration-active.md`, and this handoff. Staged and
  untracked sets are empty.
- The first five non-handoff files preserve Revelation discovery/routing and
  workflow/fixture corrections. Every `.cs` file remains read-only.
- The handoff/control commit is expected to advance Legacy HEAD by one
  documentation-only commit from the observed pre-commit HEAD above.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `a2cd1cc3768eae92b69e839e14446f2debd9249f`;
  no upstream is configured.
- Worktree and index are clean: no tracked, staged or untracked paths.
- Commit `a2cd1cc` preserves the 255/256/260 expiration wrap, strict direct
  SendHealth routing, active/expired broadcast fanout, observer suppression,
  group-join expiration sharing, static/bootstrap/transition ordering and the
  authenticated configured-260-SC transcript.

## Active leaf and protected work

- Active leaf: `STATE-P5-REVELATION-REFRESH-002`
- Discovery outcome: generate a Go-AST ledger for all current non-test
  `HealthChangedPayload` emitter files and direct Monster-like HP mutation
  candidates; classify each against Legacy `BroadcastHealthChange` behavior,
  recipients and packet order before opening production writes.
- Go write authority is currently only new
  `cmd/crystal-server/revelation_refresh_ledger_test.go` plus bounded matrix
  evidence. No production Go file is writable until the exact subset and file
  list are frozen in the active index.
- Completed Revelation expiry, HP drain, Human regen, SafeZoneHealing, another
  P5 child and every `.cs` file are protected.

## Verification ledger

- Revelation touched compile: `go test ./cmd/crystal-server -run '^$'` exit 0.
- Focused repeated gate: `go test ./cmd/crystal-server -run
  '^(TestRevelation|TestGameWorldGroupJoin|TestMapTransitionUsesViewerRelativePlayerProjection|TestTownReviveUsesViewerRelativePlayerProjection|TestSessionConfiguredHighSCRevelationPreservesWrappedExpire|TestPlayerNaturalRegenUsesActiveRevelationBroadcast)'
  -count=20 -timeout=10m` exit 0.
- Focused race gate: the same regex with `go test -race`, `-count=5` and
  `-timeout=15m` exit 0.
- Fresh integration: `go test ./... -count=1 -timeout=30m` exit 0; command
  package completed in 79.802s. `go vet ./...` and `go build ./...` exit 0.
- Full race was not due for this bounded leaf; focused race passed. The previous
  accepted full-race baseline remains the HP-drain closure at Go `0db2cde`.
- Two terminal Luna source-review attempts were closed after bounded finish and
  interrupt requests without reports, so they contribute no evidence. The main
  terminal source/diff review found no in-scope issue. All agents are closed;
  an exact `comm` process audit found no `go` or `crystal-server` process.
- Both repositories' tracked, staged and untracked `.cs` gates are empty.

## Exact recovery sequence

1. Verify each repository independently against the branches, HEADs and complete
   statuses above; rerun all six `.cs` gates.
2. Read only matrix rows/sections named above. Search the lesson archive only for
   `STATE-P5-REVELATION-REFRESH-002|BroadcastHealthChange|HealthChangedPayload|HP mutation|Go AST` matches.
3. Recompute the current 23-emitter/45-candidate denominator with a Go-only AST
   ledger in the one allowed new test file. Pair candidates to bounded Legacy
   callsites, then freeze exact production/test write authority before any
   production edit.

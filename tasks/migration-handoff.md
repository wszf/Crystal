# Crystal Go migration current handoff

Last updated: 2026-08-26 20:31 (Asia/Singapore)

This replace-in-place snapshot records the in-progress bounded Revelation
health-refresh ledger after recovery reconciliation. It is the current recovery
authority, not a historical journal.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; it is neither
  Complete nor Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers
  must use `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- Active leaf: `STATE-P5-REVELATION-REFRESH-002`.
- P5 is scope-frozen with twelve of sixteen children Complete, this leaf Active
  and three Ready. `SPELL-P5-REVELATION-EXPIRE-001` is Complete in Go commit
  `a2cd1cc3768eae92b69e839e14446f2debd9249f`.
- Normal recovery reads only Go matrix P5 row 851, registry rows 3166-3167,
  completed Revelation evidence 3198-3211 and refresh ruling 3212-3229.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `ea352c147f34e22536da9741929b55613c88ada3`;
  upstream `origin/master`; the route commit is already present.
- Complete current status before this self-changing handoff is four unstaged
  tracked files: `tasks/lessons-archive/workflow/repository-boundaries-02.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/lessons.md`, and `tasks/migration-active.md`. Staged and untracked sets
  are empty.
- The Active Index now agrees with the matrix: Revelation expiry is Complete,
  refresh is Active, and P5 has twelve Complete plus three Ready children.
  Every `.cs` file remains read-only.
- The next Legacy documentation commit is expected to advance the observed HEAD
  above by one authority-freeze commit.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `ee8ec67da749445c5772a127b807c441ce4d6892`;
  no upstream is configured.
- Worktree and index are clean. Commit `ee8ec67` freezes the passing 23/48
  Go-AST classification, corrects the incomplete textual 45-file baseline and
  records the exact production denominator in the matrix.

## Active leaf and protected work

- Active leaf: `STATE-P5-REVELATION-REFRESH-002`
- Discovery is Complete. The ledger classifies every 23 health-emitter and 48
  typed Monster-HP file. Human evidence has 34 live gaps, three follow-ups (two
  group-only) and two valid fallback-only sites; Monster evidence has 30 live
  producers, 15 constructor/reset/death and three other-authority files.
- Production write authority is now frozen to the exact 38 files plus new
  helper/test and ledger/matrix paths listed in `tasks/migration-active.md`.
  `revelation.go` remains protected and supplies routing consumers.
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
- Static ledger gate `go test ./cmd/crystal-server -run
  '^TestRevelationRefreshStaticCandidateLedger$' -count=1` exits 0 in 2.168s;
  the same exact test under `go test -race` exits 0 in 9.951s. Touched package
  compile `go test ./cmd/crystal-server -run '^$'` exits 0.
- Legacy auditor `01a03dde-e799-7553-a875-fb27645662b5` completed and was
  closed after returning the twelve exact executable broadcast callsites and
  recipient/order rules. Go auditors `01a03deb-5b23-7830-acf9-200c80357be9`,
  `01a03dfb-b20e-7560-8225-ca3614917030` and
  `01a03dfb-ddd9-7591-a089-02901ed67205` closed after the complete 23/48 file
  and 39-Human-callsite classification. No subagent remains active.

## Exact recovery sequence

1. Verify each repository independently against the branches, HEADs and complete
   statuses above; rerun all six `.cs` gates.
2. Read only matrix rows/sections named above. The relevant archive search for
   `STATE-P5-REVELATION-REFRESH-002|BroadcastHealthChange|HealthChangedPayload|HP mutation|Go AST` currently has no match.
3. Implement the new shared helper/test first without changing protected
   `revelation.go`; then split the exact Human and Monster producer files into
   at most two disjoint Luna writer scopes, preserving `HealthChanged` then
   `ObjectHealth` and lethal `ObjectDied` then `ObjectHealth` order.

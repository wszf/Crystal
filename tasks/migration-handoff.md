# Crystal Go migration current handoff

Last updated: 2026-08-27 01:17 (Asia/Singapore)

This replace-in-place snapshot records the committed scheduled SafeZoneHealing
leaf and routes the next bounded direct-movement trace. It is the current
recovery authority, not a historical journal.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; it is neither
  Complete nor Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers
  use `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- Active leaf: `REGEN-P5-SAFEZONE-DIRECT-002`.
- P5 is scope-frozen with fourteen of eighteen children Complete, this leaf
  Active and three Ready. `REGEN-P5-SAFEZONE-001` is committed Complete in Go
  `0ab4da7b091d131bdd99a0ff153b7981438262ea`.
- Normal recovery reads only matrix P5 row 851, registry rows 3168-3170 and
  completed SafeZone evidence 3255-3277.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `6754cdd653b61ad8d28bc49c3ad8f9f9751f46e5`; upstream `origin/master`.
- Unstaged tracked documentation consists of this handoff, the next-leaf active
  index, `tasks/lessons.md`, and four matched archive files:
  `migration/data-config-import-export.md`,
  `migration/world-state-lifecycle.md`,
  `verification/race-and-flake-attribution.md`, and
  `workflow/shell-tools-and-patching.md`.
- Staged and untracked sets are empty. Every tracked, staged and untracked `.cs`
  gate is empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `0ab4da7b091d131bdd99a0ff153b7981438262ea`;
  no upstream is configured.
- Worktree, index and untracked set are clean. Commit `0ab4da7` contains config,
  exported-world startup, invisible field runtime, exact Monster regen override
  behavior, deterministic/authenticated tests, matrix completion and next-leaf
  selection. Every tracked, staged and untracked `.cs` gate is empty.

## Active leaf and protected work

- Active leaf: `REGEN-P5-SAFEZONE-DIRECT-002`.
- Initial authority is read-only Legacy/Go tracing plus control/matrix routing.
  Before production writes, freeze an exact subset from the registered central
  and specialized movement ledger and name exact focused/session tests.
- Required behavior is direct Healing `ProcessSpell(target)` after successful
  turn/walk/run/push paths, bypassing field schedule/location; Run evaluates
  traversed cells after final movement. Preserve invocation/failure and packet
  order.
- Scheduled SafeZoneHealing commit `0ab4da7b091d131bdd99a0ff153b7981438262ea`,
  SafeZoneBorder, other P5 children and every `.cs` file are protected.

## Verification ledger

- Owned Go files are gofmt-clean. Final touched compile
  `go test ./cmd/crystal-server ./internal/config -run '^$'` exits 0.
- Final config focused command selects `^TestSafeZoneHealing`, count 20; exit 0.
- Final command focused selection covers every SafeZone/session test plus exact
  HealingCircle, HumanWizard, Trainer and DragonStatue regressions, count 20;
  exit 0.
- The same config and command selections under `go test -race`, count 5, exit 0.
- Fresh final integration `go test ./... -count=1 -timeout=30m` exits 0;
  command package 81.592s. `go vet ./...` and `go build ./...` exit 0.
- Final fresh `go test -race ./... -count=1 -timeout=45m` exits 1 only at the
  known unrelated `TestSessionKirinIceThrustTranscript` fixture race. Exact
  `go test -race ./cmd/crystal-server -run '^TestSessionKirinIceThrustTranscript$' -count=3 -timeout=10m`
  also exits 1: the test writes `monsterAIRoll` and its callback while its own
  session ticker reads them. No stack enters this leaf, and neither run is
  reported as a full-race pass.
- Writer `01a03eec-2120-7480-a78e-8146337af091` supplied the bounded session
  test. Reviewer `01a03f02-1e55-78e2-80af-795b462f26d9` found the accepted
  zero-MaxHP edge; its isolated inclusive-tick/direct-map findings were rejected
  from the outer scheduler and definition authority. Both threads are closed.
- Exact process audit finds no `go` or `crystal-server`; both repositories' six
  `.cs` gates are empty.

## Exact recovery sequence

1. Verify each repository independently against the branches, HEADs and complete
   statuses above; rerun all six `.cs` gates.
2. Run `git diff --check` and `tasks/check-migration-control.sh` in Legacy, stage
   only the seven documentation files listed above, commit the scheduled-leaf
   routing/evidence, read back the full hash, and verify both repositories clean.
3. For `REGEN-P5-SAFEZONE-DIRECT-002`, read only the named matrix anchors and
   search the lessons archive with `SafeZoneHealing|ProcessSpell|Turn|Walk|Run|Pushed`.
4. Trace bounded Legacy direct movement call chains and reconcile exact Go
   declarations; update the active index with frozen file/test authority before
   any production code write, then run the leaf gates.

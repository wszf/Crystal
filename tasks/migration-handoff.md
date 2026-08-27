# Crystal Go migration current handoff

Last updated: 2026-08-27 13:01 (Asia/Singapore)

This replace-in-place snapshot closes the uncommitted
`MONSTER-P5-BASE-FAMILY-001` batch and routes the next P5 leaf. It supersedes
the stale pre-terminal-review snapshot.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; workers are `luna_worker`.
- P5 is scope-frozen with seventeen of nineteen children Complete, natural
  Monster regen Active and ordinary spawn Ready.
- Matrix anchors are P5 summary row 851, registry rows 3173-3175 and completed
  BaseFamily evidence 3192-3218; never read the full matrix during recovery.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `971248f95fd56c031e5f02ce7a724937fe2820f1`; upstream `origin/master`, ahead
  486 at observation.
- Unstaged tracked files: `tasks/lessons.md`, `tasks/migration-active.md`, and
  this handoff. Untracked file:
  `tasks/lessons-archive/migration/monster-base-family.md`. Index is empty.
- `tasks/check-migration-control.sh` exits 0. Tracked, staged and untracked `.cs`
  gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; observed HEAD
  `3818544beea374ab47ee96bc59b9514dd1a1b476`; no upstream.
- Worktree and index are clean after commit `3818544`; no untracked files.
- Tracked, staged and untracked `.cs` gates are empty.

## Active leaf and protected work

- Active leaf: `REGEN-P5-MONSTER-NATURAL-001`.
- Exact 46-ordinal dispatch, inherited target/search/stack/roam/move/melee,
  Player/owned-Monster/Hero gates, delayed dead-attacker impact, AI=252 death,
  Hero Hiding lifecycle, movement cell order and unit-bound combat are closed.
- Constructor and tick/time respawn consume CoolEye/direction/regen/search/roam
  before same-stream cell selection, including `Next(1)`.
- The complete BaseFamily batch is committed as Go
  `3818544beea374ab47ee96bc59b9514dd1a1b476`.
- The default-false and true runtime `MonsterProcessWhenAlone` seam is present;
  registered `CFG-P1-MONSTER-AI-RUNTIME-001` owns production INI wiring.
- Reviewer `01a04147-0a85-7190-bbc4-f6f4affabd7e` is closed. It accepted the
  common SafeZone wrapper and corrected respawn ordering; its remaining config
  wiring note is already owned by the registered P1 leaf.

## Verification ledger

- Touched compile `go test ./cmd/crystal-server -run '^$'`: exit 0.
- Focused/relevant repeated tests `-count=10`: exit 0.
- Focused/relevant race tests `-race -count=3`: exit 0.
- First fresh `go test -count=1 ./...`: exit 1 only for the pre-existing Player
  `TestSessionHidingTranscriptPersistenceAndExpiry` closed-pipe timeout.
  Exact count-1/count-10 and fresh `go test -count=1 ./cmd/crystal-server` then
  exited 0.
- Final fresh `go test -count=1 ./...`: exit 0.
- Final fresh `go test -race -count=1 ./...`: exit 0.
- `go vet ./...` and `go build ./...`: exit 0.
- No active subagent and no `go`, `crystal-server` or `git` process remains.

## Exact recovery sequence

- `REGEN-P5-MONSTER-NATURAL-001` owns new
  `cmd/crystal-server/monster_natural_regeneration.go`, bounded world fields/
  constructor/tick wiring, convergence with `healing_circle.go`, new focused
  tests and only trace-proved poison reset adapters.
- Commit the current Legacy control documents, then resume by searching the
  lesson archive for natural Monster regen and tracing the
  bounded override/exclusion set, and freezing exact reset adapters before new
  production writes.

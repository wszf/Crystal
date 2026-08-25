# Crystal Go migration current handoff

Last updated: 2026-08-25 08:48 (Asia/Singapore)

This replace-in-place snapshot records committed closure of `MAP-P4-LOAD-001`
and synchronized routing to `BOOT-P4-STARTPOINT-001`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`. Bounded writer
  `01a03650-017e-79e1-aa3a-b0ed9e1ab20e` (`luna_worker`,
  `gpt-5.6-luna/max`) owned the two test files, returned passing focused
  evidence and was closed. No agent or Go/crystal-server process remains active.
- P4 is scope-frozen In progress with ten children, three Complete and seven
  unfinished. Matrix and Active Index both mark `MAP-P4-LOAD-001` Complete and
  `BOOT-P4-STARTPOINT-001` as the sole Active leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `b1b85fcc5bdc2a8ef290da1ad710ed49e3d1d3dc`
  (`docs(migration): refresh p4 handoff`).
- Tracked unstaged: `tasks/lessons.md`, `tasks/migration-active.md`, and this
  handoff. Staged and untracked sets are empty.
- Lessons contains bounded C01/C02 strengthening for the discarded mixed-repo
  recovery call and discarded unquoted-glob lookup. Active Index closes MAP and
  registers exact BOOT ownership in `main.go` plus new
  `p4_startpoint_startup_test.go`.
- Control checker, diff check and Legacy tracked/staged/untracked `.cs` gates
  exit 0/empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `7cceb308cef7232a1b02c64c301adc1f89275e9a`
  (`feat(migration): preserve legacy map load continuation`).
- Worktree, index and untracked set are empty; diff and C# gates are clean.
- The committed loader attempts every exported map in metadata order, prefers the
  Legacy `.map` path, does not synthesize/promote/require a primary, continues
  missing/read/corrupt failures, loads only successful metadata/rules/
  StartPoints, emits exception-if-present -> localized title -> filename, and
  reports metadata-count `MapsLoaded` before listener bind.
- The same commit marks MAP Complete, BOOT Active, P4 three Complete/seven
  unfinished and records focused/integration evidence. No Go/server process
  remains.

## Active leaf and protected work

- Active leaf: `BOOT-P4-STARTPOINT-001`.
- Outcome: reject exported-world startup unless a successfully loaded map owns
  at least one `SafeZoneInfo.StartPoint`, with exact localized failure before
  any game/status listener bind.
- Matrix anchors: exact BOOT inventory row, P4 stage row and named StartPoint/
  startup evidence only. Legacy authority: first `CanStartEnvir`
  `StartPoints.Count` check, successful-map population, caller message ->
  `StopEnvir`/`Stop` order and `CannotStartServerWithoutMapAndStartPoint`.
- Exact next Go write authority is `cmd/crystal-server/main.go`, new
  `cmd/crystal-server/p4_startpoint_startup_test.go` and bounded P4 matrix
  evidence. P12 `EnforceDBChecks`, P5-P8 behavior and every C# write are
  forbidden.

## Verification ledger

All listed commands use the final MAP candidate and exit 0:

- `go test ./cmd/crystal-server -run '^$'`
- focused eight loader/startup tests `-count=1` and `-count=20`
- focused eight-test `go test -race ... -count=5`
- `go test ./internal/mapdata -count=1`
- `go test -race ./internal/mapdata -count=1`
- `go test ./cmd/crystal-server -count=1` (75.342s)
- fresh unexcluded `go test ./... -count=1`
- `go vet ./...`
- `go build ./...`
- final `gofmt`, both diff checks, control checker, process check and all six C#
  gates are clean/empty. Full repository race is not due for this bounded,
  non-concurrent leaf; focused race is current.

## Exact recovery sequence

1. Re-read this handoff and compare both status sets; Go must remain clean and
   Legacy must contain only the three control files described above.
2. Commit the three Legacy control files, then verify both repositories clean
   and matrix/index/handoff Active fields agree.
3. Begin `BOOT-P4-STARTPOINT-001` under its exact two-file Go authority.

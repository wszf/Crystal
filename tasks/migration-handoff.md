# Crystal Go migration current handoff

Last updated: 2026-08-27 16:25 (Asia/Singapore)

This replace-in-place snapshot closes the committed specialized-constructor
leaf and routes the finite dynamic-constructor tracing substep. It supersedes
the pre-implementation specialized-constructor snapshot.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; workers use `luna_worker`.
- P5 is scope-frozen with nineteen of twenty-one children Complete,
  dynamic-constructor Active and ordinary-spawn Ready.
- Matrix anchors are P5 summary row 851, registry rows 3175-3177 and completed
  specialized-constructor evidence 3211-3229; never read the full matrix during
  recovery.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD
  `4ef4c605603b674bbdf2ba998dc3f249d47d0a29`; upstream `origin/master`, ahead
  489 at observation.
- Owned modifications are `tasks/migration-active.md`, this handoff,
  `tasks/lessons-archive/migration/monster-natural-regeneration.md` and
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`; index is
  empty and there are no untracked files.
- The archive updates record the FrostTiger/fixed-field review findings,
  warrior-Regen fixture correction and established Hallucination full-gate
  attribution. Tracked, staged and untracked `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; clean HEAD
  `8c16d9e2a8d479091c43ab379bb090a09a0ef946`; no upstream.
- Commit `8c16d9e` completes specialized factory construction and selects
  `MONSTER-P5-DYNAMIC-CONSTRUCTOR-003`; index and worktree are empty.
- Tracked, staged and untracked `.cs` gates are empty.

## Active leaf and protected work

- Active leaf: `MONSTER-P5-DYNAMIC-CONSTRUCTOR-003`.
- Specialized construction is protected at Go `8c16d9e`: 210 non-base factory
  ordinals, inherited `100/8/10000/3000/1000`, five concrete RNG families,
  fixed flags/deadlines, twenty-one direction overrides, Spawned direction and
  initial/respawn cell order are accepted.
- The first bounded Go query found 34 non-test `materializeMonster` callsites in
  30 creator files. Production writes are not open: classify each Legacy/Go
  creator as dynamic child, top-level cross-phase spawn or ordinary AI=60-63/99,
  then replace the Active Index authority gate with an exact accepted file list.
- Preserve dynamic creator-specific target capture, direction, action/search/
  lifetime overrides and registration order. Do not reopen factory processors,
  natural Regen, BaseFamily or ordinary-spawn behavior.
- Read-only reviewers `01a04225-5e4e-7cf2-97be-f72df46b7632` and
  `01a0423d-585b-70c1-b962-91407cb1c6f0` are closed; the correction pass ended
  with no remaining finding and zero writes.

## Verification ledger

- Touched compile `go test ./cmd/crystal-server -run '^$'`: exit 0.
- Constructor/affected focused tests `-count=10`: exit 0; focused race
  `-race -count=3`: exit 0.
- Server package `go test ./cmd/crystal-server -count=1`: exit 0 after the
  failure-proved direction and warrior-Regen fixture corrections.
- First final `go test -count=1 ./...`: exit 1 only at the established
  `TestSessionHallucinationTranscript` 30-second closed-pipe flake; exact
  `-count=10` exited 0 and the fresh unexcluded full rerun exited 0.
- Fresh `go test -race -count=1 ./...`, `go vet ./...` and `go build ./...`:
  exit 0. `git diff --check` and all `.cs` gates exit 0.
- No subagent remains open; Go was clean immediately after commit.

## Exact recovery sequence

- Run `tasks/check-migration-control.sh`, read back this handoff and the Active
  Index, verify both repositories independently, then commit only the four
  owned Legacy documentation files.
- Search the lessons archive for dynamic constructor/creator/RNG/target/timer
  keywords. Use one bounded read-only `luna_worker` wave to freeze the 34-callsite
  Legacy/Go partition and override order; main must update exact write authority
  before any dynamic-constructor production edit.
- Keep direct ordinary AI=60-63/99 map-spawn behavior in
  `MONSTER-P5-ORDINARY-SPAWN-001` Ready.

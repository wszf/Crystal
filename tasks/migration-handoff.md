# Crystal Go migration current handoff

Last updated: 2026-08-27 14:46 (Asia/Singapore)

This replace-in-place snapshot closes the committed natural-Monster-regeneration
leaf and routes the finite specialized-constructor follow-up. It supersedes the
stale pre-natural-regeneration snapshot.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; workers use `luna_worker`.
- P5 is scope-frozen with eighteen of twenty children Complete, specialized
  constructor Active and ordinary spawn Ready.
- Matrix anchors are P5 summary row 851, registry rows 3173-3176 and completed
  natural-Regen evidence 3193-3209; never read the full matrix during recovery.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `5daee95fd05eddc9bdbc4813beb2c9d8cdc5d924`; upstream `origin/master`, ahead
  488 at observation.
- Before this handoff write, `tasks/migration-active.md` was modified and
  `tasks/lessons-archive/migration/monster-natural-regeneration.md` was
  untracked. This handoff is now the third owned modified file. The expected
  next state is one control-document commit after the observed HEAD.
- `tasks/check-migration-control.sh` exited 0 before the final handoff refresh.
  Tracked, staged and untracked `.cs` gates were empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD
  `fc0c66cc9de88db30c752bf1a430e58764605804`; no upstream.
- Worktree and index are clean after commit `fc0c66c`; no untracked files.
- Tracked, staged and untracked `.cs` gates are empty.

## Active leaf and protected work

- Active leaf: `MONSTER-P5-SPECIALIZED-CONSTRUCTOR-002`.
- Natural Monster regeneration is closed and committed as Go `fc0c66c`: every
  constructor consumes the inherited `100/8/10000/3000/1000` prefix before
  concrete RNG and cell selection; natural/pool aggregation, reset ordering,
  exclusions and authenticated transcript evidence are accepted.
- The specialized leaf owns only bounded constructor/RNG/projection helpers in
  `cmd/crystal-server/world.go`, one new static/focused/session test file and
  failure-proved expected-direction fixture corrections.
- Freeze the complete non-base factory partition, inherited/overridden fields
  and concrete constructor RNG suffix before production writes. Do not reopen
  processors, natural regen, BaseFamily, ordinary spawn or config wiring.

## Verification ledger

- Touched compile `go test ./cmd/crystal-server -run '^$'`: exit 0.
- Natural/affected focused tests `-count=10`: exit 0.
- Natural/affected focused race tests `-race -count=3`: exit 0.
- Fresh `go test -count=1 ./...`: exit 0.
- Fresh `go test -race -count=1 ./...`: exit 0.
- `go vet ./...` and `go build ./...`: exit 0.
- `git diff --check`: exit 0 before commit; `gofmt` covered every owned Go file.
- No `go`, `crystal-server` or `git` process remained at snapshot time. No
  active subagent is registered in this main thread.

## Exact recovery sequence

- Run the Legacy control checker after this replacement, read back this file,
  then commit the three owned Legacy control/archive files.
- Resume only `MONSTER-P5-SPECIALIZED-CONSTRUCTOR-002`: use the already-read
  natural/BaseFamily archive evidence, enumerate the bounded factory and direct
  constructor overrides, and freeze the finite ledger before editing `world.go`.

# Crystal Go migration current handoff

Last updated: 2026-08-24 00:20 (Asia/Singapore)

This is the replace-in-place current snapshot. The superseded 23:31 uncommitted
recovery snapshot was preserved once as
`tasks/migration-handoff-archive/2026-08-23-2331-pre-utility-commit.md` and is not
startup-read authority.

## Goal and control-plane state

The persistent Goal remains the 100% Legacy-observable-behavior migration to the
independent pure-Go repository. It is not Complete or externally Blocked. The
platform currently reports the Goal `paused` after an earlier explicit turn
abort; the user's current instruction authorizes work in this thread. Do not
recreate the Goal or treat duration, token use, compaction count, or open scope
as a blocker.

A real compaction signal arrived immediately after the utility leaf had passed
its gates and been committed in Go, but before the Legacy active index and
handoff were closed. Implementation, tests, and discovery stopped. The compact
summary was treated only as a gate trigger; this snapshot was rebuilt from
separate repository observations plus the durable command results in the
current rollout record.

Current model authority is main `gpt-5.6-sol/ultra`; bounded independent workers
use `luna_worker` (`gpt-5.6-luna/max`).

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 416]`
- Observed HEAD before the enclosing documentation commit:
  `69d471835bf8b84a0137e883938a5a4cfc53c51c`
  (`69d47183 docs(migration): bound goal control plane`).
- Expected enclosing commit subject: `docs(migration): close utility command
  handoff`. Recovery must compare the actual HEAD/status; this one documentation
  commit delta is expected and should leave the repository clean.
- No staged files at snapshot creation. Tracked modifications were exactly:
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- The untracked file was exactly:
  - `tasks/migration-handoff-archive/2026-08-23-2331-pre-utility-commit.md`
- `AGENTS.md` and tracked `agents.md` remain the same hard-linked inode.
- `git diff --check`, the bounded control checker, and all tracked/staged/
  untracked Legacy `.cs` gates must be zero/empty in the verification below.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`
- HEAD: `5bce7c28c162296d27b61db60897d1b23a80c91e`
  (`5bce7c2 feat(p3): restore public utility commands`)
- Worktree is clean: no tracked, staged, or untracked changes.
- The commit atomically contains the fifteen former protected paths, including
  the bounded `internal/legacyfmt` package and the authoritative matrix update.
- Tracked, staged, and untracked Go-repository `.cs` gates are empty.

## Active leaf and protected work

- `OPS-P1-P3-P4-UTILITY-001` is `Complete` at Go commit `5bce7c28`.
- It restores ordinary Game-stage `@TIME`, `@ROLL`, and `@MAP`; current-culture
  command casing and formatting; reachable loaded-template FormatException
  shutdown; group/system/source and observer delivery ordering; and
  authenticated transcript coverage.
- Matrix P1/P3/P4 remain In progress and open; P8 remains Complete and frozen.
- Active leaf: `DISC-P1-CLOSURE` (`Active` for routing, with no discovery command
  run since selection at this compaction boundary).
- Its scope is phase-local finite inventory, not an up-front inventory of all
  phases. It has no functional Go ownership; its bounded documentation authority
  and acceptance checklist are in `tasks/migration-active.md`.

## Verification ledger

All commands below ran against the exact candidate later committed; no source or
matrix edit occurred between the final gates and staging/commit.

- `go test ./cmd/crystal-server -run '^$' && go test
  ./cmd/crystal-server -run
  '^TestUtilityCommandFormatFailureStopsListenerAndDisconnectsAllSessions$'
  -count=20 -timeout=120s` — exit 0.
- Owned-file `gofmt`, package compile, config/formatter/utility focused tests at
  `-count=10` — combined command exit 0.
- Focused race tests for config, formatter, utility transcript/lifecycle, and
  observer snapshot at `-count=3` — combined command exit 0.
- `go test ./... -count=1 -timeout=900s` — exit 0, unexcluded.
- `go vet ./... && go build ./...` — combined command exit 0.
- `go test -race ./... -skip
  '^TestSessionOmaMageRangeSlowFrozenTranscript$' -count=1 -timeout=900s` — exit
  0, explicitly excluded and therefore not an unexcluded full-race pass.
- Existing out-of-scope baseline
  `TestSessionOmaMageRangeSlowFrozenTranscript`: `-count=1 -v` exited 0;
  `-count=50` and a later `-count=10 -timeout=120s` each exited 1 with random
  bounds `[2 1]`, expected `[1]`. The ordinary unexcluded full run later passed.
  This flaky P5 evidence is not attributable to the utility commit and remains
  open for its own finite leaf; the exclusion must never be described as a full
  race pass.
- Before commit, owned-file gofmt diff, `git diff --check`, exact staged/
  unstaged/untracked review, process audit, and all Go `.cs` gates were clean.
  Post-commit HEAD/status and `.cs` gates were clean as well.

## Quiescence

- The formatter writer and both final read-only reviewers were closed before the
  compaction boundary; the final redundant close found no work to collect.
- The Codex writer-lock directory contains only this main Goal thread and its
  coordination lock.
- Exact `comm` audit currently finds no `go` or `crystal-server` process.
- No implementation, test, or P1 discovery command has run after the compaction
  signal.

## Exact recovery sequence

1. In the Legacy root, re-read this snapshot and `tasks/migration-active.md`;
   verify HEAD/status, `git diff --check`, hard-link identity, control limits,
   and all three `.cs` gates. Preserve the four current documentation paths.
2. In a separate Go-root call, verify HEAD `5bce7c28`, clean status, matrix P1
   row anchor, exact process quiescence, and all three `.cs` gates.
3. Commit only the four Legacy documentation paths with a concise migration
   control/handoff subject after reviewing their exact diff.
4. Resume `DISC-P1-CLOSURE` without production writes. Search only the P1 row,
   its three named residual phrases, directly linked P1 headings, relevant
   Legacy/Go declarations, and targeted archive matches.
5. Materialize stable finite P1 child leaves with ownership and acceptance
   evidence. Scope-freeze P1 only if the bounded closure audit proves no unnamed
   residual remains; otherwise register a finite follow-up discovery child.
6. Select one dependency-ready P1 child, refresh this snapshot, and begin that
   independently testable leaf in the same Goal. Do not inventory all remaining
   phases up front and do not publish a project-wide ETA yet.

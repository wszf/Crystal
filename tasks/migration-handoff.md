# Crystal Go migration current handoff

Last updated: 2026-08-23 22:05 (Asia/Singapore)

This file is the current recoverable snapshot only. Historical checkpoints were
preserved once in
`tasks/migration-handoff-archive/2026-08-23-2055-pre-control-plane-optimization.md`.
Do not append compact history here and never read that archive during startup.

## Goal and control-plane state

The persistent Goal remains the 100% Legacy-observable-behavior migration to the
independent pure-Go repository. It is not Complete or externally Blocked.

The current Goal thread `01a02e9c-eed1-7c20-b6ed-13bff98175de` was paused after
an explicit `turn_aborted` event while the user requested diagnostics and this
control-plane optimization. That pause is not a token, compact, budget, or
migration blocker. Resume the same Goal after this documentation batch; do not
recreate it solely because the stored objective still contains historical
`gpt-5.6-sol/high` text. Current repository authority is main
`gpt-5.6-sol/ultra`, bounded `luna_worker` subagents at `gpt-5.6-luna/max`.

Normal startup/recovery authority is now:

1. `agents.md` and active `tasks/lessons.md` once per new main session.
2. `tasks/goal-task.md` for the stable contract.
3. `tasks/migration-active.md` for the one active leaf and exact matrix anchors.
4. This file for current worktree/test/recovery state.
5. Only the named Go matrix rows/sections; no full-matrix startup read.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 415]`
- Observed HEAD before the enclosing control-plane commit:
  `158cb4e47e03b2b90c34dfd6478970bf0ae2aead`
  (`158cb4e4 docs(migration): refresh ALLOWTRADE handoff`)
- Expected enclosing commit subject:
  `docs(migration): bound goal control plane`
- No staged files at snapshot creation. Exact tracked/untracked status at
  2026-08-23 21:58 was seven paths and no unrelated changes:
  - `M agents.md` (hard-linked `AGENTS.md` has identical content)
  - `M tasks/goal-task.md`
  - `M tasks/lessons.md`
  - `M tasks/migration-handoff.md`
  - `?? tasks/check-migration-control.sh`
  - `?? tasks/migration-active.md`
  - `?? tasks/migration-handoff-archive/2026-08-23-2055-pre-control-plane-optimization.md`
- The enclosing commit should leave this repository clean. On recovery, actual
  `git rev-parse HEAD` and `git status --short --branch` are authoritative; the
  one documentation-commit delta from the observed HEAD is expected.
- Tracked, staged, and untracked `.cs` audits were empty before this batch and
  must be rerun after commit.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`
- HEAD: `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)
- No staged files.
- Six tracked modifications:
  - `cmd/crystal-server/main.go`
  - `cmd/crystal-server/observer.go`
  - `cmd/crystal-server/world.go`
  - `internal/config/config.go`
  - `internal/config/localization.go`
  - `internal/config/localization_test.go`
- Six untracked files:
  - `cmd/crystal-server/utility_command.go`
  - `cmd/crystal-server/utility_command_session_test.go`
  - `cmd/crystal-server/utility_command_test.go`
  - `internal/config/culture.go`
  - `internal/config/culture_test.go`
  - `legacy_composite_format.go`
- This optimization did not create, edit, format, stage, move, delete, or test
  any Go-repository file. All twelve files remain protected existing work.
- Tracked, staged, and untracked `.cs` audits are empty.
- First quiescence/fingerprint observation at 2026-08-23 21:58:
  - exact `go`/`crystal-server` process audit: empty;
  - status fingerprint:
    `4bfcd5cff216f9d3137220dbfeeb7c4e5d62c79a54dd28607d8c70f596e14b63`;
  - tracked binary-diff fingerprint:
    `7bc79a4c45d309d31dd5449ac58be3ff35da1edaa58d063aa06c69227d86c6d1`;
  - ordered twelve-file SHA-256 manifest fingerprint:
    `e6c626766fcc40774b5507c330423cc08334f435dd5118590d7e4dcfb7e77653`.
  Second comparison at 2026-08-23 22:04 after all review agents closed produced
  the same three fingerprints and another empty exact process audit. The Go
  dirty state was therefore stable across both observations.

## Active leaf and protected work

- Active leaf: `OPS-P1-P3-P4-UTILITY-001`
- Detailed routing and acceptance checklist:
  `tasks/migration-active.md`
- Matrix anchors: rows beginning `| P1 |`, `| P3 |`, `| P4 |`, and `| P8 |`.
- Scope: Legacy-equivalent `@TIME`, `@ROLL`, and `@MAP`; current-culture
  casing/formatting and FormatException boundary; group/system/private recipient
  projection; observer forwarding; authenticated packet transcript.
- Unresolved high-risk artifact: untracked root-level
  `legacy_composite_format.go` is an 892-line unreviewed prototype. Do not wire,
  move, or delete it before main-agent review and an explicit package-boundary
  ruling.
- The current twelve-file Go version has no trustworthy current functional gate:
  later worker patches superseded earlier focused results. Do not promote older
  successful tests to the current version.

## Verification ledger

Documentation/control-plane batch only:

- Exact old-handoff archive copy: `cmp -s` exit 0.
- `tasks/check-migration-control.sh`: exit 0; all five size/structure gates and
  Legacy `.cs` gates passed.
- `sh -n tasks/check-migration-control.sh`: exit 0.
- Legacy `git diff --check`: exit 0.
- Legacy tracked/staged/untracked `.cs`: empty in the control-script run.
- Go status/HEAD and tracked/staged/untracked `.cs`: verified before edits and
  must be repeated after the Legacy commit.
- Read-only `luna_worker` control reviewers: James
  (`01a02edf-26a9-7210-9f54-9aa8dc4eea08`) reviewed contract consistency;
  Newton (`01a02ee0-047d-7613-9712-35e1458e9b1f`) reviewed shell enforcement.
  Both were forbidden from Go access and all writes; both completed, returned
  concise findings, and are closed. No writing subagent was used.
- No `gofmt`, Go compile, focused, repeated, race, full test, vet, build, or
  protocol probe was run for the protected Go batch during this optimization.

## Exact recovery sequence

1. In the Legacy root, read only `agents.md`, `tasks/lessons.md`,
   `tasks/goal-task.md`, `tasks/migration-active.md`, and this file; run
   `tasks/check-migration-control.sh`; verify HEAD/status and all three `.cs`
   gates. Do not read the historical handoff archive.
2. In a separate Go-root call, verify HEAD/status and all three `.cs` gates.
   Confirm the exact twelve protected files are unchanged.
3. Read only the four fixed matrix rows named by the active index. Search the
   archive only for `utility command`, `TIME`, `ROLL`, `MAP`, `culture`,
   `composite format`, `string.Format`, `observer`, and concrete failures.
4. Main-agent review the twelve protected Go files and decide the minimal
   formatter/culture boundary before spawning a bounded `luna_worker` wave.
5. Run the leaf compile/focused/repeated/relevant-race gates, then the due
   integration gate because the leaf crosses config, command routing, observer,
   and shared recipient infrastructure.
6. Update the Go matrix, active index, and this current snapshot; make atomic
   Go and Legacy commits. Then select the next finite leaf in the same Goal.

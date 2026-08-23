# Crystal Go migration current handoff

Last updated: 2026-08-23 23:31 (Asia/Singapore)

This is the replace-in-place current snapshot. Historical checkpoints remain
under `tasks/migration-handoff-archive/` and are not startup-read authority.

## Goal and control-plane state

The persistent Goal is still the 100% Legacy-observable-behavior migration to
the independent pure-Go repository. It is neither Complete nor externally
Blocked. Platform Goal state is currently `paused` after the earlier explicit
turn abort; the user's current instruction authorizes continued work in this
thread. Do not recreate or mark the Goal blocked merely because it is long,
compacts, or still has open scope.

An actual compaction interrupted `OPS-P1-P3-P4-UTILITY-001` before its durable
handoff write completed. The pre-compact handoff described twelve protected Go
files, but recovery found fifteen current files. The automatic compact summary
is therefore not evidence. This snapshot was rebuilt from separate Legacy and
Go repository observations before any further implementation or test.

Current model authority is main `gpt-5.6-sol/ultra`; bounded independent workers
use the fixed `luna_worker` role (`gpt-5.6-luna/max`). The stored Goal objective's
older `high` text does not override the repository contract.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 416]`
- HEAD: `69d471835bf8b84a0137e883938a5a4cfc53c51c`
  (`69d47183 docs(migration): bound goal control plane`)
- No staged or untracked files at reconstruction.
- Current tracked modifications are exactly:
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- `AGENTS.md` and tracked `agents.md` remain the same hard-linked inode.
- Tracked, staged, and untracked `.cs` observations were empty before this
  snapshot write and must remain empty at the post-write verification.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`
- HEAD: `a052d771d29a37c766b20d64cd8518ab94c47260`
  (`a052d77 feat(p6): restore interactive ALLOWTRADE command`)
- No staged files.
- Seven tracked modifications:
  - `cmd/crystal-server/main.go`
  - `cmd/crystal-server/observer.go`
  - `cmd/crystal-server/world.go`
  - `docs/migration-matrix.md`
  - `internal/config/config.go`
  - `internal/config/localization.go`
  - `internal/config/localization_test.go`
- Eight untracked files:
  - `cmd/crystal-server/utility_command.go`
  - `cmd/crystal-server/utility_command_lifecycle_test.go`
  - `cmd/crystal-server/utility_command_session_test.go`
  - `cmd/crystal-server/utility_command_test.go`
  - `internal/config/culture.go`
  - `internal/config/culture_test.go`
  - `internal/legacyfmt/format.go`
  - `internal/legacyfmt/format_test.go`
- The old root-level `legacy_composite_format.go` no longer exists; its candidate
  implementation is now bounded under `internal/legacyfmt`. Do not restore the
  stale path or discard the package without a main-agent ruling.
- Recovery fingerprints at 23:27, computed over the exact current state:
  - status: `e54d8bc9a1798b75d874e492c56dcca3c5eb11d268c48aa6b68783e8756b6d18`;
  - tracked binary diff:
    `0fda5d4a595e432e24362de2992e7c9724179565c1e5306fc7387d40bfb5244d`;
  - ordered fifteen-file manifest:
    `98b9f5fffd7c14ccf122e497f48a54fd5d681f521e661f17f7fe06b99c496f52`.
- Exact `go`/`crystal-server` process audit was empty. Tracked, staged, and
  untracked `.cs` observations were empty.

## Active leaf and protected work

- Active leaf: `OPS-P1-P3-P4-UTILITY-001` (`Active`).
- Router/checklist: `tasks/migration-active.md`.
- Matrix anchors: rows beginning `| P1 |`, `| P3 |`, `| P4 |`, and `| P8 |`.
- Outcome: Legacy-equivalent `@TIME`, `@ROLL`, and `@MAP`; current-culture
  casing/formatting and reachable FormatException lifecycle; correct private,
  group, system, and observer projection; authenticated FIFO transcripts.
- The fifteen files above are protected current work. Preserve every tracked,
  staged, and untracked change; do not reset, stash, checkout, clean, move,
  delete, or overwrite unrelated work.
- `docs/migration-matrix.md` already contains candidate completion prose. It is
  not accepted evidence until the current source is reviewed and fresh gates
  substantiate each claim.

## Verification ledger

- During this compact recovery, no Go implementation, formatting, build, test,
  vet, race run, or protocol probe was started.
- The pre-compact active lesson records that an interrupted formatter writer
  briefly left two signature compile errors, after which the main agent repaired
  them and a formatter-focused test passed. The exact command/output was not
  durably recorded, so it is useful history but not a current leaf gate.
- The automatic compact summary reports a recent unexcluded OmaMage baseline
  failure, but its command/output was not durable and must be reproduced before
  attribution. The targeted archive authority identifies the known failure as
  `TestSessionOmaMageRangeSlowFrozenTranscript`, observed random bounds `[2 1]`
  versus expected `[1]`; it is outside this utility-command scope and must not be
  hidden by modifying unrelated code.
- A formatter writing `luna_worker` had already been explicitly interrupted;
  its current files are treated as unverified. A Legacy auditor had read-only
  authority. Their IDs/results were not preserved in the old handoff and no
  result is being relied on. There is no known active writer and the local exact
  process audit is empty; do not interrupt a future delayed writer merely to
  collect a report.

## Exact recovery sequence

1. Re-read this snapshot and `tasks/migration-active.md`; in the Legacy root run
   `tasks/check-migration-control.sh`, verify exact HEAD/status, `git diff
   --check`, hard-link identity, and all three `.cs` gates.
2. In a separate Go-root call, verify HEAD/status, the fifteen-file manifest,
   exact process quiescence, `git diff --check`, and all three `.cs` gates.
3. Main-agent review the current fifteen-file diff and only the four named
   matrix rows. Reconcile the formatter package, fatal lifecycle, recipients,
   observer order, and matrix prose. Use only targeted archive searches for
   utility command/culture/formatter/OmaMage evidence.
4. Run `gofmt` on owned Go files, minimum package compile, focused and repeated
   production-entry tests, then relevant focused race tests. Preserve exact
   commands, exits, and failure stacks.
5. Run the due integration ordinary test, vet, and build gates; run the required
   race gate at its cadence. Preserve any unexcluded OmaMage failure separately
   and use an explicitly labeled exclusion only for secondary attribution.
6. Update the matrix only to proven evidence, close the leaf in the active
   index, refresh this snapshot, pass both repositories' `.cs` gates, and make
   atomic Go and Legacy commits. Then activate one finite next leaf or one
   bounded `DISC-Px-CLOSURE` inventory audit in the same Goal.

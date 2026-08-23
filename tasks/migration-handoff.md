# Crystal Go migration current handoff

Last updated: 2026-08-24 02:56 (Asia/Singapore)

This replace-in-place snapshot closes `CFG-P1-CONTRACT-001` in committed Go and
routes the next dependency-ready P1 child, `LOC-P1-CATALOG-001`. Localization
writes remain gated only until the enclosing Legacy routing commit is clean.

## Goal and control-plane state

The persistent full Go migration Goal remains active, not Complete or externally
Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers use
`luna_worker` (`gpt-5.6-luna/max`). P1 remains scope-frozen and In progress.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before the enclosing routing commit:
  `master...origin/master [ahead 420]`.
- Observed HEAD: `b967cdd07d30ac2513a5b41d6b6912e5828e67c4`
  (`docs(migration): expand P1 config startup authority`).
- Expected enclosing commit subject: `docs(migration): route P1 localization
  catalog`. Recovery may observe exactly this one documentation commit delta.
- Staged and untracked files: none.
- Tracked modifications are exactly `tasks/lessons.md`,
  `tasks/migration-active.md`, and `tasks/migration-handoff.md`.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`.
- HEAD: `fbc69d25f3577ac7d2f16be17bbdbeba10722285`
  (`feat(config): match Legacy P1 contract`).
- Worktree, staged files, and untracked files are clean.
- Matrix marks CFG Complete and LOC catalog Active; no localization source is
  modified.

## Active leaf and protected work

- Active leaf: `LOC-P1-CATALOG-001` for routing only until the expected Legacy
  commit is clean.
- Outcome: exact 768-key server defaults plus English/Chinese overlay key,
  placeholder, fallback, missing/malformed, and startup behavior.
- Matrix anchors: `### 2026-08-24 P1 finite closure inventory`, the LOC row, and
  the row beginning `| P1 |`.
- Legacy read authority: `Shared/Language.cs`, the two server localization JSON
  assets, and bounded startup/lookup consumers.
- Go owned files after the gate: `internal/config/localization.go`,
  `localization_test.go`, `server_text_catalog.go`, and
  `server_text_catalog_test.go`.
- Only the three Legacy control files are protected. Every C# file remains
  read-only.

## Verification ledger

- `go test ./internal/config ./cmd/crystal-server -run '^$'`: exit 0.
- `go test ./internal/config -count=20`: exit 0.
- `go test ./cmd/crystal-server -run '^TestProductionConfigPath' -count=20`:
  exit 0.
- Focused config and startup `go test -race` commands: exit 0.
- Required unexcluded `go test ./...`: exit 1 only at established
  `TestSessionOmaMageRangeSlowFrozenTranscript`, `[2 1]` versus `[1]`.
- Isolated OmaMage `-count=3`: exit 1 with the same known maintenance-tick
  signature and no CFG file in its stack.
- First run excluding only OmaMage: exit 1 at
  `TestSessionYinDevilNodeTranscript/42`, empty notification versus `[84]`;
  isolated `-count=10` then exits 0.
- `go test ./... -skip
  '^(TestSessionOmaMageRangeSlowFrozenTranscript|TestSessionYinDevilNodeTranscript)$'`:
  exit 0. This is an excluded integration pass, not a full pass.
- `go vet ./...` and `go build ./...`: exit 0.
- The initial expected-failure config test encoded stricter Go semantics for a
  missing version file; Legacy evidence ruled missing files nonfatal with an
  empty rejecting hash set, and the corrected focused suite passes.

## Quiescence

- Read-only auditor `01a02fe2-9b51-7233-9ed6-886ccd7925c0` and test writer
  `01a02fe9-11fa-7381-98de-463eb709c444` are closed; no result is pending.
- No writing subagent remains. The main agent owns integration, docs, and both
  commits.
- No localization write has started.

## Exact recovery sequence

1. Verify both repository statuses separately and all six `.cs` gates.
2. Run the control checker/diff check and commit only the three Legacy control
   files with the expected subject.
3. Verify both worktrees clean, then begin `LOC-P1-CATALOG-001` with its targeted
   archive search and bounded Legacy/Go catalog tracing.

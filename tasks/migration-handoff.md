# Crystal Go migration current handoff

Last updated: 2026-09-03 04:19 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. `CFG-P1-SAVEDELAY-001` is Complete in Go
  `b77e0119f74c5c178532e9892406a4a2b36fc6fb` as the missing `[Database] SaveDelay`
  INI input; it does not start periodic save, backup or cross-store recovery.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `d7d9c0c9701d04f3deead1a3f31e8842b4e544fb`; upstream
  `origin/migration/goal-orchestration`, ahead/behind 0.
- Before this control refresh the index and worktree were clean. The only
  expected unstaged paths after this write are `tasks/migration-active.md` and
  this handoff. There are no staged or untracked paths and no Legacy
  implementation changes. `tasks/lessons.md` is clean.
- Pre-edit `git diff --check`, `git diff --cached --check` and all three Legacy
  C# queries exit 0/empty. This snapshot records the expected one control-document
  commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `b77e0119f74c5c178532e9892406a4a2b36fc6fb`; tracks `origin/migrate/drop-owner-p12`.
- Index and worktree are clean. Commit `b77e011` contains SaveDelay production,
  tests and matrix evidence only.
- `git diff --check`, `git diff --cached --check` and all three Go C# queries
  exit 0/empty. No owned Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now loads Legacy `Settings.SaveDelay` default 5 through
  `applyLegacyP1`, write-backs missing/invalid `SaveDelay=5`, and accepts
  zero/negative int32 values. Do not treat this as WorkLoop periodic save.
- Preserve completed P2-P11 authorities, latest-auth revision/CAS, the bounded
  P12 counter/CanStart/JSON/auth-snapshot slices, and persistence-before-visible
  projection. Do not implement backup/restore, unified periodic save, or infer a
  shared recovery owner.
- All `.cs` remains read-only.

## Verification ledger

- `go test ./internal/config -run '^$'` exits 0.
- Focused `TestDatabaseSaveDelay*` plus missing-Setup/invalid-P1 write-back tests
  exit 0; SaveDelay repeated count 20 and race count 5 exit 0.
- `go test ./internal/config -count=1` exits 0.
- `go test ./cmd/crystal-server -run 'TestProductionConfigPath' -count=1` exits 0.
- `git diff --check` and both-repository C# gates are empty. No consumer ticker
  or WorkLoop save was added.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Read the P12 summary and finite ledger; treat `CFG-P1-SAVEDELAY-001` as a
   completed INI input, not a periodic-save owner.
3. Continue read-only owner tracing for backup, world-export, sidecar restart and
   unified WorkLoop save. Do not infer a new owner or write C#.
4. Keep remaining shared-owner recovery in discovery; do not reopen completed
   P12 slices or P1 Config beyond the SaveDelay field already landed.

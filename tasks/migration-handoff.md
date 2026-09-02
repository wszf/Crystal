# Crystal Go migration current handoff

Last updated: 2026-09-03 04:38 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. `CFG-P1-SAVEDELAY-001` remains Complete. The 117 account
  writer now uses Legacy SaveAccounts n/o staging in Go
  `e972def333c5ef4780c92581716d80e5da96207a`. Dated account backups and unified
  WorkLoop save are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `1e6a2549d65260ff6104437875c03d1837a39e29`; upstream
  `origin/migration/goal-orchestration`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff. No staged or
  untracked paths and no Legacy implementation changes.
- Pre-edit `git diff --check` and all three Legacy C# queries exit 0/empty.
  This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `e972def333c5ef4780c92581716d80e5da96207a`; tracks `origin/migrate/drop-owner-p12`.
- Index and worktree are clean. Commit `e972def` contains 117 n/o staging,
  tests and matrix evidence only.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- `WriteLegacyDatabase` now writes `pathn`, renames a live file to `patho`,
  promotes the staged file, and deletes leftover `.o`. Loaders still read only
  the final path. This is not a restore selector and does not create
  `Back Up/Accounts` timestamped copies.
- Preserve completed P2-P11 authorities, SaveDelay INI, latest-auth snapshot
  generation, and persistence-before-visible projection. Do not implement
  unified periodic save, backup/restore, or infer a shared recovery owner.
- All `.cs` remains read-only.

## Verification ledger

- `go test ./internal/legacyaccount -run '^$'` exits 0.
- Focused n/o tests pass count 20 and race count 5.
- `go test ./internal/legacyaccount ./internal/legacyaccountbridge ./internal/auth -count=1` exits 0.
- `go test ./cmd/crystal-server -run 'TestProductionStartupLegacyAccountCounterCheckpointRestart' -count=1` exits 0.
- `go vet ./...` and `go build ./...` exit 0. The registered Quest fixture remains
  the unskipped baseline. No WorkLoop ticker or dated account backup was added.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI and 117 n/o staging as completed inputs, not backup or
   periodic-save owners.
3. Continue read-only owner tracing for dated `Back Up/Accounts`, world-export
   SaveDB copies, guild/goods/conquest n/o files, sidecar restart and unified
   WorkLoop save. Do not write C#.
4. Keep remaining shared-owner recovery in discovery.

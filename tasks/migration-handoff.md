# Crystal Go migration current handoff

Last updated: 2026-09-03 06:15 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. GuildRefreshNeeded is Complete in Go
  `765a4b0cf9faa82b03e53bc0cbe957d60f3d4c85`. MirDB rewrite is still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `604dbc245a7fc93441d09a95687693a61dd14e79`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `765a4b0cf9faa82b03e53bc0cbe957d60f3d4c85`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- Disbanding a guild now requests a refresh pass. The next SaveDelay deletes
  `Guilds/*.mgd` and force-resaves remaining guilds with new 0..n indexes.
- Do not claim MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- Refresh-delete guild file tests pass count 20 and race count 5.
- Disband consumes the refresh flag; focused auth tests pass count 20/race 5.
- `go vet` of touched packages and `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat GuildRefreshNeeded as a completed input, not MirDB rewrite.
3. Continue owner tracing for SaveDB rewrite. Do not write C#.

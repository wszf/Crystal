# Crystal Go migration current handoff

Last updated: 2026-09-03 06:06 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. NeedSave gating is Complete in Go
  `672d23612d58ddf7459ba8d607b8a78eb083a353`. GuildRefreshNeeded and MirDB
  rewrite are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `5c38309167e31186b88f0bca0d9be62d1ba02097`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `672d23612d58ddf7459ba8d607b8a78eb083a353`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- Periodic guild/conquest/UsedGoods file writes now skip clean objects.
  Guild and conquest NeedSave clear before write; NPC NeedSave is never
  cleared, matching Legacy.
- Do not claim GuildRefreshNeeded or MirDB rewrite. All `.cs` remains
  read-only.

## Verification ledger

- Skip-clean guild test and focused write/order tests pass count 20/race 5.
- `go test ./internal/auth -count=1`, `go vet` of touched packages and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat NeedSave gating as a completed input, not GuildRefreshNeeded or
   MirDB rewrite.
3. Continue owner tracing for GuildRefreshNeeded and SaveDB rewrite.
   Do not write C#.

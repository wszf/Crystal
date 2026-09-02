# Crystal Go migration current handoff

Last updated: 2026-09-03 05:53 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. WorkLoop store order is Complete in Go
  `62bd2f7ef985f6ee00f4e834be5ca37bd04a709a`. NeedSave, GuildRefreshNeeded and
  MirDB rewrite are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `53c0f3838ee458b93a794d5a145d176346d7e7c0`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `62bd2f7ef985f6ee00f4e834be5ca37bd04a709a`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now runs Accounts → Database copy → Guilds → Goods → Conquests
  in one timer, matching Envir.WorkLoop call order.
- Do not claim NeedSave, GuildRefreshNeeded, MirDB rewrite, or a single
  WorkLoop thread. All `.cs` remains read-only.

## Verification ledger

- WorkLoop order test passes count 20 and race count 5.
- Existing periodic account backup tests still pass.
- `go test ./cmd/crystal-server -run '^$'`, `go vet ./cmd/crystal-server` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat file stores and WorkLoop order as completed inputs, not NeedSave or
   MirDB rewrite.
3. Continue owner tracing for NeedSave, GuildRefreshNeeded and SaveDB rewrite.
   Do not write C#.

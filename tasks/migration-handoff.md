# Crystal Go migration current handoff

Last updated: 2026-09-03 06:25 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. World JSON rewrite is Complete in Go
  `a078e28bbc4f2ff53055796975b7e284dd6eb685`. Binary Server.MirDB rewrite is
  still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `e5a6071eefa369071c9d6350d8b8cdf6ae409171`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `a078e28bbc4f2ff53055796975b7e284dd6eb685`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now copies the world catalog to Back Up/Database then rewrites the
  live world JSON from the loaded snapshot.
- Do not claim binary Server.MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- WorkLoop order test proves backup content differs from the rewritten live
  catalog; count 20 and race count 5 pass.
- `go vet ./cmd/crystal-server` and `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat world JSON rewrite as a completed input, not binary MirDB rewrite.
3. Continue owner tracing for Server.MirDB binary rewrite and remaining
   shared-owner recovery. Do not write C#.

# Crystal Go migration current handoff

Last updated: 2026-09-03 06:36 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. Missing world JSON create is Complete in Go
  `af374ad7a74bc40616b09290a25aa724b2295bda`. Binary Server.MirDB rewrite is
  still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `dda40e01d6089199906be8ebb74c94ffa089c704`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `af374ad7a74bc40616b09290a25aa724b2295bda`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- A missing WorldExportPath is created as empty JSON then loaded. Existing
  catalog files are left unchanged.
- Do not claim binary Server.MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- Create-missing and leave-existing tests pass count 20 and race count 5.
- `go vet ./cmd/crystal-server` and `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat missing world JSON create as a completed input, not binary MirDB
   rewrite.
3. Continue owner tracing for Server.MirDB binary rewrite and remaining
   shared-owner recovery. Do not write C#.

# Crystal Go migration current handoff

Last updated: 2026-09-03 05:05 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. World-export dated copies are Complete in Go
  `87e33fffc819dc0c67fb095e246f6a63f4446492`. Guild/goods/conquest files and
  unified WorkLoop save are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `81c1bc1eb7cf4745761be9d91e5792f017885af2`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `87e33fffc819dc0c67fb095e246f6a63f4446492`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now copies the live world export to CWD-relative
  `Back Up/Database/Database YYYY-MM-DD HH-MM-SS.bak` without moving it, then
  continues the existing account backup/JSON/117 path.
- Do not rewrite MirDB or implement guild/goods/conquest periodic files.
  All `.cs` remains read-only.

## Verification ledger

- World-copy tests pass count 20 and race count 5.
- Persist tests now include world copy; count 20 and race count 5 pass.
- Production TCP account checkpoint/restart still passes.
- `go vet` of touched packages and `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI, 117 n/o, account backups and world-export copies as
   completed inputs, not unified WorkLoop save.
3. Continue owner tracing for guild/goods/conquest files and sidecar restart.
   Do not write C#.

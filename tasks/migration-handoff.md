# Crystal Go migration current handoff

Last updated: 2026-09-03 05:22 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. UsedGoods `.msd` writes are Complete in Go
  `73ae28caacc03b5083106117cacd8ca5b3eb8665`. Guild `.mgd` and conquest `.mcd`
  files are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `e7baa4c35a2bc5ac7e03df126cc630f3a0df6ec6`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `73ae28caacc03b5083106117cacd8ca5b3eb8665`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now writes CWD-relative `Envir/Goods/{index}.msd` from live NPC
  UsedGoods using the current 9999/version/custom/count layout and n/o staging.
- Do not add guild `.mgd` or conquest `.mcd` writers. All `.cs` remains read-only.

## Verification ledger

- UsedGoods round-trip/n/o tests pass count 20 and race count 5.
- World `.msd` write tests pass count 20 and race count 5.
- `go vet` of touched packages and `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI, 117 n/o, backups, sidecar staging and `.msd` writes as
   completed inputs, not guild/conquest files.
3. Continue owner tracing for `.mgd`/`.mcd` and unified WorkLoop save.
   Do not write C#.

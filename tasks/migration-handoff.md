# Crystal Go migration current handoff

Last updated: 2026-09-03 05:11 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. Sidecar n/o staging is Complete in Go
  `f116b1b118e764f0fa7136e5bfa6a7dbf34c1a99`. Envir/Goods `.msd`, guild `.mgd`
  and conquest `.mcd` files are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `228c83295ec2d37288e44af0241d152ea1f859ae`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `f116b1b118e764f0fa7136e5bfa6a7dbf34c1a99`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- The UsedGoods/respawn sidecar now writes `pathn`, promotes via `patho`, and
  deletes leftover `.o`. Loaders still read only the final runtime JSON.
- Do not add Envir/Goods `.msd` writers or guild/conquest periodic files.
  All `.cs` remains read-only.

## Verification ledger

- Sidecar n/o tests pass count 20 and race count 5.
- `go test ./internal/worlddata -count=1`, `go vet ./internal/worlddata` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI, 117 n/o, account/world backups and sidecar staging as
   completed inputs, not `.msd`/`.mgd`/`.mcd` writers.
3. Continue owner tracing for guild/goods/conquest Legacy files. Do not write C#.

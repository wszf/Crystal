# Crystal Go migration current handoff

Last updated: 2026-09-03 05:35 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. Guild `.mgd` writes are Complete in Go
  `bb8d06358b833645d62399e5e37953ccdeb76c69`. Conquest `.mcd` files are still
  unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `693734e0c7bb52e920545fa52196c7b710f69131`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `bb8d06358b833645d62399e5e37953ccdeb76c69`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now writes CWD-relative `Guilds/{listIndex}.mgd` from
  `auth.Service.GuildsSnapshot()` using the MaxInt32/117/0 header and n/o
  staging. Empty ranks are skipped.
- Do not add conquest `.mcd` writers. All `.cs` remains read-only.

## Verification ledger

- Guild round-trip and index tests pass count 20 and race count 5.
- `go test ./cmd/crystal-server -run '^$'`, `go vet` of touched packages and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI, 117 n/o, backups, sidecar, `.msd` and `.mgd` writes as
   completed inputs, not conquest files.
3. Continue owner tracing for `.mcd` and unified WorkLoop save. Do not write C#.

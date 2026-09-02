# Crystal Go migration current handoff

Last updated: 2026-09-03 04:51 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. Periodic dated account backups are Complete in Go
  `43332ad173eacbc6d37347957b72e9181bb7acd4`. Unified WorkLoop save of guilds,
  goods, conquests and SaveDB is still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `76bdd93ac9cfd9a57726907747c69a20c07dd24c`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff. No staged or
  untracked paths and no Legacy implementation changes.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `43332ad173eacbc6d37347957b72e9181bb7acd4`.
- Index and worktree are clean. Commit `43332ad` contains dated account backup
  helper, SaveDelay timer, tests and matrix evidence.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- After `SaveDelay` minutes the live 117 file is moved to CWD-relative
  `Back Up/Accounts/Accounts YYYY-MM-DD HH-MM-SS.bak`, then JSON/117 rewrite.
  Mutation and shutdown SaveJSON still use n/o staging only.
- Do not implement guild/goods/conquest/SaveDB periodic save or a restore
  selector. All `.cs` remains read-only.

## Verification ledger

- Backup helper tests pass count 20 and race count 5.
- Interval/persist tests pass count 20 and race count 5.
- Production TCP account checkpoint/restart still passes.
- `go test ./cmd/crystal-server -run '^$'`, `go vet` of touched packages and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI, 117 n/o staging and periodic account backups as
   completed inputs, not unified WorkLoop save.
3. Continue owner tracing for SaveDB copies, guild/goods/conquest files and
   sidecar restart. Do not write C#.

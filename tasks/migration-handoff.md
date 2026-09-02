# Crystal Go migration current handoff

Last updated: 2026-09-03 05:44 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. Conquest `.mcd` writes are Complete in Go
  `48cdac63e6573a2559904ca42bae4cc9de74e463`. Unified WorkLoop save is still
  unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `f3aa70cc954bb007d2a1641ec1329a5fe9cfdddb`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `48cdac63e6573a2559904ca42bae4cc9de74e463`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- SaveDelay now writes CWD-relative `Conquests/{Info.Index}.mcd` from
  `auth.Service.ConquestsSnapshot()` with n/o staging.
- Do not claim a single-threaded WorkLoop or MirDB rewrite. All `.cs` remains
  read-only.

## Verification ledger

- Conquest round-trip tests pass count 20 and race count 5.
- Index-name tests pass count 20 and race count 5.
- `go vet` of touched packages and `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat SaveDelay INI, 117 n/o, backups, sidecar, `.msd`, `.mgd` and `.mcd`
   writes as completed inputs, not unified WorkLoop save.
3. Continue owner tracing for single-threaded save ordering, NeedSave, and
   MirDB rewrite. Do not write C#.

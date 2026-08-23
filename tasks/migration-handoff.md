# Crystal Go migration current handoff

Last updated: 2026-08-24 02:27 (Asia/Singapore)

This is the replace-in-place recovery snapshot after the committed P1 closure
inventory was verified and `CFG-P1-CONTRACT-001` was released for production
work.

## Goal and control-plane state

The persistent Goal remains the complete Legacy-observable-behavior migration
to the independent pure-Go repository. It is active, not Complete or externally
Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers use
`luna_worker` (`gpt-5.6-luna/max`).

P1 is scope-frozen with ten unfinished finite children and two completed audit
items. Project-wide percentage and ETA remain Unavailable while other phase
inventories are open.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 418]`.
- Observed HEAD before the enclosing normalization commit:
  `73233ac3121c9e870fd3f75a4012b0e3561c2932`
  (`docs(migration): activate P1 config contract`).
- Expected enclosing commit subject: `docs(migration): normalize P1 recovery
  point`. Recovery may observe exactly this one documentation commit delta and
  a clean Legacy worktree.
- Staged files: none. Untracked files: none.
- Tracked modifications are exactly `tasks/lessons.md`,
  `tasks/migration-active.md`, and `tasks/migration-handoff.md`.
- The lesson records and invalidates the mixed-repository startup call; all
  startup authority was subsequently reread in separate zero-exit calls.
- `AGENTS.md` and tracked `agents.md` remain the same hard-linked inode.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`.
- HEAD: `460b8ed8f1ca10c5a9da5028948165d062e35db7`
  (`docs(migration): freeze P1 closure inventory`).
- Worktree, staged files, and untracked files are clean.
- The committed matrix contains the P1 finite inventory and marks P1
  `In progress (scope frozen)`.

## Active leaf and protected work

- Active leaf: `CFG-P1-CONTRACT-001`.
- Outcome: match the P1-owned Legacy Setup.ini/default/error/version-file
  contract without absorbing feature-specific P2-P12 settings.
- Matrix anchors: `### 2026-08-24 P1 finite closure inventory`, the
  `CFG-P1-CONTRACT-001` row, and the row beginning `| P1 |`.
- Legacy read authority: `Shared/Functions/IniReader.cs`, General/Network
  declarations and `Load`/`Save`/`LoadVersion` consumers in
  `Server/Settings.cs`, plus bounded direct startup consumers.
- Go owned files: `internal/config/config.go`,
  `internal/config/config_test.go`, new
  `internal/config/p1_contract_test.go`, and only if required new
  `cmd/crystal-server/p1_config_startup_test.go`.
- Protected Legacy files until the enclosing documentation commit are
  `tasks/lessons.md`, `tasks/migration-active.md`, and
  `tasks/migration-handoff.md`. No production Go file is modified yet.
- Forbidden scope remains localization catalog, network admission, logging,
  HTTP/status services, lifecycle restructuring, feature settings, broad
  matrix edits, and every C# change.

## Verification ledger

- The originally combined startup/status call was invalidated under C01 because
  it used `git -C` across repositories. Legacy startup documents and status,
  Go status, and the three matrix anchors were then reread in separate complete
  zero-exit calls.
- Legacy HEAD/subject and the preceding documentation commit were verified;
  the Legacy and Go worktrees were separately clean before these three control
  edits.
- Tracked, staged, and untracked `.cs` gates were empty in both repositories.
- Exact process audit found no `go` or `crystal-server` process.
- No functional test has run for `CFG-P1-CONTRACT-001`; production code is still
  untouched. The enclosing documentation commit requires the control checker,
  diff checks, status review, and both `.cs` gates.

## Quiescence

- No subagent has been started in this Goal session and no result is pending.
- No Go/server process was present at the recovery audit.
- Only the three listed Legacy control files are intentionally modified.

## Exact recovery sequence

1. Verify the expected Legacy documentation commit and clean status in a
   Legacy-only call; verify Go HEAD/clean status in a separate Go-only call.
2. Search the lessons archive for `CFG-P1-CONTRACT-001`, `IniReader`,
   `Settings`, `Setup.ini`, config, version, and startup keywords, reading only
   matching sections.
3. Delegate one bounded read-only Legacy contract audit to `luna_worker` while
   the main agent derives the Go config gap from exact owned files.
4. Implement only the registered Go files, run the leaf gate, update the matrix
   and active index, refresh this snapshot, and commit owned files.

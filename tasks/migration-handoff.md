# Crystal Go migration current handoff

Last updated: 2026-08-24 15:00 (Asia/Singapore)

This replace-in-place snapshot completes the bounded `DISC-P3-CLOSURE` audit,
freezes the eleven-child P3 denominator, and routes `CHAR-P3-CREATE-001` as the
one Active leaf. P1, P2, P3, and the persistent Goal remain In progress; no
phase or Goal completion is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress with twelve children: nine Complete and
  three unfinished.
- P2 remains scope-frozen/In progress with eight children: six Complete and two
  dependency-blocked Ready.
- P3 is now scope-frozen/In progress with eleven children: four Complete,
  `CHAR-P3-CREATE-001` Active, and six Ready.
- `DISC-P3-CLOSURE` is Complete. Its accepted denominator removes the vague
  ranking, administrator-capability, and client-startup residuals without
  implementing a child or reopening P2.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 441]`; observed HEAD
  `a61a30681825d9980d64594d5f42ac14a99999eb`
  (`docs(migration): close source precedence leaf`).
- Before replacing this file, tracked modifications were exactly
  `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`, and
  `tasks/migration-active.md`. This handoff is the fourth expected tracked
  modification. Index and untracked set are empty.
- The archive records the corrected Server.MirForms path, hidden-Go-helper
  failure/recovery, missed GMPassword/operator entry points, and denominator
  overlap/cycle review. Active lessons remain unchanged.
- No C# file is owned; final tracked, staged, and untracked C# gates must remain
  empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; observed clean HEAD
  `6666a91f8add63586808c1a0c270507f40bedd7b`
  (`docs(migration): freeze P3 closure inventory`).
- The commit records exactly eleven P3 children: four Complete, one Active, six
  Ready; the exact P3 stage row is `In progress (scope frozen)`.
- Worktree, index, and untracked set are empty. No C# file is owned.

## Active leaf and protected work

- Active leaf: `CHAR-P3-CREATE-001`.
- Outcome: preserve Select-stage-only NewCharacter with exact setting,
  IP-throttle, validation/result source order, administrator disabled-name
  bypass, class gates, non-deleted limit, tombstone name reservation, unchecked
  character ID allocation, creation metadata, account/global insertion, and
  complete returned SelectInfo.
- Matrix anchors: only the `CHAR-P3-CREATE-001` registry row, exact P3 stage row,
  and evidence headings/tests named by that row. Do not read the full matrix.
- Legacy read authority: `MirConnection.NewCharacter`, `Envir.NewCharacter`,
  `MirConnectionLog.CharactersMade`, `CharacterInfo` construction/SelectInfo,
  and the SelectScene consumer. Every C# file remains read-only.
- Go write authority: bounded `internal/auth/service.go`,
  `cmd/crystal-server/main.go`, their existing tests, one new focused creation
  production-entry test, and matrix evidence.
- Required gate: all result codes/order, current/future IP admission, complete
  SelectInfo, JSON/117 reload, touched compile, focused/repeated/race,
  diff/status/process, and all six C# gates.
- Forbidden: delete/tombstone/ban/start/logout, operator/ranking work, P2 reopen,
  P4-P12 inventory, broad matrix/source dumps, destructive Git, and C# writes.

## Verification ledger

The P3 discovery leaf changed documentation only. Final integration commands
against the accepted Go candidate exited 0:

- `go test ./... -count=1 -timeout=20m` (all packages; server 72.929s).
- `go vet ./...`.
- `go build ./...`.
- `git diff --check`, exact Go status, process inspection, and Go C# gates.

The initial one-off matrix editor used hidden `.tmp_p3_matrix_edit.go`; Go
ignored it and a later successful delete masked the intermediate failure. That
call produced no row edit and was not used. The exact replacement was rerun
with `set -e`, non-hidden `tmp_p3_matrix_edit.go`, single-match assertions, and
readback; the temporary file is absent.

No full race is due for this documentation-only scope freeze: P3 remains In
progress and no shared code/concurrency changed. Focused race remains mandatory
for the Active creation leaf.

## Subagents and quiescence

- Legacy auditor `01a0325d-e76b-7fa0-a241-86b4b1181ea1` enumerated twelve
  reachable P3 families read-only and is closed.
- Go auditor `01a0325e-267f-7652-9bcf-8018a52e9eb4` mapped production/test gaps
  read-only and is closed.
- Denominator reviewer `01a0327e-55a1-7f63-9fb7-0b8bdcc061af` requested one
  boundary/dependency revision, then accepted 11 = 4 Complete + 7 unfinished;
  it is closed.
- No subagent result is pending. Exact Go/server/git process inspection was
  empty before this handoff.

## Exact recovery sequence

1. Verify each repository separately against the exact HEAD/status/file lists
   above; preserve all four owned Legacy documentation modifications.
2. Run `tasks/check-migration-control.sh`, `git diff --check`, process/status and
   all six C# gates; commit only the four Legacy documentation files.
3. Read only the Active creation matrix row/stage row and search the lessons
   archive with `CHAR-P3-CREATE-001`, NewCharacter, CharactersMade, validation,
   IP throttle, SelectInfo, and JSON/117 keywords.
4. Trace the bounded Legacy/Go creation ledgers, delegate at most one bounded
   writer plus one read-only reviewer with disjoint scope, then implement and
   run the registered leaf gate.

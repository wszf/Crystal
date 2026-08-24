# Crystal Go migration current handoff

Last updated: 2026-08-24 11:42 (Asia/Singapore)

This replace-in-place snapshot closes `DISC-P2-CLOSURE`, records P2 scope
freeze, and routes `AUTH-P2-ACCOUNT-SESSION-001` Active. P2 and the persistent
Goal remain In progress; no phase or Goal completion is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress with twelve registered children: nine
  Complete and three unfinished. P2 is now scope-frozen/In progress with eight
  children: four Complete, one Active, and three Ready.
- Go matrix commit `5646b49382af5c04f11086d34aff6b05bd6c703d`
  makes the exact account/P3, storage/P7, and persistence/P12 boundaries
  authoritative and selects `AUTH-P2-ACCOUNT-SESSION-001` Active.
- `BOOT-P4-STARTPOINT-001`, `PERSIST-P12-CANSTART-DBCHECKS-001`, and
  `PERSIST-P12-ACCOUNT-ID-001` are finite cross-phase inputs, not hidden P1/P2
  implementation permission.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before this snapshot commit: `master...origin/master [ahead 437]`;
  HEAD before this snapshot commit:
  `5d5d2b99a6a551fdda1f0303871c159477a25aa0`
  (`docs(migration): preserve P2 discovery checkpoint`).
- Before this snapshot edit, tracked modifications were exactly
  `tasks/migration-active.md` and
  `tasks/lessons-archive/migration/protocol-session-wire.md`; the index and
  untracked set were empty. This handoff is the third owned documentation file.
- The lesson archive records the independent review symptom/root/prevention/
  verification. Active Index freezes P2 and selects the account-session child.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- `tasks/check-migration-control.sh` and `git diff --check` pass.
- Tracked, staged, and untracked C# gates are empty. No C# file is owned.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; observed clean HEAD:
  `5646b49382af5c04f11086d34aff6b05bd6c703d`
  (`docs(migration): freeze P2 account scope`).
- Worktree, index, and untracked set are empty; all three C# gates are empty.
- The commit changes only `docs/migration-matrix.md`: eight P2 children, explicit
  cross-phase inputs/exclusions, P2 scope-frozen status, and the next Active row.
- No Go production/test file changed during discovery. The production candidate
  remains the lifecycle-tested code at parent `cb595b4e40ae`.

## Active leaf and protected work

- Active leaf: `AUTH-P2-ACCOUNT-SESSION-001`.
- Outcome: exact TCP NewAccount/Login/ChangePassword validation, unchecked
  account index and creation/login metadata, localized ban/retry behavior,
  wrong-stage/banned connection lifetime, and same-account reason-1 takeover.
- Matrix anchors: the P2 stage row and exact child row only.
- Legacy read authority: bounded `Envir.NewAccount/Login/ChangePassword`, their
  Login-stage `MirConnection` handlers, `AccountInfo`, and server-text reason
  producers. Every C# file remains read-only.
- Go write authority: `internal/auth/service.go` plus focused tests; bounded
  `cmd/crystal-server/main.go`; new `p2_account_session_test.go`; optional one
  account-session authority helper; P2 matrix evidence.
- Forbidden: character metadata, storage/NPC behavior, generic P3/P7/P12 work,
  broad source/matrix dumps, destructive Git, and every C# write.
- Required gate: touched-package compile, focused authenticated repeated/race,
  JSON/117 checkpoint/reload, diff/status/process, and all six C# gates.

### Durable P2 discovery evidence

- Legacy read-only auditor `01a031a0-c670-74e1-92a9-87cda4e1fbcd`
  (`luna_worker`, `gpt-5.6-luna/max`) traced live auth stage/result codes,
  PBKDF2 quirks, account/character metadata, storage-password/NPC gates,
  lifecycle timestamps, save/load/archive behavior, and dead/comment-only
  paths. It changed no files and ran no tests.
- Go read-only auditor `01a031a0-efe7-7810-a9b1-f182b0c9bc37`
  (`luna_worker`, `gpt-5.6-luna/max`) reconciled existing auth, protocol,
  JSON/117 bridge, checkpoint, restart, and global-section evidence. It changed
  no files and ran no tests.
- Main tracing added wrong-stage auth connection lifetime, banned-session retry,
  duplicate-account reason-1 takeover, Unicode email, SelectInfo filtering/cap,
  storage page authorization, and source-precedence boundaries to the initial
  worker findings.
- Final reviewer `01a031c0-18ea-71e3-ba9f-b6cf96be57d4` first rejected the
  candidate for storage visibility/page, character ban/delete ownership,
  wrong-stage evidence, and global-writer overclaim. After exact P3/P7/P12
  routing and evidence wording fixes, its second read-only review found no
  blocker, accepted exactly eight children, and selected the current leaf.

## Verification ledger

No tests, builds, formatters, or code writes ran during P2 discovery; only the
matrix/control/lesson documentation changed. Read-only source/status commands
in each repository exited 0. The unchanged Go production candidate retains
these final exit-0 lifecycle gates at parent `cb595b4e40ae`:

- `gofmt -w cmd/crystal-server/main.go cmd/crystal-server/main_test.go
  cmd/crystal-server/process_lifecycle_test.go`; then touched-package compile
  `go test ./cmd/crystal-server -run '^$' -count=1`.
- Focused lifecycle/fatal production regex at `-count=3 -timeout=15m`.
- The same focused set under `go test -race` at `-count=3 -timeout=15m`.
- The strengthened concurrent-cancel/former-two-second-boundary fatal regression
  under race at `-count=5`; final `luna_worker` re-review reran it and passed.
- `go test ./... -count=1 -timeout=20m` (fresh, unexcluded).
- `go test -race ./... -count=1 -timeout=30m` (fresh, unexcluded).
- `go vet ./...`, `go build ./...`, and full-module builds with each of
  `GOOS=plan9`, `GOOS=windows`, and `GOOS=linux`, all at `GOARCH=amd64`.
- `git diff --check`, exact status/process quiescence, and all six C# gates.

During development, one full-race run reproduced the established unrelated
OmaMage `[2 1]`/`[1]` random-order flake; its exact isolated race run passed.
A later full-race run exposed the new fatal publish-before-wake test returning
nil; the root was fixed and archived. Fresh unexcluded full normal and full race
both pass after the final fixes, so no excluded final run remains.

The discovery has no test-failure attribution. Independent review findings were
documentation/authority defects and are resolved in the committed matrix plus
the owned lesson/control edits.

## Quiescence

- All three P2 read-only workers/review threads completed and were closed; no
  writing subagent was used and no result is pending.
- Exact `comm`-based process inspection found no `go` or `crystal-server`
  process. Both repositories were stable and clean before this snapshot edit.

## Exact recovery sequence

1. Verify each repository separately. Go must remain clean at
   `5646b49382af5c04f11086d34aff6b05bd6c703d`; Legacy must be clean at exactly
   one owned documentation commit after pre-snapshot HEAD `5d5d2b99a6a5`.
2. Read only the P2 stage row and `AUTH-P2-ACCOUNT-SESSION-001` child row, then
   the exact Legacy/Go auth entry points named above; do not reopen P2 discovery.
3. Implement the account-session child in its owned files. Start with a focused
   failing production transcript for wrong-stage survival, banned retry, and
   duplicate-account reason-1 takeover; then add metadata/localization/persistence.
4. Run the required compile/focused/repeated/race/JSON/117 leaf gate, update the
   matrix and control snapshot, close workers, and commit only owned files after
   diff/status/process and all six C# gates pass.

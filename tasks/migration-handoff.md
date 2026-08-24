# Crystal Go migration current handoff

Last updated: 2026-08-24 12:39 (Asia/Singapore)

This replace-in-place snapshot records `AUTH-P2-ACCOUNT-SESSION-001` committed
at Go `96ffa15e5719d00bb14eca116ff9e6cb9e5afead` and routes
`PERSIST-P2-SOURCE-PRECEDENCE-001` Active. P2 and the persistent Goal remain In
progress; no phase or Goal completion is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress with twelve registered children: nine
  Complete and three unfinished.
- P2 remains scope-frozen/In progress with eight children: five Complete, one
  Active, and two Ready. The active source-precedence child is dependency-ready;
  character metadata and storage/NPC remain dependency-blocked.
- Go matrix currently marks account-session Complete and source-precedence
  Active. The imported retained-gap version-117 header counter remains the
  finite P12 leaf `PERSIST-P12-ACCOUNT-ID-001` and is not claimed here.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 438]`; observed HEAD
  `65e035a06a5faab662e2987c182e1682fe407a8b`
  (`docs(migration): close P2 discovery and select auth`).
- Tracked modifications are exactly `tasks/lessons.md`,
  `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/migration-active.md`, and this handoff. Index and untracked set are
  empty.
- The active lesson strengthens mechanical `rg` argv and UUID writer-lock
  predicates. The archive records account-session review findings and fixes.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- No C# file is owned; tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; observed clean HEAD
  `96ffa15e5719d00bb14eca116ff9e6cb9e5afead`
  (`feat(auth): preserve legacy account sessions`).
- Worktree, index, and untracked set are empty. The commit owns the five
  account-session code/test files plus matrix evidence.
- No C# file is owned; tracked, staged, and untracked C# gates are empty.

## Active leaf and protected work

- Active leaf: `PERSIST-P2-SOURCE-PRECEDENCE-001`.
- Outcome: prove JSON account/global state wins over a conflicting configured
  version-117 source, and that graceful checkpointing writes only the
  authoritative JSON-backed state for direct-binary reload.
- Matrix anchors: the P2 stage row and exact source-precedence child row only.
- Legacy read authority: the frozen version-117 account format; every C# file is
  read-only.
- Go write authority: new `cmd/crystal-server/p2_account_precedence_test.go`;
  bounded startup code only if the production transcript fails; matrix evidence.
- Forbidden: reopening account-session behavior, P12 header-counter/global
  recovery, character metadata, storage/NPC work, broad matrix/source dumps,
  destructive Git, and every C# write.
- Required gate: conflicting nonzero JSON/binary sentinels through
  `runServerWithContext` startup/login/checkpoint/direct-binary reload, repeated
  test, compile, diff/status/process, and all six C# gates.

### Completed account-session commit

- TCP NewAccount now uses Legacy Unicode-unanchored email validation, unchecked
  account index allocation, creation IP/time, and all result codes 0..8.
- Login stamps LastIP/LastDate, preserves case-insensitive identity, exact
  localized sixth-failure/strict expiry behavior, all results, and keeps active
  bans reusable on the same connection. ChangePassword locks results 0..6 and
  active/equality expiry behavior.
- Wrong-stage NewAccount/Login/ChangePassword are silently ignored without
  closing the connection.
- Listener-owned claim authority sends prior same-account sessions disconnect
  reason 1 before LoginSuccess; identity-guarded release survives old cleanup,
  proven by a third-session replacement and a real accept-loop transcript.
- JSON and version-117 checkpoint reload preserve creation/login metadata and
  authentication. Retained-gap header continuity remains explicitly P12.

## Verification ledger

All commands below exited 0 after the final code changes:

- `gofmt -w` on the five owned changed/new Go files.
- Touched compile:
  `go test ./internal/auth ./cmd/crystal-server -run '^$' -count=1`.
- Auth focused ordinary `-count=10` and focused race `-count=3`, covering login
  validation/metadata/localized ban, strict ChangePassword expiry, and account
  creation metadata/counter cases.
- `go test ./cmd/crystal-server -run '^TestP2AccountSession' -count=10
  -timeout=10m` and the same focused set under race at `-count=3`.
- `go test ./... -count=1 -timeout=20m` (fresh, unexcluded).
- `go test -race ./... -count=1 -timeout=30m` (fresh, unexcluded).
- `go vet ./...` and `go build ./...`.
- `git diff --check`; both repositories' exact status and all six C# gates.

No final run has exclusions or failures. Earlier hard-gate command-predicate
failures are recorded in the active lesson/current diffs; they caused no Go
write or test attribution.

## Subagents and quiescence

- Test writer `01a031e5-b675-70a2-a761-052672545a3f` (`luna_worker`) changed
  only `cmd/crystal-server/p2_account_session_test.go`; its compile/focused
  checks passed and the thread is closed.
- Read-only reviewer `01a031f8-b7cb-7c70-b729-bdb23b11b362` returned PASS. Its
  production-wiring/claim-identity, full result-code, and expiry-boundary gaps
  were strengthened; its version-117 counter finding remains correctly routed
  to P12. It changed no files and is closed.
- No subagent result is pending. Exact process inspection must remain free of
  `go`, `crystal-server`, and `git`; only the main UUID writer lock may remain
  besides `.coordination.lock`.

## Exact recovery sequence

1. Verify each repository separately against the exact HEAD/status/file lists
   above; do not discard or overwrite any owned uncommitted work.
2. Run final Legacy control/diff/status/process/C# gates and commit the four
   owned Legacy docs. Go must remain clean at `96ffa15e5719`.
3. Read only the P2 stage row and `PERSIST-P2-SOURCE-PRECEDENCE-001` row, search
   the archive with source-precedence/startup/checkpoint keywords, and trace only
   the relevant Go startup/load/register order.
4. Add the focused failing production transcript in
   `cmd/crystal-server/p2_account_precedence_test.go`; change startup code only
   if that transcript proves a source-order defect, then run its leaf gate.

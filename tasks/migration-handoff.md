# Crystal Go migration current handoff

Last updated: 2026-08-24 04:12 (Asia/Singapore)

This replace-in-place snapshot closes the committed LOG leaf and routes the
next bounded P1 leaf. The Goal remains active.

## Goal and control-plane state

The persistent full Go migration Goal is active, not Complete or externally
Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers use
`luna_worker` (`gpt-5.6-luna/max`). P1 is scope-frozen/In progress with twelve
registered children: five Complete and seven unfinished.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before the enclosing route-reconciliation commit:
  `master...origin/master [ahead 425]`.
- HEAD before the enclosing control commit:
  `381fa931167e9a9c4f1d564b81c004445ec0b693`
  (`docs(migration): complete P1 logging categories`).
- Tracked modifications are exactly `tasks/lessons.md` and this handoff after
  replacement; expected subject is `docs(migration): reconcile P1 network route`.
- Staged and untracked files: none.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode; all tracked,
  staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `1878cad4b9e1e91214d7756c767180dc45db49ac`
  (`docs(migration): activate P1 network gates`).
- Worktree, staged files, and untracked files are clean.
- All tracked, staged, and untracked C# gates are empty.

## Completed LOG leaf

- Legacy locks Server/Chat/Debug/Player/Spawn categories; Info/Debug levels;
  local `dd-MM-yyyy` category names; append/roll; log4net ISO8601
  comma-millisecond layouts; Server/Chat/Debug non-evicting 100-entry queues;
  post-saturation file writes; and Player/Spawn file-only behavior.
- Production shares one manager from startup through accepted sessions. Listener
  startup uses Server queue/file; normal chat uses Chat queue/file; successful
  market search uses Debug queue/file; StartGame writes exact Player Connected;
  world-import spawn load writes Spawn file-only.
- Sink failures cannot recurse through the installed standard logger. Queue
  admission and unrelated categories continue, the failed category reopens on a
  later write, and close is idempotent. No unrelated server lock crosses I/O.
- Matrix LOG is Complete; the exact twelve-row registry now counts five Complete
  and seven unfinished. Earlier prose had failed to decrement after LOC and is
  corrected rather than mechanically subtracting from the stale count.

## Verification ledger

All commands below ran in the Go root and exited 0:

- `gofmt -w internal/logging/logging.go internal/logging/logging_test.go
  cmd/crystal-server/main.go cmd/crystal-server/runtime_logging.go
  cmd/crystal-server/runtime_logging_test.go`.
- `go test ./internal/logging ./cmd/crystal-server -run '^$'`.
- `go test ./internal/logging -count=20`.
- Runtime production-category/startup/session tests at `-count=20`.
- Existing chat, market-search, and startup-error production entries at
  `-count=10`.
- `go test -race ./internal/logging -count=5` and runtime production logging
  race tests at `-count=10`.
- Fresh unexcluded `go test ./... -count=1`; no flake reproduced.
- Fresh unexcluded `go test -race ./... -count=1`.
- `go vet ./...`, `go build ./...`, and `git diff --check`.
- Both repositories' tracked/staged/untracked C# gates are empty.

## Active leaf and protected work

- Active leaf: `NET-P1-GATES-001`; the matrix and Active Index now both say
  Active. NET writes remain gated until the expected Legacy correction is clean.
- Outcome: timed IP blocking, MaxUser/MaxIP admission/release, idle read timeout,
  MaxPacket reset/rejection, and exact disconnect/log boundaries.
- Matrix anchors: P1 finite inventory, `NET-P1-GATES-001`, and the P1 stage row.
- Legacy read authority: bounded gate ranges in `Server/MirEnvir/Envir.cs`,
  `Server/MirNetwork/MirConnection.cs`, and `Server/Settings.cs`, plus direct
  timer/admission consumers only.
- Go write authority after the gate: bounded `cmd/crystal-server/main.go` and
  `main_test.go`, plus new `connection_gates.go`/`connection_gates_test.go`.
- Forbidden: status port 3000, HTTP, lifecycle redesign, broad logging closure,
  localization, unrelated persistence/protocol, and C#.

## Quiescence

- `luna_worker` `01a0302d-1068-7ad2-91e6-d4eb449599e0` is completed and closed.
- No subagent result is pending; exact process audit found no `go` or
  `crystal-server` process.

## Exact recovery sequence

1. Run `tasks/check-migration-control.sh`, verify both repositories and all six
   C# gates separately, then commit only `tasks/lessons.md` and this handoff with
   the expected route-reconciliation subject.
2. Verify the expected one-commit Legacy delta and both clean worktrees; refresh
   this handoff only if unexpected state remains.
3. Once both worktrees are clean, read only the named NET anchors, search the
   archive for IPBlocks/MaxUser/MaxIP/MaxPacket/timeout lessons, and trace the
   bounded Legacy/Go gate authorities before any NET write.

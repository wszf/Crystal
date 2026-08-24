# Crystal Go migration current handoff

Last updated: 2026-08-25 06:14 (Asia/Singapore)

This replace-in-place snapshot records committed closure of
`CHAR-P3-START-LOGOUT-001` and routing to the sole Active leaf
`CLIENT-P3-SELECT-PROBE-001`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `luna_worker` (`gpt-5.6-luna/max`). Read-only reviewer
  `01a035c4-310d-74b0-bd55-53fca578f87d` did not return after a bounded
  conclude request and was closed without a usable report. No subagent or Go/
  crystal-server process remains active.
- P1/P2/P3 remain scope-frozen and In progress. P3 has ten Complete children
  and the unique Active child `CLIENT-P3-SELECT-PROBE-001`.
- `CHAR-P3-START-LOGOUT-001` is Complete at Go
  `2cb8a85baa04b50b6a604676c09cf11c2aba0261`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed HEAD:
  `dc3a8cc1766656cbf6713112a7d2183f58bbd8b2`
  (`docs(migration): close start logout leaf`).
- Worktree, index and untracked set were empty immediately before this
  handoff-only refresh. Recovery therefore expects exactly one documentation
  commit delta from the observed HEAD and must compare the actual HEAD/status.
- `tasks/check-migration-control.sh` and `git diff --check` exited 0; all Legacy
  C# gates were empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `2cb8a85baa04b50b6a604676c09cf11c2aba0261`
  (`migrate character start logout lifecycle`).
- Worktree, index and untracked set are empty. `git diff --check`, repository
  lock, process audit and tracked/staged/untracked C# gates were empty.
- The commit imports source-order StartPoint/NoReconnect map authority; restores
  level-zero initialization, HP-zero bind/PK-town reset, current/bind/StartPoint
  resolution and exact NoReconnect culture/no-trim/random behavior; installs
  world/Game stage before Result 4; preserves Result 3 retry; and completes
  explicit logout combat, guild/lover, ranking, visibility, metadata/runtime
  persistence and same-connection re-entry behavior.

## Active leaf and protected work

- Active leaf: `CLIENT-P3-SELECT-PROBE-001`.
- Read only its exact P3 finite-inventory row, exact P3 stage row, and evidence
  headings/tests named by that row. Do not read the full matrix.
- Required outcome: a Go-only test-client/probe transcript covering
  LoginSuccess animation boundary into Select, complete select metadata,
  New/Delete local-list effects, StartGame success/failure transition, LogOut
  success/failure return and full connection-lifetime ordering.
- Legacy read authority is not yet traced: bound it to exact select-scene packet
  consumers and serializers before writes. Every `.cs` file remains read-only.
- Tentative Go write authority is closed until tracing: bounded
  `internal/probe` plus focused production session tests; refine exact paths in
  the active index before modifying Go.
- Forbidden scope: native client UI/settings/patcher, P1 version handshake, P2
  account-auth UI, no-op Credits, no-producer StartGameDelay/RelogDelay, broad
  dumps, completed P3 domain behavior and all C# writes.

## Verification ledger

- Touched compile:
  `go test ./cmd/crystal-server ./internal/auth ./internal/legacyworld ./internal/worlddata -run '^$' -count=1`
  exited 0.
- Focused production/schema suites passed at `-count=10`/`20`; focused race
  passed at `-count=3`. Focused persistence/restart suites passed ordinary
  `-count=5`/`10` and race `-count=3`.
- The first fresh `go test ./... -count=1 -timeout=20m` exited 1. Three guild
  transcripts omitted Legacy `GuildMemberChange(offline) -> ObjectRemove`, and
  the utility fixture registered map 42 while leaving characters at invalid
  default map 0, producing StartGame Result 3. These were candidate-caused test
  assumptions, not unrelated flakes.
- After correcting those exact fixtures, the four failed tests passed
  `-count=10`; fresh unexcluded `go test ./... -count=1 -timeout=20m` exited 0.
- Fresh `go vet ./...`, `go build ./...`, and
  `go test -race ./... -count=1 -timeout=30m` each exited 0.
- Final Go diff/status/C#/process gates were clean before commit. Legacy control,
  diff and C# gates exited 0 before this rewrite. No failure is excluded from a
  claimed full pass.

## Exact recovery sequence

1. In Legacy only, run `tasks/check-migration-control.sh`, `git diff --check`,
   exact status/lock and all three C# gates; verify these four files and this
   handoff.
2. In Go only, verify clean HEAD
   `2cb8a85baa04b50b6a604676c09cf11c2aba0261`, exact status/lock/process,
   `git diff --check` and all three C# gates.
3. Read only the `CLIENT-P3-SELECT-PROBE-001` matrix row, exact P3 stage row and
   named evidence. Search archive by leaf ID, SelectScene, LoginSuccess,
   NewCharacter, DeleteCharacter, StartGame, LogOut and probe/transcript.
4. Trace exact Legacy consumers and current Go probe/session coverage, refine
   finite missing behavior and exact write authority in the active index, then
   implement the smallest Go-only transcript and run its leaf gate.

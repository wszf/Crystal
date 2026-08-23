# Crystal Go migration current handoff

Last updated: 2026-08-24 06:02 (Asia/Singapore)

This replace-in-place snapshot closes `NET-P1-STATUS-001`, activates the bounded
HTTP-service leaf, and keeps the persistent full migration Goal active.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 is scope-frozen/In progress with twelve registered children: seven
  Complete and five unfinished. Matrix and Active Index route
  `NET-P1-HTTP-001` Active.
- No phase or Goal closure is claimed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before this control commit: `master...origin/master [ahead 430]`.
- HEAD before this control commit:
  `098863a7c38fd2fb4d163a8aa79c555028c19a9d`
  (`docs(migration): complete P1 network gates`).
- Expected owned delta is `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`, `tasks/migration-active.md`, and this handoff.
  Index and untracked files were otherwise empty before this snapshot.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- Tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `269590b97f8f4f37d2387541a69981783fcc548b`
  (`feat(network): restore Legacy status monitor`).
- Worktree, index, and untracked set are clean.
- Tracked, staged, and untracked C# gates are empty.

## Completed status-monitor leaf

- Production opens the configured game listener first, completes bootstrap, then
  requests a second listener on exact `cfg.IPAddress` authority and fixed port
  3000. Bind failure is returned and the game listener is closed.
- The service admits five active clients before re-arming accept, releases
  capacity on timeout/write/disconnect, never reads peer input, and closes the
  listener and every client idempotently under context/StopNetwork shutdown.
- Wire output is exact no-terminator ASCII
  `c;/NoName/<current players>/CrystalM2/1.0.0.0//;`; count is sampled per send.
- The initial zero timer, first positive millisecond, strict ten-second equality
  and next-millisecond send, actual-process-time re-arm, absolute non-refreshing
  configured timeout, and timeout-before-send order match Legacy.
- A read-only reviewer found one missing production-entry proof. The main agent
  extracted the real runtime listener-opener seam and added a game-first/fixed-
  status-address/closure test; the same reviewer confirmed the finding resolved.

## Verification ledger

All final commands below ran in the Go root and exited 0:

- `gofmt -w cmd/crystal-server/main.go cmd/crystal-server/status_service.go cmd/crystal-server/status_service_test.go`.
- `go test ./cmd/crystal-server -run '^$'`.
- Focused status/runtime/TCP regex at `-count=20 -timeout=5m`.
- The same focused set under `go test -race` at `-count=5 -timeout=5m`.
- `go test ./... -count=1 -timeout=15m` (fresh, unexcluded).
- `go test -race ./... -count=1 -timeout=20m` (fresh, unexcluded).
- `go vet ./...`, `go build ./...`, staged `git diff --check`, exact status,
  and all six C# gates.

No failed or excluded final run remains. The real-TCP cap/release test and the
production runtime entry are included in both repeated and focused-race sets.

## Active leaf and protected work

- Active leaf: `NET-P1-HTTP-001`.
- Outcome: optional trusted-IP HTTP startup/stop plus exact root, new-account,
  add-name-list, broadcast, unknown-path, malformed/error, and POST behavior.
- Matrix anchors: P1 finite inventory, `NET-P1-HTTP-001`, and P1 stage row.
- Legacy read authority: bounded `Server/Utils/HttpService.cs`,
  `Server/Utils/HttpServer.cs`, and only direct `Envir` lifecycle and endpoint
  account/chat/name-list consumers.
- Go write authority: bounded `cmd/crystal-server/main.go`, plus new
  `http_service.go` and `http_service_test.go`.
- Forbidden: status/game-gate redesign, broad lifecycle, UI, unrelated
  persistence/protocol, logging/localization closure, and C#.
- Unresolved decisions: exact trusted-IP derivation, request parsing/response
  bytes and status codes, endpoint side-effect authority/order, and stop/error
  behavior must be ruled from the bounded Legacy chain before code changes.

## Quiescence

- Read-only STATUS tracer `01a0307c-edf8-7ef2-97fc-cb1c20bc735a`, bounded
  service writer `01a03084-ce0d-72b3-9acd-7190aa4dd194`, and read-only reviewer
  `01a03090-996f-7571-9039-0b369ac7f2e4` completed and were closed.
- No subagent result is pending. No `go` or `crystal-server` process remains.

## Exact recovery sequence

1. Recheck control schema, Legacy diff/C# gates, and commit only the four owned
   Legacy documentation files.
2. Read only the named HTTP matrix anchors and bounded Legacy HTTP files/ranges;
   search the lessons archive with `NET-P1-HTTP-001`, `HttpService`,
   `HttpServer`, trusted IP, endpoint, POST, and shutdown.
3. Delegate one bounded read-only Legacy/Go comparison, derive the finite
   request/response/side-effect/lifecycle checklist, then create only
   `http_service.go` and `http_service_test.go` before bounded `main.go` wiring.
4. Run focused/repeated/race and the standard Leaf/integration gate; update
   matrix/index/handoff and commit each repository separately.

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
- Branch before this control commit: `master...origin/master [ahead 431]`.
- HEAD before this control commit:
  `3b16e2bbefdc351a2037285d2639307c3836dda5`
  (`docs(migration): complete P1 status monitor`).
- Expected owned delta is `tasks/lessons.md`, `tasks/migration-active.md`, and this handoff.
  Index and untracked files were otherwise empty before this snapshot.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- Tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `88b0e15771a909c18c64dd4040c12264251f5349`
  (`docs(migration): bound P1 HTTP auth adapter`).
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
- Go write authority: bounded `cmd/crystal-server/main.go`, new
  `http_service.go`/`http_service_test.go`, and bounded
  `internal/auth/service.go`/`service_test.go` account metadata adapter.
- Forbidden: status/game-gate redesign, broad lifecycle, UI, unrelated
  persistence/protocol, logging/localization closure, and C#.
- Unresolved decisions now narrow to framework-dependent default headers,
  escaping/duplicate-query behavior, POST connection lifetime, and cross-
  platform rooted-path/newline projection. The source-ruled request bodies,
  metadata, side effects, recipient order, and lifecycle are finite.

## Quiescence

- Read-only STATUS tracer `01a0307c-edf8-7ef2-97fc-cb1c20bc735a`, bounded
  service writer `01a03084-ce0d-72b3-9acd-7190aa4dd194`, and read-only reviewer
  `01a03090-996f-7571-9039-0b369ac7f2e4` completed and were closed.
- No subagent result is pending. No `go` or `crystal-server` process remains.

## Exact recovery sequence

1. Recheck control schema, both worktrees, and all six C# gates; commit only the
   updated active lesson, Active Index, and this handoff.
2. Implement the already traced finite HTTP contract in the newly authorized
   files; keep framework-dependent behavior explicit in tests and evidence.
3. Review the bounded auth metadata adapter and HTTP service before production
   `main.go` wiring; do not expand into P2-P4 feature redesign.
4. Run focused/repeated/race and the standard Leaf/integration gate; update
   matrix/index/handoff and commit each repository separately.

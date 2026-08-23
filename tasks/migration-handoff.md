# Crystal Go migration current handoff

Last updated: 2026-08-24 07:49 (Asia/Singapore)

This replace-in-place snapshot closes committed `NET-P1-HTTP-001`, routes
`OPS-P1-LIFECYCLE-001` Active, and keeps the persistent full migration Goal
active.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 is scope-frozen/In progress with twelve registered children: eight
  Complete and four unfinished. Matrix and Active Index consistently route
  `OPS-P1-LIFECYCLE-001` Active.
- Imported version-117 account databases whose persisted `NextAccountID` is
  higher than every retained record are registered as finite P12 finding
  `PERSIST-P12-ACCOUNT-ID-001`; this is not hidden P1 HTTP work.
- No phase or Goal closure is claimed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before this snapshot commit: `master...origin/master [ahead 433]`;
  HEAD before this snapshot commit:
  `1abf30d2913cdf86381757fc379acfebd5317e5e`
  (`docs(migration): close P1 HTTP leaf`).
- Expected owned delta is this handoff only. Index and untracked set were empty
  before this snapshot.
- `tasks/check-migration-control.sh` and `git diff --check` pass.
- `AGENTS.md` and tracked `agents.md` remain the established hard-linked pair.
- Tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `dbbe12e8f53f944250a1bef52ba4a1446e491622`
  (`feat(server): restore legacy HTTP service`).
- Worktree, index, and untracked set are clean.
- Committed HTTP SHA-256 fingerprints are:
  `main.go` `5ee38399a0533fb3be6e7008238c0fd966e406da10ea23f6bf3a92e36173c892`;
  `http_service.go` `10b44f1d7f4d4e8fbf6767773e2b0832a96ff805b1fd0d98a1f3ea3dc8151105`;
  `http_service_test.go` `0f050bd0219c5dcbd8b4697a049140fa48fb492746f910c685c3693dabc0b65e`;
  `service.go` `7c61baf43dcdb75d3c8e4a13ebaa7225dd60c3c3ff13c6fb8e75c1ef7fc90e26`;
  `service_test.go` `9ae4faae4a6e02b8102dfc9ff84468b091d87e29c8cf4eedb0b391bdbe38a4e6`;
  matrix `004eda9a929f8ae7dce80468fb2bccabe88bbbc3469df9be2ac5138504c55125`.
- Tracked, staged, and untracked C# gates are empty.

## Completed HTTP leaf

- Production opens optional HTTP only after game/status bootstrap, preserves
  the configured host/port/path prefix and wildcard-port authority, logs
  malformed/bind failures without aborting the game process, and closes the
  service before shutdown persistence.
- Request handling locks trusted remote IP, one-at-a-time dispatch, query-only
  CurrentCulture lowercasing, case-insensitive duplicate query values in source
  order, root/new-account/add-name-list/broadcast/unknown/error behavior, every
  non-GET POST hang, disconnected POST cleanup, and active-handler closure.
- Responses lock UTF-8 byte Content-Length, exact Content-Type, default Windows
  HTTP.sys Server, Date, fixed bodies/error prefix, and source-equivalent
  framework-dependent exception suffix projection.
- Name lists lock CRLF append, missing/duplicate/case/no-newline/empty-ID,
  traversal/rooted and Windows separator authority, UTF-8 BOM and UTF-16 reads.
- HTTP accounts lock unanchored Unicode email matching, creation IP/UTC binary
  time, unchecked monotonic counter through failures/concurrency/gaps/JSON
  restart, result 8/7, pre-shutdown disk invisibility, and JSON plus version-117
  checkpoint reload. Existing non-HTTP account creation remains P2 authority.
- Production Shout2 reaches every world player in stable order and continues
  best-effort after a disconnected recipient, matching Legacy enqueue behavior.

## Verification ledger

All final commands below ran in the Go root and exited 0:

- `gofmt -w` on the five owned changed Go files.
- `go test ./internal/auth ./cmd/crystal-server -run '^$'`.
- Focused HTTP/auth/production regex at `-count=20 -timeout=12m`.
- The same focused set under `go test -race` at `-count=5 -timeout=12m`.
- `go test ./... -count=1 -timeout=15m` (fresh, unexcluded).
- `go test -race ./... -count=1 -timeout=20m` (fresh, unexcluded).
- `go vet ./...`, `go build ./...`, `git diff --check`, exact status, process
  quiescence, and all six C# gates.

No excluded or failed final run remains. During development, compile gates
caught missing imports/helper definitions, the first blocked-handler test
mistook transport EOF for callback failure, and one partial prefix fixture
omitted its port. Each symptom/root/prevention/verification is recorded in the
active or feature archive lesson and the corrected tests pass repeatedly/race.

## Active leaf and protected work

- Active leaf: `OPS-P1-LIFECYCLE-001`.
- Outcome: occupied-port/startup/work-loop failures, Stop/Start/Reboot,
  listener/session/service closure order, persistence boundaries, and
  operator-visible lifecycle state without pixel-identical WinForms UI.
- Matrix anchors: P1 finite inventory, `OPS-P1-LIFECYCLE-001`, and P1 stage row.
- Legacy read authority: bounded `Server/MirEnvir/Envir.cs` WorkLoop/Start/Stop,
  StartNetwork/StopNetwork and direct `Server.MirForms/SMain.cs` controls.
- Go write authority: bounded `cmd/crystal-server/main.go`, `main_test.go`, and
  new `process_lifecycle_test.go`.
- Forbidden: HTTP/status/game-gate redesign, pixel-identical UI, deployment,
  unrelated persistence/protocol, call-site closure, and every C# file.

## Quiescence

- Legacy tracer/reviewer `01a030cb-fb68-77b1-966a-588b957048bf`, Go reviewer
  `01a030cc-2d8b-74a0-8457-330d4689046a`, and final re-reviewer
  `01a030e7-429d-7110-aace-aa4acd9774bb` completed and were closed.
- No subagent result is pending. Exact process inspection found no `go` or
  `crystal-server` process.

## Exact recovery sequence

1. Search the archive for OPS/process/start-stop/reboot lessons and read only
   matching sections.
2. Trace the bounded Legacy lifecycle authority and current Go production
   entry, then implement the active leaf without reopening HTTP or the frozen
   P1 inventory.

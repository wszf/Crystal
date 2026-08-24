# Crystal Go migration current handoff

Last updated: 2026-08-24 10:33 (Asia/Singapore)

This replace-in-place snapshot closes committed `OPS-P1-LIFECYCLE-001`, routes
`DISC-P2-CLOSURE` Active, and keeps the persistent full migration Goal active.
No phase or Goal closure is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress with twelve registered children: nine
  Complete and three unfinished. The remaining call-site/deployment children
  are dependency-blocked, so the next useful leaf is bounded P2 discovery.
- Active Index and the Go matrix consistently mark lifecycle Complete and route
  `DISC-P2-CLOSURE` Active against the P2 stage row only.
- `BOOT-P4-STARTPOINT-001`, `PERSIST-P12-CANSTART-DBCHECKS-001`, and
  `PERSIST-P12-ACCOUNT-ID-001` are finite cross-phase inputs, not hidden P1/P2
  implementation permission.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 434]`; observed HEAD:
  `7402fa7759a23c255fecdc2a068b3f8d276c7462`.
- Tracked modifications are exactly
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`,
  `tasks/lessons.md`, `tasks/migration-active.md`, and this handoff. The index
  and untracked set are empty.
- The lesson deltas preserve lifecycle fixture/review failures and strengthen
  recurring path/argv/output prevention. The Active Index closes OPS and
  selects the P2 discovery leaf.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- `tasks/check-migration-control.sh` and `git diff --check` pass.
- Tracked, staged, and untracked C# gates are empty. No C# file is owned.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; observed clean HEAD:
  `cb595b4e40aeffa9f94e22d2deddaeb9abc7b75f`
  (`feat(server): restore legacy process lifecycle`).
- Worktree, index, and untracked set are empty; all three C# gates are empty.
- The commit owns `cmd/crystal-server/main.go`, `main_test.go`, new
  `process_lifecycle_test.go`, and bounded P1/cross-phase matrix evidence.
- Production locks operator Start/Stop/Reboot/SIGHUP state, bootstrap-before-
  bind and occupied-port quirks, exact lifecycle messages, HTTP→game→status→
  persistence shutdown, reason-0 normal fan-out, and stable bounded reason-3→
  listener-close→reason-0 fatal ordering under concurrent cancellation.
- Final accepted-client reason 0 uses a fresh post-handoff snapshot; HTTP and
  session joins preserve Legacy's one-second/five-second bounds, while the
  top-level Stop join intentionally remains synchronous until WorkLoop ends.
- `CanStartEnvir` start-point and DB-catalog authorities are registered as
  `BOOT-P4-STARTPOINT-001` and `PERSIST-P12-CANSTART-DBCHECKS-001`.

## Active leaf and protected work

- Active leaf: `DISC-P2-CLOSURE`.
- Outcome: convert every vague P2 account/login/password/storage-password,
  static NPC-access, and restart residual into finite children with exact
  completed evidence, dependencies, ownership, and gates.
- Matrix anchors: the row beginning `| P2 |` only; locate additional P2-specific
  headings/rows by exact terms without reading the full matrix.
- Legacy read authority: phase-local real account/login/password/storage-
  password, account/character metadata, static NPC-access, and restart entry
  points discovered from exact P2 terms. Every C# file remains read-only.
- Go write authority: `docs/migration-matrix.md` P2 inventory/status only;
  Legacy Active Index/current handoff are main-agent control.
- Forbidden: implementation code, P3-P12 inventory, broad matrix/source dumps,
  deployment, P1 call-site closure, destructive Git, and every C# write.
- Required output: a reviewed finite P2 child registry, explicit completed and
  deferred evidence, scope-frozen ruling, and one dependency-ready next child.

## Verification ledger

All final Go commands below ran after the last production/test change and exited
0:

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

Read-only reviewer `01a0316f-c892-7883-8fad-b0f770dd1476` raised bounded fatal/
concurrent Stop issues across two rounds. After descriptor, reason-3 barrier,
producer lifetime, fresh reason-0 snapshot, HTTP/session bound, and regression
fixes, its final review found no remaining P1/P2 correctness issue and declared
the leaf safe to close. It changed no files and was closed.

## Quiescence

- No subagent result is pending and no writing agent is active.
- Exact process inspection in each repository found no `go` or
  `crystal-server` process.

## Exact recovery sequence

1. Verify each repository separately; Go must remain clean at `cb595b4e40ae` and
   Legacy must contain only the four owned control/lesson modifications above.
2. Read only the P2 stage row, then search the archive for P2 plus exact account,
   login, password, storage, NPC-access, and restart terms.
3. Trace phase-local Legacy and Go entry points, reconcile each P2-row clause,
   and register every missing behavior as one finite child without code writes.
4. Obtain one bounded independent read-only review; if complete, mark P2 scope
   frozen, select one dependency-ready child, refresh this snapshot, and commit
   only owned control/matrix files after control and C# gates pass.

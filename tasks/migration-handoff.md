# Crystal Go migration current handoff

Last updated: 2026-08-24 11:00 (Asia/Singapore)

This replace-in-place snapshot records the first bounded P2 discovery wave and
the real-compaction safety boundary. `DISC-P2-CLOSURE` remains Active; P2 is not
yet scope-frozen, and no phase or Goal closure is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress with twelve registered children: nine
  Complete and three unfinished. The remaining call-site/deployment children
  are dependency-blocked, so the next useful leaf is bounded P2 discovery.
- Active Index and the Go matrix consistently route `DISC-P2-CLOSURE` Active
  against the P2 stage row only. The row remains `In progress` and includes
  account metadata, storage-NPC gates, JSON precedence, NPC-script access, and
  broader restart residuals.
- `BOOT-P4-STARTPOINT-001`, `PERSIST-P12-CANSTART-DBCHECKS-001`, and
  `PERSIST-P12-ACCOUNT-ID-001` are finite cross-phase inputs, not hidden P1/P2
  implementation permission.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before this snapshot commit: `master...origin/master [ahead 436]`;
  HEAD before this snapshot commit:
  `20c3232a4ed6b8b08c29d8a5fd7116d4a77a0051`
  (`docs(migration): checkpoint P2 discovery route`).
- The worktree, index, and untracked set were empty before this handoff-only
  refresh; its one expected documentation commit delta is the current snapshot.
- The committed Active Index selects the P2 discovery leaf. No P2 matrix or
  production file has yet changed in this discovery cycle.
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
- Main-agent tracing verified one concrete finite gap, provisionally named
  `AUTH-P2-METADATA-001`: TCP NewAccount uses metadata-free `CreateAccount`,
  character creation records no creation IP/time, successful TCP login does not
  stamp account LastIP/LastDate, and StartGame stamps character login date but
  not the Legacy login IP fields. The final child must include account index,
  ban transcript, explicit/disconnect/game-to-observer logout timestamps, JSON
  plus 117 persistence, and reload evidence without duplicating P3 selection.
- Still requiring main-agent ruling before scope freeze: whether storage wrong-
  key/map/distance/stale-NPC coverage is one finite P2 gate child; whether the
  implemented JSON-over-binary branch needs a production precedence leaf;
  routing the broad reachable NPC-script corpus to P7 rather than hiding it in
  P2; and routing broader runtime restart equivalence to its dependency-owning
  phases/P12. These are candidates, not registered children yet.
- Completed evidence identified by the Go auditor includes login/password/hash,
  storage-password service/protocol, SelectInfo/LoginBanned serializers,
  metadata round trips, 117/0 import/export/checkpoint, direct-binary account/
  character restart, and global-section merge behavior. Main-agent exact-path
  reconciliation and finite registry prose remain unfinished.

## Verification ledger

No tests, builds, formatters, or code writes ran during this P2 discovery wave.
Read-only source/status commands in each repository exited 0. The last committed
Go code candidate retains these final exit-0 lifecycle gates:

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

The current discovery has no failure attribution because no test was run. The
previous lifecycle development failures and unrelated OmaMage attribution are
already committed and remain summarized above.

## Quiescence

- Both P2 read-only workers completed. Explicit close checks now report both
  agent IDs absent, and no writing agent was used.
- Exact `comm`-based process inspection found no `go` or `crystal-server`
  process. Both repositories were stable and clean before this snapshot edit.

## Exact recovery sequence

1. Verify each repository separately; Go must remain clean at `cb595b4e40ae`.
   Legacy must be clean at exactly one handoff-only documentation commit after
   recorded pre-snapshot HEAD `20c3232a4ed6`.
2. In Legacy, run the bounded archive search for `DISC-P2`, account/login/
   password/storage, NPC access, metadata timestamps, precedence, and restart;
   read only matching sections.
3. Reconcile the two completed worker reports against exact phase-local entry
   points, first finishing `AUTH-P2-METADATA-001`, then ruling the storage-NPC,
   precedence, NPC-script, and restart candidates without code writes.
4. Write the finite P2 child registry to the P2 matrix anchor, obtain one final
   bounded read-only review, and only then scope-freeze P2/select one dependency-
   ready child. Refresh Active Index/handoff and commit owned docs after control,
   diff, status, process, and all six C# gates pass.

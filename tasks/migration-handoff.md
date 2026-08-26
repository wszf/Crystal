# Crystal Go migration current handoff

Last updated: 2026-08-26 22:40 (Asia/Singapore)

This replace-in-place snapshot records the clean Revelation refresh closure and
routes the next bounded SafeZoneHealing trace. It is the current recovery
authority, not a historical journal.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; it is neither
  Complete nor Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers
  use `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- Active leaf: `REGEN-P5-SAFEZONE-001`.
- P5 is scope-frozen with thirteen of sixteen children Complete, this leaf
  Active and two Ready. `STATE-P5-REVELATION-REFRESH-002` is committed Complete
  in Go `602c71055c8f417b182931e7292a494017a8522c`; Legacy routing/evidence is
  committed in `8540a5a5933b102e9bcb0c8189e97b6bb8626fc7`.
- Normal recovery reads only matrix P5 row 851, registry rows 3167-3168 and
  completed refresh evidence 3231-3248.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `8540a5a5933b102e9bcb0c8189e97b6bb8626fc7`;
  upstream `origin/master`.
- Worktree, index and untracked set were clean before this self-recording
  handoff edit. Its documentation commit is the one expected delta above the
  observed HEAD. The active index routes SafeZoneHealing and records Revelation
  refresh Complete. Every tracked, staged and untracked `.cs` gate is empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `602c71055c8f417b182931e7292a494017a8522c`;
  no upstream is configured.
- Worktree, index and untracked set are clean. Commit `602c710` contains the
  shared helpers, 43 exact producer files, static/authenticated regressions and
  the matrix completion/next-leaf route. Every tracked, staged and untracked
  `.cs` gate is empty.

## Active leaf and protected work

- Active leaf: `REGEN-P5-SAFEZONE-001`
- Initial authority is read-only tracing plus control/matrix routing. Before any
  production write, enumerate and freeze exact Go config/map/healing files.
- Required behavior is default/disabled no-op versus enabled permanent
  value-25/two-second Healing SpellObjects on every valid safe-zone cell,
  flowing through the existing Player/Monster/Hero HealAmount path with exact
  startup, cell, visibility, tick and restart behavior.
- Completed Revelation expiry/refresh, Human regen, other P5 children and every
  `.cs` file are protected.

## Verification ledger

- Owned Go files are gofmt-clean; touched compile
  `go test ./cmd/crystal-server -run '^$'` exits 0.
- Final focused repeated command selects the eight refresh/static/session tests,
  including all eight TucsonEgg miss adapters, with `-count=20 -timeout=10m`;
  exit 0 in 56.579s.
- The same exact focused selection under `go test -race`, `-count=5` and
  `-timeout=15m` exits 0 in 49.713s.
- Fresh final integration: `go test ./... -count=1 -timeout=30m` exits 0;
  command package 83.222s. `go vet ./...` and `go build ./...` exit 0.
- Fresh final full race `go test -race ./... -count=1 -timeout=45m` exits 0;
  command package 97.905s.
- Read-only Luna reviewer `01a03e69-fae4-7d61-8096-6dd3486ba74c` found five
  TucsonEgg miss refreshes. Reviewer `01a03e6a-267d-75f1-a66d-97da874a3cb1`
  found two more; main callsite closure found the eighth. All eight are fixed and
  regression-tested. The second reviewer's stale-map claim was rejected because
  the helper consumes the explicit post-mutation snapshot and delivery occurs
  only after locked producers write back and return.
- Both reviewers are closed. Exact process audit found no `go` or
  `crystal-server`; only `.coordination.lock` and this Goal's main writer lock
  remain. Both repositories' six `.cs` gates are empty.

## Exact recovery sequence

1. Verify each repository independently against the branches, HEADs and complete
   statuses above; rerun all six `.cs` gates.
2. If only this handoff is modified, run the control checker, commit it, read
   back the full hash and verify both repositories clean.
3. For `REGEN-P5-SAFEZONE-001`, read only the named matrix anchors and search the
   lessons archive with `SafeZoneHealing|safe zone|Healing SpellObject`.
4. Trace bounded Legacy startup/config/map/healing call chains and exact Go
   declarations; update the active index with frozen file authority before any
   production code write, then run the leaf gates.

# Crystal Go migration current handoff

Last updated: 2026-08-24 23:22 (Asia/Singapore)

This replace-in-place snapshot closes `CHAR-P3-BAN-DELETE-001` and routes the
same persistent Goal to `RANK-P3-CHAR-LIFECYCLE-001`. Preserve all listed
Legacy control/lesson work; never reset, stash, clean, delete, move, or
overwrite it.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). Workers `01a03437-99a7-7122-ac5a-d5a106eec145`,
  `01a03437-d74a-7091-82ba-03df6a64a88c`, and reviewer
  `01a03444-6a89-7d41-a9ad-eafc9744d278` are closed; no subagent is active.
- P1/P2/P3 remain scope-frozen and In progress. P3 has eight Complete,
  `RANK-P3-CHAR-LIFECYCLE-001` Active, and two dependency-blocked Ready
  children. Go matrix and Legacy active index agree; no phase status changed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `92776e91e82899283a30421e587061888528b0ee`
  (`docs(migration): close admin authority leaf`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/data-config-import-export.md`
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/verification/race-and-flake-attribution.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Index and untracked set are empty. Control checker and `git diff --check`
  exit 0. Separate tracked, staged, and untracked C# checks are empty. No
  repository lock exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `c0f70e833b15599d8acb9ad3ebf423cb731fc6a9`
  (`migrate character ban and deletion`).
- Worktree and index are clean. `git diff --check`, gofmt and separate tracked,
  staged, and untracked C# checks are empty. No repository lock, `go`, or
  `crystal-server` process exists.

## Active leaf and protected work

- Active leaf: `RANK-P3-CHAR-LIFECYCLE-001`.
- Outcome: preserve rank load eligibility, StartGame/level-up updates,
  delete/GM-login removal quirks, stale class/`MyRank` defects, zero-based
  re-rank behavior, RankType/offset silence, and timestamp detail resend.
- Read only the exact `RANK-P3-CHAR-LIFECYCLE-001` matrix row, exact P3 stage
  row, and evidence headings/tests named there. Do not read the full matrix.
- Legacy authority is bounded to exact AccountInfo/Envir/PlayerObject/
  MirConnection rank seed, update, removal and query consumers plus packet
  serializers. Every C# file stays read-only.
- Tentative Go ownership is bounded to auth ranking/service, server ranking/main,
  one focused lifecycle session test, and matrix evidence. Refine exact files
  in the active index after tracing and before any Go code write.
- Forbidden: completed ban/delete/account/admin mutation, StartGame map/
  bootstrap/logout lifecycle, P2 metadata, P11 core redesign, native client UI,
  broad source/matrix dumps, and every C# write.

## Verification ledger

- Final touched compile exited 0. Focused protocol/auth/import/server ban/delete
  suite passed `-count=10`; matching focused race passed `-count=3`.
- Final `go test ./... -count=1 -timeout=20m` exited 0; `cmd/crystal-server`
  took 78.704s. Final `go test -race ./... -count=1 -timeout=30m` exited 0;
  `cmd/crystal-server` took 81.229s. Final `go vet ./...` and `go build ./...`
  exited 0.
- The first full-race attempt failed only in established
  `TestSessionKirinIceThrustTranscript` test/ticker random-state races, outside
  all candidate files. Isolated race `-count=3` passed; a fresh unexcluded full
  race then passed. The failed run is not recorded as a pass.
- Production evidence covers wrong-stage survival, exact delete results,
  retained/repeated/duplicate/signed tombstones, no immediate save, relogin/JSON
  restart, deleted-index StartGame, strict character-ban packet/expiry clearing,
  selection filtering, ranking snapshot removal, and ordinary 117 retention.
- Reviewer found the pre-existing immediate `SaveJSON` mismatch; it was removed
  and the same reviewer returned `no findings`. Corrupt-index 117 re-export is
  registered as `PERSIST-P12-CORRUPT-CHAR-INDEX-001`; class-stale rank lifecycle
  is the new Active leaf.
- Intermediate candidate-owned test failures were resolved: the handmade 117
  fixture had a header offset error, and `DeferredSaveHero` violated the real
  character-name gate. Corrected fixtures passed final gates. Final diff/status,
  process audit, gofmt and all six C# gates passed before the Go commit.

## Exact recovery sequence

1. In the Legacy root, verify this handoff, active index, HEAD/status, control
   checker, diff check and three C# gates; commit the six owned control/lesson
   files if they match this snapshot.
2. In a separate Go-root command verify clean HEAD
   `c0f70e833b15599d8acb9ad3ebf423cb731fc6a9`, diff/C# gates and no process.
3. Read only the named ranking P3 row and stage row. Search only matching
   archived ranking/RemoveRank/MyRank/age/StartGame lessons.
4. Trace bounded Legacy and Go declarations, refine exact Go ownership in the
   active index before writes, then implement the smallest coherent slice and
   run its registered compile/focused/repeated/race gates.

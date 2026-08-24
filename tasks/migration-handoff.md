# Crystal Go migration current handoff

Last updated: 2026-08-25 06:56 (Asia/Singapore)

This replace-in-place snapshot records the committed closure of
`CLIENT-P3-SELECT-PROBE-001`, P3 phase-closure gates, and routing to the sole
Active discovery leaf `DISC-P4-CLOSURE`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `luna_worker` (`gpt-5.6-luna/max`). Read-only reviewer
  `01a035ec-07b1-71f1-90dd-5fb5760f0467` found three initial transcript gaps,
  then returned `no findings` after fixes and was closed. No agent or Go/
  crystal-server process remains active.
- P3 is scope-frozen and Complete with all eleven finite children Complete.
  P1/P2 remain frozen In progress. `DISC-P4-CLOSURE` is the unique Active leaf;
  no P4 functional implementation is authorized until its finite inventory is
  reviewed and scope-frozen.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD:
  `5440e6e39eb4d20d9b81a9d4bd66ffa0c4d84812`
  (`docs(migration): trace select probe leaf`).
- Tracked unstaged: `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/lessons.md`, `tasks/migration-active.md`, and this
  `tasks/migration-handoff.md`. Staged and untracked sets are empty.
- The archive records candidate fixture failures and review fixes; the active
  lesson records the corrected recurring fixed-heading control mistake; the
  active index closes P3 and routes only `DISC-P4-CLOSURE`.
- Index lock is absent. `tasks/check-migration-control.sh`, `git diff --check`
  and cached diff check exited 0. All tracked/staged/untracked Legacy C# gates
  are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `76d7f48a1d4ae2f4327bace60be2dd9fecfb4d6d`
  (`migrate select client probe transcript`).
- Worktree, index and untracked set are empty. The commit contains the five
  bounded code/test paths plus `docs/migration-matrix.md`; the matrix marks the
  Select child and P3 Complete and marks `DISC-P4-CLOSURE` Active.
- Index lock is absent; diff/cached-diff checks, tracked/staged/untracked Go C#
  gates and process audit are empty.

## Active leaf and protected work

- Active leaf: `DISC-P4-CLOSURE`.
- Read only the exact P4 stage row, named map-loading/map-info/enter-map/
  movement/visibility evidence headings/tests, and registered
  `BOOT-P4-STARTPOINT-001` finding. Do not read the full matrix.
- Outcome: enumerate a finite P4 child registry with exact dependencies, Go
  ownership and gates; obtain independent denominator review; scope-freeze P4;
  then route one dependency-ready child without implementing it in discovery.
- Legacy read authority is bounded to map loader/bootstrap, map entry/location,
  Turn/Walk/Run, visibility/object producers and matching serializers. Every
  `.cs` file remains read-only.
- Exact Go write authority during discovery is P4 inventory prose in
  `docs/migration-matrix.md` only; Legacy active index/handoff remain main-agent
  control. P5 combat/AI, P6 items, P7 NPC scripts, P8 pets/mounts, broad dumps,
  implementation code and every C# write are forbidden.
- Select-probe work is committed at Go `76d7f48`; no Go path remains protected.
  Its state parser is driven by `RunNetwork` and one bidirectional single-
  connection failure/success transcript; production reads prove
  ClientDisconnect is the final request in both probe lifetimes.

## Verification ledger

- Original candidate failures are retained: missing loaded StartPoint prevented
  level-zero ranking; two goroutines closed one done channel; after fixture
  repair the fake expected bare `probe` while real/Legacy ObjectChat emitted
  `ProbeSelect: probe`. These were candidate-caused and are corrected.
- Reviewer then found the state model direct-test-only, empty fake login
  metadata, and weak production disconnect proof. The common `expectStateID`
  path, bidirectional result 0..4/Banned/LogOutFailed/Success transcript,
  complete metadata/list assertions and recorded production frames close all
  three. Reviewer re-review returned `no findings`.
- `go test ./internal/probe ./cmd/crystal-server -run '^$' -count=1` exited 0.
- Full `go test ./internal/probe -count=10` and production
  `TestP3SelectProbeRunsAgainstProductionSession -count=10` exited 0; focused
  races for those scopes at `-count=3` exited 0.
- Fresh unexcluded `go test ./... -count=1 -timeout=20m`, `go vet ./...`,
  `go build ./...`, and `go test -race ./... -count=1 -timeout=30m` each exited
  0. No failure or package was excluded from a claimed full pass.

## Exact recovery sequence

1. In Legacy only, verify the exact four-file status, HEAD/control/diff/lock and
   all three C# gates against this snapshot.
2. In Go only, verify clean HEAD
   `76d7f48a1d4ae2f4327bace60be2dd9fecfb4d6d`, lock/process and all three C#
   gates. Do not rerun locked full gates unless committed code changes.
3. Commit only the four owned Legacy documentation files after control/diff/C#
   gates; the handoff may name this observed pre-documentation HEAD.
4. Resume `DISC-P4-CLOSURE`: search the archive by that ID and exact P4 entity
   keywords, read only the registered matrix anchors, run one bounded read-only
   reviewer wave, and enumerate the finite P4 children without code writes.

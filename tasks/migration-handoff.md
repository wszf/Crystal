# Crystal Go migration current handoff

Last updated: 2026-08-25 02:07 (Asia/Singapore)

This replace-in-place snapshot closes `RANK-P3-CHAR-LIFECYCLE-001` and routes
the same persistent Goal to `AUTH-P2-CHAR-METADATA-001`. Preserve every listed
Legacy control/lesson/archive change; never reset, stash, clean, delete, move,
or overwrite it. The compact-reconstruction snapshot was preserved once as
`tasks/migration-handoff-archive/2026-08-25-0207-pre-rank-closure.md`.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). Rank reviewers `01a034c7-b2f4-76c3-8e35-847f2798bdd8`,
  `01a034c7-dd9b-73b3-bb85-c2d60d2cf957`, and final reviewer
  `01a034e7-7b2d-7ff2-9e01-a4c290fdba79` are closed; no subagent or Go/server
  process remains active.
- P1/P2/P3 remain scope-frozen and In progress. P2 has six Complete,
  `AUTH-P2-CHAR-METADATA-001` Active and one dependency-blocked Ready child;
  P3 has nine Complete and two dependency-blocked Ready children. Matrix and
  active index agree; no phase status changed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `10b092173849978deddfc873804a3d657b53130e`
  (`docs(migration): close character ban delete leaf`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Untracked files are exactly:
  - `tasks/migration-handoff-archive/2026-08-25-0115-pre-rank-compact-reconstruction.md`
  - `tasks/migration-handoff-archive/2026-08-25-0207-pre-rank-closure.md`
- Index is empty. One expected Legacy documentation commit remains after final
  control/C# checks. No repository lock exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `fded2735c2dd5a7dcad583730a048e1938534b29`
  (`migrate character ranking lifecycle`).
- Worktree and index are clean. `git diff --check` and all three C# checks are
  empty; no repository lock or matching process exists.

## Active leaf and protected work

- Active leaf: `AUTH-P2-CHAR-METADATA-001`.
- Read only its exact P2 finite-inventory row, P2 stage row and tests/headings
  named there. Do not reread the full matrix.
- Outcome: preserve creation IP/time/index projection, construction
  `LastLoginDate`, StopGame LastIP/LastLogoutDate and `SelectInfo.LastAccess`;
  every LoginSuccess/LogOutSuccess selection omits deleted records and caps at
  four across explicit logout, disconnect, game-to-observer takeover,
  observer-only logout, failed pre-map entry, JSON and version-117 reload.
- Legacy authority is closed until exact `CharacterInfo`,
  `AccountInfo.GetSelectInfo`, `PlayerObject.Load/StopGame`, `MirConnection`
  login/logout and metadata serializers are bounded. Every C# file is read-only.
- Tentative Go ownership is closed until tracing: bounded
  `internal/auth/service.go`, `cmd/crystal-server/main.go`, existing focused
  tests and new metadata lifecycle tests. Refine exact paths before writes.
- Forbidden: completed P3 create/delete/admin/rank mutation, StartGame
  admission/map/bootstrap, P4/P7/P12 behavior, broad dumps and every C# write.

## Verification ledger

- Touched compile exited 0. Auth and server ranking/admin/operator focused suites
  passed `-count=10`; matching focused race passed `-count=3`. Full touched auth
  and server packages exited 0.
- First fresh `go test ./... -count=1 -timeout=20m` failed only established
  `TestSessionHallucinationTranscript` after 30 seconds with a closed-pipe stack
  outside owned files. Exact isolation passed `-count=10`; the next fresh
  unexcluded full test exited 0 (`cmd/crystal-server` 74.909s). The failed run
  is not recorded as a pass.
- Fresh `go test -race ./... -count=1 -timeout=30m` exited 0
  (`cmd/crystal-server` 81.557s). `go vet ./...` and `go build ./...` exited 0.
- Review fixed shared duplicate Rank final assignment/removal, materialized
  fallback, reused-service JSON replacement, stable session identity,
  name-based OnlineOnly and observer blank-account collision. The lossy old
  index snapshot is not wire authority; the companion TCP test proves exact
  type-6 reason-3/reason-0 order. Final reviewer `01a034e7-7b2d-7ff2-9e01-a4c290fdba79`
  returned `no findings` and no files changed.
- Final Go gofmt/diff/status/process and all three Go-side C# checks passed
  before commit. Legacy control/diff/all three C# checks remain the only pending
  closure step before its documentation commit.

## Exact recovery sequence

1. In the Legacy root run the control checker, diff check, status and all three
   C# gates; commit exactly the six tracked control/lesson files plus two owned
   archived snapshots if they match this handoff.
2. In a separate Go-root call verify clean HEAD `fded2735c2dd5a7dcad583730a048e1938534b29`,
   empty diff/C# gates and no process. Do not reopen the completed rank leaf.
3. Read only the named metadata P2 row/stage anchors and search matching archived
   metadata/LastAccess/logout/projection lessons.
4. Trace bounded Legacy and Go consumers, refine exact ownership before writes,
   then implement the smallest coherent metadata slice and run its registered
   compile/focused/repeated/race gates.

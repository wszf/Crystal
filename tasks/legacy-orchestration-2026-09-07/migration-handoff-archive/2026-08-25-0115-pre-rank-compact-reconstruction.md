# Crystal Go migration current handoff

Last updated: 2026-08-24 23:48 (Asia/Singapore)

This replace-in-place snapshot resumes the same persistent Goal at the traced,
write-frozen `RANK-P3-CHAR-LIFECYCLE-001` leaf. Preserve every listed change;
never reset, stash, clean, delete, move, or overwrite it.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). Legacy/Go auditors and the edge reviewer are closed;
  no subagent, Go process, or crystal-server process is active.
- P1/P2/P3 remain scope-frozen and In progress. P3 has eight Complete,
  `RANK-P3-CHAR-LIFECYCLE-001` Active, and two dependency-blocked Ready
  children. Go matrix and Legacy active index agree; no phase status changed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `10b092173849978deddfc873804a3d657b53130e`
  (`docs(migration): close character ban delete leaf`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Index and untracked set are empty. Control checker and `git diff --check`
  exit 0. Separate tracked, staged, and untracked C# checks are empty. No
  repository lock exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `c0f70e833b15599d8acb9ad3ebf423cb731fc6a9`
  (`migrate character ban and deletion`).
- Worktree and index are clean. `git diff --check` and separate tracked,
  staged, and untracked C# checks are empty. No repository lock, `go`, or
  `crystal-server` process exists.

## Active leaf and protected work

- Active leaf: `RANK-P3-CHAR-LIFECYCLE-001`.
- Legacy trace is frozen: load reset and process-only `Rank[2]`; account-order
  seed excluding admin/deleted/>13-day logout while including equality/future;
  non-GM StartGame and level-up `CheckRankUpdate`; delete/password-GM
  `RemoveRank`; request/cache serializers and outer fatal work-loop path.
- Exact Go write authority is
  `internal/auth/{ranking.go,ranking_test.go,service.go,accounts_import.go,p3_rank_lifecycle_test.go}`;
  `cmd/crystal-server/{ranking.go,ranking_test.go,ranking_session_test.go,admin_authority_session_test.go,main.go,p3_rank_lifecycle_session_test.go}`;
  and `docs/migration-matrix.md`.
- Required behavior includes stable physical tie order; materialized overall/
  class rows and rank arrays; 13-day seed; StartGame/level-up/delete/GM-login
  transitions; stale class duplicates; zero-based follower ranks; stale
  `MyRank`; global PlayerId timestamp cache with permanent resend after change;
  valid types 0..5; silent invalid offsets and types >6; and type 6 triggering
  the established reason-3 then reason-0 fatal shutdown path.
- Forbidden: completed ban/delete/account/admin mutation semantics, StartGame
  map/bootstrap/logout lifecycle, P2 metadata, P11 core redesign, native client
  UI, broad source/matrix dumps, and every C# write.

## Verification ledger

- No Go code for this leaf has been written and no leaf tests have run yet.
  The clean Go baseline remains the fully gated ban/delete commit `c0f70e8`.
- Bounded read-only tracing found current Go dynamically rebuilds sorted ranking
  rows and only has a GM-specific overall-removal marker; it lacks age seed,
  materialized rank arrays/lists, delete/re-entry defects, and timestamp state.
- Independent Legacy review confirmed: deleting/removing top A from `[A,B,C]`
  leaves overall `[B(rank0),C(rank1)]`, stale class `[A,B,C]`, stale overall
  `A.MyRank=1`, class `A.MyRank=0`; later non-GM update can duplicate both stale
  class A and a rank-zero follower. RankType 6 indexes `RankClass[5]`, escapes
  the connection loop, and invokes the global fatal shutdown; types >6 and
  invalid offsets are silent.
- Two command-construction failures and delayed hidden auditors were recorded in
  the workflow archive; all failed-call evidence was discarded. Active lessons
  returned under 50 KiB and the control checker exits 0.

## Exact recovery sequence

1. In the Legacy root, verify this handoff/active index against HEAD/status;
   run the control checker, diff check and three C# gates.
2. In a separate Go-root command verify clean HEAD `c0f70e8`, diff/C# gates and
   no process. Do not reread the full matrix or broad source trees.
3. Implement only the frozen materialized ranking state and lifecycle adapters,
   starting with auth production state plus focused unit tests; compile before
   touching main/session integration.
4. Add production transcript/fatal tests, then run focused repeated/race and the
   cadence-aware full test/full-race/vet/build gates before matrix closure.

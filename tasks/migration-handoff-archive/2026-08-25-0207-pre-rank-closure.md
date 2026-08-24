# Crystal Go migration current handoff

Last updated: 2026-08-25 01:15 (Asia/Singapore)

This replace-in-place snapshot reconstructs the same persistent Goal after a
real compaction signal exposed a stale handoff. Preserve every listed change;
never reset, stash, clean, delete, move, or overwrite it. The superseded
uncommitted snapshot was preserved once as
`tasks/migration-handoff-archive/2026-08-25-0115-pre-rank-compact-reconstruction.md`.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). Previously reported rank auditors/reviewers are closed.
  The recovered context exposes no active agent ID; the Go repository has no
  coordination/thread lock and no `go` or `crystal-server` process.
- P1/P2/P3 remain scope-frozen and In progress. P3 has eight Complete,
  `RANK-P3-CHAR-LIFECYCLE-001` Active, and two dependency-blocked Ready
  children. Go matrix and Legacy active index agree; no phase status changed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `10b092173849978deddfc873804a3d657b53130e`
  (`docs(migration): close character ban delete leaf`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Untracked files are exactly:
  - `tasks/migration-handoff-archive/2026-08-25-0115-pre-rank-compact-reconstruction.md`
- Index is empty. `tasks/check-migration-control.sh` and `git diff --check`
  exited 0 before this rewrite. Separate tracked, staged, and untracked C#
  checks are empty. No repository lock exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `c0f70e833b15599d8acb9ad3ebf423cb731fc6a9`
  (`migrate character ban and deletion`).
- Tracked unstaged files are exactly:
  - `cmd/crystal-server/admin_authority_session_test.go`
  - `cmd/crystal-server/main.go`
  - `cmd/crystal-server/ranking.go`
  - `cmd/crystal-server/ranking_session_test.go`
  - `cmd/crystal-server/ranking_test.go`
  - `internal/auth/accounts_import.go`
  - `internal/auth/p3_ban_delete_test.go`
  - `internal/auth/ranking.go`
  - `internal/auth/ranking_test.go`
  - `internal/auth/service.go`
- Untracked files are exactly:
  - `cmd/crystal-server/p3_rank_lifecycle_session_test.go`
  - `internal/auth/p3_rank_lifecycle_test.go`
- Index is empty. The tracked binary-diff SHA-256 is
  `bf1db644406c379fd7efcc79f13e5c0b7694eeb5d95ad785edb023c791455f13`;
  untracked test SHA-256 values are `b3a76aa437ca70c646fd997b7e4c628b9f1cc2be6282301916ac0233931b5416`
  and `d32ab236a9f1e71e5757506dd920b04c0895aa0f38a7d695b924c41c800ee2bd`
  in the listed order. `git diff --check` exited 0; all three C# checks are
  empty; no repository lock or matching process exists.

## Active leaf and protected work

- Active leaf: `RANK-P3-CHAR-LIFECYCLE-001`.
- Matrix anchors remain the exact Active row at line 198 and P3 stage row at
  line 791 in `docs/migration-matrix.md`; the matrix file is unmodified. The
  active index is the refined write-authority contract.
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
- The recovered candidate adds materialized auth ranking rows/state, seed/load
  filtering, lifecycle update/remove/count adapters, server request/cache/fatal
  integration, and focused auth plus production-session regressions. It also
  adjusts only owned ranking-dependent fixtures. No matrix evidence or commit
  has been written.

## Verification ledger

- Recovery-only checks, all exit 0: `git diff --check`; tracked/staged/untracked
  C# gates in each repository; Legacy `tasks/check-migration-control.sh`; and
  Go process audit `ps -Ao pid=,comm=,args= | awk '$2 == "go" || $2 ==
  "crystal-server" {print}'` with empty output. No implementation test was run
  after the compaction signal.
- The workflow/protocol archives report that a focused auth/cmd/admin/ban rank
  set exited 0 after fixture and suffix fixes. The exact command text was not
  preserved across compaction, so this report is not adopted as a leaf gate and
  must be rerun. No reliable post-candidate compile, repeated, race, full-test,
  full-race, vet, or build result is currently recorded.
- Frozen Legacy evidence remains: removing top A from `[A,B,C]` yields overall
  `[B(rank0),C(rank1)]`, stale class `[A,B,C]`, stale overall `A.MyRank=1`, and
  class `A.MyRank=0`; later non-GM re-entry can duplicate stale class A and a
  rank-zero follower. Password-GM leaks online counts across logout. RankType 6
  escapes to global fatal shutdown; types above 6 and invalid offsets are silent.
- Known intermediate candidate defects (slice suffix loss, identity-vs-name
  lookup, missing online-count leak, stale dynamic-snapshot fixtures, and typed
  delete outcome mismatch) are documented as corrected but require main-agent
  diff review and fresh verification. No review acceptance may be inferred from
  the compacted summary.

## Exact recovery sequence

1. Re-read this handoff and compare both repositories separately against the
   exact status and three Go fingerprints above; any drift invalidates it.
2. Review only the twelve owned Go candidate files, beginning with auth ranking
   invariants and all main call sites; preserve every existing edit.
3. Run gofmt on only owned changed Go files, then exact touched-package compile,
   focused auth/session tests, repeated and focused race gates. Resolve findings
   before adding matrix evidence.
4. Run cadence-due unexcluded full tests/full race/vet/build, final diff/status/
   process and all six C# gates; then update matrix/index/handoff and commit the
   completed leaf if every result passes.

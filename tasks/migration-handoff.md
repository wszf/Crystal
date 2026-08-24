# Crystal Go migration current handoff

Last updated: 2026-08-25 03:40 (Asia/Singapore)

This replace-in-place snapshot closes the verified
`AUTH-P2-CHAR-METADATA-001` candidate and routes the same persistent Goal to
`CHAR-P3-START-LOGOUT-001`. Preserve every listed change; never reset, stash,
clean, delete, move, or overwrite it.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). Legacy auditor `01a034f9-f650-7d43-9ee4-85c62b5b0de9`,
  Go auditor `01a034fa-1836-7513-bd1e-65375fb42b69`, test writer
  `01a03507-919f-7f51-9a38-8018262efae4`, and final reviewer
  `01a0351b-a7a3-7ac3-8cb0-06cb2da74cb0` are closed. No subagent, Go process,
  or crystal-server process remains active.
- P1/P2/P3 remain scope-frozen and In progress. P2 has seven Complete and one
  dependency-blocked Ready child. P3 has nine Complete,
  `CHAR-P3-START-LOGOUT-001` Active and one dependency-blocked Ready child.
  Matrix and active index agree; no phase status changed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `b37d8396f5972b8fd7d3c8fe2873804c58d03f2b`
  (`docs(migration): close character ranking lifecycle`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Index and untracked set are empty. Control checker and diff check exit 0; all
  three C# gates are empty. One expected Legacy documentation commit remains
  after the Go candidate commit supplies its exact hash. No repository lock
  exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `f0a1ff862f92a65358cecb283404e0452c042bc6`
  (`migrate character metadata lifecycle`).
- Worktree and index are clean. Gofmt/diff check and all three C# gates are
  empty; no repository lock or matching process exists.

## Active leaf and protected work

- Active leaf: `CHAR-P3-START-LOGOUT-001`.
- The completed metadata commit `f0a1ff862f92a65358cecb283404e0452c042bc6`
  atomically writes LastIP/LastAccess for real
  StopGame, preserves creation/LastLoginDate and capped Deleted-only projection,
  synchronizes duplicate-login cleanup, safely rolls back timed-out claims, and
  retires timed FIFO tickets. JSON/117 and every registered lifecycle path are
  covered; reviewer `01a0351b-a7a3-7ac3-8cb0-06cb2da74cb0` returned
  `no findings`.
- Read only the exact P3 Start/Logout row, P3 stage row and headings/tests named
  there. Do not reread the full matrix or reopen metadata after its commit.
- Next outcome: exact StartGame stage/settings/account/index/ban admission,
  deleted-record quirk, map/bind fallback and result/resolution; Game/Observer
  LogOut, combat-lock failure, stop/date/ranking effects, refreshed selection
  and same-connection re-entry cleanup.
- Legacy and Go write authority for the new leaf remain closed until bounded
  tracing. Every C# file is read-only. Completed create/delete/metadata/admin/
  ranking semantics, post-admission bootstrap, broad P4/P5/P7/P12, native UI
  and broad dumps are forbidden.

## Verification ledger

- Touched compile exited 0. Metadata/account-session/operator/FIFO focused suites
  passed `-count=10`; matching focused race passed `-count=3`. Auth and server
  touched packages passed; final server package time was 74.205s.
- Fresh `go test ./... -count=1 -timeout=20m` exited 0
  (`cmd/crystal-server` 75.328s). Fresh
  `go test -race ./... -count=1 -timeout=30m` exited 0
  (`cmd/crystal-server` 81.330s). `go vet ./...` and `go build ./...` exited 0.
- Production evidence covers exact creation/IP/date authority, physical order,
  banned inclusion, Deleted-only four-cap LoginSuccess/LogOutSuccess, explicit
  logout, disconnect, duplicate takeover, game-to-observer, observer-only,
  post-construction/pre-Game failure, concurrent snapshots, JSON and registered
  version-117 reload. P3 still owns exact missing-map Result 3 and transition
  outcomes.
- Review findings fixed unbounded takeover/FIFO waits, timeout claim loss,
  released-claim resurrection, completion ordering, callback races, race-test
  error capacity and evidence wording. Intermediate compile/control/Perl
  failures were corrected and archived; none is recorded as a passing gate.
- Final gofmt/diff/status/process and three Go-side C# checks passed. Legacy
  control/diff and three C# checks passed before this rewrite; rerun them before
  its documentation commit.

## Exact recovery sequence

1. Re-read this handoff and compare both repositories separately against the
   exact HEAD/status lists above; run Legacy control/diff/C# gates and verify
   clean Go HEAD `f0a1ff862f92a65358cecb283404e0452c042bc6` with process/C# gates.
2. Commit exactly the four Legacy documents after control/diff/C# gates using
   `docs(migration): close character metadata leaf`.
3. Read only the named Start/Logout P3 row/stage anchors and search matching
   archived StartGame/StartGameFailed/LogOut/map-bind/re-entry lessons. Trace
   bounded Legacy and Go consumers, refine exact ownership before writes, then
   implement the smallest coherent transition slice and run its leaf gate.

# Crystal Go migration current handoff

Last updated: 2026-08-25 03:43 (Asia/Singapore)

This replace-in-place snapshot records the post-compaction recovery boundary for
`CHAR-P3-START-LOGOUT-001`. Preserve every listed change; never reset, stash,
clean, delete, move, or overwrite it.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). All workers from the completed metadata leaf are closed;
  no new subagent, Go process, or crystal-server process is active.
- P1/P2/P3 remain scope-frozen and In progress. P2 has seven Complete and one
  dependency-blocked Ready child. P3 has nine Complete,
  `CHAR-P3-START-LOGOUT-001` Active, and one dependency-blocked Ready child.
- The first recovery status call mixed repositories with `git -C` and was
  truncated; its entire output is discarded. All facts below come from separate
  single-repository, zero-exit reruns with complete output.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `26974a7f68b9e2dfad1cec566c16eaa0b7d3d1a0`
  (`docs(migration): close character metadata leaf`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Index and untracked set are empty. These three control edits only record the
  corrected recovery state and archived C01/C28 evidence; no implementation
  changed. An intermediate control check correctly rejected an oversized active
  lessons file; the added history was moved to the archive and active lessons
  returned to 51,072 bytes. After the final rewrite, control checker and diff
  check exited 0; repository-lock and all three C# gates were empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `f0a1ff862f92a65358cecb283404e0452c042bc6`
  (`migrate character metadata lifecycle`).
- Worktree, index, and untracked set are empty. Diff check and all three C# gates
  exited 0/empty. No repository lock, Go process, or crystal-server process was
  found.

## Active leaf and protected work

- Active leaf: `CHAR-P3-START-LOGOUT-001`.
- Matrix anchors are only the exact P3 finite-inventory row
  `CHAR-P3-START-LOGOUT-001`, exact P3 stage row, and evidence headings/tests
  named by that row. Never reread the full matrix during normal recovery.
- Required outcome: exact StartGame stage/settings/account/index/ban admission,
  deleted-record quirk, map/bind fallback and result/resolution; Game/Observer
  LogOut, combat-lock failure, PlayerObject stop/date/ranking effects, refreshed
  selection, and same-connection re-entry cleanup.
- Legacy and Go write authority remain closed until bounded tracing refines exact
  files. Every `.cs` file is read-only. Completed create/delete/metadata/admin/
  ranking semantics, post-admission bootstrap, broad P4/P5/P7/P12, native UI,
  and broad dumps remain forbidden.

## Verification ledger

- No implementation or test ran after the compaction signal; recovery work was
  limited to single-repository audits and the three Legacy control documents.
- The completed metadata leaf previously passed touched compile; focused suites
  at `-count=10`; focused race at `-count=3`; fresh
  `go test ./... -count=1 -timeout=20m`; fresh
  `go test -race ./... -count=1 -timeout=30m`; `go vet ./...`; and
  `go build ./...`, all exit 0. Those results belong to committed Go
  `f0a1ff862f92a65358cecb283404e0452c042bc6`; they are not new Start/Logout
  evidence.
- Metadata evidence covers creation/IP/date authority, physical order, banned
  inclusion, Deleted-only four-cap projection, explicit logout, disconnect,
  duplicate takeover, game-to-observer, observer-only, pre-Game failure,
  concurrent snapshots, JSON, and registered version-117 reload. Exact
  StartGame missing-map and transition outcomes remain owned by the active leaf.

## Exact recovery sequence

1. In Legacy only, rerun `tasks/check-migration-control.sh`, `git diff --check`,
   exact status, lock, and the tracked/staged/untracked C# gates; inspect the
   three-document diff and commit only those files as
   `docs(migration): checkpoint start logout leaf`.
2. In Go only, verify HEAD
   `f0a1ff862f92a65358cecb283404e0452c042bc6`, clean status, lock/process, diff,
   and the three C# gates.
3. Read only the named Go matrix anchors. Search the Legacy lesson archive by
   `CHAR-P3-START-LOGOUT-001`, StartGame, StartGameFailed, LogOut, map-bind, and
   re-entry; read only matching sections.
4. Trace bounded Legacy and Go consumers, refine exact ownership before writes,
   then implement the smallest coherent transition slice and run its full leaf
   gate plus cadence-aware integration.

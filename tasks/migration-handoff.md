# Crystal Go migration current handoff

Last updated: 2026-08-24 22:17 (Asia/Singapore)

This replace-in-place snapshot closes `ADMIN-P3-AUTHORITY-001` and routes the
same persistent Goal to `CHAR-P3-BAN-DELETE-001`. Preserve all listed Legacy
control/lesson work; never reset, stash, clean, delete, move, or overwrite it.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers are `luna_worker`
  (`gpt-5.6-luna/max`). Reviewer `01a033fa-8785-7863-a391-d3426f306e58` is
  closed and no subagent is active.
- P1/P2/P3 remain scope-frozen and In progress. P3 now has seven Complete,
  `CHAR-P3-BAN-DELETE-001` Active, and three Ready children. Go matrix and
  Legacy active index agree; no phase status changed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `40299b2fd4339fd6d06bb3ce36a29c3e2eb8a87c`
  (`docs(migration): close account operator leaf`).
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Index and untracked set are empty. Control checker and `git diff --check`
  exit 0. Separate tracked, staged, and untracked C# checks are empty. No
  repository lock exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `02a63cefdcc551dcc08eb5a4cec71249339091d1`
  (`migrate administrator authority`).
- Worktree and index are clean. `git diff --check` and separate tracked, staged,
  and untracked C# checks are empty. No repository lock, `go`, or
  `crystal-server` process exists.

## Active leaf and protected work

- Active leaf: `CHAR-P3-BAN-DELETE-001`.
- Outcome: preserve Select-stage DeleteCharacter result/source order, soft
  tombstone/global/ranking effects, repeated/deleted-index quirks, persisted ban
  and strict expiry clearing, StartGameBanned, and first-match duplicate/corrupt
  index behavior.
- Read only the exact `CHAR-P3-BAN-DELETE-001` matrix row, exact P3 stage row,
  and evidence headings/tests named there. Do not read the full matrix.
- Legacy authority is bounded to AccountInfo selection projection,
  CharacterInfo delete/ban serialization, MirConnection delete/start handlers,
  and exact Envir delete/rank/import consumers. Every C# file stays read-only.
- Tentative Go ownership is bounded to auth service/ranking, only the proven
  legacy import adapter, bounded main session handling, one focused ban/delete
  session test, and matrix evidence. Refine exact files in the active index
  after tracing and before any Go code write.
- Forbidden: completed account/admin/creation behavior, full StartGame location/
  bootstrap/logout lifecycle, P2 metadata closure, ranking-core redesign, broad
  P12 recovery, native C# UI, broad matrix/source dumps, and every C# write.

## Verification ledger

- Final touched compile before the last review-fix set and all final focused
  package compiles exited 0.
- Final focused config/auth/server authority suite, including TCP accepted entry,
  password-promoted observe, operator current/future authority, overall/class
  rank defects and logout-gap restoration, passed `-count=10`; the matching
  focused race suite passed `-count=3`.
- `go test ./... -count=1 -timeout=20m` exited 0 on the final candidate;
  `cmd/crystal-server` took 79.568s.
- `go test -race ./... -count=1 -timeout=30m` exited 0 on the final candidate;
  `cmd/crystal-server` took 81.508s.
- Final `go vet ./...` and `go build ./...` exited 0. Owned `gofmt -w` plus
  `gofmt -l`, diff/status/process review and all six C# gates passed before the
  Go commit; post-commit Go remains clean.
- Main recovery review found stale overall `MyRank`; the independent reviewer
  then found runtime-observe authority, logout-gap removal, class `MyRank=0`,
  and missing accept-loop evidence. All were fixed; the same reviewer reread
  exact fixes and returned `no findings` without writes or tests.
- Intermediate failures were candidate-owned and resolved: requester index 0
  hid the stale-rank assertion; overlength role/account fixture names failed
  real gates; one multi-file patch partially applied before a stale main hunk.
  Exact fixture/context corrections passed the final focused and full gates.

## Exact recovery sequence

1. In the Legacy root, verify this handoff, active index, HEAD/status, control
   checker, diff check and three C# gates; commit the five owned control/lesson
   files if they match this snapshot.
2. In a separate Go-root command verify clean HEAD
   `02a63cefdcc551dcc08eb5a4cec71249339091d1`, diff/C# gates and no process.
3. Read only the named ban/delete P3 row and stage row. Search only matching
   archived ban/delete/tombstone/rank/import lessons.
4. Trace bounded Legacy and Go declarations, refine exact Go ownership in the
   active index before writes, then implement the smallest coherent slice and
   run its registered compile/focused/repeated/race/persistence gates.

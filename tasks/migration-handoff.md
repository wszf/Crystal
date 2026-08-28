# Crystal Go migration current handoff

Last updated: 2026-08-28 08:39 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-SCRIPT-LOAD-001` is Complete in Go commit
  `42ae55fffd0be35df501a9d49c19c4239b1dca4f` with matrix evidence.
- The one Active leaf is dependency-ready `NPC-P7-PAGE-GRAMMAR-001`; Script Load
  is Complete. Matrix read scope is row 181, routing ledger C row 204 and P7
  summary row 966 only.
- P7 remains scope-frozen at 24 children: 6 Complete, 1 Active and 17 Ready.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed pre-control-commit HEAD
  `fc13241ffa1f631f8ed50b46a979ae6ff2185695`; upstream `origin/master`, ahead
  495 and behind 0.
- Owned pending control changes are this handoff, `tasks/migration-active.md`,
  `tasks/lessons-archive/migration/data-config-import-export.md`, and
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`, plus the
  exact-heading recurrence in `tasks/lessons-archive/workflow/
  shell-tools-and-patching.md`. They are intended for one control commit;
  recovery must compare actual HEAD/status with that documentation-only delta.
- Latest separate tracked/staged/untracked `.cs` checks are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `42ae55fffd0be35df501a9d49c19c4239b1dca4f`;
  no upstream. Commit `42ae55f Complete P7 NPC script graph loading` owns the
  accepted implementation, tests, matrix completion and Page Grammar routing.
- Index and worktree are clean; there are no untracked files.
- Latest separate tracked/staged/untracked `.cs` checks are empty. Exact-command
  process audit found no `go` or `crystal-server` process.

## Completed Script Load behavior

- The read-only exporter now projects the scripts Legacy constructs instead of
  scanning every text file: database NPC roots, canonical aliases for configured
  Default/Monster/Robot roots, and recursively reached parse-time CALL scripts.
- Missing configured special files project Legacy startup-created empty scripts;
  custom roots retain ordinary identity while canonical aliases feed production
  runtime names. Database NPC binding remains first-source authoritative when an
  alias collides.
- INSERT runs first on the growing list, appends at end, expands nested inserts,
  preserves the stale-success replay after a later missing file, and removes all
  directives. INCLUDE then inserts selected blocks in place, expands nested
  includes, leaves too-late INSERT text untouched, and clears the whole script
  on a missing file.
- Quoted and unquoted CALL paths use case-insensitive Windows-compatible file
  resolution; missing targets create no action/edge. Recursive INSERT, INCLUDE
  and CALL graphs return bounded explicit errors rather than consuming resources
  indefinitely.
- Settings/export ordering now supplies all three special filenames before text
  enrichment and projects the default object created by Legacy startup without
  mutating the Legacy directory.

## Verification ledger

- Owned gofmt and minimum compile passed:
  `go test ./cmd/crystal-server ./internal/legacyworld -run '^$'` exit 0.
- Focused graph/settings/export/production-load tests passed together at
  `-count=10`; focused `-race -count=3` passed, both exit 0.
- `go test ./internal/legacyworld -count=1` and
  `go test ./cmd/crystal-server -count=1` passed after fixture updates.
- The first integration command failed only the known unrelated 30-second
  `TestSessionHallucinationTranscript` closed-pipe fixture. Its exact rerun
  passed. The next fresh `go test -count=1 ./...`, `go vet ./...`, and
  `go build ./...` all exited 0. The prior Access fresh full-race pass remains
  current; this loader leaf has no broad concurrency change and its focused race
  gate passed.
- One read-only `luna_worker` returned the exact Legacy trace. A terminal
  read-only Luna review remained running after finalize requests and was closed
  without a report; it is not acceptance evidence. Main review found and fixed
  File.Exists-style inaccessible-path continuation, then reran focused and full
  gates.
- `git diff --check`, both-repository C# gates and the control checker pass.

## Active leaf and protected work

- Active leaf: `NPC-P7-PAGE-GRAMMAR-001`.
- Owned Go files are `internal/worlddata/npcscript.go`, new
  `internal/worlddata/npcscript_grammar_test.go`, new bounded
  `cmd/crystal-server/npc_page_grammar_session_test.go`, and matrix/index/handoff.
- Script loading, speech formatting, runtime control flow/actions/conditions,
  callbacks and C# remain protected.

## Exact recovery sequence

1. Read this handoff back, run `tasks/check-migration-control.sh`, and separately
   verify both statuses, diff checks and three-way C# gates.
2. Read only matrix row 181, ledger C and P7 summary; confirm Script Load Complete
   / Page Grammar Active / counts 6+1+17.
3. Begin Page Grammar with archive keyword search, exact Legacy page-construction
   trace and a finite grammar/reachability checklist before any implementation.

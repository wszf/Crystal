# Crystal Go migration current handoff

Last updated: 2026-08-28 10:09 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-PAGE-GRAMMAR-001` is Complete in Go commit
  `3672281ed9f60d5be0b06c742243857bd57e9616`.
- The one Active leaf is dependency-ready `NPC-P7-SPEECH-INPUT-001`; Page
  Grammar, static NPC wire and item wire catalog prerequisites are Complete.
- Exact matrix read scope is row 182, routing ledger D row 205 and P7 summary
  row 966 only. P7 remains scope-frozen at 24 children: 7 Complete, 1 Active and
  16 Ready.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed pre-handoff-finalization HEAD
  `dc05bf55f0511e8a819b23378906a2cb643485aa`; upstream `origin/master`, ahead
  498 and behind 0. Commit `dc05bf55 Document P7 page grammar closure` records
  lessons, Page Grammar completion and Speech/Input routing.
- Index/worktree were clean before this handoff-only refresh; its own commit is
  the one expected HEAD delta. There are no staged or untracked paths.
- The active index records Page Grammar Complete, Speech/Input Active and the
  exact 7+1+16 denominator. Lessons preserve the two signature-closure compile
  failures, Turkish fixture correction, marker-neutral root correction, terminal
  review findings and their verified repairs.
- `git diff --check`, the control checker and separate tracked/staged/untracked
  `.cs` checks exit 0/are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `3672281ed9f60d5be0b06c742243857bd57e9616`;
  no upstream. Commit `3672281 Complete P7 NPC page grammar` owns implementation,
  tests, grammar-only fixture corrections, matrix completion and next routing.
- Index and worktree are clean; no untracked paths. Separate tracked, staged and
  untracked `.cs` checks are empty. Exact-command process audit found no `go` or
  `crystal-server` process.

## Completed Page Grammar behavior

- Ordinary scripts construct only the breadth-first graph reachable from the
  first CurrentCulture prefix match for `[@MAIN]`; automatic special scripts
  construct every marker-neutral `[@_]` root graph. Raw duplicate links/order,
  first duplicate headers, missing placeholder pages and Legacy's one-line
  lookahead bugs are preserved.
- Column-zero comment/directive/header boundaries, exact markup and GOTO-family
  discovery, unknown directive panics, recognized directive tails, whitespace,
  tab and greedy page-argument quirks match the direct C# constructors.
- `%ARG` replacement now mutates a token slice, preserving arguments containing
  spaces, out-of-range and lowercase replacement quirks. Turkish CurrentCulture
  casing reaches parsing, page lookup, selected-button authorization and action
  command/page-key parsing through production startup/session wiring.
- Authenticated ordinary/default transcripts prove culture-sensitive reachable
  pages and dotted-I denial. Static links in existing fixtures now use Legacy
  `<text/@PAGE>` grammar rather than the former permissive bare `/@PAGE` form.
- Duplicate automatic-root execution and marker side effects are explicitly
  retained by `NPC-P7-DEFAULT-CALLBACK-001` and
  `NPC-P7-MONSTER-ROBOT-SCRIPT-001`; parser construction is Complete.

## Verification ledger

- Owned gofmt and minimum compile passed:
  `go test ./internal/worlddata ./cmd/crystal-server -run '^$'` exit 0.
- `go test ./internal/worlddata ./cmd/crystal-server -run
  '^TestNPCP7PageGrammar' -count=10` exit 0.
- `go test ./internal/worlddata -count=1` and
  `go test ./cmd/crystal-server -count=1` both exit 0.
- `go test -race ./internal/worlddata ./cmd/crystal-server -run
  '^TestNPCP7PageGrammar' -count=3` exit 0.
- Fresh integration `go test -count=1 ./...`, `go vet ./...`, and
  `go build ./...` all exit 0 after the final review fixes.
- Read-only Legacy auditor `01a045f2-5a45-7d33-8514-76c52d1f7674` supplied the
  exact constructor/runtime trace. Terminal reviewer
  `01a04605-bda5-7001-8f21-441e84623ce9` found ARG/culture/tab issues in revision
  1; after repair its bounded revision 2 returned `No findings`. Both are closed.
- `git diff --check`, both-repository C# gates and the control checker pass.

## Active leaf and protected work

- Active leaf: `NPC-P7-SPEECH-INPUT-001`.
- Owned Go authority is bounded `internal/worlddata/npcscript.go`,
  `cmd/crystal-server/{main.go,default_npc.go,npc_input_session_test.go,
  default_npc_session_test.go}`, new `npc_script_format.go`,
  `npc_script_format_test.go`, `npc_speech_input_session_test.go`, and
  matrix/index/handoff.
- Page grammar/load, static wire/access/item catalog, control flow,
  action/condition execution, callbacks, protocol layouts and C# are protected.

## Exact recovery sequence

1. Read this handoff back, run `tasks/check-migration-control.sh`, and separately
   verify both statuses, diff checks and three-way C# gates.
2. Confirm only matrix row 182, ledger D row 205 and summary row 966: Page
   Grammar Complete / Speech Input Active / counts 7+1+16.
3. Search the archive only for Speech/Input/placeholder/metadata/sticky-input
   keywords, then trace exact Legacy SAY/Response/input consumers and freeze the
   finite placeholder/order checklist before any Go implementation.

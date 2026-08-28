# Crystal Go migration current handoff

Last updated: 2026-08-28 08:06 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-ACCESS-GATE-001` has passed its main review and all required gates.
  Go commit `1f595d4043058d1b4072e0a3faf4d548084b8b94` and the matrix mark it
  Complete. Legacy control commit `2235dd8c1a7e1e15b7ddb47e1cdb1846c41d0af0`
  records the lessons and Script Load route.
- The one Active leaf is dependency-ready `NPC-P7-SCRIPT-LOAD-001`; its config
  prerequisite is Complete. Matrix read scope is row 180, routing ledger B row
  203 and P7 summary row 966 only.
- P7 remains scope-frozen at 24 children: 5 Complete, 1 Active and 18 Ready.
  Completed Access now unblocks `STORAGE-P2-NPC-GATE-001`, but selection remains
  in the current frozen P7 phase.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `2235dd8c1a7e1e15b7ddb47e1cdb1846c41d0af0`; upstream `origin/master`, ahead
  494 and behind 0. Index/worktree were clean with no untracked files before
  this handoff-only finalization; its own commit is the one expected HEAD delta.
- Commit `2235dd8c` preserves the mixed-root/tool/path/build lessons, corrected
  Legacy action-before-response ruling, Access fixture findings and fresh gates.
- Latest separate tracked/staged/untracked `.cs` checks are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; observed HEAD
  `1f595d4043058d1b4072e0a3faf4d548084b8b94`; no upstream. Commit
  `1f595d4 Complete P7 NPC access gates` owns the accepted Access implementation,
  tests, fixture corrections, P7 registry and Script Load routing.
- Index and worktree are clean; there are no untracked files.
- Latest separate tracked/staged/untracked `.cs` checks are empty. Exact-command
  process audit found no `go` or `crystal-server` process.

## Active leaf and protected work

- Active leaf: `NPC-P7-SCRIPT-LOAD-001`

- Committed Access implements UTF-16 key length/disconnect,
  wrong-stage keepalive, ordinary/default/input identity and dead gates,
  map/range/runtime/player visibility with Legacy VisibleLog lifetime, selected
  success/else button authorization, and exact quest runtime ObjectID lookup.
- Access-only fixture corrections activate a real default Client page, preserve
  dead ordinary silence, select MAIN before storage, and give special pages an
  explicit empty `#SAY` segment so Legacy `ProcessSegment` installs their page.
  Business result assertions remain unchanged.
- Next-leaf Go authority is only `internal/legacyworld/{text.go,settings.go,
  export.go}`, new `internal/legacyworld/npc_script_load_test.go`, new bounded
  `cmd/crystal-server/npc_script_load_session_test.go`, and matrix/index/handoff.
  Page grammar, runtime control flow/actions/conditions/callbacks and C# remain
  protected.

## Verification ledger

- Owned gofmt and minimum compile passed:
  `go test ./cmd/crystal-server ./internal/worlddata -run '^$'` exit 0.
- Exact Access/parser/default/input/quest focused run exit 0; the same set passed
  `-count=10`, and focused `-race -count=3` exit 0.
- The first server-package run found stale default/dead/storage and empty-special-
  page fixtures. All failures were corrected only after authority expansion and
  exact reruns pass.
- A later server-package run failed only the unrelated 30-second
  `TestSessionHallucinationTranscript` pipe fixture. A clean archive of pre-leaf
  Go HEAD passed once and reproduced the same failure under `-count=20`; current
  exact rerun passed. This is pre-existing time-sensitive fixture evidence, not
  an Access stack. The subsequent fresh unexcluded full gates passed.
- Fresh `go test -count=1 ./...` exit 0; `go vet ./...` and `go build ./...`
  exit 0; fresh `go test -race -count=1 ./...` exit 0.
- `git diff --check`, both-repository C# gates and the final control checker
  pass after handoff readback.
- Two read-only `luna_worker` attempts and two explicitly disclosed
  `codex-auto-review` fallbacks remained running after non-interrupting finalize
  requests and returned no report; all were closed. They provide no acceptance
  evidence. Main completed the line-by-line terminal Legacy/Go review with no
  remaining finding; tests and full race are the independent executable evidence.

## Exact recovery sequence

1. Read this handoff back, run `tasks/check-migration-control.sh`, and separately
   verify both statuses, diff checks and three-way C# gates.
2. Re-read only matrix rows 179-180, ledgers A-B and P7 summary to confirm Access
   Complete / Script Load Active / counts 5+1+18.
3. Begin `NPC-P7-SCRIPT-LOAD-001` from its registered authority: archive keyword
   search, exact Legacy load-chain trace, finite checklist, then bounded work.

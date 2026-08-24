# Crystal Go migration current handoff

Last updated: 2026-08-24 14:00 (Asia/Singapore)

This replace-in-place snapshot closes `PERSIST-P2-SOURCE-PRECEDENCE-001` at Go
`4729fed32ded396d36b05c4bdafad6e17e4fc1dd` and routes the bounded P3 inventory
leaf `DISC-P3-CLOSURE` Active. P2, P3, and the persistent Goal remain In
progress; no phase or Goal completion is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress with twelve registered children: nine
  Complete and three unfinished.
- P2 remains scope-frozen/In progress with eight children: six Complete and two
  dependency-blocked Ready. JSON-over-binary source precedence is Complete;
  character metadata waits on P3 and storage/NPC waits on P7.
- P3 remains open/In progress. `DISC-P3-CLOSURE` is the one Active leaf and owns
  only a finite inventory audit; it does not own implementation.
- The retained-gap version-117 account counter/global re-export remain P12 and
  were not claimed by the completed P2 leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before this handoff snapshot commit: `master...origin/master [ahead
  440]`; observed HEAD `e65d1011fd65f87639802eb758462e7e6fa86d6c`
  (`docs(migration): refresh source precedence handoff`).
- Before replacing this file, tracked modifications were exactly:
  `tasks/migration-active.md`,
  `tasks/lessons-archive/migration/data-config-import-export.md`,
  `tasks/lessons-archive/workflow/repository-boundaries-02.md`, and
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`.
  This handoff is the fifth expected tracked modification. Index and untracked
  set are empty.
- Archive updates preserve the initial mixed-repository recovery call, legal
  zero-match `rg` failure, and source-precedence fixture/review corrections.
  Active lessons remain unchanged at 295 lines/51170 bytes.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- No C# file is owned; tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; observed clean HEAD
  `2cf64fbab31dced74d6df1ef6ce745377b05dabd`
  (`docs(migration): activate P3 closure audit`).
- Source-precedence code/evidence is committed immediately before it at
  `4729fed32ded396d36b05c4bdafad6e17e4fc1dd`
  (`test(persistence): lock account source precedence`).
- Worktree, index, and untracked set are empty. No C# file is owned; tracked,
  staged, and untracked C# gates are empty.

## Active leaf and protected work

- Active leaf: `DISC-P3-CLOSURE`.
- Outcome: bound P3 to finite children by reconciling reachable character
  list/create/delete/start/logout, operator-account, ranking, and client-startup
  behavior with existing Go production/test evidence and explicit exclusions.
- Matrix anchors: exact P3 stage row; exact cross-phase bullets
  `CHAR-P3-BAN-DELETE-001` and `ADMIN-P3-ACCOUNT-OPS-001`; then only P3 evidence
  headings/rows named by those anchors. Do not read the full matrix.
- Legacy read authority: reachable P3 handlers and their real consumers only;
  every C# file is read-only and audit tooling must be Go.
- Go write authority: P3 finite-inventory matrix evidence only. Legacy control
  writes are the active index and current handoff; no production/test code is
  owned during discovery.
- Required gate: bounded Legacy/Go handler and test ledgers, explicit Complete/
  Ready/dependency/exclusion rulings, independent read-only `luna_worker`
  denominator review, matrix/index reconciliation, control/diff/status/process,
  and all six C# gates.
- Forbidden: implementing a child before scope freeze, reopening P2,
  inventorying P4-P12, broad matrix/source dumps, destructive Git, and every C#
  write.

### Completed source-precedence evidence

- The focused Go-only transcript drives exact `runServerWithContext` with JSON
  and 117 paths configured simultaneously.
- Same-ID binary password/metadata and a binary-only account are rejected while
  JSON login authority succeeds. A JSON-only ChangePassword mutation forces
  SaveJSON and its registered checkpoint hook.
- Nonzero JSON-vs-binary NextUserItem/Auction/Mail/GameShop sentinels prove the
  binary global source is not merged into runtime JSON authority.
- After the first hook completes, the test restores the original conflicting
  117 file. Graceful cancellation then proves only the final runtime checkpoint
  can replace it; direct global parsing and account reload retain JSON-backed
  metadata/password/login state and remove the binary-only account/sentinels.
- The account-only checkpoint intentionally does not claim P12 global re-export
  or retained-gap NextAccountID continuity.

## Verification ledger

Final commands below exited 0 after the final test revision:

- `gofmt -w cmd/crystal-server/p2_account_precedence_test.go`.
- Touched compile:
  `go test ./cmd/crystal-server -run '^$' -count=1`.
- Focused ordinary:
  `go test ./cmd/crystal-server -run '^TestP2AccountSourcePrecedence$'
  -count=1 -timeout=10m` and final repeated `-count=10`.
- Focused race:
  `go test -race ./cmd/crystal-server -run
  '^TestP2AccountSourcePrecedence$' -count=3 -timeout=10m`.
- `go vet ./cmd/crystal-server` and `git diff --check`.
- Exact Go/Legacy status, process audit, and all six C# gates.

Earlier candidate checks failed only in the new test fixture: an unused `fmt`
import after removing HTTP; a characterless post-first account filtered by the
117 loader; and a second localhost session rejected by the short IP block.
Touched compile caught the import. The final same-connection ChangePassword
save trigger removes the character/P3 and second-session gate scope entirely.
No production code changed, and final ordinary/repeated/race checks pass.

No integration gate is due: this leaf adds one focused test and matrix evidence
only, changes no shared production/startup/persistence code, and the preceding
account-session commit already passed fresh unexcluded full tests/full race,
vet, and build.

## Subagents and quiescence

- Writer `01a03218-1c48-74f1-bba8-7c0f0cd4ad21` (`luna_worker`) created only
  the original focused test and reported compile/focused pass; the main agent
  tightened it to exact `runServerWithContext` and graceful-final-checkpoint
  isolation. Thread is closed.
- Reviewer `01a0322b-1d89-70b3-baad-486c930e0855` (`luna_worker`) was read-only.
  Its predicted 117 reload failure was contradicted by ordinary `-count=10` and
  race `-count=3` because the writer supplies current creation time, but its P3
  scope warning was accepted: the final fixture uses no TCP character creation.
  It reported no other finding and is closed.
- No subagent result is pending. Exact process inspection is empty for `go`,
  `crystal-server`, and `git`.

## Exact recovery sequence

1. Verify each repository separately against the exact HEAD/status/file lists
   above; do not discard or overwrite owned uncommitted Legacy documentation.
2. Run `tasks/check-migration-control.sh`, all six C# gates, and commit only the
   five listed Legacy documentation files if they remain exact and clean.
3. Read only the P3 stage row, the two registered P3 bullets, and named P3
   evidence anchors; search the archive with `DISC-P3-CLOSURE`, character,
   StartGame, administrator, ranking, and startup keywords.
4. Build bounded Legacy/Go handler/test ledgers, delegate one independent
   read-only denominator review, and register finite P3 children before any
   implementation or phase scope-freeze claim.

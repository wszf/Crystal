# Crystal Go migration current handoff

Last updated: 2026-08-24 16:42 (Asia/Singapore)

This replace-in-place snapshot closes and commits `CHAR-P3-CREATE-001`, routes
`ADMIN-P3-ACCOUNT-OPS-001` as the one Active leaf, and records the exact clean
Go/control-commit state. P1, P2, P3, and the persistent Goal remain In progress;
no phase or Goal completion is claimed.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 remains scope-frozen/In progress: nine Complete, three unfinished.
- P2 remains scope-frozen/In progress: six Complete, two dependency-blocked
  Ready.
- P3 remains scope-frozen/In progress: five Complete,
  `ADMIN-P3-ACCOUNT-OPS-001` Active, and five Ready.
- Active Index and Go matrix agree on the unique Active leaf, exact eleven-child
  denominator, and P3 stage `In progress (scope frozen)`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 442]`; observed HEAD
  `8d6db545765e78ee283cbac10e8cbd26e7d265e7`
  (`docs(migration): close P3 discovery leaf`).
- Tracked modifications are exactly
  `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/lessons-archive/workflow/repository-boundaries-02.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/lessons.md`, `tasks/migration-active.md`, and this handoff. Index and
  untracked set are empty.
- Archives preserve creation tracing/review/test attribution and recovery/tool
  failures. Active C01/C03 were strengthened without exceeding control limits.
- Tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; observed clean HEAD
  `a7f16f8bfb5a0ec4e885d5101dce6635bfafd6e5`
  (`feat(p3): preserve character creation contract`).
- Worktree, index, and untracked set are empty. The commit contains exactly
  `cmd/crystal-server/{main.go,main_test.go,p3_character_creation_test.go}`,
  `docs/migration-matrix.md`, and
  `internal/auth/{hero_test.go,service.go,service_test.go}`.
- The candidate preserves exact NewCharacter setting/IP/validation order,
  current-culture DisabledChars and administrator bypass, four-character active
  limit, tombstone reservation, character-only name authority, unchecked IDs,
  creation metadata/insertion, Legacy constructor-zero returned SelectInfo,
  JSON-owned next-character gaps, and ordinary retained-record 117 reload.
- The default loader path is installed only by the real lifecycle wrapper;
  injected runtime tests do not create repository-local `Envir`. The one
  generated `cmd/crystal-server/Envir/DisabledChars.txt` artifact was identified,
  precisely removed, and remained absent after final focused/full gates.
- Tracked, staged, and untracked C# gates are empty.

## Active leaf and protected work

- Active leaf: `ADMIN-P3-ACCOUNT-OPS-001`.
- Outcome: preserve reachable operator blank-account creation/metadata edits,
  administrator and password-change toggles, offline/live account-ban variants,
  password/storage resets and live notification, direct removal, stopped-server
  wipe quirks, and direct JSON/117 save/reload without invented disconnect or
  cleanup behavior.
- Matrix anchors are only its P3 registry row, exact P3 stage row, and evidence/
  tests named there. Do not read the full matrix.
- Legacy read authority is bounded to `AccountInfoForm`, relevant live account
  actions in `PlayerInfoForm`, and exact auth/storage/persistence consumers.
- Prospective Go write authority is bounded to new
  `internal/operator/{account.go,account_test.go}` if tracing confirms that
  architecture, bounded `internal/auth` adapters/tests, bounded
  `cmd/crystal-server` live-session/persistence wiring/tests, and matrix
  evidence. No next-leaf Go code exists yet.
- Forbidden: character delete/ban/start/logout, GMPassword/`@LOGIN`, ranking,
  player combat/map/item/flag actions, broad P12 backup/recovery, native C# UI,
  destructive Git, broad matrix/source dumps, and every C# write.

## Verification ledger

- Touched compile: `go test ./internal/auth ./cmd/crystal-server -run '^$'
  -count=1`, exit 0 after final changes.
- Auth creation/counter/Hero-name focused suite: `-count=10`, exit 0; focused
  race `-count=3`, exit 0.
- Server `^TestP3(Character|Disabled)` suite: `-count=5`, exit 0; focused race
  `-count=3`, exit 0 after final startup-loader boundary.
- Reviewer-discovered Level-zero initially changed shared test fixtures and
  caused safe-zone, ElectricShock, Hero-special, and cross-boundary failures.
  Production creation and Level-1 test/bootstrap helper were separated; the
  three attributed tests and boundary test passed `-count=10`.
- The next unexcluded full run reproduced established
  `TestSessionYinDevilNodeTranscript/42` empty-notification flake; its isolated
  `-count=20` rerun passed. Two later fresh unexcluded `go test ./... -count=1
  -timeout=20m` runs passed, with the final server package at 76.567s.
- Fresh final `go vet ./...` and `go build ./...` exited 0. `git diff --check`,
  exact status, process inspection, generated-artifact check, and Go C# gates
  exited 0. Full race is not cadence-due; concurrency-relevant focused race is
  complete.

## Subagents and quiescence

- Read-only `luna_worker` `01a032c8-61dd-7901-9e68-0879b2839245` is closed. It
  found the High Level-zero wire mismatch and no loader/counter/P12 overclaim;
  requested recount confirmed the four-character effective limit. It changed no
  file and ran no test.
- No subagent is active. Exact `go`, `crystal-server`, and `git` process
  inspection is empty.

## Exact recovery sequence

1. Reverify both repositories separately, run control/diff/C# gates, and commit
   only the six owned Legacy control/archive files.
2. Search the lessons archive with `ADMIN-P3-ACCOUNT-OPS-001`, AccountInfoForm,
   PlayerInfoForm, account ban, password reset, wipe, JSON, and 117 keywords.
3. Trace bounded Legacy/Go operator ledgers, delegate one read-only reviewer or
   disjoint writer wave, freeze exact file authority, then implement the Active
   leaf and its registered gate.

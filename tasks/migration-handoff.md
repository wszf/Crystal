# Crystal Go migration current handoff

Last updated: 2026-08-26 08:38 (Asia/Singapore)

This replace-in-place snapshot records the committed map-hazard leaf and routes
the next bounded P5 regen trace. It claims no phase or project closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither complete nor blocked. A real
  context compaction triggered this refresh; no implementation or commit ran
  across the safety gate.
- Main authority is `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- P5 remains scope-frozen In progress: nine of twelve children are Complete,
  `REGEN-P5-HUMAN-001` is Active and two children are Ready.
- Active matrix anchors are P5 summary row 851, registry row 3164 and completed
  hazard evidence 3177-3191; matrix and Active Index agree.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `e632e071af89a80042dda0a92e781227cca81325`
  (`Close mining leaf and route map hazards`).
- Unstaged tracked files are exactly:
  - `tasks/lessons-archive/migration/data-config-import-export.md`
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - this handoff after replacement
- The index is empty and there are no untracked files.
- Tracked, staged and untracked Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; observed HEAD
  `f0f5e93e48ba2e79c3ce0a72e1cba61fad802b8d`
  (`Complete P5 map hazard lifecycle`).
- Tracked, staged and untracked status are clean. All three Go `.cs` gates are
  empty.

## Active leaf and protected work

- Active leaf: `REGEN-P5-HUMAN-001`.
- `SPELL-P5-MAP-HAZARD-001` is committed at
  `f0f5e93e48ba2e79c3ce0a72e1cba61fad802b8d`. Schema/import retains
  `FireDamage`/`LightningDamage`; runtime covers loaded-map schedules,
  SpellObjects, exact RNG/order/timers, MAC Struck, packets and restart reset.
- Live account identity, real transient `@LOGIN`, online grant/revoke,
  removal/reused IDs, completed-death Brown time, revival timestamp/double
  health fanout and deterministic session-loop isolation are covered.
- Regen tracing has begun against `HumanObject.ProcessRegen`, its Settings
  weights and existing Go Player/Hero/Heal/Vamp/potion surfaces. Its exact Go
  write set is deliberately not open until the bounded call-site trace closes.
- The committed hazard files and all earlier migration evidence remain
  protected; no regen code has been written.

## Verification ledger

- Touched compile command for server/auth/legacyworld/worlddata exited 0.
- Focused hazard/schema/auth/session command exited 0 at `-count=20`; the same
  packages/regex under `-race -count=5` exited 0.
- `TestSessionMapHazardSpawnDamageRemovalAndRestart -count=100` exited 0 after
  isolating the connection loop. Before that fix, one fresh full test exited 1
  at this test (`Next(12000)` displaced the expected MAC unit draw); the stack
  proved session read-loop interference, not a production behavior failure.
- Final fresh `go test ./... -count=1`, `go vet ./...`, `go build ./...` and
  `go test -race ./... -count=1` all exited 0 after the correction.
- Initial Luna `01a03b34-6c8b-7dc2-a65b-6113f56059a7` found Brown-time loss.
  A broad replacement reviewer was shut down after repeated waits without a
  report; bounded replacement `01a03b74-25f3-7b62-b826-21292ae36739` reviewed
  the final death/live-authority diff read-only and returned `No findings`.
- No `go`/`crystal-server` process or active subagent remains. At 08:38 after
  compaction, separately rooted Legacy and Go calls verified HEAD/status/index/
  untracked state and all six C# gates; `tasks/check-migration-control.sh`
  exited 0. One earlier mixed-root audit was discarded in full.

## Exact recovery sequence

1. Read this handoff back, verify its limits, rerun the final diff/status/C#
   pre-commit gates, and commit only the current control/archive files in Legacy.
2. Verify that Legacy commit ID and both clean worktrees.
3. Resume `REGEN-P5-HUMAN-001`; finish the bounded call-site trace, freeze the
   exact Go file set in Active Index, then begin implementation.

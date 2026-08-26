# Crystal Go migration current handoff

Last updated: 2026-08-27 03:25 (Asia/Singapore)

This replace-in-place snapshot records the accepted direct SafeZoneHealing leaf
and routes the next bounded SafeZoneBorder trace. It is the current recovery
authority, not a historical journal.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- Active leaf: `SPELL-P5-SAFEZONE-BORDER-001`.
- P5 is scope-frozen with fifteen of eighteen children Complete, this leaf
  Active and two Ready. `REGEN-P5-SAFEZONE-DIRECT-002` is committed Complete in
  Go `1042adbad7514318dcca0502876b87203222b37c`.
- Normal recovery reads only matrix P5 row 851, registry rows 3168-3170 and the
  direct/scheduled evidence around rows 3255-3295; never the full matrix.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed pre-commit HEAD
  `25b389f380fe94e26beb9238575d58aba68b3bb8`; upstream `origin/master`.
- Unstaged tracked work is this handoff, `tasks/migration-active.md`, and
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`. Staged and
  untracked sets are empty before final documentation staging.
- The archive edit records invalid shell-glob/rg-option-order recurrences. No
  C# file is owned; tracked, staged and untracked `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `1042adbad7514318dcca0502876b87203222b37c`;
  no upstream is configured.
- Worktree, index and untracked set are clean. Commit `1042adb` contains the
  direct helper, central/route/specialized movement wiring, focused/authenticated
  tests and matrix completion/next-leaf selection. Every `.cs` gate is empty.

## Verification ledger

- Direct explicit-cell lookup bypasses scheduler/current location while target
  admission and final-location ObjectEffect fan-out reuse scheduled authority.
- Common AI/routes and every registered specialized Hero/owned-Monster
  turn/walk/two-cell/push boundary preserve movement/turn/push packet order,
  Run explicit traversed cells, final-only custom moves and invalid-point push
  bypass. Reviewer `01a03f85-d823-7cf2-85a3-5d3affa584a5` found omitted
  deer/Zuma/siege/route paths; all are wired and included in the AST ledger.
- Authenticated net.Pipe uses production `world.tick` ordinary-pet AI and proves
  immediate ObjectWalk/ObjectEffect with no ObjectSpell or scheduler advance.
- Touched compile exits 0. SafeZone focused count-20 and race count-5 exit 0;
  specialized movement regressions count-20 and race count-5 exit 0.
- Fresh `go test ./... -count=1 -timeout=30m`, `go vet ./...`, `go build ./...`
  and fresh `go test -race ./... -count=1 -timeout=45m` all exit 0 after the
  review fixes. No Go or crystal-server process remains.

## Active leaf and protected work

- Active leaf: `SPELL-P5-SAFEZONE-BORDER-001`.
- `SPELL-P5-SAFEZONE-BORDER-001` begins read-only. Trace exact Legacy config
  order, perimeter geometry, Decoration TrapHexagon constructor/payload and
  static visibility lifecycle; freeze exact Go config/main/world/effect and
  focused/session test authority before production writes.
- Committed direct-healing `1042adbad7514318dcca0502876b87203222b37c`,
  scheduled healing and every other completed P5 owner remain protected.

## Exact recovery sequence

- First finish the direct leaf documentation: independently verify each
  repository status and all six `.cs` gates; in Legacy run the control checker,
  stage only the three documentation files listed above, commit, and read back
  its full hash. Verify both repositories clean.
- Then search the lessons archive with
  `SafeZoneBorder|TrapHexagon|Decoration|perimeter|ObjectSpell`, read only the
  registered matrix anchors, and perform the bounded Border trace.

# Crystal Go migration current handoff

Last updated: 2026-08-27 05:05 (Asia/Singapore)

This replace-in-place snapshot records accepted SafeZoneBorder and routes the
base-monster-family trace. It is the current recovery authority.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; workers are `luna_worker`.
- Active leaf: `MONSTER-P5-BASE-FAMILY-001`.
- P5 is scope-frozen with sixteen of eighteen children Complete, this leaf
  Active and one Ready. SafeZoneBorder is committed Complete in Go `b9b6e8393e277417a88901a6ade0db8eb8e40bc5`.
- Recovery reads only matrix P5 row 851, registry around 3168-3175 and current
  SafeZone evidence; never the full matrix.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `09977192060674da94bc0fd99ada0ba46883e07b`; upstream `origin/master`.
- Only `tasks/migration-active.md` and this handoff are unstaged; index and
  untracked set are empty. All three `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `b9b6e8393e277417a88901a6ade0db8eb8e40bc5`; no upstream.
- Worktree, index and untracked set are clean. Commit `b9b6e83` contains config, startup, decoration projection, focused/session tests and matrix routing. Every `.cs` gate is empty.

## Verification ledger

- Border preserves default/fallback/Optional ordering, exact clipped perimeter,
  overlap and ID/RNG order before Healing, permanent nonblocking dedicated
  decoration state, exact TrapHexagon ObjectSpell payload and existing static
  visibility lifecycle.
- Authenticated bootstrap/range exit/re-entry/logout/restart tests pass. Reviewer
  `01a03fc5-5b7e-7f81-b92b-2a7cb8641755` found startup-toggle and mixed ordering
  evidence gaps; AST startup wiring and mixed NPC/monster/border order close them.
- Touched compile exits 0. Focused count-20 and race count-5 exit 0. Fresh full
  normal, vet, build and fresh full race all exit 0 after review fixes.
- No active worker or Go/crystal-server process remains.

## Active leaf and protected work

- Active leaf: `MONSTER-P5-BASE-FAMILY-001`.
- Begin read-only: trace exact 46-ordinal Legacy factory/dynamic types and base
  search/target/move/melee lifecycle; freeze exact Go files/tests before writes.
- All SafeZone work, specialized mapped AIs, ordinary-spawn child and `.cs`
  files are protected.

## Exact recovery sequence

1. Verify each repository independently and all six `.cs` gates.
2. Run the Legacy control checker, stage only active index/handoff, commit and
   verify both repositories clean.
3. Search lessons with `MONSTER-P5-BASE-FAMILY-001|MonsterObject|factory|AI=0`,
   read only registered matrix anchors, and perform the bounded base-family trace.

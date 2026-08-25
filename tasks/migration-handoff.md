# Crystal Go migration current handoff

Last updated: 2026-08-25 11:52 (Asia/Singapore)

This replace-in-place snapshot records accepted `DISC-P5-CLOSURE`, synchronized
routing to `MAP-P4-NOREINCARNATION-AUTH-001`, and the durable boundary required
by the real compaction signal.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- `DISC-P5-CLOSURE` is Complete. Read-only Legacy auditor
  `01a036ed-ee8f-7a50-a042-f1d665f83627` confirmed the 211 mapped/45 default AI
  denominator and both map-hazard producers. Reviewer
  `01a036ff-6ca2-7f50-ada6-1c68c2ded15d` accepted the exact eleven-child P5
  registry with no remaining finding. Both agent IDs are no longer present in
  the post-compaction agent registry; no worker owns live files.
- P5 is scope-frozen In progress with eight Complete and three Ready children.
  The completed user-spell producer dependency makes the named P4 child ready,
  so it is the unique Active leaf.
- Recovery exposed matrix/index drift: the Active Index selected the P4 leaf
  while its matrix row remained Ready. The matrix row is now Active. A stale P3
  count fragment in the Active Index was removed. No implementation or test was
  started while this inconsistency or the compaction hard gate was open.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD: `14a592a3c857804a0e89327d92f8767815e97c27`
  (`Route migration to P5 closure discovery`).
- Tracked unstaged files before this handoff write:
  `tasks/lessons-archive/migration/data-config-import-export.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`, and
  `tasks/migration-active.md`; this handoff is now also tracked unstaged.
  Staged and untracked sets are empty.
- The two archive files preserve P5 schema/row-finalization and recovery failure
  evidence without exceeding the active-lessons limit. The Active Index is 239
  lines/17226 bytes and routes only the named P4 leaf.
- All three Legacy C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `ffac428450fe90a3277819b21bdcb938004e952f`
  (`Freeze P5 migration closure inventory`).
- Worktree, index and untracked set are clean.
- The committed matrix freezes P5 into eleven finite children, records eight Complete
  and three Ready, closes `DISC-P5-CLOSURE`, and marks exactly one matrix row
  Active: `MAP-P4-NOREINCARNATION-AUTH-001` at line 232.
- `git diff --check` passes and all three Go-repository C# gates are empty.

## Active leaf and protected work

- Active leaf: `MAP-P4-NOREINCARNATION-AUTH-001`.
- Outcome: drive imported `NoReincarnation=true` through a real authenticated
  Reincarnation Magic request while preserving Legacy admission/source order,
  failed `S.Magic.Cast=true`, unchanged Shape-3 equipped-Amulet count, links,
  SpellObjects, practice, MP/cooldowns and durable state.
- Matrix anchors: exact leaf row plus only NoReincarnation/Reincarnation named
  completion prose. Do not read another phase or the full matrix.
- Exact Go write authority: existing
  `cmd/crystal-server/reincarnation_session_test.go`; production
  `reincarnation.go`/`world.go` only if the authenticated transcript proves a
  defect. The P5 frozen registry is read-only.
- Legacy read authority: exact `MapInfo.NoReincarnation` load fields and
  `HumanObject.Magic`/Reincarnation admission paths located by `rg`; every C#
  file remains read-only.
- Unresolved work: authenticated imported-map denial/positive-control transcript,
  exact self/observer packet order and negative side effects, repeated/focused
  race gates, and independent review.

## Verification ledger

- Durable-boundary repository HEAD/status and all six C# gates: exit 0, with the
  exact unstaged files recorded above.
- Go matrix unique-Active audit: exit 0; exactly line 232 is Active and the named
  leaf occurs once. Commit `ffac428450fe90a3277819b21bdcb938004e952f`
  succeeded and the Go worktree is clean.
- Legacy active lessons are 295 lines/51006 bytes; Active Index is 239
  lines/17226 bytes. `tasks/check-migration-control.sh` passed after this
  replacement, and the handoff was read back against the Legacy worktree.
- No behavior tests were run after the compaction signal. This boundary contains
  documentation/routing work only; implementation remains unopened.
- `ps` exact-command audit found no `go` or `crystal-server` process. The two P5
  agent IDs return `not_found` after compaction, consistent with closed threads.
- Failed mixed-repository, truncated and expected-zero `rg` calls were discarded
  in full and rerun as separate single-repository, explicit-rc calls; their
  output is not acceptance evidence.

## Exact recovery sequence

1. Re-read this handoff and verify both repositories separately against the
   exact status, unique-Active and C# facts above.
2. Search the lessons archive with `MAP-P4-NOREINCARNATION-AUTH-001`,
   `Reincarnation`, authenticated/session and transcript keywords; read only
   matching sections.
3. Read the exact matrix/Legacy/Go authorities, delegate one bounded read-only
   Legacy trace or independent review to `luna_worker`, then implement only the
   owned authenticated session evidence and run the required leaf gate.

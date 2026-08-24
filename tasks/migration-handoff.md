# Crystal Go migration current handoff

Last updated: 2026-08-25 08:03 (Asia/Singapore)

This replace-in-place snapshot records the committed closure of
`DISC-P4-CLOSURE`, the synchronized P4 finite registry, and routing to the sole
Active functional leaf `MAP-P4-LOAD-001`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `luna_worker` (`gpt-5.6-luna/max`). Fresh read-only reviewer
  `01a03617-76c2-7652-ab64-7e27143ce72b` is closed and no agent or Go/
  crystal-server process remains active.
- P4 now has an accepted exact ten-child denominator: two Complete and eight
  unfinished. `DISC-P4-CLOSURE` is Complete, P4 is scope-frozen In progress,
  and `MAP-P4-LOAD-001` is the sole Active leaf in both authoritative controls.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed HEAD:
  `25d9fa26555280cce5ce096f4981cca5e03f1cc8`
  (`docs(migration): freeze p4 inventory`). The eventual handoff-only commit is
  the one expected documentation delta from this observed HEAD.
- Before this snapshot refresh the worktree, index and untracked set were empty;
  this handoff is now the sole tracked unstaged file.
- `tasks/lessons.md` contains two bounded strengthening substitutions for the
  recurring P4 cross-repository/`rg` command mistakes. The active index records
  P4 Frozen, the ten-child registry and exact `MAP-P4-LOAD-001` authority.
- Control limits/headings, diff checks, lock and all three Legacy C# gates were
  clean at the registry commit and must remain clean for the handoff-only commit.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `84db86606fff4ef88bf57c28688d00233b72e43a`
  (`docs(migration): freeze p4 inventory`).
- Worktree, index and untracked set are empty. The commit adds the accepted
  ten-child P4 registry, marks P4 scope-frozen In progress, closes discovery
  and routes MAP load Active.
- The registry records earlier auditors
  `01a035fe-9de9-7ab3-855a-a2c48e43d140` and
  `01a035fe-c4c9-77d0-b0ff-abb7abfccefa`, denominator reviewer
  `01a0360b-428d-7bb0-a036-830680fa9d8e`, and the fresh reviewer above.
- Lock/process, diff/cached-diff and all three Go C# gates were empty/clean at
  the matrix commit.

## Active leaf and protected work

- Active leaf: `MAP-P4-LOAD-001`.
- Outcome: preserve Legacy per-map v0-v7/v100 metadata/cell/door/fishing/
  walkability loading and ordered registration without a synthetic or specially
  required exported-world primary; continue after missing/read/corrupt maps,
  exclude their StartPoints, and preserve exception -> localized title ->
  filename plus metadata-count observability.
- Read only the exact P4 MAP-load inventory row/stage row and bounded Legacy
  `Map.Load`, `MapInfo.CreateMap`, `Envir.StartEnvir` map loop/message order and
  localization keys. Do not read the full matrix or broad source trees.
- Exact Go write authority is `cmd/crystal-server/{maps.go,maps_test.go,main.go,
  process_lifecycle_test.go}` plus bounded P4 matrix evidence. Legacy active
  index/handoff remain main-agent control; every `.cs` file is read-only.
- `BOOT-P4-STARTPOINT-001` final no-startpoint rejection, P5 combat/AI/spawns,
  P6 item semantics, P7 NPC scripts, P8 companions and P12 persistence mechanics
  remain forbidden.

## Verification ledger

- Archive search and bounded Legacy/Go source/test ledgers covered map loading,
  StartPoint, map detail, entry, movement, visibility, chat, UTC light and
  NoReincarnation without reading the full matrix.
- Fresh reviewer first rejected omitted map shout/NoNames, unsupported UTC-light
  Complete status, and weak SearchMap/GameName/repeat gates. It rejected the
  first correction for three stale candidate statements, then returned ACCEPT
  with no remaining finding after all corrections. Zero reviewer writes/tests/
  commits and no `.cs` changes occurred.
- Locked P3 fresh unexcluded full tests/full race/vet/build at Go `76d7f48`
  remain valid because discovery changed documentation only. No functional test
  result is newly claimed for P4 discovery.
- Required discovery closure gate is control/matrix consistency, diff/status/
  process checks and all six C# gates; those are rerun immediately before commit.

## Exact recovery sequence

1. Verify both repositories clean, matrix/index/handoff agree, and no agent or
   process remains active; the actual Legacy HEAD may be one handoff-only commit
   after the observed value above.
2. Begin `MAP-P4-LOAD-001` under its exact four-file authority; derive focused
   missing/read/corrupt/source-order/diagnostic tests before production edits.

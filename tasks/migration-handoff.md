# Crystal Go migration current handoff

Last updated: 2026-08-28 02:54 (Asia/Singapore)

This is the replace-in-place current snapshot. The pre-compaction state remains
preserved once at
`tasks/migration-handoff-archive/2026-08-28-ordinary-spawn-pre-compaction.md`;
do not startup-read that archive. The automatic compact summary is not evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`).
- `MONSTER-P5-ORDINARY-SPAWN-001` is implemented, terminally reviewed and
  freshly gated. Go matrix and Active Index mark P5 scope-frozen and Complete
  with all twenty-one children Complete.
- Active leaf is now `CHAT-P4-MAP-CONTEXT-001`; P4 has eight Complete children,
  this chat child Active and collision rules dependency-blocked.
- Recovery matrix scope is only CHAT leaf row 233 and P4 summary row 850. Do not
  read the full matrix, reopen P5 or inventory another phase.
- Current control-plane edits are uncommitted. Run
  `tasks/check-migration-control.sh` after this replacement and again before
  the Legacy commit.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `4aa4117d9db6dd6128e04bc418cc991afcba87bb`
  (`4aa4117 docs: close dynamic constructor leaf`); upstream `origin/master`,
  ahead 491 and behind 0 at observation.
- Index is empty. Tracked worktree modifications are exactly:
  - `tasks/lessons-archive/migration/monster-base-family.md`
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- The only untracked file is the one-time archive
  `tasks/migration-handoff-archive/2026-08-28-ordinary-spawn-pre-compaction.md`.
  Preserve every listed file; no reset, stash, clean, deletion or overwrite.
- Tracked, staged and untracked `.cs` checks are all empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; observed HEAD
  `d2cece406910dd2dce58b577921ca230a287b086`
  (`d2cece4 Complete ordinary monster spawn convergence`); no upstream.
- Index and worktree are clean. The commit closes ordinary spawn and P5, adds
  its evidence, and routes `CHAT-P4-MAP-CONTEXT-001` Active.
- No `go` or `crystal-server` process exists. Tracked, staged and untracked
  `.cs` checks are all empty.

## Active leaf and protected work

- Active leaf: `CHAT-P4-MAP-CONTEXT-001`

- Outcome: preserve NoNames-masked normal map-local chat, cross-map whisper,
  ordinary range shout, one-shot map/server shout consumption and `@MAP`
  source/recipient behavior from the active map context.
- Before the first production write, trace and register exact physical Go paths
  for map-rule import and chat/session handling. Test authority is one new
  authenticated map-context chat test plus bounded existing chat/item/probe
  expectations after enumeration.
- P6 shout arming commit
  `1d399992a690614a122cc46b3e64b4cda8272c2f`, P3 utility behavior, unrelated
  map movement/collision/visibility and all C# files remain protected.
- Matrix anchors are row 233 and row 850 only.

## Ordinary-spawn closure evidence

- Factory AI=60-63/99 constructor prefix, cell and Spawned operate RNG order,
  fixed direction/image/kind, first packet and retained projections are locked.
- Ownerless targeting, movement, delayed impact, no-Master lifetime, SnakeTotem
  minions/owned targets, parent-child death/removal, map respawn and restart are
  production-tested. Shared summoned image/corpse/nested-master behavior is
  protected by focused regressions.
- Legacy `MonsterObject.Die` clears Master before Vampire/Charmed explosions;
  former owner/owned pet remain wild targets, wild Monster and Hero do not.
  Direct capture clears duplicate-death markers. SnakeTotem self-destruct keeps
  three seconds while direct death removes at zero time.
- Static HellBomb preserves image poison, strict deadline, inherited Alone gate
  and respawn; production HellLord-created bombs are `NoRespawn` and complete
  impact/removal without reappearing.
- Initial focused execution failed four standalone SnakeTotem/Charmed fixtures
  because the new runtime admission seam remained disabled. The fixtures were
  corrected to enable production AI and a separate disabled negative test was
  retained; no production rollback was made.

## Verification ledger

All commands below ran in the Go root against the final code state unless noted.

- `go test ./cmd/crystal-server -run '^$' -count=1` — exit 0.
- Initial focused ordinary/protected command — exit 1 only in
  `TestOrdinarySpawnCharmedSnakeMovesFromStackAndTotemFront`,
  `TestOrdinarySpawnSnakeTotemAggroAndShockGate`,
  `TestOrdinarySpawnSnakeTotemChildTargetsOnlyParentLinkedObjects` and
  `TestOrdinarySpawnSnakeTotemChildLifetimeAndDeathCleanup`; exact rerun passed
  after fixture admission correction.
- `go test ./cmd/crystal-server -run '(OrdinarySpawn|ArcherSummon|SummonSnakes|CharmedSnake|HellLord|HellBomb|SafeZoneHealingDirectCallsiteLedger)' -count=10 -timeout=180s` — exit 0.
- `go test -race ./cmd/crystal-server -run '(OrdinarySpawn|ArcherSummon|SummonSnakes|CharmedSnake|HellLord|HellBomb|SafeZoneHealingDirectCallsiteLedger)' -count=3 -timeout=240s` — exit 0.
- Fresh `go test -count=1 ./...` — exit 0.
- Fresh `go vet ./...` — exit 0.
- Fresh `go build ./...` — exit 0.
- Fresh `go test -race -count=1 ./...` — exit 0.
- Final `git diff --check` before matrix/control routing — exit 0; rerun before
  commit because documentation changed afterward.
- Reviewer `01a0444a-c3dc-7242-a6fb-d0fad25000bc` returned five findings; main
  fixed deterministic cleanup, direct-delete marker cleanup, dynamic HellBomb
  coverage and runtime admission, and ruled Master clearing from Legacy source.
  Read-only Luna `01a04475-ccc7-7e21-b9f2-ad00271f1a4d` returned no report and
  was stopped; status stayed stable and no write/process appeared.

## Exact recovery sequence

1. Run the Legacy control checker and separately reverify both repositories'
   exact status and all three `.cs` gates.
2. In Go, run `git diff --check`, stage only the eight owned ordinary/matrix
   paths, review the staged list, and commit the P5 closure. Read the exact new
   Go SHA back from Git; do not infer it from short output.
3. Replace this handoff's Go HEAD/status with that commit, rerun the control
   checker, stage only the seven listed Legacy tracked files plus the preserved
   handoff archive, review the staged list and commit the control/evidence set.
4. Resume the same Goal at `CHAT-P4-MAP-CONTEXT-001`: read only matrix rows 233
   and 850, search the lessons archive with the leaf ID plus NoNames/whisper/
   shout/@MAP/recipient keywords, then trace exact Legacy and Go production
   paths. Update Active Index physical write authority before any code write.

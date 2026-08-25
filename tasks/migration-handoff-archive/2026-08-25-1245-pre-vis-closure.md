# Crystal Go migration current handoff

Last updated: 2026-08-25 12:45 (Asia/Singapore)

This replace-in-place snapshot is the durable compaction handoff for the
unchanged active Goal and `VIS-P4-OBJECT-001`. A real compaction signal stopped
all reconnaissance, testing and writing before this refresh.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- P4 remains scope-frozen In progress with seven Complete and three unfinished
  children. P5 hide/show producers are Complete, so `VIS-P4-OBJECT-001` remains
  the unique dependency-ready Active child in both the Active Index and matrix.
- Read-only VIS workers `01a0373a-4478-73c0-b3ad-c5b243133776` and
  `01a0373a-6b15-7021-8d35-3d4dc5720429` stopped at the compaction hard gate,
  reported no writes or tests, and are closed.
- A `ps` audit at 12:45 found no `go` or `crystal-server` process.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD:
  `da05d7cb18380283fec0a2a32c4dfc2372990549`
  (`Close no-reincarnation leaf and route visibility`).
- Before this handoff replacement, tracked unstaged, staged and untracked sets
  were all empty. This handoff is the only expected tracked unstaged file.
- Active Index contains exactly one Active row, `VIS-P4-OBJECT-001`.
- All three Legacy C# gates are empty: unstaged, staged and untracked.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `1aef1eac1953ca184d7e96c5394ae3c2cc5a3cc3`
  (`Complete authenticated no-reincarnation map gate`).
- Tracked unstaged, staged and untracked sets are all empty.
- Matrix contains exactly one Active row, `VIS-P4-OBJECT-001`.
- All three Go-repository C# gates are empty: unstaged, staged and untracked.

## Active leaf and protected work

- Active leaf: `VIS-P4-OBJECT-001`.
- Outcome: preserve Legacy player/NPC/monster Object* add/remove geometry,
  per-player NPC visibility filters and interactive Observer non-blocking/
  relative projection across join, map transition, town revive and appearance
  refresh, including every currently discarded imported NPC visibility field.
- Matrix authority is only the exact VIS row and named visibility/Observer
  completion prose. Do not read another phase or the full matrix.
- Exact Go write authority: `internal/legacyworld/database.go`,
  `internal/worlddata/world.go`, bounded `cmd/crystal-server/world.go`, and only
  focused import/visibility/session tests discovered by exact enumeration.
- Legacy read authority: exact `NPCInfo` visibility fields/load/save order and
  `MapObject`/`PlayerObject` add/remove/search/Observer paths. Every C# file is
  read-only.
- Legacy worker evidence: `Server/MirDatabase/NPCInfo.cs:23-38` defines the
  visibility fields/defaults, `:44-101` loads them with version gates and
  `:105-140` saves them. `Server/MirObjects/NPCObject.cs:232-253` processes
  day/time visibility, `:320-343` hide/show geometry and recipients, `:381-394`
  `GetInfo`, `:418-421` blocking, and `:424-462` conquest/flag/level/class gates
  plus add/remove behavior.
- Go worker evidence: `internal/legacyworld/database.go` near line 636 currently
  discards `TimeVisible`, four time bytes, `MinLev`, `MaxLev`, `DayofWeek`,
  `ClassRequired` and `FlagNeeded`; current `NPCInfo` lacks those fields while
  retaining `ConquestVisible`. Runtime declarations/callers and focused tests
  were not yet fully audited.
- Unresolved work: search matching lessons archive sections; finish the finite
  field/source-order and recipient/geometry checklist; implement parser/export
  round trip and runtime filters; cover join/transition/revive/refresh and
  Observer; run repetition, focused race, due integration and review.
- No VIS implementation, tests, commits or other uncommitted work exist yet.

## Verification ledger

- No VIS tests have run; both workers performed read-only tracing only. Their
  last commands exited 0 (`NPCObject.cs` numbered read and `world.go` bounded
  read respectively).
- Last completed leaf (`MAP-P4-NOREINCARNATION-AUTH-001`) remains committed with
  focused `-count=20`, focused race `-count=5`, fresh unexcluded
  `go test ./... -count=1`, `go vet ./...` and `go build ./...` all exit 0.
- Full repository race was not due at that leaf: P4 was not closing, focused
  race passed and cadence remained below the contract threshold.
- Before this replacement, both worktrees were clean, the active-row checks
  each reported exactly one VIS row, process audit was empty, and all six C#
  gates were empty.

## Exact recovery sequence

1. Re-read this handoff and verify it against the two repositories with separate
   single-root commands; rerun the control checker after this handoff write.
2. In the Legacy root, run the next recovery command:
   `rg -n -i -e 'VIS-P4-OBJECT-001|NPC visibility|TimeVisible|Observer|Object add|Object remove' -- tasks/lessons-archive`
   and read only matching sections.
3. Read only exact VIS matrix/Legacy/Go authorities, finish the finite checklist,
   then use one bounded wave of at most three `luna_worker` agents with no more
   than two disjoint writers before integrating owned files locally.

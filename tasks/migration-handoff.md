# Crystal Go migration current handoff

Last updated: 2026-08-25 12:40 (Asia/Singapore)

This replace-in-place snapshot records committed
`MAP-P4-NOREINCARNATION-AUTH-001` closure and synchronized routing to
`VIS-P4-OBJECT-001`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- P4 remains scope-frozen In progress with seven Complete and three unfinished
  children. P5 hide/show producers are Complete, so `VIS-P4-OBJECT-001` is the
  unique dependency-ready Active child.
- Read-only Legacy tracer `01a03714-50ab-7dc2-9e0f-7ccd267c4a5e` established
  exact map-gate, timer, MP, packet/source-order and absent-side-effect evidence.
  Read-only Go reviewer `01a03719-9519-7201-8674-ff4881bae13a` reported six
  initial test findings and two re-review findings; all were resolved, and its
  final report is `resolved, no finding`. Both threads are closed.
- No Go/server process remains.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD before the expected control commit:
  `19bc8768780542cdf23de90a9b223b7890e579db`.
- Tracked unstaged before this handoff write:
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md` and
  `tasks/migration-active.md`; this handoff is now also tracked unstaged.
  Staged and untracked sets are empty.
- The archive records the failed direction/nil-grid persistence assumptions,
  Legacy ruling and final verification. The Active Index routes only VIS.
- Control checker passes after this replacement. All three Legacy C# gates are
  empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `1aef1eac1953ca184d7e96c5394ae3c2cc5a3cc3`
  (`Complete authenticated no-reincarnation map gate`).
- Worktree, index and untracked set are clean.
- The commit adds an imported-map authenticated denial/positive-control
  transcript, bounds every session read with deadlines/KeepAlive barriers, and
  synchronizes runtime spell direction into the durable character snapshot.
- Matrix closes the NoReincarnation leaf, records seven Complete/three unfinished
  P4 children and marks exactly `VIS-P4-OBJECT-001` Active.
- All three Go-repository C# gates are empty.

## Active leaf and protected work

- Active leaf: `VIS-P4-OBJECT-001`.
- Outcome: preserve Legacy player/NPC/monster Object* add/remove geometry,
  per-player NPC visibility filters and interactive Observer non-blocking/
  relative projection across join, map transition, town revive and appearance
  refresh, including every currently discarded imported NPC visibility field.
- Matrix anchors: exact VIS row plus only named visibility/Observer completion
  prose. Do not read another phase or the full matrix.
- Exact Go authority: `internal/legacyworld/database.go`,
  `internal/worlddata/world.go`, bounded `cmd/crystal-server/world.go`, and only
  focused import/visibility/session tests found by exact enumeration.
- Legacy authority: exact NPC visibility fields/load order and MapObject/
  PlayerObject add/remove/search/Observer paths located by `rg`; every C# file
  remains read-only.
- Unresolved work: finite field/source-order checklist; parser/export round trip;
  actor/NPC/monster/Observer recipient and geometry matrix across join/
  transition/revive/refresh; repeated/race/integration gates and review.

## Verification ledger

Final NoReincarnation leaf evidence:

- `gofmt` on the two owned Go files and touched-package compile: exit 0.
- Imported denial plus positive authenticated lifecycle and direct map-rule set:
  focused `-count=20`: exit 0.
- The same focused set under `-race -count=5`: exit 0.
- Final fresh unexcluded `go test ./... -count=1`: exit 0; server 78.901s.
- Final `go vet ./...` and `go build ./...`: exit 0.
- Go `git diff --check`, exact status/process review and all three C# gates:
  exit 0/empty before commit; committed worktree is clean.
- Test-driven review fixes replaced a self-referential MP oracle with Legacy
  cost 125, added bounded barriers/full payloads/deep semantic state checks, and
  exposed the stale logout-direction production defect. Final re-review reports
  no finding.
- Full repository race is not due: P4 is not closing, the leaf has focused race,
  the current cadence remains below eight commits/48 hours and the one-line
  snapshot sync has no broad concurrency impact.

## Exact recovery sequence

1. Verify the expected Legacy control commit delta, Go HEAD/clean status, unique
   VIS Active rows and both repositories' six C# gates.
2. Search the lessons archive with `VIS-P4-OBJECT-001`, NPC visibility,
   TimeVisible, Observer and Object add/remove keywords; read matching sections
   only.
3. Read exact VIS matrix/Legacy/Go authorities, enumerate the finite field and
   recipient checklist, then delegate bounded disjoint tracing/review to
   `luna_worker` before writing only the owned files.

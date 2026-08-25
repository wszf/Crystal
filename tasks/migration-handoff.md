# Crystal Go migration current handoff

Last updated: 2026-08-25 15:43 (Asia/Singapore)

This replace-in-place snapshot records the committed/reviewed VIS closure and
the synchronized route to the bounded P6 discovery leaf.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- The Active Index and Go matrix route exactly `DISC-P6-CLOSURE` Active after
  marking `VIS-P4-OBJECT-001` Complete; no P6 inventory work has started.
- Two initial `luna_worker` read-only reviews returned no report and were
  closed. Dedicated `codex-auto-review` then found six correctness gaps and
  three lifecycle/send-sequencing follow-ups; all were fixed. Final reviewer
  `01a037da-1df0-7b41-9f30-09d5308bc7c5` reports
  `resolved, no findings` and is closed.
- No `go` or `crystal-server` process remained at the final gate.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD before the expected control commit:
  `da05d7cb18380283fec0a2a32c4dfc2372990549`.
- Tracked unstaged files are `tasks/lessons.md`,
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`,
  `tasks/migration-active.md`, and this handoff.
- Untracked owned snapshot:
  `tasks/migration-handoff-archive/2026-08-25-1245-pre-vis-closure.md`.
  The staged set is empty.
- Active Index contains one Active leaf, `DISC-P6-CLOSURE`, and records VIS
  Complete. All three Legacy C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `1ba0e2fe754af9e2941506e813e2b9ea9780c4c9`
  (`Complete object visibility equivalence`).
- Tracked unstaged, staged and untracked sets are empty.
- Matrix marks VIS Complete, P4 eight Complete/two dependency-blocked, and
  `DISC-P6-CLOSURE` Active. All three Go-repository C# gates are empty.

## Active leaf and protected work

- Active leaf: `DISC-P6-CLOSURE`; VIS is committed and the accepted P1-P5
  registries/implementations remain protected during this documentation-only
  discovery leaf.
- P6 outcome is a finite inventory/equipment/item movement/use/drop/storage/
  trade/rental/repair/refine/craft registry only; production/test Go remains
  read-only during discovery.
- Read only the P6 summary row and exact named P6 completion prose. Do not read
  another phase or the full matrix.
- VIS now preserves v63-v117/JSON NPC fields/defaults, UTC-monotonic scheduling,
  ordinary CallNPC visibility authorization, deterministic geometry/order,
  cache generation across bootstrap/logout/Observer exits, one bounded FIFO NPC
  packet batch, and complete live map/teleport/revive player projection.

## Verification ledger

- Touched-package compile: exit 0.
- Parser/world/session focused repetition at `-count=20`: exit 0.
- Focused race at `-count=5` plus final cache/lifecycle race at `-count=10`:
  exit 0.
- The first post-review full run exposed and corrected an invalid hidden
  conquest-controller fixture and restored the accepted NPC load sort.
- Final fresh unexcluded `go test ./... -count=1`: exit 0; server 79.853s.
- Final `go vet ./...`, `go build ./...`, Go/Legacy `git diff --check`, control
  checker, status/process review and all six C# gates: exit 0/empty.
- Full repository race is not due: P4 is not closing, focused race covers the
  concurrency change, and the cadence remains below the full-race threshold.

## Exact recovery sequence

1. Commit the exact owned Legacy control/archive files, then verify both
   repositories' HEAD/status and all six C# gates.
2. Resume only `DISC-P6-CLOSURE`: search matching lessons, read its named P6
   matrix anchors, perform bounded independent Legacy/Go audits, and obtain
   review of the finite child registry before selecting one Ready child.

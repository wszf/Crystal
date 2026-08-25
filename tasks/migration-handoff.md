# Crystal Go migration current handoff

Last updated: 2026-08-25 10:49 (Asia/Singapore)

This replace-in-place snapshot records committed `MAP-P4-LIGHT-001` closure and
synchronized routing to `DISC-P5-CLOSURE`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- Read-only Legacy tracer `01a036b2-225c-7131-9ee4-c78436134abd`
  (`luna_worker`, `gpt-5.6-luna/max`) returned the exact UTC/Stopwatch, two-cycle,
  global broadcast/bootstrap and wire/client evidence and is closed.
- Read-only Go reviewer `01a036c3-c7ab-74b0-8f2f-694afb2c1a50` initially treated
  NPC MAPLIGHT as local time; exact Legacy global-LightSetting authority rejected
  that finding, and focused re-review reports `resolved, no remaining finding`.
  It is closed. No Go/server process remains.
- Matrix and Active Index close MAP light, leave P4 scope-frozen In progress with
  six Complete/four dependency-blocked children, and activate bounded P5 discovery.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed HEAD before the expected control commit:
  `72362f78d98ae8811ca9b41207313dae40e5cda7`.
- Tracked unstaged before this refresh: `tasks/lessons.md`,
  `tasks/lessons-archive/verification/fixtures-and-transcripts-01.md`, and
  `tasks/migration-active.md`; this handoff is now also tracked unstaged. Index
  and untracked set are empty.
- Lessons record the exact shell/path/patch failures, modulo second-cycle review
  finding, disk-cache failure/recovery and MAPLIGHT reviewer ruling. Active Index
  routes matrix-only `DISC-P5-CLOSURE` with no Go-code authority.
- Control checker and diff check pass; all three Legacy C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `fc6b98b6b312357d850f6710fbf697b8678fd4c2`.
- Worktree and index are clean.
- The committed leaf installs one UTC startup/monotonic elapsed clock without replacing
  injected clocks, evaluates all 24 UTC hours across both modulo cycles, emits
  global ordinal-61 one-byte transitions only on change, and uses the same
  snapshot for StartGame/NPC MAPLIGHT. Existing exact map payload locks static
  Lights/Lightning/Fire/MapDarkLight separately.
- Matrix closes MAP light, records six Complete/four blocked P4 children, and
  marks `DISC-P5-CLOSURE` Active. Go diff check and all three C# gates are empty.

## Active leaf and protected work

- Active leaf: `DISC-P5-CLOSURE`.
- Outcome: finite, non-overlapping P5 spell/combat/AI/death/drop/respawn/packet
  child registry with completed evidence, dependencies and ownership; no behavior
  implementation.
- Matrix anchors: exact `| P5 |`, every `| P5 ` row and P5-named completion/
  residual prose only; never another phase or the full matrix.
- Exact Go authority after MAP-light commit: bounded P5 evidence in
  `docs/migration-matrix.md` only. Legacy control routing may change; no Go source,
  test or C# file is writable during discovery.

## Verification ledger

Final MAP-light evidence:

- `gofmt` on the three owned Go files: exit 0;
- touched compile `go test ./cmd/crystal-server -run '^$'`: exit 0;
- five exact server light/NPC/bootstrap/production tests at `-count=20`: exit 0;
- three protocol ordinal/payload/static-map tests at `-count=20`: exit 0;
- the same server/protocol sets under focused race `-count=5`: exit 0;
- fresh unexcluded `go test ./... -count=1`: exit 0, server 77.657s;
- `go vet ./...`, `go build ./...`, and final `git diff --check`: exit 0;
- one focused race build failed before tests with `no space left on device` at
  257 MiB free; `go clean -cache -testcache` restored 76 GiB, then touched compile
  and the exact race command passed;
- independent focused re-review: `resolved, no remaining finding`.

Full repository race is not due: P4 remains In progress, this leaf has focused
race and fresh integration, and the preceding MAP-detail leaf had the current
protocol integration gate.

## Exact recovery sequence

1. Verify the expected Legacy control commit, committed Go MAP-light hash, clean
   Go worktree and both repositories' six C# gates.
2. Begin `DISC-P5-CLOSURE` from its exact phase-local anchors with no code writes.

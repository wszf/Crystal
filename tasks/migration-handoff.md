# Crystal Go migration current handoff

Last updated: 2026-08-26 05:55 (Asia/Singapore)

This replace-in-place snapshot records the verified Mine/Rubble leaf closure
candidate and the route to the next bounded map-hazard leaf. It claims no phase
or project closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `gpt-5.6-luna/max` through `luna_worker` without silent substitution.
- P6 is scope-frozen In progress: twelve of nineteen children are Complete and
  seven are Ready. `MINE-P6-RUBBLE-001` is committed Complete.
- `SPELL-P5-MAP-HAZARD-001` is the unique Active leaf. P5 is scope-frozen with
  eight of eleven children Complete, this one Active and two Ready.
- Its matrix anchors are P5 summary row 851, registry row 3162 and completed
  Mine evidence 3206-3231. No hazard implementation has begun.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed HEAD:
  `33036b56e94884722f326579c1a6f1862d72a1a9`
  (`Reconstruct mining implementation handoff`).
- Unstaged tracked changes before this handoff refresh are exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
- This handoff is the expected fifth tracked modification after replacement;
  the index is empty and there are no untracked files.
- Tracked, staged and untracked Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `3f76a57b2059e6a43f2edf1f31b8430a20a347a4`
  (`Complete P6 Mine and Rubble lifecycle`).
- Worktree and index are clean; there are no untracked files. Tracked, staged
  and untracked Go `.cs` gates are empty.

## Mine closure evidence

- Legacy tracing covered Mines settings, MineInfo, Map.CreateMine,
  HumanObject.Attack/Mining/GetMinePayout/CompleteMine, PlayerObject inherited
  attack/quest behavior and MapObject/SpellObject spawning.
- Production implements parser/export/schema, ordered map zones, attack and
  mounted gates, exact random ordering including zero/unit bounds, strict spot
  regeneration, Rubble spawn/refresh/effect/expiry, payout/quest fanout,
  persistence and restart boundaries.
- Luna terminal reviewer `01a03ab3-90e8-7de3-8f63-135618c86d34` found mounted
  spell coercion, accumulated quest-notification and degenerate-RNG issues.
  After fixes, its current-diff follow-up returned `No findings`; the thread is
  closed.

## Verification ledger

All commands below ran in the Go root and exited 0 after the final fixes:

- touched compile:
  `go test ./cmd/crystal-server ./internal/legacyworld ./internal/protocol ./internal/worlddata -run '^$'`
- focused repeated: Mine/session `-count=20`, legacyworld/parser/export
  `-count=20`, protocol MapEffect/ordinals `-count=20`, worlddata schema
  `-count=20`.
- focused race: the same four package groups at `-count=5`.
- fresh integration: `go test ./... -count=1 -timeout=30m`.
- `go vet ./...` and `go build ./...`.
- fresh full race: `go test -race ./... -count=1 -timeout=45m`.
- `gofmt -d` on all fifteen owned Go files and `git diff --check` produced no
  output; exact status and all six C# gates were reviewed after tests.

## Active leaf and protected work

- Active leaf: `SPELL-P5-MAP-HAZARD-001`.
- Candidate hazard ownership is limited to bounded worlddata/legacyworld map
  schema and tests, `cmd/crystal-server/world.go`, new `map_hazards.go`,
  `map_hazards_test.go` and one authenticated session test. Exact filenames and
  source-order behavior must be frozen in the Active Index before code.
- Committed Mine closure, every earlier migration commit and every C# file in
  both repositories remain protected.

## Exact recovery sequence

1. Verify both repositories separately, including exact status and six C#
   gates; run `tasks/check-migration-control.sh` in Legacy.
2. Read only Go matrix rows 851, 3162 and 3206-3231; search the lessons archive
   with `SPELL-P5-MAP-HAZARD-001`, MapLightning, MapLava, hazard, timer and RNG.
3. Trace bounded Legacy/Go entry points, freeze exact hazard filenames and
   source-order acceptance details in the Active Index, then begin code.

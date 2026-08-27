# Crystal Go migration current handoff

Last updated: 2026-08-27 21:22 (Asia/Singapore)

This replace-in-place snapshot closes the committed dynamic-constructor leaf
and routes the same active Goal to the final frozen P5 ordinary-spawn child.
There is no uncommitted Go implementation for the new leaf.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; workers use `luna_worker` with
  `gpt-5.6-luna/max`.
- Dynamic constructor `MONSTER-P5-DYNAMIC-CONSTRUCTOR-003` is Complete at Go
  `9415ba8ed3594256f3ede8221e6459392c62a018`.
- Active leaf is `MONSTER-P5-ORDINARY-SPAWN-001`. P5 is scope-frozen with
  twenty of twenty-one children Complete and this final child Active.
- Recovery reads only matrix P5 summary row 851, registry rows 3175-3177 and
  dynamic-constructor evidence rows 3230-3249. Do not read the full matrix.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `cf608d40970cd097302687c14a26e3b5cbace9b3`
  (`cf608d40 docs: route dynamic monster constructor leaf`); upstream
  `origin/master`, ahead 490 and behind 0 at observation.
- Index is empty and there are no untracked files. Five owned tracked files are
  modified before the expected Legacy documentation commit:
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
  - `tasks/lessons-archive/migration/monster-natural-regeneration.md`
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
- The Active Index routes ordinary ownerless map respawn and protects the
  committed dynamic/BaseFamily/natural-Regen/specialized-constructor leaves.
- Tracked, staged and untracked `.cs` checks are all empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD
  `9415ba8ed3594256f3ede8221e6459392c62a018`
  (`9415ba8 Complete dynamic monster constructor convergence`); no upstream.
- Worktree and index are clean; there are no untracked files.
- Matrix P5 is 20/21 Complete, dynamic is Complete and ordinary spawn is Active.
- Tracked, staged and untracked `.cs` checks are all empty.

## Active leaf and protected work

- Active leaf: `MONSTER-P5-ORDINARY-SPAWN-001`.

- Outcome: preserve direct ownerless map-respawn `AI=60-63,99` constructor,
  target/lifetime/death and respawn/restart behavior, distinct from completed
  summon-created paths.
- Go write authority is bounded to `cmd/crystal-server/archer_summons.go`,
  `hell_lord.go`, `world.go`, new `monster_ordinary_spawn_test.go` and one
  bounded authenticated transcript.
- No production or test write has begun for this leaf. First freeze the exact
  five Legacy factory branches/classes and constructor RNG/field order.
- Protected commits: dynamic `9415ba8ed3594256f3ede8221e6459392c62a018`,
  specialized `8c16d9e2a8d479091c43ab379bb090a09a0ef946`, natural Regen
  `fc0c66cc9de88db30c752bf1a430e58764605804` and BaseFamily
  `3818544beea374ab47ee96bc59b9514dd1a1b476`.

## Open review findings

- Dynamic constructor has no accepted open finding. Main ruled the prior
  RootSpider/HellLord/SepHighTaoist/StoneTrap/SummonSkeleton/Mirroring findings
  from current Legacy source and added production-entry evidence.
- Main terminal review additionally corrected constructor-before-invalid-Spawn
  ObjectID/RNG order for HornedCommander and TrapRock, plus pre-derived first
  packets for restored/item-summoned derived pets.
- Read-only Luna workers `01a0431c-5f2f-7772-a941-dcac6ef04386` and
  `01a04349-736b-72b0-ae15-892caedb6227` returned no report despite bounded
  stop requests; both were closed with zero writes and provide no verdict.
- No ordinary-spawn behavior ruling has been made yet.

## Verification ledger

All commands below ran from the Go root unless stated otherwise.

- `go test ./cmd/crystal-server -run '^$' -count=1`: exit 0.
- Focused dynamic/protected/creator group `-count=10`: exit 0 in 27.361s.
- Same focused group `-race -count=3`: exit 0 in 15.558s.
- HumanWizard restored production transcript `-count=10`: exit 0 in
  23.053s; exact race `-count=3`: exit 0 in 8.811s.
- An intermediate full package run failed the stale HumanWizard first-packet
  expectation and was corrected from Legacy `HumanWizard.Spawned/GetInfo`.
- The next full package run hit `TestSessionHidingTranscriptPersistenceAndExpiry`
  at its established 30-second closed-pipe fixture. Its exact count-10 run
  reproduced three timing failures, an immediate exact count-1 rerun passed,
  and no stack entered the dynamic diff.
- Fresh `go test ./cmd/crystal-server -count=1 -timeout=900s`: exit 0 in
  80.864s.
- Fresh `go test ./... -count=1 -timeout=900s`: exit 0; server package
  85.410s and every other package passed.
- `go vet ./...` and `go build ./...`: exit 0.
- Fresh `go test -race ./... -count=1 -timeout=1800s`: exit 0; server
  package 96.831s and every other package passed.
- Final Go gofmt-diff, `git diff --check` and touched compile: exit 0.
- Legacy control check/diff check and both repositories' three `.cs` gates:
  exit 0/empty. No `go` or `crystal-server` process remains.

## Exact recovery sequence

1. Read this handoff and `tasks/migration-active.md`; run
   `tasks/check-migration-control.sh`, then verify each repository's branch,
   HEAD, clean/index/untracked state and all three `.cs` gates separately.
2. In Go, read only matrix row 851, rows 3175-3177 and evidence 3230-3249;
   verify HEAD `9415ba8ed3594256f3ede8221e6459392c62a018` is clean.
3. Search matching archive sections only with ordinary-spawn, `AI=60`,
   `AI=61`, `AI=62`, `AI=63`, `AI=99`, ownerless, respawn and restart
   keywords. Do not inventory another phase.
4. Freeze the exact five Legacy factory/concrete constructors and direct
   `materializeRespawnMonsterAtLocked` path before any Go production write;
   keep all summon-created/dynamic behavior protected.
5. Implement only within registered authority, run the ordinary leaf gate and
   due integration/full-race gates, then update matrix/control documents and
   commit owned files. The overall Goal remains Active.

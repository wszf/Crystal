# Crystal Go migration current handoff

Last updated: 2026-08-24 03:35 (Asia/Singapore)

This replace-in-place snapshot closes `LOC-P1-CATALOG-001` in committed Go and
routes the dependency-ready `LOG-P1-CATEGORY-001`. LOG writes remain gated only
until the enclosing Legacy control commit is clean.

## Goal and control-plane state

The persistent full Go migration Goal remains active, not Complete or externally
Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers use
`luna_worker` (`gpt-5.6-luna/max`). P1 remains scope-frozen and In progress with
nine unfinished finite leaves.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before the enclosing control commit:
  `master...origin/master [ahead 422]`.
- Observed HEAD: `a20d2a8d2ff7be81b13856f4d54b07de8ce9ec3e`
  (`docs(migration): expand P1 catalog loader authority`).
- Expected enclosing subject: `docs(migration): route P1 logging categories`.
  Recovery may observe exactly this one documentation commit delta.
- Staged and untracked files: none.
- Tracked modifications are exactly `tasks/lessons.md`,
  `tasks/migration-active.md`, and this handoff after its replacement.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode; every C# file
  remains unchanged.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`.
- HEAD: `d21681845090f0030e8f214628fd9aa3d60172b7`
  (`feat(config): complete Legacy server text catalog`).
- Worktree, staged files, and untracked files are clean.
- Matrix marks LOC catalog Complete, P1 scope-frozen/In progress, and nine named
  residual leaves.

## Active leaf and protected work

- Active leaf: `LOG-P1-CATEGORY-001` for routing only until the expected Legacy
  commit is clean.
- Outcome: production wiring for Server/Chat/Debug queue+file and Player/Spawn
  file-only logging with Legacy category, cap, layout, and sink-failure behavior.
- Matrix anchors: `### 2026-08-24 P1 finite closure inventory`, the LOG category
  row, and the row beginning `| P1 |`.
- Legacy read authority: `Server/Logger.cs`, `Server/MessageQueue.cs`,
  `Server.MirForms/SMain.cs`, `Server/MirObjects/Player/Reporting.cs`,
  `Server/MirEnvir/Map.cs`, `Server.MirForms/log4net.config`, and bounded direct
  startup/dequeue consumers.
- Go owned files after the gate: `internal/logging/logging.go`,
  `logging_test.go`, bounded logging setup/calls in `cmd/crystal-server/main.go`,
  and new `runtime_logging.go`/`runtime_logging_test.go`.
- Forbidden scope: broad call-site closure, localization, network services,
  lifecycle redesign, unrelated config/persistence/protocol, and C#.

## Verification ledger

- Go-only catalog generation and self-contained tests lock 768 unique defaults
  from `Shared/Language.cs` SHA-256
  `a196217d274d95db85559a0628152a42de48388f9e778e9daac4c952094c89e6`.
- Exact English/Chinese assets each contain 766 Text and zero Enum entries at
  SHA-256 `e1f7283042702b219fc46ee30cf299dcab0684f2015026a34899db942561ecfd`
  and `b6a3b76c2f6c5c9032979da08b7880feef4c289c7a4634b6d772765197fecb53`;
  all shared placeholders match. `HeroDesummonCountdown` and
  `StoragePasswordCleared` are the only English-default fallbacks.
- `go test ./internal/config -run '^$'`: exit 0.
- Focused catalog/loader tests at `-count=20`: exit 0; focused race: exit 0.
- `go test ./cmd/crystal-server -count=20 -run
  '^TestProductionConfigPathLoadsAndCreatesLegacySetup$'`: exit 0.
- `go test ./...`: unexcluded exit 0 after the LOC production changes.
- Fresh `go test ./... -count=1`: exit 1 only at established unrelated
  `TestSessionYinDevilNodeTranscript/42`, empty IDs versus `[84]`; the exact
  isolated `-count=10` run also exits 1 with no LOC file in its stack.
- `go test ./... -count=1 -skip '^TestSessionYinDevilNodeTranscript$'`: exit 0;
  this is an excluded attribution pass, not a full pass.
- `go vet ./...`, `go build ./...`, and `git diff --check`: exit 0. Full race was
  not due; the leaf introduced no shared mutable catalog state.
- Both repositories' tracked/staged/untracked C# gates are empty.

## Quiescence

- Go-only catalog writer `01a03007-6e3f-7b82-bde9-62f4b51a405a` is closed.
- Earlier loader and invalid Python auditors are closed; the Python report was
  excluded from acceptance and independently replaced by Go-only evidence.
- No subagent result is pending and no `go` or `crystal-server` process remains.

## Exact recovery sequence

1. Verify the expected Legacy control commit and clean status; verify Go HEAD and
   clean status separately, including all six C# gates.
2. Read only the named LOG matrix anchors, run the targeted archive search, and
   trace the bounded Legacy/Go logger authorities.
3. Derive the finite category/queue/layout/failure checklist, delegate at most
   one disjoint bounded wave, then implement only the registered LOG write set.

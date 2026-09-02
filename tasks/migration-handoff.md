# Crystal Go migration current handoff

Last updated: 2026-09-03 07:50 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB NPCInfo writer is complete in Go `b08a229` with
  documentation commits `384e6d8`/`dbabaeb`; QuestInfo header writer is complete
  in Go `00f9d40` with matrix commit `d196bae`.
- QuestPath `.txt` sidecar ownership, editor counters, unified WorkLoop and complete
  multi-store restart/recovery remain outside these bounded writer slices.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; HEAD `95e7af11ae17dc3b2f8802b4999bfc1510ff8809`.
- Worktree is clean. Tracked, staged and untracked `.cs` queries are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD `d196bae5ae2e0b301a849bd6bb15aac1ceb10fed`,
  pushed to `origin/migrate/drop-owner-p12`.
- Worktree is clean. Tracked, staged and untracked `.cs` queries are empty; no owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- Current bounded routing is `WS-PERSIST-P12-RESTART-OWNER-TRACE-001`, blocked-external:
  no dependency-ready P12 implementation child is safe to select.
- The owner trace confirms QuestInfo binary metadata needs explicit FileName input and
  separate QuestPath sidecar ownership. The header writer deliberately does not claim
  sidecar/text migration, runtime ObjectID bindings or editor-counter restoration.
- Do not claim P12 restart-equivalence or overall Goal completion. All `.cs` remains
  permanently read-only.

## Verification ledger

- `go test ./internal/legacyworld -run '^(TestWriteWorldDatabase|TestWorldDatabase)' -count=20` → 0.
- `go test -race ./internal/legacyworld -run '^(TestWriteWorldDatabase|TestWorldDatabase)' -count=5` → 0.
- `go vet ./internal/legacyworld` and `go build ./...` after the Quest writer → 0.
- `go test ./... -skip '^TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin$' -count=1` → 1.
  Latest run failed at `TestProductionStartupReturnsBootstrapError` and
  `TestProductionRuntimeBootstrapPrecedesGameBind`: missing world JSON is now created
  and loaded as an empty export, so the observed error is `Cannot start server without
  atleast 1 Map and StartPoint.` instead of the tests' expected world-export error.
  This is the external world-create/startup-owner change, not the MirDB writer slices.
  The registered Quest baseline remains excluded; no full-pass claim is made.

## Exact recovery sequence

1. Verify both repositories independently and preserve the active-index edit; do not
   reset, stash, clean or overwrite parallel work.
2. Read `tasks/migration-active.md` and P12 matrix anchors only; keep
   `DISC-P12-CLOSURE` active and the owner-trace blocker explicit.
3. Do not implement QuestPath sidecars, editor-counter restoration, MirDB production
   path wiring, unified WorkLoop, manifest/backup/restore or crash recovery until a
   shared owner and dependency contract are assigned. Never write `.cs`.

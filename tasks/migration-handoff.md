# Crystal Go migration current handoff

Last updated: 2026-09-03 06:58 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB ItemInfo write is Complete in Go
  `0df8e36d5a1e0fb57e26c6e98a9a28610daca106`. Monster/NPC rewrite is still
  unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `529788924fe36b3b717791d683968df42ab9b932`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `0df8e36d5a1e0fb57e26c6e98a9a28610daca106`.
- Unrelated pre-existing unstaged files (not this leaf): `docs/migration-matrix.md`,
  `cmd/crystal-server/conquest_save_test.go`, `internal/auth/conquest.go`,
  `internal/auth/guild.go`, `internal/auth/guild_progression_buffs.go`,
  `internal/protocol/guild.go`, and untracked `internal/auth/p12_needsave_test.go`.
  Preserve them. Matrix evidence for this leaf is in the active index until those
  overlapping matrix edits can be committed separately.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldDatabaseCatalog now emits current-layout ItemInfo records with
  stats and tooltip flags. Maps/respawns remain supported.
- Do not claim monster/NPC/quest MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- Item and map round-trip tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Preserve the unrelated Go unstaged
   files. Resume only `DISC-P12-CLOSURE`.
2. Treat ItemInfo MirDB write as a completed input, not monster/NPC rewrite.
3. Continue owner tracing for remaining catalog sections. Do not write C#.

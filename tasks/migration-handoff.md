# Crystal Go migration current handoff

Last updated: 2026-09-03 06:52 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB map write is Complete in Go
  `bc6f1c231fd0c6d795063af4b70d64e117f7ce86`. Item/monster/NPC rewrite is still
  unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `8789394c1e04073d8a6c482c1dc8da67e9e42098`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `bc6f1c231fd0c6d795063af4b70d64e117f7ce86`.
- Unrelated pre-existing unstaged files (not this leaf): `internal/auth/conquest.go`,
  `internal/auth/guild.go`, `internal/auth/guild_progression_buffs.go`,
  `internal/protocol/guild.go`, and untracked `internal/auth/p12_needsave_test.go`.
  Preserve them.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldDatabaseMaps now emits current-layout maps with safe zones, mines,
  movements and matching respawns.
- Do not claim item/monster/NPC/quest MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- Map round-trip and empty MirDB tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Preserve the unrelated Go unstaged
   files. Resume only `DISC-P12-CLOSURE`.
2. Treat map/respawn MirDB write as a completed input, not item/monster rewrite.
3. Continue owner tracing for remaining catalog sections. Do not write C#.

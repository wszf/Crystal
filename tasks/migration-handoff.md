# Crystal Go migration current handoff

Last updated: 2026-09-03 07:06 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB MonsterInfo write is Complete in Go
  `befa4a9645f60080df2dc0978a55cf1b45246b00`. NPC/quest rewrite is still
  unselected. Unstaged NeedSave-transient edits exist in parallel and were
  preserved, not committed by this leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `94ed6caf80aceb81f473427cebb72fd5ce0d9eef`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `befa4a9645f60080df2dc0978a55cf1b45246b00`.
- Unrelated pre-existing unstaged files (not this leaf): `docs/migration-matrix.md`,
  `cmd/crystal-server/conquest_save_test.go`, `internal/auth/conquest.go`,
  `internal/auth/guild.go`, `internal/auth/guild_progression_buffs.go`,
  `internal/protocol/guild.go`, and untracked `internal/auth/p12_needsave_test.go`.
  Preserve them.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldDatabaseCatalog now emits current-layout MonsterInfo records. Loaded
  CanRecall stays false. NPC/quest sections remain empty.
- Do not claim NPC/quest MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- Monster, item and map round-trip tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Preserve the unrelated Go unstaged
   files. Resume only `DISC-P12-CLOSURE`.
2. Treat MonsterInfo MirDB write as a completed input, not NPC rewrite.
3. Continue owner tracing for remaining catalog sections. Do not write C#.

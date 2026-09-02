# Crystal Go migration current handoff

Last updated: 2026-09-03 07:26 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB NPCInfo write is Complete in Go
  `b08a22942bd5d2e11f31a8bfd1de431fd113a187`. QuestInfo rewrite is still
  unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `115a1fe6a9c81e016adbada0feb00ab445092373`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `b08a22942bd5d2e11f31a8bfd1de431fd113a187`.
- Unrelated unstaged file (not this leaf): `docs/migration-matrix.md`. Preserve it.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldDatabaseCatalogWithNPCs now emits current-layout NPCInfo.Save
  fields. Runtime ObjectID, script and shop goods remain outside this writer.
- Do not claim QuestInfo MirDB rewrite. All `.cs` remains read-only.

## Verification ledger

- NPC/monster/item/map round-trip tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Preserve the unstaged matrix edits.
   Resume only `DISC-P12-CLOSURE`.
2. Treat NPCInfo MirDB write as a completed input, not QuestInfo rewrite.
3. Continue owner tracing for remaining catalog sections. Do not write C#.

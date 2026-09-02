# Crystal Go migration current handoff

Last updated: 2026-09-03 07:48 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- Unique Active leaf is `DISC-P12-CLOSURE`. P12 remains Open/shared-owner for
  restart-equivalence. MirDB GameShop write is Complete in Go
  `1fbe5be8833214117a2b1d43827f4149ceefad9e`. Magics, ConquestInfo and quest
  `.txt` sidecars are still unselected.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch `migration/goal-orchestration`; pre-control-commit HEAD
  `95e7af11ae17dc3b2f8802b4999bfc1510ff8809`.
- Before this control refresh the index and worktree were clean except the
  expected unstaged `tasks/migration-active.md` and this handoff.
- This snapshot records the expected one control-document commit delta.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch `migrate/drop-owner-p12`; HEAD
  `1fbe5be8833214117a2b1d43827f4149ceefad9e`.
- Index and worktree are clean.
- `git diff --check` and all three Go C# queries exit 0/empty. No owned
  Go/server process is active.

## Active leaf and protected work

- Active leaf: `DISC-P12-CLOSURE`.
- WriteWorldDatabaseCatalogWithGameShop now emits GameShopItem.Save rows with
  gold/credit prices and buy flags.
- Do not claim MagicInfo, ConquestInfo or quest sidecar rewrite. All `.cs`
  remains read-only.

## Verification ledger

- GameShop plus prior catalog writer tests pass count 20 and race count 5.
- `go test ./internal/legacyworld -count=1`, `go vet ./internal/legacyworld` and
  `go build ./...` exit 0.

## Exact recovery sequence

1. Verify both repositories independently. Resume only `DISC-P12-CLOSURE`.
2. Treat GameShop MirDB write as a completed input, not magics/conquest-info.
3. Continue owner tracing for remaining catalog sections. Do not write C#.

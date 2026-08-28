# Crystal Go migration current handoff

Last updated: 2026-08-28 22:42 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-PANEL-ROUTING-001` is Complete in Go commit
  `4cd9f16e34021f59c378c8010239ea6849fc2eb4`. The unique next Active leaf is
  dependency-ready `NPC-P7-SHOP-QUIRKS-001`.
- Exact next matrix scope is row 192, routing ledger N row 214 and P7 summary row
  966 only. P7 remains frozen at 24 children: 12 Complete, 1 Active and 11 Ready.
- Legacy tracer `01a0487d-8d4c-7941-9a58-5ec1a94f1aab`, test writer
  `01a0488c-f789-7942-83c6-f7603ee980ec`, and terminal reviewer
  `01a048a6-1346-7721-b575-cfef4a6d1750` are closed. Terminal review revision 2
  returned `No findings`; no agent or test process remains active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `73a2ae248760c3a5f8fd58223eea5c6dd6d26698`;
  upstream `origin/master`, ahead 505 and behind 0.
- Unstaged owned paths before this snapshot are `tasks/lessons-archive/misc.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths and no Legacy implementation changes.
- The active index routes only corrected Shop Quirks. The archive records that
  Legacy uses a local Goods snapshot rather than shared/static count mutation.
- Run `tasks/check-migration-control.sh` after this replacement. Tracked, staged
  and untracked C# gates were empty at startup and must be rerun before commit.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `9218577032bcdfca73087aca72e7dd733714352e`;
  no upstream; index and worktree are clean.
- Commit `4cd9f16 Complete P7 NPC panel routing` removes missing-page special
  fallback, dispatches matched panels
  with the executed object identity, centralizes exact active-NPC/page admission,
  and admits craft from any segment-installed active page.
- Matrix row 191 and ledger M are Complete, corrected row 192 is Active, and the
  P7 summary is exactly 12+1+11. No Shop Quirks implementation or tests have
  started. Commit `9218577 Correct P7 shop quirks scope` preserves the
  source-authoritative no-growth contract.

## Active leaf and protected work

- Active leaf: `NPC-P7-SHOP-QUIRKS-001`.
- Outcome: stable per-open base-Goods snapshots without source-count mutation,
  enabled-only UsedGoods merge for BUY/BUYSELL, BUYNEW separation and
  `GoodsOn=false` BuyBack/BuyUsed panel absence with persistence/race bounds.
- Write authority is bounded `cmd/crystal-server/shop_transactions.go`, minimum
  world shop snapshot/gating symbols, new `npc_shop_quirks_session_test.go`,
  minimum existing shop tests, matrix/index and handoff.
- Complete Shop Core, Item Grid Mutation and Panel Routing are protected.
  Ordinary shop formulas, item-grid CAS semantics, conquest pricing, other
  panels, action/condition catalogs, callbacks, layouts and all C# files are
  forbidden.

## Verification ledger

- Owned gofmt and minimum server compile exit 0; `go mod verify` exits 0.
- `go test ./cmd/crystal-server -run '^TestNPCP7PanelRouting' -count=10` exits 0.
- Representative panel-positive count-3 and focused race count-3 commands exit
  0; the broader representative panel race command exits 0.
- Fresh `go test -count=1 ./...`, `go vet ./...`, and `go build ./...` each exit
  0. Full race passed fresh on the immediately preceding leaf and is not
  cadence-due after this single leaf; focused race is current.
- Initial exact-active-NPC fixture failed because authoritative ObjectIDs were
  loaded before a map existed; registering a real 20x4 map made the original
  count-10 pass, and focused race count-3 also passes. A test-review correction
  renamed the direct-storage negative for the gate it actually reaches and
  added an explicit matched/unmatched dispatch test.
- Independent terminal review revision 2 returns `No findings`. Residual
  session-level cross-NPC goods/rate routing would require callback-family scope;
  exact object forwarding is covered by dispatch and storage identity tests.
- Source correction: Legacy `NPCScript.Goods` is an instance list; BUY/BUYSELL
  copy its membership and append UsedGoods only to the local response list.
  BUYBACK first-open creates an owner key only when enabled; BUYUSED does not
  mutate its list. Only UsedGoods persists; BuyBack is restart-transient.
- Go matrix scope correction is committed; the matching Legacy control/archive/
  handoff correction commit remains before code work.

## Exact recovery sequence

1. Read this handoff and `tasks/migration-active.md`, then independently verify
   both repository HEAD/status/C# gates. Preserve the documented correction.
2. Run Legacy control/C#/diff checks and commit its three owned correction
   documents; verify the clean post-commit HEAD.
3. Resume only `NPC-P7-SHOP-QUIRKS-001`; freeze tests for stable `G/U` counts,
   enabled BUY=`G+U`, BUYNEW=`G`, first-open BuyBack owner-key behavior and
   disabled BUYBACK/BUYUSED response-only ordering before production edits.

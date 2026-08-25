# Crystal Go migration current handoff

Last updated: 2026-08-25 17:02 (Asia/Singapore)

This replace-in-place snapshot freezes the uncommitted implementation of the
unique active P6 shout-arming leaf after a new-session recovery mismatch.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- Legacy Active Index and Go matrix row 3197 now both mark the leaf `Active`.
  The mismatch discovered at recovery was repaired in the isolated Go matrix
  commit `3f7b29d41787a683c42d767ef321afbb4bec45c8` without changing the five
  implementation files.
- The first recovery command incorrectly combined both repositories with
  `git -C`; its entire output was discarded. Startup documents and repository
  facts were then reread in separate, zero-exit, complete calls.
- Read-only reviewer `01a03823-05a6-7270-8f06-0cd469a25bc2` was quiesced and
  closed. It wrote nothing and ran no tests; its unfinished review identified
  admission/chat ordering, world/auth atomicity and stale snapshots,
  interception boundaries, and test validity as remaining review points.
- No `go` or `crystal-server` process was present in either repository at the
  freeze.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD:
  `0022c3a076e585d825cc711be13f6b45f83ae21a`
  (`Route migration to P6 shout arming`).
- Before this snapshot refresh the tracked, staged, and untracked sets were
  empty. Expected current tracked unstaged files are `tasks/lessons.md` and
  this handoff; staged and untracked sets remain empty.
- `tasks/check-migration-control.sh` passed before this refresh. All three
  Legacy C# gates were empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `3f7b29d41787a683c42d767ef321afbb4bec45c8`
  (`Activate P6 shout arming leaf`).
- Tracked unstaged files are `cmd/crystal-server/main.go` and
  `cmd/crystal-server/world.go`. Untracked files are
  `cmd/crystal-server/item_shout.go`, `item_shout_test.go`, and
  `item_shout_session_test.go`; staged files are empty.
- The five file sizes/mtimes were stable across the post-review audit. The
  tracked diff has 42 insertions and one deletion; `git diff --check` passed.
- All three Go C# gates were empty. No implementation test, compile, race, vet,
  or build has run for this uncommitted batch.

## Active leaf and protected work

- Active leaf: `ITEM-P6-SHOUT-ARMING-001`.
- Outcome: preserve Scroll shape 8/9 common admission, independent transient
  one-shot map/server-shout arming, localized Hint, item consumption and exact
  successful `UseItem` order. Re-arming consumes another scroll; logout drops
  both flags.
- Matrix anchors are only the P6 summary row, exact
  `ITEM-P6-SHOUT-ARMING-001` row, and exact
  `CHAT-P4-MAP-CONTEXT-001` dependency row.
- Owned Go files are exactly the five listed above. P4 shout consumption,
  cooldown, linked-item projection, routing and recipients remain forbidden.
- Current candidate adds runtime flags to `worldPlayer`, resets them on
  enter/leave, intercepts shape 8/9 item use, persists consumed inventory, emits
  Hint then `UseItem`, and adds pure/world/authenticated session tests.
- Preserve all five uncommitted files. Do not reset, stash, clean, checkout,
  overwrite, or commit them before review and the required gates.

## Verification ledger

- Startup control documents were fully reread in separate Legacy calls.
- Exact named matrix rows were reread in a separate Go call; they exposed the
  `Active`/`Ready` inconsistency. The exact child row was changed to `Active`,
  `git diff --check` passed, and matrix-only commit `3f7b29d` succeeded.
- Legacy control checker before refresh: exit 0. Both repository status, HEAD,
  process review and three C# gates: exit 0/empty in separate calls.
- Go `git diff --check`: exit 0. No tests/builds have run for the candidate.

## Exact recovery sequence

1. Run the Legacy control checker and separately verify both repository
   statuses/C# gates; read this handoff back and confirm the five Go file
   size/mtime values are stable.
2. Review the five-file candidate against the already named Legacy and Go
   anchors, beginning with the four unresolved reviewer points; then run the
   leaf compile, focused `-count=20`, focused race `-count=5`, status, process,
   diff and six C# gates before any completion/routing commit.

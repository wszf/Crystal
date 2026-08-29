# Crystal Go migration current handoff

Last updated: 2026-08-29 11:03 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `REFINE-P6-WORKBENCH-001` is accepted Complete in Go commit
  `6408c1163c3a597c260416f3df25832b1af18e28`. P6 is scope-frozen at nineteen
  children: 13 Complete, one Active and five Ready.
- The unique next Active leaf is dependency-ready `ITEM-P6-GRID-CROSS-001` at
  matrix row 3540 and P6 summary row 965.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-control-commit HEAD
  `3e3f52f002eb0b3664033fbf2038d52e54a3a9e5`; upstream `origin/master`, ahead
  515 and behind 0.
- Owned unstaged paths are:
  - `tasks/lessons.md`
  - `tasks/lessons-archive/migration/data-config-import-export.md`
  - `tasks/lessons-archive/misc.md`
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-01.md`
  - `tasks/migration-active.md`
  - this handoff.
- There are no staged/untracked paths or Legacy implementation changes.
- `git diff --check` and all three Legacy C# queries are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `6408c1163c3a597c260416f3df25832b1af18e28`;
  no upstream. Index and worktree are clean.
- Commit `6408c11` contains exactly the accepted Refine production, tests,
  matrix evidence and next-Leaf routing. All three Go C# queries are empty.

## Active leaf and protected work

- Active leaf: `ITEM-P6-GRID-CROSS-001`.
- The completed Refine batch preserves exact admission, localized names,
  unchecked widths/formula/RNG/destruction, millisecond timing, packet order,
  full-bag cancellation, same-process workbench residue, remaining-ms restart,
  client clone fields, observer takeover cancellation and the Legacy rental
  expiry-invisibility quirk.
- Auth gold/inventory/refine mutation and world projection share the item
  revision; rejected callbacks do not overwrite dirty runtime grids. Periodic
  checkpoint semantics remain Legacy-authoritative rather than adding an
  immediate per-request save failure gate.
- The new Active leaf owns only the exact Move/Split/Merge cross-grid matrix and
  transaction/packet ordering. Completed inventory-only, Refine, Storage, Trade,
  Hero/mount and P11 attachment behavior are protected inputs.

## Verification ledger

- Owned gofmt and touched compile
  `go test ./internal/auth ./cmd/crystal-server -run '^$'` exited 0.
- Focused Refine/auth/observer/revision tests passed count 20; focused auth race
  passed count 5 and focused server race passed count 3. Protected observer,
  item-grid, Rental and admin dirty-world regressions passed repeated runs.
- The first fresh server package run failed only
  `TestAdminLogoutPreservesDirtyWorldItemsAcrossNonClearAuthRevision`: a no-op
  Refine admission snapshot incorrectly published an unrelated auth revision.
  The helper now publishes only committed Refine mutations; the exact regression
  passed count 20 and the subsequent fresh server package passed in 82.729s.
- Fresh unexcluded `go test -count=1 ./...` exited 0 with server 87.283s.
  Fresh unexcluded `go test -race -count=1 ./...` exited 0 with server 99.401s.
  `go vet ./...`, `go build ./...` and final `git diff --check` exited 0.
- Reviewer `01a04b3b-ef31-7332-a0bc-63f62d5c6772` was closed without evidence
  after launching duplicate gates and stalling. Replacement reviewer
  `01a04b60-f42d-7ae3-a8aa-839afc86ac69` raised two provisional findings;
  exact Legacy periodic-save and rental-workbench rulings plus regressions
  resolved both, and its terminal rereview returned `No findings`.
- Both reviewers are closed. No migration subagent or owned `go`/
  `crystal-server` process remains active.

## Exact recovery sequence

1. Rerun the Legacy control/C# gates, then commit only the six owned Legacy
   control/lesson paths. This handoff records the expected pre-control-commit
   Legacy HEAD and one documentation commit delta.
2. Verify both repositories independently. Resume only
   `ITEM-P6-GRID-CROSS-001`: read matrix row 3540 and P6 summary row 965, then
   search targeted archived lessons for Move/Split/Merge, storage/trade/Refine/
   Hero/fishing grid, revision and packet-order failures.
3. Before any Go write, trace the exact Legacy grid switches and freeze a finite
   positive/negative source-target matrix plus packet/persistence expectations.

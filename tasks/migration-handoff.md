# Crystal Go migration current handoff

Last updated: 2026-08-28 20:55 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-COND-LOCAL-001` is Complete in Go commit
  `424978eabe5de76d84979f3fe108bf7968173aa0`. The unique Active leaf is now
  dependency-ready `NPC-P7-PANEL-ROUTING-001`.
- Exact matrix scope is row 191, routing ledger M row 213 and P7 summary row 966
  only. P7 remains frozen at 24 children: 11 Complete, 1 Active and 12 Ready.
- Review worker `01a0483a-4db6-7dc3-ae53-21d8e18cb394` was closed after terminal
  revision 3 returned `No findings`. No agent or test process remains active.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-handoff-commit HEAD
  `978ca980599c7d8e724a1e197a51495d0505b608`; upstream `origin/master`, ahead
  503 and behind 0.
- Unstaged owned paths before this snapshot are `tasks/lessons.md`,
  `tasks/lessons-archive/misc.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths and no Legacy implementation changes.
- The active index routes only Panel Routing. Lessons preserve the ENOSPC,
  fixture, non-unique patch and terminal Local Conditions review evidence.
- `tasks/check-migration-control.sh` exits 0. Tracked, staged and untracked C#
  gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `424978eabe5de76d84979f3fe108bf7968173aa0`;
  no upstream. Index and worktree are clean.
- Commit `424978e Complete P7 NPC local conditions` changes bounded
  worlddata/auth/main/default context wiring and tests, adds CurrentCulture
  collation through `golang.org/x/text v0.41.0`, and updates the matrix.
- Matrix row 187 is Complete, row 191 is the sole Active row, ledger I contains
  accepted evidence, and P7 summary is exactly 11+1+12.
- No Panel Routing implementation or tests have started. There is no
  uncommitted Go work and no currently owned Go path modified.

## Active leaf and protected work

- Active leaf: `NPC-P7-PANEL-ROUTING-001`.
- Outcome: page-existence-only dispatch, silent missing pages, no direct storage
  bypass, executed-page storage identity, craft from any active page and bounded
  special-panel positive paths.
- Write authority is new bounded
  `cmd/crystal-server/npc_panel_routing.go`, page dispatch adapters in
  `cmd/crystal-server/main.go`, focused `npc_panel_routing_session_test.go`,
  minimum existing storage/craft/market/mail/guild/hero tests, matrix/index and
  handoff.
- Complete Access Gate, loading, Page Grammar, Speech/Input, Control Flow,
  Action State and Local Conditions are protected. Shop transaction quirks,
  conquest pricing, teleport actions, quest residuals, action/condition catalogs,
  callback-family execution, protocol layouts and every C# file are forbidden.

## Verification ledger

- Owned gofmt, `git diff --check`, minimum compile for auth/worlddata/server and
  `go mod verify` each exited 0.
- Final focused auth/worlddata/server commands with `-count=10` exited 0; the
  corresponding three focused `-race -count=3` commands exited 0.
- Fresh final `go test -count=1 ./...`, `go vet ./...`, `go build ./...`, and
  fresh unexcluded `go test -race -count=1 ./...` each exited 0.
- The first retained-count test failed because it attempted a fifth active
  character; it was corrected to tombstone one of four before replacement.
  A later whitespace test exposed a non-unique patch landing in CHECKGOLD; exact
  switch anchors fixed every TryParse-compatible branch. Both original focused
  commands then passed and every final full gate passed.
- Both repositories' tracked, staged and untracked C# gates are empty. Go is
  clean at the committed HEAD; Legacy contains only the five owned documents
  listed above.

## Exact recovery sequence

1. Read this handoff and `tasks/migration-active.md`, then separately verify the
   Legacy HEAD/status/control/C# gates and Go HEAD/status/C# gates. Account for
   the one expected Legacy documentation commit after the pre-commit HEAD above.
2. Read only matrix row 191, ledger M row 213 and P7 summary row 966. Search only
   matching Panel Routing archive sections; do not read the full matrix or open
   another leaf.
3. Start `NPC-P7-PANEL-ROUTING-001` with exact Legacy page lookup/ProcessSpecial,
   storage identity and craft admission tracing. Register no broader panel or
   business-logic authority.
4. Before implementation, freeze silent missing-page, GOTO/direct-storage,
   executed-page identity, alternate-page craft and special-panel ordering
   cases; then delegate at most one bounded read-only/writer wave.

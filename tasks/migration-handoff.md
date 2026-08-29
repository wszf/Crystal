# Crystal Go migration current handoff

Last updated: 2026-08-29 08:36 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `STORAGE-P2-NPC-GATE-001` is accepted Complete in Go commit
  `326fc4aeed5d6a12ef8021d08320f39d026f131c`. P2 is scope-frozen and Complete:
  all eight children are Complete, with fresh phase-closure full gates.
- The unique next Active leaf is dependency-ready `REFINE-P6-WORKBENCH-001` at
  matrix row 3554 and P6 summary row 965. P6 is frozen at nineteen children:
  12 Complete, 1 Active and 6 Ready.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; pre-control-commit HEAD
  `6415aee1ef9b0c761589341cde6d3bca5ea8b83d`; upstream `origin/master`, ahead
  514 and behind 0.
- Owned unstaged paths are
  `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/lessons-archive/workflow/shell-tools-and-patching.md`,
  `tasks/migration-active.md` and this handoff. There are no staged/untracked
  paths or Legacy implementation changes.
- The protocol archive records the source ruling that Storage page establishment
  and final helper revalidation are distinct; the workflow archive records the
  failed compound control patch and exact split recovery.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `326fc4aeed5d6a12ef8021d08320f39d026f131c`;
  no upstream; index and worktree are clean.
- Commit `326fc4a` contains exactly the new production-entry storage NPC gate
  transcript and matrix routing. Exact Legacy review proved existing Go
  production handlers already match, so no production code was changed.
- Go tracked/staged/untracked C# queries, gofmt and `git diff --check` were empty
  before commit.

## Active leaf and protected work

- Active leaf: `REFINE-P6-WORKBENCH-001`.
- The leaf owns delayed refine workbench lifecycle only: deposit, retrieve/
  cancel/start/check/collect, formula/RNG/destruction, strict deadline,
  first-free/full-bag behavior, exact packets and refine-field continuity.
- Completed P7 page authority, P6 item-grid/repair/craft, storage/P2 closure,
  protocol and generic persistence behavior are protected inputs.
- Allowed Go writes are bounded `cmd/crystal-server/refine_transactions.go`,
  refine-only production branches/session tests, one restart transcript, and
  control/matrix evidence.

## Verification ledger

- Storage gate touched-package compile, focused count 1 and count 20, protected
  storage/NPC-access regression count 3, focused race count 3, gofmt and
  `git diff --check` all exited 0.
- Authenticated transcripts prove all three exact wrong-stage/unavailable flag
  families and keepalive/no mutation; no page, later successful non-Storage,
  default pseudo-NPC, stale object, wrong map and production movement 16->17;
  rejected unauthorized page plus later visibility/death preserve Legacy stale
  authorization while object/map/range pass.
- Main reread the exact Legacy handler/page/helper ranges and corrected the
  initial over-strict visibility/death matrix before implementation. Existing Go
  production required no change. Terminal Luna review returned `No findings`.
- Fresh `go test -count=1 ./cmd/crystal-server` passed in 82.499s. Fresh
  unexcluded `go test -count=1 ./...` passed with server 84.430s; fresh
  unexcluded `go test -race -count=1 ./...` passed with server 100.659s;
  `go vet ./...` and `go build ./...` passed.
- Legacy tracer `01a04ad1-07ce-7261-80a1-4f42190b97af` and terminal reviewer
  `01a04ae2-3045-79e3-b2cf-e9da9929cce4` are closed. No migration subagent or
  owned `go`/`crystal-server` process remains active.

## Exact recovery sequence

1. Run `tasks/check-migration-control.sh`, Legacy diff/status and all three C#
   gates; commit only the four owned archive/index/handoff paths, then verify
   both repositories independently.
2. Resume only `REFINE-P6-WORKBENCH-001`: read matrix row 3554 and P6 summary row
   965, then search targeted archived lessons for refine/workbench/deadline/RNG/
   restart failures.
3. Trace Legacy deposit/retrieve/cancel/start/check/collect, formula/RNG,
   nonzero RefineTime deadline and save/load call chains before any Go write;
   freeze exact packet and restart transcripts first.

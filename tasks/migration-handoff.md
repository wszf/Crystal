# Crystal Go migration current handoff

Last updated: 2026-08-24 05:08 (Asia/Singapore)

This replace-in-place snapshot closes `NET-P1-GATES-001` and activates the
bounded status-monitor leaf. The persistent full migration Goal remains active.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 is scope-frozen/In progress with twelve registered children: six Complete
  and six unfinished. Matrix and Active Index route `NET-P1-STATUS-001` Active.
- No phase or Goal closure is claimed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before this control commit: `master...origin/master [ahead 429]`.
- HEAD before this control commit:
  `e5522a9b6ed29eb4ecfc7c929fdf77f7f0baaa64`
  (`docs(migration): reconstruct P1 network gate handoff`).
- Expected owned delta is `tasks/lessons.md`, the NET-specific archive section,
  `tasks/migration-active.md`, and this handoff. Staged/untracked files were
  otherwise empty before the edit.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- Tracked, staged, and untracked C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `46c1b81df12ac88bc662eaa43ffa2b69a7dd6f0b`
  (`feat(network): preserve Legacy connection gates`).
- Worktree, index, and untracked set are clean. A package-level build briefly
  produced a root `crystal-server` binary; it was identified as this batch's
  Mach-O artifact and removed before commit.
- Tracked, staged, and untracked C# gates are empty.

## Completed NET gate leaf

- Process-wide authority preserves Legacy colon-split IP keys, silent
  equality-inclusive/lazy-expiry short blocks, configured overwrite, MaxIP
  zero/count/log/release, first accept, and MaxUser re-arm backpressure.
- MaxUser waits wake on release, context, or fatal stop. Unexpected accept errors
  enter common cleanup. Ordinary shutdown sends reason 0; fatal shutdown sends
  reason 3 then 0 even while capacity is full.
- An atomic 8 KiB receive parser counts callbacks rather than frames, validates
  a whole callback before dispatch, preserves partial/coalesced framing, anchors
  a strict five-second MaxPacket window at first receive, and installs a 24-hour
  block on rate or invalid framing.
- Successful/MaxIP/invalid/rate logs use loaded Legacy templates and exact
  distinct `ClientPacketIds` history. Receive errors retain the 500 ms Legacy
  disconnect grace; idle timeout is absolute across maintenance wakes.
- Shared account/character creation logs preserve the first-untracked attempt,
  `>2`/`>4` pre-prune checks, one-expired-entry cleanup, sticky threshold quirk,
  result 0, and 24-hour overwrite. Bootstrap failure and all shutdown paths
  release active counts.

## Verification ledger

All final commands below ran in the Go root and exited 0:

- Owned-file gofmt and `go test ./cmd/crystal-server -run '^$'`.
- Focused deterministic gate/parser/TCP/net.Pipe/session/lifecycle regex at
  `-count=20 -timeout=5m`.
- The same focused set under `go test -race` at `-count=5 -timeout=5m`.
- Fresh unexcluded `go test ./... -count=1 -timeout=15m`.
- Fresh unexcluded `go test -race ./... -count=1 -timeout=20m`.
- `go vet ./...`, `go build ./...`, and `git diff --check`.

An earlier package-only run reproduced the established unrelated
`TestSessionYinDevilNodeTranscript/42` empty-notification flake. Its isolated
`-count=20` rerun passed, and the later fresh unexcluded full test passed. Early
compile/test failures from a partial reader interface, unused import, mistaken
Legacy enum/IPv6 expectations, and an existing same-IP fixture were corrected
and preserved in lessons; none remains in final evidence.

## Active leaf and protected work

- Active leaf: `NET-P1-STATUS-001`.
- Outcome: fixed port 3000 status listener, five-connection cap, exact ten-second
  ASCII status cadence, timeout/disconnect, and bounded stop behavior.
- Matrix anchors: P1 finite inventory, `NET-P1-STATUS-001`, and P1 stage row.
- Legacy read authority: bounded status ranges in `Envir.cs` and
  `MirStatusConnection.cs`, plus direct timer/count consumers only.
- Go write authority: bounded `cmd/crystal-server/main.go`, plus new
  `status_service.go` and `status_service_test.go`.
- Forbidden: game gate redesign, HTTP, broad lifecycle, logging/localization,
  unrelated persistence/protocol, and C#.

## Quiescence

- Read-only `luna_worker` `01a03055-b94f-7ce0-b9b9-6e2a39c4ade9` completed,
  returned seven bounded NET findings, changed no file, and was closed.
- No subagent result is pending. No `go` or `crystal-server` test process remains.

## Exact recovery sequence

1. Verify both clean worktrees, matrix/index Active agreement, control checker,
   and all six C# gates; commit only the four owned Legacy control files.
2. Read only the named STATUS matrix anchors and bounded Legacy status ranges;
   search the lessons archive with `NET-P1-STATUS-001`, `MirStatusConnection`,
   cadence, timeout, and shutdown.
3. Derive the finite payload/timer/admission/shutdown checklist, then create only
   `status_service.go` and its test before bounded `main.go` integration.
4. Run focused/repeated/race and the standard Leaf gate; update matrix/index/
   handoff and commit each repository separately.

# Crystal Go migration current handoff

Last updated: 2026-08-24 04:39 (Asia/Singapore)

This replace-in-place snapshot reconstructs the current `NET-P1-GATES-001`
candidate after startup verification found the prior clean-worktree claim stale.
The persistent full migration Goal remains active.

## Goal and control-plane state

- Main authority is `gpt-5.6-sol/ultra`; bounded workers use `luna_worker`
  (`gpt-5.6-luna/max`).
- P1 is scope-frozen/In progress with one Active leaf. Matrix and Active Index
  both route `NET-P1-GATES-001`; no phase or Goal closure is claimed.
- Owned Go paths are exactly `cmd/crystal-server/main.go`, `main_test.go`,
  `connection_gates.go`, and `connection_gates_test.go`.
- Forbidden scope remains status port 3000, HTTP, lifecycle redesign, broad
  logging/localization closure, unrelated protocol/persistence, and all C#.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master...origin/master [ahead 428]`.
- HEAD before this reconstruction commit:
  `6cba22f25d1aa90cd4da9dbc21289ccdff4901cf`
  (`docs(migration): checkpoint P1 network gate implementation`).
- Before this control edit the worktree, index, and untracked set were clean.
  Expected owned control delta is this handoff, the bounded Active Index wording,
  and two strengthened canonical recovery lessons.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.
- Tracked, staged, and untracked C# gates were empty at recovery.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `1878cad4b9e1e91214d7756c767180dc45db49ac`
  (`docs(migration): activate P1 network gates`).
- Tracked modifications: `cmd/crystal-server/main.go`, `main_test.go`.
- Untracked: `cmd/crystal-server/connection_gates.go`,
  `connection_gates_test.go`. Staged files: none.
- SHA-256 fingerprints at freeze: main `76e2edd66cba9ea8385de81306d712779c1dd0ff40117181222ab289211a1fa1`;
  main test `2ab59ce4983003691046b478fbf324d661e3d6bc5dfe1d5916e193115fd2f22e`;
  gates `dbec58c73f3923f833fc0d75a19c300d699f348567265b67094e819b8fa92619`;
  gates test `b0288f10a7a9f444b1a8cc38f26540b331555c34422440554553078e7056b0d6`.
- Tracked, staged, and untracked C# gates were empty at recovery.

## Active leaf and protected work

- Active leaf: `NET-P1-GATES-001`.

- Candidate adds mutexed short/24-hour IP blocks, MaxIP admissions, MaxUser
  accept backpressure and release, receive-callback counting with an 8 KiB
  buffered reader, strict five-second reset, zero timeout disconnect, and
  invalid-frame/rate abuse blocks.
- Main review still must rule exact Legacy IP string authority, absolute idle
  timeout behavior across maintenance wakes, localized/log ordering, bootstrap
  failure/release, and all equality/shutdown boundaries before acceptance.
- Do not assume these uncommitted files are correct merely because focused tests
  currently pass.

## Verification ledger

Commands run in the Go root during recovery and exited 0:

- `git diff --check`.
- `go test ./cmd/crystal-server -run '^$'`.
- Focused NET regex covering gate/window/frame-reader/admission/abuse/zero-timeout
  tests at `-count=1 -timeout=2m`.

The first mixed-root status call and a later guessed nonexistent protocol path
were discarded in full and are not evidence. No repeated, focused-race,
integration, vet, build, or final C# gate has yet been run for this candidate.

## Active agents and processes

- Read-only `luna_worker` `01a03055-b94f-7ce0-b9b9-6e2a39c4ade9` reviews the
  exact Legacy/Go NET contract and may not write any file.
- Exact `comm` process audit found no `go` or `crystal-server` process at freeze.

## Exact recovery sequence

1. Verify this control delta, run `tasks/check-migration-control.sh`, all Legacy
   C# gates, and commit only the three owned control files.
2. Recheck the four Go fingerprints/status and collect/close the read-only Luna
   review without interrupting it.
3. Resolve findings in the four-file authority, then run compile, deterministic
   focused/repeated/race tests and the due standard Leaf/integration gates.
4. Mark matrix/index Complete only after evidence, refresh this snapshot, run
   both repositories' six C# gates, and commit owned files separately by repo.

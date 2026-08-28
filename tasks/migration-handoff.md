# Crystal Go migration current handoff

Last updated: 2026-08-28 13:11 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `NPC-P7-SPEECH-INPUT-001` is Complete in Go commit
  `826842e7cd63caa356c3efd86b646bae79ee56d0`. The unique next Active leaf is
  dependency-ready `NPC-P7-CONTROL-FLOW-001`.
- Exact next matrix scope is row 183, routing ledger E row 206 and P7 summary
  row 966 only. P7 remains frozen at 24 children: 8 Complete, 1 Active and 15
  Ready.
- The real compaction recovery is complete. No agent or Go process remains;
  terminal reviewer `01a04696-568d-7ca0-bbd6-17e21e9376cd` returned `No
  findings` after inspecting the final candidate including index-zero caches.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `3e679049b3411d409a9af3f2939d8c25f766eb5c`;
  upstream `origin/master`, ahead 499 and behind 0.
- Exactly four tracked unstaged migration-control paths differ:
  `tasks/lessons.md`, `tasks/lessons-archive/migration/protocol-session-wire.md`,
  `tasks/migration-active.md`, and this handoff. There are no staged or
  untracked paths.
- The active index routes Speech/Input Complete to Control Flow Active. Lessons
  record the missing-import compile closure, compact-time tool check and the
  feature-specific parser-placeholder/no-page fixture correction.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `826842e7cd63caa356c3efd86b646bae79ee56d0`;
  no upstream. Commit `826842e Complete P7 NPC speech and input` contains exactly
  the six owned server code/test paths and `docs/migration-matrix.md`.
- Index and worktree are clean; there are no staged or untracked paths. The
  commit records 2,041 insertions and 24 deletions across seven paths.
- Matrix row 182 is Complete, row 183 is the sole Active row, ledger D contains
  accepted evidence, and P7 summary is exactly 8+1+15.

## Completed Speech/Input behavior

- SAY formatting preserves space-token/global-replace and greedy regex quirks,
  CurrentCulture token/link casing including Turkish dotted-I behavior, unknown
  and empty fallback, malformed conquest/OUTPUT panic paths, culture-specific
  date/general-number rendering and all finite scalar/function/equipment/guild/
  conquest/roll placeholder branches.
- Production snapshots use the Legacy UTC Envir clock and live-only first-map
  monster count. Equipment FriendlyName/count, mail awaiting collection, map,
  user, HP/MP, balances, guild rental and definition-catalog sources are wired.
- Link replacement preserves raw display names and source duplicates. ITEM
  sibling definitions, ITEM/MONSTER/NPC caches, observer-before-target forwarding
  and target response order match Legacy; linked NPC ClientInformation uses
  ObjectID zero/Icon=BigMapIcon and can resolve an unmaterialized definition.
- Input values are stored before authorization, ordinary resume ignores wire
  NPCID, default resume requires matching identity, rejected values remain
  sticky, successful/no-page/default-callback calls consume them, `%INPUTSTR`
  remains action-only, and stale input cannot bypass a later direct 31-unit key
  disconnect. Variable/roll writers and action-before-SAY scheduling remain the
  new Control Flow/Action owners, not a Speech residual.

## Verification ledger

- Owned gofmt and minimum compile passed:
  `go test ./cmd/crystal-server -run '^$' -count=1` exit 0.
- Focused final repeated gate passed:
  `go test ./cmd/crystal-server -run '^(TestNPCP7SpeechInput|TestSessionNPCConfirmInputResumesAtInputPage|TestSessionNPCDefaultRequiresActiveObjectAndMatchingInputID)' -count=10` exit 0.
- Focused final race passed with the same regex and `-count=3`, exit 0.
- `go test ./cmd/crystal-server -count=1` exit 0 (80.744s).
- Final fresh integration `go test -count=1 ./...`, `go vet ./...`, and
  `go build ./...` all exit 0; final server package time was 83.912s.
- First due `go test -race ./...` exited 1 only at archived
  `TestSessionOmaMageRangeSlowFrozenTranscript` (`[2 1]` vs `[1]`) with no race
  detector report and no owned stack. Exact isolated race `-count=10` and the
  explicit-skip remainder both exited 0. A subsequent fresh unexcluded
  `go test -race ./...` exited 0; server package time was 95.131s.
- Legacy auditor `01a0467c-f8b6-7a42-bd05-9eb08f953aad` supplied exact source
  ordering; Go auditor `01a0467d-2917-7a80-b502-8579a0764332` found sticky key
  bypass/Turkish risks and verified the interrupted candidate; terminal review
  returned `No findings`. All three agents are closed.
- Initial post-catalog compile failed on a missing `worlddata` import and the
  first no-page test mistook a parser placeholder for an absent page; both are
  preserved as failed evidence, corrected, and all final gates above pass.

## Active leaf and protected work

- Active leaf: `NPC-P7-CONTROL-FLOW-001`.
- Next Go authority is bounded flow data in `internal/worlddata/npcscript.go`,
  `cmd/crystal-server/{main.go,default_npc.go}`, new `npc_script_flow.go`,
  `npc_script_flow_test.go`, bounded `npc_control_flow_session_test.go`, required
  existing session fixtures, and matrix/index/handoff.
- Complete loading, page grammar, Speech/Input formatter/metadata/input lifetime,
  action/condition catalogs, callbacks, protocol layouts and every C# are
  protected.

## Exact recovery sequence

1. Run the control checker and both-repository diff/status/C# gates, then commit
   exactly the four Legacy control/lesson paths. This handoff observes Legacy
   HEAD `3e679049b3411d409a9af3f2939d8c25f766eb5c` immediately before that one
   expected documentation commit delta.
2. Verify both worktrees clean and exact row 182 Complete / row 183 Active / P7
   8+1+15. Read only matrix row 183, ledger E row 206 and summary row 966.
3. Search archives only for Control Flow, delayed queue, action-before-response,
   GOTO/CALL/BREAK/DELAYGOTO/GROUPGOTO/ROLLDIE/ROLLYUT keywords and freeze the
   seven-key Legacy checklist before any Control Flow implementation.

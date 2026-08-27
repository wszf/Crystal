# Crystal Go migration current handoff

Last updated: 2026-08-28 04:11 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; neither Complete
  nor Blocked. Main is `gpt-5.6-sol/ultra`; bounded workers default to
  `luna_worker` (`gpt-5.6-luna/max`).
- `CHAT-P4-MAP-CONTEXT-001` is implemented, terminally reviewed and freshly
  gated. Go matrix and Active Index mark it Complete. P4 remains scope-frozen
  and In progress with nine of ten children Complete; collision rules remain
  dependency-blocked on P9.
- No dependency-ready functional child remains in frozen P1-P6. The one Active
  leaf is now bounded discovery `DISC-P7-CLOSURE`; it must inventory a finite P7
  denominator without implementation.
- Recovery matrix scope after commits is only registered P7 findings rows
  152-157 and P7 summary row 902. Do not read the full matrix, reopen CHAT/P5,
  or inventory another phase.
- Go CHAT closure is committed at
  `ccab4aa5675392ed4ca013f3798a54809327d0cb`; Legacy lessons/control routing
  remains uncommitted. Run the control checker and all C# gates before commit.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `f5a9476beaeea29bdc00d3ac9c5fa02857ae4916`
  (`f5a9476b Document ordinary spawn closure and route chat context`); upstream
  `origin/master`, ahead 492 and behind 0 at observation.
- Index is empty. Tracked worktree modifications are exactly:
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- There are no untracked files. Preserve every listed modification; do not
  reset, stash, clean, delete or overwrite it.
- The control checker exits 0. Tracked, staged and untracked `.cs` checks are
  empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; observed HEAD
  `ccab4aa5675392ed4ca013f3798a54809327d0cb`
  (`ccab4aa Complete map-context chat convergence`); no upstream.
- Index and worktree are clean. The commit owns exactly:
  - `cmd/crystal-server/chat_items.go`
  - `cmd/crystal-server/chat_items_test.go`
  - `cmd/crystal-server/main.go`
  - `cmd/crystal-server/observer.go`
  - `cmd/crystal-server/social_session.go`
  - `cmd/crystal-server/world.go`
  - `docs/migration-matrix.md`
  - `internal/legacyworld/database.go`
  - `internal/legacyworld/export_test.go`
  - `internal/legacyworld/map_schema_test.go`
  - `internal/worlddata/world.go`
  - `internal/worlddata/world_test.go`
- `cmd/crystal-server/chat_map_context_session_test.go` is now tracked in that
  commit. No `go` or `crystal-server` process remains.
- Tracked, staged and untracked `.cs` checks are empty.

## Active leaf and protected work

- Active leaf: `DISC-P7-CLOSURE`

- CHAT now imports/projects `NoNames`, masks normal map-local names, preserves
  literal-space/cross-map whispers, and consumes ordinary/map/server shouts with
  exact level, cooldown, source precedence, transient reset and insertion order.
- Linked items preserve class/level sibling definitions, complete metadata
  prelude before chat, observer-before-target ItemInfo caching, target cache
  defects for late observers, and observable metadata/item packets without
  observable whisper text. `@MAP` retains active Title/FileName and observer
  forwarding through existing production utility authority.
- Terminal reviewer initially reported four findings. Main ruled three as exact
  Legacy defects and the fourth as a layered import/JSON/main/runtime seam; new
  linked-whisper and leading-space tests were added. Follow-up reviewer
  `01a044ca-e4d3-73b3-96f2-0b6d44e49011` returned `No findings`.
- Two Luna reviewers (`01a044b7-3f58-7650-82c6-db98feeedf9e` and
  `01a044c6-a40b-7023-a78a-8cfbcbf8d4d2`) produced no report and were closed
  without writes. One `codex-auto-review/max` spawn was rejected by model effort
  validation; the explicit xhigh retry above is the accepted review evidence.
- `DISC-P7-CLOSURE` may write only matrix, Active Index and handoff. Existing P7
  production/tests and all Complete phases are read-only classification inputs;
  every C# file remains read-only.

## Verification ledger

All final commands ran in the Go root against the current post-review candidate.

- Owned-file gofmt and `git diff --check` — exit 0.
- Minimum compile `go test ./cmd/crystal-server -run '^$' -count=1` — exit 0.
- Final map-import/worlddata/CHAT/item/utility/protected ItemShout focused command
  at count 10 — all three package invocations exit 0.
- Final server focused race command at count 5 — exit 0.
- `go test ./cmd/crystal-server -count=1 -timeout=900s` — exit 0 in 80.885s
  before the final test-only reviewer regressions; the later fresh full run below
  supersedes it.
- Fresh `go test -count=1 ./...` — exit 0; server 81.684s.
- Fresh `go vet ./...` — exit 0.
- Fresh `go build ./...` — exit 0.
- Fresh `go test -race -count=1 ./...` — exit 0; server 96.256s.
- Earlier failures remain attributed in lessons/handoff history: unused config
  import; one-shot regen hook; empty-list assertion; unsupported formatter type;
  first full NoNames fixture expectation. Every corrected current command passes.
- The first intended protected filter used anchored `TestItemShout.*`, matched no
  tests and is not evidence. Exact test declarations were enumerated; corrected
  `Test.*ItemShout.*` count-10/race-count-5 commands pass.

## Exact recovery sequence

1. Run `tasks/check-migration-control.sh`; separately reverify both repositories'
   exact branch/HEAD/status and all tracked/staged/untracked `.cs` gates.
2. Stage only the six listed Legacy files, review the staged list, and commit the
   CHAT lessons/control routing set. Read the exact Legacy HEAD back from Git.
3. Resume the same Goal at `DISC-P7-CLOSURE`: read only matrix rows 152-157 and
   902, search the lessons archive with `DISC-P7-CLOSURE`, NPC access, script,
   shop, quest, page, action, condition and input keywords, then run bounded
   Legacy/Go classification. Do not implement functional P7 code in discovery.
4. Produce a finite child registry with status, dependency, physical Go
   production/test authority and additional gate; obtain independent read-only
   denominator review before marking discovery Complete and selecting one
   dependency-ready P7 child.

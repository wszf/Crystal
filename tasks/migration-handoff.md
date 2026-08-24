# Crystal Go migration current handoff

Last updated: 2026-08-24 20:05 (Asia/Singapore)

This replace-in-place snapshot closes `ADMIN-P3-ACCOUNT-OPS-001` and routes the
same persistent Goal to `ADMIN-P3-AUTHORITY-001`. The Go leaf is committed and
clean. Legacy control/lesson updates remain uncommitted until this snapshot is
verified and committed. Preserve all listed work; never reset, stash, clean,
delete, move, or overwrite it.

## Goal and control-plane state

- Goal remains Active: migrate all reachable, client-observable Legacy behavior
  to the independent pure-Go repository. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers remain
  `luna_worker` (`gpt-5.6-luna/max`). No subagent is active.
- P1 remains scope-frozen/In progress: nine Complete and three unfinished. P2
  remains scope-frozen/In progress: six Complete and two Ready. P3 remains
  scope-frozen/In progress: six Complete, `ADMIN-P3-AUTHORITY-001` Active, and
  four Ready.
- Go matrix and Legacy active index agree: account ops is Complete; admin
  authority is the unique Active leaf; P3 remains In progress.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; HEAD before the pending control commit:
  `339fa518393895e3d6d4cde9ec0883b4efc2452b`.
- Tracked unstaged files are exactly:
  - `tasks/lessons-archive/migration/protocol-session-wire.md`
  - `tasks/lessons-archive/workflow/shell-tools-and-patching.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Index and untracked set are empty. `git diff --check` and
  `tasks/check-migration-control.sh` exit 0.
- Separate tracked, staged, and untracked C# checks exit 0 with empty output; no
  repository lock exists.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `2e6df8aedb921935bc79d3e48518f47ee707b89e`
  (`migrate operator account controls`).
- Worktree and index are clean. `git diff --check` exits 0. Separate tracked,
  staged, and untracked C# checks exit 0 with empty output. No repository lock,
  `go`, or `crystal-server` process remains.

## Active leaf and protected work

- Active leaf: `ADMIN-P3-AUTHORITY-001`.
- Outcome: preserve `[General] GMPassword`, unrestricted case-insensitive
  `@LOGIN`, one-shot next nonempty chat capture, exact case-sensitive password
  match, localized Hint/System and server-log order, transient `IsGM`,
  GameMaster Buff/ranking removal, logout/relogin reset, and operator
  AdminAccount grant/revoke effects on current versus future sessions.
- Read only the named P3 finite-inventory row, exact P3 stage row, and evidence
  anchors named by that row. Do not read the full matrix.
- Legacy read authority is bounded to Settings GMPassword load/save,
  `PlayerObject.Chat` `@LOGIN`/GMLogin, its Buff/rank/log consumers, and the
  AccountInfoForm AdminAccount handler. Every C# file remains read-only.
- Tentative Go ownership is bounded to the exact config/auth/game-master/
  ranking/runtime-logging/session paths listed in `migration-active.md`. Source
  tracing must refine the final existing/new file list before any Go code write.
- Forbidden: completed account CRUD/ban/password/storage behavior, character
  delete/ban/start/logout, ranking-core redesign, player combat/map/item/flag
  actions, broad P12 recovery, native C# UI, broad matrix/source dumps, and
  every C# write.

## Completed account-ops evidence

- Committed Go production has auth-owned targeted mutations/order/object
  authority, transport-neutral operator control, production publication and
  persistence, stable opaque session identity across rename/remove/reused ID,
  claim-owned live effects, stale global CharacterList projection after direct
  removal, and unified FIFO connection-write tickets with shutdown retirement.
- It preserves blank 24-byte salts, raw zero account dates, unspecified-kind
  `DateTime.MaxValue`, physical account order and first-record 117 retention,
  direct JSON/117 reload, exact messages, reason 6, storage reset, no invented
  password/removal disconnect, and stopped wipe/no-cleanup quirks.
- Read-only reviewer `01a03392-8cbd-7050-9fb2-ba06d3a3b28a` found auction
  ownership after rename, queue-wait timeout, and duplicate zero-identity blank
  ref gaps. Main fixed all three; the same reviewer re-read the exact fixes and
  reported no residual P0-P2 finding. The reviewer made no writes or test runs
  and is closed.

## Verification ledger

All commands below ran in the Go root and exited 0 on the final candidate:

- Owned `gofmt`; touched compile:
  `go test ./internal/auth ./internal/operator ./internal/legacyaccount ./internal/protocol ./cmd/crystal-server -run '^$' -count=1`.
- Focused auth/controller/production account-operator/protocol tests at
  `-count=10`; the matching focused race tests at `-count=3`.
- `go test ./... -count=1 -timeout=20m`: fresh unexcluded pass; final
  `cmd/crystal-server` package time 74.631s.
- `go test -race ./... -count=1 -timeout=30m`: fresh unexcluded pass; final
  `cmd/crystal-server` package time 81.254s.
- `go vet ./...` and `go build ./...`.
- Final Go `gofmt -l` was empty; `git diff --check`, exact staged review, process
  audit, and all three Go C# gates passed before commit. Post-commit Go HEAD is
  clean and all three C# gates remain empty.
- Legacy control checker/diff check and all three Legacy C# gates passed after
  routing the next Active leaf. Repeat them after this handoff replacement and
  before the Legacy control commit.

## Exact recovery sequence

1. In the Legacy root, read this handoff and `migration-active.md`; verify HEAD,
   exact status, control checker, diff check, and three C# gates. Commit the five
   owned control/lesson files if they still match this snapshot.
2. In a separate Go-root command verify clean HEAD
   `2e6df8aedb921935bc79d3e48518f47ee707b89e`, diff/C# gates, and no process.
3. Read only the `ADMIN-P3-AUTHORITY-001` matrix row/P3 stage row. Trace the
   bounded Legacy and Go declarations, then refine exact Go write authority in
   the active index before any code write.
4. Implement the smallest coherent admin-authority slice and run its registered
   compile, focused/repeated/race and due integration gates.

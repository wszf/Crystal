# Crystal Go migration current handoff

Last updated: 2026-08-25 10:12 (Asia/Singapore)

This replace-in-place snapshot records committed `MAP-P4-DETAIL-001` closure and
synchronized routing to `MAP-P4-LIGHT-001`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`.
- The completed MAP-detail writer and reviewer are closed; no active subagent or
  Go/server process remains.
- Matrix and Active Index close MAP detail, activate MAP light, and record P4 as
  scope-frozen In progress with ten children, five Complete/five unfinished.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed HEAD before the expected control commit:
  `35c026d5531c214e5e21c73e861c10af29e1b74a`.
- Tracked unstaged: `tasks/lessons.md`,
  `tasks/lessons-archive/verification/fixtures-and-transcripts-01.md`,
  `tasks/migration-active.md`, and this refreshed handoff. Index and untracked
  set are empty.
- Lessons record the startup single-repository violation, truncated broad/glob
  calls, missing-import compile catch, fatal closed-pipe test race and reviewer
  suppression finding. Active Index routes exact three-file MAP-light authority.
- Control checker and diff check pass; all three Legacy C# gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD:
  `ea3a4503f5c6f506888898e012e62a3fa91ac767`.
- Worktree and index are clean.
- The committed leaf preserves full current/detail/setup payloads, loaded-source/current-
  culture search, NPC suffix GameName, sentinels, persistent suppression,
  failed-known silence and unknown-index fatal. Strict parsers and the production
  probe consume the exact sequence and reject repeated map detail.
- Go diff check and all three Go C# gates are empty.

## Active leaf and protected work

- Active leaf: `MAP-P4-LIGHT-001`.
- Outcome: exact static map lights plus UTC-derived global `TimeOfDay` bootstrap
  and transition broadcast only when light changes.
- Matrix anchors: exact MAP-light/P4 rows and named TimeOfDay/light/bootstrap
  evidence only.
- Exact Go authority after MAP-detail commit: `cmd/crystal-server/time_of_day.go`,
  `cmd/crystal-server/time_of_day_test.go`, and bounded `main.go`; matrix evidence
  only. Combat fire/lightning, detail/collision/visibility/chat and P5-P12 are
  forbidden.

## Verification ledger

Final MAP-detail evidence:

- `gofmt` on all nine owned Go files: exit 0;
- touched compile `go test ./cmd/crystal-server ./internal/protocol ./internal/probe -run '^$'`: exit 0;
- seven focused server tests, four exact protocol vectors and five probe/bootstrap
  tests at `-count=20`: exit 0;
- the same focused sets under `go test -race ... -count=5`: exit 0;
- first integration `go test ./... -count=1`: exit 1 only because the new fatal
  test treated `SetReadDeadline` closed-pipe as failure; root cause fixed and its
  isolation `-count=20` passed;
- fresh unexcluded final `go test ./... -count=1`: exit 0, server 77.403s;
- final `go vet ./...`, `go build ./...`, and `git diff --check`: exit 0;
- independent focused re-review: `resolved, no remaining finding`.

Full repository race is not due; focused race covers session/protocol/probe state,
and the due protocol integration gate is fresh and unexcluded.

## Exact recovery sequence

1. Verify the expected Legacy control commit, Go MAP-detail commit, clean Go
   worktree and both repositories' six C# gates.
2. Begin `MAP-P4-LIGHT-001` from its exact three-file authority and named anchors.

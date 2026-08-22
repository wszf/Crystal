# Crystal migration Goal task

This file is the stable execution contract for the Crystal migration Goal. Keep
the `/goal` prompt concise and point it here instead of duplicating the complete
runbook in every Goal or handoff.

## Outcome

Migrate all reachable, client-observable Legacy Crystal behavior from the
read-only Legacy repository into the independent Go repository:

- Legacy baseline: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Go repository: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`

The server, test clients, protocol probes, Legacy import/export, backup/restore,
deployment, and migration utilities must use Go. All `.cs` files in both
repositories remain read-only.

The overall Goal remains unfinished until the Go migration matrix P0-P12 is
fully Complete. Completing one batch, feature, protocol, or AI is not completion
of the overall Goal.

## Authoritative evidence

Resolve behavior conflicts in this order:

1. Current Legacy source and protocol serializers.
2. Reproducible Legacy runtime behavior and transcripts.
3. Legacy storage formats and load semantics.
4. Go production-entry behavior and test evidence.
5. Migration matrix, handoff, lessons, and historical documentation.

Trace real entry points, dynamic types, overrides, shared helpers, state
authority, random calls, timing boundaries, recipient visibility, wire payloads,
persistence, relogin, and restart behavior. Do not infer behavior from names or
similar modules.

## Agent responsibilities

The main agent uses `gpt-5.6-sol` with `high` reasoning and owns:

- matrix and batch selection;
- Legacy behavior rulings and architecture decisions;
- shared-state and cross-module integration;
- subagent task boundaries and review;
- final gates, documentation, handoff, commits, and Goal completion decisions.

Prefer the custom `luna_worker` agent, configured as `gpt-5.6-luna` with `max`
reasoning, for every bounded and independently verifiable task, including:

- direct Go implementation within assigned files or modules;
- tests, fixtures, protocol vectors, and transcript coverage;
- Legacy/Go call-chain comparison and large read-only searches;
- mechanical migration, repetitive edits, and data preparation;
- compilation, targeted/repeated/race checks, log triage, and independent review.

Every delegation must define the goal, repository, read/write scope, forbidden
scope, evidence source, acceptance criteria, required checks, and return format.
Writing agents must have disjoint file and authority scopes. Do not duplicate a
delegated task in the main thread. Review and integrate the returned patch; close
the agent promptly after collecting its concise evidence summary.

`luna_worker` must not redefine the Goal, architecture, priorities, or acceptance
criteria; expand scope; modify unassigned files; perform destructive Git actions;
or commit unless commit ownership is explicitly delegated. If it is unavailable,
do not silently substitute another model.

## Batch workflow

Use one independently testable, atomic feature cluster per main session whenever
practical.

Before a batch:

1. Read `agents.md`, active `tasks/lessons.md`, this file, and
   `tasks/migration-handoff.md`.
2. Check branch, HEAD, working tree, staged/untracked files, and `.cs` status in
   each repository separately.
3. Read the authoritative Go `docs/migration-matrix.md` and select a dependency-
   ready unfinished item.
4. Search the lessons archive only with relevant AI IDs, entity/module names, and
   workflow/failure keywords; never read the archive wholesale.
5. Preserve and integrate existing work. Never reset, stash, checkout, clean, or
   overwrite unrelated changes.

During a batch:

1. Derive a Legacy behavior checklist before implementation.
2. Delegate bounded implementation, tests, comparison, and high-volume support
   work to `luna_worker` with disjoint scopes.
3. Keep architecture, shared-state decisions, integration, and final rulings in
   the main agent.
4. Drive tests through production entry points and verify domain state,
   persistence, and complete observable protocol effects.
5. Treat Go map-stored entities as values and preserve explicit writeback,
   authority, revision, lock, persistence, and notification ordering.

Before ending or committing a batch:

1. Run `gofmt`, a minimum compile gate, targeted tests, relevant repeated/race
   tests, `go test ./...`, `go vet ./...`, and `go build ./...`.
2. Attribute any failure from its actual test name, exit code, and stack; do not
   change unrelated modules to hide an existing failure.
3. Run `git diff --check`.
4. Update the Go migration matrix and `tasks/migration-handoff.md` with exact
   evidence and the next recovery point.
5. In both repositories, verify tracked, staged, and untracked `.cs` changes are
   empty.
6. Commit only files owned by the batch, using atomic commits per repository.
7. Close completed subagents.

## Session rollover

The Goal does not automatically change sessions. The handoff and matrix, not one
large conversation, preserve migration state.

Prefer a new main session after every completed atomic batch. Stop expanding
scope and prepare a safe handoff when any of these occurs:

- about 5 million cumulative session tokens;
- a rollout approaching 250 MB;
- two context compactions;
- noticeable tool, response, or migration slowdown;
- insufficient remaining context to close the current batch safely.

Treat about 8 million tokens or a 500 MB rollout as a hard ceiling. Before the
ceiling, finish the smallest safe boundary, record exact repository state and
next steps, commit only valid owned work, close subagents, and ask the user to
open a new session. A rollover is never a reason to mark the Goal Complete.

### Context-compaction safety

Compaction is a hard handoff boundary, not a point at which the model may defer
bookkeeping. **Before every context compaction**, and as soon as the system
signals imminent compaction, a context-limit rollover, or a new-session
boundary, stop implementation and write or refresh `tasks/migration-handoff.md`.
If the current batch cannot be closed safely, stop code changes and tests first
and record the smallest complete handoff. It must include both repository roots,
branches and HEADs, complete tracked/staged/untracked status, exact files owned
by the batch, tests and exit codes, failure attribution, uncommitted changes,
the current matrix row, and the next recovery command.

Read the handoff back and verify it against both worktrees before compaction.
The handoff is the durable migration record. An automatically generated compacted
summary is untrusted context and must not replace or override the handoff. After
compaction, continue the same active Goal from the verified handoff; do not
reopen or recreate the Goal, and do not call Goal creation/reset merely to
recover from compaction or a new session. If no verified handoff exists, stop
implementation and reconstruct one from both repositories before proceeding.

## Definition of done

Mark the overall Goal Complete only when all of the following are true:

1. P0-P12 in Go `docs/migration-matrix.md` are Complete with no unverified
   Pending, In progress, TODO, or residual migration gaps.
2. Protocol ordinals, payloads, ordering, recipients, domain state, randomness,
   timing, lifecycle, and error behavior are equivalent.
3. Authenticated sessions, persistence, logout/relogin, restart, import/export,
   backup/restore, deployment, and cross-platform operation are verified.
4. `gofmt`, full tests, full race, `go vet ./...`, and `go build ./...` pass.
5. Both repositories have no tracked, staged, or untracked `.cs` changes.
6. Matrix, handoff, test evidence, and Git history prove the migration is closed.

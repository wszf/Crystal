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

## Control plane and bounded context

Use these files for different purposes; do not duplicate their contents:

- `tasks/goal-task.md`: stable execution and completion contract.
- `tasks/migration-active.md`: concise routing index, current leaf batch, scope-
  freeze state, and the exact Go matrix anchors to inspect.
- `tasks/migration-handoff.md`: current recoverable snapshot only.
- Go `docs/migration-matrix.md`: authoritative detailed evidence and status.
- `tasks/migration-handoff-archive/`: historical snapshots; never a startup-read
  source.

Keep `agents.md` at or below 200 lines/16 KiB and this stable contract at or
below 300 lines/32 KiB. These limits are enforced with the other control-plane
limits by `tasks/check-migration-control.sh`.

At the start of a new main migration session, read `agents.md`, active
`tasks/lessons.md`, this file, `tasks/migration-active.md`, and the current
handoff once. Verify each repository separately. In the Go repository, read only
the matrix rows or anchored sections named by `tasks/migration-active.md`.
Read the full matrix only once for an explicit phase-closure audit, or when a
proven index/matrix inconsistency requires reconstruction. A missing next item
must activate a bounded `DISC-Px-CLOSURE` leaf and search only that phase's
headings/rows; it is not permission for a full-matrix read. Never `cat` the full
matrix, handoff archive, or broad source trees into the main thread.

After an in-session context compaction, do not repeat the full startup sequence.
Read the concise active index and current handoff, verify their stated worktree
facts, then resume the same batch. Re-read another control file or matrix section
only when the compacted context lacks a required rule or evidence.

## Finite inventory and progress

Every implementation batch must have a stable leaf ID in
`tasks/migration-active.md` before code changes begin. A leaf records:

- phase and matrix anchor;
- observable outcome and authoritative Legacy entry points;
- owned files and dependency boundary;
- acceptance evidence and required test tier;
- status: `Discovery`, `Ready`, `Active`, `Blocked-external`, or `Complete`.

Vague matrix residuals such as “remaining”, “other”, or “full closure pending”
are not measurable work. Each such residual requires a discovery/closure leaf
that enumerates finite child leaves before the phase can be marked scope-frozen.
Do not publish a completion percentage or ETA for an unfrozen phase. Stage
statuses are routing summaries, not percentages.

Only one primary/integration leaf batch may be `Active` as the main routing
target. That leaf may contain multiple bounded workstreams running in parallel
when their write and authority scopes are disjoint. A workstream must declare its
ID, leaf, role, files, authority lock, dependencies, forbidden scope, and evidence;
Agent identity and a fixed Agent count are not Goal fields. Coupled writes remain
serialized or isolated. Update the active index when selection, ownership,
status, workstream scope, or the next recovery point changes; do not append
historical batch narratives to it.

If a frozen phase has no dependency-ready implementation leaf because it is
blocked on a separately owned phase discovery, a bounded read-only `DISC-Px-CLOSURE`
workstream may run in parallel. It may enumerate finite child leaves only; it is
not permission to implement an open residual.

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

The main migration agent owns:

- matrix and leaf selection;
- Legacy behavior rulings and architecture decisions;
- shared-state and cross-module integration;
- workstream boundaries and review;
- integration gates, documentation, handoff, commits, and Goal completion.

Bounded workers may handle independently verifiable implementation, tests,
Legacy/Go tracing, mechanical migration, focused verification, and review. The
operational model preference and any explicit fallback belong in `agents.md`,
not in this stable Goal contract. The Goal does not declare specific Agent
identities or a fixed Agent count.
Ultracode chooses the number and shape of bounded workstreams from dependency,
resource, and conflict analysis, subject to platform limits. Run independent
workstreams in parallel when their file and authority scopes are disjoint;
serialize or isolate coupled writes. Do not spawn agents merely to reread
control documents, repeat status, or produce a second copy of assigned work.
Close each completed thread after collecting its concise evidence summary and
patch.

Every delegation must define the workstream ID, leaf ID, repository,
read/write scope, authority lock, forbidden scope, evidence source, acceptance
criteria, required checks, and return format. `luna_worker` must not redefine
the Goal, architecture, priorities, or acceptance criteria; expand scope;
modify unassigned files; perform destructive Git actions; or commit unless
commit ownership is explicit. If the preferred worker is unavailable, an
explicitly recorded equivalent may handle read-only, mechanical, or
verification work; production writes, architecture, and final rulings require
main-agent review and the handoff must record the actual model and extra checks.

## Work-cycle workflow

A work cycle completes one independently testable leaf or tightly coupled leaf
cluster. A cycle is not required to equal one chat, turn, or context window.

Before a cycle:

1. Verify the concise handoff and active index against each repository.
2. Preserve existing tracked, staged, and untracked work; never reset, stash,
   checkout, clean, delete, move, or overwrite unrelated changes.
3. Read only the active matrix anchors and relevant Legacy/Go sources.
4. Search the lessons archive with the leaf ID, entity/module, and concrete
   workflow/failure keywords; never read it wholesale.
5. Derive a finite Legacy behavior checklist and register missing child leaves
   before implementation.

During a cycle:

1. Register one bounded workstream wave with disjoint ownership; Ultracode may
   fan out independent tracing, fixture, review, and verification workstreams.
2. Keep architecture, shared-state decisions, integration, and final rulings in
   the main agent; join workstream results before any coupled write.
3. Drive tests through production entry points and verify domain state,
   persistence, recipients, wire order, relogin, and restart when applicable.
4. Treat Go map-stored entities as values and preserve explicit writeback,
   authority, revision, lock, persistence, and notification ordering.
5. Keep main-thread command output targeted. Locate with `rg --files`/`rg`, then
   read only the necessary ranges; broad source dumps and raw test logs belong
   in subagent threads or bounded log files, with concise summaries returned.

Before committing a leaf, run the leaf gate below, update the matrix and active
index when their state or evidence changes, refresh the current handoff when
its recovery point changes, close completed workstreams, and commit only owned
files. A completed leaf does not complete the overall Goal.

## Tiered verification

### Leaf gate — every functional leaf commit

- `gofmt` only the owned changed Go files.
- Minimum compile for touched packages (`go test <packages> -run '^$'`).
- Focused production-entry tests, relevant repeated tests, and relevant focused
  race tests.
- `git diff --check` and exact tracked/staged/untracked status review.
- Tracked, staged, and untracked `.cs` gates in both repositories.

### Integration gate — bounded cadence

Run `go test ./...`, `go vet ./...`, and `go build ./...`:

- before changing a phase status or shared architecture;
- after at most four leaf commits or 24 hours since the last integration gate,
  whichever comes first;
- immediately when a leaf changes shared protocol, persistence, scheduler,
  locking, or startup/shutdown infrastructure.

Independent integration commands may run in parallel when their tools, ports,
 caches, and generated outputs are isolated; record each command and exit code
separately. Serialize or isolate commands that share a test database, process,
port, cache, or generated file. Run full `go test -race ./...` before phase
closure, after at most eight leaf commits or 48 hours, and immediately for
changes with broad concurrency impact. Focused race remains mandatory for every
concurrency-relevant leaf.

Record actual command, exit code, failing test, and stack attribution. Do not
rerun known unrelated failures for every tiny leaf merely to reproduce the same
log, but never omit a due integration/full-race gate or describe an excluded run
as a full pass.

### Overall closure gate

The final Goal still requires fresh, unexcluded full tests, full race, vet,
build, protocol/import/export/restart/deployment evidence, and all remaining
definition-of-done checks.

## Current handoff rules

`tasks/migration-handoff.md` is a replace-in-place current snapshot, not an
append-only journal. Keep it at or below 250 lines and 24 KiB. It must contain:

- both repository roots, branches, observed HEADs, and complete tracked/staged/
  untracked status;
- active leaf ID, exact owned files, matrix anchors, and unresolved decisions;
- tests with command, exit code, and failure attribution;
- active subagents/processes or explicit quiescence evidence;
- the smallest exact recovery sequence.

Do not append successive compact boundaries or repeat completed milestone
history. Git history is the normal archive. Before replacing a snapshot that
contains unique uncommitted evidence not already preserved by Git, copy it once
to `tasks/migration-handoff-archive/`; startup and compact recovery must never
read that archive wholesale.

Run `tasks/check-migration-control.sh` after changing the control plane. A
handoff commit may record the exact HEAD observed immediately before its own
commit and identify that one expected documentation commit delta; recovery must
always compare it with the actual `git rev-parse HEAD` and status.

## Session rollover and compaction

The Goal does not automatically change sessions. An active persistent Goal has
no repository-defined token, rollout-size, turn, or compaction-count ceiling.
Those metrics are observability only and must not trigger a final answer, new-
session request, pause, or `update_goal(status="blocked")`.

Only an actual compaction/context-limit/new-session signal triggers the safety
gate. Stop new writes/tests, quiesce writing agents, and verify both worktrees.
Refresh the current handoff only if state changed; otherwise verify the existing
snapshot instead of appending another section. After compaction, read the active
index and current handoff, then resume useful work in the same Goal. Use a fresh
main session only when the user explicitly requests one or the platform requires
it.

Only a genuine external/platform impasse that prevents all meaningful progress
for the required consecutive turns may block the Goal. Difficulty, duration,
unknown remaining scope, context size, compaction count, or preference for a new
chat are not blockers.

## Definition of done

Mark the overall Goal Complete only when all of the following are true:

1. P0-P12 in Go `docs/migration-matrix.md` are scope-frozen and Complete with no
   unverified Pending, In progress, Discovery, TODO, or residual migration gaps.
2. Every registered leaf is Complete with authoritative behavior evidence.
3. Protocol ordinals, payloads, ordering, recipients, domain state, randomness,
   timing, lifecycle, and error behavior are equivalent.
4. Authenticated sessions, persistence, logout/relogin, restart, import/export,
   backup/restore, deployment, and cross-platform operation are verified.
5. `gofmt`, fresh unexcluded full tests, full race, `go vet ./...`, and
   `go build ./...` pass.
6. Both repositories have no tracked, staged, or untracked `.cs` changes.
7. Matrix, active inventory, current handoff, test evidence, and Git history
   prove the migration is closed.

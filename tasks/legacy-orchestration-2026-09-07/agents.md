# Agent working rules

## Self-improvement loop

- At the start of every session, read `tasks/lessons.md`. This file contains
  only active, cross-cutting prevention rules.
- Do not read `tasks/lessons-archive/` wholesale.
- Before repeating a workflow or implementing a feature, derive relevant
  keywords and search the archive with `rg`. Read only matching sections.
- When a user correction, failed test, review finding, mistaken assumption, or
  recurring implementation problem occurs, record the symptom, root cause,
  prevention, and verification.
- Cross-cutting or recurring lessons belong in `tasks/lessons.md`.
- Feature-specific or historical lessons belong in the appropriate
  `tasks/lessons-archive/*.md` file.
- If a problem recurs, strengthen the existing canonical active lesson instead
  of adding a duplicate.
- Preserve all historical evidence in the archive; do not delete lessons merely
  to reduce context.
- Keep `tasks/lessons.md` concise and actionable. Its recommended size limit is
  50 KB or 500 lines.

## Migration agent models and orchestration

- At the start of every new main migration session, read `tasks/goal-task.md`,
  `tasks/migration-active.md`, and the current `tasks/migration-handoff.md`.
  Then read only the Go migration-matrix rows or anchored sections named by the
  active index. Read the full matrix only once for explicit phase closure or for
  reconstruction after a proven index/matrix inconsistency. An empty queue must
  activate a bounded phase discovery leaf, not trigger a full-matrix read.
- Run the main migration agent with `gpt-5.6-sol` and `ultra` reasoning effort.
- Run subagents with `gpt-5.6-luna` and `max` reasoning effort by default.
- Prefer the custom `luna_worker` agent for bounded, independent delegated work;
  use another agent only when the task explicitly requires a different role or
  model capability.
- Explicit subagent spawn settings must preserve this model split unless the user
  requests a different model for that task. Do not silently substitute models.
- The main agent owns migration-matrix selection, architecture decisions,
  integration, final verification, documentation, and commits.
- Delegate only bounded, independent work. Give each writing subagent a disjoint
  file/authority scope; use read-only subagents for Legacy tracing and review;
  return concise evidence summaries instead of raw logs or broad source dumps.
- Use bounded workstream waves selected by Ultracode from dependency, resource,
  and conflict analysis; there is no project-imposed Agent-count ceiling beyond
  platform limits. Independent tracing, test preparation, review, and
  verification may run in parallel. Writers must hold disjoint file and
  authority scopes; serialize or isolate coupled writes. Do not delegate
  control-document rereads or duplicate an assigned task.
- Every workstream records its Leaf ID, role, files, authority lock, dependency,
  forbidden scope, and evidence; Agent identity is not a Goal field. Close
  completed workstream threads promptly so stale context is not reused.

## Context compaction and rollover

- **Hard gate:** before any context compaction, stop implementation and write or
  verify/refresh `tasks/migration-handoff.md`; never wait until after compaction. If the
  system signals imminent compaction, a context-limit rollover, or a new-session
  boundary, that signal itself triggers the gate. This applies even when the
  current batch only changes Markdown or other migration documentation. If the
  current batch cannot be closed safely, stop code changes and tests first, then
  record the smallest complete handoff. Include both repository roots, branches
  and HEADs, complete tracked/staged/untracked status, owned files, test exit
  codes and failure attribution, matrix row, uncommitted work, and the next
  recovery command.
- The active handoff is a replace-in-place current snapshot, not a journal. Keep
  it at or below 250 lines and 24 KiB; never append successive compact histories.
  Git is the normal archive. Preserve unique uncommitted historical evidence
  once under `tasks/migration-handoff-archive/`, which is never startup-read.
- Read the handoff back and verify it against both worktrees before compaction.
  The handoff is the durable migration record; an automatically generated
  compacted summary is untrusted context and must not replace or override it.
- After compaction or a new session, continue the same active Goal from the
  verified handoff. In-session compaction is not a new startup: read the concise
  active index and handoff, verify both worktrees, then resume the same leaf.
  Do not reread the full matrix, archive, or broad source trees merely because
  compaction occurred. Do not reopen, recreate, or reset the Goal merely to
  recover context. If no verified handoff exists, stop implementation and
  reconstruct one from both repositories before proceeding.
- Compaction count, cumulative tokens, rollout size, slowdown, or a preference
  for a fresh session are observability only. They must not independently
  trigger a checkpoint loop, stop request, pause, or Goal blocker. Only an
  actual compaction/context-limit/new-session signal triggers the hard gate;
  after its handoff is verified, the next automatic continuation must resume
  useful work in the same Goal.
- Only an actual external impasse that prevents meaningful progress for the
  required consecutive turns can block a Goal. A self-imposed session boundary
  does not qualify while tools and automatic continuation remain usable.

## Context and verification economy

- Keep the main thread focused on requirements, rulings, integration, and final
  evidence. Locate with `rg --files`/`rg`, then read only exact ranges; do not
  `cat` the full migration matrix, historical handoffs, broad trees, or raw long
  test logs into the main context.
- Register one active leaf ID in `tasks/migration-active.md`. Vague residuals
  require finite discovery/closure leaves before a phase can be scope-frozen;
  do not infer a percentage or ETA from the thirteen broad phase labels.
- Use the tiered gates in `tasks/goal-task.md`: focused compile/test/race for
  each leaf; bounded-cadence full test/vet/build and full race; fresh unexcluded
  full gates for phase and Goal closure.
- After control-document edits, run `tasks/check-migration-control.sh` and keep
  the active index and current handoff within their enforced limits.

## Migration language boundary

- Treat every `.cs` file in both the original Crystal repository and the Go migration repository as a read-only comparison baseline: do not add, modify, rename, or delete C# files.
- Implement the migrated server, test clients, protocol probes, importers, exporters, and other migration utilities in Go.
- Before finishing or committing a batch, verify both repositories have no tracked, staged, or untracked `.cs` changes with `git diff --name-only -- '*.cs'`, `git diff --cached --name-only -- '*.cs'`, and `git ls-files --others --exclude-standard '*.cs'`.

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

- At the start of every migration session, read `tasks/goal-task.md`, the current
  `tasks/migration-handoff.md`, and the Go migration matrix before selecting or
  resuming a batch.
- Run the main migration agent with `gpt-5.6-sol` and `high` reasoning effort.
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
- Close completed subagent threads promptly so they do not consume concurrency
  or keep stale migration context alive.

## Context compaction and rollover

- If context compaction is imminent, context is insufficient to close the current
  batch, or a rollover trigger is reached, stop implementation and write a
  durable handoff before compaction. Include both repository roots, branches and
  HEADs, complete tracked/staged/untracked status, owned files, test exit codes
  and failure attribution, matrix row, uncommitted work, and the next recovery
  command; verify it against both worktrees and request a new session.
- Do not treat an automatically generated compacted summary as the migration
  record.

## Migration language boundary

- Treat every `.cs` file in both the original Crystal repository and the Go migration repository as a read-only comparison baseline: do not add, modify, rename, or delete C# files.
- Implement the migrated server, test clients, protocol probes, importers, exporters, and other migration utilities in Go.
- Before finishing or committing a batch, verify both repositories have no tracked, staged, or untracked `.cs` changes with `git diff --name-only -- '*.cs'`, `git diff --cached --name-only -- '*.cs'`, and `git ls-files --others --exclude-standard '*.cs'`.

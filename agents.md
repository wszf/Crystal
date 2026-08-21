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

## Migration language boundary

- Treat every `.cs` file in both the original Crystal repository and the Go migration repository as a read-only comparison baseline: do not add, modify, rename, or delete C# files.
- Implement the migrated server, test clients, protocol probes, importers, exporters, and other migration utilities in Go.
- Before finishing or committing a batch, verify both repositories have no tracked, staged, or untracked `.cs` changes with `git diff --name-only -- '*.cs'`, `git diff --cached --name-only -- '*.cs'`, and `git ls-files --others --exclude-standard '*.cs'`.

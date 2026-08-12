# Agent working rules

## Self-improvement loop

- At the start of every session, read [`tasks/lessons.md`](tasks/lessons.md) before modifying the project.
- When a user correction, failed test, review finding, mistaken assumption, or recurring implementation problem occurs, record it in `tasks/lessons.md` before finishing the turn.
- Each lesson must describe the symptom, root cause, preventive pattern, and how the prevention was verified.
- Prefer concise, actionable lessons. Do not record secrets, credentials, or unnecessary personal data.
- Before repeating a workflow that has caused a previous problem, search the lessons and apply the relevant prevention.
- If the same problem recurs, update the existing lesson with the new evidence and strengthen the prevention; iterate until recurrence drops.
- Keep lessons project-specific and treat current source files and test results as the authoritative evidence.

## Migration language boundary

- Treat every `.cs` file in both the original Crystal repository and the Go migration repository as a read-only comparison baseline: do not add, modify, rename, or delete C# files.
- Implement the migrated server, test clients, protocol probes, importers, exporters, and other migration utilities in Go.
- Before finishing or committing a batch, verify both repositories have neither tracked nor untracked `.cs` changes with `git diff --name-only -- '*.cs'` and `git ls-files --others --exclude-standard '*.cs'`.

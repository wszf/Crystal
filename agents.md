# Agent working rules

## Self-improvement loop

- At the start of every session, read [`tasks/lessons.md`](tasks/lessons.md) before modifying the project.
- When a user correction, failed test, review finding, mistaken assumption, or recurring implementation problem occurs, record it in `tasks/lessons.md` before finishing the turn.
- Each lesson must describe the symptom, root cause, preventive pattern, and how the prevention was verified.
- Prefer concise, actionable lessons. Do not record secrets, credentials, or unnecessary personal data.
- Before repeating a workflow that has caused a previous problem, search the lessons and apply the relevant prevention.
- If the same problem recurs, update the existing lesson with the new evidence and strengthen the prevention; iterate until recurrence drops.
- Keep lessons project-specific and treat current source files and test results as the authoritative evidence.

# Crystal → Go server migration

Legacy C# Crystal lives in this repo (read `.cs` as source of truth; do not edit server C# for migration work).
Go implementation lives in `/workspace/Crystal.GoServer`.

## Working rules
- Implement only in Go (`/workspace/Crystal.GoServer`).
- Legacy `.cs` is read-only reference.
- Prefer small focused commits with tests.
- Do not invent owners for blocked shared problems; state blockers plainly.
- Old Goal/leaf/matrix orchestration was archived under `tasks/legacy-orchestration-2026-09-07/` and Go `docs/legacy-orchestration-2026-09-07/` — treat as historical only, not active process.

## Current ask
Re-plan the overall remaining migration from scratch based on real code gaps, then execute the plan. See `tasks/REPLAN.md`.

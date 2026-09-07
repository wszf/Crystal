# Replan Crystal → Go migration

The previous Goal / leaf / lessons / migration-matrix process was abandoned as too heavy and ineffective. Historical files are archived; do not revive that process.

## Your job
1. Inspect Legacy (`/workspace/Crystal`, especially Server/) and Go (`/workspace/Crystal.GoServer`) as they exist now.
2. Note uncommitted in-progress Go work (WORLD NPC actions and related files) — keep useful progress, do not blindly discard.
3. Write a **short** new plan to `tasks/MIGRATION-PLAN.md` (and optionally a slim `docs/MIGRATION-STATUS.md` in the Go repo):
   - What is already done (evidence from code/tests)
   - What remains, ordered by dependency / user value
   - Concrete next 3–5 work packages with acceptance checks
   - Explicit non-goals / known blockers
4. Keep the plan short and executable (aim < 200 lines). No leaf-ID bureaucracy, no giant matrices.
5. After the plan is written and committed (author wszf <wszfer@gmail.com> via local repo config only), start executing the first work package.

## Model
This session is already gpt-6-astra medium via CLI. Do not /model switch. Do not use gpt-5.6-luna or gpt-5.6-sol workers.

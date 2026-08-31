# Crystal Go migration current handoff

Last updated: 2026-08-31 08:22 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- `DROP-P6-GROUND-LIFECYCLE-001` is Complete in Go `4265f77` plus delayed-review
  correction `9bffc2d`; this closes one P6 child only, not P6 or the persistent Goal.
- Primary dependency-ready leaf is now `CAPACITY-P6-GRIDS-001`, matrix row 3541
  and P6 summary row 965. P6 remains frozen at nineteen children: sixteen Complete,
  one Active and two Ready.
- Active workstream is `WS-CAPACITY-P6-INVENTORY-001`: migrate ordinary
  `@ADDINVENTORY` capacity growth, costs and capped repeated-charge behavior.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `d6c7d049` (`Close death ground migration leaf`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `9bffc2d` (`Correct player death drop parity`), not pushed.
- Worktree and index are clean after the correction/matrix commit.

## Completed Drop closure evidence

- Go `4265f77` materializes Legacy normal/red Equipment and Inventory death results
  through the accepted ground authority before Death/ObjectDied; Go `9bffc2d` closes
  every delayed-review correction without reopening the leaf.
- Normal/red RNG, PK `> 200`, player/non-player branches, safe-zone/`NoDropPlayer`,
  bind/wedding/sealed, BreakOnDeath and incomplete-Spirit no-continue quirks remain
  intact. Hero and ordinary pet attackers normalize to their owning Player.
- Whole drops preserve identity; partial stacks use global IDs and fresh defaults,
  including ordinary Equipment's zero-count object. Death eligibility, BreakOnDeath,
  global notification and projection use the base catalog definition.
- Auth snapshot commits may omit only Rental IDs actually removed by death. Selected
  Rental returns remain authoritative until the return transaction consumes them;
  shatter/delete events retain their original slot order, and normal Inventory keeps
  the duplicated Legacy return text.
- Death ground objects ignore `NoThrowItem`, have no owner/group pickup window and use
  configured `PlayerDiedItemTimeOut`. Setup.ini ordinary/death values preserve signed
  minutes and Legacy Int32 millisecond-overflow behavior; defaults remain 30/120.
- Ground placement keeps the dying Player blocking, ignores other dead players, blocks
  living Heroes, and uses runtime visibility for NPCs and specialized monsters.
  Monster item/gold producers separately pass dead-monster physical source and
  EXPOwner pickup authority, so a co-located dead owner does not displace loot.
- Staged objects remain invisible until auth commit; rejection rolls back objects and
  output. Authenticated FireBang death proves ObjectItem/DeleteItem before
  Death/ObjectDied, immediate ownerless pickup and final persisted picker state.

## Verification ledger

Passed for Go `9bffc2d` before commit:

- config/auth Rental-removal and timeout regressions at count 20;
- all server tests matching Death or Drop at count 20;
- focused Death/Drop race tests at count 5;
- dead-monster/dead-EXPOwner item and gold source regressions at count 20;
- full `go test ./... -count=1` with only the exact known Quest P7 fixture skipped;
- full `go test -race ./... -count=1` with the same exact fixture skipped;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- tracked/staged/untracked `.cs` audits in both repositories.

Integration attribution:

- The first final full-suite attempt returned exit 1 in `cmd/crystal-server`; its test
  name was truncated from captured output. An immediate isolated package rerun and a
  complete full-suite rerun both passed, followed by a passing full-race suite.
- Delayed review identified authoritative Rental deletion, Hero/pet attribution,
  base-definition, hidden blocker, monster physical-source and extreme timeout edges.
  Each accepted finding has production code plus a focused regression.
- Independent review of the final snapshot returned no remaining high-confidence
  correctness finding.

## Active leaf and protected work

- Active leaf: `CAPACITY-P6-GRIDS-001`.
- Active workstream: `WS-CAPACITY-P6-INVENTORY-001`.
- Trace only Legacy `@ADDINVENTORY` parsing, size/cost transitions and directly
  reached persistence/projection helpers; C# stays read-only.
- Reuse existing gold mutation, Inventory grid, auth item authority, bootstrap and
  checkpoint persistence. Do not create parallel balance or capacity state.
- Freeze 46-to-54 then four-slot growth through 86, every cost tier, low-gold paths,
  packet/text order and the capped repeated-charge quirk.
- Keep `@ADDSTORAGE` as the next bounded workstream; do not enter item-use, item-
  expiry, unrelated P10 economy, protocol layout or other phase behavior.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3541 plus the active index and this handoff.
3. Trace Legacy `@ADDINVENTORY` command admission, cost table, size mutation and
   response/persistence behavior; every `.cs` remains permanently read-only.
4. Freeze the smallest command/auth/session adapter around the existing gold and
   Inventory authorities; do not implement `@ADDSTORAGE` in the same workstream.
5. Verify every size/cost boundary, capped repeated charge, low-gold atomicity,
   authenticated packet order, relogin/restart, repeated and race behavior.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

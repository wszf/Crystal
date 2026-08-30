# Crystal Go migration current handoff

Last updated: 2026-08-31 01:15 (Asia/Singapore)

This is the replace-in-place current snapshot. The automatic compact summary is
not evidence; do not startup-read historical handoff archives.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- Primary Active leaf remains dependency-ready `EQUIP-P6-CORE-001`, matrix row
  3543 and P6 summary row 965. P6 remains scope-frozen at nineteen children:
  fourteen Complete, one Active and four Ready.
- `WS-EQUIP-HERO-SNAPSHOT-HARDEN-001` is complete in Go commit
  `7657f4c0555c7e146b3e9b0ee43ddf9a548600a6`.
- Next bounded workstream is `WS-EQUIP-HERO-EQUIP-REMOVE-001`: close the finite
  Hero EquipItem/RemoveItem gates and observable packet/stat order on top of the
  hardened authority. It must not reopen P8 Hero lifecycle semantics.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- Base HEAD verified before this evidence update:
  `43f12c20eb720470af0df0c80a61eefd7980e1fa`.
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or include it in migration evidence commits.
- Every Legacy `.cs` file remains permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `7657f4c0555c7e146b3e9b0ee43ddf9a548600a6`, ahead 7 of the configured upstream;
  it has not been pushed.
- Worktree and index were clean immediately after the feature commit.

## Active leaf and protected work

- Active leaf: `EQUIP-P6-CORE-001`.
- Protected inputs: accepted Grid Cross, Storage, Refine, P8 Hero/Mount lifecycle
  and P11 Fishing/Awakening/IntelligentCreature lifecycle authorities. Do not
  reopen them from this leaf.
- Completed workstream `WS-EQUIP-HERO-SNAPSHOT-HARDEN-001` establishes the
  authority prerequisite for Hero equipment; it does not close Hero Equip/Remove.
- Active workstream `WS-EQUIP-HERO-EQUIP-REMOVE-001` owns only finite Hero
  EquipItem/RemoveItem gates, mutations and client-visible ordering over existing
  authority. Forbidden scope includes protocol layouts, owner lifecycle changes,
  P11 business behavior and all C# writes.

### Completed snapshot-hardening evidence

- CrossGrid Hero commits are item-only. They reject changes to Hero count,
  selection, spawned state, behaviour, slot count and slot identity.
- Player item authority is merged field-by-field instead of replacing live
  session runtime. Equivalent snapshots advance revision/baseline watermarks
  without recalculating unrelated combat state; this protects the Plague path.
- Terminal and runtime item snapshots share baseline three-way merge semantics.
  Disjoint stale changes rebase; same-slot and runtime conflicts reject atomically,
  retain latest auth authority and suppress candidate side effects.
- SaveJSON exposes whether atomic replacement already committed. Explicit logout
  keeps the session online after a pre-rename failure and accepts a committed JSON
  write when only the later hook fails.
- Combat durability, mining payout, revival and mount conflict paths restore local
  speculative state, suppress stale packets and keep later notifications/sessions
  usable. Mount persistence I/O runs after the world mutation callback.
- IntelligentCreature combined item/runtime commits rebase disjoint changes and
  reject same-slot/runtime conflicts. Pickup, expiry, Black Stone, forbidden-map
  dismissal and client updates use operation-local rollback; ground visibility is
  restored and one conflict no longer aborts the whole notification batch.
- Terminal health reads the latest auth result rather than overwriting it with an
  uncommitted world-only candidate. This repaired the detected NoReincarnation
  logout HP regression while preserving production `PersistHealth` changes.

### Explicitly unfinished boundaries

Do not misreport these as closed by `7657f4c`:

- `CommitIntelligentCreatureRuntime` lacks an independent expected runtime
  revision/CAS; a stale terminal snapshot can overwrite creature/Buff-only state.
- Several Buff/GM callers still need commit-result-aware candidate packet gating.
- IntelligentCreature adoption/support can commit auth before world apply failure.
- Some auth CAS callbacks remain synchronous while `world.mu` is held; only disk
  SaveJSON I/O moved out.
- SaveJSON post-memory-commit policy is not uniform across all support paths.
- Logout side effects begin before an uncommitted JSON failure can report failure;
  ordering/rollback remains a later transaction.

## Verification ledger

Passed on the committed production implementation:

- auth item/IntelligentCreature snapshot suites at repeated count 10 and focused
  race count 3;
- production conflict sessions for IntelligentCreature pickup, mount retry,
  durability, mining, revival, Plague and explicit logout at repeated count 5/10
  and focused race count 3;
- `TestSessionReincarnationImportedNoReincarnationDenialTranscript` count 10 and
  focused race count 3 after the terminal-health correction;
- complete `internal/auth`, `go vet ./...`, `go build ./...`, `git diff --check`,
  Go formatting and both-repository tracked/staged/untracked `.cs` audits.

Full integration attribution:

- `go test ./... -count=1 -timeout=900s` ran every package. Its sole failure was
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin`: packet 26 while
  expecting 206.
- `go test -race ./... -count=1 -timeout=1800s` had the same sole test failure and
  no race detector report.
- A non-destructive `git archive HEAD` baseline in a unique job directory ran the
  Quest test at count 3 and reproduced packet 26 on untouched Go HEAD. This is a
  pre-existing integration baseline, not a snapshot-hardening failure.
- The archive baseline passed the Reincarnation test at count 3. Current work
  initially failed it, the terminal-health fix corrected the regression, and the
  current repeated/race runs pass.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and run all
   three `.cs` queries in both repositories.
2. Read only active matrix rows 965 and 3543 plus this handoff/current index.
3. Freeze the Legacy Hero EquipItem/RemoveItem finite source/target, gate and
   packet/stat order matrix. Legacy C# is read-only.
4. Implement `WS-EQUIP-HERO-EQUIP-REMOVE-001` through existing CrossGrid/Hero
   authority and real authenticated session entries. Do not create a parallel
   item transaction system or modify P8 lifecycle behavior.
5. Run focused, repeated and race gates; attribute the known Quest P7 baseline
   explicitly during full integration gates.
6. Update the Go matrix, active index and this handoff, create the next Go feature
   commit and Legacy evidence commit, then continue the same Primary leaf.

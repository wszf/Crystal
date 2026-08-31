# Crystal Go migration current handoff

Last updated: 2026-09-01 01:24 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is the sole Active unfinished child.
- `WS-ITEM-P6-USE-HERO-POTION-BUFF-003-005-001` is Complete in Go `8596f22`;
  this closes one bounded workstream only, not the active leaf, P6 or the Goal.
- Primary dependency-ready leaf remains `ITEM-P6-USE-CATALOG-001`, matrix row 3546
  and P6 summary row 965.
- Active correction is `WS-ITEM-P6-USE-HERO-BUFF-SEAL-LIFECYCLE-001`: preserve
  only live Hero Buff/clock state across successful seal and same-process
  SealedHero reattach without reopening the completed SealedHero contract.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `7c0caf13`
  (`Record Potion Shape 4 and 5 migration`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `8596f22` (`Migrate Hero Potion stat Buffs`), not pushed.
- The committed feature tree is clean and no generated binary remains in the repo.

## Completed Hero Potion Shape 3-5 evidence

- Authenticated HeroInventory Potion Shape 3 maps base plus added item Stats into
  the fixed max-only Impact/Magic/Taoist/Storm/HealthAid/ManaAid/Defence/
  MagicDefence/BagWeight order. Each total must be positive; zero-stat items still
  consume successfully.
- Shape 4/5 always create private invisible Exp/Drop Buffs from base plus added Luck.
  Zero produces sparse empty Stats, negative values remain, and duration preserves
  Legacy Int32 `durability * 60000` overflow.
- The production path reuses the existing Hero latest-auth item revision/CAS,
  Hero persistence callback, Buff runtime and Player Potion projection barrier.
  It does not add a timer, queue, Hero persistence store or item authority.
- Auth commit and runtime install are atomic. JSON is saved before Item Lost/AddBuff;
  deferred PauseBuff/RemoveBuff/health projection drains after AddBuff and before
  `UseItem=true`, including zero/nonpositive-duration immediate expiry.
- StackDuration keeps the first Stats and existing per-Buff LastTime/NextTime phase;
  each new Buff has an independent clock. Finite paused nonpositive Buffs still expire,
  Infinite Buffs remain, and simultaneous removals project in Legacy reverse order.
- Hero stat refresh now advances HeroRevision, feeds real Exp/drop/attack-speed
  consumers, and projects HP/MP clamps after HealthAid/ManaAid expiry.
- Safe-zone pause follows real boundary changes across walking, owner-follow teleport,
  AssassinBird/Oma-family push, Shoulder Dash, HornedMage and TurtleKing teleport.
  A paused dormant Buff respawned outside remains paused until a real enter/exit edge,
  matching Legacy `InSafeZone` setter behavior.
- Ordinary live despawn/resummon preserves Buffs and clocks, resets LastTime while
  retaining NextTime, replays AddBuff, and restores hiding before ObjectHero projection
  plus monster-target cleanup. Logout/relogin intentionally drops Buffs because Legacy
  HeroInfo Save/Load does not serialize them.
- Strict review findings for ObjectHero hidden projection and Shoulder Dash safe-zone
  reconciliation were fixed. The proposed outside-resummon unpause change was rejected
  after direct Legacy tracing and retained as an explicit regression test.

## Verification ledger

Passed for Go `8596f22` before commit, with final post-review reruns where noted:

- focused Hero Shape 3-5 domain/auth/session, projection, phase, safe-zone, dormant,
  Exp/drop/attack-speed and expiry tests at count 20;
- post-review hiding/ObjectHero, generic forced movement and Shoulder Dash boundary
  tests at count 20, plus focused race at count 5;
- adjacent Player Potion and AssassinBird/HornedMage/TurtleKing production tests;
- full `go test ./... -skip '^TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin$' -count=1`;
- full `go test -race ./...` with the Quest fixture plus registered isolated-green
  `TestProductionTickerInitializesLightFromUTCOnNonUTCHost` and
  `TestSessionHallucinationTranscript` skips;
- `go vet ./...`, `go build ./...`, formatting, `git diff --check`, control-plane
  checker, staged diff check and final independent read-only review with no finding;
- tracked/staged/untracked `.cs` audits in both repositories returned no paths.

The Quest P7 fixture still fails on the unchanged `e7c5a10` baseline with
`mail packet id = 26, want 206`, so it remains a registered pre-existing blocker.
One earlier full-suite OmaMage transcript produced attack-roll bounds `[2 1]` instead
of `[1]`; it passed in isolation at count 20 and the final full non-race rerun passed,
so it was recorded as a one-off full-suite flake rather than skipped.

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-CATALOG-001`.
- Completed workstreams: `WS-ITEM-P6-USE-NOOP-CONSUME-001` at Go `84adba5`,
  `WS-ITEM-P6-USE-POTION-BUFF-003-001` at Go `c4f1e38`,
  `WS-ITEM-P6-USE-POTION-RATE-004-005-001` at Go `e7c5a10`, and
  `WS-ITEM-P6-USE-HERO-POTION-BUFF-003-005-001` at Go `8596f22`.
- Active correction: `WS-ITEM-P6-USE-HERO-BUFF-SEAL-LIFECYCLE-001`.
- Verified regression: Legacy seal removes the Hero from the character slots but keeps
  the same global live HeroInfo, whose Buff list is reused by same-process SealedHero
  attach. Go `commitDetachedHero` currently deletes `DormantHeroBuffs[heroID]` when the
  Hero leaves the slot array, so the reattached Hero loses live Buffs.
- Preserve the completed seal/SealedHero item transaction, admission, persistence,
  Hero revision/CAS, consume/report and spawn behavior. Do not persist Buffs to JSON,
  retain deleted Heroes, or change failed/stale attach outcomes.
- Do not intercept Player Potion, Hero Potion Shape 0-5, Scroll, Food, Pets, Book,
  Script, Transform, Deco, MonsterSpawn or any other SealedHero business behavior.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3546 plus the active index and this handoff.
3. Trace read-only Legacy SealedHero use at `PlayerObject.cs:6318-6337` and
   `SealHero`/`AddHero`/`SpawnHero` around `PlayerObject.cs:14528-14611`; do not write C#.
4. Trace Go `stageHeroDetach`, `commitDetachedHero`, SealedHero item attach and current
   `DormantHeroBuffs` ownership. Freeze summoned/unsummoned seal, failed attach,
   same-process reattach and relogin boundaries before editing.
5. Reuse the existing live dormant sidecar and authenticated Hero item authority;
   transfer only the successfully sealed Hero's Buff/clock state and never serialize it.
6. Verify authenticated seal→reattach, failure/no-consume, logout/relogin, count-20,
   focused race and integration gates; update evidence, create Go and Legacy commits,
   then continue the persistent Goal without push or merge.

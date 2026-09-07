# Crystal Go migration current handoff

Last updated: 2026-08-26 13:08 (Asia/Singapore)

This replace-in-place snapshot completes the hard-gate recovery after a real
compaction signal and an interrupted continuation. The 12:03 snapshot became
stale because the Go candidate continued changing through 12:38. The main
thread is now the only writer lock, no Go/server process is running, and two
audits at 12:41 and 13:07 found the same Go fingerprint. No implementation or
test resumed after that mismatch was discovered. This snapshot claims no leaf,
phase, or project closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither Complete nor Blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- The unique Active Leaf remains `REGEN-P5-HUMAN-001`. P5 is scope-frozen with
  fourteen children: nine Complete, this leaf Active, and four Ready.
- Active anchors are matrix P5 summary row 851, registry row 3165, and completed
  map-hazard evidence 3179-3193. Normal recovery must read only those anchors.
- The older unique uncommitted snapshot remains preserved once at
  `tasks/migration-handoff-archive/2026-08-26-1032-pre-regen-review.md`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `139d5b4cf41aaa1b37f9e1f8bcc9b5acc993d452`.
- Index/staged set is empty. Unstaged tracked files before this replacement were
  exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- The only untracked file is
  `tasks/migration-handoff-archive/2026-08-26-1032-pre-regen-review.md`
  (116 lines, 6109 bytes, SHA-256
  `097776898fcad1d4648fc027cbc95eb7247195cfffe933d786230200a1b51c3c`).
- Immediately before replacement the tracked diff was 199 insertions/101
  deletions with SHA-256
  `893d2837162260e24c4f667dfc58afa8aef2b17fe36185e8770c1113c642bcf9`;
  replacing this file is the expected fingerprint delta.
- Unstaged, staged, and untracked Legacy `.cs` gates were separately empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `f0f5e93e48ba2e79c3ce0a72e1cba61fad802b8d`.
- Index/staged set is empty. The 60 unstaged tracked files are exactly:
  - `internal/config/config.go`, `internal/config/config_test.go`
  - `docs/migration-matrix.md`
  - `cmd/crystal-server/admin_item_commands_session_test.go`
  - `cmd/crystal-server/ancient_bringer.go`, `archer_summons.go`,
    `archer_summons_test.go`, `assassin_bird.go`, `blizzard_session_test.go`, `cat_tongue.go`,
    `conquest_archers.go`, `conquest_archers_test.go`, `curse_test.go`,
    `dark_body_session_test.go`, `dark_oma_king.go`, `delayed_explosion.go`,
    `earth_golem.go`, `electric_shock_test.go`, `elemental.go`,
    `elemental_test.go`, `elephant_man.go`, `entrapment_session_test.go`,
    `general_meow_meow.go`, `hallucination_test.go`, `healing_circle.go`,
    `healing_circle_session_test.go`, `healing_circle_test.go`,
    `hero_potions_test.go`, `heroes.go`, `heroes_test.go`,
    `hiding_buffs_session_test.go`, `horned_sorceror_test.go`,
    `human_assassin.go`, `human_wizard_session_test.go`,
    `human_wizard_test.go`, `item_transactions.go`, `main.go`, `main_test.go`,
    `mass_healing_session_test.go`, `mass_healing_test.go`,
    `mirroring_session_test.go`, `moon_light_test.go`,
    `mpeater_session_test.go`, `oma_witch_doctor.go`,
    `p3_start_logout_session_test.go`, `pet_enhancer_test.go`, `plague_test.go`,
    `player_spell_buffs_session_test.go`, `poison.go`, `rhino_priest.go`,
    `scaly_beast.go`, `soul_fireball.go`, `special_arrow.go`, `stone_golem.go`,
    `stoning_statue.go`, `trainer_test.go`, `tucson_egg.go`,
    `tucson_general.go`, `warrior_attack.go`, `world.go`.
- The tracked diff is 546 insertions/328 deletions with SHA-256
  `9c7bd6147808809b3a9d7e188f309e192b500e09694000391c001dd71065e6ac`.
- The three untracked files are exactly:
  - `cmd/crystal-server/natural_regeneration.go`: 348 lines/10875 bytes,
    SHA-256 `72c5811e10049f4491cac7b73e769eb81c9370aaa9bccff51fe0bc04d171db75`
  - `cmd/crystal-server/natural_regeneration_session_test.go`: 217 lines/9093
    bytes, SHA-256
    `24bf0ab61aaa594ab6968dff770c62a6b6df9f9334853e6eed94316b4e7993d6`
  - `cmd/crystal-server/natural_regeneration_test.go`: 803 lines/33767 bytes,
    SHA-256 `7cbb71271abab979d85336a871082f3a7d10c31bda4fc7b3abc84a1515417801`
- Unstaged, staged, and untracked Go `.cs` gates were separately empty. At
  13:07 no Go/server process was running and the only non-coordination writer
  lock was this main Goal thread
  `01a02fde-6d48-7613-8545-015d3628e9f0`.

## Active leaf and protected work

- Active leaf: `REGEN-P5-HUMAN-001`.
- Exact owned authority is frozen in `tasks/migration-active.md`; do not expand
  it. The candidate covers config weights, Player/Hero runtime pools/timers,
  natural/Pot/Heal/Vamp aggregation, combat/poison reset wiring,
  authenticated/focused tests, and timer isolation in non-owner fixtures.
- The previous independent reviews found: inventory commit preceding fallible
  potion-pool installation; full-health/full-mana pool clearing; incomplete
  Player/Hero Revelation health fanout/expiry; summoned Vampire requested vs
  effective damage; MassHealing immediate monster HP mutation; potentially
  over-strict ranged reset admission; unproved extreme ushort/float widths; and
  optional SafeZoneHealing scope.
- After the stale 12:03 snapshot, the candidate changed in
  `item_transactions.go`, `natural_regeneration.go`, `main.go`, `world.go`,
  `archer_summons_test.go`, `mass_healing_session_test.go`, and
  `natural_regeneration_test.go`, with the last mtime at 12:38. The tracked diff
  is now 546/328 and two untracked hashes changed. Treat all post-snapshot work
  as unverified review-fix work: inspect every finding and diff before testing.
- `COMBAT-P5-HP-DRAIN-001` and `REGEN-P5-SAFEZONE-001` are separate Ready
  children. Do not implement either, redesign committed map hazards, or touch
  any `.cs` file while closing this leaf.
- Read-only Luna `01a03bec-d1f9-7811-9204-06e61e7cfee3` supplied the bounded
  Legacy trace; read-only Luna `01a03bed-0d8e-7413-a302-08dfad31d5f5` reviewed
  the earlier Go candidate. Both threads were already stopped/closed.

## Verification ledger

- `git diff --check` in Go: exit 0 at the 11:38 recovery/review snapshot; it has
  not been rerun against the current 13:07 fingerprint.
- Earlier compile `go test ./cmd/crystal-server ./internal/config -run '^$'`:
  exit 0, but it predates later review-fix edits and is stale.
- Earlier focused lethal-reset/Human-regen/restart checkpoint: exit 0 as
  reported by the Legacy auditor; it is not a final current-candidate gate.
- First unexcluded package run after production wiring:
  `go test ./cmd/crystal-server ./internal/config -count=1`: exit 1.
  `TestElectricShockSelfPetAndOtherTargetShockRefreshVisuals` observed an extra
  health packet and `TestSessionElementalShotObtainTranscript` timed out on
  unsolicited regen writes; attribution is archived in
  `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`.
- After fixture-only timer isolation, the exact two-test focused rerun exited 0.
  The earlier reviewer also ran the MassHealing delayed-player focused test with
  exit 0. These outcomes predate the current 13:07 fingerprint.
- No reliable test result exists for the current 13:07 fingerprint. Therefore
  compile, fresh unexcluded package, focused
  count-20, race-count-5, authenticated failure/relogin/restart, integration,
  full race, vet/build, terminal review, and final gates all remain required.

## Exact recovery sequence

1. Read `tasks/lessons.md`, `tasks/goal-task.md`, `tasks/migration-active.md` and
   this handoff; read only matrix rows 851, 3165 and 3179-3193. Reverify both
   roots/branches/HEADs/statuses/fingerprints and all six `.cs` gates using
   separate single-repository calls.
2. Run `tasks/check-migration-control.sh` in Legacy and `git diff --check` in Go.
   Then inspect the complete current owned diff and rule on every bounded review finding
   against the cited Legacy authority before making another edit.
3. Keep fixes within frozen Regen authority. Run `gofmt` only on owned changed
   Go files, then compile with
   `go test ./cmd/crystal-server ./internal/config -run '^$'` before behavior
   tests. Run every Leaf gate in `tasks/goal-task.md`, including focused
   production tests, count-20, race-count-5, authenticated potion/relogin/
   restart, and all due integration/full-race/vet/build gates.
4. Obtain terminal bounded read-only Luna review, integrate accepted findings,
   update matrix/index/handoff, rerun control/diff/status/six `.cs` gates, and
   commit only after every required command exits zero. Continue the same Goal.

# Crystal Go migration current handoff

Last updated: 2026-08-26 14:25 (Asia/Singapore)

This replace-in-place snapshot is the durable safety gate for the real
compaction that interrupted the Human regeneration leaf. The prior 13:08
snapshot was stale: the Go candidate continued changing through 14:19 and grew
from 60 to 66 tracked files. No implementation or test has run after recovery.
The stale snapshot is preserved once at
`tasks/migration-handoff-archive/2026-08-26-1424-pre-review-fixes.md`. The main
thread is now the only writer lock, no Go/server process is running, and this
snapshot claims no leaf, phase, or Goal closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither Complete nor Blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- The unique Active Leaf remains `REGEN-P5-HUMAN-001`. P5 is scope-frozen with
  fourteen children: nine Complete, this leaf Active, and four Ready.
- Active anchors are matrix P5 summary row 851, registry row 3165, and completed
  map-hazard evidence 3179-3193. Normal recovery must read only those anchors.
- Exact owned Go authority remains frozen in `tasks/migration-active.md`; the
  current 66 tracked and three untracked Go files listed below are the complete
  candidate set. Do not expand it.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `139d5b4cf41aaa1b37f9e1f8bcc9b5acc993d452`.
- Index/staged set is empty. The five unstaged tracked files are exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- The two untracked files are exactly:
  - `tasks/migration-handoff-archive/2026-08-26-1032-pre-regen-review.md`
    (116 lines/6109 bytes, SHA-256
    `097776898fcad1d4648fc027cbc95eb7247195cfffe933d786230200a1b51c3c`)
  - `tasks/migration-handoff-archive/2026-08-26-1424-pre-review-fixes.md`
    (154 lines/9084 bytes, SHA-256
    `c49dcee5341aadd1e62d7db1b8bab7e4bafaabf767bb7537193c0e37d332e183`)
- Immediately before this replacement, the tracked diff was 214 insertions/101
  deletions with SHA-256
  `1361c0259b44fa99e7dc1bd379c783bdde44b3821f2e5b1be67ed2f6626cb9d2`;
  replacing this file is the expected fingerprint delta.
- Unstaged, staged, and untracked Legacy `.cs` gates were separately empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `f0f5e93e48ba2e79c3ce0a72e1cba61fad802b8d`.
- Index/staged set is empty. The 66 unstaged tracked files are exactly:
  - `internal/config/config.go`, `internal/config/config_test.go`
  - `docs/migration-matrix.md`
  - `cmd/crystal-server/admin_item_commands_session_test.go`
  - `cmd/crystal-server/ancient_bringer.go`, `archer_summons.go`,
    `archer_summons_test.go`, `assassin_bird.go`, `blizzard_session_test.go`,
    `cat_tongue.go`, `conquest_archers.go`, `conquest_archers_test.go`,
    `curse_test.go`, `dark_body_session_test.go`, `dark_oma_king.go`,
    `delayed_explosion.go`, `earth_golem.go`, `electric_shock_test.go`,
    `elemental.go`, `elemental_test.go`, `elephant_man.go`,
    `entrapment_session_test.go`, `general_meow_meow.go`,
    `hallucination_test.go`, `healing_circle.go`,
    `healing_circle_session_test.go`, `healing_circle_test.go`,
    `hero_potions_test.go`, `heroes.go`, `heroes_test.go`,
    `hiding_buffs_session_test.go`, `horned_sorceror_test.go`,
    `human_assassin.go`, `human_wizard_session_test.go`,
    `human_wizard_test.go`, `item_transactions.go`, `main.go`, `main_test.go`,
    `mass_healing_session_test.go`, `mass_healing_test.go`,
    `mirroring_session_test.go`, `moon_light_test.go`,
    `mpeater_session_test.go`, `oma_witch_doctor.go`,
    `p3_start_logout_session_test.go`, `pet_enhancer_test.go`, `plague_test.go`,
    `player_spell_buffs_session_test.go`, `player_spell_buffs_test.go`,
    `player_spell_instant_buffs_test.go`, `poison.go`, `rhino_priest.go`,
    `scaly_beast.go`, `soul_fireball.go`, `special_arrow.go`, `stone_golem.go`,
    `stoning_statue.go`, `trainer_test.go`, `trap_test.go`, `tucson_egg.go`,
    `tucson_general.go`, `ultimate_enhancer_test.go`,
    `use_item_session_test.go`, `warrior_attack.go`, `world.go`, `world_test.go`.
- The tracked diff is 741 insertions/342 deletions with SHA-256
  `67e585dc6de78ddeebe6ac4843ea7fedc04a5d0d3649f117b5fc64f26453e06a`.
- The three untracked files are exactly:
  - `cmd/crystal-server/natural_regeneration.go`: 346 lines/11020 bytes,
    SHA-256 `b5fd285a5f986bcc994c194ae65af5d489460bda2346cb751ed81794352e554e`
  - `cmd/crystal-server/natural_regeneration_session_test.go`: 224 lines/9274
    bytes, SHA-256
    `360cee1c720b62ce79717897d1cf098d1230dbb3d2274efbf87a4019fdbb984c`
  - `cmd/crystal-server/natural_regeneration_test.go`: 863 lines/36106 bytes,
    SHA-256 `5f8148464407b72cdecfeeb361eee58145f4cab293b1e2f6e095b726093477de`
- Unstaged, staged, and untracked Go `.cs` gates were separately empty.
- Latest dirty mtime was 14:19:02 on
  `natural_regeneration_session_test.go`. At 14:24 no `go` or
  `crystal-server` process was running. The only thread-writer lock besides
  `.coordination.lock` was this Goal thread
  `01a02fde-6d48-7613-8545-015d3628e9f0`.

## Active leaf and protected work

- Active leaf: `REGEN-P5-HUMAN-001`.
- The candidate covers config weights, Player/Hero runtime pools/timers,
  natural/Pot/Heal/Vamp aggregation, combat/poison reset wiring,
  authenticated/focused tests, and timer isolation in non-owner fixtures.
- Earlier bounded review findings were inventory commit preceding fallible
  potion-pool installation; full-health/full-mana pool clearing; incomplete
  Player/Hero Revelation health fanout/expiry; summoned Vampire requested vs
  effective damage; MassHealing immediate monster HP mutation; potentially
  over-strict ranged reset admission; unproved extreme ushort/float widths; and
  optional SafeZoneHealing scope.
- The post-13:08 candidate added or changed six more tracked files and changed
  every untracked hash. Treat all current work as unverified review-fix work:
  inspect the complete owned diff and rule every finding against bounded Legacy
  authority before another edit or test.
- `COMBAT-P5-HP-DRAIN-001` and `REGEN-P5-SAFEZONE-001` are separate Ready
  children. Do not implement either, redesign committed map hazards, or touch
  any `.cs` file while closing this leaf.
- Prior read-only Luna threads were already stopped/closed. Do not assume their
  reports cover the current fingerprint.

## Verification ledger

- The first recovery status command mixed both repositories and is discarded
  under lesson C01. Subsequent Legacy and Go status/fingerprint/`.cs` audits
  were independent, complete, and exited 0.
- `git diff --check` in Go last exited 0 at the older 11:38 fingerprint; it has
  not been rerun against the current candidate.
- Earlier touched-package compile and focused tests predate the current
  fingerprint and are stale.
- The first unexcluded package run after production wiring exited 1:
  `TestElectricShockSelfPetAndOtherTargetShockRefreshVisuals` observed an extra
  health packet and `TestSessionElementalShotObtainTranscript` timed out on
  unsolicited regen writes. Their exact focused rerun exited 0 after fixture
  timer isolation, but that result also predates the current fingerprint.
- No reliable behavior result exists for the current fingerprint. Compile,
  fresh unexcluded package, focused count-20, race-count-5, authenticated
  failure/relogin/restart, due integration/full-race/vet/build, terminal review,
  and all final gates remain required.

## Exact recovery sequence

1. Reverify this snapshot against each repository separately; run
   `tasks/check-migration-control.sh` in Legacy and `git diff --check` in Go.
   Read only matrix rows 851, 3165 and 3179-3193.
2. Inspect the complete current owned diff and rule every bounded review finding
   against cited Legacy authority before making another edit. Search only
   matching lesson-archive sections for Regen/timer/transcript/repository-boundary
   keywords.
3. Keep fixes within frozen Regen authority. Run `gofmt` only on owned changed
   Go files, then compile with
   `go test ./cmd/crystal-server ./internal/config -run '^$'` before behavior
   tests. Run every Leaf gate in `tasks/goal-task.md`, including focused
   production tests, count-20, race-count-5, authenticated potion/relogin/
   restart, and all due integration/full-race/vet/build gates.
4. Obtain terminal bounded read-only Luna review, integrate accepted findings,
   update matrix/index/handoff, rerun control/diff/status/six `.cs` gates, and
   commit only after every required command exits zero. Continue the same Goal.

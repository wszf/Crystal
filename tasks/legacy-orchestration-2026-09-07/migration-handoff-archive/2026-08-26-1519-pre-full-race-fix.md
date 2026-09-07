# Crystal Go migration current handoff

Last updated: 2026-08-26 15:19 (Asia/Singapore)

This replace-in-place snapshot reconstructs the durable state after recovery
found that the prior 14:25 handoff was stale: the Go candidate had continued
changing through 15:04 and now contains 76 tracked plus three untracked files.
The stale snapshot is preserved once at
`tasks/migration-handoff-archive/2026-08-26-regen-76-file-recovery.md` (156
lines/8970 bytes, SHA-256
`6284b28ca3030d4093969f58fc1fc1a41cbfcb6c1d5034dff0b6e5601652d33f`).
The sole inherited read-only worker confirmed it made no writes or tests and was
closed. No implementation or behavior test ran during this reconstruction; the
main thread is the only writer and this snapshot claims no leaf/phase closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither Complete nor Blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- The unique Active Leaf is `REGEN-P5-HUMAN-001`. P5 is scope-frozen with
  fourteen children: nine Complete, this leaf Active, and four Ready.
- Read only matrix P5 summary row 851, registry row 3165, and completed hazard
  evidence 3179-3193 during normal recovery.
- Exact read/write authority and required leaf gate remain frozen in
  `tasks/migration-active.md` lines 42-109. Every dirty Go file below is within
  that registered authority; do not expand it.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `139d5b4cf41aaa1b37f9e1f8bcc9b5acc993d452`.
- Index/staged set is empty. The five unstaged tracked files are exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Excluding this self-changing handoff, the other four tracked diffs are 105
  insertions/43 deletions with SHA-256
  `3e8048e19794eea9556df2626d487d7922b9a31d8909dc7dc220f691341098bc`.
- The three untracked archive snapshots are exactly:
  - `tasks/migration-handoff-archive/2026-08-26-1032-pre-regen-review.md`
    (116 lines/6109 bytes, SHA-256
    `097776898fcad1d4648fc027cbc95eb7247195cfffe933d786230200a1b51c3c`)
  - `tasks/migration-handoff-archive/2026-08-26-1424-pre-review-fixes.md`
    (154 lines/9084 bytes, SHA-256
    `c49dcee5341aadd1e62d7db1b8bab7e4bafaabf767bb7537193c0e37d332e183`)
  - `tasks/migration-handoff-archive/2026-08-26-regen-76-file-recovery.md`
    (metadata and hash above)
- `tasks/check-migration-control.sh` exited 0 at 15:19. Unstaged, staged,
  and untracked Legacy `.cs` gates separately exited 0 with empty output.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `f0f5e93e48ba2e79c3ce0a72e1cba61fad802b8d`.
- Index/staged set is empty. The 76 unstaged tracked files are exactly:
  - `internal/config/config.go`, `internal/config/config_test.go`
  - `docs/migration-matrix.md`
  - `cmd/crystal-server/admin_item_commands_session_test.go`
  - `cmd/crystal-server/ancient_bringer.go`, `archer_summons.go`,
    `archer_summons_test.go`, `assassin_bird.go`,
    `blizzard_session_test.go`, `cat_tongue.go`, `conquest_archers.go`,
    `conquest_archers_test.go`, `curse_test.go`, `dark_body_session_test.go`,
    `dark_oma_king.go`, `delayed_explosion.go`, `earth_golem.go`,
    `electric_shock_test.go`, `elemental.go`, `elemental_test.go`,
    `elephant_man.go`, `entrapment_session_test.go`,
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
    `player_spell_buffs_session_test.go`, `player_spell_buffs_test.go`,
    `player_spell_instant_buffs_test.go`, `poison.go`, `poison_test.go`,
    `portal_test.go`, `rhino_priest.go`, `rhino_priest_test.go`,
    `scaly_beast.go`, `slashing_burst_session_test.go`, `soul_fireball.go`,
    `soul_fireball_test.go`, `special_arrow.go`, `stone_golem.go`,
    `stone_trap_test.go`, `stoning_statue.go`, `support_buffs_test.go`,
    `trainer_test.go`, `trap_hexagon_session_test.go`,
    `trap_hexagon_test.go`, `trap_test.go`, `tucson_egg.go`,
    `tucson_general.go`, `ultimate_enhancer_test.go`,
    `use_item_session_test.go`, `utility_command_session_test.go`,
    `warrior_attack.go`, `world.go`, `world_test.go`.
- The tracked diff is 794 insertions/351 deletions with SHA-256
  `57e23a9acf55ad2fe82c20fe7f288795861910d578aadc37b9695e8067749a99`.
- The three untracked files are exactly:
  - `cmd/crystal-server/natural_regeneration.go`: 346 lines/11020 bytes,
    SHA-256 `b5fd285a5f986bcc994c194ae65af5d489460bda2346cb751ed81794352e554e`
  - `cmd/crystal-server/natural_regeneration_session_test.go`: 224 lines/9274
    bytes, SHA-256
    `360cee1c720b62ce79717897d1cf098d1230dbb3d2274efbf87a4019fdbb984c`
  - `cmd/crystal-server/natural_regeneration_test.go`: 880 lines/37183 bytes,
    SHA-256 `c33e62db2fb680a2c8b74f88a7d8f61ca22f29f4e7796fc7a067375a3d4064ff`
- Latest dirty mtime is 15:04:16 on `natural_regeneration_test.go`.
  At 15:13 exact-command process audit found no `go` or `crystal-server`
  process, and no repository-local `*.lock` file exists.
- `git diff --check` exited 0 at 15:06. Unstaged, staged, and untracked Go
  `.cs` gates separately exited 0 with empty output.

## Active leaf and protected work

- Active leaf: `REGEN-P5-HUMAN-001`.
- Candidate scope is config weights; Player/Hero runtime pools/timers; natural,
  Pot, Heal and Vamp aggregation; combat/poison reset wiring; authenticated and
  focused tests; and timer isolation only in finite non-owner fixtures.
- Prior review findings were fallible potion-pool installation after inventory
  commit; full-health/full-mana clearing; incomplete Player/Hero Revelation
  health fanout/expiry; summoned Vampire requested-versus-effective damage;
  MassHealing immediate monster HP mutation; overly strict ranged reset
  admission; unproved extreme ushort/float widths; and optional SafeZoneHealing.
- Later edits added finite timer-isolation fixtures and changed production
  `poison.go` plus the Regen focused test. Treat all current work as an
  unverified review-fix candidate: inspect the complete owned diff and rule each
  finding against bounded Legacy authority before another edit or behavior test.
- `COMBAT-P5-HP-DRAIN-001` and `REGEN-P5-SAFEZONE-001` are separate Ready
  children. Do not implement them, redesign committed map hazards, or touch any
  `.cs` file while closing this leaf.

## Verification ledger

- Recovery status, matrix anchors, control check, diff check and all six `.cs`
  gates were repository-local, complete, and exited 0. One later mixed-root
  Revelation query was discarded, independently rerun, and recorded in C01.
- A prior fresh unexcluded package run reportedly passed in 76.904s/1.046s after
  finite timer isolation, but it predates the current 15:04 fingerprint and is
  stale. Earlier focused/package results are likewise not acceptance evidence.
- No compile or behavior result exists for the current fingerprint. Required:
  owned diff review; gofmt; touched compile; focused production tests;
  count-20; race-count-5; authenticated potion/relogin/restart; fresh unexcluded
  package/integration/full-race/vet/build as due; terminal bounded review; final
  diff/status/control/six `.cs` gates.

## Exact recovery sequence

1. Verify this handoff separately against each root. In Legacy run
   `tasks/check-migration-control.sh`; in Go run `git diff --check`, status and
   the three `.cs` queries. Read only matrix rows 851, 3165, and 3179-3193.
2. Review the complete dirty Go diff against the frozen behavior checklist and
   bounded Legacy authority. Resolve each prior finding without expanding scope.
3. Run gofmt only on owned changed Go files, then
   `go test ./cmd/crystal-server ./internal/config -run '^$'`. Continue through
   every leaf/integration/full-race gate required by `tasks/goal-task.md`.
4. Obtain terminal bounded read-only Luna review, integrate accepted findings,
   update matrix/index/handoff, rerun final control/diff/status/six `.cs` gates,
   and commit only owned files after every required command exits zero. Then
   route the next dependency-ready leaf in the same Goal.

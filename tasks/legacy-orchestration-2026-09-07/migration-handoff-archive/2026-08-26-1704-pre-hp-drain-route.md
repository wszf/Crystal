# Crystal Go migration current handoff

Last updated: 2026-08-26 15:50 (Asia/Singapore)

This replace-in-place snapshot closes a real compaction recovery gate. The
15:19 snapshot became stale when `main.go` changed at 15:34 to repair the first
current-candidate full-race finding and `docs/migration-matrix.md` changed at
15:44 to register three finite P5 findings. That prior snapshot is preserved
once at `tasks/migration-handoff-archive/2026-08-26-1519-pre-full-race-fix.md`
(152 lines/8926 bytes, SHA-256
`7b104d722571e96f2de032f40556c6dd4c6ac1929790b5646898f282db960321`).
No implementation or behavior test ran during this reconstruction. The first
recovery status call mixed both roots and is discarded in full; every fact below
comes from later independent repository-local zero-exit calls.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active and unchanged;
  it is neither Complete nor Blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- The unique Active Leaf is `REGEN-P5-HUMAN-001`. P5 is scope-frozen with
  fifteen children: nine Complete, this leaf Active, and five Ready.
- Read only matrix P5 summary row 851, registry row 3165, and completed hazard
  evidence 3184-3199 during normal recovery.
- Exact read/write authority and the leaf gate are frozen in
  `tasks/migration-active.md` lines 37-106. The current dirty Go list below is
  entirely inside that authority; no additional path is owned.
- `tasks/migration-active.md` is back at its enforced maximum of 300 lines and
  23906 bytes after whitespace-only compaction; no routing content was removed.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `139d5b4cf41aaa1b37f9e1f8bcc9b5acc993d452`.
- Index/staged set is empty. The six unstaged tracked files are exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons-archive/verification/race-and-flake-attribution.md`
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Excluding this self-changing handoff, the other five tracked diffs have
  SHA-256 `8daf016a327b1070c17360ab0069e73de49cc590be688f1b592a5b816de19969`.
- The four untracked archive snapshots are exactly:
  - `tasks/migration-handoff-archive/2026-08-26-1032-pre-regen-review.md`
    (116 lines/6109 bytes, SHA-256
    `097776898fcad1d4648fc027cbc95eb7247195cfffe933d786230200a1b51c3c`)
  - `tasks/migration-handoff-archive/2026-08-26-1424-pre-review-fixes.md`
    (154 lines/9084 bytes, SHA-256
    `c49dcee5341aadd1e62d7db1b8bab7e4bafaabf767bb7537193c0e37d332e183`)
  - `tasks/migration-handoff-archive/2026-08-26-regen-76-file-recovery.md`
    (156 lines/8970 bytes, SHA-256
    `6284b28ca3030d4093969f58fc1fc1a41cbfcb6c1d5034dff0b6e5601652d33f`)
  - `tasks/migration-handoff-archive/2026-08-26-1519-pre-full-race-fix.md`
    (metadata and hash above)
- Post-replacement `tasks/check-migration-control.sh`, `git diff --check`, and
  all three Legacy `.cs` gates exited 0; the control snapshot is 160 lines and
  9381 bytes before this two-line evidence replacement.

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
- The tracked diff is 802 insertions/352 deletions with SHA-256
  `03cced1566bf10fde582e57b873cff1bd75d9f49a64ef4b1818bc5fe90473902`.
- The three untracked files are exactly:
  - `cmd/crystal-server/natural_regeneration.go`: 346 lines/11020 bytes,
    SHA-256 `b5fd285a5f986bcc994c194ae65af5d489460bda2346cb751ed81794352e554e`
  - `cmd/crystal-server/natural_regeneration_session_test.go`: 224 lines/9274
    bytes, SHA-256
    `360cee1c720b62ce79717897d1cf098d1230dbb3d2274efbf87a4019fdbb984c`
  - `cmd/crystal-server/natural_regeneration_test.go`: 880 lines/37183 bytes,
    SHA-256 `c33e62db2fb680a2c8b74f88a7d8f61ca22f29f4e7796fc7a067375a3d4064ff`
- Latest tracked dirty mtime is 15:44:05 on the matrix; latest production mtime
  is 15:34:43 on `main.go`. Repository-local `*.lock` search is empty.
- Current `git diff --check` and all three Go `.cs` gates exited 0 with empty
  output. Exact-command process audit found no `go` or `crystal-server` process.

## Active leaf and protected work

- Active leaf: `REGEN-P5-HUMAN-001`.
- Candidate scope is config weights; Player/Hero runtime pools/timers; natural,
  Pot, Heal and Vamp aggregation; combat/poison reset wiring; authenticated and
  focused tests; and timer isolation only in finite non-owner fixtures.
- Review findings already integrated or converted into finite children include
  potion-pool installation/clearing, Player/Hero health fanout, MassHealing,
  ranged admission, ushort/float boundaries, HP drain, SafeZoneHealing and
  long-Revelation expiry. The current `main.go` change replaces an unlocked copy
  of a captured live player with `playerRuntimeSnapshot(worldObjectID)`.
- `COMBAT-P5-HP-DRAIN-001`, `SPELL-P5-REVELATION-EXPIRE-001`, and
  `REGEN-P5-SAFEZONE-001` are separate Ready children. Do not implement them,
  redesign committed map hazards, or touch any `.cs` file in this leaf.

## Verification ledger

- Before the 15:34 `main.go` fix, fresh unexcluded
  `go test ./cmd/crystal-server ./internal/config -count=1 -timeout=30m`
  exited 0 in 76.904s/1.046s. It is useful regression evidence but stale for the
  current code fingerprint.
- The first current-candidate
  `go test -race ./... -count=1 -timeout=60m` exited 1 only in
  `TestLoverRecallNetworkQueuedTransitionsAreNotOverwritten`. The race was
  ticker writes to Regen/Pot/Heal/Vamp fields versus an unlocked copy of the
  captured live player in `main.go`; no other test or race finding was reported.
- After the `main.go` snapshot fix, touched compile reportedly exited 0 and
  `go test -race ./cmd/crystal-server -run
  '^TestLoverRecallNetworkQueuedTransitionsAreNotOverwritten$' -count=10
  -timeout=20m` exited 0. The compile argv was not preserved and is not accepted
  as a gate; rerun it exactly.
- No fresh full race, vet, build, leaf count-20/race-count-5, or terminal review
  exists after the 15:34 fix. A terminal read-only worker
  `01a03d04-52d9-7a42-87bd-8be4a49c8a9e` was closed while still running at the
  compaction gate; it returned no final report and had no write authority.

## Exact recovery sequence

1. Verify this handoff separately against each root. In Legacy run
   `tasks/check-migration-control.sh`, status and the three `.cs` queries. In Go
   run `git diff --check`, status and the three `.cs` queries. Read only matrix
   rows 851, 3165 and 3184-3199.
2. Rerun `gofmt` only on owned changed Go files, then
   `go test ./cmd/crystal-server ./internal/config -run '^$'`. Review the final
   bounded `main.go` diff and complete focused/count-20/race-count-5 gates.
3. Run fresh unexcluded package/integration/full-race/vet/build gates required
   by `tasks/goal-task.md`; preserve exact failures and attribution.
4. Obtain a new terminal bounded read-only Luna review, integrate only accepted
   in-scope findings, update matrix/index/handoff, rerun final control/diff/
   status/six `.cs` gates, and commit only owned files after every required
   command exits zero. Route the next dependency-ready leaf in the same Goal.

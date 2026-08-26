# Crystal Go migration current handoff

Last updated: 2026-08-26 10:32 (Asia/Singapore)

This replace-in-place snapshot was reconstructed after a real compaction signal
because the pre-compaction handoff was stale. Implementation and tests remain
frozen. It claims no leaf, phase, or project closure.

## Goal and control-plane state

- Goal remains Active and unchanged; it is neither Complete nor Blocked.
- Main authority remains `gpt-5.6-sol/ultra`; bounded workers must use
  `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- The unique Active Leaf remains `REGEN-P5-HUMAN-001`. P5 is scope-frozen with
  nine of thirteen children Complete, this leaf Active, and three Ready.
- Active anchors remain matrix P5 summary row 851, registry row 3165, and
  completed map-hazard evidence 3179-3193. No full matrix was read.
- The old 09:34 snapshot described 27 tracked Go modifications and one
  untracked file. It is superseded by the separately verified state below.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; observed HEAD
  `139d5b4cf41aaa1b37f9e1f8bcc9b5acc993d452`.
- The index is empty and there are no untracked files. Unstaged tracked files
  are exactly:
  - `tasks/lessons-archive/workflow/repository-boundaries-02.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Before this replacement, the four-file tracked diff SHA-256 was
  `9a6fdc9207469d3f79f8e5ed9226b70549bdcfa820d8eb0536790d7c29588a3b`;
  this handoff replacement is the expected fingerprint delta.
- Unstaged, staged, and untracked Legacy `.cs` gates are empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; observed HEAD
  `f0f5e93e48ba2e79c3ce0a72e1cba61fad802b8d`.
- The index is empty. The 39 unstaged tracked files are exactly:
  - `internal/config/config.go`, `internal/config/config_test.go`
  - `cmd/crystal-server/world.go`, `main.go`, `heroes.go`,
    `healing_circle.go`, `item_transactions.go`, `poison.go`,
    `special_arrow.go`, `archer_summons.go`
  - `cmd/crystal-server/ancient_bringer.go`, `assassin_bird.go`,
    `cat_tongue.go`, `dark_oma_king.go`, `delayed_explosion.go`,
    `earth_golem.go`, `elemental.go`, `elephant_man.go`,
    `general_meow_meow.go`, `human_assassin.go`, `oma_witch_doctor.go`,
    `rhino_priest.go`, `scaly_beast.go`, `soul_fireball.go`,
    `stone_golem.go`, `stoning_statue.go`, `tucson_egg.go`,
    `tucson_general.go`, `warrior_attack.go`
  - `cmd/crystal-server/admin_item_commands_session_test.go`,
    `blizzard_session_test.go`, `conquest_archers_test.go`, `curse_test.go`,
    `dark_body_session_test.go`, `healing_circle_session_test.go`,
    `healing_circle_test.go`, `hero_potions_test.go`,
    `mass_healing_test.go`
  - `docs/migration-matrix.md`
- The three untracked files are exactly:
  - `cmd/crystal-server/natural_regeneration.go`: 332 lines/10073 bytes,
    SHA-256 `185f232490bf1de651fbcbbf29953e1c3487ca51858caf6d8ab35d2181184835`
  - `cmd/crystal-server/natural_regeneration_session_test.go`: 217
    lines/9093 bytes, SHA-256
    `24bf0ab61aaa594ab6968dff770c62a6b6df9f9334853e6eed94316b4e7993d6`
  - `cmd/crystal-server/natural_regeneration_test.go`: 525 lines/21640
    bytes, SHA-256
    `374f5770561af81abdeaccc10f35beef364e4c66e6a0eb05433773e2c41f3c4e`
- The tracked diff has 264 insertions/239 deletions and SHA-256
  `13949b1a57b63670ba67621bcbff73e339756d8de943887d98aeefc07cc26eec`.
- Unstaged, staged, and untracked Go `.cs` gates are empty.

## Active leaf and protected work

- Active leaf: `REGEN-P5-HUMAN-001`.
- The candidate is an uncommitted, interrupted aggregate. It includes config
  weights, Player/Hero runtime pools/timers, natural/Pot/Heal/Vamp aggregation,
  combat/poison reset wiring, focused tests, and authenticated session tests.
- The two known worker threads
  `01a03bbb-4149-7b33-a501-6aa4b3e35d8a` and
  `01a03bc3-0044-7440-b55f-b85fdae77101` were both still `running` when the
  hard gate closed them. Neither returned a final report. Therefore every
  changed file must be treated as partial and unreviewed until the main Agent
  compiles, inspects, and tests it.
- After closure, the writer-lock directory contained only the main Goal lock
  `01a02fde-6d48-7613-8545-015d3628e9f0.lock` plus its coordination lock.
  No `go`, `crystal-server`, `compile`, `link`, or `vet` process was running.
- Do not implement the Ready `COMBAT-P5-HP-DRAIN-001` sibling or alter protected
  committed map-hazard/earlier migration behavior.

## Verification ledger

- No complete post-write compile/test exit code survived the compaction boundary.
  The interrupted current test is not evidence. Touched compile, focused
  count-20, race count-5, production session/relogin/restart, integration, and
  bounded review are all **not verified** for this candidate.
- Separate zero-exit status calls established 42 dirty Go paths (39 tracked,
  three untracked) and four dirty Legacy paths. Separate zero-exit calls also
  established all six `.cs` gates empty.
- Process and writer-lock audits exited 0 after both known agents were closed.
- No implementation or test command ran after the freeze; only read-only audits
  and this Legacy handoff replacement ran.

## Exact recovery sequence

1. Read this handoff and `tasks/migration-active.md`; separately recheck both
   repositories' HEAD/status and all six `.cs` gates, then verify the Go tracked
   diff and three untracked hashes are stable.
2. In the Go root, run `git diff --check` as the first candidate command. Inspect
   the exact interrupted diff, then run `gofmt` only on owned changed Go files
   and `go test ./cmd/crystal-server ./internal/config -run '^$'` before
   interpreting any behavior test.
3. Finish only the frozen Regen authority, run the complete leaf gate, obtain a
   bounded read-only `luna_worker` review, integrate findings, update the matrix,
   Active Index, and this handoff, run the control and `.cs` gates, then commit
   only after all evidence is zero-exit.

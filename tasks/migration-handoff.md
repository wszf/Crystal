# Crystal Go migration current handoff

Last updated: 2026-08-26 17:48 (Asia/Singapore)

This replace-in-place snapshot closes `COMBAT-P5-HP-DRAIN-001` and routes the
same Goal to Revelation expiry. It is a current snapshot, not a journal.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains Active; it is neither
  Complete nor Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers
  must use `luna_worker` (`gpt-5.6-luna/max`) without substitution.
- HP drain is committed in Go as
  `0db2cdeae072dfed45d39c8e4824caf3597a76ff`. The unique Active Leaf is
  `SPELL-P5-REVELATION-EXPIRE-001`; P5 has fifteen frozen children: eleven
  Complete, this one Active and three Ready.
- Normal recovery may read only matrix P5 summary row 851, Revelation registry
  row 3166 and completed HP-drain evidence 3200-3215.
- Revelation Go write authority remains closed pending its finite trace.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch `master`; HEAD `8508bb0cb325bcf14f5b62894505fccfd2705fdd`;
  upstream `origin/master`, ahead 478.
- Index/staged set is empty. Four unstaged tracked files after this handoff write
  are exactly:
  - `tasks/lessons-archive/verification/fixtures-and-transcripts-03.md`
  - `tasks/lessons.md`
  - `tasks/migration-active.md`
  - `tasks/migration-handoff.md`
- Before the self-changing handoff, the other three diffs were 49 insertions/50
  deletions with SHA-256
  `843dac99e59031e3f7f4d033589348353bfa0a3f649d7138990ae7c424b44052`.
- No untracked files, locks or active subagents remain. Control check,
  `git diff --check` and all three Legacy `.cs` gates exited 0.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch `main`; HEAD `0db2cdeae072dfed45d39c8e4824caf3597a76ff`.
- Worktree and index are clean: no tracked, staged or untracked files.
- Commit `0db2cde` contains the seven HP-drain production/test files plus the
  matrix route/evidence (399 insertions/6 deletions). Staged diff/C# gates and
  post-commit status are clean.

## Active leaf and protected work

- Active leaf: `SPELL-P5-REVELATION-EXPIRE-001`.
- HP drain preserves float32 order, strict `>2`, floor/remainder/cap loss,
  Human-target Hero-to-owner normalization, ordinary-Monster Hero ownership,
  exact packet order, HP persistence and runtime-only reset.
- Revelation tracing must freeze the Legacy cast formula and byte conversion at
  255/256/260 seconds plus completion/passive/bootstrap/movement recipients.
- Until then no Go file may change. HP drain, Regen, SafeZoneHealing, unrelated
  spells, another P5 child and every `.cs` file are protected.

## Verification ledger

- Touched compile `go test ./cmd/crystal-server -run '^$'` exited 0.
- Final focused `go test ./cmd/crystal-server -run 'HPDrain' -count=20
  -timeout=20m` exited 0.
- Final focused race `go test -race ./cmd/crystal-server -run 'HPDrain'
  -count=5 -timeout=20m` exited 0.
- Authenticated attack test proves ObjectAttack -> ObjectStruck -> attacker
  HealthChanged -> target indicator, durable HP, fractional runtime state,
  JSON save/load and zero accumulator after restarted entry.
- The first fresh full `go test ./... -count=1 -timeout=30m` exited 1 only at
  `TestCombatDurabilityOrdinaryPlayerMonsterContextControlsWeaponRoll`: the new
  action flag executed weapon damage twice. Admission metadata was separated
  from durability execution; the exact regression plus HP-drain tests passed.
- Final fresh `go test ./... -count=1 -timeout=30m` exited 0; server 78.727s.
- Final `go vet ./...` and `go build ./...` exited 0.
- Final fresh unexcluded `go test -race ./... -count=1 -timeout=60m` exited 0;
  server 87.603s.
- Three bounded Luna read-only reviews remained running after repeated finish
  requests and returned no report; they were closed without substitution. Main
  terminal Legacy/source/diff review found no remaining in-scope issue.

## Exact recovery sequence

1. Verify both roots independently: control/diff/status/six `.cs` gates and the
   stated hashes. Read only the named matrix anchors.
2. Commit the four Legacy control/evidence files. This handoff records Legacy
   HEAD immediately before that one expected documentation commit delta.
3. Resume Revelation with read-only Legacy/Go tracing, search the archive only
   for Revelation/RevTime/ObjectHealth/Expire/byte-wrap keywords, freeze exact
   Go files, then implement and run its leaf gate.

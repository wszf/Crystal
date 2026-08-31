# Crystal Go migration current handoff

Last updated: 2026-08-31 15:37 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- P6 remains scope-frozen at nineteen children: eighteen Complete and
  `ITEM-P6-USE-CATALOG-001` is the sole Active unfinished child.
- `WS-ITEM-P6-USE-NOOP-CONSUME-001` is Complete in Go `84adba5`; this closes one
  bounded workstream only, not the active leaf, P6 or the persistent Goal.
- Primary dependency-ready leaf remains `ITEM-P6-USE-CATALOG-001`, matrix row 3546
  and P6 summary row 965.
- Active workstream is `WS-ITEM-P6-USE-POTION-BUFF-003-001`: only Legacy Potion
  Shape 3's timed item-stat Buff branch plus the shared successful consume tail.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `c4669e63` (`Record item expiry migration`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `84adba5` (`Migrate no-effect item use tail`), not pushed.
- Worktree and index are clean after the feature/matrix commit.

## Completed no-effect Item Use evidence

- The production `ClientUseItem` dispatcher now owns exactly four Legacy successful
  no-effect partitions: Potion shapes outside 0-5, Scroll shapes outside 0-15,
  Pets shapes 29+ and every SiegeAmmo shape.
- Shared `CanUseItem` admission, the non-Scroll/Potion riding gate and the dead-player
  gate run before the no-effect tail. Mounted Pets/SiegeAmmo therefore fail without
  consumption; they succeed after dismount. Potion/Scroll retain the riding exception.
- The latest auth `CharacterItemMutation` decrements one stack unit or clears the
  Inventory slot, advances the shared item revision, projects the result to world
  and session state, persists before response, emits one Player `Item Lost` report
  and returns `UseItem=true` without an effect packet.
- No-effect and ordinary basic-Potion commits refresh only current bag weight. They
  preserve `ItemExpiryStatsDeferred`, MaxHP/MP, combat stats and appearance until an
  explicit Equipment/stat refresh, matching Legacy's `RefreshBagWeight` tail.
- A JSON rename that committed account state but failed its later checkpoint remains
  a successful item mutation: the server logs the checkpoint error, reports the item
  loss and returns success rather than allowing a reconnect retry to consume twice.
- Item Expiry unlock protection now releases only when current auth and the detached
  world snapshot contain the same unlocked item in the same player grid/slot. Storage
  absence retains protection; TakeBack convergence releases it; later ordinary
  consumption no longer restores the item.
- Economy commits merge incoming runtime item definitions into current auth metadata
  instead of replacing protected rental/expiry definitions with a stale session list.

## Verification ledger

Passed for Go `84adba5` before commit:

- focused no-effect, Item Expiry, protected Inventory, economy catalogue, potion
  refresh and player item-authority tests at count 20;
- authenticated no-effect session repetition at count 5 and focused race at count 5;
- adjacent UseItem/Potion/Market/Auction/GameShop/Rental/Storage/Expiry tests at
  count 20;
- full `go test ./... -count=1` with only the known date-sensitive
  `TestQuestP7ProgressQuirksSessionClassZeroNameCountAndRelogin` fixture skipped;
- full race with that Quest fixture plus the two unrelated isolated-green fixtures
  `TestProductionTickerInitializesLightFromUTCOnNonUTCHost` and
  `TestSessionHallucinationTranscript` skipped;
- `go vet ./...`, `go build ./...`, formatting, `git diff --check`, commit diff check
  and independent final review with no finding;
- tracked/staged/untracked `.cs` audits in both repositories returned no paths.

The first full-race run exposed a real race introduced by the initial unconditional
unlock acknowledgement: it copied mutable session `gameCharacter` while a world
notification appended a runtime item definition. The final code acknowledges against
the already-detached world snapshot captured under `world.mu`. The isolated death
session race test then passed at count 5 and the complete race gate passed.

The known Quest P7 fixture remains date-sensitive: its fixed 2026-08-29 ground-item
clock is older than the real 2026-08-31 process clock. It remains the only non-race
full-suite skip and was not hidden.

## Active leaf and protected work

- Active leaf: `ITEM-P6-USE-CATALOG-001`.
- Completed workstream: `WS-ITEM-P6-USE-NOOP-CONSUME-001`, Go `84adba5`.
- Active workstream: `WS-ITEM-P6-USE-POTION-BUFF-003-001`.
- It owns only Potion Shape 3's timed item-stat Buff effect and common consume tail.
  Freeze exact stat mapping, duration, replacement/stacking and packet order from
  read-only Legacy evidence before implementation.
- Remaining catalogue scope stays exactly matrix row 3546. Do not reopen accepted
  Scroll 8/9, Scroll 10/13-15, Food 0/1, Pets 0-28, MonsterSpawn 1, SealedHero or
  any completed feature lifecycle.
- Reuse common admission, item revision/CAS, auth/world/session projection, Player
  report logging and persistence. Do not create a parallel item authority.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3546 plus the active index and this handoff.
3. Freeze Potion Shape 3's exact stat-to-Buff mapping, duration, replacement/stacking
   and observable packet order from read-only Legacy C# evidence.
4. Reuse current world Buff lifecycle and latest-auth item mutation; do not introduce
   another timer, player-state or persistence authority.
5. Implement through authenticated `ClientUseItem`, verify effect/consume packet order,
   admission, expiry, persistence/relogin, repeated and race behavior.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

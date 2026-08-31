# Crystal Go migration current handoff

Last updated: 2026-08-31 09:04 (Asia/Singapore)

This replace-in-place file is the current evidence snapshot; historical summaries
are not migration evidence.

## Goal and control-plane state

- Goal `01a02fde-6d48-7613-8545-015d3628e9f0` remains ongoing; it is neither
  Complete nor Blocked.
- `WS-CAPACITY-P6-INVENTORY-001` is Complete in Go `e942217`; this closes one
  bounded workstream only, not its Capacity parent, P6 or the persistent Goal.
- Primary dependency-ready leaf remains `CAPACITY-P6-GRIDS-001`, matrix row 3541
  and P6 summary row 965. P6 remains frozen at nineteen children: sixteen Complete,
  one Active and two Ready.
- Active workstream is `WS-CAPACITY-P6-STORAGE-001`: migrate ordinary
  `@ADDSTORAGE` allocation, extension, expiry and extra-slot behavior.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`.
- Branch: `migration/goal-orchestration`.
- HEAD before this evidence update: `3be5a5b9` (`Record death drop parity correction`).
- `tasks/lessons.md` is the user's pre-existing tracked modification. Preserve it;
  do not reset, overwrite, stage or commit it.
- This evidence update owns only `tasks/migration-active.md` and this handoff.
- Every Legacy `.cs` file is permanently read-only.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`.
- Branch: `migrate/drop-owner-p12`.
- HEAD: `e942217` (`Migrate inventory capacity command`), not pushed.
- Worktree and index are clean after the feature/matrix commit.

## Completed Inventory-capacity workstream evidence

- The ordinary game Chat route accepts case-insensitive `@ADDINVENTORY`, ignores extra
  arguments and has no GM, map, safe-zone or death gate.
- One auth-locked transaction computes cost from the latest Inventory length, checks
  and deducts account Gold, preserves every item, resizes the authoritative grid and
  advances the shared item revision. Concurrent calls cannot double-spend one balance.
- Standard progression is 46 -> 54 for 1,000,000, then +4 through 86 for 3,000,000 to
  10,000,000. An already-86 grid still deducts 11,000,000 and reports size 86.
- Success persists before output and sends `LoseGold -> ResizeInventory(238) -> System
  Chat`; LowGold sends only System Chat with no Gold/grid/revision change. A save error
  closes the session without any success packet while retaining the in-memory commit.
- World Inventory, baseline, revision and Gold projections converge with auth. JSON
  and version-117 checkpoint/reload preserve size 86, items and balance; an
  authenticated restarted session observes the same LowGold-at-cap state.
- Legacy-observable LoseGold, ResizeInventory and System Chat packets are forwarded to
  active observers in source order; LowGold System Chat is observable too.

## Verification ledger

Passed for Go `e942217` before commit:

- protocol/auth/session capacity tests at count 20;
- focused auth/session race tests at count 5;
- all size/cost, dead-player, case, extra-argument, cap, LowGold, observer, save-failure,
  JSON and 117 restart paths;
- full `go test ./... -count=1` with only the exact known Quest P7 fixture skipped;
- full `go test -race ./... -count=1` with the same exact fixture skipped;
- `go vet ./...`, `go build ./...`, formatting and `git diff --check`;
- tracked/staged/untracked `.cs` audits in both repositories.

Independent review found one real observer-forwarding omission. The whitelist and
source-success forwarding were corrected, authenticated observer regressions pass, and
final parity plus transaction reviews returned no remaining high-confidence finding.

## Active leaf and protected work

- Active leaf: `CAPACITY-P6-GRIDS-001`.
- Active workstream: `WS-CAPACITY-P6-STORAGE-001`.
- Trace only Legacy `@ADDSTORAGE` parsing, account storage flags/deadline and directly
  reached extra-slot access, response and persistence helpers; C# stays read-only.
- Reuse existing account Gold, Storage grid, auth item authority, bootstrap and
  checkpoint persistence. Do not create parallel balance, deadline or capacity state.
- Freeze first 80-to-160 allocation, first and repeated costs, ten-day extension,
  deadline arithmetic, strict expiry equality, packet/text order and observer output.
- Do not reopen `@ADDINVENTORY` or enter item-use, unrelated P10 economy, protocol
  layout or other phase behavior.

## Exact recovery sequence

1. Verify both repositories independently; preserve `tasks/lessons.md` and rerun
   tracked/staged/untracked `.cs` audits.
2. Read only matrix rows 965 and 3541 plus the active index and this handoff.
3. Trace Legacy `@ADDSTORAGE` command admission, first allocation, repeated extension,
   costs, deadline arithmetic and before/equal/after-expiry access; every `.cs` remains
   permanently read-only.
4. Freeze the smallest command/auth/session adapter around the existing Gold, Storage,
   account flag/deadline and item authorities; do not reopen `@ADDINVENTORY`.
5. Verify all first/repeated/low-gold and strict-expiry boundaries, authenticated and
   observer packet order, persisted relogin/restart, repeated and race behavior.
6. Update matrix/index/handoff, create Go feature and Legacy evidence commits, then
   continue the persistent Goal without push or merge.

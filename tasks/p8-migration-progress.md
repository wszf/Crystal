# P8 mount and ordinary-pet migration handoff

Last updated: 2026-08-13 (Asia/Singapore)

## Status

The P8 mount and ordinary combat-pet batch is complete and ready to pause.
Hero is intentionally not part of this batch and is the next independent P8
feature line.

## Completed scope

- Mount equipment and attachments, inventory/storage attachment transactions,
  `RemoveSlotItem`, `@ride`, `MountUpdate`, map/saddle gates, mounted action and
  combat restrictions, riding stats, loyalty loss/recovery, Strong/Ribbon,
  food, visibility, and JSON persistence.
- Ordinary-pet account/JSON bridge, `PetMode`, login restore/logout capture,
  MonsterSpawn items, five-pet limit, owner/group health visibility, following,
  target search and Focus mode, monster/player combat, friendly-owner gates,
  cross-map recall, `NoPets`, owner-death cleanup, delayed removal, and no
  ordinary respawn/drop/quest credit.
- Pet growth and persistence details: saved HP clamping after level stats,
  HP/AC/MAC/DC growth, speed floor, special-pet triple experience, MaxPetLevel,
  PetSave class/name filtering, remaining TameTime, zero/negative Wizard tame
  expiry, and base-name `ObjectName` broadcast.
- Go-only protocol vectors, deterministic P8 transcript, authenticated probe,
  session tests, world schema tests, and cross-map companion entry rules.

## Verification

The final handoff must retain the exact successful commands and commit hashes
below. These placeholders are replaced during final commit:

- Targeted protocol/server P8 tests: passed.
- `cmd/crystal-server` complete package tests: passed.
- Full `go test ./...`, `go test -race ./...`, `go vet ./...`, `go build
  ./...`, diff checks, and both repositories' tracked/staged/untracked C#
  read-only gates: passed.
- Go migration commit: `0cf4a44 feat(p8): migrate mounts and ordinary pets`.
- Original repository documentation commit: this file is part of
  `docs: record p8 migration completion` (resolve its hash with `git log -1`).

## Resume point

Start the next batch with Hero runtime/inventory/AI/combat/persistence. Read
`AGENTS.md` and `tasks/lessons.md` first, keep every `.cs` file read-only, and
use the committed Go server as the implementation baseline. Do not reopen the
mount/ordinary-pet batch unless a regression test fails.

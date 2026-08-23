# Crystal Go migration current handoff

Last updated: 2026-08-24 02:43 (Asia/Singapore)

This is the replace-in-place recovery snapshot after the P1 config authority was
expanded narrowly to include the production config-path selector discovered by
startup tracing. No production code has been changed yet.

## Goal and control-plane state

The persistent full Go migration Goal remains active, not Complete or externally
Blocked. Main authority is `gpt-5.6-sol/ultra`; bounded workers use
`luna_worker` (`gpt-5.6-luna/max`). P1 remains scope-frozen with this one active
leaf and the finite child registry in `tasks/migration-active.md`.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch before the enclosing documentation commit:
  `master...origin/master [ahead 419]`.
- Observed HEAD: `ec13ecdf52a1fda1a7e37281afd2d0af4bdb7cc0`
  (`docs(migration): normalize P1 recovery point`).
- Expected enclosing commit subject: `docs(migration): expand P1 config startup
  authority`. Recovery may observe exactly this one documentation commit delta
  and a clean worktree.
- Staged and untracked files: none.
- Tracked modifications are exactly `tasks/lessons.md`,
  `tasks/migration-active.md`, and `tasks/migration-handoff.md`.
- `AGENTS.md` and tracked `agents.md` remain one hard-linked inode.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`.
- HEAD: `b5e5999c743f3ae7183f3a51eb5fd50ec4fb3536`
  (`docs(migration): activate P1 config leaf`).
- Worktree, staged files, and untracked files are clean.
- The targeted matrix row is `Active` and now names `internal/config` plus the
  bounded production config-path selector; no Go source changed in that commit.

## Active leaf and protected work

- Active leaf: `CFG-P1-CONTRACT-001`.
- Outcome: match P1-owned Legacy Setup.ini/default/error/version-file behavior
  without absorbing feature-specific P2-P12 settings.
- Matrix anchors: `### 2026-08-24 P1 finite closure inventory`, the active leaf
  row, and the row beginning `| P1 |`.
- Legacy read authority: `Shared/Functions/IniReader.cs`, General/Network
  declarations and `Load`/`Save`/`LoadVersion` consumers in
  `Server/Settings.cs`, and bounded direct startup consumers.
- Go owned files: `internal/config/config.go`,
  `internal/config/config_test.go`, new
  `internal/config/p1_contract_test.go`, bounded config-path selection only in
  `cmd/crystal-server/main.go`, and new
  `cmd/crystal-server/p1_config_startup_test.go`.
- Protected Legacy files until the enclosing documentation commit are the three
  modified control files above. No Go source file is modified yet.
- Forbidden scope remains localization catalog, admission implementation,
  logging, HTTP/status implementation, lifecycle restructuring, feature-specific
  settings, broad matrix edits, and every C# change.

## Verification ledger

- Startup authority was traced to `Server.MirForms/Program.cs` calling
  `Settings.Load()` inside the process-level exception boundary; `Settings.Load`
  invokes `LoadVersion()` after Setup.ini reads and path creation.
- The Legacy reader is exact-case/first-match and writes defaults on missing or
  invalid typed values while swallowing file read/write errors; current Go is
  case-insensitive/last-map-write, fail-fast, and does not select the default
  production Setup.ini path. These are implementation gaps, not yet rulings.
- The first over-qualified startup `rg` returned 1 under `set -e`; that entire
  call was discarded and rerun with explicit 0/1 handling. C02 records it.
- The targeted archive search found only the existing schema/default fixture
  rules; no feature-specific historical instruction changes this leaf.
- Both repositories were clean and all three `.cs` gates empty immediately
  before these control edits. No functional test has run for this leaf.

## Quiescence

- Read-only `luna_worker` `01a02fe2-9b51-7233-9ed6-886ccd7925c0` is actively
  deriving the bounded Legacy field/parser/version/startup ledger and has no
  write authority.
- No writing subagent exists and no production Go file is modified.
- The most recent exact process audit found no `go` or `crystal-server` process.

## Exact recovery sequence

1. Verify the expected Legacy documentation commit and clean status in a
   Legacy-only call; verify the Go HEAD and clean status separately.
2. Collect the active read-only worker's concise evidence and close it.
3. Finalize the P1 field/phase routing ledger and implement only the registered
   Go files, including default production selection of `Configs/Setup.ini` while
   preserving explicit `CRYSTAL_CONFIG` overrides.
4. Run the config/startup compile, focused, repeated, and applicable race gates;
   update matrix/index/handoff and commit only owned files.

# Crystal Go migration current handoff

Last updated: 2026-08-25 09:14 (Asia/Singapore)

This replace-in-place snapshot records committed closure of
`BOOT-P4-STARTPOINT-001` and synchronized routing to `MAP-P4-DETAIL-001`.

## Goal and control-plane state

- Goal remains Active and unchanged. It is neither complete nor blocked.
- Main authority remains `gpt-5.6-sol/ultra`. Bounded writer
  `01a03667-3c77-7d90-acbd-f4ed31781f1b` (`luna_worker`,
  `gpt-5.6-luna/max`) exclusively created the new StartPoint test file, returned
  passing focused evidence and was closed. No agent or Go/server process remains.
- P4 is scope-frozen In progress with ten children, four Complete and six
  unfinished. Matrix and Active Index mark BOOT Complete and
  `MAP-P4-DETAIL-001` as the sole Active leaf.

## Legacy repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal`
- Branch: `master`; observed HEAD:
  `c5f16466085ac3d1b758609724b1467f99a4ee01`
  (`docs(migration): route p4 map detail closure`). The eventual handoff-only
  commit is the one expected documentation delta from this observed HEAD.
- Before this refresh the worktree, index and untracked set were empty; this
  handoff is now the sole tracked unstaged file.
- The archive strengthening records one fresh package exit 1 from the established
  30-second Hallucination closed-pipe flake, exact isolation count 10 exit 0 and
  fresh package rerun exit 0. No unrelated production/test change was made.
- Active Index closes BOOT and registers exact nine-file MAP-detail authority.
  Control checker, diff check and all three Legacy C# gates are clean/empty.

## Go repository state

- Root: `/Users/wszf/Dropbox/source_code/git_work/me_work/Crystal.GoServer`
- Branch: `main`; HEAD: `6db6bbb002fe7e74d8b51473409252f5c407455e`
  (`feat(migration): enforce loaded map startpoint startup gate`).
- Worktree, index and untracked set are empty; diff and C# gates are clean.
- Committed production derives the mandatory StartPoint gate only from successfully
  loaded maps, runs it before existing world-map validation, returns the exact
  localized typed error in `EnvirStarted -> error -> EnvirStopped` order and
  never calls game/status openers on rejection. A loaded non-index-zero
  StartPoint permits game then status bind.
- Matrix closes BOOT, activates MAP detail and records four Complete/six
  unfinished. Go diff check and all three Go C# gates are clean/empty.

## Active leaf and protected work

- Active leaf: `MAP-P4-DETAIL-001`.
- Outcome: exact current-map `MapInformation`, complete `NewMapInfo`, one-time
  `WorldMapSetup`, repeat suppression, and loaded-source-order/current-culture
  SearchMap silence/sentinels plus NPC underscore-GameName behavior, consumed by
  the Go production probe.
- Matrix anchors: exact MAP-detail/P4 rows and named map-detail/SearchMap/probe
  evidence. Legacy authority is bounded PlayerObject StartGame/RequestMapInfo/
  SearchMap, MirConnection dispatch, MapInfo/NPCInfo ClientInfo/GameName,
  serializers and client consumers.
- Exact Go authority is `cmd/crystal-server/{main.go,main_test.go,world.go,
  world_test.go}`, new `p4_map_detail_session_test.go`,
  `internal/protocol/{packet.go,packet_test.go}`, and
  `internal/probe/{network.go,network_test.go}` plus matrix evidence. Cell load,
  StartPoint, collision/light/visibility/chat and P5-P12 behavior are forbidden.

## Verification ledger

Final BOOT candidate commands and results:

- touched compile: exit 0;
- focused seven production/fixture tests: `-count=1` and `-count=20`, exit 0;
- focused race seven tests `-count=5`: exit 0;
- first fresh `go test ./cmd/crystal-server -count=1`: exit 1 only at known
  `TestSessionHallucinationTranscript` after 30 seconds, outside owned files;
- exact Hallucination isolation `-count=10 -timeout=10m`: exit 0;
- fresh unexcluded package rerun: exit 0 in 75.352s;
- fresh unexcluded `go test ./... -count=1`: exit 0, server 80.143s;
- `go vet ./...` and `go build ./...`: exit 0;
- worker compile/focused/repeated/race evidence also exited 0. Full repository
  race is not due; startup changes have current focused race and integration.

## Exact recovery sequence

1. Verify both repositories clean and all three Active fields agree; the actual
   Legacy HEAD may be one handoff-only commit after the observed ID.
2. Begin `MAP-P4-DETAIL-001` under its exact nine-file authority.

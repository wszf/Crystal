# Lessons archive

This directory preserves the historical contents of `tasks/lessons.md` without
loading them into every agent session. The active cross-cutting rules remain in
`tasks/lessons.md`. Do not read this directory wholesale.

## Search workflow

Search by a combination of AI ID, entity/module name, and failure/workflow term.
Resolve the AI ID through the manifest so compound titles such as `AI=52/53`
are found by either ID:

```sh
jq -r 'select(.ai_ids | index(53)) |
  "tasks/lessons-archive/" + .archive_file' \
  tasks/lessons-archive/manifest/2026.jsonl | sort -u
rg -l -i --glob '*.md' --glob '!README.md' \
  'DragonStatue|Sleeping|ticker|value-map' \
  tasks/lessons.md tasks/lessons-archive
rg -n -C 6 -i 'DragonStatue|Sleeping|ticker|value-map' <matched-files>
```

Read only the matching sections. The 831 legacy blocks and their stable
`legacy-NNNNNN` IDs are frozen evidence: never edit or delete those raw blocks.
New feature-specific or historical lessons may be appended to a suitable new
archive Markdown file without modifying the frozen legacy shards or manifest.
If a problem becomes cross-cutting or recurs, strengthen its canonical entry in
the active file instead of duplicating it.


## Active-file size gate

Run this whenever active lessons change:

```sh
active_bytes=$(wc -c < tasks/lessons.md)
active_lines=$(wc -l < tasks/lessons.md)
test "$active_bytes" -le 51200
test "$active_lines" -le 500
```

The recommended limit is 50 KB or 500 lines. Archive feature-specific evidence
before exceeding it instead of deleting historical lessons.

## Integrity

- Archived source SHA-256: `b963696a04e184cb418d76f0fda9b16a9c417f048a8a367b9b105f6a733d6f8b`
- Archived source bytes: `758558`
- Historical lesson blocks: `831`
- Manifest: `manifest/2026.jsonl`
- Source metadata: `schema/source-metadata.json`
- Legacy format exceptions: `schema/legacy-exceptions.json`

Archive files contain only original raw lesson blocks. They have no generated
headers so each manifest block hash remains verifiable.

## Files

| File | Lessons | Raw bytes |
|---|---:|---:|
| `ai/000-024.md` | 7 | 7359 |
| `ai/025-049.md` | 2 | 1750 |
| `ai/075-099.md` | 2 | 2664 |
| `ai/150-174.md` | 5 | 4442 |
| `ai/175-199.md` | 3 | 2740 |
| `ai/200-224.md` | 20 | 15810 |
| `migration/combat-general.md` | 42 | 37852 |
| `migration/data-config-import-export.md` | 21 | 19115 |
| `migration/protocol-session-wire.md` | 36 | 28966 |
| `migration/world-state-lifecycle.md` | 10 | 8990 |
| `misc.md` | 95 | 83738 |
| `verification/build-vet-and-gates.md` | 29 | 24693 |
| `verification/fixtures-and-transcripts-01.md` | 118 | 97702 |
| `verification/fixtures-and-transcripts-02.md` | 113 | 97755 |
| `verification/fixtures-and-transcripts-03.md` | 86 | 81631 |
| `verification/race-and-flake-attribution.md` | 24 | 26115 |
| `workflow/git-working-tree.md` | 6 | 5715 |
| `workflow/repository-boundaries-01.md` | 88 | 96402 |
| `workflow/repository-boundaries-02.md` | 73 | 73012 |
| `workflow/shell-tools-and-patching.md` | 51 | 42013 |

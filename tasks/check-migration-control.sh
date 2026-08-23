#!/bin/sh
set -eu

repo_root=$(git rev-parse --show-toplevel) || {
  printf '%s\n' 'FAIL: cannot resolve repository root' >&2
  exit 1
}
cd "$repo_root"

fail() {
  printf 'FAIL: %s\n' "$1" >&2
  exit 1
}

check_limit() {
  file=$1
  max_lines=$2
  max_bytes=$3
  [ -f "$file" ] || fail "missing $file"
  file_lines=$(wc -l < "$file" | tr -d ' ') || fail "cannot count lines in $file"
  file_bytes=$(wc -c < "$file" | tr -d ' ') || fail "cannot count bytes in $file"
  [ "$file_lines" -le "$max_lines" ] || fail "$file has $file_lines lines (limit $max_lines)"
  [ "$file_bytes" -le "$max_bytes" ] || fail "$file has $file_bytes bytes (limit $max_bytes)"
  printf 'PASS: %-34s %4s lines %6s bytes\n' "$file" "$file_lines" "$file_bytes"
}

check_heading_once() {
  file=$1
  heading=$2
  heading_count=$(awk -v wanted="$heading" '$0 == wanted { count++ } END { print count + 0 }' "$file") || fail "cannot inspect headings in $file"
  [ "$heading_count" -eq 1 ] || fail "$file must contain exactly one heading: $heading"
}

check_section_field_once() {
  file=$1
  section=$2
  prefix=$3
  field_count=$(awk -v wanted_section="$section" -v wanted_prefix="$prefix" '
    $0 == wanted_section { inside = 1; next }
    inside && /^## / { inside = 0 }
    inside && index($0, wanted_prefix) == 1 { count++ }
    END { print count + 0 }
  ' "$file") || fail "cannot inspect $section in $file"
  [ "$field_count" -eq 1 ] || fail "$section in $file must contain exactly one field: $prefix"
}

check_limit agents.md 200 16384
check_limit tasks/goal-task.md 300 32768
check_limit tasks/lessons.md 500 51200
check_limit tasks/migration-active.md 300 32768
check_limit tasks/migration-handoff.md 250 24576

for heading in \
  '# Agent working rules' \
  '## Migration agent models and orchestration' \
  '## Context compaction and rollover' \
  '## Context and verification economy' \
  '## Migration language boundary'
do
  check_heading_once agents.md "$heading"
done

for heading in \
  '# Crystal migration Goal task' \
  '## Control plane and bounded context' \
  '## Finite inventory and progress' \
  '## Agent responsibilities' \
  '## Work-cycle workflow' \
  '## Tiered verification' \
  '### Leaf gate — every functional leaf commit' \
  '### Integration gate — bounded cadence' \
  '### Overall closure gate' \
  '## Current handoff rules' \
  '## Session rollover and compaction' \
  '## Definition of done'
do
  check_heading_once tasks/goal-task.md "$heading"
done

for heading in \
  '# Crystal migration active index' \
  '## Progress semantics' \
  '## Phase routing summary' \
  '## Active batch' \
  '### Protected Go ownership' \
  '### Remaining acceptance work' \
  '## Scope-freeze discovery queue' \
  '## Selection protocol'
do
  check_heading_once tasks/migration-active.md "$heading"
done
check_section_field_once tasks/migration-active.md '## Active batch' '- Leaf ID: `'
check_section_field_once tasks/migration-active.md '## Active batch' '- Status: `Active`'
check_section_field_once tasks/migration-active.md '## Active batch' '- Outcome:'
check_section_field_once tasks/migration-active.md '## Active batch' '- Go matrix anchors to read:'

for heading in \
  '# Crystal Go migration current handoff' \
  '## Goal and control-plane state' \
  '## Legacy repository state' \
  '## Go repository state' \
  '## Active leaf and protected work' \
  '## Verification ledger' \
  '## Exact recovery sequence'
do
  check_heading_once tasks/migration-handoff.md "$heading"
done
check_section_field_once tasks/migration-handoff.md '## Active leaf and protected work' '- Active leaf: `'

if ! awk '
  /^## / {
    heading = tolower($0)
    if (heading ~ /(compact|compaction|rollover|checkpoint|durable boundary|session boundary)/) {
      print FNR ":" $0 > "/dev/stderr"
      bad = 1
    }
  }
  END { exit bad ? 1 : 0 }
' tasks/migration-handoff.md; then
  fail 'active handoff contains a historical compact/rollover/checkpoint section'
fi

if ! awk '
  /[ \t]$/ {
    print FILENAME ":" FNR ": trailing whitespace" > "/dev/stderr"
    bad = 1
  }
  END { exit bad ? 1 : 0 }
' agents.md tasks/goal-task.md tasks/lessons.md tasks/migration-active.md tasks/migration-handoff.md; then
  fail 'control documents contain trailing whitespace or could not be inspected'
fi

cmp -s AGENTS.md agents.md || fail 'AGENTS.md and agents.md content diverged'
git diff --check || fail 'git diff --check failed'
git diff --quiet -- '*.cs' || fail 'tracked C# changes detected or tracked audit failed'
git diff --cached --quiet -- '*.cs' || fail 'staged C# changes detected or staged audit failed'
untracked_cs=$(git ls-files --others --exclude-standard '*.cs') || fail 'untracked C# audit failed'
[ -z "$untracked_cs" ] || {
  printf '%s\n' "$untracked_cs" >&2
  fail 'untracked C# changes detected'
}

printf '%s\n' 'PASS: migration control plane is bounded, structurally complete, and Legacy C# gates are empty'

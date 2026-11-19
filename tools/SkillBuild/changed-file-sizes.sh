#!/usr/bin/env bash
# The changed-file size check (T13, T15, T16) until dydo's own changed-file gate ships: every file the
# branch adds or changes against its base, tests and docs included, stays at or under 20,000 bytes
# (5k tokens, counted as bytes / 4). A suite test cannot see the merge base reliably, so this runs as
# a gate command, from the repository root:
#
#   bash tools/SkillBuild/changed-file-sizes.sh [base]    # base defaults to origin/feature/dydo-4-0-0
#
# It covers committed changes since the merge base, uncommitted ones and untracked files, prints the
# five largest, and exits 1 naming every file over the limit.
set -euo pipefail
base=${1:-origin/feature/dydo-4-0-0}
limit=20000
fork=$(git merge-base "$base" HEAD)
sizes=$( { git diff --name-only --diff-filter=AMRC -z "$fork"; git ls-files --others --exclude-standard -z; } |
  sort -zu | xargs -0 -r wc -c | grep -v ' total$' | sort -rn || true)
count=$(printf '%s\n' "$sizes" | grep -c . || true)
echo "changed or new files against $base (merge base ${fork:0:8}): $count"
echo "five largest:"
printf '%s\n' "$sizes" | head -n 5
over=$(printf '%s\n' "$sizes" | awk -v limit="$limit" '$1 > limit')
if [ -n "$over" ]; then
  echo "over $limit bytes:"
  printf '%s\n' "$over"
  exit 1
fi
echo "all at or under $limit bytes"

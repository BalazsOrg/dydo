---
area: project
type: reference
---

# DYD-1192: retire the legacy source-to-test mapping

Base: `93419274ee513e0c234640b1dd9adfbc37027a3d`, on `feature/dydo-4-0-0`.
The ticket's 2026-11-18 ruling and DR 067 T5 govern this deletion.

The legacy mapping, its validator, the inventory fields it supplied and the shared
adapter's calls are removed. The already-empty host-driver exemption registry and
its consumers go with them. Both exempt drivers and the validator's tests had
already been purged on the base. CI and release use the shared adapter, so their
existing suite and static steps stay. No claims or skills are changed.

## Red and green

After deleting only the data file, a probe uses `gate_adapter._stack_methods` to
select the registered inventory and legacy validation collectors, then runs them
through `gate_run.collect_all`. Its empty inventory deliberately isolates the
retired file read from unrelated build and discovery work. The legacy collector
returns `FileNotFoundError`, exit 2. After removing that collector, the identical
probe returns the remaining inventory collector's pass, exit 0. This is ticket
proof, not a graduating absence test (DR 067's three-smells amendment).

Both runs use the session slice, a 4G cap, no swap and a 60-second timeout.
The recipe and raw outputs are retained at `/tmp/dyd-1192-evidence/probe.py`,
`red.json` and `green.json`. Git archives the removed implementation on the base.

## Verification

The repository-wide search for the removed filename, reader, collector symbols
and inventory fields returns no matches. `dydo check` reports zero errors and
zero warnings in 937 files. Full Python, C# and Node suites and every declared
static gate are required before readiness; their actual results belong in the PR
body. T5's job-tag rule remains the only test-to-code requirement; no replacement
mapping or selection mechanism is introduced. DYTEST owns selection.

The first full suite run began before the user supplied the shared host lock
requirement and is allowed to finish. Subsequent heavy commands use
`/var/tmp/dydo-gate-lock.sh`, with two .NET workers. A temporary launcher under
the evidence directory adds `-m:2` to internal build/test commands and
`RunConfiguration.MaxCpuCount=2` to test commands without editing the runners.

code-writer/model: gpt-6.1-sol

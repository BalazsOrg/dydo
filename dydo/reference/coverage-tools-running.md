---
explains: ["DynaDocs.Tests/coverage/gap_check.py", "DynaDocs.Tests/coverage/gap_check.json", "DynaDocs.Tests/coverage/gate_run.py", "DynaDocs.Tests/coverage/run_tests.py", "Commands/GapCheckCommand.cs"]
area: reference
type: reference
---

# Coverage Tools: Operations and Declared Rows

[Coverage and Assurance Tools](./coverage-tools.md)

## Operations

Run the facade with the pinned local interpreter. On Linux, set
`py=.static-gates/python/bin/python` and invoke `"$py"` in place of
PowerShell's `& $py` below:

```powershell
$py = ".static-gates/python/Scripts/python.exe"
& $py DynaDocs.Tests/coverage/gap_check.py --help
& $py DynaDocs.Tests/coverage/gap_check.py all
& $py DynaDocs.Tests/coverage/gap_check.py test --stack dotnet -- --filter FullyQualifiedName~ParserTests
& $py DynaDocs.Tests/coverage/gap_check.py gate static
& $py DynaDocs.Tests/coverage/gap_check.py gate coverage --stack python,node
& $py DynaDocs.Tests/coverage/gap_check.py gate mutation --since BASE --stack dotnet
& $py DynaDocs.Tests/coverage/gap_check.py capabilities
& $py DynaDocs.Tests/coverage/gap_check.py --force-run
```

`all` runs test rows only. Each `gate` operation runs exactly the named capability, defaulting to
every declared stack in manifest order. `--force-run` is the compatibility full-G operation: it
selects test, static and coverage for every stack - fifteen rows here - and never selects mutation.
Where a stack's coverage row declares `suiteVerdict`, `--force-run` derives that stack's test row
from the coverage row's single instrumented run instead of launching the suite a second time.

Rows run one at a time, stack by stack and capability by capability. Each row gets a fresh deadline
of 1800 seconds of execution plus 30 seconds of cleanup. Windows `python` coverage alone gets
2700 seconds of execution: hosted green runs reached 1783 seconds and repeated runs exhausted the
old budget (DYD-1157). The 50% margin over the exhausted 1800-second budget remains finite; a hung
adapter still interrupts its row and prevents later dispatch. The facade exports `DYDO_ROW_DEADLINE`
and `PYTHON` (the caller's own interpreter) to the child. An interrupted row stops every row after
it.

The facade runs argv arrays without a shell. It appends native arguments only after `test ... --`;
a gate row forwards nothing, and the coverage gate rejects any narrowing of the suite. Executable
paths in an argv row resolve from that stack's working directory without changing the recorded
vector; bare executable names use the platform search rules.

Every started operation writes `<artifactRoot>/run-<epoch-milliseconds>-<8 hex>/result.json` and
prints its path. It records schema, candidate commit and dirty state, the operation's memory cap,
operation, selected stacks, ordered rows and aggregate exit. `DynaDocs.Tests/coverage/.gitignore`
ignores `results/`, so every artifact named on this page is local evidence, never a committed file.

### Memory cap

Every operation that runs children (`all`, `test`, `gate` and `--force-run`, but not `--help` or
`capabilities`) caps its own memory once, before it starts its first child. Rows run one at a
time, so that one cap covers every child. The cap is 8G; Linux also disables swap. `DYDO_MEMORY_MAX`, in
systemd size syntax such as `256M`, `1.5G` or a byte count, may lower it. A higher or malformed
value exits 2 before any child runs; a raised one says "a cap hit is a finding; raising the cap is
not allowed". `run_tests.py` imports the same cap from `gap_check.py` and applies it when it runs
directly; on Linux it inherits the facade's, and on Windows it creates its own nested job.

On Linux the facade reads its cgroup v2 ancestry from `/proc/self/cgroup` and `/sys/fs/cgroup`:

| `memoryCap.state` | When | What the facade does |
|---|---|---|
| `inherited` | the tightest ancestor `memory.max` is at or below the target, that ancestor has `memory.swap.max` 0, and `systemctl --user show -p OOMPolicy` reports `continue` for its unit | runs in place; a nested facade run inside a capped operation takes this path |
| `enforced` | otherwise | probes `systemd-run … true`, then reruns the same command line once with `systemd-run --user --scope --slice=<caller slice> --quiet -p MemoryMax=<limit> -p MemorySwapMax=0 -p OOMPolicy=continue --expand-environment=no`; the limit is the lower of the target and the tightest ancestor `memory.max`, each run is capped at its own limit, and concurrent runs share the caller's slice budget |
| `unavailable` | no `systemd-run` on `PATH`, a failed probe, or a rerun whose scope shows no cap | runs uncapped and prints one `memory cap unavailable: <reason>` line to stderr |

The enforced scope joins the caller's nearest ancestor `.slice` within the user manager, found
in `/proc/self/cgroup`; `memoryCap.reason` names that slice. A caller directly in `app.slice`, or
without a user-manager slice, keeps the default `app.slice` placement. The inherited path stays
in place. An outer scope's tighter limit still bounds each rerun, but only the shared slice budget
bounds concurrent reruns together.

For ad-hoc runs where systemd exists use
`[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>`.
Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager
(on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).
Leave out `--slice` where no agent slice exists. The bracketed env prefix applies to .NET commands;
omit the brackets when using it. Probes use 4G and a short timeout. Keep any host gate lock outside
the cap, before the command.

The facade sets `MSBUILDDISABLENODEREUSE=1`, `DOTNET_CLI_USE_MSBUILD_SERVER=0` and
`UseSharedCompilation=false` in its children's environment wherever the caller left them unset.
Caller values, including empty strings, stay unchanged. Direct `run_tests.py` uses the same defaults
through the shared cap, preventing .NET build and compiler servers from outliving the run by default.

The rerun keeps the process, its stdio, exit code and signals, and argv bytes, `$(` included.
`OOMPolicy=continue` lets the facade outlive a child that the kernel kills at the cap, so it can
record the hit. An unknown `OOMPolicy` counts as `stop`, and such an outer scope gets a scope of its
own. Row deadlines remain enforced, including Windows Python coverage's 2700-second budget, and
`run_tests.py` keeps its 1800-second limit for the C# campaign.

On Windows the facade always creates its own nested Job object and assigns itself before running
children. `JOB_OBJECT_LIMIT_JOB_MEMORY` limits committed memory for the job, including descendants.
The effective recorded limit is the lower of the target and the outer job's job-memory limit, if
present. Windows applies nested limits. The facade stays in place and adds no kill-on-close flag;
`run_tests.py`'s campaign job nests beneath it. Windows records `enforced`, never `inherited`, with
mechanism `windows job object` and a reason naming the limit. A setup or query failure records
`unavailable` with the Windows error, prints one warning, and lets the operation proceed.

The facade associates a private completion port with its job. Only a delivered
`JOB_OBJECT_MSG_JOB_MEMORY_LIMIT` during a row sets `memoryCapHit: true`, fails that row with exit 1,
and prints the same finding text as Linux below. Delivery is **not guaranteed**: a missing message
proves nothing. A missing report never turns a failed child into a pass. Each Windows row records
`peakJobMemory`, the cumulative job peak (`PeakJobMemoryUsed`) sampled at row end, or `null` when
unavailable. This is diagnosis only: neither a peak nor an exit code proves a hit. macOS remains
`unavailable` with its reason and warning.

`memoryCap` records `state`, `limit` in bytes (`null` when unavailable), `mechanism` and `reason`.
On Linux and Windows it and every covered row also record `memoryCapHitReporting`: `exact` on Linux
and `best-effort` on Windows.
On Linux, around each row the facade reads `oom_kill` from the capping cgroup's `memory.events`. When the
count rises, the row is a cap hit: `failed`, exit 1, `memoryCapHit: true`, with the reason
`memory cap hit (configured scope limit MemoryMax=<cap>; an enclosing slice budget may have triggered the kill): a finding; do not raise the cap`, whatever the child returned.
The configured scope limit does not identify the exhausted budget: an enclosing slice can trigger
the kill even when the scope records no `max` or `oom` events. The diagnostic does not identify
which ancestor, if any, triggered it. Windows Job-object hits retain their existing reason,
`memory cap hit (MemoryMax=<cap>): a finding; do not raise the cap`.
The same line goes to stderr. A test row derived from a cap-hit coverage row fails the same way. A
cap hit is a finding to diagnose, never a reason to raise the cap. When the kernel kills the facade
itself, no result is written and the caller sees 137.

`capabilities` applies the same stack, isolation, command and path validation without running
children or creating result artifacts. It reports invalid entries alongside valid peers and returns
2 for malformed configuration; a valid manifest returns 0 even where capabilities are declared
unavailable. Inspection needs no mutation comparison base.

---

## Declared rows

`kind: current-python` prefixes the argv with the interpreter that is running the facade, so the
caller's identity propagates into every adapter.

| Stack | Capability | Command | Required artifact | Suite verdict |
|---|---|---|---|---|
| `dotnet` | test | `<python> -u DynaDocs.Tests/coverage/run_tests.py --` | none | - |
| `dotnet` | static | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack dotnet --gate static` | `results/adapters/dotnet-static.json` | - |
| `dotnet` | coverage | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack dotnet --gate coverage` | `results/adapters/dotnet-coverage.json` | `collectors.csharp-coverage.facts.child_exit`; `collectors.csharp-coverage.findings[gate=functional]` |
| `python` | test | `<python> -m unittest discover -s DynaDocs.Tests/coverage/tests -p test_*.py` | none | - |
| `python` | static | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack python --gate static` | `results/adapters/python-static.json` | - |
| `python` | coverage | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack python --gate coverage` | `results/adapters/python-coverage.json` | `collectors.python-coverage.facts.child_exit`; `collectors.python-coverage.findings[gate=functional]` |
| `node` | test | `node DynaDocs.Tests/coverage/node_tests.cjs` | none | - |
| `node` | static | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack node --gate static` | `results/adapters/node-static.json` | - |
| `node` | coverage | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack node --gate coverage` | `results/adapters/node-coverage.json` | `collectors.javascript-coverage.facts.child_exit`; `collectors.javascript-coverage.findings[gate=functional]` |
| `viewer` | test | `pnpm -C viewer run test` | none | - |
| `viewer` | static | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack viewer --gate static` | `results/adapters/viewer-static.json` | - |
| `viewer` | coverage | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack viewer --gate coverage` | `results/adapters/viewer-coverage.json` | `collectors.typescript-coverage.facts.suite_exit`; `collectors.typescript-coverage.findings[gate=functional]` |
| `statechart` | test | `pnpm -C skills/engineering/statechart/scripts run test` | none | - |
| `statechart` | static | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack statechart --gate static` | `results/adapters/statechart-static.json` | - |
| `statechart` | coverage | `<python> DynaDocs.Tests/coverage/gate_adapter.py --stack statechart --gate coverage` | `results/adapters/statechart-coverage.json` | `collectors.typescript-coverage.facts.suite_exit`; `collectors.typescript-coverage.findings[gate=functional]` |
| `dotnet`, `python`, `node` | mutation | unavailable, reason `Pending DYD-103` | none | - |
| `viewer`, `statechart` | mutation | unavailable: no TypeScript mutation mechanism is adopted | none | - |

Artifact paths are shown relative to `DynaDocs.Tests/coverage/`; the manifest declares them
repository-relative, and the artifact root is `DynaDocs.Tests/coverage/results`. The `dotnet` stack declares isolation
`git-worktree-copy-working-changes`, verified by the adapter `DynaDocs.Tests/coverage/run_tests.py`;
`python`, `node`, `viewer` and `statechart` declare verified `in-place` isolation. The `viewer`
and `statechart` coverage rows write their LCOV and suite reports under their own run directory, not
under the ignored `viewer/coverage/` or the skill folder. The statechart rows run the package's
tests with whatever managed prerequisites the caller's environment names (`TLA2TOOLS`, `JAVA`,
`STATECHART_PELE`); a test that lacks one skips by name, and its code then counts as uncovered.
The release workflow's Windows `validation` job downloads the TLC jar, the win-x64 JRE and Pele
from their pinned sources in `Resources/Statechart/dependency-lock.json`, refuses any that does not
match its pinned hash, and names them to the `--force-run` step.
Statechart coverage partitions points by callable: the Windows leg measures only
the pinned native callables in `statechart-platforms.json`, and Linux measures every remainder.
That provisioning is the shared `.github/workflows/provision-statechart.ps1`, which selects
the locked linux-x64 JRE on Linux and win-x64 JRE on Windows, and which the CI
`statechart-windows` job also calls. On every pull request and every push to `master`, `main` or `feature/**` the job runs
`skills/engineering/statechart/scripts/test/windows.test.ts` on `windows-latest`, and
`.github/workflows/assert-node-tests-ran.ps1 -MinimumTests 6` fails it on a missing report, a skip
or todo, a failure, or fewer than 6 tests.
The runner also materializes the
gitignored `.claude/skills` and `.agents/skills` host discovery directories into the snapshot, so
the canonical-tree guard's link-resolution check is not vacuous there. The runner marks its
snapshot with a schema-versioned marker before running anything in it and removes the snapshot on
every exit path, including a failing suite, an exception and an interrupt; at startup it also
prunes any marked `dydo-test-*` snapshot older than a configurable age.

Every G row therefore has a real mechanism: five test adapters, five static adapters and five
coverage adapters. Mutation is the only unavailable capability. On `dotnet`, `python` and `node` it
belongs to DYD-103, whose policy is already settled and does not wait on the adapter: DynaDocs
requires no surviving or uncovered changed-code mutants, as the [Testing
Strategy](../guides/testing-strategy.md) states it. Nothing here measures that today - those three
stacks' mutation rows are `unavailable` with the reason `Pending DYD-103`, so the gate cannot run,
and cannot pass, until that ticket lands a reviewed mechanism. The `viewer` and `statechart`
mutation rows are `unavailable` for a different reason: no TypeScript mutation mechanism is
adopted, and no ticket currently tracks that gap. Whether a given candidate passes is what its result artifact says; no
document stands in for a run.

---

## Related

- [Coverage and Assurance Tools](./coverage-tools.md)

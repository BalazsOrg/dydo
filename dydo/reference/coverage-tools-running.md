---
area: reference
type: reference
explains: ["DynaDocs.Tests/coverage/**", "Commands/GapCheckCommand.cs"]
---

# Assurance Tools: Provisioning and Execution

[Overview](./coverage-tools.md).

## Provisioning

`.static-gates/` is git-ignored and no gate installs anything, so every machine creates its own
measurement environment before any operation below can run. `.github/workflows/release.yml` runs
these same steps before it invokes the facade. They are grouped here by toolchain rather than given
in the workflow's order, because the three toolchains are independent of each other: the only step
that must precede another is creating the virtual environment its Python packages then install into.

```powershell
# the Python assurance toolchain; create the environment with CPython 3.14.4 itself,
# the patch DynaDocs.Tests/coverage/.python-version declares
python -m venv .static-gates/python
& .static-gates/python/Scripts/python.exe -m pip install -r DynaDocs.Tests/coverage/requirements.lock
# the Node assurance toolchain, from DynaDocs.Tests/coverage's pnpm lock
pnpm -C DynaDocs.Tests/coverage install --frozen-lockfile
# the viewer's own toolchain, which the viewer rows call through its pnpm scripts
pnpm -C viewer install --frozen-lockfile
# the statechart runtime's locked test dependencies, for the statechart rows
pnpm -C skills/engineering/statechart/scripts install --frozen-lockfile
# AltCover and the metrics closure
dotnet tool restore --tool-manifest .config/dotnet-tools.json
dotnet restore DynaDocs.Tests/coverage/metrics/GateMetrics.csproj --locked-mode
```

The interpreter that creates the environment must be CPython 3.14.4, as declared in
`.python-version`. The callable witness joins compiled code objects to parsed bodies using
`co_positions()`, so use the declared patch and remove coverage-tool `__pycache__` directories
when changing interpreters. The versions collector verifies the declaration and CPython series;
it records the observed patch separately. Earlier 3.12 patch mismatches caused callable-position
join failures; that historical evidence does not establish compatibility with a different 3.14 patch.

On Linux, use the same locked packages with the POSIX venv path:

```bash
python3.14 -m venv .static-gates/python
.static-gates/python/bin/python -m pip install -r DynaDocs.Tests/coverage/requirements.lock
pnpm -C DynaDocs.Tests/coverage install --frozen-lockfile
pnpm -C viewer install --frozen-lockfile
pnpm -C skills/engineering/statechart/scripts install --frozen-lockfile
dotnet tool restore --tool-manifest .config/dotnet-tools.json
dotnet restore DynaDocs.Tests/coverage/metrics/GateMetrics.csproj --locked-mode
```

The Python environment lives at the repository root, in `.static-gates/python`, outside
`dydo/_system/**`. An agent provisions it without user action, and the guard still refuses every
write into `dydo/_system/**`. This repository's `dydo.json` carries the install nudge that
`dydo init` writes by default, and the POSIX `bin/python -m pip install` form meets it: the guard refuses
the first try once and passes the identical retry when the user or the ticket being executed approved
that install. That nudge's pattern does not match the Windows `Scripts/python.exe -m pip install` form.
Instead of installing, a POSIX checkout may link a matching prepared environment, one created by
CPython 3.14.4 from the same `requirements.lock`:

```bash
mkdir -p .static-gates && ln -s /absolute/path/to/prepared/.static-gates/python .static-gates/python
```

`DynaDocs.Tests/Integration/GuardStaticGateProvisioningTests.cs` holds the guard to this route.

The SDK pin is 10.0.303. `DynaDocs.csproj` separately requires runtime 10.0.14 or
later and resolves its AOT compiler at 10.0.14, the first patch fixing CVE-2026-111497.
An SDK 10.0.303 installation with only shared runtime 10.0.11 can build but cannot
launch framework-dependent dydo or the dydo children used by tests. Check
`dotnet --list-runtimes` before these gates; package restore does not install the
shared runtime. Provisioning a newer runtime requires the host owner's authorization.
CI installs SDK 10.0.303 into a job-local directory, then runs
`.github/workflows/provision-dotnet-runtime.ps1` to add only Microsoft.NETCore.App
10.0.14 with Microsoft's installer. It preserves the SDK/host files and verifies
both the selected SDK and the installed shared runtime before any .NET consumer.
The script selects the Windows or Unix installer for the runner's host architecture;
the release matrix's cross-compilation RID does not select the host runtime.

### Distribution runtime requirements

`PackAsTool` packages a framework-dependent CLI. The NuGet/.NET tool and direct
`dotnet dydo.dll` launches (including CLI children of tests) require
Microsoft.NETCore.App 10.0.14 or later in a compatible 10.x version. The generated
runtimeconfig retains `LatestMinor` roll-forward; it does not permit an earlier
patch or a different major version. A host with only 10.0.0–10.0.13 cannot launch it.
The SDK version alone does not establish this runtime prerequisite.

The release workflow publishes self-contained NativeAOT archives for win-x64,
linux-x64, linux-arm64, osx-x64 and osx-arm64. Both direct GitHub-release downloads
and npm's platform downloader use these same binaries: the patched runtime is
embedded, so no separate .NET shared runtime is needed. npm's suggested fallback
on unsupported platforms is the .NET tool and therefore inherits its runtime
minimum. Local `dotnet run` and ordinary build outputs are framework-dependent;
local NativeAOT `dotnet publish` outputs follow the self-contained route. The
viewer bundle is embedded in the CLI, not another .NET distribution.

The C# coverage campaign uses the existing Linux subreaper containment and retains raw streams,
subject exit and cleanup evidence. A live detached descendant makes the campaign incomplete even
when the subject exited zero. Unconfirmed cleanup retains the isolated snapshot. When the runner
cannot confirm its own descendant cleanup, on Linux or Windows, it keeps the snapshot and its marker,
names both on stderr and exits 2 (incomplete), whatever the subject returned and even when
interrupted. It writes nothing further into the assurance output. The adapter reports `native
campaign incomplete`, never a functional failure. When `identities.json` records a complete
campaign with confirmed cleanup and a subject status other than 2, only the runner's own cleanup
can explain the exit 2, so the adapter names it as `runner_cleanup_unconfirmed`. Otherwise it
reports the failure `identities.json` records, or that no diagnosis is available; the runner's
stderr still names the retained snapshot and the error. Windows continues
to use its native Job Object owner. Native Windows tests require the Windows CI runner; Linux skips
are missing Windows proof, not passing Windows proof.

On Windows, the `tar` first on `PATH` must be the Windows one. `npm/test/download.test.cjs` builds its fixture
archives by shelling out to `tar`, and `node_tests.cjs` runs that suite; Git for Windows' GNU tar
(`C:\Program Files\Git\usr\bin\tar.exe`) reads the test's `C:\...` destination as a remote host, so
the tar fixtures fail with `tar (child): Cannot connect to C: resolve failed` and the node coverage row
reports a `functional` finding. Prepending `C:\Windows\System32`, whose `tar` is bsdtar, fixes it.
The ZIP fixture uses fixed ZIP bytes and exercises real extraction on both platforms; it does not
rely on tar to create ZIP archives.

---

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
of 1800 seconds of execution plus 30 seconds of cleanup, and the facade exports `DYDO_ROW_DEADLINE`
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
own. On every platform each row keeps its 1800-second deadline, and `run_tests.py` keeps its
1800-second limit for the C# campaign.

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

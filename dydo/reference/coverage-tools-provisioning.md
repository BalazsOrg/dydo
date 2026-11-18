---
explains: ["DynaDocs.Tests/coverage/requirements.lock", "DynaDocs.Tests/coverage/package.json", "DynaDocs.Tests/coverage/pnpm-lock.yaml", "DynaDocs.Tests/coverage/gate_versions.py", "DynaDocs.Tests/Integration/GuardStaticGateProvisioningTests.cs"]
area: reference
type: reference
---

# Coverage Tools: Provisioning

[Coverage and Assurance Tools](./coverage-tools.md)

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

## Related

- [Coverage and Assurance Tools](./coverage-tools.md)

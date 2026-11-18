---
area: reference
type: reference
explains: ["DynaDocs.Tests/coverage/**", "Commands/GapCheckCommand.cs"]
---

# Assurance Tools: Rows, Results and Summaries

[Overview](./coverage-tools.md).

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

## Row states, exits and evidence

| State | Row exit | Cause |
|---|---|---|
| `passed` | 0 | the child exited 0 and every required artifact was created or refreshed |
| `failed` | 1 | a measured failure: a nonzero test exit, or a gate child exit other than 0, 2 or 130; or a [memory cap](./coverage-tools.md#memory-cap) hit, marked `memoryCapHit: true` |
| `unavailable` | 2 | the manifest declares the capability unavailable, with its reason |
| `invalid` | 2 | malformed row, unusable destination, an unrefreshed required artifact, or a gate child exit of 2 (evidence-incomplete) |
| `interrupted` | 130 | deadline or interrupt, after the adapter's own cleanup window |
| derived test row | 0, 1, 2 or 130 | a test row whose stack declares `suiteVerdict` under `--force-run`: it launches nothing (`argv: []`) and takes `passed` 0 or `failed` 1 from the coverage report; an unestablished verdict is `invalid` 2, or `interrupted` 130 when the coverage row never ran, each with a `reason` naming the coverage report |

Each row keeps the raw child exit in `childExit` beside its `resultExit`; the operation's aggregate
is the maximum of the row exits. A configured gate row must create or observably refresh every
required artifact during its child invocation: the facade compares metadata and content,
recursively for directory artifacts, and never deletes old evidence to manufacture freshness. That
comparison proves an observable change inside the child-operation interval, while the isolation
adapter owns protection from concurrent writers at the same evidence path.

An interrupt goes to the active adapter's process group. The facade grants up to 30 seconds for
cleanup before escalation, preserves the raw child exit when observed, stops the remaining rows and
records 130.

---

## The adapter summary

`gate_adapter.py` publishes one stable summary per row at
`DynaDocs.Tests/coverage/results/adapters/<stack>-<gate>.json`, written through a temporary file
under an exclusive `<summary>.lock` sibling; a surviving lock is a publication error worth exit 2,
never a silent overwrite. The identical bytes remain at
`results/assurance/run-<32 hex>/report.json`, whose directory also holds that run's
`inventory.json`, its command streams and the raw collector evidence.

| Field | Meaning |
|---|---|
| `schema` | `1` |
| `candidate` | `{commit, dirty, sourceFingerprint}`: Git HEAD, porcelain dirtiness, and the hash of the maintained-file inventory |
| `stack`, `gate` | the row this summary belongs to |
| `inventory` | `{path, sha256}` of the run's `inventory.json`, path relative to the artifact root |
| `tools` | both interpreter identities and every resolved tool pin |
| `commands` | ordered native command evidence |
| `collectors` | one uniform row per collector |
| `findings` | measured policy violations, each tagged with its collector |
| `gaps` | measurement errors: something the gate could not measure |
| `measurementComplete` | `false` exactly when `gaps` is nonempty |
| `exitCode` | 0 for a clean pass, 1 for findings, 2 for gaps |

### tools

```json
"tools": {
  "interpreters": {
    "caller": {"path": "<absolute>/python.exe", "sha256": "<hex>", "version": "3.14.4"},
    "dependencyBearing": {"path": ".static-gates/python/Scripts/python.exe",
                          "sha256": "<hex>"}
  },
  "pins": {
    "python": {"path": "DynaDocs.Tests/coverage/requirements.lock", "sha256": "<hex>",
               "resolved": {"coverage": "7.16.0", "ruff": "0.16.6", "vulture": "2.16"}},
    "javascript": {"path": "DynaDocs.Tests/coverage/pnpm-lock.yaml", "sha256": "<hex>",
                   "resolved": {"c8": "12.0.0", "jscpd": "5.1.2"}},
    "dotnet": {"path": "DynaDocs.Tests/coverage/metrics/packages.lock.json", "sha256": "<hex>",
               "resolved": {"Microsoft.CodeAnalysis.CSharp": "<version>"}},
    "altcover": {"path": ".config/dotnet-tools.json", "sha256": "<hex>",
                 "resolved": {"altcover.global": "9.0.102"}},
    "viewer": {"path": "viewer/pnpm-lock.yaml", "sha256": "<hex>",
               "resolved": {"typescript": "5.9.3", "vitest": "4.1.11"}},
    "statechart": {"path": "skills/engineering/statechart/scripts/pnpm-lock.yaml", "sha256": "<hex>",
                   "resolved": {"fast-check": "4.5.3", "jsdom": "26.1.0", "mermaid": "11.16.0"}}
  }
}
```

`caller` is the interpreter that ran the adapter; `dependencyBearing` is the pinned local
interpreter the dependency-carrying collectors launch. Each pin names its lock file, that file's
SHA-256, and the resolved name-to-version map it contains. An unreadable or malformed lock keeps
its `path` and records `"resolved": null` with a `reason` instead of dropping provenance silently.
The `resolved` maps above are abbreviated; the artifact carries every entry of each lock.

### commands

Ordered rows of
`{name, argv, cwd, environment, exit, elapsedSeconds, stdout, stdoutSha256, stderr, stderrSha256}`.
Stream paths are relative to the run directory. `environment` is only what this gate supplied to
that command - `APPDATA`, `NUGET_PACKAGES` and `DYDO_ROW_DEADLINE` for the adapter's own children,
and `DOTNET_CLI_USE_MSBUILD_SERVER`, `MSBUILDDISABLENODEREUSE` and
`DYNADOCS_GATE_METRICS_PREBUILT_DLL` inside the C# campaign - never the inherited process
environment, so ambient secrets stay out of the artifact.

The four coverage wrapper rows `python-coverage`, `javascript-coverage`, `typescript-coverage` and
`csharp-campaign` record `"stdout": null`, `"stderr": null` and `"streams": "inherited"`: those campaigns deliberately
inherit the child's streams so an operator can watch a long run. Every native command inside the C#
campaign is republished as its own row, with hashed streams, from the campaign's `commands.json`.

### collectors

`{name: {status, facts, findings, errors, artifacts}}` for every collector the row ran. `status` is
`pass`, `fail` or `error`: a `pass` may not hide findings or errors, a `fail` requires findings and
no errors, and an `error` requires at least one error identity. `artifacts` lists every raw file the
collector wrote as run-relative `{path, sha256}`. The summary's `findings` and `gaps` are these
rows' findings and errors, each carrying its collector name.

Before collecting, the adapter sets `APPDATA` to `dydo/_system/.local/appdata` and defaults
`NUGET_PACKAGES` to `~/.nuget/packages`, so tool caches stay off the developer profile.

---

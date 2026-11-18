---
explains: ["DynaDocs.Tests/coverage/gate_adapter.py", "DynaDocs.Tests/coverage/gate_collect.py", "DynaDocs.Tests/coverage/report.py"]
area: reference
type: reference
---

# Coverage Tools: Results and Adapter Summary

[Coverage and Assurance Tools](./coverage-tools.md)

## Row states, exits and evidence

| State | Row exit | Cause |
|---|---|---|
| `passed` | 0 | the child exited 0 and every required artifact was created or refreshed |
| `failed` | 1 | a measured failure: a nonzero test exit, or a gate child exit other than 0, 2 or 130; or a [memory cap](./coverage-tools-running.md#memory-cap) hit, marked `memoryCapHit: true` |
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

## Related

- [Coverage and Assurance Tools](./coverage-tools.md)

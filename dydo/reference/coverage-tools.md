---
area: reference
type: reference
explains: ["DynaDocs.Tests/coverage/**", "Commands/GapCheckCommand.cs"]
---

# Coverage and Assurance Tools

Python platform measurement is configured in `DynaDocs.Tests/coverage/python-platforms.json`.
Its explicit neutral rule assigns all ordinary inventory targets to Linux. Its three mixed-source
entries name Windows-native callables and pin the normalized source bytes; every unnamed point
in those modules stays Linux-owned. Review those entries whenever their source changes. No
threshold or exclusion changes: the policy remains 80% line, 60% branch and HCRAP 20, applied
to each platform's portion of a module. See
[DR 048's platform measurement amendment](../project/decisions/048-one-level-static-gates-certainly-wrong-no-escape-hatch.md)
for the decision and its proof conditions.

Run the ordinary `gap_check.py gate coverage --stack python` on Linux and Windows against the
same candidate. Each invocation still runs the entire native unittest suite. The collector keeps
the full coverage.py report and counters, then `python_platform.py` partitions the denominator
and emits `platform_selection` beside the measured modules in `adapters/python-coverage.json`.
The receipt includes all source hashes, the reviewed selection hash and ownership of every
line, branch, callable and callable body point. Missing selection, unsupported platforms and
source/callable drift fail before the suite starts.

CI always runs the union job and explicitly fails if either native job failed. After both pass,
it downloads their artifacts under the ignored results directory and verifies the complete union
from the same clean checkout, using the pinned Python runtime and requirements.lock closure:

```sh
python DynaDocs.Tests/coverage/python_platform.py --linux DynaDocs.Tests/coverage/results/coverage-platforms/python-coverage-linux/adapters/python-coverage.json --windows DynaDocs.Tests/coverage/results/coverage-platforms/python-coverage-windows/adapters/python-coverage.json --output DynaDocs.Tests/coverage/results/coverage-platforms/union.json
```

The union rejects dirty or different candidates, failed or incomplete gates, source/selection
differences and missing module, branch, line or callable-body points. It rebuilds the expected
inventory and owners from the candidate sources and selection, so matching omissions in both
reports also fail. Both jobs use the declared
Python 3.14.4 and Node 26.13.0 pins. The existing cancellation matrix and other CI suites remain
in place; release and publishing workflows are independent.

.NET platform measurement is configured in `DynaDocs.Tests/coverage/dotnet-platforms.json`. Its
`native` entries name Windows-native members by their Roslyn member name (`.ctor`, `get_Stage`,
`RunWindowsAsync`) and pin each source's SHA-256 with line endings normalized to LF. Its
`windows_tests` entry lists the fully qualified tests that reach those members and records why
Windows runs only them. `dotnet_platform.py` partitions the native join by emitted-body owner and
writes `platform_selection` beside the measured modules in `adapters/dotnet-coverage.json`. That
receipt holds the source and selection hashes, the owner of every line, branch and callable, and
on Windows the selected suite's pass count. A missing or drifted selection or an unsupported
platform fails before the campaign starts. So does, on Windows only, a selected test that is not
declared exactly once in its class (`discover_windows_tests`). A named member missing from the
compiled source inventory fails after the campaign, when the join is scoped (`_members` in
`scope`). A skipped selected test fails the Windows gate. The raw `joined.json` keeps every native
point.

The `dotnet-coverage` CI job runs the gate on `ubuntu-24.04` (the full suite) and on
`windows-latest` (the selected tests), each on the run's own commit `github.sha` (the merge ref on
a pull request, the pushed commit on a push), and uploads
`dotnet-coverage-linux-attempt-<run_attempt>` and `dotnet-coverage-windows-attempt-<run_attempt>`.
The dependent `dotnet-coverage-union` job requires both legs to pass. It then reads the run's job
record for its own attempt, and `coverage_artifacts.py select` takes the artifact of the attempt in
which each leg last ran. The identity and the report of that artifact must name that commit. Then
the job runs:

```sh
python DynaDocs.Tests/coverage/dotnet_platform.py --linux DynaDocs.Tests/coverage/results/coverage-platforms/linux/adapters/dotnet-coverage.json --windows DynaDocs.Tests/coverage/results/coverage-platforms/windows/adapters/dotnet-coverage.json --output DynaDocs.Tests/coverage/results/coverage-platforms
```

Both reports are required. The union rejects a missing report, a failed, incomplete or dirty gate,
reports from different commits, a Windows receipt without its selected-test count and a Linux
receipt with one. It then rebuilds the clean candidate's zero-hit denominator on Linux, using the
campaign's own builds and AltCover instrumentation without running a suite. Each report must hold
exactly its platform's lines, branches and callables of that denominator, under the same source
and selection hashes and owners, so matching omissions fail too. It writes `union.json`. Run it
from a clean checkout with no earlier in-place AltCover output (`__Saved`) in `bin/`.

`DynaDocs.Tests/coverage/gap_check.py` is the canonical project testing facade. Its adjacent
`gap_check.json` is the schema 1 manifest that declares each stack's test, static, coverage and
mutation adapters. `dydo/reference/gap-check.example.py` is a mechanically derived byte copy of that
runner for other projects; the [Testing Strategy](../guides/testing-strategy.md) holds the copy rule
and the adopted policy.

---

## Provisioning

See [this section](./coverage-tools-running.md#provisioning).

### Distribution runtime requirements

See [this section](./coverage-tools-running.md#distribution-runtime-requirements).

## Operations

See [this section](./coverage-tools-running.md#operations).

### Memory cap

See [this section](./coverage-tools-running.md#memory-cap).

## Declared rows

See [this section](./coverage-tools-results.md#declared-rows).

## Row states, exits and evidence

See [this section](./coverage-tools-results.md#row-states-exits-and-evidence).

## The adapter summary

See [this section](./coverage-tools-results.md#the-adapter-summary).

### tools

See [this section](./coverage-tools-results.md#tools).

### commands

See [this section](./coverage-tools-results.md#commands).

### collectors

See [this section](./coverage-tools-results.md#collectors).

## Static collectors

See [this section](./coverage-tools-statics.md#static-collectors).

## Statechart platform measurement

See [this section](./coverage-tools-statics.md#statechart-platform-measurement).

## Coverage collectors

See [this section](./coverage-tools-measurement.md#coverage-collectors).

## The C# campaign

See [this section](./coverage-tools-measurement.md#the-c-campaign).

## OpenCover and MethodDef token schema

See [this section](./coverage-tools-measurement.md#opencover-and-methoddef-token-schema).

## Roslyn policy CC

See [this section](./coverage-tools-measurement.md#roslyn-policy-cc).

## Portable-PDB document origins

See [this section](./coverage-tools-measurement.md#portable-pdb-document-origins).

## Toolchain provenance

See [this section](./coverage-tools-toolchain.md#toolchain-provenance).

## Related

See [this section](./coverage-tools-toolchain.md#related).

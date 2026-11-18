---
area: reference
type: reference
explains: ["DynaDocs.Tests/coverage/**", "Commands/GapCheckCommand.cs"]
---

# Assurance Tools: Static Collectors and Statechart Measurement

[Overview](./coverage-tools.md).

## Static collectors

`projects`, `source-inventory` and `associations` run in every static row. `projects` restores each
`.csproj` and evaluates `Compile`, `PackageReference`, `IsTestProject` and `TargetPath` through
`dotnet msbuild -getItem/-getProperty`; `source-inventory` classifies every maintained file;
`associations` validates `DynaDocs.Tests/coverage/test-associations.json`.

| Stack | Collector | Mechanism |
|---|---|---|
| `dotnet` | `csharp-source` | `GateMetrics.dll --project <csproj> --root <root>`: a real Roslyn compilation per project, per-callable cognitive complexity and policy CC, parameter counts, namespace edges |
| `dotnet` | `csharp-analyzers` | two `dotnet build` passes per project, the second with `-p:RunAnalyzers=true -p:ErrorLog=<sarif>`, joined to the maintained and generated inventories |
| `dotnet` | `versions` | the resolved Python, JavaScript and .NET pins, plus `node --version` equal to `v26.13.0` and `dotnet --version` equal to `10.0.303` |
| `python` | `python-source` | `python_metrics.py` under the dependency-bearing interpreter: complexipy cognitive, radon cyclomatic, nested ternaries, module-level score |
| `python` | `python-dead-code` | `ruff check --isolated --select F` and `vulture` over every maintained Python source |
| `python` | `python-dependencies` | AST import graph and cycles; a dynamic `__import__`, `import_module` or `spec_from_file_location` call is a gap, not a finding |
| `node` | `javascript-source` | `js_metrics.cjs`: ESLint with `eslint-plugin-sonarjs`, `allowInlineConfig: false`, complexity and cognitive complexity reported at threshold 0 so every function yields its value |
| `node` | `javascript-dependencies` | `dependency-cruiser` over the maintained set: cycles and unresolved imports |
| `node` | `javascript-unused-exports` | `knip` across the `DynaDocs.Tests/coverage` and `npm` packages, its counters cross-checked against the detailed report |
| `node` | `clones` | `jscpd` over the maintained C#, Python and JavaScript sources, plus a per-source eligibility proof that a skipped file really is under 15 lines or 100 tokens |
| `viewer` | `typescript-build` | `pnpm -C viewer run typecheck` (`tsc --noEmit`, strict with `noUnused*`); each `file(line,col): error TSnnnn` line is a finding, a nonzero exit with none is a gap |
| `viewer` | `typescript-lint` | `pnpm -C viewer run lint --no-inline-config --format json --output-file <raw>/eslint.json`; every message, warnings included, is a finding, and exit 2 or a lint failure with no message is a gap |
| `viewer` | `typescript-source` | `js_metrics.cjs typescript` or `tsx`: the same ESLint and SonarJS walker parsing through `@typescript-eslint/parser`, plus the module's top-level score. TypeScript's compiler owns unused bindings, so ESLint's JavaScript-only unused rules stay off |
| `viewer` | `typescript-dependencies` | `dependency-cruiser --ts-pre-compilation-deps` over `viewer/src`, so a type-only import is still an edge |
| `viewer` | `typescript-unused-exports` | `knip` over the viewer package with `src/main.tsx` and the test files as entries; its plugins load the whole package, so the out-of-root viewer TypeScript is judged too and counted |
| `viewer` | `clones` | `jscpd` over `viewer/src`, `.ts` under its `typescript` grammar and `.tsx` under `tsx`, with the same eligibility proof |
| `statechart` | `typescript-build` | `node viewer/node_modules/typescript/bin/tsc -p skills/engineering/statechart/scripts/tsconfig.json --typeRoots viewer/node_modules/@types --pretty false`: the viewer's locked TypeScript and node types over the package's own tsconfig, strict with `noUnused*`. Each `file(line,col): error TSnnnn` line is a finding, except an unused binding (`TS6133`, `TS6138`, `TS6192`, `TS6196`, `TS6198`, `TS6199`, `TS6205`) in a byte-identical upstream file, which the row lists as `upstreamUnusedBindings`. A diagnostic it cannot attribute, or a nonzero exit with none, is a gap |
| `statechart` | `typescript-source` | the same walker over the statechart runtime's measured TypeScript |
| `statechart` | `typescript-dependencies` | `dependency-cruiser --ts-pre-compilation-deps` over the measured statechart TypeScript |
| `statechart` | `typescript-unused-exports` | `knip` over the `skills/engineering/statechart/scripts` package, with its test files and the modules launchers and tests run by path (`statechart.ts`, `native-entry.ts`, `worker.ts`, `windows-cleanup.ts`, `test/java.ts`) as entries. The byte-identical upstream files and the package's corpus views are members of the graph, so the consumers they hold count, but knip ignores their own issues; the model lists them as `ignored` |
| `statechart` | `clones` | `jscpd` over the measured statechart TypeScript, with the same eligibility proof |

The `viewer` and `statechart` rows split the inventory's TypeScript between them: a path under
`skills/engineering/statechart/scripts/` belongs to `statechart`, and every other one to `viewer`.
The package declares no TypeScript compiler, since node runs its sources by stripping their types,
so its typecheck borrows the viewer's. A byte-identical upstream file stays in that program, because
the types it declares and uses decide the targets', so a type error in it is still a finding; only
its own unused bindings go unjudged, as knip holds it without judging it. The statechart stack has no
ESLint row: the compiler's `noUnused*` carries its unused bindings, and the walker carries complexity,
parameters and nested ternaries, as the lint does for the viewer.

Three of those rows feed one judgment. `csharp-source` derives namespace edges with
`GateMetrics.NamespaceDependencies`, from every identifier outside a `using` directive whose symbol
is a type, method, property, field or event, linking the enclosing namespace to the symbol's
containing namespace and keeping only edges whose two namespaces both declare a type in that
project's maintained trees; `python-dependencies` derives module edges from the AST import graph;
and `javascript-dependencies` keeps dependency-cruiser's resolved edges between two maintained
files. Each stack hands its edges to the same `gate_inventory.dependency_cycles`, which returns the
strongly connected components of that graph - every component of more than one member, plus any
self-edge - rather than a bounded search, so no traversal depth can omit a cycle. The `dotnet`
edges are pooled across every project before the components are computed, so dependency cycles that
close through a second project are still found. Each one is a single finding carrying its whole
component as sorted `members`, named `namespace-cycle` on `dotnet` and `module-cycle` on `python`,
`node`, `viewer` and `statechart`.

Suppression is itself a finding: a suppressed C# analyzer diagnostic on maintained source is
reported as `maintained-diagnostic-suppression`, an `istanbul`, `c8` or `v8 ignore` comment in
JavaScript or TypeScript is reported as `coverage-suppression`, and inline ESLint configuration is
disabled outright, in the walker and in the viewer's lint.

`versions` runs in the `dotnet` static row and measures the *caller* process, so running the facade
with any interpreter other than the local assurance venv (`bin/python` on Linux or
`Scripts/python.exe` on Windows)
produces a gap such as `No package metadata was found for colorama` and fails that row closed with
exit 2.

---

## Statechart platform measurement

`DynaDocs.Tests/coverage/statechart-platforms.json` pins the source SHA-256 and exact walker
callable identities owned by Windows. Its neutral leg is Linux. A declared callable includes
its nested bodies; all remaining points stay on Linux, without splitting or excluding modules.
Windows owns the CLI's native lifecycle callbacks and `windows.ts`'s job guard. Linux owns
`posix-lifetime.ts`, the POSIX recording helper `test/java.ts`, and `windows-cleanup.ts`, whose
top-level logic the adaptation suite exercises on Linux. Execution on the other platform does
not change ownership.

Linux runs the full package suite. Windows runs the map's `windows_tests.tests` files filtered
to its exact `names`; every named case must appear once and pass, with no skip. The selection
includes native job and TLC tests plus injected handshake and early cleanup failures.
Both legs use the unchanged DR 048 module and method thresholds.

Istanbul's pinned instrumenter derives the same statements, branches and function counters
from source on both platforms, including zero-hit points in unloaded modules. Compilation
finishes before the suite starts; the loader only reads prepared code, so instrumentation
dependencies do not appear in the architecture tests' import traces. The existing
TypeScript walker supplies callable identities and complexity. The report records source
digests, the selection digest and every point's owner in `platform_selection`.
`statechart_platform.py` rebuilds that denominator from a clean checkout, requires two passing
reports for its commit, re-evaluates policy, and checks every module, point, callable, body and
complexity fact. Matching receipts cannot hide a point removed from both reports.

CI's `statechart-coverage` matrix runs on `ubuntu-24.04` and `windows-latest`; the
`statechart-coverage-union` job uses `coverage_artifacts.py` to select each leg's latest executing
attempt before verifying the union. The independent `statechart-windows`,
`statechart-config-oracle` and `guard-windows` jobs retain their existing work.

```sh
python DynaDocs.Tests/coverage/statechart_platform.py --linux linux/adapters/statechart-coverage.json --windows windows/adapters/statechart-coverage.json --output union.json
```

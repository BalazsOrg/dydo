---
area: reference
type: reference
---

# Coverage Tools: C# Measurement and Provenance

[Coverage and Assurance Tools](./coverage-tools.md)

## The C# campaign

Restore the pinned tool once per checkout:

```powershell
dotnet tool restore --tool-manifest .config/dotnet-tools.json
dotnet tool run altcover -- version    # AltCover version 9.0.102
```

`.config/dotnet-tools.json` pins `altcover.global` 9.0.102 with the command `altcover`. The campaign
builds `DynaDocs.sln` and `GateMetrics.csproj` in Debug and the identity producer in Release, then
runs exactly these two commands
(`csharp_coverage.altcover_commands`, pinned by
`DynaDocs.Tests/coverage/tests/test_csharp_coverage.py`):

```text
dotnet tool run altcover --
  --inputDirectory=<root>/bin/Debug/net10.0
  --inputDirectory=<root>/DynaDocs.Tests/bin/Debug/net10.0
  --inputDirectory=<root>/DynaDocs.Tests/coverage/metrics/bin/Debug/net10.0
  --inplace
  --report=<results>/template.opencover.xml
  --reportFormat=OpenCover
  --eager --localSource --visibleBranches
  --assemblyFilter=^(?!(dydo|DynaDocs.Tests|GateMetrics)$).*

dotnet tool run altcover -- runner
  --recorderDirectory=<root>/DynaDocs.Tests/bin/Debug/net10.0
  --workingDirectory=<root>
  --executable=<python>
  --outputFile=<results>/coverage.opencover.xml
  --summary=N
  -- <root>/DynaDocs.Tests/coverage/csharp_coverage.py --_subject --root <root>
```

- `--inplace` instruments the three built output directories where they are. The originals are
  staged before the run and restored after it, and the report may name either the original path or
  AltCover's `__Saved` sibling.
- `--eager` writes runner-mode visits immediately instead of buffering an end-of-process table.
  AltCover 9.0.102's default aggregation keeps only the first spool per module, which silently
  loses every hit from a child process; the retained diagnosis is
  `dydo/agents/workspace/dyd96-portable-wip/native-altcover-evidence/child-diagnosis-research.md`.
- `--localSource` is assembly-level here: it drops nothing merely because a document is absent from
  disk.
- The assembly filter admits only `dydo`, `DynaDocs.Tests` and `GateMetrics`.
- The prepare command deliberately carries no `--showGenerated`: that flag marks unvisited
  generated code with a visit count of `-2` for the Visualizer, and `-2` is not a count the join may
  read.
- The subject the recorder runs is the ordinary unfiltered suite,
  `dotnet test DynaDocs.sln -c Debug --no-build ... RunConfiguration.TreatNoTestsAsError=true`
  followed by `tests/test_csharp_metrics.py`. Supplying selected-test arguments to a coverage row is
  exit 2.

Around those two commands the campaign proves identity: assembly and PDB SHA-256, MVID, the document
table and every method's token, signature and points are captured before instrumentation, compared
after instrumentation, and compared again after restoration. The template report is mapped to
original MethodDef tokens before execution, and the collected report must produce the identical map,
so instrumented renumbering can never rebind a method.

---

## OpenCover and MethodDef token schema

`csharp_join.coverage_methods` reads the collected report strictly. Each of these is a measurement
error rather than a coverage number:

| Element | Required | Rejected |
|---|---|---|
| `Modules/Module` | exactly one whose `ModuleName` is the assembly name | an unknown `ModulePath` alias, or a `hash` attribute that is not the original assembly's SHA-1 |
| `Files/File` | a unique `uid`, and a `fullPath` present in the assembly's PDB document table | a report source outside the PDB document inventory |
| `Classes/Class/Methods/Method` | a decimal `MetadataToken` and a `Name` equal to the original emitted signature | a duplicate token, an unknown token, or a signature mismatch |
| `SequencePoints/SequencePoint` | `vc`, `uspid`, `ordinal`, `offset`, `sl`, `sc`, `el`, `ec`, `fileid`, all nonnegative with `sl` at least 1 | a negative visit count, a `(file, line)` pair the PDB does not own, or a duplicate `(path, uspid, ordinal, offset, sl)` |
| `BranchPoints/BranchPoint` | `vc`, `uspid`, `fileid`, `sl`, `offset`, `offsetend`, `path`, `ordinal` | a `(file, line)` pair the PDB does not own, or a `uspid` already seen in the module |

Every eligible original token must appear in the report; a missing one fails with
`Missing physical token coverage`. Eligible means the method has at least one `maintained` point and
is not a `SourceBehavior` structural member - a semantic synthesized member with no authored
executable behaviour, which is accounted with its evidence instead of being counted.

---

## Roslyn policy CC

`GateMetrics.SourceMetrics.PolicyCc` is `1 +` the number of descendant syntax nodes, not entering a
nested callable, in this set: `if`, conditional expression, `for`, `foreach`, foreach-variable,
`while`, `do`, `case` label, `default` label, switch-expression arm, `&&`, `||`, `??`, `??=`,
conditional access, and `and`/`or` patterns. It is the `cc` term of HCRAP; the cognitive term comes
from SonarAnalyzer's own `CSharpCognitiveComplexityMetric`, so the build-time rule and the gate
agree on one number. Methods, accessors, expression-bodied members, local functions and lambdas are
each measured on their own control flow, and a top-level-statements file is measured as one
synthetic `Program::<Main>$` member.

The other stacks use their own producers for the same two numbers: radon plus complexipy for
Python, ESLint's `complexity` and `sonar/cognitive-complexity` rules for JavaScript and TypeScript.
For TypeScript a class field's non-function initializer, which ESLint scores as its own code path,
is its own callable row; JavaScript keeps refusing it, because its V8 join has no function literal
to match it to.

---

## Portable-PDB document origins

Every document reached by a non-hidden sequence point is classified by reproducible origin, never by
name, suffix or on-disk existence:

| Origin | Rule | Identity |
|---|---|---|
| `maintained` | an absolute URL under the inventory root with no `obj` path segment | its root-relative path |
| `generated` | under the inventory root with an `obj` path segment | its root-relative path |
| `package` | outside the root, under a `packageFolders` entry of the owning project's locked `obj/project.assets.json`, its first two segments naming a `libraries` entry | `nuget:<package>/<version>/<path>` |

Anything else exits 2 with a message ending `outside inventory root: <exact URL>`. Only
`maintained` points create a denominator. A method whose points are all generated or package is
accounted as `excluded by origin`; a method mixing `maintained` with another origin is a
measurement error; and a maintained document whose bytes no longer match the PDB's SHA-256 or SHA-1
checksum fails the join. The measured per-assembly document facts are retained in
`dydo/agents/workspace/dyd96-portable-wip/pdb-document-classification-evidence.md`.

---

## Toolchain provenance

`.static-gates/` is git-ignored, so each machine provisions its own measurement environment
and no gate ever installs anything. Five tool declarations and one CI runtime declaration define
what that environment must contain. Tool summaries republish the five tool declarations with their
hashes in `tools.pins`; the Python inventory separately republishes the runtime declaration and hash:

| Declaration | Supplies | Restored into |
|---|---|---|
| `.config/dotnet-tools.json` | AltCover 9.0.102 | the tool manifest, by `dotnet tool restore --tool-manifest .config/dotnet-tools.json` |
| `DynaDocs.Tests/coverage/.python-version` | exact CPython 3.14.4 for Release CI | `actions/setup-python@v5`, through `python-version-file` |
| `DynaDocs.Tests/coverage/requirements.lock` | coverage.py 7.16.0, ruff 0.16.6, vulture 2.16, complexipy 8.0.0, radon 6.0.1 | a local CPython 3.14.4 environment at `.static-gates/python` (`Scripts/python.exe` on Windows, `bin/python` elsewhere) |
| `DynaDocs.Tests/coverage/package.json`, resolved by `DynaDocs.Tests/coverage/pnpm-lock.yaml` | @typescript-eslint/parser, c8, dependency-cruiser, eslint, eslint-plugin-sonarjs, istanbul-lib-instrument, jscpd, knip | `node_modules` under `DynaDocs.Tests/coverage`, in pnpm's isolated layout, by `pnpm -C DynaDocs.Tests/coverage install --frozen-lockfile`; its `pnpm-workspace.yaml` holds the viewer's `minimumReleaseAge` |
| `viewer/package.json`, resolved by `viewer/pnpm-lock.yaml` | the viewer's TypeScript, ESLint, vitest and Istanbul toolchain its scripts run | `viewer/node_modules`, by `pnpm -C viewer install --frozen-lockfile` |
| `DynaDocs.Tests/coverage/metrics/packages.lock.json` | the Roslyn, SonarAnalyzer and Cecil closure of `GateMetrics.csproj` | that project's `obj/project.assets.json`, by `dotnet restore` |

The `versions` collector fails the `dotnet` static row closed if an installed version differs from
its lock, if a declared JavaScript dependency and the lock disagree, if a locked package is missing
without being optional, if the resolved NuGet closure differs from `obj/project.assets.json`, if the
Python declaration is not exactly 3.14.4, or if the running implementation is not CPython 3.14.
That last check stops at the series, so it is weaker than the environment actually needs: the
callable witness is not patch-independent, and the local interpreter must be the declared 3.14.4,
as Provisioning above records. Python evidence records the declared pin and its hash, the exact
observed implementation and patch, and the requirements-lock hash. The Windows job preflight and
each accepted run likewise record the exact observed version plus the executable and base-executable
digests. Node v26.13.0 and .NET SDK 10.0.303 remain exact runtime requirements.

For JavaScript the collector reads and hashes the coverage `pnpm-lock.yaml`. Each declared
dependency must equal its importer's specifier and resolved version, and launch through its
top-level `node_modules/<name>` link. Each real package folder under
`node_modules/.pnpm/*/node_modules` must be a locked `name@version`, one per store folder; a
symlink or junction there is a link, not a package. A locked package the store lacks must be
optional in every snapshot. The links beside each store package must resolve to exactly its lock
snapshot's `dependencies`, plus the `optionalDependencies` that are installed, at their locked
versions: a missing, wrong-version or unexpected link fails closed, and an optional dependency the
platform skipped needs no link. Links also resolve to the peer variant their snapshot key pins,
the importer's top-level links included, because two folders may hold one version under different
peers.

---

## Related

- [Coverage and Assurance Tools](./coverage-tools.md)

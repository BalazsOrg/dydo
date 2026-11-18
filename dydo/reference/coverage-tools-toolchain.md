---
area: reference
type: reference
explains: ["DynaDocs.Tests/coverage/**", "Commands/GapCheckCommand.cs"]
---

# Assurance Tools: Toolchain Provenance

[Overview](./coverage-tools.md).

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

- [Testing Strategy](../guides/testing-strategy.md) - the adopted policy, the gate route and the
  recorded triage
- [DR 048](../project/decisions/048-one-level-static-gates-certainly-wrong-no-escape-hatch.md) -
  one-level static gates, no escape hatch

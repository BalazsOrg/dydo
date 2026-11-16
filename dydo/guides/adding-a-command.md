---
area: guides
type: guide
---

# Adding a dydo Command

A new command or required option changes a closed set of code, test, help, and reference surfaces.
`CommandDocConsistencyTests` discovers the System.CommandLine tree and keeps those surfaces aligned.

## Required surfaces

| Surface | Required change |
|---|---|
| `Commands/DydoRootCommand.cs` | Register the top-level command; `Program.cs` runs this tree. |
| `Commands/GuardCommand.WorkerLane.cs` | Classify the command: add it to `WorkerAllowedDydoSubcommands` or to `TopLevelOnlyDydoSubcommands`. |
| `Commands/<Name>Command.cs` | Define the command and handler behavior. |
| `Commands/HelpCommand.cs` | Include the command path in agent-facing help when appropriate. |
| `DynaDocs.Tests/Commands/CommandSmokeTests.cs` | Exercise the command factory. |
| `dydo/reference/dydo-commands.md` | Document each option and show required flags in examples. |
| `Scaffold/dydo/reference/dydo-commands.md` | Keep the scaffolded source aligned with the installed reference. |

Update focused behavior tests as required by the command's risk. The consistency suite checks command
discovery, help presence, option coverage, required example flags, template/reference parity, and factory
smoke coverage.

## Classify the command for sub-agents

A call carrying `agent_id` is a sub-agent, and the guard lets it run only the dydo commands on
`WorkerAllowedDydoSubcommands`. Decide for the new command: a sub-agent may run it (read-only,
or a gate or step its skill hands to crew), or it stays with the top-level session
(`TopLevelOnlyDydoSubcommands`: setup, generation, the hook, a server). The guard compares the first word
after `dydo`, so a command's verbs share its decision; check that every verb is one a sub-agent may run
before allowing the command. `WorkerLaneClassificationTests` fails until the command is on exactly one
list, and `WorkerLaneSkillLintTests` fails when a crew skill, an engineering skill or a sub-agent
section tells a sub-agent to run a refused command. See [Guard System](../understand/guard-system.md).

## Product-boundary check

Before adding a command, confirm the capability belongs inside dydo. Local documentation, validation,
native skill authoring, guard, configuration, and utility operations fit the product boundary.
Linear work management does not: do not add commands that create, update, poll, provision, or
mirror Linear objects. The read-only Linear readers fit: `dydo map`, with its on-demand reads and
disposable saved-map cache, under [DR 052](../project/decisions/052-dydo-map-read-only-linear-view.md),
and `dydo survey`'s on-demand reads under
[DR 063](../project/decisions/063-dydo-survey-read-only-lane-digest.md).

Transition-only commands must be labeled as historical migration compatibility in active docs and must
not be presented as the current work model.

## Authored and projected files

A skill is a plain `skills/<category>/<name>/` folder; edit only that canonical source. `setup-skills.mjs` creates
host discovery links and is not a dydo CLI command.
The `Scaffold/` tree is copied verbatim by `dydo init`; keep
`Scaffold/dydo/reference/dydo-commands.md` and `dydo/reference/dydo-commands.md` aligned by hand.

Do not hand-edit the protected `dydo/index.md`, `dydo/files-off-limits.md` or `dydo.json`; the user
owns them and the guard blocks direct writes.

## Verification

Run the command's focused tests through the worktree-isolated runner, then the coverage gate:

```bash
py DynaDocs.Tests/coverage/run_tests.py -- --filter FullyQualifiedName~CommandDocConsistencyTests
py DynaDocs.Tests/coverage/gap_check.py
```

Finish with `dydo check` and `git diff --check`.

## Related

- [Testing Strategy](./testing-strategy.md)
- [CLI Commands](../reference/dydo-commands.md)
- [Architecture](../understand/architecture.md)

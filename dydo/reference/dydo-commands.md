---
area: reference
type: hub
---

# CLI Commands Reference

Complete reference for dydo's local setup, documentation, guard, worktree cleanup, validation, testing, and utility
commands.
Live work is managed in Linear through its official surfaces; no dydo command creates, updates,
polls, or mirrors a Linear object; `dydo map` and `dydo survey` read Linear on demand, and only `dydo map` keeps disposable saved maps. FutureFeatures live in Linear and are promoted by the user; historical repository records remain
durable evidence rather than a second work board.

Commands find the project by walking up to the nearest `dydo.json`; `dydo validate` is the exception
and reads it from the current directory. `dydo check`, `dydo graph` and `dydo validate` take
`--root <dir>` to work on another project: `<dir>` is a directory, or a file inside the project (the
walk starts at its folder). They walk up to the nearest `dydo.json`, read
config and docs from there, print `Root: <project root>`, and exit `2` when `<dir>` does not exist or is in no dydo
project. `dydo help` prints the one-screen summary;
`dydo <command> --help` is the authoritative option list.

---

## Setup Commands

[Setup Commands](./dydo-commands-core.md#setup-commands)

### dydo init

[dydo init](./dydo-commands-core.md#dydo-init)

### dydo adopt

[dydo adopt](./dydo-commands-core.md#dydo-adopt)

## Documentation Commands

[Documentation Commands](./dydo-commands-core.md#documentation-commands)

### dydo check

[dydo check](./dydo-commands-core.md#dydo-check)

### dydo fix

[dydo fix](./dydo-commands-core.md#dydo-fix)

### dydo index

[dydo index](./dydo-commands-core.md#dydo-index)

### dydo graph

[dydo graph](./dydo-commands-core.md#dydo-graph)

### dydo graph stats

[dydo graph stats](./dydo-commands-core.md#dydo-graph-stats)

## Guard Command

[Guard Command](./dydo-commands-core.md#guard-command)

### dydo guard

[dydo guard](./dydo-commands-core.md#dydo-guard)

## Linear Commands

[Linear Commands](./dydo-commands-linear.md#linear-commands)

### dydo map

[dydo map](./dydo-commands-linear.md#dydo-map)

### dydo survey

[dydo survey](./dydo-commands-linear.md#dydo-survey)

## Mode Command

[Mode Command](./dydo-commands-linear.md#mode-command)

### dydo mode

[dydo mode](./dydo-commands-linear.md#dydo-mode)

## Worktree Commands

[Worktree Commands](./dydo-commands-worktrees.md#worktree-commands)

### dydo archive

[dydo archive](./dydo-commands-worktrees.md#dydo-archive)

### dydo archive branch

[dydo archive branch](./dydo-commands-worktrees.md#dydo-archive-branch)

### dydo archive worktree

[dydo archive worktree](./dydo-commands-worktrees.md#dydo-archive-worktree)

### dydo archive list

[dydo archive list](./dydo-commands-worktrees.md#dydo-archive-list)

### dydo archive restore

[dydo archive restore](./dydo-commands-worktrees.md#dydo-archive-restore)

### dydo clean

[dydo clean](./dydo-commands-worktrees.md#dydo-clean)

### dydo pr-ready

[dydo pr-ready](./dydo-commands-delivery.md#dydo-pr-ready)

### dydo merge

[dydo merge](./dydo-commands-delivery.md#dydo-merge)

## Validation Command

[Validation Command](./dydo-commands-utilities.md#validation-command)

### dydo validate

[dydo validate](./dydo-commands-utilities.md#dydo-validate)

## Testing Command

[Testing Command](./dydo-commands-utilities.md#testing-command)

### dydo gap-check

[dydo gap-check](./dydo-commands-utilities.md#dydo-gap-check)

## Utility Commands

[Utility Commands](./dydo-commands-utilities.md#utility-commands)

### dydo statechart

[dydo statechart](./dydo-commands-utilities.md#dydo-statechart)

### dydo statechart resume

[dydo statechart resume](./dydo-commands-utilities.md#dydo-statechart-resume)

### dydo statechart install-skill

[dydo statechart install-skill](./dydo-commands-utilities.md#dydo-statechart-install-skill)

### dydo statechart doctor

[dydo statechart doctor](./dydo-commands-utilities.md#dydo-statechart-doctor)

### dydo completions

[dydo completions](./dydo-commands-utilities.md#dydo-completions)

### dydo version

[dydo version](./dydo-commands-utilities.md#dydo-version)

### dydo help

[dydo help](./dydo-commands-utilities.md#dydo-help)

## Exit Codes

[Exit Codes](./dydo-commands-utilities.md#exit-codes)

## Related

- [DynaDocs](./about-dynadocs.md) - Product boundary and operating model
- [Files Off-Limits](../files-off-limits.md) - The two path tiers `dydo guard` enforces
- [Writing Documentation](./writing-docs.md) - Documentation conventions validated by dydo

---
area: reference
type: reference
explains: ["Commands/**", "Services/**"]
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

See [this section](./dydo-commands-core.md#setup-commands).

### dydo init

See [this section](./dydo-commands-core.md#dydo-init).

Options: `--join`.

### dydo adopt

See [this section](./dydo-commands-core.md#dydo-adopt).

Options: `--join`.

## Documentation Commands

See [this section](./dydo-commands-core.md#documentation-commands).

### dydo check

`--base <merge-target>` adds advisory explained-code warnings to declaration validation.

See [this section](./dydo-commands-core.md#dydo-check).

Options: `--root`.

### dydo fix

See [this section](./dydo-commands-core.md#dydo-fix).

### dydo index

See [this section](./dydo-commands-core.md#dydo-index).

### dydo graph

See [this section](./dydo-commands-core.md#dydo-graph).

### dydo graph stats

See [this section](./dydo-commands-core.md#dydo-graph-stats).

## Guard Command

See [this section](./dydo-commands-core.md#guard-command).

### dydo guard

See [this section](./dydo-commands-core.md#dydo-guard).

Options: `--command`, `--host`.

## Linear Commands

See [this section](./dydo-commands-core.md#linear-commands).

### dydo map

See [this section](./dydo-commands-core.md#dydo-map).

Options: `--no-browser`.

### dydo survey

See [this section](./dydo-commands-core.md#dydo-survey).

## Mode Command

See [this section](./dydo-commands-mode-and-archive.md#mode-command).

### dydo mode

See [this section](./dydo-commands-mode-and-archive.md#dydo-mode).

Options: `--project`.

## Worktree Commands

See [this section](./dydo-commands-mode-and-archive.md#worktree-commands).

### dydo archive

See [this section](./dydo-commands-mode-and-archive.md#dydo-archive).

Options: `--branch`, `--remote`.

### dydo archive branch

See [this section](./dydo-commands-mode-and-archive.md#dydo-archive-branch).

Options: `--dry-run`, `--remote`.

### dydo archive worktree

See [this section](./dydo-commands-mode-and-archive.md#dydo-archive-worktree).

Options: `--dry-run`, `--remote`.

### dydo archive list

See [this section](./dydo-commands-mode-and-archive.md#dydo-archive-list).

### dydo archive restore

See [this section](./dydo-commands-mode-and-archive.md#dydo-archive-restore).

Options: `--branch`, `--path`, `--remote`.

### dydo clean

See [this section](./dydo-commands-delivery.md#dydo-clean).

Options: `--all`, `--archive`, `--dry-run`.

### dydo pr-ready

`--base <merge-target>` compares explained code against that merge base (default: local
`origin/HEAD`). Warnings name unchanged docs and touched paths; update the doc or state it
still holds. A warning does not block readiness. Nothing is fetched.

See [this section](./dydo-commands-delivery.md#dydo-pr-ready).

Options: `--body`.

### dydo merge

See [this section](./dydo-commands-delivery.md#dydo-merge).

Options: `--dry-run`, `--hostname`, `--join`, `--match-base-commit`, `--match-head-commit`.

## Validation Command

See [this section](./dydo-commands-delivery.md#validation-command).

### dydo validate

See [this section](./dydo-commands-delivery.md#dydo-validate).

## Testing Command

See [this section](./dydo-commands-utilities.md#testing-command).

### dydo gap-check

See [this section](./dydo-commands-utilities.md#dydo-gap-check).

Options: `--force-run`.

## Utility Commands

See [this section](./dydo-commands-utilities.md#utility-commands).

### dydo statechart

See [this section](./dydo-commands-utilities.md#dydo-statechart).

### dydo statechart resume

See [this section](./dydo-commands-utilities.md#dydo-statechart-resume).

### dydo statechart install-skill

See [this section](./dydo-commands-utilities.md#dydo-statechart-install-skill).

Options: `--skill`.

### dydo statechart doctor

See [this section](./dydo-commands-utilities.md#dydo-statechart-doctor).

### dydo completions

See [this section](./dydo-commands-utilities.md#dydo-completions).

### dydo version

See [this section](./dydo-commands-utilities.md#dydo-version).

### dydo help

See [this section](./dydo-commands-utilities.md#dydo-help).

## Exit Codes

See [this section](./dydo-commands-utilities.md#exit-codes).

## Related

See [this section](./dydo-commands-utilities.md#related).

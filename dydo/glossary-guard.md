---
area: general
type: reference
---

# Guard Glossary

Read this glossary when the topic touches the guard: which paths agents may read or write, and the
rules that warn them.

## Nudge

A guard rule: a pattern plus a message, at one of three severities, `notice`, `warn` or `block`. A
project configures its nudges in `dydo.json`, and the guard merges dydo's shipped `block` nudges in
memory on every evaluation, so `dydo.json` can neither remove nor downgrade them.

**See also:** [Guard System: Nudges](./understand/guard-system-nudges.md#layer-3-nudges)

## Off-Limits

The **Path Tier** whose patterns block every operation, read, write, search or bash: secrets,
credentials and dydo's `dydo/_system/**` machine state. A `## Whitelist` section carves narrow
exceptions to the configured patterns.

**See also:** [Files Off-Limits](./files-off-limits.md)

## Path Tier

One of the two path classes `dydo/files-off-limits.md` declares, **Off-Limits** and **Protected**,
both binding every agent.

**See also:** [Guard System: Path Tiers](./understand/guard-system-paths.md#layer-1-path-tiers)

## Protected

The **Path Tier** that every agent may read and none may write or delete. Its patterns sit under
`## Protected Patterns` in `dydo/files-off-limits.md`, which a project may extend. dydo's own
members are `dydo/index.md`, `dydo/files-off-limits.md` and `dydo.json`, the last always protected;
dydo's own commands still rewrite what they manage.

**See also:** [Guard System: Path Tiers](./understand/guard-system-paths.md#layer-1-path-tiers)

## Related

- [Glossary](./glossary.md) - the root glossary
- [Guard System](./understand/guard-system.md)

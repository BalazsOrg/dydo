---
area: understand
type: hub
---

# Guard System

How dydo enforces boundaries through the PreToolUse hook. Every tool call the hook's matcher names - reads, writes, searches, bash, in the main thread and inside every subagent and workflow - passes through `dydo guard` before execution. Three layers: path tiers (off-limits and protected), dangerous-bash detection, and nudges.

---

## How the Hook Intercepts Tool Calls

[How the Hook Intercepts Tool Calls](./guard-system-hook.md#how-the-hook-intercepts-tool-calls)

## Layer 1: Path Tiers

[Layer 1: Path Tiers](./guard-system-paths.md#layer-1-path-tiers)

## Layer 2: Bash Command Analysis

[Layer 2: Bash Command Analysis](./guard-system-bash.md#layer-2-bash-command-analysis)

### David Ondrej denylist audit

[David Ondrej denylist audit](./guard-system-denylist.md#david-ondrej-denylist-audit)

## Layer 3: Nudges

[Layer 3: Nudges](./guard-system-nudges.md#layer-3-nudges)

## Also Enforced

[Also Enforced](./guard-system-enforcement.md#also-enforced)

## Housekeeping Rides Along

[Housekeeping Rides Along](./guard-system-hook.md#housekeeping-rides-along)

## Integration for Other AI Tools

[Integration for Other AI Tools](./guard-system-hook.md#integration-for-other-ai-tools)

## Archive-before-delete guard

[Archive-before-delete guard](./guard-system-enforcement.md#archive-before-delete-guard)

## Related

- [Configuration Reference](../reference/configuration.md) - nudge format, off-limits
- [Architecture Overview](./architecture.md) - where the guard sits in the system
- [Decision 041](../project/decisions/041-dydo-cedes-orchestration-becomes-authoring-knowledge-layer.md) - why identity-gated enforcement left the guard

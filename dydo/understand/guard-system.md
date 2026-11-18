---
area: understand
type: concept
explains: ["Commands/GuardCommand*.cs", "Services/*Analyzer*.cs", "Services/OffLimitsService.cs", "Models/NudgeConfig.cs", "Commands/GuardCommand.NewFileNudges.cs", "Services/PrReadyCheck.cs", "Services/Explains*.cs"]
---

# Guard System

How dydo enforces boundaries through the PreToolUse hook. Every tool call the hook's matcher names - reads, writes, searches, bash, in the main thread and inside every subagent and workflow - passes through `dydo guard` before execution. Three layers: path tiers (off-limits and protected), dangerous-bash detection, and nudges.

---

## How the Hook Intercepts Tool Calls

See [this section](./guard-system-hook-and-paths.md#how-the-hook-intercepts-tool-calls).

## Layer 1: Path Tiers

See [this section](./guard-system-hook-and-paths.md#layer-1-path-tiers).

## Layer 2: Bash Command Analysis

See [this section](./guard-system-shell.md#layer-2-bash-command-analysis).

### David Ondrej denylist audit

See [this section](./guard-system-denylist-audit.md#david-ondrej-denylist-audit).

## Layer 3: Nudges

See [this section](./guard-system-nudges.md#layer-3-nudges).

## Also Enforced

See [this section](./guard-system-other-checks.md#also-enforced).

## Housekeeping Rides Along

See [this section](./guard-system-other-checks.md#housekeeping-rides-along).

## Integration for Other AI Tools

See [this section](./guard-system-other-checks.md#integration-for-other-ai-tools).

## Related

See [this section](./guard-system-other-checks.md#related).

## Archive-before-delete guard

See [this section](./guard-system-other-checks.md#archive-before-delete-guard).

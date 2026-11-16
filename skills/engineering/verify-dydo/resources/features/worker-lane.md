# Sub-agent dydo commands

## Sub-features

`Commands/GuardCommand.WorkerLane.cs` classifies every dydo command: worker-allowed, or on the
top-level-only list. A hook call carrying `agent_id` is a sub-agent's, and `Commands/GuardCommand.cs`
refuses it any dydo word off the first list and prints both lists. The skills hand sub-agents these
commands: the admiral check-in's surveyor runs `dydo survey`, crew run `dydo statechart` (with its
`install-skill` and `resume` verbs, which the guard reads as the same first word), the gates, and
`dydo merge`. The journey does not claim every shell dialect or nudge; `GuardWorkerLaneTests` covers
the guard's parsing.

## How to get to it (user POV)

A sub-agent runs a dydo command in its shell, and the host's PreToolUse hook, installed by
`dydo init claude`, sends the guard the call with the sub-agent's `agent_id`. The driver reads that
installed marker and sends synthetic hook events to the real guard.

## Driving it with control-dydo

`drive --run RUN --journey worker-lane` sends, as a sub-agent, `dydo init claude` and reads the
refusal's two lists. Then each command a skill names (`SKILL_COMMANDS` in the driver) must be allowed,
and each top-level-only word on the printed list must be refused by name. The same `dydo init claude`
without `agent_id` is allowed: the refusal is the worker lane's. Capture `worker-lane-verdicts.json`
and the `guard-*.json` receipts. No requested command is launched.

## Gotchas

An exit of 0 alone does not mean allow: PreToolUse decisions are in JSON. The driver's command list
mirrors the skills by hand; the skill lint test (`WorkerLaneSkillLintTests`) is what keeps every
command in sub-agent prose worker-allowed, so a command a skill adds is checked there, not here. This
protocol drive does not prove a real host sends `agent_id` for every sub-agent, and it runs only where
the driver's Windows containment runs.

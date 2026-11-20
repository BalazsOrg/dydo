# Sub-agent dydo commands

## Sub-features

`Commands/GuardCommand.WorkerLane.cs` classifies every dydo command: worker-allowed, or on the
top-level-only list. A hook call carrying `agent_id` is a sub-agent's, and `Commands/GuardCommand.cs`
refuses it any dydo word off the first list and prints both lists. The skills hand sub-agents these
commands: a surveyor runs `dydo survey`, crew run `dydo statechart` (with its
`install-skill` and `resume` verbs, which the guard reads as the same first word), the gates, and the
Merge implementer's `dydo merge <pr> --match-head-commit <sha> --match-base-commit <sha>`. The journey does not claim every shell dialect or nudge; `GuardWorkerLaneTests` covers
the guard's parsing.

## Which callers this is

The worker lane is for a native in-process sub-agent only (Claude's `Agent` tool, Codex's
`spawn_agent`), the caller whose hook payload carries `agent_id`. A T3 `delegate_task` child is not
one: its calls reach the guard as a top-level session's (DYD-1072: a delegated child ran
`dydo survey --help`, which is not worker-allowed in the base tree, and nothing refused it). So a
delegated thread's allowed or refused dydo command says nothing about this lane, and this journey
never uses one as proof, either way. It proves the lane only with a synthetic payload carrying
`agent_id`.

## How to get to it (user POV)

A native sub-agent runs a dydo command in its shell, and the host's PreToolUse hook, installed by
`dydo init claude`, sends the guard the call with the sub-agent's `agent_id`. The driver reads that
installed marker and sends synthetic hook events to the real guard.

## Driving it with control-dydo

`drive --run RUN --journey worker-lane` sends, as a sub-agent, `dydo init claude` and reads the
refusal's two lists. Then each command a skill names (`SKILL_COMMANDS` in the driver: check, fix, mode,
pr-ready, merge with its two `--match-*` pins, survey, statechart and its `install-skill` and `resume`
verbs, `--help`) must be allowed, and each top-level-only word on the printed list must be refused by
name. The same `dydo init claude` without `agent_id` is allowed: the refusal is the worker lane's.
Capture `worker-lane-verdicts.json` and the `guard-*.json` receipts. No requested command is launched:
the merge line is a string the guard reads, never a merge.

## Gotchas

An exit of 0 alone does not mean allow: PreToolUse decisions are in JSON. The driver's command list
mirrors the skills by hand, and two tests of `WorkerLaneSkillLintTests` keep it honest from the same
reading of the sub-agent prose: one fails when prose names a command the worker lane refuses, the other
(`JourneyCommands_RunEveryWorkerAllowedCommandSubAgentProseNames`) fails when prose names a
worker-allowed command word that no `SKILL_COMMANDS` entry runs. A skill that adds a command therefore
fails there until you add its invocation here, as the prose writes it. The reading joins the continued
lines of a fenced block by the guard's own rule (`GuardCommand.JoinLineContinuations`), so a command
split across lines is read whole. The test reads the tuple as
Python does: a commented-out entry is not run and does not count, and a tuple written in any syntax
beyond plain quoted strings separated by commas (an f-string, a `+`, two literals with no comma) fails
the test rather than being guessed at. The driver must bind `SKILL_COMMANDS` exactly once, as that
column-0 tuple: a second binding (a reassignment, a rebinding inside a function, a `for` target, an
import, an assignment-shaped line in a docstring) fails the test, because Python would run a different
tuple than the one credited. Reads of the name (a default argument included) and comment lines are not
bindings. The journey must also consume the tuple, but the lint only checks a loop header: the body of
`def worker_lane(` has to hold `for <name> in SKILL_COMMANDS:` over `SKILL_COMMANDS` itself, so a second
tuple, a slice or a call fails the test. It cannot see what the loop sends: a wrong argument to `guard`
inside it stays green. `test_control_dydo.py` proves that; it decodes each `guard-*.json` receipt (the
`stdin` event and the `stdout` decision) and compares them with the expected invocations: `dydo init claude`,
every `SKILL_COMMANDS` entry and every refused top-level-only word as a sub-agent call (each with
`agent_id` and `agent_type`), allowed or denied as expected, and one top-level `dydo init claude`.
A swapped, truncated or non-sub-agent call fails it. The prose is the crew and
engineering skills and the sub-agent parts of the officer files (a file whose intro says "assume no
officer role" counts whole, otherwise the section that says it), not these feature maps. This
protocol drive does not prove a real host sends `agent_id` for every sub-agent, and it runs only where
the driver's Windows containment runs.

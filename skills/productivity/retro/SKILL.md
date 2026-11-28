---
name: retro
description: "Conduct a retrospective on a session, reading its papercuts."
disable-model-invocation: true
---

<!-- Adapted from mattpocock/skills retro at 6f7f7831a625f040a8e06a0e7954826b87ce775a (MIT). -->

# Retro

The user has asked for a **retrospective**. You are suggesting improvements to the coding agent's **environment** to improve future runs.

## Boundary

Agents file papercuts; you read them and file none. A papercut's suggested fix is a hint, and the remedy is yours to propose. An accepted change becomes a ticket, or lands directly only when it is tiny and inside the scope the current ticket grants.

## Steps

1. Load `writing-for-agents` for the writing style guide.

2. Read the primary sources for the session the user specifies. This may mean searching through session logs on this machine. If the user doesn't specify a session, default to the current one. For a ticket run across several agents, the primary sources include the ticket's comments and its commits. Read alongside it the team's `Papercuts` Linear project: its tickets and the repeat comments on them. With no such project reachable, read the `Papercut` headings in the agents' final reports available.

3. Look for candidates for improvement in these categories. Every candidate traces to a cited papercut or session moment.

- **Navigation**: how easy was it for the agent to find the right files? Are there hidden dependencies between files? Would a **navigation pointer** make it easier? _Use when_ the session took a long time to find a piece of information.
- **Automated checks**: are there automated checks that could catch errors the agent made? Linting, typing, tests, `dydo check`? Read the repo's own check command first (its `package.json`/build-tool `lint`/`check` scripts, its CI workflow), so a check that already exists but sits unwired or silently broken is the finding, not a reinvention. A missing **guardrail** (no pre-commit hook and no CI job running its lint/typecheck/test command) is a candidate only when cited evidence shows a failure it would have caught. _Use when_ the agent made a mistake an automated check could have caught.
- **Coding standards**: should the **reviewer agent** be given a new rule to enforce? Should an existing rule be removed or clarified? Classify the violation first: a **mechanical** one (a fixed syntactic pattern, a banned API, an import shape, a file-location rule) gets a deterministic check, full stop: a custom rule in the repo's own linter, a new pre-commit hook, or a new CI job, whichever the repo's language and existing guardrail make cheapest. Default to the check over the rule. Reserve `dydo/guides/coding-standards.md` for genuine **judgement calls** (cross-file consistency, "matches the surrounding style," anything no guardrail could ever substitute for). _Use when_ the reviewer agent failed to catch a mistake.
- **Entry points**: are there steering instructions in `AGENTS.md`/`CLAUDE.md` that should move to coding standards (or automated checks) instead? _Use when_ the entry point is particularly large, in the repo or in the user's global scope.
- **Tool economy**: did the agent make expensive tool calls that could be streamlined? Is there any custom tooling (CLIs, MCPs) that is particularly token-inefficient? _Use when_ the agent made an expensive tool call.
- **No-ops**: look for instructions in steering files that don't modify the agent's behaviour. _Use when_ the steering files are large and unwieldy.
- **Information access**: look for opportunities to increase the agent's access to information. Teeing dev server logs, read-only access to third-party services. _Use when_ a crucial piece of information was not available to the agent.

4. Search every candidate against the existing homes and tickets you can reach (see Files). When a remedy already exists, point to it instead of proposing a new one. With Linear unreachable, say on each candidate that ticket deduplication was unavailable.

5. Present these candidates to the user, in order of severity, each with its citation. Repeat comments raise a papercut's severity; a symptom seen twice is a pattern.

## Reference

### Implementation vs Review

Remember that all work goes through two stages: implementation and review. The implementation agent has the most **context pressure**. They are responsible for exploration, writing code, and debugging failures.

The review agent has less context pressure. It often does not need to write code or debug.

This means that the review agent should be responsible for imposing coding standards, not the implementation agent.

### Files

The existing homes are tickets, Decision Records, guides, skills, nudges and hooks. The existing canonical home wins; a second one splits it.

- `AGENTS.md`/`CLAUDE.md`: these files are pushed to the context window of any agent working in this repo. They should be used incredibly sparingly, usually only for **navigation pointers** to other files.
- `dydo/guides/coding-standards.md`: the rules for review, not implementation. Add **navigation pointers** to docs if the file gets more than 1,000 lines long.
- `dydo/` docs: use docs as reference files, pointed to by other files.
- Skills: use skills for docs (since their description goes into the agent's context window), or for user-invoked commands. Follow the advice in the `writing-for-agents` skill.

---
name: to-spec
description: "Turn the current conversation into a spec and publish it to Linear: no interview, just synthesis of what you've already discussed."
disable-model-invocation: true
---

<!-- Adapted from mattpocock/skills to-spec at 6f7f7831a625f040a8e06a0e7954826b87ce775a (MIT). -->

# To Spec

This skill takes the current conversation context and codebase understanding and produces a spec. Do NOT interview the user; just synthesize what you already know.

The tracker is Linear. From the repository root, read the Ticket labels section of `dydo/reference/linear-workspace-standard.md`: it owns the Type and Mode vocabulary below.

## To spec or to project

Use `to-project` when the intent should become a Linear Project the factory runs: a Project in `Backlog` carrying intent only, which its coordinator and map holder take from there, the map holder writing its spec later. Use `to-spec` when the user wants the whole spec written now from this conversation, typically for work he will drive more hands-on, or work that fits under one ticket tree without a Project.

## Process

1. Explore the repo to understand the current state of the codebase, if you haven't already. From the repository root, read `dydo/glossary.md` and the area glossaries the topic touches, and use their vocabulary throughout the spec. Respect any Decision Records in `dydo/project/decisions/` in the area you're touching.

2. Sketch out the seams at which you're going to test the feature (`tdd` owns the seam vocabulary). Existing seams should be preferred to new ones. Use the highest seam possible. If new seams are needed, propose them at the highest point you can. The fewer seams across the codebase, the better - the ideal number is one.

Check with the user that these seams match their expectations.

3. Write the spec using the template below, then publish it to Linear as one ticket whose description is the spec. Create it in `Todo` - no need for additional triage - with one Type (`Feature`, or `Bug` when the conversation is a fix) and one Mode: `AFK` when an agent can take it to reviewed completion alone, `HITL` when the user said he will work it with an agent. If the conversation belongs to an existing Project, file the ticket in that Project; the Project's `Spec: <Project>` document stays its map holder's.

4. Return the ticket link and the next step: the user works it hands-on, or breaks it into tickets with `to-ticket`, the spec ticket becoming their parent.

<spec-template>

## Problem Statement

The problem that the user is facing, from the user's perspective.

## Solution

The solution to the problem, from the user's perspective.

## User Stories

A LONG, numbered list of user stories. Each user story should be in the format of:

1. As an <actor>, I want a <feature>, so that <benefit>

<user-story-example>
1. As a mobile bank customer, I want to see balance on my accounts, so that I can make better informed decisions about my spending
</user-story-example>

This list of user stories should be extremely extensive and cover all aspects of the feature.

## Implementation Decisions

A list of implementation decisions that were made. This can include:

- The modules that will be built/modified
- The interfaces of those modules that will be modified
- Technical clarifications from the developer
- Architectural decisions
- Schema changes
- API contracts
- Specific interactions

Do NOT include specific file paths or code snippets. They may end up being outdated very quickly.

Exception: if a prototype produced a snippet that encodes a decision more precisely than prose can (state machine, reducer, schema, type shape), inline it within the relevant decision and note briefly that it came from a prototype. Trim to the decision-rich parts, not a working demo, just the important bits.

## Testing Decisions

A list of testing decisions that were made. Include:

- A description of what makes a good test (only test external behavior, not implementation details)
- Which modules will be tested
- Prior art for the tests (i.e. similar types of tests in the codebase)

## Out of Scope

A description of the things that are out of scope for this spec.

## Further Notes

Any further notes about the feature.

</spec-template>

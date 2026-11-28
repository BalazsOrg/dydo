---
name: domain-modeling
description: Build and sharpen a project's domain model. Use when discussing codebase terminology, writing or editing the glossary, or recording or editing a Decision Record.
---

<!-- Adapted from mattpocock/skills domain-modeling at 6f7f7831a625f040a8e06a0e7954826b87ce775a (MIT). -->

# Domain Modeling

Actively build and sharpen the project's domain model as you design. This is the *active*
discipline: challenging terms, inventing edge-case scenarios, and writing the glossary and decisions
down the moment they crystallise. (Merely *reading* the glossaries for vocabulary is not this skill:
that's a one-line habit any skill can do, by the rule in Which glossaries to read. This skill is for
when you're changing the model, not just consuming it.)

## Where the model lives

Paths are read from the repository root:

- `dydo/glossary.md`: the root glossary. It holds the universal terms and lists every other
  glossary under `## Area glossaries`. Scaffolded in every project.
- `dydo/glossary-<area>.md`: one glossary per major area of the project, created with its first
  resolved term.
- `dydo/reference/dydo-glossary.md`: the work-model glossary, for how work is named. It ships
  with dydo and is read-only in a project that adopts dydo: challenge against it, never edit it.
  In the dydo repository itself it is where that vocabulary is authored, so a resolved work-model
  term is recorded there.
- `dydo/project/decisions/`: one Decision Record per choice, governed by
  `dydo/project/decisions/_decisions.md` and shaped like the records already there. Scaffolded in
  every project.

## Which glossaries to read

Read the root glossary, then only the area glossaries the topic touches, as its `## Area glossaries`
lines describe them; never read them all by default. If it is unclear which area a topic touches,
ask.

## During the session

### Challenge against the glossary

When the user uses a term that conflicts with the existing language in a glossary, call it out
immediately. "Your glossary defines 'cancellation' as X, but you seem to mean Y. Which is it?"

### Sharpen fuzzy language

When the user uses vague or overloaded terms, propose a precise canonical term. "You're saying
'account': do you mean the Customer or the User? Those are different things."

### Discuss concrete scenarios

When domain relationships are being discussed, stress-test them with specific scenarios. Invent
scenarios that probe edge cases and force the user to be precise about the boundaries between
concepts.

### Cross-reference with code

When the user states how something works, check whether the code agrees. If you find a
contradiction, surface it: "Your code cancels entire Orders, but you just said partial cancellation
is possible. Which is right?"

### Update the glossary inline

When a term is resolved, update its glossary right there. Don't batch these up: capture them as
they happen. Use the format and placement rules in [glossary-format.md](./resources/glossary-format.md).

A glossary should be totally devoid of implementation details. Do not treat it as a spec, a
scratch pad, or a repository for implementation decisions. It is a glossary and nothing else.

### Offer Decision Records sparingly

Only offer to create a Decision Record when all three are true:

1. **Hard to reverse**: the cost of changing your mind later is meaningful
2. **Surprising without context**: a future reader will wonder "why did they do it this way?"
3. **The result of a real trade-off**: there were genuine alternatives and you picked one for
   specific reasons

If any of the three is missing, skip the Decision Record. `_decisions.md` governs its numbering and
shape: number the file after the highest existing record, and update authored navigation when the
record needs a link.

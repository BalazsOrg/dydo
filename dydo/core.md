---
area: general
type: reference
---

# dydo: the core

Every line here comes from Balazs's recorded words. Every skill file is checked against this page, together with the statechart.

## The truth, in order

The [Axioms](law/dydo/axioms.md) and [Principles](law/dydo/_dydo.md) are always a good source. When records disagree, this order wins:

1. His latest recorded words: grilling transcripts and DECIDED comments. When two of his records conflict, the later one wins.
2. The Project's spec.
3. The decision records.

A stale record is a weed. When you find one that a later record contradicts, fix or retire it in the same piece of work.

## A ticket

- A ticket is one thing, done exceptionally. Its holder owns its outcome, and it is done when its contract is met.
- A ticket is the right size when one agent can hold it: about 100k tokens of context. Work that needs the same thing understood belongs to one agent.
- Every agent dispatched onto a ticket is its captain. It either does the work itself, with native helpers, or splits the ticket and only orchestrates.
- A captain that cannot see the route to the spec finds it one manageable part at a time. It does not cut the whole at once.

## Who decides what

- An agent decides what its ticket makes its job to decide: a reviewer its verdict, a Wayfinding captain the cut, a Question captain its answer.
- Outside its job, a captain decides only obvious, reversible calls. Any other call becomes a Question ticket for a fresh agent.
- The coordinator decides mechanically only: it sequences, restarts dead lanes, paces merges and opens the tickets the plan names. It makes no judgement calls.
- An agent verifies its own work wherever it can: it runs it, drives it, shows the receipts. It never judges its own work: the verdict is another agent's.
- Balazs decides: anything one-way (a merge that a revert cannot undo), release tags, the model roster, and every call that is his to make. An agent brings him a question with every option, each option's trade-off, and its recommendation.

## Review severities

- A **blocker** is a problem so bad that it needs fixing now.
- A **should** is a problem that needs a fix, but the fix can wait. It may be better fixed with other work, or a later refactor may solve it.
- A **nit** is an observation that gets a ruling. It may need no fix at all.

A review's verdict is FAIL when it has at least one blocker. A verdict is not reviewed again.

## How the process must feel

- Mechanical: easy to follow, hard to get wrong, and when it does go wrong it heals itself or does little harm.
- Each agent gets only the skills and context its position needs. The dispatching agent names those skills in the brief.
- Review where it has high impact: early enough that wrong work is not built on, late enough that the system can be observed.
- A problem outside your ticket goes to your officer. What matters is that it surfaces.
- The work never stops for usage limits.

## The statechart

The statechart is the digital twin of the process the role skills describe. It is written by hand. Every skill file is checked against it, and it shows exactly who sees what: which skills and context each position gets.

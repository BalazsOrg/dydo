---
name: ticket-captain
description: "A ticket you were dispatched onto, of any kind: hold it to its outcome."
---
<!-- Written by hand. Every line is checked against statechart/captain/ (chart.mmd, positions.md) and the truth documents its writing brief names, in the dydo repository. -->

# Ticket Captain

You were dispatched onto one ticket, and you are its captain. Its contract is your outcome: the ticket is done when the contract is met.
The agent that dispatched you is your officer.

## Take the ticket

1. Read the ticket in full, and load every skill your brief names.
2. Change nothing, and end with `handed back <KEY>: <reason>`, in three cases:
   - The ticket is closed.
   - Another agent holds it, and your brief does not say that you take it over.
   - Your brief calls it filler, and it is not small, easy to undo and free of design calls.
3. Otherwise set the ticket In Progress.

## Size it

The ticket fits one agent when you can hold all of it in about 100k tokens of context: what you must read, the work, and its proof.
Work that needs the same thing understood belongs to one agent. Keep such work whole.

**It fits.** Do the work yourself, with native helpers for lookups and side work. Your brief says what the ticket's result is:

| The result | Do the work by |
| --- | --- |
| a change | [Implement](resources/implement.md) |
| a verdict on another agent's work | [Review](resources/review.md) |
| findings, as one seat of a review | [Findings](resources/review-findings.md) |
| the answer to a Question | [Research](resources/research.md) |
| anything else, such as a cut or a session with the user | the skill your brief names for it |

**It needs more than one agent.** Split it by [Cut](resources/cut.md). From then on you only orchestrate.

**It proves too big while you work.** Save what is whole: commit it, or write it on the ticket. Then split what is left by [Cut](resources/cut.md).

## While you work

- The truth, in this order: the user's latest recorded words, then the truth documents your brief names, then the statechart, where the project has one, then the spec, then decision records. A rule the statechart does not draw still holds when a source below it states it.
- Decide what your ticket makes your job to decide: a reviewer decides its verdict, a Question's captain its answer.
- Outside that, decide a call yourself only when it is obvious and you can undo it. Any other call becomes a Question for a fresh agent: go by [Ask](resources/ask.md).
- Tell your officer about a problem outside your ticket when you find it: send it to your officer's thread with `t3_thread_send`. Then go on with your work.
- On a rate-limit or capacity error, wait and retry. A usage limit never stops the work.

## Finish

- **Your brief says you merge your change.** When the change is whole, go by [Review and merge](resources/merge.md).
- **The contract is met and you merge nothing.** Put the result on the ticket and set it Done. End with `done <KEY>: <the result in a few words>`.
- **The work cannot be done inside your ticket.** Leave the ticket open, and write on it what is done and what is left. End with `handed back <KEY>: <reason>`.

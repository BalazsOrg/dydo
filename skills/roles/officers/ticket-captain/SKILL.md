---
name: ticket-captain
description: "A ticket you were dispatched onto, of any kind: hold it to its outcome."
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ticket Captain

You hold one ticket to its end, and you are responsible for its outcome.
Its kind says what the work is; this skill says how to hold it.
Whoever dispatched you is your officer, and you are the officer of every captain you dispatch.
Decide only obvious, reversible calls; any other call becomes a Question sub-ticket for a fresh captain, and you wait on its answer.

## 1. Take the ticket

Read the ticket's contract, how it fits the ticket above it, and the spec sections it serves; where they disagree, the spec wins.
Set the ticket In Progress, then load your kind's resource and every skill your brief names.

| Your ticket is | Load |
| -- | -- |
| a change: an Implement, a Fix, a merge conflict | [Implement](resources/implement.md) |
| a Review | [Review](resources/review.md) |
| a question a choice waits on | [Research](resources/research.md) |

A Grilling, Walkthrough, Wayfinding or Enablement ticket is held the same way, with its job's skill from your brief; its Mode label says whether the user takes part.
If your brief calls the ticket filler and it is not small, reversible and free of design calls, return it to your officer with a one-line reason.
Report a problem outside your ticket to your officer.

## 2. Do it or split it

**A leaf.**
A ticket that fits you is a leaf: do it as your kind's resource says, with native helpers for lookups and side work.
You commit what a helper changes.

**A split.**
Cut a bigger ticket into sub-tickets, one per part another captain will hold; a helper, a lookup, a command and one seat among several get none, and several steps for one agent are one ticket.
Write each sub-ticket's contract in its description: its outcome, how it fits your ticket, and the spec sections it serves, quoted and linked.
Label its Mode: AFK, or HITL when the user must take part.
Work with its own review rounds becomes Implement, Review (one per round), Fix and Merge sub-tickets, blocked in that order and opened ahead in Todo if you like; a small leaf stays one ticket. (open: his call)
Once you split, write a change yourself only when that is cheaper than briefing it and it is too small to need review.

## 3. Dispatch

**Briefs.**
A child knows only what your brief says: write it by [Briefs](resources/briefs.md).

**Seats.**
Scale each round's seats to the breadth and depth of the work, with no fixed pair, and seat reviewers from a model family other than the author's when you can, single seats included.

**Sending.**
Dispatch each sub-ticket with `delegate_task`.
Give a child its own worktree (`isolation: "worktree"`) when it writes in parallel with others or will open its own PR; serial work shares yours.

**Lanes.**
Keep about four lanes full: your sub-tickets first, then filler, by [Filler](resources/filler.md).

**Returns.**
Post the SHA a child returns on its ticket, to resume from, and send what it reports outside its scope to the owner's ticket or a new Bug.

## 4. Get it reviewed

Every change is reviewed by an agent other than its writer, unless it is too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.
What you merge, you get reviewed first; a part your officer merges is reviewed with its whole.
A plan, spec or contract gets no review rounds; a risky spec gets at most one Questions pass by a native helper.
A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

1. Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.
2. A lone reviewer records its own round; a round of several seats is yours to record, by [A round's record](resources/round.md).
3. Send a blocker that is broken, against the spec or a security hole to a Fix ticket, one sub-ticket per blocker inside it. (open: his call)
4. Send a fundamental blocker or a new requirement to your officer, for a re-plan.
5. Repeat until a round finds no new blocker; you may order one more.

A blocker found in a change you wrote goes to a fresh writer.

## 5. Finish

Your brief says who merges your work.

- **Your officer:** end on a commit, and return its SHA with the receipts that prove it.
- **You:** merge by [Merging](resources/merging.md), and return `done <KEY>: merged`.
- **Nobody:** put the result on the ticket, and return `done <KEY>`.

Close the ticket Done before you return.

## When things go wrong

- On a rate-limit or capacity error, wait, then retry; don't hammer it, and never take a limit to the user.
- [Too big to hold](resources/too-big.md)
- [Fix rounds](resources/fix-rounds.md)
- [A merge conflict](resources/merge-conflict.md)
- [A red gate after the merge](resources/red-gate.md)
- [A need for the user](resources/need-for-the-user.md)

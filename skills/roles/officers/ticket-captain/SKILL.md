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
If your brief calls the ticket filler and it is not small, reversible and free of design calls, return it to your officer unstarted and unassigned, with a one-line reason.
Set the ticket In Progress, then load your kind's resource and every skill your brief names.

| Your ticket is | Load |
| -- | -- |
| a change: an Implement, a Fix, a merge conflict | [Implement](resources/implement.md) |
| a Review | [Review](resources/review.md) |
| a question a choice waits on | [Research](resources/research.md) |

A Grilling, Walkthrough, Wayfinding or Enablement ticket is held the same way, with its job's skill from your brief; its Mode label says whether the user takes part.

## 2. Do it or split it

**A leaf.**
A ticket that fits you is a leaf: do it as your kind's resource says, with native helpers for lookups and side work.
You commit what a helper changes.

**A split.**
Cut a bigger ticket into sub-tickets, one per part another captain will hold.
Work gets a ticket only when someone other than its asker uses what it leaves.
A helper, a lookup, a command, a file and one seat among several never get a ticket.
Several steps for one agent are one ticket.
Write each sub-ticket's contract in its description: its outcome, how it fits your ticket, and the spec sections it serves, quoted and linked.
Label its Mode: AFK, or HITL when the user must take part.
Work with its own review rounds gets a sub-ticket per step, blocked in this order: Implement, Review (one per round), Fix, Merge. You may open them ahead, in Todo. A small leaf or a batched should is one ticket, with no steps. (open: his call)
Once you split, write a change yourself only when it is cheaper than briefing it and too small to need review.

## 3. Dispatch

**Briefs.**
A child starts with your brief and none of your conversation: write it by [Briefs](resources/briefs.md).

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

Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.
If the merge is yours, open a PR for the work once `dydo pr-ready` passes, and have it reviewed before you merge; a part your officer merges is reviewed with its whole.
A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

1. Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.
2. A lone reviewer records its own round; a round of several seats is yours to record, by [A round's record](resources/round.md).
3. Route each blocker and each should, by [A finding's route](resources/findings.md).
4. Repeat until a round finds no new blocker; you may order one more.

## 5. Finish

Your brief says who merges your work.

- **Your officer:** end on a commit, and return its SHA with the receipts that prove it.
- **You:** merge by [Merging](resources/merging.md), and return `done <KEY>: merged`.
- **Nobody:** put the result on the ticket, and return `done <KEY>`.

Close the ticket Done once its contract is met, then return; a return before that leaves it open, as it stands.

## When things go wrong

- On a rate-limit or capacity error, wait, then retry; don't hammer it, and never take a limit to the user.
- Report a problem outside your ticket to your officer.
- [Too big to hold](resources/too-big.md)
- [Fix rounds](resources/fix-rounds.md)
- [A merge conflict](resources/merge-conflict.md)
- [A red gate after the merge](resources/red-gate.md)
- [A need for the user](resources/need-for-the-user.md)

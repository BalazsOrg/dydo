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

A brief that names a ticket key new to you is a dispatch: take it from here. Any other wake, by a return or a message, goes by [Waking](resources/waking.md).
Read the ticket's contract, how it fits the ticket above it, and the spec sections it serves; where they disagree, the spec wins.
Then check that the ticket is unstarted and that your brief and its contract give you what the work needs; if either fails, or your brief calls it filler, go by [Before you start](resources/start.md).
Set the ticket In Progress, then load your kind's resource and every skill your brief names.

| Your ticket is | Load |
| -- | -- |
| a change: an Implement, a Fix, a merge conflict | [Implement](resources/implement.md) |
| a Review | [Review](resources/review.md) |
| a question you are briefed to research | [Research](resources/research.md) |

A Grilling, Walkthrough, Wayfinding or Enablement ticket, or any other Question, is held the same way, with its job's skill from your brief; its Mode label says whether the user takes part.

## 2. Do it or split it

**A leaf.**
A ticket that fits you is a leaf: do it as your kind's resource says, with native helpers for lookups and side work.

**Fog.**
Before you cut a bigger ticket, check that you see its route to the spec: that you can write every part's contract now.
If you cannot, find the way a part at a time with `/wayfinder`, by [Finding the way](resources/wayfind.md): never cut the whole at once.

**A split.**
Cut a bigger ticket into sub-tickets, one per part another captain will hold, by [Sub-tickets](resources/sub-tickets.md).

## 3. Dispatch

**Briefs.**
A child starts with your brief and none of your conversation: write it by [Briefs](resources/briefs.md).
Give each child one thing to own, its ticket's outcome, and leave how to reach it to the child.

**Seats.**
Seat each Review ticket's holder from a model family other than the author's when you can; it sizes and seats its own round.

Once the user's roster stands below, pick each seat's model from it.

{roster_placeholder}

**Sending.**
Dispatch each AFK sub-ticket with `delegate_task` once nothing blocks it, except a Merge: that step is yours.
Give a child its own worktree (`isolation: "worktree"`) when it writes in parallel with others or will open its own PR; serial work shares yours.

**Lanes.**
Keep about four lanes full: your sub-tickets first, then filler, by [Filler](resources/filler.md).

## 4. Get it reviewed

Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.
If the merge is yours, open a PR for the work once `dydo pr-ready` passes, then have it reviewed in that PR before you merge; a part your officer merges is reviewed with its whole.
Put your ticket's key in that PR's title, and attach its link to the ticket yourself, with Linear's `save_issue` (`links`). (open: his call)
A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

1. Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.
2. The Review's holder records the round, by [A round's record](resources/round.md); you record none of it.
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
- A dispatch refused for another reason goes to your officer as it reads, its ticket left unstarted.
- Report a problem outside your ticket to your officer.
- [Too big to hold](resources/too-big.md)
- [Fix rounds](resources/fix-rounds.md)
- [A merge conflict](resources/merge-conflict.md)
- [A red gate after the merge](resources/red-gate.md)
- [A need for the user](resources/need-for-the-user.md)

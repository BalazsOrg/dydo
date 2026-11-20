---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 4. Run the review rounds

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:Review.Open`, kept in `claim:CaptainReview.Whole`

> Once the parts are wired and before the Merge hop, review the parent once: each part, the parts together, and their harmony.

`claim:Review.Skip`, kept in `claim:CaptainReview.Groups`

> Skip the parent's review when it only groups parts.

`claim:Review.Post`, kept in `claim:CaptainReview.Record`, `claim:CaptainRound.Verdict`, `claim:CaptainRound.PrLine`

> Holding a round with several seats, post one verdict naming each seat's model and provider and its blockers, and write the PR line at the same moment.

`claim:Review.FixTicket`, kept in `claim:CaptainReview.Fix`, `claim:CaptainTrouble.Parallel`

> At verdict time, turn the blockers into Fix rN with one Blocker ticket per blocker inside it; for parallel writers, split it into Fix rNb.

`claim:Review.Nit`, kept in `claim:CaptainRound.Nits`

> Record each nit as one line on its record.

`claim:Review.CaptainMade`, kept in `claim:CaptainReview.Mine`

> A blocker a later review finds in a change you made goes to a crew writer.

`claim:Review.Labels`, kept in `claim:CaptainRound.Changed`

> Record every severity a fix-hop author changed, with its reason.

`claim:Tree.NoSelfReview`, kept in `claim:CaptainReview.Other`, `claim:CaptainReview.Mine`

> Never rule on findings about work you authored.

`claim:Review.Broken`, kept in `claim:CaptainReview.Fix`, `claim:CaptainTrouble.NextRound`

> A broken or against-spec blocker is fixed, then the next round runs.

`claim:Review.Security`, kept in `claim:CaptainReview.Fix`, `claim:CaptainTrouble.Security`

> A security hole is fixed, and the next round runs with the security lens first.

`claim:Review.Fundamental`, kept in `claim:CaptainReview.Replan`

> Send a fundamental blocker or a new requirement to the officer above to re-plan; when that is the coordinator, it only opens a Wayfinding ticket, and a fresh captain there re-plans.

`claim:Review.NeedsBalazs`, kept in `claim:CaptainTrouble.Ask`, `claim:CaptainTrouble.Up`

> Pass a blocker that needs Balazs up the chain at once.

`claim:Review.Clean`, kept in `claim:CaptainReview.Clean`

> By default, the first round with no new blocker ends the review rounds.

`claim:Review.OneMore`, kept in `claim:CaptainReview.OneMore`

> You may still order one more round after a round with no new blocker.

`claim:Review.ToCheckpoint`, kept in `claim:CaptainTrouble.Checkpoint`

> After round 3 with a blocker, stop at the checkpoint and decide on purpose.

`claim:Should.Register`, kept in `claim:CaptainRound.Shoulds`

> Holding the round, check the open shoulds under the ticket and its ancestors, then register each new should as a child of the ticket where it was found.

`claim:Should.Reparent`, kept in `claim:CaptainImplement.Reparent`

> Holding a Shoulds batch, re-parent each should into it, keeping a `related: found in <KEY> rN` link on it.

`claim:Should.ParentDone`, kept in `claim:CaptainRound.ParentDone`

> A parent may close Done while its shoulds stay open.

`claim:Should.Fixed`, kept in `claim:CaptainRound.Cheap`

> Fix a cheap should in place before its parent merges; the rest wait for a shoulds batch.

`claim:Should.Dup`, kept in `claim:CaptainImplement.Duplicate`

> In a shoulds batch, mark each duplicate Duplicate.

`claim:Should.Cancel`, kept in `claim:CaptainImplement.Cancel`

> In a shoulds batch, cancel a should only with a one-line reason.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 4. Run the review rounds

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:Review.Open`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Once the parts are wired and before the Merge hop, review the parent once: each part, the parts together, and their harmony.

`claim:Review.Skip`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Skip the parent's review when it only groups parts.

`claim:Review.Post`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Holding a round with several seats, post one verdict naming each seat's model and provider and its blockers, and write the PR line at the same moment.

`claim:Review.FixTicket`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> At verdict time, turn the blockers into Fix rN with one Blocker ticket per blocker inside it; for parallel writers, split it into Fix rNb.

`claim:Review.Nit`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Record each nit as one line on its record.

`claim:Review.CaptainMade`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> A blocker a later review finds in a change you made goes to a crew writer.

`claim:Review.Labels`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Record every severity a fix-hop author changed, with its reason.

`claim:Tree.NoSelfReview`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Never rule on findings about work you authored.

`claim:Review.Broken`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> A broken or against-spec blocker is fixed, then the next round runs.

`claim:Review.Security`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> A security hole is fixed, and the next round runs with the security lens first.

`claim:Review.Fundamental`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Send a fundamental blocker or a new requirement to the officer above to re-plan; when that is the coordinator, it only opens a Wayfinding ticket, and a fresh captain there re-plans.

`claim:Review.NeedsBalazs`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Pass a blocker that needs Balazs up the chain at once.

`claim:Review.Clean`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> By default, the first round with no new blocker ends the review rounds.

`claim:Review.OneMore`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> You may still order one more round after a round with no new blocker.

`claim:Review.ToCheckpoint`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> After round 3 with a blocker, stop at the checkpoint and decide on purpose.

`claim:Should.Register`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Holding the round, check the open shoulds under the ticket and its ancestors, then register each new should as a child of the ticket where it was found.

`claim:Should.Reparent`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Holding a Shoulds batch, re-parent each should into it, keeping a `related: found in <KEY> rN` link on it.

`claim:Should.ParentDone`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> A parent may close Done while its shoulds stay open.

`claim:Should.Fixed`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Fix a cheap should in place before its parent merges; the rest wait for a shoulds batch.

`claim:Should.Dup`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> In a shoulds batch, mark each duplicate Duplicate.

`claim:Should.Cancel`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> In a shoulds batch, cancel a should only with a one-line reason.

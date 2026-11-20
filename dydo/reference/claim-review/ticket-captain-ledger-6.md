---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 5. Merge, clean up and close

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:FeatureMerge.GateFirst`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Run `dydo test gate` on the combined tree yourself, then `dydo merge`.

`claim:FeatureMerge.PreRed`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> On a real red gate before the merge, send an implementer to fix it on the ticket branch.

`claim:FeatureMerge.OneEntry`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Merge only through `dydo merge`; when it refuses, its stated reason says why.

`claim:FeatureMerge.NoMergeReview`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> A conflict-free merge into the feature branch gets no merge review.

`claim:FeatureMerge.NoMutationWait`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> No ticket or merge waits on mutation; it is checked at the landing.

`claim:FeatureMerge.Conflict`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> On `CONFLICT`, send an implementer to resolve it on the ticket branch.

`claim:FeatureMerge.Green`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Close the Merge hop Done once DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.

`claim:FeatureMerge.Red`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> A gate red after the merge reruns once through DYTEST.

`claim:SafeDelete.Archive`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> After a merge, safe-delete the branches and worktrees you made with `dydo archive`; it keeps them 30 days, then purges them.

`claim:Dispatch.CloseTicket`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Close your ticket Done once its work is merged, or, on a ticket that merges nothing, once its result is on the ticket.

`claim:Dispatch.CaptainReturn`, deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

> Then return one line to your brief's return-to, `done <KEY>: merged`, or `done <KEY>` for a ticket that merges nothing; a sub-captain returns the same, and everything else is on the ticket.

Resource `resources/master-route.md` (A PR into master), deferred: The hop 1 placeholder rebuilds nothing; DYD-1372 hop 2 gives this its fate.

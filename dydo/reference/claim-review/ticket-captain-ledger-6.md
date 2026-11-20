---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 5. Merge, clean up and close

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:FeatureMerge.GateFirst`, kept in `claim:CaptainMerging.Gates`, `claim:CaptainMerging.Merge`

> Run `dydo test gate` on the combined tree yourself, then `dydo merge`.

`claim:FeatureMerge.PreRed`, kept in `claim:CaptainMerging.Broken`

> On a real red gate before the merge, send an implementer to fix it on the ticket branch.

`claim:FeatureMerge.OneEntry`, kept in `claim:CaptainMerging.Merge`, `claim:CaptainMerging.Refusal`

> Merge only through `dydo merge`; when it refuses, its stated reason says why.

`claim:FeatureMerge.NoMergeReview`, kept in `claim:CaptainMerging.Clean`

> A conflict-free merge into the feature branch gets no merge review.

`claim:FeatureMerge.NoMutationWait`, withheld (open) in `claim:CaptainMerging.Mutation`

> No ticket or merge waits on mutation; it is checked at the landing.

`claim:FeatureMerge.Conflict`, kept in `claim:CaptainMerging.Broken`, `claim:CaptainMergeTrouble.Resolver`

> On `CONFLICT`, send an implementer to resolve it on the ticket branch.

`claim:FeatureMerge.Green`, kept in `claim:CaptainMerging.Ci`

> Close the Merge hop Done once DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.

`claim:FeatureMerge.Red`, kept in `claim:CaptainMergeTrouble.Rerun`

> A gate red after the merge reruns once through DYTEST.

`claim:SafeDelete.Archive`, withheld (open) in `claim:CaptainMerging.Archive`

> After a merge, safe-delete the branches and worktrees you made with `dydo archive`; it keeps them 30 days, then purges them.

`claim:Dispatch.CloseTicket`, kept in `claim:CaptainFinish.Close`, `claim:CaptainFinish.Nothing`

> Close your ticket Done once its work is merged, or, on a ticket that merges nothing, once its result is on the ticket.

`claim:Dispatch.CaptainReturn`, kept in `claim:CaptainFinish.Merged`, `claim:CaptainFinish.Nothing`, `claim:CaptainFinish.Return`

> Then return one line to your brief's return-to, `done <KEY>: merged`, or `done <KEY>` for a ticket that merges nothing; a sub-captain returns the same, and everything else is on the ticket.

Resource `resources/master-route.md` (A PR into master), deferred: `dydo merge` refuses the default branch as the user's click, so the route into it is not rebuilt in DYD-1372.

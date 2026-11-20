---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 5. Merge, clean up and close

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:FeatureMerge.GateFirst`, kept in `claim:CaptainMerging.Gates`, `claim:CaptainMerging.Merge`

> Run `dydo test gate` on the combined tree yourself, then `dydo merge`.

A clause of it, dropped, it fails Verify: The CLI has no such command (71e649c0); the line names the project's gates, as the brief gives them.

> `dydo test gate`

`claim:FeatureMerge.PreRed`, kept in `claim:CaptainMerging.Broken`

> On a real red gate before the merge, send an implementer to fix it on the ticket branch.

`claim:FeatureMerge.OneEntry`, kept in `claim:CaptainMerging.Merge`, `claim:CaptainMerging.Refusal`

> Merge only through `dydo merge`; when it refuses, its stated reason says why.

A clause of it, dropped, it fails Handled: The guard blocks every other way to merge a PR and names this command, so the line has only to give it.

> Merge only through

`claim:FeatureMerge.NoMergeReview`, kept in `claim:CaptainMerging.Clean`

> A conflict-free merge into the feature branch gets no merge review.

`claim:FeatureMerge.NoMutationWait`, withheld (open) in `claim:CaptainMerging.Mutation`

> No ticket or merge waits on mutation; it is checked at the landing.

A clause of it, dropped, it fails Need: Checking it there is the act of whoever lands the Project, never a captain's (41a553f1).

> it is checked at the landing

`claim:FeatureMerge.Conflict`, kept in `claim:CaptainMerging.Broken`, `claim:CaptainMergeTrouble.Resolver`

> On `CONFLICT`, send an implementer to resolve it on the ticket branch.

`claim:FeatureMerge.Green`, kept in `claim:CaptainMerging.Ci`

> Close the Merge hop Done once DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.

A clause of it, withheld (open) in `claim:CaptainMerging.Receipt`

> DYTEST holds a green receipt on the merge tree and

`claim:FeatureMerge.Red`, kept in `claim:CaptainMergeTrouble.Rerun`

> A gate red after the merge reruns once through DYTEST.

A clause of it, withheld (open) in `claim:CaptainMergeTrouble.Dytest`

> through DYTEST

`claim:SafeDelete.Archive`, withheld (open) in `claim:CaptainMerging.Archive`

> After a merge, safe-delete the branches and worktrees you made with `dydo archive`; it keeps them 30 days, then purges them.

A clause of it, dropped, it fails Need: What the archive does with them after 30 days asks nothing of the captain.

> then purges them

`claim:Dispatch.CloseTicket`, kept in `claim:CaptainFinish.Close`, `claim:CaptainFinish.Nothing`

> Close your ticket Done once its work is merged, or, on a ticket that merges nothing, once its result is on the ticket.

A clause of it, dropped, it fails Verify: A ticket is done when its contract is met (7b3d2e3e): a captain whose officer merges closes Done on its commit, before any merge.

> once its work is merged

`claim:Dispatch.CaptainReturn`, kept in `claim:CaptainFinish.Merged`, `claim:CaptainFinish.Nothing`, `claim:CaptainFinish.Return`

> Then return one line to your brief's return-to, `done <KEY>: merged`, or `done <KEY>` for a ticket that merges nothing; a sub-captain returns the same, and everything else is on the ticket.

A clause of it, dropped, it fails Verify: One line is the return of a captain that merges, or merges nothing; one whose officer merges returns its commit's SHA with the receipts (claim CaptainFinish.HandOver).

> Then return one line

A clause of it, dropped, it fails Handled: T3 returns a child's final result to the thread that started it; no brief has to name where.

> to your brief's return-to

A clause of it, dropped, it fails Need: It asks no act of its own: the case that merges nothing already puts its result on the ticket (claim CaptainFinish.Nothing), and a merge's record is its PR and its tickets.

> and everything else is on the ticket

Resource `resources/master-route.md` (A PR into master), deferred: `dydo merge` refuses the default branch as the user's click, so the route into it is not rebuilt in DYD-1372.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: MasterRoute

## TwoWay

`claim:MasterRoute.TwoWay`: A two-way PR into master merges AFK by its captain once its evidence is complete.

Spec: R26, R27, R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Offered → Paced, "declared two-way in the PR body; it waits for its turn".

**ticket-captain** (Offered, exit):

> Declare the PR two-way in its body when it is none of the one-way kinds and no path flags it; there is no objection window.

**working-tree-standard** (TreeMerging):

> A two-way PR into master merges AFK, by its captain, once it has a PASS on the head or an ancestor, gates green on the combined tree with current master, CI green, and a body declaring it two-way.

## OneWay

`claim:MasterRoute.OneWay`: A one-way PR carries a recovery plan and waits on his click; it is paced like any main-bound merge.

Spec: R27, R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Offered → Paced, "one-way, flagged or declared; it waits for its turn".

**ticket-captain** (Offered, exit):

> A one-way PR carries a recovery plan and waits on his click; declare it yourself when no path flags it.

## Turn

`claim:MasterRoute.Turn`: Main-bound merges land one at a time; each re-checks against current master just before landing.

Spec: R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Paced → Resync, "the coordinator gives it its turn".

**coordinator** (Paced, exit):

> Land main-bound merges one at a time; each re-checks against current master just before landing.

**ticket-captain** (Resync, arrival):

> On your turn, re-sync with current master and re-check.

**working-tree-standard** (TreeMerging):

> Main-bound merges land one at a time, paced by the coordinator; each re-checks against current master just before landing, and a PR forced to re-sync a third time is held while the coordinator sequences the colliding work.

## Merge

`claim:MasterRoute.Merge`: The captain merges a two-way PR once the re-check is green.

Spec: R26, R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Resync → AfkMerge, "two-way, re-check green: PASS, gates green with current master, CI green".

**ticket-captain** (Resync, exit):

> Merge a two-way PR yourself with `dydo merge` once the re-check is green.

## AwaitClick

`claim:MasterRoute.AwaitClick`: A one-way PR whose re-check is green goes on to his click.

Spec: R27, R29, R31

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Resync → CiWatch, "one-way, re-check green: on to his click". A system transition: no skill carries it.

## Again

`claim:MasterRoute.Again`: Master moved: the PR re-syncs again.

Spec: R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Resync → Paced, "master moved: re-sync again". A system transition: no skill carries it.

## Hold

`claim:MasterRoute.Hold`: A PR forced to re-sync a third time is held while the coordinator sequences the colliding work.

Spec: R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Resync → Held, "forced to re-sync a third time".

**working-tree-standard** (TreeMerging, on claim:MasterRoute.Turn):

> Main-bound merges land one at a time, paced by the coordinator; each re-checks against current master just before landing, and a PR forced to re-sync a third time is held while the coordinator sequences the colliding work.

**coordinator** (Held, arrival):

> Hold a PR forced to re-sync a third time and sequence the colliding work.

**ticket-captain** (Held, arrival):

> A third forced re-sync holds your PR until the coordinator sequences the colliding work.

## Sequenced

`claim:MasterRoute.Sequenced`: The coordinator releases a held PR once the colliding work is sequenced.

Spec: R29

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Held → Paced, "the colliding work is sequenced".

**coordinator** (Held, exit):

> Release the held PR once the colliding work is sequenced.

## AfkDone

`claim:MasterRoute.AfkDone`: dydo merge merges the two-way PR into master.

Spec: R26

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): AfkMerge → Merged, "dydo merge merges into master". A system transition: no skill carries it.

## CiGreen

`claim:MasterRoute.CiGreen`: Before his click, the captain watches its own CI and opens the HITL Merge only when it is green.

Spec: R31, S12

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): CiWatch → HitlMerge, "its CI is green".

**ticket-captain** (CiWatch, exit):

> Watch your own CI and open the HITL Merge only when it is green.

## Click

`claim:MasterRoute.Click`: He clicks the one-way merge.

Spec: R27, R31

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): HitlMerge → Merged, "Balazs clicks the merge". A system transition: no skill carries it.

## Woken

`claim:MasterRoute.Woken`: The survey wakes the captain on a merged PR with its HITL Merge open; the hop closes on green.

Spec: R20, R31

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Merged → Closed, "receipt and CI green on the merge".

**ticket-captain** (Merged, exit):

> When the survey wakes you on `PR merged, HITL Merge open`, or your AFK merge lands, close the Merge hop as for any merge.

## RealRed

`claim:MasterRoute.RealRed`: A real red after a master merge is reverted at once and becomes a CI-findings ticket.

Spec: R22, Y9

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Merged → Reverted, "a real red after the merge". A system transition: no skill carries it.

## Reland

`claim:MasterRoute.Reland`: A reverted master merge re-lands by being offered again.

Spec: R22

Transition in A PR into master (`claims/statechart/master-route/MasterRoute.tla`): Reverted → Offered, "fixed, fix-scoped round passed, offered again".

**ticket-captain** (Reverted, exit):

> A reverted master merge re-lands by offering its PR again, paced like the first time.

## OneWayKinds

`claim:MasterRoute.OneWayKinds`: The one-way kinds of PR into master.

Spec: R27

**ticket-captain** (Offered, nominal):

> One-way kinds: data migrations; publishing a package or a public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main (CI, branch protection, release workflows, the guard).

**working-tree-standard** (TreeMerging):

> A one-way PR, one whose revert doesn't undo its effect, needs his click and carries a recovery plan. One-way kinds: data migrations; publishing a package or a public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main (CI, branch protection, release workflows, the guard).

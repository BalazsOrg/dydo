---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Overview

## Build

`claim:Overview.Build`: In the overview, the model builds the skills the Project runs on.

Spec: K1, K6

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): ClaimArea → ProjectArea, "the model builds the skills the Project runs on". A system transition: no skill carries it.

## Cut

`claim:Overview.Cut`: In the overview, the admiral cuts a ticket and dispatches its captain.

Spec: H5, H3

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): ProjectArea → DispatchArea, "the admiral cuts a ticket and dispatches its captain". A system transition: no skill carries it.

## Start

`claim:Overview.Start`: In the overview, the holder sets its step In Progress.

Spec: S6, S13

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): DispatchArea → StatusArea, "the holder sets its step In Progress". A system transition: no skill carries it.

## Hops

`claim:Overview.Hops`: In the overview, a step with its own review loop gets its hops.

Spec: S3, S4

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): StatusArea → HopArea, "a step with its own review loop gets its hops". A system transition: no skill carries it.

## Prove

`claim:Overview.Prove`: In the overview, the Implement hop proves with receipts.

Spec: R5, T3

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): HopArea → ProofArea, "the Implement hop proves with receipts". A system transition: no skill carries it.

## Explain

`claim:Overview.Explain`: In the overview, a change to declared paths flags its doc.

Spec: D5

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): ProofArea → DocsArea, "a change to declared paths flags its doc". A system transition: no skill carries it.

## Review

`claim:Overview.Review`: In the overview, the parts are wired; the review opens.

Spec: R2

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): DocsArea → ReviewArea, "the parts are wired; the review opens". A system transition: no skill carries it.

## Shoulds

`claim:Overview.Shoulds`: In the overview, a round finds a should.

Spec: S17

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): ReviewArea → ShouldArea, "a round finds a should". A system transition: no skill carries it.

## Merge

`claim:Overview.Merge`: In the overview, PASS on the head: the Merge hop runs dydo merge.

Spec: R17, R23

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): ShouldArea → MergeArea, "PASS on the head: the Merge hop runs dydo merge". A system transition: no skill carries it.

## Findings

`claim:Overview.Findings`: In the overview, a real red after the merge becomes a CI finding.

Spec: Y9

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): MergeArea → FindingsArea, "a real red after the merge becomes a CI finding". A system transition: no skill carries it.

## Feed

`claim:Overview.Feed`: In the overview, findings feed the admiral's next cut.

Spec: H5, S22

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): FindingsArea → ProjectArea, "findings feed the admiral's next cut". A system transition: no skill carries it.

## Quiet

`claim:Overview.Quiet`: In the overview, the survey finds a quiet hop.

Spec: S23, S24

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): FindingsArea → LivenessArea, "the survey finds a quiet hop". A system transition: no skill carries it.

## Need

`claim:Overview.Need`: In the overview, a need only Balazs can settle goes up the chain.

Spec: H16

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): LivenessArea → EnvoyArea, "a need only Balazs can settle goes up the chain". A system transition: no skill carries it.

## Hard

`claim:Overview.Hard`: In the overview, a hard call goes to the tribunal.

Spec: H5, L18

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): EnvoyArea → LawArea, "a hard call goes to the tribunal". A system transition: no skill carries it.

## Night

`claim:Overview.Night`: In the overview, overnight rulings wait for the morning brief.

Spec: L19, L21

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): LawArea → StaffArea, "overnight rulings wait for the morning brief". A system transition: no skill carries it.

## Land

`claim:Overview.Land`: In the overview, every delivery ticket merged: the landing runs.

Spec: N5, T12

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): StaffArea → MutationArea, "every delivery ticket merged: the landing runs". A system transition: no skill carries it.

## Accept

`claim:Overview.Accept`: In the overview, his acceptance: the landing PR goes to master.

Spec: N6

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): MutationArea → MasterArea, "his acceptance: the landing PR goes to master". A system transition: no skill carries it.

## Archive

`claim:Overview.Archive`: In the overview, merged: branches and worktrees archived.

Spec: N10

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): MasterArea → SafeDeleteArea, "merged: branches and worktrees archived". A system transition: no skill carries it.

## Tag

`claim:Overview.Tag`: In the overview, Balazs pushes the release tag.

Spec: R28, N2

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): SafeDeleteArea → Tagged, "Balazs pushes the release tag". A system transition: no skill carries it.

## Ship

`claim:Overview.Ship`: In the overview, adopters take one ordinary update.

Spec: K18, N8

Transition in [dydo 4.0.0](../../../claims/statechart/overview/Overview.tla): Tagged → AdopterArea, "adopters take one ordinary update". A system transition: no skill carries it.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Overview

## Build

`claim:Overview.Build`: In the overview, the model builds the skills; a project's init turns memory off and commits the guard.

Spec: K6, N12 DR 055 §1, N12 DR 056 §1

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ClaimArea → HostArea, "the model builds the skills; a project's init turns memory off and commits the guard". A system transition: no skill carries it.

## Hold

`claim:Overview.Hold`: In the overview, a coordinator holds a live Project.

Spec: H4

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): HostArea → ProjectArea, "a coordinator holds a live Project". A system transition: no skill carries it.

## Cut

`claim:Overview.Cut`: In the overview, a Wayfinding captain cuts the ticket; the coordinator opens, sequences and dispatches its captain.

Spec: H2, H4, H5, H3

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProjectArea → TicketArea, "a Wayfinding captain cuts the ticket; the coordinator opens, sequences and dispatches its captain". A system transition: no skill carries it.

## Prove

`claim:Overview.Prove`: In the overview, the Implement hop proves its work with red tests and receipts.

Spec: R5, T3

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): TicketArea → ProofArea, "the Implement hop proves its work with red tests and receipts". A system transition: no skill carries it.

## Review

`claim:Overview.Review`: In the overview, the parts are wired; the review opens.

Spec: R2

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProofArea → ReviewArea, "the parts are wired; the review opens". A system transition: no skill carries it.

## Merge

`claim:Overview.Merge`: In the overview, the first round with no new blocker: the Merge hop runs dydo merge.

Spec: R8, R17, R23

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ReviewArea → MergeArea, "the first round with no new blocker: the Merge hop runs dydo merge". A system transition: no skill carries it.

## Next

`claim:Overview.Next`: In the overview, merged: the coordinator dispatches the next ticket in sequence.

Spec: R20, H5

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): MergeArea → ProjectArea, "merged: the coordinator dispatches the next ticket in sequence". A system transition: no skill carries it.

## Land

`claim:Overview.Land`: In the overview, every delivery ticket merged: the landing runs.

Spec: N5

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProjectArea → LandingArea, "every delivery ticket merged: the landing runs". A system transition: no skill carries it.

## Accept

`claim:Overview.Accept`: In the overview, his acceptance: the landing PR goes to master.

Spec: N6

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): LandingArea → MasterArea, "his acceptance: the landing PR goes to master". A system transition: no skill carries it.

## Tag

`claim:Overview.Tag`: In the overview, Balazs pushes the release tag.

Spec: R28, N2

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): MasterArea → Tagged, "Balazs pushes the release tag". A system transition: no skill carries it.

## Ship

`claim:Overview.Ship`: In the overview, adopters take one ordinary update.

Spec: K18, N8

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): Tagged → AdopterArea, "adopters take one ordinary update". A system transition: no skill carries it.

## Explain

`claim:Overview.Explain`: In the overview, a change to declared paths flags its doc.

Spec: D5

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProofArea → DocsArea, "a change to declared paths flags its doc". A system transition: no skill carries it.

## Explained

`claim:Overview.Explained`: In the overview, the writer updates the doc or states it holds.

Spec: D5

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): DocsArea → ProofArea, "the writer updates the doc or states it holds". A system transition: no skill carries it.

## Watch

`claim:Overview.Watch`: In the overview, the board, the map and liveness are read.

Spec: S19, S22, S24

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProjectArea → WatchArea, "the board, the map and liveness are read". A system transition: no skill carries it.

## Route

`claim:Overview.Route`: In the overview, findings and CI reds go to their officer.

Spec: Y9, H11, H5

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): WatchArea → ProjectArea, "findings and CI reds go to their officer". A system transition: no skill carries it.

## Need

`claim:Overview.Need`: In the overview, a need only he can settle goes up the chain, or a hard call to the tribunal.

Spec: H16, L18

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProjectArea → JudgementArea, "a need only he can settle goes up the chain, or a hard call to the tribunal". A system transition: no skill carries it.

## Decided

`claim:Overview.Decided`: In the overview, a Question decided, by him by day or provisionally overnight, is acted on and never ratified.

Spec: L19, L20

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): JudgementArea → ProjectArea, "a Question decided, by him by day or provisionally overnight, is acted on and never ratified". A system transition: no skill carries it.

## Proposed

`claim:Overview.Proposed`: In the overview, overnight, both judges agree on a two-way, seriously blocking DR-grade call: a proposed DR.

Spec: L21

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): JudgementArea → StaffArea, "overnight, both judges agree on a two-way, seriously blocking DR-grade call: a proposed DR". A system transition: no skill carries it.

## Ratify

`claim:Overview.Ratify`: In the overview, he ratifies the proposed DR.

Spec: L21

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): StaffArea → ProjectArea, "he ratifies the proposed DR". A system transition: no skill carries it.

## List

`claim:Overview.List`: In the overview, the chief of staff reads the board across Projects into his short list.

Spec: H7

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): ProjectArea → StaffArea, "the chief of staff reads the board across Projects into his short list". A system transition: no skill carries it.

## GiveOut

`claim:Overview.GiveOut`: In the overview, he gives out a ticket from that list, or tells the chief of staff to.

Spec: H7

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): StaffArea → ProjectArea, "he gives out a ticket from that list, or tells the chief of staff to". A system transition: no skill carries it.

## Refix

`claim:Overview.Refix`: In the overview, a landing round's blockers become Fix rN.

Spec: N5, S9

Transition in dydo 4.0.0 (`claims/statechart/overview/Overview.tla`): LandingArea → TicketArea, "a landing round's blockers become Fix rN". A system transition: no skill carries it.

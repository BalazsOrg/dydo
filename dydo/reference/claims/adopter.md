---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Adopter

## Update

`claim:Adopter.Update`: An adopter's update: one ordinary update with an Upgrade note, through the update-dydo wizard.

Spec: K18, N8

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Released → Updating, "one ordinary update with an Upgrade note, through the update-dydo wizard". A system transition: no skill carries it.

## Tailored

`claim:Adopter.Tailored`: An adopter's update: a shipped file the project keeps tailored, docs included.

Spec: N11

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Updating → Tailored, "a shipped file the project keeps tailored, docs included". A system transition: no skill carries it.

## Consult

`claim:Adopter.Consult`: An adopter's update: update-dydo consults the user.

Spec: N11

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Tailored → Consulted, "update-dydo consults the user". A system transition: no skill carries it.

## Decided

`claim:Adopter.Decided`: An adopter's update: the user decides what happens to it.

Spec: N11

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Consulted → Updating, "the user decides what happens to it". A system transition: no skill carries it.

## Major

`claim:Adopter.Major`: An adopter's update: 4.0.0: statuses deleted, DYTEST a prerequisite, the rebuilt skills swapped in.

Spec: N1, N9, Y5

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Updating → Major, "4.0.0: statuses deleted, DYTEST a prerequisite, the rebuilt skills swapped in". A system transition: no skill carries it.

## Swapped

`claim:Adopter.Swapped`: An adopter's update: dydo's law read-only at law/dydo/, the two-law template, descriptive-doc rules on by default.

Spec: N8, L2, D1

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Major → Current, "dydo's law read-only at law/dydo/, the two-law template, descriptive-doc rules on by default". A system transition: no skill carries it.

## Done

`claim:Adopter.Done`: An adopter's update: updated; later builds stay line-stable, one claim per bullet.

Spec: K18

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Updating → Current, "updated; later builds stay line-stable, one claim per bullet". A system transition: no skill carries it.

## Harvest

`claim:Adopter.Harvest`: An adopter's update: a release is planned: its map holder harvests their Agent friction queues and local patches.

Spec: H9

Transition in [The adopter update](../../../claims/statechart/adopter/Adopter.tla): Current → Harvested, "a release is planned: its map holder harvests their Agent friction queues and local patches". A system transition: no skill carries it.

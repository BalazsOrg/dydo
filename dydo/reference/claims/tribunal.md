---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Tribunal

## Settled

`claim:Tribunal.Settled`: A call a DR clearly settles is not hard: the holder decides and cites the DR.

Spec: L18

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): HardCall → DrSettled, "a DR clearly settles it".

**admiral** (HardCall, exit):

> A call a DR clearly settles is not hard: decide it and cite the DR.

## File

`claim:Tribunal.File`: A hard call goes to a fresh clerk with a neutral case file; the admiral is no member.

Spec: L18

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): HardCall → CaseFile, "hard: a fresh clerk takes the Question".

**admiral** (HardCall, exit):

> Give a fresh clerk the Question with only the call, the options unranked, the constraints and the primary facts with sources; you sit on no tribunal.

## Seat

`claim:Tribunal.Seat`: The clerk seats two judges from different families from the roster.

Spec: L18, R13, R15

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): CaseFile → Sealed, "two judges from different families take sealed positions". A system transition: no skill carries it.

## Exchange

`claim:Tribunal.Exchange`: One exchange follows: each answers the other's strongest point or concedes.

Spec: L18

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Sealed → Exchange, "both positions in; one exchange". A system transition: no skill carries it.

## DecideNight

`claim:Tribunal.DecideNight`: Overnight, judges agreeing on a two-way hard call decide it, provisionally.

Spec: L18, L19, L20

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Exchange → DecidedOvernight, "overnight, the judges agree on a two-way hard call".

**admiral** (DecidedOvernight, arrival):

> Act on an overnight decision, citing its law and precedent; it stays provisional and is never ratified.

## Prepare

`claim:Tribunal.Prepare`: By day, on disagreement, or for a one-way or DR-grade call, the tribunal prepares it for him.

Spec: L18, L20, L21

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Exchange → Prepared, "by day, or they disagree, or it is one-way or DR-grade".

**admiral** (Prepared, arrival):

> Send a prepared call to him through an envoy, with both positions; he decides.

## ProposeDr

`claim:Tribunal.ProposeDr`: Overnight, a two-way, seriously blocking DR-grade call both judges agree on becomes a proposed DR.

Spec: L21

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Exchange → ProposedDr, "overnight, both agree on a two-way, seriously blocking DR-grade call".

**admiral** (ProposedDr, arrival):

> Write the overnight ruling as a `proposed` DR for his ratification; an irreversible call is never decided.

## Ratify

`claim:Tribunal.Ratify`: He ratifies an overnight proposed DR.

Spec: L21

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): ProposedDr → Ratified, "he ratifies it". A system transition: no skill carries it.

## Score

`claim:Tribunal.Score`: His daytime answers are scored against the prediction, as evidence only.

Spec: L20

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Prepared → Scored, "he answers; scored against the prediction". A system transition: no skill carries it.

## Wrong

`claim:Tribunal.Wrong`: A ruling found wrong is investigated: why it was made, then a measure.

Spec: L19

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): DecidedOvernight → Investigated, "the ruling is found wrong". A system transition: no skill carries it.

## OneFamily

`claim:Tribunal.OneFamily`: With one family out, Judge decides overnight and prepares by day on one family.

Spec: L22

**admiral** (HardCall, nominal):

> With one family out, the tribunal runs on the other: overnight it decides, by day it prepares.

## Clerk

`claim:Tribunal.Clerk`: The clerk holds the Question, runs it mechanically, keeps the case file neutral and seats two judges.

Spec: L18, L23

**reviewer** (CouncilJudge):

> As the clerk, hold the Question and run it mechanically: keep the case file neutral (the call, the options unranked, the constraints, the primary facts with their sources) and seat two judges from different families from the roster.

## Judge

`claim:Tribunal.Judge`: Each judge alone maps options to axioms, finds precedent, names his choice and covers two duties.

Spec: L18

**reviewer** (CouncilJudge):

> As a judge, alone: map each option to the axioms it serves, find precedent through `weighed:`, name what Balazs would choose and why, and answer both duties: should this be done at all, and is the frame too narrow?

## Axioms

`claim:Tribunal.Axioms`: Axioms are not questioned.

Spec: L4

**reviewer** (CouncilJudge):

> Take the axioms as given; never question them.

## Rebuttal

`claim:Tribunal.Rebuttal`: One exchange: each judge answers the other's strongest point or concedes.

Spec: L18

**reviewer** (CouncilJudge):

> Then one exchange: answer the other judge's strongest point, or concede.

## ClerkReturn

`claim:Tribunal.ClerkReturn`: The clerk returns both positions and the exchange to the admiral.

Spec: L18

**reviewer** (CouncilJudge):

> The clerk returns both positions and the exchange to the admiral.

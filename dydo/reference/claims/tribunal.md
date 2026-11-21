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

**ticket-captain** (legacy, not built; HardCall, exit):

> A call a DR clearly settles is not hard: decide it and cite the DR.

## HisCall

`claim:Tribunal.HisCall`: After its research, a call goes to him only when it is worth his time to understand, as a his-call child (DYD-1195 87c1e2e6).

Spec: L18, H23, H16

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): HardCall → Prepared, "worth his time to understand: a his-call child".

**ticket-captain** (legacy, not built; HardCall, exit):

> Open a `his-call` child only when the call is worth his time and attention to understand; a rubber-stamp question never reaches him.

## File

`claim:Tribunal.File`: A hard call not worth his time goes to a council child, by day or night (DYD-1195 87c1e2e6).

Spec: L18, L20, H23

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): HardCall → CaseFile, "a hard call not worth his time: a council child, by day or night".

**ticket-captain** (legacy, not built; HardCall, exit):

> Otherwise open a `council` child for a hard call, by day or night: a DR and the spec disagree, nuance may change a DR or the spec, or there is no clear answer.

## DrGrade

`claim:Tribunal.DrGrade`: A DR-grade call goes to him, except overnight, when a two-way, seriously blocking one goes to a council child.

Spec: L21, H23

**ticket-captain** (legacy, not built; HardCall, nominal):

> A `dr-grade` call goes to a `his-call` child, except overnight, when a two-way, seriously blocking one goes to a `council` child.

## Flag

`claim:Tribunal.Flag`: A captain that meets a DR-grade call flags it dr-grade on its ticket and passes it up.

Spec: L21, H16

**ticket-captain** (legacy, not built; NeedPassedUp):

> Flag a DR-grade call on your ticket with the `dr-grade` label, then pass it up like any other need.

## Clerk

`claim:Tribunal.Clerk`: The chair runs the tribunal mechanically and keeps its case file neutral.

Spec: L18, L23

**ticket-captain** (legacy, not built; Tribunal):

> Write the case file neutral: only the call, the options unranked, the constraints and the primary facts with their sources, never the research's tension mapping or chosen precedent.

## Seat

`claim:Tribunal.Seat`: The chair seats two judges from different families from the roster.

Spec: L18, R13, R15

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): CaseFile → Sealed, "two judges from different families take sealed positions".

**ticket-captain** (legacy, not built; CaseFile, exit):

> Seat two judges from different families, picked from the roster, each briefed as a judge from the council members' briefs (`council-members.md`).

## OneFamily

`claim:Tribunal.OneFamily`: With one family out, Judge runs on the remaining family: overnight it decides; by day it prepares a call worth his time, and a council decides one that is not.

Spec: L22, L18, L20

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): CaseFile → Sealed, "one family out: the judges sit on the remaining family".

**ticket-captain** (legacy, not built; CaseFile, exit):

> With one family out, seat both judges from the remaining family; the exits below hold as with two.

## Exchange

`claim:Tribunal.Exchange`: One exchange follows: each answers the other's strongest point or concedes.

Spec: L18

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Sealed → Exchange, "both positions in; one exchange". A system transition: no skill carries it.

## DecideNight

`claim:Tribunal.DecideNight`: Judges agreeing on a two-way hard call decide it, provisionally, overnight or by day for a call not worth his time (DYD-1195 87c1e2e6).

Spec: L18, L19, L20

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Exchange → DecidedOvernight, "the judges agree on a two-way hard call".

**ticket-captain** (legacy, not built; DecidedOvernight, arrival):

> When both judges agree on a two-way hard call that is not DR-grade, it is decided: record it citing its law and precedent; it is acted on, provisional, and never ratified.

## Prepare

`claim:Tribunal.Prepare`: On disagreement, or for a one-way or DR-grade call, the tribunal prepares it for him with both positions.

Spec: L18, L20, L21, H23

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Exchange → Prepared, "they disagree, or it is one-way or DR-grade".

**ticket-captain** (legacy, not built; Prepared, arrival):

> When they disagree, the precedent points both ways, or the call is one-way, return it prepared, with both positions, to the Question that opened the council, which sends it to him as a `his-call` child; a DR-grade call too, except the overnight one below.

## ProposeDr

`claim:Tribunal.ProposeDr`: Overnight, a two-way, seriously blocking DR-grade call both judges agree on becomes a proposed DR.

Spec: L21

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Exchange → ProposedDr, "overnight, both agree on a two-way, seriously blocking DR-grade call".

**ticket-captain** (legacy, not built; ProposedDr, arrival):

> Overnight, when both agree on a two-way, seriously blocking DR-grade call, write the ruling as a `proposed` DR for his ratification; an irreversible call is never decided.

## Ratify

`claim:Tribunal.Ratify`: He ratifies an overnight proposed DR.

Spec: L21

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): ProposedDr → Ratified, "he ratifies it". A system transition: no skill carries it.

## Decline

`claim:Tribunal.Decline`: A proposed DR he declines is deleted: his words go on the ticket that raised the question, which returns to its raiser (DYD-1082 1bc04dce, 555a8300 gap 8).

Spec: L21

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): ProposedDr → Declined, "he declines it: deleted, his words on the raising ticket".

**ticket-captain** (legacy, not built; ProposedDr, exit):

> When he declines a proposed DR, delete it, never keeping it beside the accepted DRs: put his decline and his words on the ticket that raised the question, which returns to its raiser.

## Score

`claim:Tribunal.Score`: His daytime answers are scored against a council's prediction, as evidence only.

Spec: L20

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): Prepared → Scored, "he answers; any council prediction is scored, as evidence only". A system transition: no skill carries it.

## Wrong

`claim:Tribunal.Wrong`: A ruling found wrong is investigated: why it was made, then a measure.

Spec: L19

Transition in The tribunal (`claims/statechart/tribunal/Tribunal.tla`): DecidedOvernight → Investigated, "the ruling is found wrong". A system transition: no skill carries it.

## JudgeSlots

`claim:Tribunal.JudgeSlots`: A judge's brief names every skill the seat needs with a slash, like any dispatch.

Spec: K14, L18

**ticket-captain** (legacy, not built; Judge):

> Skills: every skill the seat needs, each with a slash · the case file · model and effort · return to.

## Judge

`claim:Tribunal.Judge`: Each judge alone maps options to axioms, finds precedent, names his choice and covers two duties.

Spec: L18

**ticket-captain** (legacy, not built; Judge):

> Alone, map each option to the axioms it serves, find precedent through `weighed:`, name what Balazs would choose and why, and answer both duties: should this be done at all, and is the frame too narrow?

## Axioms

`claim:Tribunal.Axioms`: Axioms are not questioned.

Spec: L4

**ticket-captain** (legacy, not built; Judge):

> Take the axioms as given; never question them.

## Rebuttal

`claim:Tribunal.Rebuttal`: One exchange: each judge answers the other's strongest point or concedes.

Spec: L18

**ticket-captain** (legacy, not built; Judge):

> Then one exchange: answer the other judge's strongest point, or concede.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Council

## Hold

`claim:Council.Hold`: A ticket captain chairs a council with the council-chair resource (DYD-1082 99774812).

Spec: L23, H23, L18

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Research → Council, "a hard call not worth his time: a council child, AFK".

**ticket-captain** (Council, arrival):

> Holding a Question at stage `council`, chair it by the council chair (`resources/council-chair.md`).

## Ticket

`claim:Council.Ticket`: A council is a Question ticket at stage council, Mode AFK, with no Council Type; its chair is no member (DYD-1195 ea16ea28).

Spec: L23, L18, H23

**ticket-captain** (Council):

> A council is a Question ticket at stage `council` and Mode AFK, with no Council Type; you chair it mechanically and sit on it as no member.

## FamilyOut

`claim:Council.FamilyOut`: Merge confidence waits for both families only while nothing blocks on it.

Spec: L22

Transition in A council (`claims/statechart/council/Council.tla`): Convened → Waiting, "Merge confidence with one family out". A system transition: no skill carries it.

## Proceed

`claim:Council.Proceed`: Once something blocks on it, Merge confidence runs single-family, never escalated to him.

Spec: L22

Transition in A council (`claims/statechart/council/Council.tla`): Waiting → Answered, "something blocks: it runs on one family". A system transition: no skill carries it.

## Answer

`claim:Council.Answer`: Every member answers the same question alone, sealed and read-only.

Spec: L14, L15, L16

Transition in A council (`claims/statechart/council/Council.tla`): Convened → Answered, "every member answers alone, sealed". A system transition: no skill carries it.

## Synthesise

`claim:Council.Synthesise`: A non-member synthesises the answer, its confidence and the disagreements.

Spec: L14

Transition in A council (`claims/statechart/council/Council.tla`): Answered → Synthesised, "a non-member synthesises". A system transition: no skill carries it.

## Uses

`claim:Council.Uses`: A council has four uses; the Evaluate use keeps the /10 score.

Spec: L16, L23

**ticket-captain** (Council):

> It has four uses: Review (high stakes), Merge confidence (the landing into main), Evaluate (hill-climbs, scored out of 10) and Judge (the tribunal, below).

## Seating

`claim:Council.Seating`: The chair seats fresh members from different families from the roster, and a non-member synthesiser.

Spec: L14, L15, R13, R15

**ticket-captain** (Council):

> Seat fresh members from different families, picked from the roster, and a synthesiser who is no member, each briefed from the council members' briefs (`council-members.md`).

## Once

`claim:Council.Once`: Deterministic work runs once.

Spec: L16

**ticket-captain** (Council):

> Run deterministic work once for the whole council, never per member.

## Record

`claim:Council.Record`: The council's deliberation is recorded on its ticket next to the answer, merge confidence and scores included (DYD-1195 ea16ea28).

Spec: L23

**ticket-captain** (Council):

> Record the deliberation on the council ticket next to its answer; merge confidence and a score are answers too.

## Seats

`claim:Council.Seats`: The verdict names each seat.

Spec: S8, L22

**ticket-captain** (Council):

> The answer names each seat's model and provider.

## OneFamily

`claim:Council.OneFamily`: With one family out, work goes on: Review and Evaluate run on two models of the remaining family.

Spec: L22, L15

**ticket-captain** (Council):

> With one family out, work goes on: Review and Evaluate run on two different models of the remaining family.

## MergeOneFamily

`claim:Council.MergeOneFamily`: Merge confidence waits for both families only while nothing blocks, then runs on one, never escalated.

Spec: L22

**ticket-captain** (Council):

> With one family out, Merge confidence at the landing waits for both families only while nothing blocks on it, then runs on one; never take a missing family to him.

## Member

`claim:Council.Member`: A member answers alone, sealed and read-only, on the same thing as every other member.

Spec: L14, L16

**ticket-captain** (Member):

> Answer the one question alone, sealed and read-only; every member looks at the same thing.

## Synthesis

`claim:Council.Synthesis`: The synthesiser is a non-member and gives answer, confidence and disagreements.

Spec: L14

**ticket-captain** (Synthesiser):

> As a non-member, give the answer, its confidence (agreed, split or contested) and the disagreements.

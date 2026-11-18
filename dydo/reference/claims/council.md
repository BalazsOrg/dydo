---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Council

## FamilyOut

`claim:Council.FamilyOut`: Merge confidence waits for both families only while nothing blocks on it.

Spec: L22

Transition in [A council](../../../claims/statechart/council/Council.tla): Convened → Waiting, "Merge confidence with one family out". A system transition: no skill carries it.

## Proceed

`claim:Council.Proceed`: Once something blocks on it, Merge confidence runs single-family, never escalated to him.

Spec: L22

Transition in [A council](../../../claims/statechart/council/Council.tla): Waiting → Answered, "something blocks: it runs on one family". A system transition: no skill carries it.

## Answer

`claim:Council.Answer`: Every member answers the same question alone, sealed and read-only.

Spec: L14, L15, L16

Transition in [A council](../../../claims/statechart/council/Council.tla): Convened → Answered, "every member answers alone, sealed". A system transition: no skill carries it.

## Synthesise

`claim:Council.Synthesise`: A non-member synthesises the answer, its confidence and the disagreements.

Spec: L14

Transition in [A council](../../../claims/statechart/council/Council.tla): Answered → Synthesised, "a non-member synthesises". A system transition: no skill carries it.

## Uses

`claim:Council.Uses`: A council has four uses; the Evaluate use keeps the /10 score.

Spec: L16, L23

**reviewer** (CouncilUses):

> A council has four uses: Review (high stakes), Merge confidence (the landing into main), Evaluate (hill-climbs, scored out of 10) and Judge (the tribunal).

## Once

`claim:Council.Once`: Deterministic work runs once.

Spec: L16

**reviewer** (CouncilUses):

> Deterministic work runs once for the whole council, never per member.

## Member

`claim:Council.Member`: A member answers alone, sealed and read-only, on the same thing as every other member.

Spec: L14, L16

**reviewer** (CouncilMember):

> As a member, answer the one question alone, sealed and read-only; every member looks at the same thing.

## Synthesis

`claim:Council.Synthesis`: The synthesiser is a non-member and gives answer, confidence and disagreements.

Spec: L14

**reviewer** (CouncilSynthesis):

> As the synthesiser, a non-member, give the answer, its confidence (agreed, split or contested) and the disagreements.

## Seats

`claim:Council.Seats`: The verdict names each seat.

Spec: S8, L22

**reviewer** (CouncilSynthesis):

> The verdict names each seat's model and provider.

## OneFamily

`claim:Council.OneFamily`: With one family out, work goes on and the council runs on what is left.

Spec: L22, L15

**reviewer** (CouncilOneFamily):

> With one family out, Review and Evaluate run on two different models of the remaining family, and Judge runs on one family.

## MergeOneFamily

`claim:Council.MergeOneFamily`: Merge confidence waits for both families only while nothing blocks, then runs on one, never escalated.

Spec: L22

**admiral** (MergeReview, nominal):

> With one family out, Merge confidence waits for both families only while nothing blocks on it, then runs on one; never take a missing family to him.

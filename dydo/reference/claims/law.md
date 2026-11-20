---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Law

## Departs

`claim:Law.Departs`: An answer of his that departs from his precedent or his law is an amendment candidate.

Spec: L7, L11

Transition in How the law grows (`claims/statechart/law/Law.tla`): Evidence → Candidate, "his answer departs from his precedent or his law, or other evidence arrives". A system transition: no skill carries it.

## Mechanical

`claim:Law.Mechanical`: What no reasonable person would choose otherwise becomes a test, lint or gate.

Spec: L7, L13

Transition in How the law grows (`claims/statechart/law/Law.tla`): Candidate → Mechanical, "no reasonable person would choose otherwise". A system transition: no skill carries it.

## Propose

`claim:Law.Propose`: A choice a reasonable person could make otherwise is a principle, proposed to him in a batch.

Spec: L7, L13

Transition in How the law grows (`claims/statechart/law/Law.tla`): Candidate → Proposed, "a reasonable person could choose otherwise and still be right". A system transition: no skill carries it.

## Enact

`claim:Law.Enact`: Principles and axioms are his only; a slug is never renamed or reused.

Spec: L13, L1, L2, L3, L5, L6, L10

Transition in How the law grows (`claims/statechart/law/Law.tla`): Proposed → Enacted, "he enacts it under a new slug". A system transition: no skill carries it.

## Retire

`claim:Law.Retire`: A retired or superseded principle moves to retired.md with its reason.

Spec: L11, L13

Transition in How the law grows (`claims/statechart/law/Law.tla`): Enacted → Retired, "he retires or supersedes it". A system transition: no skill carries it.

## Precedential

`claim:Law.Precedential`: Agents prepare a case study when a call looks precedential.

Spec: L9, L8

Transition in How the law grows (`claims/statechart/law/Law.tla`): Evidence → CaseStudy, "a call looks precedential". A system transition: no skill carries it.

## Approve

`claim:Law.Approve`: He approves a case study; nothing is generated.

Spec: L9, L4

Transition in How the law grows (`claims/statechart/law/Law.tla`): CaseStudy → CaseLaw, "he approves it". A system transition: no skill carries it.

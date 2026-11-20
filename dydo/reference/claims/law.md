---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Law

## Departs

`claim:Law.Departs`: An answer of his that departs from his precedent or his law is an amendment candidate.

Spec: L7, L11

Transition in How the law grows (`claims/statechart/law/Law.tla`): Evidence → Candidate, "his answer departs from his precedent or his law, or other evidence arrives".

**coordinator** (Evidence, exit):

> Treat an answer of his that departs from his precedent or his law as an amendment candidate, never noise.

## Mechanical

`claim:Law.Mechanical`: What no reasonable person would choose otherwise becomes a test, lint or gate.

Spec: L7, L13

Transition in How the law grows (`claims/statechart/law/Law.tla`): Candidate → Mechanical, "no reasonable person would choose otherwise".

**coordinator** (Candidate, exit):

> If no reasonable person could choose otherwise, make it a test, lint or gate; those are the agents'.

## Propose

`claim:Law.Propose`: A choice a reasonable person could make otherwise is a principle, proposed to him in a batch.

Spec: L7, L13

Transition in How the law grows (`claims/statechart/law/Law.tla`): Candidate → Proposed, "a reasonable person could choose otherwise and still be right".

**coordinator** (Candidate, exit):

> If a reasonable person could choose otherwise and still be right, propose a principle; batch proposals per Project into a Milestone walkthrough or one envoy bundle.

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

Transition in How the law grows (`claims/statechart/law/Law.tla`): Evidence → CaseStudy, "a call looks precedential".

**coordinator** (Evidence, exit):

> When a call looks precedential, prepare a case study in git under `cases/` linking its Linear origin, for his approval.

## Approve

`claim:Law.Approve`: He approves a case study; nothing is generated.

Spec: L9, L4

Transition in How the law grows (`claims/statechart/law/Law.tla`): CaseStudy → CaseLaw, "he approves it". A system transition: no skill carries it.

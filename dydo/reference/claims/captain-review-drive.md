---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReviewDrive

## Driver

`claim:CaptainReviewDrive.Driver`: A driver drives through the project's verification skill, in the order of that skill's own sections.

Spec: R34

Evidence (run): `skills/engineering/verify-dydo/SKILL.md`, this repository's, holds those five sections under those names. `skills/engineering/create-verification-skill/SKILL.md` writes every project's as `verify-<app>` with the same five, and leaves it model-invoked: "implementers and reviewers load it mid-task".

Layer: How an app is started, driven and torn down is known only to the skill written for that app.

Reason: A driver that improvises its launch tests an instance nobody would recognise, and strands processes when it leaves.

**ticket-captain** (ReviewDrive; read by TicketCaptain):

> Drive through your project's verification skill, the one your brief names, whose name starts with `verify-`: follow its Launch, Doctor, Drive, Evidence and Cleanup sections, in that order.

## User

`claim:CaptainReviewDrive.User`: A driver uses the feature the way its user would, by every route the feature map lists, and says for each whether it works as the spec's problem statement intends.

Spec: R34, R3

Evidence (run): `skills/engineering/create-verification-skill/SKILL.md`: "The map is the repo's maintained verification source; a proof that drives one convenient entry point is incomplete when the map lists others." `skills/engineering/verify-dydo/SKILL.md` opens on its map, `resources/features/README.md`.

Layer: A suite exercises the seams its author chose, and nothing in it walks the product the way the person it was built for will.

Reason: A feature can pass every test and still fail the first person who tries to use it.

**ticket-captain** (ReviewDrive; read by TicketCaptain):

> Use the feature the way its user would, by every route that skill's feature map lists for it, and say for each whether it works as the spec's problem statement intends.

## Steps

`claim:CaptainReviewDrive.Steps`: A driver returns each journey as steps anyone can rerun, with what it observed beside what the spec expects; only what it observed itself is proof.

Spec: R5, R34

Evidence (run): `skills/engineering/maintain-verification-skill/SKILL.md`, Proof: "evidence you observed yourself in this session: command output, a reproduction, a screenshot. Green mocked tests do not prove that the user experience works."

Layer: A driving skill keeps raw evidence, and it is the driver who has to say which step went wrong and what was expected there.

Reason: The Fix's writer reproduces the fault from the steps alone, without asking the driver what it did.

**ticket-captain** (ReviewDrive; read by TicketCaptain):

> Return each journey as steps anyone can rerun, with what you observed beside what the spec expects; only what you observed yourself, in this session, is proof.

## Gap

`claim:CaptainReviewDrive.Gap`: What a user can do and the verification skill cannot drive is a finding to return, never a driver to build, and a driver changes no product code.

Spec: R34

Evidence (run): `skills/engineering/verify-dydo/SKILL.md`: "report product regressions without changing production code during the proof pass." `skills/engineering/maintain-verification-skill/SKILL.md` names such a hole a verification gap, of severity should, with its own upkeep pass.

Layer: A verification skill covers the journeys someone mapped, and says nothing about the ones nobody did.

Reason: A seat that extends the harness in the middle of a review returns late, and with a driver nobody reviewed.

**ticket-captain** (ReviewDrive; read by TicketCaptain):

> What a user can do and the skill cannot drive is a finding to return, never a driver to build; change no product code while you drive.

## Due

`claim:CaptainReviewDrive.Due`: Driving is due only where the verification skill is built and covers the feature, and the feature is settled.

Spec: R34

Evidence (run): `skills/engineering/verify-dydo/SKILL.md`, read 2026-11-21, covers this repository's CLI on Windows only: "Other platforms fail before child launch", and "the viewer, live officer workflows and pending release prerequisites need their own proof". So a seat on this Linux host finds driving not due, and returns that.

Layer: The table's "due" in the Drive column cannot know whether a project's driver exists, or reaches this feature on this host.

Reason: Driving by hand what no skill covers gives a result nobody can repeat, on a feature that may still change.

**ticket-captain** (ReviewDrive; read by TicketCaptain):

> Driving is due only where that skill is built and covers the feature, and the feature is settled; where it is not, return that as your finding.

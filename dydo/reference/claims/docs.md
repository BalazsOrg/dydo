---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Docs

## New

`claim:Docs.New`: A descriptive doc: a new descriptive doc is proposed.

Spec: D4, D1

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Start → Proposed, "a new descriptive doc is proposed". A system transition: no skill carries it.

## Existing

`claim:Docs.Existing`: A descriptive doc: an existing descriptive doc is grandfathered.

Spec: D7

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Start → Grandfathered, "an existing descriptive doc is grandfathered". A system transition: no skill carries it.

## Uninvited

`claim:Docs.Uninvited`: A descriptive doc: no invitation, or an architecture plan: not written.

Spec: D4, D8

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Proposed → NotWritten, "no invitation, or an architecture plan: not written". A system transition: no skill carries it.

## Invite

`claim:Docs.Invite`: A descriptive doc: he invites it.

Spec: D4

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Proposed → Invited, "he invites it". A system transition: no skill carries it.

## Admit

`claim:Docs.Admit`: A descriptive doc: solid, important and expensive to rebuild from code.

Spec: D3

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Invited → Admitted, "solid, important and expensive to rebuild from code". A system transition: no skill carries it.

## Refuse

`claim:Docs.Refuse`: A descriptive doc: it fails one of the three tests.

Spec: D3

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Invited → NotWritten, "it fails one of the three tests". A system transition: no skill carries it.

## Declare

`claim:Docs.Declare`: A descriptive doc: written, declaring the code it explains in explains:.

Spec: D5, D2

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Admitted → Declared, "written, declaring the code it explains in explains:". A system transition: no skill carries it.

## Flag

`claim:Docs.Flag`: A descriptive doc: a change to its paths: dydo check and pr-ready flag it.

Spec: D5

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Declared → Flagged, "a change to its paths: dydo check and pr-ready flag it". A system transition: no skill carries it.

## Hold

`claim:Docs.Hold`: A descriptive doc: the writer updates the doc or states it holds.

Spec: D5

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Flagged → Declared, "the writer updates the doc or states it holds". A system transition: no skill carries it.

## ReviewFinds

`claim:Docs.ReviewFinds`: A descriptive doc: review finds an unlinked why, or a declared path changed without the doc checked.

Spec: D6, D2

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Declared → Finding, "review finds an unlinked why, or a declared path changed without the doc checked". A system transition: no skill carries it.

## FixFinding

`claim:Docs.FixFinding`: A descriptive doc: the finding is fixed.

Spec: D6

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Finding → Declared, "the finding is fixed". A system transition: no skill carries it.

## GivePaths

`claim:Docs.GivePaths`: A descriptive doc: one mechanical ticket gives it its declared paths.

Spec: D7

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Grandfathered → Declared, "one mechanical ticket gives it its declared paths". A system transition: no skill carries it.

## FixDrifted

`claim:Docs.FixDrifted`: A descriptive doc: found drifted or with an unlinked reason: fixed.

Spec: D7

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Grandfathered → Declared, "found drifted or with an unlinked reason: fixed". A system transition: no skill carries it.

## CutDrifted

`claim:Docs.CutDrifted`: A descriptive doc: found drifted or with an unlinked reason: cut.

Spec: D7

Transition in Descriptive docs (`claims/statechart/docs/Docs.tla`): Grandfathered → CutDoc, "found drifted or with an unlinked reason: cut". A system transition: no skill carries it.

## Explain

`claim:Docs.Explain`: A descriptive doc explains and never authorizes; a reason counts only when it links a DR or principle.

Spec: D2

**implementer** (Working, nominal):

> A descriptive doc explains and never authorizes: on what the code does, the code wins; a reason counts only when it links a DR or a principle.

## InviteOnly

`claim:Docs.InviteOnly`: Descriptive docs are invite-only and pass the admission test.

Spec: D4, D3

**implementer** (Working, nominal):

> Write a new descriptive doc only when invited, and only if its subject is solid, important and expensive to rebuild from code.

## Explains

`claim:Docs.Explains`: A change to a doc's declared paths: the writer updates the doc or states it holds.

Spec: D5

**implementer** (Working, nominal):

> When your change touches the paths a doc's `explains:` declares, update the doc or state that it holds.

## DrFields

`claim:Docs.DrFields`: Every new DR carries applies: or new-ground, and weighed: per tension it resolved.

Spec: L8

**implementer** (Working, nominal):

> A new DR you write carries `applies:` (or `new-ground`) and one `weighed: <slug> over <slug>, because <reason>` for each tension it resolved.

## Rubric

`claim:Docs.Rubric`: Review gains two lines: an unlinked why is a finding; a change to declared paths means checking that doc.

Spec: D6

**reviewer** (Round, nominal):

> On docs, an unlinked 'why' is a finding; on code, a change to paths a doc's `explains:` declares means checking that doc.

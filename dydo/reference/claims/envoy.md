---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Envoy

## Take

`claim:Envoy.Take`: The officer above takes a need for Balazs and does the homework first.

Spec: H16

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Need → Homework, "the officer above takes the need".

**ticket-captain** (Homework, arrival):

> A need for Balazs from below: do the homework first in docs, code, Linear, DRs and the spec.

**admiral** (Homework, arrival):

> A need for Balazs, yours or passed up to you: do the homework first in docs, code, Linear, DRs and the spec.

## Decide

`claim:Envoy.Decide`: An obvious, reversible call, or one the homework or a DR settles, is decided by the officer.

Spec: H16, L18

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Homework → Decided, "the homework answers it, or the call is obvious and reversible".

**ticket-captain** (Homework, exit):

> Decide it yourself when the homework answers it, a DR clearly settles it, or the call is obvious and reversible.

**admiral** (Homework, exit):

> Decide it yourself when the homework answers it, a DR clearly settles it (cite it), or the call is obvious and reversible.

## PassUp

`claim:Envoy.PassUp`: Captains pass their needs up the chain; only the admiral sends envoys.

Spec: H16, H12

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Homework → PassedUp, "a captain passes what remains up the chain".

**ticket-captain** (Homework, exit):

> Pass what remains up the chain to your officer; only the admiral sends envoys.

## NextLevel

`claim:Envoy.NextLevel`: The officer above takes the need passed up.

Spec: H12

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): PassedUp → Homework, "the officer above takes it". A system transition: no skill carries it.

## InThread

`claim:Envoy.InThread`: When he is already in an officer's thread, it may ask one line there with the host question tool.

Spec: H22, H6

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Homework → AskedInThread, "he is already in this officer's thread".

**ticket-captain** (Homework, exit):

> When he is already in your thread, ask the one line there with the host question tool.

**admiral** (Homework, exit):

> When he is already in your thread, ask the one line there with the host question tool.

## Hard

`claim:Envoy.Hard`: Hard calls go to the council: the tribunal.

Spec: H5, L18

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Homework → ToTribunal, "a hard call: competing answers, each good through some lens".

**admiral** (Homework, exit):

> Send a hard call, one with competing answers each good through some lens, to the tribunal.

## Open

`claim:Envoy.Open`: What only he can settle becomes an envoy job's ticket; a Question blocks only what waits on it.

Spec: H15, H17, S10

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Homework → Queued, "only he can settle it".

**admiral** (Homework, exit):

> For what only he can settle, open a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.

**linear-workspace-standard** (LinearTickets):

> A need for Balazs becomes a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.

## Send

`claim:Envoy.Send`: One active envoy per admiral; the next goes once the last reported and he closed its thread.

Spec: H18, H19, H23

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Queued → Active, "the last envoy reported and he closed its thread".

**admiral** (Queued, exit):

> Dispatch the next envoy only once the last has reported and he has closed its thread; when he has plainly moved on without closing it, use your discretion.

## Report

`claim:Envoy.Report`: The envoy records the result on its ticket and reports back.

Spec: H15, H19

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Active → Reported, "the envoy records the result and reports back".

**admiral** (Reported, arrival):

> Read the envoy's result from its ticket.

## Close

`claim:Envoy.Close`: After a shared understanding he closes the envoy's thread.

Spec: H19, H18

Transition in [A need for Balazs](../../../claims/statechart/envoy/Envoy.tla): Reported → Closed, "he closes the envoy's thread". A system transition: no skill carries it.

## Enablement

`claim:Envoy.Enablement`: What only he can do becomes an Enablement; a setup procedure goes through wizard.

Spec: H17

**admiral** (Homework, nominal):

> What only he can do, a token or an authorisation, becomes an Enablement ticket; a setup procedure goes through `wizard`.

## OwnEnvoy

`claim:Envoy.OwnEnvoy`: An escalation that needs him goes from the admiral through its own envoy, never through the chief of staff.

Spec: H12

**admiral** (Homework, nominal):

> An escalation that needs him goes out through your own envoy, never through the chief of staff.

**chief-of-staff** (Day, nominal):

> An escalation that needs him reaches him through its admiral's own envoy, never through you.

## Bundle

`claim:Envoy.Bundle`: The queue behind the active envoy is triaged at the admiral's discretion, bundled per origin.

Spec: H18

**admiral** (Queued, nominal):

> Triage the queue behind the active envoy at your discretion, bundling needs from one origin into one envoy.

## Template

`claim:Envoy.Template`: Each envoy is briefed from the envoy brief and loads its job's skill.

Spec: H19, H23

**admiral** (Active, nominal):

> Brief each envoy from [the envoy brief](resources/envoy-brief.md), with its job's skill from the table there.

## SlotTicket

`claim:Envoy.SlotTicket`: The envoy brief's slot 1.

Spec: H19

**admiral** (EnvoySlots):

> The ticket, and why it needs him.

## SlotSettled

`claim:Envoy.SlotSettled`: The envoy brief's slot 2.

Spec: H19

**admiral** (EnvoySlots):

> What's settled.

## SlotSources

`claim:Envoy.SlotSources`: The envoy brief's slot 3.

Spec: H19

**admiral** (EnvoySlots):

> Sources to read first.

## SlotStop

`claim:Envoy.SlotStop`: The envoy brief's slot 4.

Spec: H19

**admiral** (EnvoySlots):

> The stop line.

## SlotRecord

`claim:Envoy.SlotRecord`: The envoy brief's slot 5.

Spec: H19

**admiral** (EnvoySlots):

> Where to record, in his words.

## SlotReturn

`claim:Envoy.SlotReturn`: The envoy brief's slot 6.

Spec: H19

**admiral** (EnvoySlots):

> Return and closing summary.

## SlotOpening

`claim:Envoy.SlotOpening`: The envoy brief's slot 7.

Spec: H19

**admiral** (EnvoySlots):

> The opening rule: a fresh 2–3-line picture, then the first question with its recommendation.

## Updates

`claim:Envoy.Updates`: Changes during a session reach the envoy as dispatcher updates; it never asks him to repeat them.

Spec: H19

**admiral** (EnvoySession):

> Changes during the session reach you as dispatcher updates; never ask him to repeat them.

## NameDrs

`claim:Envoy.NameDrs`: A question put to him names what each DR says, never only its number.

Spec: H24

**admiral** (EnvoySession):

> Name what each DR says whenever you cite one to him, never only its number.

**ticket-captain** (Homework, nominal):

> Name what each DR says whenever you cite one to him, never only its number.

**chief-of-staff** (Day, nominal):

> Name what each DR says whenever you cite one to him, never only its number.

## CloseSignal

`claim:Envoy.CloseSignal`: After a shared understanding, and once it has reported back, the envoy tells him the thread can be closed.

Spec: H19, H23

**admiral** (EnvoySession):

> After a shared understanding, and once you have reported back, tell him the thread can be closed.

## SkillTable

`claim:Envoy.SkillTable`: Each envoy job loads a fixed skill.

Spec: H23

**admiral** (EnvoySkills):

> | Job | Skill |
> |---|---|
> | Grilling | `grilling` |
> | Walkthrough | `walkthrough` |
> | Wayfinding | `wayfinder` |
> | Question | `co-thinker` |
> | Enablement | `wizard`; when it is not a setup procedure he runs (credentials, cutover), just ask |

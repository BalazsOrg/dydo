---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Captain

## Placeholder

`claim:Captain.Placeholder`: The built ticket-captain skill is a placeholder until DYD-1372 hop 2.

Spec: K9

Evidence (code): `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs`

Layer: Nothing below the skill tells an agent who loads it that the process is missing.

Reason: An agent who took an empty skill for the whole process would improvise one.

**ticket-captain** (Job; read by TicketCaptain):

> This skill is a placeholder: the ticket captain's process is not written here until DYD-1372 hop 2 lands.

## Brief

`claim:Captain.Brief`: Until the prototype lands, a ticket captain works from its dispatch brief alone.

Spec: K14

Evidence (code): `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs`

Layer: The dispatch delivers the brief, but only this skill can say that it adds nothing.

Reason: Keeps the agent on the one source it does have, instead of hunting for more.

**ticket-captain** (Hold; read by TicketCaptain):

> Work from the brief you were dispatched with; this skill adds no step to it yet.

## Report

`claim:Captain.Report`: What the placeholder leaves open goes to the dispatcher.

Spec: K9

Evidence (code): `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs`

Layer: No tool routes a process question; the agent has to know its address.

Reason: A question with a known address gets asked, not guessed.

**ticket-captain** (WhenThingsGoWrong; read by TicketCaptain):

> Take anything this placeholder leaves open to whoever dispatched you.

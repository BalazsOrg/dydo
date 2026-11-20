---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Envoy

## Decide

`claim:Envoy.Decide`: An obvious, reversible call is decided where it arises and never becomes a ticket; the coordinator makes none (DYD-1195 3a854e37).

Spec: H16, H5

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Need → Decided, "the call is obvious and reversible: decided where it arises".

**ticket-captain** (Need, exit):

> Decide an obvious, reversible call yourself, where it arises; it never becomes a ticket.

## PassUp

`claim:Envoy.PassUp`: Any other need is passed up, at every level: its officer, a captain or the coordinator, opens a research Question for it, held by a fresh captain, and waits on it; only the coordinator dispatches envoys (DYD-1195 317eb49f, 3a854e37).

Spec: H16, H12, H23

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Need → Research, "passed up: its officer opens a research Question and waits on it".

**ticket-captain** (Need, exit):

> Pass any other need up to your officer, who opens a `research` Question for it; only the coordinator dispatches envoys.

## InThread

`claim:Envoy.InThread`: When he is already in an officer's thread, it may ask one line there with the host question tool.

Spec: H22, H6

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Need → AskedInThread, "he is already in this officer's thread".

**ticket-captain** (Need, exit):

> When he is already in your thread, ask the one line there with the host question tool.

## Open

`claim:Envoy.Open`: What only he can settle reaches the coordinator's HITL queue as its own HITL ticket, blocking only what waits on it: the his-call child a research Question opens when the call is worth his time, a Grilling, Walkthrough or Wayfinding, or an Enablement (DYD-1195 317eb49f).

Spec: H23, H16, H15, H17, S10

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Research → Queued, "worth his time: its his-call child joins his HITL queue".

**coordinator** (Queued, arrival):

> A need for him arrives on your HITL queue as its own HITL ticket, a `his-call` Question, a Grilling, Walkthrough or Wayfinding, or an Enablement for what only he can do, a token or an authorisation; each carries the blocking relations of the work that waits on it.

**linear-workspace-standard** (LinearTickets):

> A need for Balazs becomes a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.

## Send

`claim:Envoy.Send`: One active envoy per coordinator: the next goes onto the head of the queue only once the last has reported and he has closed its thread (DYD-1195 893e1f5b).

Spec: H18, H19, H23

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Queued → Active, "the last envoy reported and he closed its thread: the next goes onto the head of the queue".

**coordinator** (Queued, exit):

> Send the next envoy only once the last has reported and he has closed its thread.

## Report

`claim:Envoy.Report`: The envoy records the result on its ticket and reports back.

Spec: H15, H19

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Active → Reported, "the envoy records the result and reports back".

**coordinator** (Reported, arrival):

> When an envoy reports back, read its result from its ticket.

## Close

`claim:Envoy.Close`: He closes the envoy's thread once it has reported, after a shared understanding; no moved-on exit frees the queue (DYD-1195 893e1f5b).

Spec: H19, H18

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Reported → Closed, "he closes the envoy's thread". A system transition: no skill carries it.

## Enablement

`claim:Envoy.Enablement`: What only he can do becomes an Enablement.

Spec: H17

**coordinator** (Queued, arrival, on claim:Envoy.Open):

> A need for him arrives on your HITL queue as its own HITL ticket, a `his-call` Question, a Grilling, Walkthrough or Wayfinding, or an Enablement for what only he can do, a token or an authorisation; each carries the blocking relations of the work that waits on it.

## OwnEnvoy

`claim:Envoy.OwnEnvoy`: An escalation that needs him goes from the coordinator through its own envoy, never through the chief of staff.

Spec: H12

**coordinator** (Active, nominal):

> Everything that needs him goes out through your own envoy, never through the chief of staff.

**chief-of-staff** (Day, nominal, on claim:Staff.LinksOnly):

> Pass on thread links, never their content, and send no envoys: an escalation that needs him goes through its coordinator's own envoy.

## Bundle

`claim:Envoy.Bundle`: Every waiting need from one origin goes in one wrapper ticket the coordinator opens, which takes the members' place in the queue (DYD-1195 893e1f5b).

Spec: H18, L13

**coordinator** (Queued, nominal):

> Bundle every waiting need from one origin, the ticket whose captain passed them up: open a wrapper ticket, Type Question at stage `his-call` and Mode HITL, parent its member Questions and Enablements under it, and let it take their place in the queue; L13's proposals ride in a wrapper whose origin is the Project.

## Template

`claim:Envoy.Template`: An envoy is a fresh ticket captain the coordinator dispatches onto the queued ticket itself, the his-call Question or a bundle's wrapper, opening no other and loading every job skill it needs (DYD-1082 1b6b6a5d Q17, DYD-1195 317eb49f).

Spec: H15, H16, H18, H23, K14

**coordinator** (Active, nominal):

> Dispatch each envoy onto the ticket at the head of the queue, opening no other: a fresh ticket captain from the ticket-captain brief, naming every job skill from the envoy brief (`resources/envoy-brief.md`)'s table that its ticket or a wrapper's members need.

## SlotTicket

`claim:Envoy.SlotTicket`: The envoy brief's slot 1.

Spec: H19

**coordinator** (EnvoySlots):

> The ticket, and why it needs him.

## SlotSettled

`claim:Envoy.SlotSettled`: The envoy brief's slot 2.

Spec: H19

**coordinator** (EnvoySlots):

> What's settled.

## SlotSources

`claim:Envoy.SlotSources`: The envoy brief's slot 3.

Spec: H19

**coordinator** (EnvoySlots):

> Sources to read first.

## SlotStop

`claim:Envoy.SlotStop`: The envoy brief's slot 4.

Spec: H19

**coordinator** (EnvoySlots):

> The stop line.

## SlotRecord

`claim:Envoy.SlotRecord`: The envoy brief's slot 5.

Spec: H19

**coordinator** (EnvoySlots):

> Where to record, in his words.

## SlotReturn

`claim:Envoy.SlotReturn`: The envoy brief's slot 6.

Spec: H19

**coordinator** (EnvoySlots):

> Return and closing summary.

## SlotOpening

`claim:Envoy.SlotOpening`: The envoy brief's slot 7.

Spec: H19

**coordinator** (EnvoySlots):

> The opening rule: a fresh 2–3-line picture, then the first question with its recommendation.

## Updates

`claim:Envoy.Updates`: Changes during a session reach the envoy as dispatcher updates; it never asks him to repeat them.

Spec: H19

**coordinator** (EnvoySession):

> Changes during the session reach you as dispatcher updates; never ask him to repeat them.

**ticket-captain** (Hitl):

> Changes during a session with him reach you as dispatcher updates; never ask him to repeat them.

## NameDrs

`claim:Envoy.NameDrs`: A question put to him names what each DR says, never only its number.

Spec: H24

**coordinator** (EnvoySession):

> Name what each DR says whenever you cite one to him, never only its number.

**ticket-captain** (Need, nominal):

> Name what each DR says whenever you cite one to him, never only its number.

**chief-of-staff** (Day, nominal):

> Name what each DR says whenever you cite one to him, never only its number.

## CloseSignal

`claim:Envoy.CloseSignal`: After a shared understanding, and once it has reported back, the envoy tells him the thread can be closed.

Spec: H19, H23

**coordinator** (EnvoySession):

> After a shared understanding, and once you have reported back, tell him the thread can be closed.

**ticket-captain** (Hitl):

> After a shared understanding, and once you have reported back, tell him the thread can be closed.

## SkillTable

`claim:Envoy.SkillTable`: Each job ticket loads a fixed skill; a Question's stage label picks its row.

Spec: H23

**coordinator** (EnvoySkills):

> | Job | Skill |
> |---|---|
> | Grilling | `/grilling` |
> | Walkthrough | `/walkthrough` |
> | Wayfinding | `/wayfinder` |
> | Question at stage `research` | `/ticket-captain`; it dispatches `research` crew |
> | Question at stage `council` | `/ticket-captain`, with its `council-chair` resource |
> | Question at stage `his-call` | `/co-thinker` |
> | Enablement | `/wizard`; when it is not a setup procedure he runs (credentials, cutover), just ask |

**ticket-captain** (TicketCaptainBrief):

> | Job | Skill |
> |---|---|
> | Grilling | `/grilling` |
> | Walkthrough | `/walkthrough` |
> | Wayfinding | `/wayfinder` |
> | Question at stage `research` | `/ticket-captain`; it dispatches `research` crew |
> | Question at stage `council` | `/ticket-captain`, with its `council-chair` resource |
> | Question at stage `his-call` | `/co-thinker` |
> | Enablement | `/wizard`; when it is not a setup procedure he runs (credentials, cutover), just ask |

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: JobTickets

## Hold

`claim:JobTickets.Hold`: A ticket captain holds each Question, Grilling, Walkthrough, Wayfinding and Enablement ticket with its job's skill; whether it is HITL depends on the ticket (DYD-1082 1b6b6a5d Q17).

Spec: H15, H23

**ticket-captain** (Job):

> A Question, Grilling, Walkthrough, Wayfinding or Enablement ticket is held the same way, with its job's skill loaded, and whether you work it with him depends on the ticket.

## Route

`claim:JobTickets.Route`: A job ticket's captain works it by the job-tickets resource.

Spec: H15, H23, K9

**ticket-captain** (Take):

> Holding a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket, work it by Job tickets (`resources/job-tickets.md`).

## OpenQuestion

`claim:JobTickets.OpenQuestion`: Every level opens a Question ticket at stage research for a need passed up to it, held by a fresh captain, and waits on it (DYD-1195 3a854e37).

Spec: H16, H23

**ticket-captain** (NeedPassedUp):

> A need passed up to you from below: open a Question sub-ticket at stage `research` for a fresh captain, and wait on it; never research it yourself.

## Stages

`claim:JobTickets.Stages`: A Question's stage label, set by whoever opens it, sets its Mode and picks how it is held, at any depth.

Spec: H23, S3

**ticket-captain** (Question):

> Every Question carries a stage label, set by whoever opens it: `research` and `council` are Mode AFK, `his-call` Mode HITL, and the stage, not the parent's Type, picks how it is held.

## Research

`claim:JobTickets.Research`: A research Question's captain researches first with research crew, and opens a council or his-call child only when the research does not resolve the call.

Spec: H23, H16, L18, K30

Transition in A need for Balazs (`claims/statechart/envoy/Envoy.tla`): Research → Answered, "its research resolves the call".

**ticket-captain** (Research, exit):

> At stage `research`, research the call first with `research` crew; open a `council` or `his-call` child only when the research does not resolve it.

## StaysResearch

`claim:JobTickets.StaysResearch`: A research Question opened under another Question, at any depth, stays at stage research.

Spec: H23

**ticket-captain** (Question):

> A research Question you open under another Question stays at stage `research`.

## EnvoySlots

`claim:JobTickets.EnvoySlots`: The envoy's own ticket captain writes the envoy brief's seven slots from its ticket, the his-call Question or a bundle's wrapper; the coordinator writes none.

Spec: H19, H18, H23

**ticket-captain** (Hitl):

> Holding a `his-call` Question or a bundle's wrapper, write the envoy brief's seven slots from it before you open with him: the ticket and why it needs him · what's settled · sources to read first · the stop line · where to record, in his words · return and closing summary · the opening rule, a fresh 2–3-line picture, then the first question with its recommendation.

## Deciding

`claim:JobTickets.Deciding`: A Wayfinding captain is the deciding half of the map holder; the coordinator keeps it on track (DYD-1195 b46e8343).

Spec: H4, H5

**ticket-captain** (Wayfinding):

> Holding a Wayfinding ticket, you are the deciding half of the Project's map holder: the coordinator keeps the map on track, mechanically, and every call on it is yours.

## SpecGuards

`claim:JobTickets.SpecGuards`: The spec, not a DR, is the guard: every feature has one.

Spec: S16

**ticket-captain** (Wayfinding):

> Give every feature a spec, whether or not a DR backs it.

## Milestones

`claim:JobTickets.Milestones`: The map holder's Wayfinding captain places few Milestones while charting, the walking skeleton first.

Spec: N12 DR 060 §1

**ticket-captain** (Wayfinding):

> While charting, place the Project's Milestones in Linear: few, each a chunk where drift from his intent would show, the walking skeleton first and the landing last; every delivery ticket belongs to one.

## Skeleton

`claim:JobTickets.Skeleton`: Only the walking skeleton's Walkthrough blocks the next Milestone.

Spec: N12 DR 060 §3

**ticket-captain** (Wayfinding):

> File the walking skeleton's Walkthrough with the map, as a native blocker of the next Milestone's tickets until his verdict or skip; later Milestones' Walkthroughs block nothing.

## SeatRecord

`claim:JobTickets.SeatRecord`: A seat a Wayfinding captain records on a ticket replaces the coordinator's fixed rule for it, applied unless it records why not.

Spec: R14, H2

**ticket-captain** (Wayfinding):

> A seat you record on a ticket you cut replaces the coordinator's fixed rule for it: apply the same rule unless you record why not.

## FillerRelease

`claim:JobTickets.FillerRelease`: A filler blocked on a Wayfinding ticket can be pulled again only once a Wayfinding ticket records its release.

Spec: H11

**ticket-captain** (Wayfinding):

> A filler ticket blocked on yours stays unpullable until you record its release on your ticket.

## Result

`claim:JobTickets.Result`: A job ticket's result is recorded on the ticket.

Spec: H15, H19

**ticket-captain** (Hitl):

> Record the result on your ticket, in his words.

## Wrapper

`claim:JobTickets.Wrapper`: A bundle wrapper's captain loads every job skill its members need.

Spec: H18, H23

**ticket-captain** (Hitl):

> Holding a bundle's wrapper, load every job skill its members need: `wizard` for an Enablement, `co-thinker` for a `his-call` Question.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Captain

## Owns

`claim:Captain.Owns`: A captain holds one ticket to its end and answers for its outcome.

Spec: S7, H1

Evidence (code): `AGENTS.md`

Layer: AGENTS.md gives every agent the frame; only this skill speaks to the agent just dispatched onto a ticket.

Reason: An agent that takes its ticket for a task to attempt stops at the first obstacle instead of at the result.

**ticket-captain** (Job; read by TicketCaptain):

> You hold one ticket to its end, and you are responsible for its outcome.

## Kind

`claim:Captain.Kind`: A ticket's kind names the work, and the captain skill is how any kind is held.

Spec: K30

Evidence (code): `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs`

Layer: The host loads one skill by name and knows nothing of kinds.

Reason: A reader told at once that the base is shared looks for its own recipe in a resource, not here.

**ticket-captain** (Job; read by TicketCaptain):

> Its kind says what the work is; this skill says how to hold it.

## Officer

`claim:Captain.Officer`: A captain's officer is its dispatcher, and it is the officer of the captains it dispatches.

Spec: H12

Evidence (run): This skill's writer was a `delegate_task` child whose brief said: "Your captain is the thread that dispatched you; return to it" (DYD-1372 hop 2).

Layer: T3 carries a child's result back to the thread that started it, and names nobody to turn to with a problem.

Reason: Every problem then has one address, up or down, and none is left without an owner.

**ticket-captain** (Job; read by TicketCaptain):

> Whoever dispatched you is your officer, and you are the officer of every captain you dispatch.

## Calls

`claim:Captain.Calls`: A captain decides only obvious, reversible calls; any other call is a Question sub-ticket a fresh captain answers.

Spec: H16

Evidence (run): Linear, team Dydo, read 2026-11-20: the Type label Question exists, and a ticket takes sub-tickets (DYD-1281 holds twenty).

Layer: Nothing in the host or the guard stops an agent from settling a call that was never its own.

Reason: A context full of its own work judges with a bias; a fresh one investigates first and then answers.

**ticket-captain** (Job; read by TicketCaptain):

> Decide only obvious, reversible calls; any other call becomes a Question sub-ticket for a fresh captain, and you wait on its answer.

## Read

`claim:Captain.Read`: A captain reads its contract, how it fits the ticket above, and the spec sections it serves; the spec governs.

Spec: S15, S16

Evidence (run): DYD-1372's contract, read 2026-11-20, holds all three: its outcome, a paragraph "How this fits above", and the spec rules it serves.

Layer: Linear only stores the description; weighing it against the spec is the holder's.

Reason: A holder that works from its contract alone drifts from the intent a little at every hop.

**ticket-captain** (Take; read by TicketCaptain):

> Read the ticket's contract, how it fits the ticket above it, and the spec sections it serves; where they disagree, the spec wins.

## FillerBack

`claim:Captain.FillerBack`: A filler ticket that fails its captain's check goes back unstarted and unassigned, with a one-line reason.

Spec: H11

Transition in A captain and its ticket (the model's own machine, not written; the hand-written chart is in `statechart/captain/`): Dispatched → Returned, "a filler ticket fails its check: it goes back unstarted and unassigned".

Evidence (code): `Services/Survey/SurveyRules.cs`

Layer: `dydo survey <project>` offers a ticket as pickable only while it is unstarted and unassigned, and nothing puts a rejected one back in that state.

Reason: A rejected ticket left In Progress looks held, and no officer is ever offered it again.

**ticket-captain** (StartChecks; read by TicketCaptain, on claim:Captain.Filler):

> Your brief calls the ticket filler: if it is not small, reversible and free of design calls, return it to your officer unstarted and unassigned, with a one-line reason.

## Filler

`claim:Captain.Filler`: A captain on a filler ticket checks, before it starts it, that the ticket is small, reversible and free of design calls.

Spec: H11

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the groups are Mode, Severity and Type, with nothing for small, reversible or free of design calls, so nobody upstream can have checked them.

Layer: Whoever pulls filler sorts by labels alone; these three need someone who has opened the ticket.

Reason: Work that hides a design call is caught before anyone builds it.

**ticket-captain** (StartChecks; read by TicketCaptain):

> Your brief calls the ticket filler: if it is not small, reversible and free of design calls, return it to your officer unstarted and unassigned, with a one-line reason.

## Load

`claim:Captain.Load`: A captain loads its kind's resource and every skill its brief names.

Spec: K14

Evidence (run): This skill's writer was briefed to load `code-writer`, then `writing-for-agents`, and loaded both by name with the Skill tool.

Layer: The host lists the skills it has and loads none for a kind of ticket.

Reason: Naming them in the brief keeps the process from resting on an agent's guess at which ones apply.

**ticket-captain** (Take; read by TicketCaptain, on claim:Captain.Start):

> Set the ticket In Progress, then load your kind's resource and every skill your brief names.

## Start

`claim:Captain.Start`: Whoever starts a ticket sets it In Progress.

Spec: S6

Transition in A captain and its ticket (the model's own machine, not written; the hand-written chart is in `statechart/captain/`): Dispatched → Taken, "the ticket is set In Progress and its kind's resource loaded: its result is a change, a verdict or an answer, and its brief has said who merges it".

Evidence (run): Linear, team Dydo, statuses read 2026-11-20: In Progress is a started status and Todo the unstarted one, beside Done, Canceled and Duplicate; DYD-1372 was In Progress while this was written.

Layer: T3's `link_issue` only shows the ticket beside its thread; it changes no status in Linear.

Reason: The status is how every other agent sees that someone already has the ticket.

**ticket-captain** (Take; read by TicketCaptain):

> Set the ticket In Progress, then load your kind's resource and every skill your brief names.

## Thread

`claim:Captain.Thread`: Whoever sets a ticket In Progress attaches the holder's T3 thread link.

Spec: S13

Evidence: open, so no skill states this claim yet. Spec S13 leaves it to verify whether Linear takes a T3 thread link as an attachment, and nobody has: DYD-1372, DYD-1281 and DYD-1339, read 2026-11-20, carry none. T3's `link_issue` already shows a ticket beside its thread.

Layer: T3 links ticket and thread on its own side only; from Linear, nobody can tell which thread has a ticket.

Reason: A dead thread can be found and resumed from the ticket it was working.

**ticket-captain** (withheld; Take; read by TicketCaptain):

> Attach your thread's link to the ticket, titled `T3 · ticket-captain · <kind> · <model>`.

## Kinds

`claim:Captain.Kinds`: Implement, Review and Research are the captain's kind resources.

Spec: K30, K31

Evidence (code): `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs`

Layer: The build checks that each linked file exists; choosing among them is the reader's.

Reason: Each kind carries only its own recipe, so no captain reads past two others to find it.

**ticket-captain** (Kinds; read by TicketCaptain):

> | Your ticket is | Load |
> | -- | -- |
> | a change: an Implement, a Fix, a merge conflict | Implement (`resources/implement.md`) |
> | a Review | Review (`resources/review.md`) |
> | a question you are briefed to research | Research (`resources/research.md`) |

## Stage

`claim:Captain.Stage`: A Question is held with the Research kind only when its brief sends its holder to research it; one whose brief names a job's skill is a job ticket, held with that skill.

Spec: H23, S3

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Type group holds Question and Research, and no label names a Question's stage, which spec H23 calls `research`, `council` and `his-call` and S3 leaves to the user's Enablement. So a Question's holder has its brief alone to tell which it holds. `skills/` holds co-thinker, the skill H23 gives the Question the user settles.

Layer: Linear shows that a ticket is a Question and nothing of how it is to be settled.

Reason: A call the user alone can make, sent through the sources once more, costs a pass that cannot answer it and puts off his sitting.

**ticket-captain** (Hold; read by TicketCaptain, on claim:Captain.Jobs):

> A Grilling, Walkthrough, Wayfinding or Enablement ticket, or any other Question, is held the same way, with its job's skill from your brief; its Mode label says whether the user takes part.

## Jobs

`claim:Captain.Jobs`: A Grilling, Walkthrough, Wayfinding or Enablement ticket is held as any other, with its job's skill; its Mode says whether the user takes part.

Spec: H15, H23

Evidence (run): `skills/` holds grilling, walkthrough, wayfinder and wizard; Linear's Mode group, read 2026-11-20, holds AFK and HITL.

Layer: The brief names the job's skill, and nothing but this line says the ticket is still held like every other.

Reason: One way of holding every kind keeps a job ticket from growing a process of its own.

**ticket-captain** (Hold; read by TicketCaptain):

> A Grilling, Walkthrough, Wayfinding or Enablement ticket, or any other Question, is held the same way, with its job's skill from your brief; its Mode label says whether the user takes part.

## ReportUp

`claim:Captain.ReportUp`: A captain reports a problem outside its ticket to its officer.

Spec: R5

Evidence (tool): delegate_task: a child's final result returns to the thread that started it, which reads it with `task_status`.

Layer: No tool routes a finding to its owner; the one address a captain surely has is its officer.

Reason: What matters is that it surfaces, even when the finder cannot tell whose it is.

**ticket-captain** (WhenThingsGoWrong; read by TicketCaptain):

> Report a problem outside your ticket to your officer.

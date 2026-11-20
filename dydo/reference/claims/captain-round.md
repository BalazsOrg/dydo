---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainRound

## Verdict

`claim:CaptainRound.Verdict`: The verdict lives on the Review ticket: PASS or FAIL, lenses, seats and shoulds; FAIL means at least one blocker.

Spec: R17, S8, R11

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: each round is its own Review ticket, r1 to r4.

Layer: The seats' returns live in the dispatcher's thread until someone writes them on the ticket.

Reason: The next round, and whoever resumes the ticket, read the verdict there.

**ticket-captain** (RoundSteps; read by TicketCaptain):

> Post one verdict on the Review ticket: PASS or FAIL, the lenses, each seat's model and provider with its blockers, and the shoulds. FAIL means at least one blocker.

## PrLine

`claim:CaptainRound.PrLine`: A PASS is written, with its verdict, as one line in the PR body's Independent review section.

Spec: R17

Evidence (code): `Services/Merge/IndependentReview.cs`

Layer: `dydo merge` reads that line and refuses without it; it cannot write it.

Reason: A line in any other shape is a refusal at the merge, long after the reviewer has gone.

**ticket-captain** (RoundSteps; read by TicketCaptain):

> On a PASS, in the same step, add its line to the PR body's `## Independent review`: `PASS - rubric/type: <code, docs or merge>; candidate <head SHA>; base <base SHA>`, then the Review ticket's key.

## Nits

`claim:CaptainRound.Nits`: A nit is one line on its record.

Spec: S11

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit, should described as for "should tickets filed from reviews". The nit label is described as only for an inquisition's nit ticket: "a review nit stays one line on its ticket's record".

Layer: Linear would take a ticket per nit; the workspace keeps them off the board.

Reason: A nit is kept where it can be read, without a ticket to open, hold and close.

**ticket-captain** (RoundSteps; read by TicketCaptain):

> Record each nit as one line on the Review ticket.

## Shoulds

`claim:CaptainRound.Shoulds`: A should is filed as a child of the ticket where it was found, after a check for a duplicate under that ticket and its ancestors.

Spec: S17

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit, should described as for "should tickets filed from reviews".

Layer: Linear takes a second ticket for the same fault without a word.

Reason: It stays beside the work it is about and off the Project's root.

**ticket-captain** (RoundSteps; read by TicketCaptain):

> File each should as a child of the ticket where it was found, after checking the open shoulds there and above for a duplicate.

## Group

`claim:CaptainRound.Group`: Grouping shoulds is a judgement: minor ones may share a ticket, ones on the same code may stay together, one that needs a lot of work gets its own.

Spec: S17, S4

Evidence (run): Linear, DYD-1333 read 2026-11-20: Fix r2 closed review r2's "2 blockers and 5 shoulds" as one ticket.

Layer: A ticket is both a unit of work and a record, and no rule fits both readings every time.

Reason: Five small fixes one agent makes in a pass need no five tickets, and a large one needs room of its own.

**ticket-captain** (RoundRules; read by TicketCaptain):

> Group shoulds by judgement: minor ones may share a ticket, ones on the same code may stay together, and one that needs a lot of work gets its own.

## ParentDone

`claim:CaptainRound.ParentDone`: A parent may close Done while its shoulds stay open.

Spec: S17

Evidence (run): Linear, DYD-1281 read 2026-11-20: the parent is Ready to Merge while its child DYD-1295 is Todo.

Layer: Linear ties a parent's status to none of its children, so nothing settles this either way.

Reason: A should can wait, so it never holds back the work it was found in.

**ticket-captain** (RoundRules; read by TicketCaptain, on claim:CaptainRound.Cheap):

> A cheap should goes into the current Fix; the rest wait, and their parent may close Done meanwhile.

## Cheap

`claim:CaptainRound.Cheap`: A cheap should is fixed in place before its parent merges; the rest wait.

Spec: S17

Evidence (run): Linear, DYD-1327 read 2026-11-20: Fix r1 closed "7 blockers, 12 shoulds and 9 nits" in one ticket.

Layer: Nothing weighs a should's cost against the Fix already open.

Reason: A fix made while the code is open costs less than a ticket that remembers it.

**ticket-captain** (RoundRules; read by TicketCaptain):

> A cheap should goes into the current Fix; the rest wait, and their parent may close Done meanwhile.

## Changed

`claim:CaptainRound.Changed`: A severity a fixer changed is recorded, with its reason.

Spec: R11

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: The fixer returns its dispute to its dispatcher only; the Review ticket still shows the old label.

Reason: The next reviewer sees why a blocker was not fixed as one.

**ticket-captain** (RoundRules; read by TicketCaptain):

> If a fixer changed a finding's severity, record the change and its reason.

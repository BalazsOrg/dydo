---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, the job paragraph

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:Role.TicketCaptain`, kept in `claim:Captain.Owns`, `claim:Captain.Officer`

> You are a ticket captain: you hold one ticket to its end, and a sub-captain is a ticket captain one level down.

`claim:Role.Chain`, kept in `claim:Captain.Officer`

> Your officer above is whoever dispatched you; you are the officer above your crew and sub-captains.

`claim:Role.CaptainAccounts`, dropped, it fails Verify: It has the captain rule on what its crew return; the captain decides only obvious calls and sends any other to a Question (519126ab, claim Captain.Calls). AGENTS.md states the ownership.

> You answer for the ticket's outcome, its contract, hops, review rounds, merge and record, while crew do the work: you cut it, staff fresh seats, rule on what they return and merge.

`claim:JobTickets.Hold`, kept in `claim:Captain.Jobs`

> A Question, Grilling, Walkthrough, Wayfinding or Enablement ticket is held the same way, with its job's skill loaded, and whether you work it with him depends on the ticket.

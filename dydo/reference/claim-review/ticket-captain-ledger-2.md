---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Ledger: ticket-captain, 1. Take the ticket

What became of each line of the legacy `ticket-captain` spine in this section, and of each resource it links.

`claim:Role.CaptainTools`, dropped, it fails Need: No act uses a list of tools: each tool is named in the line that uses it (2119cd65). Three of its commands are not in the CLI.

> Your tools: Linear, `delegate_task`, `dydo usage`, `dydo test gate`, `dydo merge`, `dydo survey board`, `dydo archive`, the PR and its CI.

`claim:JobTickets.Route`, deferred: Its resource, job tickets, is not rebuilt in DYD-1372; until then a job ticket's brief names its skill (claim Captain.Jobs).

> Holding a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket, work it by Job tickets (`resources/job-tickets.md`).

`claim:Inquisition.Conduct`, deferred: Its resource, the inquisition captain, is not rebuilt in DYD-1372.

> Holding an Inquisition ticket or one of its sub-tickets, conduct it by the inquisition captain (`resources/inquisition-captain.md`).

`claim:Coordination.FillerBack`, kept in `claim:Captain.Filler`

> Dispatched onto a filler ticket, check it is small, two-way and needs no design call; if not, send it back unassigned with a one-line reason.

`claim:Project.Weeds`, kept in `claim:CaptainImplement.Weed`

> Holding a ticket for a mistake found after landing, pick its fix and the lint, test or rule that keeps it from recurring.

`claim:Council.Hold`, deferred: It leads to a council ticket, and the council-chair resource that ticket's captain needs is not rebuilt in DYD-1372.

> Holding a Question at stage `council`, chair it by the council chair (`resources/council-chair.md`).

`claim:Tree.Branch`, dropped, it fails Handled: T3 makes the checkout before the agent starts (`delegate_task`'s `isolation`), so a line on naming or creating a branch has every agent solve it again (1ffbc4c8).

> Name the ticket branch `<KEY>-<slug>` off the feature branch, or off main for an atomic ticket, so Linear attaches the branch and PR; post branch, base SHA and worktree path on the ticket before the first edit.

`claim:Tree.CrewLimits`, dropped, it fails Handled: T3 makes the checkout before the agent starts (`delegate_task`'s `isolation`), so a line on naming or creating a branch has every agent solve it again (1ffbc4c8). Who merges is in the brief (claim CaptainBriefs.Merger).

> Create the ticket's branch, open its PR and merge it yourself; ordinary crew never do.

`claim:Report.Up`, kept in `claim:Captain.ReportUp`

> Report a problem outside your ticket to your officer.

`claim:Dispatch.Start`, kept in `claim:Captain.Start`, `claim:Captain.Thread` (open, not rendered)

> Set your ticket In Progress and attach your T3 thread link titled `T3 · ticket-captain · <model>`.

Resource `resources/job-tickets.md` (Job tickets), deferred: Not rebuilt in DYD-1372: nothing else is rebuilt until the user approves this base (519126ab).

Resource `resources/inquisition-captain.md` (Inquisition captain), deferred: Not rebuilt in DYD-1372: nothing else is rebuilt until the user approves this base (519126ab).

Resource `resources/council-chair.md` (Council chair), deferred: Not rebuilt in DYD-1372: nothing else is rebuilt until the user approves this base (519126ab).

Resource `resources/council-members.md` (Council members' briefs), deferred: Not rebuilt in DYD-1372: nothing else is rebuilt until the user approves this base (519126ab).

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Project

## Take

`claim:Project.Take`: He opens a Project and charts it HITL, with its grillings and first wayfindings (DYD-1082 555a8300 Q21).

Spec: H5, S14

Transition in A Project and its coordinator (`claims/statechart/project/Project.tla`): Unheld → Charting, "he opens the Project and charts it, HITL". A system transition: no skill carries it.

## Charted

`claim:Project.Charted`: Charting ends once the spec exists and the first tickets are contracted; the Project then waits for its coordinator (DYD-1082 555a8300 Q21).

Spec: S14, S15, H4

Transition in A Project and its coordinator (`claims/statechart/project/Project.tla`): Charting → Unheld, "the Project is up: the spec exists and the first tickets are contracted". A system transition: no skill carries it.

## Replan

`claim:Project.Replan`: A fundamental blocker or a new requirement goes to the officer above to re-plan.

Spec: R9, H5

Transition in A Project and its coordinator (`claims/statechart/project/Project.tla`): Watching → Replanning, "a blocker is fundamental, or a new requirement".

**coordinator** (Replanning, arrival):

> A fundamental blocker or a new requirement from below: open a Wayfinding ticket to re-plan the affected part of the map, and put a fresh captain on it.

## Replanned

`claim:Project.Replanned`: On a re-plan, dead hops go Canceled with a one-line reason.

Spec: S10

Transition in A Project and its coordinator (`claims/statechart/project/Project.tla`): Replanning → Watching, "dead hops Canceled with a reason".

**coordinator** (Replanning, exit):

> Once the re-plan is filed and its dead hops are Canceled with a one-line reason, sequence what it filed and go back to watching.

**linear-workspace-standard** (LinearStatuses):

> On a re-plan, dead hops go Canceled with a one-line reason.

## StartLanding

`claim:Project.StartLanding`: The landing starts once every delivery ticket is merged into the feature branch.

Spec: N5

Transition in A Project and its coordinator (`claims/statechart/project/Project.tla`): Watching → Landing, "every delivery ticket is merged into the feature branch".

**coordinator** (Watching, exit):

> When every delivery ticket is merged into the feature branch, start the landing.

## SpecDoc

`claim:Project.SpecDoc`: The spec is a main Linear document on the Project, written and refined by the map holder.

Spec: S14

**ticket-captain** (Wayfinding):

> Write and refine `Spec: <Project>`, the Project's main Linear document with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks; a sub-feature that needs defining well gets its own spec, linked from it.

**linear-workspace-standard** (LinearSpec):

> The spec is a main Linear document on the Project, `Spec: <Project>`, with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks. A sub-feature that needs defining well gets its own spec, linked from it. The map holder writes and refines it; at acceptance it is deleted after its lasting parts move to DRs, the glossary, docs and the verification map.

**coordinator** (Routing, on claim:Project.Rechart):

> Send every map call to a fresh captain on a Wayfinding ticket: refining the spec, reconciling the open contracts after it changes, re-charting as fog clears, contracting an `uncontracted` ticket, and the release harvest; open the ticket, wake its captain, and sequence what it files.

## FeatureBranch

`claim:Project.FeatureBranch`: The feature branch is cut from main before the first ticket branch.

Spec: N12 DR 045 §8

**coordinator** (Lanes):

> Open `feature/<project-slug>` from main before the first ticket branch.

**working-tree-standard** (TreeBranches):

> A Project's feature branch, `feature/<project-slug>`, is cut from main before its first ticket branch.

## Harvest

`claim:Project.Harvest`: A release's map holder harvests adopters' friction queues and local patches.

Spec: H9

**ticket-captain** (Wayfinding):

> When a release is planned, harvest the adopters' Agent friction queues and local patches.

**coordinator** (Routing, on claim:Project.Rechart):

> Send every map call to a fresh captain on a Wayfinding ticket: refining the spec, reconciling the open contracts after it changes, re-charting as fog clears, contracting an `uncontracted` ticket, and the release harvest; open the ticket, wake its captain, and sequence what it files.

## Tick

`claim:Project.Tick`: The coordinator's watch is a deterministic loop over the survey, the board and liveness.

Spec: H5, S22, S23

**coordinator** (Watching, nominal):

> Each tick, read `dydo survey board <project>`, the board and the liveness findings, and act on every finding.

## EmptyTick

`claim:Project.EmptyTick`: An empty tick ends in one line.

Spec: R31

**coordinator** (Watching, nominal):

> An empty tick ends in one line.

## CheckIn

`claim:Project.CheckIn`: The coordinator keeps a 20-minute scheduled check-in while work is in flight or pickable.

Spec: N12 DR 047, H5

**coordinator** (Watching, nominal):

> Keep a T3 `schedule_task` check-in every 20 minutes on your own thread while work is in flight or pickable; delete it when only his word is awaited.

## LanesFull

`claim:Project.LanesFull`: Every officer keeps its lanes full toward a soft four, pulling filler on purpose.

Spec: H10

**coordinator** (Lanes):

> Fill each lane with a captain, toward a soft target of four, oldest first at each step: ready tickets the plan names (unblocked and contracted), then the open Shoulds batch, or a new one when shoulds wait outside a batch, then the survey's `pickable` side tickets, then continuous tickets, those blocking a ticket in your Project first.

**ticket-captain** (Staffing, nominal):

> Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.

## PullRule

`claim:Project.PullRule`: The standing rule for pulling a ticket without asking.

Spec: H11

**coordinator** (Lanes):

> Pull a filler only when its labels already meet the pull rule: Mode AFK; Type Bug or Feature at Severity should or lower, or an unrated CI finding; unblocked; oldest first; its captain checks small, two-way and no design call.

**ticket-captain** (Staffing, nominal):

> Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.

## Pulled

`claim:Project.Pulled`: A pulled continuous ticket follows its officer's chain.

Spec: H8

**coordinator** (Lanes):

> A continuous ticket you pull follows your chain from then on.

**ticket-captain** (Staffing, nominal):

> A continuous ticket you pull follows your chain from then on.

## CiFinding

`claim:Project.CiFinding`: A CI finding goes to its open ticket's officer; for a closed ticket in an open Project the coordinator opens its ticket, as a child of one closed within 14 days.

Spec: H11, H8

**coordinator** (Watching, nominal):

> Send a CI finding for an open ticket to that ticket's officer; for a ticket closed more than 14 days ago, open a new ticket, and for one closed more recently, open it as a child of the closed ticket.

## OneTicket

`claim:Project.OneTicket`: One agent holds one ticket; several steps for one agent are wrapped in one ticket.

Spec: S7

**ticket-captain** (Cut, nominal):

> One agent holds one ticket; wrap several steps for one agent in one ticket.

**linear-workspace-standard** (LinearTickets):

> One agent holds one ticket, and several steps given to one agent are wrapped in one ticket; sub-tickets may nest.

## Contract

`claim:Project.Contract`: A contract is one ticket's expected outcome, quoting and linking its spec sections.

Spec: S15

**ticket-captain** (Cut, nominal):

> Write each sub-ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.

**linear-workspace-standard** (LinearSpec):

> A contract is one ticket's expected outcome, in its description. It quotes and links the spec sections it derives from, and that frozen quote replaces pinning a commit.

## Reconcile

`claim:Project.Reconcile`: When the spec changes, the map holder reconciles the open contracts.

Spec: S15

**ticket-captain** (Wayfinding):

> When the spec changes, reconcile the open contracts with it.

**coordinator** (Routing, on claim:Project.Rechart):

> Send every map call to a fresh captain on a Wayfinding ticket: refining the spec, reconciling the open contracts after it changes, re-charting as fog clears, contracting an `uncontracted` ticket, and the release harvest; open the ticket, wake its captain, and sequence what it files.

## Uncontracted

`claim:Project.Uncontracted`: An uncontracted Backlog ticket is contracted to Todo by a captain on a Wayfinding ticket (DYD-1082 1b6b6a5d Q16).

Spec: N12 DR 063 §4, H5, S5

**ticket-captain** (Wayfinding):

> Contract each `uncontracted` Backlog ticket, so it moves to Todo.

**coordinator** (Routing, on claim:Project.Rechart):

> Send every map call to a fresh captain on a Wayfinding ticket: refining the spec, reconciling the open contracts after it changes, re-charting as fog clears, contracting an `uncontracted` ticket, and the release harvest; open the ticket, wake its captain, and sequence what it files.

## FeatureBranchYours

`claim:Project.FeatureBranchYours`: The feature branch is the coordinator's, to merge into as it wishes.

Spec: R25

**coordinator** (Merges):

> Merges into your feature branch need no approval but yours.

**working-tree-standard** (TreeBranches):

> The feature branch is the coordinator's: it merges into it as it wishes.

## OneWayPaths

`claim:Project.OneWayPaths`: The guard-path list in dydo.json is his policy; the coordinator opens a ticket carrying each decision of his, for an implementer to make.

Spec: R27, R16

**coordinator** (Merges):

> When he decides a guard-path addition or removal, open a ticket carrying his decision for an implementer to edit `dydo.json`; its PR is one-way.

## MilestoneWalk

`claim:Project.MilestoneWalk`: A Milestone's walkthrough is offered when its last ticket merges.

Spec: N12 DR 060 §2, H22

**coordinator** (Merges):

> When a Milestone's last ticket merges into the feature branch, its Walkthrough ticket joins your HITL queue for a fresh envoy; a skip is a verdict to go on.

## OnMaster

`claim:Project.OnMaster`: Walkthroughs run on master whenever possible; the landing's walks the candidate.

Spec: N4

**coordinator** (Merges):

> Run a walkthrough on master whenever possible.

## Weeds

`claim:Project.Weeds`: Every weed becomes a rule.

Spec: R30

**coordinator** (Merges):

> Give each mistake found after landing a lint, test or rule as well as its fix.

## Usage

`claim:Project.Usage`: The work never stops for usage, and no officer brings a usage limit to him.

Spec: H21

**coordinator** (Watching, nominal):

> Keep the work going through any usage limit, and never bring one to him.

**ticket-captain** (Dispatching):

> Keep the work going through any usage limit, and never bring one to him.

**chief-of-staff** (Day, nominal):

> Keep the work going through any usage limit, and never bring one to him.

## Rechart

`claim:Project.Rechart`: Re-charting, cutting and every other map call go to a fresh captain on a Wayfinding ticket; the coordinator sequences what it files (DYD-1082 1b6b6a5d Q16, DYD-1195 b46e8343).

Spec: H5, H2, H14, N12 DR 045 §5

**coordinator** (Routing):

> Send every map call to a fresh captain on a Wayfinding ticket: refining the spec, reconciling the open contracts after it changes, re-charting as fog clears, contracting an `uncontracted` ticket, and the release harvest; open the ticket, wake its captain, and sequence what it files.

**ticket-captain** (Wayfinding):

> Re-chart as fog clears and cut the tickets the map needs, deciding each cut the coordinator would otherwise face; file what you cut and return, and the coordinator only sequences it.

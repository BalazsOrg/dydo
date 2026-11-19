---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Project

## Take

`claim:Project.Take`: Every open Project has a live admiral holding its outcome.

Spec: H4

Transition in A Project and its admiral (`claims/statechart/project/Project.tla`): Unheld → Charting, "the Project opens; it has a live admiral". A system transition: no skill carries it.

## Charted

`claim:Project.Charted`: Charting ends when the spec exists and the first tickets are contracted.

Spec: S14, S15

Transition in A Project and its admiral (`claims/statechart/project/Project.tla`): Charting → Watching, "the spec exists and the first tickets are contracted".

**admiral** (Charting, exit):

> Leave charting once `Spec: <Project>` exists and the first tickets carry contracts that quote and link it.

## Replan

`claim:Project.Replan`: A fundamental blocker or a new requirement goes to the officer above to re-plan.

Spec: R9

Transition in A Project and its admiral (`claims/statechart/project/Project.tla`): Watching → Replanning, "a blocker is fundamental, or a new requirement".

**admiral** (Replanning, arrival):

> A fundamental blocker or a new requirement from below: re-plan the affected part of the map.

## Replanned

`claim:Project.Replanned`: On a re-plan, dead hops go Canceled with a one-line reason.

Spec: S10

Transition in A Project and its admiral (`claims/statechart/project/Project.tla`): Replanning → Watching, "dead hops Canceled with a reason".

**admiral** (Replanning, exit):

> Cancel each dead hop with a one-line reason, then go back to watching.

**linear-workspace-standard** (LinearStatuses):

> On a re-plan, dead hops go Canceled with a one-line reason.

## StartLanding

`claim:Project.StartLanding`: The landing starts once every delivery ticket is merged into the feature branch.

Spec: N5

Transition in A Project and its admiral (`claims/statechart/project/Project.tla`): Watching → Landing, "every delivery ticket is merged into the feature branch".

**admiral** (Watching, exit):

> When every delivery ticket is merged into the feature branch, start the landing.

## SpecDoc

`claim:Project.SpecDoc`: The spec is a main Linear document on the Project, written and refined by the map holder.

Spec: S14

**admiral** (Charting, nominal):

> Write and refine `Spec: <Project>`, a main Linear document on the Project with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks; a sub-feature that needs defining well gets its own spec, linked from it.

**linear-workspace-standard** (LinearSpec):

> The spec is a main Linear document on the Project, `Spec: <Project>`, with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks. A sub-feature that needs defining well gets its own spec, linked from it. The map holder writes and refines it; at acceptance it is deleted after its lasting parts move to DRs, the glossary, docs and the verification map.

## SpecGuards

`claim:Project.SpecGuards`: The spec, not a DR, is the guard: every feature has one.

Spec: S16

**admiral** (Charting, nominal):

> Give every feature a spec, whether or not a DR backs it.

## FeatureBranch

`claim:Project.FeatureBranch`: The feature branch is cut from main before the first ticket branch.

Spec: N12 DR 045 §8

**admiral** (Charting, nominal):

> Open `feature/<project-slug>` from main before the first ticket branch.

**working-tree-standard** (TreeBranches):

> A Project's feature branch, `feature/<project-slug>`, is cut from main before its first ticket branch.

## Milestones

`claim:Project.Milestones`: The map holder places few Milestones while charting, the walking skeleton first.

Spec: N12 DR 060 §1

**admiral** (Charting, nominal):

> Place the Project's Milestones in Linear while charting: few, each a chunk where drift from his intent would show, the walking skeleton first and the landing last; every delivery ticket belongs to one.

## Skeleton

`claim:Project.Skeleton`: Only the walking skeleton's Walkthrough blocks the next Milestone.

Spec: N12 DR 060 §3

**admiral** (Charting, nominal):

> File the walking skeleton's Walkthrough with the map, as a native blocker of the next Milestone's tickets until his verdict or skip; later Milestones' Walkthroughs block nothing.

## Harvest

`claim:Project.Harvest`: A release's map holder harvests adopters' friction queues and local patches.

Spec: H9

**admiral** (Charting, nominal):

> Charting a release, first harvest the adopters' Agent friction queues and local patches.

## Tick

`claim:Project.Tick`: The admiral's watch is a deterministic loop over the survey, the board and liveness.

Spec: H5, S22, S23

**admiral** (Watching, nominal):

> Each tick, read `dydo survey board <project>`, the board and the liveness findings, and act on every finding.

## EmptyTick

`claim:Project.EmptyTick`: An empty tick ends in one line.

Spec: R31

**admiral** (Watching, nominal):

> An empty tick ends in one line.

## CheckIn

`claim:Project.CheckIn`: The admiral keeps a 20-minute scheduled check-in while work is in flight or pickable.

Spec: N12 DR 047, H5

**admiral** (Watching, nominal):

> Keep a T3 `schedule_task` check-in every 20 minutes on your own thread while work is in flight or pickable; delete it when only his word is awaited.

## LanesFull

`claim:Project.LanesFull`: Every officer keeps its lanes full toward a soft four, pulling filler on purpose.

Spec: H10

**admiral** (Watching, nominal):

> Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.

**ticket-captain** (Staffing, nominal):

> Keep your level's lanes full, toward a soft target of four; when the mainline can't fill one, pull your own pickable side tickets first, then continuous tickets.

## PullRule

`claim:Project.PullRule`: The standing rule for pulling a ticket without asking.

Spec: H11

**admiral** (Watching, nominal):

> Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.

**ticket-captain** (Staffing, nominal):

> Pull without asking only a ticket that is Mode AFK; a Bug or small Feature at Severity should or lower, or an unrated CI finding; two-way; no design call; oldest first.

## Pulled

`claim:Project.Pulled`: A pulled continuous ticket follows its officer's chain.

Spec: H8

**admiral** (Watching, nominal):

> A continuous ticket you pull follows your chain from then on.

**ticket-captain** (Staffing, nominal):

> A continuous ticket you pull follows your chain from then on.

## CiFinding

`claim:Project.CiFinding`: A CI finding goes to its open ticket's officer, or to the admiral for a long-closed one.

Spec: H11

**admiral** (Watching, nominal):

> Send a CI finding for an open ticket to that ticket's officer; one for a long-closed ticket is yours.

## OneTicket

`claim:Project.OneTicket`: One agent holds one ticket; several steps for one agent are wrapped in one ticket.

Spec: S7

**admiral** (Watching, nominal):

> One agent holds one ticket; wrap several steps for one agent in one ticket.

**ticket-captain** (Cut, nominal):

> One agent holds one ticket; wrap several steps for one agent in one ticket.

**linear-workspace-standard** (LinearTickets):

> One agent holds one ticket, and several steps given to one agent are wrapped in one ticket; sub-tickets may nest.

## Contract

`claim:Project.Contract`: A contract is one ticket's expected outcome, quoting and linking its spec sections.

Spec: S15

**admiral** (Watching, nominal):

> Write each ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.

**ticket-captain** (Cut, nominal):

> Write each sub-ticket's contract in its description: its expected outcome, quoting and linking the spec sections it derives from.

**linear-workspace-standard** (LinearSpec):

> A contract is one ticket's expected outcome, in its description. It quotes and links the spec sections it derives from, and that frozen quote replaces pinning a commit.

## Reconcile

`claim:Project.Reconcile`: When the spec changes, the map holder reconciles the open contracts.

Spec: S15

**admiral** (Watching, nominal):

> When the spec changes, reconcile the open contracts.

## Uncontracted

`claim:Project.Uncontracted`: The admiral contracts an uncontracted Backlog ticket to Todo.

Spec: N12 DR 063 §4

**admiral** (Watching, nominal):

> Contract an `uncontracted` ticket (Backlog, unassigned, no open blocker) to Todo.

## FeatureBranchYours

`claim:Project.FeatureBranchYours`: The feature branch is the admiral's, to merge into as it wishes.

Spec: R25

**admiral** (Watching, nominal):

> Merges into your feature branch need no approval but yours.

**working-tree-standard** (TreeBranches):

> The feature branch is the admiral's: it merges into it as it wishes.

## OneWayPaths

`claim:Project.OneWayPaths`: The admiral keeps the one-way path list in dydo.json.

Spec: R27

**admiral** (Watching, nominal):

> Keep the one-way path list in `dydo.json`.

## MilestoneWalk

`claim:Project.MilestoneWalk`: A Milestone's walkthrough is offered when its last ticket merges.

Spec: N12 DR 060 §2, H22

**admiral** (Watching, nominal):

> When a Milestone's last ticket merges into the feature branch, offer its walkthrough through a fresh envoy; a skip is a verdict to go on.

## NightSkeleton

`claim:Project.NightSkeleton`: Overnight, the skeleton's Walkthrough blocks nothing.

Spec: N12 DR 060 §4

**admiral** (Watching, nominal):

> Overnight, the skeleton's Walkthrough blocks nothing: post `DECIDED` on it, carry on, and treat a wrong direction as a revert.

## OnMaster

`claim:Project.OnMaster`: Walkthroughs run on master whenever possible; the landing's walks the candidate.

Spec: N4

**admiral** (Watching, nominal):

> Run a walkthrough on master whenever possible.

## Weeds

`claim:Project.Weeds`: Every weed becomes a rule.

Spec: R30

**admiral** (Watching, nominal):

> Give each mistake found after landing a lint, test or rule as well as its fix.

## Usage

`claim:Project.Usage`: The work never stops for usage, and no officer brings a usage limit to him.

Spec: H21

**admiral** (Watching, nominal):

> Keep the work going through any usage limit, and never bring one to him.

**ticket-captain** (Dispatching):

> Keep the work going through any usage limit, and never bring one to him.

**chief-of-staff** (Day, nominal):

> Keep the work going through any usage limit, and never bring one to him.

## Rechart

`claim:Project.Rechart`: The admiral re-charts as fog clears.

Spec: H5, N12 DR 045 §5

**admiral** (Wayfinding):

> Re-chart as fog clears: create, split, drop or resequence tickets.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Dispatch

## Send

`claim:Dispatch.Send`: A staffed seat goes out briefed; any reviewer or judge comes from a family other than the author's when possible.

Spec: H3, K14, R15, S1

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): Staffing → Briefed, "one seat, cross-family where possible, sent from its kind's template".

**ticket-captain** (Staffing, exit):

> Seat any reviewer from a family other than the author's when possible, single seats included.

**implementer** (Briefed, arrival):

> You arrive with a brief from the implementer dispatch template; work only within it.

**reviewer** (Briefed, arrival):

> You arrive with a brief from the reviewer dispatch template; judge only what it names.

**scout** (Briefed, arrival):

> You arrive with one question and its return-to; you hold no ticket.

## Council

`claim:Dispatch.Council`: A council convenes only for stakes or value.

Spec: L16, L17

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): Staffing → CouncilCalled, "stakes or value: a council".

**ticket-captain** (Staffing, exit):

> Convene a council only for a heavy PR, one that meets the stakes or value test: guard or security work, a base other tickets build on, or the late simplify review.

## SelfAuthor

`claim:Dispatch.SelfAuthor`: A captain authors only a change cheaper to do than to brief and too small to need its own review.

Spec: R16

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): Staffing → SelfAuthored, "cheaper to do than to brief, too small to need its own review".

**ticket-captain** (Staffing, exit):

> Author a change yourself only when it is cheaper to do than to brief and too small to need its own review; persistence, migrations, permissions, guard rules and uncertain native interfaces always go to crew, and a sub-captain authors nothing.

## Start

`claim:Dispatch.Start`: Whoever starts a step sets its hop In Progress and attaches the holder's T3 thread link.

Spec: S6, S13

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): Briefed → Working, "the holder starts its hop".

**implementer** (Briefed, exit):

> Start your hop: set it In Progress and attach your T3 thread link titled `T3 · implementer · <model>`.

**reviewer** (Briefed, exit):

> Holding a Review hop alone, set it In Progress and attach your T3 thread link titled `T3 · reviewer · <model>`.

**ticket-captain** (Briefed, exit):

> Set your ticket In Progress and attach your T3 thread link titled `T3 · ticket-captain · <model>`.

**linear-workspace-standard** (LinearStatuses):

> Whoever starts a step sets it In Progress and attaches the holder's T3 thread link, titled `T3 · <role> · <model>`; neither the assignee nor Linear's `delegate` carries identity.

## TooBig

`claim:Dispatch.TooBig`: Crew return too big, re-cut; the cutter decides whether a unit gets children; no agent promotes itself.

Spec: H2

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): Briefed → ReCut, "too big for one agent".

**implementer** (Briefed, exit):

> A job too big for one agent goes back as `too big, re-cut`.

**reviewer** (Briefed, exit):

> A candidate too big for one seat goes back as `too big, re-cut`.

**ticket-captain** (ReCut, arrival):

> On `too big, re-cut` from your crew, you are the cutter: decide whether the unit gets children; a ticket too big for you goes back to your officer the same way, and you never promote yourself.

**coordinator** (ReCut, arrival):

> A captain's `too big, re-cut` comes to you as its cutter: open a Wayfinding ticket for the re-cut and put a fresh captain on it, since cutting is its call.

**working-tree-standard** (TreeDelegation):

> The cutter, the officer above, decides whether a unit gets children; no agent promotes itself, and crew return `too big, re-cut`.

## ReCut

`claim:Dispatch.ReCut`: The cutter cuts only coherent units, never to pass work down.

Spec: H2, H14

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): ReCut → Staffing, "the cutter cuts coherent children".

**ticket-captain** (ReCut, exit):

> Cut only coherent units too big for one agent or able to run in parallel, never to pass work down.

**coordinator** (ReCut, exit):

> Once its captain files the children, sequence them and put a captain on each ready one.

## Return

`claim:Dispatch.Return`: Crew return to their dispatcher, and whoever started a step closes it.

Spec: K14, R5, S6, N12 DR 047 §7

Transition in Crew dispatch and return (`claims/statechart/dispatch/Dispatch.tla`): Working → Returned, "the crew returns to its dispatcher".

**implementer** (Working, exit):

> Close your hop Done and return its commit SHA and receipts to the brief's return-to.

**reviewer** (Returned, arrival):

> Return your verdict, or your seat's findings, to the brief's return-to.

**scout** (Working, exit):

> Return the answer and its sources to the brief's return-to.

**ticket-captain** (Returned, arrival):

> Every crew return comes back to you: post its hop's SHA on the hop as the resume point.

**linear-workspace-standard** (LinearStatuses):

> Whoever started a step closes it; a parent's In Progress means its officer holds it.

## Template

`claim:Dispatch.Template`: An officer briefs crew from the crew kind's dispatch template and never reads a crew skill to brief it.

Spec: K14, K29

**coordinator** (Dispatching):

> Brief each captain from the ticket-captain dispatch template and name every skill its role requires with a slash: the template carries the default set, and you add the ticket's own skills.

**chief-of-staff** (Dispatching):

> Brief a coordinator from the coordinator dispatch template and name every skill its role requires with a slash; the template carries the default set.

**ticket-captain** (Dispatching):

> Brief from the crew kind's dispatch template; the crew loads its own skill, so never read a crew skill to brief it.

## Channel

`claim:Dispatch.Channel`: A Linear unit goes out by delegate_task; a native sub-agent shares its holder's checkout and the holder commits.

Spec: H3

**coordinator** (Dispatching):

> A Linear unit goes out by `delegate_task`; a native sub-agent never gets one, shares your checkout, and you commit its work.

**ticket-captain** (Dispatching):

> A Linear unit goes out by `delegate_task`; a native sub-agent never gets one, shares your checkout, and you commit its work.

**implementer** (Working, nominal):

> A native helper you start shares your checkout; you commit its work.

**working-tree-standard** (TreeDelegation):

> A Linear unit goes out by `delegate_task`. A native sub-agent never gets one: it shares its holder's checkout, and the holder commits.

## Worktrees

`claim:Dispatch.Worktrees`: Parallel lanes and sub-tickets with children get their own worktree; serial hops share their ticket's.

Spec: H3

**ticket-captain** (Dispatching):

> Give parallel lanes, and sub-tickets with children, their own worktree; serial hops share their ticket's.

**working-tree-standard** (TreeDelegation):

> Each parallel lane, and each sub-ticket with children, works in its own worktree; serial hops reuse their ticket's.

## Foreground

`claim:Dispatch.Foreground`: A native foreground sub-agent only for a result awaited within the turn; no native background sub-agents.

Spec: N12 DR 057 §1, N12 DR 057 §3, H3

**coordinator** (Dispatching):

> Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.

**ticket-captain** (Dispatching):

> Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.

**chief-of-staff** (Dispatching):

> Use a native foreground sub-agent only for a result you wait on within your turn, and stay in your turn until it arrives; start no native background sub-agent.

**working-tree-standard** (TreeDelegation):

> Under T3, a native foreground sub-agent only when its dispatcher waits for it within its own turn, and no native background sub-agents; a dispatcher waiting on a native child, or a child awaiting a result it owns, stays in its turn until the result arrives.

## ParentNotActive

`claim:Dispatch.ParentNotActive`: A native child cannot delegate_task; on parent_not_active the dispatch goes up.

Spec: N12 DR 057 §2

**coordinator** (Dispatching):

> On a captain's `parent_not_active`, dispatch its crew for it.

**ticket-captain** (Dispatching):

> A native child can't `delegate_task`: on `parent_not_active`, ask your officer to dispatch it.

**chief-of-staff** (Dispatching):

> On `parent_not_active`, ask him to dispatch it.

**working-tree-standard** (TreeDelegation):

> A native child lives only inside its dispatcher's turn and cannot call `delegate_task`: on `parent_not_active`, a captain asks its officer, the chief of staff asks the user, and the coordinator dispatches a captain's crew for it.

## CodexCap

`claim:Dispatch.CodexCap`: A brief that sends out Codex agents names a concurrency cap.

Spec: N12 DR 057 §4

**coordinator** (Dispatching):

> A brief that sends out Codex agents names a cap: about 8 at once, toward 16 only while runs stay clean, fewer on `429 turn_rate_limit_exceeded`, which means retry in a few minutes, never a blocker.

**ticket-captain** (Dispatching):

> A brief that sends out Codex agents names a cap: about 8 at once, toward 16 only while runs stay clean, fewer on `429 turn_rate_limit_exceeded`, which means retry in a few minutes, never a blocker.

**working-tree-standard** (TreeDelegation):

> A brief that sends out Codex agents names a cap: start around 8, go toward 16 only while runs stay clean, and back off on `429 turn_rate_limit_exceeded`; a capacity 429 means retry after a few minutes, never a blocker.

## AskUp

`claim:Dispatch.AskUp`: Crew hand their questions up and never ask the user directly.

Spec: N12 DR 057 §6

**implementer** (Working, nominal):

> Hand a question up to your captain, never to Balazs.

**reviewer** (Round, nominal):

> Hand a question up to your captain, never to Balazs.

**scout** (Working, nominal):

> Hand a question about your question up to your dispatcher, never to Balazs.

**working-tree-standard** (TreeDelegation):

> Crew take their questions to their captain, never to the user.

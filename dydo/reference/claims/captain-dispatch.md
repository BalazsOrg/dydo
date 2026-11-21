---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainDispatch

A line marked unwritten is the model's own, of a skill written by hand in `skills/roles/officers/ticket-captain/`: the build writes it nowhere, and the hand-written skill need not hold it.

## Brief

`claim:CaptainDispatch.Brief`: A child starts with only its brief and none of its dispatcher's conversation; the dispatcher writes the brief from the briefs resource.

Spec: K14

Evidence (tool): delegate_task: "Start one child agent with only `task`. No parent history."

Layer: T3 passes the brief on untouched and adds nothing from the dispatcher's own context.

Reason: Whatever the brief leaves out, the child has to rediscover or guess.

**ticket-captain** (unwritten; Briefs; read by TicketCaptain):

> A child starts with your brief and none of your conversation: write it by Briefs (`resources/briefs.md`).

## OneThing

`claim:CaptainDispatch.OneThing`: A dispatcher gives each child one thing to own, its ticket's outcome, and leaves how to reach it to the child.

Spec: K34, K30

Evidence (run): This skill's writer got such a brief (DYD-1372 phase 1, hop A): it named each change to reach and left the wording to the writer, "Tell me which you chose and why." AGENTS.md gives the child the other half: "You choose how to reach its outcome".

Layer: AGENTS.md tells every agent what it owns; what a dispatcher must therefore keep out of a brief is said nowhere below this skill.

Reason: A brief that carries two outcomes, or a method, leaves the child a task to follow where it was meant to answer for a result.

**ticket-captain** (unwritten; Briefs; read by TicketCaptain):

> Give each child one thing to own, its ticket's outcome, and leave how to reach it to the child.

## Family

`claim:CaptainDispatch.Family`: The captain of a change seats one agent a round, the Review ticket's holder, from a model family other than the author's when possible; that holder sizes its round and fills its seats.

Spec: R15, R34

Evidence (tool): delegate_task: `target.model` and `target.providerInstanceId` seat a child on another provider's model. His words (DYD-1372 aa5b445e): "let's say there is a captain for the review, which makes sense (it will decide what kind of a review matrix to use and how to manage it's sub agents)".

Layer: T3 seats whichever model it is asked for; which family wrote the work is known only to the dispatcher.

Reason: Two models of one family tend to share the blind spot that let the fault through, and a round sized by the captain whose change it judges is no longer impartial.

**ticket-captain** (unwritten; Seats; read by TicketCaptain):

> Seat each Review ticket's holder from a model family other than the author's when you can; it sizes and seats its own round.

## Scale

`claim:CaptainDispatch.Scale`: A round's seats scale with the breadth and depth of the work; there is no constant pair.

Spec: R12, R34

Evidence (run): DYD-1281's four rounds, read 2026-11-20, are one Review ticket each; nothing in Linear or T3 sets a number of seats.

Layer: No tool counts seats; how much review a piece of work deserves is the review captain's to size.

Reason: Review is expensive, and a pair on everything spends it where it finds nothing.

**ticket-captain** (unwritten; ReviewRules; read by TicketCaptain, on claim:CaptainReviewSeats.Prefer):

> Scale your round's seats to the breadth and depth of the work, with no fixed pair: seat the kinds you do not do yourself, one seat a kind, in parallel, each from a model family other than the author's when you can, by phase 3 of the spine.

## Roster

`claim:CaptainDispatch.Roster`: Each seat's model comes from the user's roster.

Spec: R13, K33

Evidence: open, a placeholder: the skill shows `{roster_placeholder}` where this claim's text will stand. The roster is the user's to write (R13), and it is not written: there is no text to state yet. He asked for the token where it will stand (DYD-1372 07c86aad): "at least put a {roster_placeholder} there", and "I will write it. (not now). Just use a placeholder for now."

Layer: `orchestrator_capabilities` lists the models a child can run on, and says nothing of which suits which seat.

Reason: The strongest models are kept for the seats that have something non-obvious to find.

**ticket-captain** (unwritten; placeholder; Seats; read by TicketCaptain):

> Once the user's roster stands below, pick each seat's model from it.
>
> {roster_placeholder}

## Usage

`claim:CaptainDispatch.Usage`: A dispatcher balances vendors by their remaining usage for every seat it fills.

Spec: R14

Evidence: open, so no skill states this claim yet. `dydo usage` is no command of the CLI: `Commands/DydoRootCommand.cs` registers none, and R14 calls the name provisional.

Layer: No command or tool an agent has reports how much of a vendor account is left.

Reason: Quota that resets unused is lost, and a seat on a spent account stalls.

**ticket-captain** (unwritten; withheld; Seats; read by TicketCaptain):

> Read `dydo usage` and balance vendors across every seat you fill.

## MergeOwn

`claim:CaptainDispatch.MergeOwn`: A Merge sub-ticket is never dispatched: the merge is run by the captain whose merge it is.

Spec: R21, S6

Evidence (run): Linear, team Dydo, read 2026-11-21: the Type group holds Merge, and DYD-1281's four Merge sub-tickets, DYD-1290 to DYD-1293, are Done under the labels Merge and AFK. This skill has no kind resource for a merge: `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs` gives its first phase Implement, Review and Research.

Layer: `delegate_task` starts a child on any ticket it is given, a Merge among them, and the child would find no recipe to load.

Reason: A second agent sent to run one command has to be briefed on the head, the tip and the gates that its sender already holds.

**ticket-captain** (unwritten; Sending; read by TicketCaptain, on claim:CaptainDispatch.Send):

> Dispatch each AFK sub-ticket with `delegate_task` once nothing blocks it, except a Merge: that step is yours.

## Send

`claim:CaptainDispatch.Send`: An AFK sub-ticket is dispatched with delegate_task once nothing blocks it; a HITL one is its cutter's to send up, never to staff.

Spec: H3, H15

Transition in A captain and its ticket (the model's own machine, not written; the hand-written chart is in `statechart/captain/`): Split → Waiting, "each AFK sub-ticket is dispatched, and a HITL one goes up".

Evidence (tool): delegate_task: "Start one child agent with only `task`. No parent history." Linear, team Dydo, labels read 2026-11-21: Mode AFK is "an agent can produce the work without live human participation", and HITL needs the user, whose side no dispatched agent can give (claim CaptainWay.Reach sends it up).

Layer: T3 offers a native sub-agent as well; only `delegate_task` gives the child a thread that outlives the dispatcher's turn.

Reason: A captain needs a thread of its own, one its ticket can be resumed from.

**ticket-captain** (unwritten; Sending; read by TicketCaptain):

> Dispatch each AFK sub-ticket with `delegate_task` once nothing blocks it, except a Merge: that step is yours.

## Refused

`claim:CaptainDispatch.Refused`: A dispatch refused for a reason other than a limit goes to the dispatcher's officer as it reads, and its ticket stays unstarted.

Spec: H3, H12

Evidence (tool): delegate_task: `runtimeMode`, "Above yours is rejected."; `orchestrator_capabilities` lists the providers and models a child can run on, so a dispatch onto any other is turned down.

Layer: T3 tells every agent of a provider or model it cannot reach to say so and never to switch unasked; it names nobody to tell, and a captain's one address is its officer.

Reason: A child that never began leaves a lane empty while its card looks merely queued, and nobody above can tell the two apart.

**ticket-captain** (unwritten; WhenThingsGoWrong; read by TicketCaptain):

> A dispatch refused for another reason goes to your officer as it reads, its ticket left unstarted.

## OwnWorktree

`claim:CaptainDispatch.OwnWorktree`: A child that writes in parallel or opens its own PR gets its own worktree; serial work shares the dispatcher's.

Spec: H3

Evidence (tool): delegate_task: `isolation`, 'shared' (default) the parent checkout, 'worktree' a "new prepared checkout from the parent commit, for a separate checkout or PR per child".

Layer: T3 makes the checkout, but only the dispatcher knows whether the child writes beside another.

Reason: Keeps two writers out of one checkout.

**ticket-captain** (unwritten; Sending; read by TicketCaptain):

> Give a child its own worktree (`isolation: "worktree"`) when it writes in parallel with others or will open its own PR; serial work shares yours.

## Lanes

`claim:CaptainDispatch.Lanes`: An officer keeps its lanes full toward a soft target of four, its own sub-tickets first, then filler.

Spec: H10

Evidence (tool): delegate_task: `mode`, default 'async', so a dispatcher runs several children at once and is woken as each returns.

Layer: T3 runs as many children as it is given and fills no lane by itself.

Reason: An idle lane is throughput lost; four is the user's goal to strive for, never a cap.

**ticket-captain** (unwritten; Lanes; read by TicketCaptain):

> Keep about four lanes full: your sub-tickets first, then filler, by Filler (`resources/filler.md`).

## Order

`claim:CaptainDispatch.Order`: Filler is an officer's pickable side tickets first, then continuous tickets.

Spec: H10, H8

Evidence (command): `dydo survey <project>`

Layer: The survey lists what could be picked up and starts nothing.

Reason: Work near the captain's own ticket is cheaper for it to hold than a stranger's.

**ticket-captain** (unwritten; FillerLead; read by TicketCaptain):

> Filler fills a lane your own sub-tickets cannot: first the side tickets `dydo survey <project>` lists as pickable, then the team's continuous tickets, CI findings and Agent friction.

## Pull

`claim:CaptainDispatch.Pull`: Filler is pulled unasked only when AFK, reversible and free of design calls, oldest first: a Bug or small Feature at should or lower, or an unrated CI finding.

Spec: H11

Evidence (run): Linear, team Dydo, labels read 2026-11-20: Mode holds AFK, Type holds Bug and Feature, Severity holds should and nit.

Layer: The labels sort what may be pulled; nothing stops an agent from pulling the rest.

Reason: Anything riskier is the user's to hand out.

**ticket-captain** (unwritten; FillerRules; read by TicketCaptain):

> Pull unasked only a ticket that is AFK, reversible and free of design calls, oldest first: a Bug or small Feature at should or lower, or an unrated CI finding.

## Pulled

`claim:CaptainDispatch.Pulled`: A continuous ticket an officer pulls follows that officer's chain.

Spec: H8

Evidence (run): Linear, team Dydo, projects read 2026-11-20: "CI findings" and "Agent friction" exist as Projects that nobody holds.

Layer: Linear groups those tickets under no officer, so a pulled one has an owner only if its puller takes it.

Reason: Without it, a pulled ticket's questions and returns would have nowhere to go.

**ticket-captain** (unwritten; FillerRules; read by TicketCaptain):

> A ticket you pull follows your chain from then on.

## Flag

`claim:CaptainDispatch.Flag`: A filler ticket's brief says so, and its captain checks what the labels cannot show.

Spec: H11

Evidence (run): Linear's labels, read 2026-11-20, carry Mode and Severity and nothing for small, reversible or free of design calls.

Layer: A puller reads a ticket's labels, and its captain reads the ticket.

Reason: The one check a label cannot make is made by the first agent that opens the work.

**ticket-captain** (unwritten; FillerRules; read by TicketCaptain):

> Say in its brief that the ticket is filler, so its captain checks what no label shows.

## Empty

`claim:CaptainDispatch.Empty`: A captain with no ticket left to dispatch or pull checks its contract against the spec: met, it finishes; not met, it finds the way to the next part and cuts it; and when all that is left waits on Questions or the user's calls, it waits on them.

Spec: H10

Evidence (run): DYD-1372, 2026-11-21: with every hop back and the user's verdict "Accepted with changes" on it, its captain held the contract against that verdict, found it unmet, and dispatched the next part, phase 1 hop A. That part went out as a brief, with no sub-ticket of its own. delegate_task: `mode`, default 'async', wakes a dispatcher as each child returns, and no field of it asks for a number of children.

Layer: T3 runs as many children as it is given and never asks a dispatcher for more; only the target of four could be misread as a quota.

Reason: An idle captain that invents tickets spends agents on something nobody asked for, and one that simply stops leaves its outcome unreached.

**ticket-captain** (unwritten; FillerEmpty; read by TicketCaptain):

> When you have no ticket left to dispatch or pull, check your contract against the spec. If it is met, finish your ticket. If it is not, find the way to the next part toward the spec and cut it as a sub-ticket, by Finding the way (`wayfind.md`). If all that is left waits on Questions or on the user's calls, wait on them.

## Route

`claim:CaptainDispatch.Route`: An out-of-scope report from a child goes to its owner's ticket or a new Bug.

Spec: R5

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Type group holds Bug.

Layer: A child reports to its dispatcher only; reaching the owner is the dispatcher's.

Reason: A report that stops at the captain is lost with the captain's thread.

**ticket-captain** (unwritten; WakeSteps; read by TicketCaptain):

> Send what a child reports outside its scope to the owner's ticket or a new Bug.

## Resume

`claim:CaptainDispatch.Resume`: A child's returned SHA is posted on its ticket as the point to resume from.

Spec: H3, S6

Evidence (run): This skill's writer was briefed to return `IMPLEMENTED DYD-1372 hop 2 at <SHA>` to its captain, which holds the ticket's record.

Layer: T3 returns the child's words to the dispatcher's thread, and that thread can die with what it knows.

Reason: A fresh agent can pick the work up from the ticket alone.

**ticket-captain** (unwritten; WakeSteps; read by TicketCaptain):

> Post the SHA a child returns on its ticket, to resume from; a return of `too big, re-cut` then goes by Too big to hold (`too-big.md`).

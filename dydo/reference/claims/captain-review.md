---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReview

## Risk

`claim:CaptainReview.Risk`: Persistence, migrations, permissions, guard rules and uncertain native interfaces are always reviewed.

Spec: R16

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` asks every PR for a PASS and cannot tell these five apart from any other change inside one.

Reason: A mistake here is costly or impossible to undo once it has run.

**ticket-captain** (Reviewed; read by TicketCaptain, on claim:CaptainReview.Other):

> Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.

## Other

`claim:CaptainReview.Other`: Every change is reviewed by an agent other than its writer, unless it is too small to need it.

Spec: R33, R12

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → InReview, "the leaf's change is whole and the merge is its holder's: another agent reviews it".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: `dydo merge` refuses a PR with no PASS line and cannot tell who wrote the line.

Reason: A writer's own context hides from it what a fresh one sees at once.

**ticket-captain** (Reviewed; read by TicketCaptain):

> Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.

## Pr

`claim:CaptainReview.Pr`: The captain that merges its work opens the PR for it, from a clean and pushed tree, before its review.

Spec: R17, R21

Evidence (command): `dydo pr-ready`

Layer: The guard refuses a `gh` PR call from a dirty or unpushed tree and reads no other way of opening one; nothing opens the PR for a captain.

Reason: The verdict's PASS line and `dydo merge` both need a PR that shows exactly the reviewed commit.

**ticket-captain** (MergePr; read by TicketCaptain):

> Open the PR for your work as a draft once `dydo pr-ready` passes: run with no body file, it checks your tree alone, clean and pushed, and asks for no review. Any check of the PR's body waits for step 3.

## Whose

`claim:CaptainReview.Whose`: Whoever merges a piece of work has it reviewed first, in its PR; a part is reviewed with the parent it joins.

Spec: R2

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a head with no PASS, so the merger finds out at the merge; nothing tells it before.

Reason: A part reviewed alone and again inside its parent is reviewed twice for one answer.

**ticket-captain** (Reviewed; read by TicketCaptain):

> If the merge is yours, open a draft PR by Merging (`resources/merging.md`), and have the work reviewed there before you merge; a part your officer merges is reviewed with its whole.

## Attach

`claim:CaptainReview.Attach`: The captain that opens a PR puts the ticket's key in its title and attaches its link to the ticket itself.

Spec: H3, K32

Evidence (tool): save_issue: `links`, "Link attachments to add [{url, title}]. Append-only; existing links are never removed". Run, Linear read 2026-11-21: nothing attached a PR by its title or its branch. PR 379 (`DYD-1372 …`, branch `DYD-1372-captain-prototype`), PR 374 (`DYD-1192: …`, `DYD-1192-remove-association-manifest`), PR 373 (`DYD-1157 fix: …`, `t3/restore-row-deadline-helpers`) and PR 372 (`DYD-1172 fix: …`, `t3/dydo-1172-safe-delete-diagnostics`) left their tickets with no attachment. The tickets that carry a PR have a hand-written title on it (DYD-1281 "PR #378: the rebuilt 4.0.0 candidate (do not merge)", DYD-1206, DYD-1196, DYD-968), and DYD-1372 carries "PR 379: DYD-1372 ticket-captain prototype" since its captain attached it with `save_issue` that day. Open, spec section 6, question 2: the user chose to keep the key in the PR's title on the belief that Linear then links it, and has yet to choose between this and setting that link up.

Layer: Linear's GitHub link attaches nothing here, by a PR's title or by a branch's name, and `dydo merge` touches no ticket.

Reason: A ticket that shows its PR leads anyone from the board to the diff and its review; without the link the two meet only in a thread that can die.

**ticket-captain** (Reviewed; read by TicketCaptain):

> Put your ticket's key in that PR's title, and attach its link to the ticket yourself, with Linear's `save_issue` (`links`). (open: his call)

## NoLoop

`claim:CaptainReview.NoLoop`: A plan, spec or contract gets no review loop; a spec gets at most one Questions pass by a native helper, chosen by risk.

Spec: R4

Evidence (tool): Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.

Layer: Nothing limits how often a document is sent for review.

Reason: First make it work, then make it good: rounds on a plan polish what building will change.

**ticket-captain** (BriefNone; read by TicketCaptain):

> Write no review brief for a plan, spec or contract: it gets no review rounds, and a risky spec at most one pass by a native helper, which returns questions.

## Severity

`claim:CaptainReview.Severity`: A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

Spec: R11

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit.

Layer: Linear holds the three labels and says nothing of what a captain does with each.

Reason: The three words decide what happens next, so they are defined before the steps that use them.

**ticket-captain** (Reviewed; read by TicketCaptain):

> A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.

## Groups

`claim:CaptainReview.Groups`: A ticket that only groups parts gets no review of its own.

Spec: R6

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → Finishing, "the children are back and the ticket only groups parts: no review of its own".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Linear nests tickets for any reason; whether a parent integrates anything is known to its captain.

Reason: A grouping adds no behaviour for a reviewer to find wrong.

**ticket-captain** (Rounds; read by TicketCaptain, on claim:CaptainReview.Whole):

> Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.

## Whole

`claim:CaptainReview.Whole`: Once its parts are wired, a parent is reviewed once: each part, the parts together, their harmony.

Spec: R2, R1

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → InReview, "the parts are wired: a Review of the whole is dispatched".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Nothing dispatches a review when the last part returns.

Reason: Early enough that nothing is built on a fault, late enough that the whole can be seen working.

**ticket-captain** (Rounds; read by TicketCaptain):

> Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.

## Record

`claim:CaptainReview.Record`: A round is recorded by the holder of its Review ticket, alone or with seats; the captain that dispatched the ticket records none of it.

Spec: S8, R33

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3. Each round's verdict is on its own Review ticket. His words (DYD-1372 aa5b445e): the review has a captain, which "will decide what kind of a review matrix to use and how to manage it's sub agents".

Layer: Each seat returns its findings to the review's captain alone, so the captain of the change never has them to put together.

Reason: One verdict per round, however many seats looked, and none of it written by a captain with work of its own under that verdict.

**ticket-captain** (Rounds; read by TicketCaptain):

> The Review's holder records the round, by A round's record (`resources/round.md`); you record none of it.

## Route

`claim:CaptainReview.Route`: The officer running the rounds routes each blocker and each should by what it shows.

Spec: R9, S17

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: A FAIL stops `dydo merge` and sends the finding nowhere; a should stops nothing at all.

Reason: Each kind has one place that can act on it, and the wrong place sits on it.

**ticket-captain** (Rounds; read by TicketCaptain):

> Route each blocker and each should, by A finding's route (`resources/findings.md`).

## Fix

`claim:CaptainReview.Fix`: Every blocker of a round on work that is broken, against the spec or a security hole goes to that round's one Fix ticket.

Spec: R9, S9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Fixing, "a round other than the third returns a blocker: a Fix ticket".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3. Read again 2026-11-21: Fix r1, r2 and r3 (DYD-1327, DYD-1333, DYD-1339) are one ticket each, with no sub-ticket, and Fix r1 closed 7 blockers. The user took one Fix ticket for all of a round's blockers as the walkthrough brief recommended it (DYD-1372 07c86aad, "4. Lgtm").

Layer: A FAIL stops `dydo merge`; it opens no ticket for the fix.

Reason: A writer that holds all of them understands the code once and repairs it in one pass.

**ticket-captain** (BlockerRoutes; read by TicketCaptain):

> Work that is broken, against the spec or a security hole: the round's one Fix ticket, by Fix rounds (`fix-rounds.md`).

## Replan

`claim:CaptainReview.Replan`: A fundamental blocker or a new requirement goes to the officer above for a re-plan.

Spec: R9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Returned, "in a round other than the third, a blocker is fundamental or a new requirement: back to the officer".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: A Fix can only repair what the plan asked for; nothing below the officer can change the plan.

Reason: Fixing toward a plan that is itself wrong builds more of the wrong thing.

**ticket-captain** (BlockerRoutes; read by TicketCaptain):

> A fundamental fault, or a new requirement: your officer, for a re-plan.

## NeedsUser

`claim:CaptainReview.NeedsUser`: A blocker that waits on something only the user can give goes to the officer at once, never to the user.

Spec: R9

Evidence (tool): t3_thread_send: "Send a message to any thread"; its `mode:'auto'` "starts an idle thread, steers a running turn, or queues".

Layer: A FAIL stops `dydo merge` and tells nobody above the captain; the officer hears of the blocker only when the captain sends it.

Reason: The user's answer is the slowest route of the three, so each hour before it is asked for lengthens the wait.

**ticket-captain** (BlockerRoutes; read by TicketCaptain):

> A need only the user can meet: your officer, at once, by A need for the user (`need-for-the-user.md`).

## OneMore

`claim:CaptainReview.OneMore`: The officer running the rounds may order one more after a round with no new blocker.

Spec: R8

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Clear → InReview, "one more round is ordered, once".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: Nothing counts rounds or closes the loop.

Reason: A round that barely passed can be looked at again without anyone inventing a blocker.

**ticket-captain** (Rounds; read by TicketCaptain, on claim:CaptainReview.Clean):

> Repeat until a round finds no new blocker; you may order one more.

## Cleared

`claim:CaptainReview.Cleared`: A round that finds no new blocker is clear, whatever shoulds and nits it found.

Spec: R8, R11

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): InReview → Clear, "the round finds no new blocker".

Evidence (run): Linear, team Dydo, Severity labels read 2026-11-20: only the blocker is described as a FAIL, "a review blocker stays a FAIL"; a should is a ticket and a nit a line.

Layer: `dydo merge` reads a PASS line and cannot tell a clear round from one whose blockers were left out.

Reason: A should can wait, so it never keeps a round from ending.

**ticket-captain** (Rounds; read by TicketCaptain, on claim:CaptainReview.Clean):

> Repeat until a round finds no new blocker; you may order one more.

## Clean

`claim:CaptainReview.Clean`: The first round with no new blocker ends the review rounds.

Spec: R8

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Clear → Finishing, "no further round is ordered: the review rounds end".

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.

Layer: `dydo merge` needs one PASS on the head and knows nothing of earlier rounds.

Reason: The loop ends on evidence, with no cap that would let a known blocker through.

**ticket-captain** (Rounds; read by TicketCaptain):

> Repeat until a round finds no new blocker; you may order one more.

## Mine

`claim:CaptainReview.Mine`: A captain never fixes a change it wrote itself: while no child has written on that change, its Fix goes to a fresh writer.

Spec: R16, R33

Evidence (tool): delegate_task: "Start one child agent with only `task`. No parent history."

Layer: Nothing keeps an agent from repairing its own work and calling it fixed.

Reason: Whoever wrote the fault tends to defend the thinking that produced it.

**ticket-captain** (FixRules; read by TicketCaptain, on claim:CaptainTrouble.FixOwner):

> Give a Fix to the thread of the last child that wrote on the change it repairs, its implementer or an earlier Fix's writer, with `t3_thread_send`, while that thread is alive and has room in its context. Dispatch a fresh writer for it, with `delegate_task`, when that context is heavy, when a blocker says the approach was wrong, or when no child has written on the change because you wrote it: you never fix a change of your own. Either way the Fix is a new ticket to its holder: brief it by Briefs (`briefs.md`), under the Fix's own key.

## OwnFinding

`claim:CaptainReview.OwnFinding`: A captain makes no call on a finding about a change it wrote: the finding stands as the reviewer wrote it, and a doubt about it is a Question.

Spec: R33, H16

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Type group holds Question, "An open question that blocks planning or implementation and is too big to settle inline." So a doubt has a ticket that a fresh captain can hold.

Layer: A round's findings all come back to the captain running it, and nothing marks the ones that fall on its own change.

Reason: Once its own change is under review, the captain is no longer impartial about that review.

**ticket-captain** (OwnChange; read by TicketCaptain):

> Take a finding on a change you wrote as the reviewer wrote it: its Fix is never yours to write, and goes out by Fix rounds (`fix-rounds.md`); if you doubt the finding, the doubt is a Question sub-ticket, never your call.

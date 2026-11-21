---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainWaking

## Mine

`claim:CaptainWaking.Mine`: A holder tells its own ticket from another's by its own thread: the ticket it set In Progress there is its own, whatever its status says since.

Spec: S7, S6

Evidence (run): This fix's writer, 2026-11-21: woken in its own thread by its captain's `t3_thread_send` for fix g2, on DYD-1372, In Progress since 2026-11-20, with its fix g1a and that fix's return above the message in the same thread. Linear, DYD-1372 read the same day: the ticket shows that it was started and names no holder.

Layer: Linear shows that a ticket was started and nothing of who started it, so the thread's own history is the one record of that.

Reason: Without the test a holder sends back, as an intruder's, the very work it was woken to go on with.

**ticket-captain** (WakeLead; read by TicketCaptain):

> A ticket you set In Progress yourself, earlier in this thread, is yours: its status is no conflict, and you need no take-over line.

## Reread

`claim:CaptainWaking.Reread`: A woken holder re-reads its ticket, its sub-tickets' statuses and the last SHA posted on each before it goes on.

Spec: H3, S6

Evidence (run): This fix's writer, 2026-11-21: woken in its own thread by its captain's `t3_thread_send` for fix g2, on DYD-1372, In Progress since 2026-11-20, with its fix g1a and that fix's return above the message in the same thread. Its own last commit was `d36ba509`, and the message that woke it said the branch had moved while it waited: "Head is `952536c3` (your commit, then the properties)".

Layer: A thread keeps what its holder knew when it stopped, and nothing in it changes when another agent commits or moves a ticket.

Reason: Two hours on, a sibling has merged or an officer has re-planned, and work resumed from a stale picture is built on a head that is gone.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> Re-read your ticket, your sub-tickets' statuses and the last SHA posted on each: the record, never your memory of it, says where things stand.

## Part

`claim:CaptainWaking.Part`: A commit returned by a child that works in a worktree of its own, a part or a fix, is merged into its officer's branch by its SHA when the merge is the officer's, and a conflict there goes to an Implement ticket; the whole is reviewed at one SHA that holds every part.

Spec: R2, H3, K14

Evidence (run): `git log` on DYD-1372's branch, read 2026-11-21: `eaeca3f0` is a two-parent merge its captain made in its own checkout, on the ticket's branch, of `fcd71737`, a commit a child made in a T3 worktree of its own, on another branch. `git rev-parse --git-common-dir`, run in a T3 worktree the same day, names one repository that all 253 worktrees share, so a child's commit is reachable by its SHA. `git merge` ran unrefused on that ticket branch: `Commands/GuardCommand.MergeRoutes.cs` refuses a PR merged around `dydo merge` and a push that updates the default branch, and holds no rule on a local merge. The Implement kind takes a merge conflict: its row of the kind table, claim Captain.Kinds.

Layer: T3 gives a child a checkout of its own and brings none of its commits back: a returned SHA stays on the child's branch until someone merges it.

Reason: A reviewer handed a head that lacks something passes a tree nobody assembled, and the missing piece first meets the others after the verdict.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> When a child that works in its own worktree returns a commit that is yours to merge, merge it into your branch with `git merge <SHA>`: the whole is reviewed at one SHA, your branch's head once it holds every part. A conflict there goes to an Implement ticket on your branch.

## Needs

`claim:CaptainWaking.Needs`: A holder woken by a need a child sends up, or by the return or the answer of a Question it opened, takes it by the steps of a need for the user.

Spec: H16, H12

Evidence (tool): t3_thread_send, read 2026-11-21: `mode:'auto'` "starts an idle thread, steers a running turn, or queues", so a child's need and an officer's answer both arrive as a message in a thread whose holder was doing something else, or nothing.

Layer: T3 delivers the message and says nothing of which steps it belongs to, and those steps sit under a heading that names the user.

Reason: Woken with somebody else's open call, a holder that finds no route answers it from its own context, which is the one thing the steps exist to prevent.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> A need a child sends up to you, and the return or the answer of a Question you opened, go by A need for the user (`need-for-the-user.md`).

## GoOn

`claim:CaptainWaking.GoOn`: A woken holder goes on from the phase it had reached once the return is taken in, its SHA posted and its commit merged; whatever that phase has it dispatch, and its finish, wait until the unneeded step is cancelled and each later step blocks the Merge.

Spec: S6, R8, S3

Evidence (tool): delegate_task: `mode`, default 'async', wakes a dispatcher as each child returns; `isolation`, 'worktree', gives the child a "new prepared checkout from the parent commit", so what it commits lies outside its dispatcher's checkout until that dispatcher merges it.

Layer: A phase says what its holder does next, and nothing of what a return needs done to it first.

Reason: A next round sent out ahead of the merge judges a head the repair never reached, and a ticket closed before the cancel leaves a dead card behind it.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> Go on from the phase you had reached; any sub-ticket it has you send out, and phase 5 of the spine, wait for the steps below.

## Unneeded

`claim:CaptainWaking.Unneeded`: A sub-ticket opened ahead that its predecessor's outcome made unnecessary is cancelled with a one-line reason, and a cancelled step blocks nothing.

Spec: S10, S3

Evidence (run): Linear, team Dydo, statuses read 2026-11-21: Canceled is a status, of the type canceled. Linear's documentation, Issue relations, read the same day: "Once the blocking issue has been resolved, the relationship moves under Related."

Layer: Linear leaves a step opened ahead in Todo for as long as nobody moves it, and goes on listing the next step as blocked by it.

Reason: A Fix nobody will ever start holds the merge behind a card that reads as work still owed.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> Cancel a sub-ticket you opened ahead that the return made unnecessary, such as the Fix of a round that passed, with a one-line reason: a Canceled step blocks nothing.

## Wired

`claim:CaptainWaking.Wired`: A Fix or Review opened after its Merge sub-ticket is made to block that Merge before anything newly ready is dispatched.

Spec: S3, R2, R8

Evidence (tool): save_issue: `blocks`, "Issue IDs/identifiers this blocks. Append-only; existing relations are never removed", beside `blockedBy`. Linear, DYD-1290 read with its relations 2026-11-21: a Merge sub-ticket of DYD-1281, Done under the labels Merge and AFK, `blockedBy` the part it merged, DYD-1287, and nothing else. So a relation stands only where a call set it.

Layer: Linear draws a block only where one was set, and a Merge opened ahead was blocked by the steps that existed that day.

Reason: Without it a second round runs beside a card that reads as free to go, and the board invites the very merge that round was ordered to hold.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> A Fix or Review you opened after your Merge sub-ticket blocks nothing until you set it: make it block the Merge, with Linear's `save_issue` (`blocks`).

## Ready

`claim:CaptainWaking.Ready`: At each return a captain dispatches the sub-tickets it unblocked, never its Merge: those still unstarted and not sent out, whose every blocker is Done or Canceled.

Spec: S3, R2

Evidence (run): Linear, DYD-1372 read with its relations 2026-11-21: a ticket lists `blocks`, `blockedBy` and `relatedTo`, each with the other ticket's key. delegate_task takes any `task`, and reads no relation and no status of the ticket the task names. Team Dydo's statuses, read the same day: Todo is the one unstarted status. Linear, DYD-1290 read with its relations 2026-11-21: a Merge sub-ticket of DYD-1281, Done under the labels Merge and AFK, `blockedBy` the part it merged, DYD-1287, and nothing else. So its Mode alone keeps no Merge out of a dispatch.

Layer: `delegate_task` starts a child on a blocked ticket, or on one a child already holds, as readily as on a free one, and the child's own intake reads its status and its brief, never its blockers.

Reason: A Review sent out ahead of its Implement has no candidate to look at, a Fix ahead of its round has no finding to close, and a second child on a running step overwrites the first.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> Dispatch each AFK sub-ticket, the Merge excepted, that is still in Todo, that you have not sent out already and that nothing blocks any more: every sub-ticket that blocked it is Done or Canceled.

## NoMerge

`claim:CaptainWaking.NoMerge`: A holder whose brief gives the merge to its officer has no Merge: once its review rounds have ended, it finishes by returning its commit.

Spec: R21, K14, R2

Evidence (run): This skill's third fixer, 2026-11-21: briefed "Do not push; no PR; no merge.", it ended on commit `c1a8e3ab` and returned that SHA, and its captain's thread, which `t3_thread_list` showed as its parent, went on from there. `Services/Merge/PullRequestMerge.cs` merges a PR by its number, and that holder had opened none.

Layer: A phase that ends in review rounds reads the same for every holder; whose the merge is, only its brief says.

Reason: Told that a Merge comes next, a part merges what its officer meant to assemble, or stalls on a command that has no PR to name.

**ticket-captain** (WakeSteps; read by TicketCaptain, on claim:CaptainWaking.MergeNext):

> Take phase 5 of the spine last, once your work has reached it: with review rounds, once they have ended. If your brief gives you the merge, the Merge is your own step, your next one once nothing blocks it, by Merging (`merging.md`); if your officer merges, you have no Merge, and you return your commit.

## MergeNext

`claim:CaptainWaking.MergeNext`: A captain's Merge sub-ticket is never dispatched at a wake: where the merge is the captain's, once its review rounds have ended and nothing blocks the Merge, the captain runs it itself, as its own step, in the last phase.

Spec: R21, S6, R8

Evidence (run): Linear, DYD-1290 read with its relations 2026-11-21: a Merge sub-ticket of DYD-1281, Done under the labels Merge and AFK, `blockedBy` the part it merged, DYD-1287, and nothing else. This skill holds no kind resource for a merge: `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs` gives its first phase Implement, Review and Research, so a child sent onto a Merge would find no recipe to load.

Layer: Linear lists an unblocked Merge among the tickets free to go and cannot say that this one has no holder but its captain.

Reason: Left as one more card to hand out, it goes to a child with nothing to load, or waits for a taker that will never come.

**ticket-captain** (WakeSteps; read by TicketCaptain):

> Take phase 5 of the spine last, once your work has reached it: with review rounds, once they have ended. If your brief gives you the merge, the Merge is your own step, your next one once nothing blocks it, by Merging (`merging.md`); if your officer merges, you have no Merge, and you return your commit.

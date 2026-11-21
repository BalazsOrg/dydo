---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainMergeTrouble

## Resolver

`claim:CaptainMergeTrouble.Resolver`: On a conflict the captain dispatches an Implement ticket to resolve it on the ticket's branch.

Spec: R21, R19

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: On a conflict `dydo merge` refuses, merges nothing and resolves nothing.

Reason: A resolution is new code, and the captain about to merge it must not be its writer.

**ticket-captain** (ConflictSteps; read by TicketCaptain):

> Dispatch an Implement ticket to resolve the conflict on your ticket's branch.

## Saved

`claim:CaptainMergeTrouble.Saved`: The resolution's commit SHA is posted on the Merge sub-ticket.

Spec: K9, H3

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: each merge is its own ticket, such as DYD-1291.

Layer: The resolver returns its SHA to the captain's thread, which can end before the merge does.

Reason: The merge can be resumed from the ticket by whoever holds it next.

**ticket-captain** (ConflictSteps; read by TicketCaptain):

> Post the resolution's commit SHA on the Merge sub-ticket, or on your own ticket where your work was not cut into steps.

## Hunks

`claim:CaptainMergeTrouble.Hunks`: A conflict's resolution gets a fresh review of its hunks.

Spec: R19

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Repairing → InReview, "the repair is in: a fresh review of what it changed".

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses the new head until a PASS names it, and says nothing of who may give one.

Reason: A resolution is the one part of a merge that somebody wrote by hand.

**ticket-captain** (ConflictSteps; read by TicketCaptain):

> Dispatch a fresh reviewer for the resolution's hunks, the only new work on the branch.

## Again

`claim:CaptainMergeTrouble.Again`: The merge runs again once the hunks pass, on the new head.

Spec: R19, R23

Evidence (command): `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run`

Layer: `dydo merge` merges only the head it is given, and the resolution made a new one.

Reason: The pin on the old head would refuse, and rightly.

**ticket-captain** (ConflictSteps; read by TicketCaptain):

> Merge again, on the new head, once that review passes.

## Stuck

`claim:CaptainMergeTrouble.Stuck`: A conflict the captain cannot get resolved goes to its officer.

Spec: K9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Repairing → Returned, "a conflict that cannot be resolved goes to the officer".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: Two tickets that collide have two captains, and neither can re-order the other's work.

Reason: The officer above both is the first one able to sequence them.

**ticket-captain** (ConflictEnd; read by TicketCaptain):

> Take a conflict you cannot get resolved to your officer, with both sides named.

## Rerun

`claim:CaptainMergeTrouble.Rerun`: A gate red after the merge is rerun once.

Spec: R22

Evidence (tool): ci_rerun: "Re-run a PR's failing checks, or one named check."

Layer: Hosted CI reports red and stays red until somebody reruns it.

Reason: One rerun tells a flake from a break at the cost of a few minutes.

**ticket-captain** (RedLead; read by TicketCaptain):

> Rerun the red run once.

## Dytest

`claim:CaptainMergeTrouble.Dytest`: A gate red after the merge is rerun through DYTEST, which counts a flake as a result.

Spec: R22

Evidence: open, so no skill states this claim yet. DYTEST is not built: no command of the CLI is DYTEST (`Commands/DydoRootCommand.cs` registers none), so the rerun is hosted CI's, started by hand.

Layer: Hosted CI reruns a job and forgets that it failed the first time.

Reason: A wobble that leaves a trace becomes a ticket once it repeats; an untracked one fails merges for ever.

**ticket-captain** (withheld; RedLead; read by TicketCaptain):

> Run the rerun through DYTEST, so that a flake is counted.

## Flake

`claim:CaptainMergeTrouble.Flake`: Green on the rerun is a flake, and the merge is done.

Spec: R22

Evidence (code): `.github/workflows/ci.yml`

Layer: CI keeps no count of which runs turned green on a second try.

Reason: A merge that is sound is not reverted for a test that wobbled.

**ticket-captain** (RedLead; read by TicketCaptain):

> Green on the rerun is a flake: the merge is done.

## Revert

`claim:CaptainMergeTrouble.Revert`: A real red after a merge is reverted on the feature branch at once, by the captain.

Spec: R22

Evidence (run): Each layer read 2026-11-20, no revert yet made: GitHub reports `feature/dydo-4-0-0` unprotected and under no ruleset; the guard's own corpus lets `git push --force-with-lease origin feature/x` through (`GuardShellCorpusTests`), its merge-route block naming only the default branch; the feature branch's history holds no revert.

Layer: The guard stops a push to the default branch only, and nothing reverts a red merge by itself.

Reason: Everyone else builds on the feature branch, and every hour it stays red is theirs too.

**ticket-captain** (RedSteps; read by TicketCaptain):

> Revert the merge on the feature branch at once: `git revert -m 1 <merge SHA>`, pushed.

## Cancelled

`claim:CaptainMergeTrouble.Cancelled`: A reverted Merge is cancelled with its reason, and what broke becomes a CI-findings ticket.

Spec: R22, S12

Evidence (run): Linear, team Dydo, read 2026-11-20: the status Canceled exists, and so does the Project "CI findings".

Layer: Linear leaves the Merge sub-ticket open after a revert, as if the merge still stood.

Reason: The record says why the merge is gone, and the break has a ticket of its own.

**ticket-captain** (RedSteps; read by TicketCaptain):

> Cancel the Merge sub-ticket with `reverted: <gate> red on <merge SHA>`, and file what broke as a ticket in the team's CI findings; where your work was not cut into steps, write that line on your own ticket, which stays open.

## Repair

`claim:CaptainMergeTrouble.Repair`: After a revert, an Implement ticket fixes it on the ticket's branch, then a review round scoped to the fix.

Spec: R22

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` merges only a head with a PASS, and the fix makes a new head.

Reason: The rest of the branch already passed, so the round looks at what changed.

**ticket-captain** (RedSteps; read by TicketCaptain):

> Dispatch an Implement ticket to fix it on your ticket's branch, then a review round scoped to the fix.

## Reland

`claim:CaptainMergeTrouble.Reland`: The fixed branch is merged again through a PR that itself restores the reverted change.

Spec: R22, R23

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: The spec has `dydo merge` revert the revert; the command as built does not, so the branch must.

Reason: A re-merge of the old commits would land nothing and still report success.

**ticket-captain** (RedSteps; read by TicketCaptain):

> Merge the fixed branch again through a new PR that brings the reverted change back itself: `dydo merge` undoes no revert, and calls the old PR already integrated.

## Lost

`claim:CaptainMergeTrouble.Lost`: A red gate the captain cannot get fixed goes to its officer.

Spec: K9

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Repairing → Returned, "a red gate that cannot be fixed goes to the officer".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: A reverted merge leaves the ticket open with nobody but its captain watching it.

Reason: The officer can re-plan around a part that will not go in.

**ticket-captain** (RedEnd; read by TicketCaptain):

> Take a red you cannot get fixed to your officer, with the merge SHA and the gate.

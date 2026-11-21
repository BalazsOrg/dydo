---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainMerging

A line marked unwritten is the model's own, of a skill written by hand in `skills/roles/officers/ticket-captain/`: the build writes it nowhere, and the hand-written skill need not hold it.

## Mine

`claim:CaptainMerging.Mine`: Only the holder whose brief gives it the merge merges: a holder whose officer merges opens no PR, cuts no Merge sub-ticket and takes none of the merging steps.

Spec: R21, K14

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` merges whatever PR it is given and for whoever runs it; whose the merge is, it cannot know.

Reason: Run by a part, the command lands that part ahead of the whole its officer was to assemble and have judged.

**ticket-captain** (unwritten; MergeLead; read by TicketCaptain):

> Merge only if your brief gives you the merge: where your officer merges, you open no PR, cut no Merge sub-ticket and take none of these steps.

## NoPass

`claim:CaptainMerging.NoPass`: A PASS that stands on a Review ticket alone merges nothing: a head reviewed outside its PR needs a round in that PR.

Spec: R17, R23

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a PR whose body holds no PASS for its head, and reads no ticket to find one.

Reason: Copied into the body by the captain it favours, a verdict stops being the reviewer's word, so the cure is a fresh look where the command can see it.

**ticket-captain** (unwritten; MergeRules; read by TicketCaptain):

> A PASS that stands on a Review ticket alone merges nothing, since `dydo merge` reads only the PR's body: a head reviewed outside its PR needs a round in it.

## Begin

`claim:CaptainMerging.Begin`: The merging captain sets its Merge sub-ticket In Progress when it begins the merge, and closes it itself.

Spec: R20, S6

Evidence (run): Linear, DYD-1281's Merge sub-tickets read 2026-11-21: DYD-1290 to DYD-1293, one a merge, each Done under the labels Merge and AFK.

Layer: Linear leaves a Merge sub-ticket in Todo until someone moves it, and `dydo merge` touches no ticket.

Reason: The board then shows a merge under way, and a card left in Todo invites a second captain to begin the same one.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain):

> Set your Merge sub-ticket In Progress: the merge is your own step, never dispatched, and you close it yourself. Where your work was not cut into steps, your own ticket is the record instead.

## Gates

`claim:CaptainMerging.Gates`: The captain runs the project's gates on the combined tree before it merges.

Spec: R21, R18

Evidence (code): `.github/workflows/ci.yml`

Layer: Hosted CI runs when a PR is pushed and does not run again when its target moves.

Reason: Two branches that are each green can still break each other once they meet.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain):

> Run the project's gates, as your brief names them, on the combined tree: your branch merged with the target's current tip.

## Draft

`claim:CaptainMerging.Draft`: A PR is opened as a draft and stays one through its review: a draft opens on the tree check alone, its body can be written to, and a check of that body waits until the PR is made ready.

Spec: R17, R23

Evidence (run): `dydo pr-ready`, run with no body file in DYD-1372's checkout at `13da419b`, clean and pushed, 2026-11-21: "ready: … is clean and HEAD matches its upstream", exit 0, and no review was asked for. The guard runs those tree checks and no other on a call that opens a PR or makes one ready (`Commands/GuardCommand.PrReady.cs`, `Services/PrReadyCheck.cs`), and `Services/PrCallScanner.cs` reads no draft flag, so a draft opens on the same check. GitHub, asked the same day about PR 356, an open draft: `viewerCanUpdate` is true, so its body can be written to, and hosted CI had run on it: `.github/workflows/ci.yml` starts on every pull request, a draft included.

Layer: The guard and the bare check ask the same of a draft as of any PR, and nothing else marks an unreviewed PR as not yet for merging; only `dydo merge` tells the two apart, by refusing the draft.

Reason: Opened as a draft, the work can be judged in place while no command will merge it, and the one check that needs a PASS has a later moment to belong to.

**ticket-captain** (unwritten; MergePr; read by TicketCaptain, on claim:CaptainReview.Pr):

> Open the PR for your work as a draft once `dydo pr-ready` passes: run with no body file, it checks your tree alone, clean and pushed, and asks for no review. Any check of the PR's body waits for step 3.

## Heading

`claim:CaptainMerging.Heading`: A PR's body carries the Independent review heading from the moment the PR is opened: the round's holder writes its PASS under it.

Spec: R17

Evidence (code): `Services/Merge/IndependentReview.cs`

Layer: `dydo merge` reads a verdict only inside that section, and the guard stops, once, a PR opened with those two words nowhere in it (`Services/ConfigFactory.cs`); neither writes the heading.

Reason: A reviewer finds where its line goes, and a captain is spared a stop that reads as if the review had to come before the PR.

**ticket-captain** (unwritten; MergePr; read by TicketCaptain):

> Give its body the heading `## Independent review`: the round's holder writes its PASS under it.

## BodyCheck

`claim:CaptainMerging.BodyCheck`: A brief that names the PR skill adds one check of the PR's body, which needs the PASS, so it comes when the PR is made ready and never before the draft is opened.

Spec: R17, K14

Evidence (run): `dydo pr-ready --body <file>`, run in DYD-1372's checkout 2026-11-21 on a body whose Independent review section held no PASS: "not ready: the '## Independent review' section holds no reviewer 'PASS - rubric/type: ...' block with usable fields", exit 1; on that body with a PASS line and a ticket key: "ready: … the body names its ticket and holds a PASS review", exit 0. `skills/engineering/pr/SKILL.md` asks for that run "before opening or readying the PR".

Layer: The PR skill names two moments for its check and cannot know that at the first of them no review exists yet; the command only reports that the PASS is missing.

Reason: Taken at the opening, the check can never pass, and a captain that obeys both skills waits on a review that waits on its PR.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain, on claim:CaptainMerging.Ready):

> Make the PR ready, no longer a draft, once its body holds the PASS for its head: `dydo merge` refuses a draft. Where your brief names `/pr`, that skill's check of the body comes here, on the body as the PR now holds it: `dydo pr-ready --body <file>`.

## Ready

`claim:CaptainMerging.Ready`: A draft PR is made ready once its body holds the PASS for its head, before the merge, which refuses a draft.

Spec: R23, R17

Evidence (run): `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run`, run 2026-11-21 on PR 356, an open draft into `feature/dydo-4-0-0`: "refused: PR #356 is a draft", exit 1. `Services/Merge/PullRequestMerge.cs` checks that first of all on an open PR, before it looks for a PASS.

Layer: `dydo merge` refuses a draft and readies none; the guard lets a PR be made ready from any clean, pushed tree and reads no body for it.

Reason: Readied only after its PASS, a PR never stands open for merging with nothing but its author's word behind it.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain):

> Make the PR ready, no longer a draft, once its body holds the PASS for its head: `dydo merge` refuses a draft. Where your brief names `/pr`, that skill's check of the body comes here, on the body as the PR now holds it: `dydo pr-ready --body <file>`.

## Refusal

`claim:CaptainMerging.Refusal`: dydo merge states why it refuses.

Spec: R23

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: The command prints its reason; nothing tells the captain that the reason is the next thing to act on.

Reason: A refusal read as a failure gets retried; read as an answer, it gets fixed.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain, on claim:CaptainMerging.Merge):

> Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.

## Merge

`claim:CaptainMerging.Merge`: A captain merges into a feature branch with dydo merge, naming the reviewed head and the target tip it gated against.

Spec: R23, R21

Evidence (command): `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run`

Layer: The guard blocks every other way to merge a PR and names this command; the two SHAs are the captain's to supply.

Reason: The pins tie the merge to what was reviewed and gated, so nothing pushed since can ride along.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain):

> Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.

## Ci

`claim:CaptainMerging.Ci`: A merge is done once hosted CI is green on the merge commit.

Spec: R20, R18

Evidence (code): `.github/workflows/ci.yml`

Layer: CI runs on every push to `feature/**` and reports to GitHub, where no ticket sees it.

Reason: The merge commit is the first time the combined tree runs on the host everyone trusts.

**ticket-captain** (unwritten; MergeSteps; read by TicketCaptain):

> Close the Merge sub-ticket once hosted CI is green on the merge commit.

## Receipt

`claim:CaptainMerging.Receipt`: A merge is done only once DYTEST also holds a green receipt on the merge tree.

Spec: R20

Evidence: open, so no skill states this claim yet. DYTEST is not built: no command of the CLI is DYTEST (`Commands/DydoRootCommand.cs` registers none), so nothing issues a receipt to wait for.

Layer: Hosted CI runs its own jobs on the merge commit and keeps no record of the gates the captain ran on the combined tree.

Reason: A proof keyed by the tree shows the gates passed on exactly what was merged, with nobody's word taken for it.

**ticket-captain** (unwritten; withheld; MergeSteps; read by TicketCaptain):

> Before you close it, wait for DYTEST's green receipt on the merge tree as well.

## Archive

`claim:CaptainMerging.Archive`: After a merge the captain archives the branches and worktrees its dispatches created.

Spec: N10, N12 DR 045 §8

Evidence: open, so no skill states this claim yet. Dry runs with the built CLI, 2026-11-20, on this ticket's own T3-made worktree and its branch: `dydo archive worktree` refused, "T3 ownership/activity unavailable … target retained", and `dydo archive branch` refused, "Branch is checked out elsewhere". Every worktree a dispatch creates is T3's (`Services/SafeDeleteT3Policy.cs`; spec CA19, DYD-1216).

Layer: T3 makes a child's worktree and leaves it standing: `git worktree list` on the writer's host showed 237.

Reason: Merged checkouts pile up on the host, and an archive can be undone where a delete cannot.

**ticket-captain** (unwritten; withheld; MergeSteps; read by TicketCaptain):

> Archive each branch and worktree your dispatches created, with `dydo archive branch <target>` and `dydo archive worktree <target>`; the archive keeps them 30 days.

## Clean

`claim:CaptainMerging.Clean`: A conflict-free merge into the feature branch gets no merge review.

Spec: R18

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a hand-made conflict resolution's head until it has its own PASS, and asks nothing more of a clean one.

Reason: A clean merge adds no line anyone wrote, so the gates and CI prove it.

**ticket-captain** (unwritten; MergeRules; read by TicketCaptain):

> A clean merge gets no review.

## Broken

`claim:CaptainMerging.Broken`: A conflict or a real red gate before the merge goes to an Implement ticket on the ticket's branch.

Spec: R21, R19

Transition in A captain and its ticket (the model's own machine, not written; the hand-written chart is in `statechart/captain/`): Finishing → Repairing, "a conflict or a real red gate: an Implement ticket repairs it".

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: On a conflict `dydo merge` refuses and merges nothing; it repairs neither that nor a red gate.

Reason: A captain that repairs the work it is about to merge has written what nobody reviewed.

**ticket-captain** (unwritten; MergeRules; read by TicketCaptain):

> A conflict, or a gate that is really red, goes to an Implement ticket on your branch.

## Repaired

`claim:CaptainMerging.Repaired`: A repair made before the merge, of a conflict or of a red gate, returns to a fresh review of what it changed, and the merge is then run again on the new head; one that cannot be made goes to the officer.

Spec: R19, R21, R23

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a head no PASS names, and a repair always makes a new head; it sends the repair to no reviewer.

Reason: A fix written to turn a gate green is new code like any other, and unread it would be the one line nobody judged.

**ticket-captain** (unwritten; MergeRules; read by TicketCaptain):

> Either repair comes back the way A merge conflict (`merge-conflict.md`) lays out: a fresh reviewer judges what it changed, and you merge again from step 2, on the new head. A red you cannot get fixed goes to your officer, with the gate.

## Mutation

`claim:CaptainMerging.Mutation`: No ticket or merge waits on mutation testing.

Spec: T12

Evidence: open, so no skill states this claim yet. `dydo/guides/testing-strategy.md` still lists `gate mutation --since BASE` among the project's gates, and DYTEST Mutation, which T12 moves it to, is not built.

Layer: Which gates a merge waits on is whatever the brief names, and the brief may still name this one.

Reason: Mutation runs take hours, and a merge that waits on them holds a lane the whole time.

**ticket-captain** (unwritten; withheld; MergeRules; read by TicketCaptain):

> Nothing waits on mutation testing.

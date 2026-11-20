---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainMerging

## Gates

`claim:CaptainMerging.Gates`: The captain runs the project's gates on the combined tree before it merges.

Spec: R21, R18

Evidence (code): `.github/workflows/ci.yml`

Layer: Hosted CI runs when a PR is pushed and does not run again when its target moves.

Reason: Two branches that are each green can still break each other once they meet.

**ticket-captain** (MergeSteps; read by TicketCaptain):

> Run the project's gates, as your brief names them, on the combined tree: your branch merged with the target's current tip.

## Refusal

`claim:CaptainMerging.Refusal`: dydo merge states why it refuses.

Spec: R23

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: The command prints its reason; nothing tells the captain that the reason is the next thing to act on.

Reason: A refusal read as a failure gets retried; read as an answer, it gets fixed.

**ticket-captain** (MergeSteps; read by TicketCaptain, on claim:CaptainMerging.Merge):

> Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.

## Merge

`claim:CaptainMerging.Merge`: A captain merges into a feature branch with dydo merge, naming the reviewed head and the target tip it gated against.

Spec: R23, R21

Evidence (command): `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run`

Layer: The guard blocks every other way to merge a PR and names this command; the two SHAs are the captain's to supply.

Reason: The pins tie the merge to what was reviewed and gated, so nothing pushed since can ride along.

**ticket-captain** (MergeSteps; read by TicketCaptain):

> Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.

## Ci

`claim:CaptainMerging.Ci`: A merge is done once hosted CI is green on the merge commit.

Spec: R20, R18

Evidence (code): `.github/workflows/ci.yml`

Layer: CI runs on every push to `feature/**` and reports to GitHub, where no ticket sees it.

Reason: The merge commit is the first time the combined tree runs on the host everyone trusts.

**ticket-captain** (MergeSteps; read by TicketCaptain):

> Close the Merge sub-ticket once hosted CI is green on the merge commit.

## Archive

`claim:CaptainMerging.Archive`: After a merge the captain archives the branches and worktrees its dispatches created.

Spec: N10, N12 DR 045 §8

Evidence: open, so no skill states this claim yet. Dry runs with the built CLI, 2026-11-20, on this ticket's own T3-made worktree and its branch: `dydo archive worktree` refused, "T3 ownership/activity unavailable … target retained", and `dydo archive branch` refused, "Branch is checked out elsewhere". Every worktree a dispatch creates is T3's (`Services/SafeDeleteT3Policy.cs`; spec CA19, DYD-1216).

Layer: T3 makes a child's worktree and leaves it standing: `git worktree list` on the writer's host showed 237.

Reason: Merged checkouts pile up on the host, and an archive can be undone where a delete cannot.

**ticket-captain** (withheld; MergeSteps; read by TicketCaptain):

> Archive each branch and worktree your dispatches created, with `dydo archive branch <target>` and `dydo archive worktree <target>`; the archive keeps them 30 days.

## Clean

`claim:CaptainMerging.Clean`: A conflict-free merge into the feature branch gets no merge review.

Spec: R18

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a hand-made conflict resolution's head until it has its own PASS, and asks nothing more of a clean one.

Reason: A clean merge adds no line anyone wrote, so the gates and CI prove it.

**ticket-captain** (MergeRules; read by TicketCaptain):

> A clean merge gets no review.

## Broken

`claim:CaptainMerging.Broken`: A conflict or a real red gate before the merge goes to an Implement ticket on the ticket's branch.

Spec: R21, R19

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Finishing → Repairing, "a conflict or a real red gate: an Implement ticket repairs it".

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: On a conflict `dydo merge` refuses and merges nothing; it repairs neither that nor a red gate.

Reason: A captain that repairs the work it is about to merge has written what nobody reviewed.

**ticket-captain** (MergeRules; read by TicketCaptain):

> A conflict, or a gate that is really red, goes to an Implement ticket on your branch.

## Mutation

`claim:CaptainMerging.Mutation`: No ticket or merge waits on mutation testing.

Spec: T12

Evidence: open, so no skill states this claim yet. `dydo/guides/testing-strategy.md` still lists `gate mutation --since BASE` among the project's gates, and DYTEST Mutation, which T12 moves it to, is not built.

Layer: Which gates a merge waits on is whatever the brief names, and the brief may still name this one.

Reason: Mutation runs take hours, and a merge that waits on them holds a lane the whole time.

**ticket-captain** (withheld; MergeRules; read by TicketCaptain):

> Nothing waits on mutation testing.

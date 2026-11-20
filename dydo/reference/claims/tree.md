---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Tree

## Purpose

`claim:Tree.Purpose`: The working-tree standard is built from the same claims as the role skills.

Spec: K12, D10

**working-tree-standard** (TreeIntro):

> How a Project's work moves through branches, worktrees, hops and merges; every rule here is a claim, built from the same model as the role skills.

## NoSelfReview

`claim:Tree.NoSelfReview`: No agent judges its own step.

Spec: R33, K29

**working-tree-standard** (TreeInvariants):

> No self-review. An agent never reviews its own work or rules on findings about it: its own context biases it, and a fresh context catches what it can't. A leaf's evidence check is a fresh seat the officer sends. Each level checks for itself, never taking a lower level's verdict as its proof. A captain authors only a change too small to need its own review; the review above covers it.

**ticket-captain** (legacy, not built; Verdict, nominal):

> Never rule on findings about work you authored.

**reviewer** (Briefed, nominal):

> Never judge work you implemented or fixed.

## OwnedPaths

`claim:Tree.OwnedPaths`: One writer per worktree; commits touch owned paths only.

Spec: N12 DR 045 §8

**implementer** (Working, nominal):

> One writer per worktree: commit only inside your owned paths.

**working-tree-standard** (TreeInvariants):

> One writer per worktree, and commits touch owned paths only.

## Commits

`claim:Tree.Commits`: Every writer hop ends on a commit named for its hop, kept unsquashed; merges are merge commits only.

Spec: R32

**implementer** (Working, nominal):

> End the hop on one commit `<KEY> <hop>: <what>`, `<hop>` being `implement`, `fix` or `merge`, never squashed; a merge is a merge commit that keeps both parents.

**working-tree-standard** (TreeInvariants):

> Every writer hop ends on a commit `<KEY> <hop>: <what>`, where `<hop>` is `implement`, `fix` or `merge`, kept unsquashed. Merges are merge commits only, never squash or rebase.

## Branch

`claim:Tree.Branch`: A ticket branch is named by its key off the feature branch, or off main for an atomic ticket; branch, base and worktree are posted before the first edit.

Spec: N12 DR 045 §8

**ticket-captain** (legacy, not built; Briefed, nominal):

> Name the ticket branch `<KEY>-<slug>` off the feature branch, or off main for an atomic ticket, so Linear attaches the branch and PR; post branch, base SHA and worktree path on the ticket before the first edit.

**working-tree-standard** (TreeInvariants):

> A ticket branch is `<KEY>-<slug>` off its feature branch, or off main for an atomic ticket, so Linear attaches the branch and PR; its branch, base SHA and worktree path are on the ticket before the first edit.

## LandingCandidate

`claim:Tree.LandingCandidate`: The landing candidate is the feature branch with master merged in.

Spec: N6

**working-tree-standard** (TreeBranches):

> The landing candidate is the feature branch with master merged in: the exact tree master gets.

## CrewWork

`claim:Tree.CrewWork`: Crew work inside their ticket's contract, owned paths and gates, and commit their own hop.

Spec: K14, R32

**working-tree-standard** (TreeDelegation):

> Crew work inside their ticket's contract, owned paths and gates, and commit their own hop.

## CrewLimits

`claim:Tree.CrewLimits`: Ordinary crew do not create the ticket's branch, open its PR or merge it.

Spec: K29

**working-tree-standard** (TreeDelegation):

> Ordinary crew do not create its branch, open its PR or merge it.

**ticket-captain** (legacy, not built; Briefed, nominal):

> Create the ticket's branch, open its PR and merge it yourself; ordinary crew never do.

**implementer** (Working, nominal):

> Your hop is commits on the brief's branch; the branch, its PR and its merge are the captain's.

## Nesting

`claim:Tree.Nesting`: Sub-captains are allowed with no hard cap, cut only for coherent units; bad nesting is a finding.

Spec: H13, H14

**working-tree-standard** (TreeDelegation):

> Sub-captains are allowed with no hard cap, cut only for coherent units too big for one agent or able to run in parallel; the practical ceiling is coordinator → captain → sub-captain → second-order sub-captain → crew, and bad nesting, or a deeper sub-captain that fails, is dealt with as a finding.

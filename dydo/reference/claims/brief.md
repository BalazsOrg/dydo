---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Brief

## ImplementerBrief

`claim:Brief.ImplementerBrief`: The implementer dispatch template's slots.

Spec: K14

**ticket-captain** (legacy, not built; ImplementerBrief):

> Ticket and contract · branch, worktree and base · owned paths · gates · proof recipe · findings (Fix only) · model and effort · return to.

## ReviewerBrief

`claim:Brief.ReviewerBrief`: The reviewer dispatch template's slots.

Spec: K14, R3, R6, R34

**ticket-captain** (legacy, not built; ReviewerBrief):

> Ticket and contract · the level: a leaf's evidence check, a ticket review, a feature or Milestone review, or an inquisition slice · the governing spec, or none · lenses · branch, worktree and base (the candidate SHA) · owned paths · gates · proof recipe · model and effort · return to.

## ScoutBrief

`claim:Brief.ScoutBrief`: The scout dispatch template's slots.

Spec: K14, R4

**ticket-captain** (legacy, not built; ScoutBrief):

> The question, or the spec for a scout pass · model and effort · return to.

**research** (ScoutBrief):

> The question · the one source family to search · model and effort · return to.

## CaptainBrief

`claim:Brief.CaptainBrief`: The ticket-captain dispatch template's slots; a sub-captain is briefed from it too.

Spec: K14, H23

**ticket-captain** (legacy, not built; TicketCaptainBrief):

> Ticket and contract · branch, worktree and base · owned paths · gates · proof recipe · model and effort · return to. A sub-captain or a job ticket's captain is briefed from this template too, adding its job's skill from the table below.

**coordinator** (TicketCaptainBrief):

> Ticket and contract · branch, worktree and base · owned paths · gates · proof recipe · model and effort · return to.

## ResearchBrief

`claim:Brief.ResearchBrief`: The research dispatch template's slots.

Spec: K14, K30

**ticket-captain** (legacy, not built; ResearchBrief):

> The question, and the choice it waits on · model and effort · return to.

## Lenses

`claim:Brief.Lenses`: The officer names the lenses in the brief; lenses shift up the tree.

Spec: R6

**ticket-captain** (legacy, not built; Dispatching):

> Name the lenses in each brief, shifting up the tree: it works (with a security baseline), then it integrates, then optimal, secure, performant and in harmony; never review a first version as if it were final.

## SpecInBrief

`claim:Brief.SpecInBrief`: A reviewer's brief names the governing spec or says there is none.

Spec: R3

**ticket-captain** (legacy, not built; Dispatching):

> Name the governing spec in each reviewer's brief, or say there is none.

## ScoutPass

`claim:Brief.ScoutPass`: No review loop on a plan, spec or contract; a spec gets at most one scout pass.

Spec: R4

**ticket-captain** (legacy, not built; Dispatching):

> A plan, spec or contract gets no review loop; give a spec at most one scout pass, chosen by risk, which returns Questions and is never repeated.

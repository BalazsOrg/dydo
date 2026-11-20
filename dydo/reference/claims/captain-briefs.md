---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainBriefs

## NoPeek

`claim:CaptainBriefs.NoPeek`: A dispatcher briefs without loading the child's kind resource; the child loads its own.

Spec: K14, K29

Evidence (run): This skill's writer loaded its two skills from their names in the brief; nothing of them was pasted in.

Layer: Nothing stops a dispatcher from reading every resource in the folder.

Reason: A dispatcher that reads the recipe starts steering how the work is done.

**ticket-captain** (BriefLead; read by TicketCaptain):

> Brief without loading the child's kind resource: the child loads its own.

## Ticket

`claim:CaptainBriefs.Ticket`: A brief names the ticket and its kind.

Spec: K14

Evidence (run): This skill's writer got such a brief (DYD-1372 hop 2): "a code-writer on Linear DYD-1372 (team Dydo, Project dydo 4.0.0), hop 2 of 3".

Layer: T3 knows the thread and Linear the ticket; the brief is the only thing that joins them for the child.

Reason: The kind is how the child picks its resource.

**ticket-captain** (BriefAll; read by TicketCaptain):

> The ticket, by its key, and its kind.

## Skills

`claim:CaptainBriefs.Skills`: A brief names /ticket-captain and every other skill the ticket needs, each with a slash.

Spec: K14

Evidence (tool): Skill: "Users may also ask for one by name (`/<name>`, or \"slash command\"); that's a request to invoke it."

Layer: The host lists skills and leaves loading them to the agent's own judgement.

Reason: Named skills get loaded; skills left to judgement get loaded sometimes.

**ticket-captain** (BriefAll; read by TicketCaptain):

> `/ticket-captain`, then every other skill the ticket needs, each with a slash.

## Merger

`claim:CaptainBriefs.Merger`: A brief names who merges the work.

Spec: K14, R21

Evidence (run): This skill's writer got such a brief (DYD-1372 hop 2): "Do not push; no PR; no merge."

Layer: Nothing in a checkout says whose the merge is.

Reason: It decides how the child finishes, and two agents never both merge.

**ticket-captain** (BriefAll; read by TicketCaptain):

> Who merges the work: the child, or you.

## Change

`claim:CaptainBriefs.Change`: A change's brief names its owned paths and gates, and a Fix's the findings it closes.

Spec: K14

Evidence (run): This skill's writer got such a brief (DYD-1372 hop 2): it named the gates before return and "Commit only owned paths".

Layer: The guard blocks a few paths for everyone and knows nothing of one ticket's.

Reason: Two writers with stated paths cannot overwrite each other.

**ticket-captain** (BriefAll; read by TicketCaptain):

> For a change: the paths it owns and the project's gates it must pass. For a Fix, also the findings it closes.

## Asked

`claim:CaptainBriefs.Asked`: A question's brief names the question and the choice that waits on it.

Spec: K14

Evidence (tool): delegate_task: "Start one child agent with only `task`. No parent history."

Layer: The child cannot see the conversation the question came out of.

Reason: Knowing the choice tells the researcher what would count as an answer.

**ticket-captain** (BriefAll; read by TicketCaptain):

> For a question: the question, and the choice that waits on it.

## Candidate

`claim:CaptainBriefs.Candidate`: A review brief names the candidate by its SHA.

Spec: K14, R17

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` merges only the SHA a PASS names, so the reviewer has to be told which one to judge.

Reason: A verdict on a moving branch is a verdict on nothing.

**ticket-captain** (BriefReview; read by TicketCaptain):

> The candidate, by its SHA.

## Governing

`claim:CaptainBriefs.Governing`: A review brief names the governing spec, or says there is none.

Spec: R3

Evidence (run): This skill's writer got such a brief (DYD-1372 hop 2): it named the approved spec's file and the sections to read.

Layer: A ticket links its contract; nothing leads a reviewer from there to the spec.

Reason: A review held only to the contract passes a wrong rule the contract inherited.

**ticket-captain** (BriefReview; read by TicketCaptain):

> The governing spec, or that there is none.

## LeafCheck

`claim:CaptainBriefs.LeafCheck`: A leaf gets no full review, only an optional evidence check by a fresh seat.

Spec: S4, R6

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: The level table has a row for a leaf, and nothing keeps a dispatcher from asking more of one.

Reason: The review of the whole covers each part, so a full review of one part is paid for twice.

**ticket-captain** (BriefReview; read by TicketCaptain, on claim:CaptainBriefs.Level):

> The level: a leaf's evidence check, a ticket review, or a feature or Milestone review. A leaf gets at most the check, never a full review.

## Level

`claim:CaptainBriefs.Level`: A review brief names the level.

Spec: R34

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: The review resource holds a row per level and cannot know which one this candidate is at.

Reason: The level sets how much review the candidate gets, and review is expensive.

**ticket-captain** (BriefReview; read by TicketCaptain):

> The level: a leaf's evidence check, a ticket review, or a feature or Milestone review. A leaf gets at most the check, never a full review.

## Lenses

`claim:CaptainBriefs.Lenses`: A review brief names the lenses, which shift up the tree; a first version is never reviewed as final.

Spec: R6

Evidence (run): DYD-1281's rounds, read 2026-11-20: each Review ticket's title says what it looks at, "whole-candidate" in r1 to r3.

Layer: Only the dispatcher knows how far up the tree the candidate sits.

Reason: Polishing a first version spends review on code the next part will change.

**ticket-captain** (BriefReview; read by TicketCaptain):

> The lenses, which rise up the tree: it works, with a security baseline; then it integrates; then it is optimal, secure, performant and in harmony. A first version never gets a final one's lenses.

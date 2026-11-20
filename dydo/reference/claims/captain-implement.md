---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainImplement

## Change

`claim:CaptainImplement.Change`: An Implement-kind ticket is a change: a contract implemented, a review's blockers fixed, or a merge conflict resolved.

Spec: K31

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: The ticket names which of the three it is; that one resource serves all three is said only here.

Reason: A writer sees at once that the rest of the page is its whole recipe.

**ticket-captain** (ImplementLead; read by TicketCaptain):

> Your ticket is a change: implement a contract, fix a review's blockers, or resolve a merge conflict.

## Verify

`claim:CaptainImplement.Verify`: A writer verifies its work while it builds it, each file the way it is used.

Spec: K31

Evidence (run): This skill was written so: its writer rebuilt after each edit and read the built files, never the claims alone.

Layer: Gates run at the end, and only on what they were told to check.

Reason: A fault caught while the file is open costs a minute; caught at review, a round.

**ticket-captain** (ImplementLead; read by TicketCaptain):

> Verify your work while you build it, each file the way it is used: run the code and drive the app, render an image and look at it, hold a doc against its source.

## NoWorse

`claim:CaptainImplement.NoWorse`: A writer leaves what it touched no worse than it found it.

Spec: K31

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: No gate compares a file with how it read before the change.

Reason: Decay that every change adds a little of is never anyone's ticket.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> Leave what you touched no worse than you found it.

## Limit

`claim:CaptainImplement.Limit`: Every changed file stays within the file token limit, tests included.

Spec: T15

Evidence (code): `tools/SkillBuild/changed-file-sizes.sh`

Layer: The size check runs as a gate, after the file is already written.

Reason: A file an agent can read whole is one it can change without missing half of it.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> Keep every file you change at or under 20,000 bytes (5k tokens at bytes ÷ 4), tests included, and split one that passes it.

## Paths

`claim:CaptainImplement.Paths`: One writer per worktree, committing only inside its ticket's owned paths.

Spec: R32, H3

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "You are the only writer in it" and "Commit only owned paths".

Layer: git takes any path from any writer; the ticket's paths are known only to its contract.

Reason: A commit that strays into another ticket's files is a conflict someone else pays for.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> One writer per worktree: commit only inside the paths your ticket owns.

## Skills

`claim:CaptainImplement.Skills`: A writer loads domain-modeling for a glossary term or a Decision Record, and diagnosing-bugs for a Bug with no red reproduction.

Spec: K14, K31

Evidence (code): `skills/engineering/domain-modeling/SKILL.md`

Layer: A brief names the skills its writer foresaw; these two depend on what the work turns up.

Reason: Each holds a method the writer would otherwise improvise.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> Load /domain-modeling when you touch a glossary term or a Decision Record, and /diagnosing-bugs for a Bug with no red reproduction yet.

## Design

`claim:CaptainImplement.Design`: An open design question is not the writer's to settle; a prototype answers one.

Spec: H16

Evidence (code): `skills/engineering/prototype/SKILL.md`

Layer: Nothing stops a writer from choosing a design by building it.

Reason: A design chosen in passing is found at review, after it has been built on.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> An open design question is a Question sub-ticket, never yours to settle; /prototype is the skill that answers one.

## Red

`claim:CaptainImplement.Red`: A Bug starts from a red reproduction, and each new test is shown failing without the code.

Spec: T3

Evidence (run): This ticket's generator change was made so: two tests failed, on the Resources list and on an unlinked resource, before `SkillRenderer` changed.

Layer: A test runner reports green; it cannot say whether the test could ever have been red.

Reason: A test never seen failing proves nothing about the code it sits beside.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> A Bug starts from a red reproduction, and every new test is shown failing without the code.

## Weed

`claim:CaptainImplement.Weed`: A mistake found after it landed gets its fix and a lint, test or rule that keeps it from recurring.

Spec: R30

Evidence (code): `tools/SkillBuild/TruthCheck.cs`

Layer: A fix closes its ticket whether or not anything stops the same mistake next time.

Reason: The candidate's lists numbered `1. 1. 1.` reached the user once; the build now fails on one.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> A fix for a mistake that had already landed carries the lint, test or rule that keeps it from coming back.

## Source

`claim:CaptainImplement.Source`: On a Fix, each blocker's severity is checked against the source, not its label.

Spec: S9, R11

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: The Fix ticket carries the reviewer's label and none of the code behind it.

Reason: A mislabelled finding is otherwise fixed as the wrong thing.

**ticket-captain** (ImplementFix; read by TicketCaptain):

> Check each blocker's severity against the source, not its label.

## Only

`claim:CaptainImplement.Only`: A Fix closes only the blockers, preferring the change that deletes or narrows code.

Spec: S9

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: Nothing bounds what a fixer changes while it is in the file.

Reason: Review never grows the change, so the next round has less to read, not more.

**ticket-captain** (ImplementFix; read by TicketCaptain):

> Fix only the blockers, preferring the change that deletes or narrows code.

## Dispute

`claim:CaptainImplement.Dispute`: A fixer puts a label it disputes in its return, with evidence.

Spec: R11

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: A severity the fixer silently ignores looks, on the ticket, like a blocker left open.

Reason: The dispute reaches the record, where the next reviewer can weigh it.

**ticket-captain** (ImplementFix; read by TicketCaptain):

> Put a label you dispute in your return, with its evidence: file:line, test or output.

## Branch

`claim:CaptainImplement.Branch`: A conflict is resolved on the ticket's branch only, as a merge commit that keeps both parents.

Spec: R19, R32

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` merges only a head that carries a PASS, so a resolution made anywhere else never reaches review.

Reason: Both parents keep each side's history readable when the resolution is questioned.

**ticket-captain** (ImplementConflict; read by TicketCaptain):

> Resolve it on the ticket's branch only, as a merge commit that keeps both parents.

## Hunks

`claim:CaptainImplement.Hunks`: A resolver reads both sides and what changed since review before it resolves a hunk; a new product choice goes back as a Question.

Spec: R19, H16

Evidence (code): `skills/roles/crew/implementer/resources/merge.md`

Layer: git marks where two sides differ and nothing of what either side meant.

Reason: A hunk resolved by picking a side can drop behaviour the other side was reviewed for.

**ticket-captain** (ImplementConflict; read by TicketCaptain):

> Read both sides, and what changed since the source was reviewed, before you resolve a hunk; one that needs a new product choice is a Question.

## Reparent

`claim:CaptainImplement.Reparent`: A shoulds batch's captain moves each should under its ticket and keeps a link to where it was found.

Spec: S17

Evidence (run): Linear, DYD-1281 read 2026-11-20: a ticket carries `relatedTo` links beside its parent.

Layer: Linear drops the old parent when a ticket is moved.

Reason: The link is the only trace of which review found it.

**ticket-captain** (ImplementShoulds; read by TicketCaptain):

> Move each should under your ticket, keeping a `related: found in <KEY> rN` link on it.

## Cancel

`claim:CaptainImplement.Cancel`: A should is cancelled only with a one-line reason.

Spec: S18, S17

Evidence (run): Linear, team Dydo, statuses read 2026-11-20: Canceled exists.

Layer: Linear cancels a ticket without asking why.

Reason: A should is a real fault, so dropping one has to be answerable.

**ticket-captain** (ImplementShoulds; read by TicketCaptain, on claim:CaptainImplement.Duplicate):

> Mark a duplicate Duplicate, and cancel a should only with a one-line reason.

## Duplicate

`claim:CaptainImplement.Duplicate`: A duplicate should is marked Duplicate.

Spec: S17

Evidence (run): Linear, team Dydo, statuses read 2026-11-20: Duplicate exists.

Layer: Shoulds from different reviews can name one fault, and nothing compares them.

Reason: One fault is then fixed once and counted once.

**ticket-captain** (ImplementShoulds; read by TicketCaptain):

> Mark a duplicate Duplicate, and cancel a should only with a one-line reason.

## Commit

`claim:CaptainImplement.Commit`: A writer's hop ends on a commit named for its ticket and hop, never squashed.

Spec: R32

Evidence (run): `git log` on this ticket's branch, 2026-11-20: "DYD-1372 implement: …" and, below it, "DYD-1339 fix: …".

Layer: git takes any message, and a squash erases the hops a review was run against.

Reason: A reviewer finds the exact commit each hop ended on.

**ticket-captain** (ImplementEnd; read by TicketCaptain):

> End on a commit `<KEY> <hop>: <what>`, `<hop>` being `implement`, `fix` or `merge`, never squashed.

## Receipts

`claim:CaptainImplement.Receipts`: A writer's receipts are the commands it ran on its commit and their results.

Spec: R5

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): its return lists "the gate commands with their exits".

Layer: A return that says only that it passed gives the officer nothing to check.

Reason: A reviewer then judges through other lenses instead of running the same thing again.

**ticket-captain** (ImplementEnd; read by TicketCaptain):

> Your receipts are the commands you ran on that commit and their results.

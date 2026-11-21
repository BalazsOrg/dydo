---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainImplement

## Change

`claim:CaptainImplement.Change`: An Implement-kind ticket is a change: a contract implemented, a review's findings fixed, or a merge conflict resolved.

Spec: K31

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: The ticket names which of the three it is; that one resource serves all three is said only here.

Reason: A writer sees at once that the rest of the page is its whole recipe.

**ticket-captain** (ImplementLead; read by TicketCaptain):

> Your ticket is a change: implement a contract, fix a review's findings, or resolve a merge conflict.

## Verify

`claim:CaptainImplement.Verify`: A writer verifies its work while it builds it, each file the way it is used.

Spec: K31

Evidence (run): This skill was written so: its writer rebuilt after each edit and read the built files, never the claims alone.

Layer: Gates run at the end, and only on what they were told to check.

Reason: A fault caught while the file is open costs a minute; caught at review, a round.

**ticket-captain** (ImplementLead; read by TicketCaptain):

> Verify your work while you build it, each file the way it is used: run the code and drive the app, render an image and look at it, hold a doc against its source.

## Parts

`claim:CaptainImplement.Parts`: The implement resource heads each part with the work it is for, and a holder reads the parts that name its own.

Spec: K31, K30

Evidence (code): `tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs`

Layer: A page shows every recipe to every reader, and only the reader knows whether its ticket is a Bug, a doc or a conflict.

Reason: A writer on a small Fix reaches its three lines without a detour through recipes meant for somebody else.

**ticket-captain** (ImplementLead; read by TicketCaptain):

> Below the rules for every change, each part is headed by the work it is for: read the ones that name yours.

## Phases

`claim:CaptainImplement.Phases`: An Implement holder uses the spine's first, second and last phases, and dispatches or gets a review only when the merge is its own or it splits.

Spec: K30, R2

Evidence (code): `claims/statechart/captain/transitions.json`

Layer: The spine is one text for every kind of ticket, and nothing in it marks the phases a kind leaves out.

Reason: A writer told at the top that sending and review belong to others goes straight to the change.

**ticket-captain** (ImplementMap; read by TicketCaptain):

> | Spine phase | You |
> | -- | -- |
> | 1. Take the ticket | use |
> | 2. Do it or split it | use: a leaf, unless the change proves too big |
> | 3. Dispatch | skip; use it only if the merge is yours, or you split |
> | 4. Get it reviewed | skip, since a part is reviewed with its whole; use it only if the merge is yours, or you split |
> | 5. Finish | use: the case your brief names |
> | When things go wrong | use |

## NoWorse

`claim:CaptainImplement.NoWorse`: A writer leaves what it touched no worse than it found it.

Spec: K31

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: No gate compares a file with how it read before the change.

Reason: Decay that every change adds a little of is never anyone's ticket.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> Leave what you touched no worse than you found it.

## Limit

`claim:CaptainImplement.Limit`: Every changed file stays within the file size limit the brief's gates check, tests included.

Spec: T15, T14

Evidence (run): This hop's brief named the gate among its gates: `bash tools/SkillBuild/changed-file-sizes.sh`, which exits 1 on a changed file over its limit. The briefs resource has every change's brief name "the project's gates it must pass".

Layer: The size check runs as a gate, after the file is already written.

Reason: A file an agent can read whole is one it can change without missing half of it.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> Keep every file you change, tests included, within the file size limit your brief's gates check, and split one that passes it.

## LimitNumber

`claim:CaptainImplement.LimitNumber`: The file size limit is 5k tokens, counted as bytes ÷ 4, unless the project's configuration sets another.

Spec: T15, T14

Evidence: open, so no skill states this claim yet. The number is this repository's alone as the system runs: only `tools/SkillBuild/changed-file-sizes.sh` and this suite check 20,000 bytes. T14 makes the number configuration, and no key of `dydo.json`, no command of the CLI and no file of the scaffold holds it, so nothing tells another project's holder its limit.

Layer: No shipped gate or setting carries the number yet, so a holder elsewhere would learn it only from this page.

Reason: A number stated here would bind every adopting project to this repository's choice.

**ticket-captain** (withheld; ImplementRules; read by TicketCaptain):

> Unless your project's configuration says otherwise, the limit is 5k tokens, counted as bytes ÷ 4.

## Paths

`claim:CaptainImplement.Paths`: One writer per worktree, committing only inside its ticket's owned paths.

Spec: R32, H3

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "You are the only writer in it" and "Commit only owned paths".

Layer: git takes any path from any writer; the ticket's paths are known only to its contract.

Reason: A commit that strays into another ticket's files is a conflict someone else pays for.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> One writer per worktree: commit only inside the paths your ticket owns.

## Seam

`claim:CaptainImplement.Seam`: A writer loads codebase-design when it designs a seam or an interface.

Spec: K14, K31

Evidence (code): `skills/engineering/codebase-design/SKILL.md`

Layer: A brief names the skills its dispatcher foresaw, and a seam shows up only once the work is open.

Reason: Where a boundary is drawn decides how testable everything behind it will be.

**ticket-captain** (ImplementRules; read by TicketCaptain, on claim:CaptainImplement.Skills):

> Load /domain-modeling when you touch a glossary term or a Decision Record, and /codebase-design when you design a seam or an interface.

## Skills

`claim:CaptainImplement.Skills`: A writer loads domain-modeling when it touches a glossary term or a Decision Record.

Spec: K14, K31

Evidence (code): `skills/engineering/domain-modeling/SKILL.md`

Layer: A brief names the skills its writer foresaw; these depend on what the work turns up.

Reason: Each holds a method the writer would otherwise improvise.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> Load /domain-modeling when you touch a glossary term or a Decision Record, and /codebase-design when you design a seam or an interface.

## Design

`claim:CaptainImplement.Design`: An open design question is not the writer's to settle; a prototype answers one.

Spec: H16

Evidence (code): `skills/engineering/prototype/SKILL.md`

Layer: Nothing stops a writer from choosing a design by building it.

Reason: A design chosen in passing is found at review, after it has been built on.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> An open design question is a Question sub-ticket, never yours to settle; /prototype is the skill that answers one.

## Weed

`claim:CaptainImplement.Weed`: A mistake found after it landed gets its fix and a lint, test or rule that keeps it from recurring.

Spec: R30

Evidence (code): `tools/SkillBuild/TruthCheck.cs`

Layer: A fix closes its ticket whether or not anything stops the same mistake next time.

Reason: The candidate's lists numbered `1. 1. 1.` reached the user once; the build now fails on one.

**ticket-captain** (ImplementRules; read by TicketCaptain):

> A fix for a mistake that had already landed carries the lint, test or rule that keeps it from coming back.

## Source

`claim:CaptainImplement.Source`: On a Fix, each finding's severity is checked against the source, not its label.

Spec: S9, R11

Evidence (code): `skills/roles/crew/implementer/SKILL.md`

Layer: The Fix ticket carries the reviewer's label and none of the code behind it.

Reason: A mislabelled finding is otherwise fixed as the wrong thing.

**ticket-captain** (ImplementFix; read by TicketCaptain):

> Check each finding's severity against the source, not its label.

## Only

`claim:CaptainImplement.Only`: A Fix closes only the findings its brief names, its blockers and any cheap should beside them, preferring the change that deletes or narrows code.

Spec: S9, S17

Evidence (run): This Fix came so (DYD-1372 fix r1b): its brief listed five blockers and four shoulds by name, "Shoulds, fix them all", and closed with "Report anything you find beyond this list."

Layer: Nothing bounds what a fixer changes while it is in the file.

Reason: Review never grows the change, so the next round has less to read, not more.

**ticket-captain** (ImplementFix; read by TicketCaptain):

> Close only the findings your brief names, preferring the change that deletes or narrows code.

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

## Parents

`claim:CaptainImplement.Parents`: A resolver returns both parents with the merge commit's SHA.

Spec: R19, R32

Evidence (code): `skills/roles/crew/implementer/resources/merge.md`

Layer: A merge commit holds its parents, and a return that names only the new head makes its reader dig them out.

Reason: The fresh reviewer of the hunks compares the resolution with each side it came from.

**ticket-captain** (ImplementConflict; read by TicketCaptain, on claim:CaptainImplement.Intent):

> State the behaviour each hunk you resolved is meant to have, and return both parents with the merge commit's SHA.

## Intent

`claim:CaptainImplement.Intent`: A resolver states the behaviour each resolved hunk is meant to have.

Spec: R19

Evidence (code): `skills/roles/crew/implementer/resources/merge.md`

Layer: A resolved hunk looks like any other code, with nothing to show that two intents met there.

Reason: Its reviewer judges the hunk against a stated intent, and need not guess one.

**ticket-captain** (ImplementConflict; read by TicketCaptain):

> State the behaviour each hunk you resolved is meant to have, and return both parents with the merge commit's SHA.

## Refactor

`claim:CaptainImplement.Refactor`: A resolution that refactors leaves the code no worse than either side.

Spec: R19, K31

Evidence (code): `skills/roles/crew/implementer/resources/merge.md`

Layer: git accepts any content as a resolution, well beyond the lines that were in conflict.

Reason: A conflict is where two designs meet, and the lazy way out keeps the weaker of them.

**ticket-captain** (ImplementConflict; read by TicketCaptain):

> A resolution that refactors leaves the code no worse than either side had it.

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

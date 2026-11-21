---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainResearch

## Phases

`claim:CaptainResearch.Phases`: A Research holder uses the spine's first, second and last phases, and skips dispatch and review.

Spec: K30, S1, R2

Evidence (run): The spine as built, read 2026-11-21: phase 4 of `skills/roles/officers/ticket-captain/SKILL.md` opens "Every change is reviewed by an agent other than its writer", and nowhere in that phase is an answer named. Spec R2 has "The feature is the review unit"; R4 rules out a review loop for a plan, spec or contract, and an answer is none of the three. Linear, read the same day: the Research tickets DYD-1072 and DYD-1253 are Done with no sub-ticket, so no Review ticket stands beneath either.

Layer: The spine is one text for every kind of ticket, and nothing in it marks the phases a kind leaves out. No spec rule and no word of the user's speaks of reviewing an answer: the skip of phase 4 says only what this skill holds, a phase written for a change.

Reason: A researcher would otherwise wonder who is to check its answer, or open tickets for its lookups.

**ticket-captain** (ResearchMap; read by TicketCaptain):

> | Spine phase | You |
> | -- | -- |
> | 1. Take the ticket | use |
> | 2. Do it or split it | use: a leaf, whose scouts are native helpers and never sub-tickets |
> | 3. Dispatch | skip, unless you split |
> | 4. Get it reviewed | skip: an answer is no change, so no round is opened on it |
> | 5. Finish | use: the Nobody case |
> | When things go wrong | use |

## Passages

`claim:CaptainResearch.Passages`: A scout returns exact passages with their citations, from the source that owns the claim, and says what it searched where it found nothing.

Spec: K30, K22

Evidence (tool): Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.

Layer: A scout is a native helper that loads no skill, so what it must bring back reaches it only in its prompt.

Reason: A summary cannot be verified, and an empty report hides whether the family was searched at all.

**ticket-captain** (ResearchSteps; read by TicketCaptain, on claim:CaptainResearch.Scouts):

> Send one scout per family in play, as native helpers in parallel, each with the question and its one family; ask each for exact passages with their citations, from the source that owns the claim, and for what it searched where it found nothing. Read a lone family yourself.

## Asked

`claim:CaptainResearch.Asked`: A Research-kind ticket is one question a choice waits on, answered with every claim cited to an opened passage.

Spec: K30

Evidence (code): `skills/roles/crew/research/SKILL.md`

Layer: The ticket states the question; what makes an answer worth trusting is said only here.

Reason: Whoever waits on the choice acts on the answer without the means to check it.

**ticket-captain** (ResearchLead; read by TicketCaptain):

> Your ticket is one question a choice waits on. Answer it with every claim cited to a passage you opened.

## Restate

`claim:CaptainResearch.Restate`: Research restates its question and says what would count as an answer.

Spec: K30

Evidence (code): `skills/roles/crew/research/SKILL.md`

Layer: A question arrives in its asker's words, shaped by a context the researcher never saw.

Reason: A misread question is found here, before any source is opened.

**ticket-captain** (ResearchSteps; read by TicketCaptain):

> Restate the question in one sentence, and say what would count as an answer.

## Families

`claim:CaptainResearch.Families`: Research marks each source family in or out, with a reason, and reads governing docs on the default branch too.

Spec: K30, K22

Evidence (run): This skill's writer met it: the spec said `dydo merge` reverts a revert, and the code on its branch holds no such thing.

Layer: A search returns what it was pointed at and stays silent about the family nobody opened.

Reason: The answer often sits in the one family that was assumed to be empty.

**ticket-captain** (ResearchSteps; read by TicketCaptain):

> Mark each source family in or out, with a reason: decision records, the spec and plans, other docs, code and tests, git history, Linear, outside sources. Read governing docs on the default branch too; a branch can lag.

## Scouts

`claim:CaptainResearch.Scouts`: Research sends one scout per source family in play, as native helpers in parallel, and reads a lone family itself.

Spec: K30, S1

Evidence (tool): Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.

Layer: The host starts a helper on request; that a lookup is a helper's job and never a ticket is said only here.

Reason: One lookup leaves nothing anyone but the researcher uses, so a ticket for it is overhead.

**ticket-captain** (ResearchSteps; read by TicketCaptain):

> Send one scout per family in play, as native helpers in parallel, each with the question and its one family; ask each for exact passages with their citations, from the source that owns the claim, and for what it searched where it found nothing. Read a lone family yourself.

## Open

`claim:CaptainResearch.Open`: Research opens the passage behind every load-bearing claim and confirms it.

Spec: K30, K22

Evidence (tool): Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.

Layer: A helper's report is a summary, and the researcher sees nothing of how it was made.

Reason: A scout's mistake comes back as confident as its findings.

**ticket-captain** (ResearchSteps; read by TicketCaptain):

> Open the passage behind every load-bearing claim, and confirm it says what the scout said.

## Conflicts

`claim:CaptainResearch.Conflicts`: Where sources conflict, research names the conflict and the governing source; where they are silent, the point is unsettled.

Spec: K30, K22

Evidence (code): `skills/roles/crew/research/SKILL.md`

Layer: Nothing separates what a source says from what a reader inferred from it.

Reason: An inferred fact that reads like a found one is how a wrong rule gets built on.

**ticket-captain** (ResearchSteps; read by TicketCaptain):

> Where sources conflict, name the conflict and the source that governs; where they are silent, mark the point unsettled rather than reason your way to a fact.

## Answer

`claim:CaptainResearch.Answer`: Research writes the answer first, then each claim with its citation and passage, what stayed unsettled, and every family searched.

Spec: K30, K22

Evidence (code): `skills/roles/crew/research/SKILL.md`

Layer: Only the researcher knows which families it opened and found empty.

Reason: Its reader checks any claim in one step, and sees where nobody looked.

**ticket-captain** (ResearchSteps; read by TicketCaptain):

> Write the answer first, then each claim with its citation (a path at its commit, or a URL) and its exact passage, what stayed unsettled, and every family searched, the empty ones included.

## Big

`claim:CaptainResearch.Big`: A research holder whose question is too big for one agent divides it as any holder does, what it has answered so far, written on its ticket with its citations, standing in for the commit, and each part a Research sub-ticket: one question that can be answered on its own.

Spec: H2, K30

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Type group holds Research, "A factual answer whose investigation needs its own owner, status or evidence", which "Closes on cited findings on the Issue": a research result stands on its ticket, and no commit carries it. save_comment: `issueId` and `body`, a comment on that issue. delegate_task: "Start one child agent with only `task`. No parent history.", so a part's captain starts from its ticket alone.

Layer: The too-big scenario saves work as a commit and its SHA, and a holder whose work is an answer makes neither; nothing else says what such a holder saves before it divides.

Reason: Parts cut from a written state search only what is still open, and whoever takes the ticket over finds what was already established.

**ticket-captain** (ResearchBig; read by TicketCaptain):

> Divide it by Too big to hold (`too-big.md`), what you have answered so far standing in for its commit: write that on your ticket, each claim with its citation, and make each part a Research sub-ticket, one question that can be answered on its own.

## Parts

`claim:CaptainResearch.Parts`: A research holder whose parts are back checks their answers as it checks a scout's, then takes up its own work again and writes its one answer from theirs and its own.

Spec: K30, H2

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → Leaf, "a research holder's parts are back: it checks their answers and takes up its own work again, to write its one answer".

Evidence (tool): delegate_task, read 2026-11-21: `mode`, "Default 'async'. 'wait' only if this turn needs the result.", so its dispatcher goes on, or waits, until each part's return reaches its thread. A part puts its answer on its own ticket and returns one line (claim CaptainFinish.Nothing); Linear's list_comments reads it there.

Layer: Each part's answer sits on its own ticket, and nothing gathers them onto the parent's or holds one against another.

Reason: A part's mistake comes back as confident as a scout's, and an answer stitched from unchecked parts hands it to whoever decides.

**ticket-captain** (ResearchBig; read by TicketCaptain):

> When the parts are back, their answers are on their tickets: take each as you would a scout's report, from step 4 on, and write the one answer from theirs and yours.

## First

`claim:CaptainResearch.First`: A Question's captain researches first, and lays out a call its research leaves open neutrally: the call, its options unranked and the facts with their sources.

Spec: H16, L18

Evidence (run): Linear, DYD-1071 read 2026-11-21: a Question the user is to settle, whose body is all its reader has of the call. Spec L18, of the file such a call travels in: "only the call, the options unranked, the constraints, and the primary facts with their sources".

Layer: Linear takes any text as a Question's body, a recommendation as readily as a neutral account, and nothing reads the sources before somebody is asked.

Reason: Whoever decides weighs the options without the researcher's leaning, and a mistake in the research poisons no judge.

**ticket-captain** (ResearchQuestion; read by TicketCaptain):

> Research first, then go by what the facts leave of the call. An open call is laid out the same way wherever it goes: the call, its options unranked and the facts with their sources.

## Settled

`claim:CaptainResearch.Settled`: A call the facts settle is answered on the Question ticket.

Spec: H16, L18

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Type group holds Question, "Resolution: the answer, posted on close".

Layer: Linear closes a Question with or without an answer on it.

Reason: Most calls end here, and whoever meets the same one later finds it already answered.

**ticket-captain** (ResearchRoutes; read by TicketCaptain):

> They settle it: put the answer on the ticket.

## Users

`claim:CaptainResearch.Users`: A call its research leaves open is the user's to make when it cannot be undone once made, or when making it sets or changes a decision record.

Spec: L18, L21, H16

Evidence (run): `skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked, expects it of the captain it puts on a need: that captain "answers the need, sends it to a council, or puts a `his-call` Question on your HITL queue". Spec L18 names the calls that end with the user whatever a council says, "one-way or DR-grade → always prepared", and L21: "Irreversible calls are never decided." A researcher has read the decision records and knows each option, so both marks are in its own findings.

Layer: Linear's Mode label says who settles a Question and nothing of which calls are his; the council that weighs every other open call has no resource in this skill yet.

Reason: Two marks a reader can check against its notes keep a nod-seeking question away from him, and keep an agent from settling what only he may.

**ticket-captain** (ResearchRoutes; read by TicketCaptain, on claim:CaptainResearch.Handed):

> They leave a call that is the user's to make, because it cannot be undone once made or because making it sets or changes a decision record: open a Question sub-ticket under yours, Mode HITL, by Sub-tickets (`sub-tickets.md`), lay the call out on it, and have it block every ticket yours blocks. Your ticket has then handed its call on: close it Done, and return the new ticket's key to your officer. That return is how the call goes up: your officer carries the ticket on as it is, by A need for the user (`need-for-the-user.md`).

## Handed

`claim:CaptainResearch.Handed`: A Question's captain whose research leaves a call that is the user's opens a HITL Question sub-ticket for it, with the call laid out and blocking what its own ticket blocks, closes its own ticket, and returns the new ticket's key, which goes up as it is.

Spec: H23, H16, S10

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Finishing, "a Question's research leaves a call that is the user's: it is handed on as a HITL Question sub-ticket, and the research ticket has done its part".

Evidence (tool): save_issue: `parentId`, "Parent issue ID or identifier", and `blocks`, "Issue IDs/identifiers this blocks. Append-only; existing relations are never removed". Run, Linear read 2026-11-21: DYD-1071 is a Question labelled HITL that blocks the nine tickets waiting on it; and DYD-1172 has been Done since 2026-11-18 while its child DYD-1184 is In Progress, so a parent's close moves no child.

Layer: Linear keeps a block on the ticket it was set on: once the research ticket closes, what waited on it reads as free unless the new ticket blocks it, and nothing opens a ticket for the user in a researcher's place.

Reason: Sent back as merely unsettled, such a call is researched again one level up and never reaches the single queue that can end it.

**ticket-captain** (ResearchRoutes; read by TicketCaptain):

> They leave a call that is the user's to make, because it cannot be undone once made or because making it sets or changes a decision record: open a Question sub-ticket under yours, Mode HITL, by Sub-tickets (`sub-tickets.md`), lay the call out on it, and have it block every ticket yours blocks. Your ticket has then handed its call on: close it Done, and return the new ticket's key to your officer. That return is how the call goes up: your officer carries the ticket on as it is, by A need for the user (`need-for-the-user.md`).

## Question

`claim:CaptainResearch.Question`: A Question's captain returns to its officer as unsettled, with the call laid out, any call its research neither settles nor finds to be the user's.

Spec: H16, L18

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Returned, "a Question's research leaves a call that is neither settled nor the user's: it goes back to the officer unsettled, the Question still open".

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Type group holds Question, and no label names a council or a Question's stage.

Layer: Where an unsettled call goes next is its route, and the council that takes most of them has no resource in this skill yet.

Reason: An officer often holds what the sources lacked, and it is the one place the remaining routes start from.

**ticket-captain** (ResearchRoutes; read by TicketCaptain):

> They leave anything else open: lay the call out on your ticket, and return it to your officer as unsettled, the Question still open. Your officer sends the answer to your thread once there is one: woken with it, close your ticket Done on that answer and return.

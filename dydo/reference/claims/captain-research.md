---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainResearch

## Phases

`claim:CaptainResearch.Phases`: A Research holder uses the spine's first, second and last phases, and skips dispatch and review.

Spec: K30, S1

Evidence (code): `claims/statechart/captain/transitions.json`

Layer: The spine is one text for every kind of ticket, and nothing in it marks the phases a kind leaves out.

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

## Question

`claim:CaptainResearch.Question`: A Question's captain researches first, answers what the facts settle, and returns the rest to its officer as unsettled, with the call laid out.

Spec: H16, L18

Evidence (run): Linear, team Dydo, labels read 2026-11-20: the Type group holds Question, "Resolution: the answer, posted on close"; it holds no label for a council yet.

Layer: Where an unsettled call goes next is its route, and the council that takes most of them has no resource in this skill yet.

Reason: Options left unranked let whoever decides weigh them without the researcher's leaning.

**ticket-captain** (ResearchQuestion; read by TicketCaptain):

> Research first. When the facts settle the call, put the answer on the ticket. When they do not, put on it the call, its options unranked and the facts with their sources, and return it to your officer as unsettled, the Question still open.

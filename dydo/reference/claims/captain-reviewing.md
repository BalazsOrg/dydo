---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReviewing

A line marked unwritten is the model's own, of a skill written by hand in `skills/roles/officers/ticket-captain/`: the build writes it nowhere, and the hand-written skill need not hold it.

## One

`claim:CaptainReviewing.One`: A Review ticket is one candidate judged at the level its brief names, and its verdict is its holder's.

Spec: R34, S8

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: The brief names the candidate and the level; that the verdict has one owner is said only here.

Reason: A verdict nobody owns is a list of remarks that nobody acts on.

**ticket-captain** (unwritten; ReviewLead; read by TicketCaptain):

> Your ticket is a review: one candidate, judged at the level your brief names. Hold the review, and its verdict is yours; sit as one seat of it, and your findings are.

## NotMine

`claim:CaptainReviewing.NotMine`: No agent judges work it implemented or fixed.

Spec: R33

Evidence (tool): t3_thread_send: "A send to a finished child thread starts a fresh task", so a writer's thread can be handed its own work to review.

Layer: T3 will hand a review to any thread, the writer's included.

Reason: Its own context hides from a writer exactly what a fresh reader would catch.

**ticket-captain** (unwritten; ReviewLead; read by TicketCaptain):

> Never judge work you implemented or fixed.

## Phases

`claim:CaptainReviewing.Phases`: A Review holder uses the spine's first, second and last phases, dispatches only to send seats, and skips the review phase.

Spec: K30, R34

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-21: Review r1 to r4 (DYD-1294, DYD-1328, DYD-1334, DYD-1340) are Done with no sub-ticket, and no Review ticket beside them judges one of them. His words (DYD-1372 777d396e): "Reviewing the reviews is probably not a good idea." Of a seat (aa5b445e): "Yes, they're leafs. And we know that they are.", and of a read review whose slice is too big, "it can be further divided (while keeping the coherence)".

Layer: The spine is one text for every kind of ticket, and nothing in it marks the phases a kind leaves out.

Reason: Without the map a reviewer works out for itself that the phase named for review is about somebody else's change.

**ticket-captain** (unwritten; ReviewMap; read by TicketCaptain):

> | Spine phase | You hold the review | You are one seat of it |
> | -- | -- | -- |
> | 1. Take the ticket | use | use |
> | 2. Do it or split it | use: a leaf, or a split into seats | use: a leaf, unless your slice proves too big |
> | 3. Dispatch | skip, unless you send seats | skip, unless you divide a slice too big to hold |
> | 4. Get it reviewed | skip | skip |
> | 5. Finish | use: the Nobody case | use: the Nobody case |
> | When things go wrong | use | use |

## NoReview

`claim:CaptainReviewing.NoReview`: A verdict is never reviewed, and neither are a seat's findings.

Spec: R34, K30

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-21: Review r1 to r4 (DYD-1294, DYD-1328, DYD-1334, DYD-1340) are Done with no sub-ticket, and no Review ticket beside them judges one of them. His words: "Reviewing the reviews is probably not a good idea." (DYD-1372 777d396e)

Layer: The spine's fourth phase speaks to every captain alike and cannot tell a holder whose result is a judgement.

Reason: A judgement sent on to be judged again has no end, and each further pass costs as much as the first.

**ticket-captain** (unwritten; ReviewAll; read by TicketCaptain):

> A verdict is never reviewed: phase 4 is for a change, so no round is opened on your verdict or on a seat's findings, and what you return is your result.

## Anchor

`claim:CaptainReviewing.Anchor`: A review judges the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals.

Spec: R3

Evidence (run): This ticket exists for that reason: the candidate went through four review rounds (DYD-1281) and still failed the user's read (DYD-1295 519126ab).

Layer: A contract can pass on a wrong rule, and nothing below a reader of the spec would notice.

Reason: Work can meet its contract and still drift from what the Project is for.

**ticket-captain** (unwritten; ReviewAll; read by TicketCaptain):

> Judge the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals, not only the contract.

## Severities

`claim:CaptainReviewing.Severities`: A reviewer gives every finding one severity: blocker, should or nit.

Spec: R11

Evidence (run): Linear, team Dydo, Severity labels read 2026-11-20, describe them so: "the intended case is wrong", "a real bug callers will hit, inside the intent", "everything else".

Layer: Linear holds the three labels; sorting a finding into one is the reviewer's judgement.

Reason: The severity is what the captain routes on, so a finding without one cannot move.

**ticket-captain** (unwritten; ReviewAll; read by TicketCaptain):

> Give every finding one severity: a blocker, the intended case is wrong; a should, a real bug callers will hit, inside the intent; a nit, everything else.

## OnePass

`claim:CaptainReviewing.OnePass`: A reviewer reports every visible blocker in one pass.

Spec: R8

Evidence (run): Linear, DYD-1327 read 2026-11-20: review r1 returned 7 blockers, 12 shoulds and 9 nits together, closed by one Fix.

Layer: Nothing stops a reviewer at the first blocker it finds.

Reason: Blockers handed over one a round cost a fix and a round each.

**ticket-captain** (unwritten; ReviewAll; read by TicketCaptain):

> Report every visible blocker in one pass.

## Seat

`claim:CaptainReviewing.Seat`: A seat returns its findings, each with its severity, to the captain holding the round.

Spec: S8, S1

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: Each seat sees only its own findings, so none of them can write the round's verdict.

Reason: Several seats posting each a verdict would leave the round with none.

**ticket-captain** (unwritten; ReviewSeat; read by TicketCaptain, on claim:CaptainReviewing.SeatLeaf):

> Your brief names your one kind of review, and you are a leaf that stays one: read the part below headed by your kind, leave "If you hold the review" unread, and return your findings, each with its severity, to the captain holding the round.

## SeatLeaf

`claim:CaptainReviewing.SeatLeaf`: A seat does one kind of review as a known leaf that stays one, and reads only its kind's part of the review resource.

Spec: R34, K30

Evidence (run): His words (DYD-1372 aa5b445e): "Yes, they're leafs. And we know that they are. And they will stay that way." and, of a seat that drives the app, "Do they really have to know everything about what a "ticket-captain" does? They're not meant do delegate into further sub agents for sure." T3 tells every child the same by default: "do the work yourself. Start no subagents unless your task says to."

Layer: The spine shows every holder how a ticket is split, and only its dispatcher knows that this one was cut to stay whole.

Reason: A reviewer sent to do one thing spends its attention on that thing, and none on orchestration it will never use.

**ticket-captain** (unwritten; ReviewSeat; read by TicketCaptain):

> Your brief names your one kind of review, and you are a leaf that stays one: read the part below headed by your kind, leave "If you hold the review" unread, and return your findings, each with its severity, to the captain holding the round.

## Divide

`claim:CaptainReviewing.Divide`: A seat whose slice is too big for one agent divides it itself, keeping each part coherent, and returns it for a re-cut only when it cannot.

Spec: H2, K30

Evidence (run): His words (DYD-1372 aa5b445e): "let's take the read review then it makes sense to keep the escape hatch, because if it's a too big of a slice it can be further divided (while keeping the coherence)." and "So keeping the escape hatch is probably right."

Layer: A reviewer learns how large its slice really is only once it has opened it, long after the cut was made.

Reason: A reader pushed past what it can hold skims, and a skimmed slice passes with its faults unread.

**ticket-captain** (unwritten; ReviewSeat; read by TicketCaptain):

> A leaf divides only what is too big for one agent, as a large read can be. Divide such a slice by Too big to hold (`too-big.md`), your findings so far standing in for its commit and each part a slice that reads whole on its own, and return the parts' findings with yours.

## Matrix

`claim:CaptainReviewing.Matrix`: A reviewer works only the row of the level its brief names.

Spec: R34, R5

Evidence (run): The user's table, verbatim from spec R34 with its legend and two footer rules, as `skills/roles/crew/reviewer/resources/toolset.md` holds it today. Three of its cells name what this repository cannot run yet; claim CaptainReviewing.Built names them and says what a reviewer does with such a cell.

Layer: Every tool in the row is there for the taking at every level; nothing meters review.

Reason: Review is expensive, so each level spends only what it can find something with.

**ticket-captain** (unwritten; ReviewRow; read by TicketCaptain):

> Legend: **due** every time; **if** when warranted; **–** not at this level.
>
> | | Read<br>local | Read<br>vs spec | Read<br>whole / gestalt | Run tests | Hypothesis<br>custom tests | Drive<br>the app | Formal<br>statechart, … |
> | --- | --- | --- | --- | --- | --- | --- | --- |
> | Leaf evidence check<br>a lane or hop, optional seat | due | if<br>drift only | – | due<br>its receipts, tagged | – | – | – |
> | Ticket review<br>a parent integrating its parts | due | due | if<br>its own area | due<br>tagged + feature; mutation on changed | if | if<br>user-facing | – |
> | Feature / Milestone<br>a subsystem is done | if | due | due | due<br>feature + suite | due<br>at the seams | due<br>its journeys | if<br>it has a state machine |
> | Inquisition<br>before landing, per slice | due<br>every slice | due | due | due<br>suite + --heavy + mutation | due<br>every hypothesis | due | due |
>
> > Every row: never re-run what the author ran the same way; a test receipt for the same state counts (spec R5). The security baseline is always on.
> >
> > How it's used: the officer's brief names the level; the reviewer works its row.

## Built

`claim:CaptainReviewing.Built`: A cell is due only as far as the project has what it names; what is missing is reported to the officer as a finding, never built.

Spec: R34, R7

Evidence (run): Read 2026-11-20: `dydo/guides/testing-strategy.md` says this repository's test facade "has no `--feature`, `--plan`, `--heavy` or Ripple support" and that mutation is "unavailable on every stack"; no file under `DynaDocs.Tests/coverage` takes either flag, and no command of the CLI is DYTEST. So the cells "tagged + feature", "mutation on changed" and "suite + --heavy + mutation" cannot run here as written. The table is the user's, approved verbatim, and he kept it whole with this line below it (DYD-1372 aa5b445e, "2. Okay", on the walkthrough brief's recommendation).

Layer: The table's bare "due" cannot tell whether a project has the tool, the selector or the result a cell calls for.

Reason: Without it a reviewer stalls on a cell nothing can run, or builds the tooling and turns its review into a second project.

**ticket-captain** (unwritten; ReviewRules; read by TicketCaptain):

> A cell is due only as far as the project has what it names: Drive and Formal where the tool is built and the subject is settled, any other cell where its tools, selectors and results exist. Report what is missing to your officer as a finding, instead of building it.

## Resolution

`claim:CaptainReviewing.Resolution`: A conflict's resolution is judged by its hunks, and its PASS names the new head.

Spec: R19

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a head no PASS names, and a resolution always makes a new head.

Reason: Everything else on the branch already passed, so judging it again spends a round on nothing.

**ticket-captain** (unwritten; ReviewRules; read by TicketCaptain):

> Asked to judge a conflict's resolution: judge its hunks, the only new work on the branch, and name the new head in your PASS.

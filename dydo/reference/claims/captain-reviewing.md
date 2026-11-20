---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReviewing

## One

`claim:CaptainReviewing.One`: A Review ticket is one candidate judged at the level its brief names, and its verdict is its holder's.

Spec: R34, S8

Evidence (code): `skills/roles/crew/reviewer/resources/toolset.md`

Layer: The brief names the candidate and the level; that the verdict has one owner is said only here.

Reason: A verdict nobody owns is a list of remarks that nobody acts on.

**ticket-captain** (ReviewLead; read by TicketCaptain):

> Your ticket is a review: one candidate, judged at the level your brief names, and the verdict is yours.

## NotMine

`claim:CaptainReviewing.NotMine`: No agent judges work it implemented or fixed.

Spec: R33

Evidence (tool): t3_thread_send: "A send to a finished child thread starts a fresh task", so a writer's thread can be handed its own work to review.

Layer: T3 will hand a review to any thread, the writer's included.

Reason: Its own context hides from a writer exactly what a fresh reader would catch.

**ticket-captain** (ReviewLead; read by TicketCaptain):

> Never judge work you implemented or fixed.

## Matrix

`claim:CaptainReviewing.Matrix`: A reviewer works only the row of the level its brief names.

Spec: R34, R5

Evidence (run): The user's table, verbatim from spec R34 with its legend and two footer rules, as `skills/roles/crew/reviewer/resources/toolset.md` holds it today. Three of its cells name what this repository cannot run yet; claim CaptainReviewing.Built names them and says what a reviewer does with such a cell.

Layer: Every tool in the row is there for the taking at every level; nothing meters review.

Reason: Review is expensive, so each level spends only what it can find something with.

**ticket-captain** (ReviewRow; read by TicketCaptain):

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

Evidence (run): Read 2026-11-20: `dydo/guides/testing-strategy.md` says this repository's test facade "has no `--feature`, `--plan`, `--heavy` or Ripple support" and that mutation is "unavailable on every stack"; no file under `DynaDocs.Tests/coverage` takes either flag, and no command of the CLI is DYTEST. So the cells "tagged + feature", "mutation on changed" and "suite + --heavy + mutation" cannot run here as written. The table is the user's, approved verbatim, so the cells stay: whether to cut them or to keep them under this line is his call.

Layer: The table's bare "due" cannot tell whether a project has the tool, the selector or the result a cell calls for.

Reason: Without it a reviewer stalls on a cell nothing can run, or builds the tooling and turns its review into a second project.

**ticket-captain** (ReviewRules; read by TicketCaptain):

> A cell is due only as far as the project has what it names: Drive and Formal where the tool is built and the subject is settled, any other cell where its tools, selectors and results exist. Report what is missing to your officer as a finding, instead of building it. (open: his call)

## Anchor

`claim:CaptainReviewing.Anchor`: A review judges the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals.

Spec: R3

Evidence (run): This ticket exists for that reason: the candidate went through four review rounds (DYD-1281) and still failed the user's read (DYD-1295 519126ab).

Layer: A contract can pass on a wrong rule, and nothing below a reader of the spec would notice.

Reason: Work can meet its contract and still drift from what the Project is for.

**ticket-captain** (ReviewRules; read by TicketCaptain):

> Judge the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals, not only the contract.

## Severities

`claim:CaptainReviewing.Severities`: A reviewer gives every finding one severity: blocker, should or nit.

Spec: R11

Evidence (run): Linear, team Dydo, Severity labels read 2026-11-20, describe them so: "the intended case is wrong", "a real bug callers will hit, inside the intent", "everything else".

Layer: Linear holds the three labels; sorting a finding into one is the reviewer's judgement.

Reason: The severity is what the captain routes on, so a finding without one cannot move.

**ticket-captain** (ReviewRules; read by TicketCaptain):

> Give every finding one severity: a blocker, the intended case is wrong; a should, a real bug callers will hit, inside the intent; a nit, everything else.

## OnePass

`claim:CaptainReviewing.OnePass`: A reviewer reports every visible blocker in one pass.

Spec: R8

Evidence (run): Linear, DYD-1327 read 2026-11-20: review r1 returned 7 blockers, 12 shoulds and 9 nits together, closed by one Fix.

Layer: Nothing stops a reviewer at the first blocker it finds.

Reason: Blockers handed over one a round cost a fix and a round each.

**ticket-captain** (ReviewRules; read by TicketCaptain):

> Report every visible blocker in one pass.

## Lanes

`claim:CaptainReviewing.Lanes`: A reviewer may run lanes through native helpers and puts their findings together itself.

Spec: K30, S1

Evidence (tool): Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.

Layer: AGENTS.md allows the lanes; that their findings become one verdict is the holder's work.

Reason: Reading and driving need different attention, and one verdict still has to answer for both.

**ticket-captain** (ReviewRules; read by TicketCaptain):

> Run lanes through native helpers when your row calls for both reading and driving, and put their findings together yourself.

## Alone

`claim:CaptainReviewing.Alone`: A lone reviewer holds its round whole and records it itself.

Spec: S8, R17

Evidence (run): Linear, DYD-1281's sub-tickets read 2026-11-20: each round is one Review ticket.

Layer: A lone reviewer's officer sees only one line back; the record has to be on the ticket already.

Reason: The verdict is written by the one who formed it, with nothing lost in a relay.

**ticket-captain** (ReviewHolding; read by TicketCaptain):

> Holding the round alone: record it yourself, by A round's record (`round.md`).

## Seat

`claim:CaptainReviewing.Seat`: One seat among several returns its findings to the captain holding the round.

Spec: S8, S1

Evidence (tool): delegate_task: a child's final result returns to the thread that started it.

Layer: Each seat sees only its own findings, so none of them can write the round's verdict.

Reason: Several seats posting each a verdict would leave the round with none.

**ticket-captain** (ReviewHolding; read by TicketCaptain):

> Sitting as one seat among several: return your findings, each with its severity, to the captain holding the round.

## Resolution

`claim:CaptainReviewing.Resolution`: A conflict's resolution is judged by its hunks, and its PASS names the new head.

Spec: R19

Evidence (code): `Services/Merge/PullRequestMerge.cs`

Layer: `dydo merge` refuses a head no PASS names, and a resolution always makes a new head.

Reason: Everything else on the branch already passed, so judging it again spends a round on nothing.

**ticket-captain** (ReviewHolding; read by TicketCaptain):

> Asked to judge a conflict's resolution: judge its hunks, the only new work on the branch, and name the new head in your PASS.

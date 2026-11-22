# Who sees what

Each agent gets only the skills and context its position needs, and the agent that dispatches it names those skills in the brief (`dydo/core.md`, "How the process must feel"). This page says what that is for each position in [the captain model](Captain.tla). A skill file is checked against the model with the three checks at the end. [The simple chart](chart.mmd) is for reading; [view-map.mjs](view-map.mjs) shows where each model state sits on it.

"Text for a state" means the words that tell a captain what to do on each action out of that state, and on each action that stays inside it.

## What every captain gets

Every agent dispatched onto a ticket is its captain, whatever the ticket (core, "A ticket"; K30). Each one gets:

- `AGENTS.md`. The repository gives it to every dispatched agent.
- `/ticket-captain`, named in its brief. It is the one captain skill (K30), and it holds or names the text for every state in the model. Which file holds which state's text is the skill's to say; this page fixes who gets it. It names the text for each later state at the action that leads there (`AGENTS.md`: "each skill names the next").
- Its brief (K14, S15). The brief carries:
  - the skills it names;
  - the ticket and its contract;
  - how the ticket fits the ticket above it and the spec;
  - what the ticket's result is: a change, a verdict, findings as one seat of a review, an answer, or a cut;
  - for a Question, its stage: research, council or his-call (H23);
  - whether this captain merges its change, and into which branch;
  - the branch, worktree and base, and the paths it owns;
  - the gates, and how the work is proved;
  - the model and effort;
  - who its officer is;
  - whether the ticket was pulled as filler, which the captain then checks (H11).
- Every other skill the work needs, named in the brief with a slash.

Every captain also gets the text for passing a need up: a call it may not decide goes to its officer, and work that does not wait on the answer goes on (H16, S10). A captain that dispatches another agent also gets the text for writing a brief, whatever its position: a splitter its parts, a merger its reviewer and its Fix, a research captain its council or his-call child. The model roster is part of that text.

## Positions

| Position | Its states in the model | Text it gets | Context its brief adds | What it never gets |
|---|---|---|---|---|
| A captain doing a leaf | `Leaf`, and `Asking` on the Question its officer opened for its need. Not the action out of `Leaf` into `InReview`: that one is the merger's. | The text for `Leaf` and `Asking`: proving its change with receipts (R5), passing a need up and waiting on the answer, and the part for its ticket's result: writing a change, reaching and writing a verdict, answering a Question, or a cut. | Nothing more. | The text for `Cutting` and `Waiting`, unless the action "the work proves too big" takes it there. `/ticket-captain` names that text at that action, and from then on it is a captain that splits. |
| A captain that splits | `Cutting`, `Waiting`. Not the action out of `Waiting` into `InReview`: that one is the merger's. | The text for those two states: the cut of the parts in sight, and of one part that brings the next into sight when the route is in fog; the brief for each sub-ticket; the wait; a ready part dispatched; the optional evidence check beside a part that is back, and the part's fix or cheap mend after it (R6, S4, S17); a part's need, which it opens as a Question as the receiving officer, and which that part waits on (H16, S10); a stopped hop set back to Todo, then resumed or given a fresh agent (S24); checking its contract against the spec once a part that brings the route into sight is back (H10); the wiring; finishing when the contract is met (H10); and the ways back to its officer. | Nothing more. | The part for doing the work. It only orchestrates. |
| A captain that merges its change | `InReview`, `Fixing`, `Mending`, `Repairing`, `Merging`, `AwaitingTurn`, `Held`, `AwaitingClick`, `Landed`, `Rerunning`, and the two actions into `InReview`: out of `Leaf` and out of `Waiting`. | The text for those states and those two actions: dispatching a fresh reviewer onto each round's Review hop, and acting only on the verdict and the PR's PASS line its holder writes (R17, R33); routing the verdict's blockers by kind, and opening a Question for a blocker that needs Balazs (R9, S10); the Fix ticket, Done once its blockers are and proof ran on the head (S9); the checkpoint after a third FAIL and its line (R10, S10); one more round after a PASS; a cheap should mended before the merge (S17); the merge's evidence, gates and CI, and a repair only for a CONFLICT or a real red on record (R21, R23, R26); the turn into master, the re-check against current master just before landing, the re-sync and the hold on the third (R29); his click on a one-way PR (R27, R31); the checks after the landing (R20); the rerun, the revert and the repair, proved, then a round scoped to its hunks (R19, R21, R22); a stopped hop (S24). `/pr` for the PR body. | That it merges, and into which branch. | Nothing is withheld. A captain whose brief does not say it merges never gets this text. |
| A reviewer: the captain of a ticket whose result is a verdict | `Leaf` and `Asking`, and `Cutting` and `Waiting` for its seats. Never a state of the merger's row. | The part for reaching a verdict, with the severities as the core page defines them, the blockers' kinds (R9) and the cut into seats; writing the verdict on the Review sub-ticket and, for a PASS, the PR's one-line PASS together (R17), once its seats are back (S8). A blocker that needs Balazs goes in its verdict; the merger opens its Question. | The candidate to judge, its head and its base; the model that wrote it; the ticket judged and its contract; the governing spec, or that there is none; the review level and its lenses; what the author already proved. | The text for the merger's states. Its verdict is not reviewed again. |
| A seat: the captain of one part of a review, whose result is its findings | `Leaf`, and `Asking` on a need it passed up. Never `Cutting`, `Waiting`, or a state of the merger's row. | The part for finding problems: the kinds of looking its brief names. A problem it may not judge goes in its findings, or up to its officer. | The same as a reviewer's, and the one kind of review it does. | The text for cutting and for the merger's states. A seat is a leaf and never delegates (R34). |
| A research Question's captain: a Question at stage `research`, whose result is an answer | `Leaf`, `Asking` at any stage of its child, and a splitter's states for research crew. Never a state of the merger's row. | `/ticket-captain` with research crew (H23): research first, then the answer it decides, or a council child, or a his-call child, and the answer from it: see "A Question" below. | The question, and the ticket that waits on it. | The text for the merger's states. |
| A council's chair: a Question at stage `council` | `Cutting` and `Waiting` only: a council is two judges and its chair (L18). | The council-chair resource (L23, H23): the neutral case file, the two judges' sealed positions, the one exchange, and the exits: decided, or prepared for him. | The call, its options unranked, the constraints and the primary facts (L18); what its case file says of the call: two-way, DR-grade or one-way, and whether it is overnight and seriously blocking (L21). | Doing the work alone, opening a child, and the merger's states. |
| A his-call Question's captain: the envoy on a Question at stage `his-call` | A leaf's states, with him, or a splitter's when it splits. Never a state of the merger's row. | `co-thinker` (H23), and the envoy's brief of seven slots (H19). The call is his: the ticket closes only on his answer (core, "Who decides what"). | The his-call ticket, and why it needs him. | Opening a council or his-call child, and the merger's states. |
| A Wayfinding captain: the captain of a ticket whose result is a cut, the next tickets toward the spec | `Leaf`, `Asking`, `Cutting`, `Waiting`. Never a state of the merger's row. | `/wayfinder`, named in its brief: it is the part for a cut. The one optional scout pass on the spec it refines, which returns Questions and never PASS or FAIL (R4). | The ticket or Project whose route is not in sight, and its spec. | Opening a council or his-call child, a review of its cut, and the merger's states. |
| A Fix's holder: a captain on a Fix ticket, whose result is a change it does not merge | A leaf's states, or a splitter's when the Fix is big (S9: a sub-ticket for a big blocker, and Fix rNb for parallel writers), on the Fix ticket's own run of the model. | The same as a captain doing a leaf, or one that splits. | The blockers to fix, all of them, and the Review they come from. | The text for the merger's states. |
| A native helper | None. It holds no ticket, so it is not in the model. | None. It does not read `AGENTS.md` and loads no skill unless its holder's prompt names one. | Only the prompt its holder writes. It shares its holder's checkout, and the holder commits. | Everything else. |

A Fix is a ticket of its own. By default its holder is the thread that wrote the change, and a fresh agent when that thread is full or the blocker says the approach was wrong (S9). The holder does the Fix with a leaf's actions, or, when the Fix is big, splits it with a splitter's (S9). When the captain that waits in `Fixing` is that same thread, it holds two tickets in turn: it does the Fix on the Fix ticket, then goes on from `Fixing` on its own ticket. A repair at the merge is a Fix of the same kind, and the captain waits on it in `Repairing`. A cheap should mended before the merge is fixed in place by the same default holder, and the captain waits on it in `Mending`.

A review's captain may cut the review into seats, each with one kind of review (R34). Each seat is a sub-ticket of the Review ticket (S1), and its result is its findings. When the seats are back, the review's captain gives the one verdict, naming each seat (S8).

## A Question

A call a captain may not decide becomes a Question ticket for a fresh agent (core, "Who decides what"). The whole case:

- **Who opens it.** The captain with the call passes the need up to its officer, and goes on with the work that does not wait on the answer (S10). The officer that receives it opens the Question, a sub-ticket under its own ticket (or a ticket under the coordinator), at stage `research` (L18: every Question starts with research; H23: whoever opens it sets the stage), dispatches a fresh captain onto it, and the work that waits on it is blocked by it (H16). When only that work is left, a captain doing a leaf then waits on the answer in `Asking`; a captain that splits waits on it in `Waiting`, where it waits on its parts too, since after a cut it never goes back to `Asking` (H10: "if what is left all waits on Questions or his calls, it waits on them"). A captain opens a Question only as the receiving officer: a splitter for a part's need, which that part then waits on and comes back only with its answer (S10); a merger for a reviewer's blocker that needs Balazs, whose Fix then waits on it, or, when the same verdict holds a fundamental blocker, the re-plan it hands the ticket back for (R9, S10).
- **What its captain does.** It researches first, itself or with research crew (H23). When the sources settle the call, or leave a call that is its own, it decides: that is its answer. When the research does not resolve the call (H23):
  - a hard call not worth his time goes to a council: it opens a child Question at stage `council`, Mode AFK, chaired by a fresh ticket captain (L18, L20, L23);
  - a call worth his time goes toward his HITL queue: it opens a child Question at stage `his-call`, Mode HITL, with his question made ready on it, every option, each option's trade-off and a recommendation (core, "Who decides what"; H16). The coordinator dispatches an envoy onto that same ticket when its turn comes (H16, H18).

  It waits in `Asking` on the child's answer. A council that decided gives the answer; a council that prepared the call for him sends it on to a his-call child (L18, L20). Only a research Question's captain opens a `council` or `his-call` child. A his-call child closes only on his answer (core, "Who decides what").
- **A council.** Its chair cuts the case to two judges from different families, who take sealed positions; with one family out it runs on what is left (L22; register IQ20); then one exchange follows; then the exits: decided when the judges agree on a two-way call, or overnight on a two-way, seriously blocking DR-grade call as a proposed DR (L21); prepared for him otherwise, and always for a one-way call (L18).
- **A Question that comes back without its answer.** Its officer, the one that opened it, passes it up as a problem outside its ticket (core, "How the process must feel"), or dispatches a fresh agent onto it. The captain that waits on it goes on waiting: `Asking` has one way out, the answer on the Question.

What each row rests on:

- A captain doing a leaf, and one that splits: core, "A ticket": a captain "either does the work itself, with native helpers, or splits the ticket and only orchestrates", and finds the route "one manageable part at a time". K30: a leaf needs its kind's part, not the orchestration, and the escape hatch stays. H2 for the ways back to the officer; H10 for finding the way and for finishing; R5 for the proof; R6, S4 and S17 for a part's evidence check and mend; S24 for a stopped hop.
- The model roster: core, "Who decides what": Balazs decides the model roster. R13: it is built into the text of every officer that dispatches, so it is skill text, not a slot of the brief, and every captain that dispatches gets it.
- A captain that merges its change: R23, a merge needs a PASS on its head; core, "the verdict is another agent's". R8, R9, R10, S10 and S17 for the rounds, the routing, the checkpoint and the mend; R19, R21 and R22 for the repair, the rerun and the revert; R20, R23, R24, R26, R27, R29 and R31 for the merge's evidence, its landing and a merge into master. `/pr` is the repository's skill for a PR body (`skills/engineering/pr/SKILL.md`: "Use when writing a PR body").
- A reviewer: core, "Review severities": "A verdict is not reviewed again." R3, R5, R6 and R34 for what its brief names; R9 for the blockers' kinds. Its base, because `dydo merge` refuses a PASS whose base the target does not contain (`Services/Merge/PullRequestMerge.cs`).
- A seat: R34: a seat from another family "get[s] their own tickets. Yes, they're leafs." "They're not meant do delegate into further sub agents for sure." S1, S8. R5: "Any agent reports a problem outside its scope to its officer."
- A Question's captains: core, "Who decides what": a Question captain decides its answer, and Balazs gets "a question with every option, each option's trade-off, and its recommendation". H16 for who opens it and the wait; H23 for the stages and the skill each loads; L18, L20, L21 and L23 for the council.
- A Wayfinding captain: core, "Who decides what": a Wayfinding captain decides the cut. H23: its skill is `wayfinder`. R4: a spec gets one scout pass, and no review loop.
- A Fix's holder: S9, one Fix ticket lists every blocker, with a sub-ticket only for a big one, and Fix rNb for parallel writers. K14: the findings go in a Fix's brief only.
- A native helper: K34, a native helper never reads `AGENTS.md`. H3, it shares its holder's checkout and the holder commits.

## How to check a skill file against the model

A skill line is checked against the comprehensive model (`CaptainBase.tla` and `Captain.tla`, held to `register.md`), never against the simple chart alone: a line that tells a step the chart folds inside a state, such as the checkpoint after a third FAIL, is right when the model has that step. A position can take an action when the action leaves a state in its row, or stays inside one, and its guard can hold for that position. Only a captain whose brief says it merges can take the two actions into `InReview`.

1. Every action a position can take is told by text that position gets: the brief itself, a skill the brief names, or a text that such a skill names for that state.
2. No text a position gets tells an action that position cannot take, or a step the model does not have. A leaf's text says nothing of cutting past the one action that leads there, and a reviewer's says nothing of a review of its own work, a Fix or a merge.
3. Every skill that the text tells a captain to give a sub-ticket's holder is named in that sub-ticket's brief, with a slash.

## What the model checks of this page

The model is the frame for now (DYD-1504; see the README): its states are the chart's, and the states named in the rows above that the chart folds inside its own (`Mending`, `Repairing`, `AwaitingTurn`, `Held`, `AwaitingClick`, `Landed`, `Rerunning`) come with part 4. The properties in `verification/statechart/captain/` check of this page what the frame holds:

- `FR26_EachDecidesItsJob`: the verdict on a change is the reviewer's, never its captain's or the Fix's holder's; a reviewer writes its own verdict, and a Question's captain settles its own call.
- `FR28_AVerdictIsNotReviewedAgain`: a reviewer is dispatched only onto a change, never onto a verdict, a seat's findings, an answer or a cut.

The rest of this page's states column and waits are register rows owed by the parts: a seat never delegates (SP2), only a merger reaches review, fix and merge (RM1), a cut gets no review (IQ24), a captain that has cut never works itself (SP1), a council is never one agent (IQ22), a child Question only after research (IQ16), the receiving officer opens a Question (IQ9, SP13), a part waits on its answer (SP14), and a review's verdict comes once its seats are back (SP20).

The model cannot see which skill an agent loaded or what its brief said. Those are checked by reading, with the three checks above.

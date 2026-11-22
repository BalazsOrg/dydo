# Who sees what

Each agent gets only the skills and context its position needs, and the agent that dispatches it names those skills in the brief (`dydo/core.md`, "How the process must feel"). This page says what that is for each position in [the captain model](Captain.tla). A skill file is checked against the model with the three checks at the end. [The simple chart](chart.mmd) is for reading; [view-map.mjs](view-map.mjs) shows where each model state sits on it.

"Text for a state" means the words that tell a captain what to do on each action out of that state.

## What every captain gets

Every agent dispatched onto a ticket is its captain, whatever the ticket (core, "A ticket"; K30). Each one gets:

- `AGENTS.md`. The repository gives it to every dispatched agent.
- `/ticket-captain`, named in its brief. It is the one captain skill (K30), and it holds or names the text for every state in the model. Which file holds which state's text is the skill's to say; this page fixes who gets it. It names the text for each later state at the action that leads there (`AGENTS.md`: "each skill names the next").
- Its brief (K14, S15). The brief carries:
  - the skills it names;
  - the ticket and its contract;
  - how the ticket fits the ticket above it and the spec;
  - what the ticket's result is: a change, a verdict, findings as one seat of a review, or an answer;
  - whether this captain merges its change, and into which branch;
  - the branch, worktree and base, and the paths it owns;
  - the gates, and how the work is proved;
  - the model and effort;
  - who its officer is;
  - whether the ticket was pulled as filler, which the captain then checks (H11).
- Every other skill the work needs, named in the brief with a slash.

A captain that dispatches another agent also gets the text for writing a brief, whatever its position: a leaf sends out a Question's captain, a splitter its parts, a merger its reviewer. The model roster is part of that text.

## Positions

| Position | Its states in the model | Text it gets | Context its brief adds | What it never gets |
|---|---|---|---|---|
| A captain doing a leaf | `Leaf`, `Asking` at stage research. Not the action out of `Leaf` into `InReview`: that one is the merger's. | The text for `Leaf` and `Asking`, and the part for its ticket's result: writing a change, reaching a verdict, or answering a Question. | Nothing more. | The text for `Wayfinding`, `Cutting` and `Waiting`, unless the action "the work proves too big" takes it there. `/ticket-captain` names that text at that action, and from then on it is a captain that splits. |
| A captain that splits | `Wayfinding`, `Cutting`, `Waiting`. Not the action out of `Waiting` into `InReview`: that one is the merger's. | The text for those three states: finding the way when the route is not in sight, the cut of the parts in sight, a Question as one more part, the brief for each sub-ticket, the wait, what to do with a part that is handed back or that reports a problem, the wiring, and the ways back to its officer. | Nothing more. | The part for doing the work. It only orchestrates. |
| A captain that merges its change | `InReview`, `Clear`, `Checkpoint`, `Fixing`, `Mending`, `Repairing`, `Merging`, `AwaitingTurn`, `Rerunning`, `AwaitingClick`, and the two actions into `InReview`: out of `Leaf` and out of `Waiting`. | The text for those states and those two actions: dispatching a reviewer, the Fix ticket, the checkpoint after a third FAIL, one more round after a PASS, a cheap should mended before the merge, the merge, the rerun of a red gate, the repair, and a merge into master on its turn or on his click. `/pr` for the PR body. | That it merges, and into which branch. | Nothing is withheld. A captain whose brief does not say it merges never gets this text. |
| A reviewer: the captain of a ticket whose result is a verdict | The states every captain has, `Leaf` and `Asking` at stage research, and `Cutting` and `Waiting` for its seats. Never `Wayfinding`, and never a state of the merger's row. | The part for reaching a verdict, with the severities as the core page defines them, and the cut into seats. | The candidate to judge, its head and its base; the model that wrote it; the ticket judged and its contract; the governing spec, or that there is none; the review level and its lenses; what the author already proved. | The text for the merger's states. Its verdict is not reviewed again. |
| A seat: the captain of one part of a review, whose result is its findings | The states every captain has, and `Leaf`. Never `Asking`, `Wayfinding`, `Cutting`, `Waiting`, or a state of the merger's row. | The part for finding problems: the kinds of looking its brief names. | The same as a reviewer's, and the one kind of review it does. | The text for cutting, for a Question, and for the merger's states. A seat is a leaf and never delegates (R34). |
| A Question's captain: the captain of a ticket whose result is an answer | The states every captain has, `Leaf`, `Asking` at any stage, and a splitter's. Never a state of the merger's row. | The part for answering a Question: research first, then the answer it decides, or a council child, or a his-call child: see "A Question" below. | The question, and the ticket that waits on it. | The text for the merger's states. |
| A Wayfinding captain: the captain of a ticket whose result is a cut, the next tickets toward the spec | The same as a Question's captain. | `/wayfinder`, named in its brief: it is the part for a cut. | The ticket or Project whose route is not in sight, and its spec. | The text for the merger's states. |
| A Fix's holder: a captain on a Fix ticket, whose result is a change it does not merge | A leaf's states, or a splitter's when the Fix is big (S9: a sub-ticket for a big blocker, and Fix rNb for parallel writers), on the Fix ticket's own run of the model. | The same as a captain doing a leaf, or one that splits. | The blockers to fix, all of them, and the Review they come from. | The text for the merger's states. |
| A native helper | None. It holds no ticket, so it is not in the model. | None. It does not read `AGENTS.md` and loads no skill unless its holder's prompt names one. | Only the prompt its holder writes. It shares its holder's checkout, and the holder commits. | Everything else. |

A Fix is a ticket of its own. By default its holder is the thread that wrote the change, and a fresh agent when that thread is full or the blocker says the approach was wrong (S9). When the captain that waits in `Fixing` is that same thread, it holds two tickets in turn: it does the Fix with a leaf's actions, and then goes on from `Fixing` on its own ticket. A repair after a merge is a Fix of the same kind, and the captain waits on it in `Repairing`.

A review's captain may cut the review into seats, each with one kind of review (R34). Each seat is a sub-ticket of the Review ticket (S1), and its result is its findings. When the seats are back, the review's captain gives the one verdict, naming each seat (S8).

## A Question

A call a captain may not decide becomes a Question ticket for a fresh agent (core, "Who decides what"). The whole case:

- **Who opens it.** The captain that has the call opens it as a sub-ticket of its own ticket, at stage `research` (L18: every Question starts with research; H23: whoever opens it sets the stage), and dispatches a fresh agent onto it. A captain doing a leaf then waits in `Asking`. A captain that splits cuts the Question as one more part, in `Cutting`, and waits on it in `Waiting` beside its other parts. H16 has the officer that receives a need open it; the captain waits on it either way.
- **What its captain does.** It researches first. When the sources settle the call, or leave a call that is its own, it decides: that is its answer. When the research does not resolve the call (H23):
  - a hard two-way call goes to a council: it opens a child Question at stage `council`, Mode AFK, chaired by a fresh ticket captain (L18, L23);
  - a call worth his time goes toward his HITL queue: it opens a child Question at stage `his-call`, Mode HITL, with his question made ready on it, every option, each option's trade-off and a recommendation (core, "Who decides what"; H16). The coordinator dispatches an envoy onto that same ticket when its turn comes (H16, H18).

  Either way it waits in `Asking` on the child's answer, then writes its own answer from it. Only a Question's captain opens a `council` or `his-call` Question.
- **A Question that comes back without its answer.** The captain that asked passes it up to its own officer as a problem outside its ticket (core, "How the process must feel"), or dispatches a fresh agent onto it, and goes on waiting. Neither changes a state, so `Asking` has one action out: the wait ends only when the answer is on the Question.
- **A Wayfinding captain** is on the row of states a Question's captain has. The model names four results: a change, a verdict, findings and an answer. A cut is not one of them. Nothing is merged, so in the model a cut's run is an answer's run.

What each row rests on:

- A captain doing a leaf, and one that splits: core, "A ticket": a captain "either does the work itself, with native helpers, or splits the ticket and only orchestrates", and finds the route "one manageable part at a time". K30: a leaf needs its kind's part, not the orchestration, and the escape hatch stays. H2 for the ways back to the officer; H10 for finding the way.
- The model roster: core, "Who decides what": Balazs decides the model roster. R13: it is built into the text of every officer that dispatches, so it is skill text, not a slot of the brief, and every captain that dispatches gets it.
- A captain that merges its change: R23, a merge needs a PASS on its head; core, "the verdict is another agent's". R8, R10, S10 and S17 for the rounds, the checkpoint and the mend; R19, R21 and R22 for the repair and the rerun; R26, R27, R29 and R31 for a merge into master. `/pr` is the repository's skill for a PR body (`skills/engineering/pr/SKILL.md`: "Use when writing a PR body").
- A reviewer: core, "Review severities": "A verdict is not reviewed again." R3, R5, R6 and R34 for what its brief names. Its base, because `dydo merge` refuses a PASS whose base the target does not contain (`Services/Merge/PullRequestMerge.cs`).
- A seat: R34: a seat from another family "get[s] their own tickets. Yes, they're leafs." "They're not meant do delegate into further sub agents for sure." S1, S8.
- A Question's captain: core, "Who decides what": a Question captain decides its answer, and Balazs gets "a question with every option, each option's trade-off, and its recommendation". H16: research first, then the answer, a council, or his HITL queue. H23 and L18 for the stages, L23 for the council.
- A Wayfinding captain: core, "Who decides what": a Wayfinding captain decides the cut. H23: its skill is `wayfinder`.
- A Fix's holder: S9, one Fix ticket lists every blocker, with a sub-ticket only for a big one. K14: the findings go in a Fix's brief only.
- A native helper: K34, a native helper never reads `AGENTS.md`. H3, it shares its holder's checkout and the holder commits.

## How to check a skill file against the model

A skill line is checked against the comprehensive model, `Captain.tla`, never against the simple chart alone: a line that tells a step the chart folds inside a state, such as the checkpoint after a third FAIL, is right when the model has that step. A position can take an action when the action leaves a state in its row and its guard can hold for that position. Only a captain whose brief says it merges can take the two actions into `InReview`.

1. Every action a position can take is told by text that position gets: the brief itself, a skill the brief names, or a text that such a skill names for that state.
2. No text a position gets tells an action that position cannot take, or a step the model does not have. A leaf's text says nothing of cutting past the one action that leads there, and a reviewer's says nothing of a review of its own work, a Fix or a merge.
3. Every skill that the text tells a captain to give a sub-ticket's holder is named in that sub-ticket's brief, with a slash.

## What the model checks of this page

The properties in `verification/statechart/captain/` check the states column against the model:

- `R34_VerdictNeverReviewed`: a reviewer and a seat are never in a state of the merger's row.
- `R34_SeatNeverDelegates`: a seat is never in `Asking`, `Wayfinding`, `Cutting` or `Waiting`.
- `Sees_OnlyAMergerSeesReviewFixMerge`: only a captain that merges a change is ever in a state of the merger's row, so a Question's captain is not.
- `Core_SplitOnlyOrchestrates`: a captain that has cut, or begun to find the way, never goes back to `Leaf` or `Asking`.
- `L18_EveryQuestionStartsWithResearch`: only a Question's captain waits on a `council` or `his-call` Question.

The model cannot see which skill an agent loaded or what its brief said. Those are checked by reading, with the three checks above.

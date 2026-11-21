# Who sees what

Each agent gets only the skills and context its position needs, and the agent that dispatches it names those skills in the brief (`dydo/core.md`, "How the process must feel"). This page says what that is for each position on [the captain chart](chart.mmd). A skill file is checked against it with the three checks at the end.

"Text for a state" means the words that tell a captain what to do on each arrow out of that state.

## What every captain gets

Every agent dispatched onto a ticket is its captain, whatever the ticket (core, "A ticket"; K30). Each one gets:

- `AGENTS.md`. The repository gives it to every dispatched agent.
- `/ticket-captain`, named in its brief. It is the one captain skill (K30), and it holds or names the text for every state on the chart. Itself it holds the text for `Dispatched` and `Taken`, and the one line that goes back to the officer at `Done` and at `Handed back`. For every other state it names the text to load when the captain gets there (`AGENTS.md`: "each skill names the next"). Which file holds which state's text is the skill's to say; this page fixes who gets it.
- Its brief (K14, S15). The brief carries:
  - the skills it names;
  - the ticket and its contract;
  - how the ticket fits the ticket above it and the spec;
  - what the ticket's result is: a change, a verdict or an answer;
  - whether this captain merges its change, and into which branch;
  - the branch, worktree and base, and the paths it owns;
  - the gates, and how the work is proved;
  - the model and effort;
  - who its officer is.
- Every other skill the work needs, named in the brief with a slash.

A captain that dispatches another agent also gets the text for writing a brief, whatever its position: a leaf sends out a Question's captain, a splitter its parts, a merger its reviewer. The model roster is part of that text.

## Positions

| Position | Its states on the chart | Text it gets | Context its brief adds | What it never gets |
|---|---|---|---|---|
| A captain doing a leaf | `Leaf`, `Asking`. Not the arrow out of `Leaf` into `InReview`: that one is the merger's. | The text for `Leaf` and `Asking`, and the part for its ticket's result: writing a change, reaching a verdict, or answering a Question. | Nothing more. | The text for `Cutting` and `Waiting`, unless the arrow "the work proves too big" takes it there. `/ticket-captain` names that text at that arrow, and from then on it is a captain that splits. |
| A captain that splits | `Cutting`, `Waiting`. Not the arrow out of `Waiting` into `InReview`: that one is the merger's. | The text for `Cutting` and `Waiting`: the cut, a Question as one more part, the brief for each sub-ticket, the wait, what to do with a part that is handed back or that reports a problem, the wiring, and the two ways back to its officer. | Nothing more. | The part for doing the work. It only orchestrates. |
| A captain that merges its change | `InReview`, `Fixing`, `Merging`, and the two arrows into `InReview`: out of `Leaf` and out of `Waiting`. | The text for those three states and those two arrows: dispatching a reviewer, the Fix ticket, the merge. `/pr` for the PR body. | That it merges, and into which branch. | Nothing is withheld. A captain whose brief does not say it merges never gets this text. |
| A reviewer: the captain of a ticket whose result is a verdict | The states every captain has, and a leaf's or a splitter's. Never `InReview`, `Fixing`, `Merging`, and neither arrow into `InReview`. | The part for reaching a verdict, with the severities as the core page defines them. | The candidate to judge and its head; the ticket judged and its contract; the governing spec, or that there is none; the review level and its lenses; what the author already proved. | The text for `InReview`, `Fixing` and `Merging`. Its verdict is not reviewed again. |
| A Question's captain: the captain of a ticket whose result is an answer | The states every captain has, and a leaf's or a splitter's. Never `InReview`, `Fixing`, `Merging`, and neither arrow into `InReview`. | The part for answering a Question: research first, then the answer, which it decides. When the call is Balazs's it decides nothing: see "A Question" below. | The question, and the ticket that waits on it. | The text for `InReview`, `Fixing` and `Merging`. |
| A Wayfinding captain: the captain of a ticket whose result is a cut, the next tickets toward the spec | The same as a Question's captain. | `/wayfinder`, named in its brief: it is the part for a cut. | The ticket or Project whose route is not in sight, and its spec. | The text for `InReview`, `Fixing` and `Merging`. |
| A Fix's holder: a captain doing a leaf on a Fix ticket | `Leaf`, `Asking`, on the Fix ticket's own run of the chart | The same as a captain doing a leaf. | The blockers to fix, all of them, and the Review they come from. | The same as a captain doing a leaf. |
| A native helper | None. It holds no ticket, so it is not on the chart. | None. It does not read `AGENTS.md` and loads no skill unless its holder's prompt names one. | Only the prompt its holder writes. It shares its holder's checkout, and the holder commits. | Everything else. |

A Fix is a ticket of its own. By default its holder is the thread that wrote the change, and a fresh agent when that thread is full or the blocker says the approach was wrong (S9). When the captain that waits in `Fixing` is that same thread, it holds two tickets in turn: it does the Fix as a leaf, with a leaf's arrows, and then goes on from `Fixing` on its own ticket.

A review's captain may cut the review into seats, each with one kind of review (R34). A seat is on the reviewer's row. Its ticket is a part of the review, its result is its findings, and its brief adds its one kind. The review's captain gives the one verdict (S8). In the model a seat's run is a verdict's run.

## A Question

A call a captain may not decide becomes a Question ticket for a fresh agent (core, "Who decides what"). The chart draws one case of it, and this is the whole of that case:

- **Who opens it.** The captain that has the call. It opens the Question as a sub-ticket of its own ticket and dispatches a fresh agent onto it. A captain doing a leaf then waits in `Asking`. A captain that splits cuts the Question as one more part, in `Cutting`, and waits on it in `Waiting` beside its other parts. No captain passes a call up for its officer to open.
- **What its captain does.** How a Question gets answered is its captain's job (core: a Question captain decides its answer). It researches first, then decides. When the call is Balazs's, it decides nothing: it makes his question ready on the ticket, with every option, each option's trade-off and a recommendation, and hands the ticket back to its officer by the arrow "the work cannot be done inside this ticket". The ticket stays open: it is the ticket that waits on him.
- **A Question that comes back without its answer.** The captain that asked passes the ticket up to its own officer, as a problem outside its ticket (core, "How the process must feel"), and goes on waiting. Reporting changes no state, so `Asking` has one arrow out: the wait ends only when the answer is on the Question. A Question that comes back for any other reason, such as a ticket that could not be taken, gets a fresh agent from the captain that asked, and the wait goes on.
- **A Wayfinding captain** is on the row of states a Question's captain has. The model names three results: a change, a verdict and an answer. A cut is not one of them. Nothing is merged, so in the model a cut's run is an answer's run.

What each row rests on:

- A captain doing a leaf, and one that splits: core, "A ticket": a captain "either does the work itself, with native helpers, or splits the ticket and only orchestrates". K30: a leaf needs its kind's part, not the orchestration, and the escape hatch stays. H2 for the ways back to the officer.
- The model roster: core, "Who decides what": Balazs decides the model roster. R13: it is built into the text of every officer that dispatches, so it is skill text, not a slot of the brief, and every captain that dispatches gets it.
- A captain that merges its change: R23, a merge needs a PASS on its head; core, "the verdict is another agent's". `/pr` is the repository's skill for a PR body (`skills/engineering/pr/SKILL.md`: "Use when writing a PR body").
- A reviewer: core, "Review severities": "A verdict is not reviewed again." R3, R5, R6 and R34 for what its brief names.
- A Question's captain: core, "Who decides what": a Question captain decides its answer, and Balazs gets "a question with every option, each option's trade-off, and its recommendation". H16: research first. H16 has the officer that receives a need open the Question, and H23 stages a Question through research, council and his-call. There the core page wins: the captain with the call opens it, and how it gets answered is its captain's job (ruled at DYD-1454).
- A Wayfinding captain: core, "Who decides what": a Wayfinding captain decides the cut. H23: its skill is `wayfinder`.
- A Fix's holder: S9, one Fix ticket lists every blocker. K14: the findings go in a Fix's brief only.
- A native helper: K34, a native helper never reads `AGENTS.md`. H3, it shares its holder's checkout and the holder commits.

## How to check a skill file against this page

A position can take an arrow when the arrow leaves a state in its row and the position can meet the arrow's condition. Only a captain whose brief says it merges can meet "it merges it", so only that captain can take the two arrows into `InReview`.

1. Every arrow a position can take is told by text that position gets: the brief itself, a skill the brief names, or a text that such a skill names for that state.
2. No text a position gets tells an arrow that position cannot take. A leaf's text says nothing of cutting past the one arrow that leads there, and a reviewer's says nothing of a review of its own work, a Fix or a merge.
3. Every skill that the text tells a captain to give a sub-ticket's holder is named in that sub-ticket's brief, with a slash.

## What the model checks of this page

The properties in `verification/statechart/captain/` check the states column against the model:

- `R34_VerdictNeverReviewed`: a reviewer is never in `InReview`, `Fixing` or `Merging`.
- `Sees_OnlyAMergerSeesReviewFixMerge`: only a captain that merges a change is ever in those three states, so a Question's captain is not.
- `Core_SplitOnlyOrchestrates`: a captain that has cut never goes back to `Leaf` or `Asking`.

The model cannot see which skill an agent loaded or what its brief said. Those are checked by reading, with the three checks above.

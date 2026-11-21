# Who sees what

Each agent gets only the skills and context its position needs, and the agent that dispatches it names those skills in the brief (`dydo/core.md`, "How the process must feel"). This page says what that is for each position on [the captain chart](chart.mmd). A skill file is checked against it with the three checks at the end.

"Text for a state" means the words that tell a captain what to do on each arrow out of that state.

## What every captain gets

Every agent dispatched onto a ticket is its captain, whatever the ticket (core, "A ticket"; K30). Each one gets:

- `AGENTS.md`. The repository gives it to every dispatched agent.
- `/ticket-captain`, named in its brief. It holds the text for `Dispatched` and `Taken`, and the one line that goes back to the officer at `Done` and at `Handed back`.
- Its brief (K14, S15). The brief carries:
  - the ticket and its contract;
  - how the ticket fits the ticket above it and the spec;
  - what the ticket's result is: a change, a verdict or an answer;
  - whether this captain merges its change, and into which branch;
  - the branch, worktree and base;
  - the gates, and how the work is proved;
  - the model and effort;
  - who its officer is.
- Every other skill the work needs, named in the brief with a slash.

## Positions

| Position | Its states on the chart | Text it gets | Context its brief adds | What it never gets |
|---|---|---|---|---|
| A captain doing a leaf | `Leaf`, `Asking` | The text for `Leaf` and `Asking`, and the part for its ticket's result: writing a change, reaching a verdict, or answering a Question. | Nothing more. | The text for `Cutting` and `Waiting`. It loads that only on the arrow "the work proves too big". |
| A captain that splits | `Cutting`, `Waiting` | The text for `Cutting` and `Waiting`: the cut, the brief for each sub-ticket, the wait, the wiring. | The model roster, which Balazs writes. | The part for doing the work. It only orchestrates. |
| A captain that merges its change | `InReview`, `Fixing`, `Merging` | The text for those three states: dispatching a reviewer, the Fix, the merge. `/pr` for the PR body. | That it merges, and into which branch. | Nothing is withheld. A captain whose brief does not say it merges never gets this text. |
| A reviewer: the captain of a ticket whose result is a verdict | The states every captain has, and a leaf's or a splitter's. Never `InReview`, `Fixing`, `Merging`. | The part for reaching a verdict, with the severities as the core page defines them. | The candidate to judge and its head; the ticket judged and its contract; the governing spec, or that there is none; the review level and its lenses; what the author already proved. | The text for `InReview`, `Fixing` and `Merging`. Its verdict is not reviewed again. |
| A Question's captain: the captain of a ticket whose result is an answer | The states every captain has, and a leaf's or a splitter's. Never `InReview`, `Fixing`, `Merging`. | The part for answering a Question: research first, then the answer. When the call is Balazs's, the answer is his question made ready: every option, each option's trade-off, and a recommendation. | The question, and the ticket that waits on it. | The text for `InReview`, `Fixing` and `Merging`. |
| A Fix's writer: a captain doing a leaf whose change is a Fix | `Leaf`, `Asking` | The same as a captain doing a leaf. | The blockers to fix, all of them, and the Review they come from. | The same as a captain doing a leaf. |
| A native helper | None. It holds no ticket, so it is not on the chart. | None. It does not read `AGENTS.md` and loads no skill unless its holder's prompt names one. | Only the prompt its holder writes. It shares its holder's checkout, and the holder commits. | Everything else. |

What each row rests on:

- A captain doing a leaf, and one that splits: core, "A ticket": a captain "either does the work itself, with native helpers, or splits the ticket and only orchestrates". K30: a leaf needs its kind's part, not the orchestration, and the escape hatch stays.
- A captain that merges its change: R23, a merge needs a PASS on its head; core, "the verdict is another agent's". R13 for the roster, which the core page gives to Balazs.
- A reviewer: core, "Review severities": "A verdict is not reviewed again." R3, R5, R6 and R34 for what its brief names.
- A Question's captain: core, "Who decides what": a Question captain decides its answer, and Balazs gets "a question with every option, each option's trade-off, and its recommendation". H16: research first.
- A Fix's writer: S9, one Fix lists every blocker. K14: the findings go in a Fix's brief only.
- A native helper: K34, a native helper never reads `AGENTS.md`. H3, it shares its holder's checkout and the holder commits.

## How to check a skill file against this page

1. Every arrow out of a state in a position's row is told by text that position gets: a skill its brief names, or the brief itself.
2. No text a position gets tells an arrow out of a state that is outside its row. A leaf's text says nothing of cutting past the one arrow that leads there, and a reviewer's says nothing of a Fix or a merge.
3. Every skill that the text tells a captain to give a sub-ticket's holder is named in that sub-ticket's brief, with a slash.

## What the model checks of this page

The properties in `verification/statechart/captain/` check the states column against the model:

- `R34_VerdictNeverReviewed`: a reviewer is never in `InReview`, `Fixing` or `Merging`.
- `Sees_OnlyAMergerSeesReviewFixMerge`: only a captain that merges a change is ever in those three states, so a Question's captain is not.
- `Core_SplitOnlyOrchestrates`: a captain that has cut never goes back to `Leaf` or `Asking`.

The model cannot see which skill an agent loaded or what its brief said. Those are checked by reading, with the three checks above.

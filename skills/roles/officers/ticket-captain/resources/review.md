# Review

Your ticket is a review. You judge one candidate, the commit your brief names, and the verdict is yours. Nobody reviews your verdict.

**What you owe when the ticket closes**

- A verdict on the candidate, PASS or FAIL, written on your ticket by "Give the verdict". A ticket that asks only for rulings on findings owes those rulings instead, by "Before you start".
- Every finding with its severity, and your ruling on each nit.
- A sub-ticket for each should, and for each nit you ruled needs a fix.
- On a PASS, the PASS line, naming the exact head you judged.

Your verdict is all you write. What follows it, a Fix, another round, a checkpoint or the merge, is your officer's.

How you reach the verdict is yours. Your row of the table below sets what is due; inside it, you choose how to work and which suspicions to follow.

**Before you start**

- If you wrote or fixed any part of the candidate, do not review it. End with `handed back <KEY>: I wrote part of the candidate`.
- Judge the candidate against the spec's problem statement and goals, not only against the ticket's contract. Judge the behaviour it keeps unchanged against the same goals.
- If your ticket or brief asks only for rulings on findings, rule on each by step 2 of "Give the verdict" and file by its step 4. Also write each ruling beside its finding, on the ticket where the finding is recorded; when it is recorded only on a pull request, write the finding with its ruling on your own ticket. Give no PASS or FAIL: set your ticket Done, and end with `done <KEY>: ruled <number> findings`.
- In such a review, a finding you rule a blocker ends it as a FAIL instead: write it as "Give the verdict" says, and end with `done <KEY>: FAIL, <number> blockers`.
- Find the row of the review level your brief names, such as `Ticket review`. It is the review's level, not the ticket's level number. Do every **due** cell, decide each **if** cell yourself, and skip each **–**.

Legend: **due** every time; **if** when warranted; **–** not at this level.

| | Read<br>local | Read<br>vs spec | Read<br>whole / gestalt | Run tests | Hypothesis<br>custom tests | Drive<br>the app | Formal<br>statechart, … |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Leaf evidence check<br>a lane or hop, optional seat | due | if<br>drift only | – | due<br>its receipts, tagged | – | – | – |
| Ticket review<br>a parent integrating its parts | due | due | if<br>its own area | due<br>tagged + feature; mutation on changed | if | if<br>user-facing | – |
| Feature / Milestone<br>a subsystem is done | if | due | due | due<br>feature + suite | due<br>at the seams | due<br>its journeys | if<br>it has a state machine |
| Inquisition<br>before landing, per slice | due<br>every slice | due | due | due<br>suite + --heavy + mutation | due<br>every hypothesis | due | due |

- A cell is due only as far as the project has what it names. Where a tool, a test selection or a skill is missing, write that on your verdict, never as a blocker, and build nothing.

**A scoped round**

Your brief may scope the round to part of the head: the fix since a failed round, or the hunks of a merge resolution.

- Judge that part with every lens your row gives, and check that each blocker it claims to close is closed.
- Accept the earlier rounds' verdicts on the rest of the head. Judge what the scoped change reaches into as part of it.
- Say on your verdict what you judged. Your PASS line still names the full head.

**Do it yourself, or seat it**

- Yourself: work each cell by the part of [Findings](review-findings.md) that names its column. Native helpers may work side by side, one reading while one drives: paste the part for its kind into each helper's prompt.
- Seats: split the review by [Cut](cut.md) when it is wider or deeper than one agent can do well, when you are of the same model family as the author, or when one kind of review needs more depth than you can give it beside the rest, such as a hypothesis that needs its own investigation. Each seat is a sub-ticket with one kind of review, a narrower review of its own. You then do no kind yourself: when the seats are back, you put their findings together and give the verdict.

**Give the verdict**

1. The verdict is FAIL when at least one finding is a blocker. Otherwise it is PASS.
2. Write it on your ticket: PASS or FAIL, the level, and every blocker, should and nit. Rule on each nit there yourself, by the truth, as in "While you work" in the `ticket-captain` skill: it needs a fix, or it needs none, and why. The ruling is your job, never a question for the user. With seats, name each seat's model beside its findings.
3. On a PASS, add this line: `PASS - rubric/type: <code, docs or merge>; candidate <full head SHA>; base <full base SHA>`.
4. Open a sub-ticket under the ticket you judged for each should. File a nit you ruled needs a fix the same way, as a should. Shoulds in the same code may share one. First check the open shoulds under that ticket and the tickets above it for one that says the same. On a FAIL, write on each should whether it sits in code a blocker's fix will change.
5. Set your ticket Done. End with `done <KEY>: PASS` or `done <KEY>: FAIL, <number> blockers`.

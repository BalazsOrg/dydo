# Review

Your ticket is a review. You judge one candidate, the commit your brief names, and the verdict is yours. Nobody reviews your verdict.

**Before you start**

- If you wrote or fixed any part of the candidate, do not review it. End with `handed back <KEY>: I wrote part of the candidate`.
- Judge the candidate against the spec's problem statement and goals, not only against the ticket's contract. Judge the old behaviour it keeps the same way.
- Find the row of the level your brief names. Do every **due** cell, decide each **if** cell yourself, and skip each **–**.

Legend: **due** every time; **if** when warranted; **–** not at this level.

| | Read<br>local | Read<br>vs spec | Read<br>whole / gestalt | Run tests | Hypothesis<br>custom tests | Drive<br>the app | Formal<br>statechart, … |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Leaf evidence check<br>a lane or hop, optional seat | due | if<br>drift only | – | due<br>its receipts, tagged | – | – | – |
| Ticket review<br>a parent integrating its parts | due | due | if<br>its own area | due<br>tagged + feature; mutation on changed | if | if<br>user-facing | – |
| Feature / Milestone<br>a subsystem is done | if | due | due | due<br>feature + suite | due<br>at the seams | due<br>its journeys | if<br>it has a state machine |
| Inquisition<br>before landing, per slice | due<br>every slice | due | due | due<br>suite + --heavy + mutation | due<br>every hypothesis | due | due |

> Every row: never re-run what the author ran the same way; a test receipt for the same state counts (spec R5). The security baseline is always on.
>
> How it's used: the officer's brief names the level; the reviewer works its row.

- A cell is due only as far as the project has what it names. Where a tool, a test selection or a skill is missing, write that on your verdict, never as a blocker, and build nothing.

**Do it yourself, or seat it**

- Yourself: work each cell by the part of [Findings](review-findings.md) that names its column. Native helpers may work side by side, one reading while one drives: paste the part for its kind into each helper's prompt.
- Seats: split the review by [Cut](cut.md) when it is wider or deeper than one agent can do well, or when you are of the same model family as the author. Each seat is a sub-ticket with one kind of review. You then do no kind yourself: when the seats are back, you put their findings together and give the verdict.

**Give the verdict**

1. The verdict is FAIL when at least one finding is a blocker. Otherwise it is PASS.
2. Write it on your ticket: PASS or FAIL, the level, and every blocker, should and nit. With seats, name each seat's model beside its findings.
3. On a PASS, add this line: `PASS - rubric/type: <code, docs or merge>; candidate <full head SHA>; base <full base SHA>`.
4. Open a sub-ticket under the ticket you judged for each should. Shoulds in the same code may share one. First check the open shoulds under that ticket and the tickets above it for one that says the same.
5. Set your ticket Done. End with `done <KEY>: PASS` or `done <KEY>: FAIL, <number> blockers`.

<!-- built from the model; edit there: tools/SkillBuild -->

# Review

Your ticket is a review: one candidate, judged at the level your brief names, and the verdict is yours.
Never judge work you implemented or fixed.

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

- Drive and Formal are due only where the tool is built and the subject is settled; report a tool the project lacks to your officer as a finding, instead of building it.
- Judge the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals, not only the contract.
- Give every finding one severity: a blocker, the intended case is wrong; a should, a real bug callers will hit, inside the intent; a nit, everything else.
- Report every visible blocker in one pass.
- Run lanes through native helpers when your row calls for both reading and driving, and put their findings together yourself.

**Your return:**

- Holding the round alone: record it yourself, by [A round's record](round.md).
- Sitting as one seat among several: return your findings, each with its severity, to the captain holding the round.
- Asked to judge a conflict's resolution: judge its hunks, the only new work on the branch, and name the new head in your PASS.

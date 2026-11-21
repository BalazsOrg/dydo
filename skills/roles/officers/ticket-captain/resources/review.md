<!-- built from the model; edit there: tools/SkillBuild -->

# Review

Your ticket is a review: one candidate, judged at the level your brief names. Hold the review, and its verdict is yours; sit as one seat of it, and your findings are.
Never judge work you implemented or fixed.

| Spine phase | You hold the review | You are one seat of it |
| -- | -- | -- |
| 1. Take the ticket | use | use |
| 2. Do it or split it | use: a leaf, or a split into seats | use: a leaf, unless your slice proves too big |
| 3. Dispatch | skip, unless you send seats | skip, unless you divide a slice too big to hold |
| 4. Get it reviewed | skip | skip |
| 5. Finish | use: the Nobody case | use: the Nobody case |
| When things go wrong | use | use |

- A verdict is never reviewed: phase 4 is for a change, so no round is opened on your verdict or on a seat's findings, and what you return is your result.
- Judge the change, and the behaviour it inherits unchanged, against the spec's problem statement and goals, not only the contract.
- Give every finding one severity: a blocker, the intended case is wrong; a should, a real bug callers will hit, inside the intent; a nit, everything else.
- Report every visible blocker in one pass.

**If you are one seat:**
Your brief names your one kind of review, and you are a leaf that stays one: read the part below headed by your kind, leave "If you hold the review" unread, and return your findings, each with its severity, to the captain holding the round.
A leaf divides only what is too big for one agent, as a large read can be. Divide such a slice by [Too big to hold](too-big.md), your findings so far standing in for its commit and each part a slice that reads whole on its own, and return the parts' findings with yours.

**Reading:**

- Local: read each changed file, and each function in it, on its own. Does it work, and is anything plainly wrong, such as a security hole, a bad practice or waste?
- Against the spec: read the change beside the governing spec and the code around it, for anything built on an assumption the spec does not make.
- The whole: read the folders and the structure together, for what no single file shows.
- Read the tests as part of the change. One that pins a rule no spec or decision record traces, changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding; so is a job tag its test does not earn.
- On docs, a reason that links no decision record or principle is a finding; on code, a change to paths that a doc's `explains:` declares means reading that doc too.
- Write a finding so it can be checked without you: both passages quoted at `file:line`, or, for an unused member, its definition with a reference search that finds no caller outside the tests.

**Trying to break it:**

- Run the tests your row names with your project's own commands, which `dydo/guides/testing-strategy.md` holds; run again what the author ran the same way only when later changes put its result in doubt.
- Name what you suspect is wrong, then decide it with a test of your own on the candidate's commit: a test that fails for the reason you named proves the finding.
- Put that test and its failing output in the finding, and commit nothing to the candidate's branch.
- At a ticket review, mutation means reading the results that already exist for the changed code: start no mutation run, wait on none, and note a missing result, never as a blocker.

**Driving the feature:**

- Drive through your project's verification skill, the one your brief names, whose name starts with `verify-`: follow its Launch, Doctor, Drive, Evidence and Cleanup sections, in that order.
- Use the feature the way its user would, by the routes that skill's feature map lists for it, and say for each whether it works as the spec's problem statement intends.
- Do not repeat a journey the author drove the same way and holds a receipt for on the same state, unless later changes put its result in doubt: the receipt counts, and your driving goes to what it left out.
- Return each journey you drove as steps anyone can rerun, with what you observed beside what the spec expects, and list apart each receipt you counted without driving: you vouch only for what you observed yourself, in this session.
- What a user can do and the skill cannot drive is a finding to return, never a driver to build; change no product code while you drive.
- Driving is due only where that skill is built and covers the feature, and the feature is settled; where it is not, return that as your finding.

**Modelling it as a statechart:**

- Where your project has a skill that checks the subject formally, run that check first, and hold what it describes against the code as it is now.
- Otherwise write a bounded model of the subject's states, each rule the spec promises as an invariant, and check it with `dydo statechart <spec> --skill <statechart-skill-folder>`; the skill in that folder says how to write the model and how to read its report.
- Return each rule that breaks with its shortest path, told as steps in the subject's own words, and say what the model's bounds leave out.
- Modelling is due only for a finished subsystem that has a state machine; for anything else, return that it is not due.

**If you hold the review:**

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

- Work the row of the level your brief names: every **due** cell is owed, and each **if** cell is your call. The kinds above are its columns: Reading, the three Read columns; Trying to break it, Run tests and Hypothesis; Driving the feature, Drive; Modelling it as a statechart, Formal.
- A cell is due only as far as the project has what it names: Drive and Formal where the tool is built and the subject is settled, any other cell where its tools, selectors and results exist. Report what is missing to your officer as a finding, instead of building it.
- A review that fits you is a leaf: do its kinds yourself, or run them as lanes through native helpers, and put their findings together yourself.
- Seat the kinds you do not do yourself, one seat a kind, in parallel, by phase 3 of the spine. A seat from another model family goes out with `delegate_task`, on a leaf sub-ticket of its own under your Review ticket.
- Brief each seat by [Briefs](briefs.md), adding its one kind and the cells of your row it works; a native helper gets no ticket and loads no skill, so put its kind's part in its prompt.
- Post the round's one verdict yourself, by [A round's record](round.md): with seats, once they are back, from their findings put together, naming each seat. A seat posts none.
- Asked to judge a conflict's resolution: judge its hunks, the only new work on the branch, and name the new head in your PASS.

<!-- built from the model; edit there: tools/SkillBuild -->

# Toolset

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

## Drive and Formal

- Drive and Formal are due only where the tool is built and the subject is settled; a bare **due** in those columns never overrides this.
- Drive needs a built, maintained driving skill that matches the feature.
- Formal needs a purpose-built model of the subject, or a statechart, which is only for a finished subsystem that has a state machine.
- Report a tool you want but the project hasn't built to your officer as a finding, rather than building it.

## Using each tool

- Read local: the file, and the function or method in it, on their own: does it work, and is anything blatantly wrong, such as a security hole, a bad practice or waste?
- Read vs spec: the change against the governing spec and the rest of the code, so nothing builds or assumes against the spec and drifts.
- Read whole: the folders and the structure as a whole, where a problem can hide in the gestalt.
- A reading finding quotes both passages at `file:line`; an unused member quotes its definition and a reference search that finds no caller outside the tests.
- Run tests: the flavour the row names, from the tagged sub-feature through the whole feature and the suite to `--heavy` and mutation testing the tests.
- Re-run what the author already ran, driving included, only when enough has changed since that its result is in doubt.
- At a ticket review, "mutation on changed" means reading the DYTEST mutation results that already exist for the changed code: queue no mutation work and wait on none, and note a missing result on the verdict, never as a blocker.
- Hypothesis: name what you suspect is wrong and decide it with a custom test of your own, in your own worktree at the candidate SHA; a test red for the stated reason is the finding's proof.
- Drive: use the app the way a user would, through its driving skill, and judge the experience and the behaviour against the spec's problem statement; rerunnable steps with the observed and the expected output are the proof.
- Formal: run the subject's own formal verification, or `/statechart` on its state machine, to find gaps in behaviour from a model instead of the running code.

## Tests and docs

- A test that pins an untraced rule, new or inherited, is a finding.
- Check each graduating test's job tag against its evidence.
- A test that changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding.
- Coverage is a report, never a blocker.
- On docs, an unlinked 'why' is a finding; on code, a change to paths a doc's `explains:` declares means checking that doc.

## Lenses

- Read, Break and Drive are your lenses, weighted by your level's row.
- At the inquisition each lens is its own seat: Read covers the three Read columns; Break runs the tests (suite, `--heavy`, mutation), the hypothesis tests and the formal checks; Drive drives the app.

## Read phase on an area

Nominal path:

- In an inquisition round, run your phase on your area; by default an area goes Read, then Break, then Drive, and every round starts from zero.
- Read: look for what is broken in ways neither tests nor clicking around would catch.

## Break phase on the area

Nominal path:

- Break: form hypotheses about how the area fails, then write tests that confirm or refute them.

## Drive phase on the area

Nominal path:

- Drive: run every feature and edge case through the running app, against the spec's problem statement.

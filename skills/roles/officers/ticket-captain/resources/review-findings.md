# Findings

You look at a candidate for what is wrong with it. Each problem you find is a finding. Your brief, or your row of the review table, says which kinds of looking are yours. As one seat of a review, your findings are your ticket's result.

**While you look**

- Accept as proof what the author already proved. A test or a journey the author ran the same way on the candidate's commit counts: do not rerun it. Rerun one that ran on an earlier commit only when later changes may have broken it. List what you accepted apart from what you saw yourself.
- Commit nothing to the candidate's branch, and change no product code.

**Every finding**

- Give it one severity:
  - A **blocker** is a problem so bad that it needs fixing now.
  - A **should** is a problem that needs a fix, but the fix can wait.
  - A **nit** is an observation. It may need no fix at all.
- Write it so that it can be checked without you: quote the passage at `file:line`, or give the steps you took and what they printed.
- Report every blocker you can see in one pass. Do not stop at the first.

**Reading** (the three Read columns)

- Local: read each changed file, and each function in it, on its own, for what is plainly wrong: a bug, waste, and at every level a security hole.
- Against the spec: read the change beside the spec and the code around it, for anything built on an assumption the spec does not make.
- The whole: read the folders and the structure together, for what no single file shows.
- Read the tests as part of the change, by "In review" in `/tdd`.

**Trying to break it** (the Run tests and Hypothesis columns)

- Run the tests your row names that no receipt you accepted already covers.
- Name what you suspect is wrong, then write a test of your own on the candidate's commit. If it fails for the reason you named, put the test and its failing output in the finding.
- Such a test is a proof test, by "Two kinds of test" in `/tdd`.

**Driving** (the Drive column)

- Drive each journey that no receipt you accepted already covers. Use the feature as its user would, with the verification skill your brief names. Say for each journey whether it does what the spec's problem statement intends.
- Write each journey as steps anyone can rerun, with what you saw beside what the spec expects.

**Modelling** (the Formal column)

- Model only a finished subsystem that has a state machine. For anything else, write that modelling is not due.
- With the skill your brief names, model the subsystem's states and each rule the spec promises. Write each rule that breaks as a finding.

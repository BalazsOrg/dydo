<!-- built from the model; edit there: tools/SkillBuild -->

# Implement

Your ticket is a change: implement a contract, fix a review's findings, or resolve a merge conflict.
Verify your work while you build it, each file the way it is used: run the code and drive the app, render an image and look at it, hold a doc against its source.
Below the rules for every change, each part is headed by the work it is for: read the ones that name yours.

| Spine phase | You |
| -- | -- |
| 1. Take the ticket | use |
| 2. Do it or split it | use: a leaf, unless the change proves too big |
| 3. Dispatch | skip; use it only if the merge is yours, or you split |
| 4. Get it reviewed | skip, since a part is reviewed with its whole; use it only if the merge is yours, or you split |
| 5. Finish | use: the case your brief names |
| When things go wrong | use |

- Leave what you touched no worse than you found it.
- Keep every file you change, tests included, within the file size limit your brief's gates check, and split one that passes it.
- One writer per worktree: commit only inside the paths your ticket owns.
- Load /domain-modeling when you touch a glossary term or a Decision Record, and /codebase-design when you design a seam or an interface.
- An open design question is a Question sub-ticket, never yours to settle; /prototype is the skill that answers one.
- A fix for a mistake that had already landed carries the lint, test or rule that keeps it from coming back.

**Tests:**

- Write a suite test for one of three jobs only: to guard a trap, to pin a contract the spec or a decision record traces, or to reach an edge that driving the app cannot.
- Test at the deepest module boundary, or the product's, that can observe the behaviour; add a unit test only for an edge or trap that test cannot reach cheaply.
- Mock only at the system's edge, such as an external API, time or randomness; your own modules stay real.
- Show every new test failing without the code: set the code aside, run the test, see it fail for the reason it names, and put the code back.
- Tag each suite test with its job, the way the tests beside it carry theirs: trap, naming the bug or pitfall; contract, citing the spec or decision record; or edge.
- Write no test that changes whenever the code does, checks that a deleted feature is gone, or can never fail.
- A rewrite or wide refactor gets a conformance suite beside its own tests, never in their place; its assertions that only match the old implementation go once the rewrite has replaced it. Property tests for edges and differential tests for rewrites are guidance, never a gate.
- Where your contract marks an acceptance line for a scenario, write and wire it as `dydo/guides/coding-standards.md` says under Testing, and never weaken one.

**Code:**

- Your project's own rules are in `dydo/guides/coding-standards.md`: read it before you write, and follow it wherever it asks more than this page does.
- Before you write a comment, read Comments in that file: your project says there what a comment is for and what it must never be.
- The static gates and their numbers, cognitive complexity among them, are your project's: `dydo/guides/coding-standards.md` gives the numbers under Testing, and `dydo/guides/testing-strategy.md` the command that runs the gates.
- When your change trips a static gate, change the code until it passes, splitting or flattening the method it names: a gate takes no suppression, and its number is never raised.
- Prove platform-specific code, such as processes, paths and signals, on CI's platform too, not only on yours.

**Docs:**

- Every sentence you write is a claim that needs a witness in the repository: code, configuration, or governing intent such as the spec, a decision record or a ticket. Where there is none, return the gap instead of the sentence.
- Write each meaning in one home and link to it from everywhere else; change a built file only in the source its header names.
- Write a skill, its resources or an entry file with /writing-for-agents.
- A descriptive doc explains and never authorises: on what the code does the code wins, and a reason counts only when it links a decision record or a principle. Write a new one only when invited, and only if its subject is solid, important and expensive to rebuild from code.
- When `dydo check` flags a doc whose `explains:` paths your change touches, update the doc or state in your return that it still holds.
- Your project's doc conventions, frontmatter and naming among them, are in `dydo/reference/writing-docs.md`.
- Prove a docs change with `dydo check` and the checks your brief names, and run `dydo fix` after you add, move or rename a document.

**On a Bug:**

- Name the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.
- Reproduce it red before you fix it: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction. With none yet, load /diagnosing-bugs.
- An unused member needs no test: its definition at `file:line` and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.
- After the fix, revert it, watch the reproduction fail, and restore it; return that red line, with the behaviour you kept still green.
- Force a race with fake timers or a set interleaving: a test that needs a sleep or a timeout is wrong.

**On a Fix:**

- Check each finding's severity against the source, not its label.
- Close only the findings your brief names, preferring the change that deletes or narrows code.
- Put a label you dispute in your return, with its evidence: file:line, test or output.

**On a merge conflict:**

- Resolve it on the ticket's branch only, as a merge commit that keeps both parents.
- Read both sides, and what changed since the source was reviewed, before you resolve a hunk; one that needs a new product choice is a Question.
- State the behaviour each hunk you resolved is meant to have, and return both parents with the merge commit's SHA.
- A resolution that refactors leaves the code no worse than either side had it.

**On a batch of shoulds:**

- Move each should under your ticket, keeping a `related: found in <KEY> rN` link on it.
- Mark a duplicate Duplicate, and cancel a should only with a one-line reason.

**To finish:**
End on a commit `<KEY> <hop>: <what>`, `<hop>` being `implement`, `fix` or `merge`, never squashed.
Your receipts are the commands you ran on that commit and their results.

<!-- built from the model; edit there: tools/SkillBuild -->

# Implement

Your ticket is a change: implement a contract, fix a review's blockers, or resolve a merge conflict.
Verify your work while you build it, each file the way it is used: run the code and drive the app, render an image and look at it, hold a doc against its source.

- Leave what you touched no worse than you found it.
- Keep every file you change at or under 20,000 bytes (5k tokens at bytes ÷ 4), tests included, and split one that passes it.
- One writer per worktree: commit only inside the paths your ticket owns.
- Load /domain-modeling when you touch a glossary term or a Decision Record, and /diagnosing-bugs for a Bug with no red reproduction yet.
- An open design question is a Question sub-ticket, never yours to settle; /prototype is the skill that answers one.
- A Bug starts from a red reproduction, and every new test is shown failing without the code.
- A fix for a mistake that had already landed carries the lint, test or rule that keeps it from coming back.

**On a Fix:**

- Check each blocker's severity against the source, not its label.
- Fix only the blockers, preferring the change that deletes or narrows code.
- Put a label you dispute in your return, with its evidence: file:line, test or output.

**On a merge conflict:**

- Resolve it on the ticket's branch only, as a merge commit that keeps both parents.
- Read both sides, and what changed since the source was reviewed, before you resolve a hunk; one that needs a new product choice is a Question.

**On a batch of shoulds:**

- Move each should under your ticket, keeping a `related: found in <KEY> rN` link on it.
- Mark a duplicate Duplicate, and cancel a should only with a one-line reason.

**To finish:**
End on a commit `<KEY> <hop>: <what>`, `<hop>` being `implement`, `fix` or `merge`, never squashed.
Your receipts are the commands you ran on that commit and their results.

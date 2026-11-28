# Implement

Your ticket is a change. How you build it is yours. It is done when you have seen it work, not when the code is written.

**What you owe when the ticket closes**

- The change, on your ticket's branch, inside the paths it owns. Change only what your ticket needs.
- Proof on your last commit that it works the way it will be used: run the command, drive the app, open the page.
- Every check green. Fix a failing check in the code; never weaken or skip it. Delete a test only when `/tdd` or a decision record says it goes, and say why in the commit.
- Each test you add to the suite, move or rewrite, meeting `/tdd`, with its red check on the ticket.
- Your result: your last commit, named `<KEY> <hop>: <what>`, where `<hop>` is `implement`, `fix` or `merge`; its SHA, the commands you ran on it, and what each printed.

**The rules the change meets**

- Code and comments: from the repository root, read `dydo/guides/coding-standards.md` before you write.
- Tests: `/tdd`. Where a testing line in the coding standards disagrees with it, `/tdd` wins.
- A skill, a skill's resource or an `AGENTS.md`: write it by `/writing-for-agents`.
- Any other doc: state only what you checked against its source: the code, or the record that governs it.

**A bug**

Start from a failing reproduction, by "A bug starts red" in `/tdd`. Then fix the bug.

**A removal**

When code has no caller left, a reference search that finds none outside the tests is its proof. Delete its test-only callers with it.

**A fix after review**

Fix every blocker your Fix ticket lists, and nothing else.

**A merge conflict**

Resolve it as a merge commit on the ticket's branch. A resolution that refactors leaves the code no worse than either side. If a hunk needs a decision neither side made, the decision becomes a Question: go by [Ask](ask.md).

**Often, not always**

- A fact the change rests on that nobody has checked becomes a Question by [Ask](ask.md), like a call.
- After you finish, a review's Fix may come back to your thread. Do it by "A fix after review".

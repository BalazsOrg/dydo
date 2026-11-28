# Implement

Your ticket is a change. How you build it is yours. It is done when you have seen it work, not when the code is written.

**What you owe when the ticket closes**

- The change, on your ticket's branch, inside the paths it owns. Change only what your ticket needs.
- Proof on your last commit that it works the way it will be used: run the command, drive the app, open the page.
- Every check green. Fix a failing check in the code; never weaken or skip it. A test goes when `/tdd` or a decision record says it goes; say why in the commit.
- Each test you add to the suite, move or rewrite, meeting `/tdd`, with its red check on the ticket.
- Your result: your last commit, named `<KEY> <hop>: <what>`, where `<hop>` is `implement`, `fix` or `merge`; its SHA, and the commands that prove it with what each printed. A receipt for the same tree counts while no later change puts it in doubt.

**The rules the change meets**

- Code and comments: from the repository root, read `dydo/guides/coding-standards.md` before you write.
- Tests: `/tdd` decides which tests you write and keep, over any line of the coding standards that disagrees. The project's gates and their thresholds stay as the project sets them.
- A skill, a skill's resource, an `AGENTS.md` or a `CLAUDE.md`: write it by `/writing-for-agents`.
- Any other doc: state only what you checked against its source: the code, or the record that governs it.

**A bug**

Start from a failing reproduction, by "A bug starts red" in `/tdd`.

**Dead code**

Show that nothing calls the dead code you remove: a reference search of the repository, and for a public API or an entry point, its outside callers too. Its tests go by the check line above.

**A fix after review**

Fix every blocker your Fix ticket lists, and nothing else.

**A merge conflict**

Resolve it as a merge commit on the ticket's branch. A resolution that refactors leaves the code no worse than either side. A hunk that needs a decision neither side made is a call: decide it when your ticket makes it yours, or when it is obvious and you can undo it; otherwise pass it up to your officer.

**Often, not always**

- A change follows a prototype. Its validated logic may be reused once review and refactoring bring it to production standards.
- After you finish, a review's Fix ticket comes to your thread, because you wrote the change. Do it by "A fix after review".

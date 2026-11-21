# Implement

Your ticket is a change. It is done when you have seen it work, not when the code is written.

**Before you write**

Read `dydo/guides/coding-standards.md` and `dydo/guides/testing-strategy.md`. They hold the rules for code, comments and tests, and the commands for every check.

**While you build**

- Prove the change the way it will be used: run the command, drive the app, open the page.
- Test through the public interface: the command, the endpoint, the exported function.
- Watch every new test fail before you make it pass.
- Fake only what the project does not own: the network, the clock, a third-party service.
- Change only what your ticket needs.
- Fix a failing check in the code. Never weaken, skip or delete the check.

**A doc**

When the change is a doc, write it by `dydo/reference/writing-docs.md`, and state only what you checked in the code.

**A bug**

Write a test that fails because of the bug. Fix the bug. Then undo the fix once and watch the test fail again.

**A fix after review**

Fix every blocker your Fix ticket lists, and nothing else. Prefer the fix that deletes code. If you think a blocker is wrong, leave it and say so on the Fix ticket, with the evidence.

**A merge conflict**

Resolve it as a merge commit on the ticket's branch. Read what each side meant before you touch a hunk. If a hunk needs a decision neither side made, the decision becomes a Question: go by [Ask](ask.md).

**Finish**

Name your last commit `<KEY> <what>`. Your result is its SHA, the commands you ran on it, and what each printed.

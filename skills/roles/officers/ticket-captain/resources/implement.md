# Implement

Your ticket is a change. It is done when you have seen it work, not when the code is written.

**Before you write**

Read the rules for code, comments and tests that your brief names. They differ from project to project.

**While you build**

- Prove the change the way it will be used: run the command, drive the app, open the page.
- Test through the public interface: the command, the endpoint, the exported function.
- Watch every new test fail before you make it pass.
- Change only what your ticket needs.
- Fix a failing check in the code. Never weaken, skip or delete the check.

**A doc**

When the change is a doc, state only what you checked in the code.

**A bug**

Write a test that fails because of the bug. Then fix the bug.

**A fix after review**

Fix every blocker your Fix ticket lists, and nothing else.

**A merge conflict**

Resolve it as a merge commit on the ticket's branch. If a hunk needs a decision neither side made, the decision becomes a Question: go by [Ask](ask.md).

**Finish**

Name your last commit `<KEY> <hop>: <what>`, where `<hop>` is `implement`, `fix` or `merge`. Your result is its SHA, the commands you ran on it, and what each printed.

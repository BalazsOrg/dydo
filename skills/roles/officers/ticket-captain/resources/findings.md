<!-- built from the model; edit there: tools/SkillBuild -->

# A finding's route

**A blocker, by what it shows:**

- Work that is broken, against the spec or a security hole: a Fix ticket by [Fix rounds](fix-rounds.md), one sub-ticket per blocker inside it. (open: his call)
- A fundamental fault, or a new requirement: your officer, for a re-plan.
- A need only the user can meet: your officer, at once, by [A need for the user](need-for-the-user.md).

**A should, by what its fix costs:**

- Cheap: have it fixed before the merge. Name it in the round's Fix brief; when the round has no blocker, dispatch a writer onto its ticket, and order one more round on the new head.
- Anything more: it waits on its ticket, and its parent may close Done meanwhile.

Take a finding on a change you wrote as the reviewer wrote it: a fresh writer fixes it, and if you doubt it, the doubt is a Question sub-ticket, never your call.

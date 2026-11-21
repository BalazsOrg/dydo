<!-- built from the model; edit there: tools/SkillBuild -->

# A red gate after the merge

Rerun the red run once.
Green on the rerun is a flake: the merge is done.
Note the flake in one line, the red run and the green rerun, on the Merge sub-ticket, or on your own ticket where your work was not cut into steps.

**Red again is real:**

1. Revert the merge on the feature branch at once: `git revert -m 1 <merge SHA>`, pushed.
2. Cancel the Merge sub-ticket with `reverted: <gate> red on <merge SHA>`, and file what broke as a ticket in the team's CI findings; where your work was not cut into steps, write that line on your own ticket, which stays open.
3. Open the repair as steps, each blocked by the one before with Linear's `save_issue` (`blockedBy`): an Implement that fixes it on your ticket's branch and brings the reverted change back there, a Review scoped to the fix, and a new Merge, the cancelled one left as it is. Where your work was not cut into steps, open no Merge: your own ticket stays the record.
4. Dispatch the Implement, and when it returns open a new PR for the branch: `dydo merge` undoes no revert, and calls the old PR already integrated.
5. Dispatch the Review, its brief naming the new PR, and once it passes merge again, by [Merging](merging.md).

Take a red you cannot get fixed to your officer, with the merge SHA and the gate.

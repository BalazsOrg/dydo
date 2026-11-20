<!-- built from the model; edit there: tools/SkillBuild -->

# A red gate after the merge

Rerun the red run once.
Green on the rerun is a flake: the merge is done.

**Red again is real:**

1. Revert the merge on the feature branch at once: `git revert -m 1 <merge SHA>`, pushed.
2. Cancel the Merge sub-ticket with `reverted: <gate> red on <merge SHA>`, and file what broke as a ticket in the team's CI findings.
3. Dispatch an Implement ticket to fix it on your ticket's branch, then a review round scoped to the fix.
4. Merge the fixed branch again through a new PR that brings the reverted change back itself: `dydo merge` undoes no revert, and calls the old PR already integrated.

Take a red you cannot get fixed to your officer, with the merge SHA and the gate.

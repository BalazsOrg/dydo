<!-- built from the model; edit there: tools/SkillBuild -->

# A red gate after the merge

1. Rerun the red run once.
2. Green on the rerun is a flake: the merge is done.
3. Red again is real: revert the merge at once, cancel the Merge sub-ticket with `reverted: <gate> red on <merge SHA>`, and file a Bug for what broke.
4. Dispatch an Implement ticket to fix it on your ticket's branch, then a review round scoped to the fix.
5. Merge the fixed branch again through a new PR that brings the reverted change back itself: `dydo merge` undoes no revert, and calls the old PR already integrated.

Take a red you cannot get fixed to your officer, with the merge SHA and the gate.

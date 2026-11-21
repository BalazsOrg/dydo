<!-- built from the model; edit there: tools/SkillBuild -->

# Merging

1. Set your Merge sub-ticket In Progress, if your work was cut into steps: the merge is your own step, never dispatched, and you close it yourself.
2. Run the project's gates, as your brief names them, on the combined tree: your branch merged with the target's current tip.
3. Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.
4. Close the Merge sub-ticket once hosted CI is green on the merge commit.

- A clean merge gets no review.
- A conflict, or a gate that is really red, goes to an Implement ticket on your branch.
- Either repair comes back the way [A merge conflict](merge-conflict.md) lays out: its SHA goes on the Merge sub-ticket, a fresh reviewer judges what it changed, and you merge again from step 2, on the new head. A red you cannot get fixed goes to your officer, with the gate.

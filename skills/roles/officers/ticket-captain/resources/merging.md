<!-- built from the model; edit there: tools/SkillBuild -->

# Merging

1. Run the project's gates, as your brief names them, on the combined tree: your branch merged with the target's current tip.
2. Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.
3. Close the Merge sub-ticket once hosted CI is green on the merge commit.

- A clean merge gets no review.
- A conflict, or a gate that is really red, goes to an Implement ticket on your branch.

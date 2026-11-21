<!-- built from the model; edit there: tools/SkillBuild -->

# Merging

Merge only if your brief gives you the merge: where your officer merges, you open no PR, cut no Merge sub-ticket and take none of these steps.

**Before the review:**
Open the PR for your work as a draft once `dydo pr-ready` passes: run with no body file, it checks your tree alone, clean and pushed, and asks for no review. Any check of the PR's body waits for step 3.
Give its body the heading `## Independent review`: the round's holder writes its PASS under it.

**Once the rounds have ended:**

1. Set your Merge sub-ticket In Progress: the merge is your own step, never dispatched, and you close it yourself. Where your work was not cut into steps, your own ticket is the record instead.
2. Run the project's gates, as your brief names them, on the combined tree: your branch merged with the target's current tip.
3. Make the PR ready, no longer a draft, once its body holds the PASS for its head: `dydo merge` refuses a draft. Where your brief names `/pr`, that skill's check of the body comes here, on the body as the PR now holds it: `dydo pr-ready --body <file>`.
4. Merge with `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha>`: the reviewed head, and the tip you ran the gates against. When it refuses, its message says why.
5. Close the Merge sub-ticket once hosted CI is green on the merge commit.

- A clean merge gets no review.
- A conflict, or a gate that is really red, goes to an Implement ticket on your branch.
- Either repair comes back the way [A merge conflict](merge-conflict.md) lays out: a fresh reviewer judges what it changed, and you merge again from step 2, on the new head. A red you cannot get fixed goes to your officer, with the gate.
- A PASS that stands on a Review ticket alone merges nothing, since `dydo merge` reads only the PR's body: a head reviewed outside its PR needs a round in it.

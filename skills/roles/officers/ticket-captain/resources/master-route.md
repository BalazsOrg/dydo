<!-- built from the model; edit there: tools/SkillBuild -->

# A PR into master

## PR into master offered

Nominal path:

- One-way kinds: data migrations; publishing a package or a public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main (CI, branch protection, release workflows, the guard).

Exits:

- Declare the PR two-way in its body when it is none of the one-way kinds and no path flags it; there is no objection window.
- A one-way PR carries a recovery plan and waits on his click; declare it yourself when no path flags it.

## Re-checking against current master

Arrivals:

- On your turn, re-sync with current master and re-check.

Exits:

- Merge a two-way PR yourself with `dydo merge` once the re-check is green.

## One-way: watching its CI

Exits:

- Watch your own CI and open the HITL Merge only when it is green.

## Merged into master

Exits:

- When the survey wakes you on `PR merged, HITL Merge open`, or your AFK merge lands, close the Merge hop as for any merge.

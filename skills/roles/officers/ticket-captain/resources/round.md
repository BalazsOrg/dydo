<!-- built from the model; edit there: tools/SkillBuild -->

# A round's record

1. Post one verdict on your Review ticket: PASS or FAIL, the lenses, each seat's model and provider with its blockers, and the shoulds. FAIL means at least one blocker.
2. On a PASS of a candidate that is in a PR, the one your brief names, add its line at once to that PR's body, under `## Independent review`: `PASS - rubric/type: <code, docs or merge>; candidate <head SHA>; base <base SHA>`, then your Review ticket's key. A candidate in no PR, as at a leaf's evidence check, has its verdict on your Review ticket alone.
3. Record each nit as one line on your Review ticket.
4. File each should as a child of the ticket whose work you judged, the one your brief names, after checking the open shoulds there and above for a duplicate.

Group shoulds by judgement: minor ones may share a ticket, ones on the same code may stay together, and one that needs a lot of work gets its own.

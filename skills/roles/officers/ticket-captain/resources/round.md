<!-- built from the model; edit there: tools/SkillBuild -->

# A round's record

1. Post one verdict on the Review ticket: PASS or FAIL, the lenses, each seat's model and provider with its blockers, and the shoulds. FAIL means at least one blocker.
2. On a PASS, add its line at once to the PR body's `## Independent review`: `PASS - rubric/type: <code, docs or merge>; candidate <head SHA>; base <base SHA>`, then the Review ticket's key.
3. Record each nit as one line on the Review ticket.
4. File each should as a child of the ticket where it was found, after checking the open shoulds there and above for a duplicate.

Group shoulds by judgement: minor ones may share a ticket, ones on the same code may stay together, and one that needs a lot of work gets its own.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Fix

## Severity

`claim:Fix.Severity`: The fix-hop author checks each finding's severity against the source and fixes only blockers.

Spec: N12 DR 058 §3

**implementer** (Working, nominal):

> On a Fix, check each blocker's severity against the source, not the label; fix only the blockers, and put a label you dispute in your return with its evidence (file:line, test, output).

## Narrow

`claim:Fix.Narrow`: A fix hop prefers the change that deletes or narrows code; review never grows the PR.

Spec: N12 DR 058 §4

**implementer** (Working, nominal):

> On a Fix, prefer the change that deletes or narrows code; review never grows the change.

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainImplementBug

## Name

`claim:CaptainImplementBug.Name`: A Bug's writer names the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.

Spec: T3, T4

Evidence (code): `skills/roles/crew/implementer/resources/bug.md`

Layer: A Bug ticket reports a symptom in its reporter's words, wherever the reporter happened to meet it.

Reason: A defect pinned too low gets a patch where it showed, while its cause goes on failing somewhere else.

**ticket-captain** (ImplementBug; read by TicketCaptain):

> Name the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.

## Diagnose

`claim:CaptainImplementBug.Diagnose`: A Bug with no red reproduction yet is diagnosed with diagnosing-bugs.

Spec: K14, T3

Evidence (code): `skills/engineering/diagnosing-bugs/SKILL.md`

Layer: A brief names the skills its dispatcher foresaw, and only the writer finds out that the fault will not reproduce.

Reason: Guessing at a fault nobody can trigger produces a change that cannot be shown to cure it.

**ticket-captain** (ImplementBug; read by TicketCaptain, on claim:CaptainImplementBug.Reproduce):

> Reproduce it red before you fix it: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction. With none yet, load /diagnosing-bugs.

## Reproduce

`claim:CaptainImplementBug.Reproduce`: A Bug starts from a red reproduction: a test at the product's boundary when the defect shows there, else at its seam.

Spec: T3

Evidence (code): `skills/roles/crew/implementer/resources/bug.md`

Layer: A test runner reports a green run and cannot say whether the fault was ever caught.

Reason: A fix made before its failure was seen may be curing a different fault, or none.

**ticket-captain** (ImplementBug; read by TicketCaptain):

> Reproduce it red before you fix it: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction. With none yet, load /diagnosing-bugs.

## Unused

`claim:CaptainImplementBug.Unused`: An unused member needs no test: its definition and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.

Spec: T3, T11

Evidence (code): `skills/roles/crew/implementer/resources/bug.md`

Layer: No test can fail because of code that nothing calls.

Reason: Dead code kept alive by its own tests looks used to every search that follows.

**ticket-captain** (ImplementBug; read by TicketCaptain):

> An unused member needs no test: its definition at `file:line` and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.

## Revert

`claim:CaptainImplementBug.Revert`: After the fix, its writer reverts it, watches the reproduction fail, and restores it.

Spec: T3, R5

Evidence (code): `skills/roles/crew/implementer/resources/bug.md`

Layer: A reproduction that goes green proves the fix only if nothing else changed in between.

Reason: The red line shows the test holds this very fault, and gives the reviewer a receipt it need not run again.

**ticket-captain** (ImplementBug; read by TicketCaptain):

> After the fix, revert it, watch the reproduction fail, and restore it; return that red line, with the behaviour you kept still green.

## Race

`claim:CaptainImplementBug.Race`: A race is forced with fake timers or a set interleaving; a test that needs a sleep or a timeout is wrong.

Spec: T3, T1

Evidence (code): `skills/roles/crew/implementer/resources/bug.md`

Layer: A sleep makes a test pass on a quiet machine and fail on a loaded one, and the runner calls both results true.

Reason: A timing-dependent test flakes under a busy host, and a flake teaches everyone to ignore red.

**ticket-captain** (ImplementBug; read by TicketCaptain):

> Force a race with fake timers or a set interleaving: a test that needs a sleep or a timeout is wrong.

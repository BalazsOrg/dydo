---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Inquisition

## Order

`claim:Inquisition.Order`: An area's phases run Read, Break, Drive by default; every round starts from zero.

Spec: N5

**reviewer** (ReadPhase, nominal):

> In an inquisition round, run your phase on your area; by default an area goes Read, then Break, then Drive, and every round starts from zero.

## Read

`claim:Inquisition.Read`: Read looks for what is broken in ways neither tests nor clicking would catch.

Spec: N5

**reviewer** (ReadPhase, nominal):

> Read: look for what is broken in ways neither tests nor clicking around would catch.

## Break

`claim:Inquisition.Break`: Break forms hypotheses of failure and writes tests that confirm or refute them.

Spec: N5

**reviewer** (BreakPhase, nominal):

> Break: form hypotheses about how the area fails, then write tests that confirm or refute them.

## Drive

`claim:Inquisition.Drive`: Drive runs the product against the spec's problem statement.

Spec: N5, R3

**reviewer** (DrivePhase, nominal):

> Drive: run every feature and edge case through the running app, against the spec's problem statement.

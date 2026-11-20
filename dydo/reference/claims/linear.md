---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Linear

## Purpose

`claim:Linear.Purpose`: The Linear workspace standard is built from the same claims as the role skills.

Spec: K12

**linear-workspace-standard** (LinearIntro):

> The Linear vocabulary for Projects and tickets; every rule here is a claim, built from the same model as the role skills.

## Statuses

`claim:Linear.Statuses`: Seven statuses; the old three are deleted at ship and Sub-issue auto-close stays off.

Spec: S5, N9, S18

**linear-workspace-standard** (LinearSetup):

> Seven statuses only: FutureFeature, Backlog, Todo, In Progress, Done, Canceled, Duplicate. The Enablement at ship deletes `Implementing`, `In Review` and `Ready to Merge`, and Sub-issue auto-close stays off.

## ContinuousPair

`claim:Linear.ContinuousPair`: Every project owns a continuous pair, CI findings and Agent friction, that nobody oversees.

Spec: H8

**linear-workspace-standard** (LinearSetup):

> Each project (team) gets its continuous pair, `CI findings` and `Agent friction`: a Linear grouping of tickets, not a Project with an outcome; nobody oversees it or sends envoys for it.

## Mode

`claim:Linear.Mode`: The Mode label says who the work waits on.

Spec: S6

**linear-workspace-standard** (LinearLabels):

> The Mode label, AFK or HITL, says who the work waits on.

## CutRule

`claim:Linear.CutRule`: The cut rule: a ticket only when someone other than the asker uses what the work leaves.

Spec: S1

**coordinator** (Watching, nominal):

> Cut a ticket only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.

**ticket-captain** (Cut, nominal):

> Cut a ticket only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.

**linear-workspace-standard** (LinearTickets):

> A ticket exists only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.

## Priority

`claim:Linear.Priority`: Priority marks which of his tickets he takes next.

Spec: N12 DR 047 §11

**coordinator** (Watching, nominal):

> Set Priority on every ticket waiting on him, a Question or a HITL ticket, and re-set it as blockers change: High when clearing it lets a top-level ticket or the Project's next step run AFK, Medium when it frees a lane, Low when nothing runs AFK after it; Urgent only in an emergency; AFK tickets carry none.

**linear-workspace-standard** (LinearPriority):

> Priority marks which of his tickets he takes next, so the one unlocking the most AFK work goes first: High when clearing it lets a top-level ticket or the Project's next step run AFK, Medium when it frees a lane, Low when nothing runs AFK after it; Urgent is an emergency, never a default; AFK tickets carry none.

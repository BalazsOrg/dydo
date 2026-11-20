---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Liveness

## Quiet

`claim:Liveness.Quiet`: liveness fires on a hop In Progress quiet for 2 hours, or a ticket In Progress with no open hop for 2 hours.

Spec: S24, S23

Transition in Liveness (`claims/statechart/liveness/Liveness.tla`): Live → Quiet, "2 hours with no change and no liveness comment".

**linear-workspace-standard** (LinearStatuses):

> `liveness` fires on a hop In Progress and quiet for 2 hours, or a ticket In Progress with no open hop for 2 hours, counted from the later of its last change and its latest `liveness <UTC time>: <outcome>` comment.

## FoundAlive

`claim:Liveness.FoundAlive`: An alive finding snoozes for one window.

Spec: S24, H12

Transition in Liveness (`claims/statechart/liveness/Liveness.tla`): Quiet → Live, "alive: the finding snoozes one window".

**ticket-captain** (Quiet, exit):

> A liveness finding on a hop that is alive: comment `liveness <UTC time>: alive`, and the finding snoozes one window.

**coordinator** (Quiet, exit):

> A liveness finding on a hop that is alive: comment `liveness <UTC time>: alive`, and the finding snoozes one window.

**chief-of-staff** (Quiet, exit):

> A liveness finding on a hop that is alive: comment `liveness <UTC time>: alive`, and the finding snoozes one window.

## FoundBehind

`claim:Liveness.FoundBehind`: A record behind is brought up to date.

Spec: S24, H12

Transition in Liveness (`claims/statechart/liveness/Liveness.tla`): Quiet → Live, "the record is behind: brought up to date".

**ticket-captain** (Quiet, exit):

> A liveness finding on a hop whose record is behind: comment `liveness <UTC time>: record behind` and bring the record up to date (Done, Fix rN, …).

**coordinator** (Quiet, exit):

> A liveness finding on a hop whose record is behind: comment `liveness <UTC time>: record behind` and bring the record up to date (Done, Fix rN, …).

**chief-of-staff** (Quiet, exit):

> A liveness finding on a hop whose record is behind: comment `liveness <UTC time>: record behind` and bring the record up to date (Done, Fix rN, …).

## FoundStopped

`claim:Liveness.FoundStopped`: A stopped hop goes back to Todo and is resumed or restarted.

Spec: S24, H12

Transition in Liveness (`claims/statechart/liveness/Liveness.tla`): Quiet → Restarted, "stopped: the hop back to Todo".

**ticket-captain** (Quiet, exit):

> A liveness finding on a stopped hop: comment `liveness <UTC time>: stopped`, set the hop back to Todo, then resume the thread or dispatch a fresh agent; the old thread link stays as history.

**coordinator** (Quiet, exit):

> A liveness finding on a stopped hop: comment `liveness <UTC time>: stopped` and set the hop back to Todo; resume the same agent's thread the first time, and dispatch a fresh agent when the same hop stops again; the old thread link stays as history.

**chief-of-staff** (Quiet, exit):

> A liveness finding on a stopped hop: comment `liveness <UTC time>: stopped`, set the hop back to Todo, then resume the thread or dispatch a fresh agent; the old thread link stays as history.

## Resumed

`claim:Liveness.Resumed`: The resumed or fresh agent attaches its thread link and works.

Spec: S24, S13

Transition in Liveness (`claims/statechart/liveness/Liveness.tla`): Restarted → Live, "the resumed or fresh agent attaches its thread link". A system transition: no skill carries it.

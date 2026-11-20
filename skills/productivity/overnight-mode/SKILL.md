---
name: overnight-mode
description: Overnight mode, the user away while the chief of staff keeps the night watch. Load only on the chief of staff's muster or morning-brief announcement, or when the user calls overnight mode.
---

# Overnight Mode

**Overnight mode** is any long absence of the user. It belongs to the chief of staff alone. At
**muster**, the chief of staff verifies `overnight: on` with `dydo mode show` before announcing the
start; at the **morning brief**, it verifies `overnight: off` before announcing the end. In between,
it keeps the **night watch**. Intent, a watch schedule and message delivery do not establish a mode
transition.

Coordinators do not load this skill: each behaves the same day and night. A stop line, when the
user wants one, goes in the Project plan its coordinator already reads.

## Muster

The already-invoked chief of staff runs these steps; the transition preflight is this skill's own
resource.

1. While the user is present, complete the [transition preflight](resources/transition-preflight.md)
   for both startup and shutdown. Record the current mode and prepare the warning/retry and
   independent readback steps before proceeding.
2. Hold the muster by the chief of staff's checklist, `chief-of-staff/resources/muster.md`. Use
   `grilling` for decisions and `wizard` for terminal steps only. Proceed once every included coordinator confirms
   its agreed work is runnable and every gap has the user's explicit disposition.
3. Follow the preflight's warning/retry for `dydo mode set overnight on`. Wait for the setter to
   return its result, then run `dydo mode show` in a separate later tool call, never in parallel,
   and require `overnight: on`. On refusal, failure or missing readback, report activation incomplete
   with the observed state (unknown if unreadable) and exact
   recovery action; keep the user present and do not announce activation or say goodnight.
4. Create the 30-minute watch schedule, returning to this thread. Preserve the complete schedule
   ID exactly as returned by the host, including any prefix, for deletion at shutdown.
   If scheduling fails, report mode on with watch unavailable and resolve it while the user is present.
5. Announce the verified start to every included coordinator.

Done when readiness is confirmed, the schedule ID is preserved, mode is on and every included
coordinator has the announcement.

## Night watch

The chief of staff runs its overnight watch every 30 minutes, surveying by
`chief-of-staff/resources/night-survey.md`, and acts on what the watch finds:

1. Idle, or waiting with nothing in flight and runnable work: send one specific nudge naming the evidence.
2. Nudge unanswered: inspect the relevant captain threads and intervene with the owner.

The board and threads carry state; never ask for routine STATE. Mediate cross-Project trouble,
relaying heads-ups down the chain. After any gap in the watch, rebuild from Linear and sweep the
board.

## Morning brief

When the user says he's back, in the chief of staff's conversation:

1. Complete the [transition preflight](resources/transition-preflight.md) for shutdown while the user
   is present; record the current mode before changing it.
2. Delete the watch schedule using the complete preserved ID. List schedules and confirm that exact
   ID is absent before claiming the watch stopped; report a deletion or verification gap.
3. Settle any pending survey through its host lifecycle so no tick remains in flight.
4. Follow the preflight's warning/retry for `dydo mode set overnight off`. Wait for the setter to
   return its result, then run `dydo mode show` in a separate later tool call, never in parallel,
   and require `overnight: off`, then announce the end to every included coordinator.
   On refusal, failure or missing readback, report shutdown incomplete with the
   observed state (unknown if unreadable) and exact recovery action. A deleted schedule does not
   prove mode is off.
5. Write the morning brief in the conversation from the template in
   `chief-of-staff/resources/morning-brief.md`, writing nothing to the repo. Use `wizard` for terminal unblockers only.

Done when the schedule is confirmed absent, pending surveys are settled, mode is off, coordinators
have the announcement and the user has the brief.

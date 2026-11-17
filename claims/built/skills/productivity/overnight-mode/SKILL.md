---
name: overnight-mode
description: Overnight mode, the user away while admirals run on standing and night orders. Load only on the chief of staff's muster or morning-brief announcement, or when the user calls overnight mode.
---
<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Overnight Mode

## decision

- Other calls: `what-would-balazs-do`, whose trigger map decides it or prepares the Question; irreversible calls are always a Question.
- Easily reversible: decide and record `DECIDED`.

## linear

- Withdraw only Questions made obsolete by discovered facts or completed work; keep tribunal-decided Questions awaiting ratification.
- **Write for a reader who wasn't there**, every Question and `DECIDED`.

## merge

- **Merge into the feature branch** through your captain's Merge chain, which runs `dydo merge`; never into master or main.

## overnight

- Proceed once every included admiral confirms its agreed work is runnable and every gap has the user's explicit disposition.
- Send the chief of staff no status.
- The board and threads carry state; never ask for routine STATE.
- Announce the verified start to every included admiral.
- Answer a nudge in one line.
- In between, **each admiral keeps the conn** of its Project and the chief of staff keeps only the **night watch**.
- Contact it for cross-Project trouble: collisions, shared-resource problems, and heads-ups or permission before actions that affect other agents.
- Delete the watch schedule using the complete preserved ID.
- List schedules and confirm that exact ID is absent before claiming the watch stopped; report a deletion or verification gap.
- On refusal, failure or missing readback, report activation incomplete with the observed state (unknown if unreadable) and exact recovery action; keep the user present and do not announce activation or say goodnight.
- On refusal, failure or missing readback, report shutdown incomplete with the observed state (unknown if unreadable) and exact recovery action.
- Fill `chief-of-staff`'s `resources/morning-brief.md` in the conversation, writing nothing to the repo.
- Follow the preflight's warning/retry for `dydo mode set overnight on`.
- Follow the preflight's warning/retry for `dydo mode set overnight off`.
- Load only on the chief of staff's muster or morning-brief announcement, or when the user calls overnight mode.
- Mediate cross-Project trouble by chief-of-staff Method step 4, Mediate a collision, relaying heads-ups down the chain.
- Complete `chief-of-staff`'s `resources/muster.md`.
- Use `grilling` for decisions and `wizard` for terminal steps only.
- Use `wizard` for terminal unblockers only.
- Night orders expire at the morning brief and win where the two conflict.
- Idle, or waiting with nothing in flight and runnable work: send one specific nudge naming the evidence.
- Admirals obey the **standing orders** below plus the **night orders**: one Project update per Project, set at muster.
- Intent, a watch schedule and message delivery do not establish a mode transition.
- A deleted schedule does not prove mode is off.
- Project decisions are the admiral's.
- Wait for the setter to return its result, then run `dydo mode show` in a separate later tool call, never in parallel, and require `overnight: on`.
- Wait for the setter to return its result, then run `dydo mode show` in a separate later tool call, never in parallel, and require `overnight: off`, then announce the end to every included admiral.
- After any gap in the watch, rebuild from Linear and sweep everyone by Method step 5, Sweep the board.
- It relays down admiral → captain → crew.
- Settle any pending survey through its host lifecycle so no tick remains in flight.
- Done when the schedule is confirmed absent, pending surveys are settled, mode is off, admirals have the announcement and the user has the brief.
- When the user says he's back, in the chief of staff's conversation:
- The walking skeleton does not wait either: its Walkthrough Ticket blocks nothing; post `DECIDED` on it, carry on, and treat a wrong direction as a revert.
- The already-invoked chief of staff uses its own resources below.
- Resolve each resource inside the installed `chief-of-staff` skill folder; read it directly without invoking another officer role.
- Done when readiness is confirmed, the schedule ID is preserved, mode is on and every included admiral has the announcement.
- One blocker never stops the night: take the next pickable work.
- Stop only at the night orders' stop line or when nothing is pickable.
- The surveyor inspects; the chief of staff acts on its findings:
- While the user is present, complete the [transition preflight](resources/transition-preflight.md) for both startup and shutdown.
- Record the current mode and prepare the warning/retry and independent readback steps before proceeding.
- Complete the [transition preflight](resources/transition-preflight.md) for shutdown while the user is present; record the current mode before changing it.
- Nudge unanswered: inspect the relevant captain threads and intervene with the owner.
- At **muster**, the chief of staff verifies `overnight: on` with `dydo mode show` before announcing the start; at the **morning brief**, it verifies `overnight: off` before announcing the end.
- Create the 30-minute watch schedule, returning to this thread.
- If scheduling fails, report mode on with watch unavailable and resolve it while the user is present.
- Preserve the complete schedule ID exactly as returned by the host, including any prefix, for deletion at shutdown.
- The chief of staff runs its Overnight duties watch step every 30 minutes, using its `resources/night-survey.md`.

## Resources

- [Transition preflight](resources/transition-preflight.md)

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: StaffMuster

## Ready

`claim:StaffMuster.Ready`: The muster is a checklist held with him present, after overnight-mode's transition preflight; the night starts once it is done.

Spec: K11, H7, K22

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: the chief of staff read `overnight: off` with `dydo mode show` at 02:03 UTC (position 6), before it sent a request, and its report to him at 02:05 says the mode is still off (position 44). `skills/productivity/overnight-mode/SKILL.md` on this branch, hand-written: "Hold the muster by the chief of staff's checklist, `chief-of-staff/resources/muster.md`."

Layer: The guard answers the command that switches the mode with a reminder and passes its retry; it never checks that a muster was held.

Reason: Once he is asleep nobody can unblock a coordinator, so whatever wants his answer is settled while he can still give one.

**chief-of-staff** (Muster; read by ChiefOfStaff):

> Hold this checklist with him present, after `overnight-mode`'s transition preflight; the night starts only once every line is done.

## Ask

`claim:StaffMuster.Ask`: Each live coordinator returns a muster report to a fixed request.

Spec: K11, H7, K22

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: four `t3_thread_send` calls, one to each coordinator's thread, came back `delivery: started` between 02:03:44 and 02:03:58 UTC (positions 24 to 27), and the four reports arrived in the chief of staff's thread between 02:04:10 and 02:05:10 (positions 31, 34, 41 and 42).

Layer: T3 delivers a message to a thread and shapes no answer; what comes back follows what was asked.

Reason: Four answers in one shape can be laid side by side, and a part nobody asked for is a part nobody sends.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> Ask each live coordinator for its muster report, with the request below.

## Unblock

`claim:StaffMuster.Unblock`: Operational blockers are resolved while he is present, each fix verified with evidence.

Spec: K11, K22

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: the DYTEST Network coordinator named its blocker, a fix and the check that would show it fixed, "Verify: one windows-proof push succeeds" (position 41). The chief of staff passed the fix on to him unchecked at 02:05 (position 44), and read the org's billing only at 02:17, after he doubted it: one `gh api` call, while he was still there (position 52).

Layer: A coordinator's report names a fix; nothing between its thread and his reply tests one.

Reason: A repair that only looks done fails in the small hours, when the one person who could mend it is asleep.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> Resolve each operational blocker while he is present, verifying each fix with evidence.

## Permissions

`claim:StaffMuster.Permissions`: Each permission the night needs is exercised safely and its real result recorded; no dummy merge.

Spec: K11, K22

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: the four sends to the coordinators came back `delivery: started` (positions 24 to 27), and the chief of staff's Linear write returned its update's id (position 55). The LC coordinator's report named a write that only its exercise showed missing: "My `save_project` edit was refused by the classifier" (position 31). The dydo coordinator gave its merges by their last real use, "last exercised with PR 376 → 6dc834f0 on 2026-11-20" (position 34), and no merge was run for the muster. GitHub, read 2026-11-21 with `gh api repos/BalazsOrg/dydo/branches/feature%2Fdydo-4-0-0`: `protected: false`.

Layer: GitHub takes a push to an unprotected branch from any writer, and a host classifier can refuse a call the service would accept; neither says what he has granted.

Reason: A grant found missing at three in the morning stops a lane until he wakes; found at muster, he gives it in a minute.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> Exercise each permission the night's work needs through a safe, authorised operation where one exists, such as contacting the coordinator, each session's Linear writes and feature-branch merges, and record the real result; an unprotected branch proves no merge permission, and a dummy merge is never run.

## Gaps

`claim:StaffMuster.Gaps`: An exercise that can't run or an item left unresolved is a gap with his explicit disposition.

Spec: K11, K22

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: the DYTEST Network coordinator listed what it could not exercise, "Unsupported gaps (by design, Balazs only): billing; merges; re-pins (the wizard); CI activation" (position 41). He disposed of the work that hung on the first, "I would hold the windows checks until the very end" (position 50), and that night a DYTEST PR counted as ready once reviewed and proven on Linux (position 55).

Layer: Nothing lists what was skipped: a check that never ran leaves no trace on a board or in a thread.

Reason: Work that leans on something unproven is the first to stall overnight, so he says beforehand whether it runs, waits or is dropped.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> Keep an exercise you can't run, or an item left unresolved, as a gap, and get his explicit disposition for the work that depends on it.

## StopLine

`claim:StaffMuster.StopLine`: A stop line he wants goes in the Project plan the coordinator reads.

Spec: K11

Evidence (run): `skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked: "Read the Project from Linear, its plan included" and "when he wants a stop line, it is in the Project plan, and you stop there". `skills/productivity/overnight-mode/SKILL.md` on this branch, hand-written: "A stop line, when the user wants one, goes in the Project plan its coordinator already reads." The muster of 2026-11-21 ran the installed skills, which put each stop line in a Project update instead (T3 thread d4c81ce0, position 55).

Layer: Linear keeps a plan and a Project update side by side and marks neither as the one that binds tonight.

Reason: A limit written where its reader already looks is met on every tick, with no second document to find, trust and expire.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> A stop line he wants goes in the Project plan, which its coordinator reads.

## NoNightOrders

`claim:StaffMuster.NoNightOrders`: The night has no orders of its own: there are no night orders (DYD-1195 33520d6c).

Spec: K11

Evidence: open, so no skill states this claim yet. On 2026-11-21 he gave one himself at muster, "one night order: we're short on openai usage so when using models from that family use 6.1 sol on low. This order is only for the night" (T3 thread d4c81ce0, position 50), and the chief of staff posted night orders on each Project as a Project update (positions 55 and 56), which spec H16 now cites as "night orders 9099daa1". The skills on this branch have none. No muster has been held without them, and whether one can be is his call.

Layer: Linear takes a Project update from any agent; nothing marks one as an instruction or ends it in the morning.

Reason: Two places for one evening's instructions let a coordinator follow the stale one.

**chief-of-staff** (withheld; MusterChecklist; read by ChiefOfStaff):

> There are no night orders.

## Runnable

`claim:StaffMuster.Runnable`: Each coordinator confirms its next work is runnable before the night starts.

Spec: K11, K22

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: each of the four coordinators sent back that its work was runnable, between 02:19:39 and 02:20:06 UTC (positions 64, 68, 71 and 73).

Layer: A board shows a ticket as ready; whether its holder can begin tonight, only that holder knows.

Reason: A lane that cannot begin is found while he is there, not at the first watch tick half an hour later.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> Get each coordinator's confirmation that its next work is runnable.

## List

`claim:StaffMuster.List`: He gets each coordinator's thread link and its readiness, never a report's content.

Spec: K11, H7, H6

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: each coordinator's report arrived whole in the chief of staff's own thread, where he was reading (positions 31, 34, 41 and 42). The chief of staff then retold all four (position 44), and the retelling lost part of one: the dydo coordinator's question listed options (a), (b) and (c), and he was shown (a) alone.

Layer: A message sent to a thread stays there, whole; a retold one is a second copy that nothing keeps equal to the first.

Reason: He corrects the original, where its author will see the correction, instead of a summary that may have dropped something.

**chief-of-staff** (MusterChecklist; read by ChiefOfStaff):

> List for him each coordinator's thread link and its readiness in the form below, never a report's content; then the muster is ready.

## Asked

`claim:StaffMuster.Asked`: The muster request a coordinator answers.

Spec: K11, K22, H5

Evidence (run): T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: the request sent that night asked for Intent, Work, Questions, Blockers and Permissions (positions 24 to 27), and each of the four reports answered every part (positions 31, 34, 41 and 42). This one keeps four of those parts and has no Questions part; nobody has sent it yet. `skills/productivity/in-your-own-words/SKILL.md` loads on "the user's muster request relayed by the chief of staff".

Layer: No tool gives a report its parts: each Project would otherwise describe itself in headings of its own.

Reason: The same four headings from every Project let him compare them, and a missing one shows at a glance.

**chief-of-staff** (MusterRequest; read by ChiefOfStaff):

> ```text
> His muster request for <date>, <Project>. Return:
> Intent: his goals and purpose for the Project in your own words, by /in-your-own-words, apart from the work.
> Work: your next tickets in fill order, the first runnable one, and the plan's stop line, or none.
> Blockers: each operational obstacle, and the evidence that would show it fixed.
> Permissions: each action the night's work needs, with a safe exercise for it, or the gap.
> ```

## Form

`claim:StaffMuster.Form`: The line each Project gets on his muster list: its coordinator's thread link and readiness.

Spec: K11, H7

Evidence (tool): t3_thread_read, called 2026-11-21 on thread d4c81ce0-f13a-4cea-80bd-6911b05d288f: the result's `thread.link` is a Markdown link to the thread, `[Overnight Muster and Admiral Readiness](t3-thread://v2/f7f7b47f-c6f2-4990-a113-851014c04ff0/d4c81ce0-f13a-4cea-80bd-6911b05d288f)`.

Layer: T3 gives every thread a link and lists over two thousand of them; which are tonight's coordinators, no listing says.

Reason: One row for each shows at a glance who is not prepared, with the click that takes him there.

**chief-of-staff** (MusterList; read by ChiefOfStaff):

> ```text
> <Project> — <coordinator thread link> — <ready, or not ready>
> ```

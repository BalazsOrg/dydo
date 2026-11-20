---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Staff

## Ask

`claim:Staff.Ask`: He may ask the chief of staff for cross-Project thinking or about an unknown on his machine.

Spec: H7

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): Day → Investigating, "he asks for cross-Project thinking, or about an unknown on his machine".

**chief-of-staff** (Investigating, arrival):

> He asks for big cross-Project thinking, or about an unknown on his machine: find it out.

## Answered

`claim:Staff.Answered`: The chief of staff gives him what it found.

Spec: H7

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): Investigating → Day, "findings given to him".

**chief-of-staff** (Investigating, exit):

> Give him what you found, then go back to the short list.

## Muster

`claim:Staff.Muster`: The night starts with his muster; overnight mode is the chief of staff's alone (DYD-1082 555a8300 gap 6, DYD-1195 33520d6c).

Spec: H7, K11

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): Day → Muster, "he musters: the night starts".

**chief-of-staff** (Muster, arrival):

> His muster starts the night: load `overnight-mode` and hold the muster with him; overnight mode is yours alone, and the coordinators run as they do by day.

## Watch

`claim:Staff.Watch`: At muster the chief of staff lists every open dr-grade call as a thread link; then the overnight watch runs.

Spec: L21, H7

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): Muster → NightWatch, "the DR-grade calls listed; the watch starts".

**chief-of-staff** (Muster, exit):

> At muster, list for him every open ticket labelled `dr-grade` as a thread link, then start the overnight watch.

## Morning

`claim:Staff.Morning`: The night ends on his return, and the morning brief closes it (DYD-1082 555a8300 gap 7).

Spec: H7, L19

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): NightWatch → MorningBrief, "he returns: the night ends".

**chief-of-staff** (NightWatch, exit):

> When he returns, the night ends: give him the morning brief.

## Brief

`claim:Staff.Brief`: The morning brief lists overnight proposed DRs for ratification, never decided Questions.

Spec: L19, L21

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): MorningBrief → Day, "the morning brief closes the night".

**chief-of-staff** (MorningBrief, exit):

> List each overnight `proposed` DR for his ratification; a decided Question is never listed for approval.

## ShortList

`claim:Staff.ShortList`: The chief of staff keeps one short list for him from dydo survey board across Projects.

Spec: H7

**chief-of-staff** (Day, nominal):

> Keep his one short list from `dydo survey board` across Projects: HITL threads waiting on him, HITL Merges waiting on his click, and anything waiting too long, each as a thread link.

## Continuous

`claim:Staff.Continuous`: It lists continuous tickets with their age and gives one out when he says so.

Spec: H7, H8

**chief-of-staff** (Day, nominal):

> List the continuous tickets with their age; when he tells you to, give one to an officer with an open lane, and leave anything outside the pull rule on the list for him.

## NoEnvoy

`claim:Staff.NoEnvoy`: It catches an open HITL ticket with no envoy.

Spec: H7

**chief-of-staff** (Day, nominal):

> Catch each open HITL ticket that has no envoy.

## LinksOnly

`claim:Staff.LinksOnly`: It never relays content and sends no envoys.

Spec: H7

**chief-of-staff** (Day, nominal):

> Pass on thread links, never their content, and send no envoys: an escalation that needs him goes through its coordinator's own envoy.

## MusterReady

`claim:Staff.MusterReady`: The muster is a checklist held with him present, after overnight-mode's transition preflight; the night starts once it is done.

Spec: K11, H7, K22

**chief-of-staff** (Muster):

> Hold this checklist with him present, after `overnight-mode`'s transition preflight; the night starts only once every line is done.

## MusterAsk

`claim:Staff.MusterAsk`: Each live coordinator returns a muster report to a fixed request.

Spec: K11, H7, K22

**chief-of-staff** (Muster):

> Ask each live coordinator for its muster report, with the request below.

## MusterShow

`claim:Staff.MusterShow`: He sees each report, its intent read-back apart from the work, and his corrections go back to its coordinator.

Spec: K11, H7, K22

**chief-of-staff** (Muster):

> Show him each report verbatim in the form below, the intent read-back apart from the work, and send each coordinator his corrections to its read-back.

## MusterUnblock

`claim:Staff.MusterUnblock`: Operational blockers are resolved while he is present, each fix verified with evidence.

Spec: K11, K22

**chief-of-staff** (Muster):

> Resolve each operational blocker while he is present, verifying each fix with evidence.

## MusterPermissions

`claim:Staff.MusterPermissions`: Each permission the night needs is exercised safely and its real result recorded; no dummy merge.

Spec: K11, K22

**chief-of-staff** (Muster):

> Exercise each permission the night's work needs through a safe, authorised operation where one exists, such as contacting the coordinator, each session's Linear writes and feature-branch merges, and record the real result; an unprotected branch proves no merge permission, and a dummy merge is never run.

## MusterGaps

`claim:Staff.MusterGaps`: An exercise that can't run or an item left unresolved is a gap with his explicit disposition.

Spec: K11, K22

**chief-of-staff** (Muster):

> Keep an exercise you can't run, or an item left unresolved, as a gap, and get his explicit disposition for the work that depends on it.

## MusterStopLine

`claim:Staff.MusterStopLine`: A stop line he wants goes in the Project plan the coordinator reads; there are no night orders (DYD-1195 33520d6c).

Spec: K11

**chief-of-staff** (Muster):

> A stop line he wants goes in the Project plan, which its coordinator reads; there are no night orders.

## MusterRunnable

`claim:Staff.MusterRunnable`: Each coordinator confirms its next work is runnable before the night starts.

Spec: K11, K22

**chief-of-staff** (Muster):

> Get each coordinator's confirmation that its next work is runnable; then the muster is ready.

## MusterRequest

`claim:Staff.MusterRequest`: The muster request a coordinator answers.

Spec: K11, K22, H5

**chief-of-staff** (MusterRequest):

> ```text
> His muster request for <date>, <Project>. Return:
> Intent: his goals and purpose for the Project in your own words, by /in-your-own-words, apart from the work.
> Work: your next tickets in fill order, the first runnable one, and the plan's stop line, or none.
> Blockers: each operational obstacle, and the evidence that would show it fixed.
> Permissions: each action the night's work needs, with a safe exercise for it, or the gap.
> ```

## MusterForm

`claim:Staff.MusterForm`: The form each Project's report is shown to him in.

Spec: K11, K22

**chief-of-staff** (MusterPresent):

> ```text
> <Project> — <coordinator thread link>
> Intent, as the coordinator reads it: <verbatim read-back>
> Work: <next tickets, the first runnable one>
> Stop line: <the plan's, or none>
> Unblock now: <obstacle; fix; evidence it holds>
> Permissions: <action; safe exercise; result, or the gap>
> ```

## SurveyRead

`claim:Staff.SurveyRead`: Each watch tick reads the board across the Projects against the last observation, keeping no shadow board.

Spec: H7, K22, S26

**chief-of-staff** (NightSurvey):

> Each watch tick, read `dydo survey board` across the Projects and compare it with your last observation; keep no shadow board.

## SurveyThreads

`claim:Staff.SurveyThreads`: Where progress is unclear, the coordinator's thread is read, a captain's only as needed.

Spec: H7, K22

**chief-of-staff** (NightSurvey):

> Where progress is unclear, read the coordinator's thread, and a captain's only as needed, and check every nudge still unanswered.

## SurveyFinds

`claim:Staff.SurveyFinds`: The survey looks for idle officers with runnable work, unanswered nudges, blockers and colliding work.

Spec: H7, H12, K22

**chief-of-staff** (NightSurvey):

> Look for an idle officer with runnable work, an unanswered nudge, a blocker that needs intervention and colliding work; an unchanged board or a quiet running task alone is no stall.

## SurveyComplete

`claim:Staff.SurveyComplete`: A tick is quiet only once every Project and nudge is checked; an unreadable source leaves it incomplete.

Spec: H7, K22

**chief-of-staff** (NightSurvey):

> A tick ends quiet only once every Project and every outstanding nudge is checked; a source you could not read leaves it incomplete, to resolve, never quiet.

## MorningFill

`claim:Staff.MorningFill`: The morning brief is filled in the conversation after the verified shutdown, writing nothing to the repo.

Spec: H7, K11, K22

**chief-of-staff** (MorningBrief):

> Fill this in the conversation once `overnight-mode` has verified the shutdown, writing nothing to the repo.

## MorningTemplate

`claim:Staff.MorningTemplate`: The morning brief's form: the night, PRs awaiting his click, proposed DRs to ratify, what waits on him, and next.

Spec: H7, L19, L21, K22

**chief-of-staff** (MorningBrief):

> ```text
> ## The night
> <one real visual of the work touched>
> - <result; at most three result lines>
>
> ## PRs into master awaiting your click
> - <title> (<PR link>): <result>. Review <verdict>; CI <state on the current head>.
>
> ## Proposed DRs to ratify
> - <title> (<thread link>): <what it decides>. Ratify or decline.
>
> ## Waiting on you
> - <each HITL thread waiting on him, as a thread link>
>
> ## Next
> 1. <next action and its owner>
> ```

## DoubleCheck

`claim:Staff.DoubleCheck`: Overnight, its 12-hour check on a quiet Project is a cheap double check only.

Spec: H7, H12

**chief-of-staff** (NightWatch, nominal):

> Overnight, your 12-hour check on a quiet Project is only a cheap double check behind the coordinators' own scheduled surveys.

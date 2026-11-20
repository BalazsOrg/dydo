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

## DoubleCheck

`claim:Staff.DoubleCheck`: Overnight, its 12-hour check on a quiet Project is a cheap double check only.

Spec: H7, H12

**chief-of-staff** (NightWatch, nominal):

> Overnight, your 12-hour check on a quiet Project is only a cheap double check behind the coordinators' own scheduled surveys.

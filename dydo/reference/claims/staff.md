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

`claim:Staff.Muster`: Overnight mode, the chief of staff's and the coordinator's, starts at muster.

Spec: K11, L21

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): Day → Muster, "overnight mode starts".

**chief-of-staff** (Muster, arrival):

> Overnight mode starts at muster, which you hold.

## Watch

`claim:Staff.Watch`: DR-grade calls surface at muster; then the overnight watch runs.

Spec: L21, H7

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): Muster → NightWatch, "DR-grade calls surfaced; the watch starts".

**chief-of-staff** (Muster, exit):

> Surface every pending DR-grade call to him at muster, then start the overnight watch.

## Morning

`claim:Staff.Morning`: The night ends in the morning brief.

Spec: H7, L19, K11

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): NightWatch → MorningBrief, "the night ends".

**chief-of-staff** (NightWatch, exit):

> When the night ends, write the morning brief.

## Brief

`claim:Staff.Brief`: The morning brief lists overnight proposed DRs for ratification, never decided Questions.

Spec: L19, L21

Transition in The chief of staff (`claims/statechart/staff/Staff.tla`): MorningBrief → Day, "the brief is given".

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

---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Recovery

## ReCut

`claim:Recovery.ReCut`: A re-cut saves the returned work, resumes in the children, and ends staffed or with the ticket sent up as too big.

Spec: K9, H2, H16

**ticket-captain** (ReCut):

> Save the returned hop's SHA on it and resume each child from there; end resolved once the children are staffed, or escalated by sending a ticket too big for you to your officer as `too big, re-cut`.

**coordinator** (ReCut):

> Save the `too big, re-cut` on the captain's ticket and resume from the children its Wayfinding captain files; end resolved once they are staffed, or escalated as a `research` Question when its captain passes a need up.

## FixRounds

`claim:Recovery.FixRounds`: A fix round saves the verdict's head, resumes the Fix from it, and ends on a clean round or the checkpoint's escalation.

Spec: K9, R8, R10

**ticket-captain** (FixRounds):

> Save the verdict's head SHA on the Review hop and resume Fix rN from it; end resolved when a round finds no new blocker, or escalated only from the checkpoint, written `escalated: …` on the Review.

## MergeConflict

`claim:Recovery.MergeConflict`: A conflict saves the resolution's commit, resumes the merge from it, and ends merged or escalated.

Spec: K9, R19

**ticket-captain** (MergeConflict):

> Save the resolution's commit SHA on the Merge hop and resume the merge from it; end resolved once the hunks pass and the merge runs green, or escalated to your officer with what you can't resolve.

## RedGate

`claim:Recovery.RedGate`: A red gate saves the merge SHA and the gate, resumes from them, and ends green or escalated.

Spec: K9, R22

**ticket-captain** (RedGate):

> Save the merge SHA and the red gate on the Merge hop and resume from them; end resolved once the rerun is green or the re-land merges green, or escalated to your officer with what you can't resolve.

## ForcedResync

`claim:Recovery.ForcedResync`: A held PR saves its head and the hold, resumes on its turn, and ends merged or escalated.

Spec: K9, R29, H16

**ticket-captain** (ForcedResync):

> Save the held PR's head SHA on the Merge hop and resume from it on your next turn; end resolved once the PR merges, or escalated to your officer with what you can't resolve.

**coordinator** (ForcedResync):

> Save the hold on the held PR's ticket and resume pacing once the colliding work is sequenced; end resolved once the PR merges, or escalated as a `research` Question when its captain passes a need up.

## MasterRevert

`claim:Recovery.MasterRevert`: A reverted master merge saves its SHA and the gate, resumes the fix, and ends merged green or escalated.

Spec: K9, R22

**ticket-captain** (MasterRevert):

> Save the reverted merge's SHA and its red gate on the Merge hop and resume the fix from them; end resolved once the PR, offered again, merges green, or escalated to your officer with what you can't resolve.

## NeedForBalazs

`claim:Recovery.NeedForBalazs`: A need saves the branch and the need on the ticket, resumes once answered, and ends decided or passed up with the ticket blocked on its Question.

Spec: K9, H16, H18

**ticket-captain** (NeedForBalazs):

> Save your branch's head SHA and the need on your ticket and resume from them once it is answered; end resolved when the call is decided where it arose or answered on its Question, or escalated with the need passed up and your ticket blocked on its Question.

## Liveness

`claim:Recovery.Liveness`: A liveness finding is saved as its comment, the hop resumes from its last posted SHA, and it ends healed or escalated.

Spec: K9, S24, H12

**ticket-captain** (Liveness):

> Save each finding as its `liveness <UTC time>: <outcome>` comment and resume the hop from its last posted SHA; end resolved once the hop is alive, up to date or restarted, or escalated to your officer with a hop you can't restart.

**chief-of-staff** (Liveness):

> Save each finding as its `liveness <UTC time>: <outcome>` comment and resume the hop from its last posted SHA; end resolved once the hop is alive, up to date or restarted, or escalated, when it needs him, through its coordinator's own envoy, never through you.

## OverLimit

`claim:Recovery.OverLimit`: A file over the limit saves the work, resumes once its path is settled, and ends split or ignored, or by path (b).

Spec: K9, T16, T17, T18

**ticket-captain** (OverLimit):

> Save the head SHA on the ticket and resume from it once the file's path is settled; end resolved once the file is split or its entry checked, or escalated by path (b), which ends in his blessing.

**implementer** (OverLimit):

> Save your work on the brief's branch and resume from it once the file's path is settled; end resolved once the file is split or its entry is in your PR, or escalated to your captain for path (b).

## LandingNotAccepted

`claim:Recovery.LandingNotAccepted`: A landing he does not accept saves the branch's SHA, resumes the re-cut, and ends with another landing or escalated.

Spec: K9, N6

**ticket-captain** (LandingNotAccepted):

> Save the landing branch's SHA on its ticket and resume the re-cut from it; end resolved once another landing follows, or escalated to your officer with what you can't resolve.

## Replanning

`claim:Recovery.Replanning`: A re-plan saves the blocker on the Wayfinding ticket, resumes watching, and ends sequenced or as a Question.

Spec: K9, R9, H16

**coordinator** (Replanning):

> Save the blocker or requirement on the Wayfinding ticket and resume watching from what it files; end resolved once its re-plan is sequenced, or escalated as a `research` Question when its captain passes a need up.

## RejectedFiller

`claim:Recovery.RejectedFiller`: A filler sent back keeps its reason, resumes once released, and ends released or held by the Wayfinding ticket.

Spec: K9, H11

**coordinator** (RejectedFiller):

> Save the captain's one-line reason on the filler and resume pulling it once its block is released; end resolved when a Wayfinding ticket records the release or he assigns it, or escalated through the Wayfinding ticket that holds the block.

## Survivors

`claim:Recovery.Survivors`: Survivors are saved on their fix tickets, the landing resumes once they close, and it ends clean or as a Question.

Spec: K9, T12, H16

**coordinator** (Survivors):

> Save each survivor on its fix ticket and resume the landing once every one is closed; end resolved once the mutation check is clean, or escalated as a `research` Question when its captain passes a need up.

## Survivor

`claim:Recovery.Survivor`: A survivor's writer saves the work on the branch, resumes the hop, and ends killed, approved or at a council.

Spec: K9, T12

**implementer** (Survivor):

> Save your work on the brief's branch and resume the hop from it; end resolved once the survivor is killed or its claim approved, or escalated to a council when you dispute a rejected claim.

## Hunks

`claim:Recovery.Hunks`: A hunk review saves its verdict where the captain resumes, and ends on a PASS or escalated to the captain.

Spec: K9, R19

**reviewer** (Hunks):

> Save your verdict on the hop, where your captain resumes the merge from it; end resolved with a PASS naming the new head, or escalated to your captain with what you can't judge.

## Equivalent

`claim:Recovery.Equivalent`: An equivalence ruling is saved on the claim where the mutation work resumes, and ends approved, sent back or at a council.

Spec: K9, T12

**reviewer** (Equivalent):

> Save your ruling and its reason on the claim, where the mutation work resumes from it; end resolved with the claim approved or sent back to fix, or escalated to a council when the author disputes it.

## IgnoreEntry

`claim:Recovery.IgnoreEntry`: An ignore-entry check is saved in the verdict, and ends passed, sent back, or escalated for path (b).

Spec: K9, T17, T18

**reviewer** (IgnoreEntry):

> Save your check and its reason in your verdict, where your captain resumes from it; end resolved once the entry passes or goes back to its writer, or escalated to your captain for path (b) when the file is written by hand.

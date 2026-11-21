\* built from the model; edit there: tools/SkillBuild
---- MODULE Captain ----
EXTENDS Naturals

VARIABLES state, round, extra

vars == <<state, round, extra>>

Finals == {"Returned"}

Init ==
    /\ state = "Taken"
    /\ round = 1
    /\ extra = FALSE

\* claim:Captain.FillerBack
Captain_FillerBack ==
    /\ state = "Taken"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainDispatch.Send
CaptainDispatch_Send ==
    /\ state = "Split"
    /\ state' = "Waiting"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainFinish.Close
CaptainFinish_Close ==
    /\ state = "Finishing"
    /\ state' = "Done"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainFinish.HandOver
CaptainFinish_HandOver ==
    /\ state = "Leaf"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainFinish.Return
CaptainFinish_Return ==
    /\ state = "Done"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainMergeTrouble.Hunks
CaptainMergeTrouble_Hunks ==
    /\ state = "Repairing"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainMergeTrouble.Lost
CaptainMergeTrouble_Lost ==
    /\ state = "Repairing"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainMergeTrouble.Stuck
CaptainMergeTrouble_Stuck ==
    /\ state = "Repairing"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainMerging.Broken
CaptainMerging_Broken ==
    /\ state = "Finishing"
    /\ state' = "Repairing"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainNeed.Answered
CaptainNeed_Answered ==
    /\ state = "Question"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainNeed.Ask
CaptainNeed_Ask ==
    /\ state = "Leaf"
    /\ state' = "Question"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainNeed.Up
CaptainNeed_Up ==
    /\ state = "Question"
    /\ state' = "Question"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.Clean
CaptainReview_Clean ==
    /\ state = "Clear"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.Cleared
CaptainReview_Cleared ==
    /\ state = "InReview"
    /\ state' = "Clear"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.Fix
CaptainReview_Fix ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.Groups
CaptainReview_Groups ==
    /\ state = "Waiting"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.OneMore
CaptainReview_OneMore ==
    /\ state = "Clear"
    /\ ~extra
    /\ state' = "InReview"
    /\ round' = round + 1 /\ extra' = TRUE

\* claim:CaptainReview.Other
CaptainReview_Other ==
    /\ state = "Leaf"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.Replan
CaptainReview_Replan ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainReview.Whole
CaptainReview_Whole ==
    /\ state = "Waiting"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainSplit.Cut
CaptainSplit_Cut ==
    /\ state = "Taken"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainSplit.Leaf
CaptainSplit_Leaf ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainTrouble.Checkpoint
CaptainTrouble_Checkpoint ==
    /\ state = "Checkpoint"
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainTrouble.Escalate
CaptainTrouble_Escalate ==
    /\ state = "Checkpoint"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainTrouble.NextRound
CaptainTrouble_NextRound ==
    /\ state = "Fixing"
    /\ state' = "InReview"
    /\ round' = round + 1
    /\ UNCHANGED <<extra>>

\* claim:CaptainTrouble.ReCut
CaptainTrouble_ReCut ==
    /\ state = "Leaf"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainTrouble.SplitRest
CaptainTrouble_SplitRest ==
    /\ state = "Leaf"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainTrouble.Third
CaptainTrouble_Third ==
    /\ state = "InReview"
    /\ round = 3
    /\ state' = "Checkpoint"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainWay.Fog
CaptainWay_Fog ==
    /\ state = "Taken"
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainWay.Next
CaptainWay_Next ==
    /\ state = "Waiting"
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainWay.Visible
CaptainWay_Visible ==
    /\ state = "Wayfinding"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Captain_FillerBack
    \/ CaptainDispatch_Send
    \/ CaptainFinish_Close
    \/ CaptainFinish_HandOver
    \/ CaptainFinish_Return
    \/ CaptainMergeTrouble_Hunks
    \/ CaptainMergeTrouble_Lost
    \/ CaptainMergeTrouble_Stuck
    \/ CaptainMerging_Broken
    \/ CaptainNeed_Answered
    \/ CaptainNeed_Ask
    \/ CaptainNeed_Up
    \/ CaptainReview_Clean
    \/ CaptainReview_Cleared
    \/ CaptainReview_Fix
    \/ CaptainReview_Groups
    \/ CaptainReview_OneMore
    \/ CaptainReview_Other
    \/ CaptainReview_Replan
    \/ CaptainReview_Whole
    \/ CaptainSplit_Cut
    \/ CaptainSplit_Leaf
    \/ CaptainTrouble_Checkpoint
    \/ CaptainTrouble_Escalate
    \/ CaptainTrouble_NextRound
    \/ CaptainTrouble_ReCut
    \/ CaptainTrouble_SplitRest
    \/ CaptainTrouble_Third
    \/ CaptainWay_Fog
    \/ CaptainWay_Next
    \/ CaptainWay_Visible
    \/ Done

Bounds == round <= 4 /\ extra \in BOOLEAN

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

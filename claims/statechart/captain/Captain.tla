\* built from the model; edit there: tools/SkillBuild
---- MODULE Captain ----
EXTENDS Naturals

VARIABLES state, round, extra, kind

vars == <<state, round, extra, kind>>

Finals == {"Returned"}

Init ==
    /\ state = "Dispatched"
    /\ round = 1
    /\ extra = FALSE
    /\ kind = "unread"

\* claim:Captain.FillerBack
Captain_FillerBack ==
    /\ state = "Dispatched"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:Captain.Start
Captain_Start ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ kind' \in {"change", "verdict"}
    /\ UNCHANGED <<round, extra>>

\* claim:CaptainDispatch.Send
CaptainDispatch_Send ==
    /\ state = "Split"
    /\ state' = "Waiting"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainFinish.Close
CaptainFinish_Close ==
    /\ state = "Finishing"
    /\ state' = "Done"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainFinish.HandOver
CaptainFinish_HandOver ==
    /\ state = "Leaf"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainFinish.Return
CaptainFinish_Return ==
    /\ state = "Done"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainIntake.Held
CaptainIntake_Held ==
    /\ state = "Dispatched"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainMergeTrouble.Hunks
CaptainMergeTrouble_Hunks ==
    /\ state = "Repairing"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainMergeTrouble.Lost
CaptainMergeTrouble_Lost ==
    /\ state = "Repairing"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainMergeTrouble.Stuck
CaptainMergeTrouble_Stuck ==
    /\ state = "Repairing"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainMerging.Broken
CaptainMerging_Broken ==
    /\ state = "Finishing"
    /\ kind = "change"
    /\ state' = "Repairing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainNeed.Answered
CaptainNeed_Answered ==
    /\ state = "Question"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainNeed.Ask
CaptainNeed_Ask ==
    /\ state = "Leaf"
    /\ state' = "Question"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainNeed.Up
CaptainNeed_Up ==
    /\ state = "Question"
    /\ state' = "Question"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainResearch.Question
CaptainResearch_Question ==
    /\ state = "Leaf"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.Clean
CaptainReview_Clean ==
    /\ state = "Clear"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.Cleared
CaptainReview_Cleared ==
    /\ state = "InReview"
    /\ state' = "Clear"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.Fix
CaptainReview_Fix ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.Groups
CaptainReview_Groups ==
    /\ state = "Waiting"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.OneMore
CaptainReview_OneMore ==
    /\ state = "Clear"
    /\ ~extra
    /\ state' = "InReview"
    /\ round' = round + 1 /\ extra' = TRUE
    /\ UNCHANGED <<kind>>

\* claim:CaptainReview.Other
CaptainReview_Other ==
    /\ state = "Leaf"
    /\ kind = "change"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.Replan
CaptainReview_Replan ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReview.Whole
CaptainReview_Whole ==
    /\ state = "Waiting"
    /\ kind = "change"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReviewSeats.Ticket
CaptainReviewSeats_Ticket ==
    /\ state = "Taken"
    /\ kind = "verdict"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainReviewSeats.Verdict
CaptainReviewSeats_Verdict ==
    /\ state = "Waiting"
    /\ kind = "verdict"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainSplit.Cut
CaptainSplit_Cut ==
    /\ state = "Taken"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainSplit.Leaf
CaptainSplit_Leaf ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainTrouble.Checkpoint
CaptainTrouble_Checkpoint ==
    /\ state = "Checkpoint"
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainTrouble.ChildReCut
CaptainTrouble_ChildReCut ==
    /\ state = "Waiting"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainTrouble.Escalate
CaptainTrouble_Escalate ==
    /\ state = "Checkpoint"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainTrouble.NextRound
CaptainTrouble_NextRound ==
    /\ state = "Fixing"
    /\ state' = "InReview"
    /\ round' = round + 1
    /\ UNCHANGED <<extra, kind>>

\* claim:CaptainTrouble.ReCut
CaptainTrouble_ReCut ==
    /\ state = "Leaf"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainTrouble.SplitRest
CaptainTrouble_SplitRest ==
    /\ state = "Leaf"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainTrouble.Third
CaptainTrouble_Third ==
    /\ state = "InReview"
    /\ round = 3
    /\ state' = "Checkpoint"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainWay.Fog
CaptainWay_Fog ==
    /\ state = "Taken"
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainWay.Next
CaptainWay_Next ==
    /\ state = "Waiting"
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<round, extra, kind>>

\* claim:CaptainWay.Visible
CaptainWay_Visible ==
    /\ state = "Wayfinding"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, kind>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Captain_FillerBack
    \/ Captain_Start
    \/ CaptainDispatch_Send
    \/ CaptainFinish_Close
    \/ CaptainFinish_HandOver
    \/ CaptainFinish_Return
    \/ CaptainIntake_Held
    \/ CaptainMergeTrouble_Hunks
    \/ CaptainMergeTrouble_Lost
    \/ CaptainMergeTrouble_Stuck
    \/ CaptainMerging_Broken
    \/ CaptainNeed_Answered
    \/ CaptainNeed_Ask
    \/ CaptainNeed_Up
    \/ CaptainResearch_Question
    \/ CaptainReview_Clean
    \/ CaptainReview_Cleared
    \/ CaptainReview_Fix
    \/ CaptainReview_Groups
    \/ CaptainReview_OneMore
    \/ CaptainReview_Other
    \/ CaptainReview_Replan
    \/ CaptainReview_Whole
    \/ CaptainReviewSeats_Ticket
    \/ CaptainReviewSeats_Verdict
    \/ CaptainSplit_Cut
    \/ CaptainSplit_Leaf
    \/ CaptainTrouble_Checkpoint
    \/ CaptainTrouble_ChildReCut
    \/ CaptainTrouble_Escalate
    \/ CaptainTrouble_NextRound
    \/ CaptainTrouble_ReCut
    \/ CaptainTrouble_SplitRest
    \/ CaptainTrouble_Third
    \/ CaptainWay_Fog
    \/ CaptainWay_Next
    \/ CaptainWay_Visible
    \/ Done

Bounds == round <= 4 /\ extra \in BOOLEAN /\ kind \in {"unread", "change", "verdict"}

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

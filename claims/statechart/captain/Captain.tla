\* built from the model; edit there: tools/SkillBuild
---- MODULE Captain ----
EXTENDS Naturals

VARIABLES state, round, extra, mended, kind, merger

vars == <<state, round, extra, mended, kind, merger>>

Finals == {"Returned"}

Init ==
    /\ state = "Dispatched"
    /\ round = 1
    /\ extra = FALSE
    /\ mended = FALSE
    /\ kind = "unread"
    /\ merger = "unread"

\* claim:Captain.FillerBack
Captain_FillerBack ==
    /\ state = "Dispatched"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:Captain.Start
Captain_Start ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ kind' \in {"change", "verdict", "answer"} /\ merger' \in IF kind' = "change" THEN {"holder", "officer", "nobody"} ELSE {"nobody"}
    /\ UNCHANGED <<round, extra, mended>>

\* claim:CaptainDispatch.Send
CaptainDispatch_Send ==
    /\ state = "Split"
    /\ state' = "Waiting"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainFinish.Close
CaptainFinish_Close ==
    /\ state = "Finishing"
    /\ state' = "Done"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainFinish.HandOver
CaptainFinish_HandOver ==
    /\ state = "Leaf"
    /\ merger # "holder"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainFinish.Return
CaptainFinish_Return ==
    /\ state = "Done"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainIntake.Held
CaptainIntake_Held ==
    /\ state = "Dispatched"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainMergeTrouble.Hunks
CaptainMergeTrouble_Hunks ==
    /\ state = "Repairing"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainMergeTrouble.Lost
CaptainMergeTrouble_Lost ==
    /\ state = "Repairing"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainMergeTrouble.Stuck
CaptainMergeTrouble_Stuck ==
    /\ state = "Repairing"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainMerging.Broken
CaptainMerging_Broken ==
    /\ state = "Finishing"
    /\ kind = "change" /\ merger = "holder"
    /\ state' = "Repairing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainNeed.Answered
CaptainNeed_Answered ==
    /\ state = "Question"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainNeed.Ask
CaptainNeed_Ask ==
    /\ state = "Leaf"
    /\ state' = "Question"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainNeed.Up
CaptainNeed_Up ==
    /\ state = "Question"
    /\ state' = "Question"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainResearch.Handed
CaptainResearch_Handed ==
    /\ state = "Leaf"
    /\ kind = "answer"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainResearch.Parts
CaptainResearch_Parts ==
    /\ state = "Waiting"
    /\ kind = "answer"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainResearch.Question
CaptainResearch_Question ==
    /\ state = "Leaf"
    /\ kind = "answer"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.Clean
CaptainReview_Clean ==
    /\ state = "Clear"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.Cleared
CaptainReview_Cleared ==
    /\ state = "InReview"
    /\ state' = "Clear"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.Fix
CaptainReview_Fix ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.Groups
CaptainReview_Groups ==
    /\ state = "Waiting"
    /\ kind = "change" /\ merger # "holder"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.OneMore
CaptainReview_OneMore ==
    /\ state = "Clear"
    /\ ~extra
    /\ state' = "InReview"
    /\ round' = round + 1 /\ extra' = TRUE
    /\ UNCHANGED <<mended, kind, merger>>

\* claim:CaptainReview.Other
CaptainReview_Other ==
    /\ state = "Leaf"
    /\ kind = "change" /\ merger = "holder"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.Replan
CaptainReview_Replan ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReview.Whole
CaptainReview_Whole ==
    /\ state = "Waiting"
    /\ kind = "change"
    /\ state' = "InReview"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReviewSeats.Ticket
CaptainReviewSeats_Ticket ==
    /\ state = "Taken"
    /\ kind = "verdict"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainReviewSeats.Verdict
CaptainReviewSeats_Verdict ==
    /\ state = "Waiting"
    /\ kind = "verdict"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainRound.Cheap
CaptainRound_Cheap ==
    /\ state = "Clear"
    /\ ~mended /\ merger # "nobody"
    /\ state' = "Mending"
    /\ mended' = TRUE
    /\ UNCHANGED <<round, extra, kind, merger>>

\* claim:CaptainRound.MendedPart
CaptainRound_MendedPart ==
    /\ state = "Mending"
    /\ merger = "officer"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainRound.NewHead
CaptainRound_NewHead ==
    /\ state = "Mending"
    /\ merger = "holder"
    /\ state' = "InReview"
    /\ round' = round + 1
    /\ UNCHANGED <<extra, mended, kind, merger>>

\* claim:CaptainSplit.Cut
CaptainSplit_Cut ==
    /\ state = "Taken"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainSplit.Leaf
CaptainSplit_Leaf ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainTrouble.Checkpoint
CaptainTrouble_Checkpoint ==
    /\ state = "Checkpoint"
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainTrouble.ChildReCut
CaptainTrouble_ChildReCut ==
    /\ state = "Waiting"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainTrouble.Escalate
CaptainTrouble_Escalate ==
    /\ state = "Checkpoint"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainTrouble.NextRound
CaptainTrouble_NextRound ==
    /\ state = "Fixing"
    /\ state' = "InReview"
    /\ round' = round + 1
    /\ UNCHANGED <<extra, mended, kind, merger>>

\* claim:CaptainTrouble.ReCut
CaptainTrouble_ReCut ==
    /\ state = "Leaf"
    /\ state' = "Returned"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainTrouble.SplitRest
CaptainTrouble_SplitRest ==
    /\ state = "Leaf"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainTrouble.Third
CaptainTrouble_Third ==
    /\ state = "InReview"
    /\ round = 3
    /\ state' = "Checkpoint"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainWay.Fog
CaptainWay_Fog ==
    /\ state = "Taken"
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainWay.Next
CaptainWay_Next ==
    /\ state = "Waiting"
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

\* claim:CaptainWay.Visible
CaptainWay_Visible ==
    /\ state = "Wayfinding"
    /\ state' = "Split"
    /\ UNCHANGED <<round, extra, mended, kind, merger>>

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
    \/ CaptainResearch_Handed
    \/ CaptainResearch_Parts
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
    \/ CaptainRound_Cheap
    \/ CaptainRound_MendedPart
    \/ CaptainRound_NewHead
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

Bounds == round <= 4 /\ extra \in BOOLEAN /\ mended \in BOOLEAN /\ kind \in {"unread", "change", "verdict", "answer"} /\ merger \in {"unread", "holder", "officer", "nobody"}

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

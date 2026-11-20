\* built from the model; edit there: tools/SkillBuild
---- MODULE Captain ----
EXTENDS Naturals

VARIABLES state, round

vars == <<state, round>>

Finals == {"Returned"}

Init ==
    /\ state = "Taken"
    /\ round = 1

\* claim:Captain.Filler
Captain_Filler ==
    /\ state = "Taken"
    /\ state' = "Returned"
    /\ UNCHANGED <<round>>

\* claim:CaptainDispatch.Send
CaptainDispatch_Send ==
    /\ state = "Split"
    /\ state' = "Waiting"
    /\ UNCHANGED <<round>>

\* claim:CaptainFinish.Close
CaptainFinish_Close ==
    /\ state = "Finishing"
    /\ state' = "Done"
    /\ UNCHANGED <<round>>

\* claim:CaptainFinish.HandOver
CaptainFinish_HandOver ==
    /\ state = "Leaf"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round>>

\* claim:CaptainFinish.Return
CaptainFinish_Return ==
    /\ state = "Done"
    /\ state' = "Returned"
    /\ UNCHANGED <<round>>

\* claim:CaptainMerging.Broken
CaptainMerging_Broken ==
    /\ state = "Finishing"
    /\ state' = "Waiting"
    /\ UNCHANGED <<round>>

\* claim:CaptainReview.Clean
CaptainReview_Clean ==
    /\ state = "InReview"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round>>

\* claim:CaptainReview.Fix
CaptainReview_Fix ==
    /\ state = "InReview"
    /\ state' = "Fixing"
    /\ UNCHANGED <<round>>

\* claim:CaptainReview.Groups
CaptainReview_Groups ==
    /\ state = "Waiting"
    /\ state' = "Finishing"
    /\ UNCHANGED <<round>>

\* claim:CaptainReview.Other
CaptainReview_Other ==
    /\ state = "Leaf"
    /\ state' = "InReview"
    /\ UNCHANGED <<round>>

\* claim:CaptainReview.Replan
CaptainReview_Replan ==
    /\ state = "InReview"
    /\ state' = "Returned"
    /\ UNCHANGED <<round>>

\* claim:CaptainReview.Whole
CaptainReview_Whole ==
    /\ state = "Waiting"
    /\ state' = "InReview"
    /\ UNCHANGED <<round>>

\* claim:CaptainSplit.Cut
CaptainSplit_Cut ==
    /\ state = "Taken"
    /\ state' = "Split"
    /\ UNCHANGED <<round>>

\* claim:CaptainSplit.Leaf
CaptainSplit_Leaf ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round>>

\* claim:CaptainTrouble.Answered
CaptainTrouble_Answered ==
    /\ state = "Question"
    /\ state' = "Leaf"
    /\ UNCHANGED <<round>>

\* claim:CaptainTrouble.Ask
CaptainTrouble_Ask ==
    /\ state = "Leaf"
    /\ state' = "Question"
    /\ UNCHANGED <<round>>

\* claim:CaptainTrouble.NextRound
CaptainTrouble_NextRound ==
    /\ state = "Fixing"
    /\ state' = "InReview"
    /\ round' = round + 1

\* claim:CaptainTrouble.ReCut
CaptainTrouble_ReCut ==
    /\ state = "Leaf"
    /\ state' = "Returned"
    /\ UNCHANGED <<round>>

\* claim:CaptainTrouble.SplitRest
CaptainTrouble_SplitRest ==
    /\ state = "Leaf"
    /\ state' = "Split"
    /\ UNCHANGED <<round>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Captain_Filler
    \/ CaptainDispatch_Send
    \/ CaptainFinish_Close
    \/ CaptainFinish_HandOver
    \/ CaptainFinish_Return
    \/ CaptainMerging_Broken
    \/ CaptainReview_Clean
    \/ CaptainReview_Fix
    \/ CaptainReview_Groups
    \/ CaptainReview_Other
    \/ CaptainReview_Replan
    \/ CaptainReview_Whole
    \/ CaptainSplit_Cut
    \/ CaptainSplit_Leaf
    \/ CaptainTrouble_Answered
    \/ CaptainTrouble_Ask
    \/ CaptainTrouble_NextRound
    \/ CaptainTrouble_ReCut
    \/ CaptainTrouble_SplitRest
    \/ Done

Bounds == round <= 4

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

\* built from the model; edit there: tools/SkillBuild
---- MODULE Landing ----
EXTENDS Naturals

VARIABLES state, round, areas, fixFrom, securityFirst, prereqs, hardened

vars == <<state, round, areas, fixFrom, securityFirst, prereqs, hardened>>

Finals == {"Tagged", "Replanned", "Escalated"}

Open == {"GAP 1"}

Init ==
    /\ state = "Integrated"
    /\ round = 1
    /\ areas = 1
    /\ fixFrom = "none"
    /\ securityFirst = FALSE
    /\ prereqs = 0
    /\ hardened = FALSE

\* claim:Landing.Accepted
Landing_Accepted ==
    /\ state = "Walkthrough"
    /\ state' = "Merging"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.AllAreas
Landing_AllAreas ==
    /\ state = "DrivePhase"
    /\ areas = 1
    /\ state' = "Inquisition"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.BreakDone
Landing_BreakDone ==
    /\ state = "BreakPhase"
    /\ state' = "DrivePhase"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.CleanRound
Landing_CleanRound ==
    /\ state = "Inquisition"
    /\ state' = "Prerequisites"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.Confident
Landing_Confident ==
    /\ state = "MergeReview"
    /\ state' = "Walkthrough"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.Hardened
Landing_Hardened ==
    /\ state = "Landed"
    /\ ~hardened
    /\ state' = "Landed"
    /\ hardened' = TRUE
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs>>

\* claim:Landing.MapDrawn
Landing_MapDrawn ==
    /\ state = "Mapped"
    /\ state' = "ReadPhase"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.Merged
Landing_Merged ==
    /\ state = "Merging"
    /\ state' = "Landed"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.NextArea
Landing_NextArea ==
    /\ state = "DrivePhase"
    /\ areas > 1
    /\ state' = "ReadPhase"
    /\ areas' = areas - 1
    /\ UNCHANGED <<round, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.NextMergeRound
Landing_NextMergeRound ==
    /\ state = "RoundFixes"
    /\ fixFrom = "merge"
    /\ state' = "MergeReview"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.NextRound
Landing_NextRound ==
    /\ state = "RoundFixes"
    /\ fixFrom = "inquisition"
    /\ state' = "Mapped"
    /\ round' = round + 1 /\ areas' \in 1..2
    /\ UNCHANGED <<fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.PrerequisiteMet
Landing_PrerequisiteMet ==
    /\ state = "Prerequisites"
    /\ prereqs < 10
    /\ state' = "Prerequisites"
    /\ prereqs' = prereqs + 1
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, hardened>>

\* claim:Landing.ReadDone
Landing_ReadDone ==
    /\ state = "ReadPhase"
    /\ state' = "BreakPhase"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.Ready
Landing_Ready ==
    /\ state = "Prerequisites"
    /\ prereqs = 10
    /\ state' = "MergeReview"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.ReviewFail
Landing_ReviewFail ==
    /\ state = "MergeReview"
    /\ state' = "RoundFixes"
    /\ fixFrom' = "merge"
    /\ UNCHANGED <<round, areas, securityFirst, prereqs, hardened>>

\* claim:Landing.ReviewFundamental
Landing_ReviewFundamental ==
    /\ state = "MergeReview"
    /\ state' = "Replanned"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.ReviewNeedsHim
Landing_ReviewNeedsHim ==
    /\ state = "MergeReview"
    /\ state' = "Escalated"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.ReviewSecurity
Landing_ReviewSecurity ==
    /\ state = "MergeReview"
    /\ state' = "RoundFixes"
    /\ fixFrom' = "merge" /\ securityFirst' = TRUE
    /\ UNCHANGED <<round, areas, prereqs, hardened>>

\* claim:Landing.RoundFound
Landing_RoundFound ==
    /\ state = "Inquisition"
    /\ state' = "RoundFixes"
    /\ fixFrom' = "inquisition"
    /\ UNCHANGED <<round, areas, securityFirst, prereqs, hardened>>

\* claim:Landing.Staff
Landing_Staff ==
    /\ state = "Integrated"
    /\ state' = "Mapped"
    /\ areas' \in 1..2
    /\ UNCHANGED <<round, fixFrom, securityFirst, prereqs, hardened>>

\* claim:Landing.Tag
Landing_Tag ==
    /\ state = "Landed"
    /\ hardened
    /\ state' = "Tagged"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

\* GAP: N6 gives only his acceptance; the route when he does not accept is unstated
Gap1 ==
    /\ state = "Walkthrough"
    /\ state' = "GAP 1"
    /\ UNCHANGED <<round, areas, fixFrom, securityFirst, prereqs, hardened>>

Done == state \in Finals \cup Open /\ UNCHANGED vars

Next ==
    \/ Landing_Accepted
    \/ Landing_AllAreas
    \/ Landing_BreakDone
    \/ Landing_CleanRound
    \/ Landing_Confident
    \/ Landing_Hardened
    \/ Landing_MapDrawn
    \/ Landing_Merged
    \/ Landing_NextArea
    \/ Landing_NextMergeRound
    \/ Landing_NextRound
    \/ Landing_PrerequisiteMet
    \/ Landing_ReadDone
    \/ Landing_Ready
    \/ Landing_ReviewFail
    \/ Landing_ReviewFundamental
    \/ Landing_ReviewNeedsHim
    \/ Landing_ReviewSecurity
    \/ Landing_RoundFound
    \/ Landing_Staff
    \/ Landing_Tag
    \/ Gap1
    \/ Done

Bounds == round <= 3 /\ areas \in 1..2 /\ fixFrom \in {"none", "inquisition", "merge"} /\ securityFirst \in BOOLEAN /\ prereqs <= 10 /\ hardened \in BOOLEAN

Spec == Init /\ [][Next]_vars
====

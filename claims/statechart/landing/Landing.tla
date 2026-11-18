\* built from the model; edit there: tools/SkillBuild
---- MODULE Landing ----
EXTENDS Naturals

VARIABLES state, round, fixFrom, prereqs, hardened

vars == <<state, round, fixFrom, prereqs, hardened>>

Finals == {"Tagged"}

Open == {"GAP 1"}

Init ==
    /\ state = "Integrated"
    /\ round = 1
    /\ fixFrom = "none"
    /\ prereqs = 0
    /\ hardened = FALSE

\* claim:Landing.Accepted
Landing_Accepted ==
    /\ state = "Walkthrough"
    /\ state' = "Merging"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.CleanRound
Landing_CleanRound ==
    /\ state = "Inquisition"
    /\ state' = "Prerequisites"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.Confident
Landing_Confident ==
    /\ state = "MergeReview"
    /\ state' = "Walkthrough"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.Hardened
Landing_Hardened ==
    /\ state = "Landed"
    /\ ~hardened
    /\ state' = "Landed"
    /\ hardened' = TRUE
    /\ UNCHANGED <<round, fixFrom, prereqs>>

\* claim:Landing.Merged
Landing_Merged ==
    /\ state = "Merging"
    /\ state' = "Landed"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.NextMergeRound
Landing_NextMergeRound ==
    /\ state = "RoundFixes"
    /\ fixFrom = "merge"
    /\ state' = "MergeReview"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.NextRound
Landing_NextRound ==
    /\ state = "RoundFixes"
    /\ fixFrom = "inquisition"
    /\ state' = "Inquisition"
    /\ round' = round + 1
    /\ UNCHANGED <<fixFrom, prereqs, hardened>>

\* claim:Landing.PrerequisiteMet
Landing_PrerequisiteMet ==
    /\ state = "Prerequisites"
    /\ prereqs < 10
    /\ state' = "Prerequisites"
    /\ prereqs' = prereqs + 1
    /\ UNCHANGED <<round, fixFrom, hardened>>

\* claim:Landing.Ready
Landing_Ready ==
    /\ state = "Prerequisites"
    /\ prereqs = 10
    /\ state' = "MergeReview"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.ReviewFail
Landing_ReviewFail ==
    /\ state = "MergeReview"
    /\ state' = "RoundFixes"
    /\ fixFrom' = "merge"
    /\ UNCHANGED <<round, prereqs, hardened>>

\* claim:Landing.RoundFound
Landing_RoundFound ==
    /\ state = "Inquisition"
    /\ state' = "RoundFixes"
    /\ fixFrom' = "inquisition"
    /\ UNCHANGED <<round, prereqs, hardened>>

\* claim:Landing.Staff
Landing_Staff ==
    /\ state = "Integrated"
    /\ state' = "Inquisition"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* claim:Landing.Tag
Landing_Tag ==
    /\ state = "Landed"
    /\ hardened
    /\ state' = "Tagged"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

\* GAP: N6 gives only his acceptance; the route when he does not accept is unstated
Gap1 ==
    /\ state = "Walkthrough"
    /\ state' = "GAP 1"
    /\ UNCHANGED <<round, fixFrom, prereqs, hardened>>

Done == state \in Finals \cup Open /\ UNCHANGED vars

Next ==
    \/ Landing_Accepted
    \/ Landing_CleanRound
    \/ Landing_Confident
    \/ Landing_Hardened
    \/ Landing_Merged
    \/ Landing_NextMergeRound
    \/ Landing_NextRound
    \/ Landing_PrerequisiteMet
    \/ Landing_Ready
    \/ Landing_ReviewFail
    \/ Landing_RoundFound
    \/ Landing_Staff
    \/ Landing_Tag
    \/ Gap1
    \/ Done

Bounds == round <= 3 /\ fixFrom \in {"none", "inquisition", "merge"} /\ prereqs <= 10 /\ hardened \in BOOLEAN

Spec == Init /\ [][Next]_vars
====

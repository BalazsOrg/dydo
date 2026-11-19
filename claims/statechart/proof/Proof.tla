\* built from the model; edit there: tools/SkillBuild
---- MODULE Proof ----

VARIABLES state

vars == <<state>>

Finals == {"Graduated", "Dropped", "Finding"}

Init ==
    /\ state = "Start"

\* claim:Testing.Delete
Testing_Delete ==
    /\ state = "Old"
    /\ state' = "Dropped"

\* claim:Testing.Drop
Testing_Drop ==
    /\ state = "Receipt"
    /\ state' = "Dropped"

\* claim:Testing.GoRed
Testing_GoRed ==
    /\ state = "Gated"
    /\ state' = "GateRed"

\* claim:Testing.Graduate
Testing_Graduate ==
    /\ state = "Receipt"
    /\ state' = "Graduated"

\* claim:Testing.Green
Testing_Green ==
    /\ state = "Gated"
    /\ state' = "Receipt"

\* claim:Testing.Keep
Testing_Keep ==
    /\ state = "Old"
    /\ state' = "Graduated"

\* claim:Testing.Legacy
Testing_Legacy ==
    /\ state = "Start"
    /\ state' = "Old"

\* claim:Testing.MatchesOld
Testing_MatchesOld ==
    /\ state = "Conformance"
    /\ state' = "Dropped"

\* claim:Testing.Refix
Testing_Refix ==
    /\ state = "GateRed"
    /\ state' = "Gated"

\* claim:Testing.Rewrite
Testing_Rewrite ==
    /\ state = "Start"
    /\ state' = "Conformance"

\* claim:Testing.Rewritten
Testing_Rewritten ==
    /\ state = "Old"
    /\ state' = "Written"

\* claim:Testing.RunGates
Testing_RunGates ==
    /\ state = "Red"
    /\ state' = "Gated"

\* claim:Testing.ShowRed
Testing_ShowRed ==
    /\ state = "Written"
    /\ state' = "Red"

\* claim:Testing.Traced
Testing_Traced ==
    /\ state = "Conformance"
    /\ state' = "Red"

\* claim:Testing.Untraced
Testing_Untraced ==
    /\ state = "Graduated"
    /\ state' = "Finding"

\* claim:Testing.Write
Testing_Write ==
    /\ state = "Start"
    /\ state' = "Written"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Testing_Delete
    \/ Testing_Drop
    \/ Testing_GoRed
    \/ Testing_Graduate
    \/ Testing_Green
    \/ Testing_Keep
    \/ Testing_Legacy
    \/ Testing_MatchesOld
    \/ Testing_Refix
    \/ Testing_Rewrite
    \/ Testing_Rewritten
    \/ Testing_RunGates
    \/ Testing_ShowRed
    \/ Testing_Traced
    \/ Testing_Untraced
    \/ Testing_Write
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

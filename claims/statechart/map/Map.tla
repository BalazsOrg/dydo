\* built from the model; edit there: tools/SkillBuild
---- MODULE Map ----

VARIABLES state

vars == <<state>>

Finals == {"Shown", "Expanded"}

Init ==
    /\ state = "Closed"

\* claim:MapView.Animate
MapView_Animate ==
    /\ state = "Snapshot"
    /\ state' = "Fresh"

\* claim:MapView.Expand
MapView_Expand ==
    /\ state = "Shown"
    /\ state' = "Expanded"

\* claim:MapView.Open
MapView_Open ==
    /\ state = "Closed"
    /\ state' = "Fresh"

\* claim:MapView.Revisit
MapView_Revisit ==
    /\ state = "Closed"
    /\ state' = "Snapshot"

\* claim:MapView.Save
MapView_Save ==
    /\ state = "Fresh"
    /\ state' = "Shown"

\* claim:MapView.SaveFails
MapView_SaveFails ==
    /\ state = "Fresh"
    /\ state' = "Shown"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ MapView_Animate
    \/ MapView_Expand
    \/ MapView_Open
    \/ MapView_Revisit
    \/ MapView_Save
    \/ MapView_SaveFails
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

\* built from the model; edit there: tools/SkillBuild
---- MODULE Liveness ----

VARIABLES state

vars == <<state>>

Finals == {}

Init ==
    /\ state = "Live"

\* claim:Liveness.FoundAlive
Liveness_FoundAlive ==
    /\ state = "Quiet"
    /\ state' = "Live"

\* claim:Liveness.FoundBehind
Liveness_FoundBehind ==
    /\ state = "Quiet"
    /\ state' = "Live"

\* claim:Liveness.FoundStopped
Liveness_FoundStopped ==
    /\ state = "Quiet"
    /\ state' = "Restarted"

\* claim:Liveness.Quiet
Liveness_Quiet ==
    /\ state = "Live"
    /\ state' = "Quiet"

\* claim:Liveness.Resumed
Liveness_Resumed ==
    /\ state = "Restarted"
    /\ state' = "Live"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Liveness_FoundAlive
    \/ Liveness_FoundBehind
    \/ Liveness_FoundStopped
    \/ Liveness_Quiet
    \/ Liveness_Resumed
    \/ Done

Spec == Init /\ [][Next]_vars
====

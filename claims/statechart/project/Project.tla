---- MODULE Project ----

VARIABLES state

vars == <<state>>

Finals == {"Landing"}

Init ==
    /\ state = "Unheld"

\* claim:Coordination.Idle
Coordination_Idle ==
    /\ state = "Watching"
    /\ state' = "Unheld"

\* claim:Coordination.Wake
Coordination_Wake ==
    /\ state = "Unheld"
    /\ state' = "Watching"

\* claim:Project.Charted
Project_Charted ==
    /\ state = "Charting"
    /\ state' = "Unheld"

\* claim:Project.Replan
Project_Replan ==
    /\ state = "Watching"
    /\ state' = "Replanning"

\* claim:Project.Replanned
Project_Replanned ==
    /\ state = "Replanning"
    /\ state' = "Watching"

\* claim:Project.StartLanding
Project_StartLanding ==
    /\ state = "Watching"
    /\ state' = "Landing"

\* claim:Project.Take
Project_Take ==
    /\ state = "Unheld"
    /\ state' = "Charting"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Coordination_Idle
    \/ Coordination_Wake
    \/ Project_Charted
    \/ Project_Replan
    \/ Project_Replanned
    \/ Project_StartLanding
    \/ Project_Take
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

\* built from the model; edit there: tools/SkillBuild
---- MODULE Project ----

VARIABLES state

vars == <<state>>

Finals == {"Landing"}

Init ==
    /\ state = "Unheld"

\* claim:Project.Charted
Project_Charted ==
    /\ state = "Charting"
    /\ state' = "Watching"

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

\* GAP: H4 names idle Projects but never defines when a Project goes idle
Gap1 ==
    /\ state = "Watching"
    /\ state' = "Unheld"

\* GAP: H4 says who holds a live Project, not who starts or wakes its admiral
Gap2 ==
    /\ state = "Unheld"
    /\ state' = "Watching"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Project_Charted
    \/ Project_Replan
    \/ Project_Replanned
    \/ Project_StartLanding
    \/ Project_Take
    \/ Gap1
    \/ Gap2
    \/ Done

Spec == Init /\ [][Next]_vars
====

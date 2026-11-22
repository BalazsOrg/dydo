---- MODULE Staff ----

VARIABLES state

vars == <<state>>

Finals == {}

Init ==
    /\ state = "Day"

\* claim:Staff.Answered
Staff_Answered ==
    /\ state = "Investigating"
    /\ state' = "Day"

\* claim:Staff.Ask
Staff_Ask ==
    /\ state = "Day"
    /\ state' = "Investigating"

\* claim:Staff.Brief
Staff_Brief ==
    /\ state = "MorningBrief"
    /\ state' = "Day"

\* claim:Staff.Morning
Staff_Morning ==
    /\ state = "NightWatch"
    /\ state' = "MorningBrief"

\* claim:Staff.Muster
Staff_Muster ==
    /\ state = "Day"
    /\ state' = "Muster"

\* claim:Staff.Watch
Staff_Watch ==
    /\ state = "Muster"
    /\ state' = "NightWatch"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Staff_Answered
    \/ Staff_Ask
    \/ Staff_Brief
    \/ Staff_Morning
    \/ Staff_Muster
    \/ Staff_Watch
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

\* built from the model; edit there: tools/SkillBuild
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

\* GAP: H7 says the chief of staff runs the overnight watch, not what starts it
Gap1 ==
    /\ state = "Day"
    /\ state' = "Muster"

\* GAP: H7 and L19 name the watch and the morning brief, not what ends the night
Gap2 ==
    /\ state = "NightWatch"
    /\ state' = "MorningBrief"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Staff_Answered
    \/ Staff_Ask
    \/ Staff_Brief
    \/ Staff_Morning
    \/ Staff_Muster
    \/ Staff_Watch
    \/ Gap1
    \/ Gap2
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

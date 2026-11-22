---- MODULE Adopter ----

VARIABLES state

vars == <<state>>

Finals == {"Current", "Harvested"}

Init ==
    /\ state = "Released"

\* claim:Adopter.Consult
Adopter_Consult ==
    /\ state = "Tailored"
    /\ state' = "Consulted"

\* claim:Adopter.Decided
Adopter_Decided ==
    /\ state = "Consulted"
    /\ state' = "Updating"

\* claim:Adopter.Done
Adopter_Done ==
    /\ state = "Updating"
    /\ state' = "Current"

\* claim:Adopter.Harvest
Adopter_Harvest ==
    /\ state = "Current"
    /\ state' = "Harvested"

\* claim:Adopter.Major
Adopter_Major ==
    /\ state = "Updating"
    /\ state' = "Major"

\* claim:Adopter.Swapped
Adopter_Swapped ==
    /\ state = "Major"
    /\ state' = "Current"

\* claim:Adopter.Tailored
Adopter_Tailored ==
    /\ state = "Updating"
    /\ state' = "Tailored"

\* claim:Adopter.Update
Adopter_Update ==
    /\ state = "Released"
    /\ state' = "Updating"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Adopter_Consult
    \/ Adopter_Decided
    \/ Adopter_Done
    \/ Adopter_Harvest
    \/ Adopter_Major
    \/ Adopter_Swapped
    \/ Adopter_Tailored
    \/ Adopter_Update
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

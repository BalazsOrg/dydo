\* built from the model; edit there: tools/SkillBuild
---- MODULE SlugCheck ----

VARIABLES state

vars == <<state>>

Finals == {"Valid"}

Init ==
    /\ state = "Written"

\* claim:Slugs.Corrected
Slugs_Corrected ==
    /\ state = "Invalid"
    /\ state' = "Written"

\* claim:Slugs.Invalid
Slugs_Invalid ==
    /\ state = "Written"
    /\ state' = "Invalid"

\* claim:Slugs.Valid
Slugs_Valid ==
    /\ state = "Written"
    /\ state' = "Valid"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Slugs_Corrected
    \/ Slugs_Invalid
    \/ Slugs_Valid
    \/ Done

Spec == Init /\ [][Next]_vars
====

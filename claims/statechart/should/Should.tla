---- MODULE Should ----

VARIABLES state

vars == <<state>>

Finals == {"Known", "Done", "Duplicate", "Canceled", "Moved"}

Init ==
    /\ state = "Found"

\* claim:Should.AlreadyOpen
Should_AlreadyOpen ==
    /\ state = "Found"
    /\ state' = "Known"

\* claim:Should.Cancel
Should_Cancel ==
    /\ state = "Open"
    /\ state' = "Canceled"

\* claim:Should.Dup
Should_Dup ==
    /\ state = "Open"
    /\ state' = "Duplicate"

\* claim:Should.Fixed
Should_Fixed ==
    /\ state = "Open"
    /\ state' = "Done"

\* claim:Should.Move
Should_Move ==
    /\ state = "Open"
    /\ state' = "Moved"

\* claim:Should.Register
Should_Register ==
    /\ state = "Found"
    /\ state' = "Open"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Should_AlreadyOpen
    \/ Should_Cancel
    \/ Should_Dup
    \/ Should_Fixed
    \/ Should_Move
    \/ Should_Register
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

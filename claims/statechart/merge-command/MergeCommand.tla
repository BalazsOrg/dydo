---- MODULE MergeCommand ----

VARIABLES state

vars == <<state>>

Finals == {"Refused", "Conflict", "Merged", "AlreadyIn"}

Init ==
    /\ state = "Invoked"

\* claim:MergeCommand.AlreadyIn
MergeCommand_AlreadyIn ==
    /\ state = "Invoked"
    /\ state' = "AlreadyIn"

\* claim:MergeCommand.Conflict
MergeCommand_Conflict ==
    /\ state = "Invoked"
    /\ state' = "Conflict"

\* claim:MergeCommand.DraftOrFork
MergeCommand_DraftOrFork ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.HandShaped
MergeCommand_HandShaped ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.Merge
MergeCommand_Merge ==
    /\ state = "Invoked"
    /\ state' = "Merged"

\* claim:MergeCommand.NoPass
MergeCommand_NoPass ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.NoReceipt
MergeCommand_NoReceipt ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.NotTwoWay
MergeCommand_NotTwoWay ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.OneWay
MergeCommand_OneWay ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.ReviewOpen
MergeCommand_ReviewOpen ==
    /\ state = "Invoked"
    /\ state' = "Refused"

\* claim:MergeCommand.TipMoved
MergeCommand_TipMoved ==
    /\ state = "Invoked"
    /\ state' = "Refused"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ MergeCommand_AlreadyIn
    \/ MergeCommand_Conflict
    \/ MergeCommand_DraftOrFork
    \/ MergeCommand_HandShaped
    \/ MergeCommand_Merge
    \/ MergeCommand_NoPass
    \/ MergeCommand_NoReceipt
    \/ MergeCommand_NotTwoWay
    \/ MergeCommand_OneWay
    \/ MergeCommand_ReviewOpen
    \/ MergeCommand_TipMoved
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

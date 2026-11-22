---- MODULE TicketStatus ----

VARIABLES state

vars == <<state>>

Finals == {"Done", "Canceled", "Duplicate"}

Init ==
    /\ state = "New"

\* claim:TicketStatus.CancelDead
TicketStatus_CancelDead ==
    /\ state = "Todo"
    /\ state' = "Canceled"

\* claim:TicketStatus.CancelRunning
TicketStatus_CancelRunning ==
    /\ state = "InProgress"
    /\ state' = "Canceled"

\* claim:TicketStatus.Close
TicketStatus_Close ==
    /\ state = "InProgress"
    /\ state' = "Done"

\* claim:TicketStatus.Contract
TicketStatus_Contract ==
    /\ state = "Backlog"
    /\ state' = "Todo"

\* claim:TicketStatus.Filed
TicketStatus_Filed ==
    /\ state = "New"
    /\ state' = "Backlog"

\* claim:TicketStatus.Future
TicketStatus_Future ==
    /\ state = "New"
    /\ state' = "FutureFeature"

\* claim:TicketStatus.MarkDuplicate
TicketStatus_MarkDuplicate ==
    /\ state = "Todo"
    /\ state' = "Duplicate"

\* claim:TicketStatus.Open
TicketStatus_Open ==
    /\ state = "New"
    /\ state' = "Todo"

\* claim:TicketStatus.Promote
TicketStatus_Promote ==
    /\ state = "FutureFeature"
    /\ state' = "Backlog"

\* claim:TicketStatus.Start
TicketStatus_Start ==
    /\ state = "Todo"
    /\ state' = "InProgress"

\* claim:TicketStatus.Stopped
TicketStatus_Stopped ==
    /\ state = "InProgress"
    /\ state' = "Todo"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ TicketStatus_CancelDead
    \/ TicketStatus_CancelRunning
    \/ TicketStatus_Close
    \/ TicketStatus_Contract
    \/ TicketStatus_Filed
    \/ TicketStatus_Future
    \/ TicketStatus_MarkDuplicate
    \/ TicketStatus_Open
    \/ TicketStatus_Promote
    \/ TicketStatus_Start
    \/ TicketStatus_Stopped
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

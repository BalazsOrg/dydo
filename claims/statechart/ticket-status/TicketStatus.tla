\* built from the model; edit there: tools/SkillBuild
---- MODULE TicketStatus ----

VARIABLES state

vars == <<state>>

Finals == {"Done", "Canceled", "Duplicate"}

Open == {"GAP 2"}

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

\* claim:TicketStatus.MarkDuplicate
TicketStatus_MarkDuplicate ==
    /\ state = "Todo"
    /\ state' = "Duplicate"

\* claim:TicketStatus.Open
TicketStatus_Open ==
    /\ state = "New"
    /\ state' = "Todo"

\* claim:TicketStatus.Start
TicketStatus_Start ==
    /\ state = "Todo"
    /\ state' = "InProgress"

\* claim:TicketStatus.Stopped
TicketStatus_Stopped ==
    /\ state = "InProgress"
    /\ state' = "Todo"

\* GAP: S5 lists FutureFeature, but no rule says what enters it
Gap1 ==
    /\ state = "New"
    /\ state' = "FutureFeature"

\* GAP: S5 lists FutureFeature, but no rule says how a ticket leaves it
Gap2 ==
    /\ state = "FutureFeature"
    /\ state' = "GAP 2"

\* GAP: S5 lists Backlog, but no rule says when a ticket is filed there
Gap3 ==
    /\ state = "New"
    /\ state' = "Backlog"

Done == state \in Finals \cup Open /\ UNCHANGED vars

Next ==
    \/ TicketStatus_CancelDead
    \/ TicketStatus_CancelRunning
    \/ TicketStatus_Close
    \/ TicketStatus_Contract
    \/ TicketStatus_MarkDuplicate
    \/ TicketStatus_Open
    \/ TicketStatus_Start
    \/ TicketStatus_Stopped
    \/ Gap1
    \/ Gap2
    \/ Gap3
    \/ Done

Spec == Init /\ [][Next]_vars
====

---- MODULE TicketStatusProperties ----
EXTENDS TicketStatus

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* S3, S6: A completed step was started.
S6_StartBeforeDone == state = "Done" => "InProgress" \in seen

\* S3: Starting a step follows Todo.
S3_TodoBeforeStart == [][((state' = "InProgress" /\ state # state') => state = "Todo")]_<<vars, seen>>

\* S10: Canceled dead work remains closed in this ticket lifetime.
S10_CanceledStaysClosed == [][((state = "Canceled") => state' = "Canceled")]_<<vars, seen>>

====

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

\* S5: Structural guard for this terminal-status fixture; S5 does not forbid later reopening.
Structural_S5_CanceledStaysClosed == [][((state = "Canceled") => state' = "Canceled")]_<<vars, seen>>

\* S5 (DYD-1082 555a8300 gap 3): only he promotes a FutureFeature out, into scheduled work, Backlog
\* or Todo; it is never worked, canceled or closed as a FutureFeature. Who promotes is unmodeled.
S5_FutureLeavesOnlyByPromotion == [][((state = "FutureFeature" /\ state' # state) => state' \in {"Backlog", "Todo"})]_<<vars, seen>>

\* S5 (DYD-1082 555a8300 gap 4): a ticket filed without a contract is Backlog and Todo once contracted,
\* so a Backlog ticket is never started or closed Done straight away. The contract itself is unmodeled.
S5_BacklogNeverWorked == [][((state = "Backlog") => state' \notin {"InProgress", "Done"})]_<<vars, seen>>

====

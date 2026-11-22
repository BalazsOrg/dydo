---- MODULE Envoy ----

VARIABLES state, level, envoys

vars == <<state, level, envoys>>

Finals == {"Decided", "AskedInThread", "Answered", "Council", "Closed"}

Init ==
    /\ state = "Need"
    /\ level = "captain"
    /\ envoys = 0

\* claim:Council.Hold
Council_Hold ==
    /\ state = "Research"
    /\ state' = "Council"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.Close
Envoy_Close ==
    /\ state = "Reported"
    /\ state' = "Closed"
    /\ envoys' = 0
    /\ UNCHANGED <<level>>

\* claim:Envoy.Decide
Envoy_Decide ==
    /\ state = "Need"
    /\ state' = "Decided"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.InThread
Envoy_InThread ==
    /\ state = "Need"
    /\ state' = "AskedInThread"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.Open
Envoy_Open ==
    /\ state = "Research"
    /\ state' = "Queued"
    /\ level' = "coordinator"
    /\ UNCHANGED <<envoys>>

\* claim:Envoy.PassUp
Envoy_PassUp ==
    /\ state = "Need"
    /\ state' = "Research"
    /\ level' \in {"captain", "coordinator"}
    /\ UNCHANGED <<envoys>>

\* claim:Envoy.Report
Envoy_Report ==
    /\ state = "Active"
    /\ state' = "Reported"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.Send
Envoy_Send ==
    /\ state = "Queued"
    /\ envoys = 0
    /\ state' = "Active"
    /\ envoys' = 1
    /\ UNCHANGED <<level>>

\* claim:JobTickets.Research
JobTickets_Research ==
    /\ state = "Research"
    /\ state' = "Answered"
    /\ UNCHANGED <<level, envoys>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Council_Hold
    \/ Envoy_Close
    \/ Envoy_Decide
    \/ Envoy_InThread
    \/ Envoy_Open
    \/ Envoy_PassUp
    \/ Envoy_Report
    \/ Envoy_Send
    \/ JobTickets_Research
    \/ Done

Bounds == level \in {"captain", "coordinator"} /\ envoys \in {0, 1}

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

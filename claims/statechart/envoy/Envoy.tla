\* built from the model; edit there: tools/SkillBuild
---- MODULE Envoy ----

VARIABLES state, level, envoys

vars == <<state, level, envoys>>

Finals == {"Decided", "AskedInThread", "ToTribunal", "Closed", "MovedOn"}

Init ==
    /\ state = "Need"
    /\ level = "captain"
    /\ envoys = 0

\* claim:Envoy.Close
Envoy_Close ==
    /\ state = "Reported"
    /\ state' = "Closed"
    /\ envoys' = 0
    /\ UNCHANGED <<level>>

\* claim:Envoy.Decide
Envoy_Decide ==
    /\ state = "Homework"
    /\ state' = "Decided"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.Hard
Envoy_Hard ==
    /\ state = "Homework"
    /\ level = "admiral"
    /\ state' = "ToTribunal"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.InThread
Envoy_InThread ==
    /\ state = "Homework"
    /\ state' = "AskedInThread"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.MovedOn
Envoy_MovedOn ==
    /\ state = "Reported"
    /\ state' = "MovedOn"
    /\ envoys' = 0
    /\ UNCHANGED <<level>>

\* claim:Envoy.NextLevel
Envoy_NextLevel ==
    /\ state = "PassedUp"
    /\ state' = "Homework"
    /\ level' = "admiral"
    /\ UNCHANGED <<envoys>>

\* claim:Envoy.Open
Envoy_Open ==
    /\ state = "Homework"
    /\ level = "admiral"
    /\ state' = "Queued"
    /\ UNCHANGED <<level, envoys>>

\* claim:Envoy.PassUp
Envoy_PassUp ==
    /\ state = "Homework"
    /\ level = "captain"
    /\ state' = "PassedUp"
    /\ UNCHANGED <<level, envoys>>

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

\* claim:Envoy.Take
Envoy_Take ==
    /\ state = "Need"
    /\ state' = "Homework"
    /\ level' \in {"captain", "admiral"}
    /\ UNCHANGED <<envoys>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Envoy_Close
    \/ Envoy_Decide
    \/ Envoy_Hard
    \/ Envoy_InThread
    \/ Envoy_MovedOn
    \/ Envoy_NextLevel
    \/ Envoy_Open
    \/ Envoy_PassUp
    \/ Envoy_Report
    \/ Envoy_Send
    \/ Envoy_Take
    \/ Done

Bounds == level \in {"captain", "admiral"} /\ envoys \in {0, 1}

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

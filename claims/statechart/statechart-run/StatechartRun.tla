---- MODULE StatechartRun ----

VARIABLES state

vars == <<state>>

Finals == {"InvalidOverride", "NonSuccess", "Charted", "ScratchKept"}

Init ==
    /\ state = "Invoked"

\* claim:ChartTool.Authorize
ChartTool_Authorize ==
    /\ state = "ApprovalRequired"
    /\ state' = "Continuing"

\* claim:ChartTool.Check
ChartTool_Check ==
    /\ state = "Ready"
    /\ state' = "Checking"

\* claim:ChartTool.Deny
ChartTool_Deny ==
    /\ state = "ApprovalRequired"
    /\ state' = "NonSuccess"

\* claim:ChartTool.Drained
ChartTool_Drained ==
    /\ state = "Checking"
    /\ state' = "Charted"

\* claim:ChartTool.InstallFails
ChartTool_InstallFails ==
    /\ state = "Continuing"
    /\ state' = "NonSuccess"

\* claim:ChartTool.Installed
ChartTool_Installed ==
    /\ state = "Continuing"
    /\ state' = "Ready"

\* claim:ChartTool.Invalid
ChartTool_Invalid ==
    /\ state = "Invoked"
    /\ state' = "InvalidOverride"

\* claim:ChartTool.Missing
ChartTool_Missing ==
    /\ state = "Invoked"
    /\ state' = "ApprovalRequired"

\* claim:ChartTool.Ready
ChartTool_Ready ==
    /\ state = "Invoked"
    /\ state' = "Ready"

\* claim:ChartTool.Uncertain
ChartTool_Uncertain ==
    /\ state = "Checking"
    /\ state' = "ScratchKept"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ ChartTool_Authorize
    \/ ChartTool_Check
    \/ ChartTool_Deny
    \/ ChartTool_Drained
    \/ ChartTool_InstallFails
    \/ ChartTool_Installed
    \/ ChartTool_Invalid
    \/ ChartTool_Missing
    \/ ChartTool_Ready
    \/ ChartTool_Uncertain
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

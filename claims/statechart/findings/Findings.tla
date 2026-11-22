---- MODULE Findings ----

VARIABLES state

vars == <<state>>

Finals == {"Counted", "Filed", "Acted"}

Init ==
    /\ state = "Watch"

\* claim:Findings.BatchShoulds
Findings_BatchShoulds ==
    /\ state = "StaleShoulds"
    /\ state' = "Acted"

\* claim:Findings.Count
Findings_Count ==
    /\ state = "Flake"
    /\ state' = "Counted"

\* claim:Findings.DeleteSpec
Findings_DeleteSpec ==
    /\ state = "LandedSpec"
    /\ state' = "Acted"

\* claim:Findings.FileIt
Findings_FileIt ==
    /\ state = "DytestNeed"
    /\ state' = "Filed"

\* claim:Findings.FixLink
Findings_FixLink ==
    /\ state = "SharedLink"
    /\ state' = "Acted"

\* claim:Findings.LandedSpec
Findings_LandedSpec ==
    /\ state = "Board"
    /\ state' = "LandedSpec"

\* claim:Findings.Need
Findings_Need ==
    /\ state = "Watch"
    /\ state' = "DytestNeed"

\* claim:Findings.Noise
Findings_Noise ==
    /\ state = "Watch"
    /\ state' = "Flake"

\* claim:Findings.Pull
Findings_Pull ==
    /\ state = "Routed"
    /\ state' = "Acted"

\* claim:Findings.ReadBoard
Findings_ReadBoard ==
    /\ state = "Watch"
    /\ state' = "Board"

\* claim:Findings.RealRed
Findings_RealRed ==
    /\ state = "Watch"
    /\ state' = "CiRed"

\* claim:Findings.Repeat
Findings_Repeat ==
    /\ state = "Flake"
    /\ state' = "Repeated"

\* claim:Findings.Route
Findings_Route ==
    /\ state = "CiRed"
    /\ state' = "Routed"

\* claim:Findings.Rule
Findings_Rule ==
    /\ state = "Weed"
    /\ state' = "Acted"

\* claim:Findings.SharedLink
Findings_SharedLink ==
    /\ state = "Board"
    /\ state' = "SharedLink"

\* claim:Findings.StaleShoulds
Findings_StaleShoulds ==
    /\ state = "Board"
    /\ state' = "StaleShoulds"

\* claim:Findings.Ticket
Findings_Ticket ==
    /\ state = "Repeated"
    /\ state' = "CiRed"

\* claim:Findings.Weed
Findings_Weed ==
    /\ state = "Watch"
    /\ state' = "Weed"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Findings_BatchShoulds
    \/ Findings_Count
    \/ Findings_DeleteSpec
    \/ Findings_FileIt
    \/ Findings_FixLink
    \/ Findings_LandedSpec
    \/ Findings_Need
    \/ Findings_Noise
    \/ Findings_Pull
    \/ Findings_ReadBoard
    \/ Findings_RealRed
    \/ Findings_Repeat
    \/ Findings_Route
    \/ Findings_Rule
    \/ Findings_SharedLink
    \/ Findings_StaleShoulds
    \/ Findings_Ticket
    \/ Findings_Weed
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

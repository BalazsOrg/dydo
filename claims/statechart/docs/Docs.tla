---- MODULE Docs ----

VARIABLES state

vars == <<state>>

Finals == {"NotWritten", "CutDoc"}

Init ==
    /\ state = "Start"

\* claim:Docs.Admit
Docs_Admit ==
    /\ state = "Invited"
    /\ state' = "Admitted"

\* claim:Docs.CutDrifted
Docs_CutDrifted ==
    /\ state = "Grandfathered"
    /\ state' = "CutDoc"

\* claim:Docs.Declare
Docs_Declare ==
    /\ state = "Admitted"
    /\ state' = "Declared"

\* claim:Docs.Existing
Docs_Existing ==
    /\ state = "Start"
    /\ state' = "Grandfathered"

\* claim:Docs.FixDrifted
Docs_FixDrifted ==
    /\ state = "Grandfathered"
    /\ state' = "Declared"

\* claim:Docs.FixFinding
Docs_FixFinding ==
    /\ state = "Finding"
    /\ state' = "Declared"

\* claim:Docs.Flag
Docs_Flag ==
    /\ state = "Declared"
    /\ state' = "Flagged"

\* claim:Docs.GivePaths
Docs_GivePaths ==
    /\ state = "Grandfathered"
    /\ state' = "Declared"

\* claim:Docs.Hold
Docs_Hold ==
    /\ state = "Flagged"
    /\ state' = "Declared"

\* claim:Docs.Invite
Docs_Invite ==
    /\ state = "Proposed"
    /\ state' = "Invited"

\* claim:Docs.New
Docs_New ==
    /\ state = "Start"
    /\ state' = "Proposed"

\* claim:Docs.Refuse
Docs_Refuse ==
    /\ state = "Invited"
    /\ state' = "NotWritten"

\* claim:Docs.ReviewFinds
Docs_ReviewFinds ==
    /\ state = "Declared"
    /\ state' = "Finding"

\* claim:Docs.Uninvited
Docs_Uninvited ==
    /\ state = "Proposed"
    /\ state' = "NotWritten"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Docs_Admit
    \/ Docs_CutDrifted
    \/ Docs_Declare
    \/ Docs_Existing
    \/ Docs_FixDrifted
    \/ Docs_FixFinding
    \/ Docs_Flag
    \/ Docs_GivePaths
    \/ Docs_Hold
    \/ Docs_Invite
    \/ Docs_New
    \/ Docs_Refuse
    \/ Docs_ReviewFinds
    \/ Docs_Uninvited
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

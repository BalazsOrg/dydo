\* built from the model; edit there: tools/SkillBuild
---- MODULE Backfill ----

VARIABLES state

vars == <<state>>

Finals == {"Merged", "Rejected"}

Init ==
    /\ state = "Untagged"

\* claim:Backfill.Approve
Backfill_Approve ==
    /\ state = "OnPage"
    /\ state' = "Merged"

\* claim:Backfill.Check
Backfill_Check ==
    /\ state = "Drafted"
    /\ state' = "Checked"

\* claim:Backfill.Draft
Backfill_Draft ==
    /\ state = "Untagged"
    /\ state' = "Drafted"

\* claim:Backfill.Page
Backfill_Page ==
    /\ state = "Checked"
    /\ state' = "OnPage"

\* claim:Backfill.Reject
Backfill_Reject ==
    /\ state = "OnPage"
    /\ state' = "Rejected"

\* claim:Backfill.Retag
Backfill_Retag ==
    /\ state = "OnPage"
    /\ state' = "Drafted"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Backfill_Approve
    \/ Backfill_Check
    \/ Backfill_Draft
    \/ Backfill_Page
    \/ Backfill_Reject
    \/ Backfill_Retag
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

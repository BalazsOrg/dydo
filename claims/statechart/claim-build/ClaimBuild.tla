---- MODULE ClaimBuild ----

VARIABLES state

vars == <<state>>

Finals == {"Released"}

Init ==
    /\ state = "Edited"

\* claim:Build.Approve
Build_Approve ==
    /\ state = "InReview"
    /\ state' = "Approved"

\* claim:Build.EditThere
Build_EditThere ==
    /\ state = "Nudged"
    /\ state' = "Edited"

\* claim:Build.Golden
Build_Golden ==
    /\ state = "Built"
    /\ state' = "Rejected"

\* claim:Build.HandEdit
Build_HandEdit ==
    /\ state = "Built"
    /\ state' = "Nudged"

\* claim:Build.Mend
Build_Mend ==
    /\ state = "Rejected"
    /\ state' = "Edited"

\* claim:Build.Pass
Build_Pass ==
    /\ state = "Built"
    /\ state' = "Checked"

\* claim:Build.Read
Build_Read ==
    /\ state = "TlcPassed"
    /\ state' = "InReview"

\* claim:Build.Refused
Build_Refused ==
    /\ state = "Edited"
    /\ state' = "Rejected"

\* claim:Build.Rejected
Build_Rejected ==
    /\ state = "InReview"
    /\ state' = "Edited"

\* claim:Build.Remodel
Build_Remodel ==
    /\ state = "TlcFailed"
    /\ state' = "Edited"

\* claim:Build.Ship
Build_Ship ==
    /\ state = "Approved"
    /\ state' = "Released"

\* claim:Build.Tlc
Build_Tlc ==
    /\ state = "Checked"
    /\ state' = "TlcPassed"

\* claim:Build.TlcFail
Build_TlcFail ==
    /\ state = "Checked"
    /\ state' = "TlcFailed"

\* claim:Build.Write
Build_Write ==
    /\ state = "Edited"
    /\ state' = "Built"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Build_Approve
    \/ Build_EditThere
    \/ Build_Golden
    \/ Build_HandEdit
    \/ Build_Mend
    \/ Build_Pass
    \/ Build_Read
    \/ Build_Refused
    \/ Build_Rejected
    \/ Build_Remodel
    \/ Build_Ship
    \/ Build_Tlc
    \/ Build_TlcFail
    \/ Build_Write
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

---- MODULE Law ----

VARIABLES state

vars == <<state>>

Finals == {"Mechanical", "Enacted", "Retired", "CaseLaw"}

Init ==
    /\ state = "Evidence"

\* claim:Law.Approve
Law_Approve ==
    /\ state = "CaseStudy"
    /\ state' = "CaseLaw"

\* claim:Law.Departs
Law_Departs ==
    /\ state = "Evidence"
    /\ state' = "Candidate"

\* claim:Law.Enact
Law_Enact ==
    /\ state = "Proposed"
    /\ state' = "Enacted"

\* claim:Law.Mechanical
Law_Mechanical ==
    /\ state = "Candidate"
    /\ state' = "Mechanical"

\* claim:Law.Precedential
Law_Precedential ==
    /\ state = "Evidence"
    /\ state' = "CaseStudy"

\* claim:Law.Propose
Law_Propose ==
    /\ state = "Candidate"
    /\ state' = "Proposed"

\* claim:Law.Retire
Law_Retire ==
    /\ state = "Enacted"
    /\ state' = "Retired"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Law_Approve
    \/ Law_Departs
    \/ Law_Enact
    \/ Law_Mechanical
    \/ Law_Precedential
    \/ Law_Propose
    \/ Law_Retire
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

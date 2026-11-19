\* built from the model; edit there: tools/SkillBuild
---- MODULE Overview ----

VARIABLES state

vars == <<state>>

Finals == {"AdopterArea"}

Init ==
    /\ state = "ClaimArea"

\* claim:Overview.Accept
Overview_Accept ==
    /\ state = "LandingArea"
    /\ state' = "MasterArea"

\* claim:Overview.Build
Overview_Build ==
    /\ state = "ClaimArea"
    /\ state' = "HostArea"

\* claim:Overview.Cut
Overview_Cut ==
    /\ state = "ProjectArea"
    /\ state' = "TicketArea"

\* claim:Overview.Decided
Overview_Decided ==
    /\ state = "JudgementArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Explain
Overview_Explain ==
    /\ state = "ProofArea"
    /\ state' = "DocsArea"

\* claim:Overview.Explained
Overview_Explained ==
    /\ state = "DocsArea"
    /\ state' = "ProofArea"

\* claim:Overview.GiveOut
Overview_GiveOut ==
    /\ state = "StaffArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Hold
Overview_Hold ==
    /\ state = "HostArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Land
Overview_Land ==
    /\ state = "ProjectArea"
    /\ state' = "LandingArea"

\* claim:Overview.List
Overview_List ==
    /\ state = "ProjectArea"
    /\ state' = "StaffArea"

\* claim:Overview.Merge
Overview_Merge ==
    /\ state = "ReviewArea"
    /\ state' = "MergeArea"

\* claim:Overview.Need
Overview_Need ==
    /\ state = "ProjectArea"
    /\ state' = "JudgementArea"

\* claim:Overview.Next
Overview_Next ==
    /\ state = "MergeArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Proposed
Overview_Proposed ==
    /\ state = "JudgementArea"
    /\ state' = "StaffArea"

\* claim:Overview.Prove
Overview_Prove ==
    /\ state = "TicketArea"
    /\ state' = "ProofArea"

\* claim:Overview.Ratify
Overview_Ratify ==
    /\ state = "StaffArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Refix
Overview_Refix ==
    /\ state = "LandingArea"
    /\ state' = "TicketArea"

\* claim:Overview.Review
Overview_Review ==
    /\ state = "ProofArea"
    /\ state' = "ReviewArea"

\* claim:Overview.Route
Overview_Route ==
    /\ state = "WatchArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Ship
Overview_Ship ==
    /\ state = "Tagged"
    /\ state' = "AdopterArea"

\* claim:Overview.Tag
Overview_Tag ==
    /\ state = "MasterArea"
    /\ state' = "Tagged"

\* claim:Overview.Watch
Overview_Watch ==
    /\ state = "ProjectArea"
    /\ state' = "WatchArea"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Overview_Accept
    \/ Overview_Build
    \/ Overview_Cut
    \/ Overview_Decided
    \/ Overview_Explain
    \/ Overview_Explained
    \/ Overview_GiveOut
    \/ Overview_Hold
    \/ Overview_Land
    \/ Overview_List
    \/ Overview_Merge
    \/ Overview_Need
    \/ Overview_Next
    \/ Overview_Proposed
    \/ Overview_Prove
    \/ Overview_Ratify
    \/ Overview_Refix
    \/ Overview_Review
    \/ Overview_Route
    \/ Overview_Ship
    \/ Overview_Tag
    \/ Overview_Watch
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

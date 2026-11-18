\* built from the model; edit there: tools/SkillBuild
---- MODULE Overview ----

VARIABLES state

vars == <<state>>

Finals == {"AdopterArea"}

Init ==
    /\ state = "ClaimArea"

\* claim:Overview.Accept
Overview_Accept ==
    /\ state = "MutationArea"
    /\ state' = "MasterArea"

\* claim:Overview.Archive
Overview_Archive ==
    /\ state = "MasterArea"
    /\ state' = "SafeDeleteArea"

\* claim:Overview.Build
Overview_Build ==
    /\ state = "ClaimArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Cut
Overview_Cut ==
    /\ state = "ProjectArea"
    /\ state' = "DispatchArea"

\* claim:Overview.Explain
Overview_Explain ==
    /\ state = "ProofArea"
    /\ state' = "DocsArea"

\* claim:Overview.Feed
Overview_Feed ==
    /\ state = "FindingsArea"
    /\ state' = "ProjectArea"

\* claim:Overview.Findings
Overview_Findings ==
    /\ state = "MergeArea"
    /\ state' = "FindingsArea"

\* claim:Overview.Hard
Overview_Hard ==
    /\ state = "EnvoyArea"
    /\ state' = "LawArea"

\* claim:Overview.Hops
Overview_Hops ==
    /\ state = "StatusArea"
    /\ state' = "HopArea"

\* claim:Overview.Land
Overview_Land ==
    /\ state = "StaffArea"
    /\ state' = "MutationArea"

\* claim:Overview.Merge
Overview_Merge ==
    /\ state = "ShouldArea"
    /\ state' = "MergeArea"

\* claim:Overview.Need
Overview_Need ==
    /\ state = "LivenessArea"
    /\ state' = "EnvoyArea"

\* claim:Overview.Night
Overview_Night ==
    /\ state = "LawArea"
    /\ state' = "StaffArea"

\* claim:Overview.Prove
Overview_Prove ==
    /\ state = "HopArea"
    /\ state' = "ProofArea"

\* claim:Overview.Quiet
Overview_Quiet ==
    /\ state = "FindingsArea"
    /\ state' = "LivenessArea"

\* claim:Overview.Review
Overview_Review ==
    /\ state = "DocsArea"
    /\ state' = "ReviewArea"

\* claim:Overview.Ship
Overview_Ship ==
    /\ state = "Tagged"
    /\ state' = "AdopterArea"

\* claim:Overview.Shoulds
Overview_Shoulds ==
    /\ state = "ReviewArea"
    /\ state' = "ShouldArea"

\* claim:Overview.Start
Overview_Start ==
    /\ state = "DispatchArea"
    /\ state' = "StatusArea"

\* claim:Overview.Tag
Overview_Tag ==
    /\ state = "SafeDeleteArea"
    /\ state' = "Tagged"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Overview_Accept
    \/ Overview_Archive
    \/ Overview_Build
    \/ Overview_Cut
    \/ Overview_Explain
    \/ Overview_Feed
    \/ Overview_Findings
    \/ Overview_Hard
    \/ Overview_Hops
    \/ Overview_Land
    \/ Overview_Merge
    \/ Overview_Need
    \/ Overview_Night
    \/ Overview_Prove
    \/ Overview_Quiet
    \/ Overview_Review
    \/ Overview_Ship
    \/ Overview_Shoulds
    \/ Overview_Start
    \/ Overview_Tag
    \/ Done

Spec == Init /\ [][Next]_vars
====

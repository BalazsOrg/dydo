\* built from the model; edit there: tools/SkillBuild
---- MODULE Dispatch ----

VARIABLES state

vars == <<state>>

Finals == {"Returned", "CouncilCalled", "SelfAuthored"}

Init ==
    /\ state = "Staffing"

\* claim:Dispatch.Council
Dispatch_Council ==
    /\ state = "Staffing"
    /\ state' = "CouncilCalled"

\* claim:Dispatch.ReCut
Dispatch_ReCut ==
    /\ state = "ReCut"
    /\ state' = "Staffing"

\* claim:Dispatch.Return
Dispatch_Return ==
    /\ state = "Working"
    /\ state' = "Returned"

\* claim:Dispatch.SelfAuthor
Dispatch_SelfAuthor ==
    /\ state = "Staffing"
    /\ state' = "SelfAuthored"

\* claim:Dispatch.Send
Dispatch_Send ==
    /\ state = "Staffing"
    /\ state' = "Briefed"

\* claim:Dispatch.Start
Dispatch_Start ==
    /\ state = "Briefed"
    /\ state' = "Working"

\* claim:Dispatch.TooBig
Dispatch_TooBig ==
    /\ state = "Briefed"
    /\ state' = "ReCut"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Dispatch_Council
    \/ Dispatch_ReCut
    \/ Dispatch_Return
    \/ Dispatch_SelfAuthor
    \/ Dispatch_Send
    \/ Dispatch_Start
    \/ Dispatch_TooBig
    \/ Done

Spec == Init /\ [][Next]_vars
====

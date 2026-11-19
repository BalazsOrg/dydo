\* built from the model; edit there: tools/SkillBuild
---- MODULE HostSetup ----

VARIABLES state

vars == <<state>>

Finals == {}

Init ==
    /\ state = "Uninitialised"

\* claim:Host.Background
Host_Background ==
    /\ state = "Initialised"
    /\ state' = "CodexBackground"

\* claim:Host.Call
Host_Call ==
    /\ state = "Initialised"
    /\ state' = "Guarded"

\* claim:Host.Deny
Host_Deny ==
    /\ state = "Guarded"
    /\ state' = "MemoryDenied"

\* claim:Host.FlagOff
Host_FlagOff ==
    /\ state = "CodexBackground"
    /\ state' = "Initialised"

\* claim:Host.Init
Host_Init ==
    /\ state = "Uninitialised"
    /\ state' = "Initialised"

\* claim:Host.Pass
Host_Pass ==
    /\ state = "Guarded"
    /\ state' = "Initialised"

\* claim:Host.Refused
Host_Refused ==
    /\ state = "MemoryDenied"
    /\ state' = "Initialised"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Host_Background
    \/ Host_Call
    \/ Host_Deny
    \/ Host_FlagOff
    \/ Host_Init
    \/ Host_Pass
    \/ Host_Refused
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

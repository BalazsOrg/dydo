\* built from the model; edit there: tools/SkillBuild
---- MODULE IgnoreEntry ----

VARIABLES state

vars == <<state>>

Finals == {"Split", "Ignored", "Stale"}

Init ==
    /\ state = "OverLimit"

\* claim:IgnoreEntry.Authored
IgnoreEntry_Authored ==
    /\ state = "OverLimit"
    /\ state' = "OwnTicket"

\* claim:IgnoreEntry.Bless
IgnoreEntry_Bless ==
    /\ state = "CouncilB"
    /\ state' = "Ignored"

\* claim:IgnoreEntry.CheckA
IgnoreEntry_CheckA ==
    /\ state = "PathA"
    /\ state' = "Ignored"

\* claim:IgnoreEntry.Convene
IgnoreEntry_Convene ==
    /\ state = "OwnTicket"
    /\ state' = "CouncilB"

\* claim:IgnoreEntry.Generated
IgnoreEntry_Generated ==
    /\ state = "OverLimit"
    /\ state' = "PathA"

\* claim:IgnoreEntry.GoStale
IgnoreEntry_GoStale ==
    /\ state = "Ignored"
    /\ state' = "Stale"

\* claim:IgnoreEntry.Split
IgnoreEntry_Split ==
    /\ state = "OverLimit"
    /\ state' = "Split"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ IgnoreEntry_Authored
    \/ IgnoreEntry_Bless
    \/ IgnoreEntry_CheckA
    \/ IgnoreEntry_Convene
    \/ IgnoreEntry_Generated
    \/ IgnoreEntry_GoStale
    \/ IgnoreEntry_Split
    \/ Done

Spec == Init /\ [][Next]_vars
====

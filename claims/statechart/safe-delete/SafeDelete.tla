\* built from the model; edit there: tools/SkillBuild
---- MODULE SafeDelete ----

VARIABLES state

vars == <<state>>

Finals == {"Purged"}

Init ==
    /\ state = "Live"

\* claim:SafeDelete.Archive
SafeDelete_Archive ==
    /\ state = "Live"
    /\ state' = "Archived"

\* claim:SafeDelete.ArchiveT3
SafeDelete_ArchiveT3 ==
    /\ state = "Live"
    /\ state' = "Archived"

\* claim:SafeDelete.Purge
SafeDelete_Purge ==
    /\ state = "Archived"
    /\ state' = "Purged"

\* claim:SafeDelete.Restore
SafeDelete_Restore ==
    /\ state = "Archived"
    /\ state' = "Live"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ SafeDelete_Archive
    \/ SafeDelete_ArchiveT3
    \/ SafeDelete_Purge
    \/ SafeDelete_Restore
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

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

\* GAP: N10 names no restore from the archive; the built command has one
Gap1 ==
    /\ state = "Archived"
    /\ state' = "Live"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ SafeDelete_Archive
    \/ SafeDelete_ArchiveT3
    \/ SafeDelete_Purge
    \/ Gap1
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

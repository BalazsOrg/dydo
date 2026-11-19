\* built from the model; edit there: tools/SkillBuild
---- MODULE Tribunal ----

VARIABLES state, night, twoWay, drGrade, blocking, agree, oneFamily

vars == <<state, night, twoWay, drGrade, blocking, agree, oneFamily>>

Finals == {"DrSettled", "DecidedOvernight", "Ratified", "Scored", "Investigated"}

Open == {"GAP 1"}

Init ==
    /\ state = "HardCall"
    /\ night = FALSE
    /\ twoWay = FALSE
    /\ drGrade = FALSE
    /\ blocking = FALSE
    /\ agree = FALSE
    /\ oneFamily = FALSE

\* claim:Tribunal.DecideNight
Tribunal_DecideNight ==
    /\ state = "Exchange"
    /\ night /\ agree /\ twoWay /\ ~drGrade
    /\ state' = "DecidedOvernight"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* claim:Tribunal.Exchange
Tribunal_Exchange ==
    /\ state = "Sealed"
    /\ state' = "Exchange"
    /\ agree' \in BOOLEAN
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, oneFamily>>

\* claim:Tribunal.File
Tribunal_File ==
    /\ state = "HardCall"
    /\ state' = "CaseFile"
    /\ night' \in BOOLEAN /\ twoWay' \in BOOLEAN /\ drGrade' \in BOOLEAN /\ blocking' \in BOOLEAN
    /\ UNCHANGED <<agree, oneFamily>>

\* claim:Tribunal.OneFamily
Tribunal_OneFamily ==
    /\ state = "CaseFile"
    /\ state' = "Sealed"
    /\ oneFamily' = TRUE
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree>>

\* claim:Tribunal.Prepare
Tribunal_Prepare ==
    /\ state = "Exchange"
    /\ ~(night /\ agree /\ twoWay) \/ (drGrade /\ ~blocking)
    /\ state' = "Prepared"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* claim:Tribunal.ProposeDr
Tribunal_ProposeDr ==
    /\ state = "Exchange"
    /\ night /\ agree /\ twoWay /\ drGrade /\ blocking
    /\ state' = "ProposedDr"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* claim:Tribunal.Ratify
Tribunal_Ratify ==
    /\ state = "ProposedDr"
    /\ state' = "Ratified"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* claim:Tribunal.Score
Tribunal_Score ==
    /\ state = "Prepared"
    /\ state' = "Scored"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* claim:Tribunal.Seat
Tribunal_Seat ==
    /\ state = "CaseFile"
    /\ state' = "Sealed"
    /\ oneFamily' = FALSE
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree>>

\* claim:Tribunal.Settled
Tribunal_Settled ==
    /\ state = "HardCall"
    /\ state' = "DrSettled"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* claim:Tribunal.Wrong
Tribunal_Wrong ==
    /\ state = "DecidedOvernight"
    /\ state' = "Investigated"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

\* GAP: L21 has him ratify a proposed DR but gives no route when he declines it
Gap1 ==
    /\ state = "ProposedDr"
    /\ state' = "GAP 1"
    /\ UNCHANGED <<night, twoWay, drGrade, blocking, agree, oneFamily>>

Done == state \in Finals \cup Open /\ UNCHANGED vars

Next ==
    \/ Tribunal_DecideNight
    \/ Tribunal_Exchange
    \/ Tribunal_File
    \/ Tribunal_OneFamily
    \/ Tribunal_Prepare
    \/ Tribunal_ProposeDr
    \/ Tribunal_Ratify
    \/ Tribunal_Score
    \/ Tribunal_Seat
    \/ Tribunal_Settled
    \/ Tribunal_Wrong
    \/ Gap1
    \/ Done

Bounds == night \in BOOLEAN /\ twoWay \in BOOLEAN /\ drGrade \in BOOLEAN /\ blocking \in BOOLEAN /\ agree \in BOOLEAN /\ oneFamily \in BOOLEAN

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

\* built from the model; edit there: tools/SkillBuild
---- MODULE Council ----

VARIABLES state

vars == <<state>>

Finals == {"Synthesised"}

Init ==
    /\ state = "Convened"

\* claim:Council.Answer
Council_Answer ==
    /\ state = "Convened"
    /\ state' = "Answered"

\* claim:Council.FamilyOut
Council_FamilyOut ==
    /\ state = "Convened"
    /\ state' = "Waiting"

\* claim:Council.Proceed
Council_Proceed ==
    /\ state = "Waiting"
    /\ state' = "Answered"

\* claim:Council.Synthesise
Council_Synthesise ==
    /\ state = "Answered"
    /\ state' = "Synthesised"

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Council_Answer
    \/ Council_FamilyOut
    \/ Council_Proceed
    \/ Council_Synthesise
    \/ Done

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

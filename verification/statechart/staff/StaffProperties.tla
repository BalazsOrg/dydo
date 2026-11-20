---- MODULE StaffProperties ----
EXTENDS Staff

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* H7: The daytime investigation returns to daytime staff duty.
H7_InvestigationReturns == [][((state = "Investigating" /\ state' # state) => state' = "Day")]_<<vars, seen>>

\* L19: The morning brief returns to duty, not a Question-ratification phase.
L19_BriefDoesNotRatify == [][((state = "MorningBrief" /\ state' # state) => state' = "Day")]_<<vars, seen>>

\* H7 (DYD-1082 555a8300 gap 6): the night starts with his muster.
H7_NightStartsAtMuster == [][((state' = "NightWatch" /\ state # state') => state = "Muster")]_<<vars, seen>>

\* H7 (DYD-1082 555a8300 gap 7): the night ends on his return, and the morning brief closes it.
\* His return itself is unmodeled.
H7_NightEndsInMorningBrief == [][((state = "NightWatch" /\ state' # state) => state' = "MorningBrief")]_<<vars, seen>>

====

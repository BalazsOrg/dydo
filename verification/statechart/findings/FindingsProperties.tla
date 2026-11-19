---- MODULE FindingsProperties ----
EXTENDS Findings

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* Y9: A flake goes to counting or the repeated-flake route; count/threshold data is unmodeled.
Y9_CountFlakes == [][((state = "Flake" /\ state' # state) => state' \in {"Counted", "Repeated"})]_<<vars, seen>>

\* Y1, Y3: Needs from DYTEST are filed in its project.
Y3_FileInDytest == [][((state = "DytestNeed" /\ state' # state) => state' = "Filed")]_<<vars, seen>>

\* Y9, H11: Real CI findings are routed to their owning officer.
H11_RouteRealRed == [][((state = "CiRed" /\ state' # state) => state' = "Routed")]_<<vars, seen>>

====

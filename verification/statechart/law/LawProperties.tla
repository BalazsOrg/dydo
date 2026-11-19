---- MODULE LawProperties ----
EXTENDS Law

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* L13: A principle is proposed to him before enactment.
L13_ProposeBeforeEnact == state = "Enacted" => "Proposed" \in seen

\* L9: A case study is prepared before his approval.
L9_PrepareBeforeCaseLaw == state = "CaseLaw" => "CaseStudy" \in seen

\* L11: A retired principle has an enacted history.
L11_EnactBeforeRetire == state = "Retired" => "Enacted" \in seen

====

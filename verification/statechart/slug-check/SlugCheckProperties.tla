---- MODULE SlugCheckProperties ----
EXTENDS SlugCheck

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* L8: A failed slug check returns through a written correction.
L8_InvalidMustBeRechecked == [][((state = "Invalid" /\ state' # state) => state' = "Written")]_<<vars, seen>>

\* L8: A valid result ends this check invocation.
L8_ValidIsResult == [][((state = "Valid") => state' = "Valid")]_<<vars, seen>>

====

---- MODULE LivenessProperties ----
EXTENDS Liveness

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* S24: A stopped-hop restart follows a liveness finding.
S24_CheckBeforeRestart == [][((state' = "Restarted" /\ state # state') => state = "Quiet")]_<<vars, seen>>

\* S24, K28: A quiet finding is eventually healed or redispatched.
S24_QuietEventuallyHealed == (state = "Quiet") ~> (state = "Live")

\* S24, K28: A stopped hop reset to Todo is eventually resumed or redispatched.
S24_StoppedEventuallyResumed == (state = "Restarted") ~> (state = "Live")

====

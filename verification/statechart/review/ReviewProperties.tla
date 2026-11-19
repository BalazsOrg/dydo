---- MODULE ReviewProperties ----
EXTENDS Review

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* R8, R11: A review phase cannot end with an outstanding blocker.
R8_NoBlockersAtEnd == state = "Ended" => kind = "none"

\* R9: A security fix prioritises security.
R9_SecurityFix == state = "Fixing" /\ kind = "security" => securityFirst

\* R10: A blocker at the checkpoint belongs to round three.
R10_ThirdRoundCheckpoint == state = "Checkpoint" => round = 3 /\ kind # "none"

\* R9: A fundamental blocker is the replan route.
R9_FundamentalReplan == state = "Replan" => kind = "fundamental"

\* R9, R10: Escalation carries a human/fundamental blocker; this does not test immediacy.
R9_EscalationKinds == state = "Escalated" => kind \in {"balazs", "fundamental"}

\* R8: A blocked round requires a fix before the next round.
R8_FixBetweenBlockedRounds == [][((state' = "Round" /\ round' > round /\ kind # "none") => state = "Fixing")]_<<vars, seen>>

\* R10: A round-three fix is preceded by the checkpoint.
R10_CheckpointBeforeFourthFix == state = "Fixing" /\ round = 3 => "Checkpoint" \in seen

====

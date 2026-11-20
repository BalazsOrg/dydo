---- MODULE TribunalProperties ----
EXTENDS Tribunal

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* L18: Every exit of a council follows the sealed positions and the one exchange: decided, a
\* proposed DR, or prepared for him with both positions. A his-call child reaches Prepared straight
\* from the Question's research, with no case file and no council (H23, DYD-1195 87c1e2e6), so only
\* that route to Prepared is exempt.
L18_ExchangeBeforeRuling ==
    (state \in {"DecidedOvernight", "ProposedDr"} \/ (state = "Prepared" /\ "CaseFile" \in seen))
        => "Exchange" \in seen

\* L18, L20: Only agreement on a two-way call that is not DR-grade permits a provisional decision,
\* overnight or by day for a call not worth his time (DYD-1195 87c1e2e6).
L18_AgreedTwoWayProvisional == state = "DecidedOvernight" => twoWay /\ agree /\ ~drGrade

\* L21: A proposed DR requires night, two-way, serious blocking and agreement.
L21_RareProposedDr == state = "ProposedDr" => night /\ twoWay /\ blocking /\ agree /\ drGrade

\* L19, L21: Ratification applies to proposed DRs, never decided Questions.
L21_OnlyProposedDrRatified == state = "Ratified" => "ProposedDr" \in seen /\ "DecidedOvernight" \notin seen

\* L20: Scoring follows preparation for his decision.
L20_PreparedBeforeScore == state = "Scored" => "Prepared" \in seen

\* L19: An investigated wrong Question had a provisional ruling.
L19_InvestigateProvisional == state = "Investigated" => "DecidedOvernight" \in seen

\* L21: Only a proposed DR is declined, and a declined one was never ratified.
L21_DeclineOnlyProposed == state = "Declined" => "ProposedDr" \in seen /\ "Ratified" \notin seen

====

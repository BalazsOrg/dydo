---- MODULE TribunalProperties ----
EXTENDS Tribunal

\* Hand-written from the spec; never generated from the claim model (K3).
\* seen is a passive history observer. Every model step has exactly one extension.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* L18: A tribunal result follows the sealed positions and exchange.
L18_ExchangeBeforeRuling == state \in {"DecidedOvernight", "Prepared", "ProposedDr"} => "Exchange" \in seen

\* L18, L20: Only night, agreement and two-way permit a provisional Question decision.
L20_NightOnlyProvisional == state = "DecidedOvernight" => night /\ twoWay /\ agree /\ ~drGrade

\* L21: A proposed DR requires night, two-way, serious blocking and agreement.
L21_RareProposedDr == state = "ProposedDr" => night /\ twoWay /\ blocking /\ agree /\ drGrade

\* L19, L21: Ratification applies to proposed DRs, never decided Questions.
L21_OnlyProposedDrRatified == state = "Ratified" => "ProposedDr" \in seen /\ "DecidedOvernight" \notin seen

\* L20: Scoring follows preparation for his decision.
L20_PreparedBeforeScore == state = "Scored" => "Prepared" \in seen

\* L19: An investigated wrong Question had a provisional ruling.
L19_InvestigateProvisional == state = "Investigated" => "DecidedOvernight" \in seen

====


---- MODULE Mutation ----

VARIABLES state, cloudRouteNotMuchMoreWork

vars == <<state, cloudRouteNotMuchMoreWork>>

Finals == {"Clean"}

Init ==
    /\ state = "Background"
    /\ cloudRouteNotMuchMoreWork = FALSE

\* claim:Mutation.AllKilled
Mutation_AllKilled ==
    /\ state = "Running"
    /\ state' = "Clean"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Approve
Mutation_Approve ==
    /\ state = "Claimed"
    /\ state' = "Clean"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.ClaimEquivalent
Mutation_ClaimEquivalent ==
    /\ state = "Ticketed"
    /\ state' = "Claimed"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Cloud
Mutation_Cloud ==
    /\ state = "Queued"
    /\ cloudRouteNotMuchMoreWork
    /\ state' = "Running"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.CutFix
Mutation_CutFix ==
    /\ state = "Survivors"
    /\ state' = "Ticketed"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Fix
Mutation_Fix ==
    /\ state = "Ticketed"
    /\ state' = "Queued"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.LandingCheck
Mutation_LandingCheck ==
    /\ state = "Background"
    /\ state' = "Queued"
    /\ cloudRouteNotMuchMoreWork' \in BOOLEAN

\* claim:Mutation.Lapse
Mutation_Lapse ==
    /\ state = "Clean"
    /\ state' = "Queued"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Reject
Mutation_Reject ==
    /\ state = "Claimed"
    /\ state' = "Dispute"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Rejected
Mutation_Rejected ==
    /\ state = "Claimed"
    /\ state' = "Ticketed"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Ruled
Mutation_Ruled ==
    /\ state = "Dispute"
    /\ state' = "Clean"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.RuledNot
Mutation_RuledNot ==
    /\ state = "Dispute"
    /\ state' = "Ticketed"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Schedule
Mutation_Schedule ==
    /\ state = "Queued"
    /\ state' = "Running"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

\* claim:Mutation.Survive
Mutation_Survive ==
    /\ state = "Running"
    /\ state' = "Survivors"
    /\ UNCHANGED <<cloudRouteNotMuchMoreWork>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Mutation_AllKilled
    \/ Mutation_Approve
    \/ Mutation_ClaimEquivalent
    \/ Mutation_Cloud
    \/ Mutation_CutFix
    \/ Mutation_Fix
    \/ Mutation_LandingCheck
    \/ Mutation_Lapse
    \/ Mutation_Reject
    \/ Mutation_Rejected
    \/ Mutation_Ruled
    \/ Mutation_RuledNot
    \/ Mutation_Schedule
    \/ Mutation_Survive
    \/ Done

Bounds == cloudRouteNotMuchMoreWork \in BOOLEAN

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

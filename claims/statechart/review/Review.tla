\* built from the model; edit there: tools/SkillBuild
---- MODULE Review ----
EXTENDS Naturals

VARIABLES state, round, kind, securityFirst

vars == <<state, round, kind, securityFirst>>

Finals == {"Ended", "Replan", "Escalated"}

Init ==
    /\ state = "Wired"
    /\ round = 1
    /\ kind = "none"
    /\ securityFirst = FALSE

\* claim:Review.Broken
Review_Broken ==
    /\ state = "Verdict"
    /\ kind = "broken" /\ round # 3
    /\ state' = "Fixing"
    /\ UNCHANGED <<round, kind, securityFirst>>

\* claim:Review.CheckpointEscalate
Review_CheckpointEscalate ==
    /\ state = "Checkpoint"
    /\ kind \in {"fundamental", "balazs"}
    /\ state' = "Escalated"
    /\ UNCHANGED <<round, kind, securityFirst>>

\* claim:Review.CheckpointGoOn
Review_CheckpointGoOn ==
    /\ state = "Checkpoint"
    /\ kind \in {"broken", "security"}
    /\ state' = "Fixing"
    /\ securityFirst' = (securityFirst \/ kind = "security")
    /\ UNCHANGED <<round, kind>>

\* claim:Review.Clean
Review_Clean ==
    /\ state = "Verdict"
    /\ kind = "none"
    /\ state' = "Ended"
    /\ UNCHANGED <<round, kind, securityFirst>>

\* claim:Review.FixDone
Review_FixDone ==
    /\ state = "Fixing"
    /\ state' = "Round"
    /\ round' = round + 1
    /\ UNCHANGED <<kind, securityFirst>>

\* claim:Review.Fundamental
Review_Fundamental ==
    /\ state = "Verdict"
    /\ kind = "fundamental"
    /\ state' = "Replan"
    /\ UNCHANGED <<round, kind, securityFirst>>

\* claim:Review.NeedsBalazs
Review_NeedsBalazs ==
    /\ state = "Verdict"
    /\ kind = "balazs"
    /\ state' = "Escalated"
    /\ UNCHANGED <<round, kind, securityFirst>>

\* claim:Review.OneMore
Review_OneMore ==
    /\ state = "Verdict"
    /\ kind = "none"
    /\ state' = "Round"
    /\ round' = round + 1
    /\ UNCHANGED <<kind, securityFirst>>

\* claim:Review.Open
Review_Open ==
    /\ state = "Wired"
    /\ state' = "Round"
    /\ round' = 1
    /\ UNCHANGED <<kind, securityFirst>>

\* claim:Review.Post
Review_Post ==
    /\ state = "Round"
    /\ state' = "Verdict"
    /\ kind' \in {"none", "broken", "security", "fundamental", "balazs"}
    /\ UNCHANGED <<round, securityFirst>>

\* claim:Review.Security
Review_Security ==
    /\ state = "Verdict"
    /\ kind = "security" /\ round # 3
    /\ state' = "Fixing"
    /\ securityFirst' = TRUE
    /\ UNCHANGED <<round, kind>>

\* claim:Review.Skip
Review_Skip ==
    /\ state = "Wired"
    /\ state' = "Ended"
    /\ UNCHANGED <<round, kind, securityFirst>>

\* claim:Review.ToCheckpoint
Review_ToCheckpoint ==
    /\ state = "Verdict"
    /\ round = 3 /\ kind # "none"
    /\ state' = "Checkpoint"
    /\ UNCHANGED <<round, kind, securityFirst>>

Done == state \in Finals /\ UNCHANGED vars

Next ==
    \/ Review_Broken
    \/ Review_CheckpointEscalate
    \/ Review_CheckpointGoOn
    \/ Review_Clean
    \/ Review_FixDone
    \/ Review_Fundamental
    \/ Review_NeedsBalazs
    \/ Review_OneMore
    \/ Review_Open
    \/ Review_Post
    \/ Review_Security
    \/ Review_Skip
    \/ Review_ToCheckpoint
    \/ Done

Bounds == round <= 4 /\ kind \in {"none", "broken", "security", "fundamental", "balazs"} /\ securityFirst \in BOOLEAN

Fairness == TRUE

Spec == Init /\ [][Next]_vars /\ Fairness
====

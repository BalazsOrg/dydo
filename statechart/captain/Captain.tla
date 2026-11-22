---- MODULE Captain ----
\* A captain and its ticket: the comprehensive model, the hand-written twin of the process the role
\* skills describe. It holds every state and every rule of the process that has an order to check.
\* Written by hand; no build step writes this file.
\*
\* chart.mmd beside it is the simple view Balazs approved for reading (DYD-1440). The view is a
\* projection of this model: view-map.mjs sends each model state to one chart state, and each
\* action either stands under one chart arrow or stays inside one chart state. check-chart.mjs
\* proves the projection: no chart arrow without a model step, and no model step off the chart.
\* A skill line is checked against this model; the chart is for reading.
\*
\* A comment names what an action rests on. The truth, in order: his recorded words, then the
\* Project's spec (Spec: dydo 4.0.0; a letter and number is one of its rules), then the decision
\* records, with dydo/core.md ("core") while it lives. Where the core page and the spec are both
\* silent on an order, the model draws none.
EXTENDS Naturals

\* state   where the captain is with its ticket.
\* kind    what the ticket's result is: a change, a verdict (a review), findings (one seat of a
\*         review), or an answer (a Question; a Wayfinding ticket's cut runs the same way).
\* merges  whether its brief says this captain merges its change itself.
\* target  where that merge goes: a feature branch, or master as a two-way or a one-way PR. The
\*         brief names it from the start; no step depends on it before the merge comes next, so the
\*         model reads it there, "none" until then. That keeps the model small enough to draw.
\* filler  whether the ticket was pulled as filler (H11) and its filler check is still to come.
\*         Once the ticket is taken the check has passed, and the model forgets it.
\* round   the number of the review round now or last held, 0 before any; 4 means four or more.
\* extra   whether the one more round after a PASS has been ordered (R8).
\* lens    what the current or next round looks with: full lenses for its level, full with the
\*         security lens first (R9), or scoped to a repair (R22).
\* stage   the stage of the Question this captain waits on (H23), "none" when it waits on none.
VARIABLES state, kind, merges, target, filler, round, extra, lens, stage

vars == <<state, kind, merges, target, filler, round, extra, lens, stage>>

States == {"Dispatched", "Taken", "Leaf", "Asking", "Wayfinding", "Cutting", "Waiting",
           "InReview", "Clear", "Checkpoint", "Fixing", "Mending",
           "Merging", "AwaitingTurn", "Rerunning", "AwaitingClick", "Repairing",
           "Done", "HandedBack"}

\* The two ends: the contract is met, or the ticket went back to the officer unfinished.
Ends == {"Done", "HandedBack"}

Targets == {"feature", "main two-way", "main one-way"}

\* The next round's number. Four stands for four or more: the rules name round 3 and nothing later,
\* and the fifth-FAIL stop is gone (R10), so the rounds go on without a bound.
NextRound == IF round < 4 THEN round + 1 ELSE 4

TypeOK ==
    /\ state \in States
    /\ kind \in {"unread", "change", "verdict", "findings", "answer"}
    /\ merges \in BOOLEAN
    /\ target \in Targets \cup {"none"}
    /\ filler \in BOOLEAN
    /\ round \in 0..4
    /\ extra \in BOOLEAN
    /\ lens \in {"none", "full", "security first", "fix-scoped"}
    /\ stage \in {"none", "research", "council", "his-call"}
    /\ merges => kind = "change"
    /\ target # "none" => merges

\* The coordinator or an officer may pull a ticket as filler (H10, H11); the ticket says so.
Init ==
    /\ state = "Dispatched"
    /\ kind = "unread"
    /\ merges = FALSE
    /\ target = "none"
    /\ filler \in BOOLEAN
    /\ round = 0
    /\ extra = FALSE
    /\ lens = "none"
    /\ stage = "none"

\* ---- Taking the ticket ----

\* S7: one agent holds one ticket. It is held by another agent, or it is closed, so it goes back
\* untouched with the reason (S6: nobody set it In Progress).
CannotTake ==
    /\ state = "Dispatched"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* H11: the dispatched captain checks a filler's small, two-way and no-design-call itself. A filler
\* that fails goes back unassigned with a one-line reason, blocked on the open Wayfinding ticket.
FillerFailsItsCheck ==
    /\ state = "Dispatched"
    /\ filler
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* S6: whoever starts a step sets it In Progress. K30: the ticket and the brief say what its result
\* is and whether this captain merges it (K14, S15). Only a change is merged. A filler is a Bug or
\* a small Feature (H11), so its result is a change, and it is taken only once its check passes.
Take ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ kind' \in IF filler THEN {"change"} ELSE {"change", "verdict", "findings", "answer"}
    /\ merges' \in IF kind' = "change" THEN BOOLEAN ELSE {FALSE}
    /\ filler' = FALSE
    /\ UNCHANGED <<target, round, extra, lens, stage>>

\* ---- Sizing it ----

\* core "A ticket": the right size is what one agent can hold, about 100k tokens of context, and work
\* that needs the same thing understood belongs to one agent. S2
WorkItself ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* core "A ticket": it splits the ticket and only orchestrates. H2. A review may be cut into seats
\* from another model family (R34, S1). A seat is a leaf: "They're not meant do delegate into further
\* sub agents for sure." (R34)
Split ==
    /\ state = "Taken"
    /\ kind # "findings"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* core "A ticket": a captain that cannot see the route to the spec finds it one manageable part at a
\* time. H10: wayfinding toward the spec is part of every captain's work. A review and a seat judge a
\* candidate that is there; they have no route to find.
FindTheWay ==
    /\ state = "Taken"
    /\ kind \in {"change", "answer"}
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* ---- Doing the work itself ----

\* The escape hatch: a holder may still divide a slice that is too big. K30, H2. Never a seat (R34).
SplitTheRest ==
    /\ state = "Leaf"
    /\ kind # "findings"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* core "Who decides what": outside its job a captain decides only obvious, reversible calls; any other
\* call becomes a Question ticket for a fresh agent. L18: every Question starts with research, so it
\* opens at stage research (H23). H16 has the officer that receives a need open it; positions.md has
\* the captain with the call open it. Either way this captain waits on it, so the step is one.
\* Never a seat (R34).
Ask ==
    /\ state = "Leaf"
    /\ kind # "findings"
    /\ state' = "Asking"
    /\ stage' = "research"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens>>

\* H16, L18, L23: a Question's captain whose research leaves a hard two-way call opens a council
\* child (Mode AFK), chaired by a fresh ticket captain, and waits on its answer. H23: "A research
\* captain opens a council or his-call child only when its research does not resolve the call."
AskACouncil ==
    /\ state = "Leaf"
    /\ kind = "answer"
    /\ state' = "Asking"
    /\ stage' = "council"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens>>

\* H16, H23, L18: a call that is worth his time goes toward his HITL queue as a his-call child,
\* Mode HITL, with his question made ready on it: every option, each trade-off, a recommendation
\* (core "Who decides what"). The coordinator dispatches an envoy onto that same ticket when its
\* turn comes (H16, H18). This captain waits on the answer.
AskHim ==
    /\ state = "Leaf"
    /\ kind = "answer"
    /\ state' = "Asking"
    /\ stage' = "his-call"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens>>

\* H16: the work waits on the answer and then goes on; it never ends from the wait (519126ab).
\* A Question that comes back without its answer is reported to the officer, which changes no state,
\* or gets a fresh agent, and the wait goes on.
Answered ==
    /\ state = "Asking"
    /\ state' = "Leaf"
    /\ stage' = "none"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens>>

\* core: an agent never judges its own work, the verdict is another agent's. A merge needs a PASS on
\* its head, so the captain that merges is reviewed first (R23, R33). The first round looks with the
\* full lenses of its level (R6).
SendLeafToReview ==
    /\ state = "Leaf"
    /\ merges
    /\ state' = "InReview"
    /\ round' = 1
    /\ lens' = "full"
    /\ UNCHANGED <<kind, merges, target, filler, extra, stage>>

\* Nothing to merge: a verdict (never reviewed again: core, R34), findings, an answer, or a part its
\* officer reviews with the whole (R2, R6: a leaf gets no full review of its own).
CloseLeaf ==
    /\ state = "Leaf"
    /\ ~merges
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* H2: what a holder cannot settle under its own ticket goes back to the officer above.
\* core: a problem outside your ticket goes to your officer; what matters is that it surfaces.
HandBackTheWork ==
    /\ state = "Leaf"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* ---- Splitting it ----

\* H10: the parts in sight are cut, and the fog is left uncut. When no part is in sight, the cut is
\* one part that brings the next into sight: a Question, a prototype, or a Wayfinding ticket.
PartsInSight ==
    /\ state = "Wayfinding"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* A sub-ticket goes out by delegate_task, and its brief names the skills its position needs.
\* core, H3, K14
Dispatch ==
    /\ state = "Cutting"
    /\ state' = "Waiting"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* H2: a slice that cannot be divided coherently under its own ticket goes back to the officer
\* above for a re-cut.
HandBackTheCut ==
    /\ state = "Cutting"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* core "A ticket": the route is found one manageable part at a time; the whole is never cut at
\* once. H10. A part that comes back too big, or handed back, is cut again another way. H2
CutTheNextParts ==
    /\ state = "Waiting"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* H10: a part returns and fog remains, so the way is found again from what it showed. The default
\* of H10: the contract is not met and no ticket is left, so it wayfinds the next part.
FogRemains ==
    /\ state = "Waiting"
    /\ kind \in {"change", "answer"}
    /\ state' = "Wayfinding"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* H2: a change to its contract, or to a sibling's, goes through the officer above. R9: what is
\* fundamental or a new requirement is re-planned by the officer above.
HandBackTheWhole ==
    /\ state = "Waiting"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R2: the whole is reviewed once, after wiring and before its merge, with the lenses of its level
\* (R6).
SendTheWholeToReview ==
    /\ state = "Waiting"
    /\ merges
    /\ state' = "InReview"
    /\ round' = 1
    /\ lens' = "full"
    /\ UNCHANGED <<kind, merges, target, filler, extra, stage>>

\* R6: a ticket that only groups its parts needs no review of its own. S8: a review whose seats are
\* back posts the round's one verdict, naming each seat. H10: the contract is met, so it finishes.
CloseTheWhole ==
    /\ state = "Waiting"
    /\ ~merges
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* ---- The review loop (R8, R9, R10, S9, S10, S17) ----

\* R8: the phase ends on the first round with no new blocker. core "Review severities", R11: PASS
\* means no blocker. Each should becomes a ticket that waits (S17), and each nit gets a ruling (S11).
\* The round's lens is spent; the next round, if any, sets its own.
Pass ==
    /\ state = "InReview"
    /\ state' = "Clear"
    /\ lens' = "none"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, stage>>

\* core "Review severities": FAIL means at least one blocker, and a blocker is fixed now. R9: broken
\* or against the spec goes to a Fix, then the next round. S9: one Fix rN ticket lists every blocker.
\* R8: a round with a blocker is always followed by a full-lens round. Round 3 has the checkpoint.
Fail ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Fixing"
    /\ lens' = "full"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, stage>>

\* R9: a security hole goes to a Fix, then the next round with the security lens first.
FailOnASecurityHole ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "Fixing"
    /\ lens' = "security first"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, stage>>

\* R9: a blocker that is fundamental or a new requirement goes to the officer above to re-plan, and
\* one that needs Balazs goes up the chain at once. S10: dead hops go Canceled with a reason.
Replan ==
    /\ state = "InReview"
    /\ round # 3
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R10: after round 3 comes a checkpoint, not a stop. A third round that still has a blocker goes
\* to the checkpoint, where the officer running the rounds decides on purpose.
ThirdFail ==
    /\ state = "InReview"
    /\ round = 3
    /\ state' = "Checkpoint"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R10: "by default one more round". S10: one line on the Review, "checkpoint: one more round,
\* because ...". The blockers go to a Fix, and its round looks as R8 and R9 say.
CheckpointOneMoreRound ==
    /\ state = "Checkpoint"
    /\ state' = "Fixing"
    /\ lens' \in {"full", "security first"}
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, stage>>

\* R10: escalating only for a fundamental break, a new requirement, or something that needs him.
\* S10: one line on the Review, "escalated: ...".
CheckpointEscalates ==
    /\ state = "Checkpoint"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R8: a fix between every two rounds. S9: the next round is a fresh look, whoever did the Fix. Its
\* lens is the one the FAIL set: full, or full with security first.
ReviewAgain ==
    /\ state = "Fixing"
    /\ state' = "InReview"
    /\ round' = NextRound
    /\ UNCHANGED <<kind, merges, target, filler, extra, lens, stage>>

\* The Fix's holder hands it back undone, and this captain passes it on. H2
HandBackTheFix ==
    /\ state = "Fixing"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R8: the officer running the rounds decides whether another round is needed; by default a round
\* with no new blocker ends the phase. The merge comes next, into the target the brief names.
ReadTheTarget == target' \in IF target = "none" THEN Targets ELSE {target}

NoMoreRound ==
    /\ state = "Clear"
    /\ ReadTheTarget
    /\ target' # "main two-way"
    /\ state' = "Merging"
    /\ UNCHANGED <<kind, merges, filler, round, extra, lens, stage>>

\* R29: main-bound merges are paced. The coordinator gives the turn, and the captain merges (R26).
AwaitTheTurn ==
    /\ state = "Clear"
    /\ ReadTheTarget
    /\ target' = "main two-way"
    /\ state' = "AwaitingTurn"
    /\ UNCHANGED <<kind, merges, filler, round, extra, lens, stage>>

\* R8: "the officer may still order one more", once.
OneMoreRound ==
    /\ state = "Clear"
    /\ ~extra
    /\ state' = "InReview"
    /\ round' = NextRound
    /\ extra' = TRUE
    /\ lens' = "full"
    /\ UNCHANGED <<kind, merges, target, filler, stage>>

\* S17: a should is fixed in place before the parent merges if cheap. Its fix moves the head, and a
\* merge needs a PASS on its head (R23), so the mended head goes to a round of its own.
MendACheapShould ==
    /\ state = "Clear"
    /\ state' = "Mending"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R23, R24: a PASS stands only on its head, or on an ancestor whose later commits are clean syncs.
\* A mend is neither, so the new head gets a round.
ReviewTheMendedHead ==
    /\ state = "Mending"
    /\ state' = "InReview"
    /\ round' = NextRound
    /\ lens' = "full"
    /\ UNCHANGED <<kind, merges, target, filler, extra, stage>>

\* ---- The merge (R18 to R31) ----

\* R29: the coordinator gives the turn. The PR re-checks against current master just before landing.
TurnGiven ==
    /\ state = "AwaitingTurn"
    /\ state' = "Merging"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* core: the ticket is done when its contract is met. R20, R21, R23: the captain runs dydo merge and
\* the gates itself, and the merge is in with its gates and hosted CI green. R25, R26: into a feature
\* branch freely, into master as a two-way PR on its turn.
Merged ==
    /\ state = "Merging"
    /\ target # "main one-way"
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* core "Who decides what": Balazs decides anything one-way. R27: a one-way PR needs his click and
\* carries a recovery plan. R31: the captain watches its own CI and opens the HITL Merge only when
\* it is green (S12).
GreenForHisClick ==
    /\ state = "Merging"
    /\ target = "main one-way"
    /\ state' = "AwaitingClick"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R31: the survey sees "PR merged, HITL Merge open" and wakes the captain.
HisClick ==
    /\ state = "AwaitingClick"
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R19, R21: a CONFLICT is resolved only on the ticket branch, by an implementer the captain sends.
MergeConflict ==
    /\ state = "Merging"
    /\ state' = "Repairing"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R22: a red gate after a merge: DYTEST reruns once.
GateRed ==
    /\ state = "Merging"
    /\ state' = "Rerunning"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R22: a flake means the hop is Done, and the flake is counted as a DYTEST result. A one-way PR
\* still waits on his click.
Flake ==
    /\ state = "Rerunning"
    /\ state' = IF target = "main one-way" THEN "AwaitingClick" ELSE "Done"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R22: a real red means the captain reverts at once, and a Fix follows on the ticket branch.
\* S12: the reverted Merge goes Canceled with "reverted: <gate> red on <merge SHA>".
RealRed ==
    /\ state = "Rerunning"
    /\ state' = "Repairing"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* R19: "under a fresh review of its hunks". R22: "then a fix-scoped round, then a re-land".
RepairReviewed ==
    /\ state = "Repairing"
    /\ state' = "InReview"
    /\ round' = NextRound
    /\ lens' = "fix-scoped"
    /\ UNCHANGED <<kind, merges, target, filler, extra, stage>>

\* H2: a conflict that cannot be resolved, or a red gate that cannot be fixed, under the ticket goes
\* back to the officer above.
RepairFails ==
    /\ state = "Repairing"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges, target, filler, round, extra, lens, stage>>

\* An end has no step out; this keeps the deadlock check for every other state.
Ended == state \in Ends /\ UNCHANGED vars

Next ==
    \/ CannotTake
    \/ FillerFailsItsCheck
    \/ Take
    \/ WorkItself
    \/ Split
    \/ FindTheWay
    \/ SplitTheRest
    \/ Ask
    \/ AskACouncil
    \/ AskHim
    \/ Answered
    \/ SendLeafToReview
    \/ CloseLeaf
    \/ HandBackTheWork
    \/ PartsInSight
    \/ Dispatch
    \/ HandBackTheCut
    \/ CutTheNextParts
    \/ FogRemains
    \/ HandBackTheWhole
    \/ SendTheWholeToReview
    \/ CloseTheWhole
    \/ Pass
    \/ Fail
    \/ FailOnASecurityHole
    \/ Replan
    \/ ThirdFail
    \/ CheckpointOneMoreRound
    \/ CheckpointEscalates
    \/ ReviewAgain
    \/ HandBackTheFix
    \/ NoMoreRound
    \/ AwaitTheTurn
    \/ OneMoreRound
    \/ MendACheapShould
    \/ ReviewTheMendedHead
    \/ TurnGiven
    \/ Merged
    \/ GreenForHisClick
    \/ HisClick
    \/ MergeConflict
    \/ GateRed
    \/ Flake
    \/ RealRed
    \/ RepairReviewed
    \/ RepairFails
    \/ Ended

Spec == Init /\ [][Next]_vars
====

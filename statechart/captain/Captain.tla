---- MODULE Captain ----
\* A captain and its ticket: the hand-written twin of the process the role skills describe.
\* Written by hand; no build step writes this file. chart.mmd beside it is the same machine as a
\* picture: one action here is one arrow there, and view.mjs takes each arrow's words from the chart.
\* A comment names what an action rests on: "core" is dydo/core.md, a letter and number is a rule
\* of Spec: dydo 4.0.0. Where the two disagree, the core page wins.

\* state   where the captain is with its ticket.
\* kind    what the ticket's result is: a change, a verdict (a review) or an answer (a Question).
\* merges  whether its brief says this captain merges its change itself.
VARIABLES state, kind, merges

vars == <<state, kind, merges>>

States == {"Dispatched", "Taken", "Leaf", "Asking", "Cutting", "Waiting",
           "InReview", "Fixing", "Merging", "Done", "HandedBack"}

\* The two ends: the contract is met, or the ticket went back to the officer unfinished.
Ends == {"Done", "HandedBack"}

TypeOK ==
    /\ state \in States
    /\ kind \in {"unread", "change", "verdict", "answer"}
    /\ merges \in BOOLEAN
    /\ merges => kind = "change"

Init ==
    /\ state = "Dispatched"
    /\ kind = "unread"
    /\ merges = FALSE

\* The ticket is held by another agent, is closed, or is a filler that fails its check. S7, H11
CannotTake ==
    /\ state = "Dispatched"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges>>

\* Whoever starts a step sets it In Progress (S6). The ticket and the brief say what its result is
\* and whether this captain merges it; only a change is merged.
Take ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ kind' \in {"change", "verdict", "answer"}
    /\ merges' \in IF kind' = "change" THEN BOOLEAN ELSE {FALSE}

\* core "A ticket": the right size is what one agent can hold, about 100k tokens of context. S2
WorkItself ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ UNCHANGED <<kind, merges>>

\* core "A ticket": it splits the ticket and only orchestrates. H2
\* The ticket is too big for one agent, or it is a review whose seats go to another model family (R34).
Split ==
    /\ state = "Taken"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges>>

\* The escape hatch: a holder may still divide a slice that is too big. K30, H2
SplitTheRest ==
    /\ state = "Leaf"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges>>

\* core "Who decides what": outside its job a captain decides only obvious, reversible calls;
\* any other call becomes a Question ticket for a fresh agent. H16
\* The captain with the call opens the Question, as a sub-ticket of its own ticket (positions.md, "A Question").
Ask ==
    /\ state = "Leaf"
    /\ state' = "Asking"
    /\ UNCHANGED <<kind, merges>>

\* The one way out of the wait. A Question that comes back without its answer goes up to the officer
\* as a report, which changes no state, and the wait goes on.
Answered ==
    /\ state = "Asking"
    /\ state' = "Leaf"
    /\ UNCHANGED <<kind, merges>>

\* core: an agent never judges its own work, the verdict is another agent's.
\* A merge needs a PASS on its head, so the captain that merges is reviewed first. R23, R33
SendLeafToReview ==
    /\ state = "Leaf"
    /\ merges
    /\ state' = "InReview"
    /\ UNCHANGED <<kind, merges>>

\* Nothing to merge: a verdict (never reviewed again: core, R34), an answer, or a part its officer
\* reviews with the whole (R2).
CloseLeaf ==
    /\ state = "Leaf"
    /\ ~merges
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges>>

\* H2: what a holder cannot settle under its own ticket goes back to the officer above.
\* core: a problem outside your ticket goes to your officer; what matters is that it surfaces.
HandBackTheWork ==
    /\ state = "Leaf"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges>>

\* A sub-ticket goes out by delegate_task, and its brief names the skills its position needs. core, H3, K14
Dispatch ==
    /\ state = "Cutting"
    /\ state' = "Waiting"
    /\ UNCHANGED <<kind, merges>>

\* H2: a slice that cannot be divided coherently under its own ticket goes back to the officer
\* above for a re-cut.
HandBackTheCut ==
    /\ state = "Cutting"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges>>

\* core "A ticket": the route to the spec is found one manageable part at a time; the whole is
\* never cut at once. H10. A part that comes back too big is cut again the same way. H2
CutTheNextParts ==
    /\ state = "Waiting"
    /\ state' = "Cutting"
    /\ UNCHANGED <<kind, merges>>

\* H2: a change to its contract, or to a sibling's, goes through the officer above. R9: what is
\* fundamental or a new requirement is re-planned by the officer above.
HandBackTheWhole ==
    /\ state = "Waiting"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges>>

\* The whole is reviewed once, after wiring and before its merge. R2
SendTheWholeToReview ==
    /\ state = "Waiting"
    /\ merges
    /\ state' = "InReview"
    /\ UNCHANGED <<kind, merges>>

\* A ticket that only groups its parts (R6), and a review whose seats are back, which posts the
\* round's one verdict (S8).
CloseTheWhole ==
    /\ state = "Waiting"
    /\ ~merges
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges>>

\* core "Review severities": FAIL means at least one blocker, and a blocker is fixed now.
\* The blockers become one Fix ticket (R8, R11, S9). Its holder is a captain doing a leaf on its
\* own run of this chart; by default it is the thread that wrote the change. While it works, this
\* captain waits.
Fail ==
    /\ state = "InReview"
    /\ state' = "Fixing"
    /\ UNCHANGED <<kind, merges>>

\* PASS means no blocker. A should becomes a ticket that waits; a nit gets a ruling. core, R11, S17
Pass ==
    /\ state = "InReview"
    /\ state' = "Merging"
    /\ UNCHANGED <<kind, merges>>

\* A blocker that is fundamental, a new requirement or a need for Balazs goes to the officer above. R9
\* After a third FAIL the captain writes one line on the Review saying which of Fail and Replan
\* it took (R10, S10); the line is a record, not a state.
Replan ==
    /\ state = "InReview"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges>>

\* A fix between every two rounds, and the next round is a fresh look. R8, S9
ReviewAgain ==
    /\ state = "Fixing"
    /\ state' = "InReview"
    /\ UNCHANGED <<kind, merges>>

\* The Fix's holder hands it back undone, and this captain passes it on. H2
HandBackTheFix ==
    /\ state = "Fixing"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<kind, merges>>

\* core: a ticket is done when its contract is met. The merge is in and its gates are green. R20, R21, S6
Merged ==
    /\ state = "Merging"
    /\ state' = "Done"
    /\ UNCHANGED <<kind, merges>>

\* A conflict or a red gate is repaired on the ticket branch and reviewed again. R19, R21, R22
MergeBroke ==
    /\ state = "Merging"
    /\ state' = "Fixing"
    /\ UNCHANGED <<kind, merges>>

\* An end has no step out; this keeps the deadlock check for every other state.
Ended == state \in Ends /\ UNCHANGED vars

Next ==
    \/ CannotTake
    \/ Take
    \/ WorkItself
    \/ Split
    \/ SplitTheRest
    \/ Ask
    \/ Answered
    \/ SendLeafToReview
    \/ CloseLeaf
    \/ HandBackTheWork
    \/ Dispatch
    \/ HandBackTheCut
    \/ CutTheNextParts
    \/ HandBackTheWhole
    \/ SendTheWholeToReview
    \/ CloseTheWhole
    \/ Fail
    \/ Pass
    \/ Replan
    \/ ReviewAgain
    \/ HandBackTheFix
    \/ Merged
    \/ MergeBroke
    \/ Ended

Spec == Init /\ [][Next]_vars
====

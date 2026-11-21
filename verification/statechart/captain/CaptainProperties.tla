---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from the core page (dydo/core.md), the spec and his rulings, never from the model.
\* A property says what a source requires; when the model breaks one, the model is wrong.
\* seen is passive: the set of states this captain has been in. Every model step has exactly one
\* observer extension, no step is disabled and no fairness is added.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* core "A ticket": "it is done when its contract is met". DYD-1295 7b3d2e3e.
\* Checked as order only: Done comes after the work, its own or its parts'. Whether the contract
\* is met cannot be seen here.
WorkPhaseBeforeDone ==
    [][(state' = "Done" /\ state # "Done" => seen \cap {"Leaf", "Waiting"} # {})]_<<vars, seen>>

\* H3, DYD-1295 df6a785e: the first move after a cut dispatches the parts and waits.
\* H2: a slice that cannot be divided coherently under its own ticket goes back to the officer
\* above, so a cut that cannot be made is the one other way out.
H3_WaitAfterSplit ==
    [][(state = "Cutting" /\ state' # state => state' \in {"Waiting", "HandedBack"})]_<<vars, seen>>

\* core "Who decides what": a call a captain may not decide becomes a Question for a fresh agent.
\* H16, DYD-1295 519126ab: the work waits on the answer and then goes on; it never ends from the wait.
H16_ResumeLeafAfterQuestion ==
    [][(state = "Asking" /\ state' # state => state' = "Leaf")]_<<vars, seen>>

\* core "A ticket": a captain "either does the work itself, with native helpers, or splits the
\* ticket and only orchestrates". Once it has cut, it never goes back to doing the work itself.
Core_SplitOnlyOrchestrates ==
    "Cutting" \in seen => state \notin {"Leaf", "Asking"}

\* H2, H10, R2: after a part returns the captain cuts the next parts, sends the whole to review,
\* or finishes. H2 also sends a change to its contract, or to a sibling's, through the officer
\* above. It never returns to an untaken ticket.
H10_ReturnedPartContinues ==
    [][(state = "Waiting" /\ state' # state =>
        state' \in {"Cutting", "InReview", "Done", "HandedBack"})]_<<vars, seen>>

\* R8, R23: the review ends on a verdict with no blocker, and a merge needs that PASS. The merge
\* starts only from the review, and a reviewed ticket closes only from its merge.
R8_PassBeforeMerge ==
    [][ /\ (state' = "Merging" /\ state # "Merging" => state = "InReview")
        /\ (state' = "Done" /\ state # "Done" /\ "InReview" \in seen => state = "Merging")
    ]_<<vars, seen>>

\* core "Review severities": a blocker "needs fixing now". R8: "a fix between every two rounds".
\* A second round is entered only from the Fix, and the Fix leads to a round or back to the officer,
\* never straight to the merge.
R8_FixBeforeNextRound ==
    [][ /\ (state' = "InReview" /\ state # "InReview" /\ "InReview" \in seen => state = "Fixing")
        /\ (state = "Fixing" /\ state' # state => state' \in {"InReview", "HandedBack"})
    ]_<<vars, seen>>

\* R19: a conflict is resolved "under a fresh review of its hunks". R22: "then a fix-scoped round,
\* then a re-land". A merge that breaks goes to a Fix, and nothing but a closed ticket or a Fix
\* follows a merge.
R19_ReviewAfterMergeRepair ==
    [][(state = "Merging" /\ state' # state => state' \in {"Done", "Fixing"})]_<<vars, seen>>

\* core: "It never judges its own work: the verdict is another agent's." R2, R23: the holder that
\* merges a change closes only after a review, from its merge.
R2_MergingHolderHasReview ==
    [][(merges /\ state' = "Done" /\ state # "Done" =>
        "InReview" \in seen /\ state = "Merging")]_<<vars, seen>>

\* core "Review severities": "A verdict is not reviewed again." R34, DYD-1372 777d396e.
\* A verdict's holder is never in the review, fix and merge loop.
R34_VerdictNeverReviewed ==
    kind = "verdict" => state \notin {"InReview", "Fixing", "Merging"}

\* positions.md: only a captain that merges a change gets the review, fix and merge part.
\* This checks that table against the model; it is the table's rule, not a rule of the spec.
Sees_OnlyAMergerSeesReviewFixMerge ==
    state \in {"InReview", "Fixing", "Merging"} => kind = "change" /\ merges

\* K30, S6: what the ticket's result is, is known before the holder works.
K30_KindKnownBeforeWork ==
    state \notin {"Dispatched", "HandedBack"} => kind # "unread"

\* K30, S7 and AGENTS.md: one holder, one ticket. Its kind does not change under it.
K30_KindStaysKnown ==
    [][(kind # "unread" => kind' = kind)]_<<vars, seen>>

\* S7, S6: a ticket that cannot be taken goes back with no work done on it.
S7_UnstartedReturnHasNoWork ==
    kind = "unread" => seen \subseteq {"Dispatched", "HandedBack"}

\* AGENTS.md "Sort yourself": crew does its one job and returns. H12: what goes up goes to the
\* officer above. Both ends are final: nothing follows Done or a hand-back. This is a structural
\* check of the two ends; the sources say a captain returns, not what it may do after.
H1_EndsAreFinal ==
    [][(state \in {"Done", "HandedBack"} => UNCHANGED vars)]_<<vars, seen>>
====

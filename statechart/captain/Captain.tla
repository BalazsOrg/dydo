---- MODULE Captain ----
\* A captain and its ticket: the model, the hand-written twin of the process the role skills
\* describe. Written by hand; no build step writes this file.
\*
\* This is the frame (DYD-1504). It holds one action for each arrow of the chart he approved
\* (chart.mmd, DYD-1440), and the few steps inside a chart state that the arrows' conditions need.
\* Each action names the register rows it rests on (register.md). The phase parts replace the
\* actions of their section with the full process their register rows name: part 2 intake and
\* Questions, part 3 the split, part 4 review and merge. Until then the frame is the chart's level,
\* and no more.
\*
\* view-map.mjs shows each model state as one chart state, puts each action under one chart arrow or
\* inside one chart state, and gives each arrow the condition its words claim. check-chart.mjs runs
\* TLC on this file and tests those conditions on every step.
EXTENDS CaptainBase

\* ==== Part 2: intake and Questions ====

\* FR1, IQ1: the ticket is held by another agent or is closed, so it goes back untouched with the
\* reason. Nobody set it In Progress.
CannotTake ==
    /\ state = "Dispatched"
    /\ state' = "HandedBack"
    /\ event' = {Ev("cannot take", "captain"), Ev("hand back", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* FR2, IQ2: whoever starts a step sets it In Progress; the ticket and its brief say what its result
\* is and whether this captain merges it. Only a change is merged; an answer's call is open.
Take ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ event' = {Ev("take", "captain")}
    /\ kind' \in Kinds
    /\ merges' \in IF kind' = "change" THEN BOOLEAN ELSE {FALSE}
    /\ call' = IF kind' = "answer" THEN "open" ELSE "none"
    /\ UNCHANGED <<proved, written, asked, part, verdict, blockers, gates, landed, ci>>

\* FR3, IQ6: the ticket fits one agent, about 100k tokens of context, so it does the work itself.
WorkItself ==
    /\ state = "Taken"
    /\ state' = "Leaf"
    /\ event' = {Ev("fits one agent", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* FR4, IQ6: the ticket needs more than one agent, so it splits it.
Split ==
    /\ state = "Taken"
    /\ state' = "Cutting"
    /\ event' = {Ev("needs more than one agent", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* FR5, IQ7: the escape hatch. The work proves too big, so it splits what is left.
SplitTheRest ==
    /\ state = "Leaf"
    /\ state' = "Cutting"
    /\ event' = {Ev("proves too big", "captain")}
    /\ proved' = FALSE
    /\ UNCHANGED <<kind, merges, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* IQ8: the author proves its change with receipts. Nothing in it waits on an answer.
ProveTheWork ==
    /\ state = "Leaf"
    /\ kind = "change"
    /\ ~proved
    /\ asked = "none"
    /\ proved' = TRUE
    /\ event' = {Ev("prove the work", "captain")}
    /\ UNCHANGED <<state, kind, merges, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* The captain's own result is written in the leaf, or once its parts are back.
ResultReady == state = "Leaf" \/ (state = "Waiting" /\ part = "back")

\* FR26, FR27: a reviewer decides its verdict, and FAIL has at least one blocker.
WriteTheVerdict ==
    /\ kind = "verdict"
    /\ verdict = "none"
    /\ asked = "none"
    /\ ResultReady
    /\ \/ verdict' = "pass" /\ blockers' = {}
       \/ verdict' = "fail" /\ blockers' \in (SUBSET Blockers) \ {{}}
    /\ event' = {Ev("write the verdict", "captain")}
    /\ UNCHANGED <<state, kind, merges, proved, written, call, asked, part, gates, landed, ci>>

\* FR26, IQ14: a Question's captain settles its call.
AnswerTheCall ==
    /\ kind = "answer"
    /\ call = "open"
    /\ asked = "none"
    /\ ResultReady
    /\ call' = "settled"
    /\ event' = {Ev("answer the call", "captain")}
    /\ UNCHANGED <<state, kind, merges, proved, written, asked, part, verdict, blockers, gates, landed, ci>>

\* FR24: a seat writes its findings, and a Wayfinding captain its cut.
WriteTheResult ==
    /\ kind \in {"findings", "cut"}
    /\ ~written
    /\ asked = "none"
    /\ ResultReady
    /\ written' = TRUE
    /\ event' = {Ev("write the result", "captain")}
    /\ UNCHANGED <<state, kind, merges, proved, call, asked, part, verdict, blockers, gates, landed, ci>>

\* FR6, IQ9: a call it may not decide becomes a Question for a fresh agent: the captain passes the
\* need up, and the officer that receives it opens the Question. The frame folds the two into one step.
AskAQuestion ==
    /\ state = "Leaf"
    /\ asked = "none"
    /\ state' = "Asking"
    /\ asked' = "open"
    /\ event' = {Ev("pass the need up", "captain"), Ev("open a Question", "officer")}
    /\ UNCHANGED <<kind, merges, proved, written, call, part, verdict, blockers, gates, landed, ci>>

\* FR7, IQ11: the one way out of the wait, the answer on the Question.
Answered ==
    /\ state = "Asking"
    /\ asked = "open"
    /\ state' = "Leaf"
    /\ asked' = "none"
    /\ event' = {Ev("answer", "question")}
    /\ UNCHANGED <<kind, merges, proved, written, call, part, verdict, blockers, gates, landed, ci>>

\* FR9, FR24, IQ25: the contract is met and it merges nothing, so it closes the ticket.
CloseLeaf ==
    /\ state = "Leaf"
    /\ ~merges
    /\ asked = "none"
    /\ ResultIn
    /\ state' = "Done"
    /\ event' = {Ev("close", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* FR10, IQ26: the work cannot be done inside this ticket, so it goes to its officer.
HandBackTheWork ==
    /\ state = "Leaf"
    /\ state' = "HandedBack"
    /\ event' = {Ev("cannot be done inside the ticket", "captain"), Ev("hand back", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* ==== Part 3: the split ====

\* FR11, SP3: each part is dispatched as a sub-ticket.
Dispatch ==
    /\ state = "Cutting"
    /\ state' = "Waiting"
    /\ part' = "out"
    /\ event' = {Ev("dispatch the parts", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, verdict, blockers, gates, landed, ci>>

\* FR12, SP10: the work cannot be cut inside this ticket, so it goes to its officer.
HandBackTheCut ==
    /\ state = "Cutting"
    /\ state' = "HandedBack"
    /\ event' = {Ev("cannot be cut inside the ticket", "captain"), Ev("hand back", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* SP3, SP7: a part returns.
PartBack ==
    /\ state = "Waiting"
    /\ part = "out"
    /\ part' = "back"
    /\ event' = {Ev("a part returns", "part")}
    /\ UNCHANGED <<state, kind, merges, proved, written, call, asked, verdict, blockers, gates, landed, ci>>

\* FR13, SP6, SP7: a part is back and the contract is not met yet, so it cuts the next parts.
CutTheNextParts ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ state' = "Cutting"
    /\ part' = "none"
    /\ event' = {Ev("cut the next parts", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, verdict, blockers, gates, landed, ci>>

\* FR14, SP10: a part shows the contract cannot be met inside this ticket, so it goes to its officer.
HandBackTheWhole ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ state' = "HandedBack"
    /\ event' = {Ev("a part shows the contract cannot be met", "captain"), Ev("hand back", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* SP16: the parts merge into the parent, proved by their gates on the merge SHA: the whole's receipts.
WireTheParts ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ merges
    /\ asked = "none"
    /\ part' = "wired"
    /\ proved' = TRUE
    /\ event' = {Ev("wire the parts", "captain")}
    /\ UNCHANGED <<state, kind, merges, written, call, asked, verdict, blockers, gates, landed, ci>>

\* FR15, SP17, RM1: the parts are back and wired and it merges them, so a reviewer is dispatched.
SendTheWholeToReview ==
    /\ state = "Waiting"
    /\ part = "wired"
    /\ merges
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ part' = "none"
    /\ verdict' = "due"
    /\ event' = {Ev("dispatch a reviewer", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, blockers, gates, landed, ci>>

\* FR16, FR24, SP18: the parts are back, the contract is met and it merges nothing, so it closes the
\* ticket. A change that only groups its parts is met once they are back.
CloseTheWhole ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ ~merges
    /\ asked = "none"
    /\ kind = "change" \/ ResultIn
    /\ state' = "Done"
    /\ part' = "none"
    /\ event' = {Ev("close", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, verdict, blockers, gates, landed, ci>>

\* ==== Part 4: review and merge ====

\* FR8, RM1: the change is whole and it merges it, so a reviewer is dispatched onto the round's
\* Review hop, after the author's proof.
SendLeafToReview ==
    /\ state = "Leaf"
    /\ merges
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ verdict' = "due"
    /\ event' = {Ev("dispatch a reviewer", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, blockers, gates, landed, ci>>

\* FR26, FR27, RM3: the fresh reviewer on the round's Review hop writes the verdict. FAIL has at least
\* one blocker, and PASS none.
TheReviewerWritesTheVerdict ==
    /\ state = "InReview"
    /\ verdict = "due"
    /\ \/ verdict' = "pass" /\ blockers' = {}
       \/ verdict' = "fail" /\ blockers' \in (SUBSET Blockers) \ {{}}
    /\ event' = {Ev("write the verdict", "reviewer")}
    /\ UNCHANGED <<state, kind, merges, proved, written, call, asked, part, gates, landed, ci>>

\* FR18: the verdict is PASS, so the merge comes next.
Pass ==
    /\ state = "InReview"
    /\ verdict = "pass"
    /\ state' = "Merging"
    /\ verdict' = "none"
    /\ event' = {Ev("act on PASS", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, blockers, gates, landed, ci>>

\* FR17, RM6, RM8: the verdict is FAIL, so one Fix ticket takes every blocker. The Fix moves the
\* head, so its proof is owed.
Fail ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ "fundamental" \notin blockers
    /\ state' = "Fixing"
    /\ verdict' = "none"
    /\ proved' = FALSE
    /\ event' = {Ev("act on FAIL", "captain"), Ev("open a Fix", "captain")}
    /\ UNCHANGED <<kind, merges, written, call, asked, part, blockers, gates, landed, ci>>

\* FR19, RM6, RM12: a blocker is fundamental or a new requirement, so it cannot be fixed inside this ticket
\* and goes to its officer to re-plan.
Replan ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ "fundamental" \in blockers
    /\ state' = "HandedBack"
    /\ verdict' = "none"
    /\ event' = {Ev("act on FAIL", "captain"), Ev("hand back", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, blockers, gates, landed, ci>>

\* RM9: the Fix's holder fixes every blocker and proves the fixed head.
TheFixIsProved ==
    /\ state = "Fixing"
    /\ ~proved
    /\ proved' = TRUE
    /\ event' = {Ev("prove the Fix", "fixer")}
    /\ UNCHANGED <<state, kind, merges, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* FR20, RM4, RM9: the Fix is done, so a fresh reviewer looks again.
ReviewAgain ==
    /\ state = "Fixing"
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ verdict' = "due"
    /\ blockers' = {}
    /\ event' = {Ev("dispatch a reviewer", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, gates, landed, ci>>

\* FR21, RM26: the Fix's holder hands it back undone, and this captain passes it on.
HandBackTheFix ==
    /\ state = "Fixing"
    /\ state' = "HandedBack"
    /\ event' = {Ev("the Fix comes back undone", "fixer"), Ev("hand back", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, gates, landed, ci>>

\* RM14, RM15: the gates run on the combined tree: a green receipt, a real red, or a CONFLICT.
RunTheGates ==
    /\ state = "Merging"
    /\ landed = "no"
    /\ gates = "none"
    /\ gates' \in {"green", "red", "conflict"}
    /\ event' = {Ev("run the gates", "tools")}
    /\ UNCHANGED <<state, kind, merges, proved, written, call, asked, part, verdict, blockers, landed, ci>>

\* RM14, RM20: the gates are green, so the captain merges.
Land ==
    /\ state = "Merging"
    /\ landed = "no"
    /\ gates = "green"
    /\ landed' = "in"
    /\ gates' = "none"
    /\ event' = {Ev("merge", "captain")}
    /\ UNCHANGED <<state, kind, merges, proved, written, call, asked, part, verdict, blockers, ci>>

\* FR22, FR24, RM17: the merge is in and its gates and CI are green on the merge SHA, so it closes the
\* ticket.
GreenOnTheMergeSha ==
    /\ state = "Merging"
    /\ landed = "in"
    /\ state' = "Done"
    /\ gates' = "green"
    /\ ci' = "green"
    /\ event' = {Ev("green on the merge SHA", "tools"), Ev("close", "captain")}
    /\ UNCHANGED <<kind, merges, proved, written, call, asked, part, verdict, blockers, landed>>

\* FR23, RM15: the merge is not clean and green, so a Fix ticket repairs it. The repair moves the head,
\* so its proof is owed.
Repair ==
    /\ state = "Merging"
    /\ landed = "no"
    /\ gates \in {"red", "conflict"}
    /\ state' = "Fixing"
    /\ gates' = "none"
    /\ proved' = FALSE
    /\ event' = {Ev("open a Fix", "captain")}
    /\ UNCHANGED <<kind, merges, written, call, asked, part, verdict, blockers, landed, ci>>

\* An end has no step out (FR25); this keeps the deadlock check for every other state.
Ended == state \in Ends /\ UNCHANGED vars

Next ==
    \/ CannotTake
    \/ Take
    \/ WorkItself
    \/ Split
    \/ SplitTheRest
    \/ ProveTheWork
    \/ WriteTheVerdict
    \/ AnswerTheCall
    \/ WriteTheResult
    \/ AskAQuestion
    \/ Answered
    \/ CloseLeaf
    \/ HandBackTheWork
    \/ Dispatch
    \/ HandBackTheCut
    \/ PartBack
    \/ CutTheNextParts
    \/ HandBackTheWhole
    \/ WireTheParts
    \/ SendTheWholeToReview
    \/ CloseTheWhole
    \/ SendLeafToReview
    \/ TheReviewerWritesTheVerdict
    \/ Pass
    \/ Fail
    \/ Replan
    \/ TheFixIsProved
    \/ ReviewAgain
    \/ HandBackTheFix
    \/ RunTheGates
    \/ Land
    \/ GreenOnTheMergeSha
    \/ Repair
    \/ Ended

Spec == Init /\ [][Next]_vars
====

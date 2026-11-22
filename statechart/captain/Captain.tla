---- MODULE Captain ----
\* A captain and its ticket: the comprehensive model, the hand-written twin of the process the role
\* skills describe. It holds every state and every rule of the process that has an order to check.
\* Written by hand; no build step writes this file.
\*
\* chart.mmd beside it is the simple view Balazs approved for reading (DYD-1440). The view is a
\* projection of this model: view-map.mjs shows each model state as one chart state, and puts each
\* action under one chart arrow or inside one chart state. check-chart.mjs reads TLC's graph of every
\* reachable step and proves the projection. A skill line is checked against this model; the chart
\* is for reading.
\*
\* A comment names what an action rests on. The truth, in order: his recorded words, then the
\* Project's spec (Spec: dydo 4.0.0; a letter and number is one of its rules), then the decision
\* records, with dydo/core.md ("core") while it lives. Where the core page and the spec are both
\* silent on an order, the model draws none.
EXTENDS Naturals

\* The ticket, fixed once it is taken:
\* kind     what the ticket's result is: a change, a verdict (a review), findings (one seat of a
\*          review), an answer (a Question), or a cut (a Wayfinding ticket: the next tickets toward
\*          the spec). "unread" before it is taken.
\* stage    a Question's stage label (H23): research, council or his-call; "none" for every other
\*          kind.
\* merges   whether its brief says this captain integrates and merges its change itself.
\* filler   whether the ticket was pulled as filler (H11) and its filler check is still to come.
\* door     for a council, what its case file says of the call: two-way; DR-grade, two-way,
\*          seriously blocking and overnight (L21); DR-grade; or one-way. "none" otherwise.
\*
\* The work:
\* proved   the author's receipts are in for the work as it stands (R5): a leaf's change, or a
\*          whole whose wiring gates are green (R18, R20).
\* scouted  the one scout pass on a cut's spec has run (R4).
\* fog      the route to the spec is not all in sight, so only the parts in sight are cut (H10).
\* part     the sub-tickets a captain that splits waits on, as one abstract part: out, handed back
\*          for a re-cut ("returned"), back, under its evidence check, being fixed or mended after
\*          it, wiring into the parent, or wired. "none" when it has none.
\* hop      the sub-ticket this captain waits on, as its officer sees it (S24): live, found stopped,
\*          or set back to Todo.
\* asked    the Question this captain's work waits on: a need it passed up that its officer has not
\*          opened yet, or an open Question at stage research, council or his-call.
\* call     for a research Question: open until its research is done, then settled, or left for a
\*          council, or left for him (L18, H23).
\* tribunal for a council: the judges' sealed positions are out, the exchange is out, the judges
\*          agreed or split, and the call was decided or prepared for him (L18).
\*
\* The review loop:
\* round    the number of the review round now or last held, 0 before any; 4 means four or more.
\* extra    whether the one more round after a PASS has been ordered (R8).
\* lens     what the current round looks with: full, full with the security lens first (R9), or
\*          scoped to a repair (R19, R22). "none" outside a round.
\* blockers the kinds of blocker the last FAIL verdict holds (R9): broken or against the spec, a
\*          security hole, fundamental or a new requirement, and a need for Balazs. Empty outside
\*          the Fix it sends.
\* cheap    a cheap should is outstanding, to be fixed in place before the merge (S17).
\* checkpoint the line the round-3 checkpoint wrote on the Review (S10).
\*
\* The merge:
\* target   where the merge goes: a feature branch, or master as a two-way or a one-way PR. The brief
\*          names it; no step depends on it before the merge, so the model reads it there.
\* pass     the PASS the merge rests on: none, on the head, or on an ancestor whose later commits
\*          are clean syncs (R23, R24).
\* gates    DYTEST holds a green receipt keyed by the combined tree (R23).
\* ci       hosted CI is green on the PR's head (R26, R31).
\* resyncs  how often the PR into master was forced to re-sync (R29).
\* landed   whether the merge is in, not yet, or reverted after a real red (R22).
VARIABLES state, kind, stage, merges, filler, door, proved, scouted, fog, part, hop, asked, call, tribunal, round, extra, lens, blockers, cheap, checkpoint, target, pass, gates, ci, resyncs, landed

ticket == <<kind, stage, merges, filler, door>>
work == <<proved, scouted>>
split == <<fog, part, tribunal>>
question == <<asked, call>>
review == <<round, extra, lens, blockers, cheap, checkpoint>>
merge == <<target, pass, gates, ci, resyncs, landed>>
vars == <<state, ticket, work, split, hop, question, review, merge>>

States == {"Dispatched", "Taken", "Leaf", "Asking", "Cutting", "Waiting",
           "InReview", "Fixing", "Mending",
           "Merging", "AwaitingTurn", "Held", "AwaitingClick", "Landed", "Rerunning", "Repairing",
           "Done", "HandedBack"}

\* The two ends: the contract is met, or the ticket went back to the officer unfinished.
Ends == {"Done", "HandedBack"}

Kinds == {"change", "verdict", "findings", "answer", "cut"}
Stages == {"research", "council", "his-call"}
Doors == {"two-way", "dr-grade overnight", "dr-grade", "one-way"}
Blockers == {"broken", "security", "fundamental", "him"}
Targets == {"feature", "two-way", "one-way"}
Master == {"two-way", "one-way"}

\* A part, or a hop beside it, that is out and may go quiet.
PartOut == {"out", "checking", "fixing", "mending", "wiring"}
HopOut == state \in {"InReview", "Fixing", "Mending", "Repairing"} \/ (state = "Waiting" /\ part \in PartOut)

\* The blockers a Fix takes, and the verdicts its officer above re-plans.
Fixable == (SUBSET {"broken", "security", "him"}) \ {{}}
Replans == {b \in SUBSET Blockers : "fundamental" \in b}

\* Who finds the way toward the spec: every captain whose route is not given (H10). A review and a
\* seat judge a candidate that is there; a council and a his-call answer one call.
Wayfinds == kind \in {"change", "cut"} \/ stage = "research"

\* The next round's number. Four stands for four or more: the rules name round 3 and nothing later,
\* and the fifth-FAIL stop is gone (R10), so the rounds go on without a bound.
NextRound == IF round < 4 THEN round + 1 ELSE 4

\* The PASS a merge rests on still stands (R23, R24).
PassStands == pass \in {"head", "ancestor"}

TypeOK ==
    /\ state \in States
    /\ kind \in Kinds \cup {"unread"}
    /\ stage \in Stages \cup {"none"}
    /\ merges \in BOOLEAN
    /\ filler \in BOOLEAN
    /\ door \in Doors \cup {"none"}
    /\ proved \in BOOLEAN
    /\ scouted \in BOOLEAN
    /\ fog \in BOOLEAN
    /\ part \in PartOut \cup {"none", "returned", "back", "wired"}
    /\ hop \in {"live", "stopped", "todo"}
    /\ asked \in Stages \cup {"none", "passed up"}
    /\ call \in {"none", "open", "settled", "council", "his"}
    /\ tribunal \in {"none", "sealed", "exchange", "agreed", "split", "decided", "prepared"}
    /\ round \in 0..4
    /\ extra \in BOOLEAN
    /\ lens \in {"none", "full", "security first", "scoped"}
    /\ blockers \in SUBSET Blockers
    /\ cheap \in BOOLEAN
    /\ checkpoint \in {"none", "one more round", "escalated"}
    /\ target \in Targets \cup {"none"}
    /\ pass \in {"none", "head", "ancestor"}
    /\ gates \in BOOLEAN
    /\ ci \in BOOLEAN
    /\ resyncs \in 0..3
    /\ landed \in {"no", "in", "reverted"}
    /\ merges => kind = "change"
    /\ (stage # "none") = (kind = "answer")
    /\ target # "none" => merges
    /\ hop # "live" => HopOut

\* The coordinator or an officer may pull a ticket as filler (H10, H11); the ticket says so.
Init ==
    /\ state = "Dispatched"
    /\ kind = "unread"
    /\ stage = "none"
    /\ merges = FALSE
    /\ filler \in BOOLEAN
    /\ door = "none"
    /\ proved = FALSE
    /\ scouted = FALSE
    /\ fog = FALSE
    /\ part = "none"
    /\ hop = "live"
    /\ asked = "none"
    /\ call = "none"
    /\ tribunal = "none"
    /\ round = 0
    /\ extra = FALSE
    /\ lens = "none"
    /\ blockers = {}
    /\ cheap = FALSE
    /\ checkpoint = "none"
    /\ target = "none"
    /\ pass = "none"
    /\ gates = FALSE
    /\ ci = FALSE
    /\ resyncs = 0
    /\ landed = "no"

\* ---- Taking the ticket ----

\* S7: one agent holds one ticket. It is held by another agent, or it is closed, so it goes back
\* untouched with the reason (S6: nobody set it In Progress).
CannotTake ==
    /\ state = "Dispatched"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* H11: the dispatched captain checks a filler's small, two-way and no-design-call itself. A filler
\* that fails goes back unassigned with a one-line reason, blocked on the open Wayfinding ticket.
FillerFailsItsCheck ==
    /\ state = "Dispatched"
    /\ filler
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* S6: whoever starts a step sets it In Progress. K30, K14, S15: the ticket and its brief say what
\* its result is and whether this captain merges it; only a change is merged. H23: a Question
\* carries its stage label, and the stage, not the parent, picks the job. L18: a council's case file
\* says what the call is. H11: a filler is a Bug or a small Feature, so its result is a change, and
\* it is taken only once its check passes. L18: every Question starts with research, so a research
\* Question's call is open.
Take ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ kind' \in IF filler THEN {"change"} ELSE Kinds
    /\ stage' \in IF kind' = "answer" THEN Stages ELSE {"none"}
    /\ merges' \in IF kind' = "change" THEN BOOLEAN ELSE {FALSE}
    /\ door' \in IF stage' = "council" THEN Doors ELSE {"none"}
    /\ call' = IF stage' = "research" THEN "open" ELSE "none"
    /\ filler' = FALSE
    /\ UNCHANGED <<work, split, hop, asked, tribunal, review, merge>>

\* ---- Sizing it ----

\* core "A ticket": the right size is what one agent can hold, about 100k tokens of context, and work
\* that needs the same thing understood belongs to one agent. S2. A council is never one agent's: two
\* judges from different families take sealed positions (L18).
WorkItself ==
    /\ state = "Taken"
    /\ stage # "council"
    /\ state' = "Leaf"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* core "A ticket": it splits the ticket and only orchestrates. H2. A review may be cut into seats
\* from another model family (R34, S1). A seat is a leaf: "They're not meant do delegate into further
\* sub agents for sure." (R34)
Split ==
    /\ state = "Taken"
    /\ kind # "findings"
    /\ state' = "Cutting"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* core "A ticket": a captain that cannot see the route to the spec finds it one manageable part at a
\* time; it does not cut the whole at once. H10: wayfinding toward the spec is part of every
\* captain's work. The ticket needs more than one agent and its route is not all in sight, so it cuts
\* only the parts in sight; when none is, one part that brings the next into sight: a Question, a
\* prototype or a Wayfinding ticket.
FindTheWay ==
    /\ state = "Taken"
    /\ Wayfinds
    /\ state' = "Cutting"
    /\ fog' = TRUE
    /\ UNCHANGED <<ticket, work, part, tribunal, hop, question, review, merge>>

\* ---- Doing the work itself ----

\* The escape hatch: a holder may still divide a slice that is too big. K30, H2. Never a seat (R34).
SplitTheRest ==
    /\ state = "Leaf"
    /\ kind # "findings"
    /\ state' = "Cutting"
    /\ proved' = FALSE
    /\ UNCHANGED <<ticket, scouted, split, hop, question, review, merge>>

\* core "Who decides what": outside its job a captain decides only obvious, reversible calls; any
\* other call becomes a Question ticket for a fresh agent. H16: the need is passed up to the officer
\* above; R5: any agent reports a problem outside its scope to its officer. Work that does not wait
\* on the answer goes on (S10: the Question "blocks only what waits on it"). A council chair runs
\* its council mechanically (L18) and passes nothing up.
PassTheNeedUp ==
    /\ state \in {"Leaf", "Waiting"}
    /\ stage # "council"
    /\ asked = "none"
    /\ asked' = "passed up"
    /\ UNCHANGED <<state, ticket, work, split, hop, call, review, merge>>

\* H16: "the receiving officer opens it (a sub-ticket under a captain, or a ticket under the
\* coordinator) and waits on it", held by a fresh captain. L18, H23: every Question starts with
\* research, so it opens at stage research. S10: it blocks only what waits on it.
QuestionOpened ==
    /\ asked = "passed up"
    /\ state \notin Ends
    /\ asked' = "research"
    /\ UNCHANGED <<state, ticket, work, split, hop, call, review, merge>>

\* H16, S10: what is left of the work waits on the open Question, and the captain waits on it.
WaitOnTheAnswer ==
    /\ state = "Leaf"
    /\ asked \in Stages
    /\ state' = "Asking"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* L18: "every Question starts with research". H16: the Question captain researches first, then
\* answers the need. A call a DR clearly settles is not hard: the holder decides and cites the DR.
\* A hard call worth his time goes toward him; any other hard call goes to a council (L18, L20).
\* H23: its captain loads /ticket-captain and dispatches research crew, so the research is done in
\* the leaf or by its parts.
ResearchTheCall ==
    /\ call = "open"
    /\ \/ state = "Leaf"
       \/ state = "Waiting" /\ part = "back"
    /\ call' \in {"settled", "council", "his"}
    /\ UNCHANGED <<state, ticket, work, split, hop, asked, review, merge>>

\* H23: "A research captain opens a council or his-call child only when its research does not
\* resolve the call." L18, L23: a hard call not worth his time goes to a council, a Question at stage
\* council, Mode AFK, chaired by a fresh ticket captain. The research captain waits on its answer.
AskACouncil ==
    /\ call = "council"
    /\ asked = "none"
    /\ \/ state = "Leaf"
       \/ state = "Waiting" /\ part = "back"
    /\ asked' = "council"
    /\ state' = IF state = "Leaf" THEN "Asking" ELSE state
    /\ UNCHANGED <<ticket, work, split, hop, call, review, merge>>

\* H16, H23, L18: a call worth his time goes toward his HITL queue as a his-call child, Mode HITL,
\* with his question made ready on it: every option, each trade-off, a recommendation (core "Who
\* decides what"). The coordinator dispatches an envoy onto that same ticket when its turn comes
\* (H16, H18). L18, L20: so does a call a council prepared for him. The research captain waits on
\* his answer.
AskHim ==
    /\ call = "his"
    /\ asked = "none"
    /\ \/ state = "Leaf"
       \/ state = "Waiting" /\ part = "back"
    /\ asked' = "his-call"
    /\ state' = IF state = "Leaf" THEN "Asking" ELSE state
    /\ UNCHANGED <<ticket, work, split, hop, call, review, merge>>

\* H16: the work waits on the answer and then goes on; it never ends from the wait (519126ab). His
\* answer on a his-call child settles a research Question's call (H23).
Answered ==
    /\ asked \in {"research", "his-call"}
    /\ state \in {"Leaf", "Asking", "Cutting", "Waiting", "Fixing"}
    /\ asked' = "none"
    /\ call' = IF asked = "his-call" THEN "settled" ELSE call
    /\ state' = IF state = "Asking" THEN "Leaf" ELSE state
    /\ UNCHANGED <<ticket, work, split, hop, review, merge>>

\* L18: the judges agree on a two-way hard call, so the council decided it; the research captain
\* answers from it.
CouncilDecided ==
    /\ asked = "council"
    /\ state \in {"Asking", "Waiting"}
    /\ asked' = "none"
    /\ call' = "settled"
    /\ state' = IF state = "Asking" THEN "Leaf" ELSE state
    /\ UNCHANGED <<ticket, work, split, hop, review, merge>>

\* L18: the council prepared the call for him with both positions; L20: by day the tribunal
\* prepares and he decides. The call is still not resolved, so it goes toward him (H23).
CouncilPrepared ==
    /\ asked = "council"
    /\ state \in {"Asking", "Waiting"}
    /\ asked' = "none"
    /\ call' = "his"
    /\ state' = IF state = "Asking" THEN "Leaf" ELSE state
    /\ UNCHANGED <<ticket, work, split, hop, review, merge>>

\* R5: "The author proves with receipts"; core "Who decides what": an agent verifies its own work
\* wherever it can. The work is whole: nothing in it waits on an answer.
ProveTheWork ==
    /\ state = "Leaf"
    /\ kind = "change"
    /\ ~proved
    /\ asked = "none"
    /\ proved' = TRUE
    /\ UNCHANGED <<state, ticket, scouted, split, hop, question, review, merge>>

\* R4: no review loop on a plan, spec or contract. A spec gets one scout pass that returns
\* Questions, never PASS or FAIL, and is never repeated; it is optional, by risk. K30: the scout is a
\* native helper. A Wayfinding captain refines the spec and cuts (H4, S14). The Questions it
\* returns are passed up like any other need.
ScoutTheSpec ==
    /\ state = "Leaf"
    /\ kind = "cut"
    /\ ~scouted
    /\ scouted' = TRUE
    /\ UNCHANGED <<state, ticket, proved, split, hop, question, review, merge>>

\* core: an agent never judges its own work, the verdict is another agent's. A merge needs a PASS on
\* its head, so the captain that merges is reviewed first (R23, R33), after its own proof (R5). The
\* first round looks with the full lenses of its level (R6).
SendLeafToReview ==
    /\ state = "Leaf"
    /\ merges
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ round' = 1
    /\ lens' = "full"
    /\ proved' = FALSE
    /\ UNCHANGED <<ticket, scouted, split, hop, question, extra, blockers, cheap, checkpoint, merge>>

\* core "A ticket": done when its contract is met. Nothing to merge: a verdict (never reviewed again:
\* core, R34), findings, an answer, a cut, or a part its officer reviews with the whole (R2; R6: a
\* leaf gets no full review, only an optional evidence check beside it, S4). A change is proved
\* first (R5), and a research Question's call is settled (H16, H23).
CloseLeaf ==
    /\ state = "Leaf"
    /\ ~merges
    /\ asked = "none"
    /\ kind = "change" => proved
    /\ call \in {"none", "settled"}
    /\ state' = "Done"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* H2: what a holder cannot settle under its own ticket goes back to the officer above.
\* core: a problem outside your ticket goes to your officer; what matters is that it surfaces.
HandBackTheWork ==
    /\ state = "Leaf"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* ---- Splitting it ----

\* A sub-ticket goes out by delegate_task, and its brief names the skills its position needs.
\* core, H3, K14. S3: parts may be opened ahead in Todo; the ready ones go out. A council's first
\* parts are its two judges' sealed positions (L18).
Dispatch ==
    /\ state = "Cutting"
    /\ state' = "Waiting"
    /\ part' = "out"
    /\ tribunal' = IF stage = "council" THEN "sealed" ELSE tribunal
    /\ UNCHANGED <<ticket, work, fog, hop, question, review, merge>>

\* H2: a slice that cannot be divided coherently under its own ticket goes back to the officer
\* above for a re-cut.
HandBackTheCut ==
    /\ state = "Cutting"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* S24: liveness fires on a hop In Progress and quiet for 2 hours; the officer above checks it.
\* Alive snoozes and a record behind is brought up to date, and neither changes a state. Stopped:
SomeHopStops ==
    /\ HopOut
    /\ hop = "live"
    /\ hop' = "stopped"
    /\ UNCHANGED <<state, ticket, work, split, question, review, merge>>

\* S24: "stopped sets the hop back to Todo,
SetTheHopBackToTodo ==
    /\ hop = "stopped"
    /\ hop' = "todo"
    /\ UNCHANGED <<state, ticket, work, split, question, review, merge>>

\* then resumes the thread or dispatches a fresh agent that attaches a new thread link."
ResumeTheHop ==
    /\ hop = "todo"
    /\ hop' = "live"
    /\ UNCHANGED <<state, ticket, work, split, question, review, merge>>

\* H10: a part returns. So does its fix or its mend (S9, S17).
PartBack ==
    /\ state = "Waiting"
    /\ part \in {"out", "fixing", "mending"}
    /\ hop = "live"
    /\ tribunal # "exchange"
    /\ part' = "back"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* H2: a holder that cannot divide its slice coherently under its own ticket returns it to the
\* officer above for a re-cut.
PartHandedBack ==
    /\ state = "Waiting"
    /\ part = "out"
    /\ hop = "live"
    /\ stage # "council"
    /\ part' = "returned"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* H10: when a part is back and another is ready (S3: opened ahead in Todo, its blocker now done),
\* it is dispatched.
DispatchAReadyPart ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ stage # "council"
    /\ part' = "out"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* R6: "A leaf or lane gets no full review, only an optional fresh evidence-check seat." S4: "A
\* leaf's evidence check is a Review hop beside it." R34: the leaf evidence check reads it locally
\* and runs its receipts.
CheckThePart ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ kind = "change"
    /\ part' = "checking"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* The evidence check finds no blocker and no cheap should: the part stands.
TheCheckPasses ==
    /\ state = "Waiting"
    /\ part = "checking"
    /\ hop = "live"
    /\ part' = "back"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* S17: a should is "fixed in place before the parent merges if cheap". The part mends it, and its
\* parent merges: who mends is the part's holder, who merges is this captain.
TheCheckFindsACheapShould ==
    /\ state = "Waiting"
    /\ part = "checking"
    /\ hop = "live"
    /\ part' = "mending"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* core "Review severities": a blocker is fixed now. S9: by default the fix goes to the thread that
\* wrote the part, and to a fresh agent when its context is heavy or the approach was wrong.
TheCheckFails ==
    /\ state = "Waiting"
    /\ part = "checking"
    /\ hop = "live"
    /\ part' = "fixing"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* H16: a part passes a need up, and this captain is the receiving officer: it opens a Question as
\* a sub-ticket of its own ticket, at stage research (L18, H23), dispatches a fresh captain onto it
\* and waits on it. The part waits on the answer; nothing else does (S10).
NeedFromAPart ==
    /\ state = "Waiting"
    /\ part = "out"
    /\ hop = "live"
    /\ stage # "council"
    /\ asked = "none"
    /\ asked' = "research"
    /\ UNCHANGED <<state, ticket, work, split, hop, call, review, merge>>

\* core "A ticket": the route is found one manageable part at a time; the whole is never cut at
\* once. H10: a part is back, no ready part is left and the contract is not met, so it cuts the next
\* parts, now in sight. H2: a part handed back is cut again another way.
CutTheNextParts ==
    /\ state = "Waiting"
    /\ part \in {"back", "returned"}
    /\ stage # "council"
    /\ state' = "Cutting"
    /\ fog' = FALSE
    /\ part' = "none"
    /\ UNCHANGED <<ticket, work, tribunal, hop, question, review, merge>>

\* H10: "When a captain has no ticket left to dispatch or pull, it checks its own contract against the
\* spec: [...] if not, it wayfinds the next part toward the spec and cuts it as a sub-ticket." The
\* route ahead is still not all in sight.
FogRemains ==
    /\ state = "Waiting"
    /\ part \in {"back", "returned"}
    /\ Wayfinds
    /\ state' = "Cutting"
    /\ fog' = TRUE
    /\ part' = "none"
    /\ UNCHANGED <<ticket, work, tribunal, hop, question, review, merge>>

\* H2: a change to its contract, or to a sibling's, goes through the officer above. R9: what is
\* fundamental or a new requirement is re-planned by the officer above. A part shows it.
HandBackTheWhole ==
    /\ state = "Waiting"
    /\ part \in {"back", "returned"}
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R2: the parent is reviewed once, "after wiring and before its Merge hop". H10: the contract is
\* met: the parts are back and no route is left in fog. R18, R20: each part's merge into the parent
\* is a Merge hop, proved by the combined gates on its merge SHA.
WireTheParts ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ merges
    /\ ~fog
    /\ asked = "none"
    /\ part' = "wiring"
    /\ UNCHANGED <<state, ticket, work, fog, tribunal, hop, question, review, merge>>

\* R18, R20: the wiring is in, with its gates green on the merge tree: the whole's receipts (R5).
TheWiringIsIn ==
    /\ state = "Waiting"
    /\ part = "wiring"
    /\ hop = "live"
    /\ part' = "wired"
    /\ proved' = TRUE
    /\ UNCHANGED <<state, ticket, scouted, fog, tribunal, hop, question, review, merge>>

\* R2: the whole is reviewed once, after wiring and before its merge, with the lenses of its level
\* (R6), after the author's proof (R5).
SendTheWholeToReview ==
    /\ state = "Waiting"
    /\ part = "wired"
    /\ merges
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ part' = "none"
    /\ proved' = FALSE
    /\ round' = 1
    /\ lens' = "full"
    /\ UNCHANGED <<ticket, scouted, fog, tribunal, hop, question, extra, blockers, cheap, checkpoint, merge>>

\* H10: the contract is met, so it finishes its ticket. R6: a ticket that only groups its parts needs
\* no review of its own. S8: a review whose seats are back posts the round's one verdict, naming each
\* seat. A research Question's call is settled (H16, H23).
CloseTheWhole ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ ~merges
    /\ ~fog
    /\ asked = "none"
    /\ stage # "council"
    /\ call \in {"none", "settled"}
    /\ state' = "Done"
    /\ part' = "none"
    /\ UNCHANGED <<ticket, work, fog, tribunal, hop, question, review, merge>>

\* ---- A council (L18, L23) ----

\* L18: the sealed positions are back; "one exchange follows: each answers the other's strongest
\* point or concedes."
TheExchange ==
    /\ state = "Waiting"
    /\ tribunal = "sealed"
    /\ part = "back"
    /\ part' = "out"
    /\ tribunal' = "exchange"
    /\ UNCHANGED <<state, ticket, work, fog, hop, question, review, merge>>

\* L18: after the exchange the judges agree,
TheJudgesAgree ==
    /\ state = "Waiting"
    /\ tribunal = "exchange"
    /\ part = "out"
    /\ hop = "live"
    /\ part' = "back"
    /\ tribunal' = "agreed"
    /\ UNCHANGED <<state, ticket, work, fog, hop, question, review, merge>>

\* or they disagree, or the precedent points both ways.
TheJudgesSplit ==
    /\ state = "Waiting"
    /\ tribunal = "exchange"
    /\ part = "out"
    /\ hop = "live"
    /\ part' = "back"
    /\ tribunal' = "split"
    /\ UNCHANGED <<state, ticket, work, fog, hop, question, review, merge>>

\* L18: "the judges agree on a two-way hard call → decided, citing the law and precedent". L21:
\* overnight, a two-way, seriously blocking DR-grade call may be decided by both judges agreeing, as a
\* proposed DR he ratifies. L23: its outcome is an answer, recorded on the ticket.
TheCouncilDecides ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ tribunal = "agreed"
    /\ door \in {"two-way", "dr-grade overnight"}
    /\ state' = "Done"
    /\ part' = "none"
    /\ tribunal' = "decided"
    /\ UNCHANGED <<ticket, work, fog, hop, question, review, merge>>

\* L18: "they disagree, or the precedent points both ways → prepared for him with both positions";
\* "one-way or DR-grade → always prepared, except the rare overnight case in L21". L21: irreversible
\* calls are never decided.
TheCouncilPrepares ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ \/ tribunal = "split"
       \/ tribunal = "agreed" /\ door \in {"dr-grade", "one-way"}
    /\ state' = "Done"
    /\ part' = "none"
    /\ tribunal' = "prepared"
    /\ UNCHANGED <<ticket, work, fog, hop, question, review, merge>>

\* ---- The review loop (R8, R9, R10, S9, S10, S17) ----

\* R8: "the phase ends on the first round with no new blocker", and the officer running the rounds
\* orders no more. core "Review severities", R11: PASS means no blocker. S17: each should that waits
\* becomes a ticket; a cheap one is fixed in place before the merge, so none is left. S11: each nit
\* gets a ruling. R23: the merge rests on this PASS, on the head. The merge comes next, into the
\* target the brief names (R25, R26, R27).
NoMoreRound ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ ~cheap
    /\ state' = "Merging"
    /\ target' \in IF target = "none" THEN Targets ELSE {target}
    /\ pass' = "head"
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, round, extra, blockers, cheap, checkpoint, gates, ci, resyncs, landed>>

\* R8: the verdict is PASS, and "the officer may still order one more", once. The next round is
\* fresh, with the full lenses. A should it found stays outstanding (S17).
OneMoreRound ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ ~extra
    /\ round' = NextRound
    /\ extra' = TRUE
    /\ lens' = "full"
    /\ cheap' \in {cheap, TRUE}
    /\ UNCHANGED <<state, ticket, work, split, hop, question, blockers, checkpoint, merge>>

\* S17: the verdict is PASS and a should "is fixed in place before the parent merges if cheap". By
\* default the thread that wrote the change mends it (S9's default for who fixes); this captain
\* merges. The mend moves the head.
MendACheapShould ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ state' = "Mending"
    /\ cheap' = TRUE
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, round, extra, blockers, checkpoint, merge>>

\* R23, R24: a PASS stands only on its head, or on an ancestor whose later commits are clean syncs.
\* A mend is neither, so the mended head gets a round of its own.
ReviewTheMendedHead ==
    /\ state = "Mending"
    /\ hop = "live"
    /\ state' = "InReview"
    /\ cheap' = FALSE
    /\ round' = NextRound
    /\ lens' = "full"
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, blockers, checkpoint, merge>>

\* core "Review severities": FAIL means at least one blocker, and a blocker is fixed now. R9: broken
\* or against the spec goes to a Fix, then the next round; a security hole too, with the security
\* lens first next; a need for Balazs goes up at once. S9: at verdict time the blockers become one
\* Fix rN ticket listing all of them. S10, H16: the need for Balazs becomes a Question ticket that
\* blocks only what waits on it: this captain, receiving it in the verdict, opens it at stage
\* research (L18) and the Fix waits on its answer. Round 3 has the checkpoint instead (R10).
Fail ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ round # 3
    /\ state' = "Fixing"
    /\ blockers' \in Fixable
    /\ asked' = IF "him" \in blockers' THEN "research" ELSE asked
    /\ cheap' \in {cheap, TRUE}
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, call, round, extra, checkpoint, merge>>

\* R9: a blocker that is fundamental or a new requirement goes to the officer above to re-plan
\* (H2, H5). S10: dead hops go Canceled with a one-line reason.
Replan ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ round # 3
    /\ state' = "HandedBack"
    /\ blockers' \in Replans
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, round, extra, cheap, checkpoint, merge>>

\* R10: "After round 3 comes a checkpoint, not a stop: by default one more round." "At round 3, the
\* officer running the rounds decides on purpose." A third round still has a blocker, and every one
\* is fixable in a Fix (R9), so the decision is one more round. S10: "checkpoint: one more round,
\* because ...". The blockers go to a Fix, and its round looks as R8 and R9 say.
CheckpointOneMoreRound ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ round = 3
    /\ state' = "Fixing"
    /\ blockers' \in (SUBSET {"broken", "security"}) \ {{}}
    /\ checkpoint' = "one more round"
    /\ cheap' \in {cheap, TRUE}
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, round, extra, merge>>

\* R10: escalating "for something that needs him". S10: "escalated: ...", and the need becomes a
\* Question ticket that blocks only what waits on it (H16, L18): the Fix waits on its answer, then the
\* next round.
CheckpointAsksHim ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ round = 3
    /\ state' = "Fixing"
    /\ blockers' \in {b \in Fixable : "him" \in b}
    /\ asked' = "research"
    /\ checkpoint' = "escalated"
    /\ cheap' \in {cheap, TRUE}
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, call, round, extra, merge>>

\* R10: escalating "for a fundamental break, a new requirement". R9: the officer above re-plans.
\* S10: "escalated: ...".
CheckpointEscalates ==
    /\ state = "InReview"
    /\ hop = "live"
    /\ round = 3
    /\ state' = "HandedBack"
    /\ blockers' \in Replans
    /\ checkpoint' = "escalated"
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, round, extra, cheap, merge>>

\* R8: a fix between every two rounds, and "a round with a blocker is always followed by a
\* full-lens round"; R9: after a security hole the security lens comes first. S9: Fix rN is Done
\* when its blockers are Done and proof ran on the head, so nothing in it waits on an answer; the
\* next round is a fresh look, whoever did the Fix.
ReviewAgain ==
    /\ state = "Fixing"
    /\ hop = "live"
    /\ asked = "none"
    /\ state' = "InReview"
    /\ round' = NextRound
    /\ lens' = IF "security" \in blockers THEN "security first" ELSE "full"
    /\ blockers' = {}
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, cheap, checkpoint, merge>>

\* The Fix's holder hands it back undone, and this captain passes it on. H2
HandBackTheFix ==
    /\ state = "Fixing"
    /\ hop = "live"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* ---- The merge (R18 to R31) ----

\* R21: "The captain runs dydo merge and dydo test gate itself." R23: dydo merge refuses unless
\* DYTEST holds a green receipt keyed by the combined tree.
RunTheGates ==
    /\ state = "Merging"
    /\ ~gates
    /\ gates' = TRUE
    /\ UNCHANGED <<state, ticket, work, split, hop, question, review, target, pass, ci, resyncs, landed>>

\* R26: a PR into master merges when CI is green; R31: the captain watches its own CI.
WatchTheCi ==
    /\ state = "Merging"
    /\ target \in Master
    /\ ~ci
    /\ ci' = TRUE
    /\ UNCHANGED <<state, ticket, work, split, hop, question, review, target, pass, gates, resyncs, landed>>

\* R23: dydo merge refuses without the green receipt. R21: the captain "sends an implementer only for
\* a CONFLICT or a real red gate": a Fix on the ticket branch. Nothing has landed, so nothing is
\* reverted.
RedBeforeLanding ==
    /\ state = "Merging"
    /\ ~gates \/ (target \in Master /\ ~ci)
    /\ state' = "Repairing"
    /\ pass' = "none"
    /\ gates' = FALSE
    /\ ci' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, resyncs, landed>>

\* R19, R21, R23: dydo merge refuses a merge with the tip that is not clean. A CONFLICT is resolved
\* only on the ticket branch, by an implementer the captain sends.
MergeConflict ==
    /\ state = "Merging"
    /\ state' = "Repairing"
    /\ pass' = "none"
    /\ gates' = FALSE
    /\ ci' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, resyncs, landed>>

\* R18, R25: into a feature branch freely. R23: dydo merge lands it only when the PASS names the head
\* or an ancestor as R24 allows, the merge with the tip is clean and DYTEST holds a green receipt.
\* R22: a re-land after a revert is dydo merge reverting the revert.
MergeIntoFeature ==
    /\ state = "Merging"
    /\ target = "feature"
    /\ PassStands
    /\ gates
    /\ state' = "Landed"
    /\ landed' = "in"
    /\ gates' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, ci, resyncs>>

\* R26: a two-way PR into master merges AFK when its evidence is complete: a PASS on the head or an
\* ancestor, gates green on the combined tree with current master, CI green, a PR body that declares
\* it two-way. R29: main-bound merges are paced, so it waits on the coordinator's turn.
OfferForTheTurn ==
    /\ state = "Merging"
    /\ target = "two-way"
    /\ PassStands
    /\ gates
    /\ ci
    /\ state' = "AwaitingTurn"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* core "Who decides what": Balazs decides anything one-way. R27: a one-way PR needs his click and
\* carries a recovery plan. R31: "Before his click, the captain watches its own CI and opens the HITL
\* Merge only when it is green." S12: a HITL Merge only for a one-way one.
OpenTheHitlMerge ==
    /\ state = "Merging"
    /\ target = "one-way"
    /\ PassStands
    /\ gates
    /\ ci
    /\ state' = "AwaitingClick"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R29: the coordinator gives the turn, the PR re-checks against current master just before landing,
\* and the captain merges it (R26).
LandOnTheTurn ==
    /\ state = "AwaitingTurn"
    /\ state' = "Landed"
    /\ landed' = "in"
    /\ gates' = FALSE
    /\ ci' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, resyncs>>

\* R29: the re-check finds master moved, and the PR re-syncs. R24: a clean sync whose second parent is
\* in the target keeps the PASS, now on an ancestor; the gates and CI run again on the new tree. "A PR
\* forced to re-sync a third time is held while the coordinator sequences the colliding work."
ResyncOnTheTurn ==
    /\ state = "AwaitingTurn"
    /\ resyncs < 3
    /\ resyncs' = resyncs + 1
    /\ state' = IF resyncs' = 3 THEN "Held" ELSE "Merging"
    /\ pass' = "ancestor"
    /\ gates' = FALSE
    /\ ci' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, landed>>

\* R19, R29: the sync with current master conflicts; it is resolved on the ticket branch.
ConflictOnTheTurn ==
    /\ state = "AwaitingTurn"
    /\ state' = "Repairing"
    /\ pass' = "none"
    /\ gates' = FALSE
    /\ ci' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, resyncs, landed>>

\* R29: the coordinator has sequenced the colliding work; the PR runs its evidence again.
Sequenced ==
    /\ state = "Held"
    /\ state' = "Merging"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R27, R31: his click lands it, and "the survey sees 'PR merged, HITL Merge open' and wakes the
\* captain".
HisClick ==
    /\ state = "AwaitingClick"
    /\ state' = "Landed"
    /\ landed' = "in"
    /\ gates' = FALSE
    /\ ci' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, resyncs>>

\* R20: the Merge hop is Done when DYTEST holds a green receipt on the merge tree and hosted CI is
\* green on the merge SHA. core: the ticket is done when its contract is met.
GreenOnTheMergeSha ==
    /\ state = "Landed"
    /\ state' = "Done"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R22: "A red gate after a merge: DYTEST reruns once."
GateRed ==
    /\ state = "Landed"
    /\ state' = "Rerunning"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R22: "A flake means the hop is Done and the flake is counted as a DYTEST result."
Flake ==
    /\ state = "Rerunning"
    /\ state' = "Done"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R22: "A real red means the captain reverts at once [...], a Fix follows on the ticket branch".
\* S12: the reverted Merge goes Canceled with "reverted: <gate> red on <merge SHA>".
RealRed ==
    /\ state = "Rerunning"
    /\ state' = "Repairing"
    /\ landed' = "reverted"
    /\ pass' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, gates, ci, resyncs>>

\* R19: "under a fresh review of its hunks". R22: "then a fix-scoped round, then a re-land".
RepairReviewed ==
    /\ state = "Repairing"
    /\ hop = "live"
    /\ state' = "InReview"
    /\ round' = NextRound
    /\ lens' = "scoped"
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, blockers, cheap, checkpoint, merge>>

\* H2: a conflict that cannot be resolved, or a red gate that cannot be fixed, under the ticket goes
\* back to the officer above.
RepairFails ==
    /\ state = "Repairing"
    /\ hop = "live"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

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
    \/ PassTheNeedUp
    \/ QuestionOpened
    \/ WaitOnTheAnswer
    \/ ResearchTheCall
    \/ AskACouncil
    \/ AskHim
    \/ Answered
    \/ CouncilDecided
    \/ CouncilPrepared
    \/ ProveTheWork
    \/ ScoutTheSpec
    \/ SendLeafToReview
    \/ CloseLeaf
    \/ HandBackTheWork
    \/ Dispatch
    \/ HandBackTheCut
    \/ SomeHopStops
    \/ SetTheHopBackToTodo
    \/ ResumeTheHop
    \/ PartBack
    \/ PartHandedBack
    \/ DispatchAReadyPart
    \/ CheckThePart
    \/ TheCheckPasses
    \/ TheCheckFindsACheapShould
    \/ TheCheckFails
    \/ NeedFromAPart
    \/ CutTheNextParts
    \/ FogRemains
    \/ HandBackTheWhole
    \/ WireTheParts
    \/ TheWiringIsIn
    \/ SendTheWholeToReview
    \/ CloseTheWhole
    \/ TheExchange
    \/ TheJudgesAgree
    \/ TheJudgesSplit
    \/ TheCouncilDecides
    \/ TheCouncilPrepares
    \/ NoMoreRound
    \/ OneMoreRound
    \/ MendACheapShould
    \/ ReviewTheMendedHead
    \/ Fail
    \/ Replan
    \/ CheckpointOneMoreRound
    \/ CheckpointAsksHim
    \/ CheckpointEscalates
    \/ ReviewAgain
    \/ HandBackTheFix
    \/ RunTheGates
    \/ WatchTheCi
    \/ RedBeforeLanding
    \/ MergeConflict
    \/ MergeIntoFeature
    \/ OfferForTheTurn
    \/ OpenTheHitlMerge
    \/ LandOnTheTurn
    \/ ResyncOnTheTurn
    \/ ConflictOnTheTurn
    \/ Sequenced
    \/ HisClick
    \/ GreenOnTheMergeSha
    \/ GateRed
    \/ Flake
    \/ RealRed
    \/ RepairReviewed
    \/ RepairFails
    \/ Ended

Spec == Init /\ [][Next]_vars
====

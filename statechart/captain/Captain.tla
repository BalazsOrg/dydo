---- MODULE Captain ----
\* A captain and its ticket: the comprehensive model, the hand-written twin of the process the role
\* skills describe. It holds every state and every rule of the process that has an order to check.
\* Written by hand; no build step writes this file.
\*
\* chart.mmd beside it is the simple view Balazs approved for reading (DYD-1440). The view is a
\* projection of this model: view-map.mjs shows each model state as one chart state, and puts each
\* action under one chart arrow or inside one chart state. check-chart.mjs runs TLC on this file, reads
\* the graph of every reachable step and proves the projection. A skill line is checked against this
\* model; the chart is for reading.
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
\* proved   the author's receipts are in for the head as it stands (R5): a leaf's change, a whole
\*          whose wiring gates are green (R18, R20), or a head a Fix, a mend or a repair changed (S9).
\* scouts   how many scout passes ran on a cut's spec (R4).
\* fog      the route to the spec is not all in sight, so only the parts in sight are cut (H10).
\* part     the sub-tickets a captain that splits waits on, as one abstract part: out, handed back
\*          for a re-cut ("returned"), back, under its evidence check, being fixed or mended after
\*          it, wiring into the parent, or wired. "none" when it has none.
\* partwaits the part waits on a Question this captain opened for its need, as the receiving officer
\*          (H16, S10).
\* hop      the sub-ticket this captain waits on, as its officer sees it (S24): live, found stopped,
\*          or set back to Todo.
\* asked    the Question this captain's own work waits on: a need it passed up that its officer has
\*          not opened yet, or an open Question at stage research, council or his-call.
\* call     for a research Question: open until its research is done, then settled, or left for a
\*          council, or left for him (L18, H23). For a his-call Question: his, until his answer
\*          settles it (core "Who decides what", H23).
\* tribunal for a council: the judges' sealed positions are out, the exchange is out, the judges
\*          agreed or split, and the call was decided or prepared for him (L18). For a research
\*          Question, what the council child it asked did with the call: decided it, or prepared it.
\*
\* The review loop:
\* round    the number of the review round now or last held, 0 before any; 4 means four or more.
\* extra    whether the one more round after a PASS has been ordered (R8).
\* lens     what the current round looks with: full, full with the security lens first (R9), or
\*          scoped to a repair (R19, R22). "none" outside a round.
\* verdict  the round's verdict on its Review sub-ticket (R17): due while a fresh reviewer holds the
\*          round's Review hop, then PASS or FAIL as its holder wrote it, until this captain acts on
\*          it. For a captain whose ticket is a verdict, the verdict it wrote.
\* blockers the kinds of blocker the verdict holds (R9): broken or against the spec, a security
\*          hole, fundamental or a new requirement, and a need for Balazs. Kept through the Fix it
\*          sends; empty under a PASS and from a round's start.
\* cheap    a cheap should is outstanding, to be fixed in place before the merge (S17).
\* checkpoint the line the round-3 checkpoint wrote on the Review (S10).
\* hunks    a repair's hunks, written on the ticket branch, that no review's verdict has covered yet
\*          (R19, R22).
\*
\* The merge:
\* target   where the merge goes: a feature branch, or master as a two-way or a one-way PR. The brief
\*          names it; no step depends on it before the merge, so the model reads it there.
\* pass     the PR's one-line PASS the merge rests on (R17): none, on the head, or on an ancestor whose
\*          later commits are clean syncs (R23, R24).
\* gates    the gates on the combined tree: not run, a green DYTEST receipt, a real red, or a CONFLICT
\*          where the merge with the tip is not clean (R19, R21, R23). After the landing, the same on
\*          the merge tree (R20, R22).
\* ci       hosted CI on the PR's head, and after the landing on the merge SHA: not run, green or red
\*          (R20, R26, R31).
\* behind   master has moved since the PR into it last synced (R29).
\* resyncs  how often the PR into master was forced to re-sync since its last hold (R29).
\* landed   whether the merge is in, not yet, or reverted after a real red (R22).
VARIABLES state, kind, stage, merges, filler, door, proved, scouts, fog, part, partwaits, hop, asked, call, tribunal, round, extra, lens, verdict, blockers, cheap, checkpoint, hunks, target, pass, gates, ci, behind, resyncs, landed

ticket == <<kind, stage, merges, filler, door>>
work == <<proved, scouts>>
split == <<fog, part, partwaits, tribunal>>
question == <<asked, call>>
review == <<round, extra, lens, verdict, blockers, cheap, checkpoint, hunks>>
merge == <<target, pass, gates, ci, behind, resyncs, landed>>
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

\* A part, or a hop beside it, that is out and may go quiet. A round's Review hop is out until its
\* verdict is written.
PartOut == {"out", "checking", "fixing", "mending", "wiring"}
HopOut == \/ state = "InReview" /\ verdict = "due"
          \/ state \in {"Fixing", "Mending", "Repairing"}
          \/ state = "Waiting" /\ part \in PartOut

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
    /\ scouts \in 0..1
    /\ fog \in BOOLEAN
    /\ part \in PartOut \cup {"none", "returned", "back", "wired"}
    /\ partwaits \in BOOLEAN
    /\ hop \in {"live", "stopped", "todo"}
    /\ asked \in Stages \cup {"none", "passed up"}
    /\ call \in {"none", "open", "settled", "council", "his"}
    /\ tribunal \in {"none", "sealed", "exchange", "agreed", "split", "decided", "prepared"}
    /\ round \in 0..4
    /\ extra \in BOOLEAN
    /\ lens \in {"none", "full", "security first", "scoped"}
    /\ verdict \in {"none", "due", "pass", "fail"}
    /\ blockers \in SUBSET Blockers
    /\ cheap \in BOOLEAN
    /\ checkpoint \in {"none", "one more round", "escalated"}
    /\ hunks \in BOOLEAN
    /\ target \in Targets \cup {"none"}
    /\ pass \in {"none", "head", "ancestor"}
    /\ gates \in {"none", "green", "red", "conflict"}
    /\ ci \in {"none", "green", "red"}
    /\ behind \in BOOLEAN
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
    /\ scouts = 0
    /\ fog = FALSE
    /\ part = "none"
    /\ partwaits = FALSE
    /\ hop = "live"
    /\ asked = "none"
    /\ call = "none"
    /\ tribunal = "none"
    /\ round = 0
    /\ extra = FALSE
    /\ lens = "none"
    /\ verdict = "none"
    /\ blockers = {}
    /\ cheap = FALSE
    /\ checkpoint = "none"
    /\ hunks = FALSE
    /\ target = "none"
    /\ pass = "none"
    /\ gates = "none"
    /\ ci = "none"
    /\ behind = FALSE
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
\* Question's call is open. H23, core "Who decides what": a his-call Question carries a call that is
\* his to make.
Take ==
    /\ state = "Dispatched"
    /\ state' = "Taken"
    /\ kind' \in IF filler THEN {"change"} ELSE Kinds
    /\ stage' \in IF kind' = "answer" THEN Stages ELSE {"none"}
    /\ merges' \in IF kind' = "change" THEN BOOLEAN ELSE {FALSE}
    /\ door' \in IF stage' = "council" THEN Doors ELSE {"none"}
    /\ call' = CASE stage' = "research" -> "open" [] stage' = "his-call" -> "his" [] OTHER -> "none"
    /\ filler' = FALSE
    /\ UNCHANGED <<work, split, hop, asked, review, merge>>

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
    /\ UNCHANGED <<ticket, work, part, partwaits, tribunal, hop, question, review, merge>>

\* ---- Doing the work itself ----

\* The escape hatch: a holder may still divide a slice that is too big. K30, H2. Never a seat (R34).
SplitTheRest ==
    /\ state = "Leaf"
    /\ kind # "findings"
    /\ state' = "Cutting"
    /\ proved' = FALSE
    /\ UNCHANGED <<ticket, scouts, split, hop, question, review, merge>>

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
    /\ stage = "research"
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
    /\ stage = "research"
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

\* L18: the judges agree on a two-way hard call, so the council decided it; L23: its outcome is an
\* answer, recorded on its ticket. The research captain answers from it.
CouncilDecided ==
    /\ asked = "council"
    /\ state \in {"Asking", "Waiting"}
    /\ asked' = "none"
    /\ tribunal' = "decided"
    /\ call' = "settled"
    /\ state' = IF state = "Asking" THEN "Leaf" ELSE state
    /\ UNCHANGED <<ticket, work, fog, part, partwaits, hop, review, merge>>

\* L18: the council prepared the call for him with both positions; L20: by day the tribunal
\* prepares and he decides. The call is still not resolved, so it goes toward him (H23).
CouncilPrepared ==
    /\ asked = "council"
    /\ state \in {"Asking", "Waiting"}
    /\ asked' = "none"
    /\ tribunal' = "prepared"
    /\ call' = "his"
    /\ state' = IF state = "Asking" THEN "Leaf" ELSE state
    /\ UNCHANGED <<ticket, work, fog, part, partwaits, hop, review, merge>>

\* core "Who decides what": Balazs decides every call that is his to make, and an agent brings him a
\* question with every option, each option's trade-off, and its recommendation. H23: a his-call
\* Question is the envoy ticket itself, Mode HITL; the coordinator dispatches an envoy onto it when it
\* reaches the head of his HITL queue (H16, H18), and the envoy loads co-thinker. His answer settles
\* the call; nothing else does.
HeDecides ==
    /\ stage = "his-call"
    /\ call = "his"
    /\ state \in {"Leaf", "Waiting"}
    /\ call' = "settled"
    /\ UNCHANGED <<state, ticket, work, split, hop, asked, review, merge>>

\* R5: "The author proves with receipts"; core "Who decides what": an agent verifies its own work
\* wherever it can. The work is whole: nothing in it waits on an answer.
ProveTheWork ==
    /\ state = "Leaf"
    /\ kind = "change"
    /\ ~proved
    /\ asked = "none"
    /\ proved' = TRUE
    /\ UNCHANGED <<state, ticket, scouts, split, hop, question, review, merge>>

\* R4: no review loop on a plan, spec or contract. A spec gets one scout pass that returns
\* Questions, never PASS or FAIL, and is never repeated; it is optional, by risk. K30: the scout is a
\* native helper. A Wayfinding captain refines the spec and cuts (H4, S14). The Questions it
\* returns are passed up like any other need.
ScoutTheSpec ==
    /\ state = "Leaf"
    /\ kind = "cut"
    /\ scouts = 0
    /\ scouts' = scouts + 1
    /\ UNCHANGED <<state, ticket, proved, split, hop, question, review, merge>>

\* core "Who decides what": a reviewer decides its verdict. R17: the verdict lives on the Review
\* sub-ticket, PASS or FAIL, and the round's holder writes it and the PR's one-line PASS together, at
\* verdict time; R23: the PASS names the head. core "Review severities", R11: FAIL means at least one
\* blocker, and R9 gives each its kind. S8: a round with several seats is held by its officer, which
\* posts one verdict naming each seat, once they are back. The verdict is whole: nothing in it waits on
\* an answer, and a blocker that needs Balazs goes in it as its kind (R9).
WriteTheVerdict ==
    /\ kind = "verdict"
    /\ verdict = "none"
    /\ asked = "none"
    /\ \/ state = "Leaf"
       \/ state = "Waiting" /\ part = "back"
    /\ \/ verdict' = "pass" /\ blockers' = {} /\ pass' = "head"
       \/ verdict' = "fail" /\ blockers' \in (SUBSET Blockers) \ {{}} /\ pass' = "none"
    /\ UNCHANGED <<state, ticket, work, split, hop, question, round, extra, lens, cheap, checkpoint, hunks, target, gates, ci, behind, resyncs, landed>>

\* core: an agent never judges its own work, the verdict is another agent's. A merge needs a PASS on
\* its head, so the captain that merges is reviewed first (R23, R33), after its own proof (R5): it
\* dispatches a fresh reviewer onto the round's Review hop (S3: one Review per round). The first round
\* looks with the full lenses of its level (R6).
SendLeafToReview ==
    /\ state = "Leaf"
    /\ merges
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ verdict' = "due"
    /\ round' = 1
    /\ lens' = "full"
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, blockers, cheap, checkpoint, hunks, merge>>

\* core "A ticket": done when its contract is met. Nothing to merge: a verdict (never reviewed again:
\* core, R34), findings, an answer, a cut, or a part its officer reviews with the whole (R2; R6: a
\* leaf gets no full review, only an optional evidence check beside it, S4). A change is proved
\* first (R5); a verdict is written (R17); a research Question's call is settled (H16, H23), and a
\* his-call Question's by his answer (core "Who decides what").
CloseLeaf ==
    /\ state = "Leaf"
    /\ ~merges
    /\ asked = "none"
    /\ kind = "change" => proved
    /\ kind = "verdict" => verdict \in {"pass", "fail"}
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
    /\ UNCHANGED <<ticket, work, fog, partwaits, hop, question, review, merge>>

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

\* S24: "stopped sets the hop back to Todo", first.
SetTheHopBackToTodo ==
    /\ hop = "stopped"
    /\ hop' = "todo"
    /\ UNCHANGED <<state, ticket, work, split, question, review, merge>>

\* S24: "then resumes the thread or dispatches a fresh agent that attaches a new thread link."
ResumeTheHop ==
    /\ hop = "todo"
    /\ hop' = "live"
    /\ UNCHANGED <<state, ticket, work, split, question, review, merge>>

\* H10: a part returns. So does its fix or its mend (S9, S17). H16, S10: a part that waits on the
\* answer to its need comes back only with it.
PartBack ==
    /\ state = "Waiting"
    /\ part \in {"out", "fixing", "mending"}
    /\ hop = "live"
    /\ tribunal # "exchange"
    /\ ~partwaits
    /\ part' = "back"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* H2: a holder that cannot divide its slice coherently under its own ticket returns it to the
\* officer above for a re-cut.
PartHandedBack ==
    /\ state = "Waiting"
    /\ part = "out"
    /\ hop = "live"
    /\ stage # "council"
    /\ part' = "returned"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* H10: when a part is back and another is ready (S3: opened ahead in Todo, its blocker now done),
\* it is dispatched.
DispatchAReadyPart ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ stage # "council"
    /\ part' = "out"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* R6: "A leaf or lane gets no full review, only an optional fresh evidence-check seat." S4: "A
\* leaf's evidence check is a Review hop beside it." R34: the leaf evidence check reads it locally
\* and runs its receipts.
CheckThePart ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ kind = "change"
    /\ part' = "checking"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* R6, S4: the evidence check finds no blocker and no cheap should, so the part stands.
TheCheckPasses ==
    /\ state = "Waiting"
    /\ part = "checking"
    /\ hop = "live"
    /\ part' = "back"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* S17: a should is "fixed in place before the parent merges if cheap". The part mends it, and its
\* parent merges: who mends is the part's holder, who merges is this captain.
TheCheckFindsACheapShould ==
    /\ state = "Waiting"
    /\ part = "checking"
    /\ hop = "live"
    /\ part' = "mending"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* core "Review severities": a blocker is fixed now. S9: by default the fix goes to the thread that
\* wrote the part, and to a fresh agent when its context is heavy or the approach was wrong.
TheCheckFails ==
    /\ state = "Waiting"
    /\ part = "checking"
    /\ hop = "live"
    /\ part' = "fixing"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* H16: a part passes a need up, and this captain is the receiving officer: it opens a Question as
\* a sub-ticket of its own ticket, at stage research (L18, H23), dispatches a fresh captain onto it
\* and waits on it. S10: it "blocks only what waits on it": the part.
NeedFromAPart ==
    /\ state = "Waiting"
    /\ part = "out"
    /\ hop = "live"
    /\ stage # "council"
    /\ ~partwaits
    /\ partwaits' = TRUE
    /\ UNCHANGED <<state, ticket, work, fog, part, tribunal, hop, question, review, merge>>

\* H16: the Question's captain answers the need, and the part that waits on it goes on (S10).
ThePartIsAnswered ==
    /\ state = "Waiting"
    /\ partwaits
    /\ partwaits' = FALSE
    /\ UNCHANGED <<state, ticket, work, fog, part, tribunal, hop, question, review, merge>>

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
    /\ UNCHANGED <<ticket, work, partwaits, tribunal, hop, question, review, merge>>

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
    /\ UNCHANGED <<ticket, work, partwaits, tribunal, hop, question, review, merge>>

\* H10: the part that was to bring the next into sight is back, and the captain "checks its own
\* contract against the spec": what is left of the route is in sight, or nothing is left and "if the
\* contract is met, it finishes its ticket". Either way no route is left in fog.
TheWayIsFound ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ fog
    /\ fog' = FALSE
    /\ UNCHANGED <<state, ticket, work, part, partwaits, tribunal, hop, question, review, merge>>

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
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* R18, R20: the wiring is in, with its gates green on the merge tree: the whole's receipts (R5).
TheWiringIsIn ==
    /\ state = "Waiting"
    /\ part = "wiring"
    /\ hop = "live"
    /\ part' = "wired"
    /\ proved' = TRUE
    /\ UNCHANGED <<state, ticket, scouts, fog, partwaits, tribunal, hop, question, review, merge>>

\* R2: the whole is reviewed once, after wiring and before its merge, with the lenses of its level
\* (R6), after the author's proof (R5), by a fresh reviewer on the round's Review hop (R33, S3).
SendTheWholeToReview ==
    /\ state = "Waiting"
    /\ part = "wired"
    /\ merges
    /\ proved
    /\ asked = "none"
    /\ state' = "InReview"
    /\ part' = "none"
    /\ verdict' = "due"
    /\ round' = 1
    /\ lens' = "full"
    /\ UNCHANGED <<ticket, work, fog, partwaits, tribunal, hop, question, extra, blockers, cheap, checkpoint, hunks, merge>>

\* H10: the contract is met, so it finishes its ticket. R6: a ticket that only groups its parts needs
\* no review of its own. S8: a review whose seats are back posts the round's one verdict, naming each
\* seat (R17). A research Question's call is settled (H16, H23), and a his-call Question's by his
\* answer (core "Who decides what").
CloseTheWhole ==
    /\ state = "Waiting"
    /\ part = "back"
    /\ ~merges
    /\ ~fog
    /\ asked = "none"
    /\ stage # "council"
    /\ kind = "verdict" => verdict \in {"pass", "fail"}
    /\ call \in {"none", "settled"}
    /\ state' = "Done"
    /\ part' = "none"
    /\ UNCHANGED <<ticket, work, fog, partwaits, tribunal, hop, question, review, merge>>

\* ---- A council (L18, L23) ----

\* L18: the sealed positions are back; "one exchange follows: each answers the other's strongest
\* point or concedes."
TheExchange ==
    /\ state = "Waiting"
    /\ tribunal = "sealed"
    /\ part = "back"
    /\ part' = "out"
    /\ tribunal' = "exchange"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, hop, question, review, merge>>

\* L18: after the exchange the judges agree.
TheJudgesAgree ==
    /\ state = "Waiting"
    /\ tribunal = "exchange"
    /\ part = "out"
    /\ hop = "live"
    /\ part' = "back"
    /\ tribunal' = "agreed"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, hop, question, review, merge>>

\* L18: after the exchange the judges disagree, or the precedent points both ways.
TheJudgesSplit ==
    /\ state = "Waiting"
    /\ tribunal = "exchange"
    /\ part = "out"
    /\ hop = "live"
    /\ part' = "back"
    /\ tribunal' = "split"
    /\ UNCHANGED <<state, ticket, work, fog, partwaits, hop, question, review, merge>>

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
    /\ UNCHANGED <<ticket, work, fog, partwaits, hop, question, review, merge>>

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
    /\ UNCHANGED <<ticket, work, fog, partwaits, hop, question, review, merge>>

\* ---- The review loop (R8, R9, R10, R17, R33, S9, S10, S17) ----

\* The round's Review hop: its holder, the fresh reviewer, writes the verdict; this captain only acts
\* on it. R33: "No agent judges its own step. The Review hop's holder is never the Implement or Fix
\* holder." S8: a lone reviewer is handed its round whole and holds its Review hop. R17: the round's
\* holder writes the verdict on the Review sub-ticket and the PR's one-line PASS together, at verdict
\* time; R23: the PASS names the head. core "Review severities", R11: PASS means no blocker. S17: a
\* should waits as a ticket, and a cheap one is to be fixed in place before the merge; one found earlier
\* stays outstanding. R19, R22: a round after a repair has the repair's hunks in it.
TheReviewPasses ==
    /\ state = "InReview"
    /\ verdict = "due"
    /\ hop = "live"
    /\ verdict' = "pass"
    /\ blockers' = {}
    /\ pass' = "head"
    /\ cheap' \in {cheap, TRUE}
    /\ hunks' = FALSE
    /\ UNCHANGED <<state, ticket, work, split, hop, question, round, extra, lens, checkpoint, target, gates, ci, behind, resyncs, landed>>

\* The same Review hop's holder (R33, S8, R17). core "Review severities", R11: FAIL means at least one
\* blocker; R8: all visible blockers are reported in one pass; R9: each blocker has its kind. S17, R19
\* and R22 as for a PASS.
TheReviewFails ==
    /\ state = "InReview"
    /\ verdict = "due"
    /\ hop = "live"
    /\ verdict' = "fail"
    /\ blockers' \in (SUBSET Blockers) \ {{}}
    /\ cheap' \in {cheap, TRUE}
    /\ hunks' = FALSE
    /\ UNCHANGED <<state, ticket, work, split, hop, question, round, extra, lens, checkpoint, merge>>

\* R8: "the phase ends on the first round with no new blocker", and the officer running the rounds
\* orders no more. S17: a cheap should is fixed in place before the merge, so none is left. S11: each
\* nit gets a ruling. R23: the merge rests on the PR's PASS on the head, which the reviewer wrote. The
\* merge comes next, into the target the brief names (R25, R26, R27).
NoMoreRound ==
    /\ state = "InReview"
    /\ verdict = "pass"
    /\ ~cheap
    /\ state' = "Merging"
    /\ verdict' = "none"
    /\ target' \in IF target = "none" THEN Targets ELSE {target}
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, round, extra, blockers, cheap, checkpoint, hunks, pass, gates, ci, behind, resyncs, landed>>

\* R8: the verdict is PASS, and "the officer may still order one more", once. The next round is
\* fresh, on a fresh Review hop (S3, S9), with the full lenses; the merge waits on its verdict, so the
\* PR's PASS gives way to that round's. A should it found stays outstanding (S17).
OneMoreRound ==
    /\ state = "InReview"
    /\ verdict = "pass"
    /\ ~extra
    /\ verdict' = "due"
    /\ round' = NextRound
    /\ extra' = TRUE
    /\ lens' = "full"
    /\ pass' = "none"
    /\ UNCHANGED <<state, ticket, work, split, hop, question, blockers, cheap, checkpoint, hunks, target, gates, ci, behind, resyncs, landed>>

\* S17: the verdict is PASS and a should "is fixed in place before the parent merges if cheap". By
\* default the thread that wrote the change mends it (S9's default for who fixes); this captain
\* merges. The mend moves the head, so the PASS no longer names it (R23) and its proof is owed (R5).
MendACheapShould ==
    /\ state = "InReview"
    /\ verdict = "pass"
    /\ cheap
    /\ state' = "Mending"
    /\ verdict' = "none"
    /\ proved' = FALSE
    /\ pass' = "none"
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, question, round, extra, blockers, cheap, checkpoint, hunks, target, gates, ci, behind, resyncs, landed>>

\* S17: the cheap should is fixed in place; R5: its author proves the mended head with receipts.
TheMendIsProved ==
    /\ state = "Mending"
    /\ hop = "live"
    /\ ~proved
    /\ proved' = TRUE
    /\ cheap' = FALSE
    /\ UNCHANGED <<state, ticket, scouts, split, hop, question, round, extra, lens, verdict, blockers, checkpoint, hunks, merge>>

\* R23, R24: a PASS stands only on its head, or on an ancestor whose later commits are clean syncs.
\* A mend is neither, so the mended head gets a round of its own, on a fresh Review hop (R33, S3).
ReviewTheMendedHead ==
    /\ state = "Mending"
    /\ hop = "live"
    /\ proved
    /\ state' = "InReview"
    /\ verdict' = "due"
    /\ round' = NextRound
    /\ lens' = "full"
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, blockers, cheap, checkpoint, hunks, merge>>

\* core "Review severities": FAIL means at least one blocker, and a blocker is fixed now. R9: broken
\* or against the spec goes to a Fix, then the next round; a security hole too, with the security
\* lens first next; a need for Balazs goes up at once. S9: at verdict time the blockers become one
\* Fix rN ticket listing all of them; the Fix moves the head, so its proof is owed (R5). S10, H16:
\* the need for Balazs becomes a Question ticket that blocks only what waits on it: this captain,
\* receiving it in the verdict, opens it at stage research (L18) and the Fix waits on its answer.
\* Round 3 has the checkpoint instead (R10).
Fail ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ round # 3
    /\ "fundamental" \notin blockers
    /\ state' = "Fixing"
    /\ verdict' = "none"
    /\ proved' = FALSE
    /\ asked' = IF "him" \in blockers THEN "research" ELSE asked
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, call, round, extra, blockers, cheap, checkpoint, hunks, merge>>

\* R9: a blocker that is fundamental or a new requirement goes to the officer above to re-plan
\* (H2, H5). S10: dead hops go Canceled with a one-line reason. A need for Balazs in the same
\* verdict still goes up at once (R9) and still becomes a Question ticket that blocks only what waits
\* on it (S10): this captain opens it at stage research (H16, L18), and the re-plan waits on it.
Replan ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ round # 3
    /\ "fundamental" \in blockers
    /\ state' = "HandedBack"
    /\ verdict' = "none"
    /\ asked' = IF "him" \in blockers THEN "research" ELSE asked
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, call, round, extra, blockers, cheap, checkpoint, hunks, merge>>

\* R10: "After round 3 comes a checkpoint, not a stop: by default one more round." "At round 3, the
\* officer running the rounds decides on purpose." A third round still has a blocker, and every one
\* is fixable in a Fix (R9), so the decision is one more round. S10: "checkpoint: one more round,
\* because ...". The blockers go to a Fix (S9), and its round looks as R8 and R9 say.
CheckpointOneMoreRound ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ round = 3
    /\ blockers \cap {"fundamental", "him"} = {}
    /\ state' = "Fixing"
    /\ verdict' = "none"
    /\ proved' = FALSE
    /\ checkpoint' = "one more round"
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, question, round, extra, blockers, cheap, hunks, merge>>

\* R10: escalating "for something that needs him". S10: "escalated: ...", and the need becomes a
\* Question ticket that blocks only what waits on it (H16, L18): the Fix waits on its answer, then the
\* next round.
CheckpointAsksHim ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ round = 3
    /\ "him" \in blockers
    /\ "fundamental" \notin blockers
    /\ state' = "Fixing"
    /\ verdict' = "none"
    /\ proved' = FALSE
    /\ asked' = "research"
    /\ checkpoint' = "escalated"
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, call, round, extra, blockers, cheap, hunks, merge>>

\* R10: escalating "for a fundamental break, a new requirement". R9: the officer above re-plans.
\* S10: "escalated: ...". A need for Balazs in the same verdict becomes its Question, as in Replan.
CheckpointEscalates ==
    /\ state = "InReview"
    /\ verdict = "fail"
    /\ round = 3
    /\ "fundamental" \in blockers
    /\ state' = "HandedBack"
    /\ verdict' = "none"
    /\ asked' = IF "him" \in blockers THEN "research" ELSE asked
    /\ checkpoint' = "escalated"
    /\ lens' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, call, round, extra, blockers, cheap, hunks, merge>>

\* S9: "Fix rN is Done when its blockers are Done and proof ran on the head." The Fix's holder fixes
\* every blocker, and proves the fixed head with receipts (R5). A blocker that waits on a Question
\* is Done only once it is answered (S10, H16).
TheFixIsProved ==
    /\ state = "Fixing"
    /\ hop = "live"
    /\ asked = "none"
    /\ ~proved
    /\ proved' = TRUE
    /\ UNCHANGED <<state, ticket, scouts, split, hop, question, review, merge>>

\* R8: a fix between every two rounds, and "a round with a blocker is always followed by a
\* full-lens round"; R9: after a security hole the security lens comes first. S9: the Fix is Done,
\* and "the next round is a fresh look either way": a fresh reviewer on a new Review hop (R33, S3).
ReviewAgain ==
    /\ state = "Fixing"
    /\ hop = "live"
    /\ proved
    /\ state' = "InReview"
    /\ verdict' = "due"
    /\ round' = NextRound
    /\ lens' = IF "security" \in blockers THEN "security first" ELSE "full"
    /\ blockers' = {}
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, cheap, checkpoint, hunks, merge>>

\* The Fix's holder hands it back undone, and this captain passes it on. H2
HandBackTheFix ==
    /\ state = "Fixing"
    /\ hop = "live"
    /\ state' = "HandedBack"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* ---- The merge (R18 to R31) ----

\* R21: "The captain runs dydo merge and dydo test gate itself." R23: dydo merge refuses unless the
\* merge with the tip is clean and DYTEST holds a green receipt keyed by the combined tree. Its result
\* is a green receipt, a real red gate, or a CONFLICT (R19). How a red is told real before the merge
\* is not said (R21 names only "a real red gate"); the model records a red as real.
RunTheGates ==
    /\ state = "Merging"
    /\ gates = "none"
    /\ gates' \in {"green", "red", "conflict"}
    /\ UNCHANGED <<state, ticket, work, split, hop, question, review, target, pass, ci, behind, resyncs, landed>>

\* R26: a PR into master merges when CI is green; R31: the captain watches its own CI.
WatchTheCi ==
    /\ state = "Merging"
    /\ target \in Master
    /\ ci = "none"
    /\ ci' \in {"green", "red"}
    /\ UNCHANGED <<state, ticket, work, split, hop, question, review, target, pass, gates, behind, resyncs, landed>>

\* R21: the captain "sends an implementer only for a CONFLICT or a real red gate". The red is on
\* record. The repair is a Fix on the ticket branch; nothing has landed, so nothing is reverted (R22
\* speaks of a red gate after a merge). The repair moves the head, so the PASS no longer names it
\* (R23) and its proof is owed (R5).
RedBeforeLanding ==
    /\ state = "Merging"
    /\ gates = "red" \/ ci = "red"
    /\ state' = "Repairing"
    /\ proved' = FALSE
    /\ pass' = "none"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, question, review, target, behind, resyncs, landed>>

\* R19: "A conflict is resolved only on the ticket branch, under a fresh review of its hunks"; R21:
\* by an implementer the captain sends, for the CONFLICT on record. R23, R5 as above.
MergeConflict ==
    /\ state = "Merging"
    /\ gates = "conflict"
    /\ state' = "Repairing"
    /\ proved' = FALSE
    /\ pass' = "none"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, question, review, target, behind, resyncs, landed>>

\* R18, R25: into a feature branch freely. R23: dydo merge lands it only when the PASS names the head
\* or an ancestor as R24 allows, the merge with the tip is clean and DYTEST holds a green receipt.
\* R22: a re-land after a revert is dydo merge reverting the revert. R20: the Merge hop's own checks
\* then run on the merge tree and the merge SHA.
MergeIntoFeature ==
    /\ state = "Merging"
    /\ target = "feature"
    /\ PassStands
    /\ gates = "green"
    /\ state' = "Landed"
    /\ landed' = "in"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, behind, resyncs>>

\* R26: a two-way PR into master merges AFK when its evidence is complete: a PASS on the head or an
\* ancestor, gates green on the combined tree with current master, CI green, a PR body that declares
\* it two-way. R29: main-bound merges are paced, so it waits on the coordinator's turn.
OfferForTheTurn ==
    /\ state = "Merging"
    /\ target = "two-way"
    /\ PassStands
    /\ gates = "green"
    /\ ci = "green"
    /\ state' = "AwaitingTurn"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* core "Who decides what": Balazs decides anything one-way. R27: a one-way PR needs his click and
\* carries a recovery plan. R31: "Before his click, the captain watches its own CI and opens the HITL
\* Merge only when it is green." S12: a HITL Merge only for a one-way one.
OpenTheHitlMerge ==
    /\ state = "Merging"
    /\ target = "one-way"
    /\ PassStands
    /\ gates = "green"
    /\ ci = "green"
    /\ state' = "AwaitingClick"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, merge>>

\* R29: "the coordinator lands them one at a time", so master moves while a PR into it waits, on its
\* turn or on his click.
MasterMoves ==
    /\ state \in {"AwaitingTurn", "AwaitingClick"}
    /\ ~behind
    /\ behind' = TRUE
    /\ UNCHANGED <<state, ticket, work, split, hop, question, review, target, pass, gates, ci, resyncs, landed>>

\* R29: the coordinator gives the turn, the PR "re-checks against current master just before
\* landing" and finds it has not moved, and the captain merges it (R26).
LandOnTheTurn ==
    /\ state = "AwaitingTurn"
    /\ ~behind
    /\ state' = "Landed"
    /\ landed' = "in"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, behind, resyncs>>

\* R27, R31: his click lands it, and "the survey sees 'PR merged, HITL Merge open' and wakes the
\* captain". R29: each main-bound PR re-checks against current master just before landing, so it is
\* current with master when he clicks.
HisClick ==
    /\ state = "AwaitingClick"
    /\ ~behind
    /\ state' = "Landed"
    /\ landed' = "in"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, behind, resyncs>>

\* R29: the re-check finds master moved, and the PR re-syncs. R24: a clean sync whose second parent is
\* in the target keeps the PASS, now on an ancestor; the gates and CI run again on the new tree. "A PR
\* forced to re-sync a third time is held while the coordinator sequences the colliding work."
ResyncOnTheRecheck ==
    /\ state \in {"AwaitingTurn", "AwaitingClick"}
    /\ behind
    /\ resyncs' = resyncs + 1
    /\ state' = IF resyncs' = 3 THEN "Held" ELSE "Merging"
    /\ pass' = "ancestor"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ behind' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, landed>>

\* R19, R29: the re-sync with current master conflicts; dydo merge refuses it, and the CONFLICT is on
\* record for the merge.
ConflictOnTheRecheck ==
    /\ state \in {"AwaitingTurn", "AwaitingClick"}
    /\ behind
    /\ state' = "Merging"
    /\ gates' = "conflict"
    /\ ci' = "none"
    /\ behind' = FALSE
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, resyncs, landed>>

\* R29: the coordinator has sequenced the colliding work; the PR runs its evidence again. The count of
\* forced re-syncs starts again; the spec does not say how a held PR counts on.
Sequenced ==
    /\ state = "Held"
    /\ state' = "Merging"
    /\ resyncs' = 0
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, gates, ci, behind, landed>>

\* R20: the Merge hop is Done when DYTEST holds a green receipt on the merge tree and hosted CI is
\* green on the merge SHA; R18: on a feature branch, CI on the feature/** push. core: the ticket is
\* done when its contract is met.
GreenOnTheMergeSha ==
    /\ state = "Landed"
    /\ state' = "Done"
    /\ gates' = "green"
    /\ ci' = "green"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, behind, resyncs, landed>>

\* R22: "A red gate after a merge: DYTEST reruns once."
GateRed ==
    /\ state = "Landed"
    /\ state' = "Rerunning"
    /\ gates' = "red"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, ci, behind, resyncs, landed>>

\* R22: "A flake means the hop is Done and the flake is counted as a DYTEST result": the rerun is
\* green, and so is CI on the merge SHA (R20).
Flake ==
    /\ state = "Rerunning"
    /\ state' = "Done"
    /\ gates' = "green"
    /\ ci' = "green"
    /\ UNCHANGED <<ticket, work, split, hop, question, review, target, pass, behind, resyncs, landed>>

\* R22: "A real red means the captain reverts at once [...], a Fix follows on the ticket branch".
\* S12: the reverted Merge goes Canceled with "reverted: <gate> red on <merge SHA>". R23, R5 as for
\* a repair before the landing.
RealRed ==
    /\ state = "Rerunning"
    /\ state' = "Repairing"
    /\ landed' = "reverted"
    /\ proved' = FALSE
    /\ pass' = "none"
    /\ gates' = "none"
    /\ ci' = "none"
    /\ UNCHANGED <<ticket, scouts, split, hop, question, review, target, behind, resyncs>>

\* R19, R21, R22: the implementer resolves the conflict, or fixes the real red, on the ticket branch,
\* and proves the repaired head (R5). Its hunks wait on a fresh review (R19).
TheRepairIsProved ==
    /\ state = "Repairing"
    /\ hop = "live"
    /\ ~proved
    /\ proved' = TRUE
    /\ hunks' = TRUE
    /\ UNCHANGED <<state, ticket, scouts, split, hop, question, round, extra, lens, verdict, blockers, cheap, checkpoint, merge>>

\* R19: "under a fresh review of its hunks". R22: "then a fix-scoped round, then a re-land". A fresh
\* reviewer on a new Review hop (R33, S3).
RepairReviewed ==
    /\ state = "Repairing"
    /\ hop = "live"
    /\ proved
    /\ state' = "InReview"
    /\ verdict' = "due"
    /\ round' = NextRound
    /\ lens' = "scoped"
    /\ UNCHANGED <<ticket, work, split, hop, question, extra, blockers, cheap, checkpoint, hunks, merge>>

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
    \/ HeDecides
    \/ ProveTheWork
    \/ ScoutTheSpec
    \/ WriteTheVerdict
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
    \/ ThePartIsAnswered
    \/ CutTheNextParts
    \/ FogRemains
    \/ TheWayIsFound
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
    \/ TheReviewPasses
    \/ TheReviewFails
    \/ NoMoreRound
    \/ OneMoreRound
    \/ MendACheapShould
    \/ TheMendIsProved
    \/ ReviewTheMendedHead
    \/ Fail
    \/ Replan
    \/ CheckpointOneMoreRound
    \/ CheckpointAsksHim
    \/ CheckpointEscalates
    \/ TheFixIsProved
    \/ ReviewAgain
    \/ HandBackTheFix
    \/ RunTheGates
    \/ WatchTheCi
    \/ RedBeforeLanding
    \/ MergeConflict
    \/ MergeIntoFeature
    \/ OfferForTheTurn
    \/ OpenTheHitlMerge
    \/ MasterMoves
    \/ LandOnTheTurn
    \/ HisClick
    \/ ResyncOnTheRecheck
    \/ ConflictOnTheRecheck
    \/ Sequenced
    \/ GreenOnTheMergeSha
    \/ GateRed
    \/ Flake
    \/ RealRed
    \/ TheRepairIsProved
    \/ RepairReviewed
    \/ RepairFails
    \/ Ended

Spec == Init /\ [][Next]_vars
====

---- MODULE CaptainBase ----
\* The shared frame of the captain model: the variables every part of the process shares, and the
\* event record. Written by hand; no build step writes this file. Captain.tla extends it with the
\* actions, and the properties in verification/statechart/captain/ extend Captain.
\*
\* The register beside this file (register.md) is the closed list of rules the model is held to. A
\* variable here serves the rows it names; a part of the model that needs a variable the frame lacks
\* adds it in its own section below, with the rows it serves.
\*
\* The event record. Every step sets `event` to the set of events the step is: each event's name, and
\* who takes it. A property reads an event from this record, never from the state a step lands in.
\* The view check reads it too: each chart arrow's condition names the events its words claim.
EXTENDS Naturals

\* state     where the captain is with its ticket: one of the chart's states (FR1-FR23).
\* event     the events of the step that led here (FR1-FR29).
\* ticket    the ticket as the captain finds it on dispatch: free, held by another agent, or closed (IQ1).
\* kind      the ticket's result: a change, a verdict (a review), findings (one seat of a review), an
\*           answer (a Question), or a cut (a Wayfinding ticket); "unread" until it is taken (IQ2).
\* merges    its brief says this captain merges its change itself (IQ2, RM1).
\* proved    the author's receipts are in for the head as it stands (IQ8, RM2).
\* written   a ticket whose result is findings or a cut has written it (FR24).
\* call      an answer's call: open until its captain settles it (IQ14); "none" for other kinds.
\* asked     the call the captain's own work meets that it may not decide: none; needed, met and not
\*           yet passed up; or open, a Question it waits on (IQ9, IQ11).
\* part      the sub-tickets a captain that splits waits on, as one abstract part: out, back, or wired
\*           into the parent; "none" when it has none (SP3, SP7, SP16).
\* contract  what the captain found when it checked its contract against the spec once a part was back:
\*           unchecked, not met yet, met, or cannot be met inside this ticket (SP7, SP8, SP10).
\* verdict   the round's verdict: due while a fresh reviewer holds the round's Review hop, then pass or
\*           fail as it was written, until the captain acts on it; for a ticket whose result is a
\*           verdict, the verdict its captain wrote (FR26, RM3).
\* blockers  the kinds of blocker the verdict holds, kept through the Fix it sends (FR27, RM6).
\* gates     the gates on the combined tree: not run, green, a real red, or a CONFLICT (RM14, RM15).
\* landed    the merge is not in yet, or in (RM17).
\* ci        hosted CI on the merge SHA (RM17).
VARIABLES state, event, ticket, kind, merges, proved, written, call, asked, part, contract, verdict, blockers, gates, landed, ci

vars == <<state, event, ticket, kind, merges, proved, written, call, asked, part, contract, verdict, blockers, gates, landed, ci>>

\* The chart's states. A part that adds a model state adds it here and gives it a chart state in
\* view-map.mjs.
States == {"Dispatched", "Taken", "Leaf", "Asking", "Cutting", "Waiting",
           "InReview", "Fixing", "Merging", "Done", "HandedBack"}

\* The two ends: the contract is met, or the ticket went back to the officer unfinished.
Ends == {"Done", "HandedBack"}

Kinds == {"change", "verdict", "findings", "answer", "cut"}

\* R9's kinds of blocker: broken or against the spec, a security hole, fundamental or a new
\* requirement, and a need for Balazs (RM6).
Blockers == {"broken", "security", "fundamental", "him"}

\* Who takes a step: this ticket's captain; its officer, the agent above it; a part's holder; the
\* fresh reviewer on the round's Review hop; the Fix's holder; the captain of the Question this work
\* waits on; Balazs; and the tools (DYTEST and hosted CI).
Actors == {"captain", "officer", "part", "reviewer", "fixer", "question", "him", "tools"}

\* The events the frame's steps name. A part that adds a step adds its events here.
EventNames == {
    "dispatched",                                        \* Init: the officer dispatches the captain
    "set In Progress", "cannot take",                    \* IQ1, IQ2
    "fits one agent", "needs more than one agent",       \* IQ6
    "proves too big",                                    \* IQ7
    "prove the work", "write the verdict",               \* IQ8, FR26
    "answer the call", "write the result",               \* IQ14, FR24
    "meet a call it may not decide",                     \* IQ9
    "pass the need up", "open a Question", "answer",     \* IQ9, IQ11
    "cannot be done inside the ticket",                  \* IQ26
    "cannot be cut inside the ticket",                   \* SP10
    "a part shows the contract cannot be met",           \* SP10
    "hand back", "close",                                \* FR24, FR25
    "dispatch the parts", "a part returns",              \* SP3, SP7
    "check the contract against the spec",               \* SP7
    "cut the next parts", "wire the parts",              \* SP6, SP16
    "dispatch a reviewer",                               \* RM1
    "act on PASS", "act on FAIL", "open a Fix",          \* RM6, RM8
    "prove the Fix", "the Fix comes back undone",        \* RM9, RM26
    "run the gates", "merge", "green on the merge SHA"   \* RM14, RM15, RM17
}

Events == [name: EventNames, by: Actors]

\* One event of a step, by name and actor.
Ev(n, a) == [name |-> n, by |-> a]

\* The result of a ticket that does not merge is in: a change proved, a verdict written, an answer's
\* call settled, findings or a cut written (FR24).
ResultIn ==
    CASE kind = "change" -> proved
      [] kind = "verdict" -> verdict \in {"pass", "fail"}
      [] kind = "answer" -> call = "settled"
      [] OTHER -> written

TypeOK ==
    /\ state \in States
    /\ event \subseteq Events /\ event # {}
    /\ ticket \in {"free", "held", "closed"}
    /\ kind \in Kinds \cup {"unread"}
    /\ merges \in BOOLEAN
    /\ proved \in BOOLEAN
    /\ written \in BOOLEAN
    /\ call \in {"none", "open", "settled"}
    /\ asked \in {"none", "needed", "open"}
    /\ part \in {"none", "out", "back", "wired"}
    /\ contract \in {"unchecked", "not met", "met", "cannot be met"}
    /\ verdict \in {"none", "due", "pass", "fail"}
    /\ blockers \subseteq Blockers
    /\ gates \in {"none", "green", "red", "conflict"}
    /\ landed \in {"no", "in"}
    /\ ci \in {"none", "green", "red"}
    /\ merges => kind = "change"
    /\ (call # "none") => kind = "answer"

\* The officer dispatches a captain onto a ticket (S6, K30). The ticket may be free, held by another
\* agent, or closed (IQ1).
Init ==
    /\ state = "Dispatched"
    /\ event = {Ev("dispatched", "officer")}
    /\ ticket \in {"free", "held", "closed"}
    /\ kind = "unread"
    /\ merges = FALSE
    /\ proved = FALSE
    /\ written = FALSE
    /\ call = "none"
    /\ asked = "none"
    /\ part = "none"
    /\ contract = "unchecked"
    /\ verdict = "none"
    /\ blockers = {}
    /\ gates = "none"
    /\ landed = "no"
    /\ ci = "none"
====

---- MODULE CaptainProperties ----
EXTENDS Captain

\* Hand-written from the register's rows (statechart/captain/register.md), never from the model: a
\* property says what its row requires, and when the model breaks one, the model is wrong. Each
\* property names its row. These are the frame's properties (part 1, DYD-1504); the parts that build
\* the phases add the properties their rows name.
\*
\* A property reads an event from the event record, `event`: the set of events the step is, each
\* with its name and who takes it. It never infers an event from the state a step lands in.
\*
\* seen is passive: the states this captain has been in. Every model step has exactly one observer
\* extension, no step is disabled and no fairness is added. run.py replaces PropertyInit and
\* PropertyNext for its deliberate wrong step.
VARIABLE seen
PropertyInit == Init /\ seen = {state}
PropertyNext == Next /\ seen' = seen \cup {state'}

\* The step being checked has the event named n, taken by any actor, or by actor a.
Happens(n) == \E x \in event' : x.name = n
HappensBy(n, a) == Ev(n, a) \in event'
\* The step before it ended the ticket: a close or a hand-back.
Ended_ == \E x \in event : x.name \in {"close", "hand back"}

\* FR24 (core "A ticket", K34): "it is done when its contract is met". A close comes only from the
\* captain, with nothing waiting on an answer, and with the contract met: a captain that merges has
\* its merge in, with its gates and CI green on the merge SHA (R20); any other has its result in (a
\* change proved, or the parts back for a ticket that only groups them, R6; a verdict written; an
\* answer's call settled; findings or a cut written).
FR24_DoneWhenTheContractIsMet ==
    [][Happens("close") =>
        /\ HappensBy("close", "captain")
        /\ asked = "none"
        /\ IF merges
           THEN landed' = "in" /\ gates' = "green" /\ ci' = "green"
           ELSE CASE kind = "change" -> proved \/ part = "back"
                  [] kind = "verdict" -> verdict \in {"pass", "fail"}
                  [] kind = "answer" -> call = "settled"
                  [] OTHER -> written
    ]_<<vars, seen>>

\* FR25 (AGENTS.md "Sort yourself", H1): crew does its one job and returns; what goes up goes to the
\* officer above. After a close or a hand-back nothing more happens.
FR25_EndsAreFinal ==
    [][Ended_ => UNCHANGED vars]_<<vars, seen>>

\* FR26 (core "Who decides what", R33, K29): an agent decides what its ticket makes its job to
\* decide, and never judges its own work. A verdict on a change is written by the fresh reviewer on
\* the round's Review hop, never by the captain or the Fix's holder; a ticket whose result is a
\* verdict writes its own; a Question's captain settles its own call, and only for an answer.
FR26_EachDecidesItsJob ==
    [][ /\ Happens("write the verdict") =>
            IF kind = "verdict" THEN HappensBy("write the verdict", "captain")
            ELSE kind = "change" /\ HappensBy("write the verdict", "reviewer")
        /\ Happens("answer the call") => kind = "answer" /\ HappensBy("answer the call", "captain")
    ]_<<vars, seen>>

\* FR27 (core "Review severities", R11): FAIL means at least one blocker, and PASS means none.
FR27_FailHasABlocker ==
    [][Happens("write the verdict") =>
        \/ verdict' = "fail" /\ blockers' # {}
        \/ verdict' = "pass" /\ blockers' = {}
    ]_<<vars, seen>>

\* FR28 (core "Review severities", R34): "A verdict is not reviewed again." A reviewer is dispatched
\* only onto a change; never onto a verdict, a seat's findings, an answer or a cut.
FR28_AVerdictIsNotReviewedAgain ==
    [][Happens("dispatch a reviewer") => kind = "change"]_<<vars, seen>>
====

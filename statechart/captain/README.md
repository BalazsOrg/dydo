# The captain statechart

The statechart is the digital twin of the process the role skills describe. It is written by hand, every skill file is checked against it, and it shows who sees what (`dydo/core.md`, "The statechart"). This folder is that twin for one captain and its ticket. No build step writes anything here.

The twin has two levels, as Balazs set them at the DYD-1440 walkthrough: "One thing is the observability of the state chart and that should be simple. [...] But there is also the comprehensive version, which should indeed include all the states and all the logic. So it actually serves as a model, a digital twin, so that we can check if a behavior or the process is faulty by design."

- **The comprehensive model**, `Captain.tla`, holds every state and every rule of the process that has an order to check: 18 states, 85 actions and 30 variables, each action with the rule it rests on. A skill line is checked against the model.
- **The simple view**, `chart.mmd`, is the chart he approved for reading: 11 states and 23 arrows. It leaves nothing out of the process; it shows each model state as one of its states, and each model step under one of its arrows or inside one of its states.

The truth both are checked against is, in order: his recorded words, then `Spec: dydo 4.0.0`, then the decision records, with the core page while it lives.

## Files

| File | What it is |
|---|---|
| [Captain.tla](Captain.tla), `Captain.cfg` | The comprehensive model, for the TLC model checker. Each action is a case of the rules its comment names. |
| [chart.mmd](chart.mmd) | The simple view. One arrow is one case in plain words; the comment above an arrow names what it rests on. |
| [view-map.mjs](view-map.mjs) | How the two relate: the chart state that shows each model state, the chart arrows and chart states each model action's steps belong to, and the model cases no chart words are true of. |
| [positions.md](positions.md) | Who sees what: the skills and context each position gets, and how to check a skill file against the model. |
| `check-chart.mjs`, `chart-words.mjs` | The view check, and its strict reader of `chart.mmd`. |
| `view.mjs` | How the statechart tool would draw the model as the simple view. It holds no words: it reads them from `chart.mmd`. |

The properties the model must keep are hand-written too, in [verification/statechart/captain/](../../verification/statechart/captain/obligations.md), apart from the model they judge.

## What the model holds

Beyond the states, the model keeps a record of each event a rule decides on, so that a property can observe the event itself and never infer it from where a step lands:

- the ticket's result and, for a Question, its stage (H23); a his-call Question's call, which only his answer settles (core "Who decides what");
- the author's proof of the head as it stands, owed again whenever a Fix, a mend or a repair changes it (R5, S9); the count of scout passes on a cut's spec (R4);
- the route in fog (H10), and one abstract part of a ticket that splits, through its dispatch, its return or hand-back, its evidence check, fix or mend, and its wiring (H10, H2, R6, S4, S17, R2); whether that part waits on a Question its captain opened for it (H16, S10);
- whether the hop it waits on was found stopped and set back to Todo (S24);
- the Question its own work waits on, from the need passed up to the answer (H16); a research Question's call, a council's steps and its case file, and what a council child did with the call (L18, L20, L21);
- the round: its number, the one more round, the lens, and the round's Review hop: due while a fresh reviewer holds it, then the verdict its holder wrote, with the blockers by kind (R8, R9, R17, R33, S8); a cheap should outstanding, the checkpoint line (S10, S17), and a repair's hunks no verdict has covered yet (R19, R22);
- the merge: its target, the PR's PASS on the head or an ancestor, the gates' result (green, a real red, or a CONFLICT), CI's result, whether master has moved past the PR, its re-syncs, and whether it has landed or was reverted (R20 to R31).

The chart shows none of these values.

## How the view shows the model

Seven model states are shown inside a chart state whose words are true of them, so the chart draws no step between them:

| Model state | Shown as | What it is | Rests on |
|---|---|---|---|
| `Repairing` | `Fixing` | A CONFLICT or a real red gate before the merge, or a real red after it, is repaired by a Fix on the ticket branch, proved, then a round scoped to its hunks. | R19, R21, R22 |
| `AwaitingTurn` | `Merging` | A two-way PR into master, its evidence complete, waits on the coordinator's turn. | R26, R29 |
| `Held` | `Merging` | A PR forced to re-sync a third time, held while the coordinator sequences the colliding work. | R29 |
| `AwaitingClick` | `Merging` | A one-way PR, its CI green, waits on his click. | core "Who decides what"; R27, R31 |
| `Landed` | `Merging` | The merge is in; the Merge hop waits on green on the merge SHA. | R20 |
| `Rerunning` | `Merging` | A red gate after the merge: DYTEST reruns once. | R22 |
| `Mending` | `InReview` | A cheap should is fixed in place after a PASS, before the merge; the mended head is proved, then gets a round. **Not true of it**: see below. | S17, R23, R24 |

Two cases have no state of their own:

- **Finding the way is a cut.** A captain whose route is not all in sight cuts only the parts in sight, or one part that brings the next into sight (H10); the model marks that with `fog`, inside `Cutting`. When that part is back, the captain checks its contract against the spec: the fog lifts inside `Waiting`, and it cuts the parts now in sight or finishes its ticket.
- **A need passed up waits inside `Leaf`** until its officer opens the Question; the captain then waits in `Asking`, on a Question that is open.

Three cases have no chart words true of them, and the chart is his approved view, so its words are not bent: `view-map.mjs` lists them, `check-chart.mjs` prints them on every run, and they go to him as a view change.

- `Mending`, with its steps `MendACheapShould`, `TheMendIsProved` and `ReviewTheMendedHead`, sits under `InReview`, "Waiting on a reviewer's verdict". After a PASS with a cheap should, the captain waits on the should's fix in place before the merge (S17), not on a verdict. The chart has no state for that wait, and no arrow out of the review for it.
- `Waiting`, when all a captain that splits waits on is its own need, passed up and opened by its officer, sits under "Waiting on its sub-tickets". The officer opened that Question under its own ticket (H16), so it is not this captain's sub-ticket. H10 has the captain wait on it, and after a cut it never goes back to `Asking`.
- `InReview`, once the round's verdict is written and before the captain acts on it, sits under "Waiting on a reviewer's verdict". The reviewer on the round's Review hop writes the verdict, and the PR's PASS line with a PASS (R17); the captain only acts on it (R33, R8). Between the two steps the verdict is in. This case is new at DYD-1492 r2: the verdict was one step with the captain's act on it, and so written by the merging captain.

## How the view is checked

`check-chart.mjs` takes no arguments. It runs TLC on `Captain.tla` and `Captain.cfg` as they stand beside it, in a new scratch folder, and reads the state graph TLC dumps (`-dump dot,actionlabels`): every reachable state and every step, each named by its action. It reads no graph from anywhere else, and no drawing, so the model needs no bound to be checked, and a step that stays inside one chart state is checked as well as one between two. It fails on:

- any change to the chart's visible words: the SHA-256 of every line Mermaid renders must be the one pinned in the checker, the chart he approved at DYD-1440 as committed at `8077c474`; a comment may change;
- a line of `chart.mmd` its strict reader cannot place, an arrow without words, two arrows between the same two states, or an arrow that joins two cases with "or";
- a `Captain.cfg` that says anything but `INIT Init`, `NEXT Next` and `INVARIANT TypeOK`, so nothing is pruned and the deadlock check runs;
- a model state the map does not show, an action it gives no place, or a place the chart lacks;
- TLC stopping with an error, so `TypeOK` and `NoDeadlock` hold on every reachable state when it passes;
- a graph that is not this model's whole graph: a state count other than TLC's distinct states, a step count other than the states TLC generated after the initial ones, or a state whose label does not give each variable of `Captain.tla` once;
- a step named `Ended`, the stand-still, that moves or stands still anywhere but at an end, and any other step that changes nothing;
- a reachable step whose place, the chart state it stays inside or the chart arrow between its two ends' chart states, is not one the map gives its action;
- a place the map gives an action that no step of it takes, a chart arrow no step stands under, a chart state no reachable model state is shown as, and a model state or action never reached.

The statechart tool draws a chart only up to 500 model states, and the model has more than 43,000, so it draws no chart; it still checks `TypeOK` and `NoDeadlock` and every property. `view.mjs` stays for it.

## Change it

Change the process in `Captain.tla` first, with the rule each action rests on, and give each new state and action its places in `view-map.mjs`. When a rule decides on an event, keep a record of the event in a variable, so a property can observe it. A model step that crosses between two chart states must stand under a chart arrow whose words are true of it, and a model state must be shown as a chart state whose words are true of it. When no chart words fit, do not bend them: list the case in `mismatches` in `view-map.mjs`, and bring it to him with the case, because the view is his to approve. A change to the chart's words needs his approval, and with it a new pin in `check-chart.mjs`. Then check it.

## Check it

Use an absolute path for this checkout in place of `REPO`, and a new scratch folder each time. Commit first: the property runner refuses a dirty checkout.

```text
dydo statechart install-skill --root /tmp/chartroot
/var/tmp/dydo-gate-lock.sh dydo statechart REPO/statechart/captain/Captain.tla --skill /tmp/chartroot/skills/engineering/statechart --config REPO/statechart/captain/Captain.cfg --view REPO/statechart/captain/view.mjs --out /tmp/captain-drawn
/var/tmp/dydo-gate-lock.sh node REPO/statechart/captain/check-chart.mjs
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-properties
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --controls --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-controls
```

The first command checks `TypeOK` and `NoDeadlock` with the statechart tool, and installs the Java and TLC the view check uses. The second runs TLC itself and fails on any difference between the view and the model, as above; set `JAVA` and `TLA2TOOLS` to use other copies. The third checks the properties. The fourth runs a deliberately wrong step and passes when the properties catch it. Never commit what the commands write.

## Where the model settles between its sources

- **A Question's stages.** At DYD-1454 an agent ruled that the stages draw nothing, under the reading that the simple chart was the whole process. He has since said it is not. The core page says a Question's captain decides its answer and says nothing of how; the spec says how (H16, H23, L18). So the model has them, as the held ticket's stage: a research Question's captain researches first, then decides, or opens a council child for a hard call not worth his time, or a his-call child for a call worth it, and answers from the child. A council's chair runs the sealed positions, the one exchange and the exits. Only a research Question opens a child. A council child that prepared the call sends it on to him (L20).
- **A his-call Question.** The core page says Balazs decides every call that is his to make; H23 makes a his-call Question the envoy ticket itself. So its call is his from the take, his answer settles it as a step of its own, and the ticket closes only on it.
- **Who opens a Question.** H16: "the receiving officer opens it [...] and waits on it". The core page does not name the opener. The model follows H16: a captain passes its need up and goes on with what does not wait on it (S10); its officer opens the Question. A captain opens one only as the receiving officer: a splitter for a part's need, which that part then waits on and comes back only with its answer; a merger for a reviewer's blocker that needs Balazs, which R9 sends "up the chain at once" and S10 makes "a Question ticket that blocks only what waits on it". The Fix waits on it, or, when the same verdict holds a fundamental blocker, the re-plan the ticket goes back for. The model holds one Question at a time for the captain's own work, and one for its part; more are the same case again.
- **The verdict.** R17 has the round's holder write the verdict on the Review sub-ticket and the PR's one-line PASS together, at verdict time; R33 says the Review hop's holder is never the Implement or Fix holder. So each round dispatches a fresh reviewer onto its Review hop, the reviewer writes the verdict as a step of its own that moves no work, and the merging captain only acts on it. A captain whose ticket is a verdict closes only with its verdict written, once its seats are back (S8).
- **The checkpoint.** R10 lets the officer at round 3 escalate "only for a fundamental break, a new requirement, or something that needs him", and R9 sends a fundamental blocker to a re-plan whatever the round. So the act on a round-3 FAIL is the checkpoint, decided by the blockers' kinds: one more round for blockers a Fix can fix, keeping the security lens first after a security hole; escalated to a re-plan for a fundamental one; escalated as a Question for a need for him, with the Fix waiting on it.
- **The proof of a changed head.** S9: "Fix rN is Done when its blockers are Done and proof ran on the head." R5 has the author prove with receipts before a fresh reviewer judges. A mend and a repair change the head too, so each round after a Fix, a mend or a repair starts only on its proof.
- **A cheap should.** S17 says a should is "fixed in place before the parent merges if cheap". The model carries a cheap should from the verdict that found it to its mend, and no merge starts with one outstanding. A part's cheap should, found by its evidence check, is mended by the part before its parent wires it; a merger's is mended, by default, by the thread that wrote the change, and the mended head gets a round (R23).
- **A red gate before the merge.** R22 speaks of "a red gate after a merge": rerun once, revert at once if real. R21 has the captain send an implementer "for a CONFLICT or a real red gate". Before the merge nothing has landed, so nothing is reverted: a CONFLICT or a red on record goes to a repair. How a red before the merge is told real from a flake is not said; the model records the gates' red as real.
- **The re-check before landing.** R29: "Each re-checks against current master just before landing". The model lets master move while a PR into it waits, on its turn or on his click, and lands a PR only while master has not moved past it; otherwise the re-check re-syncs it, or finds the CONFLICT. R29 holds a PR forced to re-sync a third time; the model starts the count again once the coordinator has sequenced the colliding work, since the spec does not say how a held PR counts on.
- **A captain that splits and authors.** R16 lets a captain that splits author a change too small to need its own review. The core page and `AGENTS.md` say a captain that splits only orchestrates. The model follows the core page: after a cut, never `Leaf` again.
- **A merge into master.** The model has the spec's process: a two-way PR, its evidence complete, waits on its turn, re-checks against master just before landing, and its captain merges it (R26, R29); a one-way PR, its CI green, waits on his click (R27, R31); a re-land after a revert takes the same route. `dydo merge` merges only into `feature/*` branches until DYD-1207 ships.
- **One more round after a PASS** is ordered once. R8 says "may still order one more".
- **A seat** may pass a need up like any captain (R5, H16), and never cuts (R34).

## What neither the model nor the view holds

These have no order the model could check, or belong to another chart:

- A native helper. It holds no ticket.
- What a captain does inside a state: how a change is written, how a review is run, how a brief is written.
- A problem a captain reports to its officer while its own work goes on, when nothing waits on its answer. It changes no state. Neither does a Question that comes back without its answer: its officer reports it or dispatches a fresh agent, and the captain that waits on it waits on.
- Who the agents are: which model reviews, who does the Fix (by default the thread that wrote the change, S9), a judge's family. The model has the Review hop as a step of its own, apart from the author's proof and the Fix; it cannot see that two hops are held by different agents.
- What a verdict holds beyond what routes it: the shoulds that wait, the nits, the lenses beyond the three the model names, a seat's findings, and which seats it had.
- The coordinator, the envoys, usage limits and liveness beyond one stopped hop. The coordinator's side of a his-call Question, its queue, and what wakes a captain waiting on one, are on no chart yet. A council run on one family (L22) runs the same steps.
- That anything ends. Without fairness a captain may wait forever; the properties are safety checks.

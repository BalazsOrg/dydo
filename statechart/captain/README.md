# The captain statechart

The statechart is the digital twin of the process the role skills describe. It is written by hand, every skill file is checked against it, and it shows who sees what (`dydo/core.md`, "The statechart"). This folder is that twin for one captain and its ticket. No build step writes anything here.

The twin has two levels, as Balazs set them at the DYD-1440 walkthrough: "One thing is the observability of the state chart and that should be simple. [...] But there is also the comprehensive version, which should indeed include all the states and all the logic. So it actually serves as a model, a digital twin, so that we can check if a behavior or the process is faulty by design."

- **The comprehensive model**, `Captain.tla`, holds every state and every rule of the process that has an order to check: 18 states, 75 actions and 26 variables, each action with the rule it rests on. A skill line is checked against the model.
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

Beyond the states, the model holds what the rules decide on: the ticket's result and, for a Question, its stage (H23); whether the work is proved (R5); the route in fog (H10); one abstract part of a ticket that splits, through its dispatch, its return or hand-back, its evidence check, fix or mend, and its wiring (H10, H2, R6, S4, S17, R2); whether the hop it waits on was found stopped and set back to Todo (S24); the Question its work waits on, from the need passed up to the answer (H16); a research Question's call, and a council's steps and its case file (L18, L21); the round count, the one more round, the lens, the verdict's blockers by kind, a cheap should outstanding and the checkpoint line (R8, R9, R10, S10, S17); and the merge's target, its PASS on the head or an ancestor, its gates, CI, re-syncs, and whether it has landed or was reverted (R20 to R31). The chart shows none of these values.

## How the view shows the model

Seven model states are shown inside a chart state whose words are true of them, so the chart draws no step between them:

| Model state | Shown as | What it is | Rests on |
|---|---|---|---|
| `Repairing` | `Fixing` | A conflict or a red gate before the merge, or a real red after it, is repaired by a Fix on the ticket branch, then a round scoped to it. | R19, R21, R22 |
| `AwaitingTurn` | `Merging` | A two-way PR into master, its evidence complete, waits on the coordinator's turn. | R26, R29 |
| `Held` | `Merging` | A PR forced to re-sync a third time, held while the coordinator sequences the colliding work. | R29 |
| `AwaitingClick` | `Merging` | A one-way PR, its CI green, waits on his click. | core "Who decides what"; R27, R31 |
| `Landed` | `Merging` | The merge is in; the Merge hop waits on green on the merge SHA. | R20 |
| `Rerunning` | `Merging` | A red gate after the merge: DYTEST reruns once. | R22 |
| `Mending` | `InReview` | A cheap should is fixed in place after a PASS, before the merge; the new head then gets a round. **Not true of it**: see below. | S17, R23, R24 |

Three cases have no state of their own, so that every chart word stays true of what it shows:

- **The verdict and what follows it are one step.** A PASS goes straight on to the merge, to the one more round, or to the mend; a FAIL straight to the Fix or to the officer; at round 3 the FAIL step is the checkpoint, which writes its line on the Review. The model records what the verdict held: its blockers' kinds, a cheap should, the checkpoint line, the PASS. No model state sits under "Waiting on a reviewer's verdict" once the verdict is in, except `Mending`.
- **Finding the way is a cut.** A captain whose route is not all in sight cuts only the parts in sight, or one part that brings the next into sight (H10); the model marks that with `fog`, inside `Cutting`.
- **A need passed up waits inside `Leaf`** until its officer opens the Question; the captain then waits in `Asking`, on a Question that is open.

Two cases have no chart words true of them, and the chart is his approved view, so its words are not bent: `view-map.mjs` lists them, `check-chart.mjs` prints them on every run, and they go to him as a view change.

- `Mending`, with its steps `MendACheapShould` and `ReviewTheMendedHead`, sits under `InReview`, "Waiting on a reviewer's verdict". After a PASS with a cheap should, the captain waits on the should's fix in place before the merge (S17), not on a verdict. The chart has no state for that wait, and no arrow out of the review for it.
- `Waiting`, when all a captain that splits waits on is its own need, passed up and opened by its officer, sits under "Waiting on its sub-tickets". The officer opened that Question under its own ticket (H16), so it is not this captain's sub-ticket. H10 has the captain wait on it, and after a cut it never goes back to `Asking`.

## How the view is checked

`check-chart.mjs` runs TLC on the model and reads the state graph TLC dumps (`-dump dot,actionlabels`): every reachable state and every step, each named by its action. It does not read a drawing, so the model needs no bound to be checked, and a step that stays inside one chart state is checked as well as one between two. It fails on:

- a line of `chart.mmd` its strict reader cannot place, an arrow without words, two arrows between the same two states, or an arrow that joins two cases with "or";
- a model state the map does not show, an action it gives no place, or a place the chart lacks;
- TLC stopping with an error, so `TypeOK` and `NoDeadlock` hold on every reachable state when it passes;
- a reachable step whose place, the chart state it stays inside or the chart arrow between its two ends' chart states, is not one the map gives its action;
- a place the map gives an action that no step of it takes, a chart arrow no step stands under, a chart state no reachable model state is shown as, and a model state or action never reached.

The statechart tool draws a chart only up to 500 model states, and the model has more than 16,000, so it draws no chart; it still checks `TypeOK` and `NoDeadlock` and every property. `view.mjs` stays for it.

## Change it

Change the process in `Captain.tla` first, with the rule each action rests on, and give each new state and action its places in `view-map.mjs`. A model step that crosses between two chart states must stand under a chart arrow whose words are true of it, and a model state must be shown as a chart state whose words are true of it. When no chart words fit, do not bend them: list the case in `mismatches` in `view-map.mjs`, and bring it to him with the case, because the view is his to approve. Change only the words of a state or an arrow in `chart.mmd` alone. Then check it.

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

- **A Question's stages.** At DYD-1454 an agent ruled that the stages draw nothing, under the reading that the simple chart was the whole process. He has since said it is not. The core page says a Question's captain decides its answer and says nothing of how; the spec says how (H16, H23, L18). So the model has them, as the held ticket's stage: a research Question's captain researches first, then decides, or opens a council child for a hard call not worth his time, or a his-call child for a call worth it, and answers from the child. A council's chair runs the sealed positions, the one exchange and the exits. Only a research Question opens a child.
- **Who opens a Question.** H16: "the receiving officer opens it [...] and waits on it". The core page does not name the opener. The model follows H16: a captain passes its need up and goes on with what does not wait on it (S10); its officer opens the Question. A captain opens one only as the receiving officer: a splitter for a part's need, a merger for a reviewer's blocker that needs Balazs, which R9 sends "up the chain at once" and S10 makes "a Question ticket that blocks only what waits on it": the Fix waits on it. The model holds one Question at a time; more are the same case again.
- **The checkpoint.** R10 lets the officer at round 3 escalate "only for a fundamental break, a new requirement, or something that needs him", and R9 sends a fundamental blocker to a re-plan whatever the round. So the round-3 FAIL is the checkpoint, decided by the blockers' kinds: one more round for blockers a Fix can fix, keeping the security lens first after a security hole; escalated to a re-plan for a fundamental one; escalated as a Question for a need for him, with the Fix waiting on it.
- **A cheap should.** S17 says a should is "fixed in place before the parent merges if cheap". The model carries a cheap should from the verdict that found it to its mend, and no merge starts with one outstanding. A part's cheap should, found by its evidence check, is mended by the part before its parent wires it; a merger's is mended, by default, by the thread that wrote the change, and the mended head gets a round (R23).
- **A red gate before the merge.** R22 speaks of "a red gate after a merge": rerun once, revert at once if real. R21 has the captain send an implementer "for a CONFLICT or a real red gate". Before the merge nothing has landed, so nothing is reverted: a red gate or a conflict goes straight to a repair. How a red before the merge is told real from a flake is not said; the model has one step.
- **A captain that splits and authors.** R16 lets a captain that splits author a change too small to need its own review. The core page and `AGENTS.md` say a captain that splits only orchestrates. The model follows the core page: after a cut, never `Leaf` again.
- **A merge into master.** The model has the spec's process: a two-way PR, its evidence complete, waits on its turn, re-checks against master just before landing, and its captain merges it (R26, R29); a one-way PR, its CI green, waits on his click (R27, R31); a re-land after a revert takes the same route. `dydo merge` merges only into `feature/*` branches until DYD-1207 ships.
- **One more round after a PASS** is ordered once. R8 says "may still order one more".
- **A seat** may pass a need up like any captain (R5, H16), and never cuts (R34).

## What neither the model nor the view holds

These have no order the model could check, or belong to another chart:

- A native helper. It holds no ticket.
- What a captain does inside a state: how a change is written, how a review is run, how a brief is written.
- A problem a captain reports to its officer while its own work goes on, when nothing waits on its answer. It changes no state. Neither does a Question that comes back without its answer: its officer reports it or dispatches a fresh agent, and the captain that waits on it waits on.
- Who the agents are: which model reviews, who does the Fix (by default the thread that wrote the change, S9), which head a PASS names, a judge's family. The model checks order only.
- What a verdict holds beyond what routes it: the shoulds that wait, the nits, the lenses beyond the three the model names, and which seats it had.
- The coordinator, the envoys, usage limits and liveness beyond one stopped hop. The coordinator's side of a his-call Question, its queue, and what wakes a captain waiting on one, are on no chart yet. A council run on one family (L22) runs the same steps.
- That anything ends. Without fairness a captain may wait forever; the properties are safety checks.

# The captain statechart

The statechart is the digital twin of the process the role skills describe. It is written by hand, every skill file is checked against it, and it shows who sees what (`dydo/core.md`, "The statechart"). This folder is that twin for one captain and its ticket. No build step writes anything here.

The twin has two levels, as Balazs set them at the DYD-1440 walkthrough: "One thing is the observability of the state chart and that should be simple. [...] But there is also the comprehensive version, which should indeed include all the states and all the logic. So it actually serves as a model, a digital twin, so that we can check if a behavior or the process is faulty by design."

- **The comprehensive model**, `Captain.tla`, holds every state and every rule of the process that has an order to check: 19 states and 46 actions, each action with the rule it rests on. A skill line is checked against the model.
- **The simple view**, `chart.mmd`, is the chart he approved for reading: 11 states and 23 arrows. It leaves nothing out of the process; it shows each model state as one of its states, and each model step under one of its arrows or inside one of its states.

The truth both are checked against is, in order: his recorded words, then `Spec: dydo 4.0.0`, then the decision records, with the core page while it lives.

## Files

| File | What it is |
|---|---|
| [Captain.tla](Captain.tla), `Captain.cfg` | The comprehensive model, for the TLC model checker. One action is one case of one rule. |
| [chart.mmd](chart.mmd) | The simple view. One arrow is one case in plain words; the comment above an arrow names what it rests on. |
| [view-map.mjs](view-map.mjs) | How the two relate: the chart state that shows each model state, and the chart arrow or chart state each model action belongs to. |
| [positions.md](positions.md) | Who sees what: the skills and context each position gets, and how to check a skill file against the model. |
| `view.mjs`, `chart-words.mjs` | How the statechart tool draws the model as the simple view. They hold no words: they read them from `chart.mmd`. |
| `model-view.mjs` | How the statechart tool draws the model whole, one chart state per model state and each arrow named by its action. For reading the model; nothing is checked against it. |
| `check-chart.mjs` | Checks that `chart.mmd` is a faithful view of the model. |

The properties the model must keep are hand-written too, in [verification/statechart/captain/](../../verification/statechart/captain/obligations.md), apart from the model they judge.

## How the view shows the model

Eight model states are shown inside a chart state. Every step between them stays inside that chart state, so the chart draws none of those steps.

| Model state | Shown as | What it is | Rests on |
|---|---|---|---|
| `Wayfinding` | `Cutting` | The route is not in sight: the captain finds the next part toward the spec, and cuts only the parts in sight. Reached from `Taken` and, when fog remains, from `Waiting`. | core "A ticket"; H10 |
| `Clear` | `InReview` | A round found no blocker. Next comes the merge, one more round (once), or a cheap should mended in place. | R8, S17 |
| `Checkpoint` | `InReview` | A third round had a blocker. The officer running the rounds decides: by default one more round, or it escalates, and it writes the checkpoint line on the Review. | R10, S10 |
| `Mending` | `InReview` | A cheap should is fixed before the merge. The new head gets a round of its own. | S17, R23, R24 |
| `Repairing` | `Fixing` | A merge conflict or a real red gate is repaired on the ticket branch, then a fix-scoped round. | R19, R21, R22 |
| `AwaitingTurn` | `Merging` | A two-way PR into master waits on the coordinator's turn. | R26, R29 |
| `Rerunning` | `Merging` | A red gate after a merge: DYTEST reruns once. | R22 |
| `AwaitingClick` | `Merging` | A one-way PR waits on his click, after its CI is green. | core "Who decides what"; R27, R31 |

The model also counts the review rounds (`round`), remembers the one more round after a PASS (`extra`), what the next round looks with (`lens`), the stage of a Question it waits on (`stage`: research, council or his-call; H23), whether the ticket is filler that must pass its check (`filler`; H11), and where the merge goes (`target`). The chart shows none of these values.

`check-chart.mjs` proves the view faithful, two ways:

- From the files: every model state is shown as a chart state; every model action has a place, a chart arrow or a chart state; and every chart arrow has at least one model action under it, so no arrow claims behaviour the model lacks.
- From TLC's walk of every reachable step, drawn through `view.mjs`: the drawing and `chart.mmd` have the same states and the same arrows in the same words, no step crossed between chart states except where `view-map.mjs` places it, and no arrow joins two cases with "or".

## Change it

Change the process in `Captain.tla` first, with the rule each action rests on, and give each new state and action its place in `view-map.mjs`. A model step that crosses between two chart states must stand under a chart arrow whose words are true of it. When no arrow's words fit, the view needs a new arrow or state, and the view is his to approve: bring it to him with the case. Change only the words of a state or an arrow in `chart.mmd` alone. Two arrows between the same two states are two cases, and the tool would join them with "or". Then check it.

## Check it

Use an absolute path for this checkout in place of `REPO`, and a new scratch folder each time. Commit first: the property runner refuses a dirty checkout.

```text
dydo statechart install-skill --root /tmp/chartroot
/var/tmp/dydo-gate-lock.sh dydo statechart REPO/statechart/captain/Captain.tla --skill /tmp/chartroot/skills/engineering/statechart --config REPO/statechart/captain/Captain.cfg --view REPO/statechart/captain/view.mjs --out /tmp/captain-drawn
node REPO/statechart/captain/check-chart.mjs /tmp/captain-drawn/chart.mmd
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-properties
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --controls --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-controls
```

The first command draws the model as the simple view and checks `TypeOK` and `NoDeadlock`. The second fails on any difference between the view and the model, as above. The third checks the properties. The fourth runs a deliberately wrong step and passes when the properties catch it. To read the model whole, run the first command again with `--view REPO/statechart/captain/model-view.mjs` and a new `--out`. Never commit what the commands write.

The statechart tool draws a chart only up to 500 model states. The model reaches 373, so a change that adds a variable or a value may need the model made smaller to stay drawn. `Captain.tla` reads the merge target only when the merge comes next, and forgets the filler flag once the ticket is taken, for that reason.

## Where the model settles between its sources

- **A Question's stages.** At DYD-1454 an agent ruled that the stages draw nothing, under the reading that the simple chart was the whole process. He has since said it is not. The core page says a Question's captain decides its answer and says nothing of how; the spec says how (H16, H23, L18). So the model has them: every Question opens at stage research; its captain decides, or opens a council child for a hard two-way call, or a his-call child for a call worth his time, and waits on its answer. It no longer hands its own ticket back with his question on it.
- **Who opens a Question.** H16 has the officer that receives the need open it; `positions.md` has the captain with the call open it. Either way the captain waits on it, so the model has one step.
- **A captain that splits and authors.** R16 lets a captain that splits author a change too small to need its own review. The core page and `AGENTS.md` say a captain that splits only orchestrates. The model follows the core page: after a cut, never `Leaf` again.
- **A merge into master.** The model has the spec's process: a two-way PR waits on its turn and its captain merges it (R26, R29), and a one-way PR waits on his click (R27, R31). `dydo merge` merges only into `feature/*` branches until DYD-1207 ships.
- **One more round after a PASS** is ordered once. R8 says "may still order one more".

## What neither the model nor the view holds

These have no order the model could check, or belong to another chart:

- A native helper. It holds no ticket.
- What a captain does inside a state: how a change is written, how a review is run, how a brief is written.
- A problem a captain reports to its officer while its own work goes on. It changes no state, and neither does a Question that comes back without its answer: the captain reports it, or dispatches a fresh agent, and waits on.
- Who the agents are: which model reviews, who does the Fix (by default the thread that wrote the change, S9), which head a PASS names. The model checks order only.
- What a verdict holds: its blockers, shoulds and nits, the lenses beyond the three the model names, and which seats it had.
- The coordinator, the envoys, usage limits and liveness. The coordinator's side of a his-call Question, and what wakes a captain waiting on one, are on no chart yet.
- That anything ends. Without fairness a captain may wait forever; the properties are safety checks.

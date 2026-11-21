# The captain statechart

The statechart is the digital twin of the process the role skills describe. It is written by hand, every skill file is checked against it, and it shows who sees what (`dydo/core.md`, "The statechart"). This folder is that chart for one captain and its ticket. No build step writes anything here.

The truth it is checked against is the core page first, then `Spec: dydo 4.0.0`. Where they disagree, the core page wins.

## Files

| File | What it is |
|---|---|
| [chart.mmd](chart.mmd) | The chart: 11 states, 21 arrows. One arrow is one case in plain words. The comment above an arrow names what it rests on. |
| [positions.md](positions.md) | Who sees what: the skills and context each position on the chart gets, and how to check a skill file against it. |
| `Captain.tla`, `Captain.cfg` | The same machine for the TLC model checker. One action is one arrow. |
| `view.mjs`, `chart-words.mjs` | How the statechart tool draws the model. They hold no words: they read them from `chart.mmd`. |
| `check-chart.mjs` | Checks that `chart.mmd` and the model are the same machine. |

The properties the model must keep are hand-written too, in [verification/statechart/captain/](../../verification/statechart/captain/obligations.md), apart from the model they judge.

## Change it

Edit `chart.mmd` and `Captain.tla` together: an arrow added, dropped or moved in one is added, dropped or moved in the other. Change only the words of a state or an arrow in `chart.mmd` alone. Two arrows between the same two states are two cases, and the tool would join them with "or", so give the second case a state of its own or make the two one case. Then check it.

## Check it

Use an absolute path for this checkout in place of `REPO`, and a new scratch folder each time. Commit first: the property runner refuses a dirty checkout.

```text
dydo statechart install-skill --root /tmp/chartroot
/var/tmp/dydo-gate-lock.sh dydo statechart REPO/statechart/captain/Captain.tla --skill /tmp/chartroot/skills/engineering/statechart --config REPO/statechart/captain/Captain.cfg --view REPO/statechart/captain/view.mjs --out /tmp/captain-drawn
node REPO/statechart/captain/check-chart.mjs /tmp/captain-drawn/chart.mmd
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-properties
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --controls --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-controls
```

The first command draws the model and checks `TypeOK` and `NoDeadlock`. The second fails on any arrow or state that the chart and the model do not share, and on an arrow that joins two cases with "or". The third checks the properties. The fourth runs a deliberately wrong step and passes when the properties catch it. Never commit what the commands write.

## What the chart leaves out

- A native helper. It holds no ticket.
- What a captain does inside a state: how a change is written, how a review is run, how a part is briefed.
- A problem a captain reports to its officer while its own work goes on. It changes no state.
- The number of a review round. After a third FAIL the captain writes one line on the Review saying whether it took the Fix or went to its officer (R10, S10). That line is a record on two arrows the chart already has.
- One more round after a PASS, which R8 lets an officer order, and a cheap should mended before the merge, which S17 allows. A PASS ends the review, and a should becomes a ticket that waits.
- A merge that a revert cannot undo. That is Balazs's call, and the chart's merge is into a feature branch, which a revert undoes.
- The stages a Question passes on its way to Balazs (H16, H23). For the captain that asks, it is one wait.
- Who the agents are: which model reviews, which head a PASS names. The model checks order only.
- The coordinator, usage limits, and waking a quiet thread.

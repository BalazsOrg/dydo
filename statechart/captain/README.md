# The captain statechart

The statechart is the digital twin of the process the role skills describe. It is written by hand, every skill file is checked against it, and it shows who sees what (`dydo/core.md`, "The statechart"). This folder is that twin for one captain and its ticket. No build step writes anything here.

The twin has two levels, as Balazs set them at the DYD-1440 walkthrough: "One thing is the observability of the state chart and that should be simple. [...] But there is also the comprehensive version, which should indeed include all the states and all the logic. So it actually serves as a model, a digital twin, so that we can check if a behavior or the process is faulty by design."

- **The model**, `CaptainBase.tla` and `Captain.tla`, is the comprehensive level. A skill line is checked against it.
- **The simple view**, `chart.mmd`, is the chart he approved for reading: 11 states and 23 arrows. Each model state is shown as one of its states, and each model step under one of its arrows or inside one of its states.
- **The register**, `register.md`, is the closed list of rules the model is held to. Every rule of his recorded words, the spec and the core page is in it, as an action, a property, or not an order with its reason.

## Where it stands: the frame

The model is being built in parts against the register (DYD-1492, re-cut at DYD-1504). Part 1, the frame, is in:

- the register, with every row's part: 1 the frame, 2 intake and Questions, 3 the split, 4 review and merge;
- `CaptainBase.tla`: the variables the parts share, and the event record;
- `Captain.tla`: one action for each arrow of the chart, and the few steps inside a chart state that the arrows' conditions need, each naming its register rows;
- each chart arrow's condition, in `view-map.mjs`, tested on every step;
- the frame's properties, in `verification/statechart/captain/`.

Parts 2 to 4 replace the frame's actions in their section of `Captain.tla` with the full process their rows name, and write the names they build into the register. Until then the model is at the chart's level and holds no more than the frame: the round count, the checkpoint, the extra round, the mend, the merge into master, a Question's stages and the council are rows owed by their parts. The model as it stood before the re-cut is at `9c67651e`, for the parts to draw on.

## The event record

Every step sets `event` to the set of events it is, each with its name and who takes it: the captain, its officer, a part's holder, the reviewer on the round's Review hop, the Fix's holder, the captain of the Question it waits on, Balazs, or the tools. A property reads an event from this record, never from the state a step lands in. An arrow's condition reads it too: "the verdict is PASS" holds of a step only when the verdict on record is PASS and the step is the captain acting on it. An actor named on an event is its only actor: a step that gives the same event to a second position fails both.

The facts a rule decides on (the ticket as found on dispatch, its result, the proof on the head, a call met that the captain may not decide, what the captain found when it checked its contract, the verdict and its blockers, the gates, the landing) are variables beside it, each naming the rows it serves. `FR29_TheRecordIsFaithful` holds the record to them: every event is taken by the position it belongs to, and each fact changes only in a step whose events name that change.

## Files

| File | What it is |
|---|---|
| [register.md](register.md) | The closed rule register. |
| [CaptainBase.tla](CaptainBase.tla) | The shared frame: the variables, the event record, `TypeOK` and `Init`. |
| [Captain.tla](Captain.tla), `Captain.cfg` | The model's actions and `Next`, for the TLC model checker. Each action names the register rows it rests on. |
| [chart.mmd](chart.mmd) | The simple view. One arrow is one case in plain words; the comment above it names what it rests on. |
| [view-map.mjs](view-map.mjs) | How the two relate: the chart state that shows each model state, the chart arrows and states each action's steps belong to, each arrow's condition, and the model cases no chart words are true of. |
| [positions.md](positions.md) | Who sees what: the skills and context each position gets, and how to check a skill file against the model. |
| `check-chart.mjs`, `chart-words.mjs`, `tlc.mjs` | The view check, its strict reader of `chart.mmd`, and its TLC runner and graph reader. |
| `check-register.mjs` | The register check. |
| `mutants.mjs` | Wrong models the checks must catch. |
| `view.mjs` | How the statechart tool draws the model as the simple view. It holds no words: it reads them from `chart.mmd`. |

The properties the model must keep are hand-written too, in [verification/statechart/captain/](../../verification/statechart/captain/obligations.md), apart from the model they judge.

## How the view is checked

`check-chart.mjs` takes no arguments. It runs TLC on `Captain.tla` and `Captain.cfg` as they stand beside it, with every `.tla` file beside it, in a new scratch folder. It reads the state graph TLC dumps (`-dump dot,actionlabels`): every reachable state with the value of each variable, and every step, named by its action. It fails on:

- any change to the chart's visible words: the SHA-256 of every line Mermaid renders must be the one pinned in the checker, the chart he approved at DYD-1440 as committed at `8077c474`; a comment may change;
- a line of `chart.mmd` its strict reader cannot place, an arrow without words, two arrows between the same two states, or an arrow that joins two cases with "or";
- a `Captain.cfg` that says anything but `INIT Init`, `NEXT Next` and `INVARIANT TypeOK`, so nothing is pruned and the deadlock check runs;
- a model state the map does not show, an action it gives no place, a chart arrow with no condition, or a place the chart lacks;
- TLC stopping with an error, so `TypeOK` and `NoDeadlock` hold on every reachable state when it passes;
- a graph that is not this model's whole graph: a state count other than TLC's distinct states, a step count other than the states TLC generated after the initial ones, or a state that does not give each variable of `CaptainBase.tla` once;
- a step named `Ended`, the stand-still, that moves or stands still anywhere but at an end, and any other step that changes nothing;
- a reachable step whose place, the chart state it stays inside or the chart arrow between its two ends' chart states, is not one the map gives its action;
- **a step under an arrow where the arrow's condition is false of it**: each arrow's words are tested on every step under it, over the values of the step's two states and its events, not on its endpoints alone;
- a place the map gives an action that no step of it takes, a chart arrow no step stands under, a chart state no reachable model state is shown as, and a model state or action never reached.

`mutants.mjs` shows the checks bite. Each mutant changes one line of a copy of the model, and the check it names must fail on it: the view check for the PASS arrow taken on a FAIL, the FAIL arrow on a PASS, a close before the result is in, a held ticket taken, the next parts cut on a met contract, a hand-back that overwrites its reason, and eleven more; the frame's six properties for ten more, among them a captain's verdict beside the reviewer's, an officer's proof beside the captain's, and a verdict written under another event's name.

`check-register.mjs` checks that the register stays closed: every spec rule and every source key it defines is cited by a row; every action in `Next` is named by a row of part 1 and its comment cites a row; every registered property is named after its property row; and every row a file cites exists.

## Change it

A rule is added to `register.md` first, in its part's section, with where it comes from. Then the model: change the process in `Captain.tla`, in the section of the row's part, with the rows each action rests on in its comment. A step records its events in `event`; a new event name goes into `EventNames`, and a new fact a rule decides on gets a variable in `CaptainBase.tla`, in the section of the part that needs it. Give each new state and action its places in `view-map.mjs`. A model step that crosses between two chart states must stand under a chart arrow whose condition holds of it, and a model state must be shown as a chart state whose words are true of it. When no chart words fit, do not bend them: list the case in `mismatches` in `view-map.mjs` and bring it to him, because the view is his to approve. A change to the chart's words needs his approval, and with it a new pin in `check-chart.mjs`. Then check it.

## Check it

Use an absolute path for this checkout in place of `REPO`, and a new scratch folder each time. Commit first: the property runner refuses a dirty checkout.

```text
dydo statechart install-skill --root /tmp/chartroot
/var/tmp/dydo-gate-lock.sh dydo statechart REPO/statechart/captain/Captain.tla --skill /tmp/chartroot/skills/engineering/statechart --config REPO/statechart/captain/Captain.cfg --view REPO/statechart/captain/view.mjs --out /tmp/captain-drawn
/var/tmp/dydo-gate-lock.sh node REPO/statechart/captain/check-chart.mjs
node REPO/statechart/captain/check-register.mjs
/var/tmp/dydo-gate-lock.sh node REPO/statechart/captain/mutants.mjs
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-properties
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --controls --machine captain --skill /tmp/chartroot/skills/engineering/statechart --out /tmp/captain-controls
```

The first command checks `TypeOK` and `NoDeadlock` with the statechart tool, draws the chart when the model has at most 500 states, and installs the Java and TLC the other checks use; set `JAVA` and `TLA2TOOLS` to use other copies. The next three are the view check, the register check and the mutants. The fifth checks the properties. The sixth runs a deliberately wrong step and passes when the properties catch it. Never commit what the commands write.

## What neither the model nor the view holds

These have no order the model could check, or belong to another chart; the register's "Not an order" says why for each source line:

- A native helper. It holds no ticket.
- What a captain does inside a state: how a change is written, how a review is run, how a brief is written.
- Who the agents are: which model reviews, who does the Fix, a judge's family. The event record names the position that takes a step, not the agent.
- The coordinator, the envoys, usage limits and the landing: their own machines.
- That anything ends. Without fairness a captain may wait forever; the properties are safety checks.

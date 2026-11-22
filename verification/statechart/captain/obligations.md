---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain chart obligations

Hand-written from the rows of the captain's rule register ([statechart/captain/register.md](../../../statechart/captain/register.md)), never from the model (K3). The register is the closed list of rules drawn from his recorded words, the spec and the core page; each property here is named after its row and says what that row requires. When the model breaks one, the model is wrong.

The model they judge is the frame of the captain model (DYD-1504): `CaptainBase.tla` and `Captain.tla` in [statechart/captain/](../../../statechart/captain/README.md), at the chart's level, 11 states and 35 actions. The parts that build the phases (intake and Questions, the split, review and merge) add the properties their rows name, here, and register them in `CaptainProperties.cfg`.

## How a property reads the model

Every step of the model sets `event`, the event record: the set of events the step is, each with its name and who takes it. A property reads an event from it (`Happens`, and `OnlyBy` when the event is one position's alone) and never infers one from the state a step lands in. The facts a rule decides on are the model's other variables, and FR29 ties each change of a fact to the event that names it.

`seen` is the passive set of the states this captain has been in. Every model step has exactly one observer extension, no step is disabled, the model needs no bound, and no fairness is added.

## Properties

`CaptainProperties.cfg` registers six properties; the property run also checks `TypeOK`, from `Captain.cfg`, and the deadlock check.

| Property | Row | What it checks |
|---|---|---|
| FR24_DoneWhenTheContractIsMet | FR24 | A close is the captain's alone, with nothing waiting on an answer and the contract met: a merger's merge in, with gates and CI green on the merge SHA; any other ticket's result in (a change proved, or its parts back when it only groups them; a verdict written; an answer settled; findings or a cut written). |
| FR25_EndsAreFinal | FR25 | After a step that closes the ticket or hands it back, nothing changes. |
| FR26_EachDecidesItsJob | FR26 | The verdict on a change is written by the reviewer alone, never the captain or the Fix's holder; a verdict ticket's captain writes its own; only an answer's captain settles its call. |
| FR27_FailHasABlocker | FR27 | A verdict written FAIL has at least one blocker, and one written PASS has none. |
| FR28_AVerdictIsNotReviewedAgain | FR28 | A reviewer is dispatched only onto a change. |
| FR29_TheRecordIsFaithful | FR29 | Every event of a step is taken by the position it belongs to (`Owner`, from the register's rows), and by no other. The ticket as found never changes, and every other fact changes only in a step whose events name that change, each way it can change: the kind with the step that sets the ticket In Progress, a written verdict with its writing, the proof with a proof or the wiring, its loss with a Fix or a split, and so on for each variable. |

The register's other property rows are owed by parts 2 to 4. The properties written for the model before the re-cut, at `9c67651e`, are there for those parts to draw on; each must be rewritten from its register row, reading the event record.

## What the model cannot show

- Who the agents are: the event record names the position that takes a step (the captain, its officer, a part's holder, the reviewer on the round's Review hop, the Fix's holder, the Question's captain, Balazs, the tools), not which agent holds it. FR26 checks the positions; that the reviewer is not the same agent as the author is checked by reading the record.
- What a brief or a skill says. `statechart/captain/positions.md` is checked by reading.
- That anything ends: the properties are safety checks.

## Controls

The captain's registered control in `verification/statechart/controls.json` starts in `Leaf` on a ticket whose result is a verdict, with a reviewer's dispatch on record, and steps to `InReview`. `FR28_AVerdictIsNotReviewedAgain` must catch it, and no other frame property reads that step as broken. `run.py --controls` keeps every variable but `state` unchanged on its wrong step, so the step's events are the ones its start state records; `run.py` initializes every variable of `CaptainBase.tla` but `state` explicitly.

`statechart/captain/mutants.mjs` adds eleven property mutants: a close before the result is in and a second actor on a close (FR24), a step after a close (FR25), the captain writing the verdict on its own change and a captain's verdict beside the reviewer's (FR26), a FAIL with no blocker (FR27), a verdict sent to review (FR28), and a verdict written under another event's name, an officer's proof beside the captain's, a Fix that clears the blockers under its proof, and a dispatch that brings in blockers no verdict wrote (FR29). Each must be reported broken by TLC.

## Receipts

Each command ran on the committed frame at `0507e88ba0cb6cafa482cfe3fc484e42a788edc8` (the review r3 fix; clean checkout), under the gate lock and the agent memory cap, with dydo 3.6.0's managed Java 21.0.8 and TLC 2.19.

- The statechart tool on the model (`dydo statechart ... --view view.mjs`): exit 0; `TypeOK` and `NoDeadlock` hold; 361 states and 581 steps; it draws 11 states and 23 arrows, with no step drawn as not on the map or the chart.
- The view check (`node statechart/captain/check-chart.mjs`): exit 0. TLC on `Captain.cfg`: 584 states generated, 361 distinct, depth 13; 481 steps checked; the words are the approved words; 11 model states shown as 11 chart states, 35 actions under 23 arrows or inside a state, each arrow's condition holding on every step under it.
- The register check (`node statechart/captain/check-register.mjs`): exit 0. 116 rows that order the work (part 1: 23 actions and 6 properties; part 2: 22 and 10; part 3: 16 and 7; part 4: 19 and 13) and 44 lines of sources that order no step; all 200 spec rules and 75 other source keys cited; 35 actions and 6 properties each rest on a row.
- The mutants (`node statechart/captain/mutants.mjs`): exit 0. On the model as it stands every property holds (1,783 states generated, 1,191 distinct, depth 21). Each of the 29 mutants is caught by the check it names: 18 by the view check, the PASS arrow taken on a FAIL among them, and 11 by the properties.
- The properties (`run.py --machine captain`): exit 0, result pass. `TypeOK`, `NoDeadlock` and the six registered properties hold, none missing.
- The registered controls (`run.py --controls`, for the captain, then for claim-build and review): harness exit 0, each `control_caught` true. For the captain, FR28_AVerdictIsNotReviewedAgain alone rejects the review of a verdict. The other two machines show the runner change holds.
- `dydo check`: 0 errors, 0 warnings.

Output was written under `/tmp/dyd1504/run4/` and is not kept.

## Related

- [The register](../../../statechart/captain/register.md)
- [The captain statechart](../../../statechart/captain/README.md)
- [Who sees what](../../../statechart/captain/positions.md)

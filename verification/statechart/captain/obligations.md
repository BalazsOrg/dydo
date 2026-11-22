---
area: reference
type: reference
explains: ["verification/statechart/captain/**"]
---

# Captain chart obligations

Hand-written from the rows of the captain's rule register ([statechart/captain/register.md](../../../statechart/captain/register.md)), never from the model (K3). The register is the closed list of rules drawn from his recorded words, the spec and the core page; each property here is named after its row and says what that row requires. When the model breaks one, the model is wrong.

The model they judge is the frame of the captain model (DYD-1504): `CaptainBase.tla` and `Captain.tla` in [statechart/captain/](../../../statechart/captain/README.md), at the chart's level, 11 states and 33 actions. The parts that build the phases (intake and Questions, the split, review and merge) add the properties their rows name, here, and register them in `CaptainProperties.cfg`.

## How a property reads the model

Every step of the model sets `event`, the event record: the set of events the step is, each with its name and who takes it. A property reads an event from it (`Happens`, `HappensBy`) and never infers one from the state a step lands in. The facts a rule decides on are the model's other variables.

`seen` is the passive set of the states this captain has been in. Every model step has exactly one observer extension, no step is disabled, the model needs no bound, and no fairness is added.

## Properties

`CaptainProperties.cfg` registers five properties; the property run also checks `TypeOK`, from `Captain.cfg`, and the deadlock check.

| Property | Row | What it checks |
|---|---|---|
| FR24_DoneWhenTheContractIsMet | FR24 | A close is the captain's, with nothing waiting on an answer and the contract met: a merger's merge in, with gates and CI green on the merge SHA; any other ticket's result in (a change proved, or its parts back when it only groups them; a verdict written; an answer settled; findings or a cut written). |
| FR25_EndsAreFinal | FR25 | After a step that closes the ticket or hands it back, nothing changes. |
| FR26_EachDecidesItsJob | FR26 | The verdict on a change is written by the reviewer, never the captain or the Fix's holder; a verdict ticket's captain writes its own; only an answer's captain settles its call. |
| FR27_FailHasABlocker | FR27 | A verdict written FAIL has at least one blocker, and one written PASS has none. |
| FR28_AVerdictIsNotReviewedAgain | FR28 | A reviewer is dispatched only onto a change. |

The register's other property rows are owed by parts 2 to 4. The properties written for the model before the re-cut, at `9c67651e`, are there for those parts to draw on; each must be rewritten from its register row, reading the event record.

## What the model cannot show

- Who the agents are: the event record names the position that takes a step (the captain, its officer, a part's holder, the reviewer on the round's Review hop, the Fix's holder, the Question's captain, Balazs, the tools), not which agent holds it. FR26 checks the positions; that the reviewer is not the same agent as the author is checked by reading the record.
- What a brief or a skill says. `statechart/captain/positions.md` is checked by reading.
- That anything ends: the properties are safety checks.

## Controls

The captain's registered control in `verification/statechart/controls.json` starts in `Leaf` with a change that is not proved and a close on record, and steps straight to `Done`. `FR24_DoneWhenTheContractIsMet` must catch it. `run.py --controls` initializes every variable of `CaptainBase.tla` but `state` explicitly.

`statechart/captain/mutants.mjs` adds one mutant for each property: a close before the result is in (FR24), a step after a close (FR25), the captain writing the verdict on its own change (FR26), a FAIL with no blocker (FR27), and a verdict sent to review (FR28). Each must be reported broken by TLC.

## Receipts

To be written from the runs on the committed frame.

## Related

- [The register](../../../statechart/captain/register.md)
- [The captain statechart](../../../statechart/captain/README.md)
- [Who sees what](../../../statechart/captain/positions.md)

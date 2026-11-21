---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainReviewModel

## Own

`claim:CaptainReviewModel.Own`: Where the project has a skill that checks the subject formally, a modeller runs that check first.

Spec: R34

Evidence (run): `skills/engineering/maintain-formal-verification-skill/SKILL.md`: "A verify skill has no fixed name, so find it by what it does: a project skill that runs a model checker, a prover, or another verifier against named code", and a check "can keep passing while it describes code that no longer exists".

Layer: A passing check says its model still holds, and nothing about whether the model still matches the code.

Reason: A model someone already wrote and maintains finds more, for less, than a new one drawn in an afternoon.

**ticket-captain** (ReviewModel; read by TicketCaptain):

> Where your project has a skill that checks the subject formally, run that check first, and hold what it describes against the code as it is now.

## Chart

`claim:CaptainReviewModel.Chart`: Otherwise a modeller writes a bounded model of the subject's states, each rule the spec promises an invariant, and checks it with dydo statechart.

Spec: R34, R7

Evidence (command): `dydo statechart <spec> --skill <folder> --config <file> --view <file> --out <dir>`

Layer: The statechart skill is loaded only when it is named, so no seat meets it unless the line that needs it points there.

Reason: A model finds the order of events nobody tried, which neither a reader nor a driver reaches.

**ticket-captain** (ReviewModel; read by TicketCaptain):

> Otherwise write a bounded model of the subject's states, each rule the spec promises as an invariant, and check it with `dydo statechart <spec> --skill <statechart-skill-folder>`; the skill in that folder says how to write the model and how to read its report.

## Path

`claim:CaptainReviewModel.Path`: A modeller returns each rule that breaks with its shortest path, as steps in the subject's own words, and what the model's bounds leave out.

Spec: R34, R5

Evidence (run): `skills/engineering/statechart/SKILL.md`: the report "gives the state count and each bug path", and a step label is written "as the user sees it happen, such as "you press play" or "the connect fails". Do not name the code or the TLA+ action."

Layer: The checker prints a trace in the model's variables, which mean nothing to whoever has to fix the code.

Reason: A pass covers the model's scope alone, and a reader not told that scope takes it for the whole system.

**ticket-captain** (ReviewModel; read by TicketCaptain):

> Return each rule that breaks with its shortest path, told as steps in the subject's own words, and say what the model's bounds leave out.

## Finished

`claim:CaptainReviewModel.Finished`: Modelling is due only for a finished subsystem that has a state machine.

Spec: R7, R34

Evidence (run): The user's table holds it: the Formal column is "–" at a leaf's check and at a ticket review, and "if it has a state machine" at a feature or Milestone review. His words (DYD-1082 04b6a3d1): "since it's expensive, it should only be used on things which are supposedly done."

Layer: The checker models whatever it is given, a half-built part as readily as a settled one.

Reason: A model of states that are still moving is out of date before its first run ends.

**ticket-captain** (ReviewModel; read by TicketCaptain):

> Modelling is due only for a finished subsystem that has a state machine; for anything else, return that it is not due.

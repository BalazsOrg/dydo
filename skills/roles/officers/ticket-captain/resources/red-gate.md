<!-- built from the model; edit there: tools/SkillBuild -->

# A red gate after the merge

## Red gate rerun once

Exits:

- Green on the rerun is a flake: the hop is Done, and DYTEST counts the flake; it becomes a ticket only when it repeats.
- Red again, after any merge, is real: revert at once, cancel the Merge hop with `reverted: <gate> red on <merge SHA>`, and file a CI-findings ticket.

## Merge reverted

Exits:

- Send an implementer to fix it on the ticket branch, then hold a fix-scoped round.

## Fix and fix-scoped round

Exits:

- Re-land with `dydo merge`, which reverts the revert.

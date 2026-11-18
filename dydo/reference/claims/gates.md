---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Gates

## Run

`claim:Gates.Run`: Tests run through dydo test, statics through dydo code-check, on changed files only.

Spec: Y6, T21, T13

**code-writer** (Working, nominal):

> Run tests through `dydo test`, and the static gates through `dydo code-check`, which checks changed files only.

## TokenLimit

`claim:Gates.TokenLimit`: Every file, tests included, stays under the 5k-token limit, counted as bytes ÷ 4.

Spec: T15, T14

**code-writer** (Working, nominal):

> Keep every file you change, tests included, under the file token limit: 5k tokens by default, counted as bytes ÷ 4.

## Coverage

`claim:Gates.Coverage`: Coverage is a report, not a gate.

Spec: T9

**reviewer** (Round, nominal):

> Coverage is a report, never a blocker.

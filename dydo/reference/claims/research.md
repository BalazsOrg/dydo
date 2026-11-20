---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Research

## NotLookup

`claim:Research.NotLookup`: Pooling and verifying is research's job; one lookup is a scout's (DYD-1082 99774812).

Spec: K30

**research** (Job):

> Pooling and verifying is the job; a single lookup is a scout's, and you send scouts for those.

## Accounts

`claim:Research.Accounts`: Research answers for a cited answer, what stayed unsettled and where it searched.

Spec: K30

**research** (Job):

> You answer for an answer whose every claim carries a citation you opened, with what stayed unsettled and where you searched.

## Sources

`claim:Research.Sources`: Research's sources are every family that could own the answer.

Spec: K30, S1

**research** (Job):

> Your sources are every family that could own the answer: DRs, the spec and plans, the other docs, code and tests, git history, Linear, and outside sources such as official docs, specs and library source.

## Restate

`claim:Research.Restate`: Research restates its question and what would count as an answer.

Spec: K30, K22

**research** (Briefed, nominal):

> Restate the question in one sentence and say what would count as an answer.

## Families

`claim:Research.Families`: Each source family is marked in or out, with a reason.

Spec: K30, K22

**research** (Families):

> Mark each source family in or out, with a reason.

## DefaultBranch

`claim:Research.DefaultBranch`: Governing docs are read on the default branch too, because a branch can lag.

Spec: K22

**research** (Families):

> Read governing docs on the default branch too; a branch can lag.

## Scouts

`claim:Research.Scouts`: Research sends one scout per family in play from the scout template, naming its skills (K14).

Spec: K30, K14

**research** (Families):

> Send one scout per family in play, in parallel, from the scout brief below, naming its skills with a slash; read a lone family yourself.

## Verify

`claim:Research.Verify`: Research opens the passage behind every load-bearing claim.

Spec: K30

**research** (Working, nominal):

> Open the passage behind every load-bearing claim and confirm it says what the scout said.

## Conflicts

`claim:Research.Conflicts`: Conflicts are named with the source that governs; silence is marked unsettled.

Spec: K30, K22

**research** (Working, nominal):

> Where sources conflict, name the conflict and the source that governs; where they are silent, mark the point unsettled rather than reason your way to a fact.

## Write

`claim:Research.Write`: The answer comes first, each claim with its citation, then what stayed unsettled and every family searched.

Spec: K30, K22

**research** (Working, nominal):

> Write the answer first, then each claim with its citation (a path at its commit, or a URL) and its exact passage, what stayed unsettled, and every family searched, the empty ones included.

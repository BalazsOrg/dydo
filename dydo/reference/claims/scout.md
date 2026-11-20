---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Scout

## Answer

`claim:Scout.Answer`: The scout looks one question up in its one source family, searched to its end.

Spec: K30, K22

**scout** (Working, nominal):

> Look the question up in the one source family your brief names, and search it to its end, following its references inside it.

## Passages

`claim:Scout.Passages`: A scout's finding is the exact passage with its citation, from the source that owns the claim.

Spec: K30, K22

**scout** (Working, nominal):

> Copy each finding as its exact passage with its citation (a path at its commit, or a URL), from the source that owns the claim; a doc's account of the code is a lead to the code.

## Nothing

`claim:Scout.Nothing`: An empty source is reported with what was searched.

Spec: K30, K22

**scout** (Working, nominal):

> When your source holds nothing, say so and list what you searched.

## Pass

`claim:Scout.Pass`: A spec's scout pass returns Questions, never PASS or FAIL.

Spec: R4

**scout** (Working, nominal):

> Return only Questions from that pass, never PASS or FAIL.

## PassHow

`claim:Scout.PassHow`: The Questions pass reads the spec as its implementer and turns each open choice into a Question.

Spec: R4, K30

**scout** (Working, nominal):

> On a spec's Questions pass, read the spec as the implementer who must build it, and turn each choice it leaves open into a Question.

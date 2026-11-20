---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Build

## Refused

`claim:Build.Refused`: In the claim build, the build refuses: a claim not on exactly one line per carrier, a misplaced move, a shape limit.

Spec: K4, K10, K9

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Edited → Rejected, "the build refuses: a claim not on exactly one line per carrier, a misplaced move, a shape limit". A system transition: no skill carries it.

## Write

`claim:Build.Write`: In the claim build, dotnet run --project tools/SkillBuild writes every built file with its header.

Spec: K6, K8, K5, K12, K13, K23

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Edited → Built, "dotnet run --project tools/SkillBuild writes every built file with its header". A system transition: no skill carries it.

## Golden

`claim:Build.Golden`: In the claim build, the golden test finds a byte difference, or the stray-claim lint finds pasted claim text.

Spec: K7, K24

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Built → Rejected, "the golden test finds a byte difference, or the stray-claim lint finds pasted claim text". A system transition: no skill carries it.

## Pass

`claim:Build.Pass`: In the claim build, golden, shape and stray-claim checks pass, in the suite and in CI.

Spec: K7, K10, K24

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Built → Checked, "golden, shape and stray-claim checks pass, in the suite and in CI". A system transition: no skill carries it.

## Mend

`claim:Build.Mend`: In the claim build, the model is fixed.

Spec: K6

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Rejected → Edited, "the model is fixed". A system transition: no skill carries it.

## HandEdit

`claim:Build.HandEdit`: In the claim build, an Edit or Write to a built path.

Spec: K6

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Built → Nudged, "an Edit or Write to a built path". A system transition: no skill carries it.

## EditThere

`claim:Build.EditThere`: In the claim build, the edit goes into the model instead.

Spec: K6

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Nudged → Edited, "the edit goes into the model instead". A system transition: no skill carries it.

## Tlc

`claim:Build.Tlc`: In the claim build, TLC passes the generated moves against the hand-written properties; the result is recorded with the candidate.

Spec: K3, K20, N14

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Checked → TlcPassed, "TLC passes the generated moves against the hand-written properties; the result is recorded with the candidate". A system transition: no skill carries it.

## TlcFail

`claim:Build.TlcFail`: In the claim build, TLC fails a property: the push is not done and nothing goes to his approval.

Spec: K3, K20

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Checked → TlcFailed, "TLC fails a property: the push is not done and nothing goes to his approval". A system transition: no skill carries it.

## Remodel

`claim:Build.Remodel`: In the claim build, the push is not done: the model is fixed and built again.

Spec: K20, K6

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): TlcFailed → Edited, "the push is not done: the model is fixed and built again". A system transition: no skill carries it.

## Read

`claim:Build.Read`: In the claim build, the model PR is read through its built-skill diff.

Spec: K16

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): TlcPassed → InReview, "the model PR is read through its built-skill diff". A system transition: no skill carries it.

## Approve

`claim:Build.Approve`: In the claim build, he approves: in the 4.0.0 push once, at the final candidate; after it, each model PR.

Spec: K16, K20, K22

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): InReview → Approved, "he approves: in the 4.0.0 push once, at the final candidate; after it, each model PR". A system transition: no skill carries it.

## Rejected

`claim:Build.Rejected`: In the claim build, he rejects a change at his walkthrough: new captains on new tickets edit the model, and the candidate is rebuilt (DYD-1195 8661b8f4).

Spec: K16, K22

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): InReview → Edited, "he rejects a change at his walkthrough: new captains on new tickets edit the model, and the candidate is rebuilt (DYD-1195 8661b8f4)". A system transition: no skill carries it.

## Ship

`claim:Build.Ship`: In the claim build, downstream gets the built files in one ordinary update.

Spec: K18

Transition in The claim build (`claims/statechart/claim-build/ClaimBuild.tla`): Approved → Released, "downstream gets the built files in one ordinary update". A system transition: no skill carries it.

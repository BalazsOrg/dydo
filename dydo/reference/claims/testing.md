---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Testing

## Write

`claim:Testing.Write`: A test: a proof test is written for a hop.

Spec: T5

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Start → Written, "a proof test is written for a hop". A system transition: no skill carries it.

## Legacy

`claim:Testing.Legacy`: A test: an old test meets the purge.

Spec: T8, T20

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Start → Old, "an old test meets the purge". A system transition: no skill carries it.

## Rewrite

`claim:Testing.Rewrite`: A test: a rewrite gets a conformance assertion.

Spec: T6

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Start → Conformance, "a rewrite gets a conformance assertion". A system transition: no skill carries it.

## ShowRed

`claim:Testing.ShowRed`: A test: shown failing without the code.

Spec: T3

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Written → Red, "shown failing without the code". A system transition: no skill carries it.

## RunGates

`claim:Testing.RunGates`: A test: the code goes in; the gates run on changed files.

Spec: T13, T21, Y6

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Red → Gated, "the code goes in; the gates run on changed files". A system transition: no skill carries it.

## Green

`claim:Testing.Green`: A test: all selected tests pass and the statics hold.

Spec: T23, Y4, T9

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Gated → Receipt, "all selected tests pass and the statics hold". A system transition: no skill carries it.

## GoRed

`claim:Testing.GoRed`: A test: a selected test or a static gate fails.

Spec: T23, T19

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Gated → GateRed, "a selected test or a static gate fails". A system transition: no skill carries it.

## Refix

`claim:Testing.Refix`: A test: fixed; the gates run again.

Spec: T23

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): GateRed → Gated, "fixed; the gates run again". A system transition: no skill carries it.

## Graduate

`claim:Testing.Graduate`: A test: it graduates with its job tag.

Spec: T5, T4

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Receipt → Graduated, "it graduates with its job tag". A system transition: no skill carries it.

## Drop

`claim:Testing.Drop`: A test: the merge drops a test that does not graduate.

Spec: T5

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Receipt → Dropped, "the merge drops a test that does not graduate". A system transition: no skill carries it.

## Untraced

`claim:Testing.Untraced`: A test: an untraced pin, or a missing job tag.

Spec: T1, R3, T5

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Graduated → Finding, "an untraced pin, or a missing job tag". A system transition: no skill carries it.

## Keep

`claim:Testing.Keep`: A test: it meets the new test requirements: kept.

Spec: T8, T22

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Old → Graduated, "it meets the new test requirements: kept". A system transition: no skill carries it.

## Rewritten

`claim:Testing.Rewritten`: A test: rewritten to meet them.

Spec: T8

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Old → Written, "rewritten to meet them". A system transition: no skill carries it.

## Delete

`claim:Testing.Delete`: A test: deleted, with one line in the commit when it blocks 4.0.0 work.

Spec: T8, T22

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Old → Dropped, "deleted, with one line in the commit when it blocks 4.0.0 work". A system transition: no skill carries it.

## Traced

`claim:Testing.Traced`: A test: a traced assertion is shown failing without the code.

Spec: T6, T3

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Conformance → Red, "a traced assertion is shown failing without the code". A system transition: no skill carries it.

## MatchesOld

`claim:Testing.MatchesOld`: A test: a matches-old-implementation assertion goes at the rewrite's landing.

Spec: T6

Transition in Proof tests and gates (`claims/statechart/proof/Proof.tla`): Conformance → Dropped, "a matches-old-implementation assertion goes at the rewrite's landing". A system transition: no skill carries it.

## Jobs

`claim:Testing.Jobs`: A suite test guards a trap, pins a traced contract, or reaches an edge driving can't; an untraced pin is a finding.

Spec: T1, R3

**code-writer** (Working, nominal):

> Write a suite test only to guard a trap, pin a traced contract (spec or DR), or reach an edge driving can't.

**reviewer** (Round, nominal):

> A test that pins an untraced rule, new or inherited, is a finding.

## RedCheck

`claim:Testing.RedCheck`: Every graduating test is shown failing without the code; a Bug starts from a red reproduction.

Spec: T3

**code-writer** (Working, nominal):

> Show every graduating test failing without the code; a Bug still starts from a red reproduction.

## Boundary

`claim:Testing.Boundary`: A test drives the deepest boundary that can observe the behaviour.

Spec: T4

**code-writer** (Working, nominal):

> Test at the deepest module boundary, or the product boundary, that can observe the behaviour; a unit test graduates only for an edge or trap that test can't reach cheaply.

## JobTag

`claim:Testing.JobTag`: A graduating test carries a job tag naming its evidence; the reviewer checks each tag.

Spec: T5

**code-writer** (Working, nominal):

> Tag each graduating test with its job: trap (naming the bug or pitfall), contract (citing the spec or DR) or edge.

**reviewer** (Round, nominal):

> Check each graduating test's job tag against its evidence.

## ProofSha

`claim:Testing.ProofSha`: Proof tests live in the ticket's commits; the receipt and proof: record the SHA.

Spec: T5

**code-writer** (Working, nominal):

> Keep proof tests in the ticket's commits, and record their SHA in the receipt and in `proof:`.

## OldEvidence

`claim:Testing.OldEvidence`: Old evidence is never load-bearing.

Spec: T5

**code-writer** (Working, nominal):

> Make nothing depend on old evidence: a squash or a fresh start must break nothing.

## ConformanceSuite

`claim:Testing.ConformanceSuite`: A rewrite gets a conformance suite, a separate rule in a separate place from the graduation rules.

Spec: T6

**code-writer** (Conformance, nominal):

> A rewrite or wide refactor gets a conformance suite, apart from its graduating tests and never instead of them: its traced assertions graduate like any graduating test, red first, and its 'matches old implementation' assertions go at the rewrite's landing.

## Smells

`claim:Testing.Smells`: Three smells are pruned: tests that change whenever the code does, check that a deleted feature is gone, or can never fail.

Spec: T22

**code-writer** (Working, nominal):

> Write no test that changes whenever the code does, checks that a deleted feature is gone, or can never fail.

**reviewer** (Round, nominal):

> A test that changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding.

## Guidance

`claim:Testing.Guidance`: Property and differential tests are guidance, not a gate.

Spec: T7

**code-writer** (Working, nominal):

> Property tests for edges and differential tests for rewrites are guidance, never a gate.

## Receipts

`claim:Testing.Receipts`: The author proves with receipts; a DYTEST result for the same state counts.

Spec: R5

**code-writer** (Working, nominal):

> Prove the hop with receipts; a DYTEST result for the same tree counts unless later changes put it in doubt.

**working-tree-standard** (TreeDelivery):

> The author proves with receipts and a fresh reviewer judges through lenses; nothing is re-run the same way, and a DYTEST result for the same tree counts as proof unless later changes put it in doubt.

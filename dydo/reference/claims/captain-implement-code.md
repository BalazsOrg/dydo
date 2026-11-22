---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainImplementCode

A line marked unwritten is the model's own, of a skill written by hand in `skills/roles/officers/ticket-captain/`: the build writes it nowhere, and the hand-written skill need not hold it.

## Jobs

`claim:CaptainImplementCode.Jobs`: A suite test does one of three jobs: it guards a trap, pins a traced contract, or reaches an edge that driving cannot.

Spec: T1

Evidence (code): `skills/roles/crew/implementer/resources/code.md`

Layer: A test runner counts every green test alike, the one that earns its place and the one that only restates the code.

Reason: Green tests can cement behaviour nobody wanted, and each needless one is a cost on every later change.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Write a suite test for one of three jobs only: to guard a trap, to pin a contract the spec or a decision record traces, or to reach an edge that driving the app cannot.

## Boundary

`claim:CaptainImplementCode.Boundary`: A test drives the deepest module boundary, or the product's, that can observe the behaviour; a unit test is kept only for an edge or trap that test cannot reach cheaply.

Spec: T4

Evidence (run): This hop's own tests are made so: each goes through `SkillBuilder.Build` on the production model and reads the built files, and none calls a renderer's inner method.

Layer: Nothing stops a writer from testing each private step, which passes today and breaks at the next refactor.

Reason: One check at the boundary stands in for a thousand inside it, and survives when the inside is rewritten.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Test at the deepest module boundary, or the product's, that can observe the behaviour; add a unit test only for an edge or trap that test cannot reach cheaply.

## Mocks

`claim:CaptainImplementCode.Mocks`: A writer mocks only at the system's edge; its own modules stay real.

Spec: T4

Evidence (code): `skills/roles/crew/implementer/resources/code.md`

Layer: A mocking library replaces any dependency it is pointed at, the writer's own code included.

Reason: A test of mocked collaborators proves how the writer imagines them, never how they behave.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Mock only at the system's edge, such as an external API, time or randomness; your own modules stay real.

## Red

`claim:CaptainImplementCode.Red`: Every new test is shown failing without the code.

Spec: T3

Evidence (run): This hop was made so (DYD-1372 phase 1, hop B): its ten new tests ran red on the unchanged model, 10 failed of 13, before one claim was written.

Layer: A test runner reports green; it cannot say whether the test could ever have been red.

Reason: A test never seen failing proves nothing about the code it sits beside.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Show every new test failing without the code: set the code aside, run the test, see it fail for the reason it names, and put the code back.

## Tag

`claim:CaptainImplementCode.Tag`: Each suite test carries its job as a tag: trap, contract or edge.

Spec: T5

Evidence (run): This repository's suite, read 2026-11-21: `[Trait("Job", "contract: …")]` and `[Trait("Job", "trap: …")]` on each test. `dydo/guides/coding-standards.md`, here and under `Scaffold/`, says every suite test carries a trap, traced contract or edge job tag; the tag's syntax is each test stack's own, which T5 leaves to the project.

Layer: A lint can demand that a tag is present; what the test is for is known only to whoever wrote it.

Reason: A reviewer checks the tag against the test, and a later reader learns why the test may not be deleted.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Tag each suite test with its job, the way the tests beside it carry theirs: trap, naming the bug or pitfall; contract, citing the spec or decision record; or edge.

## Prune

`claim:CaptainImplementCode.Prune`: No test changes whenever the code does, checks that a deleted feature is gone, or can never fail.

Spec: T22

Evidence (code): `skills/roles/crew/implementer/resources/code.md`

Layer: All three kinds pass, so no run ever points at them.

Reason: Each one adds upkeep and a false sense of cover while guarding nothing.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Write no test that changes whenever the code does, checks that a deleted feature is gone, or can never fail.

## Guidance

`claim:CaptainImplementCode.Guidance`: Property tests for edges and differential tests for rewrites are guidance, never a gate.

Spec: T7

Evidence (code): `skills/roles/crew/implementer/resources/code.md`

Layer: No gate of the facade asks for either kind of test.

Reason: They pay off on some code and are noise on the rest, which only the writer can tell apart.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain, on claim:CaptainImplementCode.Conformance):

> A rewrite or wide refactor gets a conformance suite beside its own tests, never in their place; its assertions that only match the old implementation go once the rewrite has replaced it. Property tests for edges and differential tests for rewrites are guidance, never a gate.

## Conformance

`claim:CaptainImplementCode.Conformance`: A rewrite or wide refactor gets a conformance suite beside its own tests, and its matches-the-old-implementation assertions go once the rewrite has replaced the old code.

Spec: T6

Evidence (code): `skills/roles/crew/implementer/resources/code.md`

Layer: A suite written for a refactor looks like any other, and nothing marks which of its assertions outlive the old code.

Reason: A writer who takes the suite for the refactor's only tests ships new behaviour with nothing of its own to hold it.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> A rewrite or wide refactor gets a conformance suite beside its own tests, never in their place; its assertions that only match the old implementation go once the rewrite has replaced it. Property tests for edges and differential tests for rewrites are guidance, never a gate.

## Scenario

`claim:CaptainImplementCode.Scenario`: An acceptance line the contract marks for a scenario is written and wired as the project's coding standards say, and never weakened.

Spec: K31, T1

Evidence (run): `dydo/guides/coding-standards.md`, here and under `Scaffold/`, section 6, Testing: a scenario "is contract where the ticket calls for Gherkin: the captain decides the set, the implementer writes and wires it without weakening it, and the acceptance runner runs it."

Layer: Which runner a project's scenarios go through, and in what words, is that project's choice.

Reason: A scenario is the contract in runnable form, so loosening one changes what was agreed without anyone agreeing.

**ticket-captain** (unwritten; ImplementTests; read by TicketCaptain):

> Where your contract marks an acceptance line for a scenario, write and wire it as `dydo/guides/coding-standards.md` says of a scenario, and never weaken one.

## Graduation

`claim:CaptainImplementCode.Graduation`: Proof tests live in the ticket's commits with their SHA in the receipt, and nothing depends on old evidence.

Spec: T5

Evidence: open, so no skill states this claim yet. T5 has the merge drop the proof tests that do not graduate, and no merge in this repository has been seen to: `dydo merge` (`Services/Merge`) reads no test's tag. Until a run shows which tests a merge keeps, what a writer does with a proof test is not known.

Layer: git keeps every test a commit adds, and nothing tells a proof of one ticket from a guard meant to stay.

Reason: Evidence nobody will look at again is weight every later run carries.

**ticket-captain** (unwritten; withheld; ImplementTests; read by TicketCaptain):

> Keep proof tests in the ticket's commits, record their SHA in your receipt, and make nothing depend on old evidence.

## Rules

`claim:CaptainImplementCode.Rules`: A project's own rules for writing code are in its coding standards, which a writer reads before it writes.

Spec: K31, T14

Evidence (run): `dydo/guides/coding-standards.md` exists, here and under `Scaffold/`, the scaffold dydo ships.

Layer: This skill is one text for every project, and a project's conventions are written only in its own tree.

Reason: Practice differs between projects, and a writer that knows only the general page breaks the local one.

**ticket-captain** (unwritten; ImplementCode; read by TicketCaptain):

> Your project's rules for writing code are in `dydo/guides/coding-standards.md`: read it before you write.

## Comments

`claim:CaptainImplementCode.Comments`: What a comment is for, and what it must never be, is the project's rule, in the Comments section of its coding standards.

Spec: K31

Evidence (run): `dydo/guides/coding-standards.md`, here and under `Scaffold/`, has a section headed Comments under Conventions: a reason the code cannot show, never a retelling of the code. His words (DYD-1372 aa5b445e): "what the comments should be like, what they should not be like".

Layer: No compiler or lint reads a comment for whether it is worth having.

Reason: A model left to its habit narrates every line, and the narration goes stale with the first edit.

**ticket-captain** (unwritten; ImplementCode; read by TicketCaptain):

> Before you write a comment, read Comments in that file: your project says there what a comment is for and what it must never be.

## Gates

`claim:CaptainImplementCode.Gates`: The gates a change must pass are the ones its brief names, run with the project's own commands, which the project's testing guide holds.

Spec: T14, T11

Evidence (run): This fix's brief named its gates one by one (DYD-1372 g1b): the build with warnings as errors, the SkillBuild tests, a clean tree after a rebuild, the file size check and `dydo check`. `dydo/guides/testing-strategy.md`, here and under `Scaffold/`: here its section "Current project facade" gives the runner, and the scaffold's copy is where an adopting project declares its own.

Layer: The CLI has no command that runs a project's tests or its static checks, so which ones count for a ticket is known only from its brief.

Reason: A writer that runs what was asked proves what its officer will check, and guesses at nothing.

**ticket-captain** (unwritten; ImplementCode; read by TicketCaptain):

> The gates your change must pass are the ones your brief names: run them with your project's own commands, which `dydo/guides/testing-strategy.md` holds.

## Disagree

`claim:CaptainImplementCode.Disagree`: Where a project doc and the brief's gates disagree, the gates decide what must pass, and the writer reports the difference to its officer as a finding.

Spec: T9, T10, T11, R5

Evidence (run): Read 2026-11-21: `DynaDocs.Tests/coverage/gate_policy.py` fails a method on two limits only, cognitive complexity and parameters, and no file of the facade names HCRAP. Yet `dydo/guides/coding-standards.md:194` and `dydo/guides/testing-strategy.md:243` still demand 80% line coverage, 60% branch coverage and HCRAP at most 20. Spec CA6 records the change: "Resolved by PR 363 (`892d41f8`): CI coverage is report-only, and HCRAP no longer gates." `.github/workflows/ci.yml` calls its step "Full Python suite and platform coverage report".

Layer: A doc is edited by hand and a gate in code, so the two drift apart, and nothing compares them.

Reason: A writer that takes a stale page for law spends its ticket lifting a measure nobody enforces any more.

**ticket-captain** (unwritten; ImplementCode; read by TicketCaptain):

> Where a project doc and those gates disagree, the gates decide what must pass: report the difference to your officer as a finding, and chase no figure that no gate checks.

## Trip

`claim:CaptainImplementCode.Trip`: A change that trips a static gate is changed until it passes: a gate takes no suppression, and its limit is never raised.

Spec: T11, T19

Evidence (run): `dydo/guides/testing-strategy.md`, read 2026-11-21: "there are no suppressions", and a suppressed diagnostic is "a finding or ignored input, never an escape". `dydo/guides/coding-standards.md`, here and under `Scaffold/`: "There are no tiers, tier annotations, tier registries, classic CRAP thresholds, per-file suppressions".

Layer: An analyzer honours a suppression comment as readily as a fix.

Reason: A gate exists only where a breach is certainly wrong, so an exception to it is a decision above any one ticket.

**ticket-captain** (unwritten; ImplementCode; read by TicketCaptain):

> When your change trips a static gate, such as the one on cognitive complexity, change the code until it passes, splitting or flattening the method it names: a gate takes no suppression, and its limit is never raised.

## Platform

`claim:CaptainImplementCode.Platform`: Platform-specific code is proved on CI's platform too.

Spec: K31

Evidence (code): `skills/roles/crew/implementer/resources/code.md`

Layer: A local run passes on the one platform the writer happens to sit on.

Reason: A path separator or a signal that differs elsewhere fails only after the merge, where a revert is the cure.

**ticket-captain** (unwritten; ImplementCode; read by TicketCaptain):

> Prove platform-specific code, such as processes, paths and signals, on CI's platform too, not only on yours.

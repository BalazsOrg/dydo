---
name: tdd
description: Which tests to write, which earn a place in the suite, and how to prove them. Use when a change needs tests, a bug needs a failing reproduction, a test is up for the suite, or a review reads tests.
---

<!-- Adapted from mattpocock/skills tdd at 6f7f7831a625f040a8e06a0e7954826b87ce775a (MIT). -->
<!-- dydo's doctrine here is DR 067 (What Tests Are For) with its two accepted amendments, in the dydo repository. -->

# Tests

This is the one home for testing in dydo: what a good test is, which tests you write, which you keep, and how you show that a kept test can fail. Every section applies whenever you touch a test, while you write it and while you review it.

When exploring the codebase, from the repository root read `dydo/glossary.md` so test names match the project's domain language, and respect the decision records in the area you touch. From the repository root, read `dydo/guides/testing-strategy.md` for the project's runner, its commands and which tests run at which stage.

## Two kinds of test

- A **proof test** decides whether your change works. Write as many as the work needs: they are how you see the change working. They live in your ticket's commits.
- A **suite test** earns a lasting place. Few proof tests graduate into the suite. The rest leave before the merge, and the commit SHA your ticket records is their archive.

A test already in the suite meets the same bar, however old it is. A test of behaviour the product keeps stays when it meets this skill. When it carries a real protection but falls short, rewrite it until it meets the skill. A test of behaviour the product drops is deleted.

Tests are one rung of proof. Statics and lint own what can be said about the code's shape, driving the app owns the user's journeys, and review owns intent. A test does what those cannot.

## What a good test is

A good test verifies behaviour through a public interface, never through internals. It reads like a specification: "checkout confirms a valid cart" says what capability exists, and it survives any refactor that keeps that behaviour.

**Boundary first.** Drive the deepest module boundary, or the product boundary, that can observe the behaviour. The product boundary is where a user or agent meets the product: a command, an endpoint, a hook, a shipped entry point. One boundary test that covers a behaviour beats a thousand unit tests that each cover a step. Write a unit test for an edge or trap the boundary cannot reach cheaply.

**Assert the outcome.** Assert what a person or agent sees: the exit code, the output, the file or git state. A test that asserts each step breaks when the steps change and the outcome does not.

**Expected values come from outside the code.** Take them from the spec, a worked example, or a known-good literal, never by recomputing them the way the code does.

**Mock at system boundaries only**: external services, time, randomness, and sometimes the file system. Your own modules run for real. See [mocking](resources/mocking.md).

## Seams

A **seam** is the public boundary you test at, where you observe behaviour without reaching inside. You choose the seams: spend the testing effort on the critical paths and the complex logic, not on every edge case. You never wait for the user to approve them; the user steps in when they want a say. A seam named on your ticket, with one line on what it catches and what it misses, lets your reviewer judge the choice.

When the interface's shape is itself in question (how deep the module is, where the seam belongs, what it exposes), load `codebase-design` for the vocabulary.

## A suite test does one job

A test earns its place in the suite by one of three jobs:

- **trap**: it guards a real bug, or a pitfall no lint rule can express. Name the bug or the pitfall.
- **contract**: it pins a rule a spec or decision record states. Cite it. Behaviour pinned only because the code does it today is a finding, not a contract.
- **edge**: it reaches a case that driving the app cannot. A property test, one that checks a rule across generated inputs, is a strong way to reach edges.

Tag each graduating test with its job and its cite, in the project's tag syntax; the [worked example](resources/worked-example.md) shows dydo's. A tag must be true of every case it covers, and the reviewer checks it.

A trap that can be stated about the code's syntax becomes a lint rule instead. Design feedback, ticket proof and a gate number are real work, and none of them earns a place.

## The red check

Every test you propose for the suite is shown failing without the code it guards:

1. Run it green on your change.
2. Break or remove the code it guards, and run it again. It fails, and it fails for the reason its job names.
3. Restore the code, and run it green again.

Record the command and what each run printed on your ticket. The [worked example](resources/worked-example.md) runs the whole check on dydo's runner.

The order you work in is yours. The red check is what makes a kept test count, whatever the order. One strong way to work is in **vertical slices**: one test, then the code that passes it, each test a **tracer bullet** that answers to what the last one taught you. Tests written in a batch ahead of the code tend to test the code's imagined shape; tracer bullets test what it turned out to do.

**A bug starts red.** Before you change the code, have a test that fails because of the bug, at the boundary where the bug shows; a test that is already red on the bug counts. Fix the bug and watch it go green. Then revert the fix once, watch the test fail again, and restore the fix. When the bug resists a reproduction, load `diagnosing-bugs`. For a race, fake timers, a forced interleaving or a narrowed timing window are options for making the reproduction fail reliably.

## Smells

Each of these is a finding at review, and a reason to rewrite or delete a test you meet:

- **Changes whenever the code does.** It mocks your own collaborators, calls private methods, asserts call order, or checks through a side channel instead of the interface. The tell: it breaks on a refactor that keeps the behaviour.
- **Can never fail.** It asserts nothing the code under test controls, such as an object compared with itself or a value the test set up and never handed to the code. No plausible edit to the code can turn it red.
- **Restates the code.** Its expected value is computed the way the code computes it, so the test agrees with the code even when both are wrong (`expected = PriceMath.Sum(items)`, when `Total` itself calls `PriceMath.Sum`).
- **Checks that a deleted feature is gone.** Delete it with the feature, unless the absence is itself a contract, such as a removed security route that must stay removed.
- **Pins untraced behaviour.** It asserts what the code does today, with no spec or decision record behind it. A green test can cement behaviour nobody wants.
- **Redundant.** Kept tests already fail on everything it would catch.

## In review

Read the tests as part of the change. Check that each suite test does one job and that its tag is true, that it drives the deepest boundary that observes its behaviour, that its red check is recorded, and that no test shows a smell.

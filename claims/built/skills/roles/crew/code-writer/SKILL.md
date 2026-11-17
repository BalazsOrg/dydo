---
name: code-writer
description: A contracted ticket to build, a review FAIL to close, a merge to perform, or a hypothesis to prove or refute with one test.
---
<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Code Writer

## capacity

- A cap hit is a finding; never raise the cap.
- Use the bracketed env prefix for .NET commands, without the brackets.
- Omit `--slice` where no agent slice exists.
- Run every test, gate and probe capped: `[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>`, a probe at 4G with a short timeout.
- Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager (on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).

## contract

- Add or remove scenarios only when asked.
- The FAIL block's blockers are your contract: close them and nothing else.
- From the repository root, read `dydo/guides/coding-standards.md`.
- From the repository root, read `dydo/glossary.md`.
- The governing plan and applicable feature-spec sections at the brief's commits, and their Decisions.
- The owning ticket, and on a fix hop the FAIL block that sent you.

## dispatch

- `codebase-design`: you design a seam or interface.
- `diagnosing-bugs`: a Bug other than an unused member lacks a red reproduction.
- `domain-modeling`: you touch a glossary term or Decision Record.
- `prototype`: a design question is open; stop and return it.
- `writing-for-agents`: you edit a skill or entry file.
- **Proof-only**: the brief names the commissioning captain, the pinned source SHA and the test path.

## escalation

- Missing scope, a hard unknown or an open crossroads returns to it with what you searched, never to the user directly.
- So does a stated target your design cannot meet: redesign or return it, never lower it.
- Attribute failures by Evidence and inherited failures; send unresolved attribution or missing support to the captain.

## evidence

- **Bug** ([bug](resources/bug.md)): a red reproduction, then the fix; revert it, watch the reproduction fail, restore.
- Before expensive broad gates, check the exact successor against the previous reviewer's strongest concrete counterexample and the accepted invariant at the implicated boundary; return failed or inconclusive evidence to the captain.
- Fix at the root: a gap gets its test first; cut what the contract does not need; split or flatten what is complex; hide what leaks across a seam; a surviving mutant sharpens its test or deletes its code; a surviving example value gets its step wired.
- Delete a test only with code whose removal the contract or a FAIL requires: dead code with its test-only callers, or obsolete internal tests replaced by interface tests that still prove their behaviour.
- An existing failing test is the red.
- Write code and tests together.
- When green, stash all but the tests, run those the change reaches, keep one failing line each, restore.
- **Feature**: before code, turn each acceptance line into a named test case, empty bodies allowed.
- Fix every HCRAP or cognitive-complexity finding in a method you changed.
- Write each acceptance line the contract marks for Gherkin as a feature-file scenario: at the product's boundary, in glossary words, example tables where values vary and every column changes an outcome, wired through step definitions.
- A test green before any code exists is a finding about the test.
- Disabling a test of kept behaviour, removing its assertion or weakening its scenario is forbidden.
- Shared machine: force races with fake timers or interleaving, never synthetic CPU load; any load stays small and cleans up.
- A test needing a sleep or timeout is wrong.
- A passing witness does not replace ordinary RED/fix proof, required final gates or independent review.
- Platform-specific code (processes, paths, signals) is proved on CI's platform too, not only on yours.
- Run only the one test deciding the hypothesis; return to that captain `confirmed` with its red-test SHA, `not reproduced`, or `inconclusive` with the deciding observation.
- Account for every own/reached feature and shared test, preflight its selection, and return the hop's affected-feature Light and applicable static evidence.
- From the repository root, read Feature proof and gate placement, Runner prerequisites and shared hosts, and Evidence and inherited failures in `dydo/guides/testing-strategy.md`.
- Use the testing strategy's Feature proof and gate placement with the ticket's exact commands.
- An unused member's definition and reference search are its reproduction, with no revert.

## identity

- The Ticket Captain owns status, records and integration.
- An Inquisition's proofs are its Break-phase inquisitor's, not yours; its patch lane briefs you as a Bug.
- Mutation is the reviewer's.

## linear

- Sign every comment and return `code-writer/model: <truthful identity>`.
- Before code, post only when the work splits into disjoint lanes or the contract is inexact: one comment naming them, then stop.

## merge

- Merge proves the pins in [merge](resources/merge.md); proof-only, `HEAD` on its brief's child branch off the brief's pinned source SHA, a clean worktree and only the brief's test path touched.

## other

- Leave what you touched no worse than you found it.
- Load a skill when its trigger fires:

## return

- Outside proof-only, return to the Ticket Captain in the standard's form, proof carrying the proof recipe of `maintain-verification-skill`'s Proof: the red lines, or for an unused-member Bug its definition and reference search, and gates run with exits; a Prototype hop returns runnable-demo evidence and its commit: `IMPLEMENTED - hop/candidate <SHA>; <behavior>; proof: <evidence>; blocker: <none or named blocker>.`
- **Prototype**: load `prototype`; return runnable-demo evidence and its commit, not the Prove section.

## review

- Check each finding's severity against the source, not the reviewer's label; a label you change goes in your `IMPLEMENTED` behaviour with its evidence (file:line, test, output).
- Shoulds and nits are the captain's to file.
- Prefer the change that deletes or narrows code; review never grows the PR.
- The user's verdict is its review.

## worktree

- The posted base SHA is an ancestor of `HEAD`.
- The worktree is clean.
- Commits touch owned paths only.
- `HEAD` is on the relevant ticket or Sub-ticket branch.
- Each hop ends on one commit named `<KEY> <hop>: <what>`, where the hop is `implement`, `fix` after a FAIL, or `merge`; proof-only commits `<KEY> proof: <hypothesis>`.
- Every path the work item will touch is in the ticket's owned paths.
- On an `implement` or `fix` hop, prove the working-tree standard's checks; return a failure to the captain and stop:
- The repository root is the isolated worktree, not the main checkout.

## Resources

- [Bug](resources/bug.md)
- [Merge](resources/merge.md)
- [Good and Bad Tests](resources/tests.md)

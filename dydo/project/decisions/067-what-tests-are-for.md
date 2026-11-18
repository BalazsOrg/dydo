---
area: project
type: decision
status: accepted
date: 2026-11-17
applies: [dydo-aligned-intent, dydo-verification-independent-evidence, dydo-efficiency, dydo-design-outcomes-over-methods]
---

# 067 - What Tests Are For

A suite test earns its place by one of three jobs: it guards a trap, pins a traced contract, or
reaches an edge that driving can't. A ticket's proof test stays in the suite only if it graduates,
tagged with its job. A red check replaces test-first ordering, and a tag lint replaces DR 048's
"every non-trivial module has a test file". Decided by Balazs in grilling G on 2026-11-16/17, rules
T1-T8 of the
[DYD-971 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-971#comment-022a5db3-9f53-4ab1-aec8-d88569caab56).
Recorded under [DYD-1137](https://linear.app/bodnar-balazs/issue/DYD-1137);
[approved by Balazs on 2026-11-17](https://linear.app/bodnar-balazs/issue/DYD-1137#comment-d9968c1b-2753-4920-bb5a-39e141e57621)
(F10).

---

## Context

Grilling G asked what a test is for. His starting point: "having tests green can actually cement
incorrect behavior because the test itself asserts a truth or a reality that we do not really want."
And: "the best performance improvement for tests is probably the tests which we never write."

Until now a suite test needed no job. [DR 048](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md)
§3's Tests row asked that every non-trivial module have a test file, and code-writer required tests
before code. The gate set that measures tests is DR 068's; the file token limit is DR 069's. Both
come from the same grilling.

## Decision

1. **Three jobs (T1).** A suite test does one of three jobs: it guards a trap (a real bug, or a
   pitfall no lint rule can express), pins a traced contract (spec or DR), or reaches an edge that
   driving can't. Design feedback and ticket proof are real work, but they don't earn a place in the
   suite. A gate number never does. Pinning current behaviour counts only when it traces to the
   spec or a DR; an untraced pin is a finding, not proof
   ([DYD-1120](https://linear.app/bodnar-balazs/issue/DYD-1120)).
2. **Layers, pstack's inverted pyramid (T2).** Statics and lint own anything that can be stated
   about the code's shape. Graduated tests own traps, contracts and edges. Driving owns the journeys,
   against the spec. Review owns intent, against the spec (DYD-1120). Mutation testing owns "do the
   tests catch bugs".
   - **Lint first.** A trap that can be stated syntactically becomes a lint rule, not a test: "lint
     is cheaper and quicker."
   - Driving, integration tests and end-to-end tests are complementary, with different uses. Driving
     is already on every proof's ladder (`maintain-verification-skill`'s Proof).
3. **The red check replaces TDD ordering (T3).** Code-writer no longer requires tests before code.
   Every test proposed for graduation must be shown failing without the code. The Bug rule stays:
   red reproduction first, then revert and watch it fail.
4. **Boundary first (T4).** A graduated test drives the deepest module boundary, or the product
   boundary, that can observe the behaviour. A unit test graduates only for an edge or trap the
   boundary test can't reach cheaply. His words: "the idea of one 10ms green check amounting to
   1000*1ms green checks is a really powerful one." "naming this is already half the road to
   success." On mutation, it "changes product code so that doesn't depend on the number of tests
   being written."
5. **Graduation (T5, option b1).**
   - Proof tests live in the ticket's commits, and the merge removes the ones that don't graduate.
     The receipt and `proof:` record the commit SHA, so git is the archive.
   - The author tags each graduating test with its job: trap (naming the bug or pitfall), contract
     (citing the spec or DR) or edge. The reviewer checks each tag.
   - A tag lint, "every suite test carries a job tag", replaces DR 048's "every non-trivial module
     has a test file" rule.
   - Old evidence is never load-bearing: "No one is going to check an evidence of a ticket from 6
     months ago… they're not getting checked ever again after they had been for once." A squash or a
     fresh start must break nothing.
6. **Refactor safety (T6).** The everyday net comes from T4 and traced contracts. A rewrite or wide
   refactor gets a conformance suite as a **separate rule in a separate place**: "if purpose b is
   near where a is defined then we have a problem, because agents might think that if it's for the
   refactor only then it's fine to only write the tests then. No. It's not fine." The conformance
   mechanics (traced assertions graduate; "matches old implementation" assertions are removed at the
   rewrite's landing) need no specific home: rewrites are rare and involve him.
7. **Named techniques, as guidance, not a gate (T7).** Property tests for edges (Hypothesis,
   fast-check, FsCheck). Differential tests for rewrites, with the old system as the oracle and an
   approved-changes list. "naming thing is having power of them." Input:
   [How the Claude Code team thinks about verification](https://claude.dev/blog/how-the-claude-code-team-thinks-about-verification/).
8. **Pruning (T8).** Existing tests are grandfathered now. The tag lint applies to new and changed
   tests; a touched old test is tagged or deleted. Once DYTEST Mutation lands, each feature gets a
   pruning ticket: an untagged test that kills no mutant the other tests don't already kill is
   deleted. The evidence decides, an agent proposes, the reviewer checks, and he sees the totals per
   feature. "we should gradually have the tests pruned and revisited."

## Consequences

- **Hard to reverse:** the DYTEST suite gets pruned (grilling G's DR test).
- Proof that doesn't graduate stays reachable only through the commit SHA its receipt records.
- The doctrine T1-T8 ships in the skills; an adopting project changes it in its own copy of dydo
  (D2, recorded in DR 068).

## Amendment 2026-11-18 - three test smells to prune (proposed)

**Proposed, pending Balazs's approval of its wording (F10).** Until he approves it, this section
binds nothing and T1-T8 stand as written. Drafted under
[DYD-1183](https://linear.app/bodnar-balazs/issue/DYD-1183) from his words of 2026-11-18, after he
saw a public `/prune-tests` tool that finds and removes tests that "change whenever the code does",
"check deleted features are gone" and "can never fail". His words: "I want to make sure we have
these ideas incorporated (I don't think we explicitly have stated 'change when the code changes' ->
that kind of follows from testing at the boundary level (deep modules) and remove tests that assert
that something is gone and can never fail these are all good ones and something I have observed
myself."

The amendment names three smells. Each is a finding at review, and each is a pruning target under
T8.

1. **Changes whenever the code does.** The test is coupled to the implementation, not to observable
   behaviour. The smell follows from T4 (boundary first): a test that drives the deepest boundary
   that can observe the behaviour survives refactors. Signal: the test is edited in most commits
   that edit its subject without changing behaviour.
2. **Checks that a deleted feature is gone.** The test does none of T1's three jobs: it guards no
   trap, pins no traced contract and reaches no edge. Delete it with the feature, unless the absence
   is itself a traced contract, such as a removed security route that must stay removed. Then the
   test is tagged `contract` and names its source (T5).
3. **Can never fail.** The test is vacuous: it asserts nothing that the code under test controls.
   T3's red check stops it at graduation. Among existing tests, mutation finds it
   ([DR 068](./068-gate-set-mutation-over-coverage-changed-files-only.md) rule 4: no surviving
   mutant in changed code). Until DYTEST Mutation lands, a reviewer flags an assertion that no
   plausible edit could break.

Once he approves it, the step-4 claims
([DYD-1083](https://linear.app/bodnar-balazs/issue/DYD-1083)) carry the three smells into
code-writer (don't write them), reviewer (flag them) and the testing guide (T8's pruning criteria),
and DYTEST Mutation's pruning evidence (T8) takes them as its first heuristics. T1-T8's text is kept
as written; this amendment adds to it.

---

## Supersedes and amends

Partially supersedes [DR 048](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md) §3,
for the Tests row's "every non-trivial module has a test file" only: the T5 tag lint replaces it.
DR 048 stays `accepted` otherwise.

## Related

- [DR 048 - One-Level Static Gates](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md)
- [DYD-1120](https://linear.app/bodnar-balazs/issue/DYD-1120) - Untraced pins are findings; review against the spec
- [DYD-971](https://linear.app/bodnar-balazs/issue/DYD-971) - Grilling G
- [DYD-1183](https://linear.app/bodnar-balazs/issue/DYD-1183) - The proposed 2026-11-18 amendment: three test smells to prune

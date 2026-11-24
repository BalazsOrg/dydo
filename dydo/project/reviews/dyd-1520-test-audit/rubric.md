---
area: project
type: reference
---

# DYD-1520 Test Rubric

This page says what a good dydo test is and what makes a test bad. The
[DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520) audit sorts every test in
`DynaDocs.Tests/` against it, and the removal and rewrite batches follow it. It is short on purpose:
"Less rules and constraints, but better ones"
([DYD-1505](https://linear.app/bodnar-balazs/issue/DYD-1505#comment-2ae771ff-fd80-4b0e-94ed-d345cc6c5c24)).

## Sources

| Cite | Where it is recorded |
|---|---|
| T1-T8, G1-G6 | [Grilling G](https://linear.app/bodnar-balazs/document/400-grilling-g-what-tests-are-for-the-gate-set-the-file-token-limit-8efeb2de16b6), recorded with Balazs's words verbatim in the [DYD-971 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-971#comment-022a5db3-9f53-4ab1-aec8-d88569caab56) comment |
| DR 067, smells 1-3, purge | [DR 067](../../decisions/067-what-tests-are-for.md) (T1-T8) and its two accepted 2026-11-18 amendments: the three smells, and the purge that replaces T8 |
| DR 068 | [DR 068](../../decisions/068-gate-set-mutation-over-coverage-changed-files-only.md) (G1-G6, D1-D2) |
| DYD-1199 | Balazs's purge ruling, [DYD-1199](https://linear.app/bodnar-balazs/issue/DYD-1199) |
| Contract 3 | [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520), contract item 3, which binds every removal under this ticket |

Where `testing-strategy.md`, `coding-standards.md` or the glossary disagree, grilling G wins
([§7](#7-where-the-older-guides-disagree)).

## 1. The unit and the verdicts

- **A test is one executed case:** a `[Fact]`; each row of a `[Theory]`; a Gherkin scenario, and each
  Examples row of a Scenario Outline; a Python `test_*`; a Node `test()` call.
- **The audit records one line per test method or scenario,** with its row count. Where rows of one
  method earn different verdicts, it gives a verdict per row.
- **The verdicts are keep, rewrite and remove.** "measure them against the new test requirements and
  either keep them if they match or rewrite them to match. And have the rest deleted" (purge).
- **Helpers and fixtures are not tests.** A helper or fixture that a removal leaves with no caller is
  deleted with it: dead code is a static finding (G3).

## 2. A good test (keep)

A test is kept only when it meets every row.

| # | It | Source |
|---|---|---|
| 1 | **Does one T1 job.** A trap names the real bug or pitfall it guards. A contract cites the spec or DR it pins. An edge reaches a case driving can't. | T1 |
| 2 | **Carries a true job tag** that names that job and its cite. C#: `[Trait("Job", "<trap\|contract\|edge>: <cite>")]`, on the class only when it is true of every test in the class. Python: a `Job: <kind>: <cite>` line in the docstring. Gherkin: a `# Job:` comment heading the feature file. Node: a `// Job:` comment above the `test()` call. "The reviewer checks each tag." | T5 |
| 3 | **Drives the deepest boundary that observes the behaviour:** the `dydo` CLI or the guard hook entry first. A unit test stays only for an edge or trap that boundary can't reach cheaply. "the idea of one 10ms green check amounting to 1000*1ms green checks is a really powerful one." | T4 |
| 4 | **Asserts the outcome a person or agent sees, not the steps.** It "should test the behavior in an end-to-end integration test way. So not assert the the at each step but rather the outcome so if the code changes the test shouldn't need to change they constrain the outcome not the way to get there". | DYD-1199; smell 1 |
| 5 | **Can fail.** It asserts something the code under test controls, and it goes red when that code is removed or broken. | T3; smell 3 |
| 6 | **Guards behaviour 4.0.0 keeps.** "I only want the tests to remain which test something that is going to stay. For example, state chart, guard behavior". | purge; DYD-1199 |
| 7 | **Is not redundant.** No other kept test already fails on the same planted bug or mutant. | T4; contract 3 |

## 3. A bad test

A test that shows any of these is removed, or rewritten under [§4](#4-rewrite-not-remove).
Removal still needs the evidence in [§5](#5-the-evidence-a-removal-needs).

| Sign | Why it is bad | Source |
|---|---|---|
| **No T1 job.** It exists for a gate number, design feedback, or ticket proof that never graduated. | "Design feedback and ticket proof are real work, but they don't earn a place in the suite. A gate number never does." Coverage is a report, not a gate: "one of the big mistakes I made is that I set a coverage gate." Old evidence is never load-bearing: "No one is going to check an evidence of a ticket from 6 months ago". | T1; G1; T5 |
| **An untraced pin of current behaviour.** It asserts what the code does today, with no spec, DR or named trap behind it. | "an untraced pin is a finding, not proof" ([DYD-1120](https://linear.app/bodnar-balazs/issue/DYD-1120)). "having tests green can actually cement incorrect behavior because the test itself asserts a truth or a reality that we do not really want." | T1 |
| **Changes whenever the code does.** It is coupled to the implementation, not to what is observed. Signal: it is edited in most commits that edit its subject without changing behaviour. | A test at the deepest observing boundary survives refactors. | smell 1; T4 |
| **Checks that a deleted feature is gone.** | It does none of T1's jobs, unless the absence is itself a traced contract (a removed security route that must stay removed), tagged `contract` with its source. | smell 2 |
| **Can never fail.** It asserts nothing the code under test controls. | Vacuous. "a reviewer flags an assertion that no plausible edit could break." | smell 3; T3 |
| **Redundant.** Another kept test fails on the same planted bug or mutant. | It adds run time and no protection. "the best performance improvement for tests is probably the tests which we never write." | T4; T1 |
| **Covers nothing a user could hit.** No entry point a person or agent uses reaches the path it covers. | It protects no behaviour. | contract 3 |
| **States a trap a lint rule could state.** | "a trap that can be stated syntactically becomes a lint rule, not a test", because "lint is cheaper and quicker." | T2 |
| **Tests behaviour 4.0.0 drops.** | "All the tests about a certain roles and these kind of things must go." | purge; DYD-1199 |

## 4. Rewrite, not remove

- **Rewrite when the behaviour has a real job but the test fails rows 2-5 of §2:** the wrong
  boundary, steps instead of an outcome, a missing or false tag, or no way to fail.
- **A rewrite** drives the boundary (row 3), asserts the outcome (row 4) and carries a true tag
  (row 2). It is shown red: it fails without the code it guards, before it counts as kept (T3).
- **Collapse rows.** Many rows in one equivalence class (the same branch to the same outcome) collapse
  to one row per distinct behaviour (T4).
- **Property tests** are named guidance for edges, not a requirement: "naming thing is having power
  of them" (T7).
- **A lint-shaped trap** (§3) is removed only when a lint rule already states it, and the rule's name
  is its evidence. Until one exists, keep the test and record it as a lint candidate (T2; contract 3).

## 5. The evidence a removal needs

Contract 3 binds every removal: "A batch removes no protection that is real. Each removed test is
either shown to be redundant (another kept test fails on the same planted bug or the same broken
behaviour), or covers nothing a user could hit. Mutation on the batch's code, where a tool is
available, is the evidence. Where none is available, say so per batch."

- **Redundant.** Name the kept test that fails on the same planted bug or mutant. Where a mutation
  tool runs on the batch's production code, the evidence is that run before and after the removal:
  no mutant the suite killed before survives after. Where none runs, plant the bug by hand, show
  the kept test red, and record "mutation unavailable" for the batch. Unavailable is never a pass
  (G4; DR 068 rule 4).
- **Unreachable.** Name why no entry point (a `dydo` command, the guard hook, a shipped script)
  reaches the path the test covers.
- **Dropped behaviour.** Cite the record that drops it: the spec, a DR or the ticket. While the
  dropped code still ships, a user can still hit it, so contract 3 still asks for redundant or
  unreachable evidence. Without it, the test stays until that code goes.
- **No job, but the only guard.** A test with no T1 job that is still the only thing catching a mutant
  or planted bug in user-reachable code is not removed under this ticket. It is rewritten with a
  real job: a trace to the spec or a DR, or a named trap. Where no job can be found, it is kept and
  listed as "kept, unsure" for the captain's final report.

Why mutation is the evidence: "Mutation testing owns 'do the tests catch bugs'" (T2). It "changes
product code so that doesn't depend on the number of tests being written" (T4). It is the
test-quality gate, "no surviving mutant in changed code" (G4; DR 068 rule 4). T8's original pruning
rule, which the purge replaces but DR 067 keeps as history, used the same evidence: "an untagged
test that kills no mutant the other tests don't already kill is deleted."

## 6. The record line

Each audit file in this folder uses one table, one line per test method or scenario.

| Test | Rows | Verdict | Reason | Evidence |
|---|---|---|---|---|
| `<Class>.<Method>` or `<file>.feature: <scenario>` | `1`, or `n` with the rows that differ | keep, rewrite, remove, or kept, unsure | One line: the §2 row it meets or fails, or the §3 sign it shows | keep: its tag. rewrite: the job it will carry. remove: `kept: <test id>`, `mutant: <id>`, `planted: <bug>` or `unreachable: <why>` |

## 7. Where the older guides disagree

Grilling G wins in each case. Rewriting these files is out of scope for DYD-1520. The lines are as
of `4fff1ad3`.

- **[Testing Strategy](../../../guides/testing-strategy.md)**, opening paragraph and "Feature proof
  and gate placement": per-ticket coverage gates the ticket's changed files "under the unchanged
  thresholds". Whole-stack coverage "is required at the landing, at every sync of the default branch
  into a feature branch and in hosted CI". G1: coverage is a report, not a gate. G5: the
  whole-tree run is a report.
- **Testing Strategy**, the DR 048 policy paragraph and "Python platform coverage" / ".NET platform
  coverage": line coverage 80%, branch coverage 60% and HCRAP 20 as gates. G1 drops the floors;
  G2 drops HCRAP.
- **[Coding Standards](../../../guides/coding-standards.md)**, the scenario/test/gate list: "A
  **test** claims one seam inside, in the code's words. It comes and goes with refactors." T4 asks
  for the deepest observing boundary, and smell 1 makes a test that changes with refactors a finding.
- **Coding Standards**, same list: a gate proves "the coverage bar". G1.
- **Coding Standards**, the assurance-policy paragraph: per-module line coverage of 80% and branch
  coverage of 60%, and per-method HCRAP of at most 20. G1, G2.
- **[Glossary](../../../glossary.md)**, HCRAP: "The one threshold is 20." G2: "let's drop it".

## Related

- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - The audit files that apply this rubric
- [DR 067 - What Tests Are For](../../decisions/067-what-tests-are-for.md) - T1-T8, the three smells, the purge
- [DR 068 - The Gate Set](../../decisions/068-gate-set-mutation-over-coverage-changed-files-only.md) - G1-G6: mutation over coverage

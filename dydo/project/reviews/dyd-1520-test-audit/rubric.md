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
| DYD-971 2026-11-12 | Balazs's 2026-11-12 paragraph in the [DYD-971](https://linear.app/bodnar-balazs/issue/DYD-971) description |
| DR 067, smells 1-3, purge | [DR 067](../../decisions/067-what-tests-are-for.md) (T1-T8) and its two accepted 2026-11-18 amendments: the three smells, and the purge that replaces T8 |
| DR 068 | [DR 068](../../decisions/068-gate-set-mutation-over-coverage-changed-files-only.md) (G1-G6, D1-D2) |
| DYD-1199 | Balazs's purge ruling, [DYD-1199](https://linear.app/bodnar-balazs/issue/DYD-1199) |
| Contract 3 | [DYD-1520](https://linear.app/bodnar-balazs/issue/DYD-1520), contract item 3, which binds every removal under this ticket |

Where `testing-strategy.md`, `coding-standards.md` or the glossary disagree, grilling G wins
([§7](#7-where-the-older-guides-disagree)).

## 1. The unit and the verdicts

- **A test is one case, and each case gets one record line ([§6](#6-the-record-line)):** each C#
  `[Fact]` and `[SkippableFact]`; each data row of a `[Theory]` or `[SkippableTheory]`; each Gherkin
  scenario, and in place of a Scenario Outline each of its Examples rows; each Python `test_*` method
  that unittest discovers; each `test()` registration under `node --test`, so a `test()` called in a
  loop over three inputs is three cases. A case whose assertions run only under a condition (a
  platform, a skip condition, an environment variable) is still one case, and its line records it.
- **The verdicts are keep, rewrite and remove.** "measure them against the new test requirements and
  either keep them if they match or rewrite them to match. And have the rest deleted" (purge). The
  purge replaces T8's grandfathering: no test is exempt for already existing, and the purge does not
  wait for DYTEST Mutation.
- **Each case takes the first verdict that applies:**
  1. **Remove** when it shows a §3 sign and [§5](#5-the-evidence-a-removal-needs) shows every
     protection it carries covered (contract 3).
  2. **Rewrite** when its behaviour stays in 4.0.0 (§2 row 6) and has a T1 job the case can do
     (row 1), and the case fails a §2 row from 2 to 5 ([§4](#4-rewrite-not-remove); purge).
  3. **Keep** when it meets every §2 row. Otherwise it is kept with the reason `unsure`, because it
     carries a protection §5 does not show covered and no rewrite applies (contract 3); the
     captain's final report lists these.
- **Helpers and fixtures are not tests.** A helper or fixture that a removal leaves with no caller is
  deleted with it: dead code is a static finding (G3).

## 2. A good test (keep)

A good test meets every row.

| # | It | Source |
|---|---|---|
| 1 | **Does one T1 job.** A trap names the real bug, or the pitfall no lint rule can express, that it guards. A contract cites the spec or DR it pins; pinning current behaviour counts only with that cite. An edge reaches a case driving can't. Walking a journey is not a job of its own: driving owns journeys and review owns intent, both against the spec. | T1; T2 |
| 2 | **Carries a true job tag** that names that job and its cite. The syntax is this repository's convention; no lint enforces it yet. C#: `[Trait("Job", "<trap\|contract\|edge>: <cite>")]`. Python: a `Job: <kind>: <cite>` line in the docstring. Gherkin: a `# Job:` comment heading the feature file. Node: a `// Job:` comment above the `test()` call, or above the loop that registers it. A tag is true only when it is true of every case it covers. "The reviewer checks each tag." The tag replaces DR 048's "every non-trivial module has a test file": a test is never kept to give a module a test file. | T5 |
| 3 | **Drives the deepest module boundary, or the product boundary, that can observe the behaviour.** The product boundary is a `dydo` command, the guard hook entry or a shipped script entry point. A unit test stays only for an edge or trap that boundary can't reach cheaply. "the idea of one 10ms green check amounting to 1000*1ms green checks is a really powerful one." | T4 |
| 4 | **Asserts the outcome a person or agent sees, not the steps.** It "should test the behavior in an end-to-end integration test way. So not assert the the at each step but rather the outcome so if the code changes the test shouldn't need to change they constrain the outcome not the way to get there". | DYD-1199; smell 1 |
| 5 | **Can fail.** It asserts something the code under test controls, and it goes red when that code is removed or broken. | T3; smell 3 |
| 6 | **Guards behaviour 4.0.0 keeps.** "I only want the tests to remain which test something that is going to stay. For example, state chart, guard behavior". | purge; DYD-1199 |
| 7 | **Is not redundant:** §5 does not show kept tests covering every protection it carries. | T4; contract 3 |

## 3. A bad test

Each sign is a lead, not a verdict; §1's order decides.

| Sign | Why it is bad | Source |
|---|---|---|
| **No T1 job.** It exists for a gate number, design feedback, or ticket proof that never graduated. | "Design feedback and ticket proof are real work, but they don't earn a place in the suite. A gate number never does." Coverage is a report, not a gate: "one of the big mistakes I made is that I set a coverage gate." Old evidence is never load-bearing: "No one is going to check an evidence of a ticket from 6 months ago". | T1; G1; T5 |
| **An untraced pin of current behaviour.** It asserts what the code does today, with no spec or DR behind it. | "an untraced pin is a finding, not proof" ([DYD-1120](https://linear.app/bodnar-balazs/issue/DYD-1120)). "having tests green can actually cement incorrect behavior because the test itself asserts a truth or a reality that we do not really want." | T1 |
| **Changes whenever the code does.** It is coupled to the implementation, not to what is observed. Signal: it is edited in most commits that edit its subject without changing behaviour. | A test at the deepest observing boundary survives refactors. | smell 1; T4 |
| **Checks that a deleted feature is gone.** | It does none of T1's jobs, unless the absence is itself a traced contract (a removed security route that must stay removed), tagged `contract` with its source. | smell 2 |
| **Can never fail.** It asserts nothing the code under test controls. | Vacuous. "a reviewer flags an assertion that no plausible edit could break." | smell 3; T3 |
| **Redundant.** [§5](#5-the-evidence-a-removal-needs) shows kept tests covering every protection it carries. Rows of one equivalence class (the same branch to the same outcome) are the usual lead; collapsing keeps one row per distinct behaviour, the first by its §6 ordinal. | It adds run time and no protection. "the best performance improvement for tests is probably the tests which we never write." | T4; T1 |
| **Covers nothing a user could hit.** No entry point a person or agent uses reaches the path it covers. | It protects no behaviour. | contract 3 |
| **States a trap a lint rule could state.** | "a trap that can be stated syntactically becomes a lint rule, not a test", because "lint is cheaper and quicker." | T2 |
| **Tests behaviour 4.0.0 drops.** | "All the tests about a certain roles and these kind of things must go." | purge; DYD-1199 |

## 4. Rewrite, not remove

- **A rewrite**, step 2 of §1's order, drives the boundary (row 3), asserts the outcome (row 4) and
  carries a true tag (row 2). It graduates like a new test: its red check is observed, failing
  without the code it guards, before it counts as kept. A trap for a bug keeps the Bug rule: red
  reproduction first, then revert the fix and watch it fail (T3). An assertion it drops needs §5's
  evidence.
- **Named techniques, as guidance, not a requirement:** property tests for edges; differential tests
  for rewrites, with the old implementation as the oracle and an approved-changes list. "naming
  thing is having power of them" (T7).
- **A lint-shaped trap** (§3) has no suite job, so it is never rewritten; it is recorded as a lint
  candidate. A lint rule is not a kept test, and naming one proves neither redundancy nor
  unreachability, so the test stays until §5 evidence covers it (T2; contract 3).

## 5. The evidence a removal needs

Contract 3 binds every removal: "A batch removes no protection that is real. Each removed test is
either shown to be redundant (another kept test fails on the same planted bug or the same broken
behaviour), or covers nothing a user could hit. Mutation on the batch's code, where a tool is
available, is the evidence. Where none is available, say so per batch."

The evidence covers each protection the batch drops: every removed case and every assertion a
rewrite drops. A protection is what a dropped assertion fails on: its observable (an exit code,
output, file or git state), for an input of its equivalence class (§3), wherever its case runs
(§1). A kept test covers it only when it asserts that observable for that class at least as
strictly, exact where the dropped one was exact, and runs there too. An exact exit code `0` and a
kept test that rejects only `1` both kill a `0 → 1` mutant, but only the exact one fails on `2`, so
the looser test does not cover it.

- **Mutation, where a tool runs on the batch's code** (dydo's C# product code). The batch's tests
  run under mutation at base and again on the candidate: every mutant killed at base is still killed
  on the candidate, or is shown unreachable. A shared kill proves overlap for that mutant's failure
  only.
- **A planted bug, where mutation leaves a protection unproved:** no killed mutant breaks the dropped
  assertion's observable, the run did not cover a condition its case runs under, or no tool runs at
  all (the Python and Node tooling tests, whose batch records "mutation unavailable"). A planted bug
  or demonstrated broken behaviour that the dropped assertion fails on must turn a named kept test
  red, or the assertion stays.
- **Unreachable.** Name why no entry point (a `dydo` command, the guard hook, a shipped script)
  reaches the path the test covers.
- **Dropped behaviour.** Cite the record that drops it: the spec, a DR or the ticket. While the
  dropped code still ships, a user can still hit it, so contract 3 still asks for the evidence
  above. Without it, the test stays until that code goes.

Why mutation is the evidence: "what does that say about the quality of the tests? Nothing
(mutation testing is for that)" (DYD-971 2026-11-12). "Mutation testing owns 'do the tests catch
bugs'" (T2). It "changes product code so that doesn't depend on the number of tests being written"
(T4). T8's original pruning rule, which the purge replaces but DR 067 keeps as history, also
rested on mutation: "an untagged test that kills no mutant the other tests don't already kill is
deleted."

This before/after evidence is DYD-1520's own. G4's test-quality gate is separate and later, out of
this ticket's scope: no surviving mutant in changed code at the final candidate, with equivalent
mutants approved one at a time, each with a reason, never by a threshold. Until DYTEST Mutation
lands, that gate reports "unavailable", which never counts as a pass, and the red check (T3) plus
review against the spec carry test quality (G4; DR 068 rule 4).

## 6. The record line

Each audit file in this folder uses one table, one line per case (§1).

| Test | Runs | Verdict | Reason | Evidence |
|---|---|---|---|---|
| C#: `<Namespace>.<Class>.<Method>`, a data row adding `[<n>]`, its 0-based ordinal in the order the method's data declares or yields its rows at the audited commit; a row's arguments, generated values such as temp paths and GUIDs included, are never part of the id. Gherkin: `<file>.feature: <scenario>`, an Examples row adding `[<n>]` the same way. Python: `<file>: <Class>.<test_name>`. Node: `<file>: <registered test name>`. A `<file>` is its path from the repository root | `all`, or the platform, skip condition or environment variable its assertions need | keep, rewrite or remove | One line: the §2 row it meets or fails, the §3 sign it shows, or `unsure` | keep: its tag. keep `unsure`: `only: <mutant id or planted bug>` and what is unresolved, with no tag required. rewrite: the job it will carry. remove, or a rewrite that drops a protection: per protection, `kept: <test id> fails on <mutant id or planted bug>`, or `unreachable: <why>`, plus the batch's mutation result or "mutation unavailable" |

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

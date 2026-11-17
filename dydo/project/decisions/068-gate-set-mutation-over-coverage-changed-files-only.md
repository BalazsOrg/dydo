---
area: project
type: decision
status: accepted
date: 2026-11-17
applies: [dydo-verify-own-work, dydo-verification-independent-evidence, dydo-everything-evolves, dydo-efficiency]
weighed: ['dydo-efficiency over dydo-verify-own-work, because "one of the big mistakes I made is that I set a coverage gate", accepting that "until Mutation lands, the only gate on test quality is the red check"']
---

# 068 - The Gate Set: Mutation Over Coverage, Changed Files Only

Coverage stops being a gate and becomes a report. HCRAP is dropped. Mutation is the gate on test
quality. Every static gate checks changed files only, at every stage, the landing included. The
other statics stay as [DR 048](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md) set
them. Decided by Balazs in grilling G on 2026-11-16/17, rules G1-G6 and D1-D2 of the
[DYD-971 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-971#comment-022a5db3-9f53-4ab1-aec8-d88569caab56).
Recorded under [DYD-1138](https://linear.app/bodnar-balazs/issue/DYD-1138);
[approved by Balazs on 2026-11-17](https://linear.app/bodnar-balazs/issue/DYD-1138#comment-66f49999-18d5-4bb9-86d6-e41dc836cbdb)
(F10).

---

## Context

DR 048 gated every module on line and branch coverage floors and on HCRAP, beside the statics. His
verdict on that: "one of the big mistakes I made is that I set a coverage gate." Introducing it was
"literally a marathon to write that many tests to keep up with it", and unrelated failures failed
tickets until the rule was overruled.

[DR 067](./067-what-tests-are-for.md) settles what tests are for: a gate number never earns a test
its place (T1). This record settles the gates. The file token limit, a further static gate placed
per rule 5, is DR 069's.

## Decision

1. **Coverage is a report, not a gate (G1).** Coverage floors are dropped as a gate. Coverage
   becomes a report: changed code that no selected test reaches, for the reviewer and the
   dashboard. Coverage stays useful as information "if the agents are exposed to a quick way to get
   it", so a fast, on-demand coverage query is a feature, not a gate.
2. **HCRAP is dropped (G2):** "let's drop it". Cognitive complexity carries the structure part.
3. **The other statics are unchanged (G3):** cognitive complexity ≤ 20, parameters ≤ 7, nested
   ternary, duplication, dependency cycles, dead code, build invariants, as DR 048 §3 sets them.
   - Settled later, on 2026-11-17 in the DYD-1145 co-think
     ([DYD-971](https://linear.app/bodnar-balazs/issue/DYD-971), comment b4b2aa41): duplication,
     dependency cycles and dead code are linter work. "So in short I'd say that it is settled, it's
     not dytest's concern." G3 stays dydo's rule; the 4.0.0 default gate configuration delivers
     those three through linters wired through dydo's configuration, not through dytest.
4. **Mutation is the test-quality gate (G4):** no surviving mutant in changed code at the final
   candidate.
   - Equivalent mutants are approved one at a time, with a reason, never by a threshold.
   - Until DYTEST Mutation lands, the gate reports mutation as "unavailable", which never counts as
     a pass. Until then the red check (DR 067, T3) plus review against the spec carry test quality.
   - The mechanism (tools, budget, scheduling) is
     [DYD-826](https://linear.app/bodnar-balazs/issue/DYD-826)'s.
5. **Changed files only, at every stage, the landing included (G5).** Every static gate checks
   changed files only. At the landing, "changed" means the Project's diff. The whole-tree run is a
   report. His reasoning: "I like the idea of not having to wait days for new rule or constraint to
   be properly adapted (because when we introduced the coverage gate it was literally a marathon to
   write that many tests to keep up with it and then all unrelated failures meant failures for the
   ticket so there was an awkward transitionary period where that rule was overruled)… it will
   encourage the 'gardening' and eventually all touched files will conform."
   - **Reversal.** This revises his earlier answer, "changed at the ticket,
     whole tree at the landing". The whole tree is no longer gated at the landing.
6. **dytest supports what dydo needs (G6).** dytest drops the coverage thresholds as gates (floors
   and HCRAP). It keeps coverage measurement, for rule 1's report and for the mutation gate's
   "uncovered mutant" check. "Dytest should support what dydo needs and not necessarily more (dytest
   is still standalone and useable without dydo)." And: "we can dogfood the ideas of dydo 4.0.0
   where we can." The removal is a DYTEST-side ticket (E2). Until it lands, DYTEST runs with the
   floors off in its configuration.
   - **Reversal.** This reverses his 2026-11-12 "dytest must keep supporting coverage gates".
7. **DYTEST adopts this policy now, as configuration (D1).** The coverage and HCRAP debt behind
   [DYD-1066](https://linear.app/bodnar-balazs/issue/DYD-1066) disappears: "funnily enough yes.
   These things still left disappear. And we focus on more important matters." Under rule 5, the 50
   cognitive callables become report-only, fixed when touched. DYD-1091 to DYD-1098 are cut down to
   their cognitive items, or closed.
8. **Numbers are configuration; doctrine is the skills (D2).** The default configuration for an
   adopting project: the statics, the 5k token limit with the nudge (DR 069), the mutation gate on
   changed code, and coverage as a report. Each project can change every number and keeps its own
   ignore file. The doctrine (DR 067's T1-T8) ships in the skills, and an adopting project changes
   it in its own copy of dydo (E3).

## Consequences

- **Hard to reverse:** every adopting project's gates change (grilling G's DR test).
- **Surprising later:** "why no coverage gate?" Rule 1 and DR 067's T1 are the answer.
- **The trade-off:** until Mutation lands, the only gate on test quality is the red check.
- A new or tightened gate fails only the files a change touches. The rest of the tree shows in the
  whole-tree report and conforms as it is touched ("gardening").

---

## Supersedes and amends

Partially supersedes [DR 048](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md), which
stays `accepted` for the rest:

- **§3, the gate set.** The Coverage floor row, the HCRAP row and the HCRAP note go (rules 1, 2).
  The Tests row is superseded: DR 067's tag lint replaced its "every non-trivial module has a test
  file", and mutation now gates test quality (rule 4). Grilling G does not restate the row's "all
  pass". §3's other rows and notes stand (rule 3).
- **§4.** Mutation is no longer a per-project threshold: no surviving mutant in changed code,
  equivalent mutants approved one at a time, never by a threshold (rule 4).
- **§2, amended.** Every module is still held to the one set, but the gate checks it only on
  changed files, at every stage (rule 5).
- **The 2026-09-17, 2026-11-04 and 2026-11-05 amendments** scope the coverage thresholds (line,
  branch and HCRAP), which are no longer gates (rules 1, 2, 6). Grilling G does not decide what
  becomes of their measurement rules.

**Open, not decided here.** On 2026-11-17 Balazs questioned §2's "dydo owns the rules, not the
runners. Each project keeps its own `gap_check.py`…": "I'm really not sure about the other projects
keeping gap_check.py. And by not sure I mean I really don't know." Whether that line is superseded
goes to him as a DR question (DYD-971, comment b4b2aa41). Until he answers, §2's line stands.

## Related

- [DR 048 - One-Level Static Gates](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md)
- [DR 067 - What Tests Are For](./067-what-tests-are-for.md)
- [DYD-971](https://linear.app/bodnar-balazs/issue/DYD-971) - Grilling G

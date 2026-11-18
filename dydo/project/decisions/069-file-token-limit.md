---
area: project
type: decision
status: accepted
date: 2026-11-17
applies: [dydo-finite-context, dydo-efficiency, dydo-attention-human-decisions]
---

# 069 - File Token Limit

> **Amended 2026-11-18:** [the accepted amendment below](#amendment-2026-11-18---path-b-and-the-manifest-waiver) resolves the §1 conflict noted under Supersedes and amends and records the manifest waiver's lapse.

Every file, tests included, is limited to 5k tokens, counted as bytes ÷ 4. An edit-time nudge warns
while the file grows, and a static gate checks changed files. Files nobody writes by hand are
ignored through a reviewed entry; authored code is ignored only through its own ticket, a council
and Balazs's blessing. Decided by Balazs in grilling G on 2026-11-16/17, rules L1-L3 of the
[DYD-971 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-971#comment-022a5db3-9f53-4ab1-aec8-d88569caab56),
which promoted [DYD-342](https://linear.app/bodnar-balazs/issue/DYD-342) into dydo 4.0.0. Recorded
under [DYD-1139](https://linear.app/bodnar-balazs/issue/DYD-1139);
[approved by Balazs on 2026-11-17](https://linear.app/bodnar-balazs/issue/DYD-1139#comment-4ae8f212-8324-45a3-b395-1606cfb9369c)
(F10).

---

## Context

DYD-342 designed an edit-time nudge for oversized files and parked it: "sceptical… not needed yet".
Grilling G took it up beside the gate set
([DR 068](./068-gate-set-mutation-over-coverage-changed-files-only.md)). The source is Pocock's
pre-commit hook ("The simplest investment I ever made in my codebase").

**Measured** (grilling G). dydo: 63 of 686 source files are over 5k tokens, 53 of them tests. LC: 85
of 2,529 source files and 199 of 1,661 test files are over, plus generated migrations. Under DR 068's
changed-files-only placement (G5), no migration is needed.

## Decision

1. **Both a nudge and a gate (L1).**
   - The edit-time nudge follows DYD-342's design: it re-fires only after more growth, and agent
     scratch is excluded.
   - The gate is a static rule placed per DR 068's rule 5 (G5): it checks changed files only, at
     every stage, the landing included.
   - **Reversal.** This reverses DYD-342's "sceptical… not needed yet".
2. **5k tokens, bytes ÷ 4, for every file, tests included (L2).** "this 5k is probably the sweet
   spot… it's cheap to update. very cheap." Evidence: one LC E2E spec holds 11 tests in 3.4k
   tokens. Test files "are just as at risk as others and probably even more."
3. **The ignore file has two paths (L3).**
   - **Path (a): files nobody writes by hand** (data, generated, migrations, lockfiles, vendored).
     The PR adds the entry, with an exact path or a generated-directory glob plus a reason, and the
     reviewer checks it. An entry that matches no file fails the gate. LC's `UsernameWords.cs`
     ("correct as is") goes this way.
   - **Path (b): authored code**, a "really… special thing". It goes only through its own ticket: a
     council of agents checks it against the rule, then Balazs blesses it ("I do not like being
     bothered"). A PR can't add a path-(b) entry for itself.

The limit, like every number, is configuration: an adopting project can change it and keeps its own
ignore file (DR 068, D2).

## Consequences

- An oversized file that a change doesn't touch is reported, not failed; the gate checks it when it
  is next changed.
- A stale ignore entry cannot linger: an entry that matches no file fails the gate.

---

## Supersedes and amends

Amends [DR 048](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md) §3, whose "Not
gated" note lists file length, and its Consequences, which reject "method and file length as gates
(legitimate exceptions exist)": file size, in tokens, is now gated. Method length is untouched.

**Not reconciled by grilling G.** L3's path (b) admits a per-file exception for authored code. DR
048 §1 admits a gate only where "no per-file suppression exists" and puts everything with a
legitimate exception in review. Grilling G does not discuss §1. This record names the conflict for
his approval; it does not resolve it.

## Amendment 2026-11-18 - path (b) and the manifest waiver

**Status: accepted.** Balazs resolved the conflict with DR 048 §1 in
[DYD-1195 DECIDED, envoy 1](https://linear.app/bodnar-balazs/issue/DYD-1195#comment-fd543916-0ef1-4ce0-9f61-4ff8a5454b29),
verbatim: "option 1". Path (b) stays as the file-token gate's only per-file exception:
its own ticket, a council check and his blessing. The corresponding amendment lives in
[DR 048](./048-one-level-static-gates-certainly-wrong-no-escape-hatch.md#amendment-2026-11-18---one-per-file-exception).

**Status: accepted.** Approved by Balazs in
[DYD-1195 Q11 DECIDED, envoy 2](https://linear.app/bodnar-balazs/issue/DYD-1195#comment-2546c840-53a6-436a-aade-873052d14c89).
His answer covering Q11, verbatim: "16. a. 9. a, 13. a. 10. a, 11. a".
Q11 option (a) makes the admiral's file-size waiver (~15.7k tokens, PR 359) lapse.
[DYD-1192](https://linear.app/bodnar-balazs/issue/DYD-1192)'s later ruling deletes the legacy
file and its consumers. The admiral's waiver supplies no exemption.

## Related

- [DR 068 - The Gate Set](./068-gate-set-mutation-over-coverage-changed-files-only.md) - Placement (G5) and defaults (D2)
- [DR 067 - What Tests Are For](./067-what-tests-are-for.md)
- [DYD-342](https://linear.app/bodnar-balazs/issue/DYD-342) - The nudge's design

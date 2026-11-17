---
area: project
type: decision
status: accepted
date: 2026-11-17
applies: [dydo-aligned-intent, dydo-scarce-attention, dydo-attention-human-decisions, dydo-verification-independent-evidence, dydo-everything-evolves, dydo-efficiency, dydo-effort-right-agent, dydo-effort-right-sized-bureaucracy]
weighed:
  - "dydo-efficiency over dydo-verification-independent-evidence, because \"Not every PR deserves a council. In fact most don't. The heavy ones do.\" (F7)"
---

# 066 - The Law, Councils and the Tribunal

Each project carries a law (axioms, then principles, then case law) so agents can learn to judge the
way Balazs would. A Decision Record cites the law it applies and the values it weighed. A council of
fresh agents from different model families answers a question when confidence matters; the tribunal
is the council that judges his hard calls. A decided Question is provisional, not ratified. Changes
to principles, axioms, skill prose and larger architecture are his. Decided by Balazs in
[grilling F](https://linear.app/bodnar-balazs/document/370-grilling-f-law-and-council-b135b45ac6bb)
on 2026-11-15 (F0 to F17), as reconciled with his L1 sitting of 2026-11-17
([DYD-1078 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1078#comment-9ac278c0-00f3-4688-9971-2693bfc14e38),
A1 to A9), which wins where they differ. Recorded under
[DYD-1088](https://linear.app/bodnar-balazs/issue/DYD-1088);
[approved by Balazs on 2026-11-17](https://linear.app/bodnar-balazs/issue/DYD-1088#comment-9692966d-3c36-4810-b5f4-8ff7239e2e2f)
(F10). The grillings say 3.7.0; that release is 4.0.0.

---

## Context

His starting point (F0): "I found that many of my decisions were based on a couple of ideas and
values and these value judgments and fundamental truths are what live in the rules and the root of
the rules are the fundamental ones which are the axioms." Each project "could and should have its
own version" of them, "so that I'm needed less to make these judgment calls and the agents can kind
of learn to think like me." None of the 63 DRs cited an axiom: "Because these axioms are newer than
the DRs."

The 3.6.0 machinery had two strains. The what-would-balazs-do tribunal
([DR 054](./054-what-would-balazs-do-tribunal.md)) seated the raising officer as a sealed member
with two judges, ran up to four rounds, and left every output unratified until he answered; the
morning brief listed every decided Question with "Ratify or reverse". His verdict: "I get overwhelmed
because I get shown every single decision they made and most of them are right and I don't care. I
only care about the big ones" ([DYD-1046](https://linear.app/bodnar-balazs/issue/DYD-1046) asked for
a lighter tier). And grilling D took hard calls off the admirals and gave them to a council (D-D15),
leaving the council's shape to this grilling.

The evidence on panels (F4): in 3.6.0's 81 paired review rounds, 65 had no blocker for either seat,
and a second seat beside a strong discovery model found a unique real blocker in about 1 round in
40. Published work found that judges favour their own family's output. His correction: published
findings are dated hints, "the AI landscape changes every two weeks".

## Decision

### The law

1. **What the law is for (F0, F5).** A hard call is one with "multiple competing answers and all are
   good depending on what lens you look at it", not one with "only one correct decision, like a
   technical question" ("then let's do it properly"); research and proof settle those. Consistency is
   the test: "if I'm consistent, then I would probably choose the same way. And if I do not, then
   that's also a lot for discussion." An answer of his that departs from his precedent or his law is
   an amendment candidate, never noise.
2. **The law is a dydo structure, and it stops at case law (F1, L1 A1).** Each project has its own
   law; dydo ships the structure. The law is **axioms**, then **principles** (a tree), then **case
   law**. Standards, requirements and the moves in skill prose are not part of the law. F11's five
   layers (axioms, principles, standards, requirements, procedures) and its claim that each standard
   or procedure cites a principle are **reversed by the L1 sitting's A1**: "This is news to me." "A.
   Keeping it simple is probably better." The term "procedure" is not adopted. A rule outside the law
   *may* name the principle it serves; nothing requires it.
3. **Principles (F11, L1 A2, A3).** What F1 called "rules" are **principles**: "rename to
   principles". A principle is a value judgement that follows from an axiom or a broader principle
   and can lose to another value in a given case. Principles form a tree: each cites axioms or a
   broader principle, and every chain ends at an axiom ("Tree is good."). **F11's test** decides what
   gets into the law, and only that: could a reasonable person choose otherwise in a given case and
   still be right? Yes → a principle, weighed, not enforced. No → checkable, so a test, lint or gate
   holds it; never a principle. A principle earns its place by being weighed; one no DR or ruling has
   applied or weighed for a long time is a pruning candidate. Axioms are "the things we build upon and
   we don't question or second-guess"; they change only if the world they assume changes.
4. **Every new DR cites the law: `applies:` and `weighed:` (F1, F2, F13).** Every new DR records
   `applies:`, the principles or axioms it applies, or `new-ground`. For each tension it resolved, it
   records `weighed: <slug> over <slug>, because <reason>`; the reason clause is what a council
   reuses. "this is the case law's spine." `weighed:` appears only where the DR states the trade-off,
   quoted, never reconstructed. `dydo check` validates the slugs. A council's method in a value call:
   map each option to the axioms it serves, find the DRs that weighed the same tension, predict from
   them; if he weighed it both ways, the call is prepared for him as an amendment candidate.
5. **Case law is DRs plus case studies (L1 A5, A6).** The definition of a DR is not broadened. A
   **case study** is an optional markdown file in git beside the law: a Question, grilling decision
   or idea that shaped it, linking its Linear origin, "kind of like a post-mortem and an origin story
   for that law" ("not necessarily a post-mortem, but it can be one"). Agents prepare one when a call
   looks precedential; he approves it. Nothing is generated: no built list of DRs per principle. A
   precedent that really matters becomes a principle that points to its origin and a few examples.
6. **Two laws per project, tech and product (F16, L1 A8, A9).** "the 'workflow law' is just Dydo's
   own Product law."
   - **Tech law:** technical values and choices. **Product law:** business decisions, what gets built
     and how. dydo's product law is how agents work; its seven axioms are that law.
   - **Layout (A8):** `dydo/law/`, one folder per law, each with `axioms.md`, one file per principle
     group, `cases/` and `retired.md`. The product law's folder takes the product's name (`law/dydo/`
     here, `law/lc/` in LC); the tech law's is `law/tech/`. The scaffold ships the product folder as
     `law/product/`, with a placeholder to replace with the product's name.
   - **Slugs (A9):** `<prefix>-<group>-<name>`, axioms `<prefix>-<name>`. The product-law prefix is
     the product's name (`dydo-…`, `lc-…`), never `product-`, "ugly and impractical"; the tech law
     keeps `tech-`. A group is the kind of choice a principle guides; an area is a tag, not part of
     the slug. Axioms get named slugs, not numbers. `applies:` slugs name their law by their prefix.
   - **Adopters** get dydo's product law read-only at `law/dydo/`, beside their own two laws. This
     replaces F16's location `dydo/reference/dydo-axioms.md` (A8). They never edit it; a project that
     disagrees changes its local skills (DR 049 §4). They also get an empty law of their own with a
     short guide: F11's test, the law's three parts, a principle cites an axiom or a broader
     principle, a DR cites the law it applies, the owner writes it.
   - **Which law decides** in an adopting project: how agents work → dydo's product law; a technical
     choice → the project's tech law; a business choice → its product law. A call between tech and
     product values is a tension the owner weighs.
   - **dydo's own tech law** is an example to write, not shipped. At the L1 sitting its candidates
     stayed a shortlist for the backfill to test; nothing is enacted yet.
7. **The law grows from evidence, continuously; he is the compass (F17, L1 A7).** "it should grow
   from evidence and it should be a continuous maintenance effort. This is the job that I'm needed
   for." "it is very important that this law system is designed to evolve and gradually ripen and not
   be one-shotted."
   - No up-front elicitation interview and no new skill; `domain-modeling` stays untouched. Each
     project's `_decisions.md` carries `applies:` and `weighed:`.
   - Four sources, all existing moves: `new-ground` DRs; rulings he overturns and daytime answers that
     departed from the tribunal's prediction; recurring corrections (retro's twice threshold); values
     he states in a grilling or co-think.
   - `retro` gains one lens, **Law**: a recurrence that passes F11's test prepares a principle or
     axiom candidate (which law, what it cites, the cases behind it). Mechanical recurrences keep
     going to checks.
   - A slug is never renamed or reused. Wording may be sharpened in place; a change of meaning gets a
     new slug. A retired or superseded principle leaves its group file for its law's `retired.md`,
     which keeps the slug, the statement, *retired* or *superseded by `<slug>`*, and the reason.
   - The first law comes from the backfill (decision 8): its `new-ground` rows seed dydo's tech law;
     an adopter with DRs runs the same backfill; a new project starts empty.
8. **The accepted DRs are backfilled once, and he reviews the result (F13, F14).** "Although I would
   have to review these somehow, this is important." After he approves the sorted principles, cheap-tier
   crew drafts `applies:` on each accepted DR and `weighed:` where the DR states the trade-off;
   superseded DRs keep their history untagged. A reviewer checks every citation as a doc claim, then
   he reviews one page per batch: a table with one row per DR (title linked, `applies:`, `weighed:`
   with the DR's own sentence quoted beside it), doubted and `new-ground` rows first. He marks each
   row ✓, ✗ or "retag"; only approved rows merge.

### Gardening authority

9. **Principles, axioms, skill prose and larger architecture are his (F10).** "Neither skill prose,
   nor rule or axioms should change without my approval. And I would say that even larger
   architecture stuff should involve me. This is the exact spot where my time has the highest ROI.
   ... I'd say that's something an agent should never make. They may prepare it."

   | What changes | Agents | Balazs |
   | -- | -- | -- |
   | lint, test, small code-pattern cleanup | draft and enact through the normal review loop | - |
   | larger architecture (broken architecture vs a better pattern) | prepare | decides |
   | skill prose (claims) | prepare | approves |
   | principles and axioms | prepare, with the cases behind them | his only; an agent never makes one |

   - A proposed principle whose chain ends at no axiom is rejected as bureaucracy (F10; the tree is
     L1's A3). F10's "citing the principle it serves" on the agents' row is optional since A1.
   - Proposals batch per Project and reach him at the Milestone walkthrough or as one envoy bundle,
     never one at a time.
   - Order of fixes for a weed: good code patterns and periodic cleanup first, then lints and tests,
     then the levels that are his.

### Councils

10. **What a council is (F3, F5).** "Independent, cross-family, read-only and agreement as
    confidence." A council puts one question to fresh agents from different model families, each
    working independently and sealed before seeing the others. A non-member synthesises: the answer,
    its confidence (agreed, split or contested), and the disagreements. It buys parallel work (each
    member catches different things, and even one model answers differently each run), confidence
    when they agree, and discussion when they don't; families matter because of model biases. It is
    expensive: "I don't think that the council should be overused ... But I think the concept itself
    is a useful tool in the arsenal."
11. **Two families for now (F1).** "two model families for now is good enough." Councils run on
    Claude and Codex. A third family is a later option, through OpenCode, not a requirement; the
    council names families from the model roster, so adding one changes one row.
12. **Four uses; convene for stakes or value only (F4, F6).** "these four uses are correct."
    - **Every member looks at the same thing.** "Let's not conflate the council with having distributed
      sub agents divided and conquering." A unit too big for one reading is cut smaller, not given to a
      council.
    - **Convene for stakes or value.** Stakes: the last check before main, or a mistake expensive to
      undo. Value: a call between good options, where what Balazs would choose is the question.
      Otherwise one fresh seat.
    - The uses: (1) **Review** where the stakes are high: guard or security work, a base other tickets
      build on, the late whole-system simplify review; the officer rules. (2) **Merge confidence:** the
      landing into main, as a Review council. (3) **Evaluate:** hill-climbs, each member scoring each
      iteration against the baseline, disagreement flagged. (4) **Judge:** the tribunal (decision 15).
    - **Deterministic work runs once.** A lookup, a test run or a drive is one agent's job, once:
      running them once per seat is "pure waste". Members share the author's receipts and DYTEST's
      results and add only judgement. Verify is not a council kind.
    - Not a council: lookups, tests and drives; ordinary reviews of leaves and middle levels (one
      cross-family seat). Parallel seats for speed in an ordinary review stay a separate thing.
13. **Standing rules (F4, F5).** Any reviewer or judge, single seats included, comes from a family
    other than the author's when possible: self-preference "is the reason why the one-seats should be
    cross-vendor when possible as well." A **model roster** that every officer sees, and he keeps
    current, names the models to use, for what, what each excels at and what not to use it for.
    Published research is a hint, not law.
14. **The work never stops for usage (F15).** "Bottom line is that the work must not stop. ... If we
    ran out of usage tough luck. We cook from what we've got." An officer never brings a usage limit
    to him; it adapts and records. A council never substitutes silently. With one family out: a
    **Review** or **Evaluate** council runs two different models of the remaining family, flagged
    "single-family"; a **Judge overnight** decides on one family, flagged, and the flag shows on the
    morning list; a **Judge by day** prepares on one family, flagged, and he decides; **Merge
    confidence** at the landing waits for both families only while nothing blocks on it, then runs
    single-family, flagged, never escalated to him: "a council of different models from the same
    family is probably still better than me glancing at the door: two-way and clicking merge."
    Councils name tiers and families; the roster names models, and `dydo usage` tells the officer
    which family has headroom.

### The tribunal and provisional Questions

15. **The tribunal is the Judge council's procedure (F7).** "Let's call the judge council the
    'tribunal'." The general tool is "council".
    1. **Homework, the admiral's (D-D9).** It decides whether a tribunal is needed. If a DR clearly
       settles the call, it isn't a hard call: the holder decides and cites the DR.
    2. **A neutral case file:** only the call, the options unranked, the constraints and the primary
       facts with their sources; never the admiral's tension mapping or choice of precedent. "So if
       there's an error in that research all the judges will be poisoned?"
    3. **Who sits:** the admiral isn't a member. A fresh clerk holds the Question ticket and runs it
       mechanically. Two judges from different families, picked from the roster.
    4. **Sealed positions.** Each judge alone maps each option to the axioms it serves, reads
       precedent through `weighed:`, names what Balazs would choose and why, and covers the two duties
       ("should this be done at all?", "is the frame too narrow?").
    5. **One exchange.** Each answers the other's strongest point or concedes, quoting the argument.
    6. **Exits (D-D12).** They agree on a two-way hard call → decided, citing the law and precedent.
       They disagree, or the precedent points both ways → prepared for him with both positions.
       One-way or DR-grade → always prepared, except decision 16's overnight case.

    Most PRs get no council: "Not every PR deserves a council. In fact most don't. The heavy ones
    do." A Review council is for heavy PRs; every other PR gets one cross-family seat.
16. **A decided Question is provisional, not ratified (F8, F9); this answers DYD-1046.** "the
    decision should be left as not ratified but marked as provisional or something similar so the AIs
    can act on it and it's treated as valid unless disproven. I do not ratify them because that's a
    lot of work. And if I ratify them, that's essentially rubber stamping."
    - A tribunal-decided Question is **provisional**: acted on at once, valid unless disproven. No
      ratification; the morning brief doesn't list it for approval.
    - A provisional ruling found wrong (he finds it broken, or dislikes the outcome) is
      **investigated**: why it was made, then a measure so the failure can't recur.
    - **By day** a hard Question goes to the tribunal, which prepares it (prediction, reasoning from
      the law and precedent, any dissent); the admiral's envoy brings it to him and he answers.
      **Overnight** the tribunal decides it, provisionally. "b".
    - Every daytime answer is scored against the prediction on the Question ticket (matched or
      overturned, and why), kept as evidence only. Graduation is withdrawn: "When did I give any
      indication in support of this? This is something we may add later."
    - **DR-grade calls surface at muster:** before overnight mode, each admiral lists the DR-grade
      calls its night could hit, so he decides them in advance. Overnight, rarely, a two-way DR-grade
      call that seriously blocks work may be decided by the tribunal with both judges agreeing; the DR
      is written `proposed`, provisional pending his ratification. "Overnight decision records are the
      ones I should be ratifying and they should be very rare." These are the only decisions the
      morning brief asks him to ratify. **Irreversible calls are never decided;** they wait for him.
      This narrows D-D12's "DR-grade is never decided".

## Admiral defaults

Grilling F left these for the admiral to default from tstack's council (F, Open). Each follows from
settled decisions.

- **Keep read-only.** Council members never edit; an experiment goes to a proof-only code-writer.
  *admiral default; Balazs to confirm.*
- **Keep cross-family.** As decisions 10 and 13 state. *admiral default; Balazs to confirm.*
- **Keep report-don't-substitute,** made concrete by decision 14: a council never substitutes
  silently, and every fallback carries its "single-family" flag. *admiral default; Balazs to
  confirm.*
- **Drop tstack's /10 score for Review.** dydo grades by blocker, should and nit
  ([DR 058](./058-review-severities-blocker-should-nit.md)), so a number would add a second scale to
  argue over. *admiral default; Balazs to confirm.*
- **Drop tstack's PR table.** The record is the Review sub-ticket, one per round with its seats
  attributed; the PR carries only the one-line PASS that names it. *admiral default; Balazs to
  confirm.*

## Consequences

- The law is citable: every new DR, this one included, carries `applies:` and, where it resolved a
  tension, `weighed:`. The principle set is incomplete by design and grows by amendment.
- His attention moves from ratifying many small calls to the few DR-grade ones, the walkthrough and
  the law itself.
- The admiral prepares hard calls and routes verdicts; it no longer makes them.
- The council and tribunal rules carry the single-family fallback as claims (F15), and retro gains
  its Law lens (F17); both are skill prose and need his approval
  ([DR 064](./064-claims-build-the-skills.md)).
- Officer skills lose any "report the limit and stop" move.
- Councils cost more than one seat, so they run only for stakes or value.

## Affects

- [DR 054 - What Would Balazs Do](./054-what-would-balazs-do-tribunal.md): F7 rebuilds the tribunal it
  set (the officer is no longer a sealed member, rounds drop from up to four to one exchange, the case
  file goes neutral, two judges and a clerk), and F8 replaces its ratification of every tribunal
  output with provisional rulings.
- [DR 065 - Review Placement and the Merge-Review Cut](./065-review-placement-and-the-merge-review-cut.md): the review seats it scales, and the hard calls it sends to the council.

## Related

- [Law](../../law/_law.md) - dydo's product law and the tech law
- [Glossary](../../reference/dydo-glossary.md) - Axiom, principle, case law, case study

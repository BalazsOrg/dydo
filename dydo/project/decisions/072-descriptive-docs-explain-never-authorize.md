---
area: project
type: decision
status: proposed
date: 2026-11-18
applies: [dydo-aligned-intent, dydo-verify-own-work, dydo-efficiency, dydo-easy-path]
---

# 072 - Descriptive Docs Explain, Never Authorize

A **descriptive doc** says what the code does or how it works. It explains and never authorizes: on
what the code does, the code wins; on why it must be so, only a Decision Record or a principle
carries weight. A descriptive doc is written only when Balazs invites it, must pass a three-part
admission test, declares the code paths it explains, and is kept true by the change that touches
that code. **Prescriptive docs** (standards, guides, processes) are outside these rules. The
"architecture plan" kind is scrapped, which withdraws [DR 051](./051-captains-by-default-admiral-on-invocation.md)
§6. Inside one context a term has one meaning, and the glossary takes upstream `domain-modeling`'s
shape of one glossary per context. Decided by Balazs in the DYD-1100 grilling on 2026-11-17/18,
held live with the dydo 4.0.0 admiral's envoy and recorded as sections §1 to §5 of the
[DYD-1100 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1100#comment-bdb6b643-f33d-4bd0-b801-c33566d63bca).
He made it DR-grade "because of naming the enemy (which is prescriptive vs descriptive
documentation and drift)". Recorded under [DYD-1185](https://linear.app/bodnar-balazs/issue/DYD-1185).
Proposed; it awaits Balazs's acceptance (F10).

---

## Context

The grilling started from a question about a per-Project architecture document. He found the
question mis-framed (§1): "I think there is no such thing as per project architecture document
because we have this spec which gets deleted and we have documentation which transcends the project
boundaries and it describes the code and the processes and the behavior. and the algorithms and the
tricky bits and the important parts of our codent system. So I think this whole question started on
an imprecise definition … Projects do not get this kind of treatment and neither do features by the
way. … What gets a docs or documentation? And that is the question."

**The enemy is drift** (§2). A descriptive doc "may describe something archaic or superseded or
something bad. And worse, it may even supply a reason to why the code should be that way, and
that's a problem." Giving up on docs is no answer either: "if we go on the other extreme to say that
all documentation drifts, then we kind of give up on the good bits."

**Leftover plan files** (§1). Eleven plan files from finished Projects are still in
`dydo/project/plans/`, several still `draft` or `active`. In his words, "this is a problem for
projects closing and cleanup"; "they were still meant to be deleted". The cause is a missing step in
the admiral's Project closeout. The DECIDED records it as a finding, not a design question.

## Decision

1. **No architecture-plan kind (§1).** No Project and no feature gets its own architecture
   document. On the "architecture plan" kind: "Scrap it. It wasn't even what you meant in the first
   place." DR 051 §6's "a repository plan file is written only for a cross-cutting architecture
   contract" is withdrawn, with its `project-plan` review, the glossary's "Project plan" entry and
   grilling F's F12 rename ("architecture plan").
2. **Specs are unchanged (§1).** He restated them as they stand: one main spec per Project, with
   linked specs for sub-features that need them, the source of truth and "measuring stick" for the
   implementation tickets, moving to Linear in 4.0.0 and deleted at landing. The rule is
   [DR 070](./070-statuses-as-sub-tickets-shoulds-and-the-map.md)'s (grilling C's C8, DR 053's successor);
   this record does not restate it.
3. **Docs are just docs, of two kinds (§2).** "We call them 'documentation'. 'docs' for short." They
   live in their usual homes, with no special name or folder.
   - **Descriptive** docs say what the code does or how it works. They are mostly in `reference/`,
     but can live in any folder. Rules 4 to 9 apply to them only.
   - **Prescriptive** docs (standards, guides, processes) are outside those rules: "They can't drift
     from code, because these don't describe code as is. They don't describe code at all. … They
     describe patterns and practices which the code should follow. And if they're followed they
     happen to describe the code in a general way, because they kind of prescribed it. The
     relationship is backwards."
   - **Scope.** In his words: "if we're specifically talking about dydo then I think this scope is
     still right." This record decides the rules for dydo. It does not decide whether they bind the
     docs of a project that adopts dydo.
4. **Explain, never authorize (§3, rule 1).** On what the code does, the code wins, and a doc that
   disagrees is a doc bug. On why it must be so, only a DR or a principle carries weight, so a reason
   in a doc counts only if it links one. "This is a good rule, adopted. And I can not understate this
   how elegant a defusal this is."
5. **Admission test, all three required (§3, rule 2).** The code it explains is solid (rarely
   changes), important (costly to get wrong) and expensive to rebuild from code. His example is LC's
   authentication protocol, with asymmetric keys embedded in the hashing. "These three."
6. **Invite-only (§3, rule 3).** "adding these descriptionary docs is order-only/invite-only. I
   don't want agents creating these docs on their own, just because they've seen them being done.
   No. These docs are born specifically because I want to have them, because I deem that particular
   piece of the code worthy to have it (example: that LC auth flow)."
7. **Kept true by the change that touches the code (§3, rule 4).** "I like this."
   - The ticket that changes explained code updates the doc in the same change.
   - Each descriptive doc declares the code paths it explains in frontmatter, for example
     `explains: Services/Auth/**`.
   - `dydo check` / `pr-ready` flags a change that touches those paths; the writer updates the doc
     or states that it still holds.
8. **Review (§3, rule 5).** "this could work."
   - The docs rubric gains a first rule: an unlinked "why" is a finding.
   - The code rubric gains one line: when a change touches a doc's declared paths, the reviewer
     checks that doc against the new code.
9. **Existing descriptive docs are grandfathered (§3, rule 6).** He chose option (a), innocent until
   guilty. One mechanical ticket gives each existing descriptive doc its declared paths. A doc found
   drifted, or carrying an unlinked reason, is fixed or cut then.
10. **One meaning per term inside a context (§5).** A term may mean different things in different
    contexts. Inside one context it has one meaning, and it must always be obvious which context is
    meant, either by qualifying the term (`dydo guard` versus a UI "guard") or because the
    surrounding text makes it plain. His answer: "yes." This is Evans' bounded context.
11. **The glossary takes upstream's shape (§5).** "b -> adopt Pocock's upstream back, it's good. We
    just have to make it work for us with as little modifications as possible (ideally none) - or
    just path changes." That means one glossary per context, plus a context map when there is more
    than one context, following upstream `mattpocock/skills` `domain-modeling` (`CONTEXT-MAP.md`
    and a `CONTEXT.md` per context). dydo's own glossary is one context, shipped read-only to
    adopters like `law/dydo/`. The concern it answers: "I fear that a flat glossary is not
    necessarily a good enough thing."

## The pending principle

The grilling also gave a new principle (§4), a law amendment that is his. It is **pending**: it is
not yet committed under `law/dydo/`, so this record does not cite it in `applies:`. Its slug is
`dydo-design-shared-language`, it cites `dydo-aligned-intent`, and its statement as he approved it
("This is good.") is: "Name things precisely and use each name for one meaning, so the user and
agents mean the same things by the same terms." His reason: "it's only possible to have good
alignment if we speak the same language 'we mean the same things under certain terms'." Rules 10
and 11 serve it. The DECIDED holds his full description.

## Consequences

- A descriptive doc can no longer supply a reason the code must stay as it is. A reason without a
  DR or principle link is a review finding (rules 4 and 8).
- Drift is caught where it starts: the change that touches explained code is flagged and must
  update the doc or state that it holds (rule 7).
- Docs stay few. Agents add no descriptive doc on their own (rule 6).
- Repository plan files and the `project-plan` rubric go (rule 1).
- **Follow-ups**, from the DECIDED's "For the admiral" list. This record enacts none of them.
  - The glossary: remove "Project plan" and the Project entry's "cross-cutting architecture contract"
    clause, and undo F12's rename where it landed (DYD-1077's five swaps). The DECIDED says DYD-1086,
    DYD-1087 and DYD-1099 waited on this ruling and are unblocked by it.
  - Commit `dydo-design-shared-language`, its statement and his description, to `law/dydo/`.
  - Re-adopt upstream `domain-modeling`'s multi-context shape with path changes only. The
    adaptation's "one glossary per project" goes.
  - Tickets: the `explains:` frontmatter with the `dydo check` / `pr-ready` flag; the two rubric
    lines; the grandfathering ticket; and invite-only stated where agents would otherwise create
    docs (`docs-writer`, [Writing Docs](../../reference/writing-docs.md)).
  - A finding: Project closeout has no step that removes specs and plans at landing. Delete the
    eleven leftover plan files.

## Supersedes and amends

Withdraws [DR 051](./051-captains-by-default-admiral-on-invocation.md) §6: its repository plan file
for a cross-cutting architecture contract, and that file's `reviewer(project-plan)` review (rule 1).
What 051 hangs on §6 goes with it: its Consequences line "the `project-plan` rubric stays for the map
holder's plan file", and its amendments to DR 045 §5 and DR 047 §8 that keep a plan file "only under
decision 6". The DECIDED does not address §6's first sentence, "The Linear Project and its Issues
are the plan".

Other records name the kind or the rubric this record scraps: DR 053's authority order ("reviewed
architecture plan") and its Consequences ("Architecture plans keep their purpose and review"),
DR 045's rubric list and DR 058 decision 1's rubric list. DR 070 records DR 053's successor. This
record leaves their text as it is.

## Affects

- [dydo Glossary](../../reference/dydo-glossary.md)
- [Writing Docs](../../reference/writing-docs.md)
- [Plans](../plans/_plans.md)
- The `docs-writer`, `reviewer` (docs and code rubrics) and `domain-modeling` skills
- `dydo check` and `pr-ready`

## Related

- [DR 051 - Captains by Default, the Admiral on Invocation](./051-captains-by-default-admiral-on-invocation.md) - §6 withdrawn
- [DR 053 - Temporary Feature Specs](./053-temporary-feature-specs.md)
- [DR 070 - Statuses as Sub-tickets, Shoulds and the Map](./070-statuses-as-sub-tickets-shoulds-and-the-map.md) - Specs (C8)
- [DR 066 - The Law, Councils and the Tribunal](./066-the-law-councils-and-the-tribunal.md) - Principles are his (F10)
- [DYD-1100 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1100#comment-bdb6b643-f33d-4bd0-b801-c33566d63bca)

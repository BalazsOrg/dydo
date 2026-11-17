<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Reviewing Documentation

## docs

- **Anchor missing or doubled, or a tagline that changes nothing.**
- The no-op test grades against the model's default, and a failing line is deleted rather than softened.
- **A cross-reference missing or extra** against the exact set its brief binds.
- **Description that is not a trigger.** Model-invoked: leading word front-loaded, one trigger per branch, no identity the body already carries. Explicit-only: one punchy human-facing line.
- A crew role spawned by name may state its job, in trigger form.
- **Vocabulary off the dydo glossary**: a retired word anywhere, or a Linear noun away from a real handoff.
- `writing-for-agents` governs this writing; the list below is that method turned into verdicts.
- **Off the map.** Must-Reads that do not name what the sender hands over, a Return that does not name its receiver, or a sentence narrating what a neighbour does.
- **Steering by prohibition** where the positive target would carry it; a guardrail states both.
- **A return shape its consumer cannot parse**: the review block, or a crew member's return to the Captain.
- **Shape broken.** Officers and crew: H1 → one-line job → Must-Reads → Boundary → Method with a completion criterion on every step → Return or Handoff.
- A section order the user set for a role stands in place of Method; Must-Reads, Boundary, Return, and a completion criterion wherever steps exist still bind.
- **Sprawl**: a document longer than its reader's path needs. Reference only some branches reach goes behind a pointer; a meaning stated twice is stated once.
- Methods keep their upstream shape.
- **Upstream text altered without a binding reason.** Linear, dydo and host bindings are the whole licence, and the attribution comment stays.
- **Drift** is the failure both kinds share: a claim the tree no longer supports, or an environment restated in prose (directory layout, `--help`, config) where a pointer would not have gone stale.
- **Drift from the canonical source.** `skills/` is the single canonical tree, authored directly with no compile step; every host reads it through its own projection, never a generated copy. Judge drift against that rule and the ticket's own contract.
- `dydo check` or a ticket gate left failing on the touched tree.
- Frontmatter, naming, or a link broken against `dydo/reference/writing-docs.md`, read from the repository root.
- Written for a reader its folder does not name (`understand/` vs `guides/` vs `reference/`).
- A meaning another document already owns, written a second time here.
- A reusable decision, invariant, pitfall or explanation left only in execution evidence instead of assimilated into dydo.
- Every item below is a finding when it holds, labelled with its severity.
- **Resolve the contract.** The ticket description as of the brief's Linear `updatedAt`, and the governing plan and applicable feature-spec sections at pinned commits. Done when audience, owned paths, acceptance criteria and gates are named; an unmet criterion or an edit outside the owned paths is a finding.
- **Sort every changed file** into one of the two lists below, then work that list entire.
- **Verify from source.** Open the paths, run the commands, read the code; check this stage's exact-candidate gate evidence, `dydo check` among it, and rerun a gate only on a doubt you state. Done when every claim the change makes has met its source.

## evidence

- Open the paths, run the commands, read the code; check this stage's exact-candidate gate evidence, `dydo check` among it, and rerun a gate only on a doubt you state.

## review

- Two kinds arrive here: human-facing dydo documents under `dydo/`, judged against code for delivered behavior or governing intent for specs, and agent-facing documents (skills, their resources, entry points), judged as prompt files that must fire and route.

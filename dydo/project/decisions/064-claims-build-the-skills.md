---
area: project
type: decision
status: accepted
date: 2026-11-17
applies: [dydo-aligned-intent, dydo-verify-own-work, dydo-verification-independent-evidence, dydo-everything-evolves, dydo-context-pointers, dydo-context-lean-skills]
weighed:
  - "dydo-aligned-intent over dydo-efficiency, because alignment, not tokens, is \"the important part\" (A-D6)"
---

# 064 - Claims Build the Skills

For the skills that govern how agents work, a C# model of **claims** is the single source. A repo
tool builds their skill folders, the chart's moves and the shipped system docs from it, and is the
only writer of those folders. The built skill is committed and stays the place to read the skill.
This amends [DR 049](./049-skills-are-the-source-retire-the-compiler.md) §1 for that set. Decided by
Balazs in [grilling A](https://linear.app/bodnar-balazs/document/370-grilling-a-claims-built-skills-31f715eee513)
on 2026-11-10 (A-D1 to A-D15), with the lines of
[grilling D](https://linear.app/bodnar-balazs/document/370-grilling-d-hierarchy-t3-inbox-captain-autonomy-sub-captains-a5cb8f0b5a86)
(2026-11-13 to 15) and [grilling F](https://linear.app/bodnar-balazs/document/370-grilling-f-law-and-council-b135b45ac6bb)
(2026-11-15) that touch it. Recorded under [DYD-1084](https://linear.app/bodnar-balazs/issue/DYD-1084);
[approved by Balazs on 2026-11-17](https://linear.app/bodnar-balazs/issue/DYD-1084#comment-01b9debe-ca41-4ab9-a64e-a9edf9a3aca6)
(F10). The grillings say 3.7.0; that release is 4.0.0.

---

## Context

The problem is not token bloat. One rule has many homes in many wordings, and nothing connects a
rule to the behaviour it licenses. The 3.6.0 statecharts had to rebuild those links by hand: 640
arrows, each cited into 44 files, and 153 of them (24%) needed a silent, ambiguous or contradiction
mark. Exact duplicate sentences cost only about 2.6% of the words, so the win is correctness, not
tokens.

dydo generated its skills twice before: the 2.x role and include layer, and the `dydo sync` compiler
that DR 049 retired. Both translated one template into many host formats; neither gave a rule shared
across roles one home. This model works the other way: one rule into many role skills, in a host
format that is already native. DR 049 §5's "tiny generation script" is about agent definitions, not
skill prose, so it does not cover this (A-D2).

His framing (A-D1): some skills describe one system of processes, where "both sides expect the same
thing and there are proper checks and balances all the way". Built from free prose, holes were to be
expected, and the statechart found them. So "change how we build dydo": model the skills that govern
agent work, and build them from the model. "I don't think we need to invent or overcomplicate
anything." Agents still learn their place from the prose.

## Decision

1. **The model is the single source; the properties stay outside it (A-D1).** "I like this
   recommendation. It feels simple and workable enough." The C# model builds the built skills,
   structured by state with each role's prose; `transitions.json`; and the TLA+ spec's states and
   moves (`Next`). A guard is held in its claim as short TLA+ text (for example `"fails >= 5"`). The
   properties (what must always hold, what must eventually happen) are hand-written from the axioms
   and the rules that follow from them, never from the model: if one source wrote both, TLC could
   only prove self-consistency. A claim that licenses no transition carries prose only. The chart
   cross-check: every generated move cites a claim id; every property is hand-written and traces to
   an axiom or rule.
2. **Built prose is efficient, precise and redundant (A-D1).** Skills aren't "fast": they spend fewer
   tokens and are precise, optimised for tokens and for alignment, "so that it gets followed and
   acted out." Redundancy is a design goal: "There should be redundancies in the system so that one
   mistake can not compound." In the model, that is a role's line on the other side of a transition,
   and properties can require it. One home per claim governs where a rule is written, not how many
   roles carry and check it.
3. **The skill folder is where you read the skill (A-D2).** "Yes, I think the skill folder is no
   longer the source. It's still the way to look at the skill. Because we'll pipe the generated
   skills there." For the **claim-made set**, the model is the source and the built skill is written
   into the skill folder, the reading surface for agents, review and Balazs. The set, by his scope
   rule (roles and workflows that interact with dydo, plus docs acting as skills): the officer and
   crew role skills and their resources; the workflow skills wayfinder, what-would-balazs-do,
   overnight-mode and walkthrough; the working-tree standard (the working-tree contract when grilled)
   and the Linear workspace standard. Every other skill stays hand-written under DR 049 §1 and points
   at the built skills where it needs a rule. A claim edit's PR is read through its built-skill diff,
   not through the C# alone.
4. **Non-goals (A-D2).** No host translation, no agent configuration, no install-time build, no
   project-side model, no switchboard or includes. Downstream gets the built `SKILL.md` and its
   runtime resources only, never dydo's own tests or fixtures
   ([DYD-968](https://linear.app/bodnar-balazs/issue/DYD-968)), and no why docs (A-D11).
5. **The built output is committed; a golden test keeps hand edits out (A-D3).** "The output is
   committed." The golden test rebuilds every built file in memory and fails, with a diff, on any byte
   difference: a hand edit in a built skill, or a model edit committed without a rebuild. It runs in
   the normal suite and in CI. Built files carry a one-line header ("built from the model; edit
   there"), and the guard nudges on Edit or Write to a built path. `setup-skills.mjs`, `update-dydo`'s
   three-way merge, [DYD-956](https://linear.app/bodnar-balazs/issue/DYD-956)'s tree hash and
   [DYD-958](https://linear.app/bodnar-balazs/issue/DYD-958)'s main-checkout loading work unchanged.
6. **A claim is one rule with per-role prose (A-D4).** "C is good. We shouldn't over complicate this
   either." One line of prose per role that carries it, all roles side by side in one C# file per
   kind. Where one sentence should state several claims, a hand-written combined line names the claims
   it states, used sparingly; the build checks that every claim a role carries is stated by exactly
   one line in that role's built skill. No machine-composed prose: a human writes every line.
7. **A built role skill is a spine plus on-demand resources (A-D5, A-D8).** "Yes, splitting things
   into resources is probably the right call."
   - The **spine** (`SKILL.md`, always loaded): the role, what it is accountable for, the tools and surfaces
     it touches, then a state outline, each state's arrivals and exits as one-line moves. The spine
     carries each state's nominal path in full.
   - **Routines** join states as a spine element (A-D13): a recurring activity a role performs inside
     or across states, such as wayfinding, deciding a Question, or dispatching crew with tickets;
     overview in the spine, specifics in resources.
   - **Edge cases are appendices**, one short file per scenario, linked from the move where it arises.
     "I don't mind the resource's on-demand loaded thing to be not read at all at times. That's the
     point." An appendix is a checklist with its decisions and named sources; it saves the work and
     resumes, then ends **resolved** (silently, continue) or **escalated** (up the chain: officer, then
     the user only when his attention is needed). This is where self-healing lives in the prose.
   - No mandatory state files and no deep tree; folders by state are for organisation only.
   - No `dydo guide` command: "the resource files are better, because that's native." If wanted
     later, it is a thin reader over the same built files.
8. **Workflow skills become built role resources; his own copy is invoke-only (A-D6).** "I think b is
   good (but my version can be invoke-only)." wayfinder, what-would-balazs-do, overnight-mode and
   walkthrough are built into each role that uses them, as resources named at the move where they
   apply. Balazs's standalone copy of each is also built, with Claude Code's
   `disable-model-invocation: true`. General skills (diagnosing-bugs, maintain-verification-skill, …)
   stay shared and are named by the moves. **Superseded in part by D-D5 and D-D12:** overnight-mode
   belongs to the chief of staff and the admiral, and which roles carry what-would-balazs-do is now
   grilling F's, as one use of a general council
   ([DR 066](./066-the-law-councils-and-the-tribunal.md)).
9. **Skill-shape limits are build tests (A-D7).** "I feel like these should be turned into tests."
   The build fails on a built `SKILL.md` over 500 lines; a resource over 100 lines without a contents
   list; a resource reached from another resource that is over the short-file limit; a resource no
   spine links to. Appendices stay under 100 lines, the preview size, so any read is a whole read
   (A-D8). The limits are Anthropic's
   [skill authoring guidance](https://platform.claude.com/docs/en/agents-and-tools/agent-skills/best-practices),
   not dydo axioms; the tests cite it so they can be revised when it changes.
10. **The builder is a repo tool (A-D9).** "c". `dotnet run --project tools/SkillBuild` (name
    provisional) builds every built file. The model and its builder stay out of the shipped `dydo`
    binary, so the guard's sub-agent allowlist doesn't gate it and a crew code-writer that edits the
    model rebuilds with plain `dotnet`. No build-time writes to tracked files.
11. **The working-tree standard and the Linear standard stop being must-read skills (A-D10, A-D12).**
    Docs acting as skills: "Yes they do. And that's antipattern." Every rule an agent acts on in them
    becomes a claim, built into the spines and appendices of the roles that carry it. No role has
    them as a must-read. They become documentation, the big picture, and the built rules are rendered
    into them. They keep shipping: "Yes they do ship. 100%. Dydo is customizeable, but opinionated."
    One build emits both the `Scaffold/dydo/` and the `dydo/` copy. Appendices name them as the source
    to read when something breaks or the user asks, and the Linear-setup move names the Linear
    standard. The principle for every built skill, in his words: it shows the system "from the
    perspective of the agent working in it with just the right amount of knowledge about the other
    parts so they can interact with them well."
12. **The why lives in one why doc per claim kind, and it isn't shipped (A-D11).** "these big why-s
    probably shouldn't be shipped with dydo." Each why doc shows every role's line for each claim
    side by side, with its why and sources, and links to the charts its claims build. It lives under
    `dydo/` only, outside every skill folder and `Scaffold/`. `dydo check` covers its links.
13. **The order (A-D13).** "Yeah, I think B is good." (1) "Current skills get translated into
    claims." (2) The builder, "exactly the one which puts the outputs into the skill folders",
    generates the skills, and Balazs looks at them: "They will be horrible at first." (3) The skills
    are restructured by state and routine. (4) "Then we'll build in all the new things we want",
    model them "and make sure all of it holds up". The whole claim-made set is translated first, not
    one family. The builder is the only writer of built skill folders. The release's rule changes
    (fifth-FAIL losing its teeth, the shoulds collection, sessions B to D) are written once, in step
    4, into claims; nothing is rewritten in
    today's prose first. The memory cap is not modelled: it stays prose until DYTEST lands and is
    deleted then. Step 4 is where the generated moves meet the hand-written properties. His review
    sits after step 2 and at the end of step 4.
14. **Step 1 is a claim inventory, not a faithful translation (A-D14).** "the outcome should be to
    make the system better and not to make the translation perfect." Every rule today's skills and
    system docs state becomes a claim, drifted copies side by side. Sonnet-class crew checks it
    sentence by sentence; done when nothing is unmapped. What it turns up feeds steps 3 and 4. No
    permanent faithfulness test.
15. **Downstream gets built skills in one ordinary update (A-D15).** "So I think option A is the
    one." The restructure ships as one release of built `SKILL.md` files and runtime resources, with
    an upgrade note: swap the old skills for the new ones, update the tool. Downstream gets neither
    the model nor the builder; project-specific content stays in the project-owned docs (glossaries,
    `about.md`, `architecture.md`, `coding-standards.md`, `testing-strategy.md`), hand-written and in
    place. After that release the builder stays line-stable (one claim per bullet, no reflow). No
    migration aid; no dual layout.
16. **Officers brief crew from built dispatch templates (D-D13).** "a". One template per crew kind,
    built from the same claims as that crew's skill, as a short resource of the officer's dispatch
    routine. Its slots: ticket and contract; branch, worktree and base; owned paths; gates; proof
    recipe; findings (Fix only); model and effort; return to. The crew loads its own skill; the
    officer never reads crew skills to brief them. `handoff` is for continuing a conversation in a
    fresh thread only. The envoy template is one of these templates.
17. **The doctrine lives in built skills, so any vendor can hold any role (D-D4).** Roles, moves and
    rules live in the built skills, never in a host's coordinator feature.
18. **Claim edits are his (F10).** Skill prose is claims, and claims change only with his approval:
    the model's PRs need his approval, not only review. Agents prepare.
19. **A claim need not cite a principle (L1 sitting, A1).** The moves in skill prose are not part of
    the law; a rule outside the law *may* name the principle it serves, but nothing requires it
    ([DYD-1078 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1078#comment-9ac278c0-00f3-4688-9971-2693bfc14e38)).

## Admiral defaults

Grilling A left three small packet items for the admiral to default and flag (A, Open). Each follows
the packet's recommendation.

- **Provenance sidecar.** Each built skill folder carries `claims.map.json`, mapping built line to
  claim id. It costs no model tokens. Charts cite `claim:<id>` instead of `file:line`, which shifts
  on every edit. *admiral default; Balazs to confirm.*
- **Stray-claim lint.** A lint greps hand-written text for claim fingerprints (such as `MemoryMax=`,
  "consecutive FAIL", `t3_thread_launch`) outside built sections, so a copy pasted back by hand is
  caught. It sits beside the golden test (decision 5) and the chart cross-check (decision 1).
  *admiral default; Balazs to confirm.*
- **Claim slots in general skills.** General skills such as diagnosing-bugs and verify-dydo stay
  hand-written and may carry a built slot where a shared claim lives. Their main shared claim, the
  memory cap, is not modelled (decision 13) and goes with DYTEST, so a slot exists only once another
  shared claim needs one. *admiral default; Balazs to confirm.*

## Consequences

- One rule has one home, so drifted copies of a rule can no longer arise; a rule broken by another
  claim, or a missing move, shows in the chart as a broken property or a deadlock (grilling A's packet, "The insight").
- The statechart stays an independent check, because its properties are written by hand.
- Review of doctrine moves to the built-skill diff, and every doctrine PR waits on his approval.
- `ticket-captain`'s seven up-front must-reads shrink, since the standards stop being must-reads.
- About 4-5k tokens are saved per dispatch on the officer side (D-D13).
- Open: how redundancy is modelled as a property ("every halt is observed by a second role") belongs
  to step 4, when the properties are written (A, Open).

## Supersedes and amends

Amends [DR 049](./049-skills-are-the-source-retire-the-compiler.md) §1 for the claim-made set: the
model is the source, and the built skill folder is where the skill is read. DR 049 §1 stands for every
other skill, and its §2 (host distribution), §3 (the compiler stays retired) and §4 (local downstream
edits, no reconciliation promise) stand (A-D2).

## Related

- [DR 049 - Skills Are the Source](./049-skills-are-the-source-retire-the-compiler.md)
- [DR 065 - Review Placement and the Merge-Review Cut](./065-review-placement-and-the-merge-review-cut.md) - Rules written once, through claims
- [DR 066 - The Law, Councils and the Tribunal](./066-the-law-councils-and-the-tribunal.md) - Gardening authority (F10)
- [Working-Tree Standard](../../guides/working-tree-standard.md)
- [Linear Workspace Standard](../../reference/linear-workspace-standard.md)

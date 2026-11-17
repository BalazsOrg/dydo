---
area: project
type: decision
status: proposed
date: 2026-11-17
applies: [dydo-review-high-impact, dydo-verification-independent-evidence, dydo-aligned-intent, dydo-efficiency, dydo-effort-right-agent, dydo-effort-right-sized-bureaucracy, dydo-scarce-attention, dydo-attention-human-decisions]
weighed:
  - "dydo-verification-independent-evidence over dydo-efficiency, because \"as soon as we put a review into the picture, the captain would become not an impartial piece\" (D-D10)"
---

# 065 - Review Placement and the Merge-Review Cut

The feature is the review unit, reviewed once when it can be observed, and every review also checks
the change against the governing spec's problem statement. A conflict-free merge into the feature
branch is proved by deterministic gates and hosted CI, with no merge review; only the landing keeps
one. The captain runs the Merge hop itself. Only irreversible PRs and release tags need Balazs's
click. Decided by Balazs in
[grilling B](https://linear.app/bodnar-balazs/document/370-grilling-b-review-placement-and-the-merge-review-cut-f9088dfe7680)
on 2026-11-11 (B1 to B10), as superseded in part by
[grilling D](https://linear.app/bodnar-balazs/document/370-grilling-d-hierarchy-t3-inbox-captain-autonomy-sub-captains-a5cb8f0b5a86)
(2026-11-13 to 15), plus his spec-anchored review rule of 2026-11-16
([DYD-1120](https://linear.app/bodnar-balazs/issue/DYD-1120)). Recorded under
[DYD-1085](https://linear.app/bodnar-balazs/issue/DYD-1085); awaiting his approval (F10). The
grillings say 3.7.0; that release is 4.0.0. Every rule is written once, through claims
([DR 064](./064-claims-build-the-skills.md), A-D13).

---

## Context

In 3.6.0 the per-ticket merge review caught no tree defect the deterministic gates would have
missed. Agents read all 88 Merge Sub-tickets and the landing: 19 merge-review FAILs on 18 tickets.
15 were about evidence or process (a gate record on the wrong SHA, a gate still pending, a receipt
deleted, the tip moved), 4 were red or flaky gates the gates had already flagged, and 0 were defects
only a reviewer could see. The real seam defects at merge time were caught by CI or the merge
writer. Review pays where judgement meets intent: the whole feature, a hand-made conflict
resolution, the landing.

Two more facts shaped it. The fifth consecutive FAIL stop broke the 3.6.0 statechart, and reviews
that judged only their ticket's contract let a wrong inherited rule land: DYTEST's host admission ran
one run per project per machine
([DYD-1111](https://linear.app/bodnar-balazs/issue/DYD-1111), below).

## Decision

### Where review sits

1. **The placement principle (B4).** "Review when it has a high impact (early enough so incorrect
   stuff does not get built upon) and late enough so that the system can be observed." It is now the
   principle `dydo-review-high-impact`, and it replaces "Review where work merges, not at every hop".
2. **The feature is the review unit, and it is a ticket (B4).** "the key is that the feature is
   already tied to a ticket." A feature built in parts is one parent ticket; its parts are
   sub-tickets or lanes with no review of their own. The parent is reviewed once, after the parts are
   wired together, before its Merge hop into the feature branch. The feature review judges the
   whole: each part correct, the parts correct together, and in harmony (one home per contract and
   model, no duplicated DTOs, efficient flow across the seam, deep modules). Every merge into the
   feature branch still carries an independent PASS, and `dydo merge` needs no new verdict kind.
   **As superseded in part by D-D11:** no sub-captains until the T3 nesting experiment reports;
   meanwhile a ticket too big for one captain is cut into sibling tickets with their own captains,
   under a parent the admiral holds, and the admiral orders that parent's review once its parts are
   in; a fresh reviewer holds it.
3. **Every review is spec-anchored (DYD-1120).** Balazs, at the DYTEST walkthrough: "the new dydo
   should patch this loophole and failure mode that we drift from the spec, because only the ticket
   spec is compared to."
   - Every review also checks the change against the governing spec's problem statement and goals,
     including behaviour a ticket inherits unchanged. A review brief names the governing spec, or
     says there is none (DYD-1120, "What 4.0.0 makes true" §1).
   - Tests that pin an untraced inherited rule are findings, not proof. A test that pins a behaviour
     traces to the spec or a DR (§2). On tests that lock in inherited behaviour: "This is also a
     trap."
   - Before landing, the integrated product is driven against the problem statement, the way its
     users will run it. Gate checks alone don't count (§3).

   **Case study, [DYD-1111](https://linear.app/bodnar-balazs/issue/DYD-1111):** host admission runs
   one run per project per machine, so parallel agents queue behind each other on an idle machine.
   The DYTEST spec's problem statement asks for speed and capacity for many parallel runs. The rule
   came in with [DYD-472](https://linear.app/bodnar-balazs/issue/DYD-472) (single-machine fairness
   between projects), right for that contract, and
   [DYD-754](https://linear.app/bodnar-balazs/issue/DYD-754) carried it into the network unchanged.
   Each review judged its own contract; tests pinned the rule as correct, so reviewers saw green tests
   confirming it; the inquisition and landing reviews checked gates, not behaviour against the problem
   statement. Nobody ran parallel runs on a live network until his walkthrough, where it surfaced in
   20 minutes (DYD-1120, "Case study"). His words, in DYD-1111: "This is a serious violation of the
   design intent."
4. **No early contract review; a spec scout asks what the spec doesn't answer (B4a).** "I've had
   some bad experiences with agents reviewing plans 10 times." **First make it work, then make it
   good:** "First you make it work. Then you make it good. The reviews are supposed to catch the
   problems." No review loop runs on a plan, spec or contract. A spec gets a **scout pass**: one read
   for fundamental problems and questions the spec doesn't answer, returning Questions, never a
   PASS or FAIL, and never repeated; the answers refine the spec. The feature must line up with the
   spec as refined. After acceptance the spec is deleted, so it can't drift, and the code becomes the
   source of truth. Recorded in the grilling as defaults, not asked: the scout pass is the captain's
   call by risk, and it replaces today's optional spec review.
5. **The author proves; the reviewer judges through lenses (B4b).** The author hands off only what it
   has verified, with receipts. A fresh reviewer judges through lenses: intent, correct together,
   harmony, module boundaries. **Nothing is re-run the same way:** the reviewer doesn't re-run the
   author's tests or re-drive the same path; a result DYTEST already holds for the same state counts
   as proof. "doing the exact same thing (like driving the app the same way) is just stupid. Unless
   there were enough changes down the line that the truth is questioned." **Redundancy comes from
   downstream re-exercise:** later work exercises earlier features as part of its own job, and any
   agent that finds a problem outside its scope reports it to its officer, which routes it to the
   owner's ticket or a new Bug. "What matters is that it surfaces." Deep modules first: get a
   module's inside right, then review and test how modules interact.
6. **Review climbs the ticket tree with a shifting distribution of lenses (B5).** "I think of these
   lenses as a distribution of what we care about. On the deepest level it has to work. Then it has
   to integrate well with the other parts. Then it has to be optimal, secure and all the others."
   - Deepest: it works, with a security baseline; performance only where it matters. Next: it
     integrates. Top: optimal, secure, performant, in harmony. Size and kind of code tilt the mix; the
     officer names the lenses in the reviewer's brief.
   - A **leaf** sub-ticket or a lane gets no full review: the author proves it works, and the officer
     may send a fresh evidence check, a seat of its own, never the officer reading the evidence itself.
   - A **parent that integrates parts** is reviewed once its parts are in, at its level's lenses; the
     captain may skip it when the parent only groups. The ticket that merges into the feature branch
     gets the full review (decision 2).
   - **Each level checks for itself:** "the same evidence should not be accepted, because that
     defeats the purpose of redundancy."
   - **Don't review a first version as if final:** "Why spend reviewing the first version like it's
     the final version?"
   - **A statechart only for a finished subsystem** that has a state machine (B4b, B5).
   - The same move at every level; keep the rule short: "agents can handle a lot."
7. **The review loop: a fix between rounds, blockers routed by kind, a checkpoint after round 3
   (B6).** "Okay I like this. ... Most of the time one or two review rounds should clear the
   blockers."
   - A fix between every two rounds; never re-review an unfixed version. The reviewer reports every
     blocker it can see in one pass.
   - A review phase ends on the first round with no new blocker. Shoulds go to the should collection;
     nits stay one line on the Review.
   - A round with a blocker always gets a following round after the fix, a fresh look at the level's
     lenses, not only at the fix.
   - Each blocker is routed by kind when found: broken or contradicting the spec → Fix hop, next
     round; a security hole → Fix hop, next round with the security lens first; fundamentally broken,
     or a new requirement or prerequisite → the officer above re-plans (re-cut the ticket or refine
     the spec); needs Balazs (authorise a library, run a command, set up infrastructure, an
     irreversible decision) → escalate up the chain at once while the rest keeps moving.
   - After round 3, a checkpoint, not a stop: the officer running the rounds decides on purpose.
     Default: one more round; escalate only for the last two kinds. It is not named or framed as a
     FAIL stop.
   - **As refined by D-D9 and D-D15:** a captain or sub-captain never reaches Balazs on its own; its
     need goes to the officer above, which decides whether he is needed and, if so, prepares the
     decision first. A hard call goes to the council
     ([DR 066](./066-the-law-councils-and-the-tribunal.md)).
8. **Seats and models scale with the work (B7).** "Each review round's seats should depend on the
   breadth and depth of the work. And we should also choose models wisely."
   - No constant pair. A small leaf may need no review; if it gets one, a quick check that it is what
     it says it is. Several seats buy speed and are used dynamically.
   - Model tiers by job, today's examples in his words: check tier (evidence and behaviour checks)
     Sonnet, Luna; normal tier (ordinary reviews, lower and middle levels) Opus 5.5, GPT 6.2 Sol;
     strong tier (non-obvious discovery, the late whole-system review, "can this be simplified")
     Fable, Astra. The late whole-system review is Musk's "simplify" step.
   - Cross-vendor (a different model family) is preferred when available, "so that things don't stop
     just because I ran out of a usage limit."
   - **One model table** that Balazs owns, semi-hard-coded in one easy-to-edit place, carried by every
     dispatching officer and updated whenever a model ships or changes. Grilling F names it the model
     roster.
   - **A usage-limit command** (name provisional, `dydo usage`) tells every dispatching agent how much
     each vendor account has left and when it resets, so agents balance vendors themselves.
9. **A captain implements only what is too small to need a review of its own (D-D10).** "this should
   really be for small stuff. So small stuff that it doesn't even need review." A ticket captain may
   take a step itself only when briefing a writer would cost more than doing it and the change is
   simple enough that the review at the level above covers it. Admirals and sub-captains don't
   author. If a step needs its own review round, the captain does not implement it. Risk reasons
   (persistence, migrations, permissions, guard rules, uncertain native interfaces) always need review.

### The merge cut

10. **A conflict-free merge into the feature branch is proved by gates alone (B1).** "the first two
    can be caught deterministically and the third should be caught by reviews downstream." The code
    is broken → the combined gates on the merge SHA and hosted CI on the feature push. A merge
    conflict → detected by git and `dydo merge`. What the code does and how → the reviews before and
    after (the feature review, the inquisition, the landing), not a per-merge review.
11. **A merge review stays only at the landing; a conflict is resolved on the ticket branch (B2).** A
    conflict is never resolved on the feature branch: the feature is synced into the ticket branch,
    and the resolution gets a fresh review scoped to its hunks before `dydo merge` takes it. A merge
    is **hand-shaped** when its tree differs from git's automatic merge of its parents
    (`git merge-tree --write-tree p1 p2` against `<merge>^{tree}`); `dydo merge` refuses a hand-shaped
    merge into the feature branch and names the fix: re-sync onto the ticket. No merge review by
    size, and none for lane merges. The landing keeps its merge review, which proves acceptance;
    what follows it is decision 19.
12. **The merge is a hop sub-ticket with no review under it (B3).** "The point is that it's simple."
    Every merge into the feature branch is a Merge hop sub-ticket under its parent: Todo → In
    Progress → Done. Done means the merge landed and the gates and hosted CI are green on the merge
    SHA. The landing's merge review is the only Review under a merge. Merge order stays on the Merge
    hops' blocking relations.
13. **The captain runs the Merge hop itself (B3a).** "a. the captain runs it (it's should be dead
    simple) and if there is a conflict they will be right there do task a sub agent to fix it and land
    it)." The captain runs `dydo merge` and the post-merge gates (`dydo test gate`). It dispatches a
    code-writer only for a `CONFLICT` (decision 11's re-sync, then the captain lands it) or a real red
    gate (decision 14). Running the command is not authoring. No interim rule before DYTEST: "3.7.0
    ships dytest." **B's diagram resolved:** grilling B's B1 and B2 roles and the diagram it showed
    have a merge code-writer run `dydo merge`; B3a, and D-D16 after it ("B3a's captain-run Merge
    stands"), say the captain does. B3a and D-D16 win.
14. **A real red gate after a merge reverts the merge at once (B8).** DYTEST reruns once. A flake:
    the Merge hop is Done and a Bug is filed for the flaky test. Real red: the captain reverts the
    merge on the feature branch at once; a code-writer fixes it on the ticket branch; a fix-scoped
    round follows; the ticket lands again, and `dydo merge` reverts the revert when the same branch
    lands again.
15. **Children make an officer; the cutter decides (B8a).** "I think this is a good rule. And it cuts
    bureaucracy as well." A ticket with child tickets has an officer, who dispatches and does not
    write code (D-D10's exception aside); a leaf ticket or sub-ticket has crew. The officer above
    decides, when it cuts, whether a unit gets children; an agent learns its role from its brief and
    skill and never promotes itself; a crew agent whose job is too big returns "too big, re-cut". A
    Linear unit goes by `delegate_task`; a native sub-agent never gets one and shares its holder's
    checkout, and the holder commits. Parallel lanes and sub-tickets with children get their own
    worktree; serial hops share their ticket's. **As superseded in part by D-D11:** no sub-captains
    yet (decision 2).
16. **`dydo merge` is the one entry point and runs every pre-merge check (B8b).** It refuses unless
    the PASS names the head, the branch merges cleanly with the feature tip, and the gates are green
    on that combined tree; a refusal states its reason. After the merge, the existing hosted CI on
    push to `feature/**` runs on the merge SHA. No extra workflow writes to Linear.
17. **A PASS on an earlier commit stands when every later commit is a clean sync from the target
    (B9).** `dydo merge` accepts a PASS on an ancestor of the head when every later commit is a merge
    whose tree equals the automatic merge of its parents and whose second parent is already in the
    target. A hand-shaped sync needs a fresh review.
18. **DYTEST owns the gate receipts, keyed by tree (B10).** "I like (a)." `dydo merge` asks DYTEST
    for a green receipt on the combined tree before it lands; the merge commit's tree is that tree,
    so nothing runs twice. The Merge hop is Done when DYTEST holds a green receipt for the merge
    commit's tree and hosted CI is green on the merge SHA. No `--verify` flag; dydo reads no receipts
    itself.

### Which merges need Balazs

19. **Only irreversible PRs and release tags need his click (D-D14, D-D16).** "me clicking merge is a
    bottleneck and reducing that bottleneck is probably a good idea." "Only irreversible PRs should
    need my click." "That I'm not the one who has to do the proving, but the agents can do it
    reliably."
    - **The agents prove the code; Balazs sets the course.** Walkthroughs are his product review and
      his look at the code.
    - **A two-way PR merges to master AFK** when its evidence is complete: an independent PASS on the
      head, or on an ancestor when every later commit is a clean sync (decision 17); gates green on
      the combined tree with current master; CI green; the PR body declaring two-way. **The captain
      merges it.** This covers an atomic ticket's PR and a Project's landing PR; a Project's own
      tickets still merge into its feature branch first, which is the admiral's.
    - **One-way** means reverting the commit does not undo the effect. These always need his click:
      data migrations; publishing a package or a public API; anything that sends or publishes outside
      the repository; secrets; history rewrites; changes to what guards main (CI, branch protection,
      release workflows, the guard). Paths flag them where possible; otherwise the captain declares.
      A one-way PR carries a recovery plan in its body; revert applies to two-way changes only.
    - **Release tags are his,** per Project.
    - **Pacing:** main-bound PRs re-check against current master just before landing; the admiral
      lands its Project's main-bound PRs one at a time; a PR forced to re-sync a third time is held
      and the admiral sequences the colliding work. Recorded as a default, not asked: across Projects,
      `dydo merge` re-checking the combined tree with master is the serialiser.
    - **The landing:** "a". Merge review, then his walkthrough acceptance, then AFK when two-way; a
      one-way landing still needs his click.
    - **Every weed becomes a rule:** a mistake found after landing is fixed with a lint, test or rule
      too, so it can't land again.
    - Before his click, the captain watches its own CI in-turn and opens the HITL Merge only when
      green; nobody watches for the click; the survey sees "PR merged, HITL Merge open" and wakes the
      captain; a tick that finds nothing ends in one line and wakes no one (D-D14).

## Consequences

- Counted on 3.6.0, about 88 merge-writer dispatches become captain commands and about 150
  merge-review seats become deterministic checks. The 15 class-C FAILs (receipts on the wrong SHA) become
  DYTEST's receipt query.
- `dydo merge` gains code: the clean-merge check and gates on the combined tree before landing, the
  hand-shape refusal, the ancestor PASS after clean syncs, the DYTEST receipt query, and
  revert-of-revert. DYTEST must key receipts by tree and answer that query
  ([DYD-741](https://linear.app/bodnar-balazs/issue/DYD-741)).
- The reviewer stops re-running the author's gates. Review rubrics, the inquisitor's Drive phase, the
  captain's review brief and the landing step gain the spec-anchored checks.
- Most two-way PRs into master land without him; his attention goes to walkthroughs, one-way
  merges and tags. How an agent merges into the default branch feeds
  [DYD-944](https://linear.app/bodnar-balazs/issue/DYD-944).
- Still open in grilling B: where the spec lives and what "deleted" means for it.

## Supersedes and amends

- Supersedes [DR 045](./045-flow-map-hats-review-tiers-and-working-tree-contract.md) §3.2, the merge
  review after every merge (B1, B2).
- Supersedes [DR 047](./047-supersymmetry-hop-statuses-merge-issues-and-the-release-protocol.md) §6,
  merges as Issues each with its own merge review and the human's click on the landing (B1 to B3a,
  B8, D-D16).
- Supersedes [DR 058](./058-review-severities-blocker-should-nit.md) §2's two seats on every first
  round (B7) and its fifth-FAIL stop (B6).
- Amends [DR 061](./061-agents-merge-only-into-feature-branches.md): the merge review after a
  feature merge goes (B1); the captain runs `dydo merge`, a code-writer only for a conflict or a red
  gate (§6, B3a); a PASS on an ancestor stands after clean syncs (§4, B9); `dydo merge` checks the
  combined tree and DYTEST's receipt before landing and refuses a hand-shaped merge (B2, B8b, B10);
  and a merge into the default branch is no longer always the user's click: two-way PRs into master
  merge AFK (§1, D-D16).

## Related

- [DR 064 - Claims Build the Skills](./064-claims-build-the-skills.md) - Every rule here is written once, through claims
- [DR 066 - The Law, Councils and the Tribunal](./066-the-law-councils-and-the-tribunal.md) - Review councils and hard calls
- [Review Principles](../../law/dydo/review.md) - `dydo-review-high-impact`
- [Working-Tree Standard](../../guides/working-tree-standard.md)

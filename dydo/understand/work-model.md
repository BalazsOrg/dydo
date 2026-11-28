---
area: understand
type: concept
---

# Work Model

How work moves here: Linear owns the live work graph, Git and dydo own knowledge, feature specs and proof,
and every session places itself on one flow map before it acts. The stages, the roles that hold them,
and the reviews that prove them follow
[Decision 047](../project/decisions/047-supersymmetry-hop-statuses-merge-issues-and-the-release-protocol.md),
which amends Decisions 045 and 046.

## The flow map

A session keeps the role it was opened or spawned with and loads skills as the work needs them; an
officer never becomes another officer ([Decision 050](../project/decisions/050-officers-crew-and-skills-hats-retired.md)).

| Stage | Who | Output | Gate |
|---|---|---|---|
| Think | any session, loading co-thinker | ripe intent, and a Decision Record when the choice earns one | - |
| Chart and plan a Project | the user creates it; the map holder, the coordinator with a captain on a Wayfinding ticket, charts it with wayfinder (S14; DYD-1195 b46e8343) | the map in the Linear Project, first tickets, Questions | user approval of the charted route |
| Make a ticket exact | ticket-captain, through its compact acceptance contract | a just-in-time contract with no hidden implementation decisions; the route stays the writer's | none; a spec gets at most one scout pass, which returns Questions (R4) |
| Implement | ticket-captain | a ticket branch, a PR into the feature branch, or into main for an atomic ticket, evidence on the ticket | reviewer PASS |
| Coordinate | the coordinator, mechanically (H5) | captains in flight, dead lanes restarted, merges paced; each judgement routed to a fresh agent's ticket | each merge into the feature branch: the combined gates on the merge SHA and hosted CI, with no merge review (R18, R20) |
| Inquisition | a ticket captain with the inquisition-captain resource, on the ticket the coordinator opens (N5, H5) | a feature map, each slice read, broken with proof tests and driven through the app, each lens by its own reviewer; Fix tickets, registered shoulds and a nit ticket; rounds from zero until clean (N5, S9, S11) | the Project lands only after a round with no blocker and no should (N5) |
| Walkthrough | a ticket captain on the Walkthrough ticket, with the user, at each Milestone and on the landing candidate (H15, H23, N6) | the product inspected as it stands, his verdict or skip, findings as tickets in the same Project | the walking skeleton's verdict or skip holds back the next Milestone's tickets; the landing merges only once he accepts its candidate (N6) |
| Land | the coordinator paces it (H5) | the feature branch, with master merged in, merged into main (N6) | a merge review, then his acceptance at the landing walkthrough, then an AFK merge when two-way or his click when one-way (N6, R26, R27) |

Some work has no stage of its own: chief-of-staff triages the user's attention and never delivers,
and any session may reach for papercut, writing-for-agents and diagnosing-bugs; retro is the user's
harness improvement, read from the papercuts, and bro his corrective for agent-speak, at any stage.

## Officers, crew, and skills

- **Officers** hold something and never switch: the coordinator holds a Project, the ticket-captain a
  ticket, the chief-of-staff the board (H1, H4). A session or agent has at most one officer role; the user
  holds a row on the map but no role.
- **Envoys** are ticket captains on HITL Question, Grilling, Walkthrough, Wayfinding and Enablement
  tickets, each loading that job's skill (H15, H23; DYD-1082 1bc04dce change 10).
- **Crew** are spawned for one bounded job, hold nothing, and return their result to whoever sent
  them. The kinds are implementer, reviewer, research and scout (DYD-1082 1bc04dce change 3).
  Research pools and verifies, delegating single lookups to non-authoring scouts; other crew do their own bounded work.
- **Methods** are plain skills - reference and procedure a session applies inside its own thread, never a separate
  session.
- Some methods load only when the user asks for them; `handoff` also when an officer briefs an
  agent it dispatches. For each Milestone's tour and the landing's, the coordinator opens the
  Walkthrough Ticket and the ticket captain holding it loads `walkthrough` (H15, H16, H23;
  [DR 060](../project/decisions/060-milestone-walkthroughs-and-the-walking-skeleton.md)).
- **User commands** are invoked by the user typing their name, and by nothing else.

The [dydo glossary](../reference/dydo-glossary.md) names every member of each category. Every one of them is a native skill folder - see
[Scaffold and Customization](./scaffold-and-customization.md).

A captain's ticket is a Project one level down: the same Types, statuses and chain, with a different
map holder. The statechart, `claims/statechart/README.md` from the repository root, draws every
state and arrow (K1). The board is the inbox; the coordinator wakes on a captain's return, on the
word of the user or the chief of staff, or on its survey loop (DYD-1082 555a8300).

## Canonical ownership

| Concern | Canonical home |
|---|---|
| Initiatives, Projects, tickets, optional Milestones and Cycles | Linear |
| Status, priority, assignee, dependencies, updates, review state | Linear |
| Decisions, guides, temporary feature specs | dydo/Git |
| Audit, inquisition, migration, and assimilation evidence | dydo/Git |
| Release tags and changelog | Git |
| FutureFeatures and their promotion state | Linear |

dydo has no Linear poller, webhook receiver, or Markdown mirror. Its one Linear client is read-only
and serves two commands that use the user's `LINEAR_API_KEY`: `dydo map`, a view that keeps
disposable saved maps ([DR 052](../project/decisions/052-dydo-map-read-only-linear-view.md)), and
`dydo survey`, a lane digest that keeps nothing
([DR 063](../project/decisions/063-dydo-survey-read-only-lane-digest.md)). Agents reach Linear through
its official MCP, UI, API, and integrations.

## Reviewed intent

No implementation begins without a contract another agent can review independently.

- One atomic ticket can be its own reviewed contract. Shared multi-ticket behavior may use a [feature spec](../reference/feature-spec-convention.md) ([DR 053](../project/decisions/053-temporary-feature-specs.md)); a Project's own spec is its `Spec: <Project>` Linear document, which the map holder writes and refines (S14).
- An implementation ticket records governing Decisions and applicable feature-spec commit and
  sections before execution; a review pins the ticket's
  contract as its description as of a Linear `updatedAt`.
- The two planning resolutions, the fields a ticket must carry, and the Question Ticket that clears
  fog are in the [Linear Ticket Lifecycle](./task-lifecycle.md).

Branches, worktrees, sessions, crew, commits, PRs, and review passes are execution evidence linked
to a ticket. They are not extra levels in the work graph.

## Review and inquisition

1. **Ticket review** - a fresh reviewer with the rubric the candidate targets, code or docs, before
   merge. A plan, spec or contract gets no review loop; a spec gets at most one scout pass, which
   returns Questions, never PASS or FAIL (R4).
2. **Merges** - a Merge hop has no review under it: it is Done when DYTEST holds a green receipt on
   the merge tree and hosted CI is green on the merge SHA (R18, R20). A two-way PR into master
   merges AFK, by its captain, once its evidence is complete; a one-way one waits for the user's
   click (R26, R27).
3. **Inquisition** - held by a ticket captain with its inquisition-captain resource, on the ticket
   the coordinator opens (N5, H5). The captain draws a feature map from the landed work, and
   reviewers run the Read, Break and Drive lenses on each slice, each lens by its own reviewer,
   Break proving with its own tests and Drive through the project's verification skill (N5;
   DYD-1082 1bc04dce change 5). At the verdict, blockers become a Fix ticket, shoulds are
   registered, and nits go on the round's nit ticket (S9, S11). Rounds repeat from zero until one
   finds no blocker and no should
   ([DR 059](../project/decisions/059-inquisition-rebuilt-area-map-three-phases-rounds-until-clean.md), N5).
   It catches what got through; it never proves zero defects.
4. **Landing** - the only merge review left: a Merge-confidence council on the landing candidate,
   then the user's walkthrough acceptance, then an AFK merge when two-way or the user's click when
   one-way (R20, L16, N6).

Every reviewer verdict is the same **review block**: on its Review sub-ticket, with a one-line PASS naming that Review ticket in the PR body (R17); its one-line form is locked in the
[Linear Workspace Standard](../reference/linear-workspace-standard.md#communication-and-evidence). Independence here is independence of *context*: the
reviewer arrives fresh and reads the candidate itself rather than the story told about it. Every
finding carries a [severity](../reference/dydo-glossary.md), `blocker`, `should` or `nit`, and only
a blocker FAILs; a should becomes its own ticket the Project's landing waits on, and a nit one line
on the record. Fixes are deferred, never dropped
([DR 058](../project/decisions/058-review-severities-blocker-should-nit.md)).

Reviewer and scout are non-authoring roles: their commissions and methods produce
verdicts, findings or source passages, never candidate edits. A reviewer on the Break lens commits
only proof tests, to a proof branch that never merges. This is a methodological boundary, not
a claim that every host enforces a read-only filesystem. The portable floor is a fresh commission,
an isolated pinned candidate and an independent review; a native sandbox request strengthens that
floor only when a live check establishes its enforcement.

## References and evidence

Use a branch-following GitHub URL for current human navigation, and an exact commit permalink for a
governing contract or historical proof. The ticket branch carries its Linear ticket key, so Linear's
GitHub integration attaches branch and PR to the ticket natively; the
[Working-Tree Standard](../guides/working-tree-standard.md) is the procedure. Durable knowledge
discovered during work is extracted to a Decision, guide, audit, or assimilation brief
rather than left in a comment thread or a session transcript.

## FutureFeatures

A FutureFeature is an unscheduled strategic possibility recorded as a Linear ticket, not a generic
idea or delivery contract. It stays in `FutureFeature` until the user promotes or cancels it. The
[Linear Workspace Standard](../reference/linear-workspace-standard.md) defines its status and
promotion paths.

## Related

- [Linear Ticket Lifecycle](./task-lifecycle.md) - what a ticket carries, and how it is claimed and merged
- [Working-Tree Standard](../guides/working-tree-standard.md) - branches, worktrees, claims, cleanup
- [Decision 045 - Flow Map, Hats and Workers, Review Tiers, and the Working-Tree Contract](../project/decisions/045-flow-map-hats-review-tiers-and-working-tree-contract.md)
- [Decision 044 - Linear-Canonical PM and the dydo Knowledge Boundary](../project/decisions/044-linear-canonical-pm-and-dydo-knowledge-boundary.md)
- [dydo Glossary](../reference/dydo-glossary.md) - the locked vocabulary
- [Writing Good Briefs](../guides/writing-good-briefs.md) - how a brief is written

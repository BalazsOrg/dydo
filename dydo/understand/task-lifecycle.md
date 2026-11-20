---
area: understand
type: concept
---

# Linear Ticket Lifecycle

A ticket is the only actionable work item, and Linear owns every field on it that moves. This is what
a ticket carries, how it is planned, claimed, reviewed and merged, and the user's gates. The file keeps its historical path so durable links still resolve.

## Two kinds of ticket

An **implementation ticket** carries five required fields - outcome, owned paths, blockers, exact
gates, base branch - plus the relevant context, and it links the governing Decisions and
applicable feature-spec commit and sections. Use a
Sub-ticket only when the child needs its own status, owner,
dependency, or review evidence; a checklist is enough for mechanical steps that cannot progress
independently.

A **Question Ticket** (Linear Type `Question`, body under `## Question`) is an open question that
blocks planning or implementation and is too big or too uncertain to settle inline. The rule is *fog →
discovery → Question Ticket*: an agent in fog first runs a bounded discovery - the Decision index, the
feature specs, the ticket's own links, the glossary, the code - and only when that comes up empty does
the question become a ticket that records what was searched and blocks every named plan or
implementation ticket awaiting its answer. A crew member raises a hand to its captain. Whoever
receives a passed-up need opens a Question for it and waits on it: a Question Sub-ticket under a
captain, a Project-level Question under the coordinator. A fresh captain holds the Question and
researches it first, then answers it, sends it to a council (a Question with Mode AFK), or puts it
on the coordinator's HITL queue when it is worth the user's time (H16, H18, L18; DYD-1195 317eb49f,
3a854e37, 87c1e2e6). Facts are the agent's job; a call reaches the user only when it needs him.

Its resolution is an *answer* posted on the ticket. The answer graduates to a Decision Record only when
it is hard to reverse, surprising later, and the result of a real trade-off. Tickets carry questions,
Decision Records carry decisions, and the two are linked rather than copied.

## Planned at two resolutions

- **Project** - low resolution: the Linear Project and its tickets are the plan, charted by the map
  holder with `wayfinder`: destination, the first pickable tracer-bullet tickets, and fog written
  down in `## Not yet specified`, with blocking Question Tickets instead of a pretended complete
  route. The user approves the charted route; changes to destination, scope, acceptance
  criteria, or governing architecture return for user approval. Shared multi-ticket behavior may use a
  [feature spec](../reference/feature-spec-convention.md); reconcile affected contracts when intent shifts.
- **Ticket** - high resolution, just in time: the Ticket Captain's compact acceptance contract on the
  parent ticket or a direct lane Sub-ticket, with its scenarios where the ticket carries Gherkin, owned
  paths and gates, exact enough to build. The spawned `implementer` builds from it. A contract gets
  no review loop of its own; a spec gets at most one scout pass, which returns Questions (R4).

## Claimed, isolated, executed

Assignment is the claim: nothing else marks a ticket as taken. From there the ticket has one branch and
one worktree, one writer inside it, and commits that touch only the paths the ticket owns. The
[Working-Tree Standard](../guides/working-tree-standard.md) is that procedure end to end - how the
branch is named, what goes on the ticket before the first edit, and what is cleaned up after the
merge.

Linear owns the ticket's status, priority, assignee, blockers and updates throughout. The branch,
worktree, session, commits, PR and test runs are evidence for that ticket, never additional work
records.

Every delivery kind starts with the captain making its contract exact. Linear has seven statuses
(S5); a step with its own review loop shows its progress in hop sub-tickets: Implement, Review (one
per round), Fix and Merge. The writer
posts nothing before code unless the work splits into disjoint lanes or the contract is inexact;
its IMPLEMENTED return carries the hop SHA, the red proof, the gates run and any blocker. A
scenario changes only through a fresh author's fix hop. Every hop's SHA is posted, preserved and passed to the next crew member.

## Reviewed, then merged

The [Work Model](./work-model.md#review-and-inquisition) holds review, merges, the inquisition and
the landing; the review block's one-line form is locked in the
[Linear Workspace Standard](../reference/linear-workspace-standard.md#communication-and-evidence).
In short:

- A fresh reviewer judges the wired parent once, before its Merge hop, and the verdict goes on its
  Review hop (R2, R17).
- A FAIL's blockers become Fix rN, an `implementer`'s fix hop with those blockers as its contract;
  a fresh full-lens round follows (R8, S9).
- After round 3 comes a checkpoint, not a stop: by default one more round (R10).
- A merge into the feature branch has no review: its Merge hop is Done on a green DYTEST receipt on
  the merge tree and green hosted CI on the merge SHA (R18, R20).
- Only the landing keeps a merge review (R20, N6).
- The captain files each should as its own ticket and records each nit as one line
  ([severity](../reference/dydo-glossary.md)).

## Raising a hand

The ladder runs crew → its Ticket Captain → commissioning officers → user. A sub-captain
raises a hand to the captain above it; a top-level captain reaches the coordinator (H12). Agents
settle operational conflicts themselves by precedence, highest first: the user's live instruction, a
Decision Record, the feature spec (pinned), the ticket contract,
coding standards, existing code. The user
is reached only for a conflict with a Decision Record - is it truth, or is it obsolete? - for live
external state agents cannot coordinate, or for authority the contract cannot supply.

A crew member raises its hand by returning to its Ticket Captain; a comment on the ticket and, when
the work is blocked, a Question Ticket wired as a blocker are the captain's rungs. The implementer's
one pre-code comment naming lanes or an inexact contract is its scoped exception, not a hand-raise.
A blocked captain releases: post the resume SHA, push the branch, clean the worktrees it or its crew
made (lanes, merge checkouts, temp checkouts) with `dydo clean --dry-run <worktree>`, then
`dydo clean <worktree>`, run from outside that tree, return the parent to Todo and unassign. The
worktree its thread runs in belongs to T3: leave it. A user takeover does the same; a dead session leaves its
last recorded hop without a final push. The coordinator's next wake picks up blocker-cleared work (H5).

## Where the user is asked

Project route approval; Questions and HITL work; an escalation that survived the ladder; each
Milestone's Walkthrough, where only the walking skeleton's verdict holds back the work built on it;
and the landing's Walkthrough, which he accepts before the landing merges (N6).
A Milestone's findings become tickets in the Project as they arrive; a landing he does not accept
turns its findings into tickets in the same Project, and another landing follows (N6). A two-way PR
into main, an atomic ticket's or the landing's, merges AFK by its captain once its evidence is
complete; a one-way one waits for his click (R26, R27).
The [workspace standard](../reference/linear-workspace-standard.md) owns the Type/Mode and priority rules.

Linear status is the only delivery status. dydo does not copy it into frontmatter, infer it from Git,
or poll Linear.

## Related

- [Work Model](./work-model.md) - the flow map, ownership, review and inquisition
- [Working-Tree Standard](../guides/working-tree-standard.md) - branches, worktrees, claims, cleanup
- [Writing Good Briefs](../guides/writing-good-briefs.md) - how a ticket's contract is written
- [dydo Glossary](../reference/dydo-glossary.md) - the locked vocabulary

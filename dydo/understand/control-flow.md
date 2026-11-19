---
area: understand
type: hub
---

# Control Flow

Every handoff in the dydo 3 operating model, drawn from [DR 045](../project/decisions/045-flow-map-hats-review-tiers-and-working-tree-contract.md),
[DR 046](../project/decisions/046-executable-specifications-specifier-and-commit-addressed-hops.md),
[DR 047](../project/decisions/047-supersymmetry-hop-statuses-merge-issues-and-the-release-protocol.md),
[DR 050](../project/decisions/050-officers-crew-and-skills-hats-retired.md),
[DR 051](../project/decisions/051-captains-by-default-admiral-on-invocation.md)
and the [Linear Workspace Standard](../reference/linear-workspace-standard.md): who acts, on which
branch, what leaves them and through which channel, what the other side reads, and what happens when
the happy path breaks. It is the shared truth both sides of every contact must agree with. Section 7
shows a working day with three Projects in flight; section 8 lists what each skill must change to
match.

## How to read it

Node colours name the kind of actor; edge labels name the payload. The payload's channel and the
fields on both sides are in the edge table, section 5.

| Colour | Kind | Meaning |
|---|---|---|
| gold | user | the one person; opens every session that talks to him, acts at the gates |
| blue | officer | holds a Project, a ticket or the board and never switches; the map holder at its level |
| grey | session | a session without an officer role, loading a skill such as co-thinker |
| green | crew | spawned for one bounded job, returns to its spawner, never talks to the user |
| red | reviewer | a crew member whose return is the review block: one rubric, one binding verdict |

Channels: **L** a Linear field, comment or status · **G** a commit, branch or PR · **R** an agent's
return to its spawner, or a message to a returned one · **F** a file in the repository · **C** the
conversation with the user.

A Project's **map holder** is the user, in their own session, unless the user has invoked an
admiral. Where the flows below name the admiral, they draw an invoked one; without it, the user
takes that step, and nobody needs telling.

## Contents

- [1. Roster](./control-flow-roster.md#1-roster)
- [2. Skills that are not actors](./control-flow-roster.md#2-skills-that-are-not-actors)
- [3. The happy path](./control-flow-happy-path.md#3-the-happy-path)
- [4. States](./control-flow-states.md#4-states)
- [5. Edges: the contract table](./control-flow-edges.md#5-edges-the-contract-table)
- [6. Exceptions](./control-flow-exceptions.md#6-exceptions)
- [7. A day with three Projects](./control-flow-day-and-prompts.md#7-a-day-with-three-projects)
- [8. Prompt-file propagation - DYD-90](./control-flow-day-and-prompts.md#8-prompt-file-propagation---dyd-90)

## Related

- [Work Model](./work-model.md) - the flow map, officers and crew, reviews and inquisition this map expands
- [Linear Ticket Lifecycle](./task-lifecycle.md) - what a ticket carries and how it is claimed and merged
- [Working-Tree Standard](../guides/working-tree-standard.md) - branches, worktrees, hops, cleanup
- [Linear Workspace Standard](../reference/linear-workspace-standard.md) - statuses, Types, Mode, templates
- [dydo Glossary](../reference/dydo-glossary.md) - the locked vocabulary
- [DR 047](../project/decisions/047-supersymmetry-hop-statuses-merge-issues-and-the-release-protocol.md) - the decisions this map draws

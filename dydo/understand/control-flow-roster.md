---
area: understand
type: concept
---

# Control Flow: Roster and Skills

[Control Flow](./control-flow.md)

## 1. Roster

The actors: every officer and every crew role, plus the co-thinker skill any session loads to think
with the user, keeping its role. The ticket-captain is the default officer; an admiral exists only when
the user invokes one, as a right hand for throughput that never takes the frontline. An officer is
top-level when a session is opened with it; a captain is also spawned as an agent, so the user's
session or an invoked admiral can direct one per ticket.

| Role | Kind | Runs as | Invoked by | Works in | Does | Returns to |
|---|---|---|---|---|---|---|
| user | user | the terminal | - | `main`, where his own commits need no ticket, and any session | thinks, files Projects, holds a Project's map by default and charts it with `wayfinder`, approves plans, answers Questions, confirms inquisitions, clicks the landing one Project at a time and an atomic ticket's reviewed PR, walks through; tells an invoked admiral after each of his board moves, and takes the map back at any time | - |
| co-thinker | skill | any session, keeping its role | any session with an unripe idea | no branch; DRs and glossary on the current branch | homework, grilling, domain-modeling, recommendation; a DR when the ADR test passes; an atomic ticket with its five fields, Type and Mode | a DR (F), a Project through `to-project` (L), an atomic ticket (L) |
| admiral | officer, optional | top-level session, explicit-only | the user, only when he wants a right hand for throughput, on a Project at any stage | `feature/<slug>` once the map holder opens one at Open; none before that, or on a Project with no feature branch | holds the map until the user takes it back; wakes on a captain's return, the user's word or its scheduled check-in, reads the Project and acts: charts it with `wayfinder`, puts the charted route to the user for approval, commissions the first captain to open the feature, commissions captains, wires the merge order and re-wires it when a later PR is ready first, sets priority on what waits on the user, runs its wayfinding with the user, prepares each Milestone's walkthrough, proposes the inquisition, files the landing and its walkthrough, closes | the user in its own session (C); the board (L) |
| ticket-captain | officer, the default, also agent | top-level for an atomic or HITL ticket; spawned by the user's session or an invoked admiral for an AFK one | the user's session, or an invoked admiral | `DYD-123-<slug>` in an isolated worktree; `inquisition/<slug>` for an inquisition | claims, takes an adjacent ticket only on the user's word, sets the status at every chain spawn, directs [code-writer] → [reviewer] on the parent or each lane and adds a spec review only on a risk its contract records, divides when the writer names lanes, sets `Ready to Merge` when the PR carries its PASS, runs its Merge Sub-tickets or, on an atomic ticket, waits for the user's click, cleans up | the spawner: `done <key>` or `released <key>: <reason>` (R); everything else on the record (L) |
| chief-of-staff | officer | top-level session, explicit-only | the user | none | the bird's-eye view over the Projects in flight: the three lists, grilling open Questions, mediating collisions, sweeping stale state and orphans | the user (C); delivery staged on its Project for the map holder (L) |
| research | crew, delegates, web | agent | co-thinker, the map holder, ticket-captain | reads | one fact a choice waits on, cited; sends scouts | the invoker: one-line answer, destination, unsettled points (R); report as ticket comment (L) or scratch file (F) |
| code-writer | crew | agent | ticket-captain, on the parent first, then per lane | the ticket branch; a Merge Sub-ticket; a proof branch off the brief's pinned source SHA; a patch-lane Bug branch off an Inquisition's audit SHA | one ticket's contract on the `implement` hop, test-driven by Type: an unused-member Bug its definition and a reference search, any other Bug red before its fix and red again with the fix reverted; a Feature's acceptance lines as named tests written with the code, proved to fail with the code stashed; each hop proved by the testing strategy's [Feature proof and gate placement](../guides/testing-strategy.md#feature-proof-and-gate-placement), HCRAP and cognitive complexity fixed in changed methods. Posts nothing before code unless it names lanes or an inexact contract. On a Merge: the merge commit and its resolutions. Proof-only: the test that would catch one hypothesis, committed as `<KEY> proof: <hypothesis>` | the captain: the `IMPLEMENTED` line with hop SHA, behaviour, red proof, gates run and blocker (R); for a hypothesis, `confirmed`, `not reproduced` or `inconclusive` |
| docs-writer | crew | agent | ticket-captain, an inquisition's captain included | the ticket branch, an Inquisition's record branch included | one documentation change with a witness per claim; an inquisition's record from its pushed findings register | the captain: ending SHA, files, witnesses, `dydo check`; for a record, its path and digest (R) |
| reviewer | crew, non-authoring | agent | the map holder, ticket-captain | reads a pinned candidate | one rubric: code, docs, spec, merge | the invoker: the review block (R), posted on the record and in the PR body (L, G) |
| scout | crew, non-authoring, web | agent | research | reads one source family | passages back, no conclusions | research (R) |
| inquisitor | crew, non-authoring | agent | an inquisition's ticket-captain | Read and Drive: a worktree at the audit SHA; Break: its proof branch `proof/<KEY>-<area>-<n>`, one per dispatch, test paths only | one phase, Read, Break or Drive, on one area, refuting its own catch; past its context limit it hands the rest of the area back | the inquisition captain: findings, sightings and not-reproduced hypotheses in the fixed return shape; Break's red tests at their SHAs (R) |

For these three roles, non-authoring names the commission and method; an inquisitor's Break phase
commits only proof tests, to a proof branch that never merges. It does not assert filesystem
enforcement by the host: a native sandbox setting is a request until a live host check proves the
restriction. Fresh commission, an isolated pinned candidate and independent review are the portable
floor.

## 2. Skills that are not actors

Methods run inside the caller's thread and leave nothing of their own; some load only when the
user asks, and user commands run only when the user types them.

Agent-invoked methods:

| Method | Reached by | When | Leaves behind |
|---|---|---|---|
| wayfinder | the map holder: the user's session, an invoked admiral, or a ticket-captain | charting, writing the contracts one level down, working the map, clearing local fog | the map in the Project description; tickets and Sub-tickets, wired |
| grilling | co-thinker, chief-of-staff, any Grilling Ticket | a plan, decision or idea the user wants stress-tested, one round at a time | answers and reasoning recorded where the work lives |
| domain-modeling | co-thinker, wayfinder | a term keeps sliding, or a choice looks durable enough for a DR | `dydo/glossary.md` entries; a Decision Record |
| codebase-design | code-writer, reviewer | shaping a module or interface, choosing a seam, judging depth | vocabulary applied, nothing written |
| diagnosing-bugs | the code-writer reproducing a Bug, on the parent or its retained reproduce-or-identify Sub-ticket | a Bug other than an unused member without a red reproduction | a tight loop that goes red; the regression test; the cause on the ticket |
| prototype | the code-writer on a Prototype Ticket | how it should look or behave is the open question | `prototype/<name>`, never merged; kept and linked from the ticket until the delivery ticket is `Done`, read by the delivery ticket's code-writer as the template for a fresh rewrite, never a base or a copy (DR 047 §5) |
| wizard | the captain on an Enablement Ticket | steps only the user can perform: credentials, dashboards, cutovers | a bash wizard that walks him through them |
| writing-for-agents | anyone editing a skill or a document an agent reaches by pointer; reviewer(docs) | a prompt file is created, edited, or fires wrong | the edited file |
| retro | any session; chief-of-staff routes recurring friction to it | the same friction or workaround returns a second time | one small, testable harness-change suggestion, with proof and rollback |
| maintain-verification-skill | any agent driving the app through a project's verification skill; the map holder before a landing; `code-writer`, `reviewer`, `ticket-captain` and generated `verify-<app>` skills | work must be proved, a drive contradicts the feature map, a touched area has no map file, or the whole map needs an audit | `clean`, `changed` with one PR of proven map or harness corrections, or `blocked`; product regressions reported as Bugs |
| maintain-formal-verification-skill | any agent or the map holder after merges, refactors or edits touched verified code without rechecking the verification | verified code changed since the verification's sync commit, or a verify skill's check passes on code that has since moved | each check `in sync`, `updated`, `bug` or `blocked`, with drift and code bugs told apart and verification updates kept in a change of their own |

Methods loaded only when the user asks:

| Method | The user asks when | Produces |
|---|---|---|
| to-project | a co-think is ripe and belongs in Linear | a Project in `Backlog`: title, summary, the intent as description, links to the DR, the glossary entries and the source FutureFeature when one exists; no tickets |
| to-ticket | a plan, spec or conversation is ready to become tickets | pickable tracer-bullet tickets in `Todo`, each with one Type, a Mode where a captain holds the Type, and the five fields or its Type's own template, wired with native blocking relations |
| handoff | the session is ending and another agent continues; also loaded by an officer briefing an agent it dispatches | a handoff document in the scratch directory |
| walkthrough | a Walkthrough Ticket is open; also loaded at the map holder's Milestone and landing steps | the four-part tour of the whole product as it stands, newest changes first: what changed, where to look, how to try it, what reviewers flagged or deferred |
| teach | the user wants to learn a topic in the workspace | a mission and its learning records |

User commands:

| Command | The user types it when | Produces |
|---|---|---|
| grill-me | a plan or idea of theirs should be pressed | answers and reasoning, recorded by the session in play |
| bro | an agent's pitch did not land | the same thing said plainly, with the two glossaries in hand |
| improve-codebase-architecture | the codebase's architecture should be examined | a grilled candidate with its report |
| create-verification-skill | the project has no scripted way to drive the app as a user | a proven `verify-<app>` skill, its control CLI and a seeded feature map |
| create-formal-verification-skill | code where a bug costs the most and tests reach the least needs more than tests: concurrency, retries, state machines, parsers, unsafe code | a project-local skill that checks a change against one verified property, proven to catch a scratch break |
| statechart | the user wants to read a checked TLA+ model as states and shortest bug paths | a Mermaid chart, plain-language report and raw TLC output from the same checked model and sync commit; above 500 states, only bug paths are drawn |
| update-dydo | the project should move to the latest dydo release | the project on the new release, its tweaks kept, the update proven and adopted |

The explicit-only officers admiral and chief-of-staff are also typed by the user; they are actors and
stand in the roster.

## Related

- [Control Flow](./control-flow.md)

<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Linear Workspace Standard

## Contents

- [contract](#contract)
- [decision](#decision)
- [dispatch](#dispatch)
- [escalation](#escalation)
- [evidence](#evidence)
- [identity](#identity)
- [inquisition](#inquisition)
- [linear](#linear)
- [overnight](#overnight)
- [project](#project)
- [return](#return)
- [review](#review)
- [walkthrough](#walkthrough)
- [wayfinding](#wayfinding)

## contract

- | `Bug` | the five fields, then `## Observed`, `## Expected`, `## Reproduction` (a scenario at the boundary, else the red test; a prose Bug quotes both contradicting passages at `file:line`; an unused member, export or field quotes its definition at `file:line` and a reference search showing no caller outside the tests); default Sub-tickets: *reproduce or identify*, *fix*. |
- | `Enablement` | `## Condition` that must become true, `## Steps only the user can do`, `## Evidence` when done. |
- | `Feature` | `## Outcome` · `## Owned paths` · `## Blockers` · `## Exact gates` · `## Base branch`, plus optional `## Spec` linking relevant feature-spec sections: the captain's bounded acceptance contract; `## Exact gates` names the command for each test, suite and static gate. |
- | `Grilling` | `## Subject` (the plan, decision or idea), `## Tree` of choices with their answers and reasoning, `## Records` linked. |
- | `Inquisition` | `## Scope`: the commission inputs - the audit SHA, the integrated feature commit and the landing base, the governing intent, a provider and model per phase and the swarm width, the Codex cap, the crew context limit, and the verification skill or why nothing runs; `## Area map`, drawn fresh each round; `## Findings`, the register's link and each finding's disposition: Bugs, patches, the nit ticket, Questions; `## Record` path. The round's nit ticket is a `Feature`, Mode `AFK`, `Backlog`, held by the map holder and linked to its Inquisition; it is listed at the Project's final walkthrough, where the user decides each nit, and closes `Done` once every nit is fixed or accepted as is. |
- | `Merge` | `## Source` and `## Target` at their SHAs, `## Plan order` (the Merge Sub-ticket this one is blocked by), `## Combined gates`, `## Conflicts expected`. |
- | `Prototype` | `## Question` the sketch must settle, `## Variants`, `## Verdict` and the winning commit on `prototype/<name>`. |
- | `Research` | `## Question` and `## Destination` for the cited report. |
- | `Walkthrough` | `## Checkpoint` (the Milestone or the landing, its branch and SHA), the four-part tour, `## Verdict` (his words, or the skip), `## Findings` as linked tickets. |
- The Bug Type template is a narrow exception to parallel delivery lanes: its captain may retain ordered reproduce-or-identify and fix Sub-tickets, with fix natively blocked by reproduction and shared paths transferred only after reproduction closes with its evidence recorded.
- Joining acceptance and final review stay on the parent; the exception creates no overlapping parallel ownership.
- Each stage keeps the parent's Type and Mode and has its own contract, chain, branch and worktree; every actual integration has a Merge Sub-ticket.
- For a simple Bug, collapse the staged placeholders into parent hops and close the unused records `Canceled` with the reason.
- | `Enablement` | captain | any | Access, environment, credentials or material other work needs; `wizard` guides the steps only the user can do. | the condition true, with evidence | `#26B5CE` |
- Name unavailable support and explicitly reconcile older active obligations; no gate is cancelled retrospectively by a new default.
- A captain creates Sub-tickets one level deep: lanes for separate work that can run at the same time, each carrying its parent's Type and Mode, a Merge Sub-ticket for each merge operation except an atomic ticket's own merge into main, which the user clicks once its PR carries a reviewer PASS and CI is green, and a map-holder-held Sub-ticket for fog that blocks only its parent or a lane and stays inside the approved Project destination and ticket outcome.
- A lane that needs splitting is replaced by sibling lanes.
- The map holder writes the contracts one level down, charts with `wayfinder` before dividing, and runs the map-holder-held tickets that clear its own fog: it dispatches `research`, runs a Grilling or Walkthrough with the user, and files a `Question` only when judgment remains.
- The captain records one compact acceptance contract and points to its evidence.

## decision

- `DECIDED - by: what-would-balazs-do tribunal, unratified; result: <action>; vote: <n of 3, or split; most reversible>; log: <link>; undo: <steps>; next: <action>.`
- A Question a `what-would-balazs-do` tribunal decided while the user was away, or on his own invocation, stays in `Todo` unwired, and work proceeds; he answers it with `RATIFIED` or `OVERTURNED`.
- `RATIFIED - by: <the user> <date>: "<his words, verbatim>"; record: <DECIDED or DR link>; next: <action>.`
- `OVERTURNED - by: <the user> <date>: "<his words, verbatim>"; record: <link>; reopened: <tickets>; next: <action>.`

## dispatch

- A captain-held ticket normally uses one author - `code-writer`, or `docs-writer` for a documentation change - and review at the working-tree standard's [Delivery scale](../guides/working-tree-standard.md#delivery-scale-and-evidence).
- State the effective identity when trustworthy runtime information exposes it.
- Otherwise qualify a family or a requested/configured value; never infer an alias resolution, parent model, or effort.

## escalation

- | `AFK` | The captain reaches reviewed completion without a live conversation. An unexpected choice for the user becomes a blocking `Question`; the parent stays AFK. | `#30A46C` |
- If the answer determines which implementation ticket should exist, resolve the Question first.
- When the answer can change other tickets, a shared contract, or the Project's destination, scope, acceptance criteria or governing architecture, the captain prepares the packet and the Project's map holder creates and wires the Project-level ticket.
- A Question is the last step of discovery, not its substitute.
- Before creating one, search the governing Decision Records, Project plan, specifications, documentation, standards, code, tests and other authoritative sources; if they settle it, record the answer where the work lives and continue.
- Create the Question only when the user's judgment is still needed, in `Todo`, wired as a blocker of everything that waits.
- Its body carries: the one question; the named work it blocks and why; the sources searched and facts found; the credible options, trade-offs, and the recommendation when evidence supports one.
- | `Question` | the four items above. |

## evidence

- Before expensive tests, record cheap proof of repository or snapshot, intended selection and nonzero discovery; a quiet healthy test continues.
- List hop proof separately from final, combined-merge and release gates.
- Required G/M, integration and release gates remain.
- Review gates stay where their workflows require.
- Keep existing historical originals accessible.
- Put new full raw proof and native gate logs once in a durable, linked Git or document artifact; do not dump them into PRs or comments.
- `## Exact gates` binds the [testing strategy](../guides/testing-strategy.md#feature-proof-and-gate-placement) to real project commands: selected own/reached features and parent-shared tests, Light/Heavy levels, prerequisites and stage, applicable static/mutation assurance, final coverage producer/consumer, and permitted evidence reuse.
- Each gate result names its candidate, command, environment or session, exit and result location.
- Reuse exact-candidate evidence only when the gate and environment permit it, rerunning after relevant changes or new concerns and at mandatory integration or release boundaries.

## identity

- The map holder is the user, or an admiral the user invoked, for a Project, and the captain for a ticket; the Project description names the Project's current holder.
- A map-holder-held ticket is run directly by the map holder whose map it clears; it receives no captain, branch, PR or review loop.

## inquisition

- | `Inquisition` | captain | primary; its group Sub-tickets above about 10 areas | One round of audit of the integrated feature: an area map, each area run through Read, Break and Drive by `inquisitor` crew, every finding triaged by severity into Bugs, one nit ticket and Questions. Each round starts from zero. | its record merged and verified on the feature, Bugs filed and their proof refs retained; a group Sub-ticket, its register final and pushed | `#5E6AD2` |
- | `Inquisition` | `Backlog` → `Todo`, the user's confirmation → `In Progress`, the commission inputs, area map, phases, triage and patch lane → `Implementing` → `In Review` → `Ready to Merge` → `Done`, its record's docs chain. It releases to `Todo` with `released <key>: verification skill first`, resuming once the skill exists, or with `released <key>: groups <keys>`, resuming `In Progress` once its group Sub-tickets, each `Todo` → `In Progress` → `Done`, close; follow the [working-tree standard](../guides/working-tree-standard.md#retaining-an-inquisitions-record-and-proofs) |

## linear

- AFK tickets carry no priority: the plan order and the native blockers carry their sequence.
- Assignment is the claim: assigned means taken; unassigned, `Todo` and unblocked means pickable.
- A captain relay preserves the original author or reviewer identity and distinguishes the captain's posting identity.
- Every ticket carries exactly one Type.
- Mode sits on every Type a captain holds.
- Every agent-authored Linear comment identifies its acting role and truthful model identity.
- Filter tool output before it enters model context.
- A ticket born from a finding carries that finding's severity, by DR 058's names; whoever files the ticket sets it.
- | `FutureFeature` | backlog | An unscheduled strategic possibility with no Type yet. Only the user promotes it. |
- A `FutureFeature` is promoted by the user alone: at ticket grain it gains a Type and moves to `Backlog` or `Todo` on the same record; at Project or Initiative grain that record is created and linked and the FutureFeature is marked `Done`.
- A rejected one becomes `Canceled` with the reason recorded.
- Historical comments stay untouched.
- | `In Progress` | started | A record not running the chain itself: a parent while its lanes run, a wayfinding ticket, an Inquisition's sweep (its map, phases, triage and patch lane) until its record enters the chain. |
- Keep the stable current contract and state in the description.
- `Ready to Merge` holds for a lane into its parent, a primary into the feature branch, and an atomic ticket or the landing into main; a Merge Sub-ticket never enters it, since it runs the chain and closes.
- | Merge Sub-ticket | `Todo` → `Implementing` → `In Review` → `Done`; it merges, it is never merged |
- | `HITL` | The user and the captain work the ticket together in a session the user opened. | `#F76B15` |
- Native blocker relations connect every waiting record to what it waits on.
- Name an agent without its `@` unless you mean to delegate: an `@mention` starts that agent's session.
- A ticket with an open native blocker is blocked in any status; do not add a `Blocked` status that can drift from Linear's dependency graph.
- The captain alone sets a delivery ticket's status; the record that runs the chain flips on every chain spawn, and nothing else flips it.
- | `Todo` | unstarted | The incoming list: contracted and to be started soon. An open native blocker still prevents pickup. A `Question` in `Todo` is the user's turn. |
- | `Ready to Merge` | started | The PR carries its PASS block and waits for its merge turn. The record stays here while its own Merge Sub-ticket runs; the landing, one Project at a time, and an atomic ticket wait here for the user's click. A merge review FAIL that reverts sends it back to `Implementing`. |
- | `High` | Clearing it lets a top-level ticket, or a Project's next step, run AFK. |
- | `Low` | Nothing runs AFK when it clears; the user must clear it anyway. |
- | `Medium` | Clearing it lets a lane run AFK. |
- Priority keeps its meaning and never carries severity.
- | `Urgent` | An emergency; never set by default. |
- | `Backlog` | A possible Project retained for later; no one has started charting it. |
- | `In Progress` | The map holder is working the map through Ticket Captains toward the destination. |
- | `Planned` | The user approved the charted route, and any repository plan passed independent review; the map holder commissions the first Ticket Captain to open the feature. |
- | `Planning` | The map holder is charting the first low-resolution map with `wayfinder`; a repository plan, when one is written, runs its review loop. |
- There are no canonical Project labels: a Project's status and ticket graph carry the information.
- Read comments only for a named missing fact or binding review; comments have no assumed order.
- Use metadata first: the current active contract and named evidence only.
- The reviewer field in a reviewer block is its signature, so do not add a second or conflicting one.
- | `Implementing` | started | The author of the change's kind is spawned for one hop - a `code-writer`, or a `docs-writer` for a documentation change - a fix hop after a FAIL included. |
- | `In Review` | started | Any reviewer is spawned, the optional spec review of the contract before any code included. A FAIL, one or more blockers, returns the record to `Implementing`; a spec review returns it there on either verdict. |
- `In Review` on the board always means a reviewer is running.
- | `blocker` | The intended case is wrong. | each blocker Bug an inquisition files | `#E5484D` |
- | `nit` | Everything else. | each inquisition round's nit ticket | `#8B8D98` |
- Severity sits only on a ticket born from a finding.
- Most tickets carry none.
- | `should` | A real bug callers will hit, inside the intent. | each should ticket filed from a review; each should Bug an inquisition files | `#FFB224` |
- Small lookups, conversations and setup stay on the ticket that needs them.
- Create a map-holder-held ticket only when the work needs independent tracking or blocks other work.
- State effort only when it is known and useful.
- | `Backlog` | backlog | Retained with a Type, unscheduled, waiting to become a Todo: no contract yet, or one awaiting the user's go, as an Inquisition's. |
- | `Canceled` | canceled | The ticket will not be completed; the record says why. |
- | `Done` | completed | Merged, or the outcome the Type names reached, with its evidence. |
- | `Duplicate` | canceled | Another ticket owns the outcome; the record links to it. |
- | captain-held default, subject to the Type exceptions | `Todo` → `Implementing` → `In Review` → `Ready to Merge` → `Done`, with `In Progress` while lanes run |
- | `Grilling`, `Walkthrough` | `Todo` → `In Progress` → `Done` |
- | `Question` | `Todo` → `Done` |
- | `Research` | `Todo` → `In Progress` → `Done` |
- The order of the statuses is part of the standard: Linear draws each status circle from its position in its category.
- One set for primary tickets and Sub-tickets alike, ten statuses in Linear's categories and in this order.
- `Task` names the captain-held ticket's role on a map; it is not a label.
- Three label groups, `Type`, `Mode` and `Severity`, because one label per group is the rule Linear enforces.
- Create it from the body below where the connected tools or UI permit.
- | `Bug` | captain | any | Restore intended behaviour. The record holds the defect and its fix. | the behaviour restored | `#EB5757` |
- | `Feature` | captain | any | Add, improve, refactor or document: an outcome to build. | the outcome merged | `#BB87FC` |
- | `Grilling` | map holder | any | A tree of intent or specification choices resolved with the user. | shared understanding recorded, with its Decision Records linked | `#D4A017` |
- | `Merge` | captain | any; the landing is the only primary one | One merge operation: lanes into a parent, a primary into the feature, the feature into main. | the merge review PASS | `#4EA7FC` |
- | `Prototype` | captain | any | A design question raised to fidelity the user can react to; fast sketches, the user is the review. | the user's verdict on the ticket | `#F2994A` |
- | `Question` | map holder | any | One prepared, discrete question whose answer blocks named work. | the user's answer on the ticket | `#F2C94C` |
- | `Research` | map holder | any | A factual answer whose investigation needs its own owner, status or evidence. | cited findings on the ticket | `#95A2B3` |
- | `Walkthrough` | map holder | primary only | The user walks the whole product as it stands: the feature branch at a Milestone, or main at the landing. The emphasis is on what changed since the last walkthrough and what needs the most careful look. The tour also covers where to look, how to try it, and what reviewers left open. There's one per Milestone, prepared even if he skips it, and the landing's comes last. | his verdict quoted on the ticket, or his skip recorded; findings filed as tickets | `#C69C6D` |
- Agents read this standard and the ticket contract, then use the capabilities actually available; template listing or retrieval is not a required connector capability.
- The user's approval or inspection at a universal gate does not make a ticket HITL.
- The map holder sets it on every ticket that waits on the user, a `Question` or a HITL ticket, and re-sets it as blockers change.

## overnight

- In overnight mode it blocks nothing, and later Milestones' Walkthrough Tickets never block.

## project

- | `Canceled` | The destination was consciously abandoned; record why and commission [spec retirement](./feature-spec-convention.md#retirement). |
- Every delivery ticket belongs to one, and each Milestone has one Walkthrough Ticket.
- | `Completed` | The destination landed and the landing's walkthrough found nothing more; commission [spec retirement](./feature-spec-convention.md#retirement). |
- The landing check is a query: no open `blocker` or `should` ticket in the Project, unless the user explicitly deferred it.
- A Project's map holder places its Milestones in Linear while charting: few, each a meaningful chunk of work where a drift from the user's intent would show, the walking skeleton first and the landing last.

## return

- `DECIDED - result: <result>; immutable contract: <contract>; next: <action>.`
- `FAIL - rubric/type: <rubric>; contract <ref>; candidate <SHA>; base <SHA>; reviewer/model: <name>; gates <N/N>. 1. Where: <place>; severity: <blocker|should|nit>; wrong: <fact>; why: <impact>; required correction: <action>; owner: <role>. Evidence: <link>.`
- `IMPLEMENTED - hop/candidate <SHA>; <behavior>; proof: <evidence>; blocker: <none or named blocker>.`
- Brevity is soft: ordinary events aim for at most 800 characters, descriptions and state for at most 1,500 characters or 12 lines.
- A necessary finding, gap, blocker, or evidence may exceed those aims; omit narration and repetition either way.
- Start each post with the applicable form:
- Ordinary updates expose only counts, exits, failure excerpts, and an evidence link.
- Do not repeat contracts, hashes, full returns, passing-test name dumps, unchanged updates, or token, cost, or latency claims.
- `PASS - rubric/type: <rubric>; contract <ref>; candidate <SHA>; base <SHA>; reviewer/model: <name>; applicable tests <N/N>, gates <N/N>. Checks: <one substantive sentence>. Deferred (optional): 1. Where: <place>; severity: <should|nit>; wrong: <fact>; why: <impact>; required correction: <action>; owner: <role>. Evidence: <link>.`
- `STATE - <status>; owner: <role>; candidate or PR: <reference>; next or blocker: <fact>; evidence: <link>.`

## review

- A review blocker stays a FAIL and never becomes a ticket; a review nit stays one line on its ticket's record.
- A FAIL carries at least one blocker; a PASS carries none and lists its should and nit findings under `Deferred`.
- A writer's successful delivery is never an independent `PASS`.
- The captain files each should as its own ticket and records each nit as one line on the record.
- A review block's `contract <ref>` is the contract its brief pinned, for a ticket `<KEY> description as of <Linear updatedAt>`; `base <SHA>` is the brief's base.
- A spec review of the contract before any code needs one short concrete risk reason, recorded in the contract; persistence, migrations, permissions and uncertain native interfaces are examples of such a risk.
- Ticket-and-PR binding review stays compact but complete.

## walkthrough

- Only the walking skeleton's holds back the work built on it: filed with the map, it is a native blocker of the next Milestone's delivery tickets until his verdict or skip closes it.

## wayfinding

- Linear records the decision-making work; dydo records a qualifying decision.
- Link the Question or Grilling Ticket to its repository Decision Record and link the record back when it has a Linear origin.
- The Decision Record is canonical: keep its rationale in dydo rather than copying it into Linear.

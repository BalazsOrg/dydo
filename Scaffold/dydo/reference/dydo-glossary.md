---
area: reference
type: reference
---

# dydo Glossary

The locked vocabulary for dydo's Linear-native work model, durable knowledge, and agent execution.
Project-domain terms belong in the separate [glossary.md](../glossary.md).

## Linear work graph

- **Initiative** - an optional workspace-level goal spanning independently meaningful Projects.
- **Project** - a bounded product or technical outcome owned by one Linear team. The Linear Project
  and its tickets are its plan.
- **Milestone** - a meaningful checkpoint inside a Linear Project, a chunk of work where a drift
  from the user's intent would show. The map holder places few; every delivery ticket belongs to
  one, and each has a Walkthrough Ticket. The first is the **walking skeleton**, the rough
  foundation the Project is built on; the landing is the last.
- **Ticket** - A ticket is a step on the Project map toward the destination, sized to what one agent can handle within its context.
- **Task** - the role a captain-held ticket plays on a Wayfinder map; a name for that role, not a
  separate Linear Type.
- **Sub-ticket** - optional decomposition when child tickets need independent tracking.
- **Lane** - an ordered run of tickets one owner moves forward in a Project; it serves the Project's
  deliverables. The lane ledger is retired (S25). Distinct from a Lane Sub-ticket, a captain's parallel child ticket.
- **Filler** - small, ready work, such as review `should` tickets, that an officer takes into a lane
  when the next core ticket is blocked, toward a soft goal of four occupied lanes, not a cap (H10).
- **Cycle** - an optional team capacity timebox, orthogonal to Projects.
- **Label** - restrained cross-cutting routing metadata, never a shadow work-type hierarchy.

## Fog and Question Tickets

- **Fog** - the unknown unknowns: relevant uncertainty not yet sharp enough to state as a question.
  The rule is *fog → discovery → Question Ticket*. Search the Decision Records, the feature specs, the
  ticket's own links, the glossary, and the code first; file only what that leaves open.
- **Question Ticket** - a Linear ticket of Type `Question` carrying one open call, resolved into an
  **answer** on the ticket. Whoever receives a passed-up need opens it: a Sub-ticket under a
  captain, a Project ticket under the coordinator. A fresh captain holds it and researches first,
  then answers it, sends it to a council, or puts it on the coordinator's HITL queue when it is
  worth the user's time (H16, H18, L18; DYD-1195 317eb49f, 3a854e37, 87c1e2e6).
- **Council** - a Question ticket with Mode AFK, chaired by a ticket captain with its council-chair
  resource; the deliberation sits on the ticket next to the answer (L16, L23; DYD-1195 ea16ea28).
- **Tribunal** - the council's Judge procedure: three members testing one call against the user's own
  words, the officer that raised it and two fresh judges, one per vendor (L18, L23). Its output
  is **unratified** and becomes precedent only when the user answers it `RATIFIED`.
- **Frontier** - the open, unblocked, unassigned map tickets: the edge of what a Project knows.
  Linear's own blocking relations and assignment render it; there is no separate navigation object.

## Knowledge and contracts

- **Axiom** - a truth or value the law builds on; not questioned or second-guessed; it changes only if the world it assumes changes.
- **Principle** - a value judgement that follows from an axiom or a broader principle, and can lose to another value in a given case.
- **Case law** - the DRs and case studies that show how the law was applied.
- **Case study** - a curated git record of a Question, grilling decision or idea that shaped the law. Often, but not always, a post-mortem. It is an origin story that links its source.
- **Decision** - a choice recorded in a Decision Record because it is hard to reverse, surprising
  later, and the result of a real trade-off. Tickets carry questions, Decision Records carry
  decisions, and the two are linked rather than copied.
- **Feature spec** - a temporary current anchor for shared multi-ticket behavior under
  `dydo/reference/working-specs/<project-slug>/`; the [spec convention](./feature-spec-convention.md) owns its lifecycle.
- **FutureFeature** - an unscheduled strategic possibility recorded as a Linear ticket, distinct from
  a generic idea or delivery contract. It stays in `FutureFeature` until the user promotes or cancels it.
- **Assimilation brief** - the durable account of what changed, what was learned, and what remains.

## Roles and skills

- **Skill** - an authored native skill folder: methodology and resources any session loads when the
  work needs it. A skill that carries an identity is a role; every other skill sorts by subject into
  `engineering` (the craft) or `productivity` (thinking, writing and moving work along with the user).
- **Role** - a skill that carries an identity: an officer or crew.
- **Officer** - a role that holds something and never switches: the ticket-captain, the default
  officer, holds a ticket; the coordinator holds a Project, and every open Project has one (H4, H5);
  the chief-of-staff holds the board. A session or agent has at most one officer role; a captain never
  becomes a coordinator or the reverse.
- **Coordinator** - the Project's officer. It decides mechanically only: it keeps the lanes full,
  watches liveness, paces merges and routes every judgement out to a fresh agent's ticket (H5;
  DYD-1082 1bc04dce change 9). It behaves the same day and night (DYD-1195 33520d6c).
- **Envoy** - a ticket captain on a HITL Question, Grilling, Walkthrough, Wayfinding or Enablement
  ticket, loading that job's skill (H15, H23; DYD-1082 1bc04dce change 10).
- **Map holder** - who holds a map and charts it: for a Project, its coordinator together with a
  captain on a Wayfinding ticket, the coordinator keeping things on track and the Wayfinding
  captain doing the deciding (S14, S15; DYD-1195 b46e8343); for a ticket, its captain.
- **Crew** - a role spawned for one bounded job that holds nothing and returns its result to whoever
  sent it: implementer, reviewer, research, scout (DYD-1082 1bc04dce change 3).
- **Agent** - a native-platform instance of a spawned role: a crew member, or a Ticket Captain that
  the coordinator or the user's session keeps in flight.
- **Method** - a plain skill used inside other work, carrying no identity of its own: co-thinker,
  grilling, wayfinder, domain-modeling, codebase-design, diagnosing-bugs, prototype, show-me,
  writing-for-agents, writing-for-humans, retro, wizard, overnight-mode, maintain-verification-skill,
  maintain-formal-verification-skill. These load only when the user asks: to-project, to-ticket,
  in-your-own-words, catch-up, teach, walkthrough (also at the map holder's Milestone and landing steps) and handoff (also
  when an officer briefs an agent it dispatches).
- **User command** - a skill only the user invokes by name, never reached for by a model:
  grill-me, bro, improve-codebase-architecture, create-verification-skill,
  create-formal-verification-skill, statechart, update-dydo.
- **Rubric** - the one named standard a reviewer judges a candidate against: code, docs,
  spec, or merge.

## Execution and proof

- **Supersymmetry** - a captain's ticket is a Project one level down: the same Types, statuses and
  chain hold at both levels, and only the map holder changes.
- **Reviewed intent** - the rule that implementation begins only from an independently reviewable
  contract: an atomic ticket, or a Project's tickets with their applicable feature specs and Decisions.
- **Scenario** - one acceptance criterion at the product's boundary, written in Gherkin in the
  ticket's feature files by the implementer, where the ticket's contract calls for Gherkin. It is
  contract: the captain decides the set and the writer wires it without weakening it; a change to it
  is a contract amendment.
- **Gate** - any explicit pass/fail checkpoint.
- **Hop** - one crew member's committed pass over a ticket branch: `implement`, `fix` or `merge`, and
  `proof` for a proof test on a proof branch, a Break reviewer's included. Its SHA is evidence on the ticket,
  and the reviewer reads the hops in order.
- **Ticket review** - an independent reviewer's verdict on one candidate against one named rubric,
  before it merges. FAIL means at least one blocker; PASS means none, and may list should and nit
  findings. There is no unlabelled note: every finding carries a severity and is answered.
- **Severity** - the label on every review finding (DR 058); a ticket born from a finding carries
  it as a Linear label in the `Severity` group. A `blocker`: the intended case is
  wrong; only a blocker FAILs a review and sends a fix hop. A `should`: a real bug callers will hit,
  inside the intent; the captain files it as its own ticket, and a Project lands only once each is
  `Done` or closed with a reason. A `nit`: everything else, one line on the ticket's record, listed
  at the final walkthrough for the user to promote; an inquisition's nits go on its round's nit
  ticket instead (DR 059). The fix-hop author checks each label against
  the source and may change it with evidence; the captain records the change and its reason.
- **Merge review** - the `merge` rubric run after every merge that has a Merge Ticket: a mechanical
  spot check of the integrated state that also proves the Project's acceptance criteria at a feature's
  final merge. An atomic ticket's merge into main has none; its ticket review PASS, green CI and the
  user's click stand in its place.
- **Inquisition** - a user-confirmed Type held by a captain: one round of audit of the integrated
  feature. Its captain conducts and never hunts: it draws the area map, runs every applicable phase
  on every area through reviewers, whose lenses the phases are (N5; DYD-1082 1bc04dce change 5), triages each finding by severity, and delivers its own
  record. It files outcomes, never PASS or FAIL. Rounds repeat from zero until one finds no blocker
  and no should (DR 059).
- **Round** - one Inquisition, started from zero with a fresh area map, never a recheck of an
  earlier round's findings; each round has its own Bugs and nit ticket.
- **Area** - a small piece of the landed work's code, architecture or files; one file can be an
  area, and a seam between two areas is its own area. A big area gets superficial coverage.
- **Area map** - the table of areas the inquisition captain draws from the landing diff and the
  file layout, on the Inquisition ticket, each row saying which phases apply or why not.
- **Phase** - in an inquisition, one of an area's three tracks, run in order: **Read** hunts what neither
  a test nor a drive would catch and tags the rest `testable` or `drivable`; **Break** decides
  hypotheses with its own proof tests on a proof branch; **Drive** runs the area's journeys
  through the project's verification skill and API. A plain skill or doc area gets Read only.
- **Findings register** - `inquisition-register.md` at the root of the audit branch: one row per
  finding with its severity, proof, sightings and disposition, pushed after every return.
- **Sighting** - a further place and proof of a finding already in the register; the same bug seen
  twice is one finding with two sightings.
- **Verification gap** - something a user can do that the project's verification skill cannot;
  Drive files it as a should, owned by `maintain-verification-skill`.
- **Patch lane** - a proven, well-defined blocker or should whose fix the inquisition prepares
  mid-round on its Bug's own branch, reviewed once in the Bug's chain. The Bug stays assigned to
  the inquisition's captain until the patch is attached, then becomes pickable.
- **Nit ticket** - the one `Feature` per round that holds the round's nits, held by the map holder
  and linked to its Inquisition; a nit needs no proof, and the user decides each at the Project's
  final walkthrough.
- **Group Sub-ticket** - one slice of an area map above about 10 areas, with its own captain and
  audit branch; the parent captain runs the cross-group triage and delivers the one record.
- **Merge** - a Type for one merge operation, with its own implement/review chain and
  integrated gates. The user clicks the Project landing and an atomic ticket's PR, which has no Merge
  Ticket; every operation preserves hop SHAs.
- **Release** - a captain leaves a resume SHA on the record, pushes its branch, removes the worktree,
  returns the parent to `Todo`, unassigns and wires any blocker. A dead session has no final push.
- **Review block** - the reviewer's whole return, and the only thing that fills a Ticket Captain's
  review slot, in the one-line `PASS` or `FAIL` form of the
  [Linear Workspace Standard](./linear-workspace-standard.md#communication-and-evidence). A PASS binds one candidate under one contract; a change to
  either calls for a fresh review. It lives on the work judged: its Merge Ticket for a merge, otherwise
  its ticket; a PR carries the block when one exists.
- **Evidence** - a commit, PR, test result, review verdict, or audit artifact linked to a ticket; it
  is not another work item.
- **Working-tree standard** - the shared rules for opening a feature branch, claiming a ticket by
  assignment, keeping one writer per worktree, and cleaning up after the merge. It lives in
  [working-tree-standard.md](../guides/working-tree-standard.md), so no agent invents its own habits.
- **Worktree** - Git isolation for implementation; the host owns where it lives.
- **HITL** / **AFK** - whether producing the work requires live human participation. They are not work
  types or acceptance states.

## Retired PM terms

These words survive only in older documents. Campaign, Sprint, Slice, backlog item, and the
separate observed-problem Issue are not dydo 3 PM objects: use the Linear Initiative, Project, and
ticket where work is live, though a slice may still name an implementation technique. Tier-1
manager, orchestrator, and the run-sprint workflow (internally
run-issues) gave way to the officers - a Ticket Captain owns one ticket, and a coordinator
holds each open Project. The Project Planner gave way to the map holder, who charts with
`wayfinder` (DR 051). Hat and worker are retired too (DR 050): a session keeps the role it was given and loads skills
as the work needs them, and a spawned role is crew. Issue planner and test-writer gave way to `code-writer`, which since 2026-09-21 also absorbs the
specifier, implementer and hardener chain that briefly replaced it (DR 047, amended by DYD-222). Its
phases and the Issue-resolution plan it posted retired on 2026-09-23 (DR 046, amended by DYD-230):
the captain's contract is the ticket's only contract text. The `Specifying` and `Hardening` statuses
and the tightening or hardening pass retired the same day (DR 047, amended 2026-09-23): a spec review
shows `In Review`. Wayfinding map, Waypoint, and the Frontier they defined are gone: a Project's map is its
Linear description, and frontier now means the map tickets above. Workflow retired with the old inquisition harness. Inquisition is now a ticket Type with the
filing outcome above; its parts and lenses, its proof-only code-writer and its separate record
Feature gave way to the area map, its phases and its own record (DR 059). The 2026-11-19
walkthrough renamed or retired four roles (DYD-1082 1bc04dce changes 4, 5, 7, 9): the admiral is the
coordinator; `code-writer` and `docs-writer` are one `implementer`; the inquisitor kind is gone, its
phases now reviewer lenses; `what-would-balazs-do` retired into the ticket captain's council-chair
resource. None of these words creates a file, command, lifecycle, or Linear type.

## Related

- [Working-Tree Standard](../guides/working-tree-standard.md) - Branches, claims, worktrees, cleanup
- [Project Glossary](../glossary.md) - This project's domain vocabulary

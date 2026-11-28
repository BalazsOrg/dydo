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
- **Papercut** - one small thing that slowed an agent down, filed through the papercut skill in its
  team's `Papercuts` grouping; the user's retro reads them as evidence.
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
- **Council** - a Question ticket at stage `council`, with Mode AFK, chaired by a ticket captain with
  its council-chair resource; the deliberation sits on the ticket next to the answer (L16, L23, H23;
  DYD-1195 ea16ea28).
- **Tribunal** - the council's Judge procedure (L18, L23). Its members are two judges from different
  families, who take sealed positions on a neutral case file, then exchange once; the chairing
  captain is not a member (L18). Judges who agree on a two-way call that is not DR-grade decide it;
  otherwise the call is prepared for the user (L18, L20). A one-way or DR-grade call is always
  prepared, except overnight, when both judges agree on a two-way, seriously blocking DR-grade call:
  that one becomes a `proposed` DR (L18, L21). A decided Question is provisional: acted on, never
  ratified (L19). The user ratifies only a `proposed` DR (L21).
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
  grilling, wayfinder, domain-modeling, codebase-design, diagnosing-bugs, tdd, prototype, show-me,
  writing-for-agents, writing-for-humans, papercut, wizard, overnight-mode, maintain-verification-skill,
  maintain-formal-verification-skill. These load only when the user asks: to-project, to-ticket,
  in-your-own-words, catch-up, teach, walkthrough (also at the map holder's Milestone and landing steps) and handoff (also
  when an officer briefs an agent it dispatches).
- **User command** - a skill only the user invokes by name, never reached for by a model:
  grill-me, bro, improve-codebase-architecture, create-verification-skill,
  create-formal-verification-skill, statechart, update-dydo, retro, to-spec.
- **Rubric** - the one named standard a reviewer judges a candidate against: code or docs (D6). A
  spec gets no review, only an optional scout pass that returns Questions (R4).

## Execution and proof

- **Supersymmetry** - a captain's ticket is a Project one level down: the same Types, statuses and
  chain hold at both levels, and only the map holder changes.
- **Reviewed intent** - the rule that implementation begins only from an independently reviewable
  contract: an atomic ticket, or a Project's tickets with their applicable feature specs and Decisions.
- **Scenario** - one acceptance criterion at the product's boundary, written in Gherkin in the
  ticket's feature files, where the ticket's contract calls for Gherkin. It is contract: the captain
  decides the set, and it is wired without weakening it; a change to it is a contract amendment.
- **Gate** - any explicit pass/fail checkpoint.
- **Hop** - one crew member's committed pass over a ticket branch: `implement`, `fix` or `merge`, and
  `proof` for a proof test on a proof branch, a Break reviewer's included. Its SHA is evidence on the ticket,
  and the reviewer reads the hops in order.
- **Ticket review** - an independent reviewer's verdict on one candidate against one named rubric,
  before it merges. FAIL means at least one blocker; PASS means none, and may list should and nit
  findings. There is no unlabelled note: every finding carries a severity and is answered.
- **Severity** - the label on every review finding (DR 058); a ticket born from a finding carries
  it as a Linear label in the `Severity` group. A `blocker`: a problem so bad that it needs fixing
  now; only a blocker FAILs a review and sends a fix hop. A `should`: a problem that needs a fix, but
  the fix can wait; the captain files it as its own ticket, and a Project lands only once each is
  `Done` or closed with a reason. A `nit`: an observation that gets a ruling, and may need no fix at
  all; a reviewer rules each one, it is one line on the ticket's record, and the final walkthrough
  lists each that needs no fix with its ruling, bringing the user only rulings already made (one
  ruled to need a fix becomes a `should`, listed as one); an inquisition's nits go on
  its round's nit ticket instead (DR 059). The fix-hop author checks each label against
  the source and may change it with evidence; the captain records the change and its reason.
- **Merge review** - kept only at the landing, as a Merge-confidence council on the landing
  candidate before the user's walkthrough (R20, L16, N6). A merge into the feature branch has none;
  the combined gates on the merge SHA and hosted CI prove it (R18).
- **Inquisition** - review at the landing level, held by a ticket captain with its
  inquisition-captain resource: the landed work cut by feature and staffed with reviewers (N5;
  DYD-1082 1bc04dce change 5). Its captain conducts and never hunts: it draws the feature map, has
  each lens run on every slice by its own reviewer, and triages each finding by severity. Its record
  is written by an implementer (N12). Rounds repeat from zero until one finds no blocker and no
  should (N5, DR 059).
- **Round** - one Inquisition, started from zero with a fresh feature map, never a recheck of an
  earlier round's findings; each round has its own Bugs and nit ticket.
- **Slice** - one small piece of the feature map: a feature, or a seam where features meet (N5).
- **Feature map** - the slices the root inquisition captain draws from the landed diff, posted on
  the Inquisition ticket (N5).
- **Lens** - in an inquisition, one of Read, Break and Drive, run on a slice in that order, each by
  its own reviewer (N5, R34). **Read** hunts what neither a test nor a drive would catch; **Break**
  decides hypotheses with its own proof tests on a proof branch; **Drive** runs the slice's
  journeys through the project's verification skill. A prose slice gets Read only (N12). "Phase"
  names only a role spine's job phases (K9).
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
  and linked to its Inquisition; a nit needs no proof, a reviewer rules each one, and the Project's
  final walkthrough brings the user the rulings already made.
- **Group Sub-ticket** - up to about 10 slices of a feature map above about 10, held by a
  sub-captain; the root captain stitches the groups' results and keeps the seams between groups as
  slices of its own (N5).
- **Merge** - the Type of a Merge hop, one merge operation (Todo → In Progress → Done) with no
  Review under it. It is Done when DYTEST holds a green receipt on the merge tree and hosted CI is
  green on the merge SHA (R20). Its captain runs `dydo merge` itself (R21). A two-way PR into master
  merges AFK, by its captain, once its evidence is complete (R26).
- **Release** - a captain leaves a resume SHA on the record, pushes its branch, removes the worktree,
  returns the parent to `Todo`, unassigns and wires any blocker. A dead session has no final push.
- **Review block** - the reviewer's whole return, and the only thing that fills a Ticket Captain's
  review slot, in the one-line `PASS` or `FAIL` form of the
  [Linear Workspace Standard](./linear-workspace-standard.md#communication-and-evidence). A PASS binds one candidate under one contract; a change to
  either calls for a fresh review. It lives on its Review sub-ticket; a PR body carries only a
  one-line PASS naming that Review ticket (R17).
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
ticket where work is live, though a slice may still name an implementation technique or an
inquisition's slice. Tier-1
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
Feature gave way to the area map, its phases and its own record (DR 059); the area map, areas and
phases then gave way to the feature map, slices and lenses (N5). The 2026-11-19
walkthrough renamed or retired four roles (DYD-1082 1bc04dce changes 4, 5, 7, 9): the admiral is the
coordinator; `code-writer` and `docs-writer` are one `implementer`; the inquisitor kind is gone, its
phases now reviewer lenses; `what-would-balazs-do` retired into the ticket captain's council-chair
resource. None of these words creates a file, command, lifecycle, or Linear type.

## Related

- [Working-Tree Standard](../guides/working-tree-standard.md) - Branches, claims, worktrees, cleanup
- [Project Glossary](../glossary.md) - This project's domain vocabulary

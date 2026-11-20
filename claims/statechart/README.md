# The dydo 4.0.0 statechart

The whole of dydo 4.0.0 as 31 machines, generated from the C# claim model in `tools/SkillBuild`
(DYD-1225, K1). Every state and arrow cites the rules of `Spec: dydo 4.0.0` (approved at updatedAt
2026-11-20T01:58:55.576Z) it rests on; an arrow the spec leaves open is drawn as `GAP: …`. The role
skills' moves are the same claims as these arrows, so the two cannot drift. The skills are kept
barebones; the chart is not, and it holds behaviour the skills leave unsaid.

Everything here except this README and the `chart.mmd` files is built: in each machine's folder
its `transitions.json`, `<Machine>.tla`, `<Machine>.cfg` and `view.mjs`. Edit the model, never the
files.

## Rebuild

1. `dotnet run --project tools/SkillBuild` from the repository root writes every built file.
2. For each machine, `dydo statechart <abs>/claims/statechart/<machine>/<Machine>.tla --skill <statechart skill> --config <abs>/claims/statechart/<machine>/<Machine>.cfg --view <abs>/claims/statechart/<machine>/view.mjs --out <scratch>/<machine>`, then copy `<scratch>/<machine>/chart.mmd` here.
3. Never commit `tlc.out`, `report.md` or `chart.svg`: TLC output stays out of tracked evidence (N14).

## Charts, in the overview's order

TLC line from the run at commit time, last on the claim model at
`81ea4f84ecec92fc69a9d74792e26fc4d6f45fe9` (DYD-1327); every machine holds `NoDeadlock`.
A chart draws fewer arrows than its transitions where several share both ends: their labels are joined with "or"
on one arrow, and a self-loop (a counter step) is not drawn. The states column counts the
machine's own states, not the `GAP` end a chart adds.

| Machine | Folder | States | Transitions | GAPs | TLC |
|---|---|---|---|---|---|
| dydo 4.0.0 (the overview: its nominal path and the branches that return to it) | `overview` | 15 | 22 | 0 | 15 states, 23 steps |
| The claim build | `claim-build` | 10 | 14 | 0 | 10 states, 15 steps |
| The nine role skills evaluated | `role-evaluation` | 7 | 11 | 0 | 7 states, 12 steps |
| Host settings and the guard | `host-setup` | 5 | 7 | 0 | 5 states, 7 steps |
| A dydo statechart run | `statechart-run` | 9 | 10 | 0 | 9 states, 14 steps |
| A Project and its coordinator | `project` | 5 | 7 | 0 | 5 states, 8 steps |
| Crew dispatch and return | `dispatch` | 7 | 7 | 0 | 7 states, 10 steps |
| A ticket's status | `ticket-status` | 8 | 11 | 0 | 8 states, 14 steps |
| A step's hops, by Type label | `hops` | 10 | 12 | 0 | 10 states, 15 steps |
| Proof tests and gates | `proof` | 11 | 16 | 0 | 11 states, 19 steps |
| Descriptive docs | `docs` | 10 | 14 | 0 | 10 states, 16 steps |
| The review loop | `review` | 8 | 13 | 0 | 92 states, 155 steps |
| A should | `should` | 7 | 6 | 0 | 7 states, 11 steps |
| dydo merge and its refusals | `merge-command` | 5 | 11 | 0 | 5 states, 15 steps |
| The Merge hop into the feature branch | `feature-merge` | 12 | 18 | 0 | 12 states, 19 steps |
| Survey, board and CI findings | `findings` | 14 | 18 | 0 | 14 states, 21 steps |
| Liveness | `liveness` | 3 | 5 | 0 | 3 states, 5 steps |
| dydo map | `map` | 5 | 6 | 0 | 5 states, 8 steps |
| A need for Balazs (envoys) | `envoy` | 10 | 9 | 0 | 13 states, 20 steps |
| A council | `council` | 4 | 4 | 0 | 4 states, 5 steps |
| The tribunal | `tribunal` | 12 | 13 | 0 | 244 states, 319 steps |
| How the law grows | `law` | 8 | 7 | 0 | 8 states, 11 steps |
| A new DR's slugs checked | `slug-check` | 3 | 3 | 0 | 3 states, 4 steps |
| The DR backfill | `backfill` | 6 | 6 | 0 | 6 states, 8 steps |
| The chief of staff | `staff` | 5 | 6 | 0 | 5 states, 6 steps |
| The landing | `landing` | 16 | 23 | 0 | 293 states, 377 steps |
| The landing's mutation check | `mutation` | 8 | 14 | 0 | 15 states, 29 steps |
| A PR into master | `master-route` | 10 | 14 | 0 | 47 states, 65 steps |
| Safe delete | `safe-delete` | 3 | 4 | 0 | 3 states, 5 steps |
| A file over the token limit | `ignore-entry` | 7 | 7 | 0 | 7 states, 10 steps |
| The adopter update | `adopter` | 7 | 8 | 0 | 7 states, 10 steps |
| **31 machines** | | **250** | **326** | **0** | |

## GAPs: all 10 closed

His walkthrough closed all ten in round 5 (DYD-1082 555a8300), and envoy 5 settled the open parts
(DYD-1195). No machine draws a GAP; each is now an arrow, with its ruling in the claim's rule.

1. Project, Watching → Unheld: idle when Paused, or when no coordinator or other agent runs in it (`Coordination.Idle`).
2. Project, Unheld → Watching: he or the chief of staff starts or wakes its coordinator (`Coordination.Wake`).
3. Ticket status, New → FutureFeature and FutureFeature → Backlog: an unscheduled strategic possibility enters, and only he promotes it out (`TicketStatus.Future`, `TicketStatus.Promote`).
4. Ticket status, New → Backlog: a ticket filed without a contract, Todo once a Wayfinding captain contracts it (`TicketStatus.Filed`, `TicketStatus.Contract`).
5. Landing, Walkthrough → Not accepted → Integrated: his findings become tickets in the same Project, a fix captain re-cuts the branch, and another landing follows (`Landing.NotAccepted`, `Landing.ReCut`).
6. Chief of staff, Day → Muster: the night starts with his muster (`Staff.Muster`).
7. Chief of staff, Night watch → Morning brief: the night ends on his return (`Staff.Morning`).
8. Tribunal, Proposed DR → Declined: the record is deleted, his decline and words go on the raising ticket, and the question returns to its raiser (`Tribunal.Decline`).
9. Role evaluation, Written fresh, Folded and Removed → Changed → Questioned, and the claim build's In review → Edited: a change he rejects at a walkthrough goes to new captains on new tickets (DYD-1195 8661b8f4; `Evaluation.RejectRewrite`, `RejectFold`, `RejectRemoval`, `Rebuild`, `Build.Rejected`).
10. Safe delete, Archived → Live: `dydo archive restore` (DYD-1195 70df9ff8; `SafeDelete.Restore`).

## Notes

- **Decided rules with no named signal or performer are arrows, not GAPs.** R29's `MasterRoute.Turn` lands main-bound merges one at a time, both doors through the same pacing and re-check; the spec does not say what signals a PR's turn. L20's `Tribunal.Score` scores his daytime answers and L19's `Tribunal.Wrong` investigates a ruling found wrong; neither rule names who does it, so both arrows carry no actor. K22 does not name who questions a role skill, so `Evaluation.Keep`, `Fold`, `Remove` and `Rewrite` carry none either, and L8 names no one who corrects a failed slug (`Slugs.Corrected`). The law's growth names only "agents" (L9, L13) or no one (L7, L11), so `Law.Departs`, `Mechanical`, `Propose` and `Precedential` carry no line.
- **A need follows the Question stages (H16, H23).** In `envoy`, a need is decided where it arises when obvious and reversible, asked in one line when he is already in the thread, or passed up: its officer, a captain or the coordinator, opens a `research` Question for a fresh captain and waits on it (DYD-1195 317eb49f, 3a854e37). No officer does its homework. The research answers it, opens a `council` child, or opens a `his-call` child that joins the coordinator's HITL queue, onto which the coordinator dispatches one envoy at a time; the next goes only once he has closed the last one's thread, and no "moved on" exit remains (893e1f5b). The council child ends this machine: its route, a call it prepares for him included, is the tribunal's.
- **Host settings' two return arrows are modelling details.** `Host.Pass` (Guarded → Initialised) and `Host.Refused` (Memory denied → Initialised) exist only to keep the guard's per-call loop live; DR 055 and DR 056 decide what the guard does with a call, not a loop.
- **Blockers are routed by kind wherever a review runs (R9).** In `review` and in the landing merge review alike: broken or against the spec to Fix, a security hole to Fix with the security lens first on the next round, fundamental or a new requirement to the officer above (above a top-level captain, a fresh Wayfinding captain re-plans and the coordinator only opens its ticket, H5), and a need for him up the chain at once.
- **The root inquisition captain holds each round.** The coordinator opens the Inquisition ticket; its root captain staffs the round from the roster and the quotas (DYD-1195 6deea8c0), draws a fresh feature map, and holds the round's verdict (N5, S8), which turns blockers into Fix rN (S9) and registers shoulds (S17). Each slice runs Read → Break → Drive, each lens a separate reviewer. Groups over about 10 slices, their sub-captains and the root's seam slices are skill prose, not states: the chart's `areas` counter stands for slices.
- **A Question's call routes after its research.** The tribunal starts at a Question's call after its research (L18, H23): a DR settles it, it goes to him as a `his-call` child, or it goes to a `council` child by day or night. That routing is the research captain's prose (`Tribunal.Settled`, `HisCall`, `File`); the same children are arrivals in `envoy`, carried by the chair (`Council.Hold`) and the coordinator's queue (`Envoy.Open`), so no rule is stated twice (K4). A council decides an agreed two-way call by day too when it is not worth his time (DYD-1195 87c1e2e6), so `DecidedOvernight` keeps its id and is titled "Decided, provisionally".
- **A decided Question and a proposed DR are different routes.** In the overview a Question decided by him by day, or provisionally overnight, is acted on and never ratified (L19, L20); only an overnight DR-grade call becomes a `proposed` DR, which waits for his ratification (L21, `tribunal`'s Proposed DR → Ratified).
- **CA19, built vs decided.** N10 decides that safe delete covers `~/.t3/worktrees` without breaking T3, and the chart archives a T3 worktree like any other. The built `dydo archive` still refuses every T3-managed target (`T3_RELEASE_UNPROVEN`), owned by DYD-1216.
- **Ticket statuses are system arrows.** The `TicketStatus` machine carries no prose. Each status rule's prose sits on the role claim that makes the move (`Dispatch.Start`, `Project.Replanned`, `FeatureMerge.RealRed`, `Should.Dup`), so no rule is stated twice.
- **The landing's 4.0.0 prerequisites** are one counted guard in `landing`: shoulds closed (S18), mutation clean (T12, N15), the purge done (T8), the evidence history rewritten on his click (N14), DYTEST wired in (Y2, Y6), the principle descriptions written (L6), the backfill page passed (L12), the law shipped and the Upgrade note and re-measure (N8), and the statuses Enablement (N9). The tag also waits on guard hardening (N2). Both are self-loops, so the chart does not draw them; the TLA+ module has them.
- **Changed files stay under T15's limit.** `bash tools/SkillBuild/changed-file-sizes.sh` checks every file the branch adds or changes against `origin/feature/dydo-4-0-0` (≤ 20,000 bytes, 5k tokens) and prints the five largest; a suite test cannot see the merge base reliably, so it is a gate command.
- **The job skill table is checked at build (H23).** `Envoy.Jobs` is typed model data keyed on Type, and for a Question on its stage label (`research`, `council`, `his-call`). The builder refuses a table that misses a row, gives one another skill or resource than H23's, or adds a row H23 does not name; a council row whose `council-chair` resource the ticket captain does not declare; a table skill with no `SKILL.md` under `skills/`; and an envoy brief that states no closed-thread signal.

## Omissions

- **Properties.** No invariant or liveness property is generated: K3 has them written by hand, in `verification/statechart/` (DYD-1225, adapted to the rebuilt roles at DYD-1289), and its `run.py` checks them (K20). Each closed GAP has its own property there, written from his ruling. The one progress assumption is model data: weak fairness on `liveness`'s four healing moves (S24), so that machine's config is `SPECIFICATION Spec`.
- **Cross-machine synchronisation is not modelled.** Each machine runs alone. The overview links the areas, and a state shared in prose (a Merge hop that is both a `hops` final and the start of `feature-merge`) is two states in two machines.
- **Bounds.** Counters are bounded for TLC only (review rounds ≤ 4, landing rounds ≤ 3, areas per round ≤ 2, re-syncs ≤ 3, landing prerequisites = 10). The spec sets no stop on review rounds (R10).
- **Two-valued choices are nondeterministic.** The tribunal's night, two-way, DR-grade, blocking and agreement flags, a blocker's kind, a master PR's door, and the cloud route's cost are free choices, not modelled causes.
- **Not modelled as behaviour.** Rules with no state of their own are cited on states, not arrows: R1, H20 and N13 (the push's ticket routing). N12's carried DRs 055, 056 and 062 are machines (`host-setup`, `statechart-run`); its other carried DRs are cited as `N12 DR …` on the states they shape. The K29 rules for `dydo/reference/writing-docs.md` (§2.1–2.2) are not built: that file is hand-written and outside the claim-made set (K5).

## Coverage: the spec's §2 areas

| §2 area | Machines |
|---|---|
| 2.1 Claims-built skills (K1–K29) | `claim-build` (K20: TLC passed or failed), `role-evaluation` (K22), `adopter` (K18), `dispatch` (K14), `overview` |
| 2.2 Review and merge (R1–R33) | `review`, `hops`, `merge-command`, `feature-merge`, `master-route` (R26–R31), `dispatch` (R12–R16), `landing` (R9, R20, R28), `findings` (R30) |
| 2.3 Statuses and board (S1–S26) | `ticket-status`, `hops` (S3), `review` (S8–S11), `should` (S17, S18), `liveness` (S24), `findings` (S22, S23, S25, S26), `map` (S19–S21, S26), `project` (S14–S16) |
| 2.4 Hierarchy and envoys (H1–H24) | `dispatch`, `envoy` (H15, H16, H18, H19, H22, H23: the Question stages; H23's table checked at build), `staff`, `liveness` (H12), `project` (H4, H5, H8–H11), `findings` (H8, H11), `map` (H6), `adopter` (H9) |
| 2.5 Law, councils and tribunal (L1–L23) | `law`, `slug-check` (L8), `backfill`, `council`, `tribunal` (L18–L22), `envoy` (L18, L21, L23: research before a council child), `staff` (L19, L21), `mutation` (L14) |
| 2.6 Tests, gates and the token limit (T1–T23) | `proof`, `ignore-entry`, `mutation` (T12), `landing` (T8, T12) |
| 2.7 Descriptive docs (D1–D11) | `docs`, `landing` (D11), `adopter` (D1, D9) |
| 2.8 DYTEST (Y1–Y9) | `proof` (Y4, Y6, Y7), `findings` (Y1, Y3, Y8, Y9), `feature-merge` (Y4, Y9), `landing` (Y2, Y6), `adopter` (Y5) |
| 2.9 The landing (N1–N15) | `landing` (N5's phases), `mutation`, `master-route`, `safe-delete` (N10), `host-setup` and `statechart-run` (N12), `adopter` (N1, N8, N11) |

## Coverage: the spec's §4 acceptance checks

| §4 check | Machines |
|---|---|
| 1. The model builds every file; golden and shape tests; TLC against properties | `claim-build` (TLC passed or failed) |
| 2. Nine roles evaluated; survivors rewritten; his approval | `role-evaluation` |
| 3. Seven statuses, auto-close off, the Type labels, the stage and `dr-grade` labels | `ticket-status`, `hops`, `landing` (N9), `adopter`, `envoy` (H23's stages), `tribunal` (L21's `dr-grade`) |
| 4. `dydo merge` enforces R19, R23, R24; the AFK route into master | `merge-command`, `feature-merge`, `master-route` |
| 5. `dydo survey board`, the new findings, `liveness` and its healing; `dydo map` | `findings`, `liveness`, `map` |
| 6. The law layout, slug validation, L3 shipping | `law`, `slug-check` (L8: slug validation), `adopter`, `landing` (N8) |
| 7. Only tests meeting the requirements remain, each with its tag | `proof` |
| 8. The gate set on changed files, the 5k nudge and gate | `proof`, `ignore-entry` |
| 9. DYTEST Mutation ships; the landing's mutation check | `mutation`, `landing` |
| 10. `explains:`, flagged by `dydo check` and `pr-ready` | `docs` |
| 11. The four L1 rules in `writing-docs.md` and the working-tree standard | `claim-build` (K29); the working-tree standard is built, `writing-docs.md` is not (see Omissions) |
| 12. DYTEST wired in behind DYD-1071 | `landing` (Y2, Y6) |
| 13. A clean inquisition round; every should closed | `landing` (Read → Break → Drive per slice), `should` |
| 14. The committed evidence gone from history | `landing` (N14) |
| 15. Guard hardening before the tag | `landing` (N2) |
| 16. Safe delete, T3 worktrees included | `safe-delete` |
| 17. Principle descriptions; the backfill page | `landing` (L6, L12), `backfill` |
| 18. The Upgrade note and the re-measure | `adopter`, `landing` (N8) |
| 19. The roster, `dydo usage`, the envoy skill table, the council resource | `dispatch` (R13, R14), `envoy` (H23; the builder's table check), `council` (L23) |
| 20. The upstream glossary shape, the plan files gone, the ledger retired | `adopter` (D9), `landing` (D11), `findings` (S25) |
| 21. The rebuilt skills carry the walkthrough changes: the coordinator, the crew kinds and the implementer, skill-naming dispatch, the spine shape, the inquisition-captain and council-chair resources, the reviewer toolset | `role-evaluation` (K22's folds), `dispatch` (K14), `claim-build` (K9), `project` (H5), `landing` (N5, R34), `council` and `tribunal` (L23), `envoy` (H5, H15, H16) |
| 22. His acceptance at the landing walkthrough; the spec deleted | `landing`, `findings` |

## The ticket-captain prototype (DYD-1372)

One more machine, outside the overview and its 31: the states of a captain holding one ticket, built
from the truth-checked ticket-captain prototype's own claims (DYD-1295 519126ab). `round` counts review
rounds and is bounded at 4 only so TLC ends; the skill sets no cap.

- A round that found no new blocker is a state of its own. The review rounds end from it, and the one
  more round the captain may order leaves only from it, once (`extra`), so a round with a blocker
  reaches the next only through a Fix (R8).
- A third round that still has a blocker leaves only through the checkpoint (R10, S10).
- A repaired conflict or red gate goes on only through a review round, or back to the officer when it
  cannot be finished (R19, R21).
- A splitting captain's Question is one more child it waits on, so only a leaf has a Question state.
  A leaf's Question is left only to resume the leaf, once its answer is on the ticket. One that comes
  back unsettled goes to the officer's thread and stays open: its holder goes on waiting, a step
  that leaves the state unchanged (H16, S10).
- A filler ticket that fails its captain's check goes back from Taken, before it is set In Progress:
  unstarted and unassigned, the only state in which the survey offers it again (H11).

The chart draws 26 of the 28 arrows: a repair's two ways back to the officer share one, and the
unsettled Question's step stays in its state. TLC line from the run on the claim model of DYD-1372
fix r2b; it holds `NoDeadlock`. Its properties are written by
hand under `verification/statechart/`, from his rulings and never from this model (K3).

| Machine | Folder | States | Transitions | GAPs | TLC |
|---|---|---|---|---|---|
| A captain and its ticket | `captain` | 13 | 28 | 0 | 56 states, 100 steps |

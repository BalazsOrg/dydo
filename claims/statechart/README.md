# The dydo 4.0.0 statechart

The whole of dydo 4.0.0 as 31 machines, generated from the C# claim model in `tools/SkillBuild`
(DYD-1225, K1). Every state and arrow cites the rules of `Spec: dydo 4.0.0` (updatedAt
2026-11-18T22:00:15Z) it rests on; an arrow the spec leaves open is drawn as `GAP: …`. The role
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

TLC line from the run at commit time; every machine holds `NoDeadlock`. A chart draws fewer
arrows than its transitions where several share both ends: their labels are joined with "or"
on one arrow, and a self-loop (a counter step) is not drawn. The states column counts the
machine's own states, not the `GAP` end a chart adds.

| Machine | Folder | States | Transitions | GAPs | TLC |
|---|---|---|---|---|---|
| dydo 4.0.0 (the overview: its nominal path and the branches that return to it) | `overview` | 15 | 22 | 0 | 15 states, 23 steps |
| The claim build | `claim-build` | 10 | 13 | 0 | 10 states, 14 steps |
| The nine role skills evaluated | `role-evaluation` | 6 | 7 | 3 | 9 states, 14 steps |
| Host settings and the guard | `host-setup` | 5 | 7 | 0 | 5 states, 7 steps |
| A dydo statechart run | `statechart-run` | 9 | 10 | 0 | 9 states, 14 steps |
| A Project and its admiral | `project` | 5 | 5 | 2 | 5 states, 8 steps |
| Crew dispatch and return | `dispatch` | 7 | 7 | 0 | 7 states, 10 steps |
| A ticket's status | `ticket-status` | 8 | 8 | 3 | 9 states, 15 steps |
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
| A need for Balazs (envoys) | `envoy` | 11 | 11 | 0 | 14 states, 21 steps |
| A council | `council` | 4 | 4 | 0 | 4 states, 5 steps |
| The tribunal | `tribunal` | 11 | 11 | 1 | 244 states, 314 steps |
| How the law grows | `law` | 8 | 7 | 0 | 8 states, 11 steps |
| A new DR's slugs checked | `slug-check` | 3 | 3 | 0 | 3 states, 4 steps |
| The DR backfill | `backfill` | 6 | 6 | 0 | 6 states, 8 steps |
| The chief of staff | `staff` | 5 | 6 | 2 | 5 states, 8 steps |
| The landing | `landing` | 15 | 21 | 1 | 151 states, 201 steps |
| The landing's mutation check | `mutation` | 8 | 14 | 0 | 15 states, 29 steps |
| A PR into master | `master-route` | 10 | 14 | 0 | 47 states, 65 steps |
| Safe delete | `safe-delete` | 3 | 3 | 1 | 3 states, 5 steps |
| A file over the token limit | `ignore-entry` | 7 | 7 | 0 | 7 states, 10 steps |
| The adopter update | `adopter` | 7 | 8 | 0 | 7 states, 10 steps |
| **31 machines** | | **248** | **313** | **13** | |

## GAPs: the 10 spec gaps (13 arrows)

Each is a real gap in the spec, drawn as a TLA+ action (`Gap1`, …) so the states it reaches are
reachable; it never reaches a skill.

1. Project, Watching → Unheld: H4 names idle Projects but never defines when a Project goes idle.
2. Project, Unheld → Watching: H4 says who holds a live Project, not who starts or wakes its admiral.
3. Ticket status, New → FutureFeature, and FutureFeature → unknown: S5 lists FutureFeature, but no rule says what enters it or how a ticket leaves it.
4. Ticket status, New → Backlog: S5 lists Backlog, but no rule says when a ticket is filed there.
5. Landing, Walkthrough → unknown: N6 gives only his acceptance; the route when he does not accept is unstated.
6. Chief of staff, Day → Muster: H7 says the chief of staff runs the overnight watch, not what starts it.
7. Chief of staff, Night watch → Morning brief: H7 and L19 name the watch and the morning brief, not what ends the night.
8. Tribunal, Proposed DR → unknown: L21 has him ratify a proposed DR but gives no route when he declines it.
9. Role evaluation, Written fresh, Folded and Removed → unknown: K16 and K22 give his approval of a rewrite, a fold or a removal at the final candidate, not the route when he rejects one.
10. Safe delete, Archived → Live: N10 names no restore from the archive; the built command has one (`dydo archive restore`).

## Notes

- **Decided rules with no named signal or performer are arrows, not GAPs.** R29's `MasterRoute.Turn` lands main-bound merges one at a time, both doors through the same pacing and re-check; the spec does not say what signals a PR's turn. L20's `Tribunal.Score` scores his daytime answers and L19's `Tribunal.Wrong` investigates a ruling found wrong; neither rule names who does it, so both arrows carry no actor. K22 does not name who questions a role skill, so `Evaluation.Keep`, `Fold`, `Remove` and `Rewrite` carry none either, and L8 names no one who corrects a failed slug (`Slugs.Corrected`).
- **Host settings' two return arrows are modelling details.** `Host.Pass` (Guarded → Initialised) and `Host.Refused` (Memory denied → Initialised) exist only to keep the guard's per-call loop live; DR 055 and DR 056 decide what the guard does with a call, not a loop.
- **Blockers are routed by kind wherever a review runs (R9).** In `review` and in the landing merge review alike: broken or against the spec to Fix, a security hole to Fix with the security lens first on the next round, fundamental or a new requirement to the officer above, and a need for him up the chain at once.
- **Who draws an inquisition round's area map** is not stated. The admiral staffs and holds the round (N5, S8): each round starts from zero with a fresh area map, then Read → Break → Drive per area (the default order), and only then the round's verdict, which turns blockers into Fix rN (S9) and registers shoulds (S17).
- **A decided Question and a proposed DR are different routes.** In the overview a Question decided by him by day, or provisionally overnight, is acted on and never ratified (L19, L20); only an overnight DR-grade call becomes a `proposed` DR, which waits for his ratification (L21, `tribunal`'s Proposed DR → Ratified).
- **CA19, built vs decided.** N10 decides that safe delete covers `~/.t3/worktrees` without breaking T3, and the chart archives a T3 worktree like any other. The built `dydo archive` still refuses every T3-managed target (`T3_RELEASE_UNPROVEN`), owned by DYD-1216.
- **Ticket statuses are system arrows.** The `TicketStatus` machine carries no prose. Each status rule's prose sits on the role claim that makes the move (`Dispatch.Start`, `Project.Replanned`, `FeatureMerge.RealRed`, `Should.Dup`), so no rule is stated twice.
- **The landing's 4.0.0 prerequisites** are one counted guard in `landing`: shoulds closed (S18), mutation clean (T12, N15), the purge done (T8), the evidence history rewritten on his click (N14), DYTEST wired in (Y2, Y6), the principle descriptions written (L6), the backfill page passed (L12), the law shipped and the Upgrade note and re-measure (N8), and the statuses Enablement (N9). The tag also waits on guard hardening (N2). Both are self-loops, so the chart does not draw them; the TLA+ module has them.
- **Changed files stay under T15's limit.** `bash tools/SkillBuild/changed-file-sizes.sh` checks every file the branch adds or changes against `origin/feature/dydo-4-0-0` (≤ 20,000 bytes, 5k tokens) and prints the five largest; a suite test cannot see the merge base reliably, so it is a gate command.
- **The envoy skill table is checked at build (H23).** `Envoy.Jobs` is typed model data, and the builder refuses a table that misses a job or gives one another skill than H23's, a table skill with no `SKILL.md` under `skills/`, and an envoy brief that states no closed-thread signal.

## Omissions

- **Properties.** No invariant or liveness property is generated: K3 has them written by hand, in `verification/statechart/` (DYD-1225), and its `run.py` checks them; all 31 machines pass all 109 (K20). The one progress assumption is model data: weak fairness on `liveness`'s four healing moves (S24), so that machine's config is `SPECIFICATION Spec`.
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
| 2.4 Hierarchy and envoys (H1–H24) | `dispatch`, `envoy` (H23, checked at build), `staff`, `liveness` (H12), `project` (H4, H5, H8–H11), `findings` (H8, H11), `map` (H6), `adopter` (H9) |
| 2.5 Law, councils and tribunal (L1–L23) | `law`, `slug-check` (L8), `backfill`, `council`, `tribunal` (L18–L22), `staff` (L19, L21), `mutation` (L14) |
| 2.6 Tests, gates and the token limit (T1–T23) | `proof`, `ignore-entry`, `mutation` (T12), `landing` (T8, T12) |
| 2.7 Descriptive docs (D1–D11) | `docs`, `landing` (D11), `adopter` (D1, D9) |
| 2.8 DYTEST (Y1–Y9) | `proof` (Y4, Y6, Y7), `findings` (Y1, Y3, Y8, Y9), `feature-merge` (Y4, Y9), `landing` (Y2, Y6), `adopter` (Y5) |
| 2.9 The landing (N1–N15) | `landing` (N5's phases), `mutation`, `master-route`, `safe-delete` (N10), `host-setup` and `statechart-run` (N12), `adopter` (N1, N8, N11) |

## Coverage: the spec's §4 acceptance checks

| §4 check | Machines |
|---|---|
| 1. The model builds every file; golden and shape tests; TLC against properties | `claim-build` (TLC passed or failed) |
| 2. Nine roles evaluated; survivors rewritten; his approval | `role-evaluation` |
| 3. Seven statuses, auto-close off, the Type labels | `ticket-status`, `hops`, `landing` (N9), `adopter` |
| 4. `dydo merge` enforces R19, R23, R24; the AFK route into master | `merge-command`, `feature-merge`, `master-route` |
| 5. `dydo survey board`, the new findings, `liveness` and its healing; `dydo map` | `findings`, `liveness`, `map` |
| 6. The law layout, slug validation, L3 shipping | `law`, `slug-check` (L8: slug validation), `adopter`, `landing` (N8) |
| 7. Only tests meeting the requirements remain, each with its tag | `proof` |
| 8. The gate set on changed files, the 5k nudge and gate | `proof`, `ignore-entry` |
| 9. DYTEST Mutation ships; the landing's mutation check | `mutation`, `landing` |
| 10. `explains:`, flagged by `dydo check` and `pr-ready` | `docs` |
| 11. The four L1 rules in `writing-docs.md` and the working-tree standard | `claim-build` (K29); the working-tree standard is built, `writing-docs.md` is not (see Omissions) |
| 12. DYTEST wired in behind DYD-1071 | `landing` (Y2, Y6) |
| 13. A clean inquisition round; every should closed | `landing` (Read → Break → Drive per area), `should` |
| 14. The committed evidence gone from history | `landing` (N14) |
| 15. Guard hardening before the tag | `landing` (N2) |
| 16. Safe delete, T3 worktrees included | `safe-delete` |
| 17. Principle descriptions; the backfill page | `landing` (L6, L12), `backfill` |
| 18. The Upgrade note and the re-measure | `adopter`, `landing` (N8) |
| 19. The roster, `dydo usage`, the envoy skill table, the council resource | `dispatch` (R13, R14), `envoy` (H23; the builder's table check), `council` (L23) |
| 20. The upstream glossary shape, the plan files gone, the ledger retired | `adopter` (D9), `landing` (D11), `findings` (S25) |
| 21. His acceptance at the landing walkthrough; the spec deleted | `landing`, `findings` |

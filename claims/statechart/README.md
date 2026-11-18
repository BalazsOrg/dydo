# The dydo 4.0.0 statechart

The whole of dydo 4.0.0 as 26 machines, generated from the C# claim model in `tools/SkillBuild`
(DYD-1225, K1). Every state and arrow cites the rules of `Spec: dydo 4.0.0` (updatedAt
2026-11-18T22:00:15Z) it rests on; an arrow the spec leaves open is drawn as `GAP: …`. The role
skills' moves are the same claims as these arrows, so the two cannot drift. The skills are kept
barebones; the chart is not, and it holds behaviour the skills leave unsaid.

Everything here except this README and the `chart.mmd` files is built: `transitions.json`, and in
each machine's folder its `<Machine>.tla`, `<Machine>.cfg` and `view.mjs`. Edit the model, never the
files.

## Rebuild

1. `dotnet run --project tools/SkillBuild` from the repository root writes every built file.
2. For each machine, `dydo statechart <abs>/claims/statechart/<machine>/<Machine>.tla --skill <statechart skill> --config <abs>/claims/statechart/<machine>/<Machine>.cfg --view <abs>/claims/statechart/<machine>/view.mjs --out <scratch>/<machine>`, then copy `<scratch>/<machine>/chart.mmd` here.
3. Never commit `tlc.out`, `report.md` or `chart.svg`: TLC output stays out of tracked evidence (N14).

## Charts, in the overview's order

TLC line from the run at commit time; every machine holds `NoDeadlock`. A chart draws fewer
arrows than its transitions where several share both ends: their labels are joined with "or"
on one arrow, and a self-loop (a counter step) is not drawn.

| Machine | Folder | States | Transitions | GAPs | TLC |
|---|---|---|---|---|---|
| dydo 4.0.0 (the overview: its areas in order) | `overview` | 20 | 20 | 0 | 20 states, 21 steps |
| The claim build | `claim-build` | 9 | 11 | 0 | 9 states, 12 steps |
| A Project and its admiral | `project` | 5 | 5 | 2 | 5 states, 8 steps |
| Crew dispatch and return | `dispatch` | 7 | 7 | 0 | 7 states, 10 steps |
| A ticket's status | `ticket-status` | 8 | 8 | 3 | 9 states, 15 steps |
| A step's hops, by Type label | `hops` | 8 | 9 | 0 | 8 states, 12 steps |
| Proof tests and gates | `proof` | 11 | 16 | 0 | 11 states, 19 steps |
| Descriptive docs | `docs` | 10 | 14 | 0 | 10 states, 16 steps |
| The review loop | `review` | 8 | 13 | 0 | 106 states, 197 steps |
| A should | `should` | 7 | 6 | 0 | 7 states, 11 steps |
| dydo merge and its refusals | `merge-command` | 5 | 11 | 0 | 5 states, 15 steps |
| The Merge hop into the feature branch | `feature-merge` | 12 | 18 | 0 | 12 states, 19 steps |
| Survey, board and CI findings | `findings` | 15 | 19 | 0 | 15 states, 23 steps |
| Liveness | `liveness` | 3 | 5 | 0 | 3 states, 5 steps |
| A need for Balazs (envoys) | `envoy` | 10 | 10 | 0 | 13 states, 19 steps |
| A council | `council` | 4 | 4 | 0 | 4 states, 5 steps |
| The tribunal | `tribunal` | 10 | 9 | 2 | 129 states, 194 steps |
| How the law grows | `law` | 8 | 7 | 0 | 8 states, 11 steps |
| The DR backfill | `backfill` | 6 | 6 | 0 | 6 states, 8 steps |
| The chief of staff | `staff` | 5 | 6 | 2 | 5 states, 8 steps |
| The landing | `landing` | 9 | 13 | 1 | 85 states, 99 steps |
| The landing's mutation check | `mutation` | 8 | 13 | 0 | 15 states, 27 steps |
| A PR into master | `master-route` | 10 | 13 | 1 | 28 states, 40 steps |
| Safe delete | `safe-delete` | 3 | 3 | 1 | 3 states, 5 steps |
| A file over the token limit | `ignore-entry` | 7 | 7 | 0 | 7 states, 10 steps |
| The adopter update | `adopter` | 7 | 8 | 0 | 7 states, 10 steps |
| **26 machines** | | **215** | **261** | **12** | |

## GAPs: the 11 spec gaps (12 arrows)

Each is a real gap in the spec, drawn as a TLA+ action (`Gap1`, …) so the states it reaches are
reachable; it never reaches a skill.

1. Project, Watching → Unheld: H4 names idle Projects but never defines when a Project goes idle.
2. Project, Unheld → Watching: H4 says who holds a live Project, not who starts or wakes its admiral.
3. Ticket status, New → FutureFeature, and FutureFeature → unknown: S5 lists FutureFeature, but no rule says what enters it or how a ticket leaves it.
4. Ticket status, New → Backlog: S5 lists Backlog, but no rule says when a ticket is filed there.
5. Landing, Walkthrough → unknown: N6 gives only his acceptance; the route when he does not accept is unstated.
6. A PR into master, Paced → Resync: R29 says the admiral lands PRs one at a time, not how a turn is signalled.
7. Chief of staff, Day → Muster: H7 says the chief of staff runs the overnight watch, not what starts it.
8. Chief of staff, Night watch → Morning brief: H7 and L19 name the watch and the morning brief, not what ends the night.
9. Tribunal, Prepared → Scored: L20 scores his daytime answers against the prediction but names no scorer.
10. Tribunal, Decided overnight → Investigated: L19 has a ruling found wrong investigated but names no investigator.
11. Safe delete, Archived → Live: N10 names no restore from the archive; the built command has one (`dydo archive restore`).

## Notes

- **Who draws an inquisition round's area map** is not stated. The admiral staffs and holds the round (N5, S8), so its verdict turns blockers into Fix rN (S9) and registers shoulds (S17); the map itself is unassigned.
- **CA19, built vs decided.** N10 decides that safe delete covers `~/.t3/worktrees` without breaking T3, and the chart archives a T3 worktree like any other. The built `dydo archive` still refuses every T3-managed target (`T3_RELEASE_UNPROVEN`), owned by DYD-1216.
- **Ticket statuses are system arrows.** The `TicketStatus` machine carries no prose. Each status rule's prose sits on the role claim that makes the move (`Dispatch.Start`, `Project.Replanned`, `FeatureMerge.RealRed`, `Should.Dup`), so no rule is stated twice.
- **The landing's 4.0.0 prerequisites** are one counted guard in `landing`: shoulds closed (S18), mutation clean (T12, N15), the purge done (T8), the evidence history rewritten on his click (N14), DYTEST wired in (Y2, Y6), the principle descriptions written (L6), the backfill page passed (L12), the law shipped and the Upgrade note and re-measure (N8), and the statuses Enablement (N9). The tag also waits on guard hardening (N2).

## Omissions

- **Properties.** No invariant or liveness property is generated: K3 has them written by hand from the axioms and principles. None is written yet, so TLC checks only that no machine deadlocks; K20's "TLC passes the hand-written properties" is still owed.
- **Cross-machine synchronisation is not modelled.** Each machine runs alone. The overview links the areas, and a state shared in prose (a Merge hop that is both a `hops` final and the start of `feature-merge`) is two states in two machines.
- **Bounds.** Counters are bounded for TLC only (review rounds ≤ 4, landing rounds ≤ 3, re-syncs ≤ 3, landing prerequisites = 10). The spec sets no stop on review rounds (R10).
- **Two-valued choices are nondeterministic.** The tribunal's night, two-way, DR-grade, blocking and agreement flags, a blocker's kind, and the cloud route's cost are free choices, not modelled causes.
- **Not modelled as behaviour.** Rules with no state of their own are cited on states, not arrows: R1, H20, N12 (the carried-forward DRs, cited as `N12 DR …` throughout) and N13 (the push's ticket routing). The K29 rules for `dydo/reference/writing-docs.md` (§2.1–2.2) are not built: that file is hand-written and outside the claim-made set (K5).

## Coverage: the spec's §2 areas

| §2 area | Machines |
|---|---|
| 2.1 Claims-built skills (K1–K29) | `claim-build`, `adopter` (K18), `dispatch` (K14), `overview` |
| 2.2 Review and merge (R1–R33) | `review`, `hops`, `merge-command`, `feature-merge`, `master-route`, `dispatch` (R12–R16), `landing` (R20, R28), `findings` (R30) |
| 2.3 Statuses and board (S1–S26) | `ticket-status`, `hops`, `review` (S8–S11), `should` (S17, S18), `liveness` (S24), `findings` (S19–S23, S25, S26), `project` (S14–S16) |
| 2.4 Hierarchy and envoys (H1–H24) | `dispatch`, `envoy`, `staff`, `liveness` (H12), `project` (H4, H5, H8–H11), `findings` (H8, H11), `adopter` (H9) |
| 2.5 Law, councils and tribunal (L1–L23) | `law`, `backfill`, `council`, `tribunal`, `staff` (L19–L21), `mutation` (L14) |
| 2.6 Tests, gates and the token limit (T1–T23) | `proof`, `ignore-entry`, `mutation` (T12), `landing` (T8, T12) |
| 2.7 Descriptive docs (D1–D11) | `docs`, `landing` (D11), `adopter` (D1, D9) |
| 2.8 DYTEST (Y1–Y9) | `proof` (Y4, Y6, Y7), `findings` (Y1, Y3, Y8, Y9), `feature-merge` (Y4, Y9), `landing` (Y2, Y6), `adopter` (Y5) |
| 2.9 The landing (N1–N15) | `landing`, `mutation`, `master-route`, `safe-delete` (N10), `adopter` (N1, N8, N11) |

## Coverage: the spec's §4 acceptance checks

| §4 check | Machines |
|---|---|
| 1. The model builds every file; golden and shape tests; TLC against properties | `claim-build` |
| 2. Nine roles evaluated; survivors rewritten; his approval | `claim-build` (Approved, K22) |
| 3. Seven statuses, auto-close off, the Type labels | `ticket-status`, `hops`, `landing` (N9), `adopter` |
| 4. `dydo merge` enforces R19, R23, R24; the AFK route into master | `merge-command`, `feature-merge`, `master-route` |
| 5. `dydo survey board`, the new findings, `liveness` and its healing; `dydo map` | `findings`, `liveness` |
| 6. The law layout, slug validation, L3 shipping | `law`, `adopter`, `landing` (N8) |
| 7. Only tests meeting the requirements remain, each with its tag | `proof` |
| 8. The gate set on changed files, the 5k nudge and gate | `proof`, `ignore-entry` |
| 9. DYTEST Mutation ships; the landing's mutation check | `mutation`, `landing` |
| 10. `explains:`, flagged by `dydo check` and `pr-ready` | `docs` |
| 11. The four L1 rules in `writing-docs.md` and the working-tree standard | `claim-build` (K29); the working-tree standard is built, `writing-docs.md` is not (see Omissions) |
| 12. DYTEST wired in behind DYD-1071 | `landing` (Y2, Y6) |
| 13. A clean inquisition round; every should closed | `landing`, `should` |
| 14. The committed evidence gone from history | `landing` (N14) |
| 15. Guard hardening before the tag | `landing` (N2) |
| 16. Safe delete, T3 worktrees included | `safe-delete` |
| 17. Principle descriptions; the backfill page | `landing` (L6, L12), `backfill` |
| 18. The Upgrade note and the re-measure | `adopter`, `landing` (N8) |
| 19. The roster, `dydo usage`, the envoy skill table, the council resource | `dispatch` (R13, R14), `envoy` (H23), `council` (L23) |
| 20. The upstream glossary shape, the plan files gone, the ledger retired | `adopter` (D9), `landing` (D11), `findings` (S25) |
| 21. His acceptance at the landing walkthrough; the spec deleted | `landing`, `findings` |

---
area: reference
type: reference
---

# Independent obligations

Written from `Spec: dydo 4.0.0`, first from the copy updated 2026-11-18T22:00:15Z (DYD-1225), then
re-read against the amendment approved at updatedAt 2026-11-20T01:58:55.576Z and his round-5 rulings
(DYD-1082 555a8300, DYD-1195 envoy 5) before the generated actions were read (DYD-1289). Rule IDs below
refer to that document. These are test inputs, never builder output (K3, DR 064 §1).

| Machine | Required behaviour from the spec | Coverage in this PR |
|---|---|---|
| overview | Skills precede the final candidate (K20); inquisition, merge review and acceptance precede landing (N5, N6); cleanup follows landing (S14, D11). | Phase order checked; skills/final-candidate and cleanup unobservable. |
| claim-build | Spec, C# rebuild and spec review precede the push (K20); failed TLC cannot finish the push (K3, K20); final claims approval is his (K16); a candidate he rejects is rebuilt (K16, K22; gap 9). | Current-edit check/approval order and the rejection's rebuild checked; prerequisites, result binding and actor unobservable. |
| role-evaluation | Evaluation precedes rewriting survivors (K22); every accepted outcome requires his approval (K16, K22); a fold or rewrite he rejects returns as a change and is questioned again (K22; gap 9, DYD-1195 8661b8f4). | Rewrite/outcome order and the rejection's re-questioning checked; nine-role set, approver identity and the new captains unobservable. |
| project | Project spec precedes delivery (S14–S16); closeout moves lasting knowledge before deleting the spec (S14); a Project with no live coordinator moves only when he charts it or he or the chief of staff wakes its coordinator (H4; gaps 1–2). | Watch/replan/landing phases and the unheld exits checked; spec-before-delivery, closeout, Paused status and running agents unobservable. |
| dispatch | Dispatch names every skill the role requires (K14); work with children returns to its cutter (H2); crew finish with committed evidence (R32); officer receives the return (H1). | Brief/work/return/re-cut phases checked; skills, authority and commit data unobservable. |
| ticket-status | Working statuses are the seven statuses in S5; dead work is canceled (S10); only he promotes a FutureFeature out (S5; gap 3); an uncontracted Backlog ticket is contracted to Todo before work (S5; gap 4). | Start/Done order, structural terminal guard, FutureFeature exits and Backlog exits checked; status data, reasons, promoter and contract unobservable. |
| hops | Hop Type/kind is stable (S3); steps obey Implement → Review → Fix → fresh Review → Merge order (S3, R8); independent agents judge steps (R33). | Hop order checked; persistent kind and author/reviewer identities unobservable. |
| proof | Graduation requires a demonstrated red and a job tag (T1, T3, T5); lint precedes more expensive proof (T2); receipts identify the checked state (T5, Y4); mutation never gates ordinary work (T12). | New/rewritten/conformance red and receipt order checked; lint order, tags and receipt SHA unobservable. |
| docs | Descriptive docs derive from code, never authorize it (D2); each declares code it explains (D5); drift is fixed or cut (D6, D7). | Invitation/admission/declaration/repair phases checked; content and path data unobservable. |
| review | FAIL requires a blocker (R11); fix between rounds (R8); security gets security first (R9); round 3 is a checkpoint (R10); no author judges its own work (R33). | Kinds, fix order, security and checkpoint checked; no-self-review and immediacy unobservable. |
| should | Duplicate check precedes registration; batching preserves origin (S17); landing requires resolution or his move (S18). | Registration/disposition phases checked; duplicate lookup, origin and authorization unobservable. |
| merge-command | Accepted merge requires current/valid ancestor PASS, clean tip merge, canonical merge tree and green combined-tree receipt (R17, R19, R23, R24, Y4). | Structural invocation-result guards only; merge entry conditions unobservable. |
| feature-merge | No merge review (R18); Done requires green merge-tree proof and hosted CI (R20); rerun red once, revert real red, fix and review before relanding (R22). | Per-attempt gates/rerun/revert/fix order checked; actual receipts and CI unobservable. |
| findings | Stateless read only survey (S26); real red becomes a ticket, flakes counted until repeated (Y9); landed mistakes get prevention (R30). | Flake/red/DYTEST routing phases checked; effects, thresholds and prevention data unobservable. |
| liveness | Quiet work is checked at two hours; alive snoozes, record behind heals, stopped resets and resumes/redispatches (S24); eventual healing/redispatch (S24, K28). | Restart order and healing progress checked; clock, comments and links unobservable. |
| map | Order computed, never stored (S20); cache failure cannot prevent a fresh read, Linear stays authoritative (S26). | Fresh-read/display order checked; ordering algorithm, effects and cache failures unobservable. |
| envoy | Every need passed up becomes a research Question its officer opens and waits on, at every level, and every Question starts with research (H16, L18; DYD-1195 317eb49f, 3a854e37); a council or his-call child only after its research (H23); an obvious, reversible call is decided where it arises, never ticketed, and never by the coordinator (H16, H5); only the coordinator dispatches envoys, from its HITL queue (H16, H18); one active envoy, and only his closing a reported thread frees the queue, with no moved-on exit (H18; 893e1f5b); report before close (H19). | Research-first, coordinator level, child-after-research, decided-unticketed, counter, close-frees-queue and report order checked; skill, identity, session, bundle membership and ranking data unobservable. |
| council | Sealed fresh read-only seats; synthesis by a non-member; report confidence and disagreements (L14); single-family fallback does not block work (L22). | Answers-before-synthesis order checked; seats, seal/read-only and confidence data unobservable. |
| tribunal | One exchange before a council's ruling (L18); provisional calls require agreement on a two-way call, overnight or by day when not worth his time, and DR-grade adds night and serious blocking (L18, L20, L21; DYD-1195 87c1e2e6); questions never ratified (L19); daytime decisions scored (L20); wrong rulings investigated (L19); only a proposed DR is declined (L21; gap 8). | Exit predicates, exchange and ratification order checked; identities/evidence unobservable. |
| law | Evidence precedes proposal (L11); authority belongs to him for principles/axioms (L13); existing slugs never renamed/reused (L3); retirement preserves history (L11). | Proposal/enactment/case-study/retirement order checked; slugs, authority and history content unobservable. |
| slug-check | Invalid applies/weighed slugs fail check; correction rechecked (L8). | Correction phase order plus structural terminal guard; slug validity unobservable. |
| backfill | Only accepted DRs tagged; weighed quotes existing trade-off; citation review and his row approval precede merge (L12). | Per-row checking/page/merge order checked; quoted content, DR status and approver unobservable. |
| staff | Short thread-link attention list, no content relay or envoys (H7); decided Questions not listed for ratification (L19); dr-grade calls surface at muster (L21); the night starts with his muster and ends on his return in the morning brief (H7; gaps 6–7). | Day/brief return phases and the night's start and end checked; list content, envoy effects, his return and ratification data unobservable. |
| landing | Fresh feature map and Read → Break → Drive per slice each round (N5); no blockers/shoulds before merge review; his acceptance before merge (N6); a walkthrough he does not accept never merges, and another landing follows (N6; gap 5); all prerequisites including complete mutation before landing/tag (N7, N15); guard hardening before his tag (N2, R28). | Slice phases, count, merge/walkthrough order, the rejection's new landing and hardening flag checked; actual prerequisite data, findings tickets and actors unobservable. |
| mutation | Missing/stale work queued for exact tree, unfinished is incomplete and blocks landing (T12, N15); equivalents individually independently approved and lapse on changes; dispute to council (T12). | Each-run queuing and campaign run/claim order checked; tree/approval/scheduling data unobservable. |
| master-route | Both doors wait for turn and current-master recheck (R29); third resync held (R29); two-way complete evidence AFK (R26); one-way green CI and recovery plan before his click (R27, R31). | Each-attempt recheck/CI order, door and third-resync hold checked; actual evidence/recovery plan unobservable. |
| safe-delete | Archive before purge, retained for thirty days (N10); T3 worktrees included (N10); `dydo archive restore` brings back only what the archive holds (N10; gap 10, DYD-1195 70df9ff8). | Archive-before-purge, restore-from-archive and structural terminal guard checked; days, T3 integrity and purge safety unobservable. |
| ignore-entry | Generated ignores have reason and review (T17); authored ignores require separate ticket, council then his blessing (T18). | Exception path order checked; size, reason, verdict and blessing data unobservable. |
| adopter | Tailored files consult user before replacement (N11); downstream contains built runtime assets, never builder, model, tests or why docs (K18); Upgrade note accompanies release (N8). | Update/consult phases checked; payload, release version and user decision unobservable. |
| host-setup | Native memory denied; guard installed in committed settings (N12 DR 055, N12 DR 056). | Initialization/guard/denial order checked; settings contents/effects unobservable. |
| statechart-run | Missing dependencies require authorized captured-request continuation; no unrun check called pass (N12 DR 062, K3, K20). | Setup/continuation/check/result order checked; actual authorization, identities and drain evidence unobservable. |

All ten statechart GAPs are closed (DYD-1082 555a8300 change 14). Each closed GAP's obligation above was
written from his recorded words, before its generated move was read, and has a property.

## Adaptations at DYD-1289

The rebuild renamed roles and phases and re-shaped the envoy machine (DYD-1317). Each property that
changed, with its old and new wording and why:

| Property | Old | New | Why |
|---|---|---|---|
| `H16_HomeworkBeforeEnvoy` → `H16_ResearchBeforeEnvoy` | The admiral does homework before sending an envoy: `"Homework" \in seen`. | Every Question starts with research, so no envoy goes out before a research Question: `"Research" \in seen`. | The promise changed on purpose: H16 now supersedes "the officer above does the homework first"; every level opens a research Question for a fresh captain (DYD-1195 317eb49f, 3a854e37; L18, 87c1e2e6). |
| `H16_OnlyAdmiralSends` → `H16_OnlyCoordinatorSends` | Only an admiral dispatches the active envoy: `level = "admiral"`. | Only the coordinator dispatches envoys, from its HITL queue: `level = "coordinator"`. | The role was renamed (H5, DYD-1082 555a8300 Q20). The old check named a level the model never had once DYD-1284 renamed it. |
| `R29_HoldThirdResync` (comment) | "held for admiral sequencing". | "held for the coordinator's sequencing". | Role rename (H5). Formula unchanged. |
| `H5_WatchBeforeReplan` (comment) | "the admiral's watch". | "the coordinator's watch". | Role rename (H5). Formula unchanged. |
| `N5_ReadBeforeBreak`, `N5_BreakBeforeDrive`, `N5_FreshMapAfterFix` (comments) | "An area's" phases; "a fresh area map". | "A slice's" lenses; "a fresh feature map". | N5 now cuts the inquisition into slices of a feature map, with Read, Break and Drive as reviewer lenses (DYD-1082 1bc04dce change 5). Formulas unchanged. |
| `controls.json` envoy control | Named `H16_OnlyAdmiralSends`. | Names `H16_OnlyCoordinatorSends`; the same bad trace, a need sent straight to an envoy at captain level. | Follows the rename above. |

Added, each from the rule it cites: `H23_ChildAfterResearch`, `H16_ObviousNeverTicketed`,
`H5_CoordinatorDecidesNone` and `H18_OnlyHisCloseFrees` (envoy; the last replaces the struck "moved
on" discretion, DYD-1195 893e1f5b); for the closed GAPs, `H4_UnheldOnlyChartedOrWoken` (gaps 1–2),
`S5_FutureLeavesOnlyByPromotion` (gap 3), `S5_BacklogNeverWorked` (gap 4), `N6_NotAcceptedLandsAgain`
(gap 5), `H7_NightStartsAtMuster` (gap 6), `H7_NightEndsInMorningBrief` (gap 7),
`K22_RejectedQuestionedAgain` and `K16_RejectedRebuilt` (gap 9), and `N10_RestoreOnlyArchived` (gap 10).
Gap 8 already had `L21_DeclineOnlyProposed` (DYD-1286). No property was dropped or weakened.

## Related

- [Property modules, execution and limits](README.md)

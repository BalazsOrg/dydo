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
| `L18_ExchangeBeforeRuling` | Every tribunal result follows the exchange: `state \in {"DecidedOvernight", "Prepared", "ProposedDr"} => "Exchange" \in seen`. Commit 9f545e02 dropped `"Prepared"` without a record, so a council could reach Prepared with no exchange (review r1 B2). | Every council exit follows the exchange; only the his-call route to Prepared, with no case file, is exempt: `(state \in {"DecidedOvernight", "ProposedDr"} \/ (state = "Prepared" /\ "CaseFile" \in seen)) => "Exchange" \in seen`. | The model changed shape: a his-call child reaches Prepared straight from the Question's research, and no council sits (`Tribunal.HisCall`; H23, DYD-1195 87c1e2e6). L18's exits, "prepared for him with both positions" included, still follow the one exchange, so the council path keeps the requirement. DYD-1327 restored it: red on Codex's CaseFile → Prepared shortcut, green on the model. |
| `L20_NightOnlyProvisional` → `L18_AgreedTwoWayProvisional` | Only night, agreement and a two-way call permit a provisional decision: `state = "DecidedOvernight" => night /\ twoWay /\ agree /\ ~drGrade`. | Only agreement on a two-way call that is not DR-grade permits it, by night or by day: `state = "DecidedOvernight" => twoWay /\ agree /\ ~drGrade`. | The promise changed on purpose: the amended L20 and L18's first exit let a council decide by day too when the call is not worth his time, "during the night (or day for that matter so I can be unburdened)" (DYD-1195 87c1e2e6). Only the `night` conjunct is dropped. Made at 9f545e02; recorded at DYD-1327. |

Added, each from the rule it cites: `H23_ChildAfterResearch`, `H16_ObviousNeverTicketed`,
`H5_CoordinatorDecidesNone` and `H18_OnlyHisCloseFrees` (envoy; the last replaces the struck "moved
on" discretion, DYD-1195 893e1f5b); for the closed GAPs, `H4_UnheldOnlyChartedOrWoken` (gaps 1–2),
`S5_FutureLeavesOnlyByPromotion` (gap 3), `S5_BacklogNeverWorked` (gap 4), `N6_NotAcceptedLandsAgain`
(gap 5), `H7_NightStartsAtMuster` (gap 6), `H7_NightEndsInMorningBrief` (gap 7),
`K22_RejectedQuestionedAgain` and `K16_RejectedRebuilt` (gap 9), and `N10_RestoreOnlyArchived` (gap 10).
Gap 8 already had `L21_DeclineOnlyProposed` (DYD-1286). No property was dropped. One was weakened
without a record: `L18_ExchangeBeforeRuling` lost its council Prepared exit at 9f545e02, and DYD-1327
restored it. `L18_AgreedTwoWayProvisional` allows a day decision only because the promise changed.

## Checked at DYD-1327

Fix r1's claims change three machines' arrows: a should's cancellation moves from the coordinator
to its batch captain (`Should.Cancel`; S17, S18, H5), and the overview's `Cut` and `Next` and
mutation's `CutFix` are relabelled (H2, H4, H5). No generated `.tla` changed, and no machine carries
an actor. The should, overview and mutation properties still state their spec obligations, unchanged.
Who cancels a should stays unobservable.

## Captain prototype (DYD-1372)

These obligations were written from the DYD-1295 walkthrough rulings, the approved
4.0.0 spec and the accepted `AGENTS.md` before reading the generated captain
machine at `278b89da`. The observer runs over the generated `Init`, `Next` and
`round <= 4` bound. The model declares no fairness, so these checks are safety
properties.

| Source | Obligation and result |
|---|---|
| S6, 7b3d2e3e | A holder closes Done only after meeting the contract. **Phase order holds** (`S6_WorkBeforeDone`). The contract, evidence and acceptance predicate are unobservable. The trace Taken → Leaf → Finishing → Done looks identical whether or not the contract was met. |
| R2, R8, R12, R16 | A change that needs review clears it before finish or merge; review the integrated whole after wiring. Group-only parents and sufficiently small, low-risk leaves can skip. **First finish after entering review holds** (`R8_ClearBeforeFirstFinish`); InReview → Finishing stands for a clear round. Review eligibility, wiring, own vs. parent merge and proof tied to a revision are unobservable. Taken → Split → Waiting → Finishing → Done cannot distinguish a grouping ticket from a review-required feature. |
| R4 | A plan, spec or contract has no review loop. A spec may get one risk-based scout pass that returns Questions. **Untestable.** Taken → Leaf → InReview carries neither ticket kind nor a separate spec pass or its answers. |
| R8, R9, R11, S9, 41a553f1 | A fixable blocker requires a fix before the next full-lens round. **Numbered round order holds** (`R8_FixBeforeNextRound`). InReview → Fixing → InReview gives no evidence of blocker severity, completeness, repair proof or a fresh reviewer and lenses. InReview → Returned cannot distinguish a fundamental issue, a new requirement and a human need. Shoulds that may wait have no status. The optional extra round after a clear round has no explicit route. |
| R10, S10 | Record the third-round checkpoint before starting round four or escalating. **Both checks fail.** `R10_CheckpointBeforeFourthRound` — Taken → Leaf → InReview(1) → Fixing(1) → InReview(2) → Fixing(2) → InReview(3) → Fixing(3) → InReview(4). `S10_CheckpointBeforeThirdRoundEscalation` — the same prefix through InReview(3), then Returned(3). The chart has no checkpoint observation, note or escalation criterion. Its round bound also prevents TLC from testing continuation beyond a blocked fourth round. |
| R19, R21 | A merge-conflict repair gets a fresh review before retrying. **Fails** (`R19_ReviewAfterMergeRepair`), Taken → Leaf → Finishing → Waiting → Finishing. The single edge into Waiting covers both a conflict and a real red gate, and the grouping-only exit bypasses review in either case. |
| 2119cd65, df6a785e, 32cc5599, R16, R33, S8 | An orchestrating captain dispatches or records reviews; the reviewer judges and never authored or fixed the work. **Chart inspection only.** No arrow assigns judging to the orchestrating captain. Taken → Leaf → InReview → Finishing carries no writer, reviewer or fixer identity, lone or multiple seats, verdict origin or authorship, so TLC cannot verify independence or fresh-writer routing. The later ruling allows a sub-captain to author a small leaf. |
| 519126ab, H16, S10 | Only obvious, reversible calls are decided locally. A non-obvious call goes to a fresh Question captain; the dependent work resumes after the answer. **Leaf wait/resume order holds** (`H16_ResumeLeafAfterQuestion`). Leaf → Question → Leaf carries no call classification, answer, Question holder or blocked scope. This path exists only from Leaf, leaving the split captain's non-obvious calls, re-plan questions and passed-up human needs unrepresented. |
| df6a785e, 0f340abd, 3fb363e3, AGENTS.md, S7, H2, H3 | A split dispatches its children and waits for their required returns. **Wait phase holds** (`H3_WaitAfterSplit`). Taken → Split → Waiting → InReview carries no child identities, completion or receipt evidence, context size, cohesion, native/delegated distinction or cut authority. Leaf → Finishing is always enabled, so TLC cannot establish that oversized work is cut or returned instead of finished. |
| H2, 7b3d2e3e | A return ends this holder's run. **Holds** (`H2_ReturnedIsTerminal`); Returned only stutters. |

Two bad-trace controls use the same observer entry-point injection. Taken → Done
must break `S6_WorkBeforeDone`; Question → Finishing must break
`H16_ResumeLeafAfterQuestion`. Both use round 1.

## Related

- [Property modules, execution and limits](README.md)

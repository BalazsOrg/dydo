---
area: reference
type: reference
---

# Independent obligations

Written from `Spec: dydo 4.0.0`, updated 2026-11-18T22:00:15Z, before reading
the generated actions. Rule IDs below refer to that document. These are test
inputs, never builder output (K3, DR 064 §1).

| Machine | Required behaviour from the spec | Coverage in this PR |
|---|---|---|
| overview | Skills precede the final candidate (K20); inquisition, merge review and acceptance precede landing (N5, N6); cleanup follows landing (S14, D11). | Phase order checked; skills/final-candidate and cleanup unobservable. |
| claim-build | Spec, C# rebuild and spec review precede the push (K20); failed TLC cannot finish the push (K3, K20); final claims approval is his (K16). | Current-edit check/approval order checked; prerequisites, result binding and actor unobservable. |
| role-evaluation | Evaluation precedes rewriting survivors (K22); every accepted outcome requires his approval (K16, K22). | Rewrite/outcome order checked; nine-role set and approver identity unobservable. |
| project | Project spec precedes delivery (S14–S16); closeout moves lasting knowledge before deleting the spec (S14). Exclude both H4 gaps. | Watch/replan/landing phases checked; spec-before-delivery and closeout unobservable. |
| dispatch | Crew load their own skill (K14); work with children returns to its cutter (H2); crew finish with committed evidence (R32); officer receives the return (H1). | Brief/work/return/re-cut phases checked; skills, authority and commit data unobservable. |
| ticket-status | Working statuses are the seven statuses in S5; dead work is canceled (S10). Exclude FutureFeature and Backlog entry/exit gaps. | Start/Done order checked plus structural terminal guard; status data/reasons unobservable. |
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
| envoy | Homework before escalation; fresh HITL job, result recorded, reported then closeable (H15, H16, H19); explicit moved-on discretion permits next envoy (H18). | Homework, admiral level, counter and report order checked; skill/identity/session data unobservable. |
| council | Sealed fresh read-only seats; synthesis by a non-member; report confidence and disagreements (L14); single-family fallback does not block work (L22). | Answers-before-synthesis order checked; seats, seal/read-only and confidence data unobservable. |
| tribunal | One exchange before ruling (L18); provisional calls require night, agreement and two-way; DR-grade adds serious blocking (L18, L20, L21); questions never ratified (L19); daytime decisions scored (L20); wrong rulings investigated (L19). Exclude declined DR gap. | Exit predicates, exchange and ratification order checked; identities/evidence unobservable. |
| law | Evidence precedes proposal (L11); authority belongs to him for principles/axioms (L13); existing slugs never renamed/reused (L3); retirement preserves history (L11). | Proposal/enactment/case-study/retirement order checked; slugs, authority and history content unobservable. |
| slug-check | Invalid applies/weighed slugs fail check; correction rechecked (L8). | Correction phase order plus structural terminal guard; slug validity unobservable. |
| backfill | Only accepted DRs tagged; weighed quotes existing trade-off; citation review and his row approval precede merge (L12). | Per-row checking/page/merge order checked; quoted content, DR status and approver unobservable. |
| staff | Short thread-link attention list, no content relay or envoys (H7); decided Questions not listed for ratification (L19); proposed DRs surface at muster (L21). Exclude night start/end gaps. | Day/brief return phases checked; list content, envoy effects and ratification data unobservable. |
| landing | Fresh area map and Read → Break → Drive per round (N5); no blockers/shoulds before merge review; his acceptance before merge (N6); all prerequisites including complete mutation before landing/tag (N7, N15); guard hardening before his tag (N2, R28). Exclude rejected walkthrough gap. | Area phases, count, merge/walkthrough order and hardening flag checked; actual prerequisite data and actors unobservable. |
| mutation | Missing/stale work queued for exact tree, unfinished is incomplete and blocks landing (T12, N15); equivalents individually independently approved and lapse on changes; dispute to council (T12). | Each-run queuing and campaign run/claim order checked; tree/approval/scheduling data unobservable. |
| master-route | Both doors wait for turn and current-master recheck (R29); third resync held (R29); two-way complete evidence AFK (R26); one-way green CI and recovery plan before his click (R27, R31). | Each-attempt recheck/CI order, door and third-resync hold checked; actual evidence/recovery plan unobservable. |
| safe-delete | Archive before purge, retained for thirty days (N10); T3 worktrees included (N10). Exclude restore gap. | Archive-before-purge plus structural terminal guard; days, T3 integrity and purge safety unobservable. |
| ignore-entry | Generated ignores have reason and review (T17); authored ignores require separate ticket, council then his blessing (T18). | Exception path order checked; size, reason, verdict and blessing data unobservable. |
| adopter | Tailored files consult user before replacement (N11); downstream contains built runtime assets, never builder, model, tests or why docs (K18); Upgrade note accompanies release (N8). | Update/consult phases checked; payload, release version and user decision unobservable. |
| host-setup | Native memory denied; guard installed in committed settings (N12 DR 055, N12 DR 056). | Initialization/guard/denial order checked; settings contents/effects unobservable. |
| statechart-run | Missing dependencies require authorized captured-request continuation; no unrun check called pass (N12 DR 062, K3, K20). | Setup/continuation/check/result order checked; actual authorization, identities and drain evidence unobservable. |

The ten marked gaps (thirteen arrows) in `claims/statechart/README.md` have no
test oracle here. No liveness obligation is imposed on those exits.

## Related

- [Property modules, execution and limits](README.md)

---
area: reference
type: reference
explains: ["verification/statechart/**"]
---

# Hand-written 4.0.0 properties

These modules are independent test inputs for DYD-1225 (K3, K20, DR 064 §1),
adapted to the rebuilt roles and the closed GAPs at DYD-1289, and re-checked after Fix r1 at DYD-1327.
The checked model is the claim model at `81ea4f84ecec92fc69a9d74792e26fc4d6f45fe9`
(Fix r1 merged, DYD-1327 on `DYD-1281-fix-r1-integrate`); the property run records its own clean source HEAD.
Authority: [Spec: dydo 4.0.0](https://linear.app/bodnar-balazs/document/spec-dydo-400-08326afbb349),
approved at updatedAt 2026-11-20T01:58:55.576Z, and his rulings above it. Each property has a rule ID
and a plain-language obligation beside its definition. [obligations.md](obligations.md) records the
spec-first requirements, including requirements the current model cannot test, and every adaptation.

## Inputs and convention

Each `<machine>/<Machine>Properties.tla` extends the generated module. Its
adjacent `.cfg` registers `INVARIANT` and `PROPERTY` checks using the statechart
skill's configuration interface. The runner copies the original generated
module and its configuration to scratch, preserves its `CONSTRAINT Bounds`,
changes only the entry points to the observer wrapper, and appends this property
registration. Neither properties nor registrations live under the builder's
`claims/statechart/` output. No model build produces these files.

One machine is not generated. The captain's chart and model are written by hand
in `statechart/captain/` (DYD-1437), and the runner finds them there. Its
properties stay here, in `captain/`, apart from the model they judge.

`seen` is a passive set of visited phases. Initially it holds the initial phase.
Each step adds its destination; claim-build resets it on a new `Edited` phase,
feature-merge on `PreGate`, master-route on `Offered`, and mutation on `Queued`.
These reset points scope checks to the current edit, merge attempt or queued
campaign. Per-occurrence order also uses action properties: an earlier queue,
CI watch, citation check or rerun cannot stand for a later occurrence.
Each generated step has exactly one observer extension; projecting out `seen` gives the original model.
No action is disabled, no bound is reduced, and the runner adds no fairness assumption of its own.
Where the model declares fair moves (its config says `SPECIFICATION Spec`), a property run checks
`PropertyInit /\ [][PropertyNext]_<<vars, seen>> /\ Fairness` with the model's own `Fairness`;
controls keep `INIT`/`NEXT`, because their bad traces replace the model's moves.
The model's `Init` and `Next` remain authoritative. Action-safety properties use
TLA+'s `[][A]_v` form; the two S24 progress obligations use leads-to (`~>`).

History properties check phase order only. A phase called `Approved` does not
prove who approved it. Seeing `Gating` does not prove a green receipt exists.
All ten GAPs are closed (DYD-1082 555a8300); each closed arrow has a property
written from his ruling, listed in [obligations.md](obligations.md). Properties are
local to each machine; none establishes cross-machine synchronization.

Nine checks prefixed `Structural_` guard this fixture's terminal phases or
forbidden phase names. They are deliberately labeled structural checks, not
proofs of missing data or content. In particular, the merge-command checks
only protect the result of one invocation; they do not establish R23's entry
conditions. An absent mutation/merge-review phase cannot establish that a
hidden data-level dependency is absent.

## Run

Use an absolute path for this checkout in place of `REPO` and a fresh scratch
output directory each time. The runner does not acquire a second lock: the host
wrapper holds its existing gate slot for the whole serial run.

```text
mkdir -p /tmp/dyd1289-chartroot
dydo statechart install-skill --root /tmp/dyd1289-chartroot
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --baseline --skill /tmp/dyd1289-chartroot/skills/engineering/statechart --out /tmp/dyd1289-baseline
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --skill /tmp/dyd1289-chartroot/skills/engineering/statechart --out /tmp/dyd1289-properties
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --controls --skill /tmp/dyd1289-chartroot/skills/engineering/statechart --out /tmp/dyd1289-controls
```

On this systemd host, the recorded commands also wrap Python, inside the lock, with
`systemd-run --user --scope --slice=app-t3code-agents-cbc6c2286251.slice -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600`.
That slice is session-specific. The runner sets `JAVA_TOOL_OPTIONS` to
`-Xmx512m -XX:+UseParallelGC`. dydo selects and verifies its managed Java and TLC;
there is no direct TLC fallback. Follow the installed statechart skill if dydo
returns an approval-required dependency plan.

Each machine gets immutable copied inputs, a driver log, a statechart report,
raw TLC output and any per-property counterexample outputs. `runs.json` records
the clean source HEAD, runner hash, argv, input hashes, exits and semantic results.
It refuses a dirty checkout before running and blocks every result if the
checkout or HEAD changes during the run. An error, timeout, missing
report or unchecked property is blocked, never pass. The command exits nonzero
if any ordinary check fails. Keep output in scratch or upload it as a CI
artifact; never commit checker output (N14).

`--controls` uses the separately hand-written bad traces in
[controls.json](controls.json). It replaces only the scratch observer entry
points to inject a forbidden shortcut (or permanent stopped-hop stuttering).
It must catch the named property on every machine. These are oracle sensitivity
checks, not claims that the generated model actually takes those steps, and
not a substitute for the ordinary run. `--machine NAME` restricts a diagnostic
run; the acceptance run includes all 32 machines.

The hand-written captain chart is a separate diagnostic scope. Run the
ordinary check with `--machine captain` and run the registered control with
`--controls --machine captain`. The registered trace tries to finish with a
Question still pending. Its other controls are one changed line each in a
scratch copy of the model. The commands are in
[the captain chart's README](../../statechart/captain/README.md); the properties,
controls, results and model limits are in
[the captain obligations](captain/obligations.md).

## Model observability owed to the candidate captain

The following are unresolved model coverage defects, separate from the ten
spec gaps, which are now closed. Passing the phase checks cannot close these
obligations. A property author cannot repair them without changing the model,
which a properties-only task does not do.

| Required rule | Missing state or distinction |
|---|---|
| R17, R19, R23, R24, Y4 | `merge-command` has only `state`. It cannot distinguish PASS SHA, head, target tip, permitted sync ancestry, Review Done, canonical merge tree, or receipt tree/status. `MergeCommand_Merge` is enabled by `Invoked` alone. Refusal names do not enforce their conditions. |
| S3, S6, S9, R33, K29 | `hops` has no persistent hop identity, immutable Type, holder, author or reviewer identity, per-hop status, blocker status or proof SHA. Review has no reviewer/author identity either. Phase movement between different hops cannot test one hop's kind staying fixed or independence. |
| T12, N15, N7 | `landing.prereqs` counts ten anonymous increments. There is no mutation status, DYTEST readiness, exact landing tree or survivor list to connect to the separate mutation machine. A count of ten cannot establish that incomplete mutation blocks landing. |
| T12 | `mutation` has no mutant identity, source/test author, reviewer identity, reason, approval/tree binding or invalidation cause. Phase order cannot establish individual independent equivalent approvals or expiry when code changes. |
| S24, S13, H12 | `liveness` has no clock, two-hour quiet interval, liveness comment, alive/behind distinction, current/history thread links or parent-ticket no-hop case. Its `Spec` has no fairness/progress assumption; a restarted hop can stutter forever. |
| R26–R29, R31 | `master-route` distinguishes doors and counts re-syncs. Per-occurrence action checks establish the recheck/CI phase order on each attempt. Actual current master/tree evidence, recovery plan, CI state, acceptance/click identity and concurrency remain absent. |
| R20, R22, Y9 | `feature-merge` has phases but no hosted-CI SHA/status, merge-tree receipt, real-red/flake result or revert SHA. Only its ordering is testable. |
| K3, K16, K20, K22 | Claim-build and role-evaluation phases carry no candidate SHA, property-run result, approver identity, nine-role coverage or prerequisite completion. They cannot bind acceptance to this candidate's actual TLC result. |
| T1–T5, Y4 | Proof has no separate lint stage, job-tag data, receipt SHA/tree or selected-test outcomes. Red and receipt phase order is checked for newly written and conformance tests, including rewritten Old tests. T8's direct Keep is measurement of a retained test, not graduation; no old receipt is used as proof. |
| K20, N5, N6, S14, D11 | Overview has areas rather than skill-build/candidate acceptance or knowledge-migration/spec-deletion states. Its nominal review/merge and landing/master order is checked, but skills-before-final-candidate and closeout cleanup are not observable. |
| S14–S16, H5 | Project has no spec-exists/contract quotation/lasting-knowledge fields or cleanup states. Its watch/replan/landing phase order is checked; spec-before-delivery and migrate-before-delete cannot be checked. Gap 1's idle causes, Paused status and running agents, are absent; only the unheld exits are checked. |
| K14, H1–H3, R13–R16, R32 | Dispatch lacks agent role/family, context, roster/usage, native/delegated distinction, owned checkout, child authorization, loaded skill, committed SHA and return recipient. |
| S5–S18 | Ticket/should phases lack Type/Mode labels, holder identity, cancellation reason, duplicate lookup, batch/origin links and his move authorization. |
| D2–D7 | Docs phases lack actual content, invitation, admission predicates, `explains:` paths, code changes and linked reasons. |
| S19–S26, Y9, R30 | Map/findings lack fetched tickets, ordering/cache flags, read/write effects, age/count thresholds and prevention measures. |
| H15–H19, H22–H24, L14–L22 | Envoy/council/tribunal phases lack seat identity, freshness, family availability, sealed/read-only effects, non-member synthesis, job skill, actual agreement inputs, evidence, or result recording. Tribunal booleans permit testing the exit predicates only. The envoy machine follows one need with a holder level and an active-envoy counter: it has no Question identity, stage label data, `dr-grade` flag, bundle membership, waiting-ticket counts or estimates for H18's ranking, and its council child ends where the tribunal's route begins. |
| L3, L8–L13 | Law/slug/backfill lack slug sets, wording, quoted DR text, citation validity, authority and row approval. Phase order cannot validate the content. |
| H7, L19, L21 | Staff has only phases; its list content, envoy prohibition, ratification distinctions and escalation routes are absent. |
| N10 | Safe-delete has no elapsed days, archive retention data, T3 state or purge safety. Only archive-before-purge can be checked. |
| T15–T19 | Ignore-entry lacks byte size, generated/authored classification, reason, separate ticket, council verdict, blessing and stale-entry gate state. |
| K18, N1, N8, N11 | Adopter lacks release version, tailored-file decisions and payload contents. A normal update can go directly to Current; major-release requirements cannot be imposed without a version premise. |
| N12 DR 055, DR 056, DR 062 | Host/tool phases lack settings contents, authorization, immutable identities and drain evidence. Ordering checks cannot establish their actual values. |

These machines describe intended workflows, rather than an executable model of
the product implementation. A failure is a model/spec finding; reproducing a
product defect requires a separate implementation-level check.

## Related

- [Independent obligations](obligations.md)
- [Statechart skill](../../skills/engineering/statechart/SKILL.md)
- [Maintaining formal verification](../../skills/engineering/maintain-formal-verification-skill/SKILL.md)

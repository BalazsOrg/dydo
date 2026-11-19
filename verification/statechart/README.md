---
area: reference
type: reference
explains: ["verification/statechart/**"]
---

# Hand-written 4.0.0 properties

These modules are independent test inputs for DYD-1225 (K3, K20, DR 064 §1).
The checked model is PR 376 at `0feabe79f2f6f3c748ccac693219034b9d5add4f`.
Authority: [Spec: dydo 4.0.0](https://linear.app/bodnar-balazs/document/spec-dydo-400-08326afbb349),
updated 2026-11-18T22:00:15Z. Each property has a rule ID and a plain-language
obligation beside its definition. [obligations.md](obligations.md) records the
spec-first requirements, including requirements the current model cannot test.

## Inputs and convention

Each `<machine>/<Machine>Properties.tla` extends the generated module. Its
adjacent `.cfg` registers `INVARIANT` and `PROPERTY` checks using the statechart
skill's configuration interface. The runner copies the original generated
module and its configuration to scratch, preserves its `CONSTRAINT Bounds`,
changes only the entry points to the observer wrapper, and appends this property
registration. Neither properties nor registrations live under the builder's
`claims/statechart/` output. No model build produces these files.

`seen` is a passive set of visited phases. Initially it holds the initial phase.
Each step adds its destination; claim-build resets it on a new `Edited` phase,
feature-merge on `PreGate`, master-route on `Offered`, and mutation on `Queued`.
These reset points scope checks to the current edit, merge attempt or queued
campaign. Per-occurrence order also uses action properties: an earlier queue,
CI watch, citation check or rerun cannot stand for a later occurrence.
Each generated step has exactly one observer extension; projecting out `seen` gives the original model.
No action is disabled, no bound is reduced, and no fairness assumption is added.
The model's `Init` and `Next` remain authoritative. Action-safety properties use
TLA+'s `[][A]_v` form; the two S24 progress obligations use leads-to (`~>`).

History properties check phase order only. A phase called `Approved` does not
prove who approved it. Seeing `Gating` does not prove a green receipt exists.
The ten marked gaps (thirteen arrows) are preserved, with no oracle for their
entry/exit decision. Properties are local to each machine; none establishes
cross-machine synchronization.

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
mkdir -p /tmp/dyd1225-chartroot
dydo statechart install-skill --root /tmp/dyd1225-chartroot
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --baseline --skill /tmp/dyd1225-chartroot/skills/engineering/statechart --out /tmp/dyd1225-baseline-new
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --skill /tmp/dyd1225-chartroot/skills/engineering/statechart --out /tmp/dyd1225-properties-new
/var/tmp/dydo-gate-lock.sh python3 REPO/verification/statechart/run.py --controls --skill /tmp/dyd1225-chartroot/skills/engineering/statechart --out /tmp/dyd1225-controls-new
```

On this systemd host, the recorded commands also wrap Python with
`systemd-run --user --scope --slice=app-t3code-agents-cbc6c2286251.slice -p MemoryMax=2G -p MemorySwapMax=0 -p OOMPolicy=continue`.
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
run; the acceptance run includes all 31 machines.

## Model observability owed to the candidate captain

The following are unresolved model coverage defects, separate from the ten
marked spec gaps. Passing the phase checks cannot close these obligations.
The property author cannot repair them without changing the model, which this
task expressly forbids.

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
| S14–S16, H5 | Project has no spec-exists/contract quotation/lasting-knowledge fields or cleanup states. Its watch/replan/landing phase order is checked; spec-before-delivery and migrate-before-delete cannot be checked. The two H4 gap arrows remain unconstrained. |
| K14, H1–H3, R13–R16, R32 | Dispatch lacks agent role/family, context, roster/usage, native/delegated distinction, owned checkout, child authorization, loaded skill, committed SHA and return recipient. |
| S5–S18 | Ticket/should phases lack Type/Mode labels, holder identity, cancellation reason, duplicate lookup, batch/origin links and his move authorization. |
| D2–D7 | Docs phases lack actual content, invitation, admission predicates, `explains:` paths, code changes and linked reasons. |
| S19–S26, Y9, R30 | Map/findings lack fetched tickets, ordering/cache flags, read/write effects, age/count thresholds and prevention measures. |
| H15–H19, H22–H24, L14–L22 | Envoy/council/tribunal phases lack seat identity, freshness, family availability, sealed/read-only effects, non-member synthesis, job skill, actual agreement inputs, evidence, or result recording. Tribunal booleans permit testing the exit predicates only. |
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

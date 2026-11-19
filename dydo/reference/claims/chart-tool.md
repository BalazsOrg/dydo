---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: ChartTool

## Invalid

`claim:ChartTool.Invalid`: In a dydo statechart run, an explicit invalid override is an error.

Spec: N12 DR 062 §1

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Invoked → InvalidOverride, "an explicit invalid override is an error". A system transition: no skill carries it.

## Ready

`claim:ChartTool.Ready`: In a dydo statechart run, its dependencies verified compatible, it runs offline with no repeated installation or consent.

Spec: N12 DR 062 §1

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Invoked → Ready, "its dependencies verified compatible, it runs offline with no repeated installation or consent". A system transition: no skill carries it.

## Missing

`claim:ChartTool.Missing`: In a dydo statechart run, a dependency is missing: a precise locked plan, a structured event, the saved request and exit 3, proposing dydo statechart resume --request.

Spec: N12 DR 062 §2

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Invoked → ApprovalRequired, "a dependency is missing: a precise locked plan, a structured event, the saved request and exit 3, proposing dydo statechart resume --request". A system transition: no skill carries it.

## Authorize

`claim:ChartTool.Authorize`: In a dydo statechart run, the installed skill submits that exact continuation through the host's normal authorization, and the host grants it.

Spec: N12 DR 062 §3

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): ApprovalRequired → Continuing, "the installed skill submits that exact continuation through the host's normal authorization, and the host grants it". A system transition: no skill carries it.

## Deny

`claim:ChartTool.Deny`: In a dydo statechart run, denied, authorization unavailable or canceled: non-success, the request kept; a local flag, token or receipt is not host approval.

Spec: N12 DR 062 §3, N12 DR 062 §5

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): ApprovalRequired → NonSuccess, "denied, authorization unavailable or canceled: non-success, the request kept; a local flag, token or receipt is not host approval". A system transition: no skill carries it.

## Installed

`claim:ChartTool.Installed`: In a dydo statechart run, identities verified, the locked assets installed into owned roots, readiness proved and the receipt published atomically; the saved inputs continue.

Spec: N12 DR 062 §4

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Continuing → Ready, "identities verified, the locked assets installed into owned roots, readiness proved and the receipt published atomically; the saved inputs continue". A system transition: no skill carries it.

## InstallFails

`claim:ChartTool.InstallFails`: In a dydo statechart run, the installation fails: non-success, the request kept, and no checker starts on an unverified plan.

Spec: N12 DR 062 §5

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Continuing → NonSuccess, "the installation fails: non-success, the request kept, and no checker starts on an unverified plan". A system transition: no skill carries it.

## Check

`claim:ChartTool.Check`: In a dydo statechart run, the check starts under native statechart ownership, with registered scratch and startup gates.

Spec: N12 DR 062 §6

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Ready → Checking, "the check starts under native statechart ownership, with registered scratch and startup gates". A system transition: no skill carries it.

## Drained

`claim:ChartTool.Drained`: In a dydo statechart run, an authoritative drain: registered-owned scratch is cleaned.

Spec: N12 DR 062 §6

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Checking → Charted, "an authoritative drain: registered-owned scratch is cleaned". A system transition: no skill carries it.

## Uncertain

`claim:ChartTool.Uncertain`: In a dydo statechart run, the drain is uncertain, or only notified: scratch kept with a durable reason.

Spec: N12 DR 062 §6

Transition in A dydo statechart run (`claims/statechart/statechart-run/StatechartRun.tla`): Checking → ScratchKept, "the drain is uncertain, or only notified: scratch kept with a durable reason". A system transition: no skill carries it.

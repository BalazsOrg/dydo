---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Host

## Init

`claim:Host.Init`: In a project's host settings, dydo init or --join turns native memory off for both hosts, commits one Claude guard entry and keeps the allow list local.

Spec: N12 DR 055 §1, N12 DR 056 §1–2

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): Uninitialised → Initialised, "dydo init or --join turns native memory off for both hosts, commits one Claude guard entry and keeps the allow list local". A system transition: no skill carries it.

## Call

`claim:Host.Call`: In a project's host settings, an agent's tool call runs through dydo guard, one policy for Claude and Codex.

Spec: N12 DR 056 §1, N12 DR 056 §3

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): Initialised → Guarded, "an agent's tool call runs through dydo guard, one policy for Claude and Codex". A system transition: no skill carries it.

## Deny

`claim:Host.Deny`: In a project's host settings, a write to a host memory folder is blocked.

Spec: N12 DR 055 §2

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): Guarded → MemoryDenied, "a write to a host memory folder is blocked". A system transition: no skill carries it.

## Pass

`claim:Host.Pass`: In a project's host settings, not a memory write: the guard's other rules decide the call.

Spec: N12 DR 056 §1

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): Guarded → Initialised, "not a memory write: the guard's other rules decide the call". A system transition: no skill carries it.

## Refused

`claim:Host.Refused`: In a project's host settings, the write is refused and the session goes on: the guard stops accidents, not rogue agents.

Spec: N12 DR 055 §2–3

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): MemoryDenied → Initialised, "the write is refused and the session goes on: the guard stops accidents, not rogue agents". A system transition: no skill carries it.

## Background

`claim:Host.Background`: In a project's host settings, Codex's memory runs from a background process the guard never sees.

Spec: N12 DR 055 §3

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): Initialised → CodexBackground, "Codex's memory runs from a background process the guard never sees". A system transition: no skill carries it.

## FlagOff

`claim:Host.FlagOff`: In a project's host settings, with memories = false it writes nothing: the Codex half rests on that flag.

Spec: N12 DR 055 §1, N12 DR 055 §3

Transition in Host settings and the guard (`claims/statechart/host-setup/HostSetup.tla`): CodexBackground → Initialised, "with memories = false it writes nothing: the Codex half rests on that flag". A system transition: no skill carries it.

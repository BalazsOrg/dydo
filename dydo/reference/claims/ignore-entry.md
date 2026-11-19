---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: IgnoreEntry

## Split

`claim:IgnoreEntry.Split`: A file over the token limit is split.

Spec: T15, T16

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): OverLimit → Split, "the writer splits the file".

**code-writer** (OverLimit, exit):

> Split a file you change that passes the token limit.

## Generated

`claim:IgnoreEntry.Generated`: Path (a): a PR adds an exact path or generated-directory glob, with a reason, for a file nobody writes by hand.

Spec: T17, T14

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): OverLimit → PathA, "nobody writes it by hand".

**code-writer** (OverLimit, exit):

> For a file nobody writes by hand, add its exact path or generated-directory glob to the ignore file in your PR, with a reason.

## CheckA

`claim:IgnoreEntry.CheckA`: The reviewer checks each path (a) entry.

Spec: T17

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): PathA → Ignored, "the reviewer checks the entry".

**reviewer** (PathA, exit):

> Check each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.

## Authored

`claim:IgnoreEntry.Authored`: Path (b): authored code goes through its own ticket, a council and his blessing; a PR can't add its own entry.

Spec: T18, T19

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): OverLimit → OwnTicket, "authored code that must stay whole".

**ticket-captain** (OverLimit, exit):

> An authored file that must pass the limit takes path (b): its own ticket, a council, then his blessing; its PR never adds its own entry.

## Convene

`claim:IgnoreEntry.Convene`: The path (b) ticket goes to a council.

Spec: T18

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): OwnTicket → CouncilB, "the ticket goes to a council". A system transition: no skill carries it.

## Bless

`claim:IgnoreEntry.Bless`: He blesses a path (b) entry.

Spec: T18

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): CouncilB → Ignored, "he blesses it". A system transition: no skill carries it.

## GoStale

`claim:IgnoreEntry.GoStale`: A stale ignore entry fails the gate.

Spec: T17

Transition in A file over the token limit (`claims/statechart/ignore-entry/IgnoreEntry.tla`): Ignored → Stale, "the entry goes stale". A system transition: no skill carries it.

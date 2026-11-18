---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Should

## Register

`claim:Should.Register`: A should is registered as a child of the ticket where found, after a duplicate check.

Spec: S17

Transition in A should (`claims/statechart/should/Should.tla`): Found → Open, "no duplicate open: registered under the ticket where found".

**reviewer** (Found, exit):

> Holding the round alone, check the open shoulds under the ticket and its ancestors, then register each new should as a child of the ticket where you found it.

**ticket-captain** (Found, exit):

> Holding the round, check the open shoulds under the ticket and its ancestors, then register each new should as a child of the ticket where it was found.

**linear-workspace-standard** (LinearShoulds):

> A should is registered as a child of the ticket where it was found, after the round holder checks the open shoulds under that ticket and its ancestors for a duplicate.

## AlreadyOpen

`claim:Should.AlreadyOpen`: A should already open under the ticket or an ancestor is not registered again.

Spec: S17

Transition in A should (`claims/statechart/should/Should.tla`): Found → Known, "a duplicate is already open". A system transition: no skill carries it.

## Fixed

`claim:Should.Fixed`: A should is fixed in place when cheap, else at a set moment.

Spec: S17

Transition in A should (`claims/statechart/should/Should.tla`): Open → Done, "fixed in place, when a lane is free, before a round, or at the landing".

**ticket-captain** (Open, exit):

> Fix a cheap should in place before its parent merges; the rest wait for a shoulds batch.

## Dup

`claim:Should.Dup`: The batch captain marks duplicates Duplicate.

Spec: S17

Transition in A should (`claims/statechart/should/Should.tla`): Open → Duplicate, "the batch captain finds it a duplicate".

**ticket-captain** (Open, exit):

> In a shoulds batch, mark each duplicate Duplicate.

**linear-workspace-standard** (LinearShoulds):

> The batch captain marks a duplicate should Duplicate.

## Cancel

`claim:Should.Cancel`: At the landing a should is canceled only with a reason.

Spec: S18

Transition in A should (`claims/statechart/should/Should.tla`): Open → Canceled, "at the landing: canceled with a reason".

**admiral** (Open, exit):

> At the landing, cancel a should only with a reason.

## Move

`claim:Should.Move`: At the landing a should moves out only by his call.

Spec: S18

Transition in A should (`claims/statechart/should/Should.tla`): Open → Moved, "at the landing: moved out by his call".

**admiral** (Open, exit):

> At the landing, move a should out of the Project only on his call.

## Batch

`claim:Should.Batch`: The admiral cuts a Shoulds batch N root ticket for one captain at the set moments.

Spec: S17

**admiral** (Open, nominal):

> Collect shoulds into a `Shoulds batch N` root ticket (Type `Feature` by default) cut for one captain when a lane is free, before an inquisition round, or at the landing.

**linear-workspace-standard** (LinearShoulds):

> Shoulds are collected into a `Shoulds batch N` root ticket the admiral cuts for one captain (Type `Feature` by default); there is no standing collection ticket.

## Reparent

`claim:Should.Reparent`: The batch captain re-parents the shoulds, keeping a link to where each was found.

Spec: S17

**ticket-captain** (Open, nominal):

> Holding a Shoulds batch, re-parent each should into it, keeping a `related: found in <KEY> rN` link on it.

**linear-workspace-standard** (LinearShoulds):

> The batch captain re-parents the shoulds into it and keeps a `related: found in <KEY> rN` link on each.

## ParentDone

`claim:Should.ParentDone`: A should's parent may close Done while it stays open.

Spec: S17

**ticket-captain** (Open, nominal):

> A parent may close Done while its shoulds stay open.

**linear-workspace-standard** (LinearShoulds):

> A should's parent may close Done while the should stays open.

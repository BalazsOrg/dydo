---
area: understand
type: concept
---

# Control Flow: States

[Control Flow](./control-flow.md)

## 4. States

A delivery ticket's status is the only delivery status, and Linear owns it: ten statuses in the
standard's order, which Linear draws as progress. The captain alone sets it, flipping at every chain
spawn. An open native blocker makes a ticket blocked in any status; there is no `Blocked` status
and no `Waiting for Human`; `Planning` remains a Project status only.

```mermaid
stateDiagram-v2
  state "FutureFeature" as Future
  state "Todo" as Todo
  state "In Progress" as InProgress
  state "Implementing" as Implementing
  state "In Review" as InReview
  state "Ready to Merge" as Ready
  state "Done" as Done
  [*] --> Future: no Type yet
  Future --> Backlog: promoted with a Type
  [*] --> Backlog: retained with a Type
  Backlog --> Todo: the user schedules it, one Type, one Mode
  Backlog --> Canceled: declined
  [*] --> Todo: a map holder, co-thinker or to-ticket creates it contracted
  Todo --> Implementing: captain spawns the author
  Implementing --> InProgress: the writer named lanes
  InProgress --> InReview: lanes merged, review of the whole
  Implementing --> InReview: implement hop posted
  Todo --> InReview: a spec review of the contract, before any code
  InReview --> Implementing: FAIL, a blocker; or a spec review's verdict
  InReview --> Ready: PR ready with its PASS
  Ready --> Done: its Merge Sub-ticket PASS; for an atomic ticket, the user's click
  Ready --> Implementing: merge review FAIL, reverted
  InReview --> Done: a Merge Sub-ticket's own review PASS
  Implementing --> Todo: released
  InReview --> Todo: released
  Ready --> Todo: released
  InProgress --> Todo: released
  Future --> Todo: promoted and contracted
  InReview --> Canceled: a Merge Sub-ticket reverted
  Todo --> Canceled
  Implementing --> Canceled
  Todo --> Duplicate
```

Who sets what: the captain sets every status of its ticket and Sub-tickets; the Project's map holder
sets Project statuses and its own map-holder-held tickets'. A `Question` runs `Todo` → `Done` and `Todo` on it is
the user's turn; `Research`, `Grilling` and `Walkthrough` run `Todo` → `In Progress` → `Done`. A
captain-held ticket normally runs `Todo` → `Implementing` → `In Review` → `Ready to Merge` → `Done`,
with `In Progress` while its lanes run; a spec review the captain buys shows `In Review` before
`Implementing`. A Merge Sub-ticket runs `Todo` → `Implementing` → `In Review` →
`Done` and never waits to be merged: one code-writer maps the conflicts and merges, a resolution
that refactored leaving its code no worse than either side. An atomic ticket has no Merge Sub-ticket:
it waits in `Ready to Merge` for the user's click. An Inquisition
Ticket runs `Backlog` → `Todo`, the user's confirmation → `In Progress` for its sweep, then its
own record's docs chain, `Implementing` → `In Review` → `Ready to Merge` → `Done`. It releases to
`Todo` only for a missing verification skill or for its group Sub-tickets, by the
[workspace standard](../reference/linear-workspace-standard.md)'s status path. A Merge Sub-ticket
whose review fails on the landed work is reverted and closes `Canceled` with the reason.

## Related

- [Control Flow](./control-flow.md)

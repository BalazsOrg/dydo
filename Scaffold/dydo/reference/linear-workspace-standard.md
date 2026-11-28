---
area: reference
type: reference
---

# Linear Workspace Standard

- The Linear vocabulary for Projects and tickets; every rule here is a claim.

## Setting up

- Seven statuses only: FutureFeature, Backlog, Todo, In Progress, Done, Canceled, Duplicate. The Enablement at ship deletes `Implementing`, `In Review` and `Ready to Merge`, and Sub-issue auto-close stays off.
- Each project (team) gets its continuous pair, `CI findings` and `Papercuts`: a Linear grouping of tickets, not a Project with an outcome; nobody oversees it or sends envoys for it.

## Statuses

- Whoever starts a step sets it In Progress and attaches the holder's T3 thread link, titled `T3 · <role> · <model>`; neither the assignee nor Linear's `delegate` carries identity.
- Whoever started a step closes it; a parent's In Progress means its officer holds it.
- `liveness` fires on a hop In Progress and quiet for 2 hours, or a ticket In Progress with no open hop for 2 hours, counted from the later of its last change and its latest `liveness <UTC time>: <outcome>` comment.
- A reverted Merge goes Canceled with `reverted: <gate> red on <merge SHA>`.
- On a re-plan, dead hops go Canceled with a one-line reason.

## Ticket labels

- Each hop's kind is its Type label: Implement, Review, Fix, Blocker and Wayfinding sit in the Type group next to `Merge`; setting them up is his Enablement.
- The Mode label, AFK or HITL, says who the work waits on.
- Severity is `blocker`, `should` or `nit`: blocker, the intended case is wrong; should, a real bug callers will hit inside the intent; nit, everything else. FAIL means at least one blocker.

## Tickets

- A ticket exists only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.
- One agent holds one ticket, and several steps given to one agent are wrapped in one ticket; sub-tickets may nest.
- A leaf lane or a batched should is one ticket, with no hops.
- Hops appear only under a step with its own review loop: Implement, Review (one per round), Fix and Merge, blocked in step order; each goes Todo → In Progress → Done and may be opened ahead in Todo.
- A leaf's evidence check is a Review hop beside it.
- At verdict time, blockers become a Fix rN ticket with one Blocker ticket per blocker inside it; Fix rN is Done when its blockers are Done and proof ran on the head; for parallel writers it splits into Fix rNb.
- Merge sub-tickets: an AFK Merge for a feature-branch merge or a two-way main merge, a HITL Merge only for a one-way one.
- A need for Balazs becomes a Question, Grilling, Walkthrough, Wayfinding or Enablement ticket; a Question blocks only what waits on it.

## Communication and evidence

- The verdict lives on the Review sub-ticket: PASS or FAIL, lenses, seats and shoulds. The PR body's `## Independent review` carries only a one-line PASS naming its Review ticket, written with the verdict; the parent gets no copy, and `dydo map` and `dydo survey board` show the latest verdict on its card.
- The round-3 checkpoint is one line on the Review: `checkpoint: one more round, because …` or `escalated: …`.
- A nit is one line on its record; an inquisition round's nits go on that round's nit ticket.

## Shoulds

- A should is registered as a child of the ticket where it was found, after the round holder checks the open shoulds under that ticket and its ancestors for a duplicate.
- A should's parent may close Done while the should stays open.
- Shoulds are collected into a `Shoulds batch N` root ticket the coordinator cuts for one captain (Type `Feature` by default); there is no standing collection ticket.
- The batch captain re-parents the shoulds into it and keeps a `related: found in <KEY> rN` link on each.
- The batch captain marks a duplicate should Duplicate.

## Spec and contract

- The spec is a main Linear document on the Project, `Spec: <Project>`, with C8's headings: problem, behaviour, decisions, out of scope, acceptance checks. A sub-feature that needs defining well gets its own spec, linked from it. The map holder writes and refines it; at acceptance it is deleted after its lasting parts move to DRs, the glossary, docs and the verification map.
- A contract is one ticket's expected outcome, in its description. It quotes and links the spec sections it derives from, and that frozen quote replaces pinning a commit.

## Priority

- Priority marks which of his tickets he takes next, so the one unlocking the most AFK work goes first: High when clearing it lets a top-level ticket or the Project's next step run AFK, Medium when it frees a lane, Low when nothing runs AFK after it; Urgent is an emergency, never a default; AFK tickets carry none.

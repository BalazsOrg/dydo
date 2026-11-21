---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainFinish

## Who

`claim:CaptainFinish.Who`: A captain's brief says who merges its work.

Spec: K14, R21

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "Do not push; no PR; no merge."

Layer: A checkout does not say whose the merge is; a child sharing its officer's worktree has nothing to merge at all.

Reason: The three ways of finishing differ, and a captain that picks the wrong one merges twice or not at all.

**ticket-captain** (Who; read by TicketCaptain):

> Your brief says who merges your work.

## HandOver

`claim:CaptainFinish.HandOver`: Work the officer merges ends on a commit and returns its SHA with the receipts that prove it.

Spec: R32, R5

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Leaf → Finishing, "the leaf is whole and has no review of its own: its officer reviews it, or it is too small to need one, or it is a verdict or an answer".

Evidence (run): This skill's writer was briefed (DYD-1372 hop 2): "IMPLEMENTED DYD-1372 hop 2 at <SHA>", then "the gate commands with their exits".

Layer: T3 returns the child's words; without a SHA they point at nothing the officer can check out.

Reason: The officer merges and reviews exactly what was proved, and nothing that came after.

**ticket-captain** (Cases; read by TicketCaptain):

> **Your officer:** end on a commit, and return its SHA with the receipts that prove it.

## Merged

`claim:CaptainFinish.Merged`: A captain that merges its work returns one line, done and merged.

Spec: R21, H23

Evidence (command): `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run`

Layer: `dydo merge` merges and reports to whoever ran it; the officer learns of it only from the return.

Reason: One line is all the officer needs, because everything else is already on the ticket.

**ticket-captain** (Cases; read by TicketCaptain):

> **You:** merge by Merging (`resources/merging.md`), and return `done <KEY>: merged`.

## Nothing

`claim:CaptainFinish.Nothing`: A ticket that merges nothing puts its result on the ticket and returns one line.

Spec: S6, H23

Evidence (run): DYD-1295's comments, read from their snapshot 2026-11-20: a walkthrough's rulings sit on its ticket, and no branch holds them.

Layer: A verdict or an answer has no commit to carry it; the ticket is the only place that outlives the thread.

Reason: A result left in a thread is gone when the thread is.

**ticket-captain** (Cases; read by TicketCaptain):

> **Nobody:** put the result on the ticket, and return `done <KEY>`.

## Return

`claim:CaptainFinish.Return`: A captain returns to its officer once its ticket is Done.

Spec: H23, K14

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Done → Returned, "one line goes back to the officer".

Evidence (tool): delegate_task: a child's final result returns to the thread that started it, which reads it with `task_status`.

Layer: T3 delivers whatever the child's last message is, finished or not.

Reason: An officer woken by a return can count on the ticket being closed.

**ticket-captain** (Close; read by TicketCaptain, on claim:CaptainFinish.Close):

> Close the ticket Done once its contract is met, then return; a return before that leaves it open, as it stands.

## Close

`claim:CaptainFinish.Close`: The captain that started its ticket closes it Done once its contract is met, and a return before that leaves the ticket open.

Spec: S6

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Finishing → Done, "the contract is met: the ticket closes Done".

Evidence (run): Linear, team Dydo, read 2026-11-21: Done exists, and DYD-1281's nineteen finished sub-tickets are Done, while DYD-1295, the walkthrough whose envoy returned with no verdict from the user, is still open, in Todo.

Layer: `dydo merge` merges a PR and touches no ticket.

Reason: An open ticket whose work is finished looks stalled to everyone who reads the board.

**ticket-captain** (Close; read by TicketCaptain):

> Close the ticket Done once its contract is met, then return; a return before that leaves it open, as it stands.

## Limits

`claim:CaptainFinish.Limits`: On a rate-limit or capacity error an agent waits, then retries, and never takes a limit to the user.

Spec: H21

Evidence (run): This skill's writer ran behind CLIProxy: its session's files sit under `~/.claude-cliproxy`.

Layer: CLIProxy carries every agent's inference and its limits; all that is left to the agent is how it behaves when an error still arrives.

Reason: A swarm that retries at once turns one limit into a storm.

**ticket-captain** (WhenThingsGoWrong; read by TicketCaptain):

> On a rate-limit or capacity error, wait, then retry; don't hammer it, and never take a limit to the user.

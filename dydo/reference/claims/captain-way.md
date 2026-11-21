---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: CaptainWay

## Route

`claim:CaptainWay.Route`: A captain sees the route from its ticket to the spec when it can write every part's contract now, and it checks that before it cuts.

Spec: H10, S15

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: it sets the same test, "whether you can state the question precisely now, _not_ whether you can answer it now", and has its map holder "Create the tickets you can specify now".

Layer: AGENTS.md says a bigger ticket splits; nothing below this skill says when its holder may not cut yet.

Reason: A holder that cuts what nobody can state yet hands its children guesses, and each guess is paid for in rework.

**ticket-captain** (Fog; read by TicketCaptain):

> Before you cut a bigger ticket, check that you see its route to the spec: that you can write every part's contract now.

## Fog

`claim:CaptainWay.Fog`: A captain that cannot see the route loads the wayfinder skill and finds the way a part at a time; it never cuts the whole at once.

Spec: H10, H23

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Taken → Wayfinding, "the route to the spec is not all in sight: the way is found before the cut".

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: its description is "Fog in a Project map or inside one ticket", and it lets a captain act on it: "A Ticket Captain may chart local fog inside one approved delivery outcome." The build fails when `/wayfinder` names no skill under `skills/`.

Layer: The host lists the wayfinder among its skills and loads it only when asked, and until this line a captain was asked only when its brief named it.

Reason: A feature cut whole in the fog fixes every later part before the first has shown what the next should be.

**ticket-captain** (Fog; read by TicketCaptain):

> If you cannot, load `/wayfinder` and find the way a part at a time, by Finding the way (`resources/wayfind.md`): never cut the whole at once.

## Visible

`claim:CaptainWay.Visible`: While it finds the way, a captain cuts only the parts whose contract it can write now, each sized to one agent, and leaves the fog uncut.

Spec: H10, S2

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Wayfinding → Split, "the parts in sight are cut, and the fog is left uncut".

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "Create the tickets you can specify now", "everything you cannot yet specify stays in the fog", "Don't pre-slice the fog into ticket-sized pieces", a ticket's body "carries one outcome sized to one agent session", and "Wayfinding Tickets clear the fog blocking what comes next".

Layer: Linear takes a sub-ticket for a part nobody can state yet as readily as for any other.

Reason: Small parts keep the work moving while the rest is still dark (07c86aad: "It keeps the tasks bite sized so it's easy to move forward").

**ticket-captain** (WaySteps; read by TicketCaptain):

> Cut only the parts whose contract you can write now, each small enough for one agent, by Sub-tickets (`sub-tickets.md`), and leave the fog uncut. A question whose answer would clear fog is such a part.

## Next

`claim:CaptainWay.Next`: Each part that returns clears fog: the captain cuts the parts whose contract the result lets it write, until no fog is left.

Spec: H10, S15

Transition in A captain and its ticket (`claims/statechart/captain/Captain.tla`): Waiting → Wayfinding, "a part returns and fog remains: the way is found again from what it showed".

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "Resolving a ticket clears the fog ahead of it, graduating whatever's now specifiable into fresh tickets, one at a time, until the way to the destination is clear"; and "Defer later decisions until delivery reveals the facts they need."

Layer: T3 wakes a captain when a child returns, and nothing looks again at what that result has made plain.

Reason: A finished part is the cheapest evidence of what the next one should be (07c86aad: "the wisdom of removing the fog gradually").

**ticket-captain** (WaySteps; read by TicketCaptain):

> When a part returns, cut the parts whose contract its result lets you write, and repeat until no fog is left.

## Ahead

`claim:CaptainWay.Ahead`: What a captain can see ahead needs the user is gathered into one Grilling sub-ticket, Mode HITL, so one sitting settles the large sections before their parts are cut.

Spec: H10, H15

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Type group holds Grilling, "Intent or specification choices resolved with the human and recorded.", and the Mode group HITL. `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "a session the user drives never resolves more than one Wayfinding Ticket", so one ticket is one sitting. The user, 07c86aad: "sometimes I am needed for that in a HITL grilling session. Most of the time that's seen ahead of time. So large sections can be preemptively settled".

Layer: Linear takes a HITL ticket for every single call, and nothing gathers the ones a captain could have seen together.

Reason: His attention comes in sittings, and a point that could have joined an earlier sitting costs him a second start.

**ticket-captain** (WayUser; read by TicketCaptain):

> Settle ahead what you can see ahead: gather every call you already know is the user's into one Grilling sub-ticket, Mode HITL, so that one sitting settles the large sections before their parts are cut.

## Timed

`claim:CaptainWay.Timed`: A later point that needs the user, a call or a prototype to look at, is a HITL sub-ticket of its own that blocks only the parts its answer could change.

Spec: H10, S10

Evidence (run): Linear, team Dydo, labels read 2026-11-21: the Type group holds Prototype, "A design question tested through a sketch; closes on the human's verdict." Linear, DYD-1372 read 2026-11-20: a ticket carries `blocks` and `blockedBy` relations. The user, 07c86aad: "there may be still some calls made, or prototypes to be looked at which saves a lot of work down the line. So these human points in the workflow have to be timed very cleverly."

Layer: Linear draws a block wherever it is set; where a look saves the most work is known only to the captain that sees the route.

Reason: A look that comes after the work is built can only order it rebuilt.

**ticket-captain** (WayUser; read by TicketCaptain):

> Give each later point that needs the user, a call to make or a prototype to look at, a HITL sub-ticket of its own, blocking only the parts its answer could change: the user then sees it before that work is built.

## Reach

`claim:CaptainWay.Reach`: A HITL sub-ticket is a need for the user and goes up by that procedure.

Spec: H16, H15

Evidence (run): `skills/productivity/wayfinder/SKILL.md`, read 2026-11-21: "A HITL ticket only resolves through that live exchange; the agent never stands in for the user's side of it". `skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked, takes each one on its queue: "A need for him arrives on your HITL queue as its own HITL ticket", a Grilling among them.

Layer: `delegate_task` starts an agent on any ticket, a HITL one included, and an agent alone cannot give the user's side of it.

Reason: A captain that staffs its own HITL ticket gets an agent talking to itself where the user was meant to speak.

**ticket-captain** (WayUser; read by TicketCaptain):

> Each HITL sub-ticket is a need for the user: it goes up by A need for the user (`need-for-the-user.md`).

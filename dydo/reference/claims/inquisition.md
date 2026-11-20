---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Inquisition

## Conduct

`claim:Inquisition.Conduct`: A ticket captain conducts an inquisition with the inquisition-captain resource (DYD-1082 1bc04dce change 5).

Spec: N5, K22

**ticket-captain** (Take):

> Holding an Inquisition ticket or one of its sub-tickets, conduct it by the inquisition captain (`resources/inquisition-captain.md`).

## Level

`claim:Inquisition.Level`: The inquisition is review at the landing level, cut by feature and staffed with reviewers (DYD-1082 01266a65).

Spec: N5, R34

**ticket-captain** (Inquisition):

> An inquisition is review at the landing level: the candidate as a whole, cut by feature and staffed with reviewers who inspect everything and test every hypothesis.

## Zero

`claim:Inquisition.Zero`: Every inquisition round starts from zero.

Spec: N5

**ticket-captain** (Inquisition):

> Every round starts from zero: a fresh feature map, never an earlier round's map or findings.

## Map

`claim:Inquisition.Map`: The root inquisition captain draws the feature map: small slices, each seam a slice.

Spec: N5

**ticket-captain** (Inquisition):

> As the root, draw the feature map from the landed diff: one small slice per feature, and each seam where features meet a slice of its own; post it on the ticket.

## Split

`claim:Inquisition.Split`: Over about 10 slices the root cuts group sub-tickets, each held by a sub-captain, and owns the cross-group seams (DYD-1082 99774812).

Spec: N5, H13

**ticket-captain** (Inquisition):

> Over about 10 slices, cut the map into group sub-tickets of up to about 10 slices, each held by a sub-captain; keep a seam inside a group with it, and keep the seams between groups as slices of your own.

## Unsplit

`claim:Inquisition.Unsplit`: When the map is not split, and for the root's own seam slices, the root opens a sub-ticket with its own captain.

Spec: N5

**ticket-captain** (Inquisition):

> When the map is not split, and for your own seam slices, open a sub-ticket with its own captain for those slices.

## Root

`claim:Inquisition.Root`: The root never drives reviewers directly and stitches the groups' results (DYD-1082 99774812).

Spec: N5

**ticket-captain** (Inquisition):

> As the root, never dispatch a reviewer yourself: stitch the groups' results into the round's verdict.

## Lenses

`claim:Inquisition.Lenses`: Read, Break and Drive run as separate sub-agents, in that order, never one agent for two (DYD-1082 8239d5b6).

Spec: N5, R34

**ticket-captain** (Inquisition):

> Holding a group, run Read, then Break, then Drive on each slice, each lens its own reviewer briefed from the slice reviewer brief below; never one agent for two lenses.

## Route

`claim:Inquisition.Route`: Each return is routed: a new finding a row, a known one a sighting, one only a test or a drive settles to that lens.

Spec: N5

**ticket-captain** (Inquisition):

> Route each return: a new finding is a new row in the round's findings, a known one a sighting on its row, and one only a test or a drive can settle goes to that slice's Break or Drive seat.

## SliceBrief

`claim:Inquisition.SliceBrief`: The inquisition-captain resource carries the slice reviewer brief (DYD-1082 1bc04dce change 5).

Spec: N5, R34, K14

**ticket-captain** (SliceBrief):

> Skills: /reviewer · level: inquisition · the lens · the slice, its map row and its findings so far · the audit SHA and the landing base · the governing spec · worktree · model and effort · return to.

## Order

`claim:Inquisition.Order`: An inquisition seat runs the one lens its brief names on its slice, from zero.

Spec: N5, R34

**reviewer** (ReadPhase, nominal):

> In an inquisition, run only the lens your brief names on your slice, from zero; a separate seat runs each other lens.

## Read

`claim:Inquisition.Read`: The Read lens covers the three Read columns: local, against the spec, and the whole.

Spec: N5, R34

**reviewer** (ReadPhase, nominal):

> Read: read the slice locally, against the spec and as a whole, for what is broken in ways neither tests nor clicking would catch.

## Break

`claim:Inquisition.Break`: The Break lens runs the tests, the hypothesis tests and the formal checks.

Spec: N5, R34, R7

**reviewer** (BreakPhase, nominal):

> Break: run the suite, `--heavy` and mutation, test a hypothesis for every way the slice could fail, and run its formal checks where they are built.

## Drive

`claim:Inquisition.Drive`: The Drive lens drives the app against the spec's problem statement, where driving is built.

Spec: N5, R34, R3

**reviewer** (DrivePhase, nominal):

> Drive: drive the slice's journeys through the running app, against the spec's problem statement, where a driving skill is built for it.

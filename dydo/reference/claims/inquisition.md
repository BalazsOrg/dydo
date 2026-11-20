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

Spec: N5, N12 DR 059

**ticket-captain** (Inquisition):

> Every round starts from zero: a fresh feature map, never an earlier round's map or findings.

## NeverHunt

`claim:Inquisition.NeverHunt`: The inquisition's captain conducts and never hunts (DR 059 ruling 6).

Spec: N12 DR 059, N5

**ticket-captain** (Inquisition):

> You conduct and never hunt: reading the work for what is wrong is your reviewers' job.

## VerificationFirst

`claim:Inquisition.VerificationFirst`: A verification skill comes first: a project with a runnable surface and none gets it before the round (DR 059 ruling 4).

Spec: N12 DR 059, N5, R9, R34

**ticket-captain** (Inquisition):

> As the root, check before the first round that the project has a verification skill: one with a runnable surface and none goes to your officer as a new requirement, and the round waits for the skill; a project with nothing to run needs none, and Drive does not apply.

## Map

`claim:Inquisition.Map`: The root inquisition captain draws the feature map: small slices, each seam a slice, each lens run or its reason (DR 059 ruling 1).

Spec: N5, N12 DR 059

**ticket-captain** (InquisitionRound):

> As the root, draw the feature map from the landed diff: one small slice per feature, and each seam where features meet a slice of its own; post it on the ticket with each slice's lenses, or the one-line reason a lens does not apply.

## Prose

`claim:Inquisition.Prose`: A prose slice gets Read only; Break and Drive apply wherever the landing changed behaviour (DR 059 ruling 2).

Spec: N12 DR 059, N5

**ticket-captain** (InquisitionRound):

> A prose slice, a plain skill or doc, gets Read only, and a contradiction it finds is proved by quoting both passages; Break and Drive apply wherever the landing changed behaviour.

## Split

`claim:Inquisition.Split`: Over about 10 slices the root cuts group sub-tickets, each held by a sub-captain, and owns the cross-group seams (DYD-1082 99774812).

Spec: N5, H13

**ticket-captain** (InquisitionRound):

> Over about 10 slices, cut the map into group sub-tickets of up to about 10 slices, each held by a sub-captain; keep a seam inside a group with it, and keep the seams between groups as slices of your own.

## Unsplit

`claim:Inquisition.Unsplit`: When the map is not split, and for the root's own seam slices, the root opens a sub-ticket with its own captain.

Spec: N5

**ticket-captain** (InquisitionRound):

> When the map is not split, and for your own seam slices, open a sub-ticket with its own captain for those slices.

## Root

`claim:Inquisition.Root`: The root never drives slice reviewers directly and stitches the groups' results, joining duplicates across groups (DYD-1082 99774812, DR 059 ruling 6).

Spec: N5, N12 DR 059

**ticket-captain** (InquisitionRound):

> As the root, never dispatch a slice's reviewer yourself: merge the groups' registers into yours, a finding seen in two groups becoming one finding with both sightings, and stitch the round's verdict from them.

## Register

`claim:Inquisition.Register`: The findings register is a file on the audit branch, pushed after every return (DR 059 ruling 6).

Spec: N12 DR 059, N5

**ticket-captain** (InquisitionRound):

> Holding slices, keep the round's findings register on an `inquisition/<slug>` branch cut from the audit SHA, which never merges: one row per finding with its slice, severity, place, proof, sightings and disposition, pushed after every return and before the next dispatch.

## Lenses

`claim:Inquisition.Lenses`: Read, Break and Drive run as separate sub-agents, in that order, as a pipeline, never one agent for two (DYD-1082 8239d5b6, DR 059 ruling 3).

Spec: N5, R34, N12 DR 059

**ticket-captain** (InquisitionRound):

> Run Read, then Break, then Drive on each slice, as a pipeline across slices rather than in waves, each lens its own reviewer briefed from the slice reviewer brief below; never one agent for two lenses.

## OneSeat

`claim:Inquisition.OneSeat`: One agent per slice per lens; tiny slices are merged on the map, never packed into one dispatch (DR 059 ruling 5).

Spec: N12 DR 059, N5

**ticket-captain** (InquisitionRound):

> Send one reviewer per slice per lens: a dispatch never carries two slices, so merge tiny slices on the map instead.

## Route

`claim:Inquisition.Route`: A finding only a test or a drive settles goes to that lens, with a follow-up seat if the lens already ran (DR 059 ruling 6).

Spec: N5, N12 DR 059

**ticket-captain** (InquisitionRound):

> Route a finding only a test or a drive can settle to that slice's Break or Drive seat, with a short follow-up seat when that lens already ran.

## Sightings

`claim:Inquisition.Sightings`: One bug seen in several places is one finding with several sightings (DR 059 ruling 6).

Spec: N12 DR 059, N5

**ticket-captain** (InquisitionRound):

> A finding already in the register is a sighting on its row, never a new row: a bug read in the code and seen again while driving is one finding with two sightings.

## Severity

`claim:Inquisition.Severity`: Each return's severities are checked against the source, not the reviewer's label (DR 059 ruling 6).

Spec: N12 DR 059, N12 DR 058

**ticket-captain** (InquisitionRound):

> Check each returned severity against the source, not the reviewer's label, and record any change with its reason on the row.

## Judgement

`claim:Inquisition.Judgement`: A finding that is a judgement call goes up as a need (DR 059 ruling 6, as H16 routes a need).

Spec: N12 DR 059, H16

**ticket-captain** (InquisitionRound):

> Pass a finding that needs a judgement call up as a need, like any other.

## Patch

`claim:Inquisition.Patch`: A proven blocker or should with a well-defined fix gets its fix prepared mid-round on its own branch, for its ticket's captain to verify and review once (DR 059 ruling 7).

Spec: N12 DR 059, K31

**ticket-captain** (InquisitionRound):

> A blocker or should a Break seat proved red, with a well-defined fix, gets the fix prepared mid-round: brief an implementer for a Bug on its own branch off the audit SHA, red test first, and at the verdict attach the branch and its SHA to the finding's ticket, whose captain verifies and reviews it once.

## PatchLimits

`claim:Inquisition.PatchLimits`: A well-defined fix is one obvious, small fix in one slice touching no contract, DR or public interface; nits are never patched (DR 059 ruling 7).

Spec: N12 DR 059

**ticket-captain** (InquisitionRound):

> Well-defined means one obvious fix inside one slice, small, touching no contract, DR or public interface; any doubt means no patch, nits are never patched, and the inquisition merges nothing but its own record.

## Record

`claim:Inquisition.Record`: The round's record is its last step: an implementer writes it, a fresh reviewer checks it, and it merges into the feature branch (DR 059 ruling 8, N12).

Spec: N12 DR 059, K31, N5

**ticket-captain** (InquisitionRecord):

> As the root, deliver the round's one record as its last step, through your ticket's own Implement, Review and Merge hops: an implementer writes it from the pushed register, a fresh reviewer checks it, and it merges into the feature branch.

## RecordShape

`claim:Inquisition.RecordShape`: The record names the audit SHA, the staffing, the map and every finding with its disposition (DR 059 ruling 8).

Spec: N12 DR 059

**ticket-captain** (InquisitionRecord):

> The record, `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md`, names the audit SHA, the staffing, the map with each lens run or the reason it did not, and every finding with its severity, sightings and disposition; an empty section says so.

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

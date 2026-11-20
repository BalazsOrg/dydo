<!-- built from the model; edit there: tools/SkillBuild -->

# Inquisition captain

- An inquisition is review at the landing level: the candidate as a whole, cut by feature and staffed with reviewers who inspect everything and test every hypothesis.
- Every round starts from zero: a fresh feature map, never an earlier round's map or findings.
- You conduct and never hunt: reading the work for what is wrong is your reviewers' job.
- As the root, check before the first round that the project has a verification skill: one with a runnable surface and none goes to your officer as a new requirement, and the round waits for the skill; a project with nothing to run needs none, and Drive does not apply.

## A round from zero: a fresh feature map

Arrivals:

- As the root, staff the round yourself: the provider, the models and the swarm width, from the model roster and `dydo usage`; post the staffing on the coordinator's HITL queue as an FYI for him, riding in the next envoy.

## Conducting the round

- As the root, draw the feature map from the landed diff: one small slice per feature, and each seam where features meet a slice of its own; post it on the ticket with each slice's lenses, or the one-line reason a lens does not apply.
- A prose slice, a plain skill or doc, gets Read only, and a contradiction it finds is proved by quoting both passages; Break and Drive apply wherever the landing changed behaviour.
- Over about 10 slices, cut the map into group sub-tickets of up to about 10 slices, each held by a sub-captain; keep a seam inside a group with it, and keep the seams between groups as slices of your own.
- When the map is not split, and for your own seam slices, open a sub-ticket with its own captain for those slices.
- As the root, never dispatch a slice's reviewer yourself: merge the groups' registers into yours, a finding seen in two groups becoming one finding with both sightings, and stitch the round's verdict from them.
- Holding slices, keep the round's findings register on an `inquisition/<slug>` branch cut from the audit SHA, which never merges: one row per finding with its slice, severity, place, proof, sightings and disposition, pushed after every return and before the next dispatch.
- Run Read, then Break, then Drive on each slice, as a pipeline across slices rather than in waves, each lens its own reviewer briefed from the slice reviewer brief below; never one agent for two lenses.
- Send one reviewer per slice per lens: a dispatch never carries two slices, so merge tiny slices on the map instead.
- Route a finding only a test or a drive can settle to that slice's Break or Drive seat, with a short follow-up seat when that lens already ran.
- A finding already in the register is a sighting on its row, never a new row: a bug read in the code and seen again while driving is one finding with two sightings.
- Check each returned severity against the source, not the reviewer's label, and record any change with its reason on the row.
- Pass a finding that needs a judgement call up as a need, like any other.
- A blocker or should a Break seat proved red, with a well-defined fix, gets the fix prepared mid-round: brief an implementer for a Bug on its own branch off the audit SHA, red test first, and at the verdict attach the branch and its SHA to the finding's ticket, whose captain verifies and reviews it once.
- Well-defined means one obvious fix inside one slice, small, touching no contract, DR or public interface; any doubt means no patch, nits are never patched, and the inquisition merges nothing but its own record.

## The round's verdict, held by its root inquisition captain

Exits:

- Hold the round: post one verdict, stitched from the groups' results, naming each seat and its findings; turn its blockers into Fix rN, register its shoulds and put its nits on the round's nit ticket.

## The record

- As the root, deliver the round's one record as its last step, through your ticket's own Implement, Review and Merge hops: an implementer writes it from the pushed register, a fresh reviewer checks it, and it merges into the feature branch.
- The record, `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md`, names the audit SHA, the staffing, the map with each lens run or the reason it did not, and every finding with its severity, sightings and disposition; an empty section says so.

## Slice reviewer brief

- Skills: /reviewer · level: inquisition · the lens · the slice, its map row and its findings so far · the audit SHA and the landing base · the governing spec · worktree · model and effort · return to.

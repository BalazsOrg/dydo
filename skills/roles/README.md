# Roles

6 skills that carry an identity, sorted by whether they hold something or return a result. The
builder writes every file under `officers/` and `crew/` from the claim model in `tools/SkillBuild`;
edit there, never here.

## Officers

Officers hold something and never switch: an admiral holds one open Project, a ticket-captain holds
a ticket (a sub-captain is a ticket-captain one level down), and the chief-of-staff keeps his short
list across Projects and the overnight watch.

- **admiral** (user-invoked): Hold one open Project to its landing, through its spec, map, captains, envoys and merges.
- **chief-of-staff** (user-invoked): Your short list across Projects, and the overnight watch.
- **ticket-captain** (model-invoked): A ticket to hold to its end - cut its hops, run its review rounds, merge it. Also a sub-captain's unit.

## Crew

Crew are dispatched for one job, hold at most their own hop, and return their result to whoever sent
them. code-writer absorbs the former docs-writer, reviewer the former inquisitor (and carries the
council resource), scout the former research.

- **code-writer** (model-invoked): A writer hop - implement or fix a ticket's code or docs, or resolve a merge conflict, from a code-writer brief.
- **reviewer** (model-invoked): A seat to judge - a review round, a conflict's hunks, an equivalent-mutant claim, an inquisition phase, or a council, clerk or judge seat.
- **scout** (model-invoked): One question to answer from its sources, off the record - a spec's scout pass, or an officer's homework.

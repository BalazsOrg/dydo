# Roles

7 skills that carry an identity, sorted by whether they hold something or return a result. The
builder writes every file under `officers/` and `crew/` from the claim model in `tools/SkillBuild`;
edit there, never here.

## Officers

Officers hold something and never switch: a coordinator holds one open Project, a ticket-captain
holds a ticket (a sub-captain is a ticket-captain one level down), and the chief-of-staff keeps his
short list across Projects and the overnight watch.

- **coordinator** (user-invoked): Hold one open Project to its landing, through its spec, map, captains, envoys and merges.
- **chief-of-staff** (user-invoked): Your short list across Projects, and the overnight watch.
- **ticket-captain** (model-invoked): A ticket to hold to its end - cut its hops, run its review rounds, merge it. Also a sub-captain's unit.

## Crew

Crew are dispatched for one job, hold at most their own hop, and return their result to whoever sent
them. The kinds are fixed: implementer, reviewer, research and scout.

- **implementer** (model-invoked): A ticket hop to build: create, write or change its files, mostly code, and verify them as you go - an Implement or Fix hop, now and then a merge conflict.
- **reviewer** (model-invoked): One candidate to judge at the level your brief names - a leaf's evidence check, a ticket review, a feature or Milestone review, or an inquisition slice; also a conflict's hunks, an equivalent-mutant claim or a new ignore entry.
- **research** (model-invoked): A question a choice waits on - pool evidence from every source family that could own the answer, verify each claim at its source, and cite it.
- **scout** (model-invoked): One question, one lookup, off the record - one source family for research, or a spec's Questions pass.

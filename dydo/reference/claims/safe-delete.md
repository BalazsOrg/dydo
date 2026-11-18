---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: SafeDelete

## Archive

`claim:SafeDelete.Archive`: After a merge, the captain safe-deletes the branches and worktrees it made.

Spec: N10, N12 DR 045 §8

Transition in Safe delete (`claims/statechart/safe-delete/SafeDelete.tla`): Live → Archived, "after its merge, its captain safe-deletes it".

**ticket-captain** (Live, exit):

> After a merge, safe-delete the branches and worktrees you made with `dydo archive`; it keeps them 30 days, then purges them.

**working-tree-standard** (TreeCleanup):

> After a merge, its captain safe-deletes the branches and worktrees it made; the archive keeps them 30 days, then purges.

## ArchiveT3

`claim:SafeDelete.ArchiveT3`: Safe delete covers T3 worktrees without breaking T3.

Spec: N10

Transition in Safe delete (`claims/statechart/safe-delete/SafeDelete.tla`): Live → Archived, "a T3 worktree is archived without breaking T3". A system transition: no skill carries it.

## Purge

`claim:SafeDelete.Purge`: An archive is purged after 30 days.

Spec: N10

Transition in Safe delete (`claims/statechart/safe-delete/SafeDelete.tla`): Archived → Purged, "30 days pass". A system transition: no skill carries it.

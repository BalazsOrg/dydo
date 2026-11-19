---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: MapView

## Open

`claim:MapView.Open`: He opens a map: a fresh read of the top-level tickets, read-only and on demand.

Spec: S19, S21, S26, H6

Transition in dydo map (`claims/statechart/map/Map.tla`): Closed → Fresh, "he opens a Project's map: a fresh read from Linear". A system transition: no skill carries it.

## Revisit

`claim:MapView.Revisit`: A revisit shows last visit's snapshot first.

Spec: S26

Transition in dydo map (`claims/statechart/map/Map.tla`): Closed → Snapshot, "he opens a map he has opened before: last visit's snapshot first". A system transition: no skill carries it.

## Animate

`claim:MapView.Animate`: On a revisit, fresh changes animate over the snapshot; Linear stays authoritative.

Spec: S26

Transition in dydo map (`claims/statechart/map/Map.tla`): Snapshot → Fresh, "the fresh read arrives and its changes animate". A system transition: no skill carries it.

## Save

`claim:MapView.Save`: The map keeps one disposable, machine-local snapshot per opened Project, outside repositories.

Spec: S26

Transition in dydo map (`claims/statechart/map/Map.tla`): Fresh → Shown, "the snapshot is written outside any repository". A system transition: no skill carries it.

## SaveFails

`claim:MapView.SaveFails`: A failed cache write never fails a fresh read.

Spec: S26

Transition in dydo map (`claims/statechart/map/Map.tla`): Fresh → Shown, "the cache write fails; the fresh read still stands". A system transition: no skill carries it.

## Expand

`claim:MapView.Expand`: Expanding a card applies the same layout at any depth.

Spec: S19

Transition in dydo map (`claims/statechart/map/Map.tla`): Shown → Expanded, "he expands a card: the same layout at any depth". A system transition: no skill carries it.

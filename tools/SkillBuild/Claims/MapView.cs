namespace DynaDocs.SkillBuild;

/// <summary>dydo map, his strategic lens (the Map machine, S19–S21, S26): a fresh read on top of last visit's snapshot, and a cache write that never fails the read.</summary>
public static class MapView
{
    public static readonly Claim Open = new("He opens a map: a fresh read of the top-level tickets, read-only and on demand.", [Actor.Balazs], [])
    { Spec = ["S19", "S21", "S26", "H6"], Transition = new(MapState.Closed, MapState.Fresh, "he opens a Project's map: a fresh read from Linear") };

    public static readonly Claim Revisit = new("A revisit shows last visit's snapshot first.", [Actor.Balazs], [])
    { Spec = ["S26"], Transition = new(MapState.Closed, MapState.Snapshot, "he opens a map he has opened before: last visit's snapshot first") };

    public static readonly Claim Animate = new("On a revisit, fresh changes animate over the snapshot; Linear stays authoritative.", [Actor.DydoMap], [])
    { Spec = ["S26"], Transition = new(MapState.Snapshot, MapState.Fresh, "the fresh read arrives and its changes animate") };

    public static readonly Claim Save = new("The map keeps one disposable, machine-local snapshot per opened Project, outside repositories.", [Actor.DydoMap], [])
    { Spec = ["S26"], Transition = new(MapState.Fresh, MapState.Shown, "the snapshot is written outside any repository") };

    public static readonly Claim SaveFails = new("A failed cache write never fails a fresh read.", [Actor.DydoMap], [])
    { Spec = ["S26"], Transition = new(MapState.Fresh, MapState.Shown, "the cache write fails; the fresh read still stands") };

    public static readonly Claim Expand = new("Expanding a card applies the same layout at any depth.", [Actor.Balazs], [])
    { Spec = ["S19"], Transition = new(MapState.Shown, MapState.Expanded, "he expands a card: the same layout at any depth") };
}

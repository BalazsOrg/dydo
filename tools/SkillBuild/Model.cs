namespace DynaDocs.SkillBuild;

/// <summary>A claim model: its kind classes, one per kind, and the skills and standards its claims build.</summary>
public sealed record Model(IReadOnlyList<Type> Kinds, IReadOnlyList<SkillSpec> Skills)
{
    /// <summary>The machines its states belong to; with any, every skill state is a machine state.</summary>
    public IReadOnlyList<Machine> Machines { get; init; } = [];

    /// <summary>Every claim and machine state cites at least one spec rule; the production model does, fixtures need not.</summary>
    public bool CitesSpec { get; init; }

    /// <summary>The envoy skill table and its closed-thread signal, checked at build (H23); a model without envoys has none.</summary>
    public EnvoyTable? Envoys { get; init; }
}

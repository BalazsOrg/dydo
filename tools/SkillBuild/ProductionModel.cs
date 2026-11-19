namespace DynaDocs.SkillBuild;

/// <summary>
/// The model <c>dotnet run --project tools/SkillBuild</c> builds, read from <see cref="Registry"/>. Each
/// kind class goes in <c>Claims/&lt;Kind&gt;.cs</c>, in this namespace, because a kind namespace would
/// form a cycle with the registry. Every claim and machine state cites the spec.
/// </summary>
public static class ProductionModel
{
    public static Model Model { get; } = new(Registry.Kinds, Registry.Skills)
    {
        Machines = Registry.Machines,
        CitesSpec = true,
        Envoys = new(Envoy.Jobs, Envoy.CloseSignal, ResourceName.EnvoyBrief)
    };
}

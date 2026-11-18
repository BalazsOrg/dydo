namespace DynaDocs.SkillBuild;

/// <summary>
/// The production model's parts (DYD-1082): every machine, kind class and skill. Machines come first,
/// because a skill's <see cref="Machine.Slots"/> reads its machine while this class initializes.
/// </summary>
public static class Registry
{
    public static IReadOnlyList<Machine> Machines { get; } = [];

    public static IReadOnlyList<Type> Kinds { get; } = [];

    public static IReadOnlyList<SkillSpec> Skills { get; } = [];
}

namespace DynaDocs.SkillBuild;

/// <summary>A claim model: its kind classes, one per kind, and the skills and standards its claims build.</summary>
public sealed record Model(IReadOnlyList<Type> Kinds, IReadOnlyList<SkillSpec> Skills);

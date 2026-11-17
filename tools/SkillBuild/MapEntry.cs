namespace DynaDocs.SkillBuild;

/// <summary>One emitted line range of a built file and the unit and claims it carries.</summary>
internal sealed record MapEntry(int[] Lines, string Unit, IReadOnlyList<string> Claims);

namespace DynaDocs.SkillBuild;

/// <summary>A machine's state, with its title and the spec rule ids it rests on.</summary>
public sealed record State(Enum Id, string Title, params string[] Spec);

namespace DynaDocs.SkillBuild;

/// <summary>
/// An on-demand resource of a skill, <c>resources/&lt;kebab-case id&gt;.md</c>, linked from the spine's
/// resource list and laid out in its sections.
/// </summary>
public sealed record Resource(Enum Id, string Title, Slot[] Sections);

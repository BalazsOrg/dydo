namespace DynaDocs.SkillBuild;

/// <summary>
/// A state, routine, section or appendix a skill declares, with its heading. Only a section that
/// opens a resource or a standard may have no title; its lines follow the file's H1 directly.
/// </summary>
public sealed record Slot(Enum Id, string? Title = null);

namespace DynaDocs.SkillBuild;

/// <summary>
/// An arrow the spec leaves open, from a state to a state or, with no <paramref name="To"/>, to a state
/// the spec does not name. It is drawn as <c>GAP: note</c> and never rendered into a skill.
/// </summary>
public sealed record Gap(Enum From, Enum? To, string Note);

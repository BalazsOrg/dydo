namespace DynaDocs.SkillBuild;

/// <summary>
/// An on-demand resource of a skill, <c>resources/&lt;kebab-case id&gt;.md</c>, laid out in its slots: a
/// phase's resource, linked from that phase, or a scenario, linked from "When things go wrong" (K9).
/// A dispatch template names its default <see cref="Skills"/> (K14).
/// </summary>
public sealed record Resource(Enum Id, string Title, params Slot[] Slots)
{
    /// <summary>
    /// A dispatch template's default skill set: its first line names each with a slash, and the run
    /// fails on a name that is no skill under <c>skills/</c>, hand-written or built (<see cref="SkillNames"/>).
    /// </summary>
    public IReadOnlyList<string> Skills { get; init; } = [];

    /// <summary>The machine states a truth-checked skill's scenario covers, as <see cref="Phase.States"/> does for a phase.</summary>
    public IReadOnlyList<Enum> States { get; init; } = [];
}

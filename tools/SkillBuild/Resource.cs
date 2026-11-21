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

    /// <summary>
    /// The actors this resource belongs to. Naming them truth-checks the resource ahead of its skill (DYD-1402):
    /// its lines, claims, slots and file are held to what <see cref="SkillSpec.Readers"/> holds a whole skill to,
    /// and its open claims are withheld and listed, while the rest of the skill builds as it did. It covers no
    /// machine state: a move's line stays in its skill's state until the skill is switched over. A truth-checked
    /// skill's resources name none.
    /// </summary>
    public IReadOnlyList<Enum> Readers { get; init; } = [];
}

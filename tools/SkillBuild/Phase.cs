namespace DynaDocs.SkillBuild;

/// <summary>
/// One numbered job phase of a spine (K9): its title, its slots in the order the role meets them, and
/// the resources it links, such as the dispatch templates of the crew it sends.
/// </summary>
public sealed record Phase(string Title, params Slot[] Slots)
{
    public IReadOnlyList<Resource> Resources { get; init; } = [];

    /// <summary>
    /// The machine states a truth-checked skill's phase covers: a transition claim's line may sit in any
    /// slot of a phase, or of its resources, that names the move's from-state or to-state.
    /// </summary>
    public IReadOnlyList<Enum> States { get; init; } = [];
}

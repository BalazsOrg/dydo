namespace DynaDocs.SkillBuild;

/// <summary>
/// One numbered job phase of a spine (K9): its title, its slots in the order the role meets them, and
/// the resources it links, such as the dispatch templates of the crew it sends.
/// </summary>
public sealed record Phase(string Title, params Slot[] Slots)
{
    public IReadOnlyList<Resource> Resources { get; init; } = [];
}

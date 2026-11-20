namespace DynaDocs.SkillBuild;

/// <summary>
/// Where a line sits in its skill (K9): a slot the skill declares (a <see cref="SpinePart"/>, a state or
/// a list in a job phase or resource, or a standard's section) and its order within that slot. A line in
/// a state is a move and names its <paramref name="Step"/>.
/// </summary>
public sealed record Place(Enum Slot, int Order, MoveKind? Step = null);

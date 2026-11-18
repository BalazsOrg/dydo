namespace DynaDocs.SkillBuild;

/// <summary>
/// Where a line sits in its skill (K9): a slot the skill declares (a <see cref="HeaderPart"/>, a state,
/// a routine, a section or an appendix) and its order within that slot. A line in a state is a move
/// and names its <paramref name="Step"/>; a move may link the appendix for the scenario it raises.
/// </summary>
public sealed record Place(Enum Slot, int Order, MoveKind? Step = null, Enum? Appendix = null);

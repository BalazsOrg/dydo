namespace DynaDocs.SkillBuild;

/// <summary>
/// A place a skill declares for lines, with its heading: a state, whose lines are moves
/// (<see cref="State"/>, or <see cref="Machine.Slots"/>), or a list of lines. Only the slot that opens
/// a job phase, a resource or a standard may have no title; its lines follow that heading directly. A
/// resource or a standard heads each titled slot with its title; a job phase renders its slots as one
/// numbered checklist, so there the title names the slot in the model only (K9).
/// </summary>
public sealed record Slot(Enum Id, string? Title = null)
{
    public bool IsState { get; private init; }

    public static Slot State(Enum id, string? title) => new(id, title) { IsState = true };
}

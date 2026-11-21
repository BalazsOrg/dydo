namespace DynaDocs.SkillBuild;

/// <summary>
/// A place a skill declares for lines, with its heading: a state, whose lines are moves
/// (<see cref="State"/>, or <see cref="Machine.Slots"/>), or a list of lines. Only the slot that opens
/// a job phase, a resource or a standard may have no title; its lines follow that heading directly. A
/// resource or a standard heads each titled slot with its title; a job phase renders its slots as one
/// numbered checklist, so there the title names the slot in the model only (K9). A truth-checked skill
/// declares each slot by its shape instead (<see cref="Paragraph"/>, <see cref="Bullets"/>,
/// <see cref="Steps"/>), any of them untitled, and a title is the bold lead-in the slot opens with.
/// </summary>
public sealed record Slot(Enum Id, string? Title = null)
{
    public bool IsState { get; private init; }

    /// <summary>How the slot reads in a truth-checked skill; a legacy slot has none.</summary>
    public SlotShape? Shape { get; private init; }

    public static Slot State(Enum id, string? title) => new(id, title) { IsState = true };

    public static Slot Paragraph(Enum id, string? title = null) => new(id, title) { Shape = SlotShape.Paragraph };

    public static Slot Bullets(Enum id, string? title = null) => new(id, title) { Shape = SlotShape.Bullets };

    /// <summary>Numbered 1, 2, 3: only where the order is real.</summary>
    public static Slot Steps(Enum id, string? title = null) => new(id, title) { Shape = SlotShape.Steps };
}

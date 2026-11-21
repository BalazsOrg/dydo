namespace DynaDocs.SkillBuild;

/// <summary>
/// How a truth-checked skill's slot reads (DYD-1295 519126ab): a paragraph whose sentences are claim
/// lines, an unordered list, or steps numbered 1, 2, 3, used only where the order is real.
/// </summary>
public enum SlotShape
{
    Paragraph,
    Bullets,
    Steps
}

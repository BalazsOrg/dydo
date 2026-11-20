namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>
/// The truth fields the fixture's claims share, apart from its kinds and its model, so no kind class
/// reads the model while both initialize. A broken kind swaps one field with <c>with</c>.
/// </summary>
internal static class Verified
{
    public static readonly Truth ByRun = new(
        Evidence.Run("A fixture run showed it."),
        Layer: "Nothing below the skill covers it.",
        Reason: "Holds the fixture on its path.");
}

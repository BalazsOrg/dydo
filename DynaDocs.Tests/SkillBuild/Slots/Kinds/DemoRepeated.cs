namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>Two claims that build the same long text, so a paste of it states both (K24).</summary>
public static class DemoRepeated
{
    public const string Text = "Hand the ticket back to the officer with its proof.";

    public static readonly Claim One = new("Return with proof.", [DemoActor.Crew],
        [new(DemoSkill.Demo, Text, new(DemoSlot.Dispatch, 2))]);

    public static readonly Claim Two = new("Return to the officer.", [DemoActor.Crew],
        [new(DemoSkill.Demo, Text, new(DemoSlot.Dispatch, 3))]);
}

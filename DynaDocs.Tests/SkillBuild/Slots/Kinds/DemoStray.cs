namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind when built alone: its combined line names a claim of a kind the model does not list.</summary>
public static class DemoStray
{
    public static readonly Claim Wanders = new("Stay inside the model.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Stay inside the model.", new(DemoSlot.Dispatch, 2), () => [DemoTwice.SaidTwice])]);
}

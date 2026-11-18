namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a combined line names a claim the skill already states on its own line (K4).</summary>
public static class DemoEcho
{
    public static readonly Claim Echo = new("Echo once.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Echo once.", new(DemoSlot.Dispatch, 2))]);

    public static readonly Claim Echoed = new("Answer the echo.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Answer the echo, and echo once.", new(DemoSlot.Dispatch, 3), () => [DemoEcho.Echo])]);
}

namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim the skill carries whose only line was left out (K4).</summary>
public static class DemoSilent
{
    public static readonly Claim Unsaid = new("Say what you carry.", [DemoActor.Crew], []);
}

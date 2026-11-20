namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim only a legacy skill states, its command evidence naming a missing command.</summary>
public static class ClerkBadEvidence
{
    public static readonly Claim Gate = new("The clerk runs the gate.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, "Run the project's gate.", new(OldSlot.Desk, 2))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Command("dydo test gate") }
    };
}

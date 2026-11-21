namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A claim only a legacy skill states, its code evidence a path the repository may not hold.</summary>
public static class ClerkCodePath
{
    public static readonly Claim Filed = new("The clerk files its notes.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, "File your notes in the docs.", new(OldSlot.Desk, 2))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Code("docs/clerk.md") }
    };
}

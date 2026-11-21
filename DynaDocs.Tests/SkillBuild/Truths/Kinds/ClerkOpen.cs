namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an open claim whose line sits in a legacy skill, which withholds nothing.</summary>
public static class ClerkOpen
{
    public static readonly Claim Unseen = new("The clerk rings the bell at noon.", [KeeperActor.Keeper],
        [new(OldSkill.Clerk, "Ring the bell at noon.", new(OldSlot.Desk, 2))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Open("Nobody has watched a noon yet.") }
    };
}

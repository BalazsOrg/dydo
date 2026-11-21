namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line of the rota read by an actor the rota does not belong to.</summary>
public static class ClerkRotaWrongReader
{
    public static readonly Claim Landing = new("The lander reads the rota.", [KeeperActor.Lander],
        [new(OldSkill.Clerk, KeeperActor.Lander, "Read the rota before you land.", new(AheadSlot.RotaRules, 2))]) { Truth = Verified.ByRun };
}

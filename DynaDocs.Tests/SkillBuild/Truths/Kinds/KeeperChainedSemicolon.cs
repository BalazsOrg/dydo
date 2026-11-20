namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a missing command after <c>;</c>, on a fenced line.</summary>
public static class KeeperChainedSemicolon
{
    public static readonly Claim Gate = new("The keeper checks, then runs the gate.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "```sh\ndydo check ; dydo test gate\n```", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

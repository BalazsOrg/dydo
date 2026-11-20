namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an option the command lacks, written in brackets.</summary>
public static class KeeperBracketOption
{
    public static readonly Claim Flag = new("The keeper checks with a flag.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo check [--no-such-flag]`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

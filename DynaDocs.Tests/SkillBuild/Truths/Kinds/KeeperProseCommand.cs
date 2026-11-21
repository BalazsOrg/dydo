namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a command named outside code, where no check reads it.</summary>
public static class KeeperProseCommand
{
    public static readonly Claim Board = new("The keeper reads the board.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run dydo survey board first.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

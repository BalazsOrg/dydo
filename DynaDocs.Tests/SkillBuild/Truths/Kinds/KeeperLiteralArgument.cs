namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a literal word where the command takes an argument.</summary>
public static class KeeperLiteralArgument
{
    public static readonly Claim Board = new("The keeper reads the board.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Read `dydo survey board <project>` first.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

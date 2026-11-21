namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a literal word with a capital.</summary>
public static class KeeperCapitalLiteral
{
    public static readonly Claim Board = new("The keeper reads the board.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Read `dydo survey Board` first.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

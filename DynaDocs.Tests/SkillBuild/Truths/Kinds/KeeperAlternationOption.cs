namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an option only the first word of <c>a|b</c> has.</summary>
public static class KeeperAlternationOption
{
    public static readonly Claim Flag = new("The keeper restores or archives to a path.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run `dydo archive restore|branch <id> --path <dir>`.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

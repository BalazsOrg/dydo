namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a truth-checked skill's line that names no reader.</summary>
public static class KeeperNoReader
{
    public static readonly Claim Anyone = new("Somebody keeps the log.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, "Keep the log.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun
    };
}

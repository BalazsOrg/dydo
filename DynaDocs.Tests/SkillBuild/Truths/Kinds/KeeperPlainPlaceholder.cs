namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a placeholder whose token does not read as one, so a reader could take it for the text.</summary>
public static class KeeperPlainPlaceholder
{
    public static readonly Claim Bells = new("The keeper rings the bells the harbour master lists.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Once the list stands below, ring each bell on it.\n\nthe bells", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Placeholder("the bells", "The list is the harbour master's to write, and it is not written.") }
    };
}

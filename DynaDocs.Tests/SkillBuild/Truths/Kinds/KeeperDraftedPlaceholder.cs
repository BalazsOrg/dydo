namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a placeholder whose line drafts the text it stands for, below its token.</summary>
public static class KeeperDraftedPlaceholder
{
    public static readonly Claim Bells = new("The keeper rings the bells the harbour master lists.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Once the list stands below, ring each bell on it.\n\n{bell_placeholder}\n\nThe north bell, then the south.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Placeholder("{bell_placeholder}", "The list is the harbour master's to write, and it is not written.") }
    };
}

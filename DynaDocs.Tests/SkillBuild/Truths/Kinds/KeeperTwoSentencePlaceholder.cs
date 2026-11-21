namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a placeholder whose lead holds a second sentence, so the text it stands for is drafted above its token.</summary>
public static class KeeperTwoSentencePlaceholder
{
    public static readonly Claim Bells = new("The keeper rings the bells the harbour master lists.", [KeeperActor.Keeper],
    [
        new(KeeperSkill.Keeper, KeeperActor.Keeper,
            "Ring the north bell for a code seat, the south bell for a docs seat. Ring the east bell for a drive seat, and the west for review.\n\n{bell_placeholder}",
            new(KeeperSlot.Extra, 1))
    ])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Placeholder("{bell_placeholder}", "The list is the harbour master's to write, and it is not written.") }
    };
}

namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind beside <see cref="KeeperPlaceholder"/>: a second placeholder, which should have been withheld.</summary>
public static class KeeperSecondPlaceholder
{
    public static readonly Claim Lamps = new("The keeper lights the lamps the harbour master lists.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Once the list stands below, light each lamp on it.\n\n{lamp_placeholder}", new(KeeperSlot.Extra, 2))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Placeholder("{lamp_placeholder}", "The list is the harbour master's to write, and it is not written.") }
    };
}

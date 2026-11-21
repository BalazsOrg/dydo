namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A placeholder: a text that is someone's to write and is not written, shown by its token where it will stand.</summary>
public static class KeeperPlaceholder
{
    public const string Token = "{bell_placeholder}";

    public static readonly Claim Bells = new("The keeper rings the bells the harbour master lists.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Once the list stands below, ring each bell on it.\n\n" + Token, new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Placeholder(Token, "The list is the harbour master's to write, and it is not written.") }
    };
}

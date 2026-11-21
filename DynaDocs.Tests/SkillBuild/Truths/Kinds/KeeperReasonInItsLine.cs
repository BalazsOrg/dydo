namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>An honest reason whose words its line also uses: the line says what to do, the reason why.</summary>
public static class KeeperReasonInItsLine
{
    public static readonly Claim Split = new("A watch too long for one keeper is split.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Split a watch that one keeper cannot hold in its context.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Reason = "One keeper cannot hold more than its context." }
    };
}

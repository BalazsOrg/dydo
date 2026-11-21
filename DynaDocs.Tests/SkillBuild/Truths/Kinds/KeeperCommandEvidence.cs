namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a command evidence naming a command the CLI does not have.</summary>
public static class KeeperCommandEvidence
{
    public static readonly Claim Gate = new("The keeper runs the gate.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run the project's gate.", new(KeeperSlot.Extra, 1))])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Command("dydo test gate") }
    };
}

namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: an open claim whose line also states a verified one, so it could not be withheld alone.</summary>
public static class KeeperOpenCombined
{
    public static readonly Claim Seen = new("The keeper locks the gate.", [KeeperActor.Keeper], []) { Truth = Verified.ByRun };

    public static readonly Claim Unseen = new("The keeper rings the bell at noon.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Ring the bell at noon, then lock the gate.", new(KeeperSlot.Extra, 1), () => [Seen])])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Open("Nobody has watched a noon yet.") }
    };
}

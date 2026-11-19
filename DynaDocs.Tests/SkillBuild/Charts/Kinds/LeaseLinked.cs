namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A line linking its skill's resource, as a move links the detail it needs (K29); the why doc must not carry that link.</summary>
public static class LeaseLinked
{
    public static readonly Claim Guide = new("A held lease follows its guide.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Follow [the lease guide](resources/lease-guide.md) while you hold it.", new(LeaseState.Held, 3, MoveKind.Nominal))])
    { Spec = ["K29"] };
}

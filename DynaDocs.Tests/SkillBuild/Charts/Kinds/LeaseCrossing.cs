namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a system transition from the other machine's state into the lease's.</summary>
public static class LeaseCrossing
{
    public static readonly Claim Leap = new("An idle machine leaps into a lease.", [], [])
    {
        Spec = ["K1"],
        Transition = new(OtherState.Idle, LeaseState.Held, "it leaps")
    };
}

namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a transition's line on the nominal path of its from-state, which is no move.</summary>
public static class LeaseMisplaced
{
    public static readonly Claim Wander = new("A held lease wanders to review.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Wander to review.", new(LeaseState.Held, 2, MoveKind.Nominal))])
    {
        Spec = ["R1"],
        Transition = new(LeaseState.Held, LeaseState.Review, "it wanders")
    };
}

namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>The chart fixture's claims: three moves carried by skills, one system transition, and two lines that are no move.</summary>
public static class LeaseMoves
{
    public static readonly Claim HoldsOneLease = new("The holder holds one lease.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "You hold one lease at a time.", new(HeaderPart.Role, 1))]) { Spec = ["K9"] };

    public static readonly Claim ReviewsReturns = new("The warden reviews what holders return.", [ChartSkill.Warden],
        [new(ChartSkill.Warden, "You review each returned lease.", new(HeaderPart.Role, 1))]) { Spec = ["R1"] };

    public static readonly Claim Take = new("A holder takes a free lease when dispatched.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Take the lease when dispatched.", new(LeaseState.Held, 1, MoveKind.Arrival))])
    {
        Spec = ["K9"],
        Transition = new(LeaseState.Free, LeaseState.Held, "the holder takes the lease")
    };

    public static readonly Claim KeepProofGreen = new("The proof stays green while the lease is held.", [ChartSkill.Holder],
        [new(ChartSkill.Holder, "Keep the proof green.", new(LeaseState.Held, 1, MoveKind.Nominal))]) { Spec = ["T3"] };

    public static readonly Claim Return = new("A held lease returns for review once its proof is green.", [ChartSkill.Holder, ChartSkill.Warden],
    [
        new(ChartSkill.Holder, "Return for review once the proof is green.", new(LeaseState.Held, 1, MoveKind.Exit)),
        new(ChartSkill.Warden, "Review what the holder returns.", new(LeaseState.Review, 1, MoveKind.Arrival))
    ])
    {
        Spec = ["R1", "K28"],
        Transition = new(LeaseState.Held, LeaseState.Review, "the holder returns it")
    };

    public static readonly Claim Fail = new("A failed review sends the lease back, at most twice.", [ChartSkill.Holder, ChartSkill.Warden],
    [
        new(ChartSkill.Warden, "Fail the review with its findings.", new(LeaseState.Review, 1, MoveKind.Exit)),
        new(ChartSkill.Holder, "Take the findings back to work.", new(LeaseState.Held, 2, MoveKind.Arrival))
    ])
    {
        Spec = ["R3"],
        Transition = new(LeaseState.Review, LeaseState.Held, "the review fails") { Guard = "fixes < 2", Effect = "fixes' = fixes + 1" }
    };

    public static readonly Claim Land = new("A passed review lands by command.", [], [])
    {
        Spec = ["R2", "CA19"],
        Transition = new(LeaseState.Review, LeaseState.Done, "the merge lands") { Fair = true }
    };
}

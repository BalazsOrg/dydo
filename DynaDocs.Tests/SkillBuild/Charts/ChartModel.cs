namespace DynaDocs.Tests.SkillBuild.Charts;

using DynaDocs.SkillBuild;

/// <summary>
/// The chart fixture: a four-state lease machine with a bounded variable and two gaps, carried by a
/// holder and a warden on either side of review. It cites the spec, as the production model must.
/// </summary>
internal static class ChartModel
{
    public static readonly Machine Lease = new(ChartMachine.Lease, "Lease",
        [new(LeaseState.Free, "Free", "K1"), new(LeaseState.Held, "Held", "K9"), new(LeaseState.Review, "In review", "R1"), new(LeaseState.Done, "Done", "R2", "N12 DR 049 §2–4")],
        LeaseState.Free, [LeaseState.Done])
    {
        Extends = ["Naturals"],
        Variables = [new("fixes", "0", "fixes <= 2")],
        Gaps =
        [
            new(LeaseState.Held, LeaseState.Free, "the spec names no release of a held lease"),
            new(LeaseState.Review, null, "the spec names no move after a second failed fix")
        ]
    };

    public static readonly Machine Other = new(ChartMachine.Other, "Other", [new(OtherState.Idle, "Idle", "K1")], OtherState.Idle, []);

    public static readonly SkillSpec Holder =
        SkillSpec.Crew(ChartSkill.Holder, "Holder", "Holds a lease.") with { Phases = [new("Hold", Lease.Slots(LeaseState.Held))] };

    public static readonly SkillSpec Warden =
        SkillSpec.Officer(ChartSkill.Warden, "Warden", "Reviews a lease.") with { Phases = [new("Review", Lease.Slots(LeaseState.Review))] };

    public static Model With(params Type[] kinds) => Of([Lease], [Holder, Warden], kinds);

    public static Model Of(Machine[] machines, SkillSpec[] skills, params Type[] kinds) =>
        new([typeof(LeaseMoves), .. kinds], skills) { Machines = machines, CitesSpec = true };
}

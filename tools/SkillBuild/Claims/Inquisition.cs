namespace DynaDocs.SkillBuild;

/// <summary>An inquisition seat's phases (N5): by default Read, then Break, then Drive, on one area, every round from zero.</summary>
public static class Inquisition
{
    public static readonly Claim Order = new("An area's phases run Read, Break, Drive by default; every round starts from zero.", [Actor.Reviewer],
        [new(Skill.Reviewer, "In an inquisition round, run your phase on your area; by default an area goes Read, then Break, then Drive, and every round starts from zero.", new(LandingState.ReadPhase, 2901, MoveKind.Nominal))])
    { Spec = ["N5"] };

    public static readonly Claim Read = new("Read looks for what is broken in ways neither tests nor clicking would catch.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Read: look for what is broken in ways neither tests nor clicking around would catch.", new(LandingState.ReadPhase, 2902, MoveKind.Nominal))])
    { Spec = ["N5"] };

    public static readonly Claim Break = new("Break forms hypotheses of failure and writes tests that confirm or refute them.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Break: form hypotheses about how the area fails, then write tests that confirm or refute them.", new(LandingState.BreakPhase, 2903, MoveKind.Nominal))])
    { Spec = ["N5"] };

    public static readonly Claim Drive = new("Drive runs the product against the spec's problem statement.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Drive: run every feature and edge case through the running app, against the spec's problem statement.", new(LandingState.DrivePhase, 2904, MoveKind.Nominal))])
    { Spec = ["N5", "R3"] };
}

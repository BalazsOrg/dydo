namespace DynaDocs.SkillBuild;

/// <summary>Out-of-scope problems (R5): any agent reports one to its officer, which routes it.</summary>
public static class Report
{
    public static readonly Claim Up = new("Any agent reports a problem outside its scope to its officer.", [Actor.Implementer, Actor.Reviewer, Actor.Scout, Actor.TicketCaptain],
    [
        new(Skill.Implementer, "Report a problem outside your scope to your officer.", new(DispatchState.Working, 2601, MoveKind.Nominal)),
        new(Skill.Reviewer, "Report a problem outside your scope to your officer.", new(ReviewState.Round, 2601, MoveKind.Nominal)),
        new(Skill.Scout, "Report a problem outside your question to your officer.", new(DispatchState.Working, 2601, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Report a problem outside your ticket to your officer.", new(DispatchState.Briefed, 2601, MoveKind.Nominal))
    ])
    { Spec = ["R5"] };

    public static readonly Claim Route = new("The officer routes an out-of-scope report to the owner's ticket or a new Bug.", [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Route an out-of-scope report to the owner's ticket or a new Bug.", new(ProjectState.Watching, 2602, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Route an out-of-scope report from your crew to the owner's ticket or a new Bug.", new(DispatchState.Returned, 2602, MoveKind.Nominal))
    ])
    { Spec = ["R5"] };
}

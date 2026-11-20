namespace DynaDocs.SkillBuild;

/// <summary>What every crew kind (K30) does with its brief: it loads each skill the dispatch names with a slash (K14).</summary>
public static class Crew
{
    public static readonly Claim Skills = new("Crew load every skill their dispatch names with a slash (DYD-1082 01266a65 Q4, c1c52e25).",
        [Actor.Implementer, Actor.Reviewer, Actor.Research, Actor.Scout],
    [
        new(Skill.Implementer, "Load every skill your brief names with a slash before you start.", new(DispatchState.Briefed, 3001, MoveKind.Nominal)),
        new(Skill.Reviewer, "Load every skill your brief names with a slash before you start.", new(DispatchState.Briefed, 3001, MoveKind.Nominal)),
        new(Skill.Research, "Load every skill your brief names with a slash before you start.", new(DispatchState.Briefed, 3001, MoveKind.Nominal)),
        new(Skill.Scout, "Load every skill your brief names with a slash before you start.", new(DispatchState.Briefed, 3001, MoveKind.Nominal))
    ])
    { Spec = ["K14", "K30"] };
}

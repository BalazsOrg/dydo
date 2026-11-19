namespace DynaDocs.SkillBuild;

/// <summary>The scout's job (R4, K22): a spec's one scout pass, and one factual question from its sources.</summary>
public static class Scout
{
    public static readonly Claim Pass = new("A spec's scout pass returns Questions, never PASS or FAIL.", [Actor.Scout],
        [new(Skill.Scout, "On a spec's scout pass, return Questions only, never PASS or FAIL.", new(DispatchState.Working, 2801, MoveKind.Nominal))])
    { Spec = ["R4"] };

    public static readonly Claim Answer = new("The scout answers one factual question from its primary sources, naming where each part lives.", [Actor.Scout],
        [new(Skill.Scout, "Answer one factual question from its primary sources.", new(DispatchState.Working, 2802, MoveKind.Nominal))])
    { Spec = ["K22", "H16"] };
}

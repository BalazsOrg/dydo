namespace DynaDocs.SkillBuild;

/// <summary>The gates a writer runs and a reviewer reads (DR 068, DR 069): dydo test and dydo code-check on changed files, the token limit, coverage as a report.</summary>
public static class Gates
{
    public static readonly Claim Run = new("Tests run through dydo test, statics through dydo code-check, on changed files only.", [Actor.Implementer],
        [new(Skill.Implementer, "Run tests through `dydo test`, and the static gates through `dydo code-check`, which checks changed files only.", new(DispatchState.Working, 2301, MoveKind.Nominal))])
    { Spec = ["Y6", "T21", "T13"] };

    public static readonly Claim TokenLimit = new("Every file, tests included, stays under the 5k-token limit, counted as bytes ÷ 4.", [Actor.Implementer],
        [new(Skill.Implementer, "The file token limit is 5k tokens by default, counted as bytes ÷ 4, for every file you change, tests included.", new(DispatchState.Working, 2302, MoveKind.Nominal))])
    { Spec = ["T15", "T14"] };

    public static readonly Claim Coverage = new("Coverage is a report, not a gate.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Coverage is a report, never a blocker.", new(ReviewState.Round, 2303, MoveKind.Nominal))])
    { Spec = ["T9"] };
}

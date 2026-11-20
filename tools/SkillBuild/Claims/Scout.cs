namespace DynaDocs.SkillBuild;

/// <summary>
/// The scout's job (K30, R4): one lookup in one source family, passages back for research, and a spec's one
/// Questions pass, carried over from the pre-candidate scout process in fresh prose (K22).
/// </summary>
public static class Scout
{
    public static readonly Claim Answer = new("The scout looks one question up in its one source family, searched to its end.", [Actor.Scout],
        [new(Skill.Scout, "Look the question up in the one source family your brief names, and search it to its end, following its references inside it.", new(DispatchState.Working, 2802, MoveKind.Nominal))])
    { Spec = ["K30", "K22"] };

    public static readonly Claim Passages = new("A scout's finding is the exact passage with its citation, from the source that owns the claim.", [Actor.Scout],
        [new(Skill.Scout, "Copy each finding as its exact passage with its citation (a path at its commit, or a URL), from the source that owns the claim; a doc's account of the code is a lead to the code.", new(DispatchState.Working, 2803, MoveKind.Nominal))])
    { Spec = ["K30", "K22"] };

    public static readonly Claim Nothing = new("An empty source is reported with what was searched.", [Actor.Scout],
        [new(Skill.Scout, "When your source holds nothing, say so and list what you searched.", new(DispatchState.Working, 2804, MoveKind.Nominal))])
    { Spec = ["K30", "K22"] };

    public static readonly Claim Pass = new("A spec's scout pass returns Questions, never PASS or FAIL.", [Actor.Scout],
        [new(Skill.Scout, "Return only Questions from that pass, never PASS or FAIL.", new(DispatchState.Working, 2806, MoveKind.Nominal))])
    { Spec = ["R4"] };

    public static readonly Claim PassHow = new("The Questions pass reads the spec as its implementer and turns each open choice into a Question.", [Actor.Scout],
        [new(Skill.Scout, "On a spec's Questions pass, read the spec as the implementer who must build it, and turn each choice it leaves open into a Question.", new(DispatchState.Working, 2805, MoveKind.Nominal))])
    { Spec = ["R4", "K30"] };
}

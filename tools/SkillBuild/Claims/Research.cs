namespace DynaDocs.SkillBuild;

/// <summary>
/// Research's job (K30, DYD-1082 99774812): it pools evidence from every source family through its scouts
/// and verifies each claim at its source, carried over from the pre-candidate research process in fresh
/// prose (K22).
/// </summary>
public static class Research
{
    public static readonly Claim NotLookup = new("Pooling and verifying is research's job; one lookup is a scout's (DYD-1082 99774812).", [Actor.Research],
        [new(Skill.Research, "Pooling and verifying is the job; a single lookup is a scout's, and you send scouts for those.", new(SpinePart.Job, 122))])
    { Spec = ["K30"] };

    public static readonly Claim Accounts = new("Research answers for a cited answer, what stayed unsettled and where it searched.", [Actor.Research],
        [new(Skill.Research, "You answer for an answer whose every claim carries a citation you opened, with what stayed unsettled and where you searched.", new(SpinePart.Job, 123))])
    { Spec = ["K30"] };

    public static readonly Claim Sources = new("Research's sources are every family that could own the answer.", [Actor.Research],
        [new(Skill.Research, "Your sources are every family that could own the answer: DRs, the spec and plans, the other docs, code and tests, git history, Linear, and outside sources such as official docs, specs and library source.", new(SpinePart.Job, 124))])
    { Spec = ["K30", "S1"] };

    public static readonly Claim Restate = new("Research restates its question and what would count as an answer.", [Actor.Research],
        [new(Skill.Research, "Restate the question in one sentence and say what would count as an answer.", new(DispatchState.Briefed, 3301, MoveKind.Nominal))])
    { Spec = ["K30", "K22"] };

    public static readonly Claim Families = new("Each source family is marked in or out, with a reason.", [Actor.Research],
        [new(Skill.Research, "Mark each source family in or out, with a reason.", new(ResearchPart.Families, 3302))])
    { Spec = ["K30", "K22"] };

    public static readonly Claim DefaultBranch = new("Governing docs are read on the default branch too, because a branch can lag.", [Actor.Research],
        [new(Skill.Research, "Read governing docs on the default branch too; a branch can lag.", new(ResearchPart.Families, 3303))])
    { Spec = ["K22"] };

    public static readonly Claim Scouts = new("Research sends one scout per family in play from the scout template, naming its skills (K14).", [Actor.Research],
        [new(Skill.Research, "Send one scout per family in play, in parallel, from the scout brief below, naming its skills with a slash; read a lone family yourself.", new(ResearchPart.Families, 3304))])
    { Spec = ["K30", "K14"] };

    public static readonly Claim Verify = new("Research opens the passage behind every load-bearing claim.", [Actor.Research],
        [new(Skill.Research, "Open the passage behind every load-bearing claim and confirm it says what the scout said.", new(DispatchState.Working, 3305, MoveKind.Nominal))])
    { Spec = ["K30"] };

    public static readonly Claim Conflicts = new("Conflicts are named with the source that governs; silence is marked unsettled.", [Actor.Research],
        [new(Skill.Research, "Where sources conflict, name the conflict and the source that governs; where they are silent, mark the point unsettled rather than reason your way to a fact.", new(DispatchState.Working, 3306, MoveKind.Nominal))])
    { Spec = ["K30", "K22"] };

    public static readonly Claim Write = new("The answer comes first, each claim with its citation, then what stayed unsettled and every family searched.", [Actor.Research],
        [new(Skill.Research, "Write the answer first, then each claim with its citation (a path at its commit, or a URL) and its exact passage, what stayed unsettled, and every family searched, the empty ones included.", new(DispatchState.Working, 3307, MoveKind.Nominal))])
    { Spec = ["K30", "K22"] };
}

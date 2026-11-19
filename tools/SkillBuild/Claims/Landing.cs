namespace DynaDocs.SkillBuild;

/// <summary>The landing (M2): inquisition rounds from zero through their phases, held by the admiral, the landing prerequisites, the merge review, his walkthrough, the merge and the tag.</summary>
public static class Landing
{
    private const string Areas = "areas' \\in 1..2";

    public static readonly Claim Staff = new("The admiral staffs each inquisition round from the roster and dydo usage.", [Actor.Admiral],
        [new(Skill.Admiral, "Staff each inquisition round from the model roster and `dydo usage`, and tell him in your next envoy bundle.", new(LandingState.Integrated, 301, MoveKind.Exit))])
    { Spec = ["N5", "R13", "R14"], Transition = new(LandingState.Integrated, LandingState.Mapped, "the admiral staffs a round; a fresh area map") { Effect = Areas } };

    public static readonly Claim MapDrawn = new("Each area's phases start with Read.", [Actor.Reviewer], [])
    { Spec = ["N5"], Transition = new(LandingState.Mapped, LandingState.ReadPhase, "an area's Read phase starts") };

    public static readonly Claim ReadDone = new("Read is followed by Break, by default.", [Actor.Reviewer], [])
    { Spec = ["N5"], Transition = new(LandingState.ReadPhase, LandingState.BreakPhase, "Read done; Break, the default next phase") };

    public static readonly Claim BreakDone = new("Break is followed by Drive, by default.", [Actor.Reviewer], [])
    { Spec = ["N5"], Transition = new(LandingState.BreakPhase, LandingState.DrivePhase, "Break done; Drive, the default next phase") };

    public static readonly Claim NextArea = new("Every area runs its phases.", [Actor.Reviewer], [])
    {
        Spec = ["N5"],
        Transition = new(LandingState.DrivePhase, LandingState.ReadPhase, "Drive done; the next area's Read phase") { Guard = "areas > 1", Effect = "areas' = areas - 1" }
    };

    public static readonly Claim AllAreas = new("Once every area has run its phases, the round has its verdict.", [Actor.Admiral], [])
    { Spec = ["N5", "S8"], Transition = new(LandingState.DrivePhase, LandingState.Inquisition, "every area through its phases: the round's verdict") { Guard = "areas = 1" } };

    public static readonly Claim RoundFound = new("The admiral holds the round it staffs: one verdict, blockers into Fix rN, shoulds registered.", [Actor.Admiral],
        [new(Skill.Admiral, "Hold each round you staff: post one verdict naming each seat and its findings, turn its blockers into Fix rN and register its shoulds.", new(LandingState.Inquisition, 302, MoveKind.Exit))])
    {
        Spec = ["N5", "S8", "S9", "S17"],
        Transition = new(LandingState.Inquisition, LandingState.RoundFixes, "the round finds a blocker or a should") { Effect = "fixFrom' = \"inquisition\"" }
    };

    public static readonly Claim NextRound = new("After the fixes, the next round starts from zero.", [Actor.Admiral], [])
    {
        Spec = ["N5"],
        Transition = new(LandingState.RoundFixes, LandingState.Mapped, "the fixes are merged; the next round from zero, a fresh area map")
            { Guard = "fixFrom = \"inquisition\"", Effect = "round' = round + 1 /\\ " + Areas }
    };

    public static readonly Claim NextMergeRound = new("A landing merge review with a blocker goes Fix, then a full-lens merge-review round, the security lens first after a security hole.", [Actor.Admiral], [])
    {
        Spec = ["R8", "R9"],
        Transition = new(LandingState.RoundFixes, LandingState.MergeReview, "the fixes are merged; a full-lens merge-review round, the security lens first after a security hole") { Guard = "fixFrom = \"merge\"" }
    };

    public static readonly Claim CleanRound = new("The Project lands only after a round with no blocker and no should.", [Actor.Admiral],
        [new(Skill.Admiral, "The Project lands only after an inquisition round that finds no blocker and no should.", new(LandingState.Inquisition, 304, MoveKind.Exit))])
    { Spec = ["N5"], Transition = new(LandingState.Inquisition, LandingState.Prerequisites, "a round finds no blocker and no should") };

    public static readonly Claim PrerequisiteMet = new("Each landing prerequisite is met in turn.", [Actor.Admiral, Actor.Balazs], [])
    {
        Spec = ["N7", "S18", "T12", "N15", "T8", "N14", "Y2", "Y6", "L6", "L12", "N8", "N9"],
        Transition = new(LandingState.Prerequisites, LandingState.Prerequisites,
            "a prerequisite met: shoulds closed, mutation clean, purge done, evidence rewritten on his click, DYTEST wired in, principle descriptions written, backfill page passed, law shipped, Upgrade note and re-measure, statuses Enablement")
            { Guard = "prereqs < 10", Effect = "prereqs' = prereqs + 1" }
    };

    public static readonly Claim Ready = new("The landing merge review waits for every landing prerequisite.", [Actor.Admiral],
        [new(Skill.Admiral, "Open the landing merge review only once every should is closed and the mutation check on the Project's diff is clean.", new(LandingState.Prerequisites, 305, MoveKind.Exit))])
    {
        Spec = ["N7", "S18", "T12", "N15"],
        Transition = new(LandingState.Prerequisites, LandingState.MergeReview, "every landing prerequisite met") { Guard = "prereqs = 10" }
    };

    public static readonly Claim Confident = new("On the Merge-confidence PASS, the landing candidate goes to his walkthrough.", [Actor.Admiral],
        [new(Skill.Admiral, "On the council's PASS, send the walkthrough of the landing candidate, the feature branch with master merged in, to a fresh envoy.", new(LandingState.MergeReview, 306, MoveKind.Exit))])
    { Spec = ["N6", "L16", "R20", "H22"], Transition = new(LandingState.MergeReview, LandingState.Walkthrough, "the Merge-confidence council passes the landing candidate") };

    public static readonly Claim ReviewFail = new("A broken or against-spec blocker from the landing merge review goes to Fix, then the next round.", [Actor.Admiral],
    [
        new(Skill.Admiral, "Route a landing merge-review blocker by kind: broken or against the spec to Fix, then the next round; a security hole to Fix, then the next round with the security lens first; fundamental or a new requirement to the officer above to re-plan; one that needs him up the chain at once.",
            new(LandingState.MergeReview, 307, MoveKind.Exit), () => [ReviewSecurity!, ReviewFundamental!, ReviewNeedsHim!])
    ])
    {
        Spec = ["R9", "R11"],
        Transition = new(LandingState.MergeReview, LandingState.RoundFixes, "a blocker is broken or against the spec") { Effect = "fixFrom' = \"merge\"" }
    };

    public static readonly Claim ReviewSecurity = new("A security hole from the landing merge review goes to Fix, then a round with the security lens first.", [Actor.Admiral], [])
    {
        Spec = ["R9"],
        Transition = new(LandingState.MergeReview, LandingState.RoundFixes, "a blocker is a security hole") { Effect = "fixFrom' = \"merge\" /\\ securityFirst' = TRUE" }
    };

    public static readonly Claim ReviewFundamental = new("A fundamental landing merge-review blocker, or a new requirement, goes to the officer above to re-plan.", [Actor.Admiral], [])
    { Spec = ["R9"], Transition = new(LandingState.MergeReview, LandingState.Replanned, "a blocker is fundamental, or a new requirement") };

    public static readonly Claim ReviewNeedsHim = new("A landing merge-review blocker that needs Balazs goes up the chain at once.", [Actor.Admiral], [])
    { Spec = ["R9"], Transition = new(LandingState.MergeReview, LandingState.Escalated, "a blocker needs Balazs") };

    public static readonly Claim Accepted = new("He accepts the landing candidate, then it merges.", [Actor.Balazs, Actor.Admiral],
        [new(Skill.Admiral, "When he accepts the landing candidate, it merges: AFK when two-way, on his click when one-way.", new(LandingState.Walkthrough, 308, MoveKind.Exit))])
    { Spec = ["N6"], Transition = new(LandingState.Walkthrough, LandingState.Merging, "Balazs accepts the landing candidate") };

    public static readonly Claim Merged = new("The landing PR merges into master.", [Actor.DydoMerge, Actor.Balazs], [])
    { Spec = ["N6", "R26"], Transition = new(LandingState.Merging, LandingState.Landed, "the landing PR is in master") };

    public static readonly Claim Hardened = new("The tag waits for the Guard hardening Project to land, every family.", [Actor.Admiral], [])
    { Spec = ["N2"], Transition = new(LandingState.Landed, LandingState.Landed, "the Guard hardening Project lands") { Guard = "~hardened", Effect = "hardened' = TRUE" } };

    public static readonly Claim Tag = new("He pushes the release tag once guard hardening has landed and mutation is clean.", [Actor.Balazs], [])
    { Spec = ["R28", "N2", "N15"], Transition = new(LandingState.Landed, LandingState.Tagged, "Balazs pushes the release tag") { Guard = "hardened" } };

    public static readonly Claim SpecGone = new("At acceptance, the spec's lasting parts move and the spec and plan files are deleted.", [Actor.Admiral],
        [new(Skill.Admiral, "Once he accepts, move the spec's lasting parts to DRs, the glossary, docs and the verification map, then delete the spec and the plan files.", new(LandingState.Merging, 311, MoveKind.Nominal))])
    { Spec = ["S14", "D11", "N7"] };

    public static readonly Claim TagIsHis = new("Release tags are his.", [Actor.Admiral],
        [new(Skill.Admiral, "The release tag is his to push.", new(LandingState.Landed, 312, MoveKind.Nominal))])
    { Spec = ["R28", "N2"] };
}

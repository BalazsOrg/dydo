namespace DynaDocs.SkillBuild;

/// <summary>
/// The landing (M2): inquisition rounds from zero, each held by its root inquisition captain through the
/// slices' Read, Break and Drive lenses; the landing prerequisites, the merge review, his walkthrough and
/// its route when he does not accept, the merge and the tag.
/// </summary>
public static class Landing
{
    private const string Areas = "areas' \\in 1..2";

    public static readonly Claim Staff = new(
        "The coordinator opens the Inquisition ticket; its root captain staffs the round from the roster and the quotas and tells him as an FYI on the HITL queue (DYD-1195 6deea8c0).",
        [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Once every delivery ticket is merged, open the Inquisition ticket for a fresh root inquisition captain.", new(LandingState.Integrated, 301, MoveKind.Exit)),
        new(Skill.TicketCaptain, "As the root, staff the round yourself: the provider, the models and the swarm width, from the model roster and `dydo usage`; post the staffing on the coordinator's HITL queue as an FYI for him.", new(LandingState.Mapped, 301, MoveKind.Arrival))
    ])
    { Spec = ["N5", "H5", "R13", "R14"], Transition = new(LandingState.Integrated, LandingState.Mapped, "the Inquisition ticket opens; its root captain staffs a round and draws a fresh feature map") { Effect = Areas } };

    public static readonly Claim MapDrawn = new("Each slice's lenses start with Read, its own sub-agent.", [Actor.Reviewer], [])
    { Spec = ["N5", "R34"], Transition = new(LandingState.Mapped, LandingState.ReadPhase, "a slice's Read lens starts") };

    public static readonly Claim ReadDone = new("Read is followed by Break, run by a separate sub-agent (DYD-1082 8239d5b6).", [Actor.Reviewer], [])
    { Spec = ["N5", "R34"], Transition = new(LandingState.ReadPhase, LandingState.BreakPhase, "Read done; Break, a separate sub-agent") };

    public static readonly Claim BreakDone = new("Break is followed by Drive, run by a separate sub-agent (DYD-1082 8239d5b6).", [Actor.Reviewer], [])
    { Spec = ["N5", "R34"], Transition = new(LandingState.BreakPhase, LandingState.DrivePhase, "Break done; Drive, a separate sub-agent") };

    public static readonly Claim NextSlice = new("Every slice runs its lenses.", [Actor.Reviewer], [])
    {
        Spec = ["N5"],
        Transition = new(LandingState.DrivePhase, LandingState.ReadPhase, "Drive done; the next slice's Read lens") { Guard = "areas > 1", Effect = "areas' = areas - 1" }
    };

    public static readonly Claim AllSlices = new("Once every slice has run its lenses, the round has its verdict.", [Actor.TicketCaptain], [])
    { Spec = ["N5", "S8"], Transition = new(LandingState.DrivePhase, LandingState.Inquisition, "every slice through its lenses: the round's verdict") { Guard = "areas = 1" } };

    public static readonly Claim RoundFound = new("The root inquisition captain holds the round: one verdict stitched from the groups, blockers into Fix rN, shoulds registered, nits on the round's nit ticket.",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Hold the round: post one verdict, stitched from the groups' results, naming each seat and its findings; turn its blockers into Fix rN, register its shoulds and put its nits on the round's nit ticket.", new(LandingState.Inquisition, 302, MoveKind.Exit))])
    {
        Spec = ["N5", "S8", "S9", "S11", "S17"],
        Transition = new(LandingState.Inquisition, LandingState.RoundFixes, "the round finds a blocker or a should") { Effect = "fixFrom' = \"inquisition\"" }
    };

    public static readonly Claim NextRound = new("After the fixes, the next round starts from zero.", [Actor.Coordinator], [])
    {
        Spec = ["N5"],
        Transition = new(LandingState.RoundFixes, LandingState.Mapped, "the fixes are merged; the next round from zero, a fresh feature map")
            { Guard = "fixFrom = \"inquisition\"", Effect = "round' = round + 1 /\\ " + Areas }
    };

    public static readonly Claim NextMergeRound = new("A landing merge review with a blocker goes Fix, then a full-lens merge-review round, the security lens first after a security hole.", [Actor.Coordinator], [])
    {
        Spec = ["R8", "R9"],
        Transition = new(LandingState.RoundFixes, LandingState.MergeReview, "the fixes are merged; a full-lens merge-review round, the security lens first after a security hole") { Guard = "fixFrom = \"merge\"" }
    };

    public static readonly Claim CleanRound = new("The Project lands only after a round with no blocker and no should.", [Actor.Coordinator],
        [new(Skill.Coordinator, "The Project lands only after an inquisition round that finds no blocker and no should.", new(LandingState.Inquisition, 304, MoveKind.Exit))])
    { Spec = ["N5"], Transition = new(LandingState.Inquisition, LandingState.Prerequisites, "a round finds no blocker and no should") };

    public static readonly Claim PrerequisiteMet = new("Each landing prerequisite is met in turn.", [Actor.Coordinator, Actor.Balazs], [])
    {
        Spec = ["N7", "S18", "T12", "N15", "T8", "N14", "Y2", "Y6", "L6", "L12", "N8", "N9"],
        Transition = new(LandingState.Prerequisites, LandingState.Prerequisites,
            "a prerequisite met: shoulds closed, mutation clean, purge done, evidence rewritten on his click, DYTEST wired in, principle descriptions written, backfill page passed, law shipped, Upgrade note and re-measure, statuses Enablement")
            { Guard = "prereqs < 10", Effect = "prereqs' = prereqs + 1" }
    };

    public static readonly Claim Ready = new("The landing merge review waits for every landing prerequisite.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Open the landing merge review only once every should is closed and the mutation check on the Project's diff is clean.", new(LandingState.Prerequisites, 305, MoveKind.Exit))])
    {
        Spec = ["N7", "S18", "T12", "N15"],
        Transition = new(LandingState.Prerequisites, LandingState.MergeReview, "every landing prerequisite met") { Guard = "prereqs = 10" }
    };

    public static readonly Claim Confident = new("On the Merge-confidence PASS, the landing candidate goes to his walkthrough.", [Actor.Coordinator],
        [new(Skill.Coordinator, "On the council's PASS, send the walkthrough of the landing candidate, the feature branch with master merged in, to a fresh envoy.", new(LandingState.MergeReview, 306, MoveKind.Exit))])
    { Spec = ["N6", "L16", "R20", "H22"], Transition = new(LandingState.MergeReview, LandingState.Walkthrough, "the Merge-confidence council passes the landing candidate") };

    public static readonly Claim ReviewFail = new("A broken or against-spec blocker from the landing merge review goes to Fix, then the next round.", [Actor.Coordinator],
    [
        new(Skill.Coordinator, "Route a landing merge-review blocker by kind: broken or against the spec to Fix, then the next round; a security hole to Fix, then the next round with the security lens first; fundamental or a new requirement to the officer above to re-plan; one that needs him up the chain at once.",
            new(LandingState.MergeReview, 307, MoveKind.Exit), () => [ReviewSecurity!, ReviewFundamental!, ReviewNeedsHim!])
    ])
    {
        Spec = ["R9", "R11"],
        Transition = new(LandingState.MergeReview, LandingState.RoundFixes, "a blocker is broken or against the spec") { Effect = "fixFrom' = \"merge\"" }
    };

    public static readonly Claim ReviewSecurity = new("A security hole from the landing merge review goes to Fix, then a round with the security lens first.", [Actor.Coordinator], [])
    {
        Spec = ["R9"],
        Transition = new(LandingState.MergeReview, LandingState.RoundFixes, "a blocker is a security hole") { Effect = "fixFrom' = \"merge\" /\\ securityFirst' = TRUE" }
    };

    public static readonly Claim ReviewFundamental = new("A fundamental landing merge-review blocker, or a new requirement, goes to the officer above to re-plan.", [Actor.Coordinator], [])
    { Spec = ["R9"], Transition = new(LandingState.MergeReview, LandingState.Replanned, "a blocker is fundamental, or a new requirement") };

    public static readonly Claim ReviewNeedsHim = new("A landing merge-review blocker that needs Balazs goes up the chain at once.", [Actor.Coordinator], [])
    { Spec = ["R9"], Transition = new(LandingState.MergeReview, LandingState.Escalated, "a blocker needs Balazs") };

    public static readonly Claim Accepted = new("He accepts the landing candidate, then it merges.", [Actor.Balazs, Actor.Coordinator],
        [new(Skill.Coordinator, "When he accepts the landing candidate, it merges: AFK when two-way, on his click when one-way.", new(LandingState.Walkthrough, 308, MoveKind.Exit))])
    { Spec = ["N6"], Transition = new(LandingState.Walkthrough, LandingState.Merging, "Balazs accepts the landing candidate") };

    public static readonly Claim NotAccepted = new("A landing walkthrough he does not accept files its findings as tickets in the same Project (DYD-1082 555a8300 gap 5).",
        [Actor.Balazs, Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When he does not accept the landing walkthrough, file each of his findings as a ticket in the same Project.", new(LandingState.Walkthrough, 309, MoveKind.Exit))])
    { Spec = ["N6"], Transition = new(LandingState.Walkthrough, LandingState.NotAccepted, "he does not accept: his findings become tickets in the same Project") };

    public static readonly Claim ReCut = new("After a landing walkthrough not accepted, a fix captain re-cuts the branch and another landing follows (DYD-1082 555a8300 gap 5).",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As its fix captain, re-cut the landing branch once the findings' tickets are merged; another landing follows.", new(LandingState.NotAccepted, 310, MoveKind.Exit))])
    { Spec = ["N6"], Transition = new(LandingState.NotAccepted, LandingState.Integrated, "a fix captain re-cuts the branch; another landing follows") };

    public static readonly Claim Merged = new("The landing PR merges into master.", [Actor.DydoMerge, Actor.Balazs], [])
    { Spec = ["N6", "R26"], Transition = new(LandingState.Merging, LandingState.Landed, "the landing PR is in master") };

    public static readonly Claim Hardened = new("The tag waits for the Guard hardening Project to land, every family.", [Actor.Coordinator], [])
    { Spec = ["N2"], Transition = new(LandingState.Landed, LandingState.Landed, "the Guard hardening Project lands") { Guard = "~hardened", Effect = "hardened' = TRUE" } };

    public static readonly Claim Tag = new("He pushes the release tag once guard hardening has landed and mutation is clean.", [Actor.Balazs], [])
    { Spec = ["R28", "N2", "N15"], Transition = new(LandingState.Landed, LandingState.Tagged, "Balazs pushes the release tag") { Guard = "hardened" } };

    public static readonly Claim SpecGone = new("At acceptance, the Wayfinding captain moves the spec's lasting parts and deletes the spec and plan files; the coordinator only opens its ticket.",
        [Actor.Coordinator, Actor.TicketCaptain],
    [
        new(Skill.Coordinator, "Once he accepts, open a Wayfinding ticket to move the spec's lasting parts and delete the spec and the plan files.", new(LandingState.Merging, 311, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "At the landing, once he accepts, move the spec's lasting parts to DRs, the glossary, docs and the verification map, then delete the spec and the plan files.", new(TicketCaptainSlot.Wayfinding, 3120))
    ])
    { Spec = ["S14", "D11", "N7", "H4", "H5"] };

    public static readonly Claim TagIsHis = new("Release tags are his.", [Actor.Coordinator],
        [new(Skill.Coordinator, "The release tag is his to push.", new(LandingState.Landed, 312, MoveKind.Nominal))])
    { Spec = ["R28", "N2"] };
}

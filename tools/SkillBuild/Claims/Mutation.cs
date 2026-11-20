namespace DynaDocs.SkillBuild;

/// <summary>The landing's mutation check (M18): queued for the exact tree, run like a test run, survivors fixed or claimed equivalent and approved one at a time.</summary>
public static class Mutation
{
    public static readonly Claim LandingCheck = new("The landing check queues mutation work missing or stale for the exact tree it judges.", [Actor.Dytest], [])
    {
        Spec = ["T12", "T13"],
        Transition = new(MutationState.Background, MutationState.Queued, "the landing check judges the Project's diff") { Effect = "cloudRouteNotMuchMoreWork' \\in BOOLEAN" }
    };

    public static readonly Claim Schedule = new("Landing mutation work is scheduled exactly like a test run, on local machines and peers.", [Actor.Dytest], [])
    { Spec = ["T12"], Transition = new(MutationState.Queued, MutationState.Running, "scheduled like a test run, at normal priority") };

    public static readonly Claim Cloud = new("Under a large queue it spills to the cloud only if that route is not much more work than the test-run one.", [Actor.Dytest], [])
    {
        Spec = ["T12"],
        Transition = new(MutationState.Queued, MutationState.Running, "a large queue spills to the cloud") { Guard = "cloudRouteNotMuchMoreWork" }
    };

    public static readonly Claim Survive = new("A mutant survives in the Project's changed code.", [Actor.Dytest], [])
    { Spec = ["T12"], Transition = new(MutationState.Running, MutationState.Survivors, "a mutant survives") };

    public static readonly Claim AllKilled = new("No survivor is left without a fix or an approved equivalent.", [Actor.Dytest], [])
    { Spec = ["T12", "N15"], Transition = new(MutationState.Running, MutationState.Clean, "no survivor without a fix or an approved equivalent") };

    public static readonly Claim CutFix = new("The coordinator holds the landing and cuts each survivor's fix ticket for a captain.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Open a fix ticket for each landing survivor and put a captain on it.", new(MutationState.Survivors, 1606, MoveKind.Exit))])
    { Spec = ["N6", "H5", "T12"], Transition = new(MutationState.Survivors, MutationState.Ticketed, "the coordinator cuts the survivor's fix ticket") };

    // T12: a fix changes the tree, so the work for the new tree is stale until it is queued again.
    public static readonly Claim Fix = new("A survivor is fixed, and the changed tree's mutation work is queued again.", [Actor.Implementer],
        [new(Skill.Implementer, "Fix a surviving mutant: sharpen the test that missed it, or delete the code it shows nothing needs.", new(MutationState.Ticketed, 1607, MoveKind.Exit))])
    { Spec = ["T12"], Transition = new(MutationState.Ticketed, MutationState.Queued, "fixed; the changed tree's work is queued again") };

    public static readonly Claim ClaimEquivalent = new("The author claims a survivor equivalent, with a reason, and never approves its own claim.", [Actor.Implementer],
        [new(Skill.Implementer, "Or claim it equivalent, with a reason; a fresh reviewer decides the claim, never you.", new(MutationState.Ticketed, 1608, MoveKind.Exit))])
    { Spec = ["T12"], Transition = new(MutationState.Ticketed, MutationState.Claimed, "claimed equivalent, with a reason") };

    public static readonly Claim Approve = new("A fresh independent reviewer approves an equivalent one at a time, bound to the mutant and its code.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Approve an equivalent mutant one at a time, with a reason, only if you wrote neither the code nor its tests; your approval binds that mutant and its surrounding code.", new(MutationState.Claimed, 1609, MoveKind.Exit))])
    { Spec = ["T12"], Transition = new(MutationState.Claimed, MutationState.Clean, "a fresh independent reviewer approves it") };

    public static readonly Claim Rejected = new("A rejected claim the author accepts leaves the survivor to fix.", [Actor.Reviewer, Actor.Implementer], [])
    { Spec = ["T12"], Transition = new(MutationState.Claimed, MutationState.Ticketed, "rejected, and the author accepts: back to fixing") };

    public static readonly Claim Reject = new("A claim disputed between author and reviewer goes to a council.", [Actor.Reviewer, Actor.Implementer],
        [new(Skill.Reviewer, "A rejected claim the author disputes goes to a council.", new(MutationState.Claimed, 1610, MoveKind.Exit))])
    { Spec = ["T12", "L16"], Transition = new(MutationState.Claimed, MutationState.Dispute, "rejected, and the author disputes it: a council") };

    public static readonly Claim Ruled = new("The council rules the mutant equivalent.", [Actor.Reviewer], [])
    { Spec = ["T12", "L14"], Transition = new(MutationState.Dispute, MutationState.Clean, "the council rules it equivalent") };

    public static readonly Claim RuledNot = new("The council rules the mutant a real survivor; it gets a fix.", [Actor.Reviewer], [])
    { Spec = ["T12", "L14"], Transition = new(MutationState.Dispute, MutationState.Ticketed, "the council rules it a real survivor") };

    public static readonly Claim Lapse = new("An approval lapses when the mutant or its surrounding code changes.", [Actor.Dytest], [])
    { Spec = ["T12"], Transition = new(MutationState.Clean, MutationState.Queued, "the mutant or its code changes: the approval lapses") };

    public static readonly Claim Totals = new("He sees the totals of approved equivalents per feature.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Show him the per-feature totals of approved equivalent mutants.", new(LandingState.Prerequisites, 1614, MoveKind.Nominal))])
    { Spec = ["T12"] };
}

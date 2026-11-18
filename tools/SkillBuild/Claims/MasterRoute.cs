namespace DynaDocs.SkillBuild;

/// <summary>A PR into master (M8): two-way AFK by its captain, one-way on his click, paced one at a time by the admiral.</summary>
public static class MasterRoute
{
    public static readonly Claim TwoWay = new("A two-way PR into master merges AFK by its captain once its evidence is complete.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Declare the PR two-way in its body when it is none of the one-way kinds and no path flags it; there is no objection window.", new(MasterRouteState.Offered, 701, MoveKind.Exit)),
        new(Skill.WorkingTreeStandard, "A two-way PR into master merges AFK, by its captain, once it has a PASS on the head or an ancestor, gates green on the combined tree with current master, CI green, and a body declaring it two-way.", new(Section.TreeMerging, 7))
    ])
    { Spec = ["R26", "R27"], Transition = new(MasterRouteState.Offered, MasterRouteState.Paced, "declared two-way in the PR body") };

    public static readonly Claim OneWay = new("A one-way PR carries a recovery plan and waits on his click.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A one-way PR carries a recovery plan and waits on his click; declare it yourself when no path flags it.", new(MasterRouteState.Offered, 702, MoveKind.Exit))])
    { Spec = ["R27"], Transition = new(MasterRouteState.Offered, MasterRouteState.CiWatch, "one-way: a flagged path, or declared by its captain") };

    public static readonly Claim Turn = new("Main-bound merges land one at a time; each re-checks against current master just before landing.", [Actor.Admiral, Actor.TicketCaptain],
    [
        new(Skill.Admiral, "Land main-bound merges one at a time; each re-checks against current master just before landing.", new(MasterRouteState.Paced, 703, MoveKind.Exit)),
        new(Skill.TicketCaptain, "On your turn, re-sync with current master and re-check.", new(MasterRouteState.Resync, 703, MoveKind.Arrival)),
        new(Skill.WorkingTreeStandard, "Main-bound merges land one at a time, paced by the admiral; each re-checks against current master just before landing, and a PR forced to re-sync a third time is held while the admiral sequences the colliding work.",
            new(Section.TreeMerging, 8), () => [Hold!])
    ])
    { Spec = ["R29"], Transition = new(MasterRouteState.Paced, MasterRouteState.Resync, "the admiral gives it its turn") };

    public static readonly Claim Merge = new("The captain merges once the re-check is green.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Merge it yourself with `dydo merge` once the re-check is green.", new(MasterRouteState.Resync, 704, MoveKind.Exit))])
    { Spec = ["R26", "R29"], Transition = new(MasterRouteState.Resync, MasterRouteState.AfkMerge, "re-check green: PASS, gates green with current master, CI green") };

    public static readonly Claim Again = new("Master moved: the PR re-syncs again.", [Actor.TicketCaptain], [])
    {
        Spec = ["R29"],
        Transition = new(MasterRouteState.Resync, MasterRouteState.Paced, "master moved: re-sync again") { Guard = "resyncs < 2", Effect = "resyncs' = resyncs + 1" }
    };

    public static readonly Claim Hold = new("A PR forced to re-sync a third time is held while the admiral sequences the colliding work.", [Actor.Admiral, Actor.TicketCaptain],
    [
        new(Skill.Admiral, "Hold a PR forced to re-sync a third time and sequence the colliding work.", new(MasterRouteState.Held, 706, MoveKind.Arrival)),
        new(Skill.TicketCaptain, "A third forced re-sync holds your PR until the admiral sequences the colliding work.", new(MasterRouteState.Held, 706, MoveKind.Arrival))
    ])
    {
        Spec = ["R29"],
        Transition = new(MasterRouteState.Resync, MasterRouteState.Held, "forced to re-sync a third time") { Guard = "resyncs = 2", Effect = "resyncs' = 3" }
    };

    public static readonly Claim Sequenced = new("The admiral releases a held PR once the colliding work is sequenced.", [Actor.Admiral],
        [new(Skill.Admiral, "Release the held PR once the colliding work is sequenced.", new(MasterRouteState.Held, 707, MoveKind.Exit))])
    { Spec = ["R29"], Transition = new(MasterRouteState.Held, MasterRouteState.Paced, "the colliding work is sequenced") { Effect = "resyncs' = 0" } };

    public static readonly Claim AfkDone = new("dydo merge merges the two-way PR into master.", [Actor.DydoMerge], [])
    { Spec = ["R26"], Transition = new(MasterRouteState.AfkMerge, MasterRouteState.Merged, "dydo merge merges into master") };

    public static readonly Claim CiGreen = new("Before his click, the captain watches its own CI and opens the HITL Merge only when it is green.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Watch your own CI and open the HITL Merge only when it is green.", new(MasterRouteState.CiWatch, 709, MoveKind.Exit))])
    { Spec = ["R31", "S12"], Transition = new(MasterRouteState.CiWatch, MasterRouteState.HitlMerge, "its CI is green") };

    public static readonly Claim Click = new("He clicks the one-way merge.", [Actor.Balazs], [])
    { Spec = ["R27", "R31"], Transition = new(MasterRouteState.HitlMerge, MasterRouteState.Merged, "Balazs clicks the merge") };

    public static readonly Claim Woken = new("The survey wakes the captain on a merged PR with its HITL Merge open; the hop closes on green.", [Actor.Survey, Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When the survey wakes you on `PR merged, HITL Merge open`, or your AFK merge lands, close the Merge hop once the merge tree's receipt and CI are green.", new(MasterRouteState.Merged, 711, MoveKind.Exit))])
    { Spec = ["R20", "R31"], Transition = new(MasterRouteState.Merged, MasterRouteState.Closed, "receipt and CI green on the merge") };

    public static readonly Claim RealRed = new("A real red after a master merge is reverted at once and becomes a CI-findings ticket.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A real red after a master merge is reverted at once and gets a CI-findings ticket, as after any merge.", new(MasterRouteState.Merged, 712, MoveKind.Exit))])
    { Spec = ["R22", "Y9"], Transition = new(MasterRouteState.Merged, MasterRouteState.Reverted, "a real red after the merge") };

    public static readonly Claim Reland = new("After the fix and its round, the PR is offered again.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "After the fix and its fix-scoped round, offer the PR again.", new(MasterRouteState.Reverted, 713, MoveKind.Exit))])
    { Spec = ["R22"], Transition = new(MasterRouteState.Reverted, MasterRouteState.Offered, "fixed, fix-scoped round passed, offered again") };

    public static readonly Claim OneWayKinds = new("The one-way kinds of PR into master.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "One-way kinds: data migrations; publishing a package or a public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main (CI, branch protection, release workflows, the guard).", new(MasterRouteState.Offered, 714, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "A one-way PR, one whose revert doesn't undo its effect, needs his click and carries a recovery plan. One-way kinds: data migrations; publishing a package or a public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main (CI, branch protection, release workflows, the guard).", new(Section.TreeMerging, 6))
    ])
    { Spec = ["R27"] };
}

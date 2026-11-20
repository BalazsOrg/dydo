namespace DynaDocs.SkillBuild;

/// <summary>
/// The inquisition (N5, R34): review at the landing level, conducted by a ticket captain with the
/// inquisition-captain resource (DYD-1082 change 5, 01266a65, 99774812, 8239d5b6), and the reviewer's lens
/// on its slice.
/// </summary>
public static class Inquisition
{
    private const string Captain = "[the inquisition captain](resources/inquisition-captain.md)";

    public static readonly Claim Conduct = new("A ticket captain conducts an inquisition with the inquisition-captain resource (DYD-1082 1bc04dce change 5).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, $"Holding an Inquisition ticket or one of its sub-tickets, conduct it by {Captain}.", new(TicketCaptainSlot.Take, 121))])
    { Spec = ["N5", "K22"] };

    public static readonly Claim Level = new("The inquisition is review at the landing level, cut by feature and staffed with reviewers (DYD-1082 01266a65).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "An inquisition is review at the landing level: the candidate as a whole, cut by feature and staffed with reviewers who inspect everything and test every hypothesis.", new(TicketCaptainSlot.Inquisition, 2902))])
    { Spec = ["N5", "R34"] };

    public static readonly Claim Zero = new("Every inquisition round starts from zero.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Every round starts from zero: a fresh feature map, never an earlier round's map or findings.", new(TicketCaptainSlot.Inquisition, 2903))])
    { Spec = ["N5"] };

    public static readonly Claim Map = new("The root inquisition captain draws the feature map: small slices, each seam a slice.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As the root, draw the feature map from the landed diff: one small slice per feature, and each seam where features meet a slice of its own; post it on the ticket.", new(TicketCaptainSlot.Inquisition, 2904))])
    { Spec = ["N5"] };

    public static readonly Claim Split = new("Over about 10 slices the root cuts group sub-tickets, each held by a sub-captain, and owns the cross-group seams (DYD-1082 99774812).",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Over about 10 slices, cut the map into group sub-tickets of up to about 10 slices, each held by a sub-captain; keep a seam inside a group with it, and keep the seams between groups as slices of your own.", new(TicketCaptainSlot.Inquisition, 2905))])
    { Spec = ["N5", "H13"] };

    public static readonly Claim Unsplit = new("When the map is not split, and for the root's own seam slices, the root opens a sub-ticket with its own captain.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When the map is not split, and for your own seam slices, open a sub-ticket with its own captain for those slices.", new(TicketCaptainSlot.Inquisition, 2906))])
    { Spec = ["N5"] };

    public static readonly Claim Root = new("The root never drives reviewers directly and stitches the groups' results (DYD-1082 99774812).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As the root, never dispatch a reviewer yourself: stitch the groups' results into the round's verdict.", new(TicketCaptainSlot.Inquisition, 2907))])
    { Spec = ["N5"] };

    public static readonly Claim Lenses = new("Read, Break and Drive run as separate sub-agents, in that order, never one agent for two (DYD-1082 8239d5b6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Holding a group, run Read, then Break, then Drive on each slice, each lens its own reviewer briefed from the slice reviewer brief below; never one agent for two lenses.", new(TicketCaptainSlot.Inquisition, 2908))])
    { Spec = ["N5", "R34"] };

    public static readonly Claim Route = new("Each return is routed: a new finding a row, a known one a sighting, one only a test or a drive settles to that lens.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Route each return: a new finding is a new row in the round's findings, a known one a sighting on its row, and one only a test or a drive can settle goes to that slice's Break or Drive seat.", new(TicketCaptainSlot.Inquisition, 2909))])
    { Spec = ["N5"] };

    public static readonly Claim SliceBrief = new("The inquisition-captain resource carries the slice reviewer brief (DYD-1082 1bc04dce change 5).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Skills: /reviewer · level: inquisition · the lens · the slice, its map row and its findings so far · the audit SHA and the landing base · the governing spec · worktree · model and effort · return to.", new(TicketCaptainSlot.SliceBrief, 2910))])
    { Spec = ["N5", "R34", "K14"] };

    public static readonly Claim Order = new("An inquisition seat runs the one lens its brief names on its slice, from zero.", [Actor.Reviewer],
        [new(Skill.Reviewer, "In an inquisition, run only the lens your brief names on your slice, from zero; a separate seat runs each other lens.", new(LandingState.ReadPhase, 2911, MoveKind.Nominal))])
    { Spec = ["N5", "R34"] };

    public static readonly Claim Read = new("The Read lens covers the three Read columns: local, against the spec, and the whole.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Read: read the slice locally, against the spec and as a whole, for what is broken in ways neither tests nor clicking would catch.", new(LandingState.ReadPhase, 2912, MoveKind.Nominal))])
    { Spec = ["N5", "R34"] };

    public static readonly Claim Break = new("The Break lens runs the tests, the hypothesis tests and the formal checks.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Break: run the suite, `--heavy` and mutation, test a hypothesis for every way the slice could fail, and run its formal checks where they are built.", new(LandingState.BreakPhase, 2913, MoveKind.Nominal))])
    { Spec = ["N5", "R34", "R7"] };

    public static readonly Claim Drive = new("The Drive lens drives the app against the spec's problem statement, where driving is built.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Drive: drive the slice's journeys through the running app, against the spec's problem statement, where a driving skill is built for it.", new(LandingState.DrivePhase, 2914, MoveKind.Nominal))])
    { Spec = ["N5", "R34", "R3"] };
}

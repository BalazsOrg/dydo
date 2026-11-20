namespace DynaDocs.SkillBuild;

/// <summary>
/// The inquisition (N5, R34): review at the landing level, conducted by a ticket captain with the
/// inquisition-captain resource (DYD-1082 change 5, 01266a65, 99774812, 8239d5b6), and the reviewer's lens
/// on its slice. DR 059's conducting rulings are the resource's base, as N12 carries them forward and N5
/// changes them: areas become slices, phases reviewer lenses, and the docs-writer an implementer.
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
    { Spec = ["N5", "N12 DR 059"] };

    public static readonly Claim NeverHunt = new("The inquisition's captain conducts and never hunts (DR 059 ruling 6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "You conduct and never hunt: reading the work for what is wrong is your reviewers' job.", new(TicketCaptainSlot.Inquisition, 2904))])
    { Spec = ["N12 DR 059", "N5"] };

    public static readonly Claim VerificationFirst = new("A verification skill comes first: a project with a runnable surface and none gets it before the round (DR 059 ruling 4).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As the root, check before the first round that the project has a verification skill: one with a runnable surface and none goes to your officer as a new requirement, and the round waits for the skill; a project with nothing to run needs none, and Drive does not apply.", new(TicketCaptainSlot.Inquisition, 2905))])
    { Spec = ["N12 DR 059", "N5", "R9", "R34"] };

    public static readonly Claim Map = new("The root inquisition captain draws the feature map: small slices, each seam a slice, each lens run or its reason (DR 059 ruling 1).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As the root, draw the feature map from the landed diff: one small slice per feature, and each seam where features meet a slice of its own; post it on the ticket with each slice's lenses, or the one-line reason a lens does not apply.", new(TicketCaptainSlot.InquisitionRound, 2910))])
    { Spec = ["N5", "N12 DR 059"] };

    public static readonly Claim Prose = new("A prose slice gets Read only; Break and Drive apply wherever the landing changed behaviour (DR 059 ruling 2).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A prose slice, a plain skill or doc, gets Read only, and a contradiction it finds is proved by quoting both passages; Break and Drive apply wherever the landing changed behaviour.", new(TicketCaptainSlot.InquisitionRound, 2911))])
    { Spec = ["N12 DR 059", "N5"] };

    public static readonly Claim Split = new("Over about 10 slices the root cuts group sub-tickets, each held by a sub-captain, and owns the cross-group seams (DYD-1082 99774812).",
        [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Over about 10 slices, cut the map into group sub-tickets of up to about 10 slices, each held by a sub-captain; keep a seam inside a group with it, and keep the seams between groups as slices of your own.", new(TicketCaptainSlot.InquisitionRound, 2912))])
    { Spec = ["N5", "H13"] };

    public static readonly Claim Unsplit = new("When the map is not split, and for the root's own seam slices, the root opens a sub-ticket with its own captain.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When the map is not split, and for your own seam slices, open a sub-ticket with its own captain for those slices.", new(TicketCaptainSlot.InquisitionRound, 2913))])
    { Spec = ["N5"] };

    public static readonly Claim Root = new("The root never drives slice reviewers directly and stitches the groups' results, joining duplicates across groups (DYD-1082 99774812, DR 059 ruling 6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As the root, never dispatch a slice's reviewer yourself: merge the groups' registers into yours, a finding seen in two groups becoming one finding with both sightings, and stitch the round's verdict from them.", new(TicketCaptainSlot.InquisitionRound, 2914))])
    { Spec = ["N5", "N12 DR 059"] };

    public static readonly Claim Register = new("The findings register is a file on the audit branch, pushed after every return (DR 059 ruling 6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Holding slices, keep the round's findings register on an `inquisition/<slug>` branch cut from the audit SHA, which never merges: one row per finding with its slice, severity, place, proof, sightings and disposition, pushed after every return and before the next dispatch.", new(TicketCaptainSlot.InquisitionRound, 2915))])
    { Spec = ["N12 DR 059", "N5"] };

    public static readonly Claim Lenses = new("Read, Break and Drive run as separate sub-agents, in that order, as a pipeline, never one agent for two (DYD-1082 8239d5b6, DR 059 ruling 3).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Run Read, then Break, then Drive on each slice, as a pipeline across slices rather than in waves, each lens its own reviewer briefed from the slice reviewer brief below; never one agent for two lenses.", new(TicketCaptainSlot.InquisitionRound, 2916))])
    { Spec = ["N5", "R34", "N12 DR 059"] };

    public static readonly Claim OneSeat = new("One agent per slice per lens; tiny slices are merged on the map, never packed into one dispatch (DR 059 ruling 5).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Send one reviewer per slice per lens: a dispatch never carries two slices, so merge tiny slices on the map instead.", new(TicketCaptainSlot.InquisitionRound, 2917))])
    { Spec = ["N12 DR 059", "N5"] };

    public static readonly Claim Route = new("A finding only a test or a drive settles goes to that lens, with a follow-up seat if the lens already ran (DR 059 ruling 6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Route a finding only a test or a drive can settle to that slice's Break or Drive seat, with a short follow-up seat when that lens already ran.", new(TicketCaptainSlot.InquisitionRound, 2918))])
    { Spec = ["N5", "N12 DR 059"] };

    public static readonly Claim Sightings = new("One bug seen in several places is one finding with several sightings (DR 059 ruling 6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A finding already in the register is a sighting on its row, never a new row: a bug read in the code and seen again while driving is one finding with two sightings.", new(TicketCaptainSlot.InquisitionRound, 2919))])
    { Spec = ["N12 DR 059", "N5"] };

    public static readonly Claim Severity = new("Each return's severities are checked against the source, not the reviewer's label (DR 059 ruling 6).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Check each returned severity against the source, not the reviewer's label, and record any change with its reason on the row.", new(TicketCaptainSlot.InquisitionRound, 2920))])
    { Spec = ["N12 DR 059", "N12 DR 058"] };

    public static readonly Claim Judgement = new("A finding that is a judgement call goes up as a need (DR 059 ruling 6, as H16 routes a need).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Pass a finding that needs a judgement call up as a need, like any other.", new(TicketCaptainSlot.InquisitionRound, 2921))])
    { Spec = ["N12 DR 059", "H16"] };

    public static readonly Claim Patch = new("A proven blocker or should with a well-defined fix gets its fix prepared mid-round on its own branch, for its ticket's captain to verify and review once (DR 059 ruling 7).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "A blocker or should a Break seat proved red, with a well-defined fix, gets the fix prepared mid-round: brief an implementer for a Bug on its own branch off the audit SHA, red test first, and at the verdict attach the branch and its SHA to the finding's ticket, whose captain verifies and reviews it once.", new(TicketCaptainSlot.InquisitionRound, 2922))])
    { Spec = ["N12 DR 059", "K31"] };

    public static readonly Claim PatchLimits = new("A well-defined fix is one obvious, small fix in one slice touching no contract, DR or public interface; nits are never patched (DR 059 ruling 7).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Well-defined means one obvious fix inside one slice, small, touching no contract, DR or public interface; any doubt means no patch, nits are never patched, and the inquisition merges nothing but its own record.", new(TicketCaptainSlot.InquisitionRound, 2923))])
    { Spec = ["N12 DR 059"] };

    // Review r2 R2-S4: the pre-candidate's proof branches and their retention (c916a9ca inquisition.md steps 6, 7
    // and 10), kept as DR 059 keeps them: "Audit and proof branches never merge, and each open Bug's reproduction
    // ref is retained as before."
    public static readonly Claim ProofBranch = new("Each Break seat proves on its own proof branch, cut from the audit SHA, which never merges (DR 059 ruling 8).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Brief each Break seat on its own proof branch, `proof/<KEY>-<slice>-<n>`, cut from the audit SHA with the test paths it may touch, `<n>` counting that slice's Break seats; a proof branch never merges.", new(TicketCaptainSlot.InquisitionRound, 2924))])
    { Spec = ["N12 DR 059", "N5", "K22"] };

    public static readonly Claim ProofPush = new("A Break seat's proof branch is pushed on its return; its red-test SHAs are the reproductions.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "When a Break seat returns, push its proof branch: its red-test SHAs are the findings' reproductions.", new(TicketCaptainSlot.InquisitionRound, 2925))])
    { Spec = ["N12 DR 059", "K22"] };

    public static readonly Claim ProofRetention = new("The captain holding the slices keeps each open finding's reproduction on a pushed ref until its ticket's captain adopts it (DR 059 ruling 8).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Holding slices, keep each open finding's red-test SHA reachable from a pushed ref apart from the audit branch, its proof branch or its patch branch, linked on its ticket, until that ticket's captain records adopting it; cleanup never deletes those refs.", new(TicketCaptainSlot.InquisitionRound, 2926))])
    { Spec = ["N12 DR 059", "K19", "K22"] };

    public static readonly Claim Record =new("The round's record is its last step: an implementer writes it, a fresh reviewer checks it, and it merges into the feature branch (DR 059 ruling 8, N12).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "As the root, deliver the round's one record as its last step, through your ticket's own Implement, Review and Merge hops: an implementer writes it from the pushed register, a fresh reviewer checks it, and it merges into the feature branch.", new(TicketCaptainSlot.InquisitionRecord, 2930))])
    { Spec = ["N12 DR 059", "K31", "N5"] };

    public static readonly Claim RecordShape = new("The record names the audit SHA, the staffing, the map and every finding with its disposition (DR 059 ruling 8).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "The record, `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md`, names the audit SHA, the staffing, the map with each lens run or the reason it did not, and every finding with its severity, sightings and disposition; an empty section says so.", new(TicketCaptainSlot.InquisitionRecord, 2931))])
    { Spec = ["N12 DR 059"] };

    public static readonly Claim SliceBrief = new("The inquisition-captain resource carries the slice reviewer brief (DYD-1082 1bc04dce change 5).", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Skills: /reviewer · level: inquisition · the lens · the slice, its map row and its findings so far · the audit SHA and the landing base · the governing spec · worktree · model and effort · return to.", new(TicketCaptainSlot.SliceBrief, 2940))])
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

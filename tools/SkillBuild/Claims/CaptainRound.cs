namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's round record (DYD-1372): what the holder of a Review ticket writes
/// down for its round, alone or with seats; the captain of the change records none of it (gap hunt 1,
/// G3; spec R33). A PASS goes into a PR only where the candidate is in one (G4). What then becomes of a
/// should and of a severity a fixer changed is the act of the captain running the rounds, so those
/// lines sit where that captain reads: a finding's route and fix rounds.
/// </summary>
public static class CaptainRound
{
    private const string Labels = "Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit, should described as for \"should tickets filed from reviews\".";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Verdict = new("The verdict lives on the Review ticket: PASS or FAIL, lenses, seats and shoulds; FAIL means at least one blocker.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.RoundSteps, 1,
            "Post one verdict on your Review ticket: PASS or FAIL, the lenses, each seat's model and provider with its blockers, and the shoulds. FAIL means at least one blocker.")
    ])
    {
        Spec = ["R17", "S8", "R11"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: each round is its own Review ticket, r1 to r4."),
            Layer: "The seats' returns live in the dispatcher's thread until someone writes them on the ticket.",
            Reason: "The next round, and whoever resumes the ticket, read the verdict there.")
    };

    public static readonly Claim NoPr = new("A round on a candidate that is in no PR, a leaf's evidence check among them, is recorded on its Review ticket alone.", Me, [])
    {
        Spec = ["S4", "R17"],
        Truth = new(Evidence.Code("Commands/PrReadyCommand.cs"),
            Layer: "`dydo merge` and `dydo pr-ready` read a PASS from a PR's body and from nowhere else, and a leaf whose officer merges has no PR of its own.",
            Reason: "A reviewer ordered to edit a body that does not exist stalls, or opens a PR nobody asked for.")
    };

    public static readonly Claim PrLine = new("A PASS of a candidate that is in a PR is written, with its verdict, as one line in that PR body's Independent review section.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.RoundSteps, 2,
            "On a PASS of a candidate that is in a PR, the one your brief names, add its line at once to that PR's body, under `## Independent review`: " +
            "`PASS - rubric/type: <code, docs or merge>; candidate <head SHA>; base <base SHA>`, then your Review ticket's key. " +
            "A candidate in no PR, as at a leaf's evidence check, has its verdict on your Review ticket alone.",
            () => [NoPr])
    ])
    {
        Spec = ["R17"],
        Truth = new(Evidence.Code("Services/Merge/IndependentReview.cs"),
            Layer: "`dydo merge` reads that line and refuses without it; it cannot write it.",
            Reason: "A line in any other shape is a refusal at the merge, long after the reviewer has gone.")
    };

    public static readonly Claim Nits = new("A nit is one line on its record.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.RoundSteps, 3, "Record each nit as one line on your Review ticket.")])
    {
        Spec = ["S11"],
        Truth = new(Evidence.Run(Labels + " The nit label is described as only for an inquisition's nit ticket: \"a review nit stays one line on its ticket's record\"."),
            Layer: "Linear would take a ticket per nit; the workspace keeps them off the board.",
            Reason: "A nit is kept where it can be read, without a ticket to open, hold and close.")
    };

    public static readonly Claim Shoulds = new("A should is filed as a child of the ticket where it was found, after a check for a duplicate under that ticket and its ancestors.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.RoundSteps, 4,
            "File each should as a child of the ticket whose work you judged, after checking the open shoulds there and above for a duplicate.")
    ])
    {
        Spec = ["S17"],
        Truth = new(Evidence.Run(Labels),
            Layer: "Linear takes a second ticket for the same fault without a word.",
            Reason: "It stays beside the work it is about and off the Project's root.")
    };

    public static readonly Claim Group = new("Grouping shoulds is a judgement: minor ones may share a ticket, ones on the same code may stay together, one that needs a lot of work gets its own.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.RoundRules, 1,
            "Group shoulds by judgement: minor ones may share a ticket, ones on the same code may stay together, and one that needs a lot of work gets its own.")
    ])
    {
        Spec = ["S17", "S4"],
        Truth = new(Evidence.Run("Linear, DYD-1333 read 2026-11-20: Fix r2 closed review r2's \"2 blockers and 5 shoulds\" as one ticket."),
            Layer: "A ticket is both a unit of work and a record, and no rule fits both readings every time.",
            Reason: "Five small fixes one agent makes in a pass need no five tickets, and a large one needs room of its own.")
    };

    public static readonly Claim Cheap = new("A cheap should is fixed in place before its parent merges, in the round's Fix or by a writer of its own; the rest wait.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ShouldRoutes, 1,
            "Cheap: have it fixed before the merge. Name it in the round's Fix brief; when the round has no blocker, dispatch a writer onto its ticket, and order one more round on the new head.")
    ])
    {
        Spec = ["S17", "R8"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, DYD-1327, closed \"7 blockers, 12 shoulds and 9 nits\" and Fix r2, DYD-1333, \"2 blockers and 5 shoulds\", " +
                                 "each as one ticket. `Services/Merge/PullRequestMerge.cs` refuses a head whose PR \"holds no PASS for candidate\" it, so a fix made after a round with no blocker needs a round of its own."),
            Layer: "Nothing weighs a should's cost, and a filed should sits on its ticket until somebody dispatches it.",
            Reason: "A fix made while the code is open costs less than a ticket that remembers it.")
    };

    public static readonly Claim ShouldInFix = new("A should fixed inside a Fix is closed by that Fix's holder: each should its brief names goes Done once its fix is proved, with the proof on it, before the Fix closes.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementFix, 4,
            "Close each should your brief names Done once its fix is proved, with the proof on it, before you close the Fix.")
    ])
    {
        Spec = ["S17", "S9"],
        Truth = new(Evidence.Run("Linear, DYD-1281 read 2026-11-21: Fix r2, DYD-1333, is titled as closing \"2 blockers and 5 shoulds\", and no child of DYD-1281 carries the label should: " +
                                 "those shoulds had no ticket of their own. A should filed by its round's holder has one, and save_issue's `state` is what moves it."),
            Layer: "Linear closes a Fix and leaves every other ticket where it stood; a should repaired under another ticket's name stays open on its own.",
            Reason: "An open card for work already done is collected into the next batch, and an agent is sent to repair what is no longer broken.")
    };

    public static readonly Claim ParentDone = new("A parent may close Done while its shoulds stay open.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ShouldRoutes, 2, "Anything more: it waits on its ticket, and its parent may close Done meanwhile.")])
    {
        Spec = ["S17"],
        Truth = new(Evidence.Run("Linear, DYD-1281 read 2026-11-20: the parent is Ready to Merge while its child DYD-1295 is Todo."),
            Layer: "Linear ties a parent's status to none of its children, so nothing settles this either way.",
            Reason: "A should can wait, so it never holds back the work it was found in.")
    };

    public static readonly Claim Changed = new("A severity a fixer changed is recorded, with its reason.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.FixRules, 5, "If the Fix's writer changed a finding's severity, record the change and its reason on the Review ticket.")])
    {
        Spec = ["R11"],
        Truth = new(Evidence.Code("skills/roles/crew/implementer/SKILL.md"),
            Layer: "The fixer returns its dispute to its dispatcher only; the Review ticket still shows the old label.",
            Reason: "The next reviewer sees why a blocker was not fixed as one.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

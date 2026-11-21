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
            "File each should as a child of the ticket whose work you judged, the one your brief names, after checking the open shoulds there and above for a duplicate.")
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

    public static readonly Claim Cheap = new("A cheap should is fixed in place before its parent merges: in the round's Fix, or, after a round with no blocker, by a writer on its own ticket; the rest wait.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ShouldRoutes, 1,
            "Cheap: have it fixed before the merge. Name it in the round's Fix brief; when the round has no blocker, and somebody merges the change, dispatch a writer onto its ticket, then go by where the candidate is. " +
            "In your PR: dispatch a Review onto the new head, since `dydo merge` takes no head without a PASS. This round is not the one more you may order, and you take it once: " +
            "a should that it, or a later round with no blocker, finds waits on its ticket. " +
            "In no PR: dispatch none, since the whole's Review will judge the mended head. If that head is the one your officer merges, go on to phase 5 of the spine and return its SHA. " +
            "Where nobody merges the change, a should that a round with no blocker leaves waits on its ticket instead: no later Review would judge its fix.",
            () => [NewHead!, Once!, MendedPart!, Unmerged!])
    ])
    {
        Spec = ["S17", "R8"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: Fix r1, DYD-1327, closed \"7 blockers, 12 shoulds and 9 nits\" and Fix r2, DYD-1333, \"2 blockers and 5 shoulds\", " +
                                 "each as one ticket."),
            Layer: "Nothing weighs a should's cost, and a filed should sits on its ticket until somebody dispatches it.",
            Reason: "A fix made while the code is open costs less than a ticket that remembers it."),
        Transition = new(CaptainState.Clear, CaptainState.Mending, "a round with no blocker leaves a cheap should on a change somebody merges: a writer mends it on its own ticket, once")
        {
            Guard = "~mended /\\ " + CaptainMachines.Merged,
            Effect = "mended' = TRUE"
        }
    };

    public static readonly Claim NewHead = new("A should mended after a round with no blocker, on a candidate in its holder's PR, makes a new head, which gets a round of its own before the merge; that round is not the one more round the officer may order.", Me, [])
    {
        Spec = ["R8", "R23", "R24"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` refuses a head that its PR's PASS does not name, and counts no rounds: it cannot tell a repair's round from an optional second look.",
            Reason: "Spent as the optional look, the repair's round would leave a captain that had already taken that look with a fixed head and no way to have it passed."),
        Transition = new(CaptainState.Mending, CaptainState.InReview, "the should is mended in its holder's PR: a round on the new head, which the merge needs")
        {
            Guard = CaptainMachines.Merges,
            Effect = "round' = round + 1"
        }
    };

    public static readonly Claim MendedPart = new("A should mended on a candidate that is in no PR gets no round of its own, since the whole's Review judges the mended head; " +
                                                  "a holder whose officer merges that head goes on to finish and returns its SHA.", Me, [])
    {
        Spec = ["R2", "R23", "S17", "R32"],
        Truth = new(Evidence.Run("This fix's writer was briefed \"Do not push; no PR; no merge.\" (DYD-1372 phase 1, fix g6) and returned its commit's SHA: its part was in no PR. " +
                                 "The whole is in PR 379, and `dydo merge <pr-number> --match-head-commit <sha> --match-base-commit <sha> --dry-run`, run on it 2026-11-21 at head `13da419b`, " +
                                 "went past its PASS check and stopped at \"checks on 13da419bf447 are not green\": the one PASS the command asks for stood on the whole's head, in the whole's PR. " +
                                 "`Services/Merge/PullRequestMerge.cs` reads a PASS only from the body of the PR it is given."),
            Layer: "`dydo merge` asks a PASS of the head of the PR it is handed and of no other commit, and a part's head is in no PR: nothing asks a second round of it, and nothing tells its holder that the mend was its last step before finishing.",
            Reason: "Another verdict on the part alone would be read by no command, while the lines the writer changed are judged anyway once every part is wired."),
        Transition = new(CaptainState.Mending, CaptainState.Finishing, "the should is mended on a candidate in no PR, which the holder's officer merges: no round of its own, and the mended head is returned, to be reviewed with the whole")
        {
            Guard = CaptainMachines.OfficerMerges
        }
    };

    public static readonly Claim Unmerged = new("A should that a round with no blocker leaves on a change nobody merges is not mended there: it waits on its ticket, since no later Review would judge its fix.", Me, [])
    {
        Spec = ["S17", "R2", "R33"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-21: the Severity group holds should, \"On should tickets filed from reviews and should Bugs an inquisition files; " +
                                 "open ones gate the Project's landing\", so a should left on its ticket is still collected."),
            Layer: "Nothing asks a PASS of a head that no command merges, and no whole stands above a change nobody merges for a later round to see.",
            Reason: "Mended after its last round, such a change would end on lines no second agent ever read.")
    };

    public static readonly Claim Once = new("The round that follows a mended should is taken once: a should that it, or a later round with no blocker, finds waits on its ticket.", Me, [])
    {
        Spec = ["R8", "S17"],
        Truth = new(Evidence.Run("Linear, team Dydo, read 2026-11-21: DYD-1336, DYD-1346, DYD-1301 and DYD-1347 carry the label should and sit in Backlog in the dydo 4.0.0 Project, each a ticket of its own that no round waits on. " +
                                 "Spec R8: \"By default a round with no new blocker ends the phase\". The user (DYD-1295 41a553f1): a should \"needs fixing, but the fix can happen later\"."),
            Layer: "Nothing counts rounds or weighs what one costs, and `dydo merge` is content with the PASS the head already has. " +
                   "No rule names a number either: \"once\" is this skill's own bound, chosen within R8's default.",
            Reason: "Each repair buys another whole review, so without an end the cheapest findings keep the dearest step running.")
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

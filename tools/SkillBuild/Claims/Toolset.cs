namespace DynaDocs.SkillBuild;

/// <summary>
/// The reviewer's toolset (R34): his review-level × tool matrix verbatim, with its legend and both footer
/// rules as recovered in DYD-1195 c88d99aa; his Drive and Formal qualification; the ticket-review mutation
/// default; when and how to use each tool, in his words of DYD-1082 01266a65 and 04b6a3d1; and the Read,
/// Break and Drive lenses, mapped onto the tool columns as R34 and N5 do at the inquisition.
/// </summary>
public static class Toolset
{
    private const string Table = """
        Legend: **due** every time; **if** when warranted; **–** not at this level.

        | | Read<br>local | Read<br>vs spec | Read<br>whole / gestalt | Run tests | Hypothesis<br>custom tests | Drive<br>the app | Formal<br>statechart, … |
        | --- | --- | --- | --- | --- | --- | --- | --- |
        | Leaf evidence check<br>a lane or hop, optional seat | due | if<br>drift only | – | due<br>its receipts, tagged | – | – | – |
        | Ticket review<br>a parent integrating its parts | due | due | if<br>its own area | due<br>tagged + feature; mutation on changed | if | if<br>user-facing | – |
        | Feature / Milestone<br>a subsystem is done | if | due | due | due<br>feature + suite | due<br>at the seams | due<br>its journeys | if<br>it has a state machine |
        | Inquisition<br>before landing, per slice | due<br>every slice | due | due | due<br>suite + --heavy + mutation | due<br>every hypothesis | due | due |

        > Every row: never re-run what the author ran the same way; a test receipt for the same state counts (spec R5). The security baseline is always on.
        >
        > How it's used: the officer's brief names the level; the reviewer works its row.
        """;

    public static readonly Claim Weight = new("Review is expensive, so each level calls only for its row's tools (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Review is expensive, so each level calls only for the tools its row names: a small piece passes if it works and holds nothing blatantly wrong, a larger one must also cohere and keep to the spec, and a finished subsystem must behave as a whole.", new(SpinePart.Job, 116))])
    { Spec = ["R34", "R6"] };

    public static readonly Claim Row = new("The brief names the level, and the reviewer works its row (DYD-1082 a1b21b59 Q6/Q9).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Your brief names the level: find its row in the [toolset](resources/toolset.md) and work that row.", new(DispatchState.Briefed, 3202, MoveKind.Nominal))])
    { Spec = ["R34"] };

    public static readonly Claim Matrix = new("The review-level × tool matrix, its legend and both footer rules, verbatim (DYD-1082 a1b21b59; DYD-1195 c88d99aa).", [Actor.Reviewer],
        [new(Skill.Reviewer, Table, new(ReviewerPart.Matrix, 3203))])
    { Spec = ["R34", "R5"] };

    public static readonly Claim BuiltAndSettled = new("Drive and Formal are due only where the tool is built and the subject settled (DYD-1082 04b6a3d1, a1b21b59).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Drive and Formal are due only where the tool is built and the subject is settled; a bare **due** in those columns never overrides this.", new(ReviewerPart.DriveAndFormal, 3220))])
    { Spec = ["R34"] };

    public static readonly Claim DriveNeeds = new("Drive needs a built, maintained driving skill that matches the feature (DYD-1082 04b6a3d1).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Drive needs a built, maintained driving skill that matches the feature.", new(ReviewerPart.DriveAndFormal, 3221))])
    { Spec = ["R34"] };

    public static readonly Claim FormalNeeds = new("Formal needs a purpose-built model or a statechart of a finished state machine (DYD-1082 04b6a3d1).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Formal needs a purpose-built model of the subject, or a statechart, which is only for a finished subsystem that has a state machine.", new(ReviewerPart.DriveAndFormal, 3222), () => [Review.Statechart])])
    { Spec = ["R34", "R7"] };

    public static readonly Claim Absent = new("A reviewer that wants an absent tool reports it to its officer as a finding rather than building it (DYD-1195 c88d99aa).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Report a tool you want but the project hasn't built to your officer as a finding, rather than building it.", new(ReviewerPart.DriveAndFormal, 3223))])
    { Spec = ["R34", "R5"] };

    public static readonly Claim ReadLocal = new("Read local judges the file and its methods alone for obvious problems (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Read local: the file, and the function or method in it, on their own: does it work, and is anything blatantly wrong, such as a security hole, a bad practice or waste?", new(ReviewerPart.Tools, 3210))])
    { Spec = ["R34", "R6"] };

    public static readonly Claim ReadSpec = new("Read vs spec checks the change against the spec and the rest of the code, so nothing drifts (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Read vs spec: the change against the governing spec and the rest of the code, so nothing builds or assumes against the spec and drifts.", new(ReviewerPart.Tools, 3211))])
    { Spec = ["R34", "R3"] };

    public static readonly Claim ReadWhole = new("Read whole looks at the folders and structure, where a problem can hide in the gestalt (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Read whole: the folders and the structure as a whole, where a problem can hide in the gestalt.", new(ReviewerPart.Tools, 3212))])
    { Spec = ["R34"] };

    public static readonly Claim ReadProof = new("A reading finding quotes both passages; an unused member quotes its definition and a reference search.", [Actor.Reviewer],
        [new(Skill.Reviewer, "A reading finding quotes both passages at `file:line`; an unused member quotes its definition and a reference search that finds no caller outside the tests.", new(ReviewerPart.Tools, 3213))])
    { Spec = ["R34", "K22"] };

    public static readonly Claim RunTests = new("Run tests picks the flavour the row names, from the tagged part to mutation (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Run tests: the flavour the row names, from the tagged sub-feature through the whole feature and the suite to `--heavy` and mutation testing the tests.", new(ReviewerPart.Tools, 3214))])
    { Spec = ["R34", "R5"] };

    public static readonly Claim TicketMutation = new("At a ticket review, mutation on changed means reading existing results, never queueing or waiting on mutation.", [Actor.Reviewer],
        [new(Skill.Reviewer, "At a ticket review, \"mutation on changed\" means reading the DYTEST mutation results that already exist for the changed code: queue no mutation work and wait on none, and note a missing result on the verdict, never as a blocker.", new(ReviewerPart.Tools, 3216))])
    { Spec = ["R34", "T12"] };

    public static readonly Claim Hypothesis = new("A hypothesis is decided by the reviewer's own custom test; red for the stated reason proves it (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Hypothesis: name what you suspect is wrong and decide it with a custom test of your own, in your own worktree at the candidate SHA; a test red for the stated reason is the finding's proof.", new(ReviewerPart.Tools, 3217))])
    { Spec = ["R34", "T3"] };

    public static readonly Claim Drive = new("Drive uses the app as a user would, against the spec's problem statement (DYD-1082 01266a65, 04b6a3d1).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Drive: use the app the way a user would, through its driving skill, and judge the experience and the behaviour against the spec's problem statement; rerunnable steps with the observed and the expected output are the proof.", new(ReviewerPart.Tools, 3218))])
    { Spec = ["R34", "R3"] };

    public static readonly Claim Formal = new("Formal runs the subject's own verification or the statechart, finding gaps from a model (DYD-1082 04b6a3d1).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Formal: run the subject's own formal verification, or `/statechart` on its state machine, to find gaps in behaviour from a model instead of the running code.", new(ReviewerPart.Tools, 3219))])
    { Spec = ["R34", "R7"] };

    public static readonly Claim Lenses = new("Read, Break and Drive are reviewer lenses, weighted by the level (DYD-1082 01266a65).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Read, Break and Drive are your lenses, weighted by your level's row.", new(ReviewerPart.Lenses, 3230))])
    { Spec = ["R34", "N5"] };

    public static readonly Claim InquisitionLenses = new("At the inquisition, Read takes the Read columns, Break the tests, hypotheses and formal checks, Drive the app.", [Actor.Reviewer],
        [new(Skill.Reviewer, "At the inquisition each lens is its own seat: Read covers the three Read columns; Break runs the tests (suite, `--heavy`, mutation), the hypothesis tests and the formal checks; Drive drives the app.", new(ReviewerPart.Lenses, 3231))])
    { Spec = ["R34", "N5"] };
}

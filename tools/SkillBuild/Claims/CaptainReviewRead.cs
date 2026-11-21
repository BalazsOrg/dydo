namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Review kind, reading (DYD-1372 07c86aad, "some just reads"): the
/// three reads of the user's level table (spec R34), what a reader looks for in the tests and the docs
/// of a change (T1, T5, T22, D6), and how it writes a finding down. Its facts are the legacy reviewer
/// toolset's, each checked again against the running system.
/// </summary>
public static class CaptainReviewRead
{
    private const string Toolset = "skills/roles/crew/reviewer/resources/toolset.md";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Local = new("A local read takes each changed file, and each function in it, on its own: does it work, and is anything plainly wrong.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 1,
            "Local: read each changed file, and each function in it, on its own. Does it work, and is anything plainly wrong, such as a security hole, a bad practice or waste?")
    ])
    {
        Spec = ["R34", "R6"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "A compiler and a test run say that code builds and passes, and nothing about whether a careful reader would let it stand.",
            Reason: "Most faults sit inside one function and show themselves to anyone who reads it slowly.")
    };

    public static readonly Claim VsSpec = new("A read against the spec holds the change to the governing spec and to the code around it.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 2,
            "Against the spec: read the change beside the governing spec and the code around it, for anything built on an assumption the spec does not make.")
    ])
    {
        Spec = ["R34", "R3"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "A diff shows what changed and never the sentence of the spec it was meant to serve.",
            Reason: "Drift starts as one reasonable guess, and later parts are built on top of it.")
    };

    public static readonly Claim Whole = new("A read of the whole takes the folders and the structure together.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 3, "The whole: read the folders and the structure together, for what no single file shows.")])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "Every tool that reads code reads it a file at a time.",
            Reason: "Two sound parts can still duplicate each other, or pull a design in opposite directions.")
    };

    public static readonly Claim Tags = new("A reader checks each suite test's job tag against what the test proves.", Me, [])
    {
        Spec = ["T5"],
        Truth = new(Evidence.Run("This repository's suite, read 2026-11-21: each test carries its job, as `[Trait(\"Job\", \"contract: …\")]` or `trap: …`. " +
                                 "`dydo/guides/coding-standards.md`, here and in the scaffold, says every suite test carries a trap, traced contract or edge job tag."),
            Layer: "A lint can see that a tag is there and cannot see whether the test does the job the tag claims.",
            Reason: "A tag is a claim about why the test exists, and an unearned one keeps a useless test alive.")
    };

    public static readonly Claim Tests = new("A test that pins an untraced rule, changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 4,
            "Read the tests as part of the change. One that pins a rule no spec or decision record traces, changes whenever the code does, checks that a deleted feature is gone, " +
            "or can never fail is a finding; so is a job tag its test does not earn.",
            () => [Tags])
    ])
    {
        Spec = ["T1", "T22", "R3"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "A green run counts a test as proof whatever it asserts.",
            Reason: "A test that asserts the wrong reality cements it, and every later change is made to agree with the mistake.")
    };

    public static readonly Claim Docs = new("On docs an unlinked reason is a finding, and on code a change to paths a doc's explains declares means reading that doc.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 5,
            "On docs, a reason that links no decision record or principle is a finding; on code, a change to paths that a doc's `explains:` declares means reading that doc too.")
    ])
    {
        Spec = ["D6", "D2", "D5"],
        Truth = new(Evidence.Code("Services/ExplainsCheck.cs"),
            Layer: "`dydo check` warns that a declared path changed, and cannot tell whether the doc still says what the code does.",
            Reason: "A doc left behind by its code misleads the next agent with more authority than a stale comment.")
    };

    public static readonly Claim Quote = new("A reading finding quotes both passages by file and line; an unused member is its definition with a reference search that finds no caller outside the tests.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 6,
            "Write a finding so it can be checked without you: both passages quoted at `file:line`, or, for an unused member, its definition with a reference search that finds no caller outside the tests.")
    ])
    {
        Spec = ["R5", "R11"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "A reader's finding has no failing test or output behind it, only the words it points at.",
            Reason: "The Fix's writer checks each severity at the source, and cannot check a paraphrase.")
    };

    public static readonly Claim IgnoreEntry = new("A reader checks each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewRead, 7, "Check each new ignore entry: an exact path or a generated-directory glob, for a file nobody writes by hand, with a reason.")])
    {
        Spec = ["T17"],
        Truth = new(Evidence.Open("No ignore file for the file size limit exists in the running system: this repository's only size check, `tools/SkillBuild/changed-file-sizes.sh`, reads none, " +
                                  "and no command of the CLI checks a file's size. Until the gate and its ignore file ship (T16), a reviewer has no entry to check."),
            Layer: "An entry in an ignore file silences a gate for good, and the gate cannot ask whether the file is really generated.",
            Reason: "A hand-written file listed as generated escapes the limit without anyone having decided that it may.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

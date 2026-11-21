namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Implement kind, the Bug recipe (DYD-1372; spec K31, "plus bug and
/// merge recipes", and T3: "The Bug red-reproduction rule stays"). Its facts are the legacy
/// implementer's bug recipe.
/// </summary>
public static class CaptainImplementBug
{
    private const string Legacy = "skills/roles/crew/implementer/resources/bug.md";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Name = new("A Bug's writer names the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementBug, 1,
            "Name the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.")
    ])
    {
        Spec = ["T3", "T4"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A Bug ticket reports a symptom in its reporter's words, wherever the reporter happened to meet it.",
            Reason: "A defect pinned too low gets a patch where it showed, while its cause goes on failing somewhere else.")
    };

    public static readonly Claim Diagnose = new("A Bug with no red reproduction yet is diagnosed with diagnosing-bugs.", Me, [])
    {
        Spec = ["K14", "T3"],
        Truth = new(Evidence.Code("skills/engineering/diagnosing-bugs/SKILL.md"),
            Layer: "A brief names the skills its dispatcher foresaw, and only the writer finds out that the fault will not reproduce.",
            Reason: "Guessing at a fault nobody can trigger produces a change that cannot be shown to cure it.")
    };

    public static readonly Claim Reproduce = new("A Bug starts from a red reproduction: a test at the product's boundary when the defect shows there, else at its seam.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementBug, 2,
            "Reproduce it red before you fix it: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction. " +
            "With none yet, load /diagnosing-bugs.",
            () => [Diagnose])
    ])
    {
        Spec = ["T3"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A test runner reports a green run and cannot say whether the fault was ever caught.",
            Reason: "A fix made before its failure was seen may be curing a different fault, or none.")
    };

    public static readonly Claim Unused = new("An unused member needs no test: its definition and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementBug, 3,
            "An unused member needs no test: its definition at `file:line` and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.")
    ])
    {
        Spec = ["T3", "T11"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "No test can fail because of code that nothing calls.",
            Reason: "Dead code kept alive by its own tests looks used to every search that follows.")
    };

    public static readonly Claim Revert = new("After the fix, its writer reverts it, watches the reproduction fail, and restores it.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementBug, 4,
            "After the fix, revert it, watch the reproduction fail, and restore it; return that red line, with the behaviour you kept still green.")
    ])
    {
        Spec = ["T3", "R5"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A reproduction that goes green proves the fix only if nothing else changed in between.",
            Reason: "The red line shows the test holds this very fault, and gives the reviewer a receipt it need not run again.")
    };

    public static readonly Claim Race = new("A race is forced with fake timers or a set interleaving; a test that needs a sleep or a timeout is wrong.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementBug, 5, "Force a race with fake timers or a set interleaving: a test that needs a sleep or a timeout is wrong.")])
    {
        Spec = ["T3", "T1"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A sleep makes a test pass on a quiet machine and fail on a loaded one, and the runner calls both results true.",
            Reason: "A timing-dependent test flakes under a busy host, and a flake teaches everyone to ignore red.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

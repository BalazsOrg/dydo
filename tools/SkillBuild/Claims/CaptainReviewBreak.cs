namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Review kind, trying to break it (DYD-1372 07c86aad, "some tries to
/// break it"): the tests a row names and the reviewer's own hypothesis tests (spec R34, R5, T12). The
/// commands are the project's, so the lines name where a project keeps them and never the commands.
/// </summary>
public static class CaptainReviewBreak
{
    private const string Toolset = "skills/roles/crew/reviewer/resources/toolset.md";
    private const string Guide = "dydo/guides/testing-strategy.md";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Rerun = new("What the author ran is run again the same way only when later changes put its result in doubt.", Me, [])
    {
        Spec = ["R5"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "A test runner runs whatever it is asked to, however often the same thing passed before.",
            Reason: "A second identical run finds what the first one found, and spends the reviewer on nothing.")
    };

    public static readonly Claim Run = new("A breaker runs the tests its row names with the project's own commands, which the project's testing guide holds.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewBreak, 1,
            "Run the tests your row names with your project's own commands, which `" + Guide + "` holds; run again what the author ran the same way only when later changes put its result in doubt.",
            () => [Rerun])
    ])
    {
        Spec = ["R34", "T14"],
        Truth = new(Evidence.Run("`" + Guide + "` exists here and in the scaffold dydo ships. Here its section \"Current project facade\" gives the runner and its commands; " +
                                 "the scaffold's copy opens \"This portable policy needs project-owned commands and selectors. Declare their actual support in this guide when adopting it\"."),
            Layer: "The CLI has no test command of its own, so how a suite is run differs from one project to the next.",
            Reason: "A command copied from another repository fails here, or passes while selecting no test at all.")
    };

    public static readonly Claim Hypothesis = new("A breaker names what it suspects is wrong and decides it with a test of its own on the candidate's commit; a test red for the named reason proves the finding.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewBreak, 2,
            "Name what you suspect is wrong, then decide it with a test of your own on the candidate's commit: a test that fails for the reason you named proves the finding.")
    ])
    {
        Spec = ["R34", "T3"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "The author's tests cover what the author thought of, and nothing in a suite hunts for the case that was never imagined.",
            Reason: "A suspicion costs its reader an argument; a failing test settles it without one.")
    };

    public static readonly Claim Untouched = new("A breaker puts its test and the failing output in the finding and commits nothing to the candidate's branch.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ReviewBreak, 3, "Put that test and its failing output in the finding, and commit nothing to the candidate's branch.")])
    {
        Spec = ["R17", "R5"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` merges only the head a PASS names, and git lets a reviewer move that head with one commit.",
            Reason: "The Fix's writer starts from a reproduction that already fails, on a branch still exactly as it was judged.")
    };

    public static readonly Claim Mutation = new("At a ticket review, mutation means reading the results that already exist for the changed code; no run is started or awaited, and a missing result is a note.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewBreak, 4,
            "At a ticket review, mutation means reading the results that already exist for the changed code: start no mutation run, wait on none, and note a missing result, never as a blocker.")
    ])
    {
        Spec = ["R34", "T12"],
        Truth = new(Evidence.Run("`" + Guide + "`, read 2026-11-21: \"Mutation is unavailable on every stack\", three of them \"Pending DYD-103\". So a ticket review here finds no result and notes that. " +
                                 "His choice stands behind the rule (DYD-826, M3a): no ticket or merge waits on mutation."),
            Layer: "The table's cell says \"mutation on changed\" and leaves open whether the reviewer is to produce the result or only to read it.",
            Reason: "A mutation campaign outlasts any review, so a reviewer that started one would hold its verdict for hours.")
    };

    public static readonly Claim Equivalent = new("A reviewer that wrote neither the code nor its tests approves an equivalent mutant one at a time, with a reason; a claim its author disputes goes to a council.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewBreak, 5,
            "Asked to judge an equivalent mutant: approve one at a time, with a reason, and only if you wrote neither the code nor its tests; a rejection the author disputes goes to a council.")
    ])
    {
        Spec = ["T12"],
        Truth = new(Evidence.Open("No mutation mechanism runs on any stack of this repository (`" + Guide + "`: \"Mutation is unavailable on every stack\"), so no surviving mutant and no equivalence claim " +
                                  "exists to approve, and the council that would take a dispute has no resource in this skill."),
            Layer: "A mutation tool reports a survivor and cannot say whether killing it is possible at all.",
            Reason: "Whoever wrote the code is the one who most wants its survivor waved through.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

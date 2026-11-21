namespace DynaDocs.SkillBuild;

/// <summary>
/// The chief of staff's muster resource (K11, K22), truth-checked ahead of its skill (DYD-1402): the checklist it holds
/// with him before the night, the request each coordinator answers and the list he gets. Each claim was held against the
/// muster of 2026-11-21, which ran the installed skills, and against the skills on this branch. What a question to him
/// must hold is <see cref="StaffQuestion"/>'s.
/// </summary>
public static class StaffMuster
{
    private const string Night = "T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: ";

    private const string Request = """
        ```text
        His muster request for <date>, <Project>. Return:
        Intent: his goals and purpose for the Project in your own words, by /in-your-own-words, apart from the work.
        Work: your next tickets in fill order, the first runnable one, and the plan's stop line, or none.
        Blockers: each operational obstacle, and the evidence that would show it fixed.
        Permissions: each action the night's work needs, with a safe exercise for it, or the gap.
        ```
        """;

    private const string Listed = """
        ```text
        <Project> — <coordinator thread link> — <ready, or not ready>
        ```
        """;

    private static readonly Enum[] Me = [Actor.ChiefOfStaff];

    public static readonly Claim Ready = new("The muster is a checklist held with him present, after overnight-mode's transition preflight; the night starts once it is done.", Me,
        [At(Actor.ChiefOfStaff, ChiefOfStaffResource.Muster, 2001, "Hold this checklist with him present, after `overnight-mode`'s transition preflight; the night starts only once every line is done.")])
    {
        Spec = ["K11", "H7", "K22"],
        Truth = new(Evidence.Run(Night + "the chief of staff read `overnight: off` with `dydo mode show` at 02:03 UTC (position 6), before it sent a request, and its report to him at 02:05 " +
                                 "says the mode is still off (position 44). `skills/productivity/overnight-mode/SKILL.md` on this branch, hand-written: " +
                                 "\"Hold the muster by the chief of staff's checklist, `chief-of-staff/resources/muster.md`.\""),
            Layer: "`Commands/GuardCommand.ModeSet.cs`: the guard answers the command that switches the mode with a reminder and passes the same command's retry; that file does not name the muster.",
            Reason: "Whatever wants his answer is settled while he is awake to give it, ahead of the hours when a question finds him asleep.")
    };

    public static readonly Claim Ask = new("Each live coordinator returns a muster report to a fixed request.", [Actor.ChiefOfStaff, Actor.Coordinator],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2002, "Ask each live coordinator for its muster report, with the request below.")])
    {
        Spec = ["K11", "H7", "K22"],
        Truth = new(Evidence.Run(Night + "four `t3_thread_send` calls, one to each coordinator's thread, came back `delivery: started` between 02:03:44 and 02:03:58 UTC (positions 24 to 27), " +
                                 "and the four reports arrived in the chief of staff's thread between 02:04:10 and 02:05:10 (positions 31, 34, 41 and 42)."),
            Layer: "`t3_thread_send` delivered each request as it was written (positions 24 to 27), and each report that came back answered the request's parts (positions 31, 34, 41 and 42).",
            Reason: "Answers in one shape can be laid side by side, and a part asked of all four is missed at once when one leaves it out.")
    };

    public static readonly Claim Unblock = new("Operational blockers are resolved while he is present, each fix verified with evidence.", [Actor.ChiefOfStaff, Actor.Balazs],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2003, "Resolve each operational blocker while he is present, verifying each fix with evidence.")])
    {
        Spec = ["K11", "K22"],
        Truth = new(Evidence.Run(Night + "the DYTEST Network coordinator named its blocker, a fix and the check that would show it fixed, \"Verify: one windows-proof push succeeds\" (position 41). " +
                                 "The chief of staff passed the fix on to him at 02:05 (position 44), and read the org's billing at 02:17, after he doubted it: " +
                                 "one command, three `gh api` calls (positions 50 to 52)."),
            Layer: "The coordinator's report named a fix and its check (position 41); the fix reached him twelve minutes before the billing was read (positions 44 and 52).",
            Reason: "A repair proved in front of him can still be corrected by him; one that fails later meets him asleep.")
    };

    public static readonly Claim Permissions = new("Each permission the night needs is exercised safely and its real result recorded; no dummy merge.", Me,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2004,
            "Exercise each permission the night's work needs through a safe, authorised operation where one exists, such as contacting the coordinator, each session's Linear writes and feature-branch merges, " +
            "and record the real result; an unprotected branch proves no merge permission, and a dummy merge is never run.")
    ])
    {
        Spec = ["K11", "K22"],
        Truth = new(Evidence.Run(Night + "the four sends to the coordinators came back `delivery: started` (positions 24 to 27), and the chief of staff's Linear write returned its update's id (position 55). " +
                                 "The LC coordinator's report named a write its exercise had shown refused: \"My `save_project` edit was refused by the classifier\" (position 31). " +
                                 "The dydo coordinator gave its merges by their last real use, \"last exercised with PR 376 → 6dc834f0 on 2026-11-20\" (position 34), and no merge was run for the muster. " +
                                 "GitHub, read 2026-11-21 with `gh api repos/BalazsOrg/dydo/branches/feature%2Fdydo-4-0-0`: `protected: false`."),
            Layer: "GitHub reports `feature/dydo-4-0-0` as unprotected, which says nothing of what he has granted; and the host's classifier refused the LC coordinator a Linear write, which its report told (position 31).",
            Reason: "A grant found missing at muster is one he can give on the spot; found missing in the small hours, it waits for him.")
    };

    public static readonly Claim Gaps = new("An exercise that can't run or an item left unresolved is a gap with his explicit disposition.", [Actor.ChiefOfStaff, Actor.Balazs],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2005, "Keep an exercise you can't run, or an item left unresolved, as a gap, and get his explicit disposition for the work that depends on it.")])
    {
        Spec = ["K11", "K22"],
        Truth = new(Evidence.Run(Night + "the DYTEST Network coordinator listed what it could not exercise, \"Unsupported gaps (by design, Balazs only): billing; merges; re-pins (the wizard); CI activation\" (position 41). " +
                                 "He disposed of the work that hung on the first, \"I would hold the windows checks until the very end\" (position 50), " +
                                 "and that night a DYTEST PR counted as ready once reviewed and proven on Linux (position 55)."),
            Layer: "What the Network coordinator could not exercise was known that night because its report wrote it out (position 41).",
            Reason: "He says beforehand whether work leaning on something unproven runs, waits or is dropped, instead of finding it stalled in the morning.")
    };

    public static readonly Claim StopLine = new("A stop line he wants goes in the Project plan the coordinator reads.", Me,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2006, "A stop line he wants goes in the Project plan, which its coordinator reads.")])
    {
        Spec = ["K11"],
        Truth = new(Evidence.Run("`skills/roles/officers/coordinator/SKILL.md`, the coordinator's skill on this branch, not yet truth-checked: \"Read the Project from Linear, its plan included\" and " +
                                 "\"when he wants a stop line, it is in the Project plan, and you stop there\". `skills/productivity/overnight-mode/SKILL.md` on this branch, hand-written: " +
                                 "\"A stop line, when the user wants one, goes in the Project plan its coordinator already reads.\" " +
                                 "The muster of 2026-11-21 ran the installed skills, which put each stop line in a Project update instead (T3 thread d4c81ce0, position 55)."),
            Layer: "That night each stop line went into a Project update (position 55), a second place beside the Project plan that the coordinator's skill has it read.",
            Reason: "A limit written where its reader already looks needs no second document to be found, trusted and expired.")
    };

    // Split from StopLine (DYD-1402): the one muster on record gave night orders, so this half is open, and its line withheld.
    public static readonly Claim NoNightOrders = new("The night has no orders of its own: there are no night orders (DYD-1195 33520d6c).", Me,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2007, "There are no night orders.")])
    {
        Spec = ["K11"],
        Truth = new(Evidence.Open("On 2026-11-21 he gave one himself at muster, \"one night order: we're short on openai usage so when using models from that family use 6.1 sol on low. " +
                                  "This order is only for the night\" (T3 thread d4c81ce0, position 50), and the chief of staff posted night orders on each Project as a Project update (positions 55 and 56), " +
                                  "which spec H16 now cites as \"night orders 9099daa1\". The officer skills on this branch, `skills/roles/officers`, name none. The one muster on record gave them, and whether a muster goes without is his call."),
            Layer: "The night orders of 2026-11-21 were Project updates the chief of staff posted, each with a line of its own that it expires at the morning brief (positions 55 and 56).",
            Reason: "Two places for one evening's instructions let a coordinator follow the stale one.")
    };

    public static readonly Claim Runnable = new("Each coordinator confirms its next work is runnable before the night starts.", [Actor.ChiefOfStaff, Actor.Coordinator],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2008, "Get each coordinator's confirmation that its next work is runnable.")])
    {
        Spec = ["K11", "K22"],
        Truth = new(Evidence.Run(Night + "each of the four coordinators sent back that its work was runnable, between 02:19:39 and 02:20:06 UTC (positions 64, 68, 71 and 73)."),
            Layer: "That its work could begin was each coordinator's own statement, given after the night's orders reached it (positions 64, 68, 71 and 73).",
            Reason: "A lane that cannot begin is found while he is there, not at the first watch tick half an hour later.")
    };

    // Review r2 R2-B1: H7's "never relays content" holds at muster too, and H6 spares him the coordinator threads. A question's block is the exception (StaffQuestion.Context).
    public static readonly Claim List = new("He gets each coordinator's thread link and its readiness, never a report's content.", [Actor.ChiefOfStaff, Actor.Balazs],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterChecklist, 2009, "List for him each coordinator's thread link and its readiness in the form below, never a report's content; then the muster is ready.")])
    {
        Spec = ["K11", "H7", "H6"],
        Truth = new(Evidence.Run(Night + "each coordinator's report arrived whole in the chief of staff's own thread, where he was reading (positions 31, 34, 41 and 42). " +
                                 "The chief of staff then retold all four (position 44), and the retelling lost part of one: the dydo coordinator's question listed options (a), (b) and (c), and he was shown (a) alone."),
            Layer: "Each report stayed whole in the thread it was sent to (positions 31, 34, 41 and 42); the chief of staff's retelling was a second copy, and it differed from the first (position 44).",
            Reason: "He corrects the original, where its author will see the correction, instead of a summary that may have dropped something.")
    };

    public static readonly Claim Asked = new("The muster request a coordinator answers.", Me,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterRequest, 2010, Request)])
    {
        Spec = ["K11", "K22", "H5"],
        Truth = new(Evidence.Run(Night + "the request sent that night asked for Intent, Work, Questions, Blockers and Permissions (positions 24 to 27), and each of the four reports answered every part " +
                                 "(positions 31, 34, 41 and 42). This one keeps four of those parts and has no Questions part; the one muster on record sent the older request. " +
                                 "`skills/productivity/in-your-own-words/SKILL.md` loads on \"the user's muster request relayed by the chief of staff\"."),
            Layer: "The four reports took their parts from the request that asked for them (positions 24 to 27, then 31, 34, 41 and 42).",
            Reason: "The same four headings from every Project let him compare them, and a missing one shows at a glance.")
    };

    public static readonly Claim Form = new("The line each Project gets on his muster list: its coordinator's thread link and readiness.", Me,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterList, 2011, Listed)])
    {
        Spec = ["K11", "H7"],
        Truth = new(Evidence.Tool("t3_thread_read, called 2026-11-21 on thread d4c81ce0-f13a-4cea-80bd-6911b05d288f: the result's `thread.link` is a Markdown link to the thread, " +
                                  "`[Overnight Muster and Admiral Readiness](t3-thread://v2/f7f7b47f-c6f2-4990-a113-851014c04ff0/d4c81ce0-f13a-4cea-80bd-6911b05d288f)`."),
            Layer: "`t3_thread_read` returns a thread's link; which threads were that night's coordinators came from him, \"There are 4 admirals working\" (position 0), and from the chief of staff's search (position 19).",
            Reason: "One row for each shows at a glance who is not prepared, with the click that takes him there.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) => new(Skill.ChiefOfStaff, reader, text, new(slot, order));
}

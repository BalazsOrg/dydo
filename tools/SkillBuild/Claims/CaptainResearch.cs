namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Research kind resource (DYD-1372): one question a choice waits on.
/// Research pools and verifies; a scout is its native helper for one lookup and holds no ticket
/// (option A, DYD-1295 0f340abd).
/// </summary>
public static class CaptainResearch
{
    private const string Legacy = "skills/roles/crew/research/SKILL.md";
    private const string Helper = "Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone.";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Asked = new("A Research-kind ticket is one question a choice waits on, answered with every claim cited to an opened passage.", Me,
        [At(CaptainSlot.ResearchLead, 1, "Your ticket is one question a choice waits on. Answer it with every claim cited to a passage you opened.")])
    {
        Spec = ["K30"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "The ticket states the question; what makes an answer worth trusting is said only here.",
            Reason: "Whoever waits on the choice acts on the answer without the means to check it.")
    };

    public static readonly Claim Restate = new("Research restates its question and says what would count as an answer.", Me,
        [At(CaptainSlot.ResearchSteps, 1, "Restate the question in one sentence, and say what would count as an answer.")])
    {
        Spec = ["K30"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A question arrives in its asker's words, shaped by a context the researcher never saw.",
            Reason: "A misread question is found here, before any source is opened.")
    };

    public static readonly Claim Families = new("Research marks each source family in or out, with a reason, and reads governing docs on the default branch too.", Me,
    [
        At(CaptainSlot.ResearchSteps, 2,
            "Mark each source family in or out, with a reason: decision records, the spec and plans, other docs, code and tests, git history, Linear, outside sources. Read governing docs on the default branch too; a branch can lag.")
    ])
    {
        Spec = ["K30", "K22"],
        Truth = new(Evidence.Run("This skill's writer met it: the spec said `dydo merge` reverts a revert, and the code on its branch holds no such thing."),
            Layer: "A search returns what it was pointed at and stays silent about the family nobody opened.",
            Reason: "The answer often sits in the one family that was assumed to be empty.")
    };

    public static readonly Claim Scouts = new("Research sends one scout per source family in play, as native helpers in parallel, and reads a lone family itself.", Me,
        [At(CaptainSlot.ResearchSteps, 3, "Send one scout per family in play, as native helpers in parallel, each with the question and its one family; read a lone family yourself.")])
    {
        Spec = ["K30", "S1"],
        Truth = new(Evidence.Tool(Helper),
            Layer: "The host starts a helper on request; that a lookup is a helper's job and never a ticket is said only here.",
            Reason: "One lookup leaves nothing anyone but the researcher uses, so a ticket for it is overhead.")
    };

    public static readonly Claim Open = new("Research opens the passage behind every load-bearing claim and confirms it.", Me,
        [At(CaptainSlot.ResearchSteps, 4, "Open the passage behind every load-bearing claim, and confirm it says what the scout said.")])
    {
        Spec = ["K30", "K22"],
        Truth = new(Evidence.Tool(Helper),
            Layer: "A helper's report is a summary, and the researcher sees nothing of how it was made.",
            Reason: "A scout's mistake comes back as confident as its findings.")
    };

    public static readonly Claim Conflicts = new("Where sources conflict, research names the conflict and the governing source; where they are silent, the point is unsettled.", Me,
    [
        At(CaptainSlot.ResearchSteps, 5,
            "Where sources conflict, name the conflict and the source that governs; where they are silent, mark the point unsettled rather than reason your way to a fact.")
    ])
    {
        Spec = ["K30", "K22"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "Nothing separates what a source says from what a reader inferred from it.",
            Reason: "An inferred fact that reads like a found one is how a wrong rule gets built on.")
    };

    public static readonly Claim Answer = new("Research writes the answer first, then each claim with its citation and passage, what stayed unsettled, and every family searched.", Me,
    [
        At(CaptainSlot.ResearchSteps, 6,
            "Write the answer first, then each claim with its citation (a path at its commit, or a URL) and its exact passage, what stayed unsettled, and every family searched, the empty ones included.")
    ])
    {
        Spec = ["K30", "K22"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "Only the researcher knows which families it opened and found empty.",
            Reason: "Its reader checks any claim in one step, and sees where nobody looked.")
    };

    public static readonly Claim Question = new("A Question's captain researches first, answers what the facts settle, and returns the rest as a need for the user.", Me,
    [
        At(CaptainSlot.ResearchQuestion, 1,
            "Research first. When the facts settle the call, put the answer on the ticket; when they do not, return it to your officer as a need for the user, with what you found.")
    ])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Type group holds Question, \"Resolution: the answer, posted on close\"."),
            Layer: "Nothing keeps a question that the sources could answer from being put to the user.",
            Reason: "The user is asked only what nothing else can answer reliably.")
    };

    private static Line At(Enum slot, int order, string text) =>
        new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, text, new(slot, order));
}

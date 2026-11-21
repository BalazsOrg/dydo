namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's briefs resource (DYD-1372): what a dispatcher's brief names, for
/// every kind and for a review. It replaces the legacy per-kind dispatch templates (K14) with one file.
/// </summary>
public static class CaptainBriefs
{
    private const string OwnBrief = "This skill's writer got such a brief (DYD-1372 hop 2): ";
    private const string Toolset = "skills/roles/crew/reviewer/resources/toolset.md";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim NoPeek = new("A dispatcher briefs without loading the child's kind resource; the child loads its own.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefLead, 1, "Brief without loading the child's kind resource: the child loads its own.")])
    {
        Spec = ["K14", "K29"],
        Truth = new(Evidence.Run("This skill's writer loaded its two skills from their names in the brief; nothing of them was pasted in."),
            Layer: "Nothing stops a dispatcher from reading every resource in the folder.",
            Reason: "A dispatcher that reads the recipe starts steering how the work is done.")
    };

    public static readonly Claim Ticket = new("A brief names the ticket and its kind.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefAll, 1, "The ticket, by its key, and its kind.")])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Run(OwnBrief + "\"a code-writer on Linear DYD-1372 (team Dydo, Project dydo 4.0.0), hop 2 of 3\"."),
            Layer: "T3 knows the thread and Linear the ticket; the brief is the only thing that joins them for the child.",
            Reason: "The kind is how the child picks its resource.")
    };

    public static readonly Claim Skills = new("A brief names /ticket-captain and every other skill the ticket needs, each with a slash.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefAll, 2, "`/ticket-captain`, then every other skill the ticket needs, each with a slash.")])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Tool("Skill: \"Users may also ask for one by name (`/<name>`, or \\\"slash command\\\"); that's a request to invoke it.\""),
            Layer: "The host lists skills and leaves loading them to the agent's own judgement.",
            Reason: "Named skills get loaded; skills left to judgement get loaded sometimes.")
    };

    public static readonly Claim Leaf = new("A brief says when its ticket was cut as a leaf, which its captain does itself and splits only if it proves too big.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefAll, 3,
            "Whether you cut the ticket as a leaf: its captain then does the work itself, and splits it only if it proves too big.")
    ])
    {
        Spec = ["K30", "H2"],
        Truth = new(Evidence.Run("This skill's writer got such a brief (DYD-1372 phase 1, hop A): one writer for the whole hop, and \"If the rest will not fit your context, " +
                                 "commit what is whole and green and return `too big, re-cut`\". T3 tells every child the same by default: \"do the work yourself. Start no subagents unless your task says to.\""),
            Layer: "A child sees its ticket and never the cut that sized it, so only its dispatcher can say that the ticket was meant to fit one agent.",
            Reason: "A captain told that it holds a leaf goes straight to the work and reads the orchestration only when the leaf fails to fit.")
    };

    public static readonly Claim Merger = new("A brief names who merges the work.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefAll, 4, "Who merges the work: the child, you, or nobody when it merges nothing.")])
    {
        Spec = ["K14", "R21"],
        Truth = new(Evidence.Run(OwnBrief + "\"Do not push; no PR; no merge.\""),
            Layer: "Nothing in a checkout says whose the merge is.",
            Reason: "It decides how the child finishes, and two agents never both merge.")
    };

    public static readonly Claim Change = new("A change's brief names its owned paths and gates, and a Fix's the findings it closes.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefAll, 5, "For a change: the paths it owns and the project's gates it must pass. For a Fix, also the findings it closes.")])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Run(OwnBrief + "it named the gates before return and \"Commit only owned paths\"."),
            Layer: "The guard blocks a few paths for everyone and knows nothing of one ticket's.",
            Reason: "Two writers with stated paths cannot overwrite each other.")
    };

    public static readonly Claim Asked = new("A question's brief names the question and the choice that waits on it.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefAll, 6, "For a question: the question, and the choice that waits on it.")])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Tool("delegate_task: \"Start one child agent with only `task`. No parent history.\""),
            Layer: "The child cannot see the conversation the question came out of.",
            Reason: "Knowing the choice tells the researcher what would count as an answer.")
    };

    public static readonly Claim Candidate = new("A review brief names the candidate by its SHA.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefReview, 1, "The candidate, by its SHA.")])
    {
        Spec = ["K14", "R17"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` merges only the SHA a PASS names, so the reviewer has to be told which one to judge.",
            Reason: "A verdict on a moving branch is a verdict on nothing.")
    };

    public static readonly Claim Governing = new("A review brief names the governing spec, or says there is none.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefReview, 2, "The governing spec, or that there is none.")])
    {
        Spec = ["R3"],
        Truth = new(Evidence.Run(OwnBrief + "it named the approved spec's file and the sections to read."),
            Layer: "A ticket links its contract; nothing leads a reviewer from there to the spec.",
            Reason: "A review held only to the contract passes a wrong rule the contract inherited.")
    };

    public static readonly Claim LeafCheck = new("A leaf gets no full review, only an optional evidence check: a Review ticket of its own beside the leaf's, for a fresh seat.", Me, [])
    {
        Spec = ["S4", "R6"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4 are each a ticket of their own, children of DYD-1281 beside the Fix and part tickets they judge, " +
                                 "and none sits under one of those. `" + Toolset + "` holds the leaf's row: \"a lane or hop, optional seat\"."),
            Layer: "The level table has a row for a leaf, and nothing keeps a dispatcher from asking more of one or from leaving the check without a ticket.",
            Reason: "The review of the whole covers each part, so a full review of one part is paid for twice.")
    };

    public static readonly Claim Level = new("A review brief names the level.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefReview, 3,
            "The level: a leaf's evidence check, a ticket review, or a feature or Milestone review. " +
            "A leaf gets at most the check, never a full review: open the check as a Review ticket of its own beside the leaf's, for a fresh seat.",
            () => [LeafCheck])
    ])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Code(Toolset),
            Layer: "The review resource holds a row per level and cannot know which one this candidate is at.",
            Reason: "The level sets how much review the candidate gets, and review is expensive.")
    };

    public static readonly Claim Lenses = new("A review brief names the lenses, which shift up the tree; a first version is never reviewed as final.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefReview, 4,
            "The lenses, which rise up the tree: it works, with a security baseline; then it integrates; then it is optimal, secure, performant and in harmony. A first version never gets a final one's lenses.")
    ])
    {
        Spec = ["R6"],
        Truth = new(Evidence.Run("DYD-1281's rounds, read 2026-11-20: each Review ticket's title says what it looks at, \"whole-candidate\" in r1 to r3."),
            Layer: "Only the dispatcher knows how far up the tree the candidate sits.",
            Reason: "Polishing a first version spends review on code the next part will change.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

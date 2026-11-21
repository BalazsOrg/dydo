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
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefAll, 4,
            "Who merges the work: the child, you, or nobody when it merges nothing. When it is the child, also the branch its PR targets, a `feature/` branch: `dydo merge` takes no other.",
            () => [Target!])
    ])
    {
        Spec = ["K14", "R21"],
        Truth = new(Evidence.Run(OwnBrief + "\"Do not push; no PR; no merge.\""),
            Layer: "Nothing in a checkout says whose the merge is.",
            Reason: "It decides how the child finishes, and two agents never both merge.")
    };

    public static readonly Claim Target = new("The brief of a child that merges its own work names the branch its PR targets, which dydo merge takes only when it is a feature branch.", Me, [])
    {
        Spec = ["K14", "R23"],
        Truth = new(Evidence.Code("Services/Merge/FeatureBranch.cs"),
            Layer: "`dydo merge` refuses a PR whose base is no `feature/` branch, and says so only at the merge; a fresh checkout names no branch as the one its work is for.",
            Reason: "A child left to pick its base aims at its officer's ticket branch, where the one command that may merge a PR refuses.")
    };

    public static readonly Claim Change = new("A change's brief names its owned paths and gates, and a Fix's the findings it closes.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefAll, 5,
            "For a change: the paths it owns and the project's gates it must pass. For a Fix, also the findings it closes, each should by its ticket's key, and the SHA it starts from: your branch's head, which holds the candidate they were found at.",
            () => [FixBase!])
    ])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Run(OwnBrief + "it named the gates before return and \"Commit only owned paths\"."),
            Layer: "The guard blocks a few paths for everyone and knows nothing of one ticket's.",
            Reason: "Two writers with stated paths cannot overwrite each other.")
    };

    public static readonly Claim FixBase = new("A Fix's brief names the SHA the Fix starts from: its dispatcher's branch head, which holds the candidate the findings were found at.", Me, [])
    {
        Spec = ["K14", "S9"],
        Truth = new(Evidence.Run("This skill's third fixer got such a brief (DYD-1372 phase 1, fix g3): \"branch `DYD-1372-captain-prototype`, head `269188cd`, clean\", the head of its captain's branch that day."),
            Layer: "A writer's thread keeps its own last commit, and nothing in a message says that the whole has moved past it.",
            Reason: "Told where to begin, a writer whose checkout stayed at its own part does not repair a tree the round never saw.")
    };

    public static readonly Claim Asked = new("A captain briefs a Question only when it sends a child to research it: the brief names the question, the choice that waits on it, " +
                                             "and that the child researches it. A HITL Question is never briefed by its captain: it goes up.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefAll, 6,
            "For a Question you send a child to research, the only Question you brief: the question, the choice that waits on it, and that the child is to research it. " +
            "A HITL Question goes up unbriefed, by [A need for the user](need-for-the-user.md).")
    ])
    {
        Spec = ["K14", "H23"],
        Truth = new(Evidence.Tool("delegate_task: \"Start one child agent with only `task`. No parent history.\" Linear, team Dydo, labels read 2026-11-21: no label names a Question's stage, " +
                                  "so the brief is where its holder learns that it holds one to research."),
            Layer: "The child cannot see the conversation the question came out of, and its ticket does not say how the question is to be settled.",
            Reason: "Knowing the choice tells the researcher what would count as an answer, and being told to research sorts it to the right recipe at intake.")
    };

    public static readonly Claim TakeOver = new("A brief says when its child takes over a ticket another agent started, and names the commit it resumes from.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefAll, 7, "Whether the child takes over a ticket another agent started, and then the commit it resumes from.")])
    {
        Spec = ["S7", "H3"],
        Truth = new(Evidence.Run("This fix's writer got such a brief (DYD-1372 phase 1, fix g1a), on a ticket already In Progress: \"You are a fresh writer: the writer who built this part ran out of context " +
                                 "and started nothing on this fix. Start from the files.\", with the checkout's head, `e7bbb2b0`."),
            Layer: "A ticket shows that it was started and nothing of who holds it, so a fresh agent cannot tell a hand-over from a second dispatch.",
            Reason: "Without the word, an agent sent to resume dead work sends the dispatch back as a conflict, as it should.")
    };

    public static readonly Claim Author = new("A review brief names the model family that wrote the candidate.", Me, [])
    {
        Spec = ["R15", "R34"],
        Truth = new(Evidence.Tool("delegate_task: `target.model` and `target.providerInstanceId` are set by the dispatcher, so it knows which family it seated as writer. " +
                                  "Linear, DYD-1372 read 2026-11-21: the ticket shows no assignee, no delegate and no thread, so nothing on it names the writer's model."),
            Layer: "A reviewer is handed a SHA and a ticket, and neither names the model that wrote the work.",
            Reason: "A review captain left to guess the writer's family fills its seats blind, and may seat that same family.")
    };

    public static readonly Claim Candidate = new("A review brief names the candidate by its SHA.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefReview, 1, "The candidate, by its SHA, and the model family that wrote it.", () => [Author])])
    {
        Spec = ["K14", "R17"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` merges only the SHA a PASS names, so the reviewer has to be told which one to judge.",
            Reason: "A verdict on a moving branch is a verdict on nothing.")
    };

    public static readonly Claim Judged = new("A review brief names the ticket whose work is judged, by its key, apart from the Review ticket's own.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefReview, 2,
            "The ticket whose work is judged, by its key, apart from the Review ticket's own: the round's holder files each should under it.")
    ])
    {
        Spec = ["S17", "K14", "S4"],
        Truth = new(Evidence.Tool("save_issue: `parentId`, \"Parent issue ID or identifier\", which whoever files a should must fill. Run, Linear, DYD-1281's sub-tickets read 2026-11-21: " +
                                  "Review r1 to r4 (DYD-1294, DYD-1328, DYD-1334, DYD-1340) are children of DYD-1281 beside the part and Fix tickets, so a check of one part has the whole as its parent."),
            Layer: "Linear gives a Review ticket one parent, and for a leaf's check that is the leaf's own parent: nothing on the ticket, the SHA or the PR says which sibling the candidate belongs to.",
            Reason: "Filed under the wrong card, a finding escapes the duplicate search and whoever later gathers that part's leftovers.")
    };

    public static readonly Claim Pr = new("A review brief names the PR the candidate is in and its base, or says that the candidate has no PR.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefReview, 3,
            "The PR the candidate is in, by its number, and the base's SHA: a candidate you will merge is in its PR, a draft you opened by [Merging](merging.md), before you brief its Review, since `dydo merge` reads a PASS only from that PR's body. " +
            "The candidate is also that PR's head: after a Fix, a mended should or a resolved conflict, push your branch with `git push` and see `dydo pr-ready` pass again, " +
            "since `dydo merge` refuses a PR whose head is not the reviewed SHA. " +
            "Only a candidate somebody else will have reviewed in a PR has no PR, a leaf's evidence check or the round of a part its officer merges, and its brief says so.",
            () => [PrFirst!, Pushed!])
    ])
    {
        Spec = ["R17", "K14"],
        Truth = new(Evidence.Code("Services/Merge/IndependentReview.cs"),
            Layer: "`dydo merge` reads a PASS only from the body of the PR it is given, and a reviewer handed a bare SHA cannot tell which PR that is, or whether there is one.",
            Reason: "A reviewer that knows there is no PR writes its PASS where it will be read, and never hunts for a body to edit.")
    };

    public static readonly Claim PrFirst = new("A candidate its holder will merge is in its PR, a draft, before its Review is briefed; only a candidate somebody else will have reviewed in a PR is reviewed outside one.", Me, [])
    {
        Spec = ["R17", "R23", "R2"],
        Truth = new(Evidence.Run("`dydo pr-ready`, run with no body in DYD-1372's checkout 2026-11-21: it answered \"not ready: HEAD differs from its upstream\", a fact of the tree, and asked for no review; " +
                                 "`Commands/PrReadyCommand.cs` reads a body only under `--body`. So a PR can be opened, as a draft, before any round. " +
                                 "`Services/Merge/PullRequestMerge.cs` takes its PASS from the PR's body and refuses with \"holds no PASS for candidate\" when it is absent."),
            Layer: "`dydo merge` tells a captain that its PR holds no PASS only at the merge, after the reviewer that could have written the line has returned.",
            Reason: "A verdict written where the command never looks buys no merge, and the whole is judged a second time for the sake of one line.")
    };

    public static readonly Claim Pushed = new("The candidate of a Review in its holder's PR is that PR's head, pushed, before the Review is briefed: each commit taken in since the PR was opened is pushed first.", Me, [])
    {
        Spec = ["R23", "R17"],
        Truth = new(Evidence.Run("`Services/Merge/PullRequestMerge.cs` refuses a PR whose hosted head is not the SHA it is given: \"PR #… head is …, not the reviewed …\". " +
                                 "`dydo pr-ready`, run with no body file in DYD-1372's checkout at `951addda`, clean and pushed, 2026-11-21: \"ready: … is clean and HEAD matches its upstream\", exit 0; " +
                                 "`Services/PrReadyCheck.cs` answers a head ahead of its upstream with \"HEAD differs from its upstream\". `git push --dry-run origin HEAD`, typed there the same day on the ticket's branch, " +
                                 "passed the guard and answered \"Everything up-to-date\": `Commands/GuardCommand.MergeRoutes.cs` refuses only a push that updates the default branch, " +
                                 "and `Services/BashCommandAnalyzer.cs` a forced or deleting one."),
            Layer: "The guard lets a ticket branch be pushed and never asks for it; the tree check ran once, before the PR was opened, and `dydo merge` names a stale head only at the merge.",
            Reason: "Left local, the reviewed commit meets the host only at the merge: the command refuses there, and hosted CI has yet to run on what the reviewer passed.")
    };

    public static readonly Claim Governing = new("A review brief names the governing spec, or says there is none.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefReview, 4, "The governing spec, or that there is none.")])
    {
        Spec = ["R3"],
        Truth = new(Evidence.Run(OwnBrief + "it named the approved spec's file and the sections to read."),
            Layer: "A ticket links its contract; nothing leads a reviewer from there to the spec.",
            Reason: "A review held only to the contract passes a wrong rule the contract inherited.")
    };

    public static readonly Claim LeafCheck = new("A leaf's change gets no full review, only an optional evidence check: a Review ticket of its own beside the leaf's, for a fresh seat.", Me, [])
    {
        Spec = ["S4", "R6"],
        Truth = new(Evidence.Run("Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4 are each a ticket of their own, children of DYD-1281 beside the Fix and part tickets they judge, " +
                                 "and none sits under one of those. `" + Toolset + "` holds the leaf's row: \"a lane or hop, optional seat\"."),
            Layer: "The level table has a row for a leaf, and nothing keeps a dispatcher from asking more of one or from leaving the check without a ticket.",
            Reason: "The review of the whole covers each part, so a full review of one part is paid for twice.")
    };

    public static readonly Claim Level = new("A review brief names the level, which follows the candidate and never the size of the ticket briefed.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BriefReview, 5,
            "The level, which follows the candidate and never the size of the ticket you brief: the evidence check of a leaf's change, a ticket review, or a feature or Milestone review. " +
            "A leaf's change gets at most the check, never a full review: open the check as a Review ticket of its own beside the leaf's, for a fresh seat.",
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
        At(Actor.TicketCaptain, CaptainSlot.BriefReview, 6,
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

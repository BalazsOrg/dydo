namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Implement kind resource (DYD-1372, option A: DYD-1295 0f340abd): what
/// a captain holding a change does as its writer. Its facts come from the implementer skill writers run
/// today, each checked again against the running system.
/// </summary>
public static class CaptainImplement
{
    private const string Legacy = "skills/roles/crew/implementer/SKILL.md";
    private const string OwnBrief = "This skill's writer was briefed (DYD-1372 hop 2): ";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Change = new("An Implement-kind ticket is a change: a contract implemented, a review's findings fixed, or a merge conflict resolved.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementLead, 1, "Your ticket is a change: implement a contract, fix a review's findings, or resolve a merge conflict.")])
    {
        Spec = ["K31"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "The ticket names which of the three it is; that one resource serves all three is said only here.",
            Reason: "A writer sees at once that the rest of the page is its whole recipe.")
    };

    public static readonly Claim Verify = new("A writer verifies its work while it builds it, each file the way it is used.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementLead, 2,
            "Verify your work while you build it, each file the way it is used: run the code and drive the app, render an image and look at it, hold a doc against its source.")
    ])
    {
        Spec = ["K31"],
        Truth = new(Evidence.Run("This skill was written so: its writer rebuilt after each edit and read the built files, never the claims alone."),
            Layer: "Gates run at the end, and only on what they were told to check.",
            Reason: "A fault caught while the file is open costs a minute; caught at review, a round.")
    };

    public static readonly Claim NoWorse = new("A writer leaves what it touched no worse than it found it.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 1, "Leave what you touched no worse than you found it.")])
    {
        Spec = ["K31"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "No gate compares a file with how it read before the change.",
            Reason: "Decay that every change adds a little of is never anyone's ticket.")
    };

    public static readonly Claim Limit = new("Every changed file stays within the file token limit, tests included.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 2, "Keep every file you change at or under 20,000 bytes (5k tokens at bytes ÷ 4), tests included, and split one that passes it.")])
    {
        Spec = ["T15"],
        Truth = new(Evidence.Code("tools/SkillBuild/changed-file-sizes.sh"),
            Layer: "The size check runs as a gate, after the file is already written.",
            Reason: "A file an agent can read whole is one it can change without missing half of it.")
    };

    public static readonly Claim Paths = new("One writer per worktree, committing only inside its ticket's owned paths.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 3, "One writer per worktree: commit only inside the paths your ticket owns.")])
    {
        Spec = ["R32", "H3"],
        Truth = new(Evidence.Run(OwnBrief + "\"You are the only writer in it\" and \"Commit only owned paths\"."),
            Layer: "git takes any path from any writer; the ticket's paths are known only to its contract.",
            Reason: "A commit that strays into another ticket's files is a conflict someone else pays for.")
    };

    public static readonly Claim Skills = new("A writer loads domain-modeling for a glossary term or a Decision Record, and diagnosing-bugs for a Bug with no red reproduction.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 4, "Load /domain-modeling when you touch a glossary term or a Decision Record, and /diagnosing-bugs for a Bug with no red reproduction yet.")])
    {
        Spec = ["K14", "K31"],
        Truth = new(Evidence.Code("skills/engineering/domain-modeling/SKILL.md"),
            Layer: "A brief names the skills its writer foresaw; these two depend on what the work turns up.",
            Reason: "Each holds a method the writer would otherwise improvise.")
    };

    public static readonly Claim Design = new("An open design question is not the writer's to settle; a prototype answers one.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 5, "An open design question is a Question sub-ticket, never yours to settle; /prototype is the skill that answers one.")])
    {
        Spec = ["H16"],
        Truth = new(Evidence.Code("skills/engineering/prototype/SKILL.md"),
            Layer: "Nothing stops a writer from choosing a design by building it.",
            Reason: "A design chosen in passing is found at review, after it has been built on.")
    };

    public static readonly Claim Red = new("A Bug starts from a red reproduction, and each new test is shown failing without the code.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 6, "A Bug starts from a red reproduction, and every new test is shown failing without the code.")])
    {
        Spec = ["T3"],
        Truth = new(Evidence.Run("This ticket's generator change was made so: two tests failed, on the Resources list and on an unlinked resource, before `SkillRenderer` changed."),
            Layer: "A test runner reports green; it cannot say whether the test could ever have been red.",
            Reason: "A test never seen failing proves nothing about the code it sits beside.")
    };

    public static readonly Claim Weed = new("A mistake found after it landed gets its fix and a lint, test or rule that keeps it from recurring.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 7, "A fix for a mistake that had already landed carries the lint, test or rule that keeps it from coming back.")])
    {
        Spec = ["R30"],
        Truth = new(Evidence.Code("tools/SkillBuild/TruthCheck.cs"),
            Layer: "A fix closes its ticket whether or not anything stops the same mistake next time.",
            Reason: "The candidate's lists numbered `1. 1. 1.` reached the user once; the build now fails on one.")
    };

    public static readonly Claim Source = new("On a Fix, each finding's severity is checked against the source, not its label.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementFix, 1, "Check each finding's severity against the source, not its label.")])
    {
        Spec = ["S9", "R11"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "The Fix ticket carries the reviewer's label and none of the code behind it.",
            Reason: "A mislabelled finding is otherwise fixed as the wrong thing.")
    };

    public static readonly Claim Only = new("A Fix closes only the findings its brief names, its blockers and any cheap should beside them, preferring the change that deletes or narrows code.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementFix, 2, "Close only the findings your brief names, preferring the change that deletes or narrows code.")])
    {
        Spec = ["S9", "S17"],
        Truth = new(Evidence.Run("This Fix came so (DYD-1372 fix r1b): its brief listed five blockers and four shoulds by name, \"Shoulds, fix them all\", and closed with \"Report anything you find beyond this list.\""),
            Layer: "Nothing bounds what a fixer changes while it is in the file.",
            Reason: "Review never grows the change, so the next round has less to read, not more.")
    };

    public static readonly Claim Dispute = new("A fixer puts a label it disputes in its return, with evidence.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementFix, 3, "Put a label you dispute in your return, with its evidence: file:line, test or output.")])
    {
        Spec = ["R11"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A severity the fixer silently ignores looks, on the ticket, like a blocker left open.",
            Reason: "The dispute reaches the record, where the next reviewer can weigh it.")
    };

    public static readonly Claim Branch = new("A conflict is resolved on the ticket's branch only, as a merge commit that keeps both parents.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementConflict, 1, "Resolve it on the ticket's branch only, as a merge commit that keeps both parents.")])
    {
        Spec = ["R19", "R32"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` merges only a head that carries a PASS, so a resolution made anywhere else never reaches review.",
            Reason: "Both parents keep each side's history readable when the resolution is questioned.")
    };

    public static readonly Claim Hunks = new("A resolver reads both sides and what changed since review before it resolves a hunk; a new product choice goes back as a Question.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementConflict, 2, "Read both sides, and what changed since the source was reviewed, before you resolve a hunk; one that needs a new product choice is a Question.")])
    {
        Spec = ["R19", "H16"],
        Truth = new(Evidence.Code("skills/roles/crew/implementer/resources/merge.md"),
            Layer: "git marks where two sides differ and nothing of what either side meant.",
            Reason: "A hunk resolved by picking a side can drop behaviour the other side was reviewed for.")
    };

    public static readonly Claim Reparent = new("A shoulds batch's captain moves each should under its ticket and keeps a link to where it was found.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementShoulds, 1, "Move each should under your ticket, keeping a `related: found in <KEY> rN` link on it.")])
    {
        Spec = ["S17"],
        Truth = new(Evidence.Run("Linear, DYD-1281 read 2026-11-20: a ticket carries `relatedTo` links beside its parent."),
            Layer: "Linear drops the old parent when a ticket is moved.",
            Reason: "The link is the only trace of which review found it.")
    };

    public static readonly Claim Cancel = new("A should is cancelled only with a one-line reason.", Me, [])
    {
        Spec = ["S18", "S17"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-20: Canceled exists."),
            Layer: "Linear cancels a ticket without asking why.",
            Reason: "A should is a real fault, so dropping one has to be answerable.")
    };

    public static readonly Claim Duplicate = new("A duplicate should is marked Duplicate.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementShoulds, 2, "Mark a duplicate Duplicate, and cancel a should only with a one-line reason.", () => [Cancel])])
    {
        Spec = ["S17"],
        Truth = new(Evidence.Run("Linear, team Dydo, statuses read 2026-11-20: Duplicate exists."),
            Layer: "Shoulds from different reviews can name one fault, and nothing compares them.",
            Reason: "One fault is then fixed once and counted once.")
    };

    public static readonly Claim Commit = new("A writer's hop ends on a commit named for its ticket and hop, never squashed.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementEnd, 1, "End on a commit `<KEY> <hop>: <what>`, `<hop>` being `implement`, `fix` or `merge`, never squashed.")])
    {
        Spec = ["R32"],
        Truth = new(Evidence.Run("`git log` on this ticket's branch, 2026-11-20: \"DYD-1372 implement: …\" and, below it, \"DYD-1339 fix: …\"."),
            Layer: "git takes any message, and a squash erases the hops a review was run against.",
            Reason: "A reviewer finds the exact commit each hop ended on.")
    };

    public static readonly Claim Receipts = new("A writer's receipts are the commands it ran on its commit and their results.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementEnd, 2, "Your receipts are the commands you ran on that commit and their results.")])
    {
        Spec = ["R5"],
        Truth = new(Evidence.Run(OwnBrief + "its return lists \"the gate commands with their exits\"."),
            Layer: "A return that says only that it passed gives the officer nothing to check.",
            Reason: "A reviewer then judges through other lenses instead of running the same thing again.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

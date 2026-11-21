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
    private const string MergeRecipe = "skills/roles/crew/implementer/resources/merge.md";

    private const string Map = """
        | Spine phase | You |
        | -- | -- |
        | 1. Take the ticket | use |
        | 2. Do it or split it | use: a leaf, unless the change proves too big |
        | 3. Dispatch | skip; use it only for a Question's captain, when the merge is yours, or once you split |
        | 4. Get it reviewed | skip, since a part is reviewed with its whole; use it only if the merge is yours, or you split |
        | 5. Finish | use: the case your brief names |
        | When things go wrong | use |
        """;

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

    public static readonly Claim Parts = new("The implement resource heads each part with the work it is for, and a holder reads the parts that name its own.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementLead, 3, "Below the rules for every change, each part is headed by the work it is for: read the ones that name yours.")])
    {
        Spec = ["K31", "K30"],
        Truth = new(Evidence.Code("tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs"),
            Layer: "A page shows every recipe to every reader, and only the reader knows whether its ticket is a Bug, a doc or a conflict.",
            Reason: "A writer on a small Fix reaches its three lines without a detour through recipes meant for somebody else.")
    };

    public static readonly Claim Phases = new("An Implement holder uses the spine's first, second and last phases, and dispatches or gets a review only when the merge is its own or it splits.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementMap, 1, Map)])
    {
        Spec = ["K30", "R2"],
        Truth = new(Evidence.Code("claims/statechart/captain/transitions.json"),
            Layer: "The spine is one text for every kind of ticket, and nothing in it marks the phases a kind leaves out.",
            Reason: "A writer told at the top that sending and review belong to others goes straight to the change.")
    };

    public static readonly Claim NoWorse = new("A writer leaves what it touched no worse than it found it.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 1, "Leave what you touched no worse than you found it.")])
    {
        Spec = ["K31"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "No gate compares a file with how it read before the change.",
            Reason: "Decay that every change adds a little of is never anyone's ticket.")
    };

    public static readonly Claim Limit = new("Every changed file stays within the file size limit the brief's gates check, tests included.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 2, "Keep every file you change, tests included, within the file size limit your brief's gates check, and split one that passes it.")])
    {
        Spec = ["T15", "T14"],
        Truth = new(Evidence.Run("This hop's brief named the gate among its gates: `bash tools/SkillBuild/changed-file-sizes.sh`, which exits 1 on a changed file over its limit. " +
                                 "The briefs resource has every change's brief name \"the project's gates it must pass\"."),
            Layer: "The size check runs as a gate, after the file is already written.",
            Reason: "A file an agent can read whole is one it can change without missing half of it.")
    };

    public static readonly Claim LimitNumber = new("The file size limit is 5k tokens, counted as bytes ÷ 4, unless the project's configuration sets another.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 8, "Unless your project's configuration says otherwise, the limit is 5k tokens, counted as bytes ÷ 4.")])
    {
        Spec = ["T15", "T14"],
        Truth = new(Evidence.Open("The number is this repository's alone as the system runs: only `tools/SkillBuild/changed-file-sizes.sh` and this suite check 20,000 bytes. " +
                                  "T14 makes the number configuration, and no key of `dydo.json`, no command of the CLI and no file of the scaffold holds it, so nothing tells another project's holder its limit."),
            Layer: "No shipped gate or setting carries the number yet, so a holder elsewhere would learn it only from this page.",
            Reason: "A number stated here would bind every adopting project to this repository's choice.")
    };

    public static readonly Claim Paths = new("One writer per worktree, committing only inside its ticket's owned paths.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 3, "One writer per worktree: commit only inside the paths your ticket owns.")])
    {
        Spec = ["R32", "H3"],
        Truth = new(Evidence.Run(OwnBrief + "\"You are the only writer in it\" and \"Commit only owned paths\"."),
            Layer: "git takes any path from any writer; the ticket's paths are known only to its contract.",
            Reason: "A commit that strays into another ticket's files is a conflict someone else pays for.")
    };

    public static readonly Claim Seam = new("A writer loads codebase-design when it designs a seam or an interface.", Me, [])
    {
        Spec = ["K14", "K31"],
        Truth = new(Evidence.Code("skills/engineering/codebase-design/SKILL.md"),
            Layer: "A brief names the skills its dispatcher foresaw, and a seam shows up only once the work is open.",
            Reason: "Where a boundary is drawn decides how testable everything behind it will be.")
    };

    public static readonly Claim Skills = new("A writer loads domain-modeling when it touches a glossary term or a Decision Record.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementRules, 4,
            "Load /domain-modeling when you touch a glossary term or a Decision Record, and /codebase-design when you design a seam or an interface.", () => [Seam])
    ])
    {
        Spec = ["K14", "K31"],
        Truth = new(Evidence.Code("skills/engineering/domain-modeling/SKILL.md"),
            Layer: "A brief names the skills its writer foresaw; these depend on what the work turns up.",
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
        Truth = new(Evidence.Code(MergeRecipe),
            Layer: "git marks where two sides differ and nothing of what either side meant.",
            Reason: "A hunk resolved by picking a side can drop behaviour the other side was reviewed for.")
    };

    public static readonly Claim Parents = new("A resolver returns both parents with the merge commit's SHA.", Me, [])
    {
        Spec = ["R19", "R32"],
        Truth = new(Evidence.Code(MergeRecipe),
            Layer: "A merge commit holds its parents, and a return that names only the new head makes its reader dig them out.",
            Reason: "The fresh reviewer of the hunks compares the resolution with each side it came from.")
    };

    public static readonly Claim Intent = new("A resolver states the behaviour each resolved hunk is meant to have.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementConflict, 3, "State the behaviour each hunk you resolved is meant to have, and return both parents with the merge commit's SHA.", () => [Parents])])
    {
        Spec = ["R19"],
        Truth = new(Evidence.Code(MergeRecipe),
            Layer: "A resolved hunk looks like any other code, with nothing to show that two intents met there.",
            Reason: "Its reviewer judges the hunk against a stated intent, and need not guess one.")
    };

    public static readonly Claim Refactor = new("A resolution that refactors leaves the code no worse than either side.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementConflict, 4, "A resolution that refactors leaves the code no worse than either side had it.")])
    {
        Spec = ["R19", "K31"],
        Truth = new(Evidence.Code(MergeRecipe),
            Layer: "git accepts any content as a resolution, well beyond the lines that were in conflict.",
            Reason: "A conflict is where two designs meet, and the lazy way out keeps the weaker of them.")
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

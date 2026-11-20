namespace DynaDocs.SkillBuild;

/// <summary>
/// The implementer's own claims (K31): its job weighted properly, verifying while it builds, the checks
/// before the first edit, and its code, docs, Bug and merge recipes, carried over from the pre-candidate
/// code-writer and docs-writer processes in fresh prose (K22).
/// </summary>
public static class Implementer
{
    public static readonly Claim Weight = new("Most hops implement a contract or fix blockers; a merge conflict is the smallest part (DYD-1082 ae8e285a).", [Actor.Implementer],
        [new(Skill.Implementer, "Most hops implement a ticket's contract or fix a review's blockers; resolving a merge conflict is the occasional, smallest part.", new(SpinePart.Job, 105))])
    { Spec = ["K31"] };

    public static readonly Claim Verify = new("The implementer verifies its own work while it builds, with every tool, driving included (DYD-1082 a1b21b59 Q10).", [Actor.Implementer],
        [new(Skill.Implementer, "Verify your own work while you build it, with every tool the project has, driving the app included, so you hand off work you are confident in; how you work is yours to decide.", new(SpinePart.Job, 106))])
    { Spec = ["K31", "R5"] };

    public static readonly Claim ByUse = new("Each file is checked the way it is used, such as an image rendered and looked at (DYD-1082 a1b21b59 Q10).", [Actor.Implementer],
        [new(Skill.Implementer, "Check each file the way it is used: run the code, render an image and look at it, hold a doc against its source.", new(SpinePart.Job, 107))])
    { Spec = ["K31"] };

    public static readonly Claim Ground = new("Before the first edit the writer checks its branch, worktree, base and a clean tree, and returns a mismatch.", [Actor.Implementer],
        [new(Skill.Implementer, "Before the first edit, check that HEAD is on the brief's branch in its own worktree, the base SHA is an ancestor of HEAD and the tree is clean; return a mismatch rather than work around it.", new(DispatchState.Briefed, 3102, MoveKind.Nominal))])
    { Spec = ["N12 DR 045 §8", "K29"] };

    public static readonly Claim Recipe = new("The implementer works from its code, docs, Bug or merge recipe (DYD-1082 1bc04dce change 4).", [Actor.Implementer],
        [new(Skill.Implementer, "Open the recipe below for your work before you start it: code, docs or both, a Bug, or a merge conflict.", new(ImplementerPart.Build, 100))])
    { Spec = ["K31"] };

    public static readonly Claim NoWorse = new("The writer leaves what it touched no worse than it found it.", [Actor.Implementer],
        [new(Skill.Implementer, "Leave what you touched no worse than you found it.", new(ImplementerPart.Build, 2303))])
    { Spec = ["T13", "K22"] };

    public static readonly Claim Glossary = new("A glossary term or a Decision Record is touched with domain-modeling, as the pre-candidate code-writer did.", [Actor.Implementer],
        [new(Skill.Implementer, "Load /domain-modeling when you touch a glossary term or a Decision Record.", new(ImplementerPart.Build, 2304))])
    { Spec = ["K14", "K22"] };

    public static readonly Claim OpenDesign = new("An open design question stops the build and goes back to the captain, as the pre-candidate code-writer's prototype trigger did.", [Actor.Implementer],
        [new(Skill.Implementer, "When a design question is open, stop and return it to your captain; /prototype is the skill that answers one.", new(ImplementerPart.Build, 2305))])
    { Spec = ["K22", "K31"] };

    public static readonly Claim Gherkin = new("The implementer writes and wires each acceptance line the contract marks for Gherkin as a feature-file scenario, never weakening it.", [Actor.Implementer],
        [new(Skill.Implementer, "Write each acceptance line the contract marks for Gherkin as a feature-file scenario at the product's boundary, in glossary words, with an example table only where values vary and every column changes an outcome, wired through step definitions; add or remove a scenario only when asked, and never weaken one.", new(ImplementerPart.Code, 2214))])
    { Spec = ["K22", "K31"] };

    public static readonly Claim Platforms = new("Platform-specific code is proved on CI's platform too, as the pre-candidate code-writer did.", [Actor.Implementer],
        [new(Skill.Implementer, "Prove platform-specific code, such as processes, paths and signals, on CI's platform too, not only on yours.", new(ImplementerPart.Code, 2215))])
    { Spec = ["K22", "R5"] };

    public static readonly Claim Design = new("A seam or interface is designed with codebase-design.", [Actor.Implementer],
        [new(Skill.Implementer, "Load /codebase-design when you design a seam or an interface.", new(ImplementerPart.Code, 2190))])
    { Spec = ["K14", "K22"] };

    public static readonly Claim Mocks = new("A mock sits only at the system's edge; the project's own modules stay real.", [Actor.Implementer],
        [new(Skill.Implementer, "Mock only at the system's edge, such as external APIs, time and randomness; your own modules stay real.", new(ImplementerPart.Code, 2192))])
    { Spec = ["T4", "K22"] };

    public static readonly Claim Witness = new("Every sentence of a doc needs a witness in the repository; a missing one is returned as a gap.", [Actor.Implementer],
        [new(Skill.Implementer, "Every sentence you write is a claim that needs a witness in the repository: code, configuration or governing intent (the spec, a DR, a ticket); where none exists, return the gap instead of the sentence.", new(ImplementerPart.Docs, 2390))])
    { Spec = ["D2", "K22"] };

    public static readonly Claim OneHome = new("Each meaning has one home, and everywhere else links to it.", [Actor.Implementer],
        [new(Skill.Implementer, "Write each meaning in one home and link to it from everywhere else.", new(ImplementerPart.Docs, 2391))])
    { Spec = ["K28"] };

    public static readonly Claim BuiltSource = new("A built file changes only in its source, then the build.", [Actor.Implementer],
        [new(Skill.Implementer, "A built file changes only in its source: edit the model or the generator, then rebuild.", new(ImplementerPart.Docs, 2392))])
    { Spec = ["K6"] };

    public static readonly Claim AgentDocs = new("Agent-facing docs are written with writing-for-agents.", [Actor.Implementer],
        [new(Skill.Implementer, "Write a skill, its resources or an entry file with /writing-for-agents.", new(ImplementerPart.Docs, 2393))])
    { Spec = ["K14", "K22"] };

    public static readonly Claim DocsProof = new("A docs change is proved with dydo check and the ticket's checks; dydo fix follows an added, moved or renamed doc.", [Actor.Implementer],
        [new(Skill.Implementer, "Prove a docs change with `dydo check` and the checks the ticket names; run `dydo fix` after you add, move or rename a document.", new(ImplementerPart.Docs, 2410))])
    { Spec = ["D5", "K13"] };

    public static readonly Claim BugSeam = new("A Bug names expected and observed behaviour at the highest stable seam, and what must not change.", [Actor.Implementer],
        [new(Skill.Implementer, "Name the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.", new(ImplementerPart.Bug, 3120))])
    { Spec = ["T3", "T4"] };

    public static readonly Claim BugRed = new("A Bug is reproduced red at the boundary where it shows; an existing red test is its reproduction.", [Actor.Implementer],
        [new(Skill.Implementer, "Reproduce it red: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction.", new(ImplementerPart.Bug, 3121))])
    { Spec = ["T3", "T4"] };

    public static readonly Claim BugDiagnose = new("A Bug with no red reproduction yet loads diagnosing-bugs, as the pre-candidate code-writer did (DYD-1287 contract); its dispatcher names the skill.",
        [Actor.Implementer, Actor.TicketCaptain],
    [
        new(Skill.Implementer, "With no red reproduction yet, load /diagnosing-bugs.", new(ImplementerPart.Bug, 3122)),
        new(Skill.TicketCaptain, "On a Bug, add /diagnosing-bugs to the skills this brief names.", new(Section.ImplementerBrief, 3122))
    ])
    { Spec = ["T3", "K14"] };

    public static readonly Claim BugUnused = new("An unused member's reproduction is its definition and a reference search; the fix deletes it with its test-only callers.", [Actor.Implementer],
        [new(Skill.Implementer, "An unused member needs no test: its definition at `file:line` and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.", new(ImplementerPart.Bug, 3123))])
    { Spec = ["T3", "K22"] };

    public static readonly Claim BugRevert = new("A Bug fix is reverted to watch the reproduction fail, then restored; the return carries that red line.", [Actor.Implementer],
        [new(Skill.Implementer, "Fix it, then revert the fix, watch the reproduction fail and restore it; return that red line, with the behaviour you kept still green.", new(ImplementerPart.Bug, 3124))])
    { Spec = ["T3"] };

    public static readonly Claim BugRace = new("A race is forced with fake timers or interleaving; a test needing a sleep or timeout is wrong.", [Actor.Implementer],
        [new(Skill.Implementer, "Force a race with fake timers or a set interleaving; a test that needs a sleep or a timeout is wrong.", new(ImplementerPart.Bug, 3125))])
    { Spec = ["T1", "K22"] };

    public static readonly Claim MergeRead = new("A conflict is resolved after reading both sides and what changed since the source's review.", [Actor.Implementer],
        [new(Skill.Implementer, "Read both sides, and what changed since the source was reviewed, before you resolve a hunk.", new(FeatureMergeState.Resolving, 3130, MoveKind.Nominal))])
    { Spec = ["R19", "K22"] };

    public static readonly Claim MergeIntent = new("Each hunk's intended behaviour is stated; a new product choice goes back to the captain.", [Actor.Implementer],
        [new(Skill.Implementer, "State each hunk's intended behaviour; one that needs a new product choice goes back to your captain.", new(FeatureMergeState.Resolving, 3131, MoveKind.Nominal))])
    { Spec = ["R19", "K22"] };

    public static readonly Claim MergeNoWorse = new("A resolution that refactors leaves the code no worse than either side.", [Actor.Implementer],
        [new(Skill.Implementer, "A resolution that refactors leaves the code no worse than either side.", new(FeatureMergeState.Resolving, 3132, MoveKind.Nominal))])
    { Spec = ["R19", "K22"] };

    public static readonly Claim MergeReturn = new("A resolved conflict returns both parents and the merge commit's SHA.", [Actor.Implementer],
        [new(Skill.Implementer, "Return both parents and the merge commit's SHA.", new(FeatureMergeState.Resolving, 3133, MoveKind.Nominal))])
    { Spec = ["R32"] };
}

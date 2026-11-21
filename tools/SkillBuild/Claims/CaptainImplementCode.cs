namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Implement kind, the practice of writing code and its tests (DYD-1372
/// aa5b445e: "how tests should be made, what the comments should be like, what they should not be
/// like, cognitive complexity gate, best practices etc (and that may be different from project to
/// project even)"). What a test is for is the same in every project (spec T1 to T7, T22); a project's
/// commands and its rules for code and comments are its own, so those lines name the doc that holds
/// them, a path this repository and the scaffold both have, and never a number. The gates a change
/// must pass are the ones its brief names: a project doc that disagrees with them is reported to the
/// officer and never obeyed in their place, because such a doc can be stale (DYD-1372 g1b, G6).
/// </summary>
public static class CaptainImplementCode
{
    private const string Legacy = "skills/roles/crew/implementer/resources/code.md";
    private const string Standards = "dydo/guides/coding-standards.md";
    private const string Guide = "dydo/guides/testing-strategy.md";
    private const string Both = ", here and under `Scaffold/`";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Jobs = new("A suite test does one of three jobs: it guards a trap, pins a traced contract, or reaches an edge that driving cannot.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 1,
            "Write a suite test for one of three jobs only: to guard a trap, to pin a contract the spec or a decision record traces, or to reach an edge that driving the app cannot.")
    ])
    {
        Spec = ["T1"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A test runner counts every green test alike, the one that earns its place and the one that only restates the code.",
            Reason: "Green tests can cement behaviour nobody wanted, and each needless one is a cost on every later change.")
    };

    public static readonly Claim Boundary = new("A test drives the deepest module boundary, or the product's, that can observe the behaviour; a unit test is kept only for an edge or trap that test cannot reach cheaply.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 2,
            "Test at the deepest module boundary, or the product's, that can observe the behaviour; add a unit test only for an edge or trap that test cannot reach cheaply.")
    ])
    {
        Spec = ["T4"],
        Truth = new(Evidence.Run("This hop's own tests are made so: each goes through `SkillBuilder.Build` on the production model and reads the built files, and none calls a renderer's inner method."),
            Layer: "Nothing stops a writer from testing each private step, which passes today and breaks at the next refactor.",
            Reason: "One check at the boundary stands in for a thousand inside it, and survives when the inside is rewritten.")
    };

    public static readonly Claim Mocks = new("A writer mocks only at the system's edge; its own modules stay real.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 3, "Mock only at the system's edge, such as an external API, time or randomness; your own modules stay real.")])
    {
        Spec = ["T4"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A mocking library replaces any dependency it is pointed at, the writer's own code included.",
            Reason: "A test of mocked collaborators proves how the writer imagines them, never how they behave.")
    };

    public static readonly Claim Red = new("Every new test is shown failing without the code.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 4,
            "Show every new test failing without the code: set the code aside, run the test, see it fail for the reason it names, and put the code back.")
    ])
    {
        Spec = ["T3"],
        Truth = new(Evidence.Run("This hop was made so (DYD-1372 phase 1, hop B): its ten new tests ran red on the unchanged model, 10 failed of 13, before one claim was written."),
            Layer: "A test runner reports green; it cannot say whether the test could ever have been red.",
            Reason: "A test never seen failing proves nothing about the code it sits beside.")
    };

    public static readonly Claim Tag = new("Each suite test carries its job as a tag: trap, contract or edge.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 5,
            "Tag each suite test with its job, the way the tests beside it carry theirs: trap, naming the bug or pitfall; contract, citing the spec or decision record; or edge.")
    ])
    {
        Spec = ["T5"],
        Truth = new(Evidence.Run("This repository's suite, read 2026-11-21: `[Trait(\"Job\", \"contract: …\")]` and `[Trait(\"Job\", \"trap: …\")]` on each test. `" + Standards + "`" + Both +
                                 ", says every suite test carries a trap, traced contract or edge job tag; the tag's syntax is each test stack's own, which T5 leaves to the project."),
            Layer: "A lint can demand that a tag is present; what the test is for is known only to whoever wrote it.",
            Reason: "A reviewer checks the tag against the test, and a later reader learns why the test may not be deleted.")
    };

    public static readonly Claim Prune = new("No test changes whenever the code does, checks that a deleted feature is gone, or can never fail.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 6, "Write no test that changes whenever the code does, checks that a deleted feature is gone, or can never fail.")])
    {
        Spec = ["T22"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "All three kinds pass, so no run ever points at them.",
            Reason: "Each one adds upkeep and a false sense of cover while guarding nothing.")
    };

    public static readonly Claim Guidance = new("Property tests for edges and differential tests for rewrites are guidance, never a gate.", Me, [])
    {
        Spec = ["T7"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "No gate of the facade asks for either kind of test.",
            Reason: "They pay off on some code and are noise on the rest, which only the writer can tell apart.")
    };

    public static readonly Claim Conformance = new("A rewrite or wide refactor gets a conformance suite beside its own tests, and its matches-the-old-implementation assertions go once the rewrite has replaced the old code.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 7,
            "A rewrite or wide refactor gets a conformance suite beside its own tests, never in their place; its assertions that only match the old implementation go once the rewrite has replaced it. " +
            "Property tests for edges and differential tests for rewrites are guidance, never a gate.",
            () => [Guidance])
    ])
    {
        Spec = ["T6"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A suite written for a refactor looks like any other, and nothing marks which of its assertions outlive the old code.",
            Reason: "A writer who takes the suite for the refactor's only tests ships new behaviour with nothing of its own to hold it.")
    };

    public static readonly Claim Scenario = new("An acceptance line the contract marks for a scenario is written and wired as the project's coding standards say, and never weakened.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 8,
            "Where your contract marks an acceptance line for a scenario, write and wire it as `" + Standards + "` says of a scenario, and never weaken one.")
    ])
    {
        Spec = ["K31", "T1"],
        Truth = new(Evidence.Run("`" + Standards + "`" + Both + ", section 6, Testing: a scenario \"is contract where the ticket calls for Gherkin: the captain decides the set, " +
                                 "the implementer writes and wires it without weakening it, and the acceptance runner runs it.\""),
            Layer: "Which runner a project's scenarios go through, and in what words, is that project's choice.",
            Reason: "A scenario is the contract in runnable form, so loosening one changes what was agreed without anyone agreeing.")
    };

    public static readonly Claim Graduation = new("Proof tests live in the ticket's commits with their SHA in the receipt, and nothing depends on old evidence.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementTests, 9, "Keep proof tests in the ticket's commits, record their SHA in your receipt, and make nothing depend on old evidence.")])
    {
        Spec = ["T5"],
        Truth = new(Evidence.Open("T5 has the merge drop the proof tests that do not graduate, and no merge in this repository has been seen to: `dydo merge` (`Services/Merge`) reads no test's tag. " +
                                  "Until a run shows which tests a merge keeps, what a writer does with a proof test is not known."),
            Layer: "git keeps every test a commit adds, and nothing tells a proof of one ticket from a guard meant to stay.",
            Reason: "Evidence nobody will look at again is weight every later run carries.")
    };

    public static readonly Claim Rules = new("A project's own rules for writing code are in its coding standards, which a writer reads before it writes.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementCode, 1,
            "Your project's rules for writing code are in `" + Standards + "`: read it before you write.")
    ])
    {
        Spec = ["K31", "T14"],
        Truth = new(Evidence.Run("`" + Standards + "` exists" + Both + ", the scaffold dydo ships."),
            Layer: "This skill is one text for every project, and a project's conventions are written only in its own tree.",
            Reason: "Practice differs between projects, and a writer that knows only the general page breaks the local one.")
    };

    public static readonly Claim Comments = new("What a comment is for, and what it must never be, is the project's rule, in the Comments section of its coding standards.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementCode, 2,
            "Before you write a comment, read Comments in that file: your project says there what a comment is for and what it must never be.")
    ])
    {
        Spec = ["K31"],
        Truth = new(Evidence.Run("`" + Standards + "`" + Both + ", has a section headed Comments under Conventions: a reason the code cannot show, never a retelling of the code. " +
                                 "His words (DYD-1372 aa5b445e): \"what the comments should be like, what they should not be like\"."),
            Layer: "No compiler or lint reads a comment for whether it is worth having.",
            Reason: "A model left to its habit narrates every line, and the narration goes stale with the first edit.")
    };

    public static readonly Claim Gates = new("The gates a change must pass are the ones its brief names, run with the project's own commands, which the project's testing guide holds.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementCode, 3,
            "The gates your change must pass are the ones your brief names: run them with your project's own commands, which `" + Guide + "` holds.")
    ])
    {
        Spec = ["T14", "T11"],
        Truth = new(Evidence.Run("This fix's brief named its gates one by one (DYD-1372 g1b): the build with warnings as errors, the SkillBuild tests, a clean tree after a rebuild, the file size check and `dydo check`. " +
                                 "`" + Guide + "`" + Both + ": here its section \"Current project facade\" gives the runner, and the scaffold's copy is where an adopting project declares its own."),
            Layer: "The CLI has no command that runs a project's tests or its static checks, so which ones count for a ticket is known only from its brief.",
            Reason: "A writer that runs what was asked proves what its officer will check, and guesses at nothing.")
    };

    public static readonly Claim Disagree = new("Where a project doc and the brief's gates disagree, the gates decide what must pass, and the writer reports the difference to its officer as a finding.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementCode, 4,
            "Where a project doc and those gates disagree, the gates decide what must pass: report the difference to your officer as a finding, and chase no figure that no gate checks.")
    ])
    {
        Spec = ["T9", "T10", "T11", "R5"],
        Truth = new(Evidence.Run("Read 2026-11-21: `DynaDocs.Tests/coverage/gate_policy.py` fails a method on two limits only, cognitive complexity and parameters, and no file of the facade names HCRAP. " +
                                 "Yet `" + Standards + ":194` and `" + Guide + ":243` still demand 80% line coverage, 60% branch coverage and HCRAP at most 20. " +
                                 "Spec CA6 records the change: \"Resolved by PR 363 (`892d41f8`): CI coverage is report-only, and HCRAP no longer gates.\" " +
                                 "`.github/workflows/ci.yml` calls its step \"Full Python suite and platform coverage report\"."),
            Layer: "A doc is edited by hand and a gate in code, so the two drift apart, and nothing compares them.",
            Reason: "A writer that takes a stale page for law spends its ticket lifting a measure nobody enforces any more.")
    };

    public static readonly Claim Trip = new("A change that trips a static gate is changed until it passes: a gate takes no suppression, and its limit is never raised.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ImplementCode, 5,
            "When your change trips a static gate, such as the one on cognitive complexity, change the code until it passes, splitting or flattening the method it names: " +
            "a gate takes no suppression, and its limit is never raised.")
    ])
    {
        Spec = ["T11", "T19"],
        Truth = new(Evidence.Run("`" + Guide + "`, read 2026-11-21: \"there are no suppressions\", and a suppressed diagnostic is \"a finding or ignored input, never an escape\". " +
                                 "`" + Standards + "`" + Both + ": \"There are no tiers, tier annotations, tier registries, classic CRAP thresholds, per-file suppressions\"."),
            Layer: "An analyzer honours a suppression comment as readily as a fix.",
            Reason: "A gate exists only where a breach is certainly wrong, so an exception to it is a decision above any one ticket.")
    };

    public static readonly Claim Platform = new("Platform-specific code is proved on CI's platform too.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.ImplementCode, 6, "Prove platform-specific code, such as processes, paths and signals, on CI's platform too, not only on yours.")])
    {
        Spec = ["K31"],
        Truth = new(Evidence.Code(Legacy),
            Layer: "A local run passes on the one platform the writer happens to sit on.",
            Reason: "A path separator or a signal that differs elsewhere fails only after the merge, where a revert is the cure.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

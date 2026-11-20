namespace DynaDocs.SkillBuild;

/// <summary>Proof tests and gates (the Proof machine, DR 067): a test's three jobs, the red check, graduation and its tag, the gates on changed files, and the purge.</summary>
public static class Testing
{
    public static readonly Claim Write = Move(ProofState.Start, ProofState.Written, "a proof test is written for a hop", "T5");
    public static readonly Claim Legacy = Move(ProofState.Start, ProofState.Old, "an old test meets the purge", "T8", "T20");
    public static readonly Claim Rewrite = Move(ProofState.Start, ProofState.Conformance, "a rewrite gets a conformance assertion", "T6");
    public static readonly Claim ShowRed = Move(ProofState.Written, ProofState.Red, "shown failing without the code", "T3");
    public static readonly Claim RunGates = Move(ProofState.Red, ProofState.Gated, "the code goes in; the gates run on changed files", "T13", "T21", "Y6");
    public static readonly Claim Green = Move(ProofState.Gated, ProofState.Receipt, "all selected tests pass and the statics hold", "T23", "Y4", "T9");
    public static readonly Claim GoRed = Move(ProofState.Gated, ProofState.GateRed, "a selected test or a static gate fails", "T23", "T19");
    public static readonly Claim Refix = Move(ProofState.GateRed, ProofState.Gated, "fixed; the gates run again", "T23");
    public static readonly Claim Graduate = Move(ProofState.Receipt, ProofState.Graduated, "it graduates with its job tag", "T5", "T4");
    public static readonly Claim Drop = Move(ProofState.Receipt, ProofState.Dropped, "the merge drops a test that does not graduate", "T5");
    public static readonly Claim Untraced = Move(ProofState.Graduated, ProofState.Finding, "an untraced pin, or a missing job tag", "T1", "R3", "T5");
    public static readonly Claim Keep = Move(ProofState.Old, ProofState.Graduated, "it meets the new test requirements: kept", "T8", "T22");
    public static readonly Claim Rewritten = Move(ProofState.Old, ProofState.Written, "rewritten to meet them", "T8");
    public static readonly Claim Delete = Move(ProofState.Old, ProofState.Dropped, "deleted, with one line in the commit when it blocks 4.0.0 work", "T8", "T22");
    public static readonly Claim Traced = Move(ProofState.Conformance, ProofState.Red, "a traced assertion is shown failing without the code", "T6", "T3");
    public static readonly Claim MatchesOld = Move(ProofState.Conformance, ProofState.Dropped, "a matches-old-implementation assertion goes at the rewrite's landing", "T6");

    public static readonly Claim Jobs = new("A suite test guards a trap, pins a traced contract, or reaches an edge driving can't; an untraced pin is a finding.", [Actor.Implementer, Actor.Reviewer],
    [
        new(Skill.Implementer, "Write a suite test only to guard a trap, pin a traced contract (spec or DR), or reach an edge driving can't.", new(DispatchState.Working, 2201, MoveKind.Nominal)),
        new(Skill.Reviewer, "A test that pins an untraced rule, new or inherited, is a finding.", new(ReviewState.Round, 2201, MoveKind.Nominal))
    ])
    { Spec = ["T1", "R3"] };

    public static readonly Claim RedCheck = new("Every graduating test is shown failing without the code; a Bug starts from a red reproduction.", [Actor.Implementer],
        [new(Skill.Implementer, "Show every graduating test failing without the code; a Bug still starts from a red reproduction.", new(DispatchState.Working, 2202, MoveKind.Nominal))])
    { Spec = ["T3"] };

    public static readonly Claim Boundary = new("A test drives the deepest boundary that can observe the behaviour.", [Actor.Implementer],
        [new(Skill.Implementer, "Test at the deepest module boundary, or the product boundary, that can observe the behaviour; a unit test graduates only for an edge or trap that test can't reach cheaply.", new(DispatchState.Working, 2203, MoveKind.Nominal))])
    { Spec = ["T4"] };

    public static readonly Claim JobTag = new("A graduating test carries a job tag naming its evidence; the reviewer checks each tag.", [Actor.Implementer, Actor.Reviewer],
    [
        new(Skill.Implementer, "Tag each graduating test with its job: trap (naming the bug or pitfall), contract (citing the spec or DR) or edge.", new(DispatchState.Working, 2204, MoveKind.Nominal)),
        new(Skill.Reviewer, "Check each graduating test's job tag against its evidence.", new(ReviewState.Round, 2204, MoveKind.Nominal))
    ])
    { Spec = ["T5"] };

    public static readonly Claim ProofSha = new("Proof tests live in the ticket's commits; the receipt and proof: record the SHA.", [Actor.Implementer],
        [new(Skill.Implementer, "Keep proof tests in the ticket's commits, and record their SHA in the receipt and in `proof:`.", new(DispatchState.Working, 2205, MoveKind.Nominal))])
    { Spec = ["T5"] };

    public static readonly Claim OldEvidence = new("Old evidence is never load-bearing.", [Actor.Implementer],
        [new(Skill.Implementer, "Make nothing depend on old evidence: a squash or a fresh start must break nothing.", new(DispatchState.Working, 2206, MoveKind.Nominal))])
    { Spec = ["T5"] };

    public static readonly Claim ConformanceSuite = new("A rewrite gets a conformance suite, a separate rule in a separate place from the graduation rules.", [Actor.Implementer],
        [new(Skill.Implementer, "A rewrite or wide refactor gets a conformance suite, apart from its graduating tests and never instead of them: its traced assertions graduate like any graduating test, red first, and its 'matches old implementation' assertions go at the rewrite's landing.", new(ProofState.Conformance, 2207, MoveKind.Nominal))])
    { Spec = ["T6"] };

    public static readonly Claim Smells = new("Three smells are pruned: tests that change whenever the code does, check that a deleted feature is gone, or can never fail.",
        [Actor.Implementer, Actor.Reviewer],
    [
        new(Skill.Implementer, "Write no test that changes whenever the code does, checks that a deleted feature is gone, or can never fail.", new(DispatchState.Working, 2210, MoveKind.Nominal)),
        new(Skill.Reviewer, "A test that changes whenever the code does, checks that a deleted feature is gone, or can never fail is a finding.", new(ReviewState.Round, 2210, MoveKind.Nominal))
    ])
    { Spec = ["T22"] };

    public static readonly Claim Guidance = new("Property and differential tests are guidance, not a gate.", [Actor.Implementer],
        [new(Skill.Implementer, "Property tests for edges and differential tests for rewrites are guidance, never a gate.", new(DispatchState.Working, 2208, MoveKind.Nominal))])
    { Spec = ["T7"] };

    public static readonly Claim Receipts = new("The author proves with receipts; a DYTEST result for the same state counts.", [Actor.Implementer],
    [
        new(Skill.Implementer, "Prove the hop with receipts; a DYTEST result for the same tree counts unless later changes put it in doubt.", new(DispatchState.Working, 2209, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "The author proves with receipts and a fresh reviewer judges through lenses; nothing is re-run the same way, and a DYTEST result for the same tree counts as proof unless later changes put it in doubt.", new(Section.TreeDelivery, 3))
    ])
    { Spec = ["R5"] };

    private static Claim Move(ProofState from, ProofState to, string arrow, params string[] spec) =>
        new($"A test: {arrow}.", [Actor.Dytest], []) { Spec = spec, Transition = new(from, to, arrow) };
}

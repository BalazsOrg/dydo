namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>review</c> claims, sub-topics answer to finding.</summary>
public static class ReviewAnswerToFinding
{
    public static readonly Claim AnswerEveryFinding = new(
        "Every review finding carries a severity and is answered (fixed, ticketed, recorded or relabelled with evidence); none is dropped, from either verdict.",
        [A.TicketCaptain, A.AnyAgent],
        [
            new(S.Reviewer, "There is no unlabelled note: every finding carries its severity and is answered - fixed, ticketed, recorded, or relabelled with evidence - never dropped.", new(F.ReviewerReturn, 5)),
            new(S.TicketCaptain, "Answer every `should` and `nit`, from either verdict.", new(F.TicketCaptainMethod, 36))
        ]);

    public static readonly Claim AxesJudgedAlone = new(
        "Axes and sections are judged alone, so a clean one never masks another's finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Each item ends verified against the source or as a finding, whatever the size of the diff; a clean section never covers another section's finding.", new(F.ReviewerMethod, 10), () => [ReviewAnswerToFinding.EachRubricItemVerified!])
        ]);

    public static readonly Claim BlockerNeverTicket = new(
        "A review blocker stays a FAIL and never becomes a ticket.",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "A review blocker stays a FAIL and never becomes a ticket; a review nit stays one line on its ticket's record.", new(F.LinearWorkspaceStandardSeverity, 5), () => [ReviewFixToPrototype.NitOneLineOnRecord])
        ]);

    public static readonly Claim BriefContents = new(
        "Each fresh reviewer is briefed with rubric, Contract: <KEY> description as of <Linear updatedAt>, Candidate SHA, Base SHA and the writer's IMPLEMENTED line.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Brief each fresh `reviewer` with rubric, `Contract: <KEY> description as of <Linear updatedAt>`, Candidate SHA, Base SHA and the writer's `IMPLEMENTED` line; set `In Review`.", new(F.TicketCaptainMethod, 34))
        ]);

    public static readonly Claim CancelStuckReviewer = new(
        "A reviewer stuck on an approval prompt nobody can grant is cancelled and replaced.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Cancel and replace a reviewer stuck on an approval prompt nobody can grant.", new(F.TicketCaptainMethod, 43))
        ]);

    public static readonly Claim CheckImplementedLineAndProofAgainstPins = new(
        "Where a writer produced the candidate, check its IMPLEMENTED line against the pins and its proof against maintain-verification-skill's Proof.",
        [A.Reviewer], []);

    public static readonly Claim CodeContractAxisClaimedBehaviours = new(
        "Contract axis: every behaviour, edge case and risk the contract names is claimed by a scenario or test; a bug fix carries its reproducing test, an unused-member fix its definition and reference search.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Every behaviour, edge case and risk the contract names is claimed by a scenario or a test; a bug fix carries the test that reproduces the bug, an unused-member fix its definition and reference search instead", new(F.ReviewerCodeContract, 4))
        ]);

    public static readonly Claim CodeContractAxisDeviations = new(
        "Contract axis: every deviation the implementation reported is justified or raised.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Every deviation the implementation reported is justified or raised", new(F.ReviewerCodeContract, 7))
        ]);

    public static readonly Claim CodeContractAxisMatches = new(
        "Contract axis: the candidate matches the pinned contract, the owned paths and the requested outcome.",
        [A.Reviewer],
        [
            new(S.Reviewer, "The candidate matches the pinned contract, the owned paths and the requested outcome", new(F.ReviewerCodeContract, 1))
        ]);

    public static readonly Claim CodeContractAxisNoCreep = new(
        "Contract axis: nothing beyond the contract; an unrelated improvement is scope creep and a finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Nothing beyond the contract: an unrelated improvement is scope creep and a finding", new(F.ReviewerCodeContract, 6))
        ]);

    public static readonly Claim CodeContractAxisNothingDropped = new(
        "Contract axis: nothing the contract needed from the implement hop was dropped by a later hop.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Nothing the implement hop had that the contract needed was dropped by a later hop", new(F.ReviewerCodeContract, 5))
        ]);

    public static readonly Claim CodeContractAxisRedProof = new(
        "Contract axis: the IMPLEMENTED line holds its red proof line; the reviewer reruns it only on a stated doubt.",
        [A.Reviewer],
        [
            new(S.Reviewer, "The `IMPLEMENTED` line the brief carries holds its red proof line: a Bug's reproduction failing with the fix reverted, a Feature's tests failing with the code stashed, an unused-member Bug's definition and reference search; rerun it only on a doubt you state", new(F.ReviewerCodeContract, 3))
        ]);

    public static readonly Claim CodeContractAxisScenarios = new(
        "Contract axis: every scenario the contract calls for stands unweakened and passes.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Every scenario the contract calls for stands unweakened, and every scenario passes", new(F.ReviewerCodeContract, 2))
        ]);

    public static readonly Claim CodeGatesAxisCoverageStage = new(
        "Gates axis at the declared coverage stage: read coverage evidence for HCRAP on every touched method; one over the unchanged bar is a finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "At the declared coverage stage, read applicable coverage evidence for HCRAP on every method the change touched; one over the unchanged bar is a finding.", new(F.ReviewerCodeGates, 2))
        ]);

    public static readonly Claim CodeGatesAxisHop = new(
        "Gates axis at a hop: the writer's affected-feature Light and applicable static run under the testing strategy, every own/reached feature and parent-shared test accounted for, with exact-candidate evidence and owner disposition.",
        [A.Reviewer],
        [
            new(S.Reviewer, "At a hop, the writer's affected-feature Light and applicable static run under the testing strategy, with every own/reached feature and parent-shared test accounted for; cheap checks and attributed failures carry exact-candidate evidence and owner disposition", new(F.ReviewerCodeGates, 1))
        ]);

    public static readonly Claim CodeGatesAxisMutation = new(
        "Gates axis: run the project's changed-code mutation gate in its --since <base> form when it has one; a surviving or uncovered mutant is a finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "The project's changed-code mutation gate, in its `--since <base>` form, run by you when the project has one; a surviving or uncovered mutant is a finding", new(F.ReviewerCodeGates, 4))
        ]);

    public static readonly Claim CodeReadDiff = new(
        "The code reviewer reads the diff (git diff <base>...<candidate>) and then the code it lands in, enough of each file to judge the whole.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Read the diff, then the code it lands in.** `git diff <base>...<candidate>` gives the delta; read enough of each file to judge the whole. Done when every hunk is accounted for.", new(F.ReviewerCodeMethod, 2))
        ]);

    public static readonly Claim CodeReadHops = new(
        "The code reviewer reads the hops with git log <base>..<candidate>.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Read the hops.** `git log <base>..<candidate>` lists the `implement`, `fix` and `merge` commits. The implement hop is judged for doing what the contract says; a fix hop for closing the finding that sent it back and nothing else. Done when you can say per hop what it changed and why.", new(F.ReviewerCodeMethod, 1))
        ]);

    public static readonly Claim CodeSecurityAxisBoundaries = new(
        "Security axis: every boundary the diff touches validates what crosses it, the standards' Security vulnerabilities are asked against every such hunk, and secrets stay out of source and logs.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Every boundary the diff touches validates what crosses it, and the vulnerabilities the standards' Security section names are asked against every such hunk; secrets stay out of source and logs", new(F.ReviewerCodeSecurityAndLikelyBugs, 1))
        ]);

    public static readonly Claim CodeSecurityAxisConcurrency = new(
        "Security axis: ordering, concurrency and resource lifetime are checked where the diff introduces them.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Ordering, concurrency and resource lifetime, where the diff introduces them", new(F.ReviewerCodeSecurityAndLikelyBugs, 3))
        ]);

    public static readonly Claim CodeSecurityAxisEdges = new(
        "Security axis: logic holds at the edges (empty, null, first, last, off-by-one), no fallback masks an impossible state, each error path is handled on purpose.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Logic holds at the edges (empty, null, first, last, off-by-one), no fallback masks an impossible state, and each error path is handled on purpose", new(F.ReviewerCodeSecurityAndLikelyBugs, 2))
        ]);

    public static readonly Claim CodeStandardsAxisBinding = new(
        "Standards axis: dydo/guides/coding-standards.md and any stack-specific standard bind, including the anti-slop mandate, with the codebase-design lens on every touched seam.",
        [A.Reviewer],
        [
            new(S.Reviewer, "`dydo/guides/coding-standards.md`, read from the repository root, and any stack-specific standard bind, the anti-slop mandate included, with the `codebase-design` lens on every seam the diff touches; a documented standard beats your taste, and a rule the tooling enforces is closed", new(F.ReviewerCodeStandards, 1), () => [ReviewAnswerToFinding.DocumentedStandardBeatsTaste!])
        ]);

    public static readonly Claim CodeStandardsAxisBuiltOnBadCode = new(
        "Standards axis: already-bad code is a finding when this change builds on it.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Code that was already bad is a finding when this change builds on it", new(F.ReviewerCodeStandards, 4))
        ]);

    public static readonly Claim CodeStandardsAxisDeadCode = new(
        "Standards axis: code only tests call, or commented out, is dead.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Code that only tests call, or that is commented out, is dead. The fix is to delete it with its tests, even when a gate asked for more tests or a refactor", new(F.ReviewerCodeStandards, 5))
        ]);

    public static readonly Claim CodeStandardsAxisFowlerSmells = new(
        "Standards axis: judge Fowler's twelve smells (Refactoring ch. 3).",
        [A.Reviewer],
        [
            new(S.Reviewer, "Fowler's twelve smells (*Refactoring*, ch. 3): Mysterious Name, Duplicated Code, Feature Envy, Data Clumps, Primitive Obsession, Repeated Switches, Shotgun Surgery, Divergent Change, Speculative Generality, Message Chains, Middle Man, Refused Bequest. Each is a question against the diff, the hunk quoted, a finding only with its concrete consequence named; every smell answered, not the first one found", new(F.ReviewerCodeStandards, 3))
        ]);

    public static readonly Claim CodeStandardsAxisNoWorse = new(
        "Standards axis: touched code is no worse than found and smaller, simpler, standard or deeper where warranted; an abstraction or optimisation ahead of need is a finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "The code the candidate touched is no worse than it was found, and smaller, simpler, standard or deeper wherever the change warranted it; an abstraction or optimisation ahead of a need is a finding", new(F.ReviewerCodeStandards, 2))
        ]);

    public static readonly Claim CodeStandardsAxisPassByConstruction = new(
        "Standards axis: shapes that pass by construction (recomputed expected value, mock inside the unit, never-red suite, assertion on prompt wording, metric moved with nothing claimed, unbounded wait or order dependence) are findings.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Shapes that pass by construction: an expected value recomputed the code's way; a mock inside the unit; a suite where no test was ever red; an assertion on a prompt file's wording; a metric moved with nothing claimed; an unbounded wait or a dependence on order", new(F.ReviewerCodeStandards, 7))
        ]);

    public static readonly Claim CodeStandardsAxisTestIsContract = new(
        "Standards axis: each test is a contract (one claim, named by case and expectation, at a seam a caller observes, that some breach turns red); a test with no such breach is a finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Each test is a contract: one claim, named by case and expectation, at a seam a caller observes, that some breach turns red; a test with no such breach is a finding however green it runs", new(F.ReviewerCodeStandards, 6))
        ]);

    public static readonly Claim CodexReviewerGetsContractPasted = new(
        "A reviewer without Linear access (a Codex seat) gets the contract pasted verbatim into its brief or as a snapshot file, never a pointer to the ticket.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A reviewer without Linear access, a Codex seat, gets the contract pasted verbatim into its brief or as a snapshot file, never a pointer to the ticket.", new(F.TicketCaptainMethod, 42))
        ]);

    public static readonly Claim CorrectedMergeFreshReview = new(
        "Each corrected merge candidate gets a fresh merge review.",
        [A.TicketCaptain],
        [
            new(S.WorkingTreeStandard, "Each corrected candidate gets fresh merge review.", new(F.WorkingTreeStandardReturnAndRelease, 12))
        ]);

    public static readonly Claim DocsReviewTwoKinds = new(
        "Documentation changes are two kinds: human-facing dydo documents judged against code or governing intent, and agent-facing documents judged as prompt files that must fire and route.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Two kinds arrive here: human-facing dydo documents under `dydo/`, judged against code for delivered behavior or governing intent for specs, and agent-facing documents (skills, their resources, entry points), judged as prompt files that must fire and route.", new(F.ReviewerDocsLead, 1))
        ]);

    public static readonly Claim DocumentedStandardBeatsTaste = new(
        "A documented standard beats taste and a tooling-enforced rule is closed.",
        [A.Reviewer], []);

    public static readonly Claim EachRubricItemVerified = new(
        "Each rubric item or axis ends verified against the source or as a finding, whatever the diff size.",
        [A.Reviewer], []);

    public static readonly Claim FailBlockSentWithBrief = new(
        "The FAIL block is sent with the brief of the re-review.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Send the FAIL block with the brief and set `Implementing`.", new(F.TicketCaptainMethod, 44))
        ]);

    public static readonly Claim FailBlockersBindingToImplementing = new(
        "A FAIL (one or more blockers) is binding: the record returns to Implementing and the blockers go to a fresh author of the change's kind.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Treat FAIL, one or more blockers, as binding: its blockers go back to a fresh author of the change's kind - `code-writer`, or `docs-writer` for a documentation change - at `Implementing`, and a wrong scenario is amended by that fix hop when the FAIL block names it.", new(F.TicketCaptainMethod, 35), () => [ReviewReadToWrong.WrongScenarioAmendedByFixHop])
        ]);

    public static readonly Claim FailMeansAtLeastOneBlocker = new(
        "A FAIL carries at least one blocker.",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "A FAIL carries at least one blocker; a PASS carries none and lists its should and nit findings under `Deferred`.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 21), () => [ReviewFixToPrototype.PassMeansNoBlockerListsDeferred]),
            new(S.Reviewer, "FAIL means at least one blocker - YOU SHALL NOT PASS.", new(F.ReviewerReturn, 3))
        ]);

    public static readonly Claim FinalMergeProvesAcceptance = new(
        "Acceptance is proved at a final merge (final landing proves Project acceptance).",
        [A.Reviewer],
        [
            new(S.Reviewer, "No review at another level substitutes for this one; final landing also proves Project acceptance.", new(F.ReviewerMergeLead, 5), () => [ReviewFixToPrototype.MergeReviewNotSubstitutable])
        ]);

    public static readonly Claim FinalWalkthroughListsUnaddressedNits = new(
        "The final walkthrough lists the nits left unaddressed for the user to promote.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Record each nit as one line on the ticket; the final walkthrough lists those left unaddressed for the user to promote.", new(F.TicketCaptainMethod, 37), () => [ReviewFixToPrototype.NitOneLineOnRecord])
        ]);

    public static readonly Claim FindingCarriesOneSeverity = new(
        "A review block is done when every finding carries exactly one severity and the invoker can act on each without asking.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Done when every finding carries one severity and the invoker can act on each without asking you a question.", new(F.ReviewerMethod, 17))
        ]);
}

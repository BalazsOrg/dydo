namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>research</c> claims, sub-topics follow to source.</summary>
public static class Research
{
    public static readonly Claim FollowClaimToOwningSource = new(
        "Every claim is followed back to the source that owns it; a doc account of the code is a lead to the code, not a finding.",
        [A.Research, A.Scout],
        [
            new(S.Research, "Follow every claim back to the source that owns it, and search wide enough to know what you missed.", new(SpinePart.Job, 1)),
            new(S.Scout, "Take the passage from the source that owns the claim: a doc's account of the code is a lead to the code, not a finding.", new(F.ScoutMethod, 3))
        ]);

    public static readonly Claim NameConflictsAndGoverningSource = new(
        "Where sources conflict, name the conflict and which governs, by the order: user live instruction, Decision Record, reviewed architecture plan, feature spec, ticket contract, standards, code.",
        [A.Research],
        [
            new(S.Research, "Where sources conflict, name the conflict and which governs; inside the repository the order is the user's live instruction, Decision Record, reviewed architecture plan, feature spec (both pinned), ticket contract, standards, code.", new(F.ResearchMethod, 7))
        ]);

    public static readonly Claim OpenCitedPassage = new(
        "Open the cited passage behind every load-bearing claim and confirm it says what the scout said; verification is done when every claim in the answer carries a citation opened or is marked unsettled.",
        [A.Research],
        [
            new(S.Research, "Open the cited passage behind every load-bearing claim and confirm it says what the scout said.", new(F.ResearchMethod, 6))
        ]);

    public static readonly Claim QuestionIsWholeScope = new(
        "The question given is the whole scope: an adjacent question uncovered is a line in the return, not a second investigation.",
        [A.Research],
        [
            new(S.Research, "The question you were given is the whole of your scope: an adjacent question you uncover is a line in the return, not a second investigation.", new(F.ResearchBoundary, 3))
        ]);

    public static readonly Claim ReadGoverningDocsOnDefaultBranch = new(
        "Governing docs (Decision Records, standards, glossary) are read on the default branch too, since a branch can lag.",
        [A.Research],
        [
            new(S.Research, "Read governing docs (Decision Records, standards, glossary) on the default branch too; a branch can lag.", new(F.ResearchMethod, 3))
        ]);

    public static readonly Claim ReportIsOnlyOutput = new(
        "The report under Return is the only thing research creates; everything else is read.",
        [A.Research],
        [
            new(S.Research, "The report under Return is the one thing you create; everything else you read.", new(F.ResearchBoundary, 4))
        ]);

    public static readonly Claim RestateQuestion = new(
        "Restate the question in one sentence, say what would count as an answer and note the destination the invoker named; the question step is done when all three are written down.",
        [A.Research],
        [
            new(S.Research, "Restate it in one sentence, say what would count as an answer, and note the destination the invoker named.", new(F.ResearchMethod, 1))
        ]);

    public static readonly Claim ScoutCopyPassages = new(
        "The scout copies passages, not summaries: for each finding the exact passage, its citation (URL, DOI, or repository path with commit) and one line on how it bears on the question; done when every finding is a quotation the researcher can open.",
        [A.Scout],
        [
            new(S.Scout, "For each finding: the exact passage, its citation (URL, DOI, or repository path with commit), and one line on how it bears on the question.", new(F.ScoutMethod, 2))
        ]);

    public static readonly Claim ScoutSearchWholeFamily = new(
        "The scout searches every place in the family where the answer or a breadcrumb of intent could sit, following references to their end, and is done when it can list what it searched and nothing that could bear on the question is left unopened.",
        [A.Scout],
        [
            new(S.Scout, "Every place in it where the answer or a breadcrumb of intent could sit, following references inside the family to their end.", new(F.ScoutMethod, 1))
        ]);

    public static readonly Claim ScoutingDone = new(
        "Scouting is done when every family in play has returned passages or an honest nothing here.",
        [A.Research],
        [
            new(S.Research, "Done when every family in play has returned passages or an honest \"nothing here\".", new(F.ResearchMethod, 5))
        ]);

    public static readonly Claim SilentSourcesUnsettled = new(
        "Where sources are silent, mark the point unsettled rather than reasoning to a fact.",
        [A.Research],
        [
            new(S.Research, "Where sources are silent, mark the point unsettled rather than reasoning your way to a fact.", new(F.ResearchMethod, 8))
        ]);

    public static readonly Claim SourceFamiliesMarked = new(
        "Consider the source families (governing records and others); the family step is done when every family is marked in or out with a reason.",
        [A.Research],
        [
            new(S.Research, "Governing records: Decision Records, the Project plan at its governing commit, specifications. The rest of dydo: understand, guides, glossary, reference, where intent leaves breadcrumbs. Code and tests. Git history, for when and why something changed. Outside sources: official docs, specs, library source, first-party APIs.", new(F.ResearchMethod, 2))
        ]);
}

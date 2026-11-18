namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the crew the inventory's carriers.json declared.</summary>
public static class FixtureCrew
{
    public static readonly SkillSpec CodeWriter = SkillSpec.Crew(S.CodeWriter, "Code Writer", "A contracted ticket to build, a review FAIL to close, a merge to perform, or a hypothesis to prove or refute with one test.") with
    {
        Sections = [new(F.CodeWriterMustReads, "Must-Reads"), new(F.CodeWriterBoundary, "Boundary"), new(F.CodeWriterBeforeTheFirstEdit, "Before the first edit"), new(F.CodeWriterBugOrFeature, "Bug or Feature"), new(F.CodeWriterBuild, "Build"), new(F.CodeWriterProve, "Prove"), new(F.CodeWriterReturn, "Return"), new(F.CodeWriterWhenYouAreTheFixHop, "When you are the fix hop")],
        Resources =
        [
            new(CodeWriterResource.Bug, "Bug", [new(F.CodeWriterBugLead)]),
            new(CodeWriterResource.Merge, "Merge", [new(F.CodeWriterMergeLead)]),
            new(CodeWriterResource.Tests, "Good and Bad Tests", [new(F.CodeWriterTestsGoodTests, "Good Tests"), new(F.CodeWriterTestsBadTests, "Bad Tests"), new(F.CodeWriterTestsWhereAMockBelongs, "Where a Mock Belongs")])
        ]
    };

    public static readonly SkillSpec DocsWriter = SkillSpec.Crew(S.DocsWriter, "Docs Writer", "Documentation the repository can witness. Write or correct one reviewed change, an inquisition's own record included, for its Ticket Captain.") with
    {
        Sections = [new(F.DocsWriterMustReads, "Must-Reads"), new(F.DocsWriterBoundary, "Boundary"), new(F.DocsWriterMethod, "Method"), new(F.DocsWriterReturn, "Return")]
    };

    public static readonly SkillSpec Inquisitor = SkillSpec.Crew(S.Inquisitor, "Inquisitor", "Refute-first sweep of landed work, one phase - Read, Break or Drive - on one area. Use when an inquisition's captain sends you a phase and an area.") with
    {
        Sections = [new(F.InquisitorMustReads, "Must-Reads"), new(F.InquisitorBoundary, "Boundary"), new(F.InquisitorMethod, "Method"), new(F.InquisitorProof, "Proof"), new(F.InquisitorSeverity, "Severity"), new(F.InquisitorCalibration, "Calibration"), new(F.InquisitorReturn, "Return")],
        Resources =
        [
            new(InquisitorResource.Break, "Break", [new(F.InquisitorBreakLead), new(F.InquisitorBreakBeforeTheFirstEdit, "Before the first edit"), new(F.InquisitorBreakDiscovery, "Discovery"), new(F.InquisitorBreakWork, "Work"), new(F.InquisitorBreakDocument, "Document")]),
            new(InquisitorResource.ContextSize, "Context size", [new(F.InquisitorContextSizeLead), new(F.InquisitorContextSizeOwnSessionOnly, "Own session only"), new(F.InquisitorContextSizeInstallTheCommandOnce, "Install the command once"), new(F.InquisitorContextSizeRunItAfterEachWorkItem, "Run it after each work item")]),
            new(InquisitorResource.Drive, "Drive", [new(F.InquisitorDriveLead), new(F.InquisitorDriveDiscovery, "Discovery"), new(F.InquisitorDriveWork, "Work")]),
            new(InquisitorResource.Read, "Read", [new(F.InquisitorReadDiscovery, "Discovery"), new(F.InquisitorReadWork, "Work")])
        ]
    };

    public static readonly SkillSpec Research = SkillSpec.Crew(S.Research, "Research", "Primary sources, cited. Use when a fact a choice waits on could hide in Decision Records, plans, code, history, or outside sources, or when docs, specs, or API behaviour must be established before work depends on them.") with
    {
        Sections = [new(F.ResearchMustReads, "Must-Reads"), new(F.ResearchBoundary, "Boundary"), new(F.ResearchMethod, "Method"), new(F.ResearchReturn, "Return")]
    };

    public static readonly SkillSpec Reviewer = SkillSpec.Crew(S.Reviewer, "Reviewer", "A ticket's code or docs, a spec, a Project plan, or a merged tree - one candidate, one named rubric, one binding verdict.") with
    {
        Sections = [new(F.ReviewerMustReads, "Must-Reads"), new(F.ReviewerBoundary, "Boundary"), new(F.ReviewerMethod, "Method"), new(F.ReviewerReturn, "Return")],
        Resources =
        [
            new(ReviewerResource.Code, "Reviewing Code", [new(F.ReviewerCodeMethod, "Method"), new(F.ReviewerCodeContract, "Contract"), new(F.ReviewerCodeStandards, "Standards"), new(F.ReviewerCodeGates, "Gates"), new(F.ReviewerCodeSecurityAndLikelyBugs, "Security and likely bugs"), new(F.ReviewerCodeVerdict, "Verdict")]),
            new(ReviewerResource.Docs, "Reviewing Documentation", [new(F.ReviewerDocsLead), new(F.ReviewerDocsMethod, "Method"), new(F.ReviewerDocsHumanFacingDydoDocuments, "Human-facing dydo documents"), new(F.ReviewerDocsAgentFacingDocuments, "Agent-facing documents")]),
            new(ReviewerResource.Merge, "Reviewing a Merge", [new(F.ReviewerMergeLead), new(F.ReviewerMergeMethod, "Method")]),
            new(ReviewerResource.ProjectPlan, "Reviewing a Project Plan", [new(F.ReviewerProjectPlanLead), new(F.ReviewerProjectPlanMethod, "Method")]),
            new(ReviewerResource.Spec, "Reviewing a Spec", [new(F.ReviewerSpecLead), new(F.ReviewerSpecBoundary, "Boundary"), new(F.ReviewerSpecMethod, "Method"), new(F.ReviewerSpecChecklist, "Checklist")])
        ]
    };

    public static readonly SkillSpec Scout = SkillSpec.Crew(S.Scout, "Scout", "One question, one source family, passages back, for a researcher who verifies and pools them.") with
    {
        Sections = [new(F.ScoutMustReads, "Must-Reads"), new(F.ScoutBoundary, "Boundary"), new(F.ScoutMethod, "Method"), new(F.ScoutReturn, "Return")]
    };
}

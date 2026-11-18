namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the standards the inventory's carriers.json declared.</summary>
public static class FixtureStandards
{
    public static readonly SkillSpec LinearWorkspaceStandard = SkillSpec.Standard(S.LinearWorkspaceStandard, "reference", "Linear Workspace Standard", "area: reference", "type: reference") with
    {
        Sections = [new(F.LinearWorkspaceStandardLead), new(F.LinearWorkspaceStandardProjectStatuses, "Project statuses"), new(F.LinearWorkspaceStandardTicketStatuses, "Ticket statuses"), new(F.LinearWorkspaceStandardTicketLabels, "Ticket labels"), new(F.LinearWorkspaceStandardType, "Type"), new(F.LinearWorkspaceStandardMode, "Mode"), new(F.LinearWorkspaceStandardSeverity, "Severity"), new(F.LinearWorkspaceStandardPriority, "Priority"), new(F.LinearWorkspaceStandardWhoHoldsWhat, "Who holds what"), new(F.LinearWorkspaceStandardQuestionTickets, "Question Tickets"), new(F.LinearWorkspaceStandardTicketTemplates, "Ticket templates"), new(F.LinearWorkspaceStandardCommunicationAndEvidence, "Communication and evidence"), new(F.LinearWorkspaceStandardTruthfulCommentSignatures, "Truthful comment signatures"), new(F.LinearWorkspaceStandardDecisionRecords, "Decision Records")]
    };

    public static readonly SkillSpec WorkingTreeStandard = SkillSpec.Standard(S.WorkingTreeStandard, "guides", "Working-Tree Standard", "area: guides", "type: guide") with
    {
        Sections = [new(F.WorkingTreeStandardLead), new(F.WorkingTreeStandardInvariants, "Invariants"), new(F.WorkingTreeStandardBranchesAndTargets, "Branches and targets"), new(F.WorkingTreeStandardWhoDoesWhat, "Who does what"), new(F.WorkingTreeStandardBeforeTheFirstEdit, "Before the first edit"), new(F.WorkingTreeStandardDeliveryScaleAndEvidence, "Delivery scale and evidence"), new(F.WorkingTreeStandardDelegation, "Delegation"), new(F.WorkingTreeStandardReturnAndRelease, "Return and release"), new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, "Retaining an Inquisition's record and proofs"), new(F.WorkingTreeStandardCleanup, "Cleanup")]
    };
}

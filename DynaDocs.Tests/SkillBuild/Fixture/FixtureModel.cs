namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Fixture.Kinds;

/// <summary>
/// The migration fixture (AR1): claims/inventory/claims.json at its base, translated once, each carrier
/// keeping the first copy of each claim in source order. A throwaway build input, never the live skills.
/// </summary>
public static class FixtureModel
{
    public static Model Model { get; } = new(
    [
        typeof(Capacity),
        typeof(ContractAddToMap),
        typeof(ContractMoveToWriter),
        typeof(DecisionAwayToPrecedent),
        typeof(DecisionPreparedToWork),
        typeof(DispatchCaptainToNo),
        typeof(DispatchOneToWithout),
        typeof(Docs),
        typeof(Escalation),
        typeof(EvidenceBugToPassing),
        typeof(EvidencePinToValidate),
        typeof(Identity),
        typeof(InquisitionAreaToFollow),
        typeof(InquisitionGroupToRead),
        typeof(InquisitionRecordToWork),
        typeof(LinearAdmiralToOpen),
        typeof(LinearParentToThree),
        typeof(LinearTicketToWriter),
        typeof(MergeAnswerToWake),
        typeof(MergeWriter),
        typeof(Other),
        typeof(OvernightAdmiralToResolve),
        typeof(OvernightSettleToWatch),
        typeof(ProjectAdmiralToLedger),
        typeof(ProjectMapToWatch),
        typeof(Release),
        typeof(Research),
        typeof(ReturnAdmiralToNo),
        typeof(ReturnOrdinaryToWriter),
        typeof(ReviewAnswerToFinding),
        typeof(ReviewFixToPrototype),
        typeof(ReviewReadToWrong),
        typeof(Walkthrough),
        typeof(WayfindingAcceptToResolve),
        typeof(WayfindingRouteToWrite),
        typeof(WorktreeAuditToReviewer),
        typeof(WorktreeRootToT3)
    ],
    [
        FixtureOfficers.Admiral,
        FixtureOfficers.ChiefOfStaff,
        FixtureCrew.CodeWriter,
        FixtureCrew.DocsWriter,
        FixtureCrew.Inquisitor,
        FixtureStandards.LinearWorkspaceStandard,
        FixtureWorkflows.OvernightMode,
        FixtureCrew.Research,
        FixtureCrew.Reviewer,
        FixtureCrew.Scout,
        FixtureOfficers.TicketCaptain,
        FixtureWorkflows.Walkthrough,
        FixtureWorkflows.Wayfinder,
        FixtureWorkflows.WhatWouldBalazsDo,
        FixtureStandards.WorkingTreeStandard
    ]);
}

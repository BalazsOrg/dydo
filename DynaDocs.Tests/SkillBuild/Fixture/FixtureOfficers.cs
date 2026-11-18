namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the officers the inventory's carriers.json declared.</summary>
public static class FixtureOfficers
{
    public static readonly SkillSpec Admiral = SkillSpec.Officer(S.Admiral, "Admiral", "Your right hand on one Project - chart it, then run it through captains, reviewed merges and your gates.") with
    {
        ExplicitOnly = true,
        Sections = [new(F.AdmiralMustReads, "Must-Reads"), new(F.AdmiralBoundary, "Boundary"), new(F.AdmiralMethod, "Method"), new(F.AdmiralReturn, "Return")],
        Resources =
        [
            new(AdmiralResource.CheckIn, "Check-in", [new(F.AdmiralCheckInLead), new(F.AdmiralCheckInLedger, "Ledger"), new(F.AdmiralCheckInSchedule, "Schedule"), new(F.AdmiralCheckInTickTheAdmiral, "Tick — the Admiral"), new(F.AdmiralCheckInSurveyTheSurveyor, "Survey — the surveyor"), new(F.AdmiralCheckInFixedDispatch, "Fixed dispatch")]),
            new(AdmiralResource.ScaffoldProject, "Scaffold a Project", [new(F.AdmiralScaffoldProjectLead)])
        ]
    };

    public static readonly SkillSpec ChiefOfStaff = SkillSpec.Officer(S.ChiefOfStaff, "Chief of Staff", "Your attention, triaged - the Questions waiting on your answer, the gates waiting on your approval or your click, and what on the board has gone stale.") with
    {
        ExplicitOnly = true,
        Sections = [new(F.ChiefOfStaffMustReads, "Must-Reads"), new(F.ChiefOfStaffBoundary, "Boundary"), new(F.ChiefOfStaffMethod, "Method"), new(F.ChiefOfStaffOvernightDuties, "Overnight duties"), new(F.ChiefOfStaffReturn, "Return")],
        Resources =
        [
            new(ChiefOfStaffResource.MorningBrief, "Morning brief", [new(F.ChiefOfStaffMorningBriefLead), new(F.ChiefOfStaffMorningBriefTheNight, "The night"), new(F.ChiefOfStaffMorningBriefPRsIntoMasterAwaitingYourClick, "PRs into master awaiting your click"), new(F.ChiefOfStaffMorningBriefQuestionsDecidedFirst, "Questions — decided first"), new(F.ChiefOfStaffMorningBriefYourHelpNeeded, "Your help needed"), new(F.ChiefOfStaffMorningBriefNext, "Next")]),
            new(ChiefOfStaffResource.Muster, "Muster", [new(F.ChiefOfStaffMusterLead), new(F.ChiefOfStaffMusterPresentToTheUser, "Present to the user")]),
            new(ChiefOfStaffResource.NightSurvey, "Night survey", [new(F.ChiefOfStaffNightSurveyLead), new(F.ChiefOfStaffNightSurveySurvey, "Survey"), new(F.ChiefOfStaffNightSurveyReturn, "Return"), new(F.ChiefOfStaffNightSurveyFixedDispatchChiefOfStaffFillsAndSends, "Fixed dispatch — Chief of Staff fills and sends")])
        ]
    };

    public static readonly SkillSpec TicketCaptain = SkillSpec.Officer(S.TicketCaptain, "Ticket Captain", "One contracted ticket needs a captain - specify, direct the crew, review, merge and release from its recorded state.") with
    {
        Sections = [new(F.TicketCaptainMustReads, "Must-Reads"), new(F.TicketCaptainBoundary, "Boundary"), new(F.TicketCaptainMethod, "Method"), new(F.TicketCaptainKindsAndFailurePaths, "Kinds and failure paths"), new(F.TicketCaptainRelease, "Release"), new(F.TicketCaptainReturn, "Return")],
        Resources =
        [
            new(TicketCaptainResource.Inquisition, "Run an Inquisition", [new(F.TicketCaptainInquisitionLead), new(F.TicketCaptainInquisitionN1CommissionInputs, "1. Commission inputs"), new(F.TicketCaptainInquisitionN2OpenTheAuditBranch, "2. Open the audit branch"), new(F.TicketCaptainInquisitionN3DrawTheAreaMap, "3. Draw the area map"), new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, "4. Split above about 10 areas"), new(F.TicketCaptainInquisitionN5KeepTheFindingsRegister, "5. Keep the findings register"), new(F.TicketCaptainInquisitionN6ScheduleThePipeline, "6. Schedule the pipeline"), new(F.TicketCaptainInquisitionN7RouteEachReturn, "7. Route each return"), new(F.TicketCaptainInquisitionN8TriageEachReturn, "8. Triage each return"), new(F.TicketCaptainInquisitionN9RunThePatchLane, "9. Run the patch lane"), new(F.TicketCaptainInquisitionN10CloseTheSweep, "10. Close the sweep"), new(F.TicketCaptainInquisitionN11DeliverTheRecord, "11. Deliver the record"), new(F.TicketCaptainInquisitionN12Close, "12. Close")])
        ]
    };
}

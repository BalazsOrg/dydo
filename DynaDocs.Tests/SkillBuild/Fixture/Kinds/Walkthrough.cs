namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>walkthrough</c> claims, sub-topics admiral to walk.</summary>
public static class Walkthrough
{
    public static readonly Claim AdmiralFacilitatesLast = new(
        "The admiral opens the last Milestone's Walkthrough Ticket, loads `walkthrough` itself, and facilitates it in the admiral session.",
        [A.Admiral], []);

    public static readonly Claim AskTarget = new(
        "Ask which target to walk when the argument is missing.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "Ask which one when it is missing.", new(HeaderPart.Role, 6))
        ]);

    public static readonly Claim BriefFourParts = new(
        "Brief the user in four parts, newest changes first, marking those needing his careful look (where a drift from his intent costs most or a review left doubt); a Walkthrough covers what changed, where to look, how to try it, and what reviewers left open.",
        [A.AnyAgent, A.MapHolder],
        [
            new(S.Walkthrough, "Then brief the user in four parts, the newest changes first, and mark the ones that need his careful look: where a drift from his intent would cost most, or a review left doubt.", new(HeaderPart.Role, 8))
        ]);

    public static readonly Claim BriefMedium = new(
        "Keep the brief in the terminal, or in a scratch file when the user wants to read it beside the diff.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "Keep the brief in the terminal, or in a scratch file when the user wants to read it beside the diff.", new(HeaderPart.Role, 20))
        ]);

    public static readonly Claim DurableRecordStays = new(
        "The durable record stays where it already is: the tickets, review blocks and DRs the tour points at.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "The durable record stays where it already is: the tickets, the review blocks and the DRs the tour points at.", new(HeaderPart.Role, 21))
        ]);

    public static readonly Claim LandingFindingsReopenLap = new(
        "Findings at the landing walkthrough reopen the lap: the map holder commissions the first fix Captain to re-cut the feature branch from main.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Findings reopen the lap: commission the first fix Captain to re-cut the feature from main under the same name; another inquisition needs confirmation.", new(F.AdmiralMethod, 70)),
            new(S.Walkthrough, "At the landing, record findings as tickets in the same Project; the map holder commissions the first fix Captain to re-cut its feature branch from main for fixes and another landing/walkthrough.", new(HeaderPart.Role, 18))
        ]);

    public static readonly Claim LandingNitsReported = new(
        "The landing walkthrough also lists every unaddressed `nit` with the ticket record where it sits and every open inquisition nit ticket, for the user to promote any.",
        [A.Admiral, A.AnyAgent],
        [
            new(S.Admiral, "It also lists the unaddressed nits and every open inquisition nit ticket.", new(F.AdmiralMethod, 69)),
            new(S.Walkthrough, "At the landing, also every unaddressed `nit` with the ticket record where it sits, and every open inquisition nit ticket, for the user to promote any.", new(HeaderPart.Role, 13))
        ]);

    public static readonly Claim LaterTicketsNeverBlock = new(
        "Later Milestones' Walkthrough Tickets block nothing.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "Later Milestones' Walkthrough Tickets block nothing: their findings become tickets in the Project as they arrive.", new(F.AdmiralMethod, 41), () => [Walkthrough.MilestoneFindingsBecomeTickets!])
        ]);

    public static readonly Claim LoadTriggers = new(
        "Load the walkthrough skill only when the user asks, or at the map holder's Milestone or landing step.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "Load only when the user asks, or at the map holder's Milestone or landing step.", new(HeaderPart.Role, 1))
        ]);

    public static readonly Claim MilestoneFindingsBecomeTickets = new(
        "A Milestone's findings become tickets in the same Project as they arrive.",
        [A.MapHolder, A.Admiral],
        [
            new(S.Walkthrough, "A Milestone's findings become tickets in the same Project as they arrive.", new(HeaderPart.Role, 17))
        ]);

    public static readonly Claim MilestoneVerdictRecorded = new(
        "The Walkthrough Ticket carries the user's verdict and findings, in his own words, or the skip.",
        [A.Admiral, A.AnyAgent],
        [
            new(S.Admiral, "**Done:** each Walkthrough Ticket carries his verdict and findings, or his skip.", new(F.AdmiralMethod, 43)),
            new(S.Walkthrough, "Record his verdict on the Walkthrough Ticket in his own words, or the skip.", new(HeaderPart.Role, 15))
        ]);

    public static readonly Claim OneTicketPerMilestone = new(
        "A Project has one Walkthrough Ticket per Milestone, prepared even if the user skips it, the landing's last.",
        [A.MapHolder],
        [
            new(S.Walkthrough, "A Project has one Walkthrough Ticket per Milestone, the landing's last.", new(HeaderPart.Role, 3))
        ]);

    public static readonly Claim OtherToursOrdinaryTickets = new(
        "Other (non-Milestone, non-landing) tours route findings through ordinary tickets.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "Other tours route findings through ordinary tickets.", new(HeaderPart.Role, 16))
        ]);

    public static readonly Claim PartFlaggedOrDeferred = new(
        "Part four, What reviewers flagged or deferred: in the glossary's Severity terms, each deferred `should` and the ticket where it now lives, as it stands.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "**What reviewers flagged or deferred** - in the severities the **Severity** entry of `dydo/reference/dydo-glossary.md` defines: each `should` deferred and the ticket where it now lives, reported as it stands.", new(HeaderPart.Role, 12))
        ]);

    public static readonly Claim PartHowToTry = new(
        "Part three, How to try it: the exact commands to build it and see the new behaviour, in order, each run once by you.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "**How to try it** - the exact commands to build it and see the new behaviour, in order, each one run once by you so it is true when he types it.", new(HeaderPart.Role, 11))
        ]);

    public static readonly Claim PartWhatChanged = new(
        "Part one, What changed and why: the current state in one paragraph, then changes since the last walkthrough each traced to its ticket and DR, drawn with `show-me` where a tree or diff says it faster.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "**What changed and why** - the current state in one paragraph, then what changed since the last walkthrough, each part traced to the ticket that carried it and the DR that decided it, with the shape drawn by `show-me` where a tree or a diff says it faster than prose.", new(HeaderPart.Role, 9))
        ]);

    public static readonly Claim PartWhereToLook = new(
        "Part two, Where to look first: a route for his file-by-file pass: the careful-look parts, files that drew findings, files with an open crew doubt, then seams the change moved.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "**Where to look first** - a route for his own file-by-file pass: the parts marked for a careful look, the files that drew findings, then the ones a crew member left an open doubt on, then the seams the change moved.", new(HeaderPart.Role, 10))
        ]);

    public static readonly Claim PrepareAfterLastTicketMerges = new(
        "When a Milestone's last ticket has merged into the feature branch, prepare its Walkthrough Ticket and offer it to the user; the last Milestone's belongs to the landing.",
        [A.Admiral],
        [
            new(S.Admiral, "When a Milestone's last ticket has merged into the feature branch, prepare its Walkthrough Ticket and offer it to the user; the last Milestone's is the landing's, in step 8.", new(F.AdmiralMethod, 39))
        ]);

    public static readonly Claim SkeletonTicketBlocksNextMilestone = new(
        "The walking skeleton's Walkthrough Ticket, filed with the map, is a native blocker of the next Milestone's delivery tickets until the user's verdict or skip closes it, so nothing is built on the skeleton before then.",
        [A.Admiral, A.MapHolder],
        [
            new(S.Admiral, "File its Walkthrough Ticket with the map, wired as a native blocker of the next Milestone's delivery tickets except in overnight mode.", new(F.AdmiralMethod, 13)),
            new(S.LinearWorkspaceStandard, "Only the walking skeleton's holds back the work built on it: filed with the map, it is a native blocker of the next Milestone's delivery tickets until his verdict or skip closes it.", new(F.LinearWorkspaceStandardWhoHoldsWhat, 10))
        ]);

    public static readonly Claim TargetSha = new(
        "At a Milestone walk the feature branch at the SHA its last merge left; at the landing walk main after it landed.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "At a Milestone, walk the feature branch at the SHA its last merge left; at the landing, walk main after it landed.", new(HeaderPart.Role, 4))
        ]);

    public static readonly Claim TourEnds = new(
        "The tour ends when the user has inspected the files and tried the behavior, or skipped it.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "The tour ends when the user has inspected the files and tried the behavior, or skipped it.", new(HeaderPart.Role, 14))
        ]);

    public static readonly Claim TourWholeProduct = new(
        "Give the user a tour of the whole product as it stands, so it makes sense to him even when he is not close in.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "Give the user the tour of the whole product as it stands, so it makes sense to him even when he is not close in.", new(HeaderPart.Role, 2))
        ]);

    public static readonly Claim UserWalksOrSkips = new(
        "The user walks a Milestone's Walkthrough Ticket with the map holder or skips it; a skip is his verdict to go on.",
        [A.User, A.MapHolder],
        [
            new(S.Admiral, "He walks it with you, by `walkthrough`, or skips it, and a skip is a verdict to go on.", new(F.AdmiralMethod, 40)),
            new(S.Walkthrough, "The map holder runs the ticket with the user; he walks it, or skips it, and a skip is his verdict to go on.", new(HeaderPart.Role, 5))
        ]);

    public static readonly Claim WalkTargetFirst = new(
        "Walk the target yourself first: the whole state at that SHA, the diff since the last walkthrough and its hops, closed tickets with review blocks, cited DRs and spec sections, and posted crew returns.",
        [A.AnyAgent],
        [
            new(S.Walkthrough, "Walk it yourself first: the whole state at that SHA, the diff since the last walkthrough and its hops, the tickets it closed with their review blocks, the DRs and applicable spec sections those tickets cite, the returns their crew posted.", new(HeaderPart.Role, 7))
        ]);
}

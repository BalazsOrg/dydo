namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype (DYD-1372), the first truth-checked skill: built to
/// <c>skills/roles/officers/ticket-captain/</c> in place of the legacy <see cref="TicketCaptainCarrier"/>,
/// whose lines stay in the model, unbuilt, and are accounted for in <see cref="CaptainLedger"/>. One
/// captain base (option A, DYD-1295 0f340abd): five phases in their real order, three kind resources
/// its first phase links, the resources a later phase's line links, and one scenario per failure.
/// A block only some captains reach sits in the resource its line links, which is how the spine
/// stays within half the legacy spine's size without dropping a fact: what gets a sub-ticket, and how
/// a captain finds its way through fog, are read only by a captain that cuts (DYD-1372 phase 1); and
/// the route of a check that fails before a ticket is started is read only by the holder it stops.
/// A kind resource opens with its map of the spine, then holds each part under the work it is for:
/// a review's four kinds, each a run a seat reads alone, above the part of the review's captain; and
/// an implementer's tests, code, docs and recipes (DYD-1372 07c86aad, aa5b445e).
/// </summary>
public static class CaptainCarrier
{
    private static readonly Resource Implement = new(CaptainResource.Implement, "Implement",
        Slot.Paragraph(CaptainSlot.ImplementLead), Slot.Paragraph(CaptainSlot.ImplementMap), Slot.Bullets(CaptainSlot.ImplementRules),
        Slot.Bullets(CaptainSlot.ImplementTests, "Tests:"), Slot.Bullets(CaptainSlot.ImplementCode, "Code:"), Slot.Bullets(CaptainSlot.ImplementDocs, "Docs:"),
        Slot.Bullets(CaptainSlot.ImplementBug, "On a Bug:"), Slot.Bullets(CaptainSlot.ImplementFix, "On a Fix:"), Slot.Bullets(CaptainSlot.ImplementConflict, "On a merge conflict:"),
        Slot.Bullets(CaptainSlot.ImplementShoulds, "On a batch of shoulds:"), Slot.Paragraph(CaptainSlot.ImplementEnd, "To finish:"));

    private static readonly Resource Review = new(CaptainResource.Review, "Review",
        Slot.Paragraph(CaptainSlot.ReviewLead), Slot.Paragraph(CaptainSlot.ReviewMap), Slot.Bullets(CaptainSlot.ReviewAll),
        Slot.Paragraph(CaptainSlot.ReviewSeat, "If you are one seat:"), Slot.Bullets(CaptainSlot.ReviewRead, "Reading:"),
        Slot.Bullets(CaptainSlot.ReviewBreak, "Trying to break it:"), Slot.Bullets(CaptainSlot.ReviewDrive, "Driving the feature:"),
        Slot.Bullets(CaptainSlot.ReviewModel, "Modelling it as a statechart:"), Slot.Paragraph(CaptainSlot.ReviewRow, "If you hold the review:"),
        Slot.Bullets(CaptainSlot.ReviewRules))
    {
        // A review's captain cuts its ticket into seats and, once they are back, goes straight to finishing.
        States = [CaptainState.Split, CaptainState.Waiting, CaptainState.Finishing]
    };

    private static readonly Resource Research = new(CaptainResource.Research, "Research",
        Slot.Paragraph(CaptainSlot.ResearchLead), Slot.Paragraph(CaptainSlot.ResearchMap), Slot.Steps(CaptainSlot.ResearchSteps), Slot.Paragraph(CaptainSlot.ResearchQuestion, "On a Question ticket:"),
        Slot.Bullets(CaptainSlot.ResearchRoutes))
    {
        // A Question whose research does not settle its call leaves the leaf: handed on to the user, or back to the officer.
        States = [CaptainState.Leaf]
    };

    // The route of each check that fails before a ticket is started: read only by the holder it stops.
    private static readonly Resource Start = new(CaptainResource.Start, "Before you start", Slot.Bullets(CaptainSlot.StartChecks));

    private static readonly Resource SubTickets = new(CaptainResource.SubTickets, "Sub-tickets", Slot.Bullets(CaptainSlot.Tickets));

    // It opens with the one line that has the wayfinder skill loaded, and says how far that skill reaches.
    private static readonly Resource Wayfind = new(CaptainResource.Wayfind, "Finding the way",
        Slot.Paragraph(CaptainSlot.WayLead), Slot.Steps(CaptainSlot.WaySteps), Slot.Bullets(CaptainSlot.WayUser, "The user's part:"));

    private static readonly Resource Briefs = new(CaptainResource.Briefs, "Briefs",
        Slot.Paragraph(CaptainSlot.BriefLead), Slot.Bullets(CaptainSlot.BriefAll, "Every brief names:"), Slot.Bullets(CaptainSlot.BriefReview, "A review brief also names:"),
        Slot.Paragraph(CaptainSlot.BriefNone));

    private static readonly Resource Filler = new(CaptainResource.Filler, "Filler",
        Slot.Paragraph(CaptainSlot.FillerLead), Slot.Bullets(CaptainSlot.FillerRules), Slot.Paragraph(CaptainSlot.FillerEmpty, "No ticket left."));

    private static readonly Resource Round = new(CaptainResource.Round, "A round's record", Slot.Steps(CaptainSlot.RoundSteps), Slot.Paragraph(CaptainSlot.RoundRules));

    private static readonly Resource Findings = new(CaptainResource.Findings, "A finding's route",
        Slot.Bullets(CaptainSlot.BlockerRoutes, "A blocker, by what it shows:"), Slot.Bullets(CaptainSlot.ShouldRoutes, "A should, by what its fix costs:"),
        Slot.Paragraph(CaptainSlot.OwnChange));

    // It opens with whose the merge is: a holder whose officer merges reads no further. Then the one order of a
    // PR, in its two moments: opened as a draft before the review, and made ready and merged after it (gap hunt 5, H2).
    private static readonly Resource Merging = new(CaptainResource.Merging, "Merging",
        Slot.Paragraph(CaptainSlot.MergeLead), Slot.Paragraph(CaptainSlot.MergePr, "Before the review:"), Slot.Steps(CaptainSlot.MergeSteps, "Once the rounds have ended:"),
        Slot.Bullets(CaptainSlot.MergeRules));

    // Where a holder woken in its own thread goes on from: what it re-reads, and what each return asks of it.
    private static readonly Resource Waking = new(CaptainResource.Waking, "Waking", Slot.Paragraph(CaptainSlot.WakeLead), Slot.Steps(CaptainSlot.WakeSteps));

    public static readonly SkillSpec Spec = SkillSpec.Officer(CaptainSkill.TicketCaptain, "Ticket Captain",
        "A ticket you were dispatched onto, of any kind: hold it to its outcome.") with
    {
        Readers = [Actor.TicketCaptain],
        Ledger = CaptainLedger.Fates,
        Phases =
        [
            new("Take the ticket", Slot.Paragraph(CaptainSlot.Take), Slot.Paragraph(CaptainSlot.Kinds), Slot.Paragraph(CaptainSlot.Hold))
            {
                States = [CaptainState.Dispatched, CaptainState.Taken],
                Resources = [Implement, Review, Research, Start, Waking]
            },
            new("Do it or split it", Slot.Paragraph(CaptainSlot.Leaf, "A leaf."), Slot.Paragraph(CaptainSlot.Fog, "Fog."), Slot.Paragraph(CaptainSlot.Split, "A split."))
            {
                States = [CaptainState.Taken, CaptainState.Leaf, CaptainState.Wayfinding, CaptainState.Split],
                Resources = [SubTickets, Wayfind]
            },
            new("Dispatch", Slot.Paragraph(CaptainSlot.Briefs, "Briefs."), Slot.Paragraph(CaptainSlot.Seats, "Seats."), Slot.Paragraph(CaptainSlot.Sending, "Sending."),
                Slot.Paragraph(CaptainSlot.Lanes, "Lanes."))
            {
                States = [CaptainState.Split, CaptainState.Waiting],
                Resources = [Briefs, Filler]
            },
            new("Get it reviewed", Slot.Paragraph(CaptainSlot.Reviewed), Slot.Steps(CaptainSlot.Rounds))
            {
                States = [CaptainState.Leaf, CaptainState.Waiting, CaptainState.InReview, CaptainState.Clear, CaptainState.Fixing, CaptainState.Mending],
                Resources = [Round, Findings]
            },
            new("Finish", Slot.Paragraph(CaptainSlot.Who), Slot.Bullets(CaptainSlot.Cases), Slot.Paragraph(CaptainSlot.Close))
            {
                States = [CaptainState.Finishing, CaptainState.Done, CaptainState.Returned],
                Resources = [Merging]
            }
        ],
        Scenarios =
        [
            new(CaptainResource.TooBig, "Too big to hold", Slot.Steps(CaptainSlot.TooBigSteps), Slot.Paragraph(CaptainSlot.TooBigChild), Slot.Steps(CaptainSlot.TooBigOfficer),
                Slot.Paragraph(CaptainSlot.TooBigEnd))
            {
                States = [CaptainState.Leaf, CaptainState.Waiting]
            },
            new(CaptainResource.FixRounds, "Fix rounds", Slot.Bullets(CaptainSlot.FixRules), Slot.Paragraph(CaptainSlot.FixCheckpoint, "The checkpoint."))
            {
                States = [CaptainState.Fixing, CaptainState.InReview, CaptainState.Checkpoint]
            },
            new(CaptainResource.MergeConflict, "A merge conflict", Slot.Steps(CaptainSlot.ConflictSteps), Slot.Paragraph(CaptainSlot.ConflictEnd))
            {
                States = [CaptainState.Finishing, CaptainState.Repairing]
            },
            new(CaptainResource.RedGate, "A red gate after the merge", Slot.Paragraph(CaptainSlot.RedLead), Slot.Steps(CaptainSlot.RedSteps, "Red again is real:"),
                Slot.Paragraph(CaptainSlot.RedEnd))
            {
                States = [CaptainState.Finishing, CaptainState.Repairing]
            },
            new(CaptainResource.NeedForTheUser, "A need for the user", Slot.Paragraph(CaptainSlot.NeedLead), Slot.Steps(CaptainSlot.NeedSteps), Slot.Bullets(CaptainSlot.NeedRules))
            {
                States = [CaptainState.Question]
            }
        ]
    };
}

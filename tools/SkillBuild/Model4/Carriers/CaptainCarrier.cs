namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype (DYD-1372), the first truth-checked skill: built to
/// <c>skills/roles/officers/ticket-captain/</c> in place of the legacy <see cref="TicketCaptainCarrier"/>,
/// whose lines stay in the model, unbuilt, and are accounted for in <see cref="CaptainLedger"/>. One
/// captain base (option A, DYD-1295 0f340abd): five phases in their real order, three kind resources
/// its first phase links, the resources a later phase's line links, and one scenario per failure.
/// </summary>
public static class CaptainCarrier
{
    private static readonly Resource Implement = new(CaptainResource.Implement, "Implement",
        Slot.Paragraph(CaptainSlot.ImplementLead), Slot.Bullets(CaptainSlot.ImplementRules),
        Slot.Bullets(CaptainSlot.ImplementFix, "On a Fix:"), Slot.Bullets(CaptainSlot.ImplementConflict, "On a merge conflict:"),
        Slot.Bullets(CaptainSlot.ImplementShoulds, "On a batch of shoulds:"), Slot.Paragraph(CaptainSlot.ImplementEnd, "To finish:"));

    private static readonly Resource Review = new(CaptainResource.Review, "Review",
        Slot.Paragraph(CaptainSlot.ReviewLead), Slot.Paragraph(CaptainSlot.ReviewRow),
        Slot.Bullets(CaptainSlot.ReviewRules), Slot.Bullets(CaptainSlot.ReviewHolding, "Your return:"));

    private static readonly Resource Research = new(CaptainResource.Research, "Research",
        Slot.Paragraph(CaptainSlot.ResearchLead), Slot.Steps(CaptainSlot.ResearchSteps), Slot.Paragraph(CaptainSlot.ResearchQuestion, "On a Question ticket:"));

    private static readonly Resource Briefs = new(CaptainResource.Briefs, "Briefs",
        Slot.Paragraph(CaptainSlot.BriefLead), Slot.Bullets(CaptainSlot.BriefAll, "Every brief names:"), Slot.Bullets(CaptainSlot.BriefReview, "A review brief also names:"),
        Slot.Paragraph(CaptainSlot.BriefNone));

    private static readonly Resource Filler = new(CaptainResource.Filler, "Filler", Slot.Paragraph(CaptainSlot.FillerLead), Slot.Bullets(CaptainSlot.FillerRules));

    private static readonly Resource Round = new(CaptainResource.Round, "A round's record", Slot.Steps(CaptainSlot.RoundSteps), Slot.Bullets(CaptainSlot.RoundRules));

    private static readonly Resource Merging = new(CaptainResource.Merging, "Merging", Slot.Steps(CaptainSlot.MergeSteps), Slot.Bullets(CaptainSlot.MergeRules));

    public static readonly SkillSpec Spec = SkillSpec.Officer(CaptainSkill.TicketCaptain, "Ticket Captain",
        "A ticket you were dispatched onto, of any kind: hold it to its outcome.") with
    {
        Readers = [Actor.TicketCaptain],
        Ledger = CaptainLedger.Fates,
        Phases =
        [
            new("Take the ticket", Slot.Paragraph(CaptainSlot.Take), Slot.Paragraph(CaptainSlot.Kinds), Slot.Paragraph(CaptainSlot.Hold))
            {
                States = [CaptainState.Taken],
                Resources = [Implement, Review, Research]
            },
            new("Do it or split it", Slot.Paragraph(CaptainSlot.Leaf, "A leaf."), Slot.Paragraph(CaptainSlot.Split, "A split."))
            {
                States = [CaptainState.Taken, CaptainState.Leaf, CaptainState.Split]
            },
            new("Dispatch", Slot.Paragraph(CaptainSlot.Briefs, "Briefs."), Slot.Paragraph(CaptainSlot.Seats, "Seats."), Slot.Paragraph(CaptainSlot.Sending, "Sending."),
                Slot.Paragraph(CaptainSlot.Lanes, "Lanes."), Slot.Paragraph(CaptainSlot.Returns, "Returns."))
            {
                States = [CaptainState.Split, CaptainState.Waiting],
                Resources = [Briefs, Filler]
            },
            new("Get it reviewed", Slot.Paragraph(CaptainSlot.Reviewed), Slot.Steps(CaptainSlot.Rounds),
                Slot.Bullets(CaptainSlot.Routes, "A blocker's route, by what it shows:"), Slot.Paragraph(CaptainSlot.Mine))
            {
                States = [CaptainState.Leaf, CaptainState.Waiting, CaptainState.InReview, CaptainState.Fixing],
                Resources = [Round]
            },
            new("Finish", Slot.Paragraph(CaptainSlot.Who), Slot.Bullets(CaptainSlot.Cases), Slot.Paragraph(CaptainSlot.Close))
            {
                States = [CaptainState.Finishing, CaptainState.Done, CaptainState.Returned],
                Resources = [Merging]
            }
        ],
        Scenarios =
        [
            new(CaptainResource.TooBig, "Too big to hold", Slot.Steps(CaptainSlot.TooBigSteps), Slot.Paragraph(CaptainSlot.TooBigChild)) { States = [CaptainState.Leaf] },
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
                States = [CaptainState.Finishing]
            },
            new(CaptainResource.NeedForTheUser, "A need for the user", Slot.Paragraph(CaptainSlot.NeedLead), Slot.Bullets(CaptainSlot.NeedRules)) { States = [CaptainState.Question] }
        ]
    };
}

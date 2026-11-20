namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>
/// The truth fixture (DYD-1372): a truth-checked keeper with every slot shape, a kind resource, a
/// scenario and a machine its phases cover; and, for the ledger, the legacy keeper it replaces beside a
/// legacy clerk that stays built.
/// </summary>
internal static class TruthModel
{
    public static readonly Machine Watch = new(KeeperMachine.Watch, "Watch",
        [new(KeeperState.Held, "Held"), new(KeeperState.Sent, "Sent"), new(KeeperState.Done, "Done")], KeeperState.Held, [KeeperState.Done]);

    public static readonly Resource Guide = new(KeeperSlot.Guide, "Guide", Slot.Paragraph(KeeperSlot.GuideLead), Slot.Bullets(KeeperSlot.GuideRules, "On a fix:"));

    public static readonly Resource Stuck = new(KeeperSlot.Stuck, "Stuck", Slot.Steps(KeeperSlot.StuckSteps)) { States = [KeeperState.Sent] };

    public static readonly Phase[] Phases =
    [
        new("Take it", Slot.Paragraph(KeeperSlot.Take), Slot.Paragraph(KeeperSlot.Kinds)) { States = [KeeperState.Held], Resources = [Guide] },
        new("Send it", Slot.Paragraph(KeeperSlot.Briefs, "Briefs."), Slot.Bullets(KeeperSlot.Seats, "Seats."),
            Slot.Steps(KeeperSlot.Merge, "If you merge it:"), Slot.Bullets(KeeperSlot.Extra)) { States = [KeeperState.Sent] }
    ];

    public static readonly SkillSpec Keeper = SkillSpec.Crew(KeeperSkill.Keeper, "Keeper", "Keeps one thing.") with
    {
        Readers = [KeeperActor.Keeper],
        Phases = Phases,
        Scenarios = [Stuck]
    };

    public static readonly SkillSpec OldKeeper = SkillSpec.Crew(OldSkill.Keeper, "Keeper", "Keeps one thing, the old way.") with
    {
        Phases = [new("Do it", new Slot(OldSlot.Work)) { Resources = [new(OldSlot.Manual, "Manual", new Slot(OldSlot.ManualLead))] }],
        Scenarios = [new(OldSlot.Stale, "Stale", new Slot(OldSlot.StaleSteps))]
    };

    public static readonly SkillSpec Clerk = SkillSpec.Crew(OldSkill.Clerk, "Clerk", "Keeps the desk.") with
    {
        Phases = [new("Desk", new Slot(OldSlot.Desk))]
    };

    /// <summary>The whole legacy spine accounted for: a line kept, one dropped, one deferred, a resource kept and a scenario deferred.</summary>
    public static readonly Fate[] Entries =
    [
        Fate.Kept(OldMoves.Owns, KeeperMoves.Owns),
        Fate.Dropped(OldMoves.Cap, Question.Handled, "The proxy already limits the runs."),
        Fate.Deferred(OldMoves.Survey, "Its manual is not rebuilt here."),
        Fate.Kept(OldSlot.Manual, KeeperSlot.Guide),
        Fate.Deferred(OldSlot.Stale, "Not rebuilt here.")
    ];

    public static Model With(params Type[] kinds) => Of(Keeper, kinds);

    public static Model Of(SkillSpec keeper, params Type[] kinds) => new([typeof(KeeperMoves), .. kinds], [keeper]) { Machines = [Watch] };

    /// <summary>The keeper replacing the legacy keeper through a ledger of <paramref name="entries"/>, the clerk beside them.</summary>
    public static Model Replacing(params Fate[] entries) =>
        new([typeof(KeeperMoves), typeof(OldMoves)], [OldKeeper, Keeper with { Ledger = new(OldSkill.Keeper, entries) }, Clerk]) { Machines = [Watch] };
}

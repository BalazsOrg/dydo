namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>
/// The truth fixture's claims: a line in every slot shape, a table, a combined line, two transitions
/// placed in plain slots of the phases that cover them, a command that exists, a kind resource and a scenario.
/// </summary>
public static class KeeperMoves
{
    public static readonly Claim Owns = new("The keeper keeps one thing to its end.", [KeeperActor.Keeper],
        [Line("You keep one thing to its end.", SpinePart.Job, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Read = new("The contract is read first.", [KeeperActor.Keeper],
        [Line("Read the contract first.", KeeperSlot.Take, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Load = new("The kind's resource is loaded.", [KeeperActor.Keeper],
        [Line("Then load your kind's resource.", KeeperSlot.Take, 2)]) { Truth = Verified.ByRun };

    public static readonly Claim Kinds = new("Each kind has a resource.", [KeeperActor.Keeper],
        [Line("| Kind | Resource |\n| -- | -- |\n| Change | Guide |", KeeperSlot.Kinds, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Send = new("A held thing is sent with a brief.", [KeeperActor.Keeper],
        [Line("Brief from the template.", KeeperSlot.Briefs, 1)])
    {
        Truth = Verified.ByRun,
        Transition = new(KeeperState.Held, KeeperState.Sent, "the keeper sends it")
    };

    public static readonly Claim Lenses = new("A brief names its lenses.", [KeeperActor.Keeper],
        [Line("Name the lenses in each brief.", KeeperSlot.Briefs, 2)]) { Truth = Verified.ByRun };

    public static readonly Claim Family = new("A reviewer sits from another family.", [KeeperActor.Keeper],
        [Line("Seat a reviewer from another family when you can.", KeeperSlot.Seats, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Scale = new("Seats scale with the work.", [KeeperActor.Keeper],
        [Line("Scale the seats to the work `dydo survey <project>` shows.", KeeperSlot.Seats, 2)])
    {
        Truth = Verified.ByRun with { Evidence = Evidence.Command("dydo survey <project>") }
    };

    public static readonly Claim Merge = new("The merge goes through the command.", [KeeperActor.Keeper], []) { Truth = Verified.ByRun };

    public static readonly Claim Gate = new("The gate runs before the merge.", [KeeperActor.Keeper],
        [new(KeeperSkill.Keeper, KeeperActor.Keeper, "Run the project's gate, then `dydo merge <pr> --dry-run`.", new(KeeperSlot.Merge, 1), () => [Merge])])
    {
        Truth = Verified.ByRun
    };

    public static readonly Claim Close = new("A sent thing is closed once merged.", [KeeperActor.Keeper],
        [Line("Close the ticket.", KeeperSlot.Merge, 2)])
    {
        Truth = Verified.ByRun,
        Transition = new(KeeperState.Sent, KeeperState.Done, "the keeper closes it")
    };

    public static readonly Claim Return = new("The keeper returns one line.", [KeeperActor.Keeper],
        [Line("Return one line.", KeeperSlot.Merge, 3)]) { Truth = Verified.ByRun };

    public static readonly Claim Limit = new("A rate limit is waited out.", [KeeperActor.Keeper],
        [Line("On a rate limit, wait, then retry.", SpinePart.WhenThingsGoWrong, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Save = new("Stuck work is saved first.", [KeeperActor.Keeper],
        [Line("Save the work.", KeeperSlot.StuckSteps, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Ask = new("A stuck keeper asks its officer.", [KeeperActor.Keeper],
        [Line("Ask your officer.", KeeperSlot.StuckSteps, 2)]) { Truth = Verified.ByRun };

    public static readonly Claim Change = new("The guide covers a change.", [KeeperActor.Keeper],
        [Line("A change is implemented, fixed or merged here.", KeeperSlot.GuideLead, 1)]) { Truth = Verified.ByRun };

    public static readonly Claim Blockers = new("A fix closes only the blockers.", [KeeperActor.Keeper],
        [Line("Fix only the blockers.", KeeperSlot.GuideRules, 1)]) { Truth = Verified.ByRun };

    private static Line Line(string text, Enum slot, int order) => new(KeeperSkill.Keeper, KeeperActor.Keeper, text, new(slot, order));
}

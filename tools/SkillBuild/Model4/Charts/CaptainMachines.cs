namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's one machine (DYD-1372 A8): the states of a captain holding one
/// ticket, from taking it to returning. Its arrows are the prototype's own claims. <c>round</c> counts
/// review rounds and is bounded only so TLC ends; the skill sets no cap. A round that found no new
/// blocker is a state of its own: the rounds end from it, and the one more round the captain may order
/// leaves only from it, once (<c>extra</c>), so a round with a blocker reaches the next only through a
/// Fix (R8). A third round that still has a blocker leaves only through the checkpoint (R10), and a
/// repaired conflict or red gate goes on only through a review round, or back to the officer when it
/// cannot be finished (R19). A splitting captain's Question is one more child it waits on. A leaf's
/// Question is left only to resume the leaf: one that comes back unsettled goes to the officer and
/// stays open, and its holder goes on waiting, a step that leaves the state unchanged (H16, S10).
/// A filler ticket that fails its check goes back from Taken, before it is started (H11).
/// A captain that cannot see the route from its ticket to the spec finds the way first (H10, DYD-1372
/// 07c86aad): that state is left only by cutting the parts in sight, and entered again whenever a
/// part returns and fog remains, so the whole is never cut at once.
/// A review's captain cuts its ticket for a second reason, the family of its seats: a seat from
/// another model family goes out on a leaf sub-ticket of its own. Once its seats are back it goes
/// straight to finishing, because a verdict is never reviewed (R34, S1, DYD-1372 07c86aad, aa5b445e).
/// </summary>
public static class CaptainMachines
{
    public static readonly Machine Captain = new(MachineId.Captain, "A captain and its ticket",
    [
        new(CaptainState.Taken, "Taken", "S6", "S15"),
        new(CaptainState.Leaf, "Working it as a leaf", "S2", "S7"),
        new(CaptainState.Wayfinding, "Finding the way to the spec", "H10", "H23"),
        new(CaptainState.Split, "Split into sub-tickets", "S1", "S15"),
        new(CaptainState.Waiting, "Waiting on children, a Question's captain among them", "H3", "H16"),
        new(CaptainState.InReview, "In review", "R2", "R8"),
        new(CaptainState.Clear, "A round found no new blocker", "R8"),
        new(CaptainState.Fixing, "Fixing", "S9", "R8"),
        new(CaptainState.Checkpoint, "At the checkpoint after a third blocked round", "R10", "S10"),
        new(CaptainState.Repairing, "Repairing a conflict or a red gate", "R19", "R21", "R22"),
        new(CaptainState.Question, "A leaf's own work waiting on a Question", "H16"),
        new(CaptainState.Finishing, "Finishing", "R21", "S6"),
        new(CaptainState.Done, "Done", "S6"),
        new(CaptainState.Returned, "Returned to its officer", "H23")
    ], CaptainState.Taken, [CaptainState.Returned])
    {
        Extends = ["Naturals"],
        Variables = [new("round", "1", "round <= 4"), new("extra", "FALSE", "extra \\in BOOLEAN")]
    };
}

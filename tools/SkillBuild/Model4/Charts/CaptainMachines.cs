namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's one machine (DYD-1372 A8): the states of a captain holding one
/// ticket, from taking it to returning. Its arrows are the prototype's own claims. <c>round</c> counts
/// review rounds and is bounded only so TLC ends; the skill sets no cap. A round that found no new
/// blocker is a state of its own: the rounds end from it, and the one more round the captain may order
/// leaves only from it, once (<c>extra</c>), so a round with a blocker reaches the next only through a
/// Fix (R8). A clear round may leave a cheap should: a writer mends it on its own ticket, a state of its
/// own that only a clear round enters. Where the candidate is in the holder's PR, the new head then gets
/// a round, which <c>dydo merge</c> needs and which is not that one more round. Where it is in no PR,
/// its officer merging it, the mend is left for finishing: the mended head is returned and reviewed
/// with its whole (R2, R23). Each way out is guarded on who merges (<c>merger</c>, below), and a should
/// on a change nobody merges is never mended there, since no later Review would judge the mend: it
/// waits on its ticket (review r3, B1 and N1). The mend happens once (<c>mended</c>): a should a later clear round finds waits on its
/// ticket, so the loop ends (S17, DYD-1295 41a553f1).
/// A third round that still has a blocker leaves only through the checkpoint (R10), and a
/// repaired conflict or red gate goes on only through a review round, or back to the officer when it
/// cannot be finished (R19). A splitting captain's Question is one more child it waits on. A leaf's
/// Question is left only to resume the leaf: one that comes back unsettled goes to the officer and
/// stays open, and its holder goes on waiting, a step that leaves the state unchanged (H16, S10).
/// A research holder leaves its leaf three ways: with the answer; with the call handed on to the user
/// as a HITL Question of its own, its research ticket finished; or back to its officer, unsettled (H23).
/// One that divided a question too big for one agent leaves its wait back to the leaf, where it checks
/// the parts' answers and writes its own: the three ways out are the leaf's, so none leads from the wait
/// (H2, K30, gap hunt 6, J4).
/// A ticket is taken by one move, from Dispatched: its holder sets it In Progress and loads its kind's
/// resource, and so learns what its result is. <c>kind</c> holds that from then on: <c>change</c>;
/// <c>verdict</c>, a review's verdict or a seat's findings; or <c>answer</c>, what a research holder or
/// a Question's captain returns. Only a change enters a review round or a repair, and only a verdict's
/// holder seats a round and forms its verdict, so no path leads a verdict into a review (R34, DYD-1372
/// 777d396e). An answer is no verdict, and no rule speaks of reviewing one: the machine shows no round
/// on it only because the skill states none, its review phase being about a change.
/// The same move teaches who merges the work, which the brief says: <c>merger</c> is the <c>holder</c>,
/// its <c>officer</c> or <c>nobody</c>, and <c>unread</c> before; a verdict or an answer is merged by nobody.
/// Only a merging holder has a round of its own on a leaf's change, repairs a conflict or a red gate, or
/// gets a round on a mended head; and it never finishes with no review of its own, so it reaches finishing
/// from a clear round alone: <c>dydo merge</c> takes no head without a PASS in its PR (R23, R21, review r3, B1).
/// A ticket that cannot be started goes back from Dispatched: a filler ticket that fails its check
/// (H11), and one another agent already holds (S7).
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
        new(CaptainState.Dispatched, "Dispatched onto a ticket, not yet started", "H3", "S15"),
        new(CaptainState.Taken, "Taken: In Progress, its kind known", "S6", "K30"),
        new(CaptainState.Leaf, "Working it as a leaf", "S2", "S7"),
        new(CaptainState.Wayfinding, "Finding the way to the spec", "H10", "H23"),
        new(CaptainState.Split, "Split into sub-tickets", "S1", "S15"),
        new(CaptainState.Waiting, "Waiting on children, a Question's captain among them", "H3", "H16"),
        new(CaptainState.InReview, "In review", "R2", "R8"),
        new(CaptainState.Clear, "A round found no new blocker", "R8"),
        new(CaptainState.Fixing, "Fixing", "S9", "R8"),
        new(CaptainState.Mending, "Mending a cheap should after a clear round", "S17", "R8"),
        new(CaptainState.Checkpoint, "At the checkpoint after a third blocked round", "R10", "S10"),
        new(CaptainState.Repairing, "Repairing a conflict or a red gate", "R19", "R21", "R22"),
        new(CaptainState.Question, "A leaf's own work waiting on a Question", "H16"),
        new(CaptainState.Finishing, "Finishing", "R21", "S6"),
        new(CaptainState.Done, "Done", "S6"),
        new(CaptainState.Returned, "Returned to its officer", "H23")
    ], CaptainState.Dispatched, [CaptainState.Returned])
    {
        Extends = ["Naturals"],
        Variables =
        [
            new("round", "1", "round <= 4"), new("extra", "FALSE", "extra \\in BOOLEAN"), new("mended", "FALSE", "mended \\in BOOLEAN"),
            new("kind", "\"unread\"", "kind \\in {\"unread\", \"change\", \"verdict\", \"answer\"}"),
            new("merger", "\"unread\"", "merger \\in {\"unread\", \"holder\", \"officer\", \"nobody\"}")
        ]
    };

    /// <summary>The guard of a move only a change's holder makes: into a review round, or into a repair.</summary>
    public const string Change = "kind = \"change\"";

    /// <summary>The guard of a move only a verdict's holder makes: seating a round, and forming its verdict.</summary>
    public const string Verdict = "kind = \"verdict\"";

    /// <summary>
    /// The guard of a move only an answer's holder makes: back from its wait to its own work, once the parts
    /// of a divided question return, and a research holder's two ways out of its leaf with a call still open.
    /// </summary>
    public const string Answer = "kind = \"answer\"";

    /// <summary>The guard of a move only the holder its brief gives the merge makes: the round on a mended head.</summary>
    public const string Merges = "merger = \"holder\"";

    /// <summary>The guard of a merging holder's move on its change: a round of its own on a leaf, and a repair before the merge.</summary>
    public const string MergesChange = Change + " /\\ " + Merges;

    /// <summary>The guard of a move closed to a merging holder: finishing with no review of its own.</summary>
    public const string NoMerge = "merger # \"holder\"";

    /// <summary>The guard of the hand-back of a mended head with no round of its own: the holder's officer merges it.</summary>
    public const string OfficerMerges = "merger = \"officer\"";

    /// <summary>The guard of the mend after a clear round: somebody merges the change, so a later Review or the merge's own judges it.</summary>
    public const string Merged = "merger # \"nobody\"";

    /// <summary>
    /// The effect of the move that takes a ticket: its holder learns which of the three its result is, and
    /// who merges it: a change its holder, its officer or nobody, and a verdict or an answer nobody. An
    /// answer's holder takes its own three moves, and every move that is open to any result.
    /// </summary>
    public const string Learn = "kind' \\in {\"change\", \"verdict\", \"answer\"} /\\ merger' \\in IF kind' = \"change\" THEN {\"holder\", \"officer\", \"nobody\"} ELSE {\"nobody\"}";
}

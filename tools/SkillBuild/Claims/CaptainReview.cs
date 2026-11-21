namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's "Get it reviewed" and the route of each finding a round returns
/// (DYD-1372). The captain dispatches reviews and never reviews (DYD-1295 2119cd65); the severities
/// are defined here, where they are used, in the user's words (41a553f1); and a captain makes no call
/// on a finding about a change it wrote itself (spec R33).
/// </summary>
public static class CaptainReview
{
    private const string Rounds = "Linear, DYD-1281's sub-tickets read 2026-11-20: Review r1 to r4, each after the first titled a fresh review that follows Fix r1, r2 or r3.";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Risk = new("Persistence, migrations, permissions, guard rules and uncertain native interfaces are always reviewed.", Me, [])
    {
        Spec = ["R16"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` asks every PR for a PASS and cannot tell these five apart from any other change inside one.",
            Reason: "A mistake here is costly or impossible to undo once it has run.")
    };

    public static readonly Claim Other = new("Every change is reviewed by an agent other than its writer, unless it is too small to need it.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Reviewed, 1,
            "Every change is reviewed by an agent other than its writer, unless too small to need it; persistence, migrations, permissions, guard rules and uncertain native interfaces always are.",
            () => [Risk])
    ])
    {
        Spec = ["R33", "R12"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "`dydo merge` refuses a PR with no PASS line and cannot tell who wrote the line.",
            Reason: "A writer's own context hides from it what a fresh one sees at once."),
        Transition = new(CaptainState.Leaf, CaptainState.InReview, "the leaf's change is whole: another agent reviews it") { Guard = CaptainMachines.Change }
    };

    public static readonly Claim Pr = new("The captain that merges its work opens the PR for it, from a clean and pushed tree, before its review.", Me, [])
    {
        Spec = ["R17", "R21"],
        Truth = new(Evidence.Command("dydo pr-ready"),
            Layer: "The guard refuses a `gh` PR call from a dirty or unpushed tree and reads no other way of opening one; nothing opens the PR for a captain.",
            Reason: "The verdict's PASS line and `dydo merge` both need a PR that shows exactly the reviewed commit.")
    };

    public static readonly Claim Whose = new("Whoever merges a piece of work has it reviewed first; a part is reviewed with the parent it joins.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Reviewed, 2,
            "If the merge is yours, open a PR for the work once `dydo pr-ready` passes, then have it reviewed in that PR before you merge; a part your officer merges is reviewed with its whole.",
            () => [Pr])
    ])
    {
        Spec = ["R2"],
        Truth = new(Evidence.Code("Services/Merge/PullRequestMerge.cs"),
            Layer: "`dydo merge` refuses a head with no PASS, so the merger finds out at the merge; nothing tells it before.",
            Reason: "A part reviewed alone and again inside its parent is reviewed twice for one answer.")
    };

    public static readonly Claim Attach = new("The captain that opens a PR puts the ticket's key in its title and attaches its link to the ticket itself.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Reviewed, 3,
            "Put your ticket's key in that PR's title, and attach its link to the ticket yourself, with Linear's `save_issue` (`links`). (open: his call)")
    ])
    {
        Spec = ["H3", "K32"],
        Truth = new(Evidence.Tool("save_issue: `links`, \"Link attachments to add [{url, title}]. Append-only; existing links are never removed\". Run, Linear read 2026-11-21: nothing attached a PR by its title or its branch. " +
                                  "PR 379 (`DYD-1372 …`, branch `DYD-1372-captain-prototype`), PR 374 (`DYD-1192: …`, `DYD-1192-remove-association-manifest`), PR 373 (`DYD-1157 fix: …`, `t3/restore-row-deadline-helpers`) " +
                                  "and PR 372 (`DYD-1172 fix: …`, `t3/dydo-1172-safe-delete-diagnostics`) left their tickets with no attachment. The tickets that carry a PR have a hand-written title on it " +
                                  "(DYD-1281 \"PR #378: the rebuilt 4.0.0 candidate (do not merge)\", DYD-1206, DYD-1196, DYD-968), and DYD-1372 carries \"PR 379: DYD-1372 ticket-captain prototype\" " +
                                  "since its captain attached it with `save_issue` that day. Open, spec section 6, question 2: the user chose to keep the key in the PR's title on the belief that Linear then links it, and has yet to choose between this and setting that link up."),
            Layer: "Linear's GitHub link attaches nothing here, by a PR's title or by a branch's name, and `dydo merge` touches no ticket.",
            Reason: "A ticket that shows its PR leads anyone from the board to the diff and its review; without the link the two meet only in a thread that can die.")
    };

    public static readonly Claim NoLoop = new("A plan, spec or contract gets no review loop; a spec gets at most one Questions pass by a native helper, chosen by risk.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BriefNone, 1, "Write no review brief for a plan, spec or contract: it gets no review rounds, and a risky spec at most one pass by a native helper, which returns questions.")])
    {
        Spec = ["R4"],
        Truth = new(Evidence.Tool("Agent: a native sub-agent, awaited within the turn, returns its report to its caller alone."),
            Layer: "Nothing limits how often a document is sent for review.",
            Reason: "First make it work, then make it good: rounds on a plan polish what building will change.")
    };

    public static readonly Claim Severity = new("A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Reviewed, 4,
            "A reviewer gives each finding a severity. A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.")
    ])
    {
        Spec = ["R11"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Severity group holds blocker, should and nit."),
            Layer: "Linear holds the three labels and says nothing of what a captain does with each.",
            Reason: "The three words decide what happens next, so they are defined before the steps that use them.")
    };

    public static readonly Claim Groups = new("A ticket that only groups parts gets no review of its own.", Me, [])
    {
        Spec = ["R6"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Linear nests tickets for any reason; whether a parent integrates anything is known to its captain.",
            Reason: "A grouping adds no behaviour for a reviewer to find wrong."),
        Transition = new(CaptainState.Waiting, CaptainState.Finishing, "the children are back and the ticket only groups parts: no review of its own")
    };

    public static readonly Claim Whole = new("Once its parts are wired, a parent is reviewed once: each part, the parts together, their harmony.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Rounds, 1,
            "Once the parts are wired, dispatch a Review ticket for the whole: each part, the parts together, their harmony. A ticket that only groups parts needs none.",
            () => [Groups])
    ])
    {
        Spec = ["R2", "R1"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Nothing dispatches a review when the last part returns.",
            Reason: "Early enough that nothing is built on a fault, late enough that the whole can be seen working."),
        Transition = new(CaptainState.Waiting, CaptainState.InReview, "the parts are wired: a Review of the whole is dispatched") { Guard = CaptainMachines.Change }
    };

    public static readonly Claim Record = new("A round is recorded by the holder of its Review ticket, alone or with seats; the captain that dispatched the ticket records none of it.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.Rounds, 2,
            "The Review's holder records the round, by [A round's record](resources/round.md); you record none of it.")
    ])
    {
        Spec = ["S8", "R33"],
        Truth = new(Evidence.Run(Rounds + " Each round's verdict is on its own Review ticket. His words (DYD-1372 aa5b445e): the review has a captain, " +
                                 "which \"will decide what kind of a review matrix to use and how to manage it's sub agents\"."),
            Layer: "Each seat returns its findings to the review's captain alone, so the captain of the change never has them to put together.",
            Reason: "One verdict per round, however many seats looked, and none of it written by a captain with work of its own under that verdict.")
    };

    public static readonly Claim Route = new("The officer running the rounds routes each blocker and each should by what it shows.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Rounds, 3, "Route each blocker and each should, by [A finding's route](resources/findings.md).")])
    {
        Spec = ["R9", "S17"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "A FAIL stops `dydo merge` and sends the finding nowhere; a should stops nothing at all.",
            Reason: "Each kind has one place that can act on it, and the wrong place sits on it.")
    };

    public static readonly Claim Fix = new("Every blocker of a round on work that is broken, against the spec or a security hole goes to that round's one Fix ticket.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.BlockerRoutes, 1,
            "Work that is broken, against the spec or a security hole: the round's one Fix ticket, by [Fix rounds](fix-rounds.md).")
    ])
    {
        Spec = ["R9", "S9"],
        Truth = new(Evidence.Run(Rounds + " Read again 2026-11-21: Fix r1, r2 and r3 (DYD-1327, DYD-1333, DYD-1339) are one ticket each, with no sub-ticket, and Fix r1 closed 7 blockers. " +
                                 "The user took one Fix ticket for all of a round's blockers as the walkthrough brief recommended it (DYD-1372 07c86aad, \"4. Lgtm\")."),
            Layer: "A FAIL stops `dydo merge`; it opens no ticket for the fix.",
            Reason: "A writer that holds all of them understands the code once and repairs it in one pass."),
        Transition = new(CaptainState.InReview, CaptainState.Fixing, "a round other than the third returns a blocker: a Fix ticket") { Guard = "round # 3" }
    };

    public static readonly Claim Replan = new("A fundamental blocker or a new requirement goes to the officer above for a re-plan.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BlockerRoutes, 2, "A fundamental fault, or a new requirement: your officer, for a re-plan.")])
    {
        Spec = ["R9"],
        Truth = new(Evidence.Tool("delegate_task: a child's final result returns to the thread that started it."),
            Layer: "A Fix can only repair what the plan asked for; nothing below the officer can change the plan.",
            Reason: "Fixing toward a plan that is itself wrong builds more of the wrong thing."),
        Transition = new(CaptainState.InReview, CaptainState.Returned, "in a round other than the third, a blocker is fundamental or a new requirement: back to the officer")
        {
            Guard = "round # 3"
        }
    };

    public static readonly Claim NeedsUser = new("A blocker that waits on something only the user can give goes to the officer at once, never to the user.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.BlockerRoutes, 3, "A need only the user can meet: your officer, at once, by [A need for the user](need-for-the-user.md).")])
    {
        Spec = ["R9"],
        Truth = new(Evidence.Tool("t3_thread_send: \"Send a message to any thread\"; its `mode:'auto'` \"starts an idle thread, steers a running turn, or queues\"."),
            Layer: "A FAIL stops `dydo merge` and tells nobody above the captain; the officer hears of the blocker only when the captain sends it.",
            Reason: "The user's answer is the slowest route of the three, so each hour before it is asked for lengthens the wait.")
    };

    public static readonly Claim OneMore = new("The officer running the rounds may order one more after a round with no new blocker.", Me, [])
    {
        Spec = ["R8"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "Nothing counts rounds or closes the loop.",
            Reason: "A round that barely passed can be looked at again without anyone inventing a blocker."),
        Transition = new(CaptainState.Clear, CaptainState.InReview, "one more round is ordered, once")
        {
            Guard = "~extra",
            Effect = "round' = round + 1 /\\ extra' = TRUE"
        }
    };

    public static readonly Claim Cleared = new("A round that finds no new blocker is clear, whatever shoulds and nits it found.", Me, [])
    {
        Spec = ["R8", "R11"],
        Truth = new(Evidence.Run("Linear, team Dydo, Severity labels read 2026-11-20: only the blocker is described as a FAIL, \"a review blocker stays a FAIL\"; a should is a ticket and a nit a line."),
            Layer: "`dydo merge` reads a PASS line and cannot tell a clear round from one whose blockers were left out.",
            Reason: "A should can wait, so it never keeps a round from ending."),
        Transition = new(CaptainState.InReview, CaptainState.Clear, "the round finds no new blocker")
    };

    public static readonly Claim Clean = new("The first round with no new blocker ends the review rounds.", Me,
        [At(Actor.TicketCaptain, CaptainSlot.Rounds, 4, "Repeat until a round finds no new blocker; you may order one more.", () => [OneMore, Cleared])])
    {
        Spec = ["R8"],
        Truth = new(Evidence.Run(Rounds),
            Layer: "`dydo merge` needs one PASS on the head and knows nothing of earlier rounds.",
            Reason: "The loop ends on evidence, with no cap that would let a known blocker through."),
        Transition = new(CaptainState.Clear, CaptainState.Finishing, "no further round is ordered: the review rounds end")
    };

    public static readonly Claim Mine = new("A blocker found in a change a captain wrote goes to a fresh writer.", Me, [])
    {
        Spec = ["R16", "R33"],
        Truth = new(Evidence.Tool("delegate_task: \"Start one child agent with only `task`. No parent history.\""),
            Layer: "Nothing keeps an agent from repairing its own work and calling it fixed.",
            Reason: "Whoever wrote the fault tends to defend the thinking that produced it.")
    };

    public static readonly Claim OwnFinding = new("A captain makes no call on a finding about a change it wrote: the finding stands as the reviewer wrote it, and a doubt about it is a Question.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.OwnChange, 1,
            "Take a finding on a change you wrote as the reviewer wrote it: a fresh writer fixes it, and if you doubt it, the doubt is a Question sub-ticket, never your call.",
            () => [Mine])
    ])
    {
        Spec = ["R33", "H16"],
        Truth = new(Evidence.Run("Linear, team Dydo, labels read 2026-11-20: the Type group holds Question, \"An open question that blocks planning or implementation and is too big to settle inline.\" " +
                                 "So a doubt has a ticket that a fresh captain can hold."),
            Layer: "A round's findings all come back to the captain running it, and nothing marks the ones that fall on its own change.",
            Reason: "Once its own change is under review, the captain is no longer impartial about that review.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

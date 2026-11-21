namespace DynaDocs.SkillBuild;

/// <summary>
/// The all-facts ledger of the ticket-captain prototype (DYD-1372 A7, DYD-1295 2119cd65): every claim
/// line of the legacy ticket captain's spine, in spine order, then every resource it links. Kept names
/// the prototype's claims that state the fact; dropped names the truth question the line fails;
/// deferred means its resource is not rebuilt in this ticket. No fact is dropped to save size.
/// </summary>
public static class CaptainLedger
{
    private const string Jobs = "Its resource, job tickets, is not rebuilt in DYD-1372; until then a job ticket's brief names its skill (claim Captain.Jobs).";
    private const string Chair = "It leads to a council ticket, and the council-chair resource that ticket's captain needs is not rebuilt in DYD-1372.";
    private const string Checkout = "T3 makes the checkout before the agent starts (`delegate_task`'s `isolation`), so a line on naming or creating a branch, or on posting where it is, has every agent solve it again (1ffbc4c8).";
    private const string Master = "`dydo merge` refuses the default branch as the user's click, so the route into it is not rebuilt in DYD-1372.";
    private const string Unapproved = "Not rebuilt in DYD-1372: nothing else is rebuilt until the user approves this base (519126ab).";

    public static readonly Ledger Fates = new(Skill.TicketCaptain,
    [
        Fate.Kept(Role.TicketCaptain, Captain.Owns, Captain.Officer),
        Fate.Kept(Role.Chain, Captain.Officer),
        Fate.Dropped(Role.CaptainAccounts, Question.Verify,
            "It has the captain rule on what its crew return; the captain decides only obvious calls and sends any other to a Question (519126ab, claim Captain.Calls). AGENTS.md states the ownership."),
        Fate.Kept(JobTickets.Hold, Captain.Jobs, CaptainResearch.Question)
            .Deferred("A Question", "A research Question is held by claim CaptainResearch.Question; at stage `council` or `his-call` its job skill belongs to job tickets, not rebuilt in DYD-1372."),
        Fate.Dropped(Role.CaptainTools, Question.Need,
            "No act uses a list of tools: each tool is named in the line that uses it (2119cd65). Three of its commands are not in the CLI."),
        Fate.Deferred(JobTickets.Route, Jobs),
        Fate.Deferred(Inquisition.Conduct, "Its resource, the inquisition captain, is not rebuilt in DYD-1372."),
        Fate.Kept(Coordination.FillerBack, Captain.Filler, Captain.FillerBack),
        Fate.Kept(Project.Weeds, CaptainImplement.Weed),
        Fate.Deferred(Council.Hold, Chair),
        Fate.Kept(Tree.Branch, CaptainReview.Attach)
            .Dropped("Name the ticket branch `<KEY>-<slug>` off the feature branch, or off main for an atomic ticket", Question.Handled, Checkout)
            .Dropped("so Linear attaches the branch", Question.Verify,
                "Linear attaches nothing here by a branch's name or a PR's title: PRs 379, 374, 373 and 372, read 2026-11-21, left their tickets bare. The PR is its captain's to attach (claim CaptainReview.Attach), and nothing needs the branch.")
            .Dropped("post branch, base SHA and worktree path on the ticket before the first edit", Question.Handled, Checkout),
        Fate.Kept(Tree.CrewLimits, CaptainReview.Pr, CaptainBriefs.Merger, CaptainFinish.Merged)
            .Dropped("Create the ticket's branch", Question.Handled, Checkout)
            .Dropped("ordinary crew never do", Question.Verify,
                "Every dispatched agent is its ticket's captain now (32cc5599), so no crew stands apart from it; the brief names who merges (claim CaptainBriefs.Merger)."),
        Fate.Kept(Report.Up, Captain.ReportUp),
        Fate.Kept(Dispatch.Start, Captain.Start, Captain.Thread),
        Fate.Kept(Project.OneTicket, Captain.Owns, CaptainSplit.OneTicket),
        Fate.Kept(Project.Contract, CaptainSplit.Contract),
        Fate.Kept(Linear.CutRule, CaptainSplit.Cut, CaptainSplit.Uses, CaptainSplit.OffRecord, CaptainResearch.Scouts),
        Fate.Kept(Hops.Single, CaptainSplit.Steps),
        Fate.Kept(Hops.Chain, CaptainSplit.Steps)
            .Deferred("typed", "Linear has no Type label for Implement, Review or Fix: S3 leaves setting them up to the user's Enablement. Until then the title names the step."),
        Fate.Kept(Hops.AddCheck, CaptainBriefs.LeafCheck),
        Fate.Kept(Hops.AfkType, CaptainSplit.Mode).Deferred("or a two-way main merge", Master),
        Fate.Deferred(Hops.HitlType, Master + " The Mode rule itself is claim CaptainSplit.Mode."),
        Fate.Kept(Dispatch.Template, CaptainDispatch.Brief, CaptainBriefs.Skills, CaptainBriefs.NoPeek),
        Fate.Kept(Roster.Models, CaptainDispatch.Roster),
        Fate.Kept(Roster.Usage, CaptainDispatch.Usage),
        Fate.Kept(Roster.Seats, CaptainDispatch.Scale),
        Fate.Kept(Brief.Lenses, CaptainBriefs.Lenses),
        Fate.Kept(Brief.SpecInBrief, CaptainBriefs.Governing),
        Fate.Kept(Brief.ScoutPass, CaptainReview.NoLoop),
        Fate.Kept(Dispatch.Channel, CaptainDispatch.Send, CaptainSplit.OffRecord, CaptainSplit.Helpers)
            .Dropped("shares your checkout", Question.Handled, "The host runs a native sub-agent in its caller's session and checkout unless the caller asks otherwise; the captain arranges nothing."),
        Fate.Kept(Dispatch.Worktrees, CaptainDispatch.OwnWorktree)
            .Dropped("and sub-tickets with children", Question.Verify,
                "The sentence he accepted (1ffbc4c8) sets the condition anew: a child gets its own worktree when it \"will open its own PR\", whether or not it has children."),
        Fate.Dropped(Dispatch.Foreground, Question.Handled,
            "T3's own instructions to every agent already say it: a native child is awaited in the turn, and a background agent goes through `delegate_task`."),
        Fate.Dropped(Dispatch.ParentNotActive, Question.Need, "Its reader is a native child, which gets no ticket and never loads this skill."),
        Fate.Dropped(Dispatch.CodexCap, Question.Reason,
            "Nobody knows what the cap counts or where 8 and 16 came from: a one-off turned into a rule (1ffbc4c8). CLIProxy carries the limits, and claim CaptainFinish.Limits covers the error."),
        Fate.Kept(Project.Usage, CaptainFinish.Limits),
        Fate.Kept(Project.LanesFull, CaptainDispatch.Lanes, CaptainDispatch.Order),
        Fate.Kept(Project.PullRule, CaptainDispatch.Pull),
        Fate.Kept(Project.Pulled, CaptainDispatch.Pulled),
        Fate.Kept(Dispatch.Send, CaptainDispatch.Family),
        Fate.Deferred(Dispatch.Council, Chair),
        Fate.Deferred(Dispatch.HighStakes, Chair),
        Fate.Kept(Dispatch.SelfAuthor, CaptainSplit.Small, CaptainReview.Risk)
            .Dropped("and a sub-captain authors nothing", Question.Verify,
                "It contradicts the small-change rule beside it, since a sub-captain is a ticket captain (2119cd65); a leaf's holder writes its own work, so the limit binds a captain once it has split (df6a785e)."),
        Fate.Kept(Dispatch.Return, CaptainDispatch.Resume)
            .Dropped("Every crew return comes back to you", Question.Handled, "T3 returns each child's result to the thread that started it; the captain has only to act on it."),
        Fate.Kept(Report.Route, CaptainDispatch.Route),
        Fate.Kept(Review.Open, CaptainReview.Whole, CaptainReview.Whose),
        Fate.Kept(Review.Skip, CaptainReview.Groups),
        Fate.Kept(Review.Post, CaptainReview.Record, CaptainRound.Verdict, CaptainRound.PrLine),
        Fate.Kept(Review.FixTicket, CaptainReview.Fix, CaptainTrouble.FixName, CaptainTrouble.Big, CaptainTrouble.Parallel)
            .Dropped("with one Blocker ticket per blocker inside it", Question.Verify,
                "No Fix in Linear holds a ticket per blocker (Fix r1 to r3, read 2026-11-21), and S9 as amended has one Fix list them, with a sub-ticket only for a big one (07c86aad, \"4. Lgtm\"; claim CaptainTrouble.Big)."),
        Fate.Kept(Review.Nit, CaptainRound.Nits),
        Fate.Kept(Review.CaptainMade, CaptainReview.Mine),
        Fate.Kept(Review.Labels, CaptainRound.Changed),
        Fate.Kept(Tree.NoSelfReview, CaptainReview.OwnFinding),
        Fate.Kept(Review.Broken, CaptainReview.Fix, CaptainTrouble.NextRound),
        Fate.Kept(Review.Security, CaptainReview.Fix, CaptainTrouble.Security),
        Fate.Kept(Review.Fundamental, CaptainReview.Replan)
            .Dropped("when that is the coordinator, it only opens a Wayfinding ticket, and a fresh captain there re-plans", Question.Need,
                "Its reader is the coordinator: what an officer does with a re-plan belongs in that officer's skill (41a553f1)."),
        Fate.Kept(Review.NeedsBalazs, CaptainReview.NeedsUser, CaptainNeed.AtOnce, CaptainNeed.Skip, CaptainNeed.Refused),
        Fate.Kept(Review.Clean, CaptainReview.Clean),
        Fate.Kept(Review.OneMore, CaptainReview.OneMore),
        Fate.Kept(Review.ToCheckpoint, CaptainTrouble.Third, CaptainTrouble.Checkpoint, CaptainTrouble.Escalate),
        Fate.Kept(Should.Register, CaptainRound.Shoulds),
        Fate.Kept(Should.Reparent, CaptainImplement.Reparent),
        Fate.Kept(Should.ParentDone, CaptainRound.ParentDone),
        Fate.Kept(Should.Fixed, CaptainRound.Cheap, CaptainRound.ParentDone)
            .Dropped("for a shoulds batch", Question.Need,
                "Cutting a Shoulds batch is the coordinator's act (S17); a captain acts only on this, that the fix can wait (41a553f1). Team Dydo's Linear holds no batch yet, read 2026-11-21."),
        Fate.Kept(Should.Dup, CaptainImplement.Duplicate),
        Fate.Kept(Should.Cancel, CaptainImplement.Cancel),
        Fate.Kept(FeatureMerge.GateFirst, CaptainMerging.Gates, CaptainMerging.Merge)
            .Dropped("`dydo test gate`", Question.Verify, "The CLI has no such command (71e649c0); the line names the project's gates, as the brief gives them."),
        Fate.Kept(FeatureMerge.PreRed, CaptainMerging.Broken),
        Fate.Kept(FeatureMerge.OneEntry, CaptainMerging.Merge, CaptainMerging.Refusal)
            .Dropped("Merge only through", Question.Handled, "The guard blocks every other way to merge a PR and names this command, so the line has only to give it."),
        Fate.Kept(FeatureMerge.NoMergeReview, CaptainMerging.Clean),
        Fate.Kept(FeatureMerge.NoMutationWait, CaptainMerging.Mutation)
            .Dropped("it is checked at the landing", Question.Need, "Checking it there is the act of whoever lands the Project, never a captain's (41a553f1)."),
        Fate.Kept(FeatureMerge.Conflict, CaptainMerging.Broken, CaptainMergeTrouble.Resolver),
        Fate.Kept(FeatureMerge.Green, CaptainMerging.Ci).Withheld("DYTEST holds a green receipt on the merge tree and", CaptainMerging.Receipt),
        Fate.Kept(FeatureMerge.Red, CaptainMergeTrouble.Rerun).Withheld("through DYTEST", CaptainMergeTrouble.Dytest),
        Fate.Kept(SafeDelete.Archive, CaptainMerging.Archive)
            .Dropped("then purges them", Question.Need, "What the archive does with them after 30 days asks nothing of the captain."),
        Fate.Kept(Dispatch.CloseTicket, CaptainFinish.Close, CaptainFinish.Nothing)
            .Dropped("once its work is merged", Question.Verify,
                "A ticket is done when its contract is met (7b3d2e3e): a captain whose officer merges closes Done on its commit, before any merge."),
        Fate.Kept(Dispatch.CaptainReturn, CaptainFinish.Merged, CaptainFinish.Nothing, CaptainFinish.Return)
            .Dropped("Then return one line", Question.Verify,
                "One line is the return of a captain that merges, or merges nothing; one whose officer merges returns its commit's SHA with the receipts (claim CaptainFinish.HandOver).")
            .Dropped("to your brief's return-to", Question.Handled, "T3 returns a child's final result to the thread that started it; no brief has to name where.")
            .Dropped("and everything else is on the ticket", Question.Need,
                "It asks no act of its own: the case that merges nothing already puts its result on the ticket (claim CaptainFinish.Nothing), and a merge's record is its PR and its tickets."),
        Fate.Deferred(TicketCaptainResource.JobTickets, Unapproved),
        Fate.Deferred(TicketCaptainResource.InquisitionCaptain, Unapproved),
        Fate.Deferred(TicketCaptainResource.CouncilChair, Unapproved),
        Fate.Deferred(TicketCaptainResource.CouncilMembers, Unapproved),
        Fate.Kept(ResourceName.ImplementerBrief, CaptainResource.Briefs),
        Fate.Kept(ResourceName.ReviewerBrief, CaptainResource.Briefs),
        Fate.Kept(ResourceName.ResearchBrief, CaptainResource.Briefs),
        Fate.Kept(ResourceName.ScoutBrief, CaptainResource.Research),
        Fate.Deferred(ResourceName.TicketCaptainBrief,
            "What every brief names is `resources/briefs.md`; its table of a job ticket's skill belongs with job tickets, not rebuilt in DYD-1372."),
        Fate.Deferred(TicketCaptainResource.MasterRoute, Master),
        Fate.Kept(TicketCaptainScenario.ReCut, CaptainResource.TooBig),
        Fate.Kept(TicketCaptainScenario.FixRounds, CaptainResource.FixRounds),
        Fate.Kept(TicketCaptainScenario.MergeConflict, CaptainResource.MergeConflict),
        Fate.Kept(TicketCaptainScenario.RedGate, CaptainResource.RedGate),
        Fate.Deferred(TicketCaptainScenario.ForcedResync, Master),
        Fate.Deferred(TicketCaptainScenario.MasterRevert, Master),
        Fate.Kept(TicketCaptainScenario.NeedForBalazs, CaptainResource.NeedForTheUser),
        Fate.Deferred(TicketCaptainScenario.Liveness, "Its trigger, the survey's `liveness` finding, is not in `dydo survey`'s code (`Services/Survey`)."),
        Fate.Deferred(TicketCaptainScenario.OverLimit,
            "Its way past the limit ends in a council, whose resource is not rebuilt in DYD-1372; the limit itself is claim CaptainImplement.Limit."),
        Fate.Deferred(TicketCaptainScenario.LandingNotAccepted, "It belongs to the landing, which no line of this prototype states (41a553f1); not rebuilt in DYD-1372.")
    ]);
}

namespace DynaDocs.SkillBuild;

/// <summary>The working tree (K12, K29): its invariants, branches, the crew's limits and nesting, as the working-tree standard and the roles state them.</summary>
public static class Tree
{
    public static readonly Claim Purpose = new("The working-tree standard is built from the same claims as the role skills.", [Actor.SkillBuild],
        [new(Skill.WorkingTreeStandard, "How a Project's work moves through branches, worktrees, hops and merges; every rule here is a claim, built from the same model as the role skills.", new(Section.TreeIntro, 1))])
    { Spec = ["K12", "D10"] };

    public static readonly Claim NoSelfReview = new("No agent judges its own step.", [Actor.TicketCaptain, Actor.Reviewer],
    [
        new(Skill.WorkingTreeStandard, "No self-review. An agent never reviews its own work or rules on findings about it: its own context biases it, and a fresh context catches what it can't. A leaf's evidence check is a fresh seat the officer sends. Each level checks for itself, never taking a lower level's verdict as its proof. A captain authors only a change too small to need its own review; the review above covers it.", new(Section.TreeInvariants, 1)),
        new(Skill.TicketCaptain, "Never rule on findings about work you authored.", new(ReviewState.Verdict, 1902, MoveKind.Nominal)),
        new(Skill.Reviewer, "Never judge work you implemented or fixed.", new(ReviewState.Round, 1902, MoveKind.Nominal))
    ])
    { Spec = ["R33", "K29"] };

    public static readonly Claim OwnedPaths = new("One writer per worktree; commits touch owned paths only.", [Actor.CodeWriter],
    [
        new(Skill.CodeWriter, "One writer per worktree: commit only inside your owned paths.", new(DispatchState.Working, 1903, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "One writer per worktree, and commits touch owned paths only.", new(Section.TreeInvariants, 2))
    ])
    { Spec = ["N12 DR 045 §8"] };

    public static readonly Claim Commits = new("Every writer hop ends on a commit named for its hop, kept unsquashed; merges are merge commits only.", [Actor.CodeWriter],
    [
        new(Skill.CodeWriter, "End the hop on one commit `<KEY> <hop>: <what>`, `<hop>` being `implement`, `fix` or `merge`, never squashed.", new(DispatchState.Working, 1904, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "Every writer hop ends on a commit `<KEY> <hop>: <what>`, where `<hop>` is `implement`, `fix` or `merge`, kept unsquashed. Merges are merge commits only, never squash or rebase.", new(Section.TreeInvariants, 3))
    ])
    { Spec = ["R32"] };

    public static readonly Claim Branch = new("A ticket branch is named by its key off the feature branch; branch, base and worktree are posted before the first edit.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Name the ticket branch `<KEY>-<slug>` off the feature branch, so Linear attaches the branch and PR; post branch, base SHA and worktree path on the ticket before the first edit.", new(DispatchState.Briefed, 1905, MoveKind.Nominal)),
        new(Skill.WorkingTreeStandard, "A ticket branch is `<KEY>-<slug>` off its feature branch, so Linear attaches the branch and PR; its branch, base SHA and worktree path are on the ticket before the first edit.", new(Section.TreeInvariants, 4))
    ])
    { Spec = ["N12 DR 045 §8"] };

    public static readonly Claim LandingCandidate = new("The landing candidate is the feature branch with master merged in.", [Actor.Admiral],
        [new(Skill.WorkingTreeStandard, "The landing candidate is the feature branch with master merged in: the exact tree master gets.", new(Section.TreeBranches, 3))])
    { Spec = ["N6"] };

    public static readonly Claim CrewWork = new("Crew work inside their ticket's contract, owned paths and gates, and commit their own hop.", [Actor.CodeWriter],
        [new(Skill.WorkingTreeStandard, "Crew work inside their ticket's contract, owned paths and gates, and commit their own hop.", new(Section.TreeDelegation, 8))])
    { Spec = ["K14", "R32"] };

    public static readonly Claim CrewLimits = new("Ordinary crew do not create the ticket's branch, open its PR or merge it.", [Actor.TicketCaptain, Actor.CodeWriter],
    [
        new(Skill.WorkingTreeStandard, "Ordinary crew do not create its branch, open its PR or merge it.", new(Section.TreeDelegation, 9)),
        new(Skill.TicketCaptain, "Create the ticket's branch, open its PR and merge it yourself; ordinary crew never do.", new(DispatchState.Briefed, 1908, MoveKind.Nominal)),
        new(Skill.CodeWriter, "Your hop is commits on the brief's branch; the branch, its PR and its merge are the captain's.", new(DispatchState.Working, 1908, MoveKind.Nominal))
    ])
    { Spec = ["K29"] };

    public static readonly Claim Nesting = new("Sub-captains are allowed with no hard cap, cut only for coherent units; bad nesting is a finding.", [Actor.Admiral, Actor.TicketCaptain],
        [new(Skill.WorkingTreeStandard, "Sub-captains are allowed with no hard cap, cut only for coherent units too big for one agent or able to run in parallel; the practical ceiling is admiral → captain → sub-captain → second-order sub-captain → crew, and bad nesting, or a deeper sub-captain that fails, is dealt with as a finding.", new(Section.TreeDelegation, 10))])
    { Spec = ["H13", "H14"] };
}

namespace DynaDocs.SkillBuild;

/// <summary><c>dydo merge</c> and its refusals (M6): the one entry point, each refusal with its stated reason, and merge commits only.</summary>
public static class MergeCommand
{
    public static readonly Claim NoPass = Refusal("no PASS on the head, nor on an ancestor followed only by clean syncs", "R17", "R23", "R24");
    public static readonly Claim ReviewOpen = Refusal("the Review is not Done", "R17");
    public static readonly Claim NoReceipt = Refusal("DYTEST holds no green receipt for the combined tree", "R23", "Y4");
    public static readonly Claim HandShaped = Refusal("a hand-shaped tree, not git merge-tree --write-tree p1 p2", "R19");
    public static readonly Claim TipMoved = Refusal("the target's tip moved from the pin", "N12 DR 061 §2");
    public static readonly Claim DraftOrFork = Refusal("a draft or fork PR, or a closed PR not yet in its base", "N12 DR 061 §2");
    public static readonly Claim OneWay = Refusal("a PR into master that the dydo.json path list flags one-way", "R27");
    public static readonly Claim NotTwoWay = Refusal("a PR into master whose body does not declare it two-way, or whose CI is not green", "R26");

    public static readonly Claim Conflict = new("dydo merge reports CONFLICT when the merge with the tip is not clean.", [Actor.DydoMerge], [])
    { Spec = ["R21", "R23"], Transition = new(MergeCommandState.Invoked, MergeCommandState.Conflict, "the merge with the tip is not clean") };

    public static readonly Claim AlreadyIn = new("A closed PR whose SHA is already in its base exits 0, already integrated.", [Actor.DydoMerge], [])
    { Spec = ["N12 DR 061 §2"], Transition = new(MergeCommandState.Invoked, MergeCommandState.AlreadyIn, "a closed PR already in its base") };

    public static readonly Claim Merge = new("dydo merge merges only when every check holds, as a merge commit.", [Actor.DydoMerge],
        [new(Skill.WorkingTreeStandard, "`dydo merge` merges only when the Review is Done with a PASS on the head, or on an ancestor whose later commits are all clean syncs with their second parent already in the target, the merge with the tip is clean, and DYTEST holds a green receipt keyed by the combined tree; it refuses a hand-shaped merge, and there is no `--verify`.", new(Section.TreeMerging, 2))])
    {
        Spec = ["R23", "R24", "R29", "R32", "N12 DR 061 §3"],
        Transition = new(MergeCommandState.Invoked, MergeCommandState.Merged, "every check holds: a merge commit; a re-land reverts the revert")
    };

    private static Claim Refusal(string reason, params string[] spec) =>
        new($"dydo merge refuses, with a stated reason: {reason}.", [Actor.DydoMerge], [])
        { Spec = spec, Transition = new(MergeCommandState.Invoked, MergeCommandState.Refused, "refused: " + reason) };
}

namespace DynaDocs.SkillBuild;

/// <summary>The one-time DR backfill (M17): drafted by cheap-tier crew, checked by a reviewer, approved row by row on his page.</summary>
public static class Backfill
{
    public static readonly Claim Draft = new("After he approves the sorted principles, cheap-tier crew drafts applies: and quoted weighed: tags.", [Actor.Implementer], [])
    { Spec = ["L12"], Transition = new(BackfillState.Untagged, BackfillState.Drafted, "cheap-tier crew drafts the tags") };

    public static readonly Claim Check = new("A reviewer checks every citation as a doc claim.", [Actor.Reviewer], [])
    { Spec = ["L12"], Transition = new(BackfillState.Drafted, BackfillState.Checked, "a reviewer checks every citation") };

    public static readonly Claim Page = new("He reviews one page per batch, doubted and new-ground rows first.", [Actor.Coordinator], [])
    { Spec = ["L12"], Transition = new(BackfillState.Checked, BackfillState.OnPage, "batched onto one review page") };

    public static readonly Claim Approve = new("Only rows he approves merge.", [Actor.Balazs], [])
    { Spec = ["L12"], Transition = new(BackfillState.OnPage, BackfillState.Merged, "he marks the row approved") };

    public static readonly Claim Retag = new("A row he marks retag is drafted again.", [Actor.Balazs], [])
    { Spec = ["L12"], Transition = new(BackfillState.OnPage, BackfillState.Drafted, "he marks the row retag") };

    public static readonly Claim Reject = new("A row he rejects does not merge.", [Actor.Balazs], [])
    { Spec = ["L12"], Transition = new(BackfillState.OnPage, BackfillState.Rejected, "he rejects the row") };
}

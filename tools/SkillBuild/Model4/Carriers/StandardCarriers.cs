namespace DynaDocs.SkillBuild;

/// <summary>
/// The two standards (K12, K29), built to <c>dydo/</c> and <c>Scaffold/dydo/</c> from the same claims as the
/// role skills. Section titles keep the anchors other docs link to.
/// </summary>
public static class StandardCarriers
{
    public static readonly SkillSpec WorkingTree = SkillSpec.Standard(Skill.WorkingTreeStandard, "guides", "Working-Tree Standard", "area: guides", "type: guide") with
    {
        Sections =
        [
            new(Section.TreeIntro), new(Section.TreeInvariants, "Invariants"), new(Section.TreeBranches, "Branches"),
            new(Section.TreeDelivery, "Delivery scale and evidence"), new(Section.TreeDelegation, "Delegation"),
            new(Section.TreeMerging, "Merging"), new(Section.TreeCleanup, "Cleanup")
        ]
    };

    public static readonly SkillSpec LinearWorkspace = SkillSpec.Standard(Skill.LinearWorkspaceStandard, "reference", "Linear Workspace Standard", "area: reference", "type: reference") with
    {
        Sections =
        [
            new(Section.LinearIntro), new(Section.LinearSetup, "Setting up"), new(Section.LinearStatuses, "Statuses"),
            new(Section.LinearLabels, "Ticket labels"), new(Section.LinearTickets, "Tickets"),
            new(Section.LinearEvidence, "Communication and evidence"), new(Section.LinearShoulds, "Shoulds"),
            new(Section.LinearSpec, "Spec and contract"), new(Section.LinearPriority, "Priority")
        ]
    };
}

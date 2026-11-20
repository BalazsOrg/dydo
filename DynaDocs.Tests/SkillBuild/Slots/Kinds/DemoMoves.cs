namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>The slot fixture's claims: one line in every slot kind a spine, phase, resource and scenario hold.</summary>
public static class DemoMoves
{
    public static readonly Claim OwnsOneTicket = new("The crew owns one ticket.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "You own one ticket from intake to return.", new(SpinePart.Job, 1))]);

    public static readonly Claim ReturnsToItsOfficer = new("The crew returns to its officer.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "You return to the officer who sent you.", new(SpinePart.Job, 2))]);

    public static readonly Claim EndsResolvedOrEscalated = new("A scenario ends resolved or escalated.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Each scenario ends resolved or escalated.", new(SpinePart.WhenThingsGoWrong, 1))]);

    public static readonly Claim AccountableForProof = new("The crew proves its change.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "You answer for the proof of your change.", new(DemoSlot.Accountable, 1))]);

    public static readonly Claim TouchesItsWorktree = new("The crew works in its worktree.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Your worktree and its branch.", new(DemoSlot.Tools, 1))]);

    public static readonly Claim ReadTheBrief = new("Read the brief on arrival.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Read the brief first.", new(DemoSlot.Intake, 1, MoveKind.Arrival))]);

    public static readonly Claim CheckTheBase = new("The base is an ancestor of HEAD.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Check the base is an ancestor of HEAD.", new(DemoSlot.Intake, 2, MoveKind.Nominal))]);

    public static readonly Claim CheckTheWorktree = new("The worktree is clean.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Check the worktree is clean.", new(DemoSlot.Intake, 1, MoveKind.Nominal))]);

    public static readonly Claim StopOnAStaleLease = new("A stale lease stops the work.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Stop when the lease goes stale.", new(DemoSlot.Intake, 1, MoveKind.Exit))]);

    public static readonly Claim StartAfterIntake = new("Work starts after intake.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Start work once intake is done.", new(DemoSlot.Work, 1, MoveKind.Arrival))]);

    public static readonly Claim DispatchWithATicket = new("Crew is dispatched with a ticket.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Dispatch crew with a ticket.", new(DemoSlot.Dispatch, 1))]);

    public static readonly Claim ResumeAfterSaving = new("Resume once the work is saved.", [DemoActor.Crew], []);

    public static readonly Claim SaveTheWork = new("Save the work before anything else.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Save the work, then resume.", new(DemoSlot.StaleLeaseSteps, 1), () => [DemoMoves.ResumeAfterSaving])]);

    public static readonly Claim EscalateWhenStuck = new("Escalate what you cannot resolve.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Escalate to the officer when the lease cannot be renewed.", new(DemoSlot.StaleLeaseSteps, 2))]);

    public static readonly Claim GuideOnDemand = new("The guide is read on demand.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "Read this guide when a command is unclear.", new(DemoSlot.GuideLead, 1))]);

    public static readonly Claim CheckBeforeReturn = new("Run the check before returning.", [DemoActor.Crew],
        [new(DemoSkill.Demo, "```bash\ndydo check\n```", new(DemoSlot.Commands, 1))]);
}

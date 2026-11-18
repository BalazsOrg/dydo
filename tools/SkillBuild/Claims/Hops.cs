namespace DynaDocs.SkillBuild;

/// <summary>A step's hops, by Type label (M4): one ticket, or a chain of typed hop sub-tickets, and its Merge hop's kind.</summary>
public static class Hops
{
    public static readonly Claim Single = new("A leaf lane or a batched should is one ticket, with no hops.", [Actor.TicketCaptain, Actor.Admiral],
    [
        new(Skill.TicketCaptain, "A leaf lane or a batched should stays one ticket, with no hops.", new(HopsState.Cut, 401, MoveKind.Exit)),
        new(Skill.Admiral, "A leaf lane or a batched should stays one ticket, with no hops.", new(HopsState.Cut, 401, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "A leaf lane or a batched should is one ticket, with no hops.", new(Section.LinearTickets, 4))
    ])
    { Spec = ["S4"], Transition = new(HopsState.Cut, HopsState.OneTicket, "the step has no review loop of its own") };

    public static readonly Claim Chain = new("A step with its own review loop gets typed hop sub-tickets, blocked in step order.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "A step with its own review loop gets hop sub-tickets typed Implement, Review (one per round), Fix and Merge, blocked in step order; you may open them ahead in Todo.", new(HopsState.Cut, 402, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "Hops appear only under a step with its own review loop: Implement, Review (one per round), Fix and Merge, blocked in step order; each goes Todo → In Progress → Done and may be opened ahead in Todo.", new(Section.LinearTickets, 5))
    ])
    { Spec = ["S3", "S4"], Transition = new(HopsState.Cut, HopsState.Chain, "the step has its own review loop") };

    public static readonly Claim AddCheck = new("A leaf's evidence check is an optional fresh seat, a Review hop beside it.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "For a leaf's optional evidence check, add a fresh seat as a Review hop beside it, never a full review.", new(HopsState.OneTicket, 403, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "A leaf's evidence check is a Review hop beside it.", new(Section.LinearTickets, 6)),
        new(Skill.WorkingTreeStandard, "A leaf or lane gets no full review, only an optional fresh evidence-check seat: a Review hop beside it.", new(Section.TreeDelivery, 2))
    ])
    { Spec = ["S4", "R6"], Transition = new(HopsState.OneTicket, HopsState.EvidenceCheck, "the officer adds an evidence-check seat") };

    public static readonly Claim CheckDone = new("The evidence check's Review hop closes Done.", [Actor.Reviewer], [])
    { Spec = ["S4", "S6"], Transition = new(HopsState.EvidenceCheck, HopsState.StepDone, "the evidence check's Review hop is Done") };

    public static readonly Claim CheckFail = new("A blocker in a leaf's evidence check becomes Fix rN at verdict time.", [Actor.TicketCaptain], [])
    { Spec = ["S9", "R11"], Transition = new(HopsState.EvidenceCheck, HopsState.CheckFix, "the evidence check finds a blocker") };

    public static readonly Claim CheckAgain = new("After the fix, the next evidence check runs.", [Actor.TicketCaptain], [])
    { Spec = ["R8", "S9"], Transition = new(HopsState.CheckFix, HopsState.EvidenceCheck, "Fix rN Done; the next check runs") };

    public static readonly Claim LeafDone = new("A leaf's holder closes it Done.", [Actor.TicketCaptain], [])
    { Spec = ["S6"], Transition = new(HopsState.OneTicket, HopsState.StepDone, "its holder closes it Done") };

    public static readonly Claim AfkType = new("An AFK Merge is a feature-branch merge or a two-way main merge.", [Actor.TicketCaptain],
    [
        new(Skill.TicketCaptain, "Type the Merge hop AFK Merge for a feature-branch merge or a two-way main merge.", new(HopsState.Chain, 406, MoveKind.Exit)),
        new(Skill.LinearWorkspaceStandard, "Merge sub-tickets: an AFK Merge for a feature-branch merge or a two-way main merge, a HITL Merge only for a one-way one.", new(Section.LinearTickets, 8), () => [HitlType!])
    ])
    { Spec = ["S12"], Transition = new(HopsState.Chain, HopsState.AfkMerge, "the step merges into the feature branch, or two-way into main") };

    public static readonly Claim HitlType = new("A HITL Merge is only for a one-way main merge.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Type the Merge hop HITL Merge only for a one-way main merge.", new(HopsState.Chain, 407, MoveKind.Exit))])
    { Spec = ["S12", "R27"], Transition = new(HopsState.Chain, HopsState.HitlMerge, "a one-way main merge") };

    public static readonly Claim TypeLabels = new("Each hop's kind is its Type label.", [Actor.Linear],
        [new(Skill.LinearWorkspaceStandard, "Each hop's kind is its Type label: Implement, Review, Fix, Blocker and Wayfinding sit in the Type group next to `Merge`; setting them up is his Enablement.", new(Section.LinearLabels, 1))])
    { Spec = ["S3"] };
}

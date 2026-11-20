namespace DynaDocs.SkillBuild;

/// <summary>
/// PLACEHOLDER (DYD-1372 hop 1): the three claims that keep a truth-checked ticket-captain building until
/// hop 2 writes the prototype's own. They say only that the skill is a placeholder; none is the
/// prototype's prose, and hop 2 replaces every one.
/// </summary>
public static class Captain
{
    private const string Carrier = "tools/SkillBuild/Model4/Carriers/CaptainCarrier.cs";

    public static readonly Claim Placeholder = new("The built ticket-captain skill is a placeholder until DYD-1372 hop 2.", [Actor.TicketCaptain],
        [new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, "This skill is a placeholder: the ticket captain's process is not written here until DYD-1372 hop 2 lands.", new(SpinePart.Job, 1))])
    {
        Spec = ["K9"],
        Truth = new(Evidence.Code(Carrier),
            Layer: "Nothing below the skill tells an agent who loads it that the process is missing.",
            Reason: "An agent who took an empty skill for the whole process would improvise one.")
    };

    public static readonly Claim Brief = new("Until the prototype lands, a ticket captain works from its dispatch brief alone.", [Actor.TicketCaptain],
        [new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, "Work from the brief you were dispatched with; this skill adds no step to it yet.", new(CaptainSlot.Hold, 1))])
    {
        Spec = ["K14"],
        Truth = new(Evidence.Code(Carrier),
            Layer: "The dispatch delivers the brief, but only this skill can say that it adds nothing.",
            Reason: "Keeps the agent on the one source it does have, instead of hunting for more.")
    };

    public static readonly Claim Report = new("What the placeholder leaves open goes to the dispatcher.", [Actor.TicketCaptain],
        [new(CaptainSkill.TicketCaptain, Actor.TicketCaptain, "Take anything this placeholder leaves open to whoever dispatched you.", new(SpinePart.WhenThingsGoWrong, 1))])
    {
        Spec = ["K9"],
        Truth = new(Evidence.Code(Carrier),
            Layer: "No tool routes a process question; the agent has to know its address.",
            Reason: "A question with a known address gets asked, not guessed.")
    };
}

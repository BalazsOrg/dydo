namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype (DYD-1372), the first truth-checked skill: built to
/// <c>skills/roles/officers/ticket-captain/</c> in place of the legacy <see cref="TicketCaptainCarrier"/>,
/// whose lines stay in the model, unbuilt, and are accounted for in <see cref="CaptainLedger"/>.
/// PLACEHOLDER: hop 1 declares the smallest spine that builds; hop 2 replaces its description, phases,
/// resources and scenarios with the prototype's.
/// </summary>
public static class CaptainCarrier
{
    public static readonly SkillSpec Spec = SkillSpec.Officer(CaptainSkill.TicketCaptain, "Ticket Captain",
        "Placeholder until DYD-1372 hop 2 lands the ticket-captain prototype. It states no process yet.") with
    {
        Readers = [Actor.TicketCaptain],
        Ledger = CaptainLedger.Placeholder,
        Phases = [new("Hold the ticket", Slot.Paragraph(CaptainSlot.Hold))]
    };
}

namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>
/// The ahead fixture (DYD-1402): the legacy clerk with one resource, its rota, truth-checked ahead of the skill,
/// beside the legacy keeper. Neither skill names a reader, so only the rota's lines are held to the truth checks.
/// </summary>
internal static class AheadModel
{
    public static readonly Resource Rota = new(AheadSlot.Rota, "Rota", Slot.Paragraph(AheadSlot.RotaLead), Slot.Bullets(AheadSlot.RotaRules, "Each morning"))
    {
        Readers = [KeeperActor.Keeper]
    };

    public static Model With(params Type[] kinds) => Of(Rota, kinds);

    public static Model Of(Resource rota, params Type[] kinds) => new([typeof(OldMoves), typeof(ClerkRota), .. kinds],
        [TruthModel.OldKeeper, TruthModel.Clerk with { Phases = [new("Desk", new Slot(OldSlot.Desk)) { Resources = [rota] }] }]);
}

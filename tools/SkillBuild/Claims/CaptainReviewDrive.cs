namespace DynaDocs.SkillBuild;

/// <summary>
/// The ticket-captain prototype's Review kind, driving the feature like a user (DYD-1372 07c86aad;
/// aa5b445e, "a reviewer agent whose main job is to drive the app and figure out if the feature is
/// working as intended or not"). The driver is a project's own verification skill, so the lines say
/// how a seat finds and follows it, and never one project's commands.
/// </summary>
public static class CaptainReviewDrive
{
    private const string Verify = "skills/engineering/verify-dydo/SKILL.md";
    private const string Create = "skills/engineering/create-verification-skill/SKILL.md";
    private const string Proof = "skills/engineering/maintain-verification-skill/SKILL.md";

    private static readonly Enum[] Me = [Actor.TicketCaptain];

    public static readonly Claim Driver = new("A driver drives through the project's verification skill, in the order of that skill's own sections.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewDrive, 1,
            "Drive through your project's verification skill, the one your brief names, whose name starts with `verify-`: follow its Launch, Doctor, Drive, Evidence and Cleanup sections, in that order.")
    ])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Run("`" + Verify + "`, this repository's, holds those five sections under those names. `" + Create + "` writes every project's as `verify-<app>` with the same five, " +
                                 "and leaves it model-invoked: \"implementers and reviewers load it mid-task\"."),
            Layer: "How an app is started, driven and torn down is known only to the skill written for that app.",
            Reason: "A driver that improvises its launch tests an instance nobody would recognise, and strands processes when it leaves.")
    };

    public static readonly Claim User = new("A driver uses the feature the way its user would, by every route the feature map lists, and says for each whether it works as the spec's problem statement intends.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewDrive, 2,
            "Use the feature the way its user would, by every route that skill's feature map lists for it, and say for each whether it works as the spec's problem statement intends.")
    ])
    {
        Spec = ["R34", "R3"],
        Truth = new(Evidence.Run("`" + Create + "`: \"The map is the repo's maintained verification source; a proof that drives one convenient entry point is incomplete when the map lists others.\" " +
                                 "`" + Verify + "` opens on its map, `resources/features/README.md`."),
            Layer: "A suite exercises the seams its author chose, and nothing in it walks the product the way the person it was built for will.",
            Reason: "A feature can pass every test and still fail the first person who tries to use it.")
    };

    public static readonly Claim Steps = new("A driver returns each journey as steps anyone can rerun, with what it observed beside what the spec expects; only what it observed itself is proof.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewDrive, 3,
            "Return each journey as steps anyone can rerun, with what you observed beside what the spec expects; only what you observed yourself, in this session, is proof.")
    ])
    {
        Spec = ["R5", "R34"],
        Truth = new(Evidence.Run("`" + Proof + "`, Proof: \"evidence you observed yourself in this session: command output, a reproduction, a screenshot. " +
                                 "Green mocked tests do not prove that the user experience works.\""),
            Layer: "A driving skill keeps raw evidence, and it is the driver who has to say which step went wrong and what was expected there.",
            Reason: "The Fix's writer reproduces the fault from the steps alone, without asking the driver what it did.")
    };

    public static readonly Claim Gap = new("What a user can do and the verification skill cannot drive is a finding to return, never a driver to build, and a driver changes no product code.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewDrive, 4,
            "What a user can do and the skill cannot drive is a finding to return, never a driver to build; change no product code while you drive.")
    ])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Run("`" + Verify + "`: \"report product regressions without changing production code during the proof pass.\" " +
                                 "`" + Proof + "` names such a hole a verification gap, of severity should, with its own upkeep pass."),
            Layer: "A verification skill covers the journeys someone mapped, and says nothing about the ones nobody did.",
            Reason: "A seat that extends the harness in the middle of a review returns late, and with a driver nobody reviewed.")
    };

    public static readonly Claim Due = new("Driving is due only where the verification skill is built and covers the feature, and the feature is settled.", Me,
    [
        At(Actor.TicketCaptain, CaptainSlot.ReviewDrive, 5,
            "Driving is due only where that skill is built and covers the feature, and the feature is settled; where it is not, return that as your finding.")
    ])
    {
        Spec = ["R34"],
        Truth = new(Evidence.Run("`" + Verify + "`, read 2026-11-21, covers this repository's CLI on Windows only: \"Other platforms fail before child launch\", and \"the viewer, live officer workflows " +
                                 "and pending release prerequisites need their own proof\". So a seat on this Linux host finds driving not due, and returns that."),
            Layer: "The table's \"due\" in the Drive column cannot know whether a project's driver exists, or reaches this feature on this host.",
            Reason: "Driving by hand what no skill covers gives a result nobody can repeat, on a feature that may still change.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text, Func<Claim[]>? also = null) =>
        new(CaptainSkill.TicketCaptain, reader, text, new(slot, order), also);
}

namespace DynaDocs.SkillBuild;

/// <summary>
/// What a question the chief of staff puts to him at muster must hold (DYD-1402), whatever its source: one block in the
/// grilling skill's format, self-contained, with every option, a reasoned recommendation, verified premises, plain
/// words, and a route checked before his money or his permission is asked for. His words on the muster of 2026-11-21
/// are the ruling: "the worst offence you made is that you shown me a question I've never seen and only given me one
/// recommended answer". Which calls reach him at muster, and who carries them, is the spec's (H12, H16, L21) and is
/// not changed here.
/// </summary>
public static class StaffQuestion
{
    private const string Night = "T3 thread d4c81ce0-f13a-4cea-80bd-6911b05d288f, the muster of 2026-11-21, read 2026-11-21: ";

    private const string Grilling = "skills/productivity/grilling/SKILL.md";

    // The markers are the grilling skill's, and a test holds the two equal (MusterQuestionTests).
    private const string Block = """
        ```text
        ❓ **Q1** - **<question title>**: <what he has not seen, each premise with its evidence link, then every option with its trade-off>

        ➡️ <the option you recommend, and your reason>

        ---

        ❓ **Q2** - **<question title>**: <the same, for the next question>

        ➡️ <the option you recommend, and your reason>
        ```
        """;

    private static readonly Enum[] Asking = [Actor.ChiefOfStaff, Actor.Balazs];

    public static readonly Claim Shape = new("Every question the chief of staff puts to him at muster, whatever its source, is one block in the grilling skill's format.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2101, "Put every question you ask him at muster, whatever its source, as one block in the `grilling` skill's format, shown in the question block below.")])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "the chief of staff put three questions to him as plain bullet lists under \"Your calls\" (position 44). His reply: " +
                                 "\"You should have structured the questions better (grilling emojis)\" (position 50). " +
                                 "`skills/productivity/overnight-mode/SKILL.md` on this branch, hand-written, has the muster \"Use `grilling` for decisions\"."),
            Layer: "The host shows him whatever text an agent writes; no tool gives a decision its layout.",
            Reason: "A layout he already reads in each grilling session puts the choices and the recommendation where his eye expects them.")
    };

    public static readonly Claim Context = new("A question's block gives everything he needs to answer and has not seen, so he answers from the block alone.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2102,
            "Give in the block everything he needs to answer and has not seen, even what a coordinator's report already says: he answers from the block alone, without opening a thread.")
    ])
    {
        // His ruling of 2026-11-21 is later than H7's "never relays content" and wins for a question's block; the readiness list stays links (StaffMuster.List).
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "the third question shown to him was \"Q3. dydo: how a need reaches you\", with one recommendation and none of what the dydo coordinator's report " +
                                 "said about it (positions 34 and 44). His reply, kept verbatim on Linear DYD-1402: \"the worst offence you made is that you shown me a question I've never seen " +
                                 "and only given me one recommended answer. That is antithetical to everything\" (position 50)."),
            Layer: "The four reports sit above the questions in the same thread, each hundreds of words long; nothing carries the sentences that matter down.",
            Reason: "A decision he must research first costs him the time the muster exists to save, or gets a guess.")
    };

    public static readonly Claim Options = new("A question's block gives every option with its trade-off, and never one option.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2103, "Give every option, each with its trade-off: at least two, and all that the question's source named.")])
    {
        Spec = ["K11", "L18"],
        Truth = new(Evidence.Run(Night + "the dydo coordinator's report listed three options for its question, (a), (b) and (c), and recommended (a) (position 34); " +
                                 "the chief of staff showed him only \"I recommend (a)\" (position 44). His reply: \"only given me one recommended answer\" (position 50)."),
            Layer: "A recommendation reads as complete by itself; nothing shows him the alternatives that were dropped on the way.",
            Reason: "Choosing is his part; with a single answer in front of him he can only approve, which is no decision at all.")
    };

    public static readonly Claim Recommend = new("A question's block recommends one option, with the reason.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2104, "Recommend one option after `➡️`, with your reason.")])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "\"Q3 ... I recommend (a)\" reached him with no reason he could weigh (position 44), and on Q2 he had to guess one: " +
                                 "\"if it's a stupid thing as you implied then I defer to you\" (position 50). `" + Grilling + "` gives the marker a single placeholder, \"<your recommended answer>\"."),
            Layer: "The grilling format has a place for the recommended answer and none for why.",
            Reason: "He can overrule advice only when he sees what it rests on; bare advice asks for trust.")
    };

    public static readonly Claim Premises = new("Each premise a question rests on is verified before the question is shown, with the evidence linked in the block.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2105,
            "Verify each premise the question rests on before you show it, and link the evidence in the block; where you could not verify one, say so in the block.")
    ])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q1 reached him at 02:05 resting on a coordinator's word that the hosted Actions budget was \"exhausted\" (positions 42 and 44). " +
                                 "He doubted it, \"Did we use up github actions usage? I don't think so\" (position 50), and only then, at 02:17, did the chief of staff read the org's billing: " +
                                 "one `gh api` call (position 52)."),
            Layer: "A report states what its writer checked and what it assumed in the same voice, and nothing marks which is which.",
            Reason: "He decides on what he is told; one wrong fact makes his answer wrong, however well he reasons.")
    };

    public static readonly Claim Plain = new("A question's block is in plain language: a term he has not used is defined where it first appears, or replaced.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2106, "Write the block in plain language: define a term he has not used where it first appears, or replace it.")])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q2 asked him about \"Mutation's request to raise the output limit to 4 MB for the operator only\", and added that " +
                                 "\"the admiral hasn't shown the run would fit\" (position 44). His reply: \"I don't understand the question. Mutation for what?\" (position 50)."),
            Layer: "A coordinator writes in the words of its own Project, and nothing translates them on the way to him.",
            Reason: "Each word he must ask about adds a round of asking and answering before he can decide.")
    };

    public static readonly Claim Route = new("Before a question that asks him to spend money or grant a permission, a route that needs neither is checked, and the block says what was checked and what it showed.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2107,
            "Before a question that asks him to spend money or grant a permission, check for a route that needs neither, and say in the block what you checked and what it showed.")
    ])
    {
        Spec = ["K11", "H17"],
        Truth = new(Evidence.Run(Night + "Q1 asked him to raise the GitHub Actions budget to about $20 a month so that Windows jobs could run (position 44). His reply: " +
                                 "\"we still have blacksmith wired and at least 2 dollars of usage before our self-imposed cap so if we want to run windows we should run it there\" (position 50). " +
                                 "GitHub, read 2026-11-21 with `gh variable list -R BalazsOrg/dytest`: `DYTEST_CI_PROVIDER` is `blacksmith`, last set 2026-11-10, and " +
                                 "`DYTEST_WINDOWS_PROOF_RUNNER_LABEL`, the runner that `.github/workflows/verified-marks.yml` runs on, is `windows-latest`, last set 2026-11-15."),
            Layer: "A blocked agent reports the obstacle it met; the way round lies in a file or a setting it had no reason to open.",
            Reason: "His wallet and his consent are the costliest things to request, and a free way round makes the request unnecessary.")
    };

    public static readonly Claim Form = new("A question's block: the grilling skill's markers, with what the muster puts between them.", [Actor.ChiefOfStaff],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterBlock, 2108, Block)])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Code(Grilling),
            Layer: "The `grilling` skill holds the format for a grilling session; a chief of staff holding a muster has not loaded it.",
            Reason: "Seeing each emoji and rule in place leaves no room to approximate them, as a description in prose would.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) => new(Skill.ChiefOfStaff, reader, text, new(slot, order));
}

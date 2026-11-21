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
            "Verify each premise the question rests on before you show it, and link the evidence in the block: a premise is anything the block gives as fact, what an option can do and its trade-off included.")
    ])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q1 reached him at 02:05 resting on a coordinator's word that the hosted Actions budget was \"exhausted\" (positions 42 and 44). " +
                                 "He doubted it, \"Did we use up github actions usage? I don't think so\" (position 50), and only then, at 02:17, did the chief of staff read the org's billing: " +
                                 "one `gh api` call (position 52)."),
            Layer: "A report states what its writer checked and what it assumed in the same voice, and nothing marks which is which.",
            Reason: "He decides on what he is told; one wrong fact makes his answer wrong, however well he reasons.")
    };

    // Review r1 B1: the line this replaces let a question through on an unverified premise when the block said so. A question now waits, or asks about the unknown itself (Unknown).
    public static readonly Claim Held = new("A question whose answer depends on an unverified fact is held back until the fact is verified, and he is told which question waits and on what.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2106,
            "Hold a question back while a fact its answer depends on is unverified: verify it, or send the question back to its source to verify, and tell him in one line which question waits and on what.")
    ])
    {
        Spec = ["K11", "H16"],
        Truth = new(Evidence.Run(Night + "Q1 asked him for money at 02:05 on a coordinator's word that the hosted Actions budget was \"exhausted\" (positions 42 and 44); the chief of staff read the org's billing " +
                                 "only at 02:17, after his doubt (position 52). Asking a coordinator took under 90 seconds that night: the four requests went out by 02:03:58 and the four reports were in by 02:05:10 " +
                                 "(positions 24 to 27, then 31, 34, 41 and 42). Linear DYD-1402, review comment 656d17fc, read 2026-11-21, on the line this one replaces, " +
                                 "\"where you could not verify one, say so in the block\": the reviewer found that a Q2 re-rendered under it leaves \"the operator\" undefined, says so, and still offers \"allow it now\"."),
            Layer: "T3 shows him whatever block an agent writes; nothing between a draft and his screen checks that anybody read its facts.",
            Reason: "An approval given on a guess has to be taken back later, and by then he is asleep and the work has run on it.")
    };

    public static readonly Claim Unknown = new("A fact the answer depends on that neither the chief of staff nor the question's source can verify reaches him as a question about how to act without it.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2107,
            "Where neither you nor the question's source can verify a fact its answer depends on, ask him how to act without it: say in the block what is unknown, and give as options only ways to act without knowing it.")
    ])
    {
        Spec = ["K11", "H17"],
        Truth = new(Evidence.Run(Night + "the DYTEST Network coordinator leaves every change to dytest's CI to him, \"I do no merges, re-pins, CI or billing changes\" (position 41), and he answered the Windows question " +
                                 "with a way to act that needed nothing learned first: \"I would hold the windows checks until the very end\" (position 50). BalazsOrg/dytest at 607dc306, merged 2026-11-20, " +
                                 "`.github/workflows/verified-marks.yml`: \"DYTEST_WINDOWS_PROOF_RUNNER_LABEL must select a GitHub-hosted Windows runner\", so whether another machine can run those checks is learned only by changing that CI."),
            Layer: "Some facts sit behind what only he may do, a payment, a login, a change to CI; no tool tells an agent it has reached such a wall, it only finds nothing.",
            Reason: "Work stalls all night behind a gap only he can close, unless he says beforehand how to proceed in the dark.")
    };

    // Review r1 B4: "each with its trade-off" (Options) got a trade-off written where no record held one. A trade-off is a premise; a missing one is said to be missing.
    public static readonly Claim Unweighed = new("An option's trade-off nobody has evidence of is said to be missing, and the recommendation rests on verified premises alone.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2108,
            "Where neither you nor the question's source has evidence of an option's trade-off, say so beside the option, and rest your recommendation on verified premises alone.")
    ])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "the dydo coordinator's report named its options (b) and (c) in a few words each, with no trade-off for either (position 34). " +
                                 "Linear DYD-1402, review comment 656d17fc, read 2026-11-21: an agent told to give each option its trade-off wrote one for both, and the reviewer found that " +
                                 "\"the re-rendered Q3 invents consequences for options (b) and (c)\", and that \"they tilt the comparison toward (a)\"."),
            Layer: "A format with a place for each option's trade-off gets that place filled; nothing checks where the words in it came from.",
            Reason: "A made-up cost steers him as surely as a real one, and he cannot tell the two apart; a stated gap he can fill or ignore.")
    };

    public static readonly Claim Plain = new("A question's block is in plain language: a term he has not used is defined where it first appears, or replaced.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2109, "Write the block in plain language: define a term he has not used where it first appears, or replace it with plain words that mean the same.")])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q2 asked him about \"Mutation's request to raise the output limit to 4 MB for the operator only\", and added that " +
                                 "\"the admiral hasn't shown the run would fit\" (position 44). His reply: \"I don't understand the question. Mutation for what?\" (position 50)."),
            Layer: "A coordinator writes in the words of its own Project, and nothing translates them on the way to him.",
            Reason: "Each word he must ask about adds a round of asking and answering before he can decide.")
    };

    public static readonly Claim Route = new("Before a question that asks him to spend money or grant a permission, a route that needs neither is checked, and the block says what was checked and what it showed.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2110,
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
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterBlock, 2111, Block)])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Code(Grilling),
            Layer: "The `grilling` skill holds the format for a grilling session; a chief of staff holding a muster has not loaded it.",
            Reason: "Seeing each emoji and rule in place leaves no room to approximate them, as a description in prose would.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) => new(Skill.ChiefOfStaff, reader, text, new(slot, order));
}

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
        Truth = new(Evidence.Run(Night + "the chief of staff put three questions to him as bullet lists under \"Your calls\" (position 44). His reply: " +
                                 "\"You should have structured the questions better (grilling emojis)\" (position 50). " +
                                 "`skills/productivity/overnight-mode/SKILL.md` on this branch, hand-written, has the muster \"Use `grilling` for decisions\"."),
            Layer: "T3 showed him the three questions in the layout the chief of staff wrote, bullet lists (position 44); this resource as built at 13da419b named no layout for a question.",
            Reason: "A layout he knows by name shows him where each choice and the advice stand.")
    };

    public static readonly Claim Context = new("A question's block gives everything he needs to answer and has not seen, so he answers from the block alone.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2102,
            "Give in the block everything he needs to answer and has not seen, even what a coordinator's report already says: he answers from the block alone, without opening a thread.")
    ])
    {
        // His ruling of 2026-11-21 is later than H7's "never relays content" and wins for a question's block; the readiness list stays links (StaffMuster.List).
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "the third question shown to him was \"Q3. dydo: how a need reaches you\", with one recommendation and neither the context nor the other two options that the dydo coordinator's report " +
                                 "gave (positions 34 and 44). His reply, kept verbatim on Linear DYD-1402: \"the worst offence you made is that you shown me a question I've never seen " +
                                 "and only given me one recommended answer. That is antithetical to everything\" (position 50)."),
            Layer: "The four reports stood higher in the same thread (positions 31, 34, 41 and 42), and the dydo coordinator's three options stood in one of them; the question he was shown held one of the three (position 44).",
            Reason: "Lacking it, he must look the matter up himself or reply unsure.")
    };

    public static readonly Claim Options = new("A question's block gives every option with its trade-off, and never one option.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2103, "Give every option, each with its trade-off: at least two, and all that the question's source named.")])
    {
        Spec = ["K11", "L18"],
        Truth = new(Evidence.Run(Night + "the dydo coordinator's report listed three options for its question, (a), (b) and (c), and recommended (a) (position 34); " +
                                 "the chief of staff showed him only \"I recommend (a)\" (position 44). His reply: \"only given me one recommended answer\" (position 50)."),
            Layer: "T3 showed him the one recommendation the chief of staff wrote (position 44); the two options it left out stayed in the dydo coordinator's report, higher in the thread (position 34).",
            Reason: "Choosing is his part, and a lone answer leaves him to approve or refuse it with the alternatives unseen.")
    };

    public static readonly Claim Recommend = new("A question's block recommends one of the options it gives, with the reason.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2104, "Recommend one of those options after `➡️`, with your reason.")])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q3 reached him as \"I recommend (a), today's question-ticket rule\", with no ground for choosing (a) over the options he was not shown (position 44). " +
                                 "`" + Grilling + "` gives the marker a single placeholder, \"<your recommended answer>\"."),
            Layer: "The `grilling` skill's format shows `➡️` followed by one placeholder, \"<your recommended answer>\".",
            Reason: "Stated grounds give him something to check and to overrule; bare advice leaves him to trust it or to guess what it implies.")
    };

    public static readonly Claim Premises = new("Each premise a question rests on is verified before the question is shown, with the evidence linked in the block.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2105,
            "Verify each premise the question rests on before you show it, and link the evidence in the block: a premise is anything the block gives as fact, what an option can do and its trade-off included.")
    ])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q1 reached him at 02:05 with the budget facts as the DYTEST Network coordinator's report gave them (positions 41 and 44). " +
                                 "He doubted them, \"Did we use up github actions usage? I don't think so\" (position 50), and the chief of staff then read the org's billing, at 02:17: " +
                                 "\"I'll take a quick look at the GitHub billing API to see whether the Actions budget really is used up\" (positions 51 and 52)."),
            Layer: "The Network coordinator's report put a refusal and a budget reading in one sentence, \"Hosted Windows jobs have been refused since 2026-11-20 21:06Z: org Actions budget $0 with prevent_further_usage\" " +
                   "(position 41); the chief of staff's question repeated the sentence as fact (position 44).",
            Reason: "A wrong fact put in front of him can turn sound reasoning into a wrong answer.")
    };

    // Review r1 B1: the line this replaces let a question through on an unverified premise when the block said so. Review r2 B1: an option's trade-off is such a fact, and a missing one holds the question too.
    public static readonly Claim Held = new(
        "A question whose answer depends on an unverified fact, an option's trade-off included, is held back until the fact is verified, and he is told which question waits and on what.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2106,
            "Hold a question back while a fact its answer depends on is unverified, each option's trade-off included: verify it, or send the question back to its source to verify, " +
            "and tell him in one line which question waits and on what.")
    ])
    {
        Spec = ["K11", "H16"],
        Truth = new(Evidence.Run(Night + "Q1 asked him for money at 02:05 on the budget facts of a coordinator's report (positions 41 and 44), and the chief of staff read the org's billing at 02:17, after his doubt " +
                                 "(positions 50 to 52). Q2 said nothing on what approving could harm (position 44), and his answer hung on that: " +
                                 "\"If no harm can come from it then sure, why not\" (position 50). That night the four muster requests went out by 02:03:58 " +
                                 "and the four reports were in by 02:05:10 (positions 24 to 27, then 31, 34, 41 and 42)."),
            Layer: "T3 showed him Q1 as the chief of staff wrote it, twelve minutes before the chief of staff read the billing it rested on (positions 44 and 52).",
            Reason: "An approval given on a guess may prove wrong after he is asleep, when work has already run on it.")
    };

    // Review r2 B1: the options of the question asked in the held one's place each carry a trade-off verified without the unknown, so none of them is "do it anyway, cost unknown".
    public static readonly Claim Unknown = new(
        "A fact the answer depends on that no record and no check open to the chief of staff or the question's source verifies keeps its question held; " +
        "where at least two ways to act have trade-offs verified without the fact, he is asked how to act without it, with only those ways as options.", Asking,
    [
        At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2107,
            "Where no record and no check open to you or to the question's source verifies a fact its answer depends on, the question stays held; " +
            "where at least two ways to act have trade-offs verified without that fact, ask him a question of your own, how to act without it: " +
            "say in the block what is unknown, and give only those ways as options.")
    ])
    {
        Spec = ["K11", "H17"],
        Truth = new(Evidence.Run(Night + "he named a route for the Windows checks, Blacksmith, \"if we want to run windows we should run it there\", and a way to act that waited on nothing more, " +
                                 "\"I would hold the windows checks until the very end\" (position 50). BalazsOrg/dytest at 607dc306, merged 2026-11-20: the machine those checks run on is a repository variable, and " +
                                 "\"The repository owner sets the repository variable\" (`dydo/reference/engine/gate.md`). The DYTEST Network coordinator does \"no merges, re-pins, CI or billing changes\" (position 41). " +
                                 "So a trial of those checks on Blacksmith was open to neither the coordinator nor the chief of staff."),
            Layer: "The coordinator's report listed what it may not exercise, \"Unsupported gaps (by design, Balazs only): billing; merges; re-pins (the wizard); CI activation\" (position 41); " +
                   "a fact that shows through one of those alone is out of an agent's reach until he acts.",
            Reason: "At muster he can still say how work goes on in the dark; later the same gap may meet him asleep.")
    };

    public static readonly Claim Plain = new("A question's block is in plain language: a term he has not used is defined where it first appears, or replaced.", Asking,
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterQuestions, 2109, "Write the block in plain language: define a term he has not used where it first appears, or replace it with plain words that mean the same.")])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Run(Night + "Q2 asked him about \"Mutation's request to raise the output limit to 4 MB for the operator only\", and added that " +
                                 "\"the admiral hasn't shown the run would fit\" (position 44). His reply: \"I don't understand the question. Mutation for what?\" (position 50)."),
            Layer: "The Mutation coordinator's report used its Project's words, \"operator-only4MB proposal\" and \"forward fit\" (position 42); the chief of staff's question carried \"Mutation's request\", \"the operator\" and \"fit\" to him unexplained (position 44).",
            Reason: "Each word he has to ask about costs a round of asking and answering.")
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
            Layer: "The Network coordinator's report offered raising the budget or waiting for the reset (position 41); neither it nor the question named Blacksmith (positions 41 and 44), and he raised it himself (position 50).",
            Reason: "Where a way round exists, finding it first spares him a request for cash or consent.")
    };

    public static readonly Claim Form = new("A question's block: the grilling skill's markers, with what the muster puts between them.", [Actor.ChiefOfStaff],
        [At(Actor.ChiefOfStaff, ChiefOfStaffSlot.MusterBlock, 2111, Block)])
    {
        Spec = ["K11", "H24"],
        Truth = new(Evidence.Code(Grilling),
            Layer: "The `grilling` skill gives the format for a grilling session, with placeholders for a question's title, its body and the recommended answer; what a muster question puts in each stands here.",
            Reason: "Shown whole, each emoji and rule can be copied in place, where a description in prose would have to be rebuilt.")
    };

    private static Line At(Actor reader, Enum slot, int order, string text) => new(Skill.ChiefOfStaff, reader, text, new(slot, order));
}

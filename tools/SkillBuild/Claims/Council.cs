namespace DynaDocs.SkillBuild;

/// <summary>The council (M14): one question to sealed, read-only, cross-family seats, synthesised by a non-member; the reviewer's council resource.</summary>
public static class Council
{
    public static readonly Claim FamilyOut = new("Merge confidence waits for both families only while nothing blocks on it.", [Actor.Admiral], [])
    { Spec = ["L22"], Transition = new(CouncilState.Convened, CouncilState.Waiting, "Merge confidence with one family out") };

    public static readonly Claim Proceed = new("Once something blocks on it, Merge confidence runs single-family, never escalated to him.", [Actor.Admiral], [])
    { Spec = ["L22"], Transition = new(CouncilState.Waiting, CouncilState.Answered, "something blocks: it runs on one family") };

    public static readonly Claim Answer = new("Every member answers the same question alone, sealed and read-only.", [Actor.Reviewer], [])
    { Spec = ["L14", "L15", "L16"], Transition = new(CouncilState.Convened, CouncilState.Answered, "every member answers alone, sealed") };

    public static readonly Claim Synthesise = new("A non-member synthesises the answer, its confidence and the disagreements.", [Actor.Reviewer], [])
    { Spec = ["L14"], Transition = new(CouncilState.Answered, CouncilState.Synthesised, "a non-member synthesises") };

    public static readonly Claim Uses = new("A council has four uses; the Evaluate use keeps the /10 score.", [Actor.Reviewer],
        [new(Skill.Reviewer, "A council has four uses: Review (high stakes), Merge confidence (the landing into main), Evaluate (hill-climbs, scored out of 10) and Judge (the tribunal).", new(Section.CouncilUses, 1301))])
    { Spec = ["L16", "L23"] };

    public static readonly Claim Once = new("Deterministic work runs once.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Deterministic work runs once for the whole council, never per member.", new(Section.CouncilUses, 1302))])
    { Spec = ["L16"] };

    public static readonly Claim Member = new("A member answers alone, sealed and read-only, on the same thing as every other member.", [Actor.Reviewer],
        [new(Skill.Reviewer, "As a member, answer the one question alone, sealed and read-only; every member looks at the same thing.", new(Section.CouncilMember, 1303))])
    { Spec = ["L14", "L16"] };

    public static readonly Claim Synthesis = new("The synthesiser is a non-member and gives answer, confidence and disagreements.", [Actor.Reviewer],
        [new(Skill.Reviewer, "As the synthesiser, a non-member, give the answer, its confidence (agreed, split or contested) and the disagreements.", new(Section.CouncilSynthesis, 1304))])
    { Spec = ["L14"] };

    public static readonly Claim Seats = new("The verdict names each seat.", [Actor.Reviewer],
        [new(Skill.Reviewer, "The verdict names each seat's model and provider.", new(Section.CouncilSynthesis, 1305))])
    { Spec = ["S8", "L22"] };

    public static readonly Claim OneFamily = new("With one family out, work goes on and the council runs on what is left.", [Actor.Reviewer],
        [new(Skill.Reviewer, "With one family out, Review and Evaluate run on two different models of the remaining family, and Judge runs on one family.", new(Section.CouncilOneFamily, 1306))])
    { Spec = ["L22", "L15"] };

    public static readonly Claim MergeOneFamily = new("Merge confidence waits for both families only while nothing blocks, then runs on one, never escalated.", [Actor.Admiral],
        [new(Skill.Admiral, "With one family out, Merge confidence waits for both families only while nothing blocks on it, then runs on one; never take a missing family to him.", new(LandingState.MergeReview, 1307, MoveKind.Nominal))])
    { Spec = ["L22"] };
}

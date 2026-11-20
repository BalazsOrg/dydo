namespace DynaDocs.SkillBuild;

/// <summary>How the law grows (M16): evidence, amendment candidates, F11's test, his principles, retirement, and case studies.</summary>
public static class Law
{
    public static readonly Claim Departs = new("An answer of his that departs from his precedent or his law is an amendment candidate.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Treat an answer of his that departs from his precedent or his law as an amendment candidate, never noise.", new(LawState.Evidence, 1501, MoveKind.Exit))])
    { Spec = ["L7", "L11"], Transition = new(LawState.Evidence, LawState.Candidate, "his answer departs from his precedent or his law, or other evidence arrives") };

    public static readonly Claim Mechanical = new("What no reasonable person would choose otherwise becomes a test, lint or gate.", [Actor.Coordinator],
        [new(Skill.Coordinator, "If no reasonable person could choose otherwise, make it a test, lint or gate; those are the agents'.", new(LawState.Candidate, 1502, MoveKind.Exit))])
    { Spec = ["L7", "L13"], Transition = new(LawState.Candidate, LawState.Mechanical, "no reasonable person would choose otherwise") };

    public static readonly Claim Propose = new("A choice a reasonable person could make otherwise is a principle, proposed to him in a batch.", [Actor.Coordinator],
        [new(Skill.Coordinator, "If a reasonable person could choose otherwise and still be right, propose a principle; batch proposals per Project into a Milestone walkthrough or one envoy bundle.", new(LawState.Candidate, 1503, MoveKind.Exit))])
    { Spec = ["L7", "L13"], Transition = new(LawState.Candidate, LawState.Proposed, "a reasonable person could choose otherwise and still be right") };

    public static readonly Claim Enact = new("Principles and axioms are his only; a slug is never renamed or reused.", [Actor.Balazs], [])
    { Spec = ["L13", "L1", "L2", "L3", "L5", "L6", "L10"], Transition = new(LawState.Proposed, LawState.Enacted, "he enacts it under a new slug") };

    public static readonly Claim Retire = new("A retired or superseded principle moves to retired.md with its reason.", [Actor.Balazs], [])
    { Spec = ["L11", "L13"], Transition = new(LawState.Enacted, LawState.Retired, "he retires or supersedes it") };

    public static readonly Claim Precedential = new("Agents prepare a case study when a call looks precedential.", [Actor.Coordinator],
        [new(Skill.Coordinator, "When a call looks precedential, prepare a case study in git under `cases/` linking its Linear origin, for his approval.", new(LawState.Evidence, 1506, MoveKind.Exit))])
    { Spec = ["L9", "L8"], Transition = new(LawState.Evidence, LawState.CaseStudy, "a call looks precedential") };

    public static readonly Claim Approve = new("He approves a case study; nothing is generated.", [Actor.Balazs], [])
    { Spec = ["L9", "L4"], Transition = new(LawState.CaseStudy, LawState.CaseLaw, "he approves it") };
}

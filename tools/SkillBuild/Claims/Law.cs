namespace DynaDocs.SkillBuild;

/// <summary>How the law grows (M16): evidence, amendment candidates, F11's test, his principles, retirement, and case studies.</summary>
public static class Law
{
    // DYD-1285 took the coordinator's law lines off (H5): evidence and candidates need judgement, so these moves have no carrier yet.
    public static readonly Claim Departs = new("An answer of his that departs from his precedent or his law is an amendment candidate.", [], [])
    { Spec = ["L7", "L11"], Transition = new(LawState.Evidence, LawState.Candidate, "his answer departs from his precedent or his law, or other evidence arrives") };

    public static readonly Claim Mechanical = new("What no reasonable person would choose otherwise becomes a test, lint or gate.", [], [])
    { Spec = ["L7", "L13"], Transition = new(LawState.Candidate, LawState.Mechanical, "no reasonable person would choose otherwise") };

    public static readonly Claim Propose = new("A choice a reasonable person could make otherwise is a principle, proposed to him in a batch.", [], [])
    { Spec = ["L7", "L13"], Transition = new(LawState.Candidate, LawState.Proposed, "a reasonable person could choose otherwise and still be right") };

    public static readonly Claim Enact = new("Principles and axioms are his only; a slug is never renamed or reused.", [Actor.Balazs], [])
    { Spec = ["L13", "L1", "L2", "L3", "L5", "L6", "L10"], Transition = new(LawState.Proposed, LawState.Enacted, "he enacts it under a new slug") };

    public static readonly Claim Retire = new("A retired or superseded principle moves to retired.md with its reason.", [Actor.Balazs], [])
    { Spec = ["L11", "L13"], Transition = new(LawState.Enacted, LawState.Retired, "he retires or supersedes it") };

    public static readonly Claim Precedential = new("Agents prepare a case study when a call looks precedential.", [], [])
    { Spec = ["L9", "L8"], Transition = new(LawState.Evidence, LawState.CaseStudy, "a call looks precedential") };

    public static readonly Claim Approve = new("He approves a case study; nothing is generated.", [Actor.Balazs], [])
    { Spec = ["L9", "L4"], Transition = new(LawState.CaseStudy, LawState.CaseLaw, "he approves it") };
}

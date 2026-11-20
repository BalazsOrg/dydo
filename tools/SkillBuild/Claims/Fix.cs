namespace DynaDocs.SkillBuild;

/// <summary>A fix hop's rules (N12 DR 058 §3–4): severities checked against the source, and the change that narrows.</summary>
public static class Fix
{
    public static readonly Claim Severity = new("The fix-hop author checks each finding's severity against the source and fixes only blockers.", [Actor.Implementer],
        [new(Skill.Implementer, "On a Fix, check each blocker's severity against the source, not the label; fix only the blockers, and put a label you dispute in your return with its evidence (file:line, test, output).", new(ImplementerPart.Build, 2501))])
    { Spec = ["N12 DR 058 §3"] };

    public static readonly Claim Narrow = new("A fix hop prefers the change that deletes or narrows code; review never grows the PR.", [Actor.Implementer],
        [new(Skill.Implementer, "On a Fix, prefer the change that deletes or narrows code; review never grows the change.", new(ImplementerPart.Build, 2502))])
    { Spec = ["N12 DR 058 §4"] };
}

namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a line in the production muster resource naming a command the CLI does not have.</summary>
public static class MusterUsage
{
    public static readonly Claim Line = new("Usage is read at muster.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, Actor.ChiefOfStaff, "Read `dydo usage` at muster.", new(ChiefOfStaffResource.Muster, 2099))]) { Spec = ["K11"], Truth = Verified.ByRun };
}

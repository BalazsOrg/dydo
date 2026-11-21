namespace DynaDocs.Tests.SkillBuild.Truths;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: a claim with no truth fields, stated in the production muster resource.</summary>
public static class MusterBare
{
    public static readonly Claim Line = new("The muster is opened with a bell.", [Actor.ChiefOfStaff],
        [new(Skill.ChiefOfStaff, Actor.ChiefOfStaff, "Open the muster with a bell.", new(ChiefOfStaffResource.Muster, 2099))]) { Spec = ["K11"] };
}

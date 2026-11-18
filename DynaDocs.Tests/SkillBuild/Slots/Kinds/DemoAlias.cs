namespace DynaDocs.Tests.SkillBuild.Slots;

using DynaDocs.SkillBuild;

/// <summary>A broken kind: one claim object behind two fields, so it would have two ids.</summary>
public static class DemoAlias
{
    public static readonly Claim Original = new("Have one id.", [DemoActor.Crew], []);

    public static readonly Claim Alias = Original;
}

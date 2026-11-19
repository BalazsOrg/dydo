namespace DynaDocs.SkillBuild;

/// <summary>A new DR's slugs checked (the SlugCheck machine, L8, §4 check 6): dydo check validates its applies: and weighed: slugs.</summary>
public static class Slugs
{
    public static readonly Claim Valid = new("dydo check passes a DR whose applies: and weighed: slugs are valid.", [Actor.DydoCheck], [])
    { Spec = ["L8"], Transition = new(SlugCheckState.Written, SlugCheckState.Valid, "every applies: and weighed: slug is valid: dydo check passes") };

    public static readonly Claim Invalid = new("An unknown or malformed slug fails dydo check.", [Actor.DydoCheck], [])
    { Spec = ["L8"], Transition = new(SlugCheckState.Written, SlugCheckState.Invalid, "an unknown or malformed slug: dydo check fails") };

    public static readonly Claim Corrected = new("A DR whose slug failed is corrected and checked again.", [], [])
    { Spec = ["L8"], Transition = new(SlugCheckState.Invalid, SlugCheckState.Written, "the slug is corrected; dydo check runs again") };
}

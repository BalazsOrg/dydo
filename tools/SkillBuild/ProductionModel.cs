namespace DynaDocs.SkillBuild;

/// <summary>
/// The model <c>dotnet run --project tools/SkillBuild</c> builds. It declares nothing until DYD-1082
/// writes the 4.0.0 claims (K22): each kind class goes in <c>Claims/&lt;Kind&gt;.cs</c>.
/// </summary>
public static class ProductionModel
{
    public static Model Model { get; } = new([], []);
}
